using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using BBS.TracerX;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;
using Microsoft.SqlServer.MessageBox;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class TopDatabases : ReportContol
    {

        private static readonly Logger Log = Logger.GetLogger("Top Databases");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>();

        public TopDatabases()
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

            return true;
        }

        public override void InitReport()
        {
            base.InitReport();
            InitializeReportViewer();
            State = UIState.ParmsNeeded;
            ReportType = ReportTypes.TopDatabases;
            numDatabases.Value = 5;
            minTransactions.Value = 0;
            minWritten.Value = 0;
            minSize.Value = 0;
            minGrowth.Value = 0;
            minReads.Value = 0;
            includeSystem.Checked = true;
        }

        protected override void SetReportParameters()
        {
            base.SetReportParameters();
            reportParameters.Add("dbName", String.IsNullOrEmpty(dbName.Text)?null:dbName.Text);
            reportParameters.Add("minSize", minSize.Text);
            reportParameters.Add("minGrowth", minGrowth.Text);
            reportParameters.Add("minTransactions", minTransactions.Text);
            reportParameters.Add("minReads", minReads.Text);
            reportParameters.Add("minWritten", minWritten.Text);
            reportParameters.Add("numDatabases", numDatabases.Text);
            reportParameters.Add("includeSystem", includeSystem.Checked);
            reportParameters.Add("TopN", numDatabases.Text.ToString());
            reportParameters.Add("Period", ((int)reportData.periodType).ToString());
            //Added a new parameter that is used with Query Waits section in Top Databases report. Aditya Shukla SQLdm 8.6
            reportParameters.Add("waitThreshold", waitThreshold.Text);

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
            reportParameters.Add("ServerId", reportData.instanceID.ToString());
            reportParameters.Add("Period", ((int)reportData.periodType).ToString());
            //reportParameters.Add("UTCOffset", reportData.dateRanges[0].UtcOffsetMinutes.ToString());
            reportParameters.Add("MinSizeMB", minSize.Text.ToString());
            reportParameters.Add("MinGrowthPercent", minGrowth.Text.ToString());
            reportParameters.Add("MinTransactionsPerSecond", minTransactions.Text.ToString());
            reportParameters.Add("MinReadsPerSecond", minReads.Text);
            reportParameters.Add("MinWritesPerSecond", minWritten.Text);
            reportParameters.Add("TopN", numDatabases.Text.ToString());

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

            if (String.IsNullOrEmpty(dbName.Text))
            {
                reportParameters.Add("DatabaseNameFilter", null);
            }
            else
            {
                reportParameters.Add("DatabaseNameFilter", dbName.Text);
            }
            reportParameters.Add("IncludeSystem", includeSystem.Checked.ToString());
            if (periodCombo.SelectedItem == periodSetCustom)
            {
                reportParameters.Add("rsStart",
                                     reportData.dateRanges[0].UtcStart.ToLocalTime().ToString(
                                         "yyyy'-'MM'-'dd HH':'mm':'ss"));
                reportParameters.Add("rsEnd",
                                     reportData.dateRanges[0].UtcEnd.ToLocalTime().ToString(
                                         "yyyy'-'MM'-'dd HH':'mm':'ss"));
            }
            return reportParameters;
        }

        override protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall())
            {
                WorkerData localReportData = (WorkerData)e.Argument;
                ReportDataSource dataSource;

                Log.Debug("localReportData.reportType = ", localReportData.reportType);

                passthroughParameters.Clear();
                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
                passthroughParameters.Add(new ReportParameter("TopN", (string)localReportData.reportParameters["TopN"]));
                passthroughParameters.Add(new ReportParameter("Period", (string)localReportData.reportParameters["Period"]));
                passthroughParameters.Add(new ReportParameter("GUIDateRange", GetDateRange(localReportData.dateRanges[0].UtcStart, localReportData.dateRanges[0].UtcEnd, (int)reportData.periodType)));
                //Added a new parameter called waitThreshold. It has value in millisecond. Aditya Shukla SQLdm 8.6
                passthroughParameters.Add(new ReportParameter("waitThreshold", (string)localReportData.reportParameters["waitThreshold"]));

                if (localReportData.reportParameters.ContainsKey("rsStart")) passthroughParameters.Add(new ReportParameter("rsStart",
                                                                  localReportData.reportParameters["rsStart"].ToString()));
                if (localReportData.reportParameters.ContainsKey("rsEnd")) passthroughParameters.Add(new ReportParameter("rsEnd",
                                                                  localReportData.reportParameters["rsEnd"].ToString()));

                // Increased the ReportDataSource size from 5 to 6 to accommodate a new data source for query waits. Aditya Shukla SQLdm 8.6
                localReportData.dataSources = new ReportDataSource[6];

                dataSource = new ReportDataSource("Size");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTopDatabasesBySize",
                                                                  localReportData.instanceID,
                                                                  (string)localReportData.reportParameters["dbName"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  localReportData.reportParameters["minSize"].ToString(),
                                                                  localReportData.reportParameters["minGrowth"].ToString(),
                                                                  localReportData.reportParameters["minTransactions"].ToString(),
                                                                  localReportData.reportParameters["minReads"].ToString(),
                                                                  localReportData.reportParameters["minWritten"].ToString(),
                                                                  localReportData.reportParameters["numDatabases"].ToString(),
                                                                  (bool)localReportData.reportParameters["includeSystem"]);
                localReportData.dataSources[0] = dataSource;

                dataSource = new ReportDataSource("Growth");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTopDatabasesByGrowth",
                                                                   localReportData.instanceID,
                                                                  (string)localReportData.reportParameters["dbName"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  localReportData.reportParameters["minSize"].ToString(),
                                                                  localReportData.reportParameters["minGrowth"].ToString(),
                                                                  localReportData.reportParameters["minTransactions"].ToString(),
                                                                  localReportData.reportParameters["minReads"].ToString(),
                                                                  localReportData.reportParameters["minWritten"].ToString(),
                                                                  localReportData.reportParameters["numDatabases"].ToString(),
                                                                  (bool)localReportData.reportParameters["includeSystem"]);
                localReportData.dataSources[1] = dataSource;

                dataSource = new ReportDataSource("Reads");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTopDatabasesByReads",
                                                                   localReportData.instanceID,
                                                                  (string)localReportData.reportParameters["dbName"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  localReportData.reportParameters["minSize"].ToString(),
                                                                  localReportData.reportParameters["minGrowth"].ToString(),
                                                                  localReportData.reportParameters["minTransactions"].ToString(),
                                                                  localReportData.reportParameters["minReads"].ToString(),
                                                                  localReportData.reportParameters["minWritten"].ToString(),
                                                                  localReportData.reportParameters["numDatabases"].ToString(),
                                                                  (bool)localReportData.reportParameters["includeSystem"]);
                localReportData.dataSources[2] = dataSource;

                dataSource = new ReportDataSource("Writes");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTopDatabasesByWrites",
                                                                   localReportData.instanceID,
                                                                  (string)localReportData.reportParameters["dbName"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  localReportData.reportParameters["minSize"].ToString(),
                                                                  localReportData.reportParameters["minGrowth"].ToString(),
                                                                  localReportData.reportParameters["minTransactions"].ToString(),
                                                                  localReportData.reportParameters["minReads"].ToString(),
                                                                  localReportData.reportParameters["minWritten"].ToString(),
                                                                  localReportData.reportParameters["numDatabases"].ToString(),
                                                                  (bool)localReportData.reportParameters["includeSystem"]);
                localReportData.dataSources[3] = dataSource;

                dataSource = new ReportDataSource("Transactions");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTopDatabasesByTransactions",
                                                                  localReportData.instanceID,
                                                                  (string)localReportData.reportParameters["dbName"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  localReportData.reportParameters["minSize"].ToString(),
                                                                  localReportData.reportParameters["minGrowth"].ToString(),
                                                                  localReportData.reportParameters["minTransactions"].ToString(),
                                                                  localReportData.reportParameters["minReads"].ToString(),
                                                                  localReportData.reportParameters["minWritten"].ToString(),
                                                                  localReportData.reportParameters["numDatabases"].ToString(),
                                                                  (bool)localReportData.reportParameters["includeSystem"]);
                localReportData.dataSources[4] = dataSource;

                // Added a new data source for Query Waits section in this report. Aditya Shukla SQLdm 8.6 START
                dataSource = new ReportDataSource("Waits");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTopDatabasesByQueryWaits",
                                                                  localReportData.instanceID,
                                                                  (string)localReportData.reportParameters["dbName"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  localReportData.reportParameters["minSize"].ToString(),
                                                                  localReportData.reportParameters["minGrowth"].ToString(),
                                                                  localReportData.reportParameters["minTransactions"].ToString(),
                                                                  localReportData.reportParameters["minReads"].ToString(),
                                                                  localReportData.reportParameters["minWritten"].ToString(),
                                                                  localReportData.reportParameters["numDatabases"].ToString(),
                                                                  (bool)localReportData.reportParameters["includeSystem"],
                                                                  (string)localReportData.reportParameters["waitThreshold"]);//Added this value to accommodate a new threshold parameter in the procedure 
                localReportData.dataSources[5] = dataSource;
                // Added a new data source for Query Waits section in this report. Aditya Shukla SQLdm 8.6 END
                passthroughParameters.Add(new ReportParameter("ServerId", localReportData.instanceID.ToString()));
                passthroughParameters.Add(new ReportParameter("UTCOffset", localReportData.dateRanges[0].UtcOffsetMinutes.ToString()));
                passthroughParameters.Add(new ReportParameter("GUIServer", (string)localReportData.reportParameters["GUIServer"]));
                passthroughParameters.Add(new ReportParameter("DatabaseNameFilter", (string)localReportData.reportParameters["dbName"]));

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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.TopDatabases.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }

                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "TopDatabases";
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
                Log.Debug("e.ReportPath = ", e.ReportPath);

                if (e.ReportPath == ReportsHelper.GetReportTitle(ReportTypes.DatabaseStatistics))
                {
                    ApplicationController.Default.ShowReportsView(ReportTypes.DatabaseStatistics, e);
                }
            }
        }

        public override void ResetFilterCriteria()
        {
            base.ResetFilterCriteria();
            dbName.Text = String.Empty;
            numDatabases.Text = "5";
            minTransactions.Text = "0";
            minWritten.Text = "0";
            minSize.Text = "0";
            minGrowth.Text = "0";
            minReads.Text = "0";
            includeSystem.Checked = true;
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

        private void numDatabases_Leave(object sender, EventArgs e)
        {
            if (this.numDatabases.Text.Equals(string.Empty))
            {
                this.numDatabases.Text = this.numDatabases.Minimum.ToString();
            }
        }

        private void minSize_Leave(object sender, EventArgs e)
        {
            if (this.minSize.Text.Equals(string.Empty))
            {
                this.minSize.Text = this.minSize.Minimum.ToString();
            }
        }

        private void minTransactions_Leave(object sender, EventArgs e)
        {
            if (this.minTransactions.Text.Equals(string.Empty))
            {
                this.minTransactions.Text = this.minTransactions.Minimum.ToString();
            }
        }

        private void minGrowth_Leave(object sender, EventArgs e)
        {
            if (this.minGrowth.Text.Equals(string.Empty))
            {
                this.minGrowth.Text = this.minGrowth.Minimum.ToString();
            }
        }

        private void minWritten_Leave(object sender, EventArgs e)
        {
            if (this.minWritten.Text.Equals(string.Empty))
            {
                this.minWritten.Text = this.minWritten.Minimum.ToString();
            }
        }

        private void minReads_Leave(object sender, EventArgs e)
        {
            if (this.minReads.Text.Equals(string.Empty))
            {
                this.minReads.Text = this.minReads.Minimum.ToString();
            }
        }

        // This method checks the input from the wait threshold numeric counter Aditya Shukla SQLdm 8.6 START
        private void waitThreshold_Leave(object sender, EventArgs e)
        {
            if (this.waitThreshold.Text.Equals(string.Empty))
            {
                this.waitThreshold.Text = this.waitThreshold.Minimum.ToString();
            }
        }
        // This method checks the input from the wait threshold numeric counter Aditya Shukla SQLdm 8.6 END

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

        private string GetDateRange(DateTime rsStart, DateTime rsEnd)
        {
            string dateRange = string.Empty;

            if (periodCombo.SelectedItem == periodSetCustom)
            {
                dateRange = rsStart.ToLocalTime() + " to " + rsEnd.ToLocalTime();
            }
            else
            {
                dateRange = rsStart.ToLocalTime().ToString("d") + " to " + rsEnd.ToLocalTime().ToString("d");
            }

            return dateRange;
        }
    }
}

