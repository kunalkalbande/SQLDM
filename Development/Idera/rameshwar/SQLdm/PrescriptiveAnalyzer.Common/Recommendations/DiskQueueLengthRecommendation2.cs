using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DiskQueueLengthRecommendation2 : LogicalDiskRecommendation
    {
        public double AvgDiskQueueLength { get;  set; }
        public double AvgDiskSecPerTransfer { get;  set; }

        public DiskQueueLengthRecommendation2(string disk) : base(RecommendationType.DiskQueueLength49)
        {
            DiskName = disk;
        }

        public DiskQueueLengthRecommendation2(RecommendationProperties recProp)
            : base(RecommendationType.DiskQueueLength49, recProp)
        {
            AvgDiskQueueLength = recProp.GetDouble("AvgDiskQueueLength");
            AvgDiskSecPerTransfer = recProp.GetDouble("AvgDiskSecPerTransfer");
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
