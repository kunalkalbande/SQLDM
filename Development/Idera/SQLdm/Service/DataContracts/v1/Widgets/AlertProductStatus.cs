using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Widgets
{
    [DataContract]
    public class AlertProductStatus
    {
        [DataMember(EmitDefaultValue=true,IsRequired=true,Order=1)]
        public Int64 CriticalAlertCount { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 2)]
        public Int64 WarningAlertCount { get; set; }

        [DataMember(EmitDefaultValue = true, IsRequired = true, Order = 3)]
        public Int64 InformationalAlertCount { get; set; }


    }
}
