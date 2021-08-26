using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DiskWaitingRecommendation : Recommendation
    {
        public DiskWaitingRecommendation() : base(RecommendationType.DiskWaiting)
        {
        }

        public DiskWaitingRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DiskWaiting, recProp)
        {
        } 
    }
}
