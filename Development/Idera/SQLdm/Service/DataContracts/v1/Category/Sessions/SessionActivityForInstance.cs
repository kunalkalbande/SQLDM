using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Category.Sessions
{
    [DataContract]
    public class SessionActivityForInstance
    {
        [DataMember]
        public SessionType type { get; set; }

        [DataMember]
        public TimeSpan timeinmils { get; set; }
    }
}
