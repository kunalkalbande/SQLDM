using System;
using System.Collections;
using System.Collections.Generic;
using System.Management;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.CollectionService.Probes.Collectors;
using Idera.SQLdm.CollectionService.Probes.Sql;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.CollectionService.Probes.Wmi
{
    internal class ServiceStatusProbe : WmiMiniProbe
    {
        private readonly static Logger LOG;

        private string[] serviceNames;

        private Dictionary<string, Service> services;
        public bool CollectStartTime { get; set; }

        static ServiceStatusProbe()
        {
            LOG = Logger.GetLogger("ServiceStatusProbe");
        }

        public ServiceStatusProbe(string machineName, WmiConfiguration wmiConfig) : base(machineName, wmiConfig, LOG)
        {
            services = new Dictionary<string, Service>();
            CollectStartTime = true;
        }

        public string[] ServiceNames { set { serviceNames = value; } }

        private static WqlObjectQuery CreateServiceQuery(IEnumerable<string> serviceNames)
        {
            var b = new StringBuilder("Select Name,Description,ProcessId,State,StartMode,StartName from WIN32_Service");
            if (serviceNames != null)
            {
                var all = true;
                foreach (var name in serviceNames)
                {
                    if (all)
                    {
                        b.Append(" WHERE ");
                        all = false;
                    }
                    else
                        b.Append(" or ");
                    b.AppendFormat("Name='{0}'", name);
                }
            }
            return new WqlObjectQuery(b.ToString());
        }

        protected override void Start()
        {
            services.Clear();
            StartServiceCollector();
        }

        private void StartServiceCollector()
        {
            _collector.Query = CreateServiceQuery(serviceNames);
            _collector.BeginCollection(ServiceCallback, InterpretServiceObject, null);
        }

        private object InterpretServiceObject(Collectors.WmiCollector collector, ManagementBaseObject newObject)
        {
            var service = new Service(TranslateServiceName((string) newObject["Name"]));
            service.ServiceName = (string) newObject["Name"];
            service.Description = (string) newObject["Description"];
            service.ProcessID = (UInt32) newObject["ProcessId"];

            var state = newObject["State"] as string;
            if (String.IsNullOrEmpty(state))
                state = "not installed";

            service.RunningState = ProbeHelpers.GetServiceState(state);

            service.StartupType = newObject["StartMode"] as String;
            service.LogOnAs = newObject["StartName"] as String;

            return service;
        }

        private static ServiceName TranslateServiceName(string name)
        {
            var lname = name.ToLower();
            if (lname.StartsWith("mssql$") || lname.Equals("mssqlserver")) return ServiceName.SqlServer;
            if (lname.StartsWith("sqlagent$") || lname.Equals("sqlserveragent")) return ServiceName.Agent;
            //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --translating and interpreting the service name 
            if (lname.StartsWith("sqlbrowser")) 
                return ServiceName.Browser;
            if (lname.StartsWith("mssqlserveradhelper") || lname.StartsWith("mssqlserveradhelper100"))
                return ServiceName.ActiveDirectoryHelper;
            //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --translating and interpreting the service name 
            return lname.Equals("msdtc") ? ServiceName.DTC : ServiceName.FullTextSearch;
        }

        private void ServiceCallback(object sender, CollectorCompleteEventArgs e)
        {
            using (LOG.VerboseCall("ServiceCallback"))
            {
                LOG.VerboseFormat("Service collector ran in {0} milliseconds.", e.ElapsedMilliseconds);
                var result = e.Value as IList;
                if (result != null && result.Count > 0)
                {
                    foreach (Service service in result)
                    {
                        services.Add(service.ServiceName, service);
                    }

                    _collector.Results.Clear();
                }

                _collector.Results.Clear();

                if (CollectStartTime)
                    StartProcessCollector();
                else
                    FireCompletion(services, Result.Success);
            }
        }

        private void StartProcessCollector()
        {
            var query = CreateProcessQuery();
            if (query == null)
            {
                FireCompletion(services, Result.Success);
                return;
            }
            _collector.Query = query;
            _collector.BeginCollection(ProcessCallback, InterpretProcessObject, null);
        }

        private void ProcessCallback(object sender, CollectorCompleteEventArgs e)
        {
            using (LOG.VerboseCall("ProcessCallback"))
            {
                LOG.VerboseFormat("Process collector ran in {0} milliseconds.", e.ElapsedMilliseconds);

                _collector.Results.Clear();
                FireCompletion(services, e.Result);
            }
        }

        private object InterpretProcessObject(Collectors.WmiCollector collector, ManagementBaseObject newObject)
        {
            var h = newObject["Handle"].ToString();
            int handle = int.Parse(h);

            var dtmf = newObject["CreationDate"];
            if (dtmf != null)
            {
                var created = ManagementDateTimeConverter.ToDateTime(dtmf.ToString());
                foreach (var service in services.Values)
                {
                    if (service.ProcessID.HasValue && service.ProcessID.Value == handle)
                    {
                        service.RunningSince = created;
                        break;
                    }
                }
            }
            return null;
        }

        private ObjectQuery CreateProcessQuery()
        {
            StringBuilder b = new StringBuilder("SELECT Handle,CreationDate FROM Win32_Process where ");
            int mark = b.Length;
            foreach (var service in services.Values)
            {
                if (service.ProcessID.HasValue && service.ProcessID.Value != 0)
                {
                    if (b.Length != mark)
                        b.Append(" or ");
                    b.AppendFormat("Handle='{0}'", service.ProcessID.Value);
                }
            }

            return b.Length != mark ? new WqlObjectQuery(b.ToString()) : null;
        }

        public static string[] GetServiceNames(ServiceName? serviceName, string serverName, ServerVersion productVersion)
        {
            string instance = null;
            if (serverName != null)
            {
                var sp = serverName.IndexOf('\\');
                if (sp > 0)
                    instance = serverName.Substring(sp + 1);
            }

            if (serviceName.HasValue)
            {
                var name = "MSSQLSERVER";
                switch (serviceName.Value)
                {
                    case ServiceName.SqlServer:
                        name = String.IsNullOrEmpty(instance) ? "MSSQLSERVER" : "MSSQL$";
                        break;
                    case ServiceName.Agent:
                        name = String.IsNullOrEmpty(instance) ? "SQLSERVERAGENT" : "SQLAGENT$";
                        break;
                    case ServiceName.DTC:
                        name = "MSDTC";
                        break;
                    //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --get the SQL service name
                    case ServiceName.Browser:
                        if (productVersion.Major > 8)
                            name = "SQLBrowser";
                        break;
                    case ServiceName.ActiveDirectoryHelper:    // only applicable to SQL Server 2000, 2005 and 2008
                        if (productVersion.Major == 8 || productVersion.Major == 9)
                            name = "MSSQLServerADHelper";
                        else if (productVersion.Major == 10)
                            name = "MSSQLServerADHelper100";
                        break;
                    //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --get the SQL service name
                    case ServiceName.FullTextSearch:
                        if (productVersion.Major < 9)
                            name = "MSSearch";  // SQL Server 2000
                        else
                            if (productVersion.Major < 10)
                                name = String.IsNullOrEmpty(instance) ? "MSFTESQL" : "MSFTESQL$"; // SQL Server 2005
                            else
                                name = String.IsNullOrEmpty(instance) ? "MSSQLSERVER" : "MSSQL$"; // SQL Server 2008 and beyond
                        break;
                }
                if (name.EndsWith("$"))
                    name = name + instance;

                return new string[] { name };
            }

            if (String.IsNullOrEmpty(instance))
            {
                if (productVersion.Major < 8)
                    return new string[] { "MSSQLSERVER", "SQLSERVERAGENT", "MSDTC", "MSSearch" };  //SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --append the SQL service names in all the objects returned
                
                if (productVersion.Major == 8)
                    return new string[] { "MSSQLSERVER", "SQLSERVERAGENT", "MSDTC", "MSSearch", "MSSQLServerADHelper" };  //SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --append the SQL service names in all the objects returned

                if (productVersion.Major == 9)
                    return new string[] { "MSSQLSERVER", "SQLSERVERAGENT", "MSDTC", "MSFTESQL", "SQLBrowser", "MSSQLServerADHelper" };  //SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --append the SQL service names in all the objects returned

                if (productVersion.Major == 10)
                    return new string[] { "MSSQLSERVER", "SQLSERVERAGENT", "MSDTC", "SQLBrowser", "MSSQLServerADHelper100" };  //SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --append the SQL service names in all the objects returned

                return new string[] { "MSSQLSERVER", "SQLSERVERAGENT", "MSDTC", "SQLBrowser" };
            }

            if (productVersion.Major < 8)
                return new string[] { "MSSQL$" + instance, "SQLAGENT$" + instance, "MSDTC", "MSSearch" };  //SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --append the SQL service names in all the objects returned

            if (productVersion.Major == 8)
                return new string[] { "MSSQL$" + instance, "SQLAGENT$" + instance, "MSDTC", "MSSearch", "MSSQLServerADHelper" };    //SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --append the SQL service names in all the objects returned
            
            if (productVersion.Major == 9)
                return new string[] { "MSSQL$" + instance, "SQLAGENT$" + instance, "MSDTC", "MSFTESQL$" + instance, "SQLBrowser", "MSSQLServerADHelper" };     //SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --append the SQL service names in all the objects returned

            if (productVersion.Major == 10)
                return new string[] { "MSSQL$" + instance, "SQLAGENT$" + instance, "MSDTC", "SQLBrowser", "MSSQLServerADHelper100" };     //SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --append the SQL service names in all the objects returned

            return new string[] { "MSSQL$" + instance, "SQLAGENT$" + instance, "MSDTC", "SQLBrowser" };       //SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --append the SQL service names in all the objects returned
        }

    }
}
