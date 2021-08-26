using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DiskQueueLengthRecommendation5 : LogicalDiskRecommendation
    {
        public double AvgDiskQueueLength { get;  set; }
        public double AvgDiskSecPerTransfer { get;  set; }

        public DiskQueueLengthRecommendation5(string disk) : base(RecommendationType.DiskQueueLength52)
        {
            DiskName = disk;
        }
        public DiskQueueLengthRecommendation5(RecommendationProperties recProp)
            : base(RecommendationType.DiskQueueLength52, recProp)
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
