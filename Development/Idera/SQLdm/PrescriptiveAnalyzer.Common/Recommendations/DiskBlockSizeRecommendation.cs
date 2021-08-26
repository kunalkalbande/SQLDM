using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DiskBlockSizeRecommendation : LogicalDiskRecommendation
    {
        public ulong AllocationSizeKB { get;  set; }
        public double AvgDiskSecPerTransfer { get;  set; }
        //public double AvgDiskQueueLength { get;  set; }
        public ulong AllocationSize { get; set; }

        public DiskBlockSizeRecommendation(string disk) : base(RecommendationType.DiskBlockSize)
        {
            DiskName = disk;
        }

        public DiskBlockSizeRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DiskBlockSize, recProp)
        {
            AllocationSizeKB = recProp.GetULong("AllocationSizeKB");
            AvgDiskSecPerTransfer = recProp.GetDouble("AvgDiskSecPerTransfer");
            //AvgDiskQueueLength = recProp.GetDouble("AvgDiskQueueLength");
            AllocationSize = recProp.GetULong("AllocationSize");
        } 
    }
}
