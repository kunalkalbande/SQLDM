using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Auth
{
    [DataContract]
    public class AppSecurityEnabledResponse
    {
        [DataMember]
        public bool IsSecurityEnabled { get; set; }
    }
}
