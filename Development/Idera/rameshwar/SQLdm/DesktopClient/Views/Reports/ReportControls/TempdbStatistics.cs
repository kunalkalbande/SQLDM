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
    public partial class TempdbStatistics : ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("Tempdb Statistics");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(6);

        public TempdbStatistics()
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

            ReportType = ReportTypes.TempdbStatistics;
            RunReport(true);
        }

        /// <summary>
        /// Over-ride the base method to populate only servers that have SQL version > 2000.
        /// </summary>
        protected override void InitInstanceCombo()
        {
            instanceCombo.Items.Clear();
            ValueListItem[] instances = null;
            var nonSql2000Ids = new List<int>();
            ValueListItem instance;
            int instanceCount;

            int i = 0;
            int serverId = -1;

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
                    serverId = Convert.ToInt32(paramsList[0].Values[0]);
                }
                catch
                {
                    serverId = -1;
                }
            }

            instanceSelectOne = new ValueListItem(null, "< Select a Server >");
            instanceCombo.Items.Add(instanceSelectOne);
            currentServer = null;

            //if a tag has been selected, we must populate only the servers that are associated with that tag
            //This report does not have tags selection right now but the functioanlity is there if we decide to 
            //display the combo
            if ((tagsComboBox.SelectedItem != null) && (tagsComboBox.SelectedItem != tagSelectOne))
            {
                //Fetch all the data associated with the tag
                Tag tag = GetTag((int)(((ValueListItem)tagsComboBox.SelectedItem).DataValue));
                if (tag == null)
                {
                    tagsComboBox.Items.Remove(tagsComboBox.SelectedItem);
                    tagsComboBox.SelectedIndex = 0;
                    return;
                }

                //Populate the instances array with all servers associated with this tag
                //that are not SQL 2000 instances
                foreach (var id in tag.Instances)
                {
                    Common.ServerVersion serverVersion = null;
                    serverVersion = GetSqlVersion(id);
                    //if there is a version
                    if(serverVersion != null)
                    {
                        if(serverVersion.Major > 8)
                        {
                            nonSql2000Ids.Add(id);
                        }
                    }
                }

                instanceCount = nonSql2000Ids.Count;
                instances = new ValueListItem[instanceCount];

                //Populate the instances array with all servers associated with this tag
                foreach (int id in nonSql2000Ids)
                {
                    MonitoredSqlServer server = GetServer(id);

                    instance = new ValueListItem(server, server.InstanceName);

                    if ((serverId != -1) && (serverId == server.Id))
                    {
                        currentServer = server;
                    }
                    instances[i++] = instance;
                }
            }
            else
            {
                //Populate the instances array with all servers associated with this tag
                //that are not SQL 2000 instances


                foreach (var monitoredServer in ApplicationModel.Default.AllInstances.Values)
                {
                    Common.ServerVersion serverVersion = null;
                    serverVersion = GetSqlVersion(monitoredServer.Id);
                    
                    //if there is a version
                    if (serverVersion != null)
                    {
                        //greater than sql 2000
                        if (serverVersion.Major > 8)
                        {
                            nonSql2000Ids.Add(monitoredServer.Id);
                        }
                    }
                }

                instanceCount = nonSql2000Ids.Count;
                instances = new ValueListItem[instanceCount];

                foreach (var id in nonSql2000Ids)
                {
                    //get only the next non-sql 2000 instance
                    var server = ApplicationModel.Default.AllInstances[id];

                    instance = new ValueListItem(server, server.InstanceName);

                    //default the currently selected server
                    if ((serverId != -1) && (serverId == server.Id))
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

            reportParameters.Add("GUIServer", selectedInstance.DisplayText);
            reportParameters.Add("Period", ((int)reportData.periodType).ToString());
            reportParameters.Add("Interval", ((int)reportData.sampleSize).ToString());
            reportParameters.Add("UTCOffset", reportData.dateRanges[0].UtcOffsetMinutes.ToString());
            reportParameters.Add("ServerId", reportData.instanceID.ToString());

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
                localReportData.dataSources = new ReportDataSource[2];

                dataSource = new ReportDataSource("DBOverall");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTempdbOverall",
                                                                  localReportData.instanceID,
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

                dataSource = new ReportDataSource("TempDBStatistics");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTempdbStatistics",
                                                                  localReportData.instanceID,
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  (int)localReportData.sampleSize);
                localReportData.dataSources[1] = dataSource;

                passthroughParameters.Add(new ReportParameter("ServerId", localReportData.instanceID.ToString()));
                passthroughParameters.Add(new ReportParameter("GUIServer", (string)localReportData.reportParameters["GUIServer"]));
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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.TempdbStatistics.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }

                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "TempdbStatistics";
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
        }
    }
}
