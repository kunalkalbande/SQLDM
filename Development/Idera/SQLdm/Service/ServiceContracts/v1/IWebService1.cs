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
    public interface IWebService1 : IServerManager, IAlertManager, ITopXManager, ICategoryManager, ILicenseManager, IAuthManager, ICWFManager, IQueryManager, IProductInfo, IGeneral, ICustomDashboard, Idera.LicenseManager.ProductPlugin.ServiceContracts.ILicenseManager, ITagManager, IRecommendationManager //, Other services Contracts to come here

    {
        //[OperationContract]
        //[WebInvoke(UriTemplate = "ForceCleanup", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //[Description("Force Cleanup")]
        //void ForceCleanup();

        //[OperationContract]
        //[WebInvoke(UriTemplate = "GetVersion", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //[Description("Get the current SQL Elements version")]
        //string GetVersion();

        //[OperationContract]
        //[WebInvoke(UriTemplate = "GetServiceStatus", Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //[Description("Get the current status of the service")]
        //GetServiceStatusResponse GetServiceStatus();
    }
}
