//-----------------------------------------------------------------------
// <copyright file="AlwaysOnStatistics.cs" company="Idera Technologies, Inc.">
//     Copyright (c) BBS Technologies, Inc..  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
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
    /// <summary>
    /// This is the User Interface that the user will be able to see when he/she wants 
    /// to create a new report related with AlwaysOn statistics.
    /// </summary>
    public partial class AlwaysOnStatistics : ReportContol
    {
        private static readonly Logger Logger = Logger.GetLogger("AlwaysOn Statistics");
        private const string SelectServer = "< Select a Server >";
        private const string AllText = "< All >";
        private const string NoAvailabilityGroup = "< No Availability Groups >";
        private const string SelectAvailabilityGroup = "< Select an Availability Group >";

        private const string UniqueBaseValueText = "00000000-0000-0000-0000-000000000000";
        
        private const string GuiServersText = "GUIServers";
        private const string GuiGroupsText = "GUIGroups";
        private const string GuiTagsText = "GUITags";
        private const string ServerXmlText = "ServerXML";
        private const string GuiTagIdText = "GUITagID";
        private const string GuiGroupIdText = "GUIGroupID";
        private const string PeriodText = "Period";
        private const string UtcOffsetText = "UTCOffset";
        private const string GuiServerIdText = "GUIServerID";
        private const string StartText = "rsStart";
        private const string EndText = "rsEnd";
        private const string StartHoursText = "rsStartHours";
        private const string EndHoursText = "rsEndHours";
        private const string ExecutionStartText = "executionStart";
        private const string IntervalText = "Interval";
        private const string ChartParameterText = "ChartParameter";
        
        private const string alwaysOnStatisticsRdl = 
            "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.AlwaysOnStatistics.rdl";
        private const string alwaysOnDatabaseStatisticsRdl =
            "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.AlwaysOnDatabaseStatistics.rdl";

        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(5);
        protected ValueListItem availabilityGroupSelectOne;
        protected ValueListItem chartTypeSelectOne;

        /// <summary>
        /// This is the constructor of the class. 
        /// This will Initialize the components of this report view and will Adapt the font size.
        /// </summary>
        public AlwaysOnStatistics()
        {
            InitializeComponent();
            base.AdaptFontSize();
        }

        /// <summary>
        /// It is to initialize the report and set the ReportType as AlwaysOnStatistics.
        /// It is important to have initialized the availabilityGroupCombo before initialized
        /// the instanceCombo in order to avoid an unhandled exception. This is because the user
        /// will able to select a monitored server based on AlwaysOn Availability Group that 
        /// the user selected.
        /// </summary>
        public override void InitReport()
        {
            base.InitReport();
            InitializeReportViewer();
            ReportType = ReportTypes.AlwaysOnStatistics;
        }

        /// <summary>
        /// This method is to fill the availabilityGroupCombo with the current Availability Group
        /// available in SQLdm Repository.
        /// This method will fill with NoAlwaysOnServers if there is any instance to select into 
        /// the InstanceCombo or will try to fill with the result of the store procedure 
        /// p_GetAlwaysOnAGBasedActiveServers.
        /// </summary>
        protected void InitAvailabilityGroupCombo()
        {
            availabilityGroupCombo.Items.Clear();

            DataTable availabilityGroupTable = new DataTable();
            
            int tagId = GetTagId();

            //Calling to the store procedurer and geting the result into as DataTable
            availabilityGroupTable = 
                RepositoryHelper.GetAlwaysOnAGBasedOnActiveServers(
                    Settings.Default.ActiveRepositoryConnection.ConnectionInfo, 
                    tagId
                );
            
            FillAvailabilityGroupCombo(availabilityGroupTable);

            availabilityGroupCombo.SelectedIndex = 0;
            InitChartTypeCombo();
        }

        /// <summary>
        /// Returns the current tag id that the user selected
        /// </summary>
        /// <returns>An integer that contains the tagId related with user selection</returns>
        private int GetTagId()
        {
            int tagId = 0;
            if (tagsComboBox.SelectedItem != null)
            {
                var tagItem = tagsComboBox.SelectedItem as ValueListItem;
                if (tagItem != null && tagItem.DataValue!=null)
                {
                    int.TryParse(tagItem.DataValue.ToString(), out tagId);
                }
            }
            return tagId;
        }

        /// <summary>
        /// Fills the availabilityGroupCombo with the correct information. 
        /// When the DataTable availabilityGroupTable contains only the 'No Availability Groups' 
        /// text, the availabilityGroupCombo will only be filled with this option.
        /// In the case that the availabilityGroupTable contains all the availability groups, 
        /// first the availabilityGroupCombo will be filled with 'Select an Availability Group' 
        /// text and then with the rest of the list that are in availabilityGroupTable
        /// </summary>
        /// <param name="availabilityGroupTable"></param>
        private void FillAvailabilityGroupCombo(DataTable availabilityGroupTable)
        {
            if (NoAvailabilityGroup.Equals(availabilityGroupTable.Rows[0][0].ToString()))
            {
                availabilityGroupSelectOne = new ValueListItem(null, NoAvailabilityGroup);
                availabilityGroupCombo.Items.Add(availabilityGroupSelectOne);
            }
            else
            {
                //Adding the default option indicating that a value in the filter is required
                availabilityGroupSelectOne = new ValueListItem(null, SelectAvailabilityGroup);
                availabilityGroupCombo.Items.Add(availabilityGroupSelectOne);

                foreach (DataRow dataRow in availabilityGroupTable.Rows)
                {
                    ValueListItem itemToAdd = new ValueListItem(
                        dataRow[0],
                        dataRow[0].ToString()
                        );
                    availabilityGroupCombo.Items.Add(itemToAdd);
                }
            }
        }

        /// <summary>
        /// Returns the current tagId selected by the user and as default it returns 0
        /// </summary>
        /// <returns>The tagId that the user was selected, by default it returns 0</returns>
        private int GetCurrentTagId()
        {
            int tagId = 0;
            var valueListItem = tagsComboBox.SelectedItem as ValueListItem;
            if (valueListItem != null && valueListItem.DataValue != null)
            {
                int.TryParse(valueListItem.DataValue.ToString(), out tagId);
            }
            return tagId;
        }

        /// <summary>
        /// This method is to fill the chartTypeCombo with the 4 options that the user will have
        /// at the moment of create a new report.
        /// This method will fill with NoChartType if there is any Availability Group.
        /// </summary>
        protected void InitChartTypeCombo()
        {
            chartTypeCombo.Items.Clear();
            ArrayList chartTypeItemsNames = new ArrayList()
                                                {
                                                    "Redo Rate KB/s",
                                                    "Redo Queue Size KB",
                                                    "Log Send Rate KB/s",
                                                    "Log Send Queue Size KB"
                                                };

            foreach (string itemName in chartTypeItemsNames)
            {
                ValueListItem itemToAdd = new ValueListItem(itemName, itemName);
                chartTypeCombo.Items.Add(itemToAdd);
            }

            chartTypeCombo.SelectedIndex = 0;
        }

        /// <summary>
        /// This method is to reset all the filters criteria tha the report has
        /// </summary>
        public override void ResetFilterCriteria()
        {
            base.ResetFilterCriteria();
            availabilityGroupCombo.SelectedIndex = 0;
            chartTypeCombo.SelectedIndex = 0;
            ResetTimeFilter();
        }

        /// <summary>
        /// Reset StartHours and EndHours
        /// </summary>
        private void ResetTimeFilter()
        {
            // If customDates is null then we dont need to set startHoursTimeEditor and 
            // endHoursTimeEditor.
            if (customDates == null) return;

            startHoursTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            endHoursTimeEditor.Time = System.TimeSpan.Parse("23:00:00");
        }

        /// <summary>
        /// Method that is name after the user change the selected item into tags list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void tagsComboBox_SelectionChanged(object sender, EventArgs e)
        {
            InitAvailabilityGroupCombo();
            SetReportParameters();
        }

        /// <summary>
        /// Combo box of Servers for single select.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void instanceCombo_ValueChanged(object sender, EventArgs e)
        {
            if (instanceCombo.SelectedItem == null) return;
            ValueListItem item = instanceCombo.SelectedItem as ValueListItem;
            if (item != null)
            {
                currentServer = item.DataValue as MonitoredSqlServer;
            }

            reportData.serverIDs = null;
            if (instanceCombo.SelectedItem.ToString().ToLower() == AllText.ToLower())
            {
                //if we dont have server ids yet.
                if (reportData.serverIDs == null)
                {
                    //get the list of server ids from the server combo box.
                    List<int> servers = new List<int>();

                    IEnumerator enumerator = instanceCombo.Items.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ValueListItem listItem = enumerator.Current as ValueListItem;
                        if (listItem != null 
                            && !listItem.DisplayText.ToLower().Equals(AllText.ToLower()))
                        {
                            MonitoredSqlServer monitoredSqlServer = 
                                (enumerator.Current as ValueListItem).DataValue 
                                as MonitoredSqlServer;
                            if (monitoredSqlServer != null)
                            {
                                servers.Add(monitoredSqlServer.Id);
                            }
                        }
                    }
                    reportData.serverIDs = servers;
                }
            }
        }

        /// <summary>
        /// This method is to fill the instanceCombo with the current Monitored servers available
        /// in SQLdm Repository.
        /// This method will fill with all instance that returns .
        /// RepositoryHelper.GetMonitoredAlwaysOnServers based into the tag selected by the user.
        /// </summary>
        protected override void InitInstanceCombo()
        {
            instanceCombo.Items.Clear();
            ValueListItem[] instances = null;

            int alwaysOnInstances = 0;

            int tagId = GetCurrentTagId();
            string availabilityGroupName = availabilityGroupCombo.SelectedItem.ToString();

            //Verify if it is necesary to call to the RepositoryHelper.GetMonitoredAlwaysOnServers
            // method
            if (NoAvailabilityGroup.Equals(availabilityGroupName) || SelectAvailabilityGroup.Equals(availabilityGroupName))
            {
                instanceSelectOne = new ValueListItem(null, SelectServer);
                instanceCombo.Items.Add(instanceSelectOne);
            }
            else
            {
                //get all servers that have AlwaysOn relationships.
                List<Pair<int, string>> alwaysOnServers =
                    RepositoryHelper.GetMonitoredAlwaysOnServers(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                        tagId, availabilityGroupName
                        );

                var filtered = GetActiveServersFiltered(alwaysOnServers, alwaysOnInstances);

                instanceSelectOne = new ValueListItem(null, filtered.Length > 0
                    ? AllText
                    : SelectServer);

                instanceCombo.Items.Add(instanceSelectOne);
                instanceCombo.Items.AddRange(filtered);
            }
            instanceCombo.SelectedIndex = 0;
        }

        /// <summary>
        /// This method is to create a new ValueListItem array that contains all the join between
        /// active servers and alwaysOnServers list.
        /// </summary>
        /// <param name="alwaysOnServers"></param>
        /// <param name="alwaysOnInstances"></param>
        /// <returns>An array of ValueListItem that mach with the join with the active instance
        /// and the alwaysOnServers list. </returns>
        private ValueListItem[] GetActiveServersFiltered(List<Pair<int, string>> alwaysOnServers,
            int alwaysOnInstances)
        {
            ValueListItem[] instances;
            int i = 0;
            //How many active instances do we have?
            int instanceCount = ApplicationModel.Default.AllInstances.Count;
            instances = new ValueListItem[instanceCount];

            //iterate through the list of instances that are available to this user.
            foreach (MonitoredSqlServer server in ApplicationModel.Default.AllInstances.Values)
            {
                foreach (Pair<int, string> pair in alwaysOnServers)
                {
                    //if the server is hosting AlwaysOn
                    if (pair.First == server.Id)
                    {
                        //save the instance to the instances array
                        ValueListItem instance = new ValueListItem(server, server.InstanceName);
                        currentServer = server;
                        alwaysOnInstances++;
                        instances[i++] = instance;
                        break;
                    }
                }
            }
            //The filtered list is the list containing only AlwaysOn host that the user has 
            //access to.
            ValueListItem[] filtered = new ValueListItem[alwaysOnInstances];
            int alwaysOnInstancePtr = 0;
            for (i = 0; i < instances.Length; i++)
            {
                if (instances[i] != null)
                {
                    filtered[alwaysOnInstancePtr] = instances[i];
                    alwaysOnInstancePtr++;
                }
            }
            return filtered;
        }

        /// <summary>
        /// This method validates if it is possible to create the report or not. 
        /// If it is not possible to create the report, this method will create a message 
        /// refering this.
        /// </summary>
        /// <param name="message">The message that show if it is not able to run the report. 
        /// The default is String.Empty</param>
        /// <returns>true if it is any message and false if it is not possible run the report. 
        /// </returns>
        public override bool CanRunReport(out string message)
        {
            message = String.Empty;

            if (SelectAvailabilityGroup.Equals(availabilityGroupCombo.SelectedItem.ToString()))
            {
                message = "An availability group must be selected to generate this report.";
                return false;
            }

            if (NoAvailabilityGroup.Equals(availabilityGroupCombo.SelectedItem.ToString()))
            {
                message = "No availability groups were found. This report populates exclusively" +
                          " with Servers that have availability groups.";
                return false;
            }

            if (instanceCombo.SelectedIndex == 0)
            {
                ValueListItem item = instanceCombo.SelectedItem as ValueListItem;
                bool blnNoAlwaysOnDefined = item != null 
                    && (SelectServer.Equals(item.DisplayText));

                if (blnNoAlwaysOnDefined)
                {
                    message = "No server were found. This report populates exclusively " +
                              "with Servers that have AlwaysOn availability groups.";
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Set all the parameters that this report needs to run into the reportParameters
        /// </summary>
        protected override void SetReportParameters()
        {
            base.SetReportParameters();

            IList<int> servers;
            if (reportData.serverIDs != null)
            {
                servers = reportData.serverIDs;
            }
            else
            {
                servers = new List<int>(ApplicationModel.Default.AllInstances.Count);
                foreach (MonitoredSqlServer server in 
                    ApplicationModel.Default.AllInstances.Values)
                {
                    servers.Add(server.Id);
                }
            }
            reportData.serverIDs = servers;

            ValueListItem selectedTag = tagsComboBox.SelectedItem as ValueListItem;
            if (selectedTag == null)
            {
                selectedTag = new ValueListItem(0, AllText);
            }
            else
            {
                if (tagsComboBox.SelectedIndex == 0) selectedTag = new ValueListItem(0, AllText);
                if ((int)selectedTag.DataValue == 0) selectedTag = new ValueListItem(0, AllText);
            }

            ValueListItem selectedInstance = instanceCombo.SelectedItem as ValueListItem;
            if (selectedInstance == null)
            {
                selectedInstance = new ValueListItem(0, AllText);
            }
            else
            {
                if (selectedInstance.DataValue == null)
                {
                    selectedInstance = new ValueListItem(0, AllText);
                }
                if (instanceCombo.SelectedIndex == 0)
                {
                    selectedInstance = new ValueListItem(0, AllText);
                }
            }

            ValueListItem selectedAvailabilityGroup = availabilityGroupCombo.SelectedItem 
                as ValueListItem;

            reportParameters.Add(GuiTagsText, selectedTag.DisplayText);
            reportParameters.Add(GuiTagIdText, selectedTag.DataValue.ToString());
            if (selectedAvailabilityGroup != null && selectedAvailabilityGroup.DisplayText!= null)
            {
                reportParameters.Add(GuiGroupsText, selectedAvailabilityGroup.DisplayText);                
            }
            if (selectedAvailabilityGroup != null && selectedAvailabilityGroup.DataValue != null)
            {
                reportParameters.Add(
                    GuiGroupIdText, 
                    selectedAvailabilityGroup.DataValue.ToString()
                );
            }

            reportParameters.Add(PeriodText, ((int)reportData.periodType).ToString());
            reportParameters.Add(
                UtcOffsetText, 
                reportData.dateRanges[0].UtcOffsetMinutes.ToString()
            );
            reportParameters.Add(IntervalText, ((int)reportData.sampleSize).ToString());
            ValueListItem chartTypeItem = chartTypeCombo.SelectedItem as ValueListItem;
            if (chartTypeItem != null)
            {
                reportParameters.Add(ChartParameterText, chartTypeItem.DisplayText);
            }
            MonitoredSqlServer monitoredSqlServer = selectedInstance.DataValue 
                as MonitoredSqlServer;
            if (monitoredSqlServer != null)
            {
                reportParameters.Add(GuiServerIdText, monitoredSqlServer.Id.ToString());
            }
            if (instanceCombo.SelectedItem != null)
            {
                if (!AllText.ToLower().Equals(instanceCombo.SelectedItem.ToString().ToLower()))
                {
                    reportParameters.Add(GuiServersText, instanceCombo.SelectedItem.ToString());
                }
            }

            if (periodCombo.SelectedItem != periodSetCustom)
            {
                reportParameters.Add(StartText, DateTime.Today.ToString("yyyy'-'MM'-'dd"));
                reportParameters.Add(EndText, DateTime.Today.ToString("yyyy'-'MM'-'dd"));
            }
            else
            {
                reportParameters.Add(StartText, 
                    reportData.dateRanges[0].UtcStart.ToLocalTime().ToString("yyyy'-'MM'-'dd"));
                reportParameters.Add(EndText, 
                    reportData.dateRanges[0].UtcEnd.ToLocalTime().ToString("yyyy'-'MM'-'dd"));
            }
            reportParameters.Add(StartHoursText, "0");
            reportParameters.Add(EndHoursText, "23");
        }

        /// <summary>
        /// This method is returning the report parameters.  
        /// The names must match what is set in the RDL file.
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, object> GetReportParmeters()
        {
            SetReportParameters();
            return reportParameters;
        }

        /// <summary>
        /// This is a event that runs to do the work into the backgrandworker.
        /// This will set the passthroughParameters with parameters that the report needs to run
        /// </summary>
        /// <param name="sender">The parameter that contains the object that generate the event
        /// </param>
        /// <param name="e">This is the event that has the WorkerData as Argument and will be 
        /// filled with the dataSources</param>
        protected override void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null)
            {
                System.Threading.Thread.CurrentThread.Name = "ReportControlWorker";
            }

            using (Logger.DebugCall())
            {
                var localReportData = LoadReportData(e);

                if (localReportData.cancelled)
                {
                    Logger.Debug("reportData.cancelled = true.");
                    e.Cancel = true;
                }
                else
                {
                    e.Result = localReportData;
                }
            }
        }

        /// <summary>
        /// Set the passthroughParameters with parameters that the report needs to run
        /// </summary>
        /// <param name="e">Event that contains the WorkerData that we will use to set the 
        /// parameters and the datasource </param>
        /// <returns>The WorkerData</returns>
        private WorkerData LoadReportData(DoWorkEventArgs e)
        {
            WorkerData localReportData = (WorkerData)e.Argument;
            DateTime startTime = new DateTime();
            DateTime endTime = new DateTime();

            int serverId = 0;

            if (localReportData.reportParameters.Count > 0 
                && localReportData.reportParameters.ContainsKey(GuiServerIdText))
            {
                int.TryParse(
                    localReportData.reportParameters[GuiServerIdText].ToString(),
                    out serverId
                );
            }
            if (serverId != 0)
            {
                localReportData.serverIDs.Clear();
                localReportData.serverIDs.Add(serverId);
            }
            
            string selectedAvailabilityGroup = AllText;
            if (reportParameters.ContainsKey(GuiGroupsText) && reportParameters[GuiGroupsText] 
                is String)
            {
                selectedAvailabilityGroup = reportParameters[GuiGroupsText].ToString();
            }

            Logger.Debug("localReportData.reportType = ", localReportData.reportType);
            localReportData.dataSources = new ReportDataSource[1];

            startTime = localReportData.dateRanges[0].UtcStart;
            endTime = localReportData.dateRanges[0].UtcEnd;

            SetLocalReportDataSource(selectedAvailabilityGroup, localReportData);

            passthroughParameters.Clear();
            passthroughParameters.Add(
                new ReportParameter(ExecutionStartText, DateTime.Now.ToString()));
            passthroughParameters.Add(
                new ReportParameter(IntervalText, ((int)reportData.sampleSize).ToString()));
            passthroughParameters.Add(
                new ReportParameter(
                    PeriodText, 
                    (string)localReportData.reportParameters[PeriodText]
                )
            );
            if (localReportData.reportParameters.ContainsKey(StartText))
            {
                passthroughParameters.Add(
                    new ReportParameter(
                        StartText,
                        localReportData.reportParameters[StartText].ToString()
                    )
                );
            }
            if (localReportData.reportParameters.ContainsKey(EndText))
            {
                passthroughParameters.Add(
                    new ReportParameter(
                        EndText,
                        localReportData.reportParameters[EndText].ToString()
                    )
                );
            }
            passthroughParameters.Add(
                new ReportParameter(
                    GuiTagsText, 
                    (string)localReportData.reportParameters[GuiTagsText])
                );
            passthroughParameters.Add(
                new ReportParameter(
                    GuiServersText, 
                    reportParameters.ContainsKey(GuiServersText)
                        ? reportParameters[GuiServersText].ToString()
                        : AllText
                )
            );
            passthroughParameters.Add(
                new ReportParameter(ServerXmlText, GetServerIdXml(serverId)));
            passthroughParameters.Add(
                new ReportParameter(GuiServerIdText, serverId.ToString()));
            passthroughParameters.Add(
                new ReportParameter(
                    GuiGroupsText, 
                    reportParameters.ContainsKey(GuiGroupsText)
                        ? reportParameters[GuiGroupsText].ToString()
                        : AllText
                )
            );
            passthroughParameters.Add(
                new ReportParameter(
                    GuiGroupIdText, 
                    reportParameters.ContainsKey(GuiGroupIdText)
                        ? reportParameters[GuiGroupIdText].ToString()
                        : UniqueBaseValueText
                )
            );
            if (reportParameters.ContainsKey(ChartParameterText))
            {
                passthroughParameters.Add(
                    new ReportParameter(
                        ChartParameterText, 
                        reportParameters[ChartParameterText].ToString()
                    )
                );
            }
            return localReportData;
        }

        /// <summary>
        /// Load the localReportData.dataSources[0] with the result of the execution of 
        /// 'p_GetAlwaysOnStatistics'
        /// </summary>
        /// <param name="selectedAvailabilityGroup">The string that represents the Availability 
        /// group selected by the user</param>
        /// <param name="localReportData">The WorkerData that we will fill its datasource</param>
        private static void SetLocalReportDataSource(string selectedAvailabilityGroup
            , WorkerData localReportData)
        {
            ReportDataSource dataSource = new ReportDataSource("GetAlwaysOnStatistics");
            dataSource.Value = RepositoryHelper.GetReportData(
                "p_GetAlwaysOnStatistics",
                GetServerIdXml(localReportData.serverIDs),
                selectedAvailabilityGroup,
                localReportData.dateRanges[0].UtcStart,
                localReportData.dateRanges[0].UtcEnd,
                localReportData.dateRanges[0].UtcOffsetMinutes,
                (int)localReportData.sampleSize
                );

            localReportData.dataSources[0] = dataSource;
        }

        /// <summary>
        /// This method will run after the report process finish the DoWork
        /// If there is an error into the event 'e', this method will log it.
        /// If there is any error, this method will create an Stream with the 
        /// AlwaysOnTopology.rdl and sent the necesary parameters to run the report
        /// </summary>
        /// <param name="sender">The parameter that contains the object that generate the event
        /// </param>
        /// <param name="e">The event that could contains the error to be logged</param>
        protected override void bgWorker_RunWorkerCompleted(object sender
            , RunWorkerCompletedEventArgs e)
        {
            using (Logger.DebugCall())
            {
                // Make sure this call is for the most recently requested report.
                Logger.Debug("(reportData.bgWorker == sender) = ", reportData.bgWorker == sender);
                if (reportData.bgWorker == sender)
                {
                    // This event handler was called by the currently active report
                    if (reportData.cancelled)
                    {
                        Logger.Debug("reportData.cancelled = true.");
                        return;
                    }
                    if (e.Error != null)
                    {
                        LogError(e);

                        State = UIState.NoDataAcquired;
                    }
                    else
                    {
                        FillReportViewer();
                    }
                }
            }
        }

        /// <summary>
        /// Fills the ReportViewer with the last information related with the report and shows 
        /// this information in the current view
        /// </summary>
        private void FillReportViewer()
        {
            try
            {
                reportViewer.Reset();
                reportViewer.LocalReport.EnableHyperlinks = true;

                using (Stream stream = GetType().Assembly.GetManifestResourceStream(
                    alwaysOnStatisticsRdl))
                {
                    reportViewer.LocalReport.LoadReportDefinition(stream);
                }

                using (Stream stream = GetType().Assembly.GetManifestResourceStream(
                    alwaysOnDatabaseStatisticsRdl))
                {
                    reportViewer.LocalReport.LoadSubreportDefinition("Availability Group Database Statistics", stream);
                    //Create the method that will be calling when happen a  Subreport processing event
                    this.reportViewer.LocalReport.SubreportProcessing +=
                        LocalReport_SubreportProcessing;
                }

                foreach (ReportDataSource dataSource in reportData.dataSources)
                {
                    reportViewer.LocalReport.DataSources.Add(dataSource);
                }
                reportViewer.LocalReport.SetParameters(passthroughParameters);

                reportViewer.RefreshReport();
                reportViewer.LocalReport.DisplayName = "AlwaysOnStatistics";
                State = UIState.Rendered;
            }
            catch (Exception exception)
            {
                ApplicationMessageBox.ShowError(
                    ParentForm, 
                    "An error occurred while refreshing the report.", 
                    exception
                );
                State = UIState.ParmsNeeded;
            }
        }

        /// <summary>
        /// Log the error that could be found during the bgWorker_DoWork process
        /// </summary>
        /// <param name="e">The event that could contains the error to be logged</param>
        private void LogError(RunWorkerCompletedEventArgs e)
        {
            if (e.Error.GetType() == typeof(System.Data.SqlClient.SqlException) &&
                e.Error.Message.ToLower().Contains("msxmlsql.dll")) //
            {
                ApplicationMessageBox msgbox1 = new ApplicationMessageBox();
                Exception msg = new Exception(
                    "An error occurred while retrieving data for the report.  " +
                    "It may be due to the problem described by the article available at " +
                    "http://support.microsoft.com/Default.aspx?kbid=918767", e.Error
                    );
                Logger.Error("Showing message box: ", msg);
                msgbox1.Message = msg;
                msgbox1.SetCustomButtons("OK", "View Article");
                msgbox1.Symbol = Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol.Error;
                msgbox1.Show(this);
                if (msgbox1.CustomDialogResult ==
                    Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDialogResult.Button2)
                {
                    Process.Start("http://support.microsoft.com/Default.aspx?kbid=918767");
                }
            }
            else
            {
                ApplicationMessageBox.ShowError(
                    this,
                    "An error occurred while retrieving data for the report.  ", e.Error);
            }
        }

        /// <summary>
        /// Method that runs after the user change the period selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Set the start hours when th e selection is custom
        /// </summary>
        private void SetStartHours()
        {
            DateTime first = customDates[0].UtcStart.ToLocalTime();
            DateTime final = new DateTime(
                first.Year,
                first.Month,
                first.Day,
                startHoursTimeEditor.Time.Hours,
                startHoursTimeEditor.Time.Minutes,
                startHoursTimeEditor.Time.Seconds
            );
            customDates[0].UtcStart = final.ToUniversalTime();
        }

        /// <summary>
        /// Set the end hours when th e selection is custom
        /// </summary>
        private void SetEndHours()
        {
            DateTime last = customDates[customDates.Count - 1].UtcEnd.ToLocalTime();
            DateTime final = new DateTime(
                last.Year,
                last.Month,
                last.Day,
                endHoursTimeEditor.Time.Hours,
                endHoursTimeEditor.Time.Minutes,
                endHoursTimeEditor.Time.Seconds
            );
            customDates[customDates.Count - 1].UtcEnd = final.ToUniversalTime();
        }

        /// <summary>
        /// This method is called after the user change the value of the startHoursTimeEditor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startHoursTimeEditor_ValueChanged(object sender, EventArgs e)
        {
            SetStartHours();
        }

        /// <summary>
        /// This method is called after the user change the value of the endHoursTimeEditor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void endHoursTimeEditor_ValueChanged(object sender, EventArgs e)
        {
            SetEndHours();
        }

        /// <summary>
        /// This method is called after the user generate a new report and try to do click 
        /// in order to generate a new report. In this case is when the user has generated a 
        /// AlwaysOn Statistics report and try to access to a Topology report doing a click into 
        /// an Availability group link.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void reportViewer_Drillthrough(object sender, DrillthroughEventArgs e)
        {
            using (Logger.DebugCall())
            {
                e.Cancel = true; // We'll handle this our way.
                Logger.Root.ConsoleTraceLevel = BBS.TracerX.TraceLevel.Verbose;
                Logger.Debug("e.ReportPath = ", e.ReportPath);

                if (e.ReportPath == ReportsHelper.GetReportTitle(ReportTypes.AlwaysOnTopology))
                {
                    ApplicationController.Default.ShowReportsView(ReportTypes.AlwaysOnTopology, e);
                }
            }
        }

        /// <summary>
        /// This event is triggered for every instance of the subreport in the main report. It will
        /// contain the necessary process to call the subreport that a report has.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LocalReport_SubreportProcessing(object sender,
            SubreportProcessingEventArgs e)
        {
            string selectedAvailabilityGroup = reportParameters[GuiGroupsText].ToString();
            
            ReportDataSource dataSource = new ReportDataSource("GetAlwaysOnDatabaseStatistics");
            dataSource.Value = RepositoryHelper.GetReportData(
                "p_GetAlwaysOnDatabaseStatistics",
                e.Parameters[0].Values[0],
                e.Parameters[1].Values[0],
                e.Parameters[2].Values[0],
                e.Parameters[3].Values[0],
                e.Parameters[4].Values[0],
                e.Parameters[5].Values[0],
                e.Parameters[6].Values[0]
                );

            e.DataSources.Add(dataSource);
        }

        /// <summary>
        /// This event is to update the intaceCombo based on an availability group selected by the
        /// user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void availabilityGroupCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitInstanceCombo();
        }
    }
}

