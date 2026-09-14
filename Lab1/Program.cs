using System;
using System.Globalization;

// Лабораторная работа 1. Вариант 4.

class Program
{
    static void Main()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Лабораторная работа 1 — вариант 4");
            Console.WriteLine("1 - Факториал");
            Console.WriteLine("2 - Последовательность Фибоначчи");
            Console.WriteLine("3 - Значение функции A");
            Console.WriteLine("4 - Ряд Тейлора (ln(1+x))");
            Console.WriteLine("0 - Выход");
            Console.Write("Выберите задание: ");

            string input = Console.ReadLine();
            int choice;
            bool ok = int.TryParse(input, out choice);

            if (ok == false)
            {
                Console.WriteLine("Ошибка: введите целое число.");
                continue;
            }

            if (choice == 1)
            {
                Task1();
            }
            else if (choice == 2)
            {
                Task2();
            }
            else if (choice == 3)
            {
                Task3();
            }
            else if (choice == 4)
            {
                Task4();
            }
            else if (choice == 0)
            {
                return;
            }
            else
            {
                Console.WriteLine("Такого пункта меню нет.");
            }
        }
    }

    static void Task1()
    {
        Console.Write("Введите n (0...20): ");
        string input = Console.ReadLine();
        int n;
        bool ok = int.TryParse(input, out n);

        if (ok == false || n < 0 || n > 20)
        {
            Console.WriteLine("Ошибка: нужно целое число от 0 до 20.");
            return;
        }

        long result = Factorial(n);
        Console.WriteLine(n + "! = " + result);
    }

    static long Factorial(int n)
    {
        long result = 1;
        for (int i = 2; i <= n; i++)
        {
            result = result * i;
        }
        return result;
    }

    static void Task2()
    {
        Console.Write("Введите n: ");
        string input = Console.ReadLine();
        int n;
        bool ok = int.TryParse(input, out n);

        if (ok == false || n < 0)
        {
            Console.WriteLine("Ошибка: нужно неотрицательное целое число.");
            return;
        }

        string result = "";
        long a = 0;
        long b = 1;

        while (a <= n)
        {
            if (result == "")
            {
                result = a.ToString();
            }
            else
            {
                result = result + ", " + a.ToString();
            }

            long next = a + b;
            a = b;
            b = next;
        }

        Console.WriteLine(result);
    }

    static void Task3()
    {
        Console.Write("Введите x: ");

        if (!double.TryParse(Console.ReadLine(), out double x))
        {
            Console.WriteLine("Ошибка: введите вещественное число.");
            return;
        }

        if (x < 54)
        {
            Console.WriteLine("Функция не определена: под корнем отрицательное число (нужно x >= 54).");
            return;
        }

        double sinX2 = Math.Sin(x * x);

        if (sinX2 == 0)
        {
            Console.WriteLine("Функция не определена: деление на ноль (sin(x^2) = 0).");
            return;
        }

        double a = Math.Sqrt(x - 54) + Math.Cos(x / 2) / sinX2 - Math.Log(x);
        Console.WriteLine($"A = {a}");
    }