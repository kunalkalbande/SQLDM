using System.Xml;

namespace Idera.SQLdm.DesktopClient.Views.Alerts
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Diagnostics;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using Common;
    using Controls;
    using Dialogs;
    using Helpers;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Objects.ApplicationSecurity;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Infragistics.Win;
    using Infragistics.Win.UltraWinDataSource;
    using Infragistics.Win.UltraWinEditors;
    using Infragistics.Win.UltraWinGrid;
    using Infragistics.Win.UltraWinToolbars;
    using Objects;
    using Properties;
    using Wintellect.PowerCollections;
    using Resources = Idera.SQLdm.DesktopClient.Properties.Resources;
    using View = Idera.SQLdm.DesktopClient.Views.View;
    using Idera.SQLdm.DesktopClient.Views.Servers.Server;
    using Infragistics.Windows.Themes;

    internal partial class AlertsView : View
    {
        private const string UNABLE_TO_UPDATE = @"Unable to update data for this view.";

        private const char DELTA_CHAR = (char)0x394;
        private const int DefaultNumberOfDays = 3; // TODO: Need to get DefaultNumberOfDays from an app setting

        #region Fields

        private static string DELTA_STRING = DELTA_CHAR.ToString();
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("AlertsView");
        private static BBS.TracerX.Logger StartUpTimeLog = BBS.TracerX.Logger.GetLogger(TextConstants.StartUpTimeLogName);

        private AlertFilter customFilter;
        private AlertFilter appliedFilter; // values used in the currently applied filter
        private AlertFilter pendingFilter; // values from the filter controls - pending != applied when filter changed
        private bool suspendEventHandling;

        private bool initialized = false;
        private UltraGridColumn selectedColumn = null;
        private StandardAlertsViews currentView = StandardAlertsViews.Active;
        private object sync = new object();

        private bool filterInstanceNoPermission = false;

        //last Settings values used to determine if changed for saving when leaving
        private GridSettings lastMainGridSettings = null;
        private int lastSplitterDistance = 0;

        private bool showHistoricalView = false;

        private Control focused;

        ////[START] SQLdm 9.1 (Gaurav Karwal): for launching alert screen in the context of these values
        //public long AlertId { get; set; }
        //public string InstanceName { get; set; }
        //public int InstanceId { get; set; }
        ////[END] SQLdm 9.1 (Gaurav Karwal): for launching alert screen in the context of these values

        #endregion

        ThemeSetter themeSetter = new ThemeSetter();

        public Color ultragridBackColor = Settings.Default.ColorScheme == "Dark" ? System.Drawing.ColorTranslator.FromHtml("#012A4F") : System.Drawing.Color.White;
        public Color ultragridForeColor = Settings.Default.ColorScheme == "Dark" ? System.Drawing.Color.White : System.Drawing.Color.Black;

        public AlertsView()
        {
            InitializeComponent();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            alertsGrid.CreationFilter = new NoTooltip();
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by NoTooltip() : {0}", stopWatch.ElapsedMilliseconds);

            noSelectionLabel.BringToFront();

            stopWatch.Reset();
            stopWatch.Start();
            alertsGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by HideFocusRectangleDrawFilter() : {0}", stopWatch.ElapsedMilliseconds);

            appliedFilter = new AlertFilter();
            appliedFilter.ActiveOnly = true;
            pendingFilter = (AlertFilter)appliedFilter.Clone();
            customFilter = new AlertFilter();

            SetFilter(pendingFilter);

            AdaptFontSize();
            SetGridTheme();
            UpdateTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            //SetTheme();
            SetGridTheme();
            UpdateFilter();
            UpdateTheme();
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

        private void SetTheme()
        {
            ultragridBackColor = Settings.Default.ColorScheme == "Dark" ? System.Drawing.ColorTranslator.FromHtml("#012A4F") : System.Drawing.Color.White;
            ultragridForeColor = Settings.Default.ColorScheme == "Dark" ? System.Drawing.Color.White : System.Drawing.Color.Black;
            
            RowsCollection gridRows = this.alertsGrid.Rows;
            foreach (var item in gridRows)
            {
                item.Appearance.ForeColor = ultragridForeColor;
                item.Appearance.BackColor = ultragridBackColor;
            }

        }

        public void UpdateTheme()
        {
            if (Settings.Default.ColorScheme == "Dark")
            {
                Color forecolor = ColorTranslator.FromHtml(DarkThemeColorConstants.ForeColor);
                Color buttonBackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.EnabledActionButtonBackColor);
                this.applyButton.BackColor = buttonBackColor;
                this.applyButton.FlatAppearance.BorderColor = buttonBackColor;
                this.applyButton.FlatStyle = FlatStyle.Flat;
                this.applyButton.ForeColor = forecolor;
                this.applyButton.MouseEnter += new EventHandler(this.ok_button_MouseEnter);
                this.applyButton.MouseLeave += new EventHandler(this.ok_button_MouseLeave);

                this.clearButton.BackColor = buttonBackColor;
                this.clearButton.FlatAppearance.BorderColor = buttonBackColor;
                this.clearButton.FlatStyle = FlatStyle.Flat;
                this.clearButton.ForeColor = forecolor;
                this.clearButton.MouseEnter += new EventHandler(this.cancel_button_MouseEnter);
                this.clearButton.MouseLeave += new EventHandler(this.cancel_button_MouseLeave);

            }
            else
            {
                this.applyButton.BackColor = SystemColors.ButtonFace;
                this.applyButton.FlatStyle = FlatStyle.System;
                this.applyButton.TextAlign = ContentAlignment.MiddleCenter;

                this.clearButton.BackColor = SystemColors.ButtonFace;
                this.clearButton.FlatStyle = FlatStyle.System;
                this.clearButton.TextAlign = ContentAlignment.MiddleCenter;
            }
        }


        private void ok_button_MouseEnter(object sender, EventArgs e)
        {
            OnMouseHover(e);
            this.applyButton.BackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.HoveredActionButtonBackColor);
        }

        private void ok_button_MouseLeave(object sender, EventArgs e)
        {
            OnMouseHover(e);
            this.applyButton.BackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.EnabledActionButtonBackColor);
        }

        private void cancel_button_MouseEnter(object sender, EventArgs e)
        {
            OnMouseHover(e);
            this.clearButton.BackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.HoveredActionButtonBackColor);
        }

        private void cancel_button_MouseLeave(object sender, EventArgs e)
        {
            OnMouseHover(e);
            this.clearButton.BackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.EnabledActionButtonBackColor);
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.alertsGrid);
        }

        public void SetCurrentObjectTheme(System.Drawing.Color backColor)
        {
            this.BackColor = backColor;
        }

        ///// <summary>
        ///// SQLdm 9.1 (Gaurav Karwal): allows alerts to launch in context of the alertid and instanceName
        ///// </summary>
        ///// <param name="alertId"></param>
        ///// <param name="instanceName"></param>
        //public AlertsView(long alertId, string instanceName)
        //{
        //    InitializeComponent();

        //    alertsGrid.CreationFilter = new NoTooltip();

        //    noSelectionLabel.BringToFront();
        //    alertsGrid.DrawFilter = new HideFocusRectangleDrawFilter();

        //    appliedFilter = new AlertFilter();
        //    appliedFilter.ActiveOnly = true;

        //    pendingFilter = (AlertFilter)appliedFilter.Clone();
        //    customFilter = new AlertFilter();
        //    SetFilter(pendingFilter);
        //    AdaptFontSize();

        //    //[START] SQLdm 9.1 (Gaurav Karwal): for launching alert screen in the context of these values
        //    AlertId = alertId;
        //    InstanceName = instanceName;
        //    //[END] SQLdm 9.1 (Gaurav Karwal): for launching alert screen in the context of these values

        //}

        #region Refreshing

        public override object DoRefreshWork(BackgroundWorker backgroundWorker)
        {
            AlertFilter filter;
            lock (sync)
            {
                filter = appliedFilter;
            }

            object begin = (filter.BeginDate.HasValue) ? (object)filter.BeginDate.Value.ToUniversalTime() : null;
            object end = (filter.EndDate.HasValue) ? (object)filter.EndDate.Value.ToUniversalTime() : null;
            object metric = (filter.Metric.HasValue) ? (object)((int)filter.Metric.Value) : null;
            object severity = (filter.Severity.HasValue) ? (object)((byte)filter.Severity.Value) : null;
            object activeOnly = (filter.ActiveOnly.HasValue && filter.ActiveOnly.Value)? (object)true : null;

            // if no filter instance is specified filter contains all assigned servers.
            filterInstanceNoPermission = false; // clear the filter instance no permission flag
            XmlDocument instanceFilterXml = new XmlDocument();
            XmlElement rootElement = instanceFilterXml.CreateElement("Servers");
            instanceFilterXml.AppendChild(rootElement);

            if (filter.Instance != null)
            {
                XmlElement instanceElement = instanceFilterXml.CreateElement("Server");
                instanceElement.SetAttribute("InstanceName", filter.Instance);
                instanceFilterXml.FirstChild.AppendChild(instanceElement);
            }
            else if (filter.Tag != null && ApplicationModel.Default.Tags.Contains(filter.Tag.Id))
            {
                Tag tagFilter = ApplicationModel.Default.Tags[filter.Tag.Id];

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
            else
            {
                // adding a dummy signals the proc to include operational alerts
                XmlElement dummy = instanceFilterXml.CreateElement("Server");
                instanceFilterXml.FirstChild.AppendChild(dummy);

                foreach (MonitoredSqlServer instance in ApplicationModel.Default.ActiveInstances)
                {
                    XmlElement instanceElement = instanceFilterXml.CreateElement("Server");
                    instanceElement.SetAttribute("InstanceName", instance.InstanceName);
                    instanceFilterXml.FirstChild.AppendChild(instanceElement);
                }
            }

            using (LOG.DebugCall("DoRefreshWork"))
            {
                try
                {
                    // make sure the metric value list contains up-to-date metrics
                    ValueList valueList = alertsGrid.DisplayLayout.ValueLists["Metrics"];
                    if (valueList.ValueListItems.Count == 0)
                        valueList.ValueListItems.AddRange(ValueListHelpers.GetMetricValueListItems());
                    else
                    {
                        MetricDescription? metricDescription;
                        // grab changes to metric definitions
                        MetricDefinitions metrics = ApplicationModel.Default.MetricDefinitions;
                        metrics.Reload(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);
                        // only custom counters get added/removed while the product is running
                        foreach (int metricID in metrics.GetCounterDefinitionKeys())
                        {
                            ValueListItem listItem = valueList.FindByDataValue(metricID);
                            if (listItem == null)
                            {
                                metricDescription = metrics.GetMetricDescription(metricID);
                                if (metricDescription.HasValue)
                                {
                                    listItem = new ValueListItem(metricID, metricDescription.Value.Name);
                                    valueList.ValueListItems.Add(listItem);
                                }
                            }
                        }                               
                    }

                    if (!alertsViewDataSource.BindingNotificationsSuspended)
                        alertsViewDataSource.SuspendBindingNotifications();

                    RepositoryHelper
                        .LoadDataSource(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                        alertsViewDataSource,
                        "p_GetAlerts",
                        0, begin, end, instanceFilterXml.InnerXml, null, null, severity, metric, null, activeOnly, Settings.Default.AlertRowLimit);  //SQLDM 8.5 Mahesh: additional param CategoryId added for rest service
                }
                catch (Exception e)
                {
                    return e;
                }
                finally
                {
                    //                    try
                    //                    {
                    //                        if (backgroundWorker.CancellationPending)
                    //                            alertsViewDataSource.ResumeBindingNotifications();
                    //                    }
                    //                    catch (Exception e)
                    //                    {
                    //                        /* fails when the selected row in the grid is no longer in the data source */
                    //                    }
                }
                return alertsViewDataSource;
            }
        }

        public override void UpdateData(object data)
        {
            using (Log.VerboseCall())
            {
                if (data != null && data is Exception)
                {
                    Exception e = (Exception)data;
                    ApplicationController.Default.ClearCustomStatus();
                    ApplicationController.Default
                        .OnRefreshActiveViewCompleted(new RefreshActiveViewCompletedEventArgs(DateTime.Now, e));
                    alertsGridStatusLabel.Text = UNABLE_TO_UPDATE;
                    alertsGridStatusLabel.BringToFront();
                    return;
                }
                alertsGridStatusLabel.SendToBack();

                UltraGridHelper.GridState state = UltraGridHelper.GetGridState(alertsGrid, "AlertID");

                alertsGrid.SuspendLayout();
                alertsGrid.SuspendRowSynchronization();
                // resume binding - will cause group by rows to be recreated
                alertsViewDataSource.ResumeBindingNotifications();

                if (!initialized)
                {
                    if (lastMainGridSettings != null)
                    {
                        GridSettings.ApplySettingsToGrid(lastMainGridSettings, alertsGrid);
                        ConfigureView(currentView);

                        initialized = true;
                    }
                }

                UltraGridHelper.RestoreGridState(state);
                alertsGrid.ResumeRowSynchronization();
                alertsGrid.ResumeLayout();

                // Set filter label based on the view.
                string filterStatus = string.Empty;
                switch (currentView)
                {
                    case StandardAlertsViews.Active:
                        filterStatus = "Active Alerts";
                        break;
                    case StandardAlertsViews.Custom:
                        filterStatus = "Custom Filter";
                        break;
                    case StandardAlertsViews.BySeverity:
                        filterStatus = "Active Alerts By Severity";
                        break;
                    case StandardAlertsViews.ByInstance:
                        filterStatus = "Active Alerts By Server";
                        break;
                    case StandardAlertsViews.ByMetric:
                        filterStatus = "Active Alerts By Metric";
                        break;
                    case StandardAlertsViews.BombedJobs:
                        filterStatus = "Active SQL Agent Job Failure Alerts";
                        break;
                    case StandardAlertsViews.BlockedSessions:
                        filterStatus = "Active Blocked Session Alerts";
                        break;
                    case StandardAlertsViews.OldestOpen:
                        filterStatus = "Active Oldest Open Transaction Alerts";
                        break;
                    case StandardAlertsViews.QueryMonitor:
                        filterStatus = "Active Query Monitor Event Alerts";
                        break;
                    case StandardAlertsViews.Fragmentation:
                        filterStatus = "Active Table Fragmentation Alerts";
                        break;
                    case StandardAlertsViews.ShowDefaultOrExisting:
                    default:
                        break;
                }

                filterAppliedLabel.Text = filterStatus;
                ApplicationController.Default.SetCustomStatus(
                    string.Empty,
                    String.Format("{0} Alert{1}",
                                  ((UltraDataSource)alertsGrid.DataSource).Rows.Count,
                                  ((UltraDataSource)alertsGrid.DataSource).Rows.Count == 1 ? string.Empty : "s")
                    );

                ApplicationController.Default
                    .OnRefreshActiveViewCompleted(new RefreshActiveViewCompletedEventArgs(DateTime.Now));

                if (alertsGrid.Selected.Rows.Count == 0 || !alertsGrid.Selected.Rows[0].IsDataRow)
                {
                    noSelectionLabel.BringToFront();
                }

                // if filter has an alert id specified, select that row (if possible)
                if (appliedFilter.AlertID >= 0)
                {
                    // alertsGrid.Selected.Rows[0].Cells["Metric"].Value
                    for (int i = 0; i < alertsGrid.Rows.Count; i++)
                    {
                        if ((long)alertsGrid.Rows[i].Cells["AlertID"].Value == appliedFilter.AlertID)
                        {
                            //alertsGrid.Selected.Rows.Add(alertsGrid.Rows[i]);
                            alertsGrid.Selected.Rows.Clear();
                            alertsGrid.Rows[i].Activate();
                            alertsGrid.Rows[i].Selected = true;
                            alertsGrid.ActiveRowScrollRegion.ScrollRowIntoView(alertsGrid.Rows[i]);
                            appliedFilter.AlertID = -1; // selection is a one time event
                            break;
                        }
                    }
                }

                // If user does not have permission on the filter server, flag an error.
                if (filterInstanceNoPermission)
                {
                    string instance = null;
                    lock (sync)
                    {
                        instance = appliedFilter.Instance;
                        appliedFilter.Instance = null;
                    }
                    serverCombo.SelectedIndex = 0;
                    ApplicationMessageBox.ShowInfo(this,
                                                   string.Format(
                                                       "The Server filter has been changed from [{0}] to All because you do not have permission to monitor this server.",
                                                            instance));
                }
            }
        }

        #endregion Refreshing

        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AlertsView);
        }

        public StandardAlertsViews GetView()
        {
            return currentView;
        }

        public void SetView(StandardAlertsViews view)
        {
            // This function can be called when the outlook bar group is changed.
            // When the outlook bar group changes, it calls this function with the
            // input view set to ShowDefaultOrExisting.   If this is the case
            // set the view to the currentView.
            // The _selectedView is initialized to
            // None, so in this case set the view to the default value, i.e. Active.
            if (view == StandardAlertsViews.ShowDefaultOrExisting)
            {
                view = currentView;
            }

            if (currentView != view)
            {
                ConfigureView(view);
                UpdateControls();
                currentView = view;
            }
        }

        public override void SetArgument(object argument)
        {
            if (argument is AlertFilter)
            {
                if (currentView == StandardAlertsViews.Custom && !((AlertFilter)argument).ForRealz)
                {
                    appliedFilter = customFilter;
                }
                else
                {
                    appliedFilter = (AlertFilter)argument;
                }
                pendingFilter = (AlertFilter)appliedFilter.Clone();
                try
                {
                    suspendEventHandling = true;
                    SetFilter(appliedFilter);
                }
                finally
                {
                    suspendEventHandling = false;
                }
                UpdateControls();

                // force a refresh of the data                
                alertsViewDataSource.Clear();
                noSelectionLabel.BringToFront();
                RefreshView();
            }
        }

        public override void ApplySettings()
        {
            if (Settings.Default.AlertsViewMainGrid is GridSettings)
            {
                lastMainGridSettings = Settings.Default.AlertsViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, alertsGrid);
            }

            // Fixed panel is second panel, so restore size of second panel
            lastSplitterDistance = splitContainer.Height - Settings.Default.AlertsViewMainSplitter;
            if (lastSplitterDistance > 0)
            {
                splitContainer.SplitterDistance = lastSplitterDistance;
            }
            else
            {
                lastSplitterDistance = splitContainer.Height - splitContainer.SplitterDistance;
            }
        }

        public override void SaveSettings()
        {
            GridSettings gridSettings = GridSettings.GetSettings(alertsGrid);
            // save all settings only if anything has changed
            if (lastSplitterDistance != splitContainer.Height - splitContainer.SplitterDistance
                || !gridSettings.Equals(lastMainGridSettings))
            {
                // Fixed panel is second panel, so save size of second panel
                lastSplitterDistance =
                    Settings.Default.AlertsViewMainSplitter = splitContainer.Height - splitContainer.SplitterDistance;
                //[START] SQLdm 10.0 (Rajesh Gupta) : Small Feature-Console Configuration Retention --Fixed AlertsViewMainGrid
                lastMainGridSettings =                    
                    Settings.Default.AlertsViewMainGrid = gridSettings;
                //[END] SQLdm 10.0 (Rajesh Gupta) : Small Feature-Console Configuration Retention --Fixed AlertsViewMainGrid
            }
        }

        public AlertFilter GetPendingFilter()
        {
            return pendingFilter;
        }

        private void SetFilter(AlertFilter filter)
        {
               Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                if (filter.Tag == null)
                    tagCombo.SelectedIndex = 0;
                else
                    SelectComboItem(tagCombo, new Wrapper<Tag>(filter.Tag, filter.Tag.Name), true);

                if (String.IsNullOrEmpty(filter.Instance))
                    serverCombo.SelectedIndex = 0;
                else
                    SelectComboItem(serverCombo, new Wrapper<string>(filter.Instance, filter.Instance), true);

                if (filter.Severity == null)
                    severityCombo.SelectedIndex = 0;
                else
                    SelectComboItem(severityCombo,
                                    new Wrapper<MonitoredState>(filter.Severity.Value, filter.Severity.ToString()), false);
                if (filter.Metric == null)
                    metricCombo.SelectedIndex = 0;
                else
                {
                    ValueListItem item = metricCombo.Items.ValueList.FindByDataValue((int)filter.Metric.Value);
                    metricCombo.SelectedItem = item;
                }
                rdbtnActiveOnly.Checked = filter.ActiveOnly != null && filter.ActiveOnly.Value;
                rdbtnTimeSpan.Checked = !rdbtnActiveOnly.Checked;
                beginDateCombo.Enabled = beginTimeCombo.Enabled
                    = endDateCombo.Enabled = endTimeCombo.Enabled
                        = rdbtnTimeSpan.Checked;

                if (rdbtnTimeSpan.Checked)
                {
                    if (filter.EndDate != null && filter.BeginDate != null)
                    {
                        endDateCombo.Value = filter.EndDate.Value.Date;
                        beginDateCombo.Value = filter.BeginDate.Value.Date;
                        endTimeCombo.Time = filter.EndDate.Value.TimeOfDay;
                        beginTimeCombo.Time = filter.BeginDate.Value.TimeOfDay;
                    }
                    else
                    {
                        DateTime now = DateTime.Now;
                        beginDateCombo.Value = now.Subtract(TimeSpan.FromDays(DefaultNumberOfDays));
                        endDateCombo.Value = now;

                        // Default time range is 7:00 AM - 5:00 PM
                        beginTimeCombo.Time = TimeSpan.FromHours(0);
                        endTimeCombo.Time = TimeSpan.FromHours(23) + TimeSpan.FromMinutes(59);//TimeSpan.FromHours(23);
                    }
                }
                stopWatch.Stop();
                StartUpTimeLog.DebugFormat("Time taken by AlertView.SetFilter : {0}",stopWatch.ElapsedMilliseconds);
        }

        private void SelectComboItem(UltraComboEditor combo, object value, bool canAdd)
        {
            System.Collections.IList list = combo.DataSource as System.Collections.IList;
            if (!list.Contains(value))
                list.Add(value);

            ValueListItem item = null;
            foreach (ValueListItem vli in combo.Items)
            {
                if (vli.ListObject.Equals(value))
                {
                    item = vli;
                    break;
                }
            }
            if (item != null)
                combo.SelectedItem = item;
            else
                combo.SelectedIndex = 0;
        }

        public void ConfigureView(StandardAlertsViews view)
        {
            UltraGridBand band = alertsGrid.DisplayLayout.Bands[0];

            // Clear all the sorted columns.
            band.SortedColumns.Clear();

            // setup the columns, group by and initial sort for the grid
            switch (view)
            {
                case StandardAlertsViews.Custom:
                    band.SortedColumns.Add(band.Columns["UTCOccurrenceDateTime"], true, false);
                    band.SortedColumns.Add(band.Columns["ServerName"], false, false);
                    band.SortedColumns.Add(band.Columns["Severity"], true, false);
                    break;
                case StandardAlertsViews.ByInstance:
                    band.SortedColumns.Add(band.Columns["ServerName"], false, true);
                    band.SortedColumns.Add(band.Columns["UTCOccurrenceDateTime"], true, false);
                    band.SortedColumns.Add(band.Columns["Severity"], true, false);
                    break;
                case StandardAlertsViews.ByMetric:
                    band.SortedColumns.Add(band.Columns["Metric"], false, true);
                    band.SortedColumns.Add(band.Columns["UTCOccurrenceDateTime"], true, false);
                    band.SortedColumns.Add(band.Columns["ServerName"], false, false);
                    band.SortedColumns.Add(band.Columns["Severity"], true, false);
                    break;
                case StandardAlertsViews.BySeverity:
                    band.SortedColumns.Add(band.Columns["Severity"], true, true);
                    band.SortedColumns.Add(band.Columns["UTCOccurrenceDateTime"], true, false);
                    band.SortedColumns.Add(band.Columns["ServerName"], false, false);
                    break;
                case StandardAlertsViews.Active:
                    band.SortedColumns.Add(band.Columns["UTCOccurrenceDateTime"], true, false);
                    band.SortedColumns.Add(band.Columns["ServerName"], false, false);
                    band.SortedColumns.Add(band.Columns["Severity"], true, false);
                    break;
                case StandardAlertsViews.BlockedSessions:
                    band.SortedColumns.Add(band.Columns["UTCOccurrenceDateTime"], true, false);
                    band.SortedColumns.Add(band.Columns["ServerName"], false, false);
                    band.SortedColumns.Add(band.Columns["Severity"], true, false);
                    break;
                case StandardAlertsViews.BombedJobs:
                    band.SortedColumns.Add(band.Columns["UTCOccurrenceDateTime"], true, false);
                    band.SortedColumns.Add(band.Columns["ServerName"], false, false);
                    band.SortedColumns.Add(band.Columns["Severity"], true, false);
                    break;
                case StandardAlertsViews.OldestOpen:
                    band.SortedColumns.Add(band.Columns["UTCOccurrenceDateTime"], true, false);
                    band.SortedColumns.Add(band.Columns["ServerName"], false, false);
                    band.SortedColumns.Add(band.Columns["Severity"], true, false);
                    break;
                case StandardAlertsViews.Fragmentation:
                    band.SortedColumns.Add(band.Columns["UTCOccurrenceDateTime"], true, false);
                    band.SortedColumns.Add(band.Columns["ServerName"], false, false);
                    band.SortedColumns.Add(band.Columns["Severity"], true, false);
                    break;
                case StandardAlertsViews.QueryMonitor:
                    band.SortedColumns.Add(band.Columns["UTCOccurrenceDateTime"], true, false);
                    band.SortedColumns.Add(band.Columns["ServerName"], false, false);
                    band.SortedColumns.Add(band.Columns["Severity"], true, false);
                    break;
                default:
                    band.SortedColumns.Add(band.Columns["UTCOccurrenceDateTime"], true, false);
                    band.SortedColumns.Add(band.Columns["ServerName"], false, false);
                    band.SortedColumns.Add(band.Columns["Severity"], true, false);
                    break;
            }

        }

        public void ShowFilterOptions()
        {
            Settings.Default.AlertsViewFilterOptionsPanelVisible = true;
        }

        private void alertsGrid_MouseDown(object sender, MouseEventArgs e)
        {
            bool enableClearOptions = false;

            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    UIElement selectedElement =
                        ((UltraGrid) sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));

                    object contextObject = null;
                    if (selectedElement != null)
                        contextObject = selectedElement.GetContext(typeof (Infragistics.Win.UltraWinGrid.ColumnHeader));

                    if (contextObject is Infragistics.Win.UltraWinGrid.ColumnHeader)
                    {
                        Infragistics.Win.UltraWinGrid.ColumnHeader columnHeader =
                            contextObject as Infragistics.Win.UltraWinGrid.ColumnHeader;
                        selectedColumn = columnHeader.Column;
                        ((StateButtonTool) toolbarsManager.Tools["groupByThisColumnButton"]).Checked =
                            selectedColumn.IsGroupByColumn;
                        toolbarsManager.SetContextMenuUltra(((UltraGrid) sender), "columnContextMenu");
                        ConfigureContextMenuItems();
                    }
                    else
                    {
                        toolbarsManager.Tools["showDetailsButton"].SharedProps.Visible =
                            !Settings.Default.AlertsViewDetailsPanelExpanded;
                        toolbarsManager.SetContextMenuUltra(((UltraGrid) sender), "gridContextMenu");
                        if (selectedElement != null && selectedElement.SelectableItem is UltraGridRow)
                        {
                            ButtonTool tool = (ButtonTool) toolbarsManager.Tools["editAlertConfigurationButton"];
                            UltraGridRow row = ((UltraGridRow) selectedElement.SelectableItem);
                            if (row.IsDataRow)
                            {
                                alertsGrid.Selected.Rows.Clear();
                                row.Activate();
                                row.Selected = true;
                                // set the alert configuration edit button enabled if the selected row has a server name
                                UltraDataRow dataRow = (UltraDataRow) row.ListObject;
                                object server = dataRow["ServerName"];
                                toolbarsManager.Tools["editAlertConfigurationButton"]
                                    .SharedProps.Visible = (server is string && !String.IsNullOrEmpty((string) server));

                                bool enableSnoozeButton = false;

                                MonitoredSqlServer monitoredServer = null;
                                int instanceId = -1;
                                if (!String.IsNullOrEmpty(server as string))
                                    monitoredServer = ApplicationModel.Default.ActiveInstances[server as string];
                                if (monitoredServer != null)
                                    instanceId = monitoredServer.Id;

                                object metric = dataRow["Metric"];
                                object active = dataRow["Active"];
                                if (active is bool && ((bool) active))
                                {
                                    if (metric is int)
                                    {
                                        Metric m = (Metric) Enum.ToObject(typeof (Metric), (int) metric);
                                        switch (m)
                                        {
                                            case Metric.ReorganisationPct:
                                            case Metric.BombedJobs:
                                            case Metric.LongJobs:
                                            case Metric.LongJobsMinutes:
                                            case Metric.ClusterFailover:
                                            case Metric.JobCompletion:
                                            case Metric.AlwaysOnAvailabilityGroupRoleChange://SQLDM-30487
                                                enableClearOptions =
                                                    ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >=
                                                    PermissionType.Modify;
                                                break;
                                        }
                                    }
                                    if (alertsGrid.Selected.Rows.Count == 1)
                                    {
                                        if (server != null && !"".Equals(server.ToString()))
                                        {
                                            //Operator Security Role Changes - 10.3 --Altered the condition for ReadOnlyPlus
                                            if (metric is int &&
                                                !((int) metric == (int) Metric.MaintenanceMode ||
                                                  (int) metric == (int) Metric.Operational))
                                                enableSnoozeButton =
                                                    ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >=
                                                    PermissionType.Modify || ApplicationModel.Default.UserToken.GetServerPermission(instanceId) ==
                                                    PermissionType.ReadOnlyPlus;
                                        }
                                    }
                                }
                                tool = (ButtonTool) toolbarsManager.Tools["snoozeAlertButton"];
                                tool.SharedProps.Visible = enableSnoozeButton;
                            }
                            else
                            {
                                toolbarsManager.Tools["editAlertConfigurationButton"].SharedProps.Visible = false;
                            }
                            toolbarsManager.Tools["copyToClipboardButton"].SharedProps.Visible = true;
                        }
                        else
                        {
                            toolbarsManager.Tools["editAlertConfigurationButton"].SharedProps.Visible = false;
                            toolbarsManager.Tools["copyToClipboardButton"].SharedProps.Visible = false;
                        }
                    }
                    toolbarsManager.Tools["clearAlertButton"].SharedProps.Visible = enableClearOptions;
                    toolbarsManager.Tools["clearAllAlertsButton"].SharedProps.Visible = enableClearOptions;
                }
                catch (Exception err)
                {
                    LOG.Error("Error handling right mouse click: ", err);
                }
            }
        }

        private void ConfigureContextMenuItems()
        {
            ButtonTool tool = toolbarsManager.Tools["toggleColumnTextButton"] as ButtonTool;
            if (tool == null)
                return;

            bool showingText =
                (selectedColumn.ValueList != null && selectedColumn.ValueList.ShouldDisplayText);

            switch (selectedColumn.Key)
            {
                case "Severity":
                    tool.SharedProps.Visible = true;
                    tool.SharedProps.Caption = showingText ? "Hide Severity Description" : "Show Severity Description";
                    break;
                case "StateEvent":
                    tool.SharedProps.Visible = true;
                    tool.SharedProps.Caption = showingText ? "Hide Change Description" : "Show Change Description";
                    break;
                default:
                    tool.SharedProps.Visible = false;
                    break;
            }
        }

        private void UpdateControls()
        {
            applyButton.Enabled = true; // appliedFilter != pendingFilter;
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (ValidateFilter())
            {
                customFilter = (AlertFilter)pendingFilter.Clone();
                ApplicationController.Default.ShowAlertsView(StandardAlertsViews.Custom, pendingFilter);
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            SetFilter(new AlertFilter());
        }

        private void serverCombo_BeforeDropDown(object sender, CancelEventArgs e)
        {
            Wrapper<Tag> selectedObject = tagCombo.SelectedItem.ListObject as Wrapper<Tag>;

            List<Wrapper<string>> list = new List<Wrapper<string>>();

            Tag selectedTag = (selectedObject != null) ? selectedObject.GetValue() : null;

            // Remember the selected instance, and setup for 
            int newSelSrvrIndex = -1;
            string selSrvrText = serverCombo.SelectedItem.DisplayText;

            list.Clear();
            list.Add(new Wrapper<string>("All"));
            // If name matches the original selection rememer the index.
            if (selSrvrText == "All")
                newSelSrvrIndex = 0;

            MonitoredSqlServerCollection instances = ApplicationModel.Default.ActiveInstances;

            if (selectedTag != null && ApplicationModel.Default.Tags.Contains(selectedTag.Id))
            {
                foreach (int instanceId in ApplicationModel.Default.Tags[selectedTag.Id].Instances)
                {
                    MonitoredSqlServer instance = instances[instanceId];
                    list.Add(new Wrapper<string>(instance.InstanceName, instance.InstanceName));
                }
            }
            else
            {
                foreach (MonitoredSqlServerWrapper instance in instances)
                {
                    list.Add(new Wrapper<string>(instance.InstanceName, instance.InstanceName));
                }
            }

            list.Sort();

            BindingSource bindingList = (BindingSource) serverCombo.DataSource;
            bindingList.SuspendBinding();
            bindingList.Clear();
            foreach (Wrapper<string> item in list)
            {
                bindingList.Add(item);
                if (item.Label == selSrvrText)
                {
                    newSelSrvrIndex = bindingList.Count - 1;
                }
            }
            bindingList.ResumeBinding();

            // Set the selected index, if selected server does not match then set it to all.
            serverCombo.SelectedIndex = newSelSrvrIndex != -1 ? newSelSrvrIndex : 0;
        }

        private void FilterValueChanged(object sender, EventArgs e)
        {
            if (suspendEventHandling)
                return;

            if (sender == tagCombo)
            {
                Wrapper<Tag> value = tagCombo.SelectedItem.ListObject as Wrapper<Tag>;
                pendingFilter.Tag = (value == null || value.IsAllItem()) ? null : value.GetValue();
                serverCombo.SelectedIndex = 0;
            }
            else if (sender == serverCombo)
            {
                if (serverCombo.SelectedItem != null)
                {
                    Wrapper<string> value = serverCombo.SelectedItem.ListObject as Wrapper<string>;
                    pendingFilter.Instance = (value == null || value.IsAllItem()) ? null : value.GetValue();
                }
            }
            else if (sender == metricCombo)
            {
                int value = (int)metricCombo.SelectedItem.DataValue;
                if (value == -1)
                    pendingFilter.Metric = null;
                else
                    pendingFilter.Metric = (Metric)Enum.ToObject(typeof(Metric), value);
            }
            else if (sender == severityCombo)
            {
                Wrapper<MonitoredState> value = severityCombo.SelectedItem.ListObject as Wrapper<MonitoredState>;
                if (value == null || value.IsAllItem())
                    pendingFilter.Severity = null;
                else
                    pendingFilter.Severity = value.GetValue();
            }
            else if (sender == rdbtnActiveOnly)
            {
                beginDateCombo.Enabled = beginTimeCombo.Enabled
                    = endDateCombo.Enabled = endTimeCombo.Enabled
                        = !rdbtnActiveOnly.Checked;
                if (rdbtnActiveOnly.Checked)
                {
                    pendingFilter.ActiveOnly = true;
                    pendingFilter.EndDate = pendingFilter.BeginDate = null;
                }
                else
                {
                    pendingFilter.ActiveOnly = null;
                    pendingFilter.BeginDate = GetBeginDateTimeValue();
                    pendingFilter.EndDate = GetEndDateTimeValue();
                }
            }
            else if (sender == beginDateCombo || sender == beginTimeCombo || sender == endDateCombo || sender == endTimeCombo)
            {
                pendingFilter.BeginDate = GetBeginDateTimeValue();
                pendingFilter.EndDate = GetEndDateTimeValue();
            }
            else
                return;

            UpdateControls();
        }

        private void alertsGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {            
            using (Log.VerboseCall())
            {
                if (typeof(Infragistics.Win.UltraWinGrid.UltraGridRow).IsAssignableFrom(e.Type))
                {
                    Log.Verbose("UltraGridRow is assignable from type ", e.Type);
                    Log.Verbose("alertsGrid.Selected.Rows.Count = ", alertsGrid.Selected.Rows.Count);
                    if (alertsGrid.Selected.Rows.Count == 0 || !alertsGrid.Selected.Rows[0].IsDataRow)
                    {
                        Log.Verbose("Showing noSelectionLable");
                        noSelectionLabel.BringToFront();
                    }
                    else
                    {
                        Log.Verbose("Showing detailsContentPanel, alert id = ", alertsGrid.Selected.Rows[0].Cells["AlertID"].Value);                        

                        if (alertsGrid.Selected.Rows[0].Cells["Metric"].Value != null && alertsGrid.Selected.Rows[0].Cells["Metric"].Value != DBNull.Value)
                        {
                            Metric m = MetricDefinition.GetMetric((int)alertsGrid.Selected.Rows[0].Cells["Metric"].Value);
                            
                            showRealTimeViewLinkLabel.Visible = m != Metric.Operational &&
                                                                m != Metric.SQLdmCollectionServiceStatus;

                            showHistoricalView = ClickThroughHelper.ViewSupportsHistoricalSnapshots(m);

                            showDetailsLinkLabel.Visible = (m == Metric.BlockingAlert);

                            //If we are showing the linklabel allowing drill-in to Block report
                            //move the help link to the bottom for consistency
                            if (showDetailsLinkLabel.Visible)
                            {
                                detailsContentPanel.SetCellPosition(showDetailsLinkLabel, new TableLayoutPanelCellPosition(6, 2));
                                detailsContentPanel.SetCellPosition(helpHistoryLinkLabel2, new TableLayoutPanelCellPosition(6, 3));
                            }
                            else
                            {
                                detailsContentPanel.SetCellPosition(helpHistoryLinkLabel2, new TableLayoutPanelCellPosition(6, 2));
                            }

                            if (showRealTimeViewLinkLabel.Visible)
                            {
                                switch(m)
                                {
                                    case Metric.Deadlock:
                                        showRealTimeViewLinkLabel.Text = "Show Deadlock View";
                                        break;
                                    default:
                                        showRealTimeViewLinkLabel.Text = "Show Real Time View";
                                        break;
                                }

                                if (showHistoricalView)
                                {
                                    helpHistoryLinkLabel1.Visible = true;
                                    helpHistoryLinkLabel2.Visible = m != Metric.Custom;
                                    helpHistoryLinkLabel1.Text = "Show Historical View";
                                    helpHistoryLinkLabel2.Text = "Show Alert Help";
                                }
                                else
                                {
                                    helpHistoryLinkLabel1.Visible = m != Metric.Custom;
                                    helpHistoryLinkLabel2.Visible = false;
                                    helpHistoryLinkLabel1.Text = "Show Alert Help";
                                }
                            }
                            else
                            {
                                if (m == Metric.SQLdmCollectionServiceStatus)
                                {
                                    helpHistoryLinkLabel1.Text = "Show Alert Help";
                                    helpHistoryLinkLabel1.Visible = true;
                                    helpHistoryLinkLabel2.Visible = false;
                                }
                                else
                                {
                                    helpHistoryLinkLabel1.Visible = false;
                                    helpHistoryLinkLabel2.Visible = false;
                                }
                            }
                        }
                        else
                        {
                            showRealTimeViewLinkLabel.Visible = false;
                            helpHistoryLinkLabel1.Visible = false;
                            helpHistoryLinkLabel2.Visible = false;
                            showDetailsLinkLabel.Visible = false;
                        }

                        detailsContentPanel.BringToFront();
                    }
                }
                else
                {
                    Log.Verbose("UltraGridRow is NOT assignable from type ", e.Type);
                }
            }
        }

        private void alertsGrid_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (e.Row.IsDataRow)
            {
                NavigateToView(e.Row, false);
            }
        }
        
        private void NavigateToView(UltraGridRow row, bool showHistoricalSnapshot)
        {
            if (row != null)
            {
                string instanceName = row.Cells["ServerName"].Value as string;
                object argument = null;

                if (row.Cells["Metric"].Value != null)
                {

                    Metric metric = MetricDefinition.GetMetric((int)row.Cells["Metric"].Value);
                    //SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --fetch filegroup from filegroup alert message
                    string filegroup = GetFilegroupFromAlertMessage((string)row.Cells["Message"].Value);

                    switch (metric)
                    {
                        case Metric.ReorganisationPct:
                            argument = row.Cells["DatabaseName"].Value as object;
                            break;
                        case Metric.PageLifeExpectancy:
                            argument = Metric.PageLifeExpectancy;
                            break;
                        // START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --send arguments for highlighting on the view
                        case Metric.FilegroupSpaceFullSize:
                        case Metric.FilegroupSpaceFullPct:
                            argument = (object)(new object[] { row.Cells["DatabaseName"].Value, row.Cells["Metric"].Value, filegroup });
                            break;
                        // END : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --send arguments for highlighting on the view
                        default:
                            argument = null;
                            break;
                    }


                    if (showHistoricalSnapshot)
                    {
                        DateTime snapshotDateTime = (DateTime)row.Cells["UTCOccurrenceDateTime"].Value;
                        ClickThroughHelper.NavigateToView(instanceName, metric, snapshotDateTime, argument);
						//SQLDM-31039.
                        ApplicationController.Default.IsFromHistorySnapshot = true;
                    }
                    else
                    {
                        switch(metric)
                        {
                            case Metric.Deadlock:
                                ShowDeadlockDetails();
                                break;
                            default:
                                ClickThroughHelper.NavigateToView(instanceName, metric, null, argument );
                                break;
                        }
                    }
                }
            }
        }

        //START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --fetch filegroup name from filegroup alert message
        private string GetFilegroupFromAlertMessage(string message)
        {
            string filegroup = null;
            string referenceString = "has a file group ";

            if (message.Length > 0)
            {
                int startIdx = message.IndexOf(referenceString) + referenceString.Length + 1;
                int lastIdx = message.LastIndexOf(']');
                if (startIdx < lastIdx)
                    filegroup = message.Substring(startIdx, lastIdx - startIdx);
            }

            return filegroup;
        }
        //START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --fetch filegroup name from filegroup alert message

        private void alertsGrid_AfterSortChange(object sender, BandEventArgs e)
        {
            UltraGridBand band = e.Band;
        }

        private void alertsGrid_InitializeGroupByRow(object sender, InitializeGroupByRowEventArgs e)
        {
            string key = e.Row.Column.Key;
            if (key == "Severity") // || key == "StateEvent")
            {
                ValueList valueList = e.Row.Column.ValueList as ValueList;
                if (valueList != null)
                {
                    ValueListItem item = valueList.FindByDataValue(e.Row.Value);
                    if (item != null)
                    {
                        e.Row.Description = String.Format("{0} ({1} item{2})",
                                                          item.DisplayText,
                                                          e.Row.Rows.Count,
                                                          e.Row.Rows.Count == 1 ? String.Empty : "s");
                    }
                }
            }
        }

        private void alertsGrid_SummaryValueChanged(object sender, SummaryValueChangedEventArgs e)
        {
            Debug.Print("Summary value changed: " + e.SummaryValue.SummaryText);
        }

        #region Filter DateTime Event Handling
        
        private bool ValidateFilter()
        {
            return rdbtnActiveOnly.Checked || ValidateDateRange();
        }

        private bool ValidateDateRange()
        {
            DateTime beginDateTime = GetBeginDateTimeValue();
            DateTime endDateTime = GetEndDateTimeValue();

            if (beginDateTime >= endDateTime)
            {
                string errorMessage = "You cannot apply a filter based on a time that ends before it starts. " +
                                      "Please verify the time entries in the From and To fields.";
                if (beginDateTime.Date > endDateTime.Date)
                {
                    errorMessage = "You cannot apply a filter based on a date that ends before it starts. " +
                                   "Please verify the date entries in the From and To fields.";
                }
                else if (beginDateTime == endDateTime)
                {
                    errorMessage = "You cannot apply a filter with the same date and time values in " + 
                                   "the From and To fields. Please make sure to establish a specific time range.";
                }

                ApplicationMessageBox.ShowWarning(this, errorMessage);
                return false;
            }

            return true;
        }

        private DateTime GetBeginDateTimeValue()
        {
            DateTime value = (DateTime)beginDateCombo.Value;
            return (value.Date + beginTimeCombo.Time);
        }

        private DateTime GetEndDateTimeValue()
        {
            DateTime value = (DateTime)endDateCombo.Value;
            return (value.Date + endTimeCombo.Time);
        }

        #endregion

        #region OnLoad

        private void AlertsView_Load(object sender, EventArgs e)
        {
            suspendEventHandling = true;
            try
            {
                DetailsPanelVisible = Settings.Default.AlertsViewDetailsPanelExpanded;
                ConfigureFilterOptionsPanelVisible(Settings.Default.AlertsViewFilterOptionsPanelVisible);
                ConfigureAlertsGrid();
                ConfigureFilterBar();
            }
            finally
            {
                suspendEventHandling = false;
            }

            Settings.Default.SettingChanging += new SettingChangingEventHandler(Settings_SettingChanging);
            ApplySettings();
        }

        private void Settings_SettingChanging(object sender, SettingChangingEventArgs e)
        {
            switch (e.SettingName)
            {
                case "AlertsViewFilterOptionsPanelVisible":
                    ConfigureFilterOptionsPanelVisible((bool)e.NewValue);
                    break;
                case "AlertsViewDetailsPanelExpanded":
                    DetailsPanelVisible = (bool)e.NewValue;
                    break;
            }
        }

        private void ConfigureFilterBar()
        {
            DateTime now = DateTime.Now;
            beginDateCombo.Value = now.Subtract(TimeSpan.FromDays(DefaultNumberOfDays));
            endDateCombo.Value = now;

            // Default time range is 7:00 AM - 5:00 PM
            beginTimeCombo.Time = TimeSpan.FromHours(0);
            endTimeCombo.Time = TimeSpan.FromHours(23) + TimeSpan.FromMinutes(59);//TimeSpan.FromHours(23);

            InitializeTagCombo();
            InitializeMetricCombo();
            InitializeSeverityCombo();
            InitializeInstancesCombo();
        }

        private void InitializeMetricCombo()
        {
            if (metricCombo.Items.Count != 0)
                metricCombo.Items.Clear();

            metricCombo.Items.Add(-1, "All");
            metricCombo.Items.AddRange(ValueListHelpers.GetMetricValueListItems());
            metricCombo.SortStyle = ValueListSortStyle.None;
            metricCombo.SelectedIndex = 0;
        }

        private void InitializeSeverityCombo()
        {
            List<Wrapper<MonitoredState>> list = new List<Wrapper<MonitoredState>>();
            list.Add(new Wrapper<MonitoredState>("All"));
            list.Add(new Wrapper<MonitoredState>(MonitoredState.Critical, "Critical"));
            list.Add(new Wrapper<MonitoredState>(MonitoredState.Warning, "Warning"));
            list.Add(new Wrapper<MonitoredState>(MonitoredState.Informational, "Info"));
            list.Add(new Wrapper<MonitoredState>(MonitoredState.OK, "OK"));

            severityCombo.DataSource = list;
            severityCombo.SelectedIndex = 0;
        }

        public void InitializeInstancesCombo()
        {
            // just add the all item - build the full list during OnBeforeDropDown
            BindingSource list = new BindingSource(this.components);
            list.Add(new Wrapper<string>("All"));

            serverCombo.DataSource = list;
            serverCombo.SelectedIndex = 0;
        }

        public void InitializeTagCombo()
        {
            // just add the all item - build the full list during OnBeforeDropDown
            BindingSource list = new BindingSource(components);
            list.Add(new Wrapper<string>("All"));

            tagCombo.DataSource = list;
            tagCombo.SelectedIndex = 0;
        }

        private void ConfigureAlertsGrid()
        {
            alertsGrid.DisplayLayout.LoadStyle = LoadStyle.LoadOnDemand;

            // switch out editor from EditorWithCombo to EditorWithText
            EditorWithText textEditor = new EditorWithText();
            alertsGrid.DisplayLayout.Bands[0].Columns["Severity"].Editor = textEditor;
            alertsGrid.DisplayLayout.Bands[0].Columns["StateEvent"].Editor = textEditor;

            // populate the metric value list
            ValueList valueList = alertsGrid.DisplayLayout.ValueLists["Metrics"];
            valueList.ValueListItems.Clear();

            // configure the bindings for the details pane metric label
            ValueListBinding metricBindingText =
                new ValueListBinding("Text", alertsGrid.DataSource, alertsGrid.DataMember + ".Metric");
            metricBindingText.ValueList = valueList;
            metricLabel.DataBindings.Add(metricBindingText);
            //configure the bindings for the details pane Server Type
            ValueListBinding servertypeBindingText = new ValueListBinding("Text", alertsGrid.DataSource, alertsGrid.DataMember + ".ServerType");
            servertypeBindingText.ValueList = valueList;
            ServerTypeText.DataBindings.Add(servertypeBindingText);

            // build the severity value list
            valueList = alertsGrid.DisplayLayout.ValueLists["Severity"];
            valueList.ValueListItems.Clear();
            valueList.ValueListItems.AddRange(ValueListHelpers.GetSeverityValueListItems());

            // configure the bindings for the details pane severity image and label
            ValueListBinding severityBindingImage =
                new ValueListBinding("Image", alertsGrid.DataSource, alertsGrid.DataMember + ".Severity");
            ValueListBinding severityBindingText =
                new ValueListBinding("Text", alertsGrid.DataSource, alertsGrid.DataMember + ".Severity");
            severityBindingImage.ValueList = valueList;
            severityBindingText.ValueList = valueList;
            severityImage.DataBindings.Add(severityBindingImage);
            severityLabel.DataBindings.Add(severityBindingText);

            // load the transition value list
            valueList = alertsGrid.DisplayLayout.ValueLists["Transitions"];
            valueList.ValueListItems.Clear();
            valueList.ValueListItems.AddRange(ValueListHelpers.GetTransitionValueListItems());
            alertsGrid.DisplayLayout.Bands[0].Columns["StateEvent"].GroupByEvaluator =
                new AlertFilter.TransitionGroupByEvaluator(valueList);

            // configure the bindings for the details pane transition image and label
            ValueListBinding transitionBindingImage =
                new ValueListBinding("Image", alertsGrid.DataSource, alertsGrid.DataMember + ".StateEvent");
            ValueListBinding transitionBindingText =
                new ValueListBinding("Text", alertsGrid.DataSource, alertsGrid.DataMember + ".StateEvent");
            transitionBindingImage.ValueList = valueList;
            transitionBindingText.ValueList = valueList;
            transitionImage.DataBindings.Add(transitionBindingImage);
            transitionLabel.DataBindings.Add(transitionBindingText);
        }

        #endregion

        #region Details Panel Event Handling

        private void toggleDetailsPanelButton_Click(object sender, EventArgs e)
        {
            Settings.Default.AlertsViewDetailsPanelExpanded = !Settings.Default.AlertsViewDetailsPanelExpanded;
        }

        /// <summary>
        /// Get or Set the Details Panel visibility and trigger state update event if changed
        /// </summary>
        public bool DetailsPanelVisible
        {
            get { return !splitContainer.Panel2Collapsed; }
            set { splitContainer.Panel2Collapsed = !value; }
        }

        #endregion

        #region Filter Options Panel Event Handling

        private void toggleFilterOptionsPanelButton_Click(object sender, EventArgs e)
        {
            ToggleFilterOptionsPanelVisible();
        }

        private void ToggleFilterOptionsPanelVisible()
        {
            Settings.Default.AlertsViewFilterOptionsPanelVisible = !Settings.Default.AlertsViewFilterOptionsPanelVisible;
        }

        private void ConfigureFilterOptionsPanelVisible(bool expanded)
        {
            filterOptionsPanel.Visible = expanded;
            if (Settings.Default.ColorScheme == "Dark")
            {
                toggleFilterOptionsPanelButton.Image = expanded ? Resources.UpArrowsDark : Resources.DownArrowsDark;
            }
            else
            {
                toggleFilterOptionsPanelButton.Image = expanded ? Resources.UpArrows : Resources.DownArrows;
            }
        }

        #endregion

        #region Toolbar Event Handling

        private void toolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
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
                case "toggleColumnTextButton":
                    ToggleColumnText();
                    break;
                case "printGridButton":
                    PrintGrid();
                    break;
                case "exportGridButton":
                    SaveGrid();
                    break;
                case "editAlertConfigurationButton":
                    EditAlertConfiguration();
                    break;
                case "showDetailsButton":
                    Settings.Default.AlertsViewDetailsPanelExpanded = !Settings.Default.AlertsViewDetailsPanelExpanded;
                    break;
                case "collapseAllGroupsButton":
                    CollapseAllGroups();
                    break;
                case "expandAllGroupsButton":
                    ExpandAllGroups();
                    break;
                case "copyToClipboardButton":
                    UltraGridHelper.CopyToClipboard(alertsGrid, UltraGridHelper.CopyOptions.AllSelectedRows, UltraGridHelper.CopyFormat.AllFormats);
                    break;
                case "clearAlertButton":
                    ClearAlert(false);
                    break;
                case "clearAllAlertsButton":
                    ClearAlert(true);
                    break;
                case "snoozeAlertButton":
                    SnoozeAlert();
                    break;
                case "viewRealTimeSnapshotButton":
                    NavigateToView(alertsGrid.Selected.Rows[0], false);
                    break;
                case "viewHistoricalSnapshotButton":
                    NavigateToView(alertsGrid.Selected.Rows[0], true);
                    break;
                case "viewAlertHelpButton":
                    ShowAlertHelp();
                    break;
                case "viewDeadlockDetailsButton":
                    ShowDeadlockDetails();
                    break;
                case "viewBlockDetailsButton":
                    ShowBlockReport();
                    break;
            }
        }

        private void ShowBlockReport()
        {
            long alertId = 0;
            try
            {
                alertId = (long)alertsGrid.Selected.Rows[0].Cells["AlertID"].Value;
            }
            catch (Exception)
            {
                ApplicationMessageBox.ShowError(this, "Unable to determine the id of the selected alert.  Please select a single alert and try again.");
                return;
            }

            Log.Verbose("Showing detailsContentPanel, alert id = ", alertId);
            BlockDialog.Show(this, alertId);
        }

        private void ShowDeadlockDetails()
        {
            long alertId = 0;
            try
            {
                alertId = (long)alertsGrid.Selected.Rows[0].Cells["AlertID"].Value;
            } catch (Exception)
            {
                ApplicationMessageBox.ShowError(this, "Unable to determine the id of the selected alert.  Please select a single alert and try again.");
                return;
            }

            Log.Verbose("Showing detailsContentPanel, alert id = ", alertId);
            DeadlockDialog.Show(this, alertId);
        }

        private void ShowAlertHelp()
        {
            if (alertsGrid.Selected.Rows.Count != 0)
            {
                if ((int)alertsGrid.Selected.Rows[0].Cells["Metric"].Value == (int)Metric.SQLdmCollectionServiceStatus)
                {
                    string helpTopic;

                    if (ClickThroughHelper.GetMetricHelp(Metric.SQLdmCollectionServiceStatus, out helpTopic))
                    {
                        ApplicationHelper.ShowHelpTopic(helpTopic);
                    }
                }
                // Get instance id and metric for the selected alert.
                else if (alertsGrid.Selected.Rows[0].Cells["ServerName"].Value != null
                    && alertsGrid.Selected.Rows[0].Cells["Metric"].Value != null)
                {
                    // Get instance and metric.
                    string serverName = alertsGrid.Selected.Rows[0].Cells["ServerName"].Value as string;
                    int metricId = (int)alertsGrid.Selected.Rows[0].Cells["Metric"].Value;
                    long alertId = (long)alertsGrid.Selected.Rows[0].Cells["AlertID"].Value;
                    MonitoredSqlServerWrapper serverWrapper = ApplicationModel.Default.ActiveInstances[serverName];
                    Metric metric = MetricDefinition.GetMetric(metricId);

                    // If server and metric are valid, then proceed, else do nothing.
                    if (serverWrapper != null && metric != Metric.Custom)
                    {
                        string helpTopic;
                        if (ClickThroughHelper.GetMetricHelp(metric, out helpTopic))
                        {
                            ApplicationHelper.ShowHelpTopic(helpTopic);
                        }
                    }
                }
            }
        }

        private void SnoozeAlert()
        {
            if (alertsGrid.Selected.Rows.Count > 0)
            {
                UltraGridRow row = alertsGrid.Selected.Rows[0];
                UltraDataRow dataRow = (UltraDataRow)row.ListObject;
                object metric = dataRow["Metric"];

                if (metric is int)
                {
                    Metric m = (Metric)Enum.ToObject(typeof(Metric), (int)metric);
                    string selectedServerName = (string)dataRow["ServerName"] ?? String.Empty;
                    MonitoredSqlServerWrapper selectedServer =
                        ApplicationModel.Default.ActiveInstances[selectedServerName];

                    // Get Alert Summary.
                    String alertSummary = dataRow["Heading"] as String;

                    // Displays snooze dialog.
                    SnoozeInfo snoozeInfo = SnoozeAlertsDialog.SnoozeAlert(
                        ParentForm,
                        selectedServer.Id,
                        (int)metric,
                        SnoozeAlertsDialog.SnoozeAction.Snooze,
                        alertSummary);

                    if (snoozeInfo != null)
                    {
                        dataRow.ParentCollection.Remove(dataRow);
                    }
                }
            }
        }

        private void ClearAlert(bool allAlerts)
        {
            if (alertsGrid.Selected.Rows.Count > 0)
            {
                UltraGridRow row = alertsGrid.Selected.Rows[0];
                UltraDataRow dataRow = (UltraDataRow)row.ListObject;
                object metric = dataRow["Metric"];
                if (metric is int)
                {
                    Metric m = (Metric)Enum.ToObject(typeof(Metric), (int)metric);
                    switch (m)
                    {
                        case Metric.ReorganisationPct:
                        case Metric.BombedJobs:
                        case Metric.LongJobs:
                        case Metric.LongJobsMinutes:
                        case Metric.ClusterFailover:
                        case Metric.JobCompletion:
                        case Metric.AlwaysOnAvailabilityGroupRoleChange://SQLDM-30487
                            long alertID = (long)dataRow["AlertID"];
                            try
                            {
                                IManagementService managementService =
                                    ManagementServiceHelper.GetDefaultService(
                                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                                managementService.ClearActiveAlerts(alertID, allAlerts);

                                bool activeOnly = appliedFilter.ActiveOnly != null && appliedFilter.ActiveOnly.Value;

                                // remove the row if we are showing only active alerts
                                if (allAlerts)
                                {
                                    string selectedServer = dataRow["ServerName"] as string ?? "";

                                    List<UltraDataRow> rows = new List<UltraDataRow>();
                                    foreach (UltraDataRow udr in dataRow.ParentCollection)
                                    {
                                        if (((bool)udr["Active"]) && metric.Equals(udr["Metric"]))
                                        {
                                            string rowServer = udr["ServerName"] as string ?? "";
                                            if (selectedServer.Equals(rowServer))
                                                rows.Add(udr);
                                        }
                                    }
                                    foreach (UltraDataRow udr in rows)
                                    {
                                        if (activeOnly)
                                            udr.ParentCollection.Remove(udr);
                                        else
                                            udr["Active"] = false;
                                    }
                                }
                                else
                                {
                                    if (activeOnly)
                                        dataRow.ParentCollection.Remove(dataRow);
                                    else
                                        dataRow["Active"] = false;

                                }
                            }
                            catch (Exception e)
                            {
                                ApplicationMessageBox.ShowError(this, e);
                            }

                            break;
                    }
                }
            }
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "gridContextMenu")
            {
                if (alertsGrid.Selected.Rows.Count > 0)
                {
                    bool isViewRealTimeSnapshotButtonEnabled = false;
                    bool isViewHistoricalSnapshotButtonEnabled = false;
                    bool isViewAlertHelpButtonEnabled = false;
                    bool isViewDeadlockDetailsButtonEnabled = false;
                    bool isViewBlockDetailsButtonEnabled = false; 

                    if(alertsGrid.Selected.Rows[0].Cells["Metric"].Value != null)
                    {
                        Metric m = MetricDefinition.GetMetric((int)alertsGrid.Selected.Rows[0].Cells["Metric"].Value);
                        switch(m)
                        {
                            case Metric.Deadlock:
                                isViewDeadlockDetailsButtonEnabled = true;
                                break;
                            case Metric.BlockingAlert:
                                isViewRealTimeSnapshotButtonEnabled = true;
                                isViewBlockDetailsButtonEnabled = true;
                                break;
                            default:
                                isViewRealTimeSnapshotButtonEnabled = m != Metric.Operational && m != Metric.SQLdmCollectionServiceStatus;
                                break;
                        }

                        isViewHistoricalSnapshotButtonEnabled = ClickThroughHelper.ViewSupportsHistoricalSnapshots(m);
                        
                        switch(m)
                        {
                            case Metric.BlockingAlert:
                            case Metric.Deadlock:
                                isViewAlertHelpButtonEnabled = true;
                                break;
                            default:
                                isViewAlertHelpButtonEnabled = isViewRealTimeSnapshotButtonEnabled && m != Metric.Custom;
                                break;
                        }
                    }

                    ((PopupMenuTool)e.Tool).Tools["viewDeadlockDetailsButton"].SharedProps.Visible =
                        isViewDeadlockDetailsButtonEnabled;
                    ((PopupMenuTool)e.Tool).Tools["viewBlockDetailsButton"].SharedProps.Visible =
                        isViewBlockDetailsButtonEnabled;
                    ((PopupMenuTool)e.Tool).Tools["viewRealTimeSnapshotButton"].SharedProps.Visible =
                        isViewRealTimeSnapshotButtonEnabled;
                    ((PopupMenuTool)e.Tool).Tools["viewHistoricalSnapshotButton"].SharedProps.Visible =
                        isViewHistoricalSnapshotButtonEnabled;
                    ((PopupMenuTool)e.Tool).Tools["viewAlertHelpButton"].SharedProps.Visible =
                        isViewAlertHelpButtonEnabled;
                }
                else
                {
                    ((PopupMenuTool)e.Tool).Tools["viewDeadlockDetailsButton"].SharedProps.Visible = false;
                    ((PopupMenuTool)e.Tool).Tools["viewBlockDetailsButton"].SharedProps.Visible = false;
                    ((PopupMenuTool)e.Tool).Tools["viewRealTimeSnapshotButton"].SharedProps.Visible = false;
                    ((PopupMenuTool)e.Tool).Tools["viewHistoricalSnapshotButton"].SharedProps.Visible = false;
                    ((PopupMenuTool)e.Tool).Tools["viewAlertHelpButton"].SharedProps.Visible = false;
                }

                bool isGrouped = alertsGrid.Rows.Count > 0 && alertsGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
            if (e.Tool.Key == "columnContextMenu")
            {
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(alertsGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        private void EditAlertConfiguration()
        {
            if (alertsGrid.Selected.Rows.Count > 0)
            {
                UltraGridRow row = alertsGrid.Selected.Rows[0];
                UltraDataRow dataRow = (UltraDataRow)row.ListObject;
                object server = dataRow["ServerName"];
                if (server is string && !String.IsNullOrEmpty((string)server))
                {
                    MonitoredSqlServerWrapper wrapper = ApplicationModel.Default.ActiveInstances[(string)server];
                    if (wrapper != null)
                    {
                        try
                        {
                            using (
                                AlertConfigurationDialog alertConfigDialog =
                                    new AlertConfigurationDialog(wrapper.Id, false)
                                )
                            {
                                object metric = dataRow["Metric"];
                                if (metric is int)
                                    metric = (Metric)Enum.ToObject(typeof(Metric), ((int)metric));

                                alertConfigDialog.CreateControl();
                                if (metric is Metric)
                                {
                                    alertConfigDialog.Select((Metric)metric);
                                }

                                alertConfigDialog.ShowDialog(this);
                            }
                        }
                        catch (Exception ex)
                        {
                            ApplicationMessageBox.ShowError(this,
                                                            "Unable to retrieve the alert configuration from the SQLDM Repository.  Please resolve the following error and try again.",
                                                            ex);
                        }
                    }
                }
            }
        }

        private void ToggleColumnText()
        {
            ValueList valueList = null;

            if (selectedColumn != null)
            {
                switch (selectedColumn.Key)
                {
                    case "Severity":
                        valueList = alertsGrid.DisplayLayout.ValueLists["Severity"];
                        break;
                    case "StateEvent":
                        valueList = alertsGrid.DisplayLayout.ValueLists["Transitions"];
                        break;
                }

                if (valueList != null)
                {
                    if (valueList.DisplayStyle == ValueListDisplayStyle.Picture)
                    {
                        valueList.DisplayStyle = ValueListDisplayStyle.DisplayTextAndPicture;
                        selectedColumn.Header.Appearance.TextHAlign = HAlign.Default;
                        selectedColumn.CellAppearance.TextHAlign = HAlign.Default;
                        selectedColumn.CellAppearance.ImageHAlign = HAlign.Default;
                        if (valueList.Key == "Severity")
                        {
                            selectedColumn.Header.Caption = "Severity";
                            selectedColumn.Header.Appearance.ForeColor = Color.Black;
                            selectedColumn.Header.Appearance.FontData.Bold = DefaultableBoolean.Default;
                        }
                        else if (valueList.Key == "Transitions")
                        {
                            selectedColumn.Header.Caption = "Change";
                        }

                        selectedColumn.Width =
                            selectedColumn.CalculateAutoResizeWidth(PerformAutoSizeType.VisibleRows, true);
                    }
                    else
                    {
                        valueList.DisplayStyle = ValueListDisplayStyle.Picture;
                        selectedColumn.Width = 24;
                        selectedColumn.Header.Appearance.TextHAlign = HAlign.Center;
                        selectedColumn.CellAppearance.TextHAlign = HAlign.Center;
                        selectedColumn.CellAppearance.ImageHAlign = HAlign.Center;
                        if (valueList.Key == "Severity")
                        {
                            selectedColumn.Header.Caption = "!";
                            selectedColumn.Header.Appearance.ForeColor = Color.Red;
                            selectedColumn.Header.Appearance.FontData.Bold = DefaultableBoolean.True;
                        }
                        else if (valueList.Key == "Transitions")
                        {
                            selectedColumn.Header.Caption = DELTA_STRING;
                        }
                    }
                }
            }
        }

        private void PrintGrid()
        {
            ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            ultraGridPrintDocument.Header.TextLeft =
                string.Format("Alerts as of {0}",
                              DateTime.Now.ToString("G")
                    );
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid()
        {
            ValueListDisplayStyle severityStyle =
                ((ValueList)alertsGrid.DisplayLayout.Bands[0].Columns["Severity"].ValueList).DisplayStyle;
            ValueListDisplayStyle changeStyle =
                ((ValueList)alertsGrid.DisplayLayout.Bands[0].Columns["StateEvent"].ValueList).DisplayStyle;

            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.FileName = "Alerts";
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ((ValueList)alertsGrid.DisplayLayout.Bands[0].Columns["Severity"].ValueList).DisplayStyle =
                        ValueListDisplayStyle.DisplayText;
                    ((ValueList)alertsGrid.DisplayLayout.Bands[0].Columns["StateEvent"].ValueList).DisplayStyle =
                        ValueListDisplayStyle.DisplayText;
                    ultraGridExcelExporter.Export(alertsGrid, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }

                ((ValueList)alertsGrid.DisplayLayout.Bands[0].Columns["Severity"].ValueList).DisplayStyle =
                    severityStyle;
                ((ValueList)alertsGrid.DisplayLayout.Bands[0].Columns["StateEvent"].ValueList).DisplayStyle =
                    changeStyle;
            }
        }

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                alertsGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                alertsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                alertsGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                alertsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void ToggleGroupByBox()
        {
            alertsGrid.DisplayLayout.GroupByBox.Hidden = !alertsGrid.DisplayLayout.GroupByBox.Hidden;
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    alertsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    alertsGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
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
            alertsGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            alertsGrid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(alertsGrid);
            dialog.Show(this);
        }

        #endregion

        private void metricCombo_BeforeDropDown(object sender, CancelEventArgs e)
        {
            ValueListItem selectedItem = metricCombo.SelectedItem;
            InitializeMetricCombo();

            if (selectedItem != null)
            {
                selectedItem = metricCombo.Items.ValueList.FindByDataValue(selectedItem.DataValue);
                if (selectedItem != null)
                {
                    metricCombo.SelectedItem = selectedItem;
                    return;
                }
            }
            metricCombo.SelectedIndex = 0;
        }

        private void showRealTimeViewLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UltraGridRow row = alertsGrid.Selected.Rows[0];
            UltraDataRow dataRow = (UltraDataRow)row.ListObject;

            Metric m = MetricDefinition.GetMetric((int)row.Cells["Metric"].Value);
            switch (m)
            {
                case Metric.Deadlock:
                    ShowDeadlockDetails();
                    break;
                default:
                    NavigateToView(row, false);
                    break;
            }
        }

        private void helpHistoryLinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (showHistoricalView)
            {
                NavigateToView(alertsGrid.Selected.Rows[0], true);
            }
            else
            {
                ShowAlertHelp();
            }
        }

        private void helpHistoryLinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowAlertHelp();
        }

        private void tagCombo_BeforeDropDown(object sender, CancelEventArgs e)
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

        private void splitContainer1_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void splitContainer1_MouseUp(object sender, MouseEventArgs e)
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

        private void alertsGrid_AfterRowRegionSize(object sender, RowScrollRegionEventArgs e)
        {
            if (DetailsPanelVisible && alertsGrid.Selected.Rows.Count > 0 && e.RowScrollRegion.IsActiveScrollRegion)
            {
                UltraGridRow selectedRow = alertsGrid.Selected.Rows[0];
                e.RowScrollRegion.ScrollRowIntoView(selectedRow);

            }
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by AlertView.AdaptFontSize : {0}", stopWatch.ElapsedMilliseconds);
            if (AutoScaleSizeHelper.isScalingRequired)
            {
                ScaleControlAsPerResolution();
            }
        }

        private void ScaleControlAsPerResolution()
        {
            //this.headerStrip.Size = new Size(this.headerStrip.Width, this.headerStrip.Height + 50);
            if (AutoScaleSizeHelper.isLargeSize)
            {
                this.titleLabel.Margin = new Padding(20, 0, 0, 0);
                beginDateCombo.Width += 20;
                endDateCombo.Width += 20;
                beginTimeCombo.Location = new Point(beginTimeCombo.Location.X + 20, beginTimeCombo.Location.Y);
                endTimeCombo.Location = new Point(endTimeCombo.Location.X + 20, endTimeCombo.Location.Y);
                this.dividerVert.Height += 30;
                this.headerStrip.Style = HeaderStripStyle.Large;
                this.headerStrip.Size = new Size(headerStrip.Size.Width, 45);
                return;
            }
            if(AutoScaleSizeHelper.isXLargeSize)
            {
                this.titleLabel.Margin = new Padding(20, 0, 0, 0);
                beginDateCombo.Width += 30;
                beginDateCombo.Location = new Point(beginDateCombo.Location.X + 20);
                lblTo.Width += 20;
                endDateCombo.Width += 30;
                endDateCombo.Location = new Point(endDateCombo.Location.X + 20);
                lblFrom.Width += 20;
                beginTimeCombo.Width += 10;
                endTimeCombo.Width += 10;
                beginTimeCombo.Location = new Point(beginTimeCombo.Location.X + 20, beginTimeCombo.Location.Y);
                endTimeCombo.Location = new Point(endTimeCombo.Location.X + 20, endTimeCombo.Location.Y);
                this.dividerVert.Height += 30;
                this.headerStrip.Style = HeaderStripStyle.Large;
                this.headerStrip.Size = new Size(headerStrip.Size.Width, 45);
                return;
            }
            if (AutoScaleSizeHelper.isXXLargeSize)
            {
                this.titleLabel.Margin = new Padding(30, 0, 0, 0);
                beginDateCombo.Width += 50;
                beginDateCombo.Location = new Point(beginDateCombo.Location.X + 40);
                lblTo.Width += 20;
                endDateCombo.Width += 50;
                endDateCombo.Location = new Point(endDateCombo.Location.X + 40);
                lblFrom.Width += 40;
                beginTimeCombo.Width += 20;
                endTimeCombo.Width += 20;
                beginTimeCombo.Location = new Point(beginTimeCombo.Location.X + 50, beginTimeCombo.Location.Y);
                endTimeCombo.Location = new Point(endTimeCombo.Location.X + 50, endTimeCombo.Location.Y);
                this.dividerVert.Height += 30;
                this.headerStrip.Style = HeaderStripStyle.Large;
                this.headerStrip.Size = new Size(headerStrip.Size.Width, 45);
                return;
            }
            //ChangeTableColumnSize();

        }

        private void ChangeTableColumnSize()
        {
            TableLayoutColumnStyleCollection styles = this.tableLayoutPanel1.ColumnStyles;
            foreach (System.Windows.Forms.ColumnStyle style in styles)
            {
                if(style.SizeType == SizeType.Percent)
                {
                    style.Width = 50;
                }
                //if (style.SizeType == SizeType.Absolute)
                //{
                //    style.Width = SizeType.AutoSize;
                //}
                //else if (style.SizeType == SizeType.AutoSize)
                //{
                //    style.SizeType = SizeType.Percent;

                //    // Set the column width to be a percentage
                //    // of the TableLayoutPanel control's width.
                //    style.Width = 33;
                //}
                //else
                //{
                //    // Set the column width to 50 pixels.
                //    style.SizeType = SizeType.Absolute;
                //    style.Width = 50;
                //}
            }
        }

        private void showDetailsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UltraGridRow row = alertsGrid.Selected.Rows[0];
            //UltraDataRow dataRow = (UltraDataRow)row.ListObject;

            Metric m = MetricDefinition.GetMetric((int)row.Cells["Metric"].Value);
            switch (m)
            {
                case Metric.BlockingAlert:
                    ShowBlockReport();
                    break;
                default:
                    NavigateToView(row, false);
                    break;
            }
        }
    }

    #region Inner Classes and Enumerations

    /// <summary>
    /// This class is added to a child ui element of CellUIElement by a CreationFilter.  This allows us to
    /// construct a tooltip item that will be used instead of the private tool tip item provided by CellUIElement.
    /// </summary>
    public class ToolTipItem : IToolTipItem
    {
        #region IToolTipItem Members

        public ToolTipInfo GetToolTipInfo(Point mousePosition, UIElement element, UIElement previousToolTipElement,
                                          ToolTipInfo toolTipInfoDefault)
        {
            Point location = toolTipInfoDefault.Location;

            CellUIElement cellElement = element.Parent as CellUIElement;
            if (cellElement == null)
                return toolTipInfoDefault;

            ToolTipInfo parentInfo =
                cellElement.ToolTipItem.GetToolTipInfo(mousePosition, cellElement, previousToolTipElement,
                                                       toolTipInfoDefault);

            if (!parentInfo.HasToolTip)
                return parentInfo;

            // null out the editor so that it isn't used to render the tooltip
            parentInfo.Editor = null;
            parentInfo.EditorOwner = null;
            parentInfo.EditorOwnerContext = null;

            // make sure the image is set to none
            parentInfo.ToolTipImage = ToolTipImage.None;

            // get the text for the cell - sniped from CellUIElement (bastards made the property internal)
            UltraGridRow row = cellElement.Row;
            UltraGridColumn column = cellElement.Column;
            if (CellDisplayStyle.PlainText == cellElement.Cell.CellDisplayStyle)
            {
                object dataVal = row.GetCellValue(column);
                parentInfo.ToolTipText =
                    null != dataVal && DBNull.Value != dataVal
                        ? dataVal.ToString()
                        : column.NullTextResolved;
            }
            else // Formatted text
            {
                string text = row.GetCellText(column);
                parentInfo.ToolTipText = null != text ? text : column.NullTextResolved;
            }

            return parentInfo;
        }

        #endregion
    }

    /// <summary>
    /// CreationFilter that is used to provide our own tooltip for a cell.  Should probably only 
    /// be used for columns with an image to prevent the tooltip from showing the image, which scales funny.
    /// </summary>
    public class NoTooltip : IUIElementCreationFilter
    {
        #region IUIElementCreationFilter Members

        public void AfterCreateChildElements(UIElement parent)
        {
        }

        public bool BeforeCreateChildElements(UIElement parent)
        {
            if (parent.GetType() == typeof (EditorWithTextUIElement) && parent.ToolTipItem == null)
            {
                object context = parent.GetContext();
                if (context is UltraGridColumn)
                {
                    string key = ((UltraGridColumn) context).Key;
                    if (key == "Severity" || key == "StateEvent")
                    {
                        ((EditorWithTextUIElement) parent).ToolTipItem = new ToolTipItem();
                    }
                }
            }
            return false;
        }

        #endregion
    }

    public class ValueListBinding : Binding
    {
        private ValueList valueList;

        public ValueListBinding(string propertyName, object dataSource, string dataMemeber)
            : base(propertyName, dataSource, dataMemeber, true)
        {
            valueList = new ValueList();
        }

        public ValueList ValueList
        {
            get { return valueList; }
            set { valueList = value; }
        }

        protected override void OnFormat(ConvertEventArgs cevent)
        {
            if (cevent.DesiredType == typeof (string))
            {
                cevent.Value = ConvertToString(cevent.Value);
            }
            else if (typeof (Image).IsAssignableFrom(cevent.DesiredType))
            {
                cevent.Value = ConvertToBitmap(cevent.Value);
            }

            base.OnFormat(cevent);
        }

        private object ConvertToBitmap(object value)
        {
            ValueListItem item = valueList.FindByDataValue(value);
            return (item != null && item.HasAppearance)
                       ?
                           item.Appearance.Image
                       :
                           null;
        }

        private object ConvertToString(object value)
        {
            ValueListItem item = valueList.FindByDataValue(value);
            return (item != null)
                       ?
                           item.DisplayText
                       :
                           (value != null) ? value.ToString() : "";
        }
    }

    internal class Wrapper<T> : IComparable
    {
        private bool isAllItem;
        private string label;
        private T value;

        public Wrapper(string label)
        {
            this.label = label;
            isAllItem = true;
        }

        public Wrapper(T value, string label)
        {
            this.value = value;
            this.label = label;
        }

        public string Label
        {
            get { return label; }
        }

        public bool IsAllItem()
        {
            return isAllItem;
        }

        public T GetValue()
        {
            return value;
        }

        public override int GetHashCode()
        {
            object o = value;
            if (o == null)
                return 0;

            return value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == GetType())
            {
                object thisValue = value;
                object v = ((Wrapper<T>)obj).value;
                if (v == null)
                    return thisValue == null;
                return v.Equals(value);
            }
            return base.Equals(obj);
        }

        public int CompareTo(object obj)
        {
            Wrapper<T> other = obj as Wrapper<T>;
            if (other == null)
                return 1;

            if (isAllItem)
                return -1;

            if (other.isAllItem)
                return 1;

            return label.CompareTo(other.label);
        }
    }

    [Serializable]
    internal sealed class AlertFilter : ICloneable
    {
        public bool? ActiveOnly;
        public DateTime? BeginDate;
        public DateTime? EndDate;
        public string Instance;
        public Metric? Metric;
        public MonitoredState? Severity;
        public Tag Tag;
        public bool ForRealz; // force the filter to be used when setting as an argument
        public long AlertID = -1; // this is currently cloned!!!!!!!!!!!!!!!!!

        public bool IsEmpty
        {
            get
            {
                if (BeginDate.HasValue ||
                    EndDate.HasValue ||
                    Metric.HasValue ||
                    Severity.HasValue ||
                    ActiveOnly.HasValue || !String.IsNullOrEmpty(Instance) || Tag != null)
                {
                    // add more comparison here...
                    return false;
                }
                return true;
            }
        }

        public class TransitionGroupByEvaluator : Infragistics.Win.UltraWinGrid.IGroupByEvaluatorEx
        {
            private ValueList valueList = null;
            public TransitionGroupByEvaluator(ValueList valueList)
            {
                this.valueList = valueList;
            }
            
            public object GetGroupByValue(UltraGridGroupByRow groupbyRow, UltraGridRow row)
            {
                ValueListItem item = valueList.FindByDataValue(groupbyRow.Value);
                if (item != null)
                {
                    return item.DisplayText;
                }

                return groupbyRow.Value;
            }

            public bool DoesGroupContainRow(UltraGridGroupByRow groupbyRow, UltraGridRow row)
            {
                string groupbyValue = groupbyRow.Value as string;
                ValueListItem item = valueList.FindByDataValue(row.Cells[groupbyRow.Column].Value);
                if (item != null)
                    return groupbyValue.Equals(item.DisplayText);

                return false;
            }

            public int Compare(UltraGridCell cell1, UltraGridCell cell2)
            {
                if (((byte)cell1.Value) == ((byte)cell2.Value))
                    return 0;
                
                ValueListItem item1 = valueList.FindByDataValue(cell1.Value);
                ValueListItem item2 = valueList.FindByDataValue(cell2.Value);
                string text1 = (item1 != null) ? item1.DisplayText : "";
                string text2 = (item2 != null) ? item2.DisplayText : "";

                return text1.CompareTo(text2);
            }
        }


        #region ICloneable Members

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(AlertFilter left, AlertFilter right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AlertFilter left, AlertFilter right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AlertFilter))
                return base.Equals(obj);

            AlertFilter other = obj as AlertFilter;
            if (BeginDate == other.BeginDate && EndDate == other.EndDate &&
                Metric == other.Metric && Severity == other.Severity &&
                ActiveOnly == other.ActiveOnly)
            {
                if (Tag != null)
                {
                    if (other.Tag == null || other.Tag.Id != Tag.Id)
                        return false;
                }
                else
                {
                    if (other.Tag != null)
                        return false;
                }
                if (String.IsNullOrEmpty(Instance) || String.IsNullOrEmpty(other.Instance))
                {
                    return Instance == other.Instance;
                }
                else
                {
                    return Instance.Equals(other.Instance);
                }
            }
            return false;
        }

    }

    [Serializable]
    internal enum StandardAlertsViews
    {
        Active,
        Custom,
        BySeverity,
        ByInstance,
        ByMetric,
        BombedJobs,
        BlockedSessions,
        OldestOpen,
        QueryMonitor,
        Fragmentation,
        ShowDefaultOrExisting
    }

    #endregion
}
