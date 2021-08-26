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
using Idera.SQLdm.Service.DataContracts.v1.Database;
using Idera.SQLdm.Service.Web;

namespace Idera.SQLdm.Service.ServiceContracts.v1
{   
    [ServiceContract]
    public interface IServerManager
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "token", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get a list of Servers")]
        Idera.SQLdm.Common.Objects.ApplicationSecurity.UserToken GetUserToken();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get a list of Servers")]
        ServerSummaryContainerCollection GetInstances();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "InstancesByName?instanceName={instanceName}&page={page}&limit={limit}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get a list of Servers")]
        ServerSummaryContainerV2 GetInstancesByInstanceName(string instanceName,string page,string limit);

        /*[OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/Severity", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get a list of Servers with severity")]
        MonitoredSqlServerStatusCollection GetInstancesWithSeverity();*/

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/abridged?ActiveOnly={ActiveOnly}&FilterField={FilterField}&FilterValue={FilterValue}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get a list of Servers")]
        MonitoredSqlServerCollection GetShortInstances(string ActiveOnly, string FilterField, string FilterValue);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/abridged/{InstanceId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get a list of Servers")]
        MonitoredSqlServer GetShortInstanceDetails(string InstanceId);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/tzo/{timeZoneOffset}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Instance Details")]
        ServerSummaryContainer GetInstanceDetails(string InstanceId, string timeZoneOffset);

        [OperationContract]
        [DateTimeInspector]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/ServerStatistics/tzo/{timeZoneOffset}?NumHistoryMinutes={NumHistoryMinutes}&endDate={endDate}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get History of server staticstics")]
        MonitoredSqlServer GetServerStatisticsHistory(string InstanceId, string timeZoneOffset, int NumHistoryMinutes, DateTime endDate);
                
    	[OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Tags", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get all instance tags/groups")]
        TagsCollection GetTags();        

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/databases/tzo/{timeZoneOffset}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Instance Databases and their details")]
        IList<MonitoredSqlServerDatabase> GetDatabasesByInstance(string InstanceId, string timeZoneOffset);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "ping/{reflectingData}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Pinging service")]
        IList<string> AreYouUp(string reflectingData);

        /*[OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "LatestResponseTimeByInstance", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get latest response times across all instances sorted by worst first")]
        IList<ResponseTimeForInstance> GetLatestResponseTimesByInstance();*/

        //SQL10.1 Srishti Purohit
        //Health index Scale factors
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetScaleFactors", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get scale factors of all Servers")]
        HealthIndexScaleFactors GetScaleFactors();

        //SQL10.1 Srishti Purohit
        //Health index Scale factors
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateScaleFactors", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("Update scale factors for all Servers")]
        void UpdateScaleFactors(HealthIndexCoefficient healthCoefficient, List<InstanceScaleFoctor> ins, List<TagScaleFactor> tag);

        //SQL10.1 Nishant Adhikari
        //Consolidated Instance Overview
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{InstanceId}/GetConsolidatedInstanceOverview", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get Consolidated Instance Overview")]
        ConsolidatedInstanceOverview GetConsolidatedInstanceOverview(string InstanceId);
    }    
}
