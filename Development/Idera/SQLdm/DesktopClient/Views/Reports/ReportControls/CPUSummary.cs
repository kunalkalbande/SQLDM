using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
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

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class CPUSummary : Idera.SQLdm.DesktopClient.Views.Reports.ReportControls.ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("CPU Summary");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(2);

        public CPUSummary()
        {
            InitializeComponent();
            base.AdaptFontSize();
        }

        public override void InitReport()
        {
            base.InitReport();
            InitializeReportViewer();
            ReportType = ReportTypes.CPUSummary;
            RunReport(true);
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

        protected override void SetReportParameters()
        {
            base.SetReportParameters();

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

            //reportParameters.Add("ServerID", reportData.instanceID.ToString());
            reportParameters.Add("Period", ((int)reportData.periodType).ToString());
            reportParameters.Add("UTCOffset", reportData.dateRanges[0].UtcOffsetMinutes.ToString());
            reportParameters.Add("Interval", ((int)reportData.sampleSize).ToString());
            reportParameters.Add("DisplayTables", showTablesCheckbox.Checked.ToString());
            reportParameters.Add("ShowBaseline", showBaselineCheckBox.Checked.ToString());
            
            //reportParameters.Add("servername", instanceCombo.SelectedItem == null ? "" : instanceCombo.SelectedItem.ToString());

            if (selectedInstance.DataValue is MonitoredSqlServer) reportParameters.Add("GUIServerID", ((MonitoredSqlServer)selectedInstance.DataValue).Id.ToString());

            if (periodCombo.SelectedItem != periodSetCustom) return;

            reportParameters.Add("rsStart", reportData.dateRanges[0].UtcStart.ToLocalTime().ToString("yyyy'-'MM'-'dd HH':'mm':'ss"));
            reportParameters.Add("rsEnd", reportData.dateRanges[0].UtcEnd.ToLocalTime().ToString("yyyy'-'MM'-'dd HH':'mm':'ss"));
        }

        public override Dictionary<string, object> GetReportParmeters()
        {
            SetReportParameters();
            reportParameters.Remove("UTCOffset");
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

                passthroughParameters.Clear();
                passthroughParameters.Add(new ReportParameter("Interval", ((int)reportData.sampleSize).ToString()));
                passthroughParameters.Add(new ReportParameter("Period", ((int)reportData.periodType).ToString()));
                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
                passthroughParameters.Add(new ReportParameter("DisplayTables", (string)localReportData.reportParameters["DisplayTables"]));
                passthroughParameters.Add(new ReportParameter("ShowBaseline", (string)localReportData.reportParameters["ShowBaseline"]));
                
                if (localReportData.reportParameters.ContainsKey("rsStart")) passthroughParameters.Add(new ReportParameter("rsStart",
                                                                   localReportData.reportParameters["rsStart"].ToString()));
                if (localReportData.reportParameters.ContainsKey("rsEnd")) passthroughParameters.Add(new ReportParameter("rsEnd",
                                                                   localReportData.reportParameters["rsEnd"].ToString()));

                Log.Debug("localReportData.reportType = ", localReportData.reportType);

                localReportData.dataSources = new ReportDataSource[3];

                dataSource = new ReportDataSource("CPUSummary");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetCPUSummary",
                                                                  localReportData.instanceID,
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes,
                                                                  (int)localReportData.sampleSize);
                localReportData.dataSources[0] = dataSource;

                dataSource = new ReportDataSource("p_GetOSAvailable");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetOSStatisticsAvailable",
                                                                  localReportData.instanceID);
                localReportData.dataSources[1] = dataSource;

                dataSource = new ReportDataSource("CPUSummaryBaseline");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetCPUSummaryBaseline",
                                                                  localReportData.instanceID,
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes,
                                                                  (int)localReportData.sampleSize);

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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.CPUSummary.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }
                            reportViewer.LocalReport.SetParameters(passthroughParameters);
                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "CPU Summary";
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

        public override void ResetFilterCriteria()
        {
            base.ResetFilterCriteria(tagSelectOne, instanceSelectOne, sampleSizeCombo.Items.IndexOf(sampleMinutes), 0);
            showTablesCheckbox.Checked = false;
            showBaselineCheckBox.Checked = false;
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

        private void periodCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (periodCombo.SelectedItem == periodSetCustom)
            {
                startHoursLbl.Visible = true;
                startHoursTimeEditor.Visible = true;
                endHoursLbl.Visible = true;
                endHoursTimeEditor.Visible = true;
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
            DateTime first = customDates[0].UtcStart.ToLocalTime();
            DateTime final = new DateTime(first.Year, first.Month, first.Day, startHoursTimeEditor.Time.Hours, startHoursTimeEditor.Time.Minutes, startHoursTimeEditor.Time.Seconds);
            customDates[0].UtcStart = final.ToUniversalTime();
        }

        private void endHoursTimeEditor_ValueChanged(object sender, EventArgs e)
        {
            DateTime last = customDates[customDates.Count - 1].UtcEnd.ToLocalTime();
            DateTime final = new DateTime(last.Year, last.Month, last.Day, endHoursTimeEditor.Time.Hours, endHoursTimeEditor.Time.Minutes, endHoursTimeEditor.Time.Seconds);
            customDates[customDates.Count - 1].UtcEnd = final.ToUniversalTime();
        }

        private void showBaselineCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            SetReportParameters();
        }
    }
}
