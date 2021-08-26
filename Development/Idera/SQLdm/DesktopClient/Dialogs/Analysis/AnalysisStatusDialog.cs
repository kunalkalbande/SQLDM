using Idera.SQLdm.Common.Objects;
using System.Collections.Generic;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Analysis;
using Idera.SQLdm.Common.Configuration;
using System;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;
using System.ComponentModel;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Resources;
using Idera.Newsfeed.Plugins.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Views;
using System.Diagnostics;

namespace Idera.SQLdm.DesktopClient.Dialogs.Analysis
{
    public partial class AnalysisStatusDialog : Form
    {
        private static int count = 0;
        private MonitoredSqlServer serverInstance;
        private List<IRecommendation> ListOfRecommendations;
        private List<int> setOfAllCategories;
        string queryFromDignoses = string.Empty;
        string databaseNameDiagnoses = string.Empty;
        ActiveWaitsConfiguration activeWaitConfig;
        QueryMonitorConfiguration queryMonitorConfig;
        bool isWorkloadAnalysis = false;
        Action formDisposer = null;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        public int AnalysisDuration { get; set; }
        bool isUserSysAdmin = false;

        //To log errors
        Logger Log = Logger.GetLogger("AnalysisStatusDialog");

        public event EventHandler ScriptAnalysisActionAllowedChanged;

        private DateTime? currentHistoricalSnapshotDateTime = null;
        /// <summary>
        /// Sets HistoricalSnapshotDateTime, ViewMode, LoadingText.
        /// </summary>
        private void SetHistoricalSnapshotDateTime(DateTime? value, bool refreshView = true)
        {
            currentHistoricalSnapshotDateTime = value;
        }

        /// <summary>
        /// The date time on which has got the snapshoot.
        /// </summary>
        public DateTime? HistoricalSnapshotDateTime
        {
            get
            {
                return currentHistoricalSnapshotDateTime;
            }
            set { SetHistoricalSnapshotDateTime(value); }
        }
        ViewContainer viewHost;
        BackgroundWorker m_oWorker;
        ConstructorType currentConstructorType = ConstructorType.analysis;
        Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result result = null;
        private bool isCategoriesNotSelected = false;
        bool isAnalysisDone = true;
        AnalysisConfiguration config;
        bool isAwsInstance = false;
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

        public AnalysisStatusDialog()
        {
            InitializeComponent();
        }

        public AnalysisStatusDialog(MonitoredSqlServer mserverInstance, List<int> BlockedCategoryID, ActiveWaitsConfiguration waitConfig,
                                    QueryMonitorConfiguration queryConfig, string queryFromDignose,
                                    string databaseNameDiagnose, bool isWorkloadAnalys, ViewContainer vHost, Action disposer, int analysisDuration,Dictionary<int,string> blockedCategories)
        {
         
            serverInstance = mserverInstance;
            formDisposer = disposer;
            viewHost = vHost;
            isUserSysAdmin = ApplicationModel.Default.AllInstances[serverInstance.Id].IsUserSysAdmin;
            try
            {
                isAwsInstance = ApplicationModel.Default.AllInstances[serverInstance.Id].CloudProviderId == Common.Constants.AmazonRDSId;
            }
            catch
            {

            }
            config = ApplicationModel.Default.AllInstances[serverInstance.Id].AnalysisConfiguration;
            if (config == null)
                intitializeAnalysisConfigObjectforServer(serverInstance.Id, ref config);
            config.BlockedCategoryID = BlockedCategoryID;
            queryFromDignoses = queryFromDignose;
            databaseNameDiagnoses = databaseNameDiagnose;
            activeWaitConfig = waitConfig;
            queryMonitorConfig = queryConfig;
            isWorkloadAnalysis = isWorkloadAnalys;
            setOfAllCategories = new List<int>();
            IsAnalysisDone = false;
            InitializeComponent();
            label3.Text = "Idera SQL DM is analyzing " + serverInstance.InstanceName + " - " + databaseNameDiagnoses + "...";
            GetMasterRecommendations();
            count = 0;
            AnalysisDuration = analysisDuration;
            config.BlockedCategories = blockedCategories;
            callBackgroundWorker(ConstructorType.analysis);

        }
        private void CancelAnalysis()
        {
            count = 0;
            this.Close();
            if (m_oWorker != null)
            {
                m_oWorker.CancelAsync();
                m_oWorker.Dispose();
            }
            if (formDisposer != null)
            {
                formDisposer();
            }
        }

        private void btnCancelAnalysis_Click(object sender, System.EventArgs e)
        {
            CancelAnalysis();
        }

        private void btnHide_Click(object sender, System.EventArgs e)
        {
            this.Hide();
        }

