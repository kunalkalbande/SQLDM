using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Widgets
{
    [DataContract]
    public class IOPhysicalUsageForInstance
    {
        [DataMember]
        public Int32 InstanceId { get; set; }

        [DataMember]
        public string InstanceName { get; set; }

        [DataMember]
        public Int32 Severity { get; set; }

        [DataMember]
        public string SQLPhysicalIO { get; set; }



    }
}
