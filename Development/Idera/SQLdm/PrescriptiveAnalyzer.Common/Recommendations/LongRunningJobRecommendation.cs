using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class LongRunningJobRecommendation : Recommendation
    {
        public string JobName { get; set; }
        public TimeSpan LastRunDuration { get; set; }
        public TimeSpan MaxRunDuration { get; set; }
        public TimeSpan AvgRunDuration { get; set; }

        public double LastRunDurationMinutes
        {
            get { return Math.Round(LastRunDuration.TotalMinutes, 1); }
        }
        public double AvgRunDurationMinutes
        {
            get { return Math.Round(AvgRunDuration.TotalMinutes, 1); }
        }

        public LongRunningJobRecommendation() : base(RecommendationType.LongRunningJob)
        {
        }

        public LongRunningJobRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.LongRunningJob, recProp)
        {
            JobName = recProp.GetString("JobName");
            LastRunDuration = recProp.GetTimeSpan("LastRunDuration");
            MaxRunDuration = recProp.GetTimeSpan("MaxRunDuration");
            AvgRunDuration = recProp.GetTimeSpan("AvgRunDuration");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("JobName", JobName.ToString());
            prop.Add("LastRunDuration", LastRunDuration.ToString());
            prop.Add("MaxRunDuration", MaxRunDuration.ToString());
            prop.Add("AvgRunDuration", AvgRunDuration.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {   // default is low;

            double diff = LastRunDurationMinutes - AvgRunDurationMinutes;
            
            if (diff > 60)
                return HIGH_IMPACT;

            if (diff > 30)
                return HIGH_IMPACT - LOW_IMPACT;

            return base.AdjustImpactFactor(i);
        }
    }
}
