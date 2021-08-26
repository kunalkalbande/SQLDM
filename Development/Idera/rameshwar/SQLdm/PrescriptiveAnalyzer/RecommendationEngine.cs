using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Analyzer;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using System.Data.SqlClient;
using Idera.SQLdm.Common.Snapshots;
using System.Data;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.AdHoc;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Collectors;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents;
using Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Cache;
using System.IO;
using Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.State;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Analyzers;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;

using Idera.SQLdm.PrescriptiveAnalyzer.Properties;
using System.Threading;

namespace Idera.SQLdm.PrescriptiveAnalyzer
{
    public class RecommendationEngine
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("RecommendationEngine");

        private AnalysisConfiguration config;
        private ServerVersion serverVersion;
        private List<IRecommendation> _recommendations;
        private Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration.SqlConnectionInfo connectionInfo;
        //private SqlConnection sqlConnection;
        private AnalysisState analysisState = new AnalysisState();
        private EventWaitHandle traceWaitHandle;
        private EventWaitHandle exEventWaitHandler;
        private List<Exception> exceptions;

        private List<IAnalyze> _analyzers;
        private SnapshotMetrics _sm;
        private PrescriptiveAnalyticsSnapshot _snap;

        private Result _result;

        private StringCache TSQLCache = null;
        private PlanCache PlanCache = null;
        private SqlSystemObjectManager SystemObjectManager = null;

        private List<string> _blockedDbs;
        //To support SDR-M16
        private Idera.SQLdm.PrescriptiveAnalyzer.Common.Values.SnapshotValues previousSnapshotValue;
        public RecommendationEngine(AnalysisConfiguration config, PrescriptiveAnalyticsSnapshot pa)
        {

            _snap = pa;
            this.config = config;
            connectionInfo = new Common.Configuration.SqlConnectionInfo();
            connectionInfo.ConnectionString = pa.ConnectionString;
            //sqlConnection = new SqlConnection(pa.ConnectionString);
            connectionInfo.InstanceName = GetInstanceFromConnectionString(pa.ConnectionString);

            if (!string.IsNullOrEmpty(pa.MachineName))
            {
                connectionInfo.UseRemoteWmi = true;
                WmiConnectionInfo info = new WmiConnectionInfo();
                info.MachineName = pa.MachineName;
                connectionInfo.WmiConnectionInfo = info;
            }

            if (config != null && config.BlockedDatabases != null)
            {
                _blockedDbs = new List<string>();
                foreach (string db in config.BlockedDatabases.Values)
                {
                    _blockedDbs.Add(db.Trim());
                }
            }

            SystemObjectManager = new SqlSystemObjectManager(connectionInfo);
            _sm = new SnapshotMetrics();
            _result = new Result();
            this._recommendations = new List<IRecommendation>();
        }

        public RecommendationEngine(AnalysisConfiguration config, PrescriptiveAnalyticsSnapshot pa, Idera.SQLdm.PrescriptiveAnalyzer.Common.Values.SnapshotValues snapshotValue)
            :this(config, pa)
        {
            previousSnapshotValue = snapshotValue;
        }

        // for test purpose
        //public RecommendationEngine(PrescriptiveAnalyticsSnapshot pa)
        //{
        //    _snap = pa;
        //    if (pa.DatabaseRankingSnapshotValue != null) { _databaseRanks = pa.DatabaseRankingSnapshotValue.DatabaseRanks; }
        //    _sm = new SnapshotMetrics();
        //    connectionInfo = new Common.Configuration.SqlConnectionInfo();
        //    connectionInfo.ConnectionString = pa.ConnectionString;
        //    //sqlConnection = new SqlConnection(pa.ConnectionString);
        //    _result = new Result();
            
        //    this._recommendations = new List<IRecommendation>();
        //}

        public List<IRecommendation> Recommendations
        {
            get { return _recommendations; }
        }

        public Result Results
        {
            get { return _result; }
        }

