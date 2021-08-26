//------------------------------------------------------------------------------
// <copyright file="SqlBaseProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Threading;
using Idera.SQLdm.CollectionService.Configuration;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;

namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    using System;
    using System.Data.SqlClient;
    using Idera.SQLdm.CollectionService.Probes.Collectors;
    using Idera.SQLdm.Common.Configuration;
    using BBS.TracerX;
    using Idera.SQLdm.Common.Snapshots;

    /// <summary>
    /// Enter a description for this class
    /// </summary>
    abstract class SqlBaseProbe : BaseProbe
    {
        #region fields

        protected SqlConnectionInfo connectionInfo;
        protected Logger LOG = Logger.GetLogger("SqlBaseProbe");
        //*private static int? cloudProviderId = null;//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added a field for determining the cloud provider of the monitored instance
        protected int? cloudProviderId = null;//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added another member for determining the cloud provider of the monitored instance
        protected const int CLOUD_PROVIDER_ID_AZURE = 2;//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
      

        #endregion

        #region constructors

        protected SqlBaseProbe(SqlConnectionInfo connectionInfo) : base()
        {
            if (connectionInfo.ApplicationName != Constants.CollectionServceConnectionStringApplicationName || connectionInfo.ApplicationName != Constants.CustomCounterConnectionStringApplicationName)
            {
                connectionInfo.ApplicationName = Constants.CollectionServceConnectionStringApplicationName;
            }
            this.connectionInfo = connectionInfo;
        }

        #endregion


        #region delegates

        /// <summary>
        /// Define the generic collector delegate
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        internal delegate void Collector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver);

        //sqldm-30013 start
        /// <summary>
        /// Define the generic collector delegate for azure
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        /// <param name="dbname">Name of azure db for which collector is called</param>
        /// <summary>
        internal delegate void CollectorDatabase(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver,String dbname);
        //sqldm-30013 end
        /// <summary>
        /// Delegate to start next collector
        /// </summary>
        internal delegate void NextCollector();

        /// <summary>
        /// Define the generic collector callback delegate
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        internal delegate void CollectorCallback(CollectorCompleteEventArgs e);

        /// <summary>
        /// Delegate for callback behavior in case of error
        /// </summary>
        /// <param name="snapshot">Snapshot to be passed to method</param>
        internal delegate void FailureDelegate(Snapshot snapshot, Exception e);


        /// <summary>
        /// Generic failure delegate
        /// Unless overridden this fires completion with a failure result
        /// </summary>
        /// <param name="snapshot">Snapshot to return with failure</param>
        protected void GenericFailureDelegate(Snapshot snapshot, Exception e)
        {
            // Pass down Probe Errors in case of Failures
            Type snapshotType = snapshot.GetType();
            if (snapshotType == typeof(ScheduledRefresh))
            {
                ((ScheduledRefresh)snapshot).Server.SqlServiceStatus = ServiceState.UnableToMonitor;
            }

            FireCompletion(snapshot, Idera.SQLdm.Common.Services.Result.Failure,
                snapshot.ProbeError != null ? snapshot.ProbeError.Name : (e != null ? e.Message : null),
                snapshot.ProbeError);
        }

        /// <summary>
        /// Generic failure delegate
        /// Unless overridden this fires completion with a failure result
        /// </summary>
        /// <param name="snapshot">Snapshot to return with failure</param>
        protected void GenericFailureDelegate(Snapshot snapshot)
        {
            Type snapshotType = snapshot.GetType();
            if (snapshotType == typeof(ScheduledRefresh))
            {
                ((ScheduledRefresh)snapshot).Server.SqlServiceStatus = ServiceState.UnableToMonitor;
            }

            // Pass down Probe Errors in case of Failures
            FireCompletion(snapshot, Idera.SQLdm.Common.Services.Result.Failure,
                snapshot.ProbeError != null ? snapshot.ProbeError.Name : null, snapshot.ProbeError);
        }

        /// <summary>
        /// To fire Permission Violation Completion
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="probeError"></param>
        protected void GenericPermissionFailureDelegate(Snapshot snapshot, ProbePermissionHelpers.ProbeError probeError)
        {
            Type snapshotType = snapshot.GetType();
            if (snapshotType == typeof(ScheduledRefresh))
            {
                // Add ProbeError to Probe Permission Errors in Alerts
                var probePermissionErrors = ((ScheduledRefresh)snapshot).Alerts.ProbePermissionErrors;
                if(probePermissionErrors != null && !probePermissionErrors.Contains(probeError))
                {
                    probePermissionErrors.Add(probeError);
                }
            }
        }

        /// <summary>
        /// Delegate for handling SQL errors during connection.  For Scheduled Refresh.
        /// </summary>
        internal delegate void HandleSqlErrorDelegate(
            Snapshot snapshot, string collectorName, SqlException sqlException, SqlCollector collector);

        #endregion

        #region properties
        
        /// <summary>
        /// Minimum Permissions set on <seealso cref="StartGenericCollector(Idera.SQLdm.CollectionService.Probes.Sql.SqlBaseProbe.Collector,Idera.SQLdm.Common.Snapshots.Snapshot,string,string,System.EventHandler{Idera.SQLdm.CollectionService.Probes.Collectors.CollectorCompleteEventArgs},object[])"/>
        /// </summary>
        internal MinimumPermissions MinimumPermissions { get; set; }

        /// <summary>
        /// MetadataPermissions Permissions set on <seealso cref="StartGenericCollector(Idera.SQLdm.CollectionService.Probes.Sql.SqlBaseProbe.Collector,Idera.SQLdm.Common.Snapshots.Snapshot,string,string,System.EventHandler{Idera.SQLdm.CollectionService.Probes.Collectors.CollectorCompleteEventArgs},object[])"/>
        /// </summary>
        internal MetadataPermissions MetadataPermissions { get; set; }

        /// <summary>
        /// CollectionPermissions Permissions set on <seealso cref="StartGenericCollector(Idera.SQLdm.CollectionService.Probes.Sql.SqlBaseProbe.Collector,Idera.SQLdm.Common.Snapshots.Snapshot,string,string,System.EventHandler{Idera.SQLdm.CollectionService.Probes.Collectors.CollectorCompleteEventArgs},object[])"/>
        /// </summary>
        internal CollectionPermissions CollectionPermissions { get; set; }
        #endregion

        #region events

        #endregion

        #region methods

        protected SqlConnection OpenConnection(String dbname=null)//sqldm-30013 adding dbname default parameter for azure
        {
            SqlConnection conn = null;
            try
            {
                if (dbname == null)
                    conn = connectionInfo.GetConnection();
                else
                    conn = connectionInfo.GetConnectionDatabase(dbname);
                //sqldm-20369 start
                conn.ConnectionString += ";ApplicationIntent=READONLY";
                //sqldm-20369 end
                conn.Open();
            }
            catch (Exception ex)
            {
                LOG.Warn("Open Connection Failed with readonly: {0}",ex);
                try
                {
                    if (dbname == null)
                        conn = connectionInfo.GetConnection();
                    else
                        conn = connectionInfo.GetConnectionDatabase(dbname);
                    //sqldm-20369 start
                    //conn.ConnectionString += ";ApplicationIntent=READONLY";
                    //sqldm-20369 end
                    conn.Open();
                }
                catch (Exception ex2)
                {
                    LOG.Warn("Open Connection Failed without readonly: {0}", ex2);
                    SqlConnection.ClearPool(conn);
                    conn.Dispose();
                    throw;
                }
            }
            return conn;
        }

        //SQLDM-30013 Start

        /// <summary>
        /// Starts a SQL collector for azure
        /// </summary>
        /// <param name="collector">Delegate containing logic for individual collector construction</param>
        /// <param name="snapshot">Snapshot to which log data will be copied</param>
        /// <param name="logger">Name of logger for this collector</param>
        /// <param name="collectorName">Friendly name of collector for log messages</param>
        /// <param name="permissionViolationCallback">Callback for Permission Violation</param>
        /// <param name="permissionCheckArgs">Additional Arguments for Probe Permission Checks</param>
        /// <param name="dbname">Name of azure db for which collector is called</param>
        protected void StartGenericCollectorDatabase(CollectorDatabase collector, Snapshot snapshot, string logger, string collectorName, EventHandler<CollectorCompleteEventArgs> permissionViolationCallback, String dbname, params object[] permissionCheckArgs)
        {
            StartGenericCollectorDatabase(collector, snapshot, logger, collectorName, null, permissionViolationCallback, dbname, permissionCheckArgs);
        }

        /// <summary>
        /// Starts a SQL collector for azure
        /// </summary>
        /// <param name="collector">Delegate containing logic for individual collector construction</param>
        /// <param name="snapshot">Snapshot to which log data will be copied</param>
        /// <param name="logger">Name of logger for this collector</param>
        /// <param name="collectorName">Friendly name of collector for log messages</param>
        /// <param name="serverVersion">If the server version has been previously collected it may be referenced here.</param>
        /// <param name="permissionViolationCallback">Callback for Permission Violation</param>
        /// <param name="permissionCheckArgs">Additional Arguments for Probe Permission Checks</param>
        /// <param name="dbname">Name of azure db for which collector is called</param>
        protected void StartGenericCollectorDatabase(CollectorDatabase collector, Snapshot snapshot, string logger, string collectorName, ServerVersion serverVersion, EventHandler<CollectorCompleteEventArgs> permissionViolationCallback, String dbname, params object[] permissionCheckArgs)
        {
            HandleSqlErrorDelegate sqlError = new HandleSqlErrorDelegate(HandleSqlException);
            StartGenericCollectorMain(collector, snapshot, logger, collectorName, serverVersion, sqlError, permissionViolationCallback, dbname, permissionCheckArgs);
        }
        private static void ExecuteCollector(dynamic collector, ServerVersion serverVersion, string databaseName, SqlConnection conn, SqlCollector sdtCollector)
        {
            if (collector is Collector)
            {
                // Execute collector
                collector(conn, sdtCollector, serverVersion);
            }
            else if (collector is CollectorDatabase)
            {
                // Execute database collector
                collector(conn, sdtCollector, serverVersion, databaseName);
            }
        }
        /// <summary>
        /// Starts a SQL collector
        /// </summary>
        /// <param name="collector">Delegate containing logic for individual collector construction</param>
        /// <param name="snapshot">Snapshot to which log data will be copied</param>
        /// <param name="logger">Name of logger for this collector</param>
        /// <param name="collectorName">Friendly name of collector for log messages</param>
        /// <param name="serverVersion">If the server version has been previously collected it may be referenced here.</param>
        /// <param name="permissionViolationCallback">Callback for Permission Violation</param>
        /// <param name="permissionCheckArgs">Additional Arguments for Probe Permission Checks</param>
        protected void StartGenericCollector(Collector collector, Snapshot snapshot, string logger, string collectorName, ServerVersion serverVersion, HandleSqlErrorDelegate sqlError, EventHandler<CollectorCompleteEventArgs> permissionViolationCallback, params object[] permissionCheckArgs)
        {
            StartGenericCollectorMain(collector, snapshot, logger, collectorName, serverVersion, sqlError, permissionViolationCallback, null, permissionCheckArgs);
        }
        //sqldm-30013 end
        /// <summary>
        /// Starts a SQL collector
        /// </summary>
        /// <param name="collector">Delegate containing logic for individual collector construction</param>
        /// <param name="snapshot">Snapshot to which log data will be copied</param>
        /// <param name="logger">Name of logger for this collector</param>
        /// <param name="collectorName">Friendly name of collector for log messages</param>
        /// <param name="permissionViolationCallback">Callback for Permission Violation</param>
        /// <param name="permissionCheckArgs">Additional Arguments for Probe Permission Checks</param>
        protected void StartGenericCollector(Collector collector, Snapshot snapshot, string logger, string collectorName, EventHandler<CollectorCompleteEventArgs> permissionViolationCallback, params object[] permissionCheckArgs)
        {
            StartGenericCollector(collector, snapshot, logger, collectorName, null, permissionViolationCallback, permissionCheckArgs);
        }

        protected void StartGenericCollector(Collector collector, Snapshot snapshot, string logger, string collectorName, EventHandler<CollectorCompleteEventArgs> permissionViolationCallback, string dbName, params object[] permissionCheckArgs)
        {
            HandleSqlErrorDelegate sqlError = new HandleSqlErrorDelegate(HandleSqlException);
            StartGenericCollectorMain(collector, snapshot, logger, collectorName, null, sqlError, permissionViolationCallback, null, permissionCheckArgs);
        }

        /// <summary>
        /// Starts a SQL collector
        /// </summary>
        /// <param name="collector">Delegate containing logic for individual collector construction</param>
        /// <param name="snapshot">Snapshot to which log data will be copied</param>
        /// <param name="logger">Name of logger for this collector</param>
        /// <param name="collectorName">Friendly name of collector for log messages</param>
        /// <param name="serverVersion">If the server version has been previously collected it may be referenced here.</param>
        /// <param name="permissionViolationCallback">Callback for Permission Violation</param>
        /// <param name="permissionCheckArgs">Additional Arguments for Probe Permission Checks</param>
        protected void StartGenericCollector(Collector collector, Snapshot snapshot, string logger, string collectorName, ServerVersion serverVersion, EventHandler<CollectorCompleteEventArgs> permissionViolationCallback, params object[] permissionCheckArgs)
        {
            HandleSqlErrorDelegate sqlError = new HandleSqlErrorDelegate(HandleSqlException);
            StartGenericCollectorMain(collector, snapshot, logger, collectorName, serverVersion, sqlError, permissionViolationCallback, null , permissionCheckArgs);
        }

   
        

        /// <summary>
        /// Starts a SQL collector
        /// </summary>
        /// <param name="collector">Delegate containing logic for individual collector construction</param>
        /// <param name="snapshot">Snapshot to which log data will be copied</param>
        /// <param name="logger">Name of logger for this collector</param>
        /// <param name="collectorName">Friendly name of collector for log messages</param>
        /// <param name="serverVersion">If the server version has been previously collected it may be referenced here.</param>
        /// <param name="sqlError"></param>
        /// <param name="permissionViolationCallback">Callback for Permission Violation</param>
        /// <param name="permissionCheckArgs">Additional Arguments for Probe Permission Checks</param>
        /// <param name="dbname">Name of database for which collector is called</param>
        protected void StartGenericCollectorMain(dynamic collector, Snapshot snapshot, string logger, string collectorName, ServerVersion serverVersion, HandleSqlErrorDelegate sqlError, EventHandler<CollectorCompleteEventArgs> permissionViolationCallback,String dbname, params object[] permissionCheckArgs)
        {
            if (snapshot == null) throw new ArgumentNullException(collectorName + " snapshot is null");
            if (collector == null) throw new ArgumentNullException(collectorName + " collector is null");
            SqlConnection conn = null;
            SqlCollector sdtCollector = null;
            using (LOG.DebugCall(logger))
            {
                try
                {
                    
                    conn = OpenConnection(dbname);

                    if (serverVersion == null)
                        serverVersion = new ServerVersion(conn.ServerVersion);
                    snapshot.ProductVersion = serverVersion;
                    LOG.Debug("Monitored server " + snapshot.ServerName +
                             " is running SQL Server version: " + snapshot.ProductVersion);
                    //Check the server version
                    // Add SQL Server 2008
                    if ((serverVersion.IsSupported))
                    {
                        if (snapshot.ProductVersion.MasterDatabaseCompatibility == 0 || snapshot.ProductVersion.MasterDatabaseCompatibility == null)
                        {
                            try
                            {
                                using (SqlCommand masterCmd = conn.CreateCommand())
                                {
                                    if (cloudProviderId != CLOUD_PROVIDER_ID_AZURE)//SQLdm 10.0 (Tarun Sapra)- Choose the correct batch constant acc to cloud provider
                                    {
                                        masterCmd.CommandText =
                                            Idera.SQLdm.CollectionService.Probes.Sql.Batches.BatchConstants.GetMasterCompatibility;
                                    }
                                    else
                                    {
                                        if (dbname != null)
                                        {
                                            masterCmd.CommandText =
                                               String.Format(Idera.SQLdm.CollectionService.Probes.Sql.Batches.BatchConstants.GetMasterCompatibility_AZURE, dbname);
                                        }
                                        else
                                            masterCmd.CommandText = Idera.SQLdm.CollectionService.Probes.Sql.Batches.BatchConstants.GetMasterCompatibility_AZURE_MASTER;
                                    }                                
                                    snapshot.ProductVersion.MasterDatabaseCompatibility = Int32.Parse((string)masterCmd.ExecuteScalar());
                                }
                            }
                            //Retry in case of transient error
                            catch (SqlException)
                            {
                                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                                //SQlDM-28022 - Handling connection object to avoid leakage
                                if (conn == null || conn.State != System.Data.ConnectionState.Open)
                                {
                                    conn = OpenConnection(dbname);
                                }
                                
                                using (SqlCommand masterCmd = conn.CreateCommand())
                                {
                                    //START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Choosing aother batch constant, if the cloud provider is of azure type
                                    if (cloudProviderId != CLOUD_PROVIDER_ID_AZURE)
                                    {
                                        masterCmd.CommandText = Idera.SQLdm.CollectionService.Probes.Sql.Batches.BatchConstants.GetMasterCompatibility;
                                    }
                                    else
                                    {
                                        if(dbname!=null)
                                        masterCmd.CommandText = String.Format(Idera.SQLdm.CollectionService.Probes.Sql.Batches.BatchConstants.GetMasterCompatibility_AZURE,dbname);
                                        else
                                            masterCmd.CommandText = Idera.SQLdm.CollectionService.Probes.Sql.Batches.BatchConstants.GetMasterCompatibility_AZURE_MASTER;
                                    }
                                    //END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Choosing aother batch constant, if the cloud provider is of azure type
                                   snapshot.ProductVersion.MasterDatabaseCompatibility = Int32.Parse((string)masterCmd.ExecuteScalar());
                                }
                            }
                        }

                        if (snapshot.ProductVersion.Major > 8 && snapshot.ProductVersion.MasterDatabaseCompatibility < 90)
                        {
                            LOG.Warn("Monitored server " + snapshot.ServerName + " has master compatibility mode " +
                                     snapshot.ProductVersion.MasterDatabaseCompatibility);
                        }

                        LOG.Verbose("Monitored server " + snapshot.ServerName +
                                    " is running a supported SQL Server version.");

                        if (snapshot.ProductEdition == null)
                        {
                            try
                            {
                                using (SqlCommand timeStampCmd = conn.CreateCommand())
                                {
                                    timeStampCmd.CommandText =
                                        Idera.SQLdm.CollectionService.Probes.Sql.Batches.BatchConstants.GetEdition;
                                    snapshot.ProductEdition = (string)timeStampCmd.ExecuteScalar();
                                }
                            }
                            //Retry in case of transient error
                            catch (SqlException)
                            {
                                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                                //SQlDM-28022 - Handling connection object to avoid leakage
                                if (conn == null || conn.State != System.Data.ConnectionState.Open)
                                {
                                    conn = OpenConnection(dbname);
                                }
                                
                                using (SqlCommand timeStampCmd = conn.CreateCommand())
                                {
                                    timeStampCmd.CommandText =
                                        Idera.SQLdm.CollectionService.Probes.Sql.Batches.BatchConstants.GetEdition;
                                    snapshot.ProductEdition = (string)timeStampCmd.ExecuteScalar();
                                }
                            }
                        }

                        LOG.Debug("Monitored server " + snapshot.ServerName +
                             " is running SQL Server edition: " + snapshot.ProductEdition);

                        // Check edition
                        //SQLdm 8.6 (ankit srivastava) -- removing Express check
                        //if (snapshot.ProductEdition.ToLower().Trim() != "express edition")
                        //{
                            LOG.Verbose("Monitored server " + snapshot.ServerName +
                                        " is running a supported SQL Server edition.");

                        //Sqldm 10.3 -- (Manali H) - 28410 fix : Added IsFullTextInstalled
                        if (!snapshot.IsFullTextInstalled)
                        {
                            try
                            {
                                using (SqlCommand timeStampCmd = conn.CreateCommand())
                                {
                                    timeStampCmd.CommandText =
                                        Idera.SQLdm.CollectionService.Probes.Sql.Batches.BatchConstants.FullTextFeatureQuery;
                                    snapshot.IsFullTextInstalled = (Int32)timeStampCmd.ExecuteScalar() == 1;
                                }
                            }
                            //Retry in case of transient error
                            catch (SqlException)
                            {
                                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                                //SQlDM-28022 - Handling connection object to avoid leakage
                                if (conn == null || conn.State != System.Data.ConnectionState.Open)
                                {
                                    conn = OpenConnection(dbname);
                                }

                                using (SqlCommand timeStampCmd = conn.CreateCommand())
                                {
                                    timeStampCmd.CommandText =
                                        Idera.SQLdm.CollectionService.Probes.Sql.Batches.BatchConstants.FullTextFeatureQuery;
                                    snapshot.IsFullTextInstalled = (Int32)timeStampCmd.ExecuteScalar() == 1;
                                }
                            }
                        }

                        LOG.Debug("Monitored server " + snapshot.ServerName +
                             (snapshot.IsFullTextInstalled ? "has Full text feature installed." : "does not have Full text feature installed."));
                        //Sqldm 10.3 -- (Manali H) - 28410 fix : Added IsFullTextInstalled

                        int collectionServiceConnectionCount = 0;
                            int maxCollectionServiceConnectionCount =
                                CollectionServiceConfiguration.GetCollectionServiceElement().MaxConcurrentServiceConnections;
                            if ((snapshot.TimeStamp == null) || (snapshot.TimeStampLocal == null) || (snapshot.ServerStartupTime == null))
                            {
                                try
                                {
                                using (SqlCommand timeStampCmd = conn.CreateCommand())
                                {
                                    //START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added two batch constansts in case the cloud provider id of monitored instance is 'Azure'
                                    timeStampCmd.CommandText = cloudProviderId == CLOUD_PROVIDER_ID_AZURE ?
                                                                   (snapshot.ProductVersion.Major == 8
                                                                   ? (dbname != null ? String.Format(Batches.BatchConstants.GetTimeAndConnectionCount2000_AZURE, dbname) : Batches.BatchConstants.GetTimeAndConnectionCount2000_AZURE_MASTER)
                                                                   : (dbname != null ? String.Format(Batches.BatchConstants.GetTimeAndConnectionCount2005_AZURE, dbname) : Batches.BatchConstants.GetTimeAndConnectionCount2005_AZURE_MASTER))
                                                                       :
                                                                       (snapshot.ProductVersion.Major == 8
                                                                       ? Batches.BatchConstants.GetTimeAndConnectionCount2000
                                                                       : Batches.BatchConstants.GetTimeAndConnectionCount2005);
                                        //END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added two batch constansts in case the cloud provider id of monitored instance is 'Azure'

                                        SqlDataReader reader =   timeStampCmd.ExecuteReader();
                                        reader.Read();
                                        if (!reader.IsDBNull(0)) snapshot.TimeStamp = reader.GetDateTime(0);
                                        if (!reader.IsDBNull(1)) snapshot.TimeStampLocal = reader.GetDateTime(1);
                                        if (!reader.IsDBNull(2)) snapshot.ServerStartupTime = reader.GetDateTime(2);
                                        if (!reader.IsDBNull(3)) collectionServiceConnectionCount = reader.GetInt32(3);
                                        reader.Close();
                                    }
                                }
                                //Retry in case of transient error
                                catch (SqlException)
                                {
                                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                                    //SQlDM-28022 - Handling connection object to avoid leakage
                                    if (conn == null || conn.State != System.Data.ConnectionState.Open)
                                    {
                                        conn = OpenConnection(dbname);
                                    }
                                
                                    using (SqlCommand timeStampCmd = conn.CreateCommand())
                                    {
                                        //START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added two batch constansts in case the cloud provider id of monitored instance is 'Azure'
                                        timeStampCmd.CommandText = cloudProviderId == CLOUD_PROVIDER_ID_AZURE ?
                                                                       (snapshot.ProductVersion.Major == 8
                                                                       ? (dbname != null ? String.Format(Batches.BatchConstants.GetTimeAndConnectionCount2000_AZURE, dbname) : Batches.BatchConstants.GetTimeAndConnectionCount2000_AZURE_MASTER)
                                                                       : (dbname != null ? String.Format(Batches.BatchConstants.GetTimeAndConnectionCount2005_AZURE, dbname) : Batches.BatchConstants.GetTimeAndConnectionCount2005_AZURE_MASTER))
                                                                       :
                                                                       (snapshot.ProductVersion.Major == 8
                                                                       ? Batches.BatchConstants.GetTimeAndConnectionCount2000
                                                                       : Batches.BatchConstants.GetTimeAndConnectionCount2005);
                                        //END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added two batch constansts in case the cloud provider id of monitored instance is 'Azure'

                                        SqlDataReader reader = timeStampCmd.ExecuteReader();
                                        reader.Read();
                                        if (!reader.IsDBNull(0)) snapshot.TimeStamp = reader.GetDateTime(0);
                                        if (!reader.IsDBNull(1)) snapshot.TimeStampLocal = reader.GetDateTime(1);
                                        if (!reader.IsDBNull(2)) snapshot.ServerStartupTime = reader.GetDateTime(2);
                                        if (!reader.IsDBNull(3)) collectionServiceConnectionCount = reader.GetInt32(3);
                                        reader.Close();
                                    }
                                }
                            }

                            LOG.Verbose("Monitored server " + snapshot.ServerName +
                                        " started up at : " + snapshot.ServerStartupTime);

                            if (collectionServiceConnectionCount <=  maxCollectionServiceConnectionCount)

                            {
                                LOG.Verbose("Recorded " + collectionServiceConnectionCount +
                                " connections from the service to " + snapshot.ServerName);

                            // Skip Permissions Collector for Cloud Providers
                            // Read Collection Permissions /Minimum Permissions / Metadata Permissions

                            //if (cloudProviderId != Constants.MicrosoftAzureId &&
                             if((cloudProviderId != Constants.MicrosoftAzureId  || (cloudProviderId == Constants.MicrosoftAzureId && conn.Database != "master")) && cloudProviderId != Constants.AmazonRDSId &&
                                     (serverVersion != null && serverVersion.Major >= 10) &&
                                    (snapshot.MinimumPermissions == MinimumPermissions.None ||
                                     snapshot.CollectionPermissions == CollectionPermissions.None ||
                                     snapshot.MetadataPermissions == MetadataPermissions.None))
                                {
                                    var skipPermissions = false;
                                    if (cloudProviderId == Constants.MicrosoftAzureId)
                                    {
                                        var cachedPermissions =
                                            ProbePermissionHelpers.GetCachedPermissions(conn.DataSource,
                                                conn.Database);
                                        if (cachedPermissions != null)
                                        {
                                            snapshot.MinimumPermissions = cachedPermissions.MinimumPermissions;
                                            snapshot.CollectionPermissions = cachedPermissions.CollectionPermissions;
                                            snapshot.MetadataPermissions = cachedPermissions.MetadataPermissions;
                                        }

                                        skipPermissions =
                                            cachedPermissions != null && (cachedPermissions.UpdatedOn - DateTime.UtcNow)
                                            .TotalSeconds <= ProbePermissionHelpers.CachedPermissionsRefreshTimeout &&
                                            (snapshot.MinimumPermissions != MinimumPermissions.None &&
                                             snapshot.CollectionPermissions != CollectionPermissions.None);
                                    }
                                    try
                                    {
                                        if (!skipPermissions)
                                        {
                                            if (cloudProviderId == Constants.MicrosoftAzureId)
                                            {
                                                using (SqlCommand permissionsCommand =
                                                    SqlCommandBuilder.BuildMasterPermissionsCommand(
                                                        connectionInfo.GetConnectionDatabase("master"), connectionInfo,
                                                        cloudProviderId))
                                                {
                                                    // To Ensure Closing of DataReader
                                                    using (SqlDataReader permissionsReader =
                                                        permissionsCommand.ExecuteReader())
                                                    {
                                                        // Read Required Permissions
                                                        snapshot.CollectionPermissions =
                                                            ServerOverviewInterpreter
                                                                .ReadPermissionsToEnum<CollectionPermissions>(
                                                                    permissionsReader, LOG, snapshot);
                                                    }
                                                }
                                            }

                                            using (SqlCommand permissionsCommand =
                                                SqlCommandBuilder.BuildPermissionsCommand(conn, connectionInfo,
                                                    cloudProviderId))
                                            {
                                                // To Ensure Closing of DataReader
                                                using (SqlDataReader permissionsReader =
                                                    permissionsCommand.ExecuteReader())
                                                {
                                                    // Read Required Permissions
                                                    snapshot.MinimumPermissions =
                                                        ServerOverviewInterpreter
                                                            .ReadPermissionsToEnum<MinimumPermissions>(
                                                                permissionsReader, LOG, snapshot);
                                                    snapshot.MetadataPermissions =
                                                        ServerOverviewInterpreter
                                                            .ReadPermissionsToEnum<MetadataPermissions>(
                                                                permissionsReader, LOG, snapshot);
                                                    if (cloudProviderId == Constants.MicrosoftAzureId)
                                                    {
                                                        snapshot.CollectionPermissions =
                                                            ServerOverviewInterpreter
                                                                .ReadPermissionsToEnum<CollectionPermissions>(
                                                                    permissionsReader, LOG, snapshot);
                                                    }
                                                    else
                                                    {
                                                        snapshot.CollectionPermissions |=
                                                            ServerOverviewInterpreter
                                                                .ReadPermissionsToEnum<CollectionPermissions>(
                                                                    permissionsReader, LOG, snapshot);
                                                    }

                                                    // Read Replication Permissions
                                                    snapshot.CollectionPermissions |=
                                                        ServerOverviewInterpreter
                                                            .ReadPermissionsToEnum<CollectionPermissions>(
                                                                permissionsReader, LOG, snapshot);
                                                }
                                            }

                                            if (cloudProviderId == CLOUD_PROVIDER_ID_AZURE)
                                            {
                                                ProbePermissionHelpers.SetCachedPermissions(new ProbePermissionHelpers.CachedPermissions
                                                {
                                                    InstanceName = conn.DataSource,
                                                    DatabaseName = conn.Database,
                                                    MinimumPermissions = snapshot.MinimumPermissions,
                                                    CollectionPermissions = snapshot.CollectionPermissions,
                                                    MetadataPermissions = snapshot.MetadataPermissions,
                                                    UpdatedOn = DateTime.UtcNow
                                                });
                                            }
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        LOG.Error(string.Format(
                                            "Exception occured while reading permisssions for Monitored server {0} with SQL Server Version {1}. Exception : {2}",
                                            snapshot.ServerName, snapshot.ProductEdition, exception));
                                    }
                                }
                                // Update Permissions
                                MinimumPermissions = snapshot.MinimumPermissions;
                                MetadataPermissions = snapshot.MetadataPermissions;
                                CollectionPermissions = snapshot.CollectionPermissions;

                                LOG.Debug("Monitored server " + snapshot.ServerName +
                                          " is running SQL Server edition: " + snapshot.ProductEdition);

                            // Append Probe Error in the PermissionCheck Arguments if not already added
                            permissionCheckArgs = ProbePermissionHelpers.AppendProbeErrorInPermissionCheckArgs(permissionCheckArgs);

                            // Call Collectors directly without Permission Check for Cloud Providers
                            // Check Collection and Metadata Permissions Required for the collector are met
                            if ((cloudProviderId == Constants.MicrosoftAzureId && conn.Database == "master") || cloudProviderId == Constants.AmazonRDSId ||ProbePermissionHelpers.ValidateProbePermissions(snapshot, collectorName, cloudProviderId, permissionCheckArgs))
                            {
                                ExecuteCollector(collector, serverVersion, dbname, conn, sdtCollector);
                            }
                            else
                            {
                                LOG.Verbose(string.Format(
                                    "The user account used by the collection service does not have rights required on the monitored server {0} for {1}\r\n",
                                    snapshot.ServerName, collectorName));

                                // Get Probe Error
                                var probeError =
                                (permissionCheckArgs != null && permissionCheckArgs.Length > 0 &&
                                 permissionCheckArgs[permissionCheckArgs.Length - 1] is ProbePermissionHelpers
                                     .ProbeError)
                                    ? (ProbePermissionHelpers.ProbeError) permissionCheckArgs[
                                        permissionCheckArgs.Length - 1]
                                    : new ProbePermissionHelpers.ProbeError() {Name = collectorName};

                                if (permissionViolationCallback != null)
                                {
                                    // Update Unable to monitor status
                                    GenericPermissionFailureDelegate(snapshot, probeError);

                                    var args = new CollectorCompleteEventArgs(new object[] {collector, probeError},
                                        Result.PermissionViolation);
                                    args.Database = conn.Database;
                                    // Fire Permission Failure Callback and create and pass the probe error argument to handle and store errors
                                    QueueCallback(snapshot, null, permissionViolationCallback,
                                        args);
                                }
                                else
                                {
                                    // Update Unable to monitor status
                                    GenericPermissionFailureDelegate(snapshot, probeError);

                                    // If no callback provided it will complete the probe
                                    // Generally for few OnDemand Probes with no callbacks
                                    FireCompletion(snapshot, Result.PermissionViolation, collectorName, probeError);
                                }
                            }
                        }
                        else
                        {
							try
                            {
                                using (SqlCommand endConnCmd = conn.CreateCommand())
                                {
                                    endConnCmd.CommandText = Batches.BatchConstants.EndIdleConnections_AZURE;
                                    SqlDataReader reader = endConnCmd.ExecuteReader();
                                }
                            }
                            catch(SqlException e)
                            {
								ProbeHelpers.LogAndAttachToSnapshot(snapshot, LOG,
									"The SQLdm Collection Service has too many outstanding connections to " +
									snapshot.ServerName +
									".  The current connection count is " + collectionServiceConnectionCount +
									".  This may be a sign of a problem with the monitored server, with one of the SQLdm collectors, or with a user defined counter" +
									".  To prevent exacerbating this problem collection is being halted until the connection count falls to " +
									maxCollectionServiceConnectionCount + " or fewer."
									, false);

								conn.Close();

								Thread errorThread = new Thread(FatalErrorThread);
								errorThread.Name = "FatalErrorThread";
								errorThread.Start(snapshot);
							}
                        }
                        //SQLdm 8.6 (ankit srivastava) -- removing Express check
                        //}
                        //else
                        //{
                        //    ProbeHelpers.LogAndAttachToSnapshot(snapshot, LOG,
                        //                                  "Monitored server " + snapshot.ServerName +
                        //                                  " is running an unsupported SQL Server edition.  The reported version is " +
                        //                                  serverVersion.Version + " " + snapshot.ProductEdition
                        //                                  , false);

                        //    conn.Close();

                        //    Thread errorThread = new Thread(UnsupportedErrorThread);
                        //    errorThread.Name = "UnsupportedErrorThread";
                        //    errorThread.Start(snapshot);
                        //}
                    }
                    else
                    {
                        ProbeHelpers.LogAndAttachToSnapshot(snapshot, LOG,
                                                         "Monitored server " + snapshot.ServerName +
                                                         " is running an unsupported SQL Server version.  The reported version is " +
                                                         serverVersion.Version
                                                         , false);


                        conn.Close();

                        Thread errorThread = new Thread(UnsupportedErrorThread);
                        errorThread.Name = "UnsupportedErrorThread";
                        errorThread.Start(snapshot);
                    }
                }
                catch (SqlException sqlException)
                {
                    if (cloudProviderId == Constants.MicrosoftAzureId && (sqlException.Number == 916 || sqlException.Number == 10054) && permissionViolationCallback != null)
                    {
                        // Update Unable to monitor status
                        var probeError =
                                (permissionCheckArgs != null && permissionCheckArgs.Length > 0 &&
                                 permissionCheckArgs[permissionCheckArgs.Length - 1] is ProbePermissionHelpers
                                     .ProbeError)
                                    ? (ProbePermissionHelpers.ProbeError)permissionCheckArgs[
                                        permissionCheckArgs.Length - 1]
                                    : new ProbePermissionHelpers.ProbeError() { Name = collectorName };
                        GenericPermissionFailureDelegate(snapshot, probeError);

                        var args1 = new CollectorCompleteEventArgs(new object[] { collector, probeError },
                            Result.PermissionViolation);
                        args1.Database = dbname;
                        // Fire Permission Failure Callback and create and pass the probe error argument to handle and store errors
                        if (conn != null && conn.State != System.Data.ConnectionState.Closed)
                        {
                            SqlConnection.ClearPool(conn);
                            conn.Dispose();
                        }
                        QueueCallback(snapshot, null, permissionViolationCallback,
                            args1);
                    }
                    else
                    {
                        object[] args = new object[] { snapshot, collectorName, sqlError, sqlException, sdtCollector };
                        Thread sqlErrorThread = new Thread(SqlErrorThread);
                        sqlErrorThread.Name = "SqlErrorThread";
                        sqlErrorThread.Start(args);

                        if (conn != null && conn.State != System.Data.ConnectionState.Closed)
                        {
                            SqlConnection.ClearPool(conn);
                            conn.Dispose();
                        }
                    }
                }
                catch (Exception exception)
                {
                    //SQlDM-28022 - Handling connection object to avoid leakage
                    if (conn != null && conn.State != System.Data.ConnectionState.Closed)
                    {
                        SqlConnection.ClearPool(conn);
                        conn.Dispose();
                    }

                    ProbeHelpers.LogAndAttachToSnapshot(snapshot, LOG,
                                                        "Error starting " + collectorName + " collector: {0}", exception, false);

                    if (sdtCollector != null)
                        sdtCollector.Dispose();
                    GenericFailureDelegate(snapshot, exception);
                }
            }
        }

        private void FatalErrorThread(object o)
        {
            Snapshot snapshot = o != null ? (Snapshot)o : null;
            FireCompletion(snapshot, Idera.SQLdm.Common.Services.Result.Failure);   
        }

        private void UnsupportedErrorThread(object o)
        {
            Snapshot snapshot = o != null ? (Snapshot)o : null;
            FireCompletion(snapshot, Result.Unsupported);
        }

        public void SqlErrorThread(object o)
        {
            try
            {
                object[] args = (object[])o;
                Snapshot snapshot = args[0] != null ? (Snapshot)args[0] : null;
                string collectorName = args[1] != null ? (string)args[1] : "Unknown";
                HandleSqlErrorDelegate sqlError = args[2] != null ? (HandleSqlErrorDelegate)args[2] : HandleSqlException;
                SqlException sqlException = args[3] != null ? (SqlException)args[3] : null;
                SqlCollector sdtCollector = args[4] != null ? (SqlCollector)args[4] : null;
              
                sqlError(snapshot, collectorName, sqlException, sdtCollector);
            }
            catch (Exception e)
            {
                    object[] args = (object[])o;
                    Snapshot snapshot = args[0] != null ? (Snapshot)args[0] : null;
                    SqlException sqlException = args[3] != null ? (SqlException)args[3] : null;
                    LOG.Error("An exception was encountered in SqlErrorThread.  ", e);
                    GenericFailureDelegate(snapshot, sqlException);
            }
                     
        }

        /// <summary>
        /// Queue the completion callback to a work queue
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="collector"></param>
        /// <param name="collectionCompleteCallback"></param>
        /// <param name="args"></param>
        protected void QueueCallback(Snapshot snapshot, SqlCollector collector, EventHandler<CollectorCompleteEventArgs> collectionCompleteCallback, CollectorCompleteEventArgs args)
        {
            Collection.QueueDelegate(delegate() {
                try
                {
                    if (collector != null)
                    {
                        args.Database = collector.GetDatabase();
                    }
                    collectionCompleteCallback(collector, args);

                    // Tolga K - to fix memory leak
                    args = null;
                }
                catch (Exception e)
                {
                    LOG.Error("Unhandled exception in async command callback.  Feeding exception back to callback.");
                    bool unhandledException = args.Exception != null;
                    if (!unhandledException)
                    {
                        try
                        {
                            // try again and pass it the exception
                            collectionCompleteCallback(collector, new CollectorCompleteEventArgs(null, e));
                        }
                        catch (Exception exception)
                        {
                            unhandledException = true;
                        }
                    }
                    if (unhandledException)
                    {
                        // an exception processing the exception - force failure of the snapshot
                        LOG.ErrorFormat("Unhandled exception in exception handler for async command callback.  Calling Generic failure handler.");
                        FireCompletion(snapshot, Idera.SQLdm.Common.Services.Result.Failure);
                    }
                }
            });
        }

        /// <summary>
        /// Callback used to process the data returned from a generic collector.  
        /// This method completes the probe with a failure result for any error.
        /// </summary>
        protected void GenericCallback(CollectorCallback callback, Snapshot snapshot, string logger, 
            string collectorName, object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(callback, snapshot, logger, collectorName, new FailureDelegate(GenericFailureDelegate),
                            new FailureDelegate(GenericFailureDelegate), sender, e);
        }

        /// <summary>
        /// Callback used to process the data returned from a generic collector.  
        /// This method completes the probe with a failure result for any error.
        /// </summary>
        protected void GenericCallback(CollectorCallback callback, Snapshot snapshot, string logger,
            string collectorName, NextCollector nextCollector, object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(callback, snapshot, logger, collectorName, new FailureDelegate(GenericFailureDelegate),
                            new FailureDelegate(GenericFailureDelegate), nextCollector, sender, e, false);
        }

        /// <summary>
        /// Callback used to process the data returned from a generic collector.
        /// This method expects delegates for the behavior when an error is returned with the collector 
        /// or when an exception occurs during the Callback delegate.
        /// </summary>
        protected void GenericCallback(CollectorCallback callback, Snapshot snapshot, string logger, string collectorName,
            FailureDelegate actionOnCollectorFailed, FailureDelegate actionOnException, object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(callback, snapshot, logger, collectorName, actionOnCollectorFailed, actionOnCollectorFailed,
                            null, sender, e, false);
        }

        ///// <summary>
        ///// Callback used to process the data returned from a generic collector.  
        ///// This method completes the probe with a failure result for any error.
        ///// </summary>
        //protected void GenericCallback(CollectorCallback callback, Snapshot snapshot, string logger,
        //    string collectorName, NextCollector nextCollector, object sender, CollectorCompleteEventArgs e, bool WarnOnError)
        //{
        //    GenericCallback(callback, snapshot, logger, collectorName, new FailureDelegate(GenericFailureDelegate),
        //                    new FailureDelegate(GenericFailureDelegate), nextCollector, sender, e, WarnOnError);
        //}

        /// <summary>
        /// Callback used to process the data returned from a generic collector.
        /// This method expects delegates for the behavior when an error is returned with the collector 
        /// or when an exception occurs during the Callback delegate.
        /// </summary>
        protected void GenericCallback(CollectorCallback callback, Snapshot snapshot, string logger, string collectorName,
            FailureDelegate actionOnCollectorFailed, FailureDelegate actionOnException, object sender, CollectorCompleteEventArgs e, bool WarnOnError)
        {
            GenericCallback(callback, snapshot, logger, collectorName, actionOnCollectorFailed, actionOnCollectorFailed,
                            null, sender, e, WarnOnError);
        }

        ///// <summary>
        ///// Callback used to process the data returned from a generic collector.
        ///// </summary>
        //protected void GenericCallback(CollectorCallback callback, Snapshot snapshot, string logger, string collectorName,
        //    FailureDelegate actionOnCollectorFailed, FailureDelegate actionOnException, NextCollector nextCollector, object sender, CollectorCompleteEventArgs e)
        // {
        //    GenericCallback(callback, snapshot, logger, collectorName, actionOnCollectorFailed, actionOnException,
        //                    nextCollector, sender, e, false);
        // }

        /// <summary>
        /// Callback used to process the data returned from a generic collector.
        /// This method expects delegates for the behavior when an error is returned with the collector 
        /// or when an exception occurs during the Callback delegate.
        /// This method further expects a delegate for the start of the next collector in the chain
        /// </summary>
        protected void GenericCallback(CollectorCallback callback, Snapshot snapshot, string logger, string collectorName,
            FailureDelegate actionOnCollectorFailed, FailureDelegate actionOnException, NextCollector nextCollector, object sender, CollectorCompleteEventArgs e,
            bool WarnOnError)
        {
            GenericCallback(callback, snapshot, logger, collectorName, actionOnCollectorFailed, actionOnException,
                            nextCollector, sender, e, WarnOnError, false);
        }

        /// <summary>
        /// Callback used to process the data returned from a generic collector.
        /// This method expects delegates for the behavior when an error is returned with the collector 
        /// or when an exception occurs during the Callback delegate.
        /// This method further expects a delegate for the start of the next collector in the chain
        /// </summary>
        protected void GenericCallback(CollectorCallback callback, Snapshot snapshot, string logger, string collectorName,
            FailureDelegate actionOnCollectorFailed, FailureDelegate actionOnException, NextCollector nextCollector, object sender, CollectorCompleteEventArgs e,
            bool WarnOnError, bool suppressCollectorFailedError, FailureDelegate actionOnPermissionsViolation = null)
        {
            using (LOG.DebugCall(logger))
            {
                try
                {
                    // Proceed only for non violations
                    if (e.Result != Result.PermissionViolation)
                    {
                        using (ICollector collector = sender as ICollector)
                        {
                            LOG.Verbose(String.Format("{0} collector ran in {1} milliseconds", collectorName,
                                e.ElapsedMilliseconds));
                            if (suppressCollectorFailedError || e.Result == Idera.SQLdm.Common.Services.Result.Success)
                            {
                                callback(e);
                            }
                            else
                            {
                                if (e.Exception.GetType() == typeof(SqlException))
                                {
                                    if (((SqlException) e.Exception).Number == 321)
                                    {
                                        e.Exception =
                                            new Exception(
                                                "The master database on the monitored SQL 2005 server is set to SQL 2000 compatibility.  Please update the database compatibility mode with the command \"sp_dbcmptlevel 'master','90'\" to resume monitoring.",
                                                e.Exception);
                                    }
                                }

                            ProbeHelpers.LogAndAttachToSnapshot(snapshot, LOG,
                                                               "Error executing " + collectorName + " Collector: {0}",
                                                               e.Exception, WarnOnError);
                            actionOnCollectorFailed(snapshot, e.Exception);

                                // If generic failure delegate is called, do not allow collection to proceed
                                if (actionOnCollectorFailed == GenericFailureDelegate)
                                {
                                    nextCollector = null;
                                }
                            }
                        }
                    }
                    else // e.Result == Result.PermissionViolation
                    {
                        // To Suppress Collection Failures
                        if (suppressCollectorFailedError)
                        {
                            callback(e);
                        }
                    }

                    // Fire Action Failed Event if present for Scheduled Refresh Probes or Result not permission violation for on demand
                    Type snapshotType = snapshot.GetType();
                    if (nextCollector != null && (snapshotType == typeof(ScheduledRefresh) || e.Result == Result.Success || suppressCollectorFailedError))
                    {
                        if (!snapshot.CollectionFailed)
                        {
                            nextCollector();
                        }
                        else
                        {
                            // Already failed snapshot
                            LOG.Warn("Collection failed on collector " + collectorName +
                                     ".  Exiting probe.  Collection failed with error: " + snapshot.Error.Message);
                            GenericFailureDelegate(snapshot, snapshot.Error);
                        }
                    }
                    else if (e.Result == Result.PermissionViolation)
                    {
                        // create and pass the probe error argument to handle and store errors
                        ProbePermissionHelpers.ProbeError probeError = null;
                        var senderArgs = e.Value as object[];
                        if (senderArgs != null && senderArgs.Length >= 2 &&
                            senderArgs[1] is ProbePermissionHelpers.ProbeError)
                        {
                            probeError = (ProbePermissionHelpers.ProbeError) senderArgs[1];
                        }

                        // Update Probe Error
                        probeError = probeError ?? new ProbePermissionHelpers.ProbeError()
                        {
                            Name = collectorName
                        };

                        // Update ProbeError
                        snapshot.ProbeError = snapshot.ProbeError ?? probeError;
                        if (actionOnPermissionsViolation != null && actionOnPermissionsViolation != GenericFailureDelegate)
                        {
                            actionOnPermissionsViolation(snapshot, new Exception(probeError.ToString()));
                        }
                        else if (actionOnCollectorFailed != null && actionOnCollectorFailed != GenericFailureDelegate)
                        {
                            actionOnCollectorFailed(snapshot, new Exception(probeError.ToString()));
                        }
                        else if (actionOnException != null && actionOnCollectorFailed != GenericFailureDelegate)
                        {
                            actionOnException(snapshot, new Exception(probeError.ToString()));
                        }
                        else
                        {
                            // No Next Collector present then fire completion for PermissionViolation - Pass the Collector Name
                            FireCompletion(snapshot, Result.PermissionViolation, collectorName, probeError);
                        }
                    }
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot, LOG,
                                                               "Error processing " + collectorName + " Collector: {0}",
                                                               exception, WarnOnError);
                    actionOnException(snapshot, exception);
                }
            }
        }

        /// <summary>
        /// Handle a SQL exception that occurred before a collector was prepared
        /// </summary>
        /// <param name="snapshot">Snapshot to copy error message to</param>
        /// <param name="collectorName">Friendly collector name for error message</param>
        /// <param name="sqlException">SQL Exception that is being caught</param>
        protected void HandleSqlException(Snapshot snapshot, string collectorName, SqlException sqlException)
        {
            HandleSqlException(snapshot, collectorName, sqlException, "Not available - Failure may have occurred while establishing connection");
        }

        /// <summary>
        /// Handle a SQL exception and write failing SQL to log
        /// </summary>
        /// <param name="snapshot">Snapshot to copy error message to</param>
        /// <param name="collectorName">Friendly collector name for error message</param>
        /// <param name="sqlException">SQL Exception that is being caught</param>
        /// <param name="collector">Collector that is source of the error</param>
        protected void HandleSqlException(Snapshot snapshot, string collectorName, SqlException sqlException, SqlCollector collector)
        {
            if (collector != null)
            {
                HandleSqlException(snapshot, collectorName, sqlException, collector.SqlText);
                collector.Dispose();
            }
            else
            {
                HandleSqlException(snapshot, collectorName, sqlException);
            }
            GenericFailureDelegate(snapshot, sqlException);
        }

        /// <summary>
        /// Handle a SQL exception
        /// </summary>
        /// <param name="snapshot">Snapshot to copy error message to</param>
        /// <param name="collectorName">Friendly collector name for error message</param>
        /// <param name="sqlException">SQL Exception that is being caught</param>
        /// <param name="sqlText">Text of failing SQL if available</param>
        protected void HandleSqlException(Snapshot snapshot, string collectorName, SqlException sqlException, string sqlText)
        {
            switch (sqlException.Number)
            {
                case 17142:
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot, LOG, "Monitored server is paused", false);
                    break;
                case 2:
                case 53:
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot, LOG, "Monitored server cannot be contacted: " + sqlException.Message, false);
                    break;
                case 18752:
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot, LOG, "Error executing " + collectorName + " Collector.  SQL Server does not allow two connections to execute log-related procedures simultaneously.  This may occur if two copies of SQLdm monitor replication simultaneously.  Please try again in a few minutes.",sqlException, false);
                    break;
                default:
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot, LOG, "Error executing " + collectorName + " Collector: {0}", sqlException, false);
                    LOG.Verbose("Failing SQL: " + sqlText);
                    break;

            }
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
