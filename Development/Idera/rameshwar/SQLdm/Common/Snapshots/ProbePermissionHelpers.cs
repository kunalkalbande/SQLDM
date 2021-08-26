using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using BBS.TracerX;

namespace Idera.SQLdm.Common.Snapshots
{
    /// <summary>
    /// SQLdm Minimum Privileges - Varun Chopra - Validate server permissions
    /// </summary>
    public static class ProbePermissionHelpers
    {
        /// <summary>
        /// Logger for Probe Permissions Helpers
        /// </summary>
        private static Logger LOG = Logger.GetLogger("ProbePermissionHelpers");

        /// <summary>
        /// Minimum Permissions for SQL
        /// </summary>
        private static readonly MinimumPermissions minimumPermissionsThresholdSql = MinimumPermissions.VIEWANYDATABASE | MinimumPermissions.VIEWANYDEFINITION | MinimumPermissions.VIEWSERVERSTATE;

        /// <summary>
        /// Minimum Permissions for Amazon RDS
        /// </summary>
        private static readonly MinimumPermissions minimumPermissionsThresholdRds = MinimumPermissions.VIEWANYDATABASE | MinimumPermissions.VIEWANYDEFINITION | MinimumPermissions.VIEWSERVERSTATE;

        /// <summary>
        /// Minimum Permissions for Azure
        /// </summary>
        private static readonly MinimumPermissions minimumPermissionsThresholdAzure = MinimumPermissions.DbDataReaderAccess | MinimumPermissions.VIEWDEFINITION | MinimumPermissions.SelectAccess | MinimumPermissions.VIEWDATABASESTATE | MinimumPermissions.ExecuteAccess;

        /// <summary>
        /// Minimum Permissions for Linux
        /// </summary>
        private static readonly MinimumPermissions minimumPermissionsThresholdLinux = MinimumPermissions.VIEWANYDATABASE | MinimumPermissions.VIEWANYDEFINITION | MinimumPermissions.VIEWSERVERSTATE;

        private const string ALTERDATABASE = "ALTERDATABASE";

        /// <summary>
        /// '{0}' Probe Permission Violation - The user account used by the collection service does not have required rights required on the monitored server. {1}
        /// </summary>
        private const string FAILEDPERMISSIONSFORMAT = @"'{0}' Probe Permission Violation - The user account used by the collection service does not have required rights on the monitored server. 
 {1}";

        /// <summary>
        /// '{0}' Probe Permission Violation - The user account used by the collection service does not have minimum rights required on the monitored server. {1}
        /// </summary>
        private const string FAILEDMINIMUMPERMISSIONSFORMAT = @"'{0}' Probe Permission Violation - The user account used by the collection service does not have minimum rights required on the monitored server. 
 {1}";

        private const string Space = " ";

        /// <summary>
        /// Resource file read failed
        /// </summary>
        public const string ResourceReadFailed = "Please review the product documentation for required permissions.";

        /// <summary>
        /// Stores and passes down Probe Error Message
        /// </summary>
        [Serializable()]
        public class ProbeError
        {
            /// <summary>
            /// Ctor which initializes <seealso cref="FormatString"/>
            /// </summary>
            public ProbeError()
            {
                FormatString = FAILEDPERMISSIONSFORMAT;
                RequiredPermissions = ResourceReadFailed;
            }

            /// <summary>
            /// Required Permissions for Probe. 
            /// </summary>
            /// <remarks>
            /// Read from Idera.SQLdm.Common.ProbePermissionsResource.resx
            /// </remarks>
            public string RequiredPermissions { get; set; }

            /// <summary>
            /// Probe Name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Format String to use <see cref="Name"/> as {0} and <see cref="RequiredPermissions"/> as {1}
            /// </summary>
            /// <remarks>
            /// '{0}' Probe Permission Violation - The user account used by the collection service does not have required rights required on the monitored server. {1}
            /// </remarks>
            public string FormatString { get; set; }

            /// <summary>
            /// To String to print Probe Permissions Violation Error Message
            /// </summary>
            /// <remarks>
            /// string.Format(<see cref="FormatString"/>, <see cref="Name"/>, <see cref="RequiredPermissions"/>);
            /// </remarks>
            /// <returns>Formatted string output</returns>
            public override string ToString()
            {
                return string.Format(FormatString, Name, RequiredPermissions);
            }
        }

        /// <summary>
        /// Gets Minimum Permissions based on the server under monitoring
        /// </summary>
        /// <param name="cloudProviderId">
        /// 
        /// </param>
        /// <returns></returns>
        public static MinimumPermissions GetMinimumPermissionsThreshold(int? cloudProviderId)
        {
            // To handle minimum permissions based on cloudprovider
            switch (cloudProviderId)
            {
                case Constants.AmazonRDSId:
                    return minimumPermissionsThresholdRds;
                case Constants.MicrosoftAzureId:
                    return minimumPermissionsThresholdAzure;
                case Constants.LinuxId:
                    return minimumPermissionsThresholdLinux;
                case null:
                    return minimumPermissionsThresholdSql;
                default:
                    return minimumPermissionsThresholdSql;
            }
        }

        public class CachedPermissions
        {
            public MinimumPermissions MinimumPermissions { get; set; }
            public CollectionPermissions CollectionPermissions { get; set; }
            public MetadataPermissions MetadataPermissions { get; set; }
            public string InstanceName { get; set; }
            public string DatabaseName { get; set; }
            public DateTime UpdatedOn { get; set; }
        }

        private static readonly Dictionary<string, Dictionary<string, CachedPermissions>> PermissionsCacheMap =
            new Dictionary<string, Dictionary<string, CachedPermissions>>();

        public static DateTime CacheCreationDate = DateTime.UtcNow;
        public static double CachedPermissionsRefreshTimeout = 120;


        public static CachedPermissions GetCachedPermissions(string instanceName, string databaseName)
        {
            try
            {
                if (PermissionsCacheMap.ContainsKey(instanceName) &&
                    PermissionsCacheMap[instanceName].ContainsKey(databaseName))
                {
                    return PermissionsCacheMap[instanceName][databaseName];
                }

            }
            catch (Exception ex)
            {
                LOG.Error("GetCachedPermissions: Error in getting the permissions froom the cache " + ex);
            }
            finally
            {
                if ((DateTime.UtcNow - CacheCreationDate).TotalDays < 1)
                {
                    CacheCreationDate = DateTime.UtcNow;
                    PermissionsCacheMap.Clear();
                }
            }
            return null;
        }

        public static void SetCachedPermissions(CachedPermissions permissionsCache)
        {

            try
            {
                if (!PermissionsCacheMap.ContainsKey(permissionsCache.InstanceName))
                {
                    PermissionsCacheMap.Add(permissionsCache.InstanceName, new Dictionary<string, CachedPermissions>());
                }

                if (!PermissionsCacheMap[permissionsCache.InstanceName].ContainsKey(permissionsCache.DatabaseName))
                {
                    PermissionsCacheMap[permissionsCache.InstanceName].Add(permissionsCache.DatabaseName, permissionsCache);
                }
                else
                {
                    PermissionsCacheMap[permissionsCache.InstanceName][permissionsCache.DatabaseName] = permissionsCache;
                }

            }
            catch (Exception ex)
            {
                LOG.Error("SetCachedPermissions: Error in setting the permissions to the cache " + ex);
            }
        }

        /// <summary>
        /// Read Permissions to Enums - Provided the permissions name is same as enum name
        /// </summary>
        /// <typeparam name="T">Type of Enums</typeparam>
        /// <param name="dataReader">Datareader to read enum values</param>
        /// <param name="log">Logger</param>
        /// <param name="failureException">May be required for Logging or attaching to snapshot errors</param>
        /// <returns>Populated Enum Variable of Type T</returns>
        internal static T ReadPermissionsToEnum<T>(SqlDataReader dataReader, Logger log, out Exception failureException) where T : struct, IComparable, IFormattable, IConvertible
        {
            failureException = null;
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            // Server Permission to be returned
            dynamic serverPermission = default(T);
            var methodName = string.Format("{0} : {1}", "ReadPermissionsToEnum", enumType.Name);
            using (log.DebugCall(methodName))
            {
                try
                {
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            var permissionNameIndex = dataReader.GetOrdinal("PermissionName");
                            var permissionValueIndex = dataReader.GetOrdinal("PermissionValue");
                            if (!dataReader.IsDBNull(permissionNameIndex))
                            {
                                var permissionName = dataReader.GetString(permissionNameIndex);
                                if (!dataReader.IsDBNull(permissionValueIndex))
                                {
                                    if (dataReader.GetInt32(permissionValueIndex) != 0)
                                    {
                                        try
                                        {
                                            // Parse Value from Data Reader
                                            T permissionFromDataReader;
                                            if (Enum.TryParse<T>(permissionName, true, out permissionFromDataReader))
                                            {
                                                serverPermission |= permissionFromDataReader;
                                            }
                                            else
                                            {
                                                log.Debug(string.Format("{0}: Permission Parsing failed for {1}",
                                                    methodName, permissionName));
                                            }

                                        }
                                        catch (Exception exception)
                                        {
                                            log.Debug(string.Format("{0}: Exception Occured for Permission {1} - {2}",
                                                methodName, permissionName, exception));
                                        }
                                    }
                                    else
                                    {
                                        log.Debug(string.Format("{0}: Permission denied for {1}", methodName,
                                            permissionName));
                                    }
                                }
                                else
                                {
                                    log.Debug(string.Format("{0}: Null permission value for {1}", methodName,
                                        permissionName));
                                }
                            }
                            else
                            {
                                log.Debug(string.Format("{0}: Null permission name at index {1}", methodName,
                                    permissionNameIndex));
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    log.Error(string.Format("Read {0} Permissions failed: {1}", enumType.Name, exception));
                    // Attach error only if optional parameter is not null
                    failureException = exception;
                }
                finally
                {
                    dataReader.NextResult();
                }
                return serverPermission;
            }
        }


        /// <summary>
        /// Read Permissions to Enums - Provided the permissions name is same as enum name
        /// </summary>
        /// <typeparam name="T">Type of Enums</typeparam>
        /// <param name="dataReader">Datareader to read enum values</param>
        /// <param name="log">Logger</param>
        /// <returns>Populated Enum Variable of Type T</returns>
        public static T ReadPermissionsToEnum<T>(SqlDataReader dataReader, Logger log) where T : struct, IComparable, IFormattable, IConvertible
        {
            Exception exception;
            return ReadPermissionsToEnum<T>(dataReader, log, out exception);
        }

        private static List<string> azureCollectionsRequiredAdditionalAccess = new List<string>()
        {
            "StopActivityMonitorTraceConfiguration",
            "StartQueryMonitorTraceConfiguration",
            "StopQueryMonitorTraceConfiguration",
            "StopSessionDetailsTraceConfiguration",
            "QueryStore",
            "Active Waits",
            "Query Monitor",
            "Wait Stats",
            "Activity Monitor"
        };

        private static bool VerifyForAzure(string collectorName, int? cloudProviderId,
            MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions,
            CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            switch (collectorName)
            {
                case "QueryStore":
                    {
                        var metaPermissionsFlag = metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersysplanguides);

                        if (metaPermissionsFlag)
                        {
                            return true;
                        }
                        // Append and log probe error
                        AppendAndLogProbeError(permissionArgs, collectorName);

                        return false;
                    }
                case "Wait Stats":
                    {
                        // admin only
                        return collectionPermissions.HasFlag(CollectionPermissions.ViewDatabaseStateMaster);
                    }
                default:
                    {
                        return collectionPermissions.HasFlag(CollectionPermissions.ViewDatabaseStateMaster) ||
                               collectionPermissions.HasFlag(CollectionPermissions.ControlDb);
                    }
            }
        }

