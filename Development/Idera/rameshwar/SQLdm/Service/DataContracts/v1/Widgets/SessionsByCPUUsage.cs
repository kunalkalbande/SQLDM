using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Widgets
{
    [DataContract()]
    public class SessionsByCPUUsage
    {
        [DataMember]
        public string InstanceName { get; set; }

        [DataMember]
        public int InstanceId { get; set; }

        [DataMember]
        public int Severity { get; set; }

        [DataMember]
        public long SessionID { get; set; }

        [DataMember]
        public long CPUUsageInMillisec { get; set; }

        [DataMember]
        public DateTime UTCCollectionDateTime { get; set; }

        [DataMember]
        public string Host { get; set; }

        [DataMember]
        public string DatabaseName { get; set; }
    }
}
