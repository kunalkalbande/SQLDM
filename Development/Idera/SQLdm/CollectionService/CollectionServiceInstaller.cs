
namespace Idera.SQLdm.CollectionService
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Configuration;
    using System.Configuration.Install;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Tcp;
    using System.Text;
    using Idera.SQLdm.CollectionService.Configuration;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common;
    using System.Net;
    using System.Net.NetworkInformation;

    [RunInstaller(true)]
    public partial class CollectionServiceInstaller : Installer
    {
        public const string BASE_SERVICE_NAME = "SQLdmCollectionService";

        private string instanceName;
        private bool force;
  
        public CollectionServiceInstaller()
        {
            InitializeComponent();
            Installers.Remove(perfCounterInstaller);
        }

        public override void Install(System.Collections.IDictionary stateSaver)
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

            Customize(true);

            base.Install(stateSaver);
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
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

            // sets values based on instance name
            Customize(true);

            base.Uninstall(savedState);
        }

        protected override void OnAfterInstall(System.Collections.IDictionary savedState)
        {
            string mgtServiceAddress = Context.Parameters[Program.OPTION_MANAGEMENT_HOST];
            if (string.IsNullOrEmpty(mgtServiceAddress))
                mgtServiceAddress = Program.OPTION_MANAGEMENT_HOST_DEFAULT;

            string smgtsvcPort = Context.Parameters[Program.OPTION_MANAGEMENT_PORT];
            if (string.IsNullOrEmpty(smgtsvcPort))
                smgtsvcPort = Program.OPTION_MANAGEMENT_PORT_DEFAULT;
            
            string heartbeat = Context.Parameters[Program.OPTION_HEARTBEAT_INTERVAL];
            if (string.IsNullOrEmpty(heartbeat))
                heartbeat = Program.OPTION_HEARTBEAT_INTERVAL_DEFAULT;

            string sport = Context.Parameters[Program.OPTION_SERVICE_PORT];
            if (string.IsNullOrEmpty(sport))
                sport = Program.OPTION_SERVICE_PORT_DEFAULT;

            string tracerx = Context.Parameters[Program.OPTION_LOG_CONFIG_SECTION];
            if (string.IsNullOrEmpty(tracerx))
                tracerx = Program.OPTION_LOG_CONFIG_SECTION_DEFAULT;

            // convert strings to ints
            int heartbeatInterval = 180;
            int.TryParse(heartbeat, out heartbeatInterval);
            int servicePort = 5167;
            int.TryParse(sport, out servicePort);
            int mgtServicePort = 5166;
            int.TryParse(smgtsvcPort, out mgtServicePort);

            string machineName = Environment.MachineName;
            string address = null;

            try
            {
                address = IPGlobalProperties.GetIPGlobalProperties().HostName;
            }
            catch (Exception )
            {
                /* */
            }
            if (address == null)
                address = machineName;

            // connect to the management service and register this instance
            try
            {
                ConfigureRemoting();
                string url = String.Format("tcp://{0}:{1}/Management", mgtServiceAddress, mgtServicePort);
                IManagementService2 managementService = RemotingHelper.GetObject<IManagementService2>(url);
                managementService.RegisterCollectionService(machineName, instanceName, address, servicePort, force);
            } catch (Exception e)
            {
                Context.LogMessage(e.Message);
            }
            // update the local configuration file
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Context.LogMessage("Updating config: " + configuration.FilePath);

            // Get/Create the Idera.SQLdm configuration section
            CollectionServicesSection mss = configuration.GetSection("Idera.SQLdm") as CollectionServicesSection;
            if (mss == null)
            {
                mss = new CollectionServicesSection();
                configuration.Sections.Add("Idera.SQLdm", mss);
                Context.LogMessage("Creating Idera.SQLdm config file section");
            }

            // Get/Create the entry for this service instance
            CollectionServiceElement element = mss.CollectionServices[instanceName];
            if (element == null)
            {
                element = new CollectionServiceElement();
                element.InstanceName = instanceName;
                mss.CollectionServices.Add(element);
                Context.LogMessage("Creating entry for instance in config file");
            }
            else
                Context.LogMessage("Updating existing entry in config file");

            // set the values
            element.ManagementServiceAddress = mgtServiceAddress;
            element.ManagementServicePort = mgtServicePort;
            element.HeartbeatIntervalSeconds = heartbeatInterval;
            element.ServicePort = servicePort;
            element.TracerXSectionName = tracerx;

            if (element.DefaultSqlCommandTimeout == 0)
                element.DefaultSqlCommandTimeout = Constants.DefaultCommandTimeout;

            if (element.DefaultSqlConnectionTimeout == 0)
                element.DefaultSqlConnectionTimeout = Constants.DefaultConnectionTimeout;

            // save changes
            configuration.Save();
            
            base.OnAfterInstall(savedState);
        }

        protected void UnRegister(string mgtServiceAddress, int mgtServicePort)
        {
            try
            {
                ConfigureRemoting();
                string url = String.Format("tcp://{0}:{1}/Management", mgtServiceAddress, mgtServicePort);
                IManagementService2 managementService = RemotingHelper.GetObject<IManagementService2>(url);
                managementService.UnregisterCollectionService(Environment.MachineName, instanceName);
            }
            catch (Exception e)
            {
                Context.LogMessage(e.Message);
//                if (!(e is NotImplementedException))
//                {
//                    if (!force)
//                        throw;
//                }
            }
        }
        
        protected void ConfigureRemoting()
        {
            // register a client channel
            IDictionary properties = new System.Collections.Specialized.ListDictionary();

            properties = new System.Collections.Specialized.ListDictionary();
            properties["name"] = "tcp-client";
            properties["impersonationLevel"] = "None";
            properties["impersonate"] = false;
            properties["secure"] = true;

            BinaryClientFormatterSinkProvider clientSinkProvider = new BinaryClientFormatterSinkProvider();

            TcpClientChannel tcpClientChannel = new TcpClientChannel(properties, clientSinkProvider);
            ChannelServices.RegisterChannel(tcpClientChannel, true);

        }
        
        protected override void OnAfterUninstall(System.Collections.IDictionary savedState)
        {
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Context.LogMessage("Updating config: " + configuration.FilePath);

            // connect to the management service and unregister us
            CollectionServicesSection mss = configuration.GetSection("Idera.SQLdm") as CollectionServicesSection;
            if (mss != null)
            {
                CollectionServiceElement element = mss.CollectionServices[instanceName];
                if (element != null)
                {
                    UnRegister(element.ManagementServiceAddress, element.ManagementServicePort);
                    mss.CollectionServices.Remove(element);
                    configuration.Save();
                }
            }

            base.OnAfterUninstall(savedState);
        }

        // Customize the name of the perf counter category prior to installing
        private void Customize(bool install)
        {
            if (Context != null)
            {
                // set a new name for the service
                StringBuilder sb = new StringBuilder(BASE_SERVICE_NAME);
                sb.Append('$').Append(instanceName);

                this.perfCounterInstaller.CategoryName = sb.ToString();

//                if (install)
//                {  // modify the assembly path and pass the instance name
//                    string assemblyPath = this.Context.Parameters["assemblypath"];
//                    sb.Length = 0;
//                    // The installer will enclose the assemblypath setting in quotes when
//                    // it writes the ImagePath value to the registry.  We have to append the 
//                    // closing quote to the program name and then add a space and an open
//                    // quote to the parameters because of the traling quote added by the installer.
//                    sb.Append(assemblyPath).Append("\" \"/");
//                    sb.Append(Program.OPTION_INSTANCE_NAME);
//                    sb.Append("\" \"").Append(instanceName);
//                    this.Context.Parameters["assemblypath"] = sb.ToString();
//                }
            }
        }

        private void perfCounterInstaller_BeforeInstall(object sender, InstallEventArgs e)
        {
            Context.LogMessage("About to install performance counters...");
        }

        private void perfCounterInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            Context.LogMessage("Finished installing performance counters...");
        }
        
        /// <summary>
        /// Make sure the event source exists.  The installer is responsible
        /// for registering the service and for creating the event source.  This
        /// is being left in for the cases when the code is running in console mode.  
        /// It will only create the event source if one does not already exist.
        /// </summary>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public static EventLog RegisterEventSource(string instanceName)
        {
            string sourceName = BASE_SERVICE_NAME + "$" + instanceName;
            if (!EventLog.SourceExists(sourceName))
            {
                EventLog log;

                Assembly messageAssembly = typeof (Idera.SQLdm.Common.Messages.MessageDll).Assembly;
                string assemblyPath = messageAssembly.Location;

                EventSourceCreationData escd = new EventSourceCreationData(sourceName, "Application");
                escd.CategoryCount = 3;
                escd.CategoryResourceFile = assemblyPath;
                escd.MessageResourceFile = assemblyPath;

                EventLog.CreateEventSource(escd);
            }

            return new EventLog("Application", ".", sourceName);
        }
    }
}