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

    // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - exposed web services
    [ServiceContract]
    public interface IQueryManager
    {

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/queries/supportedgrouping", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get a list of Supported Groupings")]
        IList<SupportedGrouping> GetSupportedGroupingsForQueries(); // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - get a list of supported groupings, i.e., group by

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/queries/supportedmetrics", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get a list of Supported Metrics")]
        IList<SupportedMetric> GetSupportedMetricsForQueries();  // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - get a list of supported metrics, i.e., views

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{instanceId}/queries/applications?startlimit={startIndex}&noofrecords={recordsCount}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get a list of applications corresponding to a SQL Server")]
        IList<Application> GetApplicationsForServer(string instanceId, string startIndex, string recordsCount); // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - get a list of applications for a particular instance

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{instanceId}/queries/clients?startlimit={startIndex}&noofrecords={recordsCount}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get a list of clients corresponding to a SQL Server")]
        IList<Client> GetClientsForServer(string instanceId, string startIndex, string recordsCount); // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - get a list of clients (hosts) for a particular instance

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{instanceId}/queries/users?startlimit={startIndex}&noofrecords={recordsCount}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get a list of users corresponding to a SQL Server")]
        IList<User> GetUsersForServer(string instanceId, string startIndex, string recordsCount); // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - get a list of users for a particular instance

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{instanceId}/queries/databases?startlimit={startIndex}&noofrecords={recordsCount}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get a list of databases corresponding to a SQL Server")]
        IList<DatabaseInformation> GetDatabasesForServer(string instanceId, string startIndex, string recordsCount); // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - get a list of databases for a particular instance

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{instanceId}/queries/{queryId}/queryplan", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get the query plan for a query")]
        IList<QueryPlan> GetQueryPlan(string instanceId, string queryId); // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - get the query plan for a query

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Instances/{instanceId}/queries/view/{viewId}/groupby/{groupId}/tzo/{timeZoneOffset}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get the supported group data based on the set filters")]
        IList<QueryMonitorStatisticsData> GetQueryMonitorData(string instanceId, string viewId, string groupId, string timeZoneOffset, QueryMonitorFilters queryFilters); // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - get the supported group data based on the set filters

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Instances/{instanceId}/graphdata/view/{viewId}/groupby/{groupId}/tzo/{timeZoneOffset}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get the query monitor data for the graph representation")]
        IList<QueryMonitorDataForGraphs> GetQueryMonitorDataForGraphs(string instanceId, string viewId, string groupId, string timeZoneOffset, QueryMonitorFilters queryFilters); // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - get the query monitor data for the graph representation

    }
}
