using Idera.SQLdm.Service.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class ServiceStatus
    {
        [DataMember]
        public int State { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string Url { get; set; }
        //[DataMember]
        //public JsonExceptionWrapper Error { get; set; }

        public ServiceStatus(ServiceState state)
        {
            State = (int)state;
            Message = state.GetDescription();
        }
        public ServiceStatus(ServiceState state, Exception ex)
            : this(state)
        {
            //Error = new JsonExceptionWrapper(ex);
        }
        public ServiceStatus() { }

    }
}
