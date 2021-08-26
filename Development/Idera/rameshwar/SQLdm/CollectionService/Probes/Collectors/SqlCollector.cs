//------------------------------------------------------------------------------
// <copyright file="SqlBaseBatch.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Idera.SQLdm.CollectionService.Probes.Collectors
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading;
    using Idera.SQLdm.Common.Services;
    using System.Diagnostics;
    using Microsoft.ApplicationBlocks.Data;
    using Idera.SQLdm.CollectionService.Monitoring;

    /// <summary>
    /// Enter a description for this class
    /// </summary>
    class SqlCollector : BaseCollector
    {
        #region fields

        private SqlCommand command;
        private bool ownsConnection;
        private Stopwatch stopwatch = new Stopwatch();

        // max time to wait for async query to process on the server
        protected long maxWaitTimeMs = 0;
        protected RegisteredWaitHandle commandTimeoutMonitorHandle;
        protected int commandCompletedOrTimedout = 0;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SqlCollector"/> class that does not own the SqlConnection
        /// </summary>
        /// <param name="command">The command.</param>
        public SqlCollector(SqlCommand command)  : this(command, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SqlCollector"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="ownsConnection">if set to <c>true</c> [owns connection].</param>
        public SqlCollector(SqlCommand command, bool ownsConnection)
            : base()
        {
            if (command == null)
                throw new ArgumentNullException("command");

            Command = command;
            this.ownsConnection = ownsConnection;
        }

        #endregion

        #region properties

        protected SqlCommand Command
        {
            get { return command; }
            private set
            {
                command = value;
                if (command != null)
                {
                    int tos = command.CommandTimeout;
                    if (tos == 0)
                        tos = SqlHelper.CommandTimeout;
                    maxWaitTimeMs = tos*1000;
                }
            }
        }

        internal string SqlText
        {
            get {
                if (command != null)
                {
                    return command.CommandText;
                }
                else
                {
                    return null;
                }
            }
        }

        internal long MaxQueryProcessingTime
        {
            get { return maxWaitTimeMs;  }
            set { maxWaitTimeMs = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        public string GetDatabase()
        {
            try
            {
                return command.Connection.Database;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }
        }
        #region ICollector Members

        /// <summary>
        /// Begins the collection.
        /// </summary>
        /// <param name="collectionCompleteCallback">The collection complete callback.</param>
        /// <returns></returns>
        public override IAsyncResult BeginCollection(EventHandler<CollectorCompleteEventArgs> collectionCompleteCallback)
        {
            if (collectionCompleteCallback == null)
                throw new ArgumentNullException("collectionCompleteCallback");

            this.collectionCompleteCallback = collectionCompleteCallback;

            if (AsyncWaitHandle != null)
                throw new Exception("Attempt to call BeginCollection() twice on one instance");

            asyncWaitHandle = new ManualResetEvent(false);
            commandCompletedOrTimedout = 0;

            stopwatch.Start();
            IAsyncResult queryResult = Command.BeginExecuteReader(new AsyncCallback(CommandCompleteCallback), this);

            if (maxWaitTimeMs > 0 && !queryResult.IsCompleted)
                commandTimeoutMonitorHandle = ThreadPool.RegisterWaitForSingleObject(queryResult.AsyncWaitHandle, ExecuteReaderCompleteOrTimedOut, this, maxWaitTimeMs, true);

            return queryResult;
        }

        /// <summary>
        /// Commands the complete callback.
        /// </summary>
        /// <param name="ar">The ar.</param>
        private void CommandCompleteCallback(IAsyncResult ar)
        {
            if (Interlocked.CompareExchange(ref commandCompletedOrTimedout, 1, 0) == 1)
                return;

            stopwatch.Stop();

            UnregisterCommandTimeoutMonitor();

            try
            {
                SqlDataReader rd = Command.EndExecuteReader(ar);
                //if(rd.HasRows)
                collectionCompleteCallback(this, new CollectorCompleteEventArgs(rd, stopwatch.ElapsedMilliseconds > 0 ? stopwatch.ElapsedMilliseconds : 0, Result.Success));

                //SQLdm 10.0 : Small Features : Updating counter 'Collection Run Time'
                //Statistics.collectionRunTimeInSeconds += Convert.ToInt64(stopwatch.ElapsedMilliseconds > 0 ? Convert.ToDouble(stopwatch.ElapsedMilliseconds) / 0.001 : 0);
            }
            catch (Exception exception)
            {
                // Ensure that the callback gets called even on an exception.
                collectionCompleteCallback(this, new CollectorCompleteEventArgs(null, exception));
            }
            finally
            {
                stopwatch.Reset();
            }
        }

        #endregion

        private void ExecuteReaderCompleteOrTimedOut(object state, bool timedOut)
        {
            if (!timedOut)
                return;

            if (Interlocked.CompareExchange(ref commandCompletedOrTimedout, 1, 0) == 1)
                return;

            try
            {   // issue a cancel on the command - docs claim it has no effect on async commands
                Command.Cancel();
            } catch
            {
            }

            try
            {
                stopwatch.Stop();
                TimeoutException exception = new TimeoutException("SQL operation timed out prior to completion on the server.");
                if (collectionCompleteCallback != null)
                {
                    collectionCompleteCallback(this, new CollectorCompleteEventArgs(null, exception));
                }
                if (collectionNonQueryExecutionCompleteCallback != null)
                {
                    collectionNonQueryExecutionCompleteCallback(this, new CollectorCompleteEventArgs(null, exception));
                }
            }
            finally
            {
                stopwatch.Reset();
            }
        }

        private void UnregisterCommandTimeoutMonitor()
        {
            if (commandTimeoutMonitorHandle != null)
            {
                try
                {
                    commandTimeoutMonitorHandle.Unregister(null);
                }
                catch
                {
                }
            }
        }

        //START : SQLDM 10.3 (Manali Hukkeri): Technical debt changes
        /// <summary>
        /// Begins the non query
        /// </summary>
        /// <param name="collectionNonQueryExecutionCompleteCallback">The collection non query execution completion callback.</param>
        /// <returns></returns>
        public override IAsyncResult BeginCollectionNonQueryExecution(EventHandler<CollectorCompleteEventArgs> collectionNonQueryExecutionCompleteCallback)
        {
            IAsyncResult queryResult = null;

            if (collectionNonQueryExecutionCompleteCallback == null)
                throw new ArgumentNullException("collectionNonQueryExecutionCompleteCallback");

            this.collectionNonQueryExecutionCompleteCallback = collectionNonQueryExecutionCompleteCallback;
            
            if (AsyncWaitHandle != null)
                    throw new Exception("Attempt to call BeginCollectionNonQueryExecution() twice on one instance");

                asyncWaitHandle = new ManualResetEvent(false);
                commandCompletedOrTimedout = 0;

                stopwatch.Start();
            if (Command != null && Command.CommandText != string.Empty)
            {
                queryResult = Command.BeginExecuteNonQuery(new AsyncCallback(CommandNonQueryCompleteCallback), this);
                if (maxWaitTimeMs > 0 && !queryResult.IsCompleted)
                    commandTimeoutMonitorHandle = ThreadPool.RegisterWaitForSingleObject(queryResult.AsyncWaitHandle, ExecuteReaderCompleteOrTimedOut, this, maxWaitTimeMs, true);
            }
            else
            {
                CommandNonQueryCompleteCallback(queryResult);
            }

            return queryResult;
        }

        /// <summary>
        /// Commands the complete callback.
        /// </summary>
        /// <param name="ar">The ar.</param>
        private void CommandNonQueryCompleteCallback(IAsyncResult ar)
        {
            int rd = -1;
            if (Interlocked.CompareExchange(ref commandCompletedOrTimedout, 1, 0) == 1)
                return;

            stopwatch.Stop();

            UnregisterCommandTimeoutMonitor();

            try
            {
                if (ar != null)
                {
                    rd = Command.EndExecuteNonQuery(ar);
                }
                collectionNonQueryExecutionCompleteCallback(this, new CollectorCompleteEventArgs(rd, stopwatch.ElapsedMilliseconds > 0 ? stopwatch.ElapsedMilliseconds : 0, Result.Success));

                //SQLdm 10.0 : Small Features : Updating counter 'Collection Run Time'
                //Statistics.collectionRunTimeInSeconds += Convert.ToInt64(stopwatch.ElapsedMilliseconds > 0 ? Convert.ToDouble(stopwatch.ElapsedMilliseconds) / 0.001 : 0);
            }

            catch (Exception exception)
            {
                // Ensure that the callback gets called even on an exception.
                collectionNonQueryExecutionCompleteCallback(this, new CollectorCompleteEventArgs(null, exception));
            }
            finally
            {
                stopwatch.Reset();
            }
        }

        //END : SQLDM 10.3 (Manali Hukkeri): Technical debt changes

        #region IDisposable Members

        public override void Dispose()
        {
            base.Dispose();

            UnregisterCommandTimeoutMonitor();

            if (Command != null)
            {
                SqlConnection conn = Command.Connection;

                try
                {
                    Command.Dispose();
                }
                catch
                {
                }
                if (ownsConnection)
                {
                    try
                    {
                        conn.Close();
                        conn.Dispose();
                    }
                    catch
                    {
                    }
                }
            }

            if (collectionCompleteCallback != null)
            {
                // Tolga K - break reference from event handler to avoid memory leaks
                collectionCompleteCallback = delegate { };
            }
            if (collectionNonQueryExecutionCompleteCallback != null)
            {
                collectionNonQueryExecutionCompleteCallback = delegate { };
            }
        }

        #endregion

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
