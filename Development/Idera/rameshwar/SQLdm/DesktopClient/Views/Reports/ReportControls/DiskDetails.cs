using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml.Schema;
using Idera.SQLdm.Common.Objects;
using BBS.TracerX;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;
using Idera.SQLdm.DesktopClient.Helpers.MathLibrary;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    /// <summary>
    /// DiskDetails.cs provides methods for generating Desktop Client reports and SSRS deployed reports for DiskDetails.
    /// </summary>
    public partial class DiskDetails : ReportContol
    {

        #region Fields & Properties
        
        private const string NoDrivesAvailableText = "< No Drives Available >";
        private static readonly Logger Log = Logger.GetLogger("DiskDetails");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(7);
        private const string TagAllDrives = "< All Drives >";
        private const string GetDiskDrivesSP = "p_GetDiskDrives";
        private const string TagSelectADrive = "< Select a Drive >";
        private WorkerData localReportData = null;

        #endregion Fields & Properties

        private enum AxisType
        {
            Maximum,
            Average
        }
        
        #region Public Constructors

        public DiskDetails()
        {
            InitializeComponent();
            InitializeYaxis();
            base.AdaptFontSize();
        }

        private void InitializeYaxis()
        {
            cmbAxis.Items.Add(AxisType.Average);
            cmbAxis.Items.Add(AxisType.Maximum);
            cmbAxis.SelectedItem = AxisType.Maximum;
        }       

        #endregion Public Constructors

        #region Base Class Overrides

        public override void InitReport()
        {
            base.InitReport();
            InitializeReportViewer();
            State = UIState.ParmsNeeded;
            periodCombo.SelectedItem = periodToday;
            sampleSizeCombo.SelectedItem = sampleHours;
            driveNamesCombo.Items.Add(TagSelectADrive);
            driveNamesCombo.SelectedIndex = 0;
            ReportType = ReportTypes.DiskDetails;
        }

        public override bool CanRunReport(out string message)
        {
            message = String.Empty;

            if (instanceCombo.SelectedIndex == 0)
            {
                message = "A SQL Server instance must be selected to generate this report.";
                return false;
            }
            else if (driveNamesCombo.SelectedItem.ToString() == NoDrivesAvailableText)
            {
                message = "No drive information is available for the selected SQL Server. Select a different server to run this report.";
                return false;
            }

            this.SetReportParameters();

            return true;
        }

        /// <summary>
        /// SetReportParameters. Sets the parameters that will be sent to the report for an scheduled action. Also prepares the parameters to be consumed on the bgWorker_DoWork
        /// </summary>
        protected override void SetReportParameters()
        {
            base.SetReportParameters();

            Pair<DateTime, DateTime> range = base.GetSelectedRange();

            if (driveNamesCombo.SelectedIndex <= 0)
            {
                reportParameters.Add("DiskDrives", TagAllDrives);
            }
            else
            {
                reportParameters.Add("DiskDrives", driveNamesCombo.SelectedItem);
            }

            if (instanceCombo.SelectedItem == null)
            {
                reportParameters.Add("GUIServers", "");
            }
            else
            {
                reportParameters.Add("GUIServers", instanceCombo.SelectedItem.ToString());
                reportParameters.Add("serverName", (((ValueListItem)instanceCombo.SelectedItem).DataValue) == null ? -1 : ((MonitoredSqlServer)((ValueListItem)instanceCombo.SelectedItem).DataValue).Id);
            }

            reportParameters.Add("DisplayTables", showTablesCheckbox.Checked.ToString());
            reportParameters.Add("GUIDateRange", this.GetDateRange(reportData.dateRanges[0].UtcStart, reportData.dateRanges[0].UtcEnd));
            reportParameters.Add("GUIDrives", driveNamesCombo.SelectedItem == null ? "" : driveNamesCombo.SelectedItem.ToString());
            reportParameters.Add("rsStart", range.First);
            reportParameters.Add("rsEnd", range.Second);
            reportParameters.Add("rsStartHours", startHoursTimeEditor.Time.Hours);
            reportParameters.Add("rsEndHours", endHoursTimeEditor.Time.Hours);

            int average = 0;

            if (cmbAxis.SelectedItem is AxisType)
            {
                switch ((AxisType)cmbAxis.SelectedItem)
                {
                    case AxisType.Maximum:
                        average = 0;
                        break;
                    case AxisType.Average:
                        average = 1;
                        break;
                }
            }

            reportParameters.Add("Average", average);

            reportParameters.Add("Interval", ((int)reportData.sampleSize));
            reportParameters.Add("Period", ((int)reportData.periodType));
            reportParameters.Add("executionStart", DateTime.Now);
        }

        override protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall())
            {
                localReportData = (WorkerData)e.Argument;

                string diskDrives = (string)localReportData.reportParameters["DiskDrives"];
                diskDrives = (diskDrives == TagAllDrives) ? null : diskDrives;

                passthroughParameters.Clear();
                passthroughParameters.Add(new ReportParameter("executionStart", localReportData.reportParameters["executionStart"].ToString()));
                passthroughParameters.Add(new ReportParameter("Interval", ((int)reportData.sampleSize).ToString()));
                passthroughParameters.Add(new ReportParameter("DisplayTables", (string)localReportData.reportParameters["DisplayTables"]));
                passthroughParameters.Add(new ReportParameter("GUIServers", localReportData.reportParameters["GUIServers"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUIDateRange", localReportData.reportParameters["GUIDateRange"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUIDrives", localReportData.reportParameters["GUIDrives"].ToString()));
                passthroughParameters.Add(new ReportParameter("DiskDrives", diskDrives));

                Log.Debug("localReportData.reportType = ", localReportData.reportType);
                localReportData.dataSources = new ReportDataSource[1];

                ReportDataSource dataSource = new ReportDataSource("DiskUsage");

                int average = 0;

                if (cmbAxis.SelectedItem is AxisType)
                {
                    switch ((AxisType) cmbAxis.SelectedItem)
                    {
                        case AxisType.Maximum:
                            average = 0;
                            break;
                        case AxisType.Average:
                            average = 1;
                            break;

                    }
                }

                dataSource.Value = RepositoryHelper.GetReportData("p_DiskDetailsReport",
                                                                  localReportData.instanceID,
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  (int)localReportData.sampleSize,
                                                                  diskDrives,
                                                                  average
                                                                  );
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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.DiskDetails.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }

                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "DiskDetails";
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
            base.ResetFilterCriteria();
            sampleSizeCombo.SelectedIndex = sampleSizeCombo.Items.IndexOf(sampleHours);
            driveNamesCombo.Items.Clear();
            driveNamesCombo.Items.Add(TagSelectADrive);
            driveNamesCombo.SelectedIndex = 0;
            showTablesCheckbox.Checked = false;
            ResetTimeFilter();
        }


        #endregion Base Class Overrides




        #region Initializers



        #endregion Initializers




        #region Events

        private void instanceCombo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateDriveNames();
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

        #endregion Events





        #region Private Methods

        /// <summary>
        /// GetDateRange. Returns a string containing the date range
        /// </summary>
        /// <param name="rsStart"></param>
        /// <param name="rsEnd"></param>
        /// <returns></returns>
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

        /// <summary>
        /// UpdateDriveNames. Updates the Drive Names list
        /// </summary>
        private void UpdateDriveNames()
        {
            driveNamesCombo.Items.Clear();

            if (instanceCombo.SelectedIndex == 0)
            {
                driveNamesCombo.Items.Clear();
                driveNamesCombo.Items.Add(TagSelectADrive);
            }
            else
            {
                try
                {
                    DataTable diskUsage = RepositoryHelper.GetReportData(GetDiskDrivesSP, ((MonitoredSqlServer)((ValueListItem)instanceCombo.SelectedItem).DataValue).Id);

                    if (diskUsage.Rows.Count != 0)
                    {
                        driveNamesCombo.Items.Add(TagAllDrives);

                        foreach (DataRow row in diskUsage.Rows)
                        {
                            object value = row["DriveName"];

                            if (value != DBNull.Value)
                            {
                                driveNamesCombo.Items.Add(value);
                            }
                        }
                    }
                    else
                    {
                        driveNamesCombo.Items.Add(NoDrivesAvailableText);
                    }
                }
                catch
                {
                    driveNamesCombo.Items.Clear();
                    driveNamesCombo.Items.Add(TagSelectADrive);
                }
            }

            driveNamesCombo.SelectedIndex = 0;
        }

        /// <summary>
        /// SetStartHours. Adds custom hours to the customDates UtcStart field.
        /// </summary>
        private void SetStartHours()
        {
            DateTime first = customDates[0].UtcStart.ToLocalTime();
            DateTime final = new DateTime(first.Year, first.Month, first.Day, startHoursTimeEditor.Time.Hours, startHoursTimeEditor.Time.Minutes, startHoursTimeEditor.Time.Seconds);
            customDates[0].UtcStart = final.ToUniversalTime();
        }

        /// <summary>
        /// SetEndHours. Adds custom hours to the customDates UtcEnd field.
        /// </summary>
        private void SetEndHours()
        {
            DateTime last = customDates[customDates.Count - 1].UtcEnd.ToLocalTime();
            DateTime final = new DateTime(last.Year, last.Month, last.Day, endHoursTimeEditor.Time.Hours, endHoursTimeEditor.Time.Minutes, endHoursTimeEditor.Time.Seconds);
            customDates[customDates.Count - 1].UtcEnd = final.ToUniversalTime();
        }

        #endregion Private Methods




        #region Public Methods



        #endregion Public Methods

    }
}
