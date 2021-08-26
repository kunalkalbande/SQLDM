using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ComponentModel;
using Idera.SQLdm.Service.DataContracts.v1;
using Idera.SQLdm.Service.DataContracts.v1.Category;
using Idera.SQLdm.Service.DataContracts.v1.Databases;
using Idera.SQLdm.Service.DataContracts.v1.Category.Sessions;
using Idera.SQLdm.Service.Web;
using Idera.SQLdm.Common.Data;


namespace Idera.SQLdm.Service.ServiceContracts.v1
{
    [ServiceContract]
    public interface ICategoryManager    
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/Resources/tzo/{timeZoneOffset}?NumHistoryMinutes={NumHistoryMinutes}&endDate={endDate}&limit={limit}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Resources For Instance")]
        IList<ResourcesForInstance> GetResourcesForInstance(string InstanceId, string timeZoneOffset, int NumHistoryMinutes, DateTime endDate, int limit);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/FileDrives", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get File Drives For Instance")]
        IList<FileDrivesForInstance> GetFileDrivesForInstance(string InstanceId);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/FileActivity/tzo/{timeZoneOffset}?NumHistoryMinutes={NumHistoryMinutes}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get File Activity For Instance")]
        IList<FileActivityForInstance> GetFileActivityForInstance(string InstanceId, string timeZoneOffset, int NumHistoryMinutes);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/ServerWaits/tzo/{timeZoneOffset}?NumHistoryMinutes={NumHistoryMinutes}&endDate={endDate}&CategoryList={category}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Server Waits For Instance")]
        IList<ServerWaitsForInstance> GetServerWaitsForInstance(string InstanceId, string timeZoneOffset, int NumHistoryMinutes, DateTime endDate, string category); //SQLdm 10.2 (Anshika Sharma) : Added EndTime

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/Sessions/tzo/{timeZoneOffset}?limit={limit}&UserSessionsOnly={UserSessionsOnly}&excludeSQLDMSessions={excludeSQLDMSessions}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Sessions For Instance")]
        //IList<SessionsForInstance> GetSessionsForInstance(string InstanceId, DateTime start, DateTime end, int limit, string type);
        IList<SessionsForInstance> GetSessionsForInstance(string InstanceId, string timeZoneOffset, int limit, string UserSessionsOnly, string excludeSQLDMSessions); //SQLdm 8.6 (Ankit Srivastava) -- API Defects - changed param types from bool to string

        /* [OperationContract]
        [DateTimeInspector]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/Sessions/activity?NumHistoryMinutes={NumHistoryMinutes}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Session Activity For Instance")]
        IList<SessionActivityForInstance> GetSessionsActivityForInstance(string InstanceId, int NumHistoryMinutes);*/

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/Sessions/ResponseTime?NumHistoryMinutes={NumHistoryMinutes}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Session Response For Instance")]
        IList<SessionResponseTimeForInstance> GetSessionResponseTimeForInstance(string InstanceId, int NumHistoryMinutes);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/Queries/tzo/{timeZoneOffset}?NumHistoryMinutes={NumHistoryMinutes}&limit={limit}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Queries Stat For Instance")]
        IList<QueryStatisticsForInstance> GetQueryStatsForInstance(string InstanceId, string timeZoneOffset, int NumHistoryMinutes, int limit);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/Queries/Waits/tzo/{timeZoneOffset}?startTime={startDate}&endTime={endDate}&WaitTypeID={WaitTypeID}&WaitCategoryID={WaitCategoryID}&SQLStatementID={SQLStatementID}&ApplicationID={ApplicationID}&DatabaseID={DatabaseID}&HostID={HostID}&SessionID={SessionID}&LoginID={LoginID}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Queries Stat For Instance")]
        IList<QueryWaitStatisticsForInstance> GetQueryWaitStatisticsForInstance(string InstanceId, string timeZoneOffset, DateTime startDate, DateTime endDate, string WaitTypeID, string WaitCategoryID, string SQLStatementID, string ApplicationID, string DatabaseID, string HostID, string SessionID, string LoginID);
        
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/Overview/Queries/Waits/tzo/{timeZoneOffset}?startTime={startDate}&endTime={endDate}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Queries Stat For Instance Overview page. (Filters not applicable on this API. To filter user GetQueryWaitStatisticsForInstance API")]
        IList<QueryWaitStatisticsForInstanceOverview> GetQueryWaitStatisticsForInstanceOverview(string InstanceId, string timeZoneOffset, DateTime startDate, DateTime endDate);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/Databases/{DatabaseId}/CapacityUsage?NumHistoryMinutes={NumHistoryMinutes}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get CapacityUsage For Instance")]
        IList<CapacityUsageForDatabase> GetCapacityUsageForDatabase(string InstanceId, string DatabaseId, int NumHistoryMinutes);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/Databases/Tempdb/tzo/{timeZoneOffset}?NumHistoryMinutes={NumHistoryMinutes}&endDate={endDate}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get TempDB Statistics For Instance")]
        IList<TempDBStats> GetTempDBStatsForInstance(string InstanceId, string timeZoneOffset, int NumHistoryMinutes, DateTime endDate);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/Databases/AvailabilityGroup/tzo/{timeZoneOffset}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("This API is not being used by web UI team. Should be removed in next release as seems to be a dead API now. Please use GetAvailabilityGroupStatsForInstance API for data.")]
        IList<AvailabilityGroupSummaryForDatabase> GetAvailabilityGroupForInstance(string InstanceId, string timeZoneOffset);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/Databases/AvailabilityGroup/Statistics/tzo/{timeZoneOffset}?NumHistoryMinutes={NumHistoryMinutes}&endDate={endDate}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Availability Group Statistics For Instance")]
        IList<AvailabilityGroupSummaryForDatabase> GetAvailabilityGroupStatsForInstance(string InstanceId, string timeZoneOffset, int NumHistoryMinutes,DateTime endDate);

        //SQLdm 8.5 (Ankit Srivastava): for  Categories API-  Session Statistics
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/Sessions/Statistics/tzo/{timeZoneOffset}?NumHistoryMinutes={NumHistoryMinutes}&endDate={endDate}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Availability Group Statistics For Instance")]
        //IList<ServerSessionStatistics> GetSessionStatisticsForInstance(string InstanceId, int NumHistoryMinutes);
        IList<ServerSessionStatistics> GetSessionStatisticsForInstance(string InstanceId, string timeZoneOffset, int NumHistoryMinutes, DateTime endDate);

       /* [OperationContract]
        [DateTimeInspector]
        [WebInvoke(Method = "GET", UriTemplate = "/Instances/{InstanceId}/Metric/{metricId}/MetricHistory?numOfMinInHistory={history}&databaseid={databaseId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Metric History by Metric ID")]
        IList<MetricHistory> GetMetricsHistoryByMetricID(string InstanceId, string metricId, string numOfMinInHistory, string databaseId);
        */

        //SQLdm 9.1 (Sanjali Makkar) (Baseline Statistics) - Adding Baseline Statistics For Metric
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/Metric/{MetricID}/Baseline/tzo/{timeZoneOffset}?NumHistoryMinutes={NumHistoryMinutes}&endDate={endDate}&limit={limit}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Baseline Statistics For Metric")]
        IList<BaselineForMetric> GetBaselineForMetric(string InstanceId, string MetricID, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate, int limit);

        //SQL10.1 Nishant Adhikari
        //Memory OS Paging
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/GetOSPaging/tzo/{timeZoneOffset}?NumHistoryMinutes={NumHistoryMinutes}&endDate={endDate}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get OS Paging per second")]
        IList<OSPagesPerSec> GetMemoryOSPagingPerSecond(string InstanceId, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate);

        //SQL10.1 Nishant Adhikari
        //Database statistics
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/GetDBStats/tzo/{timeZoneOffset}?NumHistoryMinutes={NumHistoryMinutes}&endDate={endDate}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Database statistics")]
        IList<DatabaseRunningStatistics> GetDBStatistics(string InstanceId, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate);

        //SQL10.1 Nishant Adhikari
        //CPU statistics
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/GetCPUStats/tzo/{timeZoneOffset}?NumHistoryMinutes={NumHistoryMinutes}&endDate={endDate}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get CPU statistics")]
        IList<CPUStatistics> GetCPUStatistics(string InstanceId, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate);

        //SQL10.1 Nishant Adhikari
        //Network statistics
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/GetNetworkStats/tzo/{timeZoneOffset}?NumHistoryMinutes={NumHistoryMinutes}&endDate={endDate}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Network statistics")]
        IList<NetworkStatistics> GetNetworkStatistics(string InstanceId, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate);

        //SQL10.1 Nishant Adhikari
        //Lock waits statistics
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/GetLockWaitsStats/tzo/{timeZoneOffset}?NumHistoryMinutes={NumHistoryMinutes}&endDate={endDate}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Lock Waits statistics")]
        IList<LockWaitsStatistics> GetLockWaitsStatistics(string InstanceId, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate);

        //SQL10.1 Nishant Adhikari
        //File Read Write Transfer Activity
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/FileStats/tzo/{timeZoneOffset}?NumHistoryMinutes={NumHistoryMinutes}&endDate={endDate}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get File Activity statistics")]
        IList<FileActivityForInstance> GetFileStatistics(string InstanceId, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate);

        //SQL10.1 Nishant Adhikari
        //Custom Counter statistics
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/GetCustomCounterStats/tzo/{timeZoneOffset}?NumHistoryMinutes={NumHistoryMinutes}&endDate={endDate}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Custom Counter statistics")]
        IList<CustomCounterStats> GetCustomCounterStatistics(string InstanceId, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate);

        //SQL10.1 Nishant Adhikari
        //Server Waits for Dashborad statistics
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/GetServerWaitsDashboard/tzo/{timeZoneOffset}?NumHistoryMinutes={NumHistoryMinutes}&endDate={endDate}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Server Waits statistics")]
        IList<ServerWaitsDashboard> GetServerWaitsDashboard(string InstanceId, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate);

        //SQL10.1 Nishant Adhikari
        //Virtualization Statistics
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/GetVirtualizationStats/tzo/{timeZoneOffset}?NumHistoryMinutes={NumHistoryMinutes}&endDate={endDate}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Virtualization statistics")]
        VirtualizationList GetVirtualizationStats(string InstanceId, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate);

    }
}
