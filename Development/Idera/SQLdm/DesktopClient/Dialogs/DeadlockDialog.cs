using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System.Collections.Specialized;
    using System.Configuration;
    using Controls;
    using Helpers;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Infragistics.Win;
    using Infragistics.Win.UltraWinDataSource;
    using Infragistics.Win.UltraWinGrid;
    using Infragistics.Win.UltraWinToolbars;
    using Idera.SQLdm.DesktopClient.Objects;
    using Properties;
    using Wintellect.PowerCollections;
    using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;
    using Infragistics.Windows.Themes;

    public partial class DeadlockDialog : BaseDialog
    {
        private const string BAND_RESOURCE = "Resource";
        private const string LOCK_TYPE = "Lock Type";
        private const string WAIT_RESOURCE = "Wait Resource";
        private const string OWN_WAIT = "Own/Wait";
        private const string MODE = "Mode";
        private const string SPID = "SPID";
        private const string ECID = "ECID";
        private const string DATABASE = "Database";
        private const string PROCEDURE = "Procedure";
        private const string SQL = "Sql";
        private const string APPLICATION = "Application";
        private const string LINE = "Line";
        private const string VICTIM = "Victim";
        private const string DEADLOCK_PROCESS = "DeadlockProcess";

        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("DeadlockDialog");

        private static List<string> VISIBLE_COLUMNS = null;

        private static List<string> GetVisibleColumnList()
        {
            if (VISIBLE_COLUMNS == null)
            {
                string[] values = new string[]
                {
                    LOCK_TYPE,
                    WAIT_RESOURCE,
                    OWN_WAIT,
                    MODE,
                    SPID,
                    ECID,
                    DATABASE,
                    PROCEDURE,
                    VICTIM,//SQLdm 8.5 (Vamshi): Added VICTIM to the list of visible fields
                    SQL
                };

                VISIBLE_COLUMNS = new List<string>(values);
            }
            return VISIBLE_COLUMNS;
        } 

        private string xdl;
        private UltraGridColumn selectedColumn;
        private Control focused = null;

        public DeadlockDialog(string xdl)
        {
            this.DialogHeader = "DeadlockDialog";
            this.xdl = xdl;
  
            // testing override
            // this.xdl = System.IO.File.ReadAllText("C:\\Documents and Settings\\KGOOLSBEE\\My Documents\\deadlock-korean.xdl");

            InitializeComponent();
            flatGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            AdaptFontSize();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.flatGrid);
        }

        private void DeadlockDialog_Load(object sender, EventArgs args)
        {
            Form owner = this.Owner;
            if (owner != null)
                this.Icon = owner.Icon;

            UltraGridBand band = flatGrid.DisplayLayout.Bands[BAND_RESOURCE];
            
            DeadlockData deadlockData = null;
            try
            {
                deadlockData = DeadlockData.FromXDL(xdl);
                UltraDataSource table = FlattenDeadlockData(deadlockData);
                flatGrid.DataSource = table;
            } catch (Exception e)
            {
                Size = new Size(525, 200);
                flatGrid.Visible = false;
                noDataLabel.Text = "There was an error trying to parse the deadlock xdl document.  You can still use the Export XDL button to save the document to a file for viewing with SQL Server Management Studio or SQL Server Profiler.";
                splitContainer1.Panel2Collapsed = true;
                LOG.Error("Error parsing XDL: ", e);
                LOG.Verbose("Goofed up XDL: ", xdl);
            }

            if (deadlockData != null)
            {
                // hide the victim column			  
                //SQLdm 8.5 (Vamshi): Commented out the line below as we do not want victom to be hidden
                //band.Columns[VICTIM].Hidden = true;
                // dont showing filtering icon in header
                band.Columns[LOCK_TYPE].AllowRowFiltering = DefaultableBoolean.False;
                // dont show the deadlock process object
                band.Columns[DEADLOCK_PROCESS].ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;

                // only show a known list of columns
                List<String> visibleList = GetVisibleColumnList();
                foreach (UltraGridColumn gridcol in band.Columns)
                {
                    if (!visibleList.Contains(gridcol.Key))
                        gridcol.Hidden = true;
                }

                // Group by Wait Resource
                band.SortedColumns.Clear();
                int sx = band.SortedColumns.Add(WAIT_RESOURCE, false, true);
                flatGrid.Rows.ExpandAll(false);

                foreach (Deadlock deadlock in deadlockData.Items)
                {
                    DeadlockResourceList list = deadlock.resourceList;
                    if (list != null)
                    {
                        foreach (DeadlockResource resource in list.ResourceList)
                        {
                            switch (resource.ResourceType)
                            {
                                case "exchangeEvent":
                                    ((StateButtonTool)toolbarManager.Tools["exchangeEventsStateButton"]).Checked = true;
                                    toolbarManager.Tools["exchangeEventsStateButton"].SharedProps.Visible = true;
                                    break;
                                case "threadpool":
                                    ((StateButtonTool)toolbarManager.Tools["threadPoolStateButton"]).Checked = true;
                                    toolbarManager.Tools["threadPoolStateButton"].SharedProps.Visible = true;
                                    break;
                            }
                        }
                    }
                }
                ConfigureDetailsPanelExpanded(Settings.Default.DeadlockViewDetailsPanelExpanded);
            }

            band.PerformAutoResizeColumns(true, PerformAutoSizeType.AllRowsInBand);
            splitContainer1.SplitterDistance = Settings.Default.DeadlockViewSplitterDistance;
            Settings.Default.SettingChanging += Settings_SettingChanging;
            ApplySettings();
        }

        private void ConfigureDetailsPanelExpanded(bool expanded)
        {
            splitContainer1.Panel2Collapsed = !expanded;            
        }

        private void Settings_SettingChanging(object sender, SettingChangingEventArgs e)
        {
            switch (e.SettingName)
            {
                case "DeadlockViewDetailsPanelExpanded":
                    ConfigureDetailsPanelExpanded((bool)e.NewValue);
                    break;
            }
        }

        public void ApplySettings()
        {
//            if (Settings.Default.AlertsViewMainGrid is GridSettings)
//            {
//                lastMainGridSettings = Settings.Default.AlertsViewMainGrid;
//                GridSettings.ApplySettingsToGrid(lastMainGridSettings, alertsGrid);
//            }
        }

        public void SaveSettings()
        {
//            GridSettings mainGridSettings = GridSettings.GetSettings(alertsGrid);
//            // save all settings only if anything has changed
//            if (!mainGridSettings.Equals(lastMainGridSettings))
//            {
//                lastMainGridSettings =
//                    Settings.Default.AlertsViewMainGrid = mainGridSettings;
//            }
        }

        private UltraDataSource FlattenDeadlockData(DeadlockData data)
        {
            UltraDataSource ds = new UltraDataSource();
            ds.Band.Key = BAND_RESOURCE;
            ds.Band.Columns.Add(LOCK_TYPE);
            ds.Band.Columns.Add(OWN_WAIT);
            ds.Band.Columns.Add(MODE);
            ds.Band.Columns.Add(SPID, typeof(int));
            ds.Band.Columns.Add(ECID, typeof(int));
            ds.Band.Columns.Add(DATABASE);
            ds.Band.Columns.Add(PROCEDURE);
            ds.Band.Columns.Add(VICTIM, typeof(bool));
            ds.Band.Columns.Add(SQL);
            ds.Band.Columns.Add(APPLICATION);
            ds.Band.Columns.Add(LINE, typeof(int));
            //SQLdm 8.5 (Vamshi): Moved Victim column above to make it appear before sql column

            ds.Band.Columns.Add("Host");
            ds.Band.Columns.Add("Isolation Level");
            ds.Band.Columns.Add("Lock Mode");
            ds.Band.Columns.Add("Lock Timeout");

            ds.Band.Columns.Add("Login");
            ds.Band.Columns.Add("Priority");
            ds.Band.Columns.Add("Status");
            ds.Band.Columns.Add("Transaction Name");
            ds.Band.Columns.Add(WAIT_RESOURCE);
            ds.Band.Columns.Add("Wait Time");
            ds.Band.Columns.Add(DEADLOCK_PROCESS, typeof (DeadlockProcess));

            UltraDataBand frameBand = ds.Band.ChildBands.Add("Frame");
            frameBand.Columns.Add("Procedure");
            frameBand.Columns.Add("Line", typeof (int));
//            frameBand.Columns.Add("Stmt Start", typeof (int));
            frameBand.Columns.Add(SQL);

            AddDynamicColumns(ds, data);

            Dictionary<string, DeadlockProcess> processMap = new Dictionary<string, DeadlockProcess>();

            foreach (Deadlock deadlock in data.Items)
            {
				//SQLdm 9.1 (Ankit Srivastava) Activity Monitoring with Extended Events - use the new property in case of Extended event session collection
                string victim = deadlock.DeadlockVictim ?? ( deadlock.DeadlockVictimList.Length >0 ? deadlock.DeadlockVictimList[0].Id: String.Empty);

                foreach (DeadlockProcess process in deadlock.ProcessList)
                {
                    if (!String.IsNullOrEmpty(process.Id))
                    {
                        process.Victim = String.Equals(victim, process.Id, StringComparison.InvariantCultureIgnoreCase);
                        processMap.Add(process.Id, process);
                    }
                }

                DeadlockResourceList resourceList = deadlock.resourceList;
                if (resourceList != null)
                {
                    DeadlockProcess process;
                    foreach (DeadlockResource resource in resourceList.ResourceList)
                    {
                        foreach (DeadlockResourceOwner owner in resource.ownerlist)
                        {
                            UltraDataRow row = ds.Rows.Add();
                            if (processMap.TryGetValue(owner.id, out process))
                            {
                                AddResourceInformation(row, process, resource);
                                row[OWN_WAIT] = "Own";
                                row[MODE] = owner.mode;
                                AddProcessInformation(row, process);
                            }
                        }
                        foreach (DeadlockResourceWaiter waiter in resource.waiterlist)
                        {
                            if (processMap.TryGetValue(waiter.id, out process))
                            {
                                UltraDataRow row = ds.Rows.Add();
                                if (processMap.TryGetValue(waiter.id, out process))
                                {
                                    AddResourceInformation(row, process, resource);
                                    row[OWN_WAIT] = "Wait";
                                    row[MODE] = waiter.mode;
                                    AddProcessInformation(row, process);
                                }
                            }
                        }
                    }
                }
            }
            return ds;
        }

        public void AddProcessInformation(UltraDataRow row, DeadlockProcess process)
        {
            row[DEADLOCK_PROCESS] = process;
            //table.Columns.Add("Host", typeof(string));
            row["Host"] = (object)process.hostname ?? DBNull.Value;

            row["Isolation Level"] = (object)process.isolationlevel ?? DBNull.Value;
            row["Lock Mode"] = (object)process.lockMode ?? DBNull.Value;
            row["Lock Timeout"] = (object)process.lockTimeout ?? DBNull.Value;
            row["Login"] = (object)process.loginname ?? DBNull.Value;
            row["Priority"] = (object)process.priority ?? DBNull.Value;
            row["Status"] = (object)process.status ?? DBNull.Value;
            row["Transaction Name"] = (object)process.transactionname ?? DBNull.Value;
//            row["Wait Resource"] = (object)process.waitresource ?? DBNull.Value;
            row["Wait Time"] = (object)process.waittime ?? DBNull.Value;
            row[VICTIM] = process.Victim;
            if (String.IsNullOrEmpty(process.spid))
                row[SPID] = DBNull.Value;
            else
                row[SPID] = Int32.Parse(process.spid);
            if (String.IsNullOrEmpty(process.ecid))
                row[ECID] = DBNull.Value;
            else
                row[ECID] = Int32.Parse(process.ecid);

            row[APPLICATION] = process.clientapp;

            // Set process sql = inputbuf.  If inputbuf is empty then use last stack frame.  if still empty then use spaces.
            string inputbuf = process.inputbuf;
            if (!String.IsNullOrEmpty(inputbuf))
            {
                row["Procedure"] = String.Empty;
                row["Line"] = DBNull.Value;
                row[SQL] = FixupLineBreaks(inputbuf ?? String.Empty);
            } else
            {
                DeadlockExecutionStackFrame frame = GetLastStackFrame(process);
                if (frame == null || String.IsNullOrEmpty(frame.Value))
                {
                    row["Procedure"] = String.Empty;
                    row["Line"] = DBNull.Value;
                    row[SQL] = String.Empty;
                }
                else
                {
                    row["Procedure"] = (object)frame.procname ?? DBNull.Value;
                    if (String.IsNullOrEmpty(frame.line))
                        row["Line"] = DBNull.Value;
                    else
                        row["Line"] = Int32.Parse(frame.line);

                    if (!String.IsNullOrEmpty(frame.Value))
                        row[SQL] = FixupLineBreaks(frame.Value.TrimStart('\n'));
                    else
                        row[SQL] = String.Empty;
                }
            }

            if (process.executionStack != null)
            {
                // add all the stack frames as child rows
                UltraDataRowsCollection frames = row.GetChildRows("Frame");
                foreach (DeadlockExecutionStackFrame f in process.executionStack)
                {
                    UltraDataRow frow = frames.Add();
                    frow["Procedure"] = (object)f.procname ?? DBNull.Value;
                    if (String.IsNullOrEmpty(f.line))
                        frow["Line"] = DBNull.Value;
                    else
                        frow["Line"] = Int32.Parse(f.line);

                    if (!String.IsNullOrEmpty(f.Value))
                        frow[SQL] = FixupLineBreaks(f.Value.TrimStart('\n'));
                    else
                        frow[SQL] = String.Empty;
                }
            }
        }

        private void AddResourceInformation(UltraDataRow row, DeadlockProcess process, DeadlockResource resource)
        {

            string resourceType = resource.ResourceType;
            string waitResource = process.waitresource ?? String.Empty;

            switch (resource.ResourceType)
            {
                case "pagelock":
                    resourceType = "Page";
                    waitResource = FormatPageId(resource, waitResource);
                    break;
                case "keylock":
                    resourceType = "Key";
                    waitResource = FormatKeyId(resource, waitResource);
                    break;
                case "ridlock":
                    resourceType = "RID";
                    break;
                case "objectlock":
                    resourceType = "Object";
                    waitResource = FormatObjectId(resource, waitResource);
                    break;
                case "hobtlock":
                    resourceType = "HOBT";
                    waitResource = FormatHobtId(resource, waitResource);
                    break;
                case "databaselock":
                    resourceType = "DATABASE";
                    waitResource = FormatDatabaseId(resource, waitResource);
                    break;
                case "applicationlock":
                    resourceType = "APPLICATION";
                    waitResource = FormatAppId(resource, waitResource);
                    break;
                case "exchangeEvent":
                    resourceType = "Event";
                    break;
                case "threadpool":
                    resourceType = "Thread";
                    break;
                default:
                    break;
            }
            row[LOCK_TYPE] = resourceType;
            row["Wait Resource"] = waitResource;

            AddDynamicProperties(row, resource);
        }

        private string FormatAppId(DeadlockResource resource, string defaultValue)
        {
            bool complete = true;
            string dbid = GetDynamicProperty(resource, "dbid");
            if (String.IsNullOrEmpty(dbid))
                complete = false;
            string pid = GetDynamicProperty(resource, "databasePrincipalId");
            if (String.IsNullOrEmpty(pid))
                complete = false;
            string hash = GetDynamicProperty(resource, "hash");
            if (String.IsNullOrEmpty(hash))
                complete = false;

            if (!complete)
                return defaultValue;

            return String.Format("APPLICATION: {0}:{1}:{2}", dbid, pid, hash);
        }

        private string FormatDatabaseId(DeadlockResource resource, string defaultValue)
        {
            bool complete = true;
            string dbid = GetDynamicProperty(resource, "dbid");
            if (String.IsNullOrEmpty(dbid))
                complete = false;

            if (!complete)
                return defaultValue;

            return String.Format("DATABASE: {0}", dbid);
        }

        private string FormatHobtId(DeadlockResource resource, string defaultValue)
        {
            bool complete = true;
            string dbid = GetDynamicProperty(resource, "dbid");
            if (String.IsNullOrEmpty(dbid))
                complete = false;
            string hobt = GetDynamicProperty(resource, "hobtid");
            if (String.IsNullOrEmpty(hobt))
                complete = false;

            if (!complete)
                return defaultValue;

            return String.Format("HOBT: {0}:{1}", dbid, hobt);
        }

        private string FormatPageId(DeadlockResource resource, string defaultValue)
        {
            bool complete = true;
            string dbid = GetDynamicProperty(resource, "dbid");
            if (String.IsNullOrEmpty(dbid))
                complete = false;
            string file = GetDynamicProperty(resource, "fileid");
            if (String.IsNullOrEmpty(file))
                complete = false;
            string page = GetDynamicProperty(resource, "pageid");
            if (String.IsNullOrEmpty(page))
                complete = false;

            if (!complete)
                return defaultValue;

            return String.Format("PAGE: {0}:{1}:{2}", dbid, file, page);
        }

        private string FormatKeyId(DeadlockResource resource, string defaultValue)
        {
            bool complete = true;
            string dbid = GetDynamicProperty(resource, "dbid");
            if (String.IsNullOrEmpty(dbid))
                complete = false;
            string hobtid = GetDynamicProperty(resource, "hobtid");
            if (String.IsNullOrEmpty(hobtid))
                complete = false;

            if (!complete)
                return defaultValue;

            return String.Format("KEY: {0}:{1}", dbid, hobtid);
        }

        private string FormatObjectId(DeadlockResource resource, string defaultValue)
        {
            bool complete = true;
            string dbid = GetDynamicProperty(resource, "dbid");
            if (String.IsNullOrEmpty(dbid))
                complete = false;
            string objid = GetDynamicProperty(resource, "objid");
            if (String.IsNullOrEmpty(objid))
                complete = false;

            if (!complete)
                return defaultValue;

            return String.Format("OBJECT: {0}:{1}", dbid, objid);
        }


        private string GetDynamicProperty(IDynamicPropertyProvider propProvider, string key)
        {
            if (propProvider == null)
                return null;
            
            NameValueCollection values = propProvider.DynamicProperties;
            if (values != null)
                return values.Get(key);

            return null;
        }

        private void AddDynamicProperties(UltraDataRow row, IDynamicPropertyProvider propProvider)
        {
            if (propProvider == null || propProvider.DynamicProperties == null)
                return;
            
            foreach (string key in propProvider.DynamicProperties.AllKeys)
            {
                string value = propProvider.DynamicProperties[key];
                if (row.Band.Columns.Contains(key))
                    row[key] = value;
            }
        }

        public static DeadlockExecutionStackFrame GetFirstStackFrame(DeadlockProcess process)
        {
            DeadlockExecutionStackFrame[] stackFrames = process.executionStack;
            if (stackFrames == null || stackFrames.Length == 0)
                return null;

            return stackFrames[0];
        }

        public static DeadlockExecutionStackFrame GetLastStackFrame(DeadlockProcess process)
        {
            DeadlockExecutionStackFrame[] stackFrames = process.executionStack;
            if (stackFrames == null || stackFrames.Length == 0)
                return null;

            return stackFrames[stackFrames.Length - 1];
        }

        public void AddDynamicColumns(UltraDataSource ds, DeadlockData data)
        {
            foreach (Deadlock deadlock in data.Items)
            {
                DeadlockResourceList resourceList = deadlock.resourceList;
                if (resourceList != null)
                {
                    foreach (DeadlockResource resource in resourceList.ResourceList)
                    {
                        // add a column for each dynamic property in the resource
                        NameValueCollection props = resource.DynamicProperties;
                        if (props != null && props.Count > 0)
                        {
                            foreach (string key in props.AllKeys)
                            {
                                if (!ds.Band.Columns.Exists(key))
                                {
                                    ds.Band.Columns.Add(key);
                                }
                            }
                        }

                        // add a column for each dynamic property in the owners
                        foreach (DeadlockResourceOwner owner in resource.ownerlist)
                        {
                            props = owner.DynamicProperties;
                            if (props != null && props.Count > 0)
                            {
                                foreach (string key in props.AllKeys)
                                {
                                    if (!ds.Band.Columns.Exists(key))
                                    {
                                        ds.Band.Columns.Add(key);
                                    }
                                }
                            }
                        }
                        // add a column for each dynamic property in the waiters
                        foreach (DeadlockResourceWaiter waiter in resource.waiterlist)
                        {
                            props = waiter.DynamicProperties;
                            if (props != null && props.Count > 0)
                            {
                                foreach (string key in props.AllKeys)
                                {
                                    if (!ds.Band.Columns.Exists(key))
                                    {
                                        ds.Band.Columns.Add(key);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private string FixupLineBreaks(string input)
        {
            StringBuilder result = new StringBuilder();
            
            char pch = ' ';
            foreach (char ch in input)
            {
                if (ch == '\n' && pch != '\r')
                {   // make sure all \n are preceeded with a \r
                    result.Append('\r');
                }
                if (ch == '\t')
                {   // expand tabs
                    result.Append("   ");
                    pch = ' ';
                    continue;
                }
                result.Append(ch);
                pch = ch;
            }
           
            // strip all leading control characters
            while (result.Length > 0 && Char.IsControl(result[0]))
                result.Remove(0, 1);

            return result.ToString();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void flatGrid_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            UltraGridRow row = e.Row;
            if (row.Band.Key == BAND_RESOURCE)
            {
                if (row.Cells.Exists(VICTIM))
                {
                    UltraGridCell cell = row.Cells[VICTIM];
//                    if (!cell.Column.Hidden)
//                        cell.Column.Hidden = true;
                    
                    bool value = (bool) cell.Value;
                    if (value)
                    {
                        row.Appearance.BackColor = Color.Pink;
                    }
                    else
                    {
                        row.Appearance.BackColor = Color.White;
                    }
                }
            }
        }

        private void toolbarManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "showColumnChooserButton":    // ButtonTool
                    ShowColumnChooser();
                    break;
                case "toggleGroupByBoxButton":    // ButtonTool
                    ToggleGroupByBox();
                    break;
                case "sortAscendingButton":    // ButtonTool
                    SortSelectedColumnAscending();
                    break;
                case "sortDescendingButton":    // ButtonTool
                    SortSelectedColumnDescending();
                    break;
                case "removeThisColumnButton":    // ButtonTool
                    RemoveSelectedColumn();
                    break;
                case "groupByThisColumnButton":    // StateButtonTool
                    GroupBySelectedColumn(((StateButtonTool)e.Tool).Checked);
                    break;
                case "printGridButton":    // ButtonTool
                    PrintGrid();
                    break;
                case "exportGridButton":    // ButtonTool
                    SaveGrid();
                    break;
                case "collapseAllGroupsButton":    // ButtonTool
                    CollapseAllGroups();
                    break;
                case "expandAllGroupsButton":    // ButtonTool
                    ExpandAllGroups();
                    break;
                case "copyToClipboardButton":    // ButtonTool
                    UltraGridHelper.CopyToClipboard(flatGrid, UltraGridHelper.CopyOptions.AllSelectedRows, UltraGridHelper.CopyFormat.AllFormats);
                    break;
                case "exchangeEventsStateButton":    // StateButtonTool
                    UpdateFilters();
                    break;
                case "threadPoolStateButton":    // StateButtonTool
                    UpdateFilters();
                    break;
                case "showDetailsButton":    // StateButtonTool
                    Settings.Default.DeadlockViewDetailsPanelExpanded = true;
                    break;
            }
        }

        private void PrintGrid()
        {
            ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            ultraGridPrintDocument.Header.TextLeft = Text;
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";
            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid()
        {
            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.FileName = "Deadlock";
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ultraGridExcelExporter.Export(flatGrid, saveFileDialog.FileName);
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
                flatGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                flatGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                flatGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                flatGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void ToggleGroupByBox()
        {
            flatGrid.DisplayLayout.GroupByBox.Hidden = !flatGrid.DisplayLayout.GroupByBox.Hidden;
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    flatGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    flatGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
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
            flatGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            flatGrid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(flatGrid);
            dialog.Show(this);
        }


        private void UpdateFilters()
        {
            bool showExchangeEvents = ((StateButtonTool) toolbarManager.Tools["exchangeEventsStateButton"]).Checked;
            bool showThreadPoolEvents = ((StateButtonTool) toolbarManager.Tools["threadPoolStateButton"]).Checked;

            UltraGridBand band = flatGrid.DisplayLayout.Bands["Resource"];
            band.ColumnFilters[LOCK_TYPE].ClearFilterConditions();
            band.Override.RowFilterMode = RowFilterMode.AllRowsInBand;

            flatGrid.Rows.ColumnFilters[LOCK_TYPE].ClearFilterConditions();
            if (!showExchangeEvents)
                flatGrid.Rows.ColumnFilters[LOCK_TYPE].FilterConditions.Add(FilterComparisionOperator.NotEquals, "Event");
            if (!showThreadPoolEvents)
                flatGrid.Rows.ColumnFilters[LOCK_TYPE].FilterConditions.Add(FilterComparisionOperator.NotEquals, "Thread");
            flatGrid.Rows.ColumnFilters.LogicalOperator = FilterLogicalOperator.And;
            flatGrid.DisplayLayout.RefreshFilters();
        }


        private void toolbarManager_BeforeToolDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "gridContextMenu")
            {
                //if (flatGrid.Selected.Rows.Count > 0)
               // {
                //    //bool isViewHistoricalSnapshotButtonEnabled = false;
                //    //bool isViewAlertHelpButtonEnabled = false;
                //    //bool isViewDeadlockDetailsButtonEnabled = false;


                //      ((PopupMenuTool)e.Tool).Tools["viewDeadlockDetailsButton"].SharedProps.Visible =
                //    //    isViewDeadlockDetailsButtonEnabled;
                //    //((PopupMenuTool)e.Tool).Tools["viewRealTimeSnapshotButton"].SharedProps.Visible =
                //    //    isViewRealTimeSnapshotButtonEnabled;
                //    //((PopupMenuTool)e.Tool).Tools["viewHistoricalSnapshotButton"].SharedProps.Visible =
                //    //    isViewHistoricalSnapshotButtonEnabled;
                //    //((PopupMenuTool)e.Tool).Tools["viewAlertHelpButton"].SharedProps.Visible =
                //    //    isViewAlertHelpButtonEnabled;
                //}
                //else
                //{
                //    ((PopupMenuTool)e.Tool).Tools["viewDeadlockDetailsButton"].SharedProps.Visible = false;
                //    ((PopupMenuTool)e.Tool).Tools["viewRealTimeSnapshotButton"].SharedProps.Visible = false;
                //    ((PopupMenuTool)e.Tool).Tools["viewHistoricalSnapshotButton"].SharedProps.Visible = false;
                //    ((PopupMenuTool)e.Tool).Tools["viewAlertHelpButton"].SharedProps.Visible = false;
                //}

                ((PopupMenuTool)e.Tool).Tools["showDetailsButton"].SharedProps.Visible = !detailsPanel.Visible;
                bool isGrouped = flatGrid.Rows.Count > 0 && flatGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }

        }

        private void flatGrid_MouseDown(object sender, MouseEventArgs e)
        {
            bool enableClearOptions = false;

            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    UIElement selectedElement =
                        ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));

                    object contextObject = null;
                    if (selectedElement != null)
                        contextObject = selectedElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.ColumnHeader));

                    if (contextObject is Infragistics.Win.UltraWinGrid.ColumnHeader)
                    {
                        ColumnHeader columnHeader = contextObject as ColumnHeader;
                        selectedColumn = columnHeader.Column;
                        ((StateButtonTool)toolbarManager.Tools["groupByThisColumnButton"]).Checked =
                            selectedColumn.IsGroupByColumn;
                        toolbarManager.SetContextMenuUltra(((UltraGrid)sender), "columnContextMenu");
                        ConfigureContextMenuItems();
                    }
                    else
                    {
                        toolbarManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                        if (selectedElement != null && selectedElement.SelectableItem is UltraGridRow)
                        {
//                            ButtonTool tool = (ButtonTool)toolbarManager.Tools["editAlertConfigurationButton"];
                            UltraGridRow row = ((UltraGridRow)selectedElement.SelectableItem);
                            if (row.IsDataRow)
                            {
                                flatGrid.Selected.Rows.Clear();
                                row.Activate();
                                row.Selected = true;
                                // set the alert configuration edit button enabled if the selected row has a server name
//                                UltraDataRow dataRow = (UltraDataRow)row.ListObject;
//                                object server = dataRow["ServerName"];
//                                toolbarManager.Tools["editAlertConfigurationButton"]
//                                    .SharedProps.Visible = (server is string && !String.IsNullOrEmpty((string)server));

//                                bool enableSnoozeButton = false;
//                                tool = (ButtonTool)toolbarManager.Tools["snoozeAlertButton"];
//                                tool.SharedProps.Visible = enableSnoozeButton;
                            }
                            else
                            {
                                toolbarManager.Tools["editAlertConfigurationButton"].SharedProps.Visible = false;
                            }
                            toolbarManager.Tools["copyToClipboardButton"].SharedProps.Visible = true;
                        }
                        else
                        {
                            toolbarManager.Tools["editAlertConfigurationButton"].SharedProps.Visible = false;
                            toolbarManager.Tools["copyToClipboardButton"].SharedProps.Visible = false;
                        }
                    }
                    toolbarManager.Tools["clearAlertButton"].SharedProps.Visible = enableClearOptions;
                    toolbarManager.Tools["clearAllAlertsButton"].SharedProps.Visible = enableClearOptions;
                }
                catch (Exception err)
                {
                    LOG.Error("Error handling right mouse click: ", err);
                }
            }

        }

        private void ConfigureContextMenuItems()
        {
           
        }

        internal static void Show(IWin32Window owner, long alertId)
        {
            try
            {
                int serverID;
                DateTime collected;
                string xdl;

                SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
                if (!RepositoryHelper.GetLinkedAlertData(connectionInfo, alertId, out serverID, out collected, out xdl) || xdl == null)
                {
                    ApplicationMessageBox.ShowError(owner, "The linked deadlock graph is no longer available in the SQLDM Repository.");
                    return;
                }

                MonitoredSqlServerWrapper mssw = ApplicationModel.Default.ActiveInstances[serverID];

                try
                {
                    DeadlockDialog dialog = new DeadlockDialog(xdl);
                    if (mssw != null)
                    {
                        dialog.Text = String.Format("Deadlock - {0} at {1}", mssw.InstanceName, collected.ToLocalTime());
                    }
                    else
                        dialog.Text = String.Format("Deadlock - {0}", collected.ToLocalTime());

                    dialog.Show(owner);
                }
                catch (Exception)
                {
                    
                }
            } catch (Exception)
            {
                
            }
        }
        // <summary>
        /// Srishti Purohit SQLdm 10.0 - Doctor integration function added -- Learn more link for dependent objects
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <returns></returns>
        internal static void Show(IWin32Window owner, DeadlockRecommendation recommendation)
        {
            try
            {
                DeadlockDialog dialog = new DeadlockDialog(recommendation.XDL);
                dialog.Text = String.Format("Deadlock - {0}", recommendation.StartTime);
                dialog.Show(owner);
            }
            catch (Exception e)
            {
                LOG.Error("DeadlockDialog Show() Exception: ", e);
            }
        }

        internal static void Show(IWin32Window owner, DateTime collected, string xdl)
        {
            try
            {
                DeadlockDialog dialog = new DeadlockDialog(xdl);
                dialog.Text = String.Format("Deadlock - {0}", collected.ToLocalTime());
                dialog.Show(owner);
            }
            catch (Exception e)
            {
                LOG.Error("Error Showing the Deadlock Report: ", e);
            }
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            ExportHelper.ExportDeadlockGraph(this, xdl, "deadlock.xdl");
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void toggleDetailsPanelButton_Click(object sender, EventArgs e)
        {
            Settings.Default.DeadlockViewDetailsPanelExpanded = !Settings.Default.DeadlockViewDetailsPanelExpanded;
        }

        private void flatGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void flatGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if (e.Type == typeof(UltraGridRow) && flatGrid.Selected.Rows.Count > 0)
            {
                UltraGridRow row = flatGrid.Selected.Rows[0];
                if (row.IsDataRow)
                {
                    UltraDataRow data = row.ListObject as UltraDataRow;
                    if (data != null)
                    {
                        UpdateDetailsTab(data);
                        return;
                    }
                }
            }
            UpdateDetailsTab(null);
        }

        private static Control GetFocusedControl(Control.ControlCollection controls)
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

        private void DeadlockDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        public void SaveSetting()
        {
            Settings.Default.DeadlockViewSplitterDistance = splitContainer1.SplitterDistance;
        }

        private void UpdateDetailsTab(UltraDataRow row)
        {
            DeadlockProcess process = null;
            if (row != null)
            {
                UltraDataRow parentRow = row;
                if (row.Band.Key != BAND_RESOURCE)
                {
                    parentRow = row.ParentRow;
                }

                process = parentRow[DEADLOCK_PROCESS] as DeadlockProcess;
            }

            if (process == null)
            {
                ClearDetailsTab();
                return;
            }

            boundSpidLabel.Text = process.spid;
            boundUserLabel.Text = process.loginname;
            boundHostLabel.Text = process.hostname;
            boundDatabaseLabel.Text = process.Database;

            boundStatusLabel.Text = process.status;
            boundOpenTransactionsLabel.Text = process.transcount;
            boundApplicationLabel.Text = process.clientapp;
            boundExecutionContextLabel.Text = process.ecid;
            boundWaitTimeLabel.Text = String.Format("{0}", process.waittime);
            boundWaitTypeLabel.Text = process.status;
            boundWaitResourceLabel.Text = process.waitresource;

            boundBatchCompleteLabel.Text = FormatXmlDateTime(process.lastbatchcompleted);
            boundBatchStartLabel.Text = FormatXmlDateTime(process.lastbatchstarted);
            boundLastTransLabel.Text = FormatXmlDateTime(process.lasttranstarted);

            boundTransNameLabel.Text = process.transactionname;
            boundXactIdLabel.Text = process.xactid;

            boundLastCommandTextBox.Text = (row[SQL] as string) ?? String.Empty;
        }

        private static string FormatXmlDateTime(string p)
        {
            String result = String.Empty;
            try
            {
                DateTime dt = Convert.ToDateTime(p);
                result = String.Format("{0}", dt);
            } catch (Exception)
            {
            }

            if (String.IsNullOrEmpty(result))
                result = p;

            return result ?? String.Empty;
        }

        private void ClearDetailsTab()
        {
            boundSpidLabel.Text = String.Empty;
            boundUserLabel.Text = String.Empty;
            boundHostLabel.Text = String.Empty;
            boundDatabaseLabel.Text = String.Empty;
            boundStatusLabel.Text = String.Empty;
            boundOpenTransactionsLabel.Text = String.Empty;
            boundApplicationLabel.Text = String.Empty;
            boundExecutionContextLabel.Text = String.Empty;
            boundWaitTimeLabel.Text = String.Empty;
            boundWaitTypeLabel.Text = String.Empty;
            boundWaitResourceLabel.Text = String.Empty;
            boundLastCommandTextBox.Text = String.Empty;

            boundTransNameLabel.Text = String.Empty;
            boundXactIdLabel.Text = String.Empty;

            boundBatchCompleteLabel.Text = String.Empty;
            boundBatchStartLabel.Text = String.Empty;
            boundLastTransLabel.Text = String.Empty;

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