using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// Network recommendation report #75
    /// </summary>
    [Serializable]
    public class NetCongestionRecommendation : NetworkRecommendation
    {
        public UInt64 SqlPackets { get; private set; }
        public UInt64 TotalPackets { get; private set; }
        public string SqlNetworkLoad { get { return (TotalPackets <= 0) ? "0" : string.Format("{0:0.#}", SqlNetworkLoadPercent); } }
        public double SqlNetworkLoadPercent { get { return (TotalPackets <= 0) ? 0 : ((SqlPackets * 100.0) / TotalPackets); } }
        public string NonSqlNetworkLoad { get { return (TotalPackets <= 0) ? "0" : string.Format("{0:0.#}", 100.0 - ((SqlPackets * 100.0) / TotalPackets), 0); } }

        public NetCongestionRecommendation() : base(RecommendationType.NetCongestion) { }

        public NetCongestionRecommendation(UInt64 sqlPackets, UInt64 totalPackets)
            : this()
        {
            SqlPackets = sqlPackets;
            TotalPackets = totalPackets;
        }

        public NetCongestionRecommendation(RecommendationProperties recProp) : base(RecommendationType.NetCongestion, recProp)
        {
            SqlPackets = recProp.GetUInt64("SqlPackets");
            TotalPackets = recProp.GetUInt64("TotalPackets");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("SqlPackets", SqlPackets.ToString());
            prop.Add("TotalPackets", TotalPackets.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            if (SqlNetworkLoadPercent < 50.0) return (HIGH_IMPACT);
            return (base.AdjustImpactFactor(i));
        }
    }
}
