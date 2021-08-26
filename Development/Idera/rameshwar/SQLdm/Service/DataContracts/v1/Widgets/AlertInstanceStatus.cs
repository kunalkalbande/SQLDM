using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Widgets
{
    [DataContract]
    public class AlertInstanceStatus
    {
        // SQLdm 9.1 (Sanjali Makkar) (Instance Status) : To tell the count of Instances which are at Critical level, Warning level or Informational level
        //Instances having atleast 1 critical alert are considered to be at Critical level

        [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 1)]
        public int CriticalInstancesCount { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 2)]
        public int WarningInstancesCount { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 3)]
        public int InformationalInstancesCount { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 4)]
        public int OkInstancesCount { get; set; }
    }
}
