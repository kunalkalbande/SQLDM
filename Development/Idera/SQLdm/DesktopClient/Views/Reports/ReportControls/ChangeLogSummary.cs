using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class ChangeLogSummary : ReportContol
    {
        private static readonly Logger Log = Logger.GetLogger("Change Log Summary");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(2);
        private ValueListItem actionSelectOne;
        private bool isFirstRendering = false;
        private const string AllTagText = "< All >";

        /// <summary>
        /// Populates data on specific combo boxes
        /// </summary>
        private delegate void InitCombo();

        public ChangeLogSummary()
        {
            InitializeComponent();
            base.AdaptFontSize();
        }

        public override void InitReport()
        {
            base.InitReport();
            InitActionTypeCombo();
            InitRepositoryUserCombo();
            InitWorkstationCombo();
            InitWorkstationUserCombo();
            InitializeReportViewer();
            ReportType = ReportTypes.ChangeLogSummary;
            RunReport(true);
        }

        protected virtual void InitActionTypeCombo()
        {
            actionSelectOne = new ValueListItem(-1, AllTagText);
            actionTypeCombo.Items.Add(actionSelectOne);

            Array actions = Enum.GetValues(typeof(AuditableActionType));
            SortedDictionary<string, ValueListItem> sortedTags = new SortedDictionary<string, ValueListItem>();
            for (int i = 1; i < actions.Length; i++)
            {
                AuditableActionType action = (AuditableActionType)actions.GetValue(i);
                var actionName = AuditTools.GetAttributeOfType<DescriptionAttribute>(action);
                sortedTags.Add(actionName.Description, new ValueListItem(action, actionName.Description));
            }

            foreach (KeyValuePair<string, ValueListItem> item in sortedTags)
            {
                actionTypeCombo.Items.Add(item.Value);
            }

            actionTypeCombo.SelectedIndex = 0;
        }

        protected void InitRepositoryUserCombo()
        {
            repositoryUserCombo.Items.Clear();

            DataSet dataSet = new DataSet();
            DataTable repositoryTable = RepositoryHelper.GetReportData("p_GetChangeLogRepositoryUser");
            dataSet.Tables.Add(repositoryTable);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                var item = new ValueListItem(row[0].ToString(), row[0].ToString());
                repositoryUserCombo.Items.Add(item);
            }

            repositoryUserCombo.SelectedIndex = 0;
        }

        protected void InitWorkstationCombo()
        {
            workstationCombo.Items.Clear();

            DataSet dataSet = new DataSet();
            DataTable workstationTable = RepositoryHelper.GetReportData("p_GetChangeLogWorkstation");
            dataSet.Tables.Add(workstationTable);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                var item1 = new ValueListItem(row[0].ToString(), row[0].ToString());
                workstationCombo.Items.Add(item1);
            }

            workstationCombo.SelectedIndex = 0;
        }

        protected void InitWorkstationUserCombo()
        {
            workstationUserCombo.Items.Clear();

            DataSet dataSet = new DataSet();
            DataTable workstationTable = RepositoryHelper.GetReportData("p_GetChangeLogWorkstationUser");
            dataSet.Tables.Add(workstationTable);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                var item = new ValueListItem(row[0].ToString(), row[0].ToString());
                workstationUserCombo.Items.Add(item);
            }
            
            workstationUserCombo.SelectedIndex = 0;
        }

        public override bool CanRunReport(out string message)
        {
            message = "Rendering";

            if (isFirstRendering == false)
            {
                isFirstRendering = true;
                return false;
            }
            
            return this.isFirstRendering;
        }

        protected override void SetReportParameters()
        {
            base.SetReportParameters();
            //Action Type selected
            ValueListItem selectedActionType = (ValueListItem)actionTypeCombo.SelectedItem;

            if (selectedActionType == null)
            {
                selectedActionType = new ValueListItem(AllTagText, AllTagText);
            }
            else
            {
                if (selectedActionType.DataValue == null) selectedActionType = new ValueListItem(AllTagText, AllTagText);
                if (actionTypeCombo.SelectedIndex == 0) selectedActionType = new ValueListItem(AllTagText, AllTagText);
            }
            //Repository User selected
            ValueListItem selectedRepository = (ValueListItem)repositoryUserCombo.SelectedItem;

            if (selectedRepository == null)
            {
                selectedRepository = new ValueListItem(AllTagText, AllTagText);
            }
            else
            {
                if (selectedRepository.DataValue == null) selectedRepository = new ValueListItem(AllTagText, AllTagText);
                if (repositoryUserCombo.SelectedIndex == 0) selectedRepository = new ValueListItem(AllTagText, AllTagText);
            }
            //Workstation selected
            ValueListItem selectedWorkstation = (ValueListItem)workstationCombo.SelectedItem;

            if (selectedWorkstation == null)
            {
                selectedWorkstation = new ValueListItem(AllTagText, AllTagText);
            }
            else
            {
                if (selectedWorkstation.DataValue == null) selectedWorkstation = new ValueListItem(AllTagText, AllTagText);
                if (workstationCombo.SelectedIndex == 0) selectedWorkstation = new ValueListItem(AllTagText, AllTagText);
            }
            //Workstation User selected
            ValueListItem selectedWorsktationUser = (ValueListItem)workstationUserCombo.SelectedItem;

            if (selectedWorsktationUser == null)
            {
                selectedWorsktationUser = new ValueListItem(AllTagText, AllTagText);
            }
            else
            {
                if (selectedWorsktationUser.DataValue == null) selectedWorsktationUser = new ValueListItem(AllTagText, AllTagText);
                if (workstationUserCombo.SelectedIndex == 0) selectedWorsktationUser = new ValueListItem(AllTagText, AllTagText);
            }

            reportParameters.Add("AuditName", selectedActionType.DisplayText);
            reportParameters.Add("SQLUser", selectedRepository.DataValue);
            reportParameters.Add("Workstation", selectedWorkstation.DataValue);
            reportParameters.Add("WorkstationUser", selectedWorsktationUser.DataValue);
            reportParameters.Add("Period", ((int)reportData.periodType).ToString());
            reportParameters.Add("UTCOffset", reportData.dateRanges[0].UtcOffsetMinutes.ToString());
            
            if (periodCombo.SelectedItem != periodSetCustom) return;

            reportParameters.Add("rsStart", reportData.dateRanges[0].UtcStart.ToLocalTime().ToString("yyyy'-'MM'-'dd HH':'mm':'ss"));
            reportParameters.Add("rsEnd", reportData.dateRanges[0].UtcEnd.ToLocalTime().ToString("yyyy'-'MM'-'dd HH':'mm':'ss"));        
        }

        public override Dictionary<string, object> GetReportParmeters()
        {
            SetReportParameters();
            
            reportParameters.Remove("UTCOffset");
            /*
            reportParameters.Add("rsStartHours", startHoursTimeEditor.Time.Hours);
            reportParameters.Add("rsEndHours", endHoursTimeEditor.Time.Hours);
             */
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
                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
                passthroughParameters.Add(new ReportParameter("AuditName", localReportData.reportParameters["AuditName"].ToString()));
                passthroughParameters.Add(new ReportParameter("SQLUser", (string)localReportData.reportParameters["SQLUser"]));
                passthroughParameters.Add(new ReportParameter("Workstation", (string)localReportData.reportParameters["Workstation"]));
                passthroughParameters.Add(new ReportParameter("WorkstationUser", (string)localReportData.reportParameters["WorkstationUser"])); 
                passthroughParameters.Add(new ReportParameter("Period", ((int)reportData.periodType).ToString()));
                if (localReportData.reportParameters.ContainsKey("rsStart")) passthroughParameters.Add(new ReportParameter("rsStart",
                                                                   localReportData.reportParameters["rsStart"].ToString()));
                if (localReportData.reportParameters.ContainsKey("rsEnd")) passthroughParameters.Add(new ReportParameter("rsEnd",
                                                                   localReportData.reportParameters["rsEnd"].ToString()));

                Log.Debug("localReportData.reportType = ", localReportData.reportType);

                localReportData.dataSources = new ReportDataSource[1];

                string displayText = "";
                object[] dataValue = {0,0,0};

                this.Invoke(new MethodInvoker(delegate() 
                    {
                        displayText = ((ValueListItem)actionTypeCombo.SelectedItem).DisplayText;
                        dataValue[0] = ((ValueListItem)workstationCombo.SelectedItem).DataValue;
                        dataValue[1] = ((ValueListItem)workstationUserCombo.SelectedItem).DataValue;
                        dataValue[2] = ((ValueListItem)repositoryUserCombo.SelectedItem).DataValue;
                    }));

                dataSource = new ReportDataSource("ChangeLogSummary");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetChangeLogSummary",
                                                                  displayText,
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes,
                                                                  dataValue[0],
                                                                  dataValue[1],
                                                                  dataValue[2]
                                                                  );
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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.ChangeLogSummary.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }
                            reportViewer.LocalReport.SetParameters(passthroughParameters);
                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "Change Log Summary";
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
            actionTypeCombo.SelectedIndex = 0;
            repositoryUserCombo.SelectedIndex = 0;
            workstationCombo.SelectedIndex = 0;
            workstationUserCombo.SelectedIndex = 0;

            //ResetTimeFilter();
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

        private void periodCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (periodCombo.SelectedItem == periodSetCustom)
            {
                startHoursLbl.Visible = true;
                startHoursTimeEditor.Visible = true;
                endHoursLbl.Visible = true;
                endHoursTimeEditor.Visible = true;
            }
            else
            {
                startHoursLbl.Visible = false;
                startHoursTimeEditor.Visible = false;
                endHoursLbl.Visible = false;
                endHoursTimeEditor.Visible = false;
            }
        }

        private void startHoursTimeEditor_ValueChanged(object sender, EventArgs e)
        {
            DateTime first = customDates[0].UtcStart.ToLocalTime();
            DateTime final = new DateTime(first.Year, first.Month, first.Day, startHoursTimeEditor.Time.Hours, startHoursTimeEditor.Time.Minutes, startHoursTimeEditor.Time.Seconds);
            customDates[0].UtcStart = final.ToUniversalTime();
        }

        private void endHoursTimeEditor_ValueChanged(object sender, EventArgs e)
        {
            DateTime last = customDates[customDates.Count - 1].UtcEnd.ToLocalTime();
            DateTime final = new DateTime(last.Year, last.Month, last.Day, endHoursTimeEditor.Time.Hours, endHoursTimeEditor.Time.Minutes, endHoursTimeEditor.Time.Seconds);
            customDates[customDates.Count - 1].UtcEnd = final.ToUniversalTime();
        }

        private void MasterUpdateCombo(InitCombo initCombo, ComboBox comboBox)
        {
            string previousSelection = ((ValueListItem)comboBox.SelectedItem).DisplayText;
            initCombo();

            foreach (ValueListItem item in comboBox.Items)
            {
                if (item.DisplayText == previousSelection)
                {
                    comboBox.SelectedItem = item;
                    break;
                }
            }
        }

        /// <summary>
        /// Updates the dropdown with the latest Workstation Users registered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repositoryUserCombo_DropDown(object sender, EventArgs e)
        {
            MasterUpdateCombo(InitRepositoryUserCombo, repositoryUserCombo);
        }

        /// <summary>
        /// /// Updates the dropdown with the latest Workstations registered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void workstationCombo_DropDown(object sender, EventArgs e)
        {
            MasterUpdateCombo(InitWorkstationCombo, workstationCombo);
        }

        /// <summary>
        /// Updates the dropdown with the latest Workstation Users registered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void workstationUserCombo_DropDown(object sender, EventArgs e)
        {
            MasterUpdateCombo(InitWorkstationUserCombo, workstationUserCombo);
        }
    }
}
