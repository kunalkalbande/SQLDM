using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class TimedValue
    {

        [DataMember]
        public double Value { get; set; }

        [DataMember]
        public DateTime UTCCollectionDateTime { get; set; } 

    }
}
