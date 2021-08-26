using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;

using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Batches;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.MissingIndexes
{
    class HypotheticalIndexCleanupThread
    {
        private static Logger _logX = Logger.GetLogger("HypotheticalIndexCleanupThread");

        private Queue<string> _queue = new Queue<string>(10);
        private Queue<string> _retryQueue = new Queue<string>(10);
        private object _lockQueue = new object();
        private SqlConnectionInfo _info = null;

        private object _lockThreadStartStop = new object();

        private Thread _thread = null;
        private EventWaitHandle _eventSignal = null;
        private volatile bool _stopProcessing = false;
        private volatile bool _join = false;

        private HypotheticalIndexCleanupThread() { }

        public HypotheticalIndexCleanupThread(SqlConnectionInfo info) 
        {
            _info = info;
        }

        public void Add(IEnumerable<string> tsql)
        {
            if (null == tsql) return;
            int added = 0;
            lock (_lockQueue)
            {
                foreach (string s in tsql)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        ++added;
                        _queue.Enqueue(s);
                    }
                }
            }
            if (added <= 0) return;
            lock (_lockThreadStartStop)
            {
                if (null == _thread)
                {
                    _thread = new Thread(this.ProcessThread);
                    _thread.Name = "HypotheticalIndexCleanupThread";
                    _thread.Priority = ThreadPriority.Normal;
                    _stopProcessing = false;
                    _join = false;
                }
                if (null == _eventSignal) _eventSignal = new EventWaitHandle(false, EventResetMode.ManualReset);
                _eventSignal.Set();
                if (!_thread.IsAlive) _thread.Start();
            }
        }

        public bool IsAlive
        {
            get
            {
                lock (_lockThreadStartStop)
                {
                    if (null != _thread) return (_thread.IsAlive);
                    return (false);
                }
            }
        }

        private void ProcessThread()
        {
            while (!_stopProcessing)
            {
                _eventSignal.WaitOne();
                _eventSignal.Reset();
                if (_stopProcessing) break;
                using (SqlConnection conn = SQLHelper.GetConnection(_info))
                {
                    while (ProcessQueue(conn)) ;
                }
                if (_join) break;
            }
        }

        private bool ProcessQueue(SqlConnection conn)
        {
            string tsql = string.Empty;
            string retrytsql = string.Empty;
            lock (_lockQueue)
            {
                if (_queue.Count > 0)
                {
                    retrytsql = _queue.Dequeue();
                    tsql = BatchConstants.CopyrightNotice + BatchConstants.BatchHeader + "set lock_timeout 1000;\n" + retrytsql;
                }
                else if (_retryQueue.Count > 0)
                {
                    retrytsql = _retryQueue.Dequeue();
                    tsql = BatchConstants.CopyrightNotice + BatchConstants.BatchHeader + retrytsql;
                }
            }
            if (!string.IsNullOrEmpty(tsql)) return (DeleteIndexes(conn, tsql, retrytsql));
            return (false);
        }

        private bool DeleteIndexes(SqlConnection conn, string tsql, string retrytsql)
        {
            if (string.IsNullOrEmpty(tsql)) return (false);
            try
            {
                using (SqlCommand command = new SqlCommand(tsql, conn))
                {
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(string.Format("HypotheticalIndexes.DropIndex({0}) Exception: ", tsql), ex);
                SQLHelper.CheckConnection(conn);
                SqlException sqlEx = ex as SqlException;
                if (null != sqlEx)
                {
                    //-----------------------------------------------------------------------
                    // Retry 'Lock request time out period exceeded.'
                    //
                    if (1222 == sqlEx.Number)
                    {
                        lock (_lockQueue)
                        {
                            _retryQueue.Enqueue(retrytsql);
                        }
                    }
                }
            }
            return (!_stopProcessing);
        }

        public void Join(TimeSpan maxWait)
        {
            using (_logX.DebugCall("HypotheticalIndexCleanupThread.Join"))
            {
                lock (_lockThreadStartStop)
                {
                    _join = true;
                    if (null != _eventSignal) _eventSignal.Set();
                    if (null != _thread)
                    {
                        try
                        {
                            if (_thread.IsAlive)
                            {
                                _logX.Debug("Thread is alive.  Join...");
                                if (!_thread.Join(maxWait))
                                {
                                    _stopProcessing = true;
                                    _logX.Debug("Thread join timed out.  Trying to stop thread...");
                                    _thread.Join();
                                    _logX.Debug("Thread stopped.");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionLogger.Log(_logX, "HypotheticalIndexCleanupThread.Join Exception: ", ex);
                        }
                        _thread = null;
                    }
                    _join = false;
                }
                lock (_lockQueue)
                {
                    _logX.DebugFormat("There are {0} hypothetical indexes that still need to be deleted!", _queue.Count+_retryQueue.Count);
                }

            }
        }

    }
}
