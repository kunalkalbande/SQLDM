using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using BBS.TracerX;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;
using Microsoft.SqlServer.MessageBox;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    /// <summary>
    /// SQLdm-26953 10.2 (Varun Chopra) -Enhancement Request for Detailed Session Information report
    /// Added DetailedSessionReport class for showing Detailed Session Report
    /// </summary>
    public partial class DetailedSessionReport : ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("Detailed Session Report");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>();

        public DetailedSessionReport()
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
            ReportType = ReportTypes.DetailedSessionReport;
            RunReport(true);
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

            reportParameters.Add("GUIServer", selectedInstance.DisplayText);
            reportParameters.Add("ServerId", reportData.instanceID.ToString());

            //setting period
            reportParameters.Add("Period", ((int)reportData.periodType).ToString());
            if (periodCombo.SelectedItem != periodSetCustom) return;
            if (reportData.dateRanges != null)
            {
                if (reportData.dateRanges.Count > 0)
                {
                    reportParameters.Add("rsStart", reportData.dateRanges[0].UtcStart.ToLocalTime().ToString("yyyy'-'MM'-'dd HH':'mm':'ss"));
                    reportParameters.Add("rsEnd", reportData.dateRanges[0].UtcEnd.ToLocalTime().ToString("yyyy'-'MM'-'dd HH':'mm':'ss"));
                }
            }
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

                dataSource = new ReportDataSource("DetailedSessionReport");
                IList<SessionSnapshot> sessionSnapshots =
                RepositoryHelper.GetSessionsDetailsRanged(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                    localReportData.instanceID, localReportData.dateRanges[0].UtcStart, localReportData.dateRanges[0].UtcEnd);


                DataTable dataTable = new DataTable();
                List<Tuple<string, Type>> columnsList = new List<Tuple<string, Type>>()
                {
                    new Tuple<string, Type>("SessionID", typeof (int)),
                    new Tuple<string, Type>("User", typeof (string)),
                    new Tuple<string, Type>("Host", typeof (string)),
                    new Tuple<string, Type>("Application", typeof (string)),
                    new Tuple<string, Type>("LoginTime", typeof (DateTime)),
                    new Tuple<string, Type>("LastActivity", typeof (DateTime))
                };

                for (int i = 0; i < columnsList.Count; i++)
                {
                    if (dataTable.Columns.Contains(columnsList[i].Item1))
                    {
                        continue;
                    }
                    DataColumn dataColumn = new DataColumn
                    {
                        ColumnName = columnsList[i].Item1,
                        Unique = false,
                        AllowDBNull = true, //SQLDM-29427: Changed to true in order to avoid issues with the report
                        ReadOnly = false,
                        DataType = columnsList[i].Item2
                    };
                    dataTable.Columns.Add(dataColumn);
                }

                var sortedSnapshots = new SortedList<string, object[]>();
                foreach (var sessionSnapshot in sessionSnapshots)
                {
                    if (sessionSnapshot != null)
                    {
                        Dictionary<Pair<int?, DateTime?>, Session> sessionListData = sessionSnapshot.SessionList;
                    
                        object[] itemArray = new object[columnsList.Count];
                        dataTable.BeginLoadData();
                        foreach (KeyValuePair<Pair<int?, DateTime?>, Session> sessionDataPair in sessionListData)
                        {
                            // Session ID
                            if (sessionDataPair.Key.First != null)
                            {
                                itemArray[0] = sessionDataPair.Key.First.Value;
                            }
                            // User
                            itemArray[1] = sessionDataPair.Value.UserName;
                            // Host
                            itemArray[2] = sessionDataPair.Value.Workstation;
                            // Application
                            itemArray[3] = sessionDataPair.Value.Application;
                            // Login Time
                            if (sessionDataPair.Value.LoggedInSince != null)
                            {
                                itemArray[4] = sessionDataPair.Value.LoggedInSince.Value;
                            }
                            // Last Activity
                            if (sessionDataPair.Value.LastActivity != null)
                            {
                                itemArray[5] = sessionDataPair.Value.LastActivity.Value;
                            }
                            dataTable.LoadDataRow(itemArray, true);
                        }
                        dataTable.EndLoadData();
                    }
                }
                DataView dataView = new DataView(dataTable);
                dataView.Sort = "LoginTime desc";
                dataSource.Value = dataView;
                localReportData.dataSources[0] = dataSource;

                //Start : Setting parameters for rdl
                passthroughParameters.Add(new ReportParameter("ServerId", localReportData.instanceID.ToString()));
                passthroughParameters.Add(new ReportParameter("GUIServer", (string)localReportData.reportParameters["GUIServer"]));
                passthroughParameters.Add(new ReportParameter("UTCOffset", localReportData.dateRanges[0].UtcOffsetMinutes.ToString()));
                passthroughParameters.Add(new ReportParameter("Period", ((int)reportData.periodType).ToString()));
                //End : Setting parameters for rdl

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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.DetailedSessionReport.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }

                            //passing parameters to rdl
                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "DetailedSessionReport";
                            State = UIState.Rendered;
                        }
                        catch (Exception exception)
                        {
                            ApplicationMessageBox.ShowError(ParentForm, "An error occurred while refreshing the report.", exception);
                            State = UIState.ParmsNeeded;
                            Exception msg = new Exception("An error occurred while refreshing the report.", exception);
                            Log.Error("Showing message box: ", msg);
                        }
                    }
                }
            }
        }
    }
}