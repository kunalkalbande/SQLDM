using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinToolTip;
using Wintellect.PowerCollections;

using BBS.TracerX;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Thresholds;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Idera.SQLdm.DesktopClient.Helpers;
using ChartFX.WinForms;
using System.Data;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    internal partial class DashboardControl : UserControl
    {
        public enum OptionType
        {
            CustomCounter,
            Database,
            DatabaseChartType,
            Disk,
            File
        }

        private Logger logger;
        protected string helpTopic = HelpTopics.ServerDashboardView;

        protected Color headerBackColor;
        protected Color headerBackColor2;
        protected readonly Color headerForeColor;

        protected static readonly Color selectColor = Color.FromArgb(245, 159, 75);
        protected static readonly Color selectColor2 = Color.FromArgb(255, 189, 105);
        protected static readonly Color baselineRangeColor = Color.FromArgb(180, 190, 190);
        protected static readonly Color alertOkColor = Color.FromArgb(31, 128, 31);
        protected static readonly Color alertInfoColor = Color.Blue;
        protected static readonly Color alertInfoColor2 = Color.FromArgb(192, 192, 255);
        protected static readonly Color alertWarningColor = Color.FromArgb(255, 168, 14);
        protected static readonly Color alertWarningColor2 = Color.FromArgb(255, 255, 225);
        protected static readonly Color alertWarningForeColor = Color.FromArgb(72, 72, 72);
        protected static readonly Color alertCriticalColor = Color.FromArgb(153, 0, 0);
        protected static readonly Color alertCriticalColor2 = Color.FromArgb(255, 98, 98);

        protected int instanceId = -1;
        protected ServerSummaryHistoryData historyData;
        protected bool designMode = false;
        protected List<Control> designModeControls = new List<Control>();
        protected bool dragging = false;
        //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        protected ChartDrillDownProperties drilldownProperties = new ChartDrillDownProperties() { ChartCursor = Cursors.Hand };  

        private UltraToolTipInfo alertToolTip = new UltraToolTipInfo();
        private double? downXValue;
        
        public event EventHandler<ChartDrilldownEventArgs> ChartDrilldown;
        public event EventHandler<DropPanelEventArgs> DragDropPanel;

        public DashboardPanelConfiguration DashboardPanelConfiguration { get; private set; }

        /// <summary>
        /// Designer compatibility constructor should not be used
        /// </summary>
        public DashboardControl()
        {
            InitializeComponent();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
            // pull the normal colors from the designer so they match
            setBackColor();
            headerForeColor = dashboardHeaderStrip.ForeColor;
        }

        public DashboardControl(DashboardPanel panel)
            : this()
        {
            DashboardPanelConfiguration = new DashboardPanelConfiguration(0, 0, panel);
        }

        internal virtual void Initialize(ServerBaseView baseView, ServerSummaryHistoryData historyData, DashboardPanelConfiguration panelConfiguration)
        {
            // Set options and position first so initialization logic can use them if needed
            SetPosition(panelConfiguration.Column, panelConfiguration.Row);
            SetOptions(panelConfiguration.Options);

            Initialize(baseView, historyData);
        }

        internal virtual void Initialize(ServerBaseView baseView, ServerSummaryHistoryData historyData)
        {
            BaseView = baseView;
            this.instanceId = baseView.InstanceId;
            this.historyData = historyData;

            InitializeLogger();

            ultraToolTipManager.AutoPopDelay = 0;
            ultraToolTipManager.DisplayStyle = Infragistics.Win.ToolTipDisplayStyle.Office2007;
            ultraToolTipManager.InitialDelay = 500;
            ultraToolTipManager.SetUltraToolTip(dashboardHeaderStrip, alertToolTip);

            hookupDragDrop(this);
            hookupDragDrop(dashboardHeaderStrip);
        }

        private void hookupDragDrop(Control control)
        {
            control.AllowDrop = true;
            control.DragDrop += DashboardControl_DragDrop;
            control.DragEnter += DashboardControl_DragEnter;
            control.DragLeave += DashboardControl_DragLeave;
        }

        internal virtual void ConfigureDataSource()
        {
            
        }

        /// <summary>
        /// Override this method to apply user settings to a dashboard control.
        /// </summary>
        public virtual void ApplySettings()
        {

        }

        /// <summary>
        /// Override this method to save user settings for a dashboard control.
        /// </summary>
        public virtual void SaveSettings()
        {

        }

        /// <summary>
        /// Override this method to get user options for a dashboard control.
        /// </summary>
        public virtual List<DashboardPanelOption> GetOptions()
        {
            return DashboardPanelConfiguration.Options;
        }

        /// <summary>
        /// Override this method to set user options for a dashboard control.
        /// </summary>
        public virtual void SetOptions(List<DashboardPanelOption> options)
        {
            DashboardPanelConfiguration.Options = options;
        }

        /// <summary>
        /// Override this method to show Virtual data when it is enabled.
        /// </summary>
        public virtual void ShowVMData(bool showVM)
        {

        }

        /// <summary>
        /// Override this method to show Alert status
        /// </summary>
        protected virtual void UpdateAlerts()
        {

        }

        internal ServerBaseView BaseView { get; private set; }

        [Category("Appearance")]
        [Description("The name of the panel shown in the title bar on the dashboard.")]
        [DefaultValue("Dashboard Control")]
        public string Caption
        {
            get { return headerCaptionButton.Text; }
            set
            {
                headerCaptionButton.Text = value;
                headerCaptionButton.ToolTipText = string.Format("Help on the {0} area", value);
            }
        }

        protected Logger Log
        {
            get
            {
                if (logger == null)
                {
                    InitializeLogger();
                }

                return logger;
            }
        }

        protected int InstanceId
        {
            get { return instanceId; }
        }

        /// <summary>
        /// Creates a logger for the class based on the type name. 
        /// </summary>
        protected void InitializeLogger()
        {
            string typeName = GetType().Name;
            logger = Logger.GetLogger(typeName);
            Log.Info("Logger initialized.");
        }

        internal void SetDesignMode(bool enabled)
        {
            designMode = enabled;
            AllowDrop = enabled;
            if (BaseView != null)
                UpdateAlerts();

            if (enabled)
            {
                if (designModeControls.Count == 0)
                {
                    hookupMouseActions(this);
                }
            }
            else
            {
                foreach (Control control in designModeControls)
                {
                    control.MouseMove -= DashboardControl_MouseMove;
                    control.MouseDown -= DashboardControl_MouseDown;
                    control.MouseClick -= DashboardControl_MouseClick;
                }
                designModeControls.Clear();
            }
            HookUnhookDrilldownHandlers(drilldownProperties, !enabled); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void hookupMouseActions(Control control)
        {
            foreach (Control child in control.Controls)
            {
                designModeControls.Add(child);
                child.MouseMove += DashboardControl_MouseMove;
                child.MouseDown += DashboardControl_MouseDown;
                child.MouseClick += DashboardControl_MouseClick;
                hookupMouseActions(child);
            }
        }

        internal void SetPosition(int col, int row)
        {
            Log.DebugFormat("{0} set position col {1}, row {2}", Name, col, row);
            DashboardPanelConfiguration.SetPosition(col, row);
        }

        private void setSelectionStatus(bool selected)
        {
            if (selected)
            {
                dashboardHeaderStrip.BackColor = selectColor;
                dashboardHeaderStrip.BackColor2 = selectColor2;
            }
            else
            {
                dashboardHeaderStrip.BackColor = headerBackColor;
                dashboardHeaderStrip.BackColor2 = headerBackColor2;
            }
        }

        #region Alerts

        protected void SetCustomAlertStatus(List<Pair<int, IComparable>> alertValues)
        {
            MonitoredState maxState = MonitoredState.OK;
            StringBuilder alertText = new StringBuilder();

            if (!designMode)
            {
                List<Pair<MonitoredState, AlertConfigurationItem>> activeAlerts =
                    new List<Pair<MonitoredState, AlertConfigurationItem>>();
                var alertConfig = BaseView.GetSharedAlertConfiguration();

                if (alertConfig != null)
                {
                    foreach (Pair<int, IComparable> alertValue in alertValues)
                    {
                        AlertConfigurationItem alertConfigItem = alertConfig[alertValue.First, String.Empty]; // Will need to update this if any of the metrics used here support multi-thresholds

                        if (alertConfigItem != null &&
                            alertConfigItem.Enabled)
                        {
                            MonitoredState severity = alertConfigItem.GetSeverity(alertValue.Second);
                            if (severity > MonitoredState.OK)
                            {
                                activeAlerts.Add(new Pair<MonitoredState, AlertConfigurationItem>(severity,
                                                                                                  alertConfigItem));
                            }
                        }
                    }
                }
                activeAlerts.Sort(new AlertSorter());

                int count = 1;
                foreach (Pair<MonitoredState, AlertConfigurationItem> alert in activeAlerts)
                {
                    if (alertText.Length != 0)
                        alertText.Append("\n");
                    alertText.AppendFormat("{0}. {1} is {2}", count++, alert.Second.Name, alert.First.ToString());

                    if (alert.First > maxState)
                        maxState = alert.First;
                }
            }

            SetAlertStatus(maxState, alertText.ToString());
        }

        protected void SetAlertStatus(List<Pair<Metric, IComparable>> alertValues)
        {
            MonitoredState maxState = MonitoredState.OK;
            StringBuilder alertText = new StringBuilder();

            if (!designMode)
            {
                List<Pair<MonitoredState, AlertConfigurationItem>> activeAlerts =
                    new List<Pair<MonitoredState, AlertConfigurationItem>>();
                var alertConfig = BaseView.GetSharedAlertConfiguration();

                if (alertConfig != null)
                {
                    foreach (Pair<Metric, IComparable> alertValue in alertValues)
                    {
                        AlertConfigurationItem alertConfigItem = alertConfig[alertValue.First, String.Empty]; // Will need to update this if any of the metrics used here support multi-thresholds
                        MonitoredState severity;

                        if (alertConfigItem != null &&
                            alertConfigItem.Enabled)
                        {
                            if (alertValue.First == Metric.ResourceAlert)
                            {
                                MonitoredSqlServerStatus status =
                                    MonitoredSqlServerStatus.FromBackgroundRefresh(instanceId);
                                if (status != null)
                                {
                                    ICollection<Issue> resourceLimiterIssues = status[alertValue.First];
                                    if (resourceLimiterIssues.Count > 0)
                                    {
                                        IEnumerator<Issue> enumerator = resourceLimiterIssues.GetEnumerator();
                                        enumerator.MoveNext();
                                        severity = enumerator.Current.Severity;
                                        if (severity > MonitoredState.OK)
                                        {
                                            activeAlerts.Add(new Pair<MonitoredState, AlertConfigurationItem>(severity,
                                                                                                              alertConfigItem));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                severity = alertConfigItem.GetSeverity(alertValue.Second);
                                if (severity > MonitoredState.OK)
                                {
                                    activeAlerts.Add(new Pair<MonitoredState, AlertConfigurationItem>(severity,
                                                                                                      alertConfigItem));
                                }
                            }
                        }
                    }
                }
                activeAlerts.Sort(new AlertSorter());

                int count = 1;
                foreach (Pair<MonitoredState, AlertConfigurationItem> alert in activeAlerts)
                {
                    if (alertText.Length != 0)
                        alertText.Append("\n");
                    alertText.AppendFormat("{0}. {1} is {2}", count++, alert.Second.Name, alert.First.ToString());

                    if (alert.First > maxState)
                        maxState = alert.First;
                }
            }

            SetAlertStatus(maxState, alertText.ToString());
        }

        private class AlertSorter : IComparer<Pair<MonitoredState, AlertConfigurationItem>>
        {
            public int Compare(Pair<MonitoredState, AlertConfigurationItem> item1, Pair<MonitoredState, AlertConfigurationItem> item2)
            {
                // Sort MonitoredStates in reverse to get most critical on top
                if (item1.First != item2.First)
                    return item2.First.CompareTo(item1.First);

                return item1.Second.GetMetaData().Rank.CompareTo(item2.Second.GetMetaData().Rank);
            }
        }

        protected void SetMultipleItemAlertStatus(List<Pair<Metric, IEnumerable<Pair<string, IComparable>>>> alertValues)
        {
            MonitoredState maxState = MonitoredState.OK;
            StringBuilder alertText = new StringBuilder();

            if (!designMode)
            {
                List<Triple<MonitoredState, AlertConfigurationItem, string>> activeAlerts =
                    new List<Triple<MonitoredState, AlertConfigurationItem, string>>();
                var alertConfig = BaseView.GetSharedAlertConfiguration();

                if (alertConfig != null)
                {
                    foreach (Pair<Metric, IEnumerable<Pair<string, IComparable>>> alertValue in alertValues)
                    {
                        AlertConfigurationItem alertConfigItem = alertConfig[alertValue.First, String.Empty]; // Will need to update this if any of the metrics used here support multi-thresholds
                        MonitoredState severity;    

                        if (alertConfigItem != null &&
                            alertConfigItem.Enabled)
                        {
                            Dictionary<MonitoredState, List<string>> alertLists =
                                new Dictionary<MonitoredState, List<string>>();
                            foreach (Pair<string, IComparable> valueItem in alertValue.Second)
                            {
                                severity = alertConfigItem.GetSeverity(valueItem.Second);
                                if (severity > MonitoredState.OK)
                                {
                                    List<string> alertList;
                                    if (!alertLists.TryGetValue(severity, out alertList))
                                    {
                                        alertList = new List<string>();
                                        alertLists.Add(severity, alertList);
                                    }

                                    alertList.Add(valueItem.First);
                                }
                            }
                            foreach (KeyValuePair<MonitoredState, List<string>> alertList in alertLists)
                            {
                                activeAlerts.Add(
                                    new Triple<MonitoredState, AlertConfigurationItem, string>(alertList.Key,
                                                                                               alertConfigItem,
                                                                                               string.Join(",",
                                                                                                           alertList.
                                                                                                               Value.
                                                                                                               ToArray())));
                            }
                        }
                    }
                }
                activeAlerts.Sort(new MultiItemAlertSorter());

                int count = 1;
                foreach (Triple<MonitoredState, AlertConfigurationItem, string> alert in activeAlerts)
                {
                    if (alertText.Length != 0)
                        alertText.Append("\n");
                    alertText.AppendFormat("{0}. {1} is {2} for {3}", count++, alert.Second.Name, alert.First.ToString(),
                                           alert.Third);

                    if (alert.First > maxState)
                        maxState = alert.First;
                }
            }

            SetAlertStatus(maxState, alertText.ToString());
        }

        private class MultiItemAlertSorter : IComparer<Triple<MonitoredState, AlertConfigurationItem, string>>
        {
            public int Compare(Triple<MonitoredState, AlertConfigurationItem, string> item1, Triple<MonitoredState, AlertConfigurationItem, string> item2)
            {
                // Sort MonitoredStates in reverse to get most critical on top
                if (item1.First != item2.First)
                    return item2.First.CompareTo(item1.First);

                return item1.Second.GetMetaData().Rank.CompareTo(item2.Second.GetMetaData().Rank);
            }
        }

        private void SetAlertStatus(MonitoredState state, string alertText)
        {
            switch (state)
            {
                case MonitoredState.Critical:
                    dashboardHeaderStrip.BackColor = alertCriticalColor;
                    dashboardHeaderStrip.BackColor2 = alertCriticalColor2;
                    dashboardHeaderStrip.ForeColor = headerForeColor;
                    break;
                case MonitoredState.Warning:
                    dashboardHeaderStrip.BackColor = alertWarningColor;
                    dashboardHeaderStrip.BackColor2 = alertWarningColor2;
                    dashboardHeaderStrip.ForeColor = alertWarningForeColor;
                    break;
                case MonitoredState.Informational:
                    dashboardHeaderStrip.BackColor = alertInfoColor;
                    dashboardHeaderStrip.BackColor2 = alertInfoColor2;
                    dashboardHeaderStrip.ForeColor = headerForeColor;
                    break;
                default:
                    dashboardHeaderStrip.BackColor = headerBackColor;
                    dashboardHeaderStrip.BackColor2 = headerBackColor2;
                    dashboardHeaderStrip.ForeColor = headerForeColor;
                    break;
            }

            alertToolTip.ToolTipText = alertText;
        }

        internal virtual void OnServerAlertConfigurationChanged(IEnumerable<MetricThresholdEntry> thresholdEntries)
        {

        }

        private void setBackColor()
        {
            headerBackColor = dashboardHeaderStrip.BackColor;
            headerBackColor2 = dashboardHeaderStrip.BackColor2;
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            Invalidate();
            setBackColor();
        }

        #endregion

        #region Help

        private void headerCaptionButton_Click(object sender, EventArgs e)
        {
            ShowHelp(helpTopic);
        }

        protected void ShowHelp(string topic)
        {
            if (string.IsNullOrEmpty(topic))
                ApplicationHelper.ShowHelpTopic(helpTopic);
            else
                ApplicationHelper.ShowHelpTopic(topic);
        }

        #endregion

        #region Drag and Drop

        private void DashboardControl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DraggablePanel)))
            {
                e.Effect = DragDropEffects.Copy;
                Log.VerboseFormat("{0} {1} DragEnter Copy", Name, ((Control)sender).Name);
                setSelectionStatus(true);
            }
            else if (e.Data.GetDataPresent(typeof(DashboardPanelConfiguration)))
            {
                e.Effect = DragDropEffects.Move;
                Log.VerboseFormat("{0} {1} DragEnter Move", Name, ((Control)sender).Name);
                setSelectionStatus(true);
            }
        }

        private void DashboardControl_DragDrop(object sender, DragEventArgs e)
        {
            setSelectionStatus(false);
            if (e.Data.GetDataPresent(typeof(DraggablePanel)))
            {
                DraggablePanel panel = (DraggablePanel) e.Data.GetData(typeof (DraggablePanel));
                // check to see if the data was cleared out
                if (panel != null)
                {
                    Log.DebugFormat("{0} {1} DragDrop Copy {2}", Name, ((Control) sender).Name, panel.DashboardPanel);
                    // clear out the data because when prompting the mouse moves, the event fires again for the new control
                    //    and if the same data is there, it prompts them again
                    e.Data.SetData(typeof(DraggablePanel), null);
                    if (DragDropPanel != null)
                        DragDropPanel(this, new DropPanelEventArgs(panel.DashboardPanel, PanelDragType.Drop));
                }
            }
            else if (e.Data.GetDataPresent(typeof(DashboardPanelConfiguration)))
            {
                DashboardPanelConfiguration panelConfig = (DashboardPanelConfiguration)e.Data.GetData(typeof(DashboardPanelConfiguration));
                // check to see if the data was cleared out
                if (panelConfig != null)
                {
                    Log.DebugFormat("{0} {1} DragDrop Swap {2} at {3}, {4}", Name, ((Control) sender).Name,
                                    Enum.GetName(typeof (DashboardPanel), panelConfig.Panel), panelConfig.Column,
                                    panelConfig.Row);
                    // clear out the data because when it gets moved under the mouse, the event fires again for the new control
                    //    and if the same data is there, it swaps them again
                    e.Data.SetData(typeof (DashboardPanelConfiguration), null);
                    if (DragDropPanel != null)
                        DragDropPanel(this, new DropPanelEventArgs(panelConfig, PanelDragType.Swap));
                }
            }
        }

        private void DashboardControl_DragLeave(object sender, EventArgs e)
        {
            Log.VerboseFormat("{0} {1} DragLeave", Name, ((Control)sender).Name);
            setSelectionStatus(false);
        }

        private void DashboardControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!designMode)
            {
                return;
            }
            //Log.VerboseFormat("{0} {1} MouseMove", Name, ((Control)sender).Name);

            if (e.Button.Equals(MouseButtons.Left) && !dragging)
            {
                DataObject data = new DataObject();
                data.SetData(DashboardPanelConfiguration);
                Log.DebugFormat("{0} {1} MouseMove begin dragging config {2} at {3}, {4}", Name, ((Control)sender).Name, Enum.GetName(typeof(DashboardPanel), DashboardPanelConfiguration.Panel), DashboardPanelConfiguration.Column, DashboardPanelConfiguration.Row);
                DoDragDrop(data, DragDropEffects.Move);
                dragging = true;
            }
        }

        private void DashboardControl_MouseDown(object sender, MouseEventArgs e)
        {
            Log.VerboseFormat("{0} {1} MouseDown {2}", Name, ((Control)sender).Name, e.Button);
            if (e.Button.Equals(MouseButtons.Left))
            {
                Log.VerboseFormat("{0} {1} MouseDown left clear dragging", Name, ((Control)sender).Name);
                dragging = false;
            }
        }

        private void DashboardControl_MouseUp(object sender, MouseEventArgs e)
        {
            Log.VerboseFormat("{0} {1} MouseUp clear dragging", Name, ((Control)sender).Name);
            dragging = false;
        }

        protected void DashboardControl_MouseClick(object sender, MouseEventArgs e)
        {
            OnMouseClick(e);
        }

        #endregion

        #region Chart Drilldown Functionality

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Initializes drilldown functionality for charts.
        /// </summary>
        /// <param name="charts">Charts for drilldown functionality</param>
        protected void InitalizeDrilldown(params Chart[] charts)
        {
            HookUnhookDrilldownHandlers(drilldownProperties, false);
            drilldownProperties.ClearCharts();
            drilldownProperties.AddCharts(charts);
            HookUnhookDrilldownHandlers(drilldownProperties, true);
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Adds/Removes drillldown functionality.
        /// </summary>
        /// <param name="properties">Drilldown properties</param>
        /// <param name="hook">Attach or Dettach drilldown functionality</param>
        private void HookUnhookDrilldownHandlers(ChartDrillDownProperties properties, bool hook)
        {
            using (Log.InfoCall("HookUnhookDrilldownHandlers"))
            {
                if (properties == null)
                    return;

                if (properties.Charts != null)
                {
                    foreach (var chart in properties.Charts)
                    {
                        chart.MouseDown -= Chart_MouseDown;
                        chart.MouseMove -= Chart_MouseMove;
                        chart.Cursor = properties.ChartCursor;
                        PerformDrilldownCleanup(chart);
                        Log.Info("Removed drilldown functionality from chart - " + chart.Name);
                        if (hook)
                        {
                            chart.MouseDown += Chart_MouseDown;
                            chart.MouseMove += Chart_MouseMove; 
                            Log.Info("Added drilldown functionality to chart - " + chart.Name);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Starts drilldown on clicked chart.
        /// </summary>
        private void Chart_MouseDown(object sender, HitTestEventArgs e)
        {
            using (Log.InfoCall("Chart_MouseDown"))
            {
                Chart chart = sender as Chart;
                if (chart == null || e.Button != MouseButtons.Left || !ChartHelper.IsDrilldownSupportedOnChart(chart) 
                    || !ChartHelper.IsMouseInsideChartArea(e))
                    return;

                if (chart.AxisX.Sections.Count != 0) //Perform cleanup if not already performed.
                    PerformDrilldownCleanup(chart);

                chart.SuspendLayout(); // Need to perform several changes
                chart.Highlight.Enabled = false; // Chartfx highlighting disabled

                chart.MouseUp -= Chart_MouseUp;
                chart.MouseUp += Chart_MouseUp;
                chart.PrePaint -= Chart_PrePaint;
                chart.PrePaint += Chart_PrePaint;
                chart.GetTip -= Chart_GetTip;
                chart.GetTip += Chart_GetTip;

                downXValue = chart.AxisX.PixelToValue(e.Location.X);
                ChartHelper.StartChartDrilldown(chart, downXValue.Value);
                chart.ResumeLayout(false); // Finished changes
            }
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Moves the selected region as the mouse moves.
        /// </summary>
        private void Chart_MouseMove(object sender, HitTestEventArgs e)
        {
            Chart chart = (Chart)sender;
            if (chart.AxisX.Sections.Count == 0 || !downXValue.HasValue)
            {
                ChartHelper.OnDrilldownHover(chart, e, drilldownProperties.ChartCursor);
                return;
            }

            if (e.Button == MouseButtons.None)
            {
                PerformDrilldownCleanup(chart);
                return;
            }

            var curXValue = chart.AxisX.PixelToValue(e.Location.X);
            ChartHelper.UpdateChartDrilldown(chart, downXValue.Value, curXValue);
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Removes AxisSection tooltip while user perform drilldown.
        /// </summary>
        private void Chart_GetTip(object sender, GetTipEventArgs e)
        {
            Chart chart = sender as Chart;
            if (chart == null || chart.AxisX.Sections.Count == 0 || e.HitType != HitType.AxisSection)
                return;
            e.Text = string.Empty; //Need to remove the axis section tooltips
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Updates drilldown UI as x-axis duration changes due to real-time data.
        /// </summary>
        private void Chart_PrePaint(object sender, CustomPaintEventArgs e)
        {
            ChartHelper.OnChartPrePaint(sender);
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Fires drilldown event notifying the selected drilldown range.
        /// </summary>
        private void Chart_MouseUp(object sender, HitTestEventArgs e)
        {
            using (Log.InfoCall("Chart_MouseUp"))
            {
                Chart chart = sender as Chart;
                if (chart == null || chart.AxisX.Sections.Count == 0)
                    return;

                var downValue = downXValue;
                var upXValue = chart.AxisX.PixelToValue(e.Location.X);
                PerformDrilldownCleanup(chart); //Cleansup drilldown UI from the chart
                if (downValue.HasValue && downValue != upXValue)
                {
                    var handler = ChartDrilldown;
                    var args = ChartHelper.ConstructDrilldownEventArgs(chart, downValue.Value, upXValue);
                    if (handler != null && args != null)
                    {
                        Log.Info("Constructed Drilldown Args - End : {0}, Minutes : {1} ", args.HistoricalSnapshotDateTime, args.Minutes);
                        handler(this, args);
                    }
                }
            }
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Cleans up previous drilldown UI from chart.
        /// </summary>
        /// <param name="chart">Chart to cleanup</param>
        private void PerformDrilldownCleanup(Chart chart)
        {
            using (Log.InfoCall("PerformDrilldownCleanup"))
            {
                chart.SuspendLayout();

                chart.MouseUp -= Chart_MouseUp;  
                chart.PrePaint -= Chart_PrePaint;
                chart.GetTip -= Chart_GetTip;
                downXValue = null;
                chart.AxisX.Sections.Clear();
                chart.Highlight.Enabled = true;

                chart.ResumeLayout(false);
            }
        }
        
        #endregion
    }
}
