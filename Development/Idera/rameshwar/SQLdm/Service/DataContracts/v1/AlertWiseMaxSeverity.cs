using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
//SQLdm 10.2 (Anshika Sharma) : Added to Show CategoryWaiseMaxAlerts
namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class AlertWiseMaxSeverity
    {
        [DataMember]
        public string Cpu { get; set; }

        [DataMember]
        public string Memory { get; set; }

        [DataMember]
        public string IO { get; set; }

        [DataMember]
        public string Databases { get; set; }

        [DataMember]
        public string Logs { get; set; }

        [DataMember]
        public string Queries { get; set; }

        [DataMember]
        public string Services { get; set; }

        [DataMember]
        public string Sessions { get; set; }

        [DataMember]
        public string Virtualization { get; set; }

        [DataMember]
        public string Operational { get; set; }

        public AlertWiseMaxSeverity()
        {
            Cpu = null;
            Memory = null;
            IO = null;
            Databases = null;
            Logs = null;
            Queries = null;
            Services = null;
            Sessions = null;
            Virtualization = null;
            Operational = null;
        }
    }
}
