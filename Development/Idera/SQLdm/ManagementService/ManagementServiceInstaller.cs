namespace Idera.SQLdm.ManagementService
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Configuration.Install;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Management;
    using System.Management.Instrumentation;
    using System.Net.NetworkInformation;
    using System.Reflection;
    using System.Text;
    using Idera.SQLdm.Common;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.ManagementService.Configuration;
    using Idera.SQLdm.ManagementService.Helpers;
    using Microsoft.ApplicationBlocks.Data;

    [RunInstaller(true)]
    public partial class ManagementServiceInstaller : Installer
    {
        private static string BASE_SERVICE_NAME = "SQLdmManagementService";

        private string  instanceName;
        private string  repositoryHost;
        private string  repositoryDatabase;
        private bool repositoryWindowsAuthentication;
        private string  repositoryUser;
        private string  repositoryPassword;
        private int     servicePort;
        private string tracerXConfiguration;

        private bool force;
        private bool enteredAfterLogic;

        public ManagementServiceInstaller()
        {
            InitializeComponent();

            // remove the perf counter installer - we will configure in the service
            this.Installers.Remove(perfCounterInstaller);

            instanceName = "Default";
            servicePort = 5166;
            repositoryHost = Environment.MachineName;
            repositoryDatabase = "SQLdmRepository";
            repositoryWindowsAuthentication = true;
            tracerXConfiguration = "TracerX";
        }

        private void Configure()
        {
            instanceName = Context.Parameters[Program.OPTION_INSTANCE_NAME];
            if (string.IsNullOrEmpty(instanceName))
            {
                Context.LogMessage("Instance name not specified - setting to " + Program.OPTION_INSTANCE_NAME_DEFAULT);
                instanceName = Program.OPTION_INSTANCE_NAME_DEFAULT;
            }
            else
                Context.LogMessage("Instance name is: " + instanceName);

            // get the force parameter
            string sforce = Context.Parameters[Program.OPTION_FORCE];
            if (!string.IsNullOrEmpty(sforce))
                Boolean.TryParse(sforce, out force);

            this.perfCounterInstaller.CategoryName = BASE_SERVICE_NAME + "$" + instanceName + ":Statistics";

            ManagementServicesSection section = ManagementServicesSection.GetSection();
            ManagementServiceElement element = section.ManagementServices[instanceName];
            if (element != null)
            {
                repositoryHost = element.InstanceName;
                repositoryDatabase = element.RepositoryDatabase;
                repositoryUser = element.RepositoryUsername;
                repositoryPassword = element.GetRepositoryPassword();
                repositoryWindowsAuthentication = element.RepositoryWindowsAuthentication;
                servicePort = element.ServicePort;
                tracerXConfiguration = element.TracerXSectionName;
            }

            string option = Context.Parameters[Program.OPTION_REPOSITORY_HOST];
            if (!string.IsNullOrEmpty(option))
                repositoryHost = option;

            option = Context.Parameters[Program.OPTION_REPOSITORY_DB];
            if (!string.IsNullOrEmpty(option))
                repositoryDatabase = option;

            bool useSSPI = true;
            option = Context.Parameters[Program.OPTION_REPOSITORY_SSPI];
            if (!string.IsNullOrEmpty(option))
            {
                Boolean.TryParse(option, out useSSPI);
                repositoryWindowsAuthentication = useSSPI;
            } 

            if (!repositoryWindowsAuthentication)
            {
                option = Context.Parameters[Program.OPTION_REPOSITORY_USER];
                if (!string.IsNullOrEmpty(option))
                    repositoryUser = option;

                option = Context.Parameters[Program.OPTION_REPOSITORY_PASS];
                if (!string.IsNullOrEmpty(option))
                    repositoryPassword = option;
            }

            string tracerx = Context.Parameters[Program.OPTION_LOG_CONFIG_SECTION];
            if (!string.IsNullOrEmpty(tracerx))
                tracerXConfiguration = tracerx;

            string sport = Context.Parameters[Program.OPTION_SERVICE_PORT];
            if (string.IsNullOrEmpty(sport))
            {
                sport = Program.OPTION_SERVICE_PORT_DEFAULT;
            }
            servicePort = 5166;
            int.TryParse(sport, out servicePort);
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            Configure();

            try
            {
                base.Install(stateSaver);
            } catch (Exception e)
            {
                if (force && !enteredAfterLogic)
                {
                    Context.LogMessage(e.Message);
                    Context.LogMessage("Forcing post install logic...");
                    try
                    {
                        OnAfterInstall(stateSaver);
                    }
                    catch (Exception e2)
                    {
                        Context.LogMessage(e2.Message);
                    }
                }
                else
                    throw;
            }
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            Configure();

            try
            {
                base.Uninstall(savedState);
            }
            catch (Exception e)
            {
                if (force && !enteredAfterLogic)
                {
                    Context.LogMessage(e.Message);
                    Context.LogMessage("Forcing post uninstall logic...");
                    try
                    {
                        OnAfterUninstall(savedState);
                    }
                    catch (Exception e2)
                    {
                        Context.LogMessage(e2.Message);
                    }
                }
                else
                    throw;
            }
        }

        protected override void OnAfterInstall(System.Collections.IDictionary savedState)
        {
            enteredAfterLogic = true;

            // configure a connect info object from parameter and default values
            SqlConnectionInfo connectInfo = new SqlConnectionInfo();

            connectInfo.InstanceName = repositoryHost;
            connectInfo.DatabaseName = repositoryDatabase;
            connectInfo.UseIntegratedSecurity = repositoryWindowsAuthentication;
            if (!connectInfo.UseIntegratedSecurity)
            {
                connectInfo.UserName = repositoryUser;
                connectInfo.Password = repositoryPassword;
            }
            connectInfo.ApplicationName = Constants.ManagementServiceConnectionStringApplicationName;

            string machineName = Environment.MachineName;
            string address = null;

            try
            {
                address = IPGlobalProperties.GetIPGlobalProperties().HostName;
            }
            catch (Exception)
            {
                /* */
            }
            if (address == null)
                address = machineName;

            try
            {
                RepositoryHelper.AddManagementService(connectInfo.ConnectionString,
                                                    instanceName,
                                                    machineName,
                                                    address,
                                                    servicePort);
            } catch (Exception e)
            {
                Context.LogMessage(e.Message);
            }

            // update the local configuration file
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Context.LogMessage("Updating config: " + configuration.FilePath);

            ManagementServicesSection section = configuration.GetSection("Idera.SQLdm") as ManagementServicesSection;
            if (section == null)
            {
                section = new ManagementServicesSection();
                configuration.Sections.Add("Idera.SQLdm", section);
                Context.LogMessage("Creating Idera.SQLdm config file section");
            }

            ManagementServiceElement element = section.ManagementServices[instanceName];
            if (element == null)
            {
                element = new ManagementServiceElement();
                element.InstanceName = instanceName;
                section.ManagementServices.Add(element);
                Context.LogMessage("Creating entry for instance in config file");
            }
            else
                Context.LogMessage("Updating existing entry in config file");

            element.RepositoryServer = repositoryHost;
            element.RepositoryDatabase = repositoryDatabase;
            element.RepositoryWindowsAuthentication = repositoryWindowsAuthentication;
            if (!repositoryWindowsAuthentication)
            {
                element.RepositoryUsername = repositoryUser;
                element.RepositoryPassword = repositoryPassword;
            }
            element.TracerXSectionName = tracerXConfiguration;
            element.ServicePort = servicePort;

            if (element.CommandTimeout == 0)
                element.CommandTimeout = 180;

            if (element.RepositoryTimeout == 0)
                element.RepositoryTimeout = 30;

            configuration.Save();

            base.OnAfterInstall(savedState);
        }

        private bool GetManagementServiceId(string connectString, string instanceName, string machineName, out Guid id)
        {
            string instance = instanceName.ToLower();
            string machine = machineName.ToLower();
            foreach (ManagementServiceInfo msi in RepositoryHelper.GetManagementServices(connectString, null))
            {
                if (msi.InstanceName.ToLower() == instance &&
                    msi.MachineName.ToLower() == machine)
                {
                    id = msi.Id;
                    return true;
                }
            }

            id = default(Guid);
            return false;
        }

        protected override void OnAfterUninstall(System.Collections.IDictionary savedState)
        {
            enteredAfterLogic = true;

            SqlConnectionInfo connectInfo = new SqlConnectionInfo();

            connectInfo.InstanceName = repositoryHost;
            connectInfo.DatabaseName = repositoryDatabase;
            connectInfo.UseIntegratedSecurity = repositoryWindowsAuthentication;
            if (!connectInfo.UseIntegratedSecurity)
            {
                connectInfo.UserName = repositoryUser;
                connectInfo.Password = repositoryPassword;
            }
            connectInfo.ApplicationName = Constants.ManagementServiceConnectionStringApplicationName;

            // get the management service id using the instance and machine name
            Guid serviceId;
            try
            {
                Context.LogMessage("Removing management service registration from the repository");
                if (GetManagementServiceId(connectInfo.ConnectionString, instanceName, Environment.MachineName, out serviceId))
                {
                    using (SqlConnection connection = connectInfo.GetConnection())
                    {
                        connection.Open();

                        SqlParameter[] parms =
                            SqlHelperParameterCache.GetSpParameterSet(connection, "p_RemoveManagementService", true);
                        SqlHelper.AssignParameterValues(parms, serviceId);
                        SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, "p_RemoveManagementService",
                                                  parms);

                        if (0 == (int)parms[0].Value)
                        {
                            Context.LogMessage("Management service instance removed from the repository.");
                        }
                    }
                }
                else
                {
                    Context.LogMessage("Management service registration not found in the repository");
                    if (!force)
                    {
                        Context.LogMessage("Specify /Force to force processing the config file");
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Context.LogMessage(e.Message);
            }

            // update the local configuration file
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Context.LogMessage("Updating config: " + configuration.FilePath);

            ManagementServicesSection mss = configuration.GetSection("Idera.SQLdm") as ManagementServicesSection;
            if (mss == null)
            {
                mss = new ManagementServicesSection();
                configuration.Sections.Add("Idera.SQLdm", mss);
            }

            ManagementServiceElement element = mss.ManagementServices[instanceName];
            if (element == null)
            {
                Context.LogMessage("Entry for instance not found in config file");
            }
            else
            {
                mss.ManagementServices.Remove(element);
                Context.LogMessage("Removing instance entry from config file");
            }

            configuration.Save();

            // kill the WMI namespace
            try
            {
                RemoveWmiNameSpace();
            } catch
            {
                /* */
            }

            // kill the performance counter category
            try
            {
                PerformanceCounterCategory.Delete(perfCounterInstaller.CategoryName);
            } catch
            {
                /* */
            }

            base.OnAfterUninstall(savedState);
        }

//        private void InitializeConfiguration()
//        {
//            SqlConnectionInfo connectInfo = new SqlConnectionInfo();
//
//            ManagementServiceConfiguration.SetInstanceName(instanceName);
//
//            ManagementServicesSection mss = ManagementServicesSection.GetSection();
//            ManagementServiceElement element = mss.ManagementServices[instanceName];
//            if (element == null)
//                element = new ManagementServiceElement();
//
//            string option = Context.Parameters[Program.OPTION_REPOSITORY_HOST];
//            if (!string.IsNullOrEmpty(option))
//                connectInfo.InstanceName = option;
//            else 
//                connectInfo.InstanceName = element.RepositoryServer;
//
//            option = Context.Parameters[Program.OPTION_REPOSITORY_DB];
//            if (!string.IsNullOrEmpty(option))
//                connectInfo.DatabaseName = option;
//            else
//                connectInfo.DatabaseName = element.RepositoryDatabase;
//
//            bool useSSPI = true;
//            option = Context.Parameters[Program.OPTION_REPOSITORY_SSPI];
//            if (!string.IsNullOrEmpty(option))
//            {
//                Boolean.TryParse(option, out useSSPI);
//            } else
//                useSSPI = ManagementServiceConfiguration.RepositoryUseSSPI;
//            connectInfo.UseIntegratedSecurity = useSSPI;
//
//            if (!connectInfo.UseIntegratedSecurity)
//            {
//                option = Context.Parameters[Program.OPTION_REPOSITORY_USER];
//                if (!string.IsNullOrEmpty(option))
//                    connectInfo.UserName = option;
//                else
//                    connectInfo.UserName = element.RepositoryUsername;
//
//                option = Context.Parameters[Program.OPTION_REPOSITORY_PASS];
//                if (!string.IsNullOrEmpty(option))
//                    connectInfo.Password = option;
//                else
//                    connectInfo.Password = element.RepositoryPassword;
//            }
//
//            ManagementServiceConfiguration.SetRepositoryConnectInfo(connectInfo);
//        }

        private void perfCounterInstaller_BeforeInstall(object sender, InstallEventArgs e)
        {
            Context.LogMessage("About to install performance counters...");
        }

        private void perfCounterInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            Context.LogMessage("Finished installing performance counters...");
        }

        public void RemoveWmiNameSpace()
        {
            Assembly assembly = GetType().Assembly;

            // get the namespace from the assembly attribute
            InstrumentedAttribute nameSpaceAttr = Attribute.GetCustomAttribute(assembly, typeof (InstrumentedAttribute)) as InstrumentedAttribute;
            if (nameSpaceAttr == null)
                return;
            string nameSpace = nameSpaceAttr.NamespaceName;

            // get all defined management classes
            List<string> managementClasses = new List<string>();
            foreach (Type type in assembly.GetExportedTypes())
            {
                ManagedNameAttribute ica = Attribute.GetCustomAttribute(type, typeof(ManagedNameAttribute)) as ManagedNameAttribute;
                if (ica != null)
                    managementClasses.Add(ica.Name);
            }
            // see if there are any instance in the namespace
            foreach (string managementClassName in managementClasses)
            {
                using (ManagementClass mc = new ManagementClass(String.Format("{0}:{1}", nameSpace, managementClassName)))
                {
                    ManagementObjectCollection moc = mc.GetInstances();
                    if (moc.Count > 0)
                    {
                        // TODO: Log message that path not deleted because it is still in use
                        return;
                    }
                }
            }

            // split the namespace and push the paths on the stack
            Stack<string> stack = new Stack<string>();
            StringBuilder builder = new StringBuilder();

            string[] part = nameSpace.Split('/', '\\');
            int end = part.Length - 1;
            for (int i = 0; i < end; i++)
            {
                if (i > 0)
                    stack.Push(part[i]);

                if (builder.Length > 0)
                    builder.Append('\\');
                builder.Append(part[i]);
                
                stack.Push(builder.ToString());
            }
            stack.Push(part[end]);

            // process the stack
            while (stack.Count >= 2)
            {
                string name = stack.Pop();
                string path = stack.Pop();
                ManagementScope scope = new ManagementScope(path);
                try
                {
                    scope.Connect();

                    // get the namespace class for the scope
                    ObjectGetOptions ogo = new ObjectGetOptions();
                    using (ManagementClass mc = new ManagementClass(scope, new ManagementPath("__NAMESPACE"), ogo))
                    {
                        bool foundit = false;
                        // get all instances of the namespace
                        ManagementObjectCollection moc = mc.GetInstances();
                        foreach (ManagementObject mo in moc)
                        {
                            try
                            {
                                // look for the specific namespace
                                if (mo["Name"].ToString() == name)
                                {
                                    // delete the namespace
                                    mo.Delete();
                                    foundit = true;
                                }
                            }
                            catch (Exception)
                            {
                                // TODO: Log the error quit
                                return;
                            }
                        }
                        // if we didn't find the object to delete or there were more than one namespace then quit
                        if (!foundit || moc.Count > 1)
                            break;
                    }
                } catch (Exception)
                {
                    // TODO: log namespace not found
                    break;
                }
            }
        }

        public static void RemovePerformanceCounters(string catName)
        {
            try
            {
                if (PerformanceCounterCategory.Exists(catName))
                {
                    PerformanceCounterCategory.Delete(catName);
                }
            } catch (Exception)
            {
                /* */
            }
        }

        /// <summary>
        /// The installer is responsible for creating the performance counters.  
        /// Because the service can be configured outside the installer, we
        /// attempt to create them if they are missing.  This method will throw
        /// an exception if the counter creation fails.
        /// </summary>
        /// <param name="catName"></param>
        public static void RegisterPerformanceCounters(string catName)
        {
            if (!PerformanceCounterCategory.Exists(catName))
            {
                // Create new counter if it does not exist
                CounterCreationDataCollection ccdc = new CounterCreationDataCollection();
                ccdc.AddRange(new System.Diagnostics.CounterCreationData[] {
                    new System.Diagnostics.CounterCreationData("ActiveWorkers", "Number of threads processing queued work", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
                    new System.Diagnostics.CounterCreationData("WaitingWorkers", "Number of threads waiting for work to be queued", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
                    new System.Diagnostics.CounterCreationData("Task Queue Length", "Number of tasks waiting for execution", System.Diagnostics.PerformanceCounterType.NumberOfItems32),
                    new System.Diagnostics.CounterCreationData("Tasks Queued/sec", "Average number of tasks queued per second", System.Diagnostics.PerformanceCounterType.RateOfCountsPerSecond32),
                    new System.Diagnostics.CounterCreationData("Avg. Task Time", "Average time to process a queued task", System.Diagnostics.PerformanceCounterType.AverageTimer32),
                    new System.Diagnostics.CounterCreationData("Avg. Task Time Base", "Base counter for Avg. Task Time", System.Diagnostics.PerformanceCounterType.AverageBase)});

                PerformanceCounterCategory.Create(
                   catName, 
                   "SQLDM performance counters",
                   PerformanceCounterCategoryType.SingleInstance,
                   ccdc);
            }
        }

        /// <summary>
        /// The installer is responsible for adding the event source.  However, 
        /// if the service is added manually or the program is run from the 
        /// the command line then we need to try to create the event source.
        /// </summary>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public static EventLog RegisterEventSource(string instanceName)
        {
            EventLog log = new EventLog("Application");

            string sourceName = BASE_SERVICE_NAME + "$" + instanceName;
            try
            {
                if (!EventLog.SourceExists(sourceName))
                {
                    Assembly messageAssembly = typeof (Idera.SQLdm.Common.Messages.MessageDll).Assembly;
                    string assemblyPath = messageAssembly.Location;

                    EventSourceCreationData escd = new EventSourceCreationData(sourceName, "Application");
                    escd.CategoryCount = 3;
                    escd.CategoryResourceFile = assemblyPath;
                    escd.MessageResourceFile = assemblyPath;

                    EventLog.CreateEventSource(escd);
                }
            }
            catch
                (Exception)
            {
                /* Don't upchuck because it could be called from the installer */
            }

            log.Source = sourceName;            
            return log;
        }        
    }
}