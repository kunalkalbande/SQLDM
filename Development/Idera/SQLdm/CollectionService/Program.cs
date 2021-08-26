using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Remoting;
using System.ServiceProcess;
using System.Text;

using BBS.TracerX;
using Idera.SQLdm.CollectionService.Configuration;
using System.Reflection;
using System.Collections;
using System.Configuration.Install;

namespace Idera.SQLdm.CollectionService
{
    using System.Diagnostics;
    using System.Xml;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Messages;
    using System.IO;
    using Microsoft.ApplicationBlocks.Data;

    static class Program
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("Program");
        public const string SERVICE_NAME_BASE = "SQLdmCollectionService";

        public const string OPTION_INSTANCE_NAME = "InstanceName";
        public const string OPTION_INSTANCE_NAME_DEFAULT = "Default";
        public const string OPTION_MANAGEMENT_HOST = "ManagementHost";
        public const string OPTION_MANAGEMENT_HOST_DEFAULT = "localhost";
        public const string OPTION_MANAGEMENT_PORT = "ManagementPort";
        public const string OPTION_MANAGEMENT_PORT_DEFAULT = "5166";
        public const string OPTION_SERVICE_PORT = "Port";
        public const string OPTION_SERVICE_PORT_DEFAULT = "5167";
        public const string OPTION_HEARTBEAT_INTERVAL = "Heartbeat";
        public const string OPTION_HEARTBEAT_INTERVAL_DEFAULT = "180";

        public const string OPTION_LOG_CONFIG_SECTION = "Log";
        public const string OPTION_LOG_CONFIG_SECTION_DEFAULT = null;

        public const string OPTION_RUN_INTERACTIVE = "console";
        public const string OPTION_INSTALL_SERVICE = "install";
        public const string OPTION_UNINSTALL_SERVICE = "uninstall";

        public const string OPTION_FORCE = "Force";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                bool runInteractive = false;
                bool installService = false;
                bool unInstallService = false;
                bool force = false;

