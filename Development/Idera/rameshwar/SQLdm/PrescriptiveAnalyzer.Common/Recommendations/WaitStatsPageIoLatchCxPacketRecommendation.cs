using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class WaitStatsPageIoLatchCxPacketRecommendation : WaitStatsRecommendation
    {
        public WaitStatsPageIoLatchCxPacketRecommendation(AffectedBatches ab)
            : base(ab, RecommendationType.WaitStatsPageIoLatchCxPacket)
        {
        }

        public WaitStatsPageIoLatchCxPacketRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.WaitStatsPageIoLatchCxPacket, recProp)
        {
        }

        public override Dictionary<string, string> GetProperties()
        {
            return base.GetProperties();
        }
    }
}
