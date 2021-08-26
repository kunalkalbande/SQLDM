using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class Alert
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
        public int Severity { get; set; }

        //START SQLdm10.0 Srishti PUrohit -- For PreviousAlertSeverity requirement change
        [DataMember]
        public int PreviousAlertSeverity { get; set; }
        //END SQLdm10.0 Srishti PUrohit -- For PreviousAlertSeverity requirement change

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

    [DataContract]
    public class AlertsV2
    {
        [DataMember]
        public IList<GridAlert> Alerts { get; set; }

        [DataMember]
        public int totalAlerts { get; set; }
    }

}
