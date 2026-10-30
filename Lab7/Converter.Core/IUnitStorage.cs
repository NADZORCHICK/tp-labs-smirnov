using System;
using System.Collections.Generic;
using System.Text;

namespace Converter.Core
{
    public interface IUnitStorage
    {
        List<CustomUnit> Load();
        void Save(List<CustomUnit> units);
    }
}