        public void Run()
        {
            using (LOG.InfoCall("RecommendationEngine.Run()"))
            {
                _result.AnalysisStartTime = DateTime.Now;

                AnalysisState state = new AnalysisState();
                SnapshotMetricOptions options = new SnapshotMetricOptions(state);
                options.ConnectionInfo = connectionInfo.Clone();
                options.IsAdHocBatchAnalysis = false;
                if (config != null)
                {
                    if (config.BlockedDatabases != null) options.BlockedDatabases = config.BlockedDatabases.Values;
                    //bo.BlockedRecommendationTypes = config.BlockedRecommendationID;
                    options.FilterApplicationName = config.FilterApplication;
                    //bo.FilterCategory = config.TargetCategory;
                    //bo.FilterCategories = config.BlockedCategories.Values;
                    options.FilterDatabaseName = config.IncludeDatabaseName;
                    //bo.GenerateTestRecommendations = config.GenerateTestRecommendations;
                    options.IsProductionServer = config.ProductionServer;
                    options.OLTPServer = config.IsOLTP;
                }

                

                PopulateMetrics(_snap, options);
                Analyze(_sm);
                _result.AnalysisCompleteTime = DateTime.Now;
                _result.TotalRecommendationCount = _recommendations.Count;
                RankRecommendations();
                _result.LatestSnapshot = _sm.Current;
            }
        }

        private void PopulateMetrics(PrescriptiveAnalyticsSnapshot pa, SnapshotMetricOptions options)
        {
            using (LOG.InfoCall("RecommendationEngine.PopulateMetrics()"))
            {
                _sm.AddSnapshot(pa, options);
                _sm.Previous = previousSnapshotValue;
            }
        }

        private void Analyze(SnapshotMetrics sm)
        {
            using (LOG.InfoCall("RecommendationEngine.Analyze"))
            {
                _analyzers = new List<IAnalyze>(GetAnalyzers());

                foreach (IAnalyze a in _analyzers)
                {
                    try
                    {
                        AnalyzerResult res = new AnalyzerResult();
                        a.Analyze(sm, connectionInfo.GetConnection());
                        res.AnalyzerID = a.ID;
                        IEnumerable<IRecommendation> recommendations = a.GetRecommendations();
                        if (null != recommendations)
                        {
                            foreach (IRecommendation r in recommendations)
                            {
                                bool addRecoInResult = true;
                                if (config != null && config.BlockedRecommendationID != null && config.BlockedRecommendationID.Contains(r.ID))
                                {
                                    addRecoInResult = false;
                                }
                                if (config != null && config.BlockedCategories != null && ContainsCategory(r.Category))
                                {
                                    addRecoInResult = false;
                                }

                                if (addRecoInResult == true)
                                {
                                    res.RecommendationList.Add(r);
                                    _recommendations.Add(r);
                                }
                            }
                        }
                        _result.AnalyzerRecommendationList.Add(res);
                    }
                    catch(Exception ex)
                    {
                        LOG.ErrorFormat("Analyzer {0} failed. Exception raised as {1}. Continuing with next analyzer", a.ToString(), ex.Message);
                        LOG.Error(ex);
                    }
                }
            }
        }

        private bool ContainsCategory(string category)
        {
            bool contains = false;
            if (config != null && config.BlockedCategories != null)
            {
                foreach (string strCategory in config.BlockedCategories.Values)
                {
                    if (strCategory.Trim().ToLower() == category.Trim().ToLower())
                    {
                        contains = true;
                    }
                }
            }
            return contains;
        }

        private void RankRecommendations()
        {
            using (LOG.InfoCall("RecommendationEngine.RankRecommendations"))
            {
                string IncludeDatabaseName = string.Empty;
                string FilterApplication = string.Empty;
                if (config != null)
                {
                    IncludeDatabaseName = config.IncludeDatabaseName;
                    FilterApplication = config.FilterApplication;
                }
                // To updated computed rank factor for list in Result varaiable
                //10.0 SQLdm Srishti Purohit
                foreach(AnalyzerResult res in _result.AnalyzerRecommendationList)
                {
                    Recommendation.RankRecommendations(res.RecommendationList, IncludeDatabaseName, FilterApplication, connectionInfo);
                }
            }
        }

