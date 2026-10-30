using System;
using System.Collections.Generic;
using System.Text;

namespace Converter.Core
{
    public class InMemoryUnitStorage : IUnitStorage
    {
        private List<CustomUnit> _units = new List<CustomUnit>();

        public List<CustomUnit> Load()
        {
            return new List<CustomUnit>(_units);
        }
        public void Save(List<CustomUnit> units)
        {
            _units = new List<CustomUnit>(units);
        }
    }
}
