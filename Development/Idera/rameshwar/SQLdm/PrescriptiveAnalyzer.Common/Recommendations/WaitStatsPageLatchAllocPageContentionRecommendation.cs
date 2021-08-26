using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Services;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class WaitStatsPageLatchAllocPageContentionRecommendation : WaitStatsRecommendation, IProvideDatabase, IProvideAffectedBatches
    {
        public string Database { get; private set; }

        public WaitStatsPageLatchAllocPageContentionRecommendation(AffectedBatches ab, string db)
            : base(ab, RecommendationType.WaitStatsPageLatchAllocPageContention)
        {
            Database = db;
        }

        public WaitStatsPageLatchAllocPageContentionRecommendation(RecommendationProperties recProp)
            : base( RecommendationType.WaitStatsPageLatchAllocPageContention, recProp)
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
