using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview
{
    internal partial class ServerConfigurationView : ServerBaseView
    {
        private DateTime? historicalSnapshotDateTime;
        private bool initialized = false;
        private UltraGridColumn selectedColumn = null;
        private Control focused = null;
        private readonly Dictionary<string, UltraDataRow> rowLookupTable = new Dictionary<string, UltraDataRow>();
        private static readonly object updateLock = new object();

        //last Settings values used to determine if changed for saving when leaving
        private int lastSplitterDistance = 0;
        private GridSettings lastMainGridSettings = null;
        private bool lastDetailsVisible = true;

        public event EventHandler DetailsPanelVisibleChanged;
        public event EventHandler GridGroupByBoxVisibleChanged;

        public ServerConfigurationView(int instanceId) : base(instanceId)
        {
            InitializeComponent();
            AdaptFontSize();
            configurationGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeUnsupportedViewLabel;
        }

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set { historicalSnapshotDateTime = value; }
        }

        public bool DetailsPanelVisible
        {
            get { return !splitContainer.Panel2Collapsed; }
            set
            {
                splitContainer.Panel2Collapsed = !value;

                if (DetailsPanelVisibleChanged != null)
                {
                    DetailsPanelVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool GridGroupByBoxVisible
        {
            get { return !configurationGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                configurationGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (GridGroupByBoxVisibleChanged != null)
                {
                    GridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ServerConfigurationView);
        }

        public override void ApplySettings()
        {
            // Fixed panel is second panel, so restore size of second panel
            lastSplitterDistance = splitContainer.Height - Settings.Default.ServerConfigurationViewMainSplitter;
            if (lastSplitterDistance > 0)
            {
                splitContainer.SplitterDistance = lastSplitterDistance;
            }
            else
            {
                lastSplitterDistance = splitContainer.Height - splitContainer.SplitterDistance;
            }

            lastDetailsVisible =
                DetailsPanelVisible = Settings.Default.ServerConfigurationViewDetailsPanelVisible;

            if (Settings.Default.ServerConfigurationViewMainGrid is GridSettings)
            {
                lastMainGridSettings = Settings.Default.ServerConfigurationViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, configurationGrid);
                // force a change to GroupByBox so ribbon stays in sync
                GridGroupByBoxVisible = GridGroupByBoxVisible;
            }
        }

        public override void SaveSettings()
        {
            GridSettings gridSettings = GridSettings.GetSettings(configurationGrid);
            // save all settings only if anything has changed
            if (lastSplitterDistance != splitContainer.Height - splitContainer.SplitterDistance
                || lastDetailsVisible != DetailsPanelVisible
                || !gridSettings.Equals(lastMainGridSettings))
            {
                // Fixed panel is second panel, so save size of second panel
                lastSplitterDistance =
                    Settings.Default.ServerConfigurationViewMainSplitter = splitContainer.Height - splitContainer.SplitterDistance;
                lastDetailsVisible =
                    Settings.Default.ServerConfigurationViewDetailsPanelVisible = DetailsPanelVisible;
                lastMainGridSettings =
                    Settings.Default.ServerConfigurationViewMainGrid = gridSettings;
            }
        }

        public override void UpdateUserTokenAttributes()
        {
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
            {
                editConfigValueLinkLabel.Visible = false;
            }
            else
            {
                editConfigValueLinkLabel.Visible = ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
            }
        }

        public override void RefreshView()
        {
            if (HistoricalSnapshotDateTime == null)
            {
                ServerConfigurationView_Fill_Panel.Visible = true;
                base.RefreshView();
            }
            else
            {
                ServerConfigurationView_Fill_Panel.Visible = false;
                ApplicationController.Default.SetRefreshStatusText(Properties.Resources.HistoryModeStatusBarLabel);
            }
        }

        public override object DoRefreshWork()
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            OnDemandConfiguration configuration = new OnDemandConfiguration(instanceId);
            return (Snapshot)managementService.GetConfiguration(configuration);
        }

        public override void UpdateData(object data)
        {
            if (data != null && data is ConfigurationSnapshot)
            {
                lock (updateLock)
                {
                    ConfigurationSnapshot snapshot = data as ConfigurationSnapshot;

                    if (snapshot.Error == null)
                    {
                        foreach (DataRow row in snapshot.ConfigurationSettings.Rows)
                        {
                            UltraDataRow existingRow;

                            if (rowLookupTable.TryGetValue((string)row["Name"], out existingRow))
                            {
                                existingRow["Run Value"] = row["Run Value"];
                                existingRow["Config Value"] = row["Config Value"];
                            }
                            else
                            {
                                UltraDataRow newRow = configurationGridDataSource.Rows.Add(
                                    new object[]
                                        {
                                            row["Name"],
                                            row["Run Value"],
                                            row["Config Value"],
                                            row["Minimum"],
                                            row["Maximum"],
                                            (bool) row["Restart Required"] ? "TRUE" : "FALSE",
                                            row["Comment"]
                                        });

                                rowLookupTable.Add((string)row["Name"], newRow);
                            }
                        }

                        if (!initialized)
                        {
                            if (lastMainGridSettings != null)
                            {
                                GridSettings.ApplySettingsToGrid(lastMainGridSettings, configurationGrid);

                                initialized = true;
                            }
                            else if (snapshot.ConfigurationSettings.Rows.Count > 0)
                            {
                                foreach (UltraGridColumn column in configurationGrid.DisplayLayout.Bands[0].Columns)
                                {
                                    column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                }

                                initialized = true;
                            }
                        }

                        ApplicationController.Default.SetCustomStatus(
                            String.Format("Server Configuration: {0} Item{1}",
                                    snapshot.ConfigurationSettings.Rows.Count,
                                    snapshot.ConfigurationSettings.Rows.Count == 1 ? string.Empty : "s")
                            );

                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now));

                        // To update Edit Link Settings on SysAdmin
                        serverConfig_SYSADMIN_Check();
                    }
                    else
                    {
                        serverConfig_SYSADMIN_Check();
                        ApplicationController.Default.ClearCustomStatus();
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now, snapshot.Error));
                    }
                }
            }
            else
            {
                serverConfig_SYSADMIN_Check();
                ApplicationController.Default.ClearCustomStatus();
            }
        }

        private void configurationGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if (configurationGrid.Selected.Rows.Count > 0 && configurationGrid.Selected.Rows[0].Cells != null)
            {
                detailsContentPanelDescriptionLabel.Visible = false;
                detailsLayoutPanel.Visible = true;
            }
            else
            {
                detailsContentPanelDescriptionLabel.Visible = true;
                detailsLayoutPanel.Visible = false;
            }
        }

        private bool serverConfig_SYSADMIN_Check()
        {
            if (this.configurationPanelSYSAdminWarningLabel.Visible)
            {
                this.configurationPanelSYSAdminWarningLabel.Visible = false;
            }
            if (!this.splitContainer.Visible)
            {
                this.splitContainer.Visible = true;
            }
            return true;
        }

        private void configurationGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader =
                        contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;
                    ((StateButtonTool)toolbarsManager.Tools["groupByThisColumnButton"]).Checked = selectedColumn.IsGroupByColumn;
                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "columnContextMenu");
                }
                else
                {
                    contextObject = selectedElement.GetAncestor(typeof(RowUIElement));

                    if (contextObject is RowUIElement)
                    {
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;

                        toolbarsManager.Tools["showDetailsButton"].SharedProps.Visible = !DetailsPanelVisible;
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "configurationOptionContextMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
        }

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "sortAscendingButton":
                    SortSelectedColumnAscending();
                    break;
                case "sortDescendingButton":
                    SortSelectedColumnDescending();
                    break;
                case "toggleGroupByBoxButton":
                    ToggleGroupByBox();
                    break;
                case "groupByThisColumnButton":
                    GroupBySelectedColumn(((StateButtonTool)e.Tool).Checked);
                    break;
                case "removeThisColumnButton":
                    RemoveSelectedColumn();
                    break;
                case "showColumnChooserButton":
                    ShowColumnChooser();
                    break;
                case "showDetailsButton":
                    DetailsPanelVisible = true;
                    break;
                case "editConfigurationValueButton":
                    EditConfigurationValue();
                    break;
                case "printGridButton":
                    PrintGrid();
                    break;
                case "exportGridButton":
                    SaveGrid();
                    break;
                case "collapseAllGroupsButton":
                    CollapseAllGroups();
                    break;
                case "expandAllGroupsButton":
                    ExpandAllGroups();
                    break;
            }
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "configurationOptionContextMenu")
            {
                if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
                {
                    ((PopupMenuTool)e.Tool).Tools["editConfigurationValueButton"].SharedProps.Visible = false;
                }
                else
                {
                    ((PopupMenuTool)e.Tool).Tools["editConfigurationValueButton"].SharedProps.Visible =
                    ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
                }                                   
                ((PopupMenuTool)e.Tool).Tools["editConfigurationValueButton"].SharedProps.Enabled =
    this.isUserSysAdmin;
            }

            if (e.Tool.Key == "configurationOptionContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = configurationGrid.Rows.Count > 0 && configurationGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
            if (e.Tool.Key == "columnContextMenu")
            {

                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(configurationGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        private void EditConfigurationValue()
        {
            if (configurationGrid.Selected.Rows.Count > 0)
            {
                string configurationOptionName = configurationGrid.Selected.Rows[0].Cells["Name"].Text;
                int minimumValue = (int)configurationGrid.Selected.Rows[0].Cells["Minimum"].Value;
                int maximumValue = (int)configurationGrid.Selected.Rows[0].Cells["Maximum"].Value;
                bool restartRequired = configurationGrid.Selected.Rows[0].Cells["Restart Required"].Text == "TRUE"
                                           ? true
                                           : false;

                EditConfigurationValueDialog dialog =
                    new EditConfigurationValueDialog(instanceId, configurationOptionName, minimumValue, maximumValue,
                                                     restartRequired);

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    ApplicationController.Default.RefreshActiveView();
                }
            }
        }

        private void PrintGrid()
        {
            ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            ultraGridPrintDocument.Header.TextLeft =
                    string.Format("{0} - Server Configuration as of {1}",
                                        ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
                                        DateTime.Now.ToString("G")
                                    );
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid()
        {
            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.FileName = "ServerConfig";
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ultraGridExcelExporter.Export(configurationGrid, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }
            }
        }

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                configurationGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                configurationGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                configurationGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                configurationGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void ToggleGroupByBox()
        {
            GridGroupByBoxVisible = !GridGroupByBoxVisible;
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    configurationGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    configurationGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
                }
            }
        }

        private void RemoveSelectedColumn()
        {
            if (selectedColumn != null)
            {
                selectedColumn.Hidden = true;
            }
        }

        private void CollapseAllGroups()
        {
            configurationGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            configurationGrid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(configurationGrid);
            dialog.Show(this);
        }

        private void hideDetailsPaneButton_Click(object sender, EventArgs e)
        {
            DetailsPanelVisible = false;
        }

        private void configurationGrid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            DetailsPanelVisible = true;
        }

        private void splitContainer_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void splitContainer_MouseUp(object sender, MouseEventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
        }

        private static Control GetFocusedControl(ControlCollection controls)
        {
            Control focusedControl = null;

            foreach (Control control in controls)
            {
                if (control.Focused)
                {
                    focusedControl = control;
                }
                else if (control.ContainsFocus)
                {
                    return GetFocusedControl(control.Controls);
                }
            }

            return focusedControl != null ? focusedControl : controls[0];
        }

        private void editConfigValueLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            EditConfigurationValue();
        }

        private void ServerConfigurationView_Load(object sender, EventArgs e)
        {
            if (serverConfig_SYSADMIN_Check())
                ApplySettings();
        }

        private void historicalSnapshotStatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }

        /// <summary>
        /// Calls the AutoScaleFontHelper to adapt the fontsize of the Form to the current OS font size configuration.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}