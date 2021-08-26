using Idera.SQLdm.Service.DataContracts.v1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Idera.SQLdm.Service.ServiceContracts.v1
{
    [ServiceContract]
    public interface IGeneral
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "ForceCleanup", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Force Cleanup")]
        void ForceCleanup();

        [OperationContract]
        [WebInvoke(UriTemplate = "GetVersion", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get the current SQL Elements version")]
        string GetVersion();

        [OperationContract]
        [WebInvoke(UriTemplate = "GetServiceStatus", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get the current status of the service")]
        GetServiceStatusResponse GetServiceStatus();

        //SQLdm 10.2 (Anshika Sharma) : Sets User Session Settings
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SetUserSessionSettings", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Sets User Sessions settings")]
        string SetUserSessionSettings(Dictionary<string, string> Settings);


        //SQLdm 10.2 (Anshika Sharma) : Gets User Session Settings by userId
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetUserSessionSettings", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Gets User Session settings")]
        Dictionary<string, string> GetUserSessionSettings();

    }
}
