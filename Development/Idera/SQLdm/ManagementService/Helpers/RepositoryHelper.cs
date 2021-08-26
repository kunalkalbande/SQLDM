using Idera.SQLdm.Common.Events.AzureMonitor;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;

namespace Idera.SQLdm.ManagementService.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Xml;
    using System.Xml.Serialization;
    using Idera.SQLdm.Common;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Messages;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.Status;
    using Idera.SQLdm.Common.Thresholds;
    using Idera.SQLdm.Common.VMware;
    using Idera.SQLdm.ManagementService.Notification;
    using Idera.SQLdm.ManagementService.Configuration;
    using Microsoft.ApplicationBlocks.Data;
    using System.Text;
    using Monitoring;
    using Wintellect.PowerCollections;
    using Idera.SQLdm.Common.Objects.Replication;
    using Idera.SQLdm.Common.Auditing;
    using Idera.SQLdm.Common.CWFDataContracts;
    using System.Security.Principal;
    using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;
    using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Resources;
    using System.Xml.XPath;
    using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Values;
    using Common.Notification;
    using Idera.SQLdm.Common.Configuration;
    using System.Linq;
    using System.Globalization;
    using Common.Recommendations;

    internal static partial class RepositoryHelper
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("RepositoryHelper");
        private static BBS.TracerX.Logger StartUpTimeLog = BBS.TracerX.Logger.GetLogger("StartUpTimeLog");
        #region Constants

        private const string AddInitialDefaultAlertTemplateStoredProcedure = "p_AddDefaultAlertTemplate";
        private const string GetRepositoryVersionSqlCommand = "select dbo.fn_GetDatabaseVersion()";
        private const string AddManagementServiceStoredProcedure = "p_AddManagementService";
        private const string AddMonitoredSqlServerStoredProcedure = "p_AddMonitoredSqlServer";
        private const string UpdateMonitoredSqlServerStoredProcedure = "p_UpdateMonitoredSqlServer";
        private const string DeleteMonitoredSqlServerStoredProcedure = "p_DeleteMonitoredSqlServer";
        private const string GetCollectionServiceForSQLServerStoredProcedure = "p_GetCollectionServiceForSQLServer";
        private const string ActivateMonitoredSqlServerStoredProcedure = "p_DeactivateMonitoredSqlServer";
        private const string DeactivateMonitoredSqlServerStoredProcedure = "p_DeactivateMonitoredSqlServer";
        private const string GetMetricThresholdsCommand = "p_GetMetricThresholds";
        private const string GetMonitoredSqlServersCommand = "p_GetMonitoredSqlServers";
        private const string GetMonitoredSqlServerByIdCommand = "p_GetMonitoredSqlServerById";
        private const string GetVirtualHostServersByAddressCommand = "p_GetVirtualHostServersByAddress";
        private const string GetOutstandingEventsStoredProcedure = "p_GetOutstandingEvents";
        private const string GetMonitoredSqlServerIdCommand = "p_GetMonitoredSqlServerId";
        private const string GetMonitoredSqlServerNamesCommand = "p_GetMonitoredSqlServerNames";
        private const string GetMonitoredSqlServerWorkloadStoredProcedure = "p_GetMonitoredSQLServerWorkload";
        private const string UpdateTaskStoredProcedure = "p_UpdateTask";
        private const string DeleteTasksStoredProcedure = "p_DeleteTasks";
        private const string AddAlertInstanceTemplateStoredProcedure = "p_AddAlertInstanceTemplate";
        private const string GetCollectionServicesStoredProcedure = "p_GetCollectionServices";
        private const string AddCollectionServicesStoredProcedure = "p_AddCollectionServices";
        private const string GetManagementServicesStoredProcedure = "p_GetManagementServices";
        private const string GetMetricInfoStoredProcedure = "p_GetMetricInfo";
        private const string UpdateMetricInfoStoredProcedure = "p_UpdateMetricInfo";
        private const string DeleteThresholdInstanceStoredProcedure = "p_DeleteMetricThresholdInstance";
        private const string AddThresholdInstanceStoredProcedure = "p_AddMetricThresholdInstance";
        private const string UpdateMetricThresholdStoredProcedure = "p_UpdateMetricThreshold";
        private const string GetMonitoredSqlServerStatusStoredProcedure = "p_GetMonitoredSqlServerStatus";
        private const string UpdateLastRefreshTimeStoredProcedure = "p_UpdateLastRefreshTime";
        private const string UpdateLastDatabaseRefreshTimeStoredProcedure = "p_UpdateLastDatabaseRefreshTime";

        private const string AddAdvanceFilteringAlert = "p_AddAdvanceFilteringAlert";
        private const string AddAuditableEventStoredProcedure = "p_AddAuditableEvent";
        private const string GetAuditableEventsStoredProcedure = "p_GetAllAuditableEvents";
        private const string GetAWSResourceDetailsStoredProcedure = "p_GetAWSResourceDetails";

        private const string GetMaxAlertIDProcedure = "p_GetMaxAlertID";
        private const string ClearAlertsProcedure = "p_ClearAlerts";
        private const string AddCounterProcedure = "p_AddCounter";
        private const string UpdateCounterProcedure = "p_UpdateCounter";
        private const string UpdateCounterStatusProcedure = "p_UpdateCounterStatus";
        private const string DeleteCounterProcedure = "p_DeleteCounter";

        private const string AssignCounterToServersProcedure = "p_AssignCounterToServers";
        private const string AssignCountersToServerProcedure = "p_AssignCountersToServer";
        private const string SnoozeAlertsProcedure = "p_SnoozeAlerts";
        private const string UnsnoozeAlertsProcedure = "p_UnsnoozeAlerts";

        private const string DeleteTagsByIdStoredProcedure = "p_DeleteTagsById";
        private const string UpdateTagConfigurationStoredProcedure = "p_UpdateTagConfiguration";
        private const string GetTagAssociationsStoredProcedure = "p_GetTagAssociations";
        private const string UpdateServerTagsStoredProcedure = "p_UpdateServerTags";
        private const string GetServersUsingCustomCounterProcedure = "p_GetMonitoredSqlServersUsingCounter";

        private const string MirroringSetPreferredConfigProcedure = "p_SetMirroringPreferredConfig";
        private const string MirroringGetPreferredConfigProcedure = "p_GetMirroringPreferredConfig";
        private const string MirroringGetParticipantsForServer = "p_GetMirroringParticipantsForServer";
        private const string MirroringDeleteSessionFromServer = "p_DeleteMirroringSessionFromServer";
        private const string MirroringDeletePreferredConfig = "p_DeleteMirroringPreferredConfig";

        private const string PostInstallUpgradeProcedure = "p_PostInstallUpgrade";

        private const string ReplicationGetParticipantsForServer = "p_GetReplicationParticipantsForServer";
        private const string ReplicationDeleteSessionFromServer = "p_DeleteReplicationSessionFromServer";

        private const string DeleteCountersFromCustomReport = "p_DeleteCountersFromCustomReport";
        private const string InsertCustomReportCounters = "p_InsertCustomReportsCounters";
        private const string DeleteCustomReportFromRepository = "p_DeleteCustomReport";
        private const string UpdateOrCreateCustomReport = "p_UpdateCustomReport";
        private const string GetCustomReportName = "p_GetCustomReportName";

        private const string GetExcludedWaitTypesStoredProcedure = "p_GetExcludedWaitTypes";

        private const string GetDiskDrives = "select distinct DriveName from dbo.DiskDrives where SQLServerID = {0}";

        private const string GetSQLText = "p_GetSQLText";

        private const string GetTagsStoredProcedure = "p_GetTags";

        private const string GetGroomingStatusInfo = "p_GetGroomingStatusInfo";// SQLdm 9.0 -(Ankit Srivastava) - added new procedure to get if the grooming timed out or not

        private const string AddTheProductRegistrationInformation = "p_AddTheProductRegistrationInformation"; // SQLdm 9.0 -(Abhishek Joshi) - added new procedure to update the web frameworks registration information
        private const string GetTheProductRegistrationInformation = "p_GetTheProductRegistrationInformation"; // SQLdm 9.0 -(Abhishek Joshi) - added new procedure to get the webframeworks registration information
        private const string GetRecommendationsFromDBStoreProcedure = "p_GetRecommendations"; // SQLdm 10.0 -(Srishti Purohit) - added new procedure to get recommendations for specific analysis
        private const string GetAnalysisListingFromDBStoreProcedure = "p_GetAnalysisListing";
        //private const string SaveRecommendationsInDBStoreProcedure = "p_SaveAnalysisRecords"; // SQLdm 10.0 -(Srishti Purohit) - added new procedure to save recommendations for specific analysis
        private const string GetMasterRecommendationsStoreProcedure = "p_GetMasterRecommendations"; // SQLdm 10.0 -(Srishti Purohit) - added new procedure to get all master records for recommendations from DB
        private const string SavePrescriptiveAnalysisStoreProcedure = "p_SavePrescriptiveAnalysis"; // SQLdm 10.0 -(Srishti Purohit) - added new procedure to save analysis entry in [PrescriptiveAnalysis] for specific analysis
        private const string SavePrescriptiveAnalysisDetailsStoreProcedure = "p_SavePrescriptiveAnalysisDetails"; // SQLdm 10.0 -(Srishti Purohit) - added new procedure to save analysis entry in [PrescriptiveAnalysisDetails] for specific analysis
        private const string SaveAnalysisRecommendationAndPropertiesStoreProcedure = "p_SaveAnalysisRecommendationAndProperties"; // SQLdm 10.0 -(Srishti Purohit) - added new procedure to save analysis entry in [PrescriptiveAnalysisRecommendation] for specific analysis
        
        private const string UpdateBlockedRecommendationDatabaseAnalysisConfiguration = "p_UpdateBlockedRecommendationDatabaseAnalysisConfiguration";// SQLdm 10.0 -(Srishti Purohit) - added new procedure to update blocked databases and recommendations for specific analysis configuration
        private const string UpdateRecommendationOptimizationStatusStoreProcedure = "p_UpdateRecommendationOptimizationStatus";// SQLdm 10.0 -(Srishti Purohit) - added new procedure to update blocked databases and recommendations for specific analysis configuration

        private const string GetPrescriptiveAnalysisSchedule = "p_GetPrescriptiveAnalysisSchedule";// SQLdm 10.0 -(Praveen Suhalka) - added new procedure to get Prescriptive Analysis Scheduling info

        private const string GetPrescriptiveSnapshotValuesForServerStoreProcedure = "p_GetPrescriptiveSnapshotValuesForServer";
        private const string SavePrescriptiveSnapshotValuesForServerStoreProcedure = "p_UpsertPrescriptiveSnapshotValues";
        private const string UpdateSCOMAlertsEventStoreProcedure = "p_UpdateSCOMAlertsEvent";
   
        //private const string GetBaselineFlagForAllAlertableMetricsCommand = "select Metric,IsBaselineEnabled from MetricThresholds where SQLServerID = {0}";//SQLdm 10.0 (Tarun Sapra)- Alert msg when baseline alerts are enabled

        #endregion

        #region Tests

        public static bool TestRepositoryConnection(string repositoryConnectionString)
        {
            return TestRepositoryConnection(repositoryConnectionString, null);
        }

        public static bool TestRepositoryConnection(string repositoryConnectionString, EventLog eventLog)
        {
            if (String.IsNullOrEmpty(repositoryConnectionString))
            {
                string message = "Repository connection string is null or empty.";
                LOG.Debug(message);
                if (eventLog != null)
                    MainService.WriteEvent(eventLog, EventLogEntryType.Error, Status.ErrorRepositoryTestFailed,
                                           Category.General, message);
                return false;
            }

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                return TestRepositoryConnection(connection, eventLog);
            }
        }

        public static bool TestRepositoryConnection(SqlConnection connection, EventLog eventLog)
        {
            try
            {
                if (!IsValidRepository(connection))
                {
                    if (eventLog != null)
                    {
                        MainService.WriteEvent(eventLog, EventLogEntryType.Error, Status.ErrorRepositoryTestFailed,
                                               Category.General,
                                               "Repository does not appear to be valid for this version of SQLdm.");
                    }
                    return false;
                }
                else
                {
                    LOG.Debug("Successful connection to repository server established.");
                    return true;
                }
            }
            catch (Exception e)
            {
                LOG.Error("Connection to repository failed", e);
                if (eventLog != null)
                {
                    MainService.WriteEvent(eventLog, EventLogEntryType.Error, Status.ErrorRepositoryTestFailed, Category.General, "Connection to repository failed: " + e.Message);
                }
                return false;
            }
        }

        public static bool IsValidRepository(SqlConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            try
            {
                string repositorySchema =
                    (string)SqlHelper.ExecuteScalar(connection, CommandType.Text, GetRepositoryVersionSqlCommand);
                bool IsValid = repositorySchema == Constants.ValidRepositorySchemaVersion;
                if (!IsValid)
                    LOG.Error(String.Format("Invalid repository version.  Reported version is {0}, expected version is {1}", repositorySchema, Constants.ValidRepositorySchemaVersion));
                return IsValid;
            }
            catch (SqlException e)
            {
                // Assuming that a valid connection can be established to the SQL Server, 
                // an invalid call to the version function would indicate an invalid database;
                // all other exceptions will be passed on.
                //
                // Error 208 = is invalid object in SQL Server 2000
                // Error 4121 - invalid object in SQL Server 2005
                //
                if (e.Number == 208 || e.Number == 4121)
                {
                    LOG.Error("Unable to read repository version.");
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        #endregion

        #region Management Service

        public static ManagementServiceInfo AddManagementService(string repositoryConnectionString,
                                                                 string instanceName,
                                                                 string machineName,
                                                                 string address,
                                                                 int port)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            if (String.IsNullOrEmpty(instanceName))
                instanceName = "Default";
            if (String.IsNullOrEmpty(machineName))
                machineName = Environment.MachineName;
            if (String.IsNullOrEmpty(address))
            {
                try
                {
                    address = Dns.GetHostName();
                }
                catch (Exception e)
                {
                    LOG.Debug("Error retrieving host name", e);
                    address = machineName;
                }
            }
            if (port == 0)
                port = 5166;

            ManagementServiceInfo info = null;

            // Check to see if any notification rules exist.
            bool notificationRulesExist = NotificationManager.DoNotificationRulesExist(repositoryConnectionString);

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, AddManagementServiceStoredProcedure)
                        )
                    {
                        command.Transaction = transaction;
                        SqlHelper.AssignParameterValues(command.Parameters,
                                                        instanceName,
                                                        machineName,
                                                        address,
                                                        port,
                                                        DBNull.Value);

                        command.ExecuteNonQuery();

                        LOG.DebugFormat("Management service record added to repository for {0}\\{1}",
                                        machineName,
                                        instanceName);

                        SqlParameter returnValue = command.Parameters["@ReturnServiceId"];
                        object value = returnValue.Value;
                        info = new ManagementServiceInfo();
                        info.Id = (Guid)value;
                        info.InstanceName = instanceName;
                        info.MachineName = machineName;
                        info.Address = address;
                        info.Port = port;

                        // add default alerts to the database
                        IList<MetricThresholdEntry> startingAlerts = NotificationManager.GetStartingMetricThresholds(0);
                        try
                        {
                            AddDefaultMetricThresholdEntries(transaction, startingAlerts, true);
                        }
                        catch (Exception ex)
                        {
                            // could happen as a result of adding a second management service
                            LOG.Debug("AddManagementService: Exception " + ex.Message);

                        }

                        // add out-of-the-box notification rules, if no rules exist.
                        if (!notificationRulesExist)
                        {
                            NotificationManager.InsertStartingNotificationRules(transaction);
                        }

                        transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                }
            }

            return info;
        }

        public static IList<ManagementServiceInfo> GetManagementServices(string repositoryConnectionString, Guid? managementServiceId)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            List<ManagementServiceInfo> result = new List<ManagementServiceInfo>();
            object serviceId = (managementServiceId.HasValue) ? (object)managementServiceId.Value : DBNull.Value;

            using (SqlDataReader reader = SqlHelper.ExecuteReader(
                repositoryConnectionString,
                GetManagementServicesStoredProcedure,
                serviceId
                ))
            {
                while (reader.Read())
                {
                    ManagementServiceInfo info = new ManagementServiceInfo();
                    info.Id = reader.GetGuid(0);
                    info.InstanceName = reader.GetString(1);
                    info.MachineName = reader.GetString(2);
                    info.Address = reader.GetString(3);
                    info.Port = reader.GetInt32(4);

                    SqlGuid guid = reader.GetSqlGuid(5);
                    if (guid.IsNull)
                        info.DefaultCollectionServiceID = null;
                    else
                        info.DefaultCollectionServiceID = guid.Value;

                    result.Add(info);
                }
            }

            return result;
        }

        #endregion

        #region Collection Service

        public static void AddCollectionService(string repositoryConnectionString, CollectionServiceInfo collectionService)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                connection.Open();

                SqlParameter[] parameters = SqlHelperParameterCache.GetSpParameterSet(connection, AddCollectionServicesStoredProcedure, true);
                parameters[1].Value = collectionService.InstanceName;
                parameters[2].Value = collectionService.MachineName;
                parameters[3].Value = collectionService.Address;
                parameters[4].Value = collectionService.Port;
                parameters[5].Value = collectionService.Enabled;
                parameters[6].Value = collectionService.ManagementService != null
                                          ? (object)collectionService.ManagementService.Id
                                          : (object)DBNull.Value;
                parameters[7].Value = DBNull.Value;

                using (SqlCommand command = new SqlCommand(AddCollectionServicesStoredProcedure, connection))
                {
                    command.CommandTimeout = SqlHelper.CommandTimeout;
                    command.Parameters.AddRange(parameters);
                    command.CommandType = CommandType.StoredProcedure;
                    int rc = command.ExecuteNonQuery();
                    object id = command.Parameters[7].Value;
                    if (id is Guid)
                    {
                        collectionService.Id = (Guid)id;
                        LOG.InfoFormat(
                            "Collection service saved: InstanceName={0},MachineName={1},Id={2}",
                            collectionService.InstanceName,
                            collectionService.MachineName,
                            collectionService.Id);
                    }
                    else
                    {
                        LOG.InfoFormat(
                            "Failed to save collection service: InstanceName={0},MachineName={1},Address={2},Port={3},rc={4},returnValue={5}",
                            collectionService.InstanceName,
                            collectionService.MachineName,
                            collectionService.Address,
                            collectionService.Port,
                            rc,
                            parameters[0].Value);
                    }
                }
            }
        }

        public static bool GetCollectionServiceIdForServer(string repositoryConnectionString, int instanceId,
                                                           out Guid? collectionServiceId, out string instanceName)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            bool result = false;

            SqlParameter[] parameters =
                SqlHelperParameterCache.GetSpParameterSet(repositoryConnectionString,
                                                          GetCollectionServiceForSQLServerStoredProcedure, true);
            parameters[1].Value = instanceId;
            parameters[2].Value = DBNull.Value;
            parameters[3].Value = DBNull.Value;

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure,
                                          GetCollectionServiceForSQLServerStoredProcedure, parameters);
                object value = parameters[2].Value;
                if (value is Guid)
                    collectionServiceId = (Guid)value;
                else
                    collectionServiceId = null;

                instanceName = parameters[3].Value as string;
            }

            return result;
        }

        /// <summary>
        /// Return a list of collection services.  Be aware that the returned objects do not
        /// have the ManagementService property set so you shouldn't call this method without
        /// passing in a known management service id.    
        /// </summary>
        /// <param name="repositoryConnectionString"></param>
        /// <param name="collectionServiceID"></param>
        /// <param name="managementServiceID"></param>
        /// <returns></returns>
        public static List<CollectionServiceInfo> GetCollectionServices(string repositoryConnectionString, Guid? collectionServiceID, Guid? managementServiceID)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            List<CollectionServiceInfo> result = new List<CollectionServiceInfo>();
            object csid = collectionServiceID.HasValue ? (object)collectionServiceID.Value : null;
            object msid = managementServiceID.HasValue ? (object)managementServiceID.Value : null;

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetCollectionServicesStoredProcedure, csid, msid))
                {
                    while (reader.Read())
                    {
                        CollectionServiceInfo info = new CollectionServiceInfo();
                        info.Id = reader.GetGuid(0);
                        info.InstanceName = reader.GetString(1);
                        info.MachineName = reader.GetString(2);
                        info.Address = reader.GetString(3);
                        info.Port = reader.GetInt32(4);
                        info.Enabled = reader.GetBoolean(5);

                        SqlDateTime sdt = reader.GetSqlDateTime(6);
                        if (!sdt.IsNull)
                            info.LastHeartbeatReceived = sdt.Value;

                        result.Add(info);
                    }
                }
            }

            return result;
        }

        #endregion

        #region Metrics
        public static void AddDefaultMetricThresholdEntries(string repositoryConnectionString, bool allowWhining)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);

                // add default alerts to the database
                IList<MetricThresholdEntry> startingAlerts = NotificationManager.GetStartingMetricThresholds(0);
                try
                {
                    AddDefaultMetricThresholdEntries(transaction, startingAlerts, allowWhining);
                }
                catch
                {
                    // could happen as a result of adding a second management service
                }
                finally
                {
                    transaction.Commit();
                }
            }
        }


        private static void AddDefaultMetricThresholdEntries(SqlTransaction transaction, IEnumerable<MetricThresholdEntry> entries, bool whiningAllowed)
        {
            using (LOG.DebugCall())
            {
                try
                {
                    int templateID = 0;

                    using (SqlCommand command = SqlHelper.CreateCommand(transaction.Connection, AddInitialDefaultAlertTemplateStoredProcedure))
                    {
                        command.Transaction = transaction;

                        SqlHelper.AssignParameterValues(command.Parameters,
                                                        templateID);
                        command.ExecuteNonQuery();

                        SqlParameter outParm = command.Parameters["@templateID"];
                        if (outParm.Value is int)
                            templateID = (int)outParm.Value;

                    }

                    using (SqlCommand command = SqlHelper.CreateCommand(transaction.Connection, "p_AddDefaultMetricThreshold"))
                    {
                        command.Transaction = transaction;
                        Exception lastException = null;

                        foreach (MetricThresholdEntry entry in entries)
                        {
                            string infoXML = null;
                            string warningXml;
                            string criticalXml;
                            string dataXml = null;
                            //Commenting code as sp internally will add default threshold value
                            //string baselineInfoXML = null;
                            //string baselineWarningXml = null;
                            //string baselineCriticalXml = null;
                            Threshold.Serialize(entry.WarningThreshold, out warningXml);
                            Threshold.Serialize(entry.CriticalThreshold, out criticalXml);
                            //if (entry.BaselineWarningThreshold != null)
                            //{
                            //    Threshold.Serialize(entry.BaselineWarningThreshold, out baselineWarningXml);
                            //}
                            //if (entry.InfoThreshold != null)
                            //{
                            //    Threshold.Serialize(entry.BaselineInfoThreshold, out baselineInfoXML);
                            //}
                            //if (entry.InfoThreshold != null)
                            //{
                            //    Threshold.Serialize(entry.BaselineCriticalThreshold, out baselineCriticalXml);
                            //}
                            if (entry.InfoThreshold != null)
                            {
                                Threshold.Serialize(entry.InfoThreshold, out infoXML);
                            }
                            if (entry.Data != null)
                            {
                                Threshold.SerializeData(entry.Data, out dataXml);
                            }
                            try
                            {
                                SqlHelper.AssignParameterValues(command.Parameters,
                                                                templateID,
                                                                //entry.MonitoredServerID,
                                                                entry.MetricID,
                                                                entry.IsEnabled,
                                                                warningXml,
                                                                criticalXml,
                                                                dataXml,
                                                                infoXML
                                    //Commenting code as sp internally will add default threshold value
                                    //baselineWarningXml,
                                    //baselineCriticalXml,
                                    //baselineInfoXML
                                    );

                                command.ExecuteNonQuery();
                            }
                            catch (Exception ie)
                            {
                                lastException = ie;
                            }
                        }
                        if (lastException != null)
                        {
                            if (whiningAllowed)
                                LOG.Error("Exception while adding default metric threshold entries for management service.\nThis could have been caused by reregistering a management service.", lastException);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (whiningAllowed)
                        LOG.Error("Exception while adding default metric threshold entries for management service.\nThis could have been caused by reregistering a management service.", e);
                    throw;
                }
            }
        }

        public static IList<MetricThresholdEntry> GetDefaultMetricThresholds(string repositoryConnectionString, int userViewId)
        {
            return GetMetricThresholds(repositoryConnectionString, userViewId, null, null);
        }

        public static IList<MetricThresholdEntry> GetDefaultMetricThresholds(string repositoryConnectionString, int userViewId, int metric)
        {
            return GetMetricThresholds(repositoryConnectionString, userViewId, null, metric);
        }

        public static IList<MetricThresholdEntry> GetMetricThresholds(string repositoryConnectionString, int serverId)
        {
            return GetMetricThresholds(repositoryConnectionString, null, serverId, null);
        }

        public static MetricThresholdEntry GetMetricThreshold(string repositoryConnectionString, int serverId, int metric)
        {
            IList<MetricThresholdEntry> entries = GetMetricThresholds(repositoryConnectionString, null, serverId, metric);
            return entries.Count > 0 ? entries[0] : null;
        }

        public static IList<MetricThresholdEntry> GetMetricThresholds(string repositoryConnectionString, int? userViewID, int? serverID, int? metric)
        {
            IList<MetricThresholdEntry> result = null;

            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            object uvid = userViewID.HasValue ? (object)userViewID.Value : DBNull.Value;
            object sid = serverID.HasValue ? (object)serverID.Value : DBNull.Value;
            object metricId = metric.HasValue ? (object)((int)metric.Value) : DBNull.Value;

            using (
                SqlDataReader reader =
                    SqlHelper.ExecuteReader(repositoryConnectionString, GetMetricThresholdsCommand, uvid, sid, metricId))
            {
                result = GetMetricThresholds(reader);
            }

            if (result == null)
                result = new List<MetricThresholdEntry>(0);

            return result;
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
                //Commenting below code to fix defect, below code for update SP always gives FieldCount >9 so condition fails
                //SQLdm 10.0 (Srishti Puohit) -- 10.0 Defect fix
                //if (reader.FieldCount > 9)
                //    infoXml = reader.GetSqlString(10);
                //else
                //    infoXml = reader.GetSqlString(6);
                infoXml = reader.GetSqlString(reader.GetOrdinal("InfoThreshold"));
                if (!infoXml.IsNull)
                {
                    info = Threshold.Deserialize(infoXml.Value);
                }

                //START: SQLdm 10.0 (Tarun Sapra)- baselined thresholds
                Threshold warningBaselined = null;
                Threshold criticalBaselined = null;
                Threshold infoBaselined = null;

                SqlString baselinedThresholdXml;
                //Commenting below code to fix defect, below code for update SP always gives FieldCount >9 so condition fails
                //SQLdm 10.0 (Srishti Puohit) -- 10.0 Defect fix
                //if (reader.FieldCount > 13)
                //    baselinedThresholdXml = reader.GetSqlString(16);
                //else
                //    baselinedThresholdXml = reader.GetSqlString(12);
                baselinedThresholdXml = reader.GetSqlString(reader.GetOrdinal("BaselineInfoThreshold"));
                if (!baselinedThresholdXml.IsNull)
                {
                    infoBaselined = Threshold.Deserialize(baselinedThresholdXml.Value);
                }

                //Commenting below code to fix defect, below code for update SP always gives FieldCount >9 so condition fails
                //SQLdm 10.0 (Srishti Puohit) -- 10.0 Defect fix
                //if (reader.FieldCount > 13)
                //    baselinedThresholdXml = reader.GetSqlString(15);
                //else
                //    baselinedThresholdXml = reader.GetSqlString(11);
                baselinedThresholdXml = reader.GetSqlString(reader.GetOrdinal("BaselineCriticalThreshold"));
                if (!baselinedThresholdXml.IsNull)
                {
                    criticalBaselined = Threshold.Deserialize(baselinedThresholdXml.Value);
                }

                //Commenting below code to fix defect, below code for update SP always gives FieldCount >9 so condition fails
                //SQLdm 10.0 (Srishti Puohit) -- 10.0 Defect fix
                //if (reader.FieldCount > 13)
                //    baselinedThresholdXml = reader.GetSqlString(14);
                //else
                //    baselinedThresholdXml = reader.GetSqlString(10);
                baselinedThresholdXml = reader.GetSqlString(reader.GetOrdinal("BaselineWarningThreshold"));
                if (!baselinedThresholdXml.IsNull)
                {
                    warningBaselined = Threshold.Deserialize(baselinedThresholdXml.Value);
                }
                //END: SQLdm 10.0 (Tarun Sapra)- baselined thresholds

                SqlString dataXml = reader.GetSqlString(5);
                if (!dataXml.IsNull)
                {
                    data = Threshold.DeserializeData(dataXml.Value);
                }

                string thresholdInstanceName;
                if (reader["ThresholdInstanceName"] == DBNull.Value)
                    thresholdInstanceName = String.Empty;
                else
                    thresholdInstanceName = reader["ThresholdInstanceName"].ToString();

                MetricThresholdEntry entry = new MetricThresholdEntry(sqlServerId, metricID, thresholdInstanceName, warning, critical, info, warningBaselined, criticalBaselined, infoBaselined);
                entry.Data = data;

                //changing reader.FieldCount > 9 condition in below code to fix defect, below code for update SP always gives FieldCount >9 so condition fails
                //SQLdm 10.0 (Srishti Puohit) -- 10.0 Defect fix
                if (RepositoryHelper.HasColumn(reader, "UTCSnoozeStart"))
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
                //10.0 srishti purohit -- for baseline alert modifications
                entry.IsBaselineEnabled = Convert.ToBoolean(reader["IsBaselineEnabled"]);

                result.Add(entry);
            }
            return result;
        }

        /// <summary>
        /// Check if a sqlReader has specified named column.
        /// Function added in SQLdm10.1 (srishti purohit) -- for defect apply alert config corrupted
        /// </summary>
        /// <param name="r"></param>
        /// <param name="columnName"></param>
        public static bool HasColumn(IDataRecord reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
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
            //if (!thresholds.ContainsKey(thresholdEntry.MetricID))
            //{
            //    thresholds.Add(thresholdEntry.MetricID, thresholdEntry);
            //}
        }

        public static Dictionary<Metric, MetricInfo> GetMetricInfo(string repositoryConnectionString)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            Dictionary<Metric, MetricInfo> result = new Dictionary<Metric, MetricInfo>();
            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                using (SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(connection, GetMetricInfoStoredProcedure, DBNull.Value))
                {
                    while (dataReader.Read())
                    {
                        // get the metric
                        Metric metric = (Metric)Enum.ToObject(typeof(Metric), dataReader.GetInt32(0));
                        MetricMetaData mmd = MetricMetaDataFactory.GetMetaData(metric);
                        if (mmd == null)
                            continue;
                        int rank = dataReader.GetInt32(1);
                        string category = dataReader.GetString(2);
                        string name = dataReader.GetString(3);
                        string description = dataReader.GetString(4);
                        SqlString comments = (SqlString)dataReader.GetSqlValue(5);
                        MetricInfo metricInfo =
                            new MetricInfo(metric, name, category, description, comments.IsNull ? null : comments.Value,
                                           rank);
                        result.Add(metric, metricInfo);
                    }
                }
            }
            return result;
        }

        #endregion

        #region Outstanding Events

        //        public static IList<OutstandingEventEntry> GetOutstandingEvents(string repositoryConnectionString, int serverId)
        //        {
        //            IList<OutstandingEventEntry> result = new List<OutstandingEventEntry>();
        //
        //            if (repositoryConnectionString == null)
        //            {
        //                throw new ArgumentNullException("repositoryConnectionString");
        //            }
        //
        //            using (
        //                SqlDataReader reader =
        //                    SqlHelper.ExecuteReader(repositoryConnectionString, GetOutstandingEventsStoredProcedure, serverId))
        //            {
        //                GetOutstandingEvents(reader, result);
        //            }
        //
        //            return result;
        //        }

        //        public static void GetOutstandingEvents(SqlDataReader reader, IList<OutstandingEventEntry> result)
        //        {
        //            while (reader.Read())
        //            {
        //                OutstandingEventEntry entry = new OutstandingEventEntry();
        //                string serverName = reader.GetValue(0) as string;
        //                string databaseName = reader.GetValue(1) as string;
        //                string tableName = reader.GetValue(2) as string;
        //                entry.MetricID = reader.GetInt32(3);
        //                entry.MonitoredObjectName = new MonitoredObjectName(serverName, databaseName, tableName);
        //                entry.State = (MonitoredState)Enum.ToObject(typeof(MonitoredState), reader.GetByte(4));
        //                entry.OccuranceTime = reader.GetDateTime(5);
        //                entry.SerializedValue = reader.GetString(6);
        //                string qualifiers = null;
        //                if (!reader.IsDBNull(7))
        //                    qualifiers = reader.GetString(7);
        //                entry.MonitoredObjectName.AdditionalQualifiersXML = qualifiers;
        //
        //                result.Add(entry);
        //            }
        //        }

        #endregion

        #region MonitoredSqlServer

        public static XmlDocument GetMonitoredSqlServerStatus(SqlConnection connection, int? instanceId)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            XmlDocument document = new XmlDocument();
            using (XmlReader xmlReader =
                SqlHelper.ExecuteXmlReader(connection, GetMonitoredSqlServerStatusStoredProcedure,
                                           instanceId == null ? (object)null : instanceId.Value))
            {
                document.Load(xmlReader);
            }
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("RepositoryHelper.GetMonitoredSqlServerStatus took : {0}", stopWatch.ElapsedMilliseconds);
            return document;
        }

        /// <summary>
        /// Query Store parameter
        /// </summary>
        private const string QueryMonitorQueryStoreMonitoringEnabled = "@QueryMonitorQueryStoreMonitoringEnabled";

        /// <summary>
        /// Query Waits parameter
        /// </summary>
        private const string ActiveWaitQsEnable = "@ActiveWaitQsEnable";

        /// <summary>
        /// AWS Access Key paramter
        /// </summary>
        private const string AWSAccessKey = "@AWSAccessKey";

        /// <summary>
        /// AWS Secret Key
        /// </summary>
        private const string AWSSecretKey = "@AWSSecretKey";

        /// <summary>
        /// AWS Regions Endpoint
        /// </summary>
        private const string AWSRegionEndpoint = "@AWSRegionEndpoint";

        /// <summary>
        /// Maintenance mode on demand parameter
        /// </summary>
        private const string MaintenanceModeOnDemand = "@MaintenanceModeOnDemand";

        public static MonitoredSqlServer AddMonitoredSqlServer(string repositoryConnectionString,
                                                               MonitoredSqlServerConfiguration configuration)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            MonitoredSqlServer ret = null;

            // Read Parameter Map to use parameter name instead of index
            Dictionary<string, int> parameterNameToIndexMap;
            SqlParameter[] parameters =
                SqlHelperParameterCache.GetSpParameterSet(repositoryConnectionString, AddMonitoredSqlServerStoredProcedure, true, out parameterNameToIndexMap);

            parameters[1].Value = configuration.InstanceName;

            if (configuration.CollectionServiceId != Guid.Empty)
            {
                parameters[2].Value = configuration.CollectionServiceId;
            }
            else
            {
                parameters[2].Value = DBNull.Value;
            }

            parameters[3].Value = configuration.ConnectionInfo.UseIntegratedSecurity;

            if (!configuration.ConnectionInfo.UseIntegratedSecurity)
            {
                parameters[4].Value = configuration.ConnectionInfo.UserName;
                parameters[5].Value = configuration.ConnectionInfo.EncryptedPassword;
            }

            parameters[6].Value = Convert.ToInt32(configuration.ScheduledCollectionInterval.TotalSeconds);
            parameters[7].Value = configuration.QueryMonitorConfiguration.Enabled;
            parameters[8].Value = configuration.QueryMonitorConfiguration.SqlBatchEventsEnabled;
            parameters[9].Value = configuration.QueryMonitorConfiguration.SqlStatementEventsEnabled;
            parameters[10].Value = configuration.QueryMonitorConfiguration.StoredProcedureEventsEnabled;

            parameters[11].Value = configuration.ActivityMonitorConfiguration.DeadlockEventsEnabled;
            parameters[65].Value = configuration.ActivityMonitorConfiguration.Enabled;
            parameters[66].Value = configuration.ActivityMonitorConfiguration.AutoGrowEventsEnabled;
            parameters[67].Value = configuration.ActivityMonitorConfiguration.BlockingEventsEnabled;
            parameters[68].Value = configuration.ActivityMonitorConfiguration.BlockedProcessThreshold;

            //START SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with Extended Events -- Added a newly added params
            parameters[71].Value = configuration.ActivityMonitorConfiguration.TraceMonitoringEnabled;
            parameters[72].Value = configuration.ActivityMonitorConfiguration.FileSizeXeMB;
            parameters[73].Value = configuration.ActivityMonitorConfiguration.FileSizeRolloverXe;
            parameters[74].Value = configuration.ActivityMonitorConfiguration.RecordsPerRefreshXe;
            parameters[75].Value = configuration.ActivityMonitorConfiguration.MaxMemoryXeMB;
            parameters[76].Value = configuration.ActivityMonitorConfiguration.EventRetentionModeXe;
            parameters[77].Value = configuration.ActivityMonitorConfiguration.MaxDispatchLatencyXe;
            parameters[78].Value = configuration.ActivityMonitorConfiguration.MaxEventSizeXemb;
            parameters[79].Value = configuration.ActivityMonitorConfiguration.MemoryPartitionModeXe;
            parameters[80].Value = configuration.ActivityMonitorConfiguration.TrackCausalityXe;
            parameters[81].Value = configuration.ActivityMonitorConfiguration.StartupStateXe;
            parameters[82].Value = configuration.ActivityMonitorConfiguration.FileNameXEsession;
            //END SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with Extended Events -- Added a newly added params

            //START SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new parameters for query monitoring extended event session configuration
            parameters[83].Value = configuration.QueryMonitorConfiguration.FileSizeXeMB;
            parameters[84].Value = configuration.QueryMonitorConfiguration.FileSizeRolloverXe;
            //END SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new parameters for query monitoring extended event session configuration

            parameters[12].Value = Convert.ToInt32(configuration.QueryMonitorConfiguration.DurationFilter.TotalMilliseconds);
            parameters[13].Value = Convert.ToInt32(configuration.QueryMonitorConfiguration.CpuUsageFilter.TotalMilliseconds);
            parameters[14].Value = configuration.QueryMonitorConfiguration.LogicalDiskReads;
            parameters[15].Value = configuration.QueryMonitorConfiguration.PhysicalDiskWrites;
            parameters[16].Value = configuration.QueryMonitorConfiguration.TraceFileSize.Kilobytes.HasValue
                                       ? configuration.QueryMonitorConfiguration.TraceFileSize.Kilobytes
                                       : 0;
            parameters[17].Value = configuration.QueryMonitorConfiguration.TraceFileRollovers;
            parameters[18].Value = configuration.QueryMonitorConfiguration.RecordsPerRefresh;
            //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session -- Added two newly added parameters
            parameters[69].Value = configuration.QueryMonitorConfiguration.TraceMonitoringEnabled;
            parameters[70].Value = configuration.QueryMonitorConfiguration.CollectQueryPlan;

            if (configuration.GrowthStatisticsStartTime == null)
                parameters[19].Value = DBNull.Value;
            else
                parameters[19].Value = configuration.GrowthStatisticsStartTime;
            if (configuration.ReorgStatisticsStartTime == null)
                parameters[20].Value = DBNull.Value;
            else
                parameters[20].Value = configuration.ReorgStatisticsStartTime;
            if (configuration.GrowthStatisticsDays == null)
                configuration.GrowthStatisticsDays = 124;
            parameters[21].Value = configuration.GrowthStatisticsDays;
            if (configuration.ReorgStatisticsDays == null)
                configuration.ReorgStatisticsDays = 124;
            parameters[22].Value = configuration.ReorgStatisticsDays;

            List<string> tableStatisticsExcludedDatabases = configuration.TableStatisticsExcludedDatabases;
            if (tableStatisticsExcludedDatabases.Count == 0)
            {
                tableStatisticsExcludedDatabases.AddRange(new string[] { "master", "model", "msdb", "tempdb" });
            }
            XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(string[]));
            StringBuilder buffer = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(buffer))
            {
                serializer.Serialize(writer, tableStatisticsExcludedDatabases.ToArray());
                writer.Flush();
            }
            parameters[23].Value = buffer.ToString();
            parameters[24].Value = configuration.ConnectionInfo.EncryptData;
            parameters[25].Value = configuration.ConnectionInfo.TrustServerCertificate;
            parameters[26].Value = configuration.ReorganizationMinimumTableSize.Kilobytes.Value;
            parameters[27].Value = configuration.ReplicationMonitoringDisabled;
            parameters[28].Value = AdvancedQueryMonitorConfiguration.SerializeToXml(configuration.QueryMonitorConfiguration.AdvancedConfiguration);
            parameters[29].Value = configuration.ExtendedHistoryCollectionDisabled;
            parameters[30].Value = configuration.OleAutomationDisabled;
            if (configuration.OleAutomationDisabled)
                LOG.Info(configuration.InstanceName, " use of Ole Automation is disabled.");

            serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(DiskCollectionSettings));
            buffer = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(buffer))
            {
                serializer.Serialize(writer, configuration.DiskCollectionSettings);
                writer.Flush();
            }
            parameters[31].Value = buffer.ToString();

            parameters[32].Value = configuration.QueryMonitorConfiguration.StopTimeUTC;

            parameters[33].Value = configuration.InputBufferLimiter;

            parameters[34].Value = configuration.InputBufferLimited;

            if (configuration.ActiveWaitsConfiguration != null)
            {
                if (configuration.ActiveWaitsConfiguration.StartTimeRelative > DateTime.MinValue)
                    parameters[35].Value = configuration.ActiveWaitsConfiguration.StartTimeRelative;
                else
                    parameters[35].Value = DBNull.Value;
                if (configuration.ActiveWaitsConfiguration.RunTime.HasValue)
                    parameters[36].Value = configuration.ActiveWaitsConfiguration.RunTime.Value.TotalSeconds;
                else
                    parameters[36].Value = DBNull.Value;

                parameters[37].Value = configuration.ActiveWaitsConfiguration.CollectionTimeSeconds;
                parameters[38].Value = configuration.ActiveWaitsConfiguration.Enabled;
                parameters[39].Value = configuration.ActiveWaitsConfiguration.LoopTimeMilliseconds;
                parameters[40].Value = AdvancedQueryFilterConfiguration.SerializeToXml(configuration.ActiveWaitsConfiguration.AdvancedConfiguration);

                parameters[41].Value = configuration.ActiveWaitsConfiguration.EnabledXe;
                parameters[42].Value = configuration.ActiveWaitsConfiguration.FileSizeXeMB;
                parameters[43].Value = configuration.ActiveWaitsConfiguration.FileSizeRolloverXe;
                parameters[44].Value = configuration.ActiveWaitsConfiguration.RecordsPerRefreshXe;
                parameters[45].Value = configuration.ActiveWaitsConfiguration.MaxMemoryXeMB;
                parameters[46].Value = configuration.ActiveWaitsConfiguration.EventRetentionModeXe;
                parameters[47].Value = configuration.ActiveWaitsConfiguration.MaxDispatchLatencyXe;
                parameters[48].Value = configuration.ActiveWaitsConfiguration.MaxEventSizeXemb;
                parameters[49].Value = configuration.ActiveWaitsConfiguration.MemoryPartitionModeXe;
                parameters[50].Value = configuration.ActiveWaitsConfiguration.TrackCausalityXe;
                parameters[51].Value = configuration.ActiveWaitsConfiguration.StartupStateXe;
                parameters[52].Value = configuration.ActiveWaitsConfiguration.FileNameXEsession;
            }
            else
            {
                parameters[35].Value = DBNull.Value;
                parameters[36].Value = DBNull.Value;
                parameters[37].Value = DBNull.Value;
                parameters[38].Value = false;
                parameters[39].Value = DBNull.Value;
                parameters[40].Value = DBNull.Value;
                parameters[41].Value = ActiveWaitsConfiguration.EnabledXeDefault;
                parameters[42].Value = ActiveWaitsConfiguration.FileSizeXeMBDefault;
                parameters[43].Value = ActiveWaitsConfiguration.FileSizeRolloverXeDefault;
                parameters[44].Value = ActiveWaitsConfiguration.RecordsPerRefreshXeDefault;
                parameters[45].Value = ActiveWaitsConfiguration.MaxMemoryXeMBDefault;
                parameters[46].Value = ActiveWaitsConfiguration.EventRetentionModeXeDefault;
                parameters[47].Value = ActiveWaitsConfiguration.MaxDispatchLatencyXeDefault;
                parameters[48].Value = ActiveWaitsConfiguration.MaxEventSizeXembDefault;
                parameters[49].Value = ActiveWaitsConfiguration.MemoryPartitionModeXeDefault;
                parameters[50].Value = ActiveWaitsConfiguration.TrackCausalityXeDefault;
                parameters[51].Value = ActiveWaitsConfiguration.StartupStateXeDefault;
                parameters[52].Value = ActiveWaitsConfiguration.FileNameXEsessionDefault;

            }

            parameters[53].Value = configuration.ServerPingInterval != null ? (int)configuration.ServerPingInterval.TotalSeconds : 30;
            parameters[54].Value = configuration.AlertTemplateID != null ? (int)configuration.AlertTemplateID : -1;

            if (configuration.VirtualizationConfiguration == null)
            {
                parameters[55].Value = DBNull.Value;
                parameters[56].Value = DBNull.Value;
                parameters[57].Value = DBNull.Value;
                parameters[58].Value = DBNull.Value;
            }
            else
            {
                parameters[55].Value = configuration.VirtualizationConfiguration.VCHostID;
                parameters[56].Value = configuration.VirtualizationConfiguration.InstanceUUID;
                parameters[57].Value = configuration.VirtualizationConfiguration.VMName;
                parameters[58].Value = configuration.VirtualizationConfiguration.VMDomainName;
            }

            if (configuration.BaselineConfiguration == null)
            {
                parameters[59].Value = DBNull.Value;
            }
            else
            {
                parameters[59].Value = configuration.BaselineConfiguration.Serialize();
            }

            parameters[60].Value = configuration.DatabaseStatisticsInterval.TotalSeconds;

            var wmi = configuration.WmiConfig;
            parameters[61].Value = wmi.DirectWmiEnabled;
            parameters[62].Value = wmi.DirectWmiConnectAsCollectionService;
            parameters[63].Value = wmi.DirectWmiUserName;
            parameters[64].Value = wmi.EncryptedPassword;


            //START 10.0 (srishti purohit) : To handle analysis configuration
            if (configuration.AnalysisConfiguration == null)
                configuration.AnalysisConfiguration = new AnalysisConfiguration();
            var analysisConfig = configuration.AnalysisConfiguration;
            parameters[85].Value = analysisConfig.ProductionServer;
            parameters[86].Value = analysisConfig.IsOLTP;
            parameters[87].Value = analysisConfig.StartTime == DateTime.MinValue ? new DateTime(1753, 1, 1) : analysisConfig.StartTime;
            parameters[88].Value = analysisConfig.AnalysisDuration;
            parameters[89].Value = analysisConfig.SelectedDays;
            parameters[90].Value = analysisConfig.IsActive;
            parameters[91].Value = analysisConfig.IncludeDatabase;
            parameters[92].Value = analysisConfig.FilterApplication;
            //Pass list of category/ Database/ Recomm as xml to SP
            parameters[93].Value = analysisConfig.BlockedCategoryID == null ? GetXMLFromList(new List<int>(), "Category", "CategoryID").InnerXml : GetXMLFromList(analysisConfig.BlockedCategoryID, "Category", "CategoryID").InnerXml;
            parameters[94].Value = analysisConfig.BlockedDatabaseIDList == null ? GetXMLFromList(new List<int>(), "Database", "DatabaseID").InnerXml : GetXMLFromList(analysisConfig.BlockedDatabaseIDList, "Database", "DatabaseID").InnerXml;
            parameters[95].Value = analysisConfig.BlockedRecommendationID == null ? GetXMLFromList(new List<string>(), "Recommendation", "RecommendationID").InnerXml : GetXMLFromList(analysisConfig.BlockedRecommendationID, "Recommendation", "RecommendationID").InnerXml;
            //END 10.0 (srishti purohit) : To handle analysis configuration
            parameters[97].Value = analysisConfig.SchedulingStatus; // Start:SQLDm 10.0 - Praveen Suhalka - Scheduling status

            //10.0 SQLdm Srishti Purohit Fixing defect DE46150. Setting Estimated query plan on by default.
            parameters[98].Value = configuration.QueryMonitorConfiguration.CollectEstimatedQueryPlan;

            parameters[96].Value = configuration.CloudProviderId;//SQLdm 10.0 (Tarun Sapra) - Minimal Cloud Support: Added another param for cloud provider id

            //SQLdm 10.1 (Barkha Khatri) adding sys Admin flag parameter value
            parameters[99].Value = configuration.IsUserSysAdmin;

            // SQLdm 10.4 (Varun Chopra) saving top plan count filter
            parameters[100].Value = configuration.QueryMonitorConfiguration.TopPlanCountFilter;
            // SQLdm 10.4 (Varun Chopra) saving top plan count filter
            parameters[101].Value = configuration.QueryMonitorConfiguration.TopPlanCategoryFilter;


            // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store
            if (parameterNameToIndexMap.ContainsKey(QueryMonitorQueryStoreMonitoringEnabled))
            {
                parameters[parameterNameToIndexMap[QueryMonitorQueryStoreMonitoringEnabled]].Value = configuration
                    .QueryMonitorConfiguration.QueryStoreMonitoringEnabled;
            }

            // SQLdm 10.4 (Varun Chopra) - Query waits using Query Store
            if (parameterNameToIndexMap.ContainsKey(ActiveWaitQsEnable))
            {
                parameters[parameterNameToIndexMap[ActiveWaitQsEnable]].Value = configuration.ActiveWaitsConfiguration.EnabledQs;
            }

            if (parameterNameToIndexMap.ContainsKey(AWSAccessKey))
            {
                parameters[parameterNameToIndexMap[AWSAccessKey]].Value = configuration.AwsAccessKey;
            }

            if (parameterNameToIndexMap.ContainsKey(AWSSecretKey))
            {
                parameters[parameterNameToIndexMap[AWSSecretKey]].Value = configuration.AwsSecretKey;
            }

            if (parameterNameToIndexMap.ContainsKey(AWSRegionEndpoint))
            {
                parameters[parameterNameToIndexMap[AWSRegionEndpoint]].Value = configuration.AwsRegionEndpoint;
            }

            SqlTransaction xa = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();
                    xa = connection.BeginTransaction();

                    using (
                        SqlDataReader dataReader =
                            SqlHelper.ExecuteReader(xa, CommandType.StoredProcedure,
                                                    AddMonitoredSqlServerStoredProcedure, parameters))
                    {
                        if (dataReader.Read())
                        {
                            int id = (int)dataReader["SQLServerID"];
                            DateTime registeredDate = (DateTime)dataReader["RegisteredDate"];
                            int collectionServiceIdColumn = dataReader.GetOrdinal("CollectionServiceID");
                            SqlGuid sqlGuid = dataReader.GetSqlGuid(collectionServiceIdColumn);
                            Guid collectionServiceId = sqlGuid.IsNull ? Guid.Empty : sqlGuid.Value;
                            configuration.CollectionServiceId = collectionServiceId;
                            configuration.ActiveWaitsConfiguration = new ActiveWaitsConfiguration(id, configuration.ActiveWaitsConfiguration, configuration.ActiveWaitsConfiguration);
                            ret = new MonitoredSqlServer(id, registeredDate, configuration);

                            // Update the Ole Automation creation context
                            bool isOutOfProcOleAutomation = false;
                            int ordinal = dataReader.GetOrdinal("OutOfProcOleAutomation");
                            if (!dataReader.IsDBNull(ordinal))
                                isOutOfProcOleAutomation = dataReader.GetBoolean(ordinal);
                            ret.OleAutomationContext = isOutOfProcOleAutomation
                                                           ? MonitoredSqlServer.OleAutomationExecutionContext.OutOfProc
                                                           : MonitoredSqlServer.OleAutomationExecutionContext.Both;
                            LOG.Info("New monitored instance ", ret.InstanceName, " Ole Automation context is ",
                                     ret.OleAutomationContext.ToString());
                        }
                    }

                    if (ret != null && configuration.Tags.Count > 0)
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        XmlElement rootElement = xmlDoc.CreateElement("Tags");
                        xmlDoc.AppendChild(rootElement);
                        XmlAddList(xmlDoc, configuration.Tags, "Tag", "TagId");

                        using (
                            SqlDataReader dataReader =
                                SqlHelper.ExecuteReader(xa, UpdateServerTagsStoredProcedure, ret.Id,
                                                        xmlDoc.InnerXml, true))
                        {
                        }

                        // if server tags are set but no custom counters are statically linked 
                        // add a dummy custom counter list so that thresholds are created for counters 
                        // associated via tag reference.
                        if (configuration.CustomCounters == null)
                            configuration.CustomCounters = new List<int>();
                    }

                    if (ret != null && configuration.CustomCounters != null)
                    {   // only monkey with custom counters if the collection is set.  
                        // An empty collection means no counters are associated with the instance.
                        XmlDocument xmlDoc = new XmlDocument();
                        XmlElement rootElement = xmlDoc.CreateElement("MetricAssignment");
                        xmlDoc.AppendChild(rootElement);
                        XmlAddList(xmlDoc, configuration.CustomCounters, "Metric", "MetricID");

                        using (SqlDataReader dataReader = SqlHelper.ExecuteReader(xa, AssignCountersToServerProcedure, ret.Id, xmlDoc.OuterXml, true))
                        {
                        }
                    }

                    xa.Commit();
                    //SQLdm 10.1 (GK): removing this as the periodic sync will do this within 10 secs.
                    //SyncInstanceWithCWF(ret); //SQLdm 9.0 (Ankit Srivastava) - CWF Integration - syncing the newly added sql server instances

                }
            }
            catch (Exception)
            {
                if (xa != null)
                    try { xa.Rollback(); }
                    catch (Exception) { }
                throw;
            }
            finally
            {
                if (xa != null)
                    try { xa.Dispose(); }
                    catch (Exception) { }
            }

            return ret;
        }

        //[START] - SQLdm 9.0 (Ankit Srivastava) - CWF Integration - new method for synchronizing the newly added instance with CWF
        private static void SyncInstanceWithCWF(MonitoredSqlServer sqlServer)
        {
            try
            {
                //getting the CWF details
                CommonWebFramework cwfDetails = CommonWebFramework.GetInstance();
                var cwfHelper = new CWFHelper(cwfDetails);

                IList<Instance> instances = new List<Instance>();

                //Create corresponding instance object 
                var instance = new Instance();
                instance.Name = sqlServer.InstanceName;
                instance.UtcFirstSeen = sqlServer.RegisteredDate;
                instance.UtcLastSeen = String.Empty;
                instance.Edition = sqlServer.MostRecentSQLEdition ?? String.Empty;
                instance.Version = sqlServer.MostRecentSQLVersion != null ? sqlServer.MostRecentSQLVersion.Version : String.Empty;
                instance.Owner = String.Empty;
                instance.Location = String.Empty;
                instance.Comments = String.Empty;
                instance.InstanceStatus = InstanceStatus.Managed;
                instances.Add(instance);

                //adding to the CWF 
                cwfHelper.AddInstances(instances);
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error Ocurred in SyncInstanceWithCWF:", ex);
            }
        }
        //[END] - SQLdm 9.0 (Ankit Srivastava) - CWF Integration - new method for synchronizing the newly added instance with CWF		

        public static MonitoredSqlServer UpdateMonitoredSqlServer(string repositoryConnectionString, int id, MonitoredSqlServerConfiguration configuration)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            // Read Parameters Map to get index using parameter name
            Dictionary<string, int> parameterNameToIndexMap;
            SqlParameter[] parameters =
                SqlHelperParameterCache.GetSpParameterSet(repositoryConnectionString, UpdateMonitoredSqlServerStoredProcedure, true, out parameterNameToIndexMap);

            parameters[1].Value = id;
            parameters[2].Value = configuration.IsActive;

            if (configuration.CollectionServiceId != Guid.Empty)
            {
                parameters[3].Value = configuration.CollectionServiceId;
            }
            else
            {
                parameters[3].Value = DBNull.Value;
            }

            parameters[4].Value = configuration.ConnectionInfo.UseIntegratedSecurity;

            if (!configuration.ConnectionInfo.UseIntegratedSecurity)
            {
                parameters[5].Value = configuration.ConnectionInfo.UserName;
                parameters[6].Value = configuration.ConnectionInfo.EncryptedPassword;
            }

            parameters[7].Value = Convert.ToInt32(configuration.ScheduledCollectionInterval.TotalSeconds);
            parameters[8].Value = configuration.MaintenanceModeEnabled;
            parameters[9].Value = configuration.QueryMonitorConfiguration.Enabled;
            parameters[10].Value = configuration.QueryMonitorConfiguration.SqlBatchEventsEnabled;
            parameters[11].Value = configuration.QueryMonitorConfiguration.SqlStatementEventsEnabled;
            parameters[12].Value = configuration.QueryMonitorConfiguration.StoredProcedureEventsEnabled;
            parameters[13].Value = configuration.ActivityMonitorConfiguration.DeadlockEventsEnabled;

            parameters[74].Value = configuration.ActivityMonitorConfiguration.Enabled;
            parameters[75].Value = configuration.ActivityMonitorConfiguration.BlockingEventsEnabled;
            parameters[76].Value = configuration.ActivityMonitorConfiguration.AutoGrowEventsEnabled;
            parameters[77].Value = configuration.ActivityMonitorConfiguration.BlockedProcessThreshold;
            //START SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with Extended Events -- Added a newly added params
            parameters[86].Value = configuration.ActivityMonitorConfiguration.TraceMonitoringEnabled;
            parameters[87].Value = configuration.ActivityMonitorConfiguration.FileSizeXeMB;
            parameters[88].Value = configuration.ActivityMonitorConfiguration.FileSizeRolloverXe;
            parameters[89].Value = configuration.ActivityMonitorConfiguration.RecordsPerRefreshXe;
            parameters[90].Value = configuration.ActivityMonitorConfiguration.MaxMemoryXeMB;
            parameters[91].Value = configuration.ActivityMonitorConfiguration.EventRetentionModeXe;
            parameters[92].Value = configuration.ActivityMonitorConfiguration.MaxDispatchLatencyXe;
            parameters[93].Value = configuration.ActivityMonitorConfiguration.MaxEventSizeXemb;
            parameters[94].Value = configuration.ActivityMonitorConfiguration.MemoryPartitionModeXe;
            parameters[95].Value = configuration.ActivityMonitorConfiguration.TrackCausalityXe;
            parameters[96].Value = configuration.ActivityMonitorConfiguration.StartupStateXe;
            parameters[97].Value = configuration.ActivityMonitorConfiguration.FileNameXEsession;
            //END SQLdm 9.1 (Ankit Srivastava) -- Activity Monitoring with Extended Events -- Added a newly added params

            //START SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new parameters for query monitoring extended event session configuration
            parameters[98].Value = configuration.QueryMonitorConfiguration.FileSizeXeMB;
            parameters[99].Value = configuration.QueryMonitorConfiguration.FileSizeRolloverXe;
            //END SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new parameters for query monitoring extended event session configuration

            parameters[14].Value = Convert.ToInt32(configuration.QueryMonitorConfiguration.DurationFilter.TotalMilliseconds);
            parameters[15].Value = Convert.ToInt32(configuration.QueryMonitorConfiguration.CpuUsageFilter.TotalMilliseconds);
            parameters[16].Value = configuration.QueryMonitorConfiguration.LogicalDiskReads;
            parameters[17].Value = configuration.QueryMonitorConfiguration.PhysicalDiskWrites;
            parameters[18].Value = configuration.QueryMonitorConfiguration.TraceFileSize.Kilobytes;
            parameters[19].Value = configuration.QueryMonitorConfiguration.TraceFileRollovers;
            parameters[20].Value = configuration.QueryMonitorConfiguration.RecordsPerRefresh;
            //SQLdm 9.0 (Ankit Srivastava) -- Extended Events Query Monitoring -- Added two newly added parameters
            parameters[84].Value = configuration.QueryMonitorConfiguration.TraceMonitoringEnabled;
            parameters[85].Value = configuration.QueryMonitorConfiguration.CollectQueryPlan;

            if (configuration.GrowthStatisticsStartTime == null)
                parameters[21].Value = DBNull.Value;
            else
                parameters[21].Value = configuration.GrowthStatisticsStartTime.Value;

            if (configuration.ReorgStatisticsStartTime == null)
                parameters[22].Value = DBNull.Value;
            else
                parameters[22].Value = configuration.ReorgStatisticsStartTime.Value;

            if (configuration.GrowthStatisticsDays == null)
                parameters[23].Value = DBNull.Value;
            else
                parameters[23].Value = configuration.GrowthStatisticsDays.Value;

            if (configuration.ReorgStatisticsDays == null)
                parameters[24].Value = DBNull.Value;
            else
                parameters[24].Value = configuration.ReorgStatisticsDays.Value;

            List<string> tableStatisticsExcludedDatabases = configuration.TableStatisticsExcludedDatabases;
            if (tableStatisticsExcludedDatabases == null || tableStatisticsExcludedDatabases.Count == 0)
                parameters[25].Value = DBNull.Value;
            else
            {
                XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(string[]));
                StringBuilder buffer = new StringBuilder();
                using (XmlWriter writer = XmlWriter.Create(buffer))
                {
                    serializer.Serialize(writer, tableStatisticsExcludedDatabases.ToArray());
                    writer.Flush();
                }
                parameters[25].Value = buffer.ToString();
            }
            parameters[26].Value = configuration.ConnectionInfo.EncryptData;
            parameters[27].Value = configuration.ConnectionInfo.TrustServerCertificate;
            parameters[28].Value = configuration.ReorganizationMinimumTableSize.Kilobytes.Value;
            parameters[29].Value = configuration.ReplicationMonitoringDisabled;
            parameters[30].Value = configuration.MaintenanceMode.MaintenanceModeType;
            parameters[31].Value = configuration.MaintenanceMode.MaintenanceModeStart;
            parameters[32].Value = configuration.MaintenanceMode.MaintenanceModeStop;
            parameters[78].Value = configuration.MaintenanceMode.MaintenanceModeMonth;
            parameters[79].Value = configuration.MaintenanceMode.MaintenanceModeSpecificDay;
            parameters[80].Value = configuration.MaintenanceMode.MaintenanceModeWeekOrdinal;
            parameters[81].Value = configuration.MaintenanceMode.MaintenanceModeWeekDay;
            if (configuration.MaintenanceMode.MaintenanceModeMonthDuration.HasValue)
            {
                parameters[82].Value = configuration.MaintenanceMode.MaintenanceModeMonthDuration.Value.TotalSeconds;
            }
            else
            {
                parameters[82].Value = null;
            }
            parameters[83].Value = configuration.MaintenanceMode.MaintenanceModeMonthRecurringStart;
            if (configuration.MaintenanceMode.MaintenanceModeDuration.HasValue)
            {
                parameters[33].Value = configuration.MaintenanceMode.MaintenanceModeDuration.Value.TotalSeconds;
            }
            else
            {
                parameters[33].Value = null;
            }
            parameters[34].Value = configuration.MaintenanceMode.MaintenanceModeDays;
            parameters[35].Value = AdvancedQueryMonitorConfiguration.SerializeToXml(configuration.QueryMonitorConfiguration.AdvancedConfiguration);
            parameters[36].Value = configuration.ExtendedHistoryCollectionDisabled;
            parameters[37].Value = configuration.MaintenanceMode.MaintenanceModeRecurringStart;
            parameters[38].Value = configuration.OleAutomationDisabled;
            {
                XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(DiskCollectionSettings));
                StringBuilder buffer = new StringBuilder();
                using (XmlWriter writer = XmlWriter.Create(buffer))
                {
                    serializer.Serialize(writer, configuration.DiskCollectionSettings);
                    writer.Flush();
                }
                parameters[39].Value = buffer.ToString();
            }

            parameters[40].Value = configuration.QueryMonitorConfiguration.StopTimeUTC;

            parameters[41].Value = configuration.InputBufferLimiter;

            parameters[42].Value = configuration.InputBufferLimited;

            parameters[43].Value = configuration.PreferredClusterNode;

            if (configuration.ActiveWaitsConfiguration != null && configuration.ActiveWaitsConfiguration.StartTimeRelative > DateTime.MinValue)
            {
                parameters[44].Value = configuration.ActiveWaitsConfiguration.StartTimeRelative;

                if (configuration.ActiveWaitsConfiguration.RunTime.HasValue)
                    parameters[45].Value = configuration.ActiveWaitsConfiguration.RunTime.Value.TotalSeconds;
                else
                    parameters[45].Value = DBNull.Value;

                parameters[46].Value = configuration.ActiveWaitsConfiguration.CollectionTimeSeconds;
                parameters[47].Value = configuration.ActiveWaitsConfiguration.Enabled;
                parameters[48].Value = configuration.ActiveWaitsConfiguration.LoopTimeMilliseconds;
                parameters[49].Value = AdvancedQueryFilterConfiguration.SerializeToXml(configuration.ActiveWaitsConfiguration.AdvancedConfiguration);

                parameters[58].Value = configuration.ActiveWaitsConfiguration.EnabledXe;
                parameters[59].Value = configuration.ActiveWaitsConfiguration.FileSizeXeMB;
                parameters[60].Value = configuration.ActiveWaitsConfiguration.FileSizeRolloverXe;
                parameters[61].Value = configuration.ActiveWaitsConfiguration.RecordsPerRefreshXe;
                parameters[62].Value = configuration.ActiveWaitsConfiguration.MaxMemoryXeMB;
                parameters[63].Value = configuration.ActiveWaitsConfiguration.EventRetentionModeXe;
                parameters[64].Value = configuration.ActiveWaitsConfiguration.MaxDispatchLatencyXe;
                parameters[65].Value = configuration.ActiveWaitsConfiguration.MaxEventSizeXemb;
                parameters[66].Value = configuration.ActiveWaitsConfiguration.MemoryPartitionModeXe;
                parameters[67].Value = configuration.ActiveWaitsConfiguration.TrackCausalityXe;
                parameters[68].Value = configuration.ActiveWaitsConfiguration.StartupStateXe;
                parameters[69].Value = configuration.ActiveWaitsConfiguration.FileNameXEsession;

            }
            else
            {
                parameters[44].Value = DBNull.Value;
                parameters[45].Value = DBNull.Value;
                parameters[46].Value = DBNull.Value;
                parameters[47].Value = false;
                parameters[48].Value = DBNull.Value;
                parameters[49].Value = DBNull.Value;

                parameters[58].Value = configuration.ActiveWaitsConfiguration.EnabledXe;
                parameters[59].Value = configuration.ActiveWaitsConfiguration.FileSizeXeMB;
                parameters[60].Value = configuration.ActiveWaitsConfiguration.FileSizeRolloverXe;
                parameters[61].Value = configuration.ActiveWaitsConfiguration.RecordsPerRefreshXe;
                parameters[62].Value = configuration.ActiveWaitsConfiguration.MaxMemoryXeMB;
                parameters[63].Value = configuration.ActiveWaitsConfiguration.EventRetentionModeXe;
                parameters[64].Value = configuration.ActiveWaitsConfiguration.MaxDispatchLatencyXe;
                parameters[65].Value = configuration.ActiveWaitsConfiguration.MaxEventSizeXemb;
                parameters[66].Value = configuration.ActiveWaitsConfiguration.MemoryPartitionModeXe;
                parameters[67].Value = configuration.ActiveWaitsConfiguration.TrackCausalityXe;
                parameters[68].Value = configuration.ActiveWaitsConfiguration.StartupStateXe;
                parameters[69].Value = configuration.ActiveWaitsConfiguration.FileNameXEsession;
            }

            parameters[50].Value = configuration.ServerPingInterval != null ? (int)configuration.ServerPingInterval.TotalSeconds : 30;

            if (configuration.VirtualizationConfiguration == null)
            {
                parameters[51].Value = DBNull.Value;
                parameters[52].Value = DBNull.Value;
                parameters[53].Value = DBNull.Value;
                parameters[54].Value = DBNull.Value;
            }
            else
            {
                parameters[51].Value = GetVirtualHostID(repositoryConnectionString, configuration.VirtualizationConfiguration.VCAddress);
                parameters[52].Value = configuration.VirtualizationConfiguration.InstanceUUID;
                parameters[53].Value = configuration.VirtualizationConfiguration.VMName;
                parameters[54].Value = configuration.VirtualizationConfiguration.VMDomainName;
            }

            parameters[55].Value = configuration.AlertRefreshInMinutes;

            if (configuration.BaselineConfiguration == null)
            {
                parameters[56].Value = DBNull.Value;
            }
            else
            {
                parameters[56].Value = configuration.BaselineConfiguration.Serialize();
            }

            parameters[100].Value = configuration.FriendlyServerName; //SQLdm 10.0 (Gaurav Karwal): adding the parameter for friendly server name

            //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
            if (configuration.BaselineConfigurationList == null)
            {
                parameters[101].Value = DBNull.Value;
            }
            else
            {
                parameters[101].Value = Common.Configuration.BaselineConfiguration.XmlSerialize(configuration.BaselineConfigurationList).ToString();
            }
            //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
            parameters[57].Value = configuration.DatabaseStatisticsInterval.TotalSeconds;

            var wmi = configuration.WmiConfig;
            parameters[70].Value = wmi.DirectWmiEnabled;
            parameters[71].Value = wmi.DirectWmiConnectAsCollectionService;
            parameters[72].Value = wmi.DirectWmiUserName;
            parameters[73].Value = wmi.EncryptedPassword;

            //START 10.0 (srishti purohit) : To handle analysis configuration

            if (configuration.AnalysisConfiguration == null)
                configuration.AnalysisConfiguration = new AnalysisConfiguration();
            var analysisConfig = configuration.AnalysisConfiguration;
            parameters[102].Value = analysisConfig.ProductionServer;
            parameters[103].Value = analysisConfig.IsOLTP;
            parameters[104].Value = analysisConfig.StartTime == DateTime.MinValue ? new DateTime(1753, 1, 1) : analysisConfig.StartTime;
            parameters[105].Value = analysisConfig.AnalysisDuration;
            parameters[106].Value = analysisConfig.SelectedDays;
            parameters[107].Value = analysisConfig.IsActive;
            parameters[108].Value = analysisConfig.IncludeDatabase;
            parameters[109].Value = analysisConfig.FilterApplication;
            //Pass list of category/ Database/ Recomm as xml to SP
            parameters[110].Value = GetXMLFromList(analysisConfig.BlockedCategoryID, "Category", "CategoryID").InnerXml;
            parameters[111].Value = GetXMLFromList(analysisConfig.BlockedDatabaseIDList, "Database", "DatabaseID").InnerXml;
            parameters[112].Value = GetXMLFromList(analysisConfig.BlockedRecommendationID, "Recommendation", "RecommendationID").InnerXml;
            parameters[114].Value = analysisConfig.SchedulingStatus;// Start:SQLDm 10.0 - Praveen Suhalka - Scheduling status

            //SQLdm 10.0 (Tarun Sapra) - Flag for displaying estimated query plan only
            parameters[113].Value = configuration.QueryMonitorConfiguration.CollectEstimatedQueryPlan;


            //END 10.0 (srishti purohit) : To handle analysis configuration

            parameters[115].Value = configuration.QueryMonitorConfiguration.TopPlanCountFilter;
            parameters[116].Value = configuration.QueryMonitorConfiguration.TopPlanCategoryFilter;

            // Maintenance mode on demand 
            if (parameterNameToIndexMap.ContainsKey(MaintenanceModeOnDemand))
            {
                parameters[parameterNameToIndexMap[MaintenanceModeOnDemand]].Value = configuration.MaintenanceMode.MaintenanceModeOnDemand;
            }

            // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store
            if (parameterNameToIndexMap.ContainsKey(QueryMonitorQueryStoreMonitoringEnabled))
            {
                parameters[parameterNameToIndexMap[QueryMonitorQueryStoreMonitoringEnabled]].Value = configuration
                    .QueryMonitorConfiguration.QueryStoreMonitoringEnabled;
            }

            // SQLdm 10.4 (Varun Chopra) - Query waits using Query Store
            if (parameterNameToIndexMap.ContainsKey(ActiveWaitQsEnable))
            {
                parameters[parameterNameToIndexMap[ActiveWaitQsEnable]].Value = configuration.ActiveWaitsConfiguration.EnabledQs;
            }
            parameters[120].Value = configuration.CloudProviderId;
            SqlTransaction xa = null;
            MonitoredSqlServer result = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();
                    xa = connection.BeginTransaction();
                    using (
                        SqlDataReader dataReader =
                            SqlHelper.ExecuteReader(xa, CommandType.StoredProcedure,
                                                    UpdateMonitoredSqlServerStoredProcedure, parameters))
                    {
                        if (dataReader.Read())
                        {
                            int returnId = (int)dataReader["SQLServerID"];
                            DateTime registeredDate = (DateTime)dataReader["RegisteredDate"];
                            result = new MonitoredSqlServer(returnId, registeredDate, configuration);

                            // Update the Ole Automation creation context
                            if (result != null)
                            {
                                bool isOutOfProcOleAutomation = false;
                                int ordinal = dataReader.GetOrdinal("OutOfProcOleAutomation");
                                if (!dataReader.IsDBNull(ordinal))
                                    isOutOfProcOleAutomation = dataReader.GetBoolean(ordinal);
                                result.OleAutomationContext = isOutOfProcOleAutomation ? MonitoredSqlServer.OleAutomationExecutionContext.OutOfProc : MonitoredSqlServer.OleAutomationExecutionContext.Both;
                                LOG.Info("Updated monitored instance ", result.InstanceName, " Ole Automation context is ", result.OleAutomationContext.ToString());
                            }
                        }
                    }
                    // assign tags before counters
                    if (configuration.Tags != null)
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        XmlElement rootElement = xmlDoc.CreateElement("Tags");
                        xmlDoc.AppendChild(rootElement);

                        XmlAddList(xmlDoc, configuration.Tags, "Tag", "TagId");

                        using (SqlDataReader dataReader = SqlHelper.ExecuteReader(xa, UpdateServerTagsStoredProcedure, id, xmlDoc.InnerXml, true))
                        {
                            //                            while (dataReader.Read())
                            //                            {
                            //                                if (result != null)
                            //                                    result.Tags.Add((int)dataReader[0]);
                            //                            }
                        }

                        // if server tags are set but no custom counters are statically linked 
                        // add a dummy custom counter list so that thresholds are created for counters 
                        // associated via tag reference.
                        if (configuration.CustomCounters == null)
                            configuration.CustomCounters = new List<int>();
                    }
                    // counters needs to be done after tags
                    if (configuration.CustomCounters != null)
                    {   // only monkey with custom counters if the collection is set.  
                        // An empty collection means no counters are associated with the instance.
                        XmlDocument xmlDoc = new XmlDocument();
                        XmlElement rootElement = xmlDoc.CreateElement("MetricAssignment");
                        xmlDoc.AppendChild(rootElement);
                        XmlAddList(xmlDoc, configuration.CustomCounters, "Metric", "MetricID");

                        using (SqlDataReader dataReader = SqlHelper.ExecuteReader(xa, AssignCountersToServerProcedure, id, xmlDoc.OuterXml, true))
                        {
                            //                            while (dataReader.Read())
                            //                            {
                            //                                if (result != null)
                            //                                    result.CustomCounters.Add((int)dataReader[0]);
                            //                            }
                        }
                    }
                    xa.Commit();
                }
            }
            catch (Exception)
            {
                if (xa != null)
                    try { xa.Rollback(); }
                    catch (Exception) { }
                throw;
            }
            finally
            {
                if (xa != null)
                    try { xa.Dispose(); }
                    catch (Exception) { }
            }

            return result;
        }
        

        //To get xml to pass to SP from int list
        private static XmlDocument GetXMLFromList<T>(List<T> listParameter, string rootName, string childName)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<" + rootName + " ></" + rootName + ">");
            if (listParameter != null)
            {
                foreach (T item in listParameter)
                {
                    XmlNode root = doc.DocumentElement;
                    //Create a new node.
                    XmlElement elem = doc.CreateElement(childName);

                    XmlElement innerElem = doc.CreateElement("ID");
                    innerElem.InnerXml = item.ToString();
                    elem.AppendChild(innerElem);
                    root.AppendChild(elem);
                }
            }
            doc.Save(Console.Out);
            return doc;
        }


        /// <summary>
        /// To save blocked recommendations and databases
        /// 10.0 Srishti Purohit SQLdm
        /// <param name="blockedDatabaases"></param> 
        /// <param name="id"></param>
        /// <param name="blockedRecommendations"></param>
        /// <param name="repositoryConnectionString"></param>
        /// </summary>
        public static bool BlockRecommendationDatabaseAnalysisConfiguration(string repositoryConnectionString, int id, List<string> blockedRecommendations, List<int> blockedDatabaases)
        {
            bool isBlocked = false;
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            if (id == 0)
            {
                throw new ArgumentNullException("server incorrect");
            }
            SqlConnection connection = new SqlConnection(repositoryConnectionString);
            try
            {
                //DB call to save
                using (connection)
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, UpdateBlockedRecommendationDatabaseAnalysisConfiguration))
                    {
                        command.Parameters.Clear();

                        command.Parameters.AddWithValue("@sqlServerID", id);
                        command.Parameters.AddWithValue("@databaseID", GetXMLFromList(blockedDatabaases, "Database", "DatabaseID").InnerXml);
                        command.Parameters.AddWithValue("@recommendationID", GetXMLFromList(blockedRecommendations, "Recommendation", "RecommendationID").InnerXml);
                        command.ExecuteNonQuery();
                        isBlocked = true;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
            }
            return isBlocked;
        }
        /// <summary>
        /// SQLdm 10.0 (srishti purohit) -Update status of the list of Recommendation
        /// </summary>
        /// <param name="recommendation"></param>
        /// <returns></returns>
        public static bool UpdateRecommendationOptimizationStatus(string repositoryConnectionString, List<IRecommendation> recommendation)
        {
            bool updatedSuccessfully = false;
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            if (recommendation == null)
            {
                throw new ArgumentNullException("No recommendation to update.");
            }
            SqlConnection connection = new SqlConnection(repositoryConnectionString);
            try
            {
                //DB call to save
                using (connection)
                {
                    connection.Open();
                    foreach (var recomm in recommendation)
                    {
                        using (SqlCommand command = SqlHelper.CreateCommand(connection, UpdateRecommendationOptimizationStatusStoreProcedure))
                        {
                            command.Parameters.Clear();

                            command.Parameters.AddWithValue("@analysisRecommendationID", recomm.AnalysisRecommendationID);
                            command.Parameters.AddWithValue("@status", recomm.OptimizationStatus);
                            command.Parameters.AddWithValue("@errorMessageText", recomm.OptimizationErrorMessage);
                            command.Parameters.Add("@isStatusChanged", SqlDbType.Bit);
                            command.Parameters["@isStatusChanged"].Direction = ParameterDirection.Output;
                            command.ExecuteNonQuery();
                            updatedSuccessfully = Convert.ToBoolean(command.Parameters["@isStatusChanged"].Value);
                        }
                    }
                }
            }
            catch (Exception)
            {
                updatedSuccessfully = false;
                throw;
            }
            finally
            {
            }
            return updatedSuccessfully;
        }


        public static MonitoredSqlServer ActivateMonitoredSqlServer(string repositoryConnectionString, int id)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                using (SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(connection, ActivateMonitoredSqlServerStoredProcedure, id))
                {
                    if (dataReader.Read())
                    {
                        MonitoredSqlServer instance = ConstructMonitoredSqlServer(dataReader);

                        // Custom Counters
                        if (dataReader.NextResult() && dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                instance.CustomCounters.Add((int)dataReader[0]);
                            }
                        }

                        // Tags
                        if (dataReader.NextResult() && dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                instance.Tags.Add((int)dataReader[0]);
                            }
                        }
                        return instance;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public static bool DeleteMonitoredSqlServer(string repositoryConnectionString, int id)
        {
            using (LOG.DebugCall())
            {
                if (repositoryConnectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    int i = SqlHelper.ExecuteNonQuery(connection, DeleteMonitoredSqlServerStoredProcedure, id);
                    LOG.Debug("ExecuteNonQuery(", DeleteMonitoredSqlServerStoredProcedure, ", ", id, ") returned ", i);
                    LOG.Debug("Returning ", i > 0);
                    return i > 0;
                }
            }
        }

        public static bool DeactivateMonitoredSqlServer(string repositoryConnectionString, int id)
        {
            using (LOG.DebugCall())
            {
                if (repositoryConnectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    int i = SqlHelper.ExecuteNonQuery(connection, DeactivateMonitoredSqlServerStoredProcedure, id);
                    LOG.Debug("ExecuteNonQuery(", DeactivateMonitoredSqlServerStoredProcedure, ", ", id, ") returned ", i);
                    LOG.Debug("Returning ", i > 0);
                    return i > 0;
                }
            }
        }

        //SQLdm 8.6 -- (Ankit Srivastava) -- added one more parameters which contains all of metrics along with their compatibility for Sql Express
        public static void GetMonitoredSqlServerWorkload(string repositoryConnectionString, int serverId, out IDictionary<int, List<MetricThresholdEntry>> thresholds, out IList<OutstandingEventEntry> events, out Set<int> counters, out Dictionary<int, bool> metricCompatibilityForSqlExpress, out BaselineMetricMeanCollection baselineMetricMean, out int? cloudProviderId)
        {
            using (LOG.DebugCall("GetMonitoredSqlServerWorkload"))
            {
                if (repositoryConnectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }

                thresholds = new Dictionary<int, List<MetricThresholdEntry>>();
                events = new List<OutstandingEventEntry>();
                counters = new Set<int>();
                metricCompatibilityForSqlExpress = new Dictionary<int, bool>(); //SQLdm 8.6 -- (Ankit Srivastava) -- all of metrics along with their compatibility for Sql Express
                baselineMetricMean = new BaselineMetricMeanCollection(serverId); //SQLdm 10.0 -- (Srishti Purohit) -- all of metrics along with their Maean for Baseline values
                cloudProviderId = null;
                try
                {
                    using (SqlDataReader reader =
                        SqlHelper.ExecuteReader(repositoryConnectionString, GetMonitoredSqlServerWorkloadStoredProcedure,
                                                serverId))
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

                        //SQLdm 10.0 -- (Srishti Purohit) -- all of metrics along with their Maean for Baseline values
                        if (reader.NextResult())
                        {
                            baselineMetricMean = GetBaselineMean(reader, serverId);
                        }

                        //START: SQLdm 10.0 (Tarun Sapra)- Get the cloud provider of the sql server instance
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("CloudProviderId")))
                                    cloudProviderId = Convert.ToInt32(reader["CloudProviderId"]);
                                else
                                    cloudProviderId = null;
                            }
                        }
                        //END: SQLdm 10.0 (Tarun Sapra)- Get the cloud provider of the sql server instance
                    }
                }
                catch (Exception)
                {
                    LOG.ErrorFormat("Error getting server workload from repository for server id: {0}", serverId);
                    throw;
                }
            }
        }

        /// <summary>
        /// SQLdm 10.1 (Srishti Purohit)
        /// Revise Multiple Baseline for Independent Scheduling
        /// </summary>
        private static BaselineMetricMeanCollection GetBaselineMean(SqlDataReader reader, int serverId)
        {
            BaselineMetricMeanCollection baselineMetricMeanCollection = new BaselineMetricMeanCollection();
            baselineMetricMeanCollection.ServerId = serverId;
            BaselineMetricMean meanMetric = new BaselineMetricMean();
            List<BaselineMetricMean> meanMetricList = new List<BaselineMetricMean>();
            int ordinal = -1;
            try
            {
                LOG.Info("getting baseline mean using GetBaselineMean.");
                while (reader.Read())
                {
                    meanMetric = new BaselineMetricMean();
                    int templateId = reader.GetOrdinal("TemplateID");
                    if (!reader.IsDBNull(templateId))
                    {
                        templateId = reader.GetInt32(templateId);
                        meanMetric.BaselineConfig = new BaselineConfiguration((string)reader["Template"]);
                        meanMetric.BaselineConfig.TemplateID = templateId;
                        meanMetric.BaselineConfig.Active = (bool)reader["Active"];
                        meanMetric.Metric = Convert.ToInt32(reader["MetricID"]);
                        ordinal = reader.GetOrdinal("Mean");
                        if (reader.IsDBNull(ordinal))
                            meanMetric.Mean = null;
                        else
                            meanMetric.Mean = Convert.ToDouble(reader["Mean"]);

                        ordinal = reader.GetOrdinal("UTCCalculation");
                        if (!reader.IsDBNull(ordinal))
                            meanMetric.UTCCalculationTime = Convert.ToDateTime(reader["UTCCalculation"]);

                        meanMetricList.Add(meanMetric);
                    }
                }
            }
            catch (Exception ex)
            {
                LOG.Error("Exception occured in GetBaselineMean", ex);
            }
            baselineMetricMeanCollection.BaselineMeanServerMeanList.AddRange(meanMetricList);
            return baselineMetricMeanCollection;
        }
        public static int GetMonitoredSqlServer(string repositoryConnectionString, string instanceName)
        {
            using (LOG.InfoCall("GetMonitoredSqlServer"))
            {
                if (repositoryConnectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }

                object value = SqlHelper.ExecuteScalar(repositoryConnectionString, GetMonitoredSqlServerIdCommand, instanceName);
                if (value == null)
                    throw new ApplicationException(String.Format("Instance ({0}) is not being monitored by SQLdm.", instanceName));

                return (int)value;
            }
        }

        public static List<MonitoredSqlServer> GetMonitoredSqlServers(SqlConnection repositoryConnection, Guid? collectionServiceId, bool activeOnly)
        {
            List<MonitoredSqlServer> servers = new List<MonitoredSqlServer>();

            if (repositoryConnection == null)
            {
                throw new ArgumentNullException("repositoryConnection");
            }

            Dictionary<int, MonitoredSqlServer> serverMap = new Dictionary<int, MonitoredSqlServer>();

            using (
                SqlDataReader reader =
                    SqlHelper.ExecuteReader(repositoryConnection,
                                            GetMonitoredSqlServersCommand,
                                            collectionServiceId.HasValue ? (object)collectionServiceId.Value : null,
                                            activeOnly,
                                            false,
                                            true, null, null)) //SQLDM8.5 Mahesh: Added Additional Params
            {
                while (reader.Read())
                {
                    MonitoredSqlServer instance = ConstructMonitoredSqlServer(reader);
                    servers.Add(instance);
                    serverMap.Add(instance.Id, instance);
                }

                // Custom Counters
                if (reader.NextResult() && reader.HasRows)
                {
                    MonitoredSqlServer instance = null;
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        if (instance == null || instance.Id != id)
                        {
                            if (!serverMap.TryGetValue(id, out instance))
                                continue;
                        }
                        instance.CustomCounters.Add(reader.GetInt32(1));
                    }
                }
                // Tags
                if (reader.NextResult() && reader.HasRows)
                {
                    MonitoredSqlServer instance = null;
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        if (instance == null || instance.Id != id)
                        {
                            if (!serverMap.TryGetValue(id, out instance))
                                continue;
                        }

                        instance.Tags.Add(reader.GetInt32(1));
                    }
                }

                //Virtualization Configuration
                if (reader.NextResult() && reader.HasRows)
                {
                    MonitoredSqlServer instance = null;
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        if (instance == null || instance.Id != id)
                        {
                            if (!serverMap.TryGetValue(id, out instance))
                                continue;
                        }
                        vCenterHosts vcHost = new vCenterHosts((int)reader["VHostID"],
                                                               (string)reader["VHostName"],
                                                               (string)reader["VHostAddress"],
                                                               (string)reader["Username"],
                                                               (string)reader["Password"],
                                                               (string)reader["ServerType"]);
                        instance.VirtualizationConfiguration =
                            new VirtualizationConfiguration((string)reader["VmUID"], (string)reader["VmName"], (string)reader["VmDomainName"], vcHost);
                    }
                }
            }

            return servers;
        }

        public static List<MonitoredSqlServer> GetMonitoredSqlServers(string repositoryConnectionString, Guid? collectionServiceId, bool activeOnly)
        {
            using (LOG.InfoCall("GetMonitoredSqlServers"))
            {
                if (repositoryConnectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }

                using (SqlConnection repositoryConnection = new SqlConnection(repositoryConnectionString))
                {
                    repositoryConnection.Open();
                    return GetMonitoredSqlServers(repositoryConnection, collectionServiceId, activeOnly);
                }
            }
        }

        public static MonitoredSqlServer GetMonitoredSqlServer(string repositoryConnectionString, int instanceId)
        {
            MonitoredSqlServer instance = null;
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (
                SqlDataReader reader =
                    SqlHelper.ExecuteReader(repositoryConnectionString,
                                            GetMonitoredSqlServerByIdCommand,
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

        public static SnapshotValues GetPreviousSnapshotValuesForPrescriptiveAnalysis(string repositoryConnectionString, int instanceId)
        {
            SnapshotValues snapshotValues = new SnapshotValues(null);
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (
                SqlDataReader reader =
                    SqlHelper.ExecuteReader(repositoryConnectionString,
                                            GetPrescriptiveSnapshotValuesForServerStoreProcedure,
                                            instanceId))
            {
                if (reader.Read())
                {
                    snapshotValues = new SnapshotValues();
                    snapshotValues.ActiveNetworkCards = Convert.ToInt32(reader["ActiveNetworkCards"]);
                    snapshotValues.TotalNetworkBandwidth = Convert.ToUInt64(reader["TotalNetworkBandwidth"]);
                    snapshotValues.AllowedProcessorCount = Convert.ToInt32(reader["AllowedProcessorCount"]);
                    snapshotValues.TotalNumberOfLogicalProcessors = Convert.ToUInt32(reader["TotalNumberOfLogicalProcessors"]);
                    snapshotValues.TotalMaxClockSpeed = Convert.ToUInt64(reader["TotalMaxClockSpeed"]);
                    snapshotValues.TotalPhysicalMemory = Convert.ToUInt64(reader["TotalPhysicalMemory"]);
                    snapshotValues.MaxServerMemory = Convert.ToUInt64(reader["MaxServerMemory"]);
                    snapshotValues.WindowsVersion = Convert.ToString(reader["WindowsVersion"]);
                    snapshotValues.ProductVersion = Convert.ToString(reader["ProductVersion"]);
                    snapshotValues.SQLVersionString = Convert.ToString(reader["SQLVersionString"]);
                }
            }

            return snapshotValues;
        }

        public static List<Triple<int, string, bool>> GetMonitoredSqlServerNames(SqlConnection repositoryConnection, Guid? collectionServiceId, bool activeOnly)
        {
            List<Triple<int, string, bool>> servers = new List<Triple<int, string, bool>>();

            if (repositoryConnection == null)
            {
                throw new ArgumentNullException("repositoryConnection");
            }

            using (SqlDataReader reader =
                SqlHelper.ExecuteReader(repositoryConnection,
                                        GetMonitoredSqlServerNamesCommand,
                                        collectionServiceId.HasValue ? (object)collectionServiceId.Value : null,
                                        activeOnly))
            {
                while (reader.Read())
                {
                    Triple<int, string, bool> row = new Triple<int, string, bool>();
                    row.First = reader.GetInt32(0);
                    row.Second = reader.GetString(1);
                    row.Third = reader.GetBoolean(2);
                    servers.Add(row);
                }
            }

            return servers;
        }

        public static List<MonitoredSqlServerState> GetMonitoredSqlServersState(SqlConnection repositoryConnection, Guid? collectionServiceId, bool activeOnly)
        {
            List<MonitoredSqlServerState> servers = new List<MonitoredSqlServerState>();

            if (repositoryConnection == null)
            {
                throw new ArgumentNullException("repositoryConnection");
            }

            Dictionary<int, MonitoredSqlServer> serverMap = new Dictionary<int, MonitoredSqlServer>();

            using (
                SqlDataReader reader =
                    SqlHelper.ExecuteReader(repositoryConnection,
                                            GetMonitoredSqlServersCommand,
                                            collectionServiceId.HasValue ? (object)collectionServiceId.Value : null,
                                            activeOnly,
                                            false,
                                            true,
                                            null, null)) //SQLDM8.5 Mahesh: Added Additional Params
            {
                while (reader.Read())
                {
                    MonitoredSqlServer instance = ConstructMonitoredSqlServer(reader);
                    MonitoredSqlServerState state = new MonitoredSqlServerState(instance);
                    MonitoredSqlServerStateGraph graph = new MonitoredSqlServerStateGraph(state);
                    int columnid = reader.GetOrdinal("LastAlertRefreshTime");
                    if (!reader.IsDBNull(columnid))
                        graph.LastAlertRefreshTime = reader.GetDateTime(columnid);

                    servers.Add(state);
                    serverMap.Add(instance.Id, instance);
                }
                // Custom Counters
                if (reader.NextResult() && reader.HasRows)
                {
                    MonitoredSqlServer instance = null;
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        if (instance == null || instance.Id != id)
                        {
                            if (!serverMap.TryGetValue(id, out instance))
                                continue;
                        }
                        instance.CustomCounters.Add(reader.GetInt32(1));
                    }
                }
                // Tags
                if (reader.NextResult() && reader.HasRows)
                {
                    MonitoredSqlServer instance = null;
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        if (instance == null || instance.Id != id)
                        {
                            if (!serverMap.TryGetValue(id, out instance))
                                continue;
                        }

                        instance.Tags.Add(reader.GetInt32(1));
                    }
                }
            }

            return servers;
        }
		
		   #region Alerts
        //sqldm-30528

        public static AlertConfiguration GetAlertConfiguration(SqlConnection repositoryConnection, MonitoredSqlServer monitoredServer,
                                                               params int[] metrics)
        {
            return GetAlertConfiguration(repositoryConnection, monitoredServer, false, metrics);
        }

        private static AlertConfiguration GetAlertConfiguration(SqlConnection repositoryConnection, MonitoredSqlServer monitoredServer, bool getAlertTemplateConfiguration, params int[] metrics)
        {
            AlertConfiguration config = new AlertConfiguration(monitoredServer.Id);
            return GetAlertConfiguration(repositoryConnection, monitoredServer, config, getAlertTemplateConfiguration, metrics);
        }

        public static AlertConfiguration GetAlertConfiguration(SqlConnection repositoryConnection, MonitoredSqlServer monitoredServer, AlertConfiguration config, bool getAlertTemplateConfiguration, params int[] metrics)
        {
            if (repositoryConnection == null)
            {
                throw new ArgumentNullException("repositoryConnection");
            }

            if (config.MetricDefinitions == null)
                config.MetricDefinitions = new MetricDefinitions(false, false, true, false, monitoredServer.CloudProviderId);
            MetricDefinitions metricDefinitions = config.MetricDefinitions;
            metricDefinitions.Load(repositoryConnection.ConnectionString);

            Dictionary<int, List<MetricThresholdEntry>> thresholdEntries = GetMetricThresholds(repositoryConnection, config.InstanceID, getAlertTemplateConfiguration);

            int[] metricList = metrics;
            if (metrics == null || metrics.Length == 0)
                metricList = metricDefinitions.GetMetricDescriptionKeys();

            foreach (int metricID in metricList)
            {
                if (!thresholdEntries.ContainsKey(metricID))
                    continue;

                MetricDescription? metricDescription = metricDefinitions.GetMetricDescription(metricID);

                if (metricDescription == null)
                    continue;

                List<MetricThresholdEntry> mteList = thresholdEntries[metricID];

                foreach (MetricThresholdEntry mte in mteList)
                {
                    config.AddEntry(metricID, metricDescription.Value, mte);
                }
            }

            return config;
        }

        public static Dictionary<int, List<MetricThresholdEntry>> GetMetricThresholds(SqlConnection connection,
                                                                                   int instanceID, bool getAlertTemplateConfiguration)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            Dictionary<int, List<MetricThresholdEntry>> result = null;

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, "p_GetMetricThresholds", getAlertTemplateConfiguration ? (object)instanceID : DBNull.Value,
            instanceID, DBNull.Value))
            {
                result = GetMetricThresholds(dataReader, true);
            }
            return result;
        }

        private static Dictionary<int, List<MetricThresholdEntry>> GetMetricThresholds(SqlDataReader reader, bool value)
        {
            Dictionary<int, List<MetricThresholdEntry>> result = new Dictionary<int, List<MetricThresholdEntry>>();
            while (reader.Read())
            {
                // get the monitored server (parent)
                int sqlServerId = reader.GetInt32(0);

                // get the metric
                int metricID = reader.GetInt32(1);

                // threshold type name                
                Threshold warning = null;
                Threshold critical = null;
                Threshold info = null;
                //10.0 srishti purohit -- baseline support
                Threshold warningBaseline = null;
                Threshold criticalBaseline = null;
                Threshold infoBaseline = null;
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

                // get the informational threshold
                SqlString infoXML = reader["InfoThreshold"].ToString();

                if (!infoXML.IsNull)
                {
                    info = Threshold.Deserialize(infoXML.Value);
                }

                // get the warning threshold Baseline
                SqlString warningXmlBaseline = reader["BaselineWarningThreshold"].ToString();
                if (!warningXmlBaseline.IsNull && warningXmlBaseline.Value != "")
                {
                    warningBaseline = Threshold.Deserialize(warningXmlBaseline.Value);
                }

                // get the critical threshold Baseline
                SqlString criticalXmlBaseline = reader["BaselineCriticalThreshold"].ToString();
                if (!criticalXmlBaseline.IsNull && criticalXmlBaseline.Value != "")
                {
                    criticalBaseline = Threshold.Deserialize(criticalXmlBaseline.Value);
                }

                // get the informational threshold Baseline
                SqlString infoXMLBaseline = reader["BaselineInfoThreshold"].ToString();
                if (!infoXMLBaseline.IsNull && infoXMLBaseline.Value != "")
                {
                    infoBaseline = Threshold.Deserialize(infoXMLBaseline.Value);
                }

                SqlString dataXml = reader.GetSqlString(5);
                if (!dataXml.IsNull)
                {
                    data = Threshold.DeserializeData(dataXml.Value);
                    if (!(data is AdvancedAlertConfigurationSettings))
                        data = new AdvancedAlertConfigurationSettings(MetricDefinition.GetMetric(metricID), data);
                }

                MetricThresholdEntry entry = new MetricThresholdEntry(sqlServerId, metricID, warning, critical, info, warningBaseline, criticalBaseline, infoBaseline);
                entry.Data = data;

                if (reader.FieldCount > 13)
                {
                    // attach snooze info to advanced alert configuration settings
                    if (!(reader.IsDBNull(6) || reader.IsDBNull(7)))
                    {
                        AdvancedAlertConfigurationSettings settings = data as AdvancedAlertConfigurationSettings;
                        if (settings == null)
                        {
                            settings =
                                new AdvancedAlertConfigurationSettings(MetricDefinition.GetMetric(metricID), data);
                            entry.Data = settings;
                        }
                        string startUser = reader.IsDBNull(8) ? String.Empty : reader.GetString(8);
                        string endUser = reader.IsDBNull(9) ? String.Empty : reader.GetString(9);
                        SnoozeInfo snoozeInfo =
                            new SnoozeInfo(reader.GetDateTime(6), reader.GetDateTime(7), startUser, endUser);
                        settings.SnoozeInfo = snoozeInfo;
                    }
                }

                if (!reader.GetBoolean(2))
                    entry.IsEnabled = false;

                entry.IsThresholdEnabled = Convert.ToBoolean(reader["ThresholdEnabled"]);
                entry.IsBaselineEnabled = Convert.ToBoolean(reader["IsBaselineEnabled"]);

                entry.MetricInstanceName = reader["ThresholdInstanceName"].ToString();

                if (result.ContainsKey(metricID))
                {
                    result[metricID].Add(entry);
                }
                else
                {
                    result.Add(metricID, new List<MetricThresholdEntry>() { entry });
                }
            }
            return result;
        }
        //End Sqldm-30528
        #endregion
		

        private static int GetVirtualHostID(string connectionString, string address)
        {
            int hostID = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlDataReader dr = SqlHelper.ExecuteReader(connection, GetVirtualHostServersByAddressCommand, address))
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        hostID = Convert.ToInt32(dr["vcHostID"]);
                    }
                }
            }

            return hostID;
        }

        private static MonitoredSqlServer ConstructMonitoredSqlServer(SqlDataReader dataReader)
        {
            if (dataReader == null)
            {
                throw new ArgumentNullException("dataReader");
            }

            int returnId = (int)dataReader["SQLServerID"];
            SqlConnectionInfo instanceConnectionInfo = new SqlConnectionInfo();
            instanceConnectionInfo.ApplicationName = Constants.CollectionServceConnectionStringApplicationName;
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
                dataReader.IsDBNull(dataReader.GetOrdinal("QueryMonitorStopTimeUTC"))
                    ? null
                    : (DateTime?)dataReader["QueryMonitorStopTimeUTC"],
                (bool)dataReader["QueryMonitorTraceMonitoringEnabled"],
                (bool)dataReader["QueryMonitorCollectQueryPlan"],
                (bool)dataReader["QueryMonitorCollectEstimatedQueryPlan"], //SQLdm 9.0 (Ankit Srivastava)-- Extended Event Query Monitoring --Get the newly added columns from  the repository
                (int)dataReader["QueryMonitorTopPlanCountFilter"],
                (int)dataReader["QueryMonitorTopPlanCategoryFilter"],
                (bool)dataReader["QueryMonitorQueryStoreMonitoringEnabled"]);  // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store

            //START SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new property values for query monitoring extended event session configuration
            queryMonitorConfiguration.FileSizeXeMB = (int)dataReader["QueryMonitorXEFileSizeMB"];
            queryMonitorConfiguration.FileSizeRolloverXe = (int)dataReader["QueryMonitorXEFilesRollover"];
            //END SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new property values  for query monitoring extended event session configuration

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
                dataReader.IsDBNull(dataReader.GetOrdinal("QueryMonitorStopTimeUTC")) ? null : (DateTime?)dataReader["QueryMonitorStopTimeUTC"],
                (bool)dataReader["ActivityMonitorTraceMonitoringEnabled"]);//SQLdm 9.1 (Ankit Srivastava)-- Extended Event Activity Monitoring --Get the newly added column from  the repository

            //START SQLdm 9.1 (Ankit Srivastava)-- Extended Event Activity Monitoring --Fetch the newly added column from  the repository
            activityMonitorConfiguration.FileSizeXeMB = (int)dataReader["ActivityMonitorXEFileSizeMB"];
            activityMonitorConfiguration.FileSizeRolloverXe = (int)dataReader["ActivityMonitorXEFilesRollover"];
            activityMonitorConfiguration.RecordsPerRefreshXe = (int)dataReader["ActivityMonitorXERecordsPerRefresh"];
            activityMonitorConfiguration.MaxMemoryXeMB = (int)dataReader["ActivityMonitorXEMaxMemoryMB"];
            activityMonitorConfiguration.EventRetentionModeXe = (XeEventRetentionMode)(byte)dataReader["ActivityMonitorXEEventRetentionMode"];
            activityMonitorConfiguration.MaxDispatchLatencyXe = (int)dataReader["ActivityMonitorXEMaxDispatchLatencySecs"];
            activityMonitorConfiguration.MaxEventSizeXemb = (int)dataReader["ActivityMonitorXEMaxEventSizeMB"];
            activityMonitorConfiguration.MemoryPartitionModeXe = (XEMemoryPartitionMode)dataReader["ActivityMonitorXEMemoryPartitionMode"];
            activityMonitorConfiguration.TrackCausalityXe = (bool)dataReader["ActivityMonitorXETrackCausality"];
            activityMonitorConfiguration.StartupStateXe = (bool)dataReader["ActivityMonitorXEStartupState"];
            activityMonitorConfiguration.FileNameXEsession = (string)dataReader["ActivityMonitorXEFileName"];
            //END SQLdm 9.1 (Ankit Srivastava)-- Extended Event Activity Monitoring --Fetch the newly added column from  the repository

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
                catch (Exception e)
                {
                    LOG.Error("Exception deserializing TableStatisticsExcludedDatabases", e);
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
            LOG.Info(instance.InstanceName, " Ole Automation context is ", instance.OleAutomationContext.ToString());

            ordinal = dataReader.GetOrdinal("DisableExtendedHistoryCollection");
            if (!dataReader.IsDBNull(ordinal))
                instance.ExtendedHistoryCollectionDisabled = (bool)dataReader["DisableExtendedHistoryCollection"];

            ordinal = dataReader.GetOrdinal("DisableOleAutomation");
            if (!dataReader.IsDBNull(ordinal))
                instance.OleAutomationUseDisabled = (bool)dataReader["DisableOleAutomation"];
            else
                instance.OleAutomationUseDisabled = false;  // set this explicitly

            if (instance.OleAutomationUseDisabled)
                LOG.Info(instance.InstanceName, " use of Ole Automation is disabled");

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
                    LOG.Error("Exception deserializing DiskCollectionSettings", e);
                }
            }

            ordinal = dataReader.GetOrdinal("InputBufferLimiter");
            if (!dataReader.IsDBNull(ordinal))
                instance.InputBufferLimiter = dataReader.GetInt32(ordinal);

            //Capturing current value of Friendly Name for instance in object
            //SQLdm 10.1 (Srishti Purohit) Power shell 
            ordinal = dataReader.GetOrdinal("FriendlyServerName");
            if (!dataReader.IsDBNull(ordinal))
                instance.FriendlyServerName = dataReader.GetString(ordinal);

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
                instance.BaselineConfiguration.BaselineName = Common.Constants.DEFAULT_BASELINE_NAME;
                instance.BaselineConfiguration.Active = true;
            }
            #region Custom Baselines
            ordinal = dataReader.GetOrdinal("CustomBaselineTemplates");
            if (!dataReader.IsDBNull(ordinal) && !string.IsNullOrEmpty(dataReader.GetString(ordinal)))
                instance.BaselineConfigurationList = Common.Data.BaselineHelpers.GetBaselineDictionaryFromArray(dataReader.GetString(ordinal).Split(','));
            else
                instance.BaselineConfigurationList = new Dictionary<int, BaselineConfiguration>();
            #endregion

            if (dataReader["ServerVersion"] != DBNull.Value)
                instance.MostRecentSQLVersion = new ServerVersion((string)dataReader["ServerVersion"]);

            if (dataReader["ServerEdition"] != DBNull.Value)
                instance.MostRecentSQLEdition = (string)dataReader["ServerEdition"];
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
                    , (bool)dataReader["SchedulingStatus"]); // Start:SQLDm 10.0 - Praveen Suhalka - Scheduling status
            }
            #endregion

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
                LOG.Error(ex.Message + " Error while getting blocked categories or blocked databases.");
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

        public static void UpdateLastRefreshTime(SqlTransaction transaction, int instanceId, DateTime snapshotDateTime, Type refreshType)
        {

            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            try
            {
                if (refreshType == typeof(DatabaseSizeSnapshot))
                {
                    SqlHelper.ExecuteNonQuery(transaction, UpdateLastDatabaseRefreshTimeStoredProcedure, instanceId, snapshotDateTime);
                }
                else
                {
                    SqlHelper.ExecuteNonQuery(transaction, UpdateLastRefreshTimeStoredProcedure, instanceId, snapshotDateTime);
                }

            }
            catch (Exception e)
            {
                LOG.Error("Update last refresh time failed - this will hose up the list of active alerts", e);
                throw;
            }
        }

        public static bool AddRecoredAdvanceFilteringAlert(SqlTransaction transaction, string monitoredServerName, int MetricID, MonitoredState MonitoredState,string DataBaseName)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            try
            {
                int? result = (int) SqlHelper.ExecuteScalar(transaction, AddAdvanceFilteringAlert, monitoredServerName, MetricID, MonitoredState, DataBaseName);

                if (result == null || !result.HasValue)
                    return true;

                return result == 1;
            }
            catch (Exception e)
            {
                LOG.Error("Insert Recored Faild in Advance Filtering Alert", e);
                throw;
            }
        }

        #endregion

        #region Task

        public static void UpdateTask(string repositoryConnectionString, int id, TaskStatus status, string assignedTo, string comments)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                SqlHelper.ExecuteNonQuery(connection, UpdateTaskStoredProcedure, id, (byte)status, assignedTo, comments);
            }
        }

        public static void DeleteTasks(string repositoryConnectionString, IEnumerable<int> taskIDs)
        {
            // create an xml document containing the tasks to be deleted
            XmlDocument tasksParameter = new XmlDocument();
            XmlElement tasksElement = tasksParameter.CreateElement("Tasks");
            tasksParameter.AppendChild(tasksElement);

            foreach (int id in taskIDs)
            {
                XmlElement instanceElement = tasksParameter.CreateElement("Task");
                instanceElement.SetAttribute("TaskID", id.ToString());
                tasksElement.AppendChild(instanceElement);
            }

            SqlHelper.ExecuteReader(repositoryConnectionString,
                                    DeleteTasksStoredProcedure,
                                    tasksParameter.InnerXml,
                                    null);
        }

        #endregion

        #region Alerts

        internal static void ChangeAlertTemplateConfiguration(string repositoryConnectionString, AlertConfiguration configuration)
        {
            ChangeAlertConfiguration(repositoryConnectionString, configuration, true);
        }

        internal static void ChangeAlertConfiguration(string repositoryConnectionString, AlertConfiguration configuration)
        {
            ChangeAlertConfiguration(repositoryConnectionString, configuration, false);
        }

        private static void ChangeAlertConfiguration(string repositoryConnectionString, AlertConfiguration configuration, bool templateConfiguration)
        {
            using (LOG.DebugCall())
            {
                if (repositoryConnectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();
                    SqlTransaction xa = connection.BeginTransaction();

                    try
                    {
                        using (SqlCommand metricInfoCommand =
                                                SqlHelper.CreateCommand(connection, UpdateMetricInfoStoredProcedure),
                                          deleteThresholdInstance =
                                                SqlHelper.CreateCommand(connection, DeleteThresholdInstanceStoredProcedure),
                                          addThresholdInstance =
                                                SqlHelper.CreateCommand(connection, AddThresholdInstanceStoredProcedure),
                                          updateThresholdInstance =
                                                SqlHelper.CreateCommand(connection, UpdateMetricThresholdStoredProcedure))
                        {
                            metricInfoCommand.Transaction = xa;
                            deleteThresholdInstance.Transaction = xa;
                            addThresholdInstance.Transaction = xa;
                            updateThresholdInstance.Transaction = xa;

                            foreach (object o in configuration.ChangeItems)
                            {
                                if (o is Pair<int, MetricDescription>)
                                {
                                    Pair<int, MetricDescription> metricDesc = (Pair<int, MetricDescription>)o;

                                    SqlHelper.AssignParameterValues(metricInfoCommand.Parameters,
                                                                  (int)metricDesc.First,
                                                                  metricDesc.Second.Comments,metricDesc.Second.Rank);

                                    LOG.DebugFormat("Calling {0} for metric {1}.", UpdateMetricInfoStoredProcedure, metricDesc.First);
                                    metricInfoCommand.ExecuteNonQuery();
                                }
                                else if (o is MetricThresholdEntry)
                                {
                                    string infoXml;
                                    string warningXml;
                                    string criticalXml;
                                    string dataXml = null;
                                    //10.0 SQLdm Srishti Purohit To add threshold for basesline comparision
                                    string baselineInfoXml = null;
                                    string baselineWarningXml = null;
                                    string baselineCriticalXml = null;

                                    object snoozeStartTime = null;
                                    object snoozeEndTime = null;
                                    string snoozeStartUser = null;
                                    string snoozeEndUser = null;

                                    MetricThresholdEntry entry = (MetricThresholdEntry)o;

                                    AdvancedAlertConfigurationSettings settings = entry.Data as AdvancedAlertConfigurationSettings;
                                    if (settings != null)
                                    {
                                        SnoozeInfo snoozeInfo = settings.SnoozeInfo;
                                        if (snoozeInfo != null)
                                        {
                                            snoozeStartTime = snoozeInfo.StartSnoozing;
                                            snoozeEndTime = snoozeInfo.StopSnoozing;
                                            snoozeStartUser = snoozeInfo.SnoozedBy;
                                            snoozeEndUser = snoozeInfo.UnsnoozedBy;
                                        }
                                    }

                                    Threshold.Serialize(entry.InfoThreshold, out infoXml);
                                    Threshold.Serialize(entry.WarningThreshold, out warningXml);
                                    Threshold.Serialize(entry.CriticalThreshold, out criticalXml);
                                    //10.0 SQLdm Srishti Purohit To add threshold for basesline comparision
                                    if (entry.BaselineInfoThreshold != null)
                                        Threshold.Serialize(entry.BaselineInfoThreshold, out baselineInfoXml);
                                    if (entry.BaselineWarningThreshold != null)
                                        Threshold.Serialize(entry.BaselineWarningThreshold, out baselineWarningXml);
                                    if (entry.BaselineCriticalThreshold != null)
                                        Threshold.Serialize(entry.BaselineCriticalThreshold, out baselineCriticalXml);

                                    if (entry.Data != null)
                                        Threshold.SerializeData(entry.Data, out dataXml);

                                    switch (entry.State)
                                    {
                                        case ThresholdState.Deleted:
                                            SqlHelper.AssignParameterValues(deleteThresholdInstance.Parameters,
                                                                            templateConfiguration
                                                                                ? entry.MonitoredServerID
                                                                                : (object)DBNull.Value,
                                                                            templateConfiguration
                                                                                ? (object)DBNull.Value
                                                                                : entry.MonitoredServerID,
                                                                            entry.MetricID,
                                                                            entry.MetricInstanceName,
                                                                            entry.MetricInstanceType);

                                            LOG.DebugFormat("Deleting threshold instance {0} for metric {1}.",
                                                            entry.MetricInstanceName, entry.MetricID);
                                            deleteThresholdInstance.ExecuteNonQuery();
                                            break;
                                        case ThresholdState.Added:
                                            SqlHelper.AssignParameterValues(addThresholdInstance.Parameters,
                                                                            templateConfiguration
                                                                                ? entry.MonitoredServerID
                                                                                : (object)DBNull.Value,
                                                                            templateConfiguration
                                                                                ? (object)DBNull.Value
                                                                                : entry.MonitoredServerID,
                                                                            entry.MetricID,
                                                                            entry.IsEnabled,
                                                                            warningXml,
                                                                            criticalXml,
                                                                            dataXml,
                                                                            infoXml,
                                                                            entry.MetricInstanceType,
                                                                            entry.IsDefaultThreshold
                                                                                ? string.Empty
                                                                                : entry.MetricInstanceName,
                                                                            entry.IsThresholdEnabled,
                                                                            entry.IsBaselineEnabled,
                                                                            baselineWarningXml,
                                                                            baselineCriticalXml,
                                                                            baselineInfoXml);

                                            LOG.DebugFormat("Adding threshold instance {0} for metric {1}.",
                                                            entry.MetricInstanceName, entry.MetricID);
                                            addThresholdInstance.ExecuteNonQuery();
                                            break;
                                        case ThresholdState.Changed:
                                            SqlHelper.AssignParameterValues(updateThresholdInstance.Parameters,
                                                                            templateConfiguration
                                                                                ? entry.MonitoredServerID
                                                                                : (object)DBNull.Value,
                                                                            templateConfiguration
                                                                                ? (object)DBNull.Value
                                                                                : entry.MonitoredServerID,
                                                                            entry.MetricID,
                                                                            entry.IsEnabled,
                                                                            warningXml,
                                                                            criticalXml,
                                                                            dataXml,
                                                                            snoozeStartTime,
                                                                            snoozeEndTime,
                                                                            snoozeStartUser,
                                                                            snoozeEndUser,
                                                                            infoXml,
                                                                            entry.IsDefaultThreshold
                                                                                ? string.Empty
                                                                                : entry.MetricInstanceName,
                                                                            entry.MetricInstanceType,
                                                                            entry.IsThresholdEnabled,
                                                                            entry.IsBaselineEnabled,
                                                                            baselineWarningXml,
                                                                            baselineCriticalXml,
                                                                            baselineInfoXml);

                                            LOG.DebugFormat("Changing threshold instance {0} for metric {1}.",
                                                            entry.MetricInstanceName, entry.MetricID);
                                            updateThresholdInstance.ExecuteNonQuery();
                                            break;
                                    }
                                }
                            }
                        }
                        LOG.Debug("Commiting.");
                        xa.Commit();
                    }
                    catch (Exception e)
                    {
                        xa.Rollback();
                        LOG.Warn("The following exception occurred in ChangeAlertConfiguration.\n\n", e);
                        throw;
                    }
                    finally
                    {
                        xa.Dispose();
                    }
                }
            }
        }

        public static void UpdateAlertConfiguration(string repositoryConnectionString, IEnumerable<MetricThresholdEntry> entries, IEnumerable<int> targets)
        {
            using (LOG.InfoCall("UpdateAlertConfiguration"))
            {
                if (repositoryConnectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();

                    Dictionary<Pair<int, string>, MetricThresholdEntry> existingTargetEntries =
                        new Dictionary<Pair<int, string>, MetricThresholdEntry>();

                    foreach (int targetId in targets)
                    {
                        SqlTransaction xa = connection.BeginTransaction();
                        try
                        {
                            using (SqlCommand deleteThresholdInstance =
                                SqlHelper.CreateCommand(connection, DeleteThresholdInstanceStoredProcedure),
                                              addThresholdInstance =
                                                  SqlHelper.CreateCommand(connection,
                                                                          AddThresholdInstanceStoredProcedure),
                                              updateThresholdInstance =
                                                  SqlHelper.CreateCommand(connection,
                                                                          UpdateMetricThresholdStoredProcedure))
                            {
                                deleteThresholdInstance.Transaction = xa;
                                addThresholdInstance.Transaction = xa;
                                updateThresholdInstance.Transaction = xa;

                                existingTargetEntries.Clear();

                                foreach (
                                    MetricThresholdEntry entry in
                                        GetMetricThresholds(repositoryConnectionString, targetId))
                                {
                                    Pair<int, string> mteKey =
                                        new Pair<int, string>(entry.MetricID,
                                                              entry.IsDefaultThreshold
                                                                  ? String.Empty
                                                                  : entry.MetricInstanceName);

                                    if (!existingTargetEntries.ContainsKey(mteKey))
                                    {
                                        existingTargetEntries.Add(mteKey, entry);
                                    }
                                }

                                foreach (MetricThresholdEntry entry in entries)
                                {
                                    string warningXml;
                                    string criticalXml;
                                    string infoXml;
                                    string dataXml = null;
                                    object snoozeStartTime = null;
                                    object snoozeEndTime = null;
                                    string snoozeStartUser = null;
                                    string snoozeEndUser = null;
                                    //10.0 SQLdm Srishti Purohit To add threshold for basesline comparision
                                    string baselineInfoXml = null;
                                    string baselineWarningXml = null;
                                    string baselineCriticalXml = null;

                                    AdvancedAlertConfigurationSettings settings =
                                        entry.Data as AdvancedAlertConfigurationSettings;
                                    if (settings != null)
                                    {
                                        SnoozeInfo snoozeInfo = settings.SnoozeInfo;
                                        if (snoozeInfo != null)
                                        {
                                            snoozeStartTime = snoozeInfo.StartSnoozing;
                                            snoozeEndTime = snoozeInfo.StopSnoozing;
                                            snoozeStartUser = snoozeInfo.SnoozedBy;
                                            snoozeEndUser = snoozeInfo.UnsnoozedBy;
                                        }
                                    }

                                    Threshold.Serialize(entry.InfoThreshold, out infoXml);
                                    Threshold.Serialize(entry.WarningThreshold, out warningXml);
                                    Threshold.Serialize(entry.CriticalThreshold, out criticalXml);
                                    //10.0 SQLdm Srishti Purohit To add threshold for basesline comparision
                                    if (entry.BaselineInfoThreshold != null)
                                        Threshold.Serialize(entry.BaselineInfoThreshold, out baselineInfoXml);
                                    if (entry.BaselineWarningThreshold != null)
                                        Threshold.Serialize(entry.BaselineWarningThreshold, out baselineWarningXml);
                                    if (entry.BaselineCriticalThreshold != null)
                                        Threshold.Serialize(entry.BaselineCriticalThreshold, out baselineCriticalXml);

                                    if (entry.Data != null)
                                        Threshold.SerializeData(entry.Data, out dataXml);

                                    Pair<int, string> mteLookupKey =
                                        new Pair<int, string>(entry.MetricID,
                                                              entry.IsDefaultThreshold
                                                                  ? String.Empty
                                                                  : entry.MetricInstanceName);

                                    switch (entry.State)
                                    {
                                        case ThresholdState.Deleted:
                                            if (existingTargetEntries.ContainsKey(mteLookupKey))
                                            {
                                                SqlHelper.AssignParameterValues(
                                                    deleteThresholdInstance.Parameters,
                                                    (object)DBNull.Value,
                                                    targetId,
                                                    entry.MetricID,
                                                    entry.MetricInstanceName,
                                                    entry.MetricInstanceType);

                                                LOG.DebugFormat("Deleting threshold instance {0} for metric {1}.",
                                                                entry.MetricInstanceName, entry.MetricID);
                                                deleteThresholdInstance.ExecuteNonQuery();

                                                existingTargetEntries.Remove(mteLookupKey);
                                            }
                                            break;
                                        case ThresholdState.Added:
                                        case ThresholdState.Changed:
                                            if (existingTargetEntries.ContainsKey(mteLookupKey))
                                            {
                                                SqlHelper.AssignParameterValues(
                                                    updateThresholdInstance.Parameters,
                                                    (object)DBNull.Value,
                                                    targetId,
                                                    entry.MetricID,
                                                    entry.IsEnabled,
                                                    warningXml,
                                                    criticalXml,
                                                    dataXml,
                                                    snoozeStartTime,
                                                    snoozeEndTime,
                                                    snoozeStartUser,
                                                    snoozeEndUser,
                                                    infoXml,
                                                    entry.IsDefaultThreshold ? String.Empty : entry.MetricInstanceName,
                                                    entry.MetricInstanceType,
                                                    entry.IsThresholdEnabled,
                                                    entry.IsBaselineEnabled,
                                                     baselineWarningXml,
                                                     baselineCriticalXml,
                                                     baselineInfoXml
                                                    );

                                                LOG.VerboseFormat("Update threshold: {0} {1}", targetId, entry.MetricID);
                                                updateThresholdInstance.ExecuteNonQuery();
                                            }
                                            else
                                            {
                                                SqlHelper.AssignParameterValues(
                                                    addThresholdInstance.Parameters,
                                                    (object)DBNull.Value,
                                                    targetId,
                                                    entry.MetricID,
                                                    entry.IsEnabled,
                                                    warningXml,
                                                    criticalXml,
                                                    dataXml,
                                                    infoXml,
                                                    entry.MetricInstanceType,
                                                    entry.IsDefaultThreshold ? String.Empty : entry.MetricInstanceName,
                                                    entry.IsThresholdEnabled,
                                                    entry.IsBaselineEnabled,
                                                     baselineWarningXml,
                                                     baselineCriticalXml,
                                                     baselineInfoXml);

                                                LOG.DebugFormat("Adding threshold instance {0} for metric {1}.",
                                                                entry.MetricInstanceName, entry.MetricID);
                                                addThresholdInstance.ExecuteNonQuery();
                                            }
                                            break;
                                    }
                                }
                            }
                            LOG.Verbose("Comitting template threshold changes");
                            xa.Commit();
                        }
                        catch (Exception e)
                        {
                            LOG.Error("Error applying template tp server {0}: ", targetId, e);
                            try
                            {
                                xa.Rollback();
                            }
                            catch (Exception r)
                            {
                                LOG.Error("Error applying template: (rollback for server {0} failed)", targetId, r);
                            }
                            throw;
                        }
                        finally
                        {
                            xa.Dispose();
                        }
                    }

                    connection.Close();
                }
            }
        }
        //Add Alert Template
        public static void AddAlertTemplate(string repositoryConnectionString, int TemplateID, IEnumerable<int> targets)
        {
            using (LOG.InfoCall("AddAlertTemplate"))
            {
                if (repositoryConnectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }
                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();
                    string SQLserverID = string.Join(",", targets.ToArray());
                    SqlTransaction xa = connection.BeginTransaction();
                    try
                    {
                        using (SqlCommand addThresholdInstance =
                                            SqlHelper.CreateCommand(connection, AddAlertInstanceTemplateStoredProcedure))
                        {
                            addThresholdInstance.Transaction = xa;
                            SqlHelper.AssignParameterValues(addThresholdInstance.Parameters,
                                TemplateID,
                                SQLserverID);
                            LOG.DebugFormat("Adding Template Id {0} for Instance {1}.",
                                                      TemplateID, SQLserverID);
                            addThresholdInstance.ExecuteNonQuery();
                        }
                        LOG.Verbose("Comitting Alert template Instance changes");
                        xa.Commit();
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Error Adding Alert Template Instance {0}: ", SQLserverID, e);
                        try
                        {
                            xa.Rollback();
                        }
                        catch (Exception r)
                        {
                            LOG.Error("Error Adding Alert template: (rollback for server {0} failed)", SQLserverID, r);
                        }
                        throw;
                    }
                    finally
                    {
                        xa.Dispose();
                    }
                }
            }
        }
        public static void ReplaceAlertConfiguration(string repositoryConnectionString, IEnumerable<MetricThresholdEntry> entries, IEnumerable<int> targets, bool defaultConfiguration)
        {
            using (LOG.InfoCall("ReplaceAlertConfiguration"))
            {
                if (repositoryConnectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();

                    // if an alert is being disabled the metric update sp will delete 
                    // all alerts for that metric - need to serialize access to the alerts table
                    lock (Management.ScheduledCollection.AlertTableSyncRoot)
                    {
                        Dictionary<Pair<int, string>, MetricThresholdEntry> existingTargetEntries =
                            new Dictionary<Pair<int, string>, MetricThresholdEntry>();

                        foreach (int targetId in targets)
                        {
                            SqlTransaction xa = connection.BeginTransaction();
                            try
                            {
                                using (SqlCommand deleteThresholdInstance =
                                                    SqlHelper.CreateCommand(connection, DeleteThresholdInstanceStoredProcedure),
                                                  addThresholdInstance =
                                                    SqlHelper.CreateCommand(connection, AddThresholdInstanceStoredProcedure),
                                                  updateThresholdInstance =
                                                    SqlHelper.CreateCommand(connection, UpdateMetricThresholdStoredProcedure))
                                {
                                    deleteThresholdInstance.Transaction = xa;
                                    addThresholdInstance.Transaction = xa;
                                    updateThresholdInstance.Transaction = xa;

                                    existingTargetEntries.Clear();

                                    foreach (MetricThresholdEntry entry in GetMetricThresholds(repositoryConnectionString, targetId))
                                    {
                                        Pair<int, string> mteKey =
                                            new Pair<int, string>(entry.MetricID, entry.IsDefaultThreshold ? String.Empty : entry.MetricInstanceName);

                                        if (!existingTargetEntries.ContainsKey(mteKey))
                                        {
                                            existingTargetEntries.Add(mteKey, entry);
                                        }
                                    }

                                    foreach (MetricThresholdEntry entry in entries)
                                    {
                                        string warningXml;
                                        string criticalXml;
                                        string infoXml;
                                        string dataXml = null;
                                        //10.0 SQLdm Srishti Purohit To add threshold for basesline comparision
                                        string baselineInfoXml;
                                        string baselineWarningXml;
                                        string baselineCriticalXml;
                                        object snoozeStartTime = null;
                                        object snoozeEndTime = null;
                                        string snoozeStartUser = null;
                                        string snoozeEndUser = null;

                                        AdvancedAlertConfigurationSettings settings =
                                            entry.Data as AdvancedAlertConfigurationSettings;
                                        if (settings != null)
                                        {
                                            SnoozeInfo snoozeInfo = settings.SnoozeInfo;
                                            if (snoozeInfo != null)
                                            {
                                                snoozeStartTime = snoozeInfo.StartSnoozing;
                                                snoozeEndTime = snoozeInfo.StopSnoozing;
                                                snoozeStartUser = snoozeInfo.SnoozedBy;
                                                snoozeEndUser = snoozeInfo.UnsnoozedBy;
                                            }
                                        }

                                        Threshold.Serialize(entry.InfoThreshold, out infoXml);
                                        Threshold.Serialize(entry.WarningThreshold, out warningXml);
                                        Threshold.Serialize(entry.CriticalThreshold, out criticalXml);
                                        //10.0 SQLdm Srishti Purohit To add threshold for basesline comparision
                                        Threshold.Serialize(entry.BaselineInfoThreshold, out baselineInfoXml);
                                        Threshold.Serialize(entry.BaselineWarningThreshold, out baselineWarningXml);
                                        Threshold.Serialize(entry.BaselineCriticalThreshold, out baselineCriticalXml);

                                        if (entry.Data != null)
                                            Threshold.SerializeData(entry.Data, out dataXml);

                                        Pair<int, string> mteLookupKey =
                                            new Pair<int, string>(entry.MetricID, entry.IsDefaultThreshold ? String.Empty : entry.MetricInstanceName);

                                        // if exists, update
                                        if (existingTargetEntries.ContainsKey(mteLookupKey))
                                        {
                                            SqlHelper.AssignParameterValues(
                                                updateThresholdInstance.Parameters,
                                                defaultConfiguration ? targetId : (object)DBNull.Value,
                                                defaultConfiguration ? (object)DBNull.Value : targetId,
                                                entry.MetricID,
                                                entry.IsEnabled,
                                                warningXml,
                                                criticalXml,
                                                dataXml,
                                                snoozeStartTime,
                                                snoozeEndTime,
                                                snoozeStartUser,
                                                snoozeEndUser,
                                                infoXml,
                                                 entry.IsDefaultThreshold ? String.Empty : entry.MetricInstanceName,
                                                entry.MetricInstanceType,
                                                entry.IsThresholdEnabled,
                                                entry.IsBaselineEnabled,
                                                baselineWarningXml,
                                                baselineCriticalXml,
                                                baselineInfoXml
                                                );

                                            LOG.VerboseFormat("Update threshold: {0} {1}", targetId, entry.MetricID);
                                            updateThresholdInstance.ExecuteNonQuery();

                                            existingTargetEntries.Remove(mteLookupKey);
                                        }
                                        else // else add it
                                        {
                                            SqlHelper.AssignParameterValues(
                                                addThresholdInstance.Parameters,
                                                defaultConfiguration ? targetId : (object)DBNull.Value,
                                                defaultConfiguration ? (object)DBNull.Value : targetId,
                                                entry.MetricID,
                                                entry.IsEnabled,
                                                warningXml,
                                                criticalXml,
                                                dataXml,
                                                infoXml,
                                                entry.MetricInstanceType,
                                                entry.IsDefaultThreshold ? String.Empty : entry.MetricInstanceName,
                                                entry.IsThresholdEnabled,
                                                entry.IsBaselineEnabled,
                                                baselineWarningXml,
                                                baselineCriticalXml,
                                                baselineInfoXml);

                                            LOG.DebugFormat("Adding threshold instance {0} for metric {1}.",
                                                            entry.MetricInstanceName, entry.MetricID);
                                            addThresholdInstance.ExecuteNonQuery();
                                        }
                                    }

                                    // for everything left in the existingTargetEntries, delete it
                                    foreach (MetricThresholdEntry entry in existingTargetEntries.Values)
                                    {
                                        SqlHelper.AssignParameterValues(
                                            deleteThresholdInstance.Parameters,
                                            defaultConfiguration ? targetId : (object)DBNull.Value,
                                            defaultConfiguration ? (object)DBNull.Value : targetId,
                                            entry.MetricID,
                                            entry.MetricInstanceName,
                                            entry.MetricInstanceType);

                                        LOG.DebugFormat("Deleting threshold instance {0} for metric {1}.",
                                                        entry.MetricInstanceName, entry.MetricID);
                                        deleteThresholdInstance.ExecuteNonQuery();
                                    }
                                }
                                LOG.Verbose("Comitting template threshold changes");
                                xa.Commit();
                            }
                            catch (Exception e)
                            {
                                LOG.Error("Error applying template tp server {0}: ", targetId, e);
                                try
                                {
                                    xa.Rollback();
                                }
                                catch (Exception r)
                                {
                                    LOG.Error("Error applying template: (rollback for server {0} failed)", targetId, r);
                                }
                                throw;
                            }
                            finally
                            {
                                xa.Dispose();
                            }
                        }
                    }
                }
            }
        }

        public static SnoozeInfo SnoozeAlerts(string repositoryConnectionString, int instanceId, int? metricId, int snooozeForMinutes, string requestingUser)
        {
            using (LOG.InfoCall("SnoozeAlerts"))
            {
                if (repositoryConnectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandTimeout = SqlHelper.CommandTimeout;
                        command.CommandText = SnoozeAlertsProcedure;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SQLServerID", instanceId);
                        command.Parameters.AddWithValue("@Metric", metricId.HasValue ? (object)metricId.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@SnoozeMinutes", snooozeForMinutes);
                        SqlParameter startdateparm = command.Parameters.Add("@SnoozeStart", SqlDbType.DateTime);
                        startdateparm.Direction = ParameterDirection.Output;
                        SqlParameter enddateparm = command.Parameters.Add("@SnoozeEnd", SqlDbType.DateTime);
                        enddateparm.Direction = ParameterDirection.Output;
                        SqlParameter startuserparm = command.Parameters.Add("@SnoozeStartUser", SqlDbType.NVarChar, 255);
                        startuserparm.Direction = ParameterDirection.InputOutput;
                        startuserparm.Value = String.IsNullOrEmpty(requestingUser) ? (object)DBNull.Value : requestingUser;
                        SqlParameter enduserparm = command.Parameters.Add("@SnoozeEndUser", SqlDbType.NVarChar, 255);
                        enduserparm.Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();

                        SqlDateTime startdate = (SqlDateTime)startdateparm.SqlValue;
                        SqlDateTime enddate = (SqlDateTime)enddateparm.SqlValue;
                        SqlString startuser = (SqlString)startuserparm.SqlValue;
                        SqlString enduser = (SqlString)enduserparm.SqlValue;

                        return new SnoozeInfo(startdate.Value, enddate.Value, startuser.Value, enduser.Value);
                    }
                }
            }
        }

        public static SnoozeInfo UnsnoozeAlerts(string repositoryConnectionString, int instanceId, int[] metrics, string requestingUser)
        {
            using (LOG.InfoCall("SnoozeAlerts"))
            {
                if (repositoryConnectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }

                XmlDocument xmlDoc = null;
                if (metrics != null && metrics.Length > 0)
                {
                    xmlDoc = new XmlDocument();
                    XmlElement rootElement = xmlDoc.CreateElement("Metrics");
                    xmlDoc.AppendChild(rootElement);
                    XmlAddList(xmlDoc, metrics, "Metric", "MetricID");
                }

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandTimeout = SqlHelper.CommandTimeout;
                        command.CommandText = UnsnoozeAlertsProcedure;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SQLServerID", instanceId);
                        SqlParameter metricParm = command.Parameters.Add("@MetricXML", SqlDbType.NText);
                        SqlParameter startdateparm = command.Parameters.Add("@SnoozeStart", SqlDbType.DateTime);
                        startdateparm.Direction = ParameterDirection.Output;
                        SqlParameter enddateparm = command.Parameters.Add("@SnoozeEnd", SqlDbType.DateTime);
                        enddateparm.Direction = ParameterDirection.Output;
                        SqlParameter startuserparm = command.Parameters.Add("@SnoozeStartUser", SqlDbType.NVarChar, 255);
                        startuserparm.Direction = ParameterDirection.Output;
                        SqlParameter enduserparm = command.Parameters.Add("@SnoozeEndUser", SqlDbType.NVarChar, 255);
                        enduserparm.Direction = ParameterDirection.InputOutput;
                        enduserparm.Value = String.IsNullOrEmpty(requestingUser) ? (object)DBNull.Value : requestingUser;

                        metricParm.Value = xmlDoc == null ? (object)DBNull.Value : xmlDoc.OuterXml;

                        command.ExecuteNonQuery();

                        SqlDateTime startdate = (SqlDateTime)startdateparm.SqlValue;
                        SqlDateTime enddate = (SqlDateTime)enddateparm.SqlValue;
                        SqlString startuser = (SqlString)startuserparm.SqlValue;
                        SqlString enduser = (SqlString)enduserparm.SqlValue;

                        return new SnoozeInfo(startdate.Value, enddate.Value, startuser.Value, enduser.Value);
                    }
                }
            }
        }

        public static long GetHighestAlertID(SqlTransaction xa, string serverName)
        {
            using (LOG.DebugCall("GetHighestAlertID"))
            {
                if (xa == null)
                {
                    throw new ArgumentNullException("xa");
                }
                using (SqlCommand command = SqlHelper.CreateCommand(xa.Connection, GetMaxAlertIDProcedure))
                {
                    command.Transaction = xa;
                    SqlHelper.AssignParameterValues(command.Parameters,
                                                    serverName,
                                                    DBNull.Value);
                    command.ExecuteNonQuery();
                    SqlParameter returnValue = command.Parameters["@ReturnAlertID"];
                    object id = returnValue.Value;
                    if (id is long)
                        return (long)returnValue.Value;
                    else
                        return 0;
                }
            }
        }

        public static object[] ClearAlerts(SqlTransaction xa, string serverName, long highestAlertID, bool clearAll, int? metricID, long? alertID)
        {
            using (LOG.DebugCall("ClearAlerts"))
            {
                if (xa == null)
                {
                    throw new ArgumentNullException("xa");
                }
                using (SqlCommand command = SqlHelper.CreateCommand(xa.Connection, ClearAlertsProcedure))
                {
                    command.Transaction = xa;
                    SqlHelper.AssignParameterValues(command.Parameters,
                                                    serverName,
                                                    highestAlertID,
                                                    clearAll,
                                                    metricID == null ? (object)DBNull.Value : metricID.Value,
                                                    alertID == null ? (object)DBNull.Value : alertID.Value);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read() && reader.FieldCount >= 5)
                        {
                            return new object[]
                                {
                                    reader.GetInt32(0),
                                    reader.GetInt32(1),
                                    reader.IsDBNull(2) ? null : reader.GetString(2),
                                    reader.IsDBNull(3) ? null : reader.GetString(3),
                                    reader.IsDBNull(4) ? null : reader.GetString(4)
                                };
                        }
                    }
                }
                return null;
            }
        }

        internal static void ClearActiveAlerts(long alertID, bool allAlerts)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        //[START] - SQLdm 9.0 (Ankit Srivastava) - CWF Integration - new method for getting all the recent active alerts
        internal static IList<Alert> GetActiveAlertsForCWF(string repositoryConnectionString)
        {
            using (LOG.InfoCall("GetActiveAlerts"))
            {
                if (repositoryConnectionString == null)
                    throw new ArgumentNullException("repositoryConnectionString");

                CommonWebFramework webFramework = CommonWebFramework.GetInstance();
                List<Alert> alertsList = new List<Alert>();
                DateTime startTime = DateTime.UtcNow;
                //Dictionary<Metric, MetricInfo> metricInfoDictionary = RepositoryHelper.GetMetricInfo(ManagementServiceConfiguration.ConnectionString);



                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();
                    // SQLDM-29552: No DM alerts in Alerts View at Top Level of Idera Dashboard
                    // Adding the Parameters based on the SP changes
                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, "p_GetAlertsForWebConsole", null, null, null, null, null, null, null, null, 200, true, null, null, null))
                    {
                        while (reader.Read())
                        {
                            Alert alert = new Alert();

                            alert.ProductId = webFramework.ProductID;
                            int metric = reader.GetInt32(7);
                            alert.Metric = ((Metric)metric).ToString();

                            //if (metricInfoDictionary.ContainsKey((Metric)metric))
                            //{
                            //    alert.AlertCategory = metricInfoDictionary[(Metric)metric].Category;
                            //}
                            //else
                            //    alert.AlertCategory = String.Empty;
                            alert.AlertCategory = SharedMetricDefinitions.MetricDefinitions.GetMetricDefinition(metric).MetricCategory.ToString();

                            alert.Value = Convert.ToString(Convert.ToDouble(reader.GetValue(10)));
                            //SQLDM 10.1 (Barkha Khatri)
                            //SQLCORE-2462 fix
                            //Using String value -"Critical","Warning","Info","Ok" instead of 8,4,2,1 for CWF
                            alert.Severity = Enum.Parse(typeof(AlertSeverityForCWF), (reader.GetByte(8)).ToString()).ToString();
                            alert.StartTime = startTime;
                            alert.LastActiveTime = reader.GetDateTime(1);
                            alert.Instance = reader.GetString(3);
                            alert.Database = Convert.ToString(reader["DatabaseName"] ?? String.Empty);
                            alert.Table = Convert.ToString(reader["TableName"] ?? String.Empty);
                            alert.Summary = reader.GetString(12);

                            alertsList.Add(alert);
                        }
                    }
                }

                return alertsList;
            }
        }

        //[END] - SQLdm 9.0 (Ankit Srivastava) - CWF Integration - new method for getting all the recent active alerts

        //SQLdm10.1 (Srishti Purohit)
        //Update SCOMAlertEvent table according to alert rule
        //SCOM Alert Response Action
        public static void UpdateSCOMAlertEvent(string repositoryConnectionString, int metricID, bool isSCOMAlert, Guid ruleID)
        {
            try
            {
                LOG.Info("Updating SendAsAlert to {0} in SCOMAlertEvent for metrics.", isSCOMAlert);
                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = SqlHelper.CreateCommand(connection, UpdateSCOMAlertsEventStoreProcedure))
                    {
                        SqlHelper.AssignParameterValues(command.Parameters, metricID, ruleID, isSCOMAlert);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LOG.Error("Error while Updating SendAsAlert to {0} in SCOMAlertEvent for metrics.", isSCOMAlert, ex);
            }
        }

        #endregion

        #region Grooming

        public static void UpdateGrooming(string repositoryConnectionString, GroomingConfiguration configuration)
        {
            using (LOG.DebugCall("UpdateGrooming"))
            {
                int sqlStartTime = configuration.ScheduleSubDayType == GroomingConfiguration.SubDayType.Once
                                       ? configuration.GroomTime.Hours * 10000 + configuration.GroomTime.Minutes * 100
                                       : configuration.GroomTime.Hours;

                int sqlAggregationStartTime = configuration.AggregationSubDayType == GroomingConfiguration.SubDayType.Once
                                       ? configuration.AggregationTime.Hours * 10000 + configuration.AggregationTime.Minutes * 100
                                       : configuration.AggregationTime.Hours;


                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();
                    SqlHelper.ExecuteNonQuery(connection,
                                              "p_UpdateGrooming",
                                              configuration.AlertsDays,
                                              configuration.MetricsDays,
                                              configuration.TasksDays,
                                              configuration.ActivityDays,
                                              configuration.AuditDays,
                                              configuration.PADataDays,
                                              configuration.UpdateScheduleAllowed ? (object)sqlStartTime : null,
                                              configuration.UpdateScheduleAllowed ? (object)((int)configuration.ScheduleSubDayType) : null,
                                              configuration.QueriesDays,
                                              configuration.UpdateAggregationScheduleAllowed ? (object)sqlAggregationStartTime : null,
                                              configuration.UpdateAggregationScheduleAllowed ? (object)((int)configuration.AggregationSubDayType) : null,
                                              configuration.GroomForecastingDays,
                                              configuration.FADataDays);
                }
            }
        }

        public static void CreateGroomJob(string repositoryConnectionString)
        {
            using (LOG.DebugCall("CreateGroomJob"))
            {
                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();
                    SqlHelper.ExecuteNonQuery(connection, "p_CreateGroomJob", false);
                    SqlHelper.ExecuteNonQuery(connection, "p_CreateAggregationJob", false);
                }
            }
        }

        public static void GroomNow(string repositoryConnectionString)
        {
            using (LOG.DebugCall("GroomNow"))
            {
                // SQLDM-19304, Tolga K, attempt should fail when repo is in readonly mode
                if (RepositoryHelper.IsInAvailabilityGroup(repositoryConnectionString))
                {
                    LOG.DebugCall("Ignoring grooming in this database because it is readonly");
                    return;
                }

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();
                    SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "p_StartGroomJob");
                }
            }
        }

        /// <summary>
        /// SQLdm 10.2, Tolga K - Gets whether repository database is in an availability gorup
        /// </summary>
        /// <returns></returns>
        public static bool IsInAvailabilityGroup(string repositoryConnectionString)
        {
            using (LOG.DebugCall("IsInAvailabilityGroup"))
            {
                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    try
                    {
                        connection.Open();
                        ServerVersion ver = new ServerVersion(connection.ServerVersion);
                        if (ver.Major < 11)
                        {
                            return false;
                        }

                        using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, "p_GetAvailabilityItemsWithRepository"))
                        {
                            return reader.Read();
                        }
                    }
                    catch (Exception ex)
                    {
                        LOG.ErrorFormat("IsInAvailabilityGroup returned error: {0}", ex.Message);
                        return false;
                    }
                }
            }
        }

        public static void AggregateNow(string repositoryConnectionString)
        {
            using (LOG.DebugCall("AggregateNow"))
            {
                // SQLDM-19304, Tolga K, attempt should fail when repo is in readonly mode
                if (RepositoryHelper.IsInAvailabilityGroup(repositoryConnectionString))
                {
                    LOG.DebugCall("Ignoring aggregation in this database because it is readonly");
                    return;
                }

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();
                    SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "p_StartAggregationJob");
                }
            }
        }

        public static GroomingConfiguration GetGrooming(string repositoryConnectionString)
        {
            using (LOG.DebugCall("GetGrooming"))
            {
                GroomingConfiguration configuration = new GroomingConfiguration();
                SqlParameter[] arParms = new SqlParameter[26];

                arParms[0] = new SqlParameter("@AlertsOut", SqlDbType.Int);
                arParms[0].Direction = ParameterDirection.Output;

                arParms[1] = new SqlParameter("@MetricsOut", SqlDbType.Int);
                arParms[1].Direction = ParameterDirection.Output;

                arParms[2] = new SqlParameter("@TasksOut", SqlDbType.Int);
                arParms[2].Direction = ParameterDirection.Output;

                arParms[3] = new SqlParameter("@ActivityOut", SqlDbType.Int);
                arParms[3].Direction = ParameterDirection.Output;

                arParms[4] = new SqlParameter("@StartTime", SqlDbType.Int);
                arParms[4].Direction = ParameterDirection.Output;

                arParms[5] = new SqlParameter("@SubDayType", SqlDbType.Int);
                arParms[5].Direction = ParameterDirection.Output;

                arParms[6] = new SqlParameter("@AllowScheduleChange", SqlDbType.Bit);
                arParms[6].Direction = ParameterDirection.Output;

                arParms[7] = new SqlParameter("@AgentIsRunning", SqlDbType.Bit);
                arParms[7].Direction = ParameterDirection.Output;

                arParms[8] = new SqlParameter("@JobIsRunning", SqlDbType.Bit);
                arParms[8].Direction = ParameterDirection.Output;

                arParms[9] = new SqlParameter("@RepositoryTime", SqlDbType.DateTime);
                arParms[9].Direction = ParameterDirection.Output;

                arParms[10] = new SqlParameter("@GroomingTimeLimit", SqlDbType.Int);
                arParms[10].Direction = ParameterDirection.Output;

                arParms[11] = new SqlParameter("@LastRunDate", SqlDbType.Int);
                arParms[11].Direction = ParameterDirection.Output;

                arParms[12] = new SqlParameter("@LastRunTime", SqlDbType.Int);
                arParms[12].Direction = ParameterDirection.Output;

                arParms[13] = new SqlParameter("@LastRunOutcome", SqlDbType.Int);
                arParms[13].Direction = ParameterDirection.Output;

                arParms[14] = new SqlParameter("@QueriesOut", SqlDbType.Int);
                arParms[14].Direction = ParameterDirection.Output;

                arParms[15] = new SqlParameter("@AggregationStartTime", SqlDbType.Int);
                arParms[15].Direction = ParameterDirection.Output;

                arParms[16] = new SqlParameter("@AggregationSubDayType", SqlDbType.Int);
                arParms[16].Direction = ParameterDirection.Output;

                arParms[17] = new SqlParameter("@AggregationAllowScheduleChange", SqlDbType.Bit);
                arParms[17].Direction = ParameterDirection.Output;

                arParms[18] = new SqlParameter("@AggregationJobIsRunning", SqlDbType.Bit);
                arParms[18].Direction = ParameterDirection.Output;

                arParms[19] = new SqlParameter("@AggregationLastRunDate", SqlDbType.Int);
                arParms[19].Direction = ParameterDirection.Output;

                arParms[20] = new SqlParameter("@AggregationLastRunTime", SqlDbType.Int);
                arParms[20].Direction = ParameterDirection.Output;

                arParms[21] = new SqlParameter("@AggregationLastRunOutcome", SqlDbType.Int);
                arParms[21].Direction = ParameterDirection.Output;

                arParms[22] = new SqlParameter("@AuditOut", SqlDbType.Int);
                arParms[22].Direction = ParameterDirection.Output;

                arParms[23] = new SqlParameter("@PADaysOut", SqlDbType.Int);
                arParms[23].Direction = ParameterDirection.Output;

                arParms[24] = new SqlParameter("@ForecastingOut", SqlDbType.Int);
                arParms[24].Direction = ParameterDirection.Output;

                arParms[25] = new SqlParameter("@FADaysOut", SqlDbType.Int);
                arParms[25].Direction = ParameterDirection.Output;

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "p_GetGrooming";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = SqlHelper.CommandTimeout;
                        command.Parameters.AddRange(arParms);
                        command.ExecuteNonQuery();

                        if (arParms[0].Value is int)
                            configuration.AlertsDays = (int)arParms[0].Value;
                        else
                            LOG.Warn("@AlertsOut is not an int!");

                        if (arParms[1].Value is int)
                            configuration.MetricsDays = (int)arParms[1].Value;
                        else
                            LOG.Warn("@MetricsOut is not an int!");

                        if (arParms[2].Value is int)
                            configuration.TasksDays = (int)arParms[2].Value;
                        else
                            LOG.Warn("@TasksOut is not an int!");

                        if (arParms[3].Value is int)
                            configuration.ActivityDays = (int)arParms[3].Value;
                        else
                            LOG.Warn("@ActivityOut is not an int!");

                        if (arParms[4].Value is int)
                        {
                            int sqlTime = (int)arParms[4].Value;
                            configuration.GroomTime = ConvertSqlIntTime(sqlTime);
                        }
                        else
                            LOG.Warn("@StartTime is not an int!");

                        if (arParms[5].Value is int)
                        {
                            int subdaytype = (int)arParms[5].Value;
                            configuration.ScheduleSubDayType =
                                (GroomingConfiguration.SubDayType)
                                Enum.ToObject(typeof(GroomingConfiguration.SubDayType), subdaytype);
                        }
                        else
                            LOG.Warn("@SubDayType is not an int!");

                        if (arParms[6].Value is bool)
                        {
                            configuration.UpdateScheduleAllowed = (bool)arParms[6].Value;
                        }
                        else
                            LOG.Warn("@AllowScheduleChange is not a bool!");

                        if (arParms[7].Value is bool)
                        {
                            configuration.AgentIsRunning = (bool)arParms[7].Value;
                        }
                        else
                            LOG.Warn("@AgentIsRunning is not a bool!");

                        if (arParms[8].Value is bool)
                        {
                            configuration.JobIsRunning = (bool)arParms[8].Value;
                            if (configuration.JobIsRunning) configuration.LastOutcome = "Running";
                        }
                        else
                            LOG.Warn("@JobIsRunning is not a bool!");

                        if (arParms[9].Value is DateTime)
                        {
                            configuration.RepositoryTime = (DateTime)arParms[9].Value;
                        }
                        else
                            LOG.Warn("@RepositoryTime is not a DateTime!");


                        if (arParms[11].Value is int)
                        {
                            configuration.LastRun = ConvertSqlIntDate((int)arParms[11].Value);
                        }

                        if (arParms[12].Value is int)
                        {
                            configuration.LastRun += ConvertSqlIntTime((int)arParms[12].Value);
                        }

                        if (arParms[13].Value is int)
                        {
                            switch ((int)arParms[13].Value)
                            {
                                case 0:
                                    configuration.LastOutcome = "Failed";
                                    break;
                                case 1:
                                    configuration.LastOutcome = "Succeeded";
                                    break;
                                case 3:
                                    configuration.LastOutcome = "Canceled";
                                    break;
                                default:
                                    configuration.LastOutcome = "Unknown";
                                    break;
                            }
                        }

                        if (arParms[14].Value is int)
                            configuration.QueriesDays = (int)arParms[14].Value;

                        if (arParms[15].Value is int)
                        {
                            int sqlTime = (int)arParms[15].Value;
                            configuration.AggregationTime = ConvertSqlIntTime(sqlTime);
                        }

                        if (arParms[16].Value is int)
                        {
                            int subdaytype = (int)arParms[16].Value;
                            configuration.AggregationSubDayType =
                                (GroomingConfiguration.SubDayType)
                                Enum.ToObject(typeof(GroomingConfiguration.SubDayType), subdaytype);
                        }

                        if (arParms[17].Value is bool)
                        {
                            configuration.UpdateAggregationScheduleAllowed = (bool)arParms[17].Value;
                        }

                        if (arParms[18].Value is bool)
                        {
                            configuration.AggregationJobIsRunning = (bool)arParms[18].Value;
                            if (configuration.AggregationJobIsRunning) configuration.AggregationLastOutcome = "Running";
                        }

                        if (arParms[19].Value is int)
                        {
                            configuration.AggregationLastRun = ConvertSqlIntDate((int)arParms[19].Value);
                        }

                        if (arParms[20].Value is int)
                        {
                            configuration.AggregationLastRun += ConvertSqlIntTime((int)arParms[20].Value);
                        }

                        if (arParms[21].Value is int)
                        {
                            switch ((int)arParms[21].Value)
                            {
                                case 0:
                                    configuration.AggregationLastOutcome = "Failed";
                                    break;
                                case 1:
                                    configuration.AggregationLastOutcome = "Succeeded";
                                    break;
                                case 3:
                                    configuration.AggregationLastOutcome = "Canceled";
                                    break;
                                default:
                                    configuration.AggregationLastOutcome = "Unknown";
                                    break;
                            }
                        }

                        if (arParms[22].Value is int)
                            configuration.AuditDays = (int)arParms[22].Value;
                        else
                            LOG.Warn("@AuditOut is not an int!");
                        //10.0 SQLdm srishti purohit
                        //Prescriptive analysis old data grooming implementation
                        LOG.Info("Getting Prescriptive Analysis grooming days value.");
                        if (arParms[23].Value is int)
                            configuration.PADataDays = (int)arParms[23].Value;
                        else
                            LOG.Warn("@PADaysOut is not an int!");


                        if (arParms[24].Value is int)
                            configuration.GroomForecastingDays = (int)arParms[24].Value;
                        else
                            LOG.Warn("@GroomForecastingDays is not an int!");



                        if (arParms[25].Value is int)
                            configuration.FADataDays = (int)arParms[25].Value;
                        else
                            LOG.Warn("@PADaysOut is not an int!");


                        LOG.Debug("Returning GroomingConfig = ", configuration);
                        return configuration;
                    }
                }
            }
        }

        public static void SetQueryAggregationFlag(string repositoryConnectionString, long signatureID, bool doNotAggregate)
        {
            using (LOG.DebugCall("SetQueryAggregationFlag"))
            {
                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "p_SetAggregationFlag";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = SqlHelper.CommandTimeout;

                        SqlParameter[] arParms = new SqlParameter[2];
                        arParms[0] = new SqlParameter("@SQLSignatureID", SqlDbType.BigInt);
                        arParms[0].Value = signatureID;
                        arParms[1] = new SqlParameter("@DoNotAggregate", SqlDbType.Bit);
                        arParms[1].Value = doNotAggregate;
                        command.Parameters.AddRange(arParms);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        //Start - SQLdm 9.0 -(Ankit Srivastava) - added new method to get the timed out instances
        /// <summary>
        /// Gets the timed out state of grooming
        /// </summary>
        /// <param name="repositoryConnectionString"></param>
        /// <param name="sqlServerId"></param>
        /// <returns></returns>
        public static KeyValuePair<bool, string> GetGroomingTimedOutState(string repositoryConnectionString, int sqlServerId)
        {
            using (LOG.DebugCall("GetGroomingStatus"))
            {
                KeyValuePair<bool, string> result = new KeyValuePair<bool, string>(false, String.Empty);
                try
                {
                    StringBuilder instancesToBeGroomed = new StringBuilder();
                    SqlParameter param = new SqlParameter();
                    param.Value = sqlServerId;
                    param.SqlDbType = SqlDbType.Int;

                    using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                    {
                        connection.Open();

                        //SQlDM-28022 - Handling connection object to avoid leakage and Ensure the reader is closed
                        using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetGroomingStatusInfo, param))
                        {
                            if (reader.Read() && reader.GetBoolean(0))
                            {
                                if (reader.NextResult())
                                {
                                    if (reader.Read())
                                    {
                                        instancesToBeGroomed.Append(reader.GetString(0));
                                    }
                                    while (reader.Read())
                                    {
                                        instancesToBeGroomed.Append(", ").Append(reader.GetString(0));
                                    }
                                }
                                result = new KeyValuePair<bool, string>(true, instancesToBeGroomed.ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LOG.Error(String.Format("An error occured while GetGroomingStatus : {0}", ex.Message));
                }
                return result;

            }
        }
        //End - SQLdm 9.0 -(Ankit Srivastava) - added new method to get the timed out instances

        #endregion

        #region Query Text


        public static Pair<long, string> GetQueryText(
            string repositoryConnectionString,
            string SQLStatementHash,
            long? SQLStatementID,
            string SQLSignatureHash,
            long? SQLSignatureID)
        {
            try
            {
                if (repositoryConnectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }

                Pair<long, string> returnData = new Pair<long, string>();


                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command =
                        SqlHelper.CreateCommand(connection, GetSQLText))
                    {
                        SqlHelper.AssignParameterValues(command.Parameters,
                                                        SQLStatementHash,
                                                        SQLStatementID,
                                                        SQLSignatureHash,
                                                        SQLSignatureID);

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                if (dataReader.FieldCount == 1)
                                    return returnData;

                                if (!dataReader.IsDBNull(0))
                                {
                                    returnData.First = dataReader.GetInt32(0);
                                }

                                if (!dataReader.IsDBNull(1))
                                {
                                    returnData.Second = dataReader.GetString(1);
                                }
                            }
                        }
                    }
                }

                return returnData;
            }
            catch (Exception ex)
            {
                LOG.Error("Exception while looking up SQLtext.", ex);
                throw;
            }
        }

        #endregion

        #region Conversions

        // Converts an integer that represents a date in the form yymmdd to a DateTime
        private static DateTime ConvertSqlIntDate(int sqlDate)
        {
            if (sqlDate == 0)
            {
                return DateTime.MinValue;
            }
            else
            {
                int day = sqlDate % 100;
                int month = (sqlDate / 100) % 100;
                int year = sqlDate / 10000;
                return new DateTime(year, month, day);
            }
        }

        // Converts an integer that represents a time in the form hhmmss to a TimeSpan
        private static TimeSpan ConvertSqlIntTime(int sqlTime)
        {
            if (sqlTime == 0)
            {
                return TimeSpan.Zero;
            }
            else
            {
                int sec = sqlTime % 100;
                int min = (sqlTime / 100) % 100;
                int hour = sqlTime / 10000;
                return new TimeSpan(hour, min, sec);
            }
        }

        #endregion

        #region Custom Counters

        internal static int AddCustomCounter(string repositoryConnectionString, MetricDefinition metricDefinition, MetricDescription metricDescription, CustomCounterDefinition counterDefinition, bool alertOnCollectionFailure)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            int result = -1;
            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                connection.Open();
                SqlTransaction xa = null;
                try
                {
                    xa = connection.BeginTransaction();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, AddCounterProcedure))
                    {
                        command.Transaction = xa;
                        SqlHelper.AssignParameterValues(
                            command.Parameters,
                            metricDescription.Name,
                            metricDescription.Category,
                            metricDescription.Description,
                            0,
                            (int)metricDefinition.Options,
                            counterDefinition.Scale,
                            metricDefinition.MinValue,
                            metricDefinition.MaxValue,
                            metricDefinition.DefaultInfoThresholdValue,
                            metricDefinition.DefaultWarningThresholdValue,
                            metricDefinition.DefaultCriticalThresholdValue,
                            metricDefinition.ProcessNotifications,
                            3,
                            metricDefinition.DefaultMessageID,
                            metricDefinition.AlertEnabledByDefault,
                            (int)metricDefinition.ComparisonType,
                            metricDefinition.ValueType.FullName,
                            metricDefinition.Rank,
                            (int)counterDefinition.MetricType,
                            (int)counterDefinition.CalculationType,
                            counterDefinition.IsEnabled,
                            counterDefinition.ObjectName,
                            counterDefinition.CounterName,
                            counterDefinition.InstanceName,
                            counterDefinition.SqlStatement,
                            counterDefinition.ServerType,
                            counterDefinition.ProfileId,
                            alertOnCollectionFailure,
                            null);

                        command.ExecuteNonQuery();
                        object id = command.Parameters["@ReturnMetricID"].Value;
                        if (id is int)
                            result = (int)id;

                        xa.Commit();
                    }
                }
                catch (Exception)
                {
                    if (xa != null)
                        try { xa.Rollback(); }
                        catch (Exception) { /* */ }
                    throw;
                }
                finally
                {
                    if (xa != null)
                        try { xa.Dispose(); }
                        catch (Exception) { /* */ }
                }
            }
            return result;
        }

        internal static void UpdateCustomCounter(string repositoryConnectionString, MetricDefinition metricDefinition, MetricDescription metricDescription, CustomCounterDefinition counterDefinition)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                connection.Open();
                SqlTransaction xa = null;
                try
                {
                    xa = connection.BeginTransaction();

                    SqlHelper.ExecuteNonQuery(xa,
                        UpdateCounterProcedure,
                        metricDefinition.MetricID,
                        metricDescription.Name,
                        metricDescription.Category,
                        metricDescription.Description,
                        0,
                        (int)metricDefinition.Options,
                        counterDefinition.Scale,
                        metricDefinition.MinValue,
                        metricDefinition.MaxValue,
                        metricDefinition.DefaultInfoThresholdValue,
                        metricDefinition.DefaultWarningThresholdValue,
                        metricDefinition.DefaultCriticalThresholdValue,
                        metricDefinition.ProcessNotifications,
                        3,
                        metricDefinition.DefaultMessageID,
                        metricDefinition.AlertEnabledByDefault,
                        (int)metricDefinition.ComparisonType,
                        metricDefinition.ValueType.FullName,
                        metricDefinition.Rank,
                        (int)counterDefinition.MetricType,
                        (int)counterDefinition.CalculationType,
                        counterDefinition.IsEnabled,
                        counterDefinition.ObjectName,
                        counterDefinition.CounterName,
                        counterDefinition.InstanceName,
                        counterDefinition.SqlStatement,
                        counterDefinition.ServerType,
                        counterDefinition.ProfileId);

                    xa.Commit();
                }
                catch (Exception)
                {
                    if (xa != null)
                        try { xa.Rollback(); }
                        catch (Exception) { /* */ }
                    throw;
                }
                finally
                {
                    if (xa != null)
                        try { xa.Dispose(); }
                        catch (Exception) { /* */ }
                }
            }
        }

        internal static void AddCounterToServer(string repositoryConnectionString, int metricID,
            IEnumerable<int> tags, IEnumerable<int> monitoredSqlServers, bool synchronizeList)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            XmlDocument tagsXmlDoc = new XmlDocument();
            XmlElement rootElement = tagsXmlDoc.CreateElement("Tags");
            tagsXmlDoc.AppendChild(rootElement);
            XmlAddList(tagsXmlDoc, tags, "Tag", "TagId");

            XmlDocument serversXmlDoc = new XmlDocument();
            rootElement = serversXmlDoc.CreateElement("MetricAssignment");
            serversXmlDoc.AppendChild(rootElement);
            XmlAddList(serversXmlDoc, monitoredSqlServers, "SQLServer", "SQLServerID");

            SqlHelper.ExecuteNonQuery(repositoryConnectionString, AssignCounterToServersProcedure, metricID,
                                      tagsXmlDoc.InnerXml, serversXmlDoc.InnerXml, synchronizeList);
        }

        // For each database, insert an XML element under the document's root node.
        private static void XmlAddList(XmlDocument xmlDoc, IEnumerable list, string elementName, string AttrName)
        {
            foreach (object o in list)
            {
                XmlElement e = xmlDoc.CreateElement(elementName);
                e.SetAttribute(AttrName, o.ToString());
                xmlDoc.FirstChild.AppendChild(e);
            }
        }

        internal static void AddCountersToServer(string repositoryConnectionString, int monitoredSqlServer, int[] metrics, bool synchronizeList)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            XmlDocument xmlDoc = new XmlDocument();
            XmlElement rootElement = xmlDoc.CreateElement("MetricAssignment");
            xmlDoc.AppendChild(rootElement);
            XmlAddList(xmlDoc, metrics, "Metric", "MetricID");

            SqlHelper.ExecuteNonQuery(repositoryConnectionString, AssignCounterToServersProcedure, monitoredSqlServer,
                                      xmlDoc.OuterXml, synchronizeList);
        }

        internal static void DeleteCustomCounter(string repositoryConnectionString, int metricID)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            SqlHelper.ExecuteNonQuery(repositoryConnectionString, DeleteCounterProcedure, metricID);
        }

        internal static void UpdateCustomCounterStatus(string repositoryConnectionString, int metricID, bool enabled)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            SqlHelper.ExecuteNonQuery(repositoryConnectionString, UpdateCounterStatusProcedure, metricID, enabled);
        }

        internal static List<int> GetMonitoredServersUsingCounter(string repositoryConnectionString, int metricID)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            List<int> result = new List<int>();

            using (
                SqlDataReader reader =
                    SqlHelper.ExecuteReader(repositoryConnectionString,
                                            GetServersUsingCustomCounterProcedure,
                                            metricID))
            {
                while (reader.Read())
                {
                    result.Add(reader.GetInt32(0));
                }
            }
            return result;
        }

        #endregion

        #region Mirroring
        public static void DeleteMirroringPreferredConfig(string repositoryConnectionString, Guid guid)
        {
            using (LOG.DebugCall())
            {
                if (repositoryConnectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    SqlHelper.ExecuteNonQuery(connection, MirroringDeletePreferredConfig, guid);
                    LOG.Debug("ExecuteNonQuery(", MirroringDeletePreferredConfig, ", ", guid + ")");
                }
            }
        }

        /// <summary>
        /// Deletes the specified entry for the specified server and guid from mirroring participants
        /// </summary>
        /// <param name="repositoryConnectionString"></param>
        /// <param name="ServerID"></param>
        /// <param name="guid"></param>
        public static void DeleteMirroringSessionFromServer(string repositoryConnectionString, int ServerID, Guid guid)
        {
            using (LOG.DebugCall())
            {
                if (repositoryConnectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    SqlHelper.ExecuteNonQuery(connection, MirroringDeleteSessionFromServer, ServerID, guid);
                    LOG.Debug("ExecuteNonQuery(", MirroringDeleteSessionFromServer, ", ", ServerID + ")");
                }
            }
        }

        public static Dictionary<Guid, int> GetMirroringParticipantsForServer(string repositoryConnectionString, int ServerID)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }
            Dictionary<Guid, int> participants = new Dictionary<Guid, int>();

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                using (SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(connection, MirroringGetParticipantsForServer, ServerID))
                {
                    while (dataReader.Read())
                    {
                        //if any of the value type are null then dont save the session
                        if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(1))
                        {
                            int serverID = 0;
                            Guid mirroringGuid = Guid.Empty;

                            if (!dataReader.IsDBNull(0)) serverID = int.Parse(dataReader["ServerID"].ToString());
                            if (!dataReader.IsDBNull(1)) mirroringGuid = new Guid(dataReader["mirroring_guid"].ToString());

                            //only add it if it is not already in the dictionary
                            if (!participants.ContainsKey(mirroringGuid))
                                participants.Add(mirroringGuid, serverID);
                        }
                    }
                }
            }
            return participants;
        }

        /// <summary>
        /// Get the preferred operartional status of ALL mirroring sessions from our repository
        /// </summary>
        /// <param name="repositoryConnectionString"></param>
        /// <returns></returns>
        public static Dictionary<Guid, MirroringSession> GetMirroringPreferredConfig(string repositoryConnectionString)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            Dictionary<Guid, MirroringSession> preferredConfigs = new Dictionary<Guid, MirroringSession>();

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                using (SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(connection, MirroringGetPreferredConfigProcedure))
                {
                    while (dataReader.Read())
                    {
                        //if any of the value type are null then dont save the session
                        if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(1))
                        {
                            Guid mirroringGuid = Guid.Empty;
                            int PrincipalID = 0;
                            int MirrorID = 0;
                            MirroringSession.MirroringPreferredConfig Normal = MirroringSession.MirroringPreferredConfig.Delete;

                            string MirroredDatabase = null;
                            string WitnessName = null;
                            if (!dataReader.IsDBNull(0)) mirroringGuid = new Guid(dataReader["MirroringGuid"].ToString());
                            if (!dataReader.IsDBNull(1)) MirrorID = int.Parse(dataReader["MirrorInstanceID"].ToString());
                            if (!dataReader.IsDBNull(2)) PrincipalID = int.Parse(dataReader["PrincipalInstanceID"].ToString());
                            if (!dataReader.IsDBNull(3)) Normal = ((bool)dataReader["NormalConfiguration"]) ? MirroringSession.MirroringPreferredConfig.Normal : MirroringSession.MirroringPreferredConfig.FailedOver;
                            if (!dataReader.IsDBNull(4)) MirroredDatabase = dataReader["DatabaseName"].ToString();
                            if (!dataReader.IsDBNull(5)) WitnessName = dataReader["WitnessAddress"].ToString();

                            MirroringSession session = new MirroringSession(mirroringGuid, PrincipalID, MirrorID, MirroredDatabase, WitnessName, Normal);
                            preferredConfigs.Add(session.MirroringGuid, session);
                        }
                    }
                }
            }

            return preferredConfigs;
        }

        /// <summary>
        /// Set the preferred operational status of a mirroring session
        /// </summary>
        /// <param name="repositoryConnectionString"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        internal static int SetMirroringPreferredConfig(string repositoryConnectionString, MirroringSession session)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            int result = -1;
            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                connection.Open();
                SqlTransaction xa = null;
                try
                {
                    xa = connection.BeginTransaction();

                    using (SqlCommand command = SqlHelper.CreateCommand(connection, MirroringSetPreferredConfigProcedure))
                    {
                        command.Transaction = xa;
                        SqlHelper.AssignParameterValues(
                            command.Parameters,
                            session.MirroringGuid,
                            session.MirrorID,
                            session.PrincipalID,
                            session.WitnessName,
                            session.Database,
                            (int)session.PreferredConfig);

                        command.ExecuteNonQuery();

                        xa.Commit();
                    }
                }
                catch (Exception)
                {
                    if (xa != null)
                        try { xa.Rollback(); }
                        catch (Exception) { /* */ }
                    throw;
                }
                finally
                {
                    if (xa != null)
                        try { xa.Dispose(); }
                        catch (Exception) { /* */ }
                }
            }
            return result;
        }
        #endregion

        #region Custom Reports

        /// <summary>
        /// returns any existing reports with the same case insensitive name
        /// </summary>
        /// <param name="repositoryConnectionString"></param>
        /// <param name="reportName"></param>
        /// <returns></returns>
        public static string GetExistingCustomReportName(string repositoryConnectionString, string reportName)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (LOG.DebugCall("GetCustomReportName"))
            {
                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();
                    SqlTransaction xa = null;

                    try
                    {
                        xa = connection.BeginTransaction();

                        using (SqlCommand command = SqlHelper.CreateCommand(connection, GetCustomReportName))
                        {
                            command.Transaction = xa;
                            SqlHelper.AssignParameterValues(command.Parameters,
                                                            reportName);
                            command.ExecuteNonQuery();

                            string returnValue = command.Parameters["@reportName"].Value.ToString();

                            return returnValue;
                        }
                    }
                    catch (Exception)
                    {
                        if (xa != null)
                            try { xa.Rollback(); }
                            catch (Exception) { /* */ }
                        throw;
                    }
                    finally
                    {
                        if (xa != null)
                            try { xa.Dispose(); }
                            catch (Exception) { /* */ }
                    }
                }
            }
        }

        internal static void DeleteCustomReport(string repositoryConnectionString, int id)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                connection.Open();
                SqlTransaction xa = null;

                try
                {
                    xa = connection.BeginTransaction();

                    using (SqlCommand command = SqlHelper.CreateCommand(connection, DeleteCustomReportFromRepository))
                    {
                        command.Transaction = xa;

                        SqlHelper.AssignParameterValues(
                            command.Parameters, id);

                        command.ExecuteNonQuery();

                        xa.Commit();
                    }
                }
                catch (Exception)
                {
                    if (xa != null)
                        try { xa.Rollback(); }
                        catch (Exception) { /* */ }
                    throw;
                }
                finally
                {
                    if (xa != null)
                        try { xa.Dispose(); }
                        catch (Exception) { /* */ }
                }
            }
        }
        internal static void DeleteCustomReportCounters(string repositoryConnectionString, string reportName)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                connection.Open();
                SqlTransaction xa = null;

                try
                {
                    xa = connection.BeginTransaction();

                    using (SqlCommand command = SqlHelper.CreateCommand(connection, DeleteCountersFromCustomReport))
                    {
                        command.Transaction = xa;

                        SqlHelper.AssignParameterValues(
                            command.Parameters, reportName);

                        command.ExecuteNonQuery();

                        xa.Commit();
                    }
                }
                catch (Exception)
                {
                    if (xa != null)
                        try { xa.Rollback(); }
                        catch (Exception) { /* */ }
                    throw;
                }
                finally
                {
                    if (xa != null)
                        try { xa.Dispose(); }
                        catch (Exception) { /* */ }
                }
            }
        }

        internal static bool InsertCounterToGraph(string repositoryConnectionString, string ReportName, int GraphNumber, string CounterName, string ShortDescription, int Aggregation, int Source)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                connection.Open();
                SqlTransaction xa = null;

                try
                {
                    xa = connection.BeginTransaction();

                    using (SqlCommand command = SqlHelper.CreateCommand(connection, InsertCustomReportCounters))
                    {
                        command.Transaction = xa;
                        SqlHelper.AssignParameterValues(
                                                            command.Parameters,
                                                            ReportName,
                                                            GraphNumber,
                                                            ShortDescription,
                                                            CounterName,
                                                            Aggregation,
                                                            Source);

                        command.ExecuteNonQuery();

                        xa.Commit();
                    }
                }
                catch (Exception)
                {
                    if (xa != null)
                        try { xa.Rollback(); }
                        catch (Exception) { /* */ }
                    throw;
                }
                finally
                {
                    if (xa != null)
                        try { xa.Dispose(); }
                        catch (Exception) { /* */ }
                }
            }
            return true;
        }
        /// <summary>
        /// Update a custom report BUFFERSIZE characters at a time
        /// This awful technique is required because on sql 2000 you cannot have an nvarchar field of more than 4000 characters
        /// We must therefore add the report xml to the repository in chunks of up to 4000 characters
        /// </summary>
        /// <param name="repositoryConnectionString"></param>
        /// <param name="operation">{delete = 1, update text, append text, rename, create new}</param>
        /// <param name="reportID"></param>
        /// <param name="reportName"></param>
        /// <param name="reportShortDescription"></param>
        /// <param name="reportText"></param>
        /// <returns></returns>
        internal static int UpdateCustomReports(string repositoryConnectionString, CustomReport.Operation operation, int? reportID, string reportName, string reportShortDescription, string reportText, Boolean showTopServers)
        {
            const int BUFFERSIZE = 1000;
            int currentPosition = 0;
            int remainingChars = 0;
            string reportWriteBuffer = "";

            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            int result = -1;

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                connection.Open();
                SqlTransaction xa = null;

                try
                {
                    xa = connection.BeginTransaction();

                    using (SqlCommand command = SqlHelper.CreateCommand(connection, UpdateOrCreateCustomReport))
                    {
                        command.Transaction = xa;

                        switch (operation)
                        {
                            case CustomReport.Operation.Delete://delete the report
                                SqlHelper.AssignParameterValues(
                                    command.Parameters,
                                    reportID,
                                    operation,
                                    reportName,
                                    null,
                                    null,
                                    showTopServers);

                                command.ExecuteNonQuery();
                                result = 1;
                                break;
                            case CustomReport.Operation.Update://update
                                //replace the existing report text with reportText
                                currentPosition = 0;
                                remainingChars = reportText.Length - currentPosition;

                                reportWriteBuffer = remainingChars > BUFFERSIZE ? reportText.Substring(0, BUFFERSIZE) : reportText;

                                //update the text with the first BUFFERSIZE chars
                                SqlHelper.AssignParameterValues(
                                    command.Parameters,
                                    reportID,
                                    2,//update
                                    reportName,
                                    null,
                                    reportWriteBuffer,
                                    showTopServers);



                                command.ExecuteNonQuery();

                                //result = (int)(command.Parameters["@reportID"].Value);

                                currentPosition += reportWriteBuffer.Length;
                                remainingChars = reportText.Length - currentPosition;

                                //then append the rest
                                while (remainingChars > 0)
                                {
                                    //write buffer gets the next BUFFERSIZE or the rest of the string, whichever is shorter
                                    reportWriteBuffer = remainingChars > BUFFERSIZE ? reportText.Substring(currentPosition, BUFFERSIZE) : reportText.Substring(currentPosition);

                                    SqlHelper.AssignParameterValues(
                                        command.Parameters,
                                        reportID,
                                        3,//append
                                        reportName,
                                        null,
                                        reportWriteBuffer,
                                        showTopServers);

                                    command.ExecuteNonQuery();

                                    currentPosition += reportWriteBuffer.Length;
                                    remainingChars = reportText.Length - currentPosition;
                                }
                                result = (int)(command.Parameters["@reportID"].Value);
                                break;
                            case CustomReport.Operation.Append://append
                                //append reportText to the existing report text
                                currentPosition = 0;
                                remainingChars = reportText.Length - currentPosition;

                                //then append the rest
                                while (remainingChars > 0)
                                {
                                    //write buffer gets the next BUFFERSIZE or the rest of the string, whichever is shorter
                                    reportWriteBuffer = remainingChars > BUFFERSIZE ? reportText.Substring(currentPosition, BUFFERSIZE) : reportText.Substring(currentPosition);

                                    SqlHelper.AssignParameterValues(
                                        command.Parameters,
                                        reportID,
                                        3,//append
                                        reportName,
                                        null,
                                        reportWriteBuffer,
                                        showTopServers);

                                    command.ExecuteNonQuery();

                                    currentPosition += reportWriteBuffer.Length;
                                    remainingChars = reportText.Length - currentPosition;
                                }

                                result = (int)(command.Parameters["@reportID"].Value);

                                break;
                            case CustomReport.Operation.Rename://rename
                                SqlHelper.AssignParameterValues(
                                    command.Parameters,
                                    reportID,
                                    operation,
                                    reportName,
                                    null,
                                    null,
                                    showTopServers);

                                command.ExecuteNonQuery();
                                result = (int)(command.Parameters["@reportID"].Value);
                                break;
                            case CustomReport.Operation.New://create a new report
                                //replace the existing report text with reportText
                                currentPosition = 0;
                                remainingChars = reportText.Length - currentPosition;

                                reportWriteBuffer = remainingChars > BUFFERSIZE ? reportText.Substring(0, BUFFERSIZE) : reportText;

                                result = -1;

                                SqlHelper.AssignParameterValues(
                                    command.Parameters,
                                    result,
                                    operation,
                                    reportName,
                                    reportShortDescription,
                                    reportWriteBuffer,
                                    showTopServers);

                                command.ExecuteNonQuery();

                                currentPosition += reportWriteBuffer.Length;
                                remainingChars = reportText.Length - currentPosition;

                                //then append the rest
                                while (remainingChars > 0)
                                {
                                    //write buffer gets the next BUFFERSIZE or teh rest of the string, whichever is shorter
                                    reportWriteBuffer = remainingChars > BUFFERSIZE ? reportText.Substring(currentPosition, BUFFERSIZE) : reportText.Substring(currentPosition);

                                    SqlHelper.AssignParameterValues(
                                        command.Parameters,
                                        null,
                                        3,//append
                                        reportName, null,
                                        reportWriteBuffer, showTopServers);

                                    command.ExecuteNonQuery();

                                    currentPosition += reportWriteBuffer.Length;
                                    remainingChars = reportText.Length - currentPosition;
                                }
                                result = (int)(command.Parameters["@reportID"].Value);
                                break;
                        }

                        xa.Commit();
                    }
                }
                catch (Exception)
                {
                    if (xa != null)
                        try { xa.Rollback(); }
                        catch (Exception) { /* */ }
                    throw;
                }
                finally
                {
                    if (xa != null)
                        try { xa.Dispose(); }
                        catch (Exception) { /* */ }
                }
            }
            return result;
        }
        #endregion

        #region Replication
        /// <summary>
        /// Deletes the specified entry for the specified server and guid from mirroring participants
        /// </summary>
        /// <param name="repositoryConnectionString"></param>
        /// <param name="PublisherInstance"></param>
        /// <param name="PublisherDB"></param>
        /// <param name="Publication"></param>
        /// <param name="SubscriberInstance"></param>
        /// <param name="SubscriberDB"></param>
        public static void DeleteReplicationSessionFromServer(string repositoryConnectionString, string PublisherInstance, string PublisherDB, string Publication, string SubscriberInstance, string SubscriberDB)
        {
            using (LOG.DebugCall())
            {
                if (repositoryConnectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    SqlHelper.ExecuteNonQuery(connection, ReplicationDeleteSessionFromServer,
                        PublisherInstance,
                        PublisherDB,
                        Publication,
                        SubscriberInstance,
                        SubscriberDB);
                    LOG.Debug("ExecuteNonQuery(", ReplicationDeleteSessionFromServer, ", ", PublisherInstance + ")");
                }
            }
        }

        public static Dictionary<int, ReplicationSession> GetReplicationTopology(string repositoryConnectionString, int ServerID)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            Dictionary<int, ReplicationSession> participants = new Dictionary<int, ReplicationSession>();

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                using (SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(connection, ReplicationGetParticipantsForServer, ServerID))
                {
                    while (dataReader.Read())
                    {
                        //if any of the value type are null then dont save the session
                        if (!dataReader.IsDBNull(1) && !dataReader.IsDBNull(2))
                        {
                            //int serverID = 0;
                            ReplicationSession session = new ReplicationSession();

                            if (!dataReader.IsDBNull(0)) session.ArticleCount = dataReader.GetInt32(0);
                            if (!dataReader.IsDBNull(1)) session.PublisherInstance = dataReader.GetString(1);
                            if (!dataReader.IsDBNull(2)) session.PublisherDB = dataReader.GetString(2);
                            if (!dataReader.IsDBNull(3)) session.DistributorInstance = dataReader.GetString(3);
                            if (!dataReader.IsDBNull(4)) session.DistributorDB = dataReader.GetString(4);
                            if (!dataReader.IsDBNull(5)) session.SubscriberInstance = dataReader.GetString(5);
                            if (!dataReader.IsDBNull(6)) session.SubscriberDB = dataReader.GetString(6);
                            if (!dataReader.IsDBNull(7)) session.LastSnapshotDateTime = dataReader.GetDateTime(7);
                            if (!dataReader.IsDBNull(8)) session.SubscribedTransactions = dataReader.GetInt32(8);
                            if (!dataReader.IsDBNull(9)) session.NonSubscribedTransactions = dataReader.GetInt32(9);
                            if (!dataReader.IsDBNull(10)) session.NonDistributedTransactions = dataReader.GetInt32(10);
                            if (!dataReader.IsDBNull(11)) session.ReplicationLatency = dataReader.GetDouble(11);
                            if (!dataReader.IsDBNull(12)) session.MaxSubscriptionLatency = dataReader.GetInt32(12);
                            if (!dataReader.IsDBNull(13)) session.ReplicationType = dataReader.GetByte(13);
                            if (!dataReader.IsDBNull(14)) session.SubscriptionType = dataReader.GetByte(14);
                            if (!dataReader.IsDBNull(15)) session.LastSubscriberUpdate = dataReader.GetDateTime(15);
                            if (!dataReader.IsDBNull(16)) session.LastSyncStatus = dataReader.GetByte(16);
                            if (!dataReader.IsDBNull(17)) session.LastSyncSummary = dataReader.GetString(17);
                            if (!dataReader.IsDBNull(18)) session.LastSyncTime = dataReader.GetDateTime(18);
                            if (!dataReader.IsDBNull(19)) session.SubscriptionStatus = dataReader.GetByte(19);
                            if (!dataReader.IsDBNull(20)) session.Publication = dataReader.GetString(20);
                            if (!dataReader.IsDBNull(21)) session.PublicationDescription = dataReader.GetString(21);
                            if (!dataReader.IsDBNull(22)) session.PublisherSQLServerID = dataReader.GetInt32(22);
                            if (!dataReader.IsDBNull(23)) session.DistributorSQLServerID = dataReader.GetInt32(23);
                            if (!dataReader.IsDBNull(24)) session.SubscriberSQLServerID = dataReader.GetInt32(24);

                            int key = (session.PublisherInstance + "."
                                + session.PublisherDB + "."
                                + session.Publication + "."
                                + (session.SubscriberInstance ?? "") + "."
                                + (session.SubscriberDB ?? "")).GetHashCode();

                            if (!participants.ContainsKey(key))
                            {
                                participants.Add(key, session);
                            }
                            else
                            {
                                participants[key] = session;
                            }
                        }
                    }
                }
            }
            return participants;
        }
        #endregion

        #region Tags

        public static int UpdateTagConfiguration(string connectionString, Tag tag)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }

            if (tag == null)
            {
                throw new ArgumentNullException("tag");
            }

            XmlDocument serversParameter = new XmlDocument();
            XmlElement serversElement = serversParameter.CreateElement("Servers");
            serversParameter.AppendChild(serversElement);

            foreach (int id in tag.Instances)
            {
                XmlElement serverElement = serversParameter.CreateElement("Server");
                serverElement.SetAttribute("ServerId", id.ToString());
                serversElement.AppendChild(serverElement);
            }

            XmlDocument customCountersParameter = new XmlDocument();
            XmlElement customCountersElement = customCountersParameter.CreateElement("CustomCounters");
            customCountersParameter.AppendChild(customCountersElement);

            foreach (int id in tag.CustomCounters)
            {
                XmlElement customCounterElement = customCountersParameter.CreateElement("CustomCounter");
                customCounterElement.SetAttribute("CustomCounterId", id.ToString());
                customCountersElement.AppendChild(customCounterElement);
            }

            XmlDocument permissionsParameter = new XmlDocument();
            XmlElement permissionsElement = permissionsParameter.CreateElement("Permissions");
            permissionsParameter.AppendChild(permissionsElement);

            foreach (int id in tag.Permissions)
            {
                XmlElement permissionElement = permissionsParameter.CreateElement("Permission");
                permissionElement.SetAttribute("PermissionId", id.ToString());
                permissionsElement.AppendChild(permissionElement);
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = SqlHelper.CreateCommand(connection, UpdateTagConfigurationStoredProcedure))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, tag.Id, tag.Name, serversParameter.InnerXml,
                                                    customCountersParameter.InnerXml, permissionsParameter.InnerXml, 1);
                    command.ExecuteNonQuery();
                    return (int)(command.Parameters["@TagId"].Value);
                }
            }
        }

        public static void RemoveTags(string connectionString, IList<int> tagIds)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }

            XmlDocument tagsParameter = new XmlDocument();
            XmlElement tagsElement = tagsParameter.CreateElement("Tags");
            tagsParameter.AppendChild(tagsElement);

            foreach (int id in tagIds)
            {
                XmlElement tagElement = tagsParameter.CreateElement("Tag");
                tagElement.SetAttribute("TagId", id.ToString());
                tagsElement.AppendChild(tagElement);
            }

            SqlHelper.ExecuteNonQuery(connectionString, DeleteTagsByIdStoredProcedure, tagsParameter.InnerXml);
        }

        public static Triple<MultiDictionary<int, int>,
                             MultiDictionary<int, int>,
                             MultiDictionary<int, int>>
            GetTagAssociations(string connectionString, int? tagId, bool includeServerTags, bool includeCounterTags, bool includePermissionTags)
        {
            using (LOG.InfoCall("GetTagAssociations"))
            {
                Triple<MultiDictionary<int, int>,
                    MultiDictionary<int, int>,
                    MultiDictionary<int, int>> result =
                        new Triple<MultiDictionary<int, int>,
                            MultiDictionary<int, int>,
                            MultiDictionary<int, int>>();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString,
                                                                      GetTagAssociationsStoredProcedure,
                                                                      tagId.HasValue
                                                                          ? (object)tagId.Value
                                                                          : DBNull.Value,
                                                                      includeServerTags,
                                                                      includeCounterTags,
                                                                      includePermissionTags))
                {
                    bool more = true;
                    if (includeServerTags)
                    {
                        result.First = ReadServerAssociations(reader);
                        more = reader.NextResult();
                    }
                    if (more && includeCounterTags)
                    {
                        result.Second = ReadServerAssociations(reader);
                        more = reader.NextResult();
                    }
                    if (more && includePermissionTags)
                    {
                        result.Third = ReadServerAssociations(reader);
                    }
                }

                return result;
            }
        }

        public static Tag GetTagById(String connectionString, int tagId)
        {
            Tag resultTag = null;
            ICollection<Tag> allTags = GetTags(connectionString);

            foreach (Tag tag in allTags)
            {
                if (tagId == tag.Id)
                {
                    resultTag = tag;
                    break;
                }
            }

            return resultTag;
        }

        public static Dictionary<int, Tag> GetTagsByPermissionID(String connectionString, int permisionID)
        {
            Dictionary<int, Tag> resultTag = new Dictionary<int, Tag>();
            ICollection<Tag> allTags = GetTags(connectionString);

            foreach (Tag tag in allTags)
            {
                if (tag.Permissions.Contains(permisionID))
                {
                    resultTag.Add(tag.Id, tag);
                }
            }

            return resultTag;
        }

        public static ICollection<Tag> GetTags(String connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connectionString, GetTagsStoredProcedure))
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

                return new List<Tag>(tags.Values);
            }
        }

        public static List<Pair<String, int?>> GetExcludedWaitTypes(string connectionString)
        {
            using (LOG.DebugCall("GetExcludedWaitTypes"))
            {
                List<Pair<String, int?>> result = new List<Pair<String, int?>>();
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString,
                                                                     GetExcludedWaitTypesStoredProcedure))
                {
                    while (reader.Read())
                    {

                        if (!reader.IsDBNull(0))
                        {
                            int? waitNumber = null;
                            if (!reader.IsDBNull(1)) waitNumber = reader.GetInt32(1);

                            result.Add(new Pair<string, int?>(reader.GetString(0), waitNumber));
                        }
                    }
                }
                return result;
            }
        }

        private static MultiDictionary<int, int> ReadServerAssociations(IDataReader reader)
        {
            MultiDictionary<int, int> result = new MultiDictionary<int, int>(false);
            while (reader.Read())
            {
                result.Add(reader.GetInt32(0), reader.GetInt32(1));
            }
            return result;
        }

        #endregion

        #region upgrade

        internal static void PostInstallUpgrade(string connectionString)
        {
            using (LOG.InfoCall("PostInstallUpgrade"))
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.InfoMessage += Log_InfoMessage;
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                    try
                    {
                        // SQLdm FixPack1 - SQLDM-28158 (Varun Chopra)- Stores first time upgrade to DM10.2.3 or above to ensure UI changes for first time post upgrade
                        int isUpgradedToDM10_2_3 = 1;
                        using (SqlCommand command = connection.CreateCommand())
                        {

                            command.Transaction = transaction;
                            command.CommandTimeout = 0;  // <-- not ideal but how should I know how long this will take
                            command.CommandText = PostInstallUpgradeProcedure;
                            command.CommandType = CommandType.StoredProcedure;
                            // SQLdm FixPack1 - SQLDM-28158 (Varun Chopra)- Reads upgrade to 10.2.3 value
                            using (SqlDataReader dataReader = command.ExecuteReader())
                            {
                                if (dataReader.HasRows)
                                {
                                    while (dataReader.Read())
                                    {
                                        try
                                        {
                                            var UpgradedToDM10_2_3Index = dataReader.GetOrdinal("UpgradedToDM10_2_3");
                                            if (!dataReader.IsDBNull(UpgradedToDM10_2_3Index))
                                            {
                                                isUpgradedToDM10_2_3 = dataReader.GetInt32(UpgradedToDM10_2_3Index);
                                            }
                                            else
                                            {
                                                LOG.Error("PostInstallUpgrade: UpgradedToDM10_2_3 null value during post-install upgrade script.");
                                            }
                                        }
                                        catch (Exception exception)
                                        {
                                            LOG.Error(
                                                "PostInstallUpgrade: Error fetching UpgradedToDM10_3 value during post-install upgrade script: ",
                                                exception);
                                        }

                                    }
                                }
                            }

                        }

                        // SQLdm FixPack1 - SQLDM-28158 (Varun Chopra) isUpgradedToDM10_2_3  will not be set for first time upgrade scenario
                        if (isUpgradedToDM10_2_3 != 1)
                        {
                            // SQLDM 10.1 (Barkha Khatri) SCOM feature-- adding default rule to send all alerts as Events to SCOM(if rule is not there already)
                            AddSCOMDefaultRule(transaction);
                        }
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Error running post-install upgrade script: ", e);
                        transaction.Rollback();
                    }
                }

            }
        }

        /// <summary>
        /// START SQLDM 10.1 (Barkha Khatri) SCOM feature-- adding default rule to send all alerts as Events to SCOM(if rule is not there already)
        /// </summary>
        /// <param name="transaction"></param>
        private static void AddSCOMDefaultRule(SqlTransaction transaction)
        {
            bool scomRuleExists = false;
            foreach (NotificationRule rule in Management.Notification.GetNotificationRules())
            {
                if (rule.Description.ToLower().Equals(Idera.SQLdm.Common.Constants.SCOMDefaultRuleDescription.ToLower()))
                {
                    scomRuleExists = true;
                    break;
                }
            }
            if (!scomRuleExists)
            {
                NotificationRule scomDefaultRule = NotificationManager.CreateSCOMDefaultNotificationRule();
                scomDefaultRule = Management.Notification.AddNotificationRule(transaction, scomDefaultRule);
                scomDefaultRule.installationAction(transaction, scomDefaultRule.Id);

            }
        }
        //END SQLDM 10.1 (Barkha Khatri) SCOM feature-- adding default rule to send all alerts as Events to SCOM
        #endregion

        #region Predictive

        public static Dictionary<int, List<int>> GetServerAlerts(string repositoryConnectionString)
        {
            Dictionary<int, List<int>> serverAlerts = new Dictionary<int, List<int>>();

            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("select SQLServerID, Metric from MetricThresholds where Enabled = 1 group by SQLServerID, Metric order by SQLServerID, Metric", connection))
                {
                    command.CommandType = CommandType.Text;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return serverAlerts;

                        int serverid = 0;
                        int metricid = 0;

                        while (reader.Read())
                        {
                            serverid = reader.GetInt32(0);  // serverids can be duplicated
                            metricid = reader.GetInt32(1);

                            if (!serverAlerts.ContainsKey(serverid))
                                serverAlerts.Add(serverid, new List<int>());

                            serverAlerts[serverid].Add(metricid);
                        }
                    }
                }
            }

            return serverAlerts;
        }

        public static DataTable GetPredictiveModelServers(string repositoryConnectionString)
        {
            DataTable table = new DataTable();

            table.Columns.Add("serverid", typeof(int));

            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("select distinct(SQLServerID) from PredictiveModels", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return table;

                        while (reader.Read())
                        {
                            DataRow row = table.NewRow();

                            row[0] = reader.GetInt32(0);

                            table.Rows.Add(row);
                        }
                    }
                }
            }

            return table;
        }

        public static Dictionary<Triple<int, int, int>, byte[]> GetPredictiveModelsForServer(string repositoryConnectionString, int serverid)
        {
            Dictionary<Triple<int, int, int>, byte[]> models = new Dictionary<Triple<int, int, int>, byte[]>();

            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                connection.Open();

                string sql = string.Format("select datalength(Model), Model, Metric, Severity, Timeframe from PredictiveModels where SQLServerID = {0}", serverid);

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return models;

                        while (reader.Read())
                        {
                            int bufferSize = reader.GetInt32(0);
                            byte[] buffer = new byte[bufferSize];

                            // get the bytes for the model
                            reader.GetBytes(1, 0, buffer, 0, bufferSize);

                            // build the key based on the metric, severity and timeframe
                            Triple<int, int, int> key = new Triple<int, int, int>(reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4));

                            // save the model associated this key
                            models.Add(key, buffer);
                        }

                        return models;
                    }
                }
            }
        }

        private static DateTime TrainingDataReferenceDateTime = new DateTime(2000, 1, 1, 0, 0, 0);  // Jan 1, 2000

        public static DataTable GetPredictiveModelInput(string repositoryConnectionString, int serverid, int intervalMinutes, DateTime cutoffDateTime)
        {
            DataTable schemaTable = new DataTable();

            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, "p_GetPredictiveModelInput", serverid, intervalMinutes, cutoffDateTime.ToUniversalTime(), cutoffDateTime.ToUniversalTime()))
                {
                    if (!reader.HasRows)
                        return null;

                    // get the row
                    reader.Read();

                    // if we have a null for any of the first 5 columns (dates), then we don't have useful data (other columns are checked below)
                    if (reader[0] is DBNull || reader[1] is DBNull || reader[2] is DBNull || reader[3] is DBNull || reader[4] is DBNull)
                        return null;

                    // initialize new table to correct number of columns (+3 since the returned input doesn't have the 3 date parts)
                    int numColumns = reader.FieldCount + 4;
                    for (int i = 0; i < numColumns; i++)
                        schemaTable.Columns.Add(string.Format("column{0}", i), typeof(double));

                    DataRow row = schemaTable.NewRow();

                    // add a row for the state value (even though we don't populate it or use it, it's still referenced by models as classindex)
                    row[0] = double.NaN;

                    // have to add the date parts manually
                    DateTime timestamp = (DateTime)reader[0];
                    row[1] = ((TimeSpan)(timestamp - TrainingDataReferenceDateTime)).TotalMilliseconds;
                    row[2] = timestamp.Day;
                    row[3] = timestamp.DayOfWeek + 1;
                    row[4] = timestamp.Hour;

                    int j = 0;

                    // loop through and get all the values - start at 4 to skip the date columns
                    for (int i = 1; i < reader.FieldCount; i++)
                    {
                        j = i + 4; // the row has 4 more columns (at the beginning) than the reader, so we have to offset by 3

                        if (reader[i] is string)
                        {
                            row[j] = double.NaN;
                            LOG.Debug("String columns are not allowed in the input data for the Predictive Analytics Service.");
                        }
                        else if (reader[i] is bool)
                        {
                            row[j] = (bool)reader[i] ? 1 : 0;
                        }
                        else if (reader[i] is int)
                        {
                            row[j] = (double)(int)reader[i];
                        }
                        else if (reader[i] is long)
                        {
                            row[j] = (double)(long)reader[i];
                        }
                        else if (reader[i] is float)
                        {
                            row[j] = (double)(float)reader[i];
                        }
                        else if (reader[i] is decimal)
                        {
                            row[j] = (double)(decimal)reader[i];
                        }
                        else if (reader[i] is double)
                        {
                            row[j] = (double)reader[i];
                        }
                        else if (reader[i] is DBNull)
                        {
                            row[j] = double.NaN;
                        }
                        else
                        {
                            row[j] = double.NaN;
                            LOG.Debug("An unhandled column type was encountered in the training data for the Predictive Analytics Service. [" + reader[i].ToString() + "]");
                        }
                    }

                    schemaTable.Rows.Add(row);
                    return schemaTable;
                }
            }
        }

        public static Pair<DataTable, List<DateTime>> GetPredictiveTrainingData(string repositoryConnectionString, int serverid, int metricid, int severity, int timeframe, DateTime cutoffDateTime)
        {
            List<DateTime> alertDateTimes = new List<DateTime>();
            DataTable table = new DataTable();
            //bool           hasNull        = false;

            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, "p_GetPredictiveTrainingData", serverid, metricid, severity, timeframe, timeframe, cutoffDateTime.ToUniversalTime()))
                {
                    if (!reader.HasRows)
                        return new Pair<DataTable, List<DateTime>>();

                    // initialize new table to correct number of columns
                    for (int i = 0; i < reader.FieldCount; i++)
                        table.Columns.Add(string.Format("column{0}", i), typeof(double));

                    while (reader.Read())
                    {
                        DataRow row = table.NewRow();

                        //hasNull = false;

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader[i] is string)
                            {
                                row[i] = double.NaN;
                                LOG.Debug("String columns are not allowed in the training data for the Predictive Analytics Service.");
                            }
                            else if (reader[i] is bool)
                            {
                                row[i] = (bool)reader[i] ? 1 : 0;
                            }
                            else if (reader[i] is int)
                            {
                                row[i] = (double)(int)reader[i];
                            }
                            else if (reader[i] is long)
                            {
                                row[i] = (double)(long)reader[i];
                            }
                            else if (reader[i] is float)
                            {
                                row[i] = (double)(float)reader[i];
                            }
                            else if (reader[i] is decimal)
                            {
                                row[i] = (double)(decimal)reader[i];
                            }
                            else if (reader[i] is double)
                            {
                                row[i] = (double)reader[i];
                            }
                            else if (reader[i] is DateTime)
                            {
                                row[i] = ((TimeSpan)((DateTime)reader[i] - TrainingDataReferenceDateTime)).TotalMilliseconds;

                                // check the state column, a 1 means it's an alert time (we need for later processing)
                                if ((double)row[0] == 1.0)
                                    alertDateTimes.Add((DateTime)reader[i]);
                            }
                            else if (reader[i] is DBNull)
                            {
                                //hasNull = true;
                                row[i] = double.NaN;
                            }
                            else
                            {
                                row[i] = double.NaN;
                                LOG.Debug("An unhandled column type was encountered in the training data for the Predictive Analytics Service.");
                            }
                        }

                        table.Rows.Add(row);
                    }
                }
            }

            return new Pair<DataTable, List<DateTime>>(table, alertDateTimes);
        }

        public static void SavePredictiveModel(string repositoryConnectionString, int serverid, int metricid, int severity, int timeframe, byte[] modelBuffer)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                connection.Open();

                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_AddPredictiveModel"))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, serverid, metricid, severity, timeframe, modelBuffer);

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        LOG.ErrorFormat("Error occurred saving model using params: [{0}, {1}, {2}, {3}]", serverid, metricid, severity, timeframe);
                        LOG.Error("Caught exception saving predictive model.", e);
                    }
                }
            }
        }

        public static void SavePredictiveForecast(string repositoryConnectionString, int serverid, int metricid, int severity, int timeframe, int forecast, double accuracy, DateTime expiration)
        {
            decimal roundedAccuracy = decimal.Round((decimal)accuracy, 2);

            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                connection.Open();

                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_AddPredictiveForecast"))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, serverid, metricid, severity, timeframe, forecast, roundedAccuracy, expiration.ToUniversalTime());

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        LOG.ErrorFormat("Error occurred saving forecast using params: [{0}, {1}, {2}, {3}, {4}, {5}]", serverid, metricid, severity, timeframe, forecast, accuracy);
                        LOG.Error("Caught exception saving predictive forecast.", e);
                    }
                }
            }
        }

        public static void GroomExpiredForecasts(string repositoryConnectionString)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            {
                connection.Open();

                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_GroomExpiredForecasts"))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Caught exception grooming expired forecasts.", e);
                    }
                }
            }
        }

        public static bool GetPredictiveAnalyticsEnabled(string repositoryConnectionString)
        {
            using (LOG.InfoCall("GetPredictiveAnalyticsEnabled"))
            {
                if (repositoryConnectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandTimeout = SqlHelper.CommandTimeout;
                        command.CommandText = "p_GetPredictiveAnalyticsEnabled";
                        command.CommandType = CommandType.StoredProcedure;
                        SqlParameter enabledParam = command.Parameters.Add("@PredictiveAnalyticsEnabled", SqlDbType.Int);
                        enabledParam.Direction = ParameterDirection.Output;

                        command.ExecuteNonQuery();
                        int enabledInt = 0;
                        Int32.TryParse(enabledParam.SqlValue.ToString(), out enabledInt);
                        return enabledInt == 1;
                    }
                }
            }
        }

        public static void SetPredictiveAnalyticsEnabled(string repositoryConnectionString, bool predictiveAnalyticsEnabled)
        {
            using (LOG.InfoCall("SetPredictiveAnalyticsEnabled"))
            {
                if (repositoryConnectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandTimeout = SqlHelper.CommandTimeout;
                        command.CommandText = "p_SetPredictiveAnalyticsEnabled";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PredictiveAnalyticsEnabled", predictiveAnalyticsEnabled ? 1 : 0);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void SetNextPredictiveAnalyticsModelRebuild(string repositoryConnectionString, DateTime dt)
        {
            using (LOG.InfoCall("SetNextPredictiveAnalyticsModelRebuild"))
            {
                if (repositoryConnectionString == null)
                    throw new ArgumentNullException("repositoryConnectionString");

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_SetNextPredictiveModelRebuild"))
                    {
                        SqlHelper.AssignParameterValues(command.Parameters, dt);

                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            LOG.Error("Caught exception saving predictive model rebuild time.", e);
                        }
                    }
                }
            }
        }

        public static DateTime GetNextPredictiveAnalyticsModelRebuild(string repositoryConnectionString)
        {
            using (LOG.InfoCall("GetNextPredictiveAnalyticsModelRebuild"))
            {
                if (repositoryConnectionString == null)
                    throw new ArgumentNullException("repositoryConnectionString");

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();

                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, "p_GetNextPredictiveModelRebuild"))
                    {
                        if (reader == null || !reader.HasRows)
                            return DateTime.MinValue;

                        reader.Read();

                        string data = reader[0] as string;

                        return DateTime.Parse(data);
                    }
                }
            }
        }

        public static void SetNextPredictiveAnalyticsForecast(string repositoryConnectionString, DateTime dt)
        {
            using (LOG.InfoCall("SetNextPredictiveAnalyticsForecast"))
            {
                if (repositoryConnectionString == null)
                    throw new ArgumentNullException("repositoryConnectionString");

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_SetNextPredictiveForecast"))
                    {
                        SqlHelper.AssignParameterValues(command.Parameters, dt);

                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            LOG.Error("Caught exception saving predictive model forecast.", e);
                        }
                    }
                }
            }
        }

        public static DateTime GetNextPredictiveAnalyticsForecast(string repositoryConnectionString)
        {
            using (LOG.InfoCall("GetNextPredictiveAnalyticsForecast"))
            {
                if (repositoryConnectionString == null)
                    throw new ArgumentNullException("repositoryConnectionString");

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();

                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, "p_GetNextPredictiveForecast"))
                    {
                        if (reader == null || !reader.HasRows)
                            return DateTime.MinValue;

                        reader.Read();

                        string data = reader[0] as string;

                        return DateTime.Parse(data);
                    }
                }
            }
        }

        public static bool GetPredictiveAnalyticsHasModels(string repositoryConnectionString)
        {
            using (LOG.InfoCall("GetPredictiveAnalyticsHasModels"))
            {
                if (repositoryConnectionString == null)
                    throw new ArgumentNullException("repositoryConnectionString");

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();

                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, "p_GetPredictiveModelsCount"))
                    {
                        if (reader == null || !reader.HasRows)
                            return false;

                        reader.Read();

                        return reader.GetInt32(0) > 0;
                    }
                }
            }
        }

        internal static DataTable GetActiveAlerts(string repositoryConnectionString, string instanceName, string databaseName, int? maxRows)
        {
            using (LOG.InfoCall("GetActiveAlerts"))
            {
                if (repositoryConnectionString == null)
                    throw new ArgumentNullException("repositoryConnectionString");

                DataTable table = new DataTable("Alerts");
                table.Columns.Add("AlertID", typeof(long));
                table.Columns.Add("UTCOccurrenceDateTime", typeof(DateTime));
                table.Columns.Add("InstanceName", typeof(string));
                table.Columns.Add("DatabaseName", typeof(string));
                table.Columns.Add("TableName", typeof(string));
                table.Columns.Add("Active", typeof(bool));
                table.Columns.Add("Metric", typeof(int));
                table.Columns.Add("MetricName", typeof(string));
                table.Columns.Add("Severity", typeof(byte));
                table.Columns.Add("StateEvent", typeof(byte));
                table.Columns.Add("StateEventDesc", typeof(string));
                table.Columns.Add("Value", typeof(float));
                table.Columns.Add("Heading", typeof(string));
                table.Columns.Add("Message", typeof(string));
                table.Columns.Add("Category", typeof(string));

                MetricDefinitions defs = Management.GetMetricDefinitions();

                string xml = null;
                if (!String.IsNullOrEmpty(instanceName))
                {
                    StringBuilder xmldoc = new StringBuilder();
                    using (XmlWriter writer = XmlWriter.Create(xmldoc))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("Servers");
                        writer.WriteStartElement("Server");
                        writer.WriteAttributeString("InstanceName", instanceName);
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                    xml = xmldoc.ToString();
                }
                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();
                    int rows = maxRows.HasValue ? maxRows.Value : 0;
                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, "p_GetAlerts", 0, null, null, xml, databaseName, null, null, null, null, true, rows))
                    {
                        while (reader.Read())
                        {
                            int metricId = reader.GetInt32(6);
                            MetricDescription? def = defs.GetMetricDescription(metricId);
                            if (def == null)
                                continue;

                            byte transition = reader.GetByte(8);

                            DataRow row = table.NewRow();
                            row["AlertID"] = reader.GetInt64(0);
                            row["UTCOccurrenceDateTime"] = reader.GetDateTime(1);
                            row["InstanceName"] = reader.GetString(2);
                            row["DatabaseName"] = GetNullableValue(reader, 3);
                            row["TableName"] = GetNullableValue(reader, 4);
                            row["Active"] = GetNullableValue(reader, 5);
                            row["Metric"] = metricId;
                            row["MetricName"] = def.Value.Name;
                            row["Category"] = def.Value.Category;
                            row["Severity"] = GetNullableValue(reader, 7);
                            row["StateEvent"] = transition;
                            row["StateEventDesc"] = TransitionToString(transition);
                            row["Value"] = GetNullableValue(reader, 9);
                            row["Heading"] = GetNullableValue(reader, 10);
                            row["Message"] = GetNullableValue(reader, 11);
                            table.Rows.Add(row);
                        }
                    }
                }
                table.RemotingFormat = SerializationFormat.Binary;
                return table;
            }
        }

        public const string RAISED_TO_CRITICAL = "Raised to critical";
        public const string LOWERED_TO_OK = "Lowered to OK";
        private static string TransitionToString(byte transitionId)
        {
            switch (transitionId)
            {
                case (byte)Transition.OK_Info:
                    return "Raised to Informational";
                case (byte)Transition.OK_Warning:
                    return "Raised to warning";
                case (byte)Transition.OK_Critical:
                    return RAISED_TO_CRITICAL;
                case (byte)Transition.Info_OK:
                    return LOWERED_TO_OK;
                case (byte)Transition.Info_Info:
                    return "Remained informational";
                case (byte)Transition.Info_Warning:
                    return "Raised to warning";
                case (byte)Transition.Info_Critical:
                    return RAISED_TO_CRITICAL;
                case (byte)Transition.Warning_OK:
                    return LOWERED_TO_OK;
                case (byte)Transition.Warning_Info:
                    return "Lowered to informational";
                case (byte)Transition.Warning_Warning:
                    return "Remained warning";
                case (byte)Transition.Warning_Critical:
                    return RAISED_TO_CRITICAL;
                case (byte)Transition.Critical_OK:
                    return LOWERED_TO_OK;
                case (byte)Transition.Critical_Info:
                    return "Lowered to informational";
                case (byte)Transition.Critical_Warning:
                    return "Lowered to warning";
                case (byte)Transition.Critical_Critical:
                    return "Remained critical";
            }
            return String.Empty;
        }

        private static object GetNullableValue(SqlDataReader reader, int col)
        {
            if (reader.IsDBNull(col))
                return DBNull.Value;
            return reader.GetValue(col);
        }

        #endregion

        #region Wait Type Definitions

        public static DataTable GetWaitTypeDefinitions()
        {
            string connString = ManagementServiceConfiguration.ConnectionString;

            if (string.IsNullOrEmpty(connString))
                throw new ArgumentNullException("ManagementServiceConfiguration.ConnectionString");

            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("CategoryID", typeof(int)).AllowDBNull = true;
            dataTable.Columns.Add("Category", typeof(string)).AllowDBNull = false;
            dataTable.Columns.Add("WaitType", typeof(string)).AllowDBNull = false;
            dataTable.Columns.Add("Description", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("HelpLink", typeof(string)).AllowDBNull = true;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();

                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, CommandType.Text, "select wc.CategoryID as 'CategoryID', case when wc.CategoryID is null then 'Other' else wc.Category end as 'Category', wt.WaitType   as 'WaitType', wt.Description as 'Description', wt.HelpLink as 'HelpLink' from WaitTypes wt left join WaitCategories wc on wc.CategoryID = wt.CategoryID  where wc.ExcludeFromCollection = 0"))
                {
                    dataTable.BeginLoadData();

                    while (dataReader.Read())
                    {
                        DataRow row = dataTable.NewRow();

                        row["CategoryID"] = dataReader["CategoryID"];
                        row["Category"] = dataReader["Category"];
                        row["WaitType"] = dataReader["WaitType"];
                        row["Description"] = dataReader["Description"];
                        row["HelpLink"] = dataReader["HelpLink"];

                        dataTable.Rows.Add(row);
                    }

                    dataTable.EndLoadData();
                }
            }

            return dataTable;
        }

        #endregion

        #region Instance Thresholds

        public static List<string> GetDisks(int sqlServerId)
        {
            var connectionString = ManagementServiceConfiguration.ConnectionString;

            if (String.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("ManagementServiceConfiguration.ConnectionString");

            var result = new List<string>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var reader = SqlHelper.ExecuteReader(connection, CommandType.Text, String.Format(GetDiskDrives, sqlServerId)))
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0) && !result.Contains(reader.GetString(0)))
                            result.Add(reader.GetString(0));
                    }
                }
            }

            return result;
        }

        public static PermissionDefinition GetPermissionDefinitionById(int permissionId, string repositoryConnectionString)
        {
            PermissionDefinition permission = null;

            using (LOG.InfoCall("Repository Helper - GetPermissionDefinitionById"))
            {
                SqlDataReader reader = SqlHelper.ExecuteReader(repositoryConnectionString, "p_GetPermissions");
                bool isSecurityEnabled = false;
                // Refresh permissions.
                int sysPermissionID = -1;

                // Read the security enabled flag.
                int ordinalPermisionId = 0;
                if (reader.Read())
                {
                    isSecurityEnabled = !reader.IsDBNull(ordinalPermisionId) && reader.GetSqlBoolean(ordinalPermisionId).Value;
                }

                // Read permissions.
                if (isSecurityEnabled && reader.NextResult())
                {
                    while (reader.Read())
                    {
                        // Read permission ID.   If the permission id is -1, it means its a system
                        // permission.  So use a different index that keeps decreasing.
                        SqlInt32 permissionID = reader.GetInt32(ordinalPermisionId);

                        if (permissionID.Value == permissionId)
                        {
                            if (permission == null)
                            {
                                int id = permissionID.Value == -1 ? sysPermissionID-- : permissionID.Value;

                                // If permission exists, then add the monitored server instance to the 
                                // permission.   Else create a new permission and add to dictionary.
                                // Read the fields and create permission object.
                                SqlInt32 system = reader.GetInt32(reader.GetOrdinal("System"));
                                SqlInt32 enabled = reader.GetSqlInt32(reader.GetOrdinal("Enabled"));
                                SqlBinary loginSID = reader.GetSqlBinary(reader.GetOrdinal("LoginSID"));
                                SqlString login = reader.IsDBNull(reader.GetOrdinal("Login")) ? SqlString.Null : reader.GetSqlString(reader.GetOrdinal("Login"));
                                SqlInt32 loginType = reader.GetSqlInt32(reader.GetOrdinal("LoginType"));
                                SqlInt32 permissionType = reader.GetSqlInt32(reader.GetOrdinal("PermissionType"));
                                SqlInt32 sqlServerID = reader.IsDBNull(reader.GetOrdinal("SQLServerID")) ? SqlInt32.Null : reader.GetSqlInt32(reader.GetOrdinal("SQLServerID"));
                                SqlString instanceName = reader.IsDBNull(reader.GetOrdinal("InstanceName")) ? SqlString.Null : reader.GetSqlString(reader.GetOrdinal("InstanceName"));
                                SqlBoolean active = reader.IsDBNull(reader.GetOrdinal("Active")) ? SqlBoolean.Null : reader.GetSqlBoolean(reader.GetOrdinal("Active"));
                                SqlBoolean deleted = reader.IsDBNull(reader.GetOrdinal("Deleted")) ? SqlBoolean.Null : reader.GetSqlBoolean(reader.GetOrdinal("Deleted"));
                                SqlString comment = reader.IsDBNull(reader.GetOrdinal("Comment")) ? SqlString.Null : reader.GetSqlString(reader.GetOrdinal("Comment"));
                                SqlInt32 webAppPermission = reader.GetSqlInt32(reader.GetOrdinal("WebAppPermission"));

                                if (!sqlServerID.IsNull && !instanceName.IsNull && !active.IsNull)
                                {
                                    permission = new PermissionDefinition(id, system.Value == 1, enabled.Value == 1, loginSID.Value,
                                        login.IsNull ? "" : login.Value, (LoginType)loginType.Value, (PermissionType)permissionType.Value,
                                        sqlServerID.Value, instanceName.Value, active.Value, deleted.Value,
                                        comment.Value, webAppPermission.Value == 1);
                                }
                                else
                                {
                                    permission = new PermissionDefinition(id, system.Value == 1, enabled.Value == 1, loginSID.Value,
                                        login.IsNull ? String.Empty : login.Value, (LoginType)loginType.Value, (PermissionType)permissionType.Value,
                                        comment.IsNull ? String.Empty : comment.Value, webAppPermission.Value == 1);
                                }
                            }
                            else
                            {
                                // Read server id and instance name, and add to the list of instances assigned to
                                // the permission.
                                SqlInt32 sqlServerID = reader.IsDBNull(reader.GetOrdinal("SQLServerID")) ? SqlInt32.Null : reader.GetInt32(reader.GetOrdinal("SQLServerID"));
                                SqlString instanceName = reader.IsDBNull(reader.GetOrdinal("InstanceName")) ? SqlString.Null : reader.GetSqlString(reader.GetOrdinal("InstanceName"));
                                SqlBoolean active = reader.IsDBNull(reader.GetOrdinal("Active")) ? SqlBoolean.Null : reader.GetSqlBoolean(reader.GetOrdinal("Active"));
                                SqlBoolean deleted = reader.IsDBNull(reader.GetOrdinal("Deleted")) ? SqlBoolean.Null : reader.GetSqlBoolean(reader.GetOrdinal("Deleted"));

                                if (!sqlServerID.IsNull && !instanceName.IsNull && !active.IsNull && !deleted.IsNull)
                                {
                                    permission.AddInstance(sqlServerID.Value, instanceName.Value, active.Value, deleted.Value);
                                }
                            }
                        }
                    }
                }
            }

            return permission;
        }

        public static List<int> GetAvailablePermissionsIds(String repositoryConnectionString)
        {
            List<int> permissions = new List<int>();

            using (LOG.InfoCall("Repository Helper - GetPermissionDefinitionById"))
            {
                SqlDataReader reader = SqlHelper.ExecuteReader(repositoryConnectionString, "p_GetPermissions");
                bool isSecurityEnabled = false;
                // Refresh permissions.
                int sysPermissionID = -1;

                // Read the security enabled flag.
                int ordinalPermisionId = 0;
                if (reader.Read())
                {
                    isSecurityEnabled =
                        !reader.IsDBNull(ordinalPermisionId) && reader.GetSqlBoolean(ordinalPermisionId).Value;
                }

                // Read permissions.
                if (isSecurityEnabled && reader.NextResult())
                {
                    while (reader.Read())
                    {
                        // Read permission ID.   If the permission id is -1, it means its a system
                        // permission.  So use a different index that keeps decreasing.
                        SqlInt32 permissionId = reader.GetInt32(ordinalPermisionId);

                        int id = permissionId.Value == -1 ? sysPermissionID-- : permissionId.Value;

                        if (id > 0)
                        {
                            permissions.Add(id);
                        }
                    }
                }
            }

            return permissions;
        }

        /// <summary>
        /// Returns a list of tags associated with the given permission
        /// </summary>
        /// <param name="repositoryConnectionString"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public static List<int> GetTagIdsByPermissionId(String repositoryConnectionString, int permissionId)
        {
            List<int> tagIds = new List<int>();

            using (LOG.InfoCall("Repository Helper - GetTagIdsByPermissionId"))
            {
                SqlDataReader reader = SqlHelper.ExecuteReader(repositoryConnectionString, "p_GetPermissionTagsAndServers", permissionId);
                bool isSecurityEnabled = false;

                // Read Security Enabled.
                if (reader.Read())
                {
                    isSecurityEnabled =
                        !reader.IsDBNull(0) && reader.GetSqlBoolean(0).Value;
                }

                if (reader.NextResult())
                {
                    // Read tags.
                    while (reader.Read())
                    {
                        tagIds.Add(reader.GetInt32(0));
                    }
                }
            }

            return tagIds;
        }

        #endregion

        #region Audit

        /// <summary>
        /// Logs a single Audited event
        /// </summary>
        /// <param name="repositoryConnectionString"></param>
        /// <param name="auditableEntity"></param>
        /// <param name="actionId"></param>
        public static void LogAuditEvent(string repositoryConnectionString, AuditableEntity auditableEntity, short actionId)
        {
            using (LOG.InfoCall("Repository Helper - LogAuditEvent"))
            {
                if (repositoryConnectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    int result = SqlHelper.ExecuteNonQuery(connection, AddAuditableEventStoredProcedure,
                                                                    actionId,
                                                                    auditableEntity.TimeStamp,
                                                                    auditableEntity.Workstation,
                                                                    auditableEntity.WorkstationUser,
                                                                    auditableEntity.SqlUser,
                                                                    auditableEntity.Name,
                                                                    auditableEntity.MetaData,
                                                                    auditableEntity.Header);
                    LOG.WarnFormat("The Audited Action was persisted. {0}", auditableEntity.ToString());

                    if (result < 1)
                    {
                        LOG.WarnFormat("The Audited Action was not persisted. {0}", auditableEntity.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves all Audited Events from the repository
        /// </summary>
        /// <param name="repositoryConnectionString"></param>
        /// <returns></returns>
        public static DataTable GetAuditEvents(string repositoryConnectionString)
        {
            using (LOG.InfoCall("Repository Helper - GetAuditEvents"))
            {
                if (repositoryConnectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }

                DataTable table = new DataTable("AuditLog");

                table.Columns.Add("AuditableEventID", typeof(long));
                table.Columns.Add("ActionID", typeof(int));
                table.Columns.Add("Action", typeof(string));
                table.Columns.Add("Date", typeof(DateTime));
                table.Columns.Add("Workstation", typeof(string));
                table.Columns.Add("Workstation User", typeof(string));
                table.Columns.Add("SQL User", typeof(string));
                table.Columns.Add("Name", typeof(string));
                table.Columns.Add("Change Description", typeof(string));

                using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
                {
                    connection.Open();

                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetAuditableEventsStoredProcedure))
                    {
                        DateTime dateTime;
                        while (reader.Read())
                        {
                            DataRow row = table.NewRow();
                            row["AuditableEventID"] = reader.GetInt64(0);
                            row["ActionID"] = reader.GetInt32(1);
                            row["Action"] = reader.GetString(2);
                            dateTime = reader.GetDateTime(3);
                            row["Date"] = dateTime.ToLocalTime();
                            row["Workstation"] = reader.GetString(4);
                            row["Workstation User"] = reader.GetString(5);
                            row["SQL User"] = reader.GetString(6);
                            row["Name"] = reader.GetString(7);
                            row["Change Description"] = reader.GetString(8);
                            table.Rows.Add(row);
                        }
                        //row["Workstation"] = GetNullableValue(reader, 3);
                    }
                }

                return table;
            }
        }

        public static Dictionary<int, string> GetAuditHeaderTemplates(string repositoryConnectionString)
        {
            using (LOG.InfoCall("Repository Helper - GetAuditHeaderTemplates"))
            {
                var result = new Dictionary<int, string>();

                using (var connection = new SqlConnection(repositoryConnectionString))
                {
                    string query = "select [ActionID], [HeaderTemplate] from AuditableActions order by ActionID asc";

                    SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, CommandType.Text, query);

                    while (dataReader.Read())
                    {
                        result.Add(dataReader.GetInt32(0), dataReader.GetString(1));
                    }
                }

                return result;
            }
        }

        #endregion

        // START : SQLdm 9.0 (Abhishek Joshi) -CWF Integration -Helper methods to update and get the products web framework registration information
        #region Web Framework

        // SQLdm 9.0 (Abhishek Joshi) -CWF Integration -update the products web framework registration information
        public static void UpdateTheRegistrationInformation(string client, string port, string username, string password, int productId, string instance) //product might not be there at the time of update
        {
            try
            {
                var connectionString = ManagementServiceConfiguration.ConnectionString;

                if (String.IsNullOrEmpty(connectionString))
                    throw new ArgumentNullException("ManagementServiceConfiguration.ConnectionString");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlParameter[] parameters = SqlHelperParameterCache.GetSpParameterSet(connection, AddTheProductRegistrationInformation);
                    parameters[0].Value = client;
                    parameters[1].Value = port;
                    parameters[2].Value = username;
                    parameters[3].Value = password;
                    parameters[4].Value = instance;
                    parameters[5].Value = productId;

                    using (SqlCommand command = new SqlCommand(AddTheProductRegistrationInformation, connection))
                    {
                        command.CommandTimeout = SqlHelper.CommandTimeout;
                        command.Parameters.AddRange(parameters);
                        command.CommandType = CommandType.StoredProcedure;
                        int affectedRowCount = command.ExecuteNonQuery();
                        if (affectedRowCount > 0)
                        {
                            LOG.InfoFormat(
                                "Web Framework registration information updated: HostName={0}, Port={1}, UserName={2}, Password={3}, ProductId={4}",
                                client, port, username, password, productId);
                        }
                        else
                        {
                            LOG.InfoFormat(
                                "Failed to update the Web Framework registration information: HostName={0}, Port={1}, UserName={2}, Password={3}, ProductId={4}",
                                client, port, username, password, productId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occured in UpdateTheRegistrationInformation.", ex);
            }

        }

        // SQLdm 9.0 (Abhishek Joshi) -CWF Integration -get the products web framework registration information
        public static Dictionary<string, string> GetTheRegistrationInformation()
        {
            Dictionary<string, string> frameworkDetails = new Dictionary<string, string>();
            try
            {
                var connectionString = ManagementServiceConfiguration.ConnectionString;

                if (connectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetTheProductRegistrationInformation))
                    {
                        while (dataReader.Read())
                        {
                            int ordinal = dataReader.GetOrdinal("ProductID");
                            if (!(dataReader.IsDBNull(ordinal)))
                            {
                                frameworkDetails["ProductID"] = (dataReader.GetInt32(ordinal)).ToString();
                                frameworkDetails["HostName"] = dataReader["HostName"] == null ? null : dataReader["HostName"].ToString();
                                frameworkDetails["Port"] = dataReader["Port"] == null ? null : dataReader["Port"].ToString();
                                frameworkDetails["UserName"] = dataReader["UserName"] == null ? null : dataReader["UserName"].ToString();
                                frameworkDetails["Password"] = dataReader["Password"] == null ? null : dataReader["Password"].ToString();
                                frameworkDetails["InstanceName"] = dataReader["InstanceName"] == null ? null : dataReader["InstanceName"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error Occured in GetTheRegistrationInformation:", ex);
            }

            return frameworkDetails;
        }

        #endregion
        // END : SQLdm 9.0 (Abhishek Joshi) -CWF Integration -Helper methods to update and get the products web framework registration information

        #region Application Security
        //[START] - SQLdm 9.0 (Ankit Srivastava) - CWF Integration - new method for getting all the users in accoradance to the CWF sync
        public static IList<User> GetSQLServerUsers(string connectionString, bool OnlySystemAdmins)
        {
            IList<User> users = new List<User>();
            int isUserAdminOrdinal = -1;
            int isLinkedServerFoundOrdinal = -1;
            try
            {
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, "p_GetSQLServerUsers", OnlySystemAdmins))
                {

                    while (reader.Read())
                    {
                        User user = new User();
                        user.Account = reader.GetString(0);
                        SqlBinary loginSID = reader.GetSqlBinary(1);
                        if (loginSID.IsNull == false)
                        {
                            SecurityIdentifier si = new SecurityIdentifier(loginSID.Value, 0);
                            user.SID = si.Value;
                        }
                        user.UserType = reader.GetString(2);
                        isUserAdminOrdinal = reader.GetOrdinal("IsAdmin");
                        if (!reader.IsDBNull(isUserAdminOrdinal)) { user.IsAdmin = Convert.ToBoolean(reader.GetValue(isUserAdminOrdinal)); }
                        else user.IsAdmin = false;

                        isLinkedServerFoundOrdinal = reader.GetOrdinal("ServerName");
                        if (!reader.IsDBNull(isLinkedServerFoundOrdinal)) { user.LinkedInstances = Convert.ToString(reader.GetValue(isLinkedServerFoundOrdinal)).Split(',').ToList(); }
                        else user.LinkedInstances = null;
                        users.Add(user);

                    }

                }
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occured in GetSQLServerUsers", ex);
            }

            return users;
        }
        //[END] - SQLdm 9.0 (Ankit Srivastava) - CWF Integration - new method for getting all the users in accoradance to the CWF sync
        #endregion

        #region Prescriptive Analysis

        /// <summary>
        /// SQLdm 10.0 Srishti Purohit Get all data from master recommendations of populate object of MasterRecommendations class
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<MasterRecommendation> GetMasterRecommendations(string connectionString)
        {
            MasterRecommendation singleRecommendation = null;
            List<MasterRecommendation> masterData = new List<MasterRecommendation>();
            try
            {
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, GetMasterRecommendationsStoreProcedure))
                {
                    int toHandleIntParsing = 0;
                    double toHandleDoubleParsing = 0;
                    #region Get  Master Values

                    while (reader.Read())
                    {
                        string recommendationID = reader.GetString(0);

                        singleRecommendation = new MasterRecommendation(recommendationID);
                        if (singleRecommendation != null)
                        {
                            singleRecommendation.Additional_Considerations = reader["AdditionalConsiderations"].ToString();
                            singleRecommendation.Bitly = reader["bitly"].ToString();
                            singleRecommendation.Category = reader["Category"].ToString();
                            if (!int.TryParse(reader["ConfidenceFactor"].ToString(), out toHandleIntParsing)) toHandleIntParsing = 0;
                            singleRecommendation.Confidence_Factor = toHandleIntParsing;
                            singleRecommendation.Description = reader["Description"].ToString();
                            singleRecommendation.Finding = reader["Finding"].ToString();
                            singleRecommendation.Impact_Explanation = reader["ImpactExplanation"].ToString();
                            if (!int.TryParse(reader["ImpactFactor"].ToString(), out toHandleIntParsing)) toHandleIntParsing = 0;
                            singleRecommendation.Impact_Factor = toHandleIntParsing;
                            singleRecommendation.InfoLinks = reader["InfoLinks"].ToString();

                            singleRecommendation.Plural_Form_Finding = reader["PluralFormFinding"].ToString();
                            singleRecommendation.Plural_Form_Impact_Explanation = reader["PluralFormImpactExplanation"].ToString();
                            singleRecommendation.Plural_Form_Recommendation = reader["PluralFormRecommendation"].ToString();

                            singleRecommendation.Problem_Explanation = reader["ProblemExplanation"].ToString();
                            singleRecommendation.Recommendation = reader["Recommendation"].ToString();
                            if (!double.TryParse(reader["Relevance"].ToString(), out toHandleDoubleParsing)) toHandleDoubleParsing = 0;
                            singleRecommendation.Relevance = toHandleDoubleParsing;
                            singleRecommendation.Tags = reader["Tags"].ToString().Split(',');
                            masterData.Add(singleRecommendation);
                        }
                    }
                    #endregion

                }
            }
            catch (SqlException ex)
            {
                LOG.Error(ex.Message + " Error occured in GetMasterRecommendations");
                throw;
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occured in GetMasterRecommendations", ex);
                throw;
            }

            return masterData;
        }

        /// <summary>
        /// SQLdm 10.0 Srishti Purohit Doctor Analysis implementation in DM
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="instanceID"></param>
        /// <param name="filterTime"></param>
        /// <returns></returns>
        public static Result GetRecommendations(string connectionString, int instanceID, DateTime filterTime)
        {
            Result listOfRecomm = null;
            AnalyzerResult analyszerResult;
            Recommendation recommendationRow = null;
            try
            {
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, GetRecommendationsFromDBStoreProcedure, instanceID, filterTime.ToUniversalTime()))
                {
                    #region Get Properties
                    int recommCount = 0;
                    int analyzerCount = 0;
                    List<Triple<int, string, string>> recommendationPropertiesWithRecommIDs = new List<Triple<int, string, string>>();
                    Triple<int, string, string> property;
                    // populate these recommendationProperties from DB
                    LOG.Info("Getting all saved recommendations from database.");

                    while (reader.Read())
                    {
                        property = new Triple<int, string, string>();

                        property.First = reader.GetInt32(0);
                        if (property.First > 0)
                        {
                            property.Second = reader.GetString(1);
                            property.Third = reader.GetString(2);
                            recommendationPropertiesWithRecommIDs.Add(property);
                        }
                    }
                    #endregion
                    reader.NextResult();
                    Dictionary<string, string> recommendationProperties;
                    while (reader.Read())
                    {
                        string recommendationID = reader["RecommendationID"].ToString();
                        recommendationProperties = new Dictionary<string, string>();
                        foreach (Triple<int, string, string> propertyItem in recommendationPropertiesWithRecommIDs)
                        {
                            if (propertyItem.First == Convert.ToInt32(reader["AnalysisRecommendationID"]))
                            {
                                var value = System.Web.HttpUtility.HtmlDecode(propertyItem.Second);
                                if (!recommendationProperties.ContainsKey(value))
                                    recommendationProperties.Add(value, propertyItem.Third);
                            }
                        }
                        try
                        {
                            recommendationRow = RecommendationFactory.GetRecommendation(recommendationID, recommendationProperties);
                        }
                        catch (Exception ex)
                        {
                            LOG.Error(" Error occured in GetMasterRecommendations", ex);
                        }

                        if (recommendationRow != null)
                        {
                            recommendationRow.ID = reader["RecommendationID"].ToString();
                            //recommendationRow.AdditionalConsiderations = reader.GetString(1);
                            //recommendationRow.Category = reader.GetString(3);
                            //recommendationRow.ConfidenceFactor = reader.GetInt32(4);
                            //recommendationRow.FindingText = reader.GetString(6);
                            //recommendationRow.ImpactExplanationText = reader.GetString(7);
                            //recommendationRow.ImpactFactor = Convert.ToInt32( reader.GetString(8));
                            //recommendationRow.Links = reader.GetString(9);
                            //recommendationRow.ProblemExplanationText = reader.GetString(13);
                            //recommendationRow.RecommendationText = reader.GetString(14);
                            //recommendationRow.Relevance = Convert.ToDouble( reader.GetString(15));
                            //recommendationRow.Tags = reader.GetString(16).Split(',');
                            recommendationRow.ComputedRankFactor = Convert.ToSingle(reader["ComputedRankFactor"]);
                            recommendationRow.AnalysisRecommendationID = Convert.ToInt32(reader["AnalysisRecommendationID"]);
                            recommendationRow.IsFlagged = Convert.ToBoolean(reader["IsFlagged"]);
                            recommendationRow.OptimizationStatus = (RecommendationOptimizationStatus)Convert.ToInt32(reader["OptimizationStatusID"]);
                            recommendationRow.OptimizationErrorMessage = reader["OptimizationErrorMessage"].ToString();

                            if (listOfRecomm != null)
                            {
                                AnalyzerResult checkIfAnalyzerExists = listOfRecomm.AnalyzerRecommendationList.Find(item => item.AnalyzerID == Convert.ToInt32(reader["AnalyzerID"]));

                                if (checkIfAnalyzerExists != null)
                                {
                                    checkIfAnalyzerExists.RecommendationList.Add(recommendationRow);
                                    recommCount++;
                                }
                                else
                                {
                                    analyszerResult = new AnalyzerResult();
                                    analyszerResult.AnalyzerID = Convert.ToInt32(reader["AnalyzerID"]);
                                    analyszerResult.Status = Convert.ToInt32(reader["Status"]);
                                    analyszerResult.RecommendationList.Add(recommendationRow);
                                    listOfRecomm.AnalyzerRecommendationList.Add(analyszerResult);
                                    analyzerCount++;
                                    recommCount++;
                                }
                            }
                            else if (listOfRecomm == null)
                            {
                                listOfRecomm = new Result();
                                listOfRecomm.SQLServerID = instanceID;
                                listOfRecomm.AnalyzerRecommendationList = new List<AnalyzerResult>();
                                analyszerResult = new AnalyzerResult();
                                analyszerResult.AnalyzerID = Convert.ToInt32(reader["AnalyzerID"]);
                                analyszerResult.Status = Convert.ToInt32(reader["Status"]);
                                analyszerResult.RecommendationList.Add(recommendationRow);
                                listOfRecomm.AnalyzerRecommendationList.Add(analyszerResult);
                                analyzerCount++;
                                recommCount++;
                            }
                        }
                        if (listOfRecomm != null)
                            listOfRecomm.TotalRecommendationCount = recommCount;
                    }

                    LOG.Info("Total Recommendations : " + recommCount + " for total Anlyzer : " + analyzerCount + " fetched from DB for server ID : " + instanceID + " and filter time : " + filterTime + " .");
                }
            }
            catch (SqlException ex)
            {
                LOG.Error(ex.Message + "SQL Error occured in GetRecommendations.");
                throw;
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occured in GetRecommendations : ", ex);
                throw;
            }

            return listOfRecomm;
        }

        public static AnalysisListCollection GetAnalysisListing(string connectionString, int instanceID)
        {
            AnalysisListCollection listOfRecomm = new AnalysisListCollection();
            List<AnalysisList> AnalysisListColl = new List<AnalysisList>();

            try
            {
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, GetAnalysisListingFromDBStoreProcedure, instanceID))
                {
                    while (reader.Read())
                    {
                        AnalysisList anlsList = new AnalysisList();
                        anlsList.SQLServerID = Convert.ToInt32(reader["SQLServerID"]);
                        anlsList.AnalysisID = Convert.ToInt32(reader["AnalysisID"]);
                        //anlsList.AnalysisStartTime = Convert.ToDateTime(reader["Started"]);
                        //anlsList.AnalysisCompleteTime = Convert.ToDateTime(reader["UTCAnalysisCompleteTime"]);
                        anlsList.AnalysisStartTime = Convert.ToDateTime(reader["Started"]).ToLocalTime();
                        anlsList.AnalysisCompleteTime = Convert.ToDateTime(reader["UTCAnalysisCompleteTime"]).ToLocalTime();
                        anlsList.ComputedRankFactor = Convert.ToSingle(reader["Priority"]);
                        anlsList.TotalRecommendationCount = Convert.ToInt32(reader["Recommendations"]);
                        anlsList.Type = reader["TaskType"].ToString();
                        anlsList.AnalysisDuration = reader["Duration"].ToString();
                        AnalysisListColl.Add(anlsList);
                    }
                    listOfRecomm.AnalysisListColl = AnalysisListColl;
                }
                return listOfRecomm;
            }
            catch (SqlException ex)
            {
                LOG.Error(ex.Message + "SQL Error occured in GetRecommendations.");
                throw;
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occured in GetRecommendations : ", ex);
                throw;
            }
        }

        ///// <summary>
        ///// SQLdm 10.0 Srishti Purohit Doctor Analysis implementation in DM
        ///// Author: srishti purohit
        ///// Product Version: SQLdm 10.0
        ///// </summary>
        //public static int SaveRecommendations(string connectionString, Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result analysisResultList, int sqlServerID)
        //{
        //    int analysisID = 0;
        //    bool isRecordsAvailableToSave = false;
        //    SqlConnection connection = new SqlConnection(connectionString);
        //    try
        //    {
        //        // Create xml from list of recommendation                

        //        XmlDocument doc = new XmlDocument();
        //        doc.LoadXml("<ListOfAnlysis ></ListOfAnlysis>");
        //        if (analysisResultList.AnalyzerRecommendationList.Count > 0)
        //        {
        //            isRecordsAvailableToSave = true;
        //        }
        //        else
        //            isRecordsAvailableToSave = false;
        //        foreach (AnalyzerResult anaResult in analysisResultList.AnalyzerRecommendationList)
        //        {
        //            XmlNode root = doc.DocumentElement;
        //            //Create a new node.
        //            XmlElement elem = doc.CreateElement("Analysis");
        //            XmlAttribute att = doc.CreateAttribute("AnalyzerID");
        //            att.InnerXml = ((int)anaResult.AnalyzerID).ToString(); ;
        //            elem.Attributes.Append(att);
        //            att = doc.CreateAttribute("status");
        //            att.InnerXml = ((int)anaResult.Status).ToString();
        //            elem.Attributes.Append(att);
        //            att = doc.CreateAttribute("recommCount");
        //            att.InnerXml = ((int)anaResult.RecommendationCount).ToString();
        //            elem.Attributes.Append(att);
        //            foreach (Recommendation recomm in anaResult.RecommendationList)
        //            {
        //                // for recommendation inner xml 
        //                //Create a new node.
        //                //XmlDocument innerXML = new XmlDocument();

        //                XmlNode rootInner = doc.DocumentElement;

        //                XmlElement elemInner = doc.CreateElement("Recomm");
        //                XmlAttribute attInner = doc.CreateAttribute("RecommID");
        //                attInner.InnerXml = recomm.ID;
        //                elemInner.Attributes.Append(attInner);
        //                attInner = doc.CreateAttribute("ComputedRankFactor");
        //                attInner.InnerXml = recomm.ComputedRankFactor.ToString();
        //                elemInner.Attributes.Append(attInner);
        //                //attInner = doc.CreateAttribute("Relevance");
        //                //attInner.InnerXml = recomm.Relevance.ToString();
        //                //elemInner.Attributes.Append(attInner);

        //                //attInner = doc.CreateAttribute("Description");
        //                //attInner.InnerXml = recomm.ProblemExplanationText.ToString();
        //                //elemInner.Attributes.Append(attInner);
        //                //attInner = doc.CreateAttribute("Finding");
        //                //attInner.InnerXml = recomm.FindingText.ToString();
        //                //elemInner.Attributes.Append(attInner);

        //                //attInner = doc.CreateAttribute("Impact");
        //                //attInner.InnerXml = recomm.ImpactExplanationText.ToString();
        //                //elemInner.Attributes.Append(attInner);
        //                //attInner = doc.CreateAttribute("ProblemExplanation");
        //                //attInner.InnerXml = recomm.Relevance.ToString();
        //                //elemInner.Attributes.Append(attInner);

        //                //attInner = doc.CreateAttribute("Recommendation");
        //                //attInner.InnerXml = recomm.RecommendationText.ToString();
        //                //elemInner.Attributes.Append(attInner);
        //                attInner = doc.CreateAttribute("isFlagged");
        //                attInner.InnerXml = recomm.IsScriptGeneratorProvider.ToString();
        //                elemInner.Attributes.Append(attInner);

        //                //Add the node to the document.


        //                var properties = recomm.GetProperties();
        //                XmlNode propertiesNode = doc.CreateElement("Properties");
        //                foreach (var prop in properties)
        //                {
        //                    XmlNode propNode = doc.CreateElement("Property");
        //                    XmlAttribute attName = doc.CreateAttribute("Name");
        //                    attName.InnerXml = prop.Key;
        //                    propNode.Attributes.Append(attName);

        //                    XmlNode valueNode = doc.CreateElement("value");
        //                    //XmlAttribute attValue = doc.CreateAttribute("Value");
        //                    valueNode.InnerXml = prop.Value;
        //                    propNode.AppendChild(valueNode);
        //                    //propNode.Attributes.Append(attValue);

        //                    propertiesNode.AppendChild(propNode);
        //                }
        //                elemInner.AppendChild(propertiesNode);
        //                elem.AppendChild(elemInner);
        //                root.AppendChild(elem);
        //            }
        //        }

        //        Console.WriteLine("Display the modified XML...");
        //        //doc.Save(Console.Out);
        //        if (isRecordsAvailableToSave)
        //        {
        //            //DB call to save
        //            using (connection)
        //            {
        //                connection.Open();
        //                using (SqlCommand command = SqlHelper.CreateCommand(connection, SaveRecommendationsInDBStoreProcedure))
        //                {
        //                    command.Parameters.Clear();

        //                    command.Parameters.AddWithValue("@sqlServerID", sqlServerID);
        //                    command.Parameters.AddWithValue("@analysisStartTime", analysisResultList.AnalysisStartTime);
        //                    command.Parameters.AddWithValue("@analysisCompleteTime", analysisResultList.AnalysisCompleteTime);
        //                    command.Parameters.AddWithValue("@recommendationCount", analysisResultList.TotalRecommendationCount);
        //                    command.Parameters.AddWithValue("@listOfRecommendations", doc.InnerXml);
        //                    command.Parameters.Add("@analysisID", SqlDbType.Int);
        //                    command.Parameters["@analysisID"].Direction = ParameterDirection.Output;
        //                    command.Parameters.Add("@prescriptiveAnalysisDetailsID", SqlDbType.Int);

        //                    command.Parameters["@prescriptiveAnalysisDetailsID"].Direction = ParameterDirection.Output;
        //                    command.Parameters.Add("@prescriptiveAnalysisRecommendationID", SqlDbType.Int);

        //                    command.Parameters["@prescriptiveAnalysisRecommendationID"].Direction = ParameterDirection.Output;
        //                    command.ExecuteNonQuery();
        //                    analysisID = Convert.ToInt32(command.Parameters["@analysisID"].Value);
        //                }
        //            }
        //        }
        //        else
        //            analysisID = -1;
        //    }
        //    catch (Exception ex)
        //    {
        //        LOG.ErrorFormat("Error occured in SaveRecommendations", ex);
        //        throw new Exception(ex.Message);
        //    }
        //    finally
        //    {
        //        connection.Close();
        //    }

        //    return analysisID;
        //}

        /// <summary>
        /// SQLdm 10.0 Srishti Purohit Doctor Analysis implementation in DM
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// To save analysis records
        /// </summary>
        public static int SaveAnalysisResults(string connectionString, Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result analysisResultList, int sqlServerID, bool isRunAnalysis = false, int tempAnalysisID = 0)
        {
            int analysisID = 0;
            bool isRecordsAvailableToSave = false;
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                LOG.Info("Saving Analysis results in all respective tables database for server ID : " + sqlServerID);

                //DB call to save only analysis
                using (connection)
                {
                    if (!isRunAnalysis)
                    {
                        connection.Open();
                        using (SqlCommand command = SqlHelper.CreateCommand(connection, SavePrescriptiveAnalysisStoreProcedure))
                        {
                            command.Parameters.Clear();

                            command.Parameters.AddWithValue("@sqlServerID", sqlServerID);
                            command.Parameters.AddWithValue("@analysisStartTime", analysisResultList.AnalysisStartTime.ToUniversalTime());
                            command.Parameters.AddWithValue("@analysisCompleteTime", analysisResultList.AnalysisCompleteTime.ToUniversalTime());
                            command.Parameters.AddWithValue("@recommendationCount", analysisResultList.TotalRecommendationCount);
                            command.Parameters.AddWithValue("@analysisTypeId", Convert.ToInt32(analysisResultList.Type));
                            command.Parameters.Add("@analysisID", SqlDbType.Int);
                            command.Parameters["@analysisID"].Direction = ParameterDirection.Output;

                            //analysisID = Convert.ToInt32( SqlHelper.ExecuteScalar(connection, SavePrescriptiveAnalysisStoreProcedure, command.Parameters));
                            command.ExecuteNonQuery();
                            analysisID = Convert.ToInt32(command.Parameters["@analysisID"].Value);
                        }
                    }
                    if (isRunAnalysis)
                        analysisID = tempAnalysisID;
                    LOG.Info("PrescriptiveAnalysis record saved in database. Analsis ID saved is " + analysisID + " . AnalyzerRecommendationList count is " + analysisResultList.AnalyzerRecommendationList.Count + " .");

                    if (analysisID > 0 && analysisResultList.AnalyzerRecommendationList.Count > 0)
                    {
                        isRecordsAvailableToSave = true;
                    }
                    else if (analysisID > 0)
                    {
                        LOG.ErrorFormat("Records saved for PrescriptiveAnalysis while running SaveAnalysisResults. But count of Analyzer was 0. Analysis ID :", analysisID);
                        isRecordsAvailableToSave = false;
                    }
                    else
                    {
                        LOG.ErrorFormat("No records saved for PrescriptiveAnalysis while running SaveAnalysisResults. Analysis ID :", analysisID);
                        isRecordsAvailableToSave = false;
                    }

                    //Start with Anlyzer details to save in PrescriptiveAnalysisDetails
                    if (isRecordsAvailableToSave)
                    {
                        
                        LOG.Info("Saving Recommedations for each anlyzer in DB.");

                        int prescriptiveAnalysisDetailsID = 0;

                        var analysisFilteredResults =
                            analysisResultList.AnalyzerRecommendationList.Where(a => a.AnalyzerID != 0);

                        // Populate the recommendations for platform
                        List<string> findRecommendationsOfPlatFom = null;
                        foreach (AnalyzerResult anaResult in analysisFilteredResults)
                        {
                            if (findRecommendationsOfPlatFom == null)
                            {
                                findRecommendationsOfPlatFom = GetRecommendationListForTargetPlatform
                                                               (ManagementServiceConfiguration.ConnectionString,
                                                                   sqlServerID) ?? new List<string>();
                            }

                            using (SqlCommand commandAnalyzer = SqlHelper.CreateCommand(connection, SavePrescriptiveAnalysisDetailsStoreProcedure))
                            {
                                commandAnalyzer.Parameters.Clear();

                                commandAnalyzer.Parameters.AddWithValue("@analysisID", analysisID);
                                commandAnalyzer.Parameters.AddWithValue("@analyzerID", anaResult.AnalyzerID);
                                commandAnalyzer.Parameters.AddWithValue("@status", anaResult.Status);
                                commandAnalyzer.Parameters.AddWithValue("@recommendationCount", anaResult.RecommendationCount);
                                commandAnalyzer.Parameters.Add("@prescriptiveAnalysisDetailsID", SqlDbType.Int);
                                commandAnalyzer.Parameters["@prescriptiveAnalysisDetailsID"].Direction = ParameterDirection.Output;

                                if (connection.State == ConnectionState.Closed)
                                    connection.Open();
                                commandAnalyzer.ExecuteNonQuery();
                                prescriptiveAnalysisDetailsID = Convert.ToInt32(commandAnalyzer.Parameters["@prescriptiveAnalysisDetailsID"].Value);
                            }
                            LOG.Info("PrescriptiveAnalysisDetails record saved in database. Analsis Details ID saved is " + prescriptiveAnalysisDetailsID + " . RecommendationList count for this analyzer is " + anaResult.RecommendationList.Count + " .");

                            if (prescriptiveAnalysisDetailsID > 0 && anaResult.RecommendationList.Count > 0)
                            {
                                isRecordsAvailableToSave = true;
                            }
                            else if (prescriptiveAnalysisDetailsID > 0)
                            {
                                LOG.ErrorFormat("Records saved for PrescriptiveAnalysis while running SaveAnalysisResults. But count of Recommendation was 0. AnalysisDetails ID :", analysisID);
                                isRecordsAvailableToSave = false;
                            }
                            else
                            {
                                LOG.ErrorFormat("No records saved for PrescriptiveAnalysis while running SaveAnalysisResults. AnalysisDetails ID :", analysisID);
                                isRecordsAvailableToSave = false;
                            }
                            if (isRecordsAvailableToSave)
                            {
                                // Create xml from list of recommendation                
                                XmlDocument doc = new XmlDocument();
                                doc.LoadXml("<ListOfRecommendation ></ListOfRecommendation>");
                                // for recommendation inner xml 
                                //Create a new node.
                                //XmlDocument innerXML = new XmlDocument();

                                XmlNode rootInner = doc.DocumentElement;

                                foreach (IRecommendation recommendation in anaResult.RecommendationList)
                                {
                                    if (!findRecommendationsOfPlatFom.Contains(recommendation.ID))
                                    {
                                        continue;
                                    }
                                    try
                                    {
                                        XmlElement elemInner = doc.CreateElement("Recomm");
                                        XmlAttribute attInner = doc.CreateAttribute("RecommID");
                                        attInner.InnerXml = recommendation.ID;
                                        elemInner.Attributes.Append(attInner);
                                        attInner = doc.CreateAttribute("ComputedRankFactor");
                                        attInner.InnerXml = recommendation.ComputedRankFactor.ToString();
                                        elemInner.Attributes.Append(attInner);

                                        attInner = doc.CreateAttribute("isFlagged");
                                        attInner.InnerXml = recommendation.IsFlagged.ToString();
                                        elemInner.Attributes.Append(attInner);

                                        attInner = doc.CreateAttribute("OptimizationStatus");
                                        attInner.InnerXml = ((int)recommendation.OptimizationStatus).ToString();
                                        elemInner.Attributes.Append(attInner);

                                        attInner = doc.CreateAttribute("OptimizationError");
                                        attInner.InnerXml = string.IsNullOrEmpty(recommendation.OptimizationErrorMessage) ? "" : recommendation.OptimizationErrorMessage.ToString();
                                        elemInner.Attributes.Append(attInner);

                                        //Add the node to the document.

                                        var properties = recommendation.GetProperties();
                                        XmlNode propertiesNode = doc.CreateElement("Properties");
                                        foreach (var prop in properties)
                                        {
                                            XmlNode propNode = doc.CreateElement("Property");
                                            XmlAttribute attName = doc.CreateAttribute("Name");
                                            attName.InnerXml = prop.Key;
                                            propNode.Attributes.Append(attName);
                                            LOG.Info(prop.Key + " : " + prop.Value);
                                            XmlNode valueNode = doc.CreateElement("value");
                                            valueNode.InnerXml = System.Web.HttpUtility.HtmlEncode(prop.Value);
                                            propNode.AppendChild(valueNode);

                                            propertiesNode.AppendChild(propNode);
                                        }
                                        elemInner.AppendChild(propertiesNode);
                                        rootInner.AppendChild(elemInner);
                                    }
                                    catch (Exception ex)
                                    {
                                        LOG.ErrorFormat("Error occured in saving a recommendation, continuing with other reommendations : ", ex);
                                    }
                                }
                                // DB call to save xml of recommendation
                                using (SqlCommand commandRecomm = SqlHelper.CreateCommand(connection, SaveAnalysisRecommendationAndPropertiesStoreProcedure))
                                {
                                    commandRecomm.Parameters.Clear();
                                    commandRecomm.Parameters.AddWithValue("@prescriptiveAnalysisDetailsID", prescriptiveAnalysisDetailsID);
                                    commandRecomm.Parameters.AddWithValue("@listOfRecommendations", doc.InnerXml);

                                    if (connection.State == ConnectionState.Closed)
                                        connection.Open();
                                    commandRecomm.ExecuteNonQuery();

                                }
                            }
                        }
                    }
                }

                LOG.Info("Records of anlysis is successfully saved in DB.");
            }
            catch (SqlException ex)
            {
                LOG.ErrorFormat("SQL Error occured in SaveAnalysisResults : ", ex);
                throw;
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occured in SaveAnalysisResults : ", ex);
                throw;
            }
            finally
            {
                connection.Close();
            }
            return analysisID;
        }

        /// <summary>
        /// SQLdm 10.0 Srishti Purohit Doctor Analysis implementation in DM
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// To save SnapshotValues for latest state
        /// </summary>
        public static void SavePrescriptiveAnalysisSnapshotValues(string connectionString, SnapshotValues currentSnapshot, int sqlServerID)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                if (currentSnapshot == null)
                {
                    LOG.Info("No SnapshotValues found to save for server ID : " + sqlServerID);
                    return;
                }
                LOG.Info("Saving SnapshotValues current value in respective table database for server ID : " + sqlServerID);

                //DB call to save only analysis
                using (connection)
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, SavePrescriptiveSnapshotValuesForServerStoreProcedure))
                    {
                        command.Parameters.Clear();

                        command.Parameters.AddWithValue("@MonitoredServerID", sqlServerID);
                        command.Parameters.AddWithValue("@ActiveNetworkCards", currentSnapshot.ActiveNetworkCards);
                        command.Parameters.AddWithValue("@TotalNetworkBandwidth", Convert.ToInt64(currentSnapshot.TotalNetworkBandwidth));
                        command.Parameters.AddWithValue("@AllowedProcessorCount", currentSnapshot.AllowedProcessorCount);
                        command.Parameters.AddWithValue("@TotalNumberOfLogicalProcessors", Convert.ToInt32(currentSnapshot.TotalNumberOfLogicalProcessors));
                        command.Parameters.AddWithValue("@TotalMaxClockSpeed", Convert.ToInt64(currentSnapshot.TotalMaxClockSpeed));
                        command.Parameters.AddWithValue("@TotalPhysicalMemory", Convert.ToInt64(currentSnapshot.TotalPhysicalMemory));
                        command.Parameters.AddWithValue("@MaxServerMemory", Convert.ToInt64(currentSnapshot.MaxServerMemory));
                        command.Parameters.AddWithValue("@WindowsVersion", currentSnapshot.WindowsVersion == null ? "" : currentSnapshot.WindowsVersion);
                        command.Parameters.AddWithValue("@ProductVersion", currentSnapshot.ProductVersion == null ? "" : currentSnapshot.ProductVersion);
                        command.Parameters.AddWithValue("@SQLVersionString", currentSnapshot.SQLVersionString == null ? "" : currentSnapshot.SQLVersionString);

                        //analysisID = Convert.ToInt32( SqlHelper.ExecuteScalar(connection, SavePrescriptiveAnalysisStoreProcedure, command.Parameters));
                        command.ExecuteNonQuery();

                        LOG.Info("PrescriptiveAnalysis snapshot value saved in database for server {0}. ", sqlServerID);

                    }
                }
            }
            catch (SqlException ex)
            {
                LOG.ErrorFormat("SQL Error occured in SavePrescriptiveAnalysisSnapshotValues : ", ex);
                throw;
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occured in SavePrescriptiveAnalysisSnapshotValues : ", ex);
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        /// <summary>
        /// SQLdm 10.0 Praveen Suhalka Analysis implementation in DM
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.PAScheduledTask> GetPrescriptiveAnalysisScheduleList(string connectionString)
        {
            List<Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.PAScheduledTask> list = null;
            Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.PAScheduledTask task = null;
            try
            {
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, GetPrescriptiveAnalysisSchedule))
                {
                    list = new List<PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.PAScheduledTask>();
                    while (reader.Read())
                    {
                        task = new PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.PAScheduledTask();
                        task.ServerID = (int)reader["MonitoredServerID"];
                        task.SelectedDays = (short)reader["ScheduledDays"];
                        if (reader["StartTime"] != DBNull.Value)
                        {
                            task.StartTime = (DateTime)reader["StartTime"];
                        }
                        list.Add(task);
                    }
                }

                return list;
            }
            catch (SqlException ex)
            {
                LOG.Error(ex.Message + " Error occured in GetPrescriptiveAnalysisSchedule");
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occured in GetPrescriptiveAnalysisSchedule", ex);
                throw new Exception(ex.Message);
            }
        }

        #endregion

        public static AWSAccountProp GetAWSResourcePrincipleDetails(string repositoryConnectionString, string instanceName)
        {
            AWSAccountProp prop = new AWSAccountProp();

            using (
                SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(repositoryConnectionString, GetAWSResourceDetailsStoredProcedure, instanceName))
            {
                while (dataReader.Read())
                {
                    prop.aws_access_key = Convert.ToString(dataReader["aws_access_key"], CultureInfo.InvariantCulture);
                    prop.aws_secret_key = Convert.ToString(dataReader["aws_secret_key"], CultureInfo.InvariantCulture);
                    prop.aws_region_endpoint = Convert.ToString(dataReader["aws_region_endpoint"], CultureInfo.InvariantCulture);
                }
            }

            return prop;
        }

        static void Log_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            string message = e.Message;
            if (String.IsNullOrEmpty(message))
                message = "Sql server message";
            if (e.Errors != null && e.Errors.Count > 0)
            {
                foreach (SqlError error in e.Errors)
                {
                    LOG.DebugFormat("{0}", error);
                }
            }
            else
                LOG.Debug(message);
        }
        public static Dictionary<int, string> GetBlockedCategoryListForTargetPlatform(string connectionString, int sqlServerId)
        {
            var blockedRecommendationsForTarget = new Dictionary<int, string>();
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }

            using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString,
                Constants.GetBlockedCategoriesForPlatform,
                sqlServerId))
            {
                if (!reader.HasRows) {
                    return blockedRecommendationsForTarget;
                }
                
                var categoryIdOrdinal = reader.GetOrdinal("CategoryID");
                var categoryNameOrdinal = reader.GetOrdinal("Name");
                while (reader.Read())
                {
                    var categoryId = reader.GetInt32(categoryIdOrdinal);
                    var categoryName = reader.GetString(categoryNameOrdinal);
                    if (!blockedRecommendationsForTarget.ContainsKey(categoryId))
                    {
                        blockedRecommendationsForTarget.Add(categoryId, categoryName);
                    }
                }
            }

            return blockedRecommendationsForTarget;
        }

        public static List<string> GetRecommendationListForTargetPlatform(string connectionString, int? sqlServerId = null)
        {
            List<string> recommendationsForTarget = new List<string>();
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }

            using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString,
                                               Constants.GetSpecificRecommendations,
                                               sqlServerId))
            {
                while(reader.Read())
                {
                    recommendationsForTarget.Add(Convert.ToString(reader["RecommendationId"]));
                }
            }

            return recommendationsForTarget;
        }

        public static IAzureProfile GetAzureProfile(string connectionString, long inputProfileId, string inputResourceUri)
        {
            IAzureProfile azureProfile = null;
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connectionString,
                Constants.GetAzureProfileForResourceUri,
                inputProfileId,
                inputResourceUri))
            {
                if (dataReader.HasRows)
                {
                    var idOrdinal = dataReader.GetOrdinal("ID");
                    var descriptionOrdinal = dataReader.GetOrdinal("Description");
                    var sqlServerIdOrdinal = dataReader.GetOrdinal("SQLServerID");
                    var azureApplicationProfileIdOrdinal =
                        dataReader.GetOrdinal("AzureApplicationProfileId");
                    var azureApplicationProfileNameOrdinal =
                        dataReader.GetOrdinal("AzureApplicationProfileName");
                    var AzureApplicationProfileDescriptionOrdinal =
                        dataReader.GetOrdinal("AzureApplicationProfileDescription");
                    var azureApplicationIdOrdinal = dataReader.GetOrdinal("AzureApplicationId");
                    var applicationNameOrdinal = dataReader.GetOrdinal("ApplicationName");
                    var applicationDescriptionOrdinal = dataReader.GetOrdinal("ApplicationDescription");
                    var clientIdOrdinal = dataReader.GetOrdinal("ClientId");
                    var tenantIdOrdinal = dataReader.GetOrdinal("TenantId");
                    var secretOrdinal = dataReader.GetOrdinal("Secret");
                    var azureSubscriptionIdOrdinal = dataReader.GetOrdinal("AzureSubscriptionId");
                    var subscriptionIdOrdinal = dataReader.GetOrdinal("SubscriptionId");
                    var subscriptionDescriptionOrdinal = dataReader.GetOrdinal("SubscriptionDescription");
                    var resourceIdOrdinal = dataReader.GetOrdinal("ResourceId");
                    var resourceNameOrdinal = dataReader.GetOrdinal("ResourceName");
                    var resourceTypeOrdinal = dataReader.GetOrdinal("ResourceType");
                    var resourceUriOrdinal = dataReader.GetOrdinal("ResourceUri");

                    while (dataReader.Read())
                    {
                        // Azure Profile Details
                        var id = dataReader.GetInt64(idOrdinal);
                        var sqlServerId = dataReader.GetInt32(sqlServerIdOrdinal);
                        var description = !dataReader.IsDBNull(descriptionOrdinal)
                            ? dataReader.GetString(descriptionOrdinal)
                            : null;

                        // Add a new Profile
                        azureProfile = new AzureProfile
                        {
                            Id = id,
                            Description = description,
                            SqlServerId = sqlServerId
                        };

                        // Application Profile Details
                        var applicationProfileId = dataReader.GetInt64(azureApplicationProfileIdOrdinal);

                        // Application Profile Details
                        var applicationProfileName = dataReader.GetString(azureApplicationProfileNameOrdinal);
                        var applicationProfileDescription =
                            !dataReader.IsDBNull(AzureApplicationProfileDescriptionOrdinal)
                                ? dataReader.GetString(AzureApplicationProfileDescriptionOrdinal)
                                : null;

                        // Application Details
                        var azureApplicationId = dataReader.GetInt64(azureApplicationIdOrdinal);
                        var applicationName = dataReader.GetString(applicationNameOrdinal);
                        var applicationDescription = !dataReader.IsDBNull(applicationDescriptionOrdinal)
                            ? dataReader.GetString(applicationDescriptionOrdinal)
                            : null;
                        var clientId = dataReader.GetString(clientIdOrdinal);
                        var tenantId = dataReader.GetString(tenantIdOrdinal);
                        var secret = dataReader.GetString(secretOrdinal);

                        // Subscription Details
                        var azureSubscriptionId = dataReader.GetInt64(azureSubscriptionIdOrdinal);
                        var subscriptionId = dataReader.GetString(subscriptionIdOrdinal);
                        var subscriptionDescription = !dataReader.IsDBNull(subscriptionDescriptionOrdinal)
                            ? dataReader.GetString(subscriptionDescriptionOrdinal)
                            : null;

                        var azureApplicationProfile = new AzureApplicationProfile
                        {
                            Id = applicationProfileId,
                            Name = applicationProfileName,
                            Description = applicationProfileDescription,
                            Subscription = new AzureSubscription
                            {
                                Id = azureSubscriptionId,
                                SubscriptionId = subscriptionId,
                                Description = subscriptionDescription
                            },
                            Application = new AzureApplication
                            {
                                Id = azureApplicationId,
                                Name = applicationName,
                                TenantId = tenantId,
                                ClientId = clientId,
                                EncryptedSecret = secret,
                                Description = applicationDescription
                            },
                            Resources = new List<IAzureResource>()
                        };
                        azureProfile.ApplicationProfile = azureApplicationProfile;

                        // Resource Details
                        var resourceId = !dataReader.IsDBNull(resourceIdOrdinal)
                            ? dataReader.GetInt64(resourceIdOrdinal)
                            : 0;
                        var resourceName = !dataReader.IsDBNull(resourceNameOrdinal)
                            ? dataReader.GetString(resourceNameOrdinal)
                            : null;
                        var resourceType = !dataReader.IsDBNull(resourceTypeOrdinal)
                            ? dataReader.GetString(resourceTypeOrdinal)
                            : null;
                        var resourceUri = !dataReader.IsDBNull(resourceUriOrdinal)
                            ? dataReader.GetString(resourceUriOrdinal)
                            : null;
                        var applicationProfile = azureProfile.ApplicationProfile;
                        applicationProfile.Resources.Add(
                            new AzureResource
                            {
                                Type = resourceType,
                                Name = resourceName,
                                Uri = resourceUri,
                                Profile = applicationProfile
                            });
                        break;
                    }
                }
            }

            return azureProfile;
        }
    }
}