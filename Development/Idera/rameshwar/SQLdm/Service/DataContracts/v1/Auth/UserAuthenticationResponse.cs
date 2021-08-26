using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Idera.SQLdm.Service.Core.Enums;

namespace Idera.SQLdm.Service.DataContracts.v1.Auth
{
    [DataContract]
    public class UserAuthenticationResponse
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string UserType { get; set; }

        [DataMember]
        public bool IsAuthentic { get; set; }
    }
}
