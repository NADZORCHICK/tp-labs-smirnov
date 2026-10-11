using System;
using System.Collections.Generic;

// Здесь только создание объектов и вывод в консоль.
class Program
{
    static void Main()
    {
        Library library = new Library();

        library.Add(new Book("Война и мир", "Толстой", 1869, 1225));
        library.Add(new Book("Анна Каренина", "Толстой", 1877, 864));
        library.Add(new Book("Мастер и Маргарита", "Булгаков", 1967, 480));
        library.Add(new Magazine("Наука и жизнь", "Редакция", 2024, 5));
        library.Add(new Magazine("Квант", "Редакция", 2023, 11));
        library.Add(new ElectronicEdition("Clean Code", "Martin", 2008, "PDF", 4.5));

        Console.WriteLine("Каталог библиотеки -------------");
        foreach (Publication p in library.GetAll())
        {
            Console.WriteLine(p);
            Console.WriteLine("   срок выдачи: " + p.GetLoanDays() + " дн.");
        }

        Console.WriteLine();
        Console.WriteLine("Поиск по автору: Толстой ------------- ");
        List<Publication> found = library.FindByAuthor("Толстой");
        foreach (Publication p in found)
        {
            Console.WriteLine(p);
        }

        Console.WriteLine();
        Console.WriteLine("Выдача и возврат -------------");
        Console.WriteLine("В наличии: " + library.CountAvailable() + " из " + library.Count);

        int days = library.IssueByTitle("Война и мир");
        Console.WriteLine("Выдана книга \"Война и мир\" на " + days + " дн.");
        Console.WriteLine("В наличии: " + library.CountAvailable() + " из " + library.Count);

        library.ReturnByTitle("Война и мир");
        Console.WriteLine("Книга возвращена.");
        Console.WriteLine("В наличии: " + library.CountAvailable() + " из " + library.Count);

        
    }
}