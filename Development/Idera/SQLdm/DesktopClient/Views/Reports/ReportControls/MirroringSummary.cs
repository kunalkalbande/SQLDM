using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using BBS.TracerX;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class MirroringSummary :ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("Mirroring Summary");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(5);
        private const string SelectAServer = "< All Mirrored Servers >";
        private const string NoMirrorServers = "< No Mirrored Servers >";

        public MirroringSummary()
        {
            InitializeComponent();
            AdaptFontSize();
        }
        
        public override void InitReport()
        {
            base.InitReport();
            InitializeReportViewer();
            ReportType = ReportTypes.MirroringSummary;
        }

        public override void ResetFilterCriteria()
        {
            base.ResetFilterCriteria();
            chkProblemsOnly.Checked = false;
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
            //This must be sorted for the binarysearch to work
            mirroringServers.Sort();

            CaseInSensitiveEqualityComparer insensitiveComparer = new CaseInSensitiveEqualityComparer();

            //if a tag has been selected
            if ((tagsComboBox.SelectedItem != null) && (tagsComboBox.SelectedItem != tagSelectOne))
            {
                int intSelectedTagID = (int) (((ValueListItem) tagsComboBox.SelectedItem).DataValue);
                
                //get the number of instances associated with this tag
                instanceCount = GetInstanceCount(intSelectedTagID);
                
                //create a valuelist array to hold all of these instances
                instances = new ValueListItem[instanceCount];

                //get the tag object
                Tag tag = GetTag(intSelectedTagID);
                if (tag == null)
                {
                    instanceSelectOne = new ValueListItem(null, mirroringServers.Count > 0 ? SelectAServer : NoMirrorServers);
                    instanceCombo.Items.Add(instanceSelectOne);

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
                    if (mirroringServers.BinarySearch(server.InstanceName, insensitiveComparer) >= 0)
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
                    }else
                    {
                        instances[i++] = null;    
                    }
                }
            }
            else//if no tag has been selected
            {
                //How many active instances do we have?
                instanceCount = ApplicationModel.Default.ActiveInstances.Count;
                instances = new ValueListItem[instanceCount];
                i = 0;
                foreach (MonitoredSqlServer server in ApplicationModel.Default.ActiveInstances)
                {
                    //if the server is hosting mirroring
                    if (mirroringServers.BinarySearch(server.InstanceName, insensitiveComparer) >= 0)
                    {
                        instance = new ValueListItem(server, server.InstanceName);

                        if ((serverID != -1) && (serverID == server.Id))
                        {
                            currentServer = server;
                        }
                        mirroredInstances++;
                        instances[i++] = instance;
                    }else
                    {
                        instances[i++] = null;
                    }
                }
            }
            ValueListItem[] filtered = new ValueListItem[mirroredInstances];
            int mirrorInstancePtr = 0;
            for (i = 0; i < instances.Length; i++)
            {
                if(instances[i] != null)
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
                bool blnNoMirroringDefined = (((ValueListItem)instanceCombo.SelectedItem).DisplayText == NoMirrorServers);

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
            base.SetReportParameters();
            reportParameters.Add("problemsOnly", chkProblemsOnly.Checked);

            ValueListItem selectedTag = (ValueListItem)tagsComboBox.SelectedItem ?? 
                                            new ValueListItem(0, "All Tags");
            ValueListItem selectedInstance = (ValueListItem)instanceCombo.SelectedItem ??
                                             new ValueListItem(0, "All Servers");

            

            if (instanceCombo.SelectedIndex == 0) selectedInstance = new ValueListItem(0, "All Servers");
            if (instanceCombo.SelectedIndex > 0)
            {
                reportParameters.Add("Server", selectedInstance);
                reportParameters.Add("GUIServer", selectedInstance.DisplayText);
                reportParameters.Add("GUIServerID", ((MonitoredSqlServer)selectedInstance.DataValue).Id);    
            }
            

            if (tagsComboBox.SelectedIndex == 0) selectedTag = new ValueListItem(0, "All Tags");
            if ((int)selectedTag.DataValue == 0) selectedTag = new ValueListItem(0, "All Tags");
            reportParameters.Add("GUITagID", selectedTag.DataValue);
            reportParameters.Add("GUITags", selectedTag.DisplayText);
            
        }

        /// <summary>
        /// This method is returning the report parameters.  The names must match what is set in the RDL file.
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, object> GetReportParmeters()
        {
            IList<int> servers = new List<int>();
            bool blnProblemsOnly = false;

            if (reportParameters.ContainsKey("problemsOnly"))
            {
                blnProblemsOnly = (bool)reportParameters["problemsOnly"];
            }

            if (instanceCombo.SelectedItem.ToString().ToLower() != SelectAServer.ToLower())
            {
                ValueListItem Server = (ValueListItem)instanceCombo.SelectedItem;
                servers.Add(((MonitoredSqlServer)Server.DataValue).Id);
                reportData.serverIDs = servers;
            }
            else
            {
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
            }
            reportData.serverIDs = servers;

            reportParameters.Clear();
            reportParameters.Add("ProblemsOnly", blnProblemsOnly.ToString());
            //reportParameters.Add("SQLServerIDs", GetServerIdXml(reportData.serverIDs));

            reportParameters.Add("GUIServerXML", GetServerIdXml(reportData.serverIDs));
            //reportParameters.Add("ServerXML", GetServerIdXml(reportData.serverIDs));

            ValueListItem selectedTag = (ValueListItem)tagsComboBox.SelectedItem ??
                                new ValueListItem(0, "All Tags");
            ValueListItem selectedInstance = (ValueListItem)instanceCombo.SelectedItem ??
                                             new ValueListItem(0, "All Servers");



            if (instanceCombo.SelectedIndex == 0) selectedInstance = new ValueListItem(0, "All Servers");
            if (instanceCombo.SelectedIndex > 0)
            {
                reportParameters.Add("Server", ((MonitoredSqlServer)selectedInstance.DataValue).Id.ToString());
                reportParameters.Add("GUIServer", selectedInstance.DisplayText);
            }
            else
            {
                reportParameters.Add("Server", "0");
            }


            if (tagsComboBox.SelectedIndex == 0) selectedTag = new ValueListItem(0, "All Tags");
            if ((int)selectedTag.DataValue == 0) selectedTag = new ValueListItem(0, "All Tags");
            reportParameters.Add("Tags", selectedTag.DataValue.ToString());
            reportParameters.Add("GUITags", selectedTag.DisplayText);


            return reportParameters;
        }

        override protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall())
            {
                WorkerData localReportData = (WorkerData)e.Argument;
                bool blnProblemsOnly = false;

                Log.Debug("localReportData.reportType = ", localReportData.reportType);
                localReportData.dataSources = new ReportDataSource[2];

                IList<int> servers = new List<int>();

                if (reportParameters.ContainsKey("problemsOnly"))
                {
                    blnProblemsOnly = (bool)reportParameters["problemsOnly"];
                }

                if ((reportParameters.ContainsKey("Server") && ((ValueListItem)reportParameters["Server"]).DataValue is MonitoredSqlServer))
                {
                    //localReportData.serverIDs.Clear();
                    ValueListItem Server = (ValueListItem)reportParameters["Server"];
                    servers.Add(((MonitoredSqlServer)Server.DataValue).Id);
                    localReportData.serverIDs = servers;
                } 
                else
                {
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
                }

                localReportData.serverIDs = servers;
                
                passthroughParameters.Clear();

                passthroughParameters.Add(new ReportParameter("ProblemsOnly", blnProblemsOnly.ToString()));
                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
                passthroughParameters.Add(new ReportParameter("GUIServer", reportParameters.ContainsKey("GUIServer") ? reportParameters["GUIServer"].ToString() : "All Servers"));
                passthroughParameters.Add(new ReportParameter("ServerXML", GetServerIdXml(localReportData.serverIDs)));
                passthroughParameters.Add(new ReportParameter("GUITags", (string)localReportData.reportParameters["GUITags"]));


                ReportDataSource dataSource = new ReportDataSource("mirroringCurrentStatus");
                dataSource.Value =
                    RepositoryHelper.GetReportData("p_GetMirroringServerStatus",
                                                    blnProblemsOnly,
                                                   GetServerIdXml(localReportData.serverIDs));

                localReportData.dataSources[0] = dataSource;
                
                dataSource = new ReportDataSource("mirroringServerConfiguration");

                dataSource.Value =
                    RepositoryHelper.GetReportData("p_GetMirroringSessions",
                                                   blnProblemsOnly,
                                                   GetServerIdXml(localReportData.serverIDs));

                localReportData.dataSources[1] = dataSource;

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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.MirroringOverview.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }
                            
                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "MirroringSummary";
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
    }
    #region EqualityComparer
    public class CaseInSensitiveEqualityComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return (new CaseInsensitiveComparer()).Compare(x, y);
        }
    }
    #endregion EqualityComparer

}

