using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using BBS.TracerX;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    /// <summary>
    /// BaselineStatistics.cs provides methods for generating Desktop Client reports and SSRS deployed reports for Baseline Statistics.
    /// </summary>
    public partial class BaselineStatistics : ReportContol
    {

        #region Fields & Properties
        /// <summary>
        /// Log. BaselineStatistics report logger
        /// </summary>
        private static readonly Logger Log = Logger.GetLogger("BaselineStatistics Report");

        /// <summary>
        /// passthroughParameters. List of parameters that will be passed to the report.
        /// </summary>
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(7);

        /// <summary>
        /// compareCurrentServer. Selected server at the "Compare To" area
        /// </summary>
        private MonitoredSqlServer compareCurrentServer = null;

        /// <summary>
        /// lastSelectedInstsance. Selected instance from the compareInstanceCombo
        /// </summary>
        private int lastSelectedInstsance = 0;

        /// <summary>
        /// lastSelectedTag. Selected instance from the compareTagsCombo
        /// </summary>
        private int lastSelectedTag = 0;

        /// <summary>
        /// compareEndDateTime. 
        /// </summary>
        private DateTime compareEndDateTime;

        /// <summary>
        /// localReportData. 
        /// </summary>
        private WorkerData localReportData = null;

        #endregion Fields & Properties




        #region Public Constructors

        /// <summary>
        /// Initializes the report controls
        /// </summary>
        public BaselineStatistics()
        {
            compareEndDateTime = new DateTime();
            InitializeComponent();
            base.AdaptFontSize();
        }

        #endregion Public Constructors




        #region Base Class Overrides

        public override void InitReport()
        {
            base.InitReport();

            InitializeReportViewer();
            InitCompareTagsCombo();
            InitCompareInstanceCombo();

            State = UIState.ParmsNeeded;
            periodCombo.SelectedItem = period7;
            sampleSizeCombo.SelectedItem = sampleHours;
            ReportType = ReportTypes.BaselineStatistics;

            InitMetricCombo();
            InitCompareMetricCombo();
            UpdateCompareStartDate();
            UpdateCompareEndDate();
        }

        protected virtual Pair<DateTime, DateTime> GetSelectedRange()
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = startDate;

            if ((periodCombo.SelectedItem == periodToday || periodCombo.SelectedItem == periodSetCustom) && customDates != null)
            {
                startDate = customDates[0].UtcStart.ToLocalTime();
                endDate = customDates[customDates.Count - 1].UtcEnd.ToLocalTime(); //customDates[customDates.Count - 1].UtcEnd helps us to handle Daylight saving mode. Only applies to UtcEnd
            }
            else if (periodCombo.SelectedItem == period7)
            {
                startDate = endDate.AddDays(-7);
            }
            else if (periodCombo.SelectedItem == period30)
            {
                startDate = endDate.AddDays(-30);
            }
            else if (periodCombo.SelectedItem == period365)
            {
                startDate = endDate.AddDays(-365);
            }

            return new Pair<DateTime, DateTime>(startDate, endDate);
        }

        public override bool CanRunReport(out string message)
        {
            message = String.Empty;

            this.SetReportParameters();

            if (instanceCombo.SelectedIndex == 0)
            {
                message = "A SQL Server instance must be selected to generate this report.";
                return false;
            }

            if (metricCombo.SelectedIndex == 0)
            {
                message = "A metric must be selected to generate this report.";
                return false;
            }

            if (compareMetricCombo.SelectedIndex == 0 && (compareInstanceCombo.SelectedIndex != 0 || compareSameServerChkBox.Checked))
            {
                message = "A compare metric must be selected.";
                return false;
            }

            return true;
        }

        protected override void SetReportParameters()
        {
            base.SetReportParameters();

            Pair<DateTime, DateTime> baseRange = this.GetSelectedRange(); // LocalTime
            Pair<DateTime, DateTime> compareRange = new Pair<DateTime, DateTime>(compareStartDateTimePicker.Value, compareEndDateTime); // LocalTime
            Pair<string, int> compareServer = this.GetCompareServer();

            reportParameters.Add("GUIServer", instanceCombo.SelectedItem == null ? "" : instanceCombo.SelectedItem);
            reportParameters.Add("ServerID", instanceCombo.SelectedItem == null ? "" : reportData.instanceID.ToString());
            reportParameters.Add("MetricID", metricCombo.SelectedIndex > 0 ? this.metricCombo.SelectedValue : null);
            reportParameters.Add("GUIDateRange", this.GetDateRange(baseRange.First.ToUniversalTime(), baseRange.Second.ToUniversalTime(), (PeriodType)(((ValueListItem)periodCombo.SelectedItem).DataValue)));
            reportParameters.Add("UTCStart", baseRange.First.ToString("yyyy'-'MM'-'dd HH':'mm':'ss"));
            reportParameters.Add("UTCEnd", baseRange.Second.ToString("yyyy'-'MM'-'dd HH':'mm':'ss"));
            reportParameters.Add("rsStartHours", startHoursTimeEditor.Time.Hours);
            reportParameters.Add("rsEndHours", endHoursTimeEditor.Time.Hours);

            reportParameters.Add("GUICompareServer", compareServer.First);
            reportParameters.Add("GUICompareDateRange", string.IsNullOrEmpty(compareServer.First) ? string.Empty : this.GetDateRange(compareRange.First.ToUniversalTime(), compareRange.Second.ToUniversalTime(), ((PeriodType)(((ValueListItem)periodCombo.SelectedItem).DataValue))));
            reportParameters.Add("CompareSQLServerID", compareServer.Second);
            reportParameters.Add("CompareMetricID", compareMetricCombo.SelectedValue ?? -1);
            reportParameters.Add("CompareStartRange", compareRange.First);
            reportParameters.Add("CompareEndRange", compareRange.Second);
            reportParameters.Add("rsCompareStartHours", compareStartHoursTimeEditor.Time.Hours);
            reportParameters.Add("rsCompareEndHours", compareEndHoursTimeEditor.Time.Hours);

            reportParameters.Add("Interval", ((int)reportData.sampleSize));
            reportParameters.Add("Period", ((int)reportData.periodType));
            reportParameters.Add("executionStart", DateTime.Now);

        }

        override protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall())
            {
                localReportData = (WorkerData)e.Argument;

                bool custom = (localReportData.periodType == PeriodType.Custom || localReportData.periodType == PeriodType.Today);

                DateTime start = custom ? customDates[0].UtcStart : localReportData.dateRanges[0].UtcStart;
                DateTime end = custom ? customDates[customDates.Count - 1].UtcEnd : localReportData.dateRanges[0].UtcEnd;

                // Yes there is a checkbox referenced here.
                // compareSameServerChkBox.Checked
                // No. It should not be because it should cause a cross-thread exception, but it does not. So i am leaving it.
                // If a cross-thread exception starts appearing in this vicinity, put the checkbox value into reportparameters and pass 
                // it in like that.
                passthroughParameters.Clear();
                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
                passthroughParameters.Add(new ReportParameter("Interval", ((int)reportData.sampleSize).ToString()));
                passthroughParameters.Add(new ReportParameter("GUIServer", localReportData.reportParameters["GUIServer"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUIDateRange", this.GetDateRange(start, end, localReportData.periodType)));
                passthroughParameters.Add(new ReportParameter("GUICompareServer", localReportData.reportParameters["GUICompareServer"].ToString()));
                passthroughParameters.Add(new ReportParameter("GUICompareDateRange", localReportData.reportParameters["GUICompareDateRange"].ToString()));
                passthroughParameters.Add(new ReportParameter("CompareSQLServerID", compareSameServerChkBox.Checked ? localReportData.instanceID.ToString() : localReportData.reportParameters["CompareSQLServerID"].ToString()));
                passthroughParameters.Add(new ReportParameter("CompareStartRange", localReportData.reportParameters["CompareStartRange"].ToString()));
                passthroughParameters.Add(new ReportParameter("CompareEndRange", localReportData.reportParameters["CompareEndRange"].ToString()));

                localReportData.dataSources = new ReportDataSource[1];

                ReportDataSource dataSource = new ReportDataSource("BaselineStatistics");


                DataTable baselineStatisticsTable = RepositoryHelper.GetReportData("p_GetBaselineStatisticsReport",
                                                                  localReportData.instanceID,
                                                                  start,
                                                                  end,
                                                                  int.Parse(localReportData.reportParameters["MetricID"].ToString()),
                                                                  compareSameServerChkBox.Checked ?
                                                                                localReportData.instanceID :
                                                                                int.Parse(localReportData.reportParameters["CompareSQLServerID"].ToString()),
                                                                  ((DateTime)localReportData.reportParameters["CompareStartRange"]).ToUniversalTime(),
                                                                  ((DateTime)localReportData.reportParameters["CompareEndRange"]).ToUniversalTime(),
                                                                  int.Parse(localReportData.reportParameters["CompareMetricID"].ToString()),
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  (int)localReportData.sampleSize);

                dataSource.Value = baselineStatisticsTable;
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
                            e.Error.Message.ToLower().Contains("msxmlsql.dll"))
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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.BaselineStatistics.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }

                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }

                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "BaselineStatistics";
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

        /// <summary>
        /// Show the appropriate sample size options based on _customDates.
        /// The goal is to prevent the user from trying to plot too many or two few
        /// data points.  The limits are arbitrary.  
        /// </summary>
        protected override void UpdateCustomSampleSizes()
        {
            DateTime first = customDates[0].UtcStart.ToLocalTime();
            DateTime last = customDates[customDates.Count - 1].UtcEnd.ToLocalTime();
            bool isToday = !(first.Day != last.Day || first.Month != last.Month || first.Year != last.Year);
            TimeSpan span = new TimeSpan(0);

            foreach (DateRangeOffset range in customDates)
            {
                span += range.UtcEnd.ToLocalTime().Date - range.UtcStart.ToLocalTime().Date;
            }

            if (span.TotalMinutes < 2000.0) sampleSizeCombo.Items.Add(sampleMinutes);
            if (span.TotalHours < 500.0) sampleSizeCombo.Items.Add(sampleHours);
            if (!isToday && span.TotalDays < 500.0) sampleSizeCombo.Items.Add(sampleDays);
            if (first.Month != last.Month || first.Year != last.Year) sampleSizeCombo.Items.Add(sampleMonths);
            if (first.Year != last.Year) sampleSizeCombo.Items.Add(sampleYears);
        }

        public override void ResetFilterCriteria()
        {
            base.ResetFilterCriteria();
            UpdateCompareStartDate();
            UpdateCompareEndDate();
            compareTagsCombo.SelectedIndex = 0;
            compareInstanceCombo.SelectedIndex = 0;
            compareStartHoursTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
            compareEndHoursTimeEditor.Time = System.TimeSpan.Parse("23:00:00");
            compareMetricCombo.SelectedIndex = 0;
            compareSameServerChkBox.Checked = false;
            sampleSizeCombo.SelectedIndex = sampleSizeCombo.Items.IndexOf(sampleDays);
            metricCombo.SelectedIndex = 0;
            periodCombo.SelectedItem = period7;

            compareInstanceCombo.Enabled = true;
            compareTagsCombo.Enabled = true;

            if (customDates != null)
            {
                startHoursTimeEditor.Time = System.TimeSpan.Parse("00:00:00");
                endHoursTimeEditor.Time = System.TimeSpan.Parse("23:00:00");
            }
        }

        /// <summary>
        /// Init the combo box on each dropdown so that it'll reflect added/removed servers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void instanceCombo_BeforeDropDown(object sender, EventArgs e)
        {
            string selectedServer = ((ValueListItem)instanceCombo.SelectedItem).DisplayText;
            InitInstanceCombo();
            foreach (ValueListItem item in instanceCombo.Items)
            {
                if (item.DisplayText == selectedServer)
                {
                    instanceCombo.SelectedItem = item;
                    break;
                }
            }
        }

        #endregion Base Class Overrides




        #region Initializers

        private void InitCompareTagsCombo()
        {
            if (tagSelectOne == null)
            {
                tagSelectOne = new ValueListItem(null, "< All >");
            }

            compareTagsCombo.Items.Add(tagSelectOne);

            SortedDictionary<string, ValueListItem> sortedTags = new SortedDictionary<string, ValueListItem>();
            foreach (Tag tag in ApplicationModel.Default.Tags)
            {
                if (tag.Instances.Count > 0)
                {
                    sortedTags.Add(tag.Name, new ValueListItem(tag.Id, tag.Name));
                }
            }

            foreach (KeyValuePair<string, ValueListItem> tag in sortedTags)
            {
                compareTagsCombo.Items.Add(tag.Value);
            }

            compareTagsCombo.SelectedIndex = 0;
        }

        private void InitCompareInstanceCombo()
        {
            compareInstanceCombo.Items.Clear();
            ValueListItem[] instances = null;
            ValueListItem instance;
            int instanceCount;

            int i = 0;
            int serverID = -1;

            // if the instance combo is not displayed, don't do any work.
            if (compareInstanceCombo.Visible == false)
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
            compareInstanceCombo.Items.Add(instanceSelectOne);
            compareCurrentServer = null;

            if ((compareTagsCombo.SelectedItem != null) && (compareTagsCombo.SelectedItem != tagSelectOne))
            {
                instanceCount = GetInstanceCount((int)(((ValueListItem)compareTagsCombo.SelectedItem).DataValue));
                instances = new ValueListItem[instanceCount];
                Tag tag = GetTag((int)(((ValueListItem)compareTagsCombo.SelectedItem).DataValue));
                if (tag == null)
                {
                    compareTagsCombo.Items.Remove(compareTagsCombo.SelectedItem);
                    compareTagsCombo.SelectedIndex = 0;
                    return;
                }
                MonitoredSqlServer server;

                foreach (int id in tag.Instances)
                {
                    server = GetServer(id);

                    instance = new ValueListItem(server, server.InstanceName);

                    if ((serverID != -1) && (serverID == server.Id))
                    {
                        compareCurrentServer = server;
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
                        compareCurrentServer = server;
                    }
                    instances[i++] = instance;
                }
            }
            compareInstanceCombo.Items.AddRange(instances);

            //now re-select the one they had selected, if they did have one
            if (compareInstanceCombo.SelectedItem == null && compareCurrentServer != null)
            {
                compareInstanceCombo.SelectedIndex = compareInstanceCombo.FindStringExact(compareCurrentServer.InstanceName);
            }
            else
            {
                compareInstanceCombo.SelectedIndex = 0;
            }
        }

        private void InitMetricCombo()
        {
            DataSet dataSet = new DataSet();
            DataTable baselineStatisticsTable = RepositoryHelper.GetReportData("p_GetBaselineMetrics");
            dataSet.Tables.Add(baselineStatisticsTable);

            metricCombo.DataSource = dataSet.Tables[0].DefaultView;
            metricCombo.DisplayMember = "Name";
            metricCombo.ValueMember = "MetricID";
            metricCombo.SelectedIndex = 0;
        }

        private void InitCompareMetricCombo()
        {
            DataSet dataSet = new DataSet();
            DataTable baselineStatisticsTable = RepositoryHelper.GetReportData("p_GetBaselineMetrics");
            dataSet.Tables.Add(baselineStatisticsTable);

            compareMetricCombo.DataSource = dataSet.Tables[0].DefaultView;
            compareMetricCombo.DisplayMember = "Name";
            compareMetricCombo.ValueMember = "MetricID";
            compareMetricCombo.SelectedIndex = 0;
        }

        #endregion Initializers




        #region Events

        void compareTagsCombo_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            InitCompareInstanceCombo();
            SetReportParameters();
            lastSelectedTag = compareTagsCombo.SelectedIndex;
        }

        void compareInstanceCombo_DropDown(object sender, System.EventArgs e)
        {
            string selectedServer = ((ValueListItem)compareInstanceCombo.SelectedItem).DisplayText;
            InitCompareInstanceCombo();

            foreach (ValueListItem item in compareInstanceCombo.Items)
            {
                if (item.DisplayText == selectedServer)
                {
                    compareInstanceCombo.SelectedItem = item;
                    break;
                }
            }
        }

        private void metricCombo_SelectedValueChanged(object sender, EventArgs e)
        {
            SetReportParameters();
        }

        private void periodCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool display = false;

            if (periodCombo.SelectedItem == periodToday)
            {
                customDates = new List<DateRangeOffset>();
                DateRangeOffset.AddDateRange(customDates, DateTime.Now, DateTime.Now);
            }

            if (periodCombo.SelectedItem == periodSetCustom || periodCombo.SelectedItem == periodToday)
            {
                DateTime start = customDates[0].UtcStart.ToLocalTime();
                DateTime last = DateTime.Now;

                display = !(start.Year != last.Year || start.Month != last.Month || start.Day != last.Day);
            }

            UpdateCompareStartDate();
            UpdateCompareEndDate();

            if (periodCombo.SelectedItem == periodToday || display)
            {
                display = true;
                sampleSizeCombo.SelectedIndex = 0;
                try
                {
                    this.SetStartHours();
                    this.SetEndHours();
                    this.SetCompareStartHours();
                    this.SetCompareEndHours();
                }
                catch (Exception)
                {
                    ;
                }
            }

            DisplayTimeFilter(display);
        }

        private void startHoursTimeEditor_ValueChanged(object sender, EventArgs e)
        {
            SetStartHours();
        }

        private void endHoursTimeEditor_ValueChanged(object sender, EventArgs e)
        {
            SetEndHours();
        }

        private void compareStartHoursTimeEditor_ValueChanged(object sender, EventArgs e)
        {
            SetCompareStartHours();
        }

        private void compareEndHoursTimeEditor_ValueChanged(object sender, EventArgs e)
        {
            SetCompareEndHours();
        }

        private void compareSameServerChkBox_Click(object sender, EventArgs e)
        {
            bool chkState = this.compareSameServerChkBox.Checked;

            int lastSelTag = lastSelectedTag;
            int lastSelIns = lastSelectedInstsance;

            if (chkState)
            {
                compareTagsCombo.SelectedIndex = 0;
                compareInstanceCombo.SelectedIndex = 0;
            }
            else
            {
                compareTagsCombo.SelectedIndex = lastSelTag;
                compareInstanceCombo.SelectedIndex = lastSelIns;
            }

            lastSelectedTag = lastSelTag;
            lastSelectedInstsance = lastSelIns;

            compareInstanceCombo.Enabled = !chkState;
            compareTagsCombo.Enabled = !chkState;
        }

        private void compareStartDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            UpdateCompareEndDate();
        }

        private void compareInstanceCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (compareInstanceCombo.SelectedItem == null) return;
            compareCurrentServer = (MonitoredSqlServer)((ValueListItem)compareInstanceCombo.SelectedItem).DataValue;
            lastSelectedInstsance = compareInstanceCombo.SelectedIndex;
            SetReportParameters();
        }

        #endregion Events




        #region Private Methods

        /// <summary>
        /// SetStartHours. Adds custom hours to the customDates UtcStart field.
        /// </summary>
        private void SetStartHours()
        {
            DateTime first = customDates[0].UtcStart.ToLocalTime();
            DateTime final = new DateTime(first.Year, first.Month, first.Day, startHoursTimeEditor.Time.Hours, startHoursTimeEditor.Time.Minutes, startHoursTimeEditor.Time.Seconds);
            customDates[0].UtcStart = final.ToUniversalTime();
        }

        /// <summary>
        /// SetEndHours. Adds custom hours to the customDates UtcEnd field.
        /// </summary>
        private void SetEndHours()
        {
            DateTime last = customDates[customDates.Count - 1].UtcEnd.ToLocalTime();
            DateTime final = new DateTime(last.Year, last.Month, last.Day, endHoursTimeEditor.Time.Hours, endHoursTimeEditor.Time.Minutes, endHoursTimeEditor.Time.Seconds);
            customDates[customDates.Count - 1].UtcEnd = final.ToUniversalTime();
        }

        /// <summary>
        /// SetCompareStartHours. Adds custom hours to the compareStartDateTimePicker field.
        /// </summary>
        private void SetCompareStartHours()
        {
            DateTime first = compareStartDateTimePicker.Value;
            DateTime final = new DateTime(first.Year, first.Month, first.Day, compareStartHoursTimeEditor.Time.Hours, compareStartHoursTimeEditor.Time.Minutes, compareStartHoursTimeEditor.Time.Seconds);
            compareStartDateTimePicker.Value = final;
        }

        /// <summary>
        /// SetCompareEndHours. Adds custom hours to the compareEndDateTime field.
        /// </summary>
        private void SetCompareEndHours()
        {
            DateTime first = compareEndDateTime;
            DateTime final = new DateTime(first.Year, first.Month, first.Day, compareEndHoursTimeEditor.Time.Hours, compareEndHoursTimeEditor.Time.Minutes, compareEndHoursTimeEditor.Time.Seconds);
            compareEndDateTime = final.ToUniversalTime();
        }

        /// <summary>
        /// DisplayTimeFilter. Displays Hours controls if Period = Today or Period = Custom (with days corresponding to Today's)
        /// </summary>
        /// <param name="display"></param>
        private void DisplayTimeFilter(bool display)
        {
            startHoursLbl.Visible = display;
            startHoursTimeEditor.Visible = display;
            endHoursLbl.Visible = display;
            endHoursTimeEditor.Visible = display;
            compareStartHours.Visible = display;
            compareStartHoursTimeEditor.Visible = display;
            compareEndHours.Visible = display;
            compareEndHoursTimeEditor.Visible = display;
        }

        /// <summary>
        /// GetDateRange. Gets the date range that will be displayed on the report.
        /// </summary>
        /// <param name="rsStart"></param>
        /// <param name="rsEnd"></param>
        /// <param name="periodType"></param>
        /// <returns></returns>
        private string GetDateRange(DateTime rsStart, DateTime rsEnd, PeriodType periodType)
        {
            string dateRange = string.Empty;
            bool dateWithHours = true;

            if (rsStart.ToLocalTime().Date != DateTime.Now.Date || 
                rsStart.ToLocalTime().Year != rsEnd.ToLocalTime().Year || 
                rsStart.ToLocalTime().Month != rsEnd.ToLocalTime().Month || 
                rsStart.ToLocalTime().Day != rsEnd.ToLocalTime().Day)
                dateWithHours = false;

            if (dateWithHours || periodType == PeriodType.Today)
            {
                dateRange = rsStart.ToLocalTime() + " to " + rsEnd.ToLocalTime();
            }
            else
            {
                dateRange = rsStart.ToLocalTime().ToString("d") + " to " + rsEnd.ToLocalTime().ToString("d");
            }

            return dateRange;
        }

        /// <summary>
        /// UpdateCompareStartDate. Updates the CompareStartDate according to the selected Period
        /// </summary>
        private void UpdateCompareStartDate()
        {
            DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            DateTime startDate = endDate.Date;
            switch ((PeriodType)(((ValueListItem)periodCombo.SelectedItem).DataValue))
            {
                case PeriodType.Today:
                    startDate = endDate.Date;
                    break;
                case PeriodType.Last7:
                    startDate = endDate.AddDays(-7);
                    break;
                case PeriodType.Last30:
                    startDate = endDate.AddDays(-30);
                    break;
                case PeriodType.Last365:
                    startDate = endDate.AddDays(-365);
                    break;
                case PeriodType.Custom:
                    startDate = customDates[0].UtcStart.ToLocalTime();
                    break;
            }
            compareStartDateTimePicker.Value = startDate;
        }

        /// <summary>
        /// UpdateCompareEndDate. Updates the CompareEndDate according to the selected Period
        /// </summary>
        private void UpdateCompareEndDate()
        {
            compareEndDateText.Text = GetCompareEndDate().ToString("d");
        }

        /// <summary>
        /// Gets and sets the CompareEndDate
        /// </summary>
        /// <returns></returns>
        private DateTime GetCompareEndDate()
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = compareStartDateTimePicker.Value;
            bool custom = false;

            switch ((PeriodType)(((ValueListItem)periodCombo.SelectedItem).DataValue))
            {
                case PeriodType.Last7:
                    endDate = startDate.AddDays(7);
                    break;
                case PeriodType.Last30:
                    endDate = startDate.AddDays(30);
                    break;
                case PeriodType.Last365:
                    endDate = startDate.AddDays(365);
                    break;
                case PeriodType.Today:
                case PeriodType.Custom:
                    Pair<DateTime, DateTime> range = this.GetSelectedRange();
                    TimeSpan span = customDates == null ? TimeSpan.Zero : range.Second - range.First;
                    endDate = startDate.AddDays(span.TotalDays).ToLocalTime();
                    custom = true;
                    Log.DebugFormat("UtcStart: {0} - UtcEnd: {1} - compareStartDateTimePicker: {2} - endDate:{3} - span: {4}", range.First, range.Second, compareStartDateTimePicker.Value, endDate, span.TotalDays);
                    break;
            }

            this.compareEndDateTime = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, custom ? 0 : 59, custom ? 0 : 59); ;
            return endDate;
        }

        /// <summary>
        /// Gets and sets the Compare Server
        /// </summary>
        /// <returns></returns>
        private Pair<string, int> GetCompareServer()
        {
            int compareServerID = -1;
            string compareServer = string.Empty;

            if (compareInstanceCombo.SelectedIndex > 0)
            {
                compareServer = compareInstanceCombo.SelectedItem.ToString();
                compareServerID = ((MonitoredSqlServer)((ValueListItem)compareInstanceCombo.SelectedItem).DataValue).Id;
            }

            if (compareSameServerChkBox.Checked)
            {
                ValueListItem selectedServer = (ValueListItem)instanceCombo.SelectedItem;
                MonitoredSqlServer monitoredSqlServer = (MonitoredSqlServer) selectedServer.DataValue;

                bool isSelectedServerNull = (selectedServer == null);
                bool isDataValueNull = (selectedServer.DataValue == null);

                compareServer = isSelectedServerNull ? string.Empty : selectedServer.ToString();
                compareServerID = isSelectedServerNull || isDataValueNull ? -1 : monitoredSqlServer.Id;
            }

            return new Pair<string, int>(compareServer, compareServerID);
        }

        #endregion Private Methods






        #region Public Methods



        #endregion Public Methods

    }
}
