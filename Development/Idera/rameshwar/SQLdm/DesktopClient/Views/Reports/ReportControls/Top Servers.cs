using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using BBS.TracerX;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class TopServersReport : ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("Top Servers");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(1);

        public TopServersReport()
        {
            InitializeComponent();
            base.AdaptFontSize();
        }

        public override void InitReport()
        {
            base.InitReport();
            InitializeReportViewer();
            ReportType = ReportTypes.TopServers;
        }

        protected override void SetReportParameters()
        {
            base.SetReportParameters();
            reportParameters.Add("NumServers", numberOfServers.Text);
            reportParameters.Add("Period", ((int)reportData.periodType).ToString());
            //reportParameters.Add("SQLServerIDs", GetServerIdXml(reportData.serverIDs));
            //Added a new parameter that is used with Query Waits section in Top Servers report. Aditya Shukla SQLdm 8.6
            reportParameters.Add("waitThreshold", waitThreshold.Text);

            ValueListItem selectedTag = (ValueListItem)tagsComboBox.SelectedItem;

            if (selectedTag == null)
            {
                selectedTag = new ValueListItem(0, "All Tags");
            }
            else
            {
                if (tagsComboBox.SelectedIndex == 0) selectedTag = new ValueListItem(0, "All Tags");
                if ((int)selectedTag.DataValue == 0) selectedTag = new ValueListItem(0, "All Tags");
            }

            reportParameters.Add("GUITags", selectedTag.DisplayText);
            reportParameters.Add("Tags", selectedTag.DataValue.ToString());

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
            return reportParameters;
        }

        override protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall())
            {
                WorkerData localReportData = (WorkerData)e.Argument;
                ReportDataSource dataSource;
                IList<int> servers;

                if (localReportData.serverIDs != null)
                {
                    servers = localReportData.serverIDs;
                }
                else
                {
                    servers = new List<int>(ApplicationModel.Default.AllInstances.Count);

                    foreach (MonitoredSqlServer server in ApplicationModel.Default.AllInstances.Values)
                    {
                        servers.Add(server.Id);
                    }
                }
                Log.Debug("localReportData.reportType = ", localReportData.reportType);
                
                passthroughParameters.Clear();
                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
                passthroughParameters.Add(new ReportParameter("NumServers", (string)localReportData.reportParameters["NumServers"]));
                passthroughParameters.Add(new ReportParameter("Period", ((int)reportData.periodType).ToString()));
                passthroughParameters.Add(new ReportParameter("ServerXML", GetServerIdXml(localReportData.serverIDs)));
                passthroughParameters.Add(new ReportParameter("GUITags", (string)localReportData.reportParameters["GUITags"]));
                passthroughParameters.Add(new ReportParameter("GUIDateRange", GetDateRange(localReportData.dateRanges[0].UtcStart, localReportData.dateRanges[0].UtcEnd, (int)reportData.periodType)));
                //Added a new parameter called waitThreshold. It has value in millisecond. Aditya Shukla SQLdm 8.6
                passthroughParameters.Add(new ReportParameter("waitThreshold", (string)localReportData.reportParameters["waitThreshold"]));

                if (localReportData.reportParameters.ContainsKey("rsStart")) passthroughParameters.Add(new ReportParameter("rsStart",
                                                                  localReportData.reportParameters["rsStart"].ToString()));
                if (localReportData.reportParameters.ContainsKey("rsEnd")) passthroughParameters.Add(new ReportParameter("rsEnd",
                                                                  localReportData.reportParameters["rsEnd"].ToString()));
                
                // Increased the ReportDataSource size from 5 to 6 to accommodate a new data source for query waits. Aditya Shukla SQLdm 8.6
                localReportData.dataSources = new ReportDataSource[6];

                dataSource = new ReportDataSource("ResponseTime");
                dataSource.Value = RepositoryHelper.GetReportData("p_TopServersResponseTime", 
                                                                  (string)localReportData.reportParameters["NumServers"], 
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  GetServerIdXml(servers));
                localReportData.dataSources[0] = dataSource;

                dataSource = new ReportDataSource("CpuUsage");
                dataSource.Value = RepositoryHelper.GetReportData("p_TopServersCpu",
                                                                  (string)localReportData.reportParameters["NumServers"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  GetServerIdXml(servers));
                localReportData.dataSources[1] = dataSource;

                dataSource = new ReportDataSource("DiskUsage");
                dataSource.Value = RepositoryHelper.GetReportData("p_TopServersDisk",
                                                                  (string)localReportData.reportParameters["NumServers"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  GetServerIdXml(servers));
                localReportData.dataSources[2] = dataSource;

                dataSource = new ReportDataSource("MemoryUsage");
                dataSource.Value = RepositoryHelper.GetReportData("p_TopServersMemory",
                                                                  (string)localReportData.reportParameters["NumServers"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  GetServerIdXml(servers));
                localReportData.dataSources[3] = dataSource;

                dataSource = new ReportDataSource("AlertCount");
                dataSource.Value = RepositoryHelper.GetReportData("p_TopServersAlerts",
                                                                  (string)localReportData.reportParameters["NumServers"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  GetServerIdXml(servers));
                localReportData.dataSources[4] = dataSource;

                // Added a new data source for Query Waits section in this report. Aditya Shukla SQLdm 8.6 START
                dataSource = new ReportDataSource("Waits");
                dataSource.Value = RepositoryHelper.GetReportData("p_TopServersQueryWaits",
                                                                  (string)localReportData.reportParameters["NumServers"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  GetServerIdXml(servers),
                                                                  //Added the following parameter to accommodate a new threshold parameter in the procedure
                                                                  (string)localReportData.reportParameters["waitThreshold"]);
                localReportData.dataSources[5] = dataSource;
                // Added a new data source for Query Waits section in this report. Aditya Shukla SQLdm 8.6 END

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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.Top Servers.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }
                            
                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "TopServers";
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

                if (e.ReportPath == ReportsHelper.GetReportTitle(ReportTypes.ServerSummary))
                {
                    ApplicationController.Default.ShowReportsView(ReportTypes.ServerSummary, e);
                }
            }
        }

        public override void ResetFilterCriteria()
        {
            base.ResetFilterCriteria();
            numberOfServers.Text = "5";
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

        private void numberOfServers_Leave(object sender, EventArgs e)
        {
            if (this.numberOfServers.Text.Equals(string.Empty))
            {
                this.numberOfServers.Text = this.numberOfServers.Minimum.ToString();
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