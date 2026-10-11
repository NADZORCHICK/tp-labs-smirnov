using System;

public class ElectronicEdition : Publication
{
    private string _format;   // PDF или EPUB
    private double _sizeMb;

    public string Format
    {
        get { return _format; }
    }

    public double SizeMb
    {
        get { return _sizeMb; }
    }

    public ElectronicEdition(string title, string author, int year, string format, double sizeMb)
        : base(title, author, year)
    {
        if (string.IsNullOrWhiteSpace(format))
        {
            throw new ArgumentException("Формат файла не может быть пустым.", nameof(format));
        }

        if (sizeMb <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sizeMb), "Размер файла должен быть больше нуля.");
        }

        _format = format;
        _sizeMb = sizeMb;
    }

    public override int GetLoanDays()
    {
        return 7;
    }

    public override string ToString()
    {
        return "Электронное издание: " + base.ToString() + ", " + _format + ", " + _sizeMb + " МБ";
    }
}