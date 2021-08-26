using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class NetworkRecommendation : Recommendation
    {
        public NetworkRecommendation(RecommendationType type, RecommendationProperties recProp)
            : base(type, recProp)
        {
        }

        public NetworkRecommendation(RecommendationType type)
            : base(type)
        {
        }
     
        public NetworkRecommendation(RecommendationType type, string id) : base(type, id)
        {
        }

        public NetworkRecommendation(RecommendationType type, string id, RecommendationProperties recProp)
            : base(type, id, recProp)
        {
        }

        public override Dictionary<string, string> GetProperties()
        {
            return base.GetProperties();
        }
    }
}
