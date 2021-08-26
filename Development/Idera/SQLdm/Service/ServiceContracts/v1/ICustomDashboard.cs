using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ComponentModel;
using Idera.SQLdm.Service.DataContracts.v1.CustomDashboard;

namespace Idera.SQLdm.Service.ServiceContracts.v1
{
    /// <summary>
    /// SQLdm 10.0 (srishti purohit) : Class to give Service Contract for Custom Dashboard functionality
    /// </summary>
    /// 
    [ServiceContract]
    public interface ICustomDashboard
    {

        #region CustomDashboardModule
        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To add new Custom Dashboard for a user
        /// </summary>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/customdashboard/create", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("Creates Custom Dashboard by taking user input")]
        CustomDashboard CreateCustomDashboard(string customDashboardName, string isDefault);

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To delete the selected custom dashboard
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "/customdashboard/{customDashboardid}", Method = "DELETE", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("Deletes Custom Dashboard and bound widgets.")]
        CustomDashboard Delete(string customDashboardid);

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To get all created custom dashboards
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "/customdashboards", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("Gets all Custom Dashboards.")]
        List<CustomDashboard> GetAllDasboards();

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To update existing Custom Dashboard for a user
        /// </summary>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/customdashboard/{customDashboardid}/save", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("Update existing Custom Dashboard by taking user input")]
        CustomDashboard UpdateCustomDashboard(string customDashboardid, string customDashboardName, string isDefault, string tags);

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To make copy of existing Custom Dashboard for a user
        /// </summary>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/customdashboard/{customDashboardid}/copy", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("Maaking copy of existing Custom Dashboard by taking user input")]
        bool CreateCopyCustomDashboard(string customDashboardid);

        #endregion

        #region CustomWidgetsForDashboard
        /// <summary>
    /// SQLdm 10.0 (srishti purohit) : Class to give Service Contract for Custom Dashboard-Widget functionality
    /// </summary>
    /// 
    
        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To add new Widget for specific Custom Dashboard for a user
        /// </summary>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/customdashboard/{customDashboardid}/widgets/create", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("Creates Widget for Custom Dashboard by taking user input")]
        CustomDashboardWidgets CreateCustomDashboardWidget(string customDashboardid, string widgetName, string widgetTypeId, string metricId, string match, string tagId, string sourceServerIds);

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To get all Widgets for specific Custom Dashboard for a user
        /// </summary>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/customdashboard/{customDashboardid}/widgets", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("Gets all Widget for specific Custom Dashboards")]
        List<CustomDashboardWidgets> GetAllWidgets(string customDashboardid);

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To update existing Widget for specific Custom Dashboard for a user
        /// </summary>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/customdashboard/{customDashboardid}/widget/{widgetId}/save", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("Update Widget for Custom Dashboard by taking user input")]
        CustomDashboardWidgets UpdateCustomDashboardWidget(string customDashboardid, string widgetId, string widgetName, string widgetTypeId, string metricId, string match, string tagIds, string sourceServerIds);

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To delete the selected custom dashboard Widget
        /// </summary>
        [OperationContract]
        [WebInvoke(UriTemplate = "/customdashboard/{customDashboardid}/widget/{widgetId}", Method = "DELETE", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("Deletes Custom Dashboard Widget.")]
        bool DeleteWidget(string customDashboardid, string widgetId);

        #endregion

        #region MasterDataForCustomDashboard
        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To get all Widget Types
        /// </summary>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/customdashboard/WidgetTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("Gets all Widget Types")]
        Dictionary<int, string> GetAllWidgetTypes();

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To get all Match Types
        /// </summary>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/customdashboard/MatchTypes", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("Gets all Match Types")]
        Dictionary<int, string> GetAllMatchTypes();

        #endregion

        #region MetricValuesForWidgets
        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To get Metric values for widgets
        /// </summary>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/customdashboard/{customDashboardId}/widget/{widgetId}/tzo/{timeZoneOffset}/data?start={startTime}&end={endTime}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        [Description("Gets Metric values for widgets")]
        MetricValueCollectionForCustomDashboard GetMetricValueForWidget(string customDashboardId, string widgetId, string timeZoneOffset, DateTime startTime, DateTime endTime);
        #endregion


    }
}
