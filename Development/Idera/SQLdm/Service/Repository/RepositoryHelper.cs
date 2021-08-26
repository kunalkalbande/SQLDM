using System;
using System.Net;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Xml.Serialization;
using BBS.TracerX;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Snapshots;
using Microsoft.ApplicationBlocks.Data;
using Wintellect.PowerCollections;
using Constants = Idera.SQLdm.Common.Constants;
using XmlSerializerFactory = Idera.SQLdm.Common.Data.XmlSerializerFactory;
using System.Text;
using DC = Idera.SQLdm.Service.DataContracts.v1;
using Idera.SQLdm.Common.Services;
using BBS.License;
using System.ServiceModel.Web;
using Idera.SQLdm.Service.Helpers;
using Idera.SQLdm.Service.DataContracts.v1.Widgets;
using Idera.SQLdm.Service.Configuration;
using AuthDataContract = Idera.SQLdm.Service.DataContracts.v1.Auth;
using Idera.SQLdm.Service.Core.Enums;
using Idera.SQLdm.Service.DataContracts.v1.CustomDashboard;
using Idera.SQLdm.Service.DataContracts.v1.Errors;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Service.Helpers.Auth;
using Idera.SQLdm.Service.Events;
using Idera.SQLdm.Service.Web;
using Idera.SQLdm.Service.Helpers.CWF;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Resources;

namespace Idera.SQLdm.Service.Repository
{
    internal static class RepositoryHelper
    {
        #region Constants

        private const string GetRepositoryVersionSqlCommand = "select dbo.fn_GetDatabaseVersion()";
        private const string GetRepositoryInfoStoredProcedure = "p_RepositoryInfo";

        //Instance 
        private const string GetMonitoredSqlServersStoredProcedure = "p_GetMonitoredSqlServers";
        private const string GetMonitoredSqlServersByIdStoredProcedure = "p_GetMonitoredSqlServerById";
        //private const string GetServerSummaryStoredProcedure = "p_GetServerSummary";
        private const string GetAllServersSummaryStoredProcedure = "p_GetMonitoredServersSummary";
        private const string GetMonitoredSqlServerStatusStoredProcedure = "p_GetMonitoredSqlServerStatus";
        private const string GetTempdbSummaryDataStoredProcedure = "p_GetTempdbSummaryData";
        private const string GetInstanceResourcesStoredProcedure = "p_GetInstanceResources";
        private const string GetInstancesListStoredProcedure = "p_GetInstancesList";
        private const string GetBaselineStatisticsStoredProcedure = "p_GetBaselineStatistics"; //SQLdm 9.1 (Sanjali Makkar) (Baseline API): gets Baseline Statistics for metric
        private const string GetInstanceStatusStoredProcedure = "p_GetInstanceStatus"; // SQLdm 9.1 (Sanjali Makkar) (Instance Status) : Added to get Instances related information e.g. Critical Servers Count, Total Monitored Servers 

        // Alerts
        private const string GetAlertsStoredProcedure = "p_GetAlerts";
        private const string GetAlertsForWebConsoleStoredProcedure = "p_GetAlertsForWebConsole";
        private const string GetAlertStoredProcedure = "p_GetAlert";
        private const string GetAlertsHistoryStoredProcedure = "p_GetAlertsHistory";
        private const string GetMetricsHistoryForAlertStoredProcedure = "p_GetMetricsHistoryForAlert";

        // Management Service
        private const string GetDefaulManagementServiceStoredProcedure = "p_GetDefaultManagementService";

        //Top X
        private const string GetTopServersResponseTimeStoredProcedure = "p_GetTopInstancesByResponseTime";
        //private const string GetTopServersDatabaseAlertsStoredProcedure = "p_TopServersDatabaseAlerts";
        private const string GetTopQueriesByDurationStoredProcedure = "p_GetTopInstancesByQueryDuration";
        private const string GetTopBlockedSessionCountStoredProcedure = "p_GetTopBlockedSessionCount";
        private const string GetTopInstancesByQueryCountStoredProcedure = "p_GetTopInstancesByQueryCount";
        private const string GetTopInstancesBySessionCountStoredProcedure = "p_GetTopInstancesBySessionCount";
        private const string GetTopSessionsByCPUUsageStoredProcedure = "p_GetTopSessionsByCpuUsage";
        private const string GetTopDatabaseByActivityStoredProcedure = "p_GetTopDatabaseByActivity";
        private const string GetTopDatabaseByProjectedGrowthStoredProcedure = "p_GetTopDatabaseByProjectedGrowth";
        private const string GetTopSessionsByIOActivityStoredProcedure = "p_GetSessionList"; //SQLdm 9.1 (Sanjali Makkar): for Top X API- Sessions by I/O Activity
        private const string GetInstancesByTempDbUtilizationStoredProcedure = "p_GetTopXServerByTempDbUtilization"; //SQLdm 8.5 (Ankit Srivastava): for Top X API- Tempdb Utilization
        private const string GetInstancesByQueriesStoredProcedure = "p_GetTopInstancesByQueries"; //SQLdm 8.5 (Ankit Srivastava): for Top X API- Query Monitor Event
        private const string GetInstancesByDiskSpaceStoredProcedure = "p_GetTopInstancesByDiskSpace"; //SQLdm 8.5 (Ankit Srivastava): for Top X API- Disk Space

        //category view
        //p_GetInstanceQueryStats
        private const string GetInstanceQueryStatsStoredProcedure = "p_GetInstanceQueryStats"; //SQLdm 8.5 (Gaurav Karwal): for category specific queries view
        private const string GetSessionStatisticsForWebConsoleStoredProcedure = "p_GetSessionStatisticsForWebConsole";//SQLdm 8.5 (Gaurav Karwal): for category specific queries view
        private const string GetWaitStatisticsStoredProcedure = "p_GetWaitStatistics";//SQLdm 8.5 (Gaurav Karwal): for category specific queries view
        private const string GetFileActivityStoredProcedure = "p_GetFileActivity";//SQLdm 8.5 (Gaurav Karwal): for category specific queries view
        private const string GetDiskDrivesStoredProcedure = "p_GetDiskDrives";//SQLdm 8.5 (Gaurav Karwal): for category specific queries view


        //New Response Format --Ankit
        private const string GetTopInstancesByAlertsStoredProcedure = "p_GetTopInstancesByAlerts";//SQLdm 8.5 (Ankit Srivastava): for Top X API- Instance Alerts
        private const string GetTopDatabasesByAlerts = "p_GetTopDatabasesByAlerts";//SQLdm 8.5 (Ankit Srivastava): for Top X API- Most Alerts for Databases
        private const string GetTopInstancesBySqlCpuLoad = "p_GetTopInstancesByCPUUsage";//SQLdm 8.5 (Ankit Srivastava): for Top X API-  Sql Cpu Load
        private const string GetTopInstancesByIOPhysicalCount = "p_GetTopInstancesByIO";//SQLdm 8.5 (Ankit Srivastava): for Top X API- IO Physical Count
        private const string GetTopInstancesBySqlMemoryUsage = "p_GetTopInstancesBySQLMemory";//SQLdm 8.5 (Ankit Srivastava): for Top X API-  Sql Memory Usage

        private const string GetLatestResponseTimesByInstanceProcedure = "p_GetLatestResponseTimesByInstance";

        // get serverstaticsticHistory
        private const string GetServerOverviewHistoryStoredProcedure = "p_GetServerOverviewHistory";

        private const string GetDatabaseOverviewStoredProcedure = "p_GetDatabaseOverview"; //SQLdm 8.5 (Gaurav Karwal): added this for getting database details
        private const string GetDatabaseDetailsByInstanceStoredProcedure = "p_GetDatabaseByInstance"; //SQLdm 8.5 (Gaurav Karwal): added this for getting database details

        private const string GetTopInstancesByConnectionCountStoredProcedure = "p_GetTopInstancesByConnections";//SQLdm 8.5 (Gaurav Karwal): gets instances by connection count
        private const string GetTopDatabasesBySizeStoredProcedure = "p_GetTopDatabasesBySizeForWebConsole";//SQLdm 8.5 (Gaurav Karwal): gets instances by db size
        private const string GetTopInstancesByWaitsStoredProcedure = "p_GetWaitsByInstance";//SQLdm 8.5 (Gaurav Karwal): gets instances by waits
        private const string GetQueryWaitStatisticsStoredProcedure = "p_GetActiveWaits";//SQLdm 8.5 (Gaurav Karwal): gets instances by waits
        private const string GetQueryWaitOverviewStoredProcedure = "p_GetOverviewQueryWaits";
        private const string GetLatestStatisticCollectionTime = "SELECT MAX(dateadd(ss,CONVERT(DECIMAL(16,4),CONVERT(DECIMAL(16,4),ISNULL([WaitDuration],0))/1000),[StatementUTCStartTime])) FROM [ActiveWaitStatistics] (NOLOCK) WHERE [SQLServerID] = ";
        private const string GetTopDatabasesByGrowthStoredProcedure = "p_GetTopDatabasesByGrowth";//SQLdm 8.5 (Gaurav Karwal): gets databases based on growth
        private const string GetInstanceAvailbilityGroupDetailsStoredProcedure = "p_GetInstanceAvailbilityGroupDetails";

        //auth
        private const string GetSecurityEnabledStoredProcedure = "p_IsAppSecEnabled";
        private const string GetWebApplicationUserDetailsStoredProcedure = "p_GetWebApplicationUser";
        private const string AuthenticateWebConsoleUserAppSecOnStoredProcedure = "p_AuthenticateWebConsoleUserAppSecOn";

        private const string GetMetricInfoStoredProcedure = "p_GetMetricInfo";
        private const string GetAlertsCountPerCategory = "p_GetAlertsCountPerCategory";
        private const string GetAlertsCountPerDatabase = "p_GetAlertsCountPerDatabase";
        private const string GetServerActivityStoredProcedure = "p_GetServerActivity";
        //private const string GetSessionsDetailsStoredProcedure = "p_GetSessionsDetails";
        private const string GetTagsStoredProcedure = "p_GetTags";

        private const string GetServersWithTagIdStoredProcedure = "p_GetServersWithTagId";

        //QueryManager
        private const string GetApplicationsForServer = "p_GetApplicationsForServer";  // SQLdm 9.0 (Abhishek Joshi): added this for getting applications list for a SQL Server
        private const string GetClientsForServer = "p_GetClientsForServer";  // SQLdm 9.0 (Abhishek Joshi): added this for getting clients list for a SQL Server
        private const string GetUsersForServer = "p_GetUsersForServer";  // SQLdm 9.0 (Abhishek Joshi): added this for getting users list for a SQL Server
        private const string GetDatabasesForServer = "p_GetDatabasesForServer";  // SQLdm 9.0 (Abhishek Joshi): added this for getting databases list for a SQL Server
        private const string GetQueryPlan = "p_GetQueryPlan";  // SQLdm 9.0 (Abhishek Joshi): added this for getting the query plan for a SQLStatementID
        private const string GetQueryMonitorDataByApplication = "p_GetQueryMonitorDataByApplication";  // SQLdm 9.0 (Abhishek Joshi): added this for getting the query monitor data when grouped by Application
        private const string GetQueryMonitorDataByDatabase = "p_GetQueryMonitorDataByDatabase";  // SQLdm 9.0 (Abhishek Joshi): added this for getting the query monitor data when grouped by Database
        private const string GetQueryMonitorDataByUser = "p_GetQueryMonitorDataByUser";  // SQLdm 9.0 (Abhishek Joshi): added this for getting the query monitor data when grouped by User
        private const string GetQueryMonitorDataByClient = "p_GetQueryMonitorDataByClient";  // SQLdm 9.0 (Abhishek Joshi): added this for getting the query monitor data when grouped by Client
        private const string GetQueryMonitorDataByQuerySignature = "p_GetQueryMonitorDataByQuerySignature";  // SQLdm 9.0 (Abhishek Joshi): added this for getting the query monitor data when grouped by Query Signature
        private const string GetQueryMonitorDataByQueryStatement = "p_GetQueryMonitorDataByQueryStatement";  // SQLdm 9.0 (Abhishek Joshi): added this for getting the query monitor data when grouped by Query Statement
        private const string GetGraphRepresentationDataByApplication = "p_GetGraphRepresentationDataByApplication"; // SQLdm 9.0 (Abhishek Joshi): added this for getting the query monitor data when grouped by Application for graph representation
        private const string GetGraphRepresentationDataByDatabase = "p_GetGraphRepresentationDataByDatabase"; // SQLdm 9.0 (Abhishek Joshi): added this for getting the query monitor data when grouped by database for graph representation
        private const string GetGraphRepresentationDataByUser = "p_GetGraphRepresentationDataByUser"; // SQLdm 9.0 (Abhishek Joshi): added this for getting the query monitor data when grouped by user for graph representation
        private const string GetGraphRepresentationDataByClient = "p_GetGraphRepresentationDataByClient"; // SQLdm 9.0 (Abhishek Joshi): added this for getting the query monitor data when grouped by client for graph representation
        private const string GetGraphRepresentationDataByQuerySignature = "p_GetGraphRepresentationDataByQuerySignature"; // SQLdm 9.0 (Abhishek Joshi): added this for getting the query monitor data when grouped by Query Signature for graph representation
        private const string GetGraphRepresentationDataByQueryStatement = "p_GetGraphRepresentationDataByQueryStatement"; // SQLdm 9.0 (Abhishek Joshi): added this for getting the query monitor data when grouped by Query Statement for graph representation

        private const string GetAllAlertsAdvFilterStoredProcedure = "p_GetAlertsAdvanceFilters";
        private const string DeleteAlertsAdvFilterStoredProcedure = "p_DeleteAlertsAdvanceFilter";
        private const string AddAlertsAdvFilterStoredProcedure = "p_AddAlertsAdvanceFilter";

        public const int DefaultRecordsCount = 10; // SQLdm 9.0 (Abhishek Joshi): -- WebService.IQueryManager constant - default records to be sent, in case of some ambiguity
        private const int MaximumYAxisLimit = 9; // SQLdm 9.0 (Abhishek Joshi): --default number of values to be shown in Query Monitor graphs for a group, remaining to be rolled up in 'Others', if any 

        // SQLdm 9.0 (Abhishek Joshi): - WebService.IQueryManager supported grouping constant strings
        public const string GroupByApplication = "Application";
        public const string GroupByDatabase = "Database";
        public const string GroupByUser = "User";
        public const string GroupByClient = "Client";
        public const string GroupByQuerySignature = "Query Signature";
        public const string GroupByQueryStatement = "Query Statement";

        // SQLdm 9.0 (Abhishek Joshi): -- WebService.IQueryManager supported metrics constant strings
        public const string DurationView = "Duration (ms)";
        public const string CPUTimeView = "CPU Time (ms)";
        public const string ReadsView = "Reads";
        public const string WritesView = "Writes";
        public const string InputOutputView = "I/O (sum of reads and writes)";
        public const string BlockingDurationView = "Blocking Duration (ms)";
        public const string WaitDurationView = "Wait Duration (ms)";
        public const string DeadlocksView = "Deadlocks";

        // SQLdm 9.1 (Sanjali Makkar): Severity constants
        public const int CRITICAL_ALERT_SEVERITY = 8;
        public const int WARNING_ALERT_SEVERITY = 4;
        public const int INFORMATIONAL_ALERT_SEVERITY = 2;
        public const int OK_ALERT_SEVERITY = 1;


        //SQLdm 9.1 (Gaurav Karwal): For Product Info Tab
        public const string GetProductStatusStoredProcedure = "p_GetProductStatus";
        public const int MAX_HEALTH_INDEX_DEFAULT = -1;

        // SQLdm 9.1 (Sanjali Makkar) (Health Index): For Maximum Value of Health Index
        public static decimal MaxHealthIndex = MAX_HEALTH_INDEX_DEFAULT;

        // SQLdm 10.0 (Srishti Purohit) Custom Dashboard Implementation
        public const string GetCustomDashboardsStoredProcedure = "p_GetCustomDashboards";
        public const string GetWidgetTypesStoredProcedure = "p_GetWidgetTypes";
        public const string GetMatchTypesStoredProcedure = "p_GetMatchTypes";
        public const string CheckDuplicateCustomDashboardNameStoredProcedure = "p_CheckDuplicateCustomDashboardName";
        public const string CheckDashboardExistsStoredProcedure = "p_CheckDashboardExists";
        public const string DeleteCustomDashboardRecordStoredProcedure = "p_DeleteCustomDashboardRecord";
        public const string DeleteCustomDashboardWidgetRecordStoredProcedure = "p_DeleteCustomDashboardWidgetRecord";
        public const string GetWidgetsofDashboardStoredProcedure = "p_GetWidgetsofDashboard";
        public const string UpsertCustomDashboardStoredProcedure = "p_UpsertCustomDashboard";
        public const string CreateCopyOfCustomDashboardStoredProcedure = "p_CreateCopyOfCustomDashboard";
        public const string UpsertCustomDashboardWidgetStoredProcedure = "p_UpsertCustomDashboardWidget";
        public const string GetMetricsHistoryForCustomDashboardStoredProcedure = "p_GetMetricsHistoryForCustomDashboard";

        // SQLdm 10.1 (Srishti Purohit) health index customisation Implementation
        public const string GetScaleFactorsStoredProcedure = "p_GetScaleFactors";
        public const string UpdateHealthIndexScaleFactorStoredProcedure = "p_UpdateHealthIndexScaleFactor";


        //SQLdm 10.1 (Pulkit Puri) --global tags implementation
        private const int ServerNotFound = -1;

        //SQLdm 10.2 (Anshika Sharma) : Store User Session Settings
        private const string SetUserSessionSettingsStoredProcedure = "p_SetUserSessionSettings";
        private const string GetUserSessionSettingsStoredProcedure = "p_GetUserSessionSettings";

        //SQLdm 10.2 (Nishant Adhikari) : Consolidated Instance Overview
        private const string GetPreviousAnalysisInfoStoredProcedure = "p_GetPreviousAnalysisInfo";
        private const string GetRecommendationSummaryStoredProcedure = "p_GetRecommendationSummary";
        //SQLdm 10.2 (Nishant Adhikari) : OS Paging Per Second
        private const string GetOSMemoryPagesPerSecondStoredProcedure = "p_GetOSMemoryPagesPerSecond";
        //SQLdm 10.2 (Nishant Adhikari) : DB Running Statistics
        private const string GetDBRunningStatsSecondStoredProcedure = "p_GetDatabaseRunningStatistics";
        //SQL dm 10.2 (Nishant Adhikari) : CPU Running Statistics
        private const string GetCPUStatisticsStoredProcedure = "p_GetCPUStatistics";
        //SQL dm 10.2 (Nishant Adhikari) : Network Statistics
        private const string GetNetworkStatisticsStoredProcedure = "p_GetNetworkStatistics";
        //SQL dm 10.2 (Nishant Adhikari) : Custom Counter Statistics
        private const string CustomCounterStatisticsStoredProcedure = "p_GetCustomCounterStatisticsforweb";
        //SQL dm 10.2 (Nishant Adhikari) : File Read Write Transfer Statistics
        private const string GetFileRWTActivityStoredProcedure = "p_GetFileRWTActivity";
        //SQL dm 10.2 (Nishant Adhikari) : Locks Statistics
        private const string GetLocksDetailsStoredProcedure = "p_GetLocksDetails";
        //SQL dm 10.2 (Nishant Adhikari) : Server Waits Statistics(Dashboard)
        private const string GetServerWaitsDashboardStoredProcedure = "p_GetWaitStatisticsForDashboard";
        //SQL dm 10.2 (Nishant Adhikari) : Virtualization Statistics
        private const string GetVirtualizationStatisticsStoredProcedure = "p_GetVirtualizationStatisticsForWeb";

        // SQLdm 10.1.2 (Varun Chopra) QueryMonitor Optimization Constant Fields
        private const string OTHER = "Other";
        private const string DATE_FORMAT = "dd-MM-yyyy HH:mm:ss";
        private const string STARTTIME = "StartTime";
        private const string GROUPBYID = "GroupByID";
        private const string GROUPBYNAME = "GroupByName";
        private const string YAXISVALUE = "YAxisValue";
        private const string COMMASEPARATOR = ",";

        private const string GetAnalysisListingFromDBStoreProcedure = "p_GetAnalysisListing";
        private const string GetRecommendationList = "p_GetRecommendationList";
        // SQLdm 10.1.2 (Varun Chopra) QueryMonitor Optimization 

        /// <summary>
        /// BucketRange for storing Buckets of Time range
        /// </summary>
        private class BucketRange
        {
            public string BucketStartDateTime { get; set; }

            public string BucketEndDateTime { get; set; }

            public override string ToString()
            {
                return BucketStartDateTime + BucketEndDateTime;
            }
        }

        #endregion

        private static readonly Logger Log = SQLdmLogHelper.GetSQLdmLogger("RepositoryHelper");

        #region Test
        private static object GetValueOrDefault<T>(object obj)
        {
            if (DBNull.Value == obj)
            {
                if (typeof(T) == typeof(string))
                {
                    return string.Empty;
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    return Convert.ToDateTime("1-1-1900");
                }
                else
                {
                    return default(T);
                }
            }
            else
            {
                return obj;
            }
        }

        public static bool IsValidRepository(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
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
                // Use RestServiceConfiguration.SQLConnectInfo.
                if (RestServiceConfiguration.SQLConnectInfo == null)
                {
                    throw new ArgumentNullException("connectionInfo");
                }
                else
                {
                    connectionInfo = RestServiceConfiguration.SQLConnectInfo;
                }
            }

            return connectionInfo;
        }

        public static RepositoryInfo GetRepositoryInfo(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
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

        public static DatabaseState GetDatabaseState(SqlConnectionInfo connectionInfo)
        {
            connectionInfo = CheckConnectionInfo(connectionInfo);
            var masterConnectionInfo = connectionInfo.Clone() as SqlConnectionInfo;
            masterConnectionInfo.DatabaseName = "master";

            using (SqlConnection connection =
                    masterConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                string cmdText = string.Format("SELECT db.name, db.state, db.state_desc FROM [master].[sys].[databases] db WITH (NOLOCK) WHERE db.name = '{0}' ORDER BY db.name", SQLServerHelper.EscapeQuotes(connectionInfo.DatabaseName));
                using (SqlCommand command = new SqlCommand(cmdText, connection))
                {
                    command.CommandTimeout = Constants.DefaultCommandTimeout;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return (new DatabaseState(reader));
                        }
                    }
                }
                return (null);
            }
        }

        public static string GetRepositoryVersion(SqlConnectionInfo connectionInfo)
        {

            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {

                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, CommandType.Text,
                                                GetRepositoryVersionSqlCommand))
                {
                    dataReader.Read();
                    return dataReader.GetString(0);
                }
            }
        }

        #endregion

        #region MonitoredSqlServer

        private static DC.MonitoredSqlServer ConstructMonitoredSqlServerStatus(string xmlText)
        {
            DC.MonitoredSqlServer status = null;

            if (xmlText != null)
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xmlText);

                if (xmlDocument.DocumentElement.HasChildNodes)
                {
                    status = new DC.MonitoredSqlServer(xmlDocument.DocumentElement.FirstChild);
                }
            }

            return status;
        }

        private static MonitoredSqlServer ConstructMonitoredSqlServer(SqlDataReader dataReader)
        {
            if (dataReader == null)
            {
                throw new ArgumentNullException("dataReader");
            }

            int returnId = (int)GetValueOrDefault<int>(dataReader["SQLServerID"]);
            SqlConnectionInfo instanceConnectionInfo = new SqlConnectionInfo();
            instanceConnectionInfo.InstanceName = GetValueOrDefault<string>(dataReader["InstanceName"]) as string;

            instanceConnectionInfo.ApplicationName = Constants.CollectionServceConnectionStringApplicationName;
            bool isActive = (bool)GetValueOrDefault<bool>(dataReader["Active"]);
            DateTime registeredDate = (DateTime)GetValueOrDefault<DateTime>(dataReader["RegisteredDate"]); // UTC
            int collectionServiceIdColumn = dataReader.GetOrdinal("CollectionServiceID");
            SqlGuid sqlGuid = dataReader.GetSqlGuid(collectionServiceIdColumn);
            Guid collectionServiceId = sqlGuid.IsNull ? Guid.Empty : sqlGuid.Value;

            int scheduledCollectionInterval = (int)GetValueOrDefault<int>(dataReader["ScheduledCollectionIntervalInSeconds"]);
            bool maintenanceModeEnabled = (bool)GetValueOrDefault<bool>(dataReader["MaintenanceModeEnabled"]);

            AdvancedQueryMonitorConfiguration queryMonitorAdvancedConfiguration = null;
            SqlString queryMonitorAdvancedConfigurationXml = dataReader.GetSqlString(dataReader.GetOrdinal("QueryMonitorAdvancedConfiguration"));
            if (!queryMonitorAdvancedConfigurationXml.IsNull)
            {
                queryMonitorAdvancedConfiguration =
                    AdvancedQueryMonitorConfiguration.DeserializeFromXml(queryMonitorAdvancedConfigurationXml.Value);
            }

            QueryMonitorConfiguration queryMonitorConfiguration = new QueryMonitorConfiguration(
                (bool)GetValueOrDefault<bool>(dataReader["QueryMonitorEnabled"]),
                (bool)GetValueOrDefault<bool>(dataReader["QueryMonitorSqlBatchEventsEnabled"]),
                (bool)GetValueOrDefault<bool>(dataReader["QueryMonitorSqlStatementEventsEnabled"]),
                (bool)GetValueOrDefault<bool>(dataReader["QueryMonitorStoredProcedureEventsEnabled"]),
                TimeSpan.FromMilliseconds((int)GetValueOrDefault<int>(dataReader["QueryMonitorDurationFilterInMilliseconds"])),
                TimeSpan.FromMilliseconds((int)GetValueOrDefault<int>(dataReader["QueryMonitorCpuUsageFilterInMilliseconds"])),
                (int)GetValueOrDefault<int>(dataReader["QueryMonitorLogicalDiskReadsFilter"]),
                (int)GetValueOrDefault<int>(dataReader["QueryMonitorPhysicalDiskWritesFilter"]),
                new FileSize((int)GetValueOrDefault<int>(dataReader["QueryMonitorTraceFileSizeKB"])),
                (int)GetValueOrDefault<int>(dataReader["QueryMonitorTraceFileRollovers"]),
                (int)GetValueOrDefault<int>(dataReader["QueryMonitorTraceRecordsPerRefresh"]),
                queryMonitorAdvancedConfiguration,
                dataReader.IsDBNull(dataReader.GetOrdinal("QueryMonitorStopTimeUTC"))
                    ? null
                    : (DateTime?)GetValueOrDefault<DateTime>(dataReader["QueryMonitorStopTimeUTC"]),
                false,
                false,
                false, //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session -- added default values of new parameters
                (int)GetValueOrDefault<int>(dataReader["QueryMonitorTopPlanCountFilter"]),
                (int)GetValueOrDefault<int>(dataReader["QueryMonitorTopPlanCategoryFilter"]),
                (bool)dataReader["QueryMonitorQueryStoreMonitoringEnabled"]);  // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store

            ActivityMonitorConfiguration activityProfilerConfiguration = new ActivityMonitorConfiguration(
                (bool)GetValueOrDefault<bool>(dataReader["ActivityMonitorEnabled"]),
                (bool)GetValueOrDefault<bool>(dataReader["ActivityMonitorDeadlockEventsEnabled"]),
                (bool)GetValueOrDefault<bool>(dataReader["ActivityMonitorBlockingEventsEnabled"]),
                (bool)GetValueOrDefault<bool>(dataReader["ActivityMonitorAutoGrowEventsEnabled"]),
                (int)GetValueOrDefault<int>(dataReader["ActivityMonitorBlockedProcessThreshold"]),
                new FileSize((int)GetValueOrDefault<int>(dataReader["QueryMonitorTraceFileSizeKB"])),
                (int)GetValueOrDefault<int>(dataReader["QueryMonitorTraceFileRollovers"]),
                (int)GetValueOrDefault<int>(dataReader["QueryMonitorTraceRecordsPerRefresh"]),
                queryMonitorAdvancedConfiguration,
                dataReader.IsDBNull(dataReader.GetOrdinal("QueryMonitorStopTimeUTC"))
                    ? null
                    : (DateTime?)GetValueOrDefault<DateTime>(dataReader["QueryMonitorStopTimeUTC"])
                    , true);//SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events -- added default value of new parameter

            MaintenanceMode maintenanceMode = new MaintenanceMode();
            if (dataReader.IsDBNull(dataReader.GetOrdinal("MaintenanceModeType")) == false)
            {
                maintenanceMode.MaintenanceModeType = (MaintenanceModeType)(int)GetValueOrDefault<int>(dataReader["MaintenanceModeType"]);
            }
            if (dataReader.IsDBNull(dataReader.GetOrdinal("MaintenanceModeStart")) == false)
            {
                maintenanceMode.MaintenanceModeStart = (DateTime)GetValueOrDefault<DateTime>(dataReader["MaintenanceModeStart"]);
            }
            if (dataReader.IsDBNull(dataReader.GetOrdinal("MaintenanceModeStop")) == false)
            {
                maintenanceMode.MaintenanceModeStop = (DateTime)GetValueOrDefault<DateTime>(dataReader["MaintenanceModeStop"]);
            }
            if (dataReader.IsDBNull(dataReader.GetOrdinal("MaintenanceModeDurationSeconds")) == false)
            {
                maintenanceMode.MaintenanceModeDuration =
                    TimeSpan.FromSeconds((int)GetValueOrDefault<int>(dataReader["MaintenanceModeDurationSeconds"]));
            }
            if (dataReader.IsDBNull(dataReader.GetOrdinal("MaintenanceModeDays")) == false)
            {
                maintenanceMode.MaintenanceModeDays = (short)GetValueOrDefault<short>(dataReader["MaintenanceModeDays"]);
            }
            if (dataReader.IsDBNull(dataReader.GetOrdinal("MaintenanceModeRecurringStart")) == false)
            {
                maintenanceMode.MaintenanceModeRecurringStart = (DateTime)GetValueOrDefault<DateTime>(dataReader["MaintenanceModeRecurringStart"]);
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
            instance.FriendlyServerName = GetValueOrDefault<string>(dataReader["FriendlyServerName"]) as string;// SQLdm 10.1 -(Pulkit Puri) For populating Friendly server name
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

            instance.ReorganizationMinimumTableSize.Kilobytes = (int)GetValueOrDefault<int>(dataReader["ReorgMinTableSizeKB"]);

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
                //SQLdm 9.1 (Ankit Srivastava) - Activity Monitoring with Extended Events - Enum made public
                if (!dataReader.IsDBNull(ordinal)) awc.EventRetentionModeXe = (XeEventRetentionMode)dataReader.GetByte(ordinal);

                ordinal = dataReader.GetOrdinal("ActiveWaitXEMaxDispatchLatencySecs");
                if (!dataReader.IsDBNull(ordinal)) awc.MaxDispatchLatencyXe = dataReader.GetInt32(ordinal);

                ordinal = dataReader.GetOrdinal("ActiveWaitXEMaxEventSizeMB");
                if (!dataReader.IsDBNull(ordinal)) awc.MaxEventSizeXemb = dataReader.GetInt32(ordinal);

                ordinal = dataReader.GetOrdinal("ActiveWaitXEMemoryPartitionMode");
                //SQLdm 9.1 (Ankit Srivastava) - Activity Monitoring with Extended Events - Enum made public
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
                instance.VirtualizationConfiguration = new VirtualizationConfiguration((string)GetValueOrDefault<string>(dataReader["VmUID"]),
                                                                                       (string)GetValueOrDefault<string>(dataReader["VmName"]),
                                                                                       (string)GetValueOrDefault<string>(dataReader["VmDomainName"]),
                                                                                       (int)GetValueOrDefault<int>(dataReader["VHostID"]),
                                                                                       (string)GetValueOrDefault<string>(dataReader["VHostName"]),
                                                                                       (string)GetValueOrDefault<string>(dataReader["VHostAddress"]),
                                                                                       (string)GetValueOrDefault<string>(dataReader["VCUserName"]),
                                                                                       (string)GetValueOrDefault<string>(dataReader["VCPassword"]),
                                                                                       (string)GetValueOrDefault<string>(dataReader["ServerType"]));

            if (dataReader["BaselineTemplate"] != DBNull.Value)
            {
                instance.BaselineConfiguration = new BaselineConfiguration((string)GetValueOrDefault<string>(dataReader["BaselineTemplate"]));
                instance.BaselineConfiguration.TemplateID = (int)GetValueOrDefault<int>(dataReader["BaselineTemplateID"]);
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
                instance.MostRecentSQLVersion = new ServerVersion((string)GetValueOrDefault<string>(dataReader["ServerVersion"]));

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

            if (dataReader["ServerEdition"] != DBNull.Value)
                instance.MostRecentSQLEdition = (string)GetValueOrDefault<string>(dataReader["ServerEdition"]);


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
                                                (string)GetValueOrDefault<string>(dataReader["SS_InstanceName"]),
                                                (string)GetValueOrDefault<string>(dataReader["SS_DatabaseName"]),
                                                (bool)GetValueOrDefault<bool>(dataReader["SS_SecurityMode"]),
                                                (string)GetValueOrDefault<string>(dataReader["SS_UserName"]),
                                                (string)GetValueOrDefault<string>(dataReader["SS_EncryptedPassword"]),
                                                (int)GetValueOrDefault<int>(dataReader["SS_RelatedInstanceId"]),
                                                (int)GetValueOrDefault<int>(dataReader["SS_LastBackupActionId"]),
                                                (int)GetValueOrDefault<int>(dataReader["SS_LastDefragActionId"])
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

        public static MonitoredSqlServer GetMonitoredSqlServer(SqlConnectionInfo connectionInfo, int id)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
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

        public static IList<MonitoredSqlServer> GetMonitoredSqlServers(SqlConnectionInfo connectionInfo, Boolean activeOnly, UserToken UserToken, string FilterField = null, string FilterValue = null)
        {
            return GetMonitoredSqlServers(connectionInfo, activeOnly, UserToken, true, null, FilterField, FilterValue);
        }

        public static IDictionary<string, int> GetInstanceNameToId(SqlConnectionInfo connectionInfo)
        {
            IDictionary<string, int> result = new Dictionary<string, int>();

            if (connectionInfo != null)
            {
                using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();

                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetInstancesListStoredProcedure))
                    {
                        while (reader.Read())
                        {
                            result.Add(reader.GetString(reader.GetOrdinal("InstanceName")), (int)GetValueOrDefault<int>(reader["SQLServerID"]));
                        }

                    }
                }
            }
            return result;
        }



        public static IDictionary<int, string> GetInstanceIdToName(SqlConnectionInfo connectionInfo)
        {
            IDictionary<int, string> result = new Dictionary<int, string>();

            if (connectionInfo != null)
            {
                using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();

                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetInstancesListStoredProcedure))
                    {
                        while (reader.Read())
                        {
                            result.Add((int)GetValueOrDefault<int>(reader["SQLServerID"]), reader.GetString(reader.GetOrdinal("InstanceName")));
                        }

                    }
                }
            }
            return result;
        }
        /// <summary>
        /// To get license keys in addition to the servers, pass a non-null value
        /// for the licenseKeys parameter.  It will be cleared and then populated
        /// from the stored proc's result set. 
        /// </summary>
        public static IList<MonitoredSqlServer> GetMonitoredSqlServers(SqlConnectionInfo connectionInfo, Boolean activeOnly, UserToken userToken, bool customCounters, List<string> licenseKeys, string FilterField = null, string FilterValue = null)
        {
            connectionInfo = CheckConnectionInfo(connectionInfo);

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlCommand command = SqlHelper.CreateCommand(connection, GetMonitoredSqlServersStoredProcedure))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, null, activeOnly, licenseKeys != null, customCounters, FilterField, FilterValue);
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        Dictionary<int, MonitoredSqlServer> instances = new Dictionary<int, MonitoredSqlServer>();

                        while (dataReader.Read())
                        {
                            MonitoredSqlServer instance = ConstructMonitoredSqlServer(dataReader);
                            instances.Add(instance.Id, instance);
                        }

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
                        // SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                        return instances.Values.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.Id)).ToList();
                        //return new List<MonitoredSqlServer>(instances.Values);
                    }
                }
            }
        }

        #endregion

        #region Alerts

        /*public static IList<DC.Alert> GetAlerts(SqlConnectionInfo connectionInfo, string[] instanceList, string databaseName, 
            int startingAlertId, int maxRows, DateTime startDate, DateTime endDate, string severity, string metricId, MetricCategory? category, bool isActive)
        {
            string xml = null;
            if (instanceList.Length > 0)
            {

                if (instanceList != null && instanceList.Length  > 0)
                {
                    StringBuilder xmldoc = new StringBuilder();
                    using (XmlWriter writer = XmlWriter.Create(xmldoc))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("Servers");

                        foreach (string instanceName in instanceList)
                        {
                            writer.WriteStartElement("Server");
                            writer.WriteAttributeString("InstanceName", instanceName);
                            writer.WriteEndElement();

                        }
                        writer.WriteEndElement();
                    }
                    xml = xmldoc.ToString();
                }                
            }

            //ToDo: To cache this later
            MetricDefinitions md = new MetricDefinitions(false, false, true);
            md.Load(connectionInfo.ConnectionString);

            using (
               SqlConnection connection =
                   connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                 int rows = maxRows;
                 using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetAlertsStoredProcedure,
                     startingAlertId,
                     !startDate.Equals(default(DateTime)) ? startDate : SqlDateTime.MinValue,
                     !endDate.Equals(default(DateTime)) ? endDate : DateTime.Now,                     
                     xml, databaseName, null,
                     severity,
                     metricId,
                     category.HasValue ? category.Value.ToString() : null,
                     isActive, // Send active alerts if none specified
                     rows))
                 {
                     IList<DC.Alert> alerts = new List<DC.Alert>();

                     while (reader.Read())
                     {
                         var ordinal = -1;
                         DC.Alert alert = new DC.Alert();                         
                         alert.Metric = ConvertToDataContract.ToDC(md.GetMetricDefinition(reader.GetInt32(reader.GetOrdinal("Metric"))));
                         alert.AlertId = reader.GetInt64(reader.GetOrdinal("AlertID")); 
                         alert.UTCOccurrenceDateTime = reader.GetDateTime(reader.GetOrdinal("UTCOccurrenceDateTime"));                                                  
                         alert.ServerName = reader.GetString(reader.GetOrdinal("ServerName"));

                         ordinal = reader.GetOrdinal("DatabaseName");
                         alert.DatabaseName = reader.IsDBNull(ordinal) ? null:reader.GetString(ordinal);

                         alert.IsActive = reader[5].GetType() == typeof(Boolean) ? Convert.ToInt32(reader.GetBoolean(5)) : reader.GetInt32(5);
                         alert.Severity = reader.GetByte(reader.GetOrdinal("Severity"));
                         alert.StateEvent = reader.GetByte(reader.GetOrdinal("StateEvent"));
                         if (reader["Value"] != DBNull.Value)
                         {
                             alert.Value = (double?)reader["Value"];
                         }
                         alert.Heading = reader.GetString(reader.GetOrdinal("Heading"));
                         alert.Message = reader.GetString(reader.GetOrdinal("Message"));
                         alerts.Add(alert);                         
                     }

                     return alerts;
                 }
            }
        }*/

        static IDictionary<int, Type> metricToEnum = new Dictionary<int, Type>();
        static RepositoryHelper()
        {
            metricToEnum.Add(10, Idera.SQLdm.Common.Snapshots.ServiceState.Running.GetType()); //SQL Server Agent Status
            metricToEnum.Add(12, Idera.SQLdm.Common.Snapshots.ServiceState.Running.GetType()); //SQL Server Status
            metricToEnum.Add(14, DatabaseStatus.Normal.GetType()); //Database Status
            metricToEnum.Add(18, Idera.SQLdm.Common.Snapshots.ServiceState.Running.GetType()); //DTC Status
            metricToEnum.Add(19, FullTextSearchPopulationStatus.Idle.GetType()); //Full-Text Search Status
            metricToEnum.Add(23, Idera.SQLdm.Common.Snapshots.OSMetricsStatus.Available.GetType()); //OS Metrics Collection Status
            metricToEnum.Add(35, SQLServerAgentJobStatus.failed.GetType());//SQL Server Agent Job Failure
            metricToEnum.Add(48, MaintenanceModeStatus.Disabled.GetType());//Maintenance Mode Enabled
            metricToEnum.Add(49, MaintenanceModeStatus.Enabled.GetType());//CLR Enabled
            metricToEnum.Add(50, OLEAutomationStatus.Enabled.GetType());//OLE Automation Disabled
            metricToEnum.Add(52, SQLdmServiceStatus.Running.GetType());//SQL diagnostic manager Service Status
            metricToEnum.Add(53, OLEAutomationStatus.Enabled.GetType());//SQL Server Agent XPs Disabled
            metricToEnum.Add(54, WMIServiceUnavailability.Available.GetType());//WMI Service Unavailable
            metricToEnum.Add(67, SQLServerAgentLogStatus.Critical.GetType());//SQL Server Agent Log
            metricToEnum.Add(72, Idera.SQLdm.Common.Snapshots.MirroringMetrics.MirroringStateEnum.Disconnected.GetType());//Mirroring Status
            metricToEnum.Add(73, ConfigurationStatus.Changed.GetType());//Mirroring Preferred Configuration
            metricToEnum.Add(74, ConfigurationStatus.Changed.GetType());//Mirrored Server Role Changed
            metricToEnum.Add(75, Idera.SQLdm.Common.Snapshots.ConnectedState.Connected.GetType());//Mirroring Witness Connection
            metricToEnum.Add(77, SQLServerAgentJobStatus.failed.GetType());//Cluster Failover
            metricToEnum.Add(78, ClusterActiveNodePreference.Preferred.GetType());//Cluster Active Node
            metricToEnum.Add(80, DeadlockOccurance.Occured.GetType());//Deadlock
            metricToEnum.Add(96, ConfigurationStatus.Changed.GetType());//VM Resource Configuration Change
            metricToEnum.Add(97, ConfigurationStatus.Changed.GetType());//VM Host Server Changed
            metricToEnum.Add(104, VMMemorySwapDelayDetection.Detected.GetType());//VM Memory Swap Delay Detected
            metricToEnum.Add(105, VMMemorySwapDelayDetection.Detected.GetType());//Host Memory Swap Detected
            metricToEnum.Add(106, VMMemorySwapDelayDetection.Detected.GetType());//VM Resource Limits Detected
            metricToEnum.Add(107, VMPowerState.PoweredOff.GetType());//VM Power State
            metricToEnum.Add(108, HostPowerState.PoweredOff.GetType());//Host Power State
            metricToEnum.Add(116, ConfigurationStatus.Changed.GetType());//Availability Group Role Change
            metricToEnum.Add(118, Idera.SQLdm.Common.Snapshots.AlwaysOnSynchronizationHealth.None.GetType()); ///Mirroring Witness Connection

            metricToEnum.Add(88, JobStepCompletionStatus.Unknown.GetType()); //SQL Server Agent Job Completion
            metricToEnum.Add(94, AutogrowStatus.AutogrowOff.GetType()); //Log File Autogrow
            metricToEnum.Add(95, AutogrowStatus.AutogrowOff.GetType()); //Data File Autogrow

        }
        [Serializable]
        public enum SQLServerAgentLogStatus
        {
            Critical = 1,
            Warning = 2,
            Ok = 3
        }

        [Serializable]
        public enum HostPowerState
        {
            PoweredOff = 0,
            PoweredOn = 1,
            Standby = 2,
            Unknown = 3
        }


        [Serializable]
        public enum VMPowerState
        {
            PoweredOff = 0,
            PoweredOn = 1,
            Suspended = 2
        }

        [Serializable]
        public enum VMMemorySwapDelayDetection
        {
            NotDetected = 0,
            Detected = 1
        }

        [Serializable]
        public enum DeadlockOccurance
        {
            NotOccured = 0,
            Occured = 1
        }
        [Serializable]
        public enum ClusterActiveNodePreference
        {
            NotPreferred = 0,
            Preferred = 1
        }

        [Serializable]
        public enum ConfigurationStatus
        {
            Unchanged = 0,
            Changed = 1
        }


        [Serializable]
        public enum WMIServiceUnavailability
        {
            Available = 0,
            Unavailable = 1
        }


        [Serializable]
        public enum SQLServerAgentJobStatus
        {
            failed = 0,
            NotFailed = 1
        }
        [Serializable]
        public enum MaintenanceModeStatus
        {
            Disabled = 0,
            Enabled = 1
        }

        [Serializable]
        public enum OLEAutomationStatus
        {
            Enabled = 0,
            Disabled = 1
        }

        [Serializable]
        public enum SQLdmServiceStatus
        {
            Stopped = 0,
            Running = 1
        }

        public static string DescriptionAttr<T>(this T source)
        {
            if (source != null)
            {
                FieldInfo fi = source.GetType().GetField(source.ToString());

                if (fi != null)
                {
                    DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                        typeof(DescriptionAttribute), false);

                    if (attributes != null && attributes.Length > 0)
                    {
                        return attributes[0].Description;
                    }
                }
                return source.ToString();
            }
            return null;
        }

        internal static string GetDescriptionOrValueForMetric(decimal? value, int metric)
        {
            Type metricEnumType = null;
            string valueStr = null;
            if (value != null)
            {
                metricToEnum.TryGetValue(metric, out metricEnumType);
                if (metricEnumType != null)
                {
                    //object o = Enum.ToObject(metricEnumType, value);
                    valueStr = DescriptionAttr(Enum.ToObject(metricEnumType, (int)value));
                }

                if (valueStr == null)
                {
                    valueStr = value.ToString();
                }
            }
            return valueStr;
        }

        internal static string GetDescriptionOrValueForMetric(string value, int metric, string defaultValue = null)
        {
            Type metricEnumType = null;
            string valueStr = null;
            if (value != null)
            {
                metricToEnum.TryGetValue(metric, out metricEnumType);
                if (metricEnumType != null)
                {
                    //object o = Enum.ToObject(metricEnumType, value);
                    valueStr = DescriptionAttr(Enum.Parse(metricEnumType, value));
                }

                if (valueStr == null)
                {
                    valueStr = value.ToString();
                }
            }
            else
            {
                valueStr = defaultValue;
            }
            return valueStr;
        }

        public static IList<DC.Alert> GetAlertsForWebConsole(SqlConnectionInfo connectionInfo, string timeZoneOffset,
            DateTime? startDate, DateTime? endDate, string instanceId, string severity,
            string metric, string category, string orderBy, string orderType, int limit, bool activeOnly, bool abridged = false, UserToken userToken = null, int instanceLogicalOperator = 1, int metricLogicalOperator = 1, int severityLogicalOperator = 1)
        {

            MetricDefinitions md = null;

            md = new MetricDefinitions(false, false, true);
            md.Load(connectionInfo.ConnectionString);

            using (
               SqlConnection connection =
                   connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetAlertsForWebConsoleStoredProcedure,
                    startDate, endDate, instanceId, severity, metric, category, orderBy, orderType, limit, activeOnly, instanceLogicalOperator, metricLogicalOperator, severityLogicalOperator))
                {
                    IList<DC.Alert> alerts = new List<DC.Alert>();

                    while (reader.Read())
                    {

                        var ordinal = -1;
                        DC.Alert alert = new DC.Alert();

                        int metricId = reader.GetInt32(reader.GetOrdinal("Metric"));
                        MetricDefinition mdfn = md.GetMetricDefinition(metricId);
                        if (!abridged)
                        {
                            alert.Metric = ConvertToDataContract.ToDC(mdfn, ref metricToEnum);
                            alert.StateEvent = reader.GetByte(reader.GetOrdinal("StateEvent"));
                            alert.Message = reader.GetString(reader.GetOrdinal("Message"));
                        }
                        else
                        {
                            alert.Metric = new DC.Metric();
                            alert.Metric.MetricCategory = mdfn.MetricCategory.ToString("F");
                        }

                        bool add = true;
                        if (category != null)
                        {
                            add = category.Equals(alert.Metric.MetricCategory);
                        }
                        if (add)
                        {
                            alert.AlertId = reader.GetInt64(reader.GetOrdinal("AlertID"));
                            //START- SQLdm 10.0 (Sanjali Makkar) - To modify the DateTime fields of response
                            alert.UTCOccurrenceDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCOccurrenceDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time
                            //END- SQLdm 10.0 (Sanjali Makkar) - To modify the DateTime fields of response

                            ordinal = reader.GetOrdinal("InstanceId");
                            if (!reader.IsDBNull(ordinal))
                            {
                                alert.SQLServerId = reader.GetInt32(ordinal);
                            }
                            alert.ServerName = reader.GetString(reader.GetOrdinal("ServerName"));

                            ordinal = reader.GetOrdinal("DatabaseName");
                            alert.DatabaseName = reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);

                            ordinal = reader.GetOrdinal("Active");
                            alert.IsActive = reader[ordinal].GetType() == typeof(Boolean) ? Convert.ToInt32(reader.GetBoolean(ordinal)) : reader.GetInt32(ordinal);
                            alert.Severity = reader.GetByte(reader.GetOrdinal("Severity"));

                            ordinal = reader.GetOrdinal("PreviousAlertSeverity");
                            if (!reader.IsDBNull(ordinal))
                            {
                                alert.PreviousAlertSeverity = reader.GetByte(ordinal);
                            }
                            else
                                alert.PreviousAlertSeverity = 1;
                            if (reader["Value"] != DBNull.Value)
                            {
                                alert.Value = (double?)reader["Value"];
                                alert.StringValue = GetDescriptionOrValueForMetric((decimal?)alert.Value, metricId);
                            }

                            alert.Heading = reader.GetString(reader.GetOrdinal("Heading"));

                            if (ServerAuthorizationHelper.IsServerAuthorized(alert.SQLServerId, userToken))
                            {
                                alerts.Add(alert);
                            }
                        }
                    }

                    return alerts;
                }
            }
        }


        public static DC.Alert GetAlertDetails(SqlConnectionInfo connectionInfo, int alertId, string timeZoneOffset, UserToken userToken)
        {
            MetricDefinitions md = new MetricDefinitions(false, false, true);
            md.Load(connectionInfo.ConnectionString);

            using (
                          SqlConnection connection =
                              connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetAlertStoredProcedure,
                    alertId))
                {
                    if (reader.Read())
                    {
                        DC.Alert alert = new DC.Alert();
                        var ordinal = reader.GetOrdinal("SQLServerID");
                        if (!reader.IsDBNull(ordinal))
                        {
                            alert.SQLServerId = reader.GetInt32(ordinal);
                        }
                        //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                        if (alert.SQLServerId != null && !ServerAuthorizationHelper.IsServerAuthorized(alert.SQLServerId, userToken))
                        {
                            throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
                        }
                        //END
                        int metricId = reader.GetInt32(reader.GetOrdinal("Metric"));

                        alert.Metric = ConvertToDataContract.ToDC(md.GetMetricDefinition(metricId), ref metricToEnum);

                        if (alert.SQLServerId != null)
                        {
                            string[] thresholds = getThresholdsForMetricForInstance(connectionInfo, metricId, alert.SQLServerId.Value);
                            //alert.Metric.WarningThreshold = GetDescriptionOrValueForMetric(thresholds[1], metricId);
                            //alert.Metric.CriticalThreshold = GetDescriptionOrValueForMetric(thresholds[2], metricId);

                            alert.Metric.WarningThreshold = thresholds[1];
                            alert.Metric.CriticalThreshold = thresholds[2];

                        }

                        alert.AlertId = reader.GetInt64(reader.GetOrdinal("AlertID"));
                        //START- SQLdm 10.0 (Sanjali Makkar) - To modify the DateTime fields of response
                        alert.UTCOccurrenceDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCOccurrenceDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time
                        //END- SQLdm 10.0 (Sanjali Makkar) - To modify the DateTime fields of response
                        alert.ServerName = reader.GetString(reader.GetOrdinal("ServerName"));

                        ordinal = reader.GetOrdinal("Active");
                        alert.IsActive = reader["Active"].GetType() == typeof(Boolean) ? Convert.ToInt32(reader.GetBoolean(ordinal)) : reader.GetInt32(ordinal);
                        alert.Severity = reader.GetByte(reader.GetOrdinal("Severity"));
                        alert.StateEvent = reader.GetByte(reader.GetOrdinal("StateEvent"));
                        if (reader["Value"] != DBNull.Value)
                        {
                            alert.Value = (double?)reader["Value"];
                            alert.StringValue = GetDescriptionOrValueForMetric((decimal?)alert.Value, metricId);
                        }

                        alert.Heading = reader.GetString(reader.GetOrdinal("Heading"));
                        alert.Message = reader.GetString(reader.GetOrdinal("Message"));

                        return alert;
                    }

                    return null;
                }
            }
        }

        public static IList<DC.TimedValue> GetMetricsHistoryForAlert(SqlConnectionInfo connectionInfo, int alertId, string timeZoneOffset, int numHistoryHours, UserToken userToken)
        {
            IList<DC.TimedValue> metricsHistory = new List<DC.TimedValue>();

            using (SqlConnection connection =
                              connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                try
                {
                    //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                    SqlDataReader sqlReader = SqlHelper.ExecuteReader(connection, CommandType.Text, "Select SQLServerID FROM [Alerts]   INNER JOIN MonitoredSQLServers  ON ServerName = InstanceName WHERE AlertID = " + alertId);

                    while (sqlReader.Read())
                    {
                        DC.Alert alert = new DC.Alert();
                        var sqlId = sqlReader.GetOrdinal("SQLServerID");
                        if (!sqlReader.IsDBNull(sqlId))
                        {
                            alert.SQLServerId = sqlReader.GetInt32(sqlId);
                        }

                        if (alert.SQLServerId != null && !ServerAuthorizationHelper.IsServerAuthorized(alert.SQLServerId, userToken))
                        {
                            throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
                        }

                    }
                    // END
                    if (connection.State != ConnectionState.Closed) connection.Close();

                    connection.Open();

                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetMetricsHistoryForAlertStoredProcedure, alertId, numHistoryHours))
                    {

                        while (reader.Read())
                        {
                            DC.TimedValue tv = new DC.TimedValue();
                            //START- SQLdm 10.0 (Sanjali Makkar) - To modify the DateTime fields of response
                            tv.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time
                            //END- SQLdm 10.0 (Sanjali Makkar) - To modify the DateTime fields of response
                            int ordinal = reader.GetOrdinal("Value");
                            if (!(reader.GetValue(ordinal) == DBNull.Value))
                            {
                                tv.Value = Convert.ToDouble(reader.GetValue(ordinal));
                            }

                            metricsHistory.Add(tv);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (connection != null && connection.State != ConnectionState.Closed)
                        connection.Close();
                    throw e;
                }
                return metricsHistory;
            }
        }

        /*public static IList<ResponseTimeForInstance> GetLatestResponseTimesByInstance(SqlConnectionInfo connectionInfo)
        {
            IList<ResponseTimeForInstance> worstResponseTimes = new List<ResponseTimeForInstance>();

            using ( SqlConnection connection =
                              connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetLatestResponseTimesByInstanceProcedure))
                {
                    while (reader.Read())
                    {
                        ResponseTimeForInstance rtfi = new ResponseTimeForInstance();

                        rtfi.InstanceId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);

                        var instanceNameOrdinal = reader.GetOrdinal("InstanceName");
                        rtfi.InstanceName = reader.IsDBNull(instanceNameOrdinal) ? "" : reader.GetString(instanceNameOrdinal);
                        rtfi.ResponseTimeMillis = (int)GetValueOrDefault<int>(reader["ResponseTimeInMilliseconds"]);
                        rtfi.UTCCollectionDateTime = reader.GetDateTime(reader.GetOrdinal("UTCCollectionDateTime")); ;

                        worstResponseTimes.Add(rtfi);
                    }
                }
            }
            return worstResponseTimes;
        }*/
        #endregion

        #region Metrics
        public static IList<DC.AlertsByCategory> GetAlertsCountByCategory(SqlConnectionInfo connectionInfo, UserToken userToken, bool PerInstance = false)
        {
            IList<DC.AlertsByCategory> alertsByCategory = new List<DC.AlertsByCategory>();
            try
            {
                //[START] SQLdm 10.0 (Gaurav Karwal): aggregating the sqlserverids to send to the procedure
                string allowedServerIdCommaSeparated = userToken.AssignedServers.Select(a => a.Server.SQLServerID.ToString()).Aggregate((a, b) => a + "," + b);
                //[END] SQLdm 10.0 (Gaurav Karwal): aggregating the sqlserverids to send to the procedure

                using (
                   SqlConnection connection =
                       connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetAlertsCountPerCategory, allowedServerIdCommaSeparated, PerInstance))
                    {

                        while (reader.Read())
                        {
                            DC.AlertsByCategory abc = new DC.AlertsByCategory();
                            abc.NumOfAlerts = (int)GetValueOrDefault<int>(reader["NumOfAlerts"]);

                            var categoryOrdinal = reader.GetOrdinal("Category");
                            abc.Category = reader.IsDBNull(categoryOrdinal) ? "" : reader.GetString(categoryOrdinal);

                            if (PerInstance)
                            {
                                var instanceIDOrdinal = reader.GetOrdinal("SQLServerID");

                                if (!reader.IsDBNull(instanceIDOrdinal))
                                {
                                    abc.InstanceID = reader.GetInt32(instanceIDOrdinal);

                                }
                                else
                                    abc.InstanceID = -1;  //SQLdm 9.0 (Abhishek Joshi) --Web Console Improvements -display instance id as -1, when there is no instance with alert count > 0
                            }

                            alertsByCategory.Add(abc);

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat("Error in Getting Alerts Count By Category:{0}", e.Message);
            }

            return alertsByCategory;


        }

        public static IList<DC.AlertsByDatabase> GetAlertsCountByDatabase(SqlConnectionInfo connectionInfo, UserToken userToken)
        {
            IList<DC.AlertsByDatabase> alertsByDatabase = new List<DC.AlertsByDatabase>();
            using (
               SqlConnection connection =
                   connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetAlertsCountPerDatabase))
                {

                    while (reader.Read())
                    {
                        DC.AlertsByDatabase abd = new DC.AlertsByDatabase();
                        abd.InstanceId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);

                        var instanceNameOrdinal = reader.GetOrdinal("ServerName");
                        abd.InstanceName = reader.IsDBNull(instanceNameOrdinal) ? "" : reader.GetString(instanceNameOrdinal);

                        var dbnameOrdinal = reader.GetOrdinal("DatabaseName");
                        abd.DatabaseName = reader.IsDBNull(dbnameOrdinal) ? "" : reader.GetString(dbnameOrdinal);

                        abd.NumOfAlerts = (int)GetValueOrDefault<int>(reader["NumOfAlerts"]);
                        alertsByDatabase.Add(abd);
                    }
                }
            }
            //SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            return alertsByDatabase.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.InstanceId)).ToList();
            //return alertsByDatabase;

        }

        public static IList<DC.Metric> GetMetrics(SqlConnectionInfo connectionInfo, string metricId, int maxRows)
        {
            //ToDo: To cache this later
            MetricDefinitions md = new MetricDefinitions(false, false, true);
            md.Load(connectionInfo.ConnectionString);

            using (
               SqlConnection connection =
                   connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                int rows = maxRows;
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetMetricInfoStoredProcedure, metricId))
                {
                    IList<DC.Metric> metrics = new List<DC.Metric>();

                    while (reader.Read())
                    {
                        DC.Metric metric = new DC.Metric();
                        metric.MetricId = (int)GetValueOrDefault<int>(reader["Metric"]);
                        metric.Rank = (int)GetValueOrDefault<int>(reader["Rank"]);
                        metric.MetricCategory = reader.GetString(reader.GetOrdinal("Category"));
                        metric.Name = reader.GetString(reader.GetOrdinal("Name"));
                        metric.Description = reader.GetString(reader.GetOrdinal("Description"));
                        // metric.Comments = reader.GetString(reader.GetOrdinal("Comments"));
                        metrics.Add(metric);

                        //START :SQLdm 10.0 :(srishti purohit) Fixing issue with MetricNumeric property 

                        Type output = null;
                        bool retValue = false;
                        if (metricToEnum != null && metricToEnum.TryGetValue(metric.MetricId, out output) == false) retValue = true;

                        //START SQLdm 10.0 : ignoring metrics that are not being collected
                        if (metric.MetricId == 20 || metric.MetricId == 21 || metric.MetricId == 70 || metric.MetricId == 65 || metric.MetricId == 34 || metric.MetricId == 32
                            //Defect DE45347 fixed -- excluding folloqing metric also
                            || metric.MetricId == 124 || metric.MetricId == 11 || metric.MetricId == 56 || metric.MetricId == 128 || metric.MetricId == 129 || metric.MetricId == 66 || metric.MetricId == 130 || metric.MetricId == 125
                            //10.0 SQLdm Srishti Purohit -- Blocking Session Tempdb Space Usage (MB) as per defect DE45846 as this metric does not have supported table and col to caculate on
                            || metric.MetricId == 92 || metric.MetricId == 4 || metric.MetricId == 5 || metric.MetricId == 17
                            )
                        {
                            retValue = false;
                        }
                        //END SQLdm 10.0 : 

                        metric.IsMetricNumeric = retValue;
                        //END SQLdm 10.0 :(srishti purohit) Fixing issue with MetricNumeric property 

                    }

                    return metrics;
                }
            }
        }

        //SQLdm 9.1 (Sanjali Makkar) (Baseline Statistics) - Adding Baseline Statistics For Metric

        public static IList<DC.Category.BaselineForMetric> GetBaselineForMetric(SqlConnectionInfo connectionInfo, int serverId, int metricId, string timeZoneOffset, int numHistoryMin, DateTime endDate, int limit)
        {
            if (connectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                try
                {
                    connection.Open();
                    DateTime utcCollectionEndTime;
                    if (!endDate.Equals(default(DateTime)))
                        utcCollectionEndTime = DateTimeHelper.ConvertToUTC(endDate, timeZoneOffset);
                    else
                        utcCollectionEndTime = DateTime.UtcNow;

                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetBaselineStatisticsStoredProcedure, serverId, metricId, numHistoryMin, utcCollectionEndTime))
                    {
                        IList<DC.Category.BaselineForMetric> result = new List<DC.Category.BaselineForMetric>();

                        DateTime previousBaselineStartTime = DateTime.Now;      //initializing the variable. datetime.now does not have any significance in the logic
                        Double previousBaselineValue = 0;

                        int i = 0;
                        while (reader.Read())
                        {
                            if (i == 0)
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal("UTCCalculation")))
                                {
                                    //Start Time (T1) for Time Bracket of (T1-T2)
                                    //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                                    previousBaselineStartTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind((Convert.ToDateTime(GetValueOrDefault<DateTime>(reader["UTCCalculation"]))), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time
                                }

                                if (!reader.IsDBNull(reader.GetOrdinal("Mean")) && !reader.IsDBNull(reader.GetOrdinal("StandardDeviation")))
                                {
                                    //Value of baseline will be (Mean + Standard Deviation at start time (T1)) for Time Bracket of (T1-T2) 
                                    previousBaselineValue = Convert.ToDouble(GetValueOrDefault<double>(reader["Mean"])) + Convert.ToDouble(GetValueOrDefault<double>(reader["StandardDeviation"]));
                                }
                            }
                            if (i > 0)
                            {
                                DC.Category.BaselineForMetric Baseline = new DC.Category.BaselineForMetric();
                                Baseline.StartTime = previousBaselineStartTime;
                                Baseline.Value = previousBaselineValue;
                                if (!reader.IsDBNull(reader.GetOrdinal("UTCCalculation")))
                                {
                                    //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                                    // End Time (T2) for the previous Time Bracket is assigned here
                                    Baseline.EndTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind((Convert.ToDateTime(GetValueOrDefault<DateTime>(reader["UTCCalculation"]))), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                                    result.Add(Baseline);

                                    //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                                    // Next bracket
                                    previousBaselineStartTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind((Convert.ToDateTime(GetValueOrDefault<DateTime>(reader["UTCCalculation"]))), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time
                                }
                                if (!reader.IsDBNull(reader.GetOrdinal("Mean")))
                                {
                                    //Value of baseline will be (Mean + Standard Deviation at start time (T1)) for Time Bracket of (T1-T2) 
                                    previousBaselineValue = Convert.ToDouble(GetValueOrDefault<double>(reader["Mean"])) + Convert.ToDouble(GetValueOrDefault<double>(reader["StandardDeviation"]));
                                }
                            }
                            i += 1;
                        }

                        // For the Last row
                        if (reader.HasRows == true)
                        {
                            DC.Category.BaselineForMetric baseLine_lastRow = new DC.Category.BaselineForMetric();
                            baseLine_lastRow.StartTime = previousBaselineStartTime;
                            baseLine_lastRow.Value = previousBaselineValue;
                            baseLine_lastRow.EndTime = null;
                            result.Add(baseLine_lastRow);
                        }

                        return result;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// SQLdm 9.1 (Sanjali Makkar) (Instance Status) : Added to get Instances related information
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <returns></returns>
        public static InstanceStatus GetInstanceStatus(SqlConnectionInfo connectionInfo, UserToken userToken)
        {
            if (connectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                try
                {
                    connection.Open();
                    //START SQLdm 10.0 (Swati Gogia) : Implemented for Instance Level Security
                    IList<ServerPermission> assignedServers = userToken.AssignedServers.ToList();

                    SqlParameter[] arrSqlParam = new SqlParameter[1];
                    XDocument xmlDoc = new XDocument();
                    XElement xElm = new XElement("Root",
                                 from l in assignedServers.Select(x => x.Server.SQLServerID)
                                 select new XElement("Source", new XElement("ID", l))
                        );
                    xmlDoc.Add(xElm);
                    arrSqlParam[0] = new SqlParameter("@SqlIdList", "N" + xmlDoc.ToString());
                    //END SQLdm 10.0 (Swati Gogia)
                    using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetInstanceStatusStoredProcedure, arrSqlParam))
                    {
                        InstanceStatus result = new InstanceStatus();
                        AlertInstanceStatus result_alertStatus = new AlertInstanceStatus(); //To populate the properties of Alert Status class object
                        result.AlertStatus = result_alertStatus;
                        InstanceOverview result_overview = new InstanceOverview(); //To populate the properties of Instance Overview class object
                        int currMonitoredServersCount = 0; //To store the count of Monitored Instances
                        IDictionary<int, DC.MonitoredSqlServer> serverStatusMap = new Dictionary<int, DC.MonitoredSqlServer>();
                        int stateOverviewOrdinal = dataReader.GetOrdinal("StateOverview");
                        int serverIdOrdinal = dataReader.GetOrdinal("InstanceID");
                        int countServerWithNoData = 0;
                        int serverCount = 0;
                        while (dataReader.Read())
                        {
                            DC.MonitoredSqlServer status = new DC.MonitoredSqlServer();

                            if (dataReader.HasRows)
                            {
                                //confirming that both the instanceid and server overview columns have data
                                if (!dataReader.IsDBNull(serverIdOrdinal) && ((int)dataReader[serverIdOrdinal]) != 0 && stateOverviewOrdinal > -1 && !dataReader.IsDBNull(stateOverviewOrdinal))
                                {

                                    ////START SQLdm 10.0 (Swati Gogia): Added the if condition to implement Instance level security
                                    //if (ServerAuthorizationHelper.IsServerAuthorized(dataReader.GetInt32(serverIdOrdinal),userToken))
                                    //{
                                    status = ConstructMonitoredSqlServerStatus(dataReader.GetString(stateOverviewOrdinal));
                                    serverStatusMap.Add(status.SQLServerId, status);
                                    //}
                                    //End
                                }
                                else
                                {
                                    //handling the case when sqldm is not able to connect to a sql server, thus does not have data in ServerActivity or ServerStatistics table
                                    countServerWithNoData++;
                                }

                            }

                        }

                        result_alertStatus.CriticalInstancesCount = serverStatusMap.Count(a => a.Value.MaxSeverity == Convert.ToInt32((byte)Enum.Parse(typeof(MonitoredStateFlags), MonitoredStateFlags.Critical.ToString()))) + countServerWithNoData;//assuming here that servers with no data are critical servers;
                        result_alertStatus.WarningInstancesCount = serverStatusMap.Count(a => a.Value.MaxSeverity == Convert.ToInt32((byte)Enum.Parse(typeof(MonitoredStateFlags), MonitoredStateFlags.Warning.ToString())));
                        result_alertStatus.InformationalInstancesCount = serverStatusMap.Count(a => a.Value.MaxSeverity == Convert.ToInt32((byte)Enum.Parse(typeof(MonitoredStateFlags), MonitoredStateFlags.Informational.ToString())));
                        result_alertStatus.OkInstancesCount = serverStatusMap.Count(a => a.Value.MaxSeverity == Convert.ToInt32((byte)Enum.Parse(typeof(MonitoredStateFlags), MonitoredStateFlags.OK.ToString())));


                        dataReader.NextResult(); //To get data from next result set of stored procedure

                        while (dataReader.Read())
                        {
                            int activeOrdinal = dataReader.GetOrdinal("Active"), serverCountOrdinal = dataReader.GetOrdinal("ServerCount");
                            if (dataReader.IsDBNull(activeOrdinal) == false)
                            {
                                serverCount = dataReader.IsDBNull(serverCountOrdinal) == false ? Convert.ToInt32(dataReader[serverCountOrdinal]) : 0;

                                if (Convert.ToInt16(dataReader[activeOrdinal]) == 0) //For Disabled Servers, Active flag should be 0
                                {
                                    result_overview.DisabledServersCount += serverCount;
                                }
                                else //SQLDM-29212. Filter non-Active Instances.
                                {
                                    currMonitoredServersCount += serverCount;
                                }
                            }
                        }
                        result_overview.MonitoredServersCount = currMonitoredServersCount;
                        result.Overview = result_overview;
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
        }

        #endregion

        //public static ManagementServiceConfiguration GetDefaultManagementService(SqlConnectionInfo connectionInfo)
        //{
        //    if (connectionInfo == null)
        //    {
        //        throw new ArgumentNullException("connectionInfo");
        //    }

        //    using (
        //        SqlConnection connection =
        //            connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
        //    {
        //        using (
        //            SqlDataReader dataReader =
        //                SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure,
        //                                        GetDefaulManagementServiceStoredProcedure))
        //        {
        //            if (!dataReader.HasRows)
        //            {
        //                return null;
        //            }

        //            dataReader.Read();

        //            string identifier = GetValue<string>(dataReader["ManagementServiceID"]) as string;
        //            string machineName = GetValue<string>(dataReader["MachineName"]) as string;
        //            string instanceName = GetValue<string>(dataReader["InstanceName"]) as string;
        //            string address = GetValue<string>(dataReader["Address"]) as string;
        //            int port = (int)GetValue<int>(dataReader["Port"]);

        //            return new ManagementServiceConfiguration(identifier, machineName, instanceName, address, port);
        //        }
        //    }
        //}


        public static IList<DC.ServerSummaryContainer> GetAllInstancesSummary(SqlConnectionInfo connectionInfo, UserToken userToken)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            IList<DC.ServerSummaryContainer> results = new List<DC.ServerSummaryContainer>();

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                // using (
                //   SqlDataReader dataReader =
                //       SqlHelper.ExecuteReader(connection, GetAllServersSummaryStoredProcedure,arrSqlParam))
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetAllServersSummaryStoredProcedure))
                {

                    Repository.RepositoryHelper.MaxHealthIndex = MAX_HEALTH_INDEX_DEFAULT; //resetting the maxhealth index value so that the previously calculated values do not mess with the next set

                    IDictionary<int, DC.MonitoredSqlServer> serverStatusMap = new Dictionary<int, DC.MonitoredSqlServer>();
                    int stateOverviewOrdinal = dataReader.GetOrdinal("StateOverview");

                    // START SQLdm 9.1 (Sanjali Makkar): Gets Health Index Coefficients
                    int criticalAlertCoefficientOrdinal = dataReader.GetOrdinal("HealthIndexCoefficientForCriticalAlert");
                    int warningAlertCoefficientOrdinal = dataReader.GetOrdinal("HealthIndexCoefficientForWarningAlert");
                    int infoAlertCoefficientOrdinal = dataReader.GetOrdinal("HealthIndexCoefficientForInformationalAlert");
                    //Configurable Health Index Calculation
                    //SQLdm10.1 Srishti Purohit
                    int instanceScaleFactorOrdinal = dataReader.GetOrdinal("InstanceScaleFactor");
                    int averageTagsScaleFactorOrdinal = dataReader.GetOrdinal("AvgTagsScaleFactor");
                    int FriendlyServerNameOrdinal = dataReader.GetOrdinal("FriendlyServerName");
                    while (dataReader.Read())
                    {
                        DC.MonitoredSqlServer status = new DC.MonitoredSqlServer();
                        if (dataReader.HasRows)
                        {
                            if (stateOverviewOrdinal > -1 && dataReader.IsDBNull(stateOverviewOrdinal) == false)
                            {
                                status = ConstructMonitoredSqlServerStatus(dataReader.GetString(stateOverviewOrdinal));




                                if (criticalAlertCoefficientOrdinal > -1 && !dataReader.IsDBNull(criticalAlertCoefficientOrdinal))
                                    status.HealthIndexCoefficientForCriticalAlert = Convert.ToDouble(dataReader[criticalAlertCoefficientOrdinal]);

                                if (warningAlertCoefficientOrdinal > -1 && !dataReader.IsDBNull(warningAlertCoefficientOrdinal))
                                    status.HealthIndexCoefficientForWarningAlert = Convert.ToDouble(dataReader[warningAlertCoefficientOrdinal]);

                                if (infoAlertCoefficientOrdinal > -1 && !dataReader.IsDBNull(infoAlertCoefficientOrdinal))
                                    status.HealthIndexCoefficientForInformationalAlert = Convert.ToDouble(dataReader[infoAlertCoefficientOrdinal]);

                                // SQLdm 10.1 (Srishti Purohit) -- Health coefficients for instance
                                if (instanceScaleFactorOrdinal > -1)
                                {
                                    if (!dataReader.IsDBNull(instanceScaleFactorOrdinal))
                                        status.HealthIndexInstanceScaleFactor = Convert.ToDouble(dataReader[instanceScaleFactorOrdinal]);
                                    else
                                        status.HealthIndexInstanceScaleFactor = null;
                                }
                                if (averageTagsScaleFactorOrdinal > -1)
                                {
                                    if (!dataReader.IsDBNull(averageTagsScaleFactorOrdinal))
                                        status.HealthIndexAvgTagsScaleFactor = Convert.ToDouble(dataReader[averageTagsScaleFactorOrdinal]);
                                    else
                                        status.HealthIndexAvgTagsScaleFactor = null;
                                }

                                if (status.BaseHealthIndex > Repository.RepositoryHelper.MaxHealthIndex)
                                    Repository.RepositoryHelper.MaxHealthIndex = status.BaseHealthIndex; //HealthIndex of every instance will be calculated wrt the Maximum value of HealthIndex

                                if (!dataReader.IsDBNull(FriendlyServerNameOrdinal))
                                    status.FriendlyServerName = Convert.ToString(dataReader[FriendlyServerNameOrdinal]);// Sqldm 10.1 (Pulkit Puri): Adding Friendly server name


                                // END SQLdm 9.1 (Sanjali Makkar): Gets Health Index Coefficients

                                serverStatusMap.Add(status.SQLServerId, status);
                            }
                        }
                    }
                    //container.ServerStatus = status;
                    dataReader.NextResult();

                    FriendlyServerNameOrdinal = dataReader.GetOrdinal("FriendlyServerName");


                    while (dataReader.Read())
                    {
                        int instanceId = (int)dataReader["SQLServerID"];
                        DC.MonitoredSqlServer serverStatus = null;
                        serverStatusMap.TryGetValue(instanceId, out serverStatus);
                        var ssc = new DC.ServerSummaryContainer();
                        ssc.ServerStatus = serverStatus;
                        ssc.AlertCategoryWiseMaxSeverity = new DC.AlertWiseMaxSeverity(); //SQLdm 10.2 (Anshika Sharma)
                        ssc.Overview = ConvertToDataContract.ToDCAbridged(dataReader);
                        if (ssc.ServerStatus != null && ssc.Overview.maintenanceModeEnabled)
                        {
                            ssc.ServerStatus.MaxSeverity = 2; //2 stands for Maintenance Mode.
                        }

                        if (ssc.ServerStatus != null && !ssc.Overview.maintenanceModeEnabled && ssc.ServerStatus.MaxSeverity == 2)
                        {
                            ssc.ServerStatus.MaxSeverity = 1; //1 stands for OK
                        }

                        if (ssc.ServerStatus == null)
                        {
                            ssc.ServerStatus = new DC.MonitoredSqlServer();
                            ssc.ServerStatus.MaxSeverity = 8; //8 stands for Critical
                        }

                        ssc.Overview.SQLServerId = instanceId;

                        if (FriendlyServerNameOrdinal > -1 && !dataReader.IsDBNull(FriendlyServerNameOrdinal))
                            ssc.Overview.FriendlyServerName = Convert.ToString(dataReader[FriendlyServerNameOrdinal]);// Sqldm 10.1 (Pulkit Puri): Adding Friendly;

                        //START: SQLDM-30243--Update Server Overview Details..
                        if (dataReader["RunningSince"] != DBNull.Value)
                        {
                            ssc.Overview.runningSince = Convert.ToDateTime(dataReader["RunningSince"]);
                        }
                        if (dataReader["IsClustered"] != DBNull.Value)
                        {
                            ssc.Overview.isClustered = Convert.ToBoolean(dataReader["IsClustered"]);
                        }
                        if (dataReader["ProcessorCount"] != DBNull.Value)
                        {
                            ssc.Overview.processorCount = (int)dataReader["ProcessorCount"];
                        }
                        if (dataReader["ServerHostName"] != DBNull.Value)
                        {
                            ssc.Overview.serverHostName = Convert.ToString(dataReader["ServerHostName"]);
                        }
                        if (dataReader["OSTotalPhysicalMemoryInKilobytes"] != DBNull.Value)
                        {
							if(ssc.Overview.osMetricsStatistics != null)
							{
                            	ssc.Overview.osMetricsStatistics.TotalPhysicalMemory = Convert.ToDecimal(dataReader["OSTotalPhysicalMemoryInKilobytes"]);
							}
                        }
                        if (dataReader["WindowsVersion"] != DBNull.Value)
                        {
                            ssc.Overview.windowsVersion = Convert.ToString(dataReader["WindowsVersion"]);
                        }
                        if (dataReader["DataFileSpaceUsedInKilobytes"] != DBNull.Value)
                        {
							if(ssc.Overview.databaseSummary != null)
							{
                            	ssc.Overview.databaseSummary.dataFileSpaceUsed = Convert.ToDecimal(dataReader["DataFileSpaceUsedInKilobytes"]);
                        	}
						}
                        if (dataReader["LogFileSpaceUsedInKilobytes"] != DBNull.Value)
                        {
							if(ssc.Overview.databaseSummary != null)
							{
                            	ssc.Overview.databaseSummary.logFileSpaceUsed = Convert.ToDecimal(dataReader["LogFileSpaceUsedInKilobytes"]);
							}
                        }
                        //END.



                        results.Add(ssc);

                    }

                    //SQLdm 10.2 (Anshika Sharma) : Added to Show CategoryWaiseMaxAlerts
                    dataReader.NextResult();
                    List<AlertWiseSeverity> alertWiseSeverity = new List<AlertWiseSeverity>();
                    while (dataReader.Read())
                    {
                        int ServerID = Convert.ToInt32(dataReader["SQLServerID"]);
                        int metric = Convert.ToInt32(dataReader["Metric"]);
                        int severity = Convert.ToInt32(dataReader["MaxSeverity"]);
                        AlertWiseSeverity AlertWiseSeverityObj = alertWiseSeverity.Where(alertWiseSeverityObj => (alertWiseSeverityObj.ServerID == ServerID)).FirstOrDefault();
                        if (AlertWiseSeverityObj == null)
                        {
                            AlertWiseSeverity newAlertWiseSeverity = new AlertWiseSeverity();
                            newAlertWiseSeverity.ServerID = ServerID;
                            newAlertWiseSeverity.SetSeverity(metric, severity);
                            alertWiseSeverity.Add(newAlertWiseSeverity);
                        }
                        else
                        {
                            AlertWiseSeverityObj.SetSeverity(metric, severity);

                        }
                    }
                    //SQLdm 10.2 (Anshika Sharma) : Added to Show CategoryWaiseMaxAlerts
                    results.ToList().ForEach(result =>
                    {
                        var resultObj = alertWiseSeverity.Find(alertWiseSeverityObj => (alertWiseSeverityObj.ServerID == result.Overview.SQLServerId));
                        if (resultObj != null)
                        {
                            result.AlertCategoryWiseMaxSeverity.Cpu = resultObj.AlertSeverity["CPU"];
                            result.AlertCategoryWiseMaxSeverity.Memory = resultObj.AlertSeverity["MEMORY"];
                            result.AlertCategoryWiseMaxSeverity.Databases = resultObj.AlertSeverity["DATABASES"];
                            result.AlertCategoryWiseMaxSeverity.IO = resultObj.AlertSeverity["IO"];
                            result.AlertCategoryWiseMaxSeverity.Logs = resultObj.AlertSeverity["LOGS"];
                            result.AlertCategoryWiseMaxSeverity.Queries = resultObj.AlertSeverity["QUERIES"];
                            result.AlertCategoryWiseMaxSeverity.Services = resultObj.AlertSeverity["SERVICES"];
                            result.AlertCategoryWiseMaxSeverity.Sessions = resultObj.AlertSeverity["SESSIONS"];
                            result.AlertCategoryWiseMaxSeverity.Virtualization = resultObj.AlertSeverity["VIRTUALIZATION"];
                            result.AlertCategoryWiseMaxSeverity.Operational = resultObj.AlertSeverity["OPERATIONAL"];
                        }
                    });
                    //SQLdm 10.2.2--Fix for issue SQLDM-28538
                    //Getting the real time status of each sql server instance monitored by SQLdm, 
                    //and updating the List<ServerSumaryContainer> which will be returned back to webUI.
                    int SQLServerID;
                    int serverStatusSeverity;
                    Dictionary<int, int> serverStatuses = new Dictionary<int, int>();

                    dataReader.NextResult();
                    while (dataReader.Read())
                    {
                        SQLServerID = Convert.ToInt32(dataReader["SQLServerID"]);
                        serverStatusSeverity = Convert.ToInt16(dataReader["Status"]);

                        serverStatuses.Add(SQLServerID, serverStatusSeverity);
                    }

                    foreach (DC.ServerSummaryContainer ssc in results)
                    {
                        if (serverStatuses.ContainsKey(ssc.ServerStatus.SQLServerId))
                        {
                            //SQLDM-28317: Asking if the instance is on maintenance mode and supporting for informational alerts
                            if (ssc.Overview.maintenanceModeEnabled)
                            {
                                ssc.ServerStatus.MaxSeverity = 2;
                                continue;
                            }
                            if (!ssc.Overview.maintenanceModeEnabled && serverStatuses[ssc.ServerStatus.SQLServerId] == 2)
                            {
                                ssc.ServerStatus.MaxSeverity = 1;
                                continue;
                            }
                            ssc.ServerStatus.MaxSeverity = serverStatuses[ssc.ServerStatus.SQLServerId];
                        }
                    }
                    //SQLdm 10.2.2--Fix for issue SQLDM-28538
                }



                // SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                return results.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.Overview.SQLServerId)).ToList();

                // return results;
            }
        }


        #region GetServerSummary

        public static DC.ServerSummaryContainer GetServerSummary(SqlConnectionInfo connectionInfo, int monitoredServerId, string timeZoneOffset, DateTime? snapshotDateTime, int? historyInMinutes)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetAllServersSummaryStoredProcedure, monitoredServerId,
                        snapshotDateTime.HasValue ? snapshotDateTime.Value.ToUniversalTime().ToString() : null, historyInMinutes))
                {
                    DC.ServerSummaryContainer container = new DC.ServerSummaryContainer();
                    Repository.RepositoryHelper.MaxHealthIndex = MAX_HEALTH_INDEX_DEFAULT; //resetting the max health index so that the previous value does not get carried over
                    dataReader.Read();
                    int stateOverviewOrdinal = dataReader.GetOrdinal("StateOverview");
                    // START SQLdm 9.1 (Sanjali Makkar): Gets Health Index Coefficients
                    int criticalAlertCoefficientOrdinal = dataReader.GetOrdinal("HealthIndexCoefficientForCriticalAlert");
                    int warningAlertCoefficientOrdinal = dataReader.GetOrdinal("HealthIndexCoefficientForWarningAlert");
                    int infoAlertCoefficientOrdinal = dataReader.GetOrdinal("HealthIndexCoefficientForInformationalAlert");
                    //Configurable Health Index Calculation
                    //SQLdm10.1 Srishti Purohit
                    int instanceScaleFactorOrdinal = dataReader.GetOrdinal("InstanceScaleFactor");
                    int averageTagsScaleFactorOrdinal = dataReader.GetOrdinal("AvgTagsScaleFactor");
                    int FriendlyServerNameOrdinal = dataReader.GetOrdinal("FriendlyServerName");
                    DC.MonitoredSqlServer status = null;
                    if (dataReader.HasRows)
                    {
                        if (stateOverviewOrdinal > -1 && !dataReader.IsDBNull(stateOverviewOrdinal))
                        {
                            status = ConstructMonitoredSqlServerStatus(dataReader.GetString(stateOverviewOrdinal));


                            if (criticalAlertCoefficientOrdinal > -1 && !dataReader.IsDBNull(criticalAlertCoefficientOrdinal))
                                status.HealthIndexCoefficientForCriticalAlert = Convert.ToDouble(dataReader[criticalAlertCoefficientOrdinal]);

                            if (warningAlertCoefficientOrdinal > -1 && !dataReader.IsDBNull(warningAlertCoefficientOrdinal))
                                status.HealthIndexCoefficientForWarningAlert = Convert.ToDouble(dataReader[warningAlertCoefficientOrdinal]);

                            if (infoAlertCoefficientOrdinal > -1 && !dataReader.IsDBNull(infoAlertCoefficientOrdinal))
                                status.HealthIndexCoefficientForInformationalAlert = Convert.ToDouble(dataReader[infoAlertCoefficientOrdinal]);

                            // SQLdm 10.1 (Srishti Purohit) -- Health coefficients for instance
                            if (instanceScaleFactorOrdinal > -1)
                            {
                                if (!dataReader.IsDBNull(instanceScaleFactorOrdinal))
                                    status.HealthIndexInstanceScaleFactor = Convert.ToDouble(dataReader[instanceScaleFactorOrdinal]);
                                else
                                    status.HealthIndexInstanceScaleFactor = null;
                            }
                            if (averageTagsScaleFactorOrdinal > -1)
                            {
                                if (!dataReader.IsDBNull(averageTagsScaleFactorOrdinal))
                                    status.HealthIndexAvgTagsScaleFactor = Convert.ToDouble(dataReader[averageTagsScaleFactorOrdinal]);
                                else
                                    status.HealthIndexAvgTagsScaleFactor = null;
                            }

                            if (FriendlyServerNameOrdinal > -1 && !dataReader.IsDBNull(FriendlyServerNameOrdinal))
                                status.FriendlyServerName = Convert.ToString(dataReader[FriendlyServerNameOrdinal]);// Sqldm 10.1 (Pulkit Puri): Adding Friendly server name

                            if (status.BaseHealthIndex > Repository.RepositoryHelper.MaxHealthIndex)
                                Repository.RepositoryHelper.MaxHealthIndex = status.BaseHealthIndex; //HealthIndex of every instance will be calculated wrt the Maximum value of HealthIndex
                            //For a single instance, the Health Index will be calculated wrt its baseHealthIndex only
                        }
                        // END SQLdm 9.1 (Sanjali Makkar): Gets Health Index Coefficients
                    }
                    container.ServerStatus = status;
                    dataReader.NextResult();
                    var OverviewStatistics = GetTable(dataReader);



                    foreach (DataRow row in OverviewStatistics.Rows)
                    {
                        FriendlyServerNameOrdinal = dataReader.GetOrdinal("FriendlyServerName"); // Sqldm 10.1 (Pulkit Puri): Adding Friendly server name
                        ServerOverview overview = new ServerOverview((string)row["InstanceName"], row, new string[0], new DataRow[0]);
                        container.Overview = ConvertToDataContract.ToDC(overview, timeZoneOffset);

                        //START: SQLdm 9.0 (Abhishek Joshi) --Web Console Improvements -update the MaxSeverity
                        if (container.ServerStatus != null && container.Overview.maintenanceModeEnabled)
                        {
                            container.ServerStatus.MaxSeverity = 2; //2 stands for Maintenance Mode.
                        }
                        //END: SQLdm 9.0 (Abhishek Joshi) --Web Console Improvements -update the MaxSeverity

                        if (container.ServerStatus != null && !container.Overview.maintenanceModeEnabled && container.ServerStatus.MaxSeverity == 2)
                        {
                            container.ServerStatus.MaxSeverity = 1; //1 stands for OK
                        }

                        container.Overview.SQLServerId = monitoredServerId;

                        //SQLdm 10.1 (Pulkit Puri) --null check for row value instead of datareader value

                        if (FriendlyServerNameOrdinal > -1 && row[FriendlyServerNameOrdinal] != DBNull.Value)
                            container.Overview.FriendlyServerName = Convert.ToString(row[FriendlyServerNameOrdinal]); // Sqldm 10.1 (Pulkit Puri): Adding Friendly server name


                    }
                    return container;
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

        public static XmlDocument GetMonitoredSqlServerStatus(SqlConnectionInfo connectionInfo, int? instanceId)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
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
                    //SafeLoadRow(dataReader, itemArray, dataTable);
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

        public static IList<DC.Widgets.ResponseTimeForInstance> GetTopServerResponseTime(SqlConnectionInfo connectionInfo, string timeZoneOffset, int TopX, UserToken userToken)
        {
            if (connectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTopServersResponseTimeStoredProcedure, TopX))
                {

                    IList<DC.Widgets.ResponseTimeForInstance> result = new List<DC.Widgets.ResponseTimeForInstance>(TopX);

                    while (reader.Read())
                    {
                        DC.Widgets.ResponseTimeForInstance sqlserver = new DC.Widgets.ResponseTimeForInstance();
                        sqlserver.InstanceId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);
                        sqlserver.InstanceName = GetValueOrDefault<string>(reader["InstanceName"]).ToString();
                        sqlserver.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time
                        sqlserver.ResponseTimeMillis = (int)GetValueOrDefault<int>(reader["ResponseTimeInMilliseconds"]);
                        sqlserver.Severity = (int)GetValueOrDefault<int>(reader["Severity"]);
                        result.Add(sqlserver);
                    }
                    // SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                    return result.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.InstanceId)).ToList();
                    //return result;
                }
            }
        }


        public static DC.MonitoredSqlServer GetServerStatisticsHistory(SqlConnectionInfo connectionInfo, int SqlServerId, string timeZoneOffset, int NumHistoryMinutes, DateTime endDate)
        {
            if (connectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                DateTime utcCollectionEndTime;
                if (!endDate.Equals(default(DateTime)))
                    utcCollectionEndTime = DateTimeHelper.ConvertToUTC(endDate, timeZoneOffset);
                else
                    utcCollectionEndTime = DateTime.UtcNow;

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetServerOverviewHistoryStoredProcedure,
                     SqlServerId, utcCollectionEndTime, NumHistoryMinutes))
                {
                    //outer while commented by Gaurav Karwal as it did not make sense and was eating up one row
                    //while (reader.Read())
                    //{
                    IList<DC.MonitoredSqlServer.ServerStatisticsHistory> result = new List<DC.MonitoredSqlServer.ServerStatisticsHistory>();
                    DC.MonitoredSqlServer sqlserver = new DC.MonitoredSqlServer();
                    while (reader.Read())
                    {
                        DC.MonitoredSqlServer.ServerStatisticsHistory serverStats = new DC.MonitoredSqlServer.ServerStatisticsHistory();

                        if (sqlserver.SQLServerId == 0)
                        {
                            sqlserver.SQLServerId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);
                            sqlserver.InstanceName = GetValueOrDefault<string>(reader["InstanceName"]).ToString();
                            sqlserver.FriendlyServerName = GetValueOrDefault<string>(reader["FriendlyServerName"]).ToString();//SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
                        }

                        serverStats.CPUActivityPercentage = GetValueOrDefault<string>(reader["CPUActivityPercentage"]).ToString();
                        serverStats.IdlePercentage = GetValueOrDefault<string>(reader["IdleTimePercentage"]).ToString();
                        serverStats.ReponseTimeinMilliSeconds = GetValueOrDefault<string>(reader["ResponseTimeInMilliseconds"]).ToString();

                        //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                        serverStats.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                        serverStats.DiskTimePercent = GetValueOrDefault<string>(reader["DiskTimePercent"]).ToString();
                        serverStats.SqlMemoryUsedInKilobytes = GetValueOrDefault<string>(reader["SqlMemoryUsedInKilobytes"]).ToString();
                        serverStats.OSAvailableMemoryInKilobytes = GetValueOrDefault<string>(reader["OSAvailableMemoryInKilobytes"]).ToString();
                        serverStats.OSTotalPhysicalMemoryInKilobytes = GetValueOrDefault<string>(reader["OSTotalPhysicalMemoryInKilobytes"]).ToString();
                        serverStats.SqlMemoryAllocatedInKilobytes = GetValueOrDefault<string>(reader["SqlMemoryAllocatedInKilobytes"]).ToString();
                        result.Add(serverStats);
                    }
                    sqlserver.StatisticsHistory = result;
                    return sqlserver;
                    //}

                    //return null;
                }
            }
        }

        #endregion

        #region GetServerDataWithTags

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
                        servers.Add((int)GetValueOrDefault<int>(dataReader["SQLServerId"]), GetValueOrDefault<string>(dataReader["InstanceName"]) as string);
                    }

                    return servers;
                }
            }
        }

        public static ICollection<Tag> GetTags(SqlConnectionInfo connectionInfo)
        {
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
                        int tagId = (int)GetValueOrDefault<int>(dataReader["Id"]);
                        string tagName = GetValueOrDefault<string>(dataReader["Name"]) as string;
                        tags.Add(tagId, new Tag(tagId, tagName));
                    }

                    // Read in server tags
                    dataReader.NextResult();
                    while (dataReader.Read())
                    {
                        Tag tag;
                        int tagId = (int)GetValueOrDefault<int>(dataReader["TagId"]);

                        if (tags.TryGetValue(tagId, out tag))
                        {
                            tag.AddInstance((int)GetValueOrDefault<int>(dataReader["SQLServerId"]));
                        }
                    }

                    // Read in custom counter tags
                    dataReader.NextResult();
                    while (dataReader.Read())
                    {
                        Tag tag;
                        int tagId = (int)GetValueOrDefault<int>(dataReader["TagId"]);

                        if (tags.TryGetValue(tagId, out tag))
                        {
                            tag.AddCustomCounter((int)GetValueOrDefault<int>(dataReader["Metric"]));
                        }
                    }

                    // Read in permission tags
                    dataReader.NextResult();
                    while (dataReader.Read())
                    {
                        Tag tag;
                        int tagId = (int)GetValueOrDefault<int>(dataReader["TagId"]);

                        if (tags.TryGetValue(tagId, out tag))
                        {
                            tag.AddPermission((int)GetValueOrDefault<int>(dataReader["PermissionId"]));
                        }
                    }

                    return new List<Tag>(tags.Values);
                }
            }
        }

        #endregion

        #region "Database Details Methods"

        /// <summary>
        /// Gives the overview of various databases within a single instance
        /// Author: Gaurav Karwal
        /// Product Version: SQLdm 8.5
        /// </summary>
        /// <param name="connectionInfo">sql connection</param>
        /// <param name="instanceID">Id of the sql server containing the databases</param>
        /// <returns></returns>
        public static IList<DC.Database.MonitoredSqlServerDatabase> GetServerDatabasesOverview(SqlConnectionInfo connectionInfo, int instanceID, string timeZoneOffset)
        {
            if (connectionInfo == null || instanceID == 0) return null;
            IList<DC.Database.MonitoredSqlServerDatabase> _retValue = new List<DC.Database.MonitoredSqlServerDatabase>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                object[] parameters = { instanceID, 0 };
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetDatabaseDetailsByInstanceStoredProcedure, parameters))
                {
                    while (reader.Read())
                    {
                        DC.Database.MonitoredSqlServerDatabase currDatabase = new DC.Database.MonitoredSqlServerDatabase();
                        currDatabase.DatabaseName = (string)GetValueOrDefault<string>(reader["DatabaseName"]);
                        //currDatabase.TotalAlertCount = (int)GetValueOrDefault<int>(reader["TotalAlertsCount"]); 
                        //currDatabase.ActiveAlertCount = (int)GetValueOrDefault<int>(reader["ActiveAlertsCount"]);

                        //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                        currDatabase.CreationDateTimeUtc = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["CreationDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                        //currDatabase.CreationDateTimeUtc = (DateTime)GetValueOrDefault<DateTime>(reader["LatestSizeCollectionTime"]);  

                        //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                        currDatabase.LatestSizeCollectionDateTimeUtc = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["LatestSizeCollectionTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time
                        currDatabase.LatestStatsCollectionDateTimeUtc = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["LatestStatsCollectionTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                        currDatabase.CurrentDatabaseStatus = EnumHelpers.GetDatabaseStatusNameFromValue((int)GetValueOrDefault<int>(reader["LatestDatabaseStatus"]));
                        //currDatabase.CurrentDatabaseState = (string)GetValueOrDefault<string>(reader["DatabaseState"]); 

                        currDatabase.CurrentDataFileSizeInMb = (decimal)GetValueOrDefault<decimal>(reader["DataFileSizeMb"]);
                        currDatabase.CurrentDataSizeInMb = (decimal)GetValueOrDefault<decimal>(reader["DataSizeMb"]);
                        currDatabase.CurrentLogFileSizeInMb = (decimal)GetValueOrDefault<decimal>(reader["LogFileSizeMb"]);
                        currDatabase.CurrentLogSizeInMb = (decimal)GetValueOrDefault<decimal>(reader["LogSizeMb"]);
                        currDatabase.CurrentTotalFileSizeInMb = (decimal)GetValueOrDefault<decimal>(reader["TotalFileSizeMb"]);
                        currDatabase.CurrentTotalSizeInMb = (decimal)GetValueOrDefault<decimal>(reader["TotalSizeMb"]);
                        //START SQLdm 10.0 (Sanjali Makkar) - To Add Index Size and Text Size parameters
                        currDatabase.CurrentIndexSizeInMb = (decimal)GetValueOrDefault<decimal>(reader["IndexSizeMb"]);
                        currDatabase.CurrentTextSizeInMb = (decimal)GetValueOrDefault<decimal>(reader["TextSizeMb"]);
                        //END SQLdm 10.0 (Sanjali Makkar) - To Add Index Size and Text Size parameters

                        currDatabase.IsSystemDatabase = reader.GetBoolean(reader.GetOrdinal("IsSystemDatabase"));
                        currDatabase.IsInstanceEnabled = reader.GetBoolean(reader.GetOrdinal("IsInstanceEnabled"));
                        currDatabase.Transactions = (long)GetValueOrDefault<long>(reader["LatestTransactions"]);

                        currDatabase.DatabaseId = (int)GetValueOrDefault<int>(reader["DatabaseID"]);
                        currDatabase.InstanceId = instanceID;
                        currDatabase.noOfDataFiles = (int)GetValueOrDefault<int>(reader["DataFileCount"]);
                        currDatabase.noOfLogFiles = (int)GetValueOrDefault<int>(reader["LogFileCount"]);
                        //currDatabase.RecoveryModel = (string)GetValueOrDefault<string>(reader["RecoveryModel"]);

                        //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                        currDatabase.LastBackupDate = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["LastBackupDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                        //START SQLdm 10.0 (Sanjali Makkar) - To modify UnUsedDataSize parameter
                        currDatabase.UnUsedDataSizeInMb = currDatabase.CurrentDataFileSizeInMb - (currDatabase.CurrentDataSizeInMb + currDatabase.CurrentIndexSizeInMb + currDatabase.CurrentTextSizeInMb);
                        //END SQLdm 10.0 (Sanjali Makkar) - To modify UnUsedDataSize parameter
                        currDatabase.UnUsedLogSizeInMb = currDatabase.CurrentLogFileSizeInMb - currDatabase.CurrentLogSizeInMb;
                        currDatabase.UserTables = (int)GetValueOrDefault<int>(reader["UserTables"]);

                        _retValue.Add(currDatabase);
                    }
                }
            }

            return _retValue;
        }

        #endregion

        #region Top X

        /*public static IList<DC.MonitoredSqlServer> GetTopServerDatabaseAlerts(SqlConnectionInfo connectionInfo,DateTime startDate, DateTime endDate, int ServerCount)
        {
            if (connectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTopServersDatabaseAlertsStoredProcedure,
                     ServerCount,
                     !startDate.Equals(default(DateTime)) ? startDate : SqlDateTime.MinValue,
                     !endDate.Equals(default(DateTime)) ? endDate : DateTime.Now,
                     null))
                {
                    while (reader.Read())
                    {
                        IList<DC.MonitoredSqlServer> result = new List<DC.MonitoredSqlServer>(ServerCount);

                        while (reader.Read())
                        {
                            var checkExistingMS = result.Where(ms => ms.SQLServerId == (int)GetValueOrDefault<int>(reader["SQLServerID"])).FirstOrDefault();
                            if (checkExistingMS != null) // instance exists
                            {
                                checkExistingMS.Databases.Add(new DC.Database.MonitoredSqlServerDatabase(GetValueOrDefault<string>(reader["DatabaseName"]).ToString(), (int)GetValueOrDefault<int>(reader["AlertCount"])));
                            }
                            else // create new instance & add to result
                            {

                            DC.MonitoredSqlServer sqlserver = new DC.MonitoredSqlServer();
                            sqlserver.SQLServerId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);
                            sqlserver.InstanceName = GetValueOrDefault<string>(reader["InstanceName"]) as string;
                            sqlserver.Databases.Add(new DC.Database.MonitoredSqlServerDatabase(GetValueOrDefault<string>(reader["DatabaseName"]).ToString(), (int)GetValueOrDefault<int>(reader["AlertCount"])));                            
                            result.Add(sqlserver);
                            }
                        }

                        return result;
                    }

                    return null;
                }
            }
        }*/

        public static IList<DC.Widgets.LongestQueriesForInstance> GetTopQueriesByDuration(SqlConnectionInfo connectionInfo, string timeZoneOffset, int TopX, int numDays, UserToken userToken)
        {
            if (connectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTopQueriesByDurationStoredProcedure, TopX, numDays))
                {

                    IList<DC.Widgets.LongestQueriesForInstance> result = new List<DC.Widgets.LongestQueriesForInstance>(TopX);

                    while (reader.Read())
                    {
                        DC.Widgets.LongestQueriesForInstance queryInstance = new DC.Widgets.LongestQueriesForInstance();
                        queryInstance.InstanceId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);
                        queryInstance.InstanceName = GetValueOrDefault<string>(reader["InstanceName"]).ToString();
                        queryInstance.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                        queryInstance.Database = GetValueOrDefault<string>(reader["DatabaseName"]).ToString();
                        queryInstance.QueryExecTimeInMs = (long)GetValueOrDefault<long>(reader["DurationMilliseconds"]);
                        queryInstance.CPUTime = (long)GetValueOrDefault<long>(reader["CPUMilliseconds"]);
                        queryInstance.LogicalReads = (long)GetValueOrDefault<long>(reader["Reads"]);
                        queryInstance.LogicalWrites = (long)GetValueOrDefault<long>(reader["Writes"]);
                        queryInstance.QueryText = GetValueOrDefault<string>(reader["SQLSignature"]).ToString();

                        result.Add(queryInstance);
                    }

                    // SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                    return result.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.InstanceId)).ToList();
                    //return result;
                }
            }
        }

        public static IList<DC.Widgets.BlockedSessionForInstance> GetTopBlockedSessionCount(SqlConnectionInfo connectionInfo, string timeZoneOffset, int TopX, UserToken userToken)
        {
            if (connectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTopBlockedSessionCountStoredProcedure, TopX))
                {
                    IList<DC.Widgets.BlockedSessionForInstance> result = new List<DC.Widgets.BlockedSessionForInstance>(TopX);

                    while (reader.Read())
                    {
                        DC.Widgets.BlockedSessionForInstance blockedSession = new DC.Widgets.BlockedSessionForInstance();
                        blockedSession.InstanceId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);
                        blockedSession.InstanceName = GetValueOrDefault<string>(reader["InstanceName"]).ToString();
                        blockedSession.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                        blockedSession.Severity = (int)GetValueOrDefault<int>(reader["Severity"]);
                        blockedSession.BlockedSessionCount = (int)GetValueOrDefault<int>(reader["BlockedSessionCount"]);
                        result.Add(blockedSession);
                    }

                    // SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                    return result.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.InstanceId)).ToList();
                    //return result;
                }
            }
        }

        public static IList<DC.Widgets.InstancesByQueryCount> GetTopInstancesByQueryCount(SqlConnectionInfo connectionInfo, string timeZoneOffset, int TopX, UserToken userToken)
        {
            if (connectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTopInstancesByQueryCountStoredProcedure, TopX))
                {
                    IList<DC.Widgets.InstancesByQueryCount> result = new List<DC.Widgets.InstancesByQueryCount>(TopX);

                    while (reader.Read())
                    {
                        DC.Widgets.InstancesByQueryCount query = new DC.Widgets.InstancesByQueryCount();
                        query.InstanceId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);
                        query.InstanceName = GetValueOrDefault<string>(reader["InstanceName"]).ToString();
                        query.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                        query.Severity = (int)GetValueOrDefault<int>(reader["Severity"]);
                        query.NoOfQueries = (int)GetValueOrDefault<int>(reader["SqlQueryCount"]);
                        result.Add(query);
                    }

                    // SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                    return result.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.InstanceId)).ToList();
                    //return result;
                }
            }
        }

        internal static IDictionary<int, decimal[]> getThresholdsForMetric(SqlConnectionInfo connectionInfo, int metricId) //only for numeric thresholds
        {
            if (connectionInfo == null)
            {
                return null;
            }

            IDictionary<int, decimal[]> thresholds = new Dictionary<int, decimal[]>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, "p_GetMetricThresholdsForWebConsole", metricId, null))
                {
                    while (reader.Read())
                    {
                        decimal[] thresholdsForInstance = new decimal[3];
                        int instanceId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);

                        var ordinal = reader.GetOrdinal("InfoThreshold");
                        if (!reader.IsDBNull(ordinal))
                        {
                            decimal.TryParse(reader.GetString(ordinal), out thresholdsForInstance[0]);
                            thresholds.Add(instanceId, thresholdsForInstance);
                        }
                    }

                    while (reader.Read())
                    {
                        int instanceId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);

                        decimal[] thresholdsForInstance;
                        if (!thresholds.TryGetValue(instanceId, out thresholdsForInstance))
                        {
                            thresholdsForInstance = new decimal[3];
                            thresholds.Add(instanceId, thresholdsForInstance);
                        }

                        var ordinal = reader.GetOrdinal("WarningThreshold");
                        if (!reader.IsDBNull(ordinal))
                        {
                            decimal.TryParse(reader.GetString(ordinal), out thresholdsForInstance[1]);
                        }
                    }

                    while (reader.Read())
                    {
                        int instanceId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);

                        decimal[] thresholdsForInstance;
                        if (!thresholds.TryGetValue(instanceId, out thresholdsForInstance))
                        {
                            thresholdsForInstance = new decimal[3];
                            thresholds.Add(instanceId, thresholdsForInstance);
                        }

                        var ordinal = reader.GetOrdinal("CriticalThreshold");
                        if (!reader.IsDBNull(ordinal))
                        {
                            decimal.TryParse(reader.GetString(ordinal), out thresholdsForInstance[2]);
                        }
                    }
                }
            }
            return thresholds;
        }

        internal static string[] getThresholdsForMetricForInstance(SqlConnectionInfo connectionInfo, int metricId, int instanceId)
        {
            if (connectionInfo == null)
            {
                return null;
            }

            string[] thresholdsForInstance = new string[3];

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, "p_GetMetricThresholdsForWebConsole", metricId, instanceId))
                {
                    while (reader.Read())
                    {
                        var ordinal = reader.GetOrdinal("InfoThreshold");
                        if (!reader.IsDBNull(ordinal))
                        {
                            if (thresholdsForInstance[0] != null)
                            {
                                thresholdsForInstance[0] += ", ";
                            }
                            thresholdsForInstance[0] += reader.GetString(ordinal);
                        }
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        var ordinal = reader.GetOrdinal("WarningThreshold");
                        if (!reader.IsDBNull(ordinal))
                        {
                            if (thresholdsForInstance[1] != null)
                            {
                                thresholdsForInstance[1] += ", ";
                            }
                            thresholdsForInstance[1] += reader.GetString(ordinal);
                        }
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        var ordinal = reader.GetOrdinal("CriticalThreshold");
                        if (!reader.IsDBNull(ordinal))
                        {
                            if (thresholdsForInstance[2] != null)
                            {
                                thresholdsForInstance[2] += ", ";
                            }
                            thresholdsForInstance[2] += reader.GetString(ordinal);
                        }
                    }
                }
            }
            return thresholdsForInstance;
        }

        internal static int getSeverityForMetric(decimal metricValue, decimal[] thresholds)
        {
            int dir = 0;
            if (thresholds[0] < thresholds[1] || (thresholds[0] == thresholds[1] && thresholds[1] < thresholds[2]))
            {
                dir = 1;
            }
            else if (thresholds[0] > thresholds[1] || (thresholds[0] == thresholds[1] && thresholds[1] > thresholds[2]))
            {
                dir = -1;
            }

            int severity = 0;
            if (dir >= 0)
            {
                if (metricValue >= thresholds[0] && metricValue < thresholds[1])
                {
                    severity = 2;
                }
                else if (metricValue >= thresholds[1] && metricValue < thresholds[2])
                {
                    severity = 4;
                }
                else if (metricValue >= thresholds[2])
                {
                    severity = 8;
                }
                else
                {
                    severity = 1;
                }
            }
            else
            {
                if (metricValue <= thresholds[0] && metricValue > thresholds[1])
                {
                    severity = 2;
                }
                else if (metricValue <= thresholds[1] && metricValue > thresholds[2])
                {
                    severity = 4;
                }
                else if (metricValue <= thresholds[2])
                {
                    severity = 8;
                }
                else
                {
                    severity = 1;
                }
            }
            return severity;
        }

        //SQLdm 9.1 (Sanjali Makkar): adding the filter of Instance ID
        internal static IList<DC.Widgets.SessionsByCPUUsage> GetTopSessionsByCPUUsage(SqlConnectionInfo connectionInfo, string timeZoneOffset, int TopX, int? SQLServerID, UserToken userToken = null, bool UserSessionsOnly = true, bool excludeSQLDMSessions = true)
        {
            if (connectionInfo == null)
            {
                return null;
            }

            IDictionary<int, decimal[]> thresholds = getThresholdsForMetric(connectionInfo, 32);

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTopSessionsByCPUUsageStoredProcedure, TopX, SQLServerID))
                {
                    IList<DC.Widgets.SessionsByCPUUsage> result = new List<DC.Widgets.SessionsByCPUUsage>(TopX);

                    while (reader.Read())
                    {
                        int instanceId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);   //If InstanceID is provided, it will take the user-provided value, else default value
                        string instanceName = GetValueOrDefault<string>(reader["InstanceName"]).ToString();

                        //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                        DateTime utcCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                        IList<DC.Category.Sessions.SessionsForInstance> sessionStatistics = new List<DC.Category.Sessions.SessionsForInstance>();
                        var SessionListOrdinal = reader.GetOrdinal("SessionList");
                        if (!reader.IsDBNull(SessionListOrdinal))
                        {
                            SessionSnapshot ss = new SessionSnapshot(utcCollectionDateTime,
                                                        Serialized<object>.DeserializeCompressed
                                                            <Dictionary<Pair<int?, DateTime?>, Session>>(
                                                                (byte[])reader["SessionList"]
                                                            ));
                            sessionStatistics = ConvertToDataContract.ToDC(ss, timeZoneOffset);
                        }

                        foreach (DC.Category.Sessions.SessionsForInstance sfi in sessionStatistics)
                        {
                            bool add = (UserSessionsOnly && sfi.connection.IsUserSession) || (!UserSessionsOnly);
                            if (add)
                            {
                                if (excludeSQLDMSessions)
                                {
                                    if (sfi.connection.Application.StartsWith("SQL diagnostic manager", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        add = false;
                                    }
                                }
                            }

                            if (add)
                            {
                                DC.Widgets.SessionsByCPUUsage session = new DC.Widgets.SessionsByCPUUsage();
                                session.InstanceId = instanceId;
                                session.InstanceName = instanceName;
                                session.UTCCollectionDateTime = utcCollectionDateTime;

                                decimal[] thresholdsForInstance;
                                thresholds.TryGetValue(instanceId, out thresholdsForInstance);

                                session.Severity = getSeverityForMetric(sfi.usage.Cpu, thresholdsForInstance);

                                session.CPUUsageInMillisec = sfi.usage.Cpu;
                                session.DatabaseName = sfi.connection.Database;
                                session.Host = sfi.connection.Host;
                                session.SessionID = sfi.connection.Id;
                                if (instanceId != 0 && ServerAuthorizationHelper.IsServerAuthorized(instanceId, userToken))
                                {
                                    result.Add(session);
                                }

                            }
                        }
                    }
                    ((List<DC.Widgets.SessionsByCPUUsage>)result).Sort(delegate (DC.Widgets.SessionsByCPUUsage s1, DC.Widgets.SessionsByCPUUsage s2)
                    {
                        return s2.CPUUsageInMillisec.CompareTo(s1.CPUUsageInMillisec);
                    }
                    );
                    if (TopX > 0)
                    {
                        return result.Take(TopX).ToList();
                    }
                    return result;
                }
            }
        }

        internal static IList<DC.Widgets.DatabaseByActivity> GetTopDatabaseByActivity(SqlConnectionInfo connectionInfo, string timeZoneOffset, int TopX, UserToken userToken)
        {
            if (connectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTopDatabaseByActivityStoredProcedure, TopX))
                {
                    IList<DC.Widgets.DatabaseByActivity> result = new List<DC.Widgets.DatabaseByActivity>(TopX);

                    while (reader.Read())
                    {
                        DC.Widgets.DatabaseByActivity database = new DC.Widgets.DatabaseByActivity();
                        database.InstanceId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);
                        database.InstanceName = GetValueOrDefault<string>(reader["InstanceName"]).ToString();
                        database.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                        database.DatabaseName = GetValueOrDefault<string>(reader["DatabaseName"]).ToString();
                        database.TransactionPerSec = (double)GetValueOrDefault<double>(reader["TransactionsPerSecond"]);
                        result.Add(database);
                    }

                    // SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                    return result.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.InstanceId)).ToList();
                    //return result;
                }
            }
        }

        //SQLdm 8.5 (Ankit Srivastava): for Top X API- Tempdb Utilization
        internal static IList<TempDBUtilizationForInstance> GetInstancesByTempDbUtilization(SqlConnectionInfo connectionInfo, string timeZoneOffset, int passedCountOfInstances, UserToken userToken)
        {

            IList<TempDBUtilizationForInstance> result = new List<TempDBUtilizationForInstance>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetInstancesByTempDbUtilizationStoredProcedure,
                     passedCountOfInstances))
                {
                    while (reader.Read())
                    {
                        TempDBUtilizationForInstance tdufi = new TempDBUtilizationForInstance();

                        tdufi.InstanceId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);

                        var instanceNameOrdinal = reader.GetOrdinal("InstanceName");
                        tdufi.InstanceName = reader.IsDBNull(instanceNameOrdinal) ? "" : reader.GetString(instanceNameOrdinal);

                        var tempDBSizeOrdinal = reader.GetOrdinal("TempDBSizeInKilobytes");
                        tdufi.TempDBUsageInKB = reader.IsDBNull(tempDBSizeOrdinal) ? -1 : reader.GetInt64(tempDBSizeOrdinal);

                        //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                        tdufi.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                        result.Add(tdufi);
                    }
                    // SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                    return result.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.InstanceId)).ToList();
                    //return result;
                }
            }
        }

        //SQLdm 8.5 (Ankit Srivastava): for Top X API- Query Monitor Event
        internal static IEnumerable<DC.MonitoredSqlServer> GetInstancesByQueries(SqlConnectionInfo connectionInfo, DateTime startDate, DateTime endDate, int passedCountOfInstances)
        {
            IList<DC.MonitoredSqlServer> result = new List<DC.MonitoredSqlServer>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                bool _toAddtoResultset = false;

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetInstancesByQueriesStoredProcedure,
                     !startDate.Equals(default(DateTime)) ? startDate : SqlDateTime.MinValue,
                     !endDate.Equals(default(DateTime)) ? endDate : DateTime.Now,
                     passedCountOfInstances))
                {
                    while (reader.Read())
                    {
                        _toAddtoResultset = false;

                        DC.MonitoredSqlServer server = result.Where(r => r.SQLServerId == (int)GetValueOrDefault<int>(reader["SQLServerID"])).FirstOrDefault();
                        if (server == null)
                        {
                            server = new DC.MonitoredSqlServer();
                            server.SQLServerId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);

                            server.InstanceName = GetValueOrDefault<string>(reader["InstanceName"]).ToString();
                            server.NumOfQueries = GetValueOrDefault<string>(reader["NumOfQueries"]).ToString();
                            _toAddtoResultset = true;
                            server.FriendlyServerName = GetValueOrDefault<string>(reader["FriendlyServerName"]).ToString(); //SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
                        }

                        if (_toAddtoResultset)
                            result.Add(server);
                    }
                    return result;
                }
            }
        }

        //SQLdm 8.5 (Ankit Srivastava): for Top X API- Disk Space
        internal static IList<DC.Widgets.DiskSpaceByInstance> GetInstancesByDiskSpace(SqlConnectionInfo connectionInfo, string timeZoneOffset, int TopX, UserToken userToken)
        {
            if (connectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetInstancesByDiskSpaceStoredProcedure, TopX))
                {
                    IList<DC.Widgets.DiskSpaceByInstance> result = new List<DC.Widgets.DiskSpaceByInstance>(TopX);
                    while (reader.Read())
                    {
                        DC.Widgets.DiskSpaceByInstance blockedSession = new DC.Widgets.DiskSpaceByInstance();
                        blockedSession.InstanceId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);
                        blockedSession.InstanceName = GetValueOrDefault<string>(reader["InstanceName"]).ToString();
                        blockedSession.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                        blockedSession.Severity = (int)GetValueOrDefault<int>(reader["Severity"]);

                        var ordinal = reader.GetOrdinal("DiskUtilizationPercentage");
                        blockedSession.DiskSpaceUtilizationPercentage = reader.IsDBNull(ordinal) ? 0 : reader.GetDecimal(ordinal);
                        result.Add(blockedSession);
                    }

                    // SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                    return result.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.InstanceId)).ToList();
                    //return result;
                }
            }
        }

        internal static IList<DC.Widgets.SessionCountForInstance> GetTopInstancesBySessionCount(SqlConnectionInfo connectionInfo, string timeZoneOffset, int TopX, UserToken userToken)
        {
            if (connectionInfo == null)  //SQLdm 9.0 (Abhishek Joshi) --DE42790 -TopX <= 0 condition removed, TopX = -1 represents all records
            {
                return null;
            }
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                try
                {
                    connection.Open();

                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTopInstancesBySessionCountStoredProcedure))
                    {
                        IList<DC.Widgets.SessionCountForInstance> result = new List<DC.Widgets.SessionCountForInstance>();
                        IList<DC.Widgets.SessionCountForInstance> finalResult = new List<DC.Widgets.SessionCountForInstance>();

                        while (reader.Read())
                        {
                            int sessionListOrdinal = reader.GetOrdinal("SessionList");
                            if (!reader.IsDBNull(sessionListOrdinal))
                            {
                                DC.Widgets.SessionCountForInstance sessionCount = new DC.Widgets.SessionCountForInstance();
                                sessionCount.InstanceId = Convert.ToInt32(GetValueOrDefault<int>(reader["SQLServerID"]));
                                sessionCount.InstanceName = GetValueOrDefault<string>(reader["InstanceName"]).ToString();
                                sessionCount.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                                //blockedSession.Severity = (int)GetValue<int>(reader["Severity"]);
                                //blockedSession.SessionIDCount = (int)GetValueOrDefault<int>(reader["SessionList"]);

                                SessionSnapshot ss = new SessionSnapshot((DateTime)reader["UTCCollectionDateTime"],
                                                                Serialized<object>.DeserializeCompressed
                                                                    <Dictionary<Pair<int?, DateTime?>, Session>>(
                                                                        (byte[])reader["SessionList"]
                                                                    ));
                                sessionCount = ConvertToDataContract.ToDC(ss, sessionCount);
                                result.Add(sessionCount);
                            }
                        }
                        // SQLdm 10.0 (Swati Gogia):filtered the result list to contain only those sqlserver instances that the user has permission for. Implemented for Instance level security
                        finalResult = result.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.InstanceId)).ToList();

                        if (finalResult.Count > 0)
                        {
                            ((List<DC.Widgets.SessionCountForInstance>)finalResult).Sort(delegate (DC.Widgets.SessionCountForInstance s1, DC.Widgets.SessionCountForInstance s2)
                            {
                                return s2.SessionIDCount.CompareTo(s1.SessionIDCount);
                            });

                            //START: SQLdm 9.0 (Abhishek Joshi) --DE42790 -handling TopX = -1 case, TopX = -1 represents all records
                            if (TopX == -1)
                                return finalResult.ToList();
                            else
                                return finalResult.Take(TopX).ToList();
                            //END: SQLdm 9.0 (Abhishek Joshi) --DE42790 -handling TopX = -1 case, TopX = -1 represents all records
                        }

                        return null;

                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                }
            }
        }

        //SQLdm 8.5 (Gaurav Karwal): for Top X API- Instances by connection count
        internal static List<InstancesByConnectionCount> GetInstancesByConnectionCount(SqlConnectionInfo connectionInfo, string timeZoneOffset, int recordCount, UserToken userToken)
        {

            List<InstancesByConnectionCount> result = new List<InstancesByConnectionCount>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTopInstancesByConnectionCountStoredProcedure,
                     recordCount))
                {
                    while (reader.Read())
                    {
                        InstancesByConnectionCount instance = new InstancesByConnectionCount();

                        //SQLdm 10.0 (Sanjali Makkar) : Checking whether the values of respective fields returned from the stored proc are null or not
                        if (!reader.IsDBNull(reader.GetOrdinal("SQLServerID")))
                            instance.InstanceId = reader.GetInt32(reader.GetOrdinal("SQLServerID"));

                        if (!reader.IsDBNull(reader.GetOrdinal("Logins")))
                            instance.ActiveConnectionCount = reader.GetInt64(reader.GetOrdinal("Logins"));

                        if (!reader.IsDBNull(reader.GetOrdinal("InstanceName")))
                            instance.InstanceName = reader.GetString(reader.GetOrdinal("InstanceName"));

                        if (!reader.IsDBNull(reader.GetOrdinal("Severity")))
                            instance.Severity = reader.GetInt32(reader.GetOrdinal("Severity"));

                        if (!reader.IsDBNull(reader.GetOrdinal("UTCCollectionDateTime")))
                            instance.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                        result.Add(instance);
                    }

                }
            }
            // SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            return result.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.InstanceId)).ToList();
            //return result;
        }

        //SQLdm 8.5 (Gaurav Karwal): for Top X API- Instances by connection count
        internal static List<DatabasesByDatabaseFileSize> GetDatabasesByFileSize(SqlConnectionInfo connectionInfo, string timeZoneOffset, int recordCount, UserToken userToken)
        {

            List<DatabasesByDatabaseFileSize> result = new List<DatabasesByDatabaseFileSize>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTopDatabasesBySizeStoredProcedure,
                     recordCount))
                {
                    while (reader.Read())
                    {
                        int ordinal;
                        ordinal = reader.GetOrdinal("InstanceID");
                        if (!reader.IsDBNull(ordinal))
                        {
                            DatabasesByDatabaseFileSize instance = new DatabasesByDatabaseFileSize();
                            instance.InstanceId = reader.GetInt32(ordinal);

                            ordinal = reader.GetOrdinal("DataFileSizeMB");
                            if (!reader.IsDBNull(ordinal))
                                instance.FileSizeInMB = reader.GetDecimal(ordinal);

                            ordinal = reader.GetOrdinal("InstanceName");
                            if (!reader.IsDBNull(ordinal))
                                instance.InstanceName = reader.GetString(ordinal);

                            ordinal = reader.GetOrdinal("DatabaseName");
                            if (!reader.IsDBNull(ordinal))
                                instance.DatabaseName = reader.GetString(ordinal);

                            ordinal = reader.GetOrdinal("Severity");
                            if (!reader.IsDBNull(ordinal))
                                instance.Severity = reader.GetInt32(ordinal);

                            ordinal = reader.GetOrdinal("UTCCollectionDateTime");
                            if (!reader.IsDBNull(ordinal))
                                instance.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time


                            result.Add(instance);
                        }
                    }

                }
            }
            // SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            return result.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.InstanceId)).ToList();
            //return result;
        }

        //SQLdm 8.5 (Ankit Srivastava): for Top X API- Instance Alerts
        internal static IEnumerable<AlertsCountForInstance> GetInstancesByAlerts(SqlConnectionInfo connectionInfo, int passedCountOfInstances, UserToken userToken)
        {

            List<AlertsCountForInstance> result = new List<AlertsCountForInstance>();


            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTopInstancesByAlertsStoredProcedure,
                     passedCountOfInstances))
                {
                    while (reader.Read())
                    {
                        AlertsCountForInstance server = new AlertsCountForInstance();
                        server.InstanceId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);
                        server.InstanceName = GetValueOrDefault<string>(reader["InstanceName"]).ToString();
                        server.AlertCount = (int)GetValueOrDefault<int>(reader["NumOfAlerts"]);
                        server.MaxSeverity = (byte)GetValueOrDefault<byte>(reader["MaxSeverity"]);
                        result.Add(server);
                    }
                    // SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                    return result.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.InstanceId)).ToList();
                    //return result;
                }
            }
        }

        //SQLdm 8.5 (Ankit Srivastava): for Top X API- Most Alerts for Databases
        internal static IEnumerable<AlertsCountForDatabase> GetDatabasesByAlerts(SqlConnectionInfo connectionInfo, int passedCountOfInstances, UserToken userToken)
        {

            List<AlertsCountForDatabase> result = new List<AlertsCountForDatabase>();


            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTopDatabasesByAlerts,
                     passedCountOfInstances))
                {
                    while (reader.Read())
                    {
                        AlertsCountForDatabase server = new AlertsCountForDatabase();
                        server.InstanceId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);
                        server.InstanceName = GetValueOrDefault<string>(reader["InstanceName"]).ToString();
                        server.DatabaseName = GetValueOrDefault<string>(reader["DatabaseName"]).ToString();
                        server.MaxSeverity = (byte)GetValueOrDefault<byte>(reader["MaxSeverity"]);
                        server.AlertCount = (int)GetValueOrDefault<int>(reader["NumOfAlerts"]);
                        result.Add(server);
                    }
                    // SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                    return result.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.InstanceId)).ToList();
                    //return result;
                }
            }
        }

        //SQLdm 8.5 (Ankit Srivastava): for Top X API-  Sql Cpu Load
        internal static IEnumerable<SqlCpuLoadForInstance> GetInstancesBySqlCpuLoad(SqlConnectionInfo connectionInfo, int passedCountOfInstances, UserToken userToken)
        {

            List<SqlCpuLoadForInstance> result = new List<SqlCpuLoadForInstance>();


            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTopInstancesBySqlCpuLoad,
                     passedCountOfInstances))
                {
                    while (reader.Read())
                    {
                        SqlCpuLoadForInstance server = new SqlCpuLoadForInstance();
                        server.InstanceId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);
                        server.InstanceName = GetValueOrDefault<string>(reader["InstanceName"]).ToString();
                        server.CPUUsageInPercentage = (double)GetValueOrDefault<double>(reader["CPUActivityPercentage"]);
                        server.Severity = (int)GetValueOrDefault<int>(reader["Severity"]);
                        result.Add(server);
                    }
                    // SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                    return result.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.InstanceId)).ToList();
                    //return result;
                }
            }
        }

        //SQLdm 8.5 (Ankit Srivastava): for Top X API- IO Physical Count
        internal static IEnumerable<IOPhysicalUsageForInstance> GetInstancesByIOPhysicalCount(SqlConnectionInfo connectionInfo, int passedCountOfInstances, UserToken userToken)
        {

            List<IOPhysicalUsageForInstance> result = new List<IOPhysicalUsageForInstance>();


            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTopInstancesByIOPhysicalCount,
                     passedCountOfInstances))
                {
                    while (reader.Read())
                    {
                        IOPhysicalUsageForInstance server = new IOPhysicalUsageForInstance();
                        server.InstanceId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);
                        server.InstanceName = GetValueOrDefault<string>(reader["InstanceName"]).ToString();
                        server.SQLPhysicalIO = GetValueOrDefault<string>(reader["SQLPhysicalIO"]).ToString();
                        server.Severity = (int)GetValueOrDefault<int>(reader["Severity"]);
                        result.Add(server);
                    }
                    // SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                    return result.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.InstanceId)).ToList();
                    //return result;
                }
            }
        }

        //SQLdm 8.5 (Ankit Srivastava): for Top X API-  Sql Memory Usage
        internal static IEnumerable<SqlMemoryUsageForInstance> GetInstancesBySqlMemoryUsage(SqlConnectionInfo connectionInfo, int passedCountOfInstances, UserToken userToken)
        {

            List<SqlMemoryUsageForInstance> result = new List<SqlMemoryUsageForInstance>();


            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTopInstancesBySqlMemoryUsage,
                     passedCountOfInstances))
                {
                    while (reader.Read())
                    {
                        SqlMemoryUsageForInstance smufi = new SqlMemoryUsageForInstance();
                        smufi.InstanceId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);
                        smufi.Severity = (int)GetValueOrDefault<int>(reader["Severity"]);

                        var ordinal = reader.GetOrdinal("InstanceName");
                        smufi.InstanceName = reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);

                        ordinal = reader.GetOrdinal("SqlMemoryUsedInMB");
                        smufi.SqlMemoryUsedInMB = reader.IsDBNull(ordinal) ? 0 : reader.GetInt64(ordinal);

                        ordinal = reader.GetOrdinal("SqlMemoryAllocatedInMB");
                        smufi.SqlMemoryAllocatedInMB = reader.IsDBNull(ordinal) ? 0 : reader.GetInt64(ordinal);

                        result.Add(smufi);
                    }
                    // SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                    return result.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.InstanceId)).ToList();
                    //return result;
                }
            }
        }
        //SQLdm 9.1 (Sanjali Makkar): for Top X API- Sessions by I/O Activity
        internal static IList<DC.Widgets.SessionsByIOActivity> GetTopSessionsByIOActivity(SqlConnectionInfo connectionInfo, string timeZoneOffset, int TopX, int? SQLServerID, bool UserSessionsOnly = true, bool excludeSQLDMSessions = true)
        {
            if (connectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTopSessionsByIOActivityStoredProcedure, SQLServerID))
                {

                    IList<DC.Widgets.SessionsByIOActivity> result = new List<DC.Widgets.SessionsByIOActivity>(TopX);

                    long s1_PhysicalIO; //long type variables to store Physical IO of Sessions
                    long s2_PhysicalIO;

                    while (reader.Read())
                    {
                        string instanceName = GetValueOrDefault<string>(reader["InstanceName"]).ToString();

                        //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                        DateTime utcCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                        IList<DC.Category.Sessions.SessionsForInstance> sessionStatistics = new List<DC.Category.Sessions.SessionsForInstance>();
                        var sessionListOrdinal = reader.GetOrdinal("SessionList");
                        if (!reader.IsDBNull(sessionListOrdinal))
                        {
                            SessionSnapshot ss = new SessionSnapshot(utcCollectionDateTime,
                                                        Serialized<object>.DeserializeCompressed
                                                            <Dictionary<Pair<int?, DateTime?>, Session>>(
                                                                (byte[])reader["SessionList"]
                                                            ));
                            sessionStatistics = ConvertToDataContract.ToDC(ss, timeZoneOffset);
                        }

                        foreach (DC.Category.Sessions.SessionsForInstance sfi in sessionStatistics)
                        {
                            bool userSessionsOnly = (UserSessionsOnly && sfi.connection.IsUserSession) || (!UserSessionsOnly);
                            if (userSessionsOnly)
                            {
                                if (excludeSQLDMSessions)
                                {
                                    if (sfi.connection.Application.StartsWith("SQL diagnostic manager", StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        userSessionsOnly = false;
                                    }
                                }
                            }
                            if (userSessionsOnly)
                            {
                                DC.Widgets.SessionsByIOActivity session = new DC.Widgets.SessionsByIOActivity();
                                session.InstanceName = instanceName;
                                session.UTCCollectionDateTime = utcCollectionDateTime;
                                session.PhysicalIO = sfi.usage.PhysicalIO; //Physical IO extracted from Session List
                                session.SessionID = sfi.connection.Id;

                                result.Add(session);
                            }
                        }
                    }

                    ((List<DC.Widgets.SessionsByIOActivity>)result).Sort(delegate (DC.Widgets.SessionsByIOActivity s1, DC.Widgets.SessionsByIOActivity s2)
                    {
                        if (s1.PhysicalIO != null && s2.PhysicalIO != null)
                        {
                            s1_PhysicalIO = (long)s1.PhysicalIO; //CompareTo doesn't work for 'nullable' long
                            s2_PhysicalIO = (long)s2.PhysicalIO;
                            return s2_PhysicalIO.CompareTo(s1_PhysicalIO);
                        }
                        else
                        {
                            throw new InvalidOperationException("Physical IO is Null");
                        }
                    }
                    );

                    if (TopX > 0)
                    {
                        return result.Take(TopX).ToList();
                    }
                    return result;
                }
            }
        }

        internal static List<WaitStatisticsByInstance> GetInstancesByWaitStatistics(SqlConnectionInfo connectionInfo, int recordCount, UserToken userToken)
        {

            List<WaitStatisticsByInstance> result = new List<WaitStatisticsByInstance>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTopInstancesByWaitsStoredProcedure,
                     recordCount))
                {
                    while (reader.Read())
                    {
                        WaitStatisticsByInstance instance = new WaitStatisticsByInstance();
                        instance.InstanceId = reader.GetInt32(reader.GetOrdinal("SQLServerID"));
                        instance.WaitTimeInMs = reader.GetInt64(reader.GetOrdinal("WaitTimeInMilliseconds"));
                        instance.InstanceName = reader.GetString(reader.GetOrdinal("InstanceName"));
                        //instance.Severity = reader.GetInt32(reader.GetOrdinal("Severity"));
                        instance.Application = reader.GetString(reader.GetOrdinal("ApplicationName"));
                        instance.WaitType = reader.GetString(reader.GetOrdinal("WaitType"));
                        instance.UTCCollectionDateTime = reader.GetDateTime(reader.GetOrdinal("UTCCollectionDateTime"));
                        result.Add(instance);
                    }

                }
            }
            // SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            return result.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.InstanceId)).ToList();
            //return result;
        }

        internal static IList<ProjectedGrowthOfDatabaseSize> GetTopDatabasesByGrowth(SqlConnectionInfo sqlConnectionInfo, string timeZoneOffset, int TopX, int numHistoryDays, UserToken userToken)
        {
            if (sqlConnectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTopDatabaseByProjectedGrowthStoredProcedure, TopX, DateTime.Now, numHistoryDays))
                {
                    IList<ProjectedGrowthOfDatabaseSize> result = new List<ProjectedGrowthOfDatabaseSize>();

                    while (reader.Read())
                    {
                        int ordinal;
                        ordinal = reader.GetOrdinal("SQLServerID");
                        if (!reader.IsDBNull(ordinal))
                        {
                            ProjectedGrowthOfDatabaseSize database = new ProjectedGrowthOfDatabaseSize();
                            database.InstanceId = (int)GetValueOrDefault<int>(reader["SQLServerID"]);
                            database.InstanceName = GetValueOrDefault<string>(reader["InstanceName"]).ToString();

                            //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                            database.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                            database.DatabaseName = GetValueOrDefault<string>(reader["DatabaseName"]).ToString();
                            database.TotalSizeDiffernceKb = Convert.ToDecimal(GetValueOrDefault<decimal>(reader["TotalSizeDiffKb"]));
                            result.Add(database);
                        }
                    }

                    // SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                    return result.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.InstanceId)).ToList();
                    //return result;
                }
            }
        }

        #endregion

        #region Get Server Status

        public static IList<DC.MonitoredSqlServerStatus> GetStatus(int? instanceId)
        {
            IList<DC.MonitoredSqlServerStatus> monitoredSqlServerStatusList = new List<DC.MonitoredSqlServerStatus>();
            IDictionary<string, DC.MonitoredSqlServerStatus> severityToMSSSMap = new Dictionary<string, DC.MonitoredSqlServerStatus>();
            try
            {
                SqlConnectionInfo connectionInfo = RestServiceConfiguration.SQLConnectInfo;
                XmlDocument document = RepositoryHelper.GetMonitoredSqlServerStatus(connectionInfo, instanceId);

                if (document.DocumentElement.ChildNodes.Count > 0)
                {
                    foreach (XmlNode node in document.DocumentElement.ChildNodes)
                    {
                        DC.MonitoredSqlServer sqlserever = new DC.MonitoredSqlServer();
                        string severity = null;
                        int severityId = 0;
                        //string Category = null;
                        #region process instance basic details
                        foreach (XmlAttribute attribute in node.Attributes)
                        {
                            switch (attribute.Name)
                            {
                                case "SQLServerID":
                                    sqlserever.SQLServerId = Int32.Parse(attribute.Value);
                                    break;
                                case "InstanceName":
                                    sqlserever.InstanceName = attribute.Value;
                                    break;

                                case "ServerEdition":
                                    sqlserever.InstanceEdition = attribute.Value;
                                    break;

                                //SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
                                case "FriendlyServerName":
                                    sqlserever.FriendlyServerName = attribute.Value;
                                    break;


                            }
                        }
                        #endregion

                        #region process child nodes
                        // process the child nodes
                        foreach (XmlNode categoryNode in node.ChildNodes)
                        {
                            if (categoryNode.Attributes.Count == 0)
                                continue;

                            string category = categoryNode.Attributes["Name"].Value;

                            foreach (XmlNode statusNode in categoryNode.ChildNodes)
                            {
                                if (statusNode.Name == "State")
                                {
                                    foreach (XmlAttribute attribute in statusNode.Attributes)
                                    {
                                        switch (attribute.Name)
                                        {
                                            case "Severity":
                                                byte sev = 0;
                                                if (Byte.TryParse(attribute.Value, out sev))
                                                {
                                                    severity = ((MonitoredState)Enum.ToObject(typeof(MonitoredState), sev)).ToString();
                                                    severityId = Convert.ToInt32(sev);
                                                }
                                                break;
                                        }
                                    }
                                }

                                if (sqlserever.Categories.Where(c => c.name.Equals(category)).Count() > 0)
                                {
                                    var cateory = sqlserever.Categories.Where(c => c.name.Equals(category)).First();
                                    cateory.MaxSeverity = cateory.MaxSeverity > severityId ? cateory.MaxSeverity : severityId;
                                }
                                else
                                {
                                    sqlserever.Categories.Add(new DC.Categories { name = category, MaxSeverity = severityId });
                                }
                            }
                        }
                        #endregion
                        DC.MonitoredSqlServerStatus msss = null;
                        if (severity != null)
                        {
                            severityToMSSSMap.TryGetValue(severity, out msss);
                            if (msss != null)
                            {
                                msss.monitoredSqlServerList.Add(sqlserever);
                            }
                            else
                            {
                                msss = new DC.MonitoredSqlServerStatus();
                                msss.Severity = severity;
                                msss.monitoredSqlServerList.Add(sqlserever);
                                severityToMSSSMap.Add(severity, msss);
                                monitoredSqlServerStatusList.Add(msss);
                            }
                        }
                        /*else
                        {
                            msss = new DC.MonitoredSqlServerStatus();
                            msss.Severity = severity;
                            msss.monitoredSqlServerList.Add(sqlserever);
                            severityToMSSSMap.Add(severity, msss);
                        }*/
                    }
                }
            }
            catch (Exception e)
            {

                throw;
            }
            return monitoredSqlServerStatusList;
        }

        internal static void LoadServerTags(SqlConnectionInfo sqlConnectionInfo, IList<DC.ServerSummaryContainer> serverSummaryList, ICollection<Idera.SQLdm.Common.CWFDataContracts.GlobalTag> globaltags)
        {
            ICollection<Tag> tags = GetTags(sqlConnectionInfo);
            AddGlobalTags(tags, serverSummaryList, globaltags);
            IDictionary<int, IList<Tag>> instanceIdToTagList = new Dictionary<int, IList<Tag>>();
            foreach (Tag t in tags)
            {
                foreach (int instanceId in t.Instances)
                {
                    IList<Tag> tagsForInstance = null;
                    instanceIdToTagList.TryGetValue(instanceId, out tagsForInstance);
                    if (tagsForInstance == null)
                    {
                        tagsForInstance = new List<Tag>();
                        instanceIdToTagList.Add(instanceId, tagsForInstance);
                    }
                    tagsForInstance.Add(t);
                }
            }

            foreach (var serversummary in serverSummaryList)
            {
                if (serversummary.Overview != null)
                {
                    var instanceId = serversummary.Overview.SQLServerId;
                    IList<Tag> tagsForInstance = null;
                    instanceIdToTagList.TryGetValue(instanceId, out tagsForInstance);
                    if (tagsForInstance != null)
                    {
                        //SQLdm 10.1 - (Barkha khatri) 
                        //SQLDM 26533 fix-shiftng tags from MonitoredSqlServer to serverOverview
                        serversummary.Overview.Tags = tagsForInstance.Select(tg => tg.Name).ToArray();
                    }
                }
            }
        }
        /// <summary>
        ///  Function to add global tags
        ///  SQLdm 10.1 (Pulkit Puri)
        /// </summary>
        /// <param name="globalTags"></param>
        /// <param name="serverSummaryList"></param>
        /// <param name="syncTags"></param>
        private static void AddGlobalTags(ICollection<Tag> syncTags, IList<DC.ServerSummaryContainer> serverSummaryList, ICollection<Idera.SQLdm.Common.CWFDataContracts.GlobalTag> globalTags)
        {
            try
            {
                int lastTagID = 0;
                if (syncTags != null && syncTags.Count != 0)
                    lastTagID = syncTags.Select(i => i.Id).Max();

                List<int> serverIds = null;
                int serverID = ServerNotFound;
                if (globalTags != null && globalTags.Count > 0)
                {
                    foreach (Common.CWFDataContracts.GlobalTag gt in globalTags)
                    {

                        if (gt.Instances != null && gt.Instances.Count > 0)
                        {
                            serverIds = new List<int>();
                            foreach (string instanceName in gt.Instances)
                            {
                                serverID = GetInstanceIDFromName(serverSummaryList, instanceName);
                                if (serverID != ServerNotFound)
                                {
                                    serverIds.Add(serverID);
                                }
                            }
                        }

                        Tag tag = new Tag(++lastTagID, gt.Name, serverIds, null, null);
                        tag.IsGlobalTag = true;

                        syncTags.Add(tag);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while adding global tags to local tags list.", ex);
            }
        }
        /// <summary>
        ///  Function to add instnaces to list
        ///  SQLdm 10.1 (Pulkit Puri)
        /// </summary>
        /// <param name="serverSummaryList"></param>
        /// <param name="name"></param>
        private static int GetInstanceIDFromName(IList<DC.ServerSummaryContainer> serverSummaryList, string name)
        {
            try
            {
                DC.ServerSummaryContainer serverInTag = null;
                if (serverSummaryList != null && serverSummaryList.Count != 0)
                    serverInTag = serverSummaryList.First(p => p.Overview.InstanceName == name);
                if (serverInTag != null)
                    return serverInTag.Overview.SQLServerId;
                else
                    return ServerNotFound;
            }
            catch (Exception ex)
            {
                Log.Error("Error while getting instance id from name. ", ex);
                return ServerNotFound;
            }
        }

        /*internal static void LoadServerStatusFor(SqlConnectionInfo sqlConnectionInfo, IList<DC.ServerSummaryContainer> serverSummaryList)
        {
            ICollection<Tag> tags = GetTags(sqlConnectionInfo);           

            var MonitoredSQLServerStatusList = GetStatus(null);

            foreach(var serversummary in serverSummaryList)
            {
                LoadServerStatusFor(sqlConnectionInfo, serversummary, tags, MonitoredSQLServerStatusList);
            }               
        }*/

        internal static void LoadServerStatusFor(SqlConnectionInfo sqlConnectionInfo, DC.ServerSummaryContainer serversummary, ICollection<Tag> tags, IList<DC.MonitoredSqlServerStatus> MonitoredSQlServerStatusList)
        {
            if (serversummary == null || serversummary.Overview == null)
                return;

            if (tags == null)
                tags = GetTags(sqlConnectionInfo);

            if (MonitoredSQlServerStatusList == null)
                MonitoredSQlServerStatusList = GetStatus(null);

            var tagforserver = tags.Where(t => t != null && t.Instances != null && t.Instances.Contains(serversummary.Overview.SQLServerId));
            if (tagforserver.Count() > 0)
            {
                //SQLdm 10.1 - (Barkha khatri) 
                //SQLDM 26533 fix-shiftng tags from MonitoredSqlServer to serverOverview
                serversummary.Overview.Tags = tagforserver.Select(tg => tg.Name).ToArray();
            }
            if (serversummary.ServerStatus != null)
            {
                foreach (var sqlServerCollection in MonitoredSQlServerStatusList.Select(mss => mss.SqlServerCollection))
                {
                    var monitoredSqlserver = sqlServerCollection.Where(ssc => ssc.SQLServerId == serversummary.ServerStatus.SQLServerId).FirstOrDefault();
                    if (monitoredSqlserver != null)
                        serversummary.ServerStatus.Categories = monitoredSqlserver.Categories;
                }
            }
        }

        #endregion

        #region Categories

        internal static IList<DC.Category.QueryStatisticsForInstance> GetQueryStatsForInstance(SqlConnectionInfo sqlConnectionInfo, int passedinstanceId, string timeZoneOffset, int NumHistoryMinutes, int limit)
        {
            if (sqlConnectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                IList<DC.Category.QueryStatisticsForInstance> result = new List<DC.Category.QueryStatisticsForInstance>();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetInstanceQueryStatsStoredProcedure, passedinstanceId, NumHistoryMinutes,
                    limit, null, null))
                {
                    while (reader.Read())
                    {
                        DC.Category.QueryStatisticsForInstance queryStats = new DC.Category.QueryStatisticsForInstance();
                        queryStats.ApplicationName = GetValueOrDefault<string>(reader["ApplicationName"]).ToString();
                        queryStats.AverageDuration = Convert.ToDouble(GetValueOrDefault<double>(reader["AvgDurationMilliseconds"]));
                        queryStats.AverageReads = Convert.ToDouble(GetValueOrDefault<double>(reader["AvgReads"]));
                        queryStats.AverageWrites = Convert.ToDouble(GetValueOrDefault<double>(reader["AvgWrites"]));
                        queryStats.client = GetValueOrDefault<string>(reader["HostName"]).ToString();
                        queryStats.Cpuaverage = Convert.ToDouble(GetValueOrDefault<double>(reader["AvgCpu"]));
                        queryStats.Cputotal = Convert.ToDouble(GetValueOrDefault<double>(reader["TotalCPu"]));
                        queryStats.DatabaseName = GetValueOrDefault<string>(reader["DatabaseName"]).ToString();
                        queryStats.EventType = GetValueOrDefault<string>(reader["StatementType"]).ToString();
                        queryStats.Occurences = Convert.ToInt32(GetValueOrDefault<int>(reader["Occurrence"]));
                        queryStats.QueryName = GetValueOrDefault<string>(reader["SQLSignature"]).ToString();
                        queryStats.SqlText = GetValueOrDefault<string>(reader["SQLStatementText"]).ToString();

                        //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                        queryStats.StartTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["StatementUTCStartTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                        queryStats.User = GetValueOrDefault<string>(reader["LoginName"]).ToString();
                        queryStats.TotalReads = Convert.ToDouble(GetValueOrDefault<long>(reader["TotalReads"]));
                        queryStats.TotalWrites = Convert.ToInt64(GetValueOrDefault<double>(reader["TotalWrites"]));
                        queryStats.AvgCPUPerSecond = Convert.ToDouble(GetValueOrDefault<double>(reader["AvgCPUPerSecond"]));
                        queryStats.SignatureId = Convert.ToInt64(GetValueOrDefault<long>(reader["SQLSignatureID"]));
                        queryStats.SpId = Convert.ToInt32(GetValueOrDefault<int>(reader["Spid"]));
                        result.Add(queryStats);
                    }

                    result = result.OrderByDescending(x => x.StartTime).ToList<DC.Category.QueryStatisticsForInstance>();
                    result = AddQueryNums(result);
                    return result;

                }
            }
        }

        private static IList<DC.Category.QueryStatisticsForInstance> AddQueryNums(IList<DC.Category.QueryStatisticsForInstance> result)
        {
            int nextQueryId = 0;
            Dictionary<long, Dictionary<int, Dictionary<int, int>>> queryNames = new Dictionary<long, Dictionary<int, Dictionary<int, int>>>();
            string name;

            nextQueryId = queryNames.Count + 1;
            int signatureNameId;
            int uniqueId = 0;

            Dictionary<int, int> statementNames;
            foreach (var eachResult in result)
            {
                long id = (long)eachResult.SignatureId;
                int statementHash = getRowHash(eachResult);

                Dictionary<int, Dictionary<int, int>> nameLists;
                if (queryNames.TryGetValue(id, out nameLists))
                {
                    signatureNameId = nameLists.Keys.FirstOrDefault();
                    statementNames = nameLists.Values.FirstOrDefault();
                    name = getUniqueStatementName(signatureNameId, statementHash, statementNames, out uniqueId);

                }
                else
                {
                    signatureNameId = nextQueryId++;
                    statementNames = new Dictionary<int, int>();
                    name = getUniqueStatementName(signatureNameId, statementHash, statementNames, out uniqueId);
                    nameLists = new Dictionary<int, Dictionary<int, int>>();
                    nameLists.Add(signatureNameId, statementNames);
                    queryNames.Add(id, nameLists);
                }

                eachResult.QueryNum = name;
                //row[COL_NAME_SORT] = (signatureNameId << 40) + uniqueId;
            }
            return result;

        }

        private static int getRowHash(DC.Category.QueryStatisticsForInstance element)
        {
            // note including an invalid db name character to frame it and prevent runon matches

            string str = String.Empty;
            switch (element.EventType)
            {
                case "StoredProcedure":
                    str = "0";
                    break;
                case "SingleStatement":
                    str = "1";
                    break;
                case "Batch":
                    str = "2";
                    break;
            };
            StringBuilder hashText = new StringBuilder(str);

            hashText.Append(':');
            hashText.Append(element.DatabaseName);
            hashText.Append(':');
            hashText.Append(element.ApplicationName);
            hashText.Append(':');
            hashText.Append(element.StartTime.ToString());
            hashText.Append(':');
            hashText.Append(element.User);
            hashText.Append(':');
            hashText.Append(element.client);
            hashText.Append(':');
            hashText.Append(element.SpId);

            return hashText.ToString().GetHashCode();
        }

        private static string getUniqueStatementName(int id, int hash, Dictionary<int, int> statements, out int statementId)
        {
            const string FORMAT_STATEMENT_NAME = "Query {0}{1}{2}";
            const string FORMAT_NAME_SEP = @"_";
            if (!statements.TryGetValue(hash, out statementId))
            {
                statementId = statements.Count + 1;
                statements.Add(hash, statementId);
            }

            return string.Format(FORMAT_STATEMENT_NAME,
                                        id,
                                        statementId > 1 ? FORMAT_NAME_SEP : string.Empty,
                                        statementId > 1 ? statementId.ToString() : string.Empty);
        }


        internal static IList<DC.Category.ResourcesForInstance> GetResourcesForInstance(SqlConnectionInfo sqlConnectionInfo, int passedinstanceId, string timeZoneOffset, int NumHistoryMinutes, DateTime endDate, int limit)
        {
            if (sqlConnectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                IList<DC.Category.ResourcesForInstance> result = new List<DC.Category.ResourcesForInstance>();

                DateTime utcCollectionEndTime;
                if (!endDate.Equals(default(DateTime)))
                    utcCollectionEndTime = DateTimeHelper.ConvertToUTC(endDate, timeZoneOffset);
                else
                    utcCollectionEndTime = DateTime.UtcNow;

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetInstanceResourcesStoredProcedure, passedinstanceId, utcCollectionEndTime, NumHistoryMinutes, limit))
                {
                    while (reader.Read())
                    {
                        DC.Category.ResourcesForInstance resource = new DC.Category.ResourcesForInstance();
                        resource.cpu = new DC.Category.ResourcesForInstance.CPU
                        {
                            OSCPUUsage = (double)GetValueOrDefault<double>(reader["OSCpu"]),
                            ProcessorQueueLength = (double)GetValueOrDefault<double>(reader["ProcessorQueueLength"]),
                            SqlCPUUsage = (double)GetValueOrDefault<double>(reader["SSCpu"]),
                            ProcessorPrivilegedTimePercent = (double)GetValueOrDefault<double>(reader["PrivilegedTimePercent"]),
                            ProcessorUserTimePercent = (double)GetValueOrDefault<double>(reader["UserTimePercent"])
                        };

                        IDictionary<string, long> readsPerSecond = new Dictionary<string, long>();
                        readsPerSecond.Add(GetValueOrDefault<string>(reader["DriveName"]).ToString(), (long)GetValueOrDefault<long>(reader["DiskReadsPerSecond"]));

                        IDictionary<string, long> writesPerSecond = new Dictionary<string, long>();
                        writesPerSecond.Add(GetValueOrDefault<string>(reader["DriveName"]).ToString(), (long)GetValueOrDefault<long>(reader["DiskWritesPerSecond"]));

                        //START: SQLdm 9.1 (Sanjali Makkar) --Disk Drives -add the parameter of Disk Transfers per second
                        IDictionary<string, long> transfersPerSecond = new Dictionary<string, long>();
                        transfersPerSecond.Add(GetValueOrDefault<string>(reader["DriveName"]).ToString(), (long)GetValueOrDefault<long>(reader["DiskTransfersPerSecond"]));
                        //END: SQLdm 9.1 (Sanjali Makkar) --Disk Drives -add the parameter of Disk Transfers per second

                        //Nishant Adhikari 10.2 AverageDiskMillisecondsPer Read Write Transfer
                        //start
                        IDictionary<string, long> averageDiskMillisecondsPerRead = new Dictionary<string, long>();
                        averageDiskMillisecondsPerRead.Add(GetValueOrDefault<string>(reader["DriveName"]).ToString(), (long)GetValueOrDefault<long>(reader["AverageDiskMillisecondsPerRead"]));

                        IDictionary<string, long> averageDiskMillisecondsPerWrite = new Dictionary<string, long>();
                        averageDiskMillisecondsPerWrite.Add(GetValueOrDefault<string>(reader["DriveName"]).ToString(), (long)GetValueOrDefault<long>(reader["AverageDiskMillisecondsPerWrite"]));

                        IDictionary<string, long> averageDiskMillisecondsPerTransfer = new Dictionary<string, long>();
                        averageDiskMillisecondsPerTransfer.Add(GetValueOrDefault<string>(reader["DriveName"]).ToString(), (long)GetValueOrDefault<long>(reader["AverageDiskMillisecondsPerTransfer"]));

                        //end

                        resource.disk = new DC.Category.ResourcesForInstance.Disk
                        {
                            DiskReadsPerSecond = readsPerSecond,
                            DiskWritesPerSecond = writesPerSecond,
                            DiskTransfersPerSecond = transfersPerSecond, //SQLdm 9.1 (Sanjali Makkar) --Disk Drives -add the parameter of Disk Transfers per second
                            //Nishant SQLdm 10.2 AverageDiskMillisecondsPer Read Write Transfer
                            //start
                            AverageDiskMillisecondsPerRead = averageDiskMillisecondsPerRead,
                            AverageDiskMillisecondsPerWrite = averageDiskMillisecondsPerWrite,
                            AverageDiskMillisecondsPerTransfers = averageDiskMillisecondsPerTransfer,
                            //end
                            SqlPhysicalIO = new DC.Category.ResourcesForInstance.SqlPhysicalIO
                            {
                                CheckPointWrites = (double)GetValueOrDefault<double>(reader["CheckpointWrites"]),
                                LazyWriterWrites = (double)GetValueOrDefault<double>(reader["LazyWriterWrites"]),
                                PageReads = (double)GetValueOrDefault<double>(reader["PageReads"]),
                                PageWrites = (double)GetValueOrDefault<double>(reader["PageWrites"]),
                                ReadAheadPages = (double)GetValueOrDefault<double>(reader["ReadAheadPages"])
                            }
                        };

                        resource.memory = new DC.Category.ResourcesForInstance.Memory
                        {
                            BufferCacheHitRatio = (double)GetValueOrDefault<double>(reader["BufferCacheHitRatioPercentage"]),
                            MemoryAreas = new DC.Category.ResourcesForInstance.MemoryAreas
                            {
                                ConnectionsMemoryInKB = (Int64)GetValueOrDefault<Int64>(reader["ConnectionMemoryInKilobytes"]),
                                DatabaseMemoryInKB = (long)GetValueOrDefault<long>(reader["CommittedInKilobytes"]),
                                LocksMemoryInKB = (Int64)GetValueOrDefault<Int64>(reader["LockMemoryInKilobytes"]),
                                ProcedureCacheSizeInKB = (Int64)GetValueOrDefault<Int64>(reader["ProcedureCacheSizeInKilobytes"]),
                                //SQL dm 10.2 Nishant Added for two new features free and others
                                FreePagesInKilobytes = (Int64)GetValueOrDefault<Int64>(reader["FreePagesInKilobytes"]),
                                Others = (Int64)GetValueOrDefault<Int64>(reader["OptimizerMemoryInKilobytes"])
                                        + (Int64)GetValueOrDefault<Int64>(reader["LockMemoryInKilobytes"])
                                        + (Int64)GetValueOrDefault<Int64>(reader["ConnectionMemoryInKilobytes"])
                                        + (Int64)GetValueOrDefault<Int64>(reader["GrantedWorkspaceMemoryInKilobytes"]),
                                //end
                            },
                            PageLifeExpectancyinSec = (Int64)GetValueOrDefault<Int64>(reader["PageLifeExpectancy"]),
                            ProcedureCacheHitRatio = (double)GetValueOrDefault<double>(reader["ProcedureCacheHitRatioPercentage"]),
                            SqlMemory = new DC.Category.ResourcesForInstance.SqlMemory
                            {
                                AllocatedMemoryInKB = (Int64)GetValueOrDefault<Int64>(reader["SqlMemoryAllocatedInKilobytes"]),
                                TotalMemoryInKB = (Int64)GetValueOrDefault<Int64>(reader["TotalServerMemoryInKilobytes"]),
                                UsedMemoryInKB = (Int64)GetValueOrDefault<Int64>(reader["SqlMemoryUsedInKilobytes"])
                            }
                        };

                        //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                        resource.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                        result.Add(resource);
                    }

                    return result;
                }
            }
        }

        internal static IList<DC.Category.FileDrivesForInstance> GetFileDrivesForInstance(SqlConnectionInfo sqlConnectionInfo, int passedinstanceId)
        {
            if (sqlConnectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetDiskDrivesStoredProcedure, passedinstanceId))
                {
                    IList<DC.Category.FileDrivesForInstance> result = new List<DC.Category.FileDrivesForInstance>();

                    while (reader.Read())
                    {
                        DC.Category.FileDrivesForInstance filedrive = new DC.Category.FileDrivesForInstance();
                        filedrive.DriveName = GetValueOrDefault<string>(reader["DriveName"]).ToString();
                        result.Add(filedrive);
                    }

                    return result;
                }
            }
        }

        internal static IList<DC.Category.FileActivityForInstance> GetFileActivityForInstance(SqlConnectionInfo sqlConnectionInfo, int passedinstanceId, string timeZoneOffset, int NumHistoryMinutes)
        {
            if (sqlConnectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetFileActivityStoredProcedure, passedinstanceId,
                    null, NumHistoryMinutes))
                {
                    IList<DC.Category.FileActivityForInstance> result = new List<DC.Category.FileActivityForInstance>();

                    while (reader.Read())
                    {
                        DC.Category.FileActivityForInstance fileActivity = new DC.Category.FileActivityForInstance();
                        fileActivity.Drive = GetValueOrDefault<string>(reader["DriveName"]).ToString();
                        fileActivity.DatebaseName = GetValueOrDefault<string>(reader["DatabaseName"]).ToString();
                        fileActivity.FileName = GetValueOrDefault<string>(reader["FileName"]).ToString();
                        fileActivity.FileType = (reader["FileType"] is DBNull) ? String.Empty : EnumHelpers.GetFileActivityFileTypeFromValue(Convert.ToInt32(reader["FileType"]));
                        fileActivity.FilePath = reader["FilePath"].ToString();

                        //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                        fileActivity.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                        fileActivity.statistics = new DC.Category.FileActivityForInstance.Statistics();


                        fileActivity.statistics.DiskReadsPerSec = (long)GetValueOrDefault<long>(reader["DiskReadsPerSecond"]);


                        fileActivity.statistics.DiskWritesPerSec = (long)GetValueOrDefault<long>(reader["DiskWritesPerSecond"]);


                        fileActivity.statistics.FileReadsPerSec = (double)GetValueOrDefault<double>(reader["FileReadsPerSecond"]);


                        fileActivity.statistics.FileWritesPerSec = (double)GetValueOrDefault<double>(reader["FileWritesPerSecond"]);
                        result.Add(fileActivity);
                    }

                    return result;
                }
            }
        }

        internal static IList<DC.Category.ServerWaitsForInstance> GetServerWaitsForInstance(SqlConnectionInfo sqlConnectionInfo, int passedinstanceId, string timeZoneOffset, int NumHistoryMinutes, DateTime endDate, string category)
        {
            if (sqlConnectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                //Fix For SQLDM-27316 time was not taken from java url rather it was taken from system so problem if server and api hitting from different places  
                //begin
                //DateTime utcCollectionTime = DateTime.UtcNow; Removed for this
                //added for this
                DateTime utcCollectionDateTime;
                if (!endDate.Equals(default(DateTime)))
                    utcCollectionDateTime = DateTimeHelper.ConvertToUTC(endDate, timeZoneOffset);
                else
                    utcCollectionDateTime = DateTime.UtcNow;
                //end
                //SQLdm 10.2 (Anshika Sharma) : Added EndTime to include History Range


                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetWaitStatisticsStoredProcedure, passedinstanceId,
                    utcCollectionDateTime, NumHistoryMinutes,
                    category))
                {
                    IList<DC.Category.ServerWaitsForInstance> result = new List<DC.Category.ServerWaitsForInstance>();

                    while (reader.Read())
                    {
                        DC.Category.ServerWaitsForInstance serverWaits = new DC.Category.ServerWaitsForInstance();
                        serverWaits.Category = GetValueOrDefault<string>(reader["Category"]).ToString();
                        serverWaits.WaitType = GetValueOrDefault<string>(reader["WaitType"]).ToString();
                        serverWaits.WaitingTasks = (Int64)GetValueOrDefault<Int64>(reader["WaitingTasks"]);

                        //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                        serverWaits.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                        var ordinal = reader.GetOrdinal("Description");
                        serverWaits.Description = reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);

                        ordinal = reader.GetOrdinal("HelpLink");
                        serverWaits.HelpLink = reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);

                        serverWaits.statistics = new DC.Category.ServerWaitsForInstance.Statistics
                        {
                            ResourceWaitInMils = new DC.Category.ServerWaitsForInstance.WaitDescription
                            {
                                Wait = GetValueOrDefault<string>(reader["ResourceWaitMillisecondsPerSecond"]).ToString(),
                                TotalWait = GetValueOrDefault<string>(reader["ResourceWaitTimeInMilliseconds"]).ToString()
                            },
                            SignalWaitInMils = new DC.Category.ServerWaitsForInstance.WaitDescription
                            {
                                Wait = GetValueOrDefault<string>(reader["SignalWaitMillisecondsPerSecond"]).ToString(),
                                TotalWait = GetValueOrDefault<string>(reader["SignalWaitTimeInMilliseconds"]).ToString()
                            },
                            TotalWaitInMils = new DC.Category.ServerWaitsForInstance.WaitDescription
                            {
                                Wait = GetValueOrDefault<string>(reader["TotalWaitMillisecondsPerSecond"]).ToString(),
                                TotalWait = GetValueOrDefault<string>(reader["WaitTimeInMilliseconds"]).ToString()
                            }
                        };
                        result.Add(serverWaits);
                    }

                    return result;
                }
            }
        }

        /*        internal static IList<DC.Category.Sessions.SessionsForInstance> GetSessionsForInstance(SqlConnectionInfo sqlConnectionInfo, int passedinstanceId, int limit, string type)
                {
                    if (sqlConnectionInfo == null)
                    {
                        return null;
                    }
                    using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                    {
                        connection.Open();
                        using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetSessionsDetailsStoredProcedure, passedinstanceId, null))
                        {
                            SessionSnapshot snapshot = null;
                            if(dataReader.Read())
                            {

                                if (dataReader.HasRows && dataReader["SessionList"] != DBNull.Value)
                                {
                                    snapshot = new SessionSnapshot((DateTime)dataReader["UTCCollectionDateTime"],
                                                            Serialized<object>.DeserializeCompressed
                                                                <Dictionary<Pair<int?, DateTime?>, Session>>(
                                                                (byte[])dataReader["SessionList"]));
                                }                           
                            }

                            IList<DC.Category.Sessions.SessionsForInstance> result = new List<DC.Category.Sessions.SessionsForInstance>();
                            if (snapshot != null) 
                            {
                                result = ConvertToDataContract.ToDC(snapshot);
                            }

                            // filter result on type
                            if(!string.IsNullOrEmpty(type))
                            {
                                result = result.Where(res => res.connection != null && res.connection.Type.Equals(type, StringComparison.OrdinalIgnoreCase)).ToList();
                            }

                            return result.OrderByDescending(res => res.UTCCollectionDateTime).ToList();
                        }               
                    }
                }*/

        /*internal static IList<DC.Category.Sessions.SessionActivityForInstance> GetSessionsActivityForInstance(SqlConnectionInfo sqlConnectionInfo, int passedinstanceId, int NumHistoryMinutes)
        {
            if (sqlConnectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                IList<DC.Category.Sessions.SessionActivityForInstance> result = new List<DC.Category.Sessions.SessionActivityForInstance>();
                result.Add(new DC.Category.Sessions.SessionActivityForInstance
                {
                    timeinmils = default(TimeSpan),
                    type = DC.Category.Sessions.SessionType.Active
                });

                
                //using (SqlDataReader reader = SqlHelper.ExecuteReader(connection,GetServerActivityStoredProcedure, passedinstanceId,
                //    !start.Equals(default(DateTime)) ? (object)start : null,
                //    !end.Equals(default(DateTime)) ? (object)end : null,
                //    (int)0xAB,
                //    null))
                //{
                //    IList<SessionSummary> sessionlist = new List<SessionSummary>();
                //    while (reader.Read())
                //    {
                //        try
                //        {   
                //            sessionlist.Add(new SessionSummary(reader));                         
                //        }
                //        catch (Exception e)
                //        {
                //            Log.WarnFormat("Unable to reconstitute serialized object", e);
                //        }
                //    }

                //    return ConvertToDataContract.ToDC(sessionlist);
                //}
                
                //using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTopQueriesByDurationStoredProcedure, passedinstanceId, start, end))
                //{
                //    IList<DC.Widgets.LongestQueriesForInstance> result = new List<DC.Widgets.LongestQueriesForInstance>();

                //    while (reader.Read())
                //    {
                //        DC.Widgets.LongestQueriesForInstance queryInstance = new DC.Widgets.LongestQueriesForInstance();
                //        queryInstance.InstanceId = (int)GetValue<int>(reader["SQLServerID"]);
                //        queryInstance.InstanceName = GetValue<string>(reader["InstanceName"]).ToString();
                //        queryInstance.UTCCollectionDateTime = (DateTime)GetValue<DateTime>(reader["UTCCollectionDateTime"]);
                //        queryInstance.Database = GetValue<string>(reader["DatabaseName"]).ToString();
                //        queryInstance.QueryExecTimeInMs = (long)GetValue<long>(reader["DurationMilliseconds"]);
                //        queryInstance.CPUTime = (long)GetValue<long>(reader["CPUMilliseconds"]);
                //        queryInstance.LogicalReads = (long)GetValue<long>(reader["Reads"]);
                //        queryInstance.LogicalWrites = (long)GetValue<long>(reader["Writes"]);
                //        queryInstance.QueryText = GetValue<string>(reader["SQLSignature"]).ToString();

                //        result.Add(queryInstance);
                //    }

                //    return result;
                //}
                return result;
            }
        }*/

        internal static IList<DC.Category.Sessions.SessionResponseTimeForInstance> GetSessionResponseTimeForInstance(SqlConnectionInfo sqlConnectionInfo, int passedinstanceId, int NumHistoryMinutes)
        {
            if (sqlConnectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetServerActivityStoredProcedure, passedinstanceId,
                        null, null, (int)128, 0)) //TODO: Ashu: Review with proc. Passing null for both start and end. Proc will go over all collections. Should pass some start date or consider adding NumHistoryMinutes
                {
                    IList<SessionSummary> sessionlist = new List<SessionSummary>();
                    while (reader.Read())
                    {
                        try
                        {
                            sessionlist.Add(new SessionSummary(reader));
                        }
                        catch (Exception e)
                        {
                            Log.WarnFormat("Unable to reconstitute serialized object", e);
                        }
                    }

                    return ConvertToDataContract.ToDC(sessionlist);
                }
            }
        }

        public static IList<DC.Databases.TempDBStats> GetTempDBStatsForInstance(SqlConnectionInfo sqlConnectionInfo, int passedinstanceId, string timeZoneOffset, int NumHistoryMinutes, DateTime endDate)
        {
            if (sqlConnectionInfo == null)
            {
                return null;
            }

            using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                DateTime utcCollectionDateTime;
                if (!endDate.Equals(default(DateTime)))
                    utcCollectionDateTime = DateTimeHelper.ConvertToUTC(endDate, timeZoneOffset);
                else
                    utcCollectionDateTime = DateTime.UtcNow;
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetTempdbSummaryDataStoredProcedure, passedinstanceId, utcCollectionDateTime, NumHistoryMinutes))
                {
                    IList<DC.Databases.TempDBStats> result = new List<DC.Databases.TempDBStats>();

                    while (reader.Read())
                    {
                        DC.Databases.TempDBStats tempDBStats = new DC.Databases.TempDBStats();

                        //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                        tempDBStats.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                        var ordinal = reader.GetOrdinal("UserObjectsInMB");
                        tempDBStats.UserObjectsInMB = reader.IsDBNull(ordinal) ? 0.0m : reader.GetDecimal(ordinal);

                        ordinal = reader.GetOrdinal("InternalObjectsInMB");
                        tempDBStats.InternalObjectsInMB = reader.IsDBNull(ordinal) ? 0.0m : reader.GetDecimal(ordinal);

                        ordinal = reader.GetOrdinal("VersionStoreInMB");
                        tempDBStats.VersionStoreInMB = reader.IsDBNull(ordinal) ? 0.0m : reader.GetDecimal(ordinal);

                        ordinal = reader.GetOrdinal("MixedExtentsInMB");
                        tempDBStats.MixedExtentsInMB = reader.IsDBNull(ordinal) ? 0.0m : reader.GetDecimal(ordinal);

                        ordinal = reader.GetOrdinal("UnallocatedSpaceInMB");
                        tempDBStats.UnallocatedSpaceInMB = reader.IsDBNull(ordinal) ? 0.0m : reader.GetDecimal(ordinal);

                        ordinal = reader.GetOrdinal("TempdbPFSWaitTimeMilliseconds");
                        tempDBStats.TempdbPFSWaitTimeMilliseconds = (long)GetValueOrDefault<long>(reader["TempdbPFSWaitTimeMilliseconds"]);

                        ordinal = reader.GetOrdinal("TempdbGAMWaitTimeMilliseconds");
                        tempDBStats.TempdbGAMWaitTimeMilliseconds = (long)GetValueOrDefault<long>(reader["TempdbGAMWaitTimeMilliseconds"]);

                        ordinal = reader.GetOrdinal("TempdbSGAMWaitTimeMilliseconds");
                        tempDBStats.TempdbSGAMWaitTimeMilliseconds = (long)GetValueOrDefault<long>(reader["TempdbSGAMWaitTimeMilliseconds"]);

                        ordinal = reader.GetOrdinal("VersionStoreGenerationKilobytesPerSec");
                        tempDBStats.VersionStoreGenerationKilobytesPerSec = reader.IsDBNull(ordinal) ? 0.0 : reader.GetDouble(ordinal);

                        ordinal = reader.GetOrdinal("VersionStoreCleanupKilobytesPerSec");
                        tempDBStats.VersionStoreCleanupKilobytesPerSec = reader.IsDBNull(ordinal) ? 0.0 : reader.GetDouble(ordinal);

                        result.Add(tempDBStats);
                    }

                    return result;
                }
            }
        }

        /*internal static IList<DC.Databases.AvailabilityGroupStatsForDatabase> GeAvailabilityGroupStatsForInstance(SqlConnectionInfo sqlConnectionInfo, int passedinstanceId, int NumHistoryMinutes)
        {
            if (sqlConnectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, 
                    GetInstanceAvailbilityGroupDetailsStoredProcedure, passedinstanceId, NumHistoryMinutes))
                {
                    IList<DC.Databases.AvailabilityGroupStatsForDatabase> result = new List<DC.Databases.AvailabilityGroupStatsForDatabase>();

                    while (reader.Read())
                    {
                        DC.Databases.AvailabilityGroupStatsForDatabase availabilityGrp = new DC.Databases.AvailabilityGroupStatsForDatabase();
                        availabilityGrp.DatabaseName = GetValueOrDefault<string>(reader["DatabaseName"]).ToString();                        
                        availabilityGrp.LogSendQueueSize = (double)GetValueOrDefault<double>(reader["LogSendQueue"]);
                        availabilityGrp.LogTransferRate = (double)GetValueOrDefault<double>(reader["LogSendRate"]);
                        availabilityGrp.RedoQueueSize = (double)GetValueOrDefault<double>(reader["RedoQueue"]);
                        availabilityGrp.RedoTransferRate = (double)GetValueOrDefault<double>(reader["RedoRate"]);
                        availabilityGrp.UTCCollectionDateTime = (DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"]);
                        result.Add(availabilityGrp);
                    }

                    return result;
                }
            }
        }*/


        internal static IList<DC.Databases.AvailabilityGroupSummaryForDatabase> GetAvailabilityGroupForInstance(SqlConnectionInfo sqlConnectionInfo, int passedinstanceId, string timeZoneOffset, int NumHistoryMinutes = 0, DateTime endDate = default(DateTime))
        {
            IDictionary<int, string> RepRolesMap = new Dictionary<int, string>();
            RepRolesMap.Add(0, "Resolving");
            RepRolesMap.Add(1, "Primary");
            RepRolesMap.Add(2, "Secondary");
            RepRolesMap.Add(255, "None");

            if (sqlConnectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                DateTime utcCollectionDateTime;
                if (!endDate.Equals(default(DateTime)))
                    utcCollectionDateTime = DateTimeHelper.ConvertToUTC(endDate, timeZoneOffset);
                else
                    utcCollectionDateTime = DateTime.UtcNow;
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetInstanceAvailbilityGroupDetailsStoredProcedure, passedinstanceId, NumHistoryMinutes, utcCollectionDateTime))
                {
                    IList<DC.Databases.AvailabilityGroupSummaryForDatabase> resultList = new List<DC.Databases.AvailabilityGroupSummaryForDatabase>();

                    while (reader.Read())
                    {
                        DC.Databases.AvailabilityGroupSummaryForDatabase result = new DC.Databases.AvailabilityGroupSummaryForDatabase();

                        //result.DatabaseName = String.Empty;//GetValueOrDefault<string>(reader["DatabaseName"]).ToString();                        
                        result.DatabaseStatus = ((DatabaseStatusNoMaskBit)Enum.ToObject(typeof(DatabaseStatusNoMaskBit), (byte)GetValueOrDefault<byte>(reader["DatabaseState"]))).ToString();
                        result.GroupName = GetValueOrDefault<string>(reader["GroupName"]).ToString();
                        result.LogSendQueueSize = (double)GetValueOrDefault<double>(reader["LogSendQueue"]);
                        result.LogTransferRate = (double)GetValueOrDefault<double>(reader["LogSendRate"]);
                        result.RedoQueueSize = (double)GetValueOrDefault<double>(reader["RedoQueue"]);
                        result.RedoTransferRate = (double)GetValueOrDefault<double>(reader["RedoRate"]);
                        result.ReplicaName = GetValueOrDefault<string>(reader["ReplicaName"]).ToString();

                        //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                        result.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                        int replicaRole = Convert.ToInt32(GetValueOrDefault<int>(reader["ReplicaRole"]));
                        string repRole = "None";
                        result.ReplicaRole = repRole;
                        RepRolesMap.TryGetValue(replicaRole, out repRole);
                        if (repRole != null)
                        {
                            result.ReplicaRole = repRole;
                        }
                        //Ashu: dont know why but Enum.TryParse is horribly slow. Converting to dict lookup
                        /*if (Enum.TryParse<ReplicaRole>(replicaRole.ToString(), out repRole))
                        {
                            result.ReplicaRole = repRole.GetDescription();
                        }
                        else 
                        {
                            result.ReplicaRole = ReplicaRole.None.GetDescription();
                        }*/

                        result.SyncHealth = ((AlwaysOnSynchronizationHealth)Enum.ToObject(typeof(AlwaysOnSynchronizationHealth), (byte)GetValueOrDefault<byte>(reader["SynchronizationHealth"]))).ToString();
                        resultList.Add(result);
                    }

                    return resultList;
                }
            }
        }


        //SQLdm 8.5 (Ankit Srivastava): for  Categories API-  Session Statistics
        /*internal static IList<DC.Category.Sessions.ServerSessionStatistics> GetSessionStatisticsForInstance2(SqlConnectionInfo connectionInfo, int passedinstanceId, int NumHistoryMinutes)
        {

            if (connectionInfo == null)
            {
                return null;
            }
            var sessionStatistics = new List<DC.Category.Sessions.ServerSessionStatistics>();
            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetAllServersSummaryStoredProcedure, passedinstanceId, null, NumHistoryMinutes))
                {
                    DC.ServerSummaryContainer container = new DC.ServerSummaryContainer();

                    dataReader.Read();
                    dataReader.NextResult();
                    var OverviewStatistics = GetTable(dataReader);

                    foreach (DataRow row in OverviewStatistics.Rows)
                    {
                        ServerOverview overview = new ServerOverview((string)row["InstanceName"], row, new string[0], new DataRow[0]);
                        var sessionStatistic = new DC.Category.Sessions.ServerSessionStatistics(overview,Convert.ToDateTime(row["CollectionDateTime"]));
                        sessionStatistics.Add(sessionStatistic);
                    }

                    return sessionStatistics;
                }
            }
        }*/

        internal static IList<DC.Category.Sessions.SessionsForInstance> GetSessionsForInstance(SqlConnectionInfo connectionInfo, int passedinstanceId, string timeZoneOffset, int limit, bool UserSessionsOnly = true, bool excludeSQLDMSessions = true)
        {
            if (connectionInfo == null)
            {
                return null;
            }
            //var sessionStatistics = new List<DC.Category.Sessions.ServerSessionStatistics>();
            IList<DC.Category.Sessions.SessionsForInstance> sessionStatistics = new List<DC.Category.Sessions.SessionsForInstance>();
            IList<DC.Category.Sessions.SessionsForInstance> finalResult = new List<DC.Category.Sessions.SessionsForInstance>();
            using (SqlConnection connection =
                    connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetSessionStatisticsForWebConsoleStoredProcedure, passedinstanceId, limit, null))
                {
                    //DC.ServerSummaryContainer container = new DC.ServerSummaryContainer();

                    if (dataReader.Read())
                    {
                        var SessionListOrdinal = dataReader.GetOrdinal("SessionList");
                        int collectionDateTimeOrdinal = dataReader.GetOrdinal("UTCCollectionDateTime");
                        if (!dataReader.IsDBNull(SessionListOrdinal) && !dataReader.IsDBNull(collectionDateTimeOrdinal))
                        {
                            //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                            DateTime utcCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>((DateTime)dataReader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                            SessionSnapshot ss = new SessionSnapshot(utcCollectionDateTime,
                                                        Serialized<object>.DeserializeCompressed
                                                            <Dictionary<Pair<int?, DateTime?>, Session>>(
                                                                (byte[])dataReader["SessionList"]
                                                            ));
                            sessionStatistics = ConvertToDataContract.ToDC(ss, timeZoneOffset);
                        }
                    }

                    foreach (DC.Category.Sessions.SessionsForInstance sfi in sessionStatistics)
                    {
                        //SQLDM 10.2.2 (Tushar)--Fix for SQLDM-28094--Populating application name field of session when it is an internal session.
                        // Making rest service response similar to desktop client for internal session case.
                        if (sfi.connection != null && !sfi.connection.IsUserSession)
                        {
                            if (!String.IsNullOrEmpty(sfi.connection.Command))
                                sfi.connection.Application = "(SYSTEM: " + sfi.connection.Command + ")";
                            else
                                sfi.connection.Application = "(SYSTEM)";
                        }

                        //SQLDM 10.2.2 (Tushar)--Fix for SQLDM-28094--making sure connection property is not null before accessing it.
                        bool add = (UserSessionsOnly && sfi.connection != null && sfi.connection.IsUserSession) || (!UserSessionsOnly);
                        if (add)
                        {
                            if (excludeSQLDMSessions)
                            {
                                //SQLDM 10.2.2 (Tushar)--Fix for SQLDM-28094--making sure application property of connection property is not null before accessing it.
                                if (sfi.connection.Application != null
                                    && sfi.connection.Application.StartsWith("SQL diagnostic manager", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    add = false;
                                }
                            }
                        }

                        if (add)
                        {
                            finalResult.Add(sfi);
                        }
                    }
                }
            }
            return finalResult;
        }
        //internal static IList<DC.Category.Sessions.ServerSessionStatistics> GetSessionStatisticsForInstance(SqlConnectionInfo connectionInfo, int passedinstanceId, int NumHistoryMinutes)
        internal static IList<DC.Category.Sessions.ServerSessionStatistics> GetSessionStatisticsForInstance(SqlConnectionInfo connectionInfo, int passedinstanceId, string timeZoneOffset, int NumHistoryMinutes, DateTime endDate)
        {

            if (connectionInfo == null)
            {
                return null;
            }
            //var sessionStatistics = new List<DC.Category.Sessions.ServerSessionStatistics>();
            DateTime utcCollectionEndTime;
            if (!endDate.Equals(default(DateTime)))
                utcCollectionEndTime = DateTimeHelper.ConvertToUTC(endDate, timeZoneOffset);
            else
                utcCollectionEndTime = DateTime.UtcNow;
            var sessionStatistics = new List<DC.Category.Sessions.ServerSessionStatistics>();
            using (
                SqlConnection connection =
                    connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (
                    SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(connection, GetSessionStatisticsForWebConsoleStoredProcedure, passedinstanceId, NumHistoryMinutes, utcCollectionEndTime))
                {
                    //DC.ServerSummaryContainer container = new DC.ServerSummaryContainer();
                    while (dataReader.Read())
                    {
                        var SessionListOrdinal = dataReader.GetOrdinal("SessionList");
                        int collectionDateTimeOrdinal = dataReader.GetOrdinal("UTCCollectionDateTime");
                        if (!dataReader.IsDBNull(SessionListOrdinal) && !dataReader.IsDBNull(collectionDateTimeOrdinal))
                        {
                            //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                            DateTime utcCollectionTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(dataReader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                            SessionSnapshot ss = new SessionSnapshot(utcCollectionTime,
                                                        Serialized<object>.DeserializeCompressed
                                                            <Dictionary<Pair<int?, DateTime?>, Session>>(
                                                                (byte[])dataReader["SessionList"]
                                                            ));

                            int userProcessOrdinal = dataReader.GetOrdinal("UserProcesses");
                            int numUserSessions = 0;
                            if (!dataReader.IsDBNull(userProcessOrdinal))
                            {
                                numUserSessions = (int)dataReader["UserProcesses"];
                            }

                            var sss = new DC.Category.Sessions.ServerSessionStatistics(ss, utcCollectionTime, timeZoneOffset);
                            sss.ResponseTime = (Int32)dataReader["ResponseTimeInMilliseconds"];
                            sss.IdleSessionCount = numUserSessions - sss.ActiveSessionCount;

                            if (!dataReader.IsDBNull(dataReader.GetOrdinal("LockStatistics")))
                            {
                                byte[] lockBytes = (byte[])dataReader["LockStatistics"];
                                LockStatistics lockCounters = Serialized<Object>.DeserializeCompressed<LockStatistics>(lockBytes);
                                if (lockCounters.TotalCounters.Deadlocks != null)
                                {
                                    sss.TotalDeadLock = lockCounters.TotalCounters.Deadlocks.Value;
                                }
                            }
                            sessionStatistics.Add(sss);
                        }
                    }

                    /*var OverviewStatistics = GetTable(dataReader);

                    foreach (DataRow row in OverviewStatistics.Rows)
                    {
                        ServerOverview overview = new ServerOverview((string)row["InstanceName"], row, new string[0], new DataRow[0]);
                        var sessionStatistic = new DC.Category.Sessions.ServerSessionStatistics(overview, Convert.ToDateTime(row["CollectionDateTime"]));
                        sessionStatistics.Add(sessionStatistic);
                    }*/

                    return sessionStatistics;
                }
            }
        }

        internal static IList<DC.Category.QueryWaitStatisticsForInstance> GetQueryWaitStatisticsForInstance(SqlConnectionInfo sqlConnectionInfo, int instanceId, string timeZoneOffset, DateTime? startDateTimeInUTC, DateTime? endDateTimeInUTC, int? waitTypeId, int? waitCategoryId, int? sqlStatementId, int? applicationId, int? databaseId, int? hostId, int? sessionId, int? loginId)
        {
            if (sqlConnectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                IList<DC.Category.QueryWaitStatisticsForInstance> resultList = new List<DC.Category.QueryWaitStatisticsForInstance>();
                DC.Category.QueryWaitStatisticsForInstance result;

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetQueryWaitStatisticsStoredProcedure,
                    instanceId, endDateTimeInUTC, null, null, waitTypeId, waitCategoryId, sqlStatementId, applicationId, databaseId, hostId, sessionId, loginId, startDateTimeInUTC))
                {
                    while (reader.Read())
                    {
                        result = new DC.Category.QueryWaitStatisticsForInstance();
                        var ordinal = reader.GetOrdinal("StatementUTCStartTime");
                        if (!reader.IsDBNull(ordinal))
                        {
                            result.StatementUTCStartTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["StatementUTCStartTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time
                        }

                        //START SQLdm 10.0 (Sanjali Makkar): To add filters respective to IDs of different parameters
                        result.WaitTypeID = (int)GetValueOrDefault<string>(reader["WaitTypeID"]);
                        result.WaitType = (string)GetValueOrDefault<string>(reader["WaitType"]);
                        result.WaitCategoryID = (int)GetValueOrDefault<string>(reader["CategoryID"]);
                        result.WaitCategory = (string)GetValueOrDefault<string>(reader["Category"]);

                        result.WaitDurationPerSecond = (decimal)GetValueOrDefault<decimal>(reader["WaitDuration"]);
                        ordinal = reader.GetOrdinal("SessionID");
                        if (!reader.IsDBNull(ordinal))
                        {
                            result.SessionID = reader.GetInt16(ordinal);
                        }

                        result.HostID = (int)GetValueOrDefault<string>(reader["HostNameID"]);
                        result.HostName = (string)GetValueOrDefault<string>(reader["HostName"]);
                        result.ApplicationID = (int)GetValueOrDefault<string>(reader["ApplicationNameID"]);
                        result.ApplicationName = (string)GetValueOrDefault<string>(reader["ApplicationName"]);
                        result.LoginID = (int)GetValueOrDefault<string>(reader["LoginNameID"]);
                        result.LoginName = (string)GetValueOrDefault<string>(reader["LoginName"]);
                        result.DatabaseID = (int)GetValueOrDefault<string>(reader["DatabaseID"]);
                        result.DatabaseName = (string)GetValueOrDefault<string>(reader["DatabaseName"]);
                        result.SQLStatementID = (int)GetValueOrDefault<string>(reader["SQLStatementID"]);
                        result.SQLStatement = (string)GetValueOrDefault<string>(reader["SQLStatement"]);
                        resultList.Add(result);
                        //END SQLdm 10.0 (Sanjali Makkar): To add filters respective to IDs of different parameters
                    }
                }
                return resultList;

            }
        }
        /// <summary>
        /// SQLdm10.2(srishti purohit) - Gives clubbed data fetching logic for supplied duration for GetQueryWaitStatisticsForInstanceOverview API on instance overview page in web UI
        /// SQLdm-28027 defect fix 
        /// </summary>
        /// <param name="sqlConnectionInfo"></param>
        /// <param name="instanceId"></param>
        /// <param name="timeZoneOffset"></param>
        /// <param name="startDateTimeInUTC"></param>
        /// <param name="endDateTimeInUTC"></param>
        /// <returns></returns>
        internal static IList<DC.Category.QueryWaitStatisticsForInstanceOverview> GetQueryWaitStatisticsForInstanceOverview(SqlConnectionInfo sqlConnectionInfo, int instanceId, string timeZoneOffset, DateTime? startDateTimeInUTC, DateTime? endDateTimeInUTC)
        {
            if (sqlConnectionInfo == null)
            {
                return null;
            }
            IList<DC.Category.QueryWaitStatisticsForInstanceOverview> resultList = new List<DC.Category.QueryWaitStatisticsForInstanceOverview>();
            DC.Category.QueryWaitStatisticsForInstanceOverview result;
            DateTime startUTC;
            DateTime endUTC;
            //endDateTimeInUTC should never be null as web UI always supply start and end time. so control will never go inside if.
            if (endDateTimeInUTC == null)
            {
                using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();

                    using (connection)
                    {
                        endUTC = (DateTime)GetValueOrDefault<DateTime>(SqlHelper.ExecuteScalar(connection, CommandType.Text, GetLatestStatisticCollectionTime + instanceId));
                    }
                }
            }
            else
                endUTC = (DateTime)endDateTimeInUTC;
            //startDateTimeInUTC should never be null as web UI always supply start and end time. so control will never go inside if.
            if (startDateTimeInUTC == null)
            {
                startUTC = ((DateTime)endUTC).AddMinutes(-30);
            }
            else
                startUTC = (DateTime)startDateTimeInUTC;
            long expectedTimeSeconds = TimeFrameUtils.GetTimeSeconds(startUTC);
            long expectedEndTimeSeconds = TimeFrameUtils.GetTimeSeconds(endUTC);

            //Calculate summary level according to expect start and end time slot
            SummaryLevel summaryLevel = new SummaryLevel();
            summaryLevel = TimeFrameUtils.GetSummaryLevel(expectedTimeSeconds, expectedEndTimeSeconds);
            using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetQueryWaitOverviewStoredProcedure,
                    instanceId, endUTC, null, startUTC, summaryLevel))
                {
                    while (reader.Read())
                    {
                        result = new DC.Category.QueryWaitStatisticsForInstanceOverview();
                        var ordinal = reader.GetOrdinal("StatementUTCStartTime");
                        if (!reader.IsDBNull(ordinal))
                        {
                            result.StatementUTCStartTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["StatementUTCStartTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time
                        }

                        result.WaitCategoryID = (int)GetValueOrDefault<string>(reader["CategoryID"]);
                        result.WaitCategory = (string)GetValueOrDefault<string>(reader["Category"]);

                        result.WaitDurationPerSecond = (decimal)GetValueOrDefault<decimal>(reader["WaitDuration"]);

                        resultList.Add(result);
                    }
                }
            }
            //SQLdm 10.2(srishti purohit) -- Fix for SQLDM-28027 : fix for only this API as this is a bar graph API on instance overview page
            if (resultList.Count > 0)
            {
                fillHoles(expectedTimeSeconds, ref resultList, expectedEndTimeSeconds, timeZoneOffset, summaryLevel);
            }
            return resultList;
        }
        /// <summary>
        /// Function will fill gaps in start , end and between if data is not there in a calculated time range (ie summary level)
        /// </summary>
        /// <param name="startDateTimeInUTC"></param>
        /// <param name="resultList"></param>
        /// <param name="endDateTimeInUTC"></param>
        /// <returns></returns>
        private static void fillHoles(long expectedTimeSeconds, ref IList<DC.Category.QueryWaitStatisticsForInstanceOverview> resultList, long expectedEndTimeSeconds, string timeZoneOffset, SummaryLevel summaryLevel)
        {
            try
            {
                long startTimeSeconds = TimeFrameUtils.GetTimeSeconds(resultList[0].StatementUTCStartTime);
                long endTimeSeconds = TimeFrameUtils.GetTimeSeconds(resultList[resultList.Count - 1].StatementUTCStartTime);
                DC.Category.QueryWaitStatisticsForInstanceOverview result;
                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                //adding start and end default points if they are already not present
                if (expectedTimeSeconds != startTimeSeconds)
                {
                    result = new DC.Category.QueryWaitStatisticsForInstanceOverview();
                    result.StatementUTCStartTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind((start.AddSeconds(expectedTimeSeconds)), DateTimeKind.Utc)), timeZoneOffset);
                    resultList.Insert(0, result);
                }
                if (expectedEndTimeSeconds != endTimeSeconds)
                {
                    result = new DC.Category.QueryWaitStatisticsForInstanceOverview();
                    result.StatementUTCStartTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind((start.AddSeconds(expectedEndTimeSeconds)), DateTimeKind.Utc)), timeZoneOffset);
                    resultList.Insert(resultList.Count - 1, result);
                }
                fillHolesBetween(ref resultList, summaryLevel);
            }
            catch (Exception ex)
            {
                Log.Error("Exxception while manipulating data for QueryWaits.", ex);
            }
        }
        /// <summary>
        /// Using summaryLevel we will insert default rows after time interval
        /// </summary>
        /// <param name="resultList"></param>
        private static void fillHolesBetween(ref IList<DC.Category.QueryWaitStatisticsForInstanceOverview> resultList, SummaryLevel summaryLevel)
        {
            IList<DC.Category.QueryWaitStatisticsForInstanceOverview> subResultList = new List<DC.Category.QueryWaitStatisticsForInstanceOverview>();
            DateTime previousTime;
            DateTime currentTime = resultList[0].StatementUTCStartTime;
            foreach (DC.Category.QueryWaitStatisticsForInstanceOverview QWSI in resultList)
            {
                previousTime = currentTime;
                currentTime = QWSI.StatementUTCStartTime;
                long previousLong = TimeFrameUtils.GetTimeSeconds(previousTime);
                long currentLong = TimeFrameUtils.GetTimeSeconds(currentTime);
                if (currentLong - previousLong > (int)summaryLevel)
                    subResultList = subResultList.Concat(fillHolesAtSlots(previousLong, currentLong, summaryLevel)).ToList();
            }
            resultList = resultList.Concat(subResultList).OrderBy(li => li.StatementUTCStartTime).ToList();
        }
        /// <summary>
        /// Filling the slot dont have any entry in supplied range
        /// </summary>
        /// <param name="expectedTimeSeconds"></param>
        /// <param name="startTimeSeconds"></param>
        /// <param name="summaryLevel"></param>
        /// <returns></returns>
        private static IList<DC.Category.QueryWaitStatisticsForInstanceOverview> fillHolesAtSlots(long expectedTimeSeconds, long startTimeSeconds, SummaryLevel summaryLevel)
        {
            DC.Category.QueryWaitStatisticsForInstanceOverview resultStart;
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            IList<DC.Category.QueryWaitStatisticsForInstanceOverview> subResultList = new List<DC.Category.QueryWaitStatisticsForInstanceOverview>();

            for (long holeTimeSeconds = expectedTimeSeconds; holeTimeSeconds < startTimeSeconds; holeTimeSeconds += (int)summaryLevel)
            {
                resultStart = new DC.Category.QueryWaitStatisticsForInstanceOverview();
                resultStart.StatementUTCStartTime = start.AddSeconds(holeTimeSeconds);
                subResultList.Add(resultStart);
            }
            return subResultList;
        }
        #endregion

        #region License

        public static LicenseSummary GetLicenseKeys(SqlConnectionInfo connectionInfo)
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

                    SqlParameter repository = command.Parameters["@ReturnInstanceName"];
                    SqlString strValue = (SqlString)repository.SqlValue;
                    repositoryInstance = strValue.Value;
                } // using command
            }

            return LicenseSummary.SummarizeKeys(registeredServers, repositoryInstance, keys);
        }

        #endregion

        #region Auth
        //GetWebApplicationUserDetailsStoredProcedure
        public static bool IsApplicationSecurityEnabled(SqlConnectionInfo connectionInfo)
        {
            using (Log.InfoCall("Entering IsApplicationSecurityEnabled"))
            {
                connectionInfo = CheckConnectionInfo(connectionInfo);

                bool _isSecEnabled = false;

                using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, GetSecurityEnabledStoredProcedure))
                    {
                        int appSecFlag = (int)command.ExecuteScalar();
                        _isSecEnabled = appSecFlag == 1 ? true : false;
                    }
                }

                Log.Info("Using connection info: ", connectionInfo.ToString(), ":: app security is ", _isSecEnabled.ToString()); //logging if the security is enabled and the connection info

                return _isSecEnabled;
            }
        }
        public static AuthDataContract.WebApplicationUser GetWebApplicationUser(SqlConnectionInfo connectionInfo, string Name)
        {

            connectionInfo = CheckConnectionInfo(connectionInfo);
            Log.Debug("Using connection info: ", connectionInfo);

            AuthDataContract.WebApplicationUser _webAppUser = null;

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetWebApplicationUserDetailsStoredProcedure, Name))
                {
                    while (reader.Read())
                    {
                        _webAppUser = new AuthDataContract.WebApplicationUser()
                        {
                            Name = (string)GetValueOrDefault<string>(reader["Name"]),
                            Type = (string)GetValueOrDefault<string>(reader["Type"]),
                            Type_Desc = (string)GetValueOrDefault<string>(reader["Type_Desc"]),
                            Sid = new System.Security.Principal.SecurityIdentifier((byte[])GetValueOrDefault<byte[]>(reader["sid"]), 0),
                            IsSysAdmin = (int)GetValueOrDefault<int>(reader["IsSysAdmin"]) == 1 ? true : false
                        };

                    }

                }
            }
            return _webAppUser;
        }

        internal static bool AuthenticateUserFromSQLForEnabledSecurity(string user, string password, AuthType authType)
        {
            using (Log.InfoCall("Entering AuthenticateUserFromSQLForEnabledSecurity"))
            {
                bool _retValue = false;
                var connectionString = BuildConnectionString(RestServiceConfiguration.SQLConnectInfo);
                var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

                if (authType == AuthType.Sql)
                {
                    connectionStringBuilder.IntegratedSecurity = false;
                    connectionStringBuilder.UserID = user;
                    connectionStringBuilder.Password = password;
                }
                else
                {
                    connectionStringBuilder.IntegratedSecurity = true;
                }
                connectionStringBuilder.PersistSecurityInfo = false;
                connectionStringBuilder.Pooling = false;

                using (var connection = new SqlConnection(connectionStringBuilder.ToString()))
                {
                    connection.Open();
                    try
                    {
                        using (var cmd = connection.CreateCommand())
                        {
                            int isAuthentic = 0;
                            cmd.CommandText = AuthenticateWebConsoleUserAppSecOnStoredProcedure;
                            cmd.CommandType = CommandType.StoredProcedure;

                            isAuthentic = Convert.ToInt32(cmd.ExecuteScalar());


                            _retValue = isAuthentic == 1 ? true : false;
                        }
                    }
                    catch (Exception ex_auth_enabled_security)
                    {
                        Log.Info("accessing auth proc threw an error: ", ex_auth_enabled_security.Message, ex_auth_enabled_security.InnerException != null ? ex_auth_enabled_security.InnerException.Message : string.Empty);
                    }
                    finally
                    {
                        if (connection.State != ConnectionState.Closed) connection.Close();
                    }

                }
                Log.Info("The user auth is ", _retValue.ToString());

                return _retValue;
            }
        }
        internal static bool AuthenticateUserFromSQLForDisabledSecurity(string user, string password, AuthType authType)
        {
            using (Log.InfoCall("Entering AuthenticateUserFromSQLForDisabledSecurity"))
            {

                bool _retValue;
                var connectionString = BuildConnectionString(RestServiceConfiguration.SQLConnectInfo);
                var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

                if (authType == AuthType.Sql)
                {
                    connectionStringBuilder.IntegratedSecurity = false;
                    connectionStringBuilder.UserID = user;
                    connectionStringBuilder.Password = password;
                }
                else
                {
                    connectionStringBuilder.IntegratedSecurity = true;
                }
                connectionStringBuilder.PersistSecurityInfo = false;
                connectionStringBuilder.Pooling = false;

                using (var connection = new SqlConnection(connectionStringBuilder.ToString()))
                {
                    try
                    {
                        //this code just checks if the user is able to connect to the repo db
                        connection.Open();


                        _retValue = true;
                    }
                    catch (Exception ex_auth_disabled_security)
                    {
                        Log.Info("accessing auth proc threw an error: ", ex_auth_disabled_security.Message, ex_auth_disabled_security.InnerException != null ? ex_auth_disabled_security.InnerException.Message : string.Empty);
                        _retValue = false;
                    }
                    finally
                    {
                        if (connection.State != ConnectionState.Closed)
                        {
                            connection.Close();
                        }
                    }
                }
                return _retValue;
            }
        }


        private static string BuildConnectionString(SqlConnectionInfo connectionInfo)
        {
            using (Log.InfoCall("Entering BuildConnectionString"))
            {
                StringBuilder stringBuilder = new StringBuilder();

                // Data Source
                if (!String.IsNullOrWhiteSpace(connectionInfo.InstanceName))
                    stringBuilder.AppendFormat("Data Source = '{0}';", GeneralHelper.EscapeQuotes(String.IsNullOrWhiteSpace(connectionInfo.InstanceName) ? string.Empty : connectionInfo.InstanceName));

                // Database
                if (connectionInfo.DatabaseName.Trim().Length > 0)
                    stringBuilder.AppendFormat("Database = '{0}';", GeneralHelper.EscapeQuotes(String.IsNullOrWhiteSpace(connectionInfo.DatabaseName) ? string.Empty : connectionInfo.DatabaseName));

                // Authentication
                if (connectionInfo.UseIntegratedSecurity == true)
                {
                    stringBuilder.Append("Integrated Security = 'SSPI';");
                }
                else if (connectionInfo.UseIntegratedSecurity == false)
                {
                    stringBuilder.AppendFormat("User ID = '{0}';", GeneralHelper.EscapeQuotes(String.IsNullOrWhiteSpace(connectionInfo.UserName) ? String.Empty : connectionInfo.UserName));
                    stringBuilder.AppendFormat("Password = '{0}';", GeneralHelper.EscapeQuotes(String.IsNullOrWhiteSpace(connectionInfo.Password) ? string.Empty : connectionInfo.Password));
                }

                // Application Name
                stringBuilder.AppendFormat("Application Name = '{0}';", GeneralHelper.EscapeQuotes(connectionInfo.ApplicationName));

                // Timeout
                stringBuilder.AppendFormat("Connection Timeout = '{0}';", connectionInfo.ConnectionTimeout);

                // Packet size
                //stringBuilder.AppendFormat("Packet Size = '{0}';", connectionInfo.PacketSize);
                Log.Info("The query string that got build was " + stringBuilder != null ? stringBuilder.ToString() : string.Empty);
                return stringBuilder.ToString();
            }
        }

        #endregion

        #region QueryManager

        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - return a list of applications for a particular instance
        public static List<DC.Application> GetApplicationsForInstance(SqlConnectionInfo connectionInfo, int sqlServerID, int recordStartIndex, int recordsCount)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<DC.Application> appList = new List<DC.Application>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetApplicationsForServer, sqlServerID, recordStartIndex, recordsCount))
                {
                    while (dataReader.Read())
                    {
                        int ordinal = dataReader.GetOrdinal("ApplicationNameID");
                        if (!(dataReader.IsDBNull(ordinal)))
                        {
                            DC.Application application = new DC.Application();
                            application.ApplicationNameID = dataReader.GetInt32(ordinal);
                            application.ApplicationName = GetValueOrDefault<string>(dataReader["ApplicationName"]).ToString();
                            appList.Add(application);
                        }
                    }
                }
                return appList;
            }
        }


        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - return a list of clients for a particular instance
        public static List<DC.Client> GetClientsForInstance(SqlConnectionInfo connectionInfo, int sqlServerID, int recordStartIndex, int recordsCount)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<DC.Client> clientList = new List<DC.Client>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetClientsForServer, sqlServerID, recordStartIndex, recordsCount))
                {
                    while (dataReader.Read())
                    {
                        int ordinal = dataReader.GetOrdinal("ClientID");
                        if (!(dataReader.IsDBNull(ordinal)))
                        {
                            DC.Client client = new DC.Client();
                            client.ClientID = dataReader.GetInt32(ordinal);
                            client.ClientName = GetValueOrDefault<string>(dataReader["ClientName"]).ToString();
                            clientList.Add(client);
                        }
                    }
                }
                return clientList;
            }
        }


        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - return a list of users for a particular instance
        public static List<DC.User> GetUsersForInstance(SqlConnectionInfo connectionInfo, int sqlServerID, int recordStartIndex, int recordsCount)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<DC.User> userList = new List<DC.User>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetUsersForServer, sqlServerID, recordStartIndex, recordsCount))
                {
                    while (dataReader.Read())
                    {
                        int ordinal = dataReader.GetOrdinal("UserID");
                        if (!(dataReader.IsDBNull(ordinal)))
                        {
                            DC.User user = new DC.User();
                            user.UserID = dataReader.GetInt32(ordinal);
                            user.UserName = GetValueOrDefault<string>(dataReader["UserName"]).ToString();
                            userList.Add(user);
                        }
                    }
                }
                return userList;
            }
        }


        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - return a list of databases for a particular instance
        public static List<DC.DatabaseInformation> GetDatabasesForInstance(SqlConnectionInfo connectionInfo, int sqlServerID, int recordStartIndex, int recordsCount)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<DC.DatabaseInformation> databaseList = new List<DC.DatabaseInformation>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetDatabasesForServer, sqlServerID, recordStartIndex, recordsCount, 0))
                {
                    while (dataReader.Read())
                    {
                        int ordinal = dataReader.GetOrdinal("DatabaseID");
                        if (!(dataReader.IsDBNull(ordinal)))
                        {
                            DC.DatabaseInformation databaseInfo = new DC.DatabaseInformation();
                            databaseInfo.DatabaseID = dataReader.GetInt32(ordinal);
                            databaseInfo.DatabaseName = GetValueOrDefault<string>(dataReader["DatabaseName"]).ToString();
                            databaseInfo.IsSystemDatabase = (bool)GetValueOrDefault<bool>(dataReader["IsSystemDatabase"]);
                            databaseList.Add(databaseInfo);
                        }
                    }
                }
                return databaseList;
            }
        }


        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - return a list of Query plans for a QueryStatisticsID
        public static List<DC.QueryPlan> GetQueryPlans(SqlConnectionInfo connectionInfo, int sqlServerID, int queryStatisticsID)//SQLdm 9.0 (Ankit Srivastava) - Query Plan View - changing the variable name to statisticsid
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<DC.QueryPlan> queryPlanList = new List<DC.QueryPlan>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetQueryPlan, queryStatisticsID))//SQLdm 9.0 (Ankit Srivastava) - Query Plan View - changing the variable name to statisticsid
                {
                    while (dataReader.Read())
                    {
                        int ordinal = dataReader.GetOrdinal("SQLStatementID");
                        if (!(dataReader.IsDBNull(ordinal)))
                        {
                            DC.QueryPlan queryPlan = new DC.QueryPlan();
                            List<DC.SQLQueryColumns> queryColumnsList = new List<DC.SQLQueryColumns>();
                            string encodedPlanXML = null, decodedPlanXML = null;
                            queryPlan.InstanceID = sqlServerID;        // anticipatory parameter, no use at the moment
                            queryPlan.PlanID = (int)GetValueOrDefault<int>(dataReader["PlanID"]);
                            queryPlan.SQLStatementID = dataReader.GetInt32(ordinal);

                            //SQLdm 10.0 (Tarun Sapra) - Display Estimated Query Plan
                            ordinal = dataReader.GetOrdinal("IsActualPlan");

                            if (!dataReader.IsDBNull(ordinal))
                            {
                                queryPlan.IsActualPlan = (bool)GetValueOrDefault<bool>(dataReader[ordinal]);
                            }



                            encodedPlanXML = GetValueOrDefault<string>(dataReader["PlanXML"]).ToString();

                            decodedPlanXML = ObjectHelper.DecompressString(encodedPlanXML);

                            XmlDocument xml = new XmlDocument();         // Parse the query columns to be shown in model window
                            xml.LoadXml(decodedPlanXML);

                            XmlNodeList outputListColumnReference = xml.GetElementsByTagName("ColumnReference");
                            List<string> derivedColumns = new List<string>();

                            foreach (XmlNode node in outputListColumnReference)
                            {
                                string database = "N/A";
                                string schema = "N/A";
                                string table = "N/A";
                                string column = "N/A";
                                bool isEntryDistinct = false;

                                if (node.Attributes.GetNamedItem("Column") != null)
                                {
                                    column = node.Attributes.GetNamedItem("Column").InnerText;
                                    if (column.StartsWith("Expr") && column.Length == 8)     // ignore all other columns if "Column" is like "ExprXXXX"
                                    {
                                        var isObjectPresent = derivedColumns.FirstOrDefault(i => (i.Equals(column)));
                                        if (isObjectPresent == null)
                                        {
                                            column = "Derived Column";            // replace the "ExprXXXX" with "Derived Column"
                                            isEntryDistinct = true;
                                            derivedColumns.Add(column);
                                        }
                                    }
                                    else if ((node.Attributes.Count >= 4) && (node.Attributes.GetNamedItem("Database") != null) &&
                                        (node.Attributes.GetNamedItem("Schema") != null) && (node.Attributes.GetNamedItem("Table") != null))
                                    {
                                        database = node.Attributes.GetNamedItem("Database").InnerText;
                                        schema = node.Attributes.GetNamedItem("Schema").InnerText;
                                        table = node.Attributes.GetNamedItem("Table").InnerText;

                                        var isObjectPresent = queryColumnsList.FirstOrDefault(i => (i.Database.Equals(database) &&
                                                                     i.Schema.Equals(schema) && i.Table.Equals(table) &&
                                                                     i.Column.Equals(column)));
                                        if (isObjectPresent == null)
                                            isEntryDistinct = true;
                                    }

                                    if (isEntryDistinct)
                                    {
                                        DC.SQLQueryColumns queryColumns = new DC.SQLQueryColumns();
                                        queryColumns.Database = database;
                                        queryColumns.Schema = schema;
                                        queryColumns.Table = table;
                                        queryColumns.Column = column;
                                        queryColumnsList.Add(queryColumns);
                                    }
                                }
                            }

                            queryPlan.QueryColumns = queryColumnsList;
                            // START : SQLdm 9.0 (Abhishek Joshi) - for escape XML sequence
                            queryPlan.PlanXML = WebUtility.HtmlEncode(decodedPlanXML);
                            // END : SQLdm 9.0 (Abhishek Joshi) - for escape XML sequence

                            queryPlanList.Add(queryPlan);
                        }
                    }
                }
                return queryPlanList;
            }
        }

        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - return query monitor statistics data grouped by application
        public static List<DC.QueryMonitorStatisticsData> GetQueryMonitorStatisticsByApplication(SqlConnectionInfo connectionInfo, int sqlServerID, float timeZoneOffset, string applicationIds, string databaseIds, string userIds, string clientIds, string sqlIncludeText, string sqlExcludeText,
                      int includeSQLStatement, int includeSQLProcedure, int includeSQLBatch, int includeIncompletedQueries, int includeTimeOverlappedQueries, DateTime? startTimestamp, DateTime? endTimestamp,
                         string sortBy, string sortOrder, int recordStartIndex, int recordsCount, int sqlSignatureID, int statementTypeID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<DC.QueryMonitorStatisticsData> applicationGroupDataList = new List<DC.QueryMonitorStatisticsData>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetQueryMonitorDataByApplication, sqlServerID, applicationIds, databaseIds, userIds, clientIds, sqlExcludeText, sqlIncludeText,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startTimestamp, endTimestamp, sortBy, sortOrder, recordStartIndex, recordsCount, sqlSignatureID, statementTypeID))
                {
                    while (dataReader.Read())
                    {
                        int ordinal = dataReader.GetOrdinal("ApplicationID");
                        if (!(dataReader.IsDBNull(ordinal)))
                        {
                            DC.QueryMonitorStatisticsData applicationData = new DC.QueryMonitorStatisticsData();

                            applicationData.ApplicationID = dataReader.GetInt32(ordinal);
                            applicationData.Application = GetValueOrDefault<string>(dataReader["Application"]).ToString();
                            applicationData.Occurrences = (int)GetValueOrDefault<int>(dataReader["Occurrences"]);
                            applicationData.TotalDuration = (Int64)GetValueOrDefault<Int64>(dataReader["TotalDuration"]);
                            applicationData.AvgDuration = GetValueOrDefault<string>(dataReader["AvgDuration"]).ToString();
                            applicationData.TotalCPUTime = (Int64)GetValueOrDefault<Int64>(dataReader["TotalCPUTime"]);
                            applicationData.TotalReads = (Int64)GetValueOrDefault<Int64>(dataReader["TotalReads"]);
                            applicationData.TotalWrites = (Int64)GetValueOrDefault<Int64>(dataReader["TotalWrites"]);
                            applicationData.TotalIO = (Int64)GetValueOrDefault<Int64>(dataReader["TotalIO"]);
                            applicationData.TotalWaitTime = (Int64)GetValueOrDefault<Int64>(dataReader["TotalWaitTime"]);
                            applicationData.MostRecentCompletion = ((DateTime)GetValueOrDefault<DateTime>(dataReader["MostRecentCompletion"])).AddHours(-timeZoneOffset).ToString("dd-MM-yyyy HH:mm:ss");
                            applicationData.TotalBlockingTime = (Int64)GetValueOrDefault<Int64>(dataReader["TotalBlockingTime"]);
                            applicationData.TotalDeadlocks = (Int64)GetValueOrDefault<Int64>(dataReader["TotalDeadlocks"]);
                            applicationData.AvgCPUTime = GetValueOrDefault<string>(dataReader["AvgCPUTime"]).ToString();
                            applicationData.AvgCPUPerSec = GetValueOrDefault<string>(dataReader["AvgCPUPerSec"]).ToString();
                            applicationData.AvgReads = GetValueOrDefault<string>(dataReader["AvgReads"]).ToString();
                            applicationData.AvgWrites = GetValueOrDefault<string>(dataReader["AvgWrites"]).ToString();
                            applicationData.CPUAsPercentOfList = GetValueOrDefault<string>(dataReader["CPUAsPercentOfList"]).ToString();
                            applicationData.ReadsAsPercentOfList = GetValueOrDefault<string>(dataReader["ReadsAsPercentOfList"]).ToString();
                            applicationData.AvgIOPerSec = GetValueOrDefault<string>(dataReader["AvgIOPerSec"]).ToString();
                            applicationData.AvgWaitTime = GetValueOrDefault<string>(dataReader["AvgWaitTime"]).ToString();
                            applicationData.AvgBlockingTime = GetValueOrDefault<string>(dataReader["AvgBlockingTime"]).ToString();
                            applicationData.AvgDeadlocks = GetValueOrDefault<string>(dataReader["AvgDeadlocks"]).ToString();

                            applicationGroupDataList.Add(applicationData);
                        }
                    }
                }
                return applicationGroupDataList;
            }
        }


        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - return query monitor statistics data grouped by database
        public static List<DC.QueryMonitorStatisticsData> GetQueryMonitorStatisticsByDatabase(SqlConnectionInfo connectionInfo, int sqlServerID, float timeZoneOffset, string applicationIds, string databaseIds, string userIds, string clientIds, string sqlIncludeText, string sqlExcludeText,
                      int includeSQLStatement, int includeSQLProcedure, int includeSQLBatch, int includeIncompletedQueries, int includeTimeOverlappedQueries, DateTime? startTimestamp, DateTime? endTimestamp,
                         string sortBy, string sortOrder, int recordStartIndex, int recordsCount, int sqlSignatureID, int statementTypeID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<DC.QueryMonitorStatisticsData> databaseGroupDataList = new List<DC.QueryMonitorStatisticsData>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetQueryMonitorDataByDatabase, sqlServerID, applicationIds, databaseIds, userIds, clientIds, sqlExcludeText, sqlIncludeText,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startTimestamp, endTimestamp, sortBy, sortOrder, recordStartIndex, recordsCount, sqlSignatureID, statementTypeID))
                {
                    while (dataReader.Read())
                    {
                        int ordinal = dataReader.GetOrdinal("DatabaseID");
                        if (!(dataReader.IsDBNull(ordinal)))
                        {
                            DC.QueryMonitorStatisticsData databaseData = new DC.QueryMonitorStatisticsData();

                            databaseData.DatabaseID = dataReader.GetInt32(ordinal);
                            databaseData.DatabaseName = GetValueOrDefault<string>(dataReader["DatabaseName"]).ToString();
                            databaseData.Occurrences = (int)GetValueOrDefault<int>(dataReader["Occurrences"]);
                            databaseData.TotalDuration = (Int64)GetValueOrDefault<Int64>(dataReader["TotalDuration"]);
                            databaseData.AvgDuration = GetValueOrDefault<string>(dataReader["AvgDuration"]).ToString();
                            databaseData.TotalCPUTime = (Int64)GetValueOrDefault<Int64>(dataReader["TotalCPUTime"]);
                            databaseData.TotalReads = (Int64)GetValueOrDefault<Int64>(dataReader["TotalReads"]);
                            databaseData.TotalWrites = (Int64)GetValueOrDefault<Int64>(dataReader["TotalWrites"]);
                            databaseData.TotalIO = (Int64)GetValueOrDefault<Int64>(dataReader["TotalIO"]);
                            databaseData.AvgWaitTime = GetValueOrDefault<string>(dataReader["AvgWaitTime"]).ToString();
                            databaseData.MostRecentCompletion = ((DateTime)GetValueOrDefault<DateTime>(dataReader["MostRecentCompletion"])).AddHours(-timeZoneOffset).ToString("dd-MM-yyyy HH:mm:ss");
                            databaseData.AvgBlockingTime = GetValueOrDefault<string>(dataReader["AvgBlockingTime"]).ToString();
                            databaseData.AvgDeadlocks = GetValueOrDefault<string>(dataReader["AvgDeadlocks"]).ToString();
                            databaseData.Application = GetValueOrDefault<string>(dataReader["Application"]).ToString();
                            databaseData.AvgCPUTime = GetValueOrDefault<string>(dataReader["AvgCPUTime"]).ToString();
                            databaseData.AvgCPUPerSec = GetValueOrDefault<string>(dataReader["AvgCPUPerSec"]).ToString();
                            databaseData.AvgReads = GetValueOrDefault<string>(dataReader["AvgReads"]).ToString();
                            databaseData.AvgWrites = GetValueOrDefault<string>(dataReader["AvgWrites"]).ToString();
                            databaseData.CPUAsPercentOfList = GetValueOrDefault<string>(dataReader["CPUAsPercentOfList"]).ToString();
                            databaseData.ReadsAsPercentOfList = GetValueOrDefault<string>(dataReader["ReadsAsPercentOfList"]).ToString();
                            databaseData.AvgIOPerSec = GetValueOrDefault<string>(dataReader["AvgIOPerSec"]).ToString();
                            databaseData.TotalWaitTime = (Int64)GetValueOrDefault<Int64>(dataReader["TotalWaitTime"]);
                            databaseData.TotalBlockingTime = (Int64)GetValueOrDefault<Int64>(dataReader["TotalBlockingTime"]);
                            databaseData.TotalDeadlocks = (Int64)GetValueOrDefault<Int64>(dataReader["TotalDeadlocks"]);

                            databaseGroupDataList.Add(databaseData);
                        }
                    }
                }
                return databaseGroupDataList;
            }
        }

        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - return query monitor statistics data grouped by user
        public static List<DC.QueryMonitorStatisticsData> GetQueryMonitorStatisticsByUser(SqlConnectionInfo connectionInfo, int sqlServerID, float timeZoneOffset, string applicationIds, string databaseIds, string userIds, string clientIds, string sqlIncludeText, string sqlExcludeText,
                      int includeSQLStatement, int includeSQLProcedure, int includeSQLBatch, int includeIncompletedQueries, int includeTimeOverlappedQueries, DateTime? startTimestamp, DateTime? endTimestamp,
                        string sortBy, string sortOrder, int recordStartIndex, int recordsCount, int sqlSignatureID, int statementTypeID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<DC.QueryMonitorStatisticsData> userGroupDataList = new List<DC.QueryMonitorStatisticsData>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetQueryMonitorDataByUser, sqlServerID, applicationIds, databaseIds, userIds, clientIds, sqlExcludeText, sqlIncludeText,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startTimestamp, endTimestamp, sortBy, sortOrder, recordStartIndex, recordsCount, sqlSignatureID, statementTypeID))
                {
                    while (dataReader.Read())
                    {
                        int ordinal = dataReader.GetOrdinal("UserID");
                        if (!(dataReader.IsDBNull(ordinal)))
                        {
                            DC.QueryMonitorStatisticsData userData = new DC.QueryMonitorStatisticsData();

                            userData.UserID = dataReader.GetInt32(ordinal);
                            userData.UserName = GetValueOrDefault<string>(dataReader["UserName"]).ToString();
                            userData.Occurrences = (int)GetValueOrDefault<int>(dataReader["Occurrences"]);
                            userData.TotalDuration = (Int64)GetValueOrDefault<Int64>(dataReader["TotalDuration"]);
                            userData.AvgDuration = GetValueOrDefault<string>(dataReader["AvgDuration"]).ToString();
                            userData.TotalCPUTime = (Int64)GetValueOrDefault<Int64>(dataReader["TotalCPUTime"]);
                            userData.TotalReads = (Int64)GetValueOrDefault<Int64>(dataReader["TotalReads"]);
                            userData.TotalWrites = (Int64)GetValueOrDefault<Int64>(dataReader["TotalWrites"]);
                            userData.AvgWaitTime = GetValueOrDefault<string>(dataReader["AvgWaitTime"]).ToString();
                            userData.MostRecentCompletion = ((DateTime)GetValueOrDefault<DateTime>(dataReader["MostRecentCompletion"])).AddHours(-timeZoneOffset).ToString("dd-MM-yyyy HH:mm:ss");
                            userData.AvgBlockingTime = GetValueOrDefault<string>(dataReader["AvgBlockingTime"]).ToString();
                            userData.AvgDeadlocks = GetValueOrDefault<string>(dataReader["AvgDeadlocks"]).ToString();
                            userData.Application = GetValueOrDefault<string>(dataReader["Application"]).ToString();
                            userData.DatabaseName = GetValueOrDefault<string>(dataReader["DatabaseName"]).ToString();
                            userData.AvgCPUTime = GetValueOrDefault<string>(dataReader["AvgCPUTime"]).ToString();
                            userData.AvgCPUPerSec = GetValueOrDefault<string>(dataReader["AvgCPUPerSec"]).ToString();
                            userData.AvgReads = GetValueOrDefault<string>(dataReader["AvgReads"]).ToString();
                            userData.AvgWrites = GetValueOrDefault<string>(dataReader["AvgWrites"]).ToString();
                            userData.CPUAsPercentOfList = GetValueOrDefault<string>(dataReader["CPUAsPercentOfList"]).ToString();
                            userData.ReadsAsPercentOfList = GetValueOrDefault<string>(dataReader["ReadsAsPercentOfList"]).ToString();
                            userData.AvgIOPerSec = GetValueOrDefault<string>(dataReader["AvgIOPerSec"]).ToString();
                            userData.TotalWaitTime = (Int64)GetValueOrDefault<Int64>(dataReader["TotalWaitTime"]);
                            userData.TotalBlockingTime = (Int64)GetValueOrDefault<Int64>(dataReader["TotalBlockingTime"]);
                            userData.TotalDeadlocks = (Int64)GetValueOrDefault<Int64>(dataReader["TotalDeadlocks"]);

                            userGroupDataList.Add(userData);
                        }
                    }
                }
                return userGroupDataList;
            }
        }


        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - return query monitor statistics data grouped by client
        public static List<DC.QueryMonitorStatisticsData> GetQueryMonitorStatisticsByClient(SqlConnectionInfo connectionInfo, int sqlServerID, float timeZoneOffset, string applicationIds, string databaseIds, string userIds, string clientIds, string sqlIncludeText, string sqlExcludeText,
                      int includeSQLStatement, int includeSQLProcedure, int includeSQLBatch, int includeIncompletedQueries, int includeTimeOverlappedQueries, DateTime? startTimestamp, DateTime? endTimestamp,
                        string sortBy, string sortOrder, int recordStartIndex, int recordsCount, int sqlSignatureID, int statementTypeID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<DC.QueryMonitorStatisticsData> clientGroupDataList = new List<DC.QueryMonitorStatisticsData>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetQueryMonitorDataByClient, sqlServerID, applicationIds, databaseIds, userIds, clientIds, sqlExcludeText, sqlIncludeText,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startTimestamp, endTimestamp, sortBy, sortOrder, recordStartIndex, recordsCount, sqlSignatureID, statementTypeID))
                {
                    while (dataReader.Read())
                    {
                        int ordinal = dataReader.GetOrdinal("ClientID");
                        if (!(dataReader.IsDBNull(ordinal)))
                        {
                            DC.QueryMonitorStatisticsData clientData = new DC.QueryMonitorStatisticsData();

                            clientData.ClientID = dataReader.GetInt32(ordinal);
                            clientData.Client = GetValueOrDefault<string>(dataReader["Client"]).ToString();
                            clientData.Occurrences = (int)GetValueOrDefault<int>(dataReader["Occurrences"]);
                            clientData.TotalDuration = (Int64)GetValueOrDefault<Int64>(dataReader["TotalDuration"]);
                            clientData.AvgDuration = GetValueOrDefault<string>(dataReader["AvgDuration"]).ToString();
                            clientData.TotalCPUTime = (Int64)GetValueOrDefault<Int64>(dataReader["TotalCPUTime"]);
                            clientData.TotalReads = (Int64)GetValueOrDefault<Int64>(dataReader["TotalReads"]);
                            clientData.TotalWrites = (Int64)GetValueOrDefault<Int64>(dataReader["TotalWrites"]);
                            clientData.AvgWaitTime = GetValueOrDefault<string>(dataReader["AvgWaitTime"]).ToString();
                            clientData.MostRecentCompletion = ((DateTime)GetValueOrDefault<DateTime>(dataReader["MostRecentCompletion"])).AddHours(-timeZoneOffset).ToString("dd-MM-yyyy HH:mm:ss");
                            clientData.AvgBlockingTime = GetValueOrDefault<string>(dataReader["AvgBlockingTime"]).ToString();
                            clientData.AvgDeadlocks = GetValueOrDefault<string>(dataReader["AvgDeadlocks"]).ToString();
                            clientData.Application = GetValueOrDefault<string>(dataReader["Application"]).ToString();
                            clientData.DatabaseName = GetValueOrDefault<string>(dataReader["DatabaseName"]).ToString();
                            clientData.AvgCPUTime = GetValueOrDefault<string>(dataReader["AvgCPUTime"]).ToString();
                            clientData.AvgCPUPerSec = GetValueOrDefault<string>(dataReader["AvgCPUPerSec"]).ToString();
                            clientData.AvgReads = GetValueOrDefault<string>(dataReader["AvgReads"]).ToString();
                            clientData.AvgWrites = GetValueOrDefault<string>(dataReader["AvgWrites"]).ToString();
                            clientData.CPUAsPercentOfList = GetValueOrDefault<string>(dataReader["CPUAsPercentOfList"]).ToString();
                            clientData.ReadsAsPercentOfList = GetValueOrDefault<string>(dataReader["ReadsAsPercentOfList"]).ToString();
                            clientData.AvgIOPerSec = GetValueOrDefault<string>(dataReader["AvgIOPerSec"]).ToString();
                            clientData.TotalWaitTime = (Int64)GetValueOrDefault<Int64>(dataReader["TotalWaitTime"]);
                            clientData.TotalBlockingTime = (Int64)GetValueOrDefault<Int64>(dataReader["TotalBlockingTime"]);
                            clientData.TotalDeadlocks = (Int64)GetValueOrDefault<Int64>(dataReader["TotalDeadlocks"]);

                            clientGroupDataList.Add(clientData);
                        }
                    }
                }
                return clientGroupDataList;
            }
        }


        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - return query monitor statistics data grouped by query signature
        public static List<DC.QueryMonitorStatisticsData> GetQueryMonitorStatisticsByQuerySignature(SqlConnectionInfo connectionInfo, int sqlServerID, float timeZoneOffset, string applicationIds, string databaseIds, string userIds, string clientIds, string sqlIncludeText, string sqlExcludeText,
                      int includeSQLStatement, int includeSQLProcedure, int includeSQLBatch, int includeIncompletedQueries, int includeTimeOverlappedQueries, DateTime? startTimestamp, DateTime? endTimestamp,
                        string sortBy, string sortOrder, int recordStartIndex, int recordsCount, int sqlSignatureID, int statementTypeID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<DC.QueryMonitorStatisticsData> querySignatureGroupDataList = new List<DC.QueryMonitorStatisticsData>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetQueryMonitorDataByQuerySignature, sqlServerID, applicationIds, databaseIds, userIds, clientIds, sqlExcludeText, sqlIncludeText,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startTimestamp, endTimestamp, sortBy, sortOrder, recordStartIndex, recordsCount, sqlSignatureID, statementTypeID))
                {
                    while (dataReader.Read())
                    {
                        int ordinal = dataReader.GetOrdinal("SQLSignatureID");
                        if (!(dataReader.IsDBNull(ordinal)))
                        {
                            DC.QueryMonitorStatisticsData querySignatureData = new DC.QueryMonitorStatisticsData();

                            querySignatureData.SQLSignatureID = dataReader.GetInt32(ordinal);
                            querySignatureData.SignatureSQLText = GetValueOrDefault<string>(dataReader["SignatureSQLText"]).ToString();
                            querySignatureData.Occurrences = (int)GetValueOrDefault<int>(dataReader["Occurrences"]);
                            querySignatureData.EventType = GetValueOrDefault<string>(dataReader["EventType"]).ToString();
                            querySignatureData.TotalDuration = (Int64)GetValueOrDefault<Int64>(dataReader["TotalDuration"]);
                            querySignatureData.AvgDuration = GetValueOrDefault<string>(dataReader["AvgDuration"]).ToString();
                            querySignatureData.TotalCPUTime = (Int64)GetValueOrDefault<Int64>(dataReader["TotalCPUTime"]);
                            querySignatureData.TotalReads = (Int64)GetValueOrDefault<Int64>(dataReader["TotalReads"]);
                            querySignatureData.TotalWrites = (Int64)GetValueOrDefault<Int64>(dataReader["TotalWrites"]);
                            querySignatureData.AvgWaitTime = GetValueOrDefault<string>(dataReader["AvgWaitTime"]).ToString();
                            querySignatureData.MostRecentCompletion = ((DateTime)GetValueOrDefault<DateTime>(dataReader["MostRecentCompletion"])).AddHours(-timeZoneOffset).ToString("dd-MM-yyyy HH:mm:ss");
                            querySignatureData.AvgBlockingTime = GetValueOrDefault<string>(dataReader["AvgBlockingTime"]).ToString();
                            querySignatureData.AvgDeadlocks = GetValueOrDefault<string>(dataReader["AvgDeadlocks"]).ToString();
                            querySignatureData.KeepDetailedHistory = (bool)GetValueOrDefault<bool>(dataReader["KeepDetailedHistory"]);
                            querySignatureData.Aggregated = !((bool)GetValueOrDefault<bool>(dataReader["KeepDetailedHistory"]));
                            querySignatureData.Application = GetValueOrDefault<string>(dataReader["Application"]).ToString();
                            querySignatureData.DatabaseName = GetValueOrDefault<string>(dataReader["DatabaseName"]).ToString();
                            querySignatureData.AvgCPUTime = GetValueOrDefault<string>(dataReader["AvgCPUTime"]).ToString();
                            querySignatureData.AvgCPUPerSec = GetValueOrDefault<string>(dataReader["AvgCPUPerSec"]).ToString();
                            querySignatureData.AvgReads = GetValueOrDefault<string>(dataReader["AvgReads"]).ToString();
                            querySignatureData.AvgWrites = GetValueOrDefault<string>(dataReader["AvgWrites"]).ToString();
                            querySignatureData.CPUAsPercentOfList = GetValueOrDefault<string>(dataReader["CPUAsPercentOfList"]).ToString();
                            querySignatureData.ReadsAsPercentOfList = GetValueOrDefault<string>(dataReader["ReadsAsPercentOfList"]).ToString();
                            querySignatureData.AvgIOPerSec = GetValueOrDefault<string>(dataReader["AvgIOPerSec"]).ToString();
                            querySignatureData.TotalWaitTime = (Int64)GetValueOrDefault<Int64>(dataReader["TotalWaitTime"]);
                            querySignatureData.TotalBlockingTime = (Int64)GetValueOrDefault<Int64>(dataReader["TotalBlockingTime"]);
                            querySignatureData.TotalDeadlocks = (Int64)GetValueOrDefault<Int64>(dataReader["TotalDeadlocks"]);

                            querySignatureGroupDataList.Add(querySignatureData);
                        }
                    }
                }
                return querySignatureGroupDataList;
            }
        }


        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - return query monitor statistics data grouped by query statement
        public static List<DC.QueryMonitorStatisticsData> GetQueryMonitorStatisticsByQueryStatement(SqlConnectionInfo connectionInfo, int sqlServerID, float timeZoneOffset, string applicationIds, string databaseIds, string userIds, string clientIds, string sqlIncludeText, string sqlExcludeText,
                      int includeSQLStatement, int includeSQLProcedure, int includeSQLBatch, int includeIncompletedQueries, int includeTimeOverlappedQueries, DateTime? startTimestamp, DateTime? endTimestamp,
                        string sortBy, string sortOrder, int recordStartIndex, int recordsCount, int sqlSignatureID, int statementTypeID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<DC.QueryMonitorStatisticsData> queryStatementGroupDataList = new List<DC.QueryMonitorStatisticsData>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetQueryMonitorDataByQueryStatement, sqlServerID, applicationIds, databaseIds, userIds, clientIds, sqlExcludeText, sqlIncludeText,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startTimestamp, endTimestamp, sortBy, sortOrder, recordStartIndex, recordsCount, sqlSignatureID, statementTypeID))
                {
                    int queryID = 0;
                    while (dataReader.Read())
                    {
                        int ordinal = dataReader.GetOrdinal("SQLStatementID");
                        queryID++;
                        if (!(dataReader.IsDBNull(ordinal)))
                        {
                            DC.QueryMonitorStatisticsData queryStatementData = new DC.QueryMonitorStatisticsData();

                            queryStatementData.SQLStatementID = dataReader.GetInt32(ordinal);
                            queryStatementData.QueryStatisticsID = (int)GetValueOrDefault<int>(dataReader["QueryStatisticsID"]); ; // SQLdm 9.0 (Ankit Srivastava) - Query Plan View - added new property to works as unique key
                            queryStatementData.QueryName = "Query #" + queryID;
                            queryStatementData.StatementSQLText = GetValueOrDefault<string>(dataReader["StatementSQLText"]).ToString();
                            queryStatementData.Occurrences = (int)GetValueOrDefault<int>(dataReader["Occurrences"]);
                            queryStatementData.EventType = GetValueOrDefault<string>(dataReader["EventType"]).ToString();
                            queryStatementData.TotalDuration = (Int64)GetValueOrDefault<Int64>(dataReader["TotalDuration"]);
                            queryStatementData.AvgDuration = GetValueOrDefault<string>(dataReader["AvgDuration"]).ToString();
                            queryStatementData.TotalCPUTime = (Int64)GetValueOrDefault<Int64>(dataReader["TotalCPUTime"]);
                            queryStatementData.TotalReads = (Int64)GetValueOrDefault<Int64>(dataReader["TotalReads"]);
                            queryStatementData.TotalWrites = (Int64)GetValueOrDefault<Int64>(dataReader["TotalWrites"]);
                            queryStatementData.MostRecentCompletion = ((DateTime)GetValueOrDefault<DateTime>(dataReader["MostRecentCompletion"])).AddHours(-timeZoneOffset).ToString("dd-MM-yyyy HH:mm:ss");
                            queryStatementData.KeepDetailedHistory = (bool)GetValueOrDefault<bool>(dataReader["KeepDetailedHistory"]);
                            queryStatementData.Aggregated = !((bool)GetValueOrDefault<bool>(dataReader["KeepDetailedHistory"]));
                            queryStatementData.Application = GetValueOrDefault<string>(dataReader["Application"]).ToString();
                            queryStatementData.DatabaseName = GetValueOrDefault<string>(dataReader["DatabaseName"]).ToString();
                            queryStatementData.Client = GetValueOrDefault<string>(dataReader["Client"]).ToString();
                            queryStatementData.UserName = GetValueOrDefault<string>(dataReader["UserName"]).ToString();
                            queryStatementData.Spid = (Int16)GetValueOrDefault<Int16>(dataReader["Spid"]);
                            queryStatementData.StartTime = ((DateTime)GetValueOrDefault<DateTime>(dataReader["StartTime"])).AddHours(-timeZoneOffset).ToString("dd-MM-yyyy HH:mm:ss");
                            queryStatementData.AvgCPUPerSec = GetValueOrDefault<string>(dataReader["AvgCPUPerSec"]).ToString();
                            queryStatementData.CPUAsPercentOfList = GetValueOrDefault<string>(dataReader["CPUAsPercentOfList"]).ToString();
                            queryStatementData.ReadsAsPercentOfList = GetValueOrDefault<string>(dataReader["ReadsAsPercentOfList"]).ToString();
                            queryStatementData.TotalWaitTime = (Int64)GetValueOrDefault<Int64>(dataReader["TotalWaitTime"]);
                            queryStatementData.TotalBlockingTime = (Int64)GetValueOrDefault<Int64>(dataReader["TotalBlockingTime"]);
                            queryStatementData.TotalDeadlocks = (Int64)GetValueOrDefault<Int64>(dataReader["TotalDeadlocks"]);

                            queryStatementGroupDataList.Add(queryStatementData);
                        }
                    }
                }
                return queryStatementGroupDataList;
            }
        }

        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - determine the groups which needs to be displayed on the Y-Axis of graphs
        public static void GetGroupsToBeRepresentedOnYAxis(DateTime startTimestamp, DateTime endTimestamp, DataTable dataTable, out TimeSpan interval, out double totalMinutesInInterval,
                               out Dictionary<string, Int64> aggregateGroupName, out int dictionarySize, out int yAxisLimit)
        {
            aggregateGroupName = new Dictionary<string, Int64>();
            interval = new TimeSpan();
            totalMinutesInInterval = 0.0;

            if (startTimestamp != null && endTimestamp != null)
            {
                interval = endTimestamp - startTimestamp;
                totalMinutesInInterval = interval.TotalMinutes;
            }

            foreach (DataRow data in dataTable.Rows)                    // populate the dictionary with aggregation
            {
                string key = GetValueOrDefault<string>(data["GroupByID"]).ToString();
                Int64 value = (Int64)GetValueOrDefault<Int64>(data["YAxisValue"]);
                if (aggregateGroupName.ContainsKey(key))
                    aggregateGroupName[key] += value;
                else
                    aggregateGroupName[key] = value;
            }

            dictionarySize = aggregateGroupName.Count;
            yAxisLimit = Math.Min(dictionarySize, MaximumYAxisLimit);            // update the yAxisLimit   

            var temp = aggregateGroupName.OrderByDescending(i => i.Value).Take(yAxisLimit);    // sort and take the limited number of records which needs to be displayed separately on WebUI
            aggregateGroupName = temp.ToDictionary(i => i.Key, i => i.Value);
        }

        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - compute the bucket size and type for the data used to represent graphs
        public static void ComputeBucketSizeForGraphs(double totalMinutesInInterval, out int minutesToIncrement, out string bucketType)
        {
            if ((totalMinutesInInterval / 60) <= 1)                // compute bucketType and bucket width in minutes (since minutes is the most granular unit in Graph representation)
            {
                minutesToIncrement = 1;
                bucketType = "Minutes";
            }
            else if (((totalMinutesInInterval / 60) > 1) && ((totalMinutesInInterval / 60) <= 48))
            {
                minutesToIncrement = 60;
                bucketType = "Hours";
            }
            else if (((totalMinutesInInterval / 60) > 48) && ((totalMinutesInInterval / 60) <= 1440))     // 1440 = minutes in 1 day
            {
                minutesToIncrement = 1440;
                bucketType = "Days";
            }
            else                                                 // 30 days increment, 43200 = minutes in 30 days
            {
                minutesToIncrement = 43200;
                bucketType = "Months";
            }
        }

        // SQLdm 10.1.2 (Varun Chopra) QueryMonitor Optimization 
        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - return the list with all the data aggregated into the respective buckets
        public static void BucketizationOfGraphData(DateTime startTimestamp, DateTime endTimestamp, int minutesToIncrement, DataView dataView, float timeZoneOffset,
                                      Dictionary<string, Int64> aggregateGroupName, string bucketType, out List<DC.QueryMonitorDataForGraphs> groupNameDataList)
        {
            // Key based on GroupId, GroupName, BucketStart and BucketEnd
            var groupedDataDictionary = new Dictionary<string, DC.QueryMonitorDataForGraphs>();
            // GroupId of others
            var groupOtherIdToName = new Dictionary<string, string>();

            // Create Bucket Range
            var rangeBucket = new List<BucketRange>();
            for (var next = startTimestamp; next <= endTimestamp; next = next.AddMinutes(minutesToIncrement))
            {
                var bucketEndDate = endTimestamp >= next.AddMinutes(minutesToIncrement)
                    ? next.AddMinutes(minutesToIncrement)
                    : endTimestamp;

                rangeBucket.Add(new BucketRange
                {
                    BucketStartDateTime = next.AddHours(-timeZoneOffset).ToString(DATE_FORMAT),
                    BucketEndDateTime = bucketEndDate.AddHours(-timeZoneOffset).ToString(DATE_FORMAT)
                });
            }

            foreach (var row in from DataRowView rowView in dataView select rowView.Row)
            {
                var startTime = (DateTime)GetValueOrDefault<DateTime>(row[STARTTIME]);
                var totalMinutes = startTime.Subtract(startTimestamp).TotalMinutes;
                if (totalMinutes <= 0)
                {
                    // Don't process events before the startTimestamp
                    continue;
                }
                var index = (int)(totalMinutes / minutesToIncrement);
                if (index >= rangeBucket.Count)
                {
                    // Don't process events after the endTimeStamp
                    continue;
                }

                var groupById = GetValueOrDefault<string>(row[GROUPBYID]).ToString();

                // If GroupById is not part of the selected Group - Set isOther to true
                var isOther = !aggregateGroupName.ContainsKey(groupById);

                var groupByName = isOther ? OTHER : GetValueOrDefault<string>(row[GROUPBYNAME]).ToString();
                var yAxisValue = (long)GetValueOrDefault<long>(row[YAXISVALUE]);
                var selectedRangeBucket = rangeBucket[index];

                var key = (isOther ? OTHER : groupById) + selectedRangeBucket;
                var keyAlreadyExists = groupedDataDictionary.ContainsKey(key);

                if (keyAlreadyExists)
                {
                    // Increement Y-Axis Value
                    groupedDataDictionary[key].YAxisValue += yAxisValue;
                }
                else
                {
                    // Add as key not present
                    groupedDataDictionary.Add(key, new DC.QueryMonitorDataForGraphs()
                    {
                        BucketEndDateTime = selectedRangeBucket.BucketEndDateTime,
                        BucketStartDateTime = selectedRangeBucket.BucketStartDateTime,
                        BucketType = bucketType,
                        GroupByID = groupById,
                        GroupByName = groupByName,
                        YAxisValue = yAxisValue
                    });
                }

                if (!isOther || groupOtherIdToName.ContainsKey(groupById))
                {
                    // Continue for non other cases and where no need to update groupbyid
                    continue;
                }

                // Add to Others Group
                groupOtherIdToName.Add(groupById, groupByName);

                if (keyAlreadyExists)
                {
                    // Update GroupId for Other Field only if key already exists as newly added key will already have group id updated
                    groupedDataDictionary[key].GroupByID += COMMASEPARATOR + groupById;
                }
            }
            // Set groupNameDataList
            groupNameDataList = groupedDataDictionary.Values.ToList();

            // Clear the Lists
            groupOtherIdToName.Clear();
            groupedDataDictionary.Clear();
        }

        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - return next datetime while bucketization of the data
        public static IEnumerable<DateTime> GetNextDateTime(DateTime start, DateTime end, Int64 minutesToIncrement)
        {
            for (var next = start; next <= end; next = next.AddMinutes(minutesToIncrement))
                yield return next;
        }

        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - return query monitor statistics data grouped by application for graph representation
        public static List<DC.QueryMonitorDataForGraphs> GetQueryMonitorDataByApplicationForGraphs(SqlConnectionInfo connectionInfo, int sqlServerID, int viewID, float timeZoneOffset, string applicationIds, string databaseIds, string userIds, string clientIds, string sqlIncludeText, string sqlExcludeText,
                      int includeSQLStatement, int includeSQLProcedure, int includeSQLBatch, int includeIncompletedQueries, int includeTimeOverlappedQueries, DateTime startTimestamp, DateTime endTimestamp, int sqlSignatureID, int statementTypeID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<DC.QueryMonitorDataForGraphs> applicationGroupDataList = new List<DC.QueryMonitorDataForGraphs>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetGraphRepresentationDataByApplication, sqlServerID, viewID, applicationIds, databaseIds, userIds, clientIds, sqlExcludeText, sqlIncludeText,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startTimestamp, endTimestamp, sqlSignatureID, statementTypeID))
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    DataView dataView = new DataView(dataTable);        // sort according to the start time as an optimization while bucketization
                    dataView.Sort = "StartTime ASC";

                    Dictionary<string, Int64> groupByApplication = new Dictionary<string, Int64>();
                    int dictionarySize, yAxisLimit;

                    string bucketType;
                    int minutesToIncrement;

                    double totalMinutesInInterval = 0.0;                     // compute total minutes in the interval for bucketization
                    TimeSpan interval;

                    GetGroupsToBeRepresentedOnYAxis(startTimestamp, endTimestamp, dataTable, out interval, out totalMinutesInInterval, out groupByApplication, out dictionarySize, out yAxisLimit);

                    ComputeBucketSizeForGraphs(totalMinutesInInterval, out minutesToIncrement, out bucketType);

                    BucketizationOfGraphData(startTimestamp, endTimestamp, minutesToIncrement, dataView, timeZoneOffset, groupByApplication, bucketType, out applicationGroupDataList);

                }
                return applicationGroupDataList;
            }
        }

        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - return query monitor statistics data grouped by database for graph representation
        public static List<DC.QueryMonitorDataForGraphs> GetQueryMonitorDataByDatabaseForGraphs(SqlConnectionInfo connectionInfo, int sqlServerID, int viewID, float timeZoneOffset, string applicationIds, string databaseIds, string userIds, string clientIds, string sqlIncludeText, string sqlExcludeText,
                      int includeSQLStatement, int includeSQLProcedure, int includeSQLBatch, int includeIncompletedQueries, int includeTimeOverlappedQueries, DateTime startTimestamp, DateTime endTimestamp, int sqlSignatureID, int statementTypeID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<DC.QueryMonitorDataForGraphs> databaseGroupDataList = new List<DC.QueryMonitorDataForGraphs>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetGraphRepresentationDataByDatabase, sqlServerID, viewID, applicationIds, databaseIds, userIds, clientIds, sqlExcludeText, sqlIncludeText,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startTimestamp, endTimestamp, sqlSignatureID, statementTypeID))
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    DataView dataView = new DataView(dataTable);    // sort according to the start time as an optimization while bucketization
                    dataView.Sort = "StartTime ASC";

                    var groupByDatabase = new Dictionary<string, Int64>();
                    int dictionarySize, yAxisLimit;

                    string bucketType;
                    int minutesToIncrement;

                    double totalMinutesInInterval = 0.0;                     // compute total minutes in the interval for bucketization
                    TimeSpan interval;

                    GetGroupsToBeRepresentedOnYAxis(startTimestamp, endTimestamp, dataTable, out interval, out totalMinutesInInterval, out groupByDatabase, out dictionarySize, out yAxisLimit);

                    ComputeBucketSizeForGraphs(totalMinutesInInterval, out minutesToIncrement, out bucketType);

                    BucketizationOfGraphData(startTimestamp, endTimestamp, minutesToIncrement, dataView, timeZoneOffset, groupByDatabase, bucketType, out databaseGroupDataList);
                }
                return databaseGroupDataList;
            }
        }

        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - return query monitor statistics data grouped by user for graph representation
        public static List<DC.QueryMonitorDataForGraphs> GetQueryMonitorDataByUserForGraphs(SqlConnectionInfo connectionInfo, int sqlServerID, int viewID, float timeZoneOffset, string applicationIds, string databaseIds, string userIds, string clientIds, string sqlIncludeText, string sqlExcludeText,
                      int includeSQLStatement, int includeSQLProcedure, int includeSQLBatch, int includeIncompletedQueries, int includeTimeOverlappedQueries, DateTime startTimestamp, DateTime endTimestamp, int sqlSignatureID, int statementTypeID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<DC.QueryMonitorDataForGraphs> userGroupDataList = new List<DC.QueryMonitorDataForGraphs>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetGraphRepresentationDataByUser, sqlServerID, viewID, applicationIds, databaseIds, userIds, clientIds, sqlExcludeText, sqlIncludeText,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startTimestamp, endTimestamp, sqlSignatureID, statementTypeID))
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    DataView dataView = new DataView(dataTable);
                    dataView.Sort = "StartTime ASC";                  // sort according to the start time as an optimization while bucketization

                    var groupByUser = new Dictionary<string, Int64>();
                    int dictionarySize, yAxisLimit;

                    string bucketType;
                    int minutesToIncrement;

                    double totalMinutesInInterval = 0.0;                     // compute total minutes in the interval for bucketization
                    TimeSpan interval;

                    GetGroupsToBeRepresentedOnYAxis(startTimestamp, endTimestamp, dataTable, out interval, out totalMinutesInInterval, out groupByUser, out dictionarySize, out yAxisLimit);

                    ComputeBucketSizeForGraphs(totalMinutesInInterval, out minutesToIncrement, out bucketType);

                    BucketizationOfGraphData(startTimestamp, endTimestamp, minutesToIncrement, dataView, timeZoneOffset, groupByUser, bucketType, out userGroupDataList);
                }
                return userGroupDataList;
            }
        }

        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - return query monitor statistics data grouped by client for graph representation
        public static List<DC.QueryMonitorDataForGraphs> GetQueryMonitorDataByClientForGraphs(SqlConnectionInfo connectionInfo, int sqlServerID, int viewID, float timeZoneOffset, string applicationIds, string databaseIds, string userIds, string clientIds, string sqlIncludeText, string sqlExcludeText,
                      int includeSQLStatement, int includeSQLProcedure, int includeSQLBatch, int includeIncompletedQueries, int includeTimeOverlappedQueries, DateTime startTimestamp, DateTime endTimestamp, int sqlSignatureID, int statementTypeID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<DC.QueryMonitorDataForGraphs> clientGroupDataList = new List<DC.QueryMonitorDataForGraphs>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetGraphRepresentationDataByClient, sqlServerID, viewID, applicationIds, databaseIds, userIds, clientIds, sqlExcludeText, sqlIncludeText,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startTimestamp, endTimestamp, sqlSignatureID, statementTypeID))
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    DataView dataView = new DataView(dataTable);
                    dataView.Sort = "StartTime ASC";                          // sort according to the start time as an optimization while bucketization

                    var groupByClient = new Dictionary<string, Int64>();
                    int dictionarySize, yAxisLimit;

                    string bucketType;
                    int minutesToIncrement;

                    double totalMinutesInInterval = 0.0;                     // compute total minutes in the interval for bucketization
                    TimeSpan interval;

                    GetGroupsToBeRepresentedOnYAxis(startTimestamp, endTimestamp, dataTable, out interval, out totalMinutesInInterval, out groupByClient, out dictionarySize, out yAxisLimit);

                    ComputeBucketSizeForGraphs(totalMinutesInInterval, out minutesToIncrement, out bucketType);

                    BucketizationOfGraphData(startTimestamp, endTimestamp, minutesToIncrement, dataView, timeZoneOffset, groupByClient, bucketType, out clientGroupDataList);
                }
                return clientGroupDataList;
            }
        }

        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - return query monitor statistics data grouped by query signature for graph representation
        public static List<DC.QueryMonitorDataForGraphs> GetQueryMonitorDataByQuerySignatureForGraphs(SqlConnectionInfo connectionInfo, int sqlServerID, int viewID, float timeZoneOffset, string applicationIds, string databaseIds, string userIds, string clientIds, string sqlIncludeText, string sqlExcludeText,
                      int includeSQLStatement, int includeSQLProcedure, int includeSQLBatch, int includeIncompletedQueries, int includeTimeOverlappedQueries, DateTime startTimestamp, DateTime endTimestamp, int sqlSignatureID, int statementTypeID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<DC.QueryMonitorDataForGraphs> querySignatureGroupDataList = new List<DC.QueryMonitorDataForGraphs>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetGraphRepresentationDataByQuerySignature, sqlServerID, viewID, applicationIds, databaseIds, userIds, clientIds, sqlExcludeText, sqlIncludeText,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startTimestamp, endTimestamp, sqlSignatureID, statementTypeID))
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    DataView dataView = new DataView(dataTable);
                    dataView.Sort = "StartTime ASC";                          // sort according to the start time as an optimization while bucketization

                    var groupByQuerySignature = new Dictionary<string, Int64>();
                    int dictionarySize, yAxisLimit;

                    string bucketType;
                    int minutesToIncrement;

                    double totalMinutesInInterval = 0.0;                     // compute total minutes in the interval for bucketization
                    TimeSpan interval;

                    GetGroupsToBeRepresentedOnYAxis(startTimestamp, endTimestamp, dataTable, out interval, out totalMinutesInInterval, out groupByQuerySignature, out dictionarySize, out yAxisLimit);

                    ComputeBucketSizeForGraphs(totalMinutesInInterval, out minutesToIncrement, out bucketType);

                    BucketizationOfGraphData(startTimestamp, endTimestamp, minutesToIncrement, dataView, timeZoneOffset, groupByQuerySignature, bucketType, out querySignatureGroupDataList);
                }
                return querySignatureGroupDataList;
            }
        }

        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - return query monitor statistics data grouped by query statement for graph representation
        public static List<DC.QueryMonitorDataForGraphs> GetQueryMonitorDataByQueryStatementForGraphs(SqlConnectionInfo connectionInfo, int sqlServerID, int viewID, float timeZoneOffset, string applicationIds, string databaseIds, string userIds, string clientIds, string sqlIncludeText, string sqlExcludeText,
                      int includeSQLStatement, int includeSQLProcedure, int includeSQLBatch, int includeIncompletedQueries, int includeTimeOverlappedQueries, DateTime startTimestamp, DateTime endTimestamp, int sqlSignatureID, int statementTypeID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            List<DC.QueryMonitorDataForGraphs> queryStatementGroupDataList = new List<DC.QueryMonitorDataForGraphs>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetGraphRepresentationDataByQueryStatement, sqlServerID, viewID, applicationIds, databaseIds, userIds, clientIds, sqlExcludeText, sqlIncludeText,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startTimestamp, endTimestamp, sqlSignatureID, statementTypeID))
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(dataReader);
                    DataView dataView = new DataView(dataTable);
                    dataView.Sort = "StartTime ASC";                          // sort according to the start time as an optimization while bucketization

                    var groupByQueryStatement = new Dictionary<string, Int64>();
                    int dictionarySize, yAxisLimit;

                    string bucketType;
                    int minutesToIncrement;

                    double totalMinutesInInterval = 0.0;                     // compute total minutes in the interval for bucketization
                    TimeSpan interval;

                    GetGroupsToBeRepresentedOnYAxis(startTimestamp, endTimestamp, dataTable, out interval, out totalMinutesInInterval, out groupByQueryStatement, out dictionarySize, out yAxisLimit);

                    ComputeBucketSizeForGraphs(totalMinutesInInterval, out minutesToIncrement, out bucketType);

                    BucketizationOfGraphData(startTimestamp, endTimestamp, minutesToIncrement, dataView, timeZoneOffset, groupByQueryStatement, bucketType, out queryStatementGroupDataList);
                }
                return queryStatementGroupDataList;
            }
        }

        #endregion

        #region ProductInfo
        public static ProductStatus GetProductStatus(SqlConnectionInfo connectionInfo, UserToken userToken)
        {
            ProductStatus productStatus = new ProductStatus();
            AlertProductStatus alertProductStatus = new AlertProductStatus();

            if (connectionInfo == null)
            {
                return null;
            }
            using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
            {
                connection.Open();
                //START SQLdm 10.0 (Swati Gogia) : Implemented for Instance Level Security
                IList<ServerPermission> assignedServers = userToken.AssignedServers.ToList();

                SqlParameter[] arrSqlParam = new SqlParameter[1];
                XDocument xmlDoc = new XDocument();
                XElement xElm = new XElement("Root",
                             from l in assignedServers.Select(x => x.Server.SQLServerID)
                             select new XElement("Source", new XElement("ID", l))
                    );
                xmlDoc.Add(xElm);
                arrSqlParam[0] = new SqlParameter("@SqlIdList", "N" + xmlDoc.ToString());
                //END SQLdm 10.0 (Swati Gogia)

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetProductStatusStoredProcedure, arrSqlParam))
                {
                    //initializing variables
                    productStatus = new ProductStatus();
                    alertProductStatus = new AlertProductStatus();
                    productStatus.AlertStatus = alertProductStatus;

                    while (reader.Read())
                    {
                        int severityOrdinal = reader.GetOrdinal("Severity"), alertCountOrdinal = reader.GetOrdinal("AlertCount");
                        Int64 currAlertCount = 0;
                        if (severityOrdinal > -1 && reader.IsDBNull(severityOrdinal) == false)
                        {
                            currAlertCount = reader.IsDBNull(alertCountOrdinal) == false ? Convert.ToInt64(reader[alertCountOrdinal]) : 0;
                            //2 for informational, 4 for warning and 8 for critical
                            switch (Convert.ToInt16(reader[severityOrdinal]))
                            {
                                case INFORMATIONAL_ALERT_SEVERITY:
                                    //informational
                                    alertProductStatus.InformationalAlertCount += currAlertCount;
                                    break;
                                case WARNING_ALERT_SEVERITY:
                                    //warning
                                    alertProductStatus.WarningAlertCount += currAlertCount;
                                    break;
                                case CRITICAL_ALERT_SEVERITY:
                                    //critical
                                    alertProductStatus.CriticalAlertCount += currAlertCount;
                                    break;
                                default:
                                    //if none of the above choices match
                                    alertProductStatus.CriticalAlertCount += 0;
                                    alertProductStatus.WarningAlertCount += 0;
                                    alertProductStatus.InformationalAlertCount += 0;
                                    break;
                            }
                        }

                    }

                }
            }
            return productStatus;
        }
        #endregion
        #region CustomDashboard
        /// <summary>
        /// Checks if there is already an entry for the same custom dahsboard name
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        /// <param name="connectionInfo">sql connection</param>
        /// <param name="dashboardName">name to custom dashboard</param>
        /// <param name="userSID">Id of specific user</param>
        /// <returns></returns>
        public static bool CheckDuplicateDashboardName(SqlConnectionInfo connectionInfo, string dashboardName, string userSID)
        {
            if (connectionInfo == null || dashboardName == null || dashboardName == string.Empty)
                throw new NullReferenceException("Either Connection string of dashboard name is not passed");
            bool isDuplicateDashboardFound = false;
            try
            {
                using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, CheckDuplicateCustomDashboardNameStoredProcedure))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("CustomDashboardName", dashboardName);

                        command.Parameters.AddWithValue("@userSID", userSID);
                        command.Parameters.Add("@isRecordFound", SqlDbType.Bit);
                        command.Parameters["@isRecordFound"].Direction = ParameterDirection.Output;
                        int queryResult = Convert.ToInt32(command.ExecuteScalar());
                        if (bool.TryParse(command.Parameters["@isRecordFound"].Value.ToString(), out isDuplicateDashboardFound))
                        {
                            Log.Info("check duplicate procedure returns " + isDuplicateDashboardFound);
                        }
                        else
                            throw new Exception("Output from sql is not correct.");

                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception getting record for checking duplicate dashboards: ", e);
            }
            return isDuplicateDashboardFound;
        }

        /// <summary>
        /// Checks if there is already an entry for the same custom dahsboard name
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        /// <param name="connectionInfo">sql connection</param>
        /// <param name="dashboardName">name to custom dashboard</param>
        /// <param name="userSID">Id of specific user</param>
        /// <returns></returns>
        public static bool CheckDashboardExists(SqlConnectionInfo connectionInfo, Int64 dashboardId)
        {
            if (connectionInfo == null)
                throw new NullReferenceException("Connection string is not passed.");
            bool dashboardExists = false;
            try
            {
                using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, CheckDashboardExistsStoredProcedure))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@CustomDashboardId", dashboardId);

                        command.Parameters.Add("@isRecordFound", SqlDbType.Bit);
                        command.Parameters["@isRecordFound"].Direction = ParameterDirection.Output;

                        dashboardExists = Convert.ToBoolean(command.ExecuteScalar());

                        if (dashboardExists)
                        {
                            Log.Info("check dashboard exists procedure returns " + dashboardExists);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception getting record for checking duplicate dashboards: ", e);
            }
            return dashboardExists;
        }

        /// <summary>
        /// Insert new record in customdahsboard table
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        /// <param name="connectionInfo">sql connection</param>
        /// <param name="dashboardName">name to custom dashboard</param>
        /// <param name="isDefaultOnUI">Dashboard is default </param>
        /// <param name="userSID">Id of specific user</param>
        /// <returns></returns>
        public static DC.CustomDashboard.CustomDashboard CreateCustomDashboard(SqlConnectionInfo connectionInfo, string dashboardName, bool isDefaultOnUI, string userSID)
        {
            if (connectionInfo == null)
                throw new NullReferenceException("Connection string is not passed.");
            if (dashboardName == null || dashboardName == string.Empty)
                throw new NullReferenceException("Dashboard name is not passed.");
            DC.CustomDashboard.CustomDashboard insertedCustomDashboard = null;
            long parseResult = 0;
            try
            {
                using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, UpsertCustomDashboardStoredProcedure))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@dashboardName", dashboardName);
                        command.Parameters.AddWithValue("@isDefaultOnUI", isDefaultOnUI);
                        command.Parameters.AddWithValue("@SID", userSID);
                        command.Parameters.AddWithValue("@tags", "");
                        command.Parameters.AddWithValue("@RecordTimestamp", DateTime.Now.ToUniversalTime());
                        command.Parameters.AddWithValue("@customDashboardID", SqlDbType.Int);
                        command.Parameters["@customDashboardID"].Direction = ParameterDirection.Output;

                        int errorFound = Convert.ToInt32(command.ExecuteScalar());
                        if (errorFound == 0)
                        {
                            insertedCustomDashboard = new DC.CustomDashboard.CustomDashboard();
                            Int64.TryParse(command.Parameters["@customDashboardID"].Value.ToString(), out parseResult);
                            insertedCustomDashboard.CustomDashboardId = parseResult;
                            insertedCustomDashboard.CustomDashboardName = dashboardName;
                            insertedCustomDashboard.IsDefaultOnUI = isDefaultOnUI;
                            insertedCustomDashboard.UserSID = userSID;
                            //insertedCustomDashboard.RecordCreatedTimestamp = Convert.ToDateTime(command.Parameters["@RecordCreatedTimestamp"].Value);
                            //insertedCustomDashboard.RecordUpdateDateTimestamp = Convert.ToDateTime(command.Parameters["@RecordCreatedTimestamp"].Value);
                            Log.Error("Custom record for - " + insertedCustomDashboard.CustomDashboardId + " created/updated in DB.");
                        }
                        else
                        {
                            Log.Error("while excuting p_UpsertCustomDashboard error code found : " + errorFound.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception saving record for dashboards: ", e);
            }
            return insertedCustomDashboard;
        }

        /// <summary>
        /// Insert new record in customdahsboard table
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        /// <param name="connectionInfo">sql connection</param>
        /// <param name="dashboardName">name to custom dashboard</param>
        /// <param name="isDefaultOnUI">Dashboard is default </param>
        /// <param name="userSID">Id of specific user</param>
        /// <returns></returns>
        public static DC.CustomDashboard.CustomDashboard UpdateCustomDashboard(SqlConnectionInfo connectionInfo, Int64 dashboardID, string dashboardName, bool isDefaultOnUI, string userSID, string tags)
        {
            if (connectionInfo == null)
                throw new NullReferenceException("Connection string is not passed.");
            if (dashboardName == null || dashboardName == string.Empty)
                throw new NullReferenceException("Dashboard name is not passed.");
            DC.CustomDashboard.CustomDashboard updatedCustomDashboard = null;
            long parseResult = 0;
            try
            {
                using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, UpsertCustomDashboardStoredProcedure))
                    {
                        command.Parameters.Clear();

                        command.Parameters.AddWithValue("@customDashboardID", dashboardID);
                        command.Parameters["@customDashboardID"].Direction = ParameterDirection.InputOutput;
                        command.Parameters.AddWithValue("@dashboardName", dashboardName);
                        command.Parameters.AddWithValue("@isDefaultOnUI", isDefaultOnUI);
                        command.Parameters.AddWithValue("@tags", tags);
                        command.Parameters.AddWithValue("@SID", userSID);
                        command.Parameters.AddWithValue("@RecordTimestamp", DateTime.Now.ToUniversalTime());

                        int errorFound = Convert.ToInt32(command.ExecuteScalar());
                        if (errorFound == 0)
                        {
                            updatedCustomDashboard = new DC.CustomDashboard.CustomDashboard();
                            Int64.TryParse(command.Parameters["@customDashboardID"].Value.ToString(), out parseResult);
                            updatedCustomDashboard.CustomDashboardId = parseResult;
                            updatedCustomDashboard.CustomDashboardName = dashboardName;
                            updatedCustomDashboard.IsDefaultOnUI = isDefaultOnUI;
                            //updatedCustomDashboard.UserSID = userSID;
                            //insertedCustomDashboard.RecordCreatedTimestamp = Convert.ToDateTime(command.Parameters["@RecordCreatedTimestamp"].Value);
                            //insertedCustomDashboard.RecordUpdateDateTimestamp = Convert.ToDateTime(command.Parameters["@RecordCreatedTimestamp"].Value);
                            Log.Error("Custom record for - " + updatedCustomDashboard.CustomDashboardId + " created/updated in DB.");
                        }
                        else
                        {
                            Log.Error("while excuting p_UpsertCustomDashboard error code found : " + errorFound.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception saving record for dashboards: ", e);
            }
            return updatedCustomDashboard;
        }

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To make copy of existing Custom Dashboard for a user
        /// </summary>
        /// <param name="customDashboardid"></param>
        public static bool CopyCustomDashboard(SqlConnectionInfo connectionInfo, Int64 customDashboardid)
        {
            if (connectionInfo == null)
                throw new NullReferenceException("Connection string is not passed.");
            int newCopiedCustomDashboardID = 0;
            bool isCustomDashboardCopiedSuccessfully = false;
            try
            {
                using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, CreateCopyOfCustomDashboardStoredProcedure))
                    {
                        command.Parameters.Clear();

                        command.Parameters.AddWithValue("@sourceCustomDashboardID", customDashboardid);
                        command.Parameters.AddWithValue("@copyCustomDashboardID", SqlDbType.Int);
                        command.Parameters["@copyCustomDashboardID"].Direction = ParameterDirection.Output;
                        command.ExecuteNonQuery();
                        int.TryParse(command.Parameters["@copyCustomDashboardID"].Value.ToString(), out newCopiedCustomDashboardID);
                        if (newCopiedCustomDashboardID != 0)
                        {
                            isCustomDashboardCopiedSuccessfully = true;
                        }
                        else
                        {
                            isCustomDashboardCopiedSuccessfully = false;
                        }

                    }
                }
            }

            catch (Exception e)
            {
                Log.Error("Exception saving record for dashboards: ", e);
            }

            return isCustomDashboardCopiedSuccessfully;
        }

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To get all Widget Types
        /// </summary>
        /// <param name="connectionInfo"></param>
        public static Dictionary<int, string> GetAllWidgetTypes(SqlConnectionInfo connectionInfo)
        {

            if (connectionInfo == null)
                throw new NullReferenceException("Connection string is not passed.");
            Dictionary<int, string> widgetTypes = new Dictionary<int, string>();
            try
            {
                using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, "p_GetWidgetTypes"))
                    {
                        while (reader.Read())
                        {
                            if (reader.IsDBNull(reader.GetOrdinal("WidgetTypeID")) == false)
                            {
                                widgetTypes.Add((int)GetValueOrDefault<int>(reader["WidgetTypeID"]), (string)GetValueOrDefault<string>(reader["WidgetType"]));
                            }
                            //widgetTypes.Add(Convert.ToInt32(reader["WidgetTypeID"]), reader["WidgetType"].ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception getting Widget Types from DB : ", e);
            }
            return widgetTypes;
        }

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To get all Match Types
        /// </summary>
        /// <param name="connectionInfo"></param>
        public static Dictionary<int, string> GetAllMatchTypes(SqlConnectionInfo connectionInfo)
        {

            if (connectionInfo == null)
                throw new NullReferenceException("Connection string is not passed.");
            Dictionary<int, string> matchTypes = new Dictionary<int, string>();
            try
            {
                using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, "p_GetMatchTypes"))
                    {
                        while (reader.Read())
                        {
                            if (reader.IsDBNull(reader.GetOrdinal("MatchID")) == false)
                            {
                                matchTypes.Add((int)GetValueOrDefault<int>(reader["MatchID"]), (string)GetValueOrDefault<string>(reader["MatchType"]));
                            }
                            //matchTypes.Add(Convert.ToInt32(reader["MatchID"]), reader["MatchType"].ToString());
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Log.Error("Exception getting Match Types from DB : ", e);
            }
            return matchTypes;
        }


        /// <summary>
        /// Delete existing record from customdahsboard table
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        /// <param name="CustomDashboardId">Id of specific CustomDashboard</param>
        /// <returns>CustomDashboard object</returns>
        public static DC.CustomDashboard.CustomDashboard DeleteDashboardById(SqlConnectionInfo connectionInfo, int dashboardId)
        {
            if (connectionInfo == null)
                throw new NullReferenceException("Connection string is not passed.");
            DC.CustomDashboard.CustomDashboard deletedCustomDashboard = null;
            int errorFound = 0;
            long parseResult = 0;
            try
            {
                using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, DeleteCustomDashboardRecordStoredProcedure))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@customDashboardId", dashboardId);
                        command.Parameters.Add("@customDashboardName", SqlDbType.NVarChar, 500);
                        command.Parameters["@customDashboardName"].Direction = ParameterDirection.Output;
                        command.Parameters.Add("@isDefaultOnUI", SqlDbType.Bit);
                        command.Parameters["@isDefaultOnUI"].Direction = ParameterDirection.Output;
                        command.Parameters.Add("@userSID", SqlDbType.NVarChar, 200);
                        command.Parameters["@userSID"].Direction = ParameterDirection.Output;
                        //command.Parameters.Add("@recordCreatedTimestamp", SqlDbType.DateTime);
                        //command.Parameters["@recordCreatedTimestamp"].Direction = ParameterDirection.Output;
                        //command.Parameters.Add("@recordUpdateDateTimestamp", SqlDbType.DateTime);
                        //command.Parameters["@recordUpdateDateTimestamp"].Direction = ParameterDirection.Output;
                        command.Parameters.Add("@errorCode", SqlDbType.Int);
                        command.Parameters["@errorCode"].Direction = ParameterDirection.Output;


                        int recordDeleted = command.ExecuteNonQuery();
                        errorFound = command.Parameters["@errorCode"].Value != DBNull.Value ? Convert.ToInt32(command.Parameters["@errorCode"].Value) : 0;

                        deletedCustomDashboard = new DC.CustomDashboard.CustomDashboard(); //moving out to handle case when the passed custom dash id is invalid

                        if (errorFound == 0 && recordDeleted != 0)
                        {

                            if (long.TryParse(command.Parameters["@customDashboardID"].Value.ToString(), out parseResult))
                                deletedCustomDashboard.CustomDashboardId = parseResult;
                            else
                                throw new Exception("Couldnt not parse the sql received data");
                            //todosri
                            deletedCustomDashboard.CustomDashboardName = command.Parameters["@customDashboardName"].Value.ToString();
                            deletedCustomDashboard.IsDefaultOnUI = (command.Parameters["@isDefaultOnUI"].Value != DBNull.Value) ? Convert.ToBoolean(command.Parameters["@isDefaultOnUI"].Value) : false;
                            deletedCustomDashboard.UserSID = command.Parameters["@userSID"].Value != DBNull.Value ? command.Parameters["@userSID"].Value.ToString() : string.Empty;
                            //deletedCustomDashboard.RecordCreatedTimestamp = Convert.ToDateTime(command.Parameters["@recordCreatedTimestamp"].Value);
                            //deletedCustomDashboard.RecordUpdateDateTimestamp = Convert.ToDateTime(command.Parameters["@recordUpdateDateTimestamp"].Value);
                            Log.Error("Custom record for - " + deletedCustomDashboard.CustomDashboardId + " deleted in DB.");
                            Log.Error(recordDeleted + " records deleted from DB table CustomDashbaord.");
                        }
                        else
                        {
                            // if record is not deleted then log reason
                            if (recordDeleted == 0 && errorFound == 0)
                                Log.Info("No record to delete for : " + dashboardId);
                            else
                                Log.Error("while deleting custom dashboard error code found : " + errorFound.ToString());
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception getting record from Databases : ", e);
            }
            return deletedCustomDashboard;
        }

        /// <summary>
        /// Adding record int customdahsboardWidget table
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <param name="requestCustomDashboard"></param>
        /// <returns>CustomDashboardWidgets object</returns>
        public static DC.CustomDashboard.CustomDashboardWidgets AddWidgetToCustomDashboard(SqlConnectionInfo connectionInfo, DC.CustomDashboard.CustomDashboardWidgets requestCustomDashboard)
        {
            if (connectionInfo == null)
                throw new NullReferenceException("Connection string is not passed.");
            DC.CustomDashboard.CustomDashboardWidgets insertedCustomDashboardWidget = new DC.CustomDashboard.CustomDashboardWidgets();
            try
            {
                using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, UpsertCustomDashboardWidgetStoredProcedure))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@dashboardID", requestCustomDashboard.relatedCustomDashboardID);
                        command.Parameters.AddWithValue("@widgetName", requestCustomDashboard.WidgetName);
                        command.Parameters.AddWithValue("@widgetTypeID", requestCustomDashboard.WidgetTypeID);
                        command.Parameters.AddWithValue("@metricID", requestCustomDashboard.MetricID);
                        command.Parameters.AddWithValue("@match", requestCustomDashboard.Match);

                        //For One source or multiple source
                        if (requestCustomDashboard.Match == 1 || requestCustomDashboard.Match == 2)
                        {
                            //Pass blank list of Tags

                            command.Parameters.AddWithValue("@tagId", "<Root></Root>");

                            //Pass list of sources as xml to SP
                            XDocument xmlDoc = new XDocument();
                            XElement xElm = new XElement("Root",
                                                from l in requestCustomDashboard.sqlServerId
                                                select new XElement("Source", new XElement("ID", l)
                                                )
                                            );
                            xmlDoc.Add(xElm);
                            command.Parameters.AddWithValue("@list", "N" + xmlDoc.ToString());
                        }
                        //for tags or All source
                        else if (requestCustomDashboard.Match == 3)
                        {
                            //Pass list of Tags as xml to SP
                            XDocument xmlDoc = new XDocument();
                            XElement xElm = new XElement("Root",
                                                from l in requestCustomDashboard.TagId
                                                select new XElement("Tag", new XElement("ID", l)
                                                )
                                            );
                            xmlDoc.Add(xElm);
                            command.Parameters.AddWithValue("@tagId", xmlDoc.ToString().Replace("\r\n", ""));

                            //Pass blank list of Sources
                            command.Parameters.AddWithValue("@list", "<Root></Root>");
                        }
                        else if (requestCustomDashboard.Match == 4)
                        {
                            //Pass blank list of Tags

                            command.Parameters.AddWithValue("@tagId", "<Root></Root>");

                            //Pass blank list of Sources
                            command.Parameters.AddWithValue("@list", "<Root></Root>");
                        }
                        else
                        {
                            Log.Error("value for match supplied is incorrect. It should be between 1 to 4.");
                        }

                        command.Parameters.AddWithValue("@recordTimestamp", DateTime.Now.ToUniversalTime());
                        command.Parameters.Add("@widgetID", SqlDbType.Int);
                        command.Parameters["@widgetID"].Direction = ParameterDirection.Output;


                        int errorFound = Convert.ToInt32(command.ExecuteScalar());
                        if (errorFound == 0)
                        {
                            insertedCustomDashboardWidget = requestCustomDashboard;
                            insertedCustomDashboardWidget.WidgetID = Convert.ToInt32(command.Parameters["@widgetID"].Value);
                            //insertedCustomDashboardWidget.RecordCreatedTimestamp = Convert.ToDateTime(command.Parameters["@RecordCreatedTimestamp"].Value);
                            //insertedCustomDashboardWidget.RecordUpdateDateTimestamp = Convert.ToDateTime(command.Parameters["@RecordCreatedTimestamp"].Value);
                            Log.Error("Custom widget record for - " + insertedCustomDashboardWidget.WidgetID + " created/updated in DB.");
                        }
                        else
                        {
                            Log.Error("while excuting p_UpsertCustomDashboardWidget error code found : " + errorFound.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception getting Widget Types from DB : ", e);
            }
            return insertedCustomDashboardWidget;

        }

        /// <summary>
        /// Delete existing record from customdahsboard table
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        /// <param name="CustomDashboardId">Id of specific CustomDashboard</param>
        /// <returns>CustomDashboard object</returns>
        public static bool DeleteWidgetById(SqlConnectionInfo connectionInfo, int widgetId)
        {
            if (connectionInfo == null)
                throw new NullReferenceException("Connection string is not passed.");
            bool recordDeleted = false;
            try
            {
                using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, DeleteCustomDashboardWidgetRecordStoredProcedure))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@widgetID", widgetId);
                        command.Parameters.Add("@isRecordDeleted", SqlDbType.Bit);
                        command.Parameters["@isRecordDeleted"].Direction = ParameterDirection.Output;


                        command.ExecuteNonQuery();
                        recordDeleted = Convert.ToBoolean(command.Parameters["@isRecordDeleted"].Value);

                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception getting record from Databases : ", e);
            }
            return recordDeleted;
        }


        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To update existing Widget for specific Custom Dashboard for a user
        /// </summary>
        /// <param name="connectionInfo""></param>
        /// <param name="requestCustomDashboard"></param>
        public static CustomDashboardWidgets UpdateCustomDashboardWidget(SqlConnectionInfo connectionInfo, DC.CustomDashboard.CustomDashboardWidgets requestCustomDashboard)
        {
            if (connectionInfo == null || requestCustomDashboard == null)
                throw new NullReferenceException("Connection string or CustomDashboard object is not passed.");
            DC.CustomDashboard.CustomDashboardWidgets updatedCustomDashboardWidget = new DC.CustomDashboard.CustomDashboardWidgets();
            try
            {
                using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, UpsertCustomDashboardWidgetStoredProcedure))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@dashboardID", requestCustomDashboard.relatedCustomDashboardID);
                        command.Parameters.AddWithValue("@widgetName", requestCustomDashboard.WidgetName);
                        command.Parameters.AddWithValue("@widgetTypeID", requestCustomDashboard.WidgetTypeID);
                        command.Parameters.AddWithValue("@metricID", requestCustomDashboard.MetricID);
                        command.Parameters.AddWithValue("@match", requestCustomDashboard.Match);

                        //For One source or multiple source
                        if (requestCustomDashboard.Match == 1 || requestCustomDashboard.Match == 2)
                        {
                            //Pass blank list of Tags

                            command.Parameters.AddWithValue("@tagId", "<Root></Root>");

                            //Pass list of sources as xml to SP
                            XDocument xmlDoc = new XDocument();
                            XElement xElm = new XElement("Root",
                                                from l in requestCustomDashboard.sqlServerId
                                                select new XElement("Source", new XElement("ID", l)
                                                )
                                            );
                            xmlDoc.Add(xElm);
                            command.Parameters.AddWithValue("@list", "N" + xmlDoc.ToString());
                        }
                        //for tags or All source
                        else if (requestCustomDashboard.Match == 3)
                        {
                            //Pass list of Tags as xml to SP
                            XDocument xmlDoc = new XDocument();
                            XElement xElm = new XElement("Root",
                                                from l in requestCustomDashboard.TagId
                                                select new XElement("Tag", new XElement("ID", l)
                                                )
                                            );
                            xmlDoc.Add(xElm);
                            command.Parameters.AddWithValue("@tagId", xmlDoc.ToString().Replace("\r\n", ""));

                            //Pass blank list of Sources
                            command.Parameters.AddWithValue("@list", "<Root></Root>");
                        }
                        else if (requestCustomDashboard.Match == 4)
                        {
                            //Pass blank list of Tags

                            command.Parameters.AddWithValue("@tagId", "<Root></Root>");

                            //Pass blank list of Sources
                            command.Parameters.AddWithValue("@list", "<Root></Root>");
                        }
                        else
                        {
                            Log.Error("value for match supplied is incorrect. It should be between 1 to 4.");
                        }

                        command.Parameters.AddWithValue("@recordTimestamp", DateTime.Now.ToUniversalTime());
                        command.Parameters.Add("@widgetID", SqlDbType.Int);
                        command.Parameters["@widgetID"].Direction = ParameterDirection.InputOutput;
                        command.Parameters["@widgetID"].Value = requestCustomDashboard.WidgetID;


                        int errorFound = Convert.ToInt32(command.ExecuteScalar());
                        if (errorFound == 0)
                        {
                            updatedCustomDashboardWidget = requestCustomDashboard;
                            //updatedCustomDashboardWidget.WidgetID = Convert.ToInt32(command.Parameters["@widgetID"].Value);
                            //insertedCustomDashboardWidget.RecordCreatedTimestamp = Convert.ToDateTime(command.Parameters["@RecordCreatedTimestamp"].Value);
                            //insertedCustomDashboardWidget.RecordUpdateDateTimestamp = Convert.ToDateTime(command.Parameters["@RecordCreatedTimestamp"].Value);
                            Log.Error("Custom widget record for - " + updatedCustomDashboardWidget.WidgetID + " created/updated in DB.");
                        }
                        else
                        {
                            Log.Error("while excuting p_UpsertCustomDashboardWidget error code found : " + errorFound.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception getting Widget Types from DB : ", e);
            }
            return updatedCustomDashboardWidget;
        }

        /// <summary>
        /// Get all records of customdahsboard table
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        /// <param name="connectionInfo">sql connection</param>
        /// <param name="userSID">Id of specific user</param>
        /// <returns></returns>
        public static List<DC.CustomDashboard.CustomDashboard> GetAllCustomDashboards(SqlConnectionInfo connectionInfo, string userSID)
        {
            if (connectionInfo == null)
                throw new NullReferenceException("Connection string is not passed.");
            List<DC.CustomDashboard.CustomDashboard> customDashboardRecords = null;
            DC.CustomDashboard.CustomDashboard tempDashboard = null;
            try
            {
                if (connectionInfo != null)
                {
                    using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                    {
                        connection.Open();

                        using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetCustomDashboardsStoredProcedure, userSID))
                        {
                            customDashboardRecords = new List<DC.CustomDashboard.CustomDashboard>();
                            while (reader.Read())
                            {
                                tempDashboard = new DC.CustomDashboard.CustomDashboard();
                                if (reader.IsDBNull(reader.GetOrdinal("CustomDashboardId")) == false)
                                {
                                    tempDashboard.CustomDashboardId =
                                        (long)GetValueOrDefault<long>(reader["CustomDashboardId"]);
                                }

                                //tempDashboard.CustomDashboardId = Convert.ToInt64(reader["CustomDashboardId"]);
                                tempDashboard.CustomDashboardName = (string)GetValueOrDefault<string>(reader["CustomDashboardName"]);
                                //tempDashboard.CustomDashboardName = reader["CustomDashboardName"].ToString();
                                if (reader.IsDBNull(reader.GetOrdinal("IsDefaultOnUI")) == false)
                                {
                                    tempDashboard.IsDefaultOnUI =
                                        (bool)GetValueOrDefault<bool>(reader["IsDefaultOnUI"]);
                                }
                                //tempDashboard.IsDefaultOnUI = Convert.ToBoolean(reader["IsDefaultOnUI"]);
                                //tempDashboard.RecordCreatedTimestamp = Convert.ToDateTime(reader["RecordCreatedTimestamp"]);
                                //tempDashboard.RecordUpdateDateTimestamp = Convert.ToDateTime(reader["RecordUpdateDateTimestamp"]);
                                tempDashboard.UserSID = userSID;

                                //SQLdm 10.0 srishti purohit -While editing custom dashboard Tags field will get inserted
                                tempDashboard.TagsDashboard = new List<string>();
                                if (!string.IsNullOrWhiteSpace(reader["Tags"].ToString()))
                                    tempDashboard.TagsDashboard = reader["Tags"].ToString().Split(',').ToList();


                                customDashboardRecords.Add(tempDashboard);
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception getting record from Databases : ", e);
            }
            return customDashboardRecords;
        }

        /// <summary>
        /// Get all records of widgets for a customdahsboard entry
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        /// <param name="connectionInfo">sql connection</param>
        /// <param name="dashboardId">Id of specific dhsboard</param>
        /// <returns></returns>
        public static List<DC.CustomDashboard.CustomDashboardWidgets> GetAllWidgets(SqlConnectionInfo connectionInfo, int dashboardId, UserToken userToken)
        {
            if (connectionInfo == null)
                throw new NullReferenceException("Connection string is not passed.");
            List<DC.CustomDashboard.CustomDashboardWidgets> customDashboardWidgetRecords = null;
            DC.CustomDashboard.CustomDashboardWidgets tempWidget = null;
            try
            {
                if (connectionInfo != null)
                {
                    using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                    {
                        connection.Open();

                        using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetWidgetsofDashboardStoredProcedure, dashboardId))
                        {
                            customDashboardWidgetRecords = new List<DC.CustomDashboard.CustomDashboardWidgets>();
                            while (reader.Read())
                            {
                                tempWidget = new DC.CustomDashboard.CustomDashboardWidgets();
                                if (reader.IsDBNull(reader.GetOrdinal("WidgetID")) == false)
                                {
                                    tempWidget.WidgetID =
                                        (long)GetValueOrDefault<long>(reader["WidgetID"]);
                                }
                                //tempWidget.WidgetID = Convert.ToInt64(reader["WidgetID"]);
                                tempWidget.relatedCustomDashboardID = dashboardId;
                                tempWidget.WidgetName = (string)GetValueOrDefault<string>(reader["WidgetName"]);

                                //tempWidget.WidgetName = reader["WidgetName"].ToString();
                                if (reader.IsDBNull(reader.GetOrdinal("WidgetTypeID")) == false)
                                {
                                    tempWidget.WidgetTypeID =
                                        (int)GetValueOrDefault<int>(reader["WidgetTypeID"]);
                                }
                                //tempWidget.WidgetTypeID = Convert.ToInt64(reader["WidgetTypeID"]);
                                if (reader.IsDBNull(reader.GetOrdinal("MetricID")) == false)
                                {
                                    tempWidget.MetricID =
                                        (int)GetValueOrDefault<int>(reader["MetricID"]);
                                }
                                //tempWidget.MetricID = Convert.ToInt64(reader["MetricID"]);
                                if (reader.IsDBNull(reader.GetOrdinal("MatchId")) == false)
                                {
                                    tempWidget.Match =
                                        (int)GetValueOrDefault<int>(reader["MatchId"]);
                                }
                                //tempWidget.Match = Convert.ToInt64(reader["MatchId"]);
                                if (tempWidget.Match == 3)
                                {
                                    tempWidget.TagId = ((string)GetValueOrDefault<string>(reader["TagIds"])).Split(',').Select(int.Parse).ToList();
                                }
                                else
                                    tempWidget.TagId = null;

                                //tempWidget.RecordCreatedTimestamp = Convert.ToDateTime(reader["RecordCreatedTimestamp"]);
                                //tempWidget.RecordUpdateDateTimestamp = Convert.ToDateTime(reader["RecordUpdateDateTimestamp"]);
                                if (tempWidget.Match != 3)
                                {
                                    //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                                    List<int> sqlServerIdList = ((string)GetValueOrDefault<string>(reader["SourceIds"])).Split(',').Select(t => int.Parse(t)).ToList();
                                    sqlServerIdList = sqlServerIdList.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x)).ToList();
                                    tempWidget.sqlServerId = sqlServerIdList;
                                    //END
                                    //tempWidget.sqlServerId = ((string)GetValueOrDefault<string>(reader["SourceIds"])).Split(',').Select(t => int.Parse(t)).ToList();
                                }
                                else
                                    tempWidget.sqlServerId = null;

                                customDashboardWidgetRecords.Add(tempWidget);
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception getting all widgets from Databases : ", e);
            }
            return customDashboardWidgetRecords;
        }

        /// <summary>
        /// Get Metric values for widgets
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <param name="dashboardId"></param>
        /// <param name="widgetId"></param>
        /// <param name="startDateTimeInUTC"></param>
        /// <param name="endDateTimeInUTC"></param>
        public static List<DC.CustomDashboard.MetricValueforCustomDashboard> GetAllMetricValuesForCustomWidget(SqlConnectionInfo connectionInfo, int dashboardId, int widgetId, string timeZoneOffset, DateTime startDateTimeInUTC, DateTime endDateTimeInUTC, UserToken userToken)
        {
            if (connectionInfo == null)
            {
                throw new NullReferenceException("Connection string is not passed.");
            }
            List<DC.CustomDashboard.MetricValueforCustomDashboard> customDashboardMetricvaluesList = null;
            int ordinal = -1;
            int indexInstance = -1;
            try
            {
                //startDateTimeInUTC = DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(startDateTimeInUTC)), DateTimeKind.Utc);
                //endDateTimeInUTC = DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(endDateTimeInUTC)), DateTimeKind.Utc);
                using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();

                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetMetricsHistoryForCustomDashboardStoredProcedure, dashboardId, widgetId, startDateTimeInUTC, endDateTimeInUTC))
                    {
                        MetricValueforCustomDashboard customDashboardMetricvalues = null;
                        customDashboardMetricvaluesList = new List<MetricValueforCustomDashboard>();
                        while (reader.Read())
                        {
                            ordinal = reader.GetOrdinal("ServerId");

                            if (!(reader.GetValue(ordinal) == DBNull.Value))
                            {
                                if ((int)(reader.GetValue(ordinal)) == 0)
                                    throw new Exception("Invalid inputs. Please check widget ID.");
                                indexInstance = customDashboardMetricvaluesList.FindIndex(item => item.InstanceName.Equals(reader.GetValue(reader.GetOrdinal("instanceName")).ToString()));
                                if (indexInstance >= 0)
                                {
                                    customDashboardMetricvalues = customDashboardMetricvaluesList[indexInstance];
                                }
                                else
                                {
                                    customDashboardMetricvalues = new MetricValueforCustomDashboard();
                                    //customDashboardMetricvalues.ServerID = (int)(reader.GetValue(ordinal));
                                    customDashboardMetricvalues.MetricValuesforInstance = new List<MetricValuesforInstance>();
                                    ordinal = reader.GetOrdinal("ServerId");
                                    if (!(reader.GetValue(ordinal) == DBNull.Value))
                                    {
                                        customDashboardMetricvalues.ServerID = (int)(reader.GetValue(ordinal));
                                    }

                                    customDashboardMetricvalues.InstanceName = reader.GetValue(reader.GetOrdinal("instanceName")).ToString();
                                    customDashboardMetricvalues.CustomDashboardId = dashboardId;
                                    customDashboardMetricvalues.WidgetID = widgetId;

                                }
                            }
                            DateTime UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(reader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time


                            ordinal = reader.GetOrdinal("Value");

                            if (!(reader.GetValue(ordinal) == DBNull.Value) && !(reader.GetValue(ordinal).ToString() == "0"))
                            {
                                customDashboardMetricvalues.MetricValuesforInstance.Add(new MetricValuesforInstance(UTCCollectionDateTime, Convert.ToDouble(reader.GetValue(ordinal))));
                            }
                            else
                                customDashboardMetricvalues.MetricValuesforInstance.Add(new MetricValuesforInstance(UTCCollectionDateTime, 0));
                            if (!customDashboardMetricvaluesList.Contains(customDashboardMetricvalues))
                                customDashboardMetricvaluesList.Add(customDashboardMetricvalues);
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("Getting of Metric Values for custom dashboard widgets failed. error details: " + exception.Message + "::" + ((exception.InnerException != null) ? exception.InnerException.Message : string.Empty));
            }
            //SQLdm 10.0 (Swati Gogia): Implemented for instance level security.
            return customDashboardMetricvaluesList.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x.ServerID)).ToList();
            //return customDashboardMetricvaluesList;
        }

        #endregion

        #region LicenseManager Integration
        //[START] SQLdm 10.0 (Rajesh Gupta): LM 2.0 Integration - Helpers for Implementing ILicenseManager Interface
        private static object _lock = new object();

        public static void AddLicenseKeys(SqlTransaction transaction, IEnumerable<string> addList)
        {
            // using (LOG.DebugCall())
            {
                // See what errors exist before and after adding the new keys.
                // We will add the new keys if doing so doesn't add a new error.

                List<string> keys = new List<string>();
                //  LicenseSummary summary1 = GetLicenseSummary(transaction, transaction.Connection, keys);
                var summary1 = RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo);
                keys.AddRange(addList);
                LicenseSummary summary2 = LicenseSummary.SummarizeKeys(
                    summary1.MonitoredServers,
                    summary1.Repository,
                    keys);

                // See if we found a different bad key string.
                if (summary2.BadKey != null &&
                    (summary1.BadKey == null || !object.ReferenceEquals(summary1.BadKey.KeyString, summary2.BadKey.KeyString))) //
                {
                    throw new LicenseManagerException(string.Format("Can't add key '" + summary2.BadKey.KeyString + "'.  " + summary2.BadKey.Comment));
                    //throw new ServiceException("Can't add key '" + summary2.BadKey.KeyString + "'.  " + summary2.BadKey.Comment);
                }
                else
                {
                    AddKeysUnchecked(transaction, transaction.Connection, addList);
                }
            }
        }
        public static void AddKeysUnchecked(SqlTransaction transaction, SqlConnection connection, IEnumerable<string> keyList)
        {
            using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_AddLicenseKey"))
            {
                command.Transaction = transaction;
                foreach (string key in keyList)
                {
                    if (key != null)
                    {
                        SqlHelper.AssignParameterValues(command.Parameters, key, DBNull.Value);
                        command.ExecuteNonQuery();
                        // don't really care about returned id
                    }
                }
            }
        }
        public static void RemoveLicenseKeys(SqlTransaction transaction, IEnumerable<string> removeKeys)
        {
            //using (LOG.DebugCall())
            {
                List<string> currentKeys = new List<string>();
                var summary = RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo);
                foreach (var item in summary.CheckedKeys)
                {
                    currentKeys.Add(item.KeyString);
                }
                // If we already have a violation, skip checking.
                if (summary.Status == LicenseStatus.OK)
                {
                    //LOG.Debug("No violation with current keys.");
                    // There is currently no violation.
                    // See if removing the specified keys will cause a violation.
                    // First determine what the new list will be after removing
                    // the specified keys.  We use exact string matching here
                    // because the stored proc that deletes keys also works that way.
                    foreach (string toRemove in removeKeys) currentKeys.Remove(toRemove);

                    summary = LicenseSummary.SummarizeKeys(
                        summary.MonitoredServers,
                        summary.Repository,
                        currentKeys);

                    switch (summary.Status)
                    {
                        case LicenseStatus.CountExceeded:
                            throw new LicenseManagerException("Removing the specified key(s) would make the number of licensed servers less than the number of monitored servers.");
                        case LicenseStatus.Expired:
                            throw new LicenseManagerException("Removing the specified key(s) would result in an expired license.");
                        case LicenseStatus.NoValidKeys:
                            throw new LicenseManagerException("Removing the specified key(s) would leave no valid keys.");
                    }
                }

                // If we get this far, remove the keys.
                using (SqlCommand command = SqlHelper.CreateCommand(transaction.Connection, "p_DeleteLicenseKey"))
                {
                    command.Transaction = transaction;
                    foreach (string key in removeKeys)
                    {
                        SqlHelper.AssignParameterValues(command.Parameters, key);
                        command.ExecuteNonQuery();
                        // don't really care about returned id
                    }
                }
            }
        }
        public static void ReplaceLicenseKeys(SqlTransaction transaction, IEnumerable<string> keyList)
        {
            //using (LOG.DebugCall())
            {
                if (keyList == null)
                    throw new LicenseManagerException("The list of keys is null.");

                // We really need the keys in a List<string>.
                List<string> newKeys = keyList as List<string>;
                if (newKeys == null) newKeys = new List<string>(keyList);

                if (newKeys.Count == 0)
                    throw new LicenseManagerException("The list of keys is empty.");

                var summary = RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo);

                if (summary.BadKey != null)
                {
                    // All new keys must be completely valid.
                    throw new LicenseManagerException(string.Format("Can't add key '" + summary.BadKey.KeyString + "'.  " + summary.BadKey.Comment));
                }
                else
                {
                    switch (summary.Status)
                    {
                        case LicenseStatus.OK:
                            // delete all the license keys
                            SqlHelper.ExecuteNonQuery(transaction, "p_DeleteLicenseKey", DBNull.Value);
                            // add all keys in the newKeys to the database
                            AddKeysUnchecked(transaction, transaction.Connection, newKeys);
                            break;
                        case LicenseStatus.CountExceeded:
                            throw new LicenseManagerException("The number of currently monitored servers is greater than the number allowed by the specified key(s).");
                        //throw new ServiceException("The number of currently monitored servers is greater than the number allowed by the specified key(s).");
                        default:
                            // Other conditions should be impossible due to earlier check of summary.BadKey.
                            throw new LicenseManagerException("Internal error in license check.");
                            //throw new ServiceException("Internal error in license check.");
                    }
                }
            }
        }
        public static LicenseSummary SetLicenseKeys(LicenseKeyOperation operation, IEnumerable<string> keyList)
        {
            //using (LOG.DebugCall())
            {
                //LOG.Debug("operation = ", operation);

                using (SqlConnection connection = RestServiceConfiguration.SQLConnectInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {

                    connection.Open();
                    lock (_lock)
                    {
                        SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
                        try
                        {
                            switch (operation)
                            {
                                case LicenseKeyOperation.Add:
                                    AddLicenseKeys(transaction, keyList);
                                    break;
                                case LicenseKeyOperation.Remove:
                                    RemoveLicenseKeys(transaction, keyList);
                                    break;
                                case LicenseKeyOperation.Replace:
                                    ReplaceLicenseKeys(transaction, keyList);
                                    break;
                            }

                            #region Change Log

                            //MAuditingEngine.Instance.LogAction(new LicenseAction());

                            #endregion Change Log

                            var newLicense = RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo);

                            transaction.Commit();
                            // CheckLicense(true, newLicense);

                            /*if (!newLicense.IsTrial)
                            {
                                //We need to mark the desktop client machine as well as the management service machine
                                RegistryKey rk = null;
                                RegistryKey rks = null;

                                rk = Registry.LocalMachine;
                                rks = rk.CreateSubKey(@"Software\Idera\SQLdm");
                                rks.SetValue("ConfigInfo", 1, RegistryValueKind.DWord);

                                if (rks != null)
                                    rks.Close();
                                rks = null;

                                if (rk != null)
                                    rk.Close();
                                rk = null;
                            }*/

                            return newLicense;
                        }
                        catch (ServiceException e)
                        {
                            //LOG.Error("ServiceException caught in SetLicenseKeys: ", e);
                            transaction.Rollback();
                            throw;
                        }
                        catch (Exception e)
                        {
                            //LOG.Error("Exception caught in SetLicenseKeys: ", e);
                            transaction.Rollback();
                            throw;
                            //throw new ServiceException(e, Status.ErrorUnknown);
                        }
                    }
                }
            }
        }
        public static AuditableEntity GetAuditableEntity()
        {

            AuditableEntity auditableEntity = new AuditableEntity();
            auditableEntity.Name = "License Key";

            return auditableEntity;
        }
        public static void SetAuditableEntity(LicenseKeyOperation keyOp, string key, string totalKeys)
        {
            AuditableEntity auditableEntity = GetAuditableEntity();
            auditableEntity.AddMetadataProperty("License Key", key);
            auditableEntity.AddMetadataProperty("Total number of servers that license supports", totalKeys == "-1" ? "Unlimited" : totalKeys);
            auditableEntity.AddMetadataProperty("License operations", keyOp.ToString());

            //AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
            AuditingEngine.SetAuxiliarData("LicenseEntity", auditableEntity);
        }
        public static void AddOrReplaceKey(BBSLic lic, string key)
        {
            //using (Log.InfoCall())
            {

                LicenseKeyOperation keyOp = LicenseKeyOperation.Add;
                var licSummary = RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo);
                List<CheckedKey> keyList = new List<CheckedKey>();
                foreach (CheckedKey item in licSummary.CheckedKeys)
                {
                    keyList.Add(item);
                }
                if (lic.IsTrial && !licSummary.IsTrial)
                {
                    throw new LicenseManagerException("A trial key cannot be entered when a production key exists.");
                }

                if (keyList.Count > 0 && (lic.IsTrial || !lic.IsPermanent))
                {
                    // There can only be one such key.
                    keyOp = LicenseKeyOperation.Replace;
                }

                // Check for duplicate or incompatible keys.
                foreach (CheckedKey item in keyList)
                {
                    CheckedKey listObject = item;
                    if (LicenseSummary.KeysAreEqual(key, listObject.KeyString))
                    {
                        // duplicate keys
                        throw new LicenseManagerException("The specified key is the same as an existing key.");
                    }
                    else if (listObject.KeyObject == null || listObject.KeyObject.IsTrial || !listObject.KeyObject.IsPermanent)
                    {
                        keyOp = LicenseKeyOperation.Replace;
                    }
                }

                // If this key must replace the others, confirm that the user wants to do that. 


                try
                {
                    //to audit licenses replace
                    SetAuditableEntity(keyOp, key, lic.Limit1.ToString());

                    // Send the new license to the server.
                    //Log.Info("KeyOp is " + keyOp);
                    LicenseSummary license = SetLicenseKeys(keyOp, new string[] { key });
                    //Log.Info(keyOp, " operation succeeded");
                    //ShowLicense(license);
                    //textBox1.Text = string.Empty;

                    /*if (!lic.IsTrial)
                    {
                        //We need to mark the desktop client machine as well as the management service machine
                        RegistryKey rk = null;
                        RegistryKey rks = null;

                        rk = Registry.LocalMachine;
                        rks = rk.CreateSubKey(@"Software\Idera\SQLdm");
                        rks.SetValue("ConfigInfo", 1, RegistryValueKind.DWord);

                        if (rks != null)
                            rks.Close();
                        rks = null;

                        if (rk != null)
                            rk.Close();
                        rk = null;
                    }*/

                }
                catch
                {
                    throw;
                }
            }
        }
        public static BBSLic LoadKeyString(string key)
        {
            BBSLic lic = new BBSLic();
            LicErr licErr = lic.LoadKeyString(key);

            switch (licErr)
            {
                case LicErr.OK:
                    return lic;
                case LicErr.FutureKey:
                    throw new LicenseManagerException(string.Format("The specified key has a creation date of {0}.  Keys with future creation dates are not allowed.\n\nKey: {1}", lic.CreationDate, key));
                default:
                    //Log.Info("BBSLic failed to parse license key.  Error: " + licErr);
                    throw new LicenseManagerException("The specified license key is invalid.");
            }
        }
        //[END] SQLdm 10.0 (Rajesh Gupta): LM 2.0 Integration - Helpers for Implementing ILicenseManager Interface
        #endregion

        #region HealthIndex
        /// <summary>
        /// SQLdm 10.1 (srishti purohit) : To get health index coefficients details for a user
        /// </summary>
        /// <param name="connectionInfo""></param>
        public static DC.HealthIndexScaleFactors GetScaleFactors(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            try
            {

                using (SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetScaleFactorsStoredProcedure))
                    {
                        DC.HealthIndexScaleFactors healthIndexScaleFactorsInfo = new DC.HealthIndexScaleFactors();

                        DC.HealthIndexCoefficient healthCoefficient = new DC.HealthIndexCoefficient();

                        // Read in server health coefficients data
                        while (dataReader.Read())
                        {
                            switch ((int)GetValueOrDefault<int>(dataReader["ID"]))
                            {
                                // SQLdm 10.1 (Pulkit Puri)-- implementing health index for severity list
                                case (int)DC.HealthIndexes.Critical:
                                    int HealthIndexCoefficientForCriticalAlertOrdinal = dataReader.GetOrdinal("HealthIndexCoefficientValue");
                                    if (dataReader.IsDBNull(HealthIndexCoefficientForCriticalAlertOrdinal))
                                        healthCoefficient.IsHealthIndexCoefficientForCriticalAlertSet = false;
                                    else
                                        healthCoefficient.IsHealthIndexCoefficientForCriticalAlertSet = true;
                                    healthCoefficient.HealthIndexCoefficientForCriticalAlert = Convert.ToDouble(GetValueOrDefault<double>(dataReader["HealthIndexCoefficientValue"]));
                                    break;

                                case (int)DC.HealthIndexes.Warning:
                                    int HealthIndexCoefficientForWarningAlertOrdinal = dataReader.GetOrdinal("HealthIndexCoefficientValue");
                                    if (dataReader.IsDBNull(HealthIndexCoefficientForWarningAlertOrdinal))
                                        healthCoefficient.IsHealthIndexCoefficientForWarningAlertSet = false;
                                    else
                                        healthCoefficient.IsHealthIndexCoefficientForWarningAlertSet = true;
                                    healthCoefficient.HealthIndexCoefficientForWarningAlert = Convert.ToDouble(GetValueOrDefault<double>(dataReader["HealthIndexCoefficientValue"]));
                                    break;

                                case (int)DC.HealthIndexes.Informational:
                                    int HealthIndexCoefficientForInformationalAlertOrdinal = dataReader.GetOrdinal("HealthIndexCoefficientValue");
                                    if (dataReader.IsDBNull(HealthIndexCoefficientForInformationalAlertOrdinal))
                                        healthCoefficient.IsHealthIndexCoefficientForInformationalAlertSet = false;
                                    else
                                        healthCoefficient.IsHealthIndexCoefficientForInformationalAlertSet = true;
                                    healthCoefficient.HealthIndexCoefficientForInformationalAlert = Convert.ToDouble(GetValueOrDefault<double>(dataReader["HealthIndexCoefficientValue"]));
                                    break;
                                    // SQLdm 10.1 (Pulkit Puri)-- implementing health index for severity list
                            }
                        }
                        healthIndexScaleFactorsInfo.HealthIndexCoefficients = healthCoefficient;
                        dataReader.NextResult();
                        //Read health index factor for instance
                        while (dataReader.Read())
                        {
                            DC.InstanceScaleFoctor instanceCoefficient = new DC.InstanceScaleFoctor();
                            instanceCoefficient.SQLServerId = (int)GetValueOrDefault<int>(dataReader["SQLServerID"]);

                            instanceCoefficient.InstanceName = GetValueOrDefault<string>(dataReader["InstanceName"]) as string;
                            instanceCoefficient.IsActive = (bool)GetValueOrDefault<bool>(dataReader["Active"]);

                            //(start)  SQLdm 10.1 (Pulkit Puri) -- for setting the value of Coefficient IsScaleFactorSet
                            int InstanceHealthScaleFactorOrdinal = dataReader.GetOrdinal("InstanceScaleFactor");
                            if (dataReader.IsDBNull(InstanceHealthScaleFactorOrdinal))
                                instanceCoefficient.IsInstanceHealthScaleFactorSet = false;
                            else
                                instanceCoefficient.IsInstanceHealthScaleFactorSet = true;
                            //(end) SQLdm 10.1 (Pulkit Puri)

                            instanceCoefficient.InstanceHealthScaleFactor = Convert.ToDouble(GetValueOrDefault<double>(dataReader["InstanceScaleFactor"]));

                            healthIndexScaleFactorsInfo.InstanceScaleFactorList.Add(instanceCoefficient);

                        }
                        dataReader.NextResult();
                        // Read health index for all tags
                        while (dataReader.Read())
                        {
                            DC.TagScaleFactor tagCoefficient = new DC.TagScaleFactor();
                            tagCoefficient.TagId = (int)GetValueOrDefault<int>(dataReader["Id"]);
                            tagCoefficient.TagName = GetValueOrDefault<string>(dataReader["Name"]) as string;

                            //(start)  SQLdm 10.1 (Pulkit Puri) -- for setting the value of Coefficient IsTagHealthScaleFactorSet
                            int TagHealthScaleFactorOrdinal = dataReader.GetOrdinal("TagScaleFactor");
                            if (dataReader.IsDBNull(TagHealthScaleFactorOrdinal))
                                tagCoefficient.IsTagHealthScaleFactorSet = false;
                            else
                                tagCoefficient.IsTagHealthScaleFactorSet = true;
                            //(end) SQLdm 10.1 (Pulkit Puri)

                            tagCoefficient.TagHealthScaleFactor = Convert.ToDouble(GetValueOrDefault<double>(dataReader["TagScaleFactor"]));
                            healthIndexScaleFactorsInfo.TagScaleFactorList.Add(tagCoefficient);
                        }
                        Log.Info("GetScaleFactors called. Health index factor got successfully.");
                        return healthIndexScaleFactorsInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("GetScaleFactors call failed with error : ", ex);
                return null;
            }
        }

        /// <summary>
        /// SQLdm 10.1 (srishti purohit) : To update existing health index coefficients for a user
        /// </summary>
        /// <param name="connectionInfo""></param>
        /// <param name="requestCustomDashboard"></param>
        public static void UpdateScaleFactors(SqlConnectionInfo connectionInfo, DC.HealthIndexCoefficient healthCoefficients, IList<DC.InstanceScaleFoctor> updatedInstanceHealthIndex, IList<DC.TagScaleFactor> updatedTagHealthIndex)
        {
            if (connectionInfo == null)//SQLdm 10.1 (Pulkit Puri) fix for SQLDM- 25912
                throw new NullReferenceException("Connection string is not passed.");

            try
            {
                using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, UpdateHealthIndexScaleFactorStoredProcedure))
                    {
                        command.Parameters.Clear();


                        command.Parameters.AddWithValue("@healthIndexCoefficientForCriticalAlert", healthCoefficients.HealthIndexCoefficientForCriticalAlert);
                        command.Parameters.AddWithValue("@healthIndexCoefficientForWarningAlert", healthCoefficients.HealthIndexCoefficientForWarningAlert);
                        command.Parameters.AddWithValue("@healthIndexCoefficientForInformationalAlert", healthCoefficients.HealthIndexCoefficientForInformationalAlert);

                        // Create xml from list of Instances                

                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml("<Root ></Root>");
                        XmlNode rootInner = doc.DocumentElement;
                        if (updatedInstanceHealthIndex != null)//SQLdm 10.1 (Pulkit Puri) fix for SQLDM- 25912
                        {
                            foreach (DC.InstanceScaleFoctor instance in updatedInstanceHealthIndex)
                            {
                                XmlElement elemInner = doc.CreateElement("Instance");
                                XmlAttribute attInner = doc.CreateAttribute("ID");
                                attInner.InnerXml = instance.SQLServerId.ToString();
                                //elemInner.Attributes.Append(attInner);
                                //attInner = doc.CreateAttribute("CriticalScaleFactor");
                                //attInner.InnerXml = instance.HealthIndexCoefficientForCriticalAlert.ToString();
                                //elemInner.Attributes.Append(attInner);
                                //attInner = doc.CreateAttribute("InformationalScaleFactor");
                                //attInner.InnerXml = instance.HealthIndexCoefficientForInformationalAlert.ToString();
                                //elemInner.Attributes.Append(attInner);
                                //attInner = doc.CreateAttribute("WarningScaleFactor");
                                //attInner.InnerXml = instance.HealthIndexCoefficientForWarningAlert.ToString();
                                elemInner.Attributes.Append(attInner);
                                attInner = doc.CreateAttribute("InstanceScaleFactor");
                                attInner.InnerXml = instance.InstanceHealthScaleFactor.ToString();
                                elemInner.Attributes.Append(attInner);
                                rootInner.AppendChild(elemInner);
                            }
                        }
                        command.Parameters.AddWithValue("@instanceHealthIndex", doc.InnerXml);
                        // Create xml from list of Tags                

                        doc = new XmlDocument();
                        doc.LoadXml("<Root ></Root>");
                        rootInner = doc.DocumentElement;
                        if (updatedTagHealthIndex != null)//SQLdm 10.1 (Pulkit Puri) fix for SQLDM- 25912
                        {
                            foreach (DC.TagScaleFactor tag in updatedTagHealthIndex)
                            {
                                XmlElement elemInner = doc.CreateElement("Tag");
                                XmlAttribute attInner = doc.CreateAttribute("ID");
                                attInner.InnerXml = tag.TagId.ToString();
                                elemInner.Attributes.Append(attInner);
                                attInner = doc.CreateAttribute("TagScaleFactor");
                                attInner.InnerXml = tag.TagHealthScaleFactor.ToString();
                                elemInner.Attributes.Append(attInner);
                                rootInner.AppendChild(elemInner);
                            }
                        }
                        command.Parameters.AddWithValue("@tagHealthIndex", doc.InnerXml);

                        int errorFound = Convert.ToInt32(command.ExecuteScalar());
                        if (errorFound == 0)
                        {
                            Log.Error("Health index coefficients updated in DB.");
                        }
                        else
                        {
                            Log.Error("while excuting p_UpdateHealthIndexScaleFactor error code found : " + errorFound.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception updating Helath Index coefficients : ", e);
            }
        }

        #endregion

        #region General
        ///<summary>
        ///Author : Anshika Sharma
        ///Version : SQLdm 10.2
        ///Stores User Session Settings
        ///<param name="Settings"> User Session Settings as a dictionary having "Key" as name of setting and "Value" as value for that setting. </param>
        ///<param name="UserName">Name of User that is Logged in</param>
        ///</summary>
        public static string SetUserSessionSettings(SqlConnectionInfo sqlConnectionInfo, string UserName, Dictionary<string, string> Settings)
        {
            try
            {
                if (sqlConnectionInfo == null)
                {
                    throw new NullReferenceException("Connection not found");
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<Settings ></Settings>");
                XmlNode rootInner = doc.DocumentElement;
                if (Settings != null)
                {
                    foreach (var setting in Settings)
                    {
                        XmlElement elemInner = doc.CreateElement("Setting");
                        XmlAttribute attInnerKey = doc.CreateAttribute("Key");
                        attInnerKey.InnerXml = setting.Key;
                        XmlAttribute attInnerValue = doc.CreateAttribute("Value");
                        attInnerValue.InnerXml = setting.Value;
                        elemInner.Attributes.Append(attInnerKey);
                        elemInner.Attributes.Append(attInnerValue);
                        rootInner.AppendChild(elemInner);
                    }
                }
                using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, SetUserSessionSettingsStoredProcedure))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@settings", doc.InnerXml);
                        command.Parameters.Add("@UserId", UserName);
                        int ErrorFound = Convert.ToInt32(command.ExecuteScalar());
                        if (ErrorFound == 0)
                        {
                            Log.Error("Settings Saved!!");
                            return "Success! Settings Saved!";
                        }
                        else
                        {
                            Log.Error("Error Found when executing : " + SetUserSessionSettingsStoredProcedure);
                            return "Failed! Settings couldn't be saved!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception Found : " + ex);
                return "Failed! Settings couldn't be saved!";
            }

        }

        ///<summary>
        ///Author : Anshika Sharma
        ///Version : SQLdm 10.2
        ///Retrieves User Session Settings
        ///</summary>
        public static Dictionary<string, string> GetUserSessionSettings(SqlConnectionInfo sqlConnectionInfo, string UserID)
        {
            try
            {
                Dictionary<string, string> Settings = new Dictionary<string, string>();
                if (sqlConnectionInfo == null)
                {
                    throw new ArgumentNullException("Connection not Found!");
                }
                using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetUserSessionSettingsStoredProcedure, UserID))
                    {
                        while (reader.Read())
                        {
                            Settings.Add(reader["Key"].ToString(), reader["Value"].ToString());
                        }
                    }
                }
                return Settings;
            }
            catch (Exception ex)
            {
                Log.Error("Exception Occured : " + ex.Message);
                return null;
            }
        }

        #endregion

        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///Retrieves User Session Settings
        ///</summary>
        public static DC.ConsolidatedInstanceOverview GetConsolidatedInstanceOverview(SqlConnectionInfo connectionInfo, String InstanceID)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            try
            {

                using (SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    string analysisID = "-1";
                    DC.ConsolidatedInstanceOverview consolidatedInstanceOverview = new DC.ConsolidatedInstanceOverview();
                    DC.PreviousAnalysisInformation previouslyAnalysisInformation = new DC.PreviousAnalysisInformation();

                    using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetPreviousAnalysisInfoStoredProcedure, InstanceID))
                    {


                        if (dataReader.Read())
                        {
                            previouslyAnalysisInformation.AnalyisType = dataReader["AnalysisType"].ToString();
                            DateTime myTime = (DateTime)GetValueOrDefault<DateTime>(dataReader["UTCAnalysisStartTime"]);
                            previouslyAnalysisInformation.StartedOn = myTime;
                            long sec = long.Parse(dataReader["seconds"].ToString());
                            TimeSpan ts = TimeSpan.FromSeconds(sec);
                            if (ts.ToString().Contains("."))
                            {
                                previouslyAnalysisInformation.Duration = long.Parse(ts.ToString("%d")) * 24 + long.Parse(ts.ToString("%h")) + ":" + ts.ToString(@"mm\:ss");
                            }
                            else
                            {
                                previouslyAnalysisInformation.Duration = ts.ToString(@"hh\:mm\:ss");
                            }
                            analysisID = dataReader["AnalysisID"].ToString();
                            consolidatedInstanceOverview.PreviousAnalysisInformation = previouslyAnalysisInformation;
                        }
                        else
                        {
                            consolidatedInstanceOverview.PreviousAnalysisInformation = null;
                        }

                    }
                    using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetRecommendationSummaryStoredProcedure, analysisID))
                    {
                        int count = 0;
                        while (dataReader.Read())
                        {
                            float priority;

                            DC.RecommendationSummary recommendationSummary = new DC.RecommendationSummary();
                            recommendationSummary.Category = dataReader["CategoryName"].ToString();
                            recommendationSummary.Recommendations = (int)GetValueOrDefault<int>(dataReader["Recommendations"]);
                            if (float.TryParse(dataReader["MAXComputedRankFactor"].ToString(), out priority))
                                recommendationSummary.Priority = priority;
                            consolidatedInstanceOverview.RecommendationSummary.Add(recommendationSummary);
                            count++;
                        }
                        if (count == 1)
                        {
                            consolidatedInstanceOverview.RecommendationSummary.RemoveAt(0);
                        }
                        return consolidatedInstanceOverview;
                    }



                }
            }
            catch (Exception ex)
            {
                Log.Error("GetRecommendations call failed with error : ", ex);
                return null;
            }
        }



        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///Retrieves OS Memory Paging Per Second
        ///</summary>
        public static IList<DC.OSPagesPerSec> GetMemoryOSPagingPerSecond(SqlConnectionInfo connectionInfo, int InstanceID, float timeZoneOffset, string NumHistoryMinutes, DateTime endDate)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            try
            {
                int? min = toMinutes(NumHistoryMinutes);
                DateTime? endDateCanbeNull = toDateTime(endDate, timeZoneOffset);
                using (SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    IList<DC.OSPagesPerSec> MemoryOSPagesPerSec = new List<DC.OSPagesPerSec>();
                    //DC.PreviousAnalysisInformation previouslyAnalysisInformation = new DC.PreviousAnalysisInformation();
                    //NumHistoryMinutes;
                    using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetOSMemoryPagesPerSecondStoredProcedure, InstanceID, endDateCanbeNull, min))
                    {

                        while (dataReader.Read())
                        {
                            DC.OSPagesPerSec OSPagesPerSec = new DC.OSPagesPerSec();
                            OSPagesPerSec.UTCDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(dataReader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset.ToString()); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time
                            float pagespersec;
                            if (float.TryParse(dataReader["PagesPerSecond"].ToString(), out pagespersec))
                            {
                                OSPagesPerSec.PagesPerSec = pagespersec;
                            }
                            else
                            {
                                OSPagesPerSec.PagesPerSec = 0;
                            }
                            MemoryOSPagesPerSec.Add(OSPagesPerSec);
                        }
                        return MemoryOSPagesPerSec;
                    }



                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("GetMemoryOSPagingPerSec call failed with error : {0} \n {1}", ex.Message, ex.StackTrace);
                return null;
            }
        }
        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///Retrieves Database Running Statistics
        ///</summary>
        public static IList<DC.DatabaseRunningStatistics> GetDBStatistics(SqlConnectionInfo connectionInfo, int InstanceID, float timeZoneOffset, string NumHistoryMinutes, DateTime endDate)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            try
            {
                int? min = toMinutes(NumHistoryMinutes);
                DateTime? endDateCanbeNull = toDateTime(endDate, timeZoneOffset);
                using (SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    IList<DC.DatabaseRunningStatistics> DBStatistics = new List<DC.DatabaseRunningStatistics>();
                    using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetDBRunningStatsSecondStoredProcedure, InstanceID, endDateCanbeNull, min))
                    {
                        float time;
                        int localtemp;
                        while (dataReader.Read())
                        {
                            DC.DatabaseRunningStatistics DBStats = new DC.DatabaseRunningStatistics();
                            DBStats.UTCDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(dataReader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset.ToString()); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                            if (!float.TryParse(dataReader["TimeDeltaInSeconds"].ToString(), out time))
                            {
                                time = 1;
                            }
                            if (int.TryParse(dataReader["Transactions"].ToString(), out localtemp))
                            {
                                DBStats.TransactionsPerSec = localtemp / time;
                            }
                            else
                            {
                                DBStats.TransactionsPerSec = 0;
                            }
                            if (int.TryParse(dataReader["LogFlushes"].ToString(), out localtemp))
                            {
                                DBStats.LogflushesPerSec = localtemp / time;
                            }
                            else
                            {
                                DBStats.LogflushesPerSec = 0;
                            }
                            if (int.TryParse(dataReader["NumberReads"].ToString(), out localtemp))
                            {
                                DBStats.NumberReadsPerSec = localtemp / time;
                            }
                            else
                            {
                                DBStats.NumberReadsPerSec = 0;
                            }

                            if (int.TryParse(dataReader["NumberWrites"].ToString(), out localtemp))
                            {
                                DBStats.NumberWritesPerSec = localtemp / time;
                            }
                            else
                            {
                                DBStats.NumberWritesPerSec = 0;
                            }

                            if (int.TryParse(dataReader["IOStallMS"].ToString(), out localtemp))
                            {
                                DBStats.IOStallMSPerSec = localtemp / time;
                            }
                            else
                            {
                                DBStats.IOStallMSPerSec = 0;
                            }
                            if (int.TryParse(dataReader["DatabaseID"].ToString(), out localtemp))
                            {
                                DBStats.DatabaseID = localtemp;
                            }
                            else
                            {
                                DBStats.IOStallMSPerSec = 0;
                            }
                            DBStats.DatabaseName = dataReader["DatabaseName"].ToString();
                            DBStatistics.Add(DBStats);
                        }
                    }
                    return DBStatistics;


                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("GetDBStats call failed with error : {0} \n {1}", ex.Message, ex.StackTrace);
                return null;
            }
        }


        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///For Transformation of minutes and datetime
        ///</summary>
        private static int? toMinutes(String NumHistoryMinutes)
        {
            if (NumHistoryMinutes == null)
            {
                return null;
            }
            else
            {
                return (int.Parse(NumHistoryMinutes));
            }
        }
        private static DateTime? toDateTime(DateTime endDate, float tzo)
        {
            if (endDate == DateTime.MinValue)
            {
                return null;
            }
            else
            {
                DateTime? endDateCanbeNull;
                endDateCanbeNull = endDate;
                endDateCanbeNull = endDateCanbeNull.Value.AddMinutes(tzo * 60);
                return endDateCanbeNull;
            }
        }
        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///Retrieves CPU Statistics
        ///</summary>
        public static IList<DC.CPUStatistics> GetCPUStatistics(SqlConnectionInfo connectionInfo, int InstanceID, float timeZoneOffset, string NumHistoryMinutes, DateTime endDate)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            try
            {
                int? min = toMinutes(NumHistoryMinutes);
                DateTime? endDateCanbeNull = toDateTime(endDate, timeZoneOffset);


                using (SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    IList<DC.CPUStatistics> CPUStatistics = new List<DC.CPUStatistics>();
                    //DC.PreviousAnalysisInformation previouslyAnalysisInformation = new DC.PreviousAnalysisInformation();
                    //NumHistoryMinutes;
                    using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetCPUStatisticsStoredProcedure, InstanceID, endDateCanbeNull, min))
                    {
                        float time;
                        int localtemp;
                        while (dataReader.Read())
                        {
                            DC.CPUStatistics CPUStats = new DC.CPUStatistics();
                            CPUStats.UTCDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(dataReader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset.ToString()); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                            if (!float.TryParse(dataReader["TimeDeltaInSeconds"].ToString(), out time))
                            {
                                time = 1;
                            }
                            if (int.TryParse(dataReader["SQLCompilations"].ToString(), out localtemp))
                            {
                                CPUStats.SQLCompilationsPerSec = localtemp / time;
                            }
                            else
                            {
                                CPUStats.SQLCompilationsPerSec = 0;
                            }
                            if (int.TryParse(dataReader["SQLREcompilations"].ToString(), out localtemp))
                            {
                                CPUStats.SQLRecompilationsPerSec = localtemp / time;
                            }
                            else
                            {
                                CPUStats.SQLRecompilationsPerSec = 0;
                            }
                            if (int.TryParse(dataReader["Batches"].ToString(), out localtemp))
                            {
                                CPUStats.BatchesPerSec = localtemp / time;
                            }
                            else
                            {
                                CPUStats.BatchesPerSec = 0;
                            }

                            if (int.TryParse(dataReader["Transactions"].ToString(), out localtemp))
                            {
                                CPUStats.TransactionsPerSec = localtemp / time;
                            }
                            else
                            {
                                CPUStats.TransactionsPerSec = 0;
                            }

                            CPUStatistics.Add(CPUStats);
                        }
                    }
                    return CPUStatistics;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("GetCPUStats call failed with error : {0} \n {1}", ex.Message, ex.StackTrace);
                return null;
            }
        }


        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///Retrieves Network Statistics
        ///</summary>
        public static IList<DC.NetworkStatistics> GetNetworkStatistics(SqlConnectionInfo connectionInfo, int InstanceID, float timeZoneOffset, string NumHistoryMinutes, DateTime endDate)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            try
            {
                int? min = toMinutes(NumHistoryMinutes);
                DateTime? endDateCanbeNull = toDateTime(endDate, timeZoneOffset);
                using (SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    IList<DC.NetworkStatistics> NetworkStatistics = new List<DC.NetworkStatistics>();
                    using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetNetworkStatisticsStoredProcedure, InstanceID, endDateCanbeNull, min))
                    {
                        float time;
                        int localtemp;
                        while (dataReader.Read())
                        {
                            DC.NetworkStatistics NetworkStats = new DC.NetworkStatistics();
                            NetworkStats.UTCDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(dataReader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset.ToString()); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                            if (!float.TryParse(dataReader["TimeDeltaInSeconds"].ToString(), out time))
                            {
                                time = 1;
                            }
                            if (int.TryParse(dataReader["PacketsSent"].ToString(), out localtemp))
                            {
                                NetworkStats.PacketsSentPerSec = localtemp / time;
                            }
                            else
                            {
                                NetworkStats.PacketsSentPerSec = 0;
                            }
                            if (int.TryParse(dataReader["PacketsReceived"].ToString(), out localtemp))
                            {
                                NetworkStats.PacketsRecievedPerSec = localtemp / time;
                            }
                            else
                            {
                                NetworkStats.PacketsRecievedPerSec = 0;
                            }

                            NetworkStatistics.Add(NetworkStats);
                        }
                    }
                    return NetworkStatistics;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("GetNetworkStats call failed with error : {0} \n {1}", ex.Message, ex.StackTrace);
                return null;
            }
        }

        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///Retrieves Lock Waits Statistics
        ///</summary>
        public static IList<DC.LockWaitsStatistics> GetLockWaitsStatistics(SqlConnectionInfo connectionInfo, int InstanceID, float timeZoneOffset, string NumHistoryMinutes, DateTime endDate)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            try
            {
                int? min = toMinutes(NumHistoryMinutes);
                DateTime? endDateCanbeNull = toDateTime(endDate, timeZoneOffset);

                using (SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetLocksDetailsStoredProcedure, InstanceID, endDateCanbeNull, min))
                    {
                        IList<DC.LockWaitsStatistics> LockWaitStatistics = new List<DC.LockWaitsStatistics>();
                        String st = "";
                        if (!dataReader.NextResult())
                        {
                            return null;
                        }
                        while (dataReader.Read())
                        {
                            DC.LockWaitsStatistics lockwait = new DC.LockWaitsStatistics();
                            object value = dataReader["LockStatistics"];
                            lockwait.UTCDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(dataReader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset.ToString()); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time
                            if (value != DBNull.Value)
                            {
                                LockStatistics lockStatistics = Serialized<LockStatistics>.DeserializeCompressed<LockStatistics>((byte[])value);

                                if (lockStatistics.AllocUnitCounters.WaitTime.HasValue)
                                {
                                    lockwait.AllocUnit = lockStatistics.AllocUnitCounters.WaitTime.Value.TotalMilliseconds;
                                }
                                else
                                {
                                    lockwait.AllocUnit = 0;
                                }

                                if (lockStatistics.ApplicationCounters.WaitTime.HasValue)
                                {
                                    lockwait.Applicataion = lockStatistics.ApplicationCounters.WaitTime.Value.TotalMilliseconds;
                                }
                                else
                                {
                                    lockwait.Applicataion = 0;
                                }
                                if (lockStatistics.DatabaseCounters.WaitTime.HasValue)
                                {
                                    lockwait.Database = lockStatistics.DatabaseCounters.WaitTime.Value.TotalMilliseconds;
                                }
                                else
                                {
                                    lockwait.Database = 0;
                                }
                                if (lockStatistics.ExtentCounters.WaitTime.HasValue)
                                {
                                    lockwait.Extent = lockStatistics.ExtentCounters.WaitTime.Value.TotalMilliseconds;
                                }
                                else
                                {
                                    lockwait.Extent = 0;
                                }

                                if (lockStatistics.FileCounters.WaitTime.HasValue)
                                {
                                    lockwait.File = lockStatistics.FileCounters.WaitTime.Value.TotalMilliseconds;
                                }
                                else
                                {
                                    lockwait.File = 0;
                                }

                                if (lockStatistics.HeapCounters.WaitTime.HasValue)
                                {
                                    lockwait.HoBT = lockStatistics.HeapCounters.WaitTime.Value.TotalMilliseconds;
                                }
                                else
                                {
                                    lockwait.HoBT = 0;
                                }

                                if (lockStatistics.KeyCounters.WaitTime.HasValue)
                                {
                                    lockwait.Key = lockStatistics.KeyCounters.WaitTime.Value.TotalMilliseconds;
                                }
                                else
                                {
                                    lockwait.Key = 0;
                                }

                                if (lockStatistics.KeyCounters.WaitTime.HasValue)
                                {
                                    lockwait.Key = lockStatistics.KeyCounters.WaitTime.Value.TotalMilliseconds;
                                }
                                else
                                {
                                    lockwait.Key = 0;
                                }

                                if (lockStatistics.LatchCounters.WaitTime.HasValue)
                                {
                                    lockwait.Latch = lockStatistics.LatchCounters.WaitTime.Value.TotalMilliseconds;
                                }
                                else
                                {
                                    lockwait.Latch = 0;
                                }

                                if (lockStatistics.MetadataCounters.WaitTime.HasValue)
                                {
                                    lockwait.Metadata = lockStatistics.MetadataCounters.WaitTime.Value.TotalMilliseconds;
                                }
                                else
                                {
                                    lockwait.Metadata = 0;
                                }

                                if (lockStatistics.ObjectCounters.WaitTime.HasValue)
                                {
                                    lockwait.Object = lockStatistics.ObjectCounters.WaitTime.Value.TotalMilliseconds;
                                }
                                else
                                {
                                    lockwait.Object = 0;
                                }

                                if (lockStatistics.PageCounters.WaitTime.HasValue)
                                {
                                    lockwait.Page = lockStatistics.PageCounters.WaitTime.Value.TotalMilliseconds;
                                }
                                else
                                {
                                    lockwait.Page = 0;
                                }

                                if (lockStatistics.RidCounters.WaitTime.HasValue)
                                {
                                    lockwait.RID = lockStatistics.RidCounters.WaitTime.Value.TotalMilliseconds;
                                }
                                else
                                {
                                    lockwait.RID = 0;
                                }

                                if (lockStatistics.TableCounters.WaitTime.HasValue)
                                {
                                    lockwait.Table = lockStatistics.TableCounters.WaitTime.Value.TotalMilliseconds;
                                }
                                else
                                {
                                    lockwait.Table = 0;
                                }

                            }
                            //note keyRID is nothing but Key+RID
                            else lockwait = null;
                            LockWaitStatistics.Add(lockwait);
                        }
                        return LockWaitStatistics;
                    }

                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("GetLockStats call failed with error : {0} \n {1}", ex.Message, ex.StackTrace);
                return null;
            }
        }

        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///Retrieves File Activity STtatistics
        ///</summary>
        public static IList<DC.Category.FileActivityForInstance> FileActivityStatistics(SqlConnectionInfo connectionInfo, int InstanceID, float timeZoneOffset, string NumHistoryMinutes, DateTime endDate)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            try
            {
                int? min = toMinutes(NumHistoryMinutes);
                DateTime? endDateCanbeNull = toDateTime(endDate, timeZoneOffset);
                using (SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    IList<DC.Category.FileActivityForInstance> FileActivityList = new List<DC.Category.FileActivityForInstance>();
                    //DC.PreviousAnalysisInformation previouslyAnalysisInformation = new DC.PreviousAnalysisInformation();
                    //NumHistoryMinutes;
                    using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetFileRWTActivityStoredProcedure, InstanceID, endDateCanbeNull, min))
                    {

                        float time;
                        double temp;
                        while (dataReader.Read())
                        {
                            DC.Category.FileActivityForInstance FileActivity = new DC.Category.FileActivityForInstance();
                            FileActivity.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(dataReader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset.ToString()); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time
                            FileActivity.FileName = dataReader["FileName"].ToString();
                            FileActivity.FilePath = dataReader["FilePath"].ToString();
                            FileActivity.FileType = dataReader["FileType"].ToString();
                            FileActivity.Drive = dataReader["DriveName"].ToString();
                            FileActivity.DatebaseName = dataReader["DatabaseName"].ToString();
                            FileActivity.statistics = new DC.Category.FileActivityForInstance.Statistics();

                            if (!float.TryParse(dataReader["TimeDeltaInSeconds"].ToString(), out time))
                            {
                                time = 1;
                            }

                            else
                            {
                                //Added to remove divide by zero error when time from db comes 0
                                if (time == 0)
                                {
                                    FileActivityList.Add(FileActivity);
                                    continue;
                                }
                            }

                            String st = dataReader["Reads"].ToString();
                            if (double.TryParse(dataReader["Reads"].ToString(), out temp))
                            {
                                FileActivity.statistics.FileReadsPerSec = temp / time;
                            }
                            else
                            {
                                FileActivity.statistics.FileReadsPerSec = 0;
                            }
                            if (double.TryParse(dataReader["Writes"].ToString(), out temp))
                            {
                                FileActivity.statistics.FileWritesPerSec = temp / time;
                            }
                            else
                            {
                                FileActivity.statistics.FileWritesPerSec = 0;
                            }
                            FileActivityList.Add(FileActivity);
                        }
                        return FileActivityList;
                    }



                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Get File Stats call failed with error : {0} \n {1}", ex.Message, ex.StackTrace);
                return null;
            }
        }

        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///Retrieves Custom Counter Statistics
        ///</summary>
        public static IList<DC.CustomCounterStats> GetCustomCounterStatistics(SqlConnectionInfo connectionInfo, int InstanceID, float timeZoneOffset, string NumHistoryMinutes, DateTime endDate)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            try
            {
                int? min = toMinutes(NumHistoryMinutes);
                DateTime? endDateCanbeNull = toDateTime(endDate, timeZoneOffset);

                using (SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, CustomCounterStatisticsStoredProcedure, InstanceID, endDateCanbeNull, min))
                    {
                        IList<DC.CustomCounterStats> CustomCounterStatistics = new List<DC.CustomCounterStats>();
                        CalculationType calctype;
                        int type;
                        while (dataReader.Read())
                        {
                            DC.CustomCounterStats CustomCounter = new DC.CustomCounterStats();
                            CustomCounter.name = dataReader["Name"].ToString();

                            CustomCounter.UTCDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(dataReader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset.ToString()); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time

                            if (int.TryParse(dataReader["CalculationType"].ToString(), out type))
                            {
                                calctype = (CalculationType)Enum.ToObject(typeof(CalculationType), type);
                            }
                            else
                            {
                                CustomCounter.Value = 0;
                                CustomCounterStatistics.Add(CustomCounter);
                                continue;
                            }
                            if (calctype.Equals(CalculationType.Value))
                            {

                                if (dataReader["RawValue"] != null && dataReader["Scale"] != null)
                                {
                                    decimal temp;
                                    if (decimal.TryParse(dataReader["RawValue"].ToString(), out temp))
                                    {
                                        CustomCounter.Value = temp;
                                        if (decimal.TryParse(dataReader["Scale"].ToString(), out temp))
                                        {
                                            CustomCounter.Value = CustomCounter.Value * temp;
                                        }
                                        else
                                        {
                                            CustomCounter.Value = 0;
                                        }
                                    }
                                    else
                                    {
                                        CustomCounter.Value = 0;
                                    }
                                }
                                else
                                {
                                    CustomCounter.Value = 0;
                                }

                            }
                            else
                            {
                                if (dataReader["RawValue"] != null && dataReader["Scale"] != null)
                                {
                                    decimal temp;
                                    if (decimal.TryParse(dataReader["DeltaValue"].ToString(), out temp))
                                    {
                                        CustomCounter.Value = temp;
                                        if (decimal.TryParse(dataReader["Scale"].ToString(), out temp))
                                        {
                                            CustomCounter.Value = CustomCounter.Value * temp;
                                            if (decimal.TryParse(dataReader["TimeDeltaInSeconds"].ToString(), out temp))
                                            {
                                                if (temp > 0)
                                                {
                                                    CustomCounter.Value = CustomCounter.Value / temp;
                                                }

                                            }

                                        }
                                        else
                                        {
                                            CustomCounter.Value = 0;
                                        }
                                    }
                                    else
                                    {
                                        CustomCounter.Value = 0;
                                    }
                                }
                            }
                            CustomCounterStatistics.Add(CustomCounter);
                        }
                        return CustomCounterStatistics;

                    }


                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("GetCustomCounterStats call failed with error : {0} \n {1}", ex.Message, ex.StackTrace);
                return null;
            }
        }

        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///Retrieves Sever Waits Statistics For Dashboard
        ///</summary>
        public static IList<DC.ServerWaitsDashboard> GetServerWaitsDashboard(SqlConnectionInfo connectionInfo, int InstanceID, float timeZoneOffset, string NumHistoryMinutes, DateTime endDate)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            try
            {
                int? min = toMinutes(NumHistoryMinutes);
                DateTime? endDateCanbeNull = toDateTime(endDate, timeZoneOffset);

                using (SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetServerWaitsDashboardStoredProcedure, InstanceID, endDateCanbeNull, min))
                    {
                        IList<DC.ServerWaitsDashboard> ServerWaitsList = new List<DC.ServerWaitsDashboard>();
                        decimal TimeDeltaInSeconds;
                        decimal temp;
                        while (dataReader.Read())
                        {
                            DC.ServerWaitsDashboard ServerWait = new DC.ServerWaitsDashboard();
                            ServerWait.Category = dataReader["Category"].ToString();

                            ServerWait.UTCDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(dataReader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset.ToString()); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time
                            if (!decimal.TryParse(dataReader["TimeDeltaInSeconds"].ToString(), out TimeDeltaInSeconds))
                            {
                                TimeDeltaInSeconds = 1;
                            }
                            if (decimal.TryParse(dataReader["WaitMilliSeconds"].ToString(), out temp))
                            {
                                ServerWait.TotalWaitMillisecondsPerSecond = temp;
                            }
                            else
                            {
                                ServerWait.TotalWaitMillisecondsPerSecond = 0;
                            }
                            if (decimal.TryParse(dataReader["SignalWaitMilliSeconds"].ToString(), out temp))
                            {
                                ServerWait.SignalWaitMillisecondsPerSecond = temp / TimeDeltaInSeconds;
                            }
                            else
                            {
                                ServerWait.SignalWaitMillisecondsPerSecond = 0;
                            }
                            if (decimal.TryParse(dataReader["ResourceWaitMilliSeconds"].ToString(), out temp))
                            {
                                ServerWait.ResourceWaitMillisecondsPerSecond = temp / TimeDeltaInSeconds;
                            }
                            else
                            {
                                ServerWait.ResourceWaitMillisecondsPerSecond = 0;
                            }

                            ServerWaitsList.Add(ServerWait);
                        }
                        return ServerWaitsList;

                    }


                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("GetServerWaits call failed with error : {0} \n {1}", ex.Message, ex.StackTrace);
                return null;
            }
        }


        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///Retrieves Virtualization Statistics
        ///</summary>
        public static DC.VirtualizationList GetVirtualizationStats(SqlConnectionInfo connectionInfo, int InstanceID, float timeZoneOffset, string NumHistoryMinutes, DateTime endDate)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            try
            {
                int? min = toMinutes(NumHistoryMinutes);
                if (endDate == DateTime.MinValue)
                {
                    endDate = DateTime.UtcNow;
                }
                else
                {
                    endDate = DateTimeHelper.ConvertToUTC(endDate, timeZoneOffset.ToString());
                }

                DateTime startDate;
                if (min != null)
                {
                    startDate = endDate.AddMinutes(-1 * (double)min);
                }
                else
                {
                    startDate = endDate;
                }
                using (SqlConnection connection =
                        connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
                {
                    using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetVirtualizationStatisticsStoredProcedure, InstanceID.ToString(), startDate, endDate))
                    {
                        DC.VirtualizationList VirtualizationList = new DC.VirtualizationList();
                        decimal temp;

                        while (dataReader.Read())
                        {
                            DC.VirtualizationStats Virtualization = new DC.VirtualizationStats();

                            VirtualizationList.type = dataReader["ServerType"].ToString();
                            Virtualization.UTCCollectionDateTime = DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind(((DateTime)GetValueOrDefault<DateTime>(dataReader["UTCCollectionDateTime"])), DateTimeKind.Utc)), timeZoneOffset.ToString()); //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time
                            if (decimal.TryParse(dataReader["VmDiskRead"].ToString(), out temp))
                            {
                                Virtualization.VMDiskRead = temp;
                            }
                            if (decimal.TryParse(dataReader["VmDiskWrite"].ToString(), out temp))
                            {
                                Virtualization.VMDiskWrite = temp;
                            }
                            if (decimal.TryParse(dataReader["ESXDiskRead"].ToString(), out temp))
                            {
                                Virtualization.ESXDiskWrite = temp;
                            }
                            if (decimal.TryParse(dataReader["ESXDiskWrite"].ToString(), out temp))
                            {
                                Virtualization.ESXDiskWrite = temp;
                            }
                            if (decimal.TryParse(dataReader["VmAvailableByte"].ToString(), out temp))
                            {
                                Virtualization.VMAvailableByte = temp;
                            }
                            if (decimal.TryParse(dataReader["ESXAvailableMemBytes"].ToString(), out temp))
                            {
                                Virtualization.ESXAvailableMemBytes = temp;
                            }
                            if (decimal.TryParse(dataReader["VmMemGrantedMB"].ToString(), out temp))
                            {
                                Virtualization.VMMemGrantedMB = temp;
                            }
                            if (decimal.TryParse(dataReader["VmMemBaloonedMB"].ToString(), out temp))
                            {
                                Virtualization.VMMemBaloonedMB = temp;
                            }
                            if (decimal.TryParse(dataReader["VmMemActiveMB"].ToString(), out temp))
                            {
                                Virtualization.VMMemActiveMB = temp;
                            }
                            if (decimal.TryParse(dataReader["VmMemConsumedMB"].ToString(), out temp))
                            {
                                Virtualization.VMMemConsumedMB = temp;
                            }

                            if (decimal.TryParse(dataReader["ESXMemGrantedMB"].ToString(), out temp))
                            {
                                Virtualization.ESXMemGrantedMB = temp;
                            }
                            if (decimal.TryParse(dataReader["ESXMemBaloonedMB"].ToString(), out temp))
                            {
                                Virtualization.ESXMemBaloonedMB = temp;
                            }
                            if (decimal.TryParse(dataReader["ESXMemActiveMB"].ToString(), out temp))
                            {
                                Virtualization.ESXMemActiveMB = temp;
                            }
                            if (decimal.TryParse(dataReader["ESXMemConsumedMB"].ToString(), out temp))
                            {
                                Virtualization.ESXMemConsumedMB = temp;
                            }
                            VirtualizationList.VirtualizationStats.Add(Virtualization);

                        }
                        return VirtualizationList;

                    }


                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("GetVirtualizationStats call failed with error : {0} \n {1}", ex.Message, ex.StackTrace);
                return null;
            }
        }

        public static string AddAlertAdvanceFilter(SqlConnectionInfo sqlConnectionInfo, string filterName, string filterConfig)
        {
            try
            {
                if (sqlConnectionInfo == null)
                {
                    throw new NullReferenceException("Connection not found");
                }


                using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, AddAlertsAdvFilterStoredProcedure))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@FilterName", filterName);
                        command.Parameters.AddWithValue("@Config", filterConfig);
                        int ErrorFound = Convert.ToInt32(command.ExecuteScalar());
                        if (ErrorFound == 0)
                        {
                            Log.Debug("Filter Config Saved!!");
                            return "Success! Filter Config Saved!";
                        }
                        else
                        {
                            Log.Error("Error Found when executing : " + AddAlertsAdvFilterStoredProcedure);
                            return "Failed! Settings couldn't be saved!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception Found : " + ex);
                return "Failed! Settings couldn't be saved!";
            }

        }

        public static string DeleteAlertAdvanceFilter(SqlConnectionInfo sqlConnectionInfo, string filterName)
        {
            try
            {
                if (sqlConnectionInfo == null)
                {
                    throw new NullReferenceException("Connection not found");
                }

                using (SqlConnection connection = sqlConnectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    connection.Open();
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, DeleteAlertsAdvFilterStoredProcedure))
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@FilterName", filterName);
                        int ErrorFound = Convert.ToInt32(command.ExecuteScalar());
                        if (ErrorFound == 0)
                        {
                            Log.Debug("Filter Config Deleted!!");
                            return "Success! Filter Deleted!";
                        }
                        else
                        {
                            Log.Error("Error Found when executing : " + DeleteAlertsAdvFilterStoredProcedure);
                            return "Failed! Filter couldn't be deletedd!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception Found : " + ex);
                return "Failed! Filter couldn't be deletedd!";
            }

        }

        public static List<AdvanceFilter> GetAlertsAdvanceFilters(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            try
            {
                using (SqlConnection connection =
                        connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetAllAlertsAdvFilterStoredProcedure))
                    {
                        List<AdvanceFilter> advanceFilters = new List<AdvanceFilter>();
                        while (dataReader.Read())
                        {
                            AdvanceFilter advFilter = new AdvanceFilter();
                            advFilter.filterName = dataReader["FilterName"].ToString();
                            string filterConfig = dataReader["Config"].ToString();
                            advFilter.filterConfig = JsonHelper.FromJSON<List<AlertsGridAdvancedFilterParamFilter>>(filterConfig);


                            advanceFilters.Add(advFilter);
                        }
                        return advanceFilters;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("GetAlertsAdvanceFilter call failed with error : {0} \n {1}", ex.Message, ex.StackTrace);
                return null;
            }
        }

        #region Recommendations
        public static List<DC.Analysis> GetAnalysisListing(SqlConnectionInfo connectionInfo, int instanceID)
        {
            DC.Analysis listOfRecomm = new DC.Analysis();
            List<DC.Analysis> AnalysisListColl = new List<DC.Analysis>();

            try
            {
                using (SqlConnection connection = connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, GetAnalysisListingFromDBStoreProcedure, instanceID))
                    {
                        while (reader.Read())
                        {
                            DC.Analysis anlsList = new DC.Analysis();
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
                    }
                }
                return AnalysisListColl;
            }
            catch (SqlException ex)
            {
                Log.ErrorFormat(ex.Message + "SQL Error occured in GetRecommendations.");
                return null;
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error occured in GetRecommendations : ", ex);
                return null;
            }
        }

        public static List<DC.UIRecommendation> GetRecommendations(
            SqlConnectionInfo connectionInfo, int monitoredServerId, int analysisId
        )
        {
            List<string> blockedRecommendation = SqlHelper.GetBlockedRecommendations(connectionInfo, monitoredServerId);

            List<DC.UIRecommendation> lisOfRecommendattion = new List<DC.UIRecommendation>();
            Recommendation recommendationRow = null;
            try
            {
                using (
                    SqlConnection connection =
                        connectionInfo.GetConnection(Constants.RestServiceConnectionStringApplicationName))
                {
                    using (
                        SqlDataReader reader =
                            SqlHelper.ExecuteReader(connection, GetRecommendationList,
                                                    monitoredServerId,
                                                    analysisId))
                    {
                        Log.Info("Getting all saved recommendations from database history for analysisId : " + analysisId + " .");

                        #region Get Properties

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
                        #endregion

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
                            catch (Exception ex)
                            {
                                Log.ErrorFormat("Error occured in GetRecommendations RecommendationFactory.GetRecommendation: ", ex);
                            }

                            if (recommendationRow != null)
                            {
                                recommendationRow.ID = reader["RecommendationID"].ToString();

                                recommendationRow.ComputedRankFactor = Convert.ToSingle(reader["ComputedRankFactor"]);
                                recommendationRow.AnalysisRecommendationID = Convert.ToInt32(reader["AnalysisRecommendationID"]);
                                recommendationRow.IsFlagged = Convert.ToBoolean(reader["IsFlagged"]);
                                recommendationRow.OptimizationStatus = (RecommendationOptimizationStatus)Convert.ToInt32(reader["OptimizationStatusID"]);
                                recommendationRow.OptimizationErrorMessage = reader["OptimizationErrorMessage"].ToString();

                                if (blockedRecommendation != null && blockedRecommendation.Count > 0)
                                {
                                    if (!blockedRecommendation.Contains(recommendationRow.ID))
                                        lisOfRecommendattion.Add(new DC.UIRecommendation(recommendationRow, recommendationProperties));
                                }
                                else
                                {
                                    lisOfRecommendattion.Add(new DC.UIRecommendation(recommendationRow, recommendationProperties));
                                }
                            }
                            Log.Info("Total recommendations fetched from history records : " + lisOfRecommendattion.Count + " .");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.Error(ex.Message + "SQL Error occured in GetRecommendations.");
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Error occured in GetRecommendations : ", ex);
                throw new Exception(ex.Message);
            }
            return lisOfRecommendattion;
        }
        #endregion
    }
}
