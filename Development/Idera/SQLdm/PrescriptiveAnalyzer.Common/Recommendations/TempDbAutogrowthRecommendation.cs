using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class TempDbAutogrowthRecommendation : Recommendation, IProvideDatabase
    {
        public string Database { get { return "tempdb"; } }

        public TempDbAutogrowthRecommendation() : base(RecommendationType.DiskTempDbAutogrowth)
        { 
        }

        public TempDbAutogrowthRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DiskTempDbAutogrowth, recProp)
        {
        }

        public override Dictionary<string, string> GetProperties()
        {
            return base.GetProperties();
        }
    }
}
