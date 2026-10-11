using System;
public class Book : Publication
{
    private int _pages;

    public int Pages
    {
        get { return _pages; }
    }

    public Book(string title, string author, int year, int pages)
        : base(title, author, year)
    {
        if (pages <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pages), "Число страниц должно быть больше нуля.");
        }
        _pages = pages;
    }
    public override int GetLoanDays()
    {
        return 30;
    }
    public override string ToString()
    {
        return "Книга: " + base.ToString() + ", стр.: " + _pages;
    }
}