using System;
using System.Collections.Generic;
using System.Data;

using Idera.Newsfeed.Plugins.Objects;
using Idera.Newsfeed.Plugins.UI;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Notification;
using Idera.SQLdm.Common.Notification.Providers;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;


namespace Idera.SQLdm.DesktopClient
{
    using System.Windows.Forms;
    using System.Xml;
    using Common;
    using Idera.SQLdm.Common.Events;
    using Wintellect.PowerCollections;
    using System.Diagnostics;
    using System.Threading;

    internal sealed class ApplicationModel
    {
        private bool initialized = false;
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("ApplicationModel");

        private delegate void SyncActiveInstancesDelegate(IList<MonitoredSqlServer> serverList);
        private delegate void AddActiveInstancesDelegate(IList<MonitoredSqlServerWrapper> instances);
        private delegate void DeleteActiveInstancesDelegate(IList<MonitoredSqlServerWrapper> instances);
        private delegate void UpdateConsoleStatusDelegate();
        
        private delegate void SyncTagsDelegate(IList<Tag> tags);
        private delegate void SyncAddOrUpdateTagDelegate(Tag tag);
        private delegate void SyncRemoveTagsDelegate(IList<Tag> tags);

        private delegate void SyncCustomReportsDelegate(IList<CustomReport> customReports);
        private delegate void SyncAddOrUpdateCustomReportDelegate(CustomReport customReport);
        private delegate void SyncRemoveCustomReportsDelegate(IList<CustomReport> customReports);
        private readonly CustomReportCollection customReports = new CustomReportCollection();

        private delegate void PulseLogInDelegate(string username);

        private static ApplicationModel defaultApplicationModel = new ApplicationModel();
        private readonly MonitoredSqlServerCollection activeInstances = new MonitoredSqlServerCollection();
        private readonly Dictionary<int, MonitoredSqlServer> allInstances = new Dictionary<int, MonitoredSqlServer>();
        private readonly Dictionary<int, MonitoredSqlServer> allRegisteredInstances = new Dictionary<int, MonitoredSqlServer>();
        private readonly Dictionary<int, MonitoredSqlServerStatus> activeInstanceStatus = new Dictionary<int, MonitoredSqlServerStatus>();
        private readonly MetricDefinitions metricDefinitions = new MetricDefinitions(true, false, true);
        private Dictionary<string, WaitTypeInfo> waitTypes; // key-> wait_type, values: [category id, category name, wait defintion]
        private List<string> waitCategories; // sorted list of wait type categories
        private readonly object waitTypeUpdateLock = new object();
        private readonly TagCollection tags = new TagCollection();
        private bool tasksViewEnabled = false;

        // Pulse variables
        private PulseNotificationProviderInfo pulseProvider;
        private int? pulseApplicationId;
        private bool pulseApplicationIdNotReturned = false;
        public event EventHandler<ConfigurePulseEventArgs> ConfigurePulseEventHandler;
        public EventHandler PulseAuthenticationChanged;

        private object focusObject = null;

        private UserToken userToken = new UserToken();

        private static object focusObjectChangeLock = new object();

        public event EventHandler FocusObjectChanged;

        public bool Initialized
        {
            get { return initialized; }
        }

        public static ApplicationModel Default
        {
            get { return defaultApplicationModel; }
        }

        public UserToken UserToken
        {
            get { return userToken; }
        }

        public MonitoredSqlServerCollection ActiveInstances
        {
            get { return activeInstances; }
        }

        public Dictionary<int, MonitoredSqlServer> AllInstances
        {
            get { return allInstances; }
        }

        public Dictionary<int, MonitoredSqlServer> AllRegisteredInstances
        {
            get { return allRegisteredInstances; }
        }

        public TagCollection Tags
        {
            get { return tags; }
        }

        public CustomReportCollection CustomReports
        {
            get { return customReports; }
        }

        public PulseNotificationProviderInfo PulseProvider
        {
            get { return pulseProvider; }
        }

        public int? PulseApplicationId
        {
            get { return pulseApplicationId; }
        }

        /// <summary>
        /// Returns true if a Pulse notification provider exists in the repository, otherwise false.
        /// </summary>
        public bool IsPulseConfigured
        {
            get { return pulseProvider != null; }
        }

        public MonitoredSqlServerStatus GetInstanceStatus(int instanceID)
        {
            MonitoredSqlServerStatus status = null;
            lock (activeInstanceStatus)
            {
                activeInstanceStatus.TryGetValue(instanceID, out status);
            }
            return status;
        }

        public void UpdateInstanceSnoozedStatus(int instanceID, int countSnoozed)
        {
            MonitoredSqlServerStatus status = null;
            lock (activeInstanceStatus)
            {
                if (activeInstanceStatus.TryGetValue(instanceID, out status))
                {
                    status.SetAlertsSnoozed(countSnoozed);
                }
            }
            ApplicationController.Default.RefreshBackgroundData();
        }

        public object FocusObject
        {
            get { return focusObject; }
            set
            {
                lock (focusObjectChangeLock)
                {
                    if (focusObject != value)
                    {
                        focusObject = value;
                        OnFocusObjectChanged(EventArgs.Empty);
                    }
                }
            }
        }

        public MetricDefinitions MetricDefinitions
        {
            get { return metricDefinitions; }
        }

