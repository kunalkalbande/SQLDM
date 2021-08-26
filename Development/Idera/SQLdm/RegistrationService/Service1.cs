using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceProcess;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.RegistrationService.Configuration;
using Idera.SQLdm.RegistrationService.Helpers;
using System.IO;
using System.Reflection;
using Idera.SQLdm.Common.CWFDataContracts;
using System.ServiceModel.Description;

namespace Idera.SQLdm.RegistrationService
{
    public partial class Service1 : ServiceBase
    {
        private WebServiceHost _busHost;

        private static readonly Logger LogX = Logger.GetLogger("ServiceLogger");
        public static string SQldmInstanceName = "FirstInstance";
        private static object syncData = null;
        public Service1()
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
        protected void LoadProperties()
        {
            try
            {
                //                Properties.Settings.Default.Reload();
                //                Constants.CoreServicesUrl = Properties.Settings.Default.CoreServicesUrl;
                //                Constants.DashboardHost = Constants.CoreServicesUrl.Split('/')[2].Split(':')[0];
                //                Constants.DashboardPort = Constants.CoreServicesUrl.Split('/')[2].Split(':')[1];
                //                Constants.ServicePort = Properties.Settings.Default.ServicePort;
                //                Constants.ServiceAdminUser = Properties.Settings.Default.ServiceAdminAccount;
                //#if DEBUG    
                //            Constants.ServiceAdminPassword = Properties.Settings.Default.ServiceAdminPassword;
                //#else
                //                Constants.ServiceAdminPassword = EncryptionHelper.QuickDecrypt(Properties.Settings.Default.ServiceAdminPassword);
                //#endif
                //                Constants.IsRegistered = Properties.Settings.Default.IsRegistered;
                //                Constants.IsTaggable = Convert.ToInt32(Properties.Settings.Default.IsTaggingEnabled);
                //                Constants.ProductId = Properties.Settings.Default.ProductId;
                //                Constants.Instance = String.IsNullOrWhiteSpace(Properties.Settings.Default.Instance) ? "firstinstance" : Properties.Settings.Default.Instance;
                //                Constants.SQLDMSQLServerName = Properties.Settings.Default.SQLDMSQLServerName;
                //                Constants.JobManagerSQLServerName = Properties.Settings.Default.JobManagerSQLServerName;
                //                Constants.SQLDMRepository = Properties.Settings.Default.SQLDMRepository;
                //                Constants.JobManagerRepository = Properties.Settings.Default.JobManagerRepository;



                //RegistryHelper.SetValueInRegistry("IsRegisteredToDashboard", false);
                //RegistryHelper.SetValueInRegistry("ServiceHost", Environment.MachineName);
                //RegistryHelper.SetValueInRegistry("ServicePort", Program.SERVICE_PORT_DEFAULT);
                //RegistryHelper.SetValueInRegistry("Administrator", "Constants.ServiceAdminUser");// need to replace with actual admin for service
                //RegistryHelper.SetValueInRegistry("DisplayName", SQldmInstanceName);
            }
            catch (Exception e)
            {
                LogX.ErrorFormat("Error While Loading Properties: {0}", e.Message);
                
            }
            
        }

        //private void Initialize()
        //{
        //    using (LogX.InfoCall("Initialize"))
        //    {
        //        bool isInstallInfoProcessingSuccessfull;
        //        //[START]- SQLdm 9.0 (Ankit Srivastava) - CWF Integration - changed the registration behaviour a bit
        //        InstallInfo installInfo = new InstallInfo();
        //        string path = string.Empty;
        //        path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Program.INSTALL_INFO_FILE);
        //        bool installInfoFileExists = File.Exists(path);
        //        try
        //        {
        //            //Start : SQL DM 9.0 (Vineet Kumar) (License Changes + CWF) -- Reading installinfo file, which contains CWF info and new License key info dumped by installer

        //            if (installInfoFileExists)
        //            //[END]- SQLdm 9.0 (Ankit Srivastava) - CWF Integration - changed the registration behaviour a bit
        //            {
        //                LogX.Info("installinfo file exists. Read its information and process accordingly");

        //                LogX.Info("installinfo file content is valid.");
        //                PopulateInstallInfo(ref installInfo, path);
        //                //LicenseHelper.CheckLicense(true, installInfo);

        //            }
        //            else
        //            // the lic check can fail if the CS can't be contacted
        //            {
        //                LogX.Info("installinfo.txt file does not exists. Information will not be loaded with the install info");
        //                //LicenseHelper.CheckLicense(true);
        //            }
        //            //End : SQL DM 9.0 (Vineet Kumar) (License Changes + CWF) -- Reading installinfo file, which contains CWF info and new License key info dumped by installer
        //        }
        //        catch (Exception)
        //        {
        //            /* the lic check can fail if the CS can't be contacted */
        //            /* the lic check should have already logged the exception */
        //        }


        //        try
        //        {
        //            CommonWebFramework cwfDetails = CommonWebFramework.GetInstance();
        //            CWFHelper cwfHelper = null;

