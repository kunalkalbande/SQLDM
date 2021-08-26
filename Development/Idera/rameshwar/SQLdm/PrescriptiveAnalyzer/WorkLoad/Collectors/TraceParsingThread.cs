using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Data.SqlClient;

using Microsoft.Data.Schema.ScriptDom.Sql;

using Idera.SQLdm.PrescriptiveAnalyzer.Batches;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Stats;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Collectors;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Cache;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
//using Idera.SQLdm.PrescriptiveAnalyzer.Engine;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.State;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Collectors
{
    internal class TraceParsingThread
    {
        private static Logger _logX = Logger.GetLogger("TraceParsingThread");

        private Queue<Queue<TEBase>> _queue = new Queue<Queue<TEBase>>(10);
        private object _lockQueue = new object();

        private TSql90Parser _parser = new TSql90Parser(false);
        private BaseDataCollector _data = null;
        private BaseOptions _ops = null;
        private TraceEventStatsCollection _testats = null;
        private object _locktestats = new object();

        private object _lockThreadStartStop = new object();
        private Thread _thread = null;
        private EventWaitHandle _eventSignal = null;
        private SqlSystemObjectManager _ssom = null;
        private volatile bool _stopProcessing = false;
        private volatile bool _join = false;
        private readonly int _traceDurationMinutes = 0;
        private PlanCache _planCache = null;

        public BaseOptions BaseOptions { get { return (_ops); } }
        public SqlSystemObjectManager SSOM { get { return (_ssom); } }

        private TraceParsingThread() { }

        public TraceParsingThread(StringCache tsqlCache) { _data = new BaseDataCollector(tsqlCache); }

        public TraceParsingThread(BaseOptions ops, SqlSystemObjectManager ssom, int totalDurationMinutes, StringCache tsqlCache, PlanCache planCache) 
        {
            _traceDurationMinutes = totalDurationMinutes;
            _ssom = ssom;
            _ops = ops.BaseClone();
            _data = new BaseDataCollector(tsqlCache);
            _planCache = planCache;
        }

        public void AddEvents(Queue<TEBase> events, bool processAsync)
        {
            if (null == events) return;
            if (events.Count <= 0) return;
            //----------------------------------------------------------------------------
            // If the caller does not want to process the events async, process the events
            // now without queuing them.
            // 
            if (!processAsync)
            {
                ProcessEvents(events);
                return;
            }
            lock (_lockQueue)
            {
                _queue.Enqueue(events);
            }
            lock (_lockThreadStartStop)
            {
                if (null == _thread)
                {
                    _thread = new Thread(this.ProcessThread);
                    _thread.Name = "TraceParsingThread";
                    _thread.Priority = ThreadPriority.Lowest;
                    _stopProcessing = false;
                    _join = false;
                }
                if (null == _eventSignal) _eventSignal = new EventWaitHandle(false, EventResetMode.ManualReset);
                _eventSignal.Set();
                if (!_thread.IsAlive) _thread.Start();
            }
        }
        private void ProcessThread()
        {
            while (!_stopProcessing)
            {
                _eventSignal.WaitOne();
                _eventSignal.Reset();
                if (_stopProcessing) break;
                while(ProcessQueue());
                if (_join) break;
            }
        }

        private bool ProcessQueue()
        {
            Queue<TEBase> events = null;
            lock (_lockQueue)
            {
                if (_queue.Count > 0) events = _queue.Dequeue();
            }
            if (null != events) return (ProcessEvents(events));
            return (false);
        }

        private bool ProcessEvents(Queue<TEBase> events)
        {
            if (null == events) return (false);
            int count = events.Count;
            using (_logX.DebugCall((string.Format("TraceParsingThread Processed {0} events", count))))
            {
                using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                {
                    while (events.Count > 0)
                    {
                        if (_stopProcessing) break;
                        ProcessEvent(events.Dequeue(), sha1);
                    }
                }
            }
            return (!_stopProcessing);
        }

        public void Join()
        {
            using (_logX.InfoCall("TraceParsingThread.Join()"))
            {
                lock (_lockThreadStartStop)
                {
                    _join = true;
                    if (null != _eventSignal) _eventSignal.Set();
                    if (null != _thread)
                    {
                        if (_thread.IsAlive)
                        {
                            _logX.Info("Thread is alive.  Join...");
                            _thread.Join();
                            _logX.Info("Thread joined");
                        }
                        _thread = null;
                    }
                    _join = false;
                }
                lock (_lockQueue) { _queue.Clear(); }
            }
        }

        private bool SkipTraceEntry(TEBase te)
        {
            if (null == te) return (true);
            if (te.TextData.StartsWith(BatchConstants.CopyrightNotice)) return (true);
            if (te.TextData.StartsWith(BatchConstants.SwapBatchNotice)) return (true);
            if (te.TextData.StartsWith("-- SQL diagnostic manager")) return (true);
            if (!string.IsNullOrEmpty(te.ApplicationName)) 
            { 
                if (te.ApplicationName.StartsWith(Constants.ConnectionStringApplicationNamePrefix, StringComparison.InvariantCultureIgnoreCase)) return (true);
                if (te.ApplicationName.StartsWith(Constants.ConnectionStringSQLdmApplicationNamePrefix, StringComparison.InvariantCultureIgnoreCase)) return (true);                 
            }
            return (false);
        }
        private void ProcessEvent(TEBase te, SHA1 sha1)
        {
            if (null == te) return;
            if (null != _ops) { if (_ops.IsDatabaseBlocked(te.DBID)) return; }
            if (SkipTraceEntry(te)) { return; }
            _data.AddData(te, _parser, sha1);
        }
        public void LogStats()
        {
            _data.LogStats();
        }
        public Dictionary<string, List<DataBucketRanking>> GetResults()
        {
            using (_logX.InfoCall("TraceParsingThread.GetResults()"))
            {
                return (_data.GetResults());
            }
        }

        public List<DataBucketRanking> GetLongestRunning()
        {
            using (_logX.InfoCall("TraceParsingThread.GetLongestRunning()"))
            {
                Dictionary<string, List<DataBucketRanking>> results = GetResults();
                List<DataBucketRanking> data = null;
                results.TryGetValue("TotalDuration", out data);
                return (data);
            }
        }

        public TraceEventStatsCollection GetRankedResults()//(Progress progress)
        {
            using (_logX.InfoCall("TraceParsingThread.GetRankedResults()"))
            {
                lock (_locktestats)
                {
                    if (null != _testats) return (_testats);
                    _testats = new TraceEventStatsCollection(_ssom);
                    _testats.TraceDurationMinutes = _traceDurationMinutes;
                    DateTime stopTime = DateTime.Now.AddMinutes(_traceDurationMinutes);
                    //if (_stopProcessing || progress.IsCancelled()) return (_testats);
                    if (_stopProcessing ) return (_testats);
                    int count = 0;
                    //_ops.UpdateState(AnalysisStateType.ExecutionPlanCollection, "Starting execution plan collection...", 0, 100);
                    using (SqlConnection conn = SQLHelper.GetConnection(_ops.ConnectionInfo))
                    {
                        int current = 0;
                        int total = _data.GetDataBucketCount();
                        //_ops.UpdateState(AnalysisStateType.ExecutionPlanCollection, "Collecting plans...", current, total);
                        foreach (DataBucket d in _data.GetDataBuckets())
                        {
                            //_ops.UpdateState(AnalysisStateType.ExecutionPlanCollection, current++, total);
                            //if (_stopProcessing || progress.IsCancelled()) return (_testats);
                            if (_stopProcessing) return (_testats);
                            if (stopTime <= DateTime.Now)
                            {
                                _logX.InfoFormat("Aborted execution plan collection after {0} plans due to it taking too long!!", count);
                                break;
                            }
                            _testats.ActivityDuration += d.TotalDuration;
                            _testats.ActivityReads += d.TotalReads;
                            _testats.ActivityWrites += d.TotalWrites;
                            _testats.ActivityCPU += d.TotalCPU;
                            _testats.Add(new TraceEventStats(_ssom, d, GetPlan(conn, d.HighDuration)));
                            ++count;
                        }
                    }
                    _testats.SortByCost();
                    //_ops.UpdateState(AnalysisStateType.ExecutionPlanCollection, "Completed execution plan collection.", 100, 100);
                    return (_testats);
                }
            }
        }

        public TraceEventStatsCollection GetRankedResults(string queryPlan)
        {
            using (_logX.InfoCall("TraceParsingThread.GetRankedResults()"))
            {
                lock (_locktestats)
                {
                    if (null != _testats) return (_testats);
                    _testats = new TraceEventStatsCollection(_ssom);
                    _testats.TraceDurationMinutes = _traceDurationMinutes;
                    DateTime stopTime = DateTime.Now.AddMinutes(_traceDurationMinutes);
                    //if (_stopProcessing || progress.IsCancelled()) return (_testats);
                    if (_stopProcessing) return (_testats);
                    int count = 0;
                    //_ops.UpdateState(AnalysisStateType.ExecutionPlanCollection, "Starting execution plan collection...", 0, 100);
                    //using (SqlConnection conn = SQLHelper.GetConnection(_ops.ConnectionInfo))
                    //{
                    int current = 0;
                    int total = _data.GetDataBucketCount();
                    //_ops.UpdateState(AnalysisStateType.ExecutionPlanCollection, "Collecting plans...", current, total);
                    foreach (DataBucket d in _data.GetDataBuckets())
                    {
                        //_ops.UpdateState(AnalysisStateType.ExecutionPlanCollection, current++, total);
                        //if (_stopProcessing || progress.IsCancelled()) return (_testats);
                        if (_stopProcessing) return (_testats);
                        if (stopTime <= DateTime.Now)
                        {
                            _logX.InfoFormat("Aborted execution plan collection after {0} plans due to it taking too long!!", count);
                            break;
                        }
                        _testats.ActivityDuration += d.TotalDuration;
                        _testats.ActivityReads += d.TotalReads;
                        _testats.ActivityWrites += d.TotalWrites;
                        _testats.ActivityCPU += d.TotalCPU;
                        _testats.Add(new TraceEventStats(_ssom, d, GetPlan(queryPlan, d.HighDuration)));
                        ++count;
                    }
                    //}
                    _testats.SortByCost();
                    //_ops.UpdateState(AnalysisStateType.ExecutionPlanCollection, "Completed execution plan collection.", 100, 100);
                    return (_testats);
                }
            }
        }

        private TraceEventPlan GetPlan(SqlConnection conn, TEBase te)
        {
            try
            {
                SQLHelper.CheckConnection(conn);
                _ops.UpdateDatabaseName(conn, te.DBID);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("TraceParsingThread.GetPlan() Exception:", ex);
            }
            return (new TraceEventPlan(conn, te, _planCache));

        }

        private TraceEventPlan GetPlan(string queryPlan, TEBase te)
        {
            return (new TraceEventPlan(queryPlan, te, _planCache));
        }


        internal void AddWorstTSQL(WorstPerformingTSQL w)
        {
            if (null == w.WorstTSQL) return;
            using (_logX.InfoCall("TraceParsingThread.AddWorstTSQL()"))
            {
                using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
                {
                    foreach (TEWorstTSQL te in w.WorstTSQL)
                    {
                        if (null != _ops) { if (_ops.IsDatabaseBlocked(te.DBID)) continue; }
                        if (SkipTraceEntry(te)) { continue; }
                        if (IsSystemObject(te)) { continue; }
                        _data.AddData(te, _parser, sha1);
                    }
                }
            }
        }

        private bool IsSystemObject(TEWorstTSQL te)
        {
            if (SQLHelper.RESOURCEDB_ID == te.DBID) return (true);
            if (string.IsNullOrEmpty(te.ObjectName)) return (false);
            if (null == _ssom) return (false);
            return (_ssom.IsSystemObject(te.DBID, te.ObjectID, te.ObjectName));
        }

        internal void MergeResults(TraceParsingThread tpt)
        {
            using (_logX.InfoCall("TraceParsingThread.MergeResults()"))
            {
                _data.Merge(tpt._data);
            }
        }

        internal void DumpData()
        {
            using (_logX.InfoCall("DumpData()"))
            {
                _data.DumpData();
            }
        }
    }
}