        public Dictionary<string, WaitTypeInfo> WaitTypes
        {
            get
            {
                // if we don't have it yet, then fire off a thread to retrieve it...
                lock( waitTypeUpdateLock )
                {
                    if( waitTypes == null )
                    {
                        waitTypes = new Dictionary<string, WaitTypeInfo>();
                        waitCategories = new List<string>();

                        Thread t = new Thread( new ThreadStart(  delegate()
                        {                            
                            lock( waitTypeUpdateLock )
                            {
                                DataTable data = RepositoryHelper.GetWaitTypeDefinitions();

                                string waittype     = string.Empty;
                                int    categoryid   = 0;
                                string categoryname = string.Empty;
                                string description  = string.Empty;
                                string helplink     = string.Empty;

                                Set<string> categories = new Set<string>();

                                foreach( DataRow row in data.Rows )
                                {
                                    waittype     = ( string )row["WaitType"];                                    
                                    categoryname = ( string )row["Category"];
                                    categoryid   = -1;
                                    description  = string.Empty;
                                    helplink     = string.Empty;

                                    if( !( row["CategoryID"] is DBNull ) )  categoryid  = ( int )row["CategoryID"];                                    
                                    if( !( row["Description"] is DBNull ) ) description = ( string )row["Description"];                                    
                                    if( !( row["HelpLink"] is DBNull ) )    helplink    = ( string )row["HelpLink"];

                                    if( !waitTypes.ContainsKey( waittype ) )                                     
                                        waitTypes.Add( waittype, new WaitTypeInfo( waittype, categoryid, categoryname, description, helplink ) );

                                    if( !categories.Contains( categoryname ) )
                                    {
                                        categories.Add( categoryname );
                                        waitCategories.Add( categoryname );
                                    }
                                }

                                waitCategories.Sort();
                            }

                        } ) );

                        t.IsBackground = true;
                        t.Name = "Loading DesktopClient.ApplicationModel.WaitTypes";
                        t.Start();
                    }

                    return waitTypes;
                }                
            }
        }

        public IList<string> WaitCategories
        {
            get
            {
                if( !Monitor.TryEnter( waitTypeUpdateLock ) )
                    return null;

                try
                {
                    return waitCategories.AsReadOnly();
                }
                finally
                {
                    Monitor.Exit( waitTypeUpdateLock );
                }
            }
        }

        public void Initialize()
        {
            using (LOG.VerboseCall("Initialize"))
            {
                FocusObject = null;
                activeInstances.Clear();
                activeInstanceStatus.Clear();
                metricDefinitions.Clear();
                RefreshUserToken();
                RefreshActiveInstances();
                RefreshMetricMetaData();
                RefreshTags();
                RefreshCustomReports();
                RefreshTasksEnabled();
                RefreshPulseConfiguration();

                if (System.Threading.Thread.CurrentThread.IsBackground)
                {
                    UpdateConsoleStatusDelegate updateDelegate = ApplicationController.Default.UpdateConsoleStatus;
                    Program.MainWindow.Dispatcher.BeginInvoke(updateDelegate);
                }
                else
                {
                    ApplicationController.Default.UpdateConsoleStatus();
                }

                initialized = true;
            }
        }

        public bool IsTasksViewEnabled
        {
            get { return tasksViewEnabled; }
        }

