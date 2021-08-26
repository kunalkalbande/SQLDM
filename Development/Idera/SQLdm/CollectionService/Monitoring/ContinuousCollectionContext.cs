//------------------------------------------------------------------------------
// <copyright file="ContinuousCollectionContext.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Threading;
using BBS.TracerX;
using Idera.SQLdm.CollectionService.Probes;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.CollectionService.Monitoring
{
    partial class ScheduledCollectionManager
    {
        protected class ContinuousCollectionWaiter<T>
        {
            private IContinuousConfiguration configuration;
            private T snapshot;

            public ContinuousCollectionWaiter(IContinuousConfiguration configuration)
            {
                this.configuration = configuration;
            }

            public IContinuousConfiguration Configuration
            {
                get { return configuration; }
                set { configuration = value; }
            }

            public T Snapshot
            {
                get { return snapshot; }
                set { snapshot = value; }
            }
        }

        protected class ContinuousCollectionContext<T> : IDisposable
            where T : Snapshot, IContinuousSnapshot

        {
            private Dictionary<Guid, ContinuousCollectionWaiter<T>> waitingRequests =
                new Dictionary<Guid, ContinuousCollectionWaiter<T>>();

            private SqlConnectionInfo connectionInfo;
            private Logger LOG;
            private Semaphore continuousCollectionSemaphore;
            private ReaderWriterLock sync = new ReaderWriterLock();
            private bool inCollector = false;
            private Timer cleanupTimer;
            private ContinuousCollectorRunStatus runStatus = ContinuousCollectorRunStatus.Created;
            // SQLdm 10.3 (Varun Chopra) Linux Support for Continous Collector 
            private int? cloudProviderId;
            public ContinuousCollectionContext(IContinuousConfiguration config, SqlConnectionInfo connectionInfo,
                                               string MonitoredServerName, int? cloudProviderId)
            {
                LOG =
                    Logger.GetLogger("ContinuousCollectionContext:" +
                                     typeof (T).ToString().Replace("Idera.SQLdm.Common.Snapshots.", "").Replace(
                                         "Snapshot", "") + ":" + MonitoredServerName);
                LOG.Info("Continuous collector created by ClientSessionId " + config.ClientSessionId);
                continuousCollectionSemaphore = new Semaphore(1, 1);
                this.connectionInfo = connectionInfo;
                this.cloudProviderId = cloudProviderId;
                // Tolga K - to fix memory leak begins
                if (cleanupTimer != null)
                {
                    cleanupTimer.Dispose();
                }
                // Tolga K - to fix memory leak ends

                cleanupTimer = new Timer(new TimerCallback(CleanupWaiters),
                                         null,
                                         60000,
                                         Timeout.Infinite);
                if (config != null)
                {
                    //save the master config template
                    //MasterConfig = config;
                    ContinuousCollectionWaiter<T> ccw = new ContinuousCollectionWaiter<T>(config);
                    AddWaiter(ccw);
                }
            }

            /// <summary>
            /// All waiters should observe the same rules. The MasterConfig is the template for properties that cannot be combined such as the filters.
            /// </summary>
            //public IContinuousConfiguration MasterConfig { get; set; }

			/// SQLdm Minimum Privileges (Varun Chopra) - Permissions Init
            public MinimumPermissions MinimumPermissions { get; set; }
            public MetadataPermissions MetadataPermissions { get; set; }
            public CollectionPermissions CollectionPermissions { get; set; }

            public ContinuousCollectorRunStatus RunStatus
            {
                get { return runStatus; }
            }

            public bool ReadyToDispose
            {
                get { return runStatus == ContinuousCollectorRunStatus.ReadyToDispose; }
            }
            /// <summary>
            /// Adds a new waiter (has a configuration and a snapshot)
            /// </summary>
            /// <param name="waiter">ContinuousCollectionWaiter has a snapshot and a config</param>
            public void AddWaiter(ContinuousCollectionWaiter<T> waiter)
            {
                using (LOG.DebugCall("AddWaiter"))
                {
                    try
                    {
                        if (waiter == null)
                            return;

                        sync.AcquireWriterLock(-1);
                        
                        //if this sessions does not already have a waiting request
                        if (!waitingRequests.ContainsKey(waiter.Configuration.ClientSessionId))
                        {
                            LOG.Debug(string.Format("New waitingRequest is being added ({0})", waiter.Configuration.ClientSessionId));
                            
                            //go through the existing waiters, looking for snapshots that have data we can pre-populate with
                            foreach (ContinuousCollectionWaiter<T> thisWaiter in waitingRequests.Values)
                            {
                                //if this is a pre-existign waiter of the same type then it can only have more data in its snapshot than thisWaiter
                                if (thisWaiter.Snapshot == null) continue;

                                if (typeof (T) != thisWaiter.Snapshot.GetType()) continue;

                                object snapshot = thisWaiter.Snapshot;
                                if (!(snapshot is ActiveWaitsSnapshot)) continue;

                                LOG.Debug(string.Format("WaitingRequest ({0}) is an ActiveWaits Waiting Request",
                                                  waiter.Configuration.ClientSessionId));

                                if (((ActiveWaitsSnapshot) snapshot).ActiveWaits.Rows.Count <= 0) continue;

                                LOG.Debug(
                                    string.Format(
                                        "An existing WaitingRequest ({0}) has {1} rows. Using it for initialization of new request.",
                                        thisWaiter.Configuration.ClientSessionId,
                                        ((ActiveWaitsSnapshot) snapshot).ActiveWaits.Rows.Count));

                                waiter.Snapshot = thisWaiter.Snapshot;
                                break;
                            }

                            //MasterConfig = waiter.Configuration;
                            waitingRequests.Add(waiter.Configuration.ClientSessionId, waiter);
                            
                            LOG.Verbose("Adding continuous waiter for ClientSessionId " +
                                        waiter.Configuration.ClientSessionId);
                        }
                        else //if this session does already have a waiting request but the config has changed
                        {
                            if (!waitingRequests[waiter.Configuration.ClientSessionId].Configuration.Equals(
                                     waiter.Configuration))
                            {
                                //save the snapshot from the waiting session, replace the waiter (just the config really) and copy the snapshot back
                                T snapshot = waitingRequests[waiter.Configuration.ClientSessionId].Snapshot;
                                waitingRequests[waiter.Configuration.ClientSessionId] = waiter;
                                waitingRequests[waiter.Configuration.ClientSessionId].Snapshot = snapshot;
                                //MasterConfig = waiter.Configuration;
                            }
                        }
                        sync.ReleaseWriterLock();

                        StartCollector();
                    }
                    catch (Exception e)
                    {
                        LOG.Error("An error occurred while adding a waiter for ClientSessionId " +
                                  waiter != null
                                      ? (waiter.Configuration != null
                                             ? waiter.Configuration.ClientSessionId.ToString()
                                             : "(null config)")
                                      : "(null waiter)",
                                  e);
                    }
                    finally
                    {
                        if (sync.IsWriterLockHeld)
                            sync.ReleaseWriterLock();
                    }
                }
            }

            public void RemoveWaiter(ContinuousCollectionWaiter<T> waiter)
            {
                using (LOG.DebugCall("RemoveWaiter"))
                {
                    try
                    {
                        if (waiter == null)
                            return;

                        sync.AcquireWriterLock(-1);
                        if (waitingRequests.ContainsKey(waiter.Configuration.ClientSessionId))
                        {
                            waitingRequests.Remove(waiter.Configuration.ClientSessionId);
                            LOG.Debug("Removing continuous waiter for ClientSessionId " +
                                      waiter.Configuration.ClientSessionId);
                        }
                        sync.ReleaseWriterLock();

                        CleanupWaiters(null);
                    }
                    catch (Exception e)
                    {
                        LOG.Error("An error occurred while removing a waiter for ClientSessionId " +
                                  waiter != null
                                      ? (waiter.Configuration != null
                                             ? waiter.Configuration.ClientSessionId.ToString()
                                             : "(null config)")
                                      : "(null waiter)",
                                  e);
                    }
                    finally
                    {
                        if (sync.IsWriterLockHeld)
                            sync.ReleaseWriterLock();
                    }
                }
            }

            public void RemoveAllWaiters()
            {
                using (LOG.DebugCall("RemoveWaiter"))
                {
                    try
                    {
                        LOG.Info("Removing all waiters.");
                        sync.AcquireWriterLock(-1);
                        waitingRequests.Clear();
                        runStatus = ContinuousCollectorRunStatus.ReadyToDispose;
                        sync.ReleaseWriterLock();
                    }
                    catch (Exception e)
                    {
                        LOG.Error("An error occurred while removing all waiters", e);
                    }
                    finally
                    {
                        if (sync.IsWriterLockHeld)
                            sync.ReleaseWriterLock();
                    }
                }
            }

            public T GetData(IContinuousConfiguration config)
            {
                using (LOG.DebugCall("GetData"))
                {
                    try
                    {
                        if (config == null)
                            return null;

                        sync.AcquireReaderLock(-1);
                        // If the waiter is already registered, look for ready data
                        if (waitingRequests.ContainsKey(config.ClientSessionId) &&
                            waitingRequests[config.ClientSessionId] != null)
                        {
                            // Update config object if it has changed
                            if (!Equals(waitingRequests[config.ClientSessionId].Configuration, config))
                            {
                                //MasterConfig = config;
                                waitingRequests[config.ClientSessionId].Configuration = config;
                                //waitingRequests[config.ClientSessionId].Configuration = config;
                                //if (configuration != null)
                                //{
                                //    if (waitStatisticsCollectors.ContainsKey(configuration.MonitoredServerId))
                                //    {
                                //        waitStatisticsCollectors[configuration.MonitoredServerId].RemoveWaiter(
                                //            new ContinuousCollectionWaiter<ActiveWaitsSnapshot>(configuration));
                                //        CleanupActiveWaits(configuration.MonitoredServerId);
                                //    }
                                //}
                            }

                            var returnData = waitingRequests[config.ClientSessionId].Snapshot;
                            sync.UpgradeToWriterLock(-1);
                            waitingRequests[config.ClientSessionId].Snapshot = null;
                            sync.ReleaseLock();
                            if (runStatus > ContinuousCollectorRunStatus.Running)
                            {
                                runStatus = ContinuousCollectorRunStatus.Created;
                                StartCollector();
                            }
                            return returnData;
                        }
                        else
                        {
                            // Otherwise register the waiter and return no data for now
                            if (config.ReadyForCollection)
                            {
                                ContinuousCollectionWaiter<T> ccw = new ContinuousCollectionWaiter<T>(config);
                                sync.ReleaseReaderLock();
                                AddWaiter(ccw);

                                object returnSnapshot = ccw.Snapshot;
                                
                                if(returnSnapshot is ActiveWaitsSnapshot)
                                {
                                    if(((ActiveWaitsSnapshot)returnSnapshot).ActiveWaits.Rows.Count > 0)
                                    {
                                        return ccw.Snapshot;
                                    }
                                }
                                
                            }
                            return null;
                        }
                    }
                    catch (Exception e)
                    {
                        LOG.Error(
                            "An error occurred when retrieving continuous collector data for ClientSessionId " + config !=
                            null
                                ? config.ClientSessionId.ToString()
                                : "(null)",
                            e);
                        return null;
                    }
                    finally
                    {
                        if (sync.IsReaderLockHeld)
                            sync.ReleaseReaderLock();
                    }
                }
            }

            /// <summary>
            /// Starts the Continuous Collector
            /// </summary>
            /// <param name="reTriggered">
            /// Defaults to false, 
            /// set to true when called from it's own callback, to indicate re-triggering occured
            /// SQLDM-29459 (SQLdm 10.3) SQLdmCollectionService.exe causing very high CPU & network usage
            /// </param>
            private void StartCollector(bool reTriggered = false)
            {
                if (IsDisposed)
                    return;

                using (LOG.DebugCall("StartCollector"))
                {
                    try
                    {
                        if (continuousCollectionSemaphore.WaitOne(0, false) == false)
                        {
                            LOG.Verbose("StartCollector was called while a collector was already running.");
                            return;
                        }

                        if (inCollector)
                            return;
                        sync.AcquireWriterLock(-1);
                        inCollector = true;
                        sync.ReleaseWriterLock();
                        OnDemandConfiguration config = (OnDemandConfiguration) CombineConfiguration();
                        //OnDemandConfiguration config = (OnDemandConfiguration) MasterConfig;

                        // SQLDM-29459 (SQLdm 10.3) SQLdmCollectionService.exe causing very high CPU & network usage
                        // enableProbe variable defaults to true and only set to false for Active Waits enabled with extended events
                        // When set to false it prevents continuous collection into basic collection
                        var enableProbe = true;

                        // When reTriggered set and is Active Waits Collector
                        // Check and Disable re triggering only for Active Waits Probe with extended events enabled
                        if (reTriggered && config!= null && config.GetType() == typeof(ActiveWaitsConfiguration))
                        {
                            var activeWaitsConfiguration = (ActiveWaitsConfiguration)config;
                            // Disable re triggering only for Active Waits Probe with extended events enabled
                            // SQLDM-29580 (Varun Chopra) - High CPU Usage on the monitored instance when collecting query-level waits with Query Store
                            enableProbe = !(activeWaitsConfiguration.Enabled && (activeWaitsConfiguration.EnabledXe || activeWaitsConfiguration.EnabledQs));
                        }

                        // enableProbe variable defaults to true and only set to false for Active Waits enabled with extended events
                        // When set to false it prevents continuous collection into basic collection
                        if (config != null && enableProbe)
                        {
                            // SQLdm 10.3 (Varun Chopra) Linux Support
                            IProbe probe = ProbeFactory.BuildContinuousProbe(connectionInfo, config, cloudProviderId);
                            if (probe != null)
                            {
                                LOG.Debug(
                                    String.Format("Beginning continuous probe.  Waiter Count: {0}",
                                                  waitingRequests.Count));
                                runStatus = ContinuousCollectorRunStatus.Running;
                                probe.BeginProbe(ContinuousCollectorCallback);
                            }
                            else
                            {
                                runStatus = ContinuousCollectorRunStatus.Stopped;
                                LOG.Info(
                                    "No configuration is ready for collection for this continuous probe.  Ending collection.");
                                if (!IsDisposed)
                                {
                                    continuousCollectionSemaphore.Release();
                                }
                                inCollector = false;
                            }
                        }
                        else
                        {
                            LOG.Info(
                                "No configuration is ready for collection for this continuous probe.  Ending collection.");
                            if (!IsDisposed)
                            {
                                continuousCollectionSemaphore.Release();
                            }
                            //sqldm-29671 start
                            if (!enableProbe)
                            {
                                runStatus = ContinuousCollectorRunStatus.Running;
                            }
                            //sqldm-29671 end
                            else
                            {
                                runStatus = ContinuousCollectorRunStatus.Stopped;
                            }
                            inCollector = false;
                        }
                    }
                    catch (Exception e)
                    {
                        LOG.Error("An error occurred when starting the continuous collector.", e);

                        try
                        {
                            if (!IsDisposed)
                            {
                                continuousCollectionSemaphore.Release();
                            }
                        }
                        catch (Exception ex)
                        {
                            LOG.WarnFormat("Error releasing continuous collection semaphore: {0}", ex.Message);
                            return;
                        }
                        runStatus = ContinuousCollectorRunStatus.Stopped;
                        inCollector = false;
                    }
                    finally
                    {
                        if (sync.IsReaderLockHeld)
                            sync.ReleaseReaderLock();
                    }
                }
            }

            /// <summary>
            /// Run the fastest and least restrictive configuration of those submitted
            /// </summary>
            private IContinuousConfiguration CombineConfiguration()
            {
                using (LOG.DebugCall("CombineConfiguration"))
                {
                    try
                    {
                        sync.AcquireReaderLock(-1);
                        IContinuousConfiguration returnConfig = null;
                        if (waitingRequests.Count > 0)
                        {
                            Dictionary<Guid, ContinuousCollectionWaiter<T>>.Enumerator e =
                                waitingRequests.GetEnumerator();

                            List<IContinuousConfiguration> configList = new List<IContinuousConfiguration>();
                            while (e.MoveNext())
                            {
                                if (e.Current.Value != null && e.Current.Value.Configuration != null)
                                {
                                    if (returnConfig == null)
                                    {
                                        returnConfig = e.Current.Value.Configuration;
                                    }
                                    configList.Add(e.Current.Value.Configuration);
                                }
                            }
                            if (returnConfig != null)
                            {
                                returnConfig.CombineConfiguration(configList);
                            }
                        }
                        sync.ReleaseReaderLock();
                        if (returnConfig != null && returnConfig.ReadyForCollection)
                            return returnConfig;
                        else
                        {
                            return null;
                        }
                    }
                    catch (Exception e)
                    {
                        LOG.Error("An error occurred when combining configurations.", e);
                        return null;
                    }
                    finally
                    {
                        if (sync.IsReaderLockHeld)
                            sync.ReleaseReaderLock();
                    }
                }
            }

            private void ContinuousCollectorCallback(object sender, SnapshotCompleteEventArgs args)
            {
                using (LOG.DebugCall("ContinuousCollectorCallback"))
                {
                    try
                    {
                        inCollector = false;

                        T snapshot = (T) args.Snapshot;
                        /// SQLdm Minimum Privileges (Varun Chopra) - Permissions Init
						MinimumPermissions = snapshot.MinimumPermissions;
                        MetadataPermissions = snapshot.MetadataPermissions;
                        CollectionPermissions = snapshot.CollectionPermissions;

                        List<Guid> waiters = new List<Guid>();

                        sync.AcquireWriterLock(-1);

                        foreach (Guid guid in waitingRequests.Keys)
                        {
                            waiters.Add(guid);
                        }
                        foreach (Guid guid in waiters)
                        {
                            if (waitingRequests[guid] != null)
                            {
                                // Combine snapshots if it hasn't been picked up, otherwise add a new one
                                if (waitingRequests[guid].Snapshot != null)
                                {
                                    waitingRequests[guid].Snapshot.CombineSnapshot(snapshot);
                                }
                                else
                                {
                                    waitingRequests[guid].Snapshot = snapshot;
                                }
                            }
                            else
                            {
                                // Not a valid entry, so remove it
                                waitingRequests.Remove(guid);
                            }
                        }

                        // Keep the collector from hammering the server in the case of a collection problem
                        if (snapshot.CollectionFailed)
                        {
                            Thread.Sleep(5000);
                        }
                    }

                    catch (Exception e)
                    {
                        LOG.Error("An exception occurred in the continuous collector callback.", e);
                    }
                    finally
                    {
                        if (sync.IsWriterLockHeld)
                            sync.ReleaseWriterLock();

                        if (!IsDisposed)
                        {
                            ((IDisposable)sender).Dispose();

                            continuousCollectionSemaphore.Release();

                            // SQLDM-29459 (SQLdm 10.3) SQLdmCollectionService.exe causing very high CPU & network usage
                            // To indicate re-trigger boolean true is passed
                            StartCollector(true);
                        }
                    }
                }
            }

            internal void CleanupWaiters(object state)
            {
                using (LOG.DebugCall("CleanupWaiters"))
                {
                    try
                    {
                        if (IsDisposed)
                            return;
                        sync.AcquireWriterLock(-1);
                        if (waitingRequests.Count > 0)
                        {
                            Dictionary<Guid, ContinuousCollectionWaiter<T>>.Enumerator e =
                                waitingRequests.GetEnumerator();
                            List<Guid> toRemove = new List<Guid>();
                            while (e.MoveNext())
                            {
                                if (e.Current.Value == null)
                                {
                                    toRemove.Add(e.Current.Key);
                                }
                                else
                                {
                                    if (e.Current.Value.Configuration == null)
                                    {
                                        toRemove.Add(e.Current.Key);
                                    }
                                    else
                                    {
                                        if (!e.Current.Value.Configuration.InPickupWindow)
                                        {
                                            toRemove.Add(e.Current.Key);
                                        }
                                    }
                                }
                            }
                            if (toRemove.Count > 0)
                            {
                                LOG.Verbose(String.Format("Removing {0} waiters.", toRemove.Count));
                                foreach (Guid g in toRemove)
                                {
                                    waitingRequests.Remove(g);
                                }
                            }
                        }
                        sync.ReleaseWriterLock();
                    }
                    catch (Exception e)
                    {
                        LOG.Error("An error occurred when cleaning up waiters.", e);
                        if (sync.IsWriterLockHeld)
                            sync.ReleaseWriterLock();
                    }
                    finally
                    {
                        if (sync.IsWriterLockHeld)
                            sync.ReleaseWriterLock();

                        if (waitingRequests.Count == 0 && runStatus != ContinuousCollectorRunStatus.Disposed)
                            runStatus = ContinuousCollectorRunStatus.ReadyToDispose;

                        if (!IsDisposed)
                        {
                            // Tolga K - to fix memory leak begins
                            if (cleanupTimer != null)
                            {
                                cleanupTimer.Dispose();
                            }
                            // Tolga K - to fix memory leak ends

                            cleanupTimer =
                                new Timer(new TimerCallback(CleanupWaiters),
                                          null,
                                          60000,
                                          Timeout.Infinite);
                        }
                    }
                }
            }

            public bool IsDisposed
            {
                get { return runStatus == ContinuousCollectorRunStatus.Disposed; }
            }

            #region IDisposable Members

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                using (LOG.DebugCall("Dispose"))
                {
                    LOG.Info("Disposing of continuous collection context.");
                    runStatus = ContinuousCollectorRunStatus.Disposed;
                    CleanupWaiters(null);
                    if (cleanupTimer != null)
                    {
                        cleanupTimer.Dispose();
                        cleanupTimer = null;
                    }
                    continuousCollectionSemaphore.Close();
                }
            }

            #endregion
        }
    }
}