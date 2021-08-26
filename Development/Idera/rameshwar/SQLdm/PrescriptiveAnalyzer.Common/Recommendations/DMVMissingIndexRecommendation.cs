using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DMVMissingIndexRecommendation : MissingIndexBaseRecommendation
    {
        public double AvgUserCost { get; private set; }
        public double AvgUserImpact { get; private set; }
        public double DaysSinceServerStarted { get; private set; }
        public double AvgUserCostImpact { get { return (Math.Round(AvgUserCost * AvgUserImpact, 3)); } }
        public double AvgUserScansSeeksPerDay { get { return ((DaysSinceServerStarted <= 1.0) ? UserScansSeeks : Math.Round(UserScansSeeks / DaysSinceServerStarted, 1)); } }
        public UInt64 UserScans { get; private set; }
        public UInt64 UserSeeks { get; private set; }
        public UInt64 UserScansSeeks { get { return (UserScans+UserSeeks);} }

        private double _impact = 0;

        public DMVMissingIndexRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DMVMissingIndex, recProp)
        {
            AvgUserCost = recProp.GetDouble("AvgUserCost");
            AvgUserImpact = recProp.GetDouble("AvgUserImpact");
            DaysSinceServerStarted = recProp.GetDouble("DaysSinceServerStarted");
            UserScans = recProp.GetUInt64("UserScans");
            UserSeeks = recProp.GetUInt64("UserSeeks");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("AvgUserCost", AvgUserCost.ToString());
            prop.Add("AvgUserImpact", AvgUserImpact.ToString());
            prop.Add("DaysSinceServerStarted", DaysSinceServerStarted.ToString());
            prop.Add("UserScans", UserScans.ToString());
            prop.Add("UserSeeks", UserSeeks.ToString());
            return prop;
        }

        public DMVMissingIndexRecommendation(IEnumerable<RecommendedIndex> recommendedIndexes, double tableUpdatesPerSec, double tableUpdatesPerMinute, double impact, double avgUserCost, double avgUserImpact, UInt64 userScans, UInt64 userSeeks, double daysSinceServerStarted)
            : base(RecommendationType.DMVMissingIndex, recommendedIndexes, tableUpdatesPerSec, tableUpdatesPerMinute)
        {
            AvgUserCost = avgUserCost;
            AvgUserImpact = avgUserImpact;
            UserScans = userScans;
            UserSeeks = userSeeks;
            DaysSinceServerStarted = daysSinceServerStarted;

            _impact = impact;
            if (null != RecommendedIndexes)
            {
                if (RecommendedIndexes.Count > 0)
                {
                    //var ri = RecommendedIndexes.First();
                    RecommendedIndex[] array1 = new RecommendedIndex[RecommendedIndexes.Count];
                    RecommendedIndexes.CopyTo(array1, 0);
                    var ri = array1[0];
                    if (null != ri)
                    {
                        Database = ri.Database;
                        Schema = ri.Schema;
                        Table = ri.Table;
                    }
                }
            }
        }
        public override int AdjustConfidenceFactor(int i)
        {
            int confidence = base.AdjustConfidenceFactor(i);
            //----------------------------------------------------------------------------
            // For the dmv we really cannot be 100% confidence so max out at 80
            // 
            if (confidence > 8) return (8);
            return (confidence);
        }

        public override int AdjustImpactFactor(int i)
        {
            //----------------------------------------------------------------------------
            // Per PR DR294 - impact should be:
            //     Low Impact - Impact 30,000-700,000
            //     Medium Impact - Impact 700,000 - 20,000,000
            //     High Impact - Impact above 20,000,000 
            // 
            if (_impact < 700000) return (LOW_IMPACT);
            if (_impact > 20000000) return (HIGH_IMPACT);
            return (LOW_IMPACT + 1);
        }
    }
}
