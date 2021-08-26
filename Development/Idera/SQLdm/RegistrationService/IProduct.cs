using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Idera.SQLdm.RegistrationService
{
    [ServiceContract]
    public interface IProduct
    {
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "Product", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Stream GetProductData();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "CWF/Properties", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void SetDashboardLocation(PluginCommon.NotifyProduct product);
    }

}
