//------------------------------------------------------------------------------
// <copyright file="ManagementServiceManager.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.CollectionService.ManagementService
{
    using System;
    using System.Runtime.Remoting;
    using System.Threading;
    using Helpers;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.CollectionService.Configuration;
    using BBS.TracerX;
    using System.Net.NetworkInformation;
    using Idera.SQLdm.Common.Snapshots;
    using Wintellect.PowerCollections;
    using Idera.SQLdm.CollectionService.Monitoring;

    /// <summary>
    /// Class manages interactions with the management service.
    /// </summary>
    public class ManagementServiceManager : IDisposable
    {
        #region fields
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("ManagementServiceManager");

        private CollectionServiceWorkload workload;
        private HeartbeatSender heartbeatSender;
        private ManualResetEvent stopEvent;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ManagementServiceManager"/> class.
        /// </summary>
        internal ManagementServiceManager()
        {
            Initialize();
            stopEvent = new ManualResetEvent(false);
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the management service URL.
        /// </summary>
        /// <value>The management service URL.</value>
        protected string ManagementServiceUrl
        {
            get
            {
                Uri clientUrl = new Uri(CollectionServiceConfiguration.ManagementServiceUri, "Management");
                return clientUrl.ToString();
            }
        }

        #endregion

        #region events

        #endregion

        #region methods

        public void StartCollection()
        {
            using (LOG.InfoCall( "StartCollection"))
            {
                LOG.Info("Opening session to Management Service");
                do //Loop attempting to open a session until the service receives a stop request or opens the session
                {
                    try
                    {
                        Collection.ManagementService.OpenSession();
                        LOG.Info("Session to Management Service opened.  Work started");
                        break;
                    }
                    catch (Exception exception)
                    {
                        if (exception is InvalidOperationException)
                        {
                            LOG.Info("Management Service contacted --- attempting registration.");
                            RegisterWithManagementService();
                        }
                        else
                            LOG.Warn("Error opening session: ", exception);
                    }
                } while (true &&
                         stopEvent.WaitOne(CollectionServiceConfiguration.DisconnectInterval*1000, true) == false);
                LOG.Info("Session open request aborted.");
            }
        }

        public void StopCollection()
        {
            using (LOG.InfoCall( "StopCollection"))
            {
                stopEvent.Set();
                CloseSession();
            }
        }

        /// <summary>
        /// Opens the session.
        /// </summary>
        public void OpenSession()
        {
            using (LOG.InfoCall( "OpenSession"))
            {
                string machineName = Environment.MachineName;
                string instanceName = CollectionServiceConfiguration.InstanceName;

                IManagementService2 mgmtSvc = RemotingHelper.GetObject<IManagementService2>();

                workload = mgmtSvc.GetCollectionServiceWorkload(instanceName, machineName);
                if (workload == null)
                    throw new Exception("Error getting workload from Management Service.");

                // update the configuration with the collection service id
                Guid csid = workload.CollectionService.Id;
                CollectionServiceConfiguration.CollectionServiceId = csid;

                PersistenceManager pm = PersistenceManager.Instance;
//                try
//                {
//                    pm.UpgradeScheduledDataSendQueue();
//                } catch (Exception e)
//                {
//                    LOG.Error("Error upgrading cached scheduled collection queue: ", e);
//                }

                Guid? prev_csid = pm.GetCollectionServiceID();
                if (prev_csid != csid)
                {
                    pm.ReinitializePersistence();
                    pm.SetCollectionServiceID(csid);
                }


                //Start the heartbeats
                heartbeatSender.Start();

                // update the custom counters
                foreach (int key in workload.MetricDefinitions.GetCounterDefinitionKeys())
                {
                    Collection.ReplaceCustomCounter(workload.MetricDefinitions.GetCounterDefinition(key));
                }

                // update custom counter tags
                Collection.SetCustomCounterTags(workload.CustomCounterTags);

                //SQLdm 10.0 (Gaurav Karwal): setting the monitored server counter for perf counters
                Statistics.SetMonitoredServers(workload.MonitoredServerWorkloads.Count);
                int countServersInMaintenanceMode = 0;
                
                //Register monitored servers, start scheduled collection
                foreach (MonitoredServerWorkload serverWorkload in workload.MonitoredServerWorkloads)
                {
                    //SQLdm 10.0 (Gaurav Karwal): setting the maintenance mode monitored server counter for perf counters
                    if (serverWorkload.MonitoredServer.MaintenanceModeEnabled == true) countServersInMaintenanceMode++;

                    Collection.OnDemand.AddMonitoredServer(serverWorkload.MonitoredServer);
                    Collection.Scheduled.StartMonitoring(serverWorkload);
                    LOG.DebugFormat("ServerWorkload\n id: {0}\n name: {1}\n monitoredServer: {2}\n normalCollectionInterval: {3}", serverWorkload.Id, serverWorkload.Name, serverWorkload.MonitoredServer, serverWorkload.NormalCollectionInterval);
                }

                Statistics.SetMaintainenceModeServers(countServersInMaintenanceMode);
                //[END] SQLdm 10.0 (Gaurav Karwal): setting the monitored server counter
                
                if(workload.ExcludedWaitTypes.Count > 0)
                {
                    Collection.AddExcludedWaitTypes(workload.ExcludedWaitTypes);
                }

            }
        }

        protected void RegisterWithManagementService()
        {
            using (LOG.InfoCall( "RegisterWithManagementService"))
            {
                string machineName = Environment.MachineName;
                string instanceName = CollectionServiceConfiguration.InstanceName;
                int port = CollectionServiceConfiguration.ServicePort;
                string address = null;

                if (!MainService.IsClustered)
                {
                    try
                    {
                        address = IPGlobalProperties.GetIPGlobalProperties().HostName;
                    }
                    catch (Exception e)
                    {
                        /* */
                    }
                }

                if (address == null)
                    address = machineName;

                try
                {
                    LOG.InfoFormat("Registering collection service: m={0}, i={1}, a={2}, p={3}, c={4}",
                                   machineName, instanceName, address, port, MainService.IsClustered);

                    IManagementService2 mgmtSvc = RemotingHelper.GetObject<IManagementService2>();
                    Guid id = mgmtSvc.RegisterCollectionService(machineName, instanceName, address, port, false);
                    if (id != Guid.Empty)
                        LOG.Info("Registration successful: id=" + id.ToString());
                }
                catch (Exception e)
                {
                    LOG.Debug("Error registering collection service", e);
                }
            }
        }

        //<summary>
        //SQLDM - 28476
        //Checks for Valid License...
        //</summary>
        internal void CheckLicense(bool enforce)
        {
            IManagementService2 mgmtSvc = RemotingHelper.GetObject<IManagementService2>();
            mgmtSvc.CheckLicense(enforce);
        }

        /// <summary>
        /// Closes the session.
        /// </summary>
        public void CloseSession()
        {
            using (LOG.InfoCall( "CloseSession"))
            {
                //Stop scheduled collection
                Collection.Scheduled.Stop();

                if (workload != null)
                {
                    //Send graceful session close
                    IManagementService2 mgmtSvc = RemotingHelper.GetObject<IManagementService2>();
                    mgmtSvc.CloseCollectionService(workload.CollectionService.Id);
                }
            }
        }

        /// <summary>
        /// Sends the snapshot.
        /// </summary>
        public delegate void ProcessScheduledCollectionDataDelegate(Guid collectionServiceId, int monitoredServerId, Serialized<ScheduledCollectionDataMessage> scheduledRefresh);
        public void SendScheduledCollectionData(int serverId, Serialized<ScheduledCollectionDataMessage> refresh, int timeoutMs)
        {
            using (LOG.DebugCall("SendScheduledCollectionData"))
            {
                DateTime deliveryTime = DateTime.Now;
                try
                {
                    if (workload == null)
                    {
                        LOG.Error("No workload configured");
                        throw new InvalidOperationException("No workload configured");
                    }


                    IManagementService2 mgmtSvc = RemotingHelper.GetObject<IManagementService2>();
                    if (timeoutMs > 0)
                    {
                        ProcessScheduledCollectionDataDelegate pscd =
                            new ProcessScheduledCollectionDataDelegate(mgmtSvc.ProcessScheduledCollectionData);

                        IAsyncResult ar = pscd.BeginInvoke(workload.CollectionService.Id, serverId, refresh, null, null);
                        if (!ar.CompletedSynchronously)
                        {
                            WaitHandle hWait = ar.AsyncWaitHandle;
                            if (!ar.IsCompleted)
                            {
                                if (!hWait.WaitOne(timeoutMs, false))
                                {
                                    throw new TimeoutException("Timed out waiting for remote method call to complete.");
                                }
                            }

                            pscd.EndInvoke(ar);

                            try
                            {
                                hWait.Close();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    else
                        mgmtSvc.ProcessScheduledCollectionData(workload.CollectionService.Id, serverId, refresh);

                    heartbeatSender.UpdateScheduledRefreshData(deliveryTime, DateTime.Now - deliveryTime, null);
                } 
                catch (Exception e)
                {
                    heartbeatSender.UpdateScheduledRefreshData(deliveryTime, DateTime.Now - deliveryTime, e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            RemotingHelper.RegisterWellKnownType(typeof(IManagementService2), new Uri(CollectionServiceConfiguration.ManagementServiceUri, "Management"));
            RemotingHelper.RegisterWellKnownType(typeof(IManagementService), new Uri(CollectionServiceConfiguration.ManagementServiceUri, "Management"));

            if (heartbeatSender == null)
            {
                heartbeatSender = new HeartbeatSender();
                CollectionServiceConfiguration.OnHeartbeatIntervalChanged += heartbeatSender.TimerIntervalChanged;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CollectionServiceConfiguration.OnHeartbeatIntervalChanged -= heartbeatSender.TimerIntervalChanged;
            heartbeatSender.Dispose();
            heartbeatSender = null;
        }

        #endregion

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
