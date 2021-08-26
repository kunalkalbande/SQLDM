//------------------------------------------------------------------------------
// <copyright file="PeriodicCollectionContext.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Threading;
using BBS.TracerX;
using Idera.SQLdm.CollectionService.Probes;
using Idera.SQLdm.CollectionService.Probes.Sql;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.CollectionService.Monitoring
{
    partial class ScheduledCollectionManager
    {

        internal delegate SqlBaseProbe PeriodicProbe();
        internal delegate TimeSpan Interval();
        internal delegate void CallbackAction(Snapshot snapshot);

        internal class PeriodicCollectionContext<T> : IDisposable
            where T : Snapshot//, IContinuousSnapshot
        {

            private Timer periodicTimer;
            private SqlBaseProbe probe;
            private PeriodicProbe createProbe;
            private CallbackAction callbackAction;
            private bool periodicCollectionRunning = false;
            private MonitoredServerWorkload monitoredServerWorkload;
            private MonitoredServerWorkload pendingMonitoredServerWorkload;
            private static BBS.TracerX.Logger LOG;
            private volatile bool disposed = false;
            private ReaderWriterLock sync = new ReaderWriterLock();
            private string collectorName;
            private object syncRoot = new object();
            private Interval interval;
            private CollectNowHelper collectNowHelper;
            private Semaphore periodicCollectionSemaphore;
            private bool inPeriodicRefreshSnapshotCallback = false;
            private bool snapshotSent = false;
            

            public PeriodicCollectionContext(PeriodicProbe probe, CallbackAction callbackAction, Interval interval, MonitoredServerWorkload monitoredServerWorkload, string collectorName, CollectNowHelper collectNowHelper)
            {
                createProbe = probe;
                this.callbackAction = callbackAction;
                this.interval = interval;
                this.monitoredServerWorkload = monitoredServerWorkload;
                this.collectorName = collectorName;
                LOG = Logger.GetLogger("PeriodicCollector-" + collectorName + ":" + monitoredServerWorkload.Name);
                this.collectNowHelper = collectNowHelper;
                periodicCollectionSemaphore = new Semaphore(1, 1);
            }


            #region Database Statistics Periodic Refresh


            public void Start()
            {

                Start((int)interval().TotalSeconds);
            }

            public void Start(int initialDelaySeconds)
            {
                using (LOG.DebugCall("Start" + collectorName))
                {
                    LOG.Debug(String.Format("Starting periodic timer for {0}, delay is {1}",
                              monitoredServerWorkload.MonitoredServer.InstanceName, initialDelaySeconds));
                    lock (syncRoot)
                    {
                        Stop();
                        periodicCollectionRunning = true;
                    }

                    // Tolga K - to fix memory leak begins
                    if (periodicTimer != null)
                    {
                        periodicTimer.Dispose();
                    }
                    // Tolga K - to fix memory leak ends

                    periodicTimer = new Timer(new TimerCallback(TimerCallBack),
                                                        null,
                                                        initialDelaySeconds * 1000,
                                                        Timeout.Infinite);

                }
            }

            public void TimerCallBack(object state)
            {
                try
                {
                    ExecutePeriodicCollector(state);
                }
                catch (Exception ex)
                {
                    LOG.ErrorFormat("Error executing periodic collector: {0}", ex);
                    // Prevent race condition error
                    Thread.Sleep(5000);
                    StartNextCollector();
                }
                finally
                {
                    if (sync.IsReaderLockHeld)
                        sync.ReleaseReaderLock();
                }
            }

            public void Start(object state)
            {
                using (LOG.DebugCall("Start" + collectorName))
                {
                    LOG.Debug(String.Format("Starting periodic collector for {0} in response to CollectNow request",
                              monitoredServerWorkload.MonitoredServer.InstanceName));
                    lock (syncRoot)
                    {
                        periodicCollectionRunning = true;
                    }
                    ExecutePeriodicCollector(state);
                }
            }

            public void Stop()
            {
                if (periodicTimer != null)
                    periodicTimer.Dispose();
                periodicTimer = null;

                if (probe != null)
                    probe.Dispose();
                probe = null;
                
                periodicCollectionRunning = false;
            }

            public void Reconfigure(MonitoredServerWorkload workload)
            {
                pendingMonitoredServerWorkload = workload;
            }

            private void ExecutePeriodicCollector(object state)
            {
                if (disposed)
                    return;
                using (LOG.DebugCall("Collect" + collectorName))
                {
                    if (state is ISnapshotSink)
                    {
                        lock (syncRoot)
                        {
                            collectNowHelper.WaitingRequests.Add((ISnapshotSink) state);

                            // add to list of waiting on-demand requests
                            if (periodicCollectionSemaphore.WaitOne(0, false) == false)
                            {
                                // collection already in progress -  see if results have been sent 
                                if (!inPeriodicRefreshSnapshotCallback || (inPeriodicRefreshSnapshotCallback && !snapshotSent))
                                {
                                    return;
                                }
                            }
                        }
                    }
                    else if (periodicCollectionSemaphore.WaitOne(0, false) == false)
                    {
                        LOG.Debug(
                            "ExecutePeriodicCollector called but there is currently a refresh running.  Timer to next collection interval.");
                        // a periodic refresh is currently running so schedule so set the timer to try again later.
                        sync.AcquireReaderLock(-1);
                        Start(Convert.ToInt32(monitoredServerWorkload.NormalCollectionInterval.TotalSeconds));
                        sync.ReleaseReaderLock();
                        return;
                    }

                    if (pendingMonitoredServerWorkload != null)
                    {   // a new workload awaits.
                        sync.AcquireWriterLock(-1);
                        monitoredServerWorkload = pendingMonitoredServerWorkload;
                        pendingMonitoredServerWorkload = null;
                        sync.ReleaseWriterLock();
                    }

                    if (!ScheduledCollectionContext.ServerInMaintenanceMode(monitoredServerWorkload.MonitoredServer, LOG))
                    {
                        LOG.Debug("Starting periodic collection");
                        try
                        {

                            sync.AcquireReaderLock(-1);

                            probe = createProbe();

                            if (disposed)
                                return;

                            probe.BeginProbe(new EventHandler<SnapshotCompleteEventArgs>(PeriodicCollectorCallback));

                        }
                        catch (Exception exception)
                        {
                            LOG.WarnFormat("Error starting collector: {0}. Restarting in 5 seconds.", exception.Message);

                            if (probe != null)
                            {
                                probe.Dispose();
                                probe = null;
                            }

                            try
                            {
                                // release the semaphore and dispose of the probe
                                if (!disposed)
                                {
                                    periodicCollectionSemaphore.Release();
                                }
                            }
                            catch (Exception e)
                            {
                                LOG.WarnFormat("Error releasing periodic collection semaphore: {0}", exception.Message);
                            }

                            // Prevent race condition error
                            Thread.Sleep(5000);

                            StartNextCollector();

                        }
                        finally
                        {
                            if (sync.IsReaderLockHeld)
                                sync.ReleaseReaderLock();
                        }
                    }
                    else
                    {
                        LOG.Info("Periodic collector skipped because server is in maintenance mode.");
                        Stop();
                    }
                }
            }

            private void PeriodicCollectorCallback(object sender, SnapshotCompleteEventArgs e)
            {

                using (LOG.DebugCall(collectorName + "Callback"))
                {
                    inPeriodicRefreshSnapshotCallback = true;
                    snapshotSent = false;
                    try
                    {
                        using (sender as IDisposable)
                        {
                            if (disposed)
                                return;

                            lock (syncRoot)
                            {
                                if (probe != null)
                                {
                                    probe.Dispose();
                                    probe = null;
                                }
                            }

                            callbackAction(e.Snapshot);

                            // Only proceed if the server is not in maintenance mode.
                            if (monitoredServerWorkload.MonitoredServer.MaintenanceModeEnabled == false)
                            {
                                if (!e.Snapshot.CollectionFailed)
                                {
                                    if (e.Snapshot is AlertableSnapshot)
                                    {
                                        // acquire a read lock if we don't already hold one
                                        if (!sync.IsReaderLockHeld)
                                            sync.AcquireReaderLock(-1);
                                        try
                                        {
                                            // generate events

                                            ScheduledCollectionEventProcessor eventProcessor =
                                                new ScheduledCollectionEventProcessor(e.Snapshot as AlertableSnapshot, monitoredServerWorkload);

                                            eventProcessor.Process();

                                            int numberEvents = ((IEventContainer) e.Snapshot).NumberOfEvents;
                                            if (numberEvents > 0 && LOG.IsVerboseEnabled)
                                            {
                                                foreach (IEvent ievent in ((IEventContainer) e.Snapshot).Events)
                                                {
                                                    LOG.VerboseFormat("EventArgs: {0}", ievent);
                                                }
                                            }

                                            LOG.DebugFormat("Added {0} events to the snapshot", numberEvents);

                                            //SQLdm 10.0 (Sanjali Makkar) : Small Features : Updating '# Alerts Raised' counter
                                            Statistics.SetAlertsRaised(numberEvents, monitoredServerWorkload.Name,false);

                                        }
                                        catch (Exception ex)
                                        {
                                            LOG.Error("Exception while creating events", ex);
                                        }
                                        finally
                                        {
                                            if (sync.IsReaderLockHeld)
                                                sync.ReleaseReaderLock();
                                        }
                                    }

                                    //send the snapshot to the management service
                                    lock (syncRoot)
                                    {
                                        if (!disposed)
                                        {
                                            LOG.Debug("Sending database statistics to management service");
                                            ScheduledCollectionDataMessage message =
                                                new ScheduledCollectionDataMessage(monitoredServerWorkload.MonitoredServer, e.Snapshot);

                                            collectNowHelper.AddSink(message);

                                            Collection.Scheduled.EnqueueScheduledRefresh(message);
                                            snapshotSent = false;
                                        }
                                    }
                                }
                                else
                                {
                                    LOG.Error(String.Format("Collection failed for {0} : {1}", collectorName, e.Snapshot.Error));
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        LOG.Error(exception);
                        
                    }
                    finally
                    {
                        inPeriodicRefreshSnapshotCallback = false;
                        if (!disposed)
                        {
                            try
                            {
                                periodicCollectionSemaphore.Release();

                            }
                            catch (Exception exception)
                            {
                                LOG.Error(exception);
                            }
                        }
                        StartNextCollector();
                    }
                }
            }

            private void StartNextCollector()
            {
                Thread t = new Thread(NextCollector);
                t.Start();
            }

            private void NextCollector()
            {
                lock (syncRoot)
                {
                    // restart the timer to kick off the next collection cycle
                    sync.AcquireReaderLock(-1);

                    Start();

                    sync.ReleaseReaderLock();

                }
            }

            #endregion

            public void Dispose()
            {
                LOG.Debug("Disposing");
                Stop();
                periodicCollectionSemaphore.Close();
            }

        }
    }
}