        private void callBackgroundWorker(ConstructorType owner)
        {
            currentConstructorType = owner;
            _startTimeLabel.Text = DateTime.Now.ToShortTimeString();
            startTimeTimer.Start();
            _stopwatch.Restart();

            m_oWorker = new BackgroundWorker();
            m_oWorker.WorkerReportsProgress = true;
            m_oWorker.WorkerSupportsCancellation = true;

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
            //else if (owner == ConstructorType.history)
            //{
            //    m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWorkHistory);
            //}
            m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWorkCleanUp);
            m_oWorker.ProgressChanged += new ProgressChangedEventHandler(m_oWorker_ProgressChanged);
            m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_oWorker_RunWorkerCompleted);
            m_oWorker.WorkerReportsProgress = true;
            m_oWorker.WorkerSupportsCancellation = true;
            isAnalysisDone = false;
            // Kickoff the worker thread to begin it's DoWork function.
            m_oWorker.RunWorkerAsync();
            if (m_oWorker.IsBusy)
                m_oWorker.ReportProgress(count + 10);
            //detailsGridMessageLabel.Visible = false;            
        }

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
                    if (config.BlockedCategoryID.Count == 18)
                        isCategoriesNotSelected = true;
                    else
                        isCategoriesNotSelected = false;

                    if (m_oWorker.IsBusy)
                        m_oWorker.ReportProgress(count += 10);
                    if (!isCategoriesNotSelected)
                    {
                        // Perform Prescriptive Analysis
                        result = managementService.GetPrescriptiveAnalysisResult(serverInstance.Id, config);
                        AddResultToRecommendationList();

                        if (m_oWorker.IsBusy)
                            m_oWorker.ReportProgress(count += 10);

                        // WorkLoad Analysis
                        if (isWorkloadAnalysis)
                        {
                            result = managementService.GetWorkLoadAnalysisResult(serverInstance.Id, config, activeWaitConfig, queryMonitorConfig);
                            AddResultToRecommendationList();
                            if (m_oWorker.IsBusy)
                                m_oWorker.ReportProgress(count += 10);
                        }
                        if (queryFromDignoses != null && queryFromDignoses != string.Empty)
                        {
                            result = managementService.GetAdHocBatchAnalysisResult(serverInstance.Id, queryFromDignoses, databaseNameDiagnoses, config);
                            AddResultToRecommendationList();
                            if (m_oWorker.IsBusy)
                                m_oWorker.ReportProgress(count += 10);
                        }
                    }
                    // Start: If no recommendation id found make entry with ZERO count
                    if (!isUserSysAdmin && ListOfRecommendations != null && ListOfRecommendations.Count == 0)
                    {
                        RepositoryHelper.AddRecommendationForNonSysAdmin(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, serverInstance.Id);
                    }
                    // End
                }

                if (isCategoriesNotSelected)
                {
                    _stopwatch.Stop();
                    MessageBox.Show("You need to select atleast one analysis category from the server properties dialog for this analysis to run.", "No category selected", 
                                                MessageBoxButtons.OK, 
                                                MessageBoxIcon.Information,
                                                MessageBoxDefaultButton.Button1,
                                                MessageBoxOptions.ServiceNotification);
                    return;
                }

                if (m_oWorker.IsBusy)
                    m_oWorker.ReportProgress(count += 10);

                m_oWorker.ReportProgress(95);
                System.Threading.Thread.Sleep(10000);
                //updateGridDataSource();

                
                m_oWorker.ReportProgress(98);
                System.Threading.Thread.Sleep(10000);
            }
            catch (Exception ex)
            {
                //ApplicationMessageBox.ShowError(this, "Unable to display recommendtionns on screen.", ex);
                Log.Error(ex.Message + ": Not able to show records. Error in GetRealTimeAnalysisRecords for run analysis.");
                throw new Exception("Unable to display recommendtionns on screen.", ex);
            }
            finally
            {
                _stopwatch.Stop();
                m_oWorker.ReportProgress(100);
                count = 0;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_stopwatch.Elapsed.Minutes >= AnalysisDuration)
            {
                CancelAnalysis();
            }
            else
            {
                _timeElapsedLabel.Text = _stopwatch.Elapsed.ToString(@"mm\:ss");
            }
        }

        /// <summary>
        /// Add Result to Recommendation List
        /// </summary>
        private void AddResultToRecommendationList()
        {
            
            if (isUserSysAdmin || isAwsInstance)
            {
                if (result != null)
                {
                    foreach (AnalyzerResult analysisRes in result.AnalyzerRecommendationList)
                    {
                        ListOfRecommendations.AddRange(analysisRes.RecommendationList);
                    }
                    if (m_oWorker.IsBusy)
                        m_oWorker.ReportProgress(count += 10);
                }
            }
            else
            {
                ApplicationMessageBox.ShowMessage( "Unable to display recommendations as the user does not have sysadmin permissions.");
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
                if (analysisConfig == null)
                {
                    Log.Info("Creating default analysiConfig object as for server {0} analysisConfig is null", instanceId);
                    analysisConfig = new AnalysisConfiguration(instanceId);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while getting default value for analysisConfig object for server {0}. Exception found : {1}. ", instanceId, ex);
            }
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RealTimeChartHistoryLimitInMinutes":
                    //GroomHistoryData();
                    break;
                case "RealTimeChartVisibleLimitInMinutes":
                    //UpdateChartDataFilter();
                    break;
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
                    AnalysisConfiguration config = ApplicationModel.Default.AllInstances[serverInstance.Id].AnalysisConfiguration;
                    var result = managementService.GetAdHocBatchAnalysisResult(serverInstance.Id, queryToDiagnose, database, config);
                    if (result != null)
                    {

                        foreach (AnalyzerResult analysisRes in result.AnalyzerRecommendationList)
                        {
                            ListOfRecommendations.AddRange(analysisRes.RecommendationList);
                        }
                    }
                }
                //updateGridDataSource();
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

        #region Background Worker
        /// <summary>
        /// Time consuming operations go here </br>
        /// i.e. Database operations for analysis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void m_oWorker_DoWorkAnlysis(object sender, DoWorkEventArgs e)
        {
            //if (isWorkLoadAnalysis)
            //    GetWorkloadAnalysisRecords();
            //else

            GetRealTimeAnalysisRecords();

            ////Updating back ground thread
            //m_oWorker.ReportProgress(count + 10);

            //if (m_oWorker.CancellationPending)
            //{
            //    // Set the e.Cancel flag so that the WorkerCompleted event
            //    // knows that the process was cancelled.
            //    e.Cancel = true;
            //    m_oWorker.ReportProgress(count);
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
            GetDiagnosedAnalysisRecords(this.queryFromDignoses, this.databaseNameDiagnoses);

            ////Updating back ground thread
            //m_oWorker.ReportProgress(count + 10);

            //if (m_oWorker.CancellationPending)
            //{
            //    // Set the e.Cancel flag so that the WorkerCompleted event
            //    // knows that the process was cancelled.
            //    e.Cancel = true;
            //    m_oWorker.ReportProgress(count);
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
        //void m_oWorker_DoWorkHistory(object sender, DoWorkEventArgs e)
        //{
        //    if (historicalSnapshotDateTime != null)
        //        PopulateHistoricalSnapshots();

        //}

        //Finishing of back ground workers task
        void m_oWorker_DoWorkCleanUp(object sender, DoWorkEventArgs e)
        {

            //Updating back ground thread
            m_oWorker.ReportProgress(count + 10);

            if (m_oWorker.CancellationPending)
            {
                // Set the e.Cancel flag so that the WorkerCompleted event
                // knows that the process was cancelled.
                e.Cancel = true;
                m_oWorker.ReportProgress(count);
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
            progressBar1.Value = e.ProgressPercentage;
            //detailsGridProcessStatus.Text = Idera.SQLdm.Common.Constants.LOADING;
            if (e.ProgressPercentage > 90)
            {
                Settings.Default.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Settings_PropertyChanged);

                //ShowDetailsGridMethod();
                // Autoscale font size.
                //AdaptFontSize();
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
                startTimeTimer.Stop();

                if (e.Cancelled)
                {
                    //detailsGridProcessStatus.Text = "Analysis Cancelled.";
                }

                // Check to see if an error occurred in the background process.

                //else if (e.Error != null)
                // {
                //    if (currentConstructorType == ConstructorType.history)
                //detailsGridProcessStatus.Text = "Error while fetching history. ";
                //   else
                //detailsGridProcessStatus.Text = "Error while performing Analysis. Please check if Prescriptive Analysis service is not running or restart the service.";
                //}
                //else
                //{
                //if (currentConstructorType == ConstructorType.history)
                //detailsGridMessageLabel.Text = "Please select time from history browser to see previous recommendations.";
                // else
                //  if (isCategoriesNotSelected)
                //detailsGridMessageLabel.Text = "Please click Run Analysis to analyze your server.";
                //else
                //detailsGridMessageLabel.Text = "No recommendations were generated during this analysis. This can be due to category or threshold options selected when the analysis was configured. Select more categories of different filters in the analysis configuration and re-run the analysis.";

                //detailsGridMessageLabel.Visible = true;
                // Everything completed normally.
                //detailsGridProcessStatus.Text = string.Empty;

                //}
            }
            finally
            {
                IsAnalysisDone = true;
                this.Hide();
                DefaultScreenAnalysisTab defaultAnalysisView = new DefaultScreenAnalysisTab(serverInstance.Id, viewHost, false);
                defaultAnalysisView.display();
                if (formDisposer != null)
                {
                    formDisposer();
                }
            }
        }
        #endregion
    }
}