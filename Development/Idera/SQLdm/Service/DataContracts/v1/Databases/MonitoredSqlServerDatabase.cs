using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Service.DataContracts.v1.Database
{
     [DataContract]
    public class MonitoredSqlServerDatabase
    {
        [DataMember]
         public string DatabaseName { get; set; }

         [DataMember]
         public int TotalAlertCount { get; set; }

         [DataMember]
         public int ActiveAlertCount { get; set; }

         [DataMember]
         public int InstanceId { get; set; }
		
         [DataMember]
		 public int DatabaseId { get; set; }        

         [DataMember]
         public string CurrentDatabaseStatus { get; set; }

         [DataMember]
         public string CurrentDatabaseState { get; set; }

         [DataMember]
         public long Transactions { get; set; }

         [DataMember]
         public IList<Queries> QueryStatistics { get; set; }

         [DataMember]
         public IList<Session> Sessions { get; set; }

        [DataMember(EmitDefaultValue=false)]
         public DateTime LastBackupDate { get; set; }
        [DataMember]
        public string RecoveryModel { get; set; }
        [DataMember]
        public decimal UnUsedDataSizeInMb { get; set; }
        [DataMember]
        public decimal UnUsedLogSizeInMb { get; set; }
        [DataMember]
        public int noOfDataFiles { get; set; }

        [DataMember]
        public int noOfLogFiles { get; set; }

        [DataMember]
        public int UserTables { get; set; }

         #region "Flags"
             [DataMember]
             public bool IsSystemDatabase { get; set; }
             [DataMember]
             public bool IsInstanceEnabled { get; set; }
             
         #endregion

         #region "Database Sizes"
             [DataMember]
             public decimal CurrentTotalFileSizeInMb { get; set; }
             [DataMember]
             public decimal CurrentDataFileSizeInMb { get; set; }
             [DataMember]
             public decimal CurrentLogFileSizeInMb { get; set; }
             [DataMember]
             public decimal CurrentTotalSizeInMb { get; set; }
             [DataMember]
             public decimal CurrentDataSizeInMb { get; set; }
             [DataMember]
             public decimal CurrentLogSizeInMb { get; set; }
             //START SQLdm 10.0 (Sanjali Makkar) - To Add Index Size and Text Size parameters
             [DataMember]
             public decimal CurrentIndexSizeInMb { get; set; }
             [DataMember]
             public decimal CurrentTextSizeInMb { get; set; }
             //END SQLdm 10.0 (Sanjali Makkar) - To Add Index Size and Text Size parameters
         #endregion

         #region "DateTimeStamps"
             [DataMember(EmitDefaultValue=false)]
             public DateTime CreationDateTimeUtc { get; set; }
             [DataMember(EmitDefaultValue=false)]
             public DateTime LatestSizeCollectionDateTimeUtc { get; set; }
             [DataMember(EmitDefaultValue=false)]
             public DateTime LatestStatsCollectionDateTimeUtc { get; set; }             
         #endregion
             

         [DataContract]
         public class Queries
         {
              [DataMember]
              public string AverageDuration { get; set; }
              [DataMember]
              public string AverageCPU { get; set; }
              [DataMember]
              public string AverageReads { get; set; }
              [DataMember]
              public string AverageWrites { get; set; }
              [DataMember]
              public int StatementType { get; set; }
              [DataMember]
              public string StatementText { get; set; }
              [DataMember]
              public int Count { get; set; }
              [DataMember]
              public string ApplicationName { get; set; }
         }

         [DataContract]
         public class Session
         {
             [DataMember]
             public string SessionId { get; set; }
             [DataMember]
             public string CPUMilliSeconds { get; set; }
             [DataMember]
             public string HostName { get; set; }             
         }

		 #region Contructors

             public MonitoredSqlServerDatabase(string databaseName, int alertCount):this()
                {
                    TotalAlertCount = alertCount;
                    DatabaseName = databaseName;
                }

             public MonitoredSqlServerDatabase()
             {
                 QueryStatistics = new List<MonitoredSqlServerDatabase.Queries>();
                Sessions = new List<MonitoredSqlServerDatabase.Session>();
             }
         #endregion
    }
}
