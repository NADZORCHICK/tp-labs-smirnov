using Converter.Core;

namespace Converter.Tests;

public class UnitConverterTests
{
    private static UnitConverter CreateConverter()
    {
        return new UnitConverter(new InMemoryUnitStorage());
    }
    [Fact]
    public void Convert_KilometersToMeters_Returns1000()
    { //AAA
        UnitConverter converter = CreateConverter();

        double result = converter.Convert("длина", "км", "м", 1);

        Assert.Equal(1000, result, 1e-9);
    }
    [Theory]
    [InlineData("длина", "миля", "км", 1.0, 1.609344)]
    [InlineData("длина", "м", "см", 2.0, 200.0)]
    [InlineData("масса", "кг", "фунт", 1.0, 2.2046226218487757)]
    [InlineData("скорость", "км/ч", "м/с", 36.0, 10.0)]
    [InlineData("скорость", "узел", "км/ч", 10.0, 18.52)]
    public void Convert_ReturnsExpected(string quantity, string from, string to, double value, double expected)
    {//AAA
        UnitConverter converter = CreateConverter();

        double result = converter.Convert(quantity, from, to, value);

        Assert.Equal(expected, result, 1e-9);
    }
    [Fact]
    public void Convert_SameUnit_ReturnsSameValue()
    {
        UnitConverter converter = CreateConverter();

        double result = converter.Convert("масса", "кг", "кг", 5);

        Assert.Equal(5, result, 1e-9);
    }
    [Fact]
    public void Convert_Zero_ReturnsZero()
    {
        UnitConverter converter = CreateConverter();

        double result = converter.Convert("длина", "м", "км", 0);

        Assert.Equal(0, result, 1e-9);
    }
    [Fact]
    public void Convert_UnitNamesInDifferentCase_AreAccepted()
    {
        UnitConverter converter = CreateConverter();

        double result = converter.Convert("ДЛИНА", "КМ", "М", 1);

        Assert.Equal(1000, result, 1e-9);
    }
    [Fact]
    public void Convert_ThereAndBack_ReturnsOriginalValue()
    {//AAA
        UnitConverter converter = CreateConverter();

        double miles = converter.Convert("длина", "км", "миля", 42);
        double km = converter.Convert("длина", "миля", "км", miles);

        Assert.Equal(42, km, 1e-9);
    }
    [Theory]
    [InlineData("C", "F", 100.0, 212.0)]
    [InlineData("C", "K", 0.0, 273.15)]
    [InlineData("F", "C", 32.0, 0.0)]
    [InlineData("K", "C", 0.0, -273.15)]
    [InlineData("F", "K", 212.0, 373.15)]
    public void ConvertTemperature_ReturnsExpected(string from, string to, double value, double expected)
    {
        UnitConverter converter = CreateConverter();

        double result = converter.Convert("температура", from, to, value);

        Assert.Equal(expected, result, 1e-9);
    }
    [Fact]
    public void ConvertTemperature_AbsoluteZeroInCelsius_IsAllowed()
    {
        UnitConverter converter = CreateConverter();

        double result = converter.Convert("температура", "C", "K", -273.15);

        Assert.Equal(0, result, 1e-9);
    }
    [Theory]
    [InlineData("C", -300.0)]
    [InlineData("K", -1.0)]
    [InlineData("F", -500.0)]
    public void ConvertTemperature_BelowAbsoluteZero_Throws(string from, double value)
    {
        UnitConverter converter = CreateConverter();

        Assert.Throws<ArgumentOutOfRangeException>(() => converter.Convert("температура", from, "C", value));
    }
    [Theory]
    [InlineData("длина", "парсек", "м")]
    [InlineData("длина", "м", "парсек")]
    [InlineData("масса", "м", "кг")]            
    [InlineData("температура", "R", "C")]
    public void Convert_UnknownUnit_Throws(string quantity, string from, string to)
    {
        UnitConverter converter = CreateConverter();

        Assert.Throws<ArgumentException>(() => converter.Convert(quantity, from, to, 1));
    }
    [Fact]
    public void Convert_UnknownQuantity_Throws()
    {
        UnitConverter converter = CreateConverter();

        Assert.Throws<ArgumentException>(() => converter.Convert("объём", "л", "мл", 1));
    }
    [Fact]
    public void Constructor_NullStorage_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new UnitConverter(null!));
    }


    [Fact]
    public void AddUnit_NewUnit_CanBeUsedInConversion()
    {
        // AAA
        UnitConverter converter = CreateConverter();

        converter.AddUnit("длина", "ярд", 0.9144);
        double result = converter.Convert("длина", "ярд", "м", 1);

        Assert.Equal(0.9144, result, 1e-9);
    }

    [Fact]
    public void AddUnit_ExistingUnit_Throws()
    {
        UnitConverter converter = CreateConverter();

        Assert.Throws<ArgumentException>(() => converter.AddUnit("длина", "км", 1000));
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    public void AddUnit_NonPositiveFactor_Throws(double factor)
    {
        UnitConverter converter = CreateConverter();

        Assert.Throws<ArgumentOutOfRangeException>(() => converter.AddUnit("длина", "ярд", factor));
    }

    [Fact]
    public void AddUnit_ForTemperature_Throws()
    {
        UnitConverter converter = CreateConverter();

        Assert.Throws<ArgumentException>(() => converter.AddUnit("температура", "R", 1));
    }

    [Fact]
    public void AddUnit_SavesUnitToStorage()
    {
        // AAA
        InMemoryUnitStorage storage = new InMemoryUnitStorage();
        UnitConverter converter = new UnitConverter(storage);

        converter.AddUnit("масса", "пуд", 16.38);

        Assert.Equal(1, storage.Load().Count);
    }

    [Fact]
    public void Constructor_LoadsUnitsFromStorage()
    {
        // AAA
        InMemoryUnitStorage storage = new InMemoryUnitStorage();
        List<CustomUnit> saved = new List<CustomUnit>();
        saved.Add(new CustomUnit("длина", "ярд", 0.9144));
        storage.Save(saved);

        UnitConverter converter = new UnitConverter(storage);
        double result = converter.Convert("длина", "ярд", "м", 1);

        Assert.Equal(0.9144, result, 1e-9);
    }

    [Fact]
    public void GetUnits_Length_ContainsKilometers()
    {
        UnitConverter converter = CreateConverter();

        Assert.Contains("км", converter.GetUnits("длина"));
    }


    [Fact]
    public void FileUnitStorage_SaveThenLoad_ReturnsSameUnits()
    {
        // AAA
        string path = Path.GetTempFileName();
        try
        {
            FileUnitStorage storage = new FileUnitStorage(path);
            List<CustomUnit> units = new List<CustomUnit>();
            units.Add(new CustomUnit("длина", "ярд", 0.9144));
            units.Add(new CustomUnit("масса", "пуд", 16.38));

            storage.Save(units);
            List<CustomUnit> loaded = storage.Load();

            Assert.Equal(2, loaded.Count);
            Assert.Equal("ярд", loaded[0].Name);
            Assert.Equal(16.38, loaded[1].Factor, 1e-9);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void FileUnitStorage_MissingFile_ReturnsEmptyList()
    {
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".txt");
        FileUnitStorage storage = new FileUnitStorage(path);

        List<CustomUnit> loaded = storage.Load();

        Assert.Empty(loaded);
    }
}