        private void RefreshTasksEnabled()
        {
            bool oldvalue = IsTasksViewEnabled;

            int setting = Settings.Default.TasksEnabled;
            if (setting != 0)
            {
                tasksViewEnabled = setting > 0;
            }
            else
            {
                tasksViewEnabled = false;

                List<string> rules = RepositoryHelper.GetNotificationRules(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                foreach (string xml in rules)
                {
                    if (xml.Contains("<TaskDestination "))
                    {
                        tasksViewEnabled = true;
                        break;
                    }
                }
            }
        }

        private void OnFocusObjectChanged(EventArgs e)
        {
            if (FocusObjectChanged != null)
            {
                FocusObjectChanged(this, e);
            }
        }


        /// <summary>
        /// Refresh the active instance list and the instance status.
        /// </summary>
        public void RefreshActiveInstances()
        {
            RefreshActiveInstances(true);
        }

        /// <summary>
        /// Refresh the active instance list and optionally instance status.
        /// </summary>
        /// <param name="refreshInstanceStatus"></param>
        private void RefreshActiveInstances(bool refreshInstanceStatus)
        {
            using (LOG.DebugCall("RefreshActiveInstances"))
            {
                if (Settings.Default.ActiveRepositoryConnection == null)
                {
                    throw new DesktopClientException(
                        "Unable to refresh active instances because the active connection is null.");
                }

                if (refreshInstanceStatus)
                {
                    RefreshMonitoredSqlServerStatus(false);
                }

                // Get the list of monitored SQL Servers from the repository.
                IList<MonitoredSqlServer> serverList =
                    RepositoryHelper.GetMonitoredSqlServers(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                            false);

                // Process the list of monitored SQL Servers from the repository.
                //   Create and update filtered list with servers for which user has permission.
                //   Update the all active instances list of servers (contain servers with and without permissions)
                List<MonitoredSqlServer> filteredServerList = new List<MonitoredSqlServer>();
                allInstances.Clear();
                allRegisteredInstances.Clear();
                foreach (MonitoredSqlServer s in serverList)
                {
                    // If user has permission add to filtered list.
                    if (userToken.GetServerPermission(s.Id) != PermissionType.None)
                    {
                        if (s.IsActive)
                        {
                            filteredServerList.Add(s);
                        }
                        allInstances.Add(s.Id, s);
                    }

                    // Add the server to all instances collection.
                    allRegisteredInstances.Add(s.Id, s);
                }

                InvokeSyncActiveInstances(filteredServerList);
            }
        }

        private void SyncActiveInstances(IList<MonitoredSqlServer> serverList)
        {
            using (LOG.DebugCall("SyncActiveInstances"))
            {
                // this method must be called on the UI thread.
                if (serverList != null)
                {
                    // If no servers in the list, clear the active instances list.
                    if (serverList.Count == 0)
                    {
                        activeInstances.Clear();
                    }
                    else // process the server list.
                    {
                        IDictionary<int, MonitoredSqlServerWrapper> diffInstances = activeInstances.GetDictionary();

                        if (diffInstances != null)
                        {
                            List<MonitoredSqlServerWrapper> newInstances = new List<MonitoredSqlServerWrapper>();
                            List<MonitoredSqlServer> updateInstances = new List<MonitoredSqlServer>();

                            foreach (MonitoredSqlServer instance in serverList)
                            {
                                if (activeInstances.Contains(instance.Id))
                                {
                                    updateInstances.Add(instance);
                                    diffInstances.Remove(instance.Id);
                                }
                                else
                                {
                                    newInstances.Add(InitializeInstanceWrapper(instance));
                                }
                            }

                            LOG.Info(string.Format("Synchronizing - Removing {0} instances.", diffInstances.Count));
                            activeInstances.RemoveRange(diffInstances.Values);

                            LOG.Info(string.Format("Synchronizing - Updating {0} instances.", updateInstances.Count));
                            activeInstances.UpdateRange(updateInstances);

                            LOG.Info(string.Format("Synchronizing - Adding {0} instances.", newInstances.Count));
                            activeInstances.AddRange(newInstances);
                        }
                        else
                        {
                            List<MonitoredSqlServerWrapper> newInstances = new List<MonitoredSqlServerWrapper>();

                            foreach (MonitoredSqlServer instance in serverList)
                            {
                                newInstances.Add(InitializeInstanceWrapper(instance));
                            }

                            LOG.Info(string.Format("Synchronizing - Adding {0} instances.", newInstances.Count));
                            activeInstances.AddRange(newInstances);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Execute the SyncActiveInstances method on the UI thread. 
        /// </summary>
        /// <param name="serverList"></param>
        private void InvokeSyncActiveInstances(IList<MonitoredSqlServer> serverList)
        {
            if (System.Threading.Thread.CurrentThread.IsBackground)
            {
                LOG.Debug("Using BeginInvoke to execute SyncActiveInstances on the UI thread.");
                SyncActiveInstancesDelegate raid = SyncActiveInstances;
                Program.MainWindow.Dispatcher.BeginInvoke(raid, serverList);
            }
            else
            {
                SyncActiveInstances(serverList);
            }
        }

        /// <summary>
        /// Execute the SyncTags method on the UI thread. 
        /// </summary>
        /// <param name="syncTags"></param>
        private void InvokeSyncTags(ICollection<Tag> syncTags)
        {
            if (System.Threading.Thread.CurrentThread.IsBackground)
            {
                LOG.Debug("Using BeginInvoke to execute SyncTags on the UI thread.");
                SyncTagsDelegate raid = SyncTags;
                Program.MainWindow.Dispatcher.BeginInvoke(raid, syncTags);
            }
            else
            {
                SyncTags(syncTags);
            }
        }

        private void SyncTags(ICollection<Tag> syncTags)
        {
            using (LOG.DebugCall("SyncTags"))
            {
                // this method must be called on the UI thread.
                if (syncTags != null)
                {
                    // If no tags in the list
                    if (syncTags.Count == 0)
                    {
                        tags.Clear();
                    }
                    else
                    {
                        IDictionary<int, Tag> diffTags = tags.GetDictionary();

                        if (diffTags != null)
                        {
                            List<Tag> newTags = new List<Tag>();
                            List<Tag> updateTags = new List<Tag>();

                            foreach (Tag tag in syncTags)
                            {
                                if (tags.Contains(tag.Id))
                                {
                                    updateTags.Add(tag);
                                    diffTags.Remove(tag.Id);
                                }
                                else
                                {
                                    newTags.Add(tag);
                                }
                            }

                            LOG.Info(string.Format("Synchronizing - Removing {0} tags.", diffTags.Count));
                            tags.RemoveRange(diffTags.Values);

                            LOG.Info(string.Format("Synchronizing - Updating {0} tags.", updateTags.Count));
                            tags.UpdateRange(updateTags);

                            LOG.Info(string.Format("Synchronizing - Adding {0} tags.", newTags.Count));
                            tags.AddRange(newTags);
                        }
                        else
                        {
                            LOG.Info(string.Format("Synchronizing - Adding {0} instances.", syncTags.Count));
                            tags.AddRange(syncTags);
                        }
                    }
                }
            }
        }

        private MonitoredSqlServerWrapper InitializeInstanceWrapper(MonitoredSqlServer instance)
        {
            MonitoredSqlServerWrapper wrapper = new MonitoredSqlServerWrapper(instance);
            MonitoredSqlServerStatus status = GetInstanceStatus(instance.Id);
            if (status != null)
            {
                status.Instance = wrapper;
            }

            return wrapper;
        }

        public void BackgroundThreadRefresh()
        {
            using (LOG.DebugCall("BackgroundThreadRefresh"))
            {
                lock (defaultApplicationModel)
                {
                    RefreshUserToken();              // The refresh order is important
                    RefreshMonitoredSqlServerStatus();
                    RefreshMetricMetaData();
                    RefreshTags();
                    RefreshCustomReports();
                    RefreshPulseConfiguration();
                }
            }
        }

        /// <summary>
        /// Refresh the permissions assigned to the connected user.
        /// </summary>
        public void RefreshUserToken()
        {
            using (LOG.DebugCall("RefreshUserToken"))
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                try
                {
                    userToken.Refresh(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);
                }
                catch (Exception e)
                {
                    LOG.Error("Error updating user token from the repository. ", e);
                }
                finally
                {
                    stopWatch.Stop();
                    LOG.InfoFormat("RefreshPermissionToken took {0} milliseconds.",
                                   stopWatch.ElapsedMilliseconds);
                }
            }
        }

        public void RefreshMetricMetaData()
        {
            using (LOG.DebugCall("RefreshMetricMetaData"))
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                try
                {
                    metricDefinitions.Reload(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);
                }
                catch(Exception e)
                {
                    LOG.Error("Error updating cache of metric metadata from the repository. ", e);
                } 
                finally
                {
                    stopWatch.Stop();
                    LOG.InfoFormat("RefreshMetricMetaData took {0} milliseconds.",
                                   stopWatch.ElapsedMilliseconds);
                }
            }
        }
        #region Custom Reports
        /// <summary>
        /// Execute the SyncCustomReports method on the UI thread. 
        /// </summary>
        /// <param name="CustomReportsList"></param>
        private void InvokeSyncCustomReports(ICollection<CustomReport> CustomReportsList)
        {
            if (System.Threading.Thread.CurrentThread.IsBackground)
            {
                LOG.Debug("Using BeginInvoke to execute SyncCustomReports on the UI thread.");
                SyncCustomReportsDelegate raid = SyncCustomReports;
                Program.MainWindow.Dispatcher.BeginInvoke(raid, CustomReportsList);
            }
            else
            {
                SyncCustomReports(CustomReportsList);
            }
        }
        private void SyncCustomReports(ICollection<CustomReport> syncCustomReportsList)
        {
            using (LOG.DebugCall("SyncCustomReports"))
            {
                // this method must be called on the UI thread.
                if (syncCustomReportsList != null)
                {
                    // If no tags in the list
                    //if (syncCustomReportsList.Count == 0)
                    //{
                    //    customReports.Clear();
                    //}
                    //else
                    //{
                        if (customReports != null)
                        {
                            List<CustomReport> newReports = new List<CustomReport>();
                            List<CustomReport> updateReports = new List<CustomReport>();

                            Dictionary<string, CustomReport> deleteReports = new Dictionary<string,CustomReport>();

                            foreach (CustomReport report in customReports.Values)
                            {
                                deleteReports.Add(report.Name, report);
                            }

                            if (deleteReports != null)
                            {
                                foreach (CustomReport report in syncCustomReportsList)
                                {
                                    if (customReports.ContainsKey(report.Name))
                                    {
                                        updateReports.Add(report);
                                        deleteReports.Remove(report.Name);
                                    }
                                    else
                                    {
                                        newReports.Add(report);
                                    }
                                }

                                LOG.Info(string.Format("Synchronizing - Removing {0} custom reports.",
                                                       deleteReports.Count));
                                customReports.RemoveRange(deleteReports.Values);

                                LOG.Info(string.Format("Synchronizing - Updating {0} custom reports.",
                                                       updateReports.Count));
                                customReports.UpdateRange(updateReports);

                                LOG.Info(string.Format("Synchronizing - Adding {0} custom reports.", newReports.Count));
                                customReports.AddRange(newReports);
                            }
                            else
                            {
                                LOG.Info(string.Format("Synchronizing - Adding {0} instances.", syncCustomReportsList.Count));
                                customReports.AddRange(syncCustomReportsList);
                            }
                        }
                    //}
                }
            }
        }

        public void RefreshCustomReports()
        {
            using (LOG.DebugCall("RefreshCustomReports"))
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                try
                {
                    //get all reports from the repository
                    ICollection<CustomReport> CustomReportsList = RepositoryHelper.GetCustomReportsList(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    //// Filter tags based on the current user token permissions
                    //foreach (string report in customReports)
                    //{
                    //    tag.FilterInstances(userToken.ActiveAssignedServerIds);
                    //}


                    InvokeSyncCustomReports(CustomReportsList);
                }
                catch (Exception e)
                {
                    LOG.Error("Error updating Custom Reports from the repository.", e);
                }
                finally
                {
                    stopWatch.Stop();
                    LOG.InfoFormat("RefreshCustomReports took {0} milliseconds.", stopWatch.ElapsedMilliseconds);
                }
            }
        }
        public int AddOrUpdateCustomReport(CustomReport customReport)
        {
            int reportID = -1;

            using (LOG.DebugCall("AddOrUpdateCustomReport"))
            {
                IManagementService defaultManagementService =
                        ManagementServiceHelper.GetDefaultService(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                if(customReports.ContainsKey(customReport.Name))
                {
                    reportID = defaultManagementService.UpdateCustomReport(CustomReport.Operation.Update,
                        null, customReport.Name, customReport.ShortDescription,
                        new Serialized<string>(customReport.ReportRDL));

                    InvokeAddOrUpdateCustomReport(customReport);
                }
                else
                {
                    defaultManagementService.UpdateCustomReport(CustomReport.Operation.New, null, 
                        customReport.Name, "",
                        new Serialized<string>(customReport.ReportRDL));

                    InvokeAddOrUpdateCustomReport(customReport);
                }

                return reportID;
            }
        }

        private void InvokeAddOrUpdateCustomReport(CustomReport customReport)
        {
            if (System.Threading.Thread.CurrentThread.IsBackground)
            {
                SyncAddOrUpdateCustomReportDelegate syncDelegate = SyncAddOrUpdateCustomReport;
                Program.MainWindow.Dispatcher.BeginInvoke(syncDelegate, customReport);
            }
            else
            {
                SyncAddOrUpdateCustomReport(customReport);
            }
        }

        private void SyncAddOrUpdateCustomReport(CustomReport customReport)
        {
            customReports.AddOrUpdate(customReport);
        }

        public void RemoveCustomReport(string customReportToRemove)
        {
            IManagementService defaultManagementService =
                ManagementServiceHelper.GetDefaultService(
                    Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            using (LOG.DebugCall("RemoveCustomReport"))
            {
                if (customReportToRemove != null)
                {
                    defaultManagementService.UpdateCustomReport(CustomReport.Operation.Delete,
                        null, customReportToRemove, null, null);
                    InvokeRemoveCustomReport(customReportToRemove);
                }
            }
        }

        public void RemoveCustomReport(ICollection<CustomReport> customReportsToRemove)
        {
            IManagementService defaultManagementService =
                ManagementServiceHelper.GetDefaultService(
                    Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            using (LOG.DebugCall("RemoveCustomReports"))
            {
                if (customReportsToRemove != null && customReportsToRemove.Count > 0)
                {
                    List<string> customReportNames = new List<string>();

                    foreach (CustomReport report in customReportsToRemove)
                    {
                        defaultManagementService.UpdateCustomReport(CustomReport.Operation.Delete,
                            null, report.Name,null,null);
                        InvokeRemoveCustomReport(customReportsToRemove);
                    }
                }
            }
        }

        private void InvokeRemoveCustomReport(ICollection<CustomReport> customReportsToRemove)
        {
            if (System.Threading.Thread.CurrentThread.IsBackground)
            {
                SyncRemoveCustomReportsDelegate syncDelegate = SyncRemoveCustomReports;
                Program.MainWindow.Dispatcher.BeginInvoke(syncDelegate, customReportsToRemove);
            }
            else
            {
                SyncRemoveCustomReports(customReportsToRemove);
            }
        }
        private void InvokeRemoveCustomReport(string customReportToRemove)
        {
            if (System.Threading.Thread.CurrentThread.IsBackground)
            {
                SyncRemoveCustomReportsDelegate syncDelegate = SyncRemoveCustomReports;
                Program.MainWindow.Dispatcher.BeginInvoke(syncDelegate, customReportToRemove);
            }
            else
            {
                SyncRemoveCustomReports(customReportToRemove);
            }
        }
        private void SyncRemoveCustomReports(string customReportToRemove)
        {
            CustomReports.RemoveItem(customReportToRemove);
        }

        private void SyncRemoveCustomReports(ICollection<CustomReport> customReportsToRemove)
        {
            CustomReports.RemoveRange(customReportsToRemove);
        }

        #endregion

        #region Pulse

        public void InitializePulse()
        {
            if (Thread.CurrentThread.IsBackground)
            {
                MethodInvoker fginvoker = InitializePulseInternal;
                Program.MainWindow.Dispatcher.BeginInvoke(fginvoker);
                return;
            }

            InitializePulseInternal();
        }

        private void InitializePulseInternal()
        {
            LOG.Info("Initializing Newsfeed Platform connection.");
            pulseProvider = null;
            pulseApplicationId = null;
            PulseController.Default.SetAppConfigurationInfo(Application.ProductName);
            PulseController.Default.ResetPulsePlatformConfiguration();
        }

        public void RefreshPulseConfiguration()
        {
            using (LOG.InfoCall("RefreshPulseConfiguration"))
            {
                PulseNotificationProviderInfo provider = null;

                List<NotificationProviderInfo> providers =
                    RepositoryHelper.GetNotificationProvider(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                             NotificationProviderInfo.GetDefaultId
                                                                 <PulseNotificationProviderInfo>().Value);

                if (providers == null || providers.Count == 0)
                {
                    LOG.Info("No Newsfeed Platform connection configuration found.");
                    InitializePulse();
                }
                else
                {
                    provider = providers[0] as PulseNotificationProviderInfo;
                }
                
                if (provider is PulseNotificationProviderInfo && pulseProvider == null)
                {
                    LOG.Info("Configuring Newsfeed Platform connection.");

                    bool configurationComplete = false;

                    pulseProvider = provider as PulseNotificationProviderInfo;

                    try
                    {
                        IManagementService managementService =
                            ManagementServiceHelper.GetDefaultService(
                                Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                        if (managementService != null)
                        {
                            pulseApplicationId = managementService.GetPulseApplicationId();

                            string applicationUsername =
                                Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UseIntegratedSecurity
                                    ? string.Format("{0}\\{1}", Environment.UserDomainName, Environment.UserName)
                                    : Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UserName;

                            PulseController.Default.ConfigurePulsePlatformConnection(PulseProvider.PulseServer,
                                                                                     PulseProvider.PulseServerPort);

                            PulseController.Default.SetApplicationInfo(pulseApplicationId.Value, 
                                                                        applicationUsername,
                                                                        userToken.IsSecurityEnabled,
                                                                        userToken.IsSQLdmAdministrator,
                                                                        userToken.IsSysadmin);
                            configurationComplete = true;

                            InvokePulseLogIn(Settings.Default.PulseAccountKeepLoggedIn ? Settings.Default.PulseAccountName : null);
                        }

                        LOG.Info("Newsfeed Platform connection configuration complete.");
                    }
                    catch (Exception e)
                    {
                        LOG.Error("An error occurred while configuring the Newsfeed Platform connection.", e);
                    }

                    if (!configurationComplete)
                    {
                        LOG.Info("Newsfeed Platform connection configuration was incomplete.");
                        InitializePulse();
                    }
                }
            }
        }

        private static void InvokePulseLogIn(string username)
        {
            if (Thread.CurrentThread.IsBackground)
            {
                PulseLogInDelegate loginDelegate = PulseLogIn;
                Program.MainWindow.Dispatcher.BeginInvoke(loginDelegate, username);
            }
            else
            {
                PulseLogIn(username);
            }
        }

        private static void PulseLogIn(string username)
        {
            PulseController.Default.LogIn(username, Settings.Default.PulseAccountKeepLoggedIn);
        }

        public void AuthenticatePulseUser()
        {
            OnPulseAuthenticationChanged();
        }

        private void OnPulseAuthenticationChanged()
        {
            if (PulseAuthenticationChanged != null)
            {
                PulseAuthenticationChanged(this, new EventArgs());
            }
        }

        #endregion

        public void RefreshTags()
        {
            using (LOG.DebugCall("RefreshTags"))
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                try
                {
                    ICollection<Tag> syncTags =
                        RepositoryHelper.GetTags(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    // Filter tags based on the current user token permissions
                    if (!userToken.IsSQLdmAdministrator)
                    {
                        foreach (Tag tag in syncTags)
                        {
                            tag.FilterInstances(userToken.ActiveAssignedServerIds);
                        }
                    }

                    InvokeSyncTags(syncTags);
                }
                catch (Exception e)
                {
                    LOG.Error("Error updating tags from the repository.", e);
                }
                finally
                {
                    stopWatch.Stop();
                    LOG.InfoFormat("RefreshTags took {0} milliseconds.", stopWatch.ElapsedMilliseconds);
                }
            }
        }

        public int AddOrUpdateTag(Tag tag)
        {
            using (LOG.DebugCall("AddOrUpdateTag"))
            {
                IManagementService defaultManagementService =
                        ManagementServiceHelper.GetDefaultService(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                int tagId = defaultManagementService.UpdateTagConfiguration(tag);
                InvokeAddOrUpdateTag(new Tag(tagId, tag));
                RefreshActiveInstances(false);
                return tagId;
            }
        }

        private void InvokeAddOrUpdateTag(Tag tag)
        {
            if (System.Threading.Thread.CurrentThread.IsBackground)
            {
                SyncAddOrUpdateTagDelegate syncDelegate = SyncAddOrUpdateTag;
                Program.MainWindow.Dispatcher.BeginInvoke(syncDelegate, tag);
            }
            else
            {
                SyncAddOrUpdateTag(tag);
            }
        }

        private void SyncAddOrUpdateTag(Tag tag)
        {
            Tags.AddOrUpdate(tag);
        }

        public void RemoveTags(ICollection<Tag> tagsToRemove)
        {
            using (LOG.DebugCall("RemoveTags"))
            {
                if (tagsToRemove != null && tagsToRemove.Count > 0)
                {
                    List<int> tagIds = new List<int>();

                    foreach (Tag tag in tagsToRemove)
                    {
                        tagIds.Add(tag.Id);
                    }

                    IManagementService defaultManagementService =
                        ManagementServiceHelper.GetDefaultService(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                    defaultManagementService.RemoveTags(tagIds);
                    InvokeRemoveTags(tagsToRemove);
                }
            }
        }

        private void InvokeRemoveTags(ICollection<Tag> tagsToRemove)
        {
            if (System.Threading.Thread.CurrentThread.IsBackground)
            {
                SyncRemoveTagsDelegate syncDelegate = SyncRemoveTags;
                Program.MainWindow.Dispatcher.BeginInvoke(syncDelegate, tagsToRemove);
            }
            else
            {
                SyncRemoveTags(tagsToRemove);
            }
        }

        private void SyncRemoveTags(ICollection<Tag> tagsToRemove)
        {
            Tags.RemoveRange(tagsToRemove);
        }

        /// <summary>
        /// Refresh the instance status from the database and refresh the active
        /// instance list if necessary.
        /// </summary>
        public void RefreshMonitoredSqlServerStatus()
        {
            RefreshMonitoredSqlServerStatus(true);
        }

        /// <summary>
        /// Refresh the instance status from the database and refresh the active
        /// instance list if necessary and refreshActiveInstances is true.
        /// </summary>
        /// <param name="refreshActiveInstances"></param>
        public void RefreshMonitoredSqlServerStatus(bool refreshActiveInstances)
        {
            bool performActiveInstanceRefresh = false;

            using (LOG.DebugCall("RefreshMonitoredSqlServerStatus"))
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                try
                {
                    XmlDocument document = null;
                    try
                    {
                        LOG.Verbose("Getting status document from the management service");
                        IManagementService managementService =
                            ManagementServiceHelper.GetDefaultService(
                                Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                        if (managementService != null)
                        {
                            string xml = managementService.GetMonitoredSQLServerStatusDocument();
                            if (xml != null)
                            {
                                XmlDocument doc = new XmlDocument();
                                doc.LoadXml(xml);
                                document = doc;
                            }
                            LOG.Verbose("Got cached status document from the management service");
                        }
                    } catch (Exception e)
                    {
                        LOG.Error("Error getting status document from the management service.  ", e);
                    }
                    // if we failed to get a valid xml document from the management service get it from the database
                    if (document == null)
                    {
                        SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
                        LOG.VerboseFormat("Getting status document from the repository (connection info: {0})", connectionInfo);
                        document = RepositoryHelper.GetMonitoredSqlServerStatus(connectionInfo, null);
                    }

                    lock (activeInstanceStatus)
                    {
                        IDictionary<int, MonitoredSqlServerWrapper> diffInstances = activeInstances.GetDictionary();
                        IDictionary<int, MonitoredSqlServerStatus> diffStatus = new Dictionary<int, MonitoredSqlServerStatus>(activeInstanceStatus);
                        MonitoredSqlServerStatus status = null;

                        if (document != null && document.DocumentElement != null)
                        {
                            foreach (XmlNode node in document.DocumentElement.ChildNodes)
                            {
                                XmlAttribute attribute = node.Attributes["SQLServerID"];

                                if (attribute != null)
                                {
                                    int id;

                                    if (Int32.TryParse(attribute.Value, out id))
                                    {
                                        // Continue processing only if we have permission on this server.
                                        if (userToken.GetServerPermission(id) != PermissionType.None)
                                        {
                                            if (activeInstanceStatus.TryGetValue(id, out status))
                                            {   // existing node - update it
                                                bool beforeHasCustomCounters = status.CustomCounterCount > 0;
                                                ServerVersion beforeServerVersion = status.InstanceVersion;

                                                status.Update(node);
                                                diffStatus.Remove(id);

                                                if (activeInstances.Contains(id))
                                                {
                                                    // see if maintenance mode changed and update monitored server object
                                                    MonitoredSqlServerWrapper wrapper = activeInstances[id];

                                                    if (wrapper.MaintenanceModeEnabled != status.IsInMaintenanceMode)
                                                    {
                                                        wrapper.MaintenanceModeEnabled = status.IsInMaintenanceMode;
                                                    } else
                                                    if (beforeHasCustomCounters != (status.CustomCounterCount > 0))
                                                    {
                                                        wrapper.FireChanged();        
                                                    } else
                                                    if (status.InstanceVersion != null)
                                                    {
                                                        if (beforeServerVersion == null || beforeServerVersion.Major != status.InstanceVersion.Major)
                                                            wrapper.FireChanged();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                status = new MonitoredSqlServerStatus(node);
                                                activeInstanceStatus.Add(id, status);
                                            }

                                            // if able to refresh active instances determine if we need to
                                            if (refreshActiveInstances && !performActiveInstanceRefresh)
                                            {
                                                if (diffInstances != null && diffInstances.ContainsKey(id))
                                                {
                                                    diffInstances.Remove(id);
                                                }
                                                else
                                                {
                                                    performActiveInstanceRefresh = true;
                                                }
                                            }
                                        }
                                    }   
                                }
                            }
                        }

                        // get rid of instances that are no longer needed
                        foreach (KeyValuePair<int, MonitoredSqlServerStatus> pair in diffStatus)
                        {
                            activeInstanceStatus.Remove(pair.Key);
                        }

                        // if able to refresh active instances see if instanced were deleted
                        if (refreshActiveInstances && !performActiveInstanceRefresh)
                        {
                            performActiveInstanceRefresh = diffInstances != null ? diffInstances.Count > 0 : false;
                        }
                    }

                    // RefreshActiveInstanceStatus must be called without a lock on activeInstanceStatus or a deadlock will occur
                    if (performActiveInstanceRefresh)
                    {
                        RefreshActiveInstances(false);
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("An error occurred while refreshing monitored sql server status list.", e);
                } 
                finally
                {
                    stopWatch.Stop();
                    LOG.InfoFormat("RefreshMonitoredSqlServerStatus took {0} milliseconds.",
                                   stopWatch.ElapsedMilliseconds);
                }
            }
        }

        public IList<MonitoredSqlServerWrapper> AddMonitoredSqlServers(IList<MonitoredSqlServerConfiguration> configurations)
        {
            if (configurations != null && configurations.Count > 0)
            {
                try
                {
					//[START]SQLdm 9.0 (Ankit Srivastava)- Resolved defect DE44130 - decreased the default timeout to 20
                    var connInfo=Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
					connInfo.ConnectionTimeout = 20;
                    IManagementService managementService =
                        ManagementServiceHelper.GetDefaultService(
                            connInfo);
					//[END]SQLdm 9.0 (Ankit Srivastava)- Resolved defect DE44130 - decreased the default timeout to 20

                    AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                    ICollection<MonitoredSqlServer> addedServers = managementService.AddMonitoredSqlServers(configurations);

                    if (addedServers != null)
                    {
                        List<MonitoredSqlServerWrapper> addedServerWrappers = new List<MonitoredSqlServerWrapper>();

                        foreach (MonitoredSqlServer addedServer in addedServers)
                        {
                            addedServerWrappers.Add(InitializeInstanceWrapper(addedServer));
                        }

                        // Add the server in the user token.   Note when
                        // background refresh happens the user token will be synced, 
                        // but this is a short term update until the background refresh
                        // happens.
                        foreach (MonitoredSqlServerWrapper instance in addedServerWrappers)
                        {
                            userToken.AddServer(instance.Id, instance.InstanceName);

                            allInstances.Remove(instance.Id);
                            allInstances.Add(instance.Id, instance.Instance);
                            allRegisteredInstances.Remove(instance.Id);
                            allRegisteredInstances.Add(instance.Id, instance.Instance);                            
                        }

                        InvokeAddActiveInstances(addedServerWrappers);
                        RefreshTags();
                        return addedServerWrappers;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (System.Net.Sockets.SocketException)
                {
                    throw new DesktopClientException(Resources.ExceptionManagementServiceUnavailable);
                }
            }
            else
            {
                return null;
            }
        }

        private void InvokeAddActiveInstances(IList<MonitoredSqlServerWrapper> instances)
        {
            if (System.Threading.Thread.CurrentThread.IsBackground)
            {
                AddActiveInstancesDelegate addDelegate = AddActiveInstances;
                Program.MainWindow.Dispatcher.BeginInvoke(addDelegate, instances);
            }
            else
            {
                AddActiveInstances(instances);
            }
        }

        private void AddActiveInstances(IList<MonitoredSqlServerWrapper> instances)
        {
            activeInstances.AddRange(instances);
        }

        public void DeactivateMonitoredSqlServers(IList<MonitoredSqlServerWrapper> instances)
        {
            if (instances != null && instances.Count > 0)
            {
                try
                {
                    IManagementService managementService =
                        ManagementServiceHelper.GetDefaultService(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    List<int> ids = new List<int>();
                    foreach (MonitoredSqlServerWrapper instance in instances)
                    {
                        ids.Add(instance.Id);
                    }

                    AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                    managementService.DeactivateMonitoredSqlServers(ids);

                    // Deactivate the server in the user token.   Note when
                    // background refresh happens the user token will be synced, 
                    // but this is a short term update until the background refresh
                    // happens.
                    foreach (MonitoredSqlServerWrapper instance in instances)
                    {
                        userToken.DeactivateServer(instance.Id);                        
                    }

                    RefreshTags();
                    InvokeDeleteActiveInstances(instances);
                }
                catch (System.Net.Sockets.SocketException)
                {
                    throw new DesktopClientException(Resources.ExceptionManagementServiceUnavailable);
                }
            }
        }

        private void InvokeDeleteActiveInstances(IList<MonitoredSqlServerWrapper> instances)
        {
            if (System.Threading.Thread.CurrentThread.IsBackground)
            {
                DeleteActiveInstancesDelegate addDelegate = DeleteActiveInstances;
                Program.MainWindow.Dispatcher.BeginInvoke(addDelegate, instances);
            }
            else
            {
                DeleteActiveInstances(instances);
            }
        }

        private void DeleteActiveInstances(IList<MonitoredSqlServerWrapper> instances)
        {
            activeInstances.RemoveRange(instances);
        }

        public void DeleteMonitoredSqlServers(IList<MonitoredSqlServerWrapper> instances)
        {
            if (instances != null && instances.Count > 0)
            {
                try
                {
                    IManagementService managementService =
                        ManagementServiceHelper.GetDefaultService(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    List<int> ids = new List<int>();
                    foreach (MonitoredSqlServerWrapper instance in instances)
                    {
                        ids.Add(instance.Id);
                    }

                    AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                    managementService.DeleteMonitoredSqlServers(ids);
                    
                    // Delete the entry in the user token.   Note when
                    // background refresh happens the user token will be synced, 
                    // but this is a short term update until the background refresh
                    // happens.
                    foreach (MonitoredSqlServerWrapper instance in instances)
                    {
                        userToken.DeleteServer(instance.Id);
                        allInstances.Remove(instance.Id);
                        allRegisteredInstances.Remove(instance.Id);
                    }
                    
                    RefreshTags();
                    InvokeDeleteActiveInstances(instances);
                }
                catch (System.Net.Sockets.SocketException)
                {
                    throw new DesktopClientException(Resources.ExceptionManagementServiceUnavailable);
                }
            }
        }

        public IEnumerable<MonitoredSqlServer> UpdateMonitoredSqlServers(IEnumerable<Pair<int, MonitoredSqlServerConfiguration>> configurationList)
        {
            if (configurationList == null) 
                throw new ArgumentNullException("configurationList");

            IManagementService defaultManagementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            IEnumerable<MonitoredSqlServer> servers = defaultManagementService.UpdateMonitoredSqlServers(configurationList);
            foreach (MonitoredSqlServer server in servers)
            {
                activeInstances.AddOrUpdate(server);
            }
            RefreshTags();

            return servers;
        }

        public MonitoredSqlServer UpdateMonitoredSqlServer(int id, MonitoredSqlServerConfiguration configuration) {
            if (configuration == null) {
                throw new ArgumentNullException("configuration");
            }

            IManagementService defaultManagementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            MonitoredSqlServer server = defaultManagementService.UpdateMonitoredSqlServer(id, configuration);
            activeInstances.AddOrUpdate(server);
            RefreshTags();
            return server;
        }

        public void AddCustomCounterToMonitoredServers(int metricID, IList<int> selectedTags, IList<int> selectedInstances)
        {
            IManagementService defaultManagementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            defaultManagementService.AddCounterToServers(metricID, selectedTags, selectedInstances);

            Set<int> affectedServers = new Set<int>();

            foreach (Tag tag in tags)
            {
                if (selectedTags.Contains(tag.Id))
                {
                    if (tag.AddCustomCounter(metricID))
                        affectedServers.AddMany(tag.Instances);
                }
                else
                {
                    if (tag.RemoveCustomCounter(metricID))
                        affectedServers.AddMany(tag.Instances);
                }
            }

            foreach (MonitoredSqlServerWrapper wrapper in activeInstances)
            {
                MonitoredSqlServer server = wrapper;
                if (selectedInstances.Contains(wrapper.Id))
                {
                    if (!server.CustomCounters.Contains(metricID))
                    {
                        server.CustomCounters.Add(metricID);
                        affectedServers.Add(server.Id);
                    }
                } else
                {
                    if (server.CustomCounters.Contains(metricID))
                    {
                        server.CustomCounters.Remove(metricID);
                        affectedServers.Add(server.Id);
                    }
                }
            }

            foreach (int id in affectedServers)
            {
                MonitoredSqlServerWrapper wrapper = activeInstances[id];
                if (wrapper != null)
                    wrapper.FireChanged();
            }
        }

        private void FireInstanceChangedEvent(IList<MonitoredSqlServer> changedServers)
        {
            foreach (MonitoredSqlServer server in changedServers)
            {
                activeInstances.AddOrUpdate(server);
            }
        }

        public void ConfigureMaintenanceMode(int id, bool enabled)
        {
            if (ActiveInstances.Contains(id))
            {
                MonitoredSqlServer instance = ActiveInstances[id];
                MonitoredSqlServerConfiguration configuration = instance.GetConfiguration();
                configuration.MaintenanceModeEnabled = enabled;

                IManagementService managementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                MonitoredSqlServer updatedInstance = managementService.UpdateMonitoredSqlServer(id, configuration);
                ActiveInstances.AddOrUpdate(updatedInstance);

                foreach (UserView userView in Settings.Default.ActiveRepositoryConnection.UserViews)
                {
                    if (userView is SearchUserView)
                        userView.Update();
                }
            }
        }

        public AlertConfiguration GetAlertConfiguration(int instanceId)
        {
            try
            {
                if (ActiveInstances.Contains(instanceId))
                {
                    MonitoredSqlServerWrapper server = ActiveInstances[instanceId];

                    if (server != null)
                    {
                        return RepositoryHelper.GetAlertConfiguration(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                            server.AlertConfiguration,
                            false,
                            null);
                    }
                }
            }
            catch(Exception e)
            {
                LOG.Error(string.Format("An error occurred while retrieving the alert configuration for {0}.", instanceId), e);
            }
            
            return null;
        }

        public event EventHandler PredictiveAnalyticsStateChanged;

        public void NotifyPredictiveAnaltyicsStateChanged()
        {
            if (PredictiveAnalyticsStateChanged != null)
                PredictiveAnalyticsStateChanged(this, EventArgs.Empty);
        }
    }

    internal sealed class ServerTransactionEventArgs : EventArgs
    {
        private MonitoredSqlServer server;

        public ServerTransactionEventArgs(MonitoredSqlServer server)
        {
            this.server = server;
        }

        public MonitoredSqlServer Server
        {
            get { return server; }
        }
    }
}
