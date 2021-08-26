using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using Idera.SQLdm.RegistrationService.Configuration;
using System.IO;

namespace Idera.SQLdm.RegistrationService
{
    [RunInstaller(true)]
    public partial class RegistrationServiceInstaller : Installer
    {
        public const string BASE_SERVICE_NAME = "SQLdmRegistrationService";
        private PerformanceCounterInstaller perfCounterInstaller;

        private string instanceName;
        private string repositoryHost;
        private string repositoryDatabase;
        private bool repositoryWindowsAuthentication;
        private string repositoryUser;
        private string repositoryPassword;
        private int servicePort;
        private string tracerXConfiguration;

        private bool   force;

        public RegistrationServiceInstaller()
        {
            InitializeComponent();

            this.perfCounterInstaller                = new PerformanceCounterInstaller();
            this.perfCounterInstaller.CategoryName   = "SQLdm";
            this.perfCounterInstaller.CategoryType   = PerformanceCounterCategoryType.SingleInstance;
            this.perfCounterInstaller.BeforeInstall += new System.Configuration.Install.InstallEventHandler(this.perfCounterInstaller_BeforeInstall);
            this.perfCounterInstaller.AfterInstall  += new System.Configuration.Install.InstallEventHandler(this.perfCounterInstaller_AfterInstall);

            this.perfCounterInstaller.Counters.AddRange(new System.Diagnostics.CounterCreationData[] 
            {
                new CounterCreationData("ActiveWorkers",  "Number of threads processing queued work",        PerformanceCounterType.NumberOfItems32),
                new CounterCreationData("WaitingWorkers", "Number of threads waiting for work to be queued", PerformanceCounterType.NumberOfItems32)
            });            
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            Configure();
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

        private void Configure()
        {
            instanceName = Context.Parameters[Program.OPTION_INSTANCE_NAME];

            RegistrationServicesSection section = RegistrationServicesSection.GetSection();
            RegistrationServiceElement element = section.RegistrationServices[instanceName];
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
                sport = "5170";
            }
            servicePort = 5170;
            int.TryParse(sport, out servicePort);
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
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            RegistrationServicesSection section = RegistrationServicesSection.GetSection();
            if (section == null)
            {
                section = new RegistrationServicesSection();
                configuration.Sections.Add("Idera.SQLdm", section);
            }

            RegistrationServiceElement element = section.RegistrationServices[instanceName];
            if (element == null)
            {
                element = new RegistrationServiceElement();
                element.InstanceName = instanceName;
                element.RepositoryServer = Environment.MachineName;
                element.RepositoryDatabase = "SQLdmRepository";
            }

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

            element.Save();
            configuration.Save();

            base.OnAfterInstall(savedState);
        }

        protected override void OnAfterUninstall(System.Collections.IDictionary savedState)
        {
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Context.LogMessage("Updating config: " + configuration.FilePath);

            /*
            // connect to the management service and unregister us
            CollectionServicesSection mss = configuration.GetSection("Idera.SQLdm") as CollectionServicesSection;
            if (mss != null)
            {
                CollectionServiceElement element = mss.CollectionServices[instanceName];
                if (element != null)
                {
                    //UnRegister(element.ManagementServiceAddress, element.ManagementServicePort);
                    mss.CollectionServices.Remove(element);
                    configuration.Save();
                }
            }
            */

            base.OnAfterUninstall(savedState);
        }

        private void Customize(bool install)
        {
            if (Context != null)
            {
                // set a new name for the service
                StringBuilder sb = new StringBuilder(BASE_SERVICE_NAME);
                sb.Append('$').Append(instanceName);

                this.perfCounterInstaller.CategoryName = sb.ToString();
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
    }
}