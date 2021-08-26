using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Services;
//using Idera.SQLdoctor.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class MemFileSharingRecommendation : MemRecommendation
    {
        public MemFileSharingRecommendation()
            : base(RecommendationType.MemFileSharing)
        {
        }

        public MemFileSharingRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.MemFileSharing, recProp)
        {
        }

        public override Dictionary<string, string> GetProperties()
        {
            return base.GetProperties();
        }
    }
}
