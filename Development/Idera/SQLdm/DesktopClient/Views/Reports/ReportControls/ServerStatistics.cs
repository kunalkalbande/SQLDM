using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class ServerStatistics : ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("Server Statisitics");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(9);

        protected enum MasterMetric { ResponseTime, CPUPercentage, MemoryUsage, DiskBusy, SessionCount, }

        readonly ValueListItem metricResponseTime = new ValueListItem(MasterMetric.ResponseTime, "Response Time");
        readonly ValueListItem metricCPUPercent = new ValueListItem(MasterMetric.CPUPercentage, "% CPU Usage");
        readonly ValueListItem metricMemoryUsage = new ValueListItem(MasterMetric.MemoryUsage, "Memory Usage (GB)");
        readonly ValueListItem metricDiskBusy = new ValueListItem(MasterMetric.DiskBusy, "% Disk Busy");
        readonly ValueListItem metricSessionCount = new ValueListItem(MasterMetric.SessionCount, "Session Count");

        protected ValueListItem compareToNothing;

        public ServerStatistics()
        {
            InitializeComponent();
            base.AdaptFontSize();
        }

        public override bool CanRunReport(out string message)
        {
            message = String.Empty;

            if (instanceCombo.SelectedIndex == 0)
            {
                message = "A SQL Server instance must be selected to generate this report.";
                return false;
            }

            this.SetReportParameters();
            return true;
        }

        public override void InitReport()
        {
            base.InitReport();
            InitializeReportViewer();
            InitializeMetricCombo();
            InitializeCompareToCombo();
            periodCombo.SelectedItem = period7;
            periodCombo.Items.Remove(periodToday);
            compareDateStartPicker.Enabled = false;
            compareDatabaseEndDate.Enabled = false;
            ReportType = ReportTypes.ServerStatistics;

            if (drillThroughArguments != null)
            {
                IList<ReportParameter> paramsList = ((LocalReport)drillThroughArguments.Report).OriginalParametersToDrillthrough;

                try
                {
                    switch ((MasterMetric)(Convert.ToInt32(paramsList[1].Values[0])))
                    {
                        case MasterMetric.ResponseTime:
                            masterMetricCombo.SelectedItem = metricResponseTime;
                            break;
                        case MasterMetric.CPUPercentage:
                            masterMetricCombo.SelectedItem = metricCPUPercent;
                            break;
                        case MasterMetric.DiskBusy:
                            masterMetricCombo.SelectedItem = metricDiskBusy;
                            break;
                        case MasterMetric.MemoryUsage:
                            masterMetricCombo.SelectedItem = metricMemoryUsage;
                            break;
                        case MasterMetric.SessionCount:
                            masterMetricCombo.SelectedItem = metricSessionCount;
                            break;
                        default:
                            masterMetricCombo.SelectedItem = metricResponseTime;
                            break;
                    }
                }
                catch
                {
                    masterMetricCombo.SelectedItem = metricResponseTime;
                }
            }
            RunReport(true);
        }

        private void InitializeMetricCombo()
        {
            masterMetricCombo.Items.Add(metricResponseTime);
            masterMetricCombo.Items.Add(metricCPUPercent);
            masterMetricCombo.Items.Add(metricMemoryUsage);
            masterMetricCombo.Items.Add(metricDiskBusy);
            masterMetricCombo.SelectedItem = metricResponseTime;
        }

        private void InitializeCompareToCombo()
        {
            compareInstanceCombo.Items.Clear();
            ValueListItem[] instances = new ValueListItem[ApplicationModel.Default.AllInstances.Count];
            ValueListItem instance;

            int i = 0;

            foreach (MonitoredSqlServer server in ApplicationModel.Default.AllInstances.Values)
            {
                instance = new ValueListItem(server, server.InstanceName);
                instances[i++] = instance;
            }
            compareToNothing = new ValueListItem(null, "< Select a Server >");
            compareInstanceCombo.Items.Add(compareToNothing);
            compareInstanceCombo.Items.AddRange(instances);
            compareInstanceCombo.SelectedItem = compareToNothing;
        }

        protected override void SetReportParameters()
        {
            base.SetReportParameters();

            if (masterMetricCombo.SelectedItem != null)
            {
                reportParameters.Add("MasterMetric", ((ValueListItem)masterMetricCombo.SelectedItem).DataValue);
            }
            else
            {
                reportParameters.Add("MasterMetric", 0);
            }

            if ((compareInstanceCombo.SelectedItem != compareToNothing) && (compareInstanceCombo.SelectedItem != null))
            {
                reportParameters.Add("CompareToId", ((MonitoredSqlServer)((ValueListItem)compareInstanceCombo.SelectedItem).DataValue).Id);
            }
            else
            {
                reportParameters.Add("CompareToId", 0);
            }

            DateTime selected = compareDateStartPicker.Value;
            reportParameters.Add("CompareToDate", new DateTime(selected.Year, selected.Month, selected.Day, startHoursTimeEditor.Time.Hours, startHoursTimeEditor.Time.Minutes, 0).ToUniversalTime());

            ValueListItem selectedInstance = (ValueListItem)instanceCombo.SelectedItem;
            ValueListItem selectedCompareInstance = (ValueListItem)compareInstanceCombo.SelectedItem;

            if (selectedInstance == null)
            {
                selectedInstance = new ValueListItem(0, "All Servers");
            }
            else
            {
                if (selectedInstance.DataValue == null) selectedInstance = new ValueListItem(0, "All Servers");
                if (instanceCombo.SelectedIndex == 0) selectedInstance = new ValueListItem(0, "All Servers");
            }
            if (selectedCompareInstance == null)
            {
                selectedCompareInstance = new ValueListItem(0, "No Compare Server");
            }
            else
            {
                if (selectedCompareInstance.DataValue == null) selectedCompareInstance = new ValueListItem(0, "No Compare Server");
                if (compareInstanceCombo.SelectedIndex == 0) selectedCompareInstance = new ValueListItem(0, "No Compare Server");
            }
            reportParameters.Add("serverName", reportData.instanceID.ToString());
            reportParameters.Add("GUIServers", selectedInstance.DisplayText);
            reportParameters.Add("GUICompareTo", selectedCompareInstance.DisplayText);

            if (periodCombo.SelectedItem != periodSetCustom) return;
            reportParameters.Add("rsStart", reportData.dateRanges[0].UtcStart.ToLocalTime().ToString("yyyy'-'MM'-'dd HH':'mm':'ss"));
            reportParameters.Add("rsEnd", reportData.dateRanges[0].UtcEnd.ToLocalTime().ToString("yyyy'-'MM'-'dd HH':'mm':'ss"));
        }

        /// <summary>
        /// This method is returning the report parameters.  The names must match what is set in the RDL file.
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, object> GetReportParmeters()
        {
            reportParameters.Clear();
            reportParameters.Add("Period", ((int)reportData.periodType).ToString());

            //reportParameters.Add("UTCOffset", reportData.dateRanges[0].UtcOffsetMinutes.ToString());
            reportParameters.Add("Interval", ((int)reportData.sampleSize).ToString());
            reportParameters.Add("MasterMetric", ((int)(MasterMetric)(((ValueListItem)masterMetricCombo.SelectedItem).DataValue)).ToString());
            reportParameters.Add("serverName", reportData.instanceID.ToString());

            if ((compareInstanceCombo.SelectedItem != compareToNothing) & (compareInstanceCombo.SelectedItem != null))
            {
                reportParameters.Add("CompareToId", ((int)((MonitoredSqlServer)((ValueListItem)compareInstanceCombo.SelectedItem).DataValue).Id).ToString());
                reportParameters.Add("CompareTo", ((int)((MonitoredSqlServer)((ValueListItem)compareInstanceCombo.SelectedItem).DataValue).Id).ToString());
            }

            DateTime selected = compareDateStartPicker.Value;
            reportParameters.Add("CompareStartRange", new DateTime(selected.Year, selected.Month, selected.Day, startHoursTimeEditor.Time.Hours, startHoursTimeEditor.Time.Minutes, 0));
            if (periodCombo.SelectedItem == periodSetCustom)
            {
                reportParameters.Add("rsStart",
                                     reportData.dateRanges[0].UtcStart.ToLocalTime().ToString(
                                         "yyyy'-'MM'-'dd HH':'mm':'ss"));
                reportParameters.Add("rsEnd",
                                     reportData.dateRanges[0].UtcEnd.ToLocalTime().ToString(
                                         "yyyy'-'MM'-'dd HH':'mm':'ss"));
            }

            reportParameters.Add("rsStartHours", startHoursTimeEditor.Time.Hours);
            reportParameters.Add("rsEndHours", endHoursTimeEditor.Time.Hours);

            return reportParameters;
        }

        override protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall())
            {
                WorkerData localReportData = (WorkerData)e.Argument;
                ReportDataSource dataSource;

                Log.Debug("localReportData.reportType = ", localReportData.reportType);
                localReportData.dataSources = new ReportDataSource[1];

                dataSource = new ReportDataSource("ServerStatistics");

                passthroughParameters.Clear();
                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
                passthroughParameters.Add(new ReportParameter("GUIServers", (string)localReportData.reportParameters["GUIServers"]));
                passthroughParameters.Add(new ReportParameter("GUICompareTo", (string)localReportData.reportParameters["GUICompareTo"]));
                passthroughParameters.Add(new ReportParameter("serverName", reportData.instanceID.ToString()));
                passthroughParameters.Add(new ReportParameter("Interval", ((int)reportData.sampleSize).ToString()));
                passthroughParameters.Add(new ReportParameter("Period", ((int)reportData.periodType).ToString()));
                passthroughParameters.Add(new ReportParameter("CompareStartRange", localReportData.reportParameters["CompareToDate"].ToString()));
                passthroughParameters.Add(new ReportParameter("CompareToId", localReportData.reportParameters["CompareToId"].ToString()));

                if (localReportData.reportParameters.ContainsKey("rsStart")) passthroughParameters.Add(new ReportParameter("rsStart",
                                                                  localReportData.reportParameters["rsStart"].ToString()));
                if (localReportData.reportParameters.ContainsKey("rsEnd")) passthroughParameters.Add(new ReportParameter("rsEnd",
                                                                  localReportData.reportParameters["rsEnd"].ToString()));

                if (reportData.reportParameters.ContainsKey("CompareToId"))
                {
                    dataSource.Value = RepositoryHelper.GetReportData("p_GetMasterMetrics",
                                                                      localReportData.instanceID,
                                                                      localReportData.dateRanges[0].UtcStart,
                                                                      localReportData.dateRanges[0].UtcEnd,
                                                                      localReportData.dateRanges[0].UtcOffsetMinutes,
                                                                      (int)localReportData.sampleSize,
                                                                      (int)(MasterMetric)localReportData.reportParameters["MasterMetric"],
                                                                      (int)localReportData.reportParameters["CompareToId"],
                                                                      (DateTime)localReportData.reportParameters["CompareToDate"]);
                }
                else
                {
                    dataSource.Value = RepositoryHelper.GetReportData("p_GetMasterMetrics",
                                                                      localReportData.instanceID,
                                                                      localReportData.dateRanges[0].UtcStart,
                                                                      localReportData.dateRanges[0].UtcEnd,
                                                                      localReportData.dateRanges[0].UtcOffsetMinutes,
                                                                      (int)localReportData.sampleSize,
                                                                      (int)(MasterMetric)localReportData.reportParameters["MasterMetric"],
                                                                      DBNull.Value,
                                                                      (DateTime)localReportData.reportParameters["CompareToDate"]);
                }
                localReportData.dataSources[0] = dataSource;

                if (localReportData.cancelled)
                {
                    Log.Debug("reportData.cancelled = true.");
                    e.Cancel = true;
                }
                else
                {
                    e.Result = localReportData;
                }
            }
        }

        override protected void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            using (Log.DebugCall())
            {
                // Make sure this call is for the most recently requested report.
                Log.Debug("(reportData.bgWorker == sender) = ", reportData.bgWorker == sender);
                if (reportData.bgWorker == sender)
                {
                    // This event handler was called by the currently active report
                    if (reportData.cancelled)
                    {
                        Log.Debug("reportData.cancelled = true.");
                        return;
                    }
                    else if (e.Error != null)
                    {
                        if (e.Error.GetType() == typeof(System.Data.SqlClient.SqlException) &&
                            e.Error.Message.ToLower().Contains("msxmlsql.dll"))//
                        {
                            ApplicationMessageBox msgbox1 = new ApplicationMessageBox();
                            Exception msg = new Exception("An error occurred while retrieving data for the report.  It may be due to the problem described by the article available at http://support.microsoft.com/Default.aspx?kbid=918767", e.Error);
                            Log.Error("Showing message box: ", msg);
                            msgbox1.Message = msg;
                            msgbox1.SetCustomButtons("OK", "View Article");
                            msgbox1.Symbol = Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol.Error;
                            msgbox1.Show(this);
                            if (msgbox1.CustomDialogResult == Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDialogResult.Button2)
                            {
                                Process.Start("http://support.microsoft.com/Default.aspx?kbid=918767");
                            }
                        }
                        else
                        {
                            ApplicationMessageBox.ShowError(this, "An error occurred while retrieving data for the report.  ",
                                            e.Error);
                        }

                        State = UIState.NoDataAcquired;
                    }
                    else
                    {
                        try
                        {
                            reportViewer.Reset();
                            reportViewer.LocalReport.EnableHyperlinks = true;

                            using (Stream stream = GetType().Assembly.GetManifestResourceStream(
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.ServerStatistics.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }

                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "ServerStatistics";
                            State = UIState.Rendered;
                        }
                        catch (Exception exception)
                        {
                            ApplicationMessageBox.ShowError(ParentForm, "An error occurred while refreshing the report.", exception);
                            State = UIState.ParmsNeeded;
                        }
                    }
                }
            }
        }

        override protected void reportViewer_Drillthrough(object sender, DrillthroughEventArgs e)
        {
            using (Log.DebugCall())
            {
                e.Cancel = true; // We'll handle this our way.
                Logger.Root.ConsoleTraceLevel = BBS.TracerX.TraceLevel.Verbose;
                Log.Debug("e.ReportPath = ", e.ReportPath);
                LocalReport rpt = (LocalReport)e.Report;
            }
        }

        public override void ResetFilterCriteria()
        {
            base.ResetFilterCriteria();
            masterMetricCombo.SelectedItem = metricResponseTime;
            compareInstanceCombo.SelectedItem = compareToNothing;
            UpdateCompareStartDate();
            UpdateCompareEndDate();
            compareDateStartPicker.Enabled = false;
            compareDatabaseEndDate.Enabled = false;
            ResetTimeFilter();
        }

        /// <summary>
        /// Reset StartHours and EndHours
        /// </summary>
        private void ResetTimeFilter()
        {
            // If customDates is null then we dont need to set startHoursTimeEditor and endHoursTimeEditor
            if (customDates == null) return;

            startHoursTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            endHoursTimeEditor.Time = System.TimeSpan.Parse("23:00:00");
        }

        private void instanceCombo_SelectionChanged(object sender, EventArgs e)
        {
            MonitoredSqlServer selectedServer = (MonitoredSqlServer)((ValueListItem)instanceCombo.SelectedItem).DataValue;

            if (selectedServer == null)
            {
                return;
            }
            InitializeCompareToCombo();

            foreach (ValueListItem server in compareInstanceCombo.Items)
            {
                if (server.DataValue != null)
                {
                    if (((MonitoredSqlServer)server.DataValue).Id == selectedServer.Id)
                    {
                        compareInstanceCombo.Items.Remove(server);
                        break;
                    }
                }
            }
        }

        private void compareIndex_IndexChanged(object sender, EventArgs e)
        {
            if (compareInstanceCombo.SelectedItem != null)
            {
                compareDateStartPicker.Enabled = true;
                UpdateCompareStartDate();
                UpdateCompareEndDate();
            }
            else
            {
                compareDateStartPicker.Enabled = false;
            }
        }

        private void UpdateCompareStartDate()
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = endDate.Date;
            switch ((PeriodType)(((ValueListItem)periodCombo.SelectedItem).DataValue))
            {
                case PeriodType.Today:
                    startDate = endDate.Date;
                    break;
                case PeriodType.Last7:
                    startDate = endDate.AddDays(-7);
                    break;
                case PeriodType.Last30:
                    startDate = endDate.AddDays(-30);
                    break;
                case PeriodType.Last365:
                    startDate = endDate.AddDays(-365);
                    break;
                case PeriodType.Custom:
                    startDate = customDates[0].UtcStart.ToLocalTime();
                    break;
            }
            compareDateStartPicker.Value = startDate;
        }

        private void UpdateCompareEndDate()
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = compareDateStartPicker.Value;

            switch ((PeriodType)(((ValueListItem)periodCombo.SelectedItem).DataValue))
            {
                case PeriodType.Today:
                    endDate = endDate.Date.Add(new TimeSpan(23, 59, 59));
                    break;
                case PeriodType.Last7:
                    endDate = startDate.AddDays(7);
                    break;
                case PeriodType.Last30:
                    endDate = startDate.AddDays(30);
                    break;
                case PeriodType.Last365:
                    endDate = startDate.AddDays(365);
                    break;
                case PeriodType.Custom:
                    Pair<DateTime, DateTime> range = base.GetSelectedRange();
                    TimeSpan span = range.Second - range.First;
                    endDate = startDate.AddDays(span.TotalDays);
                    Log.DebugFormat("UtcStart: {0} - UtcEnd: {1} - compareStartDateTimePicker: {2} - endDate:{3} - span: {4}", range.First, range.Second, compareDateStartPicker.Value, endDate, span.TotalDays);
                    break;
            }

            compareDatabaseEndDate.Text = endDate.ToString("d");
        }

        private void compareDateStartPicker_ValueChanged(object sender, EventArgs e)
        {
            UpdateCompareEndDate();
        }

        private void periodCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateCompareStartDate();
            UpdateCompareEndDate();

            if (periodCombo.SelectedItem == periodSetCustom)
            {
                startHoursLbl.Visible = true;
                startHoursTimeEditor.Visible = true;
                endHoursLbl.Visible = true;
                endHoursTimeEditor.Visible = true;

                this.SetStartHours();
                this.SetEndHours();
            }
            else
            {
                startHoursLbl.Visible = false;
                startHoursTimeEditor.Visible = false;
                endHoursLbl.Visible = false;
                endHoursTimeEditor.Visible = false;
            }
        }

        private void SetStartHours()
        {
            DateTime first = customDates[0].UtcStart.ToLocalTime();
            DateTime final = new DateTime(first.Year, first.Month, first.Day, startHoursTimeEditor.Time.Hours, startHoursTimeEditor.Time.Minutes, startHoursTimeEditor.Time.Seconds);
            customDates[0].UtcStart = final.ToUniversalTime();
        }

        private void SetEndHours()
        {
            DateTime last = customDates[customDates.Count - 1].UtcEnd.ToLocalTime();
            DateTime final = new DateTime(last.Year, last.Month, last.Day, endHoursTimeEditor.Time.Hours, endHoursTimeEditor.Time.Minutes, endHoursTimeEditor.Time.Seconds);
            customDates[customDates.Count - 1].UtcEnd = final.ToUniversalTime();
        }

        private void startHoursTimeEditor_ValueChanged(object sender, EventArgs e)
        {
            SetStartHours();
        }

        private void endHoursTimeEditor_ValueChanged(object sender, EventArgs e)
        {
            SetEndHours();
        }
    }
}