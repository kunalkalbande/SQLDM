using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.Common.Configuration;
using Wintellect.PowerCollections;
using Idera.SQLdm.DesktopClient.Controls;
using System.Text.RegularExpressions;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.UI.Dialogs;
using Microsoft.SqlServer.MessageBox;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.Common.Configuration.ServerActions;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Resources
{
    internal enum FileActivitySort { Databasename, Filename, Reads, Writes }
    internal enum FileActivitySortDirection { Up, Down }

    class OSCollectionStateInfo
    {
        private bool collectionSucceeded;
        private string message;
        private bool showDialog;
        private OSMetricsStatus osMetricsStatus;

        /// <summary>
        /// Returns true if the osMetricsStatus = Available
        /// </summary>
        public bool CollectionSucceeded
        {
            get { return collectionSucceeded; }
            set { collectionSucceeded = value; }
        }

        /// <summary>
        /// Returns the Message that will be displayed whenever a collection issue occured
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        /// <summary>
        /// Returns true if the Popup message can be displayed
        /// </summary>
        public bool ShowDialog
        {
            get { return showDialog; }
            set { showDialog = value; }
        }

        public OSCollectionStateInfo()
        {
            SetOsMetricsStatus(OSMetricsStatus.Available);
        }

        public OSCollectionStateInfo(OSMetricsStatus osMetricsStatus)
        {
            SetOsMetricsStatus(osMetricsStatus);
        }

        /// <summary>
        /// If the popup checkbox was checked, means that the user does not want to see it again, so we need to persist it.
        /// </summary>
        /// <param name="boxChecked"></param>
        public void SetMessageVisibility(bool boxChecked)
        {
            switch (osMetricsStatus)
            {
                case OSMetricsStatus.Disabled:
                    Settings.Default.ShowMessage_OLEAutomationDisabled = !boxChecked;
                    break;
                case OSMetricsStatus.WMIServiceTimedOut:
                    Settings.Default.ShowMessage_WMIServiceTimedout = !boxChecked;
                    break;
                case OSMetricsStatus.WMIServiceUnreachable:
                    Settings.Default.ShowMessage_WMIServiceUnreachable = !boxChecked;
                    break;
                case OSMetricsStatus.OLEAutomationUnavailable:
                    Settings.Default.ShowMessage_OLEAutomationUnavailable = !boxChecked;
                    break;
            }
        }

        /// <summary>
        /// Sets the OSMetricsStatus and initializes all values
        /// </summary>
        /// <param name="osMetricsStatus"></param>
        public void SetOsMetricsStatus(OSMetricsStatus osMetricsStatus)
        {
            this.osMetricsStatus = osMetricsStatus;
            collectionSucceeded = false;
            switch (osMetricsStatus)
            {
                case OSMetricsStatus.WMIServiceTimedOut:
                    message = "WMI service has timed out.";
                    showDialog = Settings.Default.ShowMessage_WMIServiceTimedout;
                    break;
                case OSMetricsStatus.OLEAutomationUnavailable:
                    message = "WMI must be enabled to collect Operating System data.";
                    showDialog = Settings.Default.ShowMessage_OLEAutomationUnavailable;
                    break;
                case OSMetricsStatus.WMIServiceUnreachable:
                    message = "WMI service is unreachable.";
                    showDialog = Settings.Default.ShowMessage_WMIServiceUnreachable;
                    break;
                case OSMetricsStatus.Disabled:
                    message = "Operating System metrics collection is disabled.";
                    showDialog = Settings.Default.ShowMessage_OLEAutomationDisabled;
                    break;
                default:
                    message = "";
                    showDialog = false;
                    collectionSucceeded = true;
                    break;
            }
        }
    }

    internal partial class ResourcesFileActivity : ServerBaseView
    {
		///Ankit Nagpal --Sqldm10.0.0
        #region Constants
        private const string SYSADMIN_MESSAGE = @"Sysadmin privileges are required to obtain data for this view.";
        #endregion

        #region Fields

        private static readonly object updateLock = new object();        

        private DateTime? currentHistoricalSnapshotDateTime = null;
        private Exception historyModeLoadError              = null;
        private bool      realtimeIsBackfilled              = false;
        private bool      realtimeBackFillIsRendered        = true;
        private bool      filterControlsUpdating            = false;
        private bool      oleDisabled                       = false;
        private bool      olePending                        = false;
        private bool      isFilterInitialized               = false;
		///Ankit Nagpal --Sqldm10.0.0
        //private bool      isUserSysAdmin                          = false;

        private FileActivityFilter   filter;
        private FileActivitySort sort;
        private FileActivitySortDirection   sortDirection;
        private FileActivitySnapshot        currentSnapshot;
        private FileActivitySnapshot        previousSnapshot;
        private DataTable                   currentDataTable;
        private Dictionary<string, Set<string>>  dbToDiskMap;
        private Dictionary<string, Set<string>>  diskToDbMap;
        private OSCollectionStateInfo           osCollectionState;
        
        private Random random = new Random();
        public event EventHandler FilterPaneVisibleChanged;
        private const string OperationalStatusMessage = "       {0}";
        private bool displayWMIErrorDialog = true;
        private int? cloudProviderId;     //SQLDM 10.3 (Manali H): Fix for SQLDM-28754 

        public bool FilterPaneVisible
        {
            get { return filterPanel.Visible; }
            set { UpdateFilterPanel(value); }
        }

        #endregion

        public ResourcesFileActivity(int instanceId) : base(instanceId)
        {
            cloudProviderId = ApplicationModel.Default.GetInstanceStatus(instanceId).Instance.Instance.CloudProviderId; //SQLDM 10.3 (Manali H): Fix for SQLDM-28754 
            filter              = new FileActivityFilter();
            sort                = FileActivitySort.Filename;
            sortDirection       = FileActivitySortDirection.Down;
            dbToDiskMap         = new Dictionary<string, Set<string>>();
            diskToDbMap         = new Dictionary<string, Set<string>>();
            if (cloudProviderId != Idera.SQLdm.Common.Constants.LinuxId) //SQLDM 10.3 (Manali H): Fix for SQLDM-28754
                osCollectionState   = new OSCollectionStateInfo();
			
            InitializeComponent();

            this.AdaptFontSize();

            fileActivityPanel1.InstanceId = instanceId;

            ApplicationModel.Default.ActiveInstances[instanceId].Changed += ResourcesFileActivity_Changed;

            msgPanel.Dock = DockStyle.Fill;
        }

        void ResourcesFileActivity_Changed(object sender, Objects.MonitoredSqlServerChangedEventArgs e)
        {

        }


        #region Properties

        public override DateTime? HistoricalSnapshotDateTime
        {
            get
            {
                return currentHistoricalSnapshotDateTime;
            }

            set
            {
                currentHistoricalSnapshotDateTime = value;
            }
        }

        #endregion

        private void ResourcesFileActivity_Load(object sender, EventArgs e)
        {
            msgPanel.BringToFront();
        }

        #region Overrides

        public override void ShowHelp()
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.ResourcesFileActivityView);
        }

        public override void RefreshView()
        {
                historyModeLoadError = null;
                base.RefreshView();
        }

        public override object DoRefreshWork(BackgroundWorker backgroundWorker)
        {
            using (Log.InfoCall("DoRefreshWork"))
            {
                DateTime? historyDateTime = HistoricalSnapshotDateTime;

                if (historyDateTime == null)
                {
                    Log.Info("Getting real-time snapshot");
                    return GetRealTimeSnapshot();
                }
                else
                {
                    Log.InfoFormat("Populating historical snapshots (end={0}).", historyDateTime.Value);
                    return GetHistoricalSnapshot();
                }
            }
        }

        public override void UpdateData(object data)
        {
            lock (updateLock)
            {
                if (HistoricalSnapshotDateTime == null)
                    UpdateDataFromRealtimeSnapshot(data);
                else
                    UpdateDataFromHistoricalSnapshot(data);

                UpdateMaps();
                UpdateFilterControls();
                UpdateControls();

                if(currentSnapshot != null && currentSnapshot.Error == null)
                    ApplicationController.Default.OnRefreshActiveViewCompleted(new RefreshActiveViewCompletedEventArgs(DateTime.Now));
            }
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            if (olePending)
            {
                ShowOperationalStatus("Failed to enable OLE Automation.", Properties.Resources.StatusWarningSmall);
                olePending = false;
            }

            base.HandleBackgroundWorkerError(e);
        }

        #endregion

        #region Real time updates

        private object GetRealTimeSnapshot()
        {            
            if (!realtimeIsBackfilled)
            {
                lock (updateLock)
                {
                    currentDataTable = (DataTable)GetHistoricalSnapshot(DateTime.Now);
                    realtimeIsBackfilled = true;
                    realtimeBackFillIsRendered = false;
                }
            }

            // get snapshot
            IManagementService   managementService = ManagementServiceHelper.GetDefaultService();
            FileActivitySnapshot snapshot          = managementService.GetFileActivity(new FileActivityConfiguration(instanceId, previousSnapshot));

            previousSnapshot = currentSnapshot;
            currentSnapshot  = snapshot;            

            // check for OLE
            if (snapshot != null && snapshot.Error == null && cloudProviderId != Idera.SQLdm.Common.Constants.LinuxId) //SQLDM 10.3 (Manali H) : fix for SQLDM-28754 
                CheckForOLEAutomation(snapshot);

            if (olePending)
            {
                managementService.SendReconfiguration(new ReconfigurationConfiguration(instanceId, "Ole Automation Procedures", 1));
                olePending = false;
            }

            return snapshot;
        }

        private void UpdateDataFromRealtimeSnapshot(object data)
        {            
            if (data == null)
                return;

            FileActivitySnapshot snapshot = (FileActivitySnapshot)data;

            if (snapshot == null)
            {
                Log.Debug("UpdateDataFromRealtimeSnapshot returned null for instance " + instanceId);
                return;
            }

            if (snapshot.Error != null)
            {
				///Ankit Nagpal --Sqldm10.0.0
				///If not a sysadmin display sysadmin message
                if(!isUserSysAdmin)
                    ShowMessage(SYSADMIN_MESSAGE);
                else
                    ShowMessage("Error loading data. See log for details.");
                Log.Error("UpdateDataFromRealtimeSnapshot encountered an error while obtaining file activity data.", snapshot.Error);
                ApplicationController.Default.OnRefreshActiveViewCompleted(new RefreshActiveViewCompletedEventArgs(DateTime.Now, snapshot.Error));
                return;
            }            
        }

        #endregion

        #region Historical updates

        private object GetHistoricalSnapshot()
        {
            return GetHistoricalSnapshot(HistoricalSnapshotDateTime.Value);
        }

        private object GetHistoricalSnapshot(DateTime dateTimeCutoff)
        {
            DataTable data = RepositoryHelper.GetFileActivity(this.instanceId, dateTimeCutoff, ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);

            return data;
        }

        private void UpdateDataFromHistoricalSnapshot(object dataobj)
        {
            if (HistoricalSnapshotDateTime == null)
                return;

            ShowOperationalStatus(Properties.Resources.HistoryModeOperationalStatusLabel, Properties.Resources.StatusWarningSmall);
            currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
            DataTable data = dataobj as DataTable;

            if (data == null)
            {
                Log.Debug("UpdateDataFromHistoricalSnapshot received null data for instance " + instanceId);
                return;
            }

            // pass data to the file activity control
            currentDataTable = data;          
        }        

        #endregion                        
        
        private void UpdateMaps()
        {
            if (currentSnapshot == null)
                return;

            dbToDiskMap.Clear();
            diskToDbMap.Clear();

            string diskname = "";
            string dbname   = "";

            foreach (FileActivityDisk disk in currentSnapshot.Drives.Values)
            {
                diskname = disk.DriveLetter;

                if (!diskToDbMap.ContainsKey(diskname))
                    diskToDbMap.Add(diskname, new Set<string>());

                foreach (FileActivityFile file in disk.Files.Values)
                {
                    dbname = file.DatabaseName;

                    // db name -> disk name mapping
                    if (!dbToDiskMap.ContainsKey(dbname))
                        dbToDiskMap.Add(dbname, new Set<string>());

                    if (!dbToDiskMap[dbname].Contains(diskname))
                        dbToDiskMap[dbname].Add(diskname);
                    
                    // disk name -> db name mapping
                    if (!diskToDbMap[diskname].Contains(dbname))
                        diskToDbMap[diskname].Add(dbname);
                }
            }
        }

        private bool IsDbOnlyOnDisks(string dbname, List<string> disknames)
        {
            if (!dbToDiskMap.ContainsKey(dbname))
                return false;

            Set<string> disksDbIsOn = dbToDiskMap[dbname];

            IEnumerator<string> e = disksDbIsOn.GetEnumerator();

            while (e.MoveNext())
            {
                if (!disknames.Contains(e.Current))
                    return false;
            }

            return true;
        }

        private bool IsDbOnDisk2(string diskname, string[] dbnames)
        {
            for (int i = 0; i < dbnames.Length; i++)
            {
                if (diskToDbMap.ContainsKey(diskname) && diskToDbMap[diskname].Contains(dbnames[i]))
                    return true;
            }

            return false;
        }

        private delegate void OleCheckDelegate(FileActivitySnapshot snapshot);
        private void CheckForOLEAutomation(FileActivitySnapshot snapshot)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new OleCheckDelegate(CheckForOLEAutomation), snapshot);
                return;
            }

            if (olePending || snapshot == null)
                return;

            oleDisabled = false;

            osCollectionState.SetOsMetricsStatus(snapshot.OsMetricsStatus);

            // Hide message for Resources > File Activity (Managed Instance View)
            var isAzureManagedInstance = ApplicationModel.Default.AllInstances.ContainsKey(instanceId) &&
                                         ApplicationModel.Default.AllInstances[instanceId].CloudProviderId ==
                                         Common.Constants.MicrosoftAzureManagedInstanceId;
            var isAwsInstance = ApplicationModel.Default.AllInstances.ContainsKey(instanceId) &&
                                   ApplicationModel.Default.AllInstances[instanceId].CloudProviderId ==
                                   Common.Constants.AmazonRDSId;
            if (isAzureManagedInstance || osCollectionState.CollectionSucceeded || isAwsInstance) return;

            ApplicationMessageBox box = new ApplicationMessageBox();
            DialogResult result = DialogResult.Cancel;
            box.Symbol       = ExceptionMessageBoxSymbol.Information;
            box.Buttons      = ExceptionMessageBoxButtons.OK;
            box.ShowCheckBox = true;
            box.CheckBoxText = "Don't show this message again.";
            box.Caption = "Operating System metrics collection is experiencing some difficulties.";

            string opText = "";
            oleDisabled = true;
                
            string boxTextSuggestion = "Would you like to change the Operating System metrics collection settings?";
            string opTextSuggestion = "Click here to change the Operating System metrics collection settings.";
            string infoText = "Click here for more information.";
            string viewUserIssueText =
                "Operating System metrics collection must be available to collect operating system statistics.";
                
            if (ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify && snapshot.ProductVersion != null && snapshot.ProductVersion.Major >= 9) // SQL Server 2005/2008
            {
                opText = string.Format("{0} {1}", osCollectionState.Message, opTextSuggestion);
                box.Text = string.Format("{0} {1}", osCollectionState.Message, boxTextSuggestion);
                box.Buttons = ExceptionMessageBoxButtons.YesNo;
                box.Symbol  = ExceptionMessageBoxSymbol.Question;
            }
            else
            {
                opText = string.Format("{0} {1}", osCollectionState.Message, infoText);
                box.Text = string.Format("{0} {1}", osCollectionState.Message, viewUserIssueText);
            }

            if (!operationalStatusPanel.Visible && osCollectionState.ShowDialog && displayWMIErrorDialog)
            {
                this.displayWMIErrorDialog = false;
                result = box.Show(this);
                osCollectionState.SetMessageVisibility(box.IsCheckBoxChecked);
            }

            if (result == DialogResult.Yes)
                operationalStatusLabel_MouseUp(null, null);
            else
                ShowOperationalStatus(opText, Properties.Resources.StatusWarningSmall);
        }

        private void UpdateFilterControls()
        {
            if (currentSnapshot == null)
                return;

            var systemdbs = new Set<string>(new string[]{ "master", "model", "msdb", "tempdb" });

            // to prevent checkstate event firings
            filterControlsUpdating = true;

            bool skipDrivesChanges = drivesComboBox.DroppedDown || !drivesComboBox.InnerDropdownClosed;
            bool skipDatabasesChanges = databasesComboBox.DroppedDown || !databasesComboBox.InnerDropdownClosed;

            if (!skipDrivesChanges)
            {
                string[] driveLetters = GetDriveLetters();

                drivesComboBox.Items.Clear();
                drivesComboBox.Items.AddRange(driveLetters);

                int driveLetterWidth = GetAverageItemLength(driveLetters) * 12;

                if (driveLetterWidth < drivesComboBox.Width)
                    drivesComboBox.DropDownWidth = drivesComboBox.Width;
                else
                    drivesComboBox.DropDownWidth = driveLetterWidth;
            }

            if (!skipDatabasesChanges)
            {
                string[] databaseNames = GetDatabaseNames();

                databasesComboBox.Items.Clear();
                databasesComboBox.Items.AddRange(GetDatabaseNames());

                int databaseNamesWidth = GetAverageItemLength(databaseNames) * 12;

                if (databaseNamesWidth < databasesComboBox.Width)
                    databasesComboBox.DropDownWidth = databasesComboBox.Width;
                else
                    databasesComboBox.DropDownWidth = databaseNamesWidth;
            }

            if (filter.IsSetByUser)
            {
                if (!skipDrivesChanges)
                {
                    for (int i = 0; i < drivesComboBox.Items.Count; i++)
                    {
                        if (filter.Drives.Contains(drivesComboBox.Items[i] as string))
                            drivesComboBox.SetItemChecked(i, true);
                        else
                            drivesComboBox.SetItemChecked(i, false);
                    }
                }

                if (!skipDatabasesChanges)
                {
                    for (int i = 0; i < databasesComboBox.Items.Count; i++)
                    {
                        databasesComboBox.SetItemChecked(i, false);
                        if (filter.Databases.Contains(databasesComboBox.Items[i] as string))
                        {
                            bool check = true;

                            if (!isFilterInitialized)
                                check = !systemdbs.Contains((string) databasesComboBox.Items[i]);

                            databasesComboBox.SetItemChecked(i, check);
                        }
                    }

                    for (int i = 0; i < databasesComboBox.Items.Count; i++)
                    {
                        if (filter.Databases.Contains(databasesComboBox.Items[i] as string))
                            databasesComboBox.SetItemChecked(i, true);
                        else
                            databasesComboBox.SetItemChecked(i, false);
                    }
                }

                cbFiletypeData.Checked  = filter.Filetypes.Contains("Data");
                cbFiletypeLog.Checked   = filter.Filetypes.Contains("Log");
                cbFiletypeOther.Checked = filter.IncludeOthers;

                filenameLike.Text = filter.Filename;
                filepathLike.Text = filter.Filepath;
            }
            else
            {
                if (!skipDrivesChanges)
                {
                    for (int i = 0; i < drivesComboBox.Items.Count; i++)
                        drivesComboBox.SetItemChecked(i, true);
                }

                if (!skipDatabasesChanges)
                {

                    for (int i = 0; i < databasesComboBox.Items.Count; i++)
                    {
                        if (string.IsNullOrEmpty(argDb))
                        {
                            bool check = true;

                            if (!isFilterInitialized)
                                check = !systemdbs.Contains((string) databasesComboBox.Items[i]);

                            databasesComboBox.SetItemChecked(i, check);
                        }
                        else
                            databasesComboBox.SetItemChecked(i, (string) databasesComboBox.Items[i] == argDb);
                    }
                }

                cbFiletypeData.Checked  = true;
                cbFiletypeLog.Checked   = true;
                cbFiletypeOther.Checked = true;

                filenameLike.Text = string.Empty;
                filepathLike.Text = string.Empty;

                rbSortByDatabasename.Checked = true;
                rbSortAscending.Checked = true;
            }

            HandleFilterChanged(false, filter.IsSetByUser);

            if (!isFilterInitialized)
                isFilterInitialized = true;
            
            // we are done...
            filterControlsUpdating = false;
        }

        private int GetAverageItemLength(string[] array)
        {
            if (array.Length == 0)
                return 0;

            int sum = 0;

            Array.ForEach(array, new Action<string>(delegate(string s)
            {
                sum += s.Length;
            }));

            return sum / array.Length;
        }

        private string[] GetDriveLetters()
        {
           var driveLetters = new List<string>();

            foreach (string driveLetter in currentSnapshot.Drives.Keys)
            {
                if (currentSnapshot.Drives[driveLetter].Files.Count > 0)
                    driveLetters.Add(driveLetter);
            }

            if (driveLetters.Count == 0)
                return driveLetters.ToArray();

            var instance = ApplicationModel.Default.ActiveInstances[instanceId].Instance;
            if (!instance.DiskCollectionSettings.AutoDiscover)
            {
                if (instance.DiskCollectionSettings.Drives != null && instance.DiskCollectionSettings.Drives.Length > 0)
                {
                    var configured = instance.DiskCollectionSettings.Drives.Select(drive => drive.TrimEnd(':')).ToList();
                    var intersection = Algorithms.SetIntersection(configured, driveLetters, StringComparer.CurrentCultureIgnoreCase);
                    driveLetters = new List<string>(intersection);
                }
            }

            driveLetters.Sort();
            return driveLetters.ToArray();
        }

        private string[] GetDatabaseNames()
        {
            Set<string>  dictnames = new Set<string>();
            List<string> names     = new List<string>();            

            foreach (FileActivityDisk disk in currentSnapshot.Drives.Values)
            {
                foreach (FileActivityFile file in disk.Files.Values)
                {
                    if (!dictnames.Contains(file.DatabaseName))
                    {
                        dictnames.Add(file.DatabaseName);
                        names.Add(file.DatabaseName);
                    }
                }
            }

            names.Sort();

            return names.ToArray();
        }
        
        private void UpdateControls()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(UpdateControls));
                return;
            }

            if (HistoricalSnapshotDateTime == null && !oleDisabled)
            {
                operationalStatusPanel.Visible = false;
                //fileActivityPanel1.Top         = filterPanel.Top + filterPanel.Height + 3;
                //fileActivityPanel1.Height      = this.Height - filterPanel.Height - 3;
            }

            if (currentSnapshot == null)
            {
                ShowMessage("No data available.");
                return;
            }

            if (currentSnapshot.Error != null)
                return;

            HideMessage();

            fileActivityPanel1.SetFilter(filter);

            if (HistoricalSnapshotDateTime != null)
                fileActivityPanel1.SetData(currentDataTable);
            else
            {
                if (!realtimeBackFillIsRendered && currentDataTable != null)
                {
                    fileActivityPanel1.SetData(currentDataTable);
                    realtimeBackFillIsRendered = true;
                }

                fileActivityPanel1.UpdateData(currentSnapshot, Settings.Default.RealTimeChartHistoryLimitInMinutes);
            }

            fileActivityPanel1.Invalidate();
        }        

        private void HideMessage()
        {
            msgPanel.SendToBack();
        }

        private void ShowMessage(string messageText)
        {
            msgLabel.Text = messageText;
            msgPanel.BringToFront();
        }

        #region Operational status

        private delegate void ShowOpStatusDelegate(string message, Image icon);

        private void ShowOperationalStatus(string message, Image icon)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ShowOpStatusDelegate(ShowOperationalStatus), message, icon);
                return;
            }


            //fileActivityPanel1.Top         = 126;// headerStrip1.Top + headerStrip1.Height + 3;
            //fileActivityPanel1.Height      = this.Height - operationalStatusPanel.Height - filterPanel.Height - 3;
            operationalStatusImage.Image   = icon;
            operationalStatusLabel.Text    = string.Format(message, message);
            operationalStatusPanel.Visible = true;            
        }

        private void SwitchToRealTimeMode()
        {
            realtimeIsBackfilled              = false;
            operationalStatusPanel.Visible    = false;
            currentHistoricalSnapshotDateTime = null;
            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }

        private void operationalStatusLabel_MouseEnter(object sender, EventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(255, 189, 105);
            operationalStatusImage.BackColor = Color.FromArgb(255, 189, 105);
        }

        private void operationalStatusLabel_MouseLeave(object sender, EventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(211, 211, 211);
            operationalStatusImage.BackColor = Color.FromArgb(211, 211, 211);
        }

        private void operationalStatusLabel_MouseDown(object sender, MouseEventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.White;
            operationalStatusLabel.BackColor = Color.FromArgb(251, 140, 60);
            operationalStatusImage.BackColor = Color.FromArgb(251, 140, 60);
        }

        private void operationalStatusLabel_MouseUp(object sender, MouseEventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(255, 189, 105);
            operationalStatusImage.BackColor = Color.FromArgb(255, 189, 105);

            if (HistoricalSnapshotDateTime != null)
            {
                SwitchToRealTimeMode();
            }
            else if (currentSnapshot != null && oleDisabled)
            {
                if (olePending)
                    return;

                if(currentSnapshot.OsMetricsStatus == OSMetricsStatus.OLEAutomationUnavailable)
                {
                        if (ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify && currentSnapshot.ProductVersion.Major >= 9) // SQL Server 2005/2008
                        {
                            ShowOperationalStatus("Enabling OLE Automation...", Properties.Resources.StatusWarningSmall);

                            olePending = true;
                            ApplicationController.Default.RefreshActiveView();
                        }
                        else
                        {
                            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.EnablingOsMetricsMonitoring);
                        }
                }
                else if (currentSnapshot.OsMetricsStatus == OSMetricsStatus.Disabled ||
                        currentSnapshot.OsMetricsStatus == OSMetricsStatus.WMIServiceTimedOut ||
                        currentSnapshot.OsMetricsStatus == OSMetricsStatus.WMIServiceUnreachable)
                {
                    if (ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify)
                    {                        
                        using (MonitoredSqlServerInstancePropertiesDialog dlg = new MonitoredSqlServerInstancePropertiesDialog(instanceId))
                        {
                            dlg.SelectedPropertyPage = MonitoredSqlServerInstancePropertiesDialogPropertyPages.OleAutomation;

                            if (dlg.ShowDialog(this.ParentForm) == DialogResult.OK)
                            {
                                if (!dlg.SavedServer.OleAutomationUseDisabled)
                                {
                                    ShowOperationalStatus("Enabling OLE Automation...", Properties.Resources.StatusWarningSmall);
                                    olePending = false;
                                }
                                else if(dlg.SavedServer.WmiConfig.DirectWmiEnabled)
                                {
                                    operationalStatusLabel.Text = string.Format(OperationalStatusMessage, "Enabling Direct WMI...");
                                }

                                ApplicationController.Default.RefreshActiveView();
                            }
                        }
                    }
                    else
                    {
                        Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.EnablingOsMetricsMonitoring);
                    }
                }
            }
        }

        #endregion

        #region Private Types

        private class DoubleBufferedPanel : Panel
        {
            public DoubleBufferedPanel() : base()
            {
                ControlStyles styles = ControlStyles.OptimizedDoubleBuffer;
                GetStyle(styles);
                SetStyle(styles | ControlStyles.OptimizedDoubleBuffer, true);
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.ExStyle |= 0x02000000;
                    return cp; 
                }
            }
        }        

        #endregion        

        private void clearFilterButton_Click(object sender, EventArgs e)
        {
            lock (updateLock)
            {
                // check everything...
                filter.IsSetByUser  = false;
                isFilterInitialized = false;
                argDb = string.Empty;
                UpdateFilterControls();
                UpdateControls();
            }
        }

        private void hideFilterButton_Click(object sender, EventArgs e)
        {
            UpdateFilterPanel(false);
        }

        public void ShowFilterPane()
        {
            UpdateFilterPanel(true);
        }

        private void UpdateFilterPanel(bool isVisible)
        {
            filterPanel.Visible = isVisible;

            if (FilterPaneVisibleChanged != null)
                FilterPaneVisibleChanged(this, EventArgs.Empty);
        }

        private void drivesComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (filterControlsUpdating)
                return;

            lock (updateLock)
            {                
                List<string> uncheckedDrives = new List<string>();

                for (int i = 0; i < drivesComboBox.Items.Count; i++)
                {
                    if (!drivesComboBox.GetItemChecked(i))
                        uncheckedDrives.Add((string)drivesComboBox.Items[i]);
                }

                if (uncheckedDrives.Count > 0)
                {
                    filterControlsUpdating = true; 

                    // uncheck databases that exist ONLY on the unchecked drives
                    for (int i = 0; i < databasesComboBox.Items.Count; i++)
                    {
                        if (IsDbOnlyOnDisks((string)databasesComboBox.Items[i], uncheckedDrives))
                            databasesComboBox.SetItemChecked(i, false);
                    }

                    filterControlsUpdating = false;
                }                

                HandleFilterChanged();
            }            
        }

        private void databasesComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (filterControlsUpdating)
                return;

            argDb = string.Empty;
            HandleFilterChanged();
        }

        private void filenameLike_TextChanged(object sender, EventArgs e)
        {
            if (filterControlsUpdating)
                return;

            filter.Filename = filenameLike.Text;
        }

        private void filepathLike_TextChanged(object sender, EventArgs e)
        {
            if (filterControlsUpdating)
                return;

            filter.Filepath = filepathLike.Text;
        }        

        private void filenameLike_Leave(object sender, EventArgs e)
        {
            HandleFilterChanged();
        }

        private void filenameLike_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                HandleFilterChanged();
        }

        private void filepathLike_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                HandleFilterChanged();
        }

        private void filepathLike_Leave(object sender, EventArgs e)
        {
            HandleFilterChanged();
        }

        private void cbFiletypeData_CheckedChanged(object sender, EventArgs e)
        {
            if (filterControlsUpdating)
                return;
            
            HandleFilterChanged();
        }

        private void cbFiletypeLog_CheckedChanged(object sender, EventArgs e)
        {
            if (filterControlsUpdating)
                return;                       

            HandleFilterChanged();
        }

        private void cbFiletypeOther_CheckedChanged(object sender, EventArgs e)
        {
            if (filterControlsUpdating)
                return;                       

            HandleFilterChanged();
        }

        private void rbSortby_CheckedChanged(object sender, EventArgs e)
        {
            if (filterControlsUpdating)
                return;            

            HandleFilterChanged();
        }

        private void rbSortDirection_CheckedChanged(object sender, EventArgs e)
        {
            if (filterControlsUpdating)
                return;

            HandleFilterChanged();
        }

        private void HandleFilterChanged(bool withUpdate = true, bool setByUser = true)
        {
            lock (updateLock)
            {
                filter.Drives.Clear();
                filter.Databases.Clear();

                for (int i = 0; i < drivesComboBox.CheckedItems.Count; i++)
                    filter.Drives.Add(drivesComboBox.CheckedItems[i] as string);

                for (int i = 0; i < databasesComboBox.CheckedItems.Count; i++)
                    filter.Databases.Add(databasesComboBox.CheckedItems[i] as string);

                filter.Filename = filenameLike.Text;
                filter.Filepath = filepathLike.Text;

                if (cbFiletypeData.Checked)
                    filter.AddFileType("Data");
                else
                    filter.RemoveFileType("Data");

                if (cbFiletypeLog.Checked)
                    filter.AddFileType("Log");
                else
                    filter.RemoveFileType("Log");

                filter.IncludeOthers = cbFiletypeOther.Checked;

                if (rbSortByDatabasename.Checked)
                    sort = FileActivitySort.Databasename;
                else if (rbSortbyFilename.Checked)
                    sort = FileActivitySort.Filename;
                else if (rbSortbyReads.Checked)
                    sort = FileActivitySort.Reads;
                else if (rbSortbyWrites.Checked)
                    sort = FileActivitySort.Writes;                

                if (rbSortAscending.Checked)
                    sortDirection = FileActivitySortDirection.Down;
                else if (rbSortDescending.Checked)
                    sortDirection = FileActivitySortDirection.Up;

                fileActivityPanel1.SortType = sort;
                fileActivityPanel1.SortDirection = sortDirection;

                filter.IsSetByUser = setByUser;

                if(withUpdate)
                    UpdateControls();
            }
        }        

        private void HandleNavigateToDisksView(object sender, EventArgs eventArgs)
        {            
            ApplicationController.Default.ShowServerView(this.instanceId, ServerViews.ResourcesDisk);
        }

        private void HandleNavigateToDatabaseFilesView(object sender, EventArgs eventArgs)
        {
            DatabaseFileActivity senderfile = sender as DatabaseFileActivity;
            if (senderfile.FileInfo == null)
            {
                Log.Debug("HandleNavigateToDatabaseFilesView: can't drill to specific database - FileInfo is null.");
                ApplicationController.Default.ShowServerView(this.instanceId, ServerViews.DatabasesFiles);
            }
            else
                ApplicationController.Default.ShowServerView(this.instanceId, ServerViews.DatabasesFiles, senderfile.FileInfo.DatabaseName);
        }    

        private string argDb = string.Empty;
        public override void SetArgument(object argument)
        {
            string[] args = argument as string[]; // dbname, if length = 1, disk if 2 (repeated)

            argDb = string.Empty;
            if (args != null)
            {
                if (args.Length == 1)
                {
                    filter.IsSetByUser = false;                    
                    argDb = args[0];
                }
                else if (args.Length == 2)
                {
                    // ?
                }
            }

            ApplicationController.Default.ActiveView.CancelRefresh();
            ApplicationController.Default.RefreshActiveView();
        }

        /// <summary>
        /// Autoscale the font size to adapt the Form according the current OS font size configuration.
        /// </summary>
        private void AdaptFontSize()
        {            
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}
