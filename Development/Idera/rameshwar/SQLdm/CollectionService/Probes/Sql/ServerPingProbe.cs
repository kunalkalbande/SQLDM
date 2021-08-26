//------------------------------------------------------------------------------
// <copyright file="ServerPingProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Diagnostics;
using System.Threading;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    using System;
    using System.Data.SqlClient;
    using BBS.TracerX;
    using Common.Configuration;
    using Common.Snapshots;
    using Common.Services;

    /// <summary>
    /// Server Ping Collector
    /// </summary>
    internal sealed class ServerPingProbe : SqlBaseProbe
    {
        #region fields

        private ScheduledRefresh serverPingSnapshot = null;
        private MonitoredServerWorkload workload = null;
        //private SqlConnection conn = null;
        private Exception innerException = null;
        private bool isCancelled;
        delegate void ConnectionWorkerHandler();
        protected RegisteredWaitHandle commandTimeoutMonitorHandle;
        Stopwatch sw = new Stopwatch();
        private Thread openCon;

        private int commandCompletedOrTimedout = 0;
        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerPingProbe"/> class.
        /// </summary>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public ServerPingProbe(MonitoredSqlServer monitoredServer, MonitoredServerWorkload workload, int? cloudProviderId)
            : base(monitoredServer.ConnectionInfo)
        {
            LOG = Logger.GetLogger("ServerPingProbe");
            serverPingSnapshot = new ScheduledRefresh(monitoredServer);
            this.workload = workload;
            this.cloudProviderId = cloudProviderId;
            connectionInfo.ConnectionTimeout = (int)workload.MonitoredServer.ServerPingInterval.TotalSeconds;
        }

        #endregion

        #region properties

        internal bool IsCancelled
        {
            get { return isCancelled; }
            set { isCancelled = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods


        /// <summary>
        /// Starts the probe, virtual method called by BaseProbe.BeginProbe().
        /// If this returns cleanly, probe must call FireCompletion()
        /// </summary>
        protected override void Start()
        {
            if (workload != null)
            {
                sw.Start();
                innerException = null;
                StartServerPingCollector();
            }
            else
            {
                FireCompletion(serverPingSnapshot, Result.Success);
            }
        }

        private void OpenConnectionThread()
        {

                if (IsCancelled)
                    return;

                openCon = new Thread(OpenConnectionThreadPrivate);
                openCon.Start();
                if (openCon.Join((connectionInfo.ConnectionTimeout++) * 1000))
                {
                    if (innerException != null)
                        throw innerException;
                    
                    serverPingSnapshot.Server.SqlServiceStatus = ServiceState.Running;
                }
                else
                {
                    if (innerException != null)
                        throw innerException;
                    throw new Exception("Timeout expired attempting to open server connection.");
                }

            
        }

        private void OpenConnectionThreadPrivate(object o)
        {
            try
            {
                if (connectionInfo.ConnectionTimeout > 1)
                    connectionInfo.ConnectionTimeout--;
                try
                {

                    using (SqlConnection connInner = connectionInfo.GetConnection())
                    {
                        connInner.Open();

                        SqlCommand cmd =
                            SqlCommandBuilder.BuildConnectionStatusCommand(connInner);
                        cmd.ExecuteScalar();
                    }
                }
                catch (SqlException)
                {
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(1));
                    using (SqlConnection connInner = connectionInfo.GetConnection())
                    {
                        connInner.Open();

                        SqlCommand cmd =
                            SqlCommandBuilder.BuildConnectionStatusCommand(connInner);
                        cmd.ExecuteScalar();
                    }
                }
            }
            catch (ThreadAbortException )
            {
               if (innerException == null)
                    innerException = new Exception("Timeout expired opening server connection.");
            }
            catch (Exception e)
            {
                innerException = e;
            }
        }

        public IAsyncResult StartServerPingCollector()
        {

            ConnectionWorkerHandler workerMethod = new ConnectionWorkerHandler(OpenPingConnection);
            IAsyncResult connectionResult =  workerMethod.BeginInvoke(OpenConnectionCallback,null);
               
            if (!connectionResult.IsCompleted)
                commandTimeoutMonitorHandle = ThreadPool.RegisterWaitForSingleObject(connectionResult.AsyncWaitHandle, ExecuteReaderCompleteOrTimedOut, this, workload.MonitoredServer.ServerPingInterval, true);
            return connectionResult;

        }

        public void OpenPingConnection()
         {

             try
             {
                 OpenConnectionThread();
             }
             catch (Exception e)
             {
                 if (!IsCancelled)
                 {
                     if (e is SqlException && ((SqlException) e).Number == 17142)
                     {
                         serverPingSnapshot.Server.SqlServiceStatus = ServiceState.Paused;
                         ProbeHelpers.LogAndAttachToSnapshot(serverPingSnapshot, LOG, "Monitored server is paused",
                                                             false);
                     }
                     else
                     {
                         serverPingSnapshot.Server.SqlServiceStatus = ServiceState.UnableToConnect;
                         ProbeHelpers.LogAndAttachToSnapshot(serverPingSnapshot,
                                                             LOG,
                                                             "Monitored server cannot be contacted: " + e.Message,
                                                             false);
                     }
                 }
                 else
                 {
                     LOG.Debug("Server Ping callback exception returned after timeout.",e);
                 }

             }
           
         }

        public void OpenConnectionCallback(IAsyncResult result)
        {
            try
            {
                sw.Stop();
                if (Interlocked.CompareExchange(ref commandCompletedOrTimedout, 1, 0) == 1)
                    return;

                FireCompletion(serverPingSnapshot, !IsCancelled ? Result.Success : Result.Failure);
            }
            catch (Exception e )
            {
                LOG.Error("Error in OpenConnectionCallback",e);
            }
            finally
            {
                UnregisterCommandTimeoutMonitor();

                
            }
        }


        private void ExecuteReaderCompleteOrTimedOut(object state, bool timedOut)
        {
            sw.Stop();
            if (!timedOut)
                return;

            if (Interlocked.CompareExchange(ref commandCompletedOrTimedout, 1, 0) == 1)
                return;

            serverPingSnapshot.Server.SqlServiceStatus = ServiceState.UnableToConnect;
            string errorMessage = String.Format("Server timed out in connection verification check after {0} ms.",
                                                sw.ElapsedMilliseconds);
            serverPingSnapshot.SetError(errorMessage, new Exception(errorMessage));

            //if (openCon != null)
            //    openCon.Abort();

            UnregisterCommandTimeoutMonitor();

       
            FireCompletion(serverPingSnapshot, Result.Success);
           
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

        #endregion

        #region interface implementations

        new public void Dispose()
        {
            
            UnregisterCommandTimeoutMonitor();

            
            base.Dispose();

        }

        #endregion
    }
}
