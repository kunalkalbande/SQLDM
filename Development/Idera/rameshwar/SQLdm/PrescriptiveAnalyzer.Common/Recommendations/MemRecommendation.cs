using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Services;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class MemRecommendation : Recommendation
    {
        public MemRecommendation(RecommendationType type)
            : base(type)
        {
        }

        public MemRecommendation(RecommendationType type, RecommendationProperties recProp)
            : base(type, recProp)
        {
        }

        public override Dictionary<string, string> GetProperties()
        {
            return base.GetProperties();
        }
    }
}
