using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class NetBandwidthLostRecommendation : NetworkRecommendation
    {
        public UInt64 PreviousBandwidth { get; private set; }
        public UInt64 PreviousBandwidthMB { get { return (PreviousBandwidth / 1000000); } }
        public UInt64 Bandwidth { get; private set; }
        public UInt64 BandwidthMB { get { return (Bandwidth / 1000000); } }
        public string BandwidthLost { get { return (PreviousBandwidth <= 0) ? "0" : string.Format("{0:0.#}", BandwidthLostPercent); } }
        public double BandwidthLostPercent { get { return (PreviousBandwidth <= 0) ? 0 : 100.0 - ((Bandwidth * 100.0) / PreviousBandwidth); } }

        public NetBandwidthLostRecommendation() : base(RecommendationType.NetBandwidthLost) { }

        public NetBandwidthLostRecommendation(UInt64 prevBandwidth, UInt64 bandwidth) : this()
        {
            PreviousBandwidth = prevBandwidth;
            Bandwidth = bandwidth;
        }

        public NetBandwidthLostRecommendation(RecommendationProperties recProp) : base(RecommendationType.NetBandwidthLost, recProp) 
        {
            PreviousBandwidth = recProp.GetUInt64("PreviousBandwidth");
            Bandwidth = recProp.GetUInt64("Bandwidth");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("PreviousBandwidth", PreviousBandwidth.ToString());
            prop.Add("Bandwidth", Bandwidth.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            if (BandwidthLostPercent < 7.0) return (LOW_IMPACT);
            else if (BandwidthLostPercent > 25.0) return (HIGH_IMPACT);
            return (LOW_IMPACT + 1);
        }
    }
}