        private IEnumerable<IAnalyze> GetAnalyzers()
        {
            yield return new BackupAndRecoveryAnalyzer();
            yield return new DBPropertiesAnalyzer();
            yield return new DBSecurityAnalyzer();
            yield return new FragmentedIndexesAnalyzer();
            yield return new DisabledIndexAnalyzer();
            yield return new HighIndexUpdatesAnalyzer();
            yield return new IndexContentionAnalyzer();
            yield return new OutOfDateStatsAnalyzer();
            yield return new OverlappingIndexesAnalyzer();
            yield return new SQLModuleOptionsAnalyzer();
            yield return new ServerConfigurationAnalyzer();
            yield return new IoAnalyzer();
            yield return new MemoryAnalyzer();
            yield return new NetworkAnalyzer();
            yield return new ProcessorAnalyzer();
            yield return new WaitStatsAnalyzer();
            //SQLdm 10.0 Adding New Recommendation Srishti Purohit
            yield return new NonIncrementalColumnStatAnalyzer();
            yield return (new HashIndexAnalyzer());
            yield return (new QueryStoreAnalyzer());
            yield return (new InMemoryTableIndexAnalyzer());
            yield return (new QueryAnalyzer());
            yield return (new ColumnStoreIndexAnalyzer());
            yield return (new FilteredIndexAnalyzer());
            yield return (new HighCPUTimeProcedureAnalyzer());
            yield return (new LargeTableStatsAnalyzer());
        }


        public void RunAdHocBatchAnalysis(AdHocBatches batches)
        {
            using (LOG.InfoCall("RecommendationEngine.RunAdHocBatchAnalysis"))
            {
                _result.AnalysisStartTime = DateTime.Now;
                CreateCache();
                AnalysisState state = new AnalysisState();
                BaseOptions bo = new BaseOptions(state);
                bo.ConnectionInfo = connectionInfo.Clone();
                bo.IsAdHocBatchAnalysis = true;
                int totalDurationMinute = 0;
                if (config != null)
                {
                    if (config.BlockedDatabases != null) bo.BlockedDatabases = config.BlockedDatabases.Values;
                    //bo.BlockedRecommendationTypes = config.BlockedRecommendationID;
                    bo.FilterApplicationName = config.FilterApplication;
                    //bo.FilterCategory = config.TargetCategory;
                    //bo.FilterCategories = config.BlockedCategories.Values;
                    bo.FilterDatabaseName = config.IncludeDatabaseName;
                    //bo.GenerateTestRecommendations = config.GenerateTestRecommendations;
                    bo.IsProductionServer = config.ProductionServer;
                    totalDurationMinute = config.AnalysisDuration;
                }

                TraceParsingThread tpt = new TraceParsingThread(bo, SystemObjectManager, totalDurationMinute, TSQLCache, PlanCache);
                Queue<TEBase> q = new Queue<TEBase>(batches.Count);
                foreach (AdHocBatch b in batches)
                {
                    for (int n = 0; n < b.ExecutionCount; ++n)
                    {
                        q.Enqueue(new TEBatchComplete(b));
                    }
                }
                tpt.AddEvents(q, false);
                tpt.Join();
                if (Properties.Settings.Default.DumpDataBuckets) { tpt.DumpData(); }
                var tesc = tpt.GetRankedResults();
                RunTraceAnalyzers(tpt);
                _result.AnalysisCompleteTime = DateTime.Now;
                _result.TotalRecommendationCount = _recommendations.Count;
                RankRecommendations();
                _result.LatestSnapshot = _sm.Current;
            }
        }

        public void RunWorkLoadAnalysis(ActiveWaitsConfiguration waitConfig, QueryMonitorConfiguration queryConfig)
        {
            using (LOG.InfoCall("RecommendationEngine.RunWorkLoadAnalysis"))
            {
                using (SqlConnection conn = connectionInfo.GetConnection())
                {
                    conn.Open();
                    serverVersion = new ServerVersion(conn.ServerVersion);
                }

                if (serverVersion.Major >= 11)
                {
                    RunWorkLoadAnalysisUsingExEvents(waitConfig, queryConfig);
                }
                else
                {
                    RunWorkLoadAnalysisUsingTrace();
                }
            }
        }

