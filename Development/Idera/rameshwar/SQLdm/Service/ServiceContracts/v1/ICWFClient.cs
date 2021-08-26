using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ComponentModel;
using Idera.SQLdm.Service.DataContracts.v1;
using Idera.SQLdm.Service.Web;
using PluginCommon;
using CWFContracts = Idera.SQLdm.Common.CWFDataContracts;

namespace Idera.SQLdm.Service.ServiceContracts.v1
{
    [ServiceContract]
    public interface ICWFManager
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/SyncAlerts", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Sync the list of alerts with CWF")]
        void SyncAlertsWithCWF(List<CWFContracts.Alert> freshAlerts);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/SyncInstances", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Sync the list of instances with CWF")]
        void SyncInstanceWithCWF(List<CWFContracts.Instance> freshInstances);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/SyncUsers", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Sync the list of users with CWF")]
        void SyncUsersWithCWF(List<CWFContracts.User> freshUsers);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/GetAllProductInstances", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get a list of all instances from CWF")]
        List<CWFContracts.Instance> GetInstancesFromCWF();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddDashboardWidgets", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("adds the passed instances into IDERA dashboard")]
        void AddDashboardWidgets();

    }
}
