using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Databases
{
    [DataContract]
    public class AvailabilityGroupSummaryForDatabase
    {
        //[DataMember]
        //public string DatabaseName { get; set; }
        [DataMember]
        public string GroupName { get; set; }
        [DataMember]
        public string ReplicaName { get; set; }
        [DataMember]
        public string ReplicaRole { get; set; }
        [DataMember]
        public string SyncHealth { get; set; }
        [DataMember]
        public double RedoQueueSize { get; set; }
        [DataMember]
        public double RedoTransferRate { get; set; }
        [DataMember]
        public double LogSendQueueSize { get; set; }
        [DataMember]
        public double LogTransferRate { get; set; }
        [DataMember]
        public string DatabaseStatus { get; set; }
        [DataMember]
        public DateTime UTCCollectionDateTime { get; set; }
    }    
}
