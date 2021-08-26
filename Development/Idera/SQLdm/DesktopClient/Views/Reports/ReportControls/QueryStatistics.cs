using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;


namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class QueryStatistics : Idera.SQLdm.DesktopClient.Views.Reports.ReportControls.ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("Query Statistics");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(10);
        private bool exportToExcel;

        readonly protected ValueListItem storedProcedure = new ValueListItem(0, "Stored Procedure");
        readonly protected ValueListItem sqlStatement = new ValueListItem(1, "SQL Statement");
        readonly protected ValueListItem batch = new ValueListItem(2, "Batch");

        public QueryStatistics()
        {
            InitializeComponent();
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
            InitStatementTypeCombo();

            exportToExcel = false;
            sigMode.Checked = false;
            queryText.Text = null;
            appName.Text = null;
            dbName.Text = null;
            eventTypeCombo.SelectedItem = storedProcedure;

            if (drillThroughArguments != null)
            {
                try
                {
                    queryText.Text = Convert.ToString(drillthroughParams["QueryText"][0]);
                }
                catch
                {
                    queryText.Text = null;
                }

                try
                {
                    appName.Text = Convert.ToString(drillthroughParams["ApplicationName"][0]);
                }
                catch
                {
                    appName.Text = null;
                }

                try
                {
                    dbName.Text = Convert.ToString(drillthroughParams["DatabaseName"][0]);
                }
                catch
                {
                    dbName.Text = null;
                }

                try
                {
                    int eventType = Convert.ToInt32(drillthroughParams["StatementType"][0]);
                    switch (eventType)
                    {
                        case 0:
                            eventTypeCombo.SelectedItem = storedProcedure;
                            break;
                        case 1:
                            eventTypeCombo.SelectedItem = sqlStatement;
                            break;
                        case 2:
                            eventTypeCombo.SelectedItem = batch;
                            break;
                        default:
                            eventTypeCombo.SelectedItem = storedProcedure;
                            break;
                    }
                    
                }
                catch
                {
                    eventTypeCombo.SelectedItem = storedProcedure;
                }


                try
                {
                    sigMode.Checked = Convert.ToBoolean(drillthroughParams["SignatureMode"][0]);
                }
                catch
                {
                    sigMode.Checked = false;
                }

                try
                {
                    caseInsensitive.Checked = Convert.ToBoolean(drillthroughParams["CaseInsensitive"][0]);
                }
                catch
                {
                    caseInsensitive.Checked = false;
                }
            }

            ReportType = ReportTypes.QueryStatistics;
            RunReport(true);
        }

        private void InitStatementTypeCombo()
        {
            eventTypeCombo.Items.Add(storedProcedure);
            eventTypeCombo.Items.Add(sqlStatement);
            eventTypeCombo.Items.Add(batch);
            eventTypeCombo.SelectedItem = storedProcedure;
        }

        protected override void SetReportParameters()
        {
            base.SetReportParameters();
            passthroughParameters.Clear();
            reportParameters.Add("queryText", queryText.Text);
            reportParameters.Add("appName", appName.Text);
            
            if (eventTypeCombo.SelectedItem != null)
            {
                reportParameters.Add("eventTypeCombo", ((ValueListItem)eventTypeCombo.SelectedItem).DataValue);
                passthroughParameters.Add(new ReportParameter("StatementType", (((ValueListItem)eventTypeCombo.SelectedItem).DataValue).ToString()));
            }
            reportParameters.Add("dbName", dbName.Text);
            reportParameters.Add("sigMode", sigMode.Checked);
            reportParameters.Add("caseInsensitive", caseInsensitive.Checked);
            reportParameters.Add("Period", ((int)reportData.periodType).ToString());
            reportParameters.Add("ExportToExcel", exportToExcel.ToString());
            passthroughParameters.Add(new ReportParameter("CaseInsensitive", caseInsensitive.Checked.ToString()));

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

            reportParameters.Add("GUIServers", selectedInstance.DisplayText);

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
            reportParameters.Add("StartDateTime", reportData.dateRanges[0].UtcStart.ToString(DateTimeFormatInfo.InvariantInfo));
            reportParameters.Add("EndDateTime", reportData.dateRanges[0].UtcEnd.ToString(DateTimeFormatInfo.InvariantInfo));
            //reportParameters.Add("UtcOffsetMinutes", reportData.dateRanges[0].UtcOffsetMinutes.ToString());
            reportParameters.Add("Interval", ((int)reportData.sampleSize).ToString());
            reportParameters.Add("QueryText", queryText.Text);
            reportParameters.Add("ApplicationName", appName.Text);
            reportParameters.Add("StatementType", (((ValueListItem)eventTypeCombo.SelectedItem).DataValue).ToString());
            reportParameters.Add("DatabaseName", dbName.Text);
            reportParameters.Add("SignatureMode", sigMode.Checked.ToString());
            reportParameters.Add("CaseInsensitive", caseInsensitive.Checked.ToString());
            reportParameters.Add("ExportToExcel", exportToExcel.ToString());
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
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall())
            {
                WorkerData localReportData = (WorkerData)e.Argument;
                ReportDataSource dataSource;

                Log.Debug("localReportData.reportType = ", localReportData.reportType);
                localReportData.dataSources = new ReportDataSource[2];

                
                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
                passthroughParameters.Add(new ReportParameter("ExportToExcel", localReportData.reportParameters["ExportToExcel"].ToString()));

                dataSource = new ReportDataSource("Overall");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetQueryStatistics",
                                                                  localReportData.instanceID,
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes,
                                                                  5,
                                                                  localReportData.reportParameters["queryText"].ToString(),
                                                                  localReportData.reportParameters["appName"].ToString(),
                                                                  localReportData.reportParameters["eventTypeCombo"].ToString(),
                                                                  localReportData.reportParameters["dbName"].ToString(),
                                                                  (bool)localReportData.reportParameters["sigMode"],
                                                                  (bool)localReportData.reportParameters["caseInsensitive"]);
                localReportData.dataSources[0] = dataSource;

                dataSource = new ReportDataSource("Grouped");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetQueryStatistics",
                                                                  localReportData.instanceID,
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes,
                                                                  localReportData.sampleSize,
                                                                  localReportData.reportParameters["queryText"].ToString(),
                                                                  localReportData.reportParameters["appName"].ToString(),
                                                                  localReportData.reportParameters["eventTypeCombo"].ToString(),
                                                                  localReportData.reportParameters["dbName"].ToString(),
                                                                  (bool)localReportData.reportParameters["sigMode"], 
                                                                  (bool)localReportData.reportParameters["caseInsensitive"]);
                localReportData.dataSources[1] = dataSource;

                if (localReportData.reportParameters.ContainsKey("rsStart")) passthroughParameters.Add(new ReportParameter("rsStart",
                                                                  localReportData.reportParameters["rsStart"].ToString()));
                if (localReportData.reportParameters.ContainsKey("rsEnd")) passthroughParameters.Add(new ReportParameter("rsEnd",
                                                                  localReportData.reportParameters["rsEnd"].ToString()));
                
                passthroughParameters.Add(new ReportParameter("Period", (string)localReportData.reportParameters["Period"]));
                passthroughParameters.Add(new ReportParameter("ApplicationName", (string)localReportData.reportParameters["appName"]));
                passthroughParameters.Add(new ReportParameter("QueryText", (string)localReportData.reportParameters["queryText"]));
                passthroughParameters.Add(new ReportParameter("DatabaseName", (string)localReportData.reportParameters["dbName"]));
                //passthroughParameters.Add(new ReportParameter("StartDateTime", localReportData.dateRanges[0].UtcStart.AddHours(localReportData.dateRanges[0].UtcOffsetMinutes).ToString()));
                //passthroughParameters.Add(new ReportParameter("EndDateTime", localReportData.dateRanges[0].UtcEnd.AddHours(localReportData.dateRanges[0].UtcOffsetMinutes).ToString()));
                passthroughParameters.Add(new ReportParameter("GUIServers", (string)localReportData.reportParameters["GUIServers"]));
                passthroughParameters.Add(new ReportParameter("SQLServerID", reportData.instanceID.ToString()));
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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.QueryStatistics.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }
                            
                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "QueryStatistics";
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
            sigMode.Checked = false;
            queryText.Text = null;
            appName.Text = null;
            dbName.Text = null;
            eventTypeCombo.SelectedItem = storedProcedure;
        }

        void reportViewer_ReportExport(object sender, Microsoft.Reporting.WinForms.ReportExportEventArgs e)
        {
            if (e != null && e.Extension.Name.ToLower().Equals("excel"))
            {
                
                exportToExcel = true;
                ChangeHyperLinkState(exportToExcel);
                exportToExcel = false;
            }
        }

        private void ChangeHyperLinkState(bool status)
        {
            List<ReportParameter> parameters = passthroughParameters;

            foreach (ReportParameter paramenter in parameters)
            {
                if (paramenter.Name.Equals("ExportToExcel"))
                {
                    parameters.Remove(paramenter);
                    parameters.Add(new ReportParameter("ExportToExcel", status.ToString()));
                    reportViewer.LocalReport.SetParameters(parameters);
                    break;
                }
            }
        }
    }
}

