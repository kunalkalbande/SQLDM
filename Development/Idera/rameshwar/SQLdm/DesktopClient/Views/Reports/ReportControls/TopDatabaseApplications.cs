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
    public partial class TopDatabaseApplications : ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("DatabaseGrowthForecast");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(2);

        public TopDatabaseApplications()
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
            ReportType = ReportTypes.TopDatabaseApps;
            dbName.Text = null;
            chartTypeCombo.SelectedItem = chartTypeCombo.Items[0];
            numStatements.Value = 5;
            minCPU.Value = 0;
            minWrites.Value = 0;
            minReads.Value = 0;
            
        }

        protected override void SetReportParameters()
        {
            base.SetReportParameters();
            reportParameters.Add("dbName", dbName.Text);
            reportParameters.Add("minReads", minReads.Text);
            reportParameters.Add("minWrites", minWrites.Text);
            reportParameters.Add("minCPU", minCPU.Text);
            reportParameters.Add("numStatements", numStatements.Text);
            reportParameters.Add("SQLServerID", reportData.instanceID.ToString());
            
            if (chartTypeCombo.SelectedItem != null)
            {
                reportParameters.Add("ChartType", chartTypeCombo.SelectedItem.ToString());
            }
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
            reportParameters.Add("SQLServerID", reportData.instanceID.ToString());
            reportParameters.Add("Period", ((int)reportData.periodType).ToString());
            //reportParameters.Add("UtcOffsetMinutes", reportData.dateRanges[0].UtcOffsetMinutes.ToString());
            reportParameters.Add("MinReads", minReads.Text.ToString());
            reportParameters.Add("MinWrites", minWrites.Text.ToString());
            reportParameters.Add("MinCPU", minCPU.Text.ToString());
            reportParameters.Add("TopN", numStatements.Text.ToString());
            reportParameters.Add("DatabaseName", dbName.Text);
            reportParameters.Add("ChartType", chartTypeCombo.SelectedItem.ToString());
            if (periodCombo.SelectedItem != periodSetCustom)
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
                passthroughParameters.Add(new ReportParameter("ChartType", (string)localReportData.reportParameters["ChartType"]));
                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
                passthroughParameters.Add(new ReportParameter("SQLServerID", (string)localReportData.reportParameters["SQLServerID"]));
                passthroughParameters.Add(new ReportParameter("Period", ((int)reportData.periodType).ToString()));
                passthroughParameters.Add(new ReportParameter("GUIServer", (string)localReportData.reportParameters["GUIServer"]));
                passthroughParameters.Add(new ReportParameter("GUIDateRange", GetDateRange(localReportData.dateRanges[0].UtcStart, localReportData.dateRanges[0].UtcEnd, (int)reportData.periodType)));
                
                if (localReportData.reportParameters.ContainsKey("rsStart")) passthroughParameters.Add(new ReportParameter("rsStart",
                                                                  localReportData.reportParameters["rsStart"].ToString()));
                if (localReportData.reportParameters.ContainsKey("rsEnd")) passthroughParameters.Add(new ReportParameter("rsEnd",
                                                                  localReportData.reportParameters["rsEnd"].ToString()));

                localReportData.dataSources = new ReportDataSource[3];

                dataSource = new ReportDataSource("CPU");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTopDatabaseApplications",
                                                                  localReportData.instanceID,
                                                                  (string)localReportData.reportParameters["dbName"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  localReportData.reportParameters["minReads"].ToString(),
                                                                  localReportData.reportParameters["minWrites"].ToString(),
                                                                  localReportData.reportParameters["minCPU"].ToString(),
                                                                  localReportData.reportParameters["numStatements"].ToString(),
                                                                  0);
                localReportData.dataSources[0] = dataSource;

                dataSource = new ReportDataSource("Reads");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTopDatabaseApplications",
                                                                  localReportData.instanceID,
                                                                  (string)localReportData.reportParameters["dbName"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  localReportData.reportParameters["minReads"].ToString(),
                                                                  localReportData.reportParameters["minWrites"].ToString(),
                                                                  localReportData.reportParameters["minCPU"].ToString(),
                                                                  localReportData.reportParameters["numStatements"].ToString(),
                                                                  1);
                localReportData.dataSources[1] = dataSource;

                dataSource = new ReportDataSource("Writes");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTopDatabaseApplications",
                                                                  localReportData.instanceID,
                                                                  (string)localReportData.reportParameters["dbName"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  localReportData.reportParameters["minReads"].ToString(),
                                                                  localReportData.reportParameters["minWrites"].ToString(),
                                                                  localReportData.reportParameters["minCPU"].ToString(),
                                                                  localReportData.reportParameters["numStatements"].ToString(),
                                                                  2);
                localReportData.dataSources[2] = dataSource;

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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.TopDatabaseApplications.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }

                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "TopDatabaseApplications";
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

        private void label4_Click(object sender, EventArgs e)
        {

        }

        public override void ResetFilterCriteria()
        {
            base.ResetFilterCriteria();
            dbName.Text = null;
            chartTypeCombo.SelectedItem = chartTypeCombo.Items[0];
            numStatements.Text = "5";
            minCPU.Text = "0";
            minWrites.Text = "0";
            minReads.Text = "0";
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

        private void numStatements_Leave(object sender, EventArgs e)
        {
            if (this.numStatements.Text.Equals(string.Empty))
            {
                this.numStatements.Text = this.numStatements.Minimum.ToString();
            }
        }

        private void minWrites_Leave(object sender, EventArgs e)
        {
            if (this.minWrites.Text.Equals(string.Empty))
            {
                this.minWrites.Text = this.minWrites.Minimum.ToString();
            }
        }

        private void minCPU_Leave(object sender, EventArgs e)
        {
            if (this.minCPU.Text.Equals(string.Empty))
            {
                this.minCPU.Text = this.minCPU.Minimum.ToString();
            }
        }

        private void minReads_Leave(object sender, EventArgs e)
        {
            if (this.minReads.Text.Equals(string.Empty))
            {
                this.minReads.Text = this.minReads.Minimum.ToString();
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

