using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Widgets
{
    [DataContract]
    public class TempDBUtilizationForInstance
    {
        [DataMember]
        public string InstanceName { get; set; }

        [DataMember]
        public int InstanceId { get; set; }

        [DataMember]
        public Int64 TempDBUsageInKB { get; set; }

        [DataMember]
        public DateTime UTCCollectionDateTime { get; set; }


    }
}
