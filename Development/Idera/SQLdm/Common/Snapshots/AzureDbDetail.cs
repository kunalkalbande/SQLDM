using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents basic information on an index
    /// </summary>
    [Serializable]
    public class AzureDbDetail
    {
        private decimal? avgCpuPercent;
        private decimal? avgDataIoPercent;
        private decimal? avgLogWritePercent;
        private int? dtuLimit;
        private decimal? cpuLimit;

        public AzureDbDetail()
        {
        }

        public AzureDbDetail(decimal? avgCpuPercent, decimal? avgDataIoPercent, decimal? avgLogWritePercent, int? dtuLimit, decimal? cpuLimit)
        {
            this.avgCpuPercent = avgCpuPercent;
            this.avgDataIoPercent = avgDataIoPercent;
            this.avgLogWritePercent = avgLogWritePercent;
            this.dtuLimit = dtuLimit;
            this.cpuLimit = cpuLimit;
        }
        public decimal? AvgCpuPercent
        {
            get { return avgCpuPercent; }
            internal set { avgCpuPercent = value; }
        }
        public decimal? AvgDataIoPercent
        {
            get { return avgDataIoPercent; }
            internal set { avgDataIoPercent = value; }
        }

        public decimal? AvgLogWritePercent
        {
            get { return avgLogWritePercent; }
            internal set { avgLogWritePercent = value; }
        }

        public int? DtuLimit
        {
            get { return dtuLimit; }
            internal set { dtuLimit = value; }
        }

        public decimal? CpuLimit
        {
            get { return cpuLimit; }
            internal set { cpuLimit = value; }
        }
    }
}