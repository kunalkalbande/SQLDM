using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Objects;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DeadlockRecommendation : Recommendation, IProvideDatabase, IProvideApplicationName
    {
        public long EventSequence { get; set; }
        public DateTime TimeStamp { private get; set; }
        public String XDL { get; set; }
        public DateTime StartTime { get { return TimeStamp; } }

        public int DatabaseId { get; set; }
        public string Database { get; set; }
        public string ApplicationName { get; set; }
        public string HostName { get; set; }
        public string UserName { get; set; }

        public DeadlockRecommendation() : base(RecommendationType.Deadlock)
        {
            ApplicationName = Database = HostName = UserName = String.Empty;
        }

        public DeadlockRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.Deadlock, recProp)
        {
            EventSequence = recProp.GetInt("EventSequence");
            TimeStamp = recProp.GetDateTime("TimeStamp");
            XDL = recProp.GetString("XDL");
            DatabaseId = recProp.GetInt("DatabaseId");
            Database = recProp.GetString("Database");
            ApplicationName = recProp.GetString("ApplicationName");
            HostName = recProp.GetString("HostName");
            UserName = recProp.GetString("UserName");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("EventSequence", EventSequence.ToString());
            prop.Add("TimeStamp", TimeStamp.ToString());
            prop.Add("XDL", XDL.ToString());
            prop.Add("DatabaseId", DatabaseId.ToString());
            prop.Add("Database", Database.ToString());
            prop.Add("ApplicationName", ApplicationName.ToString());
            prop.Add("HostName", HostName.ToString());
            prop.Add("UserName", UserName.ToString());
            return prop;
        }
    }
}
