using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Widgets
{
    [DataContract]
    public class LongestQueriesForInstance
    {
        [DataMember]
        public Int32 InstanceId { get; set; }

        [DataMember]
        public string InstanceName { get; set; }

        [DataMember]
        public string Database { get; set; }

        [DataMember]
        public string QueryText { get; set; }

        [DataMember]
        public long QueryExecTimeInMs { get; set; }

        [DataMember]
        public long CPUTime { get; set; }

        [DataMember]
        public long PhysicalReads { get; set; }

        [DataMember]
        public long LogicalReads { get; set; }

        [DataMember]
        public long LogicalWrites { get; set; }

        [DataMember]
        public DateTime UTCCollectionDateTime { get; set; }       
    }
}
