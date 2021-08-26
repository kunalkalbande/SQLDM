using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.DesktopClient.Properties;
using BBS.TracerX;

namespace Idera.SQLdm.DesktopClient.Controls
{
    [ToolboxBitmap(typeof(Panel))]
    public partial class AlertForecastPanel : UserControl
    {
        #region Nested Types

        private class ForecastWindow
        {
            private DataTable data;
            private int       timeframe;
            private DateTime  now;
            private int       topSeverity;
            private double    topAccuracy;
            private string    topMetric;
            private AlertForecastPanel forecastPanel;

            public ForecastWindow(AlertForecastPanel forecastPanel, int timeframe, DataTable data)
            {
                this.forecastPanel = forecastPanel;
                this.timeframe = timeframe;
                this.data      = data;                
            }

            public DateTime Now
            {
                get { return now == DateTime.MinValue ? DateTime.Now : now; }
                set { now = value; }
            }

            public string HeaderText
            {
                get
                {
                    return EndTime.ToShortTimeString();
                }
            }

            public Image StatusImage
            {
                get
                {
                    bool useSmall = forecastPanel.useSmallStatusImages;

                    if (topSeverity == (int)MonitoredState.Critical)
                        return useSmall ? Properties.Resources.StatusCriticalSmall : Properties.Resources.Critical32x32;

                    else if (topSeverity == (int)MonitoredState.Warning)
                        return useSmall ? Properties.Resources.StatusWarningSmall : Properties.Resources.Warning32x32;

                    else if (topSeverity == (int)MonitoredState.OK)
                        return useSmall ? Properties.Resources.StatusOKSmall : Properties.Resources.OK32x32;

                    else
                        return useSmall ? Properties.Resources.Information16x16 : Properties.Resources.Information32x32;
                }
            }

            public string LinkText
            {
                get
                {
                    if (topSeverity > (int)MonitoredState.OK)
                        return string.Format("{0}% chance", topAccuracy * 100);

                    else if (topSeverity == (int)MonitoredState.OK)
                        return "< 20% chance";

                    else
                        return "No Data";
                }
            }

            public string MetricText
            {
                get { return topMetric; }                
            }

            public string Status
            {
                get
                {
                    if (topSeverity == (int)MonitoredState.Critical)
                        return "critical";

                    else if (topSeverity == (int)MonitoredState.Warning)
                        return "warning";

                    else if (topSeverity == (int)MonitoredState.OK)
                        return "ok";

                    else
                        return "info";
                }
            }

            public DateTime StartTime
            {
                get { return EndTime.AddHours(-2); }
            }

            public DateTime EndTime
            {
                get { return now.AddMinutes(timeframe); }
            }

            public void Update(DateTime now, DataTable data)
            {
                this.data = data;
                this.now  = now;

                DataRow topWarningRow;
                DataRow topCriticalRow;
                double topWarningOdds  = 0;
                double topCriticalOdds = 0;

                if (data == null)
                    return;

                // grab the top metric by accuracy
                data.DefaultView.Sort = "accuracy desc";

                // get the top warning row
                data.DefaultView.RowFilter = string.Format("severity = {2} and (expiration > #{0}# and expiration <= #{1}#)", StartTime.ToString("o"), EndTime.ToString("o"), ((int)MonitoredState.Warning).ToString());
                topWarningRow = data.DefaultView.Count > 0 ? data.DefaultView[0].Row : null;
                topWarningOdds = topWarningRow != null ? (double)topWarningRow["accuracy"] : 0;

                // get the top critical row
                data.DefaultView.RowFilter = string.Format("severity = {2} and (expiration > #{0}# and expiration <= #{1}#)", StartTime.ToString("o"), EndTime.ToString("o"), ((int)MonitoredState.Critical).ToString());
                topCriticalRow = data.DefaultView.Count > 0 ? data.DefaultView[0].Row : null;
                topCriticalOdds = topCriticalRow != null ? (double)topCriticalRow["accuracy"] : 0;

                if (topCriticalOdds >= 0.5)
                {
                    topSeverity = (int)MonitoredState.Critical;
                    topMetric   = topCriticalRow["metricname"] as string;
                    topAccuracy = topCriticalOdds;
                    
                }
                else if (topWarningOdds >= 0.5)
                {
                    topSeverity = (int)MonitoredState.Warning;
                    topMetric   = topWarningRow["metricname"] as string;
                    topAccuracy = topWarningOdds;
                }
                else if (topCriticalOdds >= 0.2 || topWarningOdds >= 0.2)
                {
                    if (topCriticalOdds >= topWarningOdds)
                    {
                        topSeverity = (int)MonitoredState.Critical;
                        topMetric   = topCriticalRow["metricname"] as string;
                        topAccuracy = topCriticalOdds;
                    }
                    else
                    {
                        topSeverity = (int)MonitoredState.Warning;
                        topMetric   = topWarningRow["metricname"] as string;
                        topAccuracy = topWarningOdds;
                    }
                }
                else
                {
                    if (topCriticalRow == null && topWarningRow == null)
                    {
                        topSeverity = -1; // no data
                        topMetric   = "";
                        topAccuracy = 0;
                    }
                    else
                    {
                        topSeverity = (int)MonitoredState.OK;  // ok
                        topMetric   = "";
                        topAccuracy = 0;
                    }
                }

                if (topAccuracy >= 0.9)
                    topAccuracy = 0.9;
            }

