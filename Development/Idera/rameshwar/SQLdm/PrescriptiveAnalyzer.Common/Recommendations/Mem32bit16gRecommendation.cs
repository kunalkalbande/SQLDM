using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Services;
//using Idera.SQLdoctor.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class Mem32bit16gRecommendation : MemRecommendation
    {
        public Mem32bit16gRecommendation()
            : base(RecommendationType.Mem32bit16g)
        {
        }

        public Mem32bit16gRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.Mem32bit16g, recProp)
        {
        }
        public override Dictionary<string, string> GetProperties()
        {
            return base.GetProperties();
        }
    }
}
