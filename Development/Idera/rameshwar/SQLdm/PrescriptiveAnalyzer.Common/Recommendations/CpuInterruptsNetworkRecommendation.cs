using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class CpuInterruptsNetworkRecommendation : CpuInterruptsRecommendation
    {
        public ICollection<string> Cards { get; private set; }
        public CpuInterruptsNetworkRecommendation(UInt64 activity, UInt32 interruptsPerSec, double interruptsTime, IEnumerable<string> cards)
            : base(RecommendationType.CpuInterruptsNetwork, activity, interruptsPerSec, interruptsTime)
        {
            if (null != cards) Cards = new List<string>(cards);
        }

        public CpuInterruptsNetworkRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.CpuInterruptsNetwork, recProp)
        {
            Cards = recProp.GetListOfStrings("Cards");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Cards", RecommendationProperties.GetXml<List<string>>((List<string>)Cards));
            return prop;
        }
    }
}
