using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Thresholds;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using ChartFX.WinForms;
using System.Drawing;
using System.Data;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server
{
    internal class ServerBaseView : View, IServerView
    {
        protected readonly int instanceId = -1;
        protected bool isUserSysAdmin = false;//SQLdm 10.1 (Barkha Khatri) using MonitoredSqlServer class property
        protected ChartDrillDownProperties drilldownProperties = new ChartDrillDownProperties(); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        protected int currentRealTimeVisibleLimitInMinutes = 0;  // SqlDM 10.2(Anshul Aggarwal) : New History Browser
        protected DateTime? currentHistoricalSnapshotDateTime = null;
        protected DateTime? currentHistoricalStartDateTime = null;
        protected AlertConfiguration alerts;

        // Event to notify of changes in drilldown
        public event EventHandler<ChartDrilldownEventArgs> ChartDrilldown;
        private double? downXValue;

        /// <summary>
        /// The default constructor is required so the designers will work. Generally any derived
        /// view should call the base constructor and pass in the instance ID.
        /// </summary>
        public ServerBaseView()
        {
        }

        public ServerBaseView(int instanceId)
        {
            this.instanceId = instanceId;

            if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
            {
                InitializeLogger(ApplicationModel.Default.ActiveInstances[instanceId].InstanceName);
                isUserSysAdmin = ApplicationModel.Default.AllInstances[instanceId].IsUserSysAdmin;//SQLdm 10.1 (Barkha Khatri) using MonitoredSqlServer class property
                Log.Info("For {0} instance , IsUserSysAdmin value is : {1}.", instanceId, isUserSysAdmin);
            }

            currentRealTimeVisibleLimitInMinutes = ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes;  // SqlDM 10.2(Anshul Aggarwal) : New History Browser
        }

        public int InstanceId
        {
            get { return instanceId; }
        }

        /// <summary>
        /// Current server mode - realtime, historical, custom
        /// </summary>
        public ServerViewMode ViewMode { get; set; }

        /// <summary>
        /// Override this property to manage the historical snapshot start timestamp
        /// </summary>
        public virtual DateTime? HistoricalStartDateTime { get; set; }

        /// <summary>
        /// Override this property to manage the historical snapshot timestamp
        /// </summary>
        public virtual DateTime? HistoricalSnapshotDateTime
        {
            get { return null; }
            set {}
        }

        internal virtual void OnServerAlertConfigurationChanged(IEnumerable<MetricThresholdEntry> thresholdEntries)
        {
            
        }

        internal AlertConfiguration GetSharedAlertConfiguration()
        {
            if (alerts == null)
            {
                alerts = ApplicationModel.Default.GetAlertConfiguration(instanceId);
            }
            return alerts;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (this.IsHandleCreated && !(this is ServerView))
            {   // repost to continue propagation
                this.BeginInvoke((MethodInvoker)delegate { base.OnSizeChanged(e); });
                return;
            } 
            base.OnSizeChanged(e);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                HookUnhookDrilldownHandlers(drilldownProperties, false); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
            }
            base.Dispose(disposing);
        }

        #region Chart Drilldown Functionality
        
        /// <summary>
        /// Allows derived classes to fire ChartDrilldown event
        /// </summary>
        protected virtual void OnChartDrilldown(object sender, ChartDrilldownEventArgs e)
        {
            var handler = ChartDrilldown;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Initializes drilldown functionality for charts.
        /// </summary>
        /// <param name="charts">Charts for drilldown functionality</param>
        protected void InitalizeDrilldown(params Chart[] charts)
        {
            CleanupDrilldown();
            if (charts != null && charts.Length > 0)
            {
                drilldownProperties.AddCharts(charts);
                HookUnhookDrilldownHandlers(drilldownProperties, true);
            }
        }

        /// <summary>
        /// Cleans up drilldown functionality from previous charts
        /// </summary>
        private void CleanupDrilldown()
        {
            HookUnhookDrilldownHandlers(drilldownProperties, false);
            drilldownProperties.ClearCharts();
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Adds/Removes drillldown functionality.
        /// </summary>
        /// <param name="properties">Drilldown properties</param>
        /// <param name="hook">Attach or Dettach drilldown functionality</param>
        protected void HookUnhookDrilldownHandlers(ChartDrillDownProperties properties, bool hook)
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
                            chart.AxisX.LabelTrimming = StringTrimming.Character; // SqlDM 10.2 (Anshul Aggarwal) - New History Browser : Missing x-axis labels
                            chart.AxisX.Style |= AxisStyles.ShowEnds;
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
                if (chart == null || e.Button != MouseButtons.Left || !ChartHelper.IsDrilldownSupportedOnChart(chart) ||
                    !ChartHelper.IsMouseInsideChartArea(e))
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
            if (chart.Series.Count == 0) //SqlDM 10.2 (Anshul Aggarwal) History Browser - Skip cursor change to cross if no data in chart.
                return;

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
                if (chart == null || !ChartHelper.IsDrilldownSupportedOnChart(chart) || chart.AxisX.Sections.Count == 0 )
                    return;

                var downValue = downXValue;
                var upXValue = chart.AxisX.PixelToValue(e.Location.X);
                PerformDrilldownCleanup(chart);

                if (downValue.HasValue && downValue != upXValue)
                {
                    var args = ChartHelper.ConstructDrilldownEventArgs(chart, downValue.Value, upXValue);
                    var handler = ChartDrilldown;
                    if (handler != null && args != null)
                    {
                        Log.Info("Constructed Drilldown Args - End : {0} Minutes {1} ", args.HistoricalSnapshotDateTime, args.Minutes);
                        handler(this, args);
                    }
                }
            }
        }

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

    internal class ServerDesignBaseView : ServerBaseView, IServerDesignView
    {
        protected bool designMode = false;
        protected bool hasDesignChanges = false;

        /// <summary>
        /// The default constructor is required so the designers will work. Generally any derived
        /// view should call the base constructor and pass in the instance ID.
        /// </summary>
        public ServerDesignBaseView()
        {
        }

        public ServerDesignBaseView(int instanceId)
            : base(instanceId)
        {
        }

        public string DesignTab { get; set; }

        public bool DesignModeEnabled
        {
            get { return designMode; }
        }

        public void ToggleDesignMode()
        {
            ToggleDesignMode(!designMode);
        }

        public virtual void ToggleDesignMode(bool enabled)
        {
            designMode = enabled;
        }

        public virtual void CheckIfSaveNeeded()
        {
            if (hasDesignChanges)
            {
                if (DialogResult.Yes == ApplicationMessageBox.ShowQuestion(this, "You have made changes to the current design. Do you wish to save the changes before continuing?"))
                {
                    SaveDashboardDesign();
                }
            }
        }

        public virtual void SaveDashboardDesign()
        {
            
        }
    }
}
