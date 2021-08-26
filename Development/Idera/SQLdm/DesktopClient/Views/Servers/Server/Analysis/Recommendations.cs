//------------------------------------------------------------------------------
// SQLdm 10.0 Srishti Purohit
//     For doctors UI implementation in DM
// 07/07/2015
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using ChartFX.WinForms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.Thresholds;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Wintellect.PowerCollections;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;
using Idera.SQLdm.Common.Events;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.DesktopClient.Presenters.GridEntries;
using System.Text;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Objects;
using System.IO;
using Idera.SQLdm.StandardClient.Dialogs;
using Idera.SQLdm.Common.UI.Controls;
using Infragistics.Win.UltraWinEditors;
using System.Linq;
using System.Reflection;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Resources;
using System.Collections;
using System.Text.RegularExpressions;
using Infragistics.Excel;
using Infragistics.Win.UltraWinDataSource;
using System.Threading;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Analysis
{
    internal partial class Recommendations : ServerBaseView
    {
        #region constants
        private const string SYSADMIN_MESSAGE = @"No data is available for this view"; //SQLDM 10.3.2. Changed the message text from the one below. 
        #endregion



        #region fields

        private DataTable chartRealTimeDataTable;
        private bool chartRealTimeDataTablePrePopulated = false;
        private Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result currentSnapshot = null;

        private readonly Dictionary<int, Triple<string, DataRow, DataColumn>> lookupTable =
            new Dictionary<int, Triple<string, DataRow, DataColumn>>();
        private readonly Dictionary<int, FieldMap> chartFieldMapLookupTable = new Dictionary<int, FieldMap>();
        private UltraGridColumn selectedColumn = null;
        private bool initialized = false;
        private Control focused = null;
        private static readonly object updateLock = new object();
        private DataTable historicalStatisticsDataTable = null;

        private DateTime? currentHistoricalSnapshotDateTime = null;
        private DateTime? historicalSnapshotDateTime = null;
        private DataTable chartHistoricalDataTable;
        private Exception historyModeLoadError = null;
        private bool isRealTimeView = false;

        private List<IRecommendation> ListOfRecommendations;
        private Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result result;
        private delegate void ShowDetailsGrid();

        //To handle Optmization and undo buttons enabling disabling
        private bool isOptimizable = false;

        private bool isUndoSupported = false;

        private bool isShowProblemSupported = false;
        private bool isWorkLoadAnalysis = false;
        string queryFromDignose = string.Empty;
        string databaseNameDiagnose = string.Empty;

        //For background worker implementation
        BackgroundWorker m_oWorker;
        bool isAnalysisDone = true;
        ConstructorType currentConstructorType = ConstructorType.analysis;
        private object sync = new object();

        //bool sysAdmin = true;//Praveen Suhalka --Sqldm10.0.0
        private bool isCategoriesNotSelected = false; // SQLdm 10.0 Srishti Purohit  -- To support check of 'if required categories are selected'
        #endregion

        #region constructors

        public Recommendations(int instanceId, bool isWorkload)
            : base(instanceId)
        {
            //sysAdmin = isSysAdmin(instanceId); //Praveen Suhalka --Sqldm10.0.0
            IsAnalysisDone = false;
            isRealTimeView = true;
            GetMasterRecommendations();
            InitializeComponent();
            detailsGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            this.queryFromDignose = string.Empty;
            this.isWorkLoadAnalysis = isWorkload;
            defaultStatusOfToolbar();
            detailsGridMessageLabel.Visible = false;

            InitializeDataSources();
            if (isUserSysAdmin)
            {
                callBackgroundWorker(ConstructorType.analysis);
            }
            else
            {
                detailsGridMessageLabel.Text = SYSADMIN_MESSAGE;
                IsAnalysisDone = true;
            }

            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }
        //To support diagnose query functions
        public Recommendations(string queryToDiagnose, string database, int instanceId)
            : base(instanceId)
        {
            //sysAdmin = isSysAdmin(instanceId); //Praveen Suhalka --Sqldm10.0.0
            IsAnalysisDone = false;
            isRealTimeView = true;
            GetMasterRecommendations();
            InitializeComponent();
            detailsGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            this.queryFromDignose = queryToDiagnose;
            this.databaseNameDiagnose = database;
            defaultStatusOfToolbar();
            InitializeDataSources();
            detailsGridMessageLabel.Visible = false;
            if (isUserSysAdmin)
            {
                callBackgroundWorker(ConstructorType.diagnose);
            }
            else
            {
                detailsGridMessageLabel.Text = SYSADMIN_MESSAGE;
                IsAnalysisDone = true;
            }
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }
        public Recommendations(int instanceId, DateTime? historySnapShotTime)
            : base(instanceId)
        {
            IsAnalysisDone = true;
            if (historySnapShotTime == null)
            {
                isRealTimeView = true;
            }
            this.queryFromDignose = string.Empty;
            GetMasterRecommendations();
            InitializeComponent();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
            detailsGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            historicalSnapshotDateTime = historySnapShotTime;
            defaultStatusOfToolbar();

            InitializeDataSources();
            currentConstructorType = ConstructorType.history;
            detailsGridMessageLabel.Visible = false;
            callBackgroundWorker(ConstructorType.history);
        }

        #endregion

        #region properties

        public event EventHandler GridGroupByBoxVisibleChanged;
        public event EventHandler ScriptAnalysisActionAllowedChanged;

        public bool GridGroupByBoxVisible
        {
            get { return !detailsGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                detailsGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (GridGroupByBoxVisibleChanged != null)
                {
                    GridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set
            {
                historicalSnapshotDateTime = value;
                currentHistoricalSnapshotDateTime = null;
            }
        }
        public bool IsOptimizable
        {
            get { return isOptimizable; }
            set
            {
                isOptimizable = value;

                if (ScriptAnalysisActionAllowedChanged != null)
                {
                    ScriptAnalysisActionAllowedChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool IsUndoSupported
        {
            get { return isUndoSupported; }
            set
            {
                isUndoSupported = value;
                if (ScriptAnalysisActionAllowedChanged != null)
                {
                    ScriptAnalysisActionAllowedChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool IsShowProblemSupported
        {
            get { return isShowProblemSupported; }
            set
            {
                isShowProblemSupported = value;
                if (ScriptAnalysisActionAllowedChanged != null)
                {
                    ScriptAnalysisActionAllowedChanged(this, EventArgs.Empty);
                }
            }
        }
        public bool IsAnalysisDone
        {
            get { return isAnalysisDone; }
            set
            {
                isAnalysisDone = value;
                if (ScriptAnalysisActionAllowedChanged != null)
                {
                    ScriptAnalysisActionAllowedChanged(this, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Get the availability of enabling optimization button determined by whether recommendation selected supports IsScriptProvider
        /// </summary>
        public bool OptimizationAllowed
        {
            get
            {
                return IsOptimizable;
            }
        }
        /// <summary>
        /// Get the availability of enabling undo button determined by whether recommendation selected supports IsUndoScriptProvider
        /// </summary>
        public bool UndoAllowed
        {
            get
            {
                return IsUndoSupported;
            }
        }
        /// <summary>
        /// Get the availability of enabling show problem button determined by whether recommendation implements TSqlRecommendation
        /// </summary>
        public bool ShowProblemAllowed
        {
            get
            {
                return IsShowProblemSupported;
            }
        }

        public bool IsWorkLoadAnalysis
        {
            get { return isWorkLoadAnalysis; }
        }
        #endregion

        #region methods
        //call to back ground worker
        private void callBackgroundWorker(ConstructorType owner)
        {
            try
            {
                currentConstructorType = owner;
                m_oWorker = new BackgroundWorker();

                // Create a background worker thread that ReportsProgress &
                // SupportsCancellation
                // Hook up the appropriate events.
                if (owner == ConstructorType.analysis)
                {
                    m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWorkAnlysis);
                }
                else if (owner == ConstructorType.diagnose)
                {
                    m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWorkDignose);
                }
                else if (owner == ConstructorType.history)
                {
                    m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWorkHistory);
                }
                m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWorkCleanUp);
                m_oWorker.ProgressChanged += new ProgressChangedEventHandler
                        (m_oWorker_ProgressChanged);
                m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler
                        (m_oWorker_RunWorkerCompleted);
                m_oWorker.WorkerReportsProgress = true;
                m_oWorker.WorkerSupportsCancellation = true;
                isAnalysisDone = false;
                // Kickoff the worker thread to begin it's DoWork function.
                m_oWorker.RunWorkerAsync();
                if (m_oWorker.IsBusy)
                {
                    try
                    {
                        m_oWorker.ReportProgress(33);
                    }
                    catch (Exception ex)
                    { }
                }
                detailsGridMessageLabel.Visible = false;
            }
            catch (Exception ex) { }
        }


        private void defaultStatusOfToolbar()
        {
            IsOptimizable = false;
            IsUndoSupported = false;
            IsShowProblemSupported = false;
        }

        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.AnalysisView);
        }
        //To get master recommendation data from database through Management Service
        private void GetMasterRecommendations()
        {
            try
            {
                if (MasterRecommendations.MasterRecommendationsInformation == null || MasterRecommendations.MasterRecommendationsInformation.Count < 1)
                {
                    IManagementService managementService =
                                            ManagementServiceHelper.GetDefaultService(
                                                Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                    if (managementService != null)
                    {
						MasterRecommendations.MasterRecommendationsInformation = managementService.GetMasterRecommendations();
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Not able to get master recommendation list from database. ", ex);
                Log.Error("Not able to get master recommendation list from database. ", ex);
                //throw new Exception("Not able to get master recommendation list from database. " + ex.Message);
            }
        }

        public override void RefreshView()
        {
            // Allow refresh if in real-time mode or if in historical mode and last loaded historical snapshot is stale
            if (HistoricalSnapshotDateTime == null ||
                HistoricalSnapshotDateTime != null && HistoricalSnapshotDateTime != currentHistoricalSnapshotDateTime)
            {
                historyModeLoadError = null;
                base.RefreshView();
            }
        }

        public override object DoRefreshWork(BackgroundWorker backgroundWorker)
        {
            try
            {
                //if (string.IsNullOrEmpty(queryFromDignose))
                //{
                using (Log.InfoCall("DoRefreshWork"))
                {

                    DateTime? historyDateTime = HistoricalSnapshotDateTime;
                    if (historyDateTime == null)
                    {
                        Log.Info("Getting real-time snapshot.");
                        return GetRealTimeSnapshot();
                    }
                    else
                    {
                        Log.InfoFormat("Populating historical snapshots (end={0}).", historyDateTime.Value);

                        PopulateHistoricalSnapshots();
                        if (ListOfRecommendations != null && ListOfRecommendations.Count > 0)
                        {
                            ShowDetailsGridMethod();
                        }
                        return ListOfRecommendations;
                    }
                }
                //}
                //else
                //    return ListOfRecommendations;
            }
            catch (Exception ex)
            {
                //ApplicationMessageBox.ShowError(this, "Not able to show records.", ex);
                Log.Error(ex.Message + ": Not able to show records. Error in back ground worker running for recommendations.");
                return ex;
                //return null;
            }
        }

        //Fucntion to show grid
        private void ShowDetailsGridMethod()
        {
            if (detailsGrid.InvokeRequired) // This branch calls the delegate
            {
                ShowDetailsGrid theDelegate = new ShowDetailsGrid(ShowDetailsGridMethod);
                detailsGrid.Invoke(theDelegate);
            }
            else
            {
                if (detailsGrid.Rows != null)
                    detailsGrid.Visible = detailsGrid.Rows.FilteredInRowCount > 0;
                else
                    detailsGrid.Visible = false;
                if (detailsGrid.Visible)
                    detailsGrid.BringToFront();
            }
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            if (HistoricalSnapshotDateTime != null)
            {
                historyModeLoadError = e;
                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotLoadErrorViewLabel;
                RunAnalysisView_Fill_Panel.Visible = false;
            }

            base.HandleBackgroundWorkerError(e);
        }

        private Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result GetRealTimeSnapshot()
        {
            if (!chartRealTimeDataTablePrePopulated)
            {
                //PrePopulateRealTimeDataTable();
                chartRealTimeDataTablePrePopulated = true;
            }
            else
                BackfillScheduledRefreshData();

            return result;
        }

        private void BackfillScheduledRefreshData()
        {
            var rtdt = this.chartRealTimeDataTable;
            var lastRowIndex = rtdt.Rows.Count - 1;
            if (lastRowIndex <= 0)
            {
                Log.Info("No data - doing prepopulate");
                // if no rows then prepopulate should do what we need
                //PrePopulateRealTimeDataTable();
                return;
            }

            var now = DateTime.Now;
            var lastRow = rtdt.Rows[lastRowIndex];
            var lastDate = (DateTime)lastRow["Date"];
            var timeDiff = now - lastDate;
            if (timeDiff > TimeSpan.FromMinutes(Settings.Default.RealTimeChartHistoryLimitInMinutes))
            {
                Log.InfoFormat("Last data point is from {0} ({1} old) - doing prepopulate to reload data", lastDate, timeDiff);
                // if last data point is older than our grooming period then prepopulate should work
                //PrePopulateRealTimeDataTable();
                return;
            }

            var refreshInterval = TimeSpan.FromSeconds(Settings.Default.ForegroundRefreshIntervalInSeconds * 3);
            if (timeDiff <= refreshInterval || timeDiff.TotalMinutes < 1.0)
            {
                Log.VerboseFormat("Backfill skipped due to time difference between now and the last data point.  dif:{0}", timeDiff);
                return;
            }

            //UpdateRealTimeData(recommSummary, false);
        }

        private List<IRecommendation> PopulateHistoricalSnapshots()
        {
            currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
            ListOfRecommendations =
                RepositoryHelper.GetRecommendationHistory(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                  instanceId, HistoricalSnapshotDateTime.Value,
                                                  ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);


            
            updateGridDataSource();
            return ListOfRecommendations;
        }


        public override void UpdateData(object data)
        {
            try
            {
                lock (updateLock)
                {
                    if (!initialized)
                    {
                    }

                    if (HistoricalSnapshotDateTime == null)
                    {
                        if (data is Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result)
                        {
                            UpdateDataWithRealTimeSnapshot((Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result)data);
                        }

                    }
                    else
                    {
                        UpdateDataWithHistoricalSnapshot((List<IRecommendation>)data);
                        ApplicationController.Default.RefreshActiveView();
                        return;
                    }

                    ShowDetailsGridMethod();
                    detailsGrid.DisplayLayout.Rows.EnsureSortedAndFiltered();
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Unable to show records.", ex);
                Log.Error(ex.Message + ": Not able to show records. Error in updating data for recommendations.");
                //throw new Exception("Not able to show records.");
            }
            finally
            {
                ApplicationController.Default.OnRefreshActiveViewCompleted(
                    new RefreshActiveViewCompletedEventArgs(DateTime.Now));
            }
        }

        private void UpdateDataWithRealTimeSnapshot(Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result snapshot)
        {
            RunAnalysisView_Fill_Panel.Visible = true;


            currentSnapshot = snapshot;
            if (snapshot != null && snapshot.AnalyzerRecommendationList.Count > 0)
            {
                foreach (AnalyzerResult analysisRes in snapshot.AnalyzerRecommendationList)
                {
                    ListOfRecommendations.AddRange(analysisRes.RecommendationList);
                }
                //Turn on Visibility
                RunAnalysisView_Fill_Panel.Visible = true;

                updateGridDataSource();
            }
            //if (detailsGrid.Rows != null)
            //    ApplicationController.Default.SetCustomStatus(
            //        String.Format("Recommendation Details: {0} Item{1}",
            //                      detailsGrid.Rows.Count,
            //                      detailsGrid.Rows.Count == 1 ? string.Empty : "s")
            //        );

        }
        private void UpdateDataWithHistoricalSnapshot(List<IRecommendation> snapshot)
        {
            if (snapshot != null && snapshot.Count > 0)
            {
                currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime;

                RunAnalysisView_Fill_Panel.Visible = true;
                ListOfRecommendations = snapshot;
                //snapshot = snapshot.GroupBy(x => x.FindingText).Select(y => y.First()).ToList<IRecommendation>();

                updateGridDataSource();

            }
            else
            {
                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotDoesNotExistAnalyzeView;
                //Turn off visibility
                RunAnalysisView_Fill_Panel.Visible = false;

            }
        }
        //To marshaled the function call back to the UI thread.
        private void updateGridDataSource()
        {
            //Lock to keep only one thread enter at a time
            if (Monitor.TryEnter(sync, 100))
            {
                try
                {
                    detailsGridDataSource.SuspendBindingNotifications();
                    if (detailsGridDataSource.Rows != null && detailsGridDataSource.Rows.Count > 0)
                        detailsGridDataSource.Rows.Clear();
                    if (ListOfRecommendations != null)
                    {
                        //ListOfRecommendations = ListOfRecommendations.GroupBy(x => x.FindingText).Select(y => y.First()).ToList<IRecommendation>();

                        for (int rowNum = 0; rowNum < ListOfRecommendations.Count; rowNum++)
                        {
                            InitializeRecommendation(ListOfRecommendations[rowNum], rowNum);
                        }
                    }
                    detailsGridDataSource.ResumeBindingNotifications();
                }
                finally
                {
                    Monitor.Exit(sync);
                }
            }
        }

        private void InitializeRecommendation(IRecommendation recommendation, int index)
        {
            Infragistics.Win.UltraWinDataSource.UltraDataRow metricRow = detailsGridDataSource.Rows.Add();
            metricRow["Flags"] = getFlagState(recommendation.IsFlagged);
            metricRow["OptimizationStatus"] = getOptimizationStatusImage(recommendation.OptimizationStatus);
            metricRow["AnalysisRecommendationID"] = recommendation.AnalysisRecommendationID;
            metricRow["Recommendation ID"] = recommendation.ID;

            //new object[] { getFlagState(recommendation.IsFlagged), getOptimizationStatusImage(recommendation.OptimizationStatus), recommendation.AnalysisRecommendationID, recommendation.ID });
            metricRow["Finding Text"] = recommendation.FindingText;
            // 
            // priority
            // 
            PriorityBar priority = new PriorityBar();
            priority.Anchor = System.Windows.Forms.AnchorStyles.Right;
            priority.Location = new System.Drawing.Point(6, 7);
            priority.Name = "priority";
            priority.Size = new System.Drawing.Size(80, 8);
            priority.TabIndex = 3;
            priority.Value = 20F;
            priority.Value = recommendation.ComputedRankFactor;
            Bitmap IMG = new Bitmap(80, 8);
            priority.DrawToBitmap(IMG, new Rectangle(0, 0, 80, 8));
            metricRow["Priority"] = IMG;
            metricRow["Priority Value"] = recommendation.ComputedRankFactor;

        }

        private Image getFlagState(bool isFlagged)
        {
            if (isFlagged)
                return global::Idera.SQLdm.DesktopClient.Properties.Resources.FlagEnabled16;
            else
                return global::Idera.SQLdm.DesktopClient.Properties.Resources.FlagClear16;
        }
        private Image getOptimizationStatusImage(RecommendationOptimizationStatus status)
        {
            Image statusImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.FlagClear16; ;
            switch (status)
            {
                case RecommendationOptimizationStatus.NotOptimized:
                    statusImage = null;
                    break;
                case RecommendationOptimizationStatus.OptimizationCompleted:
                    statusImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.OK16;
                    break;
                case RecommendationOptimizationStatus.OptimizationUndone:
                    statusImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.reverted_16;
                    break;
                case RecommendationOptimizationStatus.OptimizationException:
                case RecommendationOptimizationStatus.OptimizationUndoneException:
                    statusImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.Error16;
                    break;
            }
            return statusImage;
        }

        private void UpdateCellColors(DataTable sourceTable)
        {
            if (sourceTable != null)
            {
                AlertConfiguration alertConfig = ApplicationModel.Default.GetAlertConfiguration(instanceId);

                if (alertConfig != null)
                {
                    detailsGrid.SuspendLayout();
                    if (detailsGrid.Rows != null)
                    {
                        foreach (UltraGridRow gridRow in detailsGrid.Rows.GetAllNonGroupByRows())
                        {
                            DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                            if (dataRowView == null)
                                return;
                            DataRow dataRow = dataRowView.Row;
                            if (!dataRow.IsNull("AlertMetric"))
                                UpdateCellColor(sourceTable, (Metric)dataRow["AlertMetric"], alertConfig, gridRow, 1);
                            gridRow.RefreshSortPosition();
                        }

                    }
                    detailsGrid.ResumeLayout();
                }

                detailsGrid.Refresh();
            }
        }

        private void UpdateCellColor(DataTable sourceTable, Metric metric, AlertConfiguration alertConfig, UltraGridRow gridRow, int adjustmentMultiplier)
        {
            if (sourceTable != null && sourceTable.Rows.Count > 0)
            {
                DataRow valueRow = sourceTable.Rows[sourceTable.Rows.Count - 1];

                UltraGridCell stateCell = gridRow.Cells["State"];
                UltraGridCell infoThresholdCell = gridRow.Cells["Info Threshold"];
                UltraGridCell warningThresholdCell = gridRow.Cells["Warning Threshold"];
                UltraGridCell criticalThresholdCell = gridRow.Cells["Critical Threshold"];
                UltraGridCell metricCell = gridRow.Cells["Metric Type"];

                AlertConfigurationItem alertConfigItem = alertConfig[metric, String.Empty]; // Will need to update this if this metric ever supports multi-thresholds
                if (alertConfigItem == null || !alertConfigItem.ThresholdEntry.IsEnabled)
                {
                    DataRowView dataRowView = (DataRowView)stateCell.Row.ListObject;
                    if (dataRowView == null)
                        return;
                    DataRow dataRow = dataRowView.Row;
                    dataRow["State"] = 0;
                    stateCell.Appearance.ResetBackColor();
                    stateCell.Appearance.ResetForeColor();
                    infoThresholdCell.Value = null;
                    warningThresholdCell.Value = null;
                    criticalThresholdCell.Value = null;
                }
                else
                {
                    Threshold infoThreshold = alertConfigItem.ThresholdEntry.InfoThreshold;
                    if (infoThreshold.Enabled)
                    {
                        if (infoThreshold.Value != null)
                        {
                            object value = infoThreshold.Value;
                            if (value is IFormattable && value.GetType().IsValueType && !value.GetType().IsEnum)
                                infoThresholdCell.Value = ((IFormattable)value).ToString("N0", CultureInfo.CurrentCulture.NumberFormat);
                            else
                                infoThresholdCell.Value = value.ToString();
                        }
                        else
                            infoThresholdCell.Value = String.Empty;
                    }

                    Threshold warningThreshold = alertConfigItem.ThresholdEntry.WarningThreshold;
                    if (warningThreshold.Enabled)
                    {
                        if (warningThreshold.Value != null)
                        {
                            object value = warningThreshold.Value;
                            if (value is IFormattable && value.GetType().IsValueType && !value.GetType().IsEnum)
                                warningThresholdCell.Value = ((IFormattable)value).ToString("N0", CultureInfo.CurrentCulture.NumberFormat);
                            else
                                warningThresholdCell.Value = value.ToString();
                        }
                        else
                            warningThresholdCell.Value = String.Empty;
                    }
                    Threshold criticalThreshold = alertConfigItem.ThresholdEntry.CriticalThreshold;
                    if (criticalThreshold.Enabled)
                    {
                        if (criticalThreshold.Value != null)
                        {
                            object value = criticalThreshold.Value;
                            if (value is IFormattable && value.GetType().IsValueType && !value.GetType().IsEnum)
                                criticalThresholdCell.Value = ((IFormattable)value).ToString("N0", CultureInfo.CurrentCulture.NumberFormat);
                            else
                                criticalThresholdCell.Value = value.ToString();
                        }
                        else
                            criticalThresholdCell.Value = String.Empty;
                    }

                    DataRowView dataRowView = (DataRowView)gridRow.ListObject;
                    DataRow dataRow = dataRowView.Row;
                    if (dataRow.IsNull("Value"))
                    {
                        dataRow["State"] = 0;
                        stateCell.Appearance.ResetBackColor();
                        stateCell.Appearance.ResetForeColor();
                    }

                }
            }
        }
        /// <summary>
        /// SQLdm 10.0 Srishti Purohit Doctor Analysis implementation in DM To Copy recommendations
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        internal void BlockRecommendations(int instanceId)
        {
            try
            {
                using (var d = new BlockRecommendationsDialog(instanceId, GetSelectedRecommendations()))
                {
                    d.ShowDialog(this);
                }

                ApplicationModel.Default.RefreshActiveInstances();
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Unable to block recommendations/databases.", ex);
                Log.Error(ex.Message + ": Not able to show records. Error in back ground worker running for recommendations.");
                //throw new Exception("Unable to block recommendations/databases.");
            }
        }
        /// <summary>
        /// SQLdm 10.0 Srishti Purohit Doctor Analysis implementation in DM To Export recommendations in some file
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        internal void ExportSelectedRecommendations()
        {
            try
            {
                SaveGrid();
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Unable to exoprt.", ex);
                Log.Error(ex.Message + ": Not able to show records. Error in back ground worker running for recommendations.");
                //throw new Exception("Unable to export.");
            }
        }

        private void SaveGrid()
        {
            List<IRecommendation> selectedRecomms = GetSelectedRecommendations();
            if (selectedRecomms.Count > 0)
            {
                Regex illegalInFileName = new Regex(string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidFileNameChars()))), RegexOptions.Compiled);

                saveFileDialog.DefaultExt = "xls";
                saveFileDialog.FileName = String.Format("{0} {1}.xls", ApplicationModel.Default.ActiveInstances[instanceId].InstanceName, DateTime.Now.ToString("MMM dd yyyy hhmmss"));

                saveFileDialog.FileName = illegalInFileName.Replace(saveFileDialog.FileName, "");
                saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
                saveFileDialog.Title = "Save as Excel Spreadsheet";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    UltraGrid toBeExportedGrid = new UltraGrid();
                    toBeExportedGrid.BindingContext = detailsGrid.BindingContext;


                    toBeExportedGrid.DataSource = ToDataTable<IRecommendation>(selectedRecomms);
                    try
                    {
                        // columns to export with a heading
                        //toBeExportedGrid.DisplayLayout.Bands[0].Columns["IsFlagged"].Hidden = true;
                        //toBeExportedGrid.DisplayLayout.Bands[0].Columns["AdditionalConsiderations"].Header.Caption = "Additional Considerations";
                        //toBeExportedGrid.DisplayLayout.Bands[0].Columns["AnalysisRecommendationID"].Hidden = true;
                        //toBeExportedGrid.DisplayLayout.Bands[0].Columns["SourceObjects"].Hidden = true;
                        //toBeExportedGrid.DisplayLayout.Bands[0].Columns["FindingText"].Header.Caption = "Finding";
                        //toBeExportedGrid.DisplayLayout.Bands[0].Columns["RecommendationText"].Header.Caption = "Recommendation";
                        //toBeExportedGrid.DisplayLayout.Bands[0].Columns["RecommendationType"].Hidden = true;
                        //toBeExportedGrid.DisplayLayout.Bands[0].Columns["Relevance"].Hidden = true;
                        //toBeExportedGrid.DisplayLayout.Bands[0].Columns["ComputedRankFactor"].Hidden = true;
                        //toBeExportedGrid.DisplayLayout.Bands[0].Columns["RankID"].Hidden = true;
                        //toBeExportedGrid.DisplayLayout.Bands[0].Columns["ImpactFactor"].Hidden = true;
                        //toBeExportedGrid.DisplayLayout.Bands[0].Columns["ConfidenceFactor"].Hidden = true;
                        //toBeExportedGrid.DisplayLayout.Bands[0].Columns["AffectedBatches"].Hidden = true;

                        //toBeExportedGrid.DisplayLayout.Bands[0].Columns["ProblemExplanationText"].Hidden = true;
                        //toBeExportedGrid.DisplayLayout.Bands[0].Columns["OptimizationErrorMessage"].Hidden = true;
                        //toBeExportedGrid.DisplayLayout.Bands[0].Columns["OptimizationStatus"].Hidden = true;


                        toBeExportedGrid.DisplayLayout.Bands[0].Columns["ImpactExplanationText"].Header.Caption = "Impact Explanation";

                        // 
                        // ultraGridExcelExporter
                        //
                        Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter();
                        ultraGridExcelExporter.CellExporting += new Infragistics.Win.UltraWinGrid.ExcelExport.CellExportingEventHandler(this.ultraGridExcelExporter_CellExporting);

                        ultraGridExcelExporter.Export(toBeExportedGrid, saveFileDialog.FileName);//exporting from the dummy grid object

                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                        Log.Error(ex.Message + ": Error while exporting for recommendations.");
                        //throw new Exception("Unable to export data.");
                    }
                    finally
                    {
                    }
                }
            }
            else
            {
                ApplicationMessageBox.ShowInfo(FindForm(), "No recommendations selected.");
            }

        }

        /*Converts List To DataTable*/
        private DataTable ToDataTable<TSource>(IList<TSource> data)
        {
            DataTable dataTable = new DataTable(typeof(TSource).Name);
            PropertyInfo[] props = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            string[] columnsNameInRequiredSequence = new string[]{
            "ID",
            "Category",
            "FindingText",
            "RecommendationText",
            "ImpactExplanationText",
            "AdditionalConsiderations",
            "Database"  ,
            "Table" ,
            "Application",
            "Login User",
            "Workstation"   ,
            "TSqlBatch" ,
            "Fix Script",
            "Undo Script",
            "Learn more about"

        };
            foreach (PropertyInfo prop in props)
            {
                if (prop.Name == "IsScriptGeneratorProvider")
                {
                    dataTable.Columns.Add("Fix Script", typeof(string));
                }
                else if (prop.Name == "IsUndoScriptGeneratorProvider")
                {
                    dataTable.Columns.Add("Undo Script", typeof(string));
                }
                else
                {
                    dataTable.Columns.Add(prop.Name, typeof(string));
                }
            }
            dataTable.Columns.Add("Database", typeof(String));
            dataTable.Columns.Add("Table", typeof(String));
            dataTable.Columns.Add("Application", typeof(String));
            dataTable.Columns.Add("Login User", typeof(String));
            dataTable.Columns.Add("Workstation", typeof(String));
            dataTable.Columns.Add("TSqlBatch", typeof(String));
            dataTable.Columns.Add("Learn more about", typeof(string));
            foreach (TSource item in data)
            {
                var values = new object[props.Length + 7];
                for (int i = 0; i < props.Length; i++)
                {
                    if (props[i].Name == "IsScriptGeneratorProvider")
                    {
                        #region column 11
                        if (item is Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IScriptGeneratorProvider)
                        {
                            string tsql = string.Empty;
                            try
                            {

                                Idera.SQLdm.Common.Services.IManagementService managementService =
                           Idera.SQLdm.DesktopClient.Helpers.ManagementServiceHelper.GetDefaultService(
                               Idera.SQLdm.DesktopClient.Properties.Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                                tsql = managementService.GetPrescriptiveOptimizeScript(instanceId, (IRecommendation)item);
                            }
                            catch (Exception ex)
                            {
                                for (Exception excp = ex; excp != null; excp = excp.InnerException)
                                {
                                    tsql = excp.Message;
                                }
                            }
                            finally
                            {
                                if (!String.IsNullOrEmpty(tsql))
                                {
                                    if (tsql.Length > 32767)
                                    {
                                        tsql = tsql.ToString().Substring(0, 32700).TrimEnd() + "..." + Environment.NewLine + "  Fix script exceeds Excel cell limit.";
                                    }
                                }
                                values[i] = tsql;
                            }
                        }
                        else
                            values[i] = string.Empty;
                    }
                    #endregion

                    #region column 12
                    else if (props[i].Name == "IsUndoScriptGeneratorProvider")
                    {
                        try
                        {
                            if (item is Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IUndoScriptGeneratorProvider)
                            {
                                string tsql = string.Empty;
                                try
                                {

                                    Idera.SQLdm.Common.Services.IManagementService managementService =
                               Idera.SQLdm.DesktopClient.Helpers.ManagementServiceHelper.GetDefaultService(
                                   Idera.SQLdm.DesktopClient.Properties.Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                                    tsql = managementService.GetPrescriptiveUndoScript(instanceId, (IRecommendation)item);

                                }
                                catch (Exception ex)
                                {
                                    for (Exception excp = ex; excp != null; excp = excp.InnerException)
                                    {
                                        tsql = excp.Message;
                                    }
                                }
                                finally
                                {
                                    if (!String.IsNullOrEmpty(tsql))
                                    {
                                        if (tsql.Length > 32767)
                                        {
                                            tsql = tsql.Substring(0, 32700).TrimEnd() + "..." + Environment.NewLine + "  Fix script exceeds Excel cell limit.";
                                        }
                                    }
                                    values[i] = tsql;
                                }
                            }
                            else
                                values[i] = string.Empty;
                        }
                        catch (Exception)
                        {
                            // eat any exceptions thrown from script generator
                        }
                    }
                    #endregion

                    else
                        values[i] = Convert.ToString(props[i].GetValue(item, null));
                }
                int nextColCount = props.Length;
                if (item is IProvideDatabase)
                {
                    string Database = ((IProvideDatabase)item).Database;
                    values[nextColCount] = Database;
                }
                else
                    values[nextColCount] = "";
                if (item is IProvideTableName)
                {
                    string Table = ((IProvideTableName)item).Table;
                    values[++nextColCount] = Table;
                }
                else
                    values[++nextColCount] = "";
                if (item is IProvideApplicationName)
                {
                    string Application = ((IProvideApplicationName)item).ApplicationName;
                    values[++nextColCount] = Application;
                }
                else
                    values[++nextColCount] = "";
                if (item is IProvideUserName)
                {
                    string LoginUser = ((IProvideUserName)item).UserName;
                    values[++nextColCount] = LoginUser;
                }
                else
                    values[++nextColCount] = "";

                if (item is IProvideHostName)
                {
                    string host = ((IProvideHostName)item).HostName;
                    values[++nextColCount] = host;
                }
                else
                    values[++nextColCount] = "";

                if (item is TSqlRecommendation)
                {
                    TSqlRecommendation tsqlr = item as TSqlRecommendation;
                    if (null != tsqlr)
                        if (null != tsqlr.Sql)
                            if (null != tsqlr.Sql.Script)
                            {
                                string tsql = tsqlr.Sql.Script;
                                if (!String.IsNullOrEmpty(tsql))
                                {
                                    if (tsql.Length > 32767)
                                        tsql = tsql.Substring(0, 32700).TrimEnd() + "..." + Environment.NewLine + "  TSQL exceeds Excel cell limit.";
                                    values[++nextColCount] = tsql;
                                }
                            }
                }
                else
                    values[++nextColCount] = "";


                RecommendationLinks links = ((IRecommendation)item).Links;
                if (links != null && links.Count > 0)
                {
                    ++nextColCount;
                    values[nextColCount] = "";
                    foreach (var link in links)
                    {
                        if (!string.IsNullOrEmpty(link.Link))
                        {
                            //if (sb.Length == mark) sb.AppendLine(" ").AppendLine("Learn more about: ");
                            values[nextColCount] = values[nextColCount] + "\t " + link.Link;
                        }
                    }
                }
                else
                    values[++nextColCount] = "";

                dataTable.Rows.Add(values);
            }
            //Change sequence of AffectedBatchescolumn
            SetColumnsOrder(dataTable, columnsNameInRequiredSequence);

            return dataTable;
        }

        /// <summary>
        /// SetOrdinal of DataTable columns based on the index of the columnNames array. Removes invalid column names first.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnNames"></param>
        /// <remarks> 
        public void SetColumnsOrder(DataTable dtbl, params String[] columnNames)
        {
            List<string> listColNames = columnNames.ToList();

            //Remove invalid column names.
            foreach (string colName in columnNames)
            {
                if (!dtbl.Columns.Contains(colName))
                {
                    listColNames.Remove(colName);
                }
            }

            foreach (string colName in listColNames)
            {
                dtbl.Columns[colName].SetOrdinal(listColNames.IndexOf(colName));
            }
            for (int i = dtbl.Columns.Count - 1; i >= listColNames.Count; i--)
            {
                dtbl.Columns.RemoveAt(i);
            }
        }
        private void ultraGridExcelExporter_CellExporting(object sender, Infragistics.Win.UltraWinGrid.ExcelExport.CellExportingEventArgs e)
        {
            if (e.GridColumn.Key == "Last Command")
            {
                string value = e.Value.ToString();

                if (value.Length > 32767)
                {
                    e.Value = string.Format("{0}...", value.Substring(0, 32764));
                }
            }
            if (e.CurrentColumnIndex > 1)
                e.CurrentWorksheet.Columns[e.CurrentColumnIndex].Width = 75 * 256;
            else
                e.CurrentWorksheet.Columns[e.CurrentColumnIndex].Width = 20 * 256;
            e.CurrentWorksheet.Columns[e.CurrentColumnIndex].CellFormat.WrapText = ExcelDefaultableBoolean.True;
        }

        /// <summary>
        /// SQLdm 10.0 Srishti Purohit Doctor Analysis implementation in DM To Copy recommendations
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        internal void CopyRecommendations()
        {
            try
            {
                string s = GetSelectedRecommendationText();
                if (s.Length > 0) Clipboard.SetText(s);
                else ApplicationMessageBox.ShowInfo(FindForm(), "No recommendations selected.");
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Unable to copy data.", ex);
                Log.Error(ex.Message + ": Not able to show records. Error while copying data for recommendations.");
                //throw new Exception("Unable to copy data.");
            }
        }
        /// <summary>
        /// SQLdm 10.0 Srishti Purohit Doctor Analysis implementation in DM To Email recommendations
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        public void EmailRecommendations()
        {
            try
            {
                using (Log.InfoCall("EmailRecommendations"))
                {
                    try
                    {

                        string s = GetSelectedRecommendationText();
                        if (s.Length > 0)
                        {
                            MAPI m = new MAPI();
                            Log.Info("Sending email...");
                            m.SendMailPopup(this.FindForm(), "Selected SQL doctor Recommendations", GetSelectedRecommendationText());
                            Log.Info("Complete.");
                        }
                        else ApplicationMessageBox.ShowInfo(FindForm(), "No recommendations selected.");

                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError(this.FindForm(), ex);
                        Log.Error(ex.Message + ": Not able to show records. Error in creating mail for recommendations.");
                        //throw new Exception("Unable to email data.");
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Not able to send mail.", ex);
                Log.Error(ex.Message + ": Not able to show records. Error in creating mail for recommendations.");
                //throw new Exception("Unable to email data.");
            }
        }

        /// <summary>
        /// SQLdm 10.0 Srishti Purohit Doctor Analysis implementation in DM To Undo Script for recommendations
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        public void UndoScriptRecommendations()
        {
            try
            {
                using (Log.InfoCall("UndoScriptRecommendations"))
                {

                    bool undo = false;
                    List<IRecommendation> l = GetSelectedRecommendations();
                    if (null == l) return;
                    if (l.Count <= 0) return;

                    l.RemoveAll(delegate (IRecommendation r) { return (!r.IsUndoScriptGeneratorProvider); });
                    if (l.Count <= 0)
                    {
                        ApplicationMessageBox.ShowInfo(this, "Recommendation does not support Undo script");
                        return;
                    }
                    using (var dlg = new UndoScriptDialog(result, l, true, InstanceId))
                    {
                        undo = (DialogResult.OK == dlg.ShowDialog(this));
                        if (dlg.Recommendations != null && dlg.Recommendations.Count > 0)
                        {
                            IManagementService managementService =
                                    ManagementServiceHelper.GetDefaultService(
                                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                            if (managementService != null)
                            {
                                bool updatedSuccessfully = managementService.UpdateRecommendationOptimizationStatus(dlg.Recommendations);
                                if (updatedSuccessfully)
                                {
                                    updateDataSourceStatus(dlg.Recommendations);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Unable to undo scripts.", ex);
                Log.Error(ex.Message + ": Not able to show records. Error in undo script for recommendations.");
                //throw new Exception("Unable to undo scripts.");
            }
        }

        /// <summary>
        /// SQLdm 10.0 Srishti Purohit Doctor Analysis implementation in DM To Undo Script for recommendations
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        public void OptimizeScriptRecommendations()
        {
            try
            {
                using (Log.InfoCall("UndoScriptRecommendations"))
                {
                    bool optimize = false;
                    List<IRecommendation> l = GetSelectedRecommendations();
                    if (null == l) return;
                    if (l.Count <= 0) return;
                    l.RemoveAll(delegate (IRecommendation r) { return (!r.IsScriptGeneratorProvider); });
                    if (l.Count <= 0)
                    {
                        ApplicationMessageBox.ShowInfo(this, "Recommendation does not support optimize query.");
                        return;
                    }
                    using (var dlg = new OptimizeScriptDialog(result, l, true, InstanceId))
                    {
                        optimize = (DialogResult.OK == dlg.ShowDialog(this));
                        if (dlg.Recommendations != null && dlg.Recommendations.Count > 0)
                        {
                            IManagementService managementService =
                                    ManagementServiceHelper.GetDefaultService(
                                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                            if (managementService != null)
                            {
                                bool updatedSuccessfully = managementService.UpdateRecommendationOptimizationStatus(dlg.Recommendations);
                                if (updatedSuccessfully)
                                {
                                    updateDataSourceStatus(dlg.Recommendations);
                                }
                            }
                        }
                    }
                    //if (optimize) OptimizeOrUndo(l, true);
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Unable to optimize scripts.", ex);
                Log.Error(ex.Message + ": Not able to show records. Error in optimize script for recommendations.");
                //throw new Exception("Unable to optimize scripts.");
            }
        }

        //private void OptimizeOrUndo(List<IRecommendation> l, bool optimize)
        //{
        //    using (var dlg = new OptimizingDialog(CommonSettings.Default.FindServer(null), result, l, !optimize))
        //    {
        //        dlg.ShowDialog(this);
        //    }
        //    RefreshSelectedRows();
        //}

        /// <summary>
        /// SQLdm 10.0 Srishti Purohit Doctor Analysis implementation in DM To show problem for recommendations
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        public void ShowProblemRecommendations()
        {
            try
            {
                using (Log.InfoCall("ShowProblemRecommendations"))
                {
                    List<IRecommendation> selectedRecommendations = GetSelectedRecommendations();
                    if (null == selectedRecommendations) return;
                    if (selectedRecommendations.Count <= 0) return;
                    selectedRecommendations.RemoveAll(delegate (IRecommendation r) { return (!(r is TSqlRecommendation)); });
                    if (selectedRecommendations.Count <= 0)
                    {
                        ApplicationMessageBox.ShowInfo(this, "Recommendation does not support show problem query.");
                        return;
                    }
                    using (var dlg = new SqlViewerDialog(InstanceId))
                    {
                        dlg.Recommendations = selectedRecommendations;
                        dlg.ShowDialog(this);
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Unable to show problem for queries.", ex);
                Log.Error(ex.Message + ": Not able to show problem for queries. Error in show problems for recommendations.");
                //throw new Exception("Unable to optimize scripts.");
            }
        }

        private void updateDataSourceStatus(List<IRecommendation> recommendationsUpdated)
        {
            try
            {
                foreach (IRecommendation recomm in recommendationsUpdated)
                {
                    int listIndex = ListOfRecommendations.FindIndex(item => item.AnalysisRecommendationID == recomm.AnalysisRecommendationID);
                    ListOfRecommendations[listIndex] = recomm;

                    foreach (UltraDataRow row in detailsGridDataSource.Rows)
                    {
                        if (Convert.ToInt32(row["AnalysisRecommendationID"]) == recomm.AnalysisRecommendationID)
                            row["OptimizationStatus"] = getOptimizationStatusImage(recomm.OptimizationStatus);
                    }

                }
                RefreshSelectedRows();
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Unable to update screen after optimize scripts.", ex);
                Log.Error(ex.Message + ": Not able to update screen after optimize scripts for recommendations.");
            }
        }
        //Refresh grid
        private void RefreshSelectedRows()
        {
            if (null == detailsGrid.Selected) return;
            if (null == detailsGrid.Selected.Rows) return;
            if (detailsGrid.Selected.Rows.Count <= 0) return;
            foreach (var r in detailsGrid.Selected.Rows)
            {
                if (null != r) r.Refresh(RefreshRow.ReloadData);
            }
        }

        private string GetSelectedRecommendationText()
        {
            List<IRecommendation> l = GetSelectedRecommendations();
            StringBuilder builder = new StringBuilder();

            if (null != l)
            {
                if (l.Count > 0)
                {
                    l.Sort(new RankComparer());
                    foreach (var r in l) BuildRecommendationString(r, builder);
                }
            }
            return (builder.ToString());
        }
        private List<IRecommendation> GetSelectedRecommendations()
        {
            List<IRecommendation> l = new List<IRecommendation>();
            if (null == detailsGrid.Selected) return (l);
            if (null == detailsGrid.Selected.Rows) return (l);
            if (detailsGrid.Selected.Rows.Count <= 0) return (l);
            //recommendationDetails.Instance = instanceName;
            foreach (var r in detailsGrid.Selected.Rows)
            {
                if (ListOfRecommendations != null && r.Cells["AnalysisRecommendationID"] != null && r.Cells["AnalysisRecommendationID"].Value != DBNull.Value)
                    recommendationDetails.Recommendation = ListOfRecommendations.Find(item => item.AnalysisRecommendationID == Convert.ToInt32(r.Cells["AnalysisRecommendationID"].Value));
                if (recommendationDetails.Recommendation != null)
                    l.Add(recommendationDetails.Recommendation);
            }
            return (l);
        }
        private void BuildRecommendationString(IRecommendation r, StringBuilder builder)
        {
            if (null == r) return;
            string text = GetRecommendationString(r);
            if (builder.Length > 0)
            {
                builder.AppendLine(" ");
                builder.AppendLine("=============================");
                builder.AppendLine(" ");
            }
            builder.AppendLine(text);
        }

        public String GetRecommendationString(IRecommendation r)
        {
            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrEmpty(r.ID))
            {
                sb.AppendLine(String.Format("DEBUG: {0}", r.RecommendationType));
            }
            else
            {
                sb.AppendLine(String.Format("Finding: {0}", r.FindingText));
                sb.AppendLine(String.Format("Recommendation ID: {0}", string.IsNullOrEmpty(r.ID) ? "DEBUG" : r.ID));
                sb.AppendLine(String.Format("Impact: {0}", r.ImpactFactor > 2 ? "HIGH" : r.ImpactFactor > 1 ? "MEDIUM" : "LOW"));
                sb.AppendLine(String.Format("Category: {0}", string.IsNullOrEmpty(r.Category) ? "DEBUG" : r.Category.ToUpper()));
                sb.AppendLine(String.Format("Rank: {0} ({1}:{2}:{3}:{4})", r.RankID, r.ComputedRankFactor, r.ImpactFactor, r.ConfidenceFactor, r.Relevance));

                if (r is IProvideDatabase)
                {
                    string db = ((IProvideDatabase)r).Database;
                    if (!string.IsNullOrEmpty(db)) sb.AppendLine(String.Format("Database: {0}", db));
                }
                if (r is IProvideApplicationName)
                {
                    string app = ((IProvideApplicationName)r).ApplicationName;
                    if (!string.IsNullOrEmpty(app)) sb.AppendLine(String.Format("Application: {0}", app));
                }
                if (r is IProvideUserName)
                {
                    string user = ((IProvideUserName)r).UserName;
                    if (!string.IsNullOrEmpty(user)) sb.AppendLine(String.Format("Login: {0}", user));
                }
                if (r is IProvideHostName)
                {
                    string host = ((IProvideHostName)r).HostName;
                    if (!string.IsNullOrEmpty(host)) sb.AppendLine(String.Format("Workstation: {0}", host));
                }
            }

            sb.AppendLine().AppendLine(r.ImpactExplanationText);
            if (!string.IsNullOrEmpty(r.ProblemExplanationText)) sb.AppendLine().AppendLine("Why is this a problem?").AppendLine(r.ProblemExplanationText);
            if (!string.IsNullOrEmpty(r.AdditionalConsiderations)) sb.AppendLine().AppendLine("When is this not a problem?").AppendLine(r.AdditionalConsiderations);
            sb.AppendLine().AppendLine("Recommendation:").AppendLine(r.RecommendationText);

            if (r.SourceObjects != null)
            {
                int mark = sb.Length;
                foreach (DatabaseObjectName don in r.SourceObjects)
                {
                    if (sb.Length > mark)
                        sb.Append(", ");
                    else
                        sb.AppendLine(" ").Append("Source Objects: ");
                    sb.Append(don.ToString());
                }
                if (sb.Length > mark)
                    sb.AppendLine("");
            }

            RecommendationLinks links = r.Links;
            if (links != null && links.Count > 0)
            {
                int mark = sb.Length;
                foreach (var link in links)
                {
                    if (!string.IsNullOrEmpty(link.Link))
                    {
                        if (sb.Length == mark) sb.AppendLine(" ").AppendLine("Learn more about: ");
                        sb.AppendLine(string.Format("\t{0} ({1})", link.Title, link.Link));
                    }
                }
                if (sb.Length > mark) sb.AppendLine("");
            }

            return (sb.ToString());
        }

        #endregion

        #region helpers

        private static Control GetFocusedControl(ControlCollection controls)
        {
            Control focusedControl = null;

            foreach (Control control in controls)
            {
                if (control.Focused)
                {
                    focusedControl = control;
                }
                else if (control.ContainsFocus)
                {
                    return GetFocusedControl(control.Controls);
                }
            }

            return focusedControl ?? controls[0];
        }

        private void InitializeDataSources()
        {
            try
            {
                chartRealTimeDataTable = new DataTable();
                chartRealTimeDataTable.Columns.Add("Date", typeof(DateTime));

                //detailsGridDataSource = new DataTable();
                detailsGridDataSource.Band.Columns.Add("Flags", typeof(Image));
                detailsGridDataSource.Band.Columns.Add("OptimizationStatus", typeof(Image));
                detailsGridDataSource.Band.Columns.Add("AnalysisRecommendationID", typeof(int));
                detailsGridDataSource.Band.Columns.Add("Recommendation ID", typeof(string));
                detailsGridDataSource.Band.Columns.Add("Finding Text", typeof(string));
                //detailsGridDataSource.Columns.Add("Recommendation Text", typeof(string));
                //detailsGridDataSource.Columns.Add("Impact Explanation Text", typeof(string));

                //detailsGridDataSource.Columns.Add("Additional Considerations", typeof(string));
                detailsGridDataSource.Band.Columns.Add("Priority", typeof(Bitmap));
                detailsGridDataSource.Band.Columns.Add("Priority Value", typeof(float));
                //detailsGridDataSource.Columns.Add("Confidence Factor", typeof(int));
                //detailsGridDataSource.Columns.Add("Problem Explanation Text", typeof(string));

                ShowDetailsGridMethod();
                chartHistoricalDataTable = chartRealTimeDataTable.Clone();

                detailsGrid.SetDataBinding(detailsGridDataSource, string.Empty);
                detailsGrid.DisplayLayout.Bands[0].Columns["AnalysisRecommendationID"].Hidden = true;
                //detailsGrid.DisplayLayout.Bands[0].Columns["Recommendation Text"].Hidden = true;
                //detailsGrid.DisplayLayout.Bands[0].Columns["Impact Explanation Text"].Hidden = true;
                //detailsGrid.DisplayLayout.Bands[0].Columns["Category"].Hidden = true;

                detailsGrid.DisplayLayout.Bands[0].Columns["Recommendation ID"].Hidden = true;
                detailsGrid.DisplayLayout.Bands[0].Columns["Priority Value"].Hidden = true;
                //detailsGrid.DisplayLayout.Bands[0].Columns["Confidence Factor"].Hidden = true;
                detailsGrid.DisplayLayout.Bands[0].Columns["Finding Text"].Width = 500;
                //detailsGrid.DisplayLayout.Bands[0].Columns["Problem Explanation Text"].Hidden = true;
                //detailsGrid.DisplayLayout.Bands[0].Columns["Additional Considerations"].Hidden = true;            

            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Unable to display recommendtionns on screen.", ex);
                Log.Error(ex.Message + ": Not able to show records. Error in intializing data source for recommendations.");
                //throw new Exception("Unable to display recommendtionns on screen.");
            }
        }

        private void GetRealTimeAnalysisRecords()
        {
            try
            {
                IManagementService managementService =
                                    ManagementServiceHelper.GetDefaultService(
                                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                ListOfRecommendations = new List<IRecommendation>();
                if (managementService != null)
                {
                    AnalysisConfiguration config = ApplicationModel.Default.AllInstances[instanceId].AnalysisConfiguration;
                    if (config == null)
                        intitializeAnalysisConfigObjectforServer(instanceId, ref config);
                    if (config.BlockedCategoryID.Count == 18)
                        isCategoriesNotSelected = true;
                    else
                        isCategoriesNotSelected = false;
                    if (!isCategoriesNotSelected)
                    {
                        var result = managementService.GetPrescriptiveAnalysisResult(instanceId, config);
                        if (result != null)
                        {

                            foreach (AnalyzerResult analysisRes in result.AnalyzerRecommendationList)
                            {
                                ListOfRecommendations.AddRange(analysisRes.RecommendationList);
                            }
                        }
                    }
                }
                if (m_oWorker.IsBusy)
                    m_oWorker.ReportProgress(66);
                updateGridDataSource();
                if (isCategoriesNotSelected)
                    MessageBox.Show("You need to select atleast one analysis category from the server properties dialog for this analysis to run.", "No category selected",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //ApplicationMessageBox.ShowError(this, "Unable to display recommendtionns on screen.", ex);
                Log.Error(ex.Message + ": Not able to show records. Error in GetRealTimeAnalysisRecords for run analysis.");
                throw new Exception("Unable to display recommendtionns on screen.", ex);
            }
        }

        /// <summary>
        /// if anlysisconfig object is null for any server provide it a default value anlysisconfig object
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="analysisConfig">to be updated with default value</param>
        private void intitializeAnalysisConfigObjectforServer(int instanceId, ref AnalysisConfiguration analysisConfig)
        {
            try
            {
                Log.Info("Creating default analysiConfig object as for server {0} analysisConfig is null", instanceId);
                analysisConfig = new AnalysisConfiguration(instanceId);

            }
            catch (Exception ex)
            {
                Log.Error("Error while getting default value for analysisConfig object for server {0}. Exception found : {1}. ", instanceId, ex);
            }
        }

        private void GetWorkloadAnalysisRecords()
        {
            try
            {
                IManagementService managementService =
                                    ManagementServiceHelper.GetDefaultService(
                                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                ListOfRecommendations = new List<IRecommendation>();
                if (managementService != null)
                {
                    AnalysisConfiguration config = ApplicationModel.Default.AllInstances[instanceId].AnalysisConfiguration;
                    if (config == null)
                        intitializeAnalysisConfigObjectforServer(instanceId, ref config);
                    ActiveWaitsConfiguration waitConfig = ApplicationModel.Default.AllInstances[instanceId].ActiveWaitsConfiguration;
                    QueryMonitorConfiguration queryConfig = ApplicationModel.Default.AllInstances[instanceId].QueryMonitorConfiguration;
                    if (config.BlockedCategories.Values.Any(v => v.Contains("Index Optimization")) && config.BlockedCategories.Values.Any(v => v.Contains("Query Optimization")))
                    {
                        isCategoriesNotSelected = true;
                    }
                    else
                        isCategoriesNotSelected = false;
                    if (!isCategoriesNotSelected)
                    {
                        var result = managementService.GetWorkLoadAnalysisResult(instanceId, config, waitConfig, queryConfig);
                        if (result != null)
                        {

                            foreach (AnalyzerResult analysisRes in result.AnalyzerRecommendationList)
                            {
                                ListOfRecommendations.AddRange(analysisRes.RecommendationList);
                            }
                        }
                    }
                    //ListOfRecommendations.Add(new Recommendation(RecommendationType.FunctionInWhereClause));
                    //ListOfRecommendations.Add(new Recommendation(RecommendationType.FragmentedIndex));
                }
                if (m_oWorker.IsBusy)
                    m_oWorker.ReportProgress(66);
                updateGridDataSource();
                if (isCategoriesNotSelected)
                    MessageBox.Show("You need to select either 'Index Optimization' or 'Query Optimization' categories from the server properties dialog for this analysis to run.", "No category selected",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //ApplicationMessageBox.ShowError(this, "Unable to display recommendtionns on screen.", ex);
                Log.Error(ex.Message + ": Not able to show records. Error in GetRealTimeAnalysisRecords for run analysis.");
                throw new Exception("Unable to display recommendtionns on screen.", ex);
            }
        }
        private void GetDiagnosedAnalysisRecords(string queryToDiagnose, string database)
        {
            try
            {
                IManagementService managementService =
                                    ManagementServiceHelper.GetDefaultService(
                                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                ListOfRecommendations = new List<IRecommendation>();
                if (managementService != null)
                {
                    AnalysisConfiguration config = ApplicationModel.Default.AllInstances[instanceId].AnalysisConfiguration;
                    var result = managementService.GetAdHocBatchAnalysisResult(instanceId, queryToDiagnose, database, config);
                    if (result != null)
                    {

                        foreach (AnalyzerResult analysisRes in result.AnalyzerRecommendationList)
                        {
                            ListOfRecommendations.AddRange(analysisRes.RecommendationList);
                        }
                    }
                }
                updateGridDataSource();
            }
            catch (Exception ex)
            {
                //ApplicationMessageBox.ShowError(this, "Unable to display recommendtionns on screen.", ex);
                Log.Error(ex.Message + ": Not able to show records. Error in GetRealTimeAnalysisRecords for run analysis.");
                throw new Exception("Unable to display recommendtionns on screen.", ex);
            }
            finally
            {
                SqlTextDialog.DiagnoseQuery = null;
                SqlTextDialog.Database = string.Empty;
            }
        }



        #region grid



        public void ToggleGroupByBox()
        {
            GridGroupByBoxVisible = !GridGroupByBoxVisible;
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    detailsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    detailsGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
                }
            }
        }

        private void RemoveSelectedColumn()
        {
            if (selectedColumn != null)
            {
                selectedColumn.Hidden = true;
            }
        }


        #endregion

        #region chart



        private void UpdateChartDataFilter()
        {
            if (chartRealTimeDataTable != null)
            {
                DateTime viewFilter =
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));

                chartRealTimeDataTable.DefaultView.RowFilter = string.Format("Date > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
                chartRealTimeDataTable.DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
            }
        }

        private void GroomHistoryData()
        {
            if (chartRealTimeDataTable != null)
            {
                DateTime groomThreshold =
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(Settings.Default.RealTimeChartHistoryLimitInMinutes));

                DataRow[] groomedRows = chartRealTimeDataTable.Select(string.Format("Date < #{0}#", groomThreshold.ToString(CultureInfo.InvariantCulture))); // SQLDM-19237, Tolga K

                foreach (DataRow row in groomedRows)
                {
                    row.Delete();
                }
                chartRealTimeDataTable.AcceptChanges();
            }
        }

        #endregion

        #endregion

        #region events

        #region Background Worker
        /// <summary>
        /// Time consuming operations go here </br>
        /// i.e. Database operations for analysis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_oWorker_DoWorkAnlysis(object sender, DoWorkEventArgs e)
        {
            if (isWorkLoadAnalysis)
                GetWorkloadAnalysisRecords();
            else
                GetRealTimeAnalysisRecords();

            ////Updating back ground thread
            //m_oWorker.ReportProgress(99);

            //if (m_oWorker.CancellationPending)
            //{
            //    // Set the e.Cancel flag so that the WorkerCompleted event
            //    // knows that the process was cancelled.
            //    e.Cancel = true;
            //    m_oWorker.ReportProgress(0);
            //    return;
            //}

            ////Report 100% completion on operation completed
            //m_oWorker.ReportProgress(100);
        }
        /// <summary>
        /// Time consuming operations go here for diagnose</br>
        /// i.e. Database operations for analysis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_oWorker_DoWorkDignose(object sender, DoWorkEventArgs e)
        {
            GetDiagnosedAnalysisRecords(this.queryFromDignose, databaseNameDiagnose);

            ////Updating back ground thread
            //m_oWorker.ReportProgress(99);

            //if (m_oWorker.CancellationPending)
            //{
            //    // Set the e.Cancel flag so that the WorkerCompleted event
            //    // knows that the process was cancelled.
            //    e.Cancel = true;
            //    m_oWorker.ReportProgress(0);
            //    return;
            //}

            ////Report 100% completion on operation completed
            //m_oWorker.ReportProgress(100);
        }
        /// <summary>
        /// Time consuming operations go here for histroy load</br>
        /// i.e. Database operations for analysis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_oWorker_DoWorkHistory(object sender, DoWorkEventArgs e)
        {
            if (historicalSnapshotDateTime != null)
                PopulateHistoricalSnapshots();

        }

        //Finishing of back ground workers task
        void m_oWorker_DoWorkCleanUp(object sender, DoWorkEventArgs e)
        {

            //Updating back ground thread
            m_oWorker.ReportProgress(99);

            if (m_oWorker.CancellationPending)
            {
                // Set the e.Cancel flag so that the WorkerCompleted event
                // knows that the process was cancelled.
                e.Cancel = true;
                m_oWorker.ReportProgress(0);
                return;
            }

            //Report 100% completion on operation completed
            m_oWorker.ReportProgress(100);
        }

        /// <summary>
        /// Notification is performed here to the lable message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_oWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            detailsGridProcessStatus.Text = Idera.SQLdm.Common.Constants.LOADING;
            if (e.ProgressPercentage > 99)
            {
                Settings.Default.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Settings_PropertyChanged);

                ShowDetailsGridMethod();
                // Autoscale font size.
                AdaptFontSize();
            }
        }
        /// <summary>
        /// On completed do the appropriate task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_oWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Cancelled)
                {
                    detailsGridProcessStatus.Text = "Analysis Cancelled.";
                }

                // Check to see if an error occurred in the background process.

                else if (e.Error != null)
                {
                    if (currentConstructorType == ConstructorType.history)
                        detailsGridProcessStatus.Text = "Error while fetching history. ";
                    else
                        detailsGridProcessStatus.Text = "Error while performing Analysis. Please check if Prescriptive Analysis service is not running or restart the service.";
                }
                else
                {
                    if (currentConstructorType == ConstructorType.history)
                        detailsGridMessageLabel.Text = "Please select time from history browser to see previous recommendations.";
                    else
                        if (isCategoriesNotSelected)
                        detailsGridMessageLabel.Text = "Please click Run Analysis to analyze your server.";
                    else
                        detailsGridMessageLabel.Text = "No recommendations were generated during this analysis. This can be due to category or threshold options selected when the analysis was configured. Select more categories of different filters in the analysis configuration and re-run the analysis.";

                    detailsGridMessageLabel.Visible = true;
                    // Everything completed normally.
                    detailsGridProcessStatus.Text = string.Empty;

                }
            }
            finally
            {
                IsAnalysisDone = true;
            }
        }

        #endregion

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RealTimeChartHistoryLimitInMinutes":
                    GroomHistoryData();
                    break;
                case "RealTimeChartVisibleLimitInMinutes":
                    UpdateChartDataFilter();
                    break;
            }
        }

        private void splitContainer_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void splitContainer_MouseUp(object sender, MouseEventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
        }
        private void historicalSnapshotStatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (historyModeLoadError == null)
            {
                //If active view is analysis view user can directly go to Real time view by clicking on Run Analysis or Run workoad. 
                //so switch not needed
                //10.0 SQLdm Srishti Purohit
                //SwitchToRealTimeMode();
            }
            else
            {
                ApplicationMessageBox.ShowError(ParentForm,
                                                "An error occurred while loading a historical snapshot.",
                                                historyModeLoadError, false);
            }
        }

        #endregion

        #region grid


        private void detailsGrid_AfterRowActivate(object sender, EventArgs e)
        {
            //UltraGridRow row = this.detailsGrid.ActiveRow;
            //if (ListOfRecommendations != null)
            //{
            //    IRecommendation selectedRecommendation = ListOfRecommendations.Find(item => item.AnalysisRecommendationID == Convert.ToInt32(row.Cells["AnalysisRecommendationID"].Value));

            //    recommendationDetails.Recommendation = selectedRecommendation;
            //}
        }

        private void detailsGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            List<IRecommendation> selectedRecommendations = GetSelectedRecommendations();
            if (null != selectedRecommendations)
            {
                SetToolbarState(selectedRecommendations);
                return;
            }
            SetToolbarState(null);
        }

        //Check if Optimization or Undo Supported
        private void SetToolbarState(List<IRecommendation> selectedRecommendations)
        {
            try
            {
                if (null != selectedRecommendations && selectedRecommendations.Count > 0)
                {
                    IRecommendation ir = selectedRecommendations[0];
                    if (null == ir)
                    {
                        return;
                    }
                    //if (ir is TSqlRecommendation)
                    //{
                    //    TSqlRecommendation tsql = ir as TSqlRecommendation;
                    //    // If we have a valid sql script enable the show problem toolbar button
                    //    IsShowProblemSupported = tsql.Sql != null && !string.IsNullOrEmpty(tsql.Sql.Script);
                    //}
                    TSqlRecommendation tsqlRecomm;
                    if (ir.IsScriptGeneratorProvider && ir.IsUndoScriptGeneratorProvider && ir is TSqlRecommendation)
                    {
                        IsOptimizable = ir.IsScriptGeneratorProvider;
                        IsUndoSupported = ir.IsUndoScriptGeneratorProvider;

                        //Moving show problem tab enable logic here to support multiple selection
                        tsqlRecomm = ir as TSqlRecommendation;
                        if (tsqlRecomm != null)
                        {
                            // If we have a valid sql script enable the show problem toolbar button
                            IsShowProblemSupported = tsqlRecomm.Sql != null && !string.IsNullOrEmpty(tsqlRecomm.Sql.Script);
                        }
                    }
                    else
                    {
                        bool script = false;
                        bool undo = false;
                        bool tsql = false;
                        foreach (IRecommendation r in selectedRecommendations)
                        {
                            if (!script) script = r.IsScriptGeneratorProvider;
                            if (!undo) undo = r.IsUndoScriptGeneratorProvider;
                            if (!tsql)
                            {
                                tsqlRecomm = r as TSqlRecommendation;

                                if (tsqlRecomm != null)
                                {
                                    // If we have a valid sql script enable the show problem toolbar button
                                    tsql = tsqlRecomm.Sql != null && !string.IsNullOrEmpty(tsqlRecomm.Sql.Script);
                                }
                            }
                            if (script && undo && tsql) break;
                        }
                        IsOptimizable = script;
                        IsUndoSupported = undo;
                        IsShowProblemSupported = tsql;
                    }
                    //recommendationDetails.Recommendation = selectedRecommendations[selectedRecommendations.Count - 1];
                }
                Log.Info("Successful updated scripts tool bar.");
            }
            catch (Exception ex)
            {
                Log.Error("exception while handling support of scripts for recommendations. " + ex.Message);
                throw ex;
            }
        }

        private void detailsGrid_MouseClick(object sender, MouseEventArgs e)
        {
            UIElement selectedElement = detailsGrid.DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
            if (selectedElement == null) return;
            UltraGridRow selectedRow = selectedElement.SelectableItem is UltraGridCell ? ((UltraGridCell)selectedElement.SelectableItem).Row : selectedElement.SelectableItem as UltraGridRow;
            if (selectedRow != null)
            {
                IRecommendation selectedRecommendation = ListOfRecommendations.Find(item => item.AnalysisRecommendationID == Convert.ToInt32(selectedRow.Cells["AnalysisRecommendationID"].Value));
                if (e.X > 25)
                {
                    if (!(selectedElement is ImageUIElement))
                    {
                        selectedElement = selectedElement.Parent;
                        if (!(selectedElement is ImageUIElement || selectedElement is HeaderUIElement))
                        {
                            ////Check if Optimization or Undo Supported

                            //if (selectedRecommendation.IsScriptGeneratorProvider && selectedRecommendation.IsUndoScriptGeneratorProvider)
                            //{
                            //    IsOptimizable = selectedRecommendation.IsScriptGeneratorProvider;
                            //    IsUndoSupported = selectedRecommendation.IsUndoScriptGeneratorProvider;
                            //}
                            //else
                            //{
                            //    IsOptimizable = false;
                            //    IsUndoSupported = false;
                            //}
                            return;
                        }
                    }
                    else if (selectedElement is ImageUIElement && e.X < 50)
                    {
                        if (!string.IsNullOrEmpty(selectedRecommendation.OptimizationErrorMessage))
                        {
                            ApplicationMessageBox.ShowError(this, "Error occurred when optimizing/ undoing the recommendation.\n" + selectedRecommendation.OptimizationErrorMessage);
                            Log.InfoFormat("Previous exception checked.");
                        }
                        return;
                    }
                    return;
                }


                if (selectedRecommendation != null)
                {
                    selectedRecommendation.IsFlagged = !selectedRecommendation.IsFlagged;
                    selectedRow.Cells[0].Value = getFlagState(selectedRecommendation.IsFlagged);
                    bool changedSuccessfully = RepositoryHelper.ChangeFlagOfRecommendation(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                      selectedRecommendation.AnalysisRecommendationID, selectedRecommendation.IsFlagged);

                }

            }

        }


        private void detailsGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridColumn col;
            UltraGridBand band;


            band = detailsGrid.DisplayLayout.Bands[0];
            col = band.Columns["Flags"];
            col.Header.Caption = string.Empty;
            col.Header.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.FlagEnabled16;
            col.Header.Appearance.ImageHAlign = HAlign.Center;
            col.SortComparer = new srtImgComparer();
            col.Header.Fixed = true;
            col.Width = 20;

            //For status images
            col = band.Columns["OptimizationStatus"];
            col.Header.Caption = string.Empty;
            //col.Header.Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Error16;
            col.Header.Appearance.ImageHAlign = HAlign.Center;
            col.SortComparer = new srtImgComparer();
            col.Header.Fixed = true;
            col.Width = 20;


            //col = band.Columns["Impact Explanation Text"];


            col = band.Columns["AnalysisRecommendationID"];
            col.Header.Fixed = true;
            col.Width = 95;
            band.SortedColumns.Add(col, false);

            col = band.Columns["Recommendation ID"];
            col.Header.Fixed = true;
            col.Width = 195;
            band.SortedColumns.Add(col, false);

            //col = band.Columns["Recommendation Text"];
            //col.Width = 105;

            col = band.Columns["Finding Text"];
            col.Header.Fixed = true;
            col.PerformAutoResize();
            band.SortedColumns.Add(col, false);

            //col = band.Columns["Additional Considerations"];
            col = band.Columns["Priority"];
            col.Header.Fixed = true;
            col.Width = 195;
            col.SortComparer = new srtComparer();
            band.SortedColumns.Clear();
            band.SortedColumns.Add("Priority", true, false);

            col = band.Columns["Priority Value"];
            //col = band.Columns["Category"];

            // all rows shown by default
            band.ColumnFilters[col].ClearFilterConditions();
            //this.detailsGrid.DisplayLayout.Bands[0].Columns["Finding Text"].PerformAutoResize();
            // never show row filter crap in the column headers
            band.Override.AllowRowFiltering = DefaultableBoolean.False;
        }

        private void detailsGrid_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Index == 0)
            {
                e.Row.Selected = true;
            }

        }



        private void RecommendationsView_Load(object sender, EventArgs e)
        {
            ApplySettings();
        }

        #endregion

        #region UI Status Change

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        private void SwitchToRealTimeMode()
        {
            //Turn Off Visibility
            RunAnalysisView_Fill_Panel.Visible = true;
            GetRealTimeAnalysisRecords();
            Settings.Default.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Settings_PropertyChanged);

            ShowDetailsGridMethod();
            //detailsGrid.Visible = detailsGrid.Rows.FilteredInRowCount > 0;
            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }
        #endregion

        #region Classes
        private class RankComparer : IComparer<IRecommendation>
        {
            public int Compare(IRecommendation x, IRecommendation y)
            {
                return x.RankID.CompareTo(y.RankID);
            }
        }
        public class srtComparer : IComparer
        {
            public srtComparer()
            {
            }

            public int Compare(object x, object y)
            {
                try
                {
                    UltraGridCell xCell = (UltraGridCell)x;
                    UltraGridCell yCell = (UltraGridCell)y;

                    return (Convert.ToSingle(xCell.Row.Cells["Priority Value"].Value)).CompareTo(Convert.ToSingle(yCell.Row.Cells["Priority Value"].Value));
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }
        public class srtImgComparer : IComparer
        {
            public srtImgComparer()
            {
            }

            public int Compare(object x, object y)
            {
                try
                {
                    UltraGridCell xCell = (UltraGridCell)x;
                    UltraGridCell yCell = (UltraGridCell)y;
                    byte[] image1Bytes;
                    byte[] image2Bytes;
                    if (xCell.Column.Index == 0)
                    {
                        using (var mstream = new MemoryStream())
                        {
                            ((Bitmap)xCell.Row.Cells["Flags"].Value).Save(mstream, ((Bitmap)xCell.Row.Cells["Flags"].Value).RawFormat);
                            image1Bytes = mstream.ToArray();
                        }

                        using (var mstream = new MemoryStream())
                        {
                            ((Bitmap)yCell.Row.Cells["Flags"].Value).Save(mstream, ((Bitmap)yCell.Row.Cells["Flags"].Value).RawFormat);
                            image2Bytes = mstream.ToArray();
                        }
                        var image164 = Convert.ToBase64String(image1Bytes);
                        var image264 = Convert.ToBase64String(image2Bytes);
                        return image164.CompareTo(image264);
                    }
                    if (xCell.Column.Index == 1)
                    {
                        using (var mstream = new MemoryStream())
                        {
                            if (xCell.Row.Cells["OptimizationStatus"].Value is Bitmap)
                            {
                                ((Bitmap)xCell.Row.Cells["OptimizationStatus"].Value).Save(mstream, ((Bitmap)xCell.Row.Cells["OptimizationStatus"].Value).RawFormat);
                            }
                            image1Bytes = mstream.ToArray();
                        }

                        using (var mstream = new MemoryStream())
                        {
                            if (yCell.Row.Cells["OptimizationStatus"].Value is Bitmap)
                            {
                                ((Bitmap)yCell.Row.Cells["OptimizationStatus"].Value).Save(mstream, ((Bitmap)yCell.Row.Cells["OptimizationStatus"].Value).RawFormat);
                            }
                            image2Bytes = mstream.ToArray();
                        }
                        var image164 = Convert.ToBase64String(image1Bytes);
                        var image264 = Convert.ToBase64String(image2Bytes);
                        return image164.CompareTo(image264);
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }

        #endregion

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.detailsGrid);
        }
    }

    //Constructor Type
    public enum ConstructorType
    {
        analysis = 0,
        diagnose = 1,
        history = 2
    }

}
