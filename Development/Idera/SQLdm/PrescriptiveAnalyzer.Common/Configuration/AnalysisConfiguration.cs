using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Values;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.AdHoc;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration
{
    [Serializable]
    public enum AnalysisStatus
    {
        [Description("Waiting to start analysis")]
        AnalysisWaiting,
        [Description("Analysis completed")]
        AnalysisCompleted,
        [Description("Analysis processing cancelled")]
        AnalysisCancelled,
        [Description("Analysis aborted early due to error")]
        AnalysisEarlyAbort,
        
        [Description("Step processing started")]
        StepStarted,
        [Description("Step processing progress")]
        StepProgress,
        [Description("Step processing finished")]
        StepFinished
    }

    public delegate void AnalysisStatusHandler(Guid requestId, AnalysisStatus status, object data);

    public class AnalysisConfiguration : MarshalByRefObject
    {
        public static int nextTraceId = 1;

        /// <summary>
        /// When status is started data is a string description otherwise it is an int containing percent complete.
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="status"></param>
        /// <param name="data"></param>
        public event AnalysisStatusHandler AnalysisStatusChanged;
        
        public readonly Guid RequestId;
        private readonly SqlConnectionInfo connectionInfo;

        public bool RunBestPracticesOnWorkload;
        public bool RunIndexAnalysisOnWorkload;
        public bool RunDatabaseObjectAnalyzer = true;

        /// <summary>
        /// The 'EnableOleAutomation' flag indicates that Ole Automation will
        /// be enabled for the duration of the analysis.  Once the analysis
        /// is complete, Ole Automation will be disabled.
        /// </summary>
        public bool EnableOleAutomation = false;

        /// <summary>
        /// When this flag is 'True', the recommendation engine will generate certain
        /// recommendations that would normally not be possible to generate due to configuration
        /// and various error conditions.
        /// </summary>
        public bool GenerateTestRecommendations = false;

        public bool ProductionServer = false;
        public bool OLTPServer = false;

        public AnalysisValues Values = new AnalysisValues();

        public int TraceId;
        public int TraceTotalDurationMinutes = 5;
        public int SnapshotTotalDurationMinutes = 5;

        public AdHocBatches AnalyzeAdHocBatches { get; set; }

        //----------------------------------------------------------------------------
        // Allow database and recommendation types to be blocked.
        // 
        public IEnumerable<string> BlockedDatabases { get; set; }
        public IEnumerable<RecommendationType> BlockedRecommendationTypes { get; set; }

        //----------------------------------------------------------------------------
        // Allow filtering on only specific applications and databases.
        // 
        public string FilterApplicationName { get; set; }
        public string FilterDatabaseName { get; set; }

        public string TargetCategory { get; set; }
        private List<string> _targetCategories;
        public List<string> TargetCategories 
        {
            get
            {
                if (null == _targetCategories) return (null);
                return (new List<string>(_targetCategories));
            }
            set
            {
                if (null == value) { _targetCategories = null; return; }
                _targetCategories = new List<string>(value);
            }
        }

        public AnalysisConfiguration(SqlConnectionInfo connectionInfo)
        {
            RequestId = Guid.NewGuid();
            this.connectionInfo = connectionInfo.Clone();
            lock (this.GetType())
            {
                TraceId = nextTraceId++;
            }
        }

        public Guid GetRequestId() { return (RequestId); }

        // Prevent this fuqer from expiring 
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public string InstanceName
        {
            get { return connectionInfo.InstanceName; }
        }

        public SqlConnectionInfo ConnectionInfo
        {
            get { return connectionInfo; }
        }

        public bool TraceQueries
        {
            get { return RunBestPracticesOnWorkload || RunIndexAnalysisOnWorkload; }
        }

        public bool IsAdHocBatchAnalysis { get { return (null != AnalyzeAdHocBatches); } }

        public void SetTraceOptions(int totalDurationMinutes)
        {
            TraceTotalDurationMinutes = totalDurationMinutes;
        }

        public int EstimatedRunTimeSeconds
        {
            get { return Math.Max(TraceTotalDurationMinutes * 72, SnapshotTotalDurationMinutes * 60); }
        }

        internal void FireStatusChanged(AnalysisStatus status, object data)
        {
            if (AnalysisStatusChanged != null)
            {
                AnalysisStatusHandler handler = null;
                foreach (Delegate del in AnalysisStatusChanged.GetInvocationList())
                {
                    try
                    {
                        handler = (AnalysisStatusHandler)del;
                        handler(GetRequestId(), status, data);
                    }
                    catch 
                    {
                        // remove trouble makers
                        AnalysisStatusChanged -= handler;
                    }
                }
            }
        }
    }
}
