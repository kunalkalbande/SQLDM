
namespace Idera.SQLdm.PredictiveAnalyticsService
{
    using System;    
    using System.Text;
    using System.IO;
    using System.Data;
    using System.Data.Sql;
    using System.Data.SqlClient;
    using System.Collections;
    using System.Collections.Generic;
    
    using Helpers;
    using Classifiers;
    using System.Diagnostics;
    using System.Reflection;
    using System.Configuration.Install;
    using Idera.SQLdm.Common.Configuration;

    using Microsoft.ApplicationBlocks.Data;
    using System.ServiceProcess;
    using Idera.SQLdm.PredictiveAnalyticsService.Configuration;
    using BBS.TracerX;
    using System.Threading;

    static class Program
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("Program");

        public const string SERVICE_NAME_BASE = "SQLdmPredictiveAnalyticsService";

        public const string OPTION_INSTANCE_NAME              = "InstanceName";
        public const string OPTION_INSTANCE_NAME_DEFAULT      = "Default";
        public const string OPTION_MANAGEMENT_HOST            = "ManagementHost";
        public const string OPTION_MANAGEMENT_HOST_DEFAULT    = "localhost";
        public const string OPTION_MANAGEMENT_PORT            = "ManagementPort";
        public const string OPTION_MANAGEMENT_PORT_DEFAULT    = "5166";
        public const int    OPTION_MANAGEMENT_PORT_DEFAULT_INT = 5166;

        //Start: SQL DM 10.0 -- Doctor Integration -- Added const
        public const string OPTION_COLLECTION_HOST = "CollectionHost";
        public const string OPTION_COLLECTION_HOST_DEFAULT = "localhost";
        public const string OPTION_COLLECTION_PORT = "CollectionPort";
        public const string OPTION_COLLECTION_PORT_DEFAULT = "5167";
        public const int OPTION_COLLECTION_PORT_DEFAULT_INT = 5167;
        public const string OPTION_PRESCRIPTIVE_PORT_DEFAULT = "5169";
        public const int OPTION_PRESCRIPTIVE_PORT_DEFAULT_INT = 5169;
        //End: SQL DM 10.0 -- Doctor Integration -- Added const

        public const string OPTION_SERVICE_PORT               = "Port";
        public const string OPTION_SERVICE_PORT_DEFAULT       = "5168";
        public const string OPTION_HEARTBEAT_INTERVAL         = "Heartbeat";
        public const string OPTION_HEARTBEAT_INTERVAL_DEFAULT = "180";
        public const string OPTION_LOG_CONFIG_SECTION         = "Log";
        public const string OPTION_LOG_CONFIG_SECTION_DEFAULT = null;
        public const string OPTION_RUN_INTERACTIVE            = "console";
        public const string OPTION_INSTALL_SERVICE            = "install";
        public const string OPTION_UNINSTALL_SERVICE          = "uninstall";
        public const string OPTION_FORCE                      = "Force";

