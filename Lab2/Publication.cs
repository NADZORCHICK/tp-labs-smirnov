using System;

public abstract class Publication
{
    private string _title;
    private string _author;
    private int _year;
    private bool _isIssued;


    public string Title
    {
        get { return _title; }
    }

    public string Author
    {
        get { return _author; }
    }

    public int Year
    {
        get { return _year; }
    }

    public bool IsIssued
    {
        get { return _isIssued; }
    }

    public Publication(string title, string author, int year)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Название не может быть пустым.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(author))
        {
            throw new ArgumentException("Автор не может быть пустым.", nameof(author));
        }

        if (year < 1450 || year > DateTime.Now.Year)
        {
            throw new ArgumentOutOfRangeException(nameof(year), "Некорректный год издания.");
        }

        _title = title;
        _author = author;
        _year = year;
        _isIssued = false; 
    }

    public abstract int GetLoanDays();

    public virtual void Issue()
    {
        if (_isIssued)
        {
            throw new InvalidOperationException("Издание уже выдано.");
        }
        _isIssued = true;
    }

    public virtual void Return()
    {
        if (_isIssued == false)
        {
            throw new InvalidOperationException("Издание не было выдано.");
        }
        _isIssued = false;
    }

    public override string ToString()
    {
        string status;
        if (_isIssued)
        {
            status = "выдано";
        }
        else
        {
            status = "в наличии";
        }

        return "\"" + _title + "\", " + _author + ", " + _year + " г. [" + status + "]";
    }
}