using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Services;
//using Idera.SQLdoctor.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class WaitStatsAsyncNetIORecommendation : WaitStatsRecommendation
    {

        public WaitStatsAsyncNetIORecommendation(AffectedBatches ab)
            : base(ab, RecommendationType.WaitStatsAsyncNetIO)
        {
        }

        public WaitStatsAsyncNetIORecommendation(RecommendationProperties recProp)
            : base(RecommendationType.WaitStatsAsyncNetIO, recProp)
        {
        }

        public override Dictionary<string, string> GetProperties()
        {
            return base.GetProperties();
        }
    }
}
