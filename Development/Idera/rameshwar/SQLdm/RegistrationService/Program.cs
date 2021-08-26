using BBS.TracerX;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.RegistrationService.Configuration;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Idera.SQLdm.RegistrationService
{
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Name = "FullTrust")]
    [System.Security.Permissions.PermissionSetAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Name = "FullTrust")]

    [System.ComponentModel.RunInstaller(true)]
    public class MyInstaller : RegistrationServiceInstaller { }

    public static class Program
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("Program");

        public const string SERVICE_NAME_BASE = "SQLdmRegistrationService";

        public const string OPTION_REPOSITORY_HOST = "RepositoryHost";
        public const string OPTION_REPOSITORY_HOST_DEFAULT = "localhost";
        public const string OPTION_REPOSITORY_DB = "RepositoryDB";
        public const string OPTION_REPOSITORY_DB_DEFAULT = "SQLdmRepository";
        public const string OPTION_REPOSITORY_USER = "RepositoryUser";
        public const string OPTION_REPOSITORY_USER_DEFAULT = null;
        public const string OPTION_REPOSITORY_PASS = "RepositoryPassword";
        public const string OPTION_REPOSITORY_PASS_DEFAULT = null;
        public const string OPTION_REPOSITORY_SSPI = "RepositorySSPI";
        public const string OPTION_REPOSITORY_SSPI_DEFAULT = null;

        public const string OPTION_INSTANCE_NAME_DEFAULT = "Default";
        public const string OPTION_INSTANCE_NAME = "InstanceName";
        public const string OPTION_SERVICE_PORT = "Port";
        public const string OPTION_LOG_CONFIG_SECTION = "Log";
        public const string OPTION_RUN_INTERACTIVE = "console";
        public const string OPTION_INSTALL_SERVICE = "install";
        public const string OPTION_UNINSTALL_SERVICE = "uninstall";
        public const string OPTION_FORCE = "Force";

        public static string INSTALL_INFO_FILE = "InstallInfo.ini";
        public static string CWF_SECTION_NAME = "CWFInformation";

        static void Main(string[] args)
        {
            try
            {
                bool runInteractive = false;
                bool installService = false;
                bool unInstallService = false;
                bool force = false;
                bool useSSPI = true;
                string instanceName = OPTION_INSTANCE_NAME_DEFAULT;
                string servicePort = null;
                string logSection = null;
                bool haveRepositoryParms = false;

                string repositoryHost = null;
                string repositoryDatabase = null;
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
                            instanceName = args[++i];

                        continue;
                    }
                    if (comparer.Compare(arg, OPTION_SERVICE_PORT) == 0)
                    {
                        if (i + 1 < argCount && args[i + 1].Length > 0)
                            servicePort = args[++i];

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
                    /*if(comparer.Compare(arg, OPTION_CONNECTION_USER) == 0)
                    {

                    }*/
                    if (Environment.UserInteractive)
                    {
                        LOG.Debug("Invalid switch: " + arg);
                        OutputUsage();
                        return;
                    }
                    else
                        throw new ArgumentException(arg);
                }

                RegistrationServiceConfiguration.SetInstanceName(instanceName);

                InitLogging();

                int modeCount = 0;

                if (runInteractive) modeCount++;
                if (installService) modeCount++;
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
                }
                else
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

                    if (servicePort != null && (!int.TryParse(servicePort, out test) || test < 0 || test > 65535))
                        servicePort = null;

                    IDictionary state = new Hashtable();
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
                    }
                    else
                    {
                        parms.Add(InstallSwitch(OPTION_REPOSITORY_SSPI, bool.FalseString));
                        parms.Add(InstallSwitch(OPTION_REPOSITORY_USER, repositoryUser));
                        parms.Add(InstallSwitch(OPTION_REPOSITORY_PASS, repositoryPassword));
                    }
                    if (servicePort != null)
                        parms.Add(InstallSwitch(OPTION_SERVICE_PORT, servicePort));

                    if (logSection != null)
                        parms.Add(InstallSwitch(OPTION_LOG_CONFIG_SECTION, logSection));

                    string[] iargs = new string[parms.Count];
                    parms.CopyTo(iargs);
                    AssemblyInstaller installer = new AssemblyInstaller(assembly, iargs);

                    try
                    {
                        if (installService)
                        {
                            installer.Install(state);
                        }
                        else
                        {
                            installer.Uninstall(state);
                        }
                    }
                    catch (Exception ex)
                    {
                        LOG.Error("Failed to perform install/uninstall.", ex);
                    }

                    return;
                }

                SqlConnectionInfo.DefaultConnectionTimeout = RegistrationServiceConfiguration.GetRegistrationServiceElement().RepositoryTimeout;
                SqlHelper.CommandTimeout = RegistrationServiceConfiguration.GetRegistrationServiceElement().CommandTimeout;

                if (haveRepositoryParms)
                {
                    SqlConnectionInfo connectInfo = new SqlConnectionInfo();

                    connectInfo.InstanceName = (String.IsNullOrEmpty(repositoryHost))
                                                   ?
                                               RegistrationServiceConfiguration.RepositoryHost
                                                   : repositoryHost;

                    connectInfo.DatabaseName = (String.IsNullOrEmpty(repositoryDatabase))
                                                   ?
                                               RegistrationServiceConfiguration.RepositoryDatabase
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

                    RegistrationServiceConfiguration.SetRepositoryConnectInfo(connectInfo);
                }

                using (RegistrationService mainSvc = new RegistrationService())
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

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LOG.FatalFormat("Unhandled exception: {0}", e.IsTerminating ? "Terminating" : "Continuing");
            LOG.Fatal(e.ExceptionObject);
        }

        private static void OutputUsage()
        {
            Console.WriteLine("");
            Console.Clear();
            Console.WriteLine("Usage: RegistrationService /install /InstanceName name /ManagementHost host /ManagementPort 5170 ");
            Console.WriteLine("       Run the service installer to register a new instance of the service. ");
            Console.WriteLine("       After installing the service you can start the new instance using the Net Start command: ");
            Console.WriteLine("       Net Start SQLdmRegistrationService$InstanceName");
            Console.WriteLine("");
            Console.WriteLine("       RegistrationService /install /InstanceName name [/ManagementHost host] [/ManagementPort 5166] [/force] ");
            Console.WriteLine("       Run the service installer to unregister an instance of the collection service. ");
            Console.WriteLine("       In order to use the /ManagementHost and /ManagementPort parameters you must also specify /force. ");
            Console.WriteLine("       It is not recommended to use these parameters unless directed to by Technical Support.");
            Console.WriteLine("");
            Console.WriteLine("       RegistrationService /console /InstanceName name [/ManagementHost host] [/ManagementPort 5166] [/Log log4net] ");
            Console.WriteLine("       Run the service in a cmd window.  Options /ManagementHost, /ManagementPort and /Log will override ");
            Console.WriteLine("       corresponding values stored in the config file.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.ReadKey();
        }

        private static void InitLogging()
        {
            bool configRc = Logger.Xml.Configure();
            Logger.FileLogging.Name = "SQLdmRegistrationService.tx1";
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
