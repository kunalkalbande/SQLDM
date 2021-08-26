using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
    public partial class ServerUptime : ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("Alert History");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(1);
        private string serverName;

        public ServerUptime()
        {
            InitializeComponent();
            base.AdaptFontSize();
        }

        public override void InitReport()
        {
            base.InitReport();
            InitializeReportViewer();
            ReportType = ReportTypes.ServerUptime;
        }

        protected override void SetReportParameters()
        {
            base.SetReportParameters();
            
            reportParameters.Add("UTCOffset", reportData.dateRanges[0].UtcOffsetMinutes.ToString());
            if(serverName != null)
                reportParameters.Add("Servername", serverName);
            
            if (periodCombo.SelectedItem != periodSetCustom) return;
        }

        /// <summary>
        /// This method is returning the report parameters.  The names must match what is set in the RDL file.
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, object> GetReportParmeters()
        {
            SetReportParameters();
            if (reportParameters.ContainsKey("UTCOffset")) reportParameters.Remove("UTCOffset");
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
                passthroughParameters.Add(new ReportParameter("UTCStart", reportData.dateRanges[0].UtcStart.ToString()));
                passthroughParameters.Add(new ReportParameter("UTCEnd", reportData.dateRanges[0].UtcEnd.ToString()));
                if (serverName != null)
                    passthroughParameters.Add(new ReportParameter("Servername", serverName));

                Log.Debug("localReportData.reportType = ", localReportData.reportType);
                localReportData.dataSources = new ReportDataSource[1];

                dataSource = new ReportDataSource("ServerUptime");

                if (localReportData.serverIDs != null)
                {
                    dataSource.Value =
                        RepositoryHelper.GetReportData("p_ServerUptime",
                                                       localReportData.dateRanges[0].UtcStart,
                                                       localReportData.dateRanges[0].UtcEnd,
                                                       serverName);
                }
                else
                {
                    IList<int> servers;

                    if (localReportData.instanceID < 1)
                    {
                        servers = new List<int>(ApplicationModel.Default.AllInstances.Count);

                        foreach (MonitoredSqlServer server in ApplicationModel.Default.AllInstances.Values)
                        {
                            servers.Add(server.Id);
                        }
                    }
                    else
                    {
                        servers = new List<int>(1);
                        servers.Add(localReportData.instanceID);
                    }
                    dataSource.Value = RepositoryHelper.GetReportData("p_ServerUptime",
                                                                      localReportData.dateRanges[0].UtcStart,
                                                                      localReportData.dateRanges[0].UtcEnd,
                                                                      serverName);
                }
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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.UptimeReport.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }
                            
                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "ServerUptime";
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

                if (e.ReportPath == ReportsHelper.GetReportTitle(ReportTypes.ServerSummary))
                {
                    ApplicationController.Default.ShowReportsView(ReportTypes.ServerSummary, e);
                }
            }
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

        public override void ResetFilterCriteria()
        {
            base.ResetFilterCriteria();
        }

      
        
    }
}

