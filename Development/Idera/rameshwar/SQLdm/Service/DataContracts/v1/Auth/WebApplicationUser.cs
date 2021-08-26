using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace Idera.SQLdm.Service.DataContracts.v1.Auth
{
    public class WebApplicationUser
    {
        public string Name { get; set; }

        public int PrincipalId { get; set; }

        public bool IsSysAdmin { get; set; }

        public SecurityIdentifier  Sid { get; set; }

        public string Type { get; set; }

        public string Type_Desc { get; set; }


    }
}