                string instanceName = OPTION_INSTANCE_NAME_DEFAULT;
                string managementHost = null;
                string managementPort = null;
                string servicePort = null;
                string heartbeat = null;
                string logSection = null;

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
                        {
                            instanceName = args[++i];
                        }
                        continue;
                    }
                    if (comparer.Compare(arg, OPTION_MANAGEMENT_HOST) == 0)
                    {
                        if (i + 1 < argCount && args[i + 1].Length > 0)
                        {
                            managementHost = args[++i];
                        }
                        continue;
                    }
                    if (comparer.Compare(arg, OPTION_MANAGEMENT_PORT) == 0)
                    {
                        if (i + 1 < argCount && args[i + 1].Length > 0)
                        {
                            managementPort = args[++i];
                        }
                        continue;
                    }
                    if (comparer.Compare(arg, OPTION_SERVICE_PORT) == 0)
                    {
                        if (i + 1 < argCount && args[i + 1].Length > 0)
                        {
                            servicePort = args[++i];
                        }
                        continue;
                    }
                    if (comparer.Compare(arg, OPTION_HEARTBEAT_INTERVAL) == 0)
                    {
                        if (i + 1 < argCount && args[i + 1].Length > 0)
                        {
                            heartbeat = args[++i];
                        }
                        continue;
                    }
                    if (comparer.Compare(arg, OPTION_LOG_CONFIG_SECTION) == 0)
                    {
                        if (i + 1 < argCount && args[i + 1].Length > 0)
                        {
                            logSection = args[++i];
                        }
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

                // always set the instance name in the configuration object
                CollectionServiceConfiguration.InstanceName = instanceName;

                // configure the event log in case we want to log stuff
                EventLog eventLog = CollectionServiceInstaller.RegisterEventSource(instanceName);

                int modeCount = 0;
                if (runInteractive)
                    modeCount++;
                if (installService)
                    modeCount++;
                if (unInstallService)
                    modeCount++;

                if (modeCount == 0)
                {
                    runInteractive = Environment.UserInteractive;
                }
                else
                    if (modeCount != 1)
                    {
                        OutputUsage();
                        return;
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

                    Assembly assembly = typeof(Program).Assembly;
                    List<string> parms = new List<string>(8);
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

                    string[] iargs = new string[parms.Count];
                    parms.CopyTo(iargs);

                    IDictionary state = new Hashtable();
                    AssemblyInstaller installer = new AssemblyInstaller(assembly, iargs);
                    try
                    {
                        if (installService)
                            installer.Install(state);
                        else
                            installer.Uninstall(state);
                    }
                    catch
                    {

                    }
                    return;
                }

                bool configCorrupt = true;
                try
                {
                    CollectionServicesSection section = (CollectionServicesSection)ConfigurationManager.GetSection(CollectionServicesSection.SectionName);
                    if (section != null)
                    {
                        if (section.CollectionServices.Count > 0)
                        {
                            CollectionServiceElement element = section.CollectionServices[instanceName];
                            if (element != null)
                            {
                                if (!String.IsNullOrEmpty(element.ManagementServiceAddress))
                                {
                                    configCorrupt = false;
                                }
                            }
                        }
                    }
                }
                catch (ConfigurationErrorsException cee)
                {
                    // leave suicide note
                    EventInstance suicideEvent = new EventInstance((long)Status.ErrorConfigFoobar, (int)Category.General,EventLogEntryType.Error);
                    StringBuilder builder = new StringBuilder();
                    builder.Append(cee.Message);
                    for (Exception e = cee.InnerException; e != null; e = e.InnerException)
                        builder.Append("\n").Append(e.Message);
                    try
                    {
                        eventLog.WriteEvent(suicideEvent, "SQLdmCollectionService.exe.config", builder.ToString());
                        
                    } catch(Exception e)
                    {
                        LOG.Error(e);
                    } 
                    // returning here will kill the service
                    CommitSuicide(instanceName);
                    return;
                }

                // configure logging 
                string diraccess = "Logs";
                try
                {
                    CollectionServiceConfiguration.ConfigureLogging(logSection);
                    string localData =
                        Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), instanceName);

                    diraccess = instanceName;
                    CollectionServiceConfiguration.CheckAccess(localData);
                }
                catch (UnauthorizedAccessException uae)
                {
                    EventInstance suicideEvent = new EventInstance((long)Status.ErrorUnauthorizedAccessException, (int)Category.General, EventLogEntryType.Error);
                    StringBuilder builder = new StringBuilder();
                    builder.Append(uae.Message);
                    for (Exception e = uae.InnerException; e != null; e = e.InnerException)
                        builder.Append("\n").Append(e.Message);
                    try
                    {
                        String user = Environment.UserName;
                        eventLog.WriteEvent(suicideEvent, user, diraccess, builder.ToString());
                    }
                    catch (Exception e)
                    {
                        LOG.Error(e);
                    }
                    CommitSuicide(instanceName);
                    return;
                }


                if (configCorrupt)
                {
                    LOG.Error(
                        "Collection Service configuration file (SQLdmCollectionService.exe.config) is invalid.  One or more expected items are missing.");
                    try
                    {
                        EventInstance configEvent =
                            new EventInstance((long) Status.ErrorConfigInvalid, 
                                              (int) Category.General,
                                               EventLogEntryType.Error);
                        eventLog.WriteEvent(configEvent, "SQLdmCollectionService.exe.config");
                    }
                    catch (Exception e)
                    {
                        LOG.Error(e);
                    }
                }

                int parse;

                // convert the port to an int
                int? port = null;
                if (managementPort != null)
                {
                    if (int.TryParse(managementPort, out parse))
                        port = parse;
                }

                // set the management service address
                if (managementHost != null || port.HasValue)
                    CollectionServiceConfiguration.SetManagementServiceAddress(managementHost, port);

                if (servicePort != null && (int.TryParse(servicePort, out parse) && parse > 0 && parse < 65530))
                    CollectionServiceConfiguration.SetServicePort(parse);

                // set the global connection timeout from the config file
                SqlConnectionInfo.DefaultConnectionTimeout =
                    CollectionServiceConfiguration.GetCollectionServiceElement().DefaultSqlConnectionTimeout;
                // set the golbal command timeout from the config file
                SqlHelper.CommandTimeout =
                    CollectionServiceConfiguration.GetCollectionServiceElement().DefaultSqlCommandTimeout;

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

        private static void OutputUsage()
        {
            Console.WriteLine("");
            Console.Clear();
            Console.WriteLine("Usage: CollectionService /install /InstanceName name /ManagementHost host /ManagementPort 5166 ");
            Console.WriteLine("       Run the service installer to register a new instance of the collection service. ");
            Console.WriteLine("       After installing the service you can start the new instance using the Net Start command: ");
            Console.WriteLine("       Net Start SQLdmCollSvc$InstanceName");
            Console.WriteLine("");
            Console.WriteLine("       CollectionService /install /InstanceName name [/ManagementHost host] [/ManagementPort 5166] [/force] ");
            Console.WriteLine("       Run the service installer to unregister an instance of the collection service. ");
            Console.WriteLine("       In order to use the /ManagementHost and /ManagementPort parameters you must also specify /force. ");
            Console.WriteLine("       It is not recommended to use these parameters unless directed to by Technical Support.");
            Console.WriteLine("");
            Console.WriteLine("       CollectionService /console /InstanceName name [/ManagementHost host] [/ManagementPort 5166] [/Log log4net] ");
            Console.WriteLine("       Run the service in a cmd window.  Options /ManagementHost, /ManagementPort and /Log will override ");
            Console.WriteLine("       corresponding values stored in the config file.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.ReadKey();
        }

        private static void CommitSuicide(string instanceName)
        {
            if (Environment.UserInteractive)
                return;

            using (SuicideService suicide = new SuicideService())
            {
                suicide.AutoLog = false;
                suicide.ServiceName = SERVICE_NAME_BASE + "$" + instanceName;
                ServiceBase.Run(suicide);
            }

            return;
        }

        class SuicideService : ServiceBase
        {
            protected override void OnStart(string[] args)
            {
                new System.Threading.Thread(Run).Start();
            }
            protected void Run()
            {
                // write the event to the application event log
                MainService.WriteEvent(
                    EventLog,
                    EventLogEntryType.Error,
                    Status.CollectionServiceStopFatal,
                    Category.General);
                // wait a second
                System.Threading.Thread.Sleep(1000);
                // bang!
                Stop();
            }
        }
    }
}