        static void Main(string[] args)
        {
            try
            {
                bool   runInteractive   = false;
                bool   installService   = false;
                bool   unInstallService = false;
                bool   force            = false;
                string instanceName     = OPTION_INSTANCE_NAME_DEFAULT;
                string managementHost   = null;
                string managementPort   = null;
                string servicePort      = null;
                string heartbeat        = null;
                string logSection       = null;

                CaseInsensitiveComparer comparer = CaseInsensitiveComparer.DefaultInvariant;

                int argCount = args.Length;

                for (int i = 0; i < args.Length; i++)
                {
                    string arg = args[i];

                    if (!(arg.StartsWith("-") || arg.StartsWith("/")))
                        continue;

                    arg = arg.Substring(1).Trim();

                    if (comparer.Compare(arg, OPTION_INSTANCE_NAME) == 0)
                    {
                        if (i + 1 < argCount && args[i + 1].Length > 0)
                            instanceName = args[++i];

                        continue;
                    }
                    if (comparer.Compare(arg, OPTION_MANAGEMENT_HOST) == 0)
                    {
                        if (i + 1 < argCount && args[i + 1].Length > 0)
                            managementHost = args[++i];
                        
                        continue;
                    }
                    if (comparer.Compare(arg, OPTION_MANAGEMENT_PORT) == 0)
                    {
                        if (i + 1 < argCount && args[i + 1].Length > 0)
                            managementPort = args[++i];
                        
                        continue;
                    }
                    if (comparer.Compare(arg, OPTION_SERVICE_PORT) == 0)
                    {
                        if (i + 1 < argCount && args[i + 1].Length > 0)
                            servicePort = args[++i];
                        
                        continue;
                    }
                    if (comparer.Compare(arg, OPTION_HEARTBEAT_INTERVAL) == 0)
                    {
                        if (i + 1 < argCount && args[i + 1].Length > 0)
                            heartbeat = args[++i];
                        
                        continue;
                    }
                    if (comparer.Compare(arg, OPTION_LOG_CONFIG_SECTION) == 0)
                    {
                        if (i + 1 < argCount && args[i + 1].Length > 0)
                            logSection = args[++i];
                        
                        continue;
                    }
                    if (comparer.Compare(arg, OPTION_RUN_INTERACTIVE) == 0)
                    {
                        runInteractive = true;
                        continue;
                    }
                    if (comparer.Compare(arg, OPTION_INSTALL_SERVICE) == 0)
                    {
                        installService = true;
                        continue;
                    }
                    if (comparer.Compare(arg, OPTION_UNINSTALL_SERVICE) == 0)
                    {
                        unInstallService = true;
                        continue;
                    }
                    if (comparer.Compare(arg, OPTION_FORCE) == 0)
                    {
                        force = true;
                        continue;
                    }

                    OutputUsage();
                    return;
                }

                InitLogging();

                // configure the event log in case we want to log stuff
                //EventLog eventLog = CollectionServiceInstaller.RegisterEventSource(instanceName);

                int modeCount = 0;

                if (runInteractive)   modeCount++;
                if (installService)   modeCount++;
                if (unInstallService) modeCount++;

                if (modeCount == 0)
                {
                    runInteractive = Environment.UserInteractive;
                }
                else
                {
                    if (modeCount != 1)
                    {
                        OutputUsage();
                        return;
                    }
                }

                if (installService || unInstallService)
                {
                    int test = 0;

                    if (managementPort != null && (!int.TryParse(managementPort, out test) || test < 0 || test > 65535))
                        managementPort = null;

                    if (heartbeat != null && (!int.TryParse(heartbeat, out test) || test < 0))
                        heartbeat = null;

                    if (servicePort != null && (!int.TryParse(servicePort, out test) || test < 0 || test > 65535))
                        servicePort = null;
                    
                    IDictionary       state     = new Hashtable(); 
                    Assembly          assembly  = typeof(Program).Assembly;
                    List<string>      parms     = new List<string>(8);
                    string[]          iargs     = new string[parms.Count];
                    AssemblyInstaller installer = new AssemblyInstaller(assembly, iargs);

                    parms.Add(InstallSwitch(OPTION_INSTANCE_NAME, instanceName));
                    parms.Add(InstallSwitch(OPTION_FORCE, force ? Boolean.TrueString : Boolean.FalseString));

                    if (managementHost != null)
                        parms.Add(InstallSwitch(OPTION_MANAGEMENT_HOST, managementHost));

                    if (managementPort != null)
                        parms.Add(InstallSwitch(OPTION_MANAGEMENT_PORT, managementPort));

                    if (servicePort != null)
                        parms.Add(InstallSwitch(OPTION_SERVICE_PORT, servicePort));

                    if (logSection != null)
                        parms.Add(InstallSwitch(OPTION_LOG_CONFIG_SECTION, logSection));

                    if (heartbeat != null)
                        parms.Add(InstallSwitch(OPTION_HEARTBEAT_INTERVAL, heartbeat));
                    
                    parms.CopyTo(iargs);
                                       
                    try
                    {
                        if (installService)
                            installer.Install(state);
                        else
                            installer.Uninstall(state);
                    }
                    catch(Exception ex)
                    {
                        LOG.Error("Failed to perform install/uninstall.", ex);
                    }

                    return;
                }
              
                // convert the port to an int
                int  parse;
                int? port = null;

                if (managementPort != null)
                {
                    if (int.TryParse(managementPort, out parse))
                        port = parse;
                }

                // set the management service address
                if (managementHost != null || port.HasValue)
                    PredictiveAnalyticsConfiguration.ManagementServiceAddress = managementHost;

                // set the management service port
                if (port != null && port > 0 && port < 65530)
                    PredictiveAnalyticsConfiguration.ManagementServicePort = port.HasValue ? port.Value : 0;

                // set the global connection timeout from the config file
                SqlConnectionInfo.DefaultConnectionTimeout = 60000;// CollectionServiceConfiguration.GetCollectionServiceElement().DefaultSqlConnectionTimeout;

                // set the golbal command timeout from the config file
                SqlHelper.CommandTimeout = 60000;// CollectionServiceConfiguration.GetCollectionServiceElement().DefaultSqlCommandTimeout;

                using (MainService mainSvc = new MainService())
                {
                    if (runInteractive)
                        mainSvc.RunFromConsole();
                    else
                        ServiceBase.Run(mainSvc);
                }
            }
            catch (Exception exception)
            {                
                LOG.Fatal("Unhandled Exception", exception);
            }
        }

