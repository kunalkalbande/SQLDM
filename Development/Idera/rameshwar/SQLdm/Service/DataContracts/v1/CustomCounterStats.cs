using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class CustomCounterStats
    {
        [DataMember]
        public int metricID { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public decimal Value { get; set; }
        [DataMember]
        public DateTime UTCDateTime { get; set; }
    }
}
