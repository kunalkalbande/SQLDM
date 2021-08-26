using System;
//using Idera.SQLdoctor.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class MixedModeAuthenticationRecommendation : Recommendation
    {
        public MixedModeAuthenticationRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.MixedModeAuthentication, recProp)
        {

        }

        public MixedModeAuthenticationRecommendation()
            : base(RecommendationType.MixedModeAuthentication)
        {

        }

        public override System.Collections.Generic.Dictionary<string, string> GetProperties()
        {
            return base.GetProperties();
        }
    }
}