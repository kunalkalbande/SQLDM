using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ComponentModel;
using Idera.SQLdm.Service.DataContracts.v1;
using Idera.SQLdm.Service.DataContracts.v1.Errors;
using Idera.SQLdm.Service.DataContracts.v1.Widgets;
using Idera.SQLdm.Service.Web;

namespace Idera.SQLdm.Service.ServiceContracts.v1
{
    [ServiceContract]
    public interface ITopXManager
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/TopInstanceByResponseTime/tzo/{timeZoneOffset}?limit={count}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Top Instances By Response Time")]
        IList<ResponseTimeForInstance> GetTopInstanceByResponseTime(string timeZoneOffset, int count);

        /*[OperationContract]
        [DateTimeInspector]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/TopServersAlerts?startTime={start}&endDate={end}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Top Servers Alerts")]
        MonitoredSqlServerCollection GetTopServersAlerts(DateTime start, DateTime end);*/

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/GetTopDatabasesByGrowth/tzo/{timeZoneOffset}?limit={count}&numDays={numHistoryDays}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Top Databases By Growth Rate")]
        IList<ProjectedGrowthOfDatabaseSize> GetTopDatabasesByGrowth(string timeZoneOffset, int count, int numHistoryDays);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/TopInstanceByQueryDuration/tzo/{timeZoneOffset}?limit={count}&NumDays={NumDays}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("GetTop Queries By Duration")]
        IList<LongestQueriesForInstance> GetTopQueriesByDuration(string timeZoneOffset, int count, int NumDays);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/TopInstanceByBlockedSessions/tzo/{timeZoneOffset}?limit={count}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Top Blocked Session count")]
        IList<BlockedSessionForInstance> GetBlockedSessionCount(string timeZoneOffset, int count);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/TopInstanceByQueryCount/tzo/{timeZoneOffset}?limit={count}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Top Instances By Query Count")]
        IList<InstancesByQueryCount> GetTopInstancesByQueryCount(string timeZoneOffset, int count);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/TopInstanceBySessionCount/tzo/{timeZoneOffset}?limit={count}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Top Session count")]
        IList<SessionCountForInstance> GetTopInstanceBySessionCount(string timeZoneOffset, int count);

        //SQLdm 8.5 (Ankit Srivastava): for Top X API- Disk Space
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/TopInstanceByDiskSpace/tzo/{timeZoneOffset}?limit={count}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Instances By Disk Space")]
        IList<DiskSpaceByInstance> GetInstancesByDiskSpace(string timeZoneOffset, int count);

        //SQLdm 9.1 (Sanjali Makkar): adding the filter of Instance ID
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/TopSessionsByCPUUsage/tzo/{timeZoneOffset}?InstanceID={InstanceID}&limit={count}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Sessions By CPU Usage")]
        IList<SessionsByCPUUsage> GetTopSessionsByCPUUsage(string timeZoneOffset, string InstanceID, int count);
        
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/TopDatabaseByActivity/tzo/{timeZoneOffset}?limit={count}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Databast By Activity")]
        IList<DatabaseByActivity> GetTopDatabaseByActivity(string timeZoneOffset, int count);

        //SQLdm 8.5 (Ankit Srivastava): for Top X API- Tempdb Utilization
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/InstancesByTempDbUtilization/tzo/{timeZoneOffset}?limit={count}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Instances By TempDb Utilization")]
        IList<TempDBUtilizationForInstance> GetInstancesByTempDbUtilization(string timeZoneOffset, int count);

        //SQLdm 8.5 (Ankit Srivastava): for Top X API- Query Monitor Eventn
        /*[OperationContract]
        [DateTimeInspector]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/InstancesByQueries?startTime={startDate}&endDate={endDate}&limit={count}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Instances By Query Monitor Event")]
        MonitoredSqlServerCollection GetInstancesByQueries(DateTime startDate, DateTime endDate, string count);*/
        
        //SQLdm 8.5 (Gaurav Karwal): for Top X API- List of instances by connection count
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/TopInstancesByConnCount/tzo/{timeZoneOffset}?limit={count}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Instances By Connection Count")]
        List<InstancesByConnectionCount> GetInstancesByConnectionCount(string timeZoneOffset, int count);

        //SQLdm 8.5 (Gaurav Karwal): for Top X API- List of DBs by DataFileSize
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/TopDatabasesBySize/tzo/{timeZoneOffset}?limit={count}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get List of DBs by DataFileSize")]
        List<DatabasesByDatabaseFileSize> GetDatabasesByFileSize(string timeZoneOffset, int count);
        //SQLdm 8.5 (Ankit Srivastava): for Top X API- Instance Alerts
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/ByAlerts?limit={count}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Instances By Instance Alerts")]
        IEnumerable<AlertsCountForInstance> GetInstancesByAlerts(int count);

        //SQLdm 8.5 (Gaurav Karwal): for Top X API- List of instances by wait stats
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/TopInstancesByWaits?limit={count}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get List of instances by wait stats")]
        List<WaitStatisticsByInstance> GetInstancesByWaitStatistics(int count);

        //SQLdm 8.5 (Ankit Srivastava): for Top X API- Most Alerts for Databases
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/Databases/ByAlerts?limit={count}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Databases By most number of alerts")]
        IEnumerable<AlertsCountForDatabase> GetDatabasesByAlerts(int count);
        //SQLdm 8.5 (Ankit Srivastava): for Top X API-  Sql Cpu Load
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/BySqlCpuLoad?limit={count}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Instances by SQL CPU Load")]
        IEnumerable<SqlCpuLoadForInstance> GetInstancesBySqlCpuLoad(int count);

        //SQLdm 8.5 (Ankit Srivastava): for Top X API- IO Physical Count
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/ByIOPhysicalCount?limit={count}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Instances by IO Physical Count")]
        IEnumerable<IOPhysicalUsageForInstance> GetInstancesByIOPhysicalCount(int count);


        //SQLdm 8.5 (Ankit Srivastava): for Top X API-  Sql Memory Usage
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/BySqlMemoryUsage?limit={count}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Instances by SQL Memory Usage")]
        IEnumerable<SqlMemoryUsageForInstance> GetInstancesBySqlMemoryUsage(int count);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "MetricThresholds?metricId={metricId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Instances by SQL Memory Usage")]
        IDictionary<int, decimal[]> getThresholdsForMetric(int metricId);

        //SQLdm 9.1 (Sanjali Makkar): for Top X API- Sessions by I/O Activity
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/Instances/TopSessionsByIOActivity/tzo/{timeZoneOffset}?InstanceID={InstanceID}&limit={limit}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Sessions By I/O Activity")]
        IList<SessionsByIOActivity> GetTopSessionsByIOActivity(string timeZoneOffset, string InstanceID, int limit);
    }
}
