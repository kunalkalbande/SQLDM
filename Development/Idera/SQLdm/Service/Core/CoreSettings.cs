using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Idera.SQLdm.Service.Configuration;

namespace Idera.SQLdm.Service.Core
{
    class CoreSettings
    {
        // Configurable/Persisted Options.  Service.OnStart handles loading these.
        public static string RepositoryDatabase = "SQLdmRepository";
        public static int DefaultUrlPort = 9278;
        public static string BaseUrl { get { return (string.Format("http://{0}:{1}/SQLdm", Dns.GetHostName(), RestServiceConfiguration.ServicePort != 0 ? RestServiceConfiguration.ServicePort : DefaultUrlPort )); } }
        public static int ApplicationId = 3; //TODO: to change this later if needed
        public static bool IsAuthEnabled = true; //added to override auth
        public static string InstanceName = "Default";
        public static string BaseServiceName = "SQLdmRestService";

        public static bool repositoryWindowsAuthentication = true;
        public static string tracerXConfiguration = "TracerX";
    }
}
