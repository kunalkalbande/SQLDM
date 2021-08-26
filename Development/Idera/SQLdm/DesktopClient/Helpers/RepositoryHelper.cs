using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Linq;
using System.Xml.Serialization;
using BBS.TracerX;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Helpers;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.Thresholds;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using QueryMonitorViewMode = Idera.SQLdm.DesktopClient.Views.Servers.Server.Queries.QueryMonitorView.QueryMonitorViewMode;
using Infragistics.Win;
using Microsoft.ApplicationBlocks.Data;
using Microsoft.Reporting.WinForms;
using Wintellect.PowerCollections;
using Constants = Idera.SQLdm.Common.Constants;
using XmlSerializerFactory = Idera.SQLdm.Common.Data.XmlSerializerFactory;
using System.Globalization;
using Idera.SQLdm.Common.Objects.Replication;
using Idera.SQLdm.Common.Notification;
using Idera.SQLdm.Common.VMware;
using DashboardFilter = Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview.DashboardLayoutGalleryViewModel.DashboardFilter;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.DesktopClient.Presenters.GridEntries;
using System.Text;
using System.Diagnostics;
using Idera.SQLdm.Common.Events.AzureMonitor;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;
using Idera.SQLdm.Common.Services;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    internal static class RepositoryHelper
    {
        #region Constants

        private const string GetRepositoryVersionSqlCommand = "select dbo.fn_GetDatabaseVersion()";
        private const string GetRepositoryInfoStoredProcedure = "p_RepositoryInfo";
        private const string GetDefaulManagementServiceStoredProcedure = "p_GetDefaultManagementService";
        private const string GetMonitoredSqlServersByIdStoredProcedure = "p_GetMonitoredSqlServerById";
        //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Stored procedur to retrieve List of configuration
        //private const string GetBaselineTemplatesByIdStoredProcedure = "p_GetBaselineTemplatesById";
        //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Stored procedur to retrieve List of configuration
        private const string GetMonitoredSqlServersStoredProcedure = "p_GetMonitoredSqlServers";
        private const string GetMostCriticalServersStoredProcedure = "p_GetMostCriticalServers";
        private const string GetServerWideStatisticsStoredProcedure = "p_GetServerWideStatistics";
        private const string GetServerOverviewStoredProcedure = "p_GetServerOverview";
        private const string GetServerOverviewStatisticsStoredProcedure = "p_GetServerOverviewStatistics";
        private const string GetTasksStoredProcedure = "p_GetTasks";
        private const string GetMonitoredSqlServerStatusStoredProcedure = "p_GetMonitoredSqlServerStatus";
        private const string GetQueryMonitorStatementsStoredProcedure = "p_GetQueryMonitorStatements";
        private const string GetAlertTemplatesSqlCommand = "SELECT [TemplateID],[Name],[Description],[Default]  FROM dbo.AlertTemplateLookup ORDER BY [Default] DESC";
        private const string GetNewAlertTemplateOptions = "p_GetNewTemplateOptions";
        private const string AddAlertTemplateStoredProcedure = "p_AddAlertTemplate";
        private const string AddDashboardLayoutStoredProcedure = "p_AddDashboardLayout";
        private const string AddAzureResourceStoredProcedure = "p_AddAzureResource";
        private const string DeleteAzureResourceStoredProcedure = "p_DeleteAzureResource";
        private const string DeleteDashboardLayoutStoredProcedure = "p_DeleteDashboardLayout";
        private const string GetDashboardLayoutIDStoredProcedure = "p_GetDashboardLayoutID";
        private const string GetDashboardLayoutsStoredProcedure = "p_GetDashboardLayouts";
        private const string GetDashboardLayoutStoredProcedure = "p_GetDashboardLayout";
        private const string GetServerDashboardLayoutStoredProcedure = "p_GetServerDashboardLayout";
        private const string SetDefaultDashboardLayoutStoredProcedure = "p_SetDefaultDashboardLayout";
        private const string UpdateDashboardLayoutStoredProcedure = "p_UpdateDashboardLayout";
        private const string UpdateDashboardLayoutViewedStoredProcedure = "p_UpdateDashboardLayoutViewed";
        private const string AddUpdateVirtualHostStoredProcedure = "p_AddUpdateVirtualHost";
        private const string DeleteVirtualHostStoredProcedure = "p_DeleteVirtualHost";
        private const string AddVMToMonitoredSQLServerStoredProcedure = "p_AddVMToMonitoredServer";
        private const string GetVirtualHostServersStoredProcedure = "p_GetVirtualHostServers";
        private const string GetVMAssociatedSQLServersCommand = "SELECT a.[SQLServerID] as [assocSQLServerID], a.[InstanceName] as [assocInstanceName], a.[VHostID] as [assocHostID], b.[VHostAddress] as [assocServer], b.[VHostName] as [assocServerName], '' as [assocName], a.[VmUID] as [assocVmUID] from [MonitoredSQLServers] a LEFT OUTER JOIN [VirtualHostServers] b on a.[VHostID] = b.[VHostID] WHERE a.[Active] = 1";
        private const string GetServerHostNameCommand = "SELECT SQLServerID = mss.SQLServerID, ss.ServerHostName FROM MonitoredSQLServers mss INNER JOIN ServerStatistics ss on (mss.SQLServerID = ss.SQLServerID and mss.LastScheduledCollectionTime = ss.UTCCollectionDateTime) WHERE mss.Active = 1";
        private const string DeleteAlertTemplateStoredProcedure = "p_DeleteAlertTemplate";
        private const string SetDefaultAlertTemplateStoredProcedure = "p_SetDefaultAlertTemplate";
        private const string UpdateAlertTemplateStoredProcedure = "p_UpdateAlertTemplate";
        private const string GetMetricThresholdsStoredProcedure = "p_GetMetricThresholds";
        private const string GetMetricInfoStoredProcedure = "p_GetMetricInfo";
        private const string GetTableFragmentationStoredProcedure = "p_GetTableFragmentation";
        private const string GetManagementServicesStoredProcedure = "p_GetManagementServices";
        private const string GetServerActivityStoredProcedure = "p_GetServerActivity";
        //Changing behaviour of history browser for analysis tab 
        private const string GetAnalysisActivityStoredProcedure = "p_GetAnalysisActivity";
        private const string GetServerSummaryStoredProcedure = "p_GetServerSummary";
        private const string GetSessionsDetailsStoredProcedure = "p_GetSessionsDetails";
        private const string GetSessionsDetailsRangedStoredProcedure = "p_GetSessionsDetailsRanged";
        private const string GetLocksDetailsStoredProcedure = "p_GetLocksDetails";

        private const string GetBlockingDetailsStoredProcedure = "p_GetBlockingDetails";
        private const string GetPreviousServerActivitySnapshotDateTimeStoredProcedure = "p_GetPreviousServerActivitySnapshotDateTime";
        private const string GetNextServerActivitySnapshotDateTimeStoredProcedure = "p_GetNextServerActivitySnapshotDateTime";
        //For analysis tab
        private const string GetPreviousAnalysisActivitySnapshotDateTimeStoredProcedure = "p_GetPreviousAnalysisActivitySnapshotDateTime";
        private const string GetNextAnalysisActivitySnapshotDateTimeStoredProcedure = "p_GetNextAnalysisActivitySnapshotDateTime";
        private const string GetCounterCategoriesProcedure = "p_GetCounterCategories";
        private const string GetInstancesMonitoringCustomCounterProcedure = "p_GetInstancesMonitoringCustomCounter";
        private const string GetMonitoredSQLServerCountersProcedure = "p_GetMonitoredSQLServerCounters";
        private const string GetCustomCounterStatisticsProcedure = "p_GetCustomCounterStatistics";
        private const string GetBaselineForMetricsProcedure = "p_GetBaselineForMetrics";
        private const string GetStateOverviewStoredProcedure = "p_GetStateOverview";
        private const string GetCounterNameAvailableStoredProcedure = "p_GetCounterNameAvailable";
        //SQLdm 10.0 (Swati Gogia):Export Import Wizard
        private const string GetRuleNameAvailableStoredProcedure = "p_GetRuleNameAvailable";
        private const string GetTablesStoredProcedure = "p_GetTables";
        private const string GetSnoozedAlertsStoredProcedure = "p_GetSnoozedAlerts";
        private const string GetAllMetricAlertsNameProcedure = "p_GetAllMetricsAlertsName";
        public const string GetAlertsForInstanceProcedure = "p_GetAlertsForInstance"; //SqlDM 10.2 (Anshul Aggarwal) - Fetches Alerts for Server > ActiveAlertsView
        private const string GetRepositoryUserSql = "select system_user";
        private const string GetTagsStoredProcedure = "p_GetTags";
        private const string GetTagConfigurationStoredProcedure = "p_GetTagConfiguration";
        private const string GetServersWithTagIdStoredProcedure = "p_GetServersWithTagId";
        private const string GetServerTagsStoredProcedure = "p_GetServerTags";
        private const string GetMirroredServersStoredProcedure = "p_GetMonitoredMirroredServers";
        private const string GetMirroredDatabasesStoredProcedure = "p_GetMirroredDatabases";
        private const string GetPermissionTagsAndServersStoredProcedure = "p_GetPermissionTagsAndServers";
        private const string GetCustomCounterTagsAndServersStoredProcedure = "p_GetCustomCounterTagsAndServers";
        private const string GetCustomCountersWithTagIdsStoredProcedure = "p_GetCustomCountersWithTagIds";
        private const string GetCurrentDiskDrivesStoredProcedure = "p_GetCurrentDiskDrives";
        private const string GetKnownDatabasesStoredProcedure = "p_GetDatabases";
        private const string GetDeadlockStoredProcedure = "p_GetDeadlock";
        private const string GetBlockStoredProcedure = "p_GetBlock";
        private const string GetNotificationRulesStoredProcedure = "p_GetNotificationRules";
        private const string ReplicationGetParticipantsForServer = "p_GetReplicationParticipantsForServer";
        private const string ReplicationChartHistory = "p_GetReplicationChartMetrics";
        private const string GetDatabaseCountersStoredProcedure = "p_GetDatabaseCounters";
        private const string GetTempdbSummaryDataStoredProcedure = "p_GetTempdbSummaryData";
        private const string GetTempdbFileDataStoredProcedure = "p_GetTempdbFileData";
        private const string GetDiskSizeDetailsStoredProcedure = "p_GetDiskSizeDetails";//SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements -Added new constant for new procedure

        //string-29697
        private const string AssignCountersToServerProcedure = "p_AssignCountersToServer";

        private const string GetCustomReports = "p_GetCustomReports";
        private const string GetSelectedCustomReportCounters = "p_GetSelectedCustomReportCounters";
        private const string GetAvailableCustomReportCounters = "p_GetListOfCounters";
        //This is only used from reports
        private const string GetCustomReportDataSet = "p_GetCustomReportsDataSet";
        private const string GetDefaultTemplateID = "p_GetDefaultTemplateID";
        private const string GetBlocksList = "p_GetBlocksList";
        private const string GetDeadlocksList = "p_GetDeadlocksList";

        //This is for AlwaysOn reports to get all the Always on servers that are being monitored by SQLdm.
        private const string GetAlwaysOnServersStoredProcedure = "p_GetMonitoredAlwaysOnServers";
        private const string GetAlwaysOnAGBasedActiveServers = "p_GetAlwaysOnAGBasedActiveServers";

        //Addd Server Wizard
        private const string GetVersion = " select serverproperty('productversion') ";//SQLdm 9.0 (Ankit Srivastava) -- Query Monitor Improvement - Add server wizard - constant for checking the server version
        //10.0 SQLdm srishti purohit Docotrs UI implemetation        
        private const string GetRecommendationHisotry = "p_GetRecommendationHistory";
        private const string UpdateFlagStatusPrescriptiveAnalysisRecommendation = "p_UpdateRecommendationFlagStatus";
        private const string GetAllCategoriesStoredProcedure = "p_GetAllCategories";
        private const string GetDatabasesForServer = "p_GetDatabases";  // SQLdm 10.0 (srishti purohit): added this for getting databases list for a SQL Server
        private const string GetBlockedRecommendationDatabaseAnalysisConfigurationSP = "p_GetBlockedRecommendationDatabaseAnalysisConfiguration";

        //Get metric list for baseline assistant 
        private const string GetMetricListStoredProcedure = "p_GetMetricList";

        //baseline value for a date and history in minutes 
        private const string GetAllBaselinesDataStoredProcedure = "p_GetAllBaselinesData";//SQLdm 10.0 (Tarun Sapra) - This stored proc would get data for all the baselines
        private const string GetBdaDataStoredProcedure = "p_GetBaselineDataForOneWeek";//SQLdm 10.0 (Tarun Sapra) - This stored proc would get data for BDA
        private const string GetCloudProvidersStoredProcedure = "p_GetCloudProviders";//SQLdm 10.0 (Tarun Sapra) - This stored proc would get data for cloud providers

        private const string GetAvailbilityGroups = "p_GetAvailbilityGroups";
        private const string GetAzureApplicationProfilesDirectQuery = "SELECT [ID],[Name] FROM dbo.AzureApplicationProfile ORDER BY [Name]";
        private const string GetAzureSqlSevers = "SELECT [SQLServerID],[InstanceName] FROM dbo.MonitoredSQLServers ORDER BY [InstanceName]";

        #endregion

        private static readonly Logger Log = Logger.GetLogger("RepositoryHelper");
        private static readonly Logger StartUpTimeLog = Logger.GetLogger(TextConstants.StartUpTimeLogName);

        private const string GetCloudProviderIdForMonitoredServerCommand = "select CloudProviderId from MonitoredSQLServers where SQLServerID = {0}";//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
        private const int MONITORED_SQL_SERVER_CALL_TIMEOUT = 5;
        private const string GetVersionAndPermissions = @"select serverproperty('productversion') AS ProductVersion, is_srvrolemember('sysadmin') AS SysAdmin;";//SQLdm 10.1 (srishti purohit) -IsSysAdmin check

        private const string UpdateSysAdminPermissionsInRepository = "p_UpdateSysAdminPermissions";

        //Get and Set User Session Settings
        private const string UpdateUserSetting = "p_SaveDMHistorySetting";
        private const string GetUserSettings = "p_GetUserSessionSettings";

        private const string GetQueryPlanInformationStoredProcedure = "p_GetQueryPlanInformation";

        //SQLdm 10.4 (Nikhil Bansal) - For Getting the info required to save the fetched plan
        private const string GetInfoToSavePlanStoredProcedure = "p_GetInfoToSavePlan";

        //SQLdm 10.4 (Nikhil Bansal) - For Saving the fetched plan
        private const string AddSQLQueryPlanStoredProcedure = "p_AddSQLQueryPlan";

        private const string GetAzureProfilesStoredProcedure = "p_GetAzureProfiles";
        private const string GetAzureApplicationProfilesStoredProcedure = "p_GetAzureApplicationProfiles";
        private const string GetAzureProfileWithMetricInfoStoredProcedure = "p_GetAzureProfileWithMetricInfo";
        private const string GetAzureApplicationsStoredProcedure = "p_GetAzureApplications";
        private const string GetAzureSubscriptionsStoredProcedure = "p_GetAzureSubscriptions";
        private const string InsertUpdateAzureSubscriptionsStoredProcedure = "p_InsertUpdateAzureSubscriptions";
        private const string DeleteAzureSubscriptionsStoredProcedure = "p_DeleteAzureSubscriptions";
        private const string InsertUpdateAzureAppProfileStoredProcedure = "p_InsertUpdateAzureAppProfile";
        private const string DeleteAzureApplicationStoredProcedure = "p_DeleteAzureApplication";
        private const string DeleteAzureApplicationProfileStoredProcedure = "p_DeleteAzureApplicationProfile";
        private const string DeleteAzureProfileStoredProcedure = "p_DeleteAzureProfile";

        private const string GettemplateName = "p_GetTemplateList";
        private const string GetCloudByInstanceId = "p_GetCloudByInstanceID";
        private const string AddUpdateAzureApplicationProcedure = "p_AddAzureApplication";
        private const string AddUpdateAzureLinkedProfileProcedure = "p_AddAzureLinkedProfile";
		private const string GetCustomCountersForTemplate = "p_GetCustomCountersForTemplate";     
		/// <summary>
        /// SqlDm 10.2 - Mitul Kapoor - Save user setting in database(Table : UserSessionSettings)
        /// SP call to save user preferences in database.
        /// </summary>
        public static void SetUserSessionSettings(SqlConnectionInfo sqlConnectionInfo, string settingXml)
        {
            try
            {
                if (sqlConnectionInfo == null)
                {
                    throw new NullReferenceException("Connection not found");
                }
                using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, UpdateUserSetting))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@UserId", sqlConnectionInfo.ActiveRepositoryUser);
                        command.Parameters.AddWithValue("@key", Idera.SQLdm.Common.Constants.KEY_USER_PERSIST_SETTINGS);
                        command.Parameters.AddWithValue("@value", settingXml);
                        int errorFound = Convert.ToInt32(command.ExecuteScalar());
                        if (errorFound == 0)
                        {
                            Log.Info("Settings Saved!!");
                        }
                        else
                        {
                            Log.Error("Error Found when executing : " + UpdateUserSetting);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception Found : " + ex);
            }
        }

        /// <summary>
        /// SqlDm 10.2 - Mitul Kapoor - Gets the user setting from the database(Table : UserSessionSettings)
        /// SP call to get the user saved preferences in database.
        /// /// </summary>
        public static string GetUserSessionSettings(SqlConnectionInfo sqlConnectionInfo)
        {
            try
            {
                Dictionary<string, string> Settings = new Dictionary<string, string>();
                if (sqlConnectionInfo == null)
                {
                    throw new ArgumentNullException("Connection not Found!");
                }
                using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetUserSettings, sqlConnectionInfo.ActiveRepositoryUser))
                    {
                        while (reader.Read())
                        {
                            Settings.Add(reader["Key"].ToString(), reader["Value"].ToString());
                        }
                    }
                }
                return Settings.ContainsKey(Constants.KEY_USER_PERSIST_SETTINGS) ? Settings[Constants.KEY_USER_PERSIST_SETTINGS] : null;
            }
            catch (Exception ex)
            {
                Log.Error("Exception Occured : " + ex.Message);
                return null;
            }
        }

        #region Test

        public static bool IsValidRepository(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                try
                {
                    using (
                        SqlDataReader dataReader =
                            SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure,
                                                    GetRepositoryVersionSqlCommand))
                    {
                        dataReader.Read();
                        return dataReader.GetString(0) == Constants.ValidRepositorySchemaVersion;
                    }
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
                        return false;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        private static SqlConnectionInfo CheckConnectionInfo(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                // Use Settings.Default.ActiveRepositoryConnection.ConnectionInfo.
                if (Settings.Default.ActiveRepositoryConnection.ConnectionInfo == null)
                {
                    throw new ArgumentNullException("connectionInfo");
                }
                else
                {
                    connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
                }
            }

            return connectionInfo;
        }

        #endregion

        public static RepositoryInfo GetRepositoryInfo(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                try
                {
                    using (
                        SqlDataReader dataReader =
                            SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure,
                                                    GetRepositoryInfoStoredProcedure))
                    {
                        string versionString = string.Empty;
                        string instanceName = string.Empty;
                        int monitoredServerCount = 0;

                        while (dataReader.Read())
                        {
                            switch (dataReader.GetString(0))
                            {
                                case "Repository Version":
                                    versionString = dataReader.GetString(2);
                                    break;
                                case "Instance Name":
                                    instanceName = dataReader.GetString(2);
                                    break;
                                case "Active Servers":
                                    monitoredServerCount = dataReader.GetInt32(1);
                                    break;
                            }
                        }

                        return new RepositoryInfo(versionString, monitoredServerCount, instanceName);
                    }
                }
                catch (SqlException e)
                {
                    // Assuming that a valid connection can be established to the SQL Server, 
                    // an invalid call to the version procedure would indicate an invalid database;
                    // all other exceptions will be passed on.
                    //
                    // Error 2812 = cannot find stored procedure in SQL Server 2000 & 2005
                    //
                    if (e.Number == 2812)
                    {
                        return null;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        //START: SQLdm 10.0 (Tarun Sapra) - Get the list of the metrics for which bda is rqd
        public static Dictionary<string, Tuple<int, string>> GetMetricList(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            Dictionary<string, Tuple<int, string>> dict = new Dictionary<string, Tuple<int, string>>();
            using (
                SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, GetMetricListStoredProcedure))
                {
                    while (dataReader.Read())
                    {
                        string key = Convert.ToString(dataReader[0]);
                        int value1 = Convert.ToInt32(dataReader[1]);
                        string value2 = Convert.ToString(dataReader[2]);
                        value2 = (value2 == "") ? key : value2;//if no unit is defined, metric name would be displayed as unit on chart
                        dict.Add(key, new Tuple<int, string>(value1, value2));
                    }
                }
            }
            return dict;
        }
        //END: SQLdm 10.0 (Tarun Sapra) - Get the list of the metrics for which bda is rqd

        //START: SQLdm 10.0 (Tarun Sapra) - Returns the modified date for the same time and day of week as the original datetime
        private static string GetModifiedDateForBDA(DateTime date)
        {
            string modifiedDate = "";

            string mon = "1947-01-06";
            string tue = "1947-01-07";
            string wed = "1947-01-08";
            string thr = "1947-01-09";
            string fri = "1947-01-10";
            string sat = "1947-01-11";
            string sun = "1947-01-12";

            switch ((int)date.DayOfWeek)
            {
                case 1:
                    modifiedDate = mon + " " + date.ToString("HH:mm:ss");
                    break;
                case 2:
                    modifiedDate = tue + " " + date.ToString("HH:mm:ss");
                    break;
                case 3:
                    modifiedDate = wed + " " + date.ToString("HH:mm:ss");
                    break;
                case 4:
                    modifiedDate = thr + " " + date.ToString("HH:mm:ss");
                    break;
                case 5:
                    modifiedDate = fri + " " + date.ToString("HH:mm:ss");
                    break;
                case 6:
                    modifiedDate = sat + " " + date.ToString("HH:mm:ss");
                    break;
                case 0:
                    modifiedDate = sun + " " + date.ToString("HH:mm:ss");
                    break;
            }

            return modifiedDate;
        }
        //END: SQLdm 10.0 (Tarun Sapra) - Returns the modified date for the same time and day of week as the original datetime

        //START: SQLdm 10.0 (Tarun Sapra) - Gets week wise data from the db and fills the data table
        public static DataTable GetDataForBDA(SqlConnectionInfo connectionInfo, int instanceId, int itemId, int noOfWeeks)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Week1", typeof(double));
            dataTable.Columns.Add("Week2", typeof(double));
            dataTable.Columns.Add("Week3", typeof(double));
            dataTable.Columns.Add("Week4", typeof(double));
            dataTable.Columns.Add("Week5", typeof(double));
            dataTable.Columns.Add("Week6", typeof(double));
            dataTable.Columns.Add("Week7", typeof(double));
            dataTable.Columns.Add("Week8", typeof(double));
            dataTable.Columns.Add("Date", typeof(DateTime)).AllowDBNull = false;

            int week1Delta = ((int)DateTime.Now.DayOfWeek - 1 + 7) % 7;
            DateTime endDate = DateTime.Now;
            DateTime startDate = Convert.ToDateTime(DateTime.Now.AddDays(-1 * week1Delta).ToString("yyyy-MM-dd"));

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                for (int i = 0; i < noOfWeeks; i++)
                {
                    if (i > 0)//not first week
                    {
                        endDate = startDate;
                        startDate = endDate.AddDays(-7);
                    }
                    using (SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetBdaDataStoredProcedure, instanceId, itemId, startDate.ToUniversalTime(), endDate.ToUniversalTime()))
                    {
                        dataTable.BeginLoadData();

                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                DataRow row = dataTable.NewRow();
                                DateTime date = Convert.ToDateTime(dataReader[1]).ToLocalTime();
                                row["Week" + (i + 1).ToString()] = dataReader[0];
                                row["Date"] = GetModifiedDateForBDA(date);
                                dataTable.Rows.Add(row);
                            }
                        }
                        dataTable.EndLoadData();
                    }
                }
            }
            return dataTable;
        }
        //END: SQLdm 10.0 (Tarun Sapra) - Gets week wise data from the db and fills the data table

        #region Management Service

        public static IList<ManagementServiceConfiguration> GetManagementServices(SqlConnectionInfo connectionInfo, Guid? managementServiceId)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<ManagementServiceConfiguration> result = new List<ManagementServiceConfiguration>();
            object serviceId = (managementServiceId.HasValue) ? (object)managementServiceId.Value : DBNull.Value;

            using (
                SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(connection, GetManagementServicesStoredProcedure, serviceId))
                {
                    while (dataReader.Read())
                    {
                        string identifier = dataReader["ManagementServiceID"] as string;
                        string machineName = dataReader["MachineName"] as string;
                        string instanceName = dataReader["InstanceName"] as string;
                        string address = dataReader["Address"] as string;
                        int port = (int)dataReader["Port"];

                        result.Add(new ManagementServiceConfiguration(identifier, machineName, instanceName, address, port));
                    }
                }
            }

            return result;
        }

        public static ManagementServiceConfiguration GetDefaultManagementService(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure,
                                                GetDefaulManagementServiceStoredProcedure))
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

        #endregion

        #region MonitoredSqlServer

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
                dataReader.IsDBNull(dataReader.GetOrdinal("QueryMonitorStopTimeUTC"))
                    ? null
                    : (DateTime?)dataReader["QueryMonitorStopTimeUTC"],
                (bool)dataReader["QueryMonitorTraceMonitoringEnabled"],
                (bool)dataReader["QueryMonitorCollectQueryPlan"], //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session --Get the newly added columns from  the repository
                (bool)dataReader["QueryMonitorCollectEstimatedQueryPlan"] //SQLdm 10.0 (Tarun Sapra): Get the flag value for estimated query plan
                ,
                (int)dataReader["QueryMonitorTopPlanCountFilter"],
                (int)dataReader["QueryMonitorTopPlanCategoryFilter"],
                (bool)dataReader["QueryMonitorQueryStoreMonitoringEnabled"]);  // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store

            ActivityMonitorConfiguration activityProfilerConfiguration = new ActivityMonitorConfiguration(
                (bool)dataReader["ActivityMonitorEnabled"],
                (bool)dataReader["ActivityMonitorDeadlockEventsEnabled"],
                (bool)dataReader["ActivityMonitorBlockingEventsEnabled"],
                (bool)dataReader["ActivityMonitorAutoGrowEventsEnabled"],
                (int)dataReader["ActivityMonitorBlockedProcessThreshold"],
                new FileSize((int)dataReader["QueryMonitorTraceFileSizeKB"]),
                (int)dataReader["QueryMonitorTraceFileRollovers"],
                (int)dataReader["QueryMonitorTraceRecordsPerRefresh"],
                queryMonitorAdvancedConfiguration,
                dataReader.IsDBNull(dataReader.GetOrdinal("QueryMonitorStopTimeUTC"))
                    ? null
                    : (DateTime?)dataReader["QueryMonitorStopTimeUTC"],
                    (bool)dataReader["ActivityMonitorTraceMonitoringEnabled"]);//SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events --Get the newly added column from  the repository

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
                                       activityProfilerConfiguration,
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

            ordinal = dataReader.GetOrdinal("AlertRefreshInMinutes");
            if (dataReader.IsDBNull(ordinal))
            {
                instance.AlertRefresInMinutes = true;
            }
            else
            {
                instance.AlertRefresInMinutes = dataReader.GetBoolean(ordinal);
            }

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
                    Log.Error("Exception deserializing TableStatisticsExcludedDatabases ", e);
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
                Log.Debug("Exception getting EarliestData in ConstructMonitoredSqlServer: ", ex);
            }

            instance.ReorganizationMinimumTableSize.Kilobytes = (int)dataReader["ReorgMinTableSizeKB"];

            ordinal = dataReader.GetOrdinal("DisableReplicationMonitoring");
            if (!dataReader.IsDBNull(ordinal))
                instance.ReplicationMonitoringDisabled = dataReader.GetBoolean(ordinal);

            ordinal = dataReader.GetOrdinal("DisableExtendedHistoryCollection");
            if (!dataReader.IsDBNull(ordinal))
                instance.ExtendedHistoryCollectionDisabled = dataReader.GetBoolean(ordinal);

            ordinal = dataReader.GetOrdinal("CustomCounterTimeoutInSeconds");
            if (dataReader.IsDBNull(ordinal))
                instance.CustomCounterTimeout = TimeSpan.FromSeconds(180);
            else
                instance.CustomCounterTimeout = TimeSpan.FromSeconds(dataReader.GetInt32(ordinal));

            ordinal = dataReader.GetOrdinal("DisableOleAutomation");
            if (!dataReader.IsDBNull(ordinal))
                instance.OleAutomationUseDisabled = dataReader.GetBoolean(ordinal);
            else
                instance.OleAutomationUseDisabled = false;  // treat null value as false

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
                    Log.Error("Exception deserializing DiskCollectionSettings", e);
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

            ordinal = dataReader.GetOrdinal("FriendlyServerName");
            if (!dataReader.IsDBNull(ordinal))
                instance.FriendlyServerName = dataReader.GetString(ordinal);

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
                if (!dataReader.IsDBNull(ordinal)) awc.EnabledXe = dataReader.GetBoolean(ordinal);

                ordinal = dataReader.GetOrdinal("ActiveWaitXEFilesRollover");
                if (!dataReader.IsDBNull(ordinal)) awc.FileSizeRolloverXe = dataReader.GetInt32(ordinal);

                ordinal = dataReader.GetOrdinal("ActiveWaitXEFileSizeMB");
                if (!dataReader.IsDBNull(ordinal)) awc.FileSizeXeMB = dataReader.GetInt32(ordinal);

                ordinal = dataReader.GetOrdinal("ActiveWaitXERecordsPerRefresh");
                if (!dataReader.IsDBNull(ordinal)) awc.RecordsPerRefreshXe = dataReader.GetInt32(ordinal);

                ordinal = dataReader.GetOrdinal("ActiveWaitXEMaxMemoryMB");
                if (!dataReader.IsDBNull(ordinal)) awc.MaxMemoryXeMB = dataReader.GetInt32(ordinal);

                ordinal = dataReader.GetOrdinal("ActiveWaitXEEventRetentionMode");
                //SQLdm 9.1 (Ankit Srivastava) Activity Monitoring with Extended Events - enum made public 
                if (!dataReader.IsDBNull(ordinal)) awc.EventRetentionModeXe = (XeEventRetentionMode)dataReader.GetByte(ordinal);

                ordinal = dataReader.GetOrdinal("ActiveWaitXEMaxDispatchLatencySecs");
                if (!dataReader.IsDBNull(ordinal)) awc.MaxDispatchLatencyXe = dataReader.GetInt32(ordinal);

                ordinal = dataReader.GetOrdinal("ActiveWaitXEMaxEventSizeMB");
                if (!dataReader.IsDBNull(ordinal)) awc.MaxEventSizeXemb = dataReader.GetInt32(ordinal);

                ordinal = dataReader.GetOrdinal("ActiveWaitXEMemoryPartitionMode");
                //SQLdm 9.1 (Ankit Srivastava) Activity Monitoring with Extended Events - enum made public 
                if (!dataReader.IsDBNull(ordinal)) awc.MemoryPartitionModeXe = (XEMemoryPartitionMode)dataReader.GetInt32(ordinal);

                ordinal = dataReader.GetOrdinal("ActiveWaitXETrackCausality");
                if (!dataReader.IsDBNull(ordinal)) awc.TrackCausalityXe = dataReader.GetBoolean(ordinal);

                ordinal = dataReader.GetOrdinal("ActiveWaitXEStartupState");
                if (!dataReader.IsDBNull(ordinal)) awc.StartupStateXe = dataReader.GetBoolean(ordinal);

                ordinal = dataReader.GetOrdinal("ActiveWaitsXEFileName");
                if (!dataReader.IsDBNull(ordinal)) awc.FileNameXEsession = dataReader.GetString(ordinal);

            }
            else
            {
                awc.Enabled = false;
            }
            instance.ActiveWaitsConfiguration = awc;

            ordinal = dataReader.GetOrdinal("ClusterCollectionSetting");
            if (!dataReader.IsDBNull(ordinal))
                instance.ClusterCollectionSetting = (ClusterCollectionSetting)dataReader.GetInt16(ordinal);

            ordinal = dataReader.GetOrdinal("ServerPingInterval");
            if (!dataReader.IsDBNull(ordinal))
                instance.ServerPingInterval = TimeSpan.FromSeconds(dataReader.GetInt16(ordinal));

            if (dataReader["VHostID"] != DBNull.Value && (int)dataReader["VHostID"] > 0)
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
                //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
                instance.BaselineConfiguration.BaselineName = Common.Constants.DEFAULT_BASELINE_NAME;
                instance.BaselineConfiguration.Active = true;
                //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
            }

            #region Custom Baselines
            ordinal = dataReader.GetOrdinal("CustomBaselineTemplates");
            if (!dataReader.IsDBNull(ordinal) && !string.IsNullOrEmpty(dataReader.GetString(ordinal)))
                instance.BaselineConfigurationList = Common.Data.BaselineHelpers.GetBaselineDictionaryFromArray(dataReader.GetString(ordinal).Split(','));
            else
                instance.BaselineConfigurationList = new Dictionary<int, BaselineConfiguration>();
            #endregion

            if (dataReader["ServerVersion"] != DBNull.Value)
            {
                if (dataReader["ServerVersion"].ToString() != "?")
                    instance.MostRecentSQLVersion = new ServerVersion((string)dataReader["ServerVersion"]);
            }

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
                    , String.IsNullOrEmpty(dataReader["BlockedCategories"].ToString()) ? new List<int>() : ((dataReader["BlockedCategories"].ToString()).Split(',')).Select(Int32.Parse).ToList()
                    , String.IsNullOrEmpty(dataReader["BlockedCategoriesWithName"].ToString()) ? new Dictionary<int, string>() : GetDictinaoryFromArray((dataReader["BlockedCategoriesWithName"].ToString()).Split(','))
                    , String.IsNullOrEmpty(dataReader["BlockedDatabases"].ToString()) ? new List<int>() : ((dataReader["BlockedDatabases"].ToString()).Split(',')).Select(Int32.Parse).ToList()
                    , String.IsNullOrEmpty(dataReader["BlockedDatabasesWithName"].ToString()) ? new Dictionary<int, string>() : GetDictinaoryFromArray((dataReader["BlockedDatabasesWithName"].ToString()).Split(','))
                    , String.IsNullOrWhiteSpace(dataReader["BlockedRecommendations"].ToString()) ? new List<string>() : ((dataReader["BlockedRecommendations"].ToString()).Split(',')).ToList()
                    , (bool)dataReader["SchedulingStatus"]);
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
            if (dataReader["CloudProviderId"] != DBNull.Value)
                instance.CloudProviderId = (int)dataReader["CloudProviderId"];

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
                Log.Error(ex.Message + " Error while getting blocked categories or blocked databases.");
                throw new Exception("Error while getting blocked categories or blocked databases. " + ex.Message);
            }
            return blockedItems;
        }

        public static MonitoredSqlServer GetMonitoredSqlServer(SqlConnectionInfo connectionInfo, int id)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {

                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetMonitoredSqlServersByIdStoredProcedure, id, false))
                {
                    if (!dataReader.HasRows)
                    {
                        return null;
                    }
                    else
                    {
                        dataReader.Read();
                        MonitoredSqlServer instance = ConstructMonitoredSqlServer(dataReader);
                        //[START] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
                        //        using (
                        //SqlConnection connection1 =
                        //    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                        //        using (
                        //            SqlDataReader reader =
                        //                SqlHelper.ExecuteReader(connection1, GetBaselineTemplatesByIdStoredProcedure, id))
                        //        {
                        //            instance.BaselineConfigurationList = new Dictionary<int, BaselineConfiguration>();
                        //            while (reader.Read())
                        //            {
                        //                BaselineConfiguration config = new BaselineConfiguration();
                        //                config = new BaselineConfiguration((string)reader["Template"]);
                        //                config.TemplateID = (int)reader["TemplateID"];
                        //                config.Active = (bool)reader["Active"];
                        //                config.BaselineName = reader["BaselineName"].ToString();
                        //                config.IsChanged = false;
                        //                instance.BaselineConfigurationList.Add(config.TemplateID, config);
                        //            }
                        //        }
                        //[END] SQLdm 10.0 (Rajesh Gupta): Baseline Enhancement - Added a list for multiple BaselineConfiguration 
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
                }
            }
        }

        public static IList<MonitoredSqlServer> GetMonitoredSqlServers(SqlConnectionInfo connectionInfo, bool activeOnly)
        {
            return GetMonitoredSqlServers(connectionInfo, activeOnly, true, null);
        }

        /// <summary>
        /// To get license keys in addition to the servers, pass a non-null value
        /// for the licenseKeys parameter.  It will be cleared and then populated
        /// from the stored proc's result set. 
        /// </summary>
        public static IList<MonitoredSqlServer> GetMonitoredSqlServers(
            SqlConnectionInfo connectionInfo,
            bool activeOnly,
            bool customCounters,
            List<string> licenseKeys)
        {
            Stopwatch stopWatchMain = new Stopwatch();
            stopWatchMain.Start();
            connectionInfo = CheckConnectionInfo(connectionInfo);

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlCommand command = SqlHelper.CreateCommand(connection, GetMonitoredSqlServersStoredProcedure))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, null, activeOnly, licenseKeys != null, customCounters, null, null); //SQLDM8.5 Mahesh: Added Additional Params
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        stopWatch.Stop();
                        StartUpTimeLog.DebugFormat("Time taken by p_GetMonitoredSqlServers sp : {0}", stopWatch.ElapsedMilliseconds);
                        Dictionary<int, MonitoredSqlServer> instances = new Dictionary<int, MonitoredSqlServer>();

                        stopWatch.Reset();
                        stopWatch.Start();
                        while (dataReader.Read())
                        {
                            MonitoredSqlServer instance = ConstructMonitoredSqlServer(dataReader);
                            instances.Add(instance.Id, instance);
                        }
                        stopWatch.Stop();
                        StartUpTimeLog.DebugFormat("Time taken for Creating and adding instance to instances dictionary : {0}", stopWatch.ElapsedMilliseconds);
                        if (licenseKeys != null && dataReader.NextResult())
                        {
                            int fieldId = dataReader.GetOrdinal("LicenseKey");
                            licenseKeys.Clear();
                            while (dataReader.Read())
                            {
                                licenseKeys.Add(dataReader.GetString(fieldId));
                            }
                        }

                        if (customCounters && dataReader.NextResult())
                        {   // update custom counters
                            MonitoredSqlServer instance = null;
                            while (dataReader.Read())
                            {
                                int id = dataReader.GetInt32(0);
                                int metric = dataReader.GetInt32(1);
                                if (instance == null || instance.Id != id)
                                {
                                    if (!instances.TryGetValue(id, out instance))
                                        continue;
                                }
                                instance.CustomCounters.Add(metric);
                            }
                        }
                        if (customCounters && dataReader.NextResult())
                        {   // update tags
                            MonitoredSqlServer instance = null;
                            while (dataReader.Read())
                            {
                                int id = dataReader.GetInt32(0);
                                int tagId = dataReader.GetInt32(1);
                                if (instance == null || instance.Id != id)
                                {
                                    if (!instances.TryGetValue(id, out instance))
                                        continue;
                                }
                                instance.Tags.Add(tagId);
                            }
                        }
                        stopWatchMain.Stop();
                        StartUpTimeLog.DebugFormat("Time taken by GetMonitoredSqlServers : {0}", stopWatchMain.ElapsedMilliseconds);
                        return new List<MonitoredSqlServer>(instances.Values);
                    }
                }
            }
        }

        /// <summary>
        /// To get blocked recommendations and databases
        /// 10.0 Srishti Purohit SQLdm
        /// <param name="id"></param>
        /// </summary>
        public static Triple<Dictionary<int, string>, List<int>, List<string>> GetBlockedRecommendationDatabaseAnalysisConfiguration(SqlConnectionInfo connectionInfo, int id)
        {
            Dictionary<int, string> listOfAllDatabases = new Dictionary<int, string>();
            List<int> blockedDB = new List<int>();
            List<string> blockedRecomm = new List<string>();
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo is null.");
            }

            if (id == 0)
            {
                throw new ArgumentNullException("server incorrect");
            }
            try
            {
                //DB call to save
                connectionInfo = CheckConnectionInfo(connectionInfo);

                using (
                    SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, GetBlockedRecommendationDatabaseAnalysisConfigurationSP))
                    {
                        command.Parameters.Clear();

                        command.Parameters.AddWithValue("@sqlServerID", id);
                        SqlDataReader dataReader = command.ExecuteReader();
                        while (dataReader.Read())
                        {
                            listOfAllDatabases.Add(dataReader.GetInt32(0), dataReader.GetString(1));
                        }

                        dataReader.NextResult();
                        while (dataReader.Read())
                        {
                            blockedDB = String.IsNullOrEmpty(dataReader["BlockedDatabases"].ToString()) ? new List<int>() : GetINTListFromArray((dataReader["BlockedDatabases"].ToString()).Split(','));
                            blockedRecomm = (String.IsNullOrEmpty(dataReader["BlockedRecommendations"].ToString()) ? new List<string>() : GetListFromStringArray((dataReader["BlockedRecommendations"].ToString()).Split(',')));

                        }
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
            return new Triple<Dictionary<int, string>, List<int>, List<string>>(listOfAllDatabases, blockedDB, blockedRecomm);
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

        #endregion

        #region Tables

        /// <summary>
        /// Gets the tables from the repository rather than SMO or the management service.
        /// Each triple contains a schema name, table name and if it's a system table.
        /// </summary>
        public static List<Triple<string, string, bool>> GetTables(SqlConnectionInfo connectionInfo, int instanceId, string databaseName)
        {
            connectionInfo = CheckConnectionInfo(connectionInfo);

            List<Triple<string, string, bool>> tableList = new List<Triple<string, string, bool>>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlCommand command = SqlHelper.CreateCommand(connection, GetTablesStoredProcedure))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, instanceId, databaseName);

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                            tableList.Add(new Triple<string, string, bool>((string)dataReader["SchemaName"], (string)dataReader["TableName"], (bool)dataReader["SystemTable"]));
                    }
                }

                return tableList;
            }
        }

        #endregion

        #region Report Data

        /// <summary>
        /// Call the specified stored proc for multi-database reports.
        /// The connectionInfo parameter can be null to use the one stored in Settings.
        /// </summary>
        public static DataTable GetDatabasesReportData(
            string storedProcName,
            SqlConnectionInfo connectionInfo,
            int serverId,
            IList<string> dbNames,
            IList<DateRangeOffset> dateRanges,
            int sample)
        {
            using (Log.DebugCall())
            {
                Log.Debug("storedProcName = ", storedProcName);

                connectionInfo = CheckConnectionInfo(connectionInfo);

                if (dbNames == null || dbNames.Count == 0) throw new ArgumentNullException("dbNames");
                if (dateRanges == null || dateRanges.Count == 0) throw new ArgumentNullException("dateRanges");

                XmlDocument xmlDoc = new XmlDocument();
                XmlElement rootElement = xmlDoc.CreateElement("ReportParameters");
                xmlDoc.AppendChild(rootElement);
                XmlAddList(xmlDoc, dbNames, "Database", "DatabaseName");
                XmlAddDates(xmlDoc, dateRanges);

                using (
                    SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.Debug("Calling ", storedProcName, " with these parameters:");
                        Log.Debug("xmlDoc = ", FormatXml(xmlDoc));
                        Log.Debug("sample = ", sample);
                    }

                    using (
                        SqlDataReader dataReader =
                            SqlHelper.ExecuteReader(connection, storedProcName,
                                                    xmlDoc.InnerXml, serverId, sample))
                    {
                        DataTable table = GetTable(dataReader, false);
                        Log.Debug(table.Rows.Count, " records were returned.");
                        return table;
                    }
                }
            }
        }

        //10.0 srishti purohit --Get All data Bases
        public static List<DatabaseInformation> GetDatabasesForInstance(SqlConnectionInfo connectionInfo, int sqlServerID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<DatabaseInformation> databaseList = new List<DatabaseInformation>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetDatabasesForServer, sqlServerID))
                {
                    while (dataReader.Read())
                    {
                        int ordinal = dataReader.GetOrdinal("DatabaseID");
                        if (!(dataReader.IsDBNull(ordinal)))
                        {
                            DatabaseInformation databaseInfo = new DatabaseInformation();
                            databaseInfo.DatabaseID = dataReader.GetInt32(ordinal);
                            databaseInfo.DatabaseName = dataReader["DatabaseName"].ToString();
                            databaseInfo.IsSystemDatabase = Convert.ToBoolean(dataReader["SystemDatabase"]);
                            databaseList.Add(databaseInfo);
                        }
                    }
                }
                return databaseList;
            }
        }


        /// <summary>
        /// Call the specified stored proc for the multi-table report.
        /// The connectionInfo parameter can be null to use the one stored in Settings.
        /// </summary>
        public static DataTable GetTablesReportData(
            string storedProcName,
            int serverId,
            string dbName,
            IList<string> tableNames,
            IList<DateRangeOffset> dateRanges,
            int sample)
        {
            using (Log.DebugCall())
            {
                if (storedProcName == null) throw new ArgumentNullException("storedProcName");
                if (dbName == null) throw new ArgumentNullException("dbName");
                if (tableNames == null || tableNames.Count == 0) throw new ArgumentNullException("tableNames");
                if (dateRanges == null || dateRanges.Count == 0) throw new ArgumentNullException("dateRanges");

                Log.Debug("storedProcName = ", storedProcName);
                Log.Debug("serverId = ", serverId);
                Log.Debug("dbName = ", dbName);
                Log.Debug("sample = ", sample);

                SqlConnectionInfo connectionInfo = CheckConnectionInfo(null);

                XmlDocument xmlDoc = new XmlDocument();
                XmlElement rootElement = xmlDoc.CreateElement("ReportParameters");
                xmlDoc.AppendChild(rootElement);
                XmlAddList(xmlDoc, tableNames, "Table", "TableName");
                XmlAddDates(xmlDoc, dateRanges);
                Log.Debug("xmlDoc = ", FormatXml(xmlDoc));

                using (
                    SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    using (
                        SqlDataReader dataReader =
                            SqlHelper.ExecuteReader(connection, storedProcName,
                                                    xmlDoc.InnerXml, serverId, dbName, sample))
                    {
                        DataTable table = GetTable(dataReader, false);
                        Log.Debug(table.Rows.Count, " records were returned.");
                        return table;
                    }
                }
            }
        }

        /// <summary>
        /// Perform the specified select and return the results
        /// The connectionInfo parameter can be null to use the one stored in Settings.
        /// </summary>
        public static DataTable GetCustomReportData(string SQLSelect, params SqlParameter[] sqlParameters)
        {
            using (Log.DebugCall())
            {
                //Log.Debug("storedProcName = ", storedProcName);

                SqlConnectionInfo connectionInfo = null;

                connectionInfo = CheckConnectionInfo(connectionInfo);

                using (
                    SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    using (
                    SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, SQLSelect, sqlParameters))
                    {
                        return GetTable(dataReader, false);
                    }
                }
            }
        }

        public static DataTable GetTemplateName()
        {
            using (Log.DebugCall())
            {
                Log.Debug("storedProcName");

                SqlConnectionInfo connectionInfo = null;

                connectionInfo = CheckConnectionInfo(connectionInfo);

                using (
                    SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    using (
                    SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(connection, GettemplateName))
                    {
                        return GetTable(dataReader, false);
                    }
                }
            }
        }


        /// <summary>
        /// Call the specified stored proc for single or multi-server reports (most reports).
        /// The connectionInfo parameter can be null to use the one stored in Settings.
        /// </summary>
        /// 

        public static DataTable GetReportData(string storedProcName, params object[] sqlParameters)
        {
            using (Log.DebugCall())
            {
                Log.Debug("storedProcName = ", storedProcName);

                SqlConnectionInfo connectionInfo = null;

                connectionInfo = CheckConnectionInfo(connectionInfo);

                using (
                    SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    using (
                    SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(connection, storedProcName, sqlParameters))
                    {
                        return GetTable(dataReader, false);
                    }
                }
            }
        }

        public static ReportDataSource LoadReportOutput(string dataSourceName, string storedProc, SqlConnectionInfo connectionInfo, object[] sqlParameters)
        {
            ReportDataSource dataSource = new ReportDataSource(dataSourceName);
            dataSource.Value = GetReportData(storedProc, connectionInfo, sqlParameters);
            return dataSource;
        }


        #endregion

        #region Virtual Machine Crap

        public static void DeleteUnusedVirtualHost(SqlConnectionInfo connectionInfo, List<vCenterHosts> hostList, List<string> edited)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            Dictionary<string, vCenterHosts> existingHosts = GetVirtualHostServers(connectionInfo, null);

            // Delete existing hosts that aren't still in the list
            foreach (vCenterHosts oldHost in existingHosts.Values)
            {
                bool delete = true;
                foreach (vCenterHosts host in hostList)
                {
                    if (host.vcAddress.Equals(oldHost.vcAddress))
                    {
                        delete = false;
                        break;
                    }
                }

                if (delete)
                {
                    deleteVirtualHost(connectionInfo, oldHost);

                    if (!edited.Contains(oldHost.vcAddress))
                    {
                        AuditingEngine.Instance.ManagementService =
                            ManagementServiceHelper.GetDefaultService(
                                Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                        AuditingEngine.Instance.SQLUser =
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UseIntegratedSecurity
                                ? AuditingEngine.GetWorkstationUser()
                                : Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UserName;
                        if (oldHost.ServerType == "HyperV")
                        {
                            AuditingEngine.Instance.LogAction(oldHost.GetAuditableEntity(),
                                                              AuditableActionType.DeleteHyperV, oldHost.vcName);
                        }
                        else if (oldHost.ServerType == "vCenter")
                        {
                            AuditingEngine.Instance.LogAction(oldHost.GetAuditableEntity(),
                                                              AuditableActionType.DeleteVmCenter, oldHost.vcName);
                        }
                    }
                }
            }

        }

        private static void deleteVirtualHost(SqlConnectionInfo connectionInfo, vCenterHosts host)
        {
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlCommand command = SqlHelper.CreateCommand(connection, DeleteVirtualHostStoredProcedure))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, host.vcAddress);
                    command.ExecuteNonQuery();
                }
            }

        }

        public static Dictionary<string, vCenterHosts> GetVirtualHostServers(SqlConnectionInfo connectionInfo, int? hostID)
        {
            Dictionary<string, vCenterHosts> vcDict = new Dictionary<string, vCenterHosts>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetVirtualHostServersStoredProcedure, hostID))
                {
                    while (dataReader.Read())
                    {
                        vcDict.Add(Convert.ToString(dataReader["vcAddress"]), new vCenterHosts(Convert.ToInt32(dataReader["vcHostID"]),
                                                    Convert.ToString(dataReader["vcName"]),
                                                    Convert.ToString(dataReader["vcAddress"]),
                                                    Convert.ToString(dataReader["vcUser"]),
                                                    Convert.ToString(dataReader["vcPassword"]),
                                                    Convert.ToString(dataReader["ServerType"])
                                                    )
                                  );
                    }
                }
            }
            return vcDict;
        }

        public static DataTable GetVMAssociatedSQLServers(SqlConnectionInfo connectionInfo)
        {
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, CommandType.Text, GetVMAssociatedSQLServersCommand))
                {
                    return GetTable(dataReader, true, false);
                }
            }
        }

        public static void AddUpdateVirtualHostServer(SqlConnectionInfo connectionInfo, vCenterHosts host)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlCommand command = SqlHelper.CreateCommand(connection, AddUpdateVirtualHostStoredProcedure))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, host.vcName, host.vcAddress, host.vcUser, host.vcEncryptedPassword, host.ServerType, host.vcHostID);
                    command.ExecuteNonQuery();
                    SqlParameter outParm = command.Parameters["@VhHostID"];
                    if (outParm.Value is int)
                        host.vcHostID = (int)outParm.Value;
                }
            }
        }

        public static void AddUpdateMonitoredSQLServerVM(SqlConnectionInfo connectionInfo, int sqlServerID, object hostID, object vmUID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlCommand command = SqlHelper.CreateCommand(connection, AddVMToMonitoredSQLServerStoredProcedure))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, sqlServerID, hostID, vmUID);
                    command.ExecuteNonQuery();
                }
            }
        }


        #endregion

        #region XML

        /// <summary>
        /// Formats the XmlDocument into a string containing newlines and indentation.
        /// Primarily for logging.
        /// </summary>
        public static string FormatXml(XmlDocument doc)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter writer = new XmlTextWriter(sw);
            writer.Formatting = Formatting.Indented;
            doc.Save(writer);

            return sw.ToString();
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

        // For each DateRangeOffset, insert an XML element under the document's root node.
        private static void XmlAddDates(XmlDocument xmlDoc, IList<DateRangeOffset> dateRanges)
        {
            foreach (DateRangeOffset dro in dateRanges)
            {
                // Using InvariantInfo fixes PR 2010624.
                XmlElement elem = xmlDoc.CreateElement("AllowedDates");
                elem.SetAttribute("UtcStart", dro.UtcStart.ToString(DateTimeFormatInfo.InvariantInfo));
                elem.SetAttribute("UtcEnd", dro.UtcEnd.ToString(DateTimeFormatInfo.InvariantInfo));
                elem.SetAttribute("UtcOffset", dro.UtcOffsetMinutes.ToString());
                xmlDoc.FirstChild.AppendChild(elem);
            }
        }

        #endregion

        //START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
        public static int? GetCloudProviderIdForMonitoredServer(SqlConnectionInfo connectionInfo, int instanceId)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                string commandText = String.Format(GetCloudProviderIdForMonitoredServerCommand, instanceId);
                SqlCommand command = new SqlCommand(commandText, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    if (dataReader.IsDBNull(0))
                        return null;
                    return Convert.ToInt32(dataReader[0]);
                }
                return null;
            }
        }
        //END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support

        public static int GetAlternateServerID(SqlConnectionInfo connectionInfo, string ServerName)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            try
            {

                using (
                    SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    connection.Open();
                    string commandText =
                        "select SQLServerID from MonitoredSQLServers where lower(RealServerName) = lower(@ServerName) and Active = 1";
                    SqlCommand command = new SqlCommand(commandText, connection);
                    command.Parameters.Add("@ServerName", SqlDbType.NVarChar);
                    command.Parameters["@ServerName"].Value = ServerName;
                    object result = command.ExecuteScalar();
                    connection.Close();
                    if (result == null)
                        return -1;
                    int resultInt = -1;
                    if (Int32.TryParse(result.ToString(), out resultInt))
                    {
                        return resultInt;
                    }
                    else
                    {

                        return -1;
                    }
                }
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public static DataTable GetServerHostNames(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                string commandText = GetServerHostNameCommand;
                SqlCommand command = new SqlCommand(commandText, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                return GetTable(dataReader, true, false);
            }
        }


        public static DataTable GetServerWideStatistics(SqlConnectionInfo connectionInfo, UserView view, DateTime? startTime)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            XmlDocument serversParameter = new XmlDocument();
            XmlElement serversElement = serversParameter.CreateElement("SQLServers");
            serversParameter.AppendChild(serversElement);

            foreach (int id in view.Instances)
            {
                XmlElement instanceElement = serversParameter.CreateElement("SQLServer");
                instanceElement.SetAttribute("SQLServerID", id.ToString());
                serversElement.AppendChild(instanceElement);
            }

            return GetServerWideStatistics(connectionInfo, serversParameter, startTime);
        }

        public static DataTable GetServerWideStatistics(SqlConnectionInfo connectionInfo, Tag tag, DateTime? startTime)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            if (tag == null)
            {
                throw new ArgumentNullException("tag");
            }

            XmlDocument serversParameter = new XmlDocument();
            XmlElement serversElement = serversParameter.CreateElement("SQLServers");
            serversParameter.AppendChild(serversElement);

            foreach (int id in tag.Instances)
            {
                XmlElement instanceElement = serversParameter.CreateElement("SQLServer");
                instanceElement.SetAttribute("SQLServerID", id.ToString());
                serversElement.AppendChild(instanceElement);
            }

            return GetServerWideStatistics(connectionInfo, serversParameter, startTime);
        }

        public static IList<int> GetMostCriticalServers(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<int> mostCriticalServers = new List<int>();

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetMostCriticalServersStoredProcedure))
                {
                    while (dataReader.Read())
                    {
                        mostCriticalServers.Add(Convert.ToInt32(dataReader["SQLServerID"]));
                    }
                }
            }

            return mostCriticalServers;
        }

        public static DataTable GetServerWideStatistics(SqlConnectionInfo connectionInfo, XmlDocument serversXml, DateTime? startTime)
        {
            DataTable mainTable;
            DataTable newTable;
            DataTable finalTable;
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            if (serversXml == null)
            {
                throw new ArgumentNullException("serversXml");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection,
                                                GetServerWideStatisticsStoredProcedure,
                                                serversXml.InnerXml,
                                                1,
                                                null,
                                                startTime.HasValue ? (object)startTime.Value.ToUniversalTime() : null,
                                                null))
                {
                    mainTable = GetTable(dataReader, true, false);
                }
            }
            try // changed for SQLDM 30946, Saurabh
            {
                newTable = CreateNewTableForNewValues(connectionInfo);
                finalTable = HealthIndicesModel.CommonData.JoinDataTables(newTable, mainTable, (row1, row2) => row1.Field<int>("SQLServerID") == row2.Field<int>("SQLServerID"));
            }
            catch (Exception e)
            {
                Log.Debug(e);
                return mainTable;
            }
            return finalTable;
        }

        public static DataTable GetServerWideStatistics(SqlConnectionInfo connectionInfo, DateTime? startTime)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            DataTable mainTable;
            DataTable newTable;
            DataTable finalTable;
            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection,
                                                GetServerWideStatisticsStoredProcedure,
                                                null,
                                                1,
                                                null,
                                                startTime.HasValue ? (object)startTime.Value.ToUniversalTime() : null,
                                                null))
                {
                    mainTable = GetTable(dataReader, true, false);
                }
            }
            try // changed for SQLDM 30946, Saurabh
            {
                newTable = CreateNewTableForNewValues(connectionInfo);
                finalTable = HealthIndicesModel.CommonData.JoinDataTables(newTable, mainTable, (row1, row2) => row1.Field<int>("SQLServerID") == row2.Field<int>("SQLServerID"));
            }
            catch (Exception e)
            {
                Log.Debug(e);
                return mainTable;
            }
            return finalTable;
        }

        private static DataTable CreateNewTableForNewValues(SqlConnectionInfo connectioninfo)
        {
            HealthIndicesHelper summaryHelper = new HealthIndicesHelper();
            IList<HealthIndicesModel.ServerSummaryContainer> container = summaryHelper.GetAllInstancesSummary(connectioninfo);
            //performance enhancement
            try
            {
                if (ApplicationModel.Default.ActiveInstances.Count > 0 && container != null)
                {
                    foreach (var c in container)
                    {
                        if (ApplicationModel.Default.ActiveInstances[c.Overview.SQLServerId] != null) ApplicationModel.Default.ActiveInstances[c.Overview.SQLServerId].BaseHealthIndex = c.ServerStatus.BaseHealthIndex;
                    }
                }
            }
            catch (Exception ex) { } // even if the above call fails, the data table will be created and no exception will be in UI.

            DataTable table = new DataTable();
            //first data coumn must be a unique identifier for both the tables through which merge will happen, need to identify that.As of now adding a dummy identifier name.
            DataColumn columnIdentifier = new DataColumn();
            columnIdentifier.AllowDBNull = false;
            columnIdentifier.ColumnName = "SQLServerID";
            columnIdentifier.DataType = typeof(int);
            table.Columns.Add(columnIdentifier);

            //create as many columns as you want.
            DataColumn columnAlert = new DataColumn();
            columnAlert.AllowDBNull = true;
            columnAlert.ColumnName = "Alert";
            columnAlert.DataType = typeof(int);
            table.Columns.Add(columnAlert);

            DataColumn columnCPU = new DataColumn();
            columnCPU.AllowDBNull = true;
            columnCPU.ColumnName = "CPU";
            columnCPU.DataType = typeof(int);
            table.Columns.Add(columnCPU);

            DataColumn columnIO = new DataColumn();
            columnIO.AllowDBNull = true;
            columnIO.ColumnName = "I/O";
            columnIO.DataType = typeof(int);
            table.Columns.Add(columnIO);

            DataColumn columnMemory = new DataColumn();
            columnMemory.AllowDBNull = true;
            columnMemory.ColumnName = "Memory";
            columnMemory.DataType = typeof(int);
            table.Columns.Add(columnMemory);

            DataColumn columnDatabase = new DataColumn();
            columnDatabase.AllowDBNull = true;
            columnDatabase.ColumnName = "Database";
            columnDatabase.DataType = typeof(int);
            table.Columns.Add(columnDatabase);

            DataColumn columnLogs = new DataColumn();
            columnLogs.AllowDBNull = true;
            columnLogs.ColumnName = "Logs";
            columnLogs.DataType = typeof(int);
            table.Columns.Add(columnLogs);

            DataColumn columnQueries = new DataColumn();
            columnQueries.AllowDBNull = true;
            columnQueries.ColumnName = "Queries";
            columnQueries.DataType = typeof(int);
            table.Columns.Add(columnQueries);

            foreach (HealthIndicesModel.ServerSummaryContainer cont in container)
            {
                var row = table.NewRow();
                row["SQLServerID"] = cont.Overview.SQLServerId;
                row["Alert"] = cont.ServerStatus.MaxSeverity !=null ? cont.ServerStatus.MaxSeverity : 2 ;
                row["CPU"] = cont.AlertCategoryWiseMaxSeverity.Cpu != null ? Convert.ToInt16(cont.AlertCategoryWiseMaxSeverity.Cpu) : 2;
                row["I/O"] = cont.AlertCategoryWiseMaxSeverity.IO != null ? Convert.ToInt16(cont.AlertCategoryWiseMaxSeverity.IO) : 2;
                row["Memory"] = cont.AlertCategoryWiseMaxSeverity.Memory != null ? Convert.ToInt16(cont.AlertCategoryWiseMaxSeverity.Memory) : 2;
                row["Database"] = cont.AlertCategoryWiseMaxSeverity.Databases != null ? Convert.ToInt16(cont.AlertCategoryWiseMaxSeverity.Databases) : 2;
                row["Logs"] = cont.AlertCategoryWiseMaxSeverity.Logs != null ? Convert.ToInt16(cont.AlertCategoryWiseMaxSeverity.Logs) : 2;
                row["Queries"] = cont.AlertCategoryWiseMaxSeverity.Queries != null ? Convert.ToInt16(cont.AlertCategoryWiseMaxSeverity.Queries) : 2;
                table.Rows.Add(row);
            }
            return table;
        }

        private static string getFormatedTags(HealthIndicesModel.ServerSummaryContainer container)
        {
            var tagString = string.Empty;
            if(container != null && container.Overview != null && container.Overview.Tags != null)
            {
                var tags = container.Overview.Tags;
                foreach(string tag in tags)
                {
                    tagString = tagString + tag + ",";
                }
            }
            return tagString.Length > 0 ? tagString.Substring(0,tagString.Length - 1) : string.Empty;
        }

        public static DataTable GetServerOverviewStatistics(SqlConnectionInfo connectionInfo, int id, DateTime startDate, DateTime endDate)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetServerOverviewStatisticsStoredProcedure, id, startDate, endDate))
                {
                    return GetTable(dataReader);
                }
            }
        }

        public static DataTable GetTasks(SqlConnectionInfo connectionInfo, DateTime fromDate, TaskStatus status,
                                         MonitoredStateFlags severity, string instance, string owner)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                Log.DebugFormat(
                    "GetTasks called with fromDate = {0}, status = {1}, severity = {2}, instance = {3}, owner = {4}.",
                    fromDate, status, severity, instance, owner);
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetTasksStoredProcedure, fromDate, status, severity,
                                                instance, owner))
                {
                    DataTable table = GetTable(dataReader);
                    Log.DebugFormat("Stored proc {0} returned {1} records.", GetTasksStoredProcedure, table.Rows.Count);
                    return table;
                }
            }
        }

        public static QueryMonitorData GetQueryMonitorStatements(
            SqlConnectionInfo connectionInfo, QueryMonitorData data, bool getQueriesIcons = false)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            if (data == null || data.Filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            GetPresenceOfIcon(connectionInfo);

            QueryMonitorFilter filter = data.Filter;
            using (
                //SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43572 -- Calling newly created method to get icreased timeout value
                SqlConnection connection =
                    connectionInfo.GetQueryMonitorConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                DateTime? startDate = null;
                DateTime? endDate = null;

                if (filter.StartDate != DateTime.MinValue)
                {
                    startDate = filter.StartDate;
                }

                if (filter.EndDate != DateTime.MinValue)
                {
                    endDate = filter.EndDate;
                }

                DateTime startTime =
                    new DateTime(2007, 1, 1, filter.StartTime.Hours, filter.StartTime.Minutes, filter.StartTime.Seconds);

                DateTime endTime =
                    new DateTime(2007, 1, 1, filter.EndTime.Hours, filter.EndTime.Minutes, filter.EndTime.Seconds);

                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetQueryMonitorStatementsStoredProcedure,
                                                filter.InstanceId,
                                                startDate,
                                                endDate,
                                                startTime,
                                                endTime,
                                                filter.IncludeSqlStatements,
                                                filter.IncludeStoredProcedures,
                                                filter.IncludeSqlBatches,
                                                filter.ApplicationNameExcludeFilter,
                                                filter.ApplicationNameIncludeFilter,
                                                data.ViewMode == QueryMonitorViewMode.Signature ? null : filter.HostNameExcludeFilter,
                                                data.ViewMode == QueryMonitorViewMode.Signature ? null : filter.HostNameIncludeFilter,
                                                filter.DatabaseNameExcludeFilter,
                                                filter.DatabaseNameIncludeFilter,
                                                data.ViewMode == QueryMonitorViewMode.Signature ? null : filter.UserNameExcludeFilter,
                                                data.ViewMode == QueryMonitorViewMode.Signature ? null : filter.UserNameIncludeFilter,
                                                filter.SqlTextExcludeFilter,
                                                filter.SqlTextIncludeFilter,
                                                null, //filter.DurationFilter,  is no longer used
                                                filter.UtcOffset,
                                                (int)data.ViewMode,
                                                data.ViewMode == QueryMonitorViewMode.History ? data.SignatureId : null,
                                                data.ViewMode == QueryMonitorViewMode.History ? data.SignatureHash : null,
                                                data.DetailItemsToReturn,
                                                data.SummaryItemsToReturn,
                                                data.GetFullQueryText)) //-SQLdm 9.0 (Ankit Srivastava) - Defect DE3932 -- Added new parameter for export 
                {
                    DataTable detailTable = GetTable(dataReader);

                    dataReader.NextResult();
                    DataTable summaryTable = GetTable(dataReader);

                    dataReader.NextResult();
                    DataTable resultsTable = GetTable(dataReader);

                    return new QueryMonitorData(data, detailTable, summaryTable, resultsTable, icons);
                }
            }
        }

        public static DataTable GetTable(SqlConnectionInfo connectionInfo, string spName,
                                         params object[] parameterValues)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, spName, parameterValues))
                {
                    return GetTable(dataReader);
                }
            }
        }

        /// <summary>
        /// Get all metrics id and name from a store procedure
        /// </summary>
        /// <param name="connectionInfo">DB connection Information</param>
        /// <param name="spName">store procedure name</param>
        /// <param name="serverListId">Severlist id</param>
        /// <returns></returns>
        public static DataTable GetTable(SqlConnectionInfo connectionInfo, string spName, IList<int> serverListId)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            if (serverListId == null)
            {
                throw new ArgumentNullException("serverListId");
            }

            XmlDocument serversParameters = new XmlDocument();
            XmlElement tagsElement = serversParameters.CreateElement("Servers");
            serversParameters.AppendChild(tagsElement);

            foreach (int serverId in serverListId)
            {
                XmlElement tagElement = serversParameters.CreateElement("Server");
                tagElement.SetAttribute("ServerId", serverId.ToString());
                tagsElement.AppendChild(tagElement);
            }


            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, spName, serversParameters.InnerXml))
                {
                    return GetTable(dataReader);
                }
            }
        }

        private static DataTable GetTable(SqlDataReader dataReader)
        {
            return GetTable(dataReader, true);
        }

        private static DataTable GetTable(SqlDataReader dataReader, bool convertDatesToLocalTime)
        {
            return GetTable(dataReader, convertDatesToLocalTime, null);
        }

        private static DataTable GetTable(SqlDataReader dataReader, bool convertDatesToLocalTime, bool? isColumnReadOnly)
        {
            if (dataReader == null)
            {
                return null;
            }

            List<int> dateColumns = new List<int>();
            DataTable schemaTable = dataReader.GetSchemaTable();
            DataTable dataTable = new DataTable();

            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                if (!dataTable.Columns.Contains(schemaTable.Rows[i]["ColumnName"].ToString()))
                {
                    DataColumn dataColumn = new DataColumn();
                    dataColumn.ColumnName = schemaTable.Rows[i]["ColumnName"].ToString();
                    dataColumn.Unique = Convert.ToBoolean(schemaTable.Rows[i]["IsUnique"]);
                    dataColumn.AllowDBNull = Convert.ToBoolean(schemaTable.Rows[i]["AllowDBNull"]);
                    dataColumn.ReadOnly = isColumnReadOnly.HasValue
                                              ? isColumnReadOnly.Value
                                              : Convert.ToBoolean(schemaTable.Rows[i]["IsReadOnly"]);
                    dataColumn.DataType = schemaTable.Rows[i]["DataType"] as Type;
                    dataTable.Columns.Add(dataColumn);

                    if (convertDatesToLocalTime && dataColumn.DataType == typeof(DateTime))
                        dateColumns.Add(i);
                }
            }

            object[] itemArray = new object[dataReader.FieldCount];

            dataTable.BeginLoadData();
            while (dataReader.Read())
            {
                try
                {
                    dataReader.GetValues(itemArray);
                }
                catch (OverflowException)
                {
                    SafeLoadRow(dataReader, itemArray, dataTable);
                }
                if (dateColumns.Count > 0)
                {
                    foreach (int columnIndex in dateColumns)
                    {
                        if (itemArray[columnIndex] != DBNull.Value)
                        {
                            itemArray[columnIndex] = ((DateTime)itemArray[columnIndex]).ToLocalTime();
                        }
                    }
                }

                dataTable.LoadDataRow(itemArray, true);
            }
            dataTable.EndLoadData();

            return dataTable;
        }

        public static void SafeLoadRow(SqlDataReader reader, object[] itemArray, DataTable dataTable)
        {

            int n = itemArray.Length;
            for (int i = 0; i < n; i++)
            {
                try
                {
                    if (dataTable.Columns[i].DataType == typeof(decimal))
                    {
                        SqlDecimal sqlDecimal = reader.GetSqlDecimal(i);
                        itemArray[i] = sqlDecimal.IsNull ? (object)DBNull.Value : Convert.ToDecimal(sqlDecimal.ToDouble());
                    }
                    else
                        itemArray[i] = reader.GetValue(i);
                }
                catch (OverflowException)
                {
                    itemArray[i] = DBNull.Value;
                }
            }
        }

        public static XmlDocument GetMonitoredSqlServerStatus(SqlConnectionInfo connectionInfo, int? instanceId)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (XmlReader xmlReader =
                    SqlHelper.ExecuteXmlReader(connection, GetMonitoredSqlServerStatusStoredProcedure, instanceId == null ? (object)null : instanceId.Value))
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(xmlReader);
                    return document;
                }
            }
        }


        public static DataSource GetDataSource(SqlConnectionInfo connectionInfo, string spName,
                                               params object[] parameterValues)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, spName, parameterValues))
                {
                    return new DataSource(dataReader);
                }
            }
        }

        public static void LoadDataSource<KeyColumnType>(SqlConnectionInfo connectionInfo,
                                                         DataSourceWithID<KeyColumnType> dataSource, string spName,
                                                         params object[] parameterValues)
        {
            using (Log.VerboseCall())
            {
                if (connectionInfo == null)
                {
                    throw new ArgumentNullException("connectionInfo");
                }

                using (
                    SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    using (
                        SqlDataReader dataReader =
                            SqlHelper.ExecuteReader(connection, spName, parameterValues))
                    {
                        dataSource.Update(dataReader);
                    }
                }
            }
        }

        public static DateTime ExecuteQuery(SqlConnectionInfo connectionInfo, string query)
        {
            using (Log.VerboseCall())
            {
                DateTime minDate = DateTime.Now.Date.ToUniversalTime();

                if (connectionInfo == null)
                {
                    throw new ArgumentNullException("connectionInfo");
                }

                using (
                    SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, CommandType.Text, query);

                    while (dataReader.Read())
                    {
                        minDate = dataReader.GetDateTime(0);
                    }

                }

                return minDate;
            }
        }

        #region Alerts

        public static AlertConfiguration GetDefaultAlertConfiguration(SqlConnectionInfo connectionInfo, int instanceID,
                                                               params int[] metrics)
        {
            return GetAlertConfiguration(connectionInfo, instanceID, true, metrics);
        }

        public static AlertConfiguration GetAlertConfiguration(SqlConnectionInfo connectionInfo, int instanceID,
                                                               params int[] metrics)
        {
            return GetAlertConfiguration(connectionInfo, instanceID, false, metrics);
        }

        private static AlertConfiguration GetAlertConfiguration(SqlConnectionInfo connectionInfo, int instanceID, bool getAlertTemplateConfiguration, params int[] metrics)
        {
            AlertConfiguration config = new AlertConfiguration(instanceID);
            return GetAlertConfiguration(connectionInfo, config, getAlertTemplateConfiguration, metrics);
        }

        public static AlertConfiguration GetAlertConfiguration(SqlConnectionInfo connectionInfo, AlertConfiguration config, bool getAlertTemplateConfiguration, params int[] metrics)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            if (config.MetricDefinitions == null)
                config.MetricDefinitions = new MetricDefinitions(false, false, true, false,
                    ApplicationModel.Default.AllInstances.ContainsKey(config.InstanceID)
                        ? ApplicationModel.Default.AllInstances[config.InstanceID].CloudProviderId
                        : null);
            MetricDefinitions metricDefinitions = config.MetricDefinitions;
            metricDefinitions.Load(connectionInfo.ConnectionString);

            Dictionary<int, List<MetricThresholdEntry>> thresholdEntries = GetMetricThresholds(connectionInfo, config.InstanceID, getAlertTemplateConfiguration);

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

        #endregion

        #region AlertTemplates


        public static List<AlertTemplate> GetAlertTemplateList(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<AlertTemplate> result = new List<AlertTemplate>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, CommandType.Text, GetAlertTemplatesSqlCommand))
                {
                    while (dataReader.Read())
                    {
                        result.Add(new AlertTemplate(dataReader.GetString(1), dataReader.GetString(2), dataReader.GetInt32(0), dataReader.GetBoolean(3)));
                    }
                }
            }
            return result;
        }

        public static List<Triple<string, int, string>> GetAlertThresholdOptions(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<Triple<string, int, string>> result = new List<Triple<string, int, string>>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, GetNewAlertTemplateOptions))
                {
                    while (dataReader.Read())
                    {
                        result.Add(new Triple<string, int, string>(dataReader.GetString(0), dataReader.GetInt32(1), dataReader.GetString(2)));
                    }
                    dataReader.NextResult();
                    while (dataReader.Read())
                    {
                        result.Add(new Triple<string, int, string>(dataReader.GetString(0), dataReader.GetInt32(1), dataReader.GetString(2)));
                    }
                }
            }
            return result;
        }

        public static int GetTemplateID(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlCommand command = SqlHelper.CreateCommand(connection, GetDefaultTemplateID))
                {
                    int defaultTemplateID = -1;
                    SqlHelper.AssignParameterValues(command.Parameters, defaultTemplateID);
                    command.ExecuteNonQuery();
                    SqlParameter outParm = command.Parameters["@DefaultTemplateID"];
                    if (outParm.Value is int)
                        return (int)outParm.Value;
                    else
                    {
                        throw new ArgumentOutOfRangeException("DefaultTemplateID");
                    }
                }
            }
        }

        public static int AddAlertTemplate(SqlConnectionInfo connectionInfo, int sourceType, int SourceID, string name, string description)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            int newTemplateID = 0;
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlCommand command = SqlHelper.CreateCommand(connection, AddAlertTemplateStoredProcedure))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, sourceType, SourceID, name, description, newTemplateID);
                    command.ExecuteNonQuery();
                    SqlParameter outParm = command.Parameters["@templateID"];
                    if (outParm.Value is int)
                        newTemplateID = (int)outParm.Value;
                    return newTemplateID;
                }
            }
        }

        public static void DeleteAlertTemplate(SqlConnectionInfo connectionInfo, int templateID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlCommand command = SqlHelper.CreateCommand(connection, DeleteAlertTemplateStoredProcedure))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, templateID);
                    command.ExecuteNonQuery();
                }
            }
        }

        //
        public static void AddRecommendationForNonSysAdmin(SqlConnectionInfo connectionInfo, int sqlServerID)
        {
            try
            {
                if (connectionInfo == null)
                {
                    throw new ArgumentNullException("connectionInfo");
                }

                using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_AddRecommendationWithZeroCount"))
                    {
                        SqlHelper.AssignParameterValues(command.Parameters, sqlServerID);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        //

        public static void SetDefaultAlertTemplate(SqlConnectionInfo connectionInfo, int templateID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlCommand command = SqlHelper.CreateCommand(connection, SetDefaultAlertTemplateStoredProcedure))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, templateID);
                    command.ExecuteNonQuery();
                }
            }

        }

        public static void UpdateAlertTemplate(SqlConnectionInfo connectionInfo, int templateID, string name, string description)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlCommand command = SqlHelper.CreateCommand(connection, UpdateAlertTemplateStoredProcedure))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, templateID, name, description);
                    command.ExecuteNonQuery();
                }

            }

        }


        #endregion

        public static void UpdateAzureResources(SqlConnectionInfo connectionInfo, IAzureApplicationProfile appProfile, List<IAzureResource> azureResources)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (var connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    try
                    {
                        using (var command = SqlHelper.CreateCommand(connection, DeleteAzureResourceStoredProcedure))
                        {
                            command.Transaction = transaction;
                            SqlHelper.AssignParameterValues(command.Parameters, appProfile.Id);
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("UpdateAzureResources:: Error removing the resources associated with the azure application profile" + appProfile.Name, ex);
                        transaction.Rollback();
                        throw;
                    }
                    using (var command = SqlHelper.CreateCommand(connection, AddAzureResourceStoredProcedure))
                    {
                        command.Transaction = transaction;
                        foreach (var azureResource in azureResources)
                        {
                            long newID = 0;
                            SqlHelper.AssignParameterValues(command.Parameters,
                                appProfile.Id,
                                azureResource.Name,
                                azureResource.Type,
                                azureResource.Uri,
                                newID);
                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("UpdateAzureResources:: Error adding the resources associated with the azure application profile" + appProfile.Name, ex);
                    transaction.Rollback();
                    throw;
                }
            }
        }

        #region Dashboard Layouts


        public static int AddDashboardLayout(SqlConnectionInfo connectionInfo, string userName, string name, DashboardConfiguration configuration, System.Drawing.Bitmap image, bool useAsGlobalDefault)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            int newID = 0;
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, AddDashboardLayoutStoredProcedure))
                    {
                        command.Transaction = transaction;
                        configuration.Name = name;
                        SqlHelper.AssignParameterValues(command.Parameters,
                                                        userName,
                                                        name,
                                                        configuration.Serialize(),
                                                        ImageHelper.ConvertImageToByteArray(image),
                                                        useAsGlobalDefault,
                                                        newID);
                        command.ExecuteNonQuery();
                        SqlParameter outParm = command.Parameters["@newID"];
                        if (outParm.Value is int)
                            newID = (int)outParm.Value;

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }

                return newID;
            }
        }

        public static DashboardConfiguration GetDashboardLayout(SqlConnectionInfo connectionInfo, string userName, int instanceId, out int dashboardLayoutID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetServerDashboardLayoutStoredProcedure, userName, instanceId))
                {
                    DataTable dataTable = GetTable(dataReader);
                    if (dataTable.Rows.Count == 1)
                    {
                        dashboardLayoutID = (int)dataTable.Rows[0]["DashboardLayoutID"];
                        string xmlString = (string)dataTable.Rows[0]["Configuration"];
                        return DashboardConfiguration.Deserialize(xmlString);
                    }
                }
                dashboardLayoutID = 0;
                return DashboardHelper.GetDefaultConfig(new ServerVersion("9.00.00.00"), instanceId);
            }
        }

        public static DashboardConfiguration GetDashboardLayout(SqlConnectionInfo connectionInfo, int dashboardLayoutID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetDashboardLayoutStoredProcedure, dashboardLayoutID))
                {
                    if (dataReader != null && dataReader.HasRows)
                    {
                        DataTable dataTable = GetTable(dataReader);
                        if (dataTable.Rows.Count == 1)
                        {
                            string xmlString = (string)dataTable.Rows[0]["Configuration"];
                            return DashboardConfiguration.Deserialize(xmlString);
                        }
                    }
                }
                return null;
            }
        }

        public static int GetDashboardLayoutID(SqlConnectionInfo connectionInfo, string userName, string name)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetDashboardLayoutIDStoredProcedure, userName, name))
                {
                    if (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(0))
                            return dataReader.GetInt32(0);
                    }
                    return 0;
                }
            }
        }

        public static List<DashboardLayoutSelector> GetDashboardLayouts(SqlConnectionInfo connectionInfo, string userName, DashboardFilter filter)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            Log.DebugFormat("Getting dashboard layouts for {0}.", filter == DashboardFilter.User ? userName : filter.ToString());

            List<DashboardLayoutSelector> layouts = new List<DashboardLayoutSelector>();
            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetDashboardLayoutsStoredProcedure, userName, (int)filter))
                {
                    DataTable dataTable = GetTable(dataReader);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        string name = string.Empty;
                        try
                        {
                            int id = (int)row["DashboardLayoutID"];
                            string owner = row["LoginName"] == DBNull.Value ? null : (string)row["LoginName"];
                            string currentUser = (string)row["currentUser"];
                            name = (string)row["Name"];
                            DateTime? lastUpdated = row["LastUpdated"] == DBNull.Value ? (DateTime?)null : (DateTime)row["LastUpdated"];
                            DateTime? lastViewed = row["LastViewed"] == DBNull.Value ? (DateTime?)null : (DateTime)row["LastViewed"];
                            string xmlString = (string)row["Configuration"];
                            DashboardLayoutSelector layout;
                            Image image;
                            if (owner == null)
                            {
                                // system default images are not stored in the database and are resources in the console instead
                                switch (id)
                                {
                                    case 2:
                                        image = Properties.Resources.SQLdm_Default_2;
                                        break;
                                    case 3:
                                        image = Properties.Resources.SQLdm_Default_3;
                                        break;
                                    default:
                                        image = Properties.Resources.SQLdm_Default;
                                        break;
                                }
                                layout = new DashboardLayoutSelector(currentUser,
                                                                        id,
                                                                        owner,
                                                                        DashboardConfiguration.Deserialize(xmlString),
                                                                        lastUpdated,
                                                                        lastViewed,
                                                                        image);
                            }
                            else if (row["LayoutImage"] == DBNull.Value)
                            {
                                layout = new DashboardLayoutSelector(currentUser,
                                                                        id,
                                                                        owner,
                                                                        DashboardConfiguration.Deserialize(xmlString),
                                                                        lastUpdated,
                                                                        lastViewed,
                                                                        Properties.Resources.SQLdm_Default);
                            }
                            else
                            {
                                layout = new DashboardLayoutSelector(currentUser,
                                                                        id,
                                                                        owner,
                                                                        DashboardConfiguration.Deserialize(xmlString),
                                                                        lastUpdated,
                                                                        lastViewed,
                                                                        (Byte[])row["LayoutImage"]);
                            }
                            layouts.Add(layout);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(string.Format("Error getting dashboard layout configuration {0}", name), ex);
                        }
                    }
                }
            }

            return layouts;
        }

        public static void DeleteDashboardLayout(SqlConnectionInfo connectionInfo, int dashboardLayoutID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, DeleteDashboardLayoutStoredProcedure))
                    {
                        command.Transaction = transaction;
                        SqlHelper.AssignParameterValues(command.Parameters, dashboardLayoutID);
                        command.ExecuteNonQuery();

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public static void SetDefaultDashboardLayout(SqlConnectionInfo connectionInfo, string userName, int? instanceId, int dashboardLayoutID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlCommand command = SqlHelper.CreateCommand(connection, SetDefaultDashboardLayoutStoredProcedure))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, userName, instanceId, dashboardLayoutID);
                    command.ExecuteNonQuery();
                }
            }

        }

        public static void UpdateDashboardLayout(SqlConnectionInfo connectionInfo, int dashboardLayoutID, string name, DashboardConfiguration configuration, System.Drawing.Bitmap image, bool useAsGlobalDefault)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, UpdateDashboardLayoutStoredProcedure))
                    {
                        command.Transaction = transaction;
                        SqlHelper.AssignParameterValues(command.Parameters,
                                                        dashboardLayoutID,
                                                        name,
                                                        configuration.Serialize(),
                                                        ImageHelper.ConvertImageToByteArray(image),
                                                        useAsGlobalDefault);
                        command.ExecuteNonQuery();

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public static void UpdateDashboardLayoutViewed(SqlConnectionInfo connectionInfo, int dashboardLayoutID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                try
                {
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, UpdateDashboardLayoutViewedStoredProcedure))
                    {
                        command.Transaction = transaction;
                        SqlHelper.AssignParameterValues(command.Parameters, dashboardLayoutID);
                        command.ExecuteNonQuery();

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }



        #endregion

        #region Metrics

        public static Dictionary<Metric, MetricInfo> LoadMetricInfo(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            Dictionary<Metric, MetricInfo> result = new Dictionary<Metric, MetricInfo>();
            using (SqlConnection connection =
                connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
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

        public static IList<Metric> GetMetricsByCategory(SqlConnectionInfo connectionInfo, string category)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<Metric> resultList = new List<Metric>();
            string commandString = string.Format("Select Metric from MetricInfo where Category = \'{0}\'", category);

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, CommandType.Text, commandString))
                {
                    while (dataReader.Read())
                    {
                        resultList.Add((Metric)Enum.ToObject(typeof(Metric), dataReader.GetInt32(0)));
                    }
                }
            }

            return resultList;
        }

        public static Dictionary<int, List<MetricThresholdEntry>> GetMetricThresholds(SqlConnectionInfo connectionInfo,
                                                                                   int instanceID, bool getAlertTemplateConfiguration)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }


            Dictionary<int, List<MetricThresholdEntry>> result = null;

            using (SqlConnection connection =
                connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(
                        connection,
                        GetMetricThresholdsStoredProcedure,
                        getAlertTemplateConfiguration ? (object)instanceID : DBNull.Value,
                        instanceID,
                        DBNull.Value))
                {
                    result = GetMetricThresholds(dataReader);
                }
            }

            return result;
        }

        private static Dictionary<int, List<MetricThresholdEntry>> GetMetricThresholds(SqlDataReader reader)
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
                //if (reader.FieldCount > 9)
                //{
                //    entry.MetricInstanceName = reader.IsDBNull(11) ? string.Empty : reader.GetString(11);
                //}
                //else
                //{
                //    entry.MetricInstanceName = reader.IsDBNull(7) ? string.Empty : reader.GetString(7);
                //}

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

        #endregion

        public static LicenseSummary GetLicenseKeys(SqlConnectionInfo connectionInfo)
        {
            using (Log.DebugCall())
            {
                List<string> keys = new List<string>();
                int registeredServers = -1;
                string repositoryInstance = null;

                // Use default connection if null was passed.
                connectionInfo = CheckConnectionInfo(connectionInfo);
                Log.Debug("Using connection info: ", connectionInfo);

                using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_GetLicenseKeys"))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            int fieldId = reader.GetOrdinal("LicenseKey");
                            while (reader.Read())
                            {
                                string key = reader.GetString(fieldId);
                                keys.Add(key);
                            }
                        } // using reader

                        SqlParameter rsc = command.Parameters["@ReturnServerCount"];
                        SqlInt32 sqlValue = (SqlInt32)rsc.SqlValue;
                        if (!sqlValue.IsNull)
                            registeredServers = sqlValue.Value;

                        Log.Debug("Returning registeredServers = ", registeredServers);

                        SqlParameter repository = command.Parameters["@ReturnInstanceName"];
                        SqlString strValue = (SqlString)repository.SqlValue;
                        repositoryInstance = strValue.Value;

                        Log.Debug("Returning repositoryInstance = ", repositoryInstance);
                    } // using command
                }

                return LicenseSummary.SummarizeKeys(
                    registeredServers,
                    repositoryInstance,
                    keys);
            }
        }

        public static DataTable GetTableFragmentation(SqlConnectionInfo connectionInfo, int monitoredServerId, string database)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetTableFragmentationStoredProcedure,
                                                monitoredServerId, database))
                {
                    return GetTable(dataReader);
                }
            }
        }

        public static Dictionary<DateTime, MonitoredSqlServerStatus> GetServerActivityList(SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime start, DateTime end)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            Dictionary<DateTime, MonitoredSqlServerStatus> result = new Dictionary<DateTime, MonitoredSqlServerStatus>();

            using (SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader reader = SqlHelper.ExecuteReader(
                    connection,
                    GetServerActivityStoredProcedure,
                    monitoredServerId,
                    start,
                    end,
                    7,
                    AlertableSnapshotType.ScheduledRefresh))
                {
                    while (reader.Read())
                    {
                        DateTime collectionDateTime = reader.GetDateTime(1).ToLocalTime();

                        if (!reader.IsDBNull(2))
                        {
                            MonitoredSqlServerStatus status = ConstructMonitoredSqlServerStatus(reader.GetString(2));

                            if (status != null)
                            {
                                result.Add(collectionDateTime, status);
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///Changing history behaviour for analysis tab
        /// <param name="connectionInfo"></param>
        /// <param name="monitoredServerId"></param>
        /// <param name="end"></param>
        /// <param name="start"></param>
        /// </summary>
        public static Dictionary<DateTime, MonitoredSqlServerStatus> GetAnalysisActivityList(SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime start, DateTime end)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            Dictionary<DateTime, MonitoredSqlServerStatus> result = new Dictionary<DateTime, MonitoredSqlServerStatus>();

            using (SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader reader = SqlHelper.ExecuteReader(
                    connection,
                    GetAnalysisActivityStoredProcedure,
                    monitoredServerId,
                    start,
                    end,
                    AlertableSnapshotType.ScheduledRefresh))
                {
                    while (reader.Read())
                    {
                        DateTime collectionDateTime = reader.GetDateTime(1).ToLocalTime();

                        //if (!reader.IsDBNull(2))
                        //{
                        //    MonitoredSqlServerStatus status = ConstructMonitoredSqlServerStatus(reader.GetString(2));

                        //    if (status != null)
                        //    {
                        //        result.Add(collectionDateTime, status);
                        //    }
                        //}
                        result.Add(collectionDateTime, MonitoredSqlServerStatus.CreateDesignTimeStatus());
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Prepares snapshot from historical data - SQLDM 10.0.0(Ankit Nagpal)
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <param name="monitoredServerId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static AlwaysOnAvailabilityGroupsSnapshot GetInstanceAvailbilityGroupDetailsData(SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime start, DateTime end)
        {
            try
            {
                using (
               SqlConnection connection =
                   connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetAvailbilityGroups, monitoredServerId,
                                                    start.ToUniversalTime(), end.ToUniversalTime()))

                        return new AlwaysOnAvailabilityGroupsSnapshot(dataReader);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("error occurred in GetInstanceAvailbilityGroupDetailsData. Details: " + ex.Message + ex.StackTrace);
            }

            return null;
        }



        public static DataTable GetTempdbSummaryData(SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime snapshotDateTime, int? historyInMinutes,
            DateTime? startDateTime = null)
        {

            using (
           SqlConnection connection =
               connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(connection, GetTempdbSummaryDataStoredProcedure, monitoredServerId,
                                            snapshotDateTime.ToUniversalTime(),
                                            historyInMinutes != null ? historyInMinutes : MathHelper.GetMinutes(startDateTime, snapshotDateTime)))
                    return GetTable(dataReader);
            }
        }

        public static DataTable GetTempdbFileData(SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime snapshotDateTime, int? historyInMinutes,
            DateTime? startDateTime = null)
        {

            using (
           SqlConnection connection =
               connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(connection, GetTempdbFileDataStoredProcedure, monitoredServerId,
                                            snapshotDateTime.ToUniversalTime(),
                                            historyInMinutes != null ? historyInMinutes : MathHelper.GetMinutes(startDateTime, snapshotDateTime)))
                    return GetTable(dataReader);
            }
        }

        public static Pair<DateTime, MonitoredSqlServerStatus>? GetStateOverview(SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime? searchDateTime)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            if (searchDateTime.HasValue)
            {
                searchDateTime = searchDateTime.Value.ToUniversalTime();
            }

            using (SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader reader = SqlHelper.ExecuteReader(
                    connection,
                    GetStateOverviewStoredProcedure,
                    monitoredServerId,
                    searchDateTime))
                {
                    reader.Read();

                    if (reader.HasRows)
                    {
                        DateTime snapshotDateTime = ((DateTime)reader["UTCCollectionDateTime"]).ToLocalTime();

                        return
                            new Pair<DateTime, MonitoredSqlServerStatus>(
                                snapshotDateTime, ConstructMonitoredSqlServerStatus(reader["StateOverview"] as string));
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        private static MonitoredSqlServerStatus ConstructMonitoredSqlServerStatus(string xmlText)
        {
            MonitoredSqlServerStatus status = null;

            if (xmlText != null)
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlText);

                if (xmlDocument.DocumentElement.HasChildNodes)
                {
                    status = new MonitoredSqlServerStatus(xmlDocument.DocumentElement.FirstChild);
                }
            }

            return status;
        }

        public static List<T> GetServerActivity<T>(SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime? start, DateTime? end)
            where T : class
        {
            using (Log.InfoCall(String.Format("GetServerActivity<{0}>", typeof(T).Name)))
            {
                if (connectionInfo == null)
                    throw new ArgumentNullException("connectionInfo");

                Type resultType = typeof(T);

                int columnMap;
                if (resultType == typeof(SessionSummary))
                    columnMap = 0xAB;
                else if (resultType == typeof(LockDetails))
                    columnMap = 0x63;
                else if (resultType == typeof(SessionSnapshot))
                    columnMap = 0x1B;
                else
                    throw new ArgumentException("result type not supported");

                List<T> result = new List<T>();
                using (SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    using (SqlDataReader reader = SqlHelper.ExecuteReader(
                        connection,
                        GetServerActivityStoredProcedure,
                        monitoredServerId,
                        start.HasValue ? (object)start.Value : null,
                        end.HasValue ? (object)end.Value : null,
                        columnMap,
                        AlertableSnapshotType.ScheduledRefresh))
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                if (resultType == typeof(SessionSummary))
                                    result.Add(new SessionSummary(reader) as T);
                                else if (resultType == typeof(LockDetails))
                                    result.Add(new LockDetails(reader) as T);
                                else if (resultType == typeof(SessionSnapshot))
                                    result.Add(new SessionSnapshot(reader) as T);
                            }
                            catch (Exception e)
                            {
                                Log.WarnFormat("Unable to reconstitute serialized object", e);
                            }
                        }
                    }
                }
                return result;
            }
        }

        public static SessionSnapshot GetSessionsDetails(
            SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime snapshotDateTime, DateTime? startDateTime = null,
            int? historyInMinutes = null)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetSessionsDetailsStoredProcedure,
                                                monitoredServerId, snapshotDateTime.ToUniversalTime(),
                                                historyInMinutes != null ? historyInMinutes : MathHelper.GetMinutes(startDateTime, snapshotDateTime)))
                {
                    dataReader.Read();

                    if (dataReader.HasRows && dataReader["SessionList"] != DBNull.Value)
                    {
                        return
                            new SessionSnapshot((DateTime)dataReader["UTCCollectionDateTime"],
                                                Serialized<object>.DeserializeCompressed
                                                    <Dictionary<Pair<int?, DateTime?>, Session>>(
                                                    (byte[])dataReader["SessionList"]));
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public static IList<SessionSnapshot> GetSessionsDetailsRanged(SqlConnectionInfo connectionInfo, int monitoredServerId,
                                                          DateTime startDateTime, DateTime endDateTime)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            List<SessionSnapshot> sessionSnapshots = new List<SessionSnapshot>();
            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetSessionsDetailsRangedStoredProcedure,
                                                monitoredServerId, startDateTime.ToUniversalTime(), endDateTime.ToUniversalTime()))
                {

                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            if (dataReader["SessionList"] != DBNull.Value)
                            {
                                var newSnapshot = new SessionSnapshot((DateTime)dataReader["UTCCollectionDateTime"],
                                                        Serialized<object>.DeserializeCompressed
                                                            <Dictionary<Pair<int?, DateTime?>, Session>>(
                                                            (byte[])dataReader["SessionList"]));

                                for (int i = 0; i < newSnapshot.SessionList.Count; i++)
                                {
                                    var newSnapshotKey = newSnapshot.SessionList.Keys.ElementAt(i);
                                    var newSnapshotItem = newSnapshot.SessionList[newSnapshotKey];
                                    if (newSnapshotItem.LastActivity < startDateTime || newSnapshotItem.LastActivity > endDateTime ||
                                        newSnapshotKey.Second < startDateTime || newSnapshotKey.Second > endDateTime)
                                    {
                                        newSnapshot.SessionList.Remove(newSnapshotKey);
                                        i--;
                                        continue;
                                    }
                                    foreach (var snapShot in sessionSnapshots)
                                    {
                                        var isRemoved = false;
                                        foreach (var sessionPair in snapShot.SessionList)
                                        {
                                            if (newSnapshotKey.Second.Value == sessionPair.Key.Second.Value &&
                                                sessionPair.Value.UserName == newSnapshotItem.UserName &&
                                                sessionPair.Value.Workstation == newSnapshotItem.Workstation &&
                                                sessionPair.Value.Application == newSnapshotItem.Application &&
                                                sessionPair.Value.LoggedInSince == newSnapshotItem.LoggedInSince &&
                                                sessionPair.Value.LastActivity == newSnapshotItem.LastActivity)
                                            {
                                                newSnapshot.SessionList.Remove(newSnapshotKey);
                                                isRemoved = true;
                                                i--;
                                                break;
                                            }
                                        }
                                        if (isRemoved || sessionSnapshots.Count == 0)
                                        {
                                            break;
                                        }
                                    }
                                }
                                sessionSnapshots.Add(newSnapshot);
                            }
                        }
                    }
                }
                return sessionSnapshots;
            }
        }

        //START SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements -Added new procedure for getting the history disk size details
        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Fetches the disk size details based on start/end/minutes values.
        /// </summary>
        public static DiskSizeDetails GetDiskSizeDetails(
            SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime snapshotDateTime, int? historyInMinutes, DateTime? startDateTime = null)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            DiskSizeDetails snapshot = new DiskSizeDetails(connectionInfo);
            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetDiskSizeDetailsStoredProcedure, monitoredServerId,
                    snapshotDateTime.ToUniversalTime(),
                    historyInMinutes != null ? historyInMinutes : MathHelper.GetMinutes(startDateTime, snapshotDateTime)))
                {

                    snapshot.DiskDrives = snapshot.DiskDrives ?? new Dictionary<string, DiskDriveStatistics>();
                    while (dataReader.Read())
                    {
                        int ordinalDrive = dataReader.GetOrdinal("DriveName");
                        if (!(dataReader.IsDBNull(ordinalDrive)))
                        {
                            var driveName = dataReader.GetString(ordinalDrive);
                            DiskDriveStatistics diskDrive = new DiskDriveStatistics();
                            if (!snapshot.DiskDrives.ContainsKey(driveName))
                                snapshot.DiskDrives.Add(driveName, diskDrive);
                            else
                                diskDrive = snapshot.DiskDrives[driveName];

                            diskDrive.SQLServerID = monitoredServerId;
                            diskDrive.DriveName = driveName;
                            diskDrive.UTCCollectionDateTime = Convert.ToDateTime(dataReader["UTCCollectionDateTime"]);
                            diskDrive.UnusedSizeKB = Convert.ToDouble(dataReader["UnusedSizeKB"]);
                            diskDrive.TotalSizeKB = Convert.ToDouble(dataReader["TotalSizeKB"]);

                            snapshot.DiskDrives[driveName] = diskDrive;
                        }
                    }
                    dataReader.NextResult();
                    while (dataReader.Read())
                    {
                        int ordinal = dataReader.GetOrdinal("DriveName");
                        if (!(dataReader.IsDBNull(ordinal)))
                        {

                            var driveName = dataReader.GetString(ordinal);
                            if (snapshot.DiskDrives.ContainsKey(driveName))
                            {
                                var drive = snapshot.DiskDrives[driveName];


                                var InitialSize = Convert.ToDouble(dataReader["InitialSize"]);
                                var UsedSpace = Convert.ToDouble(dataReader["UsedSpace"]);
                                var AvailableSpace = Convert.ToDouble(dataReader["AvailableSpace"]);

                                var isDataFile = Convert.ToBoolean(dataReader["IsDataFile"]);
                                if (isDataFile)
                                {
                                    drive.SQLDataFreeMB = (drive.SQLDataFreeMB == null) ? AvailableSpace : (drive.SQLDataFreeMB + AvailableSpace);
                                    drive.SQLDataUsedMB = (drive.SQLDataUsedMB == null) ? UsedSpace : (drive.SQLDataUsedMB + UsedSpace);
                                }
                                else
                                    drive.SQLLogFileMB = (drive.SQLLogFileMB == null) ? InitialSize : (drive.SQLLogFileMB + InitialSize);


                                snapshot.DiskDrives[driveName] = drive;

                            }
                        }
                    }
                }
            }
            return snapshot;
        }
        //END SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements -Added new procedure for getting the history disk size details

        public static Pair<LockDetails, DataTable> GetLocksDetails(
            SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime snapshotDateTime, int? historyInMinutes, DateTime? startDateTime = null)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetLocksDetailsStoredProcedure, monitoredServerId,
                                                snapshotDateTime.ToUniversalTime(),
                                                historyInMinutes != null ? historyInMinutes : MathHelper.GetMinutes(startDateTime, snapshotDateTime)))
                {
                    dataReader.Read();

                    LockDetails lockDetails = null;

                    if (dataReader.HasRows && dataReader["LockList"] != DBNull.Value)
                    {
                        lockDetails =
                            new LockDetails((DateTime)dataReader["UTCCollectionDateTime"],
                                            Serialized<object>.DeserializeCompressed<Dictionary<Guid, Lock>>(
                                                (byte[])dataReader["LockList"]));
                    }

                    dataReader.NextResult();
                    return new Pair<LockDetails, DataTable>(lockDetails, GetTable(dataReader));
                }
            }
        }

        public static Triple<SessionSnapshot, LockDetails, DataTable> GetBlockingDetails(
            SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime snapshotDateTime, int? historyInMinutes, DateTime? startDateTime = null)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetBlockingDetailsStoredProcedure, monitoredServerId,
                                                snapshotDateTime.ToUniversalTime(),
                                                historyInMinutes != null ? historyInMinutes : MathHelper.GetMinutes(startDateTime, snapshotDateTime)))
                {
                    dataReader.Read();

                    SessionSnapshot sessionSnapshot = null;
                    LockDetails lockDetails = null;

                    if (dataReader.HasRows)
                    {
                        if (dataReader["SessionList"] != DBNull.Value)
                        {
                            sessionSnapshot =
                                new SessionSnapshot((DateTime)dataReader["UTCCollectionDateTime"],
                                                    Serialized<object>.DeserializeCompressed
                                                        <Dictionary<Pair<int?, DateTime?>, Session>>(
                                                        (byte[])dataReader["SessionList"]));
                        }

                        if (dataReader["LockList"] != DBNull.Value)
                        {
                            lockDetails =
                                new LockDetails((DateTime)dataReader["UTCCollectionDateTime"],
                                                Serialized<object>.DeserializeCompressed<Dictionary<Guid, Lock>>(
                                                    (byte[])dataReader["LockList"]));
                        }
                    }

                    dataReader.NextResult();

                    return
                        new Triple<SessionSnapshot, LockDetails, DataTable>(sessionSnapshot, lockDetails,
                                                                            GetTable(dataReader));
                }
            }
        }

        public static ServerSummaryHistoryDataContainer GetServerSummary(
            SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime snapshotDateTime, int? historyInMinutes, DateTime? startDateTime = null)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetServerSummaryStoredProcedure, monitoredServerId,
                                                snapshotDateTime.ToUniversalTime(),
                                                historyInMinutes != null ? historyInMinutes : MathHelper.GetMinutes(startDateTime, snapshotDateTime)))
                {
                    ServerSummaryHistoryDataContainer container = new ServerSummaryHistoryDataContainer();

                    dataReader.Read();

                    MonitoredSqlServerStatus status = null;

                    if (dataReader.HasRows && !dataReader.IsDBNull(0))
                    {
                        status = ConstructMonitoredSqlServerStatus(dataReader.GetString(0));
                    }

                    container.ServerStatus = status;
                    dataReader.NextResult();
                    container.OverviewStatistics = GetTable(dataReader);

                    dataReader.NextResult();
                    List<string> diskDrives = new List<string>();
                    while (dataReader.Read())
                    {
                        diskDrives.Add(dataReader["DriveName"] as string);
                    }

                    dataReader.NextResult();
                    DataTable diskDriveMetrics = GetTable(dataReader);

                    container.DiskDriveInfo = new Pair<List<string>, DataTable>(diskDrives, diskDriveMetrics);

                    return container;
                }
            }
        }

        //SQLdm 10.0 srishti purohit recommendation history
        //SQLdm 10.5.2 changes for plateform specific recommendation history
        public static List<IRecommendation> GetRecommendationHistory(SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime snapshotDateTime, int? historyInMinutes)
        {
            List<IRecommendation> listOfRecommendation = new List<IRecommendation>();
            Recommendation recommendationRow = null;
            try
            {
                using (
                    SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    using (
                        SqlDataReader reader =
                            SqlHelper.ExecuteReader(connection, GetRecommendationHisotry,
                                                    monitoredServerId,
                                                    snapshotDateTime.ToUniversalTime()))//, historyInMinutes))
                    {
                        Log.Info("Getting all saved recommendations from database history for serverID : " + monitoredServerId + " for all analysis completed in last : " + historyInMinutes + " from analysis compeltion time : " + snapshotDateTime + " .");

                        List<string> blockedRecommendation = SqlHelper.GetBlockedRecommendations(connectionInfo, monitoredServerId);

                        // changes for platform specific recommendation history
                        var targetPlatformAllowedRecommendations = SqlHelper.GetTotalRecommendations(connectionInfo, monitoredServerId);

                        List<Triple<int, string, string>> recommendationPropertiesWithRecommIDs = new List<Triple<int, string, string>>();
                        Triple<int, string, string> property;
                        // populate these recommendationProperties from DB

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

                        reader.NextResult();
                        Dictionary<string, string> recommendationProperties;

                        while (reader.Read())
                        {
                            string recommendationID = reader.GetString(0);
                            recommendationProperties = new Dictionary<string, string>();
                            foreach (Triple<int, string, string> propertyItem in recommendationPropertiesWithRecommIDs)
                            {
                                if (propertyItem.First == reader.GetInt32(4))
                                {
                                    if (!recommendationProperties.ContainsKey(propertyItem.Second))
                                        recommendationProperties.Add(propertyItem.Second, propertyItem.Third);
                                }
                            }

                            try
                            {
                                recommendationRow = RecommendationFactory.GetRecommendation(recommendationID, recommendationProperties);
                            }
                            catch (Exception)
                            {
                                Log.Error("Caught Exception in GetRecommendation");
                            }

                            if (recommendationRow != null)
                            {
                                //recommendationRow = new Recommendation(FindingIdAttribute.GetRecommendationTypes(reader.GetString(0))[0]);
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

                                //if (reader.GetInt32(18))
                                //{
                                //    ((RecommendationsList)recommSnapshot.RecommendationList[reader.GetInt32(19)]).Recommendations.Add(recommendationRow);
                                //}
                                //else
                                //{
                                //    listOfRecomm = new RecommendationsList(reader.GetDateTime(18), recommendationRow);
                                //    listOfRecomm.AnalysisTime = reader.GetDateTime(18);
                                //    listOfRecomm.AnalysisID = reader.GetInt32(19);
                                //    recommSnapshot.RecommendationList.Add(listOfRecomm.AnalysisID, listOfRecomm);
                                //}

                                // totalRecommendationAllowed contains the list of recommendations allowed for the platform
                                if (!targetPlatformAllowedRecommendations.Contains(recommendationRow.ID))
                                {
                                    Log.Info("Skipping the recommendation: " + recommendationRow.ID + " .");
                                    continue;
                                }

                                if (blockedRecommendation != null && blockedRecommendation.Count > 0)
                                {
                                    if (!blockedRecommendation.Contains(recommendationRow.ID))
                                        listOfRecommendation.Add(recommendationRow);
                                }
                                else
                                {
                                    listOfRecommendation.Add(recommendationRow);
                                }
                            }
                            Log.Info("Total recommendations fetched from history records : " + listOfRecommendation.Count + " .");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex.Message + "SQL Error occured in GetRecommendationsHistory.");
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error occured in GetRecommendationsHistory : ", ex);
                throw new Exception(ex.Message);
            }
            return listOfRecommendation;
        }

        #region Analysis
        //SQLdm 10.0 srishti purohit Change Flag
        public static bool ChangeFlagOfRecommendation(
            SqlConnectionInfo connectionInfo, int analysisRecommendationID, bool flag)
        {
            bool isFlagChanged = false;
            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                if (connection != null)
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, UpdateFlagStatusPrescriptiveAnalysisRecommendation))
                    {
                        command.Parameters.Clear();

                        command.Parameters.AddWithValue("@analysisRecommendationID", analysisRecommendationID);
                        command.Parameters.AddWithValue("@flag", flag);
                        command.Parameters.Add("@isFlagChanged", SqlDbType.Bit);
                        command.Parameters["@isFlagChanged"].Direction = ParameterDirection.Output;
                        command.ExecuteNonQuery();
                        isFlagChanged = Convert.ToBoolean(command.Parameters["@isFlagChanged"].Value);
                    }
                }
            }


            return isFlagChanged;
        }
        //SQLdm 10.0 srishti purohit recommendation categories
        /// <summary>
        /// Get all the recommendation categories with hierarchy
        /// </summary>
        /// <param name="connectionInfo">connection details</param>
        /// <param name="sqlServerId">monitored server id</param>
        /// <returns>recommendation count and list of recommendations</returns>
        /// <remarks>
        /// Passing null value for <paramref name="sqlServerId"/> will get all the categories
        /// </remarks>
        public static Pair<int, DataTable> GetRecommendationCategories(SqlConnectionInfo connectionInfo, int? sqlServerId = null, int? cloudProviderId = null)
        {
            int categoryCount = 0;
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            try
            {
                using (
                    SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    using (
                        SqlDataReader dataReader =
                            SqlHelper.ExecuteReader(connection, GetAllCategoriesStoredProcedure, sqlServerId, cloudProviderId))
                    {
                        if (dataReader.Read())
                        {
                            categoryCount = Convert.ToInt32(dataReader["categoryCount"]);
                            if (dataReader.NextResult())
                            {

                                DataTable categoryMasterData = GetTable(dataReader);

                                return
                                   new Pair<int, DataTable>(categoryCount, categoryMasterData);
                            }
                            else
                            {
                                throw new Exception("Counld not read data from database for categories details.");
                            }
                        }
                        else
                        {
                            throw new Exception("Counld not read data from database for categories details.");
                        }
                    }
                    return new Pair<int, DataTable>(0, null);
                }
            }
            catch (SqlException ex)
            {
                Log.ErrorFormat("SQL Error occured in GetRecommendationCategories : ", ex);
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error occured in GetRecommendationCategories : ", ex);
                throw new Exception(ex.Message);
            }

        }
        #endregion

        public static DataTable GetDatabaseCounters(
            SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime snapshotDateTime, int? historyInMinutes, DateTime? startDateTime = null)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            Dictionary<String, DatabaseStatistics> dbStatistics = new Dictionary<String, DatabaseStatistics>();
            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetDatabaseCountersStoredProcedure, monitoredServerId,
                                                snapshotDateTime.ToUniversalTime(),
                                                historyInMinutes != null ? historyInMinutes : MathHelper.GetMinutes(startDateTime, snapshotDateTime)))
                {
                    DataTable databaseStatistics = GetTable(dataReader);

                    return
                       databaseStatistics;
                }
            }
        }
        #region Snapshot

        public static DateTime? GetPreviousServerActivitySnapshotDateTime(
            SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime snapshotDateTime)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection,
                                                GetPreviousServerActivitySnapshotDateTimeStoredProcedure,
                                                monitoredServerId,
                                                snapshotDateTime.ToUniversalTime(),
                                                AlertableSnapshotType.ScheduledRefresh))
                {
                    dataReader.Read();

                    if (dataReader.HasRows && !dataReader.IsDBNull(0))
                    {
                        return Convert.ToDateTime(dataReader[0]).ToLocalTime();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        public static DateTime? GetPreviousAnalysisActivitySnapshotDateTime(
            SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime snapshotDateTime)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection,
                                                GetPreviousAnalysisActivitySnapshotDateTimeStoredProcedure,
                                                monitoredServerId,
                                                snapshotDateTime.ToUniversalTime()))
                {
                    dataReader.Read();

                    if (dataReader.HasRows && !dataReader.IsDBNull(0))
                    {
                        return Convert.ToDateTime(dataReader[0]).ToLocalTime();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        public static DateTime? GetNextServerActivitySnapshotDateTime(
            SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime snapshotDateTime)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection,
                                                GetNextServerActivitySnapshotDateTimeStoredProcedure,
                                                monitoredServerId,
                                                snapshotDateTime.ToUniversalTime(),
                                                AlertableSnapshotType.ScheduledRefresh))
                {
                    dataReader.Read();

                    if (dataReader.HasRows && !dataReader.IsDBNull(0))
                    {
                        return Convert.ToDateTime(dataReader[0]).ToLocalTime();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        //For analysis tab
        public static DateTime? GetNextAnalysisActivitySnapshotDateTime(
            SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime snapshotDateTime)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection,
                                                GetNextAnalysisActivitySnapshotDateTimeStoredProcedure,
                                                monitoredServerId,
                                                snapshotDateTime.ToUniversalTime()))
                {
                    dataReader.Read();

                    if (dataReader.HasRows && !dataReader.IsDBNull(0))
                    {
                        return Convert.ToDateTime(dataReader[0]).ToLocalTime();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        #endregion

        #region Counters

        public static string[] GetCounterCategories(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            List<string> result = new List<string>();
            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetCounterCategoriesProcedure))
                {
                    while (dataReader.Read())
                    {
                        result.Add(dataReader.GetString(0));
                    }
                }
            }
            return result.ToArray();
        }
		
		 public static List<int> GetCustomCounterList(SqlConnectionInfo connectionInfo, int serverId)	
        {	
            if (connectionInfo == null)	
            {	
                throw new ArgumentNullException("connectionInfo");	
            }	
            List<int> result = new List<int>();	
            using (	
                SqlConnection connection =	
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))	
            {	
                using (	
                    SqlDataReader dataReader =	
                        SqlHelper.ExecuteReader(connection, GetCustomCountersForTemplate,serverId))	
                {	
                    while (dataReader.Read())	
                    {	
                        result.Add(dataReader.GetInt32(0));	
                    }	
                }	
            }	
            return result;	
        }

        public static Set<int> GetInstancesMonitoringCustomCounter(SqlConnectionInfo connectionInfo, int metricID)
        {
            if (connectionInfo == null)
                throw new ArgumentNullException("connectionInfo");

            Set<int> result = new Set<int>();
            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetInstancesMonitoringCustomCounterProcedure, metricID))
                {
                    while (dataReader.Read())
                    {
                        result.Add(dataReader.GetInt32(0));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Return dictionary containing lists of custom counters being monitored for each instance.
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <param name="monitoredServerId">pass null for all servers or specific instance id for just one server</param>
        /// <returns></returns>
        public static Dictionary<int, List<int>> GetMonitoredServerCustomCounters(SqlConnectionInfo connectionInfo, int? monitoredServerId, bool includeTaggedCounters)
        {
            if (connectionInfo == null)
                throw new ArgumentNullException("connectionInfo");

            Dictionary<int, List<int>> result = new Dictionary<int, List<int>>();
            List<int> list = null;
            int lastInstance = -1;
            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetMonitoredSQLServerCountersProcedure, monitoredServerId, includeTaggedCounters))
                {
                    while (dataReader.Read())
                    {
                        int currentInstance = dataReader.GetInt32(0);
                        if (lastInstance != currentInstance)
                        {
                            lastInstance = currentInstance;
                            list = new List<int>();
                            result.Add(currentInstance, list);
                        }
                        list.Add(dataReader.GetInt32(1));
                    }
                }
            }
            return result;
        }

        public static DataTable GetCustomCounterStatistics(SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime snapshotDateTime,
            int? historyInMinutes, DateTime? startDateTime = null)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            DataTable dataTable = new DataTable("CustomCounterStatistics");
            dataTable.Columns.Add("InstanceName", typeof(string)).AllowDBNull = false;
            dataTable.Columns.Add("CollectionDateTime", typeof(DateTime)).AllowDBNull = false;
            dataTable.Columns.Add("MetricID", typeof(int)).AllowDBNull = false;
            dataTable.Columns.Add("TimeDeltaInSeconds", typeof(double)).AllowDBNull = true;
            dataTable.Columns.Add("RawValue", typeof(decimal)).AllowDBNull = true;
            dataTable.Columns.Add("DeltaValue", typeof(decimal)).AllowDBNull = true;
            dataTable.Columns.Add("DisplayValue", typeof(decimal)).AllowDBNull = true;
            dataTable.Columns.Add("ErrorMessage", typeof(string)).AllowDBNull = true;

            using (
                    SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetCustomCounterStatisticsProcedure, monitoredServerId,
                                                snapshotDateTime.ToUniversalTime(),
                                                historyInMinutes != null ? historyInMinutes : MathHelper.GetMinutes(startDateTime, snapshotDateTime), true))
                {
                    dataTable.BeginLoadData();
                    object[] firstFour = new object[4];
                    object[] columnArray = new object[dataTable.Columns.Count];
                    while (dataReader.Read())
                    {
                        dataReader.GetValues(firstFour);
                        firstFour[1] = ((DateTime)firstFour[1]).ToLocalTime();
                        firstFour.CopyTo(columnArray, 0);

                        columnArray[4] = GetDecimalValue(dataReader.GetSqlDecimal(4));
                        columnArray[5] = GetDecimalValue(dataReader.GetSqlDecimal(5));
                        columnArray[7] = dataReader.GetValue(6);

                        //calculate the display value
                        if (columnArray[7] == DBNull.Value)     // ErrorMessage
                        {
                            CustomCounterDefinition definition =
                                ApplicationModel.Default.MetricDefinitions.GetCounterDefinition((int)firstFour[2]);
                            if (definition != null && definition.IsEnabled)
                            {
                                decimal? value = null;
                                try
                                {
                                    if (definition.CalculationType == CalculationType.Value)
                                    {
                                        if (columnArray[4] is decimal)  // RawValue
                                        {
                                            value = Convert.ToDecimal(definition.Scale) * (decimal)columnArray[4];
                                        }
                                    }
                                    else
                                    {
                                        if (columnArray[5] is decimal)  // DeltaValue
                                        {
                                            if (firstFour[3] is double)  // TimeDeltaInSeconds
                                            {
                                                decimal timeDelta = Convert.ToDecimal((double)firstFour[3]);
                                                if (timeDelta > 0)
                                                {
                                                    value = Convert.ToDecimal(definition.Scale) *
                                                                  (((decimal)columnArray[5]) / timeDelta);
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.ErrorFormat("Error scaling custom counter {0} values. {1}",
                                            definition.CounterName, ex);
                                    throw;
                                }

                                if (value.HasValue)
                                {
                                    columnArray[6] = value.Value;
                                }
                            }
                        }
                        dataTable.LoadDataRow(columnArray, true);
                    }
                    dataTable.EndLoadData();
                }
            }
            return dataTable;
        }

        //START: SQLdm 10.0 (Tarun Sapra) - Get all the baselines data for every metric defined in 'MetricBaselineMap'
        public static DataTable FillBaselineData(SqlConnectionInfo connectionInfo, int serverId, DateTime endDate, int historyInSeconds, double dayDifferenceAfterUTCConversion)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("MetricID", typeof(double)).AllowDBNull = false;
            dataTable.Columns.Add("Value", typeof(double)).AllowDBNull = false;
            dataTable.Columns.Add("Date", typeof(DateTime)).AllowDBNull = false;

            using (
                    SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetAllBaselinesDataStoredProcedure, serverId, endDate, historyInSeconds, dayDifferenceAfterUTCConversion))
                {
                    dataTable.BeginLoadData();

                    while (dataReader.Read())
                    {
                        if (dataReader.HasRows)
                        {
                            DataRow row = dataTable.NewRow();
                            row["MetricID"] = dataReader[0];
                            row["Value"] = dataReader[1];
                            row["Date"] = Convert.ToDateTime(dataReader[2]).ToLocalTime();
                            dataTable.Rows.Add(row);
                        }
                    }
                    dataTable.EndLoadData();
                }
            }
            return dataTable;
        }
        //END: SQLdm 10.0 (Tarun Sapra) - Get all the baselines data for every metric defined in 'MetricBaselineMap'

        private static object GetDecimalValue(SqlDecimal sqlDecimal)
        {
            if (!sqlDecimal.IsNull)
            {
                try
                {
                    return sqlDecimal.Value;
                }
                catch
                {
                    if (sqlDecimal.Scale > 0)
                    {
                        try
                        {
                            int newscale = sqlDecimal.Scale - (sqlDecimal.Precision - 29);
                            if (newscale < 0)
                                newscale = 0;
                            sqlDecimal = SqlDecimal.ConvertToPrecScale(sqlDecimal,
                                                                        sqlDecimal.Precision - (sqlDecimal.Scale - newscale),
                                                                        newscale);
                            return sqlDecimal.Value;
                        }
                        catch
                        {

                        }
                    }
                }
            }

            return DBNull.Value;
        }

        public static bool IsCustomCounterNameAvailable(SqlConnectionInfo connectionInfo, string counterName)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlCommand command = SqlHelper.CreateCommand(connection, GetCounterNameAvailableStoredProcedure))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, counterName, null);
                    command.ExecuteNonQuery();
                    SqlParameter outParm = command.Parameters["@Available"];
                    if (outParm.Value is bool)
                        return (bool)outParm.Value;
                    return false;
                }
            }
        }
        //START SQLdm 10.0 (Swati Gogia): Import Export Wizard
        public static bool IsNotificationRuleNameAvailable(SqlConnectionInfo connectionInfo, string ruleName)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlCommand command = SqlHelper.CreateCommand(connection, GetRuleNameAvailableStoredProcedure))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, ruleName, null);
                    command.ExecuteNonQuery();
                    SqlParameter outParm = command.Parameters["@Available"];
                    if (outParm.Value is bool)
                        return (bool)outParm.Value;
                    return false;
                }
            }
        }
        //END

        #endregion

        public static string GetRepositoryUser(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
                throw new ArgumentNullException("connectionInfo");

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = GetRepositoryUserSql;
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                    }
                }
            }

            return Environment.UserName;
        }

        public static DataTable GetSnoozedAlerts(SqlConnectionInfo connectionInfo, int monitoredServerId)
        {
            if (connectionInfo == null)
                throw new ArgumentNullException("connectionInfo");

            return GetTable(connectionInfo, GetSnoozedAlertsStoredProcedure, monitoredServerId);
        }

        /// <summary>
        /// call get table with list of servers
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <param name="severListId"></param>
        /// <returns></returns>
        public static DataTable GetAllTableSnoozedAlerts(SqlConnectionInfo connectionInfo, IList<int> severListId)
        {
            if (connectionInfo == null)
                throw new ArgumentNullException("connectionInfo");

            return GetTable(connectionInfo, GetAllMetricAlertsNameProcedure, severListId);
        }

        public static ICollection<Tag> GetTags(SqlConnectionInfo connectionInfo)
        {

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
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
                    stopWatch.Stop();
                    StartUpTimeLog.DebugFormat("Time taken by RepositoryHelper.GetTags : {0}", stopWatch.ElapsedMilliseconds);
                    return new List<Tag>(tags.Values);
                }
            }
        }

        public static Triple<IDictionary<int, string>, IList<int>, IList<int>>
            GetTagConfiguration(SqlConnectionInfo connectionInfo, int tagId)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetTagConfigurationStoredProcedure, tagId))
                {
                    Dictionary<int, string> servers = new Dictionary<int, string>();
                    List<int> customCounters = new List<int>();
                    List<int> permissions = new List<int>();

                    while (dataReader.Read())
                    {
                        servers.Add((int)dataReader["SQLServerId"], dataReader["InstanceName"] as string);
                    }

                    dataReader.NextResult();

                    while (dataReader.Read())
                    {
                        customCounters.Add((int)dataReader["Metric"]);
                    }

                    dataReader.NextResult();

                    while (dataReader.Read())
                    {
                        permissions.Add((int)dataReader["PermissionId"]);
                    }

                    return
                        new Triple<IDictionary<int, string>, IList<int>, IList<int>>(servers, customCounters,
                                                                                     permissions);
                }
            }
        }

        public static IDictionary<int, string> GetServersWithTag(SqlConnectionInfo connectionInfo, int tagId)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                SqlHelper.ExecuteReader(connection, GetServersWithTagIdStoredProcedure, tagId))
                {
                    Dictionary<int, string> servers = new Dictionary<int, string>();

                    while (dataReader.Read())
                    {
                        servers.Add((int)dataReader["SQLServerId"], dataReader["InstanceName"] as string);
                    }

                    return servers;
                }
            }
        }

        public static IDictionary<string, int> GetServerTags(SqlConnectionInfo connectionInfo, int serverId)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
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
        /// <summary>
        /// gets all of the monitored servers that have mirroring relationships defined for stored databases
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <returns></returns>
        public static List<String> GetMonitoredMirroredServers(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, GetMirroredServersStoredProcedure))
                {
                    List<string> servers = new List<string>();

                    while (dataReader.Read())
                    {
                        servers.Add(dataReader["InstanceName"] as string);
                    }

                    return servers;
                }
            }

        }

        public static List<ValueListItem> GetMirroredDatabases(SqlConnectionInfo connectionInfo, string permittedServerIDs)
        {
            return GetMirroredDatabases(connectionInfo, permittedServerIDs, false);
        }

        /// <summary>
        /// Get all mirrored databases
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <param name="permittedServerIDs"></param>
        /// <param name="addSelectRequest"></param>
        /// <returns>List of allowed databases</returns>
        public static List<ValueListItem> GetMirroredDatabases(SqlConnectionInfo connectionInfo, string permittedServerIDs, bool addSelectRequest)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            int intselectionRequest = 0;
            intselectionRequest = addSelectRequest ? 1 : 0;

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetMirroredDatabasesStoredProcedure, permittedServerIDs, intselectionRequest))
                {
                    List<ValueListItem> databases = new List<ValueListItem>();

                    while (dataReader.Read())
                    {
                        databases.Add(new ValueListItem(dataReader["DatabaseID"], dataReader["DatabaseName"].ToString()));
                    }

                    return databases;
                }
            }
        }

        public static Triple<bool, IList<int>, IList<int>> GetPermissionTagsAndServers(
            SqlConnectionInfo connectionInfo, int permissionId)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
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

        public static Pair<IList<int>, IList<int>> GetCustomCounterTagsAndServers(
            SqlConnectionInfo connectionInfo, int metricId)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetCustomCounterTagsAndServersStoredProcedure, metricId))
                {
                    List<int> tags = new List<int>();
                    List<int> servers = new List<int>();

                    while (dataReader.Read())
                    {
                        tags.Add((int)dataReader[0]);
                    }

                    dataReader.NextResult();
                    while (dataReader.Read())
                    {
                        servers.Add((int)dataReader[0]);
                    }

                    return new Pair<IList<int>, IList<int>>(tags, servers);
                }
            }
        }

        public static MultiDictionary<int, int> GetCustomCountersWithTagIds(
            SqlConnectionInfo connectionInfo, IEnumerable<int> tagIds)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            if (tagIds == null)
            {
                throw new ArgumentNullException("tagIds");
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

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetCustomCountersWithTagIdsStoredProcedure, tagsParameter.InnerXml))
                {
                    MultiDictionary<int, int> customCounterLookupTable = new MultiDictionary<int, int>(false);

                    while (dataReader.Read())
                    {
                        customCounterLookupTable.Add((int)dataReader["Metric"], (int)dataReader["TagId"]);
                    }

                    return customCounterLookupTable;
                }
            }
        }

        //SQLDM-29697
        public static void UpdateCustomCounters(List<int> servers)
        {
            foreach (int serverId in servers)
            {

                SqlTransaction xa = null;
                SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
                Dictionary<int, List<int>> customCounters = RepositoryHelper.GetMonitoredServerCustomCounters(connectionInfo, serverId, true);
                List<int> counters = new List<int>();
                customCounters.TryGetValue(serverId, out counters);

                XmlDocument xmlDoc = new XmlDocument();
                XmlElement rootElement = xmlDoc.CreateElement("MetricAssignment");
                xmlDoc.AppendChild(rootElement);
                if (counters != null)
                {
                    XmlAddList(xmlDoc, counters, "Metric", "MetricID");
                }

                using (SqlConnection connection = connectionInfo.GetConnection(Idera.SQLdm.Common.Constants.DesktopClientConnectionStringApplicationName))
                {

                    try
                    {
                        connection.Open();
                        xa = connection.BeginTransaction();

                        using (SqlDataReader dataReader = SqlHelper.ExecuteReader(xa, AssignCountersToServerProcedure, serverId, xmlDoc.OuterXml, true))
                        {
                        }
                        xa.Commit();

                    }

                    catch (Exception ex)
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
                }
            }
        }

      public static void AddCustomCounters(int serverId)	
        {	

                SqlTransaction xa = null;	
                SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;	
                List<int> customCounters = GetCustomCounterList(connectionInfo,serverId);	

                XmlDocument xmlDoc = new XmlDocument();	
                XmlElement rootElement = xmlDoc.CreateElement("MetricAssignment");	
                xmlDoc.AppendChild(rootElement);	
                XmlAddList(xmlDoc, customCounters, "Metric", "MetricID");	

                using (SqlConnection connection = connectionInfo.GetConnection(Idera.SQLdm.Common.Constants.DesktopClientConnectionStringApplicationName))	
                {	

                    try	
                    {	
                        connection.Open();	
                        xa = connection.BeginTransaction();	

                        using (SqlDataReader dataReader = SqlHelper.ExecuteReader(xa, AssignCountersToServerProcedure, serverId, xmlDoc.OuterXml, true))	
                        {	
                        }	
                        xa.Commit();	

                    }	

                    catch (Exception ex)	
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
                }	

        }

        public static List<String> GetDatabaseList(SqlConnectionInfo connectionInfo, int serverId)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetKnownDatabasesStoredProcedure, serverId))
                {
                    List<String> databases = new List<String>();

                    while (dataReader.Read())
                    {
                        string database = dataReader.GetString(1).Trim();

                        if (database.Length > 0)
                            databases.Add(database);
                    }

                    return databases;
                }
            }
        }

        public static List<String> GetDriveList(SqlConnectionInfo connectionInfo, int serverId)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetCurrentDiskDrivesStoredProcedure, serverId))
                {
                    List<string> drives = new List<string>();

                    while (dataReader.Read())
                    {
                        string drive = dataReader.GetString(0).Trim();
                        drive = drive.TrimEnd('\\', '/');

                        switch (drive.Length)
                        {
                            case 0:
                                continue;
                            case 1:
                                if (drive[0] == ':')
                                    continue;
                                drive = drive.ToUpper() + ":";
                                break;
                            case 2:
                                drive = drive.ToUpper();
                                break;
                            default:
                                break;
                        }
                        drives.Add(drive);
                    }

                    return drives;
                }
            }
        }

        #region Replication
        public static Dictionary<int, ReplicationSession> GetReplicationTopology(SqlConnectionInfo connectionInfo, int ServerID)
        {
            //if (repositoryConnectionString == null)
            //{
            //    throw new ArgumentNullException("repositoryConnectionString");
            //}
            //SqlConnection connection =
            //        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))

            Dictionary<int, ReplicationSession> participants = new Dictionary<int, ReplicationSession>();

            //using (SqlConnection connection = new SqlConnection(repositoryConnectionString))
            using (SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
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
                            if (participants.ContainsKey(key))
                            {
                                participants[key] = session;
                            }
                            else
                            {
                                participants.Add(key, session);
                            }
                        }
                    }
                }
            }
            return participants;
        }

        public static SortedDictionary<DateTime, ReplicationTrendDataSample> GetReplicationChartHistory(SqlConnectionInfo connectionInfo, int ServerID, int ChartDuration)
        {
            DataTable replicationHistory = new DataTable("ReplicationChartHistoryDataTable");

            //replicationHistory.Columns.Add("UTCCollectionDateTime", typeof(DateTime));
            //replicationHistory.Columns.Add("ReplicationLatencyInSeconds", typeof(double));
            //replicationHistory.Columns.Add("DistributionLatencyInSeconds", typeof(double));
            //replicationHistory.Columns.Add("ReplicationSubscribed", typeof(long));
            //replicationHistory.Columns.Add("ReplicationUnsubscribed", typeof(long));
            //replicationHistory.Columns.Add("ReplicationUndistributed", typeof(long));

            //replicationHistory.DefaultView.Sort = "UTCCollectionDateTime";
            SortedDictionary<DateTime, ReplicationTrendDataSample> samples = new SortedDictionary<DateTime, ReplicationTrendDataSample>();

            using (SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(connection, ReplicationChartHistory, new object[] { ServerID, null, ChartDuration }))
                {
                    while (dataReader.Read())
                    {
                        //replicationHistory.LoadDataRow(
                        //    new object[]{
                        //        dataReader.GetDateTime(0),
                        //        dataReader.GetDouble(1),
                        //        dataReader.GetDouble(2),
                        //        dataReader.IsDBNull(3) ? 0: dataReader.GetInt64(3),
                        //        dataReader.IsDBNull(4) ? 0: dataReader.GetInt64(4),
                        //        dataReader.IsDBNull(5) ? 0: dataReader.GetInt64(5)}, true);
                        ReplicationTrendDataSample sample = new ReplicationTrendDataSample(dataReader.GetDateTime(0),
                            dataReader.IsDBNull(1) ? (double?)null : dataReader.GetDouble(1),
                            dataReader.IsDBNull(2) ? (double?)null : dataReader.GetDouble(2),
                            dataReader.IsDBNull(3) ? (long?)null : dataReader.GetInt64(3),
                            dataReader.IsDBNull(4) ? (long?)null : dataReader.GetInt64(4),
                            dataReader.IsDBNull(5) ? (long?)null : dataReader.GetInt64(5));

                        samples.Add(dataReader.GetDateTime(0), sample);
                    }
                }
            }
            return samples;
        }
        #endregion

        internal static bool GetLinkedBlockingAlertData(SqlConnectionInfo connectionInfo, long alertId, out int sqlServerId, out DateTime collectionDateTime, out string xdl)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetBlockStoredProcedure, alertId))
                {
                    if (!dataReader.Read())
                    {
                        sqlServerId = 0;
                        collectionDateTime = DateTime.MinValue;
                        xdl = null;
                        return false;
                    }

                    sqlServerId = dataReader.GetInt32(0);
                    collectionDateTime = dataReader.GetDateTime(1);
                    xdl = dataReader.GetString(2);
                }
            }

            return true;
        }

        public static List<Triple<int, DateTime, string>> GetBlocksRange(SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime start, DateTime end)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            //SQLdm 10.0 - vineet -- DE45706 -- Multiple deadlocks/blocks were not being displayed for same collection time. Fixed the same. Changed return type from dictionary to list of tuples
            List<Triple<int, DateTime, string>> result = new List<Triple<int, DateTime, string>>();
            int x = 1;
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetBlocksList, monitoredServerId, start.ToUniversalTime(), end.ToUniversalTime()))
                {
                    while (dataReader.Read())
                    {
                        DateTime collectionDateTime = dataReader.GetDateTime(1).ToLocalTime();
                        if (!dataReader.IsDBNull(2))
                        {
                            result.Add(new Triple<int, DateTime, string>(x, collectionDateTime, dataReader.GetString(2)));
                            x++;
                        }
                    }
                }
            }

            return result;
        }

        public static List<Triple<int, DateTime, string>> GetDeadlocksRange(SqlConnectionInfo connectionInfo, int monitoredServerId, DateTime start, DateTime end)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            //SQLdm 10.0 - vineet -- DE45706 -- Multiple deadlocks/blocks were not being displayed for same collection time. Fixed the same. Changed return type from dictionary to list of tuples
            List<Triple<int, DateTime, string>> result = new List<Triple<int, DateTime, string>>();
            int x = 1;
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetDeadlocksList, monitoredServerId, start.ToUniversalTime(), end.ToUniversalTime()))
                {
                    while (dataReader.Read())
                    {
                        DateTime collectionDateTime = dataReader.GetDateTime(1).ToLocalTime();
                        if (!dataReader.IsDBNull(2))
                        {
                            result.Add(new Triple<int, DateTime, string>(x, collectionDateTime, dataReader.GetString(2)));
                            x++;
                        }
                    }
                }
            }

            return result;
        }

        internal static bool GetLinkedAlertData(SqlConnectionInfo connectionInfo, long alertId, out int sqlServerId, out DateTime collectionDateTime, out string xdl)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetDeadlockStoredProcedure, alertId))
                {
                    if (!dataReader.Read())
                    {
                        sqlServerId = 0;
                        collectionDateTime = DateTime.MinValue;
                        xdl = null;
                        return false;
                    }

                    sqlServerId = dataReader.GetInt32(0);
                    collectionDateTime = dataReader.GetDateTime(1);
                    xdl = dataReader.GetString(2);
                }
            }

            return true;
        }
        #region Custom Reports
        /// <summary>
        /// get the xml for the specified report
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <param name="reportName"></param>
        /// <returns></returns>
        internal static CustomReport GetCustomReport(SqlConnectionInfo connectionInfo, string reportName)
        {
            string reportXML = null;
            string title = null;
            string shortDesc = null;
            CustomReport report = null;
            Boolean topServerReport = false;
            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetCustomReports, reportName))
                {
                    while (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(0)) title = dataReader["reportName"].ToString();
                        if (!dataReader.IsDBNull(1)) shortDesc = dataReader["reportShortDescription"].ToString();
                        if (!dataReader.IsDBNull(2)) reportXML = dataReader["reportText"].ToString();
                        if (!dataReader.IsDBNull(3)) topServerReport = dataReader.GetBoolean(3);
                    }
                    report = new CustomReport(title, shortDesc, reportXML, topServerReport);
                }
            }
            return report;
        }
        /// <summary>
        /// Get the list of all custom reports that are in the repository
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <returns></returns>
        internal static ICollection<CustomReport> GetCustomReportsList(SqlConnectionInfo connectionInfo)
        {
            string reportName = null;
            string shortDesc = null;
            string reportXML = null;
            Boolean topServerReport = false;

            ICollection<CustomReport> reports = new List<CustomReport>();

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetCustomReports))
                {
                    while (dataReader.Read())
                    {

                        if (!dataReader.IsDBNull(0)) reportName = dataReader["reportName"].ToString();
                        if (!dataReader.IsDBNull(1)) shortDesc = dataReader["reportShortDescription"].ToString();
                        if (!dataReader.IsDBNull(2)) reportXML = dataReader["reportText"].ToString();
                        if (!dataReader.IsDBNull(3)) topServerReport = dataReader.GetBoolean(3);

                        CustomReport report = new CustomReport(reportName, shortDesc, reportXML, topServerReport);
                        reports.Add(report);
                    }
                }
            }
            return reports;
        }
        /// <summary>
        /// Get the counters that have been selected for this Custom Report
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <param name="reportName"></param>
        /// <returns></returns>
        internal static DataTable GetSelectedCounters(SqlConnectionInfo connectionInfo, string reportName)
        {
            DataTable selectedCounterDataSet = new DataTable("SelectedReportCounters");

            selectedCounterDataSet.Columns.Add("ID", typeof(string)).AllowDBNull = false;
            selectedCounterDataSet.Columns.Add("GraphNumber", typeof(string)).AllowDBNull = false;
            selectedCounterDataSet.Columns.Add("CounterShortDescription", typeof(string)).AllowDBNull = false;
            selectedCounterDataSet.Columns.Add("CounterName", typeof(string)).AllowDBNull = false;
            selectedCounterDataSet.Columns.Add("Aggregation", typeof(string)).AllowDBNull = false;
            selectedCounterDataSet.Columns.Add("reportName", typeof(string)).AllowDBNull = false;
            selectedCounterDataSet.Columns.Add("CounterType", typeof(string)).AllowDBNull = false;
            selectedCounterDataSet.Columns.Add("reportShortDescription", typeof(string)).AllowDBNull = true;

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetSelectedCustomReportCounters, reportName))
                {
                    object[] columnArray = new object[selectedCounterDataSet.Columns.Count];
                    selectedCounterDataSet.Clear();

                    while (dataReader.Read())
                    {
                        selectedCounterDataSet.BeginLoadData();

                        if (!dataReader.IsDBNull(0)) columnArray[0] = dataReader["ID"].ToString();
                        if (!dataReader.IsDBNull(1)) columnArray[1] = dataReader["GraphNumber"].ToString();
                        if (!dataReader.IsDBNull(2)) columnArray[2] = dataReader["CounterShortDescription"].ToString();
                        if (!dataReader.IsDBNull(3)) columnArray[3] = dataReader["CounterName"].ToString();
                        if (!dataReader.IsDBNull(4)) columnArray[4] = dataReader["Aggregation"].ToString();
                        if (!dataReader.IsDBNull(5)) columnArray[5] = dataReader["reportName"].ToString();
                        if (!dataReader.IsDBNull(6)) columnArray[6] = dataReader["CounterType"].ToString();
                        if (!dataReader.IsDBNull(7)) columnArray[7] = dataReader["reportShortDescription"].ToString();

                        selectedCounterDataSet.LoadDataRow(columnArray, true);
                    }
                    selectedCounterDataSet.EndLoadData();
                }
            }
            return selectedCounterDataSet;
        }
        /// <summary>
        /// Get all the counters that are available for reporting
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <returns>Datatable Name, friendly name and source</returns>
        internal static DataTable GetAvailableCounters(SqlConnectionInfo connectionInfo)
        {
            DataTable availableCounterDataSet = new DataTable("AvailableReportCounters");
            availableCounterDataSet.Columns.Add("CounterName", typeof(string)).AllowDBNull = false;
            availableCounterDataSet.Columns.Add("CounterFriendlyName", typeof(string)).AllowDBNull = false;
            availableCounterDataSet.Columns.Add("Source", typeof(string)).AllowDBNull = false;

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetAvailableCustomReportCounters))
                {
                    object[] columnArray = new object[availableCounterDataSet.Columns.Count];
                    availableCounterDataSet.Clear();

                    while (dataReader.Read())
                    {
                        availableCounterDataSet.BeginLoadData();

                        if (!dataReader.IsDBNull(0)) columnArray[0] = dataReader["CounterName"].ToString();
                        if (!dataReader.IsDBNull(1)) columnArray[1] = dataReader["CounterFriendlyName"].ToString();
                        if (!dataReader.IsDBNull(2)) columnArray[2] = dataReader["Source"].ToString();

                        availableCounterDataSet.LoadDataRow(columnArray, true);
                    }
                    availableCounterDataSet.EndLoadData();
                }
            }
            return availableCounterDataSet;
        }
        #endregion

        internal static DataTable GetWaitTypeDefinitions()
        {
            if (Settings.Default.ActiveRepositoryConnection.ConnectionInfo == null)
                throw new ArgumentNullException("Settings.Default.ActiveRepositoryConnection.ConnectionInfo");

            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("CategoryID", typeof(int)).AllowDBNull = true;
            dataTable.Columns.Add("Category", typeof(string)).AllowDBNull = false;
            dataTable.Columns.Add("WaitType", typeof(string)).AllowDBNull = false;
            dataTable.Columns.Add("Description", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("HelpLink", typeof(string)).AllowDBNull = true;

            using (SqlConnection connection = Settings.Default.ActiveRepositoryConnection.ConnectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
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

        internal static DataTable GetWaitStatistics(int serverId, DateTime collectionDateTime, int? historyInMinutes, DateTime? startDateTime = null)
        {
            // get stats for all categories
            return GetWaitStatistics(serverId, collectionDateTime, historyInMinutes, null, startDateTime);
        }

        internal static DataTable GetWaitStatistics(int serverId, DateTime collectionDateTime, int? historyInMinutes, string categoryName,
            DateTime? startDateTime = null)
        {
            if (Settings.Default.ActiveRepositoryConnection.ConnectionInfo == null)
                throw new ArgumentNullException("Settings.Default.ActiveRepositoryConnection.ConnectionInfo");

            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Timestamp", typeof(DateTime)).AllowDBNull = false;
            dataTable.Columns.Add("Type", typeof(string)).AllowDBNull = false;
            dataTable.Columns.Add("Category", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("Waiting Tasks Count Total", typeof(long)).AllowDBNull = true;
            dataTable.Columns.Add("Wait Time Total", typeof(TimeSpan)).AllowDBNull = true;
            dataTable.Columns.Add("Max Wait Time", typeof(TimeSpan)).AllowDBNull = true;
            dataTable.Columns.Add("Signal Wait Time Total", typeof(TimeSpan)).AllowDBNull = true;
            dataTable.Columns.Add("Resource Wait Time Total", typeof(TimeSpan)).AllowDBNull = true;
            dataTable.Columns.Add("Waiting Tasks Per Second", typeof(double)).AllowDBNull = true;
            dataTable.Columns.Add("Total Wait Milliseconds Per Second", typeof(double)).AllowDBNull = true;
            dataTable.Columns.Add("Signal Wait Milliseconds Per Second", typeof(double)).AllowDBNull = true;
            dataTable.Columns.Add("Resource Wait Milliseconds Per Second", typeof(double)).AllowDBNull = true;

            using (SqlConnection connection = Settings.Default.ActiveRepositoryConnection.ConnectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, "p_GetWaitStatistics", serverId,
                    collectionDateTime.ToUniversalTime(),
                    historyInMinutes != null ? historyInMinutes : MathHelper.GetMinutes(startDateTime, collectionDateTime), categoryName))
                {
                    dataTable.BeginLoadData();

                    while (dataReader.Read())
                    {
                        DataRow row = dataTable.NewRow();

                        row["Timestamp"] = ((DateTime)dataReader["UTCCollectionDateTime"]).ToLocalTime();
                        row["Type"] = dataReader["WaitType"];
                        row["Category"] = dataReader["Category"];
                        row["Waiting Tasks Count Total"] = dataReader["WaitingTasks"];
                        row["Wait Time Total"] = dataReader["WaitTimeInMilliseconds"];
                        row["Max Wait Time"] = dataReader["MaxWaitTimeInMilliseconds"];
                        row["Signal Wait Time Total"] = dataReader["SignalWaitTimeInMilliseconds"];
                        row["Resource Wait Time Total"] = dataReader["ResourceWaitTimeInMilliseconds"];
                        row["Waiting Tasks Per Second"] = dataReader["WaitingTasksPerSecond"];
                        row["Total Wait Milliseconds Per Second"] = dataReader["TotalWaitMillisecondsPerSecond"];
                        row["Signal Wait Milliseconds Per Second"] = dataReader["SignalWaitMillisecondsPerSecond"];
                        row["Resource Wait Milliseconds Per Second"] = dataReader["ResourceWaitMillisecondsPerSecond"];

                        dataTable.Rows.Add(row);
                    }

                    dataTable.EndLoadData();
                }
            }

            return dataTable;
        }

        internal static DataTable GetQueryWaitStatistics(int serverId, DateTime collectionDateTime, int historyInMinutes)
        {
            if (Settings.Default.ActiveRepositoryConnection.ConnectionInfo == null)
                throw new ArgumentNullException("Settings.Default.ActiveRepositoryConnection.ConnectionInfo");

            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("timestamp", typeof(DateTime)).AllowDBNull = false;
            dataTable.Columns.Add("duration", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("session id", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("wait type", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("wait category", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("host name", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("program name", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("login name", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("database name", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("statement txt", typeof(string)).AllowDBNull = true;

            using (SqlConnection connection = Settings.Default.ActiveRepositoryConnection.ConnectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, "p_GetActiveWaits", serverId, collectionDateTime.ToUniversalTime(), historyInMinutes, null, null, null, null, null, null, null, null, null, null))
                {
                    dataTable.BeginLoadData();

                    while (dataReader.Read())
                    {
                        DataRow row = dataTable.NewRow();
                        DateTime ts = ((DateTime)dataReader["StatementUTCStartTime"]).ToLocalTime();
                        row["timestamp"] = ts.AddTicks(-(ts.Ticks % TimeSpan.TicksPerSecond));
                        row["duration"] = dataReader["WaitDuration"];
                        row["session id"] = dataReader["SessionID"];
                        row["wait type"] = dataReader["WaitType"];
                        row["wait category"] = dataReader["Category"];
                        row["host name"] = dataReader["HostName"];
                        row["program name"] = dataReader["ApplicationName"];
                        row["login name"] = dataReader["LoginName"];
                        row["database name"] = dataReader["DatabaseName"];
                        row["statement txt"] = dataReader["SQLStatement"];

                        dataTable.Rows.Add(row);
                    }

                    dataTable.EndLoadData();
                }
            }

            return dataTable;
        }

        internal static DataTable GetQueryWaitsConfiguration(int serverId)
        {
            if (Settings.Default.ActiveRepositoryConnection.ConnectionInfo == null)
                throw new ArgumentNullException("Settings.Default.ActiveRepositoryConnection.ConnectionInfo");

            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("configuration", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("XEEnabled", typeof(string));
            dataTable.Columns.Add("QsEnabled", typeof(string));

            using (SqlConnection connection = Settings.Default.ActiveRepositoryConnection.ConnectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                // SQLdm 10.4(Varun Chopra) query waits using Query Store
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, CommandType.Text, "select ActiveWaitAdvancedConfiguration, ActiveWaitXEEnable, ActiveWaitQsEnable from MonitoredSQLServers where SQLServerID = " + serverId))
                {
                    if (dataReader.HasRows)
                    {
                        dataReader.Read();

                        DataRow row = dataTable.NewRow();

                        if (!(dataReader[0] is DBNull))
                            row["configuration"] = dataReader.GetString(0);
                        if (!(dataReader[1] is DBNull))
                            row["XEEnabled"] = dataReader.GetBoolean(1);

                        // SQLdm 10.4(Varun Chopra) query waits using Query Store
                        if (!(dataReader[2] is DBNull))
                        {
                            row["QsEnabled"] = dataReader.GetBoolean(2);
                        }

                        dataTable.Rows.Add(row);
                    }
                }
            }

            return dataTable;
        }

        internal static DataTable GetServerForecats()
        {
            return GetServerForecasts(-1);
        }

        internal static DataTable GetServerForecasts(int serverId)
        {
            if (Settings.Default.ActiveRepositoryConnection.ConnectionInfo == null)
                throw new ArgumentNullException("Settings.Default.ActiveRepositoryConnection.ConnectionInfo");

            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("servername", typeof(string));
            dataTable.Columns.Add("metricname", typeof(string));
            dataTable.Columns.Add("severity", typeof(int));
            dataTable.Columns.Add("timeframe", typeof(int));
            dataTable.Columns.Add("forecast", typeof(int));
            dataTable.Columns.Add("accuracy", typeof(double));
            dataTable.Columns.Add("expiration", typeof(DateTime));

            using (SqlConnection connection = Settings.Default.ActiveRepositoryConnection.ConnectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, "p_GetPredictiveForecasts", serverId))
                {
                    dataTable.BeginLoadData();

                    while (dataReader.Read())
                    {
                        DataRow row = dataTable.NewRow();

                        row["servername"] = dataReader["ServerName"];
                        row["metricname"] = dataReader["MetricName"];
                        row["severity"] = dataReader["Severity"];
                        row["timeframe"] = dataReader["Timeframe"];
                        row["forecast"] = dataReader["Forecast"];
                        row["accuracy"] = dataReader["Accuracy"];
                        row["expiration"] = ((DateTime)dataReader["Expiration"]).ToLocalTime();

                        if ((double)row["accuracy"] >= 0.9)
                            row["accuracy"] = 0.9;

                        if ((double)row["accuracy"] < 0.1)
                            row["accuracy"] = 0.1;

                        dataTable.Rows.Add(row);
                    }

                    dataTable.EndLoadData();
                }
            }

            return dataTable;
        }

        public static bool GetPredictiveAnalyticsEnabled(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            using (var connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
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

        public static List<NotificationProviderInfo> GetNotificationProvider(SqlConnectionInfo connectionInfo, Guid? providerId)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<NotificationProviderInfo> result = new List<NotificationProviderInfo>();
            List<Type> types = NotificationProviderInfo.GetAvailableProviderTypes();

            object pid = DBNull.Value;
            if (providerId.HasValue)
                pid = providerId.Value;

            using (Log.InfoCall("GetNotificationProvider"))
            {
                using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, "p_GetNotificationProviders", pid))
                    {
                        while (reader.Read())
                        {
                            NotificationProviderInfo npi = null;

                            Guid id = reader.GetGuid(0);
                            string typeName = reader.GetString(1);
                            string xml = reader.GetString(2);

                            Type providerType = null;

                            foreach (Type type in types)
                            {
                                if (type.Name == typeName)
                                {
                                    providerType = type;
                                    break;
                                }
                            }

                            if (providerType == null)
                                throw new ApplicationException(String.Format("Unable to find notification provider type: {0}", typeName));

                            XmlSerializer serializer =
                                Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(providerType);
                            try
                            {
                                StringReader stream = new StringReader(xml);
                                using (XmlReader xmlReader = XmlReader.Create(stream))
                                {
                                    npi = serializer.Deserialize(xmlReader) as NotificationProviderInfo;
                                }
                                npi.Id = id;
                            }
                            catch (Exception)
                            {
                                Log.ErrorFormat("Error deserializing notification provider info: Id={0} Type={1} Xml={2}",
                                                id, typeName, xml);
                                continue;
                            }

                            result.Add(npi);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) -Fetches File Activity based on start/end/minutes values.
        /// </summary>
        internal static DataTable GetFileActivity(int serverId, DateTime collectionDateTime, int? historyInMinutes, DateTime? startDateTime = null)
        {
            if (Settings.Default.ActiveRepositoryConnection.ConnectionInfo == null)
                throw new ArgumentNullException("Settings.Default.ActiveRepositoryConnection.ConnectionInfo");

            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("timestamp", typeof(DateTime)).AllowDBNull = false;
            dataTable.Columns.Add("drive name", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("database name", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("file name", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("file type", typeof(int)).AllowDBNull = true;
            dataTable.Columns.Add("file path", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("disk reads per second", typeof(long)).AllowDBNull = true;
            dataTable.Columns.Add("disk writes per second", typeof(long)).AllowDBNull = true;
            dataTable.Columns.Add("file reads per second", typeof(double)).AllowDBNull = true;
            dataTable.Columns.Add("file writes per second", typeof(double)).AllowDBNull = true;
            dataTable.Columns.Add("disk transfers per second", typeof(long)).AllowDBNull = true;

            using (SqlConnection connection = Settings.Default.ActiveRepositoryConnection.ConnectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, "p_GetFileActivity", serverId,
                    collectionDateTime.ToUniversalTime(),
                    historyInMinutes != null ? historyInMinutes : MathHelper.GetMinutes(startDateTime, collectionDateTime)))
                {
                    dataTable.BeginLoadData();

                    while (dataReader.Read())
                    {
                        DataRow row = dataTable.NewRow();

                        row["timestamp"] = ((DateTime)dataReader["UTCCollectionDateTime"]);
                        row["drive name"] = dataReader["DriveName"];
                        row["database name"] = dataReader["DatabaseName"];
                        row["file name"] = dataReader["FileName"];
                        row["file type"] = dataReader["FileType"];
                        row["file path"] = dataReader["FilePath"];
                        row["disk reads per second"] = dataReader["DiskReadsPerSecond"];
                        row["disk writes per second"] = dataReader["DiskWritesPerSecond"];
                        row["file reads per second"] = dataReader["FileReadsPerSecond"];
                        row["file writes per second"] = dataReader["FileWritesPerSecond"];
                        row["disk transfers per second"] = dataReader["DiskTransfersPerSecond"];

                        dataTable.Rows.Add(row);
                    }

                    dataTable.EndLoadData();
                }
            }

            return dataTable;
        }

        internal static DataTable GetFileActivity(
            SqlConnectionInfo connectionInfo, int serverId, DateTime collectionDateTime, int? historyInMinutes, DateTime? startDateTime = null)
        {
            if (connectionInfo == null)
                throw new ArgumentNullException("connectionInfo");

            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Date", typeof(DateTime)).AllowDBNull = false;
            dataTable.Columns.Add("FileName", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("FileType", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("FilePath", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("DriveName", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("DatabaseName", typeof(string)).AllowDBNull = true;
            dataTable.Columns.Add("ReadsPerSecond", typeof(double)).AllowDBNull = true;
            dataTable.Columns.Add("WritesPerSecond", typeof(double)).AllowDBNull = true;
            dataTable.Columns.Add("TransfersPerSecond", typeof(double)).AllowDBNull = true;

            dataTable.Columns.Add("IsHistorical", typeof(bool)).AllowDBNull = true;

            using (
                SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, "p_GetFileActivity", serverId,
                                                                       collectionDateTime.ToUniversalTime(),
                                              historyInMinutes != null ? historyInMinutes :
                                              MathHelper.GetMinutes(startDateTime, collectionDateTime)))
                {
                    // I decided not to use the GetTable function because of the Date column name containing UTC which would be misleading
                    dataTable.BeginLoadData();

                    while (dataReader.Read())
                    {
                        DataRow row = dataTable.NewRow();

                        row["Date"] = ((DateTime)dataReader["UTCCollectionDateTime"]).ToLocalTime();
                        row["FileName"] = dataReader["FileName"];
                        row["FileType"] = dataReader["FileType"];
                        row["FilePath"] = dataReader["FilePath"];
                        row["DriveName"] = dataReader["DriveName"];
                        row["DatabaseName"] = dataReader["DatabaseName"];
                        row["ReadsPerSecond"] = dataReader["FileReadsPerSecond"];
                        row["WritesPerSecond"] = dataReader["FileWritesPerSecond"];
                        row["TransfersPerSecond"] = dataReader["FileTransfersPerSecond"];

                        dataTable.Rows.Add(row);
                    }

                    dataTable.EndLoadData();
                }
            }

            return dataTable;
        }

        public static List<string> GetNotificationRules(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
                throw new ArgumentNullException("connectionInfo");

            List<string> result = new List<string>();
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetNotificationRulesStoredProcedure, DBNull.Value))
                {
                    while (reader.Read())
                    {
                        result.Add(reader.GetString(1));
                    }
                }
            }

            return result;
        }

        //START: SQLdm 10.0 (Tarun Sapra)- function to call when multiple baseline templates needs to be imported to different servers
        public static void SaveMultipleBaselineTemplate(int serverid, string unifiedXmlForAllConfigs)
        {
            if (Settings.Default.ActiveRepositoryConnection.ConnectionInfo == null)
                throw new ArgumentNullException("Settings.Default.ActiveRepositoryConnection.ConnectionInfo");

            using (SqlConnection connection = Settings.Default.ActiveRepositoryConnection.ConnectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_AddMultipleBaselineTemplate"))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, serverid, unifiedXmlForAllConfigs);

                    command.ExecuteNonQuery();
                }
            }
        }
        //END: SQLdm 10.0 (Tarun Sapra)- function to call when multiple baseline templates needs to be imported to different servers

        public static void SaveBaselineTemplate(int serverid, BaselineConfiguration config)
        {
            if (Settings.Default.ActiveRepositoryConnection.ConnectionInfo == null)
                throw new ArgumentNullException("Settings.Default.ActiveRepositoryConnection.ConnectionInfo");

            using (SqlConnection connection = Settings.Default.ActiveRepositoryConnection.ConnectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_AddBaselineTemplate"))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, serverid, config.Serialize());

                    command.ExecuteNonQuery();
                }
            }
        }
        /*
        public static int EditOrAddBaselineTemplate(int serverid, BaselineConfiguration config)
        {
            if (Settings.Default.ActiveRepositoryConnection.ConnectionInfo == null)
                throw new ArgumentNullException("Settings.Default.ActiveRepositoryConnection.ConnectionInfo");

            using (SqlConnection connection = Settings.Default.ActiveRepositoryConnection.ConnectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_EditOrAddBaselineTemplate"))
                {
                    int newID = 0;
                    SqlHelper.AssignParameterValues(command.Parameters, serverid, config.Serialize(), config.BaselineName,0,newID);

                    command.ExecuteNonQuery();
                    newID = (int)command.Parameters["@newTemplateID"].Value;
                    return newID;
                }
            }
        }
        */
        public static void GetServerTimelineEvents(DataTable table, string instanceName, DateTime start, DateTime end)
        {
            if (Settings.Default.ActiveRepositoryConnection.ConnectionInfo == null)
                throw new ArgumentNullException("Settings.Default.ActiveRepositoryConnection.ConnectionInfo");

            using (SqlConnection connection = Settings.Default.ActiveRepositoryConnection.ConnectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, "p_GetServerTimeline", instanceName, start.ToUniversalTime(), end.ToUniversalTime()))
                {
                    while (reader.Read())
                    {
                        DataRow row = table.NewRow();

                        row["AlertID"] = reader["AlertID"];
                        row["UTCOccurrenceDateTime"] = reader["UTCOccurrenceDateTime"];
                        row["DatabaseName"] = reader["DatabaseName"];
                        row["TableName"] = reader["TableName"];
                        row["Active"] = reader["Active"];
                        row["Category"] = reader["Category"];
                        row["Metric"] = reader["Metric"];
                        row["Severity"] = reader["Severity"];
                        row["Rank"] = reader["Rank"];
                        row["StateEvent"] = reader["StateEvent"];
                        row["Value"] = reader["Value"];
                        row["Heading"] = reader["Heading"];

                        table.Rows.Add(row);
                    }
                }
            }
        }

        /// <summary>
        /// Get all of the monitored servers that have AlwaysOn relationships defined for stored
        /// databases
        /// </summary>
        /// <param name="connectionInfo"> this is a SqlConnectionInfo that contains the information
        /// to connect with the SQL server</param>
        /// <returns>This method return a list of SQLServerID and InstanceName that contains our
        /// Monitored SQL servers and contains AlwaysOn databases</returns>
        public static List<Pair<int, string>> GetMonitoredAlwaysOnServers(
            SqlConnectionInfo connectionInfo, int tagId, string availabilityGroupName)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = connectionInfo.GetConnection(
                Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(
                        connection,
                        CommandType.StoredProcedure,
                        GetAlwaysOnServersStoredProcedure,
                        new SqlParameter[]
                            {
                                new SqlParameter("tagID", tagId),
                                new SqlParameter("availabilityGroupName", availabilityGroupName),
                            }))
                {
                    List<Pair<int, string>> servers = new List<Pair<int, string>>();

                    while (dataReader.Read())
                    {
                        int serverId = (int)dataReader["SQLServerID"];
                        string serverName = dataReader["InstanceName"] as string;
                        servers.Add(new Pair<int, string>(serverId, serverName));
                    }

                    return servers;
                }
            }
        }

        public static DataTable GetAlwaysOnAGBasedOnActiveServers
            (SqlConnectionInfo connectionInfo, int tagId)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = connectionInfo.GetConnection(
                Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(
                        connection,
                        CommandType.StoredProcedure,
                        GetAlwaysOnAGBasedActiveServers,
                        new SqlParameter[]
                            {
                                new SqlParameter("tagId", tagId),
                            }))
                {
                    return GetTable(dataReader, false);
                }
            }
        }
        //Start: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report

        /// <summary>
        /// This method fetches the wait categories from database
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <returns></returns>
        public static DataTable GetWaitCategories
            (SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = connectionInfo.GetConnection(
                Constants.DesktopClientConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(
                        connection,
                        CommandType.Text,
                        "select wc.Category as 'Category', wc.CategoryID as 'CategoryID' from WaitCategories wc"
                        ))
                {
                    return GetTable(dataReader, false);
                }
            }
        }
        //End: SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report


        //SQLdm 10.0 (Tarun Sapra) -- Minimal Cloud Support -- Get the cloud provider for this newly added server
        /// <summary>
        ///  get the cloud provider for this newly added server
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>

        public static Dictionary<string, int> GetBinding(SqlConnectionInfo connectionInfo)
        {
            Dictionary<string, int> dictOfProviders = new Dictionary<string, int>();
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionString");
            }

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, GetCloudProvidersStoredProcedure))
                    {
                        while (dataReader.Read())
                        {
                            int cloudProviderNameOrdinal = dataReader.GetOrdinal("CloudProviderName");
                            int cloudProviderIdOrdinal = dataReader.GetOrdinal("CloudProviderId");

                            if (!dataReader.IsDBNull(cloudProviderIdOrdinal) && !dataReader.IsDBNull(cloudProviderNameOrdinal))
                            {
                                dictOfProviders.Add(Convert.ToString(dataReader[cloudProviderNameOrdinal]), Convert.ToInt32(dataReader[cloudProviderIdOrdinal]));
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                    Log.Error(String.Format("GetCloudProviders-- Error while accessing the SQL Server with the Connection String :[{0}]./n {1}", (connection != null && connection.DataSource != null) ? connection.DataSource : string.Empty, e.Message));
                    return null;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                        connection.Close();
                }
            }
            return dictOfProviders;
        }
        public static Dictionary<string, int> GetCloudProviders(SqlConnectionInfo connectionInfo)
        {
            Dictionary<string, int> dictOfProviders = new Dictionary<string, int>();
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionString");
            }

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, GetCloudProvidersStoredProcedure))
                    {
                        while (dataReader.Read())
                        {
                            int cloudProviderNameOrdinal = dataReader.GetOrdinal("CloudProviderName");
                            int cloudProviderIdOrdinal = dataReader.GetOrdinal("CloudProviderId");

                            if (!dataReader.IsDBNull(cloudProviderIdOrdinal) && !dataReader.IsDBNull(cloudProviderNameOrdinal))
                            {
                                dictOfProviders.Add(Convert.ToString(dataReader[cloudProviderNameOrdinal]), Convert.ToInt32(dataReader[cloudProviderIdOrdinal]));
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                    Log.Error(String.Format("GetCloudProviders-- Error while accessing the SQL Server with the Connection String :[{0}]./n {1}", (connection != null && connection.DataSource != null) ? connection.DataSource : string.Empty, e.Message));
                    return null;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                        connection.Close();
                }
            }
            return dictOfProviders;
        }
        #region sys admin feature
        /// <summary>
        /// SQLdm(10.1) Barkha Khatri SysAdmin feature-getting xml to update permissions in database
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="listParameter"></param>
        /// <param name="rootName"></param>
        /// <param name="elementName"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        private static XmlDocument GetXMLFromList<S, T>(List<Pair<S, T>> listParameter, string rootName, string elementName, string[] childName)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode rootNode = doc.CreateElement(rootName);
            doc.AppendChild(rootNode);
            //doc.LoadXml("<" + rootName + " ></" + rootName + ">");
            if (listParameter != null)
            {
                foreach (Pair<S, T> paramPair in listParameter)
                {
                    XmlNode serverNode = doc.CreateElement(elementName);
                    XmlElement innerElem1 = doc.CreateElement(childName[0]);
                    innerElem1.InnerXml = paramPair.First.ToString();
                    XmlElement innerElem2 = doc.CreateElement(childName[1]);
                    innerElem2.InnerXml = paramPair.Second.ToString();
                    serverNode.AppendChild(innerElem1);
                    serverNode.AppendChild(innerElem2);
                    rootNode.AppendChild(serverNode);
                }
            }
            doc.Save(Console.Out);
            return doc;
        }
        /// <summary>
        /// updating IsUserSysAdmin flag in MonitoredSqlServers table
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <param name="serversPermissionList"></param>
        public static void UpdateSysAdminPermissionsInRepo(SqlConnectionInfo connectionInfo, List<Pair<int, bool>> serversPermissionList)
        {
            try
            {
                using (Log.InfoCall("updating sys admin permissions in repository"))
                {
                    string[] childnames = { "ID", "IsUserSysAdmin" };
                    XmlDocument xmlData = GetXMLFromList(serversPermissionList, "Servers", "Server", childnames);
                    using (SqlConnection con = new SqlConnection(connectionInfo.ConnectionString))
                    {

                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(UpdateSysAdminPermissionsInRepository))
                        {
                            cmd.Connection = con;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(
                                                new SqlParameter("@data", SqlDbType.Xml)
                                                {
                                                    Value = new SqlXml(new XmlTextReader(xmlData.InnerXml
                                                                , XmlNodeType.Document, null))
                                                });
                            //cmd.Parameters.AddWithValue("@data", xmlData.InnerXml);
                            cmd.ExecuteNonQuery();
                        }
                        con.Close();

                    }
                }
            }
            catch (Exception e)
            {

                Log.Error(String.Format("UpdateSysAdminPermissionsInRepo: error while updating repo{0}", e.Message));

            }
        }
        #endregion 

        //SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring Improvement -- Add server wizard -start
        /// <summary>
        ///  gets the product vesion of an SQL Server given the connection string 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static ServerVersion GetProductVersion(string connectionString)
        {
            //START SQLdm 9.1 (Ankit Srivastava) -- Fixed NUnit errors
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }
            //END SQLdm 9.1 (Ankit Srivastava) -- Fixed NUnit errors
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = GetVersion;
                        object productVersion = command.ExecuteScalar();
                        if (productVersion != null)
                        {
                            Log.InfoFormat("GetProductVersion -- This is the server version {0}:", productVersion.ToString());
                            return new ServerVersion((string)productVersion);
                        }
                        else
                        {
                            Log.Error(String.Format("GetProductVersion--the query returns null for the SQL Server with the Connection String :[{0}]./n {1}", (connection != null && connection.DataSource != null) ? connection.DataSource : string.Empty));
                            return null;
                        }
                    }
                }
                catch (Exception e)
                {

                    Log.Error(String.Format("GetProductVersion--Error while accessing the SQL Server with the Connection String :[{0}]./n {1}", (connection != null && connection.DataSource != null) ? connection.DataSource : string.Empty, e.Message));
                    return null;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                        connection.Close();
                }
            }
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

                try
                {
                    IManagementService defaultManagementService =
                        ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                    TestSqlConnectionResult result = defaultManagementService.isSysAdmin(connectionInfo);

                    if (result != null)
                    {
                        queryResult.ServerVersion = new ServerVersion(result.ServerVersion.ToString());
                        queryResult.IsUserSysAdmin = result.IsAdmin;
                    }

                    if (queryResult.ServerVersion != null)
                    {
                        Log.InfoFormat("GetServerVersionAndPermission -- This is the server version and sysAdminFlag {0} {1}:", queryResult.ServerVersion.ToString(), queryResult.IsUserSysAdmin);
                        return queryResult;
                    }
                    else
                    {
                        Log.Error(String.Format("GetServerVersionAndPermission--the query returns null for the SQL Server with the Connection String :[{0}].", connectionInfo.ToString()));
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(String.Format("GetServerVersionAndPermission--Error while executing GetServerVersionAndPermission.", e));
                    return null;
                }
            }
        }

        //SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring Improvement -- Add server wizard - end

        public static string GetQueryPlanInformation(
           SqlConnectionInfo connectionInfo,
           int? sqlSignatureId,
           int? statementType,
           int? queryStatisticsId)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            StringBuilder retVal = new StringBuilder();

            using (
                 SqlConnection connection =
                     new SqlConnection(connectionInfo.ConnectionString))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(
                    connection,
                    GetQueryPlanInformationStoredProcedure,
                    sqlSignatureId,
                    statementType,
                    queryStatisticsId))
                {

                    while (dataReader.Read())
                    {
                        int ordinalPlanXml = dataReader.GetOrdinal("PlanXML");
                        if (!(dataReader.IsDBNull(ordinalPlanXml)))
                        {
                            var encodedPlanXML = dataReader.GetString(ordinalPlanXml);
                            var decodedPlanXML = ObjectHelper.DecompressString(encodedPlanXML);
                            retVal.Append(decodedPlanXML);
                            break;
                        }
                    }
                }
            }
            return retVal.ToString();
        }

        //SQLdm 10.4 (Nikhil Bansal) - To get the additional information required to save the estimated query plan
        public static Pair<int, int> GetInfoToSavePlan(SqlConnectionInfo connectionInfo, int signatureID, int statementType)
        {

            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            int statisticsID = -1, statementId = -1;  // To get the additional infomation

            using (
                 SqlConnection connection =
                     new SqlConnection(connectionInfo.ConnectionString))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(
                    connection,
                    GetInfoToSavePlanStoredProcedure,
                    signatureID,
                    statementType))
                {

                    while (dataReader.Read())
                    {
                        //To get the statement ID and the QueryStatistics ID
                        if (!dataReader.IsDBNull(0))
                            statementId = dataReader.GetInt32(0);

                        if (!dataReader.IsDBNull(1))
                            statisticsID = dataReader.GetInt32(1);
                    }
                }
            }
            return new Pair<int, int>(statementId, statisticsID);
        }
        //SQLdm 10.4 (Charles Schultz) - Provide the Desktop UI with actual or estimated for labeling of the tabs
        public static bool IsActualQueryPlan(SqlConnectionInfo connectionInfo, int signatureID)
        {

            if (connectionInfo == null)
                throw new ArgumentNullException("connectionInfo");

            using (SqlConnection connection = new SqlConnection(connectionInfo.ConnectionString))
            {
                connection.Open();
                string command = "select top 1 sqp.IsActualPlan from [dbo].[SQLSignatures] as ssig inner join[dbo].[SQLStatements] " +
                                    "ss on ssig.SQLStatementExampleID = ss.SQLStatementID inner join[dbo].[SQLQueryPlans] sqp on " +
                                    "ss.SQLStatementID = sqp.SQLStatementID where ssig.SQLSignatureID = " + signatureID;


                using (SqlCommand sqlCommand = new SqlCommand(command, connection))
                {
                    var isActualPlan = sqlCommand.ExecuteScalar() as bool?;

                    connection.Close();

                    if ((isActualPlan == null) || (!isActualPlan.HasValue))
                        return false;
                    else
                        return isActualPlan.Value;
                }
            }

        }

        //SQLdm-29500 (Charles Schultz) - Resolves issue with query plans presenting an icon when query plan is not available
        public static void GetPresenceOfIcon(SqlConnectionInfo connectionInfo)
        {

            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            using (SqlConnection connection = new SqlConnection(connectionInfo.ConnectionString))
            {


                connection.Open();
                string command = @"SELECT 
	QMS.SQLSignatureID AS SignatureId,
    QMS.StatementType AS StatementType,
    QMS.QueryStatisticsID AS StatisticsId
FROM 
	[QueryMonitorStatistics] QMS 
		RIGHT JOIN SQLQueryPlans AS SQP 
			ON SQP.PlanID=QMS.PlanID 
		LEFT JOIN SQLQueryPlansOverflow AS SQPO 
			ON SQP.PlanID = SQPO.PlanID
WHERE
    ((SQP.Overflow=0 AND SQP.PlanXML IS NOT NULL AND SQP.PlanXML != '') 
		OR (SQP.Overflow=1 AND SQPO.PlanXMLOverflow IS NOT NULL AND SQPO.PlanXMLOverflow != ''))";
                using (SqlCommand sqlCommand = new SqlCommand(command, connection))
                {

                    var returnedData = sqlCommand.ExecuteReader();
                    // clear icons here?
                    while (returnedData.Read())
                    {
                        HandleEachRow((IDataRecord)returnedData);
                    }
                    connection.Close();
                }
            }
        }
        static List<QueryMonitorPlanParameters> icons = new List<QueryMonitorPlanParameters>();
        private static void HandleEachRow(IDataRecord returnedData)
        {
            if ((returnedData[0] as int?).HasValue && (returnedData[1] as int?).HasValue)
            {
                icons.Add(new QueryMonitorPlanParameters { SignatureId = (int)returnedData[0], StatementTypeId = (int)returnedData[1], QueryMonitorStatisticsId = (int)returnedData[2] });
            }
        }

        //SQLdm 10.4 (Charles Schultz) - Resolves issue with SQL Statement being unavailable in XML Plan
        public static string GetSQLStatement(SqlConnectionInfo connectionInfo, int statementID)
        {
            string strSqlStatement = "";
            if (connectionInfo == null)
            {

                throw new ArgumentNullException("connectionInfo");

            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionInfo.ConnectionString))
                {
                    connection.Open();
                    string command = "SELECT SQLStatement FROM dbo.SQLStatements WHERE SQLStatementID = " + statementID;
                    using (SqlCommand sqlCommand = new SqlCommand(command, connection))
                    {
                        var returnedDate = sqlCommand.ExecuteScalar();


                        connection.Close();
                        if (returnedDate != null)
                        {
                            strSqlStatement = returnedDate.ToString();
                        }
                    }


                }
            }
            catch (Exception e)
            {
                Log.Error("Exception in GetSQLStatement method", e);
            }
            return strSqlStatement;
        }

        //SQLdm 10.4 (Nikhil Bansal) - Saving the estimated query plan
        public static void SaveEstimatedQueryPlan(SqlConnectionInfo connectionInfo, int statmentID, int queryStatisticsID, string queryPlan)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = new SqlConnection(connectionInfo.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = SqlHelper.CreateCommand(connection, AddSQLQueryPlanStoredProcedure))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, statmentID, queryStatisticsID,
                        !string.IsNullOrEmpty(queryPlan) ? ObjectHelper.CompressString(queryPlan) : String.Empty, 0);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
        //Return Cloud Id by Instance ID
        internal static int GetCloudByInstanceID(SqlConnectionInfo connectionInfo, int instanceId)
        {
            int CloudId = Constants.Windows;
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetCloudByInstanceId, instanceId))
                {
                    while (reader.Read())
                    {
                        if (DBNull.Value != reader["CloudProviderId"] && 0 != Convert.ToInt32(reader["CloudProviderId"], CultureInfo.InvariantCulture))
                        {
                            CloudId = Convert.ToInt32(reader["CloudProviderId"], CultureInfo.InvariantCulture);
                        }
                    }
                }
                connection.Close();
            }
            return CloudId;
        }

        public static List<IAzureProfile> GetAzureProfiles(SqlConnectionInfo connectionInfo)
        {
            // SQLServerId to Azure Profile
            // AzureProfile - AzureApplication
            // AzureProfileApplication - List of AzureResources
            var azureProfiles = new List<IAzureProfile>();
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = new SqlConnection(connectionInfo.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, GetAzureProfilesStoredProcedure))
                    {
                        if (dataReader.HasRows)
                        {
                            long? previousProfileId = null;
                            long? previousApplicationProfileId = null;

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
                                if (previousProfileId != id)
                                {
                                    previousProfileId = id;
                                    previousApplicationProfileId = null;
                                    var azureProfile = new AzureProfile
                                    {
                                        Id = id,
                                        Description = description,
                                        SqlServerId = sqlServerId
                                    };
                                    azureProfiles.Add(azureProfile);
                                }

                                // Application Profile Details
                                var applicationProfileId = dataReader.GetInt64(azureApplicationProfileIdOrdinal);
                                if (previousApplicationProfileId != applicationProfileId)
                                {
                                    previousApplicationProfileId = applicationProfileId;

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
                                           // Secret=secret,
                                            Description = applicationDescription
                                        },
                                        Resources = new List<IAzureResource>()
                                    };
                                    azureProfiles.Last().ApplicationProfile = azureApplicationProfile;
                                }

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
                                var applicationProfile = azureProfiles.Last().ApplicationProfile;
                                applicationProfile.Resources.Add(
                                    new AzureResource
                                    {
                                        Type = resourceType,
                                        Name = resourceName,
                                        Uri = resourceUri,
                                        Profile = applicationProfile
                                    });
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                    Log.Error(String.Format("GetAzureProfiles:: Error while accessing the SQL Server with the Connection String :[{0}]./n {1}", (connection != null && connection.DataSource != null) ? connection.DataSource : string.Empty, e.Message));
                    return null;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                        connection.Close();
                }
            }

            return azureProfiles;
        }

        public static DataTable GetAzureProfileForResourceUri(SqlConnectionInfo sqlConnectionInfo, String sqlServerInstance, String resourceUri, int metricId)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(string));
            dataTable.Columns.Add("Description", typeof(string));
            dataTable.Columns.Add("SQLServerID", typeof(string));
            dataTable.Columns.Add("ProfileId", typeof(string));
            dataTable.Columns.Add("ProfileName", typeof(string));
            dataTable.Columns.Add("ProfileDescription", typeof(string));
            dataTable.Columns.Add("ApplicationId", typeof(string));
            dataTable.Columns.Add("ApplicationName", typeof(string));
            dataTable.Columns.Add("ApplicationDescription", typeof(string));
            dataTable.Columns.Add("ClientId", typeof(string));
            dataTable.Columns.Add("TenantId", typeof(string));
            dataTable.Columns.Add("Secret", typeof(string));
            dataTable.Columns.Add("AzureSubscriptionId", typeof(string));
            dataTable.Columns.Add("SubscriptionId", typeof(string));
            dataTable.Columns.Add("SubscriptionDescription", typeof(string));
            dataTable.Columns.Add("ResourceId", typeof(string));
            dataTable.Columns.Add("ResourceName", typeof(string));
            dataTable.Columns.Add("ResourceType", typeof(string));
            dataTable.Columns.Add("ResourceUri", typeof(string));
            dataTable.Columns.Add("MetricName", typeof(string));
            dataTable.Columns.Add("MetricDisplayName", typeof(string));
            dataTable.Columns.Add("AzureSqlServerId",typeof(string));
          
            if (sqlConnectionInfo == null)
            {
                throw new NullReferenceException("Connection not found");
            }
            using (
                SqlConnection connection =
                    sqlConnectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                try
                {
                    using (
                        SqlDataReader dataReader =
                            SqlHelper.ExecuteReader(connection, GetAzureProfileWithMetricInfoStoredProcedure, sqlServerInstance, resourceUri, metricId))
                    {
                        while (dataReader.Read())
                        {
                            dataTable.Rows.Add(dataReader["Id"], 
                            dataReader["Description"].ToString(),
                            dataReader["SQLServerID"], 
                            dataReader["AzureApplicationProfileId"].ToString(),
                            dataReader["AzureApplicationProfileName"],
                            dataReader["AzureApplicationProfileDescription"].ToString(),
                            dataReader["AzureApplicationId"], 
                            dataReader["ApplicationName"].ToString(),
                            dataReader["ApplicationDescription"],
                            dataReader["ClientId"].ToString(), 
                            dataReader["TenantId"], dataReader["Secret"].ToString(),
                            dataReader["AzureSubscriptionId"],
                            dataReader["SubscriptionId"].ToString(),
                            dataReader["SubscriptionDescription"], 
                            dataReader["ResourceId"].ToString(), 
                            dataReader["ResourceName"].ToString(),
                            dataReader["ResourceType"].ToString(),
                            dataReader["ResourceUri"].ToString(),
                            dataReader["MetricName"].ToString(),
                            dataReader["MetricDisplayName"].ToString(),
                            dataReader["AzureSqlServerId"].ToString());
                        }
                    }
                    }
                
                    catch (Exception e)
                {

                    Log.Error(String.Format(
                        "GetAzureApplicationProfiles:: Error while accessing the SQL Server with the Connection String :[{0}]./n {1}",
                        (sqlConnectionInfo != null && sqlConnectionInfo.DatabaseName != null) ? sqlConnectionInfo.DatabaseName : string.Empty,
                        e.Message));
                    return null;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                        connection.Close();
                }
            }

            return dataTable;

        } 

        public static List<IAzureApplicationProfile> GetAzureApplicationProfiles(SqlConnectionInfo connectionInfo)
        {
            var azureApplicationProfiles = new List<IAzureApplicationProfile>();
            if (connectionInfo == null)
            {
                throw new NullReferenceException("Connection not found");
            }
            using (SqlConnection connection = new SqlConnection(connectionInfo.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure,
                            GetAzureApplicationProfilesStoredProcedure))
                    {
                        if (dataReader.HasRows)
                        {
                            long? previousApplicationProfileId = null;
                            var idOrdinal = dataReader.GetOrdinal("ID");
                            var nameOrdinal = dataReader.GetOrdinal("Name");
                            var descriptionOrdinal = dataReader.GetOrdinal("Description");
                            var azureApplicationIdOrdinal = dataReader.GetOrdinal("AzureApplicationId");
                            var applicationNameOrdinal = dataReader.GetOrdinal("ApplicationName");
                            var applicationDescriptionOrdinal = dataReader.GetOrdinal("ApplicationDescription");
                            var clientIdOrdinal = dataReader.GetOrdinal("ClientId");
                            var tenantIdOrdinal = dataReader.GetOrdinal("TenantId");
                            var secretOrdinal = dataReader.GetOrdinal("Secret");
                            var azureSubscriptionIdOrdinal = dataReader.GetOrdinal("AzureSubscriptionId");
                            var subscriptionIdOrdinal = dataReader.GetOrdinal("SubscriptionId");
                            var subscriptionDescriptionOrdinal = dataReader.GetOrdinal("SubscriptionDescription");

                            while (dataReader.Read())
                            {
                                // Profile Details
                                var id = dataReader.GetInt64(idOrdinal);
                                var name = dataReader.GetString(nameOrdinal);
                                var description = !dataReader.IsDBNull(descriptionOrdinal)
                                    ? dataReader.GetString(descriptionOrdinal)
                                    : null;

                                if (previousApplicationProfileId != id)
                                {
                                    previousApplicationProfileId = id;
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

                                    var azureProfile = new AzureApplicationProfile
                                    {
                                        Id = id,
                                        Name = name,
                                        Description = description,
                                        Subscription = new AzureSubscription
                                        {
                                            Id = azureSubscriptionId,
                                            SubscriptionId = subscriptionId,
                                            Description = subscriptionDescription
                                        },
                                        Application = new AzureApplication
                                        {
                                            Id = azureApplicationId,
                                            Description = applicationDescription,
                                            Name = applicationName,
                                            ClientId = clientId,
                                            TenantId = tenantId,
                                            EncryptedSecret = secret,
                                        },
                                        Resources = new List<IAzureResource>()
                                    };
                                    azureApplicationProfiles.Add(azureProfile);
                                }

                                var applicationProfile = azureApplicationProfiles.Last();
                               
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                    Log.Error(String.Format(
                        "GetAzureApplicationProfiles:: Error while accessing the SQL Server with the Connection String :[{0}]./n {1}",
                        (connection != null && connection.DataSource != null) ? connection.DataSource : string.Empty,
                        e.Message));
                    return null;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                        connection.Close();
                }
            }

            return azureApplicationProfiles;
        }


        public static List<IAzureApplication> GetAzureApplications(SqlConnectionInfo connectionInfo)
        {
            var azureApplications = new List<IAzureApplication>();
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = new SqlConnection(connectionInfo.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure,
                            GetAzureApplicationsStoredProcedure))
                    {
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                var azureApplicationIdOrdinal = dataReader.GetOrdinal("AzureApplicationId");
                                var applicationNameOrdinal = dataReader.GetOrdinal("ApplicationName");
                                var applicationDescriptionOrdinal = dataReader.GetOrdinal("ApplicationDescription");
                                var clientIdOrdinal = dataReader.GetOrdinal("ClientId");
                                var tenantIdOrdinal = dataReader.GetOrdinal("TenantId");
                                var secretOrdinal = dataReader.GetOrdinal("Secret");

                                // Application Details
                                var azureApplicationId = dataReader.GetInt64(azureApplicationIdOrdinal);
                                var applicationName = dataReader.GetString(applicationNameOrdinal);
                                var applicationDescription = !dataReader.IsDBNull(applicationDescriptionOrdinal)
                                    ? dataReader.GetString(applicationDescriptionOrdinal)
                                    : null;
                                var clientId = dataReader.GetString(clientIdOrdinal);
                                var tenantId = dataReader.GetString(tenantIdOrdinal);
                                var secret = dataReader.GetString(secretOrdinal);

                                azureApplications.Add(
                                    new AzureApplication
                                    {
                                        Id = azureApplicationId,
                                        Description = applicationDescription,
                                        Name = applicationName,
                                        ClientId = clientId,
                                        TenantId = tenantId,
                                        EncryptedSecret = secret,
                                        //Secret = secret,
                                    });
                            }
                        }
                    }
                    // Sort the applications
                    azureApplications.Sort((a1, a2) => string.CompareOrdinal(a1.Name, a2.Name));
                }
                catch (Exception e)
                {

                    Log.Error(String.Format(
                        "GetAzureApplications:: Error while accessing the SQL Server with the Connection String :[{0}]./n {1}",
                        (connection != null && connection.DataSource != null) ? connection.DataSource : string.Empty,
                        e.Message));
                    return null;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                        connection.Close();
                }

                connection.Close();
            }

            return azureApplications;
        }

        public static List<IAzureSubscription> GetAzureSubscriptions(SqlConnectionInfo connectionInfo)
        {
            var azureSubscriptions = new List<IAzureSubscription>();
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = new SqlConnection(connectionInfo.ConnectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure,
                            GetAzureSubscriptionsStoredProcedure))
                    {
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                var azureSubscriptionIdOrdinal = dataReader.GetOrdinal("AzureSubscriptionId");
                                var subscriptionIdOrdinal = dataReader.GetOrdinal("SubscriptionId");
                                var subscriptionDescriptionOrdinal = dataReader.GetOrdinal("SubscriptionDescription");

                                // Subscription Details
                                var azureSubscriptionId = dataReader.GetInt64(azureSubscriptionIdOrdinal);
                                var subscriptionId = dataReader.GetString(subscriptionIdOrdinal);
                                var subscriptionDescription = !dataReader.IsDBNull(subscriptionDescriptionOrdinal)
                                    ? dataReader.GetString(subscriptionDescriptionOrdinal)
                                    : null;

                                azureSubscriptions.Add(
                                    new AzureSubscription
                                    {
                                        Id = azureSubscriptionId,
                                        SubscriptionId = subscriptionId,
                                        Description = subscriptionDescription
                                    });
                            }
                        }
                    }
                    // Sort the subscriptions
                    azureSubscriptions.Sort((s1, s2) => string.CompareOrdinal(s1.SubscriptionId, s2.SubscriptionId));
                }
                catch (Exception e)
                {

                    Log.Error(String.Format(
                        "GetAzureSubscriptions:: Error while accessing the SQL Server with the Connection String :[{0}]./n {1}",
                        (connection != null && connection.DataSource != null) ? connection.DataSource : string.Empty,
                        e.Message));
                    return null;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                        connection.Close();
                }

                connection.Close();
            }

            return azureSubscriptions;
        }

        public static void InsertUpdateAzureSubscriptions(SqlConnectionInfo sqlConnectionInfo, string subscriptionId, string desc, long? azureSubscriptionId)
        {
            try
            {
                if (sqlConnectionInfo == null)
                {
                    throw new NullReferenceException("Connection not found");
                }
                using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, InsertUpdateAzureSubscriptionsStoredProcedure))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@subscriptionId", subscriptionId);
                        command.Parameters.AddWithValue("@desc", desc);
                        command.Parameters.AddWithValue("@azureSubscriptionId", azureSubscriptionId);
                        command.ExecuteScalar();
                    }
                }
            }

            catch (Exception ex)
            {
                Log.Error("InsertUpdateAzureSubscriptions:: Exception Found : " + ex);
            }
        }

        public static void InsertUpdateAzureAppProfile(SqlConnectionInfo sqlConnectionInfo, String name, String desc, long subscriptionId, long appProfileId, long? azureAppProfileId, out long newAzureAppProfileId)
        {
            // set the new azure profile id
            newAzureAppProfileId = azureAppProfileId != null ? azureAppProfileId.Value : 0;
            try
            {
                if (sqlConnectionInfo == null)
                {
                    throw new NullReferenceException("Connection not found");
                }
                using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, InsertUpdateAzureAppProfileStoredProcedure))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@desc", desc);
                        command.Parameters.AddWithValue("@subscriptionId", subscriptionId);
                        command.Parameters.AddWithValue("@appId", appProfileId);
                        command.Parameters.AddWithValue("@azureAppProfileId", azureAppProfileId);
                        command.Parameters.Add("@newAzureAppProfileId", SqlDbType.BigInt);
                        command.Parameters["@newAzureAppProfileId"].Direction = ParameterDirection.Output;
                        command.ExecuteScalar();
                        var outParm = command.Parameters["@newAzureAppProfileId"];
                        if (outParm.Value is long)
                        {
                            newAzureAppProfileId = (long)outParm.Value;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Log.Error("Exception Found : " + ex);
            }
        }

        public static void DeleteAzureSubscriptions(SqlConnectionInfo sqlConnectionInfo, String subscriptionId)
        {

            try
            {
                if (sqlConnectionInfo == null)
                {
                    throw new NullReferenceException("Connection not found");
                }
                using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, DeleteAzureSubscriptionsStoredProcedure))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@subscriptionId", subscriptionId);
                        command.ExecuteScalar();
                    }
                }
            }

            catch (Exception ex)
            {
                Log.Error("Exception Found : " + ex);
            }
        }

        public static void DeleteAzureApplicationProfile(SqlConnectionInfo sqlConnectionInfo, long appProfileId)
        {
            try
            {
                if (sqlConnectionInfo == null)
                {
                    throw new NullReferenceException("Connection not found");
                }
                using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, DeleteAzureApplicationProfileStoredProcedure))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@appProfileId", appProfileId);
                        command.ExecuteScalar();
                    }
                }
            }

            catch (Exception ex)
            {
                Log.Error("Exception Found : " + ex);
            }
        }

        public static void DeleteAzureProfile(SqlConnectionInfo sqlConnectionInfo, long appId)
        {
            try
            {
                if (sqlConnectionInfo == null)
                {
                    throw new NullReferenceException("Connection not found");
                }
                using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, DeleteAzureProfileStoredProcedure))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@profileId", appId);
                        command.ExecuteScalar();
                    }
                }
            }

            catch (Exception ex)
            {
                Log.Error("Exception Found : " + ex);
            }
        }

        public static void DeleteAzureApplication(SqlConnectionInfo sqlConnectionInfo, long appId)
        {
            try
            {
                if (sqlConnectionInfo == null)
                {
                    throw new NullReferenceException("Connection not found");
                }
                using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, DeleteAzureApplicationStoredProcedure))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@appId", appId);
                        command.ExecuteScalar();
                    }
                }
            }

            catch (Exception ex)
            {
                Log.Error("Exception Found : " + ex);
            }
        }
        public static long AddOrUpdateAzureApplication(string connectionString, AzureApplication azureApplication)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            long result = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction xa = null;
                try
                {
                    xa = connection.BeginTransaction();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, AddUpdateAzureApplicationProcedure))
                    {
                        command.Transaction = xa;
                        SqlHelper.AssignParameterValues(
                            command.Parameters,
                            azureApplication.Name,
                            azureApplication.ClientId,
                            azureApplication.Description,
                            azureApplication.EncryptedSecret,
                            azureApplication.TenantId,
                            azureApplication.Id);

                        command.ExecuteNonQuery();
                        object profileId = command.Parameters["@Id"].Value;
                        if (profileId is long)
                            result = (long)profileId;

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

         public static long AddUpdateAzureLinkedProfile(string connectionString, AzureProfile azureProfile)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }

            long result = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction xa = null;
                try
                {
                    xa = connection.BeginTransaction();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, AddUpdateAzureLinkedProfileProcedure))
                    {
                        command.Transaction = xa;
                        SqlHelper.AssignParameterValues(
                            command.Parameters,
                            azureProfile.Description,
                            azureProfile.SqlServerId,
                            azureProfile.ApplicationProfile.Id,
                            azureProfile.Id);

                        command.ExecuteNonQuery();
                        var profileId = command.Parameters["@Id"].Value;
                        if (profileId is long)
                            result = (long)profileId;

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
                                //Secret = secret,
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

        public static Dictionary<long, string> GetAzureApplicationProfile(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                string commandText = GetAzureApplicationProfilesDirectQuery;
                SqlCommand command = new SqlCommand(commandText, connection);
                SqlDataReader dataReader = command.ExecuteReader();
                return GetTable(dataReader, true, false).AsEnumerable().ToList().ToDictionary(row => row.Field<long>(0), row => row.Field<string>(1));
            }
        }
    }
}

