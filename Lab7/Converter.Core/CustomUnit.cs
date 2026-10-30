namespace Converter.Core;

public class CustomUnit
{
    private string _quantity;
    private string _name;
    private double _factor;

    public string Quantity
    {
        get { return _quantity; }
    }

    public string Name
    {
        get { return _name; }
    }

    public double Factor
    {
        get { return _factor; }
    }
    public CustomUnit(string quantity, string name, double factor)
    {
        _quantity = quantity;
        _name = name;
        _factor = factor;
    }
}