using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class AlertsByCategory
    {
        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public Int64 NumOfAlerts { get; set; }

        [DataMember(EmitDefaultValue=false)]
        public int? InstanceID { get; set; }
    }
}
 