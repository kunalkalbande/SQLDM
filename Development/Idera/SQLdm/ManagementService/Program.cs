namespace Idera.SQLdm.ManagementService
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration.Install;
    using System.Reflection;
    using System.ServiceProcess;
    using System.Text;
    using Helpers;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.ManagementService.Configuration;
    using BBS.TracerX;
    using System.Management.Instrumentation;
    using System.Diagnostics;
    using System.Configuration;
    using Idera.SQLdm.Common.Messages;
    using System.IO;
    using Microsoft.ApplicationBlocks.Data;

    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Name = "FullTrust")]
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Name = "FullTrust")]

    [System.ComponentModel.RunInstaller(true)]
    public class MyInstaller : DefaultManagementProjectInstaller { }
   
    public static class Program
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("Program");

        public const string SERVICE_NAME_BASE                   = "SQLdmManagementService";
        
        public const string OPTION_INSTANCE_NAME                = "InstanceName";
        public const string OPTION_INSTANCE_NAME_DEFAULT        = "Default";
        public const string OPTION_REPOSITORY_HOST              = "RepositoryHost";
        public const string OPTION_REPOSITORY_HOST_DEFAULT      = "localhost";
        public const string OPTION_REPOSITORY_DB                = "RepositoryDB";
        public const string OPTION_REPOSITORY_DB_DEFAULT        = "SQLdm";
        public const string OPTION_REPOSITORY_USER              = "RepositoryUser";
        public const string OPTION_REPOSITORY_USER_DEFAULT      = null;
        public const string OPTION_REPOSITORY_PASS              = "RepositoryPassword";
        public const string OPTION_REPOSITORY_PASS_DEFAULT      = null;
        public const string OPTION_REPOSITORY_SSPI              = "RepositorySSPI";
        public const string OPTION_REPOSITORY_SSPI_DEFAULT      = null;
        public const string OPTION_LOG_CONFIG_SECTION           = "Log";
        public const string OPTION_LOG_CONFIG_SECTION_DEFAULT   = null;
        public const string OPTION_SERVICE_PORT                 = "Port";
        public const string OPTION_SERVICE_PORT_DEFAULT         = "5166";
        public const string OPTION_HEARTBEAT_INTERVAL           = "Heartbeat";
        public const string OPTION_HEARTBEAT_INTERVAL_DEFAULT   = "60";

        public const string OPTION_RUN_INTERACTIVE              = "console";
        public const string OPTION_INSTALL_SERVICE              = "install";
        public const string OPTION_UNINSTALL_SERVICE            = "uninstall";

        public const string OPTION_FORCE                        = "Force";

        //Start: SQL DM 10.0 -- Doctor Integration -- Added const
        public const string OPTION_PRESCRIPTIVE_HOST_DEFAULT = "localhost";
        public const string OPTION_PRESCRIPTIVE_PORT_DEFAULT = "5169";
        public const int OPTION_PRESCRIPTIVE_PORT_DEFAULT_INT = 5169;
        //End: SQL DM 10.0 -- Doctor Integration -- Added const

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static  void Main(string[] args)
        {
            bool runInteractive = false;
            bool installService = false;
            bool unInstallService = false;
            bool force = false;
            bool useSSPI = true;
            bool haveRepositoryParms = false;

            string instanceName = OPTION_INSTANCE_NAME_DEFAULT;
            string repositoryHost = null;
            string repositoryDatabase = null;
            string logSection = null;
            string servicePort = null;
            string heartbeat = null;
            string repositoryUser = OPTION_REPOSITORY_USER_DEFAULT;
            string repositoryPassword = OPTION_REPOSITORY_PASS_DEFAULT;
            string repositorySSPI = OPTION_REPOSITORY_SSPI_DEFAULT;

            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

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
                if (comparer.Compare(arg, OPTION_REPOSITORY_HOST) == 0)
                {
                    if (i + 1 < argCount && args[i + 1].Length > 0)
                    {
                        repositoryHost = args[++i];
                        haveRepositoryParms = true;
                    }
                    continue;
                }
                if (comparer.Compare(arg, OPTION_REPOSITORY_DB) == 0)
                {
                    if (i + 1 < argCount && args[i + 1].Length > 0)
                    {
                        repositoryDatabase = args[++i];
                        haveRepositoryParms = true;
                    }
                    continue;
                }
                if (comparer.Compare(arg, OPTION_REPOSITORY_USER) == 0)
                {
                    if (i + 1 < argCount && args[i + 1].Length > 0)
                    {
                        repositoryUser = args[++i];
                        haveRepositoryParms = true;
                    }
                    continue;
                }
                if (comparer.Compare(arg, OPTION_REPOSITORY_PASS) == 0)
                {
                    if (i + 1 < argCount && args[i + 1].Length > 0)
                    {
                        repositoryPassword = args[++i];
                        haveRepositoryParms = true;
                    }
                    continue;
                }
                if (comparer.Compare(arg, OPTION_REPOSITORY_SSPI) == 0)
                {
                    if (i + 1 < argCount && args[i + 1].Length > 0)
                    {
                        repositorySSPI = args[++i];
                        haveRepositoryParms = true;
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
                if (Environment.UserInteractive)
                {
                    LOG.Debug("Invalid switch: " + arg);
                    OutputUsage();
                    return;
                }
                else
                    throw new ArgumentException(arg);
            }

            // always set the instance name in the configuration object
            ManagementServiceConfiguration.SetInstanceName(instanceName);

            // configure the event log in case we want to log stuff
            EventLog eventLog = ManagementServiceInstaller.RegisterEventSource(instanceName);

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
            } else
            if (modeCount != 1)
            {
                OutputUsage();
                return;
            }
            
            // default useSSPI based on existance of a user id or password parameter
            useSSPI = (String.IsNullOrEmpty(repositoryUser) && String.IsNullOrEmpty(repositoryPassword));

            // allow sspi parameter to override if specified
            if (!String.IsNullOrEmpty(repositorySSPI))
                bool.TryParse(repositorySSPI, out useSSPI);

            if (useSSPI)
            {
                // only way this will happen is if the user specified /RepositorySSPI True
                if (!String.IsNullOrEmpty(repositoryUser) || !String.IsNullOrEmpty(repositoryPassword)) 
                {
                    Console.WriteLine("You may not specify a user id or password and use SSPI for the repository");
                    OutputUsage();
                    return;
                }
            } else
            {
                if (String.IsNullOrEmpty(repositoryUser) || String.IsNullOrEmpty(repositoryPassword))
                {
                    Console.WriteLine("You must specify both a user id and password if not using SSPI for the repository");
                    OutputUsage();
                    return;
                }
            }
            
            if (installService || unInstallService)
            {
                int test = 0;
                if (heartbeat != null && (!int.TryParse(heartbeat, out test) || test < 0))
                    heartbeat = null;
                if (servicePort != null && (!int.TryParse(servicePort, out test) || test < 0 || test > 65535))
                    servicePort = null;

                Assembly assembly = typeof(Program).Assembly;
                List<string> parms = new List<string>(8);
                parms.Add(InstallSwitch(OPTION_INSTANCE_NAME, instanceName));
                parms.Add(InstallSwitch(OPTION_FORCE, force ? Boolean.TrueString : Boolean.FalseString));
                if (repositoryHost != null)
                    parms.Add(InstallSwitch(OPTION_REPOSITORY_HOST, repositoryHost));
                if (repositoryDatabase != null)
                    parms.Add(InstallSwitch(OPTION_REPOSITORY_DB, repositoryDatabase));
                if (useSSPI)
                {
                    parms.Add(InstallSwitch(OPTION_REPOSITORY_SSPI, bool.TrueString));
                } else
                {
                    parms.Add(InstallSwitch(OPTION_REPOSITORY_SSPI, bool.FalseString));
                    parms.Add(InstallSwitch(OPTION_REPOSITORY_USER, repositoryUser));
                    parms.Add(InstallSwitch(OPTION_REPOSITORY_PASS, repositoryPassword));
                }
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

#pragma warning disable 0219
            bool configCorrupt = true;
            try
            {
                ManagementServicesSection section = (ManagementServicesSection)ConfigurationManager.GetSection(ManagementServicesSection.SectionName);
                if (section != null)
                {
                    if (section.ManagementServices.Count > 0)
                    {
                        ManagementServiceElement element = section.ManagementServices[instanceName];
                        if (element != null)
                        {
                            if (!String.IsNullOrEmpty(element.RepositoryServer))
                                configCorrupt = false;
                            if (!String.IsNullOrEmpty(element.RepositoryDatabase))
                                configCorrupt = false;
                            if (!element.RepositoryWindowsAuthentication)
                            {
                                if (!String.IsNullOrEmpty(element.RepositoryUsername))
                                    configCorrupt = false;
                                if (!String.IsNullOrEmpty(element.RepositoryPassword))
                                    configCorrupt = false;
                            }
                        }
                    }
                }
            }
            catch (ConfigurationErrorsException cee)
            {
                // leave suicide note
                EventInstance suicideEvent = new EventInstance((long)Status.ErrorConfigFoobar, (int)Category.General, EventLogEntryType.Error);
                StringBuilder builder = new StringBuilder();
                builder.Append(cee.Message);
                for (Exception e = cee.InnerException; e != null; e = e.InnerException)
                    builder.Append("\n").Append(e.Message);
                try
                {
                    eventLog.WriteEvent(suicideEvent, "SQLdmManagementService.exe.config", builder.ToString());
                }
                catch (Exception e)
                {
                    LOG.Error(e);
                }
                CommitSuicide(instanceName);
                return;
            }
#pragma warning restore 0219

             // configure logging 
            string diraccess = "Logs";
            try
            {
                ManagementServiceConfiguration.ConfigureLogging(logSection);
                string localData =
                    Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), instanceName);

                diraccess = instanceName;
                ManagementServiceConfiguration.CheckAccess(localData);
            } catch (UnauthorizedAccessException uae)
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

            if (servicePort != null)
            {
                int port = 5166;
                Int32.TryParse(servicePort, out port);
                ManagementServiceConfiguration.SetServicePort(port);
            }

            if (heartbeat != null)
            {
                int hb = 60;
                if (int.TryParse(heartbeat, out hb) && hb > 10)
                    ManagementServiceConfiguration.SetHeartbeatInterval(TimeSpan.FromSeconds(hb));
            }

            SqlConnectionInfo.DefaultConnectionTimeout = ManagementServiceConfiguration.GetManagementServiceElement().RepositoryTimeout;
            SqlHelper.CommandTimeout = ManagementServiceConfiguration.GetManagementServiceElement().CommandTimeout;

            if (haveRepositoryParms)
            {
                SqlConnectionInfo connectInfo = new SqlConnectionInfo();

                connectInfo.InstanceName = (String.IsNullOrEmpty(repositoryHost))
                                               ?
                                           ManagementServiceConfiguration.RepositoryHost
                                               : repositoryHost;

                connectInfo.DatabaseName = (String.IsNullOrEmpty(repositoryDatabase))
                                               ?
                                           ManagementServiceConfiguration.RepositoryDatabase
                                               : repositoryDatabase;

                if (!String.IsNullOrEmpty(repositoryUser))
                {
                    connectInfo.UserName = repositoryUser;
                    connectInfo.UseIntegratedSecurity = false;
                }
                if (!String.IsNullOrEmpty(repositoryPassword))
                {
                    connectInfo.Password = repositoryPassword;
                    connectInfo.UseIntegratedSecurity = false;
                }

                ManagementServiceConfiguration.SetRepositoryConnectInfo(connectInfo);
            }

            using (MainService mainSvc = new MainService())
            {
                if (runInteractive)
                    mainSvc.RunFromConsole();
                else
                    ServiceBase.Run(mainSvc);
            }

            return;
        }


        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LOG.FatalFormat("Unhandled exception: {0}", e.IsTerminating ? "Terminating" : "Continuing");
            LOG.Fatal(e.ExceptionObject);
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
            Console.WriteLine("Usage: ManagementService /install /InstanceName name /RepositoryHost host /RepositoryDB db ");
            Console.WriteLine("       Run the service installer to register a new instance of the management service. ");
            Console.WriteLine("       After installing the service you can start the new instance using the Net Start command: ");
            Console.WriteLine("       Net Start SQLdmMgmtSvc$InstanceName");
            Console.WriteLine("");
            Console.WriteLine("       ManagementService /install /InstanceName name [/RepositoryHost host] [/RepositoryDB db] [/force] ");
            Console.WriteLine("       Run the service installer to unregister an instance of the management service. ");
            Console.WriteLine("       In order to use the /RepositoryHost and /RepositoryDB parameters you must also specify /force. ");
            Console.WriteLine("       It is not recommended to use these parameters unless directed to by Technical Support.");
            Console.WriteLine("");
            Console.WriteLine("       ManagementService /console /InstanceName name [/RepositoryHost host] [/RepositoryDB db] [/Log log4net] ");
            Console.WriteLine("       Run the service in a cmd window.  Options /RepositoryHost, /RepositoryDB and /Log will override ");
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
                    Status.ManagementServiceStopFatal,
                    Category.General);
                // wait a second
                System.Threading.Thread.Sleep(1000);
                // bang!
                Stop();
            }
        }
    }
}
