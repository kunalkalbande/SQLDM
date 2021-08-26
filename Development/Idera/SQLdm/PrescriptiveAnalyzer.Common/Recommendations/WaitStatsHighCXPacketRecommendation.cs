using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class WaitStatsHighCXPacketRecommendation : WaitStatsRecommendation
    {
        public bool HyperThreaded { get; private set; }
        public bool OLTPServer { get; private set; }

        public WaitStatsHighCXPacketRecommendation(AffectedBatches ab, bool hyperThreaded, bool oltpServer)
            : base(ab, RecommendationType.WaitStatsHighCXPacket)
        {
            HyperThreaded = hyperThreaded;
            OLTPServer = oltpServer;
        }

        public WaitStatsHighCXPacketRecommendation()
            : base( RecommendationType.WaitStatsHighCXPacket)
        {
        }

        public WaitStatsHighCXPacketRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.WaitStatsHighCXPacket, recProp)
        {
            HyperThreaded = recProp.GetBool("HyperThreaded");
            OLTPServer = recProp.GetBool("OLTPServer");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("HyperThreaded", HyperThreaded.ToString());
            prop.Add("OLTPServer", OLTPServer.ToString());
            return prop;
        }
    }
}
