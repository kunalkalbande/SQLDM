using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class TempDbRecoveryModelRecommendation : Recommendation, IProvideDatabase
    {
        public string Database { get { return "tempdb"; } }

        public TempDbRecoveryModelRecommendation() : base(RecommendationType.DiskTempDbRecoveryModel)
        {
        }

        public TempDbRecoveryModelRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DiskTempDbRecoveryModel, recProp)
        {
        }

        public override Dictionary<string, string> GetProperties()
        {
            return base.GetProperties();
        }
    }
}
