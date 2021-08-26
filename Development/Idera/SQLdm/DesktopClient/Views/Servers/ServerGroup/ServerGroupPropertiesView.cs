using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinMaskedEdit;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Windows.Themes;
using Microsoft.SqlServer.MessageBox;
using ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;
using Resources = Idera.SQLdm.DesktopClient.Properties.Resources;

namespace Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup
{
    internal partial class ServerGroupPropertiesView : UserControl
    {
        private static readonly Logger LOG = Logger.GetLogger("ServerGroupPropertiesView");
        private readonly Dictionary<int, List<UltraGridCell>> changedCells = new Dictionary<int, List<UltraGridCell>>();
        private readonly object sync = "lock";
        private bool initialized;

        //last Settings values used to determine if changed for saving when leaving
        private GridSettings lastMainGridSettings;
        private IList<MonitoredSqlServer> monitoredServers;

        private ServerGroupView parentView;
        private UltraGridColumn selectedColumn;
        private object view;


        public ServerGroupPropertiesView()
        {
            InitializeComponent();
            ValueListItem listItem;
            ValueList valueList = propertiesGrid.DisplayLayout.ValueLists["hasChangesValueList"];
            listItem = new ValueListItem(true, "Yes");
            listItem.Appearance.Image = Resources.data_edit;
            valueList.ValueListItems.Add(listItem);
            listItem = new ValueListItem(false, "No");
            valueList.ValueListItems.Add(listItem);

            valueList = propertiesGrid.DisplayLayout.ValueLists["yesNoValueList"];
            listItem = new ValueListItem(true, "Yes");
            valueList.ValueListItems.Add(listItem);
            listItem = new ValueListItem(false, "No");
            valueList.ValueListItems.Add(listItem);

            propertiesGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            ApplicationModel.Default.ActiveInstances.Changed += ActiveInstances_Changed;

            lblNoSqlservers.Text = "There are no items to show in this view.";
            SetGridTheme();
            UpdateCellColors();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
        }
        private void ScaleControlsAsPerResolution()
        {
            //this.saveButton.Anchor = AnchorStyles.None;
            this.undoButton.Size = new System.Drawing.Size(94, 25);
            this.saveButton.Size = new System.Drawing.Size(94, 25);
            this.columnHelp.Location = new System.Drawing.Point(10, 48);

            if (AutoScaleSizeHelper.isLargeSize)
            {
                this.saveButton.Location = new System.Drawing.Point(850, 15);
                this.undoButton.Location = new System.Drawing.Point(850, 70);
                this.columnHelp.Size = new System.Drawing.Size(700, 98);
            }
            else if (AutoScaleSizeHelper.isXLargeSize)
            {
                this.saveButton.Location = new System.Drawing.Point(1350, 15);
                this.undoButton.Location = new System.Drawing.Point(1350, 70);
                this.columnHelp.Size = new System.Drawing.Size(1200, 98);
            }
            else if (AutoScaleSizeHelper.isXXLargeSize)
            {
                this.saveButton.Location = new System.Drawing.Point(1550, 15);
                this.undoButton.Location = new System.Drawing.Point(1550, 70);
                this.columnHelp.Size = new System.Drawing.Size(1450, 98);
            }
        }

        private void ActiveInstances_Changed(object sender, MonitoredSqlServerCollectionChangedEventArgs e)
        {
            ApplicationController.Default.RefreshActiveView();
        }

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                    //
                    // Column context menu
                    //
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
                    GroupBySelectedColumn(((StateButtonTool) e.Tool).Checked);
                    break;
                case "removeThisColumnButton":
                    RemoveSelectedColumn();
                    break;
                case "showColumnChooserButton":
                    ShowColumnChooser();
                    break;

                    //
                    // Instance context menu
                    //
                case "openInstanceButton":
                    OpenSelectedInstance();
                    break;
                case "refreshInstanceButton":
                    RefreshSelectedInstance();
                    break;
                case "deleteInstanceButton":
                    DeleteSelectedInstance();
                    break;
                case "showInstancePropertiesButton":
                    ShowInstanceProperties(GetSelectedInstanceId());
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
            toolbarsManager.Tools["deleteInstanceButton"].SharedProps.Visible =
                ApplicationModel.Default.UserToken.IsSQLdmAdministrator;

