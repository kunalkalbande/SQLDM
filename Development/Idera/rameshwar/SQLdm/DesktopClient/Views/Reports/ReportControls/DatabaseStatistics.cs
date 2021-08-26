using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;
using Microsoft.SqlServer.MessageBox;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class DatabaseStatistics : DatabaseReport
    {
        private static readonly Logger Log = Logger.GetLogger("Database Statistics");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(6);

        public DatabaseStatistics()
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

            if (Databases == null || Databases.Count == 0)
            {
                message = "A database must be selected to generate this report.";
                return false;
            }

            return true;
        }

        public override void InitReport()
        {
            base.InitReport();
            InitializeReportViewer();
            UpdateCompareStartDate();
            UpdateCompareEndDate();
            compareDateStartPicker.Enabled = false;
            compareDatabaseEndDate.Enabled = false;
            chartTypeCombo.SelectedItem = chartTypeCombo.Items[3];
            Databases = null;
            compareDatabase.Text = null;

            if (drillThroughArguments != null)
            {

                try
                {
                    chartTypeCombo.SelectedItem = Convert.ToString(drillthroughParams["ChartParameter"][0]);
                }
                catch
                {
                    chartTypeCombo.SelectedItem = chartTypeCombo.Items[3];
                }

                try
                {
                    Databases = new List<string>();
                    Databases.Add(Convert.ToString(drillthroughParams["DatabaseNameFilter"][0]));
                    databaseTextbox.Text = Convert.ToString(drillthroughParams["DatabaseNameFilter"][0]);
                }
                catch
                {
                    Databases = null;
                }


            }

            ReportType = ReportTypes.DatabaseStatistics;
            RunReport(true);
        }

        protected override void SetReportParameters()
        {
            base.SetReportParameters();

            //only for local use
            if (Databases != null && Databases.Count > 0)
                reportParameters.Add("Database", Databases[0]);
            else
                reportParameters.Add("Database", null);

            reportParameters.Add("CompareDatabaseNameFilter", compareDatabase.Text == "" ? "< No Comparison Database >" : compareDatabase.Text);
            //this is for local use
            reportParameters.Add("CompareDateStart", compareDateStartPicker.Value.ToUniversalTime());
            //This is for deployment
            reportParameters.Add("CompareStartRange", compareDateStartPicker.Value.ToUniversalTime().ToString("yyyy'-'MM'-'dd HH':'mm':'ss"));
            //This is for deployment
            if (chartTypeCombo.SelectedItem != null) reportParameters.Add("ChartParameter", chartTypeCombo.SelectedItem.ToString());
            //This is for local use
            if (chartTypeCombo.SelectedItem != null) reportParameters.Add("ChartType", chartTypeCombo.SelectedItem.ToString());

            ValueListItem selectedInstance = (ValueListItem)instanceCombo.SelectedItem;
            if (selectedInstance == null)
            {
                selectedInstance = new ValueListItem(0, "All Servers");
            }
            else
            {
                if (selectedInstance.DataValue == null) selectedInstance = new ValueListItem(0, "All Servers");
                if (instanceCombo.SelectedIndex == 0) selectedInstance = new ValueListItem(0, "All Servers");
            }

            reportParameters.Add("GUIServer", selectedInstance.DisplayText);
            reportParameters.Add("Period", ((int)reportData.periodType).ToString());
            reportParameters.Add("Interval", ((int)reportData.sampleSize).ToString());
            reportParameters.Add("UTCOffset", reportData.dateRanges[0].UtcOffsetMinutes.ToString());
            reportParameters.Add("ServerId", reportData.instanceID.ToString());

            if (Databases != null && Databases.Count > 0)
                reportParameters.Add("DatabaseNameFilter", Databases[0]);
            else
                reportParameters.Add("DatabaseNameFilter", null);

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
            SetReportParameters();
            //this is only for local use and it breaks the deployment wizard which only copes with strings
            if (reportParameters.ContainsKey("CompareDateStart")) reportParameters.Remove("CompareDateStart");
            if (reportParameters.ContainsKey("Database")) reportParameters.Remove("Database");
            if (reportParameters.ContainsKey("ChartType")) reportParameters.Remove("ChartType");
            if (reportParameters.ContainsKey("UTCOffset")) reportParameters.Remove("UTCOffset");

            return reportParameters;
        }

        override protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall())
            {
                WorkerData localReportData = (WorkerData)e.Argument;
                ReportDataSource dataSource;

                passthroughParameters.Clear();
                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));

                if (localReportData.reportParameters.ContainsKey("rsStart")) passthroughParameters.Add(new ReportParameter("rsStart",
                                                                  localReportData.reportParameters["rsStart"].ToString()));
                if (localReportData.reportParameters.ContainsKey("rsEnd")) passthroughParameters.Add(new ReportParameter("rsEnd",
                                                                  localReportData.reportParameters["rsEnd"].ToString()));


                Log.Debug("localReportData.reportType = ", localReportData.reportType);
                localReportData.dataSources = new ReportDataSource[2];

                dataSource = new ReportDataSource("DBOverall");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTopDatabases",
                                                                  localReportData.instanceID,
                                                                  (string)localReportData.reportParameters["Database"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  null,
                                                                  null,
                                                                  null,
                                                                  null,
                                                                  null,
                                                                  1,
                                                                  0,
                                                                  true);
                localReportData.dataSources[0] = dataSource;

                dataSource = new ReportDataSource("DBStatistics");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetDatabaseStatistics",
                                                                  localReportData.instanceID,
                                                                  (string)localReportData.reportParameters["Database"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  (string)localReportData.reportParameters["CompareDatabaseNameFilter"],
                                                                  (DateTime)localReportData.reportParameters["CompareDateStart"],
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  (int)localReportData.sampleSize);
                localReportData.dataSources[1] = dataSource;

                passthroughParameters.Add(new ReportParameter("ServerId", localReportData.instanceID.ToString()));
                passthroughParameters.Add(new ReportParameter("GUIServer", (string)localReportData.reportParameters["GUIServer"]));
                passthroughParameters.Add(new ReportParameter("DatabaseNameFilter", (string)localReportData.reportParameters["Database"]));
                //passthroughParameters.Add(new ReportParameter("UTCStart", localReportData.dateRanges[0].UtcStart.ToString()));
                //passthroughParameters.Add(new ReportParameter("UTCEnd", localReportData.dateRanges[0].UtcEnd.ToString()));
                passthroughParameters.Add(new ReportParameter("CompareDatabaseNameFilter", (string)localReportData.reportParameters["CompareDatabaseNameFilter"]));
                passthroughParameters.Add(new ReportParameter("UTCOffset", localReportData.dateRanges[0].UtcOffsetMinutes.ToString()));
                passthroughParameters.Add(new ReportParameter("Interval", ((int)localReportData.sampleSize).ToString()));
                passthroughParameters.Add(new ReportParameter("ChartParameter", (string)localReportData.reportParameters["ChartType"]));
                passthroughParameters.Add(new ReportParameter("CompareStartRange", localReportData.reportParameters["CompareDateStart"].ToString()));
                passthroughParameters.Add(new ReportParameter("Period", ((int)reportData.periodType).ToString()));

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
                        if (e.Error.GetType() == typeof(SqlException) &&
                            e.Error.Message.ToLower().Contains("msxmlsql.dll"))//
                        {
                            ApplicationMessageBox msgbox1 = new ApplicationMessageBox();
                            Exception msg = new Exception("An error occurred while retrieving data for the report.  It may be due to the problem described by the article available at http://support.microsoft.com/Default.aspx?kbid=918767", e.Error);
                            Log.Error("Showing message box: ", msg);
                            msgbox1.Message = msg;
                            msgbox1.SetCustomButtons("OK", "View Article");
                            msgbox1.Symbol = ExceptionMessageBoxSymbol.Error;
                            msgbox1.Show(this);
                            if (msgbox1.CustomDialogResult == ExceptionMessageBoxDialogResult.Button2)
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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.DatabaseStatistics.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }

                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "DatabaseStatistics";
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


        private void compareDatabaseBrowseButton_Click(object sender, EventArgs e)
        {
            if ((instanceCombo.SelectedItem != null) && (instanceCombo.SelectedItem != instanceSelectOne))
            {
                DatabaseBrowserDialog dlg =
                    new DatabaseBrowserDialog(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                              ((MonitoredSqlServer)((ValueListItem)instanceCombo.SelectedItem).DataValue).Id,
                                              ((MonitoredSqlServer)((ValueListItem)instanceCombo.SelectedItem).DataValue).InstanceName, multipleDatabasesAllowed, true,
                                              "Check one or more databases for the report.", "Select Databases");

                if (compareDatabase.Text != null)
                {
                    dlg.SelectedDatabase = compareDatabase.Text;
                }

                if (DialogResult.OK == dlg.ShowDialog(FindForm()))
                {
                    compareDatabase.Text = dlg.SelectedDatabase;
                    if (compareDatabase.Text != null)
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
            }
            else
            {
                ApplicationMessageBox.ShowInfo(this, "Select a server before selecting a database.");
            }
        }

        private void compareDateStartPicker_ValueChanged(object sender, EventArgs e)
        {
            UpdateCompareEndDate();

        }

        private void periodCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
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

            UpdateCompareStartDate();
            UpdateCompareEndDate();
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

        private void instanceCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            Databases = null;
            compareDatabase.Text = null;
        }

        private void chartTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        public override void ResetFilterCriteria()
        {
            base.ResetFilterCriteria();
            UpdateCompareStartDate();
            UpdateCompareEndDate();
            compareDateStartPicker.Enabled = false;
            compareDatabaseEndDate.Enabled = false;
            chartTypeCombo.SelectedItem = chartTypeCombo.Items[3];
            Databases = null;
            compareDatabase.Text = null;
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

        private void startHoursTimeEditor_ValueChanged(object sender, EventArgs e)
        {
            SetStartHours();
            SetStartCompareDates();
        }

        private void endHoursTimeEditor_ValueChanged(object sender, EventArgs e)
        {
            SetEndHours();
        }

        private void SetStartCompareDates()
        {
            compareDateStartPicker.Value = customDates[0].UtcStart.ToLocalTime();
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
    }
}

