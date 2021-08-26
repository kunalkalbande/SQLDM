using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class CpuRecommendation : Recommendation
    {
        public CpuRecommendation(RecommendationType type) : base(type)
        {
        }

        public CpuRecommendation(RecommendationType type, RecommendationProperties recProp)
            : base(type, recProp)
        {
        }
    }
}
