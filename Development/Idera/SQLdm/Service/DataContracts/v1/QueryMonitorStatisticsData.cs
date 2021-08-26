using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    // SQLdm 9.0 (Abhishek Joshi) --Query Monitoring -response for Query Monitor Statistics

    [DataContract]
    public class QueryMonitorStatisticsData
    {
        [DataMember]
        public int QueryStatisticsID { get; set; } // SQLdm 9.0 (Ankit Srivastava) - Query Plan View - added new property to works as unique key

        [DataMember]
        public string Application { get; set; }

        [DataMember]
        public int Occurrences { get; set; }

        [DataMember]
        public Int64 TotalDuration { get; set; }

        [DataMember]
        public string AvgDuration { get; set; }

        [DataMember]
        public Int64 TotalCPUTime { get; set; }

        [DataMember]
        public Int64 TotalReads { get; set; }

        [DataMember]
        public Int64 TotalWrites { get; set; }

        [DataMember]
        public Int64 TotalIO { get; set; }

        [DataMember]
        public Int64 TotalWaitTime { get; set; }

        [DataMember]
        public string MostRecentCompletion { get; set; }

        [DataMember]
        public Int64 TotalBlockingTime { get; set; }

        [DataMember]
        public Int64 TotalDeadlocks { get; set; }

        [DataMember]
        public string AvgCPUTime { get; set; }

        [DataMember]
        public string AvgCPUPerSec { get; set; }       

        [DataMember]
        public string AvgReads { get; set; }

        [DataMember]
        public string AvgWrites { get; set; }

        [DataMember]
        public string AvgIOPerSec { get; set; }

        [DataMember]
        public string AvgWaitTime { get; set; }

        [DataMember]
        public string AvgBlockingTime { get; set; }

        [DataMember]
        public string AvgDeadlocks { get; set; }

        [DataMember]   
        public int ApplicationID { get; set; }

        [DataMember]
        public int DatabaseID { get; set; }

        [DataMember]
        public int UserID { get; set; }

        [DataMember]
        public int ClientID { get; set; }

        [DataMember]
        public int SQLSignatureID { get; set; }

        [DataMember]
        public int SQLStatementID { get; set; }

        [DataMember]
        public string DatabaseName { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Client { get; set; }

        [DataMember]
        public string SignatureSQLText { get; set; }

        [DataMember]
        public string EventType { get; set; }

        [DataMember]
        public bool KeepDetailedHistory { get; set; }

        [DataMember]
        public bool Aggregated { get; set; }

        [DataMember]
        public string StatementSQLText { get; set; }

        [DataMember]
        public string StartTime { get; set; }

        [DataMember]
        public Int16 Spid { get; set; }

        [DataMember]
        public string QueryName { get; set; }

        [DataMember]
        public string CPUAsPercentOfList { get; set; }

        [DataMember]
        public string ReadsAsPercentOfList { get; set; }
        
    }
}
