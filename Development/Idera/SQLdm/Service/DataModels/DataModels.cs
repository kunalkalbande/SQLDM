// ===============================
// AUTHOR       : CWF Team - Gowrish 
// PURPOSE      : Backend Isolation
// TICKET       : SQLDM-29086
// ===============================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginCommon;
using System.Security.Principal;

namespace Idera.SQLdm.Service.DataModels
{
 
    public class Principal
    {
        public User user;
        public IPrincipal principal;
        public Principal(User user, IPrincipal principal)
        {
            this.principal = principal;
            this.user = user;
        }
    }
}
