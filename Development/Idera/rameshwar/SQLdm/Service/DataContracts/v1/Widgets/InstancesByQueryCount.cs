using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Widgets
{
    [DataContract]
    public class InstancesByQueryCount
    {
        [DataMember]
        public int InstanceId { get; set; }

        [DataMember]
        public string InstanceName { get; set; }

        [DataMember]
        public DateTime UTCCollectionDateTime { get; set; }

        [DataMember]
        public int Severity { get; set; }

        [DataMember]
        public int NoOfQueries { get; set; }


    }
}
