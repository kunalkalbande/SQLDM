using System;
//------------------------------------------------------------------------------
// <copyright file="ServerOverview.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using BBS.TracerX;
using Idera.SQLdm.CollectionService.Helpers;
using Idera.SQLdm.CollectionService.Probes.Collectors;
using Idera.SQLdm.CollectionService.Probes.Wmi;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.VMware;
using Idera.SQLdm.Common.HyperV;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Snapshots.Cloud;
using Idera.SQLdm.CollectionService.Monitoring;
using Idera.SQLdm.CollectionService.Helpers;
using System.Globalization;

namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    /// <summary>
    /// On-demand probe for SQL Server overview
    /// </summary>
    class ServerOverviewProbe : SqlBaseProbe
    {
        #region fields

        private ServerOverview refresh = null;
        private OSMetrics previousOSMetrics = null;
        private VMwareVirtualMachine previousVconfig = null;
        private ServerStatistics previousServerStatistics = null;
        private Dictionary<string, DatabaseStatistics> previousDbStatistics = null;
        private TempdbSummaryStatistics previousTempdbStatistics = null;
        private WaitStatisticsSnapshot previousWaitStats = null;
        private Dictionary<string, FileActivityFile> previousFileActivity = null;
        private DateTime? lastRefresh = null;
        private DateTime? serverStartupTime = null;
        private ServerOverviewConfiguration config = null;
        private HandleSqlErrorDelegate sqlErrorDelegate;
        private HandleSqlErrorDelegate connectionErrorDelagate;
        private ClusterCollectionSetting clusterCollectionSetting = ClusterCollectionSetting.Default;
        private DiskCollectionSettings diskSettings;
        private VirtualizationConfiguration virtualizationConfiguration = null;

        private WmiConfiguration wmiConfig;
        private OSStatisticsWmiMiniProbe osProbe;
        private DriveStatisticsWmiProbe driveProbe;
        private Dictionary<string, FileActivityFile> fileMap;

        //OsStatisticAvailability possible texts
        private const string ServiceTimedOutText = "service timedout";
        private const string ServiceUnavailableText = "service unavailable";
        private string machineName = String.Empty;//SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Declaring global variable machineName
        //private int? cloudProviderId = null;//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support

        //Added for DiskSize2005
        private object driveProbeData;
        private List<string> dbInfo;
        private int databaseCount;
        private MonitoredSqlServer msi;
        private List<string> cloudDBNames = new List<string>();
        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerOverviewProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="configuration">The configuration info.</param>
        public ServerOverviewProbe(SqlConnectionInfo connectionInfo, ServerOverviewConfiguration configuration, WmiConfiguration wmiConfig, ClusterCollectionSetting clusterCollectionSetting, DiskCollectionSettings diskSettings, VirtualizationConfiguration vcConfig, MonitoredSqlServer msi, int? cloudProviderId)//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added an optional parameter to support cloud instances
            : base(connectionInfo)
        {
            dbInfo = new List<string>();          
            this.cloudProviderId = cloudProviderId;
            sqlErrorDelegate = HandleSqlException;
            connectionErrorDelagate = ConnectionErrorDelagate;
            LOG = Logger.GetLogger("ServerOverviewProbe");
            refresh = new ServerOverview(connectionInfo);
            refresh.SqlServiceStatus = ServiceState.Running;
            this.cloudProviderId = cloudProviderId;//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
            if (configuration != null)
            {
                serverStartupTime = configuration.ServerStartupTime;
                lastRefresh = configuration.LastRefresh;
                previousOSMetrics = configuration.PreviousOSMetrics;
                previousServerStatistics = configuration.PreviousServerStatistics;
                previousDbStatistics = configuration.PreviousDbStatistics;
                previousTempdbStatistics = configuration.PreviousTempdbStatistics;
                previousWaitStats = configuration.PreviousWaitStatistics;
                previousVconfig = configuration.PreviousVconfig;
                if (configuration.PreviousLockStatistics != null)
                {
                    refresh.LockCounters = configuration.PreviousLockStatistics;
                }
                previousFileActivity = configuration.PreviousFileActivity;
            }
            config = configuration;
            this.wmiConfig = wmiConfig;
            this.clusterCollectionSetting = clusterCollectionSetting;
            this.diskSettings = diskSettings;
            this.virtualizationConfiguration = vcConfig;
            this.msi = msi; //SQLdm 11 Azure Metrics
        }

        #endregion

        #region properties

        #endregion

        #region methods

        /// <summary>
        /// Starts the probe, virtual method called by BaseProbe.BeginProbe().
        /// If this returns cleanly, probe must call FireCompletion()
        /// </summary>
        protected override void Start()
        {
            if (config != null && config.ReadyForCollection)
            {
                StartConnectionStatusCollector();
            }
            else
            {
                FireCompletion(refresh, Result.Success);
            }
        }


        /// <summary>
        /// Define the ConnectionStatus collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void ConnectionStatusCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd = SqlCommandBuilder.BuildConnectionStatusCommand(conn);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(ConnectionStatusCallback);
        }

        /// <summary>
        /// Starts the Connection Status collector.
        /// </summary>
        private void StartConnectionStatusCollector()
        {
            StartGenericCollector(ConnectionStatusCollector, refresh, "StartConnectionStatusCollector", "Connection Status", null, connectionErrorDelagate, ConnectionStatusCallback, new object[] { });
        }

        /// <summary>
        /// Define the server overview collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void ServerOverviewCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            refresh.ProductVersion = ver;
            SqlCommand cmd = SqlCommandBuilder.BuildServerOverviewCommand(conn, ver, wmiConfig, cloudProviderId);//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added a new optional param 
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerOverviewCallback));
        }

        void StartServerOverviewCollectorAzure()
        {
            StartGenericCollectorDatabase(new CollectorDatabase(ServerOverviewCollectorDatabase), refresh, "StartServerOverviewCollector", "Server Overview", ServerOverviewCallbackDatabase, dbInfo[databaseCount], new object[] { });
        }

        /// <summary>
        /// Starts the server overview collector.
        /// </summary>
        void StartServerOverviewCollector()
        {
            var collectorName = "Server Overview";
            // handling case for azure 
            if (cloudProviderId == CLOUD_PROVIDER_ID_AZURE )
            {
                dbInfo = CollectionHelper.GetDatabases(connectionInfo, LOG);
                databaseCount = 0;
                if (dbInfo.Count > 0)
                {
                    StartServerOverviewCollectorAzure();
                }
                else
                {
                    if (wmiConfig.DirectWmiEnabled)
                    {
                        StartMachineNameCollector();
                    }
                    else
                    {
                        StartResponseTimeCollector();
                    }
                }
            }
            // end
            else
            {
                // create and pass the probe error argument to handle and store errors
                StartGenericCollector(new Collector(ServerOverviewCollector), refresh, "StartServerOverviewCollector",
                    collectorName, null, sqlErrorDelegate, ServerOverviewCallback,
                    new object[] {new ProbePermissionHelpers.ProbeError() {Name = collectorName}});
            }
        }

        private void ServerOverviewCollectorDatabase(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver, string dbname)
        {
            refresh.ProductVersion = ver;
            SqlCommand cmd = SqlCommandBuilder.BuildServerOverviewCommand(conn, ver, wmiConfig, cloudProviderId,dbname);//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added a new optional param 
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerOverviewCallbackDatabase));
        }

        private void ServerOverviewCallbackDatabase(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(ServerOverviewCallbackDatabase),
                            refresh,
                           "ServerOverviewCallback", "Server Overview",
                            new FailureDelegate(GenericFailureDelegate),
                            new FailureDelegate(GenericFailureDelegate),
                            null, sender, e, true, true);
            /*GenericCallback(new CollectorCallback(ServerOverviewCallbackDatabase), refresh, "ServerOverviewCallback", "Server Overview",
                sender, e);*/
        }

        private void ServerOverviewCallbackDatabase(CollectorCompleteEventArgs e)
        {
            try
            {
                if (e.Result == Result.Success)
                {
                    if (e.ElapsedMilliseconds.HasValue)
                    {
                        refresh.ResponseTime = e.ElapsedMilliseconds.Value;
                    }

                    if (refresh.ServerStartupTime != serverStartupTime)
                    {
                        if (serverStartupTime.HasValue && refresh.ServerStartupTime.HasValue)
                            LOG.Info("Server restart detected.  Disposing of previous metrics.");
                        previousServerStatistics = new ServerStatistics();
                        previousOSMetrics = new OSMetrics();

                        lastRefresh = null;
                    }

                    if (lastRefresh.HasValue && refresh.TimeStamp > lastRefresh.Value)
                    {
                        refresh.TimeDelta = refresh.TimeStamp - lastRefresh.Value;
                    }
                    // Helpful in debugging per database - lock (refresh)
                    if (e.Value is SqlDataReader)
                    {
                        using (var rd = (SqlDataReader)e.Value)
                        {
                            // For master database, should read Server values
                            // For all databases, should read the database specific values

                            var dbName = string.Empty;
                            if (rd.Read())
                            {
                                dbName = rd.GetString(0);
                            }
                            rd.NextResult();
                            if (dbName == "master")
                            {
                                ServerOverviewInterpreter.ReadVersionInformation(rd, refresh, refresh, LOG);
                                ServerOverviewInterpreter.ReadServerHostName(rd, refresh, refresh, LOG);
                                ServerOverviewInterpreter.ReadEdition(rd, refresh, refresh, LOG);
                                ServerOverviewInterpreter.ReadWindowsServicePack(rd, refresh, refresh, LOG);
                                refresh.ProcessorsUsed =
                                    ServerOverviewInterpreter.CalculateProcessorsUsed(rd, refresh.ProcessorCount, refresh,
                                        LOG);
                                refresh.MaxConnections =
                                    ServerOverviewInterpreter.ReadMaxServerConnections(rd, refresh, LOG);
                                refresh.RunningSince = refresh.ServerStartupTime;
                                ServerOverviewInterpreter.ReadIsClustered(rd, refresh, refresh, LOG,
                                    clusterCollectionSetting);
                                refresh.LoginConfiguration =
                                    ServerOverviewInterpreter.ReadLoginConfiguration(rd, refresh, LOG);
                                if (rd.Read())
                                {
                                    ServerOverviewInterpreter.ReadDatabaseSummary(rd, refresh.DatabaseSummary, refresh,
                                        LOG);
                                    ProbeHelpers.ReadDatabaseStatus(rd, previousDbStatistics, refresh, LOG);
                                    ProbeHelpers.ReadDatabaseCounters(rd, previousDbStatistics, refresh, LOG);
                                    InterpretAzureIO(rd);
                                }
                                rd.NextResult();
                            }


                            // Handle Addition using separate method, if required
                            // For each database

                            /*InterpretAzureStorageSizeLimit(rd);    
                            rd.NextResult();
                            InterpretAzureDbDetail(rd);
                            rd.NextResult();
                            ServerOverviewInterpreter.ReadThroughput(rd, previousServerStatistics, refresh.Statistics,
                                refresh, LOG, lastRefresh);

                            var locks= ServerOverviewInterpreter.ReadTotalLocks(rd, refresh, LOG);
                            if (refresh.TotalLocks == null)
                                refresh.TotalLocks = locks;
                            else
                                refresh.TotalLocks += locks;
                            ServerOverviewInterpreter.ReadProcessDetails(rd, refresh, refresh, LOG);
                            ProbeHelpers.ReadLockStatistics(rd, refresh, LOG, refresh.LockCounters, null);
                            ServerOverviewInterpreter.ReadServerStatistics(rd, previousServerStatistics, refresh, refresh,
                                LOG);
                            refresh.DatabaseSummary.LogFileSpaceUsed.Bytes +=
                                ServerOverviewInterpreter.ReadUsedLogSize(rd, refresh, LOG).Bytes;
                            InterpretMemory(rd);
                            ProbeHelpers.ReadDatabaseWaitTime(rd, previousDbStatistics, refresh, LOG);
                            InterpretFileActivity(rd);
                            if (dbName == "master")
                            {
                                InterpretTempdbSummary(rd);
                            }*/

                            InterpretAzureStorageSizeLimit(rd);
                            rd.NextResult();
                            InterpretAzureDbDetail(rd);
                            rd.NextResult();
                            ServerOverviewInterpreter.ReadThroughput(rd, previousServerStatistics, refresh.Statistics,
                                refresh, LOG, lastRefresh);
                            refresh.TotalLocks += ServerOverviewInterpreter.ReadTotalLocks(rd, refresh, LOG);
                            ServerOverviewInterpreter.ReadProcessDetails(rd, refresh, refresh, LOG);
                            ProbeHelpers.ReadLockStatistics(rd, refresh, LOG, refresh.LockCounters, null);
                            ServerOverviewInterpreter.ReadServerStatistics(rd, previousServerStatistics, refresh, refresh,
                                LOG);
                            refresh.DatabaseSummary.LogFileSpaceUsed.Bytes +=
                                ServerOverviewInterpreter.ReadUsedLogSize(rd, refresh, LOG).Bytes;
                            InterpretMemory(rd);
                            ProbeHelpers.ReadDatabaseWaitTime(rd, previousDbStatistics, refresh, LOG);
                            InterpretFileActivity(rd);
                            if (dbName == "master")
                            {
                                InterpretTempdbSummary(rd);
                            }

                            if (rd.NextResult())
                            {
                                ReadWaitStats(rd);
                            }
                            if (rd.NextResult())
                            {
                                InterpretAzureDatabaseMetrics(dbName, rd);
                            }
                            if(rd.NextResult())
                            {
                                InterpretElasticPools(dbName,rd);
                            }
                           

                        }
                    }
                }
                Interlocked.Increment(ref databaseCount);
                if (databaseCount >= dbInfo.Count)
                {
                    ProbeHelpers.CalculateDatabaseStatistics(previousDbStatistics, refresh);

                    if (wmiConfig.DirectWmiEnabled)
                    {
                        StartMachineNameCollector();
                    }
                    else
                    {
                        StartResponseTimeCollector();
                    }
                }
                else
                {
                    StartServerOverviewCollectorAzure();
                }
            }
            catch(Exception ex)
            {
                //
            }
        }

        private void InterpretAzureDatabaseMetrics(String dbName,SqlDataReader rd)
        {
            Dictionary<string, object> currentAzureDBMetrics = new Dictionary<string, object>();
            string[] metricnames = Enum.GetNames(typeof(CloudMetricList.AzureMetric));
            while (rd.Read())
            {
                if (!rd.IsDBNull(0)) currentAzureDBMetrics.Add(metricnames[0], Convert.ToDouble(rd.GetValue(0), CultureInfo.InvariantCulture));
                if (!rd.IsDBNull(1)) currentAzureDBMetrics.Add(metricnames[1], Convert.ToDouble(rd.GetValue(1), CultureInfo.InvariantCulture));
                if (!rd.IsDBNull(2)) currentAzureDBMetrics.Add(metricnames[2], Convert.ToDouble(rd.GetValue(2), CultureInfo.InvariantCulture));
                if (!rd.IsDBNull(3)) currentAzureDBMetrics.Add(metricnames[3], Convert.ToDouble(rd.GetValue(3), CultureInfo.InvariantCulture));
                if (!rd.IsDBNull(5)) currentAzureDBMetrics.Add(metricnames[5], Convert.ToDouble(rd.GetValue(5), CultureInfo.InvariantCulture));
                if (!rd.IsDBNull(6)) currentAzureDBMetrics.Add(metricnames[6], Convert.ToDouble(rd.GetValue(6), CultureInfo.InvariantCulture));

                //START 5.4.2

                if (!rd.IsDBNull(0)) currentAzureDBMetrics.Add(metricnames[7], Convert.ToDouble(rd.GetValue(0), CultureInfo.InvariantCulture));
                if (!rd.IsDBNull(1)) currentAzureDBMetrics.Add(metricnames[8], Convert.ToDouble(rd.GetValue(1), CultureInfo.InvariantCulture));
                if (!rd.IsDBNull(2)) currentAzureDBMetrics.Add(metricnames[9], Convert.ToDouble(rd.GetValue(2), CultureInfo.InvariantCulture));
                if (!rd.IsDBNull(3)) currentAzureDBMetrics.Add(metricnames[10], Convert.ToDouble(rd.GetValue(3), CultureInfo.InvariantCulture));
                if (!rd.IsDBNull(5)) currentAzureDBMetrics.Add(metricnames[11], Convert.ToDouble(rd.GetValue(5), CultureInfo.InvariantCulture));
                if (!rd.IsDBNull(6)) currentAzureDBMetrics.Add(metricnames[12], Convert.ToDouble(rd.GetValue(6), CultureInfo.InvariantCulture));

            }
            if (!refresh.AzureCloudMetrics.ContainsKey(dbName))
                refresh.AzureCloudMetrics.Add(dbName, currentAzureDBMetrics);
            else
                refresh.AzureCloudMetrics[dbName] = currentAzureDBMetrics;
        }
        private void InterpretElasticPools(string dbName,SqlDataReader dataReader)
        {
            while (dataReader.Read())
            {

                string elasticPoolName = dataReader.IsDBNull(1) ? null : dataReader.GetString(1);
                string databaseName = dataReader.IsDBNull(0) ? null : dataReader.GetString(0);
                if (elasticPoolName != null && elasticPoolName!="" && refresh.AzureElasticPools.ContainsKey(elasticPoolName) && databaseName != null && databaseName != "")
                {
                    refresh.AzureElasticPools[elasticPoolName].Add(databaseName);
                }
                else if (elasticPoolName != null && elasticPoolName != "" && databaseName != null && databaseName != "")
                {
                    refresh.AzureElasticPools.Add(elasticPoolName, new List<string>() { databaseName });
                }

            }
            //Elastic Pool Support
            foreach (var pool in refresh.AzureElasticPools)
            {
                if (pool.Value.Contains(dbName))
                {
                    refresh.DbStatistics[dbName].ElasticPool = pool.Key;
                }

            }
        }

        /// <summary>
        /// Define the ResponseTime collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void ResponseTimeCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildResponseTimeCommand(conn);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ResponseTimeCallback));
        }
       
        /// <summary>
        /// Starts the Response Time collector.
        /// </summary>
        private void StartResponseTimeCollector()
        {
            StartGenericCollector(new Collector(ResponseTimeCollector), refresh, "StartResponseTimeCollector", "Response Time", refresh.ProductVersion, sqlErrorDelegate, ResponseTimeCallback, new object[] { });
        }

        /// <summary>
        /// Define the OS Metrics collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void OSMetricsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd = SqlCommandBuilder.BuildOSMetricsCommand(conn, ver, wmiConfig, refresh.Statistics.SQLProcessID, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(OSMetricsCallback));
        }


        /// <summary>
        /// Starts the OS Metrics collector
        /// </summary>
        void StartOSMetricsCollector()
        {
            if (wmiConfig.DirectWmiEnabled && cloudProviderId!= Constants.MicrosoftAzureManagedInstanceId)
                StartOSMetricsWmiCollector();
            else
                StartGenericCollector(new Collector(OSMetricsCollector), refresh, "StartOSMetricsCollector", "OS Metrics", 
                                     refresh.ProductVersion, sqlErrorDelegate, null, new object[] { });
        }

        private void StartOSMetricsWmiCollector()
        {
            if (driveProbe != null && !driveProbe.WMIStatisticsAvailable)
            {   // if the drive probe didn't complete then no use trying os metrics
                refresh.OSMetricsStatistics.OsStatisticAvailability = driveProbe.WMIServiceTimedout ? ServiceTimedOutText : ServiceUnavailableText;
                FireCompletion(refresh, Result.Success);
                return;
            }

            if (osProbe == null)
            { 
                osProbe = new OSStatisticsWmiMiniProbe(machineName, wmiConfig, refresh.Statistics.SQLProcessID);//START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe to get the machineName 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
            }

            osProbe.BeginProbe(OnOSMetricsWmiCollectorComplete);
        }

        private void OnOSMetricsWmiCollectorComplete(object sender, ProbeCompleteEventArgs args)
        {
            refresh.OSMetricsStatistics = osProbe.ReadOSMetrics(previousOSMetrics, refresh, LOG);

            //Calculate SQL CPU percent divide against logical processors
            if (refresh.ProductVersion.Major > 8)
                refresh.Statistics.CpuPercentage = refresh.OSMetricsStatistics.PercentSQLProcessorTime / refresh.Statistics.LogicalProcessors;
            FireCompletion(refresh, Result.Success);
        }


        /// <summary>
        /// Define the ConnectionStatus callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ConnectionStatusCallback(CollectorCompleteEventArgs e)
        {
            // If we get here, the connection was successful, so just keep going.
            StartServerOverviewCollector();
        }

        /// <summary>
        /// Callback used to process the data returned from the ConnectionStatus collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ConnectionStatusCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(ConnectionStatusCallback, refresh, "ConnectionStatusCallback", "Connection Status", sender, e);
        }

        /// <summary>
        /// Override the HandleSqlException method because the purpose if this prob is to determine if we are unable to connect.  The actual
        /// query is select 1, so the only error should be unable to connect, bad password, etc.
        /// </summary>
        protected void ConnectionErrorDelagate(Snapshot snapshot, string collectorName, SqlException sqlException, SqlCollector collector)
        {
            //Clear out any data collected so far
            refresh = new ServerOverview(refresh.ServerName);
            if (sqlException.Number == 17142)
            {
                LOG.Info("Monitored server is paused");
                //Set the server as paused
                refresh.SqlServiceStatus = ServiceState.Paused;
            }
            else
            {
                //Set the service status as unable to connect
                refresh.SqlServiceStatus = ServiceState.UnableToConnect;
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Monitored server cannot be contacted: " + sqlException.Message, false);
            }
            
            if (collector != null)
                collector.Dispose();
            FireCompletion(refresh, Result.Success);
        }

        /// <summary>
        /// Define the Server Overview callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void ServerOverviewCallback(CollectorCompleteEventArgs e)
        {
            if (e.ElapsedMilliseconds.HasValue)
            {
                refresh.ResponseTime = e.ElapsedMilliseconds.Value;
            }

            if (refresh.ServerStartupTime != serverStartupTime)
            {
                if (serverStartupTime.HasValue && refresh.ServerStartupTime.HasValue)
                    LOG.Info("Server restart detected.  Disposing of previous metrics.");
                previousServerStatistics = new ServerStatistics();
                previousOSMetrics = new OSMetrics();

                lastRefresh = null;
            }

            if (lastRefresh.HasValue && refresh.TimeStamp > lastRefresh.Value)
            {
                refresh.TimeDelta = refresh.TimeStamp - lastRefresh.Value;
            }
            
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                //get SQL permission to view if has SysAdmin permission or not
                //string userPermission = GetSqlSysPermission(rd);

                //if ("IsSysAdmin".Equals(userPermission))
                {

                    if (cloudProviderId == Constants.MicrosoftAzureManagedInstanceId)
                    {
                        InterpretAzureManagedInstance(rd);
                        rd.NextResult();
                    }

                    ServerOverviewInterpreter.InterpretForOnDemand(rd, previousServerStatistics, refresh,
                        refresh, LOG, clusterCollectionSetting, lastRefresh,cloudProviderId);

                    InterpretMemory(rd);

                    ProbeHelpers.ReadDatabaseStatistics(rd, previousDbStatistics, refresh, LOG);

                    InterpretFileActivity(rd);

                    InterpretTempdbSummary(rd);

                    if (rd.NextResult())
                    {
                        ReadWaitStats(rd);
                    }
                }
                if (cloudProviderId == Constants.MicrosoftAzureManagedInstanceId)
                {
                    if (rd.NextResult())
                    {
                        InterpretAzureDatabaseMetrics("master", rd);
                    }

                }
                //else if ("IsNotSysAdmin".Equals(userPermission))
                //{
                //    ServerOverviewInterpreter.InterpretForOnDemandNoSysAdmin(rd, previousServerStatistics, 
                //        refresh, refresh, LOG, clusterCollectionSetting, lastRefresh);
                //}
            }

            if (virtualizationConfiguration != null)
            {
                refresh.VMConfig = CollectVMInfo();
            }
            else
            {
                LOG.Debug("[ServerOverview] skipping VM Collection ");
            }

            if (cloudProviderId == Constants.AmazonRDSId)
            {

                StartAWSCloudWatchMetricCollector();
            }
            else
            {

                if (wmiConfig.DirectWmiEnabled)
                    StartMachineNameCollector();//START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe before driveStatisticsProbe
                                                //StartDriveStatisticsProbe();
                else
                    StartResponseTimeCollector();
            }
        
        }
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
                            null,
                            sender,
                            e,
                            false,
                            false);
        }

        private void AWSCloudWatchMetricCollectorCallback(CollectorCompleteEventArgs e)
        {
            Dictionary<string, double> awsMetrics = e.Value as Dictionary<string, double>;
            InterpretAWSMetrics(awsMetrics);
            if (wmiConfig.DirectWmiEnabled)
                StartMachineNameCollector();//START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe before driveStatisticsProbe
                                            //StartDriveStatisticsProbe();
            else
                StartResponseTimeCollector();
        }

        private void InterpretAWSMetrics(Dictionary<string, double> awsMetrics)
        {
            refresh.AWSCloudMetrics = awsMetrics;
            
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
                ProbeHelpers.ReadAzureDbDetail(dataReader, refresh, LOG, refresh, GenericFailureDelegate);
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting Memory Collector: {0}", e,
                                                    false);
                GenericFailureDelegate(refresh);
            }
        }

        /// <summary>
        /// Get string if if SysAdmin or not
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns>IsSQLSysAdmin/IsNotSQLSysAdmin</returns>
        private String GetSqlSysPermission(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("IsSQLSysAdmin"))
            {
                try
                {
                    if (dataReader.Read())
                    {
                        Int32 ordinal = dataReader.GetOrdinal("SQLPermission");

                        if (!dataReader.IsDBNull(ordinal))
                        {
                            return dataReader.GetString(ordinal) as string;
                        }
                    }
                    return String.Empty;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception.ToString());
                    return String.Empty;
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }

		//START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
        /// <summary>
        /// Starts the Machine Name collector.
        /// before the WMI probe for disk statistics and OS Metric which need the machine name 
        /// </summary>
        private void StartMachineNameCollector()
        {
            MachineNameProbe machineProbe = new MachineNameProbe(connectionInfo,cloudProviderId);
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
                return;
            if (e.Snapshot != null)
            {
                var _machineSnapshot = e.Snapshot as MachineNameSnapshot;
                if (_machineSnapshot != null)
                    machineName = _machineSnapshot.MachineName;
            }

            ((IDisposable)sender).Dispose();

            // start the next probe
            StartDriveStatisticsProbe();
        }
		//END SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm

        private void StartDriveStatisticsProbe()
        {
            //driveProbe = new DriveStatisticsWmiProbe(refresh.ServerHostName, wmiConfig);
            driveProbe = new DriveStatisticsWmiProbe(machineName, wmiConfig, diskSettings);//START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe to get the machineName 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
            driveProbe.AutoDiscovery = diskSettings.AutoDiscover;
            if (!diskSettings.AutoDiscover && diskSettings.Drives != null && diskSettings.Drives.Length > 0)
            {
                driveProbe.IncludeAll = false;
                driveProbe.DriveList.AddRange(diskSettings.Drives);
            }
            driveProbe.BeginProbe(OnDiskStatsComplete);
        }

        private void OnDiskStatsComplete(object sender, ProbeCompleteEventArgs args)
        {
            //START SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - replaced older object with the new object type
            var probedata = args.Data as DriveStatisticsWmiProbe.DriveStatisticsWmiDetails;
            if (probedata != null && probedata.DiskMap !=null  && probedata.DiskMap.Count > 0)
            {
                AddDiskStatistics(probedata.DiskMap.Values);
            }
            driveProbeData = probedata;
            //END SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - replaced older object with the new object type
            StartResponseTimeCollector();
        }

        private void AddDiskStatistics(IEnumerable<DriveStatisticsWmiProbe.DiskStatistics> valueCollection)
        {
            var diskDrives = new Dictionary<string, DiskDrive>();

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

                    ProbeHelpers.CalcDiskDrive(dd, config.PreviousDiskDrives);

                    diskDrives.Add(dd.DriveLetter, dd);
                }
            }

            refresh.IsFileSystemObjectDataAvailable = (diskDrives.Count > 0);

            refresh.DiskDrives = diskDrives;

            MapFilesToDisks();
        }

        private void MapFilesToDisks()
        {
            var driveList = new List<string>(refresh.DiskDrives.Keys);
            driveList.Sort(DriveComparison);

            foreach (var file in fileMap.Values)
            {
                var drive = MatchDrive(driveList, file);
                if (String.IsNullOrEmpty(drive)) continue;
                file.DriveName = drive;
                CalculateFile(file);
                if (!refresh.FileActivity.ContainsKey(file.Filepath))
                {
                    refresh.FileActivity.Add(file.Filepath, file);
                }
            }
        }

        private int DriveComparison(string left, string right)
        {
            return string.Compare(left, right, true);
        }

        private string MatchDrive(List<string> driveList, FileActivityFile file)
        {
            var filepath = file.Filepath;
            for (var i = driveList.Count - 1; i >= 0; i--)
            {
                if (filepath.StartsWith(driveList[i], StringComparison.CurrentCultureIgnoreCase))
                    return driveList[i];
            }

            // file does not match a mount point
            return null;
        }


        private VMwareVirtualMachine CollectVMInfo()
        {
            using (LOG.DebugCall("[ServerOverviewProbe] Collect VM Counters"))
            {
                LOG.DebugFormat("Collecting for {0}", refresh.ServerName);
                VMwareVirtualMachine result = null;
                int iterations = 1;
                if (lastRefresh.HasValue)
                {
                    TimeSpan? timeDelta = refresh.TimeStamp - lastRefresh;
                    iterations = timeDelta.HasValue ? timeDelta.Value.Seconds%20 : 1;
                }

                try
                {
                    Stopwatch timer = new Stopwatch();
                    timer.Start();

                    if (virtualizationConfiguration.VCServerType.Equals("HyperV"))
                    {
                        HyperVService hyperVService = new HyperVService(virtualizationConfiguration.VCAddress);
                        ScheduledRefresh sr = null;
                        try
                        {
                           // hyperVService.Connect(virtualizationConfiguration.VCUser, virtualizationConfiguration.VCPassword);
                            LOG.DebugFormat("Elapsed Time to connect {0} ms", timer.ElapsedMilliseconds.ToString());

                            //if (hyperVService.ConnectState == Idera.SQLdm.Common.HyperV.ConnectState.Connected)
                            //{
                                LOG.InfoFormat("Collecting {0} iteration{1} of VM Performance Statistics for {2}", iterations, iterations > 1 ? "s" : "", refresh.ServerName);
                                result = hyperVService.CollectVMInfo(virtualizationConfiguration, iterations, sr, previousVconfig, wmiConfig);
                                LOG.DebugFormat("Results collected in {0} ms", timer.ElapsedMilliseconds.ToString());
                            //}

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
                        ServiceUtil vimService = new ServiceUtil(virtualizationConfiguration.VCAddress);
                        try
                        {
                            vimService.Connect(virtualizationConfiguration.VCUser, virtualizationConfiguration.VCPassword);
                            LOG.DebugFormat("Elapsed Time to connect {0} ms", timer.ElapsedMilliseconds.ToString());

                            if (vimService.ConnectState == Idera.SQLdm.Common.VMware.ConnectState.Connected)
                            {
                                LOG.InfoFormat("Collecting {0} iteration{1} of VM Performance Statistics for {2}", iterations, iterations > 1 ? "s" : "", refresh.ServerName);
                                result = vimService.CollectVMInfo(virtualizationConfiguration.InstanceUUID, iterations);
                                LOG.DebugFormat("Results collected in {0} ms", timer.ElapsedMilliseconds.ToString());
                            }

                            vimService.Disconnect();
                        }
                        catch (Exception e)
                        {
                            LOG.Error("Error collecting VM Configuration and Statistics", e);
                            if (vimService.ConnectState == Idera.SQLdm.Common.VMware.ConnectState.Connected)
                            {
                                vimService.Disconnect();
                            }
                        }
                    }
                    
                    

                    timer.Stop();
                    LOG.InfoFormat("[ServerOverviewProbe] VM Collector completed in {0} ms", timer.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    LOG.ErrorFormat("Server Overview Probe errored connecting to Virtualization Host ({0})", virtualizationConfiguration.VCAddress);
                }
            return result;
            }
        }


        /// <summary>
        /// Callback used to process the data returned from the server overview collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void ServerOverviewCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(ServerOverviewCallback), refresh, "ServerOverviewCallback", "Server Overview",
                            sender, e);
        }

        /// <summary>
        /// Define the Response Time callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void ResponseTimeCallback(CollectorCompleteEventArgs e)
        {
            if (e.ElapsedMilliseconds.HasValue)
            {
                refresh.ResponseTime = e.ElapsedMilliseconds.Value;
            }
           
            if (wmiConfig.DirectWmiEnabled|| cloudProviderId == CLOUD_PROVIDER_ID_AZURE)
                StartOSMetricsCollector();
            else
            {
               
                StartDiskDrivesCollector();
             
            }
        }

        /// <summary>
        /// Callback used to process the data returned from the ResponseTime collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ResponseTimeCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(ResponseTimeCallback), refresh, "ResponseTimeCallback", "Response Time",
                            sender, e);
        }

        /// <summary>
        /// Define the OS Metrics callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void OSMetricsCallback(CollectorCompleteEventArgs e)
        {
            LOG.Verbose("Entering OS Metrics Callback Delegate");
            using (SqlDataReader rd = e.Value as SqlDataReader)
                
            {
                if(cloudProviderId == Constants.AmazonRDSId)
                    refresh.OSMetricsStatistics =
                   ProbeHelpers.ReadAWSOSMetrics(rd, previousOSMetrics, refresh, LOG, cloudProviderId,refresh.ServerName ?? refresh.RealServerName);
                else
                refresh.OSMetricsStatistics =
                    ProbeHelpers.ReadOSMetrics(rd, previousOSMetrics, refresh, LOG, cloudProviderId);

                rd.NextResult();
                ProbeHelpers.ReadAzureManagedInstancePages(rd, refresh, LOG, refresh, GenericFailureDelegate);
                if (cloudProviderId == Constants.MicrosoftAzureManagedInstanceId)
                {
                    rd.NextResult();
                    ProbeHelpers.ReadAvgCpuPercent(rd, refresh, LOG, refresh, GenericFailureDelegate);
                }
               

                //Calculate SQL CPU percent divide against logical processors
                if (refresh.ProductVersion.Major > 8)
                    refresh.Statistics.CpuPercentage = refresh.OSMetricsStatistics.PercentSQLProcessorTime / refresh.Statistics.LogicalProcessors;

            }
            LOG.Verbose("Firing Completion from OS Metrics Callback Delegate");

            FireCompletion(refresh, Result.Success);
            LOG.Verbose("Exiting OS Metrics Callback Delegate");
        }

        /// <summary>
        /// OS Metrics failure delegate
        /// Since OS Metrics are optional we do not want to fail the entire refresh if unavailable
        /// </summary>
        /// <param name="snapshot">Snapshot to return with partial failure</param>
        protected void OSMetricsFailureDelegate(Snapshot snapshot, Exception e)
        {
            FireCompletion(snapshot, Idera.SQLdm.Common.Services.Result.Partial);
        }

        /// <summary>
        /// Callback used to process the data returned from the OS Metrics collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The Idera.SQLdm.Probes.Collectors.CollectorCompleteEventArgs instance containing the event data.</param>
        void OSMetricsCallback(object sender, CollectorCompleteEventArgs e)
        {
            LOG.Verbose("Entering OS Metrics Callback");
            //If OS Metrics fails we do not want to fail the entire snapshot, so return partial
            GenericCallback(new CollectorCallback(OSMetricsCallback), refresh, "OSMetricsCallback", "OS Metrics",
                            new FailureDelegate(OSMetricsFailureDelegate),new FailureDelegate(OSMetricsFailureDelegate),
                           sender, e);
            LOG.Verbose("Exiting OS Metrics Callback");
        }

        /// <summary>
        /// Define the DiskDrives collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void DiskDrivesCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildDiskDrivesCommand(conn, ver, wmiConfig, diskSettings, driveProbeData,cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DiskDrivesCallback));
        }

        /// <summary>
        /// Starts the Disk Dives collector.
        /// </summary>
        private void StartDiskDrivesCollector()
        {
            //Arg params for DiskSize2005
            var driveProbeDataParam = (DriveStatisticsWmiProbe.DriveStatisticsWmiDetails)driveProbeData;
            bool isWMICallFailed = (driveProbeDataParam == null || driveProbeDataParam.DiskMap == null || driveProbeDataParam.DiskMap.Count == 0);
            StartGenericCollector(new Collector(DiskDrivesCollector), refresh, "StartDiskDrivesCollector", "Disk Drives", null, new object[] { isWMICallFailed });
        }

        /// <summary>
        /// Define the DiskDrives callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DiskDrivesCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretDiskDrives(rd);
            }
        }

        /// <summary>
        /// Callback used to process the data returned from the DiskDrives collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DiskDrivesCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(DiskDrivesCallback), refresh, "DiskDrivesCallback", "Disk Dives",
                            sender, e);
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
                    bool IsFileSystemObjectDataAvailable;
                    refresh.DiskDrives = ProbeHelpers.ReadDiskDrives(dataReader, config.PreviousDiskDrives, out IsFileSystemObjectDataAvailable, LOG);
                    refresh.IsFileSystemObjectDataAvailable = IsFileSystemObjectDataAvailable;
                    StartOSMetricsCollector();
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting Disk Dives Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(refresh);
                }
            }
        }

        /// <summary>
        /// Interpret Memory data
        /// </summary>
        private void InterpretMemory(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretMemory"))
            {
                try
                {
                    dataReader.NextResult();
                    ReadMemoryDiagnostics(dataReader);
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting Memory Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(refresh);
                }
            }
        }
        //6.2.4
        private void InterpretAzureIO(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretAzureIO"))
            {
                try
                {
                    dataReader.NextResult();
                    AzureIODetails(dataReader);
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting IO Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(refresh);
                }
            }
        }

        //6.2.4
        private void AzureIODetails(SqlDataReader dataReader)
        {
            try
            {
                ProbeHelpers.ReadAzureIO(dataReader, refresh, LOG, refresh, GenericFailureDelegate);
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting IO Collector: {0}", e,
                                                    false);
                GenericFailureDelegate(refresh);
            }
        }

        // 6.2.3
        private void InterpretAzureStorageSizeLimit(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretMemory"))
            {
                try
                {
                    ReadAzureStorageSizeLimit(dataReader);
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting Memory Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(refresh);
                }
            }
        }

        // 6.2.3
        private void ReadAzureStorageSizeLimit(SqlDataReader dataReader)
        {
            try
            {
                ProbeHelpers.ReadAzureStorageSizeLimit(dataReader, refresh, LOG, refresh, GenericFailureDelegate);
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting Memory Collector: {0}", e,
                                                    false);
                GenericFailureDelegate(refresh);
            }
        }

        private void InterpretAzureManagedInstance(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretAzureIO"))
            {
                try
                {
                    AzureManagedInstanceDetails(dataReader);
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting Managed Instance Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(refresh);
                }
            }
        }

        //5.1
        private void AzureManagedInstanceDetails(SqlDataReader dataReader)
        {
            try
            {
                ProbeHelpers.ReadAzureManagedInstance(dataReader, refresh, LOG, refresh, GenericFailureDelegate);
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting Managed Instance Collector: {0}", e,
                                                    false);
                GenericFailureDelegate(refresh);
            }
        }


        private void ReadMemoryDiagnostics(SqlDataReader dataReader)
        {
            try
            {
                ProbeHelpers.ReadMemoryCounters(dataReader, refresh, LOG, refresh.MemoryStatistics, refresh.ProductVersion, GenericFailureDelegate);
    
                //if (refresh.ProductVersion.Major < 9)
                //{
                //    ReadPinnedTables(dataReader);
                //}
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting Memory Collector: {0}", e,
                                                    false);
                GenericFailureDelegate(refresh);
            }
        }

        //private void ReadPinnedTables(SqlDataReader dataReader)
        //{
        //    try
        //    {
        //        bool moreResults = true;

        //        while (moreResults)
        //        {
        //            // If a record exists, a pinned table exists
        //            while (dataReader.Read())
        //            {
        //                if (dataReader.FieldCount == 3 && dataReader.HasRows)
        //                {
        //                    PinnedTable pinnedTable = new PinnedTable();
        //                    if (!dataReader.IsDBNull(0)) pinnedTable.Name = dataReader.GetString(0);
        //                    if (!dataReader.IsDBNull(1)) pinnedTable.UsedSpace = dataReader.GetInt32(1);
        //                    if (!dataReader.IsDBNull(2)) pinnedTable.RowCount = dataReader.GetInt32(2);

        //                    refresh.MemoryStatistics.PinnedTables.Add(pinnedTable);
        //                }
        //            }

        //            moreResults = dataReader.NextResult();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting Memory Pinned Tables: {0}", e,
        //                                            false);
        //        GenericFailureDelegate(refresh);
        //    }
        //    finally
        //    {
        //        dataReader.NextResult();
        //    }
        //}


        private void ReadWaitStats(SqlDataReader dataReader)
        {
            if (refresh == null) return;
            refresh.WaitStats = new WaitStatisticsSnapshot();
            refresh.WaitStats.Waits = ProbeHelpers.ReadWaitStatistics(dataReader, refresh.TimeDelta);

            if (previousWaitStats != null && previousWaitStats.Waits != null)
            {
                refresh.WaitStats.CalculateWaitsDeltas(previousWaitStats.Waits);
            }
        }

        /// <summary>
        /// Interpret TempdbSummary data
        /// </summary>
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
                                                        previousTempdbStatistics,
                                                        ref tempdbStats,
                                                        ref tempdbFiles,
                                                        previousTempdbStatistics != null
                                                            ? previousTempdbStatistics.TimeStamp : null,
                                                        refresh.TimeStamp);
                    TempdbSummaryStatistics tempdbSummaryStats = new TempdbSummaryStatistics(tempdbStats);

                    foreach (TempdbFileActivity tempdbFile in tempdbFiles)
                    {
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
                    refresh.TempdbStatistics = tempdbSummaryStats;
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

        /// <summary>
        /// Interpret FileActivity data
        /// </summary>
        private void InterpretFileActivity(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretFileActivity"))
            {
                var directWmi = wmiConfig.DirectWmiEnabled;
                try
                {
                    if (!directWmi)
                    {
                        dataReader.NextResult();
                        // skip OsStatisticAvailability
                        dataReader.NextResult();
                        // skip Disk results
                        dataReader.NextResult();
                        ReadFiles(dataReader);
                    }
                    else
                    {
                        dataReader.NextResult();
                        dataReader.NextResult();
                        ReadFilesDirectWmi(dataReader);
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error interpreting File Activity Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(refresh);
                }
            }
        }

        private void ReadFilesDirectWmi(SqlDataReader dr)
        {
            try
            {
                fileMap = new Dictionary<string, FileActivityFile>();
                while (dr.Read())
                {
                    if (!dr.IsDBNull(6)) // skip null file path
                    {
                        var filePath = dr.GetString(6);
                        if (!fileMap.ContainsKey(filePath))
                        {
                            var file = new FileActivityFile();
                            if (!dr.IsDBNull(0)) file.DatabaseName = dr.GetString(0);
                            if (!dr.IsDBNull(1)) file.ReadsRaw = dr.GetDecimal(1);
                            if (!dr.IsDBNull(2)) file.WritesRaw = dr.GetDecimal(2);
                            if (!dr.IsDBNull(3)) file.DriveName = dr.GetString(3);
                            if (!dr.IsDBNull(4)) file.Filename = dr.GetString(4);
                            if (!dr.IsDBNull(5)) file.FileType = (FileActivityFileType) dr.GetInt32(5);
                            file.Filepath = filePath;
                            fileMap.Add(filePath, file);
                        }
                        else
                        {
                            LOG.ErrorFormat("An error has occurred while adding {0} (this Key already exists).", filePath);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error reading files: {0}", e, false);
                GenericFailureDelegate(refresh);
            }
        }

        private void ReadFiles(SqlDataReader dr)
        {
            try
            {
                while (dr.Read())
                {

                    if (!dr.IsDBNull(3) && !dr.IsDBNull(6))
                    {
                        {
                            FileActivityFile file = new FileActivityFile();
                            if (!dr.IsDBNull(0)) file.DatabaseName = dr.GetString(0);
                            if (!dr.IsDBNull(1)) file.ReadsRaw = dr.GetDecimal(1);
                            if (!dr.IsDBNull(2)) file.WritesRaw = dr.GetDecimal(2);
                            file.DriveName = dr.GetString(3);
                            if (!dr.IsDBNull(4)) file.Filename = dr.GetString(4);
                            if (!dr.IsDBNull(5)) file.FileType = (FileActivityFileType)dr.GetInt32(5);
                            file.Filepath = dr.GetString(6);
                            CalculateFile(file);
                            if (!refresh.FileActivity.ContainsKey(file.Filepath))
                            {
                                refresh.FileActivity.Add(file.Filepath, file);
                            }
                            else
                            {
                                LOG.ErrorFormat("An error has occurred while adding {0} (this Key already exists).", file.Filepath);
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Error reading files: {0}", e,
                                                    false);
                GenericFailureDelegate(refresh);
            }
        }

        private void CalculateFile(FileActivityFile file)
        {
            if (config != null
                && previousFileActivity != null
                && previousFileActivity.ContainsKey(file.Filepath)
                && refresh.TimeDelta.HasValue
                && refresh.TimeDelta.Value.TotalSeconds > 0
                )
            {
                FileActivityFile prevActivity;
                if (previousFileActivity.TryGetValue(file.Filepath, out prevActivity))
                {
                    if (file.ReadsRaw.HasValue && prevActivity.ReadsRaw.HasValue &&
                        file.ReadsRaw.Value >= prevActivity.ReadsRaw.Value)
                    {
                        file.Reads = file.ReadsRaw.Value - prevActivity.ReadsRaw.Value;
                        file.ReadsPerSec =
                            (double?)
                            (file.Reads /
                             (decimal) refresh.TimeDelta.Value.TotalSeconds);
                    }

                    if (file.WritesRaw.HasValue && prevActivity.WritesRaw.HasValue &&
                        file.WritesRaw.Value >= prevActivity.WritesRaw.Value)
                    {
                        file.Writes = file.WritesRaw.Value - prevActivity.WritesRaw.Value;
                        file.WritesPerSec =
                            (double?)
                            (file.Writes /
                             (decimal) refresh.TimeDelta.Value.TotalSeconds);
                    }

                    decimal? transfersRaw = file.ReadsRaw + file.WritesRaw;
                    decimal? prevTransfersRaw = prevActivity.ReadsRaw + prevActivity.WritesRaw;
                    if (transfersRaw.HasValue && prevTransfersRaw.HasValue &&
                        transfersRaw.Value >= prevTransfersRaw.Value)
                    {
                        file.Transfers = transfersRaw.Value - prevTransfersRaw.Value;
                        file.TransfersPerSec =
                            (double?)
                            (file.Transfers /
                             (decimal)refresh.TimeDelta.Value.TotalSeconds);
                    }
                }
            }
        }

        /// <summary>
        /// Override the HandleSqlException method, because in this probe both Paused and unable to monitor are not errors
        /// </summary>
        protected new void HandleSqlException(Snapshot snapshot, string collectorName, SqlException sqlException, SqlCollector collector)
        {
            // 17142 is returned when the SQL Server service is paused
            if (sqlException.Number == 17142)
            {
                LOG.Info("Monitored server is paused");
                //Clear out any data collected so far
                refresh = new ServerOverview(refresh.ServerName);
                //Set the server as paused
                refresh.SqlServiceStatus = ServiceState.Paused;
                if (collector != null)
                    collector.Dispose();
                FireCompletion(refresh, Result.Success);
            }
            else
            {
                if (sqlException.Number == 2 || sqlException.Number == 53)
                {
                    LOG.Info("Monitored server cannot be contacted: " + sqlException.Message);
                    //Clear out any data collected so far
                    refresh = new ServerOverview(refresh.ServerName);
                    //Set the service status as unable to monitor
                    refresh.SqlServiceStatus = ServiceState.UnableToMonitor;
                    if (collector != null)
                        collector.Dispose();
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

        #region interface implementations

        #endregion

        #region nested types

        #endregion

       }
}
