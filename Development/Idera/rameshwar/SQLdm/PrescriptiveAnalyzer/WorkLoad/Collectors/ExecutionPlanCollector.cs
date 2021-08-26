using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Cache;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Collectors
{
    class ExecutionPlanCollector
    {
        private static Logger _logX = Logger.GetLogger("ExecutionPlanCollector");

        private Queue<TEBase> _queue = new Queue<TEBase>(10);
        private object _lockQueue = new object();

        private Dictionary<string, TraceEventPlan> _plans = new Dictionary<string, TraceEventPlan>();
        private object _lockPlans = new object();

        private SqlConnectionInfo _connInfo = null;
        private SqlConnection _conn = null;

        private SqlDbNameManager _dbNames = new SqlDbNameManager();

        private object _lockThreadStartStop = new object();
        private Thread _thread = null;
        private EventWaitHandle _eventSignal = null;
        private volatile bool _stopProcessing = false;
        private volatile bool _join = false;
        private PlanCache _planCache = null;

        private ExecutionPlanCollector() { }
        public ExecutionPlanCollector(SqlConnectionInfo info, PlanCache planCache) 
        {
            _planCache = planCache;
            _connInfo = info.Clone();
        }

        public void AddQuery(TEBase te)
        {
            lock (_lockQueue)
            {
                _queue.Enqueue(te);
            }
            lock (_lockThreadStartStop)
            {
                if (null == _thread)
                {
                    _thread = new Thread(this.ProcessThread);
                    _thread.Name = "ExecutionPlanCollector";
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
            if (null == _connInfo)
            {
                Debug.Assert(false, "SqlConnectionInfo required!");
                return;
            }
            using (_conn = SQLHelper.GetConnection(_connInfo))
            {
                while (!_stopProcessing)
                {
                    _eventSignal.WaitOne();
                    _eventSignal.Reset();
                    if (_stopProcessing) break;
                    while (ProcessQueue()) ;
                    if (_join) break;
                }
            }
            _conn = null;
        }

        private bool ProcessQueue()
        {
            TEBase query = null;
            lock (_lockQueue)
            {
                if (_queue.Count > 0) query = _queue.Dequeue();
            }
            if (null != query) return (ProcessQuery(query));
            return (false);
        }

        private bool ProcessQuery(TEBase query)
        {
            lock (_lockPlans)
            {
                try
                {
                    SQLHelper.CheckConnection(_conn);
                    _dbNames.UpdateDatabaseName(_conn, query.DBID);
                    if (!_plans.ContainsKey(query.TextNormalized))
                    {
                        _plans.Add(query.TextNormalized, new TraceEventPlan(_conn, query, _planCache));
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "ProcessQuery Exception: ", ex);
                }
            }
            return (!_stopProcessing);
        }

        public void Stop() { _stopProcessing = true; Join(); }
        public void Join()
        {
            lock (_lockThreadStartStop)
            {
                _join = true;
                if (null != _eventSignal) _eventSignal.Set();
                if (null != _thread)
                {
                    if (_thread.IsAlive) _thread.Join();
                    _thread = null;
                }
                _join = false;
            }
            lock (_lockQueue) { _queue.Clear(); }
        }

        internal TraceEventPlan GetPlan(string text)
        {
            TraceEventPlan plan = null;
            lock (_lockPlans)
            {
                _plans.TryGetValue(text, out plan);
            }
            return (plan);
        }

        internal bool RemovePlan(string text)
        {
            lock (_lockQueue)
            {
                if (_queue.Count > 0)
                {
                    Queue<TEBase> q = new Queue<TEBase>(_queue);
                    _queue.Clear();
                    foreach (TEBase te in q)
                    {
                        if (0 != string.Compare(te.TextNormalized, text)) { _queue.Enqueue(te); }
                    }
                }
            }
            lock (_lockPlans)
            {
                return (_plans.Remove(text));
            }
        }

        internal void Merge(ExecutionPlanCollector epc)
        {
            using (_logX.InfoCall("ExecutionPlanCollector.MergeResults()"))
            {
                TraceEventPlan plan = null;
                int added = 0;
                lock (_lockPlans)
                {
                    lock (epc._lockPlans)
                    {
                        foreach (var kv in epc._plans)
                        {
                            if (!_plans.TryGetValue(kv.Key, out plan))
                            {
                                _plans.Add(kv.Key, kv.Value);
                                ++added;
                            }
                        }
                    }
                }
                _logX.InfoFormat("TraceEventPlans Added:{0}", added);
            }
        }
    }
}
