using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Category
{
    [DataContract]
    public class ResourcesForInstance
    {             
        [DataMember]
        public CPU cpu { get; set; }
        
        [DataMember]
        public Memory memory { get; set; }
                
        [DataMember]
        public Disk disk { get; set; }
        
        [DataMember]
        public DateTime UTCCollectionDateTime { get; set; }

        [DataContract]
        public class CPU
        { 
            [DataMember]
            public double SqlCPUUsage { get; set; }
            
            [DataMember]
            public double OSCPUUsage { get; set; }
                        
            [DataMember]
            public double ProcessorQueueLength { get; set; }

            [DataMember]
            public double ProcessorUserTimePercent { get; set; }

            [DataMember]
            public double ProcessorPrivilegedTimePercent { get; set; }
        }

        [DataContract]
        public class Memory
        {
            [DataMember]
            public SqlMemory SqlMemory { get; set; }

            [DataMember]
            public MemoryAreas MemoryAreas { get; set; }

            [DataMember]
            public Int64 PageLifeExpectancyinSec { get; set; }

            [DataMember]
            public double BufferCacheHitRatio { get; set; }

            [DataMember]
            public double ProcedureCacheHitRatio { get; set; }
        }


        [DataContract]
        public class SqlMemory
        {
            [DataMember]
            public Int64 UsedMemoryInKB { get; set; }
            [DataMember]
            public Int64 AllocatedMemoryInKB { get; set; }
            [DataMember]
            public Int64 TotalMemoryInKB { get; set; }
        }

        [DataContract]
        public class MemoryAreas
        {
            [DataMember]
            public Int64 ProcedureCacheSizeInKB { get; set; }
            [DataMember]
            public Int64 ConnectionsMemoryInKB { get; set; }
            [DataMember]
            public Int64 LocksMemoryInKB { get; set; }
            [DataMember]
            public long DatabaseMemoryInKB { get; set; }
            [DataMember]
            public long FreePagesInKilobytes { get; set; }
            [DataMember]
            public long Others { get; set; }
        }

        [DataContract]
        public class Disk
        {
            [DataMember]
            public IDictionary<string, long> DiskReadsPerSecond { get; set; }

            [DataMember]
            public IDictionary<string, long> DiskWritesPerSecond { get; set; }

            //SQLdm 9.1 (Sanjali Makkar) --Disk Drives -add the parameter of Disk Transfers per second
            [DataMember]
            public IDictionary<string, long> DiskTransfersPerSecond { get; set; }


            //SQLdm 10.2 Nishant Adhikari for Average milliseconds per read write transfer
            //start
            [DataMember]
            public IDictionary<string, long> AverageDiskMillisecondsPerRead { get; set; }
            [DataMember]
            public IDictionary<string, long> AverageDiskMillisecondsPerWrite { get; set; }
            [DataMember]
            public IDictionary<string, long> AverageDiskMillisecondsPerTransfers { get; set; }
            //end

            [DataMember]
            public SqlPhysicalIO SqlPhysicalIO { get; set; }
        }

        [DataContract]
        public class SqlPhysicalIO
        {
            [DataMember]
            public double CheckPointWrites { get; set; }
            
            [DataMember]
            public double LazyWriterWrites { get; set; }

            [DataMember]
            public double ReadAheadPages { get; set; }

            [DataMember]
            public double PageReads { get; set; }

            [DataMember]
            public double PageWrites { get; set; }
        }

    }
}
