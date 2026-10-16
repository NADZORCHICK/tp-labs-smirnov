using System;
using System.Collections.Generic;
using Lab4.Core;
using Xunit;

namespace Lab4.Tests;

// Тесты классов из лабораторной 2 (вариант 4 — Библиотека).
public class LibraryTests
{
    // Каждый тест получает свою новую библиотеку(тесты не зависят друг от друга).
    private static Library CreateLibrary()
    {
        Library library = new Library();
        library.Add(new Book("Война и мир", "Толстой", 1869, 1225));
        library.Add(new Book("Анна Каренина", "Толстой", 1877, 864));
        library.Add(new Book("Мастер и Маргарита", "Булгаков", 1967, 480));
        library.Add(new Magazine("Квант", "Редакция", 2023, 11));
        return library;
    }

    //Защита инвариантов

    [Fact]
    public void Book_NegativePages_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Book("Война и мир", "Толстой", 1869, -5));
    }

    //Полиморфизм

    [Fact]
    public void GetLoanDays_ForEachType_ReturnsDifferentValues()
    {
        Publication[] items = new Publication[]
        {
            new Book("Война и мир", "Толстой", 1869, 1225),
            new Magazine("Квант", "Редакция", 2023, 11),
            new ElectronicEdition("Clean Code", "Martin", 2008, "PDF", 4.5)
        };
        List<int> days = new List<int>();

        foreach (Publication item in items)
        {
            days.Add(item.GetLoanDays());
        }

        Assert.Equal(new int[] { 30, 14, 7 }, days);
    }

    //Выдача и возврат (изменение состояния)

    [Fact]
    public void Issue_AvailablePublication_MarksAsIssued()
    {
        Book book = new Book("Война и мир", "Толстой", 1869, 1225);

        book.Issue();

        Assert.True(book.IsIssued);
    }

    [Fact]
    public void Issue_AlreadyIssued_Throws()
    {
        Book book = new Book("Война и мир", "Толстой", 1869, 1225);
        book.Issue();

        Assert.Throws<InvalidOperationException>(() => book.Issue());
    }

    [Fact]
    public void Return_IssuedPublication_MarksAsAvailable()
    {
        Book book = new Book("Война и мир", "Толстой", 1869, 1225);
        book.Issue();

        book.Return();

        Assert.False(book.IsIssued);
    }

    [Fact]
    public void Return_NotIssued_Throws()
    {
        Book book = new Book("Война и мир", "Толстой", 1869, 1225);

        Assert.Throws<InvalidOperationException>(() => book.Return());
    }

    //Library 

    [Fact]
    public void FindByAuthor_EmptyLibrary_ReturnsEmptyList()
    {
        Library library = new Library();

        List<Publication> found = library.FindByAuthor("Толстой");

        Assert.Empty(found);
    }

    [Fact]
    public void FindByAuthor_ExistingAuthor_ReturnsOnlyHisPublications()
    {
        Library library = CreateLibrary();

        List<Publication> found = library.FindByAuthor("Толстой");

        Assert.Equal(2, found.Count);
    }

    [Fact]
    public void IssueByTitle_ExistingTitle_ReturnsLoanDaysAndDecreasesAvailable()
    {
        Library library = CreateLibrary();

        int days = library.IssueByTitle("Квант");

        Assert.Equal(14, days);
        Assert.Equal(3, library.CountAvailable());
    }

    [Fact]
    public void IssueByTitle_UnknownTitle_Throws()
    {
        Library library = CreateLibrary();

        Assert.Throws<ArgumentException>(() => library.IssueByTitle("Несуществующая книга"));
    }
}