            public DataTable GetDataForWindow()
            {
                data.DefaultView.RowFilter = string.Format("expiration > #{0}# and expiration <= #{1}#", StartTime.ToString("o"), EndTime.ToString("o"));
                return data.DefaultView.ToTable();
            }
        }

        #endregion

        private static readonly Logger Log = Logger.GetLogger("AlertForecastPanel");

        private DataTable forecastData;
        private bool      inHistoryMode;
        private bool      paEnabled;
        private bool      paIsAdmin;
        private bool      paHasModels;
        private bool      paHasForecasts;
        private DateTime  nextModelRebuild;
        private DateTime  nextForecast;
        private ToolTip   tooltip = new ToolTip();
        private Dictionary<int, ForecastWindow> forecastWindows;
        private bool      useSmallStatusImages;
        private readonly object datalock = new object();

        public bool InHistoryMode
        {
            get { return inHistoryMode; }
            set { inHistoryMode = value; if (inHistoryMode) UpdateControls(); }
        }

        public DateTime NextModelRebuild
        {
            set { nextModelRebuild = value; }
        }

        public DateTime NextForecast
        {
            set { nextForecast = value; }
        }

        public AlertForecastPanel()
        {
            InitializeComponent();

            forecastWindows = new Dictionary<int, ForecastWindow>();
            forecastWindows.Add(120, new ForecastWindow(this, 120, forecastData));
            forecastWindows.Add(240, new ForecastWindow(this, 240, forecastData));
            forecastWindows.Add(360, new ForecastWindow(this, 360, forecastData));
            forecastWindows.Add(480, new ForecastWindow(this, 480, forecastData));
            forecastWindows.Add(600, new ForecastWindow(this, 600, forecastData));
            forecastWindows.Add(720, new ForecastWindow(this, 720, forecastData));

            probability1.Tag = 120;
            probability2.Tag = 240;
            probability3.Tag = 360;
            probability4.Tag = 480;
            probability5.Tag = 600;
            probability6.Tag = 720;

            messagePanel.Dock = DockStyle.Fill;

            ApplicationModel.Default.PredictiveAnalyticsStateChanged += new EventHandler(Default_PredictiveAnalyticsStateChanged);

            this.VisibleChanged += new EventHandler(AlertForecastPanel_VisibleChanged);
        }

        void AlertForecastPanel_VisibleChanged(object sender, EventArgs e)
        {
            Size originalSize = tableLayoutPanel2.Size;
            Rectangle r = new Rectangle(new Point(0,0), tableLayoutPanel2.Size);

            r.Inflate(-10, -10);

            tableLayoutPanel2.Size = r.Size;
            tableLayoutPanel2.Size = originalSize;
        }

        private void AlertForecastPanel_Load(object sender, EventArgs e)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            paIsAdmin = ApplicationModel.Default.UserToken.IsSQLdmAdministrator;

            UpdateServiceValues();            
            UpdateControls();
        }

        private void AlertForecastPanel_Resize(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = splitContainer1.Height / 2;
            useSmallStatusImages = Height < 130;
            UpdateControls();
        }

        private void Default_PredictiveAnalyticsStateChanged(object sender, EventArgs e)
        {
            paIsAdmin = ApplicationModel.Default.UserToken.IsSQLdmAdministrator;

            UpdateServiceValues();
            UpdateControls();
        }

