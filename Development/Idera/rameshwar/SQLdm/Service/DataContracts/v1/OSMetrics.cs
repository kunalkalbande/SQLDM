using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class OSMetrics
    {
        [DataMember]
        public double PercentDiskIdleTime;
        [DataMember]
        public double PercentDiskTime { get; set; }
        [DataMember]
        public double AvgDiskQueueLength { get; set; }
        [DataMember]
        public decimal AvailableBytes { get; set; }
        [DataMember]
        public double PercentPrivilegedTime { get; set; }
        [DataMember]
        public double ProcessorQueueLength { get; set; }
        [DataMember]
        public double PercentProcessorTime { get; set; }
        [DataMember]
        public double PercentUserTime { get; set; }
        [DataMember]
        public decimal TotalPhysicalMemory { get; set; }
        [DataMember]
        public double PagesPersec { get; set; }
    }    
}
