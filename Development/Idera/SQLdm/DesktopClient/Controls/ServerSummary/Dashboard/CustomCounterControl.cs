using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ChartFX.WinForms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Administration;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    internal partial class CustomCounterControl : DashboardControl
    {
        private const string OPTIONTYPE_COUNTERS = @"Counters";

        private Control contextControl = null;

        private SortedList<string, CustomCounter> counters = new SortedList<string, CustomCounter>();
        private SortedList<string, CustomCounter> selectedCounters = new SortedList<string, CustomCounter>();
        List<int> serverCounters = new List<int>();

        private DataTable customCounterDataTable;

        public CustomCounterControl()
            : base(DashboardPanel.CustomCounters)
        {
            InitializeComponent();
            ChartFxExtensions.SetContextMenu(counterChart, toolbarsManager);

            helpTopic = HelpTopics.ServerDashboardViewCustomCountersPanel;

            headerActionButton.Click += headerActionButton_Click;
            UpdateSelectButton();
        }

        internal override void Initialize(ServerBaseView baseView, ServerSummaryHistoryData history)
        {
            base.Initialize(baseView, history);

            GetCustomCounters();

            if (selectedCounters.Count == 0)
            {
                // show the first available counter if none is selected
                foreach (var counter in counters)
                {
                    if (serverCounters.Contains(counter.Value.MetricID))
                    {
                        selectedCounters.Add(counter.Key, counter.Value);
                        break;
                    }
                }
            }
            UpdateSelectButton();
            CreateChartDataSource();

            InitializeCharts();
        }

        public override void SetOptions(List<DashboardPanelOption> options)
        {
            foreach (var option in options)
            {
                if (option.Type == OPTIONTYPE_COUNTERS)
                {
                    selectedCounters.Clear();
                    GetCustomCounters();
                    foreach (string counterName in option.Values)
                    {
                        if (counters.Keys.Contains(counterName))
                        {
                            selectedCounters.Add(counterName, counters[counterName]);
                        }
                    }
                    UpdateConfigOptions();
                    break;
                }
            }
        }

        private void UpdateConfigOptions()
        {
            List<string> options = selectedCounters.Select(counter => counter.Key).ToList();
            DashboardPanelConfiguration.SetOptions(new List<DashboardPanelOption> { new DashboardPanelOption(OPTIONTYPE_COUNTERS, options) });
        }

        private void headerActionButton_Click(object sender, EventArgs e)
        {
            ShowSelectDialog();
        }

        private void ShowSelectDialog()
        {
            SelectCustomCounters dialog = new SelectCustomCounters(counters.Values, selectedCounters.Values, serverCounters);

            if (DialogResult.OK == dialog.ShowDialog(this))
            {
                selectedCounters = dialog.selectedCounters;
                UpdateConfigOptions();
                UpdateSelectButton();
                CreateChartDataSource();
                ConfigureDataSource();
            }
        }

        private void UpdateSelectButton()
        {
            if (counters.Count > 0 && serverCounters.Count > 0)
            {
                headerActionButton.Text = selectedCounters.Count == 0
                                              ? @"Select Counters"
                                              : (selectedCounters.Count == 1
                                              ? string.Format("{0}{1}",
                                                                selectedCounters.Keys[0].Substring(0, Math.Min(50, selectedCounters.Keys[0].Length)),
                                                                selectedCounters.Keys[0].Length > 50 ? "..." : string.Empty)
                                                     : @"Multiple Counters Selected");
                headerActionButton.ToolTipText = (selectedCounters.Count == 1)
                                                     ? selectedCounters.Keys[0]
                                                     : headerActionButton.Text;
                headerActionButton.Visible =
                    headerActionButtonSeparator.Visible = true;
            }
            else
            {
                headerActionButton.Visible =
                    headerActionButtonSeparator.Visible = false;
            }
        }

        private void GetCustomCounters()
        {
            counters.Clear();

            MetricDefinitions metrics = new MetricDefinitions(true, false, true);
            metrics.Load(Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ConnectionString);

            foreach (int metricID in metrics.GetCounterDefinitionKeys())
            {
                var description = metrics.GetMetricDescription(metricID);
                if (description.HasValue)
                {
                    var counter = new CustomCounter(metrics.GetMetricDefinition(metricID), description.Value,
                                                    metrics.GetCounterDefinition(metricID));
                    counters.Add(counter.Name, counter);
                }
            }

            if (counters.Count > 0)
            {
                // get the list of counters for the current instance
                Dictionary<int, List<int>> counterMap =
                    RepositoryHelper.GetMonitoredServerCustomCounters(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                        instanceId, true);
                if (!counterMap.TryGetValue(instanceId, out serverCounters))
                {
                    serverCounters = new List<int>();
                }

                List<string> removeCounters = (from counter in selectedCounters.Values where !serverCounters.Contains(counter.MetricID) select counter.Name).ToList();

                foreach(string name in removeCounters)
                {
                    selectedCounters.Remove(name);
                }
            }

            UpdateStatus();
        }

        private void CreateChartDataSource()
        {
            // this will keep existing colums even if counters are removed to maintain real time refresh history
            if (customCounterDataTable == null)
            {
                customCounterDataTable = new DataTable();
                customCounterDataTable.Columns.Add("Date", typeof (DateTime));
                customCounterDataTable.PrimaryKey = new DataColumn[] { customCounterDataTable.Columns["Date"] };

                customCounterDataTable.DefaultView.Sort = "Date";
            }
            foreach (var counter in selectedCounters)
            {
                string col = counter.Value.MetricID.ToString();
                if (!customCounterDataTable.Columns.Contains(col))
                {
                    customCounterDataTable.Columns.Add(col, typeof(Decimal));
                }
            }
        }

        private void InitializeCharts()
        {
            // fix the chart context menus to work correctly with new Infragistics controls
            ChartFxExtensions.SetContextMenu(counterChart, toolbarsManager);

            InitializeCounterChart();
            ConfigureCounterChart();
            InitalizeDrilldown(counterChart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void InitializeCounterChart()
        {
            counterChart.Printer.Orientation = PageOrientation.Landscape;
            counterChart.Printer.Compress = true;
            counterChart.Printer.ForceColors = true;
            counterChart.Printer.Document.DocumentName = "Custom Counter Chart";
            counterChart.ToolTipFormat = string.Format("%s\n%v\n%x");
        }

        private void ConfigureCounterChart()
        {
            counterChart.SuspendLayout();
            counterChart.DataSourceSettings.Fields.Clear();

            if (selectedCounters.Count > 0)
            {
                FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
                dateFieldMap.DisplayName = "Date";
                counterChart.DataSourceSettings.Fields.Add(dateFieldMap);

                foreach (var counter in selectedCounters)
                {
                    // only show counters that are linked to this server
                    if (serverCounters.Contains(counter.Value.MetricID))
                    {
                        FieldMap fieldMap = new FieldMap(counter.Value.MetricID.ToString(), FieldUsage.Value);
                        fieldMap.DisplayName = counter.Value.Name;
                        counterChart.DataSourceSettings.Fields.Add(fieldMap);
                    }
                }

                counterChart.DataSourceSettings.ReloadData();
                counterChart_Resize(counterChart, new EventArgs());
                counterChart.AxisY.DataFormat.Decimals = 2;
                counterChart.Invalidate();

                counterChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
                counterChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
                counterChart.BringToFront();
            }

            counterChart.ResumeLayout();
        }

        internal override void ConfigureDataSource()
        {
            using (Log.VerboseCall("ConfigureDataSource"))
            {
                UpdateAlerts();

                try
                {
                    if (IsCounterFound())
                    {
                        ConfigureCounterChart();

                        //historyData.
                        customCounterDataTable.BeginLoadData();
                        customCounterDataTable.Rows.Clear();
                        
                        // SQLDM 10.2 (Anshul Aggarwal) - Added null checks
                        // SQLDM-27456 - History Range control_DC: Multiple errors displayed when custom range is used
                        DataTable historyDataTable = historyData != null ? historyData.CurrentCustomCounterSnapshotsDataTable : null;
                        if (historyDataTable != null && selectedCounters.Count > 0)
                        {
                            string metricIDs = string.Empty;
                            foreach (var counter in selectedCounters.Values)
                            {
                                if (metricIDs.Length > 0)
                                    metricIDs += ",";
                                metricIDs += counter.MetricID;
                            }
                            string viewFilter = string.Format("MetricID in ({0})", metricIDs);

                            if (historyData.HistoricalSnapshotDateTime == null)
                            {
                                DateTime dateLimit = DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));
                                viewFilter = String.Format("Date > #{0:o}# AND {1}", dateLimit, viewFilter);
                            }

                            DataView currentData = new DataView(historyDataTable,   // SQLDM-27456 - History Range control_DC: Multiple errors displayed when custom range is used
                                                                viewFilter,
                                                                "Date",
                                                                DataViewRowState.CurrentRows);

                            foreach (DataRowView row in currentData)
                            {
                                DateTime date = (DateTime)row["Date"];
                                DataRow newRow = customCounterDataTable.Rows.Find(date);
                                if (newRow == null)
                                {
                                    newRow = customCounterDataTable.NewRow();
                                    newRow["Date"] = date;
                                    customCounterDataTable.Rows.Add(newRow);
                                }
                                if (row["DisplayValue"] != DBNull.Value)
                                {
                                    newRow[row["MetricID"].ToString()] = (decimal)row["DisplayValue"];
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "An error occurrred getting the custom counters to display. ", ex);
                }
                finally
                {
                    customCounterDataTable.EndLoadData();
                }
                if (IsCounterFound())
                {
                    counterChart.DataSource = customCounterDataTable;
                    ChartFxExtensions.SetAxisXTimeScale(counterChart, 2);
                    ConfigureCounterChart();
                }
                else
                {
                    counterChart.DataSource = null;
                }

                GetCustomCounters();

                Invalidate();
            }
        }

        protected override void UpdateAlerts()
        {
            List<Pair<int, IComparable>> alerts = new List<Pair<int, IComparable>>();

            if (historyData != null && historyData.CurrentCustomCounterSnapshot != null)
            {
                var activeSnapshot = historyData.CurrentCustomCounterSnapshot;

                foreach (var counter in selectedCounters.Values)
                {
                    CustomCounterSnapshot snapshot;
                    if (activeSnapshot.CustomCounterList.TryGetValue(counter.MetricID, out snapshot))
                    {
                        if (snapshot.DisplayValue != null && snapshot.DisplayValue > 0)
                        {
                            alerts.Add(new Pair<int, IComparable>(snapshot.Definition.MetricID, snapshot.DisplayValue));
                        }
                    }
                }
            }

            SetCustomAlertStatus(alerts);
        }

        private bool IsCounterFound()
        {
            if (serverCounters.Count == 0 || selectedCounters.Count == 0)
                return false;

            return selectedCounters.Any(counter => serverCounters.Contains(counter.Value.MetricID));
        }

        private void UpdateStatus()
        {
            statusLinkLabel.Links.Clear();
            if (counters.Count == 0)
            {
                string linkText = ApplicationHelper.GetEnumDescription(StatusLinkType.Create);
                statusLinkLabel.Text = string.Format("No Custom Counters have been created for SQLdm.\r\n\r\n{0}", linkText);
                statusLinkLabel.Links.Add(statusLinkLabel.Text.IndexOf(linkText), linkText.Length, StatusLinkType.Create);
                statusLinkLabel.Visible = true;
                statusLinkLabel.BringToFront();
                headerStatusLabel.ToolTipText = @"There are no custom counters configured for SQLdm.";
                headerStatusLabel.Image = Properties.Resources.StatusWarningSmall;
                headerStatusLabel.Visible = true;
            }
            else if (serverCounters.Count == 0)
            {
                string linkText = ApplicationHelper.GetEnumDescription(StatusLinkType.ConfigureServer);
                statusLinkLabel.Text = string.Format("No custom counters have been linked to this instance.\r\n\r\n{0}", linkText);
                statusLinkLabel.Links.Add(statusLinkLabel.Text.IndexOf(linkText), linkText.Length, StatusLinkType.ConfigureServer);
                statusLinkLabel.Visible = true;
                statusLinkLabel.BringToFront();
                headerStatusLabel.ToolTipText = @"There are no custom counters linked to this instance.";
                headerStatusLabel.Image = Properties.Resources.StatusWarningSmall;
                headerStatusLabel.Visible = true;
            }
            else if (selectedCounters.Count == 0)
            {
                string linkText = ApplicationHelper.GetEnumDescription(StatusLinkType.Select);
                statusLinkLabel.Text = string.Format("No custom counters have been selected.\r\n\r\n{0}", linkText);
                statusLinkLabel.Links.Add(statusLinkLabel.Text.IndexOf(linkText), linkText.Length, StatusLinkType.Select);
                statusLinkLabel.Visible = true;
                statusLinkLabel.BringToFront();
                headerStatusLabel.ToolTipText = @"There are no counters selected for this panel.";
                headerStatusLabel.Image = Properties.Resources.StatusWarningSmall;
                headerStatusLabel.Visible = true;
            }
            else
            {
                string goodCounters = string.Empty;
                string badCounters = string.Empty;
                headerStatusLabel.Image = Properties.Resources.StatusInfoSmall;
                foreach (var counter in selectedCounters)
                {
                    if (serverCounters.Contains(counter.Value.MetricID))
                    {
                        goodCounters += string.Format("\r\n  {0}", counter.Key);
                    }
                    else
                    {
                        badCounters += string.Format("\r\n  {0}", counter.Key);
                    }
                }
                headerStatusLabel.ToolTipText = goodCounters.Length > 0 ? @"Counters selected for display:" + goodCounters : string.Empty;
                if (badCounters.Length > 0)
                {
                    if (goodCounters.Length > 0)
                    {
                        statusLinkLabel.SendToBack();
                        statusLinkLabel.Visible = false;
                    }
                    else
                    {
                        string linkText = ApplicationHelper.GetEnumDescription(StatusLinkType.Select);
                        statusLinkLabel.Text = string.Format("No custom counters have been selected that are linked to this server.\r\n\r\n{0}", linkText);
                        statusLinkLabel.Links.Add(statusLinkLabel.Text.IndexOf(linkText), linkText.Length, StatusLinkType.Select);
                        statusLinkLabel.Visible = true;
                        statusLinkLabel.BringToFront();
                    }
                    headerStatusLabel.ToolTipText +=
                        string.Format("{0}Counters selected but not linked to this server:{1}",
                                      (goodCounters.Length > 0) ? "\r\n" : string.Empty, badCounters);
                    headerStatusLabel.Image = Properties.Resources.StatusWarningSmall;
                }
                else
                {
                    statusLinkLabel.SendToBack();
                    statusLinkLabel.Visible = false;
                }
                headerStatusLabel.Visible = true;
            }

            UpdateSelectButton();
        }

        #region toolbar

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if (designMode)
            {
                e.Cancel = true;
                return;
            }

            if (e.SourceControl is Chart)
            {
                contextControl = e.SourceControl;
            }
        }

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "configureCustomCountersButton":
                    ApplicationController.Default.ShowAdministrationView(AdministrationView.AdministrationNode.CustomCounters);
                    break;
                case "configureAlertsButton":
                    ConfigureControlAlerts(contextControl);
                    break;
                case "showHelpButton":
                    ShowControlHelp(contextControl);
                    break;
                case "showDetailsButton":
                    ShowControlDetails(contextControl);
                    break;
                case "printChartButton":
                    PrintChart(contextControl);
                    break;
                case "exportChartDataButton":
                    SaveChartData(contextControl);
                    break;
                case "exportChartImageButton":
                    SaveChartImage(contextControl);
                    break;
            }
            contextControl = null;
        }

        #endregion

        #region Chart Actions

        private void ConfigureControlAlerts(Control targetControl)
        {
            if (targetControl != null)
            {
                try
                {
                    AlertConfigurationDialog dialog = new AlertConfigurationDialog(instanceId, false);
                    dialog.Select(Metric.Custom);
                    dialog.ShowDialog(ParentForm);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this,
                                                    "Unable to retrieve the alert configuration from the SQLDM Repository.  Please resolve the following error and try again.",
                                                    ex);
                }
            }
        }

        private void ShowControlHelp(Control targetControl)
        {
            string topic = HelpTopics.ServerDashboardViewCustomCountersPanel;

            ShowHelp(topic);
        }

        private void ShowControlDetails(Control targetControl)
        {
            if (targetControl != null)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.OverviewDetails, ServerDetailsView.Selection.Custom);
            }
        }

        private void PrintChart(Control control)
        {
            if (control is Chart)
            {
                Chart chart = (Chart)control;

                ExportHelper.ChartHelper.PrintChartWithTitle(this, chart, Caption, ultraPrintPreviewDialog);
            }
        }

        private void SaveChartData(Control control)
        {
            if (control is Chart)
            {
                Chart chart = (Chart)control;
                ExportHelper.ChartHelper.ExportToCsv(this, chart, ExportHelper.GetValidFileName(Caption, true));
            }
        }

        private void SaveChartImage(Control control)
        {
            if (control is Chart)
            {
                Chart chart = (Chart)control;
                ExportHelper.ChartHelper.ExportImageWithTitle(this, chart, Caption, ExportHelper.GetValidFileName(Caption, true));
            }
        }

        #endregion

        #region Chart Click Events

        private void chart_MouseClick(object sender, HitTestEventArgs e)
        {
            if (designMode)
            {
                DashboardControl_MouseClick(sender, e);
                return;
            }

            if (e.Button != MouseButtons.Right && e.HitType != HitType.Other
                && !ChartHelper.IsMouseInsideChartArea(e))  //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
            {
                // hit type of other means they clicked on the chart toolbar
                ShowControlDetails((Chart)sender);
            }
        }

        #endregion

        private void counterChart_Resize(object sender, EventArgs e)
        {
            Chart chart = (Chart)sender;
            int maxLegendWidth = chart.Width / 3;

            chart.LegendBox.AutoSize = true;
            chart.UpdateSizeNow();
            if (chart.LegendBox.Width > maxLegendWidth)
            {
                chart.LegendBox.Width = maxLegendWidth;
                chart.Invalidate();
            }
        }

        private void statusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Link.LinkData is StatusLinkType)
            {
                StatusLinkType statusLinkType = (StatusLinkType)e.Link.LinkData;
                switch (statusLinkType)
                {
                    case StatusLinkType.Create:
                        ApplicationController.Default.ShowAdministrationView(AdministrationView.AdministrationNode.CustomCounters);
                        break;
                    case StatusLinkType.ConfigureServer:
                        MonitoredSqlServerInstancePropertiesDialog dialog = new MonitoredSqlServerInstancePropertiesDialog(instanceId);
                        dialog.SelectedPropertyPage = MonitoredSqlServerInstancePropertiesDialogPropertyPages.CustomCounters;
                        if (DialogResult.OK == dialog.ShowDialog(this))
                        {
                            GetCustomCounters();
                            ConfigureCounterChart();
                            ConfigureDataSource();
                        }
                        break;
                    case StatusLinkType.Select:
                        ShowSelectDialog();
                        break;
                    default:
                        break;
                }
            }
        }

        private enum StatusLinkType
        {
            [Description("Create Custom Counters now")]
            Create,
            [Description("Link Custom Counters to server now")]
            ConfigureServer,
            [Description("Select Custom Counters now")]
            Select
        }
    }
}
