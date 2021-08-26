//------------------------------------------------------------------------------
// <copyright file="RepositoryHelper.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;
    using System.Diagnostics;
    using System.IO;
    using System.Management.Automation;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Data;
    using BBS.TracerX;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Objects.ApplicationSecurity;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Snapshots;
    using Microsoft.ApplicationBlocks.Data;
    using Objects;
    using Wintellect.PowerCollections;
    using XmlSerializerFactory = Idera.SQLdm.Common.Data.XmlSerializerFactory;
    using System.Globalization;

    internal static class Helper
    {
        private const string GetTagsStoredProcedure = "p_GetTags";
        private const string GetServerTagsStoredProcedure = "p_GetServerTags";
        private const string GetPermissionTagsAndServersStoredProcedure = "p_GetPermissionTagsAndServers";
        private static readonly Logger Log = Logger.GetLogger("RepositoryHelper");

        private const int MONITORED_SQL_SERVER_CALL_TIMEOUT = 5;
        private const string GetVersionAndPermissions = @"select serverproperty('productversion') AS ProductVersion, is_srvrolemember('sysadmin') AS SysAdmin;";//SQLdm 10.1 (srishti purohit) -IsSysAdmin check

        private static object sync = new object();


        internal static MonitoredSqlServer NewInstance(IManagementService managementService, MonitoredSqlServerInfo info)
        {
            MonitoredSqlServerConfiguration mssc = (MonitoredSqlServerConfiguration)info;
            MonitoredSqlServerMixin versionAndPermission = Helper.GetServerVersionAndPermission(mssc.ConnectionInfo);
            if (versionAndPermission == null)
            {
                Log.Error("after calling RepositoryHelper.GetServerVersionAndPermission, versionAndPermission object found null. Initializing object with default value.");
                versionAndPermission = new MonitoredSqlServerMixin();
            }
            mssc.IsUserSysAdmin = versionAndPermission.IsUserSysAdmin;
            return managementService.AddMonitoredSqlServer(mssc);
        }

        internal static bool IsInstanceValid(SQLdmDriveInfo drive, string instanceName, ProgressProvider progress)
        {
            return GetInstances(drive, instanceName, progress).Count > 0;
        }

        internal static List<MonitoredSqlServerInfo> GetInstances(SQLdmDriveInfo drive, string instanceName, ProgressProvider progress)
        {
            return GetInstances(drive, instanceName, true, true, progress);
        }

        internal static MonitoredSqlServerInfo GetInstance(SQLdmDriveInfo drive, string instanceName, ProgressProvider progress)
        {
            List<MonitoredSqlServerInfo> list = GetInstances(drive, instanceName, true, true, progress);
            foreach (MonitoredSqlServerInfo server in list)
            {
                if (server.InstanceName.Equals(instanceName, StringComparison.CurrentCultureIgnoreCase))
                    return server;
            }
            return null;
        }

        internal static List<MonitoredSqlServerInfo> GetInstances(SQLdmDriveInfo drive, string instanceName, bool activeOnly, bool customCounters, ProgressProvider progress)
        {
            List<MonitoredSqlServerInfo> result = new List<MonitoredSqlServerInfo>();

            UserToken userToken = drive.UserToken;

            if (userToken.IsSecurityEnabled && !String.IsNullOrEmpty(instanceName))
            {   // are they authorized to the instance
                PermissionType permissions = userToken.GetServerPermission(instanceName);
                if (permissions == PermissionType.None)
                    return result;
            }
            Dictionary<int, Tag> tags = drive.Tags;
            Dictionary<int, MonitoredSqlServerInfo> instances = new Dictionary<int, MonitoredSqlServerInfo>();

            using (SqlConnection connection = new SqlConnection(drive.RepositoryConnectionString))
            {
                connection.Open();
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, "p_GetMonitoredSqlServers",
                                                                   null, activeOnly, false, customCounters, null, null)) //SQLDM8.5 Mahesh: Added Additional Params
                {
                    while (reader.Read())
                    {
                        string serverName = reader.GetString(1);
                        if (userToken.IsSecurityEnabled && userToken.GetServerPermission(serverName) == PermissionType.None)
                        {
                            // only return servers the user is authorized view
                            continue;
                        }

                        MonitoredSqlServer mss = ConstructMonitoredSqlServer(reader);
                        MonitoredSqlServerInfo instance = new MonitoredSqlServerInfo(mss, drive);
                        instance.Drive = drive;

                        result.Add(instance);
                        instances.Add(instance.Id, instance);
                    }
                    if (customCounters && reader.NextResult())
                    {   // eat the custom counters
                    }
                    if (customCounters && reader.NextResult())
                    {   // update tags
                        MonitoredSqlServerInfo instance = null;
                        Tag tag = null;
                        string[] tagNames = new string[1];
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            int tagId = reader.GetInt32(1);
                            if (instance == null || instance.Id != id)
                            {
                                if (!instances.TryGetValue(id, out instance))
                                    continue;
                            }
                            if (tags.TryGetValue(tagId, out tag))
                            {
                                tagNames[0] = tag.Name;
                                instance.AddTags(tagNames);
                            }
                        }
                    }
                }
            }
            return result;
        }

        //        internal static MonitoredSqlServerConfiguration GetServerConfiguration(SQLdmDriveInfo drive, MonitoredSqlServerInfo mssi)
        //        {
        //            Dictionary<int, Tag> tags = drive.Tags;
        //
        //            // does not copy tags to configuration
        //            MonitoredSqlServerConfiguration result = (MonitoredSqlServerConfiguration)mssi;
        //
        //
        //        }

        internal static IList<CustomCounterInfo> GetCustomCounters(SQLdmDriveInfo drive, string counterName)
        {
            List<CustomCounterInfo> result = new List<CustomCounterInfo>();

            MetricDefinitions metrics = new MetricDefinitions(true, false, true);
            metrics.Load(drive.RepositoryConnectionString);

            foreach (int metricID in metrics.GetCounterDefinitionKeys())
            {
                MetricDescription? description = metrics.GetMetricDescription(metricID);
                if (description.HasValue)
                {
                    // add new counter to the grid
                    CustomCounterInfo counter = new CustomCounterInfo(metrics.GetCounterDefinition(metricID), metrics.GetMetricDefinition(metricID), description.Value);
                    result.Add(counter);
                }
            }

            return result;
        }

        internal static IList<AlertInfo> GetActiveAlerts(SQLdmDriveInfo drive, string instanceName)
        {
            List<AlertInfo> result = new List<AlertInfo>();

            string serverXmlFilter = UserToken.CreateServerXmlFilter(instanceName);

            MetricDefinitions metrics = new MetricDefinitions(true, false, true);
            metrics.Load(drive.RepositoryConnectionString);

            using (SqlConnection connection = new SqlConnection(drive.RepositoryConnectionString))
            {
                connection.Open();
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(
                    connection,
                    "p_GetAlerts",
                    0,
                    null,
                    null,
                    serverXmlFilter,
                    null,
                    null,
                    null,
                    null,
                    null, //SQLDM 8.5 Mahesh: CategoryId added for rest service
                    true,
                    0))
                {
                    while (dataReader.Read())
                    {
                        result.Add(new AlertInfo(metrics, dataReader));
                    }
                }
            }

            return result;
        }

        internal static T GetReaderValue<T>(SqlDataReader dataReader, int columnId, T defaultValue)
        {
            if (dataReader.IsDBNull(columnId))
                return defaultValue;

            return (T)dataReader.GetValue(columnId);
        }

        internal static T GetDynamicParameterValue<T>(RuntimeDefinedParameterDictionary parameters, string name, T defaultValue)
        {
            T result = defaultValue;

            RuntimeDefinedParameter rdp;
            if (parameters.TryGetValue(name, out rdp))
            {
                if (rdp.IsSet)
                {
                    result = (T)rdp.Value;
                }
            }
            return result;
        }

        internal static bool IsDynamicParameterSet(RuntimeDefinedParameterDictionary parameters, string name)
        {
            RuntimeDefinedParameter rdp;
            if (parameters.TryGetValue(name, out rdp))
            {
                return rdp.IsSet;
            }
            return false;
        }

        internal static SQLdmUserInfo GetSQLdmUserInfo(SQLdmDriveInfo drive, string userName)
        {
            SQLdmUserInfo userInfo = null;
            foreach (PermissionDefinition pd in GetSQLdmUserPermission(drive, userName))
            {
                if (userInfo == null)
                    userInfo = new SQLdmUserInfo(pd);
                else
                    userInfo.AddPermissions(pd);
            }
            return userInfo;
        }

        internal static bool GetSQLdmUserExists(SQLdmDriveInfo drive, string userName)
        {
            Configuration secConfig = drive.GetAppSecurityConfiguration();
            foreach (PermissionDefinition pd in secConfig.Permissions.Values)
            {
                if (pd.Login.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        internal static bool GetSQLdmUserHasServerPermission(SQLdmDriveInfo drive, string userName, string instanceName)
        {
            bool result = false;
            Configuration secConfig = drive.GetAppSecurityConfiguration();
            foreach (PermissionDefinition pd in secConfig.Permissions.Values)
            {
                if (!pd.Enabled)
                    continue;

                if (pd.Login.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
                {
                    if (pd.PermissionType == PermissionType.Administrator)
                        break;

                    foreach (Server server in pd.GetServerList())
                    {
                        if (instanceName.Equals(server.InstanceName, StringComparison.CurrentCultureIgnoreCase))
                            return true;
                    }
                }
            }
            return result;
        }

        internal static bool IsServerInPermissionDefinition(PermissionDefinition permission, string instanceName)
        {
            foreach (Server server in permission.GetServerList())
            {
                if (server.InstanceName.Equals(instanceName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        internal static int[] GetUpdatedServerList(SQLdmDriveInfo drive, string addInstanceName, string removeInstanceName, PermissionDefinition pd, ProgressProvider progress)
        {
            List<int> result = new List<int>();

            foreach (Server server in pd.GetServerList())
            {
                if (!String.IsNullOrEmpty(removeInstanceName))
                {
                    if (removeInstanceName.Equals(server.InstanceName, StringComparison.CurrentCultureIgnoreCase))
                        continue;
                }

                result.Add(server.SQLServerID);
            }

            if (!String.IsNullOrEmpty(addInstanceName))
            {
                MonitoredSqlServerInfo serverInfo = GetInstance(drive, addInstanceName, progress);
                if (serverInfo != null && !result.Contains(serverInfo.Id))
                    result.Add(serverInfo.Id);
                else
                    throw new ApplicationException(String.Format("Instance '{0}' is not valid", addInstanceName));
            }

            return result.ToArray();
        }

        internal static Set<int> GetTagsForPermissionId(SQLdmDriveInfo drive, int permissionId)
        {
            Set<int> result = new Set<int>();
            foreach (Tag tag in drive.Tags.Values)
            {
                if (tag.Permissions.Contains(permissionId))
                    result.Add(tag.Id);
            }
            return result;
        }

        internal static bool GetGrantPermissionModifications(SQLdmDriveInfo drive, string userName, Tag tag, PermissionType grantPermission, out PermissionDefinition grant, out PermissionDefinition[] revoke)
        {
            bool grantNeeded = true;
            PermissionDefinition gpd = null;
            List<PermissionDefinition> rpd = null;

            foreach (PermissionDefinition pd in GetSQLdmUserPermission(drive, userName))
            {
                bool permissionInTag = tag.Permissions.Contains(pd.PermissionID);

                if (pd.PermissionType == grantPermission)
                {
                    if (!pd.Enabled)
                        continue;

                    if (permissionInTag)
                        grantNeeded = false;

                    if (gpd == null)
                        gpd = pd;
                }
                else if (permissionInTag)
                {
                    if (rpd == null)
                        rpd = new List<PermissionDefinition>();

                    rpd.Add(pd);
                }
            }

            grant = grantNeeded ? gpd : null;
            revoke = rpd != null ? rpd.ToArray() : null;

            return grantNeeded;
        }

        internal static bool GetGrantPermissionModifications(SQLdmDriveInfo drive, string userName, string instanceName, PermissionType grantPermission, out PermissionDefinition grant, out PermissionDefinition[] revoke)
        {
            bool grantNeeded = true;
            PermissionDefinition gpd = null;
            List<PermissionDefinition> rpd = null;

            foreach (PermissionDefinition pd in GetSQLdmUserPermission(drive, userName))
            {
                if (!pd.Enabled)
                    continue;

                bool serverInPd = IsServerInPermissionDefinition(pd, instanceName);

                if (pd.PermissionType == grantPermission)
                {
                    if (serverInPd)
                        grantNeeded = false;

                    if (gpd == null)
                        gpd = pd;

                    continue;
                }

                if (serverInPd)
                {
                    if (rpd == null)
                        rpd = new List<PermissionDefinition>();

                    rpd.Add(pd);
                }
            }

            grant = grantNeeded ? gpd : null;
            revoke = rpd != null ? rpd.ToArray() : null;

            return grantNeeded;
        }

        internal static List<PermissionDefinition> GetSQLdmUserPermission(SQLdmDriveInfo drive, string userName)
        {
            List<PermissionDefinition> result = new List<PermissionDefinition>();

            Configuration secConfig = drive.GetAppSecurityConfiguration();
            foreach (PermissionDefinition pd in secConfig.Permissions.Values)
            {
                if (pd.Login.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
                {
                    result.Add(pd);
                }
            }
            return result;
        }

        internal static int[] GetInstanceIds(SQLdmDriveInfo drive, string[] instanceNames)
        {
            Set<string> nameSet = new Set<string>(instanceNames, new CaseInsensitiveEqualityComparer());
            List<int> resultList = new List<int>();

            UserToken userToken = drive.UserToken;

            if (userToken.IsSecurityEnabled)
            {   // make sure user has authority to each instance specified
                foreach (string instanceName in instanceNames)
                {
                    PermissionType permissions = userToken.GetServerPermission(instanceName);
                    if (permissions == PermissionType.None)
                        throw new PSSecurityException(String.Format("You are not autorized to server {0}", instanceName));
                }
            }

            if (instanceNames.Length > 0)
            {
                using (SqlConnection connection = new SqlConnection(drive.RepositoryConnectionString))
                {
                    connection.Open();
                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, "p_GetMonitoredSqlServers",
                                                                          null, false, false, false, null, null)) //SQLDM8.5 Mahesh: Added Additional Params
                    {
                        while (reader.Read())
                        {
                            string serverName = reader.GetString(1);
                            if (nameSet.Contains(serverName))
                                resultList.Add(reader.GetInt32(0));
                        }
                    }
                }
            }

            return resultList.ToArray();
        }

        ///SQLdm 10.1 (Barkha Khatri) adding this as a part of sysAdmin feature 
        /// <summary>
        /// gets the server version and permissions(sysAdmin or not) for the particular server
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <returns></returns>
        public static MonitoredSqlServerMixin GetServerVersionAndPermission(SqlConnectionInfo connectionInfo)
        {
            using (Log.InfoCall("GetServerVersionAndPermission"))
            {
                if (connectionInfo == null)
                {
                    Log.Error("Error in GetServerVersionAndPermission : connectionInfo found null.");
                    throw new ArgumentNullException("connectionInfo");
                }
                MonitoredSqlServerMixin queryResult = new MonitoredSqlServerMixin();
                string connectionString = connectionInfo.ConnectionString;

                try
                {
                    if (connectionString == null)
                    {
                        throw new ArgumentNullException("connectionString");
                    }
                    connectionInfo.ConnectionTimeout = MONITORED_SQL_SERVER_CALL_TIMEOUT;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = GetVersionAndPermissions;
                            using (var reader = command.ExecuteReader())
                            {

                                while (reader.Read())
                                {
                                    queryResult.ServerVersion = new Common.ServerVersion(reader["ProductVersion"].ToString());
                                    queryResult.IsUserSysAdmin = Convert.ToBoolean(reader["SysAdmin"]);
                                }
                            }
                        }

                        if (queryResult.ServerVersion != null)
                        {
                            Log.InfoFormat("GetServerVersionAndPermission -- This is the server version and sysAdminFlag {0} {1}:", queryResult.ServerVersion.ToString(), queryResult.IsUserSysAdmin);
                            return queryResult;
                        }
                        else
                        {
                            Log.Error(String.Format("GetServerVersionAndPermission--the query returns null for the SQL Server with the Connection String :[{0}]./n {1}", (connection != null && connection.DataSource != null) ? connection.DataSource : string.Empty));
                            return null;
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(String.Format("GetServerVersionAndPermission--Error while executing GetServerVersionAndPermission.", e));
                    return null;
                }
            }
        }


        private static MonitoredSqlServer ConstructMonitoredSqlServer(SqlDataReader dataReader)
        {
            if (dataReader == null)
            {
                throw new ArgumentNullException("dataReader");
            }

            int returnId = (int)dataReader["SQLServerID"];
            SqlConnectionInfo instanceConnectionInfo = new SqlConnectionInfo();
            instanceConnectionInfo.ApplicationName = Idera.SQLdm.Common.Constants.CollectionServceConnectionStringApplicationName;
            instanceConnectionInfo.InstanceName = dataReader["InstanceName"] as string;
            bool isActive = (bool)dataReader["Active"];
            DateTime registeredDate = (DateTime)dataReader["RegisteredDate"]; // UTC
            int collectionServiceIdColumn = dataReader.GetOrdinal("CollectionServiceID");
            SqlGuid sqlGuid = dataReader.GetSqlGuid(collectionServiceIdColumn);
            Guid collectionServiceId = sqlGuid.IsNull ? Guid.Empty : sqlGuid.Value;
            instanceConnectionInfo.UseIntegratedSecurity = (bool)dataReader["UseIntegratedSecurity"];

            if (!instanceConnectionInfo.UseIntegratedSecurity)
            {
                instanceConnectionInfo.UserName = dataReader["Username"] as string;
                instanceConnectionInfo.EncryptedPassword = dataReader["Password"] as string;
            }
            instanceConnectionInfo.EncryptData = (bool)dataReader["EncryptData"];
            instanceConnectionInfo.TrustServerCertificate = (bool)dataReader["TrustServerCert"];

            int scheduledCollectionInterval = (int)dataReader["ScheduledCollectionIntervalInSeconds"];
            bool maintenanceModeEnabled = (bool)dataReader["MaintenanceModeEnabled"];

            AdvancedQueryMonitorConfiguration queryMonitorAdvancedConfiguration = null;
            SqlString queryMonitorAdvancedConfigurationXml = dataReader.GetSqlString(dataReader.GetOrdinal("QueryMonitorAdvancedConfiguration"));
            if (!queryMonitorAdvancedConfigurationXml.IsNull)
            {
                queryMonitorAdvancedConfiguration =
                    AdvancedQueryMonitorConfiguration.DeserializeFromXml(queryMonitorAdvancedConfigurationXml.Value);
            }

            QueryMonitorConfiguration queryMonitorConfiguration = new QueryMonitorConfiguration(
                (bool)dataReader["QueryMonitorEnabled"],
                (bool)dataReader["QueryMonitorSqlBatchEventsEnabled"],
                (bool)dataReader["QueryMonitorSqlStatementEventsEnabled"],
                (bool)dataReader["QueryMonitorStoredProcedureEventsEnabled"],
                TimeSpan.FromMilliseconds((int)dataReader["QueryMonitorDurationFilterInMilliseconds"]),
                TimeSpan.FromMilliseconds((int)dataReader["QueryMonitorCpuUsageFilterInMilliseconds"]),
                (int)dataReader["QueryMonitorLogicalDiskReadsFilter"],
                (int)dataReader["QueryMonitorPhysicalDiskWritesFilter"],
                new FileSize((int)dataReader["QueryMonitorTraceFileSizeKB"]),
                (int)dataReader["QueryMonitorTraceFileRollovers"],
                (int)dataReader["QueryMonitorTraceRecordsPerRefresh"],
                queryMonitorAdvancedConfiguration,
                (bool)dataReader["QueryMonitorTraceMonitoringEnabled"],//SQLdm 9.1 (Ankit Srivastava) -- Fixed Open functional isssue--replaced default value with the value in repo
                (bool)dataReader["QueryMonitorCollectQueryPlan"],//SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended event sessions --  Added parameters
                (bool)dataReader["QueryMonitorCollectEstimatedQueryPlan"],
                (int)dataReader["QueryMonitorTopPlanCountFilter"],
                (int)dataReader["QueryMonitorTopPlanCategoryFilter"],
                (bool)dataReader["QueryMonitorQueryStoreMonitoringEnabled"]); // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store

            ActivityMonitorConfiguration activityMonitorConfiguration = new ActivityMonitorConfiguration(
                (bool)dataReader["ActivityMonitorEnabled"],
                (bool)dataReader["ActivityMonitorDeadlockEventsEnabled"],
                (bool)dataReader["ActivityMonitorBlockingEventsEnabled"],
                (bool)dataReader["ActivityMonitorAutoGrowEventsEnabled"],
                (int)dataReader["ActivityMonitorBlockedProcessThreshold"],
                new FileSize((int)dataReader["QueryMonitorTraceFileSizeKB"]),
                (int)dataReader["QueryMonitorTraceFileRollovers"],
                (int)dataReader["QueryMonitorTraceRecordsPerRefresh"],
                queryMonitorAdvancedConfiguration,
                (bool)dataReader["ActivityMonitorTraceMonitoringEnabled"]);//START SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events -- added default value for new param

            MaintenanceMode maintenanceMode = new MaintenanceMode();
            if (dataReader.IsDBNull(dataReader.GetOrdinal("MaintenanceModeType")) == false)
            {
                maintenanceMode.MaintenanceModeType = (MaintenanceModeType)(int)dataReader["MaintenanceModeType"];
            }
            if (dataReader.IsDBNull(dataReader.GetOrdinal("MaintenanceModeStart")) == false)
            {
                maintenanceMode.MaintenanceModeStart = (DateTime)dataReader["MaintenanceModeStart"];
            }
            if (dataReader.IsDBNull(dataReader.GetOrdinal("MaintenanceModeStop")) == false)
            {
                maintenanceMode.MaintenanceModeStop = (DateTime)dataReader["MaintenanceModeStop"];
            }
            if (dataReader.IsDBNull(dataReader.GetOrdinal("MaintenanceModeDurationSeconds")) == false)
            {
                maintenanceMode.MaintenanceModeDuration =
                    TimeSpan.FromSeconds((int)dataReader["MaintenanceModeDurationSeconds"]);
            }
            if (dataReader.IsDBNull(dataReader.GetOrdinal("MaintenanceModeDays")) == false)
            {
                maintenanceMode.MaintenanceModeDays = (short)dataReader["MaintenanceModeDays"];
            }
            if (dataReader.IsDBNull(dataReader.GetOrdinal("MaintenanceModeRecurringStart")) == false)
            {
                maintenanceMode.MaintenanceModeRecurringStart = (DateTime)dataReader["MaintenanceModeRecurringStart"];
            }

            MonitoredSqlServer instance =
                new MonitoredSqlServer(returnId, registeredDate, isActive, collectionServiceId,
                                       instanceConnectionInfo,
                                       TimeSpan.FromSeconds(scheduledCollectionInterval),
                                       maintenanceModeEnabled, queryMonitorConfiguration,
                                       activityMonitorConfiguration, maintenanceMode);

            //SQLdm 10.1 (Barkha Khatri) Sys Admin flag
            instance.IsUserSysAdmin = (bool)dataReader["IsUserSysAdmin"];
            instance.TableGrowthConfiguration = new TableGrowthConfiguration(returnId);
            instance.TableFragmentationConfiguration = new TableFragmentationConfiguration(returnId);

            int ordinal = dataReader.GetOrdinal("GrowthStatisticsStartTime");
            if (dataReader.IsDBNull(ordinal))
                instance.TableGrowthConfiguration.GrowthStatisticsStartTime = null;
            else
                instance.TableGrowthConfiguration.GrowthStatisticsStartTime = dataReader.GetDateTime(ordinal);


            ordinal = dataReader.GetOrdinal("LastGrowthStatisticsRunTime");
            if (dataReader.IsDBNull(ordinal))
                instance.TableGrowthConfiguration.LastGrowthStatisticsRunTime = null;
            else
                instance.TableGrowthConfiguration.LastGrowthStatisticsRunTime = dataReader.GetDateTime(ordinal);

            ordinal = dataReader.GetOrdinal("GrowthStatisticsDays");
            if (dataReader.IsDBNull(ordinal))
                instance.TableGrowthConfiguration.GrowthStatisticsDays = null;
            else
                instance.TableGrowthConfiguration.GrowthStatisticsDays = dataReader.GetByte(ordinal);

            ordinal = dataReader.GetOrdinal("TableStatisticsExcludedDatabases");
            if (!dataReader.IsDBNull(ordinal))
            {
                try
                {
                    string xml = dataReader.GetString(ordinal);
                    Type valueType = typeof(string[]);
                    XmlSerializer serializer = XmlSerializerFactory.GetSerializer(valueType);

                    StringReader stream = new StringReader(xml);
                    using (XmlReader xmlReader = XmlReader.Create(stream))
                    {
                        List<string> valueList = new List<string>();
                        string[] values = (string[])serializer.Deserialize(xmlReader);
                        valueList.AddRange(values);
                        instance.TableGrowthConfiguration.TableStatisticsExcludedDatabases = valueList;
                        instance.TableFragmentationConfiguration.TableStatisticsExcludedDatabases = valueList;
                    }
                }
                catch (Exception e)
                {
                    Debug.Print("Exception deserializing TableStatisticsExcludedDatabases ", e);
                }
            }



            ordinal = dataReader.GetOrdinal("ReorgStatisticsStartTime");
            if (dataReader.IsDBNull(ordinal))
                instance.TableFragmentationConfiguration.FragmentationStatisticsStartTime = null;
            else
                instance.TableFragmentationConfiguration.FragmentationStatisticsStartTime = dataReader.GetDateTime(ordinal);



            ordinal = dataReader.GetOrdinal("LastReorgStatisticsRunTime");
            if (dataReader.IsDBNull(ordinal))
                instance.TableFragmentationConfiguration.LastFragmentationStatisticsRunTime = null;
            else
                instance.TableFragmentationConfiguration.LastFragmentationStatisticsRunTime = dataReader.GetDateTime(ordinal);



            ordinal = dataReader.GetOrdinal("ReorgStatisticsDays");
            if (dataReader.IsDBNull(ordinal))
                instance.TableFragmentationConfiguration.FragmentationStatisticsDays = null;
            else
                instance.TableFragmentationConfiguration.FragmentationStatisticsDays = dataReader.GetByte(ordinal);



            try
            {
                // EarliestData was added 6/28/2007.
                ordinal = dataReader.GetOrdinal("EarliestData");
                if (!dataReader.IsDBNull(ordinal))
                {
                    instance.EarliestData = dataReader.GetDateTime(ordinal).ToLocalTime();
                }
            }
            catch (Exception ex)
            {
                // This should only happen when connecting to old repositories.
                Debug.Print("Exception getting EarliestData in ConstructMonitoredSqlServer: ", ex);
            }

            instance.ReorganizationMinimumTableSize.Kilobytes = (int)dataReader["ReorgMinTableSizeKB"];

            ordinal = dataReader.GetOrdinal("DisableReplicationMonitoring");
            if (!dataReader.IsDBNull(ordinal))
                instance.ReplicationMonitoringDisabled = (bool)dataReader["DisableReplicationMonitoring"];

            ordinal = dataReader.GetOrdinal("CustomCounterTimeoutInSeconds");
            if (dataReader.IsDBNull(ordinal))
                instance.CustomCounterTimeout = TimeSpan.FromSeconds(180);
            else
                instance.CustomCounterTimeout = TimeSpan.FromSeconds(dataReader.GetInt32(ordinal));

            // Update the Ole Automation creation context
            bool isOutOfProcOleAutomation = false;
            ordinal = dataReader.GetOrdinal("OutOfProcOleAutomation");
            if (!dataReader.IsDBNull(ordinal))
                isOutOfProcOleAutomation = dataReader.GetBoolean(ordinal);
            instance.OleAutomationContext = isOutOfProcOleAutomation
                                                ? MonitoredSqlServer.OleAutomationExecutionContext.OutOfProc
                                                : MonitoredSqlServer.OleAutomationExecutionContext.Both;

            ordinal = dataReader.GetOrdinal("DisableExtendedHistoryCollection");
            if (!dataReader.IsDBNull(ordinal))
                instance.ExtendedHistoryCollectionDisabled = (bool)dataReader["DisableExtendedHistoryCollection"];

            ordinal = dataReader.GetOrdinal("DisableOleAutomation");
            if (!dataReader.IsDBNull(ordinal))
                instance.OleAutomationUseDisabled = (bool)dataReader["DisableOleAutomation"];

            ordinal = dataReader.GetOrdinal("DiskCollectionSettings");
            if (!dataReader.IsDBNull(ordinal))
            {
                try
                {
                    string xml = dataReader.GetString(ordinal);
                    XmlSerializer serializer =
                        Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(DiskCollectionSettings));

                    StringReader stream = new StringReader(xml);
                    using (XmlReader xmlReader = XmlReader.Create(stream))
                    {
                        DiskCollectionSettings settings = serializer.Deserialize(xmlReader) as DiskCollectionSettings;
                        if (settings != null)
                            instance.DiskCollectionSettings = settings;
                    }
                }
                catch (Exception e)
                {
                    Debug.Print("Exception deserializing DiskCollectionSettings", e);
                }
            }

            ordinal = dataReader.GetOrdinal("InputBufferLimiter");
            if (!dataReader.IsDBNull(ordinal))
                instance.InputBufferLimiter = dataReader.GetInt32(ordinal);

            //START -- Capturing current value of Friendly Name, InputBufferLimited, Serverversion for instance in object
            //SQLdm 10.1 (Srishti Purohit) Power shell 
            ordinal = dataReader.GetOrdinal("FriendlyServerName");
            if (!dataReader.IsDBNull(ordinal))
                instance.FriendlyServerName = dataReader.GetString(ordinal);

            ordinal = dataReader.GetOrdinal("InputBufferLimited");
            if (!dataReader.IsDBNull(ordinal))
                instance.InputBufferLimited = dataReader.GetBoolean(ordinal);

            ordinal = dataReader.GetOrdinal("ServerVersion");
            if (!dataReader.IsDBNull(ordinal))
                instance.MostRecentSQLVersion = new Common.ServerVersion(dataReader.GetString(ordinal));
            //END -- Capturing current value of Friendly Name, InputBufferLimited, Serverversion for instance in object
            //SQLdm 10.1 (Srishti Purohit) Power shell 

            ordinal = dataReader.GetOrdinal("ActiveClusterNode");
            if (!dataReader.IsDBNull(ordinal))
                instance.ActiveClusterNode = dataReader.GetString(ordinal);

            ordinal = dataReader.GetOrdinal("PreferredClusterNode");
            if (!dataReader.IsDBNull(ordinal))
                instance.PreferredClusterNode = dataReader.GetString(ordinal);


            ActiveWaitsConfiguration awc = new ActiveWaitsConfiguration(instance.Id);

            ordinal = dataReader.GetOrdinal("ActiveWaitCollectorEnabled");
            if (!dataReader.IsDBNull(ordinal))
            {
                awc.Enabled = dataReader.GetBoolean(ordinal);

                ordinal = dataReader.GetOrdinal("ActiveWaitCollectorStartTimeRelative");
                if (!dataReader.IsDBNull(ordinal)) awc.StartTimeRelative = dataReader.GetDateTime(ordinal);

                ordinal = dataReader.GetOrdinal("ActiveWaitCollectorRunTimeSeconds");
                if (!dataReader.IsDBNull(ordinal))
                {
                    awc.RunTime = TimeSpan.FromSeconds(dataReader.GetInt32(ordinal));
                }

                else
                {
                    awc.RunTime = null;
                }

                ordinal = dataReader.GetOrdinal("ActiveWaitCollectorCollectionTimeSeconds");
                if (!dataReader.IsDBNull(ordinal)) awc.CollectionTimeSeconds = dataReader.GetInt32(ordinal);

                ordinal = dataReader.GetOrdinal("ActiveWaitLoopTimeMilliseconds");
                if (!dataReader.IsDBNull(ordinal)) awc.LoopTimeMilliseconds = dataReader.GetInt32(ordinal);

                SqlString activeWaitAdvancedConfigurationXml =
                   dataReader.GetSqlString(dataReader.GetOrdinal("ActiveWaitAdvancedConfiguration"));
                AdvancedQueryFilterConfiguration queryFilterAdvancedConfiguration = null;
                if (!activeWaitAdvancedConfigurationXml.IsNull)
                {
                    queryFilterAdvancedConfiguration =
                        AdvancedQueryFilterConfiguration.DeserializeFromXml(activeWaitAdvancedConfigurationXml.Value);

                    awc.AdvancedConfiguration = queryFilterAdvancedConfiguration;
                }
                else
                {
                    awc.AdvancedConfiguration = new AdvancedQueryFilterConfiguration();
                }

                // SQLdm 10.4(Varun Chopra) query waits using Query Store
                ordinal = dataReader.GetOrdinal("ActiveWaitQsEnable");
                if (!dataReader.IsDBNull(ordinal))
                {
                    awc.EnabledQs = dataReader.GetBoolean(ordinal);
                }

                ordinal = dataReader.GetOrdinal("ActiveWaitXEEnable");
                if (!dataReader.IsDBNull(ordinal))
                {
                    awc.EnabledXe = dataReader.GetBoolean(ordinal);

                    ordinal = dataReader.GetOrdinal("ActiveWaitXEFileSizeMB");
                    if (!dataReader.IsDBNull(ordinal))
                    {
                        if (!dataReader.IsDBNull(ordinal)) awc.FileSizeXeMB = dataReader.GetInt32(ordinal);
                    }

                    ordinal = dataReader.GetOrdinal("ActiveWaitXEFilesRollover");
                    if (!dataReader.IsDBNull(ordinal))
                    {
                        if (!dataReader.IsDBNull(ordinal)) awc.FileSizeRolloverXe = dataReader.GetInt32(ordinal);
                    }

                    ordinal = dataReader.GetOrdinal("ActiveWaitXERecordsPerRefresh");
                    if (!dataReader.IsDBNull(ordinal))
                    {
                        if (!dataReader.IsDBNull(ordinal)) awc.RecordsPerRefreshXe = dataReader.GetInt32(ordinal);
                    }

                    ordinal = dataReader.GetOrdinal("ActiveWaitXEMaxMemoryMB");
                    if (!dataReader.IsDBNull(ordinal))
                    {
                        if (!dataReader.IsDBNull(ordinal)) awc.MaxMemoryXeMB = dataReader.GetInt32(ordinal);
                    }

                    ordinal = dataReader.GetOrdinal("ActiveWaitXEEventRetentionMode");
                    if (!dataReader.IsDBNull(ordinal))
                    {
                        //SQLdm 9.1 (Ankit Srivastava) Activity Monitoring with Extended Events - enum made public 
                        if (!dataReader.IsDBNull(ordinal)) awc.EventRetentionModeXe = (XeEventRetentionMode)dataReader.GetByte(ordinal);
                    }

                    ordinal = dataReader.GetOrdinal("ActiveWaitXEMaxDispatchLatencySecs");
                    if (!dataReader.IsDBNull(ordinal))
                    {
                        if (!dataReader.IsDBNull(ordinal)) awc.MaxDispatchLatencyXe = dataReader.GetInt32(ordinal);
                    }

                    ordinal = dataReader.GetOrdinal("ActiveWaitXEMaxEventSizeMB");
                    if (!dataReader.IsDBNull(ordinal))
                    {
                        if (!dataReader.IsDBNull(ordinal)) awc.MaxEventSizeXemb = dataReader.GetInt32(ordinal);
                    }

                    ordinal = dataReader.GetOrdinal("ActiveWaitXEMemoryPartitionMode");
                    if (!dataReader.IsDBNull(ordinal))
                    {
                        //SQLdm 9.1 (Ankit Srivastava) Activity Monitoring with Extended Events - enum made public 
                        if (!dataReader.IsDBNull(ordinal)) awc.MemoryPartitionModeXe = (XEMemoryPartitionMode)dataReader.GetInt32(ordinal);
                    }

                    ordinal = dataReader.GetOrdinal("ActiveWaitXETrackCausality");
                    if (!dataReader.IsDBNull(ordinal))
                    {
                        if (!dataReader.IsDBNull(ordinal)) awc.TrackCausalityXe = dataReader.GetBoolean(ordinal);
                    }

                    ordinal = dataReader.GetOrdinal("ActiveWaitXEStartupState");
                    if (!dataReader.IsDBNull(ordinal))
                    {
                        if (!dataReader.IsDBNull(ordinal)) awc.StartupStateXe = dataReader.GetBoolean(ordinal);
                    }

                    ordinal = dataReader.GetOrdinal("ActiveWaitsXEFileName");
                    if (!dataReader.IsDBNull(ordinal))
                    {
                        if (!dataReader.IsDBNull(ordinal)) awc.FileNameXEsession = dataReader.GetString(ordinal);
                    }
                }
            }
            else
            {
                awc.Enabled = false;
            }

            instance.ActiveWaitsConfiguration = awc;

            ordinal = dataReader.GetOrdinal("ClusterCollectionSetting");
            if (!dataReader.IsDBNull(ordinal)) instance.ClusterCollectionSetting = (ClusterCollectionSetting)dataReader.GetInt16(ordinal);

            ordinal = dataReader.GetOrdinal("ServerPingInterval");
            if (!dataReader.IsDBNull(ordinal))
                instance.ServerPingInterval = TimeSpan.FromSeconds(dataReader.GetInt16(ordinal));

            ordinal = dataReader.GetOrdinal("VHostID");
            if (!dataReader.IsDBNull(ordinal) && dataReader.GetInt32(ordinal) != 0)
                instance.VirtualizationConfiguration = new VirtualizationConfiguration((string)dataReader["VmUID"],
                                                                                       (string)dataReader["VmName"],
                                                                                       (string)dataReader["VmDomainName"],
                                                                                       (int)dataReader["VHostID"],
                                                                                       (string)dataReader["VHostName"],
                                                                                       (string)dataReader["VHostAddress"],
                                                                                       (string)dataReader["VCUserName"],
                                                                                       (string)dataReader["VCPassword"],
                                                                                       (string)dataReader["ServerType"]);

            if (dataReader["BaselineTemplate"] != DBNull.Value)
            {
                instance.BaselineConfiguration = new BaselineConfiguration((string)dataReader["BaselineTemplate"]);
                instance.BaselineConfiguration.TemplateID = (int)dataReader["BaselineTemplateID"];
            }

            #region Analysis Configuration
            if (dataReader["AnalysisProductionServer"] != DBNull.Value)
            {
                instance.AnalysisConfiguration = new AnalysisConfiguration(returnId,
                    (bool)dataReader["AnalysisProductionServer"],
                    (bool)dataReader["OLTP"],
                    (DateTime)dataReader["StartTime"],
                    (int)dataReader["Duration"],
                    (short)dataReader["ScheduledDays"],
                    (int)dataReader["IncludeDatabase"],
                    dataReader["IncludeDatabaseName"].ToString(),
                    dataReader["FilterApplication"].ToString()
                    , String.IsNullOrEmpty(dataReader["BlockedCategories"].ToString()) ? new List<int>() : GetINTListFromArray((dataReader["BlockedCategories"].ToString()).Split(','))
                    , String.IsNullOrEmpty(dataReader["BlockedCategoriesWithName"].ToString()) ? new Dictionary<int, string>() : GetDictinaoryFromArray((dataReader["BlockedCategoriesWithName"].ToString()).Split(','))
                    , String.IsNullOrEmpty(dataReader["BlockedDatabases"].ToString()) ? new List<int>() : GetINTListFromArray((dataReader["BlockedDatabases"].ToString()).Split(','))
                    , String.IsNullOrEmpty(dataReader["BlockedDatabasesWithName"].ToString()) ? new Dictionary<int, string>() : GetDictinaoryFromArray((dataReader["BlockedDatabasesWithName"].ToString()).Split(','))
                    , (String.IsNullOrEmpty(dataReader["BlockedRecommendations"].ToString()) ? new List<string>() : GetListFromStringArray((dataReader["BlockedRecommendations"].ToString()).Split(',')))
                    , (bool)dataReader["SchedulingStatus"]);
            }
            #endregion

            ordinal = dataReader.GetOrdinal("DatabaseStatisticsRefreshIntervalInSeconds");
            if (dataReader.IsDBNull(ordinal))
            {
                instance.DatabaseStatisticsInterval = TimeSpan.FromMinutes(60);
            }
            else
            {
                instance.DatabaseStatisticsInterval = TimeSpan.FromSeconds(dataReader.GetInt32(ordinal));
            }

            var wmi = instance.WmiConfig;
            ordinal = dataReader.GetOrdinal("WmiCollectionEnabled");
            wmi.DirectWmiEnabled = dataReader.GetBoolean(ordinal);
            ordinal = dataReader.GetOrdinal("WmiConnectAsService");
            wmi.DirectWmiConnectAsCollectionService = dataReader.GetBoolean(ordinal);
            ordinal = dataReader.GetOrdinal("WmiUserName");
            if (!dataReader.IsDBNull(ordinal))
                wmi.DirectWmiUserName = dataReader.GetString(ordinal);
            ordinal = dataReader.GetOrdinal("WmiPassword");
            if (!dataReader.IsDBNull(ordinal))
                wmi.EncryptedPassword = dataReader.GetString(ordinal);

            return instance;
        }
        // To Get list of blocked Items with their name and id 
        private static Dictionary<int, string> GetDictinaoryFromArray(string[] arrayOfIDNamePair)
        {
            Dictionary<int, string> blockedItems = new Dictionary<int, string>();
            try
            {
                for (int indexSeq = 0; indexSeq < arrayOfIDNamePair.Length; indexSeq = indexSeq + 2)
                {
                    if (indexSeq % 2 == 0)
                    {
                        blockedItems.Add(Convert.ToInt32(arrayOfIDNamePair[indexSeq]), arrayOfIDNamePair[indexSeq + 1].Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message + " Error while getting blocked categories or blocked databases.");
                throw new Exception("Error while getting blocked categories or blocked databases. " + ex.Message);
            }
            return blockedItems;
        }

        //to Convert array of strings to List<int>
        private static List<string> GetListFromStringArray(string[] arrToconvert)
        {
            List<string> list = new List<string>(arrToconvert);
            return list;
        }
        //to Convert array of strings to List<int>
        private static List<int> GetINTListFromArray(string[] arrToconvert)
        {
            List<int> list = new List<int>();
            foreach (string item in arrToconvert)
            {
                list.Add(Convert.ToInt32(item));
            }
            return list;
        }
        internal static IDictionary<string, int> GetServerTags(SQLdmDriveInfo drive, int serverId)
        {
            using (SqlConnection connection = new SqlConnection(drive.RepositoryConnectionString))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetServerTagsStoredProcedure, serverId))
                {
                    SortedDictionary<string, int> tags = new SortedDictionary<string, int>();

                    while (dataReader.Read())
                    {
                        tags.Add(dataReader["TagName"] as string, (int)dataReader["TagId"]);
                    }

                    return tags;
                }
            }
        }

        internal static Dictionary<int, Tag> GetTags(SQLdmDriveInfo drive)
        {
            using (SqlConnection connection = new SqlConnection(drive.RepositoryConnectionString))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetTagsStoredProcedure))
                {
                    Dictionary<int, Tag> tags = new Dictionary<int, Tag>();

                    // Read in all tags
                    while (dataReader.Read())
                    {
                        int tagId = (int)dataReader["Id"];
                        string tagName = dataReader["Name"] as string;
                        tags.Add(tagId, new Tag(tagId, tagName));
                    }

                    // Read in server tags
                    dataReader.NextResult();
                    while (dataReader.Read())
                    {
                        Tag tag;
                        int tagId = (int)dataReader["TagId"];

                        if (tags.TryGetValue(tagId, out tag))
                        {
                            tag.AddInstance((int)dataReader["SQLServerId"]);
                        }
                    }

                    // Read in custom counter tags
                    dataReader.NextResult();
                    while (dataReader.Read())
                    {
                        Tag tag;
                        int tagId = (int)dataReader["TagId"];

                        if (tags.TryGetValue(tagId, out tag))
                        {
                            tag.AddCustomCounter((int)dataReader["Metric"]);
                        }
                    }

                    // Read in permission tags
                    dataReader.NextResult();
                    while (dataReader.Read())
                    {
                        Tag tag;
                        int tagId = (int)dataReader["TagId"];

                        if (tags.TryGetValue(tagId, out tag))
                        {
                            tag.AddPermission((int)dataReader["PermissionId"]);
                        }
                    }

                    return tags;
                }
            }
        }

        public static Triple<bool, IList<int>, IList<int>> GetPermissionTagsAndServers(SQLdmDriveInfo drive, int permissionId)
        {
            using (SqlConnection connection = new SqlConnection(drive.RepositoryConnectionString))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetPermissionTagsAndServersStoredProcedure, permissionId))
                {
                    bool isSecurityEnabled;
                    List<int> tags = new List<int>();
                    List<int> servers = new List<int>();

                    dataReader.Read();
                    isSecurityEnabled = (bool)dataReader[0];

                    dataReader.NextResult();
                    while (dataReader.Read())
                    {
                        tags.Add((int)dataReader[0]);
                    }

                    dataReader.NextResult();
                    while (dataReader.Read())
                    {
                        servers.Add((int)dataReader[0]);
                    }

                    return new Triple<bool, IList<int>, IList<int>>(isSecurityEnabled, tags, servers);
                }
            }
        }

        public static int GetAlertTemplateById(SqlConnectionStringBuilder connectionString, string TemplateName)
        {
            int templateId = 0;
            using (SqlConnection connection = new SqlConnection(connectionString.ToString()))
            {
                connection.Open();
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, "p_GetAlertTemplatesByTemplateName", TemplateName))
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            templateId = (Int32)reader["TemplateID"];
                        }
                    }
                    else
                    {
                        string templateException = string.Format("Template Id not found for TemplateName - {0}", TemplateName);
                        throw new Exception(templateException);
                    }

                }
            }
            return templateId;
        }

        public static Dictionary<int, string> GetInstanceId(SqlConnectionStringBuilder connectionString, List<string> instanceName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString.ToString()))
            {
                connection.Open();
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, "p_GetInstanceSQLServerById", NewParameter("@InstanceName", SqlDbType.NVarChar, string.Join(",", instanceName.ToArray()))))
                {
                    Dictionary<int, string> instanceIds = new Dictionary<int, string>();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            instanceIds.Add(Convert.ToInt32(reader.GetValue(0), CultureInfo.InvariantCulture), Convert.ToString(reader.GetValue(1), CultureInfo.InvariantCulture));
                        }
                    }
                    return instanceIds;
                }
            }
        }

        public static Dictionary<int, string> GetInstanceByTags(SqlConnectionStringBuilder connectionString, List<string> tagsName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString.ToString()))
            {
                Int32 intSQLServer;
                string tagNames = string.Empty;
                Dictionary<int, string> instanceIds = new Dictionary<int, string>();
                connection.Open();

                DataSet ds = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, "p_GetServerInstanceByTags", NewParameter("@TagsName", SqlDbType.NVarChar, string.Join(",", tagsName.ToArray())));

                for (int tagCounter = 0; tagCounter < ds.Tables[0].Rows.Count; tagCounter++)
                {
                    tagNames = "";
                    ds.Tables[0].DefaultView.RowFilter = "SQLServerId = " + ds.Tables[0].Rows[tagCounter].ItemArray[0];
                    DataTable dt = (ds.Tables[0].DefaultView).ToTable();

                    if (dt.Rows.Count > 1)
                    {
                        intSQLServer = Convert.ToInt32(dt.Rows[0].ItemArray[0]);
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            tagNames += dt.Rows[i].ItemArray[1].ToString() + ",";
                        }
                        instanceIds.Add(Convert.ToInt32(intSQLServer), tagNames.Substring(0, tagNames.Length - 1));
                        tagCounter += dt.Rows.Count - 1;
                    }
                    else
                    {
                        tagNames = dt.Rows[0].ItemArray[1].ToString();
                        instanceIds.Add(Convert.ToInt32(dt.Rows[0].ItemArray[0], CultureInfo.InvariantCulture), tagNames);
                    }
                }

                return instanceIds;
            }
        }

        public static SqlParameter NewParameter(string name, SqlDbType dbType, object value)
        {
            SqlParameter p = new SqlParameter(name, dbType);
            p.Value = value ?? DBNull.Value;
            return p;
        }

        public static ICollection<Tag> GetTags(SqlConnectionStringBuilder connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString.ToString()))
            {
                connection.Open();
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, "p_GetTags"))
                {
                    Dictionary<int, Tag> tags = new Dictionary<int, Tag>();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int tagId = (int)reader["Id"];
                            string tagName = reader["Name"] as string;
                            tags.Add(tagId, new Tag(tagId, tagName));
                        }
                    }
                    else
                    {
                        string tagException = string.Format("Tags not found");
                        throw new Exception(tagException);
                    }
                    return new List<Tag>(tags.Values);
                }
            }
        }
    }
}