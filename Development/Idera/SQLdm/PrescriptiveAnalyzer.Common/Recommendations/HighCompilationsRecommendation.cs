using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class HighCompilationsRecommendation : NetworkRecommendation
    {
        public UInt64 BatchesPerSec { get; private set; }
        public UInt64 CompilationsPerSec { get; private set; }

        public HighCompilationsRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.HighCompilations, recProp)
        {
            BatchesPerSec = recProp.GetUInt64("BatchesPerSec");
            CompilationsPerSec = recProp.GetUInt64("CompilationsPerSec");
        }

        public HighCompilationsRecommendation(UInt64 batchesPerSec, UInt64 compilationsPerSec) : base(RecommendationType.HighCompilations) 
        {
            BatchesPerSec = batchesPerSec;
            CompilationsPerSec = compilationsPerSec;
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("BatchesPerSec", BatchesPerSec.ToString());
            prop.Add("CompilationsPerSec", CompilationsPerSec.ToString());
            return prop;
        }
    }
}