        public void RunWorkLoadAnalysisUsingExEvents(ActiveWaitsConfiguration waitConfig, QueryMonitorConfiguration queryConfig)
        {
            using (LOG.InfoCall("RecommendationEngine.RunWorkLoadAnalysisUsingExEvents"))
            {
                ExtendedEventCollector exEventSession = null;
                _result.AnalysisStartTime = DateTime.Now;
                try
                {
                    LogExEventSettings();
                    CreateCache();
                    int analysisDurationMinutes = 5;
                    string filterApp = string.Empty;
                    if (config != null)
                    {
                        analysisDurationMinutes = config.AnalysisDuration;
                        filterApp = config.FilterApplication;
                    }
                    if (_blockedDbs == null) _blockedDbs = new List<string>();
                    if (!_blockedDbs.Contains("master")) { _blockedDbs.Add("master"); }
                    if (!_blockedDbs.Contains("msdb")) { _blockedDbs.Add("msdb"); }
                    if (!_blockedDbs.Contains("model")) { _blockedDbs.Add("model"); }

                    bool runExEvent2 = (Settings.Default.ExEvent2_Enabled &&
                                        (Settings.Default.ExEvent2_MinQueryTime > Settings.Default.ExEvent1_MinQueryTime) &&
                                        (Settings.Default.ExEvent1_MaxQueryTime > 0) &&
                                        (Settings.Default.ExEvent1_MaxQueryTime <= Settings.Default.ExEvent2_MinQueryTime));

                    List<EventWaitHandle> waits = new List<EventWaitHandle>();
                    List<ExtendedEventCollector> sessions = new List<ExtendedEventCollector>();

                    ExtendedEventCollectorOptions options = new ExtendedEventCollectorOptions(analysisState, TSQLCache, PlanCache, 1,//config.TraceId,
                                                                            connectionInfo,
                                                                            Settings.Default.ExEvent1_Continuous,
                                                                            string.IsNullOrEmpty(filterApp),
                                                                            Settings.Default.ExEvent1_MinQueryTime,
                                                                            Settings.Default.ExEvent1_MaxQueryTime,
                                                                            Settings.Default.ExEvent1_SampleInterval,
                                                                            analysisDurationMinutes);
                    options.queryConfig = queryConfig;
                    options.waitConfig = waitConfig;
                    if (config != null)
                    {
                        if (_blockedDbs != null) { options.BlockedDatabases = _blockedDbs; }
                        //options.BlockedRecommendationTypes = config.BlockedRecommendationTypes;
                        options.FilterApplicationName = config.FilterApplication;
                        //options.FilterCategory = config.TargetCategory;
                        //options.FilterCategories = config.BlockedCategories.Values;
                        options.FilterDatabaseName = config.IncludeDatabaseName;
                        //options.GenerateTestRecommendations = config.GenerateTestRecommendations;
                        options.IsProductionServer = config.ProductionServer;
                    }
                    options.IsAdHocBatchAnalysis = false;

                    waits.Add(exEventWaitHandler = new EventWaitHandle(false, EventResetMode.ManualReset));
                    sessions.Add(exEventSession = new ExtendedEventCollector(exEventWaitHandler, options, SystemObjectManager));

                    LOG.Info("ExtendedEvents Session1 starting...");
                    exEventSession.Start();
                    LOG.Info("ExtendedEvents Session1 started");

                    if (runExEvent2)
                    {
                        LOG.Info("Trace2 is configured to run.");
                        options = new ExtendedEventCollectorOptions(analysisState, TSQLCache, PlanCache, 2,//config.TraceId + 1,
                                                                                connectionInfo,
                                                                                Settings.Default.ExEvent2_Continuous,
                                                                                false,
                                                                                Settings.Default.ExEvent2_MinQueryTime,
                                                                                Settings.Default.ExEvent2_MaxQueryTime,
                                                                                Settings.Default.ExEvent2_SampleInterval,
                                                                                analysisDurationMinutes);
                        options.queryConfig = queryConfig;
                        options.waitConfig = waitConfig;
                        if (config != null)
                        {
                            if (_blockedDbs != null) { options.BlockedDatabases = _blockedDbs; }
                            //options.BlockedRecommendationTypes = config.BlockedRecommendationTypes;
                            options.FilterApplicationName = config.FilterApplication;
                            //options.FilterCategory = config.TargetCategory;
                            //options.FilterCategories = config.TargetCategories;
                            options.FilterDatabaseName = config.IncludeDatabaseName;
                            //options.GenerateTestRecommendations = config.GenerateTestRecommendations;
                            options.IsProductionServer = config.ProductionServer;
                        }
                        options.IsAdHocBatchAnalysis = false;

                        waits.Add(traceWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset));
                        sessions.Add(exEventSession = new ExtendedEventCollector(exEventWaitHandler, options, SystemObjectManager));

                        LOG.Info("ExtendedEvents Session2 starting...");
                        exEventSession.Start();
                        LOG.Info("ExtendedEvents Session2 started");
                    }


                    LOG.Info("Waiting for exEvent sessions to complete...");
                    WaitHandle.WaitAll(waits.ToArray());
                    LOG.Info("ExEvent Sessions complete.");

                    Exception eventException = null;
                    foreach (var s in sessions)
                    {
                        AddExceptions(s.GetExceptions());
                        if (null == eventException) eventException = s.PeekException();
                    }
                    if (eventException == null)
                    {

                        exEventSession = sessions[0];
                        if (sessions.Count > 1)
                        {
                            LOG.Info("Merge exEvents results...");
                            exEventSession.MergeResults(sessions[1]);
                            LOG.Info("Merge results complete.");
                        }
                        if (Properties.Settings.Default.DumpDataBuckets)
                        {
                            DumpData(exEventSession);
                        }
                        RunTraceAnalyzers(exEventSession.ParsingThread);
                    }
                    else
                    {
                        //-----------------------------------------------
                        // If we are not starting the trace analyzers we will
                        // need to cleanup the cache files here since the new thread
                        // will not be created for the analysis and cleanup.
                        //
                        CleanupCache();
                    }
                    LOG.Info("Query trace complete.");
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Assert(false, e.Message);
                    LOG.Error(e);
                }
                finally
                {
                    if (exEventSession != null)
                        exEventSession.Stop();
                    _result.AnalysisCompleteTime = DateTime.Now;
                    _result.TotalRecommendationCount = _recommendations.Count;
                    RankRecommendations();
                    _result.LatestSnapshot = _sm.Current;
                }
            }
        }

        public void RunWorkLoadAnalysisUsingTrace()
        {
            using (LOG.InfoCall("RecommendationEngine.RunWorkLoadAnalysisUsingTrace"))
            {
                TraceCollector trace = null;
                _result.AnalysisStartTime = DateTime.Now;
                try
                {
                    LogTraceSettings();
                    CreateCache();
                    int analysisDurationMinutes = 5;
                    string filterApp = string.Empty;
                    if (config != null) 
                    { 
                        analysisDurationMinutes = config.AnalysisDuration;
                        filterApp = config.FilterApplication;
                    }
                    if (_blockedDbs == null) _blockedDbs = new List<string>();
                    if (!_blockedDbs.Contains("master")) { _blockedDbs.Add("master"); }
                    if (!_blockedDbs.Contains("msdb")) { _blockedDbs.Add("msdb"); }
                    if (!_blockedDbs.Contains("model")) { _blockedDbs.Add("model"); }

                    bool runTrace2 = (Settings.Default.Trace2_Enabled &&
                                        (Settings.Default.Trace2_MinQueryTime > Settings.Default.Trace1_MinQueryTime) &&
                                        (Settings.Default.Trace1_MaxQueryTime > 0) &&
                                        (Settings.Default.Trace1_MaxQueryTime <= Settings.Default.Trace2_MinQueryTime));
                    List<EventWaitHandle> waits = new List<EventWaitHandle>();
                    List<TraceCollector> traces = new List<TraceCollector>();

                    TraceCollectorOptions options = new TraceCollectorOptions(analysisState, TSQLCache, PlanCache, 1,//config.TraceId,
                                                                            connectionInfo,
                                                                            Settings.Default.Trace1_Continuous,
                                                                            string.IsNullOrEmpty(filterApp),
                                                                            Settings.Default.Trace1_MinQueryTime,
                                                                            Settings.Default.Trace1_MaxQueryTime,
                                                                            Settings.Default.Trace1_SampleInterval,
                                                                            analysisDurationMinutes);
                    if (config != null)
                    {
                        if (_blockedDbs != null) { options.BlockedDatabases = _blockedDbs; }
                        //options.BlockedRecommendationTypes = config.BlockedRecommendationTypes;
                        options.FilterApplicationName = config.FilterApplication;
                        //options.FilterCategory = config.TargetCategory;
                        //options.FilterCategories = config.BlockedCategories.Values;
                        options.FilterDatabaseName = config.IncludeDatabaseName;
                        //options.GenerateTestRecommendations = config.GenerateTestRecommendations;
                        options.IsProductionServer = config.ProductionServer;
                    }
                    options.IsAdHocBatchAnalysis = false;

                    waits.Add(traceWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset));
                    traces.Add(trace = new TraceCollector(traceWaitHandle, options, SystemObjectManager));

                    LOG.Info("Trace1 starting...");
                    trace.Start();
                    LOG.Info("Trace1 started");

                    if (runTrace2)
                    {
                        LOG.Info("Trace2 is configured to run.");
                        options = new TraceCollectorOptions(analysisState, TSQLCache, PlanCache, 2,//config.TraceId + 1,
                                                                                connectionInfo,
                                                                                Settings.Default.Trace2_Continuous,
                                                                                false,
                                                                                Settings.Default.Trace2_MinQueryTime,
                                                                                Settings.Default.Trace2_MaxQueryTime,
                                                                                Settings.Default.Trace2_SampleInterval,
                                                                                analysisDurationMinutes);
                        if (config != null)
                        {
                            if (_blockedDbs != null) { options.BlockedDatabases = _blockedDbs; }
                            //options.BlockedRecommendationTypes = config.BlockedRecommendationTypes;
                            options.FilterApplicationName = config.FilterApplication;
                            //options.FilterCategory = config.TargetCategory;
                            //options.FilterCategories = config.TargetCategories;
                            options.FilterDatabaseName = config.IncludeDatabaseName;
                            //options.GenerateTestRecommendations = config.GenerateTestRecommendations;
                            options.IsProductionServer = config.ProductionServer;
                        }
                        options.IsAdHocBatchAnalysis = false;

                        waits.Add(traceWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset));
                        traces.Add(trace = new TraceCollector(traceWaitHandle, options, SystemObjectManager));

                        LOG.Info("Trace2 starting...");
                        trace.Start();
                        LOG.Info("Trace2 started");
                    }


                    LOG.Info("Waiting for traces to complete...");
                    WaitHandle.WaitAll(waits.ToArray());
                    LOG.Info("Traces complete.");

                    Exception traceException = null;
                    foreach (var t in traces)
                    {
                        AddExceptions(t.GetExceptions());
                        if (null == traceException) traceException = t.PeekException();
                    }
                    if (traceException == null)
                    {
                        trace = traces[0];
                        if (traces.Count > 1)
                        {
                            LOG.Info("Merge trace results...");
                            trace.MergeResults(traces[1]);
                            LOG.Info("Merge results complete.");
                        }
                        if (Properties.Settings.Default.DumpDataBuckets)
                        {
                            DumpData(trace);
                        }
                        RunTraceAnalyzers(trace.ParsingThread);
                    }
                    else
                    {
                        //-----------------------------------------------
                        // If we are not starting the trace analyzers we will
                        // need to cleanup the cache files here since the new thread
                        // will not be created for the analysis and cleanup.
                        //
                        CleanupCache();
                    }
                    LOG.Info("Query trace complete.");
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Assert(false, e.Message);
                    LOG.Error(e);
                }
                finally
                {
                    if (trace != null)
                        trace.Stop();
                    _result.AnalysisCompleteTime = DateTime.Now;
                    _result.TotalRecommendationCount = _recommendations.Count;
                    RankRecommendations();
                    _result.LatestSnapshot = _sm.Current;
                }
            }
        }

        public void RunTraceAnalyzers(object traceParsing)
        {
            RunQueryBestPractices(traceParsing);
            RunIndexAnalysis(traceParsing);
            CleanupCache();
        }

        public void RunIndexAnalysis(object collector)
        {
            using (LOG.InfoCall("RecommendationEngine.RunIndexAnalysis"))
            {
                TraceParsingThread tpt = (TraceParsingThread)collector;
                int totalDurationMinute = 0;
                if (config != null) { totalDurationMinute = config.AnalysisDuration; }
                ExecutionPlanAnalyzer a = new ExecutionPlanAnalyzer(tpt.BaseOptions, tpt.SSOM, totalDurationMinute);
                a.Analyze(tpt.GetRankedResults());
                _recommendations.AddRange(a.GetRecommendations(config));
                _result.AnalyzerRecommendationList.AddRange(a.GetAnalyzerRecommendations(config));
            }
        }

        public void RunQueryBestPractices(object collector)
        {
            using (LOG.InfoCall("RecommendationEngine.RunQueryBestPractices"))
            {
                using (SqlConnection conn = connectionInfo.GetConnection())
                {
                    conn.Open();
                    serverVersion = new ServerVersion(conn.ServerVersion);
                }
                TraceParsingThread tpt = (TraceParsingThread)collector;
                QueryBestPracticeAnalyzer bpa = new QueryBestPracticeAnalyzer(serverVersion);
                List<DataBucketRanking> queryList = tpt.GetLongestRunning();
                if (queryList != null && queryList.Count > 0)
                {
                    SqlDbNameManager names = new SqlDbNameManager();
                    foreach (DataBucketRanking rank in queryList)
                    {
                        Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents.TEBase teb = rank.Bucket.HighDuration;
                        string script = teb.TextData;
                        string application = teb.ApplicationName;
                        string host = teb.HostName;
                        string user = teb.LoginName;
                        string database = "";
                        using (SqlConnection cnn = connectionInfo.GetConnection())
                        {
                            cnn.Open();
                            database = names.GetDatabaseName(cnn, rank.Bucket.HighDuration.DBID);
                        }

                        bpa.AnalyzeScript(database, application, user, host, script);
                    }
                    _recommendations.AddRange(bpa.GetRecommendations(config));
                    _result.AnalyzerRecommendationList.AddRange(bpa.GetAnalyzerRecommendations(config));
                    RunExecutionStatsAnalyzer(tpt);
                }
            }
        }

        void RunExecutionStatsAnalyzer(TraceParsingThread tpt)
        {
            List<DataBucketRanking> bucketList = tpt.GetLongestRunning();
            if (bucketList != null)
            {
                ExecutionStatsAnalyzer esa = new ExecutionStatsAnalyzer(tpt.BaseOptions, tpt.SSOM);
                esa.Analyze(bucketList);
                _recommendations.AddRange(esa.GetRecommendations(config));
                _result.AnalyzerRecommendationList.AddRange(esa.GetAnalyzerRecommendations(config));
            }
        }

        private void CreateCache()
        {
            CleanupCache();
            string cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Idera\SQLdm\Cache");
            // make sure the directory is accessible
            if (!Directory.Exists(cachePath))
            {
                // attemtp to create the directory
                Directory.CreateDirectory(cachePath);
            }

            string cacheName = string.Format("1_{0}.cache", GetSafeFilename(connectionInfo.InstanceName));
            TSQLCache = new StringCache(Path.Combine(cachePath, cacheName),
                                        Properties.Settings.Default.Cache_TSQL_MinSizeInMB,
                                        Properties.Settings.Default.Cache_TSQL_MaxSizeInMB,
                                        Properties.Settings.Default.Cache_TSQL_GrowSizeInMB);
            cacheName = string.Format("2_{0}.cache", GetSafeFilename(connectionInfo.InstanceName));
            PlanCache = new PlanCache(Path.Combine(cachePath, cacheName),
                                        Properties.Settings.Default.Cache_Plan_MinSizeInMB,
                                        Properties.Settings.Default.Cache_Plan_MaxSizeInMB,
                                        Properties.Settings.Default.Cache_Plan_GrowSizeInMB);
        }

        private string GetSafeFilename(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars()) { name = name.Replace(c, '_'); }
            return (name);
        }

        private void CleanupCache()
        {
            using (LOG.InfoCall("RecommendationEngine.CleanupCache()"))
            {
                if (null != TSQLCache)
                {
                    try
                    {
                        using (TSQLCache) { }
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogger.Log(LOG, "CleanupCache(TSQLCache) Exception:", ex);
                    }
                    TSQLCache = null;
                }
                if (null != PlanCache)
                {
                    try
                    {
                        using (PlanCache) { }
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogger.Log(LOG, "CleanupCache(PlanCache) Exception:", ex);
                    }
                    PlanCache = null;
                }
            }
        }

        private string GetInstanceFromConnectionString(string conStr)
        {
            string instance = string.Empty;
            if (conStr != null)
            {
                string[] a1 = conStr.Split(';');
                if (a1.Length > 0)
                {
                    conStr = a1[0];
                    string[] a2 = conStr.Split('=');
                    if (a2.Length == 2)
                    {
                        conStr = a2[1];
                        instance = conStr;
                    }
                }
            }
            return instance;
        }

        private void LogTraceSettings()
        {
            using (LOG.InfoCall("LogTraceSettings()"))
            {
                LOG.InfoFormat("Trace1_Continuous:{0}", Settings.Default.Trace1_Continuous);
                LOG.InfoFormat("Trace1_MinQueryTime:{0}", Settings.Default.Trace1_MinQueryTime);
                LOG.InfoFormat("Trace1_MaxQueryTime:{0}", Settings.Default.Trace1_MaxQueryTime);
                LOG.InfoFormat("Trace1_SampleInterval:{0}", Settings.Default.Trace1_SampleInterval);
                LOG.InfoFormat("Trace2_Enabled:{0}", Settings.Default.Trace2_Enabled);
                LOG.InfoFormat("Trace2_Continuous:{0}", Settings.Default.Trace2_Continuous);
                LOG.InfoFormat("Trace2_MinQueryTime:{0}", Settings.Default.Trace2_MinQueryTime);
                LOG.InfoFormat("Trace2_MaxQueryTime:{0}", Settings.Default.Trace2_MaxQueryTime);
                LOG.InfoFormat("Trace2_SampleInterval:{0}", Settings.Default.Trace2_SampleInterval);
            }
        }

        private void LogExEventSettings()
        {
            using (LOG.InfoCall("LogExEventSettings()"))
            {
                LOG.InfoFormat("ExEvent1_Continuous:{0}", Settings.Default.ExEvent1_Continuous);
                LOG.InfoFormat("ExEvent1_MinQueryTime:{0}", Settings.Default.ExEvent1_MinQueryTime);
                LOG.InfoFormat("ExEvent1_MaxQueryTime:{0}", Settings.Default.ExEvent1_MaxQueryTime);
                LOG.InfoFormat("ExEvent1_SampleInterval:{0}", Settings.Default.ExEvent1_SampleInterval);
                LOG.InfoFormat("ExEvent2_Enabled:{0}", Settings.Default.ExEvent2_Enabled);
                LOG.InfoFormat("ExEvent2_Continuous:{0}", Settings.Default.ExEvent2_Continuous);
                LOG.InfoFormat("ExEvent2_MinQueryTime:{0}", Settings.Default.ExEvent2_MinQueryTime);
                LOG.InfoFormat("ExEvent2_MaxQueryTime:{0}", Settings.Default.ExEvent2_MaxQueryTime);
                LOG.InfoFormat("ExEvent2_SampleInterval:{0}", Settings.Default.ExEvent2_SampleInterval);
            }
        }

        public void AddException(Exception ex)
        {
            if (null == ex) return;
            exceptions.Add(ex); 
        }

        public void AddExceptions(IEnumerable<Exception> e)
        {
            if (null == e) return;
            try { exceptions.AddRange(e); }
            catch (Exception ex)
            {
               ExceptionLogger.Log(LOG, "AddExceptions() Exception: ", ex); 
            }
        }

        private void DumpData(TraceCollector trace)
        {
            using (LOG.InfoCall("Dump data buckets"))
            {
                trace.DumpData();
            }
        }

        private void DumpData(ExtendedEventCollector exevent)
        {
            using (LOG.InfoCall("Dump data buckets"))
            {
                exevent.DumpData();
            }
        }
    }
}
