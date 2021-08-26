using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Services;
//using Idera.SQLdoctor.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class Mem32bit4gSS2012Recommendation : MemRecommendation
    {
        public UInt64 ServerMemory { get; private set; }
        public Mem32bit4gSS2012Recommendation(UInt64 serverMemory)
            : base(RecommendationType.Mem32bit4gSS2012)
        {
            ServerMemory = serverMemory;
        }

        public Mem32bit4gSS2012Recommendation(RecommendationProperties recProp)
            : base(RecommendationType.Mem32bit4gSS2012, recProp)
        {
            ServerMemory = recProp.GetUInt64("ServerMemory");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("ServerMemory", ServerMemory.ToString());
            return prop;
        }
    }
}
