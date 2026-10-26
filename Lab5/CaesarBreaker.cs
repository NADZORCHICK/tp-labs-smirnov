using System;
using System.Collections.Generic;
using System.Threading;

// Взлом шифра Цезаря перебором ключей.
public class CaesarBreaker
{
    private CaesarCipher _cipher = new CaesarCipher();

    // расшифровываем весь текст и ищем в нём известное слово.
    private bool CheckKey(string encrypted, string knownWord, int key)
    {
        string decrypted = _cipher.Decrypt(encrypted, key);
        return decrypted.Contains(knownWord, StringComparison.OrdinalIgnoreCase);
    }

    // один поток по очереди проверяет все ключи.
    public List<int> FindKeysSequential(string encrypted, string knownWord)
    {
        List<int> found = new List<int>();

        for (int key = 0; key < CaesarCipher.AlphabetSize; key++)
        {
            if (CheckKey(encrypted, knownWord, key))
            {
                found.Add(key);
            }
        }

        return found;
    }

    // Параллельная версия: ключи делим между threadCount потоками.
    // Поток с номером t проверяет ключи t, t + threadCount, t + 2 * threadCount и тд
    public List<int> FindKeysParallel(string encrypted, string knownWord, int threadCount)
    {
        List<int> found = new List<int>();
        object lockObj = new object();
        Thread[] threads = new Thread[threadCount];

        for (int t = 0; t < threadCount; t++)
        {
            // Копия номера потока, если использовать сам t, то к моменту запуска
            // потока цикл уже изменит его и потоки получат неверные номера.
            int threadIndex = t;

            threads[t] = new Thread(() =>
            {
                for (int key = threadIndex; key < CaesarCipher.AlphabetSize; key = key + threadCount)
                {
                    if (CheckKey(encrypted, knownWord, key))
                    {
                        // Список общий для всех потоков, поэтому добавляем под lock
                        // одновременно в секции может быть только один поток.
                        lock (lockObj)
                        {
                            found.Add(key);
                        }
                    }
                }
            });

            threads[t].Start();
        }

        for (int t = 0; t < threadCount; t++)
        {
            threads[t].Join();
        }

        found.Sort();
        return found;
    }
}