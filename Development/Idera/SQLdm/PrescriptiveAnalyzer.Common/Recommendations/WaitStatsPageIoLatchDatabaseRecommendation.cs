using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class WaitStatsPageIoLatchDatabaseRecommendation : WaitStatsRecommendation, IProvideDatabase, IProvideAffectedBatches
    {
        public string Database { get; private set; }

        public WaitStatsPageIoLatchDatabaseRecommendation(AffectedBatches ab, string db)
            : base(ab, RecommendationType.WaitStatsPageIoLatchDatabase)
        {
            Database = db;
        }

        public WaitStatsPageIoLatchDatabaseRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.WaitStatsPageIoLatchDatabase, recProp)
        {
            Database = recProp.GetString("Database");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Database", Database.ToString());
            return prop;
        }
    }
}
