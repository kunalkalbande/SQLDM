using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Widgets
{
    [DataContract]
    public class DatabasesByDatabaseFileSize
    {
        [DataMember]
        public int InstanceId { get; set; }

        [DataMember]
        public string InstanceName { get; set; }

        [DataMember]
        public string DatabaseName { get; set; }

        [DataMember]
        public decimal FileSizeInMB { get; set; }

        [DataMember]
        public int Severity { get; set; }

        [DataMember]
        public DateTime? UTCCollectionDateTime { get; set; }
    }
}
