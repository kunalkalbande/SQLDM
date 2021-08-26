using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// Network repot #78 for network retransmitted segments
    /// </summary>
    [Serializable]
    public class NetRetranSegsRecommendation : NetworkRecommendation
    {
        public UInt64 SegmentsPersec { get; private set; }
        public UInt64 SegmentsRetransmittedPerSec { get; private set; }
        public string SegmentsRetransmittedPercent { get { return (SegmentsPersec <= 0) ? "0" : string.Format("{0:0.#}", SegmentsRetransmittedPercentNum); } }
        public double SegmentsRetransmittedPercentNum { get { return (SegmentsPersec <= 0) ? 0 : ((SegmentsRetransmittedPerSec * 100.0) / SegmentsPersec); } }

        public NetRetranSegsRecommendation() : base(RecommendationType.NetRetranSegs) { }

        public NetRetranSegsRecommendation(RecommendationProperties recProp) :
            base(RecommendationType.NetRetranSegs, recProp) {
                SegmentsPersec = recProp.GetUInt64("SegmentsPersec");
                SegmentsRetransmittedPerSec = recProp.GetUInt64("SegmentsRetransmittedPerSec");
            }

        public NetRetranSegsRecommendation(UInt64 total, UInt64 retran)
            : this()
        {
            SegmentsPersec = total;
            SegmentsRetransmittedPerSec = retran;
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("SegmentsPersec", SegmentsPersec.ToString());
            prop.Add("SegmentsRetransmittedPerSec", SegmentsRetransmittedPerSec.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            if (SegmentsRetransmittedPercentNum < 5.0) return (LOW_IMPACT);
            else if (SegmentsRetransmittedPercentNum <= 12.0) return (LOW_IMPACT + 1);
            else return (HIGH_IMPACT);
        }
    }
}
