using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class OpenTransactionRecommendation : TSqlRecommendation
    {
        public long TID { get; set; }
        public int SPID { get; set; }
        public DateTime Start { get; set; }
        public TimeSpan Duration { get; set; }
        public double DurationMinutes { get { return Math.Round(Duration.TotalMinutes, 1); } }

        public OpenTransactionRecommendation(String database, String application, String user, String host) : base(RecommendationType.OpenTransaction, database, application, user, host)
        {
        }

        public OpenTransactionRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.OpenTransaction, recProp)
        {
            TID = recProp.GetLong("TID");
            SPID = recProp.GetInt("SPID");
            Start = recProp.GetDateTime("Start");
            Duration = recProp.GetTimeSpan("Duration");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("TID", TID.ToString());
            prop.Add("SPID", SPID.ToString());
            prop.Add("Start", Start.ToString());
            prop.Add("Duration", Duration.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {   // default is low;
            if (DurationMinutes > 30)
                return HIGH_IMPACT;

            if (DurationMinutes > 15)
                return HIGH_IMPACT - LOW_IMPACT;

            return base.AdjustConfidenceFactor(i);
        }
    }
}
