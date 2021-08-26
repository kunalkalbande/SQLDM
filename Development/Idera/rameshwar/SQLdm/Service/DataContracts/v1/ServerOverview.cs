using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.VMware;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class ServerOverview
    {
        #region fields

        [DataMember]
        public string agentServiceStatus = Enum.GetName(ServiceState.UnableToMonitor.GetType(),ServiceState.UnableToMonitor);

        [DataMember]
        public string dtcServiceStatus = Enum.GetName(ServiceState.UnableToMonitor.GetType(), ServiceState.UnableToMonitor);

        [DataMember]
        public string fullTextServiceStatus = Enum.GetName(ServiceState.UnableToMonitor.GetType(), ServiceState.UnableToMonitor);

        [DataMember]
        public string sqlServiceStatus = Enum.GetName(ServiceState.UnableToMonitor.GetType(), ServiceState.UnableToMonitor);

        //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --add serve status field for server overview tab
        [DataMember]
        public string sqlBrowserServiceStatus = Enum.GetName(ServiceState.UnableToMonitor.GetType(), ServiceState.UnableToMonitor);

        [DataMember]
        public string sqlActiveDirectoryHelperServiceStatus = Enum.GetName(ServiceState.UnableToMonitor.GetType(), ServiceState.UnableToMonitor);
        //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --add serve status field for server overview tab

        [DataMember]
        public string language = null;

        [DataMember]
        public bool loginHasAdministratorRights = false;

        [DataMember]
        public Int64 maxConnections = 0;

        [DataMember]
        public decimal physicalMemory;

        [DataMember]
        public int processorCount = 0;

        [DataMember]
        public int processorsUsed = 0;

        [DataMember]
        public string processorType = null;

        [DataMember]
        public string serverHostName = null;

        [DataMember]
        public string realServerName = null;

        [DataMember]
        public string windowsVersion = null;

        [DataMember]
        public string sqlServerEdition = null;

        [DataMember]
        public ServerStatistics statistics = null;

        [DataMember]
        public decimal targetServerMemory;

        [DataMember]
        public decimal totalServerMemory;

        [DataMember]
        public bool isClustered;

        [DataMember]
        public string clusterNodeName;

        [DataMember]
        public decimal procedureCacheSize;

        [DataMember]
        public double procedureCachePercentageUsed;

        [DataMember]
        public OSMetrics osMetricsStatistics;
         
        //[DataMember]
        //public ServerLoginConfiguration loginConfiguration;

        [DataMember]
        public long responseTime;

        [DataMember(EmitDefaultValue=false)]
        public DateTime runningSince;

        [DataMember]
        public bool maintenanceModeEnabled { get; set; }

        [DataMember]
        public ServerSystemProcesses systemProcesses;

        [DataMember]        
        public ServerDatabaseSummary databaseSummary;

        //[DataMember]        
        //public decimal totalLocks = 0;

        //[DataMember]
        //public TimeSpan timeDelta = new TimeSpan(0);

        //[DataMember]
        //public LockStatistics lockCounters;

        //[DataMember]
        //public Memory memoryStatistics = new Memory();

        //[DataMember]
        //public Dictionary<string, DiskDrive> diskDrives = new Dictionary<string, DiskDrive>();

        //[DataMember]
        //public bool isFileSystemObjectDataAvailable = true;

        [DataMember]
        public TempdbSummaryStatistics TempDBSummary = new TempdbSummaryStatistics();

        [DataMember]
        public DatabaseStatistics TempDBStatistics = new DatabaseStatistics();

        //[DataMember]
        //public Dictionary<string, FileActivityFile> fileActivity = new Dictionary<string, FileActivityFile>();

        //[DataMember]
        //public VMwareVirtualMachine vmConfig = null;

        //[DataMember]
        //public WaitStatisticsSummary waitStatSummary = null;



        [DataMember]
        public string ProductVersion { get; set; }
        [DataMember]
        public string ProductEdition { get; set; }

        [DataMember]
        public string InstanceName {get; set;}

        [DataMember]
        public int SQLServerId {get; set;}

        //SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
        [DataMember]
        public string FriendlyServerName { get; set; }
        // end 10.1- (Pulkit Puri)

        //SQLdm 10.1 - (Barkha khatri) 
        //SQLDM 26533 fix-shiftng tags from MonitoredSqlServer to serverOverview
        [DataMember]
        public string[] Tags { get; set; }

        ////SQLDM-29855. Update SWA Icon in Dashboard
        //[DataMember]
        //public bool isSWAInstance { get; set; }

        ////SQLDM-29855. Update SWA Icon in Dashboard
        //[DataMember]
        //public string SWAUrl { get; set; }
        #endregion
    }
}
