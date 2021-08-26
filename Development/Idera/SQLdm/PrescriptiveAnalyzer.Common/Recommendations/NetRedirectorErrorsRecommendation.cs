using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// Network repot #79 for network redirector errors
    /// </summary>
    [Serializable]
    public class NetRedirectorErrorsRecommendation : NetworkRecommendation
    {
        public UInt32 Errors { get; set; }

        public NetRedirectorErrorsRecommendation() : base(RecommendationType.NetRedirectorErrors, "SDR-N5") { }

        public NetRedirectorErrorsRecommendation(UInt32 errors)
            : this()
        {
            Errors = errors;
        }

        public NetRedirectorErrorsRecommendation(RecommendationProperties recProp) :
            base(RecommendationType.NetRedirectorErrors, "SDR-N5", recProp)
        {
            Errors = recProp.GetUInt32("Errors");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Errors", Errors.ToString());
            return prop;
        }

    }
}
