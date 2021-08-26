//------------------------------------------------------------------------------
// <copyright file="ManagementServiceConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService
{
    using System;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;
    using Common;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Messages;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.ManagementService.Configuration;
    using Idera.SQLdm.ManagementService.Helpers;
    using Idera.SQLdm.ManagementService.Monitoring;
    using Idera.SQLdm.ManagementService.Notification;
    using Microsoft.ApplicationBlocks.Data;
    using Idera.SQLdm.Common.Services;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Security;
    using Idera.SQLdm.Common.CWFDataContracts;
    using Idera.SQLdm.Common.Objects.ApplicationSecurity;

    /// <summary>
    /// Top-level static class for the management service.  It's main purpose is to hold
    /// instances of managers for the various management service subsystems.
    /// </summary>
    public static class Management
    {
        #region fields

        private static readonly BBS.TracerX.Logger LOG;

        private static ManagementServiceInfo managementService;
        private static CollectionServiceManager collectionServices;
        private static ScheduledCollectionManager scheduledCollection;
        private static NotificationManager notification;
        private static DelegateWorkQueue workQueue;
        private static EventLog eventLog;
        private static MessageDll messageDll;
        private static Dictionary<Guid, ISnapshotSink> snapshotSinks;
        private static bool allowAutoRegistration;
        private static bool clusterNameChangeChecked;
        private static object sync = new object();
        private static MetricDefinitions metricDefinitions = null;
        private static QueryMonitorUpgradeHelper queryMonitorUpgradeHelper;
        private static AlertsUpgradeHelper alertsUpgradeHelper;
        private static BaselineTemplatesUpgradeHelper baselineUpgradeHelper;
        public static DatabaseStatisticsUpgradeHelper DatabaseStatisticsUpgradeHelper { get; set; }

        private static DateTime writeNextEvent = DateTime.MinValue;
        //Start : SQL DM 9.0 (Vineet Kumar) (License Changes) -- Adding constants
        private static string INSTALL_INFO_FILE = "InstallInfo.ini";
        private static string CWF_SECTION_NAME = "CWFInformation";
        private static string LICENSE_SECTION_NAME = "LicenseInformation";
        private static object syncData = null;
        //End : SQL DM 9.0 (Vineet Kumar) (License Changes) -- Adding constants
        //[START] SQLdm 10.1 (GK): declaration of the required variables
        const double ONE_MINUTE = (1000 * 60);
        const double ONE_HOUR = ONE_MINUTE * 60;
        const double CWF_SYNC_FREQUENCY_IN_MILLISECOND = ONE_HOUR;
        static System.Timers.Timer _cwfSyncTimer = null;
        static bool isCWFSyncInProgress = false;
        //[END] SQLdm 10.1 (GK): declaration of the required variables
        #endregion

        #region constructors

        /// <summary>
        /// Initializes the <see cref="T:Management"/> class.
        /// </summary>
        static Management()
        {
            LOG = BBS.TracerX.Logger.GetLogger("Management");
            using (LOG.InfoCall("Management"))
            {
                // must get the event log object configured prior to creating the notification manager
                eventLog = ManagementServiceInstaller.RegisterEventSource(ManagementServiceConfiguration.InstanceName);
                workQueue = new DelegateWorkQueue(new MultipleWorkerStrategy(10, 50));
                snapshotSinks = new Dictionary<Guid, ISnapshotSink>();

                // force the prevalence engine to initialize prior to loading the heartbeat monitor
                PersistenceManager pm = PersistenceManager.Instance;
                // make sure the persistence object upgrades its queues to last-in-first-out
                pm.UpgradeScheduledDataSendQueue();
                // allow the management service to be automatically registered in console mode
                allowAutoRegistration = Environment.UserInteractive;
                clusterNameChangeChecked = allowAutoRegistration;

                try
                {
                    CollectionServices = new CollectionServiceManager();
                    ScheduledCollection = new ScheduledCollectionManager();
                    QueryMonitorUpgradeHelper = new QueryMonitorUpgradeHelper();
                    AlertsUpgradeHelper = new AlertsUpgradeHelper();
                    BaselineUpgradeHelper = new BaselineTemplatesUpgradeHelper();
                    Notification = new NotificationManager();
                    DatabaseStatisticsUpgradeHelper = new DatabaseStatisticsUpgradeHelper();
                }
                catch (Exception e)
                {
                    LOG.Fatal("Error initializing the Management object.", e);
                }
            }
        }

        #endregion


        public static ManagementServiceInfo ManagementService
        {
            get
            {
                lock (sync)
                {
                    if (managementService == null)
                    {
                        string instanceName = ManagementServiceConfiguration.InstanceName;
                        managementService = GetManagementService(instanceName, Environment.MachineName);
                        if (managementService == null)
                            managementService = AddManagementService(instanceName, Environment.MachineName);
                        else
                        {
                            AddDefaultMetricThresholds();
                            allowAutoRegistration = false;
                        }

                        // check for repository/management service change
                        if (managementService != null)
                        {
                            Guid? mid = PersistenceManager.Instance.GetManagementServiceID();
                            if (mid == null || mid != managementService.Id)
                            {
                                PersistenceManager.Instance.ReinitializePersistence();
                                PersistenceManager.Instance.SetManagementServiceID(managementService.Id);
                            }
                        }
                    }
                }
                return managementService;
            }
        }

        private static void AddDefaultMetricThresholds()
        {
            RepositoryHelper.AddDefaultMetricThresholdEntries(ManagementServiceConfiguration.ConnectionString, false);
        }

        public static Dictionary<Guid, ISnapshotSink> SnapshotSinks
        {
            get { return snapshotSinks; }
        }

        public static bool AllowAutoRegistration
        {
            get
            {
                lock (sync)
                {
                    return allowAutoRegistration;
                }
            }
            set
            {
                lock (sync)
                {
                    allowAutoRegistration = value;
                }
            }
        }

        /// <summary>
        /// Populating the Install Info
        /// </summary>
        /// <param name="installInfo"></param>
        /// <param name="path"></param>
        private static void PopulateInstallInfo(ref InstallInfo installInfo, string path) 
        {
            
            if (File.Exists(path) && installInfo!=null)
            {
                LOG.Info("installinfo file exists. Read its information and process accordingly");

                LOG.Info("installinfo file content is valid.");
                //installInfo = new InstallInfo();
                installInfo.CWFHostName = INIFile.ReadValue(CWF_SECTION_NAME, "Host", path);
                installInfo.CWFPort = INIFile.ReadValue(CWF_SECTION_NAME, "Port", path);
                installInfo.CWFServiceUserName = INIFile.ReadValue(CWF_SECTION_NAME, "ServiceUserName", path);
                installInfo.CWFServicePassword = INIFile.ReadValue(CWF_SECTION_NAME, "ServicePassword", path);
                installInfo.InstanceName = INIFile.ReadValue(CWF_SECTION_NAME, "InstanceName", path);
                installInfo.LicenseKey = INIFile.ReadValue(LICENSE_SECTION_NAME, "LicenseKey", path);
                LicenseHelper.CheckLicense(true, installInfo);
                
            }
            else
            // the lic check can fail if the CS can't be contacted
            {
                LOG.Info("installinfo.txt file does not exists. So continue without modifying any key");
              
            }
        }

        public static void Initialize()
        {
            using (LOG.InfoCall("Initialize"))
            {
                managementService = null;
                metricDefinitions = null;
                CollectionServices.Initialize();
                ScheduledCollection.Start();
                bool isInstallInfoProcessingSuccessfull;
                //[START]- SQLdm 9.0 (Ankit Srivastava) - CWF Integration - changed the registration behaviour a bit
                InstallInfo installInfo = new InstallInfo();
                string path = string.Empty;
                path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), INSTALL_INFO_FILE);
                bool installInfoFileExists = File.Exists(path);
                try
                {
                    //Start : SQL DM 9.0 (Vineet Kumar) (License Changes + CWF) -- Reading installinfo file, which contains CWF info and new License key info dumped by installer

                    if (installInfoFileExists)
                    //[END]- SQLdm 9.0 (Ankit Srivastava) - CWF Integration - changed the registration behaviour a bit
                    {
                        LOG.Info("installinfo file exists. Read its information and process accordingly");

                        LOG.Info("installinfo file content is valid.");
                        PopulateInstallInfo(ref installInfo, path);
                        LicenseHelper.CheckLicense(true, installInfo);

                    }
                    else
                    // the lic check can fail if the CS can't be contacted
                    {
                        LOG.Info("installinfo.txt file does not exists. Information will not be loaded with the install info");
                        LicenseHelper.CheckLicense(true);
                    }
                    //End : SQL DM 9.0 (Vineet Kumar) (License Changes + CWF) -- Reading installinfo file, which contains CWF info and new License key info dumped by installer
                }
                catch (Exception)
                {
                    /* the lic check can fail if the CS can't be contacted */
                    /* the lic check should have already logged the exception */
                }
                //[START] SQLdm 9.0 (Gaurav Karwal) : Added code to register SQLdm with CWF
                try
                {
                    CommonWebFramework cwfDetails = CommonWebFramework.GetInstance();
                    CWFHelper cwfHelper = null;
                    //- SQLdm 9.0 (Ankit Srivastava) - CWF Integration -  added installinfo object and hostname property validation
                    if (cwfDetails != null && cwfDetails.IsSaved == false && installInfo != null && !string.IsNullOrEmpty(installInfo.CWFHostName))  //SQLdm is not registed with CWF
                    {
                        LOG.Info("did not find the registration detail in the repo. populating new value");
                        //loading various properties in the cwf object. no persistence yet
                        cwfDetails.LoadCriticalInfo(installInfo.CWFHostName, installInfo.CWFPort, installInfo.CWFServiceUserName, installInfo.CWFServicePassword, installInfo.InstanceName);
                        LOG.Info("cwf details before product registration processing:" + cwfDetails.ToString());
                        cwfHelper = new CWFHelper(cwfDetails);
                        cwfHelper.RegisterProduct();
                        cwfHelper.LoadProductDetails(cwfDetails.ProductID);
                        LOG.Info("product registered successfully");
                    }
                    else if (cwfDetails != null && cwfDetails.IsSaved == true && installInfo != null && !string.IsNullOrEmpty(installInfo.CWFHostName))
                    {
                        LOG.Info("cwf info is saved in the database");
                        cwfHelper = new CWFHelper(cwfDetails);
                        if (cwfHelper.UnRegisterProduct(cwfDetails.ProductID) == true)
                        {
                            LOG.Info("unregistration went through sucessfully for the product id" + cwfDetails.ProductID.ToString());
                            LOG.Info("cwf details before product registration processing:" + cwfDetails.ToString());
                            cwfDetails.LoadCriticalInfo(installInfo.CWFHostName, installInfo.CWFPort, installInfo.CWFServiceUserName, installInfo.CWFServicePassword, installInfo.InstanceName);
                            cwfHelper.RefreshCWFDetails(cwfDetails);
                            cwfHelper.RegisterProduct();
                            cwfHelper.LoadProductDetails(cwfDetails.ProductID);
                            LOG.Info("product registered successfully");
                        }

                    }
                    else if (installInfo != null && string.IsNullOrEmpty(installInfo.CWFHostName))
                    {
                        LOG.Info("no installinfo file found. getting product details from CWF");
                        cwfHelper = new CWFHelper(cwfDetails);
                        if (cwfDetails.ProductID > 0) cwfHelper.LoadProductDetails(cwfDetails.ProductID);
                    }

                    //[START] SQLdm 10.0 (Rajesh Gupta) : LM 2.0 Integration-Register License Manger,Write Registry Keys
                    LMHelper.RegisterLicenseManager();
                    //[END] SQLdm 10.0 (Rajesh Gupta) : LM 2.0 Integration-Register License Manger,Write Registry Keys

                    LOG.Info("cwf details after product registration processing:" + cwfDetails.ToString());
                    isInstallInfoProcessingSuccessfull = true;//modify it while implementing CWF changes
                    //[START] SQLdm 9.0 (Ankit Srivastava) - CWF Integration -starting synchronization on a different thread


                    //SyncWithCWF(CollectionServices.DefaultCollectionServiceID); SQLdm 10.1(GK) - commenting out as this is being done by timer now.

                    //syncThread.Start();
                }	//[END] SQLdm 9.0 (Ankit Srivastava) - CWF Integration -starting synchronization on a different thread
                catch (Exception ex)
                {
                    isInstallInfoProcessingSuccessfull = false;
                    LOG.Info("product registration failed." + ex.Message + "::" + (ex.InnerException != null ? ex.InnerException.Message : string.Empty));
                }
                //[END]  SQLdm 9.0 (Gaurav Karwal) : Added code to register SQLdm with CWF


                if (!string.IsNullOrEmpty(path) && File.Exists(path) && isInstallInfoProcessingSuccessfull == true)//delete file only if processing of file is successfull
                {
                    LOG.Info("Storing install file information in repository successfull. Hence deleting this file");
                    File.Delete(path);
                }

                if (isInstallInfoProcessingSuccessfull)
                {
                    //Do an intial sync at startup. Then start the timer.
                    SyncCWFData();

                    _cwfSyncTimer = new System.Timers.Timer();
                    _cwfSyncTimer.Interval = CWF_SYNC_FREQUENCY_IN_MILLISECOND;
                    _cwfSyncTimer.Elapsed += _cwfSyncTimer_Elapsed;
                    _cwfSyncTimer.Start();
                }
            }
        }

        static double GetSyncTimerValue()
        {
            try
            {
                object value = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Idera\\SQLdm\\", "CWFSyncTimer", ONE_HOUR);

                if (value != null)
                    return (double)value;

                return ONE_HOUR;
            } 
            catch (SecurityException e)
            {
                LOG.Info("The service account is not authorized to read the data path from the registry: ", e);
            } 
            catch (Exception e)
            {
                LOG.Warn("Unable read the data path from the registry: ", e);
            }
            return ONE_HOUR;
        }

        /// <summary>
        /// gets called on cwf sync timer elapsed. syncs various obects with cwf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void _cwfSyncTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SyncCWFData();
        }

        static void SyncCWFData()
        {
            CommonWebFramework webFramework = CommonWebFramework.GetInstance();

            if (!string.IsNullOrWhiteSpace(webFramework.BaseURL) && !string.IsNullOrWhiteSpace(webFramework.RestURI) && webFramework.ProductID > 0 && isCWFSyncInProgress == false)
            {
                if (Monitor.TryEnter(isCWFSyncInProgress, 1000))
                {
                    isCWFSyncInProgress = true;
                    CWFHelper cwfHelper = new CWFHelper(webFramework);
                    SyncAlerts(cwfHelper);
                    SyncUsers(cwfHelper);
                    SyncInstances(cwfHelper, CollectionServices.DefaultCollectionServiceID);
                    isCWFSyncInProgress = false;
                }

            }
        }
        
        //[START] SQLdm 9.0 (Ankit Srivastava) - CWF Integration -Method for CWF synchronization when the mngmt svc starts
        //SQLdm 10.1 (GK): this function is not being used anymore
        private static void SyncWithCWF(Guid collectionServiceId)
        {
            try
            {
            CommonWebFramework cwfDetails = CommonWebFramework.GetInstance();
            var cwfHelper = new CWFHelper(cwfDetails);

                //synchronizing alerts with the CWF
                //[START] SQLdm 10.0 (Gaurav Karwal): commenting out due to memory leak
                //SyncByHelper syncAlertsDelegate=new SyncByHelper (SyncAlerts);
                //IAsyncResult alertResult= syncAlertsDelegate.BeginInvoke(cwfHelper, new AsyncCallback(TaskCompleted), syncData);
                //syncAlertsDelegate.EndInvoke(alertResult);
                //[END] SQLdm 10.0 (Gaurav Karwal): commenting out due to memory leak

                //synchronizing users with the CWF
                SyncByHelper syncUserDelegate = new SyncByHelper(SyncUsers);
                IAsyncResult userResult = syncUserDelegate.BeginInvoke(cwfHelper, new AsyncCallback(TaskCompleted), null);
                syncUserDelegate.EndInvoke(userResult);


                //synchronizing instances with the CWF
                SyncByHelperAndCollSvcId syncInstanceDelegate = new SyncByHelperAndCollSvcId(SyncInstances);
                IAsyncResult instanceResult = syncInstanceDelegate.BeginInvoke(cwfHelper,collectionServiceId, new AsyncCallback(TaskCompleted), null);
                syncInstanceDelegate.EndInvoke(instanceResult);

               

            }
            catch (Exception ex)
            {

                LOG.ErrorFormat("Error occured while CWF Sychronization", ex);
            }
        }
		//[END] SQLdm 9.0 (Ankit Srivastava) - CWF Integration -Method for CWF synchronization when the mngmt svc starts

        //[START] SQLdm 9.0 (Ankit Srivastava) - CWF Integration -Delegates and new method for async CWF synchronization 
        private delegate void SyncByHelper(CWFHelper cwfHelper);
        private delegate void SyncByHelperAndCollSvcId(CWFHelper cwfHelper, Guid collectionServiceId);

        public static void TaskCompleted(IAsyncResult result)
        {
            try
            {
                if (result == null) return;
                string resultMessage = result.AsyncState as string;
                if (resultMessage.Contains("successfully"))
                    LOG.Info(resultMessage);
                else
                    LOG.Error(resultMessage);
            }
            catch (Exception ex)
            {

                LOG.ErrorFormat("Error occured in TaskCompleted", ex);
            }
        }
        //[END] SQLdm 9.0 (Ankit Srivastava) - CWF Integration -Delegates and new method for async CWF synchronization 

        //[START] SQLdm 10.0 (Gaurav Karwal): for syncing
        public static void AddAlertsInCWF(List<Idera.SQLdm.Common.CWFDataContracts.Alert> listOfAlertsToSync) 
        {
            try
            {
                CWFHelper helper = new CWFHelper(CommonWebFramework.GetInstance());
                helper.AddAlerts(listOfAlertsToSync);
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occured while syncing the alert list with CWF", ex);
            }
            //[END] SQLdm 10.0 (Gaurav Karwal): commenting out due to memory leak
        }
        //[END] SQLdm 10.0 (Gaurav Karwal): commenting out due to memory leak

		//[START] SQLdm 9.0 (Ankit Srivastava) - CWF Integration -Method for Alert CWF synchronization when the mngmt svc starts
        private static void SyncAlerts(CWFHelper cwfHelper)
        {
            syncData = null;
            try
            {
            var alerts=RepositoryHelper.GetActiveAlertsForCWF(ManagementServiceConfiguration.ConnectionString);
            cwfHelper.AddAlerts(alerts);
                syncData = "SyncAlerts successfully completed.";
            }
            catch (Exception ex)
            {
                syncData = "SyncAlerts failed due to " + ex.Message;
                LOG.Error(syncData);
            }
        }
		//[END] SQLdm 9.0 (Ankit Srivastava) - CWF Integration -Method for Alert CWF synchronization when the mngmt svc starts
		
		//[START] SQLdm 9.0 (Ankit Srivastava) - CWF Integration -Method for Instances CWF synchronization when the mngmt svc starts
        private static void SyncInstances(CWFHelper cwfHelper, Guid collectionServiceId)
        {
            syncData = null;
            try
            {
            var monitoredSqlServers=RepositoryHelper.GetMonitoredSqlServers(ManagementServiceConfiguration.ConnectionString,(Guid?)collectionServiceId,true);
            IList<Instance> instances = new List<Instance>();
            foreach (var sqlServer in monitoredSqlServers)
            {
                var instance = new Instance();
                instance.Name = sqlServer.InstanceName;
                instance.UtcFirstSeen = sqlServer.RegisteredDate;
                instance.UtcLastSeen = String.Empty;
                instance.Edition = sqlServer.MostRecentSQLEdition ?? String.Empty;
                instance.Version = sqlServer.MostRecentSQLVersion != null ? sqlServer.MostRecentSQLVersion.Version : String.Empty;
                instance.InstanceStatus = InstanceStatus.Managed; //SQLdm 10.1 (Gaurav Karwal): all instances are being managed
                instance.Owner = String.Empty;
                instance.Location = String.Empty;
                instance.Comments = String.Empty;
                instances.Add(instance);
            }
            cwfHelper.AddInstances(instances);
                syncData = "SyncInstances successfully completed.";
            }
            catch (Exception ex)
            {
                syncData = "SyncInstances failed due to " + ex.Message;
            }
        }
		//[END] SQLdm 9.0 (Ankit Srivastava) - CWF Integration -Method for Instances CWF synchronization when the mngmt svc starts

		//[START] SQLdm 9.0 (Ankit Srivastava) - CWF Integration -Method for users CWF synchronization when the mngmt svc starts
        private static void SyncUsers(CWFHelper cwfHelper)
        {
            syncData = null;
            try
            {
            var users = RepositoryHelper.GetSQLServerUsers(ManagementServiceConfiguration.ConnectionString, false);
            cwfHelper.AddUsers(users);
                syncData = "SyncUsers successfully completed.";
            }
            catch (Exception ex)
            {
                syncData = "SyncUsers failed due to " + ex.Message;
            }
           
        }
		//[END] SQLdm 9.0 (Ankit Srivastava) - CWF Integration -Method for users CWF synchronization when the mngmt svc starts

        /// <summary>
        /// Get the management service record from the database - if it is not available then create it from
        /// the config file.  If there isn't enough info to create it return null.
        /// </summary>
        /// <param name="instanceName"></param>
        /// <param name="machineName"></param>
        /// <returns></returns>
        private static ManagementServiceInfo GetManagementService(string instanceName, string machineName)
        {
            ManagementServiceInfo info = null;

            string iname = instanceName.ToLower();
            string mname = machineName.ToLower();

            string connectionString = ManagementServiceConfiguration.ConnectionString;

            using (SqlDataReader reader = SqlHelper.ExecuteReader(
                connectionString,
                "p_GetManagementServices",
                DBNull.Value
                ))
            {
                while (reader.Read())
                {
                    string name = reader.GetString(1);
                    if (!iname.Equals(name.ToLower()))
                        continue;

                    string machine = reader.GetString(2);
                    if (!mname.Equals(machine.ToLower()))
                        continue;

                    info = new ManagementServiceInfo();
                    info.Id = reader.GetGuid(0);
                    info.InstanceName = name;
                    info.MachineName = machine;
                    info.Address = reader.GetString(3);
                    info.Port = reader.GetInt32(4);

                    SqlGuid guid = reader.GetSqlGuid(5);
                    if (guid.IsNull)
                        info.DefaultCollectionServiceID = null;
                    else
                        info.DefaultCollectionServiceID = guid.Value;

                    break;
                }
            }

            return info;
        }


        private static ManagementServiceInfo AddManagementService(string machineName, string instanceName)
        {
            ManagementServiceInfo msi = null;

            // see if the non-clustered name is registered 
            if (!clusterNameChangeChecked)
                DetectNameChange(machineName, instanceName);

            if (allowAutoRegistration)
            {
                string connectionString = ManagementServiceConfiguration.ConnectionString;

                LOG.DebugFormat("Management service record not found in repository for {0}\\{1}", machineName,
                                instanceName);
                ManagementServiceElement element = ManagementServiceConfiguration.GetManagementServiceElement();
                if (element == null)
                {
                    LOG.DebugFormat("Management service entry not found in config file for {0}\\{1}", machineName,
                                    instanceName);
                    return null;
                }
                // make sure repository information is set
                if (!element.IsValid)
                {
                    LOG.DebugFormat("Management service entry is not valid for {0}\\{1}", machineName, instanceName);
                }
                if (RepositoryHelper.TestRepositoryConnection(connectionString))
                {
                    msi = RepositoryHelper.AddManagementService(
                            ManagementServiceConfiguration.ConnectionString,
                            element.InstanceName,
                            null,
                            null,
                            element.ServicePort);
                }

                allowAutoRegistration = false;
            }

            return msi;
        }

        public static void CheckRegistration()
        {
            using (LOG.DebugCall("CheckRegistration"))
            {
                ManagementServiceInfo startup = ManagementService;
                ManagementServiceInfo current = null;
                try
                {
                    current = GetManagementService(ManagementServiceConfiguration.InstanceName, Environment.MachineName);
                }
                catch (Exception e)
                {
                    LOG.Error("Error getting management service info from the repository: ", e);
                    throw;
                }
                if (startup == null || current == null || !current.Id.Equals(startup.Id))
                {
                    string msid = startup == null ? "[no id set]" : startup.Id.ToString();
                    WriteNoLongerRegisteredMessage(msid);
                    throw new ServiceException(Status.ErrorInvalidManagementServiceId, msid);
                }
                else
                    writeNextEvent = DateTime.MinValue;
            }
        }

        #region ClusterNameChangeCheck
        enum COMPUTER_NAME_FORMAT
        {
            ComputerNameNetBIOS,
            ComputerNameDnsHostname,
            ComputerNameDnsDomain,
            ComputerNameDnsFullyQualified,
            ComputerNamePhysicalNetBIOS,
            ComputerNamePhysicalDnsHostname,
            ComputerNamePhysicalDnsDomain,
            ComputerNamePhysicalDnsFullyQualified
        }


        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool GetComputerNameEx(COMPUTER_NAME_FORMAT NameType, [Out] StringBuilder lpBuffer, ref int lpnSize);
        private static void DetectNameChange(string machineName, string instanceName)
        {
            clusterNameChangeChecked = true;
            if (allowAutoRegistration)
                return;

            // if 
            int size = 260;
            StringBuilder nonClusteredName = new StringBuilder(size);
            bool success = GetComputerNameEx(COMPUTER_NAME_FORMAT.ComputerNamePhysicalNetBIOS, nonClusteredName, ref size);
            if (success)
            {
                // see if we are switching from clustered to non or vice-versa
                if (!string.Equals(nonClusteredName.ToString(), Environment.MachineName, StringComparison.InvariantCultureIgnoreCase))
                {
                    ManagementServiceInfo ncmsi = GetManagementService(instanceName, nonClusteredName.ToString());
                    if (ncmsi != null)
                    {
                        AllowAutoRegistration = true;
                        LOG.Info("Management service registration found under non-clustered machine name - allowing registration of clustered machine name");
                    }
                    else
                        LOG.Info("Management service registration not found for non-clustered machine name ({0})", nonClusteredName);
                }
            }
            else
                LOG.Warn("Unable to retrieve physical netbios name");
        }
        #endregion

        private static StateDeviationEvent managementServiceOutstandingEvent = null;

        public static void ClearManagementServiceAlert()
        {
            if (managementServiceOutstandingEvent != null)
            {
                managementServiceOutstandingEvent = null;
            }
        }

        public static void ManagementServiceUnhappyNotification(string machineName, string instanceName, object value, object[] additionalData)
        {
            DateTime now = DateTime.UtcNow;

            List<IEvent> events = new List<IEvent>();

            if (managementServiceOutstandingEvent == null)
            {
                managementServiceOutstandingEvent = new StateDeviationEvent(
                    new MonitoredObjectName(String.Format("{0}\\{1}", machineName.Trim(), instanceName.Trim()), "", ""),
                    (int)Metric.SQLdmCollectionServiceStatus,
                    MonitoredState.Critical,
                    value, //SQLdmServiceState.Undetermined,
                    now,
                    additionalData); //machineName.Trim());
                events.Add(managementServiceOutstandingEvent);
            }
            else
            {
                if (managementServiceOutstandingEvent.OccuranceTime + TimeSpan.FromMinutes(6) > DateTime.UtcNow)
                    return;

                StateDeviationUpdateEvent sdue = new StateDeviationUpdateEvent(managementServiceOutstandingEvent, now);
                events.Add(sdue);
            }

            Notification.Process(events, null);
        }

        public static void WriteNoLongerRegisteredMessage(string serviceId)
        {
            if (DateTime.Now > writeNextEvent)
            {   // only log the event once every 15 minutes
                writeNextEvent = DateTime.Now + TimeSpan.FromMinutes(15);
                LOG.Error(
                    "The management service has determined that it is no longer registered with the repository and has send a stop collection request to its registered collection services.");
                Management.CollectionServices.PauseAllCollection(true);
                // write it to the event log...
                WriteEvent((int)EventLogEntryType.Error, Status.ErrorInvalidManagementServiceId, Category.General, serviceId);
            }
        }

        public static EventLog EventLog
        {
            get { return eventLog; }
            set { eventLog = value; }
        }

        public static void WriteEvent(Status messageId, Category categoryId, params string[] vars)
        {
            try
            {
                EventLog.WriteEvent(new EventInstance((long)messageId, (int)categoryId), vars);
            }
            catch (Exception e)
            {
                LogWriteEventError(e, messageId, vars);
            }
        }

        public static void WriteEvent(int eventType, Status messageId, Category categoryId, params string[] vars)
        {
            int eventId = (int)messageId;
            int category = (int)categoryId;
            try
            {
                MessageDll.WriteEvent(eventLog.Source, (uint)eventType, (uint)category, (uint)eventId, vars);
            }
            catch (Exception e)
            {
                LogWriteEventError(e, messageId, vars);
            }
        }

        public static void LogWriteEventError(Exception exception, Status messageId, params string[] vars)
        {
            LOG.ErrorFormat("Error writing to the event log: {0}", exception.Message);
            try
            {
                if (messageDll == null)
                    messageDll = new MessageDll();

                string message = messageDll.Format(messageId, vars);
                LOG.ErrorFormat(message);
            }
            catch (Exception e)
            {
                StringBuilder builder = new StringBuilder();
                if (vars != null)
                {
                    foreach (string var in vars)
                    {
                        if (builder.Length > 0)
                            builder.Append(",");
                        builder.AppendFormat("'{0}'", var);
                    }
                }
                LOG.ErrorFormat("Error formatting event log message: {0:F} - {1}", messageId, builder.ToString(), e);
            }
        }

        /// <summary>
        /// Gets or sets the collection services.
        /// </summary>
        /// <value>The collection services.</value>
        public static CollectionServiceManager CollectionServices
        {
            get { return collectionServices; }
            private set { collectionServices = value; }
        }

        /// <summary>
        /// Gets or sets the scheduled collection.
        /// </summary>
        /// <value>The scheduled collection.</value>
        public static ScheduledCollectionManager ScheduledCollection
        {
            get { return scheduledCollection; }
            private set { scheduledCollection = value; }
        }

        /// <summary>
        /// Gets or sets the notification.
        /// </summary>
        /// <value>The notification.</value>
        public static NotificationManager Notification
        {
            get { return notification; }
            set { notification = value; }
        }


        public static QueryMonitorUpgradeHelper QueryMonitorUpgradeHelper
        {
            get { return queryMonitorUpgradeHelper; }
            set { queryMonitorUpgradeHelper = value; }
        }

        public static AlertsUpgradeHelper AlertsUpgradeHelper
        {
            get { return alertsUpgradeHelper; }
            set { alertsUpgradeHelper = value; }
        }

        public static BaselineTemplatesUpgradeHelper BaselineUpgradeHelper
        {
            get { return baselineUpgradeHelper; }
            set { baselineUpgradeHelper = value; }
        }

        public static void QueueDelegate(WorkQueueDelegate method)
        {
            workQueue.Enqueue(method);
        }

        public static void JoinWorkQueue()
        {
            workQueue.Stop(true);
            workQueue.Join();
        }


        internal static MetricDefinitions GetMetricDefinitions()
        {
            lock (sync)
            {
                if (metricDefinitions == null)
                {
                    metricDefinitions = new MetricDefinitions(true, true, true);
                    metricDefinitions.Load(ManagementServiceConfiguration.ConnectionString);
                    // share copy with common assembly
                    SharedMetricDefinitions.MetricDefinitions = metricDefinitions;
                }
                return metricDefinitions;
            }
        }


        /// <summary>
        /// //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) - Get PA service proxy
        /// </summary>
        /// <returns></returns>
        public static IPrescriptiveAnalysisService GetPrescriptiveAnalysisService()
        {
            string address = Program.OPTION_PRESCRIPTIVE_HOST_DEFAULT;
            int port = Program.OPTION_PRESCRIPTIVE_PORT_DEFAULT_INT;

            try
            {
                Uri uri = new Uri(String.Format("tcp://{0}:{1}/Prescriptive", address, port));

                ServiceCallProxy proxy = new ServiceCallProxy(typeof(IPrescriptiveAnalysisService), uri.ToString());
                IPrescriptiveAnalysisService ipas = proxy.GetTransparentProxy() as IPrescriptiveAnalysisService;

                return ipas;
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception contacting PA service.", ex);
                return null;
            }
        }
    }
}
