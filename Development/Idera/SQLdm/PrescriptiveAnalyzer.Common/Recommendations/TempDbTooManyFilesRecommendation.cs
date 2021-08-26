using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class TempDbTooManyFilesRecommendation : Recommendation, IProvideDatabase
    {
        public int ProcessorCount { get;  set; }
        public int FileCount { get;  set; }
        public string Database { get { return "tempdb"; } }


        public TempDbTooManyFilesRecommendation() : base(RecommendationType.DiskTempDbTooManyFiles)
        {
        }

        public TempDbTooManyFilesRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DiskTempDbTooManyFiles, recProp)
        {
            ProcessorCount = recProp.GetInt("ProcessorCount");
            FileCount = recProp.GetInt("FileCount");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("FileCount", FileCount.ToString());
            prop.Add("ProcessorCount", ProcessorCount.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            if (FileCount > 16 || FileCount > (ProcessorCount * 3))
                return HIGH_IMPACT;

            return base.AdjustImpactFactor(i);
        }
    }
}