        private delegate bool PermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs);
        private static readonly Dictionary<string, PermissionFunction> MapValidation = new Dictionary<string, PermissionFunction>
        {
            {"Server Basics", ServerBasicsPermissionFunction},
            {"DTC Status", DTCStatusPermissionFunction},
            {"Replication", ReplicationPermissionFunction},
            {"Database Size", DatabaseSizePermissionFunction},
            {"Active Waits", ActiveWaitsPermissionFunction},
            {"Lock Details", LockDetailsPermissionFunction},
            {"Backup Restore History", BackupRestorePermissionFunction},
            {"Bombed job variables store", BombedJobPermissionFunction},
            {"BufferPoolExtIO", BufferPoolExtIOPermissionFunction},
            {"Dependent Object", DependentObjectPermissionFunction},
            {"DisabledIndexes", DisabledIndexesPermissionFunction},
            {"Drop Plans", DropPlansPermissionFunction},
            {"Table Fragmentation", TableFragmentationPermissionFunction},
            {"Table FragmentationWorkload", TableFragmentationWorkloadPermissionFunction},
            {"FragmentedIndexes", FragmentedIndexesPermissionFunction},
            {"Full Text Catalogs", FullTextCatalogsPermissionFunction},
            {"WorstFillFactorIndexes", WorstFillFactorIndexesPermissionFunction},
            {"HashIndex", HashIndexPermissionFunction},
            {"HighIndexUpdates", HighIndexUpdatesPermissionFunction},
            {"HighCPUTimeProcedures", HighCPUTimeProceduresPermissionFunction},
            {"Server Details", ServerDetailsPermissionFunction},
            {"Wait Stats", WaitStatisPermissionFunction},
            {"Server Action", ServerActionPermissionFunction},
            {"Oldest Open Transaction", OldestOpenTransactionPermissionFunction},
            {"OutOfDateStats", OutOfDateStatsPermissionFunction},
            {"Job Alerts", JobAlertsPermissionFunction},
            {"ColumnStoreIndex", ColumnStoreIndexPermissionFunction},
            {"DatabaseFileInfo", DatabaseFileInfoPermissionFunction},
            {"Full Text Search Service", FullTextSearchServicePermissionFunction},
            {"Full Text Columns", FullTextColumnsPermissionFunction},
            {"Full Text Tables", FullTextTablesPermissionFunction},
            {"LargeTableStats", LargeTableStatsPermissionFunction},
            {"Index Statistics", IndexStatisticsPermissionFunction},
            {"Log Scan", LogScanPermissionFunction},
            {"Log Scan variable store", LogScanvariablestorePermissionFunction},
            {"Log List", LogListPermissionFunction},
            {"MirrorMetricsUpdateCallback", MirrorMetricsUpdateCallbackPermissionFunction},
            {"MirrorMonitoringHistoryCallback", MirrorMonitoringHistoryCallbackPermissionFunction},
            {"CustomCounterOS", CustomCounterOSPermissionFunction},
            {"Database Files", DatabaseFilesPermissionFunction},
            {"Database Summary", DatabaseSummaryPermissionFunction},
            {"DBSecurity", DBSecurityPermissionFunction},
            {"Disk Drives", DiskDrivesPermissionFunction},
            {"Distributor Queue", DistributorQueuePermissionFunction},
            {"File Activity", FileActivityPermissionFunction},
            {"Server Overview", ServerOverviewPermissionFunction},
            {"File Group", FileGroupPermissionFunction},
            {"FilteredColumnNotInKeyOfFilteredIndex", FilteredColumnNotInKeyOfFilteredIndexPermissionFunction},
            {"MirrorMonitoringRealtimeCallback", MirrorMonitoringRealtimeCallbackPermissionFunction},
            {"IndexContention", IndexContentionPermissionFunction},
            {"NonIncrementalColumnStatsCallback", NonIncrementalColumnStatsCallbackPermissionFunction},
            {"Query Monitor", QueryMonitorPermissionFunction},
            {"Services", ServicesPermissionFunction},
            {"Session Details", SessionDetailsPermissionFunction},
            {"Session List", SessionListPermissionFunction},
            {"LockedPageKB", LockedPageKBPermissionFunction},
            {"OverlappingIndexes", OverlappingIndexesPermissionFunction},
            {"QueryPlanEstRows", QueryPlanEstRowsPermissionFunction},
            {"QueryStore", QueryStorePermissionFunction},
            {"RarelyUsedIndexOnInMemoryTable", RarelyUsedIndexOnInMemoryTablePermissionFunction},
            {"Distributor Details", DistributorDetailsPermissionFunction},
            {"SampleServerResources", SampleServerResourcesPermissionFunction},
            {"Table Growth", TableGrowthPermissionFunction},
            {"Session Summary", SessionSummaryPermissionFunction},
            {"Replication Status", ReplicationStatusPermissionFunction},
            {"SQLModuleOptions", SQLModuleOptionsPermissionFunction},
            {"WaitingBatches", WaitingBatchesPermissionFunction},
            {"NUMANodeCounters", NUMANodeCountersPermissionFunction},
            {"Table Details", TableDetailsPermissionFunction},
            {"AlwaysOn Metrics", AlwaysOnMetricsPermissionFunction},
            {"BackupAndRecovery", BackupAndRecoveryPermissionFunction},
            {"sp_OA Check/Test", sp_OACheckPermissionFunction},
            {"Procedure Cache", ProcedureCachePermissionFunction},
            {"Mirror Monitoring Real-time", MirrorMonitoringRealtimePermissionFunction},
            {"ServerConfiguration", ServerConfigurationPermissionFunction},
            {"OS Metrics", OSMetricsPermissionFunction},
            {"Agent Job History", AgentJobHistoryPermissionFunction},
            {"AgentJobSummary", AgentJobSummaryPermissionFunction},
            {"Agent Job Summary", Agent_Job_SummaryPermissionFunction},
            {"Query Monitor State Store", QueryMonitorStateStorePermissionFunction},
            {"Activity Monitor", ActivityMonitorPermissionFunction},
            {"Configuration", ConfigurationPermissionFunction},
         };

