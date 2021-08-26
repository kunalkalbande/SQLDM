using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Widgets
{
    [DataContract]
    public class InstanceOverview
    {
        // SQLdm 9.1 (Sanjali Makkar) (Instance Status) : Added to get the count of Monitored Instances and Disabled Instances

        [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 1)]
        public int MonitoredServersCount { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 2)]
        public int DisabledServersCount { get; set; }
    }
}