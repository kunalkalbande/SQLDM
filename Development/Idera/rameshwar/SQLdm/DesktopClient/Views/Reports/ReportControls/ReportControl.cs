using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Microsoft.Reporting.WinForms;
using Infragistics.Win;
using Wintellect.PowerCollections;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Objects;
using System.Data;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class ReportContol : UserControl
    {
        private static readonly Logger Log = Logger.GetLogger("Report Control");
        private ReportTypes reportType;
        internal string _customReportName;
        protected DrillthroughEventArgs drillThroughArguments;
        protected Dictionary<string, StringCollection> drillthroughParams = new Dictionary<string, StringCollection>();
        protected Dictionary<string, object> reportParameters = new Dictionary<string, object>();
        public bool firstRendering = true;

        public class WorkerData
        {
            public bool cancelled;
            public BackgroundWorker bgWorker;   // BackgroundWorker used for the report.
            public ReportViewer reportViewer;
            public ReportTypes reportType;     // Type of report to generate.
            public PeriodType periodType;       // The period option the user selected for the report
            public string periodDescription;    // Period string to show on the report.
            public List<DateRangeOffset> dateRanges;    // Date ranges to pass to the stored proc(s).
            public SampleSize sampleSize;   // Sample size parameter to pass to the stored proc(s).
            public ReportDataSource[] dataSources;// DataTable returned by a stored proc.
            public int instanceID;  // ID of the monitored instance to run the report against.
            public IList<int> serverIDs; //Names and IDs of all the servers for a tag.
            public Dictionary<string, object> reportParameters;  //to be filled in by derived class.
            public DateTime X1Min;
            public DateTime X1Max;
            public String serverName; //Server Name Wild card string to pass to stored procedure.
            private object _Parameters;
            public int SrcTemplateId;
            public int TrgTemplateId;
            public int TagId;
            public void Cancel()
            {
                cancelled = true;

                if (bgWorker != null)
                    bgWorker.CancelAsync();

                if (reportViewer != null)
                    reportViewer.CancelRendering(0);
            }
            public object Parameters
            {
                get { return _Parameters; }
                set { _Parameters = value; }
            }
        }

        protected WorkerData reportData = new WorkerData();
        protected DateTime renderStart;
        protected List<DateRangeOffset> customDates; // Currently specified custom dates.
        private ValueListItem prevPeriod;
        private bool ignorePeriodSelection = false;

        // Currently selected single server.
        internal MonitoredSqlServer currentServer; // Current single single selection of servers.


        public enum PeriodType { Today, Last7, Last30, Last365, Custom };
        public enum SampleSize { Minutes, Hours, Days, Months, Years };

        // These values appear in the periodCombo drop-down.
        readonly protected ValueListItem periodToday = new ValueListItem(PeriodType.Today, "Today");
        readonly protected ValueListItem period7 = new ValueListItem(PeriodType.Last7, "Last 7 Days");
        readonly protected ValueListItem period30 = new ValueListItem(PeriodType.Last30, "Last 30 Days");
        readonly protected ValueListItem period365 = new ValueListItem(PeriodType.Last365, "Last 365 Days");
        readonly protected ValueListItem periodSetCustom = new ValueListItem(PeriodType.Custom, "Custom Range");

        protected ValueListItem instanceSelectOne;
        protected ValueListItem tagSelectOne;

        // These values appear in the sampleSizeCombo drop-down.
        readonly protected ValueListItem sampleMinutes = new ValueListItem(SampleSize.Minutes, "Minutes");
        readonly protected ValueListItem sampleHours = new ValueListItem(SampleSize.Hours, "Hours");
        readonly protected ValueListItem sampleDays = new ValueListItem(SampleSize.Days, "Days");
        readonly protected ValueListItem sampleMonths = new ValueListItem(SampleSize.Months, "Months");
        readonly protected ValueListItem sampleYears = new ValueListItem(SampleSize.Years, "Years");

        // This value is the prefix for the custom date string.
        protected const string _customPrefix = "Custom ";

        private bool canCancel = false;
        public event EventHandler CanCancelChanged;

        protected enum UIState
        {
            ParmsNeeded,
            GettingData,
            NoDataAcquired,
            NoTableDataAcquired,
            Rendering,
            Rendered,
            Cancelled,
            ReportError
        };

        protected UIState State
        {
            set
            {
                Log.Debug("Setting state to ", value);

                switch (value)
                {
                    case UIState.GettingData:
                    case UIState.Rendering:
                        if (firstRendering)
                        {
                            firstRendering = false;
                            loadingCircle.Active = true;
                            loadingPanel.BringToFront();
                            filterPanel.Enabled = false;
                            CanCancel = true;
                        }
                        break;
                    case UIState.Rendered:
                        loadingCircle.Active = false;
                        reportViewer.BringToFront();
                        filterPanel.Enabled = true;
                        CanCancel = false;
                        break;
                    default:
                        loadingCircle.Active = false;
                        reportInstructionsControl.BringToFront();
                        filterPanel.Enabled = true;
                        CanCancel = false;
                        break;
                }
            }
        }

        protected Panel MainContentPanel
        {
            get { return mainContentPanel; }
        }

        public bool CanCancel
        {
            get { return canCancel; }
            private set
            {
                canCancel = value;

                if (CanCancelChanged != null)
                {
                    CanCancelChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get the daterange from the selected start, end and periodtype
        /// </summary>
        /// <param name="rsStart"></param>
        /// <param name="rsEnd"></param>
        /// <param name="periodType"></param>
        /// <returns></returns>
        internal string GetDateRange(DateTime rsStart, DateTime rsEnd, int periodType)
        {
            string dateRange = string.Empty;
            //bool isCustomPeriod = false;

            //Helpers.CrossThreadHelper.UIThread(this, (MethodInvoker)delegate
            //        {
            //            isCustomPeriod = periodCombo.SelectedItem == periodSetCustom;
            //        });

            //if (isCustomPeriod)
            //if(periodCombo.SelectedItem == periodSetCustom)
            if ((PeriodType)periodType == PeriodType.Custom)
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
        /// Tell base class to ignore period selection
        /// </summary>
        public bool IgnorePeriodSelection
        {
            get { return ignorePeriodSelection; }
            set { ignorePeriodSelection = value; }
        }

        public DrillthroughEventArgs DrillThroughArguments
        {
            get
            {
                return drillThroughArguments;
            }
            set
            {
                drillThroughArguments = value;
            }
        }

        public bool FiltersVisible
        {
            get { return panel1.Visible; }
            set { panel1.Visible = value; }
        }

        public bool ToolbarVisible
        {
            get { return reportViewer.ShowToolBar; }
            set { reportViewer.ShowToolBar = value; }
        }

        public ReportContol()
        {
            InitializeComponent();
            State = UIState.ParmsNeeded;
        }

        public void ShowHelp()
        {
            ReportsHelper.ShowReportHelp(reportType);
        }

        virtual public void InitReport()
        {
            InitDrillthrough();
            InitPeriodCombo();
            InitTagsCombo();
            InitInstanceCombo();
            InitTargetTemplate();
            InitSourceTemplate();
            tagsComboBox.SelectedIndex = 0;
            sampleSizeCombo.SelectedIndex = 0;
        }

        /// <summary>
        /// This method should must be overriden in every derived class to return the string representation of the Report filter parameters
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, object> GetReportParmeters()
        {
            return reportParameters;
        }

        public ReportTypes ReportType
        {
            get
            {
                return reportType;
            }
            set
            {
                reportType = value;
                string reportTitle = (value != ReportTypes.Custom
                                         ? ReportsHelper.GetReportTitle(reportType)
                                         : _customReportName);
                reportInstructionsControl.ReportTitle = reportTitle;
                reportInstructionsControl.ReportDescription = ReportsHelper.GetReportLongDescription(reportType);
            }
        }

        public string CustomReportName
        {
            get { return _customReportName; }
            set { _customReportName = value; }
        }

        public string CurrentServerName
        {
            get { return currentServer.InstanceName.ToString(); }
        }

        private void InitDrillthrough()
        {
            if (drillThroughArguments != null)
            {
                IList<ReportParameter> drillthroughParamsList =
                    ((LocalReport)drillThroughArguments.Report).OriginalParametersToDrillthrough;

                foreach (ReportParameter p in drillthroughParamsList)
                {
                    drillthroughParams.Add(p.Name, p.Values);
                }
            }
        }
        //private void LoadAvailableTemplates()
        //{
        //    //[START]SQLdm 9.0 (Ankit Srivastava)- Resolved defect DE44130 - decreased the default timeout to 20
        //    var connInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
        //    connInfo.ConnectionTimeout = 20;
        //    foreach (AlertTemplate template in RepositoryHelper.GetAlertTemplateList(connInfo))
        //    //[END]SQLdm 9.0 (Ankit Srivastava)- Resolved defect DE44130 - decreased the default timeout to 20
        //    {
        //        sourceCombo.Items(template.TemplateID, template.Name);
        //    }
        //    sourceCombo.SelectedIndex = 0;
        //}
        protected virtual void InitTagsCombo()
        {
            tagSelectOne = new ValueListItem(null, "< All >");
            tagsComboBox.Items.Add(tagSelectOne);

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
                tagsComboBox.Items.Add(tag.Value);
            }

            tagsComboBox.SelectedIndex = 0;
        }

        protected virtual void InitPeriodCombo()
        {
            periodCombo.Items.Add(periodToday);
            periodCombo.Items.Add(period7);
            periodCombo.Items.Add(period30);
            periodCombo.Items.Add(period365);
            periodCombo.Items.Add(periodSetCustom);

            string startDateString = "";
            string endDateString = "";

            foreach (string key in drillthroughParams.Keys)
            {
                if (key.ToLower().Contains("date") || key.ToLower().Contains("utc"))
                {
                    if (key.ToLower().Contains("start"))
                    {
                        startDateString = key;
                    }
                    else
                    if (key.ToLower().Contains("end"))
                    {
                        endDateString = key;
                    }
                }

                if (startDateString == "" && key.Equals("rsStart")) startDateString = key;
                if (endDateString == "" && key.Equals("rsEnd")) endDateString = key;
            }

            if (startDateString.Length > 0)
            {
                DateTime startRange;
                DateTime endRange;

                try
                {
                    startRange = Convert.ToDateTime(drillthroughParams[startDateString][0]).Date;
                }
                catch
                {
                    startRange = DateTime.Today;
                }

                try
                {
                    endRange = Convert.ToDateTime(drillthroughParams[endDateString][0]).Date;
                    endRange = endRange.Date <= DateTime.Today
                                   ? endRange.Add(new TimeSpan(23, 59, 59))
                                   : DateTime.Today.Add(new TimeSpan(23, 59, 59));
                }
                catch
                {
                    endRange = DateTime.Today.Add(new TimeSpan(23, 59, 59));
                }

                bool setCustomDate = false;

                if (endRange.Date == DateTime.Today.Date)
                {
                    switch ((endRange - startRange).Days)
                    {
                        case 0:
                        case 1:
                            periodCombo.SelectedItem = periodToday;
                            break;
                        case 7:
                            periodCombo.SelectedItem = period7;
                            break;
                        case 30:
                            periodCombo.SelectedItem = period30;
                            break;
                        case 365:
                            periodCombo.SelectedItem = period365;
                            break;
                        default:
                            setCustomDate = true;
                            break;
                    }
                }
                else
                {
                    setCustomDate = true;
                }

                if (setCustomDate)
                {
                    ignorePeriodSelection = true;
                    customDates = new List<DateRangeOffset>();
                    DateRangeOffset.AddDateRange(customDates, startRange, endRange);
                    periodCombo.SelectedItem = periodSetCustom;
                    UpdateSampleSizes(false);
                    ignorePeriodSelection = false;
                }
            }
            else
            {
                periodCombo.SelectedItem = periodToday;
            }

            UpdateRangeLabel();
        }

        protected virtual Pair<DateTime, DateTime> GetSelectedRange()
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = startDate;

            if (periodCombo.SelectedItem == periodToday)
            {
                // Nothing to do
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
            else if (periodCombo.SelectedItem == periodSetCustom && customDates != null)
            {
                startDate = customDates[0].UtcStart.ToLocalTime();
                endDate = customDates[customDates.Count - 1].UtcEnd.ToLocalTime();
            }

            return new Pair<DateTime, DateTime>(startDate, endDate);
        }

        protected void UpdateRangeLabel()
        {
            string rangeText = "{0} - {1}";
            Pair<DateTime, DateTime> selectedRange = GetSelectedRange();
            rangeLabel.Text =
                string.Format(rangeText, selectedRange.First.ToString("d"),
                              selectedRange.Second.ToString("d"));
        }

        public void InitSourceTemplate()
        {
            DataRow dr;
            DataTable dt = RepositoryHelper.GetTemplateName();
            dr = dt.NewRow();
            dr.ItemArray = new object[] { -1, "--Select Source Template--" };
            dt.Rows.InsertAt(dr, 0);

            Dictionary<int, string> sourceDic = new Dictionary<int, string>();
            foreach (var data in dt.Rows)
            {
                sourceDic.Add(Convert.ToInt32(((System.Data.DataRow)data).ItemArray[0]), Convert.ToString(((System.Data.DataRow)data).ItemArray[1]));
            }

            sourceCombo.DataSource = new BindingSource(sourceDic, null);
            sourceCombo.DisplayMember = "Value";
            sourceCombo.ValueMember = "Key";
        }

        public void InitTargetTemplate()
        {
            DataRow dr;
            DataTable dt = RepositoryHelper.GetTemplateName();
            dr = dt.NewRow();
            dr.ItemArray = new object[] { -1, "--Select Target Template--" };
            dt.Rows.InsertAt(dr, 0);

            Dictionary<int, string> targetDic = new Dictionary<int, string>();
            foreach (var data in dt.Rows)
            {
                targetDic.Add(Convert.ToInt32(((System.Data.DataRow)data).ItemArray[0]), Convert.ToString(((System.Data.DataRow)data).ItemArray[1]));
            }

            targetCombo.DataSource = new BindingSource(targetDic, null);
            targetCombo.DisplayMember = "Value";
            targetCombo.ValueMember = "Key";
        }
        protected virtual void InitInstanceCombo()
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

        /// <summary>
        /// Reset the basic filter criteria to default
        /// </summary>
        public virtual void ResetFilterCriteria()
        {
            tagsComboBox.SelectedItem = tagSelectOne;
            instanceCombo.SelectedItem = instanceSelectOne;
            periodCombo.SelectedIndex = 0;
            sampleSizeCombo.SelectedIndex = 0;
            targetCombo.SelectedIndex = 0;
            sourceCombo.SelectedIndex = 0;
        }

        /// <summary>
        /// Reset the basic filter criteria to the provided values
        /// </summary>
        /// <param name="defaultTag"></param>
        /// <param name="defaultInstance"></param>
        /// <param name="defaultSampleSizeIndex"></param>
        /// <param name="defaultPeriodIndex"></param>
        public virtual void ResetFilterCriteria(ValueListItem defaultTag, ValueListItem defaultInstance, int defaultSampleSizeIndex, int defaultPeriodIndex)
        {
            tagsComboBox.SelectedItem = defaultTag;
            instanceCombo.SelectedItem = defaultInstance;
            periodCombo.SelectedIndex = defaultPeriodIndex < 0 ? 0 : defaultPeriodIndex;
            sampleSizeCombo.SelectedIndex = defaultSampleSizeIndex < 0 ? 0 : defaultSampleSizeIndex;
        }
        // Init the combo box on each dropdown so that it'll reflect added/removed servers
        protected virtual void instanceCombo_BeforeDropDown(object sender, EventArgs e)
        {
            InitInstanceCombo();
        }

        protected virtual void targetCombo_BeforeDropDown(object sender, EventArgs e)
        {
            InitTargetTemplate();
        }

        protected virtual void sourceCombo_BeforeDropDown(object sender, EventArgs e)
        {
            InitSourceTemplate();
        }

        // Combo box of Servers for single select
        protected virtual void instanceCombo_ValueChanged(object sender, EventArgs e)
        {
            if (instanceCombo.SelectedItem == null) return;
            currentServer = (MonitoredSqlServer)((ValueListItem)instanceCombo.SelectedItem).DataValue;
            SetReportParameters();
        }
      
        private void SourceCombo_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            SetReportParameters();
        }

        private void TargetCombo_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            SetReportParameters();
        }

        private void periodCombo_SelectionChanged(object sender, EventArgs e)
        {
            if (ignorePeriodSelection) return;

            if (periodCombo.SelectedItem == periodSetCustom)
            {
                DateTime minimumDate = DateTime.Now.Date;

                foreach (MonitoredSqlServer server in ApplicationModel.Default.AllInstances.Values)
                {
                    minimumDate = server.EarliestData < minimumDate ? server.EarliestData : minimumDate;
                }

                Pair<DateTime, DateTime> selectedRange = GetSelectedRange();
                PeriodSelectionDialog dialog = new PeriodSelectionDialog(minimumDate.ToLocalTime().Date, selectedRange.First, selectedRange.Second);
                Point location = PointToScreen(periodCombo.Location);
                dialog.Width = periodCombo.Width;
                dialog.Location = location;

                if (dialog.ShowDialog(ParentForm) == DialogResult.OK)
                {
                    customDates = new List<DateRangeOffset>();
                    DateRangeOffset.AddDateRange(customDates, dialog.SelectedStartDate, dialog.SelectedEndDate);
                }
                else if (prevPeriod != null)
                {
                    periodCombo.SelectedItem = prevPeriod;
                }
            }

            prevPeriod = (ValueListItem)periodCombo.SelectedItem;
            UpdateRangeLabel();
            UpdateSampleSizes(false);
            SetReportParameters();
        }

        // Show the appropriate sample size options based on the period.
        protected virtual void UpdateSampleSizes(bool selectDefault)
        {
            ValueListItem selected = (ValueListItem)sampleSizeCombo.SelectedItem;
            sampleSizeCombo.Items.Clear();

            if (periodCombo.SelectedItem == periodToday)
            {
                sampleSizeCombo.Items.Add(sampleMinutes);
                sampleSizeCombo.Items.Add(sampleHours);
            }
            else if (periodCombo.SelectedItem == period7)
            {
                sampleSizeCombo.Items.Add(sampleHours);
                sampleSizeCombo.Items.Add(sampleDays);
            }
            else if (periodCombo.SelectedItem == period30)
            {
                sampleSizeCombo.Items.Add(sampleDays);
            }
            else if (periodCombo.SelectedItem == period365)
            {
                sampleSizeCombo.Items.Add(sampleDays);
                sampleSizeCombo.Items.Add(sampleMonths);
            }
            else if (periodCombo.SelectedItem == periodSetCustom)
            {
                UpdateCustomSampleSizes();
            }
            else
            {
                // Unexpected
                throw new ApplicationException("An unexpected report period value was selected.");
            }

            if ((selected != null) && (!selectDefault && sampleSizeCombo.Items.Contains(selected)))
            {
                sampleSizeCombo.SelectedItem = selected;
            }
            else if (sampleSizeCombo.Items[0] == sampleMinutes)
            {
                sampleSizeCombo.SelectedIndex = 1;
            }
            else
            {
                sampleSizeCombo.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Show the appropriate sample size options based on _customDates.
        /// The goal is to prevent the user from trying to plot too many or two few
        /// data points.  The limits are arbitrary.  
        /// </summary>
        protected virtual void UpdateCustomSampleSizes()
        {
            DateTime first = customDates[0].UtcStart.ToLocalTime();
            DateTime last = customDates[customDates.Count - 1].UtcEnd.ToLocalTime();
            TimeSpan span = new TimeSpan(0);

            foreach (DateRangeOffset range in customDates)
            {
                span += range.UtcEnd.ToLocalTime().Date - range.UtcStart.ToLocalTime().Date;
            }

            if (span.TotalMinutes < 2000.0) sampleSizeCombo.Items.Add(sampleMinutes);
            if (span.TotalHours < 500.0) sampleSizeCombo.Items.Add(sampleHours);
            if (span.TotalDays < 500.0 && span.TotalDays > 1.0) sampleSizeCombo.Items.Add(sampleDays);
            if (first.Month != last.Month || first.Year != last.Year) sampleSizeCombo.Items.Add(sampleMonths);
            if (first.Year != last.Year) sampleSizeCombo.Items.Add(sampleYears);
        }

       

        /// <summary>
        ///Based on the current period selection, get the start and end dates to pass to the stored proc and report.
        /// </summary>
        /// <param name="workerData"></param>
        protected void SetDateRanges(WorkerData workerData)
        {
            DateRangeOffset dro = new DateRangeOffset();

            if (workerData.periodType == PeriodType.Custom)
            {
                workerData.dateRanges = new List<DateRangeOffset>();
                workerData.periodDescription = ((ValueListItem)periodCombo.SelectedItem).DisplayText;
                workerData.X1Min = customDates[0].UtcStart.ToLocalTime();
                workerData.X1Max = customDates[customDates.Count - 1].UtcEnd.ToLocalTime();
                dro.UtcStart = workerData.X1Min.ToUniversalTime();
                dro.UtcEnd = workerData.X1Max.ToUniversalTime();
                dro.UtcOffsetMinutes = TimeZone.CurrentTimeZone.GetUtcOffset(workerData.X1Min).TotalMinutes;
                workerData.dateRanges.Add(dro);
            }
            else
            {
                DateTime today = DateTime.Now.Date;
                workerData.X1Max = new DateTime(today.Year, today.Month, today.Day, 23, 59, 59);

                switch (workerData.periodType)
                {
                    case PeriodType.Today:
                        workerData.X1Min = workerData.X1Max.Date;
                        break;
                    case PeriodType.Last7:
                        workerData.X1Min = workerData.X1Max.AddDays(-7).Date;
                        break;
                    case PeriodType.Last30:
                        workerData.X1Min = workerData.X1Max.AddDays(-30).Date;
                        break;
                    case PeriodType.Last365:
                        workerData.X1Min = workerData.X1Max.AddDays(-365).Date;
                        break;
                    default:
                        // Unexpected
                        throw new ApplicationException("An unexpected report duration value was selected.");
                }

                workerData.dateRanges = new List<DateRangeOffset>();
                dro.UtcStart = workerData.X1Min.ToUniversalTime();
                dro.UtcEnd = workerData.X1Max.ToUniversalTime();
                dro.UtcOffsetMinutes = TimeZone.CurrentTimeZone.GetUtcOffset(workerData.X1Min).TotalMinutes;
                workerData.dateRanges.Add(dro);
                workerData.periodDescription = string.Format(
                    "{0} ({1} - {2})",
                    ((ValueListItem)periodCombo.SelectedItem).DisplayText,
                    workerData.X1Min,
                    workerData.X1Max);
            }
        }

        public virtual bool CanRunReport(out string message)
        {
            message = String.Empty;
            return true;
        }

        virtual protected void tagsComboBox_SelectionChanged(object sender, EventArgs e)
        {
            InitInstanceCombo();
            SetReportParameters();
        }

        public void RunReport()
        {
            RunReport(false);
        }

        public void RunReport(bool suppressMessage)
        {
            string message;

            if (CanRunReport(out message))
            {
                firstRendering = true;
                Initialize_bgWorker();
            }
            else if (!suppressMessage)
            {
                ApplicationMessageBox.ShowInfo(ParentForm, message);
            }
        }

        virtual protected void runReportButton_Click(object sender, EventArgs e)
        {
            RunReport();
        }

        /// <summary>
        /// Override this function only if you need to customize your initializattion of the background worker.
        /// </summary>
        virtual protected void Initialize_bgWorker()
        {
            using (Log.DebugCall())
            {
                if ((tagsComboBox.SelectedItem != null) && (tagsComboBox.SelectedItem != tagSelectOne) && (((ValueListItem)tagsComboBox.SelectedItem).DataValue != tagSelectOne.DataValue))
                {
                    int intSelectedTagID = (int)(((ValueListItem)tagsComboBox.SelectedItem).DataValue);
                    
                    //get the tag object
                    Tag tag = GetTag(intSelectedTagID);
                    if (tag == null)
                    {
                        tagsComboBox.Items.Remove(tagsComboBox.SelectedItem);
                        tagsComboBox.SelectedIndex = 0;
                        ApplicationMessageBox.ShowInfo(ParentForm, "The selected tag no longer exists. Please select a tag.");
                    }
                }
                // Set the report filter data
                State = UIState.GettingData;
                reportData = new WorkerData();
                reportData.reportViewer = reportViewer;
                reportData.reportType = reportType;
                reportParameters.Clear();
                SetReportParameters();
                reportData.reportParameters = reportParameters;

                //Create the background thread and start it.
                reportData.bgWorker = new BackgroundWorker();
                reportData.bgWorker.WorkerSupportsCancellation = true;
                reportData.bgWorker.DoWork += bgWorker_DoWork;
                reportData.bgWorker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;
                reportData.bgWorker.RunWorkerAsync(reportData);
            }
        }
        /// <summary>
        /// Clear the existing report parameters and then extracts all of the base parameters from the controls that have
        /// been inherited from the base control
        /// InstanceID into reportData.InstanceID
        /// ServerIDs for selected tag into reportData.ServerIDs.
        /// Period into reportData.PeriodType
        /// Interval into reportData.sampleSize
        /// UTCStart, UTCEnd and UTCOffset are populated
        /// </summary>
        virtual protected void SetReportParameters()
        {
            reportParameters.Clear();

            if ((instanceCombo.Visible) && (instanceCombo.SelectedItem != null) && ((ValueListItem)instanceCombo.SelectedItem).DataValue != null)
            {
                reportData.instanceID = ((MonitoredSqlServer)((ValueListItem)instanceCombo.SelectedItem).DataValue).Id;
            }
            if (periodCombo.SelectedItem == null)
            {
                periodCombo.SelectedIndex = 0;
            }
            //set the Tag server IDs to null
            reportData.serverIDs = null;

            // if there is a tag selected, get the server IDs.
            if (reportData.instanceID == 0 && (tagsComboBox.Visible) && (tagsComboBox.SelectedItem != null))
            {
                if (((ValueListItem)tagsComboBox.SelectedItem).DataValue != null)
                {
                    Tag selectedTag = GetTag(((int)((ValueListItem)tagsComboBox.SelectedItem).DataValue));
                    if (selectedTag == null)
                    {
                        InitReport();
                        return;
                    }
                    reportData.serverIDs = GetTag(((int)((ValueListItem)tagsComboBox.SelectedItem).DataValue)).Instances;
                }
                else//if all tags selected then populate with all servers. 
                {
                    IList<int> servers = new List<int>(ApplicationModel.Default.AllInstances.Count);
                    foreach (MonitoredSqlServer server in ApplicationModel.Default.AllInstances.Values)
                    {
                        servers.Add(server.Id);
                    }
                    reportData.serverIDs = servers;
                }
            }

            if ((periodCombo.Visible) && (periodCombo.SelectedItem != null) && ((ValueListItem)periodCombo.SelectedItem).DataValue != null)
            {
                reportData.periodType = (PeriodType)((ValueListItem)periodCombo.SelectedItem).DataValue;
            }
            if ((sampleSizeCombo.Visible) && (sampleSizeCombo.SelectedItem != null) && ((ValueListItem)sampleSizeCombo.SelectedItem).DataValue != null)
            {
                reportData.sampleSize = (SampleSize)((ValueListItem)sampleSizeCombo.SelectedItem).DataValue;
            }
            if ((sourceCombo.Visible) && (sourceCombo.SelectedItem != null))
            {
                reportData.SrcTemplateId = ((System.Collections.Generic.KeyValuePair<int, string>)sourceCombo.SelectedItem).Key;
            }
            if ((targetCombo.Visible) && (targetCombo.SelectedItem != null))
            {
                reportData.TrgTemplateId = ((System.Collections.Generic.KeyValuePair<int, string>)targetCombo.SelectedItem).Key;
            }
            SetDateRanges(reportData);
        }

        virtual protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        virtual protected void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        public void CancelReportRefresh()
        {
            reportData.Cancel();
            State = UIState.Cancelled;
        }

        virtual protected void InitializeReportViewer()
        {
            reportViewer.RenderingBegin += reportViewer_RenderingBegin;
            reportViewer.RenderingComplete += reportViewer_RenderingComplete;
            reportViewer.ReportError += reportViewer_ReportError;
            reportViewer.Drillthrough += reportViewer_Drillthrough;
            reportViewer.ReportRefresh += reportViewer_ReportRefresh;
        }

        virtual protected void reportViewer_Drillthrough(object sender, DrillthroughEventArgs e)
        {
            Log.Debug("DrillThrough event for ", sender.GetHashCode());
        }

        virtual protected void reportViewer_RenderingBegin(object sender, CancelEventArgs e)
        {
            renderStart = DateTime.Now;
            Log.Debug("RenderingBegin event for ", sender.GetHashCode());
            State = UIState.Rendering;
        }

        // This generally occurs when the user switches to or from print layout view.
        virtual protected void reportViewer_ReportRefresh(object sender, CancelEventArgs e)
        {
            RunReport();
            Log.Debug("ReportRefresh event for ", sender.GetHashCode());
        }

        virtual protected void reportViewer_ReportError(object sender, ReportErrorEventArgs e)
        {
            // Generally, errors occur when the datasource does not have enough
            // rows for the report viewer to generate one or more charts. 

            Log.Warn("ReportError event for ", sender.GetHashCode());
            //ReportViewer senderObject = (ReportViewer)sender;

            // Make sure this event is for the currently active instance of ReportViewer.
            if (sender == reportViewer)
            {
                Log.Warn("ReportErrorEventArgs.Exception = ", e.Exception);
                Log.Warn("ReportErrorEventArgs.Handled   = ", e.Handled);

                // Don't display an error for a cancelled report.
                //if (cancelledLabel.Visible == false)
                //{
                State = UIState.ReportError;
                ApplicationMessageBox.ShowError(this, e.Exception);
                //}
            }
        }

        // This is called when rendering is cancelled or completed.
        virtual protected void reportViewer_RenderingComplete(object sender, RenderingCompleteEventArgs e)
        {
            Log.Debug("RenderingComplete event for ", sender.GetHashCode());

            // Make sure this event is for the currently active instance of ReportViewer.
            if (sender == reportViewer)
            {
                Log.Debug("Rendering took ", DateTime.Now - renderStart);
                if (e.Exception == null)
                {
                    State = UIState.Rendered;
                }
                else
                {
                    Log.Warn("RenderingCompleteEventArgs.Exception = ", e.Exception);

                    if (e.Exception is OperationCanceledException)
                    {
                        //State = UIState.Cancelled;
                    }
                    else
                    {
                        State = UIState.ReportError;
                    }
                }

                if (e.Warnings != null)
                {
                    foreach (Warning warning in e.Warnings)
                    {
                        Log.Warn("RenderingCompleteEventArgs warning = ", warning);
                    }
                }
            }
        }

        protected static bool ListsAreEqual(List<Triple<string, string, bool>> l1, List<Triple<string, string, bool>> l2)
        {
            if (l1 == l2) return true; // Includes both = null.

            // At least one is not null.
            if (l1 == null || l2 == null) return false;

            // Both are not null.
            if (l1.Count != l2.Count) return false;

            return Algorithms.EqualCollections(l1, l2);
        }

        protected static bool ListsAreEqual(List<MonitoredSqlServer> l1, List<MonitoredSqlServer> l2)
        {
            if (l1 == l2) return true; // Includes both = null.

            // At least one is not null.
            if (l1 == null || l2 == null) return false;

            // Both are not null.
            if (l1.Count != l2.Count) return false;

            for (int i = 0; i < l1.Count; ++i)
            {
                if (l1[i].Id != l2[i].Id) return false;
            }

            return true;
        }

        protected static bool ListsAreEqual(List<string> l1, List<string> l2)
        {
            if (l1 == l2) return true; // Includes both = null.

            // At least one is not null.
            if (l1 == null || l2 == null) return false;

            // Both are not null.
            if (l1.Count != l2.Count) return false;

            for (int i = 0; i < l1.Count; ++i)
            {
                if (l1[i] != l2[i]) return false;
            }

            return true;
        }

        protected void MakeCSVList(TextBox textBox, IEnumerable list)
        {
            textBox.Clear();
            if (list != null)
            {
                foreach (object o in list)
                {
                    if (textBox.Text == string.Empty)
                    {
                        textBox.Text = MaybeDelimit(o.ToString());
                    }
                    else
                    {
                        textBox.Text += ", " + MaybeDelimit(o.ToString());
                    }
                }
            }
        }

        // SQL identifiers can contain commas.  If this one does, delemit with [ and ].
        private string MaybeDelimit(string sqlIdentifier)
        {
            if (sqlIdentifier.Contains(",")) return "[" + sqlIdentifier + "]";
            else return sqlIdentifier;
        }

        private void sampleSizeCombo_SelectionChanged(object sender, EventArgs e)
        {
            SetReportParameters();
        }

        protected static string GetServerIdXml(int serverId)
        {
            return (GetServerIdXml(null, serverId));
        }

        protected static string GetServerIdXml(IList<int> servers)
        {
            return (GetServerIdXml(servers, 0));
        }

        protected static string GetServerIdXml(IList<int> servers, int serverId)
        {
            string xml;
            string escapedXml;

            xml = "<Srvrs>";

            if (servers != null)
            {
                foreach (int server in servers)
                {
                    escapedXml = server.ToString();
                    escapedXml = escapedXml.Replace("&", "&amp;");
                    escapedXml = escapedXml.Replace("\"", "&quot;");
                    escapedXml = escapedXml.Replace("<", "&lt;");
                    escapedXml = escapedXml.Replace(">", "&gt;");
                    xml += String.Format("<Srvr ID=\"{0}\"/>", escapedXml);
                }
            }
            else
            {
                if (serverId > 0)
                {
                    escapedXml = serverId.ToString();
                    escapedXml = escapedXml.Replace("&", "&amp;");
                    escapedXml = escapedXml.Replace("\"", "&quot;");
                    escapedXml = escapedXml.Replace("<", "&lt;");
                    escapedXml = escapedXml.Replace(">", "&gt;");
                    xml += String.Format("<Srvr ID=\"{0}\"/>", escapedXml);
                }
            }
            xml += "</Srvrs>";
            return xml;
        }

        /// <summary>
        /// Get a count of all instances that are associated with this tag id
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        protected static int GetInstanceCount(int tagId)
        {
            foreach (Tag tag in ApplicationModel.Default.Tags)
            {
                if (tag.Id == tagId)
                {
                    return tag.Instances.Count;
                }
            }
            return 0;
        }
        protected static ServerVersion GetSqlVersion(int serverId)
        {
            Objects.MonitoredSqlServerStatus serverStatus = ApplicationModel.Default.GetInstanceStatus(serverId);
            if (serverStatus != null && serverStatus.InstanceVersion != null)
            {
                return serverStatus.InstanceVersion;
            }
            return null;
        }

        protected static Tag GetTag(int tagId)
        {
            foreach (Tag tag in ApplicationModel.Default.Tags)
            {
                if (tag.Id == tagId)
                {
                    return tag;
                }
            }
            return null;
        }

        protected static MonitoredSqlServer GetServer(int serverId)
        {
            foreach (MonitoredSqlServer server in ApplicationModel.Default.AllInstances.Values)
            {
                if (serverId == server.Id)
                {
                    return server;
                }
            }
            return null;
        }

        protected void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}
