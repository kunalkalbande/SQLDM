using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Services;
//using Idera.SQLdoctor.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class Mem32bit3gOrLessRecommendation : MemRecommendation
    {
        public Mem32bit3gOrLessRecommendation()
            : base(RecommendationType.Mem32bit3gOrLess)
        {
        }

        public Mem32bit3gOrLessRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.Mem32bit3gOrLess, recProp)
        {
        }
        public override Dictionary<string, string> GetProperties()
        {
            return base.GetProperties();
        }
    }
}
