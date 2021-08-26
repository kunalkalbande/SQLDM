using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.Status;
using Idera.SQLdm.Common.Thresholds;
using Microsoft.ApplicationBlocks.Data;
using NUnit.Framework;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.CoreServiceTest
{
    class CoreServiceHelper
    {
        public const string ConnectionStringApplicationNamePrefix = "SQL Diagnostic Manager";
        public const string CollectionServceConnectionStringApplicationName =
            ConnectionStringApplicationNamePrefix + " Collection Service";

        internal static MonitoredSqlServer GetMonitoredSqlServer(string repositoryConnectionString, int instanceId)
        {
            MonitoredSqlServer instance = null;
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (
                SqlDataReader reader =
                    SqlHelper.ExecuteReader(repositoryConnectionString,
                                            "p_GetMonitoredSqlServerById",
                                            instanceId,
                                            false))
            {
                if (reader.Read())
                {
                    instance = ConstructMonitoredSqlServer(reader);

                    // Custom Counters
                    if (reader.NextResult() && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            instance.CustomCounters.Add((int)reader[0]);
                        }
                    }

                    // Tags
                    if (reader.NextResult() && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            instance.Tags.Add((int)reader[0]);
                        }
                    }
                }
            }

            return instance;
        }

        private static MonitoredSqlServer ConstructMonitoredSqlServer(SqlDataReader dataReader)
        {
            if (dataReader == null)
            {
                throw new ArgumentNullException("dataReader");
            }

            int returnId = (int)dataReader["SQLServerID"];
            SqlConnectionInfo instanceConnectionInfo = new SqlConnectionInfo();
            instanceConnectionInfo.ApplicationName = CollectionServceConnectionStringApplicationName;
            instanceConnectionInfo.InstanceName = dataReader["InstanceName"] as string;
            bool isActive = (bool)dataReader["Active"];
            DateTime registeredDate = (DateTime)dataReader["RegisteredDate"];
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
                dataReader.IsDBNull(dataReader.GetOrdinal("QueryMonitorStopTimeUTC")) ? null : (DateTime?)dataReader["QueryMonitorStopTimeUTC"],
                false, false, false,    //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended event sessions --  Added default values of parameters            
                (int)dataReader["QueryMonitorTopPlanCountFilter"],
                (int)dataReader["QueryMonitorTopPlanCategoryFilter"],
                (bool)dataReader["QueryMonitorQueryStoreMonitoringEnabled"]);    // SQLdm 10.4 (Varun Chopra) Query Monitor using Query Store

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
                dataReader.IsDBNull(dataReader.GetOrdinal("QueryMonitorStopTimeUTC")) ? null : (DateTime?)dataReader["QueryMonitorStopTimeUTC"], true);//SQLdm 9.1 (Ankit Srivastava) Activity Monitoring with Extended Events - added newly added parameter


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
            // new fields added for maintenanceMode
            if (dataReader.IsDBNull(dataReader.GetOrdinal("MaintenanceModeMonth")) == false)
            {
                maintenanceMode.MaintenanceModeMonth = (int)dataReader["MaintenanceModeMonth"];
            }
            if (dataReader.IsDBNull(dataReader.GetOrdinal("MaintenanceModeSpecificDay")) == false)
            {
                maintenanceMode.MaintenanceModeSpecificDay = (int)dataReader["MaintenanceModeSpecificDay"];
            }
            if (dataReader.IsDBNull(dataReader.GetOrdinal("MaintenanceModeWeekOrdinal")) == false)
            {
                maintenanceMode.MaintenanceModeWeekOrdinal = (int)dataReader["MaintenanceModeWeekOrdinal"];
            }
            if (dataReader.IsDBNull(dataReader.GetOrdinal("MaintenanceModeWeekDay")) == false)
            {
                maintenanceMode.MaintenanceModeWeekDay = (int)dataReader["MaintenanceModeWeekDay"];
            }
            if (dataReader.IsDBNull(dataReader.GetOrdinal("MaintenanceModeMonthDuration")) == false)
            {
                maintenanceMode.MaintenanceModeMonthDuration = TimeSpan.FromSeconds((int)dataReader["MaintenanceModeMonthDuration"]);
            }
            if (dataReader.IsDBNull(dataReader.GetOrdinal("MaintenanceModeMonthRecurringStart")) == false)
            {
                maintenanceMode.MaintenanceModeMonthRecurringStart = (DateTime)dataReader["MaintenanceModeMonthRecurringStart"];
            }
            if (dataReader.IsDBNull(dataReader.GetOrdinal("MaintenanceModeOnDemand")) == false)
            {
                maintenanceMode.MaintenanceModeOnDemand = (bool)dataReader["MaintenanceModeOnDemand"];
            }

            MonitoredSqlServer instance =
                new MonitoredSqlServer(returnId, registeredDate, isActive, collectionServiceId,
                                       instanceConnectionInfo,
                                       TimeSpan.FromSeconds(scheduledCollectionInterval),
                                       maintenanceModeEnabled,
                                       queryMonitorConfiguration,
                                       activityMonitorConfiguration,
                                       maintenanceMode);
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
                    XmlSerializer serializer =
                        Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(string[]));

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
                catch (Exception)
                {
                    throw new Exception("Exception deserializing TableStatisticsExcludedDatabases");
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


            instance.TableFragmentationConfiguration.FragmentationMinimumTableSize.Kilobytes = (int)dataReader["ReorgMinTableSizeKB"];

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
            //LOG.Info(instance.InstanceName, " Ole Automation context is ", instance.OleAutomationContext.ToString());

            ordinal = dataReader.GetOrdinal("DisableExtendedHistoryCollection");
            if (!dataReader.IsDBNull(ordinal))
                instance.ExtendedHistoryCollectionDisabled = (bool)dataReader["DisableExtendedHistoryCollection"];

            ordinal = dataReader.GetOrdinal("DisableOleAutomation");
            if (!dataReader.IsDBNull(ordinal))
                instance.OleAutomationUseDisabled = (bool)dataReader["DisableOleAutomation"];
            else
                instance.OleAutomationUseDisabled = false;  // set this explicitly

            //if (instance.OleAutomationUseDisabled)
            //    LOG.Info(instance.InstanceName, " use of Ole Automation is disabled");

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
                    throw new Exception("Exception deserializing DiskCollectionSettings", e);
                }
            }

            ordinal = dataReader.GetOrdinal("InputBufferLimiter");
            if (!dataReader.IsDBNull(ordinal))
                instance.InputBufferLimiter = dataReader.GetInt32(ordinal);

            ordinal = dataReader.GetOrdinal("InputBufferLimited");
            if (!dataReader.IsDBNull(ordinal))
                instance.InputBufferLimited = dataReader.GetBoolean(ordinal);

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

            if (dataReader["ServerVersion"] != DBNull.Value)
                instance.MostRecentSQLVersion = new ServerVersion((string)dataReader["ServerVersion"]);
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
                    ,(bool)dataReader["SchedulingStatus"]);
            }
            #endregion

            if (dataReader["ServerEdition"] != DBNull.Value)
                instance.MostRecentSQLEdition = (string)dataReader["ServerEdition"];


            ordinal = dataReader.GetOrdinal("JobAlertEnabled");
            if (dataReader.IsDBNull(ordinal))
            {
                instance.JobAlerts = false;
            }
            else
            {
                instance.JobAlerts = dataReader.GetBoolean(ordinal);
            }

            ordinal = dataReader.GetOrdinal("LogAlertEnabled");
            if (dataReader.IsDBNull(ordinal))
            {
                instance.ErrorlogAlerts = false;
            }
            else
            {
                instance.ErrorlogAlerts = dataReader.GetBoolean(ordinal);
            }

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

            if (dataReader["SS_RelatedInstanceId"] != DBNull.Value &&
                !string.IsNullOrEmpty(((string)dataReader["SS_InstanceName"])))
            {
                instance.SQLsafeConfig = new SQLsafeRepositoryConfiguration
                                             (
                                                (string)dataReader["SS_InstanceName"],
                                                (string)dataReader["SS_DatabaseName"],
                                                (bool)dataReader["SS_SecurityMode"],
                                                (string)dataReader["SS_UserName"],
                                                (string)dataReader["SS_EncryptedPassword"],
                                                (int)dataReader["SS_RelatedInstanceId"],
                                                (int)dataReader["SS_LastBackupActionId"],
                                                (int)dataReader["SS_LastDefragActionId"]
                                             );
            }


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

        internal static MonitoredServerWorkload GetMonitoredServerWorkload(MonitoredSqlServer monitoredSqlServer, string connectionString)
        {

            MonitoredServerWorkload workload = new MonitoredServerWorkload();
            workload.MonitoredServer = monitoredSqlServer;

            IDictionary<int, List<MetricThresholdEntry>> thresholds = null;
            IList<OutstandingEventEntry> events = null;
            Set<int> counters = null;
            Dictionary<int, bool> metricCompatibilityForSqlExpress = null; //SQLdm 8.6 -- (Ankit Srivastava) -- all of metrics along with their compatibility for Sql Express

            GetMonitoredSqlServerWorkload(
                connectionString,
                monitoredSqlServer.Id,
                out thresholds,
                out events,
                out counters,
                out metricCompatibilityForSqlExpress
                );

            workload.ThresholdInstances = thresholds;
            workload.OutstandingEvents = events;
            workload.CustomCounters.AddRange(counters);
            workload.MetricCompatibilityForSqlExpress = metricCompatibilityForSqlExpress;

            return workload;

        }

        //SQLdm 8.6 -- (Ankit Srivastava) -- added one more parameters which contains all of metrics along with their compatibility for Sql Express
        private static void GetMonitoredSqlServerWorkload(string repositoryConnectionString, int serverId, out IDictionary<int, List<MetricThresholdEntry>> thresholds, out IList<OutstandingEventEntry> events, out Set<int> counters, out Dictionary<int, bool> metricCompatibilityForSqlExpress)
        {


            thresholds = new Dictionary<int, List<MetricThresholdEntry>>();
            events = new List<OutstandingEventEntry>();
            counters = new Set<int>();
            metricCompatibilityForSqlExpress = new Dictionary<int, bool>(); //SQLdm 8.6 -- (Ankit Srivastava) -- all of metrics along with their compatibility for Sql Express


            using (SqlDataReader reader =
                SqlHelper.ExecuteReader(repositoryConnectionString, "p_GetMonitoredSQLServerWorkload", serverId))
            {
                GetMetricThresholds(reader, thresholds);
                //                if (reader.NextResult())
                //                    GetOutstandingEvents(reader, events);
                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        counters.Add(reader.GetInt32(1));
                    }
                }
                if (reader.NextResult()) //SQLdm 8.6 -- (Ankit Srivastava) -- reading all of metrics along with their compatibility for Sql Express  from the datareader
                {
                    while (reader.Read())
                    {
                        metricCompatibilityForSqlExpress.Add(reader.GetInt32(0), reader.GetBoolean(1));
                    }
                }
            }

        }

        private static IList<MetricThresholdEntry> GetMetricThresholds(SqlDataReader reader)
        {
            List<MetricThresholdEntry> result = new List<MetricThresholdEntry>();
            while (reader.Read())
            {
                // get the monitored server (parent)
                int sqlServerId = reader.GetInt32(0);

                // get the metric
                //                Metric metric = (Metric)Enum.ToObject(typeof(Metric), reader.GetInt32(1));
                int metricID = reader.GetInt32(1);

                // threshold type name                
                Threshold warning = null;
                Threshold critical = null;
                Threshold info = null;
                object data = null;

                // get the warning threshold
                SqlString warningXml = reader.GetSqlString(3);
                if (!warningXml.IsNull)
                {
                    warning = Threshold.Deserialize(warningXml.Value);
                }

                // get the critical threshold
                SqlString criticalXml = reader.GetSqlString(4);
                if (!criticalXml.IsNull)
                {
                    critical = Threshold.Deserialize(criticalXml.Value);
                }

                // get the info threshold
                SqlString infoXml;
                if (reader.FieldCount > 9)
                    infoXml = reader.GetSqlString(10);
                else
                    infoXml = reader.GetSqlString(6);
                if (!infoXml.IsNull)
                {
                    info = Threshold.Deserialize(infoXml.Value);
                }

                SqlString dataXml = reader.GetSqlString(5);
                if (!dataXml.IsNull)
                {
                    data = Threshold.DeserializeData(dataXml.Value);
                }

                string thresholdInstanceName;
                if (reader.IsDBNull(reader.FieldCount > 9 ? 11 : 7))
                    thresholdInstanceName = String.Empty;
                else
                    thresholdInstanceName = reader.GetString(reader.FieldCount > 9 ? 11 : 7);

                MetricThresholdEntry entry = new MetricThresholdEntry(sqlServerId, metricID, thresholdInstanceName, warning, critical, info);//SQLdm 10.0 (Tarun Sapra)- Passing null params for the baselined thresholds
                entry.Data = data;

                if (reader.FieldCount > 9)
                {
                    // attach snooze info to advanced alert configuration settings
                    if (!(reader.IsDBNull(6) || reader.IsDBNull(7)))
                    {
                        AdvancedAlertConfigurationSettings settings = data as AdvancedAlertConfigurationSettings;
                        if (settings == null)
                        {
                            settings = new AdvancedAlertConfigurationSettings(MetricDefinition.GetMetric(metricID), data);
                            entry.Data = settings;
                        }
                        string startUser = reader.IsDBNull(8) ? String.Empty : reader.GetString(8);
                        string endUser = reader.IsDBNull(9) ? String.Empty : reader.GetString(9);
                        SnoozeInfo snoozeInfo = new SnoozeInfo(reader.GetDateTime(6), reader.GetDateTime(7), startUser, endUser);
                        settings.SnoozeInfo = snoozeInfo;
                    }
                }

                if (!reader.GetBoolean(2))
                    entry.IsEnabled = false;

                entry.IsThresholdEnabled = Convert.ToBoolean(reader["ThresholdEnabled"]);

                result.Add(entry);
            }
            return result;
        }

        private static void GetMetricThresholds(SqlDataReader reader, IDictionary<int, List<MetricThresholdEntry>> thresholds)
        {
            foreach (MetricThresholdEntry thresholdEntry in GetMetricThresholds(reader))
            {
                if (thresholds.ContainsKey(thresholdEntry.MetricID))
                    thresholds[thresholdEntry.MetricID].Add(thresholdEntry);
                else
                    thresholds.Add(thresholdEntry.MetricID, new List<MetricThresholdEntry>() { thresholdEntry });
            }
        }

        /// <summary>
        /// Gets the service object by the connection info
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionInfo"></param>
        /// <returns></returns>
        internal static T GetDefaultService<T>(SqlConnectionInfo connectionInfo)
        {

            ManagementServiceConfiguration serviceConfig;

            serviceConfig = GetDefaultServiceConfig(connectionInfo);
            if (serviceConfig == null)
            {
                throw new ApplicationException("No management services have been registered with the SQLdm repository.");
            }
            else
            {
                T svc = GetService<T>(serviceConfig);

                return svc;
            }

        }

        /// <summary>
        /// Gets the ManagementServiceConfiguration object by connectionInfo
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <returns></returns>
        private static ManagementServiceConfiguration GetDefaultServiceConfig(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo not found.");
            }

            string connectionString = @"Data Source=ACCOLITE-PC\MSSQLSERVER12;Initial Catalog=SQLdmDatabase;Integrated Security=SSPI;";
            using (
                SqlConnection connection = new SqlConnection { ConnectionString = connectionString })
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure,
                                                "p_GetDefaultManagementService"))
                {
                    if (!dataReader.HasRows)
                    {
                        return null;
                    }

                    dataReader.Read();

                    string identifier = dataReader["ManagementServiceID"] as string;
                    string machineName = dataReader["MachineName"] as string;
                    string instanceName = dataReader["InstanceName"] as string;
                    string address = dataReader["Address"] as string;
                    int port = (int)dataReader["Port"];

                    return new ManagementServiceConfiguration(identifier, machineName, instanceName, address, port);
                }
            }
        }

        /// <summary>
        /// Gets the Service object from the ManagementServiceConfiguration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceConfig"></param>
        /// <returns></returns>
        internal static T GetService<T>(ManagementServiceConfiguration serviceConfig)
        {
            //return GetService(serviceConfig.Address, serviceConfig.Port);
            if (serviceConfig.Address == null || serviceConfig.Address.Length == 0)
            {
                throw new ArgumentException("The management service host is invalid.", "host");
            }
            Uri uri = new System.Uri("tcp://ACCOLITE-PC:5166/Management");

            if (typeof(T).Name.Equals("IManagementService"))
                uri = new Uri(String.Format("tcp://{0}:{1}/Management", serviceConfig.Address, serviceConfig.Port));

            if (typeof(T).Name.Equals("ICollectionService"))
                uri = new Uri(String.Format("tcp://{0}:{1}/Collection", serviceConfig.Address, 5167));


            //IManagementService  ims= (IManagementService) Activator.GetObject(typeof(IManagementService),uri.ToString());
            T svc = (T)Activator.GetObject(typeof(T), uri.ToString());

            //ServiceCallProxy proxy = new ServiceCallProxy(typeof(IManagementService), uri.ToString());
            //IManagementService ims = proxy.GetTransparentProxy() as IManagementService;

            return svc;
        }
    }
}

