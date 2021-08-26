using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Idera.SQLdm.Service.DataContracts.v1;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class GridAlert
    {
        [DataMember]
        public Int64 AlertId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime UTCOccurrenceDateTime { get; set; }

        [DataMember]
        public string ServerName { get; set; }

        [DataMember]
        public string DatabaseName { get; set; }

        [DataMember]
        public int? SQLServerId { get; set; }

        [DataMember]
        public int IsActive { get; set; }

        [DataMember]
        public Metric Metric { get; set; }

        [DataMember]
        public string Severity { get; set; }

        [DataMember]
        public int PreviousAlertSeverity { get; set; }

        [DataMember]
        public int? StateEvent { get; set; }

        [DataMember]
        public double? Value { get; set; }

        [DataMember]
        public string StringValue { get; set; }

        [DataMember]
        public string Heading { get; set; }

        [DataMember]
        public string Message { get; set; }
    }

}
