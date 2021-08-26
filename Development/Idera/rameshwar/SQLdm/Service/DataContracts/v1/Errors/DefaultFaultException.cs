using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Errors
{
    [DataContract]
    public class DefaultFaultException
    {
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public DefaultFaultException InnerException { get; set; }

        public DefaultFaultException(Exception ex)
        {
            if (null == ex) return;
            this.Message = ex.Message;
            this.Type = ex.GetType().ToString();
        }

    }
}
