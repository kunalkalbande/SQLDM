using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceProcess;
using Idera.SQLdm.RegistrationService.Configuration;
using System.ServiceModel.Description;
using System.Threading;
using Idera.SQLdm.Service.ServiceContracts.v1;
using Idera.SQLdm.Service.Web;
using System.Management.Automation;
using System.Collections.ObjectModel;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Resources;

namespace Idera.SQLdm.RegistrationService
{
    public partial class RegistrationService : ServiceBase
    {
        private WebServiceHost BusHost;
        private WebServiceHost SQLdmRestBusHost;
        private WebServiceHost SQLdmRestBusHostHTTP;

        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("RegistrationService");
        public static string SQldmInstanceName = "Default";

        private ManualResetEvent stopEvent;
        private Thread main;
        private bool running;

        private object sync = new object();
        private bool consoleMode;

        public RegistrationService()
        {
            InitializeComponent();
            if (!System.Diagnostics.EventLog.SourceExists("SQLdmRegistrationService"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "SQLdmRegistrationService", "SQLdmRegistrationServiceLog");
            }

            eventLog1.Source = "SQLdmRegistrationService";
            eventLog1.Log = "SQLdmRegistrationServiceLog";
        }
        protected override void OnStart(string[] args)
        {
            LOG.InfoFormat("Current FileTraceLevel = {0}", LOG.FileTraceLevel);
            using (LOG.InfoCall("OnStart"))
            {
                if (!consoleMode)
                    RequestAdditionalTime(60000);

                LOG.Info("Starting main thread...");
                stopEvent = new ManualResetEvent(false);
                main = new Thread(new ThreadStart(Run));
                main.Start();
            }
        }

        public void Run()
        {
            using (LOG.InfoCall("Run"))
            {
                try
                {
                    running = true;
                    RegistrationServiceConfiguration.LogConfiguration();
                    string instanceName = RegistrationServiceConfiguration.InstanceName;

                    // Populate Master Recommendations for CWF
                    if (MasterRecommendations.MasterRecommendationsInformation == null || MasterRecommendations.MasterRecommendationsInformation.Count < 1)
                    {
                        MasterRecommendations.MasterRecommendationsInformation = Helpers.RepositoryHelper.GetMasterRecommendations(RegistrationServiceConfiguration.ConnectionString);
                    }

                    StartWebServices("http://localhost:" + Common.Constants.REGISTRATION_SERVICE_PORT_DEFAULT + "/SQLdm/",
                        "https://localhost:" + Common.Constants.REGISTRATION_SERVICE_HTTPS_PORT_DEFAULT + "/SQLdm/");
                }
                catch (Exception ex)
                {
                    running = false;
                    LOG.Error("Error occured on Run", ex);
                }
            }
        }

        public bool ExecuteRun()
        {
            bool isRunning = true;
            try
            {
                RegistrationServiceConfiguration.LogConfiguration();
                string instanceName = RegistrationServiceConfiguration.InstanceName;

                StartWebServices("http://localhost:" + Common.Constants.REGISTRATION_SERVICE_PORT_DEFAULT + "/SQLdm/",
                    "https://localhost:" + Common.Constants.REGISTRATION_SERVICE_HTTPS_PORT_DEFAULT + "/SQLdm/");
            }
            catch (Exception ex)
            {
                isRunning = false;
            }
            return isRunning;
        }

        public void AskServiceControlManagerForMoreTime(int milliSeconds)
        {
            if (!Environment.UserInteractive)
            {
                try
                {
                    this.RequestAdditionalTime(milliSeconds);
                }
                catch
                {
                    /* */
                }
            }
        }

        protected override void OnStop()
        {
            using (LOG.InfoCall("OnStop"))
            {
                try
                {
                    LOG.Info("Shutdown started...");
                    running = false;
                    BusHost.Close();

                    try
                    {
                        AskServiceControlManagerForMoreTime(65000);
                        stopEvent.Set();
                        main.Interrupt();

                        if (Thread.CurrentThread != main && !main.Join(TimeSpan.FromMinutes(1)))
                            main.Abort();
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Exception while waiting for main thread termination", e);
                    }

                    LOG.Info("Shutdown complete");
                }
                catch (Exception e)
                {
                    LOG.Error("Exception while waiting for web services to stop", e);
                }
            }
        }

        protected override void OnShutdown()
        {
            using (LOG.InfoCall("OnShutdown"))
            {
                base.Stop();
            }
        }

        internal void RunFromConsole()
        {
            LOG.InfoFormat("Current FileTraceLevel = {0}", LOG.FileTraceLevel);
            using (LOG.InfoCall("RunFromConsole"))
            {
                consoleMode = true;

                LOG.Info("Running Service.  Press a key to exit.");
                Console.WriteLine("\nService is running.  Press Ctrl-C to stop service.");
                OnStart(null);
                Console.ReadKey();
                LOG.Info("Exiting Service.");

                OnStop();
            }
        }

