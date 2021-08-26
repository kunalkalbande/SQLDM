using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common.Auditing;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;
using Wintellect.PowerCollections;
using BBS.TracerX;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.VMware;
using Idera.SQLdm.Common.HyperV;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Resources = Idera.SQLdm.DesktopClient.Properties.Resources;
using Settings = Idera.SQLdm.DesktopClient.Properties.Settings;
using System.Runtime.InteropServices;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class VirtualizationConfig : Form
    {
        private static readonly Logger Log = Logger.GetLogger("VirtualizationConfig");
        private const string STATUS_GATHERING_ALL_VMS = "Contacting Virtualization Host servers to get virtual machine names";
        private const string STATUS_FMT_GATHERING_VMS = "Gathering a list of virtual machines from {0}";
        private const string STATUS_FMT_TEST = "Attempting to connect to {0}";
        private const string ERROR_FMT_GATHERING_VMS = "Error retrieving list of virtual machines from {0}:\r\n{1}";
        private static Color BACKCOLOR_CHANGED = System.Drawing.Color.FromArgb(225, 225, 175);

        private const string VALUE_NONE = @"<none>";
        private const string VALUE_SELECT_VM = @"<select a virtual machine...>";
        private static readonly string VALUE_EMPTY = string.Empty;

        private Dictionary<string, vCenterStatus> vcDict = null;
        private IList<vCenterStatus> vcList = null; 

        private bool allowDelete = true;
        private DataTable serverLinksDataTable;
        private ValueList vcValueList;
        private ValueList vmValueList;
        private Dictionary<string, Dictionary<string, basicVMInfo>> vmByVC = null;
        private List<Pair<vCenterConnectAction, vCenterStatus>> vcConnectList = new List<Pair<vCenterConnectAction, vCenterStatus>>();
        private bool creatingDropDownList = false;

        private Dictionary<string, vCenterHosts> vcEditDict = null;

        #region Constructor

        public VirtualizationConfig(bool allowDelete) : this ()
        {
            this.allowDelete = allowDelete;
        }

        public VirtualizationConfig()
        {
            InitializeComponent();

            vCenterConnectionLabel.Text = string.Empty;

            // using the same images here as in the manage servers dialog
            ValueList statusValueList = vcGrid.DisplayLayout.ValueLists["statusValueList"];
            ValueListItem listItem = new ValueListItem(vCenterTestStatus.Unknown);
            listItem.Appearance.Image = Resources.TestConnectionUnknown;
            statusValueList.ValueListItems.Add(listItem);
            listItem = new ValueListItem(vCenterTestStatus.Testing);
            listItem.Appearance.Image = Resources.ToolbarRefresh;
            statusValueList.ValueListItems.Add(listItem);
            listItem = new ValueListItem(vCenterTestStatus.Success);
            listItem.Appearance.Image = Resources.TestConnectionSuccess;
            statusValueList.ValueListItems.Add(listItem);
            listItem = new ValueListItem(vCenterTestStatus.Failed);
            listItem.Appearance.Image = Resources.TestConnectionFailed;
            statusValueList.ValueListItems.Add(listItem);

            vcValueList = vmGrid.DisplayLayout.ValueLists["vcValueList"];
            vcValueList.SortStyle = ValueListSortStyle.Ascending;
            vmValueList = vmGrid.DisplayLayout.ValueLists["vmValueList"];
            vmValueList.SortStyle = ValueListSortStyle.Ascending;

            vcGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            vcGrid.CreationFilter = new ToolTipItemCreationFilter();
            vmGrid.DrawFilter = new HideFocusRectangleDrawFilter();

            initializeDataSource();
            btnOK.Enabled = false;
            AdaptFontSize();
        }

        #endregion

        #region Event Methods

        #region Button Click Events

        private void btnAdd_Click(object sender, EventArgs e)
        {
            vCenterHosts newHost = new vCenterHosts();

            // Add the new vCenter server to the list
            if (DialogResult.OK == showAddEditDialog(DialogAction.Add, newHost))
            {
                refreshControls(RefreshRow.ReloadData);
            }

            updateVCenterButtons();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            vCenterHosts editHost;

            // get the host is being edited
            editHost = (vCenterHosts)vcGrid.Selected.Rows[0].ListObject;

            // save the new information and update the relevant controls
            if (DialogResult.OK == showAddEditDialog(DialogAction.Edit, editHost))
            {
                refreshControls(RefreshRow.RefreshDisplay);
            }

            updateVCenterButtons();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            vCenterHosts deleteHost = (vCenterHosts)vcGrid.Selected.Rows[0].ListObject;

            StringBuilder message = new StringBuilder();
            message.AppendFormat("Deleting Virtualization Host Server {0} will remove links to all associated SQL Server instances.", deleteHost.vcName);
            message.Append("\r\n\r\nAre you sure you want to delete this Virtualization Host Server?");

            if (DialogResult.Yes == ApplicationMessageBox.ShowQuestion(this, message.ToString(), Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo))
            {
                foreach (UltraGridRow gridRow in vmGrid.Rows)
                {
                    DataRow row = ((DataRowView)gridRow.ListObject).Row;
                    if (row["vcServerAddress"] != DBNull.Value && (string)row["vcServerAddress"] == deleteHost.vcAddress)
                    {
                        row["vcHostID"] = DBNull.Value;
                        row["vcServerName"] = DBNull.Value;
                        row["vcServerAddress"] = DBNull.Value;
                        row["vmUID"] = DBNull.Value;
                        row["vmName"] = string.Empty;
                        row["vmDomainName"] = string.Empty;
                        row["vcServerType"] = string.Empty;
                        gridRow.Appearance.BackColor = BACKCOLOR_CHANGED;
                    }
                }
                vmByVC.Remove(deleteHost.vcAddress);
                vcDict.Remove(deleteHost.vcAddress);
                foreach (vCenterStatus vcstat in vcList)
                {
                    if (vcstat.vcAddress == deleteHost.vcAddress)
                    {
                        vcList.Remove(vcstat);
                        break;
                    }
                }

                refreshControls(RefreshRow.ReloadData);
                btnOK.Enabled = true;
            }

            updateVCenterButtons();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
            AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

            // load a list of the current servers suitable for updating
            Dictionary<int, MonitoredSqlServer> currentServers = new Dictionary<int, MonitoredSqlServer>();
            foreach (MonitoredSqlServer server in RepositoryHelper.GetMonitoredSqlServers(connectionInfo, false))
            {
                currentServers.Add(server.Id, server);
            }

            //The VmConfigurationFlag indicate to management service this action should no be auditing in sql server properties change.
            //true is for audit and the false is for ignore
            AuditingEngine.SetAuxiliarData("VmConfigurationFlag", new AuditAuxiliar<bool>(false));

            List<vCenterHosts> hosts = new List<vCenterHosts>();
            foreach (vCenterHosts host in vcDict.Values)
            {
                if (host.ServerType == null) {
                    host.ServerType = "Unknown";
                }
                hosts.Add(host);

                if (host.vcHostID == -1)
                {
                    AuditingEngine.Instance.ManagementService =
                        ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                    AuditingEngine.Instance.SQLUser =
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UseIntegratedSecurity ?
                        AuditingEngine.GetWorkstationUser() : Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UserName;
                    if (host.ServerType == "HyperV")
                    {
                        AuditingEngine.Instance.LogAction(host.GetAuditableEntity(), AuditableActionType.AddHyperV, host.vcName);
                    }
                    else if (host.ServerType == "vCenter") {
                        AuditingEngine.Instance.LogAction(host.GetAuditableEntity(), AuditableActionType.AddVmCenter, host.vcName);
                    }
                }

                RepositoryHelper.AddUpdateVirtualHostServer(connectionInfo, host);

                AuditableEntity editEntity = new AuditableEntity();

                if (vcEditDict.ContainsKey(host.vcAddress))
                {
                    vCenterHosts oldValue = vcEditDict[host.vcAddress];
                    oldValue.vcHostID = host.vcHostID;
                    editEntity = host.GetAuditableEntity(oldValue);
                    AuditingEngine.Instance.ManagementService =
                        ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                    AuditingEngine.Instance.SQLUser =
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UseIntegratedSecurity ?
                        AuditingEngine.GetWorkstationUser() : Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UserName;
                    if (host.ServerType == "HyperV")
                    {
                        AuditingEngine.Instance.LogAction(editEntity, AuditableActionType.EditHyperV, editEntity.Name);
                    }
                    else if (host.ServerType == "vCenter")
                    {
                        AuditingEngine.Instance.LogAction(editEntity, AuditableActionType.EditVmCenter, editEntity.Name);
                    }
                }

                DataRow[] changedRows = serverLinksDataTable.Select(string.Format("vcServerAddress = '{0}'", host.vcAddress.Trim()));

                foreach (DataRow row in changedRows)
                {
                    row["vcHostID"] = host.vcHostID;
                }
            }

            List<Pair<int, MonitoredSqlServerConfiguration>> configurations = new List<Pair<int, MonitoredSqlServerConfiguration>>();
            Dictionary<int, VirtualizationConfiguration> oldConfigurations = new Dictionary<int, VirtualizationConfiguration>();
            foreach (DataRow row in serverLinksDataTable.Rows)
            {
                if ((row["vcServerAddress"] == DBNull.Value) || (row["vmUID"] == DBNull.Value )|| (row["vmName"] == DBNull.Value))
                {
                    row["vcHostID"] = DBNull.Value;
                    row["vcServerName"] = DBNull.Value;
                    row["vcServerAddress"] = DBNull.Value;
                    row["vmUID"] = DBNull.Value;
                    row["vmName"] = DBNull.Value;
                    row["vmDomainName"] = DBNull.Value;
                }

                int serverId = (int)row["monitoredServerID"];
                // don't try to match the instance if the server hasn't been created yet
                // will be -1 coming from manage servers or 0 coming from server properties
                if (serverId > 0)
                {
                    // always compare/update using the most current
                    MonitoredSqlServer server = currentServers[serverId];
                    VirtualizationConfiguration virtualizationConfiguration = null;

                    if ((row["vmUID"] != DBNull.Value))
                    {
                        vCenterHosts host = vcDict[(string)row["vcServerAddress"]];
                        virtualizationConfiguration =
                            new VirtualizationConfiguration((string)row["vmUID"],
                                                            (string)row["vmName"],
                                                            (string)row["vmDomainName"],
                                                            host.vcHostID,
                                                            host.vcName,
                                                            host.vcAddress,
                                                            host.vcUser,
                                                            host.vcEncryptedPassword,
                                                            host.ServerType
                                                            );
                    }

                    if (server.VirtualizationConfiguration.IsEqual(virtualizationConfiguration))
                        continue;

                    MonitoredSqlServerConfiguration serverConfig = server.GetConfiguration();
                    oldConfigurations.Add(server.Id, serverConfig.VirtualizationConfiguration);
                    serverConfig.VirtualizationConfiguration = virtualizationConfiguration;
                    configurations.Add(new Pair<int, MonitoredSqlServerConfiguration>(server.Id, serverConfig));
                }
            }

            if (configurations.Count > 0)
            {
                ApplicationModel.Default.UpdateMonitoredSqlServers(configurations);
            }

            List<string> editedHostList = new List<string>();
            foreach (var host in vcEditDict.Values)
            {
                editedHostList.Add(host.vcAddress);
            }

            // remove any deleted vcenters after updating the server links
            RepositoryHelper.DeleteUnusedVirtualHost(connectionInfo, hosts, editedHostList);

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            if (vcGrid.Selected.Rows.Count > 0)
            {
                UltraGridRow selectedRow = vcGrid.Selected.Rows[0];
                vCenterStatus hostStatus = (vCenterStatus)selectedRow.ListObject;
                hostStatus.Status = vCenterTestStatus.Testing;
                vcGrid.Rows.Refresh(RefreshRow.ReloadData);

                Pair<vCenterConnectAction, vCenterStatus> workItem = new Pair<vCenterConnectAction, vCenterStatus>(vCenterConnectAction.Connect, hostStatus);
                if (vCenterWorker.IsBusy)
                {
                    vcConnectList.Add(workItem);
                }
                else
                {
                    updateStatusProgressBar(true);
                    vCenterConnectionLabel.Text = string.Format(STATUS_FMT_TEST, hostStatus.vcName);
                    vCenterWorker.RunWorkerAsync(workItem);
                }
            }
        }

        private void btnLink_Click(object sender, EventArgs e)
        {
            if (serverLinksDataTable.Rows.Count > 0)
            {
                linkVMsToServers();
            }
        }

        private void selectAllServersCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataRow row in serverLinksDataTable.Rows)
            {
                row["selected"] = selectAllServersCheckBox.Checked;
            }

            updateVmActions(null);
        }

        #endregion

        #region Form Events

        private void VirtualizationConfig_Load(object sender, EventArgs e)
        {
            setupDataSource();
            if (serverLinksDataTable.Rows.Count == 0)
            {
                foreach (MonitoredSqlServerWrapper server in ApplicationModel.Default.ActiveInstances)
                {
                    // Make sure the user has permission to see the server
                    if (ApplicationModel.Default.UserToken.GetServerPermission(server.Id) != Common.Objects.ApplicationSecurity.PermissionType.None)
                    {
                        if (server.Instance.IsVirtualized)
                        {
                            AddLinkedServer(server.Id,
                                           server.InstanceName,
                                           server.Instance.VirtualizationConfiguration.VCHostID,
                                           server.Instance.VirtualizationConfiguration.VCAddress,
                                           server.Instance.VirtualizationConfiguration.VCName,
                                           server.Instance.VirtualizationConfiguration.InstanceUUID,
                                           server.Instance.VirtualizationConfiguration.VMName,
                                           server.Instance.VirtualizationConfiguration.VMDomainName);
                        }
                        else
                        {
                            AddServer(server.Id, server.InstanceName);
                        }
                    }
                }
            }

            if (vcDict.Count > 0)
            {
                vcConnectList.Clear();
                foreach (vCenterStatus host in vcDict.Values)
                {
                    host.Status = vCenterTestStatus.Testing;
                    vcConnectList.Add(new Pair<vCenterConnectAction, vCenterStatus>(vCenterConnectAction.GatherAllVMs, host));
                }

                updateStatusProgressBar(true);
                vCenterConnectionLabel.Text = STATUS_GATHERING_ALL_VMS;
                if (vcConnectList.Count > 0)
                {
                    Pair<vCenterConnectAction, vCenterStatus> testHost = vcConnectList[0];
                    vcConnectList.RemoveAt(0);
                    vCenterWorker.RunWorkerAsync(testHost);
                }
                vcGrid.Rows.Refresh(RefreshRow.ReloadData);
                updateVmActions(null);
            }

            updateVCenterButtons();

            // Update the empty list labels.
            DisplayNovCenterInstancesLabel();
            DisplayNoVirtualMachinesLabel();
        }

        private void VirtualizationConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (vCenterWorker.IsBusy)
                vCenterWorker.CancelAsync();
        }

        private void VirtualizationConfig_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            showHelp();
        }

        private void VirtualizationConfig_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            showHelp();
        }

        #endregion

        #region Grid Events

        private void vcGrid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (btnEdit.Enabled)
            {
                btnEdit_Click(sender, e);
            }
        }

        private void vcGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            updateVCenterButtons();
        }

        private void vmGrid_BeforeCellListDropDown(object sender, CancelableCellEventArgs e)
        {
            if (e.Cell.Column.Key == "vcServerName")
            {
                try
                {
                    creatingDropDownList = true;

                    string cellValue = (e.Cell.Value == DBNull.Value) ? VALUE_EMPTY : (string)e.Cell.Value;

                    vcValueList.ValueListItems.Clear();

                    bool found = false;
                    if (string.IsNullOrEmpty(cellValue))
                    {
                        // add the blank value at the top
                        vcValueList.ValueListItems.Add(cellValue);
                        found = true;
                    }
                    else
                    {
                        vcValueList.ValueListItems.Add(VALUE_NONE);
                    }

                    foreach (vCenterHosts host in vcDict.Values)
                    {
                        ValueListItem item = new ValueListItem(host.vcName);
                        vcValueList.ValueListItems.Add(item);
                        if (cellValue == host.vcName)
                        {
                            found = true;
                            vcValueList.SelectedItem = item;
                        }
                    }
                    if (!found && cellValue != VALUE_NONE)
                    {
                        // this adds the empty string if it is null too
                        ValueListItem item = new ValueListItem(cellValue);
                        vcValueList.ValueListItems.Insert(0, item);
                        vcValueList.SelectedItem = item;
                    }
                }
                finally
                {
                    creatingDropDownList = false;
                }
            }
            else if (e.Cell.Column.Key == "vmName")
            {
                try
                {
                    creatingDropDownList = true;

                    string cellValue = (e.Cell.Value == DBNull.Value) ? VALUE_EMPTY : (string)e.Cell.Value;

                    vmValueList.ValueListItems.Clear();

                    vmValueList.ValueListItems.Add(VALUE_SELECT_VM);

                    bool found = false;
                    Dictionary<string, basicVMInfo> vmInfos;
                    if (vmByVC.TryGetValue(e.Cell.Row.Cells["vcServerAddress"].Text, out vmInfos))
                    {
                        foreach (basicVMInfo vm in vmInfos.Values)
                        {
                            ValueListItem item = new ValueListItem(vm.VMName, string.Format("{0} ({1})",
                                                                                                vm.VMName, 
                                                                                                string.IsNullOrEmpty(vm.DomainName) ? "Unknown" : vm.DomainName));
                            vmValueList.ValueListItems.Add(item);
                            if (cellValue == vm.VMName)
                            {
                                found = true;
                                vmValueList.SelectedItem = item;
                            }
                        }
                    }

                    if (!found && cellValue != VALUE_EMPTY && cellValue != VALUE_SELECT_VM)
                    {
                        ValueListItem item = new ValueListItem(cellValue);
                        vmValueList.ValueListItems.Add(item);
                        vmValueList.SelectedItem = item;
                    }
                }
                finally
                {
                    creatingDropDownList = false;
                }
            }
        }

        private void vmGrid_CellChange(object sender, CellEventArgs e)
        {
            if (creatingDropDownList)
                return;

            if (e.Cell.Column.Key == "selected")
            {
                updateVmActions(e.Cell);
            }
            else if (e.Cell.Column.Key == "vcServerName")
            {
                if (e.Cell.Value == e.Cell.Text ||
                    (e.Cell.Value == DBNull.Value && e.Cell.Text == VALUE_EMPTY))
                    return;

                if (e.Cell.Text == VALUE_EMPTY ||
                    e.Cell.Text == VALUE_NONE)
                {
                    e.Cell.Row.Cells["vcHostID"].Value = DBNull.Value;
                    e.Cell.Row.Cells["vcServerAddress"].Value = DBNull.Value;
                    e.Cell.Row.Cells["vcServerType"].Value = VALUE_EMPTY;
                    e.Cell.Row.Cells["vmUID"].Value = DBNull.Value;
                    e.Cell.Row.Cells["vmName"].Value = DBNull.Value;
                    e.Cell.Row.Cells["vmDomainName"].Value = DBNull.Value;
                }
                else
                {
                    vCenterStatus selectedHost = null;
                    foreach (vCenterStatus host in vcDict.Values)
                    {
                        if (host.vcName == e.Cell.Text)
                        {
                            selectedHost = host;
                            break;
                        }
                    }

                    if (selectedHost != null)
                    {
                        e.Cell.Row.Cells["vcHostID"].Value = selectedHost.vcHostID;
                        e.Cell.Row.Cells["vcServerAddress"].Value = selectedHost.vcAddress;
                        e.Cell.Row.Cells["vcServerType"].Value = selectedHost.ServerType;
                        if (!selectedHost.vcName.Equals(e.Cell.Value))
                        {
                            e.Cell.Row.Cells["vmUID"].Value = DBNull.Value;
                            e.Cell.Row.Cells["vmName"].Value = DBNull.Value;
                            e.Cell.Row.Cells["vmDomainName"].Value = DBNull.Value;
                        }
                    }
                }
                e.Cell.Row.Appearance.BackColor = BACKCOLOR_CHANGED;
                btnOK.Enabled = true;
            }
            else if (e.Cell.Column.Key == "vmName")
            {
                //if (e.Cell.Value.ToString() == e.Cell.Text ||
                //    ((e.Cell.Value == DBNull.Value || e.Cell.Value.ToString() == VALUE_EMPTY) &&
                //        (e.Cell.Text == VALUE_EMPTY || e.Cell.Text == VALUE_SELECT_VM)))
                //    return;
                //if (e.Cell.Value.ToString() == e.Cell.Text ||
                //    (e.Cell.Value == DBNull.Value && e.Cell.Value.ToString() == VALUE_EMPTY))
                //    return;
                if (e.Cell.Value.ToString() == e.Cell.Text)
                    return;


                if (e.Cell.Text == VALUE_EMPTY ||
                    e.Cell.Text == VALUE_SELECT_VM)
                {
                    e.Cell.Value = DBNull.Value;
                    e.Cell.Row.Cells["vmUID"].Value = DBNull.Value;
                    e.Cell.Row.Cells["vmDomainName"].Value = DBNull.Value;
                }
                else
                {
                    Dictionary<string, basicVMInfo> vms;
                    if (vmByVC.TryGetValue(e.Cell.Row.Cells["vcServerAddress"].Text, out vms))
                    {
                        string cellValue = string.Empty;
                        if (e.Cell.ValueListResolved.SelectedItemIndex > -1)
                            cellValue = (string)e.Cell.ValueListResolved.GetValue(e.Cell.ValueListResolved.SelectedItemIndex);
                        else
                            cellValue = e.Cell.Text;
                        foreach (basicVMInfo vm in vms.Values)
                        {
                            if (vm.VMName.Equals(cellValue))
                            {
                                e.Cell.Row.Cells["vmUID"].Value = vm.InstanceUUID;
                                e.Cell.Row.Cells["vmDomainName"].Value = vm.DomainName;
                                break;
                            }
                        }
                    }
                }
                e.Cell.Row.Appearance.BackColor = BACKCOLOR_CHANGED;
                btnOK.Enabled = true;
            }
        }

        private void vmGrid_CellDataError(object sender, CellDataErrorEventArgs e)
        {
            e.RaiseErrorEvent = false;

            if (this.ActiveControl == btnCancel)
            {
                e.StayInEditMode = false;
                e.RestoreOriginalValue = true;
            }
            else
            {
                e.StayInEditMode = true;

                string val = "value";

                if (vmGrid.ActiveCell != null &&
                    (vmGrid.ActiveCell.Column.Key == "vcServerName" || vmGrid.ActiveCell.Column.Key == "vmName"))
                    val = vmGrid.ActiveCell.Column.Header.Caption;

                ApplicationMessageBox.ShowError(this, string.Format("Please select a valid {0} from the list.", val));
            }
        }

        private void vmGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = vmGrid.DisplayLayout.Bands[0];

            // set the validation in code because it won't stick in the designer
            band.Columns["vcServerName"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownValidate;
            band.Columns["vmName"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownValidate;
        }

        #endregion

        #endregion

        #region Threading Methods for working with vCenters

        private void vCenterWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            RetrieveVmResult result = new RetrieveVmResult();
            result.serverType = "Unknown";
            try
            {
                Pair<vCenterConnectAction, vCenterStatus> workItem = (Pair<vCenterConnectAction, vCenterStatus>)e.Argument;

                result.Action = workItem.First;
                result.Host = workItem.Second;

                ServiceUtil vimService = new ServiceUtil(result.Host.vcAddress);
                vimService.Connect(result.Host.vcUser, result.Host.vcPassword);

                switch (result.Action)
                {
                    case vCenterConnectAction.GatherVMs:
                    case vCenterConnectAction.GatherAllVMs:
                        // Filter should be set up as <"property", "value">
                        Dictionary<string, string> filter = new Dictionary<string, string>();

                        result.VmInfo = vimService.GetListOfVMs(filter);
                        break;
                    case vCenterConnectAction.Connect:
                        break;
                }

                try
                {
                    vimService.Disconnect();
                }
                catch { }

                result.Result = vCenterTestStatus.Success;
                result.serverType = "vCenter";
            }
            catch (Exception ex)
            {
                result.Result = vCenterTestStatus.Failed;
                result.Exception = ex;
            }

            if (result.Result == vCenterTestStatus.Failed && result.Exception.GetType().Name == "WebException")
            {
                // connection for HyperV server
                try
                {
                    HyperVService hyperVService = new HyperVService(result.Host.vcAddress);
                    hyperVService.Connect(result.Host.vcUser, result.Host.vcPassword);
                    switch (result.Action)
                    {
                        case vCenterConnectAction.GatherVMs:
                        case vCenterConnectAction.GatherAllVMs:
                            // Filter should be set up as <"property", "value">
                            Dictionary<string, string> filter = new Dictionary<string, string>();

                            result.VmInfo = hyperVService.GetListOfVMs(filter);
                            break;
                        case vCenterConnectAction.Connect:
                            break;
                    }
                    result.Result = vCenterTestStatus.Success;
                    result.serverType = "HyperV";
                }
                catch (Exception exp)
                {
                    result.Result = vCenterTestStatus.Failed;
                    if (exp.GetType().Name == "COMException")
                    {
                        Log.Error("The remote name could not be resolved: '" + result.Host.vcAddress + "'.", exp);
                        e.Result = new COMException("The remote name could not be resolved: '" + result.Host.vcAddress + "'.");
                    }
                    else if (exp.GetType().Name == "UnauthorizedAccessException")
                    {
                        Log.Error("cannot complete login due to an incorrect username or password.", exp);
                        e.Result = new UnauthorizedAccessException("cannot complete login due to an incorrect username or password.");
                    }
                    else
                    {
                        Log.Error("Unable to connect to the remote server.", exp);
                        e.Result = new Exception("Unable to connect to the remote server.");
                    }
                }
            }
            e.Result = result;
            e.Cancel = vCenterWorker.CancellationPending;
        }

        private void vCenterWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || this.IsDisposed)
            {
                vCenterConnectionLabel.Text = string.Empty;
                updateStatusProgressBar(false);
            }
            else
            {
                if (e.Result is RetrieveVmResult)
                {
                    RetrieveVmResult result = (RetrieveVmResult)e.Result;
                    vCenterStatus hostStatus;
                    switch (result.Result)
                    {
                        case vCenterTestStatus.Success:
                            if (vcDict.TryGetValue(result.Host.vcAddress, out hostStatus))
                            {
                                switch (result.Action)
                                {
                                    case vCenterConnectAction.GatherAllVMs:
                                    case vCenterConnectAction.GatherVMs:
                                        vmByVC.Remove(result.Host.vcAddress);
                                        vmByVC.Add(result.Host.vcAddress, result.VmInfo);
                                        hostStatus.VmCount = result.VmInfo.Count;
                                        hostStatus.Status = vCenterTestStatus.Success;
                                        hostStatus.ServerType = result.serverType;
                                        updateLinkedVMNames(result.Host, result.VmInfo);
                                        break;
                                    case vCenterConnectAction.Connect:
                                        // if previous status was failed and it is now successful, then get the VMs again
                                        if (hostStatus.PriorStatus == vCenterTestStatus.Failed)
                                        {
                                            Pair<vCenterConnectAction, vCenterStatus> workItem = new Pair<vCenterConnectAction, vCenterStatus>(vCenterConnectAction.GatherVMs, hostStatus);
                                            vCenterConnectionLabel.Text = string.Format(STATUS_FMT_GATHERING_VMS, hostStatus.vcName);

                                            vCenterWorker.RunWorkerAsync(workItem);
                                            return;
                                        }
                                        else
                                        {
                                            hostStatus.Status = vCenterTestStatus.Success;
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                // In case it was deleted while it was processing
                                vmByVC.Remove(result.Host.vcAddress);
                            }
                            break;
                        case vCenterTestStatus.Failed:
                            Log.ErrorFormat(ERROR_FMT_GATHERING_VMS, result.Host.vcAddress, result.Exception);

                            if (result.Action == vCenterConnectAction.GatherAllVMs || result.Action == vCenterConnectAction.GatherVMs)
                            {
                                vmByVC.Remove(result.Host.vcAddress);
                            }
                            if (vcDict.TryGetValue(result.Host.vcAddress, out hostStatus))
                            {
                                hostStatus.VmCount = -1;
                                hostStatus.Status = vCenterTestStatus.Failed;
                                hostStatus.StatusMessage = string.Format(ERROR_FMT_GATHERING_VMS, result.Host.vcAddress, result.Exception.Message);
                            }
                            else
                            {
                                // In case it was deleted while it was processing
                                vmByVC.Remove(result.Host.vcAddress);
                            }
                            break;
                        default:
                            Log.ErrorFormat("Unexpected result received retrieving VM List: {0}", result.Result.ToString());

                            if (vcDict.TryGetValue(result.Host.vcAddress, out hostStatus))
                            {
                                hostStatus.Status = vCenterTestStatus.Failed;
                                hostStatus.StatusMessage = string.Format("Unexpected result received when retrieving VM List. No VMs are currently available.", result.Host.vcAddress, result.Exception);
                            }
                            else
                            {
                                // In case it was deleted while it was processing
                                vmByVC.Remove(result.Host.vcAddress);
                            }
                            break;
                    }
                }

                if (!this.IsDisposed)
                {
                    if (vcConnectList.Count > 0)
                    {
                        Pair<vCenterConnectAction, vCenterStatus> workItem = vcConnectList[0];
                        vcConnectList.RemoveAt(0);
                        switch (workItem.First)
                        {
                            case vCenterConnectAction.GatherAllVMs:
                                vCenterConnectionLabel.Text = STATUS_GATHERING_ALL_VMS;
                                break;
                            case vCenterConnectAction.GatherVMs:
                                vCenterConnectionLabel.Text = string.Format(STATUS_FMT_GATHERING_VMS, workItem.Second.vcName);
                                break;
                            case vCenterConnectAction.Connect:
                                vCenterConnectionLabel.Text = string.Format(STATUS_FMT_TEST, workItem.Second.vcName);
                                break;
                        }
                        vCenterWorker.RunWorkerAsync(workItem);
                    }
                    else
                    {
                        updateVmActions(null);
                        vCenterConnectionLabel.Text = string.Empty;
                        updateStatusProgressBar(false);
                    }

                    vcGrid.Rows.Refresh(RefreshRow.ReloadData);
                }
            }
        }
        #endregion

        #region Helper Methods

        private void showHelp()
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.VirtualizationConfig);
        }

        private void initializeDataSource()
        {
            serverLinksDataTable = new DataTable();
            serverLinksDataTable.Columns.Add("selected", typeof(bool));
            serverLinksDataTable.Columns.Add("monitoredServerID", typeof(int));
            serverLinksDataTable.Columns.Add("instanceName", typeof(string));
            serverLinksDataTable.Columns.Add("vcHostID", typeof(int));
            serverLinksDataTable.Columns.Add("vcServerAddress", typeof(string));
            serverLinksDataTable.Columns.Add("vcServerName", typeof(string));
            serverLinksDataTable.Columns.Add("vmUID", typeof(string));
            serverLinksDataTable.Columns.Add("vmName", typeof(string));
            serverLinksDataTable.Columns.Add("vmDomainName", typeof(string));
            serverLinksDataTable.Columns.Add("vcServerType", typeof(string));
        }

        private void setupDataSource()
        {
            vmGrid.DataSource = serverLinksDataTable;

            //Get List of vCenter Servers
            Dictionary<string, vCenterHosts> hosts = RepositoryHelper.GetVirtualHostServers(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, null);

            vcDict = new Dictionary<string,vCenterStatus>();
            vcList = new List<vCenterStatus>();

            vcEditDict = new Dictionary<string, vCenterHosts>();

            foreach (KeyValuePair<string, vCenterHosts> host in hosts)
            {
                vCenterStatus vcStatus = new vCenterStatus(host.Value);
                try
                {
                    vcDict.Add(host.Key, vcStatus);
                    vcList.Add(vcStatus);
                }
                catch 
                {
                }
            }
            if (vcList.Count > 0)
            {
                vcGrid.DataSource = vcList;
            }

            vmByVC = new Dictionary<string, Dictionary<string, basicVMInfo>>();

            refreshControls(RefreshRow.ReloadData);
        }

        private void updateLinkedVMNames(vCenterHosts host, Dictionary<string, basicVMInfo> vmInfos)
        {
            int hostID;
            string vmUID;
            string vmName;
            string vmDomainName;
            bool changes = false;

            foreach (UltraGridRow gridRow in vmGrid.Rows)
            {
                DataRow row = ((DataRowView)gridRow.ListObject).Row;
                if (row["vcHostID"] == DBNull.Value)
                    continue;

                hostID = (int)row["vcHostID"];
                if (hostID == host.vcHostID && row["vmUID"] != DBNull.Value)
                {
                    vmUID = (string)row["vmUID"];
                    vmName = row["vmName"] == DBNull.Value ? null : (string)row["vmName"];
                    vmDomainName = row["vmDomainName"] == DBNull.Value ? null : (string)row["vmDomainName"];

                    basicVMInfo vmInfo;
                    if (vmInfos.TryGetValue(vmUID, out vmInfo))
                    {
                        if ((!vmName.Equals(vmInfo.VMName) && !String.IsNullOrEmpty(vmInfo.VMName)) ||
                            (!vmDomainName.Equals(vmInfo.DomainName) && !String.IsNullOrEmpty(vmInfo.DomainName)))
                        {
                            row["vmName"] = vmInfo.VMName;
                            row["vmDomainName"] = vmInfo.DomainName;
                            gridRow.Appearance.BackColor = BACKCOLOR_CHANGED;
                            changes = true;
                        }
                        if (row["vcServerType"] != host.ServerType && !String.IsNullOrEmpty(host.ServerType))
                        {
                            row["vcServerType"] = host.ServerType;
                        }
                    }
                }
            }

            if (changes)
            {
                refreshControls(RefreshRow.RefreshDisplay);
                ApplicationMessageBox.ShowInfo(this, string.Format("Virtual machine names for one or more instances linked to {0} have changed and the links have been updated. Review the links to confirm the changes and click OK to save.", host.vcName));
                btnOK.Enabled = true;
            }
        }

        private void linkVMsToServers()
        {
            string server;
            string vmUID;
            string machineName = null;
            int hostID;

            // get the real server names to match up against for aliases and IP addresses
            DataTable serverHostNames = RepositoryHelper.GetServerHostNames(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            foreach (UltraGridRow gridRow in vmGrid.Rows)
            {
                DataRow row = ((DataRowView)gridRow.ListObject).Row;
                bool selected = (bool)row["selected"];
                if (selected)
                {
                    server = row["vcServerAddress"] == DBNull.Value ? null : (string)row["vcServerAddress"];
                    vmUID = row["vmUID"] == DBNull.Value ? null : (string)row["vmUID"];
                    machineName = null;
                    hostID = row["vcHostID"] == DBNull.Value ? -1 : (int)row["vcHostID"];

                    int serverId = (int)row["monitoredServerID"];
                    if (serverId >= 0)
                    {
                        DataRow[] serverRow = serverHostNames.Select(string.Format("SQLServerID = '{0}'", serverId));
                        if (serverRow.Length > 0)
                            machineName = serverRow[0]["ServerHostName"] == DBNull.Value ? null : (string)serverRow[0]["ServerHostName"];
                    }
                    if (machineName == null)
                    {
                        if (row["instanceName"].ToString().Contains("\\"))
                        {
                            string[] nameArray = row["instanceName"].ToString().Split(new char[] { '\\' });
                            machineName = nameArray[0];
                        }
                        else
                            machineName = row["instanceName"].ToString();
                    }
                    bool found = false;
                    foreach (Dictionary<string, basicVMInfo> hostList in vmByVC.Values)
                    {
                        foreach (basicVMInfo vm in hostList.Values)
                        {
                            if (vm.MachineName.Equals(machineName, StringComparison.CurrentCultureIgnoreCase) ||
                                vm.VMName.Equals(machineName, StringComparison.CurrentCultureIgnoreCase))
                            {
                                found = true;
                                vCenterStatus host = vcDict[vm.HostAddress];
                                if (host.vcHostID != (row["vcHostID"] == DBNull.Value ? -1 : (int)row["vcHostID"]) ||
                                    host.vcAddress != (row["vcServerAddress"] == DBNull.Value ? null : (string)row["vcServerAddress"]) ||
                                    host.vcName != (row["vcServerName"] == DBNull.Value ? null : (string)row["vcServerName"]) ||
                                    vm.InstanceUUID != (row["vmUID"] == DBNull.Value ? null : (string)row["vmUID"]) ||
                                    vm.VMName != (row["vmName"] == DBNull.Value ? null : (string)row["vmName"]) ||
                                    vm.DomainName != (row["vmDomainName"] == DBNull.Value ? null : (string)row["vmDomainName"]))
                                {
                                    row["vcHostID"] = host.vcHostID;
                                    row["vcServerAddress"] = host.vcAddress;
                                    row["vcServerName"] = host.vcName;
                                    row["vmUID"] = vm.InstanceUUID;
                                    row["vmName"] = vm.VMName;
                                    row["vmDomainName"] = vm.DomainName;
                                    row["vcServerType"] = host.ServerType;
                                    gridRow.Appearance.BackColor = BACKCOLOR_CHANGED;
                                    btnOK.Enabled = true;
                                }
                                break;
                            }
                        }

                        if (found)
                            break;
                    }
                }
            }
            refreshControls(RefreshRow.RefreshDisplay);
        }

        private void updateStatusProgressBar(bool running)
        {
            if (running)
            {
                statusProgressBar.Start();
                statusProgressBar2.Start();
            }
            else
            {
                statusProgressBar.Stop();
                statusProgressBar2.Stop();
            }
        }

        private void updateVCenterButtons()
        {
            bool enable = vcGrid.Selected.Rows.Count > 0;

            btnEdit.Enabled = enable && ApplicationModel.Default.UserToken.IsSQLdmAdministrator;
            btnDelete.Enabled = allowDelete && enable && ApplicationModel.Default.UserToken.IsSQLdmAdministrator;
            btnTest.Enabled = enable;
            if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator)
            {
                btnEdit.Visible = true;
                btnDelete.Visible = true;
            }
            else
            {
                btnEdit.Visible = false;
                btnDelete.Visible = false;
            }

            // Update the empty list label for vcGrid.
            DisplayNovCenterInstancesLabel();
        }

        private void updateVmActions(UltraGridCell currentCell)
        {
            if (vCenterWorker.IsBusy)
            {
                btnLink.Enabled = false;
                selectAllServersCheckBox.Enabled = false;
                vmGrid.DisplayLayout.Bands[0].Columns["selected"].CellActivation = Activation.Disabled;
                vmGrid.DisplayLayout.Bands[0].Columns["vcServerName"].CellActivation =
                    vmGrid.DisplayLayout.Bands[0].Columns["vmName"].CellActivation = Activation.NoEdit;
                vmGrid.DisplayLayout.Override.AllowUpdate = DefaultableBoolean.False;
                return;
            }

            selectAllServersCheckBox.Enabled = true;
            vmGrid.DisplayLayout.Bands[0].Columns["selected"].ResetCellActivation();
            vmGrid.DisplayLayout.Bands[0].Columns["vcServerName"].ResetCellActivation();
            vmGrid.DisplayLayout.Bands[0].Columns["vmName"].ResetCellActivation();
            vmGrid.DisplayLayout.Override.ResetAllowUpdate();

            // if updating from the grid, it passes in the cell to get the Text and row for comparison 
            // because the cell value update doesn't happen until the row is committed
            if (currentCell != null && bool.Parse(currentCell.Text) && vcGrid.Rows.Count > 0)
            {
                btnLink.Enabled = true;
                return;
            }
            else
            {
                foreach (UltraGridRow row in vmGrid.Rows)
                {
                    if ((currentCell == null || !currentCell.Row.Equals(row)) &&
                        bool.Parse(row.Cells["selected"].Text) == true && vcGrid.Rows.Count > 0)
                    {
                        btnLink.Enabled = true;
                        return;
                    }
                }
            }
            btnLink.Enabled = false;

            // Update empty list label for vmGrid.
            DisplayNoVirtualMachinesLabel();
        }

        private void refreshControls(RefreshRow method)
        {
            vmGrid.Rows.Refresh(method);
            vcGrid.Rows.Refresh(method);
            // Update empty list labels for the gridviews.
            DisplayNovCenterInstancesLabel();
            DisplayNoVirtualMachinesLabel();
        }

        /// <summary>
        /// Display the empty list label, if the vcGrid does not contains elements to show.
        /// </summary>
        private void DisplayNovCenterInstancesLabel()
        {
            if (vcGrid.Rows.Count > 0)
            {
                lblVcNoInstances.Visible = false;
                lblVcNoInstances.SendToBack();
            }
            else
            {
                lblVcNoInstances.Visible = true;
                lblVcNoInstances.BringToFront();
            }
        }

        /// <summary>
        /// Display the empty list label, if the vmGrid does not contains elements to show.
        /// </summary>
        private void DisplayNoVirtualMachinesLabel()
        {
            if (vmGrid.Rows.Count > 0)
            {
                lblNoVirtualMachines.Visible = false;
                lblNoVirtualMachines.SendToBack();
            }
            else
            {
                lblNoVirtualMachines.Visible = true;
                lblNoVirtualMachines.BringToFront();
            }
        }

        private DialogResult showAddEditDialog(DialogAction action, vCenterHosts host)
        {
            DialogResult result = DialogResult.None;

            // save the original host address in case they change it when editing
            string hostAddress = host.vcAddress;

            Dictionary<string, vCenterHosts> hosts = new Dictionary<string, vCenterHosts>();
            foreach (KeyValuePair<string, vCenterStatus> vCenter in vcDict)
            {
                hosts.Add(vCenter.Key, vCenter.Value as vCenterHosts);
            }

            AddEditvCenterConnection vCenterDialog = new AddEditvCenterConnection(host, hosts);

            vCenterHosts oldValue = new vCenterHosts(host.vcHostID, host.vcName, host.vcAddress, host.vcUser, host.vcEncryptedPassword, host.ServerType);
            result = vCenterDialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                // If edit, update the vm grid and clear the list of VMs for this server 
                if (action == DialogAction.Edit)
                {
                    DataRow[] linkedRows = serverLinksDataTable.Select(string.Format("vcServerAddress = '{0}'", hostAddress));
                    foreach (DataRow row in linkedRows)
                    {
                        row["vcHostID"] = host.vcHostID;
                        row["vcServerName"] = host.vcName;
                        row["vcServerAddress"] = host.vcAddress;
                        row["vcServerType"] = host.ServerType;
                    }

                    if (vcEditDict.ContainsKey(host.vcAddress))
                    {
                        vcEditDict.Remove(host.vcAddress);
                        vcEditDict.Add(host.vcAddress, oldValue);
                    }
                    else
                    {
                        vcEditDict.Add(host.vcAddress, oldValue);
                    }

                    vmByVC.Remove(hostAddress);
                    if (hostAddress != host.vcAddress)
                    {
                        vcDict.Remove(hostAddress);
                        foreach (vCenterStatus vcStat in vcList)
                        {
                            if (vcStat.vcAddress == host.vcAddress)
                            {
                                vcList.Remove(vcStat);
                                break;
                            }
                        }
                    }
                }

                vCenterStatus hostStatus;
                if (!vcDict.TryGetValue(host.vcAddress, out hostStatus))
                {
                    hostStatus = new vCenterStatus(host);
                    vcDict.Add(host.vcAddress, hostStatus);
                    vcList.Add(hostStatus);
                    vcGrid.DataSource = vcList;
                }

                hostStatus.Status = vCenterTestStatus.Testing;

                Pair<vCenterConnectAction, vCenterStatus> workItem = new Pair<vCenterConnectAction, vCenterStatus>(vCenterConnectAction.GatherVMs, hostStatus);
                if (vCenterWorker.IsBusy)
                {
                    vcConnectList.Add(workItem);
                }
                else
                {
                    updateStatusProgressBar(true);
                    vCenterConnectionLabel.Text = string.Format(STATUS_FMT_GATHERING_VMS, hostStatus.vcName);
                    vCenterWorker.RunWorkerAsync(workItem);
                    vcGrid.Rows.Refresh(RefreshRow.ReloadData);
                    updateVmActions(null);
                }
                btnOK.Enabled = true;
            }

            return result;
        }

        public void AddServer(int sqlServerId, string instance)
        {
            this.AddLinkedServer(sqlServerId, instance, -1, null, null, null, null, null);
        }

        public void AddLinkedServer(int sqlServerId, string instance, int hostId, string serverAddress, string serverName, string vUID, string vmName, string vmDomainName)
        {
            serverLinksDataTable.Rows.Add(new object[] { false, sqlServerId, instance, hostId, serverAddress, serverName, vUID, vmName ?? string.Empty, vmDomainName ?? string.Empty});

        }

        public VirtualizationConfiguration GetVirtualizationConfig(string instance)
        {
            DataRow[] row = serverLinksDataTable.Select("instanceName = '" + instance.Trim() + "'" );

            VirtualizationConfiguration vcConfig = null;

            if (row.Length != 0 && row[0]["vmUID"] != DBNull.Value)
            {
                vCenterStatus hostStatus = vcDict[(string)row[0]["vcServerAddress"]];
                // create a vCenterHosts object instead of passing back the vCenterStatus which isn't serializable or understoond outside of this dialog
                vCenterHosts host = new vCenterHosts(hostStatus.vcHostID, hostStatus.vcName, hostStatus.vcAddress, hostStatus.vcUser, hostStatus.vcEncryptedPassword, hostStatus.ServerType);
                vcConfig = new VirtualizationConfiguration((string)row[0]["vmUID"], (string)row[0]["vmName"], (string)row[0]["vmDomainName"], host);
            }

            return vcConfig;
        }

        #endregion

        private enum DialogAction
        {
            Add,
            Edit
        }

        private enum vCenterConnectAction
        {
            GatherAllVMs,
            Connect,
            GatherVMs
        }

        private enum vCenterTestStatus
        {
            Unknown,
            Testing,
            Success,
            Failed
        }

        private class vCenterStatus : vCenterHosts
        {
            private vCenterTestStatus status = vCenterTestStatus.Unknown;
            private vCenterTestStatus priorStatus;

            public vCenterStatus(vCenterHosts host)
            {
                this.vcHostID = host.vcHostID;
                this.vcName = host.vcName;
                this.vcAddress = host.vcAddress;
                this.vcUser = host.vcUser;
                this.vcEncryptedPassword = host.vcEncryptedPassword;
                this.ServerType = host.ServerType;

                this.Status = vCenterTestStatus.Unknown;
                this.StatusMessage = string.Empty;
                this.VmCount = null;
            }

            public int? VmCount { get; set; }
            public string VmCountDisplay
            {
                get
                {
                    if (VmCount.HasValue)
                        return VmCount.Value == -1 ? "unknown" : VmCount.Value.ToString("N0");
                    else
                        return "retrieving...";
                }
            }

            public vCenterTestStatus PriorStatus { get { return priorStatus; } }
            public vCenterTestStatus Status
            {
                get { return status; }
                set
                {
                    priorStatus = status;
                    status = value;
                    switch (status)
                    {
                        case vCenterTestStatus.Unknown:
                            StatusMessage = @"Connection status unknown.\n\nSelect this Virtualization Host and click the Test button to test the connection.";
                            break;
                        case vCenterTestStatus.Testing:
                            StatusMessage = @"Testing connection...";
                            break;
                        case vCenterTestStatus.Success:
                            StatusMessage = @"Successfully connected to the Virtualization Host server";
                            break;
                        case vCenterTestStatus.Failed:
                            StatusMessage = @"Error connecting to the Virtualization Host server";
                            break;
                    }
                }
            }
            public string StatusMessage { get; set; }
        }

        private class RetrieveVmResult
        {
            public vCenterConnectAction Action;
            public vCenterHosts Host;
            public vCenterTestStatus Result = vCenterTestStatus.Unknown;
            public Dictionary<string, basicVMInfo> VmInfo;
            public Exception Exception = null;
            public string serverType =  "Unknown";
        }

        #region ToolTipItemCreationFilter for displaying the status on the vCenter connection status image

        /// <summary>
        /// A CreationFilter that will trap for elements within the UltraGrid
        /// and set tooltip information on them.
        /// </summary>
        public class ToolTipItemCreationFilter : IUIElementCreationFilter
        {
            private IToolTipItem toolTipItem = null;

            void IUIElementCreationFilter.AfterCreateChildElements(Infragistics.Win.UIElement parent)
            {
                if (parent is CellUIElement && ((CellUIElement)parent).Column.Key.Equals("Status"))
                {
                    parent.ToolTipItem = this.ToolTipItem;
                }
            }

            bool IUIElementCreationFilter.BeforeCreateChildElements(Infragistics.Win.UIElement parent)
            {
                // Do nothing here.
                return false;
            }

            #region ToolTipItem
            /// <summary>
            /// This single instance of a TreeToolTipItem will be assigned to 
            /// any UIElement in the tree that needs to display a tooltip. The TreeToolTipItem
            /// class will determine what tooltip to display based on the element. 
            /// </summary>
            private IToolTipItem ToolTipItem
            {
                get
                {
                    if (this.toolTipItem == null)
                        this.toolTipItem = new vCenterToolTipItem();

                    return this.toolTipItem;
                }
            }
            #endregion ToolTipItem
        }

        /// <summary>
        /// This class provides tooltip information based on the type of UIElement
        /// </summary>
        public class vCenterToolTipItem : IToolTipItem
        {
            ToolTipInfo IToolTipItem.GetToolTipInfo(Point mousePosition, UIElement element, UIElement previousToolTipElement, ToolTipInfo toolTipInfoDefault)
            {
                if (element is CellUIElement)
                {
                    UltraGridCell cell = ((CellUIElement)element).Cell;

                    if (cell != null)
                    {
                        if (cell.Row.IsDataRow && cell.Column.Key.Equals("Status"))
                        {
                            toolTipInfoDefault.ToolTipText = cell.Row.Cells["StatusMessage"].Text;
                        }

                        // Position the tooltip over the node
                        toolTipInfoDefault.Location = mousePosition;
                    }
                }

                return toolTipInfoDefault;
            }
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
        #endregion ToolTipItemCreationFilter
    }
}
