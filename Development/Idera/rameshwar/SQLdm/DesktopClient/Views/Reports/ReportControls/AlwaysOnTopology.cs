//-----------------------------------------------------------------------
// <copyright file="AlwaysOnTopology.cs" company="Idera Technologies, Inc.">
//     Copyright (c) BBS Technologies, Inc..  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using BBS.TracerX;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    /// <summary>
    /// This is the User Interface that the user will be able to see when he/she wants to create 
    /// a new report related with AlwaysOn Topology report
    /// </summary>
    public partial class AlwaysOnTopology : ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("AlwaysOn Topology");
        private const string AllText = "< All >";
        private const string NoAvailabilityGroup = "< No Availability Groups >";
        private const string AvailabilityGroupText = "AvailabilityGroup";
        private const string ExecutionStartText = "executionStart";
        private const string alwaysOnTopologyRdl = 
            "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.AlwaysOnTopology.rdl";

        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(5);

        /// <summary>
        /// This is the constructor of the class. 
        /// This will Initialize the UI components of this report view and will Adapt the font 
        /// size. 
        /// </summary>
        public AlwaysOnTopology()
        {
            InitializeComponent();
            AdaptFontSize();
        }

        /// <summary>
        /// This method initialize the base report, the report viewer,
        /// and set the ReportType as ReportTypes.AlwaysOnTopology.
        /// This will Initialize the availabilityGroupCombo with the values optained with the 
        /// excecution of p_GetAlwaysOnAGBasedActiveServers store procedure.
        /// </summary>
        public override void InitReport()
        {
            base.InitReport();
            InitializeReportViewer();
            //TagId = 0 and serverId = 0 means that we will take ALL the availability groups,
            //it doesnt mother if there are repositories that aren't being monitored by SQLdm
            InitAvailabilityGroupCombo(0, 0);
            ReportType = ReportTypes.AlwaysOnTopology;
            if (drillThroughArguments != null)
            {
                RunReport(true);
            }
        }

        /// <summary>
        /// This method reset all the filters of the report. 
        /// In this case the only filter that we have is availabilityGroupCombo
        /// </summary>
        public override void ResetFilterCriteria()
        {
            base.ResetFilterCriteria();
            availabilityGroupCombo.SelectedIndex = 0;
        }

        /// <summary>
        /// This method initialize the items of availabilityGroupCombo. 
        /// The items will set from the result of the execution of p_GetAlwaysOnAGBasedActiveServers store 
        /// procedure.
        /// This result could be a No Availability Groups or a list of all availability groups 
        /// including the option 'All' as first item
        /// </summary>
        /// <param name="tagId">This is one of the parameters send to the p_GetAlwaysOnAGBasedActiveServers 
        /// store procedure. 
        /// If the value is 0, it means the store procedure will try to select all the 
        /// availability groups related with all the tags</param>
        /// <param name="serverId">This is second parameter send to the p_GetAlwaysOnAGBasedActiveServers
        /// store procedure. 
        /// If the value is 0, it means the store procedure will try to select all the 
        /// availability groups related with all the servers</param>
        private void InitAvailabilityGroupCombo(int tagId, int serverId)
        {
            availabilityGroupCombo.Items.Clear();
            string availabilityGroupName = string.Empty;

            //This property will be different of null when the call is from another report
            if (drillThroughArguments != null)
            {
                LocalReport localReport = drillThroughArguments.Report as LocalReport;
                if (localReport != null)
                {
                    IList<ReportParameter> paramsList =
                        localReport.OriginalParametersToDrillthrough;

                    bool hasParamListItems = paramsList != null && paramsList.Count > 0
                        && paramsList[0].Values != null && paramsList[0].Values.Count > 0
                        && !string.IsNullOrEmpty(paramsList[0].Values[0]);

                    availabilityGroupName = hasParamListItems 
                        ? paramsList[0].Values[0] : NoAvailabilityGroup;

                    GetAvailabilityGroupComboItems(availabilityGroupName.Equals(
                        NoAvailabilityGroup));
                }

                ValueListItem selectedItem = new ValueListItem(NoAvailabilityGroup
                    , NoAvailabilityGroup);
                foreach (ValueListItem item in availabilityGroupCombo.Items)
                {
                    if (item.DisplayText.Equals(availabilityGroupName))
                    {
                        selectedItem = item;
                        break;
                    }
                }
                availabilityGroupCombo.SelectedItem = selectedItem;
            }
            else
            {
                GetAvailabilityGroupComboItems(false);
                
                availabilityGroupCombo.SelectedIndex = 0;
            }

        }

        /// <summary>
        /// This method will  fill the availabilityGroupCombo.Items with ' No Availability Groups '
        /// text if isNoAvailabilityGroup is true or the DateTable availabilityGroupTable rows 
        /// count is less than 1, or will be filled with the result of the excecution of
        /// p_GetAlwaysOnAGBasedActiveServers store procedure.
        /// </summary>
        /// <param name="isNoAvailabilityGroup">This method will recive a isNoAvailabilityGroup 
        /// parameter that indicates if the Item  that are into availabilityGroupCombo is 
        /// ' No Availability Groups '. If you only want to have the result of the excecution of
        /// p_GetAlwaysOnAGBasedActiveServers store procedure, set this parameter as 'false'</param>
        private void GetAvailabilityGroupComboItems(bool isNoAvailabilityGroup)
        {
            availabilityGroupCombo.Items.Clear();
            DataTable availabilityGroupTable = new DataTable();

            //Calling to the store procedurer and geting the result into as DataTable
            availabilityGroupTable =
                RepositoryHelper.GetAlwaysOnAGBasedOnActiveServers(
                    Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                    0
                );
            FillAvailabilityGroupCombo(isNoAvailabilityGroup, availabilityGroupTable);
            availabilityGroupCombo.SelectedIndex = 0;
        }

        /// <summary>
        /// Fills the availabilityGroupCombo with the correct information. 
        /// When the DataTable availabilityGroupTable contains only the 'No Availability Groups' 
        /// text, the availabilityGroupCombo will only be filled with this option.
        /// In the case that the availabilityGroupTable contains all the availability groups, 
        /// first the availabilityGroupCombo will be filled with 'Select an Availability Group' 
        /// text and then with the rest of the list that are in availabilityGroupTable
        /// </summary>
        /// <param name="isNoAvailabilityGroup">This method will recive a isNoAvailabilityGroup 
        /// parameter that indicates if the Item  that are into availabilityGroupCombo is 
        /// ' No Availability Groups '. If you only want to have the result of the excecution of
        /// p_GetAlwaysOnAGBasedActiveServers store procedure, set this parameter as 'false'</param>
        /// <param name="availabilityGroupTable"></param>
        private void FillAvailabilityGroupCombo(bool isNoAvailabilityGroup, 
            DataTable availabilityGroupTable)
        {
            if (NoAvailabilityGroup.Equals(availabilityGroupTable.Rows[0][0].ToString())
                || isNoAvailabilityGroup)
            {
                ValueListItem itemToAdd = new ValueListItem(NoAvailabilityGroup
                                                            , NoAvailabilityGroup);
                availabilityGroupCombo.Items.Add(itemToAdd);
            }
            else
            {
                ValueListItem allItem = new ValueListItem(AllText, AllText);
                availabilityGroupCombo.Items.Add(allItem);

                foreach (DataRow dataRow in availabilityGroupTable.Rows)
                {
                    ValueListItem itemToAdd = new ValueListItem(dataRow[0]
                                                                , dataRow[0].ToString());
                    availabilityGroupCombo.Items.Add(itemToAdd);
                }
            }
        }

        /// <summary>
        /// This method determines if the report will be able to run or not. 
        /// If the availabilityGroupCombo selected item is No Availability Groups 
        /// this method will return false and a message with the corresponding explanation
        /// </summary>
        /// <param name="message">This message is empty if the report will be able to run. 
        /// In the other hand it will contain a message to show the user</param>
        /// <returns>This method will return true if the report could be run and false in the 
        /// other hand</returns>
        public override bool CanRunReport(out string message)
        {
            message = String.Empty;

            if (availabilityGroupCombo.SelectedIndex == 0)
            {
                ValueListItem selectedAvailabilityGroup = availabilityGroupCombo.SelectedItem 
                    as ValueListItem;
                bool blnNoAvailabilityGroupDefined = selectedAvailabilityGroup != null
                    && (selectedAvailabilityGroup.DisplayText == NoAvailabilityGroup);

                if (blnNoAvailabilityGroupDefined)
                {
                    message = "No availability groups were found. This report populates " +
                              "exclusively with Servers that have availability groups.";
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// This method set all the necesary parameters to send to the report designer into 
        /// reportParameters
        /// </summary>
        protected override void SetReportParameters()
        {
            reportParameters.Clear();
            base.SetReportParameters();

            ValueListItem selectedAvailabilityGroup = availabilityGroupCombo.SelectedItem 
                as ValueListItem ?? new ValueListItem(AllText, AllText);

            if (availabilityGroupCombo.SelectedIndex > 0)
            {
                selectedAvailabilityGroup = availabilityGroupCombo.SelectedItem as ValueListItem;
            }

            reportParameters.Add(AvailabilityGroupText, selectedAvailabilityGroup);
        }

        /// <summary>
        /// This method is returning the report parameters.  The names must match what is set in 
        /// the RDL file.
        /// </summary>
        /// <returns>This method return a filled reportParameters with the parameters 
        /// necesary to send to the report designer</returns>
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
        override protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null)
            {
                System.Threading.Thread.CurrentThread.Name = "ReportControlWorker";
            }

            using (Log.DebugCall())
            {
                var localReportData = LocalReportData(e);

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

        /// <summary>
        /// This method returns a WorkerData that contains the 2 dataSources with the result 
        /// of the excecution of p_GetAlwaysOnDatabases and p_GetAlwaysOnAvailabilityReplica 
        /// store procedures.
        /// This method will fill the passthroughParameters with the ExecutionStartText and 
        /// AvailabilityGroupText as ReportParameter.
        /// </summary>
        /// <param name="e">This is the event that has the WorkerData as Argument and will be 
        /// filled with the dataSources.</param>
        /// <returns></returns>
        private WorkerData LocalReportData(DoWorkEventArgs e)
        {
            WorkerData localReportData = (WorkerData)e.Argument;
            Log.Debug("localReportData.reportType = ", localReportData.reportType);
            localReportData.dataSources = new ReportDataSource[2];

            ValueListItem selectedAvailabilityGroup = new ValueListItem(AllText, AllText);
            if (reportParameters.ContainsKey(AvailabilityGroupText) 
                && reportParameters[AvailabilityGroupText] is ValueListItem)
            {
                selectedAvailabilityGroup = reportParameters[AvailabilityGroupText] 
                    as ValueListItem;
            }

            passthroughParameters.Clear();

            passthroughParameters.Add(
                new ReportParameter(ExecutionStartText, DateTime.Now.ToString()));
            passthroughParameters.Add(
                new ReportParameter(AvailabilityGroupText, selectedAvailabilityGroup.DisplayText));

            ReportDataSource dataSource = new ReportDataSource("GetAlwaysOnDatabases");
            dataSource.Value =
                RepositoryHelper.GetReportData("p_GetAlwaysOnDatabases",
                                               selectedAvailabilityGroup.DisplayText);

            localReportData.dataSources[0] = dataSource;

            dataSource = new ReportDataSource("GetAlwaysOnAvailabilityReplica");

            dataSource.Value =
                RepositoryHelper.GetReportData("p_GetAlwaysOnAvailabilityReplica",
                                               selectedAvailabilityGroup.DisplayText);

            localReportData.dataSources[1] = dataSource;
            return localReportData;
        }

        /// <summary>
        /// This method will run after the report process finish the DoWork.
        /// If there is an error into the event 'e', this method will log it.
        /// If there is any error, this method will create an Stream with the AlwaysOnTopology.rdl
        /// and sent the necesary parameters to run the report.
        /// </summary>
        /// <param name="sender">The parameter that contains the object that generate the event.
        /// </param>
        /// <param name="e">This is the event that has the WorkerData as Argument and will be 
        /// filled with the dataSources.</param>
        protected override void bgWorker_RunWorkerCompleted(object sender
            , RunWorkerCompletedEventArgs e)
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
                    if (e.Error != null)
                    {
                        LogError(e);
                    }
                    else
                    {
                        try
                        {
                            reportViewer.Reset();
                            reportViewer.LocalReport.EnableHyperlinks = true;

                            using (Stream stream = GetType().Assembly.GetManifestResourceStream(
                                alwaysOnTopologyRdl))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }

                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }

                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "AlwaysOnTopology";
                            State = UIState.Rendered;
                        }
                        catch (Exception exception)
                        {
                            ApplicationMessageBox.ShowError(
                                ParentForm
                                ,"An error occurred while refreshing the report."
                                , exception
                            );
                            State = UIState.ParmsNeeded;
                        }
                    }
                }
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
                Exception msg =
                    new Exception(
                        "An error occurred while retrieving data for the report.  " +
                        "It may be due to the problem described by the article available at " +
                        "http://support.microsoft.com/Default.aspx?kbid=918767",
                        e.Error);
                Log.Error("Showing message box: ", msg);
                msgbox1.Message = msg;
                msgbox1.SetCustomButtons("OK", "View Article");
                msgbox1.Symbol = Microsoft.SqlServer.MessageBox.ExceptionMessageBoxSymbol.Error;
                msgbox1.Show(this);
                if (msgbox1.CustomDialogResult 
                    == Microsoft.SqlServer.MessageBox.ExceptionMessageBoxDialogResult.Button2)
                {
                    Process.Start("http://support.microsoft.com/Default.aspx?kbid=918767");
                }
            }
            else
            {
                ApplicationMessageBox.ShowError(
                    this
                    , "An error occurred while retrieving data for the report.  "
                    , e.Error
                );
            }

            State = UIState.NoDataAcquired;
        }
    }
}

