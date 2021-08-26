using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class IndexPageLatchContentionRecommendation : IndexRecommendation
    {
        public readonly int Partition;
        public readonly long WaitCount;
        public readonly long WaitMs;
        public readonly decimal AvgWaitMs;
        public readonly UInt64 ServerUpSeconds;

        public string FormattedAvgWait { get { return (FormatHelper.FormatMS(AvgWaitMs)); } }

        public IndexPageLatchContentionRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.IndexPageLatchContention, recProp)
        {
            Partition = recProp.GetInt("Partition");
            WaitCount = recProp.GetLong("WaitCount");
            WaitMs = recProp.GetLong("WaitMs");
            AvgWaitMs = recProp.GetDecimal("AvgWaitMs");
            AvgWaitMs = recProp.GetUInt64("ServerUpSeconds");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Partition", Partition.ToString());
            prop.Add("WaitCount", WaitCount.ToString());
            prop.Add("WaitMs", WaitMs.ToString());
            prop.Add("AvgWaitMs", AvgWaitMs.ToString());
            prop.Add("ServerUpSeconds", ServerUpSeconds.ToString());
            return prop;
        }

        public IndexPageLatchContentionRecommendation(string db, 
                                                        string schema, 
                                                        string table, 
                                                        string name, 
                                                        int partition,
                                                        long waitCount,
                                                        long waitMs,
                                                        decimal avgWaitMs,
                                                        UInt64 serverUpSeconds
                                                        )
            : base(RecommendationType.IndexPageLatchContention, db, schema, table, name)
        {
            Partition = partition;
            WaitCount = waitCount;
            WaitMs = waitMs;
            AvgWaitMs = avgWaitMs;
            ServerUpSeconds = serverUpSeconds;
        }

        public override int AdjustImpactFactor(int i)
        {
            long ms = (long)Math.Round((ServerUpSeconds * .1) * 1000, 0);
            if (WaitMs > ms) return (HIGH_IMPACT);
            else if (WaitMs > (ms / 2)) return (LOW_IMPACT + 1);
            return (LOW_IMPACT);
        }

    }
}
