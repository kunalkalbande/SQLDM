using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class CpuConstantFrequencyRecommendation : CpuRecommendation
    {
        public double AvgFrequencyPercent { get; private set; }
        public string AvgFrequencyPercentString { get { return (string.Format("{0:0.#}", AvgFrequencyPercent)); } }
        public CpuConstantFrequencyRecommendation(double avgFreqPercent)
            : base(RecommendationType.CpuConstantFrequency)
        {
            AvgFrequencyPercent = avgFreqPercent;
        }

        public CpuConstantFrequencyRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.CpuConstantFrequency, recProp)
        {
            AvgFrequencyPercent = recProp.GetDouble("AvgFrequencyPercent");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("AvgFrequencyPercent", AvgFrequencyPercent.ToString());
            return prop;
        }
    }
}
