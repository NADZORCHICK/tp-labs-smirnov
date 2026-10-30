using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Converter.Core
{
    public class FileUnitStorage : IUnitStorage
    {
        private string _path;

        public FileUnitStorage(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(path));
            }

            _path = path;
        }
        public List<CustomUnit> Load()
        {
            List<CustomUnit> result = new List<CustomUnit>();

            if (File.Exists(_path) == false)
            {
                return result;
            }

            string[] lines = File.ReadAllLines(_path);
            foreach (string line in lines)
            {
                string[] parts = line.Split(';');

                if (parts.Length != 3)
                {
                    continue;
                }

                double factor;
                if (double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out factor) == false)
                {
                    continue;
                }

                result.Add(new CustomUnit(parts[0], parts[1], factor));
            }
            return result;
        }
        public void Save(List<CustomUnit> units)
        {
            List<string> lines = new List<string>();

            foreach (CustomUnit unit in units)
            {
                lines.Add(unit.Quantity + ";" + unit.Name + ";" + unit.Factor.ToString(CultureInfo.InvariantCulture));
            }

            File.WriteAllLines(_path, lines);
        }
    }
}
   
