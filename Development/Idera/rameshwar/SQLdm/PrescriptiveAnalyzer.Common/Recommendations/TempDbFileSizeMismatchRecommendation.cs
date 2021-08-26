using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class TempDbFileSizeMismatchRecommendation : Recommendation, IProvideDatabase
    {
        public long SmallestFileSize { get;  set; }
        public long LargestFileSize { get;  set; }
        public string Database { get { return "tempdb"; } }

        public string SmallestFileSizeText { get { return FormatHelper.FormatBytes((ulong)SmallestFileSize); } }
        public string LargestFileSizeText { get { return FormatHelper.FormatBytes((ulong)LargestFileSize); } }

        public TempDbFileSizeMismatchRecommendation() : base(RecommendationType.DiskTempDbSizeMismatch)
        {
        }

        public TempDbFileSizeMismatchRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DiskTempDbSizeMismatch, recProp)
        {
            SmallestFileSize = recProp.GetLong("SmallestFileSize");
            LargestFileSize = recProp.GetLong("LargestFileSize");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("SmallestFileSize", SmallestFileSize.ToString());
            prop.Add("LargestFileSize", LargestFileSize.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            double diff = ((double)SmallestFileSize) / ((double)LargestFileSize);

            if (diff < .10)
                return HIGH_IMPACT;

            if (diff < .50)
                return HIGH_IMPACT - LOW_IMPACT;

            return base.AdjustImpactFactor(i);
        }
    }
}
