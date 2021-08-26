using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ComponentModel;
using Idera.SQLdm.Service.DataContracts.v1.Auth;
using Idera.SQLdm.Service.Web;

namespace Idera.SQLdm.Service.ServiceContracts.v1
{
    [ServiceContract]
    public interface IAuthManager
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Authenticate", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Authenticate User")]
        UserAuthenticationResponse GetUserAuthenticationResponse(UserAuthenticationRequest authReq);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Authenticate/CheckSecurityEnabled", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Check if application security is turned on")]
        AppSecurityEnabledResponse IsAppSecurityEnabled();
    }
}
