using System;
public class Magazine : Publication
{
    private int _issueNumber;

    public int IssueNumber
    {
        get { return _issueNumber; }
    }

    public Magazine(string title, string author, int year, int issueNumber)
        : base(title, author, year)
    {
        if (issueNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(issueNumber), "Номер выпуска должен быть больше нуля.");
        }
        _issueNumber = issueNumber;
    }

    public override int GetLoanDays()
    {
        return 14;
    }

    public override string ToString()
    {
        return "Журнал: " + base.ToString() + ", выпуск №" + _issueNumber;
    }
}