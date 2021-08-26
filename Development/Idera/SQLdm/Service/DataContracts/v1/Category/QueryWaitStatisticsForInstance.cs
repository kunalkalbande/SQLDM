using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Category
{
    [DataContract]
    public class QueryWaitStatisticsForInstance
    {
        [DataMember]
        public DateTime StatementUTCStartTime{ get; set; }

        [DataMember]
        public int WaitTypeID { get; set; }

        [DataMember]
        public string WaitType {get; set;}

        [DataMember]
        public int WaitCategoryID { get; set; }

        [DataMember]
        public string WaitCategory { get; set; }

        [DataMember]
        public decimal WaitDurationPerSecond { get; set; }

        [DataMember]
        public Int16 SessionID { get; set; }

        [DataMember]
        public int HostID { get; set; }

        [DataMember]
        public string HostName { get; set; }

        [DataMember]
        public int ApplicationID { get; set; }

        [DataMember]
        public string ApplicationName { get; set; }

        [DataMember]
        public int LoginID { get; set; }

        [DataMember]
        public string LoginName { get; set; }

        [DataMember]
        public int DatabaseID { get; set; }

        [DataMember]
        public string DatabaseName { get; set; }
        
        [DataMember]
        public int SQLStatementID { get; set; }

        [DataMember]
        public string SQLStatement { get; set; }
    }
}
