using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml;
using BBS.TracerX;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class ActiveAlerts : ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("Active Alerts");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(1);

        public ActiveAlerts()
        {
            InitializeComponent();
            AdaptFontSize();
        }

        /// <summary>
        /// Override the base initinstancecombo to ensure this report does not show any servers that have been deleted.
        /// </summary>
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
            string id;
            int serverID = -1;
            string serverXml = null;
            Boolean bFoundParameter = false;

            base.InitReport();

            InitializeReportViewer();
            ReportType = ReportTypes.ActiveAlerts;

            if (drillThroughArguments == null) return;

            if (drillthroughParams.ContainsKey("GUIServerID"))
            {
                try
                {
                    id = Convert.ToString(drillthroughParams["GUIServerID"][0]);
                    serverID = Convert.ToInt32(id);
                }
                catch{}
            }
            
            if (drillthroughParams.ContainsKey("SQLServerIDs"))
            {
                try
                {
                    serverXml = Convert.ToString(drillthroughParams["SQLServerIDs"][0]);
                }
                catch
                {
                    serverXml = null;
                }
            }
            if (serverXml != null)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(serverXml);
                    XmlNodeList servers = doc.GetElementsByTagName("Srvr");
                    id = servers[0].Attributes["ID"].Value;
                    serverID = Convert.ToInt32(id);
                }
                catch{}
            }

            foreach (ValueListItem monitoredServer in instanceCombo.Items)
            {
                // skip the all servers entry
                if (monitoredServer == instanceSelectOne) continue;

                if (((MonitoredSqlServer) monitoredServer.DataValue).Id != serverID) continue;

                instanceCombo.SelectedItem = monitoredServer;
                bFoundParameter = true;
                break;
            }

            if (serverID == -1) instanceCombo.SelectedItem = 0;

            if(bFoundParameter) RunReport(true);
        }

        /// <summary>
        /// This method is returning the report parameters.  The names must match what is set in the RDL file.
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, object> GetReportParmeters()
        {
            reportParameters.Clear();
            SetReportParameters();
            return reportParameters;
        }
        protected override void SetReportParameters()
        {
            base.SetReportParameters();

            ValueListItem selectedTag = (ValueListItem) tagsComboBox.SelectedItem;
            ValueListItem selectedInstance = (ValueListItem)instanceCombo.SelectedItem;

            if (selectedTag == null)
            {
                selectedTag = new ValueListItem(0, "All Tags");
            }
            else
            {
                if (tagsComboBox.SelectedIndex == 0) selectedTag = new ValueListItem(0, "All Tags");
                if ((int)selectedTag.DataValue == 0) selectedTag = new ValueListItem(0, "All Tags");
            }

            if (selectedInstance == null)
            {
                selectedInstance = new ValueListItem(0, "All Servers");
            }
            else
            {
                if (selectedInstance.DataValue == null)selectedInstance = new ValueListItem(0, "All Servers");
                if (instanceCombo.SelectedIndex == 0) selectedInstance = new ValueListItem(0, "All Servers");
            }

            reportParameters.Add("GUITags", selectedTag.DisplayText);
            reportParameters.Add("GUITagID", selectedTag.DataValue.ToString());
            reportParameters.Add("GUIServers", selectedInstance.DisplayText);
            if(selectedInstance.DataValue is MonitoredSqlServer)
            {
                reportParameters.Add("GUIServerID", ((MonitoredSqlServer)selectedInstance.DataValue).Id.ToString());
            }else
            {
                reportParameters.Add("GUIServerID", "0");
            }

        }
        override protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall())
            {
                WorkerData localReportData = (WorkerData)e.Argument;
                ReportDataSource dataSource;

                passthroughParameters.Clear();
                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
                passthroughParameters.Add(new ReportParameter("ServerXML", GetServerIdXml(localReportData.serverIDs)));
                passthroughParameters.Add(new ReportParameter("GUITags", (string) localReportData.reportParameters["GUITags"]));
                passthroughParameters.Add(new ReportParameter("GUIServers", (string)localReportData.reportParameters["GUIServers"]));

                Log.Debug("localReportData.reportType = ", localReportData.reportType);
                localReportData.dataSources = new ReportDataSource[1];

                dataSource = new ReportDataSource("ActiveAlerts");
                
                IList<int> servers;
                //if a single instance has not been selected then try showing by tag
                if (localReportData.instanceID < 1)
                {
                    if (localReportData.serverIDs != null)
                    {
                        servers = localReportData.serverIDs;
                    }else{

                        servers = new List<int>(ApplicationModel.Default.AllInstances.Count);
                        foreach (MonitoredSqlServer server in ApplicationModel.Default.AllInstances.Values)
                        {
                            servers.Add(server.Id);
                        }
                    }
                }
                else
                {
                    servers = new List<int>(1);
                    servers.Add(localReportData.instanceID);
                }
                dataSource.Value = RepositoryHelper.GetReportData("p_GetActiveAlerts",
                    GetServerIdXml(servers));
                
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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.ActiveAlerts.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }
                            
                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "ActiveAlerts";
                            
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
    }
}

