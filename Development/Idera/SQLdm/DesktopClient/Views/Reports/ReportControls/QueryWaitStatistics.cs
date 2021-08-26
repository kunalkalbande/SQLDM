using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;
using Microsoft.SqlServer.MessageBox;
using Wintellect.PowerCollections;
using System.Data;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    /// <summary>
    /// SQLdm 8.6 (Vineet Kumar): Added for Query Wait Stats Report
    /// Added QueryWaitStatistics class for showing Query Wait Statistics Report
    /// </summary>
    public partial class QueryWaitStatistics : DatabaseReport
    {
        private static readonly Logger Log = Logger.GetLogger("Query Wait Statistics");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(6);
        private const string AllText = "< All >";
        private const string WaitCategoryText = "WaitCategory";
        public IList<int> waitCategoryIDs;
        public QueryWaitStatistics()
        {
            InitializeComponent();
            base.AdaptFontSize();
        }

        /// <summary>
        /// This is a validation method. Checks if required fields are entered or not.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override bool CanRunReport(out string message)
        {
            message = String.Empty;

            if (instanceCombo.SelectedIndex == 0)
            {
                message = "A SQL Server instance must be selected to generate this report.";
                return false;
            }

            if (Databases == null || Databases.Count == 0)
            {
                message = "A database must be selected to generate this report.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// This is initialisation method. Initialises reportviewer and categorycombo. It also sets reportType
        /// </summary>
        public override void InitReport()
        {
            base.InitReport();
            InitializeReportViewer();
            InitWaitCategoryCombo();
            Databases = null;
            if (drillThroughArguments != null)
            {
                try
                {
                    Databases = new List<string>();
                    Databases.Add(Convert.ToString(drillthroughParams["DatabaseNameFilter"][0]));
                    databaseTextbox.Text = Convert.ToString(drillthroughParams["DatabaseNameFilter"][0]);
                }
                catch
                {
                    Databases = null;
                }
            }
            ReportType = ReportTypes.QueryWaitStatistics;
            RunReport(true);
        }

        /// <summary>
        /// This method is used to set report parameters. For example if selection of waitcategory combo is changed, this method is called to set the parameters.
        /// </summary>
        protected override void SetReportParameters()
        {
            base.SetReportParameters();
            //setting SQL Server
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

            //setting database
            if (Databases != null && Databases.Count > 0)
            {
                reportParameters.Add("DatabaseNameFilter", Databases[0]);
            }
            else
                reportParameters.Add("DatabaseNameFilter", null);

            //setting period
            reportParameters.Add("Period", ((int)reportData.periodType).ToString());

            //setting wait category
            ValueListItem selectedwaitCategory = waitCategoryCombo.SelectedItem
               as ValueListItem ?? new ValueListItem(AllText, AllText);

            if (waitCategoryCombo.SelectedIndex > 0)
            {
                selectedwaitCategory = waitCategoryCombo.SelectedItem as ValueListItem;
                reportParameters.Add("WaitCategory", selectedwaitCategory.DataValue);
                reportParameters.Add("GUIWaitCategory", selectedwaitCategory.DisplayText);
            }
            else
            {
                reportParameters.Add("WaitCategory", 0);
                reportParameters.Add("GUIWaitCategory", AllText);
            }

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
        /// <summary>
        /// This method gets the report data and render it on UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        override protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall())
            {
                if (e.Argument != null)
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

                    dataSource = new ReportDataSource("QueryWaitStatistics");
                    dataSource.Value = RepositoryHelper.GetReportData("p_GetQueryWaitStatistics",
                                                                      localReportData.instanceID,
                                                                      (string)localReportData.reportParameters["DatabaseNameFilter"],
                                                                      localReportData.dateRanges[0].UtcStart,
                                                                      localReportData.dateRanges[0].UtcEnd,
                                                                      localReportData.dateRanges[0].UtcOffsetMinutes,
                                                                      (int)localReportData.reportParameters["WaitCategory"]
                                                                     );
                    localReportData.dataSources[0] = dataSource;

                    //Start : Setting parameters for rdl
                    passthroughParameters.Add(new ReportParameter("DatabaseNameFilter", (string)localReportData.reportParameters["DatabaseNameFilter"]));
                    passthroughParameters.Add(new ReportParameter("ServerId", localReportData.instanceID.ToString()));
                    passthroughParameters.Add(new ReportParameter("WaitCategory", localReportData.reportParameters["WaitCategory"].ToString()));
                    passthroughParameters.Add(new ReportParameter("GUIWaitCategory", localReportData.reportParameters["GUIWaitCategory"].ToString()));
                    passthroughParameters.Add(new ReportParameter("GUIServer", (string)localReportData.reportParameters["GUIServer"]));
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
        }


        /// <summary>
        /// This method is called once bgWorker_DoWork is completed. i.e. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                            e.Error.Message.ToLower().Contains("msxmlsql.dll"))
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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.QueryWaitStatistics.rdl"))
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
                            reportViewer.LocalReport.DisplayName = "QueryWaitStatistics";
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

        //This method clears database selection when selected instance is changed
        private void instanceCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            Databases = null;
        }

        //Resets filter criteria
        public override void ResetFilterCriteria()
        {
            base.ResetFilterCriteria();
            waitCategoryCombo.SelectedIndex = 0;
            Databases = null;
        }

        //Start : Wait Categories Combo
        void waitCategoryCombo_SelectedValueChanged(object sender, System.EventArgs e)
        {
            SetReportParameters();
        }

        //initialising wait category combo
        private void InitWaitCategoryCombo()
        {
            GetWaitCategoryComboItems();
            waitCategoryCombo.SelectedIndex = 0;
        }

        //This method gets the wait categories and bind the waitategory combo with those values.
        private void GetWaitCategoryComboItems()
        {
            waitCategoryCombo.Items.Clear();
            DataTable waitCategoryTable = new DataTable();

            //Calling to the store procedurer and geting the result into as DataTable
            waitCategoryTable =
                RepositoryHelper.GetWaitCategories(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            FillWaitCategoryCombo(waitCategoryTable);

        }

        /// <summary>
        /// This method fills the waitcategory combo with its values
        /// </summary>
        /// <param name="waitCategoryTable">Values to be filled with</param>
        private void FillWaitCategoryCombo(
               DataTable waitCategoryTable)
        {
            if (waitCategoryTable != null)
            {
                ValueListItem allItem = new ValueListItem(AllText, AllText);
                waitCategoryCombo.Items.Add(allItem);
                foreach (DataRow dataRow in waitCategoryTable.Rows)
                {
                    ValueListItem itemToAdd = new ValueListItem(dataRow[1]
                                                                , dataRow[0].ToString());
                    waitCategoryCombo.Items.Add(itemToAdd);
                }
            }
        }
        //End : Wait Categories Combo
    }
}

