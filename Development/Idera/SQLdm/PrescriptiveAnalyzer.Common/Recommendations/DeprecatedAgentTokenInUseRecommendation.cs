using System;
using System.Collections.Generic;
//using Idera.SQLdoctor.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DeprecatedAgentTokenInUseRecommendation : Recommendation
    {
        public readonly string JobName;
        public readonly string StepName;

        public DeprecatedAgentTokenInUseRecommendation(string JobName, string StepName)
            : base(RecommendationType.DeprecatedAgentTokenInUse)
        {
            this.JobName = JobName;
            this.StepName = StepName;
        }

        public DeprecatedAgentTokenInUseRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DeprecatedAgentTokenInUse, recProp)
        {
            JobName = recProp.GetString("JobName");
            StepName = recProp.GetString("StepName");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("JobName", JobName.ToString());
            prop.Add("StepName", StepName.ToString());
            return prop;
        }

    }
}