        //            //- SQLdm 9.0 (Ankit Srivastava) - CWF Integration -  added installinfo object and hostname property validation
        //            if (cwfDetails != null && cwfDetails.IsSaved == false && installInfo != null && !string.IsNullOrEmpty(installInfo.CWFHostName))  //SQLdm is not registed with CWF
        //            {
        //                LogX.Info("did not find the registration detail in the repo. populating new value");
        //                //loading various properties in the cwf object. no persistence yet
        //                cwfDetails.LoadCriticalInfo(installInfo.CWFHostName, installInfo.CWFPort, installInfo.CWFServiceUserName, installInfo.CWFServicePassword, installInfo.InstanceName);
        //                LogX.Info("cwf details before product registration processing:" + cwfDetails.ToString());
        //                cwfHelper = new CWFHelper(cwfDetails);
        //                cwfHelper.RegisterProduct();
        //                cwfHelper.LoadProductDetails(cwfDetails.ProductID);
        //                LogX.Info("product registered successfully");

        //            }
        //            else if (cwfDetails != null && cwfDetails.IsSaved == true && installInfo != null && !string.IsNullOrEmpty(installInfo.CWFHostName))
        //            {
        //                LogX.Info("cwf info is saved in the database");
        //                cwfHelper = new CWFHelper(cwfDetails);
        //                if (cwfHelper.UnRegisterProduct(cwfDetails.ProductID) == true)
        //                {
        //                    LogX.Info("unregistration went through sucessfully for the product id" + cwfDetails.ProductID.ToString());
        //                    LogX.Info("cwf details before product registration processing:" + cwfDetails.ToString());
        //                    cwfDetails.LoadCriticalInfo(installInfo.CWFHostName, installInfo.CWFPort, installInfo.CWFServiceUserName, installInfo.CWFServicePassword, installInfo.InstanceName);
        //                    cwfHelper.RefreshCWFDetails(cwfDetails);
        //                    cwfHelper.RegisterProduct();
        //                    cwfHelper.LoadProductDetails(cwfDetails.ProductID);
        //                    LogX.Info("product registered successfully");
        //                }

        //            }
        //            else if (installInfo != null && string.IsNullOrEmpty(installInfo.CWFHostName))
        //            {
        //                LogX.Info("no installinfo file found. getting product details from CWF");
        //                cwfHelper = new CWFHelper(cwfDetails);
        //                if (cwfDetails.ProductID > 0) cwfHelper.LoadProductDetails(cwfDetails.ProductID);
        //            }

        //            //[START] SQLdm 10.0 (Rajesh Gupta) : LM 2.0 Integration-Register License Manger,Write Registry Keys
        //            //LMHelper.RegisterLicenseManager();
        //            //[END] SQLdm 10.0 (Rajesh Gupta) : LM 2.0 Integration-Register License Manger,Write Registry Keys

        //            LogX.Info("cwf details after product registration processing:" + cwfDetails.ToString());
        //            isInstallInfoProcessingSuccessfull = true;//modify it while implementing CWF changes
        //            //[START] SQLdm 9.0 (Ankit Srivastava) - CWF Integration -starting synchronization on a different thread

        //            SyncWithCWF(Guid.Empty);

        //            //syncThread.Start();
        //        }	//[END] SQLdm 9.0 (Ankit Srivastava) - CWF Integration -starting synchronization on a different thread
        //        catch (Exception ex)
        //        {
        //            isInstallInfoProcessingSuccessfull = false;
        //            LogX.Info("product registration failed." + ex.Message + "::" + (ex.InnerException != null ? ex.InnerException.Message : string.Empty));
        //        }
        //        //[END]  SQLdm 9.0 (Gaurav Karwal) : Added code to register SQLdm with CWF

        //    }
        //}

        //[START] SQLdm 9.0 (Ankit Srivastava) - CWF Integration -Method for CWF synchronization when the mngmt svc starts
        //private static void SyncWithCWF(Guid collectionServiceId)
        //{
        //    try
        //    {
        //        CommonWebFramework cwfDetails = CommonWebFramework.GetInstance();
        //        var cwfHelper = new CWFHelper(cwfDetails);

        //        //synchronizing alerts with the CWF
        //        //[START] SQLdm 10.0 (Gaurav Karwal): commenting out due to memory leak
        //        //SyncByHelper syncAlertsDelegate=new SyncByHelper (SyncAlerts);
        //        //IAsyncResult alertResult= syncAlertsDelegate.BeginInvoke(cwfHelper, new AsyncCallback(TaskCompleted), syncData);
        //        //syncAlertsDelegate.EndInvoke(alertResult);
        //        //[END] SQLdm 10.0 (Gaurav Karwal): commenting out due to memory leak

