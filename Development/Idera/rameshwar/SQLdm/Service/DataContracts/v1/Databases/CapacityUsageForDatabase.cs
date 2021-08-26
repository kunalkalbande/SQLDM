using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Databases
{
    [DataContract]
    public class CapacityUsageForDatabase
    {

        [DataMember]
        public double DataFileSizeInMb {get; set;}

        [DataMember]
        public double UnusedDataSizeInMb { get; set; }

        [DataMember]
        public double LogFileSizeInMb { get; set; }

        [DataMember]
        public double UnusedLogSizeInMb { get; set; }

        [DataMember]
        public int NoOfFiles { get; set; }

        [DataMember]
        public DateTime UTCCollectionDateTime { get; set; }
    }
    
}
