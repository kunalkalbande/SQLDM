//------------------------------------------------------------------------------
// <copyright file="ServerActionProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Idera.SQLdm.Common.Configuration.ServerActions;


namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using BBS.TracerX;
    using Collectors;
    using Common;
    using Common.Configuration;
    using Common.Services;
    using Common.Snapshots;
    using Microsoft.ApplicationBlocks.Data;

    /// <summary>
    /// On-demand probe for server sessions
    /// </summary>
    internal class ServerActionProbe : SqlBaseProbe
    {
        #region fields

        private Snapshot snapshot = null;
        IServerActionConfiguration serverActionConfig = null;
        private Collector serverActionCollector = new Collector(UndefinedCollector);
        private CollectorCallback serverActionCallback = null;


        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public ServerActionProbe(SqlConnectionInfo connectionInfo, IServerActionConfiguration configuration, int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            snapshot = new ServerActionSnapshot(connectionInfo.InstanceName);
            string logName = "UndefinedServerAction";

            serverActionConfig = configuration;
            Type serverActionType = configuration.GetType();

            serverActionCallback = new CollectorCallback(DefaultCallback);

            if (serverActionType == typeof(FreeProcedureCacheConfiguration))
            {
                logName = "FreeProcedureCache";
                serverActionCollector = new Collector(FreeProcedureCacheCollector);
            }

            if (serverActionType == typeof(FullTextActionConfiguration))
            {
                logName = "FullTextAction";
                serverActionCollector = new Collector(FullTextActionCollector);
            }

            if (serverActionType == typeof(JobControlConfiguration))
            {
                logName = "JobControl";
                serverActionCollector = new Collector(JobControlCollector);
            }


            if (serverActionType == typeof(KillSessionConfiguration))
            {
                logName = "KillSession";
                serverActionCollector = new Collector(KillSessionCollector);
            }

            if (serverActionType == typeof(ReconfigurationConfiguration))
            {
                logName = "ReconfigureServer";
                serverActionCollector = new Collector(ReconfigurationCollector);
            }
            if (serverActionType == typeof(BlockedProcessThresholdConfiguration))
            {
                logName = "BlockedProcessThresholdChange";
                serverActionCollector = BlockedProcessThresholdChangeCollector;
            }
            if (serverActionType == typeof(ServiceControlConfiguration))
            {
                logName = "ServiceControl";
                serverActionCollector = new Collector(ServiceControlCollector);
            }

            if (serverActionType == typeof(ShutdownSQLServerConfiguration))
            {
                logName = "ShutdownSQLServer";
                serverActionCollector = new Collector(ShutdownSQLServerCollector);
            }

            if (serverActionType == typeof(SetNumberOfLogsConfiguration))
            {
                logName = "SetNumberOfSqlLogs";
                serverActionCollector = new Collector(SetNumberOfLogsCollector);
            }

            if (serverActionType == typeof(StopSessionDetailsTraceConfiguration))
            {
                logName = "StopSessionDetailsTrace";
                serverActionCollector = new Collector(StopSessionDetailsTraceCollector);
            }

            if (serverActionType == typeof(StopQueryMonitorTraceConfiguration))
            {
                logName = "StopQueryMonitorTraceAndExtendedEvent"; //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session -- Changed the logName
                serverActionCollector = new Collector(StopQueryMonitorAllCollector);

            }

            if (serverActionType == typeof(StopActivityMonitorTraceConfiguration))
            {
                logName = "StopActivityMonitorTrace";
                serverActionCollector = new Collector(StopActivityMonitorTraceCollector);
            }

            //SQLdm 9.0 (Ankit Srivastava) - Query Monitor with Extended Event Session - this part is useless since the type StartQueryMonitorTraceConfiguration is not use anywhere
            //if (serverActionType == typeof(StartQueryMonitorTraceConfiguration))
            //{
            //    logName = "StartQueryMonitorTrace";
            //   serverActionCollector = new Collector(StartQueryMonitorTraceCollector);
            //}

            if (serverActionType == typeof(RecycleLogConfiguration))
            {
                logName = "RecycleLog";
                serverActionCollector = new Collector(RecycleLogCollector);
            }

            if (serverActionType == typeof(RecycleAgentLogConfiguration))
            {
                logName = "RecycleAgentLog";
                serverActionCollector = new Collector(RecycleAgentLogCollector);
            }

            if (serverActionType == typeof(ReindexConfiguration))
            {
                snapshot = new ReindexSnapshot(connectionInfo.InstanceName);
                logName = "Reindex";
                serverActionCollector = new Collector(ReindexCollector);
            }

            if (serverActionType == typeof(UpdateStatisticsConfiguration))
            {
                logName = "UpdateStatistics";
                serverActionCollector = new Collector(UpdateStatisticsCollector);
            }
            if (serverActionType == typeof(MirroringPartnerActionConfiguration))
            {
                logName = "MirroringPartnerAction";
                serverActionCollector = new Collector(MirroringPartnerActionsCollector);
            }

            if (serverActionType == typeof(AdhocQueryConfiguration))
            {
                snapshot = new AdhocQuerySnapshot(connectionInfo.InstanceName);
                ((AdhocQuerySnapshot) snapshot).Configuration = (AdhocQueryConfiguration)configuration;
                logName = "AdhocQuery";
                serverActionCollector = new Collector(AdhocQueryCollector);

            }

           
            LOG = Logger.GetLogger(logName);
            serverActionConfig = configuration;
        }

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Starts the probe, virtual method called by BaseProbe.BeginProbe().
        /// If this returns cleanly, probe must call FireCompletion()
        /// </summary>
        protected override void Start()
        {
            if (serverActionConfig != null && ((OnDemandConfiguration)serverActionConfig).ReadyForCollection)
            {
                StartServerActionCollector();
            }
            else
            {
                FireCompletion(snapshot, Result.Success);
            }
        }

        /// <summary>
        /// Define the default collector in case the configuration object is not recognized
        /// </summary>
        static void UndefinedCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            
        }

        /// <summary>
        /// Free procedure cache
        /// </summary>
        void FreeProcedureCacheCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildFreeProcedureCacheCommand(conn, ver);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerActionCallback));
        }

        /// <summary>
        /// Perform an action on a fulltext catalog
        /// </summary>
        void FullTextActionCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildFullTextActionCommand(conn, ver, (FullTextActionConfiguration)serverActionConfig);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerActionCallback));
        }

        /// <summary>
        /// Start/stop SQL Agent Job
        /// </summary>
        void JobControlCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildJobControlCommand(conn, ver, (JobControlConfiguration)serverActionConfig);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerActionCallback));
        }

        /// <summary>
        /// Kill SQL Server session
        /// </summary>
        void KillSessionCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildKillSessionCommand(conn, ver, (KillSessionConfiguration)serverActionConfig);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerActionCallback));
        }

        /// <summary>
        /// Reconfigure the SQL Server
        /// </summary>
        void ReconfigurationCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildReConfigurationCommand(conn, ver, (ReconfigurationConfiguration)serverActionConfig, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerActionCallback));
        }
        
        /// <summary>
        /// Change the Blocked Process Threshold
        /// </summary>
        void BlockedProcessThresholdChangeCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
            SqlCommand cmd =
                           SqlCommandBuilder.BuildBlockedProcessThresholdChangeCommand(conn, ver, (BlockedProcessThresholdConfiguration)serverActionConfig, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(ServerActionCallback);
        }
        /// <summary>
        /// Control services on the SQL Server
        /// </summary>
        void ServiceControlCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildServiceControlCommand(conn, ver, (ServiceControlConfiguration)serverActionConfig,snapshot.ServerName);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServiceControlCallback));
        }


        /// <summary>
        /// Shutdown the SQL Server
        /// </summary>
        void ShutdownSQLServerCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildServerShutdownCommand(conn, ver, ((ShutdownSQLServerConfiguration)serverActionConfig).WithNoWait);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerActionCallback));
        }

        /// <summary>
        /// Set number of SQL Server logs
        /// </summary>
        void SetNumberOfLogsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildSetNumberOfLogsCommand(conn, ver, (SetNumberOfLogsConfiguration)serverActionConfig);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerActionCallback));
        }

        /// <summary>
        /// Stop the session detail trace
        /// </summary>
        void StopSessionDetailsTraceCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
            SqlCommand cmd =
                           SqlCommandBuilder.BuildStopSessionDetailsTraceCommand(conn, ver, (StopSessionDetailsTraceConfiguration)serverActionConfig, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerActionCallback));
        }

        /// <summary>
        /// Stop the query monitor trace
        /// </summary>
        //void StopQueryMonitorTraceCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        //{
        //    SqlCommand cmd =
        //                   SqlCommandBuilder.BuildQueryMonitorStopCommand(conn, ver);
        //    sdtCollector = new SqlCollector(cmd, true);
        //    sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerActionCallback));
        //}

        /// <summary>
        /// Stop the query monitor Extended Event Session and Trace 
        /// </summary>
        void StopQueryMonitorAllCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
		//SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session -- Changed the MethodCall from StopCommand to StopCommandAll
            // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
            SqlCommand cmd =
                           SqlCommandBuilder.BuildQueryMonitorStopCommandAll(conn, ver, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerActionCallback));
        }

        /// <summary>
        /// Stop the Activity monitor trace
        /// </summary>
        void StopActivityMonitorTraceCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
            SqlCommand cmd =
                           SqlCommandBuilder.BuildActivityMonitorStopCommandAll(conn, ver, cloudProviderId);//SQLdm 9.1 (Ankit Srivastava) - Activity Monitoring with Extended Events  - modified stop method call in case of deletion
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerActionCallback));
        }

        //SQLdm 9.0 (Ankit Srivastava) - Query Monitor with Extended Event Session - this part is useless since the method StartQueryMonitorTraceCollector is not use anywhere
        /// <summary>
        /// Recycle the SQL Server log
        /// </summary>
        //void StartQueryMonitorTraceCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        //{
        //    SqlCommand cmd =
        //        SqlCommandBuilder.BuildQueryMonitorStartCommand(conn,
        //                                                      ver,
        //                                                      (StartQueryMonitorTraceConfiguration)
        //                                                      serverActionConfig);
        //  sdtCollector = new SqlCollector(cmd, true);
        //  sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerActionCallback));
        //}


        /// <summary>
        /// Recycle the SQL Server log
        /// </summary>
        void RecycleLogCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildRecycleLogCommand(conn, ver);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerActionCallback));
        }

        /// <summary>
        /// Recycle the SQL Agent log
        /// </summary>
        void RecycleAgentLogCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildRecycleAgentLogCommand(conn, ver);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerActionCallback));
        }


        /// <summary>
        /// Reindex the selected table and/or index
        /// </summary>
        void ReindexCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            // Copy config values to snapshot to be returned even in case of error
            ReindexSnapshot reindexSnapshot = (ReindexSnapshot)snapshot;
            ReindexConfiguration config = (ReindexConfiguration)serverActionConfig;
            reindexSnapshot.TableId = config.TableId;
            reindexSnapshot.IndexName = config.IndexName;
            reindexSnapshot.DatabaseName = config.DatabaseName;
            snapshot = reindexSnapshot;

            SqlCommand cmd =
                           SqlCommandBuilder.BuildReindexCommand(conn, ver,(ReindexConfiguration)serverActionConfig, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ReindexCallback));
        }

        /// <summary>
        /// Update statistics on the selected table and/or index
        /// </summary>
        void UpdateStatisticsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildUpdateStatistcisCommand(conn, ver, (UpdateStatisticsConfiguration)serverActionConfig, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerActionCallback));
        }
        /// <summary>
        /// Perform a mirroring action on the mirrored database
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sdtCollector"></param>
        /// <param name="ver"></param>
        void MirroringPartnerActionsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd = SqlCommandBuilder.BuildMirroringPartnerActionCommand(conn, ver, (MirroringPartnerActionConfiguration)serverActionConfig);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerActionCallback));
        }

        /// <summary>
        /// Start/stop SQL Agent Job
        /// </summary>
        void AdhocQueryCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = ((AdhocQueryConfiguration)serverActionConfig).Sql;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;

            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(AdhocQueryCallback));
        }
        /// <summary>
        /// Starts the Configuration collector.
        /// </summary>
        void StartServerActionCollector()
        {
            // Additional Parameters for Dynamic Permissions Collection
            string serverActionConfigGetTypeName = serverActionConfig.GetType().Name;
            var parameters = GetServerActionPermissionsSqlCommand(serverActionConfigGetTypeName);
            var dbName = string.Empty;
            if(parameters.Length > 1 && parameters[1] is SqlCommand)
            {
                dbName = ((SqlCommand) parameters[1]).Connection.Database;
            }

            StartGenericCollector(serverActionCollector, snapshot, "StartServerActionCollector", "Server Action",
                ServerActionCallback, !string.IsNullOrWhiteSpace(dbName) ? dbName : null, parameters);
        }

        /// <summary>
        /// Permissions Command required at runtime based on database / table information
        /// </summary>
        /// <param name="serverActionConfigGetTypeName">Type of Server Action Configuration</param>
        /// <returns>SQL Command which can be executed to gather permissions</returns>
        private object[] GetServerActionPermissionsSqlCommand(string serverActionConfigGetTypeName)
        {
            // SqlCommand
            SqlCommand sqlCommand = null;
            SqlConnection connection = null;
            object[] retVal = null; 
            // Get required parameters for permissions
            switch (serverActionConfigGetTypeName)
            {
                case "UpdateStatisticsConfiguration": // UpdateStatistics.sql
                    var updateStatisticsConfiguration = (UpdateStatisticsConfiguration)serverActionConfig;
                    // SqlConnection for Dynamic Permission Check
                    connection = OpenConnection(updateStatisticsConfiguration.DatabaseName);
                   // Create the command
                    sqlCommand = SqlCommandBuilder.BuildUpdateStatistcisPermissionsCommand(connection,
                        new ServerVersion(connection.ServerVersion), updateStatisticsConfiguration, cloudProviderId);
                    retVal = new object[] {serverActionConfigGetTypeName, sqlCommand, updateStatisticsConfiguration.DatabaseName, updateStatisticsConfiguration.TableId};
                    break;
                case "MirroringPartnerActionConfiguration": // MirroringPartnerAction.sql
                    var mirroringPartnerActionConfiguration = (MirroringPartnerActionConfiguration)serverActionConfig;
                    // SqlConnection for Dynamic Permission Check
                    connection = OpenConnection(mirroringPartnerActionConfiguration.Database);
                    // Create the command
                    sqlCommand = SqlCommandBuilder.BuildMirroringPartnerActionPermissionsCommand(connection,
                        new ServerVersion(connection.ServerVersion), mirroringPartnerActionConfiguration, cloudProviderId);
                    retVal = new object[] {serverActionConfigGetTypeName, sqlCommand, mirroringPartnerActionConfiguration.Database};
                    break;
                case "ReindexConfiguration":
                    var reindexConfiguration = (ReindexConfiguration)serverActionConfig;
                    connection = OpenConnection(reindexConfiguration.DatabaseName);
                    sqlCommand = SqlCommandBuilder.BuildReindexPermissionsCommand(connection,
                        new ServerVersion(connection.ServerVersion),
                        (ReindexConfiguration) serverActionConfig, cloudProviderId);
                    retVal = new object[] {serverActionConfigGetTypeName, sqlCommand, reindexConfiguration.DatabaseName, reindexConfiguration.TableId };
                    break;
                case "FullTextActionConfiguration":
                    var fullTextActionConfiguration = (FullTextActionConfiguration)serverActionConfig;
                    connection = OpenConnection(fullTextActionConfiguration.DatabaseName);
                    sqlCommand = SqlCommandBuilder.BuildDbOwnerPermissionsCommand(connection,
                        new ServerVersion(connection.ServerVersion),
                        (FullTextActionConfiguration)serverActionConfig);
                    retVal = new object[] { serverActionConfigGetTypeName, sqlCommand, fullTextActionConfiguration.DatabaseName};
                    break;
                default:
                    retVal = new object[] {serverActionConfigGetTypeName, sqlCommand};
                    break;
            }
            return retVal;
        }

        /// <summary>
        /// Define the default callback in case the configuration object is not recognized or has no special handling
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void DefaultCallback(CollectorCompleteEventArgs e)
        {
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the ServerAction collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void ServerActionCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(serverActionCallback, snapshot, "ConfigurationCallback", "Server Action",sender, e);
        }

        /// <summary>
        /// Define the Reindex callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ReindexCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretReindex(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Interpret Reindex data
        /// </summary>
        private void InterpretReindex(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretInterpretReindex"))
            {
                try
                {
                    ReindexSnapshot reindexSnapshot = (ReindexSnapshot)snapshot;
                    ReindexConfiguration config = (ReindexConfiguration)serverActionConfig;
                    reindexSnapshot.TableId = config.TableId;
                    reindexSnapshot.IndexName = config.IndexName;
                    reindexSnapshot.DatabaseName = config.DatabaseName;
                    if (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(0)) reindexSnapshot.TableName = dataReader.GetString(0);
                        dataReader.NextResult();
                        if (dataReader.Read())
                        {
                            if (reindexSnapshot.ProductVersion.Major == 8)
                            {
                                if (!dataReader.IsDBNull(15))
                                    reindexSnapshot.Fragmentation = (double)dataReader.GetValue(18);
                            }
                            else
                            {
                                if (!dataReader.IsDBNull(0))
                                    reindexSnapshot.Fragmentation = (double) dataReader.GetValue(0);
                            }

                        }
                    }

                    snapshot = reindexSnapshot;
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot, LOG, "Error interpreting Reindex Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(snapshot);
                }
            }
        }

        /// <summary>
        /// Callback used to process the data returned from the Reindex collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ReindexCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(ReindexCallback), snapshot, "ReindexCallback", "Reindex",
                            sender, e);
        }

        /// <summary>
        /// Callback used to process the data returned from the ServiceControl collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ServiceControlCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(ServiceControlCallback), snapshot, "ServiceControl", "Service Control",
                            sender, e);
        }

        /// <summary>
        /// Callback used to process the data returned from the Reindex collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void AdhocQueryCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(AdhocQueryCallback), snapshot, "AdhocQueryCallback", "AdhocQuery",
                            sender, e);
        }

        /// <summary>
        /// Define the Reindex callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ServiceControlCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                while (rd.Read())
                {
                    if (!rd.IsDBNull(0))
                    {
                        if (rd.GetString(0).ToLower().Trim() == "access is denied.")
                        {
                            snapshot.SetError("Unable to execute service control command.  Access is denied.",
                                              null);
                            FireCompletion(snapshot, Result.Failure);
                            return;
                        }
                    }
                }
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Define the Reindex callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void AdhocQueryCallback(CollectorCompleteEventArgs e)
        {
            AdhocQuerySnapshot adhocsnapshot = (AdhocQuerySnapshot) snapshot;

            DataTable dtatTable;

            if (e.ElapsedMilliseconds == null)
                adhocsnapshot.Duration = null;
            else
                adhocsnapshot.Duration = TimeSpan.FromMilliseconds(e.ElapsedMilliseconds.Value);
           
            using (SqlDataReader rd = (SqlDataReader)e.Value)
            {
                adhocsnapshot.RowsAffected = rd.RecordsAffected;

                if (adhocsnapshot.Configuration.ReturnData)
                {
                    adhocsnapshot.DataSet = new DataSet("ServerActionProbe");
                    adhocsnapshot.DataSet.RemotingFormat = SerializationFormat.Binary;
                    if (rd.HasRows)
                    {
                        int tableId = 1;
                        do
                        {
                            DataTable table = ProbeHelpers.GetTable(rd, false, true);
                            table.TableName = String.Format("Table-{0}", tableId++);
                            adhocsnapshot.DataSet.Tables.Add(table);
                        } while (rd.NextResult());
                    }
                }
                else
                {
                    int rscount = rd.HasRows ? 1 : 0;
                    while (rd.NextResult())
                        rscount++;
                    adhocsnapshot.RowSetCount = rscount;
                }
            }

            FireCompletion(snapshot, Result.Success);
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
