using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Widgets
{
    [DataContract]
    public class InstanceStatus
    {
        // SQLdm 9.1 (Sanjali Makkar) (Instance Status) : Added to get Instances related information e.g. Critical Servers Count, Total Monitored Servers 

        [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 1)]
        public AlertInstanceStatus AlertStatus { get; set; }

 	    [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 2)]
        public InstanceOverview Overview { get; set; }
    }
}