        private static void OutputUsage()
        {
            Console.WriteLine("");
            Console.Clear();
            Console.WriteLine("Usage: PredictiveAnalyticsService /install /InstanceName name /ManagementHost host /ManagementPort 5166 ");
            Console.WriteLine("       Run the service installer to register a new instance of the service. ");
            Console.WriteLine("       After installing the service you can start the new instance using the Net Start command: ");
            Console.WriteLine("       Net Start SQLdmCollSvc$InstanceName");
            Console.WriteLine("");
            Console.WriteLine("       PredictiveAnalyticsService /install /InstanceName name [/ManagementHost host] [/ManagementPort 5166] [/force] ");
            Console.WriteLine("       Run the service installer to unregister an instance of the collection service. ");
            Console.WriteLine("       In order to use the /ManagementHost and /ManagementPort parameters you must also specify /force. ");
            Console.WriteLine("       It is not recommended to use these parameters unless directed to by Technical Support.");
            Console.WriteLine("");
            Console.WriteLine("       PredictiveAnalyticsService /console /InstanceName name [/ManagementHost host] [/ManagementPort 5166] [/Log log4net] ");
            Console.WriteLine("       Run the service in a cmd window.  Options /ManagementHost, /ManagementPort and /Log will override ");
            Console.WriteLine("       corresponding values stored in the config file.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.ReadKey();
        }

        private static void InitLogging()
        {
            bool configRc = Logger.Xml.Configure();
            Logger.FileLogging.Name = "SQLdmPredictiveAnalyticsService.tx1";
            if (Thread.CurrentThread.Name == null) 
                Thread.CurrentThread.Name = "Main";

            // Do not log the command line args, because it can contain the password for SQL Auth.
            Logger.GetLogger("StandardData").FileTraceLevel = BBS.TracerX.TraceLevel.Info;
            Logger.FileLogging.Open(); 
        }

        /// <summary>
        /// Create a command line option to be passed to the installer.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static string InstallSwitch(string name, string value)
        {
            StringBuilder builder = new StringBuilder();
             
            builder.Append('/');
            builder.Append(name);
            builder.Append('=');
            builder.Append(value);

            return builder.ToString();
        }        
    }
}
