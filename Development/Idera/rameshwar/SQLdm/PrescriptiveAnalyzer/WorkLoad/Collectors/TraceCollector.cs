using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
//using Idera.SQLdm.PrescriptiveAnalyzer.Cancel;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Stats;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Batches;
//using Idera.SQLdm.PrescriptiveAnalyzer.Engine;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.State;
using System.Management;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Collectors
{

    public class TraceCollector //: ICancelable
    {
        private static Logger _logX = Logger.GetLogger("TraceCollector");

        private TraceCollectorOptions _options = null;
        private TraceCollectorOptions _optionsAdjusted = null;
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

        private TraceCollector() { }

        public TraceCollector(EventWaitHandle completeEvent, TraceCollectorOptions options, SqlSystemObjectManager ssom) 
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
                    _thread = new Thread(ProcessTrace);
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

        private void ProcessTrace(SqlConnection conn)
        {
            int trace = 0;
            int sampleDuration;
            int processingDuration;
            TimeSpan ts;
            DateTime processingStarted = DateTime.MinValue;
            try
            {
                DeleteTrace(conn, _options);
                _endTime = DateTime.Now.AddMinutes(_options.TotalDurationMinutes);
                trace = CreateTrace(conn, _options);
                _optionsAdjusted = _options.Clone() as TraceCollectorOptions;
                sampleDuration = _optionsAdjusted.SampleDurationSeconds;
                _options.UpdateState(AnalysisStateType.ProfilerTraceCollection, "Collecting data...", 0, 100);
                //----------------------------------------------------------------------------
                // If the trace is not going to continuously run, just use the trace collection
                // logic for sampling instead of waiting in here.
                // 
                if (_options.ContinuousCollection)
                {
                    while (!_eventSignal.WaitOne(0, false))
                    {
                        LogStats();
                        CollectAndProcess(conn, ref trace, _optionsAdjusted);
                        //using (dt = CollectTraceData(conn, trace, _optionsAdjusted))
                        //{
                        //    ProcessTraceData(dt);
                        //}
                        if (_endTime <= DateTime.Now) break;
                    }
                }
                else
                {
                    while (!_eventSignal.WaitOne(TimeSpan.FromSeconds(sampleDuration), false))
                    {
                        LogStats();
                        processingStarted = DateTime.Now;
                        CollectAndProcess(conn, ref trace, _optionsAdjusted);
                        //using (dt = CollectTraceData(conn, trace, _optionsAdjusted))
                        //{
                        //    if (_endTime > DateTime.Now) AdjustTrace(conn, ref trace, _optionsAdjusted, dt);
                        //    ProcessTraceData(dt);
                        //}
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
            finally
            {
                _options.UpdateState(AnalysisStateType.ProfilerTraceCollection, "Completed profiler trace collection.", 100, 100);
                if (trace != 0)
                {
                    try
                    {
                        DeleteTrace(conn, _options);
                    }
                    catch (Exception exDel)
                    {
                        ExceptionLogger.Log(_logX, "DeleteTrace Exception: ", exDel);
                    }
                }
            }
        }

        private void CollectAndProcess(SqlConnection conn, ref int trace, TraceCollectorOptions options)
        {
            using (_logX.DebugCall(string.Format("{0} - CollectAndProcess", options.Name)))
            {
                string sqlInsert = options.ContinuousCollection ? "" : BuildSamplingSql(options);
                string sql = BatchConstants.CopyrightNotice + string.Format(Properties.Resources.CollectTraceData, trace, _eventSeq.ToString(), sqlInsert);
                SQLHelper.CheckConnection(conn);

                double remainingSeconds = (_endTime - DateTime.Now).TotalSeconds;
                int n = 100 - (int)((remainingSeconds / (_options.TotalDurationMinutes * 60)) * 100);
                if (n >= 100) n = 98;
                _options.UpdateState(AnalysisStateType.ProfilerTraceCollection, n, 100);

                try
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            ProcessTraceData(conn, r, ref trace, options);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "CollectAndProcess Exception: ", ex);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    TruncateTrace(conn, ref trace, options);
                    return;
                }
            }
        }

        private void ProcessTraceData(SqlConnection conn, SqlDataReader r, ref int trace, TraceCollectorOptions options)
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
                //----------------------------------------------------------------
                // If we want to continuously collect trace information and 
                // the trace status has been turned off, reset the trace.
                //
                if (options.ContinuousCollection)
                {
                    if (_endTime > DateTime.Now) AdjustTrace(conn, ref trace, _optionsAdjusted, rows);
                    if (r.NextResult())
                    {
                        if (r.Read()) TruncateTrace(conn, ref trace, options);
                    }
                }
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

        private void ProcessTrace()
        {
            try
            {
                Debug.Assert(null != _eventSignal, "Event was not created!");
                _rowsSinceLastTruncate = 0;
                using (SqlConnection conn = SQLHelper.GetConnection(_options.ConnectionInfo))
                {
                    //--------------------------------------------------------------------
                    // If we are not configured to use remote wmi, require that either ole
                    // automation is enabled or cmd shell is enabled to allow the trace 
                    // files to be deleted.
                    //
                    if (!_options.ConnectionInfo.UseRemoteWmi)
                    {
                        if (!SQLHelper.IsOleEnabled(conn) && !SQLHelper.IsCmdShellEnabled(conn))
                        {
                            _logX.Info("SQL Trace collection cannot be performed due to xp_cmdshell and Ole Automation not being enabled.");
                            throw new Exception("Workload collection could not be performed due to no xp_cmdshell or Ole Automation access.");
                        }
                    }
                    ProcessTrace(conn);
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

        private void ProcessTraceData(DataTable dt)
        {
            if (null == dt) return;
            if (null == dt.Rows) return;
            if (dt.Rows.Count <= 0) return;
            using (_logX.DebugCall(string.Format("ProcessTraceData({0} - RowCount: {1})", _options.Name, dt.Rows.Count.ToString())))
            {
                UInt64 seq = AddEvents(dt);
                if (_eventSeq < seq) _eventSeq = seq;
            }
        }

        private UInt64 AddEvents(DataTable dt)
        {
            if (null == dt) return (0);
            if (null == dt.Rows) return (0);
            if (dt.Rows.Count <= 0) return (0);
            TEBase te = null;
            UInt64 seq = 0;
            Queue<TEBase> q = new Queue<TEBase>(dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
            {
                switch (DataHelper.ToUInt64(row, "EventClass"))
                {
                    case (10): { te = new TERpcComplete(row); break; }
                    case (12): { te = new TEBatchComplete(row); break; }
                    case (43): { te = new TESpComplete(row); break; }
                    default: { te = null; break; }
                }
                if (null != te) { q.Enqueue(te); if (te.EventSequence > seq) { seq = te.EventSequence; } }
            }
            _traceParsingThread.AddEvents(q, _options.ContinuousCollection);
            return (seq);
        }

        private DataTable CollectTraceData(SqlConnection conn, int trace, TraceCollectorOptions options)
        {
            using (_logX.DebugCall(string.Format("{0} - CollectTraceData", options.Name)))
            {
                DataSet ds = null;
                string sqlInsert = options.ContinuousCollection ? "" : BuildSamplingSql(options);
                string sql = BatchConstants.CopyrightNotice + string.Format(Properties.Resources.CollectTraceData, trace, _eventSeq.ToString(), sqlInsert);
                SQLHelper.CheckConnection(conn);

                using (SqlDataAdapter adapter = new SqlDataAdapter(sql, conn))
                {
                    adapter.SelectCommand.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                    ds = new DataSet();
                    try
                    {
                        adapter.Fill(ds);
                        _totalSampleSeconds += options.SampleDurationSeconds;
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogger.Log(_logX, "CollectTraceData Exception: ", ex);
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        TruncateTrace(conn, ref trace, options);
                        return null;
                    }
                }
                if (null != ds)
                {
                    if (null != ds.Tables)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            //----------------------------------------------------------------
                            // If we want to continuously collect trace information and 
                            // the trace status has been turned off, reset the trace.
                            //
                            if ((ds.Tables.Count > 1) && (options.ContinuousCollection))
                            {
                                if (null != ds.Tables[1].Rows[0][0]) TruncateTrace(conn, ref trace, options);
                            }
                            return (ds.Tables[0]);
                        }
                    }
                }
                return (null);
            }
        }

        private string BuildSamplingSql(TraceCollectorOptions options)
        {
            if (options.ContinuousCollection) return string.Empty;
            TimeSpan ts = TimeSpan.FromSeconds(options.SampleDurationSeconds);
            string fmt = string.Format("{0}:{1}:{2}", ts.Hours.ToString("00"), ts.Minutes.ToString("00"), ts.Seconds.ToString("00"));
            return (BatchConstants.CopyrightNotice + string.Format(Properties.Resources.SampleTrace, fmt));
        }

        private void AdjustTrace(SqlConnection conn, ref int trace, TraceCollectorOptions options, int rowCount)
        {
            using (_logX.DebugCall(string.Format("AdjustTrace({0})", options.Name)))
            {
                try
                {
                    _rowsSinceLastTruncate += rowCount;
                    _logX.DebugFormat("{0} - RowCount:{1}  RowsSinceLastTruncate:{2}", options.Name, rowCount, _rowsSinceLastTruncate);
                    if (_rowsSinceLastTruncate > MAX_TRACE_FILE_ENTRIES) TruncateTrace(conn, ref trace, options);
                    if (rowCount < 10)
                    {
                        if (options.MinQueryTime > _options.MinQueryTime)
                        {
                            options.MinQueryTime -= (options.MinQueryTime - _options.MinQueryTime) / 2;
                            _logX.DebugFormat("{0} - Changing MingQueryTime={1}  RowCount:{2}", options.Name, options.MinQueryTime, rowCount);
                            TruncateTrace(conn, ref trace, options);
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
                        TruncateTrace(conn, ref trace, options);
                        _logX.DebugFormat("{0} - Adjusting MinQueryTime={1}  RowCount:{2}", options.Name, options.MinQueryTime, rowCount);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "AdjustTrace()  Exception: ", ex);
                }
            }
        }

        private void TruncateTrace(SqlConnection conn, ref int trace, TraceCollectorOptions options)
        {
            using (_logX.InfoCall(string.Format("{0} - TruncateTrace Min:{1} Max:{2}", options.Name, options.MinQueryTime, options.MaxQueryTime)))
            {
                _rowsSinceLastTruncate = 0;
                DeleteTrace(conn, options);
                trace = CreateTrace(conn, options);
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

        private static int CreateTrace(SqlConnection conn, TraceCollectorOptions options)
        {
            using (_logX.InfoCall(string.Format("CreateTrace({0})", options.Name)))
            {
                string sql = BatchConstants.CopyrightNotice + string.Format(Properties.Resources.CreateTrace, 
                                            options.Name, 
                                            options.TotalDurationMinutes, 
                                            options.MinQueryTime, 
                                            options.MaxQueryTime, 
                                            (options.ContinuousCollection ? "1" : "0"),
                                            CreateSafeString(options.FilterApplicationName),
                                            CreateSafeString(options.FilterDatabaseName));
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                    return ((int)command.ExecuteScalar());
                }
            }
        }

        private static object CreateSafeString(string p)
        {
            if (string.IsNullOrEmpty(p)) return "''";
            return (SQLHelper.CreateSafeString(p));
        }

        private static void DeleteTrace(SqlConnection conn, TraceCollectorOptions options)
        {
            using (_logX.InfoCall(string.Format("DeleteTrace({0})", options.Name)))
            {
                if (SQLHelper.IsCmdShellEnabled(conn))
                {
                    string sql = BatchConstants.CopyrightNotice + string.Format(Properties.Resources.DeleteTraceUsingCmdShell, options.Name);
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                        command.ExecuteNonQuery();
                    }
                }
                else if (!DeleteTraceUsingWmi(conn, options))
                {
                    string sql = BatchConstants.CopyrightNotice + string.Format(Properties.Resources.DeleteTrace, options.Name, Properties.Settings.Default.OLEContext.ToString());
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static string GetTraceFilename(SqlConnection conn, TraceCollectorOptions options)
        {
            try
            {
                string sql = BatchConstants.CopyrightNotice + string.Format(Properties.Resources.GetTraceFilename, options.Name);
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                    return (command.ExecuteScalar().ToString());
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(_logX, "GetTraceFilename() Exception: ", ex);
            }
            return (string.Empty);
        }

        private static bool DeleteTraceUsingWmi(SqlConnection conn, TraceCollectorOptions options)
        {
            using (_logX.InfoCall(string.Format("DeleteTraceUsingWmi({0})", options.Name)))
            {
                if (null == options) return (false);
                if (null == options.ConnectionInfo) return (false);
                if (!options.ConnectionInfo.UseRemoteWmi) return (false);
                try
                {
                    WmiConnectionInfo wmiInfo = (null == options.ConnectionInfo.WmiConnectionInfo) ? new WmiConnectionInfo() : options.ConnectionInfo.WmiConnectionInfo.Clone();
                    if (string.IsNullOrEmpty(wmiInfo.MachineName)) wmiInfo.MachineName = SQLHelper.GetMachineName(conn);
                    string filename = GetTraceFilename(conn, options);
                    if (string.IsNullOrEmpty(filename))
                    {
                        _logX.Debug("Trace filename not found!");
                        return (false);
                    }
                    _logX.Debug("Attempting to delete " + filename);
                    filename = filename.Replace("'", "''").Replace("\\", "\\\\");
                    ManagementScope scope = WmiHelper.GetManagementScopeCimV2(options.ConnectionInfo.WmiConnectionInfo);
                    ObjectQuery query = new ObjectQuery(string.Format("select * from CIM_DataFile where Name = '{0}'", filename));

                    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                    {
                        foreach (ManagementObject file in searcher.Get())
                        {
                            file.InvokeMethod("Delete", new object[] { });
                            _logX.Debug("Deleted " + filename);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "DeleteTraceUsingWmi() Exception: ", ex);
                }
                return (true);
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
        public TraceEventStatsCollection GetRankedResults()//(Progress progress)
        {
            return (_traceParsingThread.GetRankedResults());//(progress));
        }
        public void MergeResults(TraceCollector tc)
        {
            _traceParsingThread.MergeResults(tc._traceParsingThread);
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
