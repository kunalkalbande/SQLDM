using System;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdm.Service.DataContracts.v1.License;

namespace Idera.SQLdm.Service.ServiceContracts.v1
{
    [ServiceContract]
    public interface ILicenseManager
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "License/GetLicense", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get details of the current license.")]
        LicenseDetails GetLicense();
    }
}
