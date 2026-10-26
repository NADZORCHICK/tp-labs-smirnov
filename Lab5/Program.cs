using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

class Program
{
    const int TextLength = 5000000;

    static void Main()
    {
        CaesarCipher cipher = new CaesarCipher();
        CaesarBreaker breaker = new CaesarBreaker();

        Console.WriteLine("Лабораторная работа 5 (взлом шифра Цезаря)");
        Console.WriteLine("Ядер процессора: " + Environment.ProcessorCount);

        // Слово вводит пользователь
        string knownWord = ReadWord("Введите известное слово (русскими буквами, не короче 5 букв): ");

        string original = BuildText(TextLength, knownWord);
        int secretKey = ReadInt("Введите ключ шифрования (1-32): ", 1, 32);
        string encrypted = cipher.Encrypt(original, secretKey);
        Console.WriteLine("Текст зашифрован. Начало: " + encrypted.Substring(0, 50));

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("1 - Один запуск (число потоков задаёте вы)");
            Console.WriteLine("2 - Таблица замеров для 1, 2, 4, 8 потоков");
            Console.WriteLine("0 - Выход");

            int choice = ReadInt("Выберите пункт: ", 0, 2);

            if (choice == 1)
            {
                int threads = ReadInt("Число потоков (1-64): ", 1, 64);
                RunOnce(cipher, breaker, encrypted, knownWord, threads);
            }
            else if (choice == 2)
            {
                RunTable(breaker, encrypted, knownWord);
            }
            else
            {
                return;
            }
        }
    }

    // последовательная версия, параллельная версия, сравнение.
    static void RunOnce(CaesarCipher cipher, CaesarBreaker breaker, string encrypted, string knownWord, int threads)
    {
        Stopwatch sw = Stopwatch.StartNew();
        List<int> seqKeys = breaker.FindKeysSequential(encrypted, knownWord);
        sw.Stop();
        long seqTime = sw.ElapsedMilliseconds;

        sw.Restart();
        List<int> parKeys = breaker.FindKeysParallel(encrypted, knownWord, threads);
        sw.Stop();
        long parTime = sw.ElapsedMilliseconds;

        Console.WriteLine("Последовательно: ключи " + KeysToString(seqKeys) + ", " + seqTime + " мс");
        Console.WriteLine("Параллельно (" + threads + " потоков): ключи " + KeysToString(parKeys) + ", " + parTime + " мс");

        if (KeysEqual(seqKeys, parKeys))
        {
            Console.WriteLine("Результаты совпадают.");
        }
        else
        {
            Console.WriteLine("ОШИБКА: результаты разные!");
        }

        Console.WriteLine("Ускорение: " + Speedup(seqTime, parTime));

        if (parKeys.Count > 0)
        {
            string start = cipher.Decrypt(encrypted.Substring(0, 50), parKeys[0]);
            Console.WriteLine("Расшифровка (начало): " + start);
        }
    }

    static void RunTable(CaesarBreaker breaker, string encrypted, string knownWord)
    {
        Stopwatch sw = Stopwatch.StartNew();
        List<int> seqKeys = breaker.FindKeysSequential(encrypted, knownWord);
        sw.Stop();
        long seqTime = sw.ElapsedMilliseconds;
        Console.WriteLine("Последовательно: " + seqTime + " мс");
        Console.WriteLine();
        Console.WriteLine("Потоков | Время, мс | Ускорение | Результат");

        int[] counts = new int[] { 1, 2, 4, 8 };
        for (int i = 0; i < counts.Length; i++)
        {
            sw.Restart();
            List<int> parKeys = breaker.FindKeysParallel(encrypted, knownWord, counts[i]);
            sw.Stop();
            long parTime = sw.ElapsedMilliseconds;

            string status = "совпадает";
            if (KeysEqual(seqKeys, parKeys) == false)
            {
                status = "ОШИБКА";
            }

            Console.WriteLine(counts[i] + " | " + parTime + " | " + Speedup(seqTime, parTime) + " | " + status);
        }
    }

    static string Speedup(long seqTime, long parTime)
    {
        if (parTime == 0)
        {
            return "—";
        }

        double value = (double)seqTime / parTime;
        return Math.Round(value, 2) + "x";
    }

    static bool KeysEqual(List<int> a, List<int> b)
    {
        if (a.Count != b.Count)
        {
            return false;
        }

        for (int i = 0; i < a.Count; i++)
        {
            if (a[i] != b[i])
            {
                return false;
            }
        }

        return true;
    }

    static string KeysToString(List<int> keys)
    {
        if (keys.Count == 0)
        {
            return "не найден";
        }

        return string.Join(", ", keys);
    }

    static string BuildText(int length, string word)
    {
        string phrase = "Я люблю " + word + " на языке C# и изучаю потоки. ";
        StringBuilder sb = new StringBuilder();

        while (sb.Length < length)
        {
            sb.Append(phrase);
        }

        return sb.ToString();
    }

    static string ReadWord(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? line = Console.ReadLine();

            if (line != null)
            {
                string word = line.Trim();
                if (word.Length >= 5 && word.Contains(' ') == false)
                {
                    return word;
                }
            }

            Console.WriteLine("Нужно одно слово длиной не меньше 5 символов.");
        }
    }

    // Читает целое число от min до max. Если ввод неверный — спрашивает снова.
    static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            string? line = Console.ReadLine();

            int value;
            if (line != null && int.TryParse(line, out value) && value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine("Введите целое число от " + min + " до " + max + ".");
        }
    }
}