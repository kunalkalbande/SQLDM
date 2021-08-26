using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public enum IndexLockType
    {
        Page,
        Row
    }

    [Serializable]
    public class IndexLockContentionRecommendation : IndexRecommendation
    {
        public readonly int Partition;
        public readonly long LockCount;
        public readonly long WaitCount;
        public readonly decimal LockPercent;
        public readonly long WaitMs;
        public readonly decimal AvgWaitMs;
        public readonly IndexLockType LockType;
        public readonly UInt64 ServerUpSeconds;

        public string FormattedAvgWait { get { return (FormatHelper.FormatMS(AvgWaitMs)); } }

        public string FormattedLockPercent 
        { 
            get 
            {
                if (LockPercent >= 1) return (string.Format("{0:0.#}%", LockPercent)); 
                return ("less than 1%");
            } 
        }

        public IndexLockContentionRecommendation(IndexLockType lockType, RecommendationProperties recProp)
            : base(RecommendationType.IndexLockContention, recProp)
        {
            Partition = recProp.GetInt("Partition");
            LockCount = recProp.GetLong("LockCount");
            WaitCount = recProp.GetLong("WaitCount");
            LockPercent = recProp.GetDecimal("LockPercent");
            WaitMs = recProp.GetLong("WaitMs");
            AvgWaitMs = recProp.GetDecimal("AvgWaitMs");
            LockType = lockType;
            ServerUpSeconds = recProp.GetUInt64("ServerUpSeconds");
        }

        public IndexLockContentionRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.IndexLockContention, recProp)
        {
            Partition = recProp.GetInt("Partition");
            LockCount = recProp.GetLong("LockCount");
            WaitCount = recProp.GetLong("WaitCount");
            LockPercent = recProp.GetDecimal("LockPercent");
            WaitMs = recProp.GetLong("WaitMs");
            AvgWaitMs = recProp.GetDecimal("AvgWaitMs");
            LockType = (IndexLockType)recProp.GetInt("LockType"); ;
            ServerUpSeconds = recProp.GetUInt64("ServerUpSeconds");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Partition", Partition.ToString());
            prop.Add("LockCount", LockCount.ToString());
            prop.Add("WaitCount", WaitCount.ToString());
            prop.Add("LockPercent", LockPercent.ToString());
            prop.Add("WaitMs", WaitMs.ToString());
            prop.Add("AvgWaitMs", AvgWaitMs.ToString());
            prop.Add("LockType", ((int)LockType).ToString());
            prop.Add("ServerUpSeconds", ServerUpSeconds.ToString());
            return prop;
        }

        public IndexLockContentionRecommendation(IndexLockType lockType,
                                                        string db, 
                                                        string schema, 
                                                        string table, 
                                                        string name, 
                                                        int partition,
                                                        long lockCount,
                                                        long waitCount,
                                                        decimal lockPercent,
                                                        long waitMs,
                                                        decimal avgWaitMs,
                                                        UInt64 serverUpSeconds
                                                        )
            : base(RecommendationType.IndexLockContention, db, schema, table, name)
        {
            LockType = lockType;
            Partition = partition;
            LockCount = lockCount;
            WaitCount = waitCount;
            LockPercent = lockPercent;
            WaitMs = waitMs;
            AvgWaitMs = avgWaitMs;
            ServerUpSeconds = serverUpSeconds;
        }

        public IndexLockType ContentionType
        {
            get { return LockType; }
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
