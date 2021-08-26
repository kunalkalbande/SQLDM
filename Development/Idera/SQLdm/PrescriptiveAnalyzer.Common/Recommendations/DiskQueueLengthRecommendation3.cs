using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DiskQueueLengthRecommendation3 : LogicalDiskRecommendation
    {
        public double AvgDiskIOSplitRatio { get;  set; }
        //public double AvgDiskQueueLength { get;  set; }
        public double AvgDiskSecPerTransfer { get;  set; }

        public DiskQueueLengthRecommendation3(string disk) : base(RecommendationType.DiskQueueLength50)
        {
            DiskName = disk;
        }

        public DiskQueueLengthRecommendation3(RecommendationProperties recProp)
            : base(RecommendationType.DiskQueueLength50, recProp)
        {
            AvgDiskIOSplitRatio = recProp.GetDouble("AvgDiskIOSplitRatio");
            //AvgDiskQueueLength = recProp.GetDouble("AvgDiskQueueLength");
            AvgDiskSecPerTransfer = recProp.GetDouble("AvgDiskSecPerTransfer");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("AvgDiskIOSplitRatio", AvgDiskIOSplitRatio.ToString());
            //prop.Add("AvgDiskQueueLength", AvgDiskQueueLength.ToString());
            prop.Add("AvgDiskSecPerTransfer", AvgDiskSecPerTransfer.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            if (AvgDiskIOSplitRatio > .10)
                return LOW_IMPACT + 1;

            return base.AdjustImpactFactor(i);
        }
    }
}