        /// <summary>
        /// ServerBasics2000.sql and ServerBasics2005.sql batch
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool ServerBasicsPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = collectionPermissions.HasFlag(
                CollectionPermissions.VIEWDEFINITION
                //| CollectionPermissions.ALTERSETTINGS // Required by sp_configure but used with Reconfigure which requires sysadmin, hence covered in batch condition
                );
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }

            var metadataPermissionsFlag =
                metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilityXpMsver);
            // Log failed permissions for Collector
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// DTCStatus -  Vamshi/Varun - We Need Revisit this - WMI change
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool DTCStatusPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = true; //collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember);   If this becomes a problem in a future release, we should probably add a different kind of check

            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// ReplicationCheck AND ReplicationCheck2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool ReplicationPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag =
                collectionPermissions.HasFlag(CollectionPermissions.CREATEDATABASE) &&
                (collectionPermissions.HasFlag(CollectionPermissions.ALTERANYLINKEDSERVER) ||
                 collectionPermissions.HasFlag(CollectionPermissions.ALTERANYLOGIN)) &&
                collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember |
                                              CollectionPermissions.ReplicationCheck |  // DB_Owner at publication and access to distribution
                                              CollectionPermissions.EXECUTESERVER);
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }

            var metadataPermissionsFlag =
                metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitysysobjects);
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// NOTE - Requires membership in the public role.
        /// DATABASESIZE2012 - Handled Required Permissions
        /// DiskSize2005 - Required Permissions already handled
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool DatabaseSizePermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            bool collectionPermissionsFlag = false;
            if (productVersion.Major > 11)
            {
                switch (cloudProviderId)
                {
                    case Constants.MicrosoftAzureId:
                        collectionPermissionsFlag = collectionPermissions.HasFlag(
                                                            CollectionPermissions.AzureAdmin)
                                                        || (collectionPermissions.HasFlag(
                                                            CollectionPermissions.SERVERADMIN) &&
                                                            collectionPermissions.HasFlag(
                                                                //CollectionPermissions.ALTERSERVERSTATE
                                                                //| // DBCC SQLPERF Reset / Wait / Launch
                                                                CollectionPermissions.MSDBAccess));
                        break;
                    default:
                        collectionPermissionsFlag = true;
                        //collectionPermissions.HasFlag(
                        //    /*CollectionPermissions.ALTERSERVERSTATE |*/    // DBCC SQLPERF Reset / Wait / Launch
                        //    CollectionPermissions.MSDBAccess) || collectionPermissions.HasFlag(
                        //        CollectionPermissions.SYSADMINMember);
                        break;
                }
                if (!collectionPermissionsFlag)
                {
                    // Append and log probe error
                    AppendAndLogProbeError(permissionArgs, collectorName);
                    return false;
                }

                // Included checks for public role
                var metadataPermissionsFlag =
                    metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitysysfiles | MetadataPermissions.MetadataVisibilitymastersysinternaltables | MetadataPermissions.MetadataVisibilitySysAllocationUnits | MetadataPermissions.MetadataVisibilitySysPartitions | MetadataPermissions.MetadataVisibilityTempdbSysDatabaseFiles) && (metadataPermissions.HasFlag(MetadataPermissions.TempDbDataWriter) || metadataPermissions.HasFlag(MetadataPermissions.TempDbDbOwnerAccess) || collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember));

                if (!metadataPermissionsFlag)
                {
                    // Append and log probe error
                    AppendAndLogProbeError(permissionArgs, collectorName);
                    return false;
                }
                return true;
            }
            else
            {
                switch (cloudProviderId)
                {
                    case Constants.MicrosoftAzureId:
                        collectionPermissionsFlag =
                            (collectionPermissions.HasFlag(CollectionPermissions.CREATEDATABASE) ||
                             collectionPermissions.HasFlag(CollectionPermissions.ALTERANYDATABASE)) &&
                             collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) &&
                             collectionPermissions.HasFlag(CollectionPermissions.MSDBAccess);
                        break;
                    default:
                        collectionPermissionsFlag =
                            (collectionPermissions.HasFlag(CollectionPermissions.CREATEDATABASE) ||
                             collectionPermissions.HasFlag(CollectionPermissions.ALTERANYDATABASE)) &&
                             collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember) &&
                             collectionPermissions.HasFlag(CollectionPermissions.MSDBAccess);
                        break;
                }
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// ActiveWaits2012.sql batch with extended events
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool ActiveWaitsPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            bool collectionPermissionsFlag = false;
            if (permissionArgs != null && permissionArgs.Length >= 2)
            {
                // try parsing enabled xe
                try
                {
                    // Parse if xe enable
                    var enabledXe = (bool)permissionArgs[0];
                    var enabledQs = (bool)permissionArgs[1];

                    // Validate Permissions
                    if (productVersion.Major >= 13 && enabledQs)
                    {
                        // Minimum permission set you need to have in order to use Query Store
                        collectionPermissionsFlag = metadataPermissions.HasFlag(MetadataPermissions.TempDbDbOwnerAccess);
                        // Note: We are not configuring here so db owner not required
                    }
                    else if (productVersion.Major >= 11 && enabledXe)
                    {
                        // Default case - check for both extended events and trace conditions
                        collectionPermissionsFlag =
                            collectionPermissions.HasFlag(CollectionPermissions.ALTERANYEVENTSESSION);
                    }
                    else
                    {
                        // Default case - check for both extended events and trace conditions
                        collectionPermissionsFlag =
                            collectionPermissions.HasFlag(CollectionPermissions.ALTERTRACE);
                    }

                    // Handling Tempdb Permissions
                    collectionPermissionsFlag = collectionPermissionsFlag && (metadataPermissions.HasFlag(MetadataPermissions.TempDbDataWriter)
                        || metadataPermissions.HasFlag(MetadataPermissions.TempDbDbOwnerAccess)
                        || collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember));

                    // Log Error and Exit
                    if (!collectionPermissionsFlag)
                    {
                        AppendAndLogProbeError(
                            permissionArgs,
                            collectorName,
                            enabledQs ? "Qs" : enabledXe ? "Ex" : "Trc");
                    }
                    return collectionPermissionsFlag;
                }
                catch (Exception exception)
                {
                    LOG.Error("Failed to parse Active Waits permission Args : " + exception);
                }
            }
            // Default case - check for both extended events and trace conditions
            collectionPermissionsFlag =
                collectionPermissions.HasFlag(
                    CollectionPermissions.ALTERANYEVENTSESSION | CollectionPermissions.ALTERTRACE)
                && (metadataPermissions.HasFlag(MetadataPermissions.TempDbDataWriter)
                || metadataPermissions.HasFlag(MetadataPermissions.TempDbDbOwnerAccess)
                || collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember));

            // Log Error and Exit
            if (!collectionPermissionsFlag)
            {
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// Probe name - LOCKDETAILS2005, LockCounterStatistics2005
        /// Requires membership in the public role
        /// LockDetails2000.sql and LockDetails2005.sql batch
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool LockDetailsPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            bool collectionPermissionsFlag = false;
            switch (cloudProviderId)
            {
                case Constants.MicrosoftAzureId:
                    collectionPermissionsFlag =
                        collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin)
                        || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                    break;
                default:
                    collectionPermissionsFlag = true;
                    break;
            }
            if (!collectionPermissionsFlag)
            {
                // Append and Log ProbeError
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }

            // Added Checks for Sys Partitions and Sys Objects
            var metadataPermissionsFlag =
                metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersysallobjects |
                                            MetadataPermissions.MetadataVisibilitymastersysobjects |
                                            MetadataPermissions.MetadataVisibilitySysPartitions);

            if (!metadataPermissionsFlag)
            {
                // Append and Log ProbeError
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// BACKUPHISTORYFULL , BACKUPHISTORYSMALL, RestoreHistoryFull, RestoreHistorySmall
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool BackupRestorePermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            if (permissionArgs != null && permissionArgs.Length > 0) //permissionArgs - config.ShowBackups, config.ShowRestores, config.ShowLogicalFileNames
            {
                try
                {
                    if ((bool)permissionArgs[0] == true)
                    {

                        if ((bool)permissionArgs[2] == true) //BACKUPHISTORYFULL
                        {
                            var collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.MSDBAccessBACKUPFILE |
                            CollectionPermissions.MSDBAccessBACKUPMEDIAFAMILY |
                            CollectionPermissions.MSDBAccessBACKUPSET);

                            // Log failed permissions for Collector
                            if (!collectionPermissionsFlag)
                            {
                                // Append and log probe error
                                AppendAndLogProbeError(permissionArgs, collectorName);
                            }
                            return collectionPermissionsFlag;
                        }
                        else //BACKUPHISTORYSMALL
                        {
                            var collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.MSDBAccessBACKUPMEDIAFAMILY | CollectionPermissions.MSDBAccessBACKUPSET);
                            // Log failed permissions for Collector
                            if (!collectionPermissionsFlag)
                            {
                                // Append and log probe error
                                AppendAndLogProbeError(permissionArgs, collectorName);
                            }
                            return collectionPermissionsFlag;
                        }
                    }
                    if ((bool)permissionArgs[1] == true)
                    {
                        if ((bool)permissionArgs[2] == true) //RestoreHistoryFull
                        {
                            var collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.MSDBAccessBACKUPFILE |
                                    CollectionPermissions.MSDBAccessBACKUPMEDIAFAMILY |
                                    CollectionPermissions.MSDBAccessBACKUPSET |   // msdb..restorehistory permission needs to be added
                                    CollectionPermissions.MSDBRESTOREHISTORY);
                            // Log failed permissions for Collector
                            if (!collectionPermissionsFlag)
                            {
                                // Append and log probe error
                                AppendAndLogProbeError(permissionArgs, collectorName);
                            }
                            return collectionPermissionsFlag;
                        }
                        else //RestoreHistorySmall
                        {
                            var collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.MSDBAccessBACKUPMEDIAFAMILY |
                                    CollectionPermissions.MSDBAccessBACKUPSET |   // msdb..restorehistory permission needs to be added
                                    CollectionPermissions.MSDBRESTOREHISTORY);     // msdb..restorehistory permission needs to be added
                            // Log failed permissions for Collector
                            if (!collectionPermissionsFlag)
                            {
                                // Append and log probe error
                                AppendAndLogProbeError(permissionArgs, collectorName);
                            }
                            return collectionPermissionsFlag;
                        }
                    }
                }
                catch (Exception exception)
                {
                    LOG.Error("Failed to parse Backup Restore History permission Args : " + exception);
                }
            }
            // Append and log probe error
            AppendAndLogProbeError(permissionArgs, collectorName);
            return false;
        }

        /// <summary>
        /// BombedJobs
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool BombedJobPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.MSDBAccessSYSCATEGORIES |
                CollectionPermissions.MSDBAccessSYSJOBHISTORY |
                CollectionPermissions.MSDBAccessSYSJOBS |
                CollectionPermissions.MSDBAccessSYSJOBSTEPS);
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// BufferPoolExtIO2014
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool BufferPoolExtIOPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = new bool();
            switch (cloudProviderId)
            {
                case Constants.MicrosoftAzureId:
                    collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                    break;
                default:
                    collectionPermissionsFlag = true;
                    break;
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// DependentObjectsPrescriptiveRecommendation
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool DependentObjectPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var metadataPermissionsFlag =
            metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersysassemblies |
            MetadataPermissions.MetadataVisibilitymastersysassemblymodules |
            MetadataPermissions.MetadataVisibilitymastersysassemblyreferences |
            MetadataPermissions.MetadataVisibilitymastersysassemblytypes |
            MetadataPermissions.MetadataVisibilitymastersyscolumns |
            MetadataPermissions.MetadataVisibilitymastersysindexes |
            MetadataPermissions.MetadataVisibilitymastersysobjects |
            MetadataPermissions.MetadataVisibilitymastersysforeignkeys |
            MetadataPermissions.MetadataVisibilitymastersysparameters |
            MetadataPermissions.MetadataVisibilitymastersyssynonyms |
            MetadataPermissions.MetadataVisibilitymastersystables |
            MetadataPermissions.MetadataVisibilitymastersystypes |
            MetadataPermissions.MetadataVisibilitymastersysxmlschemacollections |
            MetadataPermissions.MetadataSysPartitionFunction |
            MetadataPermissions.MetadataSysPartitionSchemes |
            MetadataPermissions.MetadataSysSqlDependencies);

            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);

                return false;
            }
            return true;
        }

        /// <summary>
        /// DisabledIndexes2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool DisabledIndexesPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var metadataPermissionsFlag = metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersysindexes |
                                                                        MetadataPermissions.MetadataVisibilitySysSchemas |
                                                                        MetadataPermissions.MetadataVisibilitySysConfigurations);
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// DropPlansFromCache2008
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool DropPlansPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = new bool();
            switch (cloudProviderId)
            {
                case Constants.MicrosoftAzureId:
                    collectionPermissionsFlag =
                        collectionPermissions.HasFlag(CollectionPermissions.ALTERSERVERSTATE) &&
                        (collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin)
                        || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN));
                    break;
                default:
                    collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember) || collectionPermissions.HasFlag(CollectionPermissions.ALTERSERVERSTATE);
                    break;
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// FRAGMENTATION2005
        /// Adding Dynamic Permissions for ObjectName
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool TableFragmentationPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = false;
            //Expects permissionsArgs[0] as sqlCommand
            if (permissionArgs != null && permissionArgs.Length >= 1)
            {
                var sqlCommand = (SqlCommand)permissionArgs[0];
                // Stores Required Runtime Permissions
                var dynamicPermissions = new Dictionary<string, int>();

                // Execute Dynammic Permissions for Server Action Collector 
                PopulateDynamicPermissions(collectorName, sqlCommand, dynamicPermissions);

                // Validate Permissions
                collectionPermissionsFlag =
                    dynamicPermissions.ContainsKey("FRAGMENTATION") && dynamicPermissions["FRAGMENTATION"] == 1;
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// FRAGMENTATIONWORKLOAD2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool TableFragmentationWorkloadPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag =
               collectionPermissions.HasFlag(CollectionPermissions.ALTERANYDATABASE);

            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);

                return false;
            }

            var metadataPermissionsFlag =
                metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersysindexes |
                MetadataPermissions.MetadataVisibilitymastersysobjects |
                MetadataPermissions.MetadataVisibilitytempdbsyscolumns |
                MetadataPermissions.MetadataVisibilitySysSchemas);
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// FragmentedIndexes2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool FragmentedIndexesPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)        //        (collectorName, cloudProviderId, minimumPermissions, metadataPermissions, collectionPermissions, productVersion, permissionArgs) =>
        {
            var metadataPermissionsFlag =
               metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersysindexes | MetadataPermissions.MetadataVisibilitySysSchemas);

            // Log failed permissions for Collector
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return metadataPermissionsFlag;
        }

        /// <summary>
        /// FULLTEXTCATALOGS2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool FullTextCatalogsPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = new bool();
            switch (cloudProviderId)
            {
                case Constants.MicrosoftAzureId:
                    collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin
                        | CollectionPermissions.ALTERSERVERSTATE);
                    break;
                default:
                    collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.CREATEDATABASE
                        | CollectionPermissions.ALTERANYUSER);
                    break;
            }

            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }

            var metadataPermissionsFlag =
                metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersysfulltextcatalogs |
                MetadataPermissions.MetadataVisibilitysysobjects | MetadataPermissions.MetadataSysDataSpaces);

            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// GETWORSTFILLFACTORINDEXES2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool WorstFillFactorIndexesPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.ALTERANYDATABASE);

            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }

            var metadataPermissionsFlag =
                metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersysinternaltables |
                MetadataPermissions.MetadataVisibilitymastersysobjects |
                MetadataPermissions.MetadataVisibilitymastersysindexes | MetadataPermissions.MetadataVisibilitySysAllocationUnits | MetadataPermissions.MetadataVisibilitySysPartitions | MetadataPermissions.MetadataVisibilitySysSchemas);
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// HashIndex2014
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool HashIndexPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember);

            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);

                return false;
            }

            var metadataPermissionsFlag = metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersyshashindexes);
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// HIGHINDEXUPDATES2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool HighIndexUpdatesPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = new bool();
            switch (cloudProviderId)
            {
                case Constants.MicrosoftAzureId:
                    collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                    break;
                default:
                    collectionPermissionsFlag = true;
                    break;
            }
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            var metadataPermissionsFlag =
                 metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersysindexes |
                 MetadataPermissions.MetadataVisibilitymastersystables);
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// HIGHCPUTIMEPROCEDURE2016
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool HighCPUTimeProceduresPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = new bool();
            if (productVersion.Major >= 13)
            {
                switch (cloudProviderId)
                {
                    case Constants.MicrosoftAzureId:
                        collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                        break;
                    default:
                        collectionPermissionsFlag = true;
                        break;
                }
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// MEMORY2012, ResourceCheck2005, BlockingCheck2005, WaitStatistics, ServerDetails2012
        /// For all 3, the permissions are same and no check for ProductVersion is required
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool ServerDetailsPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            //For all MEMORY2012, ResourceCheck2005, BlockingCheck2005, WaitStatistics,, the permissions are same and no check for ProductVersion is required
            var collectionPermissionsFlag = new bool();
            switch (cloudProviderId)
            {
                case Constants.MicrosoftAzureId:
                    collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin);
                    break;
                default:
                    // SysAdmin Covers the required permissions
                    // 1. ResourceCheck -> Requires Update/Insert/Delete/Select on Tempdb
                    // 2. procedurecache2005 -> Requires DBCC Proccache
                    collectionPermissionsFlag = true;
                    break;
            }

            /* SQLDM-29432: Eliminate requirement for create db privilege.
            if (productVersion.Major>=11)   //ServerDetails2012  -- Additional permissions for ServerDetails2012
            {
                collectionPermissionsFlag = collectionPermissionsFlag &&
                            collectionPermissions.HasFlag(CollectionPermissions.CREATEDATABASE);

            }
            */
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }

            var metadataPermissionsFlag =
                metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitySysConfigurations)
                // Have write permissions on tempdb database
                && (metadataPermissions.HasFlag(MetadataPermissions.TempDbDbOwnerAccess)
                    // || metadataPermissions.HasFlag(MetadataPermissions.TempDbDataWriter)
                    || collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember));
            // Log failed permissions for Collector
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            return collectionPermissionsFlag;

        }

        /// <summary>
        /// WaitStatistics
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool WaitStatisPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = new bool();
            switch (cloudProviderId)
            {
                case Constants.MicrosoftAzureId:
                    collectionPermissionsFlag =
                        collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN)
                        || collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin);
                    break;
                default:
                    collectionPermissionsFlag = true;
                    break;
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// probe name - StopSessionDetailsTrace2005, MIRRORINGPARTNERACTION and UpdateStatistics
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool ServerActionPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = false;
            var metadataPermissionsFlag = false;

            // Expects two parameters for Server Action - ServerActionConfiguration Type and SqlCommand
            if (permissionArgs != null && permissionArgs.Length >= 2)
            {
                try
                {
                    // Check for Server Action Configuration type
                    var serverActionConfigGetTypeName = permissionArgs[0] as string;

                    // Get the connection Info Object
                    var sqlCommand = permissionArgs[1] as SqlCommand;

                    // To store the dynamic permissions
                    Dictionary<string, int> dynamicPermissions = null;
                    string databaseName = null;
                    int tableId = -1;

                    // Validate Permissions
                    switch (serverActionConfigGetTypeName)
                    {
                        case "UpdateStatisticsConfiguration": // UpdateStatistics.sql
                            // Get database name for dynamic permissions
                            databaseName = permissionArgs[2] as string;

                            // Get the tableId
                            tableId = (int)permissionArgs[3];

                            // Metadata Permissions not found
                            if (!metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitysysobjects))
                            {
                                // Dispose Sql Command and associated connection
                                DisposeSqlCommand(sqlCommand);

                                // Append and log probe error
                                AppendAndLogProbeError(permissionArgs, collectorName, serverActionConfigGetTypeName, null, null, new object[] { databaseName, tableId });
                                return false;
                            }
                            // Stores Required Runtime Permissions
                            dynamicPermissions = new Dictionary<string, int>();

                            // Execute Dynammic Permissions for Server Action Collector - Alter Table permissions on particular table in database
                            PopulateDynamicPermissions("Server Action . " + serverActionConfigGetTypeName, sqlCommand, dynamicPermissions);

                            // Validate Permissions
                            collectionPermissionsFlag = (dynamicPermissions.ContainsKey("UPDATESTATISTICS") && dynamicPermissions["UPDATESTATISTICS"] == 1);

                            if (!collectionPermissionsFlag)
                            {
                                AppendAndLogProbeError(permissionArgs, collectorName, serverActionConfigGetTypeName, null, null, new object[] { databaseName, tableId });
                            }
                            return collectionPermissionsFlag;
                        case "FullTextActionConfiguration":
                            // Get database name for dynamic permissions
                            databaseName = permissionArgs[2] as string;

                            // Stores Required Runtime Permissions for ALTER DATABASE on Particular Database
                            dynamicPermissions = new Dictionary<string, int>();

                            // Execute Dynamic Permissions for Server Action Collector 
                            PopulateDynamicPermissions("Server Action . " + serverActionConfigGetTypeName, sqlCommand, dynamicPermissions);

                            // Validate Permissions
                            collectionPermissionsFlag = (dynamicPermissions.ContainsKey("DbOwner") && dynamicPermissions["DbOwner"] == 1);

                            // Log failed permissions for Collector
                            if (!collectionPermissionsFlag)
                            {
                                // Append and log probe error
                                AppendAndLogProbeError(permissionArgs, collectorName, serverActionConfigGetTypeName, null, null, new object[] { databaseName });
                            }
                            return collectionPermissionsFlag;
                        case "MirroringPartnerActionConfiguration": // MirroringPartnerAction.sql
                            // Get database name for dynamic permissions
                            databaseName = permissionArgs[2] as string;

                            // Stores Required Runtime Permissions for ALTER DATABASE on Particular Database
                            dynamicPermissions = new Dictionary<string, int>();

                            // Execute Dynammic Permissions for Server Action Collector 
                            PopulateDynamicPermissions("Server Action . " + serverActionConfigGetTypeName, sqlCommand, dynamicPermissions);

                            // Validate Permissions
                            collectionPermissionsFlag = (dynamicPermissions.ContainsKey("MIRRORINGPARTNERACTION") && dynamicPermissions["MIRRORINGPARTNERACTION"] == 1);

                            // Log failed permissions for Collector
                            if (!collectionPermissionsFlag)
                            {
                                // Append and log probe error
                                AppendAndLogProbeError(permissionArgs, collectorName, serverActionConfigGetTypeName, null, null, new object[] { databaseName, null });
                            }
                            return collectionPermissionsFlag;
                        case "ReindexConfiguration":
                            // Get database name for dynamic permissions
                            databaseName = permissionArgs[2] as string;

                            // Get the tableId
                            tableId = (int)permissionArgs[3];

                            // Metadata Permissions not found
                            if (!metadataPermissions.HasFlag(MetadataPermissions
                                                                 .MetadataVisibilitysyscolumns |
                                                             MetadataPermissions
                                                                 .MetadataVisibilitymastersysindexcolumns |
                                                             MetadataPermissions
                                                                 .MetadataVisibilitymastersysindexes |
                                                             MetadataPermissions
                                                                 .MetadataVisibilitymastersystypes |
                                                             MetadataPermissions
                                                                 .MetadataVisibilitymastersysxmlindexes |
                                                             MetadataPermissions
                                                                 .MetadataVisibilitymastersysobjects))
                            {
                                // Dispose Sql Command and associated connection
                                DisposeSqlCommand(sqlCommand);

                                // Append and log probe error
                                AppendAndLogProbeError(permissionArgs, collectorName, serverActionConfigGetTypeName, null, null, new object[] { databaseName, tableId });
                                return false;
                            }
                            // Stores Required Runtime Permissions
                            dynamicPermissions = new Dictionary<string, int>();

                            // Execute Dynammic Permissions for Server Action Collector - Alter Table permissions on particular table in database
                            PopulateDynamicPermissions("Server Action . " + serverActionConfigGetTypeName, sqlCommand, dynamicPermissions);

                            // Validate Permissions
                            collectionPermissionsFlag = (dynamicPermissions.ContainsKey("ReindexAlterTable") &&
                                                        (dynamicPermissions["ReindexAlterTable"] == 1) &&
                                                        dynamicPermissions.ContainsKey("ReindexShowContig") &&
                                            (dynamicPermissions["ReindexShowContig"] == 1 ||
                                            collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember) ||
                                            collectionPermissions.HasFlag(CollectionPermissions.DBDDLADMINAccess) ||
                                            collectionPermissions.HasFlag(CollectionPermissions.DBOWNERAccess)) &&
                                                        dynamicPermissions.ContainsKey("ReindexControlPermissions") &&
                                                        (dynamicPermissions["ReindexControlPermissions"] == 1)
                            );

                            if (!collectionPermissionsFlag)
                            {
                                AppendAndLogProbeError(permissionArgs, collectorName, serverActionConfigGetTypeName, null, null, new object[] { databaseName, tableId });
                            }
                            return collectionPermissionsFlag;
                        //Configuration2012 
                        case "BlockedProcessThresholdConfiguration":
                        case "ReconfigurationConfiguration":
                            // Validate Permissions
                            switch (cloudProviderId)
                            {
                                case Constants.MicrosoftAzureId:
                                    collectionPermissionsFlag =
                                        (collectionPermissions.HasFlag(CollectionPermissions.CONTROLSERVER) ||
                                         collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember))
                                        && (collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN) ||
                                            collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin));
                                    break;
                                default:
                                    collectionPermissionsFlag =
                                        (collectionPermissions.HasFlag(CollectionPermissions.CONTROLSERVER) ||
                                        collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember));
                                    break;
                            }

                            if (!collectionPermissionsFlag)
                            {
                                AppendAndLogProbeError(permissionArgs, collectorName);
                                return false;
                            }
                            // Check Public Member Permissions
                            metadataPermissionsFlag = metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitySysConfigurations | MetadataPermissions.MetadataVisibilityXpMsver);

                            if (!metadataPermissionsFlag)
                            {
                                AppendAndLogProbeError(permissionArgs, collectorName);
                            }
                            return metadataPermissionsFlag;
                        case "FreeProcedureCacheConfiguration":
                            collectionPermissionsFlag =
                                collectionPermissions.HasFlag(CollectionPermissions.ALTERSERVERSTATE);
                            if (!collectionPermissionsFlag)
                            {
                                AppendAndLogProbeError(permissionArgs, collectorName);
                            }
                            return collectionPermissionsFlag;
                        case "RecycleLogConfiguration":
                            collectionPermissionsFlag =
                                collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember)
                                || collectionPermissions.HasFlag(CollectionPermissions.DBOWNERAccess);
                            if (!collectionPermissionsFlag)
                            {
                                AppendAndLogProbeError(permissionArgs, collectorName);
                            }
                            return collectionPermissionsFlag;
                        case "KillSessionConfiguration":
                        case "ServiceControlConfiguration":
                        case "JobControlConfiguration":
                        case "ShutdownSQLServerConfiguration":
                        case "SetNumberOfLogsConfiguration":
                        case "RecycleAgentLogConfiguration":
                            collectionPermissionsFlag =
                                collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember);
                            if (!collectionPermissionsFlag)
                            {
                                AppendAndLogProbeError(permissionArgs, collectorName);
                            }
                            return collectionPermissionsFlag;
                        case "StopActivityMonitorTraceConfiguration":
                        case "StartQueryMonitorTraceConfiguration":
                        case "StopQueryMonitorTraceConfiguration":
                        case "StopSessionDetailsTraceConfiguration": //StopSessionDetailsTrace2005
                            collectionPermissionsFlag =
                                collectionPermissions.HasFlag(CollectionPermissions.ALTERTRACE);
                            if (!collectionPermissionsFlag)
                            {
                                // Append and log probe error
                                AppendAndLogProbeError(permissionArgs, collectorName);

                                return false;
                            }
                            metadataPermissionsFlag =
                                metadataPermissions.HasFlag(
                                    MetadataPermissions.MetadataVisibilitymastersysobjects |
                                    MetadataPermissions.MetadataVisibilitymastersystraces);
                            if (!metadataPermissionsFlag)
                            {
                                // Append and log probe error
                                AppendAndLogProbeError(permissionArgs, collectorName);
                                return false;
                            }
                            return true;
                    }
                }
                catch (Exception exception)
                {
                    LOG.Error("Failed to parse Server Action permission Args : " + exception);
                }
            }
            // Pass for new cases
            return true;
        }

        /// <summary>
        /// OLDESTOPENTRANSACTION2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool OldestOpenTransactionPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            bool collectionPermissionsFlag = false;
            if (productVersion.Major >= 9)
            {
                switch (cloudProviderId)
                {
                    case Constants.MicrosoftAzureId:
                        collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                        break;
                    default:
                        collectionPermissionsFlag = true;
                        break;
                }
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// OutOfDateStats2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool OutOfDateStatsPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            bool metaPermissionsFlag = false;
            if (productVersion.Major >= 9)
            {
                metaPermissionsFlag = metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersysobjects |
                    MetadataPermissions.MetadataVisibilitymastersysindexes | MetadataPermissions.MetadataVisibilitySysSchemas);
            }
            // Log failed permissions for Collector
            if (!metaPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return metaPermissionsFlag;
        }

        /// <summary>
        /// Job Alerts
        /// Probe name  -LONGJOBSWITHSTEPS2005, LONGJOBSBYRUNTIME, LONGJOBSBYPERCENT, LONGJOBS2005, CompletedJobs, FAILEDJOBS2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool JobAlertsPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            bool collectionPermissionsFlag = true;

            if (permissionArgs != null && permissionArgs.Length >= 2)
            {
                // try parsing permissionArgs
                try
                {
                    var alertsOnSteps = (bool)permissionArgs[0];
                    var completedJob = (bool)permissionArgs[1];
                    var bombedJob = (bool)permissionArgs[2];
                    var longJobs = (bool)permissionArgs[3];

                    if (bombedJob) //FAILEDJOBS2005
                    {
                        collectionPermissionsFlag =
                            collectionPermissions.HasFlag(CollectionPermissions.MSDBAccessSYSCATEGORIES
                                                          | CollectionPermissions
                                                              .MSDBAccessSYSJOBHISTORY
                                                          | CollectionPermissions.MSDBAccessSYSJOBS
                                                          | CollectionPermissions
                                                              .MSDBAccessSYSJOBSTEPS);
                    }
                    if (longJobs)
                    {
                        // check if Alert On Steps is present - for LONGJOBSWITHSTEPS2005
                        // for LONGJOBSBYRUNTIME , LONGJOBSBYPERCENT and LONGJOBS2005 
                        collectionPermissionsFlag =
                            collectionPermissionsFlag && collectionPermissions.HasFlag(
                                CollectionPermissions.MSDBAccessSYSCATEGORIES
                                | CollectionPermissions.MSDBAccessSYSJOBHISTORY
                                | CollectionPermissions.MSDBAccessSYSJOBS
                                | CollectionPermissions.EXECUTEMASTERXPSQLAGENTENUMJOBS);
                    }

                    if (completedJob) // for CompletedJobs
                    {
                        collectionPermissionsFlag = collectionPermissionsFlag &&
                        collectionPermissions.HasFlag(
                            CollectionPermissions.MSDBAccessSYSCATEGORIES |
                            CollectionPermissions.MSDBAccessSYSJOBHISTORY |
                            CollectionPermissions.MSDBAccessSYSJOBS
                            | CollectionPermissions.MSDBAccessSYSJOBSTEPS);
                    }

                    // Log failed permissions for Collector
                    if (!collectionPermissionsFlag)
                    {
                        // Append and log probe error
                        AppendAndLogProbeError(permissionArgs, collectorName);
                    }
                    return collectionPermissionsFlag;
                }
                catch (Exception exception)
                {
                    LOG.Error("Failed to parse Job Alerts permission Args : " + exception);
                }
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// Probe name - COLUMNSTOREINDEX2016
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool ColumnStoreIndexPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = false;
            switch (cloudProviderId)
            {
                case Constants.MicrosoftAzureId:
                    collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                    break;
                default:
                    collectionPermissionsFlag = true;
                    break;
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }

            var metadataPermissionsFlag =
                metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitySysPartitions);

            // Log failed permissions for Collector
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }

            return collectionPermissionsFlag;
        }

        /// <summary>
        /// Probe name - GETMASTERFILES2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool DatabaseFileInfoPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.CREATEDATABASE) || collectionPermissions.HasFlag(CollectionPermissions.ALTERANYDATABASE);
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// Probe name - FULLTEXTSEARCHSERVICE2005  - Vamshi/Varun - We Need Revisit this
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool FullTextSearchServicePermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember);

            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// Probe name - FULLTEXTCOLUMNS
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool FullTextColumnsPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var metadataPermissionsFlag =
               metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitysyscolumns | MetadataPermissions.MetadataVisibilitysystypes);

            // Log failed permissions for Collector
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }

            return metadataPermissionsFlag;
        }

        /// <summary>
        /// Probe name - FULLTEXTTABLES
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool FullTextTablesPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var metadataPermissionsFlag =
               metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitysysobjects | MetadataPermissions.MetadataVisibilitysysfulltextcatalogs |
               MetadataPermissions.MetadataVisibilitysysindexes);
            // Log failed permissions for Collector
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return metadataPermissionsFlag;
        }

        /// <summary>
        /// Probe name - LARGETABLESTATS2008
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool LargeTableStatsPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember |
               CollectionPermissions.DBDDLADMINAccess | CollectionPermissions.DBOWNERAccess);
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);

                return false;
            }
            var metadataPermissionsFlag =
               metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersysdmdbstatsproperties | MetadataPermissions.MetadataVisibilitymastersysstats);
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);

                return false;
            }
            return true;
        }

        /// <summary>
        /// Probe name - INDEXSTATISTICS
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool IndexStatisticsPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember |
                CollectionPermissions.DBDDLADMINAccess | CollectionPermissions.DBOWNERAccess);
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);

                return false;
            }
            var metadataPermissionsFlag =
                metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitysysobjects);
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);

                return false;
            }
            return true;
        }

        /// <summary>
        /// Probe name - LOG SCAN 2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool LogScanPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.SECURITYADMINMEMBER);
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// Probe name - LOG SCAN 2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool LogScanvariablestorePermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.SECURITYADMINMEMBER);
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// Probe name - LOGLIST2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool LogListPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag =
                    collectionPermissions.HasFlag(
                        CollectionPermissions.EXECUTEMASTERDBOXPINSTANCEREGREAD
                        | CollectionPermissions.EXECUTEMASTERXPREADERRORLOG);

            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// probe Name- MIRRORMETRICSUPDATE2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool MirrorMetricsUpdateCallbackPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.ALTERANYDATABASE);
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// probe Name- MIRRORMONITORINGHISTORY2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool MirrorMonitoringHistoryCallbackPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.DBMMONITORAccess);
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// CUSTOMCOUNTEROS - Vamshi/Varun - We Need Revisit this - WMI Change
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool CustomCounterOSPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember);
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// DATABASEFILES2005, TEMPDBSUMMARY2005 - Vamshi/Varun - We Need Revisit this - Code related to master.sys.dm_tran_locks
        /// DiskSize2005 - Permissions already handled in DATABASEFILES2005 permissions
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool DatabaseFilesPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = new bool();
            //For both DATABASEFILES2005, TEMPDBSUMMARY2005
            switch (cloudProviderId)
            {
                case Constants.MicrosoftAzureId:
                    collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                    break;
                default:
                    collectionPermissionsFlag = true;
                    break;
            }
            ////TEMPDBSUMMARY2005
            //if(productVersion.Major >= 9 && permissionArgs != null && permissionArgs.Length > 0 && Convert.ToString(permissionArgs[0]) == "tempdb")
            //{                                                               
            //    // Log failed permissions for Collector
            //    if (!collectionPermissionsFlag)
            //    {
            //        // Append and log probe error
            //        AppendAndLogProbeError(permissionArgs, collectorName);
            //    }
            //    return collectionPermissionsFlag;
            //}

            ////DATABASEFILES2005
            //collectionPermissionsFlag = collectionPermissionsFlag && collectionPermissions.HasFlag(CollectionPermissions.ALTERSERVERSTATE)
            //    && (collectionPermissions.HasFlag(CollectionPermissions.ALTERANYDATABASE) || collectionPermissions.HasFlag(CollectionPermissions.CREATEDATABASE));

            //if (!collectionPermissionsFlag)
            //{
            //    // Append and log probe error
            //    AppendAndLogProbeError(permissionArgs, collectorName);

            //    return false;
            //}

            var metadataPermissionsFlag = metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitysysfilegroups | MetadataPermissions.MetadataVisibilitysysfiles | MetadataPermissions.MetadataVisibilityTempdbSysDatabaseFiles);

            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);

                return false;
            }
            return true;
        }

        /// <summary>
        /// DATABASESUMMARY2012, DiskSize2005  -- Have to check for false negative cases
        /// NEED TO REVISIT - When its not able to connect to WMI, it will use DiskSize2005 permissions
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool DatabaseSummaryPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = new bool();

            // DiskSize2005
            var isWMICallFailed = (bool)permissionArgs[0];
            var includeSummaryData = (bool)permissionArgs[1];
            if (isWMICallFailed)
            {
                //collectionPermissionsFlag = true;//collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember);

                //if (!collectionPermissionsFlag)
                //{
                //    // Append and log probe error
                //    AppendAndLogProbeError(permissionArgs, collectorName);

                //    return false;
                //}
                return true;
            }
            // if its not DiskSize2005 then go to DATABASESUMMARY2012
            //Reqd for DATABASESUMMARY2012

            switch (cloudProviderId)
            {
                case Constants.MicrosoftAzureId:
                    collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                    break;
                default:
                    collectionPermissionsFlag = true;
                    break;
            }

            // Corrected the Condition
            collectionPermissionsFlag = collectionPermissionsFlag
                && collectionPermissions.HasFlag(/*CollectionPermissions.ALTERSERVERSTATE |*/ CollectionPermissions.MSDBAccessBACKUPSET);

            //if (includeSummaryData) Added condition around DBCC Commands
            //{
            //    collectionPermissionsFlag =
            //        collectionPermissionsFlag &&
            //        collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember);
            //}

            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            // Additionallly reqd for DATABASESUMMARY2012
            var metadataPermissionsFlag =
                metadataPermissions.HasFlag(
                    MetadataPermissions.MetadataVisibilitysysfilegroups | //sysfilegroups
                    MetadataPermissions.MetadataVisibilitysysfiles | //sysfiles
                    MetadataPermissions.MetadataVisibilitymastersysallobjects | //sysobjects
                    MetadataPermissions.MetadataVisibilitymastersysinternaltables | //sys.internal_tables
                    MetadataPermissions.MetadataVisibilitytempdbsysobjects | //tempdb.sys.database_files
                    MetadataPermissions.MetadataVisibilitySysAllocationUnits |//sys.allocation_units
                    MetadataPermissions.MetadataVisibilitySysPartitions | //sys.partitions
                    MetadataPermissions.MetadataVisibilityTempdbSysDatabaseFiles);

            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// DBSecurity2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool DBSecurityPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag =
                collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember);

            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }

            var metadataPermissionsFlag =
                metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitysysusers |
                                            MetadataPermissions.MetadataVisibilitymastersyssymmetrickeys);
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// DISKDRIVES2005, DiskSize2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool DiskDrivesPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            bool collectionPermissionsFlag = false;

            if (productVersion.Major >= 9) //DISKDRIVES2005
            {
                collectionPermissionsFlag = true;//collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember);
            }
            collectionPermissionsFlag = collectionPermissionsFlag &&
                collectionPermissions.HasFlag(CollectionPermissions.CREATEDATABASE |
                CollectionPermissions.ALTERANYDATABASE);

            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            var metadataPermissionsFlag =
               metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitySysConfigurations);

            // Log failed permissions for Collector
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// DISTRIBUTORQUEUE - Vamshi/Varun - We Need Revisit this - Distribution DB related
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool DistributorQueuePermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag =
                (collectionPermissions.HasFlag(CollectionPermissions.ALTERANYLINKEDSERVER) || collectionPermissions.HasFlag(CollectionPermissions.ALTERANYLOGIN)) &&
                (collectionPermissions.HasFlag(CollectionPermissions.SETUPADMINMember) || collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember));
            //Need to include Access to Distribution database: MSdistribution_history, MSpublications, MSrepl_commands, MSrepl_transactions, MSsubscriptions

            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// File Activity
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool FileActivityPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = true; // collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember);    If this becomes a problem in a future release, we should probably add a different kind of check

            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// Vamshi/Varun - We Need Revisit this - This has become a combination of 3 probes. Please review this once.
        /// FileActivity2005, WAITSTATISTICS, SERVEROVERVIEW2012, LockCounterStatistics2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool ServerOverviewPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = collectionPermissions.HasFlag(
                /* SQLDM-29432:  Remove create db privilege check.  CollectionPermissions.CREATEDATABASE
                //SERVEROVERVIEW2012
                | */ CollectionPermissions.EXECUTEMASTERDBOXPREGREAD)
                                            && (collectionPermissions.HasFlag(
                                                CollectionPermissions.CONTROLSERVER)
                                                || collectionPermissions.HasFlag(
                                                    CollectionPermissions
                                                        .EXECUTEMASTERXPLOGINCONFIG));
            //WAITSTATISTICS, SERVEROVERVIEW2012,LockCounterStatistics2005
            if (productVersion.Major > 8)
            {
                switch (cloudProviderId)
                {
                    case Constants.MicrosoftAzureId:
                        collectionPermissionsFlag = collectionPermissionsFlag && (collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN));
                        break;
                }
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            var metadataPermissionsFlag = metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilityXpMsver | MetadataPermissions.MetadataVisibilityTempdbSysDatabaseFiles);
            // Log failed permissions for Collector
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// FILEGROUP2005, DiskSize2005
        /// DiskSize2005 requires "Member of SYSADMIN role", and "CREATE DATABASE, ALTER ANY DATABASE, or VIEW ANY DEFINITION"
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool FileGroupPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.CREATEDATABASE) || collectionPermissions.HasFlag(CollectionPermissions.ALTERANYDATABASE);

            switch (cloudProviderId)
            {
                case Constants.MicrosoftAzureId:
                    collectionPermissionsFlag = collectionPermissionsFlag && (collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN));
                    break;
            }
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            var metadataPermissionsFlag =
                metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitysysfilegroups) | //sysfilegroups
                metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitysysfiles); //sysfiles

            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// FILTEREDINDEX2008
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool FilteredColumnNotInKeyOfFilteredIndexPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var metadataPermissionsFlag =
                metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitysysindexes) | //sys.indexes
                metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersyscolumns) | //sys.columns
                metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersysindexcolumns); //sys.index_columns
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);

                return false;
            }
            return true;
        }

        /// <summary>
        /// probe Name- MIRRORMONITORINGREALTIME2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool MirrorMonitoringRealtimeCallbackPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = (collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember) || collectionPermissions.HasFlag(CollectionPermissions.DBMMONITORAccess)) &&
                (collectionPermissions.HasFlag(CollectionPermissions.ALTERANYDATABASE) || collectionPermissions.HasFlag(CollectionPermissions.CREATEDATABASE));
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// probe name - INDEXCONTENTION2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool IndexContentionPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var metadataPermissionsFlag =
                 metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitysysobjects |
                 MetadataPermissions.MetadataVisibilitymastersysindexes | MetadataPermissions.MetadataVisibilitySysSchemas);

            // Log failed permissions for Collector
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return metadataPermissionsFlag;
        }

        /// <summary>
        /// probe name - NONINCREMENTALCOLUMNSTATONPARTITIONEDTABLE2014
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool NonIncrementalColumnStatsCallbackPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var metadataPermissionsFlag =
                 metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersysstatscolumns |
                 MetadataPermissions.MetadataVisibilitymastersysstats | MetadataPermissions.MetadataVisibilitymastersyscolumns |
                 MetadataPermissions.MetadataVisibilitymastersysindexes | MetadataPermissions.MetadataVisibilitySysPartitions);

            // Log failed permissions for Collector
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return metadataPermissionsFlag;
        }

        /// <summary>
        /// probe Name- QueryMonitor2012EX, QueryMonitor2005, QUERYMONITOREXTENDEDEVENTSBATCHES2012
        /// QueryMonitorExtendedEventsSingleStmt2012, QueryMonitorExtendedEventsSP2012,
        /// QueryMonitorTraceBatches,QueryMonitorTraceSingleStmt, QueryMonitorTraceSP
        /// QUERYMONITORSTOPEX,QUERYMONITORSTOP2005,QueryMonitorRestart2005,QueryMonitorRestart2012EX,
        /// QueryMonitorRead2005,QueryMonitorRead2012EX
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool QueryMonitorPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            bool collectionPermissionsFlag = false;
            if (permissionArgs != null && permissionArgs.Length >= 6)
            {
                // try parsing permissionArgs
                try
                {
                    bool TraceMonitoringEnabled = (bool)permissionArgs[0];
                    bool SqlBatchEventsEnabled = (bool)permissionArgs[1];
                    bool SqlStatementEventsEnabled = (bool)permissionArgs[2];
                    bool StoredProcedureEventsEnabled = (bool)permissionArgs[3];
                    bool IsAlertResponseQueryTrace = (bool)permissionArgs[4];
                    DateTime? StopTimeUTC = (DateTime?)permissionArgs[5];
                    DateTime? timeStamp = (DateTime?)permissionArgs[6];
                    // SQLdm 10.4 (Varun Chopra) Query Monitor using Query Store
                    bool queryStoreMonitoringEnabled = (bool)permissionArgs[7];

                    // Collection Permissions
                    collectionPermissionsFlag =
                        metadataPermissions.HasFlag(MetadataPermissions.TempDbDataWriter)
                        || metadataPermissions.HasFlag(MetadataPermissions.TempDbDbOwnerAccess)
                        || collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember);

                    if (TraceMonitoringEnabled)
                    {
                        //case - check for QueryMonitor2005,QUERYMONITORSTOP2005,QueryMonitorTraceBatches,
                        //QueryMonitorTraceSingleStmt, QueryMonitorTraceSP,QueryMonitorRestart2005,QueryMonitorRead2005
                        collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.ALTERTRACE);
                    }
                    else if (!queryStoreMonitoringEnabled)
                    //case - check for QUERYMONITORSTOPEX,QUERYMONITOREXTENDEDEVENTSBATCHES2012,
                    //QueryMonitorExtendedEventsSingleStmt2012, QueryMonitorExtendedEventsSP2012,QueryMonitorRestart2012EX,
                    {
                        if (StoredProcedureEventsEnabled || SqlStatementEventsEnabled
                            || (SqlBatchEventsEnabled && productVersion.Major > 10)
                            || (IsAlertResponseQueryTrace && StopTimeUTC.HasValue && timeStamp.HasValue
                                && StopTimeUTC <= timeStamp))
                        {
                            collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.ALTERANYEVENTSESSION);
                            // Log failed permissions for Collector
                            if (!collectionPermissionsFlag)
                            {
                                // Append and log probe error
                                AppendAndLogProbeError(permissionArgs, collectorName);
                            }
                            return collectionPermissionsFlag;
                        }
                        //case for QueryMonitorRestart2012EX
                        else if (!IsAlertResponseQueryTrace && StopTimeUTC.HasValue && StopTimeUTC <= timeStamp.Value.AddHours(1))
                        {
                            switch (cloudProviderId)
                            {
                                case Constants.MicrosoftAzureId:
                                    collectionPermissionsFlag =
                                        collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) ||
                                        collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                                    break;
                                default:
                                    collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.ALTERANYEVENTSESSION);
                                    break;
                            }
                            // Log failed permissions for Collector
                            if (!collectionPermissionsFlag)
                            {
                                // Append and log probe error
                                AppendAndLogProbeError(permissionArgs, collectorName);
                            }
                            return collectionPermissionsFlag;
                        }
                        //case for QueryMonitor2012EX and QueryMonitorRead2012EX
                        //Since both are being called unconditionally under BuildQueryMonitorCommandTextEX method in SqlCommandBuilder.cs
                        // so permissions need to be clubbed
                        else
                        {
                            collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.ALTERANYEVENTSESSION);
                            // Log failed permissions for Collector
                            if (!collectionPermissionsFlag)
                            {
                                // Append and log probe error
                                AppendAndLogProbeError(permissionArgs, collectorName);
                            }
                            return collectionPermissionsFlag;
                        }
                    }
                    else // queryStoreMonitoringEnabled
                    {
                        // Minimum permission set you need to have in order to use Query Store
                        collectionPermissionsFlag = metadataPermissions.HasFlag(MetadataPermissions.TempDbDbOwnerAccess);
                        // Note: We are not configuring here so db owner not required
                        // Note: We are not configuring here so db owner not required
                    }
                    // Log failed permissions for Collector
                    if (!collectionPermissionsFlag)
                    {
                        // Append and log probe error
                        AppendAndLogProbeError(permissionArgs, collectorName);
                    }
                    return collectionPermissionsFlag;
                }
                catch (Exception exception)
                {
                    LOG.Error("Failed to parse permission Args : " + exception);
                }
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// probe Name- Services2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool ServicesPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = true;
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// probe Name- SessionDetailsTrace2005, SessionDetails2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool SessionDetailsPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            bool collectionPermissionsFlag = false;
            // check if trace is on for SessionDetailsTrace2005
            if (permissionArgs != null && permissionArgs.Length >= 1)
            {
                // try parsing permissionArgs
                try
                {
                    var traceOn = (bool)permissionArgs[0];
                    if (traceOn)    // Condition for Alter Trace Permissions
                    {
                        collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.ALTERTRACE);
                        // Log failed permissions for Collector
                        if (!collectionPermissionsFlag)
                        {
                            // Append and log probe error
                            AppendAndLogProbeError(permissionArgs, collectorName);
                        }
                        return collectionPermissionsFlag;
                    }

                    var metadataPermissionsFlag = metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitysysobjects |
                                                                              MetadataPermissions.MetadataVisibilitymastersystraces);
                    if (!metadataPermissionsFlag)
                    {
                        // Append and log probe error
                        AppendAndLogProbeError(permissionArgs, collectorName);
                        return false;
                    }
                    return true;
                }
                catch (Exception exception)
                {
                    LOG.Error("Failed to parse Session Details permission Args : " + exception);

                    // Log failed permissions for Collector
                    AppendAndLogProbeError(permissionArgs, collectorName, null, null, null, null, exception);
                    return false;
                }
            }
            else   // for SessionDetails2005
            {
                if (productVersion.IsGreaterThanSql2014sp2())
                {
                    switch (cloudProviderId)
                    {
                        case Constants.MicrosoftAzureId:
                            collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                            break;
                        default://In case of non-cloud instance only minimum permissions are sufficient which are already verified.
                            collectionPermissionsFlag = true;
                            break;
                    }
                }
                else
                {
                    switch (cloudProviderId)
                    {
                        case Constants.MicrosoftAzureId:
                            collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                            break;
                        default:
                            collectionPermissionsFlag = true;
                            break;
                    }
                }

                // Log failed permissions for Collector
                if (!collectionPermissionsFlag)
                {
                    // Append and log probe error
                    AppendAndLogProbeError(permissionArgs, collectorName);
                }
                return collectionPermissionsFlag;
            }

        }

        /// <summary>
        /// probe Name- Sessions2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool SessionListPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = false;
            switch (cloudProviderId)
            {
                case Constants.MicrosoftAzureId:
                    collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                    break;
                default:
                    // SQLDM-29432:  remove requirement of CREATE DB.  collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.CREATEDATABASE);
                    collectionPermissionsFlag = true;
                    break;
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// probe Name- GetLockedPageKB2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool LockedPageKBPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = false;
            switch (cloudProviderId)
            {
                case Constants.MicrosoftAzureId:
                    collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin);
                    break;
                default:
                    collectionPermissionsFlag = true;
                    break;
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// OVERLAPPINGINDEXES2005 & OVERLAPPINGINDEXES2008
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool OverlappingIndexesPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            bool collectionPermissionsFlag = false;
            if (productVersion.Major >= 10)   //OVERLAPPINGINDEXES2008
            {
                switch (cloudProviderId)
                {
                    case Constants.MicrosoftAzureId:
                        collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                        break;
                    default:
                        collectionPermissionsFlag = true;
                        break;
                }

                if (!collectionPermissionsFlag)
                {
                    // Append and log probe error
                    AppendAndLogProbeError(permissionArgs, collectorName);
                    return false;
                }

                var metadataPermissionsFlag = metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersysforeignkeys |
                        MetadataPermissions.MetadataVisibilitymastersysindexcolumns |
                        MetadataPermissions.MetadataVisibilitymastersysindexes |
                        MetadataPermissions.MetadataVisibilitymastersysobjects | MetadataPermissions.MetadataVisibilitySysSchemas);

                if (!metadataPermissionsFlag)
                {
                    // Append and log probe error
                    AppendAndLogProbeError(permissionArgs, collectorName);

                    return false;
                }
                return true;
            }
            else if (productVersion.Major >= 9) //OVERLAPPINGINDEXES2005
            {
                switch (cloudProviderId)
                {
                    case Constants.MicrosoftAzureId:
                        collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                        break;
                    default:
                        collectionPermissionsFlag = true;
                        break;
                }

                if (!collectionPermissionsFlag)
                {
                    // Append and log probe error
                    AppendAndLogProbeError(permissionArgs, collectorName);
                    return false;
                }

                var metaPermissionsFlag = metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersysforeignkeys |
                        MetadataPermissions.MetadataVisibilitymastersysindexcolumns |
                        MetadataPermissions.MetadataVisibilitymastersysindexes |
                        MetadataPermissions.MetadataVisibilitymastersysobjects);

                if (!metaPermissionsFlag)
                {
                    // Append and log probe error
                    AppendAndLogProbeError(permissionArgs, collectorName);
                    return false;
                }
                return true;
            }
            // Append and log probe error
            AppendAndLogProbeError(permissionArgs, collectorName);
            return false;
        }

        /// <summary>
        /// QueryPlanEstRows2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool QueryPlanEstRowsPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = new bool();
            if (productVersion.Major >= 9)
            {

                switch (cloudProviderId)
                {
                    case Constants.MicrosoftAzureId:
                        collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                        break;
                    default:
                        collectionPermissionsFlag = true;
                        break;
                }

            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// QueryStore2016
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool QueryStorePermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            if (productVersion.Major >= 13) //OVERLAPPINGINDEXES2005
            {
                var metaPermissionsFlag = metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersysplanguides);

                if (!metaPermissionsFlag)
                {
                    // Append and log probe error
                    AppendAndLogProbeError(permissionArgs, collectorName);

                    return false;
                }
                return true;
            }
            // Append and log probe error
            AppendAndLogProbeError(permissionArgs, collectorName);

            return false;
        }

        /// <summary>
        /// RARELYUSEDINDEXONINMEMORYTABLE2014
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool RarelyUsedIndexOnInMemoryTablePermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            if (productVersion.Major >= 12)
            {
                var metaPermissionsFlag = metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersysindexes);

                if (!metaPermissionsFlag)
                {
                    // Append and log probe error
                    AppendAndLogProbeError(permissionArgs, collectorName);
                    return false;
                }
                return true;
            }
            // Append and log probe error
            AppendAndLogProbeError(permissionArgs, collectorName);

            return false;
        }

        /// <summary>
        /// REPLICATIONDISTRIBUTORDETAILS2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool DistributorDetailsPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = (collectionPermissions.HasFlag(CollectionPermissions.ALTERANYLINKEDSERVER) || collectionPermissions.HasFlag(CollectionPermissions.ALTERANYLOGIN)) &&
                                            (collectionPermissions.HasFlag(CollectionPermissions.SETUPADMINMember) || collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember));

            // Handling of Distributor Permissions if required
            if (permissionArgs != null && permissionArgs.Length >= 1 && permissionArgs[0] != null &&
                permissionArgs[0] is SqlCommand)
            {
                var sqlCommand = (SqlCommand)permissionArgs[0];
                if (collectionPermissionsFlag)
                {
                    // Stores Required Runtime Permissions
                    var dynamicPermissions = new Dictionary<string, int>();

                    // Execute Dynammic Permissions for Server Action Collector 
                    PopulateDynamicPermissions(collectorName, sqlCommand, dynamicPermissions);

                    // Validate Permissions
                    collectionPermissionsFlag =
                        dynamicPermissions.ContainsKey("ReplicationDistributionCheck") &&
                        dynamicPermissions["ReplicationDistributionCheck"] == 1;
                }
                else
                {
                    DisposeSqlCommand(sqlCommand);
                }
            }

            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// SAMPLESERVERRESOURCES2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool SampleServerResourcesPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            bool collectionPermissionsFlag = false;
            switch (cloudProviderId)
            {
                case Constants.MicrosoftAzureId:
                    collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                    break;
                default:
                    collectionPermissionsFlag = true;
                    break;
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// TableGrowth2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool TableGrowthPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            bool collectionPermissionsFlag = false;
            if (productVersion.Major >= 9)
            {
                switch (cloudProviderId)
                {
                    case Constants.MicrosoftAzureId:
                        collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                        break;
                    default:
                        collectionPermissionsFlag = true;
                        break;
                }

                if (!collectionPermissionsFlag)
                {
                    // Append and log probe error
                    AppendAndLogProbeError(permissionArgs, collectorName);
                    return false;
                }
                var metaPermissionsFlag = metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersysobjects |
                                                                        MetadataPermissions.MetadataVisibilitymastersysinternaltables |
                                                                        MetadataPermissions.MetadataVisibilitymastersysindexes |
                                                                        MetadataPermissions.MetadataVisibilitySysAllocationUnits |
                                                                        MetadataPermissions.MetadataVisibilitySysPartitions |
                                                                        MetadataPermissions.MetadataVisibilitySysSchemas);

                if (!metaPermissionsFlag)
                {
                    // Append and log probe error
                    AppendAndLogProbeError(permissionArgs, collectorName);
                    return false;
                }
                return true;
            }
            // Append and log probe error
            AppendAndLogProbeError(permissionArgs, collectorName);
            return false;
        }

        /// <summary>
        /// TableSummary2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool TableSummaryPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            bool collectionPermissionsFlag = false;
            if (productVersion.Major >= 9)
            {
                collectionPermissionsFlag = true; // collectionPermissions.HasFlag(CollectionPermissions.CREATEDATABASE);     If this becomes a problem in a future release, we should probably add a different kind of check
                if (!collectionPermissionsFlag)
                {
                    // Append and log probe error
                    AppendAndLogProbeError(permissionArgs, collectorName);
                    return false;
                }
                var metaPermissionsFlag = metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitymastersysfulltextcatalogs |
                                                                        MetadataPermissions.MetadataVisibilitymastersysinternaltables |
                                                                        MetadataPermissions.MetadataVisibilitymastersysindexes |
                                                                        MetadataPermissions.MetadataVisibilitymastersysobjects |
                                                                        MetadataPermissions.MetadataVisibilitySysAllocationUnits |
                                                                        MetadataPermissions.MetadataVisibilitySysPartitions |
                                                                        MetadataPermissions.MetadataVisibilitySysSchemas |
                                                                        MetadataPermissions.MetadataSysDataSpaces);

                if (!metaPermissionsFlag)
                {
                    // Append and log probe error
                    AppendAndLogProbeError(permissionArgs, collectorName);
                    return false;
                }
                return true;
            }
            // Append and log probe error
            AppendAndLogProbeError(permissionArgs, collectorName);
            return false;
        }

        /// <summary>
        /// SESSIONSUMMARY2005, LockCounterStatistics2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool SessionSummaryPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            bool collectionPermissionsFlag = false;
            if (productVersion.Major >= 9)
            {
                switch (cloudProviderId)
                {
                    case Constants.MicrosoftAzureId:
                        collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                        break;
                    default:
                        collectionPermissionsFlag = true;
                        break;
                }
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// ReplicationStatus
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool ReplicationStatusPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag =
                collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember);
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// SQLMODULEOPTIONS2005
        /// NOTE - Requires membership in the public role.
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool SQLModuleOptionsPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var metadataPermissionsFlag =
                metadataPermissions.HasFlag(
                    MetadataPermissions.MetadataVisibilitymastersysallobjects //sys.all_objects
                    | MetadataPermissions.MetadataVisibilitymastersysallsqlmodules //sys.all_sql_module
                    | MetadataPermissions.MetadataVisibilitysysobjects //sys.objects
                    | MetadataPermissions.MetadataVisibilitySysSchemas //sys.schemas
                );
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// WAITINGBATCHES2005 - Vamshi/Varun - We Need Revisit this
        /// "sys.dm_exec_requests", "sys.dm_exec_sessions","sys.dm_exec_sql_text", "sys.dm_os_waiting_tasks" Should these be separately handled? Did not find these anywhere
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool WaitingBatchesPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = new bool();
            switch (cloudProviderId)
            {
                case Constants.MicrosoftAzureId:
                    collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                    break;
                default:
                    collectionPermissionsFlag = true;
                    break;
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// NUMANodeCounters2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool NUMANodeCountersPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = new bool();
            switch (cloudProviderId)
            {
                case Constants.MicrosoftAzureId:
                    collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                    break;
                default:
                    collectionPermissionsFlag = true;
                    break;
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// Table Details
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool TableDetailsPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            bool metaPermissionsFlag = false;
            if (productVersion.Major >= 9)
            {
                metaPermissionsFlag = metadataPermissions.HasFlag(
                    MetadataPermissions.MetadataVisibilitysysdepends //sysdepends
                    | MetadataPermissions.MetadataVisibilitysysindexes //sysindexes
                    | MetadataPermissions.MetadataVisibilitysysobjects //sysobjects
                    | MetadataPermissions.MetadataVisibilitysysreferences //sysreferences
                    );
            }
            // Log failed permissions for Collector
            if (!metaPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return metaPermissionsFlag;
        }

        /// <summary>
        /// ALWAYSONTOPOLOGY2012
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool AlwaysOnMetricsPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.ALTERANYDATABASE) || collectionPermissions.HasFlag(CollectionPermissions.CREATEDATABASE);

            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);

                return false;
            }

            var metadataPermissionsFlag = metadataPermissions.HasFlag(
                MetadataPermissions.MetadataVisibilitymastersysavailabilitygrouplisteneripaddresses //master.sys.availability_group_listener_ip_addresses
                | MetadataPermissions.MetadataVisibilitymastersysavailabilitygrouplisteners //master.sys.availability_group_listeners
                );

            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);

                return false;
            }
            return true;
        }

        /// <summary>
        /// BackupAndRecovery2005 - No need to change
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool BackupAndRecoveryPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag =
                collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember |
                                              CollectionPermissions.MSDBAccessBACKUPMEDIAFAMILY |
                                              CollectionPermissions.MSDBAccessBACKUPSET); //msdb..suspect_pages need to include this here

            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// WMICONFIGURATIONTEST
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool sp_OACheckPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember);
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// PROCEDURECACHELIST2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool ProcedureCachePermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = new bool();
            switch (cloudProviderId)
            {
                case Constants.MicrosoftAzureId:
                    collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin) || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                    break;
                default:
                    collectionPermissionsFlag = true;
                    break;
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// MirroredDatabaseScheduled2005
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool MirrorMonitoringRealtimePermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = false;
            switch (cloudProviderId)
            {
                case Constants.MicrosoftAzureId:
                    collectionPermissionsFlag = (collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN) || collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin));
                    break;
                default:
                    collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.DBMMONITORAccess);
                    break;
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// Probe Name - SERVERCONFIGURATION2014, SERVERCONFIGURATION2016
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool ServerConfigurationPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            bool collectionPermissionsFlag = false;
            collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.MSDBAccessSYSJOBS |
                      CollectionPermissions.MSDBAccessSYSJOBSTEPS | CollectionPermissions.SYSADMINMember | CollectionPermissions.ALTERANYLOGIN
                      | CollectionPermissions.ALTERANYUSER | /* SQLDM-29432:  Remove check for create db privilege.  CollectionPermissions.CREATEDATABASE | */ CollectionPermissions.ALTERANYDATABASE);

            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);

                return false;
            }
            var metadataPermissionsFlag =
                        metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitysyssqllogins | MetadataPermissions.MetadataVisibilitySysConfigurations);

            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);

                return false;
            }
            return true;
        }

        /// <summary>
        /// Probe Name - OS Metrics
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool OSMetricsPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = true;  // collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember);

            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }

            var metadataPermissionsFlag = metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilityXpMsver);
            // Log failed permissions for Collector
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Probe Name - AGENJOBHISTORY
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool AgentJobHistoryPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = collectionPermissions.HasFlag(
                CollectionPermissions.MSDBAccessSYSJOBHISTORY |
                CollectionPermissions.MSDBAccessSYSJOBS);
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// Probe Name - AgentJobSummary
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool AgentJobSummaryPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag =
               collectionPermissions.HasFlag(
                   CollectionPermissions.MSDBAccessSYSJOBHISTORY
                   | CollectionPermissions.EXECUTEMASTERXPSQLAGENTENUMJOBS)
               && (collectionPermissions.HasFlag(
                      CollectionPermissions.MSDBAccessSPGETCOMPOSITEJOBINFO)
                   || collectionPermissions.HasFlag(CollectionPermissions.MsdbDbOwner))  //  To Execute Stored Procedure
               && (collectionPermissions.HasFlag(
                       CollectionPermissions.MsdbSQLAgentReaderRole)
                   || collectionPermissions.HasFlag(CollectionPermissions.MsdbSQLAgentOperatorRole));  // To View Data from GETCOMPOSITEJOBINFO SP

            if (productVersion.IsGreaterThanSql2008Sp1R2())
            {
                collectionPermissionsFlag =
                    collectionPermissionsFlag
                    && collectionPermissions.HasFlag(
                        CollectionPermissions.SELECTSYSDMSERVERSERVICES);
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// Probe Name - Agent Job Summary
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool Agent_Job_SummaryPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag =
                collectionPermissions.HasFlag(
                    CollectionPermissions.MSDBAccessSYSJOBHISTORY
                    | CollectionPermissions.EXECUTEMASTERXPSQLAGENTENUMJOBS)
                && (collectionPermissions.HasFlag(
                        CollectionPermissions.MSDBAccessSPGETCOMPOSITEJOBINFO)
                    || collectionPermissions.HasFlag(CollectionPermissions.MsdbDbOwner)
                   ) //  To Execute Stored Procedure
                && (collectionPermissions.HasFlag(CollectionPermissions.MsdbSQLAgentReaderRole)
                    || collectionPermissions.HasFlag(CollectionPermissions.MsdbSQLAgentOperatorRole)
                   ); // To View Data from GETCOMPOSITEJOBINFO SP

            if (productVersion.IsGreaterThanSql2008Sp1R2())
            {
                collectionPermissionsFlag =
                    collectionPermissionsFlag
                    && collectionPermissions.HasFlag(
                        CollectionPermissions.SELECTSYSDMSERVERSERVICES);
            }
            // Log failed permissions for Collector
            if (!collectionPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return collectionPermissionsFlag;
        }

        /// <summary>
        /// Probe Name - QUERYMONITORWRITE2012EX
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool QueryMonitorStateStorePermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var metadataPermissionsFlag = metadataPermissions.HasFlag(MetadataPermissions.MetadataVisibilitytempdbsysobjects);

            // Log failed permissions for Collector
            if (!metadataPermissionsFlag)
            {
                // Append and log probe error
                AppendAndLogProbeError(permissionArgs, collectorName);
            }
            return metadataPermissionsFlag;
        }

        /// <summary>
        /// Probe Name - ACTIVITYMONITORAUTOGROW2012EX,ACTIVITYMONITORBLOCKING2012EX,ACTIVITYMONITORDEADLOCKS2012EX,
        /// ACTIVITYMONITORBLOCKING2005,ACTIVITYMONITORREAD2005,ACTIVITYMONITORRESTART2005,ACTIVITYMONITOR2012EX,
        /// ACTIVITYMONITORRESTART2012,ACTIVITYMONITORSTOP2005,ACTIVITYMONITORSTOPEX,
        /// ACTIVITYMONITORTRACEAUTOGROW,ACTIVITYMONITORTRACEDEADLOCKS
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool ActivityMonitorPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            bool collectionPermissionsFlag = false;
            if (permissionArgs != null && permissionArgs.Length >= 1)
            {
                // try parsing permissionArgs
                try
                {
                    bool TraceMonitoringEnabled = (bool)permissionArgs[0];
                    if (TraceMonitoringEnabled)
                    {
                        //case for ACTIVITYMONITORBLOCKING2005,ACTIVITYMONITORREAD2005,ACTIVITYMONITORRESTART2005,
                        //ACTIVITYMONITORRESTART2012,ACTIVITYMONITORSTOP2005,ACTIVITYMONITORTRACEAUTOGROW,ACTIVITYMONITORTRACEDEADLOCKS
                        collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.ALTERTRACE);
                    }
                    //case for ACTIVITYMONITORAUTOGROW2012EX, ACTIVITYMONITORBLOCKING2012EX,ACTIVITYMONITORDEADLOCKS2012EX,
                    //ACTIVITYMONITOR2012EX,ACTIVITYMONITORSTOPEX
                    else
                    {
                        collectionPermissionsFlag = collectionPermissions.HasFlag(CollectionPermissions.ALTERANYEVENTSESSION);
                    }

                    collectionPermissionsFlag = collectionPermissionsFlag && (metadataPermissions.HasFlag(MetadataPermissions.TempDbDataWriter) || metadataPermissions.HasFlag(MetadataPermissions.TempDbDbOwnerAccess) || collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember));

                    // Log failed permissions for Collector
                    if (!collectionPermissionsFlag)
                    {
                        // Append and log probe error
                        AppendAndLogProbeError(permissionArgs, collectorName);
                    }
                    return collectionPermissionsFlag;
                }
                catch (Exception exception)
                {
                    LOG.Error("Failed to parse permission Args : " + exception);
                }
            }
            return false;
        }

        /// <summary>
        /// Configuration2012
        /// </summary>
        /// <param name="collectorName"></param>
        /// <param name="cloudProviderId"></param>
        /// <param name="minimumPermissions"></param>
        /// <param name="metadataPermissions"></param>
        /// <param name="collectionPermissions"></param>
        /// <param name="productVersion"></param>
        /// <param name="permissionArgs"></param>
        /// <returns></returns>
        private static bool ConfigurationPermissionFunction(string collectorName, int? cloudProviderId, MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, object[] permissionArgs)
        {
            var collectionPermissionsFlag = false;
            switch (cloudProviderId)
            {
                case Constants.MicrosoftAzureId:
                    collectionPermissionsFlag =
                        collectionPermissions.HasFlag(CollectionPermissions.AzureAdmin)
                        || collectionPermissions.HasFlag(CollectionPermissions.SERVERADMIN);
                    break;
                default:
                    collectionPermissionsFlag = true;
                    break;
            }

            if (!collectionPermissionsFlag)
            {
                AppendAndLogProbeError(permissionArgs, collectorName);
                return false;
            }

            var metadataPermissionsFlag = metadataPermissions.HasFlag(
                MetadataPermissions.MetadataVisibilitySysConfigurations |
                MetadataPermissions.MetadataVisibilityXpMsver);
            if (!metadataPermissionsFlag)
            {
                AppendAndLogProbeError(permissionArgs, collectorName);
            }

            return metadataPermissionsFlag;
        }

        /// <summary>
        /// Append Failed Permissions <seealso cref="ProbeError"/>for Probe to permission args and log the required permissions using <seealso cref="ProbePermissionsResourceMessage"/>
        /// </summary>
        /// <param name="permissionArgs"></param>
        /// <param name="collectorName">Name of the collector</param>
        /// <param name="resourceKeyAppenders">
        /// Value to be appended to collector name indicating unique key in resource files (may be used to idenify permissions on cloudprovider/sql versions/batches)
        /// <example>
        /// <see cref="collectorName"/>_3 to denote Linux
        /// <see cref="collectorName"/>2008_3 to denote 2008 sql server and linux
        /// </example>
        /// </param>
        /// <param name="resourceKeyName">Defaults to <seealso cref="collectorName"/> if not passed</param>
        /// <param name="formatMessage">Defaults to <seealso cref="FAILEDPERMISSIONSFORMAT"/> format string</param>
        /// <param name="requiredFormatParameters">Required Format Parameters</param>
        /// <param name="exception">Exception occured while permissions evaluations</param>
        private static void AppendAndLogProbeError(object[] permissionArgs, string collectorName, string resourceKeyAppenders = null, string resourceKeyName = null, string formatMessage = null, object[] requiredFormatParameters = null, Exception exception = null)
        {
            // Append Probe Error in the PermissionCheck Arguments if not already added
            permissionArgs = AppendProbeErrorInPermissionCheckArgs(permissionArgs);

            // Modify Collector Name to include 
            permissionArgs[permissionArgs.Length - 1] = LogFailedPermissions(collectorName, resourceKeyAppenders, resourceKeyName, formatMessage, requiredFormatParameters, exception);
        }

        /// <summary>
        /// Log Failed Permissions for Probe and print the required permissions using <seealso cref="ProbePermissionsResourceMessage"/>
        /// </summary>
        /// <param name="collectorName">Name of the collector</param>
        /// <param name="resourceKeyAppenders">
        /// Value to be appended to collector name indicating unique key in resource files (may be used to idenify permissions on cloudprovider/sql versions/batches)
        /// <example>
        /// <see cref="collectorName"/>_3 to denote Linux
        /// <see cref="collectorName"/>2008_3 to denote 2008 sql server and linux
        /// </example>
        /// </param>
        /// <param name="resourceKeyName">Defaults to <seealso cref="collectorName"/> if not passed</param>
        /// <param name="formatMessage">Defaults to <seealso cref="FAILEDPERMISSIONSFORMAT"/> format string</param>
        /// <param name="requiredFormatParameters">Required Permissions Format Parameters</param>
        /// <param name="exception">Exception occured while permissions evaluations</param>
        private static ProbeError LogFailedPermissions(string collectorName, string resourceKeyAppenders = null, string resourceKeyName = null, string formatMessage = null, object[] requiredFormatParameters = null, Exception exception = null)
        {
            string requiredPermissions = null;
            try
            {
                // Remove Spaces from Collector Name and add appenders
                requiredPermissions =
                    ProbePermissionsResourceMessage.Instance.GetString(
                        (resourceKeyName ?? collectorName + resourceKeyAppenders).Replace(Space, string.Empty)
                        .Replace("-", string.Empty).Replace("/", string.Empty));
            }
            catch (Exception resourceReadException)
            {
                LOG.Error(
                    "Failed to find key in the resource file for Collector - {0}, Resource Appenders - {1}, ResourceKey - {2}, Format Message- {3}\n Exception: {4}",
                    collectorName, resourceKeyAppenders, resourceKeyName, formatMessage, resourceReadException);
            }

            // Failsafe approach
            if (string.IsNullOrEmpty(requiredPermissions))
            {
                requiredPermissions = ResourceReadFailed;
            }
            else if (requiredFormatParameters != null && requiredFormatParameters.Length > 0)
            {
                requiredPermissions = string.Format(requiredPermissions, requiredFormatParameters);
            }

            // Logs Error
            LOG.Error(string.Format(formatMessage ?? FAILEDPERMISSIONSFORMAT, collectorName, requiredPermissions) + exception);

            // Create and Returns Probe Error Object
            return new ProbeError() { Name = collectorName, RequiredPermissions = requiredPermissions };
        }

        /// <summary>
        /// Dispose Sql Command and associated connection
        /// </summary>
        /// <param name="sqlCommand"></param>
        private static void DisposeSqlCommand(SqlCommand sqlCommand)
        {
            if (sqlCommand != null)
            {
                var connection = sqlCommand.Connection;
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    connection.Dispose();
                }
                sqlCommand.Dispose();
            }
        }

        /// <summary>
        /// Populate runtime Permissions into <paramref name="dynamicPermissions"/> after executing <param name="sqlCommand"></param>
        /// </summary>
        /// <param name="collectorName">Name of the Collector and Type for Logging Purposes</param>
        /// <param name="sqlCommand">Command executed to return table with PermissionsName and PermissionValue</param>
        /// <param name="dynamicPermissions">Permissions Map to be populated</param>
        private static void PopulateDynamicPermissions(string collectorName, SqlCommand sqlCommand,
            Dictionary<string, int> dynamicPermissions)
        {
            if (sqlCommand != null && sqlCommand.Connection != null)
            {
                var connection = sqlCommand.Connection;
                using (sqlCommand)
                {
                    using (connection)
                    {
                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }
                        // To Ensure Closing of DataReader
                        using (SqlDataReader dataReader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            // Read Required Permissions
                            try
                            {
                                if (dataReader.HasRows)
                                {
                                    while (dataReader.Read())
                                    {
                                        var permissionNameIndex = dataReader.GetOrdinal("PermissionName");
                                        var permissionValueIndex = dataReader.GetOrdinal("PermissionValue");
                                        if (dataReader.IsDBNull(permissionNameIndex))
                                        {
                                            continue;
                                        }
                                        // Read Permission Name
                                        var permissionName = dataReader.GetString(permissionNameIndex);

                                        // Read Permission Value
                                        var permissionValue = dataReader.IsDBNull(permissionValueIndex)
                                            ? -1
                                            : dataReader.GetInt32(permissionValueIndex);

                                        // add to dynamic Permissions
                                        dynamicPermissions.Add(permissionName, permissionValue);
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                                // Logs MapValidation
                                LOG.Error(string.Format(
                                    "MapValidation:: Read {0} Permissions failed : {1}",
                                    collectorName,
                                    exception));
                            }
                            finally
                            {
                                dataReader.NextResult();
                            }
                        }
                    }
                }
            }
            // Dispose SQL Command
            DisposeSqlCommand(sqlCommand);
        }

        /// <summary>
        /// Validate Probe Permissions for <paramref name="collectorName"/> in server type <paramref name="cloudProviderId"/>
        /// </summary>
        /// <param name="masterPermissionsCommand">Read permissions from the master database</param>
        /// <param name="permissionsCommand">To read Minimum / Metadata/ Collection Permissions Object for server</param>
        /// <param name="productVersion">Server Product Version</param>
        /// <param name="collectorName">Probe Name</param>
        /// <param name="cloudProviderId">Server Type</param>
        /// <param name="permissionCheckArgs">Additional arguments for Permission Check</param>
        /// <returns>true for valid permissions</returns>
        internal static bool ValidateProbePermissions(SqlCommand masterPermissionsCommand, SqlCommand permissionsCommand, ServerVersion productVersion, string collectorName, int? cloudProviderId, params object[] permissionCheckArgs)
        {
            // To Skip Checking for Cloud Providers
            if (cloudProviderId == Constants.AmazonRDSId || (productVersion != null && productVersion.Major < 11))
            {
                return true;
            }

            var minimumPermissions = MinimumPermissions.None;
            var metadataPermissions = MetadataPermissions.None;
            var collectionPermissions = CollectionPermissions.None;

            // Dynamically checking permissions since for on demand direct command
            try
            {
                if (cloudProviderId == Constants.MicrosoftAzureId)
                {
                    using (masterPermissionsCommand)
                    {
                        // To Ensure Closing of DataReader
                        using (SqlDataReader permissionsReader = permissionsCommand.ExecuteReader())
                        {
                            collectionPermissions =
                                ReadPermissionsToEnum<CollectionPermissions>(
                                    permissionsReader, LOG);
                        }
                    }
                }

                using (permissionsCommand)
                {
                    // To Ensure Closing of DataReader
                    using (SqlDataReader permissionsReader = permissionsCommand.ExecuteReader())
                    {
                        // Read Required Permissions
                        minimumPermissions =
                            ReadPermissionsToEnum<MinimumPermissions>(
                                permissionsReader, LOG);
                        metadataPermissions =
                            ReadPermissionsToEnum<MetadataPermissions>(
                                    permissionsReader, LOG);
                        if (cloudProviderId == Constants.MicrosoftAzureId)
                        {
                            collectionPermissions |=
                                ReadPermissionsToEnum<CollectionPermissions>(
                                    permissionsReader, LOG);
                        }
                        else
                        {
                            collectionPermissions =
                                ReadPermissionsToEnum<CollectionPermissions>(
                                    permissionsReader, LOG);
                        }

                        // Read Replication Permissions
                        collectionPermissions |=
                            ReadPermissionsToEnum<CollectionPermissions>(
                                    permissionsReader, LOG);
                    }
                }

                // Dispose SQL Command
                DisposeSqlCommand(permissionsCommand);
            }
            catch (Exception exception)
            {
                LOG.Error(string.Format(
                    "ValidateProbePermissions:: Exception occured while reading permisssions for {0} with SQL Server Version {1}. Exception : {2}",
                    collectorName, productVersion, exception));
            }

            if (minimumPermissions != MinimumPermissions.None)
            {
                var hasMinimumPermissions = minimumPermissions.HasFlag(GetMinimumPermissionsThreshold(cloudProviderId));
                if (!hasMinimumPermissions)
                {
                    // Add Cloud Conditions
                    var collectorAppenders = cloudProviderId.HasValue ? cloudProviderId.ToString() : string.Empty;
                    // Append Probe Error in the PermissionCheck Arguments if not already added
                    AppendAndLogProbeError(permissionCheckArgs, collectorName, collectorAppenders, "MinimumPermissions",
                        FAILEDMINIMUMPERMISSIONSFORMAT);
                    return false;
                }
            }
            // Check Collection and Metadata Permissions Required for the collector are met
            if (minimumPermissions != MinimumPermissions.None && metadataPermissions != MetadataPermissions.None &&
                collectionPermissions != CollectionPermissions.None && !collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember) &&
                !ValidateProbePermissions(minimumPermissions, metadataPermissions,
                    collectionPermissions, productVersion, collectorName, cloudProviderId, permissionCheckArgs))    // Needs Permissions Check Args to be passed
            {
                var message =
                    string.Format(
                        "ValidateProbePermissions:: The user account used by the collection service on the collector {0} (with SQL Server Version {1}) does not have required rights required",
                        collectorName, productVersion);
                LOG.Error(message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validate Probe Permissions for <paramref name="collectorName"/> in server type <paramref name="cloudProviderId"/>
        /// </summary>
        /// <param name="snapshot">Snapshot containing permissions information</param>
        /// <param name="collectorName">Probe Name</param>
        /// <param name="cloudProviderId">Server Type</param>
        /// <param name="permissionCheckArgs">Additional arguments for Permission Check</param>
        /// <returns>true for valid permissions</returns>
        public static bool ValidateProbePermissions(Snapshot snapshot, string collectorName, int? cloudProviderId, params object[] permissionCheckArgs)
        {
            return ValidateProbePermissions(snapshot.MinimumPermissions, snapshot.MetadataPermissions,
                snapshot.CollectionPermissions, snapshot.ProductVersion, collectorName, cloudProviderId,
                permissionCheckArgs);
        }

        /// <summary>
        /// Validate Probe Permissions for <paramref name="collectorName"/> in server type <paramref name="cloudProviderId"/>
        /// </summary>
        /// <param name="minimumPermissions">Minimum Permissions Object for server</param>
        /// <param name="metadataPermissions">Metadata Permissions Object for server</param>
        /// <param name="collectionPermissions">Collection Permissions Object for server</param>
        /// <param name="productVersion">Server Product Version</param>
        /// <param name="collectorName">Probe Name</param>
        /// <param name="cloudProviderId">Server Type</param>
        /// <param name="permissionCheckArgs">Additional arguments for Permission Check</param>
        /// <returns>true for valid permissions</returns>
        public static bool ValidateProbePermissions(MinimumPermissions minimumPermissions, MetadataPermissions metadataPermissions, CollectionPermissions collectionPermissions, ServerVersion productVersion, string collectorName, int? cloudProviderId, params object[] permissionCheckArgs)
        {
            // To Skip Checking for Cloud Providers
            if (cloudProviderId == Constants.AmazonRDSId || (productVersion != null && productVersion.Major < 11))
            {
                return true;
            }
            // Calculate Minimum Permissions Threshold
            var hasMinimumPermissions = minimumPermissions.HasFlag(GetMinimumPermissionsThreshold(cloudProviderId));
            if (!hasMinimumPermissions)
            {
                // Add Cloud Conditions
                var collectorAppenders = cloudProviderId.HasValue ? cloudProviderId.ToString() : string.Empty;
                // Append Probe Error in the PermissionCheck Arguments if not already added
                AppendAndLogProbeError(permissionCheckArgs, collectorName, collectorAppenders, "MinimumPermissions",
                    FAILEDMINIMUMPERMISSIONSFORMAT);
                return false;
            }

            if (collectionPermissions != CollectionPermissions.None && collectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember))    // Needs Permissions Check Args to be passed
            {
                return true;
            }

            if (collectionPermissions.HasFlag(CollectionPermissions.ViewDatabaseStateMaster) &&
                cloudProviderId == Constants.MicrosoftAzureId)
            {
                return true;
            }

            if (cloudProviderId == Constants.MicrosoftAzureId)
            {
                // TODO: Segregate the batches and identify the collectors that needs segregated permissions
                return !azureCollectionsRequiredAdditionalAccess.Contains(collectorName) || VerifyForAzure(
                           collectorName,
                           cloudProviderId, minimumPermissions, metadataPermissions, collectionPermissions,
                           productVersion, permissionCheckArgs);
            }

            // Validate Mimimum Permissions first and then calls predicate for Collector if present
            return !MapValidation.ContainsKey(collectorName) || MapValidation[collectorName](collectorName,
                       cloudProviderId, minimumPermissions, metadataPermissions, collectionPermissions,
                       productVersion, permissionCheckArgs);
        }

        /// <summary>
        /// Append <seealso cref="ProbeError"/> object to <seealso cref="permissionCheckArgs"/> if not already added
        /// </summary>
        /// <param name="permissionCheckArgs">input permissions args</param>
        /// <returns>Appended <seealso cref="ProbeError"/> args</returns>
        public static object[] AppendProbeErrorInPermissionCheckArgs(object[] permissionCheckArgs)
        {
            if (permissionCheckArgs == null) // Create new Object and Adds new ProbeError
            {
                permissionCheckArgs = new object[1];
                permissionCheckArgs[0] = new ProbeError();
            }
            else if (permissionCheckArgs.Length == 0 || !(permissionCheckArgs[permissionCheckArgs.Length - 1] is ProbeError)) // Appends new ProbeError
            {
                var newlength = permissionCheckArgs.Length + 1;
                Array.Resize(ref permissionCheckArgs, newlength);
                permissionCheckArgs[newlength - 1] = new ProbeError();
            }
            else if (permissionCheckArgs[permissionCheckArgs.Length - 1] == null) // Adds new ProbeError
            {
                permissionCheckArgs[permissionCheckArgs.Length - 1] = new ProbeError();
            }
            return permissionCheckArgs;
        }
    }
}



