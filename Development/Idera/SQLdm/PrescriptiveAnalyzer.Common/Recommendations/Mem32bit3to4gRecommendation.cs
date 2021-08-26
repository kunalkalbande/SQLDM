using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Services;
//using Idera.SQLdoctor.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class Mem32bit3to4gRecommendation : MemRecommendation
    {
        public UInt64 TotalPhysicalMemory { get; private set; }
        public Int64 SuggestedUSERVA { get; private set; }

        public Mem32bit3to4gRecommendation()
            : base(RecommendationType.Mem32bit3to4g) { }

        public Mem32bit3to4gRecommendation(UInt64 totalPhysicalMemory)
            : base(RecommendationType.Mem32bit3to4g)
        {
            TotalPhysicalMemory = totalPhysicalMemory;
            try
            {
                SuggestedUSERVA = (Int64)(TotalPhysicalMemory / (1024 * 1024)) - 1350;
            }
            catch { }
        }

        public Mem32bit3to4gRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.Mem32bit3to4g, recProp)
        {
            TotalPhysicalMemory = recProp.GetUInt64("TotalPhysicalMemory");
            try
            {
                SuggestedUSERVA = (Int64)(TotalPhysicalMemory / (1024 * 1024)) - 1350;
            }
            catch { }
        }
        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("TotalPhysicalMemory", TotalPhysicalMemory.ToString());
            return prop;
        }
    }
}
