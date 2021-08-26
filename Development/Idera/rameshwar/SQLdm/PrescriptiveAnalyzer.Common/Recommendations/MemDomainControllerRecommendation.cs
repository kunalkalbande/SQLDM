using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Helpers;
//using Idera.SQLdoctor.Common.Services;
//using Idera.SQLdoctor.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class MemDomainControllerRecommendation : MemRecommendation
    {
        public MemDomainControllerRecommendation()
            : base(RecommendationType.MemDomainController)
        {
        }

        public MemDomainControllerRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.MemDomainController, recProp)
        {
        }

        public override Dictionary<string, string> GetProperties()
        {
            return base.GetProperties();
        }
    }
}
