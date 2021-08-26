using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Management;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers
{
    public static class WmiHelper
    {
        public static readonly string WMI_Namespace_root = @"\\.\root\";
        public static readonly string WMI_Namespace_CIMV2 = WMI_Namespace_root + @"CIMV2";
        public static readonly string WMI_Namespace_MicrosoftVolumeEncryption = WMI_Namespace_root + @"CIMV2\Security\MicrosoftVolumeEncryption";

        public static ManagementScope GetManagementScopeCimV2(WmiConnectionInfo info)
        {
            return (GetManagementScope(WMI_Namespace_CIMV2, info));
        }

        public static ManagementScope GetManagementScope(string wmiNamespace, WmiConnectionInfo info)
        {
            string path = wmiNamespace;
            ManagementScope scope;
            if (null != info)
            {
                if (!string.IsNullOrEmpty(info.MachineName))
                {
                    if (path.StartsWith(WMI_Namespace_root))
                    {
                        path = string.Format(@"\\{0}\root\", info.MachineName) + path.Substring(WMI_Namespace_root.Length);
                    }
                }
                ConnectionOptions options = new ConnectionOptions();
                if (!string.IsNullOrEmpty(info.Authority)) options.Authority = info.Authority;
                if (!string.IsNullOrEmpty(info.Password))
                {
                    options.Impersonation = ImpersonationLevel.Impersonate;
                    options.Username = info.Username;
                    options.Password = info.Password;
                    options.EnablePrivileges = true;
                }

                scope = new ManagementScope(path, options);
            }
            else
            {
                scope = new ManagementScope(path);
            }
            scope.Connect();
            return (scope);
        }

    }
}