            if (e.Tool.Key == "instanceContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = propertiesGrid.Rows.Count > 0 && propertiesGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool) e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool) e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
            if (e.Tool.Key == "instanceContextMenu")
            {
                bool enableRefreshButton = true;
                int selectedInstanceId = GetSelectedInstanceId();
                if (selectedInstanceId != -1)
                {
                    MonitoredSqlServerWrapper selectedInstance =
                        ApplicationModel.Default.ActiveInstances[selectedInstanceId];
                    if (selectedInstance != null)
                    {
                        if (selectedInstance.IsRefreshing)
                            enableRefreshButton = false;
                    }
                }
                var refreshButton = (ButtonTool) toolbarsManager.Tools["refreshInstanceButton"];
                refreshButton.SharedProps.Enabled = enableRefreshButton;
            }
            if (e.Tool.Key == "columnContextMenu")
            {
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(propertiesGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        public void Initialize(ServerGroupView parent, object initView)
        {
            parentView = parent;
            view = initView;
        }

        public void ApplySettings()
        {
            if (Settings.Default.ServerGroupPropertiesViewMainGrid is GridSettings)
            {
                lastMainGridSettings = Settings.Default.ServerGroupPropertiesViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, propertiesGrid);
            }
        }

        public void SaveSettings()
        {
            GridSettings mainGridSettings = GridSettings.GetSettings(propertiesGrid);
            // save all settings only if anything has changed
            if (!mainGridSettings.Equals(lastMainGridSettings))
            {
                lastMainGridSettings =
                    Settings.Default.ServerGroupPropertiesViewMainGrid = mainGridSettings;
            }
        }


        public void UpdateData()
        {
            UpdateData(null);
        }

        public void UpdateData(MonitoredSqlServerWrapper changedInstance)
        {
            // Check to see if this is the active view before updating
            // This view is loaded with the application and not checking can cause unnecessary startup churn
            if (!IsDisposed && ApplicationController.Default.ActiveView == this.parentView)
            {
                IList<MonitoredSqlServer> monitoredServers = new List<MonitoredSqlServer>();

                if (changedInstance != null)
                    // If this is being called in response to a server update, just accept those changes
                    monitoredServers.Add(changedInstance.Instance);
                else
                    // Otherwise we'll get the latest from the repository
                    // Technically this data is in ApplicationModel.Default.ActiveInstances already but getting from the repository is more up-to-date
                    // in case someone has made a change on another desktop client

                    //I have commented out the call to the repository.   The stored procedure it calls is messed up!  (And needs to be fixed.)
                    foreach (MonitoredSqlServer server in ApplicationModel.Default.ActiveInstances)
                    {
                        monitoredServers.Add(server);
                    }
                    //monitoredServers = RepositoryHelper.GetMonitoredSqlServers(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, true);
                
                
                // Create a list of instance IDs allowed to show based on the view (user view, tags, etc)
                var instances = new List<int>();
                
                if (view == null)
                {
                    foreach (MonitoredSqlServerWrapper instance in ApplicationModel.Default.ActiveInstances)
                    {
                        instances.Add(instance.Id);
                    }
                }
                else if (view is UserView)
                {
                    instances = new List<int>(((UserView) view).Instances);
                }
                else if (view is int)
                {
                    var tagId = (int) view;

                    if (ApplicationModel.Default.Tags.Contains(tagId))
                    {
                        Tag tag = ApplicationModel.Default.Tags[tagId];
                        instances = new List<int>(tag.Instances);
                    }
                }

                var currentServerIds = new List<int>();
                lock (sync)
                {
                    foreach (MonitoredSqlServer mss in monitoredServers)
                    {
                        if (instances.Contains(mss.Id))
                        {
                            PermissionType serverPerms = ApplicationModel.Default.UserToken.GetServerPermission(mss.Id);
                            if (serverPerms != PermissionType.None)
                            {
                                currentServerIds.Add(mss.Id);
                                MonitoredSqlServerConfiguration mssc = mss.GetConfiguration();

                                var match =
                                    (monitoredSqlServerConfigurationBindingSource.List.OfType
                                        <MonitoredSQLServerConfigurationDisplayWrapper>()).
                                        Select((item, index) => new {Item = item, Position = index}).Where(
                                            x => x.Item.InstanceName == mssc.InstanceName);

                                if (match.Count() == 1)
                                {
                                    // Don't replace if there are unsaved changes, unless the server is deleted
                                    if (!match.Single().Item.HasChanges)
                                    {
                                        monitoredSqlServerConfigurationBindingSource.List[match.Single().Position] =
                                            new MonitoredSQLServerConfigurationDisplayWrapper(mssc, mss.Id);
                                    }
                                }
                                else
                                {
                                    monitoredSqlServerConfigurationBindingSource.Add(
                                        new MonitoredSQLServerConfigurationDisplayWrapper(mssc, mss.Id));
                                }

                                if (serverPerms == PermissionType.View)
                                {
                                    UltraGridRow lockRow =
                                        propertiesGrid.Rows.GetAllNonGroupByRows().Where(
                                            x => (int) x.Cells["SQLServerID"].Value == mss.Id).First();
                                    lockRow.Activation = Activation.NoEdit;
                                    lockRow.Appearance.BackColorDisabled =
                                        lockRow.Appearance.BackColor = Color.FromArgb(240, 240, 240);
                                }
                            }
                        }
                    }

                    RemoveDeletedServers(currentServerIds);
                }


                if (!initialized)
                {
                    foreach (UltraGridColumn column in propertiesGrid.DisplayLayout.Bands[0].Columns)
                    {
                        if (column.Key != "Status Image")
                        {
                            column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                        }
                    }
                    initialized = true;
                }

                DisplayNoSqlServersLabel(propertiesGrid.Rows.Count > 0);
            }
        }

        /// <summary>
        /// Display a label to indicate that there are not have items to show in the view
        /// </summary>
        /// <param name="isEnable">Indicates if the label will display or not.</param>
        private void DisplayNoSqlServersLabel(bool isEnable)
        {
            if (isEnable)
            {
                lblNoSqlservers.Visible = false;
                lblNoSqlservers.SendToBack();
            }
            else
            {
                lblNoSqlservers.Visible = true;
                lblNoSqlservers.BringToFront();
            }
        }

        #region grid methods
        private void PrintGrid()
        {
            ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            ultraGridPrintDocument.Header.TextLeft =
                string.Format("Server Configuration Properties as of {0}",
                              DateTime.Now.ToString("G")
                    );
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid()
        {
            ValueListDisplayStyle hasChangesStyle =
                ((ValueList) propertiesGrid.DisplayLayout.Bands[0].Columns["HasChanges"].ValueList).DisplayStyle;

            string fileName = "AllServerProperties";
            string viewName = String.Empty;

            if (view is UserView)
            {
                viewName = ((UserView) view).Name;
            }
            else if (view is Tag)
            {
                viewName = ((Tag) view).Name;
            }

            if (viewName != String.Empty)
            {
                fileName = ExportHelper.GetValidFileName(string.Format("{0}{1}",
                                                                       viewName,
                                                                       viewName.IndexOf("Servers") > 0
                                                                           ? String.Empty
                                                                           : "Servers"
                                                             ));
            }

            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.FileName = fileName;
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ((ValueList) propertiesGrid.DisplayLayout.Bands[0].Columns["HasChanges"].ValueList).DisplayStyle =
                        ValueListDisplayStyle.DisplayText;
                    ultraGridExcelExporter.Export(propertiesGrid, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }
                ((ValueList) propertiesGrid.DisplayLayout.Bands[0].Columns["HasChanges"].ValueList).DisplayStyle =
                    hasChangesStyle;
            }
        }

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                propertiesGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                propertiesGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                propertiesGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                propertiesGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void ToggleGroupByBox()
        {
            propertiesGrid.DisplayLayout.GroupByBox.Hidden = !propertiesGrid.DisplayLayout.GroupByBox.Hidden;
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    propertiesGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    propertiesGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
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
            propertiesGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            propertiesGrid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            var dialog = new SimpleUltraGridColumnChooserDialog(propertiesGrid);
            dialog.Show(this);
        }
        #endregion

        /// <summary>
        /// Extract the instance id from the event args if they are from a row or a cell
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static int GetEventArgsInstanceId(EventArgs e)
        {
            if (e is DoubleClickRowEventArgs)
            {
                var args = (DoubleClickRowEventArgs) e;
                return (int)args.Row.Cells["SQLServerID"].Value;
                //if (propertiesGrid.Selected.Rows.Count == 1 && propertiesGrid.Selected.Rows[0].Cells != null)
                //{
                //    return (int) propertiesGrid.Selected.Rows[0].Cells["SQLServerID"].Value;
                //}
            }
            if(e is DoubleClickCellEventArgs)
            {
                var args = (DoubleClickCellEventArgs)e;
                return (int)args.Cell.Row.Cells["SQLServerID"].Value;
            }

            return -1;
        }
        
        /// <summary>
        /// This returns the instance id of the selected row. This is not the same as the clicked or double-clicked row
        /// </summary>
        /// <returns></returns>
        private int GetSelectedInstanceId()
        {
            if (propertiesGrid.Selected.Rows.Count == 1 && propertiesGrid.Selected.Rows[0].Cells != null)
            {
                return (int) propertiesGrid.Selected.Rows[0].Cells["SQLServerID"].Value;
            }

            return -1;
        }

        /// <summary>
        /// Shows the server view from the instance in the highlighted (not clicked) row
        /// </summary>
        private void OpenSelectedInstance()
        {
            int selectedInstanceId = GetSelectedInstanceId();

            if (selectedInstanceId != -1)
            {
                ApplicationController.Default.ShowServerView(selectedInstanceId);
            }
        }

        private void RefreshSelectedInstance()
        {
            int instanceId = GetSelectedInstanceId();
            MonitoredSqlServerWrapper selectedInstance = ApplicationModel.Default.ActiveInstances[instanceId];
            if (selectedInstance != null)
            {
                if (!selectedInstance.IsRefreshing)
                {
                    selectedInstance.ForceScheduledRefresh();
                }
            }
            else
                ApplicationController.Default.RefreshActiveView();
        }

        private void DeleteSelectedInstance()
        {
            if (ApplicationController.Default.ActiveView == parentView &&
                ApplicationModel.Default.UserToken.IsSQLdmAdministrator)
            {
                int selectedInstanceId = GetSelectedInstanceId();

                if (selectedInstanceId != -1 && ApplicationModel.Default.ActiveInstances.Contains(selectedInstanceId))
                {
                    DialogResult dialogResult =
                        ApplicationMessageBox.ShowWarning(ParentForm,
                                                          "SQL Diagnostic Manager allows you to retain data collected for SQL Server instances that are no longer monitored. " +
                                                          "This data may be useful for reporting purposes at a later time.\r\n\r\n" +
                                                          "Would you like to retain the data collected for the selected instance(s)?",
                                                          ExceptionMessageBoxButtons.YesNoCancel);

                    if (dialogResult != DialogResult.Cancel)
                    {
                        try
                        {
                            if (dialogResult == DialogResult.Yes)
                            {
                                ApplicationModel.Default.DeactivateMonitoredSqlServers(
                                    new[]
                                        {ApplicationModel.Default.ActiveInstances[selectedInstanceId]});
                            }
                            else if (dialogResult == DialogResult.No)
                            {
                                ApplicationModel.Default.DeleteMonitoredSqlServers(
                                    new[]
                                        {ApplicationModel.Default.ActiveInstances[selectedInstanceId]});
                            }
                        }
                        catch (Exception e)
                        {
                            ApplicationMessageBox.ShowError(ParentForm,
                                                            "An error occurred while removing the selected SQL Server instance.",
                                                            e);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Show server properties
        /// </summary>
        /// <param name="selectedInstanceId">id of server</param>
        private void ShowInstanceProperties(int selectedInstanceId)
        {
            if (selectedInstanceId != -1)
            {
                try
                {
                    var dialog =
                        new MonitoredSqlServerInstancePropertiesDialog(selectedInstanceId);
                    dialog.ShowDialog(this);
                }
                catch (Exception e)
                {
                    ApplicationMessageBox.ShowError(ParentForm, "Unable to show instance properties.", e);
                }
            }
        }

        private void propertiesGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid) sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));

                object contextObject;

                if (selectedElement != null)
                {
                    contextObject =
                        selectedElement.GetContext(typeof (ColumnHeader));
                }
                else
                {
                    LOG.Verbose(String.Format("Cancelled context menu: {0},{1},{2},{3}", e.Button, e.Clicks, e.X, e.Y));
                    return;
                }

                if (contextObject is ColumnHeader)
                {
                    var columnHeader = contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;
                    ((StateButtonTool)
                     toolbarsManager.Tools["groupByThisColumnButton"]).Checked = selectedColumn.IsGroupByColumn;
                    toolbarsManager.SetContextMenuUltra(((UltraGrid) sender), "columnContextMenu");
                }
                else
                {
                    contextObject = selectedElement.GetAncestor(typeof (RowUIElement));

                    if (contextObject is RowUIElement)
                    {
                        var row = contextObject as RowUIElement;
                        row.Row.Selected = true;
                        toolbarsManager.SetContextMenuUltra(((UltraGrid) sender), "instanceContextMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid) sender), "gridContextMenu");
                    }
                }
            }
            //else
            //{
            //    UIElement selectedElement =
            //       ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));

            //    if (selectedElement != null)
            //    {
            //        object contextObject = selectedElement.GetAncestor(typeof(RowUIElement));

            //        if (contextObject is RowUIElement)
            //        {
            //            var row = contextObject as RowUIElement;
            //            row.Row.Selected = true;
            //        }
            //    }
            //}
        }

        private void propertiesGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            int selectedInstanceId = GetSelectedInstanceId();

            if (selectedInstanceId != -1 && ApplicationModel.Default.ActiveInstances.Contains(selectedInstanceId))
            {
                ApplicationModel.Default.FocusObject = ApplicationModel.Default.ActiveInstances[selectedInstanceId];
            }
        }

        private void propertiesGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
            if (AutoScaleSizeHelper.isScalingRequired)
                AutoScaleSizeHelper.Default.AutoScaleControl(this.propertiesGrid, AutoScaleSizeHelper.ControlType.UltraGridCheckbox);
            try
            {
                e.Layout.UseFixedHeaders = true;
                //HasChanged column
                e.Layout.Bands[0].Columns["HasChanges"].Header.Fixed = true;
                //Server Name Column
                e.Layout.Bands[0].Columns["InstanceName"].Header.Fixed = true;

                UltraGridLayout layout = e.Layout;
                UltraTimeSpanEditorHelper helper;

                foreach (UltraGridBand band in layout.Bands)
                {
                    foreach (UltraGridColumn column in band.Columns)
                    {
                        if (column.DataType.IsAssignableFrom(typeof (TimeSpan)))
                        {
                            switch (column.Key)
                            {
                                // Long story here
                                // These were originally regular timespan editors but when it came time to put in custom events 
                                // (like the "oven timer" behavior) and the custom spinner buttons, it would not work within a grid.
                                // The buttons would not appear and the custom validation events would not fire.
                                // I found a thread here: http://forums.infragistics.com/forums/p/48274/256759.aspx
                                // in which Infragistics explains "Embedded editors use a different notification mechanism than controls. 
                                // You can use an UltraControlContainerEditor to provide a wrapper around your custom control and then assign 
                                // that UltraControlContainerEditor instance to the EditorComponent property."
                                //
                                // So, the UltraTimeSpanEditorHelper creates this control, but further, there is a problem whereby
                                // the control container is expecting that for a property "Value" that there will be a "ValueChanged" event
                                // but in the case of a timespan editor, TimeSpan does not have a TimeSpanEditor and Value (type Object) is not 
                                // able to directly cast to TimeSpan.  So this means that values that are using this control must
                                // be of type Object.
                                //
                                // I have added an attribute HasTimeSpanEditor to MonitoredSQLServerConfigurationDisplayWrapper to
                                // help identify these Objects which are supposted to be TimeSpans
                                case "ScheduledCollectionInterval":
                                    helper = new UltraTimeSpanEditorHelper(Constants.MinScheduledRefreshIntervalSeconds,
                                                                             Constants.MaxScheduledRefreshIntervalSeconds,
                                                                             Constants.DefaultScheduledRefreshIntervalSeconds);
                                    column.EditorComponent = helper.CreateControlContainer(Controls);
                                    break;
                                case "ServerPingInterval":
                                    
                                    helper = new UltraTimeSpanEditorHelper(Constants.MinServerPingIntervalSeconds,
                                                                             Constants.MaxServerPingIntervalSeconds,
                                                                             Constants.DefaultServerPingIntervalSeconds);
                                    column.EditorComponent = helper.CreateControlContainer(Controls);
                                    break;
                                case "DatabaseStatisticsInterval":
                                    var dbStatsEditor = new UltraTimeSpanEditor();
                                    helper = new UltraTimeSpanEditorHelper(Constants.MinDatabaseStatisticsIntervalSeconds,
                                                                             Constants.MaxDatabaseStatisticsIntervalSeconds,
                                                                             Constants.DefaultDatabaseStatisticsIntervalSeconds);
                                    column.EditorComponent = helper.CreateControlContainer(Controls);
                                    break;
                            }
                        }
                        else
                        {
                            switch (column.Key)
                            {
                                case "ReorganizationMinimumTableSizeKB":
                                    var reorgEditor = new UltraNumericEditor();
                                    reorgEditor.MaxValue = 999999;
                                    reorgEditor.MinValue = 0;
                                    reorgEditor.Nullable = false;
                                    reorgEditor.MaskInput = "nnnnnn KB";
                                    reorgEditor.MaskDisplayMode = MaskMode.IncludeLiterals;
                                    column.EditorComponent = reorgEditor;
                                    continue;
                            }

                            try
                            {
                                PropertyInfo property =
                                    (typeof (MonitoredSQLServerConfigurationDisplayWrapper)).GetProperty(column.Key);

                                if (!property.CanWrite)
                                {
                                    column.CellAppearance.BackColorDisabled =
                                        column.CellAppearance.BackColor =
                                        Color.FromArgb(240, 240, 240);
                                    column.CellActivation = Activation.NoEdit;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(e);
                                column.CellAppearance.BackColorDisabled =
                                    column.CellAppearance.BackColor =
                                    Color.FromArgb(240, 240, 240);
                                column.CellActivation = Activation.NoEdit;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void ServerGroupDetailsView_Load(object sender, EventArgs e)
        {
            ApplySettings();
        }

        internal void instance_Changed(object sender, MonitoredSqlServerChangedEventArgs e)
        {
            if (e.Instance != null)
                UpdateData(e.Instance);
            else
            {
                UpdateData();
            }
        }

        private void propertiesGrid_AfterCellUpdate(object sender, CellEventArgs e)
        {
            var serverId = (int) e.Cell.Row.Cells["SQLServerID"].Value;
            MonitoredSQLServerConfigurationDisplayWrapper serverConfig = FindServerBySQLServerId(serverId);
            bool hasChanged = false;

            if (serverConfig != null)
            {
                hasChanged = serverConfig.FieldHasChanged(e.Cell.Column.Key, e.Cell.Value);
            }

            if (hasChanged)
            {
                saveButton.Enabled = true;
                undoButton.Enabled = true;
                e.Cell.Appearance.BackColor = Color.FromArgb(255, 255, 216, 137);

                if (changedCells.ContainsKey(serverId))
                {
                    if (changedCells[serverId].Contains(e.Cell))
                        return;
                    else
                    {
                        changedCells[serverId].Add(e.Cell);
                    }
                }
                else
                {
                    changedCells.Add(serverId, new List<UltraGridCell>());
                    changedCells[serverId].Add(e.Cell);
                }
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                Focus();
                propertiesGrid.UpdateData();


                lock (sync)
                {
                    var serverConfigurationListChange = new List<MonitoredSQLServerConfigurationDisplayWrapper>();
                    serverConfigurationListChange.AddRange(ChangedServers());

                    foreach (MonitoredSQLServerConfigurationDisplayWrapper config in serverConfigurationListChange)
                    {
                        AuditingEngine.SetContextData(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                        //The VmConfigurationFlag indicate to management service this action should be auditing in sql server properties change.
                        //true is for audit and the false is for ignore
                        AuditingEngine.SetAuxiliarData("VmConfigurationFlag", new AuditAuxiliar<bool>(true));
                        AuditingEngine.SetAuxiliarData("ServerGroupPropertiesView", new AuditAuxiliar<bool>(true));
                        
                        config.SaveConfiguration();
                        if (changedCells.ContainsKey(config.SQLServerID))
                        {
                            foreach (UltraGridCell cell in changedCells[config.SQLServerID])
                            {
                                cell.Appearance.ResetBackColor();
                            }
                            changedCells.Remove(config.SQLServerID);
                        }
                    }
                }

                UpdateData();

                foreach (MonitoredSQLServerConfigurationDisplayWrapper config in ChangedServers())
                {
                    // If there are any remaining changed servers (on account of failed saves) the save button should stay enabled
                    saveButton.Enabled = true;
                    undoButton.Enabled = true;
                    return;
                }

                // We should only disable if there are no remaining changed servers
                saveButton.Enabled = false;
                undoButton.Enabled = false;
            }
            catch (Exception exception)
            {
                ApplicationMessageBox.ShowError(this, "A connection to the management service may not be available.",
                                                exception);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private IEnumerable<MonitoredSQLServerConfigurationDisplayWrapper> ChangedServers()
        {
            for (int i = 0; i < monitoredSqlServerConfigurationBindingSource.List.Count; i++)
            {
                var config =
                    (MonitoredSQLServerConfigurationDisplayWrapper) monitoredSqlServerConfigurationBindingSource.List[i];
                if (config.HasChanges)
                    yield return config;
            }
        }

        private MonitoredSQLServerConfigurationDisplayWrapper FindServerBySQLServerId(int id)
        {
            for (int i = 0; i < monitoredSqlServerConfigurationBindingSource.List.Count; i++)
            {
                var config =
                    (MonitoredSQLServerConfigurationDisplayWrapper) monitoredSqlServerConfigurationBindingSource.List[i];
                if (config.SQLServerID == id)
                    return config;
            }

            return null;
        }


        private void RemoveDeletedServers(List<int> currentServerIds)
        {
            foreach (int i in RemoveDeletedServersIterator(currentServerIds))
            {
                monitoredSqlServerConfigurationBindingSource.List.RemoveAt(i);
            }
        }

        private IEnumerable<int> RemoveDeletedServersIterator(List<int> deletedServers)
        {
            // Need to step backwards because we're deleting
            for (int i = monitoredSqlServerConfigurationBindingSource.List.Count - 1; i >= 0; i--)
            {
                var config =
                    (MonitoredSQLServerConfigurationDisplayWrapper) monitoredSqlServerConfigurationBindingSource.List[i];

                if (!deletedServers.Contains(config.SQLServerID))
                {
                    if (changedCells.ContainsKey(config.SQLServerID))
                    {
                        changedCells.Remove(config.SQLServerID);
                    }
                    yield return i;
                }
            }
        }

        private void undoButton_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                Focus();
                propertiesGrid.UpdateData();


                lock (sync)
                {
                    foreach (MonitoredSQLServerConfigurationDisplayWrapper config in ChangedServers())
                    {
                        config.ResetConfiguration();
                        if (changedCells.ContainsKey(config.SQLServerID))
                        {
                            foreach (UltraGridCell cell in changedCells[config.SQLServerID])
                            {
                                cell.Appearance.ResetBackColor();
                            }
                            changedCells.Remove(config.SQLServerID);
                        }
                    }
                }

                UpdateData();

                foreach (MonitoredSQLServerConfigurationDisplayWrapper config in ChangedServers())
                {
                    // If there are any remaining changed servers (on account of failed saves) the undo button should stay enabled
                    undoButton.Enabled = true;
                    saveButton.Enabled = true;
                    return;
                        // Quick return - the ChangedServers() function uses yield return, so when we find 1, go ahead and exit
                }

                // We should only disable if there are no remaining changed servers
                undoButton.Enabled = false;
                saveButton.Enabled = false;
            }
            catch (Exception exception)
            {
                ApplicationMessageBox.ShowError(this, "A connection to the management service may not be available.",
                                                exception);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void propertiesGrid_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            var serverId = GetEventArgsInstanceId(e);

            PermissionType serverPerms = ApplicationModel.Default.UserToken.GetServerPermission(serverId);
            if (serverPerms == PermissionType.None)
                return;
            MonitoredSQLServerConfigurationDisplayWrapper serverConfig = FindServerBySQLServerId(serverId);
            if (serverConfig.HasChanges)
            {
                DialogResult dialogResult =
                    ApplicationMessageBox.ShowQuestion(ParentForm,
                                                       "This server has unsaved changes.\r\n\r\n" +
                                                       "Would you like to save the changes before continuing?",
                                                       ExceptionMessageBoxButtons.YesNoCancel);

                switch (dialogResult)
                {
                    case DialogResult.Yes:
                        serverConfig.SaveConfiguration();
                        if (changedCells.ContainsKey(serverConfig.SQLServerID))
                        {
                            foreach (UltraGridCell cell in changedCells[serverConfig.SQLServerID])
                            {
                                cell.Appearance.ResetBackColor();
                            }
                            changedCells.Remove(serverConfig.SQLServerID);
                        }
                        break;
                    case DialogResult.No:
                        serverConfig.ResetConfiguration();
                        if (changedCells.ContainsKey(serverConfig.SQLServerID))
                        {
                            foreach (UltraGridCell cell in changedCells[serverConfig.SQLServerID])
                            {
                                cell.Appearance.ResetBackColor();
                            }
                            changedCells.Remove(serverConfig.SQLServerID);
                        }
                        break;
                    case DialogResult.Cancel:
                        return;
                }
            }

            if (e.Cell.Column.Key == "JobAlerts" || e.Cell.Column.Key == "ErrorlogAlerts")
            {
                Metric metric;
                if (e.Cell.Column.Key == "JobAlerts")
                {
                    metric = Metric.JobCompletion;
                }
                else
                {
                    metric = Metric.ErrorLog;
                }
                using (
                    var alertConfigDialog =
                        new AlertConfigurationDialog(serverId, false)
                    )
                {
                    alertConfigDialog.Select(metric);

                    alertConfigDialog.ShowDialog(this);
                }
                return;
            }


            using (var dialog = new MonitoredSqlServerInstancePropertiesDialog(serverId))
            {
                switch (e.Cell.Column.Key)
                {
                    case "PreferredClusterNode":
                    case "ActiveClusterNode":
                        dialog.SelectedPropertyPage =
                            MonitoredSqlServerInstancePropertiesDialogPropertyPages.ClusterSettings;
                        break;
                    case "ReorganizationMinimumTableSizeKB":
                    case "LastGrowthStatisticsRunTime":
                    case "LastReorgStatisticsRunTime":
                    case "GrowthStatisticsDisplay":
                    case "ReorgStatisticsDisplay":
                    case "TableStatisticsExcludedDatabasesCount":
                    case "TableStatisticsExcludedDatabasesString":
                        dialog.SelectedPropertyPage =
                            MonitoredSqlServerInstancePropertiesDialogPropertyPages.TableStatistics;
                        break;
                    case "ReplicationMonitoringDisabled":
                        dialog.SelectedPropertyPage =
                            MonitoredSqlServerInstancePropertiesDialogPropertyPages.Replication;
                        break;
                    case "WmiConfiguration":
                        dialog.SelectedPropertyPage =
                            MonitoredSqlServerInstancePropertiesDialogPropertyPages.OleAutomation;
                        break;
                    case "MaintenanceMode":
                        dialog.SelectedPropertyPage =
                            MonitoredSqlServerInstancePropertiesDialogPropertyPages.MaintenanceMode;
                        break;
                    case "QueryMonitorConfiguration":
                    case "QueryMonitorEnabled":
                    case "QueryMonitorFilterTypes":
                    case "QueryMonitorAdvancedFilters":
                        dialog.SelectedPropertyPage =
                            MonitoredSqlServerInstancePropertiesDialogPropertyPages.QueryMonitor;
                        break;
                    case "CustomCountersCount":
                    case "CustomCountersList":
                        dialog.SelectedPropertyPage =
                            MonitoredSqlServerInstancePropertiesDialogPropertyPages.CustomCounters;
                        break;
                    case "DiskCollectionSettings":
                        dialog.SelectedPropertyPage = MonitoredSqlServerInstancePropertiesDialogPropertyPages.Disk;
                        break;
                    case "ActiveWaitsConfiguration":
                    case "ActiveWaitsFilters":
                        dialog.SelectedPropertyPage =
                            MonitoredSqlServerInstancePropertiesDialogPropertyPages.WaitStatistics;
                        break;
                    case "VCenter":
                    case "VMHost":
                        dialog.SelectedPropertyPage =
                            MonitoredSqlServerInstancePropertiesDialogPropertyPages.Virtualization;
                        break;
                    case "BaselineTimePeriod":
                    case "BaselineDateRange":
                        dialog.SelectedPropertyPage = MonitoredSqlServerInstancePropertiesDialogPropertyPages.Baseline;
                        break;
                    case "ActivityMonitorEnabled":
                    case "DeadlockMonitoring":
                        dialog.SelectedPropertyPage = MonitoredSqlServerInstancePropertiesDialogPropertyPages.ActivityMonitor;
                        break;
                    default:
                        dialog.SelectedPropertyPage = MonitoredSqlServerInstancePropertiesDialogPropertyPages.Popular;
                        break;
                }
                dialog.ShowDialog(this);
            }
        }

        private void propertiesGrid_ClickCell(object sender, ClickCellEventArgs e)
        {
            columnLabel.Text = e.Cell.Column.Header.Caption;
            switch (e.Cell.Column.Key)
            {
                case "SQLServerID":
                    columnHelp.Text =
                        @"The SQL Server ID of the monitored SQL Server as used internally by SQL Diagnostic Manager.  This cannot be modified.";
                    break;
                case "HasChanges":
                    columnLabel.Text = "Server Has Changes";
                    columnHelp.Text =
                        @"Indicates whether the server has configuration changes that have not yet been saved.";
                    break;
                case "PreferredClusterNode":
                    columnHelp.Text =
                        @"The preferred cluster node of a server, used to alert when a server is running on a non-preferred node.  Modify on this screen or in Server Properties.";
                    break;
                case "ScheduledCollectionInterval":
                    columnHelp.Text =
                        @"The interval between times that diagnostic data is collected and associated alerts are raised.  Lower values cause alerts to be more raised more quickly, but also cause refreshes to be performed more often, which may increase the monitoring overhead.  Modify on this screen or in Server Properties.";
                    break;
                case "ReorganizationMinimumTableSizeKB":
                    columnHelp.Text =
                        @"The minimum size, in kilobytes, a table must meet before fragmentation statistics will be collected.  Fragmentation statistics are only gathered on tables with clustered indexes.  Modify on this screen or in Server Properties.";
                    break;
                case "ReplicationMonitoringDisabled":
                    columnHelp.Text =
                        @"Controls whether replication statistics collection is disabled.  Replication monitoring may cause excess monitoring overhead in some environments and may not be necessary for all servers.  Modify on this screen or in Server Properties.";
                    break;
                case "ExtendedHistoryCollectionDisabled":
                    columnHelp.Text =
                        @"Controls whether extended session data collection is enabled, including history browser collection of session details, locks, and blocks.  Collection of this data allows viewing of past session detail, lock, and block information but incurs extra monitoring overhead to provide this data.  This data also requires extra space in the SQLDM repository.  Modify on this screen or in Server Properties.";
                    break;
                case "WmiConfiguration":
                    columnHelp.Text =
                        @"Controls whether SQL Diagnostic Manager will use Windows Management Instrumentation (WMI) to collect operating system and disk statistics.  This setting controls whether SQLDM will attempt to use OLE Automation or a direct connection to the WMI service.  Disabling this setting will prevent the collection of certain metrics in SQLDM but may lessen the monitoring overhead.  Modify in Server Properties.";
                    break;
                case "ServerPingInterval":
                    columnHelp.Text =
                        @"The interval, in seconds, between times when the server availability is verified.  If a ""select 1"" query cannot execute in this timeframe the server is considered unresponsive.  Setting this to a very low value may result in false positive alerts.  Modify on this screen or in Server Properties.";
                    break;
                case "MostRecentSQLVersion":
                    columnHelp.Text =
                        @"The SQL Server version of the monitored instance at the last successful refresh.  This cannot be modified.";
                    break;
                case "MostRecentSQLEdition":
                    columnHelp.Text =
                        @"The SQL Server edition of the monitored instance at the last successful refresh.  This cannot be modified.";
                    break;
                case "JobAlerts":
                    columnHelp.Text =
                        @"Indicates whether job alerting is enabled, which causes the job alerts collector to run.  Job alerting is considered enabled if one of the following alerts is enabled: 
•	SQL Server Agent Job Completion
•	SQL Server Agent Job Failure
•	SQL Server Agent Long Running Job (Minutes)
•	SQL Server Agent Long Running Job (Percent)

If none of these alerts are enabled, the job alerting collector is not executed.  Modify this on the Alert Configuration screen.";
                    break;
                case "ErrorlogAlerts":
                    columnHelp.Text =
                        @"Displays whether error log alerting is enabled, causing a specific error log collector to run.  Error log alerting is considered enabled if either the SQL Server Agent Log or SQL Server Error Log alert is enabled.  These collectors may be performance intensive.  The impact of error log reading may be reduced by regularly cycling the server error logs.  Modify this on the Alert Configuration screen.";
                    break;
                case "AuthenticationMode":
                    columnHelp.Text =
                        @"The authentication mode used by the SQLDM Collection Service to collect diagnostic data from the monitored SQL Server instance.  Modify on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "ActiveClusterNode":
                    columnHelp.Text =
                        @"The name of the active cluster node on the monitored SQL Server instance as of the last successful refresh.  This cannot be modified.";
                    break;
                case "InstanceName":
                    columnHelp.Text =
                        @"The name of the monitored SQL Server instance.  This cannot be modified.";
                    break;
                case "MaintenanceMode":
                    columnHelp.Text =
                        @"Displays the occasions when this server is in maintenance mode.  Maintenance mode ceases scheduled statistics collection and alerting on the server.  Options include Never, Until further notice, Recurring every week at the specified time, and Occurring once at the specified time.  Modify on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "QueryMonitorConfiguration":
                    columnHelp.Text =
                        @"Displays the configuration for the Query Monitor including what items are captured, and the poorly-performing thresholds for these items.  Low thresholds for the Query Monitor may cause a performance impact.  Modify on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "QueryMonitorEnabled":
                    columnHelp.Text =
                        @"Displays whether the Query Monitor is enabled.  The Query Monitor is used to collect poorly performing SQL as well as deadlocks and autogrow events.  Low thresholds for the Query Monitor may cause a performance impact.  Modify on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "QueryMonitorFilterTypes":
                    columnHelp.Text =
                        @"Displays the effective thresholds for the Query Monitor. Low thresholds for Query Monitor may impact performance.  You can modify thresholds on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "QueryMonitorAdvancedFilters":
                    columnHelp.Text =
                        @"Displays the advanced filter configuration for your Query Monitor including which applications, databases, and SQL text you want to exclude from the Query Monitor.  Filtering out unnecessary data can reduce the monitoring impact of the Query Monitor.  Modify on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "LastGrowthStatisticsRunTime":
                    columnHelp.Text =
                        @"Displays the date and time of the most recent, successful table statistics collection.  This timestamp is local to the monitored SQL Server instance.  This cannot be modified.";
                    break;
                case "LastReorgStatisticsRunTime":
                    columnHelp.Text =
                        @"Displays the date and time of the most recent, successful table fragmentation collection.  This timestamp is local to the monitored SQL Server instance.  This cannot be modified.";
                    break;
                case "GrowthStatisticsDisplay":
                    columnHelp.Text =
                        @"Displays the days of the week and time of day when an attempt at table statistics collection occurs.  Modify on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "ReorgStatisticsDisplay":
                    columnHelp.Text =
                        @"Displays the days of the week and time of day when an attempt at table fragmentation collection occurs.  Modify on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "TableStatisticsExcludedDatabasesCount":
                    columnHelp.Text =
                        @"Displays the total number of databases excluded from table statistic collection.  Modify on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "TableStatisticsExcludedDatabasesString":
                    columnHelp.Text =
                        @"Displays the name of each database excluded from table statistic collection.  Modify on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "CustomCountersCount":
                    columnHelp.Text =
                        @"Displays the total number of custom counters monitored on this SQL Server instance.  Modify on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "CustomCountersList":
                    columnHelp.Text =
                        @"Displays the name for each custom counter monitored on this SQL Server instance.  Modify on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "DiskCollectionSettings":
                    columnHelp.Text =
                        @"Displays whether the connected disk drives are discovered automatically, and if not, which drives are monitored.  Modify on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "InputBufferLimiter":
                    columnHelp.Text =
                        @"Displays the limit of the executions performed by the DBCC Inputbuffer.  This command is used to retrieve the actual input command for the Session Details view, among others.  On busy servers limiting this value can reduce monitoring impact.  Modify on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "ActiveWaitsConfiguration":
                    columnHelp.Text =
                        @"Displays the date and time when the query-level wait statistics are collected or whether they are collected indefinitely. By default, these statistics are available only when the Query Waits view is open.  Collection of query waits is a performance intensive operation.  Modify on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "ActiveWaitsFilters":
                    columnHelp.Text =
                        @"Displays the filter configuration of the query-level wait statistics for this monitored SQL Server instance, including which applications, databases, and SQL text you want to exclude from the query wait statistics collection.  Collection of query waits is a performance intensive operation and excluding unnecessary data can reduce the monitoring footprint.  Modify on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "VCenter":
                    columnHelp.Text =
                        @"Displays the name of the host server on which this monitored SQL Server instance is running. This field applies only to SQL Server instances running on a virtual machine.  Modify on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "VMHost":
                    columnHelp.Text =
                        @"Displays the name of the virtual machine on which this monitored SQL Server instance is running. This field applies only to SQL Server instances running on a virtual machine. Modify on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "BaselineTimePeriod":
                    columnHelp.Text =
                        @"Displays time period over which the server baseline is calculated.  Modify on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "BaselineDateRange":
                    columnHelp.Text =
                        @"Displays date range over which the server baseline is calculated.  Modify on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;
                case "DatabaseStatisticsInterval":
                    columnHelp.Text =
                        String.Format(
                            @"The interval between times that database space related data is collected and associated alerts are raised.  Lower values cause alerts to be more raised more quickly, but also cause refreshes to be performed more often, which increases the monitoring overhead.  In environments with a large number of databases whose sizes do not change rapidly, setting this to a long interval can greatly reduce the monitoring footprint.  Modify on this screen or in Server Properties.  This value must be set between {0} minute and {1} hours.",
                            Constants.MinDatabaseStatisticsIntervalSeconds/60,
                            Constants.MaxDatabaseStatisticsIntervalSeconds/60/60);
                    break;
                case "DeadlockMonitoring":
                    columnHelp.Text =
                        @"Indicates whether deadlock monitoring is enabled. This is required in order to raise alerts on deadlocking sessions on the monitored SQL server.  Deadlock Monitoring is supported on monitored servers running SQL Server 2005 or greater and is dependent on enabling the monitoring of non-query activities in the Activity Monitor window. A value of Enabled /Not Running indicates that only query monitoring is enabled, the monitoring of non-query activities is disabled and no deadlock events are captured. Modify this setting on the Server Properties dialog for the server, which can be accessed by double-clicking on the cell.";
                    break;

                case "ActivityMonitorEnabled":
                    columnHelp.Text =
                        @"The Activity Monitor captures non-query related activites and events that could impact your monitored SQL Server such as blocking events.";
                    break;
            }

            PropertyInfo property =
                (typeof (MonitoredSQLServerConfigurationDisplayWrapper)).GetProperty(e.Cell.Column.Key);

            serverValidRange.Visible = false;
            columnHelp.Top = 31;
            if (AutoScaleSizeHelper.isScalingRequired)
                columnHelp.Top = 48 ;
            columnHelp.Height = 98;

            if (property.CanWrite)
            {
                foreach (
                    object attr in
                        property.GetCustomAttributes(
                            typeof (MonitoredSQLServerConfigurationDisplayWrapper.MinAndMaxValue), false))
                {
                    serverValidRange.Visible = true;
                    columnHelp.Top = 59;
                    if (AutoScaleSizeHelper.isScalingRequired)
                        columnHelp.Top = 48;
                    columnHelp.Height = 70;

                    int minValue = ((MonitoredSQLServerConfigurationDisplayWrapper.MinAndMaxValue) attr).MinValue;
                    int maxValue = ((MonitoredSQLServerConfigurationDisplayWrapper.MinAndMaxValue) attr).MaxValue;
                    string formatString = @"Value must be between {0} and {1}";
                    if (property.GetCustomAttributes(typeof(MonitoredSQLServerConfigurationDisplayWrapper.HasTimeSpanEditor),false).Count() > 0)
                    {
                        serverValidRange.Text = String.Format(formatString, FriendlyTimeFormat(minValue),
                                                              FriendlyTimeFormat(maxValue));
                    }
                    else
                    {
                        serverValidRange.Text = String.Format(formatString, minValue,
                                                              maxValue);
                    }
                }
            }
        }

        private string FriendlyTimeFormat(int seconds)
        {
            if (seconds == 1)
            {
                return "1 second";
            }
            if (seconds < 60)
            {
                return String.Format("{0} seconds", seconds);
            }
            if (seconds == 60)
            {
                return "1 minute";
            }
            if (60 < seconds && seconds < 3600)
            {
                return String.Format("{0} minutes", seconds/60);
            }
            if (seconds == 3600)
            {
                return "1 hour";
            }
            if (3600 < seconds)
            {
                return String.Format("{0} hours", seconds/3600);
            }

            return String.Format("{0} seconds", seconds);
        }

        private void ChangeDatabaseStatisticsIntervalToHours(UltraGridCell cell)
        {
            cell.Value = TimeSpan.FromHours(24);
        }

        private void propertiesGrid_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
            PropertyInfo property =
                (typeof (MonitoredSQLServerConfigurationDisplayWrapper)).GetProperty(e.Cell.Column.Key);

            bool hasTimeSpanEditor = false;
            hasTimeSpanEditor =
                (property.GetCustomAttributes(typeof (MonitoredSQLServerConfigurationDisplayWrapper.HasTimeSpanEditor),
                                              false).Count() > 0);


            if (property.CanWrite)
            {
                if (property.PropertyType != e.NewValue.GetType() && !hasTimeSpanEditor)
                {
                    if (property.PropertyType == typeof (TimeSpan) )
                    {
                        ApplicationMessageBox.ShowError(this,
                                                        String.Format(
                                                            "The input value for {0} was not in an expected format.  Time should be entered in hh:mm:ss format.",
                                                            e.Cell.Column.Header.Caption));
                    }
                    else
                    {
                        ApplicationMessageBox.ShowError(this,
                                                        String.Format(
                                                            "The input value for {0} was not in an expected format.",
                                                            e.Cell.Column.Header.Caption));
                    }

                    e.Cancel = true;
                    return;
                }
                // HACK - 24:00:00 is evaluated as 24 days instead of 24 hours, so correct that for Database Statistics Interval
                // You cannot change it in this event (it will throw an error) but can use BeginInvoke to change it outside this event
                if (property.Name == "DatabaseStatisticsInterval" && ((FormattedTimeSpan)e.NewValue).TimeSpan.TotalDays == 24)
                {
                    GridDelegate d = ChangeDatabaseStatisticsIntervalToHours;
                    e.Cancel = true;
                    BeginInvoke(d, e.Cell);
                    return;
                }

                foreach (
                    object attr in
                        property.GetCustomAttributes(
                            typeof (MonitoredSQLServerConfigurationDisplayWrapper.MinAndMaxValue), false))
                {
                    int minValue = ((MonitoredSQLServerConfigurationDisplayWrapper.MinAndMaxValue) attr).MinValue;
                    int maxValue = ((MonitoredSQLServerConfigurationDisplayWrapper.MinAndMaxValue) attr).MaxValue;
                    if (property.PropertyType == typeof(TimeSpan) || hasTimeSpanEditor)
                    {
                        TimeSpan time = new TimeSpan();
                        if (e.NewValue is FormattedTimeSpan)
                            time = ((FormattedTimeSpan)e.NewValue).TimeSpan;
                        else
                            time = (TimeSpan) e.NewValue;
                        if (time.TotalSeconds < minValue)

                        {
                            ApplicationMessageBox.ShowError(this,
                                                            String.Format("The minimum value for {0} is {1}.",
                                                                          e.Cell.Column.Header.Caption,
                                                                          FriendlyTimeFormat(minValue)));
                            e.Cancel = true;
                        }
                        else if (time.TotalSeconds > maxValue)
                        {
                            ApplicationMessageBox.ShowError(this,
                                                            String.Format("The maximum value for {0} is {1}.",
                                                                          e.Cell.Column.Header.Caption,
                                                                          FriendlyTimeFormat(maxValue)));
                            e.Cancel = true;
                        }
                    }
                    else
                    {
                        if ((int) e.NewValue < minValue)
                        {
                            ApplicationMessageBox.ShowError(this,
                                                            String.Format("The minimum value for {0} is {1}.",
                                                                          e.Cell.Column.Header.Caption, maxValue));
                            e.Cancel = true;
                        }
                        else if ((int) e.NewValue > maxValue)
                        {
                            ApplicationMessageBox.ShowError(this,
                                                            String.Format("The maximum value for {0} is {1}.",
                                                                          e.Cell.Column.Header.Caption, maxValue));
                            e.Cancel = true;
                        }
                    }
                }
            }
        }

      

        #region Nested type: GridDelegate

        private delegate void GridDelegate(UltraGridCell cell);

        #endregion

        #region Nested type: HideFocusRectangleDrawFilter

        internal sealed class HideFocusRectangleDrawFilter : IUIElementDrawFilter
        {
            #region IUIElementDrawFilter Members

            public bool DrawElement(DrawPhase drawPhase, ref UIElementDrawParams drawParams)
            {
                return true;
            }

            public DrawPhase GetPhasesToFilter(ref UIElementDrawParams drawParams)
            {
                return DrawPhase.BeforeDrawFocus;
            }

            #endregion
        }

        #endregion

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            UpdateCellColors();
            SetGridTheme();
        }

        void UpdateCellColors()
        {
            try
            {
                UltraGridLayout layout = this.propertiesGrid.DisplayLayout;
                foreach (UltraGridBand band in layout.Bands)
                {
                    foreach (UltraGridColumn column in band.Columns)
                    {
                        if (!column.DataType.IsAssignableFrom(typeof(TimeSpan)))
                        {
                            try
                            {
                                PropertyInfo property =
                                    (typeof(MonitoredSQLServerConfigurationDisplayWrapper)).GetProperty(column.Key);

                                if (!property.CanWrite)
                                {

                                    column.CellAppearance.BackColorDisabled =
                                        column.CellAppearance.BackColor =
                                        Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor) : Color.FromArgb(240, 240, 240);
                                    column.CellActivation = Activation.NoEdit;

                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                column.CellAppearance.BackColorDisabled =
                                        column.CellAppearance.BackColor =
                                        Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor) : Color.FromArgb(240, 240, 240);
                                column.CellActivation = Activation.NoEdit;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.propertiesGrid);
        }

    }
}