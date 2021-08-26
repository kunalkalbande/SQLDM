using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DatabaseCheckIntegrityRecommendation : DatabaseRecommendation
    {
        public double DaysSinceLastCheck { get; private set; }        
        public DatabaseCheckIntegrityRecommendation(string database, double totalDays)
            : base(RecommendationType.DatabaseCheckIntegrity, database)
        {
            DaysSinceLastCheck = Math.Round(totalDays, 2);
        }

        public DatabaseCheckIntegrityRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DatabaseCheckIntegrity, recProp)
        {
            DaysSinceLastCheck = recProp.GetDouble("DaysSinceLastCheck");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("DaysSinceLastCheck", DaysSinceLastCheck.ToString());
            return prop;
        }
    }
}
