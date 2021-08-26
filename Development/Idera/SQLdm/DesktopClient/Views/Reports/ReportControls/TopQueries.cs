using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using BBS.TracerX;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;
using Microsoft.SqlServer.MessageBox;
using TraceLevel=BBS.TracerX.TraceLevel;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Dialogs;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class TopQueries : ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("Top Queries");
        private const int topNDefault = 5;
        private bool isHyperLinkActivated = true;
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>();

        private TopQueriesAdvancedFilter advancedFilter = new TopQueriesAdvancedFilter();

        public TopQueries()
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

            if (!showStoredProcedures.Checked &&
                !showSQLStatements.Checked &&
                !showBatches.Checked)
            {
                message = "At least one event type must be selected to generate this report.";
                return false;
            }

            if (numStatements.Value < 1)
            {
                message = "The number of statements must be greater than one to generate this report.";
                return false;
            }

            return true;
        }

        public override void InitReport()
        {
            base.InitReport();
            InitializeReportViewer();

            numStatements.Value = topNDefault;
            showStoredProcedures.Checked = true;
            showSQLStatements.Checked = true;
            showBatches.Checked = true;
            minExecutions.Value = 0;
            minDuration.Value = 0;
            minReads.Value = 0;
            minWrites.Value = 0;
            sigMode.Checked = true;
            caseInsensitive.Checked = false;
            advancedFilter.ClearValues();

            if (drillThroughArguments != null)
            {
                IList<ReportParameter> paramsList = ((LocalReport)drillThroughArguments.Report).OriginalParametersToDrillthrough;
                Dictionary<string, StringCollection> paramsDictionary = new Dictionary<string, StringCollection>();
                foreach(ReportParameter p in paramsList)
                {
                    paramsDictionary.Add(p.Name,p.Values);    
                }

                try
                {
                    showStoredProcedures.Checked = Convert.ToBoolean(paramsDictionary["IncludeStoredProcedures"][0]);
                }
                catch
                {
                    showStoredProcedures.Checked = true;
                }


                try
                {
                    showSQLStatements.Checked = Convert.ToBoolean(paramsDictionary["IncludeSQLStatements"][0]);
                }
                catch
                {
                    showSQLStatements.Checked = true;
                }


                try
                {
                    showBatches.Checked = Convert.ToBoolean(paramsDictionary["IncludeSQLBatches"][0]);
                }
                catch
                {
                    showBatches.Checked = true;
                }


                try
                {
                    minExecutions.Value = Convert.ToInt32(paramsDictionary["MinExecutions"][0]);
                }
                catch
                {
                    minExecutions.Value = 0;
                }

                try
                {
                    minDuration.Value = Convert.ToInt32(paramsDictionary["MinDuration"][0]);
                }
                catch
                {
                    minDuration.Value = 0;
                }

                try
                {
                    minReads.Value = Convert.ToInt32(paramsDictionary["MinReads"][0]);
                }
                catch
                {
                    minReads.Value = 0;
                }

                try
                {
                    minWrites.Value = Convert.ToInt32(paramsDictionary["MinWrites"][0]);
                }
                catch
                {
                    minWrites.Value = 0;
                }

                try
                {
                    numStatements.Value = Convert.ToInt32(paramsDictionary["TopN"][0]);
                }
                catch
                {
                    numStatements.Value = topNDefault;
                }

                try
                {
                    sigMode.Checked = Convert.ToBoolean(paramsDictionary["SignatureMode"][0]);
                }
                catch
                {
                    sigMode.Checked = false;
                }

                try
                {
                    caseInsensitive.Checked = Convert.ToBoolean(paramsDictionary["CaseInsensitive"][0]);
                }
                catch
                {
                    caseInsensitive.Checked = false;
                }
            }

            ReportType = ReportTypes.TopQueries;
            RunReport(true);
        }

        protected override void SetReportParameters()
        {
            base.SetReportParameters();
            reportParameters.Add("SQLServerID", reportData.instanceID.ToString());
            reportParameters.Add("showStoredProcedures", showStoredProcedures.Checked);
            reportParameters.Add("showSQLStatements", showSQLStatements.Checked);
            reportParameters.Add("showBatches", showBatches.Checked);
            reportParameters.Add("minExecutions", minExecutions.Text);
            reportParameters.Add("minDuration", minDuration.Text);
            reportParameters.Add("minReads", minReads.Text);
            reportParameters.Add("minWrites", minWrites.Text);
            reportParameters.Add("minCPU", minCPU.Text);
            reportParameters.Add("TopN", numStatements.Text);
            reportParameters.Add("sigMode", sigMode.Checked);
            reportParameters.Add("caseInsensitive", caseInsensitive.Checked);
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
            reportParameters.Add("ApplLike", advancedFilter.ApplicationLike);
            reportParameters.Add("ApplNotLike", advancedFilter.ApplicationNotLike);
            reportParameters.Add("ClientLike", advancedFilter.ClientLike);
            reportParameters.Add("ClientNotLike", advancedFilter.ClientNotLike);
            reportParameters.Add("DatabaseLike", advancedFilter.DatabaseLike);
            reportParameters.Add("DatabaseNotLike", advancedFilter.DatabaseNotLike);
            reportParameters.Add("SQLTextLike", advancedFilter.SQLTextLike);
            reportParameters.Add("SQLTextNotLike", advancedFilter.SQLTextNotLike);
            reportParameters.Add("UserLike", advancedFilter.UserLike);
            reportParameters.Add("UserNotLike", advancedFilter.UserNotLike);
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
            //reportParameters.Add("StartDateTime", reportData.dateRanges[0].UtcStart.ToString(DateTimeFormatInfo.InvariantInfo));
            //reportParameters.Add("EndDateTime", reportData.dateRanges[0].UtcEnd.ToString(DateTimeFormatInfo.InvariantInfo));
            //reportParameters.Add("UtcOffsetMinutes", reportData.dateRanges[0].UtcOffsetMinutes.ToString());
            reportParameters.Add("IncludeStoredProcedures", showStoredProcedures.Checked.ToString());
            reportParameters.Add("IncludeSQLStatements", showSQLStatements.Checked.ToString());
            reportParameters.Add("IncludeSQLBatches", showBatches.Checked.ToString());
            reportParameters.Add("MinExecutions", minExecutions.Text.ToString());
            reportParameters.Add("MinDuration", minDuration.Text.ToString());
            reportParameters.Add("MinReads", minReads.Text.ToString());
            reportParameters.Add("MinWrites", minWrites.Text.ToString());
            reportParameters.Add("MinCPU", minCPU.Text.ToString());
            reportParameters.Add("TopN", numStatements.Text.ToString());
            reportParameters.Add("SignatureMode", sigMode.Checked.ToString());
            reportParameters.Add("CaseInsensitive", caseInsensitive.Checked.ToString());

            if (periodCombo.SelectedItem == periodSetCustom)
            {
                reportParameters.Add("rsStart",
                                     reportData.dateRanges[0].UtcStart.ToLocalTime().ToString(
                                         "yyyy'-'MM'-'dd HH':'mm':'ss"));
                reportParameters.Add("rsEnd",
                                     reportData.dateRanges[0].UtcEnd.ToLocalTime().ToString(
                                         "yyyy'-'MM'-'dd HH':'mm':'ss"));
            }
            reportParameters.Add("ApplLike", advancedFilter.ApplicationLike);
            reportParameters.Add("ApplNotLike", advancedFilter.ApplicationNotLike);
            reportParameters.Add("ClientLike", advancedFilter.ClientLike);
            reportParameters.Add("ClientNotLike", advancedFilter.ClientNotLike);
            reportParameters.Add("DatabaseLike", advancedFilter.DatabaseLike);
            reportParameters.Add("DatabaseNotLike", advancedFilter.DatabaseNotLike);
            reportParameters.Add("SQLTextLike", advancedFilter.SQLTextLike);
            reportParameters.Add("SQLTextNotLike", advancedFilter.SQLTextNotLike);
            reportParameters.Add("UserLike", advancedFilter.UserLike);
            reportParameters.Add("UserNotLike", advancedFilter.UserNotLike);

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
                isHyperLinkActivated = true;
                passthroughParameters.Clear();
                passthroughParameters.Add(new ReportParameter("isDeployedReport", "false"));
                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
                passthroughParameters.Add(new ReportParameter("GUIServer", (string)localReportData.reportParameters["GUIServer"]));
                passthroughParameters.Add(new ReportParameter("IncludeStoredProcedures", (string)localReportData.reportParameters["showStoredProcedures"].ToString()));
                passthroughParameters.Add(new ReportParameter("IncludeSQLStatements", (string)localReportData.reportParameters["showSQLStatements"].ToString()));
                passthroughParameters.Add(new ReportParameter("IncludeSQLBatches", (string)localReportData.reportParameters["showBatches"].ToString()));
                passthroughParameters.Add(new ReportParameter("SQLServerID", reportData.instanceID.ToString()));
                passthroughParameters.Add(new ReportParameter("ApplLike", advancedFilter.ApplicationLike));
                passthroughParameters.Add(new ReportParameter("ApplNotLike", advancedFilter.ApplicationNotLike));
                passthroughParameters.Add(new ReportParameter("ClientLike", advancedFilter.ClientLike));
                passthroughParameters.Add(new ReportParameter("ClientNotLike", advancedFilter.ClientNotLike));
                passthroughParameters.Add(new ReportParameter("DatabaseLike", advancedFilter.DatabaseLike));
                passthroughParameters.Add(new ReportParameter("DatabaseNotLike", advancedFilter.DatabaseNotLike));
                passthroughParameters.Add(new ReportParameter("SQLTextLike", advancedFilter.SQLTextLike));
                passthroughParameters.Add(new ReportParameter("SQLTextNotLike", advancedFilter.SQLTextNotLike));
                passthroughParameters.Add(new ReportParameter("UserLike", advancedFilter.UserLike));
                passthroughParameters.Add(new ReportParameter("UserNotLike", advancedFilter.UserNotLike));
                passthroughParameters.Add(new ReportParameter("IsHyperlinksEnabled", isHyperLinkActivated.ToString()));

                passthroughParameters.Add(new ReportParameter("Period", ((int)reportData.periodType).ToString()));

                passthroughParameters.Add(new ReportParameter("GUIDateRange", GetDateRange(localReportData.dateRanges[0].UtcStart, 
                    localReportData.dateRanges[0].UtcEnd, (int)reportData.periodType)));
                if (localReportData.reportParameters.ContainsKey("rsStart")) passthroughParameters.Add(new ReportParameter("rsStart",
                                                                                  localReportData.reportParameters["rsStart"].ToString()));
                if (localReportData.reportParameters.ContainsKey("rsEnd")) passthroughParameters.Add(new ReportParameter("rsEnd",
                                                                  localReportData.reportParameters["rsEnd"].ToString()));

                localReportData.dataSources = new ReportDataSource[5];

                dataSource = new ReportDataSource("Frequency");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTopQueriesByFrequency",
                                                                  localReportData.instanceID,
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  (bool)localReportData.reportParameters["showStoredProcedures"],
                                                                  (bool)localReportData.reportParameters["showSQLStatements"],
                                                                  (bool)localReportData.reportParameters["showBatches"],
                                                                  localReportData.reportParameters["minExecutions"].ToString(),
                                                                  localReportData.reportParameters["minDuration"].ToString(),
                                                                  localReportData.reportParameters["minReads"].ToString(),
                                                                  localReportData.reportParameters["minWrites"].ToString(),
                                                                  localReportData.reportParameters["minCPU"].ToString(),
                                                                  localReportData.reportParameters["TopN"].ToString(),
                                                                  (bool)localReportData.reportParameters["sigMode"],
                                                                  (bool)localReportData.reportParameters["caseInsensitive"],
                                                                  advancedFilter.ApplicationLike,
                                                                  advancedFilter.ApplicationNotLike,
                                                                  advancedFilter.ClientLike,
                                                                  advancedFilter.ClientNotLike,
                                                                  advancedFilter.DatabaseLike,
                                                                  advancedFilter.DatabaseNotLike,
                                                                  advancedFilter.SQLTextLike,
                                                                  advancedFilter.SQLTextNotLike,
                                                                  advancedFilter.UserLike,
                                                                  advancedFilter.UserNotLike);
                localReportData.dataSources[0] = dataSource;

                dataSource = new ReportDataSource("Duration");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTopQueriesByDuration",
                                                                  localReportData.instanceID,
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                   (bool)localReportData.reportParameters["showStoredProcedures"],
                                                                  (bool)localReportData.reportParameters["showSQLStatements"],
                                                                  (bool)localReportData.reportParameters["showBatches"],
                                                                  localReportData.reportParameters["minExecutions"].ToString(),
                                                                  localReportData.reportParameters["minDuration"].ToString(),
                                                                  localReportData.reportParameters["minReads"].ToString(),
                                                                  localReportData.reportParameters["minWrites"].ToString(),
                                                                  localReportData.reportParameters["minCPU"].ToString(),
                                                                  localReportData.reportParameters["TopN"].ToString(),
                                                                  (bool)localReportData.reportParameters["sigMode"],
                                                                  (bool)localReportData.reportParameters["caseInsensitive"],
                                                                  advancedFilter.ApplicationLike,
                                                                  advancedFilter.ApplicationNotLike,
                                                                  advancedFilter.ClientLike,
                                                                  advancedFilter.ClientNotLike,
                                                                  advancedFilter.DatabaseLike,
                                                                  advancedFilter.DatabaseNotLike,
                                                                  advancedFilter.SQLTextLike,
                                                                  advancedFilter.SQLTextNotLike,
                                                                  advancedFilter.UserLike,
                                                                  advancedFilter.UserNotLike);
                localReportData.dataSources[1] = dataSource;

                dataSource = new ReportDataSource("CPU");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTopQueriesByCPU",
                                                                  localReportData.instanceID,
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  (bool)localReportData.reportParameters["showStoredProcedures"],
                                                                  (bool)localReportData.reportParameters["showSQLStatements"],
                                                                  (bool)localReportData.reportParameters["showBatches"],
                                                                  localReportData.reportParameters["minExecutions"].ToString(),
                                                                  localReportData.reportParameters["minDuration"].ToString(),
                                                                  localReportData.reportParameters["minReads"].ToString(),
                                                                  localReportData.reportParameters["minWrites"].ToString(),
                                                                  localReportData.reportParameters["minCPU"].ToString(),
                                                                  localReportData.reportParameters["TopN"].ToString(),
                                                                  (bool)localReportData.reportParameters["sigMode"],
                                                                  (bool)localReportData.reportParameters["caseInsensitive"],
                                                                  advancedFilter.ApplicationLike,
                                                                  advancedFilter.ApplicationNotLike,
                                                                  advancedFilter.ClientLike,
                                                                  advancedFilter.ClientNotLike,
                                                                  advancedFilter.DatabaseLike,
                                                                  advancedFilter.DatabaseNotLike,
                                                                  advancedFilter.SQLTextLike,
                                                                  advancedFilter.SQLTextNotLike,
                                                                  advancedFilter.UserLike,
                                                                  advancedFilter.UserNotLike);
                localReportData.dataSources[2] = dataSource;

                dataSource = new ReportDataSource("Reads");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTopQueriesByReads",
                                                                  localReportData.instanceID,
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  (bool)localReportData.reportParameters["showStoredProcedures"],
                                                                  (bool)localReportData.reportParameters["showSQLStatements"],
                                                                  (bool)localReportData.reportParameters["showBatches"],
                                                                  localReportData.reportParameters["minExecutions"].ToString(),
                                                                  localReportData.reportParameters["minDuration"].ToString(),
                                                                  localReportData.reportParameters["minReads"].ToString(),
                                                                  localReportData.reportParameters["minWrites"].ToString(),
                                                                  localReportData.reportParameters["minCPU"].ToString(),
                                                                  localReportData.reportParameters["TopN"].ToString(),
                                                                  (bool)localReportData.reportParameters["sigMode"],
                                                                  (bool)localReportData.reportParameters["caseInsensitive"],
                                                                  advancedFilter.ApplicationLike,
                                                                  advancedFilter.ApplicationNotLike,
                                                                  advancedFilter.ClientLike,
                                                                  advancedFilter.ClientNotLike,
                                                                  advancedFilter.DatabaseLike,
                                                                  advancedFilter.DatabaseNotLike,
                                                                  advancedFilter.SQLTextLike,
                                                                  advancedFilter.SQLTextNotLike,
                                                                  advancedFilter.UserLike,
                                                                  advancedFilter.UserNotLike);

                localReportData.dataSources[3] = dataSource;

                dataSource = new ReportDataSource("Writes");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTopQueriesByWrites",
                                                                  localReportData.instanceID,
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  (bool)localReportData.reportParameters["showStoredProcedures"],
                                                                  (bool)localReportData.reportParameters["showSQLStatements"],
                                                                  (bool)localReportData.reportParameters["showBatches"],
                                                                  localReportData.reportParameters["minExecutions"].ToString(),
                                                                  localReportData.reportParameters["minDuration"].ToString(),
                                                                  localReportData.reportParameters["minReads"].ToString(),
                                                                  localReportData.reportParameters["minWrites"].ToString(),
                                                                  localReportData.reportParameters["minCPU"].ToString(),
                                                                  localReportData.reportParameters["TopN"].ToString(),
                                                                  (bool)localReportData.reportParameters["sigMode"],
                                                                  (bool)localReportData.reportParameters["caseInsensitive"],
                                                                  advancedFilter.ApplicationLike,
                                                                  advancedFilter.ApplicationNotLike,
                                                                  advancedFilter.ClientLike,
                                                                  advancedFilter.ClientNotLike,
                                                                  advancedFilter.DatabaseLike,
                                                                  advancedFilter.DatabaseNotLike,
                                                                  advancedFilter.SQLTextLike,
                                                                  advancedFilter.SQLTextNotLike,
                                                                  advancedFilter.UserLike,
                                                                  advancedFilter.UserNotLike);

                localReportData.dataSources[4] = dataSource;

                passthroughParameters.Add(new ReportParameter("SQLServerID", localReportData.instanceID.ToString()));
                passthroughParameters.Add(new ReportParameter("TopN", localReportData.reportParameters["TopN"].ToString()));
                passthroughParameters.Add(new ReportParameter("UtcOffset", localReportData.dateRanges[0].UtcOffsetMinutes.ToString()));
                passthroughParameters.Add(new ReportParameter("SignatureMode", localReportData.reportParameters["sigMode"].ToString()));
                //passthroughParameters.Add(
                //    new ReportParameter("StartDateTime",
                //                        localReportData.dateRanges[0].UtcStart.AddHours(
                //                            localReportData.dateRanges[0].UtcOffsetMinutes).ToString()));
                //passthroughParameters.Add(new ReportParameter("EndDateTime", localReportData.dateRanges[0].UtcEnd.AddHours(
                //                            localReportData.dateRanges[0].UtcOffsetMinutes).ToString()));
                passthroughParameters.Add(new ReportParameter("CaseInsensitive", localReportData.reportParameters["caseInsensitive"].ToString()));
                
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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.TopQueries.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }

                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "TopQueries";
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

        /// <summary>
        /// Event for enable the report links.
        /// </summary>
        private void MainContentPanelGotFocus(object sender, EventArgs e)
        {
            if (!isHyperLinkActivated)
            {
                ChangeHyperLinkState(true);
                reportViewer.RefreshReport();
            }
        }

        /// <summary>
        /// Event for disable the report links when the report is exported to excel.
        /// </summary>
        private void ExportDisableHyperlinkEventHandler(object sender, ReportExportEventArgs e)
        {
            if(e != null && e.Extension.Name.ToLower().Equals("excel"))
            {
                MainContentPanel.Focus();
                ChangeHyperLinkState(false);    
            }           
        }

        /// <summary>
        /// Enable or Disable the report links.
        /// </summary>
        /// <param name="status">True for enable, false otherwise</param>
        private void ChangeHyperLinkState(bool status)
        {          
            List<ReportParameter> parameters = passthroughParameters;
                      
            foreach (ReportParameter paramenter in parameters)
            {
                if (paramenter.Name.Equals("IsHyperlinksEnabled"))
                {
                    isHyperLinkActivated = status;

                    parameters.Remove(paramenter);
                    parameters.Add(new ReportParameter("IsHyperlinksEnabled", isHyperLinkActivated.ToString()));
                    reportViewer.LocalReport.SetParameters(parameters);                                       
                    break;
                }
            }               
        }
        override protected void reportViewer_Drillthrough(object sender, DrillthroughEventArgs e)
        {
            using (Log.DebugCall())
            {
                e.Cancel = true; // We'll handle this our way.
                Log.Debug("e.ReportPath = ", e.ReportPath);
                if (e.ReportPath == ReportsHelper.GetReportTitle(ReportTypes.QueryStatistics))
                {
                    ApplicationController.Default.ShowReportsView(ReportTypes.QueryStatistics, e);
                }
            }
        }

        public override void ResetFilterCriteria()
        {
            base.ResetFilterCriteria();
            showStoredProcedures.Checked = true;
            showSQLStatements.Checked = true;
            showBatches.Checked = true;
            numStatements.Text = topNDefault.ToString();
            minExecutions.Text = "0";
            minDuration.Text = "0";
            minCPU.Text = "0";
            minReads.Text = "0";
            minWrites.Text = "0";
            sigMode.Checked = true;
            caseInsensitive.Checked = false;
        }

        public void ShowAdvancedFilterDialog()
        {
            TopQueriesAdvancedFilter newfilter = new TopQueriesAdvancedFilter();

            newfilter.UpdateValues(advancedFilter);

            GenericFilterDialog dialog = new GenericFilterDialog(newfilter);
            DialogResult result = dialog.ShowDialog(this);

            if (result == DialogResult.OK)
                advancedFilter.UpdateValues(newfilter);
        }

        private void AdvancedFilterLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowAdvancedFilterDialog();
        }

        private void numStatements_Leave(object sender, EventArgs e)
        {
            if (this.numStatements.Text.Equals(string.Empty))
            {
                this.numStatements.Text = this.numStatements.Minimum.ToString();
            }
        }

        private void minDuration_Leave(object sender, EventArgs e)
        {
            if (this.minDuration.Text.Equals(string.Empty))
            {
                this.minDuration.Text = this.minDuration.Minimum.ToString();
            }
        }

        private void minExecutions_Leave(object sender, EventArgs e)
        {
            if (this.minExecutions.Text.Equals(string.Empty))
            {
                this.minExecutions.Text = this.minExecutions.Minimum.ToString();
            }
        }

        private void minCPU_Leave(object sender, EventArgs e)
        {
            if (this.minCPU.Text.Equals(string.Empty))
            {
                this.minCPU.Text = this.minCPU.Minimum.ToString();
            }
        }

        private void minWrites_Leave(object sender, EventArgs e)
        {
            if (this.minWrites.Text.Equals(string.Empty))
            {
                this.minWrites.Text = this.minWrites.Minimum.ToString();
            }
        }

        private void minReads_Leave(object sender, EventArgs e)
        {
            if (this.minReads.Text.Equals(string.Empty))
            {
                this.minReads.Text = this.minReads.Minimum.ToString();
            }
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

