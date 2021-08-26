using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using BBS.TracerX;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win;
using Microsoft.Reporting.WinForms;
using Microsoft.SqlServer.MessageBox;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class TopTablesFragmentation : DatabaseReport
    {
        private static readonly Logger Log = Logger.GetLogger("Top Tables - Growth");
        private List<ReportParameter> passthroughParameters = new List<ReportParameter>(6);
        
        readonly protected ValueListItem logicalFrag = new ValueListItem(1, "Logical Fragmentation %");
        readonly protected ValueListItem tableSize = new ValueListItem(2, "Data Size");

        public TopTablesFragmentation()
        {
            InitializeComponent();
            base.AdaptFontSize();
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

        public override void InitReport()
        {
            base.InitReport();
            InitializeReportViewer();

            chartType.Items.Add(logicalFrag);
            chartType.Items.Add(tableSize);

            ReportType = ReportTypes.TopTableFrag;

            //periodCombo.SelectedIndex = 1;
            Databases = null;
            chartType.SelectedItem = logicalFrag;
            tableCount.Value = 5;
            sizeComparison.SelectedIndex = 0;
            scanDensityComparison.SelectedIndex = 0;
            sizeFilter.Value = 0;
            scanDensityFilter.Value = 0;
        }

        protected override void InitPeriodCombo()
        {
            //periodCombo.Items.Add(periodToday);
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
                    IgnorePeriodSelection = true;
                    customDates = new List<DateRangeOffset>();
                    DateRangeOffset.AddDateRange(customDates, startRange, endRange);
                    periodCombo.SelectedItem = periodSetCustom;
                    UpdateSampleSizes(false);
                    IgnorePeriodSelection = false;
                }
            }
            else
            {
                periodCombo.SelectedItem = period7;
            }

            UpdateRangeLabel();
        }


        protected override void SetReportParameters()
        {
            base.SetReportParameters();

            if (chartType.SelectedItem != null)
            {
                passthroughParameters.Clear();
                reportParameters.Add("ReturnData", ((ValueListItem)chartType.SelectedItem).DataValue.ToString());
                passthroughParameters.Add(new ReportParameter("ReturnData", ((ValueListItem)chartType.SelectedItem).DataValue.ToString()));
                passthroughParameters.Add(new ReportParameter("executionStart", DateTime.Now.ToString()));
            }

            if (Databases != null && Databases.Count > 0)
                reportParameters.Add("Database", Databases[0]);
            else
                reportParameters.Add("Database", "");
            if (Databases != null && Databases.Count > 0)
                reportParameters.Add("DatabaseNameFilter", Databases[0]);
            else
                reportParameters.Add("DatabaseNameFilter", null);

            reportParameters.Add("TopN", tableCount.Text.ToString());

            switch (sizeComparison.SelectedIndex)
            {
                case 0:
                    reportParameters.Add("MinSizeMB", sizeFilter.Text.ToString());
                    reportParameters.Add("MaxSizeMB", null);
                    break;
                case 1:
                    reportParameters.Add("MinSizeMB", null);
                    reportParameters.Add("MaxSizeMB", sizeFilter.Text.ToString());
                    break;
                default:
                    reportParameters.Add("MinSizeMB", null);
                    reportParameters.Add("MaxSizeMB", null);
                    break;
            }

            switch (scanDensityComparison.SelectedIndex)
            {
                case 0:
                    reportParameters.Add("MinLogicalFragmentation", scanDensityFilter.Text.ToString());
                    reportParameters.Add("MaxLogicalFragmentation", null);
                    break;
                case 1:
                    reportParameters.Add("MinLogicalFragmentation", null);
                    reportParameters.Add("MaxLogicalFragmentation", scanDensityFilter.Text.ToString());
                    break;
                default:
                    reportParameters.Add("MinLogicalFragmentation", null);
                    reportParameters.Add("MaxLogicalFragmentation", null);
                    break;
            }

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

            if (periodCombo.SelectedItem != periodSetCustom) return;
            reportParameters.Add("rsStart", reportData.dateRanges[0].UtcStart.ToLocalTime().ToString("yyyy'-'MM'-'dd HH':'mm':'ss"));
            reportParameters.Add("rsEnd", reportData.dateRanges[0].UtcEnd.ToLocalTime().ToString("yyyy'-'MM'-'dd HH':'mm':'ss"));

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
            TimeSpan span = new TimeSpan(0);

            foreach (DateRangeOffset range in customDates)
            {
                span += range.UtcEnd.ToLocalTime().Date - range.UtcStart.ToLocalTime().Date;
            }
            if (span.TotalDays < 500.0) sampleSizeCombo.Items.Add(sampleDays);
            if (first.Month != last.Month || first.Year != last.Year) sampleSizeCombo.Items.Add(sampleMonths);
            if (first.Year != last.Year) sampleSizeCombo.Items.Add(sampleYears);
        }

        /// <summary>
        /// Show the appropriate sample size options based on the period.
        /// </summary>
        /// <param name="selectDefault"></param>
        protected override void UpdateSampleSizes(bool selectDefault)
        {
            ValueListItem selected = (ValueListItem)sampleSizeCombo.SelectedItem;

            sampleSizeCombo.Items.Clear();

            if (periodCombo.SelectedItem == periodToday
                || periodCombo.SelectedItem == period7
                || periodCombo.SelectedItem == period30)
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
            else
            {
                sampleSizeCombo.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// This method is returning the report parameters.  The names must match what is set in the RDL file.
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, object> GetReportParmeters()
        {
            reportParameters.Clear();
            reportParameters.Add("ServerId", reportData.instanceID.ToString());
            reportParameters.Add("Period", ((int)reportData.periodType).ToString());
            //reportParameters.Add("UTCStart", reportData.dateRanges[0].UtcStart.ToString());
            //reportParameters.Add("UTCEnd", reportData.dateRanges[0].UtcEnd.ToString());
            //reportParameters.Add("UTCOffset", reportData.dateRanges[0].UtcOffsetMinutes.ToString());
            if (Databases != null && Databases.Count > 0)
                reportParameters.Add("DatabaseNameFilter", Databases[0]);
            else
                reportParameters.Add("DatabaseNameFilter", "< Select a Database (Optional) >");
            reportParameters.Add("TopN", tableCount.Text.ToString());
            reportParameters.Add("ReturnData", ((ValueListItem)chartType.SelectedItem).DataValue.ToString());
            reportParameters.Add("Interval", ((int)reportData.sampleSize).ToString());

            switch (sizeComparison.SelectedIndex)
            {
                case 0:
                    reportParameters.Add("MinSizeMB", sizeFilter.Text.ToString());
                    reportParameters.Add("MaxSizeMB", null);
                    break;
                case 1:
                    reportParameters.Add("MinSizeMB", null);
                    reportParameters.Add("MaxSizeMB", sizeFilter.Text.ToString());
                    break;
                default:
                    reportParameters.Add("MinSizeMB", null);
                    reportParameters.Add("MaxSizeMB", null);
                    break;
            }

            switch (scanDensityComparison.SelectedIndex)
            {
                case 0:
                    reportParameters.Add("MinLogicalFragmentation", scanDensityFilter.Text.ToString());
                    reportParameters.Add("MaxLogicalFragmentation", null);
                    break;
                case 1:
                    reportParameters.Add("MinLogicalFragmentation", null);
                    reportParameters.Add("MaxLogicalFragmentation", scanDensityFilter.Text.ToString());
                    break;
                default:
                    reportParameters.Add("MinLogicalFragmentation", null);
                    reportParameters.Add("MaxLogicalFragmentation", null);
                    break;
            }
            if (periodCombo.SelectedItem == periodSetCustom)
            {
                reportParameters.Add("rsStart",
                                     reportData.dateRanges[0].UtcStart.ToLocalTime().ToString(
                                         "yyyy'-'MM'-'dd HH':'mm':'ss"));
                reportParameters.Add("rsEnd",
                                     reportData.dateRanges[0].UtcEnd.ToLocalTime().ToString(
                                         "yyyy'-'MM'-'dd HH':'mm':'ss"));
            }
            return reportParameters;
        }

        override protected void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "ReportControlWorker";

            using (Log.DebugCall())
            {
                WorkerData localReportData = (WorkerData)e.Argument;
                ReportDataSource dataSource;

                Log.Debug("localReportData.reportType = ", localReportData.reportType);
                localReportData.dataSources = new ReportDataSource[4];

                passthroughParameters.Add(new ReportParameter("ServerId", reportData.instanceID.ToString()));
                passthroughParameters.Add(new ReportParameter("Period", ((int)reportData.periodType).ToString()));
                passthroughParameters.Add(new ReportParameter("UTCOffset", reportData.dateRanges[0].UtcOffsetMinutes.ToString()));
                passthroughParameters.Add(new ReportParameter("DatabaseNameFilter", (string)localReportData.reportParameters["DatabaseNameFilter"]));
                passthroughParameters.Add(new ReportParameter("Interval", ((int)reportData.sampleSize).ToString()));
                passthroughParameters.Add(new ReportParameter("GUIServer", (string)localReportData.reportParameters["GUIServer"]));
                passthroughParameters.Add(new ReportParameter("TopN", (string)localReportData.reportParameters["TopN"]));

                if (localReportData.reportParameters.ContainsKey("rsStart")) passthroughParameters.Add(new ReportParameter("rsStart",
                                                                   localReportData.reportParameters["rsStart"].ToString()));
                if (localReportData.reportParameters.ContainsKey("rsEnd")) passthroughParameters.Add(new ReportParameter("rsEnd",
                                                                   localReportData.reportParameters["rsEnd"].ToString())); 

                dataSource = new ReportDataSource("ScanDensity");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTopTablesFragmentation",
                                                                  localReportData.instanceID,
                                                                  (string)localReportData.reportParameters["Database"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  (string)localReportData.reportParameters["MinSizeMB"],
                                                                  (string)localReportData.reportParameters["MaxSizeMB"],
                                                                  (string)localReportData.reportParameters["MinLogicalFragmentation"],
                                                                  (string)localReportData.reportParameters["MaxLogicalFragmentation"],
                                                                  (string)localReportData.reportParameters["TopN"],
                                                                  0,
                                                                  0);
                localReportData.dataSources[0] = dataSource;

                dataSource = new ReportDataSource("LogicalFragmentation");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTopTablesFragmentation",
                                                                  localReportData.instanceID,
                                                                  (string)localReportData.reportParameters["Database"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  (string)localReportData.reportParameters["MinSizeMB"],
                                                                  (string)localReportData.reportParameters["MaxSizeMB"],
                                                                  (string)localReportData.reportParameters["MinLogicalFragmentation"],
                                                                  (string)localReportData.reportParameters["MaxLogicalFragmentation"],
                                                                  (string)localReportData.reportParameters["TopN"],
                                                                  1,
                                                                  0);
                localReportData.dataSources[1] = dataSource;

                dataSource = new ReportDataSource("DataSize");
                dataSource.Value = RepositoryHelper.GetReportData("p_GetTopTablesFragmentation",
                                                                  localReportData.instanceID,
                                                                  (string)localReportData.reportParameters["Database"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  (string)localReportData.reportParameters["MinSizeMB"],
                                                                  (string)localReportData.reportParameters["MaxSizeMB"],
                                                                  (string)localReportData.reportParameters["MinLogicalFragmentation"],
                                                                  (string)localReportData.reportParameters["MaxLogicalFragmentation"],
                                                                  (string)localReportData.reportParameters["TopN"],
                                                                  2,
                                                                  0);
                localReportData.dataSources[2] = dataSource;



                dataSource = new ReportDataSource("ChartData");
                string topNmaxString = (string) localReportData.reportParameters["TopN"];
                int topNmax = 20;
                int.TryParse(topNmaxString, out topNmax);

                dataSource.Value = RepositoryHelper.GetReportData("p_GetTopTablesFragmentationChart",
                                                                  localReportData.instanceID,
                                                                  (string)localReportData.reportParameters["Database"],
                                                                  localReportData.dateRanges[0].UtcStart,
                                                                  localReportData.dateRanges[0].UtcEnd,
                                                                  localReportData.dateRanges[0].UtcOffsetMinutes.ToString(),
                                                                  (string)localReportData.reportParameters["MinSizeMB"],
                                                                  (string)localReportData.reportParameters["MaxSizeMB"],
                                                                  (string)localReportData.reportParameters["MinLogicalFragmentation"],
                                                                  (string)localReportData.reportParameters["MaxLogicalFragmentation"],
                                                                  (topNmax > 20 ? "20" : topNmax.ToString()),
                                                                  (string)localReportData.reportParameters["ReturnData"],
                                                                  localReportData.sampleSize);
                localReportData.dataSources[3] = dataSource;

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
                        if (e.Error.GetType() == typeof(SqlException) &&
                            e.Error.Message.ToLower().Contains("msxmlsql.dll"))//
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
                                "Idera.SQLdm.DesktopClient.Views.Reports.ReportDefinitions.TopTablesFragmentation.rdl"))
                            {
                                reportViewer.LocalReport.LoadReportDefinition(stream);
                            }
                            foreach (ReportDataSource dataSource in reportData.dataSources)
                            {
                                reportViewer.LocalReport.DataSources.Add(dataSource);
                            }

                            reportViewer.LocalReport.SetParameters(passthroughParameters);

                            reportViewer.RefreshReport();
                            reportViewer.LocalReport.DisplayName = "TopTablesFragmentation";
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
            periodCombo.SelectedIndex = 0;
            Databases = null;
            chartType.SelectedItem = logicalFrag;
            sizeComparison.SelectedIndex = 0;
            scanDensityComparison.SelectedIndex = 0;
            sizeFilter.Text = "0";
            scanDensityFilter.Text = "0";
            tableCount.Text = "5";
        }

        private void tableCount_Leave(object sender, EventArgs e)
        {
            if (this.tableCount.Text.Equals(string.Empty))
            {
                this.tableCount.Text = this.tableCount.Minimum.ToString();
            }
        }

        private void sizeFilter_Leave(object sender, EventArgs e)
        {
            if (this.sizeFilter.Text.Equals(string.Empty))
            {
                this.sizeFilter.Text = this.sizeFilter.Minimum.ToString();
            }
        }

        private void scanDensityFilter_Leave(object sender, EventArgs e)
        {
            if (this.scanDensityFilter.Text.Equals(string.Empty))
            {
                this.scanDensityFilter.Text = this.scanDensityFilter.Minimum.ToString();
            }
        }
    }
}

