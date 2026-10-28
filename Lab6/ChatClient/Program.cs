using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

class Program
{
    const int Port = 5555;

    static bool exiting = false;

    static async Task Main()
    {
        Console.Write("Адрес сервера (Enter = localhost): ");
        string? host = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(host))
        {
            host = "localhost";
        }

        TcpClient client = new TcpClient();

        try
        {
            await client.ConnectAsync(host, Port);
        }
        catch (Exception ex)
        {
            // Сервер выключен, неверный адрес и тд
            Console.WriteLine("Не удалось подключиться к серверу: " + ex.Message);
            return;
        }

        NetworkStream stream = client.GetStream();
        StreamReader reader = new StreamReader(stream);
        StreamWriter writer = new StreamWriter(stream);
        writer.AutoFlush = true;

        try
        {
            // Представляемся серверу. Если ник занят, сервер ответит ошибкой —
            while (true)
            {
                Console.Write("Ваш ник: ");
                string? nick = Console.ReadLine();
                if (nick == null)
                {
                    nick = "anon";
                }

                await writer.WriteLineAsync(nick);
                string? answer = await reader.ReadLineAsync();

                if (answer == null)
                {
                    Console.WriteLine("Сервер закрыл соединение.");
                    return;
                }

                if (answer == "OK")
                {
                    break;
                }

                if (answer.StartsWith("ERR "))
                {
                    Console.WriteLine(answer.Substring(4));
                }
                else
                {
                    Console.WriteLine(answer);
                }
            }

            // Приём сообщений одновременно с вводом.
            _ = Task.Run(async () =>
            {
                try
                {
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }
                catch (Exception)
                {
                    // соединение оборвалось
                }

                if (exiting == false)
                {
                    Console.WriteLine("Соединение с сервером потеряно.");
                    Environment.Exit(0);
                }
            });

            Console.WriteLine("Подключено. Команды: /list, /w ник текст, /exit.");
            while (true)
            {
                string? msg = Console.ReadLine();
                if (msg == null)
                {
                    break;
                }

                if (msg.Trim() == "")
                {
                    continue;
                }

                await writer.WriteLineAsync(msg);

                if (msg.Trim() == "/exit")
                {
                    break;
                }
            }
        }
        catch (IOException)
        {
            Console.WriteLine("Соединение с сервером потеряно.");
        }
        finally
        {
            exiting = true;
            client.Close();
        }
    }
}
