using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Text;
using System.Configuration;
using System.Diagnostics;

using Idera.SQLdm.PredictiveAnalyticsService;

namespace PredictiveAnalyticsService
{
    [RunInstaller(true)]
    public partial class PredictiveAnalyticsServiceInstaller : Installer
    {
        public const string BASE_SERVICE_NAME = "SQLdmPredictiveAnalyticsService";
        private PerformanceCounterInstaller perfCounterInstaller;

        private string instanceName;
        private bool   force;

        public PredictiveAnalyticsServiceInstaller()
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