using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class NetCardLostRecommendation : NetworkRecommendation
    {
        public int PreviousCards { get; private set; }
        public UInt64 PreviousBandwidth { get; private set; }
        public UInt64 PreviousBandwidthMB { get { return (PreviousBandwidth / 1000000); } }
        public int Cards { get; private set; }
        public UInt64 Bandwidth { get; private set; }
        public UInt64 BandwidthMB { get { return (Bandwidth / 1000000); } }
        public string BandwidthLost { get { return (PreviousBandwidth <= 0) ? "0" : string.Format("{0:0.#}", BandwidthLostPercent); } }
        public double BandwidthLostPercent { get { return (PreviousBandwidth <= 0) ? 0 : 100.0 - ((Bandwidth * 100.0) / PreviousBandwidth); } }

        public NetCardLostRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.NetCardLost, recProp)
        {
            PreviousCards = recProp.GetInt("PreviousCards");
            PreviousBandwidth = recProp.GetUInt64("PreviousBandwidth");
            Cards = recProp.GetInt("Cards");
            Bandwidth = recProp.GetUInt64("Bandwidth");
        }

        public NetCardLostRecommendation() : base(RecommendationType.NetCardLost) 
        {
        }

        public NetCardLostRecommendation(int prevCards, UInt64 prevBandwidth, int cards, UInt64 bandwidth) : this()
        {
            PreviousCards = prevCards;
            PreviousBandwidth = prevBandwidth;
            Cards = cards;
            Bandwidth = bandwidth;
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("PreviousCards", PreviousCards.ToString());
            prop.Add("PreviousBandwidth", PreviousBandwidth.ToString());
            prop.Add("Cards", Cards.ToString());
            prop.Add("Bandwidth", Bandwidth.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            if (BandwidthLostPercent > 25.0) return (HIGH_IMPACT);
            return base.AdjustImpactFactor(i);
        }

    }
}
