using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class TempDbWaitingRecommendation : Recommendation, IProvideDatabase
    {
        public int WaitingSampleCount { get; set; }
        public string Database { get { return "tempdb"; } }
        public string TraceStatus { get; set; }

        public TempDbWaitingRecommendation(int status) : base(RecommendationType.DiskTempDbWaiting)
        {
            if (status == 0)
                TraceStatus = "It is recommended that trace flag 1118 be turned on.  This trace flag causes SQL Server to use uniform extent allocation instead of mixed extent allocation, which in turn will reduce contention on allocation pages when an object is allocated or deallocated." +
                              Environment.NewLine +
                              "If these steps have already been done, then it";
            else
                TraceStatus = "Trace flag 1118 is truned on. It";
        }
        public TempDbWaitingRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DiskTempDbWaiting, recProp)
        {
            WaitingSampleCount = recProp.GetInt("WaitingSampleCount");
            TraceStatus = recProp.GetString("TraceStatus");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("WaitingSampleCount", WaitingSampleCount.ToString());
            prop.Add("TraceStatus", TraceStatus.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            if (WaitingSampleCount > 1)
                return HIGH_IMPACT;

            return base.AdjustImpactFactor(i);
        }

    }
}