        void StartWebServices(string urlAuthority, string urlAuthorityRest)
        {
            BusHost = new WebServiceHost(typeof(CWFRegistrationService), new Uri(urlAuthority + ""));
            var busBinding = new WebHttpBinding(WebHttpSecurityMode.TransportCredentialOnly);
            var busEndpoint = BusHost.AddServiceEndpoint(typeof(IProduct), busBinding, urlAuthority + "");
            //busEndpoint.Behaviors.Add(new ErrorHandlerBehavior());
            busEndpoint.Behaviors.Add(new WebHttpBehavior { HelpEnabled = true });
            BusHost.Open();


            SQLdmRestBusHostHTTP = new WebServiceHost(typeof(WebService), new Uri(urlAuthority + "Rest/"));
            var sqldmServerManagerBusBindingHTTP = new WebHttpBinding(WebHttpSecurityMode.TransportCredentialOnly);
            sqldmServerManagerBusBindingHTTP.CrossDomainScriptAccessEnabled = true;
            var sqldmServerManagerBusEndpointHTTP = SQLdmRestBusHostHTTP.AddServiceEndpoint(typeof(IWebService1), sqldmServerManagerBusBindingHTTP, urlAuthority + "Rest/");
            sqldmServerManagerBusEndpointHTTP.Behaviors.Add(new WebHttpBehavior { HelpEnabled = true });
            sqldmServerManagerBusEndpointHTTP.Behaviors.Add(new EnableCrossOriginResourceSharingBehavior());
            SQLdmRestBusHostHTTP.Open();

            using (PowerShell PowerShellInstance1 = PowerShell.Create())
            {
                SQLdmRestBusHost = new WebServiceHost(typeof(WebService), new Uri(urlAuthorityRest + "Rest/"));
                var sqldmServerManagerBusBinding = new WebHttpBinding(WebHttpSecurityMode.Transport);
                sqldmServerManagerBusBinding.CrossDomainScriptAccessEnabled = true;
                var sqldmServerManagerBusEndpoint = SQLdmRestBusHost.AddServiceEndpoint(typeof(IWebService1), sqldmServerManagerBusBinding, urlAuthorityRest + "Rest/");
                sqldmServerManagerBusEndpoint.Behaviors.Add(new WebHttpBehavior { HelpEnabled = true });
                sqldmServerManagerBusEndpoint.Behaviors.Add(new EnableCrossOriginResourceSharingBehavior());
                SQLdmRestBusHost.Open();

                // Command to remove any certificates binded to https rest service port
                PowerShellInstance1.AddScript("netsh http delete sslcert ipport=0.0.0.0:" + Common.Constants.REGISTRATION_SERVICE_HTTPS_PORT_DEFAULT);
                // Command to create a new self signed certificate for the https rest service
                //PowerShellInstance1.AddScript("New-SelfSignedCertificate -CertStoreLocation cert:\\LocalMachine\\My -DnsName localhost");

                //Command to Import a Certificate
                string file = AppDomain.CurrentDomain.BaseDirectory + "idera.pfx";
                string file1 = AppDomain.CurrentDomain.BaseDirectory + "User Creation Scripts\\idera.pfx";
                string command1 = "certutil -f -p password -importpfx '" + file + "'";
                string command2 = "certutil -f -p password -importpfx '" + file1 + "'";
                LOG.InfoFormat("The command is {0}", command1);
                PowerShellInstance1.AddScript(command1);
                // Execute the command and get the output
                Collection<PSObject> PSOutput = PowerShellInstance1.Invoke();
                PSObject outputItem = PSOutput[0];
                LOG.InfoFormat("Result of add SSL Cert Binding is {0}", outputItem);
                if (null != outputItem && outputItem.ToString().Contains("FAILED"))
                {
                    LOG.ErrorFormat("File not found in Folder and trying in User Creation Scripts folder");
                    using (PowerShell PowerShellInstance3 = PowerShell.Create())
                    {
                        PowerShellInstance3.AddScript(command2);
                        PSOutput = PowerShellInstance3.Invoke();
                        outputItem = PSOutput[0];
                        LOG.InfoFormat("Result of add SSL Cert Binding is {0}", outputItem);
                    }

                }

                //Command to Add SSL Certificate binding
                var command = "netsh http add sslcert ipport=0.0.0.0:" + Common.Constants.REGISTRATION_SERVICE_HTTPS_PORT_DEFAULT + " certhash=42bcbc557ed3060d8dd5957917d89d5766f3cbbe appid=\"{0051d7ed-de72-46d3-ae44-97d566b1ca5a}\"";
                LOG.InfoFormat("The command to add SSL Cert Binding is {0}", command);
                using (PowerShell PowerShellInstance2 = PowerShell.Create())
                {
                    PowerShellInstance2.AddScript(command);
                    PSOutput = PowerShellInstance2.Invoke();
                    LOG.InfoFormat("Result of add SSL Cert Binding is {0}", PSOutput[0]);
                }
            }
        }
    }
}
