using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class DatabaseStatistics
    {
        [DataMember]
        public decimal UsedSize { get; set; }

        [DataMember]
        public decimal UnusedSize { get; set; }

        [DataMember]
        public float PercentDataSize { get; set; }
    }
}
