using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Services;
//using Idera.SQLdoctor.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class Mem64bitLockPagesRecommendation : MemRecommendation
    {
        public Mem64bitLockPagesRecommendation()
            : base(RecommendationType.Mem64bitLockPages)
        {
        }

        public Mem64bitLockPagesRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.Mem64bitLockPages, recProp)
        {
        }

        public override Dictionary<string, string> GetProperties()
        {
            return base.GetProperties();
        }
    }
}
