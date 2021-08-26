using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.Common.Configuration
{
    [Serializable]
    public class HealthMetric
    {
        public string Category;
        public string Name;
        public Baseline Baseline;
        public string BaselineUnits;
        public bool BaselineUnitVisible;

        public HealthMetric(string name, string category, Baseline baseline, string units, bool showUnits)
        {
            Category = category;
            Name = name;
            Baseline = baseline;
            BaselineUnits = units;
            BaselineUnitVisible = showUnits;
        }

        public HealthMetric() { }
    }
}
