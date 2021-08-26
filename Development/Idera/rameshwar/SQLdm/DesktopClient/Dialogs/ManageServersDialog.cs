using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Microsoft.SqlServer.MessageBox;
using Idera.SQLdm.Common;
using Wintellect.PowerCollections;
using BBS.TracerX;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class ManageServersDialog : Form
    {
        private static readonly Logger Log = Logger.GetLogger("ManageServersDialog");

        public const string LicenseInformationLabelText =
            "SQL Diagnostic Manager is currently licensed to monitor {0} SQL Server instances.";

        public const string InstanceCountLabelText = "{0} instance(s)";
        public const string WindowsAuthenticationModeText = "Windows";
        public const string SqlServerAuthenticationModeText = "SQL Server";

        public const string TestConnectionStatusUnknownTooltipText =
            "Connection status unknown.\r\n\r\nSelect this instance and click the Test button to test the connection configuration.";

        public const string TestConnectionRefreshingTooltipText = "Testing connection...";
        public const string TestConnectionSucceededTooltipText = "Test connection succeeded.";
        public const string TestConnectionFailedTooltipText = "Test connection failed.";
        public const string TestConnectionUnsupportedVersionTooltipText = "The SQL Server version is unsupported.";

        public const int TestConnectionFailedImageIndex = 0;
        public const int TestConnectionWarningImageIndex = 1;
        public const int TestConnectionUnknownImageIndex = 2;
        public const int TestConnectionInProgressImageIndex = 3;
        public const int TestConnectionSuccessImageIndex = 4;

        private IManagementService defaultManagementService = null;
        private InstancesListViewColumnSorter instancesListViewColumnSorter;
        private readonly Dictionary<string, ListViewItem> listViewLookupTable = new Dictionary<string, ListViewItem>();

        private readonly Dictionary<string, ListViewItem> pendingConnectionTests =
            new Dictionary<string, ListViewItem>();

        private readonly List<BackgroundWorker> activeTestConnectionWorkers = new List<BackgroundWorker>();
        private readonly Dictionary<object, ChangeType> pendingChanges = new Dictionary<object, ChangeType>();
        private bool okButtonPressed = false;

        public ManageServersDialog()
        {
            InitializeComponent();
            Icon = Resources.ServersIcon;
            connectionTestStatusImages.Images.Add(Resources.TestConnectionFailed);
            connectionTestStatusImages.Images.Add(Resources.StatusWarningSmall);
            connectionTestStatusImages.Images.Add(Resources.TestConnectionUnknown);
            connectionTestStatusImages.Images.Add(Resources.ToolbarRefresh);
            connectionTestStatusImages.Images.Add(Resources.TestConnectionSuccess);

            instancesListViewColumnSorter = new InstancesListViewColumnSorter();
            instancesListView.ListViewItemSorter = instancesListViewColumnSorter;
            instancesListViewColumnSorter.SortColumn = 1;
            instancesListViewColumnSorter.Order = SortOrder.Ascending;

            Icon licenseInformationImage = new Icon(SystemIcons.Information, 16, 16);
            licenseInformationPictureBox.Image = licenseInformationImage.ToBitmap();
            licenseInformationLabel.Text = string.Format(LicenseInformationLabelText, "???");

            SizeLastColumn(instancesListView);

            LoadMonitoredInstances();
            UpdateForm();
            AdaptFontSize();
        }

        private void SizeLastColumn(ListView instancesListView)
        {
            if (instancesListView.Columns.Count > 0)
            {
                instancesListView.Columns[instancesListView.Columns.Count - 1].Width = -2;
            }
        }

        private void LoadMonitoredInstances()
        {
            try
            {
                List<string> licenseKeys = new List<string>();
                IList<MonitoredSqlServer> monitoredServers = 
                    RepositoryHelper.GetMonitoredSqlServers(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, true, true, licenseKeys);

                if (monitoredServers != null)
                {
                    foreach (MonitoredSqlServer instance in monitoredServers)
                    {
                        ListViewItem instanceItem = new ListViewItem(string.Empty, TestConnectionUnknownImageIndex);
                        instanceItem.Tag = instance;
                        instanceItem.ToolTipText = TestConnectionStatusUnknownTooltipText;
                        instanceItem.SubItems.Add(instance.ConnectionInfo.InstanceName);
                        instanceItem.SubItems.Add(instance.ConnectionInfo.UseIntegratedSecurity
                                                      ?
                                                  WindowsAuthenticationModeText
                                                      :
                                                  SqlServerAuthenticationModeText);
                        instanceItem.SubItems.Add(instance.IsVirtualized 
                                                      ? instance.VirtualizationConfiguration.VCName 
                                                      : "");
                        instanceItem.SubItems.Add(instance.IsVirtualized
                                                      ? instance.VirtualizationConfiguration.VMName
                                                      : "");
                        instancesListView.Items.Add(instanceItem);
                        listViewLookupTable.Add(instance.ConnectionInfo.InstanceName.ToUpper(), instanceItem);
                    }
                }

                LicenseSummary license = LicenseSummary.SummarizeKeys(0, Settings.Default.ActiveRepositoryConnection.RepositoryInfo.InstanceName, licenseKeys);
                if (license.IsUnlimited) 
                    licenseInformationLabel.Text = string.Format(LicenseInformationLabelText, "unlimited");
                else
                    licenseInformationLabel.Text = string.Format(LicenseInformationLabelText, license.LicensedServers);
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this,
                                                "Unable to retrieve currently monitored SQL Servers from the SQLDM Repository. Please resolve the following error and try again.",
                                                e);
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            IDictionary<string, object> currentInstances = GetCurrentInstancesLookupTable();

            AddServersWizard addServersWizard = new AddServersWizard(false, currentInstances);

            if (addServersWizard.ShowDialog(this) == DialogResult.OK)
            {
                IList<MonitoredSqlServerConfiguration> addedInstances = addServersWizard.AddedInstances;
                List<ListViewItem> addedItems = new List<ListViewItem>();

                foreach (MonitoredSqlServerConfiguration addedInstance in addedInstances)
                {
                    ListViewItem newInstanceItem = new ListViewItem(string.Empty, TestConnectionUnknownImageIndex);
                    newInstanceItem.Tag = addedInstance;
                    newInstanceItem.ToolTipText = TestConnectionStatusUnknownTooltipText;
                    newInstanceItem.SubItems.Add(addedInstance.ConnectionInfo.InstanceName);
                    newInstanceItem.SubItems.Add(addedInstance.ConnectionInfo.UseIntegratedSecurity
                                                     ? WindowsAuthenticationModeText
                                                     : SqlServerAuthenticationModeText);
                    newInstanceItem.SubItems.Add("");
                    newInstanceItem.SubItems.Add("");
                    instancesListView.Items.Add(newInstanceItem);
                    addedItems.Add(newInstanceItem);
                    listViewLookupTable.Add(addedInstance.ConnectionInfo.InstanceName.ToUpper(), newInstanceItem);
                    AddPendingChange(addedInstance, ChangeType.Add);
                }

                if (ApplicationMessageBox.ShowQuestion(this, "Would you like to perform connection tests on the SQL Servers you added?") == DialogResult.Yes)
                {
                    TestConnections(addedItems);
                }
            }

            UpdateForm();
        }

        private IDictionary<string, object> GetCurrentInstancesLookupTable()
        {
            Dictionary<string, object> lookupTable = new Dictionary<string, object>();

            foreach (ListViewItem item in instancesListView.Items)
            {
                SqlConnectionInfo connectionInfo = null;

                if (item.Tag is MonitoredSqlServer)
                {
                    MonitoredSqlServer instance = item.Tag as MonitoredSqlServer;
                    connectionInfo = instance.ConnectionInfo;
                }
                else if (item.Tag is MonitoredSqlServerConfiguration)
                {
                    MonitoredSqlServerConfiguration configuration = item.Tag as MonitoredSqlServerConfiguration;
                    connectionInfo = configuration.ConnectionInfo;
                }

                if (connectionInfo != null)
                {
                    string instanceName = connectionInfo.InstanceName.ToUpper();
                    lookupTable.Add(instanceName, instanceName);
                }
            }

            return lookupTable;
        }

        private void UpdateForm()
        {
            instanceCountLabel.Text = string.Format(InstanceCountLabelText, instancesListView.Items.Count);
            gettingStartedLabel.Visible = instancesListView.Items.Count == 0;
            instancesListView.Visible = instancesListView.Items.Count != 0;
            instancesListView.Enabled = true;
            testButton.Enabled = instancesListView.SelectedItems.Count > 0;
            addButton.Enabled = true;
            editButton.Enabled = instancesListView.SelectedItems.Count == 1;
            removeButton.Enabled = instancesListView.SelectedItems.Count > 0;
            okButton.Enabled = true;
            cancelButton.Enabled = true;
            btnVMConfig.Enabled = true;
        }

        private void instancesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            TestSelectedConnections();
        }

        private void TestSelectedConnections()
        {
            TestConnections(instancesListView.SelectedItems);
        }

        private void TestConnections(IList items)
        {
            if (items != null && items.Count > 0)
            {
                try
                {
                    statusProgressBar.Start();

                    defaultManagementService =
                        ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    foreach (ListViewItem item in items)
                    {
                        QueueConnectionTest(item);
                    }
                }
                catch (Exception e)
                {
                    statusProgressBar.Stop();
                    ApplicationMessageBox.ShowError(this, "Unable to test connections for the selected instances.", e);
                }
            }
        }

        private void QueueConnectionTest(ListViewItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            SqlConnectionInfo connectionInfo = null;

            if (item.Tag is MonitoredSqlServer)
            {
                connectionInfo = ((MonitoredSqlServer) item.Tag).ConnectionInfo;
            }
            else if (item.Tag is MonitoredSqlServerConfiguration)
            {
                connectionInfo = ((MonitoredSqlServerConfiguration) item.Tag).ConnectionInfo;
            }

            if (connectionInfo == null)
            {
                throw new ArgumentNullException("item.Tag");
            }

            if (!pendingConnectionTests.ContainsKey(connectionInfo.InstanceName))
            {
                item.ImageIndex = TestConnectionInProgressImageIndex;
                item.ToolTipText = TestConnectionRefreshingTooltipText;
                pendingConnectionTests.Add(connectionInfo.InstanceName, item);
                TestConnection(connectionInfo);
            }
        }

        private void TestConnection(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            BackgroundWorker testConnectionWorker = new BackgroundWorker();
            testConnectionWorker.WorkerSupportsCancellation = true;
            testConnectionWorker.DoWork += new DoWorkEventHandler(testConnectionWorker_DoWork);
            testConnectionWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(testConnectionWorker_RunWorkerCompleted);
            testConnectionWorker.RunWorkerAsync(connectionInfo);
        }

        private void testConnectionWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "TestConnectionWorker";
            
            BackgroundWorker worker = sender as BackgroundWorker;

            if (worker != null)
            {

                if (!worker.CancellationPending)//SQLdm 9.1 (Gaurav Karwal) : move ahead only is user has not cancelled the operation
                {
                    SqlConnectionInfo connectionInfo = e.Argument as SqlConnectionInfo;

                    try
                    {
                        activeTestConnectionWorkers.Add(worker);
                        e.Result = defaultManagementService.TestSqlConnection(connectionInfo);
                        e.Cancel = worker.CancellationPending;
                    }
                    catch (Exception exception)
                    {
                        e.Result =
                            new TestSqlConnectionResult(connectionInfo,
                                                        new ApplicationException(
                                                            "A connection to the management service may not be available.",
                                                            exception));
                    }
                }
            }
        }

        private void testConnectionWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (e.Error != null)
                {
                    ApplicationMessageBox.ShowError(this, "An unhandled exception occurred while testing connections.",
                                                    e.Error);
                }
                else
                {
                    TestSqlConnectionResult testResult = e.Result as TestSqlConnectionResult;

                    if (testResult != null)
                    {
                        ListViewItem listViewItem;

                        if (pendingConnectionTests.TryGetValue(testResult.ConnectionInfo.InstanceName, out listViewItem))
                        {
                            if (testResult.Succeeded)
                            {
                                StringBuilder toolTipText;
                                string[] versionSplit = testResult.ServerVersion.Split(new char[] {'.'});

                                if (Convert.ToInt32(versionSplit[0]) >= 8)
                                {
                                    listViewItem.ImageIndex = TestConnectionSuccessImageIndex;
                                    toolTipText = new StringBuilder(TestConnectionSucceededTooltipText);
                                }
                                else
                                {
                                    listViewItem.ImageIndex = TestConnectionWarningImageIndex;
                                    toolTipText = new StringBuilder(TestConnectionUnsupportedVersionTooltipText);
                                }

                                toolTipText.Append("\r\n\r\n");
                                toolTipText.Append("Version: ");
                                toolTipText.Append(testResult.ServerVersion);
                                listViewItem.ToolTipText = toolTipText.ToString();
                            }
                            else
                            {
                                listViewItem.ImageIndex = TestConnectionFailedImageIndex;

                                StringBuilder toolTipText = new StringBuilder(TestConnectionFailedTooltipText);
                                toolTipText.Append("\r\n\r\n");
                                toolTipText.Append(testResult.Error != null
                                                       ? testResult.Error.Message
                                                       : "Unknown error.");
                                listViewItem.ToolTipText = toolTipText.ToString();
                            }

                            pendingConnectionTests.Remove(testResult.ConnectionInfo.InstanceName);

                            if (pendingConnectionTests.Count == 0)
                            {
                                statusProgressBar.Stop();
                            }
                        }
                    }
                }

                activeTestConnectionWorkers.Remove(sender as BackgroundWorker);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            okButtonPressed = true;
            ApplyPendingChanges();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            CancelActiveConnectionTests();
            base.OnClosing(e);
        }

        private void CancelActiveConnectionTests()
        {
            if (activeTestConnectionWorkers != null)
            {
                foreach (BackgroundWorker worker in activeTestConnectionWorkers)
                {
                    worker.CancelAsync();
                }
            }
        }

        private bool DoesMonitoredSqlServerExistInSelectedItems()
        {
            bool exists = false;

            foreach (ListViewItem selectedItem in instancesListView.SelectedItems)
            {
                if (selectedItem.Tag is MonitoredSqlServer)
                {
                    exists = true;
                    break;
                }
            }

            return exists;
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult;

            if (DoesMonitoredSqlServerExistInSelectedItems())
            {
                dialogResult =
                    ApplicationMessageBox.ShowWarning(this,
                                                      "SQL Diagnostic Manager allows you to retain data collected for SQL Server instances that are no longer monitored. " +
                                                      "This data may be useful for reporting purposes at a later time.\r\n\r\n" +
                                                      "Would you like to retain the data collected for the selected instance(s)?",
                                                      ExceptionMessageBoxButtons.YesNoCancel);
            }
            else
            {
                dialogResult = DialogResult.No;
            }

            if (dialogResult != DialogResult.Cancel)
            {
                foreach (ListViewItem selectedItem in instancesListView.SelectedItems)
                {
                    instancesListView.Items.Remove(selectedItem);

                    if (selectedItem.Tag is MonitoredSqlServer)
                    {
                        MonitoredSqlServer instance = selectedItem.Tag as MonitoredSqlServer;
                        listViewLookupTable.Remove(instance.ConnectionInfo.InstanceName.ToUpper());

                        if (dialogResult == DialogResult.Yes)
                        {
                            AddPendingChange(instance, ChangeType.Deactivate);
                        }
                        else
                        {
                            AddPendingChange(instance, ChangeType.Delete);
                        }
                    }
                    else if (selectedItem.Tag is MonitoredSqlServerConfiguration)
                    {
                        MonitoredSqlServerConfiguration configuration =
                            selectedItem.Tag as MonitoredSqlServerConfiguration;
                        listViewLookupTable.Remove(configuration.ConnectionInfo.InstanceName.ToUpper());

                        if (dialogResult == DialogResult.Yes)
                        {
                            AddPendingChange(configuration, ChangeType.Deactivate);
                        }
                        else
                        {
                            AddPendingChange(configuration, ChangeType.Delete);
                        }
                    }
                }

                UpdateForm();
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            EditSelectedInstance();
        }

        private void instancesListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            EditSelectedInstance();
        }

        private void EditSelectedInstance()
        {
            if (instancesListView.SelectedItems.Count == 1)
            {
                if (instancesListView.SelectedItems[0].Tag is MonitoredSqlServer)
                {
                    MonitoredSqlServer instance = instancesListView.SelectedItems[0].Tag as MonitoredSqlServer;

                    try
                    {
                        MonitoredSqlServerInstancePropertiesDialog dialog =
                            new MonitoredSqlServerInstancePropertiesDialog(instance.Id);

                        if (dialog.ShowDialog(this) == DialogResult.OK)
                        {
                            instancesListView.SelectedItems[0].Tag = dialog.SavedServer;
                            instancesListView.SelectedItems[0].SubItems[2].Text =
                                dialog.SavedServer.ConnectionInfo.UseIntegratedSecurity
                                    ?
                                WindowsAuthenticationModeText
                                    :
                                SqlServerAuthenticationModeText;
                            instancesListView.SelectedItems[0].SubItems[3].Text =
                                dialog.SavedServer.IsVirtualized
                                    ? dialog.SavedServer.VirtualizationConfiguration.VCName
                                    : "";
                            instancesListView.SelectedItems[0].SubItems[4].Text = 
                                dialog.SavedServer.IsVirtualized
                                    ? dialog.SavedServer.VirtualizationConfiguration.VMName
                                    : "";
                        }
                    }
                    catch (Exception e)
                    {
                        ApplicationMessageBox.ShowError(this,
                                                            string.Format("Unable to show properties for {0}.",
                                                                          instance), e);
                    }
                }
                else if (instancesListView.SelectedItems[0].Tag is MonitoredSqlServerConfiguration)
                {
                    MonitoredSqlServerConfiguration configuration =
                        instancesListView.SelectedItems[0].Tag as MonitoredSqlServerConfiguration;
                    MonitoredSqlServerInstancePropertiesDialog dialog =
                        new MonitoredSqlServerInstancePropertiesDialog(configuration);

                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        AddPendingChange(configuration, ChangeType.Delete);
                        AddPendingChange(dialog.Configuration, ChangeType.Add);

                        instancesListView.SelectedItems[0].Tag = dialog.Configuration;
                        instancesListView.SelectedItems[0].SubItems[2].Text =
                            dialog.Configuration.ConnectionInfo.UseIntegratedSecurity
                                ?
                            WindowsAuthenticationModeText
                                :
                            SqlServerAuthenticationModeText;
                        instancesListView.SelectedItems[0].SubItems[3].Text =
                            dialog.Configuration.IsVirtualized
                                ? dialog.Configuration.VirtualizationConfiguration.VCName
                                : "";
                        instancesListView.SelectedItems[0].SubItems[4].Text =
                            dialog.Configuration.IsVirtualized
                                ? dialog.Configuration.VirtualizationConfiguration.VMName
                                : "";
                    }
                }
            }
        }

        private void AddPendingChange(object target, ChangeType type)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            ChangeType existing;

            if (pendingChanges.TryGetValue(target, out existing))
            {
                if (existing == type)
                {
                    return;
                }
                else if (type == ChangeType.Deactivate || type == ChangeType.Delete)
                {
                    pendingChanges.Remove(target);

                    if (existing != ChangeType.Add)
                    {
                        pendingChanges.Add(target, type);
                    }
                }
                else if (type == ChangeType.Add)
                {
                    MonitoredSqlServerConfiguration addedConfiguration = target as MonitoredSqlServerConfiguration;

                    if (addedConfiguration != null)
                    {
                        MonitoredSqlServer pendingRemove = FindPendingChange(addedConfiguration.InstanceName);

                        if (pendingRemove != null)
                        {
                            pendingChanges.Remove(pendingRemove);
                            pendingChanges.Add(pendingRemove, ChangeType.Update);
                            // TODO: Update existing server configuration
                        }
                    }
                }
            }
            else
            {
                pendingChanges.Add(target, type);
            }

            applyButton.Enabled = pendingChanges.Count > 0;
        }

        private void btnVMConfig_Click(object sender, EventArgs e)
        {
            try
            {
                using (VirtualizationConfig virtualizationConfig = new VirtualizationConfig())
                {
                    foreach(ListViewItem item in instancesListView.Items)
                    {
                        if (item.Tag is MonitoredSqlServer)
                        {
                            MonitoredSqlServer server = item.Tag as MonitoredSqlServer;
                            
                            if (server.IsVirtualized)
                                virtualizationConfig.AddLinkedServer(server.Id, 
                                                                    server.InstanceName, 
                                                                    server.VirtualizationConfiguration.VCHostID,
                                                                    server.VirtualizationConfiguration.VCAddress,
                                                                    server.VirtualizationConfiguration.VCName,
                                                                    server.VirtualizationConfiguration.InstanceUUID,
                                                                    server.VirtualizationConfiguration.VMName,
                                                                    server.VirtualizationConfiguration.VMDomainName);
                            else
                            {
                                virtualizationConfig.AddServer(server.Id, server.InstanceName);
                            }
                        }
                        else if (item.Tag is MonitoredSqlServerConfiguration)
                        {
                            MonitoredSqlServerConfiguration serverConfig = item.Tag as MonitoredSqlServerConfiguration;

                            if (serverConfig.IsVirtualized)
                                virtualizationConfig.AddLinkedServer(-1,
                                                                    serverConfig.InstanceName,
                                                                    serverConfig.VirtualizationConfiguration.VCHostID,
                                                                    serverConfig.VirtualizationConfiguration.VCAddress,
                                                                    serverConfig.VirtualizationConfiguration.VCName,
                                                                    serverConfig.VirtualizationConfiguration.InstanceUUID,
                                                                    serverConfig.VirtualizationConfiguration.VMName,
                                                                    serverConfig.VirtualizationConfiguration.VMDomainName);
                            else
                            {
                                virtualizationConfig.AddServer(-1, serverConfig.InstanceName);
                            }
                        }
                    }

                    if (DialogResult.OK == virtualizationConfig.ShowDialog(this))
                    {
                        foreach (ListViewItem item in instancesListView.Items)
                        {
                            VirtualizationConfiguration vmConfig = null;
                            if (item.Tag is MonitoredSqlServerConfiguration)
                            {
                                MonitoredSqlServerConfiguration serverConfig =
                                    item.Tag as MonitoredSqlServerConfiguration;

                                serverConfig.VirtualizationConfiguration =
                                    virtualizationConfig.GetVirtualizationConfig(serverConfig.InstanceName);

                                AddPendingChange(serverConfig, ChangeType.Delete);
                                AddPendingChange(serverConfig, ChangeType.Add);

                                item.Tag = serverConfig;
                                vmConfig = serverConfig.IsVirtualized ? serverConfig.VirtualizationConfiguration : null;
                            }
                            else if (item.Tag is MonitoredSqlServer)
                            {
                                MonitoredSqlServer server = item.Tag as MonitoredSqlServer;

                                server.VirtualizationConfiguration =
                                    virtualizationConfig.GetVirtualizationConfig(server.InstanceName);

                                item.Tag = server;
                                vmConfig = server.IsVirtualized ? server.VirtualizationConfiguration : null;
                            }

                            if (vmConfig != null)
                            {
                                item.SubItems[3].Text = vmConfig.VCName;
                                item.SubItems[4].Text = vmConfig.VMName;
                            }
                            else
                            {
                                item.SubItems[3].Text = "";
                                item.SubItems[4].Text = "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Error in the Virtualization Configuration Dialog", ex);
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            ApplyPendingChanges();
        }

        private void ApplyPendingChanges()
        {
            Cursor = Cursors.WaitCursor;
            statusProgressBar.Start();
            instancesListView.Enabled = false;
            addButton.Enabled = false;
            editButton.Enabled = false;
            removeButton.Enabled = false;
            testButton.Enabled = false;
            okButton.Enabled = false;
            applyButton.Enabled = false;
            cancelButton.Enabled = false;
            btnVMConfig.Enabled = false;
            applyChangesBackgroundWorker.RunWorkerAsync();
        }

        private MonitoredSqlServer FindPendingChange(string instanceName)
        {
            foreach (MonitoredSqlServer instance in pendingChanges.Keys)
            {
                if (string.Compare(instance.ConnectionInfo.InstanceName, instanceName, true) == 0)
                {
                    return instance;
                }
            }

            return null;
        }

        private void instancesListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == instancesListViewColumnSorter.SortColumn)
            {
                if (instancesListViewColumnSorter.Order == SortOrder.Ascending)
                {
                    instancesListViewColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    instancesListViewColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                instancesListViewColumnSorter.SortColumn = e.Column;
                instancesListViewColumnSorter.Order = SortOrder.Ascending;
            }

            instancesListView.Sort();
        }

        private void ManageServersDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ManageServers);
        }

        private void ManageServersDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ManageServers);
        }

        private void instancesListView_SizeChanged(object sender, EventArgs e)
        {
            gettingStartedLabel.Bounds = instancesListView.Bounds;
        }

        private void applyChangesBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "ManageServersApply";

            List<MonitoredSqlServerConfiguration> pendingAdds = new List<MonitoredSqlServerConfiguration>();
            List<MonitoredSqlServerWrapper> pendingDeactivates = new List<MonitoredSqlServerWrapper>();
            List<MonitoredSqlServerWrapper> pendingDeletes = new List<MonitoredSqlServerWrapper>();

            foreach (object target in pendingChanges.Keys)
            {
                ChangeType operation;

                if (pendingChanges.TryGetValue(target, out operation))
                {
                    switch (operation)
                    {
                        case ChangeType.Add:
                            if (target is MonitoredSqlServerConfiguration)
                            {
                                pendingAdds.Add(target as MonitoredSqlServerConfiguration);
                            }
                            break;
                        case ChangeType.Deactivate:
                            if (target is MonitoredSqlServer)
                            {
                                MonitoredSqlServer instance = target as MonitoredSqlServer;

                                if (ApplicationModel.Default.ActiveInstances.Contains(instance.Id))
                                {
                                    pendingDeactivates.Add(ApplicationModel.Default.ActiveInstances[instance.Id]);
                                }
                            }
                            break;
                        case ChangeType.Delete:
                            if (target is MonitoredSqlServer)
                            {
                                MonitoredSqlServer instance = target as MonitoredSqlServer;

                                if (ApplicationModel.Default.ActiveInstances.Contains(instance.Id))
                                {
                                    pendingDeletes.Add(ApplicationModel.Default.ActiveInstances[instance.Id]);
                                }
                            }
                            break;
                        case ChangeType.Update:
                            break;
                    }
                }
            }

            // Apply changes as batches... delete should come before adds to prevent license violations
            ApplicationModel.Default.DeleteMonitoredSqlServers(pendingDeletes);
            ApplicationModel.Default.DeactivateMonitoredSqlServers(pendingDeactivates);
            IList<MonitoredSqlServerWrapper> addedInstances = ApplicationModel.Default.AddMonitoredSqlServers(pendingAdds);

            if (addedInstances != null)
            {
                for (int i = 0; i < addedInstances.Count; i++)
                {
                    ListViewItem existingItem;

                    if (listViewLookupTable.TryGetValue(addedInstances[i].InstanceName.ToUpper(),
                                                        out existingItem))
                    {
                        existingItem.Tag = addedInstances[i].Instance;


                        string vCenterNameString = "";
                        string vmNameString = "";
                        if (addedInstances[i].Instance.IsVirtualized)
                        {
                            vCenterNameString = addedInstances[i].Instance.VirtualizationConfiguration.VCName;
                            vmNameString = addedInstances[i].Instance.VirtualizationConfiguration.VMName;
                        }

                        Helpers.CrossThreadHelper.UIThread(this, (MethodInvoker)delegate
                        {
                            existingItem.SubItems[3].Text = vCenterNameString; // runs on UI thread
                            existingItem.SubItems[4].Text = vmNameString;
                            Log.Debug(string.Format("Cross-thread helper used to add {0}", addedInstances[i].Instance.InstanceName));
                        });

                    }
                }
            }
        }

        private void applyChangesBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Default;
            statusProgressBar.Stop();
            UpdateForm();

            if (e.Error != null)
            {
                okButtonPressed = false;
                applyButton.Enabled = true;
                ApplicationMessageBox.ShowError(this,
                                                "An error occurred while applying the changes. Please resolve the following error and try again.",
                                                e.Error);
            }
            else
            {
                pendingChanges.Clear();

                if (okButtonPressed)
                {
                    Close();
                }
            }
        }

        #region Private Types

        private enum ChangeType
        {
            Add,
            Delete,
            Deactivate,
            Update
        }

        private class InstancesListViewColumnSorter : IComparer
        {
            private int columnToSort;
            private SortOrder orderOfSort;
            private CaseInsensitiveComparer objectCompare;

            public InstancesListViewColumnSorter()
            {
                columnToSort = 0;
                orderOfSort = SortOrder.None;
                objectCompare = new CaseInsensitiveComparer();
            }

            /// <summary>
            /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
            /// </summary>
            /// <param name="x">First object to be compared</param>
            /// <param name="y">Second object to be compared</param>
            /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
            public int Compare(object x, object y)
            {
                int compareResult;
                ListViewItem listviewX, listviewY;

                // Cast the objects to be compared to ListViewItem objects
                listviewX = (ListViewItem) x;
                listviewY = (ListViewItem) y;

                // Compare the two items
                if (columnToSort == 0)
                {
                    if (listviewX.ImageIndex < listviewY.ImageIndex)
                    {
                        compareResult = -1;
                    }
                    else if (listviewX.ImageIndex > listviewY.ImageIndex)
                    {
                        compareResult = 1;
                    }
                    else
                    {
                        compareResult = 0;
                    }
                }
                else
                {
                    compareResult =
                        objectCompare.Compare(listviewX.SubItems[columnToSort].Text,
                                              listviewY.SubItems[columnToSort].Text);
                }

                if (orderOfSort == SortOrder.Ascending)
                {
                    return compareResult;
                }
                else if (orderOfSort == SortOrder.Descending)
                {
                    return (-compareResult);
                }
                else
                {
                    return 0;
                }
            }

            /// <summary>
            /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
            /// </summary>
            public int SortColumn
            {
                set { columnToSort = value; }
                get { return columnToSort; }
            }

            /// <summary>
            /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
            /// </summary>
            public SortOrder Order
            {
                set { orderOfSort = value; }
                get { return orderOfSort; }
            }
        }

        #endregion

        private void instancesListView_Resize(object sender, EventArgs e)
        {
            SizeLastColumn((ListView)sender);
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}
