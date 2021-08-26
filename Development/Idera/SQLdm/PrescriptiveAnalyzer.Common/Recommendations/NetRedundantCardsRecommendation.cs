using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// Network recommendation report #79a
    /// </summary>
    [Serializable]
    public class NetRedundantCardsRecommendation : NetworkRecommendation
    {
        public NetRedundantCardsRecommendation() : base(RecommendationType.NetRedundantCards, "SDR-N8") { }

        public NetRedundantCardsRecommendation(RecommendationProperties recProp) :
            base(RecommendationType.NetRedundantCards, "SDR-N8", recProp) { }

        public override Dictionary<string, string> GetProperties()
        {
            return base.GetProperties();
        }
    }
}
