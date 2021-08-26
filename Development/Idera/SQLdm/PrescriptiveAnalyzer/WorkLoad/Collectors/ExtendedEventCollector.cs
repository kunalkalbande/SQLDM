using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Stats;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Batches;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.State;
using System.Management;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Collectors
{
    public class ExtendedEventCollector
    {
        private static Logger _logX = Logger.GetLogger("ExtendedEventCollector");

        private ExtendedEventCollectorOptions _options = null;
        private ExtendedEventCollectorOptions _optionsAdjusted = null;
        private object _lockStartStop = new object();
        private bool _started = false;
        private bool _stopRequested = false;
        private Thread _thread = null;
        private EventWaitHandle _eventSignal = null;
        private EventWaitHandle _completeEvent = null;
        private DateTime _endTime = DateTime.MinValue;
        private UInt64 _eventSeq = 0;
        private int _rowsSinceLastTruncate = 0;
        private TraceParsingThread _traceParsingThread = null;
        private long _totalSampleSeconds = 0;
        private DateTime _lastLog = DateTime.MinValue;

        private Stack<Exception> _errors = null;
        private object _lockErrors = new object();

        private static int MAX_TRACE_FILE_ENTRIES = 12288;

        public BaseOptions BaseOptions { get { return (null == _options ? _options : _options.BaseClone()); } }
        public SqlSystemObjectManager SSOM { get; private set; }
        internal TraceParsingThread ParsingThread { get { return (_traceParsingThread); } }

        private ExtendedEventCollector() { }

        public ExtendedEventCollector(EventWaitHandle completeEvent, ExtendedEventCollectorOptions options, SqlSystemObjectManager ssom) 
        {
            SSOM = ssom;
            _options = options;
            _completeEvent = completeEvent;
            _traceParsingThread = new TraceParsingThread(options, SSOM, options.TotalDurationMinutes, options.TSQLCache, options.PlanCache);
        }

        public bool Start()
        {
            lock (_lockStartStop)
            {
                if (_started) return (false);
                if (null != _thread)
                {
                    if (_thread.IsAlive) return (false);
                }
                else
                {
                    _options.UpdateState(AnalysisStateType.ProfilerTraceCollection, "Starting profiler trace collection...", 0, 100);
                    _thread = new Thread(ProcessSession);
                    _thread.Name = string.Format("SQLTrace.ProcessTrace({0})", _options.Name);
                }
                if (null == _eventSignal) _eventSignal = new EventWaitHandle(false, EventResetMode.AutoReset);
                _eventSignal.Reset();
                _thread.Start();
                _stopRequested = false;
                _started = true;
            }
            return (true);
        }

        private void ProcessSession(SqlConnection conn)
        {
            int sampleDuration;
            int processingDuration;
            TimeSpan ts;
            DateTime processingStarted = DateTime.MinValue;

            try
            {
                _endTime = DateTime.Now.AddMinutes(_options.TotalDurationMinutes);
                _optionsAdjusted = _options.Clone() as ExtendedEventCollectorOptions;
                sampleDuration = _optionsAdjusted.SampleDurationSeconds;
                CreateSession(conn, _options);


                if (_options.ContinuousCollection)
                {
                    while (!_eventSignal.WaitOne(0, false))
                    {
                        LogStats();
                        CollectAndProcess(conn, _optionsAdjusted);
                        if (_endTime <= DateTime.Now) break;
                    }
                }
                else
                {
                    while (!_eventSignal.WaitOne(TimeSpan.FromSeconds(sampleDuration), false))
                    {
                        LogStats();
                        processingStarted = DateTime.Now;
                        CollectAndProcess(conn, _optionsAdjusted);
                        if (_endTime <= DateTime.Now) break;

                        //----------------------------------------------------------------------------
                        // Find out how long we need to wait for collecting the next sample.  If it takes
                        // us too long to process the results we may end up not sampling the data fast
                        // enough.
                        // 
                        sampleDuration = _optionsAdjusted.SampleDurationSeconds;
                        ts = DateTime.Now - processingStarted;
                        processingDuration = Convert.ToInt32(ts.TotalSeconds);
                        if (processingDuration < sampleDuration)
                        {
                            sampleDuration -= processingDuration;
                        }
                        else
                        {
                            sampleDuration = 0;
                        }
                    }
                }

            }
            catch (Exception ex)
            { 
                ExceptionLogger.Log(_logX, "ProcessSession Exception: ", ex); 
            }
            finally
            {
                try
                {
                    DeleteSession(conn, _options);
                }
                catch (Exception exDel)
                {
                    ExceptionLogger.Log(_logX, "DeleteSession Exception: ", exDel);
                }
            }
        }

        private void CollectAndProcess(SqlConnection conn, ExtendedEventCollectorOptions options)
        {
            using (_logX.DebugCall(string.Format("{0} - CollectAndProcess", options.Name)))
            {
                string sqlInsert = options.ContinuousCollection ? "" : BuildSamplingSql(options);
                string sql = BatchConstants.CopyrightNotice + string.Format(Properties.Resources.CollectExEventSessionData, options.Name, _eventSeq, sqlInsert);
                if (!options.ContinuousCollection) { _eventSeq = 0; }
                SQLHelper.CheckConnection(conn);


                try
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            ProcessSessionData(conn, r, options);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "CollectAndProcess Exception: ", ex);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    TruncateSession(conn, options);
                    return;
                }
            }
        }

        private string BuildSamplingSql(ExtendedEventCollectorOptions options)
        {
            if (options.ContinuousCollection) return string.Empty;
            TimeSpan ts = TimeSpan.FromSeconds(options.SampleDurationSeconds);
            string fmt = string.Format("{0}:{1}:{2}", ts.Hours.ToString("00"), ts.Minutes.ToString("00"), ts.Seconds.ToString("00"));
            return (BatchConstants.CopyrightNotice + string.Format(Properties.Resources.SampleExEventSession,options.Name, fmt, options.Name));
        }

        private void ProcessSessionData(SqlConnection conn, SqlDataReader r, ExtendedEventCollectorOptions options)
        {
            using (_logX.DebugCall(string.Format("ProcessTraceData({0})", _options.Name)))
            {
                int rows = 0;
                Queue<TEBase> q = new Queue<TEBase>();
                UInt64 seq = _eventSeq;
                TEBase te;
                while (r.Read())
                {
                    ++rows;
                    te = null;
                    switch (DataHelper.ToUInt64(r, "EventClass"))
                    {
                        case (10): { te = new TERpcComplete(r); break; }
                        case (12): { te = new TEBatchComplete(r); break; }
                        case (43): { te = new TESpComplete(r); break; }
                        default: { te = null; break; }
                    }
                    if (null != te) { q.Enqueue(te); if (te.EventSequence > seq) { seq = te.EventSequence; } }
                }
                _traceParsingThread.AddEvents(q, _options.ContinuousCollection);
                if (_eventSeq < seq) _eventSeq = seq;
                _logX.DebugFormat("Rows processed ({0}): {1}", _options.Name, rows);

                if (options.ContinuousCollection)
                {
                    if (_endTime > DateTime.Now) AdjustSessionOption(conn, _optionsAdjusted, rows);
                }
            }
        }

        private void AdjustSessionOption(SqlConnection conn, ExtendedEventCollectorOptions options, int rowCount)
        {
            using (_logX.DebugCall(string.Format("AdjustSessionOption({0})", options.Name)))
            {
                try
                {
                    _rowsSinceLastTruncate += rowCount;
                    _logX.DebugFormat("{0} - RowCount:{1}  RowsSinceLastTruncate:{2}", options.Name, rowCount, _rowsSinceLastTruncate);
                    if (_rowsSinceLastTruncate > MAX_TRACE_FILE_ENTRIES) TruncateSession(conn, options);
                    if (rowCount < 10)
                    {
                        if (options.MinQueryTime > _options.MinQueryTime)
                        {
                            options.MinQueryTime -= (options.MinQueryTime - _options.MinQueryTime) / 2;
                            _logX.DebugFormat("{0} - Changing MingQueryTime={1}  RowCount:{2}", options.Name, options.MinQueryTime, rowCount);
                            TruncateSession(conn, options);
                            return;
                        }
                        if (options.SampleDurationSeconds < _options.SampleDurationSeconds)
                        {
                            options.SampleDurationSeconds += 1;
                            _logX.DebugFormat("{0} - Increasing SampleDurationSeconds={1}  RowCount:{2}", options.Name, options.SampleDurationSeconds, rowCount);
                        }
                        return;
                    }
                    if (rowCount < 2000) return;
                    if (options.SampleDurationSeconds > 1)
                    {
                        options.SampleDurationSeconds /= 2;
                        _logX.DebugFormat("{0} - Decreasing SampleDurationSeconds={1}  RowCount:{2}", options.Name, options.SampleDurationSeconds, rowCount);
                        return;
                    }
                    //----------------------------------------------------------------------------
                    // If we are collecting too much data and we have already dropped our sample
                    // duration down to a second, adjust to only collect queries that are running
                    // closer to our max query time.
                    // 
                    UInt64 maxQuery = (options.MaxQueryTime > 0) ? options.MaxQueryTime : UInt64.MaxValue;
                    UInt64 adjust = (maxQuery - options.MinQueryTime) / 4;
                    if (0 == adjust) adjust = (maxQuery - options.MinQueryTime);
                    if (adjust > (options.MinQueryTime * 4)) adjust = (options.MinQueryTime * 4);
                    if (maxQuery > (options.MinQueryTime + adjust))
                    {
                        options.MinQueryTime += adjust;
                        TruncateSession(conn, options);
                        _logX.DebugFormat("{0} - Adjusting MinQueryTime={1}  RowCount:{2}", options.Name, options.MinQueryTime, rowCount);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "AdjustSessionOption()  Exception: ", ex);
                }
            }
        }

        private void TruncateSession(SqlConnection conn, ExtendedEventCollectorOptions options)
        {
            using (_logX.InfoCall(string.Format("{0} - TruncateSession Min:{1} Max:{2}", options.Name, options.MinQueryTime, options.MaxQueryTime)))
            {
                _rowsSinceLastTruncate = 0;
                _eventSeq = 0;
                DeleteSession(conn, options);
                CreateSession(conn, options);
            }
        }

        private void LogStats()
        {
            try
            {
                if (DateTime.MinValue != _lastLog)
                {
                    TimeSpan ts = DateTime.Now - _lastLog;
                    if (ts.TotalMinutes < 1) return;
                }
                _traceParsingThread.LogStats();
                _lastLog = DateTime.Now;
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(_logX, "LogStats()", ex);
            }
        }

        private void ProcessSession()
        {
            try
            {
                Debug.Assert(null != _eventSignal, "Event was not created!");
                _rowsSinceLastTruncate = 0;
                using (SqlConnection conn = SQLHelper.GetConnection(_options.ConnectionInfo))
                {
                    ProcessSession(conn);
                    if (!_stopRequested && _options.CollectWorstPerformingTSQL)
                    {
                        _traceParsingThread.AddWorstTSQL(new WorstPerformingTSQL(conn, _options));
                    }
                }
            }
            catch (Exception ex)
            {
                PushException(ex);
            }
            finally
            {
                _traceParsingThread.Join();
                _started = false;
                if (null != _completeEvent) _completeEvent.Set();
            }
        }

        public Exception PeekException()
        {
            lock (_lockErrors)
            {
                if (null != _errors) return ((_errors.Count > 0) ? _errors.Peek() : null);
            }
            return (null);
        }
        public Exception PopException()
        {
            lock (_lockErrors)
            {
                if (null != _errors) return ((_errors.Count > 0) ? _errors.Pop() : null);
            }
            return (null);
        }
        private void PushException(Exception ex)
        {
            lock (_lockErrors)
            {
                if (null == _errors) _errors = new Stack<Exception>();
                _errors.Push(ex);
            }
        }

        public List<Exception> GetExceptions()
        {
            lock (_lockErrors)
            {
                if (null == _errors) return (null);
                return (new List<Exception>(_errors));
            }
        }

        public bool Stop()
        {
            using (_logX.InfoCall("Stop()"))
            {
                lock (_lockStartStop)
                {
                    _stopRequested = true;
                    if (!_started) return (false);
                    Debug.Assert(null != _thread, "Thread was not created!");
                    Debug.Assert(null != _eventSignal, "Event was not created!");
                    if ((null != _thread) && (null != _eventSignal)) _eventSignal.Set();
                    //_thread.Join();
                    //_started = false;
                }
                return (true);
            }
        }

        private static void CreateSession(SqlConnection conn, ExtendedEventCollectorOptions options)
        {
            using (_logX.InfoCall(string.Format("CreateSession({0})", options.Name)))
            {
                string startSession = options.ContinuousCollection ? string.Format(Properties.Resources.StartExEventSession,options.Name) : "";
                string sql = BatchConstants.CopyrightNotice + string.Format(Properties.Resources.CreateExEventSession, 
                                                options.MinQueryTime,
                                                options.MaxQueryTime,
                                                CreateSafeString(options.FilterApplicationName),
                                                CreateSafeString(options.FilterDatabaseName),
                                                options.Name, 
                                                options.Name, 
                                                startSession,
                                                options.queryConfig.FileSizeXeMB,
                                                options.waitConfig.MaxMemoryXeMB,
                                                options.waitConfig.EventRetentionModeXe,
                                                options.waitConfig.MaxDispatchLatencyXe,
                                                options.waitConfig.MaxEventSizeXemb,
                                                options.waitConfig.MemoryPartitionModeXe,
                                                options.waitConfig.TrackCausalityXe ? "ON" : "OFF",
                                                options.waitConfig.StartupStateXe ? "ON" : "OFF");

                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                    command.ExecuteNonQuery();
                }
            }
        }

        private static object CreateSafeString(string p)
        {
            if (string.IsNullOrEmpty(p)) return "''";
            return (SQLHelper.CreateSafeString(p));
        }

        private static void DeleteSession(SqlConnection conn, ExtendedEventCollectorOptions options)
        {
            string sql = BatchConstants.CopyrightNotice + string.Format(Properties.Resources.DeleteExEventSession, options.Name);
            using (SqlCommand command = new SqlCommand(sql, conn))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                command.ExecuteNonQuery();
            }
        }

        public Dictionary<string, List<DataBucketRanking>> GetResults()
        {
            return (_traceParsingThread.GetResults());
        }

        public List<DataBucketRanking> GetLongestRunning()
        {
            return (_traceParsingThread.GetLongestRunning());
        }
        public TraceEventStatsCollection GetRankedResults()
        {
            return (_traceParsingThread.GetRankedResults());
        }
        public void MergeResults(ExtendedEventCollector ec)
        {
            _traceParsingThread.MergeResults(ec._traceParsingThread);
        }

        public void Cleanup()
        {
        }

        #region ICancelable Members

        public void Cancel()
        {
            using (_logX.DebugCall("TraceCollector.Cancel()"))
            {
                Stop();
                Cleanup();
            }
        }

        #endregion

        internal void DumpData()
        {
            using (_logX.InfoCall("Dump data buckets"))
            {
                _traceParsingThread.DumpData();
            }
        }

    }
}
