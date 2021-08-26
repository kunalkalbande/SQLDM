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
    public partial class ServerSummary : Idera.SQLdm.DesktopClient.Views.Reports.ReportControls.ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("Server Summary");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(2);

        public ServerSummary()
        {
            InitializeComponent();
            base.AdaptFontSize();
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
                instanceCount = ApplicationModel.Default.ActiveInstances.Count;
                instances = new ValueListItem[instanceCount];
                i = 0;
                foreach (MonitoredSqlServer server in ApplicationModel.Default.ActiveInstances)
                {
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
            sampleSizeCombo.Visible = false;
            sampleLabel.Visible = false;
            periodLabel.Visible = false;
            periodCombo.Visible = false;
            tagsComboBox.Visible = false;
            tagsLabel.Visible = false;
            ReportType = ReportTypes.ServerSummary;
            RunReport(true);
        }

        public override void ResetFilterCriteria()
        {
            base.ResetFilterCriteria();
            startTimePicker.Value = new DateTime(2009, 1, 1, 8, 0, 0, 0);
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

            DateTime localStartTime = new DateTime(DateTime.Now.Year,
                          DateTime.Now.Month,
                          DateTime.Now.Day,
                          startTimePicker.Value.Hour,
                          startTimePicker.Value.Minute,
                          startTimePicker.Value.Second);
            
            DateTime startTime = localStartTime.ToUniversalTime();
            
            reportParameters.Add("startTime", startTime);
            reportParameters.Add("localStartTime", localStartTime.ToLongTimeString());
            reportParameters.Add("serverName", instanceCombo.SelectedItem == null ? "" : instanceCombo.SelectedItem.ToString());
        }

        /// <summary>
        /// This method is returning the report parameters.  The names must match what is set in the RDL file.
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, object> GetReportParmeters()
        {
            reportParameters.Clear();
            
            DateTime startTime = new DateTime(DateTime.Now.Year,
                          DateTime.Now.Month,
                          DateTime.Now.Day,
                          startTimePicker.Value.Hour,
                          startTimePicker.Value.Minute,
                          startTimePicker.Value.Second);
            
            //startTime = startTime.ToUniversalTime();

            reportParameters.Add("ServerId", reportData.instanceID.ToString());
            reportParameters.Add("SQLServerIDs", GetServerIdXml(reportData.instanceID));
            //reportParameters.Add("UTCOffset", reportData.dateRanges[0].UtcOffsetMinutes.ToString());
            
            //reportParameters.Add("StartTime", startTime.ToString("yyyy'-'MM'-'dd HH':'mm':'ss"));
            reportParameters.Add("StartTimeOfDay", startTime.ToString("HH:mm:ss"));
            
            ValueListItem selectedInstance = (ValueListItem)instanceCombo.SelectedItem ??
                                             new ValueListItem(0, "All Servers");
            if (instanceCombo.SelectedIndex == 0) selectedInstance = new ValueListItem(0, "All Servers");
            if (instanceCombo.SelectedIndex > 0)
                reportParameters.Add("GUIServerID", ((MonitoredSqlServer) selectedInstance.DataValue).Id.ToString());
            
            return reportParameters;
        }

        override protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall())
            {
                WorkerData localReportData = (WorkerData)e.Argument;

                Log.Debug("localReportData.reportType = ", localReportData.reportType);
                localReportData.dataSources = new ReportDataSource[3];

                passthroughParameters.Clear();

                passthroughParameters.Add(new ReportParameter("StartTimeOfDay", (string)localReportData.reportParameters["localStartTime"]));
                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
                passthroughParameters.Add(new ReportParameter("serverName", (string)localReportData.reportParameters["serverName"]));

                ReportDataSource dataSource = new ReportDataSource("ServerSummary");
                if (localReportData.serverIDs != null)
                {
                    dataSource.Value =
                        RepositoryHelper.GetReportData("p_GetEnterpriseSummary",
                                                       GetServerIdXml(localReportData.serverIDs));
                }
                else
                {
                    dataSource.Value = RepositoryHelper.GetReportData("p_GetEnterpriseSummary", GetServerIdXml(localReportData.instanceID), localReportData.dateRanges[0].UtcOffsetMinutes.ToString());
                }
                localReportData.dataSources[0] = dataSource;

                dataSource = new ReportDataSource("DatabaseInfo");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetDatabaseOverview", localReportData.instanceID);
                localReportData.dataSources[1] = dataSource;

                dataSource = new ReportDataSource("ServerSummaryCharts");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetServerSummaryReport", 
                                                                  localReportData.instanceID, 
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(), 
                                                                  (DateTime)localReportData.reportParameters["startTime"],
                                                                  DateTime.Now.ToUniversalTime());
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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.ServerSummary.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }
                            reportViewer.LocalReport.SetParameters(passthroughParameters);
                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "ServerSummary";
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

                if (e.ReportPath == ReportsHelper.GetReportTitle(ReportTypes.ServerStatistics))
                {
                    ApplicationController.Default.ShowReportsView(ReportTypes.ServerStatistics, e);
                }
                else if (e.ReportPath == ReportsHelper.GetReportTitle(ReportTypes.ActiveAlerts))
                {
                    ApplicationController.Default.ShowReportsView(ReportTypes.ActiveAlerts, e);
                }
            }
        }
    }
}

