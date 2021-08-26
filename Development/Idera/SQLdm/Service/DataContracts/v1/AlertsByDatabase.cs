using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class AlertsByDatabase
    {
        [DataMember]
        public Int32 InstanceId { get; set; }
        [DataMember]
        public string InstanceName { get; set; }
        [DataMember]
        public string DatabaseName { get; set; }
        [DataMember]
        public Int64 NumOfAlerts { get; set; }
    }
}
