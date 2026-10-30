using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ConsoleTables;
using Converter.Core;

class Program
{
    static void Main()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "custom_units.txt");
        UnitConverter converter = CreateConverter(path);

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Конвертер величин — вариант 4");
            Console.WriteLine("1 - Конвертировать значение");
            Console.WriteLine("2 - Показать таблицу коэффициентов");
            Console.WriteLine("3 - Добавить свою единицу");
            Console.WriteLine("0 - Выход");

            int choice = ReadInt("Выберите пункт: ", 0, 3);

            if (choice == 1)
            {
                ConvertValue(converter);
            }
            else if (choice == 2)
            {
                ShowTable(converter);
            }
            else if (choice == 3)
            {
                AddUnit(converter);
            }
            else
            {
                return;
            }

        }
    }
    static UnitConverter CreateConverter(string path)
    {
        try
        {
            return new UnitConverter(new FileUnitStorage(path));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Не удалось прочитать файл с единицами: " + ex.Message);
            Console.WriteLine("Работаем без сохранения: добавленные единицы пропадут после выхода.");
            return new UnitConverter(new InMemoryUnitStorage());
        }
    }
    static void ConvertValue(UnitConverter converter)
    {
        string quantity = ChooseQuantity(converter);
        Console.WriteLine("Доступные единицы: " + string.Join(", ", converter.GetUnits(quantity)));

        string fromUnit = ReadInput("Из какой единицы: ");
        string toUnit = ReadInput("В какую единицу: ");
        double value = ReadDouble("Значение: ");
        try
        {
            double result = converter.Convert(quantity, fromUnit, toUnit, value);
            Console.WriteLine(value + " " + fromUnit + " = " + Math.Round(result, 6) + " " + toUnit);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("Ошибка: " + ex.Message);
        }
    }
    static void ShowTable(UnitConverter converter)
    {
        foreach (string quantity in converter.GetQuantities())
        {
            Console.WriteLine();

            if (quantity == UnitConverter.Temperature)
            {
                Console.WriteLine("Температура: C, F, K (пересчёт по формулам, коэффициентов нет)");
                continue;
            }
            Console.WriteLine(quantity + " (базовая единица: " + converter.GetBaseUnit(quantity) + ")");

            ConsoleTable table = new ConsoleTable("Единица", "Коэффициент");
            foreach (string unit in converter.GetUnits(quantity))
            {
                table.AddRow(unit, converter.GetFactor(quantity, unit));
            }

            table.Write();
        }
    }
    static void AddUnit(UnitConverter converter)
    {
        string quantity = ChooseQuantity(converter);

        if (quantity == UnitConverter.Temperature)
        {
            Console.WriteLine("Для температуры свои единицы добавлять нельзя.");
            return;
        }
        string baseUnit = converter.GetBaseUnit(quantity);
        string name = ReadInput("Название новой единицы (без пробелов): ");
        double factor = ReadDouble("Коэффициент (сколько «" + baseUnit + "» в одной вашей единице): ");

        try
        {
            converter.AddUnit(quantity, name, factor);
            Console.WriteLine("Единица добавлена.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("Ошибка: " + ex.Message);
        }
        catch (Exception ex)
        {
            // не удалось записать файл.
            Console.WriteLine("Единица добавлена, но не сохранена в файл: " + ex.Message);
        }
    }
    static string ChooseQuantity(UnitConverter converter)
    {
        List<string> quantities = converter.GetQuantities();

        for (int i = 0; i < quantities.Count; i++)
        {
            Console.WriteLine((i + 1) + " - " + quantities[i]);
        }

        int number = ReadInt("Выберите величину: ", 1, quantities.Count);
        return quantities[number - 1];
    }
    static string ReadInput(string prompt)
    {
        Console.Write(prompt);
        string? line = Console.ReadLine();

        if (line == null)
        {
            return "";
        }

        return line.Trim();
    }
    static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            string text = ReadInput(prompt);

            int value;
            if (int.TryParse(text, out value) && value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine("Введите целое число от " + min + " до " + max + ".");
        }
    }
    static double ReadDouble(string prompt)
    {
        while (true)
        {
            string text = ReadInput(prompt).Replace(',', '.');

            double value;
            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            {
                return value;
            }

            Console.WriteLine("Введите число.");
        }
    }
}
