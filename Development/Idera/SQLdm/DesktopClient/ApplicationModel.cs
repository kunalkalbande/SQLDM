using System;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
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
    using System.Threading.Tasks;
    using System.Linq;
    using Idera.SQLdm.Common.Events.AzureMonitor;

    internal sealed class ApplicationModel
    {
        private bool initialized = false;
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("ApplicationModel");
        private static BBS.TracerX.Logger StartUpTimeLog = BBS.TracerX.Logger.GetLogger(TextConstants.StartUpTimeLogName);

        private delegate void SyncActiveInstancesDelegate(IList<MonitoredSqlServer> serverList);
        private delegate void AddActiveInstancesDelegate(IList<MonitoredSqlServerWrapper> instances);
        private delegate void DeleteActiveInstancesDelegate(IList<MonitoredSqlServerWrapper> instances);
        private delegate void UpdateConsoleStatusDelegate();
        private delegate void ClearActiveInstancesDelegate();

        private delegate void SyncTagsDelegate(IList<Tag> tags);
        private delegate void SyncAddOrUpdateTagDelegate(Tag tag);
        private delegate void SyncRemoveTagsDelegate(IList<Tag> tags);

        private delegate void SyncCustomReportsDelegate(IList<CustomReport> customReports);
        private delegate void SyncAddOrUpdateCustomReportDelegate(CustomReport customReport);
        private delegate void SyncRemoveCustomReportsDelegate(IList<CustomReport> customReports);
        private readonly CustomReportCollection customReports = new CustomReportCollection();

        private delegate void PulseLogInDelegate(string username);

        private static ApplicationModel defaultApplicationModel = new ApplicationModel();
        private readonly Dictionary<int, MonitoredSqlServerCollection> activeInstances = new Dictionary<int, MonitoredSqlServerCollection>(); 
        private readonly Dictionary<int, Dictionary<int, MonitoredSqlServer>> allInstances = new Dictionary<int, Dictionary<int, MonitoredSqlServer>>();
        private readonly Dictionary<int, Dictionary<int, MonitoredSqlServer>> allRegisteredInstances = new Dictionary<int, Dictionary<int, MonitoredSqlServer>>();
        private readonly Dictionary<int,Dictionary<int, MonitoredSqlServerStatus>> activeInstanceStatus =new Dictionary<int, Dictionary<int, MonitoredSqlServerStatus>>();
        private readonly MetricDefinitions metricDefinitions = new MetricDefinitions(true, false, true);
        private Dictionary<string, WaitTypeInfo> waitTypes; // key-> wait_type, values: [category id, category name, wait defintion]
        private List<string> waitCategories; // sorted list of wait type categories
        private readonly object waitTypeUpdateLock = new object();
        private readonly TagCollection tags = new TagCollection();
        private TagCollection localTags = new TagCollection();
        private bool tasksViewEnabled = false;
        private HistoryTimeValue historyTimeValue = new HistoryTimeValue(); //SqlDM 10.2 (Anshul Aggarwal) - New History Browser

        private Dictionary<int, bool> instancePrivilege = null;
        private BackgroundWorker permissionsBacgroundWorker;

        // Pulse variables
        private PulseNotificationProviderInfo pulseProvider;
        private int? pulseApplicationId;
        private bool pulseApplicationIdNotReturned = false;
        public event EventHandler<ConfigurePulseEventArgs> ConfigurePulseEventHandler;
        public EventHandler PulseAuthenticationChanged;
        private int selectedInstanceId = 0;

        private object focusObject = null;

        private UserToken userToken = new UserToken();

        private static object focusObjectChangeLock = new object();

        //For analysis tab
        private bool analysisHistoryMode = false;

        public event EventHandler FocusObjectChanged;

        //Start :SqlDM 10.2 (Tushar)--Adding a boolean to represent whether saved active repository connection is changed or not. 
        private bool repositoryConnectionChanged =false;

        public bool RepositoryConnectionChanged
        {
            get { return repositoryConnectionChanged; }
            set { repositoryConnectionChanged = value; }
        }
        //End :SqlDM 10.2 (Tushar)--Adding a boolean to represent whether saved active repository connection is changed or not.

        public bool Initialized
        {
            get { return initialized; }
        }

        public static string DateFormat { get; private set; }

        public static ApplicationModel Default
        {
            get { return defaultApplicationModel; }
        }

        public UserToken UserToken
        {
            get { return userToken; }
        }
        public Dictionary<int, MonitoredSqlServerCollection> RepoActiveInstances
        {
            get
            {
                return activeInstances;
            }
        }
        public MonitoredSqlServerCollection ActiveInstances
        {
            get { return activeInstances.Keys.Contains(Settings.Default.CurrentRepoId) ? activeInstances[Settings.Default.CurrentRepoId]:new MonitoredSqlServerCollection(); }
        }

        public int SelectedInstanceId
        {
            get { return selectedInstanceId; }
            set { selectedInstanceId = value; }
        }

        //public Dictionary<int, bool> InstancePrivilege
        //{
        //    get{
        //        if (instancePrivilege == null)
        //        {
        //            instancePrivilege = new Dictionary<int, bool>();
        //            IManagementService managementService = ManagementServiceHelper.GetDefaultService();

        //            ICollection<int> instanceIds = ApplicationModel.Default.ActiveInstances.Keys;
        //            foreach (int id in instanceIds)
        //            {
        //                bool isUserSysAdmin = true; //defaulting to true so that all views show the default behavoir in that case
        //                try
        //                {
        //                    isUserSysAdmin = managementService.isSysAdmin(id);
        //                }
        //                catch (Idera.SQLdm.Common.Services.ServiceCallProxy.ServiceCallException ex)
        //                {
        //                    Idera.SQLdm.Common.UI.Dialogs.ApplicationMessageBox.ShowError(null,
        //                                                    "An error occurred while attempting to contact the Management Service.",
        //                                                    ex as Idera.SQLdm.Common.Services.ServiceCallProxy.ServiceCallException);
        //                }
        //                catch (Exception genericEx)
        //                {
        //                    Idera.SQLdm.Common.UI.Dialogs.ApplicationMessageBox.ShowError(null,
        //                                                    Resources.ExceptionUnhandled,
        //                                                    genericEx as Exception);
        //                }

        //                instancePrivilege.Add(id, isUserSysAdmin);
        //            }
        //        }
        //        return instancePrivilege;
        //    }
        //}

        public Dictionary<int, MonitoredSqlServer> AllInstances
        {
            get { return allInstances[Settings.Default.CurrentRepoId]; }
        }
        public Dictionary<int, MonitoredSqlServer> AllRepoInstances
        {
            get { return allInstances[Settings.Default.RepoId]; }
        }
        public Dictionary<int, MonitoredSqlServer> AllRegisteredInstances
        {
            get { return allRegisteredInstances[Settings.Default.CurrentRepoId]; }
        }
        public Dictionary<int, MonitoredSqlServer> AllRepoRegisteredInstances
        {
            get { return allRegisteredInstances[Settings.Default.RepoId]; }
        }
        public TagCollection Tags
        {
            get { return tags; }
        }

        /// <summary>
        /// SQLdm 10.1(Srishti Purohit)
        /// removing global tags from list 
        /// </summary>
        public TagCollection LocalTags
        {
            get
            {
                return FilterLocalTags();
            }
        }

        //SQLDM 10.1 (Barkha Khatri)
        //Getting local tags from all tags
        private TagCollection FilterLocalTags()
        {
            try
            {

                if (tags != null && tags.Count != 0)
                {
                    foreach (Tag tag in tags)
                    {
                        if (tag.IsGlobalTag == false)
                            localTags.Add(tag);
                    }
                }

                return localTags;
            }
            catch (Exception ex)
            {
                LOG.Error("Error while getting local tags. ", ex);
                return tags;
            }
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

        //For analysis tab
        public bool AnalysisHistoryMode
        {
            get { return analysisHistoryMode; }
            set { analysisHistoryMode = value; }
        }

        public MonitoredSqlServerStatus GetInstanceStatus(int instanceID,int repoId=0)
        {
            MonitoredSqlServerStatus status = null;
            lock (activeInstanceStatus)
            {
                if (repoId == 0)
                {
                    if (activeInstanceStatus.Keys.Contains(Settings.Default.RepoId))
                        activeInstanceStatus[Settings.Default.RepoId].TryGetValue(instanceID, out status);
                }
                else
                {
                    if (activeInstanceStatus.Keys.Contains(repoId))
                        activeInstanceStatus[repoId].TryGetValue(instanceID, out status);
                }
            }
            return status;
        }

        public String GetInstanceDisplayName(int instanceID)
        {
            MonitoredSqlServer instance = null;
            lock (allInstances)
            {
                allInstances[Settings.Default.RepoId].TryGetValue(instanceID, out instance);
            }
            if (instance != null)
            {
                return instance.DisplayInstanceName;
            }
            return "";
        }

        public void UpdateInstanceSnoozedStatus(int instanceID, int countSnoozed)
        {
            MonitoredSqlServerStatus status = null;
            lock (activeInstanceStatus)
            {
                if (activeInstanceStatus[Settings.Default.CurrentRepoId].TryGetValue(instanceID, out status))
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
                lock (waitTypeUpdateLock)
                {
                    if (waitTypes == null)
                    {
                        waitTypes = new Dictionary<string, WaitTypeInfo>();
                        waitCategories = new List<string>();

                        Thread t = new Thread(new ThreadStart(delegate ()
                     {
                         lock (waitTypeUpdateLock)
                         {
                             DataTable data = RepositoryHelper.GetWaitTypeDefinitions();

                             string waittype = string.Empty;
                             int categoryid = 0;
                             string categoryname = string.Empty;
                             string description = string.Empty;
                             string helplink = string.Empty;

                             Set<string> categories = new Set<string>();

                             foreach (DataRow row in data.Rows)
                             {
                                 waittype = (string)row["WaitType"];
                                 categoryname = (string)row["Category"];
                                 categoryid = -1;
                                 description = string.Empty;
                                 helplink = string.Empty;

                                 if (!(row["CategoryID"] is DBNull)) categoryid = (int)row["CategoryID"];
                                 if (!(row["Description"] is DBNull)) description = (string)row["Description"];
                                 if (!(row["HelpLink"] is DBNull)) helplink = (string)row["HelpLink"];

                                 if (!waitTypes.ContainsKey(waittype))
                                     waitTypes.Add(waittype, new WaitTypeInfo(waittype, categoryid, categoryname, description, helplink));

                                 if (!categories.Contains(categoryname))
                                 {
                                     categories.Add(categoryname);
                                     waitCategories.Add(categoryname);
                                 }
                             }

                             waitCategories.Sort();
                         }

                     }));

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
                if (!Monitor.TryEnter(waitTypeUpdateLock))
                    return null;

                try
                {
                    return waitCategories.AsReadOnly();
                }
                finally
                {
                    Monitor.Exit(waitTypeUpdateLock);
                }
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - New History Browser
        /// Gloabl variable for chart visible limit and custom start/end dates
        /// </summary>
        public HistoryTimeValue HistoryTimeValue
        {
            get { return historyTimeValue; }
        }

        public void Initialize()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            FocusObject = null;
            if (System.Threading.Thread.CurrentThread.IsBackground)
            {
                foreach (var key in RepoActiveInstances.Keys)
                {
                    ClearActiveInstancesDelegate clearDelegate = RepoActiveInstances[key].Clear;
                    Program.MainWindow.Dispatcher.BeginInvoke(clearDelegate);
                }
            }
            else
            {
                foreach (var key in RepoActiveInstances.Keys) 
                    RepoActiveInstances[key].Clear();
            }
            activeInstanceStatus.Clear();
            metricDefinitions.Clear();
            RefreshUserToken();
            RefreshActiveInstances();
            ////SQLdm 10.1 (Barkha Khatri) SysAdmin feature-- updating server permissions at launch time
            RefreshServerPermissions();

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
            DateFormat = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            initialized = true;
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by ApplicationModel.Default.Initialize : {0}", stopWatch.ElapsedMilliseconds);
        }

        public bool IsTasksViewEnabled
        {
            get { return tasksViewEnabled; }
        }
        #region SysAdmin feature
        //SQLdm 10.1 (Barkha Khatri) SysAdmin feature-- updating server permissions at launch time
        /// <summary>
        /// refreshing servers permission at launch time
        /// </summary>
        private void RefreshServerPermissions()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            permissionsBacgroundWorker = new BackgroundWorker();
            permissionsBacgroundWorker.DoWork += new DoWorkEventHandler(permissionsBacgroundWorker_DoWork);
            permissionsBacgroundWorker.RunWorkerCompleted +=
                        new RunWorkerCompletedEventHandler(permissionsBacgroundWorker_RunWorkerCompleted);
            
            if (!permissionsBacgroundWorker.IsBusy)
            {
                permissionsBacgroundWorker.RunWorkerAsync();
            }
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by RefreshServerPermissions : {0}", stopWatch.ElapsedMilliseconds);

        }
        /// <summary>
        /// SQLdm 10.1(Barkha Khatri) sysAdmin feature--updating sys admin permissions at launch time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void permissionsBacgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (var conn in Settings.Default.RepositoryConnections)
            {
                //wait until the active instances is updated or until an instance is added.
                while (activeInstances[conn.RepositiryId] != null && activeInstances[conn.RepositiryId].Count == 0)
                {
                    Thread.Sleep(5000);
                }

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                List<Pair<int, bool>> serversPermissionList = new List<Pair<int, bool>>();
                Stopwatch stopWatch1 = new Stopwatch();
                stopWatch1.Start();
                Parallel.ForEach(activeInstances[conn.RepositiryId].Keys, (instanceId) =>
                 {
                     MonitoredSqlServerWrapper instanceWrapper = activeInstances[conn.RepositiryId][instanceId];
                     MonitoredSqlServerMixin serverVersionAndPermission = RepositoryHelper.GetServerVersionAndPermission(instanceWrapper.Instance.ConnectionInfo);
                     if (serverVersionAndPermission != null)
                     {
                     //SQLDM 10.1 Barkha Khatri Updating server permissions here instead of waiting for the background refresh
                     //SQLDM-31053
                     ApplicationModel.Default.AllRegisteredInstances[instanceWrapper.Id].IsUserSysAdmin = serverVersionAndPermission.IsUserSysAdmin;
                         serversPermissionList.Add(new Pair<int, bool>(instanceWrapper.Id, serverVersionAndPermission.IsUserSysAdmin));
                     }
                 });
                stopWatch1.Stop();
                StartUpTimeLog.DebugFormat("Time taken for getting permissions from DB : {0}", stopWatch1.ElapsedMilliseconds);

                stopWatch1.Reset();
                stopWatch1.Start();
                if (serversPermissionList.Count > 0)
                {
                    RepositoryHelper.UpdateSysAdminPermissionsInRepo(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, serversPermissionList);
                }
                stopWatch1.Stop();
                StartUpTimeLog.DebugFormat("Time taken by RepositoryHelper.UpdateSysAdminPermissionsInRepo() : {0}", stopWatch1.ElapsedMilliseconds);
                stopWatch.Stop();
                StartUpTimeLog.DebugFormat("Time taken by permissionsBacgroundWorker_DoWork() : {0}", stopWatch.ElapsedMilliseconds);
            }
        }
        /// <summary>
        /// SQLdm 10.1(Barkha Khatri) sysAdmin feature--updating sys admin permissions at launch time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void permissionsBacgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LOG.Info("permissionsBacgroundWorker_RunWorkerCompleted:permissions updated in Database");
        }
        #endregion
        private void RefreshTasksEnabled()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
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
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by RefreshTasksEnabled : {0}", stopWatch.ElapsedMilliseconds);
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
            Stopwatch stopWatchMain = new Stopwatch();
            stopWatchMain.Start();
            if (Settings.Default.ActiveRepositoryConnection == null)
            {
                throw new DesktopClientException(
                    "Unable to refresh active instances because the active connection is null.");
            }

            if (refreshInstanceStatus)
            {
                RefreshMonitoredSqlServerStatus(false);
            }

            // Process the list of monitored SQL Servers from the repository.
            //   Create and update filtered list with servers for which user has permission.
            //   Update the all active instances list of servers (contain servers with and without permissions)
            // activeInstances.Clear();
            foreach (var key in RepoActiveInstances.Keys)
            {
                RepoActiveInstances[key].Clear();
            }
            List<MonitoredSqlServer> filteredServerList = new List<MonitoredSqlServer>();
            allInstances.Clear();
            allRegisteredInstances.Clear();
            foreach (var connection in Settings.Default.RepositoryConnections)
            {

                Settings.Default.CurrentRepoId = connection.RepositiryId;
             if(!activeInstances.ContainsKey(Settings.Default.CurrentRepoId))
                activeInstances.Add(Settings.Default.CurrentRepoId, new MonitoredSqlServerCollection());
                allInstances.Add(Settings.Default.CurrentRepoId, new Dictionary<int, MonitoredSqlServer>());
                allRegisteredInstances.Add(Settings.Default.CurrentRepoId, new Dictionary<int, MonitoredSqlServer>());

                connection.RefreshRepositoryInfo();
                userToken.Refresh(connection.ConnectionInfo.ConnectionString);
               
                // Get the list of monitored SQL Servers from the repository.
                IList<MonitoredSqlServer> serverList =
                    RepositoryHelper.GetMonitoredSqlServers(connection.ConnectionInfo,
                                                            false);
                foreach (MonitoredSqlServer s in serverList)
                {
                    var Id = RepositoryHelper.GetClusterRepoServerID(s.Id, connection.RepositiryId);
                            s.ClusterRepoId = Id;
                    s.RepoId= connection.RepositiryId;

                    // If user has permission add to filtered list.
                    if (userToken.GetServerPermission(s.Id) != PermissionType.None)
                    {
                        if (s.IsActive)
                        {
                            filteredServerList.Add(s);
                        }
                            AllInstances.Add(s.Id, s);
                    }
                    else
                    {

                    }

                   
                    // Add the server to all instances collection.
                    AllRegisteredInstances.Add(s.Id, s);
                   //else
                   // allRegisteredInstances.Add(s.Id, s);
                }
                //SyncActiveInstances(filteredServerList);
            }
           InvokeSyncActiveInstances(filteredServerList);
            stopWatchMain.Stop();
            StartUpTimeLog.DebugFormat("Time taken by RefreshActiveInstances : {0}", stopWatchMain.ElapsedMilliseconds);
        }

        private void SyncActiveInstances(IList<MonitoredSqlServer> serverList)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            // this method must be called on the UI thread.
            foreach (var con in Settings.Default.RepositoryConnections)
            {
                int id = con.RepositiryId;
                Settings.Default.RepoId = id;
                if (serverList != null)
                {
                    // If no servers in the list, clear the active instances list.
                    if (serverList.Where(s=>s.RepoId==id).Count() == 0)
                    {
                        activeInstances[id].Clear();
                    }
                    else // process the server list.
                    {
                        IDictionary<int, MonitoredSqlServerWrapper> diffInstances = activeInstances[id].GetDictionary();

                        if (diffInstances != null)
                        {
                            List<MonitoredSqlServerWrapper> newInstances = new List<MonitoredSqlServerWrapper>();
                            List<MonitoredSqlServer> updateInstances = new List<MonitoredSqlServer>();

                            foreach (MonitoredSqlServer instance in serverList)
                            {
                                if (instance.RepoId == id)
                                {
                                    if (activeInstances[id].Contains(instance.Id))
                                    {
                                        updateInstances.Add(instance);
                                        diffInstances.Remove(instance.Id);
                                    }
                                    else
                                    {
                                        newInstances.Add(InitializeInstanceWrapper(instance));
                                    }
                                }
                            }

                            LOG.Info(string.Format("Synchronizing - Removing {0} instances.", diffInstances.Count));
                            activeInstances[id].RemoveRange(diffInstances.Values);

                            LOG.Info(string.Format("Synchronizing - Updating {0} instances.", updateInstances.Count));
                            activeInstances[id].UpdateRange(updateInstances);

                            LOG.Info(string.Format("Synchronizing - Adding {0} instances.", newInstances.Count));
                            activeInstances[id].AddRange(newInstances);
                        }
                        else
                        {
                            List<MonitoredSqlServerWrapper> newInstances = new List<MonitoredSqlServerWrapper>();

                            foreach (MonitoredSqlServer instance in serverList)
                            {
                                if (instance.RepoId == id && instance.ClusterRepoId>0)
                                {
                                    newInstances.Add(InitializeInstanceWrapper(instance));
                                }
                            }

                            LOG.Info(string.Format("Synchronizing - Adding {0} instances.", newInstances.Count));
                            activeInstances[id].AddRange(newInstances);
                        }
                    }
                }
            }
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by SyncActiveInstances() : {0}", stopWatch.ElapsedMilliseconds);
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
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            //SQLDM 10.1 (Barkha Khatri )
            //SQLDM -26392 fix 
            if (System.Threading.Thread.CurrentThread.IsBackground)
            {
                LOG.Debug("Using BeginInvoke to execute SyncTags on the UI thread.");
                SyncTagsDelegate raid = SyncTags;
                raid += SyncLocalTags;
                Program.MainWindow.Dispatcher.BeginInvoke(raid, syncTags);
            }
            else
            {
                SyncTags(syncTags);
                SyncLocalTags(syncTags);
            }
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by InvokeSyncTags : {0}", stopWatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Execute the SyncLocalTags method on the UI thread. 
        /// </summary>
        /// <param name="syncTags"></param>
        private void InvokeSyncLocalTags(ICollection<Tag> syncTags)
        {
            if (System.Threading.Thread.CurrentThread.IsBackground)
            {
                LOG.Debug("Using BeginInvoke to execute SyncLocalTags on the UI thread.");
                SyncTagsDelegate raid = SyncLocalTags;
                Program.MainWindow.Dispatcher.BeginInvoke(raid, syncTags);
            }
            else
            {
                SyncLocalTags(syncTags);
            }
        }

        private void SyncTags(ICollection<Tag> syncTags)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            // this method must be called on the UI thread.
            if (syncTags != null)
            {
                LOG.DebugFormat("syncTags Count = {0}", syncTags.Count);
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
            else
                LOG.DebugFormat("syncTags is null.");
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by SyncTags : {0}", stopWatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// SQLDM 10.1 (Barkha Khatri ) 
        /// SQLDM 26392 fix
        /// Global tag feature-- Synchronizing local tags 
        /// </summary>
        /// <param name="syncTags"></param>
        private void SyncLocalTags(ICollection<Tag> syncTags)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            TagCollection syncLocalTags = new TagCollection();
            foreach (Tag tag in syncTags)
            {
                if (tag.IsGlobalTag == false)
                {
                    syncLocalTags.Add(tag);
                }
            }
            // this method must be called on the UI thread.
            if (syncTags != null)
            {
                // If no tags in the list
                if (syncTags.Count == 0)
                {
                    localTags.Clear();
                }
                else
                {
                    IDictionary<int, Tag> diffTags = localTags.GetDictionary();

                    if (diffTags != null)
                    {
                        List<Tag> newTags = new List<Tag>();
                        List<Tag> updateTags = new List<Tag>();

                        foreach (Tag tag in syncLocalTags)
                        {
                            if (localTags.Contains(tag.Id))
                            {
                                updateTags.Add(tag);
                                diffTags.Remove(tag.Id);
                            }
                            else
                            {
                                newTags.Add(tag);
                            }
                        }

                        LOG.Info(string.Format("Synchronizing Local tags- Removing {0} tags.", diffTags.Count));
                        localTags.RemoveRange(diffTags.Values);

                        LOG.Info(string.Format("Synchronizing Local tags- Updating {0} tags.", updateTags.Count));
                        localTags.UpdateRange(updateTags);

                        LOG.Info(string.Format("Synchronizing Local tags- Adding {0} tags.", newTags.Count));
                        localTags.AddRange(newTags);
                    }
                    else
                    {
                        LOG.Info(string.Format("Synchronizing Local tags- Adding {0} instances.", syncTags.Count));
                        localTags.AddRange(syncLocalTags);
                    }
                }
            }
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by SyncLocalTags : {0}", stopWatch.ElapsedMilliseconds);
        }

        private MonitoredSqlServerWrapper InitializeInstanceWrapper(MonitoredSqlServer instance)
        {
            MonitoredSqlServerWrapper wrapper = new MonitoredSqlServerWrapper(instance);
            MonitoredSqlServerStatus status = GetInstanceStatus(instance.Id,instance.RepoId);
            if (status != null)
            {
                status.Instance = wrapper;
            }

            //Add the Alert configuration data.
            AlertConfiguration config = RepositoryHelper.GetAlertConfiguration(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                                                wrapper.AlertConfiguration,
                                                                                false,
                                                                                null);

            foreach (AlertConfigurationItem item in config.ItemList)
            {
                wrapper.AlertConfiguration.AddEntry(item);
            }
            return wrapper;
        }

        public void BackgroundThreadRefresh()
        {
			/* ORIGINAL
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
			*/
            LOG.Debug("BackgroundThreadRefresh - begin.....");
            lock (defaultApplicationModel)
            {
                RefreshUserToken();              // The refresh order is important
                RefreshMonitoredSqlServerStatus();
                RefreshMetricMetaData();
                RefreshTags();
                RefreshCustomReports();
                RefreshPulseConfiguration();
            }
            LOG.Debug("BackgroundThreadRefresh - end.");

        }

        /// <summary>
        /// Refresh the permissions assigned to the connected user.
        /// </summary>
        public void RefreshUserToken()
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
                StartUpTimeLog.DebugFormat("Time taken by RefreshUserToken : {0} ",
                               stopWatch.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Refresh user settings with persisted values.
        /// </summary>
        public void RefershUserSessionSettings()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                if (Settings.Default.ActiveRepositoryConnection != null && Settings.Default.ActiveRepositoryConnection.ConnectionInfo != null &&
                    Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser != null)
                {
                    LOG.Info("Loading history settings for user - " + Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                    HistoryTimeValue deserializedValue = null;
                    string userSettingValues = RepositoryHelper.GetUserSessionSettings(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                    if (userSettingValues != null)
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(Common.Objects.HistoryTimeValue));
                        StringReader rdr = new StringReader(userSettingValues);
                        deserializedValue = (HistoryTimeValue) serializer.Deserialize(rdr);
                    }
                    // [SQLDM-27451] (Anshul Aggarwal) - History range settings are not persisted when switched between multiple users without closing DM console
                    HistoryTimeValue.Refresh(deserializedValue);
                }
                else
                {
                    LOG.Info("Failed to load User History settings.");
                }
            }
            catch (Exception e)
            {
                LOG.Error("Error in Initialization. Error during fetching User History settings", e);
            }
            finally
            {
                stopWatch.Stop();
                StartUpTimeLog.DebugFormat("RefershUserSessionSettings took {0} milliseconds.",
                               stopWatch.ElapsedMilliseconds);
            }
        }

        public void RefreshMetricMetaData()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                metricDefinitions.Reload(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);
            }
            catch (Exception e)
            {
                LOG.Error("Error updating cache of metric metadata from the repository. ", e);
            }
            finally
            {
                stopWatch.Stop();
                StartUpTimeLog.DebugFormat("RefreshMetricMetaData took {0} milliseconds.",
                               stopWatch.ElapsedMilliseconds);
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
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
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

                    Dictionary<string, CustomReport> deleteReports = new Dictionary<string, CustomReport>();

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
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by SyncCustomReports : {0}", stopWatch.ElapsedMilliseconds);
        }

        public void RefreshCustomReports()
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
                StartUpTimeLog.InfoFormat("Time taken by RefreshCustomReports : {0} ", stopWatch.ElapsedMilliseconds);
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

                if (customReports.ContainsKey(customReport.Name))
                {
                    reportID = defaultManagementService.UpdateCustomReport(CustomReport.Operation.Update,
                        null, customReport.Name, customReport.ShortDescription,
                        new Serialized<string>(customReport.ReportRDL), customReport.ShowTopServers);

                    InvokeAddOrUpdateCustomReport(customReport);
                }
                else
                {
                    defaultManagementService.UpdateCustomReport(CustomReport.Operation.New, null,
                        customReport.Name, "",
                        new Serialized<string>(customReport.ReportRDL), customReport.ShowTopServers);

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
                        null, customReportToRemove, null, null, false);
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
                            null, report.Name, null, null, false);
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
            var stopWatch = new Stopwatch();
            stopWatch.Start();
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
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by RefreshPulseConfiguration : {0}", stopWatch.ElapsedMilliseconds);
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

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                ICollection<Tag> syncTags =
                      RepositoryHelper.GetTags(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);


                //Start: SQLdm 10.1 - Praveen Suhalka - CWF3 integration
                //SQLDM 10.1 (Barkha Khatri)
                //SQLDM-26495 fix 
                try
                {
                    AddGlobalTags(syncTags);
                }
                catch (Exception ex)
                {
                    LOG.Error("Error while getting global tags " + ex.Message);
                }

                Stopwatch stopWatch1 = new Stopwatch();
                stopWatch1.Start();
                // Filter tags based on the current user token permissions
                if (!userToken.IsSQLdmAdministrator)
                {
                    foreach (Tag tag in syncTags)
                    {
                        tag.FilterInstances(userToken.ActiveAssignedServerIds);
                    }
                }
                stopWatch1.Stop();
                StartUpTimeLog.DebugFormat("Time taken by FilterInstances number of tags times : {0}", stopWatch1.ElapsedMilliseconds);
                InvokeSyncTags(syncTags);
            }
            catch (Exception e)
            {
                LOG.Error("Error updating tags from the repository.", e);
            }
            finally
            {
                stopWatch.Stop();
                StartUpTimeLog.InfoFormat("RefreshTags took : {0} ", stopWatch.ElapsedMilliseconds);
            }
        }

        public void RefreshLocalTags()
        {
            using (LOG.DebugCall("RefreshLocalTags"))
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

                    InvokeSyncLocalTags(syncTags);
                }
                catch (Exception e)
                {
                    LOG.Error("Error updating tags from the repository.", e);
                }
                finally
                {
                    stopWatch.Stop();
                    LOG.InfoFormat("RefreshLocalTags took {0} milliseconds.", stopWatch.ElapsedMilliseconds);
                }
            }
        }

        /// <summary>
        /// //SQLdm 10.1 - (Praveen Suhalka) - CWF3 integration
        /// </summary>
        /// <param name="syncTags"></param>
        private void AddGlobalTags(ICollection<Tag> syncTags)
        {
            Stopwatch stopWatchMain = new Stopwatch();
            stopWatchMain.Start();
            int lastTagID = 0;
            //foreach (Tag t in syncTags)
            //{
            //    if (lastTagID < t.Id)
            //    {
            //        lastTagID = t.Id;
            //    }
            //}
            //SQLDM 10.1(Barkha Khatri) SQLDM-26392 fix---- using negative ids for global tags
            ICollection<Common.CWFDataContracts.GlobalTag> globalTags;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            IManagementService defaultManagementService =
                         ManagementServiceHelper.GetDefaultService(
                             Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            globalTags = null;//defaultManagementService.GetGlobalTags();
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by ManagementService.GetGlobalTags : {0} ", stopWatch.ElapsedMilliseconds);

            stopWatch.Reset();
            stopWatch.Start();
            if (globalTags != null && globalTags.Count > 0)
            {
                foreach (Common.CWFDataContracts.GlobalTag gt in globalTags)
                {
                    List<int> serverIds = null;
                    if (gt.Instances != null && gt.Instances.Count > 0)
                    {
                        serverIds = new List<int>();
                        foreach (string instanceName in gt.Instances)
                        {
                            int serverID = GetInstanceIDFromName(instanceName);
                            if (serverID != -1)
                            {
                                serverIds.Add(serverID);
                            }
                        }
                    }

                    Tag tag = new Tag(--lastTagID, gt.Name, serverIds, null, null);
                    tag.IsGlobalTag = true;

                    syncTags.Add(tag);
                }
            }
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken for adding global tags in synTags object : {0}", stopWatch.ElapsedMilliseconds);
            stopWatchMain.Stop();
            StartUpTimeLog.DebugFormat("Time taken by AddGlobalTags : {0}", stopWatchMain.ElapsedMilliseconds);
        }


        /// <summary>
        /// //SQLdm 10.1 - (Praveen Suhalka) - CWF3 Integration
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int GetInstanceIDFromName(string name)
        {
            foreach (MonitoredSqlServer i in ActiveInstances)
            {
                if (string.Compare(i.InstanceName, name, true) == 0)
                {
                    return i.Id;
                }
            }
            return -1;
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
        //SQLDM 10.1 (Barkha Khatri)
        //SQLD 26392 fix
        private void SyncAddOrUpdateTag(Tag tag)
        {
            Tags.AddOrUpdate(tag);
            LocalTags.AddOrUpdate(tag);
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
        //SQLDM 10.1 (Barkha Khatri)
        //SQLD 26392 fix
        private void SyncRemoveTags(ICollection<Tag> tagsToRemove)
        {
            LocalTags.RemoveRange(tagsToRemove);
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
            foreach (var conn in Settings.Default.RepositoryConnections)
            {
                bool performActiveInstanceRefresh = false;

                Stopwatch stopWatchMain = new Stopwatch();
                stopWatchMain.Start();

                try
                {
                    XmlDocument document = null;
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    try
                    {

                        LOG.Verbose("Getting status document from the management service");
                        IManagementService managementService =
                            ManagementServiceHelper.GetDefaultService(
                               conn.ConnectionInfo);
                        if (managementService != null)
                        {
                            string xml;
                            Stopwatch stopWatch1 = new Stopwatch();
                            stopWatch1.Start();
                            xml = managementService.GetMonitoredSQLServerStatusDocument();
                            stopWatch1.Stop();
                            StartUpTimeLog.DebugFormat("Time taken by managementService.GetMonitoredSQLServerStatusDocument() : {0}", stopWatch1.ElapsedMilliseconds);
                            if (xml != null)
                            {
                                XmlDocument doc = new XmlDocument();
                                doc.LoadXml(xml);
                                document = doc;
                            }
                            LOG.Verbose("Got cached status document from the management service");
                        }
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Error getting status document from the management service.  ", e);
                    }
                    stopWatch.Stop();
                    StartUpTimeLog.DebugFormat("Time taken for getting status document from management service took : {0}", stopWatch.ElapsedMilliseconds);



                    // if we failed to get a valid xml document from the management service get it from the database
                    if (document == null)
                    {
                        stopWatch.Reset();
                        stopWatch.Start();
                        SqlConnectionInfo connectionInfo = conn.ConnectionInfo;
                        LOG.VerboseFormat("Getting status document from the repository (connection info: {0})", connectionInfo);
                        document = RepositoryHelper.GetMonitoredSqlServerStatus(connectionInfo, null);
                        stopWatch.Stop();
                        StartUpTimeLog.DebugFormat("Time taken for getting status document from repository : {0}", stopWatch.ElapsedMilliseconds);
                    }
                    lock (activeInstanceStatus)
                    {
                        stopWatch.Reset();
                        stopWatch.Start();
                        IDictionary<int, MonitoredSqlServerWrapper> diffInstances = RepoActiveInstances.ContainsKey(conn.RepositiryId)? RepoActiveInstances[conn.RepositiryId].GetDictionary():null;
                        IDictionary<int, MonitoredSqlServerStatus> diffStatus = activeInstanceStatus.ContainsKey(conn.RepositiryId) ? new Dictionary<int, MonitoredSqlServerStatus>(activeInstanceStatus[conn.RepositiryId]):new Dictionary<int, MonitoredSqlServerStatus>();
                        Dictionary<int, MonitoredSqlServerStatus> currentactiveInstantStatus = new Dictionary<int, MonitoredSqlServerStatus>();
                         MonitoredSqlServerStatus status = null;
                        if(!activeInstanceStatus.Keys.Contains(conn.RepositiryId))
                        {
                            activeInstanceStatus.Add(conn.RepositiryId, currentactiveInstantStatus);
                        }
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
                                            if (activeInstanceStatus[conn.RepositiryId].TryGetValue(id, out status))
                                            {   // existing node - update it
                                                bool beforeHasCustomCounters = status.CustomCounterCount > 0;
                                                ServerVersion beforeServerVersion = status.InstanceVersion;

                                                status.Update(node);
                                                diffStatus.Remove(id);

                                                if (RepoActiveInstances.ContainsKey(conn.RepositiryId) && RepoActiveInstances[conn.RepositiryId].Contains(id))
                                                {
                                                    // see if maintenance mode changed and update monitored server object
                                                    MonitoredSqlServerWrapper wrapper = RepoActiveInstances[conn.RepositiryId][id];

                                                    if (wrapper.MaintenanceModeEnabled != status.IsInMaintenanceMode)
                                                    {
                                                        wrapper.MaintenanceModeEnabled = status.IsInMaintenanceMode;
                                                    }
                                                    else
                                                    if (beforeHasCustomCounters != (status.CustomCounterCount > 0))
                                                    {
                                                        wrapper.FireChanged();
                                                    }
                                                    else
                                                    if (status.InstanceVersion != null)
                                                    {
                                                        if (beforeServerVersion == null || beforeServerVersion.Major != status.InstanceVersion.Major)
                                                            wrapper.FireChanged();
                                                    }
                                                }
                                                else
                                                {
                                                    status = new MonitoredSqlServerStatus(node);
                                                    currentactiveInstantStatus.Add(id, status);
                                                }
                                            }
                                            else
                                            {
                                                status = new MonitoredSqlServerStatus(node);
                                                currentactiveInstantStatus.Add(id, status);
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
                            activeInstanceStatus[Settings.Default.RepoId].Remove(pair.Key);
                        }

                        // if able to refresh active instances see if instanced were deleted
                        if (refreshActiveInstances && !performActiveInstanceRefresh)
                        {
                            performActiveInstanceRefresh = diffInstances != null ? diffInstances.Count > 0 : false;
                        }
                        stopWatch.Stop();
                        StartUpTimeLog.DebugFormat("Time taken for Parsing status document : {0}", stopWatch.ElapsedMilliseconds);
                    }

                    performActiveInstanceRefresh = false;
                    // RefreshActiveInstanceStatus must be called without a lock on activeInstanceStatus or a deadlock will occur
                    performActiveInstanceRefresh = false;  // SQLDM-29715 - this solved the problem for USAA.
                    if (performActiveInstanceRefresh)
                    {
                        stopWatch.Reset();
                        stopWatch.Start();
                        RefreshActiveInstances(false);
                        stopWatch.Stop();
                        StartUpTimeLog.DebugFormat("Time taken by RefreshActiveInstances(false) : {0}", stopWatch.ElapsedMilliseconds);
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("An error occurred while refreshing monitored sql server status list.", e);
                }
                finally
                {
                    stopWatchMain.Stop();
                    StartUpTimeLog.DebugFormat("RefreshMonitoredSqlServerStatus took : {0} ",
                                   stopWatchMain.ElapsedMilliseconds);
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
                    var connInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
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
                            addedServer.RepoId = Settings.Default.CurrentRepoId;
                            addedServer.ClusterRepoId= RepositoryHelper.AddUpdateClusterMonitorServer(addedServer.Id,addedServer.InstanceName);
                            addedServerWrappers.Add(InitializeInstanceWrapper(addedServer));
                        }

                        // Add the server in the user token.   Note when
                        // background refresh happens the user token will be synced, 
                        // but this is a short term update until the background refresh
                        // happens.
                        foreach (MonitoredSqlServerWrapper instance in addedServerWrappers)
                        {
                            userToken.AddServer(instance.Id, instance.InstanceName);

                            AllInstances.Remove(instance.Id);
                            AllInstances.Add(instance.Id, instance.Instance);
                            AllRegisteredInstances.Remove(instance.Id);
                            AllRegisteredInstances.Add(instance.Id, instance.Instance);
							RepositoryHelper.AddCustomCounters(instance.Id);	
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
            ActiveInstances.AddRange(instances);
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
            ActiveInstances.RemoveRange(instances);
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
                        AllInstances.Remove(instance.Id);
                        AllRegisteredInstances.Remove(instance.Id);
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
                ActiveInstances.AddOrUpdate(server);
            }
            RefreshTags();

            return servers;
        }

        public MonitoredSqlServer UpdateMonitoredSqlServer(int id, MonitoredSqlServerConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            IManagementService defaultManagementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            MonitoredSqlServer server = defaultManagementService.UpdateMonitoredSqlServer(id, configuration);
            server.RepoId = Settings.Default.CurrentRepoId;
            server.ClusterRepoId= RepositoryHelper.GetClusterRepoServerID(server.Id, server.RepoId);
            ActiveInstances.AddOrUpdate(server);
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

            foreach (MonitoredSqlServerWrapper wrapper in ActiveInstances)
            {
                MonitoredSqlServer server = wrapper;
                if (selectedInstances.Contains(wrapper.Id))
                {
                    if (!server.CustomCounters.Contains(metricID))
                    {
                        server.CustomCounters.Add(metricID);
                        affectedServers.Add(server.Id);
                    }
                }
                else
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
                MonitoredSqlServerWrapper wrapper = ActiveInstances[id];
                if (wrapper != null)
                    wrapper.FireChanged();
            }
        }

        private void FireInstanceChangedEvent(IList<MonitoredSqlServer> changedServers)
        {
            foreach (MonitoredSqlServer server in changedServers)
            {
                ActiveInstances.AddOrUpdate(server);
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
            catch (Exception e)
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