        //        //synchronizing instances with the CWF
        //        SyncByHelperAndCollSvcId syncInstanceDelegate = new SyncByHelperAndCollSvcId(SyncInstances);
        //        IAsyncResult instanceResult = syncInstanceDelegate.BeginInvoke(cwfHelper, collectionServiceId, new AsyncCallback(TaskCompleted), null);
        //        syncInstanceDelegate.EndInvoke(instanceResult);

        //        //synchronizing users with the CWF
        //        SyncByHelper syncUserDelegate = new SyncByHelper(SyncUsers);
        //        IAsyncResult userResult = syncUserDelegate.BeginInvoke(cwfHelper, new AsyncCallback(TaskCompleted), null);
        //        syncUserDelegate.EndInvoke(userResult);

        //    }
        //    catch (Exception ex)
        //    {

        //        LogX.ErrorFormat("Error occured while CWF Sychronization", ex);
        //    }
        //}
        ////[END] SQLdm 9.0 (Ankit Srivastava) - CWF Integration -Method for CWF synchronization when the mngmt svc starts

        //private delegate void SyncByHelper(CWFHelper cwfHelper);
        //private delegate void SyncByHelperAndCollSvcId(CWFHelper cwfHelper, Guid collectionServiceId);

        //public static void TaskCompleted(IAsyncResult result)
        //{
        //    try
        //    {
        //        if (result == null) return;
        //        string resultMessage = result.AsyncState as string;
        //        if (resultMessage.Contains("successfully"))
        //            LogX.Info(resultMessage);
        //        else
        //            LogX.Error(resultMessage);
        //    }
        //    catch (Exception ex)
        //    {

        //        LogX.ErrorFormat("Error occured in TaskCompleted", ex);
        //    }
        //}

        /// <summary>
        /// Populating the Install Info
        /// </summary>
        /// <param name="installInfo"></param>
        /// <param name="path"></param>
        /// 
        //private static void PopulateInstallInfo(ref InstallInfo installInfo, string path)
        //{

        //    if (File.Exists(path) && installInfo != null)
        //    {
        //        LogX.Info("installinfo file exists. Read its information and process accordingly");

        //        LogX.Info("installinfo file content is valid.");
        //        //installInfo = new InstallInfo();
        //        installInfo.CWFHostName = INIFile.ReadValue(Program.CWF_SECTION_NAME, "Host", path);
        //        installInfo.CWFPort = INIFile.ReadValue(Program.CWF_SECTION_NAME, "Port", path);
        //        installInfo.CWFServiceUserName = INIFile.ReadValue(Program.CWF_SECTION_NAME, "ServiceUserName", path);
        //        installInfo.CWFServicePassword = INIFile.ReadValue(Program.CWF_SECTION_NAME, "ServicePassword", path);
        //        installInfo.InstanceName = INIFile.ReadValue(Program.CWF_SECTION_NAME, "InstanceName", path);
        //        SQldmInstanceName = installInfo.InstanceName;
        //        //installInfo.LicenseKey = INIFile.ReadValue(LICENSE_SECTION_NAME, "LicenseKey", path);
        //        //LicenseHelper.CheckLicense(true, installInfo);
        //    }
        //    else
        //    // the lic check can fail if the CS can't be contacted
        //    {
        //        LogX.Info("installinfo.txt file does not exists. So continue without modifying any key");

        //    }
        //}

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("In OnStart");
            LogX.Info("In Start Method");

            RegistrationServiceConfiguration.LogConfiguration();
            string instanceName = RegistrationServiceConfiguration.InstanceName;

            LoadProperties();

            StartWebServices("http://localhost:" + Program.SERVICE_PORT_DEFAULT + "/SQLdm/");
        }

        protected override void OnStop()
        {
            if(_busHost.State != CommunicationState.Closed) _busHost.Close();
            eventLog1.WriteEntry("In OnStop");
            LogX.Info("In Stop Method");
        }

        protected override void OnShutdown()
        {
            eventLog1.WriteEntry("In OnShutdown");
            LogX.Info("In Shutdown Method Method");
        }

        internal void RunFromConsole()
        {

            Console.WriteLine("\nService is running.  Press Ctrl-C to stop service.");
            OnStart(null);

            Console.ReadKey();

            OnStop();

        }

        void StartWebServices(string urlAuthority)
        {
            _busHost = new WebServiceHost(typeof(SQLdm), new Uri(urlAuthority + ""));
            var busBinding = new WebHttpBinding(WebHttpSecurityMode.TransportCredentialOnly);
            var busEndpoint = _busHost.AddServiceEndpoint(typeof(IProduct), busBinding, urlAuthority + "");
            //busEndpoint.Behaviors.Add(new ErrorHandlerBehavior());
            busEndpoint.Behaviors.Add(new WebHttpBehavior { HelpEnabled = true });
            _busHost.Open();
            //LogX.InfoFormat("Core services URL: {0}", Constants.CoreServicesUrl);
            /*if (!Constants.IsRegistered)
            {
                SampleProduct _sample = new SampleProduct();
                _sample.RegisterSampleProduct();
            }*/
        }
    }
}
