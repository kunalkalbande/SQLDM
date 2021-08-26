using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DiskQueueLengthRecommendation6 : LogicalDiskRecommendation
    {
        public double AvgDiskQueueLength { get; set; }
        public double AvgDiskSecPerTransfer { get; set; }
        public DiskQueueLengthRecommendation6(string disk) : base(RecommendationType.DiskQueueLength53)
        {
            DiskName = disk;
        }

        public DiskQueueLengthRecommendation6(RecommendationProperties recProp)
            : base(RecommendationType.DiskQueueLength53, recProp)
        {
            AvgDiskQueueLength = recProp.GetInt("AvgDiskQueueLength");
            AvgDiskSecPerTransfer = recProp.GetLong("AvgDiskSecPerTransfer");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("AvgDiskQueueLength", AvgDiskQueueLength.ToString());
            prop.Add("AvgDiskSecPerTransfer", AvgDiskSecPerTransfer.ToString());
            return prop;
        }
    }
}
