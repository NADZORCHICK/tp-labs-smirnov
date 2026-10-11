using System;

// только ввод и вывод.
class Program
{
    static void Main()
    {
        FileCopier copier = new FileCopier();

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Лабораторная работа 3 — вариант 4");
            Console.WriteLine("1 - Копировать файл с переименованием");
            Console.WriteLine("0 - Выход");

            string choice = ReadInput("Выберите пункт: ");

            if (choice == "1")
            {
                CopyFile(copier);
            }
            else if (choice == "0")
            {
                return;
            }
            else
            {
                Console.WriteLine("Такого пункта меню нет.");
            }
        }
    }

    static void CopyFile(FileCopier copier)
    {
        string source = ReadInput("Путь к исходному файлу: ");
        string folder = ReadInput("Папка назначения: ");
        string newName = ReadInput("Новое имя файла: ");

        // проверяются введённые данные.
        string error = copier.Check(source, folder, newName);
        if (error != "")
        {
            Console.WriteLine("Ошибка: " + error);
            return;
        }

        // Если такой файл уже есть — перезаписывается после подтверждения.
        bool overwrite = false;
        if (copier.DestinationFileExists(folder, newName))
        {
            string answer = ReadInput("Файл с таким именем уже существует. Перезаписать? (д/н): ").ToLower();

            if (answer == "д")
            {
                overwrite = true;
            }
            else
            {
                Console.WriteLine("Копирование отменено.");
                return;
            }
        }

        error = copier.Copy(source, folder, newName, overwrite);
        if (error == "")
        {
            Console.WriteLine("Файл успешно скопирован.");
        }
        else
        {
            Console.WriteLine("Ошибка: " + error);
        }
    }

    // Чтобы не дублировать ввод и не получить null.
    static string ReadInput(string prompt)
    {
        Console.Write(prompt);
        string? line = Console.ReadLine();

        if (line == null)
        {
            return "";
        }

        return line;
    }
}