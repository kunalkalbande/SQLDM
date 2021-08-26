using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Databases
{
    [DataContract]
    public class TempDBStats
    {
        [DataMember]
        public decimal UnallocatedSpaceInMB { get; set; }

        [DataMember]
        public decimal VersionStoreInMB { get; set; }

        [DataMember]
        public decimal UserObjectsInMB { get; set; }

        [DataMember]
        public decimal MixedExtentsInMB { get; set; }

        [DataMember]
        public decimal InternalObjectsInMB { get; set; }

        [DataMember]
        public long TotalSize { get; set; }

        [DataMember]
        public double VersionStoreGenerationKilobytesPerSec { get; set; }

        [DataMember]
        public double VersionStoreCleanupKilobytesPerSec { get; set; }

        [DataMember]
        public long TempdbPFSWaitTimeMilliseconds { get; set; }

        [DataMember]
        public long TempdbGAMWaitTimeMilliseconds { get; set; }

        [DataMember]
        public long TempdbSGAMWaitTimeMilliseconds { get; set; }

        [DataMember]
        public DateTime UTCCollectionDateTime { get; set; }
    }
}
