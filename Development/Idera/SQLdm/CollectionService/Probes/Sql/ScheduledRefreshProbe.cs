//------------------------------------------------------------------------------
// <copyright file="ScheduledRefreshProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using BBS.TracerX;
using Idera.SQLdm.CollectionService.Configuration;
using Idera.SQLdm.CollectionService.Helpers;
using Idera.SQLdm.CollectionService.Monitoring;
using Idera.SQLdm.CollectionService.Probes.Collectors;
using Idera.SQLdm.CollectionService.Probes.Wmi;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.HyperV;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.Replication;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
//using Idera.SQLdm.Common.SQLsafe;
using Idera.SQLdm.Common.Snapshots.AlwaysOn;
using Idera.SQLdm.Common.VMware;
using Microsoft.SqlServer.XEvent;
using Microsoft.SqlServer.XEvent.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Wintellect.PowerCollections;
using System.Xml;
using Idera.SQLdm.Common.Snapshots.Cloud;

namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    /// <summary>
    /// Probe for scheduled refresh
    /// </summary>
    class ScheduledRefreshProbe : SqlBaseProbe
    {
        /// <summary>
        /// No data in query store
        /// </summary>
        private const string NoDataQs = "InterpretQueryMonitorDataQs: No query monitor data found using query store.";

        /// <summary>
        /// TextData Column stores query text
        /// </summary>
        private const string TextDataColumn = "TextData";

        /// <summary>
        /// Stores name from sys.object in query store
        /// </summary>
        private const string ObjectnameColumn = "ObjectName";

        /// <summary>
        /// "--" Sql Comment
        /// </summary>
        private const string SqlComment = "-- ";

        /// <summary>
        /// Statement Type determined by sys.objects for query store
        /// </summary>
        private const string StatementTypeColumn = "StatementType";

        /// <summary>
        /// P = SQL Stored Procedure
        /// </summary>
        private const string P_SqlStoredProcedure = "P";

        /// <summary>
        /// PC = Assembly(CLR) stored - procedure
        /// </summary>
        private const string Pc_AssemblyStoredProcedure = "PC";

        /// <summary>
        /// RF = Replication - filter - procedure        
        /// </summary>
        private const string Rf_ReplicationFilterProcedure = "RF";

        /// <summary>
        /// X = Extended stored procedure
        /// </summary>
        private const string X_ExtendedStoredProcedure = "X";

        /// <summary>
        /// TA = Assembly(CLR) DML trigger  
        /// </summary>
        private const string Ta_AssemblyDmlTrigger = "TA";

        /// <summary>
        /// TR = SQL DML trigger 
        /// </summary>
        private const string Tr_SqlDmlTrigger = "TR";

        /// <summary>
        /// AF = Aggregate function(CLR)
        /// </summary>
        private const string AF_AggregateFunction = "AF";

        /// <summary>
        /// FN = SQL scalar function
        /// </summary>
        private const string FnScalarFunction = "FN";

        /// <summary>
        /// FS = Assembly(CLR) scalar - function
        /// </summary>
        private const string FsScalarFunction = "FS";

        /// <summary>
        /// FT = Assembly(CLR) table - valued function
        /// </summary>
        private const string Ft_AssemblyTableValuedFunction = "FT";

        /// <summary>
        /// IF = SQL inline table-valued function
        /// </summary>
        private const string If_InlineTableValuedFunction = "IF";

        /// <summary>
        /// TF = SQL table - valued - function
        /// </summary>
        private const string Tf_TableValuedFunction = "TF";

        /// <summary>
        /// Duration Column
        /// </summary>
        private const string DurationColumn = "Duration";

        /// <summary>
        /// Database Name
        /// </summary>
        private const string DbNameColumn = "DBName";

        /// <summary>
        /// Execution Time for Query
        /// </summary>
        private const string StartTimeColumn = "StartTime";

        /// <summary>
        /// NT User Name from session
        /// </summary>
        private const string UserNameColumn = "NTUserName";

        /// <summary>
        /// Client Name from session
        /// </summary>
        private const string HostnameColumn = "HostName";

        /// <summary>
        /// Program Name from session
        /// </summary>
        private const string ApplicationNameColumn = "ApplicationName";

        /// <summary>
        /// Login name
        /// </summary>
        private const string LoginNameColumn = "LoginName";

        /// <summary>
        /// IO Reads for Query
        /// </summary>
        private const string IoReadsColumn = "Reads";

        /// <summary>
        /// IO Writes for Query
        /// </summary>
        private const string IoWritesColumn = "Writes";

        /// <summary>
        /// CPU Time
        /// </summary>
        private const string CpuColummn = "CPU";

        /// <summary>
        /// Session Id
        /// </summary>
        private const string SpidColumn = "SPID";

        /// <summary>
        /// Query Plan
        /// </summary>
        private const string QueryPlanColumn = "QueryPlan";
        private bool initializedUnfilteredTableFromXML = false;
        private DataTable unfilteredTableFromXML;
        #region fields

        private ScheduledRefresh refresh = null;
        private ScheduledRefresh previousRefresh = null;
        private Stopwatch responseTimeStopWatch = new Stopwatch();
        private MonitoredServerWorkload workload;
        private QueryMonitorConfiguration queryMonitorConfiguration;
        private ActivityMonitorConfiguration activityMonitorConfiguration;
        private HandleSqlErrorDelegate sqlErrorDelegate;
        private int completionCallbackCount;
        private EventHandler<CollectorCompleteEventArgs> connectionStatusCallback = null;
        private ClusterCollectionSetting clusterCollectionSetting = ClusterCollectionSetting.Default;

        private Regex ErrorLogSeverity = new Regex(@"(?<=Severity:\s+)[0-9]+");
        private Regex AgentLogPattern2000 = new Regex(@"(?<=-\s)[?+!](?=\s\[)");

        private QueryMonitorCollectionState qmCollectionState;
        private ActivityMonitorCollectionState activityMonitorCollectionState;

        private SQLsafeRepositoryConfiguration sqlSafeConfig;
        private WmiConfiguration wmiConfig;
        private ServiceStatusProbe statusProbe;
        private OSStatisticsWmiMiniProbe osProbe;
        private DriveStatisticsWmiProbe driveProbe;
        private string machineName;
        private readonly string queryMonitorLogStartTemplate = "Query Monitor - "; // SqlDM 10.2 (Anshul Aggarwal) - Template for logs for query monitor
        private readonly string activityMonitorLogStartTemplate = "Activity Monitor - "; // SqlDM 10.3 (Tushar) - Template for logs for activity monitor
        private IAsyncResult statusIAR;
        private bool initializedUnfilteredAMTableFromXML = false;
        private DataTable unfilteredAMTableFromXML;

        //SQLDM 10.3 : Technical debt changes
        private string currentFileName;
        private string dbLastFileName;
        private long lastFileRecordCount;
        private long dbLastRecordCount;

        //Added for DiskSize2005
        private object driveProbeData;
        private HashSet<string> systemDbNames = new HashSet<string>() {"model", "msdb", "tempdb", "master"};
        private List<string> cloudDBNames = new List<string>();
        private List<string> cloudDBNamesQMAzure = new List<string>();
        private List<string> cloudDBNamesAMAzure = new List<string>();
        private int numberOfDatabases;
        private int numberOfDatabasesAM;
        private int numberOfDatabasesQM;
        private object serverDetailsDbLockObject = new object();
        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the ScheduledRefreshProbe class.
        /// </summary>
        /// <param name="monitoredServer">The monitored server.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public ScheduledRefreshProbe(MonitoredSqlServer monitoredServer, MonitoredServerWorkload workload, int? cloudProviderId)
            : base(monitoredServer.ConnectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
         
            sqlErrorDelegate = new HandleSqlErrorDelegate(HandleSqlException);
            this.workload = workload;
            refresh = new ScheduledRefresh(monitoredServer);
            this.cloudProviderId = workload.MonitoredServer.CloudProviderId;//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Defining cloud provider id, which is a protected data member of base class

            if (workload.PeriodicRefreshPreviousValues.ContainsKey(typeof(DatabaseSizeSnapshot)))
            {
                DatabaseSizeSnapshot db = (DatabaseSizeSnapshot)workload.PeriodicRefreshPreviousValues[typeof(DatabaseSizeSnapshot)];

                if (db != null)
                {
                    // Pull in the database summary from the database periodic collector so this can be saved with ServerStatistics
                    refresh.Server.DatabaseSummary = db.DatabaseSummary;

                    //We need to use whatever db statistics we have for the scheduled refresh
                    //specifically the tempdb details come from the less frequently executed database 
                    //refresh and so we must use these until new ones are available

                    //if the database details collection has left us with db statistics
                    if (refresh.DbStatistics != null)
                    {
                        if (db.DbStatistics.ContainsKey("tempdb"))
                        {
                            if (refresh.DbStatistics.ContainsKey("tempdb"))
                            {
                                refresh.DbStatistics["tempdb"] = db.DbStatistics["tempdb"];
                            }
                            else
                            {
                                //if the existing dbstatistics is empty then use the previous one passed in from the workload
                                //better'n nuttin
                                if (refresh.DbStatistics.Count == 0)
                                {
                                    refresh.DbStatistics = CloneDdStatistics(db.DbStatistics);
                                }
                            }
                        }
                        else
                        {
                            LOG.Debug("tempdb was not found in database statistics or in the current refresh.");
                        }
                    }
                }
            }

            previousRefresh = (ScheduledRefresh)workload.PreviousRefresh;
            if (previousRefresh != null)
            {
                refresh.TableFragmentationStatus = previousRefresh.TableFragmentationStatus;
                refresh.ConfigurationDetails = previousRefresh.ConfigurationDetails;
            }
            queryMonitorConfiguration = monitoredServer.QueryMonitorConfiguration;
            activityMonitorConfiguration = monitoredServer.ActivityMonitorConfiguration;
            LOG = Logger.GetLogger("ScheduledRefreshProbe:" + monitoredServer.InstanceName);
            clusterCollectionSetting = monitoredServer.ClusterCollectionSetting;
            wmiConfig = monitoredServer.WmiConfig;
            sqlSafeConfig = monitoredServer.SQLsafeConfig;
            queryMonitorLogStartTemplate = string.Format("Query Monitor ({0}) - ", monitoredServer.InstanceName);
        }

        #endregion

        private static Dictionary<string, DatabaseStatistics> CloneDdStatistics(Dictionary<string, DatabaseStatistics> dbStatistics)
        {
            var clone = new Dictionary<string, DatabaseStatistics>();

            foreach (var entry in dbStatistics)
            {
                clone.Add(entry.Key, (DatabaseStatistics)entry.Value.Clone());
            }

            return clone;
        }

        #region properties

        /// <summary>
        /// Used to determine the number of time the completion callback has been called.  
        /// The completion callback should only get called one time per refresh but for 
        /// unknown reasons it gets called multiple times every now and then.  This count 
        /// will allow us to gracefully handle when this happens.
        /// </summary>
        public int CompletionCallbackCount
        {
            get { return completionCallbackCount; }
            set { completionCallbackCount = value; }
        }

        public QueryMonitorCollectionState CreateQueryMonitorCollectionState()
        {
            QueryMonitorConfiguration prevQMConfig = null;
            if (previousRefresh == null)
                prevQMConfig = PersistenceManager.Instance.GetQueryMonitorConfiguration(refresh.MonitoredServer.InstanceName);
            else
                prevQMConfig = previousRefresh.MonitoredServer.QueryMonitorConfiguration;

            qmCollectionState = new QueryMonitorCollectionState(queryMonitorConfiguration, prevQMConfig);
            return qmCollectionState;
        }

        public ActivityMonitorCollectionState CreateActivityMonitorCollectionState()
        {
            ActivityMonitorConfiguration prevActivityMonitorConfig = null;
            if (previousRefresh == null)
                prevActivityMonitorConfig = PersistenceManager.Instance.GetActivityMonitorConfiguration(refresh.MonitoredServer.InstanceName);
            else
                prevActivityMonitorConfig = previousRefresh.MonitoredServer.ActivityMonitorConfiguration;

            activityMonitorCollectionState = new ActivityMonitorCollectionState(activityMonitorConfiguration, prevActivityMonitorConfig);
            return activityMonitorCollectionState;
        }
        #endregion

        #region methods

        /// <summary>
        /// Starts the probe, virtual method called by BaseProbe.BeginProbe().
        /// If this returns cleanly, probe must call FireCompletion()
        /// </summary>
        protected override void Start()
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing scheduled refresh probe to the work queue.");
                Collection.QueueDelegate(Start);
                return;
            }

            LOG.Verbose("Starting Scheduled Refresh");

            if (workload.MonitoredServer.IsVirtualized)
            {
                // Collect VM Statistics 
                int iterations = 1;
                if (previousRefresh != null)
                {
                    TimeSpan? timeDelta = refresh.TimeStamp - previousRefresh.TimeStamp;
                    iterations = timeDelta.HasValue ? timeDelta.Value.Seconds % 20 : 1;
                }
                CollectVMInfo(refresh, iterations);
            }

            //if (workload.MonitoredServer.IsSQLsafeConnected)
            //{
            //    // Collect SQLsafe Operations
            //    SQLsafeHelper.CollectSSOperations(refresh);
            //}
           
            StartConnectionStatusCollector();
        }

        #region Virtualization

        internal void CollectVMInfo(ScheduledRefresh refresh, int iterations)
        {
            using (LOG.DebugCall("Collect VM Counters"))
            {
                MonitoredSqlServer ms = refresh.MonitoredServer;

                LOG.DebugFormat("Collecting for {0}", ms.InstanceName);
                LOG.DebugFormat("   VC Server: {0}", ms.VirtualizationConfiguration.VCAddress);
                LOG.DebugFormat("   Iterations: {0}", iterations.ToString());

                try
                {
                    Stopwatch timer = new Stopwatch();
                    timer.Start();
                    if (ms.VirtualizationConfiguration.VCServerType.Equals("HyperV"))
                    {
                        HyperVService hyperVService = new HyperVService(ms.VirtualizationConfiguration.VCAddress);
                        VMwareVirtualMachine previousVcconfig = null;
                        try
                        {
                            // hyperVService.Connect(ms.VirtualizationConfiguration.VCUser, ms.VirtualizationConfiguration.VCPassword);
                            LOG.DebugFormat("Elapsed Time to connect {0} ms", timer.ElapsedMilliseconds.ToString());

                            // if (hyperVService.ConnectState == Idera.SQLdm.Common.HyperV.ConnectState.Connected)
                            //{
                            LOG.DebugFormat("Collecting {0} iteration{1} of VM Performance Statistics for {2}", iterations, iterations > 1 ? "s" : "", refresh.ServerName);
                            refresh.Server.VMConfig = hyperVService.CollectVMInfo(ms.VirtualizationConfiguration, iterations, previousRefresh, previousVcconfig, ms.WmiConfig);
                            LOG.DebugFormat("Results collected in {0} ms", timer.ElapsedMilliseconds.ToString());
                            // }

                            //hyperVService.Disconnect();
                        }
                        catch (Exception e)
                        {
                            LOG.Error("Error collecting VM Configuration and Statistics", e);
                            if (hyperVService.ConnectState == Idera.SQLdm.Common.HyperV.ConnectState.Connected)
                            {
                                hyperVService.Disconnect();
                            }
                        }

                    }
                    else
                    {
                        ServiceUtil serviceUtil = new ServiceUtil(ms.VirtualizationConfiguration.VCAddress);
                        try
                        {
                            serviceUtil.Connect(ms.VirtualizationConfiguration.VCUser, ms.VirtualizationConfiguration.VCPassword);
                            LOG.Debug("VM Collector: Connection to {0} took {1} ms", ms.VirtualizationConfiguration.VCAddress,
                                                                                      timer.ElapsedMilliseconds.ToString());

                            if (serviceUtil.ConnectState == Idera.SQLdm.Common.VMware.ConnectState.Connected)
                            {
                                refresh.Server.VMConfig = serviceUtil.CollectVMInfo(ms.VirtualizationConfiguration.InstanceUUID, iterations);
                                LOG.Debug("VM Collector: Collection of VM Info Completed [{0} ms]", timer.ElapsedMilliseconds.ToString());
                            }

                            serviceUtil.Disconnect();
                        }
                        catch (Exception e)
                        {
                            LOG.Error("Error collecting VM Configuration and Statistics", e);
                            if ((serviceUtil.ConnectState == Idera.SQLdm.Common.VMware.ConnectState.Connected))
                            {
                                serviceUtil.Disconnect();
                            }
                        }
                    }


                    timer.Stop();
                    LOG.DebugFormat("VM Collector: completed in {0} ms", timer.ElapsedMilliseconds.ToString());
                }
                catch (Exception ex)
                {
                    LOG.Error("Unrecoverable exception collecting virtualization info for scheduled collection.  Exiting custom counter collection.", ex);

                }
            }
        }

        #endregion

        #region Connection Status

        #region Connection Status Collector Methods

        /// <summary>
        /// Starts the Connection Status collector.
        /// </summary>
        private void StartConnectionStatusCollector()
        {
            StartGenericCollector(new Collector(ConnectionStatusCollector),
                                  refresh,
                                  "StartConnectionStatusCollector",
                                  "Connection Status",
                                  refresh.Server.ProductVersion,
                                  ConnectionErrorDelagate, ConnectionStatusCallback, new object[] { });	// Provide Callback for permissions failure

        }

        /// <summary>
        /// Callback used to process the data returned from the ConnectionStatus collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ConnectionStatusCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(ConnectionStatusCallback),
                refresh,
                "ConnectionStatusCallback",
                "Connection Status",
                // Execute StartServerBasicsCollector for Cloud Providers
                // Allow permissions collector for Azure
                (cloudProviderId == Constants.AmazonRDSId ||
                 (refresh.ProductVersion != null && refresh.ProductVersion.Major < 11) || cloudProviderId == Constants.MicrosoftAzureId)
                    ? new NextCollector(StartServerBasicsCollector)
                    : cloudProviderId == Constants.MicrosoftAzureId
                        ? new NextCollector(StartMasterPermissionsCollector)
                        : new NextCollector(StartPermissionsCollector),
                sender,
                e);
        }

        /// <summary>
        /// Define the ConnectionStatus collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void ConnectionStatusCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            LOG.Verbose("Monitored server " + refresh.ServerName + " is running " + refresh.ProductVersion + " " + refresh.ProductEdition);
            SqlCommand cmd = SqlCommandBuilder.BuildConnectionStatusCommand(conn);
            sdtCollector = new SqlCollector(cmd, true);
            if (connectionStatusCallback == null)
            {
                connectionStatusCallback = new EventHandler<CollectorCompleteEventArgs>(ConnectionStatusCallback);
            }

            sdtCollector.BeginCollection(connectionStatusCallback);
        }

        /// <summary>
        /// Define the ConnectionStatus callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ConnectionStatusCallback(CollectorCompleteEventArgs e)
        {
            // Nothing to do here.  Unable to pass null to SqlBaseProbe::GenericCallback
            LOG.Verbose("In ConnectionStatusCallback.  Connected to server successfully.");
        }

        /// <summary>
        /// Override the HandleSqlException method because the purpose if this prob is to determine if we are unable to connect.  The actual
        /// query is select 1, so the only error should be unable to connect, bad password, etc.
        /// </summary>
        protected void ConnectionErrorDelagate(Snapshot snapshot, string collectorName, SqlException sqlException,
                                               SqlCollector collector)
        {
            try
            {
                VMwareVirtualMachine vmData = null;
                //Clear out any data collected so far (saving VM data if it was collected)
                if (refresh.Server.VMConfig != null)
                {
                    vmData = refresh.Server.VMConfig;
                }
                refresh = new ScheduledRefresh(refresh.MonitoredServer);

                refresh.Server.VMConfig = vmData;

                //Set the service status as unable to connect or paused
                if (sqlException.Number == 17142)
                {
                    refresh.Server.SqlServiceStatus = ServiceState.Paused;
                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Monitored server is paused", false);
                }
                else
                {
                    refresh.Server.SqlServiceStatus = ServiceState.UnableToConnect;
                    ProbeHelpers.LogAndAttachToSnapshot(refresh,
                                                        LOG,
                                                        "Monitored server cannot be contacted: " + sqlException.Message,
                                                        false);
                }
                if (collector != null)
                    collector.Dispose();
            }
            finally
            {
                FireCompletion(refresh, Result.Success);
            }
        }

        #endregion

        #region Connection Status Interpretation Methods

        //None

        #endregion

        #endregion

        #region Server Basics

        #region Server Basics Collector Methods

        /// <summary>
        /// Starts the permissions collector.
        /// </summary>
        private void StartPermissionsCollector()
        {
            StartGenericCollector(new Collector(PermissionsCollector),
                                  refresh,
                                  "StartPermissionsCollector",
                                  "Permissions",
                                  null,
                                  sqlErrorDelegate, null, new object[] { });
        }

        private void StartMasterPermissionsCollector()
        {
            StartGenericCollector(new Collector(MasterPermissionsCollector),
                refresh,
                "StartPermissionsCollector",
                "Permissions",
                null,
                sqlErrorDelegate, null, new object[] { });
        }

        /// <summary>
        /// Callback used to process the data returned from the collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void MasterPermissionsCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(MasterPermissionsCallback),
                refresh,
                "PermissionsCallback",
                "Permissions",
                new NextCollector(StartPermissionsCollector),
                sender,
                e);
        }

        private void MasterPermissionsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                //Start interpreting the Permissions batch
                InterpretMasterPermissions(rd);
            }
        }

        private void InterpretMasterPermissions(SqlDataReader dataReader)
        {
            // Read Required Permissions
            refresh.CollectionPermissions = ServerOverviewInterpreter.ReadPermissionsToEnum<CollectionPermissions>(dataReader, LOG, refresh);
        }

        private void MasterPermissionsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            refresh.Server.ProductVersion = ver;
            SqlCommand cmd = SqlCommandBuilder.BuildMasterPermissionsCommand(conn, connectionInfo, workload.MonitoredServer.CloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(MasterPermissionsCallback));
        }
        /// <summary>
        /// Create Permissions Collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void PermissionsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            refresh.Server.ProductVersion = ver;
            SqlCommand cmd = SqlCommandBuilder.BuildPermissionsCommand(conn, connectionInfo, workload.MonitoredServer.CloudProviderId, workload.MonitoredServer.ReplicationMonitoringDisabled);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(PermissionsCallback));
        }

        /// <summary>
        /// Callback used to process the data returned from the collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void PermissionsCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(PermissionsCallback),
                           refresh,
                           "PermissionsCallback",
                           "Permissions",
                           new NextCollector(StartServerBasicsCollector),
                           sender,
                           e);
        }

        /// <summary>
        /// Define the Permissions Callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void PermissionsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                //Start interpreting the Permissions batch
                InterpretPermissions(rd);
            }
        }

        private void InterpretPermissions(SqlDataReader dataReader)
        {
            // Read Required Permissions
            refresh.MinimumPermissions = ServerOverviewInterpreter.ReadPermissionsToEnum<MinimumPermissions>(dataReader, LOG, refresh);
            refresh.MetadataPermissions = ServerOverviewInterpreter.ReadPermissionsToEnum<MetadataPermissions>(dataReader, LOG, refresh);

            // First iteration captured using master collector
            if (cloudProviderId == Constants.MicrosoftAzureId)
            {
                refresh.CollectionPermissions = ServerOverviewInterpreter.ReadPermissionsToEnum<CollectionPermissions>(dataReader, LOG, refresh);

            }
            else
            {
                refresh.CollectionPermissions |= ServerOverviewInterpreter.ReadPermissionsToEnum<CollectionPermissions>(dataReader, LOG, refresh);
            }

            // Read Replication Permissions
            if (!workload.MonitoredServer.ReplicationMonitoringDisabled)
            {
                refresh.CollectionPermissions |= ServerOverviewInterpreter.ReadPermissionsToEnum<CollectionPermissions>(dataReader, LOG, refresh);
            }

            if (!refresh.MinimumPermissions.HasFlag(ProbePermissionHelpers.GetMinimumPermissionsThreshold(cloudProviderId)))
            {
                // If we do not have sufficient rights on the server, we need to fail the batch
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG,
                                                    "The user account used by the collection service does not have minimum rights required on the monitored server or the rights could not be determined\r\n",
                                                    false);
                // TODO: Validate it exits the probe
            }
        }

        /// <summary>
        /// Starts the server basics collector.
        /// </summary>
        private void StartServerBasicsCollector()
        {
            StartGenericCollector(new Collector(ServerBasicsCollector),
                                  refresh,
                                  "StartServerBasicsCollector",
                                  "Server Basics",
                                  null,
                                  sqlErrorDelegate, ServerBasicsCallback, new object[] { });	// Added Permission Failure Callback
        }

        private NextCollector GetNextCollectorAfterCloud()
        {
            NextCollector nextCollector = StartDTCStatusCollector;
            if (wmiConfig.DirectWmiEnabled)
                nextCollector = StartMachineNameCollector;//SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Missing call problem solved on 31st March 2015
                                                          //nextCollector = StartServiceStatusCollector;

            if (refresh.Server.ProductVersion.Major >= 11)
                nextCollector = StartAlwaysOnCollector;


            return nextCollector;

            //GenericCallback(new CollectorCallback(ServerBasicsCallback),
            //                refresh,
            //                "ServerBasicsCallback",
            //                "Server Basics",
            //                null,
            //                new FailureDelegate(GenericFailureDelegate),
            //                nextCollector,
            //                sender,
            //                e,
            //                false,
            //                false);
        }

        /// <summary>
        /// Callback used to process the data returned from the collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ServerBasicsCallback(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing ServerBasicsCallback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, ServerBasicsCallback, e);
                return;
            }

            // 10.5 - Injecting collectors for Azure and AWS cloud providers
            NextCollector nextCollector = null;
            if (cloudProviderId == Constants.AmazonRDSId)
            {
                nextCollector = StartAWSCloudWatchMetricCollector;
            }
            else if (cloudProviderId == Constants.MicrosoftAzureId)
            {
                nextCollector = StartCloudSQLDatabaseCollector;
            }
            else
            {
                nextCollector = StartDTCStatusCollector;
                if (wmiConfig.DirectWmiEnabled)
                    nextCollector = StartMachineNameCollector;//SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Missing call problem solved on 31st March 2015
                                                              //nextCollector = StartServiceStatusCollector;

                if (refresh.Server.ProductVersion.Major >= 11)
                    nextCollector = StartAlwaysOnCollector;
            }

            GenericCallback(new CollectorCallback(ServerBasicsCallback),
                            refresh,
                            "ServerBasicsCallback",
                            "Server Basics",
                            null,
                            new FailureDelegate(GenericFailureDelegate),
                            nextCollector,
                            sender,
                            e,
                            false,
                            false);
        }

        /// <summary>
        /// Create Server Basics Collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void ServerBasicsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            refresh.Server.ProductVersion = ver;
            SqlCommand cmd = SqlCommandBuilder.BuildServerBasicsCommand(conn, connectionInfo, wmiConfig, clusterCollectionSetting, workload.MonitoredServer.CloudProviderId);//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added flag for cloud provider id
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerBasicsCallback));
        }


        /// <summary>
        /// Define the Server Basics callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ServerBasicsCallback(CollectorCompleteEventArgs e)
        {
            if (previousRefresh != null && refresh.ServerStartupTime != previousRefresh.ServerStartupTime)
            {
                LOG.Info("Server restart detected.  Disposing of previous metrics.");
                previousRefresh = null;
            }
            refresh.Server.RunningSince = refresh.ServerStartupTime;
            refresh.Server.ProductVersion = refresh.ProductVersion;

            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                //Start interpreting the Server Basics batch
                InterpretServerBasics(rd);
            }
        }

        #endregion

        #region Server Basics Interpretation Methods

        private void InterpretServerBasics(SqlDataReader dataReader)
        {
            refresh.Server.LoginHasAdministratorRights = ReadAdministrator(dataReader);
            ///Ankit Nagpal --Sqldm10.0.0
            ///Removing sysadmin check
            //if (refresh.Server.LoginHasAdministratorRights.HasValue && refresh.Server.LoginHasAdministratorRights.Value)
            if (true)
            {
                refresh.Server.SqlServiceStatus = ServiceState.Running;

                refresh.Server.RealServerName = ReadServerName(dataReader);

                if (previousRefresh != null
                    && previousRefresh.TimeStamp.HasValue && refresh.TimeStamp.HasValue
                    && previousRefresh.TimeStamp < refresh.TimeStamp)
                    refresh.Server.TimeDelta =
                        refresh.TimeStamp.Value.Subtract(previousRefresh.TimeStamp.Value);
                ServerOverviewInterpreter.ReadVersionInformation(dataReader, refresh.Server, refresh, LOG);
                ServerOverviewInterpreter.ReadServerHostName(dataReader, refresh.Server, refresh, LOG);
                ServerOverviewInterpreter.ReadEdition(dataReader, refresh.Server, refresh, LOG);
                ServerOverviewInterpreter.ReadDefaultInstancesPresent(dataReader, refresh.Server, refresh, LOG);

                if (!wmiConfig.DirectWmiEnabled)
                {
                    // Read for clustered servers
                    if (dataReader.FieldCount == 2)
                    {
                        int serviceNameControl = -1;
                        string serviceStatusString = null;
                        do
                        {
                            if (dataReader.Read())
                            {
                                if (!dataReader.IsDBNull(0)) serviceNameControl = dataReader.GetInt32(0);
                                if (!dataReader.IsDBNull(1)) serviceStatusString = dataReader.GetString(1);

                                switch ((ServiceName)serviceNameControl)
                                {
                                    case ServiceName.Agent:
                                        refresh.Server.AgentServiceStatus =
                                            ProbeHelpers.GetServiceState(serviceStatusString);
                                        break;
                                    case ServiceName.DTC:
                                        refresh.Server.DtcServiceStatus =
                                            ProbeHelpers.GetServiceState(serviceStatusString);
                                        break;
                                    case ServiceName.FullTextSearch:
                                        refresh.Server.FullTextServiceStatus =
                                            ProbeHelpers.GetServiceState(serviceStatusString);
                                        break;
                                    case ServiceName.SqlServer:
                                        //refresh.Server.SqlServiceStatus = ProbeHelpers.GetServiceState(serviceStatusString);
                                        break;
                                    //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --populate the scheduled refresh object with the service state
                                    case ServiceName.Browser:
                                        refresh.Server.SQLBrowserServiceStatus =
                                            ProbeHelpers.GetServiceState(serviceStatusString);
                                        break;
                                    case ServiceName.ActiveDirectoryHelper:
                                        refresh.Server.SQLActiveDirectoryHelperServiceStatus =
                                            ProbeHelpers.GetServiceState(serviceStatusString);
                                        break;
                                        //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --populate the scheduled refresh object with the service state
                                }
                            }
                        } while (dataReader.NextResult());
                    }
                    else
                    {
                        //Read for non-clustered servers
                        // SQLdm 10.3 (Varun Chopra) Linux Support for Service Status
                        refresh.Server.AgentServiceStatus =
                            ProbeHelpers.ReadServiceState(dataReader, LOG, cloudProviderId,
                                cloudProviderId == Constants.LinuxId || cloudProviderId == Constants.AmazonRDSId);
                        try
                        {
                            dataReader.NextResult();
                            // SQLdm 10.3 (Varun Chopra) Linux Support for Service Status
                            refresh.Server.FullTextServiceStatus = ProbeHelpers.ReadServiceState(dataReader, LOG, cloudProviderId);
                        }
                        catch
                        {
                            refresh.Server.FullTextServiceStatus = ServiceState.NotInstalled;
                        }
                    }
                }
            }
            else
            {
                // If we do not have administrator rights on the server, we need to fail the batch
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG,
                                                    "The user account used by the collection service does not have administrator rights on the monitored server or the rights could not be determined\r\n",
                                                    false);
            }
        }

        private bool? ReadAdministrator(SqlDataReader dataReader)
        {
            try
            {
                if (dataReader.Read() && dataReader.IsDBNull(0))
                    return false;
                else
                    return dataReader.GetString(0) == "sa" ? true : false;
            }
            catch (Exception exception)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Read administrator settings failed: {0}", exception,
                                                    false);
                return null;
            }
            finally
            {
                dataReader.NextResult();
            }
        }

        private string ReadServerName(SqlDataReader dataReader)
        {
            try
            {
                if (dataReader.Read() && dataReader.IsDBNull(0))
                    return null;
                else
                    return dataReader.GetString(0);
            }
            catch (Exception exception)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Read server name failed: {0}", exception, false);
                return null;
            }
            finally
            {
                dataReader.NextResult();
            }
        }

        #endregion

        #region Server Status Collector (WMI)

        private void StartServiceStatusCollector()
        {
            if (machineName == null)
            {
                LOG.Warn("The machineName could not be collected before StartServiceStatusCollector hence using ServerHostName");
                machineName = refresh.Server.ServerHostName;
            }
            else
            {
                LOG.VerboseFormat("The Machine name collected as before StartServiceStatusCollector {0}.", machineName);
            }

            //statusProbe = new ServiceStatusProbe(refresh.Server.ServerHostName, wmiConfig);
            statusProbe = new ServiceStatusProbe(machineName, wmiConfig);//SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe to get machineName 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
            statusProbe.ServiceNames = ServiceStatusProbe.GetServiceNames(null, refresh.Server.RealServerName, refresh.ProductVersion);
            statusProbe.CollectStartTime = false;
            statusProbe.BeginProbe(OnServiceStatusComplete);
        }

        private void OnServiceStatusComplete(object sender, ProbeCompleteEventArgs args)
        {
            var servicevalues = args.Data as Dictionary<string, Service>;
            if (servicevalues != null)
            {
                foreach (var entry in servicevalues)
                {
                    var service = entry.Value;
                    var serviceName = service.ServiceType;
                    switch (serviceName)
                    {
                        case ServiceName.Agent:
                            refresh.Server.AgentServiceStatus = service.RunningState;
                            break;
                        case ServiceName.DTC:
                            refresh.Server.DtcServiceStatus = service.RunningState;
                            break;
                        case ServiceName.FullTextSearch:
                            refresh.Server.FullTextServiceStatus = service.RunningState;
                            break;
                        case ServiceName.SqlServer:
                            refresh.Server.SqlServiceStatus = ServiceState.Running;
                            break;
                        //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --populate the scheduled refresh object with the service state
                        case ServiceName.Browser:
                            refresh.Server.SQLBrowserServiceStatus = service.RunningState;
                            break;
                        case ServiceName.ActiveDirectoryHelper:
                            refresh.Server.SQLActiveDirectoryHelperServiceStatus = service.RunningState;
                            break;
                            //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --populate the scheduled refresh object with the service state
                    }
                }
            }

            if (refresh.ProductVersion.Major > 9)
            {
                refresh.Server.FullTextServiceStatus = ServiceState.NotInstalled;
            }

            StartResponseTimeCollector();
        }

        #endregion

        #endregion

        #region DTC Status

        #region DTC Status Collector Methods

        /// <summary>
        /// Starts the DTC Status collector.
        /// </summary>
        private void StartDTCStatusCollector()
        {
            StartGenericCollector(new Collector(DTCStatusCollector),
                                  refresh,
                                  "StartDTCStatusCollector",
                                  "DTC Status",
                                  refresh.Server.ProductVersion,
                                  sqlErrorDelegate, DTCStatusCallback, new object[] { });
        }

        /// <summary>
        /// Callback used to process the data returned from the collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DTCStatusCallback(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing DTCStatusCallback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, DTCStatusCallback, e);
                return;
            }

            GenericCallback(new CollectorCallback(DTCStatusCallback),
                            refresh,
                            "DTCStatusCallback",
                            "DTC Status",
                            new FailureDelegate(DTCStatusFailureDelegate),
                            new FailureDelegate(GenericFailureDelegate),
                            new NextCollector(StartResponseTimeCollector),
                            sender,
                            e,
                            true);
        }

        /// <summary>
        /// Create DTC Status Collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void DTCStatusCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            // SQLdm 10.3 (Varun Chopra) Skipping DTC Service Status for Linux
            if (cloudProviderId.HasValue && cloudProviderId == Constants.LinuxId)
            {
                new NextCollector(StartResponseTimeCollector)();
                return;
            }
            // Check for Service Control Execute Access
            var hasServiceControlExecuteAccess = refresh.CollectionPermissions != CollectionPermissions.None &&
                             refresh.CollectionPermissions.HasFlag(CollectionPermissions.EXECUTEMASTERXPSERVICECONTROL);
            SqlCommand cmd =
                SqlCommandBuilder.BuildDTCStatusCommand(conn, ver, connectionInfo, wmiConfig, clusterCollectionSetting, hasServiceControlExecuteAccess, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DTCStatusCallback));
        }

        /// <summary>
        /// DTC Status failure delegate
        /// We will receive a SQL error if DTC is not installed so we need to handle that
        /// </summary>
        /// <param name="snapshot">Snapshot to modify</param>
        protected void DTCStatusFailureDelegate(Snapshot snapshot, Exception e)
        {
            refresh.Server.DtcServiceStatus = ServiceState.NotInstalled;
        }

        /// <summary>
        /// Define the DTC Status callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DTCStatusCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                //Start interpreting the DTC Status batch
                InterpretDTCStatus(rd);
            }
        }

        #endregion

        #region DTC Status Interpretation Methods

        private void InterpretDTCStatus(SqlDataReader dataReader)
        {
            try
            {
                // SQLdm 10.3 (Varun Chopra) Linux Support for Service Status
                refresh.Server.DtcServiceStatus = ProbeHelpers.ReadServiceState(dataReader, LOG, cloudProviderId);
            }
            catch (Exception)
            {
                refresh.Server.DtcServiceStatus = ServiceState.NotInstalled;
                return;
            }
        }

        #endregion

        #endregion

        #region Response Time

        #region Response Time Collector Methods

        /// <summary>
        /// Starts the response time collector.
        /// </summary>
        private void StartResponseTimeCollector()
        {
            StartGenericCollector(new Collector(ResponseTimeCollector),
                                  refresh,
                                  "StartResponseTimeCollector",
                                  "Response Time",
                                  refresh.Server.ProductVersion,
                                  sqlErrorDelegate, null, new object[] { });
        }

        /// <summary>
        /// Callback used to process the data returned from the response time collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The Idera.SQLdm.Probes.Collectors.CollectorCompleteEventArgs instance containing the event data.</param>
        private void ResponseTimeCallback(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing ResponseTimeCallback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, ResponseTimeCallback, e);
                return;
            }

            GenericCallback(new CollectorCallback(ResponseTimeCallback),
                            refresh,
                            "ResponseTimeCallback",
                            "Response Time",
                            new NextCollector(StartServerDetailsCollector),
                            sender,
                            e);
        }

        /// <summary>
        /// Create Response Time Collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void ResponseTimeCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd = SqlCommandBuilder.BuildResponseTimeCommand(conn);
            sdtCollector = new SqlCollector(cmd, true);
            responseTimeStopWatch.Start();
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ResponseTimeCallback));
        }


        /// <summary>
        /// Define the Response Time callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ResponseTimeCallback(CollectorCompleteEventArgs e)
        {
            if (e.ElapsedMilliseconds.HasValue)
            {
                refresh.Server.ResponseTime = e.ElapsedMilliseconds.Value;
            }
        }

        #endregion

        #region Response Time Interpretation Methods

        //None 

        #endregion

        #endregion

        #region Server Details

        #region Server Details Collector Methods

        /// <summary>
        /// Starts the Server Details collector
        /// </summary>
        private void StartServerDetailsCollector()
        {
            if (cloudProviderId == CLOUD_PROVIDER_ID_AZURE)
            {
                cloudDBNames = cloudDBNames != null && cloudDBNames.Count != 0
                    ? CollectionHelper.GetDatabases(connectionInfo, LOG)
                    : new List<string>();
                numberOfDatabases = 0;
                refresh.Server.TotalLocks = 0;
                if (cloudDBNames.Count > 0)
                {
                    StartServerDetailsCollectorAzure();
                }
                else
                {
                    var nextCollector = GetNextCollectorForServerDetails();
                    nextCollector.Invoke();
                }
            }
            else
            {
                StartGenericCollector(new Collector(ServerDetailsCollector),
                                  refresh,
                                  "StartServerDetailsCollector",
                                  "Server Details",
                                  refresh.Server.ProductVersion,
                                  sqlErrorDelegate, ServerDetailsCallback, new object[] { });
            }
        }

        private void StartServerDetailsCollectorAzure()
        {
            StartGenericCollectorDatabase(new CollectorDatabase(ServerDetailsCollectorDatabase),
                refresh,
                "StartServerDetailsCollector",
                "Server Details",
                ServerDetailsCallbackDatabase, cloudDBNames[numberOfDatabases], new object[] { });

        }

        private void ServerDetailsCollectorDatabase(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver, string dbname)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildServerDetailsCommand(conn,
                    ver,
                    refresh.Server.RealServerName ?? refresh.Server.ServerName ?? refresh.Server.ServerHostName, // RealServerName null for Linux
                    previousRefresh,
                    workload, workload.MonitoredServer.CloudProviderId);//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added an extra param to identify the cloud provider
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerDetailsCallbackDatabase));
        }

        private void ServerDetailsCallbackDatabase(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing ServerDetailsCallbackDatabase to work queue.");
                QueueCallback(refresh, sender as SqlCollector, ServerDetailsCallbackDatabase, e);
                return;
            }

            using (LOG.DebugCall("ServerDetailsCallbackDatabase"))
            {
                Interlocked.Increment(ref numberOfDatabases);
                NextCollector nextCollector = numberOfDatabases < cloudDBNames.Count
                    ? StartServerDetailsCollectorAzure
                    : GetNextCollectorForServerDetails();

                GenericCallback(new CollectorCallback(ServerDetailsCallbackDatabase),
                    refresh, "ServerDetailsCallbackDatabase",
                    "Server Details Database",
                    new FailureDelegate(GenericFailureDelegate), new FailureDelegate(GenericFailureDelegate),
                    nextCollector, sender, e, true, true);

                /*GenericCallback(new CollectorCallback(ServerDetailsCallbackDatabase),
                    refresh,
                    "ServerDetailsCallbackDatabase",
                    "Server Details Database",
                    nextCollector,
                    sender,
                    e);*/
            }
        }

        private NextCollector GetNextCollectorForServerDetails()
        {
            var nextCollector = new NextCollector(StartReplicationCollector);
            if (workload.GetMetricThresholdEnabled(Metric.BombedJobs) ||
                workload.GetMetricThresholdEnabled(Metric.JobCompletion) ||
                workload.GetMetricThresholdEnabled(Metric.LongJobs) ||
                workload.GetMetricThresholdEnabled(Metric.LongJobsMinutes))
            {

                nextCollector = (workload.GetMetricThresholdEnabled(Metric.BombedJobs)) ? new NextCollector(StartBombedJobsVariablesStoreCollector) : SelectJobAlertsForNonAzure();

            }
            else
            {
                if (workload.GetMetricThresholdEnabled(Metric.OldestOpenTransMinutes))
                {
                    nextCollector = new NextCollector(StartOldestOpenCollector);
                }
            }

            return nextCollector;
        }

        private void ServerDetailsCallbackDatabase(CollectorCompleteEventArgs e)
        {
            if (e.Result == Result.Success)
            {
                using (SqlDataReader rd = e.Value as SqlDataReader)
                {
                    InterpretServerDetailsDatabase(rd);
                }
            }
        }

        private void InterpretServerDetailsDatabase(SqlDataReader dataReader)
        {
            var dbName = string.Empty;
            if (dataReader.Read())
            {
                dbName = dataReader.GetString(0);
            }
            dataReader.NextResult();

            var previousDbStatistics = (previousRefresh != null && previousRefresh.Server != null)
                ? previousRefresh.Server.DbStatistics
                : null;
            if (dbName == "master")
            {
                refresh.Server.MaxConnections =
                    ServerOverviewInterpreter.ReadMaxServerConnections(dataReader, refresh, LOG);
                refresh.Server.ProcessorsUsed =
                    ServerOverviewInterpreter.CalculateProcessorsUsed(dataReader, refresh.Server.ProcessorCount, refresh,
                        LOG);
                ServerOverviewInterpreter.ReadIsClustered(dataReader, refresh.Server, refresh, LOG,
                    clusterCollectionSetting);

                Read2005ConfigAlerts(dataReader);
                ProbeHelpers.ReadDatabaseStatus(dataReader, previousDbStatistics, refresh.Server, LOG);
                ProbeHelpers.ReadDatabaseCounters(dataReader, previousDbStatistics, refresh.Server, LOG);
                dataReader.NextResult();
                ProbeHelpers.ReadAzureIO(dataReader, refresh, LOG, refresh.Server, GenericFailureDelegate);
                dataReader.NextResult();
            }

            // Handle Addition using separate method, if required
            // For each database

            ProbeHelpers.ReadAzureStorageSizeLimit(dataReader, refresh, LOG,refresh.Server, GenericFailureDelegate);
            dataReader.NextResult();

            InterpretAzureDbDetail(dataReader);
            dataReader.NextResult();

            var prevRefreshServerStats = previousRefresh != null
                ? previousRefresh.Server.Statistics
                : null;
            ServerOverviewInterpreter.ReadThroughput(dataReader, prevRefreshServerStats, refresh.Server.Statistics, refresh, LOG,
                previousRefresh != null ? previousRefresh.TimeStamp : null);
            ServerOverviewInterpreter.ReadProcessDetails(dataReader, refresh.Server, refresh, LOG);
            //There is no reason to interpret this data if the alert is not set up
            if (workload.GetMetricThresholdEnabled(Metric.ResourceAlert))
            {
                ReadResourceCheck(dataReader);
            }
            else
            {
                dataReader.NextResult();
            }
            ReadBlockingCheck(dataReader);
            ServerOverviewInterpreter.ReadServerStatistics(dataReader, prevRefreshServerStats, refresh.Server, refresh, LOG);
            ServerOverviewInterpreter.ReadProcedureCache(dataReader, refresh.Server, refresh.MemoryStatistics, refresh, LOG);
            var TotalLocks = ServerOverviewInterpreter.ReadTotalLocks(dataReader, refresh, LOG);
            if (refresh.Server.TotalLocks == null)
                refresh.Server.TotalLocks = TotalLocks;
            else
                refresh.Server.TotalLocks += TotalLocks;
            ProbeHelpers.ReadMemoryCounters(dataReader, refresh, LOG, refresh.MemoryStatistics, refresh.ProductVersion,
                GenericFailureDelegate);
            ProbeHelpers.ReadDatabaseWaitTime(dataReader, previousDbStatistics, refresh.Server, LOG);

            if (numberOfDatabases >= cloudDBNames.Count)
            {
                ProbeHelpers.CalculateDatabaseStatistics(previousDbStatistics, refresh.Server);
            }

            dataReader.NextResult();
            InterpretWaitStats(dataReader);
            InterpretTempdbSummary(dataReader);
        }

        /// <summary>
        /// Callback used to process the data returned from the Server Details collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The Idera.SQLdm.Probes.Collectors.CollectorCompleteEventArgs instance containing the event data.</param>
        private void ServerDetailsCallback(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing ServerDetailsCallback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, ServerDetailsCallback, e);
                return;
            }

            using (LOG.DebugCall("ServerDetailsCallback"))
            {
                NextCollector nextCollector = new NextCollector(StartReplicationCollector);
                if (workload.GetMetricThresholdEnabled(Metric.BombedJobs) ||
                    workload.GetMetricThresholdEnabled(Metric.JobCompletion) ||
                    workload.GetMetricThresholdEnabled(Metric.LongJobs) ||
                    workload.GetMetricThresholdEnabled(Metric.LongJobsMinutes))
                {
                  
                    nextCollector = (workload.GetMetricThresholdEnabled(Metric.BombedJobs)) ? new NextCollector(StartBombedJobsVariablesStoreCollector) : SelectJobAlertsForNonAzure();
                   
                }
                else
                {
                    if (workload.GetMetricThresholdEnabled(Metric.OldestOpenTransMinutes))
                    {
                        nextCollector = new NextCollector(StartOldestOpenCollector);
                    }
                }

                GenericCallback(new CollectorCallback(ServerDetailsCallback),
                                refresh,
                                "ServerDetailsCallback",
                                "Server Details",
                                nextCollector,
                                sender,
                                e);
            }
        }

        /// <summary>
        /// Create Server Details Collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void ServerDetailsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {

            SqlCommand cmd =
                SqlCommandBuilder.BuildServerDetailsCommand(conn,
                                                            ver,
                                                            refresh.Server.RealServerName ?? refresh.Server.ServerName ?? refresh.Server.ServerHostName, // RealServerName null for Linux
                                                            previousRefresh,
                                                            workload, workload.MonitoredServer.CloudProviderId);//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added an extra param to identify the cloud provider
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerDetailsCallback));
        }

        /// <summary>
        /// Define the Server Details callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ServerDetailsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                var prevRefreshServerStats = previousRefresh != null
                ? previousRefresh.Server.Statistics
                : null;
                InterpretServerDetails(rd);
                if (cloudProviderId == Constants.MicrosoftAzureManagedInstanceId)
                {
                    rd.NextResult();
                    ProbeHelpers.ReadAzureManagedInstance(rd, refresh, LOG, refresh.Server, GenericFailureDelegate);
                    rd.NextResult();
                    InterpretManagedAzureMetrics("master", rd);// 5.4.1 support for master db only
                    
                }
                rd.NextResult();
                ServerOverviewInterpreter.ReadServerStatistics(rd, prevRefreshServerStats, refresh.Server, refresh, LOG);
            }
        }

        #endregion

        #region Server Details Interpretation Methods

        private void InterpretServerDetails(SqlDataReader dataReader)
        {
            // Split the InterpretForScheduledRefresh into two steps due to alerting
            ServerOverviewInterpreter.InterpretForScheduledRefreshStepOne(dataReader,
                                                                          previousRefresh != null
                                                                              ? previousRefresh.Server.Statistics
                                                                              : null, refresh.Server, refresh,
                                                                          LOG,
                                                                          previousRefresh != null ? previousRefresh.TimeStamp : null);

            //There is no reason to interpret this data if the alert is not set up
            if (workload.GetMetricThresholdEnabled(Metric.ResourceAlert))
            {
                ReadResourceCheck(dataReader);
            }
            else
            {
                dataReader.NextResult();
            }


            ReadBlockingCheck(dataReader);


            // Split the InterpretForScheduledRefresh into two steps due to alerting
            ServerOverviewInterpreter.InterpretForScheduledRefreshStepTwo(dataReader,
                                                                          previousRefresh != null
                                                                              ? previousRefresh.Server.Statistics
                                                                              : null, refresh.Server,
                                                                          refresh.MemoryStatistics, refresh,
                                                                          LOG, clusterCollectionSetting);
            if (refresh.ProductVersion.Major >= 9)
            {
                Read2005ConfigAlerts(dataReader);
            }

            ProbeHelpers.ReadMemoryCounters(dataReader, refresh, LOG, refresh.MemoryStatistics, refresh.ProductVersion,
                                            GenericFailureDelegate);

            ProbeHelpers.ReadDatabaseStatistics(dataReader, (previousRefresh != null && previousRefresh.Server != null) ? previousRefresh.Server.DbStatistics : null, refresh.Server, LOG);

            if (refresh.ProductVersion.Major >= 9)
            {
                dataReader.NextResult();
                InterpretWaitStats(dataReader);
                InterpretTempdbSummary(dataReader);

            }
          
        }
        private void InterpretManagedAzureMetrics(String dbName, SqlDataReader rd)
        {
            Dictionary<string, object> currentAzureDBMetrics = new Dictionary<string, object>();
            string[] metricnames = Enum.GetNames(typeof(CloudMetricList.AzureMetric));
            while (rd.Read())
            {
                currentAzureDBMetrics.Add(metricnames[0], Convert.ToDouble(rd.GetValue(0), CultureInfo.InvariantCulture));
                currentAzureDBMetrics.Add(metricnames[1], Convert.ToDouble(rd.GetValue(1), CultureInfo.InvariantCulture));
                currentAzureDBMetrics.Add(metricnames[2], Convert.ToDouble(rd.GetValue(2), CultureInfo.InvariantCulture));
                currentAzureDBMetrics.Add(metricnames[3], Convert.ToDouble(rd.GetValue(3), CultureInfo.InvariantCulture));
                currentAzureDBMetrics.Add(metricnames[5], Convert.ToDouble(rd.GetValue(5), CultureInfo.InvariantCulture));
                currentAzureDBMetrics.Add(metricnames[6], Convert.ToDouble(rd.GetValue(6), CultureInfo.InvariantCulture));

                //START 5.4.2
                currentAzureDBMetrics.Add(metricnames[7], Convert.ToDouble(rd.GetValue(0), CultureInfo.InvariantCulture));
                currentAzureDBMetrics.Add(metricnames[8], Convert.ToDouble(rd.GetValue(1), CultureInfo.InvariantCulture));
                currentAzureDBMetrics.Add(metricnames[9], Convert.ToDouble(rd.GetValue(2), CultureInfo.InvariantCulture));
                currentAzureDBMetrics.Add(metricnames[10], Convert.ToDouble(rd.GetValue(3), CultureInfo.InvariantCulture));
                currentAzureDBMetrics.Add(metricnames[11], Convert.ToDouble(rd.GetValue(5), CultureInfo.InvariantCulture));
                currentAzureDBMetrics.Add(metricnames[12], Convert.ToDouble(rd.GetValue(6), CultureInfo.InvariantCulture));
                //END 5.4.2
            }
            refresh.AzureCloudMetrics.Add(dbName, currentAzureDBMetrics);
        }

        
        private void InterpretAzureDbDetail(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretMemory"))
            {
                try
                {
                    ReadAzureDbDetail(dataReader);
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting Memory Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(refresh);
                }
            }
        }

        private void ReadAzureDbDetail(SqlDataReader dataReader)
        {
            try
            {
                ProbeHelpers.ReadAzureDbDetail(dataReader, refresh, LOG, refresh.Server, GenericFailureDelegate);
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting Memory Collector: {0}", e,
                                                    false);
                GenericFailureDelegate(refresh);
            }
        }
        private void Read2005ConfigAlerts(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("Read2005ConfigAlerts"))
            {
                try
                {
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            if (dataReader.FieldCount == 2)
                            {
                                if (!dataReader.IsDBNull(0))
                                {
                                    switch (dataReader.GetInt32(0))
                                    {
                                        case 1562:
                                            refresh.Alerts.ClrEnabled = dataReader.GetInt32(1) == 1 ? true : false;
                                            break;
                                        case 16384:
                                            refresh.Alerts.AgentXpEnabled = dataReader.GetInt32(1) == 1 ? true : false;
                                            break;
                                        case 16390:
                                            refresh.Alerts.CommandShellEnabled = dataReader.GetInt32(1) == 1
                                                                                     ? true
                                                                                     : false;
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG,
                                                        "Error interpreting SQL 2005 configuration alerts: {0}", e,
                                                        false);
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

        private void ReadResourceCheck(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("ReadResourceCheck"))
            {
                Session session = null;

                try
                {
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            // The fieldcount will be 1 if there are no sessions to read - this is NOT an error
                            if (dataReader.FieldCount == 22)
                            {
                                if (!dataReader.IsDBNull(0))
                                {
                                    session = new Session();

                                    session.Spid = dataReader.GetInt32(0);
                                    if (!dataReader.IsDBNull(1)) session.UserName = dataReader.GetString(1);
                                    if (!dataReader.IsDBNull(2)) session.Workstation = dataReader.GetString(2);
                                    if (!dataReader.IsDBNull(3))
                                        session.Status = Session.ConvertToSessionStatus(dataReader.GetString(3));
                                    if (!dataReader.IsDBNull(4)) session.Application = dataReader.GetString(4);
                                    if (!dataReader.IsDBNull(5)) session.Command = dataReader.GetString(5);
                                    if (!dataReader.IsDBNull(6)) session.Database = dataReader.GetString(6);
                                    if (!dataReader.IsDBNull(7))
                                        session.Cpu = TimeSpan.FromMilliseconds(dataReader.GetInt64(7));
                                    if (!dataReader.IsDBNull(8))
                                        session.Memory.Kilobytes = dataReader.GetInt32(8) * 8;
                                    if (!dataReader.IsDBNull(9)) session.PhysicalIo = dataReader.GetInt64(9);
                                    if (!dataReader.IsDBNull(10)) session.BlockedBy = dataReader.GetInt32(10);
                                    if (!dataReader.IsDBNull(11)) session.BlockingCount = dataReader.GetInt32(11);
                                    if (!dataReader.IsDBNull(12)) session.LoggedInSince = dataReader.GetDateTime(12);
                                    if (!dataReader.IsDBNull(13)) session.LastActivity = dataReader.GetDateTime(13);
                                    if (!dataReader.IsDBNull(14))
                                        session.OpenTransactions = Convert.ToInt32(dataReader.GetInt16(14));
                                    if (!dataReader.IsDBNull(15))
                                        session.WorkstationNetAddress = dataReader.GetString(15);
                                    if (!dataReader.IsDBNull(16)) session.NetLibrary = dataReader.GetString(16);
                                    if (!dataReader.IsDBNull(17))
                                        session.WaitTime = TimeSpan.FromMilliseconds(dataReader.GetInt64(17));
                                    if (!dataReader.IsDBNull(18))
                                        session.ExecutionContext = Convert.ToInt32(dataReader.GetInt16(18));
                                    if (!dataReader.IsDBNull(19))
                                        session.WaitType = dataReader.GetString(19).TrimEnd(new Char[] { ' ', '\0' });
                                    if (!dataReader.IsDBNull(20))
                                        session.WaitResource = dataReader.GetString(20).TrimEnd(new Char[] { ' ', '\0' });
                                    if (!dataReader.IsDBNull(21)) session.LastCommand = dataReader.GetString(21);

                                    refresh.Alerts.ResourceCheckSessions.Add(session);
                                }
                                else
                                {
                                    LOG.Error(
                                        "There was an error while reading the resource check list: The SPID field was null.");
                                }
                            }
                        }
                    }
                    else
                    {
                        LOG.Verbose(
                            "No fields returned");
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting resource check collector: {0}",
                                                        e,
                                                        false);
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }


        private void ReadBlockingCheck(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("ReadBlockingCheck"))
            {
                try
                {
                    while (dataReader.Read())
                    {
                        BlockingSession blockingItem = new BlockingSession();

                        if (!dataReader.IsDBNull(0)) blockingItem.Spid = dataReader.GetInt32(0);
                        if (!dataReader.IsDBNull(1)) blockingItem.Application = dataReader.GetString(1);
                        if (!dataReader.IsDBNull(2)) blockingItem.Host = dataReader.GetString(2);
                        if (!dataReader.IsDBNull(3)) blockingItem.Login = dataReader.GetString(3);
                        if (!dataReader.IsDBNull(4)) blockingItem.Databasename = dataReader.GetString(4);
                        if (!dataReader.IsDBNull(5)) blockingItem.ObjectId = dataReader.GetInt32(5);
                        if (!dataReader.IsDBNull(6)) blockingItem.Requestmode = dataReader.GetString(6);
                        if (!dataReader.IsDBNull(7))
                            blockingItem.BlockingTime =
                                TimeSpan.FromMilliseconds(Convert.ToInt64(dataReader.GetValue(7)));
                        if (!dataReader.IsDBNull(8)) blockingItem.InputBuffer = dataReader.GetString(8);
                        if (!dataReader.IsDBNull(9)) blockingItem.BlockingStartTimeUTC = dataReader.GetDateTime(9);
                        if (!dataReader.IsDBNull(10)) blockingItem.BlockingLastBatch = dataReader.GetDateTime(10);
                        if (!dataReader.IsDBNull(11)) blockingItem.WaitResource = dataReader.GetString(11);

                        // If we have a bloking session with same Spid do not add to bloking session list
                        if (!refresh.Alerts.BlockingSessions.Exists(
                            delegate (BlockingSession e) { return e.Spid == blockingItem.Spid; }))
                        {
                            refresh.Alerts.BlockingSessions.Add(blockingItem);
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting Blocking Alert Collector: {0}",
                                                        e,
                                                        false);
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }


        private void InterpretWaitStats(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretWaitStats"))
            {
                try
                {
                    refresh.WaitStats.Waits = ProbeHelpers.ReadWaitStatistics(dataReader, refresh.Server.TimeDelta);

                    if (previousRefresh != null && previousRefresh.WaitStats != null && previousRefresh.WaitStats.Waits != null && 
                        (cloudProviderId != CLOUD_PROVIDER_ID_AZURE || numberOfDatabases >= cloudDBNames.Count))
                    {
                        refresh.WaitStats.CalculateWaitsDeltas(previousRefresh.WaitStats.Waits);
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh,
                                                        LOG,
                                                        "Error interpreting Wait Stats Collector: {0}",
                                                        e, true);
                }
            }
        }

        private void InterpretTempdbSummary(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretTempdbSummary"))
            {
                try
                {

                    if (!refresh.DbStatistics.ContainsKey("tempdb"))
                    {
                        refresh.DbStatistics.Add("tempdb", new DatabaseStatistics(refresh.ServerName, "tempdb"));
                    }


                    List<TempdbFileActivity> tempdbFiles = new List<TempdbFileActivity>();
                    TempdbStatistics tempdbStats = new TempdbStatistics();
                    ProbeHelpers.ReadTempdbStatistics(dataReader, refresh, LOG,
                                                        previousRefresh != null
                                                        ? previousRefresh.Server.TempdbStatistics : null,
                                                        ref tempdbStats,
                                                        ref tempdbFiles,
                                                         previousRefresh != null
                                                         ? previousRefresh.TimeStamp : null,
                                                        refresh.TimeStamp);
                    TempdbSummaryStatistics tempdbSummaryStats = new TempdbSummaryStatistics(tempdbStats);
                    refresh.Server.TempdbStatistics = tempdbSummaryStats;

                    foreach (TempdbFileActivity tempdbFile in tempdbFiles)
                    {
                        if (string.IsNullOrWhiteSpace(tempdbFile.Filepath))
                        {
                            tempdbFile.Filepath = "Unknown";
                        }
                        if (!refresh.DbStatistics["tempdb"].Files.ContainsKey(tempdbFile.Filepath))

                        {
                            if (!refresh.DbStatistics["tempdb"].Files.ContainsKey(tempdbFile.Filepath))
                            {
                                refresh.DbStatistics["tempdb"].Files.Add(tempdbFile.Filepath, tempdbFile);
                            }
                            else
                            {
                                refresh.DbStatistics["tempdb"].Files[tempdbFile.Filepath] = tempdbFile;
                            }
                        }

                        if (tempdbFile.UserObjects != null &&
                            tempdbFile.UserObjects.Megabytes.HasValue)
                        {
                            if (tempdbSummaryStats.UserObjectsMegabytes == null)
                            {
                                tempdbSummaryStats.UserObjectsMegabytes = tempdbFile.UserObjects.Megabytes.Value;
                            }
                            else
                            {
                                tempdbSummaryStats.UserObjectsMegabytes += tempdbFile.UserObjects.Megabytes.Value;
                            }
                        }

                        if (tempdbFile.InternalObjects != null &&
                            tempdbFile.InternalObjects.Megabytes.HasValue)
                        {
                            if (tempdbSummaryStats.InternalObjectsMegabytes == null)
                            {
                                tempdbSummaryStats.InternalObjectsMegabytes = tempdbFile.InternalObjects.Megabytes.Value;
                            }
                            else
                            {
                                tempdbSummaryStats.InternalObjectsMegabytes += tempdbFile.InternalObjects.Megabytes.Value;
                            }
                        }

                        if (tempdbFile.VersionStore != null &&
                            tempdbFile.VersionStore.Megabytes.HasValue)
                        {
                            if (tempdbSummaryStats.VersionStoreMegabytes == null)
                            {
                                tempdbSummaryStats.VersionStoreMegabytes = tempdbFile.VersionStore.Megabytes.Value;
                            }
                            else
                            {
                                tempdbSummaryStats.VersionStoreMegabytes += tempdbFile.VersionStore.Megabytes.Value;
                            }
                        }

                        if (tempdbFile.MixedExtents != null &&
                            tempdbFile.MixedExtents.Megabytes.HasValue)
                        {
                            if (tempdbSummaryStats.MixedExtentsMegabytes == null)
                            {
                                tempdbSummaryStats.MixedExtentsMegabytes = tempdbFile.MixedExtents.Megabytes.Value;
                            }
                            else
                            {
                                tempdbSummaryStats.MixedExtentsMegabytes += tempdbFile.MixedExtents.Megabytes.Value;
                            }
                        }

                        if (tempdbFile.UnallocatedSpace != null &&
                            tempdbFile.UnallocatedSpace.Megabytes.HasValue)
                        {
                            if (tempdbSummaryStats.UnallocatedSpaceMegabytes == null)
                            {
                                tempdbSummaryStats.UnallocatedSpaceMegabytes = tempdbFile.UnallocatedSpace.Megabytes.Value;
                            }
                            else
                            {
                                tempdbSummaryStats.UnallocatedSpaceMegabytes += tempdbFile.UnallocatedSpace.Megabytes.Value;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh,
                                                        LOG,
                                                        "Error interpreting Tempdb Summary Collector: {0}",
                                                        e, true);
                }
            }
        }


        #endregion

        #endregion

        #region Job Alerts

        #region Job Alerts Collector Methods

        //START : SQLDM 10.3 (Manali Hukkeri): Technical debt changes
        /// <summary>
        /// Starts the Job Alerts collector.
        /// </summary>
        private void StartBombedJobsVariablesStoreCollector()
        {
            StartGenericCollector(new Collector(BombedJobsVariablesStoreCollector), refresh, "StartBombedJobsVariablesStoreCollector", "Bombed job variables store", BombedJobsVariablesStoreCallback, new object[] { });
        }

        /// <summary>
        /// Define the BombedJobsVariablesStoreCollector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void BombedJobsVariablesStoreCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                    SqlCommandBuilder.BuildBombedJobsVariables(conn,
                                                               refresh.Server.RealServerName,
                                                               ver,
                                                               previousRefresh,
                                                               workload);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollectionNonQueryExecution(new EventHandler<CollectorCompleteEventArgs>(BombedJobsVariablesStoreCallback));
        }

        /// <summary>
        /// Callback used to store bombed job variables
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void BombedJobsVariablesStoreCallback(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing BombedJobsVariablesStoreCallback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, BombedJobsVariablesStoreCallback, e);
                return;
            }

            using (LOG.DebugCall("BombedJobsVariablesStoreCallback"))
            {
               
                GenericCallback(new CollectorCallback(BombedJobsVariablesStoreCallback),
                                refresh,
                                "JobAlertsCallback",
                                "Job Alerts",
                                SelectJobAlertsForNonAzure(),
                                sender,
                                e);
            }
        }

        /// <summary>
        /// Define the BombedJobsVariablesStoreCallback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void BombedJobsVariablesStoreCallback(CollectorCompleteEventArgs e)
        {
            // Callback used to store bombed job variables required for logging purposes only
            if (e.Result == Result.Success)
            {
                LOG.Verbose("BombedJobsVariablesStoreCallback() successfully completed");
            }
            else
            {
                var exceptionMessage = "BombedJobsVariablesStoreCallback() store bombed job variables failed ";
                if (e.Exception != null)
                {
                    exceptionMessage += e.Exception.ToString();
                }
                LOG.Error(exceptionMessage);
            }
        }

        //END : SQLDM 10.3 (Manali Hukkeri): Technical debt changes

        /// <summary>
        /// Define the JobAlerts collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void JobAlertsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildJobAlertsCommand(conn, ver, refresh.Server.RealServerName, previousRefresh, workload);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(JobAlertsCallback));
        }

        /// <summary>
        /// Starts the Job Alerts collector.
        /// </summary>
        private void StartJobAlertsCollector()
        {
            AdvancedAlertConfigurationSettings config =
                   ScheduledCollectionEventProcessor.GetAdvancedConfiguration(workload.Thresholds[(int)Metric.LongJobs]);
            if (config == null)
                config = new AdvancedAlertConfigurationSettings(Metric.LongJobs, workload.Thresholds[(int)Metric.LongJobs].Data);
            StartGenericCollector(
                new Collector(JobAlertsCollector),
                refresh,
                "StartJobAlertsCollector",
                "Job Alerts",
                JobAlertsCallback,
                new object[]
                    {
                        config.AlertOnJobSteps, workload.GetMetricThresholdEnabled(Metric.JobCompletion),
                        workload.GetMetricThresholdEnabled(Metric.BombedJobs),
                        workload.GetMetricThresholdEnabled(Metric.LongJobs)
                        || workload.GetMetricThresholdEnabled(Metric.LongJobsMinutes)
                    });
        }

        /// <summary>
        /// Define the JobAlerts callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void JobAlertsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretJobAlerts(rd);
            }
        }
        //sqldm-29523 start
        private NextCollector SelectJobAlertsForNonAzure()
        {
            return cloudProviderId != CLOUD_PROVIDER_ID_AZURE
                    ? new NextCollector(StartJobAlertsCollector)
                    : SkipJobAlertsCollector();
        }
        private NextCollector SkipJobAlertsCollector()
        {
            NextCollector nextCollector = new NextCollector(StartReplicationCollector);
            if (workload.GetMetricThresholdEnabled(Metric.OldestOpenTransMinutes))
            {
                nextCollector = new NextCollector(StartOldestOpenCollector);
            }

            return nextCollector;
        }
        //sqldm-29523 end
        /// <summary>
        /// Callback used to process the data returned from the JobAlerts collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void JobAlertsCallback(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing JobAlertsCallback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, JobAlertsCallback, e);
                return;
            }

            using (LOG.DebugCall("JobAlertsCallback"))
            {
                NextCollector nextCollector = SkipJobAlertsCollector();

                GenericCallback(new CollectorCallback(JobAlertsCallback),
                                refresh,
                                "JobAlertsCallback",
                                "Job Alerts",
                            nextCollector,
                                sender,
                                e);
            }
        }


        #endregion

        #region Job Alerts Interpretation Methods

        /// <summary>
        /// Interpret JobAlerts data
        /// </summary>
        private void InterpretJobAlerts(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretJobAlerts"))
            {
                try
                {
                    // Only read these alert values if the alert is enabled
                    if (workload.GetMetricThresholdEnabled(Metric.BombedJobs))
                    {
                        ReadBombedJobs(dataReader);

                        dataReader.NextResult();
                    }

                    if (workload.GetMetricThresholdEnabled(Metric.JobCompletion))
                    {
                        ReadCompletedJobs(dataReader);

                        dataReader.NextResult();
                    }

                    if (workload.GetMetricThresholdEnabled(Metric.LongJobs) ||
                        workload.GetMetricThresholdEnabled(Metric.LongJobsMinutes))
                    {
                        ReadLongJobs(dataReader);
                    }

                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh,
                                                        LOG,
                                                        "Error interpreting Job Alerts Collector: {0}",
                                                        e,
                                                        false);
                }
            }
        }

        private void ReadBombedJobs(SqlDataReader dataReader)
        {
            try
            {

                if (dataReader.Read() && !dataReader.IsDBNull(0))
                {
                    refresh.Alerts.JobFailures.LastInstanceId = dataReader.GetInt32(0);
                    // update start value for the next refresh
                    PersistenceManager.Instance.SetFailedJobInstanceId(workload.MonitoredServer.InstanceName,
                                                                       refresh.Alerts.JobFailures.LastInstanceId);
                }

                dataReader.NextResult();

                List<Pair<Guid, int>> thisRefreshFailedJobs = new List<Pair<Guid, int>>();
                List<Pair<Guid, int>> cumulativeFailedJobs = new List<Pair<Guid, int>>();

                if (previousRefresh != null && previousRefresh.Alerts.JobFailures.LastInstanceId != 0 &&
                    previousRefresh.Alerts.JobFailures.FailedJobSteps != null)
                {
                    cumulativeFailedJobs = previousRefresh.Alerts.JobFailures.FailedJobSteps;
                }
                else
                {
                    cumulativeFailedJobs = PersistenceManager.Instance.GetFailedJobSteps(refresh.Server.RealServerName);
                }

                //Read bombed jobs
                if (dataReader.FieldCount == 14)
                {
                    while (dataReader.Read())
                    {
                        AgentJobFailure failedJob = new AgentJobFailure();
                        if (!dataReader.IsDBNull(0)) failedJob.JobName = dataReader.GetString(0);
                        if (!dataReader.IsDBNull(1)) failedJob.JobDescription = dataReader.GetString(1);
                        if (!dataReader.IsDBNull(2)) failedJob.StepName = dataReader.GetString(2);
                        if (!dataReader.IsDBNull(3)) failedJob.SqlMessageId = dataReader.GetInt32(3);
                        if (!dataReader.IsDBNull(4)) failedJob.SqlSeverity = dataReader.GetInt32(4);
                        if (!dataReader.IsDBNull(5)) failedJob.RunTime = dataReader.GetDateTime(5);
                        if (!dataReader.IsDBNull(6)) failedJob.Command = dataReader.GetString(6);
                        if (!dataReader.IsDBNull(7)) failedJob.ErrorMessage = dataReader.GetString(7);
                        if (!dataReader.IsDBNull(8)) failedJob.JobId = dataReader.GetGuid(8);
                        if (!dataReader.IsDBNull(9)) failedJob.Executions = dataReader.GetInt32(9);
                        if (!dataReader.IsDBNull(10)) failedJob.FailedRuns = dataReader.GetInt32(10);
                        if (!dataReader.IsDBNull(11)) failedJob.CollectionsSince = dataReader.GetDateTime(11);
                        if (!dataReader.IsDBNull(12)) failedJob.StepId = dataReader.GetInt32(12);
                        if (!dataReader.IsDBNull(13)) failedJob.Category = dataReader.GetString(13);

                        if (failedJob.JobId != null && failedJob.JobId != Guid.Empty)
                            thisRefreshFailedJobs.Add(new Pair<Guid, int>(failedJob.JobId, failedJob.StepId));
                        refresh.Alerts.JobFailures.JobList.Add(failedJob);
                    }

                    dataReader.NextResult();

                    while (dataReader.Read())
                    {
                        AgentJobClear clearedJob = new AgentJobClear();
                        if (!dataReader.IsDBNull(0)) clearedJob.JobId = dataReader.GetGuid(0);
                        if (!dataReader.IsDBNull(5)) clearedJob.StepID = dataReader.GetInt32(5);
                        Pair<Guid, int> successfulJob = new Pair<Guid, int>(clearedJob.JobId, clearedJob.StepID);
                        // Don't clear something we have just raised
                        if (clearedJob.JobId == null || clearedJob.JobId == Guid.Empty ||
                            thisRefreshFailedJobs.Contains(successfulJob))
                            continue;
                        else
                        {
                            if (!dataReader.IsDBNull(1)) clearedJob.JobName = dataReader.GetString(1);
                            if (!dataReader.IsDBNull(2)) clearedJob.JobDescription = dataReader.GetString(2);
                            if (!dataReader.IsDBNull(3)) clearedJob.LastRunStartTime = dataReader.GetDateTime(3);
                            if (!dataReader.IsDBNull(4))
                                clearedJob.RunDuration = ProbeHelpers.TimeSpanFromHHMMSS(dataReader.GetInt32(4));

                            refresh.Alerts.JobFailures.ClearJobList.Add(clearedJob);

                            // Clear out any jobs from prior refreshes
                            if (cumulativeFailedJobs.Contains(successfulJob))
                                cumulativeFailedJobs.Remove(successfulJob);
                        }
                    }

                    foreach (Pair<Guid, int> job in thisRefreshFailedJobs)
                    {
                        if (!cumulativeFailedJobs.Contains(job))
                            cumulativeFailedJobs.Add(job);
                    }

                    refresh.Alerts.JobFailures.FailedJobSteps = cumulativeFailedJobs;
                    PersistenceManager.Instance.SetFailedJobSteps(refresh.Server.ServerHostName,
                                                                  refresh.Alerts.JobFailures.FailedJobSteps);
                }
            }
            catch (Exception exception)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Read bombed jobs failed: {0}", exception, false);
                return;
            }
        }

        private void ReadLongJobs(SqlDataReader dataReader)
        {
            try
            {

                while (dataReader.Read())
                {
                    AgentJobRunning runningJob = new AgentJobRunning();
                    if (!dataReader.IsDBNull(0)) runningJob.JobName = dataReader.GetString(0);
                    if (!dataReader.IsDBNull(1)) runningJob.JobDescription = dataReader.GetString(1);
                    if (!dataReader.IsDBNull(2)) runningJob.StepName = dataReader.GetString(2);
                    if (!dataReader.IsDBNull(3)) runningJob.RetryAttempt = dataReader.GetInt32(3);
                    if (!dataReader.IsDBNull(4))
                        runningJob.RunningTime = TimeSpan.FromSeconds(dataReader.GetInt32(4));
                    if (!dataReader.IsDBNull(5)) runningJob.StartedAt = dataReader.GetDateTime(5);
                    if (!dataReader.IsDBNull(6)) runningJob.AverageDurationInSeconds = dataReader.GetDouble(6);
                    if (!dataReader.IsDBNull(7)) runningJob.JobId = dataReader.GetGuid(7);
                    if (!dataReader.IsDBNull(8)) runningJob.AssociatedThreshold = (Metric)dataReader.GetInt32(8);
                    if (!dataReader.IsDBNull(9)) runningJob.Category = dataReader.GetString(9);
                    if (!dataReader.IsDBNull(10)) runningJob.StepID = dataReader.GetInt32(10);
                    if (!dataReader.IsDBNull(11)) runningJob.StepRunningTime = TimeSpan.FromSeconds(dataReader.GetInt32(11));
                    if (!dataReader.IsDBNull(12)) runningJob.AverageStepDurationInSeconds = dataReader.GetDouble(12);
                    //SQLdm 10.1.3 (Vamshi Krishna) SQLDM-19816 - reading the job_last_run_duration value 
                    try
                    {
                        if (!dataReader.IsDBNull(13)) runningJob.LastRunningJobDurationInSeconds = dataReader.GetDouble(13);
                    }
                    catch (Exception exception)
                    {
                        ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Read long running jobs failed for job_last_run_duration: {0}", exception, false);
                    }

                    refresh.Alerts.LongRunningJobs.Add(runningJob);
                }
            }

            catch (Exception exception)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Read long running jobs failed: {0}", exception, false);
                return;
            }
        }

        private void ReadCompletedJobs(SqlDataReader dataReader)
        {
            try
            {

                if (dataReader.Read() && !dataReader.IsDBNull(0))
                {
                    refresh.Alerts.JobsCompleted.LastInstanceId = dataReader.GetInt32(0);
                    // update start value for the next refresh
                    PersistenceManager.Instance.SetCompletedJobInstanceId(workload.MonitoredServer.InstanceName,
                                                                       refresh.Alerts.JobsCompleted.LastInstanceId);
                }

                dataReader.NextResult();

                while (dataReader.Read())
                {

                    AgentJobCompletion completedJob = new AgentJobCompletion();

                    if (!dataReader.IsDBNull(0)) completedJob.JobId = dataReader.GetGuid(0);
                    if (!dataReader.IsDBNull(1)) completedJob.JobName = dataReader.GetString(1);
                    if (!dataReader.IsDBNull(2)) completedJob.StepId = dataReader.GetInt32(2);
                    if (!dataReader.IsDBNull(3)) completedJob.StepName = dataReader.GetString(3);
                    if (!dataReader.IsDBNull(4)) completedJob.RunStatus = AgentJobCompletion.ConvertToJobStepCompletionStatus(dataReader.GetInt32(4));
                    if (!dataReader.IsDBNull(5)) completedJob.StartTime = dataReader.GetDateTime(5);
                    if (!dataReader.IsDBNull(6)) completedJob.RunDuration = ProbeHelpers.TimeSpanFromHHMMSS(dataReader.GetInt32(6));
                    if (!dataReader.IsDBNull(7)) completedJob.SQLMessageId = dataReader.GetInt32(7);
                    if (!dataReader.IsDBNull(8)) completedJob.SQLServerity = dataReader.GetInt32(8);
                    if (!dataReader.IsDBNull(9)) completedJob.Command = dataReader.GetString(9);
                    if (!dataReader.IsDBNull(10)) completedJob.Message = dataReader.GetString(10);
                    if (!dataReader.IsDBNull(11)) completedJob.Category = dataReader.GetString(11);
                    if (!dataReader.IsDBNull(12)) completedJob.Runs = dataReader.GetInt32(12);
                    if (!dataReader.IsDBNull(13)) completedJob.Failures = dataReader.GetInt32(13);
                    if (!dataReader.IsDBNull(14)) completedJob.Successful = dataReader.GetInt32(14);
                    if (!dataReader.IsDBNull(15)) completedJob.Retries = dataReader.GetInt32(15);
                    if (!dataReader.IsDBNull(16)) completedJob.Canceled = dataReader.GetInt32(16);
                    if (!dataReader.IsDBNull(17)) completedJob.CollectionSince = dataReader.GetDateTime(17);

                    refresh.Alerts.JobsCompleted.JobList.Add(completedJob);

                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Read completed jobs failed: {0}", e, false);
                return;
            }
        }



        #endregion

        #endregion

        #region Oldest Open Transaction

        #region Oldest Open Transaction Collector Methods

        /// <summary>
        /// Starts the Oldest Open Transaction collector.
        /// </summary>
        private void StartOldestOpenCollector()
        {
            StartGenericCollector(new Collector(OldestOpenCollector),
                                  refresh,
                                  "StartOldestOpenCollector",
                                  "Oldest Open Transaction", OldestOpenCallback, new object[] { refresh.Server.ProductVersion.Major });
        }

        /// <summary>
        /// Callback used to process the data returned from the OldestOpen collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void OldestOpenCallback(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing OldestOpenCallback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, OldestOpenCallback, e);
                return;
            }

            GenericCallback(new CollectorCallback(OldestOpenCallback),
                            refresh,
                            "OldestOpenCallback",
                            "Oldest Open Transaction",
                            new FailureDelegate(OldestOpenFailureDelegate),
                            new FailureDelegate(OldestOpenFailureDelegate),
                            null,   // new NextCollector(StartReplicationCollector),
                            sender,
                            e,
                            true,
                            false);

            if (!refresh.CollectionFailed)
                StartReplicationCollector();
        }

        /// <summary>
        /// Define the OldestOpen collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void OldestOpenCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildOldestOpenCommand(conn, ver, workload);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(OldestOpenCallback));
        }


        /// <summary>
        /// Define the OldestOpen callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void OldestOpenCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretOldestOpen(rd);
            }
        }

        /// <summary>
        /// Oldest open failure delegate
        /// Do not fail the entire refresh if unavailable
        /// </summary>
        /// <param name="snapshot">Snapshot to return with partial failure</param>
        protected void OldestOpenFailureDelegate(Snapshot snapshot, Exception e)
        {
            ((ScheduledRefresh)snapshot).OldestOpenTransactionError = e;
        }

        #endregion

        #region Oldest Open Transaction Interpretation Methods

        /// <summary>
        /// Interpret OldestOpen data
        /// </summary>
        private void InterpretOldestOpen(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretOldestOpen"))
            {
                try
                {
                    ReadOldestOpen(dataReader);
                }
                catch (Exception e)
                {
                    refresh.OldestOpenTransactionError = e;
                    LOG.Warn("Error interpreting Oldest Open Transaction Collector:", e);
                }
            }
        }

        /// <summary>
        /// Read oldest open transactions data
        /// </summary>
        private void ReadOldestOpen(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("ReadOldestOpen"))
            {
                OpenTransaction oldestOpen = null;

                try
                {
                    if (dataReader.FieldCount != 24)
                    {
                        LOG.Warn("Oldest open transaction not found.");
                        //return;
                    }
                    while (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(0))
                        {
                            oldestOpen = new OpenTransaction();

                            oldestOpen.Spid = dataReader.GetInt32(0);
                            if (!dataReader.IsDBNull(1)) oldestOpen.UserName = dataReader.GetString(1);
                            if (!dataReader.IsDBNull(2)) oldestOpen.Workstation = dataReader.GetString(2);
                            if (!dataReader.IsDBNull(3))
                                oldestOpen.Status = Session.ConvertToSessionStatus(dataReader.GetString(3));
                            if (!dataReader.IsDBNull(4)) oldestOpen.Application = dataReader.GetString(4);
                            if (!dataReader.IsDBNull(5)) oldestOpen.Command = dataReader.GetString(5);
                            if (!dataReader.IsDBNull(6)) oldestOpen.Database = dataReader.GetString(6);
                            if (!dataReader.IsDBNull(7))
                                oldestOpen.Cpu = TimeSpan.FromMilliseconds(dataReader.GetInt32(7));
                            if (!dataReader.IsDBNull(8))
                                oldestOpen.Memory.Kilobytes = dataReader.GetInt32(8) * 8;
                            if (!dataReader.IsDBNull(9)) oldestOpen.PhysicalIo = dataReader.GetInt64(9);
                            if (!dataReader.IsDBNull(10)) oldestOpen.BlockedBy = dataReader.GetInt32(10);
                            if (!dataReader.IsDBNull(11)) oldestOpen.BlockingCount = dataReader.GetInt32(11);
                            if (!dataReader.IsDBNull(12)) oldestOpen.LoggedInSince = dataReader.GetDateTime(12);
                            if (!dataReader.IsDBNull(13)) oldestOpen.LastActivity = dataReader.GetDateTime(13);
                            if (!dataReader.IsDBNull(14))
                                oldestOpen.OpenTransactions = Convert.ToInt32(dataReader.GetInt16(14));
                            if (!dataReader.IsDBNull(15))
                                oldestOpen.WorkstationNetAddress = dataReader.GetString(15);
                            if (!dataReader.IsDBNull(16)) oldestOpen.NetLibrary = dataReader.GetString(16);
                            if (!dataReader.IsDBNull(17))
                                oldestOpen.WaitTime = TimeSpan.FromMilliseconds(dataReader.GetInt64(17));
                            if (!dataReader.IsDBNull(18))
                                oldestOpen.ExecutionContext = Convert.ToInt32(dataReader.GetInt16(18));
                            if (!dataReader.IsDBNull(19))
                                oldestOpen.WaitType = dataReader.GetString(19).TrimEnd(new Char[] { ' ', '\0' });
                            if (!dataReader.IsDBNull(20))
                                oldestOpen.WaitResource =
                                    dataReader.GetString(20).TrimEnd(new Char[] { ' ', '\0' });
                            if (!dataReader.IsDBNull(21)) oldestOpen.LastCommand = dataReader.GetString(21);
                            if (!dataReader.IsDBNull(22))
                                oldestOpen.RunTime = TimeSpan.FromSeconds(dataReader.GetInt32(22));
                            if (!dataReader.IsDBNull(23)) oldestOpen.StartTime = dataReader.GetDateTime(23);

                            refresh.Alerts.OldestOpenTransactions.Add(oldestOpen);
                        }
                        else
                        {
                            LOG.Warn(
                                "There was an error while reading the oldest open transaction list: The SPID field was null.");
                        }
                    }
                }
                catch (Exception e)
                {
                    refresh.OldestOpenTransactionError = e;
                    LOG.Warn("Error interpreting oldest open transaction:", e);
                }
            }
        }

        #endregion

        #endregion

        #region Replication

        #region Replication Collector Methods

        /// <summary>
        /// Starts the Replication collector.
        /// </summary>
        private void StartReplicationCollector()
        {
            if (workload.MonitoredServer.ReplicationMonitoringDisabled)
            {
                refresh.Replication.ReplicationStatus = ReplicationState.CollectionDisabled;
                if (cloudProviderId == CLOUD_PROVIDER_ID_AZURE)
                {
                    numberOfDatabasesQM = 0;
                    initializeDBNamesForQMAzure();
                    if (cloudDBNamesQMAzure != null && cloudDBNamesQMAzure.Count > 0)
                    {
                        StartQueryMonitorCollector();
                    }
                    else
                    {
                        StartConfigurationCollector();
                    }
                }
                else
                    StartQueryMonitorCollector();
            }
            else
            {
                StartGenericCollector(new Collector(ReplicationCollector),
                                      refresh,
                                      "StartReplicationCollector",
                                      "Replication", ReplicationCallback, new object[] { });
            }
        }

        /// <summary>
        /// Callback used to process the data returned from the Replication collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ReplicationCallback(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing ReplicationCallback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, ReplicationCallback, e);
                return;
            }

            GenericCallback(new CollectorCallback(ReplicationCallback),
                            refresh,
                            "ReplicationCallback",
                            "Replication",
                            new FailureDelegate(ReplicationFailureDelegate),
                            new FailureDelegate(ReplicationFailureDelegate),
                            null, // new NextCollector(StartDatabaseSizeCollector),
                            sender,
                            e,
                            true,
                            false);

            if (!refresh.CollectionFailed)
            {
                if (cloudProviderId == CLOUD_PROVIDER_ID_AZURE || cloudProviderId == Constants.MicrosoftAzureManagedInstanceId)
                {
                    numberOfDatabasesQM = 0;
                    initializeDBNamesForQMAzure();
                    if (cloudDBNamesQMAzure != null && cloudDBNamesQMAzure.Count > 0)
                    {
                        StartQueryMonitorCollector();
                    }
                    else
                    {
                        StartConfigurationCollector();
                    }
                }
                else
                    StartQueryMonitorCollector();
            }
        }

        /// <summary>
        /// Define the Replication collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void ReplicationCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildReplicationCommand(conn, ver, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ReplicationCallback));
        }

        /// <summary>
        /// Define the Replication callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ReplicationCallback(CollectorCompleteEventArgs e)
        {
            if (e.Exception != null)
            {
                refresh.ReplicationError = e.Exception;
            }
            else
            {
                try
                {
                    using (SqlDataReader rd = e.Value as SqlDataReader)
                    {
                        InterpretReplication(rd);
                    }
                }
                catch (Exception ex)
                {
                    ReplicationFailureDelegate(refresh, ex);
                }
            }

        }

        /// <summary>
        /// Replication failure delegate
        /// Do not fail the entire refresh if unavailable
        /// </summary>
        /// <param name="snapshot">Snapshot to return with partial failure</param>
        protected void ReplicationFailureDelegate(Snapshot snapshot, Exception e)
        {
            ((ScheduledRefresh)snapshot).ReplicationError = e;
        }

        #endregion

        #region Replication Interpretation Methods

        /// <summary>
        /// Interpret Replication data
        /// </summary>
        private void InterpretReplication(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretReplication"))
            {
                try
                {
                    ReadReplication(dataReader);
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting Replication Collector: {0}", e,
                                                        true);
                }
            }
        }

        /// <summary>
        /// Extracts replication data from the dataReader.
        /// The dataReader will contain all replication data for every replication role this server is performing.
        /// A subscription has one publication
        /// A subscriber can subscribe to publications on any number of distributors.
        /// A distributor can be used utilised by any number of publishers
        /// A publication has one distributor and one subscriber so this function populates an object heirachy accordingly.
        /// A publisher has only one distributor.
        /// </summary>
        /// <param name="dataReader"></param>
        private void ReadReplication(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("ReadReplication"))
            {
                try
                {
                    publishedDB published;
                    cDistributor distributor;
                    subscribedDB replSubscriber;

                    string publishedDatabase = "";
                    string subscribedDatabase = "";

                    string distributorDatabase = "";
                    string distributorInstance = "";

                    string director = null;

                    int dictionaryKey;
                    string prevPublication = "";
                    string prevSubscriberDB = "";
                    string prevSubscriberInstance = "";

                    long? totalSubscribed = null;
                    long? totalUnsubscribed = null;

                    long publishedArticles = 0;
                    string publisherInstance = "";
                    string publisherdb = "";
                    string publicationName = "";

                    while (director != "end replication details")
                    {
                        while (dataReader.Read())
                        {
                            if (!dataReader.IsDBNull(0)) director = dataReader.GetString(0).Trim();

                            switch (director)
                            {
                                case "published_database": //queries from the publisher

                                    if (!dataReader.IsDBNull(1)) publishedDatabase = dataReader.GetString(1);

                                    if (refresh.Replication.Publisher == null) refresh.Replication.Publisher = new cPublisher();

                                    refresh.Replication.Publisher.InstanceName = refresh.ServerName;

                                    // each publisher can have many subscribers - we don't get all the subscriber data here 
                                    // but we get what we can and suppliment it later

                                    //string prevSubscription = "";
                                    prevPublication = "";

                                    if (dataReader.NextResult())
                                    {
                                        while (dataReader.Read())
                                        {
                                            if (dataReader.FieldCount == 9)
                                            {
                                                //This data is returned from syssubscriptions on the publisher
                                                //There may be multiple distinct subscribers
                                                if (!dataReader.IsDBNull(1)) publisherInstance = dataReader.GetString(1);
                                                if (!dataReader.IsDBNull(2)) publisherdb = dataReader.GetString(2);
                                                if (!dataReader.IsDBNull(3)) publicationName = dataReader.GetString(3);

                                                dictionaryKey = (publisherInstance + publisherdb + publicationName).ToLower().GetHashCode();

                                                //Does this publisher already exist. If so then use existing ref
                                                if (refresh.Replication.Publisher.PublishedDatabases.ContainsKey(dictionaryKey))
                                                {
                                                    published = refresh.Replication.Publisher.PublishedDatabases[dictionaryKey];
                                                }
                                                else
                                                {
                                                    published = new publishedDB();
                                                    refresh.Replication.Publisher.PublishedDatabases.Add(dictionaryKey, published);
                                                }

                                                published.Instance = publisherInstance;
                                                published.DBName = publisherdb;
                                                published.PublicationName = publicationName;

                                                replSubscriber = new subscribedDB();
                                                replSubscriber.Instance = dataReader.GetString(4);
                                                replSubscriber.Dbname = dataReader.GetString(5);

                                                //These properties, although coming from the publisher are referring to the subscriber
                                                if (!dataReader.IsDBNull(0)) replSubscriber.Articles = dataReader.GetInt32(0);
                                                if (!dataReader.IsDBNull(6)) replSubscriber.SubscriptionStatus = dataReader.GetByte(6);
                                                if (!dataReader.IsDBNull(7)) replSubscriber.SyncType = dataReader.GetByte(7);
                                                if (!dataReader.IsDBNull(8)) replSubscriber.SubscriptionType = dataReader.GetInt32(8);

                                                published.Subscriptions.Add(replSubscriber);
                                            }
                                        }
                                    }
                                    break;
                                case "replcounters": //contains publisher specific counters
                                    publishedDatabase = "";
                                    if (dataReader.NextResult())
                                    {
                                        while (dataReader.Read())
                                        {
                                            if (dataReader.FieldCount == 6)
                                            {
                                                publishedDatabase = dataReader.GetString(0);

                                                //sum the replicated - non-distributed transactions
                                                if (!dataReader.IsDBNull(1)) refresh.Replication.Publisher.ReplicatedTrans += dataReader.GetInt32(1);
                                                if (!dataReader.IsDBNull(2)) refresh.Replication.Publisher.ReplicatedTranRate = dataReader.GetFloat(2);

                                                if (!dataReader.IsDBNull(3))
                                                {
                                                    refresh.Replication.Publisher.ReplicationLatency
                                                    = dataReader.GetFloat(3) > refresh.Replication.Publisher.ReplicationLatency ?
                                                    dataReader.GetFloat(3) : refresh.Replication.Publisher.ReplicationLatency;
                                                }

                                                ///Alert
                                                if (refresh.Replication.NonDistributedTransactions.HasValue)
                                                {
                                                    if (!dataReader.IsDBNull(1))
                                                        refresh.Replication.NonDistributedTransactions += dataReader.GetInt32(1);
                                                }
                                                else
                                                {
                                                    if (!dataReader.IsDBNull(1))
                                                        refresh.Replication.NonDistributedTransactions = dataReader.GetInt32(1);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                    break;
                                case "sp_helpdistributor": //can only be run on the publisher returns 11 fields on 2000 and 12 on 2005
                                    //The publisher needs to have a pointer to the distributor
                                    if (dataReader.NextResult())
                                    {
                                        while (dataReader.Read())
                                        {
                                            int intFieldCount = 12;
                                            if (refresh.Server.ProductVersion.Major < 9) intFieldCount = 11;

                                            if (dataReader.FieldCount == intFieldCount)
                                            {
                                                if (!dataReader.IsDBNull(1)) distributorDatabase = dataReader.GetString(1);

                                                distributor = new cDistributor();

                                                if (refresh.Replication.Publisher == null) refresh.Replication.Publisher = new cPublisher();

                                                if (!dataReader.IsDBNull(0)) distributor.Instance = dataReader.GetString(0).ToUpper();
                                                if (!dataReader.IsDBNull(1)) distributor.Dbname = dataReader.GetString(1);
                                                if (!dataReader.IsDBNull(2)) distributor.SnapshotDirectory = dataReader.GetString(2);
                                                if (!dataReader.IsDBNull(3)) distributor.Account = dataReader.GetString(3);
                                                if (!dataReader.IsDBNull(4)) distributor.MinDistributionRetention = dataReader.GetInt32(4);
                                                if (!dataReader.IsDBNull(5)) distributor.MaxDistributionRetention = dataReader.GetInt32(5);
                                                if (!dataReader.IsDBNull(6)) distributor.HistoryRetention = dataReader.GetInt32(6);

                                                refresh.Replication.Publisher.Distributor = distributor;
                                            }
                                        }
                                    }
                                    break;
                                case "subscribed_database":
                                    if (!dataReader.IsDBNull(1)) subscribedDatabase = dataReader.GetString(1);

                                    if (dataReader.NextResult())
                                    {
                                        while (dataReader.Read())
                                        {
                                            if (!dataReader.IsDBNull(1)) publishedDatabase = dataReader.GetString(1);

                                            replSubscriber = new subscribedDB();

                                            if (dataReader.FieldCount == 11)
                                            {
                                                if (!dataReader.IsDBNull(0)) replSubscriber.PublisherInstance = dataReader.GetString(0).ToUpper();
                                                if (!dataReader.IsDBNull(1)) replSubscriber.Publisherdb = dataReader.GetString(1);
                                                if (!dataReader.IsDBNull(2)) replSubscriber.PublicationName = dataReader.GetString(2);
                                                if (!dataReader.IsDBNull(3)) replSubscriber.ReplicationType = dataReader.GetInt32(3);
                                                if (!dataReader.IsDBNull(4)) replSubscriber.SubscriptionType = dataReader.GetInt32(4);
                                                if (!dataReader.IsDBNull(5)) replSubscriber.LastUpdated = dataReader.GetDateTime(5);
                                                if (!dataReader.IsDBNull(6)) replSubscriber.Dbname = dataReader.GetString(6);

                                                replSubscriber.Instance = refresh.ServerName.ToUpper();
                                                //if (!dataReader.IsDBNull(7)) refresh.Replication.SubscribedDatabases[publishedDatabase].UpdateMode = dataReader.GetString(7);
                                                if (!dataReader.IsDBNull(8)) replSubscriber.LastSyncStatus = dataReader.GetInt32(8);
                                                if (!dataReader.IsDBNull(9)) replSubscriber.LastSyncSummary = dataReader.GetString(9);
                                                if (!dataReader.IsDBNull(10)) replSubscriber.LastSyncTime = dataReader.GetDateTime(10);

                                                dictionaryKey = (replSubscriber.PublisherInstance + publishedDatabase + replSubscriber.Instance + subscribedDatabase + replSubscriber.PublicationName).ToLower().GetHashCode();

                                                if (!refresh.Replication.SubscribedDatabases.ContainsKey(dictionaryKey))
                                                {
                                                    refresh.Replication.SubscribedDatabases.Add(dictionaryKey, replSubscriber);
                                                }
                                                else
                                                {
                                                    refresh.Replication.SubscribedDatabases[dictionaryKey] = replSubscriber;
                                                }
                                            }
                                        }
                                    }
                                    break;
                                case "distribution_database":
                                    if (!dataReader.IsDBNull(1)) distributorDatabase = dataReader.GetString(1);

                                    string distributorKey = (refresh.ServerName.ToUpper() + "." + distributorDatabase);

                                    if (dataReader.NextResult())
                                    {
                                        while (dataReader.Read())
                                        {
                                            if (dataReader.FieldCount == 10)
                                            {
                                                if (!dataReader.IsDBNull(2)) publishedDatabase = dataReader.GetString(2);
                                                distributorInstance = refresh.ServerName.ToUpper();

                                                //if the list of distributors does not contain this distributor add it
                                                if (!refresh.Replication.Distributors.ContainsKey(distributorKey))
                                                {
                                                    refresh.Replication.Distributors.Add(distributorKey, new cDistributor());
                                                }

                                                if (!dataReader.IsDBNull(1))
                                                    refresh.Replication.Distributors[distributorKey].Dbname = distributorDatabase;

                                                refresh.Replication.Distributors[distributorKey].Instance = distributorInstance;

                                                if (!dataReader.IsDBNull(0)) publishedArticles = dataReader.GetInt32(0);
                                                if (!dataReader.IsDBNull(1)) publisherInstance = dataReader.GetString(1);
                                                if (!dataReader.IsDBNull(2)) publisherdb = dataReader.GetString(2);
                                                if (!dataReader.IsDBNull(5)) publicationName = dataReader.GetString(5);

                                                dictionaryKey = (publisherInstance + publisherdb + publicationName).ToLower().GetHashCode();

                                                //Does this publisher already exist. If so then use existing ref
                                                if (refresh.Replication.Distributors[distributorKey].DistributedPublications.ContainsKey(dictionaryKey))
                                                {
                                                    published = refresh.Replication.Distributors[distributorKey].DistributedPublications[dictionaryKey];
                                                }
                                                else
                                                {
                                                    published = new publishedDB();
                                                    refresh.Replication.Distributors[distributorKey].DistributedPublications.Add(dictionaryKey, published);
                                                }

                                                published.PublishedArticles = publishedArticles;
                                                published.Instance = publisherInstance;
                                                published.DBName = publisherdb;
                                                published.PublicationName = publicationName;

                                                string thisSubscriberDb = "";
                                                string thisSubscriberInstance = "";

                                                subscribedDB thisSubscriberdb = new subscribedDB();

                                                if (!dataReader.IsDBNull(0)) thisSubscriberdb.Articles = dataReader.GetInt32(0);
                                                if (!dataReader.IsDBNull(3)) thisSubscriberdb.Instance = dataReader.GetString(3);
                                                if (!dataReader.IsDBNull(4)) thisSubscriberdb.Dbname = dataReader.GetString(4);

                                                //dont add a subscriber if empty instance and db were returned
                                                if (!string.IsNullOrEmpty(thisSubscriberdb.Instance)
                                                    && !string.IsNullOrEmpty(thisSubscriberdb.Dbname))
                                                {
                                                    published.Subscriptions.Add(thisSubscriberdb);
                                                    thisSubscriberDb = thisSubscriberdb.Dbname;
                                                    thisSubscriberInstance = thisSubscriberdb.Instance;
                                                }

                                                if (!dataReader.IsDBNull(6)) published.PublicationDescription = dataReader.GetString(6);
                                                if (!dataReader.IsDBNull(7)) published.SubscribedTrans = dataReader.GetInt64(7);
                                                if (!dataReader.IsDBNull(8)) published.NonSubscribedTrans = dataReader.GetInt64(8);
                                                if (!dataReader.IsDBNull(9)) published.ReplicationType = (ReplicationType)dataReader.GetInt32(9);

                                                //This total is per publication so only add when the publication name changes
                                                if (published.PublicationName != prevPublication
                                                    || prevSubscriberDB != thisSubscriberDb
                                                    || prevSubscriberInstance != thisSubscriberInstance)
                                                {
                                                    if (published.SubscribedTrans.HasValue)
                                                    {
                                                        totalSubscribed = (totalSubscribed.HasValue ? totalSubscribed.Value : 0) + published.SubscribedTrans.Value;
                                                    }
                                                    if (published.NonSubscribedTrans.HasValue)
                                                    {
                                                        totalUnsubscribed = (totalUnsubscribed.HasValue ? totalUnsubscribed.Value : 0) + published.NonSubscribedTrans.Value;
                                                    }
                                                    prevPublication = published.PublicationName;
                                                    prevSubscriberDB = thisSubscriberDb;
                                                    prevSubscriberInstance = thisSubscriberInstance;
                                                }
                                            }
                                        }

                                        refresh.Replication.NonSubscribedTransactions = totalUnsubscribed;
                                        refresh.Replication.SubscribedDeliveredTransactions = totalSubscribed;
                                    }

                                    break;
                                case "replicationstate":
                                    if (!dataReader.IsDBNull(1))
                                        refresh.Replication.ReplicationStatus =
                                            (ReplicationState)dataReader.GetInt32(1);
                                    break;
                                case "subscription latency":
                                    if (!dataReader.IsDBNull(1))
                                        if (refresh.Replication.SubscriptionLatency.TotalSeconds > 0)
                                        {
                                            TimeSpan tempTimeSpan = TimeSpan.FromSeconds(dataReader.GetInt32(1));
                                            if (tempTimeSpan.TotalSeconds >
                                                refresh.Replication.SubscriptionLatency.TotalSeconds)
                                                refresh.Replication.SubscriptionLatency = tempTimeSpan;
                                        }
                                        else
                                        {
                                            refresh.Replication.SubscriptionLatency =
                                                TimeSpan.FromSeconds(dataReader.GetInt32(1));
                                        }
                                    break;
                                case "non-subscribed transactions":
                                    if (refresh.Replication.SubscribedDeliveredTransactions.HasValue)
                                    {
                                        if (!dataReader.IsDBNull(1))
                                            refresh.Replication.SubscribedDeliveredTransactions +=
                                                dataReader.GetInt64(1);
                                    }
                                    else
                                    {
                                        if (!dataReader.IsDBNull(1))
                                            refresh.Replication.SubscribedDeliveredTransactions = dataReader.GetInt64(1);
                                    }
                                    if (refresh.Replication.NonSubscribedTransactions.HasValue)
                                    {
                                        if (!dataReader.IsDBNull(2))
                                            refresh.Replication.NonSubscribedTransactions += dataReader.GetInt64(2);
                                    }
                                    else
                                    {
                                        if (!dataReader.IsDBNull(2))
                                            refresh.Replication.NonSubscribedTransactions = dataReader.GetInt64(2);
                                    }
                                    break;
                            }
                        }
                        if (!dataReader.NextResult())
                        {
                            break;
                        }
                    }
                }
                // Do not fail collection on refrehs
                catch (Exception e)
                {
                    refresh.ReplicationError = e;
                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting replication collector: {0}", e,
                                                        true);
                }
            }
        }

        #endregion

        #endregion

        #region Mirrored Databases
        #region Update Mirror Metrics Collector Methods

        /// <summary>
        /// Starts the Mirror metrics collector.
        /// </summary>
        private void StartMirrorMetricsUpdateCollector()
        {
            StartGenericCollector(new Collector(MirrorMetricsUpdateCollector), refresh, "StartMirrorMetricsUpdateCollector", "Mirror Metrics Update", MirrorMetricsUpdateCallback, new object[] { });
        }
        /// <summary>
        /// Callback used to process the data returned from the MirrorMetricsUpdate collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void MirrorMetricsUpdateCallback(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing MirrorMetricsCallback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, MirrorMetricsUpdateCallback, e);
                return;
            }
            GenericCallback(new CollectorCallback(MirrorMetricsUpdateCallback),
                            refresh,
                            "MirrorMetricsUpdateCallback",
                            "Mirror Metrics Update",
                            new FailureDelegate(MirrorMetricsUpdateFailureDelegate),
                            new FailureDelegate(MirrorMetricsUpdateFailureDelegate),
                            null, // new NextCollector(StartMirrorDetailsCollector),
                            sender,
                            e,
                            true,
                            false);

            if (!refresh.CollectionFailed)
                StartMirrorDetailsCollector();
        }

        /// <summary>
        /// Define the MirrorMetricsUpdate collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void MirrorMetricsUpdateCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildMirrorMetricsUpdateCommand(conn, ver);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(MirrorMetricsUpdateCallback));
        }

        /// <summary>
        /// Define the MirrorMetricsUpdate callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private static void MirrorMetricsUpdateCallback(CollectorCompleteEventArgs e)
        {
        }
        /// <summary>
        /// Mirror Metrics Update failure delegate
        /// Do not fail the entire refresh if unavailable
        /// </summary>
        /// <param name="snapshot">Snapshot to return with partial failure</param>
        protected void MirrorMetricsUpdateFailureDelegate(Snapshot snapshot, Exception e)
        {
            ((ScheduledRefresh)snapshot).MirroringError = e;
            LOG.Error("Principal and mirror logs are out of sync. Oldest unsent cannot be calculated.", e);
        }

        #endregion
        #region Mirrored Databases Collector Methods
        /// <summary>
        /// Starts the Mirror Monitoring Details collector.
        /// </summary>
        private void StartMirrorDetailsCollector()
        {
            StartGenericCollector(new Collector(MirrorDetailsCollector), refresh, "StartMirrorDetailsCollector", "Mirror Monitoring Real-time", MirrorDetailsCallback, new object[] { });
        }
        /// <summary>
        /// Callback used to process the data returned from the MirrorDetails collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void MirrorDetailsCallback(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing MirrorDetailsCallback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, MirrorDetailsCallback, e);
                return;
            }

            GenericCallback(new CollectorCallback(MirrorDetailsCallback),
                refresh,
                "MirrorDetailsCallback",
                "Mirror Monitor",
                new NextCollector(StartOSMetricsCollector),
                sender,
                e);
        }

        /// <summary>
        /// Define the MirrorDetails collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void MirrorDetailsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildMirrorDetailsCommand(conn, ver);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(MirrorDetailsCallback));
        }

        /// <summary>
        /// Define the MirrorDetails callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void MirrorDetailsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretMirrorDetails(rd);
            }
        }
        #endregion
        #region Database Mirror Interpretation
        /// <summary>
        /// Interpret MirrorDetails data
        /// </summary>
        private void InterpretMirrorDetails(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretMirrorDetails"))
            {
                try
                {
                    string dbName = null;
                    string instance = null;
                    bool resultOne = false;
                    bool resultTwo = false;

                    MirrorMonitoringDatabaseDetail detail = null;

                    do
                    {
                        Guid mirroringGuid = Guid.Empty;
                        resultOne = resultTwo = false;

                        //The detail RowNotInTableException consists of 2 resultsets. without either resultset the record is no good.

                        //Goes to next row in the current resultset. We get just a single row at a time.
                        if (dataReader.Read())
                        {
                            if (dataReader.GetString(0).Equals("endMirrorMonitoring")) break;

                            resultOne = true;

                            if (dataReader.FieldCount == 10)
                            {
                                if (!dataReader.IsDBNull(0)) dbName = dataReader.GetString(0).TrimEnd();
                                if (!dataReader.IsDBNull(1)) instance = dataReader.GetString(1).TrimEnd();

                                mirroringGuid = new Guid(dataReader.GetSqlGuid(5).ToByteArray());

                                detail = new MirrorMonitoringDatabaseDetail(instance, dbName);

                                if (!dataReader.IsDBNull(2)) detail.Partner = dataReader.GetString(2);
                                if (!dataReader.IsDBNull(3)) detail.WitnessAddress = dataReader.GetString(3);
                                if (!dataReader.IsDBNull(4)) detail.PartnerAddress = dataReader.GetString(4);
                                if (!dataReader.IsDBNull(5)) detail.MirroringGuid = mirroringGuid;
                                if (!dataReader.IsDBNull(6)) detail.SafetyLevel = (MirrorMonitoringDatabaseDetail.SafetyLevelEnum)dataReader.GetInt32(6);

                                if (!dataReader.IsDBNull(7)) detail.CurrentMirroringMetrics.Role = (MirroringMetrics.MirroringRoleEnum)dataReader.GetInt32(7);
                                if (!dataReader.IsDBNull(8)) detail.CurrentMirroringMetrics.MirroringState = (MirroringMetrics.MirroringStateEnum)dataReader.GetInt32(8);
                                if (!dataReader.IsDBNull(9)) detail.CurrentMirroringMetrics.WitnessStatus = (MirroringMetrics.WitnessStatusEnum)dataReader.GetInt32(9);
                            }
                        }
                        //if there is a next resultset, go into it.
                        if (dataReader.NextResult())
                        {
                            //read the next row of the resultset
                            if (!dataReader.Read()) break;

                            if (dataReader.GetString(0).Equals("endMirrorMonitoring")) break;

                            resultTwo = true;

                            if (detail != null)
                            {
                                if (dataReader.FieldCount == 15)
                                {
                                    //if (!dataReader.IsDBNull(1)) detail.CurrentMirroringMetrics.Role = (MirroringMetrics.MirroringRoleEnum)dataReader.GetByte(1);
                                    //if (!dataReader.IsDBNull(2)) detail.CurrentMirroringMetrics.MirroringState = (MirroringMetrics.MirroringStateEnum)dataReader.GetByte(2);
                                    //if (!dataReader.IsDBNull(3)) detail.CurrentMirroringMetrics.WitnessStatus = (MirroringMetrics.WitnessStatusEnum)dataReader.GetByte(3);
                                    if (!dataReader.IsDBNull(4)) detail.CurrentMirroringMetrics.LogGenerationRate = dataReader.GetInt32(4);
                                    if (!dataReader.IsDBNull(5)) detail.CurrentMirroringMetrics.UnsentLog = dataReader.GetInt32(5);
                                    if (!dataReader.IsDBNull(6)) detail.CurrentMirroringMetrics.SendRate = dataReader.GetInt32(6);
                                    if (!dataReader.IsDBNull(7)) detail.CurrentMirroringMetrics.UnrestoredLog = dataReader.GetInt32(7);
                                    if (!dataReader.IsDBNull(8)) detail.CurrentMirroringMetrics.RecoveryRate = dataReader.GetInt32(8);
                                    if (!dataReader.IsDBNull(9)) detail.CurrentMirroringMetrics.TransactionDelay = dataReader.GetInt32(9);
                                    if (!dataReader.IsDBNull(10)) detail.CurrentMirroringMetrics.TransactionsPerSec = dataReader.GetInt32(10);
                                    if (!dataReader.IsDBNull(11)) detail.CurrentMirroringMetrics.AverageDelay = dataReader.GetInt32(11);
                                    if (!dataReader.IsDBNull(12)) detail.CurrentMirroringMetrics.TimeRecorded = dataReader.GetDateTime(12);
                                    if (!dataReader.IsDBNull(13)) detail.CurrentMirroringMetrics.TimeBehind = dataReader.GetDateTime(13);
                                    if (!dataReader.IsDBNull(14)) detail.CurrentMirroringMetrics.LocalTime = dataReader.GetDateTime(14);
                                }
                            }
                        }
                        else break;

                        if (!mirroringGuid.Equals(Guid.Empty) && resultOne && resultTwo)
                        {
                            refresh.MirroredDatabases.Add(mirroringGuid, detail);
                        }
                        else break;
                    } while (dataReader.NextResult());

                    //get the preferred mirroring configurations that have been saved for this server
                    IDictionary<Guid, ServerPreferredMirrorConfig> preferredConfigs = workload.GetPreferredConfig;
                    if (preferredConfigs != null)
                    {
                        foreach (KeyValuePair<Guid, ServerPreferredMirrorConfig> configKeyValue in preferredConfigs)
                        {
                            //if the preferred config has a counterpart in the monitored mirred servers continue
                            if (refresh.MirroredDatabases.ContainsKey(configKeyValue.Key)) continue;
                            //if no longer monitored then delete from the dictionary
                            refresh.noLongerMirroredDatabases.Add(configKeyValue.Key, new MirrorMonitoringDatabaseDetail(workload.MonitoredServer.InstanceName, configKeyValue.Value.MirroredDatabase));
                        }
                    }
                }
                catch (Exception e)
                {
                    LOG.ErrorFormat("{0} Error interpreting Mirror Details Collector:", (refresh.MonitoredServer.InstanceName != null ? refresh.MonitoredServer.InstanceName + " - " : ""), e);
                }
            }
        }
        #endregion
        #endregion

        #region AlwaysOn

        #region AlwaysOn Collector Methods

        private void StartAlwaysOnCollector()
        {
            StartGenericCollector(new Collector(AlwaysOnMetricsCollector),
                                  refresh,
                                  "StartAlwaysOnCollector",
                                  "AlwaysOn Metrics",
                                  refresh.Server.ProductVersion,
                                  sqlErrorDelegate, AlwaysOnMetricsCallback, new object[] { });
        }

        /// <summary>
        /// Create AlwaysOn Metrics Collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void AlwaysOnMetricsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd = new SqlCommand();

            if (ver.Major >= 11)
            {
                cmd = SqlCommandBuilder.BuildAlwaysOnMetricsCommand(conn, refresh.Server.ProductVersion);
                sdtCollector = new SqlCollector(cmd, true);
                sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(AlwaysOnMetricsCallback));

            }
        }

        /// <summary>
        /// Callback used to process the data returned from the Always On Metrics collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The Idera.SQLdm.Probes.Collectors.CollectorCompleteEventArgs instance containing the event data.</param>
        private void AlwaysOnMetricsCallback(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing AlwaysOnMetricsCallback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, AlwaysOnMetricsCallback, e);
                return;
            }

            NextCollector nextCollector = StartDTCStatusCollector;
            if (wmiConfig.DirectWmiEnabled)
                nextCollector = StartMachineNameCollector; //START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe before ServiceStatusProbe
                                                           //nextCollector = StartServiceStatusCollector;

            //If Always On Metrics fails we do not want to fail the entire snapshot, so return partial
            GenericCallback(new CollectorCallback(AlwaysOnMetricsCallback),
                            refresh,
                            "AlwaysOnMetricsCallback",
                            "AlwaysOn Metrics",
                            new FailureDelegate(AlwaysOnMetricsFailureDelegate),
                            new FailureDelegate(AlwaysOnMetricsFailureDelegate),
                            nextCollector,
                            sender,
                            e,
                            true);
        }

        //START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
        /// <summary>
        /// Starts the Machine Name collector.
        /// before the WMI probe for disk statistics,Service Status and OS Metric which need the machine name 
        /// </summary>
        private void StartMachineNameCollector()
        {
            MachineNameProbe machineProbe = new MachineNameProbe(connectionInfo, cloudProviderId);
            machineProbe.BeginProbe(MachineNameCallback);
        }

        /// <summary>
        /// Machine Name Collector Call back
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MachineNameCallback(object sender, SnapshotCompleteEventArgs e)
        {
            // This means we cancelled out the probe
            if (e.Result == Result.Failure)
            {
                LOG.Error("Machine name probe has failed");
            }
            else if (e.Snapshot != null)
            {
                LOG.Verbose("MachineNameCallback-- MachineNameSnapshot found.");
                var _machineSnapshot = e.Snapshot as MachineNameSnapshot;
                if (_machineSnapshot != null)
                {
                    // SQLdm 10.2 (Varun Chopra) - Defect SQLDM-26709 - Add Hostname type for alerting (SMTP, SNMP)
                    machineName = refresh.ServerHostName = _machineSnapshot.MachineName;
                    LOG.VerboseFormat("MachineNameCallback-- MachineName found as {0}.", machineName);
                }
            }
            else
            {
                LOG.Warn("MachineNameCallback-- MachineNameSnapshot not found.");
            }

            ((IDisposable)sender).Dispose();

            // start the next probe 
            StartServiceStatusCollector();
        }

        //END SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
        /// <summary>
        /// Define the Query Monitor callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void AlwaysOnMetricsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader reader = e.Value as SqlDataReader)
            {
                if (reader.HasRows)
                {
                    AlwaysOnAvailabilityGroupsSnapshot snapshot = new AlwaysOnAvailabilityGroupsSnapshot();

                    ReadAlwaysOnAvailabilityGroups(snapshot, reader);
                    ReadAlwaysOnReplicas(snapshot, reader);
                    ReadAlwaysOnDatabases(snapshot, reader);
                    ReadAlwaysOnStatistics(snapshot, reader);
                    refresh.AvailabilityGroupsSnapshot = snapshot;

                }
                else
                {
                    refresh.AvailabilityGroupsSnapshot = null;
                }
            }
        }


        /// <summary>
        /// OS Metrics failure delegate
        /// Since OS Metrics are optional we do not want to fail the entire refresh if unavailable
        /// </summary>
        /// <param name="snapshot">Snapshot to return with partial failure</param>
        protected void AlwaysOnMetricsFailureDelegate(Snapshot snapshot, Exception e)
        {
            ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "There was an error in the Always On collector: {0}", e, true);
        }

        #endregion

        #region AlwaysOn Interpretation Methods

        /// <summary>
        /// Fill the 'AlwaysOnAvailabilityGroupsSnapshot', based in the collected information from 'dataReader'.
        /// </summary>
        /// <param name="snapshot">The 'AlwaysOnAvailabilityGroupsSnapshot' in which push the 'dataReader'.</param>
        /// <param name="dataReader">The result set of the executed batch (AlwaysOnTopology2012.sql).</param>
        private void ReadAlwaysOnAvailabilityGroups(AlwaysOnAvailabilityGroupsSnapshot snapshot, SqlDataReader dataReader)
        {
            while (dataReader.Read())
            {
                // Fill 'Availability Group'.
                AvailabilityGroup availabilityGroup = new AvailabilityGroup();

                int ordinalColumnIndex;

                if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("group_id")))
                {
                    availabilityGroup.GroupId = dataReader.GetGuid(ordinalColumnIndex);
                }

                if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("Groupname")))
                {
                    availabilityGroup.GroupName = dataReader.GetString(ordinalColumnIndex);
                }

                if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("listenerDNSName")))
                {
                    availabilityGroup.ListenerDnsName = dataReader.GetString(ordinalColumnIndex);
                }

                if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("listnerPort")))
                {
                    availabilityGroup.ListenerPort = dataReader.GetInt32(ordinalColumnIndex);
                }

                if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("listenerIPAddress")))
                {
                    availabilityGroup.ListenerIPAddress = dataReader.GetString(ordinalColumnIndex);
                }

                if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("ServerSourceName")))
                {
                    availabilityGroup.ServerSourceName = dataReader.GetString(ordinalColumnIndex);
                }

                snapshot.AddAvailabilityGroup(availabilityGroup);
            }
        }

        /// <summary>
        /// Fill the 'availability replicas' based in the content of 'dataReader'.
        /// </summary>
        /// <param name="snapshot">The 'AlwaysOnAvailabilityGroupsSnapshot' in which push the 'dataReader'.</param>
        /// <param name="dataReader">The result set that contains the 'AvailabilityReplicas' information.</param>
        private void ReadAlwaysOnReplicas(AlwaysOnAvailabilityGroupsSnapshot snapshot, SqlDataReader dataReader)
        {
            if (dataReader.NextResult())
            {
                while (dataReader.Read())
                {
                    int ordinalColumnIndex;

                    if (dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("group_id")))
                        continue; //IF the group Id is not present then continue with the next record
                    Guid groupID = dataReader.GetGuid(ordinalColumnIndex); //get the Group ID

                    AvailabilityGroup availabilityGroup = null;
                    try
                    {
                        availabilityGroup = snapshot.GetAvailabilityGroup(groupID);
                    }
                    catch (Exception e)
                    {
                        LOG.WarnFormat("Alwayson: Group GUID {0} For the Replica {1} is not found in this Availability group", groupID, dataReader.GetGuid(0));

                        if (!(e is KeyNotFoundException))
                        {
                            LOG.Error(e);
                        }
                    }
                    if (availabilityGroup == null) //the group id is not found in the list
                        continue;

                    // Build 'AvailabilityReplica' based in the probe response.
                    AvailabilityReplica availabilityReplica = new AvailabilityReplica(availabilityGroup.GroupId);

                    if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("replica_id")))
                    {
                        availabilityReplica.ReplicaId = dataReader.GetGuid(ordinalColumnIndex);
                    }
                    if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("Replica_Name")))
                    {
                        availabilityReplica.ReplicaName = dataReader.GetString(ordinalColumnIndex);
                    }
                    if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("failover_mode")))
                    {
                        availabilityReplica.FailoverMode =
                            AvailabilityReplica.ConvertToFailoverMode(dataReader.GetByte(ordinalColumnIndex));
                    }
                    if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("availability_mode")))
                    {
                        availabilityReplica.AvailabilityMode =
                            AvailabilityReplica.ConvertToAvailabilityMode(dataReader.GetByte(ordinalColumnIndex));
                    }
                    if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("Replica_Role")))
                    {
                        availabilityReplica.ReplicaRole =
                            AlwaysOnStatistics.ConvertToReplicaRole(dataReader.GetByte(ordinalColumnIndex));
                    }
                    if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("primary_role_allow_connections")))
                    {
                        availabilityReplica.PrimaryConnectionMode =
                            (PrimaryConnectionMode)dataReader.GetByte(ordinalColumnIndex);
                    }
                    if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("secondary_role_allow_connections")))
                    {
                        availabilityReplica.SecondaryConnectionMode =
                            (SecondaryConnectionMode)dataReader.GetByte(ordinalColumnIndex);
                    }
                    if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("redo_queue_size")))
                    {
                        availabilityReplica.RedoQueueSize = dataReader.GetInt64(ordinalColumnIndex);
                    }
                    if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("redo_rate")))
                    {
                        availabilityReplica.RedoRate = dataReader.GetInt64(ordinalColumnIndex);
                    }
                    if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("log_send_queue_size")))
                    {
                        availabilityReplica.LogSendQueueSize = dataReader.GetInt64(ordinalColumnIndex);
                    }
                    if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("log_send_rate")))
                    {
                        availabilityReplica.LogSendRate = dataReader.GetInt64(ordinalColumnIndex);
                    }
                    if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("ServerSourceName")))
                    {
                        availabilityReplica.ServerSourceName = dataReader.GetString(ordinalColumnIndex);
                    }

                    availabilityGroup.AddReplica(availabilityReplica);
                }
            }
        }

        /// <summary>
        /// Fill the 'availability databases' for the replicas.
        /// </summary>
        /// <param name="snapshot">The 'AlwaysOnAvailabilityGroupsSnapshot' in which push the 'dataReader'.</param>
        /// <param name="dataReader">The result set that contains the 'AlwaysOnDatabase' information.</param>
        private void ReadAlwaysOnDatabases(AlwaysOnAvailabilityGroupsSnapshot snapshot, SqlDataReader dataReader)
        {
            if (dataReader.NextResult())
            {
                while (dataReader.Read())
                {
                    int ordinalColumnIndex;

                    if (dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("group_id")))
                        continue; //IF the group Id is not present then continue with the next record

                    Guid groupID = dataReader.GetGuid(ordinalColumnIndex); //get the Group ID

                    AvailabilityGroup availabilityGroup = null;

                    try
                    {
                        availabilityGroup = snapshot.GetAvailabilityGroup(groupID);
                    }
                    catch (Exception e)
                    {
                        LOG.WarnFormat("Alwayson: Group GUID {0} For the Replica {1} is not found in this Availability group", groupID, dataReader.GetGuid(0));
                        
                        if (!(e is KeyNotFoundException))
                        {
                            LOG.Error(e);
                        }
                    }

                    if (availabilityGroup == null) //the group id is not found in the list
                        continue;

                    if (dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("replica_id")))
                        continue; //IF the replica Id is not present then continue with the next record

                    Guid replicaID = dataReader.GetGuid(ordinalColumnIndex); //get the replica ID

                    if (!availabilityGroup.Replicas.ContainsKey(replicaID))
                        continue;

                    AvailabilityReplica availabilityReplica = availabilityGroup.Replicas[replicaID];

                    // Build 'AlwaysOnDatabase' based in the probe response.
                    AlwaysOnDatabase availabilityDatabase = new AlwaysOnDatabase(availabilityReplica.ReplicaId, availabilityReplica.GroupId);

                    if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("group_database_id")))
                    {
                        availabilityDatabase.GroupDatabaseId = dataReader.GetGuid(ordinalColumnIndex);
                    }
                    if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("DatabaseName")))
                    {
                        availabilityDatabase.DatabaseName = dataReader.GetString(ordinalColumnIndex);
                    }
                    if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("database_id")))
                    {
                        availabilityDatabase.DatabaseId = dataReader.GetInt32(ordinalColumnIndex);
                    }
                    if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("ServerSourceName")))
                    {
                        availabilityDatabase.ServerSourceName = dataReader.GetString(ordinalColumnIndex);
                    }

                    availabilityReplica.AddDatabase(availabilityDatabase);
                }
            }
        }

        private void ReadAlwaysOnStatistics(AlwaysOnAvailabilityGroupsSnapshot snapshot, SqlDataReader reader)
        {
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    AlwaysOnStatistics alwaysOnStatistics = new AlwaysOnStatistics();
                    if (!reader.IsDBNull(0)) alwaysOnStatistics.ReplicaId = reader.GetGuid(0);
                    if (!reader.IsDBNull(1)) alwaysOnStatistics.GroupId = reader.GetGuid(1);
                    if (!reader.IsDBNull(2)) alwaysOnStatistics.DatabaseId = reader.GetInt32(2);
                    if (!reader.IsDBNull(3)) alwaysOnStatistics.IsFailoverReady = reader.GetBoolean(3);

                    if (!reader.IsDBNull(4)) alwaysOnStatistics.SynchronizationDatabaseState =
                        AlwaysOnStatistics.ConvertToAlwaysOnSynchronizationState(reader.GetByte(4));

                    if (!reader.IsDBNull(5)) alwaysOnStatistics.SynchronizationDatabaseHealth =
                        AlwaysOnStatistics.ConvertToAlwaysOnSynchronizationHealth(reader.GetByte(5));

                    if (!reader.IsDBNull(6)) alwaysOnStatistics.DatabaseState =
                        AlwaysOnStatistics.ConvertToDatabaseStatusNoMaskBit(reader.GetByte(6));

                    if (!reader.IsDBNull(7)) alwaysOnStatistics.IsSuspended = reader.GetBoolean(7);
                    if (!reader.IsDBNull(8)) alwaysOnStatistics.LastHardenedTime = reader.GetDateTime(8);
                    if (!reader.IsDBNull(9)) alwaysOnStatistics.LogSendQueueSize = reader.GetInt64(9);
                    if (!reader.IsDBNull(10)) alwaysOnStatistics.LogSendRate = reader.GetInt64(10);
                    if (!reader.IsDBNull(11)) alwaysOnStatistics.RedoQueueSize = reader.GetInt64(11);
                    if (!reader.IsDBNull(12)) alwaysOnStatistics.RedoRate = reader.GetInt64(12);

                    if (!reader.IsDBNull(13)) alwaysOnStatistics.ReplicaRole =
                        AlwaysOnStatistics.ConvertToReplicaRole(reader.GetByte(13));

                    if (!reader.IsDBNull(14)) alwaysOnStatistics.OperationalState =
                        AlwaysOnStatistics.ConvertToOperationalState(reader.GetByte(14));

                    if (!reader.IsDBNull(15)) alwaysOnStatistics.ConnectedState =
                        reader.GetByte(15) == 1 ? ConnectedState.Connected : ConnectedState.Disconnected;

                    if (!reader.IsDBNull(16)) alwaysOnStatistics.LastConnectionErrorNumber = reader.GetInt32(16);
                    if (!reader.IsDBNull(17)) alwaysOnStatistics.LastConnectedErrorDescription = reader.GetString(17);
                    if (!reader.IsDBNull(18)) alwaysOnStatistics.LastConnectErrorTimestamp = reader.GetDateTime(18);
                    if (!reader.IsDBNull(19)) alwaysOnStatistics.FileStreamSendRate = reader.GetInt64(19);
                    if (!reader.IsDBNull(20)) alwaysOnStatistics.SynchronizationPerformace = reader.GetInt32(20);
                    if (!reader.IsDBNull(21)) alwaysOnStatistics.EstimatedDataLossTime = reader.GetInt32(21);
                    if (!reader.IsDBNull(22)) alwaysOnStatistics.EstimatedRecoveryTime = reader.GetInt32(22);
                    if (!reader.IsDBNull(23)) alwaysOnStatistics.GroupDatabaseId = reader.GetGuid(23);

                    snapshot.AddReplicaStatistic(alwaysOnStatistics);
                }
            }
        }
        #endregion

        #endregion

        #region Table Growth

        #region Table Growth Collector Methods

        ///// <summary>
        ///// Starts the Table Growth collector
        ///// </summary>
        //private void StartTableGrowthCollector()
        //{
        //    connectionInfo.ConnectionTimeout = 10 * 60;
        //    StartGenericCollector(new Collector(TableGrowthCollector),
        //                          refresh,
        //                          "StartTableGrowthCollector",
        //                          "Table Growth",
        //                          refresh.Server.ProductVersion,
        //                          sqlErrorDelegate);
        //}


        ///// <summary>
        ///// Callback used to process the data returned from the Table Growth collector.
        ///// </summary>
        ///// <param name="sender">The sender.</param>
        ///// <param name="e">The Idera.SQLdm.Probes.Collectors.CollectorCompleteEventArgs instance containing the event data.</param>
        //private void TableGrowthCallback(object sender, CollectorCompleteEventArgs e)
        //{
        //    if (Thread.CurrentThread.IsThreadPoolThread)
        //    {
        //        LOG.Debug("Pushing TableGrowthCallback to work queue.");
        //        QueueCallback(refresh, sender as SqlCollector, TableGrowthCallback, e);
        //        return;
        //    }

        //    NextCollector nextCollector = new NextCollector(StartQueryMonitorCollector);
        //    if (refresh.TimeStampLocal.HasValue)
        //    {
        //        if ((refresh.MonitoredServer.ReorgStatisticsStartTime.HasValue) &&
        //            MonitoredSqlServer.MatchDayOfWeek(refresh.TimeStampLocal.Value.DayOfWeek,
        //                                              refresh.MonitoredServer.ReorgStatisticsDays))
        //        {
        //            if (
        //                ((!refresh.MonitoredServer.LastReorgStatisticsRunTime.HasValue) //If there is no known last refresh
        //                 ||
        //                 (refresh.MonitoredServer.LastReorgStatisticsRunTime.Value.DayOfYear <
        //                  refresh.TimeStampLocal.Value.DayOfYear) //Or if the last refresh was yesterday
        //                 ||
        //                 (refresh.MonitoredServer.LastReorgStatisticsRunTime.Value.Year <
        //                  refresh.TimeStampLocal.Value.Year) //Or if the last refresh fell in the last year
        //                 ||
        //                 (refresh.MonitoredServer.LastReorgStatisticsRunTime.Value.TimeOfDay <
        //                  refresh.MonitoredServer.ReorgStatisticsStartTime.Value.TimeOfDay) //Or the stats time has been moved forward
        //                  ||
        //                  (previousRefresh.ReorgRetryTables.Count > 0 || previousRefresh.ReorgRetryDatabases.Count > 0) // Or we are retrying something
        //                )
        //                &&
        //                (refresh.TimeStampLocal.Value.TimeOfDay >=
        //                 refresh.MonitoredServer.ReorgStatisticsStartTime.Value.TimeOfDay)
        //                &&
        //                (refresh.TimeStampLocal.Value.TimeOfDay <=
        //                 refresh.MonitoredServer.ReorgStatisticsStartTime.Value.TimeOfDay + TimeSpan.FromHours(3)))
        //            //And the current server time is within the window
        //            {
        //                nextCollector = new NextCollector(StartReorganizationCollector);
        //            }
        //        }
        //    }
        //    GenericCallback(new CollectorCallback(TableGrowthCallback),
        //                    refresh,
        //                    "TableGrowthCallback",
        //                    "Table Growth",
        //                    nextCollector,
        //                    sender,
        //                    e);
        //}

        ///// <summary>
        ///// Create Table Growth Collector
        ///// </summary>
        ///// <param name="conn">Open SQL connection</param>
        ///// <param name="sdtCollector">Standard SQL collector</param>
        ///// <param name="ver">Server version</param>
        //private void TableGrowthCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        //{
        //    SqlCommand cmd =
        //        SqlCommandBuilder.BuildTableGrowthCommand(conn,
        //                                                  ver,
        //                                                  refresh.MonitoredServer.TableStatisticsExcludedDatabases,
        //                                                  false);
        //    sdtCollector = new SqlCollector(cmd, true);
        //    sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(TableGrowthCallback));
        //}


        ///// <summary>
        ///// Define the Table Growth callback
        ///// </summary>
        ///// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        //private void TableGrowthCallback(CollectorCompleteEventArgs e)
        //{
        //    refresh.MonitoredServer.PreviousGrowthStatisticsRunTime =
        //        refresh.MonitoredServer.LastGrowthStatisticsRunTime;
        //    refresh.MonitoredServer.LastGrowthStatisticsRunTime = refresh.TimeStampLocal;
        //    using (SqlDataReader rd = e.Value as SqlDataReader)
        //    {
        //        InterpretTableGrowth(rd);
        //    }
        //}

        //#endregion

        //#region Table Growth Interpretation Methods

        //private void InterpretTableGrowth(SqlDataReader dataReader)
        //{
        //    using (LOG.DebugCall("InterpretTableGrowth"))
        //    {
        //        try
        //        {
        //            string dbName;
        //            string tableName;
        //            string schemaName;
        //            bool isSystemTable;
        //            while (dataReader.Read())
        //            {
        //                if (dataReader.FieldCount == 9)
        //                {
        //                    if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(1) && !dataReader.IsDBNull(7))
        //                    {
        //                        dbName = dataReader.IsDBNull(0) ? null : dataReader.GetString(0).TrimEnd();
        //                        tableName = dataReader.IsDBNull(1) ? null : dataReader.GetString(1);
        //                        schemaName = dataReader.IsDBNull(7) ? null : dataReader.GetString(7);
        //                        isSystemTable = dataReader.IsDBNull(8) ? false : dataReader.GetBoolean(8);

        //                        //Add the database to our dictionary if we haven't already
        //                        if (!refresh.DbStatistics.ContainsKey(dbName))
        //                        {
        //                            refresh.DbStatistics.Add(dbName, new DatabaseStatistics(refresh.ServerName, dbName));
        //                        }


        //                        //Add the database to our dictionary if we haven't already
        //                        if (!refresh.DbStatistics[dbName].TableSizes.ContainsKey(tableName))
        //                        {
        //                            refresh.DbStatistics[dbName].TableSizes.Add(tableName,
        //                                                                        new TableSize(refresh.ServerName, dbName,
        //                                                                                      tableName, schemaName,
        //                                                                                      isSystemTable));
        //                        }

        //                        if (!dataReader.IsDBNull(2))
        //                            refresh.DbStatistics[dbName].TableSizes[tableName].DataSize =
        //                                new FileSize(dataReader.GetDecimal(2));

        //                        if (!dataReader.IsDBNull(3))
        //                            refresh.DbStatistics[dbName].TableSizes[tableName].IndexSize =
        //                                new FileSize(dataReader.GetDecimal(3));

        //                        if (!dataReader.IsDBNull(4))
        //                            refresh.DbStatistics[dbName].TableSizes[tableName].ImageTextSize =
        //                                new FileSize(dataReader.GetDecimal(4));

        //                        if (!dataReader.IsDBNull(5))
        //                            refresh.DbStatistics[
        //                                dbName].TableSizes[tableName].RowCount = dataReader.GetInt64(5);
        //                    }
        //                    //error
        //                }
        //                //error
        //            }
        //        }
        //        catch (Exception)
        //        {
        //        }
        //        finally
        //        {
        //            dataReader.NextResult();
        //        }
        //    }
        //}

        #endregion

        #endregion

        #region Reorganization

        #region Reorganization Collector Methods

        ///// <summary>
        ///// Starts the Reorganization collector
        ///// </summary>
        //private void StartReorganizationCollector()
        //{
        //    //if (previousRefresh != null && previousRefresh.ReorgRetryDatabases != null && previousRefresh.ReorgRetryDatabases.Count > 0)
        //        refresh.ReorgRetryCounter++;
        //    //else
        //    //    refresh.ReorgRetryCounter = 0;
        //        if (refresh.ReorgRetryCounter > 5)
        //            refresh.ReorgRetryCounter = 0;

        //    connectionInfo.ConnectionTimeout = 10 * 60;
        //    StartGenericCollector(new Collector(ReorganizationCollector),
        //                          refresh,
        //                          "StartReorganizationCollector",
        //                          "Reorganization",
        //                          refresh.Server.ProductVersion,
        //                          sqlErrorDelegate);
        //}

        ///// <summary>
        ///// Callback used to process the data returned from the Reorganization collector.
        ///// </summary>
        ///// <param name="sender">The sender.</param>
        ///// <param name="e">The Idera.SQLdm.Probes.Collectors.CollectorCompleteEventArgs instance containing the event data.</param>
        //private void ReorganizationCallback(object sender, CollectorCompleteEventArgs e)
        //{
        //    if (Thread.CurrentThread.IsThreadPoolThread)
        //    {
        //        LOG.Debug("Pushing ReorganizationCallback to work queue.");
        //        QueueCallback(refresh, sender as SqlCollector, ReorganizationCallback, e);
        //        return;
        //    }


        //    GenericCallback(new CollectorCallback(ReorganizationCallback),
        //                    refresh,
        //                    "ReorganizationCallback",
        //                    "Reorganization",
        //                    new FailureDelegate(ReorganizationFailureDelegate),
        //                    new FailureDelegate(ReorganizationFailureDelegate),
        //                    null, //new NextCollector(StartQueryMonitorCollector),
        //                    sender,
        //                    e,
        //                    true,
        //                    true);

        //    if (!refresh.CollectionFailed)
        //        StartQueryMonitorCollector();
        //}

        //protected void ReorganizationFailureDelegate(Snapshot snapshot, Exception e)
        //{
        //    ProbeHelpers.LogAndAttachToSnapshot(refresh,LOG,"There was an error in the Reorganization collector: {0}",e,true);
        //}

        ///// <summary>
        ///// Create Reorganization Collector
        ///// </summary>
        ///// <param name="conn">Open SQL connection</param>
        ///// <param name="sdtCollector">Standard SQL collector</param>
        ///// <param name="ver">Server version</param>
        //private void ReorganizationCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        //{
        //    //conn.InfoMessage += new SqlInfoMessageEventHandler(InterpretReorganization);
        //    SqlCommand cmd =
        //        SqlCommandBuilder.BuildReorganizationCommand(conn,
        //                                                     ver,
        //                                                     refresh.MonitoredServer.TableStatisticsExcludedDatabases,
        //                                                     false,
        //                                                     workload,
        //                                                     previousRefresh != null ? previousRefresh.ReorgRetryDatabases:null,
        //                                                     previousRefresh != null ? previousRefresh.ReorgRetryTables : null,
        //                                                     refresh.ReorgRetryCounter % 2);
        //    sdtCollector = new SqlCollector(cmd, true);
        //    sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ReorganizationCallback));
        //}

        ///// <summary>
        ///// Define the Reorganization callback
        ///// </summary>
        ///// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        //private void ReorganizationCallback(CollectorCompleteEventArgs e)
        //{
        //    if (e.Value != null)
        //    {
        //        using (SqlDataReader rd = e.Value as SqlDataReader)
        //        {
        //            InterpretReorganization(rd);
        //        }
        //        refresh.MonitoredServer.PreviousReorgStatisticsRunTime =
        //            refresh.MonitoredServer.LastReorgStatisticsRunTime;
        //        refresh.MonitoredServer.LastReorgStatisticsRunTime = refresh.TimeStampLocal;
        //    }
        //    else
        //    {

        //        LOG.Warn("No data found for reorganization collector.  Will retry.");
        //    }
        //}

        #endregion

        #region Reorganization Interpretation Methods

        //private void InterpretReorganization(SqlDataReader dataReader)
        //{
        //    try
        //    {
        //        int currentDbid = 0;
        //        int deleteDbid = 0;
        //        int tableid = 0;
        //        int isSystemTable = 0;
        //        string schema = "";
        //        string tableName = "";
        //        string databaseName = "";

        //        while (dataReader.Read())
        //        {
        //            refresh.ReorgRetryDatabases.Add(dataReader.GetInt32(0));
        //        }

        //        while (dataReader.NextResult())
        //        {
        //            while (dataReader.Read())
        //            {
        //                if (dataReader.HasRows)
        //                {
        //                    switch (dataReader.GetString(0))
        //                    {
        //                        case "TableList":
        //                            {
        //                                // We've moved on to a new database so remove
        //                                // the last one from the retry list
        //                                if (refresh.ReorgRetryDatabases.Contains(deleteDbid))
        //                                    refresh.ReorgRetryDatabases.Remove(deleteDbid);

        //                                if (refresh.ReorgRetryTables.ContainsKey(deleteDbid))
        //                                    refresh.ReorgRetryTables.Remove(deleteDbid);

        //                                do
        //                                {
        //                                    if (!dataReader.IsDBNull(1))
        //                                        databaseName = dataReader.GetString(1);
        //                                    if (!dataReader.IsDBNull(2))
        //                                        currentDbid = dataReader.GetInt32(2);
        //                                    if (!dataReader.IsDBNull(3))
        //                                        tableid = dataReader.GetInt32(3);
        //                                    if (!dataReader.IsDBNull(4))
        //                                        isSystemTable = dataReader.GetInt32(4);
        //                                    if (!dataReader.IsDBNull(5))
        //                                        schema = dataReader.GetString(5);

        //                                    deleteDbid = currentDbid;

        //                                    if (refresh.ReorgRetryTables.Count == 0 ||
        //                                        !refresh.ReorgRetryTables.ContainsKey(currentDbid))
        //                                    {
        //                                        refresh.ReorgRetryTables.Add(currentDbid,
        //                                                                     new Dictionary<int, Pair<int, string>>());
        //                                    }

        //                                    if (!refresh.ReorgRetryTables[currentDbid].ContainsKey(tableid))
        //                                    {
        //                                        refresh.ReorgRetryTables[currentDbid].Add(tableid,
        //                                                                                  new Pair<int, string>(
        //                                                                                      isSystemTable,
        //                                                                                      schema));
        //                                    }
        //                                } while (dataReader.Read());
        //                                break;
        //                            }
        //                        case "Retry database":
        //                            {
        //                                if (!dataReader.IsDBNull(1))
        //                                    databaseName = dataReader.GetString(1);
        //                                if (!dataReader.IsDBNull(2))
        //                                    currentDbid = dataReader.GetInt32(2);
        //                                LOG.Info(
        //                                    String.Format("Reorganization Collector marking database {0} for retry.",
        //                                                  databaseName));
        //                                if (!refresh.ReorgRetryDatabases.Contains(currentDbid))
        //                                    refresh.ReorgRetryDatabases.Add(currentDbid);

        //                                // Copy table list from previous refresh in case most of the tables were already scanned
        //                                if (previousRefresh != null && previousRefresh.ReorgRetryTables != null
        //                                    && previousRefresh.ReorgRetryTables.ContainsKey(currentDbid))
        //                                {
        //                                    if (refresh.ReorgRetryTables.ContainsKey(currentDbid))
        //                                        refresh.ReorgRetryDatabases.Remove(currentDbid);

        //                                    refresh.ReorgRetryTables.Add(currentDbid,previousRefresh.ReorgRetryTables[currentDbid]);
        //                                }
        //                                // Set dbid to 0 to prevent it being removed from the list later
        //                                deleteDbid = 0;
        //                                break;
        //                            }
        //                        case "Retry Table":
        //                            {
        //                                if (!dataReader.IsDBNull(1))
        //                                    currentDbid = dataReader.GetInt32(1);
        //                                if (!dataReader.IsDBNull(2))
        //                                    tableid = dataReader.GetInt32(2);
        //                                if (!dataReader.IsDBNull(3))
        //                                    isSystemTable = dataReader.GetInt32(3);
        //                                if (!dataReader.IsDBNull(4))
        //                                    schema = dataReader.GetString(4);
        //                                LOG.Info(
        //                                    String.Format(
        //                                        "Reorganization Collector marking table {0} in database {1} for retry.",
        //                                        tableid,
        //                                        databaseName));

        //                                if (!refresh.ReorgRetryDatabases.Contains(currentDbid))
        //                                    refresh.ReorgRetryDatabases.Add(currentDbid);

        //                                if (refresh.ReorgRetryTables.Count == 0 ||
        //                                    !refresh.ReorgRetryTables.ContainsKey(currentDbid))
        //                                {
        //                                    refresh.ReorgRetryTables.Add(currentDbid,
        //                                                                 new Dictionary<int, Pair<int, string>>());
        //                                }

        //                                if (!refresh.ReorgRetryTables[currentDbid].ContainsKey(tableid))
        //                                {
        //                                    refresh.ReorgRetryTables[currentDbid].Add(tableid,
        //                                                                              new Pair<int, string>(
        //                                                                                  isSystemTable,
        //                                                                                  schema));
        //                                }
        //                                // Set dbid to 0 to prevent it being removed from the list later
        //                                deleteDbid = 0;
        //                                break;
        //                            }
        //                        case "Skip database":
        //                            {
        //                                int tempdbid = 0;
        //                                string tempDatabaseName = "";

        //                                if (!dataReader.IsDBNull(1))
        //                                    tempDatabaseName = dataReader.GetString(1);
        //                                if (!dataReader.IsDBNull(2))
        //                                    tempdbid = dataReader.GetInt32(2);

        //                                if (refresh.ReorgRetryDatabases.Contains(tempdbid))
        //                                    refresh.ReorgRetryDatabases.Remove(tempdbid);

        //                                if (refresh.ReorgRetryTables.ContainsKey(tempdbid))
        //                                    refresh.ReorgRetryTables.Remove(tempdbid);

        //                                LOG.Info(
        //                                    String.Format(
        //                                        "Reorganization Collector skipping database {0} as no valid objects were located.",
        //                                        tempDatabaseName));
        //                                break;
        //                            }
        //                        default:
        //                            {
        //                                if ((refresh.ProductVersion.Major == 8 && dataReader.FieldCount >= 10) || (refresh.ProductVersion.Major > 8 && dataReader.FieldCount == 3))
        //                                {

        //                                    if (!dataReader.IsDBNull(0))
        //                                        tableName = dataReader.GetString(0);
        //                                    if (!dataReader.IsDBNull(1))
        //                                        tableid = dataReader.GetInt32(1);

        //                                    if (refresh.ReorgRetryTables.ContainsKey(currentDbid) && refresh.ReorgRetryTables[currentDbid].ContainsKey(tableid))
        //                                    {
        //                                        isSystemTable = refresh.ReorgRetryTables[currentDbid][tableid].First;
        //                                        schema = refresh.ReorgRetryTables[currentDbid][tableid].Second;
        //                                        refresh.ReorgRetryTables[currentDbid].Remove(tableid);
        //                                        if (refresh.ReorgRetryTables[currentDbid].Count == 0)
        //                                            refresh.ReorgRetryTables.Remove(currentDbid);
        //                                    }

        //                                    if (tableName != null && databaseName != null && schema != null)
        //                                    {
        //                                        //Add the database to our dictionary if we haven't already
        //                                        if (!refresh.DbStatistics.ContainsKey(databaseName))
        //                                        {
        //                                            refresh.DbStatistics.Add(databaseName,
        //                                                                     new DatabaseStatistics(refresh.ServerName,
        //                                                                                            databaseName));
        //                                        }


        //                                        //Add the table to our dictionary if we haven't already
        //                                        if (
        //                                            !refresh.DbStatistics[databaseName].TableReorganizations.ContainsKey
        //                                                 (tableName))
        //                                        {
        //                                            refresh.DbStatistics[databaseName].TableReorganizations.Add(
        //                                                tableName,
        //                                                new TableReorganization
        //                                                    (refresh.
        //                                                         ServerName,
        //                                                     databaseName,
        //                                                     tableName,
        //                                                     schema,
        //                                                     isSystemTable > 0 ? true : false));
        //                                        }

        //                                        refresh.DbStatistics[databaseName].TableReorganizations[tableName].
        //                                            ScanDensity =
        //                                            refresh.ProductVersion.Major == 8 ? dataReader.GetDouble(15) : (double?)null;


        //                                        refresh.DbStatistics[databaseName].TableReorganizations[tableName].
        //                                            LogicalFragmentation
        //                                            = refresh.ProductVersion.Major == 8 ? dataReader.GetDouble(18) : dataReader.GetDouble(2);
        //                                    }
        //                                }

        //                                break;
        //                            }
        //                    }
        //                }
        //            }
        //        }

        //        // Remove last database if there is one
        //        if (refresh.ReorgRetryDatabases.Contains(deleteDbid))
        //            refresh.ReorgRetryDatabases.Remove(deleteDbid);

        //        if (refresh.ReorgRetryTables.ContainsKey(deleteDbid))
        //            refresh.ReorgRetryTables.Remove(deleteDbid);
        //    }
        //     catch (Exception exception)
        //    {
        //        ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error processing Reorganization data: {0}", exception,true);
        //    }
        //}

        ////private void InterpretReorganization(object sender, SqlInfoMessageEventArgs e)
        ////{
        ////    try
        ////    {
        ////        foreach (SqlError err in e.Errors)
        ////        {
        ////            if (DatabaseNameRegex.IsMatch(err.Message))
        ////                CurrentReorgDatabase = DatabaseNameRegex.Replace(err.Message, "").TrimEnd();

        ////            if (TableNameRegex.IsMatch(err.Message))
        ////                CurrentReorgTable = TableNameRegex.Replace(err.Message, "");

        ////            if (SchemaNameRegex.IsMatch(err.Message))
        ////                CurrentSchema = SchemaNameRegex.Replace(err.Message, "");

        ////            if (IsSystemTableRegex.IsMatch(err.Message))
        ////                CurrentIsSystemTable = SchemaNameRegex.Replace(err.Message, "") == "1" ? true : false;


        ////            bool ScanDensityMatch = ScanDensityRegex.IsMatch(err.Message);
        ////            bool LogicalFragmentationMatch = LogicalFragmentationRegex.IsMatch(err.Message);
        ////            if (ScanDensityMatch || LogicalFragmentationMatch)
        ////            {
        ////                if (CurrentReorgTable != null && CurrentReorgDatabase != null && CurrentSchema != null)
        ////                {
        ////                    //Add the database to our dictionary if we haven't already
        ////                    if (!refresh.DbStatistics.ContainsKey(CurrentReorgDatabase))
        ////                    {
        ////                        refresh.DbStatistics.Add(CurrentReorgDatabase,
        ////                                                 new DatabaseStatistics(refresh.ServerName, CurrentReorgDatabase));
        ////                    }


        ////                    //Add the table to our dictionary if we haven't already
        ////                    if (
        ////                        !refresh.DbStatistics[CurrentReorgDatabase].TableReorganizations.ContainsKey(
        ////                             CurrentReorgTable))
        ////                    {
        ////                        refresh.DbStatistics[CurrentReorgDatabase].TableReorganizations.Add(CurrentReorgTable,
        ////                                                                                            new TableReorganization
        ////                                                                                                (refresh.
        ////                                                                                                     ServerName,
        ////                                                                                                 CurrentReorgDatabase,
        ////                                                                                                 CurrentReorgTable,
        ////                                                                                                 CurrentSchema,
        ////                                                                                                 CurrentIsSystemTable));
        ////                    }

        ////                    if (ScanDensityMatch)
        ////                        refresh.DbStatistics[CurrentReorgDatabase].TableReorganizations[CurrentReorgTable].
        ////                            ScanDensity =
        ////                            Convert.ToDouble(ScanDensityRegex.Match(err.Message).Value,
        ////                                             CultureInfo.InvariantCulture);

        ////                    if (LogicalFragmentationMatch)
        ////                        refresh.DbStatistics[CurrentReorgDatabase].TableReorganizations[CurrentReorgTable].
        ////                            LogicalFragmentation
        ////                            =
        ////                            Convert.ToDouble(LogicalFragmentationRegex.Match(err.Message).Value,
        ////                                             CultureInfo.InvariantCulture);
        ////                }
        ////                else
        ////                {
        ////                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG,
        ////                                                        "Reorganization data was not saved due to missing table identifiers." +
        ////                                                        "DatabaseName: " + CurrentReorgDatabase + " TableName: " +
        ////                                                        CurrentReorgTable + " SchemaName: " + CurrentSchema,
        ////                                                        false);
        ////                }
        ////            }
        ////        }
        ////    }
        ////    catch (Exception exception)
        ////    {
        ////        ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error processing Reorganization data: {0}", exception,
        ////                                            false);
        ////    }
        ////}

        #endregion

        #endregion

        #region Query Monitor

        #region Query Monitor Collector Methods

        private StringBuilder queryMonitorDbSelection(bool notCondition,  string dbName)
        {
            StringBuilder res = new StringBuilder();
            res.Append(" AND name ");
            if (notCondition)
                res.Append("NOT ");
            res.Append("LIKE '" + dbName + "'");
            return res;
        }

        private string getDbPredicates(string[] dbs, bool notCondition)
        {
            StringBuilder res = new StringBuilder();
            if(dbs != null)
            {
                foreach (string filterString in dbs)
                {
                    if (filterString != null && filterString.Length > 0)
                    {
                        res.Append(queryMonitorDbSelection(notCondition, filterString));
                    }
                }
            }
            return res.ToString();
        }

        private void initializeDBNamesForQMAzure()
        {
            //GET CLOUD DB NAMES
            StringBuilder predicateString = new StringBuilder();
            // Prioritize Exclude, as per on-prem
            predicateString.Append(getDbPredicates(queryMonitorConfiguration.AdvancedConfiguration.DatabaseExcludeMatch, true));
            predicateString.Append(getDbPredicates(queryMonitorConfiguration.AdvancedConfiguration.DatabaseExcludeLike, true));
            if (predicateString.Length == 0)
            {
                predicateString.Append(getDbPredicates(queryMonitorConfiguration.AdvancedConfiguration.DatabaseIncludeMatch, false));
                predicateString.Append(getDbPredicates(queryMonitorConfiguration.AdvancedConfiguration.DatabaseIncludeLike, false));
            }
            string sqlQuery = SqlCommandBuilder.BuildFilteredMonitoredDatabasesAzureQuery(predicateString.ToString());
            cloudDBNamesQMAzure = CollectionHelper.GetFilteredDatabasesForMonitoringAzure(connectionInfo, sqlQuery, LOG);
        }
        /// <summary>
        /// Starts the Query Monitor collector
        /// </summary>
        private void StartQueryMonitorCollector()
        {
            try
            {
                if (qmCollectionState != null)
                {
                    lock (qmCollectionState)
                    {
                        if (qmCollectionState.UpdatedConfig != null)
                            queryMonitorConfiguration = qmCollectionState.UpdatedConfig;
                        qmCollectionState.QueryMonitorCollectorStarted = true;
                    }
                }
                if (cloudProviderId == CLOUD_PROVIDER_ID_AZURE)
                {
                    StartGenericCollectorDatabase(new CollectorDatabase(QueryMonitorCollectorDatabase),
                        refresh,
                        "StartQueryMonitorCollector",
                        "Query Monitor",
                        QueryMonitorCallback, cloudDBNamesQMAzure[numberOfDatabasesQM], new object[]
                        {
                            queryMonitorConfiguration.TraceMonitoringEnabled,
                            queryMonitorConfiguration.SqlBatchEventsEnabled,
                            queryMonitorConfiguration.SqlStatementEventsEnabled,
                            queryMonitorConfiguration.StoredProcedureEventsEnabled,
                            queryMonitorConfiguration.IsAlertResponseQueryTrace,
                            queryMonitorConfiguration.StopTimeUTC,
                            refresh.TimeStamp,
                            this.queryMonitorConfiguration.QueryStoreMonitoringEnabled    // SQLdm 10.4 (Varun Chopra) Passing Query Store parameters for permission check
                        });

                }
                else
                {
                    StartGenericCollector(
                        new Collector(QueryMonitorCollector),
                        refresh,
                        "StartQueryMonitorCollector",
                        "Query Monitor",
                        refresh.Server.ProductVersion,
                        sqlErrorDelegate,
                        QueryMonitorCallback,
                        new object[]
                            {
                                queryMonitorConfiguration.TraceMonitoringEnabled,
                                queryMonitorConfiguration.SqlBatchEventsEnabled,
                                queryMonitorConfiguration.SqlStatementEventsEnabled,
                                queryMonitorConfiguration.StoredProcedureEventsEnabled,
                                queryMonitorConfiguration.IsAlertResponseQueryTrace,
                                queryMonitorConfiguration.StopTimeUTC,
                                refresh.TimeStamp,
                                this.queryMonitorConfiguration.QueryStoreMonitoringEnabled    // SQLdm 10.4 (Varun Chopra) Passing Query Store parameters for permission check
                            });
                }
            }
            finally
            {
                PersistenceManager.Instance.SetQueryMonitorConfiguration(refresh.MonitoredServer.InstanceName,
                                                                         refresh.MonitoredServer.
                                                                             QueryMonitorConfiguration);
            }
        }


        /// <summary>
        /// Callback used to process the data returned from the Query Monitor collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The Idera.SQLdm.Probes.Collectors.CollectorCompleteEventArgs instance containing the event data.</param>
        private void QueryMonitorCallback(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing QueryMonitorCallback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, QueryMonitorCallback, e);
                return;
            }
            NextCollector nextCollector = new NextCollector(StartQueryMonitorStateStoreCollector);
            if (cloudProviderId == CLOUD_PROVIDER_ID_AZURE)
            {
                Interlocked.Increment(ref numberOfDatabasesQM);
                nextCollector = numberOfDatabasesQM < cloudDBNamesQMAzure.Count
                        ? StartQueryMonitorCollector
                        : nextCollector;
            }
            GenericCallback(new CollectorCallback(QueryMonitorCallback),
                            refresh,
                            "QueryMonitorCallback",
                            "Query Monitor",
                            new FailureDelegate(QueryMonitorFailureDelegate),
                            new FailureDelegate(QueryMonitorFailureDelegate),
                            nextCollector,   //SQLDM 10.3 (Manali Hukkeri) : Technical debt changes
                            sender,
                            e,
                            true,
                            true);

        }

        protected void QueryMonitorFailureDelegate(Snapshot snapshot, Exception e)
        {
            //START-SQLdm 9.1 (Ankit Srivastava) handling old/deleted .xel file scneario - logging it specifically
            if (e.Message != null && (e.Message.ToLower().Contains("is invalid for log file") ||
                    e.Message.ToLower().Contains("specify an offset that exists in the log file and retry your query")))
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "The Query Monitor Collection with Extended Event skipped because of following error , this collection would resume in next collection cycle: {0}", e, true);
            }
            else
                //END-SQLdm 9.1 (Ankit Srivastava) handling old/deleted .xel file scneario - logging it specifically
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "There was an error in the Query Monitor collector: {0}", e, true);
        }

        private void QueryMonitorCollectorDatabase(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver, string dbName)
        {       
            SqlCommand cmd;
            //SQlDM-28022 - Handling connection object to avoid leakage
            if (conn == null || conn.State != System.Data.ConnectionState.Open)
            {
                conn = OpenConnection();
            }
            SqlCommand newCmd = conn.CreateCommand(); //SQLdm 9.0 (Ankit Srivastava) -Query Monitoring Improvement --  new additional SQL command

            if (qmCollectionState != null)
            {
                lock (qmCollectionState)
                {
                    if (qmCollectionState.UpdatedConfig != null)
                        queryMonitorConfiguration = qmCollectionState.UpdatedConfig;
                    qmCollectionState.QueryMonitorCollectorStarted = true;
                }
            }

            if (!queryMonitorConfiguration.Enabled)
            { 
                PersistenceManager.Instance.ClearAzureQsStartTime(workload.Id, dbName, AzureQsType.QueryMonitor);

                //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session - stoping the extended event session if disabled
                LOG.Verbose(queryMonitorLogStartTemplate + "Query monitor Extended Events Session disabled - stopping Extended Events Session if started");
                cmd = SqlCommandBuilder.BuildQueryMonitorStopCommandEX(conn, ver, workload.MonitoredServer.CloudProviderId);

            }
            else
            {
                var queryStoreMonitoringEnabled = this.queryMonitorConfiguration.QueryStoreMonitoringEnabled;

                if (previousRefresh != null)
                {
                    bool hasBeenStopped = (!previousRefresh.MonitoredServer.QueryMonitorConfiguration.Enabled);

                    if (!queryStoreMonitoringEnabled) //SQLdm 10.4 (Varun Chopra) Query Monitoring with Query Store
                    {
                        cmd = SqlCommandBuilder.BuildQueryMonitorEXCommand(
                            conn,
                            ver,
                            queryMonitorConfiguration,
                            previousRefresh.MonitoredServer.QueryMonitorConfiguration,
                            refresh.MonitoredServer.ActiveWaitsConfiguration,
                            refresh.TimeStamp,
                            hasBeenStopped,
                            workload.MonitoredServer.CloudProviderId,
                            cloudDBNamesQMAzure.Count
                        );
                    }
                    else
                    {
                        // Stop Extended Event Commands
                        LOG.Verbose(queryMonitorLogStartTemplate + "Query monitor Extended Events Session disabled - stopping Extended Events Session since Query Store Enabled Now");
                        newCmd = SqlCommandBuilder.BuildQueryMonitorStopCommandEX(
                            conn,
                            ver,
                            workload.MonitoredServer.CloudProviderId);

                        // Start Query Store Command
                        cmd = SqlCommandBuilder.BuildQueryMonitorQsCommand(
                            conn,
                            ver,
                            queryMonitorConfiguration,
                            workload.MonitoredServer.Id,
                            workload.MonitoredServer.CloudProviderId);
                    }
                }
                else
                {
                    if (!queryStoreMonitoringEnabled) //SQLdm 10.4 (Varun Chopra) Query Monitoring with Query Store
                    {
                        cmd = SqlCommandBuilder.BuildQueryMonitorEXCommandWithRestart(
                            conn,
                            ver,
                            queryMonitorConfiguration,
                            refresh.MonitoredServer.ActiveWaitsConfiguration,
                            refresh.TimeStamp,
                            workload.MonitoredServer.CloudProviderId,
                            cloudDBNamesQMAzure.Count
                        );
                    }
                    else
                    {
                        // Stop extended event session
                        LOG.Verbose(queryMonitorLogStartTemplate + "Query monitor Extended Events Session disabled - stopping Extended Events Session since trace Enabled Now");
                        newCmd = SqlCommandBuilder.BuildQueryMonitorStopCommandEX(
                            conn,
                            ver,
                            workload.MonitoredServer.CloudProviderId);

                        // Query Store Command
                        cmd = SqlCommandBuilder.BuildQueryMonitorQsCommandWithRestart(
                            conn,
                            ver,
                            queryMonitorConfiguration,
                            workload.MonitoredServer.Id,
                            workload.MonitoredServer.CloudProviderId
                            );
                    }
                }
            }
            if (!String.IsNullOrEmpty(newCmd.CommandText))
                newCmd.CommandText += cmd.CommandText;
            else
                newCmd = cmd;

            sdtCollector = new SqlCollector(newCmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(QueryMonitorCallback));
        }

        /// <summary>
        /// Create the Query Monitor collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void QueryMonitorCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd;
            //SQlDM-28022 - Handling connection object to avoid leakage
            if (conn == null || conn.State != System.Data.ConnectionState.Open)
            {
                conn = OpenConnection();
            }
            SqlCommand newCmd = conn.CreateCommand(); //SQLdm 9.0 (Ankit Srivastava) -Query Monitoring Improvement --  new additional SQL command

            if (qmCollectionState != null)
            {
                lock (qmCollectionState)
                {
                    if (qmCollectionState.UpdatedConfig != null)
                        queryMonitorConfiguration = qmCollectionState.UpdatedConfig;
                    qmCollectionState.QueryMonitorCollectorStarted = true;
                }
            }

            // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
            // Stop trace every refresh if disabled
            if (!queryMonitorConfiguration.Enabled)
            {
                LOG.Verbose(queryMonitorLogStartTemplate + "Query monitor trace disabled - stopping trace if started");
                newCmd = SqlCommandBuilder.BuildQueryMonitorStopCommand(conn, ver, workload.MonitoredServer.CloudProviderId);//SQLdm 9.0 (Ankit Srivastva) - Fixed the error found in the log file for SQL 2000 and 2005

                //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session - stoping the extended event session if disabled
                LOG.Verbose(queryMonitorLogStartTemplate + "Query monitor Extended Events Session disabled - stopping Extended Events Session if started");
                cmd = SqlCommandBuilder.BuildQueryMonitorStopCommandEX(conn, ver, workload.MonitoredServer.CloudProviderId);

                if (ver.Major >= 11 || cloudProviderId == Constants.MicrosoftAzureManagedInstanceId)
                {
                    //SQLdm 10.4 (Varun Chopra) Query Monitoring with Query Store - clearing the State if disabled
                    LOG.Verbose(queryMonitorLogStartTemplate + "Query monitor Query Store disabled - clearing Query Store since trace Enabled Now");
                    newCmd = SqlCommandBuilder.BuildQueryMonitorStopCommandQs(conn, ver, workload.MonitoredServer.CloudProviderId);
                }
            }
            else
            {
                // Restart trace if necessary
                if (previousRefresh != null)
                {
                    //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session --stopping and starting the trace/extended event session accordingly -start 
                    bool traceMonitoringEnabled = queryMonitorConfiguration.TraceMonitoringEnabled;
                    //SQLdm 10.4 (Varun Chopra) Query Monitoring with Query Store
                    var queryStoreMonitoringEnabled = this.queryMonitorConfiguration.QueryStoreMonitoringEnabled;

                    if (traceMonitoringEnabled)
                    {
                        //start -SQLdm 9.0 (Ankit Srivastava) -- defect DE43960 -shifted stop commands with the start ones
                        LOG.Verbose(queryMonitorLogStartTemplate + "Query monitor Extended Events Session disabled - stopping Extended Events Session since trace Enabled Now");
                        newCmd = SqlCommandBuilder.BuildQueryMonitorStopCommandEX(conn, ver, workload.MonitoredServer.CloudProviderId);
                        //end -SQLdm 9.0 (Ankit Srivastava) -- defect DE43960 -shifted stop commands with the start ones

                        if (ver.Major >= 11)
                        {
                            //SQLdm 10.4 (Varun Chopra) Query Monitoring with Query Store - clearing the state if disabled
                            LOG.Verbose(queryMonitorLogStartTemplate + "Query monitor Query Store disabled - clearing Query Store since trace Enabled Now");
                            newCmd = SqlCommandBuilder.BuildQueryMonitorStopCommandQs(conn, ver, workload.MonitoredServer.CloudProviderId);
                        }

                        cmd = SqlCommandBuilder.BuildQueryMonitorTraceCommand(
                        conn,
                        ver,
                        queryMonitorConfiguration,
                        previousRefresh.MonitoredServer.QueryMonitorConfiguration,
                            refresh.TimeStamp, workload.MonitoredServer.CloudProviderId);

                    }
                    else if (!queryStoreMonitoringEnabled)  //SQLdm 10.4 (Varun Chopra) Query Monitoring with Query Store
                    {
                        //start -SQLdm 9.0 (Ankit Srivastava) -- defect DE43960 -shifted stop commands with the start ones
                        LOG.Verbose(queryMonitorLogStartTemplate + "Query monitor trace disabled - stopping trace since Extended Events Session Enabled Now");
                        newCmd = SqlCommandBuilder.BuildQueryMonitorStopCommand(conn, ver, workload.MonitoredServer.CloudProviderId);
                        bool hasBeenStopped = (!previousRefresh.MonitoredServer.QueryMonitorConfiguration.Enabled);
                        //end -SQLdm 9.0 (Ankit Srivastava) -- defect DE43960 -shifted stop commands with the start ones

                        //SQLdm 10.4 (Varun Chopra) Query Monitoring with Query Store - clearing the state if disabled
                        LOG.Verbose(queryMonitorLogStartTemplate + "Query monitor Query Store disabled - clearing Query Store since trace Enabled Now");
                        newCmd = SqlCommandBuilder.BuildQueryMonitorStopCommandQs(conn, ver, workload.MonitoredServer.CloudProviderId);

                        cmd = SqlCommandBuilder.BuildQueryMonitorEXCommand(
                            conn,
                            ver,
                            queryMonitorConfiguration,
                            previousRefresh.MonitoredServer.QueryMonitorConfiguration,
                            refresh.MonitoredServer.ActiveWaitsConfiguration,
                            refresh.TimeStamp,
                            hasBeenStopped,
                            workload.MonitoredServer.CloudProviderId);
                    }
                    else  // Query Store Monitoring Enabled
                    {
                        // Stop Trace Commands
                        LOG.Verbose(queryMonitorLogStartTemplate + "Query monitor trace disabled - stopping trace since Query Store Enabled Now");
                        if (cloudProviderId != Constants.MicrosoftAzureId &&
                            cloudProviderId != Constants.MicrosoftAzureManagedInstanceId)
                        {
                            newCmd = SqlCommandBuilder.BuildQueryMonitorStopCommand(
                                conn,
                                ver,
                                workload.MonitoredServer.CloudProviderId);
                        }

                        // Stop Extended Event Commands
                        LOG.Verbose(queryMonitorLogStartTemplate + "Query monitor Extended Events Session disabled - stopping Extended Events Session since Query Store Enabled Now");
                        newCmd = SqlCommandBuilder.BuildQueryMonitorStopCommandEX(
                            conn,
                            ver,
                            workload.MonitoredServer.CloudProviderId);

                        // Start Query Store Command
                        cmd = SqlCommandBuilder.BuildQueryMonitorQsCommand(
                            conn,
                            ver,
                            queryMonitorConfiguration,
                            workload.MonitoredServer.Id,
                            workload.MonitoredServer.CloudProviderId);
                    }
                }
                //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session --stopping and starting the trace/extended event session accordingly - end
                else
                {
                    // Prior to 7.5.2 we would read the previous configuration from persistence when the service restarted
                    // Starting with 7.5.2 we actually just want to go ahead and restart the trace if the previous state is
                    // not known. The direct cause is PR 18126, which was causing duplicate alerting
                    //
                    //QueryMonitorConfiguration qmc =
                    //    PersistenceManager.Instance.GetQueryMonitorConfiguration(
                    //        refresh.MonitoredServer.InstanceName);
                    //if (qmc != null)
                    //{
                    //    cmd = SqlCommandBuilder.BuildQueryMonitorTraceCommand(
                    //        conn,
                    //        ver,
                    //        queryMonitorConfiguration,
                    //        qmc,
                    //        refresh.TimeStamp);
                    //}
                    //else
                    //{
                    // If we don't know the previous configuration, go ahead and restart so we know we're operating with
                    // appropriate flags/filters on the trace
                    //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session - starting the trace OR extended event session accordingly -start
                    if (queryMonitorConfiguration.TraceMonitoringEnabled)
                    {
                        //start -SQLdm 9.0 (Ankit Srivastava) -- defect DE43960 -shifted stop commands with the start ones
                        //stopping extended event session
                        LOG.Verbose(queryMonitorLogStartTemplate + "Query monitor Extended Events Session disabled - stopping Extended Events Session since trace Enabled Now");
                        newCmd = SqlCommandBuilder.BuildQueryMonitorStopCommandEX(conn, ver, workload.MonitoredServer.CloudProviderId);
                        //end -SQLdm 9.0 (Ankit Srivastava) -- defect DE43960 -shifted stop commands with the start ones

                        //SQLdm 10.4 (Varun Chopra) Query Monitoring with Query Store - clearing the state if disabled
                        LOG.Verbose(queryMonitorLogStartTemplate + "Query monitor Query Store disabled - clearing Query Store since trace Enabled Now");
                        newCmd = SqlCommandBuilder.BuildQueryMonitorStopCommandQs(conn, ver, workload.MonitoredServer.CloudProviderId);

                        cmd = SqlCommandBuilder.BuildQueryMonitorTraceCommandWithRestart(
                            conn,
                            ver,
                            queryMonitorConfiguration,
                            refresh.TimeStamp,
                            workload.MonitoredServer.CloudProviderId);
                    }
                    else if (!queryMonitorConfiguration.QueryStoreMonitoringEnabled)  //SQLdm 10.4 (Varun Chopra) Query Monitoring with Query Store
                    {
                        //start -SQLdm 9.0 (Ankit Srivastava) -- defect DE43960 -shifted stop commands with the start ones
                        //stopping trace
                        LOG.Verbose(queryMonitorLogStartTemplate + "Query monitor trace disabled - stopping trace since Extended Events Session Enabled Now");
                        newCmd = SqlCommandBuilder.BuildQueryMonitorStopCommand(conn, ver);
                        //end -SQLdm 9.0 (Ankit Srivastava) -- defect DE43960 -shifted stop commands with the start ones

                        //SQLdm 10.4 (Varun Chopra) Query Monitoring with Query Store - clearing the state if disabled
                        LOG.Verbose(queryMonitorLogStartTemplate + "Query monitor Query Store disabled - clearing Query Store since trace Enabled Now");
                        newCmd = SqlCommandBuilder.BuildQueryMonitorStopCommandQs(conn, ver, workload.MonitoredServer.CloudProviderId);

                        cmd = SqlCommandBuilder.BuildQueryMonitorEXCommandWithRestart(
                            conn,
                            ver,
                            queryMonitorConfiguration,
                            refresh.MonitoredServer.ActiveWaitsConfiguration,
                            refresh.TimeStamp,
                            workload.MonitoredServer.CloudProviderId);
                    }
                    else  // Query Store Monitoring Enabled
                    {
                        // Stop Trace Commands
                        LOG.Verbose(queryMonitorLogStartTemplate + "Query monitor trace disabled - stopping trace since Query Store Enabled Now");
                        newCmd = SqlCommandBuilder.BuildQueryMonitorStopCommand(conn, ver);

                        // Stop extended event session
                        LOG.Verbose(queryMonitorLogStartTemplate + "Query monitor Extended Events Session disabled - stopping Extended Events Session since trace Enabled Now");
                        newCmd = SqlCommandBuilder.BuildQueryMonitorStopCommandEX(
                            conn,
                            ver,
                            workload.MonitoredServer.CloudProviderId);

                        // Query Store Command
                        cmd = SqlCommandBuilder.BuildQueryMonitorQsCommandWithRestart(
                            conn,
                            ver,
                            queryMonitorConfiguration,
                            workload.MonitoredServer.Id,
                            workload.MonitoredServer.CloudProviderId);
                    }
                    //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session - starting the trace OR extended event session accordingly -end
                }

            }

            //Start ---SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session - add the stop commands if exists
            if (!String.IsNullOrEmpty(newCmd.CommandText))
                newCmd.CommandText += cmd.CommandText;
            else
                newCmd = cmd;
            //end ---SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session - add the stop commands if exists

            sdtCollector = new SqlCollector(newCmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(QueryMonitorCallback));
        }

        private bool PopulateRow(XmlNode doc, string eventPrefix, DataRow row, Dictionary<byte[], string> planHandleToQueryPlan)
        {
            var node =
                doc.SelectSingleNode(eventPrefix + @"@name");
            if (node == null)
                return false;
            if (string.Compare(node.InnerText, "query_post_execution_showplan") != 0)
            {
                node = doc.SelectSingleNode(eventPrefix + @"action[@name='database_name']");
                if (node == null)
                    return false;
                else
                {
                    row["DBName"] = Convert.ToString(node.InnerText);
                    bool isExcludeChecked = false;
                    if(queryMonitorConfiguration.AdvancedConfiguration.DatabaseExcludeLike != null)
                    {
                        isExcludeChecked = true;
                        if (AlertFilterHelper.IsLikeValue(
                            queryMonitorConfiguration.AdvancedConfiguration.DatabaseExcludeLike,
                            Convert.ToString(row["DBName"])
                            ))
                            return false;
                    }

                    if (queryMonitorConfiguration.AdvancedConfiguration.DatabaseExcludeMatch != null)
                    {
                        isExcludeChecked = true;
                        if (AlertFilterHelper.IsMatchValue(
                            queryMonitorConfiguration.AdvancedConfiguration.DatabaseExcludeMatch,
                            Convert.ToString(row["DBName"])
                            ))
                            return false;
                    }
                    // Giving priority to exclude
                    if (!isExcludeChecked)
                    {
                        if (queryMonitorConfiguration.AdvancedConfiguration.DatabaseIncludeMatch != null &&
                            !AlertFilterHelper.IsMatchValue(
                            queryMonitorConfiguration.AdvancedConfiguration.DatabaseIncludeMatch,
                            Convert.ToString(row["DBName"])
                            ))
                            return false;
                        if (queryMonitorConfiguration.AdvancedConfiguration.DatabaseIncludeLike != null &&
                            !AlertFilterHelper.IsLikeValue(
                            queryMonitorConfiguration.AdvancedConfiguration.DatabaseIncludeLike,
                            Convert.ToString(row["DBName"])
                            ))
                            return false;
                    }
                }
            
                node = doc.SelectSingleNode(eventPrefix + @"@name");
                if (node != null) row["EventClass"] = ProbeHelpers.GetTraceEventId(node.InnerText);

                node = doc.SelectSingleNode(eventPrefix + @"@timestamp");
                if (node != null) row["CompletionTime"] = Convert.ToDateTime(node.InnerText);

                node = doc.SelectSingleNode(eventPrefix + @"data[@name='duration']");
                if (node != null)
                {
                    TimeSpan duration = TimeSpan.FromMilliseconds(Convert.ToInt64(node.InnerText) / 1000);
                    row["Duration"] = (long)duration.TotalMilliseconds;
                    DateTime statementUTCompletionTime = ProbeHelpers.Truncate(Convert.ToDateTime(row["CompletionTime"]), TimeSpan.TicksPerMillisecond);
                    if (row[StartTimeColumn] != null && string.IsNullOrEmpty(row[StartTimeColumn].ToString()))
                    {
                        row["StartTime"] = statementUTCompletionTime.Subtract(duration);
                    }
                    else
                    {
                        row["StartTime"] = ((DateTime)row[StartTimeColumn]).Subtract(duration);
                    }
                }
                if(cloudProviderId == Constants.MicrosoftAzureManagedInstanceId)
                    node = doc.SelectSingleNode(eventPrefix + @"action[@name='nt_username']");
                else if(cloudProviderId == Constants.MicrosoftAzureId)
                    node = doc.SelectSingleNode(eventPrefix + @"action[@name='username']");
                if (node != null) row["NTUserName"] = Convert.ToString(node.InnerText);

                node = doc.SelectSingleNode(eventPrefix + @"action[@name ='client_hostname']");
                if (node != null) row["HostName"] = Convert.ToString(node.InnerText);

                node = doc.SelectSingleNode(eventPrefix + @"action[@name='client_app_name']");
                if (node != null) row["ApplicationName"] = Convert.ToString(node.InnerText);

                node = doc.SelectSingleNode(eventPrefix + @"action[@name='username']");
                if (node != null) row["LoginName"] = Convert.ToString(node.InnerText);

                node = doc.SelectSingleNode(eventPrefix + @"data[@name='logical_reads']");
                if (node != null) row["Reads"] = Convert.ToInt64(node.InnerText);

                node = doc.SelectSingleNode(eventPrefix + @"data[@name='writes']");
                if (node != null) row["Writes"] = Convert.ToInt64(node.InnerText);

                node = doc.SelectSingleNode(eventPrefix + @"data[@name='cpu_time']");
                if (node != null) row["CPU"] = Convert.ToInt64(node.InnerText);

                node = doc.SelectSingleNode(eventPrefix + @"data[@name='statement']");
                if (node != null) 
                    row["TextData"] = Convert.ToString(node.InnerText);
                else
                {
                    node = doc.SelectSingleNode(eventPrefix + @"data[@name = 'batch_text']");
                    if (node != null)
                        row["TextData"] = Convert.ToString(node.InnerText);
                    else
                    {
                        node = doc.SelectSingleNode(eventPrefix + @"data[@name='user_info']");
                        if (node != null)
                            row["TextData"] = Convert.ToString(node.InnerText);
                    }
                }
                
                node = doc.SelectSingleNode(eventPrefix + @"data[@name='object_name']");
                if (node != null) row["ObjectName"] = Convert.ToString(node.InnerText);

                node = doc.SelectSingleNode(eventPrefix + @"action[@name='session_id']");
                if (node != null) row["SPID"] = Convert.ToInt32(node.InnerText);
             
                node = doc.SelectSingleNode(eventPrefix + @"action[@name='database_id']");
                if (node != null) row["DatabaseID"] = Convert.ToInt32(node.InnerText);

                node = doc.SelectSingleNode(eventPrefix + @"action[@name='plan_handle']");
                if (node != null) row["PlanHandle"] = Convert.ToString(node.InnerText);
            }
            else
            {
                if (queryMonitorConfiguration.CollectQueryPlan)
                {
                    node = doc.SelectSingleNode(eventPrefix + @"action[@name='plan_handle']");
                    if (node != null)                      
                    {
                        var nodeShowPlanXML = doc.SelectSingleNode(eventPrefix + @"data[@name='showplan_xml']/value");
                        if(nodeShowPlanXML != null && !planHandleToQueryPlan.ContainsKey((byte[])Encoding.ASCII.GetBytes(node.InnerText)))
                            planHandleToQueryPlan.Add((byte[]) Encoding.ASCII.GetBytes(node.InnerText), (new XMLData(nodeShowPlanXML.InnerXml)).RawString);
                    }
                }
            }
            return true;
        }

        private void MapQueryPlanToRow(Dictionary<byte[], string> planHandleToQueryPlan, ref DataTable finalTable, ref DataTable tempTable)
        {
            DataRow row = finalTable.NewRow();
            foreach(DataRow tempRow in tempTable.Rows)
            {
                row["EventClass"] = tempRow["EventClass"];
                row["Duration"] = tempRow["Duration"];
                row["DBName"] = tempRow["DBName"];
                row["CompletionTime"] = tempRow["CompletionTime"];
                row["NTUserName"] = tempRow["NTUserName"];
                row["HostName"] = tempRow["HostName"];
                row["ApplicationName"] = tempRow["ApplicationName"];
                row["LoginName"] = tempRow["LoginName"];
                row["Reads"] = tempRow["Reads"];
                row["Writes"] = tempRow["Writes"];
                row["CPU"] = tempRow["CPU"];
                row["TextData"] = tempRow["TextData"];
                row["ObjectName"] = tempRow["ObjectName"];
                row["SPID"] = tempRow["SPID"];
                row["StartTime"] = tempRow["StartTime"];
                row["DatabaseID"] = tempRow["DatabaseID"];
                if (planHandleToQueryPlan.ContainsKey((byte[])Encoding.ASCII.GetBytes(Convert.ToString(tempRow["PlanHandle"]))))
                    row["QueryPlan"] = planHandleToQueryPlan[(byte[])Encoding.ASCII.GetBytes(Convert.ToString(tempRow["PlanHandle"]))];
                finalTable.Rows.Add(row);
                row = finalTable.NewRow();
            }
        }

        private DbDataReader ConvertQueryMonitorDataFromXML(SqlDataReader rawDataReader)
        {
            String dbName = null;
            Dictionary<byte[], string> planHandleToQueryPlan = new Dictionary<byte[], string>(new ProbeHelpers.ByteArrayComparer());
            // Dictionary<byte[], DataRow> planHandleToDataRow = new Dictionary<byte[], DataRow>(new ProbeHelpers.ByteArrayComparer());
            if (!initializedUnfilteredTableFromXML)
            {
                unfilteredTableFromXML = new DataTable("tempTable");
                initializedUnfilteredTableFromXML = true;
                LOG.Debug(queryMonitorLogStartTemplate + "Initializing unfiltered table to store data after parsing XML");
                //Initialize the data table

                unfilteredTableFromXML.Columns.Add("EventClass", Type.GetType("System.Int32"));
                unfilteredTableFromXML.Columns.Add("Duration", Type.GetType("System.Int64"));
                unfilteredTableFromXML.Columns.Add("DBName", Type.GetType("System.String"));
                unfilteredTableFromXML.Columns.Add("CompletionTime", Type.GetType("System.DateTime"));
                unfilteredTableFromXML.Columns.Add("NTUserName", Type.GetType("System.String"));
                unfilteredTableFromXML.Columns.Add("HostName", Type.GetType("System.String"));
                unfilteredTableFromXML.Columns.Add("ApplicationName", Type.GetType("System.String"));
                unfilteredTableFromXML.Columns.Add("LoginName", Type.GetType("System.String"));
                unfilteredTableFromXML.Columns.Add("Reads", Type.GetType("System.Int64"));
                unfilteredTableFromXML.Columns.Add("Writes", Type.GetType("System.Int64"));
                unfilteredTableFromXML.Columns.Add("CPU", Type.GetType("System.Int64"));
                unfilteredTableFromXML.Columns.Add("TextData", Type.GetType("System.String"));
                unfilteredTableFromXML.Columns.Add("ObjectName", Type.GetType("System.String"));
                unfilteredTableFromXML.Columns.Add("SPID", Type.GetType("System.Int32"));
                unfilteredTableFromXML.Columns.Add("StartTime", Type.GetType("System.DateTime"));
                unfilteredTableFromXML.Columns.Add("DatabaseID", Type.GetType("System.Int32"));
                unfilteredTableFromXML.Columns.Add("QueryPlan", Type.GetType("System.String"));
            }
            unfilteredTableFromXML.Clear();
            DataTable tempTable = unfilteredTableFromXML.Clone();
            tempTable.Columns.Add("PlanHandle", Type.GetType("System.String")); //Temporary table for mapping query plans
            if (rawDataReader != null && rawDataReader.Read())
            {
                if (!rawDataReader.IsDBNull(0))
                {
                    dbName = rawDataReader.GetString(0);
                    rawDataReader.NextResult();
                }
            }
            if (rawDataReader != null && dbName != null && rawDataReader.Read())
            {
                try
                {
                    //T xmlStng = rawDataReader.GetTextReader(0);
                    string ringBufferPrefix = @"/RingBufferTarget";
                    string xmlString = rawDataReader.GetString(0);
                    DataRow row;
                    var doc = new XmlDocument();
                    doc.LoadXml(xmlString);
                    
                    if (doc.DocumentElement != null)
                    {
                        var eventPrefix = ringBufferPrefix + @"/event/";
                        var eventNodes = doc.DocumentElement.SelectNodes(eventPrefix.TrimEnd('/'));
                        row = tempTable.NewRow();
                        foreach (XmlNode eventNode in eventNodes)
                        {
                            if (eventNode == null || !PopulateRow(eventNode, null, row, planHandleToQueryPlan))
                            {
                                continue;
                            }
                            //row["DBName"] = dbName;
                            tempTable.Rows.Add(row);
                            row = tempTable.NewRow();
                        }
                        MapQueryPlanToRow(planHandleToQueryPlan, ref unfilteredTableFromXML, ref tempTable);
                    }                   
                }
                catch (Exception e)
                {
                    //
                }
            }
            return unfilteredTableFromXML.CreateDataReader();
        }

        /// <summary>
        /// Define the Query Monitor callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void QueryMonitorCallback(CollectorCompleteEventArgs e)
        {
            if (e.Exception != null && e.Result != Result.Success)
            {
                refresh.QueryMonitorError = e.Exception;
            }
            else
            {
                try
                {
                    LOG.Verbose(queryMonitorLogStartTemplate + "Version = " + refresh.ProductVersion.Major);
                    if (this.queryMonitorConfiguration.QueryStoreMonitoringEnabled &&
                        cloudProviderId == CLOUD_PROVIDER_ID_AZURE)
                    {
                        LOG.Verbose(queryMonitorLogStartTemplate + "Trace and Extended Events disabled. Using Query Store");
                        using (SqlDataReader rd = e.Value as SqlDataReader)
                        {
                            // Interpret Query Store Data
                            InterpretQueryMonitorDataQs(rd, e.Database);
                        }
                    }
                    else if (!queryMonitorConfiguration.TraceMonitoringEnabled &&
                             (cloudProviderId == CLOUD_PROVIDER_ID_AZURE ||
                              cloudProviderId == Constants.MicrosoftAzureManagedInstanceId))
                    {
                        LOG.Verbose(queryMonitorLogStartTemplate + " Interpreting Query Monitor data for Azure.");
                        using (SqlDataReader rd = e.Value as SqlDataReader)
                        {
                            InterpretQueryMonitor(FilterQueryMonitorData(ConvertQueryMonitorDataFromXML(rd)));
                        }
                    }         
                    //SQLdm 10.4 (Varun Chopra) Query Monitoring with Query Store
                    else if ((refresh.ProductVersion.Major >= 13 || cloudProviderId == Constants.MicrosoftAzureManagedInstanceId) && this.queryMonitorConfiguration.QueryStoreMonitoringEnabled) // (SQL Server >= 2016 + Query Store
                    {
                        LOG.Verbose(queryMonitorLogStartTemplate + "Trace and Extended Events disabled. Using Query Store");
                        using (SqlDataReader rd = e.Value as SqlDataReader)
                        {
                            // Interpret Query Store Data
                            InterpretQueryMonitorDataQs(rd);
                        }
                    }
                    else if (refresh.ProductVersion.Major >= 11 && !queryMonitorConfiguration.TraceMonitoringEnabled) // (SQL Server >= 2012 + Extended Events)
                    {
                        LOG.Verbose(queryMonitorLogStartTemplate + "Trace disabled. Using XEReaderAPI for extended events.");
                        if (refresh.CollectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember))
                        {
                            InterpretQueryMonitor(GetQueryMonitorXEReaderData(e));
                        }
                        else
                        {
                            using (SqlDataReader rd = e.Value as SqlDataReader)
                            {
                                InterpretQueryMonitor(FilterQueryMonitorData(rd));
                            }
                        }                      
                    }
                    else if (refresh.ProductVersion.Major > 8) // (SQL Server 2005, 2008 + Extended Events) or (SQL Server >= 2005 + Trace)
                    {
                        LOG.Verbose(queryMonitorLogStartTemplate + "Trace = " + queryMonitorConfiguration.TraceMonitoringEnabled);
                        using (SqlDataReader rd = e.Value as SqlDataReader)
                        {
                            InterpretQueryMonitor(FilterQueryMonitorData(rd));
                        }
                    }
                    else
                    {
                        LOG.Verbose(queryMonitorLogStartTemplate + "Using Trace approach for < 2005 servers.");
                        using (SqlDataReader rd = e.Value as SqlDataReader)
                        {
                            InterpretQueryMonitor(rd);
                        }
                    }

                }
                catch (Exception ex)
                {
                    QueryMonitorFailureDelegate(refresh, ex);
                }
            }

        }

        /// <summary>
        /// Interpret Query Store Data into <see cref="QueryMonitorStatement"/>
        /// </summary>
        /// <param name="dBdataReader">
        /// Datareader after executing <see cref="SqlCommandBuilder.BuildQueryMonitorQsCommand"/>
        /// </param>
        private void InterpretQueryMonitorDataQs(SqlDataReader dBdataReader, string database = null)
        {
            try
            {
                int countInterprettedRows = 0;
                int countEmptyStatements = 0;
                long longestRowText = 0;

                using (LOG.DebugCall("InterpretQueryMonitorDataQs"))
                {
                    //if we got no results just return
                    if (dBdataReader == null)
                    {
                        LOG.Debug(queryMonitorLogStartTemplate + NoDataQs);
                        return;
                    }

                    // set the start time for next query store collection for azure
                    DateTimeOffset maxDateTime = DateTimeOffset.MinValue;
                    string dbName = database;
                    do
                    {
                        while (dBdataReader.Read())
                        {
                            countInterprettedRows++;
                            // initialize statement
                            var statement = new QueryMonitorStatement();

                            // Read TextData
                            var ordinalIndex = dBdataReader.GetOrdinal(TextDataColumn);
                            string decompressedSQLText =
                                dBdataReader.IsDBNull(ordinalIndex) ? null : dBdataReader.GetString(ordinalIndex);
                            // update text data statistics
                            if (String.IsNullOrEmpty(decompressedSQLText))
                            {
                                countEmptyStatements++;
                            }
                            else
                            {
                                if (decompressedSQLText.Length > longestRowText)
                                {
                                    longestRowText = decompressedSQLText.Length;
                                }
                            }
                            // Read ObjectName
                            ordinalIndex = dBdataReader.GetOrdinal(ObjectnameColumn);
                            string objectName = dBdataReader.IsDBNull(ordinalIndex) ? null : dBdataReader.GetString(ordinalIndex);

                            // Set Sql Text
                            if (!string.IsNullOrEmpty(objectName))
                            {
                                // Define with Object Type if provided
                                statement.SqlText = SqlComment + objectName + Environment.NewLine + decompressedSQLText;
                            }
                            else
                            {
                                statement.SqlText = decompressedSQLText;
                            }

                            // Statement Type determined by sys.objects for query store
                            // Read Statement Type
                            ordinalIndex = dBdataReader.GetOrdinal(StatementTypeColumn);
                            var sysObjectType = dBdataReader.IsDBNull(ordinalIndex) ? null : dBdataReader.GetString(ordinalIndex);

                            // Populate Statement Type
                            PopulateStatementTypeQs(sysObjectType, statement);

                            // Read Duration
                            ordinalIndex = dBdataReader.GetOrdinal(DurationColumn);
                            // Set the duration - in this case, we will treat null as zero
                            statement.Duration =
                                TimeSpan.FromMilliseconds(
                                    dBdataReader.IsDBNull(ordinalIndex) ? 0 : (dBdataReader.GetInt64(ordinalIndex) / 1000));

                            // Read Db Name
                            ordinalIndex = dBdataReader.GetOrdinal(DbNameColumn);
                            statement.Database = dBdataReader.IsDBNull(ordinalIndex) ? null : dBdataReader.GetString(ordinalIndex);
                            if (dbName == null)
                            {
                                dbName = statement.Database;
                            }
                            
                            // Read Start Time
                            ordinalIndex = dBdataReader.GetOrdinal(StartTimeColumn);
                            if (!dBdataReader.IsDBNull(ordinalIndex))
                            {
                                var startTime = dBdataReader.GetDateTimeOffset(ordinalIndex);
                                statement.CompletionTime = startTime
                                    .AddMilliseconds(statement.Duration.Value.Milliseconds).DateTime;
                                if (cloudProviderId == Constants.MicrosoftAzureId && startTime > maxDateTime)
                                {
                                    maxDateTime = startTime;
                                }
                            }

                            // Read NTUserName
                            ordinalIndex = dBdataReader.GetOrdinal(UserNameColumn);
                            statement.NtUser = dBdataReader.IsDBNull(ordinalIndex) ? null : dBdataReader.GetString(ordinalIndex);

                            // Read HostName
                            ordinalIndex = dBdataReader.GetOrdinal(HostnameColumn);
                            statement.Client = dBdataReader.IsDBNull(ordinalIndex) ? null : dBdataReader.GetString(ordinalIndex);

                            // Read ApplicationName
                            ordinalIndex = dBdataReader.GetOrdinal(ApplicationNameColumn);
                            statement.Client = dBdataReader.IsDBNull(ordinalIndex) ? null : dBdataReader.GetString(ordinalIndex);

                            // Read LoginName
                            ordinalIndex = dBdataReader.GetOrdinal(LoginNameColumn);
                            statement.Client = dBdataReader.IsDBNull(ordinalIndex) ? null : dBdataReader.GetString(ordinalIndex);

                            // Read IO reads
                            ordinalIndex = dBdataReader.GetOrdinal(IoReadsColumn);
                            if (!dBdataReader.IsDBNull(ordinalIndex))
                            {
                                statement.Reads = dBdataReader.GetInt64(ordinalIndex);
                            }

                            // Read IO Writes
                            ordinalIndex = dBdataReader.GetOrdinal(IoWritesColumn);
                            if (!dBdataReader.IsDBNull(ordinalIndex))
                            {
                                statement.Writes = dBdataReader.GetInt64(ordinalIndex);
                            }

                            // Read CPU
                            ordinalIndex = dBdataReader.GetOrdinal(CpuColummn);
                            if (!dBdataReader.IsDBNull(ordinalIndex))
                            {
                                statement.CpuTime = TimeSpan.FromMilliseconds(dBdataReader.GetInt64(ordinalIndex) / 1000);
                            }

                            // Read SPID
                            statement.Spid = -1;
                            ordinalIndex = dBdataReader.GetOrdinal(SpidColumn);
                            if (!dBdataReader.IsDBNull(ordinalIndex))
                            {
                                statement.Spid = dBdataReader.GetInt16(ordinalIndex);
                            }

                            // Added the new QueryPlan Column value
                            // Read QueryPlan
                            ordinalIndex = dBdataReader.GetOrdinal(QueryPlanColumn);
                            if (!dBdataReader.IsDBNull(ordinalIndex))
                            {
                                statement.QueryPlan = dBdataReader.GetString(ordinalIndex);
                            }

                            // Note: Need to sync Hash Calculations for Query Store and Query Waits, 
                            // Hence we need to include object name for Query Waits as well so that hash remains same
                            if (!String.IsNullOrEmpty(decompressedSQLText))
                            {
                                string signatureHash = SqlParsingHelper.GetSignatureHash(decompressedSQLText);

                                statement.SignatureHash = signatureHash;
                                if (!refresh.QueryMonitorSignatures.ContainsKey(signatureHash))
                                {
                                    string signature = SqlParsingHelper.GetReadableSignature(decompressedSQLText);
                                    byte[] bytes = Serialized<string>.SerializeCompressed<string>(signature);
                                    refresh.QueryMonitorSignatures.Add(signatureHash, bytes);
                                }

                                // For getting estimated query, when actual plan is not available
                                // In case of query store: if only estimated query plan needs to be collected OR if actual plan is rqd to be collected but due to it's unavailability needs to be replaced by estimated one
                                if (queryMonitorConfiguration != null
                                    // To calculate for  only Top X Query Plans
                                    && countInterprettedRows <= this.queryMonitorConfiguration.TopPlanCountFilter
                                    && (queryMonitorConfiguration.CollectEstimatedQueryPlan
                                        || (queryMonitorConfiguration.CollectQueryPlan
                                            && (String.IsNullOrEmpty(statement.QueryPlan)
                                                || String.IsNullOrEmpty(statement.QueryPlan.Trim()))))
                                    && !statement.StatementType.Equals(WorstPerformingStatementType.StoredProcedure))
                                {
                                    // For getting estimated query, when actual plan is not available
                                    this.PopulateEstimatedPlan(statement);
                                }
                                // Note: QS returns signature for stored procedure, allow statements except stored procedure

                                // Add Statement
                                refresh.QueryMonitorStatements.Add(statement);
                            }
                        }
                    } while (dBdataReader.NextResult());

                    // Set Start time for Azure
                    if (cloudProviderId == Constants.MicrosoftAzureId && dbName != null)
                    {
                        PersistenceManager.Instance.SetAzureQsStartTime(workload.Id, dbName, AzureQsType.QueryMonitor,
                            string.Format(Constants.LastStartTimeAzureQsFormat,
                                (maxDateTime != DateTimeOffset.MinValue ? maxDateTime : DateTime.UtcNow).ToString(
                                    "yyyy, MM, dd, HH, mm, ss")));
                    }


                    // Updating the query collected count
                    Statistics.SetQueryMonitorStatements(refresh.QueryMonitorStatements.Count, refresh.MonitoredServer.InstanceName);

                    // Log for debug information
                    LOG.Debug(queryMonitorLogStartTemplate + String.Format("{0} statement texts are null or empty from query store", countEmptyStatements.ToString(CultureInfo.InvariantCulture)));
                    LOG.Debug(queryMonitorLogStartTemplate + String.Format("{0} rows were interpretted from query store", countInterprettedRows.ToString(CultureInfo.InvariantCulture)));
                    LOG.Debug(queryMonitorLogStartTemplate + String.Format("The longest TextData on any row in this filtered resultset from query store was {0} characters", longestRowText.ToString(CultureInfo.InvariantCulture)));

                    // Number of statements read
                    if (refresh.QueryMonitorStatements.Count > 0)
                    {
                        LOG.Verbose(queryMonitorLogStartTemplate + refresh.QueryMonitorStatements.Count + " query monitor statements read from query store.");
                    }
                }
            }
            catch (Exception exception)
            {
                refresh.QueryMonitorError = exception;
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Read query monitor data from query store failed: {0}", exception, true);
                return;
            }
        }

        /// <summary>
        /// Populate Statement Type = Sp or Statement
        /// </summary>
        /// <param name="sysObjectType">type from sys.objects</param>
        /// <param name="statement">input query statement</param>
        private static void PopulateStatementTypeQs(string sysObjectType, QueryMonitorStatement statement)
        {
            // Set the statement type based on BatchConstants.SqlProcedureFilterQs and BatchConstants.SqlStatementsFilterQs
            // Reference Ex and Trc to decide Triggers / Functions and Procedures assigned under StoredProcedure
            switch (sysObjectType)
            {
                // Procedures
                case P_SqlStoredProcedure: // P = SQL Stored Procedure        
                case Pc_AssemblyStoredProcedure: // PC = Assembly(CLR) stored - procedure         
                case Rf_ReplicationFilterProcedure: // RF = Replication - filter - procedure        
                case X_ExtendedStoredProcedure: // X = Extended stored procedure        
                // Triggers           
                case Ta_AssemblyDmlTrigger: //  TA = Assembly(CLR) DML trigger       
                case Tr_SqlDmlTrigger: //  TR = SQL DML trigger       
                // Functions
                case AF_AggregateFunction: //  AF = Aggregate function(CLR)
                case FnScalarFunction: //  FN = SQL scalar function
                case FsScalarFunction: //  FS = Assembly(CLR) scalar - function
                case Ft_AssemblyTableValuedFunction: //  FT = Assembly(CLR) table - valued function
                case If_InlineTableValuedFunction: //  IF = SQL inline table-valued function
                case Tf_TableValuedFunction: //  TF = SQL table - valued - function
                    statement.StatementType = WorstPerformingStatementType.StoredProcedure;
                    break;
                default:
                    statement.StatementType = WorstPerformingStatementType.SingleStatement;
                    break;
            }
            // Note: Bring in Logic from Extended Events to compare text with EXEC / EXECUTE if sys.object.type doesn't works
        }

        /// <summary>
        /// Populate Estimated Plan when actual plan is not available
        /// </summary>
        /// <param name="statement">Statement with Sql text</param>
        private void PopulateEstimatedPlan(QueryMonitorStatement statement)
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            try
            {
                connection = this.OpenConnection(statement.Database);
                transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted, "Estimated_Plan_Transaction");

                SqlCommand cmdToggleShowPlanOn = new SqlCommand("SET SHOWPLAN_XML ON", connection, transaction);
                SqlCommand cmdToggleShowPlanOff = new SqlCommand("SET SHOWPLAN_XML OFF", connection, transaction);

                //string commandUseDatabase = string.Format("USE [{0}]", statement.Database);
                //SqlCommand cmdUseDatabase = new SqlCommand(commandUseDatabase, connection, transaction);
                //cmdUseDatabase.ExecuteScalar();

                cmdToggleShowPlanOn.ExecuteScalar();

                SqlCommand cmd = SqlCommandBuilder.BuildQueryMonitorReadEstimatedPlanCommand(connection, this.refresh.ProductVersion, statement.SqlText);
                if (cmd != null)
                {
                    cmd.Transaction = transaction;
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                statement.QueryPlan = Convert.ToString(dr[0]);
                                statement.IsActualPlan = false;
                            }
                        }
                    }
                    transaction.Commit();
                }
                cmdToggleShowPlanOff.ExecuteScalar();

                connection.Close();
            }
            catch (SqlException sqlException)
            {
                if (transaction != null) transaction.Rollback("Estimated_Plan_Transaction");
                //SQlDM-28022 - Removing the redundant code of closing connection as it was handled in Finally block
                this.LOG.Warn(queryMonitorLogStartTemplate + "Exception occurred in PopulateEstimatedPlan() while getting estimated query plan : ", sqlException);
            }
            catch (Exception exception)
            {
                if (transaction != null) transaction.Rollback("Estimated_Plan_Transaction");
                //SQlDM-28022 - Removing the redundant code of closing connection as it was handled in Finally block
                this.LOG.Warn(this.queryMonitorLogStartTemplate + "Exception occurred in PopulateEstimatedPlan() : ", exception);
            }
            finally
            {
                //SQlDM-28022 - Handling connection object to avoid leakage
                if (connection != null && connection.State != System.Data.ConnectionState.Closed)
                {
                    SqlConnection.ClearPool(connection);
                    connection.Dispose();
                }
            }
        }

        #endregion

        #region Query Monitor Interpretation Methods

        /// <summary>
        /// Reduce the returned data to just the events that have occurred since we last got events
        /// </summary>
        /// <param name="unfilteredDataReader"></param>
        /// <returns></returns>
        private DbDataReader FilterQueryMonitorData(DbDataReader unfilteredDataReader)
        {
            try
            {
                var unfilteredDataSet = new DataSet("unfiltered");
                unfilteredDataSet.Tables.Add(new DataTable("unfilteredDataTable"));
                var unfilteredTable = unfilteredDataSet.Tables["unfilteredDataTable"];

                long lastRowRead = 0;
                long currentRowRead = 0;
                long stopReadingAtRow = 0;
                long rowCounter = 0;
                bool initializedDatatable = false;
                bool goNoFurther = false;
                int countNonNullTextData = 0;
                long longestRowText = 0;

                string flag = "";
                string lastReadFlag = "";
                using (LOG.DebugCall("FilterQueryMonitorData"))
                {
                    Stopwatch filterSW = new Stopwatch();
                    filterSW.Start();
                    if (unfilteredDataReader != null)
                    {
                        if (!(cloudProviderId == CLOUD_PROVIDER_ID_AZURE || cloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId))
                        {
                            while (unfilteredDataReader.Read())
                            {
                                if (!unfilteredDataReader.IsDBNull(0)) flag = unfilteredDataReader.GetString(0);
                                //Flag Format: 'SQLdm1 - ' + host_name() + ' - ' + convert(nvarchar(50),@currenttime,121) 
                                if (flag.Length > 23) lastReadFlag = flag.Substring(0, flag.Length - 23);
                            }

                            LOG.Debug(queryMonitorLogStartTemplate + string.Format("Last user     defined flag was entered at {0}", String.IsNullOrEmpty(flag) ? "NA" : flag));

                            unfilteredDataReader.NextResult();
                        }
                        while (unfilteredDataReader.Read())
                        {
                            //if the event column is not null
                            if (!unfilteredDataReader.IsDBNull(0))
                            {
                                if (initializedDatatable == false)
                                {
                                    LOG.Debug(queryMonitorLogStartTemplate + "Initializing     unfiltered table");

                                    //Initialize the data table
                                    unfilteredTable.Columns.Add("EventClass", Type.GetType("System.Int32"));
                                    unfilteredTable.Columns.Add(DurationColumn, Type.GetType("System.Int64"));
                                    unfilteredTable.Columns.Add(DbNameColumn, Type.GetType("System.String"));
                                    unfilteredTable.Columns.Add("EndTime", Type.GetType("System.DateTime"));
                                    unfilteredTable.Columns.Add(UserNameColumn, Type.GetType("System.String"));
                                    unfilteredTable.Columns.Add(HostnameColumn, Type.GetType("System.String"));
                                    unfilteredTable.Columns.Add(ApplicationNameColumn, Type.GetType("System.String"));
                                    unfilteredTable.Columns.Add(LoginNameColumn, Type.GetType("System.String"));
                                    unfilteredTable.Columns.Add(IoReadsColumn, Type.GetType("System.Int64"));
                                    unfilteredTable.Columns.Add(IoWritesColumn, Type.GetType("System.Int64"));
                                    unfilteredTable.Columns.Add(CpuColummn, Type.GetType("System.Int64"));
                                    unfilteredTable.Columns.Add(TextDataColumn, Type.GetType("System.String"));
                                    unfilteredTable.Columns.Add("sql 1", Type.GetType("System.String"));
                                    unfilteredTable.Columns.Add(SpidColumn, Type.GetType("System.Int32"));
                                    unfilteredTable.Columns.Add(StartTimeColumn, Type.GetType("System.DateTime"));
                                    unfilteredTable.Columns.Add("DatabaseID", Type.GetType("System.Int32"));
                                    unfilteredTable.Columns.Add("RowNumber", Type.GetType("System.Int32"));
                                    //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with     Extended Event Session -- Added the new QueryPlan Column
                                    if (!queryMonitorConfiguration.TraceMonitoringEnabled)
                                        unfilteredTable.Columns.Add(QueryPlanColumn, Type.GetType("System.String"));

                                    DataColumn[] keys = new DataColumn[2];
                                    keys[0] = unfilteredTable.Columns["RowNumber"];
                                    keys[1] = unfilteredTable.Columns["EndTime"];

                                    unfilteredTable.PrimaryKey = keys;
                                    initializedDatatable = true;
                                }

                                DataRow row = unfilteredTable.NewRow();
                                var eventNumber = unfilteredDataReader.GetInt32(0);

                                string textData = "";
                                if (!unfilteredDataReader.IsDBNull(11))
                                {
                                    textData = unfilteredDataReader.GetString(11);

                                    if (textData.Length > longestRowText) longestRowText = textData.Length;

                                    //row["TextData"] = textData; Dont put it in textdata     column. Waste of memory. 
                                    //We already have it an a variable. Cant propogate this   to   multiple rows.
                                    countNonNullTextData++;
                                }

                                rowCounter++;

                                //This was a bad idea.  What if there is no flag in the  first    1000 rows?
                                //Rather just do flood control after the filter
                                //if ((rowCounter - currentRowRead) >=     queryMonitorConfiguration.RecordsPerRefresh)
                                //{
                                //    //save the last current
                                //    lastRowRead = currentRowRead;
                                //    currentRowRead = rowCounter;
                                //    stopReadingAtRow = currentRowRead;
                                //    LOG.Debug(string.Format("Read up to the {0} row  limit",    queryMonitorConfiguration.RecordsPerRefresh.ToString    (CultureInfo.InvariantCulture)));
                                //    goNoFurther = true;
                                //}


                                //if this is a use configured event
                                switch (eventNumber)
                                {
                                    case (int)TraceEvent.UserConfigurable0:
                                        if (!String.IsNullOrEmpty(textData))
                                        {
                                            //if the file has 7.2 flags the flags will be     SQLdm not sqldm1
                                            //If this user config event is from performance     monitor
                                            if (textData.Contains(lastReadFlag) || textData.Contains(lastReadFlag.Replace("SQLdm1", "SQLdm")))
                                            {
                                                //walk up the SQLdm1 rows
                                                lastRowRead = currentRowRead;
                                                currentRowRead = rowCounter;

                                                //stop reading at the flag that was just     inserted
                                                //lastrowread will be the last sqldm\sqldm1     row
                                                if (textData == flag)
                                                {
                                                    stopReadingAtRow = currentRowRead;
                                                    LOG.Debug(queryMonitorLogStartTemplate + string.Format("Flag found at row {0}, Previous flag was at  row    {1}", stopReadingAtRow.ToString(CultureInfo.InvariantCulture), lastRowRead.ToString(CultureInfo.InvariantCulture)));

                                                    goNoFurther = true;
                                                }
                                            }
                                        }
                                        break;
                                    case 12:
                                    case 41:
                                    case 43:
                                    case 45:
                                    case 148:
                                    case 92:
                                    case 93:
                                        row["RowNumber"] = rowCounter;

                                        if (textData.Length > 0)
                                        {
                                            if ((eventNumber == 41 || eventNumber == 45)
                                                &&
                                                (textData.Substring(0, textData.Length > 100 ? 100 : textData.Length).ToUpper().Contains(
                                                    "EXEC ") ||
                                                 textData.Substring(0, textData.Length > 100 ? 100 : textData.Length).ToUpper().Contains(
                                                     "EXECUTE ")))
                                            {
                                                row["EventClass"] = 43;
                                            }
                                            else
                                            {
                                                row["EventClass"] = unfilteredDataReader.GetInt32(0);
                                            }
                                        }
                                        else
                                            row["EventClass"] = unfilteredDataReader.GetInt32(0);

                                        if (!unfilteredDataReader.IsDBNull(1))
                                            row[DurationColumn] = unfilteredDataReader.GetInt64(1) / 1000;
                                        if (!unfilteredDataReader.IsDBNull(2))
                                            row[DbNameColumn] = unfilteredDataReader.GetString(2);
                                        if (!unfilteredDataReader.IsDBNull(3))
                                            row["EndTime"] = unfilteredDataReader.GetDateTime(3);
                                        if (!unfilteredDataReader.IsDBNull(4))
                                            row[UserNameColumn] = unfilteredDataReader.GetString(4);
                                        if (!unfilteredDataReader.IsDBNull(5))
                                            row[HostnameColumn] = unfilteredDataReader.GetString(5);
                                        if (!unfilteredDataReader.IsDBNull(6))
                                            row[ApplicationNameColumn] = unfilteredDataReader.GetString(6);
                                        if (!unfilteredDataReader.IsDBNull(7))
                                            row[LoginNameColumn] = unfilteredDataReader.GetString(7);
                                        if (!unfilteredDataReader.IsDBNull(8)) row[IoReadsColumn] = unfilteredDataReader.GetInt64(8);
                                        if (!unfilteredDataReader.IsDBNull(9)) row[IoWritesColumn] = unfilteredDataReader.GetInt64(9);
                                        if (!unfilteredDataReader.IsDBNull(10)) row[CpuColummn] = unfilteredDataReader.GetInt64(10);

                                        //if an interpreter sql_text limit has been imposed     (CollectionServiceConfiguration.GetCollectionServiceElement    ().MaxQueryMonitorEventSizeKB > -1)
                                        if (!String.IsNullOrEmpty(textData) &&
                                            CollectionServiceConfiguration.GetCollectionServiceElement().
                                                MaxQueryMonitorEventSizeKB > -1)
                                        {
                                            Encoding encoding = Encoding.Default;
                                            var size = new FileSize { Bytes = encoding.GetByteCount(textData) };
                                            if (size.Kilobytes.HasValue &&
                                                size.Kilobytes.Value >
                                                CollectionServiceConfiguration.GetCollectionServiceElement().
                                                    MaxQueryMonitorEventSizeKB)
                                            {
                                                LOG.Debug(queryMonitorLogStartTemplate +
                                                    String.Format(
                                                        "Skipping query monitor statement in filter due to length. Length is {0:N2} KB. App: {1}, Database: {2}",
                                                        size.Kilobytes, row[ApplicationNameColumn] ?? "", row[DbNameColumn] ?? ""));
                                                //row will not be added
                                                continue;
                                            }
                                        }

                                        //if objectname does not contain a null
                                        if (!unfilteredDataReader.IsDBNull(12))
                                        {
                                            row["sql 1"] = SqlComment + unfilteredDataReader.GetString(12) + "\r\n" + textData;
                                        }
                                        else
                                        {
                                            row["sql 1"] = textData;
                                        }

                                        if (!unfilteredDataReader.IsDBNull(13)) row[SpidColumn] = unfilteredDataReader.GetInt32(13);
                                        if (!unfilteredDataReader.IsDBNull(14))
                                            row[StartTimeColumn] = unfilteredDataReader.GetDateTime(14);
                                        if (!unfilteredDataReader.IsDBNull(15))
                                            row["DatabaseID"] = unfilteredDataReader.GetInt32(15);
                                        //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session -- Added the new QueryPlan Column value                                     
                                        if (!queryMonitorConfiguration.TraceMonitoringEnabled && !unfilteredDataReader.IsDBNull(16))
                                            row[QueryPlanColumn] = unfilteredDataReader.GetString(16);

                                        unfilteredTable.Rows.Add(row);

                                        break;
                                }
                                if (goNoFurther)
                                    break;
                            }
                        } //end of loop
                    }
                    LOG.Debug(queryMonitorLogStartTemplate + string.Format("There are {0} unfiltered rows", rowCounter.ToString(CultureInfo.InvariantCulture)));
                    LOG.Debug(queryMonitorLogStartTemplate + string.Format("There is {0} difference between the row numbers", (stopReadingAtRow - lastRowRead).ToString(CultureInfo.InvariantCulture)));
                    LOG.Debug(queryMonitorLogStartTemplate + string.Format("There are {0} rows of non-null textdata", countNonNullTextData.ToString(CultureInfo.InvariantCulture)));
                    LOG.Debug(queryMonitorLogStartTemplate + string.Format("The longest TextData in any one row was {0} characters", longestRowText.ToString(CultureInfo.InvariantCulture)));

                    //This is necessary only because we are .net 2.
                    //if we were on .net 3.5 we could use filteredData = unfilteredTable.Select().CopyToArray
                    // or dr.copytoarray
                    if (!initializedDatatable) return null;

                    //START SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback-- all the rows should be processes since the duplication is handlled in the batch via offset
                    if (!queryMonitorConfiguration.TraceMonitoringEnabled)
                    {
                        lastRowRead = 0;
                        stopReadingAtRow = rowCounter + 1;
                    }
                    //END SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback -- all the rows should be processes since the duplication is handlled in the batch via offset

                    DataRow[] dr =
                        unfilteredTable.Select(string.Format("RowNumber > {0} and RowNumber < {1} and EventClass in (12,41,43,45,148,92,93)",
                                                             lastRowRead.ToString(CultureInfo.InvariantCulture),
                                                             (stopReadingAtRow > lastRowRead
                                                                  ? stopReadingAtRow
                                                                  : currentRowRead).ToString(CultureInfo.InvariantCulture)), "RowNumber desc");

                    //get the schema of the unfiltered table
                    var filteredData = unfilteredTable.Clone();

                    LOG.Verbose(queryMonitorLogStartTemplate + string.Format("There are {0} filtered rows", dr.Length.ToString(CultureInfo.InvariantCulture)));

                    //flood control
                    var maxRow = dr.Length < queryMonitorConfiguration.RecordsPerRefresh
                                     ? dr.Length
                                     : queryMonitorConfiguration.RecordsPerRefresh;

                    //import every row that matches the selection
                    for (var i = 0; i < maxRow; i++) filteredData.ImportRow(dr[i]);

                    filterSW.Stop();

                    TimeSpan ts = filterSW.Elapsed;

                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);

                    LOG.Verbose(queryMonitorLogStartTemplate + string.Format("Filtering took {0}", elapsedTime));

                    //Return the dbdatareader of the filtered data
                    return filteredData.CreateDataReader();
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Interpret Query monitor data returned by filtered, lean batch
        /// </summary>
        /// <param name="dBdataReader"></param>
        private void InterpretQueryMonitor(DbDataReader dBdataReader)
        {
            try
            {
                int countInterprettedRows = 0;
                int counteDeadlocks = 0;
                int countAutoGrows = 0;
                int countEmptyStatements = 0;
                int countFiltered = 0;
                long longestRowText = 0;

                using (LOG.DebugCall("InterpretQueryMonitor"))
                {
                    //if we got no results just return
                    if (dBdataReader == null)
                    {
                        LOG.Debug(queryMonitorLogStartTemplate + "No query monitor data found.");
                        return;
                    }


                    do
                    {
                        Dictionary<long, string> databaseNames = null;

                        while (dBdataReader.Read())
                        {
                            countInterprettedRows++;

                            if (!dBdataReader.IsDBNull(0))
                            {
                                var statement = new QueryMonitorStatement();

                                //if (dBdataReader.GetInt32(0) == (int) TraceEvent.DeadlockGraph)
                                //{
                                //    if (databaseNames == null)
                                //    {
                                //        databaseNames = new Dictionary<long, string>();
                                //        foreach (DatabaseStatistics d in refresh.DbStatistics.Values)
                                //        {
                                //            if (!databaseNames.ContainsKey(d.DatabaseId))
                                //                databaseNames.Add(d.DatabaseId, d.Name);
                                //        }
                                //    }
                                //    counteDeadlocks++;
                                //    ReadDeadlockData(dBdataReader, databaseNames);
                                //    continue;
                                //}

                                //if (dBdataReader.GetInt32(0) == (int) TraceEvent.DataFileAutoGrow ||
                                //    dBdataReader.GetInt32(0) == (int) TraceEvent.LogFileAutoGrow)
                                //{
                                //    countAutoGrows++;
                                //    ReadAutogrowData(dBdataReader);
                                //    continue;
                                //}


                                int eventClass = dBdataReader.GetInt32(0);
                                // Set the statement type based on event class
                                switch (eventClass)
                                {
                                    case (int)TraceEvent.SQLBatchCompleted:
                                        statement.StatementType = WorstPerformingStatementType.Batch;
                                        break;
                                    case (int)TraceEvent.SQLStmtCompleted:
                                        statement.StatementType = WorstPerformingStatementType.SingleStatement;
                                        break;
                                    default:
                                        statement.StatementType = WorstPerformingStatementType.StoredProcedure;
                                        break;
                                }

                                // Set the duration - in this case, we will treat null as zero
                                statement.Duration =
                                    TimeSpan.FromMilliseconds(dBdataReader.IsDBNull(1) ? 0 : dBdataReader.GetInt64(1));

                                statement.Database = dBdataReader.IsDBNull(2) ? null : dBdataReader.GetString(2);

                                if (!dBdataReader.IsDBNull(3))
                                    statement.CompletionTime = dBdataReader.GetDateTime(3);

                                statement.NtUser = dBdataReader.IsDBNull(4) ? null : dBdataReader.GetString(4);
                                statement.Client = dBdataReader.IsDBNull(5) ? null : dBdataReader.GetString(5);

                                statement.AppName = dBdataReader.IsDBNull(6) ? null : dBdataReader.GetString(6);
                                statement.SqlUser = dBdataReader.IsDBNull(7) ? null : dBdataReader.GetString(7);

                                if (!dBdataReader.IsDBNull(8))
                                    statement.Reads = dBdataReader.GetInt64(8);

                                if (!dBdataReader.IsDBNull(9))
                                    statement.Writes = dBdataReader.GetInt64(9);

                                if (!dBdataReader.IsDBNull(10))
                                    statement.CpuTime = TimeSpan.FromMilliseconds(dBdataReader.GetInt64(10));

                                string decompressedSQLText = dBdataReader.IsDBNull(12) ? null : dBdataReader.GetString(12);

                                if (String.IsNullOrEmpty(decompressedSQLText))
                                {
                                    countEmptyStatements++;
                                }
                                else
                                {
                                    if (decompressedSQLText.Length > longestRowText)
                                        longestRowText = decompressedSQLText.Length;
                                }

                                statement.SqlText = decompressedSQLText; //Compress the text

                                if (!dBdataReader.IsDBNull(13))
                                    statement.Spid = dBdataReader.GetInt32(13);

                                //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session -- Added the new QueryPlan Column value
                                if (!queryMonitorConfiguration.TraceMonitoringEnabled && !dBdataReader.IsDBNull(17)) //SQLdm 9.0 (Ankit Srivastava) 
                                    statement.QueryPlan = dBdataReader.GetString(17);

                                if (queryMonitorConfiguration.CpuUsageFilter != null && queryMonitorConfiguration.CpuUsageFilter.Ticks > 0)
                                {
                                    if (statement.CpuTime == null ||
                                        refresh.MonitoredServer.QueryMonitorConfiguration.CpuUsageFilter.
                                            TotalMilliseconds >
                                        statement.CpuTime.Value.TotalMilliseconds)
                                    {
                                        countFiltered++;
                                        continue;
                                    }
                                }

                                if (queryMonitorConfiguration.DurationFilter != null && queryMonitorConfiguration.DurationFilter.Ticks > 0)
                                {
                                    if (statement.Duration == null ||
                                        refresh.MonitoredServer.QueryMonitorConfiguration.DurationFilter.
                                            TotalMilliseconds >
                                        statement.Duration.Value.TotalMilliseconds)
                                    {
                                        countFiltered++;
                                        continue;
                                    }
                                }

                                if (queryMonitorConfiguration.LogicalDiskReads != null && queryMonitorConfiguration.LogicalDiskReads > 0)
                                {
                                    if (statement.Reads == null ||
                                        refresh.MonitoredServer.QueryMonitorConfiguration.LogicalDiskReads >
                                        statement.Reads)
                                    {
                                        countFiltered++;
                                        continue;
                                    }
                                }

                                if (queryMonitorConfiguration.PhysicalDiskWrites != null && queryMonitorConfiguration.PhysicalDiskWrites > 0)
                                {
                                    if (statement.Writes == null ||
                                        refresh.MonitoredServer.QueryMonitorConfiguration.PhysicalDiskWrites >
                                        statement.Writes)
                                    {
                                        countFiltered++;
                                        continue;
                                    }
                                }

                                if (queryMonitorConfiguration.AdvancedConfiguration.ApplicationExcludeLike != null)
                                {
                                    if (AlertFilterHelper.IsLikeValue(
                                        queryMonitorConfiguration.AdvancedConfiguration.
                                            ApplicationExcludeLike,
                                        statement.AppName))
                                    {
                                        countFiltered++;
                                        continue;
                                    }
                                }


                                if (queryMonitorConfiguration.AdvancedConfiguration.ApplicationExcludeMatch != null)
                                {
                                    if (AlertFilterHelper.IsMatchValue(
                                        queryMonitorConfiguration.AdvancedConfiguration.
                                            ApplicationExcludeMatch,
                                        statement.AppName))
                                    {
                                        countFiltered++;
                                        continue;
                                    }
                                }



                                if (queryMonitorConfiguration.AdvancedConfiguration.DatabaseExcludeLike != null)
                                {
                                    if (AlertFilterHelper.IsLikeValue(
                                        queryMonitorConfiguration.AdvancedConfiguration.
                                            DatabaseExcludeLike,
                                        statement.Database))
                                    {
                                        countFiltered++;
                                        continue;
                                    }
                                }



                                if (queryMonitorConfiguration.AdvancedConfiguration.DatabaseExcludeMatch != null)
                                {
                                    if (AlertFilterHelper.IsMatchValue(
                                        queryMonitorConfiguration.AdvancedConfiguration.
                                            DatabaseExcludeMatch,
                                        statement.Database))
                                    {
                                        countFiltered++;
                                        continue;
                                    }
                                }

                                if (queryMonitorConfiguration.AdvancedConfiguration.SqlTextExcludeLike != null)
                                {
                                    if (AlertFilterHelper.IsLikeValue(
                                        queryMonitorConfiguration.AdvancedConfiguration.
                                            SqlTextExcludeLike,
                                        decompressedSQLText))
                                    {
                                        countFiltered++;
                                        continue;
                                    }
                                }



                                if (queryMonitorConfiguration.AdvancedConfiguration.SqlTextExcludeMatch != null)
                                {
                                    if (AlertFilterHelper.IsMatchValue(
                                        queryMonitorConfiguration.AdvancedConfiguration.
                                            SqlTextExcludeMatch,
                                        decompressedSQLText))
                                    {
                                        countFiltered++;
                                        continue;
                                    }
                                }

                                //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters
                                //For App Inclusion Filter
                                byte isAppMatch = 0;
                                byte isAppLike = 0;
                                if (IsValid(queryMonitorConfiguration.AdvancedConfiguration.ApplicationIncludeMatch))
                                {
                                    if (AlertFilterHelper.IsMatchValue(queryMonitorConfiguration.AdvancedConfiguration.
                                                                    ApplicationIncludeMatch, statement.AppName))
                                        isAppMatch = 1;
                                    else
                                        isAppMatch = 2;
                                }


                                if (IsValid(queryMonitorConfiguration.AdvancedConfiguration.ApplicationIncludeLike))
                                {
                                    if (AlertFilterHelper.IsLikeValue(queryMonitorConfiguration.AdvancedConfiguration.
                                                                ApplicationIncludeLike, statement.AppName))
                                        isAppLike = 1;
                                    else
                                        isAppLike = 2;
                                }
                                if (isAppLike > 0 || isAppMatch > 0)
                                {
                                    if (isAppLike == 2 || isAppMatch == 2)
                                    {
                                        countFiltered++;
                                        continue;
                                    }
                                }

                                //for DB Inclusion Filter
                                byte isDbMatch = 0;
                                byte isDbLike = 0;
                                if (IsValid(queryMonitorConfiguration.AdvancedConfiguration.DatabaseIncludeMatch))
                                {
                                    if (AlertFilterHelper.IsMatchValue(queryMonitorConfiguration.AdvancedConfiguration.
                                                                    DatabaseIncludeMatch, statement.Database))
                                        isDbMatch = 1;
                                    else
                                        isDbMatch = 2;
                                }
                                if (IsValid(queryMonitorConfiguration.AdvancedConfiguration.DatabaseIncludeLike))
                                {
                                    if (AlertFilterHelper.IsLikeValue(queryMonitorConfiguration.AdvancedConfiguration.
                                                                DatabaseIncludeLike, statement.Database))
                                        isDbLike = 1;
                                    else
                                        isDbLike = 2;
                                }
                                if (isDbLike > 0 || isDbMatch > 0)
                                {
                                    if (isDbLike == 2 || isDbMatch == 2)
                                    {
                                        countFiltered++;
                                        continue;
                                    }
                                }

                                //For SQL Text Inclusion Filter
                                byte isTextMatch = 0;
                                byte isTextLike = 0;
                                if (IsValid(queryMonitorConfiguration.AdvancedConfiguration.SqlTextIncludeMatch))
                                {
                                    if (AlertFilterHelper.IsMatchValue(queryMonitorConfiguration.AdvancedConfiguration.
                                                                    SqlTextIncludeMatch, decompressedSQLText))
                                        isTextMatch = 1;
                                    else
                                        isTextMatch = 2;
                                }


                                if (IsValid(queryMonitorConfiguration.AdvancedConfiguration.SqlTextIncludeLike))
                                {
                                    if (AlertFilterHelper.IsLikeValue(queryMonitorConfiguration.AdvancedConfiguration.
                                                                SqlTextIncludeLike, decompressedSQLText))
                                        isTextLike = 1;
                                    else
                                        isTextLike = 2;
                                }
                                if (isTextLike > 0 || isTextMatch > 0)
                                {
                                    if (isTextLike == 2 || isTextMatch == 2)
                                    {
                                        countFiltered++;
                                        continue;
                                    }
                                }


                                //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters




                                if (!String.IsNullOrEmpty(decompressedSQLText))
                                {
                                    string signatureHash = string.Empty;
                                    try
                                    {
                                        signatureHash = SqlParsingHelper.GetSignatureHash(decompressedSQLText);
                                    }
                                    catch (Exception exception)
                                    {
                                        this.LOG.Error("Error while calculating Signature Hash for " + decompressedSQLText + " " + exception);
                                        continue;
                                    }

                                    statement.SignatureHash = signatureHash;
                                    if (!refresh.QueryMonitorSignatures.ContainsKey(signatureHash))
                                    {
                                        string signature = SqlParsingHelper.GetReadableSignature(decompressedSQLText);
                                        byte[] bytes = Serialized<string>.SerializeCompressed<string>(signature);
                                        refresh.QueryMonitorSignatures.Add(signatureHash, bytes);
                                    }

                                    //START SQLdm 10.0 (Tarun Sapra) --For getting estimated query, when actual plan is not available
                                    //In case of extended events:if only estimated query plan needs to be collected OR if actual plan is rqd to be collected but due to it's unavailability needs to be replaced by estimated one
                                    if ((queryMonitorConfiguration != null
                                        && countInterprettedRows <= queryMonitorConfiguration.TopPlanCountFilter   //SQLdm 10.4 (Nikhil Bansal) - To Filter only Top X Query Plans
                                        && !queryMonitorConfiguration.TraceMonitoringEnabled
                                        && statement != null
                                        && (queryMonitorConfiguration.CollectEstimatedQueryPlan
                                            || (queryMonitorConfiguration.CollectQueryPlan
                                                && (String.IsNullOrEmpty(statement.QueryPlan) || String.IsNullOrEmpty(statement.QueryPlan.Trim()))))) && statement.StatementType.Equals(WorstPerformingStatementType.Batch))
                                    {
                                        this.PopulateEstimatedPlan(statement);
                                    }
                                    //END SQLdm 10.0 (Tarun Sapra) --For getting estimated query, when actual plan is not available

                                    refresh.QueryMonitorStatements.Add(statement);
                                }
                            }
                        }
                    } while (dBdataReader.NextResult());

                    //[START] SQLdm 10.0 (GK): updating the query collected count
                    Statistics.SetQueryMonitorStatements(refresh.QueryMonitorStatements.Count, refresh.MonitoredServer.InstanceName);
                    //[END] SQLdm 10.0 (GK): updating the query collected count

                    LOG.Debug(queryMonitorLogStartTemplate + String.Format("{0} Autogrows", countAutoGrows.ToString(CultureInfo.InvariantCulture)));
                    LOG.Debug(queryMonitorLogStartTemplate + String.Format("{0} Deadlocks", counteDeadlocks.ToString(CultureInfo.InvariantCulture)));
                    LOG.Debug(queryMonitorLogStartTemplate + String.Format("{0} statement texts are null or empty", countEmptyStatements.ToString(CultureInfo.InvariantCulture)));
                    LOG.Debug(queryMonitorLogStartTemplate + String.Format("{0} rows were interpretted", countInterprettedRows.ToString(CultureInfo.InvariantCulture)));
                    LOG.Debug(queryMonitorLogStartTemplate + String.Format("{0} rows were filtered out (other than empty sqltext)", countFiltered.ToString(CultureInfo.InvariantCulture)));
                    LOG.Debug(queryMonitorLogStartTemplate + String.Format("The longest TextData on any row in this filtered resultset was {0} characters", longestRowText.ToString(CultureInfo.InvariantCulture)));

                    if (refresh.QueryMonitorStatements.Count > 0)
                    {
                        LOG.Verbose(queryMonitorLogStartTemplate + refresh.QueryMonitorStatements.Count + " query monitor statements read.");
                    }
                }
            }
            catch (Exception exception)
            {
                refresh.QueryMonitorError = exception;
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Read query monitor failed: {0}", exception, true);
                return;
            }
        }

        private bool IsValid(string[] filters)
        {
            if (filters != null && filters.Length > 0)
            {
                if (filters.Length == 1)
                {
                    return !String.IsNullOrEmpty(filters[0]);
                }
                else
                    return true;

            }
            else return false;
        }

        private void InterpretQueryMonitor(SqlDataReader dataReader)
        {
            try
            {

                do
                {
                    Dictionary<long, string> databaseNames = null;

                    while (dataReader.Read())
                    {
                        string decompressedSQLText = null;

                        if (!dataReader.IsDBNull(0))
                        {
                            QueryMonitorStatement statement = new QueryMonitorStatement();

                            if (dataReader.GetInt32(0) == (int)TraceEvent.DeadlockGraph)
                            {
                                if (databaseNames == null)
                                {
                                    databaseNames = new Dictionary<long, string>();
                                    foreach (DatabaseStatistics d in refresh.DbStatistics.Values)
                                    {
                                        if (!databaseNames.ContainsKey(d.DatabaseId))
                                            databaseNames.Add(d.DatabaseId, d.Name);
                                    }
                                }
                                ReadDeadlockData(dataReader, databaseNames);
                                continue;
                            }

                            if (dataReader.GetInt32(0) == (int)TraceEvent.DataFileAutoGrow ||
                                dataReader.GetInt32(0) == (int)TraceEvent.LogFileAutoGrow)
                            {
                                ReadAutogrowData(dataReader);
                                continue;
                            }



                            // Set the statement type based on event class
                            switch (dataReader.GetInt32(0))
                            {
                                case (int)TraceEvent.SQLBatchCompleted:
                                    statement.StatementType = WorstPerformingStatementType.Batch;
                                    break;
                                case (int)TraceEvent.SQLStmtCompleted:
                                    statement.StatementType = WorstPerformingStatementType.SingleStatement;
                                    break;
                                default:
                                    statement.StatementType = WorstPerformingStatementType.StoredProcedure;
                                    break;
                            }

                            // Set the duration - in this case, we will treat null as zero
                            statement.Duration =
                                TimeSpan.FromMilliseconds(dataReader.IsDBNull(1) ? 0 : dataReader.GetInt64(1));

                            if (!dataReader.IsDBNull(2))
                                statement.CompletionTime = dataReader.GetDateTime(2);

                            statement.Database = dataReader.IsDBNull(3) ? null : dataReader.GetString(3);
                            statement.NtUser = dataReader.IsDBNull(4) ? null : dataReader.GetString(4);
                            statement.Client = dataReader.IsDBNull(5) ? null : dataReader.GetString(5);
                            statement.AppName = dataReader.IsDBNull(6) ? null : dataReader.GetString(6);
                            statement.SqlUser = dataReader.IsDBNull(7) ? null : dataReader.GetString(7);

                            if (!dataReader.IsDBNull(8))
                                statement.Reads = dataReader.GetInt64(8);

                            if (!dataReader.IsDBNull(9))
                                statement.Writes = dataReader.GetInt64(9);

                            if (!dataReader.IsDBNull(10))
                            {
                                statement.CpuTime = TimeSpan.FromMilliseconds(dataReader.GetInt64(10));
                            }
                            else
                            {
                                statement.CpuTime = TimeSpan.FromMilliseconds(0);
                            }

                            decompressedSQLText = dataReader.IsDBNull(11) ? null : dataReader.GetString(11);

                            if (!String.IsNullOrEmpty(decompressedSQLText) &&
                                CollectionServiceConfiguration.GetCollectionServiceElement().
                                    MaxQueryMonitorEventSizeKB > -1)
                            {
                                Encoding encoding = Encoding.Default;
                                FileSize size = new FileSize();
                                size.Bytes = encoding.GetByteCount(decompressedSQLText);
                                if (size != null && size.Kilobytes.HasValue &&
                                    size.Kilobytes.Value >
                                    CollectionServiceConfiguration.GetCollectionServiceElement().
                                        MaxQueryMonitorEventSizeKB)
                                {
                                    LOG.Warn(queryMonitorLogStartTemplate +
                                        String.Format(
                                            "Skipping query monitor statement due to length. Length is {0:N2} KB. App: {1}, Database: {2}",
                                            size.Kilobytes, statement.AppName, statement.Database));
                                    continue;
                                }
                            }

                            statement.SqlText = decompressedSQLText; //Compress the text

                            if (!dataReader.IsDBNull(12))
                                statement.Spid = dataReader.GetInt32(12);


                            if (queryMonitorConfiguration.CpuUsageFilter != null)
                            {
                                if (statement.CpuTime == null ||
                                    refresh.MonitoredServer.QueryMonitorConfiguration.CpuUsageFilter.
                                        TotalMilliseconds >
                                    statement.CpuTime.Value.TotalMilliseconds)
                                    continue;
                            }

                            if (queryMonitorConfiguration.DurationFilter != null)
                            {
                                if (statement.Duration == null ||
                                    refresh.MonitoredServer.QueryMonitorConfiguration.DurationFilter.
                                        TotalMilliseconds >
                                    statement.Duration.Value.TotalMilliseconds)
                                    continue;
                            }

                            if (queryMonitorConfiguration.LogicalDiskReads != null)
                            {
                                if (statement.Reads == null ||
                                    refresh.MonitoredServer.QueryMonitorConfiguration.LogicalDiskReads >
                                    statement.Reads)
                                    continue;
                            }

                            if (queryMonitorConfiguration.PhysicalDiskWrites != null)
                            {
                                if (statement.Writes == null ||
                                    refresh.MonitoredServer.QueryMonitorConfiguration.PhysicalDiskWrites >
                                    statement.Writes)
                                    continue;
                            }

                            if (queryMonitorConfiguration.AdvancedConfiguration.ApplicationExcludeLike != null)
                            {
                                if (AlertFilterHelper.IsLikeValue(
                                    queryMonitorConfiguration.AdvancedConfiguration.
                                        ApplicationExcludeLike,
                                    statement.AppName))
                                    continue;
                            }


                            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters
                            if (queryMonitorConfiguration.AdvancedConfiguration.ApplicationIncludeLike != null)
                            {
                                if (AlertFilterHelper.IsLikeValue(
                                    queryMonitorConfiguration.AdvancedConfiguration.
                                        ApplicationIncludeLike,
                                    statement.AppName))
                                    continue;
                            }
                            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters

                            if (queryMonitorConfiguration.AdvancedConfiguration.ApplicationExcludeMatch != null)
                            {
                                if (AlertFilterHelper.IsMatchValue(
                                    queryMonitorConfiguration.AdvancedConfiguration.
                                        ApplicationExcludeMatch,
                                    statement.AppName))
                                    continue;
                            }

                            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters
                            if (queryMonitorConfiguration.AdvancedConfiguration.ApplicationIncludeMatch != null)
                            {
                                if (AlertFilterHelper.IsMatchValue(
                                    queryMonitorConfiguration.AdvancedConfiguration.
                                        ApplicationIncludeMatch,
                                    statement.AppName))
                                    continue;
                            }
                            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters

                            if (queryMonitorConfiguration.AdvancedConfiguration.DatabaseExcludeLike != null)
                            {
                                if (AlertFilterHelper.IsLikeValue(
                                    queryMonitorConfiguration.AdvancedConfiguration.
                                        DatabaseExcludeLike,
                                    statement.Database))
                                    continue;
                            }

                            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters
                            if (queryMonitorConfiguration.AdvancedConfiguration.DatabaseIncludeLike != null)
                            {
                                if (AlertFilterHelper.IsLikeValue(
                                    queryMonitorConfiguration.AdvancedConfiguration.
                                        DatabaseIncludeLike,
                                    statement.Database))
                                    continue;
                            }
                            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters

                            if (queryMonitorConfiguration.AdvancedConfiguration.DatabaseExcludeMatch != null)
                            {
                                if (AlertFilterHelper.IsMatchValue(
                                    queryMonitorConfiguration.AdvancedConfiguration.
                                        DatabaseExcludeMatch,
                                    statement.Database))
                                    continue;
                            }

                            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters
                            if (queryMonitorConfiguration.AdvancedConfiguration.DatabaseIncludeMatch != null)
                            {
                                if (AlertFilterHelper.IsMatchValue(
                                    queryMonitorConfiguration.AdvancedConfiguration.
                                        DatabaseIncludeMatch,
                                    statement.Database))
                                    continue;
                            }
                            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters

                            if (queryMonitorConfiguration.AdvancedConfiguration.SqlTextExcludeLike != null)
                            {
                                if (AlertFilterHelper.IsLikeValue(
                                    queryMonitorConfiguration.AdvancedConfiguration.
                                        SqlTextExcludeLike,
                                    decompressedSQLText))
                                    continue;
                            }

                            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters
                            if (queryMonitorConfiguration.AdvancedConfiguration.SqlTextIncludeLike != null)
                            {
                                if (AlertFilterHelper.IsLikeValue(
                                    queryMonitorConfiguration.AdvancedConfiguration.
                                        SqlTextIncludeLike,
                                    decompressedSQLText))
                                    continue;
                            }
                            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters


                            if (queryMonitorConfiguration.AdvancedConfiguration.SqlTextExcludeMatch != null)
                            {
                                if (AlertFilterHelper.IsMatchValue(
                                    queryMonitorConfiguration.AdvancedConfiguration.
                                        SqlTextExcludeMatch,
                                    decompressedSQLText))
                                    continue;
                            }

                            if (queryMonitorConfiguration.AdvancedConfiguration.SqlTextIncludeMatch != null)
                            {
                                if (AlertFilterHelper.IsMatchValue(
                                    queryMonitorConfiguration.AdvancedConfiguration.
                                        SqlTextIncludeMatch,
                                    decompressedSQLText))
                                    continue;
                            }


                            if (!String.IsNullOrEmpty(decompressedSQLText))
                            {
                                string signatureHash = SqlParsingHelper.GetSignatureHash(decompressedSQLText);

                                statement.SignatureHash = signatureHash;
                                if (!refresh.QueryMonitorSignatures.ContainsKey(signatureHash))
                                {
                                    string signature = SqlParsingHelper.GetReadableSignature(decompressedSQLText);
                                    byte[] bytes = Serialized<string>.SerializeCompressed<string>(signature);
                                    refresh.QueryMonitorSignatures.Add(signatureHash, bytes);
                                }
                                refresh.QueryMonitorStatements.Add(statement);
                            }
                        }
                    }
                } while (dataReader.NextResult());

                refresh.QueryMonitorStatements = refresh.QueryMonitorStatements.Take(queryMonitorConfiguration.TopPlanCountFilter).ToList();
                // END - SQLdm 10.4 (Nikhil Bansal) : For Query Monitoring Performance Improvement

                //[START] SQLdm 10.0 (GK): updating the query collected count
                Statistics.SetQueryMonitorStatements(refresh.QueryMonitorStatements.Count, refresh.MonitoredServer.InstanceName);
                //[END] SQLdm 10.0 (GK): updating the query collected count

                if (refresh.QueryMonitorStatements.Count > 0)
                {
                    LOG.Verbose(queryMonitorLogStartTemplate + refresh.QueryMonitorStatements.Count + " query monitor statements read.");
                }
            }
            catch (Exception exception)
            {
                refresh.QueryMonitorError = exception;
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Read query monitor failed: {0}", exception, true);
                return;
            }
        }

        /// <summary>
        /// Lean batch 2005 +
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="databaseNames"></param>
        private void ReadDeadlockData(DbDataReader dataReader, Dictionary<long, string> databaseNames)
        {
            if (!dataReader.IsDBNull(10))
            {
                DateTime? startTime = null;
                if (!dataReader.IsDBNull(12)) startTime = dataReader.GetDateTime(12);

                var deadlockInfo = new DeadlockInfo(dataReader.GetString(10), databaseNames, startTime);
                refresh.Deadlocks.Add(deadlockInfo);
            }
        }

        private void ReadDeadlockData(SqlDataReader dataReader, Dictionary<long, string> databaseNames)
        {
            if (!dataReader.IsDBNull(11))
            {
                DateTime? startTime = null;
                if (!dataReader.IsDBNull(13))
                    startTime = dataReader.GetDateTime(13);
                DeadlockInfo deadlockInfo = new DeadlockInfo(dataReader.GetString(11), databaseNames, startTime);
                refresh.Deadlocks.Add(deadlockInfo);
            }
        }

        /// <summary>
        /// Lean batch 2005+
        /// </summary>
        /// <param name="dataReader"></param>
        private void ReadAutogrowData(DbDataReader dataReader)
        {
            int eventId =
            dataReader.GetInt32(0);
            string dbName = dataReader.IsDBNull(2) ? null : dataReader.GetString(2);

            if (dbName == null || refresh.DbStatistics == null)
                return;

            if (!refresh.DbStatistics.ContainsKey(dbName))
            {
                int dbid = dataReader.IsDBNull(13) ? 0 : dataReader.GetInt32(13);
                refresh.DbStatistics.Add(dbName, new DatabaseStatistics(refresh.ServerName, dbName, dbid));
            }
            switch (eventId)
            {
                case (int)TraceEvent.LogFileAutoGrow:
                    refresh.DbStatistics[dbName].LogAutogrowDetected = true;
                    break;
                case (int)TraceEvent.DataFileAutoGrow:
                    refresh.DbStatistics[dbName].DataAutogrowDetected = true;
                    break;
            }
        }



        private void ReadAutogrowData(SqlDataReader dataReader)
        {
            int eventId =
            dataReader.GetInt32(0);
            string dbName = dataReader.IsDBNull(3) ? null : dataReader.GetString(3);

            if (dbName == null || refresh.DbStatistics == null)
                return;

            if (!refresh.DbStatistics.ContainsKey(dbName))
            {
                int dbid = dataReader.IsDBNull(14) ? 0 : dataReader.GetInt32(14);
                refresh.DbStatistics.Add(dbName, new DatabaseStatistics(refresh.ServerName, dbName, dbid));
            }
            switch (eventId)
            {
                case (int)TraceEvent.LogFileAutoGrow:
                    refresh.DbStatistics[dbName].LogAutogrowDetected = true;
                    break;
                case (int)TraceEvent.DataFileAutoGrow:
                    refresh.DbStatistics[dbName].DataAutogrowDetected = true;
                    break;
            }
        }

        #endregion


        #region XEReader API Support

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Returns events processed using XEReader API.
        /// </summary>
        private DbDataReader GetQueryMonitorXEReaderData(CollectorCompleteEventArgs e)
        {
            using (LOG.DebugCall("GetQueryMonitorXEReaderData"))
            {
                Stopwatch filterSW = new Stopwatch();
                filterSW.Start();

                List<Tuple<string, string>> files = new List<Tuple<string, string>>();
                string flag = "", lastReadFlag = "";

                using (SqlDataReader rd = e.Value as SqlDataReader)
                {
                    LOG.Debug(queryMonitorLogStartTemplate + string.Format("Last user defined flag was entered at {0}", String.IsNullOrEmpty(flag) ? "NA" : flag));
                    if (rd != null)
                    {
                        while (rd.Read())
                        {
                            if (!rd.IsDBNull(0)) flag = rd.GetString(0);
                            //Flag Format: 'SQLdm1 - ' + host_name() + ' - ' + convert(nvarchar(50),@currenttime,121) 
                            if (flag.Length > 23) lastReadFlag = flag.Substring(0, flag.Length - 23);
                        }

                        rd.NextResult(); // Read previous Query Monitor State
                        while (rd.Read())
                        {
                            if (!rd.IsDBNull(0)) dbLastFileName = rd.GetString(0);
                            if (!rd.IsDBNull(1)) dbLastRecordCount = rd.GetInt64(1);
                            break;
                        }
                        LOG.Debug(queryMonitorLogStartTemplate + string.Format("DB FileName = {0} DB RecordCount = {1}", dbLastFileName, dbLastRecordCount));

                        rd.NextResult(); //Read filePath and fileStart
                        while (rd.Read())
                        {
                            //Reading filePath and FileStart from sql batch now to fetch the names of all the files
                            //inside the filePath folder using DirectoryInfo Class. This change is to remove call to 
                            //sys.xp_dirtree SP in collection batch which require sysadmin role to run successfully. 
                            //string filePath = string.Empty;
                            //string fileStart = string.Empty;

                            //if (!rd.IsDBNull(0)) filePath = rd.GetString(0);
                            //if (!rd.IsDBNull(1)) fileStart = rd.GetString(1);
                            //files = GetFilesName(filePath, fileStart, dbLastFileName);
                            //break;


                            string fileName = null;
                            string fileNameInMilliseconds = null;
                            if (!rd.IsDBNull(0)) fileName = rd.GetString(0);
                            if (!rd.IsDBNull(1)) fileNameInMilliseconds = rd.GetString(1);
                            if (fileName != null && fileNameInMilliseconds != null)
                                files.Add(Tuple.Create(fileName, fileNameInMilliseconds));
                        }
                    }
                    LOG.Debug(queryMonitorLogStartTemplate + string.Format("Files to process - {0}", string.Join(",", files.Select(f => f.Item1 + ":" + f.Item2))));
                }
                var table = ReadQueryMonitorData(files, flag, lastReadFlag, dbLastFileName, dbLastRecordCount);
                filterSW.Stop();
                LOG.Debug(string.Format(queryMonitorLogStartTemplate + "Reading and Filtering took {0} milliseconds.", filterSW.ElapsedMilliseconds));
                return table;
            }
        }


        /// <summary>
        /// SqlDM 10.3 (Tushar)--Returns list of QM extended events files to be read from the path given as input.
        /// </summary>
        private List<Tuple<string, string>> GetFilesName(string filePath, string fileStart, string lastFileName)
        {
            //Creating an empty list of tuple since ReadQueryMonitorData method does not handle null for list of files.
            List<Tuple<string, string>> files = new List<Tuple<string, string>>();

            //If fileStart or filePath is empty then we need to return empty list of tuple. 
            if (string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(fileStart))
                return files;

            try
            {
                string fileNameInMilliSeconds = string.Empty;
                DirectoryInfo dir = new DirectoryInfo(filePath);
                FileInfo[] fis = dir.GetFiles();
                foreach (FileInfo fi in fis)
                {
                    //Check to make sure only QM files are processed since log folder conatains lots of other type of files also.
                    if (Regex.IsMatch(fi.FullName, fileStart + ".*\\.xel"))
                    {
                        fileNameInMilliSeconds = fi.FullName.Substring(fi.FullName.LastIndexOf('_') + 1);
                        if (fileNameInMilliSeconds.CompareTo(dbLastFileName) >= 0)
                        {
                            //File passed the name check and timestamp comparision also, so adding it to files to be read.
                            files.Add(Tuple.Create(fi.FullName, fileNameInMilliSeconds));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LOG.Error(queryMonitorLogStartTemplate + " Exception encountered while fetching QM file names form directory : + " + filePath + ex.Message + ex.StackTrace);
                return files;
            }
            return files;
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Reads QM data and returns filtered datatable.
        /// </summary>
        private DbDataReader ReadQueryMonitorData(List<Tuple<string, string>> files, string flag, string lastReadFlag,
            string dbLastFileName, long dbLastRecordCount)
        {
            using (LOG.DebugCall("ReadQueryMonitorData"))
            {
                //string currentFileName = null;
                DataTable unfilteredTable = null;
                Tuple<Regex[], string[], Regex[], string[]> queryFilters = GetQueryMatchLikeFilters(refresh.MonitoredServer.QueryMonitorConfiguration);
                Dictionary<byte[], string> planHandleToQueryPlan = new Dictionary<byte[], string>(new ProbeHelpers.ByteArrayComparer());
                PublishedEventField eventField = null;
                PublishedAction action = null;
                long lastRowRead = 0, currentRowRead = 0, stopReadingAtRow = 0, rowCounter = 0, longestRowText = 0,
                    countNonNullTextData = 0, currentRecordCount = 0;
                lastFileRecordCount = 0;
                bool initializedDatatable = false, goNoFurther = false, shouldSkipEvents = dbLastFileName != null;

                for (int fileIndex = 0; fileIndex < files.Count; fileIndex++)
                {
                    if (goNoFurther)    // Stop processing if flag set to true in inner loop.
                        break;

                    currentFileName = files[fileIndex].Item2;
                    List<PublishedEvent> filteredEvents = new List<PublishedEvent>();
                    Tuple<string, string> file = files[fileIndex];
                    using (QueryableXEventData events = new QueryableXEventData(connectionInfo.ConnectionString, file.Item1,
                        EventStreamSourceOptions.EventFile, EventStreamCacheOptions.DoNotCache))
                    {
                        foreach (PublishedEvent eventData in events)
                        {
                            if (fileIndex == files.Count - 1) // For last file, track events.
                                lastFileRecordCount++;

                            if (shouldSkipEvents)   // Need to skip such events
                            {
                                int comparison = file.Item2.CompareTo(dbLastFileName);
                                if (comparison < 0)
                                    continue;

                                if (comparison == 0 && currentRecordCount <= dbLastRecordCount)
                                {
                                    currentRecordCount++;
                                    continue;
                                }

                                shouldSkipEvents = false;
                            }

                            int traceEventId = ProbeHelpers.GetTraceEventId(eventData.Name);    // Map event name to event id
                            if (traceEventId == 98)    // query_post_execution_showplan
                            {
                                if (eventData.Actions.TryGetValue("plan_handle", out action) && action.Value != null && action.Value is byte[]
                                && !planHandleToQueryPlan.ContainsKey((byte[])action.Value)
                                && eventData.Fields.TryGetValue("showplan_xml", out eventField) && eventField.Value != null)
                                {
                                    planHandleToQueryPlan.Add((byte[])action.Value, ((XMLData)eventField.Value).RawString);
                                }

                                continue;
                            }

                            if (!FilterExtendedEvent(queryFilters.Item1, queryFilters.Item2,
                                queryFilters.Item3, queryFilters.Item4, eventData))  // Process include/exlcude like/match filters
                                continue;

                            filteredEvents.Add(eventData);
                        }

                        foreach (PublishedEvent eventData in filteredEvents)  // Process on filtered events
                        {
                            if (string.IsNullOrWhiteSpace(eventData.Name))
                                continue;

                            if (initializedDatatable == false)
                            {
                                LOG.Debug(queryMonitorLogStartTemplate + "Initializing unfiltered table");
                                unfilteredTable = ConstructUnfilteredDataTable();
                                initializedDatatable = true;
                            }

                            DataRow row = unfilteredTable.NewRow();
                            var eventNumber = ProbeHelpers.GetTraceEventId(eventData.Name);
                            string textData = GetTextData(eventData);
                            if (textData != null)
                            {
                                if (textData.Length > longestRowText) longestRowText = textData.Length;

                                //row["TextData"] = textData; Dont put it in textdata column. Waste of memory. 
                                //We already have it an a variable. Cant propogate this to multiple rows.
                                countNonNullTextData++;
                            }

                            rowCounter++;

                            //if this is a use configured event
                            switch (eventNumber)
                            {
                                case (int)TraceEvent.UserConfigurable0:
                                    if (!String.IsNullOrEmpty(textData))
                                    {
                                        //if the file has 7.2 flags the flags will be SQLdm not sqldm1
                                        //If this user config event is from performance monitor
                                        if (textData.Contains(lastReadFlag) || textData.Contains(lastReadFlag.Replace("SQLdm1", "SQLdm")))
                                        {
                                            //walk up the SQLdm1 rows
                                            lastRowRead = currentRowRead;
                                            currentRowRead = rowCounter;

                                            //stop reading at the flag that was just inserted
                                            //lastrowread will be the last sqldm\sqldm1 row
                                            if (textData == flag)
                                            {
                                                stopReadingAtRow = currentRowRead;
                                                LOG.Debug(queryMonitorLogStartTemplate + string.Format("Query Monitor - Flag found at row {0}, Previous flag was at row {1}",
                                                    stopReadingAtRow.ToString(CultureInfo.InvariantCulture), lastRowRead.ToString(CultureInfo.InvariantCulture)));

                                                goNoFurther = true;
                                            }
                                        }
                                    }
                                    break;
                                case 12:
                                case 41:
                                case 43:
                                case 45:
                                case 148:
                                case 92:
                                case 93:
                                    row["RowNumber"] = rowCounter;

                                    if (textData.Length > 0)
                                    {
                                        if ((eventNumber == 41 || eventNumber == 45)
                                            && (textData.Substring(0, textData.Length > 100 ? 100 : textData.Length).ToUpper().Contains(
                                                "EXEC ") || textData.Substring(0, textData.Length > 100 ? 100 : textData.Length).ToUpper().Contains(
                                                    "EXECUTE ")))
                                        {
                                            row["EventClass"] = 43;
                                        }
                                        else
                                        {
                                            row["EventClass"] = eventNumber;
                                        }
                                    }
                                    else
                                        row["EventClass"] = eventNumber;

                                    //if an interpreter sql_text limit has been imposed (CollectionServiceConfiguration.GetCollectionServiceElement().MaxQueryMonitorEventSizeKB > -1)
                                    if (!String.IsNullOrEmpty(textData) && CollectionServiceConfiguration.GetCollectionServiceElement().
                                                MaxQueryMonitorEventSizeKB > -1)
                                    {
                                        Encoding encoding = Encoding.Default;
                                        var size = new FileSize { Bytes = encoding.GetByteCount(textData) };
                                        if (size.Kilobytes.HasValue && size.Kilobytes.Value > CollectionServiceConfiguration.GetCollectionServiceElement().
                                                MaxQueryMonitorEventSizeKB)
                                        {
                                            LOG.Debug(queryMonitorLogStartTemplate + String.Format("Query Monitor - Skipping query monitor statement in filter due to length. Length is {0:N2} KB. App: {1}, Database: {2}",
                                                    size.Kilobytes, row[ApplicationNameColumn] ?? "", row[DbNameColumn] ?? ""));
                                            //row will not be added
                                            continue;
                                        }
                                    }

                                    AddDataToRow(row, planHandleToQueryPlan, textData, eventData, eventField, action);
                                    unfilteredTable.Rows.Add(row);
                                    break;
                            }

                            if (goNoFurther)
                                break;
                        }
                    }

                } //end of loop

                // If processed new data, save last read file name to DB.
                //if (currentFileName != null && (currentFileName != dbLastFileName || lastFileRecordCount > dbLastRecordCount))
                //{
                //    SaveQueryMonitorState(currentFileName, lastFileRecordCount);
                //}
                //else
                //{
                //    LOG.Info(queryMonitorLogStartTemplate + "Skipping Query Monitor State Save Operation - No changes in state.");
                //    LOG.Info(string.Format(queryMonitorLogStartTemplate + "DB FileName = {0} CurrentFileName = {1} DB RecordCount = {2} Current RecordCount = {3}", dbLastFileName,
                //        currentFileName, dbLastRecordCount, lastFileRecordCount));
                //}

                LOG.DebugFormat(queryMonitorLogStartTemplate + "There are {0} unfiltered rows", rowCounter.ToString(CultureInfo.InvariantCulture));
                LOG.DebugFormat(queryMonitorLogStartTemplate + "There is {0} difference between the row numbers", (stopReadingAtRow - lastRowRead).ToString(CultureInfo.InvariantCulture));
                LOG.DebugFormat(queryMonitorLogStartTemplate + "There are {0} rows of non-null textdata", countNonNullTextData.ToString(CultureInfo.InvariantCulture));
                LOG.DebugFormat(queryMonitorLogStartTemplate + "The longest TextData in any one row was {0} characters", longestRowText.ToString(CultureInfo.InvariantCulture));

                if (!initializedDatatable)
                {
                    LOG.Debug(queryMonitorLogStartTemplate + "DataTable was not initialized.");
                    return null;
                }

                //START SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback-- all the rows should be processes since the duplication is handlled in the batch via offset
                if (!queryMonitorConfiguration.TraceMonitoringEnabled)
                {
                    lastRowRead = 0;
                    stopReadingAtRow = rowCounter + 1;
                }
                //END SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback -- all the rows should be processes since the duplication is handlled in the batch via offset

                DataRow[] dr = unfilteredTable.Select(string.Format("RowNumber > {0} and RowNumber < {1} and EventClass in (12,41,43,45,148,92,93)",
                                                         lastRowRead.ToString(CultureInfo.InvariantCulture),
                                                         (stopReadingAtRow > lastRowRead
                                                              ? stopReadingAtRow
                                                              : currentRowRead).ToString(CultureInfo.InvariantCulture)), "RowNumber desc");

                //flood control
                if (dr.Length > queryMonitorConfiguration.RecordsPerRefresh)
                    dr = dr.Take(queryMonitorConfiguration.RecordsPerRefresh).ToArray();

                //SQLdm 10.4 (Anshika Sharma) - sort by topPlanCategoryFilter and select top topPlanCountFilter
                Dictionary<int, string> topPlanCategoryFilter = new Dictionary<int, string>();
                topPlanCategoryFilter.Add(0, DurationColumn);
                topPlanCategoryFilter.Add(1, IoReadsColumn);
                topPlanCategoryFilter.Add(2, CpuColummn);
                topPlanCategoryFilter.Add(3, IoWritesColumn);

                // 10.5 - Handle condition if no data is present
                if (0 < unfilteredTable.AsEnumerable().OrderByDescending(row => row.Field<System.Int64?>(topPlanCategoryFilter[queryMonitorConfiguration.TopPlanCategoryFilter])).Count())
                {
                    DataTable unfilteredData = unfilteredTable.AsEnumerable().OrderByDescending(row => row.Field<System.Int64?>(topPlanCategoryFilter[queryMonitorConfiguration.TopPlanCategoryFilter])).CopyToDataTable();
                    // if data was filtered, pick it, otherwise continue with original table.
                    DataTable filteredData = (dr.Length != unfilteredTable.Rows.Count) ?
                        (dr.Length > 0 ?
                            dr.AsEnumerable().OrderByDescending(row => row.Field<System.Int64?>(topPlanCategoryFilter[queryMonitorConfiguration.TopPlanCategoryFilter])).CopyToDataTable()
                            : unfilteredData)
                        : unfilteredData;

                    //START - SQLDM 10.4 (Nikhil Bansal) - TOP X Query Plan Filter - Add the plan for Top X Queries and set the rest to null
                    int numOfQueries = 0;
                    foreach (DataRow row in filteredData.Rows)
                    {
                        numOfQueries++;
                        if (numOfQueries > queryMonitorConfiguration.TopPlanCountFilter)
                        {
                            row[QueryPlanColumn] = String.Empty;
                            continue;
                        }
                        String planHandleString = row.Field<System.String>(QueryPlanColumn);
                        if (String.IsNullOrEmpty(planHandleString))
                            continue;

                        byte[] planHandle = Convert.FromBase64String(planHandleString);

                        if (planHandleToQueryPlan != null)
                        {
                            if (!queryMonitorConfiguration.TraceMonitoringEnabled && planHandle != null && planHandleToQueryPlan.ContainsKey(planHandle))
                            {
                                String plan = String.Empty;
                                planHandleToQueryPlan.TryGetValue(planHandle, out plan);
                                row[QueryPlanColumn] = plan;
                            }
                        }
                    }
                    //END - SQLDM 10.4 (Nikhil Bansal) - TOP X Query Plan Filter - Add the plan for Top X Queries

                    //Return the dbdatareader of the filtered data
                    return filteredData.CreateDataReader();
                }
                else
                {
                    LOG.Debug(queryMonitorLogStartTemplate + "The table contains no rows, hence returning null.");
                    return null;
                }
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Returns Exclude Like, Exclude Match, Include Like, Include Match
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private Tuple<Regex[], string[], Regex[], string[]> GetQueryMatchLikeFilters(QueryMonitorConfiguration config)
        {
            return Tuple.Create(
                config.AdvancedConfiguration.SqlTextExcludeLike != null ? config.AdvancedConfiguration.SqlTextExcludeLike.ToList()
                .Select(s => new Regex(ProbeHelpers.GetRegexFromLikeExpression(s))).ToArray() : null,
                config.AdvancedConfiguration.SqlTextExcludeMatch,
                config.AdvancedConfiguration.SqlTextIncludeLike != null ? config.AdvancedConfiguration.SqlTextIncludeLike.ToList()
                .Select(s => new Regex(ProbeHelpers.GetRegexFromLikeExpression(s))).ToArray() : null,
                config.AdvancedConfiguration.SqlTextIncludeMatch);
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Tests if the event should be included or excluded based on query monitor configuration.
        /// </summary>
        private bool FilterExtendedEvent(Regex[] sqlTextExcludeLike, string[] sqlTextExcludeMatch,
            Regex[] sqlTextIncludeLike, string[] sqlTextIncludeMatch, PublishedEvent eventData)
        {
            PublishedEventField eventField;
            string sql_statement = null, batch_text = null;
            if (eventData.Fields.TryGetValue("statement", out eventField) && eventField.Value != null)
                sql_statement = GetString(eventField);
            else if (eventData.Fields.TryGetValue("batch_text", out eventField) && eventField.Value != null)
                batch_text = GetString(eventField);

            bool isFilteringPerformed = false;
            string toMatch = sql_statement ?? batch_text;
            if (toMatch == null) // For User events
                return true;
            if (sqlTextExcludeLike != null && sqlTextExcludeLike.Length > 0)
            {
                isFilteringPerformed = true;
                foreach (Regex r in sqlTextExcludeLike)
                {
                    if (r.IsMatch(toMatch))
                        return false;
                }
            }

            if (sqlTextExcludeMatch != null && sqlTextExcludeMatch.Length > 0)
            {
                isFilteringPerformed = true;
                foreach (string filterString in sqlTextExcludeMatch)
                {
                    if (filterString != null && filterString.Length > 0 && toMatch.Equals(filterString))
                        return false;
                }
            }
            if (sqlTextIncludeMatch != null && sqlTextIncludeMatch.Length > 0)
            {
                isFilteringPerformed = true;
                foreach (string filterString in sqlTextIncludeMatch)
                {
                    if (filterString != null && filterString.Length > 0 && toMatch.Equals(filterString))
                        return true;
                }
            }
            if (sqlTextIncludeLike != null && sqlTextIncludeLike.Length > 0)
            {
                isFilteringPerformed = true;
                foreach (Regex r in sqlTextIncludeLike)
                {
                    if (r.IsMatch(toMatch))
                        return true;
                }
            }

            return !isFilteringPerformed;
        }

        /// <summary>
        /// Add Data to Activity Monitor Row
        /// </summary>
        /// <param name="row">Set data to Row</param>
        /// <param name="textData">Query Text</param>
        /// <param name="eventData">Event Related Data</param>
        private void AddDataToActivityMonitorRow(DataRow row, string textData, PublishedEvent eventData)
        {
            PublishedEventField eventField = null;
            PublishedAction action = null;
            DateTime statementUTCompletionTime = ProbeHelpers.Truncate(eventData.Timestamp.DateTime, TimeSpan.TicksPerMillisecond);
            if (row["EndTime"] != null && string.IsNullOrEmpty(row["EndTime"].ToString()))
                row["EndTime"] = statementUTCompletionTime; // Completion time

            //if objectname does not contain a null
            if (eventData.Actions.TryGetValue("object_name", out action) && action.Value != null)
            {
                row["sql 1"] = SqlComment + (string)action.Value + "\r\n" + textData;
            }
            else
            {
                row["sql 1"] = textData;
            }

            if (eventData.Fields.TryGetValue("duration", out eventField) && eventField.Value != null)
            {
                // Set the duration - in this case, we will treat null as zero
                TimeSpan duration = TimeSpan.FromMilliseconds(Convert.ToInt64(eventField.Value) / 1000);
                row[DurationColumn] = (long)duration.TotalMilliseconds;
                if (row[StartTimeColumn] != null && string.IsNullOrEmpty(row[StartTimeColumn].ToString()))
                {
                    row[StartTimeColumn] = statementUTCompletionTime.Subtract(duration);
                }
                else
                {
                    row[StartTimeColumn] = ((DateTime)row[StartTimeColumn]).Subtract(duration);
                }
            }

            if (eventData.Actions.TryGetValue("database_name", out action) && action.Value != null)
                row[DbNameColumn] = (string)action.Value;

            if (eventData.Actions.TryGetValue("nt_username", out action) && action.Value != null)
                row[UserNameColumn] = (string)action.Value;

            if (eventData.Actions.TryGetValue("client_hostname", out action) && action.Value != null)
                row[HostnameColumn] = (string)action.Value;

            if (eventData.Actions.TryGetValue("client_app_name", out action) && action.Value != null)
                row[ApplicationNameColumn] = (string)action.Value;

            if (eventData.Actions.TryGetValue("username", out action) && action.Value != null)
                row[LoginNameColumn] = (string)action.Value;

            if (eventData.Actions.TryGetValue("session_id", out action) && action.Value != null)
                row[SpidColumn] = Convert.ToInt32(action.Value);

            if (eventData.Actions.TryGetValue("database_id", out action) && action.Value != null)
                row["DatabaseID"] = Convert.ToInt32(action.Value);
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Processes and adds event to datatable.
        /// </summary>
        private void AddDataToRow(DataRow row, Dictionary<byte[], string> planHandleToQueryPlan, string textData,
            PublishedEvent eventData, PublishedEventField eventField, PublishedAction action)
        {
            DateTime statementUTCompletionTime = ProbeHelpers.Truncate(eventData.Timestamp.DateTime, TimeSpan.TicksPerMillisecond);
            row["EndTime"] = statementUTCompletionTime; // Completion time

            //if objectname does not contain a null
            if (eventData.Actions.TryGetValue("object_name", out action) && action.Value != null)
            {
                row["sql 1"] = SqlComment + (string)action.Value + "\r\n" + textData;
            }
            else
            {
                row["sql 1"] = textData;
            }

            if (eventData.Fields.TryGetValue("duration", out eventField) && eventField.Value != null)
            {
                // Set the duration - in this case, we will treat null as zero
                TimeSpan duration = TimeSpan.FromMilliseconds(Convert.ToInt64(eventField.Value) / 1000);
                row[DurationColumn] = (long)duration.TotalMilliseconds;
                row[StartTimeColumn] = statementUTCompletionTime.Subtract(duration);
            }

            if (eventData.Actions.TryGetValue("database_name", out action) && action.Value != null)
                row[DbNameColumn] = (string)action.Value;

            if (eventData.Actions.TryGetValue("nt_username", out action) && action.Value != null)
                row[UserNameColumn] = (string)action.Value;

            if (eventData.Actions.TryGetValue("client_hostname", out action) && action.Value != null)
                row[HostnameColumn] = (string)action.Value;

            if (eventData.Actions.TryGetValue("client_app_name", out action) && action.Value != null)
                row[ApplicationNameColumn] = (string)action.Value;

            if (eventData.Actions.TryGetValue("username", out action) && action.Value != null)
                row[LoginNameColumn] = (string)action.Value;

            if (eventData.Fields.TryGetValue("logical_reads", out eventField) && eventField.Value != null)
                row[IoReadsColumn] = Convert.ToInt64(eventField.Value);

            if (eventData.Fields.TryGetValue("writes", out eventField) && eventField.Value != null)
                row[IoWritesColumn] = Convert.ToInt64(eventField.Value);

            if (eventData.Fields.TryGetValue("cpu_time", out eventField) && eventField.Value != null)
                row[CpuColummn] = Convert.ToInt64(eventField.Value) / 1000;

            if (eventData.Actions.TryGetValue("session_id", out action) && action.Value != null)
                row[SpidColumn] = Convert.ToInt32(action.Value);

            if (eventData.Actions.TryGetValue("database_id", out action) && action.Value != null)
                row["DatabaseID"] = Convert.ToInt32(action.Value);


            //SQLDM 10.4 (Nikhil Bansal) - Query Plan will be set after all the queries have been fetched, for now store the plan_handle in the QueryPlan column
            if (planHandleToQueryPlan != null)
            {
                if (!queryMonitorConfiguration.TraceMonitoringEnabled &&
                 eventData.Actions.TryGetValue("plan_handle", out action) && action.Value != null && action.Value is byte[]
                 && planHandleToQueryPlan.ContainsKey((byte[])action.Value))
                {
                    //row["QueryPlan"] = planHandleToQueryPlan[((byte[])action.Value)];
                    row[QueryPlanColumn] = Convert.ToBase64String((byte[])action.Value);
                }
            }

        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Returns text data from event.
        /// </summary>
        private string GetTextData(PublishedEvent eventData)
        {
            PublishedEventField eventField;
            if (eventData.Fields.TryGetValue("statement", out eventField) && eventField.Value != null)
                return GetString(eventField).Replace("\r\n", "\n");
            if (eventData.Fields.TryGetValue("batch_text", out eventField) && eventField.Value != null)
                return GetString(eventField).Replace("\r\n", "\n");
            if (eventData.Fields.TryGetValue("user_info", out eventField) && eventField.Value != null)
                return GetString(eventField);
            return null;

        }

        /// <summary>
        /// Get Text Data for Activity Monitor
        /// </summary>
        /// <param name="eventData">Event Data</param>
        /// <returns>Strinv Value for Field related with Activity Monitor</returns>
        /// <remarks>
        /// DM 10.3 SQLDM-28752 - Sessions and Blocking Data
        /// </remarks>
        private string GetTextDataForActivityMonitor(PublishedEvent eventData)
        {
            PublishedEventField eventField;
            if (eventData.Fields.TryGetValue("statement", out eventField) && eventField.Value != null)
                return GetString(eventField).Replace("\r\n", "\n");
            if (eventData.Fields.TryGetValue("batch_text", out eventField) && eventField.Value != null)
                return GetString(eventField).Replace("\r\n", "\n");
            if (eventData.Fields.TryGetValue("user_info", out eventField) && eventField.Value != null)
                return GetString(eventField);
            // For Blocked XML Information
            if (eventData.Fields.TryGetValue("blocked_process", out eventField) && eventField.Value != null)
                return GetString(eventField).Replace("\r\n", "\n");
            // For Deadlock XML Information
            if (eventData.Fields.TryGetValue("xml_report", out eventField) && eventField.Value != null)
                return GetString(eventField).Replace("\r\n", "\n");
            // For Size Autogrow Information
            if (eventData.Fields.TryGetValue("sql_text", out eventField) && eventField.Value != null)
                return GetString(eventField).Replace("\r\n", "\n");
            return null;

        }

        /// <summary>
        ///  SqlDM 10.2 (Anshul Aggarwal) - Extarct string from eventField based on type of Value.
        /// </summary>
        private string GetString(PublishedEventField eventField)
        {
            if (eventField.Value is string)
                return eventField.Value.ToString();
            else if (eventField.Value is XMLData)
                return ((XMLData)eventField.Value).RawString;
            return string.Empty;
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Constructs DataTable for query monitor processing.
        /// </summary>
        private DataTable ConstructUnfilteredDataTable()
        {
            //Initialize the data table
            var unfilteredTable = new DataTable("unfilteredDataTable");
            unfilteredTable.Columns.Add("EventClass", Type.GetType("System.Int32"));
            unfilteredTable.Columns.Add(DurationColumn, Type.GetType("System.Int64"));
            unfilteredTable.Columns.Add(DbNameColumn, Type.GetType("System.String"));
            unfilteredTable.Columns.Add("EndTime", Type.GetType("System.DateTime"));
            unfilteredTable.Columns.Add(UserNameColumn, Type.GetType("System.String"));
            unfilteredTable.Columns.Add(HostnameColumn, Type.GetType("System.String"));
            unfilteredTable.Columns.Add(ApplicationNameColumn, Type.GetType("System.String"));
            unfilteredTable.Columns.Add(LoginNameColumn, Type.GetType("System.String"));
            unfilteredTable.Columns.Add(IoReadsColumn, Type.GetType("System.Int64"));
            unfilteredTable.Columns.Add(IoWritesColumn, Type.GetType("System.Int64"));
            unfilteredTable.Columns.Add(CpuColummn, Type.GetType("System.Int64"));
            unfilteredTable.Columns.Add(TextDataColumn, Type.GetType("System.String"));
            unfilteredTable.Columns.Add("sql 1", Type.GetType("System.String"));
            unfilteredTable.Columns.Add(SpidColumn, Type.GetType("System.Int32"));
            unfilteredTable.Columns.Add(StartTimeColumn, Type.GetType("System.DateTime"));
            unfilteredTable.Columns.Add("DatabaseID", Type.GetType("System.Int32"));
            unfilteredTable.Columns.Add("RowNumber", Type.GetType("System.Int32"));
            //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session -- Added the new QueryPlan Column
            if (!queryMonitorConfiguration.TraceMonitoringEnabled)
                unfilteredTable.Columns.Add(QueryPlanColumn, Type.GetType("System.String"));

            DataColumn[] keys = new DataColumn[2];
            keys[0] = unfilteredTable.Columns["RowNumber"];
            keys[1] = unfilteredTable.Columns["EndTime"];

            unfilteredTable.PrimaryKey = keys;
            return unfilteredTable;
        }

        ///// <summary>
        ///// SqlDM 10.2 (Anshul Aggarwal) - Saves last read query monitor state.
        ///// </summary>
        //private void SaveQueryMonitorState(string fileName, long recordCount)
        //{
        //    using (LOG.DebugCall("SaveQueryMonitorState"))
        //    {
        //        SqlConnection connection = null;
        //        try
        //        {
        //            connection = OpenConnection();
        //            SqlCommand saveCommand = new SqlCommand(SqlCommandBuilder.BildQueryMonitor2012ExWriteQuery(
        //                fileName, recordCount), connection);

        //            LOG.Info(string.Format(queryMonitorLogStartTemplate + "Trying to save query monitor state : FileName = {0}, RecordCount = {1}", fileName, recordCount));
        //            var r = saveCommand.ExecuteNonQuery();
        //            LOG.Info(string.Format(queryMonitorLogStartTemplate + "Saved successfully. FileName = {0}, RecordCount = {1}", fileName, recordCount));
        //            connection.Close();
        //        }
        //        catch (Exception e)
        //        {
        //            LOG.Warn(queryMonitorLogStartTemplate + "Exception occurred in SaveQueryMonitorState() while saving query monitor state : ", e);
        //            if (connection != null && connection.State != System.Data.ConnectionState.Closed)
        //            {
        //                SqlConnection.ClearPool(connection);
        //                connection.Dispose();
        //            }
        //        }
        //    }
        //}

        //Start--SQLdm 10.3 (Tushar)--Support of LINQ assembly for activity monitor extended events collection.
        /// <summary>
        /// SQLdm 10.3 (Tushar)--This method reads data for activity monitor using extendend events api.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private DbDataReader GetActivityMonitorXEReaderData(CollectorCompleteEventArgs e)
        {
            using (LOG.DebugCall("GetActivityMonitorXEReaderData"))
            {
                Stopwatch filterSW = new Stopwatch();
                filterSW.Start();

                List<Tuple<string, string>> files = new List<Tuple<string, string>>();
                string flag = "", lastReadFlag = "";
                dbLastFileName = null;
                dbLastRecordCount = 0;

                using (SqlDataReader rd = e.Value as SqlDataReader)
                {
                    LOG.Debug(activityMonitorLogStartTemplate + string.Format("Last user defined flag was entered at {0}", String.IsNullOrEmpty(flag) ? "NA" : flag));
                    if (rd != null)
                    {
                        while (rd.Read())
                        {
                            if (!rd.IsDBNull(0)) flag = rd.GetString(0);
                            //Flag Format: 'SQLdm1 - ' + host_name() + ' - ' + convert(nvarchar(50),@currenttime,121) 
                            if (flag.Length > 23) lastReadFlag = flag.Substring(0, flag.Length - 23);
                        }

                        rd.NextResult(); // Read previous Activity Monitor State
                        while (rd.Read())
                        {
                            if (!rd.IsDBNull(0)) dbLastFileName = rd.GetString(0);
                            if (!rd.IsDBNull(1)) dbLastRecordCount = rd.GetInt64(1);
                            break;
                        }
                        LOG.Debug(activityMonitorLogStartTemplate + string.Format("DB FileName = {0} DB RecordCount = {1}", dbLastFileName, dbLastRecordCount));

                        rd.NextResult(); // Read filenames
                        while (rd.Read())
                        {
                            string fileName = null;
                            string fileNameInMilliseconds = null;
                            if (!rd.IsDBNull(0)) fileName = rd.GetString(0);
                            if (!rd.IsDBNull(1)) fileNameInMilliseconds = rd.GetString(1);
                            if (fileName != null && fileNameInMilliseconds != null)
                                files.Add(Tuple.Create(fileName, fileNameInMilliseconds));
                        }
                    }
                    LOG.Debug(activityMonitorLogStartTemplate + string.Format("Files to process - {0}", string.Join(",", files.Select(f => f.Item1 + ":" + f.Item2))));
                }
                var table = ReadActivityMonitorData(files, flag, lastReadFlag, dbLastFileName, dbLastRecordCount);
                filterSW.Stop();
                LOG.DebugFormat(activityMonitorLogStartTemplate + "Reading and Filtering took {0} milliseconds.", filterSW.ElapsedMilliseconds);
                return table;
            }
        }

        /// <summary>
        /// This method reads data for activity monitor using extended events api for each file.
        /// </summary>
        /// <param name="files"></param>
        /// <param name="flag"></param>
        /// <param name="lastReadFlag"></param>
        /// <param name="dbLastFileName"></param>
        /// <param name="dbLastRecordCount"></param>
        /// <returns></returns>
        private DbDataReader ReadActivityMonitorData(List<Tuple<string, string>> files, string flag, string lastReadFlag,
            string dbLastFileName, long dbLastRecordCount)
        {
            using (LOG.DebugCall("ReadActivityMonitorData"))
            {
                //string currentFileName = null;
                DataTable unfilteredTable = null;

                PublishedEventField eventField = null;
                PublishedAction action = null;
                long lastRowRead = 0, currentRowRead = 0, stopReadingAtRow = 0, rowCounter = 0, longestRowText = 0,
                    countNonNullTextData = 0, currentRecordCount = 0;
                lastFileRecordCount = 0;
                bool initializedDatatable = false, goNoFurther = false, shouldSkipEvents = dbLastFileName != null;

                for (int fileIndex = 0; fileIndex < files.Count; fileIndex++)
                {
                    if (goNoFurther)    // Stop processing if flag set to true in inner loop.
                        break;

                    currentFileName = files[fileIndex].Item2;
                    List<PublishedEvent> filteredEvents = new List<PublishedEvent>();
                    Tuple<string, string> file = files[fileIndex];
                    using (QueryableXEventData events = new QueryableXEventData(connectionInfo.ConnectionString, file.Item1,
                        EventStreamSourceOptions.EventFile, EventStreamCacheOptions.DoNotCache))
                    {
                        foreach (PublishedEvent eventData in events)
                        {
                            if (fileIndex == files.Count - 1) // For last file, track events.
                                lastFileRecordCount++;

                            if (shouldSkipEvents)   // Need to skip such events
                            {
                                int comparison = file.Item2.CompareTo(dbLastFileName);
                                if (comparison < 0)
                                    continue;

                                if (comparison == 0 && currentRecordCount < dbLastRecordCount)
                                {
                                    currentRecordCount++;
                                    continue;
                                }

                                shouldSkipEvents = false;
                            }

                            //int traceEventId = ProbeHelpers.GetTraceEventId(eventData.Name);    // Map event name to event id
                            //if (traceEventId == 98)    // query_post_execution_showplan
                            //{
                            //    if (eventData.Actions.TryGetValue("plan_handle", out action) && action.Value != null && action.Value is byte[]
                            //    && !planHandleToQueryPlan.ContainsKey((byte[])action.Value)
                            //    && eventData.Fields.TryGetValue("showplan_xml", out eventField) && eventField.Value != null)
                            //    {
                            //        planHandleToQueryPlan.Add((byte[])action.Value, ((XMLData)eventField.Value).RawString);
                            //    }

                            //    continue;
                            //}

                            //if (!FilterExtendedEvent(queryFilters.Item1, queryFilters.Item2,
                            //    queryFilters.Item3, queryFilters.Item4, eventData))  // Process include/exlcude like/match filters
                            //    continue;

                            filteredEvents.Add(eventData);
                        }

                        foreach (PublishedEvent eventData in filteredEvents)  // Process on filtered events
                        {
                            if (string.IsNullOrWhiteSpace(eventData.Name))
                                continue;

                            if (initializedDatatable == false)
                            {
                                LOG.Debug(queryMonitorLogStartTemplate + "Initializing unfiltered table");
                                // DM 10.3 SQLDM-28752 - Sessions and Blocking Data
                                unfilteredTable = ConstructUnfilteredActivityMonitorDataTable();
                                initializedDatatable = true;
                            }

                            DataRow row = unfilteredTable.NewRow();
                            var eventNumber = ProbeHelpers.GetTraceEventId(eventData.Name);
                            // DM 10.3 SQLDM-28752 - Sessions and Blocking Data
                            string textData = GetTextDataForActivityMonitor(eventData);
                            if (textData != null)
                            {
                                if (textData.Length > longestRowText) longestRowText = textData.Length;

                                //row["TextData"] = textData; Dont put it in textdata column. Waste of memory. 
                                //We already have it an a variable. Cant propogate this to multiple rows.
                                countNonNullTextData++;
                            }

                            rowCounter++;

                            //if this is a use configured event
                            switch (eventNumber)
                            {
                                case (int)TraceEvent.UserConfigurable0:
                                    if (!String.IsNullOrEmpty(textData))
                                    {
                                        if (textData.Contains(lastReadFlag) || textData.Contains("SQLdm3"))
                                        {
                                            //walk up the SQLdm1 rows
                                            lastRowRead = currentRowRead;
                                            currentRowRead = rowCounter;

                                            //stop reading at the flag that was just inserted
                                            //lastrowread will be the last sqldm\sqldm1 row
                                            if (textData == flag)
                                            {
                                                stopReadingAtRow = currentRowRead;
                                                LOG.Debug(activityMonitorLogStartTemplate + string.Format("Activity Monitor - Flag found at row {0}, Previous flag was at row {1}",
                                                    stopReadingAtRow.ToString(CultureInfo.InvariantCulture), lastRowRead.ToString(CultureInfo.InvariantCulture)));

                                                goNoFurther = true;
                                            }
                                        }
                                    }
                                    break;
                                // DM 10.3 SQLDM-28752 - Sessions and Blocking Data
                                case (int)TraceEvent.BlockedProcessReport:
                                    row["RowNumber"] = rowCounter;
                                    row["EventClass"] = eventNumber;
                                    row[TextDataColumn] = textData;

                                    // Object Id Information
                                    if (eventData.Fields.TryGetValue("object_id", out eventField) && eventField.Value != null)
                                        row["ObjectID"] = (int)eventField.Value;

                                    // Database Id Information
                                    if (eventData.Fields.TryGetValue("database_id", out eventField) && eventField.Value != null)
                                        row["DatabaseID"] = (int)((short)eventField.Value);

                                    // Pass the EndTime as Local Time similar behaior in Trace
                                    row["EndTime"] = ProbeHelpers.Truncate(eventData.Timestamp.LocalDateTime, TimeSpan.TicksPerMillisecond);

                                    ulong taskTime = 0;
                                    if (eventData.Actions.TryGetValue("task_time", out action) && action.Value != null)
                                    {
                                        taskTime = (ulong)action.Value / 1000;
                                    }
                                    // Start Time Logic
                                    row[StartTimeColumn] = eventData.Timestamp.DateTime.Subtract(TimeSpan.FromMilliseconds(taskTime));

                                    //if an interpreter sql_text limit has been imposed (CollectionServiceConfiguration.GetCollectionServiceElement().MaxQueryMonitorEventSizeKB > -1)
                                    if (!String.IsNullOrEmpty(textData) &&
                                           CollectionServiceConfiguration.GetCollectionServiceElement().
                                               MaxQueryMonitorEventSizeKB > -1)
                                    {
                                        Encoding encoding = Encoding.Default;
                                        var size = new FileSize { Bytes = encoding.GetByteCount(textData) };
                                        if (size.Kilobytes.HasValue &&
                                            size.Kilobytes.Value >
                                            CollectionServiceConfiguration.GetCollectionServiceElement().
                                                MaxQueryMonitorEventSizeKB)
                                        {
                                            LOG.DebugFormat("Skipping activity monitor statement in filter due to length. Length is {0:N2} KB. App: {1}, Database: {2}", size.Kilobytes, row[ApplicationNameColumn] ?? "", row[DbNameColumn] ?? "");
                                            //row will not be added
                                            continue;
                                        }
                                    }

                                    //[START]SQLdm 9.1 (Gaurav Karwal): no need to put the object name at the start of text data in activity monitor. This is copied from query monitor, where it makes sense.
                                    //Over here, this line was causing the XML to get corrupted and thus blocking report was not coming up
                                    row["sql 1"] = textData;


                                    AddDataToActivityMonitorRow(row, textData, eventData);
                                    unfilteredTable.Rows.Add(row);
                                    break;
                                case 12:
                                case 41:
                                case 43:
                                case 45:
                                // DM 10.3 SQLDM-28752 - Sessions and Blocking Data
                                case (int)TraceEvent.DeadlockGraph:
                                    row["RowNumber"] = rowCounter;
                                    row["EventClass"] = eventNumber;
                                    row[TextDataColumn] = textData;
                                    taskTime = 0;
                                    if (eventData.Actions.TryGetValue("task_time", out action) && action.Value != null)
                                    {
                                        taskTime = (ulong)action.Value / 1000;
                                    }
                                    // Start Time Logic
                                    row[StartTimeColumn] = eventData.Timestamp.DateTime.Subtract(TimeSpan.FromMilliseconds(taskTime));
                                    //if an interpreter sql_text limit has been imposed (CollectionServiceConfiguration.GetCollectionServiceElement().MaxQueryMonitorEventSizeKB > -1)
                                    if (!String.IsNullOrEmpty(textData) &&
                                           CollectionServiceConfiguration.GetCollectionServiceElement().
                                               MaxQueryMonitorEventSizeKB > -1)
                                    {
                                        Encoding encoding = Encoding.Default;
                                        var size = new FileSize { Bytes = encoding.GetByteCount(textData) };
                                        if (size.Kilobytes.HasValue &&
                                            size.Kilobytes.Value >
                                            CollectionServiceConfiguration.GetCollectionServiceElement().
                                                MaxQueryMonitorEventSizeKB)
                                        {
                                            LOG.DebugFormat("Skipping activity monitor statement in filter due to length. Length is {0:N2} KB. App: {1}, Database: {2}", size.Kilobytes, row[ApplicationNameColumn] ?? "", row[DbNameColumn] ?? "");
                                            //row will not be added
                                            continue;
                                        }
                                    }

                                    //[START]SQLdm 9.1 (Gaurav Karwal): no need to put the object name at the start of text data in activity monitor. This is copied from query monitor, where it makes sense.
                                    //Over here, this line was causing the XML to get corrupted and thus blocking report was not coming up
                                    row["sql 1"] = textData;


                                    AddDataToActivityMonitorRow(row, textData, eventData);
                                    unfilteredTable.Rows.Add(row);

                                    break;
                                // DM 10.3 SQLDM-28752 - Sessions and Blocking Data
                                case (int)TraceEvent.DataFileAutoGrow:
                                case (int)TraceEvent.LogFileAutoGrow:
                                    row["RowNumber"] = rowCounter;
                                    row["EventClass"] = eventNumber;
                                    row[TextDataColumn] = textData;

                                    // Duration Information
                                    if (eventData.Fields.TryGetValue("duration", out eventField) && eventField.Value != null)
                                        row[DurationColumn] = (long)((ulong)eventField.Value / 1000);

                                    // Database Name Information
                                    if (eventData.Fields.TryGetValue("database_name", out eventField) && eventField.Value != null)
                                        row[DbNameColumn] = GetString(eventField);

                                    // Database Id Information
                                    if (eventData.Fields.TryGetValue("database_id", out eventField) && eventField.Value != null)
                                        row["DatabaseID"] = (int)((uint)eventField.Value);
                                    taskTime = 0;
                                    if (eventData.Actions.TryGetValue("task_time", out action) && action.Value != null)
                                    {
                                        taskTime = (ulong)action.Value / 1000;
                                    }
                                    // Start Time Logic
                                    row[StartTimeColumn] = eventData.Timestamp.DateTime.Subtract(TimeSpan.FromMilliseconds(taskTime));


                                    //if an interpreter sql_text limit has been imposed (CollectionServiceConfiguration.GetCollectionServiceElement().MaxQueryMonitorEventSizeKB > -1)
                                    if (!String.IsNullOrEmpty(textData) && CollectionServiceConfiguration.GetCollectionServiceElement().
                                                MaxQueryMonitorEventSizeKB > -1)
                                    {
                                        Encoding encoding = Encoding.Default;
                                        var size = new FileSize { Bytes = encoding.GetByteCount(textData) };
                                        if (size.Kilobytes.HasValue && size.Kilobytes.Value > CollectionServiceConfiguration.GetCollectionServiceElement().
                                                MaxQueryMonitorEventSizeKB)
                                        {
                                            LOG.Debug(activityMonitorLogStartTemplate + String.Format("Skipping activity monitor statement in filter due to length. Length is {0:N2} KB. App: {1}, Database: {2}",
                                                    size.Kilobytes, row[ApplicationNameColumn] ?? "", row[DbNameColumn] ?? ""));
                                            //row will not be added
                                            continue;
                                        }
                                    }

                                    //[START]SQLdm 9.1 (Gaurav Karwal): no need to put the object name at the start of text data in activity monitor. This is copied from query monitor, where it makes sense.
                                    //Over here, this line was causing the XML to get corrupted and thus blocking report was not coming up
                                    row["sql 1"] = textData;
                                    AddDataToActivityMonitorRow(row, textData, eventData);
                                    unfilteredTable.Rows.Add(row);
                                    break;
                            }

                            if (goNoFurther)
                                break;
                        }
                    }

                } //end of loop

                // If processed new data, save last read file name to DB.
                //if (currentFileName != null && (currentFileName != dbLastFileName || lastFileRecordCount > dbLastRecordCount))
                //{
                //    SaveActivityMonitorState(currentFileName, lastFileRecordCount);
                //}
                //else
                //{
                //    LOG.Info(activityMonitorLogStartTemplate + "Skipping Activity Monitor State Save Operation - No changes in state.");
                //    LOG.Info(string.Format(activityMonitorLogStartTemplate + "DB FileName = {0} CurrentFileName = {1} DB RecordCount = {2} Current RecordCount = {3}", dbLastFileName,
                //        currentFileName, dbLastRecordCount, lastFileRecordCount));
                //}

                LOG.DebugFormat(activityMonitorLogStartTemplate + "There are {0} unfiltered rows", rowCounter.ToString(CultureInfo.InvariantCulture));
                LOG.DebugFormat(activityMonitorLogStartTemplate + "There is {0} difference between the row numbers", (stopReadingAtRow - lastRowRead).ToString(CultureInfo.InvariantCulture));
                LOG.DebugFormat(activityMonitorLogStartTemplate + "There are {0} rows of non-null textdata", countNonNullTextData.ToString(CultureInfo.InvariantCulture));
                LOG.DebugFormat(activityMonitorLogStartTemplate + "The longest TextData in any one row was {0} characters", longestRowText.ToString(CultureInfo.InvariantCulture));

                if (!initializedDatatable)
                {
                    LOG.Debug(activityMonitorLogStartTemplate + "DataTable was not initialized.");
                    return null;
                }
                if (!activityMonitorConfiguration.TraceMonitoringEnabled)
                {
                    lastRowRead = 0;
                    stopReadingAtRow = rowCounter + 1;
                }

                DataRow[] dr = unfilteredTable.Select(string.Format("RowNumber > {0} and RowNumber < {1} and EventClass in (137,148,92,93)",
                                                         lastRowRead.ToString(CultureInfo.InvariantCulture),
                                                         (stopReadingAtRow > lastRowRead
                                                              ? stopReadingAtRow
                                                              : currentRowRead).ToString(CultureInfo.InvariantCulture)), "RowNumber desc");

                //flood control
                if (dr.Length > activityMonitorConfiguration.RecordsPerRefresh)
                    dr = dr.Take(activityMonitorConfiguration.RecordsPerRefresh).ToArray();

                // if data was filtered, pick it, otherwise continue with original table.
                DataTable filteredData = (dr.Length != unfilteredTable.Rows.Count) ? (dr.Length > 0 ? dr.CopyToDataTable() : unfilteredTable.Clone())
                    : unfilteredTable;
                LOG.DebugFormat(activityMonitorLogStartTemplate + "There are {0} filtered rows", filteredData.Rows.Count.ToString(CultureInfo.InvariantCulture));

                //Return the dbdatareader of the filtered data
                return filteredData.CreateDataReader();
            }
        }

        /// <summary>
        /// Construct Activity Monitor Data Table
        /// </summary>
        /// <returns>Data Table for Activity Monitor</returns>
        /// <remarks>
        /// DM 10.3 SQLDM-28752 - Sessions and Blocking Data
        /// </remarks>
        private DataTable ConstructUnfilteredActivityMonitorDataTable()
        {
            //Initialize the data table
            var unfilteredTable = new DataTable("unfilteredDataTable");
            //Initialize the data table
            unfilteredTable.Columns.Add("EventClass", Type.GetType("System.Int32"));
            unfilteredTable.Columns.Add(DurationColumn, Type.GetType("System.Int64"));
            unfilteredTable.Columns.Add(DbNameColumn, Type.GetType("System.String"));
            unfilteredTable.Columns.Add("EndTime", Type.GetType("System.DateTime"));
            unfilteredTable.Columns.Add(UserNameColumn, Type.GetType("System.String"));
            unfilteredTable.Columns.Add(HostnameColumn, Type.GetType("System.String"));
            unfilteredTable.Columns.Add(ApplicationNameColumn, Type.GetType("System.String"));
            unfilteredTable.Columns.Add(LoginNameColumn, Type.GetType("System.String"));
            unfilteredTable.Columns.Add("ObjectID", Type.GetType("System.Int32"));
            unfilteredTable.Columns.Add(TextDataColumn, Type.GetType("System.String"));
            unfilteredTable.Columns.Add("sql 1", Type.GetType("System.String"));
            unfilteredTable.Columns.Add(SpidColumn, Type.GetType("System.Int32"));
            unfilteredTable.Columns.Add(StartTimeColumn, Type.GetType("System.DateTime"));
            unfilteredTable.Columns.Add("DatabaseID", Type.GetType("System.Int32"));
            unfilteredTable.Columns.Add("RowNumber", Type.GetType("System.Int32"));

            DataColumn[] keys = new DataColumn[2];
            keys[0] = unfilteredTable.Columns["RowNumber"];

            unfilteredTable.PrimaryKey = keys;
            return unfilteredTable;
        }

        ///// <summary>
        ///// This method saves the current state of activity monitor to the database.
        ///// </summary>
        ///// <param name="fileName"></param>
        ///// <param name="recordCount"></param>
        //private void SaveActivityMonitorState(string fileName, long recordCount)
        //{
        //    using (LOG.DebugCall("SaveActivityMonitorState"))
        //    {
        //        SqlConnection connection = null;
        //        try
        //        {
        //            connection = OpenConnection();
        //            // SQLdm 10.3 (Varun Chopra) Linux Support by passing cloudProviderId
        //            SqlCommand saveCommand = new SqlCommand(SqlCommandBuilder.BuildActivityMonitor2012ExWriteQuery(
        //                fileName, recordCount), connection);

        //            LOG.Info(string.Format(activityMonitorLogStartTemplate + "Trying to save activity monitor state : FileName = {0}, RecordCount = {1}", fileName, recordCount));
        //            saveCommand.ExecuteNonQuery();
        //            LOG.Info(string.Format(activityMonitorLogStartTemplate + "Saved successfully. FileName = {0}, RecordCount = {1}", fileName, recordCount));
        //            connection.Close();
        //        }
        //        catch (Exception e)
        //        {
        //            LOG.Warn(activityMonitorLogStartTemplate + "Exception occurred in SaveActivityMonitorState() while saving activity monitor state : ", e);
        //            if (connection != null && connection.State != System.Data.ConnectionState.Closed)
        //            {
        //                SqlConnection.ClearPool(connection);
        //                connection.Dispose();
        //            }
        //        }
        //    }
        //}
        //End--SQLdm 10.3 (Tushar)--Support of LINQ assembly for activity monitor extended events collection.

        #endregion

        #endregion


        #region Configuration
        #region Configuration Collector Methods

        private void StartConfigurationCollector()
        {
            StartGenericCollector(new Collector(ConfigurationCollector),
                                  refresh,
                                  "StartConfigurationCollector",
                                  "Configuration",
                                  refresh.Server.ProductVersion,
                                  sqlErrorDelegate, ConfigurationCallback, new object[] { });
        }

        /// <summary>
        /// Create Configuration Collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void ConfigurationCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd = SqlCommandBuilder.BuildConfigurationCommand(conn, refresh.Server.ProductVersion, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ConfigurationCallback));
        }

        /// <summary>
        /// Callback used to process the data returned from the Configuration collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The Idera.SQLdm.Probes.Collectors.CollectorCompleteEventArgs instance containing the event data.</param>
        private void ConfigurationCallback(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing ConfigurationCallback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, ConfigurationCallback, e);
                return;
            }
            numberOfDatabasesAM = 0;
            cloudDBNamesAMAzure =
                cloudDBNames.Where(db => !systemDbNames.Contains(db, StringComparer.OrdinalIgnoreCase)).ToList();
            NextCollector nextCollector = new NextCollector(StartActivityMonitorCollector);

            GenericCallback(new CollectorCallback(ConfigurationCallback),
                            refresh,
                            "ConfigurationCallback",
                            "Configuration",
                            new FailureDelegate(ConfigurationFailureDelegate),
                            new FailureDelegate(ConfigurationFailureDelegate),
                            nextCollector,
                            sender,
                            e,
                            true);
        }



        /// <summary>
        /// Define the Configuration callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ConfigurationCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader reader = e.Value as SqlDataReader)
            {
                ConfigurationSnapshot snapshot = new ConfigurationSnapshot(connectionInfo);
                snapshot.ConfigurationSettings.Columns.Add("Name", typeof(string));
                snapshot.ConfigurationSettings.Columns.Add("Comment", typeof(string));
                snapshot.ConfigurationSettings.Columns.Add("Minimum", typeof(int));
                snapshot.ConfigurationSettings.Columns.Add("Maximum", typeof(int));
                snapshot.ConfigurationSettings.Columns.Add("Config Value", typeof(int));
                snapshot.ConfigurationSettings.Columns.Add("Run Value", typeof(int));
                snapshot.ConfigurationSettings.Columns.Add("Restart Required", typeof(bool));

                while (reader.Read())
                {
                    snapshot.ConfigurationSettings.Rows.Add(
                        reader.GetString(0),
                        reader.GetString(1),
                        reader.GetInt32(2),
                        reader.GetInt32(3),
                        reader.GetInt32(4),
                        reader.GetInt32(5),
                        reader.GetInt32(6) == 1 ? true : false);
                }
                refresh.ConfigurationDetails = snapshot;
            }
        }

        /// <summary>
        /// Configuration failure delegate
        /// Since Configuration are optional we do not want to fail the entire refresh if unavailable
        /// </summary>
        /// <param name="snapshot">Snapshot to return with partial failure</param>
        protected void ConfigurationFailureDelegate(Snapshot snapshot, Exception e)
        {
            ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "There was an error in the Configuration collector: {0}", e, true);
        }

        #endregion
        #endregion

        #region Activity Monitor
        #region Activity Monitor Collector Methods

        /// <summary>
        /// Starts the Query Monitor collector
        /// </summary>
        private void StartActivityMonitorCollector()
        {

            try
            {
                if (cloudProviderId == CLOUD_PROVIDER_ID_AZURE)
                {
                    try
                    {
                        StartGenericCollectorDatabase(new CollectorDatabase(ActivityMonitorCollectorDatabase),
                                    refresh,
                                    "StartActivityMonitorCollector",
                                    "Activity Monitor",
                                     ActivityMonitorCallback, cloudDBNamesAMAzure[numberOfDatabasesAM], new object[] { refresh.MonitoredServer.
                                                                         ActivityMonitorConfiguration.TraceMonitoringEnabled });
                    }
                    catch (Exception exception)
                    {
                        LOG.Error("Unable to process Activity Monitor for Azure Instance.", exception);
                    }
                }
                else
                {
                    StartGenericCollector(new Collector(ActivityMonitorCollector),
                                        refresh,
                                        "StartActivityMonitorCollector",
                                        "Activity Monitor",
                                        refresh.Server.ProductVersion,
                                        sqlErrorDelegate, ActivityMonitorCallback, new object[] { refresh.MonitoredServer.
                                                                             ActivityMonitorConfiguration.TraceMonitoringEnabled });
                }

            }
            finally
            {
                PersistenceManager.Instance.SetActivityMonitorConfiguration(refresh.MonitoredServer.InstanceName,
                                                                         refresh.MonitoredServer.
                                                                             ActivityMonitorConfiguration);
            }
        }


        /// <summary>
        /// Callback used to process the data returned from the Activity Monitor collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The Idera.SQLdm.Probes.Collectors.CollectorCompleteEventArgs instance containing the event data.</param>
        private void ActivityMonitorCallback(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing Activity Monitor Callback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, ActivityMonitorCallback, e);
                return;
            }
            NextCollector nextCollector = new NextCollector(StartActivityMonitorStateStoreCollector);
            if (cloudProviderId == CLOUD_PROVIDER_ID_AZURE)
            {
                Interlocked.Increment(ref numberOfDatabasesAM);
                nextCollector = numberOfDatabasesAM < cloudDBNamesAMAzure.Count
                        ? StartActivityMonitorCollector
                        : nextCollector;
            }
            GenericCallback(new CollectorCallback(ActivityMonitorCallback),
                                refresh,
                                "ActivityMonitorCallback",
                                "Activity Monitor",
                                new FailureDelegate(ActivityMonitorFailureDelegate),
                                new FailureDelegate(ActivityMonitorFailureDelegate),
                                nextCollector,
                                sender,
                                e,
                                true,
                                true);

        }

        protected void ActivityMonitorFailureDelegate(Snapshot snapshot, Exception e)
        {
            //START-SQLdm 9.1 (Ankit Srivastava) handling old/deleted .xel file scneario - logging it specifically
            if (e.Message != null && (e.Message.ToLower().Contains("is invalid for log file") ||
                    e.Message.ToLower().Contains("specify an offset that exists in the log file and retry your query")))
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "The Activity Monitor Collector with Extended Event skipped because of following error , this collection would resume in next collection cycle: {0}", e, true);
            }
            else
                //END-SQLdm 9.1 (Ankit Srivastava) handling old/deleted .xel file scneario - logging it specifically
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "There was an error in the Activity Monitor collector: {0}", e, true);
        }
        
            /// <summary>
            /// Create the ActivityMonitor collector
            /// </summary>
            /// <param name="conn">Open SQL connection</param>
            /// <param name="sdtCollector">Standard SQL collector</param>
            /// <param name="ver">Server version</param>
            private void ActivityMonitorCollectorDatabase(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver, string dbName)
            {
                SqlCommand cmd;
                SqlCommand newCmd = conn.CreateCommand();

                if (activityMonitorCollectionState != null)
                {
                    lock (activityMonitorCollectionState)
                    {
                        if (activityMonitorCollectionState.UpdatedConfig != null)
                            activityMonitorConfiguration = activityMonitorCollectionState.UpdatedConfig;
                        activityMonitorCollectionState.ActivityMonitorCollectorStarted = true;
                    }
                }

                // SQLdm 10.3 (Varun Chopra) Linux Support for Activity Monitor by passing cloud provider id
                // Stop trace every refresh if disabled
                if (activityMonitorConfiguration != null && !activityMonitorConfiguration.Enabled)
                {
                    // LOG.Info("ActivityMonitor trace disabled - stopping trace if started");
                    // newCmd = SqlCommandBuilder.BuildActivityMonitorStopCommand(conn, ver, cloudProviderId);

                    //SQLdm 9.1 (Ankit Srivastava) --Activity Monitoring with Extended Events - stoping the extended event session if disabled
                    LOG.Verbose("Activity monitor Extended Events Session disabled - stopping Extended Events Session if started");
                    cmd = SqlCommandBuilder.BuildActivityMonitorStopCommandEX(conn, ver, cloudProviderId);
                }
                else
                { 
                    // Restart trace if necessary
                    if (previousRefresh != null)
                    {
                        if (activityMonitorConfiguration.TraceMonitoringEnabled)//SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with Extended Events -- to check if trace is enabled
                        {
                            //SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with Extended Events - stoping the extended event session before starting the trace
                            LOG.Verbose("Activity monitor Extended Events Session disabled - stopping Extended Events Session since trace Enabled Now");
                            newCmd = SqlCommandBuilder.BuildActivityMonitorStopCommandEX(conn, ver, cloudProviderId);

                            cmd = SqlCommandBuilder.BuildActivityMonitorTraceCommand(
                                conn,
                                ver,
                                activityMonitorConfiguration,
                                previousRefresh.MonitoredServer.ActivityMonitorConfiguration,
                                refresh.TimeStamp, cloudProviderId);
                        }
                        else
                        {
                            //SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with Extended Events - stoping the trace before starting extended event session
                            LOG.Verbose("ActivityMonitor trace disabled - stopping trace if started");
                            newCmd = SqlCommandBuilder.BuildActivityMonitorStopCommand(conn, ver, cloudProviderId);

                            cmd = SqlCommandBuilder.BuildActivityMonitorCommandEX(
                                conn,
                                ver,
                                activityMonitorConfiguration,
                                previousRefresh.MonitoredServer.ActivityMonitorConfiguration,
                                cloudProviderId,
                                cloudDBNames.Count());
                        }

                    }
                    else
                    {
                        LOG.Verbose("ActivityMonitor trace disabled - stopping trace if started");
                        newCmd = SqlCommandBuilder.BuildActivityMonitorStopCommand(conn, ver, cloudProviderId);

                        cmd = SqlCommandBuilder.BuildActivityMonitorCommandWithRestartEX(
                            conn,
                            ver,
                            activityMonitorConfiguration, cloudProviderId);

                    }
                }
                if (!String.IsNullOrEmpty(newCmd.CommandText))
                    newCmd.CommandText += cmd.CommandText;
                else
                    newCmd = cmd;

                sdtCollector = new SqlCollector(newCmd, true);
                sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ActivityMonitorCallback));
            }
        private void ActivityMonitorCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd;
            SqlCommand newCmd = conn.CreateCommand(); //SQLdm 9.1 (Ankit Srivastava) -Activity Monitoring with Extended Events --  new additional SQL command

            if (activityMonitorCollectionState != null)
            {
                lock (activityMonitorCollectionState)
                {
                    if (activityMonitorCollectionState.UpdatedConfig != null)
                        activityMonitorConfiguration = activityMonitorCollectionState.UpdatedConfig;
                    activityMonitorCollectionState.ActivityMonitorCollectorStarted = true;
                }
            }

            // SQLdm 10.3 (Varun Chopra) Linux Support for Activity Monitor by passing cloud provider id
            // Stop trace every refresh if disabled
            if (activityMonitorConfiguration != null && !activityMonitorConfiguration.Enabled)
            {
                LOG.Verbose("ActivityMonitor trace disabled - stopping trace if started");
                newCmd = SqlCommandBuilder.BuildActivityMonitorStopCommand(conn, ver, cloudProviderId);

                //SQLdm 9.1 (Ankit Srivastava) --Activity Monitoring with Extended Events - stoping the extended event session if disabled
                LOG.Verbose("Activity monitor Extended Events Session disabled - stopping Extended Events Session if started");
                cmd = SqlCommandBuilder.BuildActivityMonitorStopCommandEX(conn, ver, cloudProviderId);
            }
            else
            {
                // Restart trace if necessary
                if (previousRefresh != null)
                {
                    if (activityMonitorConfiguration.TraceMonitoringEnabled)//SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with Extended Events -- to check if trace is enabled
                    {
                        //SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with Extended Events - stoping the extended event session before starting the trace
                        LOG.Verbose("Activity monitor Extended Events Session disabled - stopping Extended Events Session since trace Enabled Now");
                        newCmd = SqlCommandBuilder.BuildActivityMonitorStopCommandEX(conn, ver, cloudProviderId);

                        cmd = SqlCommandBuilder.BuildActivityMonitorTraceCommand(
                            conn,
                            ver,
                            activityMonitorConfiguration,
                            previousRefresh.MonitoredServer.ActivityMonitorConfiguration,
                            refresh.TimeStamp, cloudProviderId);
                    }
                    else
                    {
                        //SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with Extended Events - stoping the trace before starting extended event session
                        LOG.Verbose("ActivityMonitor trace disabled - stopping trace if started");
                        newCmd = SqlCommandBuilder.BuildActivityMonitorStopCommand(conn, ver, cloudProviderId);

                        cmd = SqlCommandBuilder.BuildActivityMonitorCommandEX(
                            conn,
                            ver,
                            activityMonitorConfiguration,
                            previousRefresh.MonitoredServer.ActivityMonitorConfiguration,
                            cloudProviderId);
                    }

                }
                else
                {
                    int blockedProcessThresholdOnServer = 0;
                    if (refresh.ConfigurationDetails != null && refresh.ConfigurationDetails.Error == null)
                    {
                        foreach (DataRow row in refresh.ConfigurationDetails.ConfigurationSettings.Rows)
                        {
                            string rowName = (string)row["Name"];
                            rowName = rowName.ToLower();
                            if (rowName.StartsWith("blocked process threshold"))
                            {
                                blockedProcessThresholdOnServer = (int)row["Config Value"];
                                break;
                            }
                        }
                    }

                    //if the customer has the blocked process threshold set, we need to honor it.
                    if (blockedProcessThresholdOnServer != 0)
                        activityMonitorConfiguration.BlockedProcessThreshold = blockedProcessThresholdOnServer;

                    // If we don't know the previous configuration, go ahead and restart so we know we're operating with
                    // appropriate flags/filters on the trace
                    if (activityMonitorConfiguration.TraceMonitoringEnabled) //SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with Extended Events -- to check if trace is enabled
                    {
                        //SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with Extended Events - stoping the extended event session before starting the trace
                        LOG.Verbose("Activity monitor Extended Events Session disabled - stopping Extended Events Session since trace Enabled Now");
                        newCmd = SqlCommandBuilder.BuildActivityMonitorStopCommandEX(conn, ver, cloudProviderId);

                        cmd = SqlCommandBuilder.BuildActivityMonitorTraceCommandWithRestart(
                            conn,
                            ver,
                            activityMonitorConfiguration,
                            refresh.TimeStamp, cloudProviderId);
                    }
                    else
                    { //SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with Extended Events - stoping the trace before starting extended event session
                        LOG.Verbose("ActivityMonitor trace disabled - stopping trace if started");
                        newCmd = SqlCommandBuilder.BuildActivityMonitorStopCommand(conn, ver, cloudProviderId);

                        cmd = SqlCommandBuilder.BuildActivityMonitorCommandWithRestartEX(
                            conn,
                            ver,
                            activityMonitorConfiguration, cloudProviderId);
                    }
                }
            }
            //Start ---SQLdm 9.1 (Ankit Srivastava) --Activity Monitoring with Extended Events - add the stop commands if exists
            if (!String.IsNullOrEmpty(newCmd.CommandText))
                newCmd.CommandText += cmd.CommandText;
            else
                newCmd = cmd;
            //end ---SQLdm 9.1 (Ankit Srivastava) --Activity Monitoring with Extended Events - add the stop commands if exists

            sdtCollector = new SqlCollector(newCmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ActivityMonitorCallback));
        }

        /// <summary>
        /// Define the Query Monitor callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ActivityMonitorCallback(CollectorCompleteEventArgs e)
        {
            if (e.Exception != null)
            {
                refresh.ActivityMonitorError = e.Exception;
            }
            else
            {
                try
                {
                    LOG.Verbose(activityMonitorLogStartTemplate + "Version = " + refresh.ProductVersion.Major);
                    if (cloudProviderId == CLOUD_PROVIDER_ID_AZURE || cloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId)
                    {
                        LOG.Verbose(queryMonitorLogStartTemplate + " Interpreting Query Monitor data for Azure.");
                        using (SqlDataReader rd = e.Value as SqlDataReader)
                        {
                            InterpretActivityMonitor(FilterActivityMonitorDataAzure(ConvertActivityMonitorDataFromXML(rd)));
                        }
                    }
                    else if (refresh.ProductVersion.Major >= 11 && !activityMonitorConfiguration.TraceMonitoringEnabled && refresh.CollectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember))
                    {
                        LOG.Verbose(activityMonitorLogStartTemplate + "Trace disabled. Using XEReaderAPI for extended events.");
                        InterpretActivityMonitor(GetActivityMonitorXEReaderData(e));
                    }
                    else if (refresh.ProductVersion.Major > 8)
                    {
                        LOG.Verbose(activityMonitorLogStartTemplate + "Trace = " + activityMonitorConfiguration.TraceMonitoringEnabled);
                        using (SqlDataReader rd = e.Value as SqlDataReader)
                        {
                            InterpretActivityMonitor(FilterActivityMonitorData(rd));
                        }
                    }
                    else
                    {
                        LOG.Verbose(activityMonitorLogStartTemplate + "Using Trace approach for < 2005 servers.");
                        using (SqlDataReader rd = e.Value as SqlDataReader)
                        {
                            InterpretActivityMonitor(rd);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ActivityMonitorFailureDelegate(refresh, ex);
                }
            }

        }
        private DbDataReader ConvertActivityMonitorDataFromXML(SqlDataReader rawDataReader)
        {
            String dbName = null;

            if (!initializedUnfilteredAMTableFromXML)
            {
                unfilteredAMTableFromXML = new DataTable("tempTable");
                initializedUnfilteredAMTableFromXML = true;
                LOG.Debug(queryMonitorLogStartTemplate + "Initializing unfiltered table to store data after parsing XML");
                //Initialize the data table

                unfilteredAMTableFromXML.Columns.Add("EventClass", Type.GetType("System.Int32"));
                unfilteredAMTableFromXML.Columns.Add("Duration", Type.GetType("System.Int64"));
                unfilteredAMTableFromXML.Columns.Add("DBName", Type.GetType("System.String"));
                unfilteredAMTableFromXML.Columns.Add("CompletionTime", Type.GetType("System.DateTime"));
                unfilteredAMTableFromXML.Columns.Add("NTUserName", Type.GetType("System.String"));
                unfilteredAMTableFromXML.Columns.Add("HostName", Type.GetType("System.String"));
                unfilteredAMTableFromXML.Columns.Add("ApplicationName", Type.GetType("System.String"));
                unfilteredAMTableFromXML.Columns.Add("LoginName", Type.GetType("System.String"));
                unfilteredAMTableFromXML.Columns.Add("ObjectID", Type.GetType("System.Int32"));
                unfilteredAMTableFromXML.Columns.Add("TextData", Type.GetType("System.String"));
                unfilteredAMTableFromXML.Columns.Add("ObjectName", Type.GetType("System.String"));
                unfilteredAMTableFromXML.Columns.Add("SPID", Type.GetType("System.Int32"));
                unfilteredAMTableFromXML.Columns.Add("StartTime", Type.GetType("System.DateTime"));
                unfilteredAMTableFromXML.Columns.Add("DatabaseID", Type.GetType("System.Int32"));
            }
            unfilteredAMTableFromXML.Clear();
            if (rawDataReader != null && rawDataReader.Read())
            {
                if (!rawDataReader.IsDBNull(0))
                {
                    dbName = rawDataReader.GetString(0);
                    rawDataReader.NextResult();
                }
            }
            if (rawDataReader != null && dbName != null && rawDataReader.Read())
            {
                try
                {
                    //T xmlStng = rawDataReader.GetTextReader(0);
                    string ringBufferPrefix = @"/RingBufferTarget";
                    string xmlString = rawDataReader.GetString(0);
                    DataRow row;
                    var doc = new XmlDocument();
                    doc.LoadXml(xmlString);

                    if (doc.DocumentElement != null)
                    {
                        var eventPrefix = ringBufferPrefix + @"/event/";
                        var eventNodes = doc.DocumentElement.SelectNodes(eventPrefix.TrimEnd('/'));
                        row = unfilteredAMTableFromXML.NewRow();
                        foreach (XmlNode eventNode in eventNodes)
                        {
                            if (eventNode == null || !(cloudProviderId==Constants.MicrosoftAzureId? PopulateRowForAM(eventNode, null, row): PopulateRowForAMManaged(eventNode, null, row)))
                            {
                                continue;
                            }
                            row["DBName"] = dbName;
                            unfilteredAMTableFromXML.Rows.Add(row);
                            
                            row = unfilteredAMTableFromXML.NewRow();
                        }
                        //MapQueryPlanToRow(planHandleToQueryPlan, ref unfilteredTableFromXML, ref tempTable);
                    }
                    DataRow row1 = unfilteredAMTableFromXML.NewRow();
                   // addDummyDeadlock(row1,dbName);
                    unfilteredAMTableFromXML.Rows.Add(row1);
                }
                catch (Exception exception)
                {
                    LOG.Error("Unable to process the Ring Buffer.", exception);
                }
            }
            return unfilteredAMTableFromXML.CreateDataReader();
        }
        
        private bool PopulateRowForAM(XmlNode doc, string eventPrefix, DataRow row)
        {
            var node =
                doc.SelectSingleNode(eventPrefix + @"@name");
            if (node == null)
                return false;
            node = doc.SelectSingleNode(eventPrefix + @"data[@name='database_name']");
            if (node != null)
            {
                row["DBName"] = Convert.ToString(node.InnerText);
            }

            node = doc.SelectSingleNode(eventPrefix + @"@name");
            if(node!=null)
            row["EventClass"] = ProbeHelpers.GetTraceEventId(node.InnerText);

            node = doc.SelectSingleNode(eventPrefix + @"@timestamp");
            if (node != null) row["CompletionTime"] = Convert.ToDateTime(node.InnerText);

            node = doc.SelectSingleNode(eventPrefix + @"data[@name='duration']");
            if (node != null)
            {
                TimeSpan duration = TimeSpan.FromMilliseconds(Convert.ToInt64(node.InnerText) / 1000);
                row["Duration"] = (long)duration.TotalMilliseconds;
                DateTime statementUTCompletionTime = ProbeHelpers.Truncate(Convert.ToDateTime(row["CompletionTime"]), TimeSpan.TicksPerMillisecond);
                if (row[StartTimeColumn] != null && string.IsNullOrEmpty(row[StartTimeColumn].ToString()))
                {
                    row["StartTime"] = statementUTCompletionTime.Subtract(duration);
                }
                else
                {
                    row["StartTime"] = ((DateTime)row[StartTimeColumn]).Subtract(duration);
                }
            }

            node = doc.SelectSingleNode(eventPrefix + @"action[@name='username']");
            if (node != null) row["NTUserName"] = Convert.ToString(node.InnerText);

            node = doc.SelectSingleNode(eventPrefix + @"action[@name ='client_hostname']");
            if (node != null) row["HostName"] = Convert.ToString(node.InnerText);

            node = doc.SelectSingleNode(eventPrefix + @"action[@name='client_app_name']");
            if (node != null) row["ApplicationName"] = Convert.ToString(node.InnerText);

            node = doc.SelectSingleNode(eventPrefix + @"action[@name='username']");
            if (node != null) row["LoginName"] = Convert.ToString(node.InnerText);

            node = doc.SelectSingleNode(eventPrefix + @"data[@name ='object_id']");
            if (node != null) row["ObjectID"] = Convert.ToString(node.InnerText);

            node = doc.SelectSingleNode(eventPrefix + @"data[@name='blocked_process']/value");
            if (node != null)
                row["TextData"] = Convert.ToString(node.OuterXml.Replace("<value>","").Replace("</value>", ""));

            else
            {
                node = doc.SelectSingleNode(eventPrefix + @"data[@name='xml_report']/value");
                if (node != null)
                    row["TextData"] = Convert.ToString(node.InnerXml);
            }
            var x = Convert.ToString(node.InnerXml.Replace("<value>", "").Replace("<value>", ""));
            node = doc.SelectSingleNode(eventPrefix + @"data[@name='object_name']");
            if (node != null) row["ObjectName"] = Convert.ToString(node.InnerText);

            node = doc.SelectSingleNode(eventPrefix + @"action[@name='session_id']");
            if (node != null) row["SPID"] = Convert.ToInt32(node.InnerText);

            node = doc.SelectSingleNode(eventPrefix + @"data[@name='database_id']");
            if (node != null) row["DatabaseID"] = Convert.ToInt32(node.InnerText);

            return true;
        }

        private bool PopulateRowForAMManaged(XmlNode doc, string eventPrefix, DataRow row)
        {
            var node =
                doc.SelectSingleNode(eventPrefix + @"@name");
            if (node == null)
                return false;
            node = doc.SelectSingleNode(eventPrefix + @"data[@name='database_name']");
            if (node != null)
            {
                row["DBName"] = Convert.ToString(node.InnerText);
            }

            node = doc.SelectSingleNode(eventPrefix + @"@name");
            if (node != null)
                row["EventClass"] = ProbeHelpers.GetTraceEventId(node.InnerText);

            node = doc.SelectSingleNode(eventPrefix + @"@timestamp");
            if (node != null) row["CompletionTime"] = Convert.ToDateTime(node.InnerText);

            node = doc.SelectSingleNode(eventPrefix + @"data[@name='duration']");
            if (node != null)
            {
                TimeSpan duration = TimeSpan.FromMilliseconds(Convert.ToInt64(node.InnerText) / 1000);
                row["Duration"] = (long)duration.TotalMilliseconds;
                DateTime statementUTCompletionTime = ProbeHelpers.Truncate(Convert.ToDateTime(row["CompletionTime"]), TimeSpan.TicksPerMillisecond);
                if (row[StartTimeColumn] != null && string.IsNullOrEmpty(row[StartTimeColumn].ToString()))
                {
                    row["StartTime"] = statementUTCompletionTime.Subtract(duration);
                }
                else
                {
                    row["StartTime"] = ((DateTime)row[StartTimeColumn]).Subtract(duration);

                }
            }
            else
            {
                node = doc.SelectSingleNode(eventPrefix + @"action[@name='task_time']/value");
                if (node != null)
                {
                    TimeSpan duration = TimeSpan.FromMilliseconds(Convert.ToInt64(node.InnerText) / 1000);
                    row["Duration"] = (long)duration.TotalMilliseconds;
                    DateTime statementUTCompletionTime = ProbeHelpers.Truncate(Convert.ToDateTime(row["CompletionTime"]), TimeSpan.TicksPerMillisecond);
                    if (row[StartTimeColumn] != null && string.IsNullOrEmpty(row[StartTimeColumn].ToString()))
                    {
                        row["StartTime"] = statementUTCompletionTime.Subtract(duration);
                    }
                    else
                    {
                        row["StartTime"] = ((DateTime)row[StartTimeColumn]).Subtract(duration);

                    }
                }
            }

            node = doc.SelectSingleNode(eventPrefix + @"action[@name='nt_username']");
            if (node != null) row["NTUserName"] = Convert.ToString(node.InnerText);

            node = doc.SelectSingleNode(eventPrefix + @"action[@name ='client_hostname']");
            if (node != null) row["HostName"] = Convert.ToString(node.InnerText);

            node = doc.SelectSingleNode(eventPrefix + @"action[@name='client_app_name']");
            if (node != null) row["ApplicationName"] = Convert.ToString(node.InnerText);

            node = doc.SelectSingleNode(eventPrefix + @"action[@name='username']");
            if (node != null) row["LoginName"] = Convert.ToString(node.InnerText);

            node = doc.SelectSingleNode(eventPrefix + @"data[@name ='object_id']");
            if (node != null) row["ObjectID"] = Convert.ToString(node.InnerText);

            node = doc.SelectSingleNode(eventPrefix + @"data[@name='blocked_process']/value");
            if (node != null)
                row["TextData"] = Convert.ToString(node.OuterXml.Replace("<value>", "").Replace("</value>", ""));

            else
            {
                node = doc.SelectSingleNode(eventPrefix + @"data[@name='xml_report']/value");
                if (node != null)
                    row["TextData"] = Convert.ToString(node.InnerXml);
            }
            var x = Convert.ToString(node.InnerXml.Replace("<value>", "").Replace("<value>", ""));
            node = doc.SelectSingleNode(eventPrefix + @"data[@name='object_name']");
            if (node != null) row["ObjectName"] = Convert.ToString(node.InnerText);

            node = doc.SelectSingleNode(eventPrefix + @"action[@name='session_id']");
            if (node != null) row["SPID"] = Convert.ToInt32(node.InnerText);

            node = doc.SelectSingleNode(eventPrefix + @"data[@name='database_id']");
            if (node != null) row["DatabaseID"] = Convert.ToInt32(node.InnerText);

            return true;
        }

        //START - SQLDM 10.3 (Manali Hukkeri) : Technical debt changes
        /// <summary>
        /// Starts the Activity Monitor State store collector. This collector will save the last file the last line read.
        /// </summary>
        private void StartActivityMonitorStateStoreCollector()
        {
            try
            {
                StartGenericCollector(new Collector(ActivityMonitorStateStoreCollector),
                                   refresh,
                                   "StartActivityMonitorStateStoreCollector",
                                   "Activity Monitor State Store",
                                   refresh.Server.ProductVersion,
                                   sqlErrorDelegate, null, new object[] { });

            }
            finally
            {
                PersistenceManager.Instance.SetActivityMonitorConfiguration(refresh.MonitoredServer.InstanceName,
                                                                         refresh.MonitoredServer.ActivityMonitorConfiguration);
            }
        }

        /// <summary>
        /// Create Acitivity Monitor State Store Collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void ActivityMonitorStateStoreCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand saveCommand = null;
            using (LOG.DebugCall("SaveActivityMonitorState"))
            {
                if (this.refresh.CollectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember) && currentFileName != null && (currentFileName != dbLastFileName || lastFileRecordCount > dbLastRecordCount))
                {
                    // SQLdm 10.3 (Varun Chopra) Linux Support by passing cloudProviderId
                    saveCommand = new SqlCommand(SqlCommandBuilder.BuildActivityMonitor2012ExWriteQuery(
                        currentFileName, lastFileRecordCount), conn);
                    sdtCollector = new SqlCollector(saveCommand, true);
                }
                else
                {
                    sdtCollector = new SqlCollector(new SqlCommand(string.Empty, conn), true);
                }


                LOG.Debug(string.Format(activityMonitorLogStartTemplate + "Trying to save activity monitor state : FileName = {0}, RecordCount = {1}", currentFileName, lastFileRecordCount));
                sdtCollector.BeginCollectionNonQueryExecution(new EventHandler<CollectorCompleteEventArgs>(ActivityMonitorStateStoreCallback));
                LOG.Debug(string.Format(activityMonitorLogStartTemplate + "Saved successfully. FileName = {0}, RecordCount = {1}", currentFileName, lastFileRecordCount));
            }
        }

        /// <summary>
        /// Callback used to process the data returned from the Activity Monitor State Store collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The Idera.SQLdm.Probes.Collectors.CollectorCompleteEventArgs instance containing the event data.</param>
        private void ActivityMonitorStateStoreCallback(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing ActivityMonitorStateStoreCallback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, ActivityMonitorStateStoreCallback, e);
                return;
            }


            NextCollector nextCollector = new NextCollector(StartOSMetricsCollector);
            if (workload.GetMetricThresholdEnabled(Metric.ErrorLog) || workload.GetMetricThresholdEnabled(Metric.AgentLog))
            {
                nextCollector = new NextCollector(StartLogScanVariableStoreCollector);     //SQLDM 10.3 (Manali Hukkeri): Technical debt changes
            }
            else if (refresh.ProductVersion.Major > 8)
            {
                nextCollector = new NextCollector(StartMirrorMetricsUpdateCollector);
            }


            GenericCallback(new CollectorCallback(ActivityMonitorStateStoreCallback),
                            refresh,
                            "ActivityMonitorStateStoreCallback",
                            "Activity Monitor",
                            new FailureDelegate(ActivityMonitorStateStoreFailureDelegate),
                            new FailureDelegate(ActivityMonitorStateStoreFailureDelegate),
                            nextCollector,
                            sender,
                            e,
                            true,
                            true);

        }

        /// <summary>
        /// Define the ActivityMonitorStateStoreCallback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ActivityMonitorStateStoreCallback(CollectorCompleteEventArgs e)
        {
            // Write the Last Read File Name and Offset to the tempdb
            // Callback required for logging purposes only
            if (e.Result == Result.Success)
            {
                LOG.Verbose("ActivityMonitorStateStoreCallback() successfully completed");
            }
            else
            {
                var exceptionMessage = "ActivityMonitorStateStoreCallback() Write the Last Read File Name and Offset to the tempdb failed ";
                if (e.Exception != null)
                {
                    exceptionMessage += e.Exception.ToString();
                }
                LOG.Error(exceptionMessage);
            }
        }

        protected void ActivityMonitorStateStoreFailureDelegate(Snapshot snapshot, Exception e)
        {
            //START-SQLdm 9.1 (Ankit Srivastava) handling old/deleted .xel file scneario - logging it specifically
            if (e.Message != null && (e.Message.ToLower().Contains("is invalid for log file") ||
                    e.Message.ToLower().Contains("specify an offset that exists in the log file and retry your query")))
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "The Activity Monitor State store Collection with Extended Event skipped because of following error , this collection would resume in next collection cycle: {0}", e, true);
            }
            else
                //END-SQLdm 9.1 (Ankit Srivastava) handling old/deleted .xel file scneario - logging it specifically
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "There was an error in the Activity Monitor State store collector: {0}", e, true);
        }


        //END - SQLDM 10.3 (Manali Hukkeri) : Technical debt changes//START - SQLDM 10.3 (Manali Hukkeri) : Technical debt change
        #endregion

        #region Activity Monitor Interpretation Methods
        /// <summary>
        /// Reduce the returned data to just the events that have occurred since we last got events
        /// </summary>
        /// <param name="unfilteredDataReader"></param>
        /// <returns></returns>
        private DbDataReader FilterActivityMonitorData(SqlDataReader unfilteredDataReader)
        {
            var unfilteredDataSet = new DataSet("unfiltered");
            unfilteredDataSet.Tables.Add(new DataTable("unfilteredDataTable"));
            var unfilteredTable = unfilteredDataSet.Tables["unfilteredDataTable"];

            long lastRowRead = 0;
            long currentRowRead = 0;
            long stopReadingAtRow = 0;
            long rowCounter = 0;
            bool initializedDatatable = false;
            bool goNoFurther = false;
            int countNonNullTextData = 0;
            long longestRowText = 0;

            string flag = "";
            string lastReadFlag = "";
            using (LOG.DebugCall("FilterActivityMonitorData"))
            {
                Stopwatch filterSW = new Stopwatch();
                filterSW.Start();

                if (unfilteredDataReader != null)
                {
                    while (unfilteredDataReader.Read())
                    {
                        //if (!unfilteredDataReader.IsDBNull(0)) flag = unfilteredDataReader.GetString(0);
                        //Flag Format: 'SQLdm1 - ' + host_name() + ' - ' + convert(nvarchar(50),@currenttime,121) 
                        //if (flag.Length > 23) lastReadFlag = flag.Substring(0, flag.Length - 23);
                    }

                    LOG.DebugFormat("Last user defined flag was entered at {0}", String.IsNullOrEmpty(flag) ? "NA" : flag);

                    unfilteredDataReader.NextResult();

                    while (unfilteredDataReader.Read())
                    {
                        //if the event column is not null
                        if (!unfilteredDataReader.IsDBNull(0))
                        {
                            if (initializedDatatable == false)
                            {
                                LOG.Debug("Initializing unfiltered table");

                                //Initialize the data table
                                unfilteredTable.Columns.Add("EventClass", Type.GetType("System.Int32"));
                                unfilteredTable.Columns.Add(DurationColumn, Type.GetType("System.Int64"));
                                unfilteredTable.Columns.Add(DbNameColumn, Type.GetType("System.String"));
                                unfilteredTable.Columns.Add("EndTime", Type.GetType("System.DateTime"));
                                unfilteredTable.Columns.Add(UserNameColumn, Type.GetType("System.String"));
                                unfilteredTable.Columns.Add(HostnameColumn, Type.GetType("System.String"));
                                unfilteredTable.Columns.Add(ApplicationNameColumn, Type.GetType("System.String"));
                                unfilteredTable.Columns.Add(LoginNameColumn, Type.GetType("System.String"));
                                unfilteredTable.Columns.Add("ObjectID", Type.GetType("System.Int32"));
                                unfilteredTable.Columns.Add(TextDataColumn, Type.GetType("System.String"));
                                unfilteredTable.Columns.Add("sql 1", Type.GetType("System.String"));
                                unfilteredTable.Columns.Add(SpidColumn, Type.GetType("System.Int32"));
                                unfilteredTable.Columns.Add(StartTimeColumn, Type.GetType("System.DateTime"));
                                unfilteredTable.Columns.Add("DatabaseID", Type.GetType("System.Int32"));
                                unfilteredTable.Columns.Add("RowNumber", Type.GetType("System.Int32"));

                                DataColumn[] keys = new DataColumn[1];
                                keys[0] = unfilteredTable.Columns["RowNumber"];
                                //keys[1] = unfilteredTable.Columns["StartTime"];

                                unfilteredTable.PrimaryKey = keys;
                                initializedDatatable = true;
                            }

                            DataRow row = unfilteredTable.NewRow();
                            var eventNumber = unfilteredDataReader.GetInt32(0);

                            string textData = "";
                            if (!unfilteredDataReader.IsDBNull(9))
                            {
                                textData = unfilteredDataReader.GetString(9);

                                if (textData.Length > longestRowText) longestRowText = textData.Length;

                                //row["TextData"] = textData; Dont waste mem by putting this on every row.
                                //sql 1 is enough.  That will ultimately be compressed.
                                countNonNullTextData++;
                            }

                            rowCounter++;


                            //if this is a use configured event
                            switch (eventNumber)
                            {
                                case (int)TraceEvent.UserConfigurable0:
                                    if (!String.IsNullOrEmpty(textData))
                                    {
                                        //if the file has 7.2 flags the flags will be SQLdm not sqldm1
                                        //If this user config event is from performance monitor
                                        if (textData.Contains(lastReadFlag) || textData.Contains("SQLdm3"))
                                        {
                                            //walk up the SQLdm3 rows
                                            lastRowRead = currentRowRead;
                                            currentRowRead = rowCounter;

                                            //stop reading at the flag that was just inserted
                                            //lastrowread will be the last sqldm3 row
                                            if (textData == flag)
                                            {
                                                stopReadingAtRow = currentRowRead;
                                                LOG.DebugFormat("Flag found at row {0}, Previous flag was at row {1}", stopReadingAtRow.ToString(CultureInfo.InvariantCulture), lastRowRead.ToString(CultureInfo.InvariantCulture));
                                                goNoFurther = true;
                                            }
                                        }
                                    }
                                    break;
                                //case 12:
                                //case 41:
                                //case 43:
                                //case 45:
                                case 137:
                                case 148:
                                case 92:
                                case 93:
                                    row["RowNumber"] = rowCounter;

                                    row["EventClass"] = unfilteredDataReader.GetInt32(0);

                                    if (!unfilteredDataReader.IsDBNull(1))
                                        row[DurationColumn] = unfilteredDataReader.GetInt64(1) / 1000;
                                    if (!unfilteredDataReader.IsDBNull(2))
                                        row[DbNameColumn] = unfilteredDataReader.GetString(2);
                                    if (!unfilteredDataReader.IsDBNull(3))
                                        row["EndTime"] = unfilteredDataReader.GetDateTime(3);
                                    if (!unfilteredDataReader.IsDBNull(4))
                                        row[UserNameColumn] = unfilteredDataReader.GetString(4);
                                    if (!unfilteredDataReader.IsDBNull(5))
                                        row[HostnameColumn] = unfilteredDataReader.GetString(5);
                                    if (!unfilteredDataReader.IsDBNull(6))
                                        row[ApplicationNameColumn] = unfilteredDataReader.GetString(6);
                                    if (!unfilteredDataReader.IsDBNull(7))
                                        row[LoginNameColumn] = unfilteredDataReader.GetString(7);
                                    if (!unfilteredDataReader.IsDBNull(8))
                                        row["ObjectID"] = unfilteredDataReader.GetInt32(8);

                                    //if an interpreter sql_text limit has been imposed (CollectionServiceConfiguration.GetCollectionServiceElement().MaxQueryMonitorEventSizeKB > -1)
                                    if (!String.IsNullOrEmpty(textData) &&
                                           CollectionServiceConfiguration.GetCollectionServiceElement().
                                               MaxQueryMonitorEventSizeKB > -1)
                                    {
                                        Encoding encoding = Encoding.Default;
                                        var size = new FileSize { Bytes = encoding.GetByteCount(textData) };
                                        if (size.Kilobytes.HasValue &&
                                            size.Kilobytes.Value >
                                            CollectionServiceConfiguration.GetCollectionServiceElement().
                                                MaxQueryMonitorEventSizeKB)
                                        {
                                            LOG.DebugFormat("Skipping activity monitor statement in filter due to length. Length is {0:N2} KB. App: {1}, Database: {2}", size.Kilobytes, row[ApplicationNameColumn] ?? "", row[DbNameColumn] ?? "");
                                            //row will not be added
                                            continue;
                                        }
                                    }

                                    //[START]SQLdm 9.1 (Gaurav Karwal): no need to put the object name at the start of text data in activity monitor. This is copied from query monitor, where it makes sense.
                                    //Over here, this line was causing the XML to get corrupted and thus blocking report was not coming up
                                    row["sql 1"] = textData;

                                    //if objectname does not contain a null
                                    /*if (!unfilteredDataReader.IsDBNull(10))
                                    {
                                        row["sql 1"] = "-- " + unfilteredDataReader.GetString(10) + "\r\n" + textData;
                                    }
                                    else
                                    {
                                        row["sql 1"] = textData;
                                    }*/
                                    //[START]SQLdm 9.1 (Gaurav Karwal): no need to put the object name at the start of text data in activity monitor. This is copied from query monitor, where it makes sense.
                                    //Over here, this line was causing the XML to get corrupted and thus blocking report was not coming up
                                    if (!unfilteredDataReader.IsDBNull(11)) row[SpidColumn] = unfilteredDataReader.GetInt32(11);
                                    if (!unfilteredDataReader.IsDBNull(12))
                                        row[StartTimeColumn] = unfilteredDataReader.GetDateTime(12);
                                    if (!unfilteredDataReader.IsDBNull(13))
                                        row["DatabaseID"] = unfilteredDataReader.GetInt32(13);

                                    unfilteredTable.Rows.Add(row);

                                    break;
                            }
                            if (goNoFurther)
                                break;
                        }
                    } //end of loop
                }
                LOG.DebugFormat("There are {0} unfiltered rows", rowCounter.ToString(CultureInfo.InvariantCulture));
                LOG.DebugFormat("There is {0} difference between the row numbers", (stopReadingAtRow - lastRowRead).ToString(CultureInfo.InvariantCulture));
                LOG.DebugFormat("There are {0} rows of non-null textdata", countNonNullTextData.ToString(CultureInfo.InvariantCulture));
                LOG.DebugFormat("The longest TextData in any one row was {0} characters", longestRowText.ToString(CultureInfo.InvariantCulture));

                //This is necessary only because we are .net 2.
                //if we were on .net 3.5 we could use filteredData = unfilteredTable.Select().CopyToArray
                // or dr.copytoarray
                if (!initializedDatatable) return null;

                //start -SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with extended events  -- all the rows should be processes since the duplication is handlled in the batch via offset
                if (!activityMonitorConfiguration.TraceMonitoringEnabled)
                {
                    lastRowRead = 0;
                    stopReadingAtRow = rowCounter + 1;
                }
                //end -SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with extended events -- all the rows should be processes since the duplication is handlled in the batch via offset


                DataRow[] dr =
                    unfilteredTable.Select(string.Format("RowNumber > {0} and RowNumber < {1} and EventClass in (137,148,92,93)",
                                                         lastRowRead.ToString(CultureInfo.InvariantCulture),
                                                         (stopReadingAtRow > lastRowRead
                                                              ? stopReadingAtRow
                                                              : currentRowRead).ToString(CultureInfo.InvariantCulture)), "RowNumber desc");

                //get the schema of the unfiltered table
                var filteredData = unfilteredTable.Clone();

                LOG.DebugFormat("There are {0} filtered rows", dr.Length.ToString(CultureInfo.InvariantCulture));

                //flood control
                var maxRow = dr.Length < activityMonitorConfiguration.RecordsPerRefreshXe
                                 ? dr.Length
                                 : activityMonitorConfiguration.RecordsPerRefreshXe;

                //import every row that matches the selection
                for (var i = 0; i < maxRow; i++) filteredData.ImportRow(dr[i]);

                filterSW.Stop();

                TimeSpan ts = filterSW.Elapsed;

                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);

                LOG.DebugFormat("Filtering took {0}", elapsedTime);

                //Return the dbdatareader of the filtered data
                return filteredData.CreateDataReader();
            }
        }
        private DbDataReader FilterActivityMonitorDataAzure(DbDataReader unfilteredDataReader)
        {
            var unfilteredDataSet = new DataSet("unfiltered");
            unfilteredDataSet.Tables.Add(new DataTable("unfilteredDataTable"));
            var unfilteredTable = unfilteredDataSet.Tables["unfilteredDataTable"];

            long lastRowRead = 0;
            long currentRowRead = 0;
            long stopReadingAtRow = 0;
            long rowCounter = 0;
            bool initializedDatatable = false;
            bool goNoFurther = false;
            int countNonNullTextData = 0;
            long longestRowText = 0;

            string flag = "";
            string lastReadFlag = "";
            using (LOG.DebugCall("FilterActivityMonitorData"))
            {
                Stopwatch filterSW = new Stopwatch();
                filterSW.Start();

                if (unfilteredDataReader != null)
                {

                    while (unfilteredDataReader.Read())
                    {
                        //if the event column is not null
                        if (!unfilteredDataReader.IsDBNull(0))
                        {
                            if (initializedDatatable == false)
                            {
                                LOG.Debug("Initializing unfiltered table");

                                //Initialize the data table
                                unfilteredTable.Columns.Add("EventClass", Type.GetType("System.Int32"));
                                unfilteredTable.Columns.Add(DurationColumn, Type.GetType("System.Int64"));
                                unfilteredTable.Columns.Add(DbNameColumn, Type.GetType("System.String"));
                                unfilteredTable.Columns.Add("EndTime", Type.GetType("System.DateTime"));
                                unfilteredTable.Columns.Add(UserNameColumn, Type.GetType("System.String"));
                                unfilteredTable.Columns.Add(HostnameColumn, Type.GetType("System.String"));
                                unfilteredTable.Columns.Add(ApplicationNameColumn, Type.GetType("System.String"));
                                unfilteredTable.Columns.Add(LoginNameColumn, Type.GetType("System.String"));
                                unfilteredTable.Columns.Add("ObjectID", Type.GetType("System.Int32"));
                                unfilteredTable.Columns.Add(TextDataColumn, Type.GetType("System.String"));
                                unfilteredTable.Columns.Add("sql 1", Type.GetType("System.String"));
                                unfilteredTable.Columns.Add(SpidColumn, Type.GetType("System.Int32"));
                                unfilteredTable.Columns.Add(StartTimeColumn, Type.GetType("System.DateTime"));
                                unfilteredTable.Columns.Add("DatabaseID", Type.GetType("System.Int32"));
                                unfilteredTable.Columns.Add("RowNumber", Type.GetType("System.Int32"));

                                DataColumn[] keys = new DataColumn[1];
                                keys[0] = unfilteredTable.Columns["RowNumber"];
                                //keys[1] = unfilteredTable.Columns["StartTime"];

                                unfilteredTable.PrimaryKey = keys;
                                initializedDatatable = true;
                            }

                            DataRow row = unfilteredTable.NewRow();
                            var eventNumber = unfilteredDataReader.GetInt32(0);

                            string textData = "";
                            if (!unfilteredDataReader.IsDBNull(9))
                            {
                                textData = unfilteredDataReader.GetString(9);

                                if (textData.Length > longestRowText) longestRowText = textData.Length;

                                //row["TextData"] = textData; Dont waste mem by putting this on every row.
                                //sql 1 is enough.  That will ultimately be compressed.
                                countNonNullTextData++;
                            }

                            rowCounter++;


                            //if this is a use configured event
                            switch (eventNumber)
                            {
                                case (int)TraceEvent.UserConfigurable0:
                                    if (!String.IsNullOrEmpty(textData))
                                    {
                                        //if the file has 7.2 flags the flags will be SQLdm not sqldm1
                                        //If this user config event is from performance monitor
                                        if (textData.Contains(lastReadFlag) || textData.Contains("SQLdm3"))
                                        {
                                            //walk up the SQLdm3 rows
                                            lastRowRead = currentRowRead;
                                            currentRowRead = rowCounter;

                                            //stop reading at the flag that was just inserted
                                            //lastrowread will be the last sqldm3 row
                                            if (textData == flag)
                                            {
                                                stopReadingAtRow = currentRowRead;
                                                LOG.DebugFormat("Flag found at row {0}, Previous flag was at row {1}", stopReadingAtRow.ToString(CultureInfo.InvariantCulture), lastRowRead.ToString(CultureInfo.InvariantCulture));

                                                goNoFurther = true;
                                            }
                                        }
                                    }
                                    break;
                                //case 12:
                                //case 41:
                                //case 43:
                                //case 45:
                                case 137:
                                case 148:
                                case 92:
                                case 93:
                                    row["RowNumber"] = rowCounter;

                                    row["EventClass"] = unfilteredDataReader.GetInt32(0);

                                    if (!unfilteredDataReader.IsDBNull(1))
                                        row[DurationColumn] = unfilteredDataReader.GetInt64(1) / 1000;
                                    if (!unfilteredDataReader.IsDBNull(2))
                                        row[DbNameColumn] = unfilteredDataReader.GetString(2);
                                    if (!unfilteredDataReader.IsDBNull(3))
                                        row["EndTime"] = unfilteredDataReader.GetDateTime(3);
                                    if (!unfilteredDataReader.IsDBNull(4))
                                        row[UserNameColumn] = unfilteredDataReader.GetString(4);
                                    if (!unfilteredDataReader.IsDBNull(5))
                                        row[HostnameColumn] = unfilteredDataReader.GetString(5);
                                    if (!unfilteredDataReader.IsDBNull(6))
                                        row[ApplicationNameColumn] = unfilteredDataReader.GetString(6);
                                    if (!unfilteredDataReader.IsDBNull(7))
                                        row[LoginNameColumn] = unfilteredDataReader.GetString(7);
                                    if (!unfilteredDataReader.IsDBNull(8))
                                        row["ObjectID"] = unfilteredDataReader.GetInt32(8);

                                    //if an interpreter sql_text limit has been imposed (CollectionServiceConfiguration.GetCollectionServiceElement().MaxQueryMonitorEventSizeKB > -1)
                                    if (!String.IsNullOrEmpty(textData) &&
                                           CollectionServiceConfiguration.GetCollectionServiceElement().
                                               MaxQueryMonitorEventSizeKB > -1)
                                    {
                                        Encoding encoding = Encoding.Default;
                                        var size = new FileSize { Bytes = encoding.GetByteCount(textData) };
                                        if (size.Kilobytes.HasValue &&
                                            size.Kilobytes.Value >
                                            CollectionServiceConfiguration.GetCollectionServiceElement().
                                                MaxQueryMonitorEventSizeKB)
                                        {
                                            LOG.DebugFormat("Skipping activity monitor statement in filter due to length. Length is {0:N2} KB. App: {1}, Database: {2}", size.Kilobytes, row[ApplicationNameColumn] ?? "", row[DbNameColumn] ?? "");
                                            //row will not be added
                                            continue;
                                        }
                                    }

                                    //[START]SQLdm 9.1 (Gaurav Karwal): no need to put the object name at the start of text data in activity monitor. This is copied from query monitor, where it makes sense.
                                    //Over here, this line was causing the XML to get corrupted and thus blocking report was not coming up
                                    row["sql 1"] = textData;

                                    //if objectname does not contain a null
                                    /*if (!unfilteredDataReader.IsDBNull(10))
                                    {
                                        row["sql 1"] = "-- " + unfilteredDataReader.GetString(10) + "\r\n" + textData;
                                    }
                                    else
                                    {
                                        row["sql 1"] = textData;
                                    }*/
                                    //[START]SQLdm 9.1 (Gaurav Karwal): no need to put the object name at the start of text data in activity monitor. This is copied from query monitor, where it makes sense.
                                    //Over here, this line was causing the XML to get corrupted and thus blocking report was not coming up
                                    if (!unfilteredDataReader.IsDBNull(11)) row[SpidColumn] = unfilteredDataReader.GetInt32(11);
                                    if (!unfilteredDataReader.IsDBNull(12))
                                        row[StartTimeColumn] = unfilteredDataReader.GetDateTime(12);
                                    if (!unfilteredDataReader.IsDBNull(13))
                                        row["DatabaseID"] = unfilteredDataReader.GetInt32(13);

                                    unfilteredTable.Rows.Add(row);

                                    break;
                            }
                            if (goNoFurther)
                                break;
                        }
                    } //end of loop
                }
                LOG.DebugFormat("There are {0} unfiltered rows", rowCounter.ToString(CultureInfo.InvariantCulture));
                LOG.DebugFormat("There is {0} difference between the row numbers", (stopReadingAtRow - lastRowRead).ToString(CultureInfo.InvariantCulture));
                LOG.DebugFormat("There are {0} rows of non-null textdata", countNonNullTextData.ToString(CultureInfo.InvariantCulture));
                LOG.DebugFormat("The longest TextData in any one row was {0} characters", longestRowText.ToString(CultureInfo.InvariantCulture));

                //This is necessary only because we are .net 2.
                //if we were on .net 3.5 we could use filteredData = unfilteredTable.Select().CopyToArray
                // or dr.copytoarray
                if (!initializedDatatable) return null;

                //start -SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with extended events  -- all the rows should be processes since the duplication is handlled in the batch via offset
                if (!activityMonitorConfiguration.TraceMonitoringEnabled)
                {
                    lastRowRead = 0;
                    stopReadingAtRow = rowCounter + 1;
                }
                //end -SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with extended events -- all the rows should be processes since the duplication is handlled in the batch via offset


                DataRow[] dr =
                    unfilteredTable.Select(string.Format("RowNumber > {0} and RowNumber < {1} and EventClass in (137,148,92,93)",
                                                         lastRowRead.ToString(CultureInfo.InvariantCulture),
                                                         (stopReadingAtRow > lastRowRead
                                                              ? stopReadingAtRow
                                                              : currentRowRead).ToString(CultureInfo.InvariantCulture)), "RowNumber desc");

                //get the schema of the unfiltered table
                var filteredData = unfilteredTable.Clone();

                LOG.DebugFormat("There are {0} filtered rows", dr.Length.ToString(CultureInfo.InvariantCulture));

                //flood control
                var maxRow = dr.Length < activityMonitorConfiguration.RecordsPerRefreshXe
                                 ? dr.Length
                                 : activityMonitorConfiguration.RecordsPerRefreshXe;

                //import every row that matches the selection
                for (var i = 0; i < maxRow; i++) filteredData.ImportRow(dr[i]);

                filterSW.Stop();

                TimeSpan ts = filterSW.Elapsed;

                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);

                LOG.DebugFormat("Filtering took {0}", elapsedTime);

                //Return the dbdatareader of the filtered data
                return filteredData.CreateDataReader();
            }
        }

        /// <summary>
        /// Interpret Activity monitor data returned by filtered, lean batch
        /// </summary>
        /// <param name="dBdataReader"></param>
        private void InterpretActivityMonitor(DbDataReader dBdataReader)
        {
            try
            {
                int countInterprettedRows = 0;
                int countDeadlocks = 0;
                int countAutoGrows = 0;
                int countBlocks = 0;
                int countEmptyStatements = 0;
                int countFiltered = 0;
                long longestRowText = 0;

                using (LOG.DebugCall("InterpretActivityMonitor"))
                {
                    //if we got no results just return
                    if (dBdataReader == null)
                    {
                        LOG.Debug("No ActivityMonitor data found.");
                        return;
                    }

                    DataTable tBlocking = new DataTable("BlockingTable");
                    List<int> blockedSpids = new List<int>();

                    tBlocking.Columns.Add(new DataColumn("spid", Type.GetType("System.Int32")));
                    tBlocking.Columns.Add(new DataColumn("application", Type.GetType("System.String")));
                    tBlocking.Columns.Add(new DataColumn("host", Type.GetType("System.String")));
                    tBlocking.Columns.Add(new DataColumn("login", Type.GetType("System.String")));
                    tBlocking.Columns.Add(new DataColumn("databasename", Type.GetType("System.String")));
                    tBlocking.Columns.Add(new DataColumn("objectid", Type.GetType("System.Int32")));
                    tBlocking.Columns.Add(new DataColumn("requestmode", Type.GetType("System.String")));
                    tBlocking.Columns.Add(new DataColumn("blockingtimems", Type.GetType("System.Int64")));
                    tBlocking.Columns.Add(new DataColumn("inputbuffer", Type.GetType("System.String")));
                    tBlocking.Columns.Add(new DataColumn("blockingstarttimeutc", Type.GetType("System.DateTime")));
                    tBlocking.Columns.Add(new DataColumn("blockingxact", Type.GetType("System.Int64")));
                    tBlocking.Columns.Add(new DataColumn("blockedxact", Type.GetType("System.Int64")));
                    tBlocking.Columns.Add(new DataColumn("blockingLastBatch", Type.GetType("System.DateTime")));

                    DataColumn[] keys = new DataColumn[2];
                    keys[0] = tBlocking.Columns["spid"];
                    keys[1] = tBlocking.Columns["blockingtimems"];

                    tBlocking.PrimaryKey = keys;

                    do//each resultset
                    {
                        Dictionary<long, string> databaseNames = null;

                        while (dBdataReader.Read())//each row
                        {
                            countInterprettedRows++;

                            if (!dBdataReader.IsDBNull(0))
                            {
                                var statement = new ActivityMonitorStatement();

                                if (dBdataReader.GetInt32(0) == (int)TraceEvent.DeadlockGraph)
                                {
                                    if (databaseNames == null || (databaseNames != null && databaseNames.Count < refresh.DbStatistics.Count))
                                    {
                                        databaseNames = new Dictionary<long, string>();
                                        foreach (DatabaseStatistics d in refresh.DbStatistics.Values)
                                        {
                                            if (!databaseNames.ContainsKey(d.DatabaseId))
                                                databaseNames.Add(d.DatabaseId, d.Name);
                                        }
                                    }
                                    countDeadlocks++;
                                    ReadDeadlockData(dBdataReader, databaseNames);
                                    continue;
                                }

                                if (dBdataReader.GetInt32(0) == (int)TraceEvent.DataFileAutoGrow ||
                                    dBdataReader.GetInt32(0) == (int)TraceEvent.LogFileAutoGrow)
                                {
                                    countAutoGrows++;
                                    ReadAutogrowData(dBdataReader);
                                    continue;
                                }

                                if (dBdataReader.GetInt32(0) == (int)TraceEvent.BlockedProcessReport)
                                {
                                    countBlocks++;

                                    if (databaseNames == null || (databaseNames != null && databaseNames.Count < refresh.DbStatistics.Count))
                                    {
                                        databaseNames = new Dictionary<long, string>();
                                        foreach (DatabaseStatistics d in refresh.DbStatistics.Values)
                                        {
                                            if (!databaseNames.ContainsKey(d.DatabaseId))
                                                databaseNames.Add(d.DatabaseId, d.Name);
                                        }
                                    }

                                    if (!dBdataReader.IsDBNull(8))
                                        statement.ObjectID = dBdataReader.GetInt32(8);

                                    //Sometimes databaseName does not have all of the databases in it. Why?
                                    ReadBlockingSessionData(dBdataReader, databaseNames, statement, ref tBlocking);
                                    continue;
                                }

                                // Set the duration - in this case, we will treat null as zero
                                statement.Duration =
                                    TimeSpan.FromMilliseconds(dBdataReader.IsDBNull(1) ? 0 : dBdataReader.GetInt64(1));

                                statement.Database = dBdataReader.IsDBNull(2) ? null : dBdataReader.GetString(2);

                                if (!dBdataReader.IsDBNull(3))
                                    statement.CompletionTime = dBdataReader.GetDateTime(3);

                                statement.NtUser = dBdataReader.IsDBNull(4) ? null : dBdataReader.GetString(4);
                                statement.Client = dBdataReader.IsDBNull(5) ? null : dBdataReader.GetString(5);

                                statement.AppName = dBdataReader.IsDBNull(6) ? null : dBdataReader.GetString(6);
                                statement.SqlUser = dBdataReader.IsDBNull(7) ? null : dBdataReader.GetString(7);

                                //sql 1
                                string decompressedSQLText = dBdataReader.IsDBNull(10) ? null : dBdataReader.GetString(10);

                                if (String.IsNullOrEmpty(decompressedSQLText))
                                {
                                    countEmptyStatements++;
                                }
                                else
                                {
                                    if (decompressedSQLText.Length > longestRowText)
                                        longestRowText = decompressedSQLText.Length;
                                }

                                statement.SqlText = decompressedSQLText; //Compress the text

                                if (!dBdataReader.IsDBNull(11))
                                    statement.Spid = dBdataReader.GetInt32(11);

                                if (!String.IsNullOrEmpty(decompressedSQLText))
                                {
                                    string signatureHash = SqlParsingHelper.GetSignatureHash(decompressedSQLText);

                                    statement.SignatureHash = signatureHash;
                                    if (!refresh.QueryMonitorSignatures.ContainsKey(signatureHash))
                                    {
                                        string signature = SqlParsingHelper.GetReadableSignature(decompressedSQLText);
                                        byte[] bytes = Serialized<string>.SerializeCompressed<string>(signature);
                                        refresh.QueryMonitorSignatures.Add(signatureHash, bytes);
                                    }
                                    refresh.QueryMonitorStatements.Add(statement.QueryMonitorStatement);
                                }
                            }
                        }
                    } while (dBdataReader.NextResult());

                    List<long> leadBlockers = FindLeadBlockers(tBlocking);
                    List<long> sessionsToDelete = new List<long>();

                    //get hashes of non-lead blockers
                    foreach (BlockingSessionInfo i in refresh.BlockingSessions.Values)
                    {
                        //get a comprehensive list of blocked spids
                        if (!blockedSpids.Contains(i.BlockedSession.SessionId)) blockedSpids.Add(i.BlockedSession.SessionId);
                        if (!leadBlockers.Contains(i.XActId)) sessionsToDelete.Add(i.XActId);
                    }
                    //delete the non-leads
                    //foreach (int sessionHash in sessionsToDelete) refresh.BlockingSessions.Remove(sessionHash);
                    refresh.BlockedSpids = blockedSpids;

                    LOG.DebugFormat("{0} Autogrows", countAutoGrows.ToString(CultureInfo.InvariantCulture));
                    LOG.DebugFormat("{0} Blocked Sessions with {1} lead blockers", countBlocks.ToString(CultureInfo.InvariantCulture), leadBlockers.Count);
                    LOG.DebugFormat("{0} Deadlocks", countDeadlocks.ToString(CultureInfo.InvariantCulture));
                    LOG.DebugFormat("{0} statement texts are null or empty", countEmptyStatements.ToString(CultureInfo.InvariantCulture));
                    LOG.DebugFormat("{0} rows were interpretted", countInterprettedRows.ToString(CultureInfo.InvariantCulture));
                    LOG.DebugFormat("{0} rows were filtered out (other than empty sqltext)", countFiltered.ToString(CultureInfo.InvariantCulture));
                    LOG.DebugFormat("The longest TextData on any row in this filtered resultset was {0} characters", longestRowText.ToString(CultureInfo.InvariantCulture));

                    if (refresh.QueryMonitorStatements.Count > 0)
                    {
                        LOG.Debug(refresh.QueryMonitorStatements.Count + " query monitor statements read.");
                    }
                }
            }
            catch (Exception exception)
            {
                refresh.ActivityMonitorError = exception;
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Read ActivityMonitor failed: {0}", exception, true);
                return;
            }
        }

        /// <summary>
        /// Find all of the lead blockers and return them
        /// </summary>
        /// <param name="sourceTable"></param>
        /// <returns></returns>
        private List<long> FindLeadBlockers(DataTable sourceTable)
        {

            //DataTable OldestSessions = new DataTable();
            DataTable LeadBlockers = sourceTable.Clone();

            //find the maximum wait for each blocking session (unique blocking and blocked xact)
            var sessions = sourceTable.DefaultView.ToTable(true, new string[] { "blockingxact", "blockedxact" });

            sessions.Columns.Add("blockingtimems");

            List<long> blocking = new List<long>();
            List<long> blocked = new List<long>();
            List<long> leadBlockers = new List<long>();

            for (var i = 0; i < sessions.Rows.Count; i++)
            {
                object result = sourceTable.Compute("max(blockingtimems)",
                              string.Format("[blockingxact] = {0} and [blockedxact] = {1}",
                                            sessions.Rows[i][0].ToString(),
                                            sessions.Rows[i][1].ToString()));

                blocking.Add((long)sessions.Rows[i][0]);
                blocked.Add((long)sessions.Rows[i][1]);

                DataRow[] row = sessions.Select(string.Format("[blockingxact] = {0} and [blockedxact] = {1}",
                                            sessions.Rows[i][0].ToString(),
                                            sessions.Rows[i][1].ToString()));

                //now save the max blocking time to the relevant row
                for (var rowPtr = 0; rowPtr < row.Length; rowPtr++) { row[rowPtr]["blockingtimems"] = result; }

            }
            //sessions now contains the key for the oldest sessions
            //choose those that are blocking but not blocked
            foreach (long blockingxact in blocking)
            {
                if (!blocked.Contains(blockingxact))
                {
                    leadBlockers.Add(blockingxact);
                }
            }

            //foreach(int leadBlocker in leadBlockers)
            //{
            //    var leadBlockRows = sessions.Select(string.Format("[blockingxact] = {0}", leadBlocker));

            //    for(int leadBlockerRow = 0; leadBlockerRow < leadBlockRows.Length; leadBlockerRow++)
            //    {
            //        LeadBlockers.Rows.Add(leadBlockRows[leadBlockerRow]);
            //    }
            //}
            return leadBlockers;
        }

        private void InterpretActivityMonitor(SqlDataReader dataReader)
        {
            try
            {

                do
                {
                    Dictionary<long, string> databaseNames = null;

                    while (dataReader.Read())
                    {
                        string decompressedSQLText = null;

                        if (!dataReader.IsDBNull(0))
                        {
                            QueryMonitorStatement statement = new QueryMonitorStatement();

                            if (dataReader.GetInt32(0) == (int)TraceEvent.DeadlockGraph)
                            {
                                if (databaseNames == null)
                                {
                                    databaseNames = new Dictionary<long, string>();
                                    foreach (DatabaseStatistics d in refresh.DbStatistics.Values)
                                    {
                                        if (!databaseNames.ContainsKey(d.DatabaseId))
                                            databaseNames.Add(d.DatabaseId, d.Name);
                                    }
                                }
                                ReadDeadlockData(dataReader, databaseNames);
                                continue;
                            }

                            if (dataReader.GetInt32(0) == (int)TraceEvent.DataFileAutoGrow ||
                                dataReader.GetInt32(0) == (int)TraceEvent.LogFileAutoGrow)
                            {
                                ReadAutogrowData(dataReader);
                                continue;
                            }

                            if (dataReader.GetInt32(0) == (int)TraceEvent.BlockedProcessReport)
                            {
                                LOG.Debug("Found a BLOCKED process report");
                                continue;
                            }


                            // Set the statement type based on event class
                            switch (dataReader.GetInt32(0))
                            {
                                case (int)TraceEvent.SQLBatchCompleted:
                                    statement.StatementType = WorstPerformingStatementType.Batch;
                                    break;
                                case (int)TraceEvent.SQLStmtCompleted:
                                    statement.StatementType = WorstPerformingStatementType.SingleStatement;
                                    break;
                                default:
                                    statement.StatementType = WorstPerformingStatementType.StoredProcedure;
                                    break;
                            }

                            // Set the duration - in this case, we will treat null as zero
                            statement.Duration =
                                TimeSpan.FromMilliseconds(dataReader.IsDBNull(1) ? 0 : dataReader.GetInt64(1));

                            if (!dataReader.IsDBNull(2))
                                statement.CompletionTime = dataReader.GetDateTime(2);

                            statement.Database = dataReader.IsDBNull(3) ? null : dataReader.GetString(3);
                            statement.NtUser = dataReader.IsDBNull(4) ? null : dataReader.GetString(4);
                            statement.Client = dataReader.IsDBNull(5) ? null : dataReader.GetString(5);
                            statement.AppName = dataReader.IsDBNull(6) ? null : dataReader.GetString(6);
                            statement.SqlUser = dataReader.IsDBNull(7) ? null : dataReader.GetString(7);

                            if (!dataReader.IsDBNull(8))
                                statement.Reads = dataReader.GetInt64(8);

                            if (!dataReader.IsDBNull(9))
                                statement.Writes = dataReader.GetInt64(9);

                            if (!dataReader.IsDBNull(10))
                                statement.CpuTime = TimeSpan.FromMilliseconds(dataReader.GetInt64(10));

                            decompressedSQLText = dataReader.IsDBNull(11) ? null : dataReader.GetString(11);

                            if (!String.IsNullOrEmpty(decompressedSQLText) &&
                                CollectionServiceConfiguration.GetCollectionServiceElement().
                                    MaxQueryMonitorEventSizeKB > -1)
                            {
                                Encoding encoding = Encoding.Default;
                                FileSize size = new FileSize();
                                size.Bytes = encoding.GetByteCount(decompressedSQLText);
                                if (size != null && size.Kilobytes.HasValue &&
                                    size.Kilobytes.Value >
                                    CollectionServiceConfiguration.GetCollectionServiceElement().
                                        MaxQueryMonitorEventSizeKB)
                                {
                                    LOG.WarnFormat("Skipping query monitor statement due to length. Length is {0:N2} KB. App: {1}, Database: {2}", size.Kilobytes, statement.AppName, statement.Database);
                                    continue;
                                }
                            }

                            statement.SqlText = decompressedSQLText; //Compress the text

                            if (!dataReader.IsDBNull(12))
                                statement.Spid = dataReader.GetInt32(12);


                            //if (queryMonitorConfiguration.CpuUsageFilter != null)
                            //{
                            //    if (statement.CpuTime == null ||
                            //        refresh.MonitoredServer.QueryMonitorConfiguration.CpuUsageFilter.
                            //            TotalMilliseconds >
                            //        statement.CpuTime.Value.TotalMilliseconds)
                            //        continue;
                            //}

                            //if (queryMonitorConfiguration.DurationFilter != null)
                            //{
                            //    if (statement.Duration == null ||
                            //        refresh.MonitoredServer.QueryMonitorConfiguration.DurationFilter.
                            //            TotalMilliseconds >
                            //        statement.Duration.Value.TotalMilliseconds)
                            //        continue;
                            //}

                            //if (queryMonitorConfiguration.LogicalDiskReads != null)
                            //{
                            //    if (statement.Reads == null ||
                            //        refresh.MonitoredServer.QueryMonitorConfiguration.LogicalDiskReads >
                            //        statement.Reads)
                            //        continue;
                            //}

                            //if (queryMonitorConfiguration.PhysicalDiskWrites != null)
                            //{
                            //    if (statement.Writes == null ||
                            //        refresh.MonitoredServer.QueryMonitorConfiguration.PhysicalDiskWrites >
                            //        statement.Writes)
                            //        continue;
                            //}

                            //if (queryMonitorConfiguration.AdvancedConfiguration.ApplicationExcludeLike != null)
                            //{
                            //    if (AlertFilterHelper.IsLikeValue(
                            //        queryMonitorConfiguration.AdvancedConfiguration.
                            //            ApplicationExcludeLike,
                            //        statement.AppName))
                            //        continue;
                            //}

                            //if (queryMonitorConfiguration.AdvancedConfiguration.ApplicationExcludeMatch != null)
                            //{
                            //    if (AlertFilterHelper.IsMatchValue(
                            //        queryMonitorConfiguration.AdvancedConfiguration.
                            //            ApplicationExcludeMatch,
                            //        statement.AppName))
                            //        continue;
                            //}

                            //if (queryMonitorConfiguration.AdvancedConfiguration.DatabaseExcludeLike != null)
                            //{
                            //    if (AlertFilterHelper.IsLikeValue(
                            //        queryMonitorConfiguration.AdvancedConfiguration.
                            //            DatabaseExcludeLike,
                            //        statement.Database))
                            //        continue;
                            //}

                            //if (queryMonitorConfiguration.AdvancedConfiguration.DatabaseExcludeMatch != null)
                            //{
                            //    if (AlertFilterHelper.IsMatchValue(
                            //        queryMonitorConfiguration.AdvancedConfiguration.
                            //            DatabaseExcludeMatch,
                            //        statement.Database))
                            //        continue;
                            //}

                            //if (queryMonitorConfiguration.AdvancedConfiguration.SqlTextExcludeLike != null)
                            //{
                            //    if (AlertFilterHelper.IsLikeValue(
                            //        queryMonitorConfiguration.AdvancedConfiguration.
                            //            SqlTextExcludeLike,
                            //        decompressedSQLText))
                            //        continue;
                            //}

                            //if (queryMonitorConfiguration.AdvancedConfiguration.SqlTextExcludeMatch != null)
                            //{
                            //    if (AlertFilterHelper.IsMatchValue(
                            //        queryMonitorConfiguration.AdvancedConfiguration.
                            //            SqlTextExcludeMatch,
                            //        decompressedSQLText))
                            //        continue;
                            //}


                            if (!String.IsNullOrEmpty(decompressedSQLText))
                            {
                                string signatureHash = SqlParsingHelper.GetSignatureHash(decompressedSQLText);

                                statement.SignatureHash = signatureHash;
                                if (!refresh.QueryMonitorSignatures.ContainsKey(signatureHash))
                                {
                                    string signature = SqlParsingHelper.GetReadableSignature(decompressedSQLText);
                                    byte[] bytes = Serialized<string>.SerializeCompressed<string>(signature);
                                    refresh.QueryMonitorSignatures.Add(signatureHash, bytes);
                                }
                                refresh.QueryMonitorStatements.Add(statement);
                            }
                        }
                    }
                } while (dataReader.NextResult());

                if (refresh.QueryMonitorStatements.Count > 0)
                {
                    LOG.Debug(refresh.QueryMonitorStatements.Count + " ActivityMonitor statements read.");
                }
            }
            catch (Exception exception)
            {
                refresh.ActivityMonitorError = exception;
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Read ActivityMonitor failed: {0}", exception, true);
                return;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="databaseNames"></param>
        private void ReadBlockingSessionData(DbDataReader dataReader, Dictionary<long, string> databaseNames, ActivityMonitorStatement statement, ref DataTable tBlocking)
        {
            if (!dataReader.IsDBNull(10))
            {
                DateTime? startTime = null;
                DateTime endTime = DateTime.MinValue;

                if (!dataReader.IsDBNull(12)) startTime = dataReader.GetDateTime(12);
                if (!dataReader.IsDBNull(3)) endTime = dataReader.GetDateTime(3);

                var blockingInfo = new BlockingSessionInfo(dataReader.GetString(10), databaseNames, startTime, endTime);
                blockingInfo.ObjectID = statement.ObjectID;

                DataRow row = tBlocking.NewRow();
                row["spid"] = blockingInfo.SessionId;
                row["application"] = blockingInfo.ClientApp;
                row["host"] = blockingInfo.Host;
                row["login"] = blockingInfo.Login;
                row["databasename"] = blockingInfo.DatabaseName;
                row["objectid"] = statement.ObjectID;
                row["requestmode"] = blockingInfo.BlockedSession.RequestMode;
                row["blockingtimems"] = blockingInfo.BlockedSession.BlockingTimems;
                row["inputbuffer"] = blockingInfo.InputBuffer;
                row["blockingstarttimeutc"] = blockingInfo.BlockingStartTimeUtc;
                row["blockingxact"] = blockingInfo.XActId;
                row["blockedxact"] = blockingInfo.BlockedSession.XActId;
                if (blockingInfo.LastBatchCompleted.HasValue)
                    row["blockingLastBatch"] = blockingInfo.LastBatchCompleted;
                else
                    row["blockingLastBatch"] = DBNull.Value;

                var searchOn = new object[2];
                searchOn[0] = row["spid"];
                searchOn[1] = row["blockingtimems"];

                if (tBlocking.Rows.Find(searchOn) == null)
                    tBlocking.Rows.Add(row);

                //does not contain the blocking then add it (inludes the blocked)
                if (!refresh.BlockingSessions.ContainsKey(blockingInfo.XActId))
                {
                    refresh.BlockingSessions.Add(blockingInfo.XActId, blockingInfo);
                }
            }
        }
        ///// <summary>
        ///// Lean batch
        ///// </summary>
        ///// <param name="dataReader"></param>
        ///// <param name="databaseNames"></param>
        //private void ReadDeadlockData(DbDataReader dataReader, Dictionary<long, string> databaseNames)
        //{
        //    if (!dataReader.IsDBNull(12))
        //    {
        //        DateTime? startTime = null;
        //        if (!dataReader.IsDBNull(14))
        //            startTime = dataReader.GetDateTime(14);
        //        DeadlockInfo deadlockInfo = new DeadlockInfo(dataReader.GetString(12), databaseNames, startTime);
        //        refresh.Deadlocks.Add(deadlockInfo);
        //    }
        //}

        //private void ReadDeadlockData(SqlDataReader dataReader, Dictionary<long, string> databaseNames)
        //{
        //    if (!dataReader.IsDBNull(11))
        //    {
        //        DateTime? startTime = null;
        //        if (!dataReader.IsDBNull(13))
        //            startTime = dataReader.GetDateTime(13);
        //        DeadlockInfo deadlockInfo = new DeadlockInfo(dataReader.GetString(11), databaseNames, startTime);
        //        refresh.Deadlocks.Add(deadlockInfo);
        //    }
        //}

        ///// <summary>
        ///// Lean batch
        ///// </summary>
        ///// <param name="dataReader"></param>
        //private void ReadAutogrowData(DbDataReader dataReader)
        //{
        //    int eventId =
        //    dataReader.GetInt32(0);
        //    string dbName = dataReader.IsDBNull(2) ? null : dataReader.GetString(2);

        //    if (dbName == null || refresh.DbStatistics == null)
        //        return;

        //    if (!refresh.DbStatistics.ContainsKey(dbName))
        //    {
        //        int dbid = dataReader.IsDBNull(16) ? 0 : dataReader.GetInt32(16);
        //        refresh.DbStatistics.Add(dbName, new DatabaseStatistics(refresh.ServerName, dbName, dbid));
        //    }
        //    switch (eventId)
        //    {
        //        case (int)TraceEvent.LogFileAutoGrow:
        //            refresh.DbStatistics[dbName].LogAutogrowDetected = true;
        //            break;
        //        case (int)TraceEvent.DataFileAutoGrow:
        //            refresh.DbStatistics[dbName].DataAutogrowDetected = true;
        //            break;
        //    }
        //}



        //private void ReadAutogrowData(SqlDataReader dataReader)
        //{
        //    int eventId =
        //    dataReader.GetInt32(0);
        //    string dbName = dataReader.IsDBNull(3) ? null : dataReader.GetString(3);

        //    if (dbName == null || refresh.DbStatistics == null)
        //        return;

        //    if (!refresh.DbStatistics.ContainsKey(dbName))
        //    {
        //        int dbid = dataReader.IsDBNull(14) ? 0 : dataReader.GetInt32(14);
        //        refresh.DbStatistics.Add(dbName, new DatabaseStatistics(refresh.ServerName, dbName, dbid));
        //    }
        //    switch (eventId)
        //    {
        //        case (int)TraceEvent.LogFileAutoGrow:
        //            refresh.DbStatistics[dbName].LogAutogrowDetected = true;
        //            break;
        //        case (int)TraceEvent.DataFileAutoGrow:
        //            refresh.DbStatistics[dbName].DataAutogrowDetected = true;
        //            break;
        //    }
        //}

        #endregion

        #endregion

        #region Error Logs

        #region Error Logs Collector Methods

        //START : SQLDM 10.3 (Manali Hukkeri): Technical debt changes
        /// <summary>
        /// Starts the Log Scan variables store collector.
        /// </summary>
        private void StartLogScanVariableStoreCollector()
        {
            StartGenericCollector(new Collector(LogScanVariableStoreCollector), refresh, "StartLogScanVariableStoreCollector", "Log Scan variable store", LogScanVariableStoreCallback, new object[] { });
        }

        /// <summary>
        /// Define the LogScan store variable collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void LogScanVariableStoreCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd = SqlCommandBuilder.BuildLogScanVariables(conn, previousRefresh, refresh, this.cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollectionNonQueryExecution(new EventHandler<CollectorCompleteEventArgs>(LogScanVariableStoreCallback));
        }


        /// <summary>
        /// Callback used to process the data returned from the LogScan variable store collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void LogScanVariableStoreCallback(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing LogScanVariableStoreCallback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, LogScanVariableStoreCallback, e);
                return;
            }

            NextCollector nextCollector = new NextCollector(StartLogScanCollector);
            GenericCallback(new CollectorCallback(LogScanVariableStoreCallback),
                            refresh,
                            "LogScanVariableStoreCallback",
                            "Log Scan Variable store",
                            nextCollector,
                            sender,
                            e);
        }


        /// <summary>
        /// Define the LogScan variable store callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void LogScanVariableStoreCallback(CollectorCompleteEventArgs e)
        {
            // Callback used to store Logscan variables required for logging purposes only
            if (e.Result == Result.Success)
            {
                LOG.Verbose("LogScanVariableStoreCallback() successfully completed");
            }
            else
            {
                var exceptionMessage = "LogScanVariableStoreCallback() store Logscan variables failed ";
                if (e.Exception != null)
                {
                    exceptionMessage += e.Exception.ToString();
                }
                LOG.Error(exceptionMessage);
            }
        }

        //END : SQLDM 10.3 (Manali Hukkeri): Technical debt changes

        /// <summary>
        /// Define the LogScan collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void LogScanCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildLogScanCommand(conn, ver, workload, cloudProviderId);
            //(DateTime)(previousRefresh != null ? previousRefresh.TimeStampLocal.HasValue ? previousRefresh.TimeStampLocal : refresh.TimeStampLocal : refresh.TimeStampLocal));
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(LogScanCallback));
        }

        /// <summary>
        /// Starts the Log Scan collector.
        /// </summary>
        private void StartLogScanCollector()
        {
            StartGenericCollector(new Collector(LogScanCollector), refresh, "StartLogScanCollector", "Log Scan", LogScanCallback, new object[] { });
        }

        /// <summary>
        /// Define the LogScan callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void LogScanCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretLogScan(rd);
            }
        }

        /// <summary>
        /// Callback used to process the data returned from the LogScan collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void LogScanCallback(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing LogScanCallback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, LogScanCallback, e);
                return;
            }

            NextCollector nextCollector = new NextCollector(StartOSMetricsCollector);
            if (refresh.ProductVersion.Major > 8)
            {
                // avoid to chain the StartMirrorMetricsUpdateCollector collector since it is not compatible with AZURE  https://docs.microsoft.com/en-us/azure/sql-database/sql-database-features
                if (cloudProviderId != CLOUD_PROVIDER_ID_AZURE)
                {
                    nextCollector = new NextCollector(StartMirrorMetricsUpdateCollector);
                }
            }

            GenericCallback(new CollectorCallback(LogScanCallback),
                            refresh,
                            "LogScanCallback",
                            "Log Scan",
                            nextCollector,
                            sender,
                            e);
        }

        #endregion

        #region Error Logs Interpretation Methods

        /// <summary>
        /// Interpret LogScan data
        /// </summary>
        private void InterpretLogScan(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretLogScan"))
            {
                try
                {
                    int expectedAgentLogs = 0;
                    int expectedErrorLogs = 0;
                    long errorLogSizeBytes = 0;
                    long agentLogSizeBytes = 0;
                    int currentAgentLog = 0;
                    int currentErrorLog = 0;

                    while (dataReader.Read())
                    {
                        if (dataReader.FieldCount > 2)
                        {
                            if (!dataReader.IsDBNull(0))
                            {
                                switch (dataReader.GetInt32(0))
                                {
                                    case (int)LogFileType.Agent:
                                        expectedAgentLogs = !dataReader.IsDBNull(1) ? dataReader.GetInt32(1) : 0;
                                        if (dataReader.FieldCount > 3) //2005+
                                        {
                                            agentLogSizeBytes = !dataReader.IsDBNull(3) ? dataReader.GetInt64(3) : 0;
                                        }
                                        else
                                        {
                                            agentLogSizeBytes = !dataReader.IsDBNull(2) ? dataReader.GetInt32(2) : 0;
                                        }
                                        break;
                                    case (int)LogFileType.SQLServer:
                                        expectedErrorLogs = !dataReader.IsDBNull(1) ? dataReader.GetInt32(1) : 0;
                                        if (dataReader.FieldCount > 3) //2005+
                                        {
                                            errorLogSizeBytes = !dataReader.IsDBNull(3) ? dataReader.GetInt64(3) : 0;
                                        }
                                        else
                                        {
                                            errorLogSizeBytes = !dataReader.IsDBNull(2) ? dataReader.GetInt32(2) : 0;
                                        }
                                        break;
                                }
                            }
                        }
                    }

                    try
                    {
                        AdvancedAlertConfigurationSettings errorLogAdvancedSettings = null;
                        errorLogAdvancedSettings =
                                ScheduledCollectionEventProcessor.GetAdvancedConfiguration(
                                    workload.Thresholds[(int)Metric.ErrorLog]);
                        List<Regex> errorLogRegexCritical = new List<Regex>();
                        List<Regex> errorLogRegexWarning = new List<Regex>();
                        List<Regex> errorLogRegexInfo = new List<Regex>();

                        //if the alert is enabled and in excess of the set limit
                        if (workload.Thresholds[(int)Metric.ErrorLog].IsEnabled && errorLogSizeBytes >= errorLogAdvancedSettings.LogSizeLimit * 1024)
                        {
                            refresh.Alerts.ErrorLogFileSizeInBytes = errorLogSizeBytes;
                            refresh.Alerts.ErrorLogMostSevereMatch = MonitoredState.Warning;
                        }
                        else
                        {
                            refresh.Alerts.ErrorLogFileSizeInBytes = -1;

                            if (expectedErrorLogs > 0 && errorLogAdvancedSettings != null && workload.Thresholds[(int)Metric.ErrorLog].IsEnabled)
                            {
                                if (errorLogAdvancedSettings.LogIncludeRegexCritical != null)
                                    foreach (String regexstr in errorLogAdvancedSettings.LogIncludeRegexCritical)
                                    {
                                        errorLogRegexCritical.Add(new Regex(regexstr));
                                    }
                                if (errorLogAdvancedSettings.LogIncludeRegexWarning != null)
                                    foreach (String regexstr in errorLogAdvancedSettings.LogIncludeRegexWarning)
                                    {
                                        errorLogRegexWarning.Add(new Regex(regexstr));
                                    }
                                if (errorLogAdvancedSettings.LogIncludeRegexInfo != null)
                                    foreach (String regexstr in errorLogAdvancedSettings.LogIncludeRegexInfo)
                                    {
                                        errorLogRegexInfo.Add(new Regex(regexstr));
                                    }
                            }
                        }



                        AdvancedAlertConfigurationSettings agentLogAdvancedSettings = null;
                        agentLogAdvancedSettings =
                                ScheduledCollectionEventProcessor.GetAdvancedConfiguration(
                                    workload.Thresholds[(int)Metric.AgentLog]);
                        List<Regex> agentLogRegexCritical = new List<Regex>();
                        List<Regex> agentLogRegexWarning = new List<Regex>();
                        List<Regex> agentLogRegexInfo = new List<Regex>();

                        //if the alert is enabled and in excess of the set limit
                        if (workload.Thresholds[(int)Metric.AgentLog].IsEnabled && agentLogSizeBytes >= agentLogAdvancedSettings.LogSizeLimit * 1024)
                        {
                            refresh.Alerts.AgentLogFileSizeInBytes = agentLogSizeBytes;
                            refresh.Alerts.AgentLogMostSevereMatch = MonitoredState.Warning;
                        }
                        else
                        {
                            refresh.Alerts.AgentLogFileSizeInBytes = -1;

                            if (agentLogAdvancedSettings.LogIncludeRegexCritical != null)
                                foreach (String regexstr in agentLogAdvancedSettings.LogIncludeRegexCritical)
                                {
                                    agentLogRegexCritical.Add(new Regex(regexstr));
                                }
                            if (agentLogAdvancedSettings.LogIncludeRegexWarning != null)
                                foreach (String regexstr in agentLogAdvancedSettings.LogIncludeRegexWarning)
                                {
                                    agentLogRegexWarning.Add(new Regex(regexstr));
                                }
                            if (agentLogAdvancedSettings.LogIncludeRegexInfo != null)
                                foreach (String regexstr in agentLogAdvancedSettings.LogIncludeRegexInfo)
                                {
                                    agentLogRegexInfo.Add(new Regex(regexstr));
                                }
                        }

                        while (dataReader.NextResult())
                        {
                            if (dataReader.Read())
                            {
                                if (dataReader.FieldCount == 1)
                                {
                                    string message = dataReader.GetValue(0).ToString();
                                    if (message.ToLower() != "no events to read")
                                    {
                                        message = String.Format(
                                            @"The log scan could not be completed.  Reason: {0}  
Last Agent Log Read: {1} 
Last Error Log Read: {2}",
                                            message,
                                            currentAgentLog,
                                            currentErrorLog);
                                        refresh.Alerts.LogScanFailure = new Exception(message);
                                        LOG.Warn(message);
                                    }
                                }
                                else
                                {
                                    if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(1))
                                    {
                                        switch (dataReader.GetInt32(0))
                                        {
                                            case (int)LogFileType.Agent:
                                                currentAgentLog = dataReader.GetInt32(1);
                                                //only do this if there as not an excess file size alert
                                                if (refresh.Alerts.AgentLogFileSizeInBytes == -1)
                                                {
                                                    ReadAgentLog(dataReader,
                                                                 agentLogRegexCritical,
                                                                 agentLogRegexWarning,
                                                                 agentLogRegexInfo,
                                                                 agentLogAdvancedSettings);
                                                }
                                                break;
                                            case (int)LogFileType.SQLServer:
                                                currentErrorLog = dataReader.GetInt32(1);
                                                //only do this if there as not an excess file size alert
                                                if (refresh.Alerts.ErrorLogFileSizeInBytes == -1)
                                                {
                                                    ReadErrorLog(dataReader,
                                                                 errorLogRegexCritical,
                                                                 errorLogRegexWarning,
                                                                 errorLogRegexInfo,
                                                                 errorLogAdvancedSettings);
                                                }
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        LOG.Warn("Null value encountered while interpreting Log Scan.");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        string message = e.Message;
                        message = String.Format(
                              @"The log scan could not be completed.  Reason: {0}  
Last Agent Log Read: {1} 
Last Error Log Read: {2}",
                            message,
                            currentAgentLog,
                            currentErrorLog);
                        message = message + "\nStack Trace:" + e.StackTrace;
                        refresh.Alerts.LogScanFailure = new Exception(message);
                        LOG.Warn(message);
                    }
                }
                catch (Exception e)
                {
                    string message = e.Message;
                    message = String.Format(
                        "The log scan could not be completed.  Reason: {0}",
                        message);
                    message = message + "\nStack Trace:" + e.StackTrace;
                    refresh.Alerts.LogScanFailure = new Exception(message);
                    LOG.Warn(message);
                }
                finally
                {
                    PersistenceManager.Instance.SetLogScanDate(refresh.Alerts.LogScanDate);
                }
            }
        }

        private void ReadAgentLog(SqlDataReader dataReader, List<Regex> agentLogRegexCritical, List<Regex> agentLogRegexWarning,
                                  List<Regex> agentLogRegexInfo, AdvancedAlertConfigurationSettings advancedSettings)
        {
            DateTime? lastScanDate = DateTime.MinValue;
            // If we're picking up events that were covered by the previous refresh, skip them
            if (previousRefresh != null && previousRefresh.Alerts.LogScanDate.HasValue)
                lastScanDate = previousRefresh.Alerts.LogScanDate.Value;
            else
            {
                lastScanDate = PersistenceManager.Instance.GetLogScanDate();
            }

            if (refresh.ProductVersion.Major >= 9) // Use same method for reading SQL Server 2005/2008
            {
                ReadAgentLog2005(dataReader, agentLogRegexCritical, agentLogRegexWarning, agentLogRegexInfo, advancedSettings, lastScanDate);
            }
            else
            {
                ReadAgentLog2000(dataReader, agentLogRegexCritical, agentLogRegexWarning, agentLogRegexInfo, advancedSettings, lastScanDate);
            }
        }

        private void ReadAgentLog2005(SqlDataReader dataReader, List<Regex> agentLogRegexCritical,
                                      List<Regex> agentLogRegexWarning, List<Regex> agentLogRegexInfo,
                                      AdvancedAlertConfigurationSettings advancedSettings, DateTime? lastScanDate)
        {

            DateTime? eventTime = null;
            String eventString = null;
            String eventSpid = null;
            int? eventSeverity = null;
            String previousString;
            bool previousStringMatched = false;
            bool previousStringUsedAsFooter = false;
            MonitoredState severity = MonitoredState.OK;
            MonitoredState previousSeverity = MonitoredState.OK;
            Regex messageNumberPattern = new Regex(@"(?<=\[)[0-9]+(?=\])");
            do
            {
                previousString = eventString;
                previousSeverity = severity;
                eventString = null;

                if (!dataReader.IsDBNull(2))
                    eventTime = dataReader.GetDateTime(2);
                if (!dataReader.IsDBNull(3))
                    eventSeverity = Convert.ToInt32(dataReader.GetString(3).TrimEnd());
                if (!dataReader.IsDBNull(4))
                    eventString += dataReader.GetString(4);

                eventSpid = ProbeHelpers.ParseMessageNumber(eventString, messageNumberPattern).ToString();
                eventString = ProbeHelpers.ParseAgentMessage(eventString);

                if (!lastScanDate.HasValue || eventTime > lastScanDate)
                    HandleLogLines(eventTime,
                                   ref eventString,
                                   eventSpid,
                                   previousString,
                                   agentLogRegexCritical,
                                   agentLogRegexWarning,
                                   agentLogRegexInfo,
                                   advancedSettings,
                                   previousSeverity,
                                   ref severity,
                                   ref previousStringMatched,
                                   ref previousStringUsedAsFooter,
                                   true, eventSeverity);
            } while (dataReader.Read());
        }

        private void ReadAgentLog2000(SqlDataReader dataReader, List<Regex> agentLogRegexCritical,
                                      List<Regex> agentLogRegexWarning, List<Regex> agentLogRegexInfo,
                                      AdvancedAlertConfigurationSettings advancedSettings, DateTime? lastScanDate)
        {
            DateTime? eventTime = null;
            String eventString = null;
            String eventSpid = null;
            String previousString = null;
            bool previousStringMatched = false;
            bool previousStringUsedAsFooter = false;
            MonitoredState severity = MonitoredState.OK;
            MonitoredState previousSeverity = MonitoredState.OK;
            Regex messageNumberPattern = new Regex(@"(?<=\[)[0-9]+(?=\])");
            int? eventSeverity = null;

            string inputBuffer = "";
            string outputBuffer = "";
            string inputTimeStamp = null;
            string outputTimeStamp = null;


            do
            {
                if (!dataReader.IsDBNull(2))
                {
                    inputBuffer = dataReader.GetString(2);

                    //Check to see if this is a ContinuationRow and if so, append to previous input but do not save (yet)
                    if (!dataReader.IsDBNull(3) && (Convert.ToInt16(dataReader.GetValue(3)) > 0))
                    {
                        outputBuffer += inputBuffer + '\n';
                    }
                    else
                    {
                        inputTimeStamp = ProbeHelpers.ParseSqlAgentLogLineTimeStamp2000(inputBuffer);
                        //SQL Server 7 does not appear to utilize the ContinuationRow field, and SQL Server 2000 uses it inconsistently,
                        //so check to see if the date returned by the parse is valid.  If not, throw it away and append the message to
                        //the output buffer
                        if (inputTimeStamp == null)
                        {
                            outputBuffer += inputBuffer + '\n';
                        }
                        else
                        {
                            //If the current row is not a continuation row, you may safely save the output buffer
                            //Do not save empty buffers
                            if (outputBuffer != null && outputBuffer.Length > 0)
                            {
                                previousString = eventString;
                                previousSeverity = severity;
                                eventString = outputBuffer;

                                DateTime localTime;
                                DateTime.TryParse(outputTimeStamp, out localTime);
                                eventTime = localTime;

                                eventSpid =
                                    ProbeHelpers.ParseMessageNumber(eventString, messageNumberPattern).ToString();
                                eventString = ProbeHelpers.ParseAgentMessage(eventString);
                                eventSeverity = (int)ProbeHelpers.ParseMessageType(outputBuffer, AgentLogPattern2000);

                                if (!lastScanDate.HasValue || eventTime > lastScanDate)
                                    HandleLogLines(eventTime,
                                                   ref eventString,
                                                   eventSpid,
                                                   previousString,
                                                   agentLogRegexCritical,
                                                   agentLogRegexWarning,
                                                   agentLogRegexInfo,
                                                   advancedSettings,
                                                   previousSeverity,
                                                   ref severity,
                                                   ref previousStringMatched,
                                                   ref previousStringUsedAsFooter,
                                                   true, eventSeverity);
                            }

                            //Begin a new buffer to hold the current row and possibly append continuation rows as the loop continues
                            outputBuffer = inputBuffer;
                            outputTimeStamp = inputTimeStamp;
                        }
                    }
                }
            } while (dataReader.Read());
            //This is a final check for anything left in the input buffer at the end of the loop, in case the last
            //line was a continuation row
            if (inputBuffer != null && inputBuffer.Length > 0)
            {
                previousString = eventString;
                previousSeverity = severity;
                eventString = outputBuffer;

                DateTime localTime;
                DateTime.TryParse(outputTimeStamp, out localTime);
                eventTime = localTime;

                eventSpid =
                    ProbeHelpers.ParseMessageNumber(eventString, messageNumberPattern).ToString();
                eventString = ProbeHelpers.ParseAgentMessage(eventString);
                eventSeverity = (int)ProbeHelpers.ParseMessageType(outputBuffer, AgentLogPattern2000);

                if (!lastScanDate.HasValue || eventTime > lastScanDate)
                    HandleLogLines(eventTime,
                                   ref eventString,
                                   eventSpid,
                                   previousString,
                                   agentLogRegexCritical,
                                   agentLogRegexWarning,
                                   agentLogRegexInfo,
                                   advancedSettings,
                                   previousSeverity,
                                   ref severity,
                                   ref previousStringMatched,
                                   ref previousStringUsedAsFooter,
                                   true, eventSeverity);
            }

        }

        private void ReadErrorLog(SqlDataReader dataReader, List<Regex> errorLogRegexCritical, List<Regex> errorLogRegexWarning,
                                  List<Regex> errorLogRegexInfo, AdvancedAlertConfigurationSettings advancedSettings)
        {
            DateTime? lastScanDate = DateTime.MinValue;
            // If we're picking up events that were covered by the previous refresh, skip them
            if (previousRefresh != null && previousRefresh.Alerts.LogScanDate.HasValue)
                lastScanDate = previousRefresh.Alerts.LogScanDate.Value;
            else
            {
                lastScanDate = PersistenceManager.Instance.GetLogScanDate();
            }

            if (refresh.ProductVersion.Major >= 9) // Use same method for reading SQL Server 2005/2008
            {
                ReadErrorLog2005(dataReader, errorLogRegexCritical, errorLogRegexWarning, errorLogRegexInfo, advancedSettings, lastScanDate);
            }
            else
            {
                ReadErrorLog2000(dataReader, errorLogRegexCritical, errorLogRegexWarning, errorLogRegexInfo, advancedSettings, lastScanDate);
            }
        }

        private void ReadErrorLog2005(SqlDataReader dataReader, List<Regex> errorLogRegexCritical,
                                      List<Regex> errorLogRegexWarning, List<Regex> errorLogRegexInfo,
                                      AdvancedAlertConfigurationSettings advancedSettings, DateTime? lastScanDate)
        {
            DateTime? eventTime = null;
            String eventString = null;
            String eventSpid = null;
            String previousString;
            bool previousStringMatched = false;
            bool previousStringUsedAsFooter = false;
            MonitoredState severity = MonitoredState.OK;
            MonitoredState previousSeverity = MonitoredState.OK;

            do
            {
                previousString = eventString;
                previousSeverity = severity;
                eventString = null;

                if (!dataReader.IsDBNull(2))
                    eventTime = dataReader.GetDateTime(2);
                if (!dataReader.IsDBNull(3))
                    eventSpid = dataReader.GetString(3);
                if (!dataReader.IsDBNull(4))
                    eventString += dataReader.GetString(4);

                if (!lastScanDate.HasValue || eventTime > lastScanDate)
                    HandleLogLines(eventTime,
                                   ref eventString,
                                   eventSpid,
                                   previousString,
                                   errorLogRegexCritical,
                                   errorLogRegexWarning,
                                   errorLogRegexInfo,
                                   advancedSettings,
                                   previousSeverity,
                                   ref severity,
                                   ref previousStringMatched,
                                   ref previousStringUsedAsFooter,
                                   false, null);
            } while (dataReader.Read());
        }

        private void ReadErrorLog2000(SqlDataReader dataReader, List<Regex> errorLogRegexCritical,
                                      List<Regex> errorLogRegexWarning, List<Regex> errorLogRegexInfo,
                                      AdvancedAlertConfigurationSettings advancedSettings, DateTime? lastScanDate)
        {
            DateTime? eventTime = null;
            String eventString = null;
            String eventSpid = null;
            String previousString = null;
            bool previousStringMatched = false;
            bool previousStringUsedAsFooter = false;
            MonitoredState severity = MonitoredState.OK;
            MonitoredState previousSeverity = MonitoredState.OK;

            //Set up buffer to handle multiple-line entries
            string outputBuffer = "";
            string inputBuffer = "";

            //Set up space location variables
            bool firstLoop = true;
            int spaceLocation = 0;

            //Set up temporary date parsing variable
            DateTime? inputTimeStamp = null;
            DateTime? outputTimeStamp = null;


            do
            {
                //Skip null lines
                if (!dataReader.IsDBNull(2))
                {
                    inputBuffer = dataReader.GetString(2);

                    //To keep from having to check for the date type on every line, we will check here to see where the
                    //log line object needs to start to parse out the date 
                    if (firstLoop)
                    {
                        spaceLocation = inputBuffer.IndexOf(" ") + 1;
                        spaceLocation += inputBuffer.Substring(spaceLocation).IndexOf(" ");
                        firstLoop = false;
                    }

                    //Check to see if this is a ContinuationRow and if so, append to previous input but do not save (yet)
                    if (!dataReader.IsDBNull(3) && (Convert.ToInt16(dataReader.GetValue(3)) > 0))
                    {
                        outputBuffer += inputBuffer + '\n';
                    }
                    else
                    {
                        inputTimeStamp = ProbeHelpers.ParseSqlServerLogTimeStamp2000(ref inputBuffer, spaceLocation);
                        if (inputTimeStamp == DateTime.MinValue)
                        {
                            outputBuffer += inputBuffer + '\n';
                        }
                        else
                        {
                            //If the current row is not a continuation row, you may safely save the output buffer
                            //Do not save empty buffers
                            if (outputBuffer != null && outputBuffer.Length > 0)
                            {
                                previousString = eventString;
                                previousSeverity = severity;
                                eventString = null;

                                eventTime = outputTimeStamp;

                                int spidSpaceLocation = outputBuffer.IndexOf(" ");
                                if (spidSpaceLocation < 0)
                                {
                                    eventSpid = outputBuffer;
                                    outputBuffer = "";
                                }
                                else
                                {
                                    eventSpid = outputBuffer.Substring(0, spidSpaceLocation).Trim(new char[] { ' ' });
                                    outputBuffer =
                                        outputBuffer.Substring(spidSpaceLocation,
                                                               outputBuffer.Length - spidSpaceLocation).
                                            Trim(new char[] { ' ' });
                                }

                                eventString = outputBuffer;

                                if (!lastScanDate.HasValue || eventTime > lastScanDate)
                                    HandleLogLines(eventTime,
                                                   ref eventString,
                                                   eventSpid,
                                                   previousString,
                                                   errorLogRegexCritical,
                                                   errorLogRegexWarning,
                                                   errorLogRegexInfo,
                                                   advancedSettings,
                                                   previousSeverity,
                                                   ref severity,
                                                   ref previousStringMatched,
                                                   ref previousStringUsedAsFooter,
                                                   false, null);
                            }

                            //Begin a new buffer to hold the current row and possibly append continuation rows as the loop continues
                            outputBuffer = inputBuffer;
                            outputTimeStamp = inputTimeStamp;
                        }
                    }
                }
            } while (dataReader.Read());

            if (inputBuffer != null && inputBuffer.Length > 0)
            {
                previousString = eventString;
                previousSeverity = severity;
                eventString = null;

                eventTime = outputTimeStamp;

                int spidSpaceLocation = outputBuffer.IndexOf(" ");
                if (spidSpaceLocation < 0)
                {
                    eventSpid = outputBuffer;
                    outputBuffer = "";
                }
                else
                {
                    eventSpid = outputBuffer.Substring(0, spidSpaceLocation).Trim(new char[] { ' ' });
                    outputBuffer =
                        outputBuffer.Substring(spidSpaceLocation,
                                               outputBuffer.Length - spidSpaceLocation).
                            Trim(new char[] { ' ' });
                }

                eventString = outputBuffer;

                if (!lastScanDate.HasValue || eventTime > lastScanDate)
                    HandleLogLines(eventTime,
                                   ref eventString,
                                   eventSpid,
                                   previousString,
                                   errorLogRegexCritical,
                                   errorLogRegexWarning,
                                   errorLogRegexInfo,
                                   advancedSettings,
                                   previousSeverity,
                                   ref severity,
                                   ref previousStringMatched,
                                   ref previousStringUsedAsFooter,
                                   false, null);
            }
        }

        private void HandleLogLines(DateTime? eventTime,
                                    ref string eventString,
                                    string eventSpid,
                                    string previousString,
                                    List<Regex> errorLogRegexCritical,
                                    List<Regex> errorLogRegexWarning,
                                    List<Regex> errorLogRegexInfo,
                                    AdvancedAlertConfigurationSettings advancedSettings,
                                    MonitoredState previousSeverity,
                                    ref MonitoredState severity,
                                    ref bool previousStringMatched,
                                    ref bool previousStringUsedAsFooter,
                                    bool isAgentLog,
                                    int? agentLogMessageType
            )
        {
            using (LOG.DebugCall("HandleLogLines"))
            {
                bool hasAlreadyMatched = false;
                // If we're picking up events since the beginning of the refresh, reset the start date for the next refresh

                if (eventTime > refresh.TimeStampLocal)
                {
                    refresh.Alerts.LogScanDate = eventTime;
                }

                if (!isAgentLog && ErrorLogSeverity.IsMatch(eventString))
                {
                    int severityMatch;
                    severityMatch = Convert.ToInt32(ErrorLogSeverity.Match(eventString).Value);
                    LOG.DebugFormat("The severity parse match is : {0}, event string {1}", severityMatch, eventString);
                    LOG.DebugFormat("InfoThreshold value : {0},InfoThreshold type: {1}",
                                    workload.Thresholds[(int)Metric.ErrorLog].InfoThreshold.Value,
                                    workload.Thresholds[(int)Metric.ErrorLog].InfoThreshold.Value.GetType());

                    if (workload.Thresholds[(int)Metric.ErrorLog].InfoThreshold.Enabled &&
                        (severityMatch >=
                         Convert.ToInt32(workload.Thresholds[(int)Metric.ErrorLog].InfoThreshold.Value)))
                    {
                        hasAlreadyMatched = true;
                        severity = MonitoredState.Informational;
                    }

                    LOG.DebugFormat("WarningThreshold value : {0},WarningThreshold type: {1}",
                                    workload.Thresholds[(int)Metric.ErrorLog].WarningThreshold.Value,
                                    workload.Thresholds[(int)Metric.ErrorLog].WarningThreshold.Value.GetType());

                    if (workload.Thresholds[(int)Metric.ErrorLog].WarningThreshold.Enabled &&
                        (severityMatch >=
                         Convert.ToInt32(workload.Thresholds[(int)Metric.ErrorLog].WarningThreshold.Value)))
                    {
                        hasAlreadyMatched = true;
                        severity = MonitoredState.Warning;
                    }

                    LOG.DebugFormat("CriticalThreshold value : {0},CriticalThreshold type: {1}",
                                   workload.Thresholds[(int)Metric.ErrorLog].CriticalThreshold.Value,
                                    workload.Thresholds[(int)Metric.ErrorLog].CriticalThreshold.Value.GetType());

                    if (workload.Thresholds[(int)Metric.ErrorLog].CriticalThreshold.Enabled &&
                        (severityMatch >=
                         Convert.ToInt32(workload.Thresholds[(int)Metric.ErrorLog].CriticalThreshold.Value)))
                    {
                        hasAlreadyMatched = true;
                        severity = MonitoredState.Critical;
                    }
                    LOG.DebugFormat("The severity error is : {0}", severity.ToString());

                }

                if (isAgentLog && agentLogMessageType.HasValue)
                {
                    LOG.DebugFormat("(AgentLog)InfoThreshold value : {0},InfoThreshold type: {1}",
                                    workload.Thresholds[(int)Metric.AgentLog].InfoThreshold.Value,
                                    workload.Thresholds[(int)Metric.AgentLog].InfoThreshold.Value.GetType());

                    if (workload.Thresholds[(int)Metric.AgentLog].InfoThreshold.Enabled &&
                        (agentLogMessageType <=
                         Convert.ToInt32(workload.Thresholds[(int)Metric.AgentLog].InfoThreshold.Value)))
                    {
                        hasAlreadyMatched = true;
                        severity = MonitoredState.Informational;
                    }

                    LOG.DebugFormat("(AgentLog)WarningThreshold value : {0},WarningThreshold type: {1}",
                                    workload.Thresholds[(int)Metric.AgentLog].WarningThreshold.Value,
                                    workload.Thresholds[(int)Metric.AgentLog].WarningThreshold.Value.GetType());

                    if (workload.Thresholds[(int)Metric.AgentLog].WarningThreshold.Enabled &&
                        (agentLogMessageType <=
                         Convert.ToInt32(workload.Thresholds[(int)Metric.AgentLog].WarningThreshold.Value)))
                    {
                        hasAlreadyMatched = true;
                        severity = MonitoredState.Warning;
                    }

                    LOG.DebugFormat("(AgentLog)CriticalThreshold value : {0},CriticalThreshold type: {1}",
                                    workload.Thresholds[(int)Metric.AgentLog].CriticalThreshold.Value,
                                    workload.Thresholds[(int)Metric.AgentLog].CriticalThreshold.Value.GetType());

                    if (workload.Thresholds[(int)Metric.AgentLog].CriticalThreshold.Enabled &&
                        (agentLogMessageType <=
                         Convert.ToInt32(workload.Thresholds[(int)Metric.AgentLog].CriticalThreshold.Value)))
                    {
                        hasAlreadyMatched = true;
                        severity = MonitoredState.Critical;
                    }

                    LOG.DebugFormat("(AgentLog)The severity error is : {0}", severity.ToString());
                }

                if (hasAlreadyMatched ||
                    ProbeHelpers.ErrorLogScanIsMatch(eventString,
                                                     errorLogRegexCritical,
                                                     errorLogRegexWarning,
                                                     errorLogRegexInfo,
                                                     advancedSettings,
                                                     ref severity))
                {
                    // Construct final string after match
                    eventString = eventTime + "\t" + eventSpid + "\t" + eventString;

                    if (!previousStringMatched && !previousStringUsedAsFooter)
                    {
                        if (isAgentLog)
                        {
                            refresh.Alerts.AgentLogScanLines.Add(
                                new Triple<MonitoredState, string, bool>(previousSeverity, previousString, false));
                        }
                        else
                        {
                            refresh.Alerts.ErrorLogScanLines.Add(
                                new Triple<MonitoredState, string, bool>(previousSeverity, previousString, false));
                        }
                    }
                    if (isAgentLog)
                    {
                        refresh.Alerts.AgentLogScanLines.Add(
                            new Triple<MonitoredState, string, bool>(severity, eventString, true));
                    }
                    else
                    {
                        refresh.Alerts.ErrorLogScanLines.Add(
                            new Triple<MonitoredState, string, bool>(severity, eventString, true));
                    }
                    previousStringMatched = true;
                }
                else
                {
                    // Construct final string after match
                    eventString = eventTime + "\t" + eventSpid + "\t" + eventString;

                    if (previousStringMatched)
                    {
                        if (isAgentLog)
                        {
                            refresh.Alerts.AgentLogScanLines.Add(
                                new Triple<MonitoredState, string, bool>(severity, eventString, false));
                        }
                        else
                        {
                            refresh.Alerts.ErrorLogScanLines.Add(
                                new Triple<MonitoredState, string, bool>(severity, eventString, false));
                        }
                        previousStringMatched = false;
                        previousStringUsedAsFooter = true;
                    }
                    else
                    {
                        previousStringUsedAsFooter = false;
                        previousStringMatched = false;
                    }
                }

                if (isAgentLog)
                {
                    refresh.Alerts.AgentLogMostSevereMatch = severity;
                }
                else
                {
                    refresh.Alerts.ErrorLogMostSevereMatch = severity;
                }
            }
        }


        #endregion

        #endregion

        //START : SQLDM 10.3 (Manali Hukkeri): Technical debt changes
        #region QueryMonitorState
        /// <summary>
        /// Starts the Query Monitor State store collector. This collector will save the last file the last line read.
        /// </summary>
        private void StartQueryMonitorStateStoreCollector()
        {
            try
            {
                StartGenericCollector(new Collector(QueryMonitorStateStoreCollector),
                                   refresh,
                                   "StartQueryMonitorStateStoreCollector",
                                   "Query Monitor State Store",
                                   refresh.Server.ProductVersion,
                                   sqlErrorDelegate, QueryMonitorStateStoreCallback, new object[] { });

            }
            finally
            {
                PersistenceManager.Instance.SetQueryMonitorConfiguration(refresh.MonitoredServer.InstanceName,
                                                                         refresh.MonitoredServer.
                                                                             QueryMonitorConfiguration);
            }
        }

        /// <summary>
        /// Create Query Monitor State Store Collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void QueryMonitorStateStoreCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand saveCommand = null;
            using (LOG.DebugCall("SaveQueryMonitorState"))
            {
                if (!this.queryMonitorConfiguration.QueryStoreMonitoringEnabled && this.refresh.CollectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember) && currentFileName != null && (currentFileName != dbLastFileName || lastFileRecordCount > dbLastRecordCount))
                {
                    saveCommand = new SqlCommand(SqlCommandBuilder.BildQueryMonitor2012ExWriteQuery(
                    currentFileName, lastFileRecordCount), conn);
                    sdtCollector = new SqlCollector(saveCommand, true);
                }
                else
                {
                    sdtCollector = new SqlCollector(new SqlCommand(string.Empty, conn), true);
                }

                LOG.Verbose(string.Format(queryMonitorLogStartTemplate + "Trying to save query monitor state : FileName = {0}, RecordCount = {1}", currentFileName, lastFileRecordCount));
                sdtCollector.BeginCollectionNonQueryExecution(new EventHandler<CollectorCompleteEventArgs>(QueryMonitorStateStoreCallback));
                LOG.Debug(string.Format(queryMonitorLogStartTemplate + "Saved successfully. FileName = {0}, RecordCount = {1}", currentFileName, lastFileRecordCount));
            }
        }

        /// <summary>
        /// Callback used to process the data returned from the Query Monitor State Store collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The Idera.SQLdm.Probes.Collectors.CollectorCompleteEventArgs instance containing the event data.</param>
        private void QueryMonitorStateStoreCallback(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing QueryMonitorStateStoreCallback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, QueryMonitorStateStoreCallback, e);
                return;
            }

            GenericCallback(new CollectorCallback(QueryMonitorStateStoreCallback),
                            refresh,
                            "QueryMonitorStateStoreCallback",
                            "Query Monitor",
                            new FailureDelegate(QueryMonitorStateStoreFailureDelegate),
                            new FailureDelegate(QueryMonitorStateStoreFailureDelegate),
                            new NextCollector(StartConfigurationCollector),
                            sender,
                            e,
                            true,
                            true);

        }

        /// <summary>
        /// Define the QueryMonitorStateStoreCallback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void QueryMonitorStateStoreCallback(CollectorCompleteEventArgs e)
        {
            // Write the Last Read File Name and Offset to the tempdb
            // Callback required for logging purposes only
            if (e.Result == Result.Success)
            {
                LOG.Verbose("QueryMonitorStateStoreCallback() successfully completed");
            }
            else
            {
                var exceptionMessage = "QueryMonitorStateStoreCallback() Write the Last Read File Name and Offset to the tempdb failed ";
                if (e.Exception != null)
                {
                    exceptionMessage += e.Exception.ToString();
                }
                LOG.Error(exceptionMessage);
            }
        }

        protected void QueryMonitorStateStoreFailureDelegate(Snapshot snapshot, Exception e)
        {
            //START-SQLdm 9.1 (Ankit Srivastava) handling old/deleted .xel file scneario - logging it specifically
            if (e.Message != null && (e.Message.ToLower().Contains("is invalid for log file") ||
                    e.Message.ToLower().Contains("specify an offset that exists in the log file and retry your query")))
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "The Query Monitor State store Collection with Extended Event skipped because of following error , this collection would resume in next collection cycle: {0}", e, true);
            }
            else
                //END-SQLdm 9.1 (Ankit Srivastava) handling old/deleted .xel file scneario - logging it specifically
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "There was an error in the Query Monitor State store collector: {0}", e, true);
        }

        //START : SQLDM 10.3 (Manali Hukkeri): Technical debt changes
        #endregion

        #region OS Metrics

        #region OS Metrics Collector Methods

        /// <summary>
        /// Starts the OS Metrics collector
        /// </summary>
        private void StartOSMetricsCollector()
        {
            StartGenericCollector(new Collector(OSMetricsCollector),
                                  refresh,
                                  "StartOSMetricsCollector",
                                  "OS Metrics",
                                  refresh.Server.ProductVersion,
                                  sqlErrorDelegate, OSMetricsCallback, new object[] { });
        }

        private void StartOSMetricsWmiCollector()
        {
            //START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe to get the machineName 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
            if (machineName == null)
            {
                LOG.Warn("The machineName could not be collected before StartOSMetricsWmiCollector hence using ServerHostName");
                machineName = refresh.Server.ServerHostName;
            }
            else
            {
                LOG.VerboseFormat("The Machine name collected as {0}.", machineName);
            }

            if (osProbe == null)
            {
                osProbe = new OSStatisticsWmiMiniProbe(machineName, wmiConfig, refresh.Server.Statistics.SQLProcessID);
            }

            //osProbe = new OSStatisticsWmiMiniProbe(refresh.Server.ServerHostName, wmiConfig);
            //END SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe to get the machineName 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
          
                osProbe.BeginProbe(OnOSMetricsWmiCollectorComplete);
        }

        private void OnOSMetricsWmiCollectorComplete(object sender, ProbeCompleteEventArgs args)
        {
            OSMetrics previousStats = null;
            if (previousRefresh != null && previousRefresh.Server != null)
                previousStats = previousRefresh.Server.OSMetricsStatistics;

            refresh.Server.OSMetricsStatistics = osProbe.ReadOSMetrics(previousStats, refresh, LOG);

            //Calculate SQL CPU percent divide against logical processors
            if (refresh.ProductVersion.Major > 8)
                refresh.Server.Statistics.CpuPercentage = refresh.Server.OSMetricsStatistics.PercentSQLProcessorTime / refresh.Server.Statistics.LogicalProcessors;

            StartDiskDrivesCollector();
        }

        /// <summary>
        /// Callback used to process the data returned from the OS Metrics collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The Idera.SQLdm.Probes.Collectors.CollectorCompleteEventArgs instance containing the event data.</param>
        private void OSMetricsCallback(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing OSMetricsCallback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, OSMetricsCallback, e);
                return;
            }

                //If OS Metrics fails we do not want to fail the entire snapshot, so return partial
                GenericCallback(new CollectorCallback(OSMetricsCallback),
                            refresh,
                            "OSMetricsCallback",
                            "OS Metrics",
                            new FailureDelegate(OSMetricsFailureDelegate),
                            new FailureDelegate(OSMetricsFailureDelegate),
                            sender,
                            e,
                            true);
          
        }

        /// <summary>
        /// Create OS Metrics Collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void OSMetricsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd = SqlCommandBuilder.BuildOSMetricsCommand(conn, refresh.Server.ProductVersion, wmiConfig, refresh.Server.Statistics.SQLProcessID, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(OSMetricsCallback));
        }


        /// <summary>
        /// Define the Query Monitor callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void OSMetricsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                if (wmiConfig.DirectWmiEnabled &&  cloudProviderId != Constants.MicrosoftAzureManagedInstanceId)
                {
                    //START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe to get the machineName 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
                    if (machineName == null)
                    {
                        LOG.Warn("The machineName could not be collected before OSMetricCallback hence using ServerHostName");
                        machineName = refresh.Server.ServerHostName;
                    }
                    else
                    {
                        LOG.VerboseFormat("The Machine name collected before OSMetricCallback as {0}.", machineName);
                    }
                    if (osProbe == null)
                        osProbe = new OSStatisticsWmiMiniProbe(machineName, wmiConfig, refresh.Server.Statistics.SQLProcessID);
                    //osProbe = new OSStatisticsWmiMiniProbe(refresh.Server.ServerHostName, wmiConfig);
                    //ENDS SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe to get the machineName 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm

                    if (rd.Read() && !rd.IsDBNull(2))
                        osProbe.ServerTimeStamp = rd.GetDateTime(2);

                    StartOSMetricsWmiCollector();
                    return;
                }

                if(cloudProviderId == Constants.AmazonRDSId)
                refresh.Server.OSMetricsStatistics =
                    ProbeHelpers.ReadAWSOSMetrics(rd,
                                               (previousRefresh != null && previousRefresh.Server != null &&
                                                previousRefresh.Server.OSMetricsStatistics != null &&
                                                previousRefresh.ServerStartupTime == refresh.ServerStartupTime)
                                                   ? previousRefresh.Server.OSMetricsStatistics
                                                   : null,
                                               refresh,
                                               LOG, cloudProviderId, refresh.ServerName ?? refresh.ServerHostName);
                else
                    refresh.Server.OSMetricsStatistics =
                    ProbeHelpers.ReadOSMetrics(rd,
                                               (previousRefresh != null && previousRefresh.Server != null &&
                                                previousRefresh.Server.OSMetricsStatistics != null &&
                                                previousRefresh.ServerStartupTime == refresh.ServerStartupTime)
                                                   ? previousRefresh.Server.OSMetricsStatistics
                                                   : null,
                                               refresh,
                                               LOG, cloudProviderId);
                                
                if (cloudProviderId == Constants.MicrosoftAzureManagedInstanceId)
                {
                    rd.NextResult();
                    ProbeHelpers.ReadAzureManagedInstancePages(rd, refresh, LOG, refresh.Server, GenericFailureDelegate);
                    rd.NextResult();
                    ProbeHelpers.ReadAvgCpuPercent(rd, refresh, LOG, refresh.Server, GenericFailureDelegate);
                }

                //Calculate SQL CPU percent divide against logical processors
                if (refresh.ProductVersion.Major > 8)
                    refresh.Server.Statistics.CpuPercentage = refresh.Server.OSMetricsStatistics.PercentSQLProcessorTime / refresh.Server.Statistics.LogicalProcessors;

            }

            StartDiskDrivesCollector();

        }

        /// <summary>
        /// OS Metrics failure delegate
        /// Since OS Metrics are optional we do not want to fail the entire refresh if unavailable
        /// </summary>
        /// <param name="snapshot">Snapshot to return with partial failure</param>
        protected void OSMetricsFailureDelegate(Snapshot snapshot, Exception e)
        {
            LOG.Error("Error collecting OS metrics", e);
            StartDiskDrivesCollector();
        }

        #endregion

        #region OS Metrics Interpretation Methods

        // None

        #endregion

        #endregion

        #region Disk Collectors

        // <summary>
        /// Define the DiskDrives collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void DiskDrivesCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildDiskDrivesCommand(conn, ver, wmiConfig, workload.MonitoredServer.DiskCollectionSettings, driveProbeData,cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DiskDrivesCallback));
        }

        /// <summary>
        /// Starts the Disk Dives collector.
        /// </summary>
        private void StartDiskDrivesCollector()
        {
            if (wmiConfig.DirectWmiEnabled)
                StartDiskDriveCollector();//SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Loop problem solved on 31st March 2015
            else
            {      //Arg params for DiskSize2005
                var driveProbeDataParam = (DriveStatisticsWmiProbe.DriveStatisticsWmiDetails)driveProbeData;
                bool isWMICallFailed = (driveProbeDataParam == null || driveProbeDataParam.DiskMap == null || driveProbeDataParam.DiskMap.Count == 0);

                StartGenericCollector(new Collector(DiskDrivesCollector), refresh, "StartDiskDrivesCollector", "Disk Drives", DiskDrivesCallback, new object[] { isWMICallFailed });
            }
        }

        /// <summary>
        /// Define the DiskDrives callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DiskDrivesCallback(CollectorCompleteEventArgs e)
        {
            try
            {
                using (SqlDataReader rd = e.Value as SqlDataReader)
                {
                    InterpretDiskDrives(rd);
                }
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error in Disk Drives Callback: {0}", ex);
            }
            finally
            {
                try
                {
                    // start the next probe
                    if (refresh.Server.ProductVersion.Major >= 10 &&
                        !String.IsNullOrEmpty(CollectionServiceConfiguration.GetCollectionServiceElement().DropPlansFromCache))
                    {
                        StartDropPlansCollector();
                    }
                    else
                    {
                        FireCompletion(refresh, Result.Success);
                    }
                }
                catch (Exception ex)
                {
                    LOG.ErrorFormat("Error in DiskDrivesCallback Finally Block: {0}", ex);
                    FireCompletion(refresh, Result.Success);
                }
            }
        }

        /// <summary>
        /// Callback used to process the data returned from the DiskDrives collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DiskDrivesCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(DiskDrivesCallback), refresh, "DiskDrivesCallback", "Disk Drives", new FailureDelegate(DiskMetricsFailureDelegate),
                            new FailureDelegate(DiskMetricsFailureDelegate), sender, e);
        }

        /// <summary>
        /// Interpret DiskDrives data
        /// </summary>
        private void InterpretDiskDrives(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretDiskDrives"))
            {
                try
                {
                    bool IsFileSystemObjectDataAvailable = false;
                    refresh.DiskDrives = ProbeHelpers.ReadDiskDrives(dataReader,
                                                                     previousRefresh != null ? previousRefresh.DiskDrives : null,
                                                                     out IsFileSystemObjectDataAvailable, LOG);
                    if (!IsFileSystemObjectDataAvailable)
                    {
                        LOG.Verbose("Extended disk information is unavailable for server " + refresh.ServerName);
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting Disk Drives data: {0}", exception,
                                                        true);
                    return;
                }
            }
        }


        /// <summary>
        /// Disk Metrics failure delegate
        /// Since disk metrics are optional we do not want to fail the entire refresh if unavailable
        /// </summary>
        /// <param name="snapshot">Snapshot to return with partial failure</param>
        protected void DiskMetricsFailureDelegate(Snapshot snapshot, Exception e)
        {
            LOG.Error("Error collecting disk metrics", e);

            try
            {
                // start the next probe
                if (refresh.Server.ProductVersion.Major >= 10 &&
                    !String.IsNullOrEmpty(CollectionServiceConfiguration.GetCollectionServiceElement().DropPlansFromCache))
                {
                    StartDropPlansCollector();
                }
                else
                {
                    FireCompletion(refresh, Result.Success);
                }
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error in DiskMetricsFailureDelegate: {0}", ex);
                FireCompletion(refresh, Result.Success);
            }
        }

        /// <summary>
        /// When doing direct wmi, the database summary probe needs disk size information to use in expansion calculations.
        /// We have to collect the disk size info first and poke it into the temp table (#disk_drives) that the batch uses.
        /// </summary>
        private void StartDiskDriveCollector()
        {
            try
            {
                //START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe to get the machineName 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
                if (machineName == null)
                {
                    LOG.Warn("The machineName could not be collected before StartDiskDriveCollector hence using ServerHostName");
                    machineName = refresh.Server.ServerHostName;
                }
                else
                {
                    LOG.VerboseFormat("The Machine name collected before StartDiskDriveCollector as {0}.", machineName);
                }
                //END SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe to get the machineName 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
                var diskSettings = workload.MonitoredServer.DiskCollectionSettings;

                driveProbe = new DriveStatisticsWmiProbe(machineName, wmiConfig, workload.MonitoredServer.DiskCollectionSettings);
                driveProbe.AutoDiscovery = diskSettings.AutoDiscover;
                if (!diskSettings.AutoDiscover && diskSettings.Drives != null && diskSettings.Drives.Length > 0)
                {
                    driveProbe.IncludeAll = false;
                    driveProbe.DriveList.AddRange(diskSettings.Drives);
                }
                driveProbe.BeginProbe(OnDiskStatsComplete);
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error starting disk drive collector: {0}", ex);
                try
                {
                    // start the next probe
                    if (refresh.Server.ProductVersion.Major >= 10 &&
                        !String.IsNullOrEmpty(CollectionServiceConfiguration.GetCollectionServiceElement().DropPlansFromCache))
                    {
                        StartDropPlansCollector();
                    }
                    else
                    {
                        FireCompletion(refresh, Result.Success);
                    }
                }
                catch (Exception e)
                {
                    LOG.ErrorFormat("Error in StartDiskDriveCollector exit block: {0}", e);
                    FireCompletion(refresh, Result.Success);
                }
            }
        }

        private void OnDiskStatsComplete(object sender, ProbeCompleteEventArgs args)
        {
            using (LOG.DebugCall("OnDiskStatsComplete"))
            {
                try
                {
                    //START SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - replaced older object with the new object type
                    var probedata = args.Data as DriveStatisticsWmiProbe.DriveStatisticsWmiDetails;
                    if (probedata != null && probedata.DiskMap != null && probedata.DiskMap.Count > 0)
                    {
                        AddDiskStatistics(probedata.DiskMap.Values);
                    }
                    driveProbeData = probedata;
                    //END SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - replaced older object with the new object type
                }
                catch (Exception e)
                {
                    LOG.ErrorFormat("Error in OnDiskStatsComplete: {0}", e);
                }
                finally
                {
                    try
                    {
                        // start the next probe
                        if (refresh.Server.ProductVersion.Major >= 10 &&
                            !String.IsNullOrEmpty(CollectionServiceConfiguration.GetCollectionServiceElement().DropPlansFromCache))
                        {
                            StartDropPlansCollector();
                        }
                        else
                        {
                            FireCompletion(refresh, Result.Success);
                        }
                    }
                    catch (Exception e)
                    {
                        LOG.ErrorFormat("Error in OnDiskStatsComplete Finally Block: {0}", e);
                        FireCompletion(refresh, Result.Success);
                    }
                }
            }
        }

        private void AddDiskStatistics(IEnumerable<DriveStatisticsWmiProbe.DiskStatistics> valueCollection)
        {
            using (LOG.DebugCall("AddDiskStatistics"))
            {
                var diskDrives = new Dictionary<string, DiskDrive>();
                try
                {

                    var previousDiskDrives = (previousRefresh != null && previousRefresh.DiskDrives != null) ? previousRefresh.DiskDrives : null;

                    // blow out the mount points for each volume
                    foreach (var disk in valueCollection)
                    {
                        if (disk.DriveType == 4 || disk.DriveType == 5) continue;

                        foreach (var mount in disk.Paths)
                        {
                            var dd = new DiskDrive();
                            dd.DriveLetter = mount;
                            dd.UnusedSize.Bytes = disk.FreeSpace;
                            dd.TotalSize.Bytes = disk.TotalSize;
                            dd.DiskIdlePercentRaw = disk.PercentIdleTime;
                            dd.DiskIdlePercentBaseRaw = disk.PercentIdleTime_Base;
                            dd.AverageDiskQueueLengthRaw = disk.AvgDiskQueueLength;
                            dd.Timestamp_Sys100ns = disk.TimeStamp_Sys100NS;
                            dd.AvgDisksecPerReadRaw = disk.AvgDisksecPerRead;
                            dd.AvgDisksecPerRead_Base = disk.AvgDisksecPerRead_Base;
                            dd.AvgDisksecPerTransferRaw = disk.AvgDisksecPerTransfer;
                            dd.AvgDisksecPerTransfer_Base = disk.AvgDisksecPerTransfer_Base;
                            dd.AvgDisksecPerWriteRaw = disk.AvgDisksecPerWrite;
                            dd.AvgDisksecPerWrite_Base = disk.AvgDisksecPerWrite_Base;
                            dd.Frequency_Perftime = disk.Frequency_Perftime;
                            dd.DiskReadsPerSec_Raw = disk.DiskReadsPerSec;
                            dd.DiskTransfersPerSec_Raw = disk.DiskTransfersPerSec;
                            dd.DiskWritesPerSec_Raw = disk.DiskWritesPerSec;
                            dd.TimeStamp_PerfTime = disk.TimeStamp_PerfTime;
                            dd.Timestamp_utc = refresh.TimeStamp;

                            ProbeHelpers.CalcDiskDrive(dd, previousDiskDrives);

                            if (LOG.IsVerboseEnabled)
                            {
                                var sb = new StringBuilder();
                                sb.AppendLine("Verbose Disk Data for " + dd.DriveLetter);
                                sb.AppendLine("Raw:");
                                sb.AppendLine("dd.UnusedSize.Bytes = " + dd.UnusedSize.Bytes);
                                sb.AppendLine("dd.TotalSize.Bytes = " + dd.TotalSize.Bytes);
                                sb.AppendLine("dd.DiskIdlePercentRaw = " + dd.DiskIdlePercentRaw);
                                sb.AppendLine("dd.DiskIdlePercentBaseRaw = " + dd.DiskIdlePercentBaseRaw);
                                sb.AppendLine("dd.AverageDiskQueueLengthRaw = " + dd.AverageDiskQueueLengthRaw);
                                sb.AppendLine("dd.Timestamp_Sys100ns = " + dd.Timestamp_Sys100ns);
                                sb.AppendLine("dd.AvgDisksecPerReadRaw = " + dd.AvgDisksecPerReadRaw);
                                sb.AppendLine("dd.AvgDisksecPerRead_Base = " + dd.AvgDisksecPerRead_Base);
                                sb.AppendLine("dd.AvgDisksecPerTransferRaw = " + dd.AvgDisksecPerTransferRaw);
                                sb.AppendLine("dd.AvgDisksecPerTransfer_Base = " + dd.AvgDisksecPerTransfer_Base);
                                sb.AppendLine("dd.AvgDisksecPerWriteRaw = " + dd.AvgDisksecPerWriteRaw);
                                sb.AppendLine("dd.AvgDisksecPerWrite_Base = " + dd.AvgDisksecPerWrite_Base);
                                sb.AppendLine("dd.Frequency_Perftime = " + dd.Frequency_Perftime);
                                sb.AppendLine("dd.DiskReadsPerSec_Raw = " + dd.DiskReadsPerSec_Raw);
                                sb.AppendLine("dd.DiskTransfersPerSec_Raw = " + dd.DiskTransfersPerSec_Raw);
                                sb.AppendLine("dd.DiskWritesPerSec_Raw = " + dd.DiskWritesPerSec_Raw);
                                sb.AppendLine("dd.TimeStamp_PerfTime = " + dd.TimeStamp_PerfTime);
                                sb.AppendLine("dd.Timestamp_utc = " + dd.Timestamp_utc);
                                sb.AppendLine("Calculated:");
                                sb.AppendLine("dd.DiskIdlePercent = " + dd.DiskIdlePercent);
                                sb.AppendLine("dd.AverageDiskQueueLength = " + dd.AverageDiskQueueLength);
                                sb.AppendLine("dd.AvgDiskSecPerRead (ms) = " + (dd.AvgDiskSecPerRead.HasValue ? dd.AvgDiskSecPerRead.Value.TotalMilliseconds.ToString() : ""));
                                sb.AppendLine("dd.AvgDiskSecPerTransfer (ms) = " + (dd.AvgDiskSecPerTransfer.HasValue ? dd.AvgDiskSecPerTransfer.Value.TotalMilliseconds.ToString() : ""));
                                sb.AppendLine("dd.AvgDiskSecPerWrite (ms) = " + (dd.AvgDiskSecPerWrite.HasValue ? dd.AvgDiskSecPerWrite.Value.TotalMilliseconds.ToString() : ""));
                                sb.AppendLine("dd.DiskReadsPerSec = " + dd.DiskReadsPerSec);
                                sb.AppendLine("dd.DiskTransfersPerSec = " + dd.DiskTransfersPerSec);
                                sb.AppendLine("dd.DiskWritesPerSec = " + dd.DiskWritesPerSec);
                                LOG.Verbose(sb.ToString());
                            }

                            if (!diskDrives.ContainsKey(dd.DriveLetter))
                            {
                                diskDrives.Add(dd.DriveLetter, dd);
                            }
                            else
                            {
                                LOG.WarnFormat("Duplicate disk drive data collected and discarded for drive {0}", dd.DriveLetter);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LOG.ErrorFormat("Exception adding WMI disk statistics: {0}", e);
                }
                finally
                {
                    refresh.DiskDrives = diskDrives;
                }
            }
        }

        #endregion

        #region Drop Plans

        /// <summary>
        /// Starts the OS Metrics collector
        /// </summary>
        private void StartDropPlansCollector()
        {
            StartGenericCollector(new Collector(DropPlansCollector),
                                  refresh,
                                  "StartDropPlansCollector",
                                  "Drop Plans",
                                  refresh.Server.ProductVersion,
                                  sqlErrorDelegate, DropPlansCallback, new object[] { });
        }

        /// <summary>
        /// Callback used to process the data returned from the OS Metrics collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The Idera.SQLdm.Probes.Collectors.CollectorCompleteEventArgs instance containing the event data.</param>
        private void DropPlansCallback(object sender, CollectorCompleteEventArgs e)
        {
            try
            {
                if (Thread.CurrentThread.IsThreadPoolThread)
                {
                    LOG.Debug("Pushing DropPlansCallback to work queue.");
                    QueueCallback(refresh, sender as SqlCollector, DropPlansCallback, e);
                    return;
                }

                //If OS Metrics fails we do not want to fail the entire snapshot, so return partial
                GenericCallback(new CollectorCallback(DropPlansCallback),
                                refresh,
                                "DropPlansCallback",
                                "Drop Plans",
                                new FailureDelegate(DropPlansFailureDelegate),
                                new FailureDelegate(DropPlansFailureDelegate),
                                sender,
                                e,
                                true);
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error in DropPlansCallback: {0}", ex);
                FireCompletion(refresh, Result.Success);
            }
        }

        /// <summary>
        /// Create OS Metrics Collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void DropPlansCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildDropPlansFromCacheCommand(conn, refresh.Server.ProductVersion);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DropPlansCallback));
        }


        /// <summary>
        /// Define the Query Monitor callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DropPlansCallback(CollectorCompleteEventArgs e)
        {
            FireCompletion(refresh, Result.Success);
        }

        /// <summary>
        /// OS Metrics failure delegate
        /// Since OS Metrics are optional we do not want to fail the entire refresh if unavailable
        /// </summary>
        /// <param name="snapshot">Snapshot to return with partial failure</param>
        protected void DropPlansFailureDelegate(Snapshot snapshot, Exception e)
        {
            FireCompletion(snapshot, Result.Success);
        }

        #endregion

        #region Cloud Collectors

        #region Azure Collectors

        private void StartCloudSQLDatabaseCollector()
        {
            StartGenericCollector(new Collector(StartCloudSQLDatabaseCollector), refresh, "StartCloudSQLDatabaseCollector", "Cloud SQL Database", null, new object[] { });
        }

        private void StartCloudSQLDatabaseCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd = SqlCommandBuilder.BuildCloudSQLDatabaseCommand(conn);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(StartCloudSQLDatabaseCollectorCallback));
        }

        private void StartCloudSQLDatabaseCollectorCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(StartStartCloudSQLDatabaseCollectorCallback),
                            refresh,
                            "StartStartCloudSQLDatabaseCollectorCallback",
                            "Cloud SQL Database",
                            null,
                            new FailureDelegate(GenericFailureDelegate),
                            StartAzureSQLMetricCollector,
                            sender,
                            e,
                            false,
                            false);
        }

        private void StartStartCloudSQLDatabaseCollectorCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretCloudDatabases(rd);
                rd.NextResult();
                InterpretElasticPools(rd);
            }
            
        }
        private void InterpretElasticPools(SqlDataReader dataReader)
        {
            while (dataReader.Read())
            {

                string elasticPoolName = dataReader.IsDBNull(1) ? null : dataReader.GetString(1);
                string databaseName = dataReader.IsDBNull(0) ? null : dataReader.GetString(0);
                if (elasticPoolName != null && refresh.AzureElasticPools.ContainsKey(elasticPoolName))
                {
                    refresh.AzureElasticPools[elasticPoolName].Add(databaseName);
                }
                else if (elasticPoolName != null)
                {
                    refresh.AzureElasticPools.Add(elasticPoolName, new List<string>() { databaseName });
                }
                
            }
            //Elastic Pool Support
            foreach (var pool in refresh.AzureElasticPools)
            {
                foreach (var dbName in pool.Value)
                {
                    if (refresh.DbStatistics.ContainsKey(dbName))
                    {
                        refresh.DbStatistics[dbName].ElasticPool = pool.Key;
                    }
                }

            }
        }

        private void InterpretCloudDatabases(SqlDataReader rd)
        {
            cloudDBNames.Clear();
            while (rd.Read())
            {
                cloudDBNames.Add(Convert.ToString(rd.GetValue(0), CultureInfo.InvariantCulture));
            }
        }

        private void StartAzureSQLMetricCollector()
        {
            StartGenericCollector(new Collector(AzureSQLMetricCollector), refresh, "StartAzureSQLMetricCollector", "Azure SQL Metric", null, new object[] { });
        }

        private void AzureSQLMetricCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            AzureMetricCollector azureMetricCollector = new AzureMetricCollector(conn, cloudDBNames, refresh.ServerName, refresh.MonitoredServer.ConnectionInfo.UserName, refresh.MonitoredServer.ConnectionInfo.Password);
            azureMetricCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(StartAzureSQLMetricCollectorCallback));
        }

        private void StartAzureSQLMetricCollectorCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(StartAzureSQLMetricCollectorCallback),
                            refresh,
                            "StartAzureSQLMetricCollectorCallback",
                            "Azure SQL Metric",
                            null,
                            new FailureDelegate(GenericFailureDelegate),
                            GetNextCollectorAfterCloud(),
                            sender,
                            e,
                            false,
                            false);
        }

        private void StartAzureSQLMetricCollectorCallback(CollectorCompleteEventArgs e)
        {
            Dictionary<string, Dictionary<string, object>> azureMetrics = e.Value as Dictionary<string, Dictionary<string, object>>;
            InterpretAzureMetrics(azureMetrics);
        }

        private void InterpretAzureMetrics(Dictionary<string, Dictionary<string, object>> azureMetrics)
        {
            refresh.AzureCloudMetrics = azureMetrics;
        }

        
        #endregion Azure Collectors

        #region AWS Collectors

        private void StartAWSCloudWatchMetricCollector()
        {
            StartGenericCollector(new Collector(AWSCloudWatchMetricCollector), refresh, "StartAWSCloudWatchMetricCollector", "AWS Cloud Watch Metric", null, new object[] { });
        }

        private void AWSCloudWatchMetricCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            AWSMetricCollector awsMetricCollector = new AWSMetricCollector(refresh.ServerName);
            awsMetricCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(AWSCloudWatchMetricCollectorCallback));
        }

        private void AWSCloudWatchMetricCollectorCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(AWSCloudWatchMetricCollectorCallback),
                            refresh,
                            "AWSCloudWatchMetricCollectorCallback",
                            "AWS CloudWatch Metric",
                            null,
                            new FailureDelegate(GenericFailureDelegate),
                            GetNextCollectorAfterCloud(),
                            sender,
                            e,
                            false,
                            false);
        }

        private void AWSCloudWatchMetricCollectorCallback(CollectorCompleteEventArgs e)
        {
            Dictionary<string, double> awsMetrics = e.Value as Dictionary<string, double>;
            InterpretAWSMetrics(awsMetrics);
        }

        private void InterpretAWSMetrics(Dictionary<string, double> awsMetrics)
        {
            refresh.AWSCloudMetrics = awsMetrics;
        }

        #endregion AWS Collectors

        #endregion Cloud Collectors

        #endregion

        #region interface implementations

        /// <summary>
        /// Override the HandleSqlException method, because in this probe both Paused and unable to monitor are not errors
        /// </summary>
        protected new void HandleSqlException(Snapshot snapshot, string collectorName, SqlException sqlException, SqlCollector collector)
        {
            // 17142 is returned when the SQL Server service is paused
            if (sqlException.Number == 17142)
            {
                //Clear out any data collected so far (making sure to retain the VM data if it was already collected)
                VMwareVirtualMachine vmData = null;
                //Clear out any data collected so far (saving VM data if it was collected)
                if (refresh.Server.VMConfig != null)
                {
                    vmData = refresh.Server.VMConfig;
                }
                refresh = new ScheduledRefresh(refresh.MonitoredServer);

                refresh.Server.VMConfig = vmData;
                //Set the server as paused
                refresh.Server.SqlServiceStatus = ServiceState.Paused;
                if (collector != null)
                    collector.Dispose();
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Monitored server is paused", false);
                FireCompletion(refresh, Result.Success);
            }
            else
            {
                if (sqlException.Number == 2 || sqlException.Number == 53)
                {
                    //Clear out any data collected so far (making sure to retain and VM data that was already collected)
                    VMwareVirtualMachine vmData = null;
                    //Clear out any data collected so far (saving VM data if it was collected)
                    if (refresh.Server.VMConfig != null)
                    {
                        vmData = refresh.Server.VMConfig;
                    }
                    refresh = new ScheduledRefresh(refresh.MonitoredServer);

                    refresh.Server.VMConfig = vmData;
                    //Set the service status as unable to monitor
                    refresh.Server.SqlServiceStatus = ServiceState.UnableToMonitor;
                    if (collector != null)
                        collector.Dispose();
                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Monitored server cannot be contacted: " + sqlException.Message, false);
                    FireCompletion(refresh, Result.Success);
                }
                else
                {
                    //Any other error is unhandled and should follow the usual path
                    if (collector != null)
                    {
                        HandleSqlException(snapshot, collectorName, sqlException, collector.SqlText);
                        collector.Dispose();
                    }
                    else
                    {
                        HandleSqlException(snapshot, collectorName, sqlException);
                    }
                    FireCompletion(snapshot, Result.Failure);
                }
            }
        }

        #endregion
    }

    internal sealed class ActivityMonitorCollectionState
    {
        private readonly object sync = new object();
        private readonly ActivityMonitorConfiguration probeConfig;
        private readonly ActivityMonitorConfiguration probePrevConfig;
        private ActivityMonitorConfiguration updatedConfig;
        private bool amCollectorStarted;

        public ActivityMonitorCollectionState(ActivityMonitorConfiguration probeConfig, ActivityMonitorConfiguration probePrevConfig)
        {
            this.probeConfig = probeConfig;
            this.probePrevConfig = probePrevConfig;
        }

        public ActivityMonitorConfiguration ProbeAMConfig
        {
            get { return probeConfig; }
        }

        public ActivityMonitorConfiguration ProbePreviousAMConfig
        {
            get { return probePrevConfig; }
        }

        public ActivityMonitorConfiguration UpdatedConfig
        {
            get { return updatedConfig; }
            set { updatedConfig = value; }
        }

        public bool ActivityMonitorCollectorStarted
        {
            get { return amCollectorStarted; }
            set { amCollectorStarted = value; }
        }

        public bool IsAMAlreadyEnabled
        {
            get
            {
                // currently enabled and qm collector started
                if (amCollectorStarted)
                    return probeConfig.Enabled;

                // return previous refresh setting
                if (probePrevConfig != null)
                    return probePrevConfig.Enabled;

                // can't know for sure - assume not running
                return false;
            }
        }
    }

    internal sealed class QueryMonitorCollectionState
    {
        private readonly object sync = new object();
        private readonly QueryMonitorConfiguration probeConfig;
        private readonly QueryMonitorConfiguration probePrevConfig;
        private QueryMonitorConfiguration updatedConfig;
        private bool qmCollectorStarted;

        public QueryMonitorCollectionState(QueryMonitorConfiguration probeConfig, QueryMonitorConfiguration probePrevConfig)
        {
            this.probeConfig = probeConfig;
            this.probePrevConfig = probePrevConfig;
        }

        public QueryMonitorConfiguration ProbeQMConfig
        {
            get { return probeConfig; }
        }

        public QueryMonitorConfiguration ProbePreviousQMConfig
        {
            get { return probePrevConfig; }
        }

        public QueryMonitorConfiguration UpdatedConfig
        {
            get { return updatedConfig; }
            set { updatedConfig = value; }
        }

        public bool QueryMonitorCollectorStarted
        {
            get { return qmCollectorStarted; }
            set { qmCollectorStarted = value; }
        }

        public bool IsQMAlreadyEnabled
        {
            get
            {
                // currently enabled and qm collector started
                if (qmCollectorStarted)
                    return probeConfig.Enabled;

                // return previous refresh setting
                if (probePrevConfig != null)
                    return probePrevConfig.Enabled;

                // can't know for sure - assume not running
                return false;
            }
        }
    }
}
