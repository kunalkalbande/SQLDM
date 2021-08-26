using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using Idera.SQLdm.Service.Core.Security;
using Idera.SQLdm.Service.Repository;
using System.Security;
using Idera.SQLdm.Service.Core.Enums;
using Idera.SQLdm.Service.Configuration;
using System.IdentityModel.Tokens;
using BBS.TracerX;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;

namespace Idera.SQLdm.Service.Helpers.Auth
{
    public static class ServerAuthorizationHelper
    {
        private static readonly Logger LogX = Logger.GetLogger("ServerAuthorizationHelper"); //initializing the logger for this class

        public static bool IsServerAuthorized(int? instanceId, UserToken token) 
        {
            return (instanceId != null && token!=null) ? token.AssignedServers.Any(x => x.Server.SQLServerID == Convert.ToInt32(instanceId)) : false;
        }


    }
}
