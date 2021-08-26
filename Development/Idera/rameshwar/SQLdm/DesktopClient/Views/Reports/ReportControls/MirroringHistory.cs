using System;
using System.Collections;
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
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class MirroringHistory : ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("Mirroring History");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(5);
        protected ValueListItem databaseSelectOne;
        private const string NoMirrorServers = "< No Mirrored Servers >";
        private const string SelectAServer = "< All Mirrored Servers >";
        private const string SelectADatabase = "< All Mirrored Databases >";
        private const string NoMirroredDatabases = "< No Mirrored Databases >";

        public MirroringHistory()
        {
            InitializeComponent();
            base.AdaptFontSize();
        }

        public override void InitReport()
        {
            base.InitReport();
            InitializeReportViewer();
            ReportType = ReportTypes.MirroringHistory;
        }

        protected void InitDatabasesCombo(string permittedServerIDs)
        {
            databasesCombo.Items.Clear();

            databaseSelectOne = new ValueListItem(null, instanceCombo.SelectedItem.ToString()==NoMirrorServers?NoMirroredDatabases:SelectADatabase);
            databasesCombo.Items.Add(databaseSelectOne);

            List<ValueListItem> mirroredDatabases = RepositoryHelper.GetMirroredDatabases(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, permittedServerIDs);

            foreach(ValueListItem database in mirroredDatabases)
            {
                databasesCombo.Items.Add(database);
            }

            if (databasesCombo.Items.Count > 0) databasesCombo.SelectedIndex = 0;
        }

        public override void ResetFilterCriteria()
        {
            base.ResetFilterCriteria();
            chkProblemsOnly.Checked = false;
            databasesCombo.SelectedIndex = 0;
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

        protected override void tagsComboBox_SelectionChanged(object sender, EventArgs e)
        {
            InitInstanceCombo();
            SetReportParameters();
        }

        /// <summary>
        /// Combo box of Servers for single select
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void instanceCombo_ValueChanged(object sender, EventArgs e)
        {
            if (instanceCombo.SelectedItem == null) return;
            currentServer = (MonitoredSqlServer)((ValueListItem)instanceCombo.SelectedItem).DataValue;
            //SetReportParameters();
            reportData.serverIDs = null;
            if (instanceCombo.SelectedItem.ToString().ToLower() == SelectAServer.ToLower())
            {
                //if we dont have server ids yet
                if (reportData.serverIDs == null)
                {
                    //get the list of server ids from the server combo box
                    List<int> servers = new List<int>();

                    IEnumerator enumerator = instanceCombo.Items.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (!((ValueListItem)enumerator.Current).DisplayText.ToLower().Equals(SelectAServer.ToLower()))
                        {
                            servers.Add(((MonitoredSqlServer)((ValueListItem)enumerator.Current).DataValue).Id);
                        }
                    }
                    reportData.serverIDs = servers;
                }
            }

            if (reportData.serverIDs != null)
            {
                InitDatabasesCombo(GetServerIdXml(reportData.serverIDs));
            }
            else
            {
                InitDatabasesCombo(currentServer == null ? null : GetServerIdXml(null, currentServer.Id));
            }
        }

        protected override void InitInstanceCombo()
        {
            instanceCombo.Items.Clear();
            ValueListItem[] instances = null;
            ValueListItem instance;
            int instanceCount;
            int mirroredInstances = 0;

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


            //get all servers that have mirroring relationships
            List<string> mirroringServers = RepositoryHelper.GetMonitoredMirroredServers(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            //if a tag has been selected
            if ((tagsComboBox.SelectedItem != null) && (tagsComboBox.SelectedItem != tagSelectOne) && (((ValueListItem)tagsComboBox.SelectedItem).DataValue != tagSelectOne.DataValue))
            {
                int intSelectedTagID = -1;
                ValueListItem selectedTagValueListItem = tagsComboBox.SelectedItem as ValueListItem;
                
                //if a tag has been selected
                if (selectedTagValueListItem != null && selectedTagValueListItem.DataValue != null)
                {
                    intSelectedTagID = (int)(((ValueListItem)tagsComboBox.SelectedItem).DataValue);
                }
                else //if the selected tag was a null entry
                {
                    instanceSelectOne = new ValueListItem(null, mirroringServers.Count > 0 ? SelectAServer : NoMirrorServers);
                    instanceCombo.Items.Add(instanceSelectOne);
                    //if no tag selected then select the first item in instance combo
                    instanceCombo.SelectedIndex = 0;
                    return;
                }

                //get the number of instances associated with this tag
                instanceCount = GetInstanceCount(intSelectedTagID);

                //create a valuelist array to hold all of these instances
                instances = new ValueListItem[instanceCount];

                //get the tag object
                Tag tag = GetTag(intSelectedTagID);
                if (tag == null)
                {
                    tagsComboBox.Items.Remove(tagsComboBox.SelectedItem);
                    tagsComboBox.SelectedIndex = 0;
                    return;
                }
                MonitoredSqlServer server;

                //iterate through each instance
                foreach (int id in tag.Instances)
                {
                    //get the monitored server
                    server = GetServer(id);

                    //if the server is hosting mirroring
                    if (mirroringServers.Contains(server.InstanceName))
                    {
                        //save the server
                        instance = new ValueListItem(server, server.InstanceName);

                        //if this is the current server then save it for later
                        if ((serverID != -1) && (serverID == server.Id))
                        {
                            currentServer = server;
                        }
                        mirroredInstances++;
                        instances[i++] = instance;
                    }
                    else
                    {
                        instances[i++] = null;
                    }
                }
            }
            else//if no tag has been selected
            {
                //How many active instances do we have?
                instanceCount = ApplicationModel.Default.AllInstances.Count;
                instances = new ValueListItem[instanceCount];
                
                //iterate through the list of instances that are available to this user
                foreach (MonitoredSqlServer server in ApplicationModel.Default.AllInstances.Values)
                {
                    //if the server is hosting mirroring
                    if (mirroringServers.Contains(server.InstanceName))
                    {
                        //save the instance to the instances array
                        instance = new ValueListItem(server, server.InstanceName);

                        if ((serverID != -1) && (serverID == server.Id))
                        {
                            currentServer = server;
                        }
                        mirroredInstances++;
                        instances[i++] = instance;
                    }
                    else
                    {
                        //this instance hosts no mirroring so set it to null to be cleared out
                        instances[i++] = null;
                    }
                }
            }

            //The filtered list is the list containing only mirroring host that the user has access to
            ValueListItem[] filtered = new ValueListItem[mirroredInstances];
            int mirrorInstancePtr = 0;
            for (i = 0; i < instances.Length; i++)
            {
                if (instances[i] != null)
                {
                    filtered[mirrorInstancePtr] = instances[i];
                    mirrorInstancePtr++;
                }
            }

            instanceSelectOne = new ValueListItem(null, filtered.Length > 0 ? SelectAServer : NoMirrorServers);
            instanceCombo.Items.Add(instanceSelectOne);

            instanceCombo.Items.AddRange(filtered);

            //now re-select the one they had selected, if they did have one
            if (instanceCombo.SelectedItem == null && currentServer != null)
            {
                instanceCombo.SelectedIndex = instanceCombo.FindStringExact(currentServer.InstanceName);
                if (instanceCombo.SelectedIndex == -1) instanceCombo.SelectedIndex = 0;
            }
            else
            {
                instanceCombo.SelectedIndex = 0;
            }
        }

        public override bool CanRunReport(out string message)
        {
            message = String.Empty;

            if (instanceCombo.SelectedIndex == 0)
            {
                bool blnNoMirroringDefined = (((ValueListItem) instanceCombo.SelectedItem).DisplayText==NoMirrorServers);

                if (blnNoMirroringDefined)
                {
                    message = "No Mirroring sessions have been set up for this tag\\server. This report populates exclusively with Servers that have mirrored databases.";
                    return false;
                }
            }
            return true;
        }

        protected override void SetReportParameters()
        {
            ValueListItem selectedInstance = (ValueListItem)instanceCombo.SelectedItem;
            ValueListItem selectedTag = (ValueListItem)tagsComboBox.SelectedItem;
            ValueListItem selectedDatabase = (ValueListItem)databasesCombo.SelectedItem;

            base.SetReportParameters();
            IList<int> servers;

            if (reportData.serverIDs != null)
            {
                servers = reportData.serverIDs;
            }
            else
            {
                servers = new List<int>(ApplicationModel.Default.AllInstances.Count);

                foreach (MonitoredSqlServer server in ApplicationModel.Default.AllInstances.Values)
                {
                    servers.Add(server.Id);
                }
            }
            reportData.serverIDs = servers;

            reportParameters.Add("problemsOnly", chkProblemsOnly.Checked.ToString());

            if (instanceCombo.SelectedItem != null)
            {
                if (instanceCombo.SelectedItem.ToString().ToLower() != SelectAServer.ToLower())
                {
                    reportParameters.Add("GUIServer", instanceCombo.SelectedItem.ToString());
                }
            }

            if (databasesCombo.SelectedItem != null)
            {
                if (databasesCombo.SelectedItem.ToString().ToLower() != SelectADatabase.ToLower())
                {
                    reportParameters.Add("GUIDatabase", databasesCombo.SelectedItem.ToString());
                }
            }
            
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
                if (selectedInstance.DataValue == null) selectedInstance = new ValueListItem(0, "All Servers");
                if (instanceCombo.SelectedIndex == 0) selectedInstance = new ValueListItem(0, "All Servers");
            }

            if (selectedDatabase == null)
            {
                selectedDatabase = new ValueListItem(0, "All Databases");
            }
            else
            {
                if (selectedDatabase.DataValue == null) selectedDatabase = new ValueListItem(0, "All Databases");
                if (databasesCombo.SelectedIndex == 0) selectedDatabase = new ValueListItem(0, "All Databases");
            }

            reportParameters.Add("GUITags", selectedTag.DisplayText);
            reportParameters.Add("Period", ((int)reportData.periodType).ToString());
            reportParameters.Add("SQLServerIDs", GetServerIdXml(reportData.serverIDs));

            reportParameters.Add("GUITagID", selectedTag.DataValue.ToString());
            reportParameters.Add("GUIDatabasesID", selectedDatabase.DataValue.ToString());
            if (selectedInstance.DataValue is MonitoredSqlServer) reportParameters.Add("GUIServerID", ((MonitoredSqlServer)selectedInstance.DataValue).Id.ToString());

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
                //object[] parameters = null;
                string selectedServer = null;
                string selectedDatabase = null;
                //int selectedTag = -1;
                DateTime startTime = new DateTime();
                DateTime endTime = new DateTime();
                
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
                localReportData.serverIDs = servers;

                if (reportParameters.ContainsKey("GUIServer") && reportParameters["GUIServer"] is String)
                {
                    selectedServer = reportParameters["GUIServer"].ToString();
                }
                if (reportParameters.ContainsKey("GUIDatabase") && reportParameters["GUIDatabase"] is String)
                {
                    selectedDatabase = reportParameters["GUIDatabase"].ToString();
                }
                ReportDataSource dataSource = new ReportDataSource("mirroringHistory");;

                passthroughParameters.Clear();
                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
                passthroughParameters.Add(new ReportParameter("Period", (string)localReportData.reportParameters["Period"]));
                if (localReportData.reportParameters.ContainsKey("rsStart")) passthroughParameters.Add(new ReportParameter("rsStart",
                                                                  localReportData.reportParameters["rsStart"].ToString()));
                if (localReportData.reportParameters.ContainsKey("rsEnd")) passthroughParameters.Add(new ReportParameter("rsEnd",
                                                                  localReportData.reportParameters["rsEnd"].ToString()));

                Log.Debug("localReportData.reportType = ", localReportData.reportType);
                localReportData.dataSources = new ReportDataSource[1];

                startTime = localReportData.dateRanges[0].UtcStart;
                endTime = localReportData.dateRanges[0].UtcEnd;

                dataSource.Value = RepositoryHelper.GetReportData("p_GetMirroringHistory",
                                               chkProblemsOnly.Checked,
                                               selectedServer,
                                               localReportData.dateRanges[0].UtcStart,
                                               localReportData.dateRanges[0].UtcEnd,
                                               selectedDatabase,
                                               GetServerIdXml(localReportData.serverIDs),
                                               localReportData.dateRanges[0].UtcOffsetMinutes);
                
                localReportData.dataSources[0] = dataSource;

                passthroughParameters.Add(new ReportParameter("GUIServer", reportParameters.ContainsKey("GUIServer")?reportParameters["GUIServer"].ToString():"All Servers"));
                passthroughParameters.Add(new ReportParameter("ServerXML", GetServerIdXml(localReportData.serverIDs)));
                passthroughParameters.Add(new ReportParameter("GUITags", (string)localReportData.reportParameters["GUITags"]));
                passthroughParameters.Add(new ReportParameter("problemsOnly", localReportData.reportParameters["problemsOnly"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUIDatabase", reportParameters.ContainsKey("GUIDatabase") ? reportParameters["GUIDatabase"].ToString() : "All Databases"));
                passthroughParameters.Add(new ReportParameter("GUIDateRange", GetDateRange(localReportData.dateRanges[0].UtcStart, localReportData.dateRanges[0].UtcEnd, (int)localReportData.periodType)));

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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.MirroringHistory.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }
                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "MirroringHistory";
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
    }
}

