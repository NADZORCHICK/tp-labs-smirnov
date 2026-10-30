using System;
using System.Collections.Generic;
using System.Text;

namespace Converter.Core
{
    public class UnitConverter
    {
        public const string Length = "длина";
        public const string Mass = "масса";
        public const string Speed = "скорость";
        public const string Temperature = "температура";
        private IUnitStorage _storage;
        private Dictionary<string, Dictionary<string, double>> _factors = new Dictionary<string, Dictionary<string, double>>(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, string> _baseUnits = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private List<CustomUnit> _customUnits = new List<CustomUnit>();
        public UnitConverter(IUnitStorage storage)
        {
            if (storage == null)
            {
                throw new ArgumentNullException(nameof(storage));
            }

            _storage = storage;

            CreateTable();

            List<CustomUnit> saved = _storage.Load();
            foreach (CustomUnit unit in saved)
            {
                try
                {
                    AddToTable(unit.Quantity, unit.Name, unit.Factor);
                    _customUnits.Add(unit);
                }
                catch (ArgumentException)
                {
                    // Некорректная запись в хранилище — пропускаем её.
                }
            }
        }
        private void CreateTable()
        {
            Dictionary<string, double> length = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            length.Add("м", 1);
            length.Add("км", 1000);
            length.Add("см", 0.01);
            length.Add("мм", 0.001);
            length.Add("миля", 1609.344);
            length.Add("фут", 0.3048);
            length.Add("дюйм", 0.0254);
            _factors.Add(Length, length);
            _baseUnits.Add(Length, "м");

            Dictionary<string, double> mass = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            mass.Add("кг", 1);
            mass.Add("г", 0.001);
            mass.Add("т", 1000);
            mass.Add("фунт", 0.45359237);
            mass.Add("унция", 0.028349523125);
            _factors.Add(Mass, mass);
            _baseUnits.Add(Mass, "кг");

            Dictionary<string, double> speed = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            speed.Add("м/с", 1);
            speed.Add("км/ч", 1000.0 / 3600);
            speed.Add("миля/ч", 0.44704);
            speed.Add("узел", 1852.0 / 3600);
            _factors.Add(Speed, speed);
            _baseUnits.Add(Speed, "м/с");
        }
        public List<string> GetQuantities()
        {
            List<string> result = new List<string>();
            result.Add(Length);
            result.Add(Mass);
            result.Add(Speed);
            result.Add(Temperature);
            return result;
        }
        public List<string> GetUnits(string quantity)
        {
            CheckQuantity(quantity);

            List<string> result = new List<string>();

            if (IsTemperature(quantity))
            {
                result.Add("C");
                result.Add("F");
                result.Add("K");
                return result;
            }
            foreach (string unit in _factors[quantity].Keys)
            {
                result.Add(unit);
            }

            return result;
        }
        public string GetBaseUnit(string quantity)
        {
            CheckQuantity(quantity);

            if (IsTemperature(quantity))
            {
                throw new ArgumentException("У температуры нет базовой единицы с коэффициентами.", nameof(quantity));
            }

            return _baseUnits[quantity];
        }
        public double GetFactor(string quantity, string unit)
        {
            CheckQuantity(quantity);

            if (IsTemperature(quantity))
            {
                throw new ArgumentException("У температуры нет коэффициентов: она пересчитывается по формулам.", nameof(quantity));
            }
            if (string.IsNullOrWhiteSpace(unit))
            {
                throw new ArgumentException("Единица измерения не указана.", nameof(unit));
            }
            double factor;
            if (_factors[quantity].TryGetValue(unit.Trim(), out factor) == false)
            {
                throw new ArgumentException("Неизвестная единица «" + unit + "» для величины «" + quantity + "».", nameof(unit));
            }

            return factor;
        }
        public double Convert(string quantity, string fromUnit, string toUnit, double value)
        {
            CheckQuantity(quantity);

            if (IsTemperature(quantity))
            {
                return ConvertTemperature(fromUnit, toUnit, value);
            }
            double fromFactor = GetFactor(quantity, fromUnit);
            double toFactor = GetFactor(quantity, toUnit);

            return value * fromFactor / toFactor;
        }
        public void AddUnit(string quantity, string name, double factor)
        {
            AddToTable(quantity, name, factor);

            _customUnits.Add(new CustomUnit(quantity, name, factor));
            _storage.Save(_customUnits);
        }
        private void AddToTable(string quantity, string name, double factor)
        {
            CheckQuantity(quantity);

            if (IsTemperature(quantity))
            {
                throw new ArgumentException("Для температуры нельзя добавлять свои единицы.", nameof(quantity));
            }
            if (string.IsNullOrWhiteSpace(name) || name.Contains(' ') || name.Contains(';'))
            {
                throw new ArgumentException("Название единицы не должно быть пустым и содержать пробелы или «;».", nameof(name));
            }
            if (double.IsNaN(factor) || double.IsInfinity(factor) || factor <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(factor), "Коэффициент должен быть положительным числом.");
            }
            if (_factors[quantity].ContainsKey(name))
            {
                throw new ArgumentException("Единица «" + name + "» уже есть в таблице.", nameof(name));
            }

            _factors[quantity].Add(name, factor);
        }
        private double ConvertTemperature(string fromUnit, string toUnit, double value)
        {
            double celsius = ToCelsius(fromUnit, value);

            if (celsius < -273.15 - 1e-9)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Температура не может быть ниже абсолютного нуля.");
            }

            return FromCelsius(toUnit, celsius);
        }
        private double ToCelsius(string unit, double value)
        {
            string u = NormalizeTemperatureUnit(unit);

            if (u == "C")
            {
                return value;
            }
            if (u == "F")
            {
                return (value - 32) * 5 / 9;
            }

            // Остался только K.
            return value - 273.15;
        }
        private double FromCelsius(string unit, double celsius)
        {
            string u = NormalizeTemperatureUnit(unit);

            if (u == "C")
            {
                return celsius;
            }
            if (u == "F")
            {
                return celsius * 9 / 5 + 32;
            }

            return celsius + 273.15;
        }
        private string NormalizeTemperatureUnit(string unit)
        {
            if (string.IsNullOrWhiteSpace(unit))
            {
                throw new ArgumentException("Единица измерения не указана.", nameof(unit));
            }

            string u = unit.Trim().ToUpperInvariant();

            if (u != "C" && u != "F" && u != "K")
            {
                throw new ArgumentException("Неизвестная единица температуры: " + unit + ". Доступно: C, F, K.", nameof(unit));
            }

            return u;
        }
        private bool IsTemperature(string quantity)
        {
            return string.Equals(quantity, Temperature, StringComparison.OrdinalIgnoreCase);
        }
        private void CheckQuantity(string quantity)
        {
            if (string.IsNullOrWhiteSpace(quantity))
            {
                throw new ArgumentException("Величина не указана.", nameof(quantity));
            }

            if (_factors.ContainsKey(quantity) == false && IsTemperature(quantity) == false)
            {
                throw new ArgumentException("Неизвестная величина: " + quantity, nameof(quantity));
            }
        }
    }
}
