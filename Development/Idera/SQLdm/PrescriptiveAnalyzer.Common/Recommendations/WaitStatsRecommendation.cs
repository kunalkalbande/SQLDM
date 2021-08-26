using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class WaitStatsRecommendation : Recommendation, IProvideAffectedBatches
    {
        private AffectedBatches _ab = null;

        public WaitStatsRecommendation(AffectedBatches ab, RecommendationType type) : base(type)
        {
            _ab = ab;
        }

        public WaitStatsRecommendation(RecommendationType type)
            : base(type)
        {
           
        }

        public WaitStatsRecommendation(RecommendationType type, RecommendationProperties recProp)
            : base(type, recProp)
        {
            _ab = recProp.GetAffectedBatches("AffectedBatches");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("AffectedBatches", RecommendationProperties.GetXml<AffectedBatches>(_ab));
            return prop;
        }

        public AffectedBatches GetAffectedBatches()
        {
            return (_ab);
        }
    }
}
