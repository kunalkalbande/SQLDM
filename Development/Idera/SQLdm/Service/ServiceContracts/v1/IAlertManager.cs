using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ComponentModel;
using Idera.SQLdm.Service.DataContracts.v1;
using Idera.SQLdm.Service.Web;

namespace Idera.SQLdm.Service.ServiceContracts.v1
{
    [ServiceContract]
    public interface IAlertManager
    {
        /*[OperationContract]
        [DateTimeInspector]
        [WebInvoke(Method = "GET", UriTemplate = "Alerts?instanceId={instanceId}&startingAlertId={startingAlertId}&startTime={startDate}&endTime={endDate}&severity={severity}&metric={metric}&category={category}&isActive={isActive}&maxRows={maxRows}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get a list of Alerts")]
        IList<Alert> GetAlerts(string instanceId, int startingAlertId, int maxRows, DateTime startDate, DateTime endDate, string severity, string metric, string category, bool isActive);*/

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "AlertsForWebConsole/tzo/{timeZoneOffset}?startTime={startDate}&endTime={endDate}&instanceId={instanceId}&severity={severity}&metric={metric}&category={category}&orderBy={orderBy}&orderType={orderType}&limit={limit}&activeOnly={activeOnly}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get a list of Alerts")]
        IList<Alert> GetAlertsForWebConsole(string timeZoneOffset, DateTime startDate, DateTime endDate, string instanceId, string severity,
            string metric, string category, string orderBy, string orderType, int limit, bool activeOnly);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "AlertsForWebConsoleGrid/tzo/{timeZoneOffset}?startTime={startDate}&endTime={endDate}&instanceId={instanceId}&severity={severity}&metric={metric}&category={category}&orderBy={orderBy}&orderType={orderType}&limit={limit}&activeOnly={activeOnly}&gPage={gPage}&gLimit={gLimit}&filter={advancedFilterParam}&sort={sort}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get a list of Alerts")]
        AlertsV2 GetAlertsForWebConsoleGrid(string timeZoneOffset, DateTime startDate, DateTime endDate, string instanceId, string severity,
            string metric, string category, string orderBy, string orderType, int limit, bool activeOnly, int gPage, int gLimit, string advancedFilterParam, string sort);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "AlertsForWebConsole/abridged/tzo/{timeZoneOffset}?startTime={startDate}&endTime={endDate}&instanceId={instanceId}&severity={severity}&metric={metric}&category={category}&orderBy={orderBy}&orderType={orderType}&limit={limit}&activeOnly={activeOnly}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get a list of Alerts")]
        IList<Alert> GetAlertsForWebConsoleAbridged(string timeZoneOffset, DateTime startDate, DateTime endDate, string instanceId, string severity,
            string metric, string category, string orderBy, string orderType, int limit, bool activeOnly);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Alerts/{alertId}/tzo/{timeZoneOffset}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Alert Details")]
        Alert GetAlertDetails(string alertId, string timeZoneOffset);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Alerts/{alertId}/MetricHistory/tzo/{timeZoneOffset}?numHistoryHours={numHistoryHours}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Alert Details")]
        IList<TimedValue> GetMetricsHistoryForAlert(string alertId, string timeZoneOffset, int numHistoryHours);
        
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Metrics/tzo/{timeZoneOffset}?metricId={metricId}&startTime={startDate}&endTime={endDate}&maxRows={maxRows}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Metrics Details")]
        MetricCollection GetMetricsDetails(string timeZoneOffset, string metricId, int maxRows, DateTime startDate, DateTime endDate);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "numAlertsByCategory?PerInstance={PerInstance}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get alerts counts by category")]
        IList<AlertsByCategory> GetAlertsCountByCategory(bool PerInstance);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "numAlertsByDatabase", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get alerts counts by database")]
        IList<AlertsByDatabase> GetAlertsCountByDatabase();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetAlertsAdvanceFilters", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Alerts Advance Filters")]
        List<AdvanceFilter> GetAlertsAdvanceFilters();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "AddAlertsAdvanceFilter?filterName={filterName}&filterConfig={filterConfig}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Add Alert Advance Filters")]
        void AddAlertsAdvanceFilters(string filterName,string filterConfig);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "DeleteAlertsAdvanceFilter?filterName={filterName}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Delete Alert Advance Filter")]
        void DeleteAlertsAdvanceFilters(string filterName);
    }
}
