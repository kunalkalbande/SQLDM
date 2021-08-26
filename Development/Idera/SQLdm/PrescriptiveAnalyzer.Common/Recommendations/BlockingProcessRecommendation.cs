using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class BlockingProcessRecommendation : TSqlRecommendation
    {
        public int SPID { get; set; }
        public long BlockedWait { get; set; }
        public int BlockedNumberOfProcesses { get; set; }
        public string BlockedObject { get; set; }

        public BlockingProcessRecommendation(String database, String application, String user, String host)
            : base(RecommendationType.BlockingProcess, database, application, user, host)
        {
        }

        public BlockingProcessRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.BlockingProcess, recProp)
        {
            SPID = recProp.GetInt("SPID");
            BlockedWait = recProp.GetLong("BlockedWait");
            BlockedNumberOfProcesses = recProp.GetInt("BlockedNumberOfProcesses");
            BlockedObject = recProp.GetString("BlockedObject");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("SPID", SPID.ToString());
            prop.Add("BlockedWait", BlockedWait.ToString());
            prop.Add("BlockedNumberOfProcesses", BlockedNumberOfProcesses.ToString());
            prop.Add("BlockedObject", BlockedObject.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {   // default is low;

            if (BlockedWait > 20000)
                return HIGH_IMPACT;

            if (BlockedWait > 5000)
                return 2;

            return base.AdjustConfidenceFactor(i);
        }
    }
}
