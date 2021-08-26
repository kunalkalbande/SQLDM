using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DatabaseOutdatedBackupsRecommendation : DatabaseRecommendation
    {
        public double DaysSinceLastBackup { get; private set; }
        public DatabaseOutdatedBackupsRecommendation(string database, double totalDays)
            : base(RecommendationType.DatabaseOutdatedBackups, database)
        {
            DaysSinceLastBackup = Math.Round(totalDays, 2);
        }

        public DatabaseOutdatedBackupsRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DatabaseOutdatedBackups, recProp)
        {
            DaysSinceLastBackup = recProp.GetDouble("DaysSinceLastBackup");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("DaysSinceLastBackup", DaysSinceLastBackup.ToString());
            return prop;
        }
    }
}
