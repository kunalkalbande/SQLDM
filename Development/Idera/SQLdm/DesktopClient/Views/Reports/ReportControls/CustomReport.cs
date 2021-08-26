using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Data;
using System.IO;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class CustomReport: ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("CustomReport");
        private readonly List<ReportParameter> passthroughParameters = new List<ReportParameter>(2);
        private readonly string _reportKey = null;
        private int? _defaultServerID = null;
        private Common.Objects.CustomReport _customReport = null;
        private const string TOPSERVER_STOREDPROCEDURE = "p_GetTopServersForCustomCountersDataSet";

        public CustomReport():this(null, null)
        {
            // No op.
        }

        public CustomReport(string reportKey, int? defaultServerID)
        {
            _reportKey = reportKey;
            _customReportName = reportKey;
            _defaultServerID = defaultServerID;//get the custom report object populated with a string of rdl
            _customReport = RepositoryHelper.GetCustomReport(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                 ReportName);

            //START: SQLdm 10.0 (Tarun Sapra)- Top Servers For Custom Counters 
            if (_customReport.ShowTopServers)
            {                
                InitializeComponentTopServers();
            }
            else 
            {
                InitializeComponent();
                showTablesCheckbox.Checked = _customReport.ShowTable;
            }
            //END: SQLdm 10.0 (Tarun Sapra)- Top Servers For Custom Counters
            _reportKey = reportKey;
            _customReportName = reportKey;
            _defaultServerID = defaultServerID;
            

            AdaptFontSize();
        }

        public override void InitReport()
        {
            base.InitReport();
            InitializeReportViewer();
            ReportType = ReportTypes.Custom;
            reportInstructionsControl.ReportTitle = _customReportName;
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

        protected void SetReportParametersTopServers()
        {
            //reportParameters.Add("ShowTopServers", true);
            reportParameters.Add("NumServers", numberOfServers.Text);
            reportParameters.Add("Period", ((int)reportData.periodType).ToString());

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

        protected override void SetReportParameters()
        {
            base.SetReportParameters();
            if (_customReport.ShowTopServers)
            {
                SetReportParametersTopServers();
                return;
            }

            //reportParameters.Add("ShowTopServers", false);

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
            reportParameters.Add("DisplayTables", showTablesCheckbox.Checked.ToString());
            
            if (selectedInstance.DataValue is MonitoredSqlServer) reportParameters.Add("GUIServerID", ((MonitoredSqlServer)selectedInstance.DataValue).Id.ToString());

            if (periodCombo.SelectedItem != periodSetCustom) return;

            reportParameters.Add("rsStart", reportData.dateRanges[0].UtcStart.ToLocalTime().ToString("yyyy'-'MM'-'dd HH':'mm':'ss"));
            reportParameters.Add("rsEnd", reportData.dateRanges[0].UtcEnd.ToLocalTime().ToString("yyyy'-'MM'-'dd HH':'mm':'ss"));
        }

        public override Dictionary<string, object> GetReportParmeters()
        {
            SetReportParameters();
            return reportParameters;
        }
        private string ReportName
        {
            get { return _reportKey; }
        }

        override protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall())
            {
                WorkerData localReportData = (WorkerData)e.Argument;
                Boolean ShowTopServers = _customReport.ShowTopServers;
                ReportDataSource dataSource;
                if (ShowTopServers)
                {
                    //START: SQLdm 10.0(Tarun Sapra)- Passing report parameters and load data for report
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

                    if (localReportData.reportParameters.ContainsKey("rsStart")) passthroughParameters.Add(new ReportParameter("rsStart",
                                                                      localReportData.reportParameters["rsStart"].ToString()));
                    if (localReportData.reportParameters.ContainsKey("rsEnd")) passthroughParameters.Add(new ReportParameter("rsEnd",
                                                                      localReportData.reportParameters["rsEnd"].ToString()));

                    DataTable reportCounters =  RepositoryHelper.GetSelectedCounters(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, ReportName);

                    SortedDictionary<int, CustomReportMetric> metrics =
                        new SortedDictionary<int, CustomReportMetric>();

                    foreach (DataRow row in reportCounters.Rows)
                    {
                        int temp1,temp2,temp3;
                        if (int.TryParse(row["CounterType"].ToString(), out temp1) && int.TryParse(row["Aggregation"].ToString(), out temp2))
                        {
                            CustomReportMetric metric = new CustomReportMetric(row["CounterName"].ToString(),
                                                                           row["CounterShortDescription"].ToString(),
                                                                           (Common.Objects.CustomReport.CounterType)temp1,
                                                                           (Common.Objects.CustomReport.Aggregation)temp2);
                            if (int.TryParse(row["GraphNumber"].ToString(), out temp3) && !metrics.ContainsKey(temp3))
                                metrics.Add(temp3, metric);  
                        }                        
                    }

                    _customReport.Metrics = metrics;

                    //localReportData.dataSources = new ReportDataSource[_customReport.Metrics.Count ];
                    localReportData.dataSources = new ReportDataSource[1];
                    _customReport.Metrics = metrics;
                    
                    dataSource = new ReportDataSource("TopServersForCustomCounters");
                    dataSource.Value = RepositoryHelper.GetReportData(TOPSERVER_STOREDPROCEDURE,
                                                                      (string)localReportData.reportParameters["NumServers"],
                                                                      localReportData.dateRanges[0].UtcStart,
                                                                      localReportData.dateRanges[0].UtcEnd,
                                                                      _customReport.Name);
                    localReportData.dataSources[0] = dataSource;
                    //END: SQLdm 10.0(Tarun Sapra)- Passing report parameters and load data for report
                }
                else
                {
                    passthroughParameters.Clear();
                    passthroughParameters.Add(new ReportParameter("Interval", ((int)reportData.sampleSize).ToString()));
                    passthroughParameters.Add(new ReportParameter("Period", ((int)reportData.periodType).ToString()));
                    passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
                    passthroughParameters.Add(new ReportParameter("DisplayTables", (string)localReportData.reportParameters["DisplayTables"]));

                    if (localReportData.reportParameters.ContainsKey("rsStart")) passthroughParameters.Add(new ReportParameter("rsStart",
                                                                       localReportData.reportParameters["rsStart"].ToString()));
                    if (localReportData.reportParameters.ContainsKey("rsEnd")) passthroughParameters.Add(new ReportParameter("rsEnd",
                                                                       localReportData.reportParameters["rsEnd"].ToString()));

                    Log.Debug("localReportData.reportType = ", localReportData.reportType);

                    localReportData.dataSources = new ReportDataSource[2];


                    dataSource = new ReportDataSource("CounterSummary");


                    DataTable reportCounters =
                        RepositoryHelper.GetSelectedCounters(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo, ReportName);

                    SortedDictionary<int, CustomReportMetric> metrics =
                        new SortedDictionary<int, CustomReportMetric>();

                    foreach (DataRow row in reportCounters.Rows)
                    {

                        CustomReportMetric metric = new CustomReportMetric(row["CounterName"].ToString(),
                                                                           row["CounterShortDescription"].ToString(),
                                                                           (Common.Objects.CustomReport.CounterType)
                                                                           int.Parse(row["CounterType"].ToString()),
                                                                           (Common.Objects.CustomReport.Aggregation)
                                                                           int.Parse(row["Aggregation"].ToString()));

                        if (!metrics.ContainsKey(int.Parse(row["GraphNumber"].ToString())))
                        {
                            metrics.Add(int.Parse(row["GraphNumber"].ToString()), metric);
                        }
                    }

                    _customReport.Metrics = metrics;

                    SqlParameter[] SqlParameters = new SqlParameter[6];

                    SqlParameters[0] = new SqlParameter("@ServerID", SqlDbType.Int);
                    SqlParameters[0].Value = localReportData.instanceID;
                    SqlParameters[1] = new SqlParameter("@UTCStart", SqlDbType.DateTime);
                    SqlParameters[1].Value = localReportData.dateRanges[0].UtcStart;
                    SqlParameters[2] = new SqlParameter("@UTCEnd", SqlDbType.DateTime);
                    SqlParameters[2].Value = localReportData.dateRanges[0].UtcEnd;
                    SqlParameters[3] = new SqlParameter("@UTCOffset", SqlDbType.Int);
                    SqlParameters[3].Value = localReportData.dateRanges[0].UtcOffsetMinutes;
                    SqlParameters[4] = new SqlParameter("@Interval", SqlDbType.Int);
                    SqlParameters[4].Value = (int)localReportData.sampleSize;
                    SqlParameters[5] = new SqlParameter("@reportName", SqlDbType.NVarChar);
                    SqlParameters[5].Value = (string)_customReport.Name;

                    dataSource.Value = RepositoryHelper.GetCustomReportData(_customReport.GetStoredProcedureName(), SqlParameters);

                    localReportData.dataSources[0] = dataSource;

                    dataSource = new ReportDataSource("p_GetOSAvailable");
                    dataSource.Value = RepositoryHelper.GetReportData("p_GetOSStatisticsAvailable",
                                                                      localReportData.instanceID);
                    localReportData.dataSources[1] = dataSource;

                }
                if (localReportData.cancelled)
                {
                    Log.Debug("reportData.cancelled = true.");
                    e.Cancel = true;
                }
                else
                {
                    if (ShowTopServers)
                    {
                        e.Result = new Pair<WorkerData, string>(localReportData, _customReport.ReportRDL);
                    }
                    else
                    {
                        e.Result = new Pair<WorkerData, string>(localReportData, _customReport.ReportRDL);
                    }
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


                            string reportRDL = "";
                            if(e.Result is Pair<WorkerData, string>)
                            {
                                if(((Pair<WorkerData, string>)e.Result).Second != null)
                                {
                                    reportRDL = ((Pair<WorkerData, string>) e.Result).Second;
                                    using (StreamReader stream = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(reportRDL))))
                                    {
                                        reportViewer.LocalReport.LoadReportDefinition(stream);
                                    }

                                    foreach (ReportDataSource dataSource in reportData.dataSources)
                                    {
                                        reportViewer.LocalReport.DataSources.Add(dataSource);
                                    }

                                    reportViewer.LocalReport.SetParameters(passthroughParameters);

                                    reportViewer.RefreshReport();
                                    reportViewer.LocalReport.DisplayName = this._customReportName;
                                    State = UIState.Rendered;
                                }
                            }
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
            //START: SQLdm 10.0 (Tarun Sapra)- Reset filter changes for the top servers report
            if (!_customReport.ShowTopServers)
            {
                if (_customReport != null)
                {
                    showTablesCheckbox.Checked = _customReport.ShowTable;
                    return;
                }
                showTablesCheckbox.Checked = true;
            }
            else
            {
                numberOfServers.Value = 5;
            } 
            //END: SQLdm 10.0 (Tarun Sapra)- Reset filter changes for the top servers report
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

            if (_defaultServerID.HasValue)
            {

                try
                {
                    serverID = _defaultServerID.Value;
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
                instanceCount = ApplicationModel.Default.AllInstances.Count;
                instances = new ValueListItem[instanceCount];

                foreach (MonitoredSqlServer server in ApplicationModel.Default.AllInstances.Values)
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

    }

}
