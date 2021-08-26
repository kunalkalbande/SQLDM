using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Category.Sessions
{
    [DataContract]
    public class SessionResponseTimeForInstance
    {
        [DataMember]
        public SessionType type { get; set; }

        [DataMember]
        public long timeinmils { get; set; }
    }

    [DataContract]
    public enum SessionType
    {
        [EnumMember]
        Active,
        [EnumMember]
        Idle,
        [EnumMember]
        System
    }
}
