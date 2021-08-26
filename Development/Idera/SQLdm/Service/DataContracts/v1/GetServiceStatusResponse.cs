using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class GetServiceStatusResponse
    {
        [DataMember]
        public bool? CanRun { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public List<ServiceStatus> ServiceStatuses { get; set; }

        public GetServiceStatusResponse() { }

    }
}
