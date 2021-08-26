using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DiskQueueLengthRecommendation1 : LogicalDiskRecommendation
    {
        public double AvgDiskQueueLength { get;  set; }
        public double AvgPagesPerSecond  { get;  set; }
        public double AvgDiskSecPerTransfer { get;  set; }
        public DiskQueueLengthRecommendation1(string disk)
            : base(RecommendationType.DiskQueueLength48)
        {
            DiskName = disk;
        }

        public DiskQueueLengthRecommendation1(RecommendationProperties recProp)
            : base(RecommendationType.DiskQueueLength48,recProp)
        {
            AvgDiskQueueLength = recProp.GetDouble("AvgDiskQueueLength");
            AvgPagesPerSecond = recProp.GetDouble("AvgPagesPerSecond");
            AvgDiskSecPerTransfer = recProp.GetDouble("AvgDiskSecPerTransfer");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("AvgDiskQueueLength", AvgDiskQueueLength.ToString());
            prop.Add("AvgPagesPerSecond", AvgPagesPerSecond.ToString());
            prop.Add("AvgDiskSecPerTransfer", AvgDiskSecPerTransfer.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            if (AvgPagesPerSecond >= 20)
                return HIGH_IMPACT;

            return base.AdjustImpactFactor(i);
        }
    }
}