        private void Msg2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // load the help for this view
            ApplicationHelper.ShowHelpTopic(HelpTopics.AlertForecasting);
        }

        private void Msg4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                IManagementService managementService = ManagementServiceHelper.GetDefaultService();

                AuditingEngine.SetContextData(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                // enable PA service
                managementService.SetPredictiveAnalyticsEnabled(true);

                if (managementService.GetPredictiveAnalyticsEnabled())
                {
                    paEnabled = true;
                    ApplicationModel.Default.NotifyPredictiveAnaltyicsStateChanged();

                    // Let them know we were successful
                    MessageBox.Show("Predictive Analytics has been enabled.", "Success Dialog", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateControls();
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Failed to enable Predictive Analytics", ex);
            }
        }

        private void Probability_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DataTable dialogData = null;
            ForecastWindow window = forecastWindows[(int)((LinkLabel)sender).Tag];
            
            lock (datalock)
            {                                
                dialogData = window.GetDataForWindow();
            }

            if (dialogData == null || dialogData.Rows.Count == 0)
            {
                ApplicationMessageBox.ShowInfo(this, "No forecast data available for this timeframe.  The Predictive Analytics service may not be running, or there may not be enough data to make predictions.", false);
                return;
            }
            
            // show the dialog with the data
            using (AlertForecastDialog dialog = new AlertForecastDialog())
            {
                dialog.SortByCritical = window.Status != "warning";
                dialog.StartTime = window.StartTime;
                dialog.EndTime   = window.EndTime;
                dialog.Data      = dialogData;

                dialog.ShowDialog(this);
            }
        }

        private void UpdateControls()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(UpdateControls));
                return;
            }

            paHasForecasts = forecastData != null && forecastData.Rows.Count > 0;

            if (paEnabled)
            {               
                if (!paHasModels)
                {
                    string m = string.Format("Alert forecast models have not been built.");
                    string t = string.Format("The next time forecast models will be built is {0}.", nextModelRebuild);

                    if (nextModelRebuild == DateTime.MinValue)
                        t = "Forecast models are built once a day at 2 AM (default).";

                    ShowMessage(string.Format("{0}\r\n{1}", m, t), "Learn more about forecasting.", "noforecast");
                    return;
                }
                else if (paHasModels && !paHasForecasts)
                {
                    string m = string.Format("Alert forecast models have been built, but no forecasts are available.");
                    string t = string.Format("The next forecast calculations will occur at {0}.", nextForecast.ToShortTimeString());

                    if (nextForecast == DateTime.MinValue)
                        t = "Forecasts are calculated once every 60 minutes.";

                    ShowMessage(string.Format("{0}\r\n{1}", m, t), "Learn more about forecasting.", "noforecast");
                    return;
                }
                else if (inHistoryMode)
                {
                    ShowMessage("Alert forecasting is not supported in historical mode.");
                    return;
                }
            }
            else
            {
                if (paIsAdmin)
                    ShowMessage("Alert forecasting is not enabled.", "Click here to enable.", "forecastdisabled");
                else
                    ShowMessage("Alert forecasting is not enabled.", "Learn more about forecasting.", "noforecast");

                return;
            }

            // Hide message panel
            HideMessage();

            // get the time now, for consistency
            DateTime now = DateTime.Now;

            // update the controls with the top values
            UpdateTimeframe(now, forecastWindows[120], time1, iconpanel1, probability1, metricLabel1);
            UpdateTimeframe(now, forecastWindows[240], time2, iconpanel2, probability2, metricLabel2);
            UpdateTimeframe(now, forecastWindows[360], time3, iconpanel3, probability3, metricLabel3);
            UpdateTimeframe(now, forecastWindows[480], time4, iconpanel4, probability4, metricLabel4);
            UpdateTimeframe(now, forecastWindows[600], time5, iconpanel5, probability5, metricLabel5);
            UpdateTimeframe(now, forecastWindows[720], time6, iconpanel6, probability6, metricLabel6);
        }        

        private void UpdateTimeframe(DateTime now, ForecastWindow window, Label headerLabel, Panel iconPanel, LinkLabel probabilityLink, Label metricLabel)
        {
            window.Update(now, forecastData);

            headerLabel.Text          = window.HeaderText;
            iconPanel.BackgroundImage = window.StatusImage;
            iconPanel.Tag             = window.Status;
            probabilityLink.Text      = window.LinkText;
            metricLabel.Text          = window.MetricText;

            tooltip.SetToolTip(metricLabel, metricLabel.Text);            
        }        
        
        private void UpdateServiceValues()
        {
            try
            {
                IManagementService managementService = ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                paEnabled    = managementService.GetPredictiveAnalyticsEnabled();
                paHasModels  = managementService.GetPredictiveAnalyticsHasModels();                
                nextForecast = managementService.GetNextPredictiveAnalyticsForecast();
                nextModelRebuild = managementService.GetNextPredictiveAnalyticsModelRebuild();

                if (nextForecast != DateTime.MinValue)     nextForecast     = nextForecast.ToLocalTime();
                if (nextModelRebuild != DateTime.MinValue) nextModelRebuild = nextModelRebuild.ToLocalTime();
            }
            catch (Exception ex)
            {
                Log.Error("Exception in UpdateServiceValues", ex);
            }
        }

        private void HideMessage()
        {
            messagePanel.SendToBack();
        }        

        private void ShowMessage(string text)
        {
            ShowMessage(text, string.Empty, string.Empty);
        }

        private void ShowMessage(string headerText, string linkText, string linkData)
        {
            topMessageLabel.Text   = headerText;
            bottomMessageLink.Text = linkText;
            bottomMessageLink.Tag  = linkData;
            bottomMessageLink.Visible = !string.IsNullOrEmpty(linkText);

            // Bring the message information panel to the front.
            messagePanel.BringToFront();
        }

        private void bottomMessageLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string data = ((LinkLabel)sender).Tag as string;

            if (data == "noforecast")
                Msg2_LinkClicked(sender, e);

            else if(data == "forecastdisabled")
                Msg4_LinkClicked(sender, e);
        }

        public void UpdateForecastData(DataTable data)
        {
            lock (datalock)
            {
                if (data != null)
                    forecastData = data.Copy();

                paIsAdmin = ApplicationModel.Default.UserToken.IsSQLdmAdministrator;
                UpdateServiceValues();
                UpdateControls();
            }
        }

        
    }
}
