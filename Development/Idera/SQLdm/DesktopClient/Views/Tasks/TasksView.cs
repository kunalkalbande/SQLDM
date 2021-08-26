using System;
using System.Threading;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Views.Alerts;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Resources=Idera.SQLdm.DesktopClient.Properties.Resources;
using BBS.TracerX;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Views.Tasks
{
    internal partial class TasksView : View {
        #region Data members
        private UltraGridColumn selectedColumn = null;
        private bool _loaded = false;
        private StandardTasksViews _currentView = StandardTasksViews.None;
        private StandardTasksViews _selectedView = StandardTasksViews.None;
        //private string _numItemsFormat;
        private List<UltraGridColumn> _designerColumns;
        ValueList _metricValues; // Set in OnLoad().
        private bool showingTaskPropertiesDialog = false;

        // Flag indicating user does not have permission to monitor server filter.
        private bool filterInstanceNoPermission = false;


        // Filter parameters to use on the next RefreshView.
        // These are set whenever the user clicks the Apply
        // button or a radio button in the nav pane.  The
        // refresh code uses these values as opposed to whatever
        // is in the filter controls, which may not be committed.
        const decimal DefaultDaysOld = 7;
        private decimal _currentDaysOld = DefaultDaysOld;
        private ValueListItem _currentStatus;
        private ValueListItem _currentSeverity;
        private ValueListItem _currentInstance;
        private Wrapper<Tag> _currentTag;
        private string _currentOwner = string.Empty;

        private decimal _customDaysOld = DefaultDaysOld;
        private ValueListItem _customStatus;
        private ValueListItem _customSeverity;
        private ValueListItem _customInstance;
        private Wrapper<Tag> _customTag;
        private string _customOwner = string.Empty;

        // These are special items in the UltraCombos.
        private ValueListItem _anyStatus;
        private ValueListItem _completedStatus;
        private ValueListItem _notCompletedStatus;
        private ValueListItem _anySeverity;
        private ValueListItem _anyInstanceRow;

        // These are special columns in the tasksGrid.
        private UltraGridColumn _ownerCol;
        private UltraGridColumn _statusCol;

        // Used for maping severity values to strings.
        private ValueList _severityList;

        // Column keys for the grid and the data table.
        private const string _colTaskID = "TaskID";
        private const string _colStatus = "Status";
        private const string _colStatusIcon = "StatusIcon";
        private const string _colCompletedCheck = "CompletedCheck";
        private const string _colSubject = "Subject";
        private const string _colMessage = "Message";
        private const string _colServerName = "ServerName";
        private const string _colComments = "Comments";
        private const string _colOwner = "Owner";
        private const string _colCreatedOn = "CreatedOn";
        private const string _colCompletedOn = "CompletedOn";
        private const string _colMetric = "Metric";
        private const string _colSeverity = "Severity";
        private const string _colSeverityIcon = "SevIcon";
        private const string _colValue = "Value";
        private const string _colEventID = "EventID";

        //last Settings values used to determine if changed for saving when leaving
        private GridSettings lastMainGridSettings = null;

        #endregion

        #region Initialization
        // ctor
        public TasksView() 
        {
            InitializeLogger("Todo List View");

            using (Log.InfoCall("TasksView.ctor")) {
                InitializeComponent();
                SetGridTheme();
                ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);

                tasksGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            }
        }

        // OnLoad does most of the initialization because the grid doesn't seem to be fully
        // initialized in the constructor (e.g. the columns collection is empty).
        protected override void OnLoad(EventArgs e) {
            using (Log.DebugCall()) {
                Log.Debug("_loaded = ", _loaded);
                base.OnLoad(e);

                if (!_loaded) {
                    Debug.Print("TasksView.OnLoad called first time.");

                    // Make a list of the columns specified in the designer.
                    // When we bind the DataTable to the grid, any "extra" columns will be suppressed.
                    // Note that the grid's column collection is empty in the constructor.
                    _designerColumns = new List<UltraGridColumn>();
                    foreach (UltraGridColumn col in tasksGrid.DisplayLayout.Bands[0].Columns) {
                        _designerColumns.Add(col);
                    }

                    tasksGrid.DisplayLayout.Bands[0].Columns[_colCreatedOn].GroupByEvaluator = new DateGroupByEvaluator();

                    InitializeTagCombo();
                    InitSeverityCombo();
                    InitStatusCombo();
                    InitInstanceCombo();
                    ConfigureFilterOptionsPanelVisible(Settings.Default.TasksViewFilterOptionsPanelVisible);
                    Settings.Default.SettingChanging += new SettingChangingEventHandler(Settings_SettingChanging);

                    SetSeverityValueList();
                    SetStatusValueList();
                    SetMetricValueList();

                    _statusCol = tasksGrid.DisplayLayout.Bands[0].Columns[_colStatus];
                    _ownerCol = tasksGrid.DisplayLayout.Bands[0].Columns[_colOwner];

                    // Set the custom filter default values.
                    _customStatus = _anyStatus;
                    _customSeverity = _anySeverity;
                    _customInstance = _anyInstanceRow;

                    _loaded = true; // Now we are ready to preselect the filters.
                    SetView(_selectedView);
                }
            }
        }

        // Map Metric enums to strings in the grid.
        private void SetMetricValueList() {
            if (_metricValues == null)
            {   // get the value list and ensure column is using it
                _metricValues = tasksGrid.DisplayLayout.ValueLists.Add("MetricValues");
                tasksGrid.DisplayLayout.Bands[0].Columns[_colMetric].ValueList = _metricValues;
            }
            if (_metricValues.ValueListItems.Count == 0)
            {   // initial load
                _metricValues.ValueListItems.AddRange(ValueListHelpers.GetMetricValueListItems());
            }
            else
            {  
                MetricDescription? metricDescription;
                // grab changes to metric definitions
                MetricDefinitions metrics = ApplicationModel.Default.MetricDefinitions;
                metrics.Reload(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);
                // only custom counters get added/removed while the product is running
                foreach (int metricID in metrics.GetCounterDefinitionKeys())
                {
                    ValueListItem listItem = _metricValues.FindByDataValue(metricID);
                    if (listItem == null)
                    {
                        metricDescription = metrics.GetMetricDescription(metricID);
                        if (metricDescription.HasValue)
                        {
                            listItem = new ValueListItem(metricID, metricDescription.Value.Name);
                            _metricValues.ValueListItems.Add(listItem);
                        }
                    }
                }
            }
        }

        private void SetStatusValueList() {
            // This maps status values to strings in the grid
            ValueList stringValues = tasksGrid.DisplayLayout.ValueLists.Add("StatusStrings");
            stringValues.ValueListItems.AddRange(MakeStatusValues(false));
            tasksGrid.DisplayLayout.Bands[0].Columns[_colStatus].ValueList = stringValues;

            // build the task status icon value list
            ValueList valueList = tasksGrid.DisplayLayout.ValueLists["StatusIcons"];
            valueList.ValueListItems.Clear();
            valueList.ValueListItems.AddRange(ValueListHelpers.GetTaskStatusValueListItems(false));

            // switch out editor from EditorWithCombo to EditorWithText to prevent
            // wierd behavior when hovering over the status icon cells.
            EditorWithText textEditor = new EditorWithText();
            tasksGrid.DisplayLayout.Bands[0].Columns[_colStatusIcon].Editor = textEditor;

        }

        // Create ValueListItems to be used in the statusCombo and statusDetailsCombo.
        private ValueListItem[] MakeStatusValues(bool forFilter) {
            List<ValueListItem> values = new List<ValueListItem>(10);

            if (forFilter) {
                values.Add(new ValueListItem(TaskStatus.AnyAll, "All"));
                values.Add(new ValueListItem(~TaskStatus.Completed, "Not Completed"));
            }

            values.Add(new ValueListItem(TaskStatus.NotStarted, "Not Started"));
            values.Add(new ValueListItem(TaskStatus.InProgress, "In Progress"));
            values.Add(new ValueListItem(TaskStatus.OnHold, "On Hold"));
            values.Add(new ValueListItem(TaskStatus.Completed, "Completed"));

            ValueListItem[] array = new ValueListItem[values.Count];
            values.CopyTo(array);
            return array;
        }

        // Create ValueLists related to severity codes (i.e. MonitoredState).
        private void SetSeverityValueList() {
            // This maps severity values to strings in the grid.
            _severityList = tasksGrid.DisplayLayout.ValueLists.Add("SeverityStrings");

            _severityList.ValueListItems.Add(MonitoredState.OK, "OK");
            _severityList.ValueListItems.Add(MonitoredState.Informational, "Info");
            _severityList.ValueListItems.Add(MonitoredState.Warning, "Warning");
            _severityList.ValueListItems.Add(MonitoredState.Critical, "Critical");

            tasksGrid.DisplayLayout.Bands[0].Columns[_colSeverity].ValueList = _severityList;
        }
        private void InitStatusCombo() {
            // Initialize the statusCombo in the filter pane.
            statusCombo.Items.AddRange(MakeStatusValues(true));
            _anyStatus = statusCombo.Items[0];
            _notCompletedStatus = statusCombo.Items[1];
            _completedStatus = statusCombo.Items[5];

        }

        private void InitSeverityCombo() {
            // Initialize the severityCombo.
            ValueListItem[] severities = new ValueListItem[5];
            severities[0] = new ValueListItem(MonitoredStateFlags.All, "All");
            severities[1] = new ValueListItem(MonitoredStateFlags.OK, "OK");
            severities[2] = new ValueListItem(MonitoredStateFlags.Informational, "Informational");
            severities[3] = new ValueListItem(MonitoredStateFlags.Warning, "Warning");
            severities[4] = new ValueListItem(MonitoredStateFlags.Critical, "Critical");
            severityCombo.Items.AddRange(severities);
            _anySeverity = severities[0];
            //_currentSeverity = _anySeverity;
        }

        public void InitializeTagCombo()
        {
            // just add the all item - build the full list during OnBeforeDropDown
            BindingSource list = new BindingSource(components);
            list.Add(new Wrapper<string>("All"));

            tagCombo.DataSource = list;
            tagCombo.SelectedIndex = 0;
        }

        private void InitInstanceCombo() {

            // Initialize the instanaceCombo with the All item.  The rest of the items are filled during drop down event.
            ValueListItem[] instances = new ValueListItem[1];
            instances[0] = new ValueListItem(null, "All");
            instanceCombo.Items.AddRange(instances);
            _anyInstanceRow = instances[0];
        }
        #endregion

        override public void ShowHelp() {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.TasksView);
        }

        public override void ApplySettings()
        {
            if (Settings.Default.TasksViewMainGrid is GridSettings)
            {
                lastMainGridSettings = Settings.Default.TasksViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, tasksGrid);
            }
        }

        public override void SaveSettings()
        {
            GridSettings mainGridSettings = GridSettings.GetSettings(tasksGrid);
            // save all settings only if anything has changed
            if (!mainGridSettings.Equals(lastMainGridSettings))
            {
                lastMainGridSettings =
                    Settings.Default.TasksViewMainGrid = mainGridSettings;
            }
        }

        #region SetView/refresh/apply
        // This sets the grid's Group By settings based on the specified view.
        // Doing this seems to cause wierd things to happen with the selected items
        // and can therefore screw up the details pane (e.g. show details for the
        // wrong task, show details when no task is selected, or behave as if no
        // task is selected when one actually is). Therefore, this is called AFTER the
        // data from a refresh is placed in the grid and BEFORE the code that re-selects
        // the items is called (and only IF the refresh was preceded by a call to SetView).
        private void SetGroupByForView(StandardTasksViews view) {
            switch (view) {
                case StandardTasksViews.Active:
                case StandardTasksViews.Completed:
                    tasksGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                    break;
                case StandardTasksViews.ByOwner:
                    tasksGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                    tasksGrid.DisplayLayout.Bands[0].SortedColumns.Add(_ownerCol, false, true);
                    break;
                case StandardTasksViews.ByStatus:
                    tasksGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                    tasksGrid.DisplayLayout.Bands[0].SortedColumns.Add(_statusCol, false, true);
                    break;
                case StandardTasksViews.Custom:
                    tasksGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                    break;
            }

            _currentView = view;
        }

        // When the user clicks one of the radio buttons in the nav pane (and also when
        // this view is first shown), the following occurs.
        //
        // 1) SetView is called.  This sets the appropriate filter settings and sets _selectedView.
        // 2) RefreshView called.  This normally calls base.RefreshView,
        //    which starts the background worker thread.
        // 4) The worker thread calls DoRefreshWork, which queries the DB for the tasks.
        // 5) When DoRefreshWork returns, UpdateData is called on the main UI thread.
        // 6) UpdateData puts the new data in the grid.  If _currentView != _selectedView, it
        //    means the refresh was initiated by one of the nav pane radio buttons and the
        //    appropriate GroupBy setting must be applied.
        //
        // Clicking the Apply button is the same, except SetView is not called and
        // therefore _selectedView is not changed.
        //
        // SetView and RefreshView are first called by the application before the
        // OnLoad event.  Since we are not fully initialized at that time, those
        // calls return without doing anything significant other than setting _selectedView.
        // RefreshView is called again immediately after OnLoad and that call does the first real refresh.
        public void SetView(StandardTasksViews view) {
            using (Log.DebugCall()) {
                Log.Debug("view = ", view, ", _currentView = ", _currentView, ", _selectedView = ", _selectedView);

                // This function can be called when the outlook bar group is changed.
                // When the outlook bar group changes, it calls this function with the
                // input view set to ShowDefaultOrExisting.   The _selectedView is initialized to
                // None, so in this case set the view to the default value, i.e. Active.
                if (view == StandardTasksViews.ShowDefaultOrExisting)
                {
                    if (_selectedView == StandardTasksViews.None)
                    {
                        view = StandardTasksViews.Active;
                    }
                    else
                    {
                        view = _selectedView;
                    }
                }

                // This causes UpdateData to apply the appropriate GroupBy settings
                // at the end of the refresh cycle.
                _selectedView = view;

                if (_loaded) {
                    // Change the filter controls to match the selected view.
                    switch (view) {
                        case StandardTasksViews.Active:
                            instanceCombo.SelectedItem = _anyInstanceRow;
                            statusCombo.SelectedItem = _notCompletedStatus;
                            severityCombo.SelectedItem = _anySeverity;
                            lastDaysInput.Value = DefaultDaysOld;
                            ownerBox.Text = string.Empty;
                            break;
                        case StandardTasksViews.Completed:
                            instanceCombo.SelectedItem = _anyInstanceRow;
                            statusCombo.SelectedItem = _completedStatus;
                            severityCombo.SelectedItem = _anySeverity;
                            lastDaysInput.Value = DefaultDaysOld;
                            ownerBox.Text = string.Empty;
                            break;
                        case StandardTasksViews.ByOwner:
                        case StandardTasksViews.ByStatus:
                            instanceCombo.SelectedItem = _anyInstanceRow;
                            statusCombo.SelectedItem = _anyStatus;
                            severityCombo.SelectedItem = _anySeverity;
                            lastDaysInput.Value = DefaultDaysOld;
                            ownerBox.Text = string.Empty;
                            break;
                        case StandardTasksViews.Custom:
                            instanceCombo.SelectedItem = _customInstance;
                            statusCombo.SelectedItem = _customStatus;
                            severityCombo.SelectedItem = _customSeverity;
                            lastDaysInput.Value = _customDaysOld;
                            ownerBox.Text = _customOwner;
                            break;
                    }

                    // This saves the above changes in data members that are referenced
                    // by DoRefreshWork.
                    SaveFilter(false);
                }
            }
        }

        public override void RefreshView() {
            using (Log.DebugCall()) {
                // If the view hasn't been loaded yet, certain variables are still
                // uninitialized so we can't really show the requested view.  
                // OnLoad() will call this in the future to apply the view.
                Log.Debug("_loaded = ", _loaded);
                if (_loaded) {
                    // This starts the background worker thread unless it is already running.
                    base.RefreshView();
                }
            }
        }

        // Called by the base View class on a worker thread.
        public override object DoRefreshWork() {
            // update the metric value list 
            SetMetricValueList();

            // If the properties dialog is not being displayed, then get
            // the data from the repository for refreshing.
            DataTable o = null;

            if (!showingTaskPropertiesDialog)
            {
                // if no filter instance is specified filter contains all assigned servers.
                filterInstanceNoPermission = false; // clear the filter instance no permission flag
                XmlDocument instanceFilterXml = new XmlDocument();
                XmlElement rootElement = instanceFilterXml.CreateElement("Servers");
                instanceFilterXml.AppendChild(rootElement);

                if (_currentInstance.DataValue != null)
                {
                    XmlElement instanceElement = instanceFilterXml.CreateElement("Server");
                    instanceElement.SetAttribute("InstanceName", _currentInstance.DataValue as string);
                    instanceFilterXml.FirstChild.AppendChild(instanceElement);
                }
                else if (_currentTag != null && _currentTag.GetValue() != null)
                {
                    Tag selectedTag = _currentTag.GetValue();

                    if (ApplicationModel.Default.Tags.Contains(selectedTag.Id))
                    {
                        Tag tagFilter = ApplicationModel.Default.Tags[selectedTag.Id];

                        foreach (int instanceId in tagFilter.Instances)
                        {
                            if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
                            {
                                MonitoredSqlServer instance = ApplicationModel.Default.ActiveInstances[instanceId];
                                XmlElement instanceElement = instanceFilterXml.CreateElement("Server");
                                instanceElement.SetAttribute("InstanceName", instance.InstanceName);
                                instanceFilterXml.FirstChild.AppendChild(instanceElement);
                            }
                        }
                    }
                }
                else
                {
                    foreach (MonitoredSqlServer instance in ApplicationModel.Default.ActiveInstances)
                    {
                        XmlElement instanceElement = instanceFilterXml.CreateElement("Server");
                        instanceElement.SetAttribute("InstanceName", instance.InstanceName);
                        instanceFilterXml.FirstChild.AppendChild(instanceElement);
                    }
                }

                if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "TaskViewWorker";
                using (Log.DebugCall())
                {
                    DateTime start = DateTime.Now;
                    o = RepositoryHelper.GetTasks(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                        DateTime.UtcNow.AddDays(-(double)_currentDaysOld),
                        (TaskStatus)_currentStatus.DataValue,
                        (MonitoredStateFlags)_currentSeverity.DataValue,
                        instanceFilterXml.InnerXml,
                        _currentOwner == string.Empty ? null : _currentOwner);

                    TimeSpan span = DateTime.Now - start;
                    Log.Debug("RepositoryHelper.GetTasks took this long: ", span);
                }
            }

            return o;
        }

        // Called by the base View class when the refresh worker thread completes.
        // This runs on the main UI thread.
        public override void UpdateData(object data) {
            using (Log.DebugCall()) {
                // Parameter data should be what was returned by DoRefreshWork.
                if (data != null && data is DataTable) {
                    DataTable newTable = (DataTable)data;

                    AddSeverityImageColumn(newTable);
                    AddStatusImageColumn(newTable);
                    AddCompletedCheckColumn(newTable);

                    GridSettings gridSettings = GridSettings.GetSettings(tasksGrid);
                    UltraGridHelper.GridState state = UltraGridHelper.GetGridState(tasksGrid, _colTaskID);
                    tasksGrid.SuspendLayout();

                    if (tasksGrid.DataSource == null) {
                        // No old data source.  Just use the new one.
                        tasksGrid.DataSource = newTable;

                        // The DataTable has more columns than we want to expose to the user.
                        // They automatically get added to the grid, which we don't really want.
                        // Columns that weren't in the designer are already hidden.
                        // Also exclude them from the column chooser so the user never sees them.
                        foreach (UltraGridColumn col in tasksGrid.DisplayLayout.Bands[0].Columns) {
                            if (!_designerColumns.Contains(col)) {
                                col.ExcludeFromColumnChooser = ExcludeFromColumnChooser.True;
                            }
                        }

                        // Read-only columns cause problems when row values are replaced later.
                        foreach (DataColumn col in newTable.Columns) {
                            if (col.ReadOnly)
                                col.ReadOnly = false;
                        }

                        if (_currentView != _selectedView)
                        {
                            // This refresh is due to the user clicking one of the radio
                            // buttons in the nav pane (or the programmatic equivalent).
                            // Set the appropriate groupby.
                            SetGroupByForView(_selectedView);
                        }
                        else
                        {
                            GridSettings.ApplySettingsToGrid(gridSettings, tasksGrid);
                        }
                    } else {
                        GridSettings.ApplySettingsToGrid(gridSettings, tasksGrid);
                        MergeNewDataTable(newTable);
                    }

                    UltraGridHelper.RestoreGridState(state);
                    tasksGrid.ResumeLayout();

                    // If its custom view then set the string to be displayed as a filter.
                    string filterStatus = string.Empty;
                    switch (_currentView)
                    {
                        case StandardTasksViews.Active:
                            filterStatus = "Active To Do Items";
                            break;
                        case StandardTasksViews.Completed:
                            filterStatus = "Completed To Do Items"; 
                            break;
                        case StandardTasksViews.ByOwner:
                            filterStatus = "All To Do Items By Owner";
                            break;
                        case StandardTasksViews.ByStatus:
                            filterStatus = "All To Do Items By Status";
                            break;
                        case StandardTasksViews.Custom:
                            filterStatus = "Custom Filter";
                            break;
                        case StandardTasksViews.None:
                        case StandardTasksViews.ShowDefaultOrExisting:
                            break;
                        default:
                            break;
                    }

                    ApplicationController.Default.SetCustomStatus(
                            string.Empty,
                            String.Format("{0} Item{1}",
                                    newTable.Rows.Count,
                                    newTable.Rows.Count == 1 ? string.Empty : "s")
                            );
                    filterAppliedLabel.Text = filterStatus;

                    // If user does not have permission on the filter server, flag an error.
                    if (filterInstanceNoPermission)
                    {
                        string instance = _currentInstance.DataValue as string;
                        _customInstance = _currentInstance = _anyInstanceRow;
                        instanceCombo.SelectedIndex = 0;
                        ApplicationMessageBox.ShowInfo(this,
                                                       string.Format(
                                                           "The Server filter has been changed from [{0}] to All because you do not have permission to monitor this server.",
                                                                instance));
                    }

                } else {
                    Log.Debug("TasksView.UpdateData passed a non-DataTable object.");
                }

                ApplicationController.Default.OnRefreshActiveViewCompleted(new RefreshActiveViewCompletedEventArgs(DateTime.Now));
            }
        }

        // Merge rows from the new DataTable into the old DataTable that is the grid's
        // current data source.  This prevents the grid from totally resetting itself (e.g.
        // scroll position, sorting, grouping, and selections), which is what happens if
        // we just change the grid's DataSource property.
        private void MergeNewDataTable(DataTable newTable) {
            using (Log.DebugCall()) {
                DataTable oldTable = (DataTable)tasksGrid.DataSource;

                Log.DebugFormat("Old rows = {0}, new rows = {1}.", oldTable.Rows.Count, newTable.Rows.Count);

                // The current rows will be replaced by the new rows.
                // If any tasks are currently selected, they may not be in
                // the new list, or their positions may change.  To fix
                // this up later, get the IDs of the currently selected tasks.
                List<int> selectedIds = GetSelectedTaskIds();
                tasksGrid.Selected.Rows.Clear();

                // Set the GroupBy before reselecting the rows since
                // doing so tends to screw up the selections.
                if (_currentView != _selectedView) {
                    SetGroupByForView(_selectedView);
                }

                // Determine the lowest row count of the two tables.
                int leastCount = Math.Min(oldTable.Rows.Count, newTable.Rows.Count);

                for (int i = 0; i < leastCount; ++i) {
                    oldTable.Rows[i].ItemArray = newTable.Rows[i].ItemArray;
                }

                // If the old table has more rows than the new, remove them.
                while (oldTable.Rows.Count > newTable.Rows.Count) {
                    oldTable.Rows.RemoveAt(oldTable.Rows.Count - 1);
                }

                // If the new table has more rows than the old, add them.
                for (int i = leastCount; i < newTable.Rows.Count; ++i) {
                    oldTable.Rows.Add(newTable.Rows[i].ItemArray);
                }

                // Re-select the grid rows for the task IDs that were 
                // previously selected.                      
                foreach (UltraGridRow row in DataRows()) {
                    row.Selected = selectedIds.Contains(TaskIdFromRow(row));
                }

                // Resort the grid.
                tasksGrid.DisplayLayout.Bands[0].SortedColumns.RefreshSort(true);

                Log.Debug("Reselected ", tasksGrid.Selected.Rows.Count, " rows.");
            }
        }

        // Add a column to the DataTable for the security icon and populate
        // each row with the appropriate bitmap.
        private static void AddSeverityImageColumn(DataTable dataTable) {
            DataColumn iconCol = new DataColumn(_colSeverityIcon, typeof(Bitmap));

            Bitmap ok = Resources.StatusOKSmall;
            Bitmap info = Resources.StatusInfoSmall;
            Bitmap warning = Resources.StatusWarningSmall;
            Bitmap critical = Resources.StatusCriticalSmall;

            iconCol.ReadOnly = false;
            dataTable.Columns.Add(iconCol);

            foreach (DataRow row in dataTable.Rows) {
                switch ((MonitoredState)row[_colSeverity]) {
                    case MonitoredState.OK:
                        row[iconCol] = ok;
                        break;
                    case MonitoredState.Informational:
                        row[iconCol] = info;
                        break;
                    case MonitoredState.Warning:
                        row[iconCol] = warning;
                        break;
                    case MonitoredState.Critical:
                        row[iconCol] = critical;
                        break;
                }
            }
        }

        // Add a column to the DataTable for the status icon and populate
        // each row with the appropriate value.
        private static void AddStatusImageColumn(DataTable dataTable) {
            DataColumn iconCol = new DataColumn(_colStatusIcon, typeof(byte));

            iconCol.ReadOnly = false;
            dataTable.Columns.Add(iconCol);

            foreach (DataRow row in dataTable.Rows) {
                row[_colStatusIcon] = row[_colStatus];
            }
        }

        // Add a column to the DataTable for the status icon and populate
        // each row with the appropriate value.
        private static void AddCompletedCheckColumn(DataTable dataTable) {
            DataColumn iconCol = new DataColumn(_colCompletedCheck, typeof(bool));

            iconCol.ReadOnly = false;
            dataTable.Columns.Add(iconCol);

            foreach (DataRow row in dataTable.Rows) {
                row[_colCompletedCheck] = (TaskStatus)row[_colStatus] == TaskStatus.Completed;
            }
        }

        private List<int> GetSelectedTaskIds() {
            List<int> Ids = new List<int>();
            foreach (UltraGridRow row in tasksGrid.Selected.Rows) {
                if (row.IsDataRow) {
                    Ids.Add(TaskIdFromRow(row));
                }
            }

            Log.Debug("GetSelectedTaskIds returning ", Ids.Count, " IDs.");
            return Ids;
        }
        #endregion

        #region Filter pane
        private void toggleFilterOptionsPanelButton_Click(object sender, EventArgs e) {
            ToggleFilterOptionsPanelVisible();
        }

        private void ToggleFilterOptionsPanelVisible() {
            Settings.Default.TasksViewFilterOptionsPanelVisible = !Settings.Default.TasksViewFilterOptionsPanelVisible;
        }

        private void ConfigureFilterOptionsPanelVisible(bool filterOptionsVisible) {
            filterOptionsPanel.Visible = filterOptionsVisible;
            if (Settings.Default.ColorScheme == "Dark")
            {
                toggleFilterOptionsPanelButton.Image = filterOptionsVisible ? Resources.UpArrowsDark : Resources.DownArrowsDark;
            }
            else
            {
                toggleFilterOptionsPanelButton.Image = filterOptionsVisible ? Resources.UpArrows : Resources.DownArrows;
            }
        }

        // Called when any filter parameter control is changed.
        private void FilterControlChanged(object sender, EventArgs e) 
        {
            if (sender == tagCombo)
            {
                instanceCombo.SelectedIndex = 0;
            }

            applyButton.Enabled = true;
        }

        private void applyButton_Click(object sender, EventArgs e) {
            using (Log.InfoCall()) {
                // Save the current settings of the filter controls for
                // future refreshes to use.
                SaveFilter(true);

                // Start a refresh cycle.
                //ApplicationController.Default.RefreshActiveView();
                ApplicationController.Default.ShowTasksView(StandardTasksViews.Custom);
            }
        }

        // Save the filter controls into the data members that are referenced
        // by the refresh logic.
        private void SaveFilter(bool isSaveCurrent) {
            applyButton.Enabled = false; // Until user changes filter criteria.
            _currentTag = tagCombo.SelectedItem.ListObject as Wrapper<Tag>;
            _currentInstance = instanceCombo.SelectedItem;
            _currentStatus = statusCombo.SelectedItem;
            _currentSeverity = severityCombo.SelectedItem;
            _currentDaysOld = this.lastDaysInput.Value;
            _currentOwner = ownerBox.Text;
        
            if (isSaveCurrent)
            {
                _customTag = _currentTag;
                _customInstance = _currentInstance;
                _customStatus = _currentStatus;
                _customSeverity = _currentSeverity;
                _customDaysOld = _currentDaysOld;
                _customOwner = _currentOwner;
            }
        }

        private void clearButton_Click(object sender, EventArgs e) {
            tagCombo.SelectedIndex = 0;
            statusCombo.SelectedItem = _anyStatus;
            severityCombo.SelectedItem = _anySeverity;
            instanceCombo.SelectedItem = _anyInstanceRow;
            ownerBox.Text = string.Empty;
            lastDaysInput.Value = 7;
        }
        #endregion

        public StandardTasksViews GetSelectedView()
        {
            return _selectedView;
        }

        private void Settings_SettingChanging(object sender, SettingChangingEventArgs e) {
            switch (e.SettingName) {
                case "TasksViewFilterOptionsPanelVisible":
                    ConfigureFilterOptionsPanelVisible((bool)e.NewValue);
                    break;
            }
        }

        private static Control GetFocusedControl(Control.ControlCollection controls) {
            Control focusedControl = null;

            foreach (Control control in controls) {
                if (control.Focused) {
                    focusedControl = control;
                } else if (control.ContainsFocus) {
                    return GetFocusedControl(control.Controls);
                }
            }

            return focusedControl != null ? focusedControl : controls[0];
        }

        private int TaskIdFromRow(UltraGridRow gridRow) {
            return (int)gridRow.Cells[_colTaskID].Value;
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "rowContextMenu")
            {
                if (tasksGrid.Selected.Rows.Count > 0)
                {
                    toolbarsManager.Tools["deleteTask"].SharedProps.Visible =
                        ApplicationModel.Default.UserToken.IsSQLdmAdministrator;

                    ((PopupMenuTool)e.Tool).Tools["viewRealTimeSnapshotButton"].SharedProps.Visible = true;

                    bool viewHistoricalSnapshotButtonEnabled = false;
                    if (tasksGrid.Selected.Rows[0].Cells["Metric"].Value != null)
                    {
                        Metric m = MetricDefinition.GetMetric((int)tasksGrid.Selected.Rows[0].Cells["Metric"].Value);
                        viewHistoricalSnapshotButtonEnabled = ClickThroughHelper.ViewSupportsHistoricalSnapshots(m);
                    }

                    ((PopupMenuTool)e.Tool).Tools["viewHistoricalSnapshotButton"].SharedProps.Visible =
                        viewHistoricalSnapshotButtonEnabled;
                }
                else
                {
                    ((PopupMenuTool)e.Tool).Tools["viewRealTimeSnapshotButton"].SharedProps.Visible = false;
                    ((PopupMenuTool)e.Tool).Tools["viewHistoricalSnapshotButton"].SharedProps.Visible = false;
                }
            }

            if (e.Tool.Key == "rowContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = tasksGrid.Rows.Count > 0 && tasksGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
        }

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e) {
            using (Log.DebugCall()) {
                Log.Debug("e.Tool.Key = ", e.Tool.Key);
                switch (e.Tool.Key) {
                    // Column menu
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

                    // Row menu
                    case "deleteTask":
                        DeleteSelectedTasks();
                        break;
                    case "propertiesButton":
                        ShowTaskProperties();
                        break;
                    case "printGridButton":
                        PrintGrid();
                        break;
                    case "exportGridButton":
                        SaveGrid();
                        break;
                    case "notStartedButton":
                        SetSelectedTaskStatus(TaskStatus.NotStarted);
                        break;
                    case "inProgressButton":
                        SetSelectedTaskStatus(TaskStatus.InProgress);
                        break;
                    case "onHoldButton":
                        SetSelectedTaskStatus(TaskStatus.OnHold);
                        break;
                    case "completedButton":
                        SetSelectedTaskStatus(TaskStatus.Completed);
                        break;
                    case "collapseAllGroupsButton":
                        CollapseAllGroups();
                        break;
                    case "expandAllGroupsButton":
                        ExpandAllGroups();
                        break;
                    case "copyToClipboardButton":
                        UltraGridHelper.CopyToClipboard(tasksGrid, UltraGridHelper.CopyOptions.AllSelectedRows, UltraGridHelper.CopyFormat.AllFormats);
                        break;
                    case "viewRealTimeSnapshotButton":
                        NavigateToView(tasksGrid.Selected.Rows[0], false);
                        break;
                    case "viewHistoricalSnapshotButton":
                        NavigateToView(tasksGrid.Selected.Rows[0], true);
                        break;
                }
            }
        }

        private void DeleteSelectedTasks() 
        {
            if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator)
            {
                List<int> IDs = new List<int>();
                List<DataRow> dataRows = new List<DataRow>();
                DataTable dataTable = (DataTable) tasksGrid.DataSource;

                // Build a list of the task Ids and the corresponding rows in the DataTable.
                foreach (UltraGridRow row in tasksGrid.Selected.Rows)
                {
                    IDs.Add(TaskIdFromRow(row));
                    dataRows.Add(dataTable.Rows[row.ListIndex]);
                }

                // Delete the tasks in the DB.
                IManagementService defaultManagementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                defaultManagementService.DeleteTask(IDs);

                // This triggers the SelectionChanged event.
                tasksGrid.Selected.Rows.Clear();

                // If the Management Service call didn't throw an exception, delete the rows from the DataTable.
                // Because the grid is bound to the DataTable, this also deletes them
                // from the grid.
                foreach (DataRow dataRow in dataRows)
                {
                    dataTable.Rows.Remove(dataRow);
                }

                ApplicationController.Default.SetCustomStatus(
                    "Filter Applied",
                    String.Format("{0} Item{1}",
                                  dataTable.Rows.Count,
                                  dataTable.Rows.Count == 1 ? string.Empty : "s")
                    );
            }
        }

        private void SetSelectedTaskStatus(TaskStatus newStatus) {
            IManagementService defaultManagementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            // There is no API for bulk updates, so each one is a separate call
            // to the Management Service.
            foreach (UltraGridRow row in tasksGrid.Selected.Rows) {
                UpdateTaskStatus(defaultManagementService, row, newStatus);
            }
        }

        private void ShowAssociatedView()
        {
            string instanceName = tasksGrid.Selected.Rows[0].Cells["ServerName"].Value as string;
            if (tasksGrid.Selected.Rows[0].Cells["Metric"].Value != null)
            {
                Metric metric = MetricDefinition.GetMetric((int)tasksGrid.Selected.Rows[0].Cells["Metric"].Value);
                ClickThroughHelper.NavigateToView(instanceName, metric, null);
            }
        }

        private void ShowTaskProperties()
        {
            try
            {
                showingTaskPropertiesDialog = true;

                // If row is valid.
                UltraGridRow row = tasksGrid.Selected.Rows[0];
                if (row != null || row.IsDataRow)
                {
                    // Display properties dialog and update todo in Repository
                    // if user has made changes.
                    ToDoPropertiesDialog dlg = new ToDoPropertiesDialog(row);
                    DialogResult rc = dlg.ShowDialog();
                    if (rc == DialogResult.OK)
                    {
                        // Write new settings out to the MS.
                        IManagementService defaultManagementService =
                            ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                        defaultManagementService.UpdateTask(dlg.TaskId, dlg.TaskStatus, dlg.TaskOwner, dlg.TaskComments);

                        row.Cells[_colOwner].Value = dlg.TaskOwner;
                        row.Cells[_colComments].Value = dlg.TaskComments;
                        row.Cells[_colStatus].Value = dlg.TaskStatus;
                        row.Cells[_colStatusIcon].Value = dlg.TaskStatus;
                        row.Cells[_colCompletedCheck].Value = dlg.TaskStatus == TaskStatus.Completed;

                        //// When a task status is changed to completed, the completed on time
                        //// must also be updated.
                        if (dlg.TaskStatus == TaskStatus.Completed)
                        {
                            if (row.Cells[_colCompletedOn].Value == DBNull.Value)
                            {
                                row.Cells[_colCompletedOn].Value = DateTime.Now;
                            }
                        }
                        else
                        {
                            row.Cells[_colCompletedOn].Value = DBNull.Value;
                        }
                    }
                }
                else
                {
                    ApplicationMessageBox.ShowError(null, "Invalid To Do item data row specified.");
                    return;
                }
            }
            finally
            {
                showingTaskPropertiesDialog = false;
            }
        }

        private void UpdateTaskStatus(IManagementService defaultManagementService, UltraGridRow row, TaskStatus newStatus) {
            int taskId = TaskIdFromRow(row);
            string owner = row.Cells[_colOwner].Value as string;
            string comments = row.Cells[_colComments].Value as string;
            defaultManagementService.UpdateTask(taskId, newStatus, owner, comments);

            // Also update the row in the view
            row.Cells[_colStatus].Value = newStatus;
            row.Cells[_colStatusIcon].Value = newStatus;
            row.Cells[_colCompletedCheck].Value = newStatus == TaskStatus.Completed;
            
            if (newStatus == TaskStatus.Completed) {
                row.Cells[_colCompletedOn].Value = DateTime.Now;
            } else {
                row.Cells[_colCompletedOn].Value = DBNull.Value;
            }
        }

        private void PrintGrid() {
            ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            ultraGridPrintDocument.Header.TextLeft =
                    string.Format("To do list as of {0}",
                                        DateTime.Now.ToString("G")
                                    );
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid() {
            bool iconColHidden = tasksGrid.DisplayLayout.Bands[0].Columns["SevIcon"].Hidden;

            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.FileName = "ToDoList";
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    tasksGrid.DisplayLayout.Bands[0].Columns["SevIcon"].Hidden = true;
                    ultraGridExcelExporter.Export(tasksGrid, saveFileDialog.FileName);
                } catch (Exception ex) {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }

                tasksGrid.DisplayLayout.Bands[0].Columns["SevIcon"].Hidden = iconColHidden;
            }
        }

        private void SortSelectedColumnAscending() {
            if (selectedColumn != null) {
                tasksGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                tasksGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending() {
            if (selectedColumn != null) {
                tasksGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                tasksGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void ToggleGroupByBox() {
            tasksGrid.DisplayLayout.GroupByBox.Hidden = !tasksGrid.DisplayLayout.GroupByBox.Hidden;
        }

        private void GroupBySelectedColumn(bool GroupBy) {
            if (selectedColumn != null) {
                if (GroupBy) {
                    tasksGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                } else {
                    tasksGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
                }
            }
        }

        private void RemoveSelectedColumn() {
            if (selectedColumn != null) {
                selectedColumn.Hidden = true;
            }
        }

        private void CollapseAllGroups()
        {
            tasksGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            tasksGrid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser() {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(tasksGrid);
            dialog.Show(this);
        }

        private void tasksGrid_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                UIElement selectedElement = ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.ColumnHeader));

                if (contextObject is Infragistics.Win.UltraWinGrid.ColumnHeader) {
                    Infragistics.Win.UltraWinGrid.ColumnHeader columnHeader =
                        contextObject as Infragistics.Win.UltraWinGrid.ColumnHeader;
                    selectedColumn = columnHeader.Column;
                    ((StateButtonTool)toolbarsManager.Tools["groupByThisColumnButton"]).Checked = selectedColumn.IsGroupByColumn;
                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "columnContextMenu");
                } else {
                    contextObject = selectedElement.GetAncestor(typeof(RowUIElement));

                    if (contextObject is RowUIElement) {
                        RowUIElement row = contextObject as RowUIElement;
                        if (!row.Row.Selected) {
                            tasksGrid.Selected.Rows.Clear();
                            row.Row.Selected = true;
                        }
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "rowContextMenu");
                    } else {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
        }

        private void tasksGrid_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                UIElement selectedElement = tasksGrid.DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                if (selectedElement is CheckIndicatorUIElement) {
                    UltraGridColumn col = selectedElement.GetContext() as UltraGridColumn;
                    if (col != null && col.Key == _colCompletedCheck) {
                        UltraGridRow selectedRow = selectedElement.SelectableItem as UltraGridRow;
                        if (selectedRow != null) {
                            TaskStatus taskStatus = ((bool)selectedRow.Cells[_colCompletedCheck].Value) ? TaskStatus.NotStarted : TaskStatus.Completed;
                            IManagementService defaultManagementService =
                                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                            UpdateTaskStatus(defaultManagementService, selectedRow, taskStatus);
                        }
                    }
                }
            }
        }

        // Iterator for all the data rows in the grid, including children of groupby rows,
        private IEnumerable<UltraGridRow> DataRows() {
            foreach (UltraGridRow row in AllRowsIn(tasksGrid.Rows)) {
                if (row.IsDataRow) {
                    yield return row;
                }
            }
        }

        // Iterator for all rows, including child rows, in the specified collection.
        private IEnumerable<UltraGridRow> AllRowsIn(RowsCollection collection) {
            foreach (UltraGridRow row in collection) {
                yield return row;

                if (row.ChildBands != null) {
                    foreach (UltraGridChildBand band in row.ChildBands) {
                        foreach (UltraGridRow subrow in AllRowsIn(band.Rows)) {
                            yield return subrow;
                        }
                    }
                }
            }
        }

        // This class is used to group DateTime columns by date only (ignoring time).
        private class DateGroupByEvaluator : Infragistics.Win.UltraWinGrid.IGroupByEvaluator {
            public object GetGroupByValue(UltraGridGroupByRow groupbyRow, UltraGridRow row) {
                if (groupbyRow.Value == null || !(groupbyRow.Value is DateTime))
                    return null;
                else
                    return ((DateTime)groupbyRow.Value).Date;

            }

            public bool DoesGroupContainRow(UltraGridGroupByRow groupbyRow, UltraGridRow row) {
                // Compare only the Date parts of the DateTimes. 
                DateTime cellValue = (DateTime)row.Cells[groupbyRow.Column].Value;
                DateTime groupValue = (DateTime)groupbyRow.Value;
                return cellValue.Date == groupValue.Date;
            }
        }

        private static void NavigateToView(UltraGridRow row, bool showHistoricalSnapshot)
        {
            if (row != null)
            {
                string instanceName = row.Cells["ServerName"].Value as string;
                if (row.Cells["Metric"].Value != null)
                {
                    Metric metric = MetricDefinition.GetMetric((int)row.Cells["Metric"].Value);
                    if (showHistoricalSnapshot)
                    {
                        DateTime snapshotDateTime = (DateTime)row.Cells["CreatedOn"].Value;
                        ClickThroughHelper.NavigateToView(instanceName, metric, snapshotDateTime);
                    }
                    else
                    {
                        ClickThroughHelper.NavigateToView(instanceName, metric, null);
                    }
                }
            }
        }

        private void tasksGrid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.IsDataRow)
            {
                ShowTaskProperties();
            }
        }

        private void TasksView_Load(object sender, EventArgs e)
        {
            ApplySettings();
        }

        private void instanceCombo_BeforeDropDown(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Remember the selected item.
            string prevSelItem = (instanceCombo.SelectedItem != null ? instanceCombo.SelectedItem.DisplayText : "");

            // Add the active servers to the list of instances, and at same time figure
            // out which index in the list matches the original selection.
            int i = 0;
            int newSelIndex = 0; // init to index for All

            List<ValueListItem> instances = new List<ValueListItem>();
            instances.Add(new ValueListItem(null, "All"));
            _anyInstanceRow = instances[0];

            Wrapper<Tag> selectedObject = tagCombo.SelectedItem.ListObject as Wrapper<Tag>;

            if (selectedObject != null && selectedObject.GetValue() != null)
            {
                Tag selectedTag = selectedObject.GetValue();

                if (selectedTag != null && ApplicationModel.Default.Tags.Contains(selectedTag.Id))
                {
                    foreach (int instanceId in ApplicationModel.Default.Tags[selectedTag.Id].Instances)
                    {
                        if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
                        {
                            MonitoredSqlServer instance = ApplicationModel.Default.ActiveInstances[instanceId];
                            ++i;
                            instances.Add(new ValueListItem(instance.InstanceName, instance.InstanceName));

                            if (string.Compare(instance.InstanceName, prevSelItem, true) == 0)
                            {
                                newSelIndex = i;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (MonitoredSqlServer instance in ApplicationModel.Default.ActiveInstances)
                {
                    ++i;
                    instances.Add(new ValueListItem(instance.InstanceName, instance.InstanceName));

                    if (string.Compare(instance.InstanceName, prevSelItem, true) == 0)
                    {
                        newSelIndex = i;
                    }
                }
            }

            // Clear and add items, and set the selected item based on new sel index.
            instanceCombo.Items.Clear();
            instanceCombo.Items.AddRange(instances.ToArray());
            instanceCombo.SelectedIndex = newSelIndex;

            // Set the custom instance to the selected item.
            _customInstance = instances[newSelIndex];
        }

        private void tagCombo_BeforeDropDown(object sender, System.ComponentModel.CancelEventArgs e)
        {
            List<Wrapper<Tag>> list = new List<Wrapper<Tag>>();
            int newTagIndex = -1;
            string selTagText = tagCombo.SelectedItem.DisplayText;

            list.Add(new Wrapper<Tag>("All"));

            if (selTagText == "All")
                newTagIndex = 0;

            foreach (Tag tag in ApplicationModel.Default.Tags)
            {
                if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator || tag.Instances.Count > 0)
                    list.Add(new Wrapper<Tag>(tag, tag.Name));
            }

            list.Sort();

            BindingSource bindingList = (BindingSource)tagCombo.DataSource;
            bindingList.SuspendBinding();
            bindingList.Clear();
            foreach (Wrapper<Tag> item in list)
            {
                bindingList.Add(item);
                if (item.Label == selTagText)
                    newTagIndex = bindingList.Count - 1;
            }
            bindingList.ResumeBinding();
            tagCombo.SelectedIndex = newTagIndex != -1 ? newTagIndex : 0;
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
            UpdateFilter();
        }

        private void UpdateFilter()
        {
            if (Settings.Default.ColorScheme == "Dark")
            {
                toggleFilterOptionsPanelButton.Image = filterOptionsPanel.Visible ? Resources.UpArrowsDark : Resources.DownArrowsDark;
            }
            else
            {
                toggleFilterOptionsPanelButton.Image = filterOptionsPanel.Visible ? Resources.UpArrows : Resources.DownArrows;
            }
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.tasksGrid);
        }
    }

    internal enum StandardTasksViews
    {
        None,
        Active,
        Completed,
        ByOwner,
        ByStatus,
        Custom,
        ShowDefaultOrExisting
    }
}
