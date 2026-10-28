using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

class Program
{
    const string LogFile = "chat.log";

    // Подключённые клиенты: ник

    static Dictionary<string, StreamWriter> clients =
        new Dictionary<string, StreamWriter>(StringComparer.OrdinalIgnoreCase);

    // Словарь общий для всех клиентов
    static object lockObj = new object();
    static object logLock = new object();

    // Порт можно передать аргументом командной строки
    static async Task Main(string[] args)
    {
        int port = 5555;

        if (args.Length > 0)
        {
            if (int.TryParse(args[0], out port) == false || port < 1 || port > 65535)
            {
                Console.WriteLine("Неверный порт. Укажите число от 1 до 65535.");
                return;
            }
        }

        TcpListener listener = new TcpListener(IPAddress.Any, port);

        try
        {
            listener.Start();
        }
        catch (SocketException ex)
        {
            // Например, порт уже занят.
            Console.WriteLine("Не удалось запустить сервер на порту " + port + ": " + ex.Message);
            return;
        }

        WriteLog("Сервер запущен на порту " + port + ". Ctrl+C — остановка.");

        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            _ = HandleClientAsync(client);   // клиент - задача
        }
    }

    static async Task HandleClientAsync(TcpClient client)
    {
        string nick = "";

        try
        {
            EndPoint? endpoint = client.Client.RemoteEndPoint;
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            // Первое сообщение клиента — ник. Спрашиваем, пока не назовёт свой
            while (true)
            {
                string? wanted = await reader.ReadLineAsync();

                if (wanted == null)
                {
                    // Клиент закрылся, так и не назвав ник.
                    return;
                }

                wanted = wanted.Trim();

                if (wanted == "" || wanted.Contains(' '))
                {
                    await SendAsync(writer, "ERR Ник не может быть пустым или содержать пробелы.");
                }
                else if (TryAddClient(wanted, writer))
                {
                    nick = wanted;
                    await SendAsync(writer, "OK");
                    break;
                }
                else
                {
                    await SendAsync(writer, "ERR Ник «" + wanted + "» уже занят. Введите другой.");
                }
            }

            WriteLog(nick + " подключился (" + endpoint + ")");
            await BroadcastAsync("*** " + nick + " вошёл в чат ***");

            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                line = line.Trim();

                if (line == "")
                {
                    continue;
                }

                if (line == "/exit")
                {
                    break;
                }
                else if (line == "/list")
                {
                    await SendListAsync(writer);
                }
                else if (line.StartsWith("/w "))
                {
                    await SendPrivateAsync(nick, writer, line);
                }
                else if (line.StartsWith("/"))
                {
                    await SendAsync(writer, "Неизвестная команда. Доступно: /list, /w ник текст, /exit.");
                }
                else
                {
                    WriteLog(nick + ": " + line);
                    await BroadcastAsync("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + nick + ": " + line);
                }
            }
        }
        catch (IOException)
        {
            // клиент оборвал соединение — сервер продолжает работать
        }
        finally
        {
            // Выполнится в любом случае: и при /exit, и при обрыве связи.
            if (nick != "")
            {
                lock (lockObj)
                {
                    clients.Remove(nick);
                }

                WriteLog(nick + " отключился");
                await BroadcastAsync("*** " + nick + " покинул чат ***");
            }

            client.Close();
        }
    }

    // Добавляет клиента, если ник свободен. Проверка и добавление — lock,
    // иначе два клиента могли бы одновременно занять один и тот же ник.
    static bool TryAddClient(string nick, StreamWriter writer)
    {
        lock (lockObj)
        {
            if (clients.ContainsKey(nick))
            {
                return false;
            }

            clients.Add(nick, writer);
            return true;
        }
    }

    static async Task SendListAsync(StreamWriter writer)
    {
        List<string> names;
        lock (lockObj)
        {
            names = new List<string>(clients.Keys);
        }

        await SendAsync(writer, "Подключены (" + names.Count + "): " + string.Join(", ", names));
    }

    // Команда /w ник текст: лс.
    static async Task SendPrivateAsync(string senderNick, StreamWriter senderWriter, string line)
    {
        // Делим строку на три части: "/w", ник, всё остальное (текст может содержать пробелы).
        string[] parts = line.Split(' ', 3);

        if (parts.Length < 3 || parts[2].Trim() == "")
        {
            await SendAsync(senderWriter, "Формат: /w ник текст");
            return;
        }

        string target = parts[1];
        string text = parts[2].Trim();

        StreamWriter? targetWriter = null;
        lock (lockObj)
        {
            clients.TryGetValue(target, out targetWriter);
        }

        if (targetWriter == null)
        {
            await SendAsync(senderWriter, "Пользователь «" + target + "» не найден.");
            return;
        }

        string time = DateTime.Now.ToString("HH:mm:ss");
        await SendAsync(targetWriter, "[" + time + "] [лично от " + senderNick + "]: " + text);
        await SendAsync(senderWriter, "[" + time + "] [лично для " + target + "]: " + text);
        WriteLog("Личное " + senderNick + " -> " + target + ": " + text);
    }

    // Рассылка сообщения всем клиентам.
    static async Task BroadcastAsync(string message)
    {
        // Копия списка под lock: пока идёт рассылка, кто-то может войти или выйти.
        List<StreamWriter> snapshot;
        lock (lockObj)
        {
            snapshot = new List<StreamWriter>(clients.Values);
        }

        foreach (StreamWriter w in snapshot)
        {
            await SendAsync(w, message);
        }
    }

    // Отправка одной строки одному клиенту.
    static async Task SendAsync(StreamWriter writer, string text)
    {
        try
        {
            await writer.WriteLineAsync(text);
        }
        catch (IOException)
        {
            // Клиент отвалился: его собственная задача сама удалит его из списка.
        }
        catch (ObjectDisposedException)
        {
            // Соединение уже закрыто.
        }
    }

    // логи.
    static void WriteLog(string text)
    {
        string line = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + text;
        Console.WriteLine(line);

        lock (logLock)
        {
            try
            {
                File.AppendAllText(LogFile, line + Environment.NewLine);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Нет прав на запись в журнал " + LogFile);
            }
            catch (IOException ex)
            {
                Console.WriteLine("Не удалось записать в журнал: " + ex.Message);
            }
        }
    }
}
