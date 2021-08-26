using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Threading;
using BBS.TracerX;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;
using Microsoft.SqlServer.MessageBox;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class VirtualizationStatistics : ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("Virtualization Statistics");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(6);

        public VirtualizationStatistics()
        {
            InitializeComponent();
            AdaptFontSize();
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

        protected override void InitInstanceCombo()
        {
            instanceCombo.Items.Clear();
            ValueListItem[] instances = null;
            ValueListItem instance;
            int instanceCount;

            int i = 0;
            int serverID = -1;

            // if the instance combo is not displayed, don't do any work.
            if (instanceCombo.Visible == false)
            {
                return;
            }

            if (drillThroughArguments != null)
            {
                IList<ReportParameter> paramsList = ((LocalReport)drillThroughArguments.Report).OriginalParametersToDrillthrough;

                try
                {
                    serverID = Convert.ToInt32(paramsList[0].Values[0]);
                }
                catch
                {
                    serverID = -1;
                }
            }
            instanceSelectOne = new ValueListItem(null, "< Select a Server >");
            instanceCombo.Items.Add(instanceSelectOne);
            currentServer = null;

            if ((tagsComboBox.SelectedItem != null) && (tagsComboBox.SelectedItem != tagSelectOne))
            {
                instanceCount = GetInstanceCount((int)(((ValueListItem)tagsComboBox.SelectedItem).DataValue));
                instances = new ValueListItem[instanceCount];
                Tag tag = GetTag((int)(((ValueListItem)tagsComboBox.SelectedItem).DataValue));
                if (tag == null)
                {
                    tagsComboBox.Items.Remove(tagsComboBox.SelectedItem);
                    tagsComboBox.SelectedIndex = 0;
                    return;
                }
                MonitoredSqlServer server;
                i = 0;
                foreach (int id in tag.Instances)
                {
                    server = GetServer(id);

                    if (!server.IsVirtualized)
                        continue;

                    instance = new ValueListItem(server, server.InstanceName);

                    if ((serverID != -1) && (serverID == server.Id))
                    {
                        currentServer = server;
                    }
                    instances[i++] = instance;
                }
            }
            else
            {
                instanceCount = countVirtualInstances();
                instances = new ValueListItem[instanceCount];
                i = 0;
                foreach (MonitoredSqlServer server in ApplicationModel.Default.ActiveInstances)
                {
                    if (!server.IsVirtualized)
                        continue;

                    instance = new ValueListItem(server, server.InstanceName);

                    if ((serverID != -1) && (serverID == server.Id))
                    {
                        currentServer = server;
                    }
                    instances[i++] = instance;
                }
            }

            instanceCombo.Items.AddRange(instances);
            //now re-select the one they had selected, if they did have one
            if (instanceCombo.SelectedItem == null && currentServer != null)
            {
                instanceCombo.SelectedIndex = instanceCombo.FindStringExact(currentServer.InstanceName);
            }
            else
            {
                instanceCombo.SelectedIndex = 0;
            }
        }

        public override void InitReport()
        {
            base.InitReport();
            InitializeReportViewer();
            chartTypeCombo.SelectedItem = chartTypeCombo.Items[3];

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
            }

            ReportType = ReportTypes.VirtualizationStatistics;
            RunReport(true);
        }

        protected override void SetReportParameters()
        {
            base.SetReportParameters();

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
            
            reportParameters.Add("Period", ((int)reportData.periodType).ToString());
            reportParameters.Add("Interval", ((int)reportData.sampleSize).ToString());
            reportParameters.Add("UTCOffset", reportData.dateRanges[0].UtcOffsetMinutes.ToString());
            reportParameters.Add("SQLServerID", reportData.instanceID.ToString());
            reportParameters.Add("GUIServers", selectedInstance.DisplayText);

            if (periodCombo.SelectedItem != periodSetCustom) return;
            reportParameters.Add("rsStart", reportData.dateRanges[0].UtcStart.ToLocalTime().ToString("yyyy'-'MM'-'dd HH':'mm':'ss"));
            reportParameters.Add("rsEnd", reportData.dateRanges[0].UtcEnd.ToLocalTime().ToString("yyyy'-'MM'-'dd HH':'mm':'ss"));
        }

        public override Dictionary<string, object> GetReportParmeters()
        {
            SetReportParameters();
            //this is only for local use and it breaks the deployment wizard which only copes with strings
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
                localReportData.dataSources = new ReportDataSource[1];

                passthroughParameters.Add(new ReportParameter("GUIDateRange", GetDateRange(localReportData.dateRanges[0].UtcStart, localReportData.dateRanges[0].UtcEnd, (int)reportData.periodType)));

                dataSource = new ReportDataSource("VirtualizationStatistics");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetVirtualizationStatistics",
                                                                  localReportData.instanceID,
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  (int)localReportData.sampleSize);
                localReportData.dataSources[0] = dataSource;

                passthroughParameters.Add(new ReportParameter("GUIServers", (string)localReportData.reportParameters["GUIServers"]));
                passthroughParameters.Add(new ReportParameter("SQLServerID", localReportData.instanceID.ToString()));
                passthroughParameters.Add(new ReportParameter("UTCOffset", localReportData.dateRanges[0].UtcOffsetMinutes.ToString()));
                passthroughParameters.Add(new ReportParameter("Interval", ((int)localReportData.sampleSize).ToString()));
                passthroughParameters.Add(new ReportParameter("ChartParameter", (string)localReportData.reportParameters["ChartType"]));
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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.VirtualizationStatistics.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }

                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "Virtualization Statistics";
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
            chartTypeCombo.SelectedItem = chartTypeCombo.Items[3];
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

        private int countVirtualInstances()
        {
            int result = 0;
            foreach (MonitoredSqlServer server in ApplicationModel.Default.ActiveInstances)
            {
                if (server.IsVirtualized)
                    result++;
            }
            return result;
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
