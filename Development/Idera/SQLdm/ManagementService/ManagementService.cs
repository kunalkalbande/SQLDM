//------------------------------------------------------------------------------
// <copyright file="ManagementService.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
// Change Log   ----------------------------------------------------------------
// Modified By          :   Pruthviraj Nikam
// Modification ID      :   M1
// Date                 :   06-Feb-2019
// Description          :   Done changes for New Azure SQL DB alerts.
//----------------------------------------------------------------------------
using System.Collections;
using System.Linq;
using Idera.SQLdm.ManagementService.Auditing;

using System.Runtime.Remoting.Messaging;
using Idera.SQLdm.Common.Events.AzureMonitor;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;
using Idera.SQLdm.ManagementService.Auditing.Actions;
using Idera.SQLdm.Services.Common.Probes.Azure;

namespace Idera.SQLdm.ManagementService
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Security.Principal;
	using System.Text;
	using System.Threading;
	using System.Xml;
	using Common;
	using Configuration;
	using Helpers;
	using Idera.SQLdm.Common.Configuration;
	using Idera.SQLdm.Common.Configuration.ServerActions;
	using Idera.SQLdm.Common.Data;
	using Idera.SQLdm.Common.Events;
	using Idera.SQLdm.Common.Messages;
	using Idera.SQLdm.Common.Notification;
	using Idera.SQLdm.Common.Notification.Providers;
	using Idera.SQLdm.Common.Objects;
	using Idera.SQLdm.Common.Objects.ApplicationSecurity;
	using Idera.SQLdm.Common.Services;
	using Idera.SQLdm.Common.Snapshots;
	using Idera.SQLdm.Common.Thresholds;
	using Idera.SQLdm.ManagementService.Notification.Providers;
	using Monitoring;
	using Notification;
	using Wintellect.PowerCollections;
	using System.Diagnostics;
	using Idera.SQLdm.Common.Auditing;
	using Idera.SQLdm.Common.CWFDataContracts;
	using PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Resources;
	using PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.AdHoc;
	using PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Values;
	using Common.Snapshots.Cloud;
	using PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration;
	using Common.Recommendations;
	using PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Objects;

	/// <summary>
	/// Implementation for the remoted IManagementService interface
	/// </summary>
	public partial class ManagementService : MarshalByRefObject, IManagementService, IManagementService2, IPredictiveAnalytics
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("ManagementService");
        private static BBS.TracerX.Logger StartUpTimeLog = BBS.TracerX.Logger.GetLogger("StartUpTimeLog");
        private static int tempAnalysisID = 0;

		#region Application Security methods

		private object applicationSecurityLock = new object();

        public void EnableSecurity()
        {
            lock (applicationSecurityLock)
            {
                try
                {
                    SecurityManagement.EnableSecurity(ManagementServiceConfiguration.ConnectionString);
                }
                catch (Exception ex)
                {
                    LOG.Error("EnableSecurity encountered an exception", ex);
                    throw ex;
                }
                AuditContextData auditContext = CallContext.GetData(AuditContextData.ContextName) as AuditContextData;

                if (auditContext != null)
                {
                    var entity = new MAuditableEntity(auditContext);
                    entity.Name = "SQLdm";

                    MAuditingEngine.Instance.LogAction(entity, AuditableActionType.ApplicationSecurityEnabled);
                }

                // sync asset list and permissions with pulse
                WebClient.WebClient.SafeKickoffAssetSynchronization(null);
            }
        }

        public void DisableSecurity()
        {
            lock (applicationSecurityLock)
            {
                try
                {
                    SecurityManagement.DisableSecurity(ManagementServiceConfiguration.ConnectionString);
                }
                catch (Exception ex)
                {
                    LOG.Error("DisableSecurity encountered an exception", ex);
                    throw ex;
                }

                AuditContextData auditContext = CallContext.GetData(AuditContextData.ContextName) as AuditContextData;
                if (auditContext != null)
                {
                    var entity = new MAuditableEntity(auditContext);
                    entity.Name = "SQLdm";
                    MAuditingEngine.Instance.LogAction(entity, AuditableActionType.ApplicationSecurityDisabled);
                }

                // sync asset list and permissions with pulse
                WebClient.WebClient.SafeKickoffAssetSynchronization(null);
            }
        }

        public Idera.SQLdm.Common.Objects.ApplicationSecurity.Configuration GetSecurityConfiguration()
        {
            // No locking needed.
            Idera.SQLdm.Common.Objects.ApplicationSecurity.Configuration config = null;
            try
            {
                config = SecurityManagement.GetSecurityConfiguration(ManagementServiceConfiguration.ConnectionString);
            }
            catch (Exception ex)
            {
                LOG.Error("GetSecurityConfiguration encountered an exception", ex);
                throw ex;
            }
            return config;
        }

        public bool DoesLoginExist(string login)
        {
            bool ret = false;
            lock (applicationSecurityLock)
            {
                try
                {
                    ret = SecurityManagement.DoesLoginExist(ManagementServiceConfiguration.ConnectionString, login);
                }
                catch (Exception ex)
                {
                    LOG.Error("DoesLoginExist encountered an exception", ex);
                    throw ex;
                }
            }
            return ret;
        }

        public void AddPermission(
                string login,
                bool isSQLLogin,
                string password,
                PermissionType permission,
                IEnumerable<int> tags,
                IEnumerable<int> servers,
                string comment,
                bool webAppPermission
            )
        {
            lock (applicationSecurityLock)
            {
                try
                {
                    SecurityManagement.AddPermission(ManagementServiceConfiguration.ConnectionString,
                                                        ManagementServiceConfiguration.GetRepositoryConnection().Database,
                                                            login, isSQLLogin, password, permission, tags, servers, comment, webAppPermission);
                }

                catch (Exception ex)
                {
                    LOG.Error("AddPermission encountered an exception", ex);
                    throw ex;
                }
            }

            MAuditingEngine.LogAddPermissionAuditAction();

            // sync asset list and permissions with pulse
            WebClient.WebClient.SafeKickoffAssetSynchronization(null);
        }

        public void EditPermission(
                int permissionID,
                bool enabled,
                PermissionType
                permission,
                IEnumerable<int> tags,
                IEnumerable<int> servers,
                string comment,
                bool webAppPermission
            )
        {
            lock (applicationSecurityLock)
            {
                var oldTags = RepositoryHelper.GetTagsByPermissionID(ManagementServiceConfiguration.ConnectionString, permissionID);

                var oldPermissionDef = RepositoryHelper.GetPermissionDefinitionById(permissionID,
                    ManagementServiceConfiguration.ConnectionString);

                try
                {
                    SecurityManagement.EditPermission(ManagementServiceConfiguration.ConnectionString, permissionID,
                                                      enabled, permission, tags, servers, comment, webAppPermission);

                }
                catch (Exception ex)
                {
                    LOG.Error("EditPermission encountered an exception", ex);
                    throw ex;
                }

                var newTags = RepositoryHelper.GetTagsByPermissionID(ManagementServiceConfiguration.ConnectionString, permissionID);

                var currentPermissionDef = RepositoryHelper.GetPermissionDefinitionById(permissionID,
                ManagementServiceConfiguration.ConnectionString);

                AuditContextData auditContext = CallContext.GetData(AuditContextData.ContextName) as AuditContextData;

                if (auditContext != null && oldPermissionDef != null && currentPermissionDef != null)
                {
                    var entity = new MAuditableEntity(currentPermissionDef.GetAuditableEntity(oldPermissionDef));

                    if (entity.HasMetadataProperties())
                    {
                        entity.SqlUser = auditContext.SqlUser;
                        entity.Workstation = auditContext.Workstation;
                        entity.WorkstationUser = auditContext.WorkstationUser;
                        string login = currentPermissionDef.Login;
                        entity.Name = login;
                        entity.SetHeaderParam(login);
                        MAuditingEngine.Instance.LogAction(entity, AuditableActionType.ApplicationSecurityEditUserAccount);
                    }
                    MAuditingEngine.Instance.LogAction(new TagSecurityAction(oldTags, newTags, currentPermissionDef));
                }
            }

            // sync asset list and permissions with pulse
            WebClient.WebClient.SafeKickoffAssetSynchronization(null);
        }

        public void SetPermissionStatus(
                int permissionID,
                bool flag
            )
        {
            lock (applicationSecurityLock)
            {
                var oldPermissionDef = RepositoryHelper.GetPermissionDefinitionById(permissionID,
                    ManagementServiceConfiguration.ConnectionString);

                try
                {
                    SecurityManagement.SetPermissionStatus(ManagementServiceConfiguration.ConnectionString, permissionID, flag);

                }
                catch (Exception ex)
                {
                    LOG.Error("SetPermissionStatus encountered an exception", ex);
                    throw ex;
                }

                var contextEntity = CallContext.GetData(AuditContextData.ContextName) as AuditContextData;

                if (contextEntity != null && oldPermissionDef != null)
                {
                    var currentPermissionDef = ObjectHelper.Clone(oldPermissionDef);
                    currentPermissionDef.Enabled = flag;
                    var entity = new MAuditableEntity(currentPermissionDef.GetAuditableEntity(oldPermissionDef));
                    entity.SqlUser = contextEntity.SqlUser;
                    entity.Workstation = contextEntity.Workstation;
                    entity.WorkstationUser = contextEntity.WorkstationUser;
                    string login = currentPermissionDef.Login;
                    entity.Name = login;
                    entity.SetHeaderParam(login);

                    MAuditingEngine.Instance.LogAction(entity, AuditableActionType.ApplicationSecurityEditUserAccount);
                }
            }

            // sync asset list and permissions with pulse
            WebClient.WebClient.SafeKickoffAssetSynchronization(null);
        }

        public void SetWebAccessStatus(
                int permissionID,
                bool flag
            )
        {
            lock (applicationSecurityLock)
            {
                var oldPermissionDef = RepositoryHelper.GetPermissionDefinitionById(permissionID,
                    ManagementServiceConfiguration.ConnectionString);

                try
                {
                    SecurityManagement.SetWebAccessStatus(ManagementServiceConfiguration.ConnectionString, permissionID, flag);

                }
                catch (Exception ex)
                {
                    LOG.Error("SetPermissionStatus encountered an exception", ex);
                    throw ex;
                }

                var contextEntity = CallContext.GetData(AuditContextData.ContextName) as AuditContextData;

                if (contextEntity != null && oldPermissionDef != null)
                {
                    var currentPermissionDef = ObjectHelper.Clone(oldPermissionDef);
                    currentPermissionDef.Enabled = flag;
                    var entity = new MAuditableEntity(currentPermissionDef.GetAuditableEntity(oldPermissionDef));
                    entity.SqlUser = contextEntity.SqlUser;
                    entity.Workstation = contextEntity.Workstation;
                    entity.WorkstationUser = contextEntity.WorkstationUser;
                    string login = currentPermissionDef.Login;
                    entity.Name = login;
                    entity.SetHeaderParam(login);

                    MAuditingEngine.Instance.LogAction(entity, AuditableActionType.ApplicationSecurityEditUserAccount);
                }
            }

            // sync asset list and permissions with pulse
            WebClient.WebClient.SafeKickoffAssetSynchronization(null);
        }

        public void DeletePermission(int permissionID)
        {
            var permissionDefinition = RepositoryHelper.GetPermissionDefinitionById(permissionID, ManagementServiceConfiguration.ConnectionString);

            lock (applicationSecurityLock)
            {
                try
                {
                    SecurityManagement.DeletePermission(ManagementServiceConfiguration.ConnectionString, permissionID);
                }
                catch (Exception ex)
                {
                    LOG.Error("DeletePermission encountered an exception", ex);
                    throw ex;
                }
            }

            AuditContextData auditContext = CallContext.GetData(AuditContextData.ContextName) as AuditContextData;
            if (auditContext != null)
            {
                if (permissionDefinition != null)
                {
                    var entity = new MAuditableEntity(permissionDefinition.GetAuditableEntity());
                    entity.SqlUser = auditContext.SqlUser;
                    entity.Workstation = auditContext.Workstation;
                    entity.WorkstationUser = auditContext.WorkstationUser;
                    string login = entity.GetPropertyValue("Login");
                    entity.Name = login;
                    entity.SetHeaderParam(login);
                    MAuditingEngine.Instance.LogAction(entity, AuditableActionType.ApplicationSecurityDeleteUserAccount);
                }
            }

            // sync asset list and permissions with pulse
            WebClient.WebClient.SafeKickoffAssetSynchronization(null);
        }

        #endregion

        #region Configuration methods

        public ManagementServiceConfigurationMessage GetManagementServiceConfiguration()
        {
            using (LOG.InfoCall("GetManagementServiceConfiguration"))
            {
                return new ConfigurationService().GetManagementServiceConfiguration();
            }
        }

        public bool SetManagementServiceConfiguration(ManagementServiceConfigurationMessage config)
        {
            using (LOG.InfoCall("SetManagementServiceConfiguration"))
            {
                return new ConfigurationService().SetManagementServiceConfiguration(config);
            }
        }

        /// <summary>
        /// Returns a list of Collection Services registered to this Management Service.
        /// </summary>
        /// <returns></returns>
        public IList<CollectionServiceInfo> GetCollectionServices()
        {
            using (LOG.InfoCall("GetCollectionServices"))
            {
                return Management.CollectionServices.GetCollectionServices();
            }
        }

        public Guid RegisterCollectionService(string machineName, string instanceName, string address, int servicePort, bool force)
        {
            using (LOG.InfoCall("RegisterCollectionService"))
            {
                return Management.CollectionServices.RegisterCollectionService(machineName, instanceName, address, servicePort, force);
            }
        }

        public void UnregisterCollectionService(string MachineName, string InstanceName)
        {
            using (LOG.InfoCall("UnregisterCollectionService"))
            {
                LOG.Warn("UnregisterCollectionService is not implemented!");
            }
        }

        public ManagementServiceStatus GetServiceStatus()
        {
            using (LOG.InfoCall("GetServiceStatus"))
            {
                return new ConfigurationService().GetServiceStatus();
            }
        }

        public object Echo(object incoming)
        {
            using (LOG.InfoCall("Echo"))
            {
                LOG.InfoFormat("Echo: {0}", incoming);
                return incoming;
            }
        }

        #endregion

        #region Collection Service methods

        public CollectionServiceWorkload GetCollectionServiceWorkload(string collectionServiceInstance, string collectionServiceMachine)
        {
            using (LOG.InfoCall("OpenCollectionServiceSession"))
            {
                if (collectionServiceInstance == null)
                    throw new ArgumentNullException("collectionServiceInstance");
                if (collectionServiceMachine == null)
                    throw new ArgumentNullException("collectionServiceMachine");

                // Check to make sure this is THE management service in the repository
                Management.CheckRegistration();

                Guid? collectionServiceId;
                if (!Management.CollectionServices.GetCollectionServiceId(collectionServiceInstance, collectionServiceMachine, out collectionServiceId))
                {
                    throw new InvalidOperationException(
                        string.Format("Collection Service ID not found: {0}/{1}",
                                      collectionServiceInstance,
                                      collectionServiceMachine));
                }

                return GetCollectionServiceWorkload(collectionServiceId.Value);
            }
        }

        public CollectionServiceWorkload GetCollectionServiceWorkload(Guid collectionServiceId)
        {
            using (LOG.InfoCall("GetCollectionServiceWorkload"))
            {
                try
                {
                    return Management.CollectionServices.GetCollectionServiceWorkload(collectionServiceId);
                }
                catch (Exception e)
                {
                    LOG.Error("Error building collection service workload: ", e);
                    throw;
                }
            }
        }

        public Result CloseCollectionService(Guid collectionServiceId)
        {
            using (LOG.InfoCall("CloseCollectionService"))
            {
                try
                {
                    if (Management.CollectionServices.Close(collectionServiceId))
                        return Result.Success;
                }
                catch (Exception exception)
                {
                    LOG.Fatal(exception);
                }
                return Result.Failure;
            }
        }

        /// <summary>
        /// Handles receipt of a heartbeat from the collection service.
        /// </summary>
        /// <param name="collectionServiceId"></param>
        /// <param name="nextExpected"></param>
        /// <returns></returns>
        public Result ProcessCollectionServiceHeartbeat(Guid collectionServiceId, TimeSpan nextExpected, DateTime? lastSnapshotDeliveryAttempt, TimeSpan? lastSnapshotDeliveryAttemptTime, Exception lastSnapshotDeliveryException, int scheduledRefreshDeliveryTimeoutCount)
        {
            using (LOG.InfoCall("ProcessCollectionServiceHeartbeat"))
            {
                try
                {
                    Management.CollectionServices.UpdateHeartbeat(collectionServiceId, nextExpected, lastSnapshotDeliveryAttempt, lastSnapshotDeliveryAttemptTime, lastSnapshotDeliveryException, scheduledRefreshDeliveryTimeoutCount);
                    return Result.Success;
                }
                catch (Exception exception)
                {
                    LOG.Fatal(exception);
                    return Result.Failure;
                }
            }
        }

        /// <summary>
        /// Called by the Collection Service to send the scheduled collection data.
        /// </summary>
        public void ProcessScheduledCollectionData(Guid collectionServiceId, int monitoredServerId, Serialized<ScheduledCollectionDataMessage> serializedRefresh)
        {
            using (LOG.InfoCall("ProcessScheduledCollectionData"))
            {
                if (serializedRefresh == null)
                {
                    ServiceException excp = new ServiceException(Status.ErrorArgumentRequired, "scheduledRefresh");
                    LOG.Error(excp);
                    throw excp;
                }

                /* uncomment to test collection service delivery timeouts 
                Thread.Sleep((int)TimeSpan.FromMinutes(5).TotalMilliseconds);
                */

                // make sure the collection service exists and is enabled
                CollectionServiceContext csc = Management.CollectionServices[collectionServiceId];
                if (csc == null || !csc.CollectionService.Enabled)
                {
                    ServiceException excp = new ServiceException(Status.ErrorInvalidCollectionServiceId, collectionServiceId.ToString());
                    LOG.Error(excp);
                    throw excp;
                }

                try
                {
                    ScheduledCollectionDataMessage refresh = serializedRefresh;
                    if (ManagementServiceConfiguration.GetManagementServiceElement().IgnoreScheduledCollection)
                    {
                        LOG.InfoFormat("Ignored refresh for {0} at {1}",
                                        refresh.MonitoredServer.InstanceName,
                                        refresh.Snapshot.TimeStampLocal);
                    }
                    else
                    {
                        LOG.InfoFormat("Received refresh for {0} at {1}",
                                       refresh.MonitoredServer.InstanceName,
                                       refresh.Snapshot.TimeStampLocal);
                        Management.ScheduledCollection.Add(monitoredServerId, refresh);
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("Error receiving scheduled collection from collection service: ", e);
                }
                /*
                // process the data in the background
                Management.QueueDelegate(delegate()
                {
                    try
                    {
                        LOG.DebugFormat("Processing scheduled collection data for server: {0}", data.MonitoredServer.ConnectionInfo.InstanceName);
                        Management.ScheduledCollection.Process(data);
                    }
                    catch (Exception exception)
                    {
                        LOG.Error(exception);
                    } 
                    finally
                    {
                        data.Dispose();
                    }
                });
                 */
            }
        }

        #endregion

        #region IOnDemandServer Members

        public void CancelOnDemandRequest(Guid sessionId)
        {
            using (LOG.InfoCall("CancelOnDemandRequest"))
            {
                Dictionary<Guid, ISnapshotSink> sinks = Management.SnapshotSinks;
                lock (sinks)
                {
                    ISnapshotSink sink = null;
                    if (sinks.TryGetValue(sessionId, out sink))
                        sink.Cancelled = true;
                }
            }
        }

        /// <summary>
        /// Sets sysAdmin to true if monitored sql server has sysadmin rights
        /// </summary>
        /// <param name="monitoredSqlServer"></param>
        /// <returns></returns>
        public TestSqlConnectionResult isSysAdmin(SqlConnectionInfo connectionInfo)
        {
            //Serialized<ActiveWaitsSnapshot> snapshot = null;

            using (LOG.InfoCall("IsSysAdmin"))
            {
                try
                {
                    TestSqlConnectionResult result =
                    Management.CollectionServices.DefaultCollectionService.IsSysAdmin(connectionInfo);

                    return result;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// SQLdm 10.0.2 (Barkha Khatri)
        /// gets the product vesion of an SQL Server given the connection string 
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public ServerVersion GetProductVersion(int instanceId)
        {
            using (LOG.InfoCall("GetProductVersion"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(instanceId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                            instanceId));

                    return collSvc.GetProductVersion(instanceId);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }
        public Serialized<ActiveWaitsSnapshot> GetActiveWaits(ActiveWaitsConfiguration configuration)
        {
            //Serialized<ActiveWaitsSnapshot> snapshot = null;

            using (LOG.InfoCall("GetActiveWaits"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    return collSvc.GetActiveWaits(configuration);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public void StopWaitingForActiveWaits(ActiveWaitsConfiguration configuration)
        {
            using (LOG.InfoCall("StopWaitingForActiveWaits"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    collSvc.StopWaitingForActiveWaits(configuration);

                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public void StopActiveWaitCollector(int MonitoredServerId)
        {
            using (LOG.InfoCall("StopActiveWaitCollector"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          MonitoredServerId));

                    collSvc.StopActiveWaitCollector(MonitoredServerId);

                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        // Read Permissions Information along with Wait Status
        public Tuple<ContinuousCollectorRunStatus, MinimumPermissions, MetadataPermissions, CollectionPermissions> GetActiveWaitCollectorStatus(int MonitoredServerId)
        {
            using (LOG.InfoCall("GetActiveWaitCollectorStatus"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          MonitoredServerId));

                    return collSvc.GetActiveWaitCollectorStatus(MonitoredServerId);

                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<AgentJobHistorySnapshot> GetAgentJobHistory(AgentJobHistoryConfiguration configuration)
        {
            Serialized<AgentJobHistorySnapshot> snapshot = null;

            using (LOG.InfoCall("GetAgentJobHistory"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<AgentJobHistorySnapshot> context = new OnDemandCollectionContext<AgentJobHistorySnapshot>())
                    {
                        // start collecting the data
                        collSvc.GetAgentJobHistory(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<AgentJobSummary> GetAgentJobSummary(AgentJobSummaryConfiguration configuration)
        {
            Serialized<AgentJobSummary> snapshot = null;

            using (LOG.InfoCall("GetAgentJobSummary"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<AgentJobSummary> context = new OnDemandCollectionContext<AgentJobSummary>())
                    {
                        // start collecting the data
                        collSvc.GetAgentJobSummary(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<BackupRestoreHistory> GetBackupRestoreHistory(BackupRestoreHistoryConfiguration configuration)
        {
            Serialized<BackupRestoreHistory> snapshot = null;

            using (LOG.InfoCall("GetBackupRestoreHistory"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<BackupRestoreHistory> context = new OnDemandCollectionContext<BackupRestoreHistory>())
                    {
                        // start collecting the data
                        collSvc.GetBackupRestoreHistory(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<ConfigurationSnapshot> GetConfiguration(OnDemandConfiguration configuration)
        {
            Serialized<ConfigurationSnapshot> snapshot = null;

            using (LOG.InfoCall("GetConfiguration"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(string.Format("Unable to get interface for monitored server {0}", configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<ConfigurationSnapshot> context = new OnDemandCollectionContext<ConfigurationSnapshot>())
                    {
                        // start collecting the data
                        collSvc.GetConfiguration(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<CustomCounterSnapshot> GetCustomCounter(CustomCounterConfiguration configuration)
        {
            using (LOG.InfoCall("GetCustomCounter"))
            {
                List<CustomCounterConfiguration> collection = new List<CustomCounterConfiguration>();
                collection.Add(configuration);
                CustomCounterCollectionSnapshot ccs;
                ccs = GetCustomCounter(collection);
                if (ccs.CustomCounterList != null && ccs.CustomCounterList.ContainsKey(configuration.MetricID))
                {
                    return ccs.CustomCounterList[configuration.MetricID];
                }
                else
                {
                    return null;
                }
            }
        }

        public Serialized<CustomCounterCollectionSnapshot> GetCustomCounter(List<CustomCounterConfiguration> configuration)
        {
            Serialized<CustomCounterCollectionSnapshot> snapshot = null;

            using (LOG.InfoCall("GetCustomCounter"))
            {
                if (configuration == null || configuration.Count == 0)
                {
                    LOG.Error("No configuration was provided");
                    throw new ArgumentException("No configuration was provided");
                }

                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration[0].MonitoredServerId);
                    if (collSvc == null)
                    {
                        LOG.Error("Unable to get interface for monitored server");
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration[0].MonitoredServerId));
                    }
                    using (OnDemandCollectionContext<CustomCounterCollectionSnapshot> context = new OnDemandCollectionContext<CustomCounterCollectionSnapshot>())
                    {
                        // start collecting the data
                        collSvc.GetCustomCounter(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }

                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<DatabaseConfigurationSnapshot> GetDatabaseConfiguration(DatabaseProbeConfiguration configuration)
        {
            Serialized<DatabaseConfigurationSnapshot> snapshot = null;

            using (LOG.InfoCall("GetDatabaseConfiguration"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<DatabaseConfigurationSnapshot> context = new OnDemandCollectionContext<DatabaseConfigurationSnapshot>())
                    {
                        // start collecting the data
                        collSvc.GetDatabaseConfiguration(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<DatabaseFilesSnapshot> GetDatabaseFiles(DatabaseFilesConfiguration configuration)
        {
            Serialized<DatabaseFilesSnapshot> snapshot = null;

            using (LOG.InfoCall("GetDatabaseFiles"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<DatabaseFilesSnapshot> context = new OnDemandCollectionContext<DatabaseFilesSnapshot>())
                    {
                        // start collecting the data
                        collSvc.GetDatabaseFiles(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<DatabaseSummary> GetDatabaseSummary(DatabaseSummaryConfiguration configuration)
        {
            Serialized<DatabaseSummary> snapshot = null;

            using (LOG.InfoCall("GetDatabaseSummary"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<DatabaseSummary> context = new OnDemandCollectionContext<DatabaseSummary>())
                    {
                        // start collecting the data
                        collSvc.GetDatabaseSummary(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<AlwaysOnAvailabilityGroupsSnapshot> GetDatabaseAlwaysOnStatistics(AlwaysOnAvailabilityGroupsConfiguration configuration)
        {
            Serialized<AlwaysOnAvailabilityGroupsSnapshot> snapshot = null;

            using (LOG.InfoCall("GetDatabaseAlwaysOnStatistics"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<AlwaysOnAvailabilityGroupsSnapshot> context = new OnDemandCollectionContext<AlwaysOnAvailabilityGroupsSnapshot>())
                    {
                        // start collecting the data
                        collSvc.GetDatabaseAlwaysOnStatistics(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<AlwaysOnAvailabilityGroupsSnapshot> GetDatabaseAlwaysOnTopology(AlwaysOnAvailabilityGroupsConfiguration configuration)
        {
            Serialized<AlwaysOnAvailabilityGroupsSnapshot> snapshot = null;

            using (LOG.InfoCall("GetDatabaseAlwaysOnTopology"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<AlwaysOnAvailabilityGroupsSnapshot> context = new OnDemandCollectionContext<AlwaysOnAvailabilityGroupsSnapshot>())
                    {
                        // start collecting the data
                        collSvc.GetDatabaseAlwaysOnTopology(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<DistributorQueue> GetDistributorQueue(DistributorQueueConfiguration configuration)
        {
            Serialized<DistributorQueue> snapshot = null;

            using (LOG.InfoCall("GetDistributorQueue"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<DistributorQueue> context = new OnDemandCollectionContext<DistributorQueue>())
                    {
                        // start collecting the data
                        collSvc.GetDistributorQueue(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<DistributorDetails> GetDistributorDetails(DistributorDetailsConfiguration configuration)
        {
            Serialized<DistributorDetails> snapshot = null;

            using (LOG.InfoCall("GetDistributorDetails"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<DistributorDetails> context = new OnDemandCollectionContext<DistributorDetails>())
                    {
                        // start collecting the data
                        collSvc.GetDistributorDetails(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<ErrorLog> GetErrorLog(ErrorLogConfiguration configuration)
        {
            Serialized<ErrorLog> snapshot = null;

            using (LOG.InfoCall("GetErrorLog"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<ErrorLog> context = new OnDemandCollectionContext<ErrorLog>(TimeSpan.FromMinutes(5)))
                    {
                        context.SetSessionId(configuration.ClientSessionId);
                        // start collecting the data
                        collSvc.GetErrorLog(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<FileActivitySnapshot> GetFileActivity(FileActivityConfiguration configuration)
        {
            Serialized<FileActivitySnapshot> snapshot = null;

            using (LOG.InfoCall("GetFileActivity"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(string.Format("Unable to get interface for monitored server {0}",
                                                                  configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<FileActivitySnapshot> context = new OnDemandCollectionContext<FileActivitySnapshot>())
                    {
                        // start collecting the data
                        collSvc.GetFileActivity(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;

                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }


        public Serialized<FullTextCatalogs> GetFullTextCatalogs(OnDemandConfiguration configuration)
        {
            Serialized<FullTextCatalogs> snapshot = null;

            using (LOG.InfoCall("GetFullTextCatalogs"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<FullTextCatalogs> context = new OnDemandCollectionContext<FullTextCatalogs>())
                    {
                        // start collecting the data
                        collSvc.GetFullTextCatalogs(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<FullTextColumns> GetFullTextColumns(FullTextColumnsConfiguration configuration)
        {
            Serialized<FullTextColumns> snapshot = null;

            using (LOG.InfoCall("GetFullTextColumns"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<FullTextColumns> context = new OnDemandCollectionContext<FullTextColumns>())
                    {
                        // start collecting the data
                        collSvc.GetFullTextColumns(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<FullTextTables> GetFullTextTables(FullTextTablesConfiguration configuration)
        {
            Serialized<FullTextTables> snapshot = null;

            using (LOG.InfoCall("GetFullTextTables"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<FullTextTables> context = new OnDemandCollectionContext<FullTextTables>())
                    {
                        // start collecting the data
                        collSvc.GetFullTextTables(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<IndexStatistics> GetIndexStatistics(IndexStatisticsConfiguration configuration)
        {
            Serialized<IndexStatistics> snapshot = null;

            using (LOG.InfoCall("GetIndexStatistics"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<IndexStatistics> context = new OnDemandCollectionContext<IndexStatistics>())
                    {
                        // start collecting the data
                        collSvc.GetIndexStatistics(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }
        public Serialized<LockDetails> GetLockDetails(LockDetailsConfiguration configuration)
        {
            Serialized<LockDetails> snapshot = null;

            using (LOG.InfoCall("GetLockDetails"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<LockDetails> context = new OnDemandCollectionContext<LockDetails>())
                    {
                        // start collecting the data
                        collSvc.GetLockDetails(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<LogFileList> GetLogList(OnDemandConfiguration configuration)
        {
            Serialized<LogFileList> snapshot = null;

            using (LOG.InfoCall("GetLogList"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<LogFileList> context = new OnDemandCollectionContext<LogFileList>())
                    {
                        // start collecting the data
                        collSvc.GetLogList(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<MirrorMonitoringRealtimeSnapshot> GetMirrorMonitoringRealtime(MirrorMonitoringRealtimeConfiguration configuration)
        {
            Serialized<MirrorMonitoringRealtimeSnapshot> snapshot = null;

            using (LOG.InfoCall("GetMirrorMonitoringRealtime"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(string.Format("Unable to get interface for monitored server {0}",
                                                                  configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<MirrorMonitoringRealtimeSnapshot> context = new OnDemandCollectionContext<MirrorMonitoringRealtimeSnapshot>())
                    {
                        // start collecting the data
                        collSvc.GetMirrorMonitoringRealtime(configuration, context, null);

                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<JobsAndStepsSnapshot> GetJobsAndSteps(JobsAndStepsConfiguration configuration)
        {
            Serialized<JobsAndStepsSnapshot> snapshot = null;

            using (LOG.InfoCall("GetJobsAndSteps"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(string.Format("Unable to get interface for monitored server {0}",
                                                                  configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<JobsAndStepsSnapshot> context = new OnDemandCollectionContext<JobsAndStepsSnapshot>())
                    {
                        // start collecting the data
                        collSvc.GetJobsAndSteps(configuration, context, null);

                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public MirrorMonitoringHistorySnapshot GetMirrorMonitoringHistory(MirrorMonitoringHistoryConfiguration configuration)
        {
            Serialized<MirrorMonitoringHistorySnapshot> snapshot = null;

            using (LOG.InfoCall("GetMirrorMonitoringHistory"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(string.Format("Unable to get interface for monitored server {0}",
                                                                  configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<MirrorMonitoringHistorySnapshot> context = new OnDemandCollectionContext<MirrorMonitoringHistorySnapshot>())
                    {
                        // start collecting the data
                        collSvc.GetMirrorMonitoringHistory(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<ProcedureCache> GetProcedureCache(ProcedureCacheConfiguration configuration)
        {
            Serialized<ProcedureCache> snapshot = null;

            using (LOG.InfoCall("GetProcedureCache"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<ProcedureCache> context = new OnDemandCollectionContext<ProcedureCache>())
                    {
                        // start collecting the data
                        collSvc.GetProcedureCache(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        //public Serialized<PublisherQueue> GetPublisherQueue(PublisherQueueConfiguration configuration)
        //{
        //    Serialized<PublisherQueue> snapshot = null;

        //    using (LOG.InfoCall("GetPublisherQueue"))
        //    {
        //        try
        //        {
        //            ICollectionService collSvc =
        //                Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
        //            if (collSvc == null)
        //                throw new ArgumentException(
        //                    String.Format("Unable to get interface for monitored server {0}",
        //                                  configuration.MonitoredServerId));

        //            using (OnDemandCollectionContext<PublisherQueue> context = new OnDemandCollectionContext<PublisherQueue>())
        //            {
        //                // start collecting the data
        //                collSvc.GetPublisherQueue(configuration, context, null);
        //                // wait for the request to complete
        //                snapshot = context.Wait();
        //            }
        //            return snapshot;
        //        }
        //        catch (Exception exception)
        //        {
        //            LOG.Error(exception);
        //            throw;
        //        }
        //    }
        //}

        public Serialized<PublisherDetails> GetPublisherDetails(PublisherDetailsConfiguration configuration)
        {
            Serialized<PublisherDetails> snapshot = null;

            using (LOG.InfoCall("GetPublisherDetails"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<PublisherDetails> context = new OnDemandCollectionContext<PublisherDetails>())
                    {
                        // start collecting the data
                        collSvc.GetPublisherDetails(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<SubscriberDetails> GetSubscriberDetails(SubscriberDetailsConfiguration configuration)
        {
            Serialized<SubscriberDetails> snapshot = null;

            using (LOG.InfoCall("GetSubscriberDetails"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<SubscriberDetails> context = new OnDemandCollectionContext<SubscriberDetails>())
                    {
                        // start collecting the data
                        collSvc.GetSubscriberDetails(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        //public Serialized<ResourceSnapshot> GetResource(ResourceConfiguration configuration)
        //{
        //    Serialized<ResourceSnapshot> snapshot = null;

        //    using (LOG.InfoCall("GetResource"))
        //    {
        //        try
        //        {
        //            ICollectionService collSvc =
        //                Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
        //            if (collSvc == null)
        //                throw new ArgumentException(
        //                    String.Format("Unable to get interface for monitored server {0}",
        //                                  configuration.MonitoredServerId));

        //            using (OnDemandCollectionContext<ResourceSnapshot> context = new OnDemandCollectionContext<ResourceSnapshot>())
        //            {
        //                // start collecting the data
        //                collSvc.GetResource(configuration, context, null);
        //                // wait for the request to complete
        //                snapshot = context.Wait();
        //            }
        //            return snapshot;
        //        }
        //        catch (Exception exception)
        //        {
        //            LOG.Error(exception);
        //            throw;
        //        }
        //    }
        //}

        /// <summary>
        /// Get Server Permissions from Collector on demand
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns>Server Permissions - Minimum, Metadata and Collection</returns>
        public Tuple<MinimumPermissions, MetadataPermissions, CollectionPermissions> GetServerPermissions(
            int instanceId)
        {
            using (LOG.InfoCall("GetServerPermissions"))
            {
                string instanceName = instanceId.ToString();
                ICollectionService collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName);

                if (collectionService == null)
                    collectionService = Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    // Read Permissions
                    return collectionService.GetServerPermissions(instanceId);
                }
                catch (Exception e)
                {
                    string message = "Unable to retrieve current server permissions for " + instanceName;
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }

        public Triple<ServerVersion, DateTime, DateTime> GetServerTimeAndVersion(int instanceId)
        {
            using (LOG.InfoCall("GetServerTimeAndVersion"))
            {
                string instanceName = instanceId.ToString();
                ICollectionService collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName);

                if (collectionService == null)
                    collectionService = Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.GetServerTimeAndVersion(instanceId);
                }
                catch (Exception e)
                {
                    string message = "Unable to retrieve current server time and version for " + instanceName;
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }

        public Serialized<ServerOverview> GetServerOverview(ServerOverviewConfiguration configuration)
        {
            Serialized<ServerOverview> snapshot = null;

            using (LOG.InfoCall("GetServerOverview"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(string.Format("Unable to get interface for monitored server {0}", configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<ServerOverview> context = new OnDemandCollectionContext<ServerOverview>())
                    {
                        // start collecting the data
                        collSvc.GetServerOverview(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public ServerSummarySnapshots GetServerSummary(
            ServerOverviewConfiguration serverOverviewConfiguration)
        {
            using (LOG.InfoCall("GetServerSummary"))
            {
                try
                {
                    Serialized<ServerOverview> serverOverviewSnapshot;

                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(serverOverviewConfiguration.MonitoredServerId);

                    if (collSvc == null)
                    {
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          serverOverviewConfiguration.MonitoredServerId));
                    }

                    // Get the Server Overview snapshot
                    using (OnDemandCollectionContext<ServerOverview> context = new OnDemandCollectionContext<ServerOverview>())
                    {
                        collSvc.GetServerOverview(serverOverviewConfiguration, context, null);
                        serverOverviewSnapshot = context.Wait();
                    }

                    if (serverOverviewConfiguration.CustomCounterConfigurations == null ||
                        serverOverviewConfiguration.CustomCounterConfigurations.Count == 0)
                    {
                        return new ServerSummarySnapshots(serverOverviewSnapshot);
                    }

                    // Get the Custom Counters snapshot if a configuration is provided
                    LOG.Info("Collecting Custom Counters");
                    CustomCounterCollectionSnapshot customCounterCollectionSnapshot;
                    using (OnDemandCollectionContext<CustomCounterCollectionSnapshot> context = new OnDemandCollectionContext<CustomCounterCollectionSnapshot>())
                    {
                        // start collecting the data
                        collSvc.GetCustomCounter(serverOverviewConfiguration.CustomCounterConfigurations, context, null);
                        // wait for the request to complete
                        customCounterCollectionSnapshot = context.Wait();
                    }

                    return new ServerSummarySnapshots(serverOverviewSnapshot, customCounterCollectionSnapshot);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<ServicesSnapshot> GetServices(OnDemandConfiguration configuration)
        {
            Serialized<ServicesSnapshot> snapshot = null;

            using (LOG.InfoCall("GetServices"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<ServicesSnapshot> context = new OnDemandCollectionContext<ServicesSnapshot>())
                    {
                        // start collecting the data
                        collSvc.GetServices(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<SessionSnapshot> GetSessions(SessionsConfiguration configuration)
        {
            Serialized<SessionSnapshot> snapshot = null;

            using (LOG.InfoCall("GetSessions"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(string.Format("Unable to get interface for monitored server {0}", configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<SessionSnapshot> context = new OnDemandCollectionContext<SessionSnapshot>())
                    {
                        // start collecting the data
                        collSvc.GetSessions(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<SessionDetailSnapshot> GetSessionDetails(SessionDetailsConfiguration configuration)
        {
            Serialized<SessionDetailSnapshot> snapshot = null;

            using (LOG.InfoCall("GetSessionDetails"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<SessionDetailSnapshot> context = new OnDemandCollectionContext<SessionDetailSnapshot>())
                    {
                        // start collecting the data
                        collSvc.GetSessionDetails(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<SessionSummary> GetSessionSummary(SessionSummaryConfiguration configuration)
        {
            Serialized<SessionSummary> snapshot = null;

            using (LOG.InfoCall("GetSessionSummary"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<SessionSummary> context = new OnDemandCollectionContext<SessionSummary>())
                    {
                        // start collecting the data
                        collSvc.GetSessionSummary(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<TableDetail> GetTableDetails(TableDetailConfiguration configuration)
        {
            Serialized<TableDetail> snapshot = null;

            using (LOG.InfoCall("GetTableDetails"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<TableDetail> context = new OnDemandCollectionContext<TableDetail>())
                    {
                        // start collecting the data
                        collSvc.GetTableDetails(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<TableSummary> GetTableSummary(TableSummaryConfiguration configuration)
        {
            Serialized<TableSummary> snapshot = null;

            using (LOG.InfoCall("GetTableSummary"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<TableSummary> context = new OnDemandCollectionContext<TableSummary>())
                    {
                        // start collecting the data
                        collSvc.GetTableSummary(configuration, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<WaitStatisticsSnapshot> GetWaitStatistics(WaitStatisticsConfiguration configuration)
        {
            Serialized<WaitStatisticsSnapshot> snapshot = null;

            using (LOG.InfoCall("GetWaitStats"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    OnDemandCollectionContext<WaitStatisticsSnapshot> context = new OnDemandCollectionContext<WaitStatisticsSnapshot>();
                    // start collecting the data
                    collSvc.GetWaitStatistics(configuration, context, null);
                    // wait for the request to complete
                    snapshot = context.Wait();
                    return (WaitStatisticsSnapshot)snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Serialized<WmiConfigurationTestSnapshot> SendWmiConfigurationTest(TestWmiConfiguration configuration)
        {
            Serialized<WmiConfigurationTestSnapshot> snapshot = null;

            using (LOG.InfoCall("SendWmiConfigurationTest"))
            {
                try
                {
                    var collSvc = Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(string.Format("Unable to get interface for monitored server {0}", configuration.MonitoredServerId));

                    var context = new OnDemandCollectionContext<WmiConfigurationTestSnapshot>();
                    // start collecting the data
                    collSvc.SendWmiConfigurationTest(configuration, context, null);
                    // wait for the request to complete
                    snapshot = context.Wait();
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }

                return snapshot;
            }
        }

        #region Server Actions

        internal static Serialized<R> SendServerAction<T, R>(T configuration)
            where T : OnDemandConfiguration, IServerActionConfiguration
                                                                    where R : Snapshot
        {
            Serialized<R> snapshot = null;

            try
            {
                ICollectionService collSvc =
                    Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);

                if (collSvc == null)
                {
                    LOG.Error("Unable to get interface for monitored server");
                    throw new ArgumentException(
                        string.Format("Unable to get interface for monitored server {0}",
                                      configuration.MonitoredServerId));
                }
                using (OnDemandCollectionContext<R> context = new OnDemandCollectionContext<R>())
                {
                    // start collecting the data
                    collSvc.SendServerAction(configuration, context, null);

                    // wait for the request to complete
                    snapshot = context.Wait();
                }

                if (((Snapshot)snapshot).Error == null)
                {
                    MAuditingEngine.Instance.LogServerAction<T>(configuration);
                }

                return snapshot;
            }
            catch (Exception exception)
            {
                LOG.Error(exception);
                throw;
            }
        }

        public Serialized<Snapshot> SendFreeProcedureCache(FreeProcedureCacheConfiguration configuration)
        {
            using (LOG.InfoCall("SendFreeProcedureCache"))
            {
                return SendServerAction<FreeProcedureCacheConfiguration, Snapshot>(configuration);
            }
        }

        public Serialized<Snapshot> SendFullTextAction(FullTextActionConfiguration configuration)
        {
            using (LOG.InfoCall("SendFullTextAction"))
            {
                return SendServerAction<FullTextActionConfiguration, Snapshot>(configuration);
            }
        }

        public Serialized<Snapshot> SendJobControl(JobControlConfiguration configuration)
        {
            using (LOG.InfoCall("SendJobControl"))
            {
                return SendServerAction<JobControlConfiguration, Snapshot>(configuration);
            }
        }

        public Serialized<Snapshot> SendKillSession(KillSessionConfiguration configuration)
        {
            using (LOG.InfoCall("SendKillSession"))
            {
                return SendServerAction<KillSessionConfiguration, Snapshot>(configuration);
            }
        }

        public Serialized<Snapshot> SendReconfiguration(ReconfigurationConfiguration configuration)
        {
            using (LOG.InfoCall("SendReconfiguration"))
            {
                return SendServerAction<ReconfigurationConfiguration, Snapshot>(configuration);
            }
        }

        public Serialized<Snapshot> SendBlockedProcessThresholdChange(BlockedProcessThresholdConfiguration configuration)
        {
            using (LOG.InfoCall("SendBlockedProcessThresholdChange"))
            {
                return SendServerAction<BlockedProcessThresholdConfiguration, Snapshot>(configuration);
            }
        }





        public Serialized<Snapshot> SendShutdownSQLServer(ShutdownSQLServerConfiguration configuration)
        {
            using (LOG.InfoCall("SendShutdownSQLServer"))
            {
                return SendServerAction<ShutdownSQLServerConfiguration, Snapshot>(configuration);
            }
        }

        public Serialized<Snapshot> SendSetNumberOfLogs(SetNumberOfLogsConfiguration configuration)
        {
            using (LOG.InfoCall("SendSetNumberOfLogs"))
            {
                return SendServerAction<SetNumberOfLogsConfiguration, Snapshot>(configuration);
            }
        }

        public Serialized<Snapshot> SendStopSessionDetailsTrace(StopSessionDetailsTraceConfiguration configuration)
        {
            using (LOG.InfoCall("SendStopSessionDetailsTrace"))
            {
                return SendServerAction<StopSessionDetailsTraceConfiguration, Snapshot>(configuration);
            }
        }

        public Serialized<Snapshot> SendStopQueryMonitorTrace(StopQueryMonitorTraceConfiguration configuration)
        {
            using (LOG.InfoCall("SendStopQueryMonitorTrace"))
            {
                return SendServerAction<StopQueryMonitorTraceConfiguration, Snapshot>(configuration);
            }
        }

        public Serialized<Snapshot> SendStopActivityMonitorTrace(StopActivityMonitorTraceConfiguration configuration)
        {
            using (LOG.InfoCall("SendStopActivityMonitorTrace"))
            {
                return SendServerAction<StopActivityMonitorTraceConfiguration, Snapshot>(configuration);
            }
        }

        public Serialized<Snapshot> SendServiceControl(ServiceControlConfiguration configuration)
        {
            using (LOG.InfoCall("SendServiceControl"))
            {
                return SendServerAction<ServiceControlConfiguration, Snapshot>(configuration);
            }
        }

        public Serialized<Snapshot> SendRecycleLog(RecycleLogConfiguration configuration)
        {
            using (LOG.InfoCall("SendRecycleLog"))
            {
                return SendServerAction<RecycleLogConfiguration, Snapshot>(configuration);
            }
        }

        public Serialized<Snapshot> SendRecycleAgentLog(RecycleAgentLogConfiguration configuration)
        {
            using (LOG.InfoCall("SendRecycleAgentLog"))
            {
                return SendServerAction<RecycleAgentLogConfiguration, Snapshot>(configuration);
            }
        }

        public Serialized<ReindexSnapshot> SendReindex(ReindexConfiguration configuration)
        {
            using (LOG.InfoCall("SendReindex"))
            {
                return SendServerAction<ReindexConfiguration, ReindexSnapshot>(configuration);
            }
        }

        public Serialized<Snapshot> SendUpdateStatistics(UpdateStatisticsConfiguration configuration)
        {
            using (LOG.InfoCall("SendUpdateStatistics"))
            {
                return SendServerAction<UpdateStatisticsConfiguration, Snapshot>(configuration);
            }
        }

        public Serialized<Snapshot> SendMirroringPartnerAction(MirroringPartnerActionConfiguration configuration)
        {
            using (LOG.InfoCall("SendMirroringAction"))
            {
                return SendServerAction<MirroringPartnerActionConfiguration, Snapshot>(configuration);
            }
        }
        /// <summary>
        /// Get the preferred mirroring configuration from the repository
        /// </summary>
        /// <returns></returns>
        public Dictionary<Guid, MirroringSession> GetMirroringPreferredConfig()
        {
            using (LOG.InfoCall("GetMirroringPreferredConfig"))
            {
                try
                {
                    return RepositoryHelper.GetMirroringPreferredConfig(ManagementServiceConfiguration.ConnectionString);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repositoryConnectionString"></param>
        /// <param name="ReportName"></param>
        /// <param name="GraphNumber"></param>
        /// <param name="CounterName"></param>
        /// <param name="ShortDescription"></param>
        /// <param name="Aggregation"></param>
        /// <returns></returns>
        public bool InsertCounterToGraph(string repositoryConnectionString, string ReportName, int GraphNumber, string CounterName, string ShortDescription, int Aggregation, int Source)
        {
            bool result;
            using (LOG.InfoCall("InsertcounterToGraph"))
            {
                try
                {
                    result = RepositoryHelper.InsertCounterToGraph(ManagementServiceConfiguration.ConnectionString, ReportName, GraphNumber, CounterName, ShortDescription, Aggregation, Source);
                }
                catch (Exception e)
                {
                    LOG.Error("Error inserting custom report counter", e);
                    throw new ManagementServiceException("Error inserting custom report counter", e);
                }
                return result;
            }
        }

        public bool DeleteCustomReportCounters(string repositoryConnectionString, string ReportName)
        {

            using (LOG.InfoCall("DeleteCustomReportCounters"))
            {
                bool result;
                try
                {
                    RepositoryHelper.DeleteCustomReportCounters(ManagementServiceConfiguration.ConnectionString, ReportName);
                    result = true;
                }
                catch (Exception e)
                {
                    LOG.Error("Error deleting custom report counters", e);
                    throw new ManagementServiceException("Error deleting custom report counters", e);
                }
                return result;
            }
        }

        #region Audit

        /// <summary>
        /// Logs audit events based on a signle AuditEntity object
        /// </summary>
        /// <param name="repositoryConnectionString"></param>
        /// <param name="auditEntity"></param>
        /// <param name="typeId"></param>
        public void LogAuditEvent(AuditableEntity auditEntity, short typeId)
        {
            LogChangeEvent(auditEntity, typeId);
        }

        /// <summary>
        /// Logs audit events based on a signle AuditEntity object
        /// </summary>
        /// <param name="repositoryConnectionString"></param>
        /// <param name="auditEntity"></param>
        /// <param name="typeId"></param>
        public static void LogChangeEvent(AuditableEntity auditEntity, short typeId)
        {
            using (LOG.InfoCall("LogAuditEvent"))
            {
                try
                {
                    RepositoryHelper.LogAuditEvent(ManagementServiceConfiguration.ConnectionString, auditEntity, typeId);
                }
                catch (Exception e)
                {
                    string message = string.Format("An error occurred while inserting audit action: {0}.", auditEntity);
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }

        /// <summary>
        /// Returns all Audited Events
        /// </summary>
        /// <param name="repositoryConnectionString"></param>
        /// <returns></returns>
        public DataTable GetAuditEvents()
        {
            using (LOG.InfoCall("GetAuditEvents"))
            {
                try
                {
                    if (Management.CollectionServices.DefaultCollectionService == null)
                    {
                        throw new ManagementServiceException("The collection service interface is not available.");
                    }

                    return RepositoryHelper.GetAuditEvents(ManagementServiceConfiguration.ConnectionString);
                }
                catch (Exception e)
                {
                    string message = "Unable to retrieve audit events.";
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }

        public Dictionary<int, string> GetAuditHeaderTemplates()
        {
            return GetAuditingHeaderTemplates();
        }

        public static Dictionary<int, string> GetAuditingHeaderTemplates()
        {
            using (LOG.InfoCall("GetAuditHeaderTemplates"))
            {
                try
                {
                    if (Management.CollectionServices.DefaultCollectionService == null)
                    {
                        throw new ManagementServiceException("The collection service interface is not available.");
                    }

                    return RepositoryHelper.GetAuditHeaderTemplates(ManagementServiceConfiguration.ConnectionString);
                }
                catch (Exception e)
                {
                    string message = "Unable to retrieve audit header templates.";
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="reportID"></param>
        /// <param name="reportName"></param>
        /// <param name="shortDescription"></param>
        /// <param name="reportText"></param>
        public int UpdateCustomReport(CustomReport.Operation operation, int? reportID, string reportName, string shortDescription, Serialized<string> reportText, bool showTopServers)
        {
            using (LOG.InfoCall("UpdateCustomReport"))
            {
                int result;
                try
                {
                    reportName = RepositoryHelper.GetExistingCustomReportName(ManagementServiceConfiguration.ConnectionString,
                        reportName);

                    result = RepositoryHelper.UpdateCustomReports(ManagementServiceConfiguration.ConnectionString, operation, reportID, reportName, shortDescription, reportText, showTopServers);
                }
                catch (Exception e)
                {
                    LOG.Error("Error updating custom report ", e);
                    throw new ManagementServiceException("Error updating custom report", e);
                }
                return result;
            }
        }

        /// <summary>
        /// Save the preferred mirroring configuration to the server context
        /// </summary>
        /// <param name="session"></param>
        public void SetMirroringPreferredConfig(MirroringSession session)
        {
            using (LOG.InfoCall("SetMirroringPreferredConfig"))
            {
                try
                {
                    RepositoryHelper.SetMirroringPreferredConfig(
                        ManagementServiceConfiguration.ConnectionString, session);

                    Management.QueueDelegate(delegate ()
                    {
                        try
                        {
                            // push the change to the collection service 
                            ICollectionService collSvc = Management.CollectionServices.DefaultCollectionService;
                            //Save preferred config to the principal workload
                            collSvc.SaveMirrorPreferredConfig(session);
                        }
                        catch (Exception e)
                        {
                            LOG.Error(e);
                        }
                    });
                }
                catch (Exception e)
                {
                    LOG.Error("Error saving mirroring preferred configuration: ", e);
                    throw new ManagementServiceException("Error saving mirroring preferred configuration", e);
                }
                return;
            }
        }

        public Serialized<AdhocQuerySnapshot> SendAdhocQuery(AdhocQueryConfiguration configuration)
        {
            using (LOG.InfoCall("SendAdhocQuery"))
            {
                return SendServerAction<AdhocQueryConfiguration, AdhocQuerySnapshot>(configuration);
            }
        }

        #endregion

        #endregion

        #region Monitored SQL Server methods

        public DataTable GetAvailableSqlServerInstances()
        {
            using (LOG.InfoCall("GetAvailableSqlServerInstances"))
            {
                if (Management.CollectionServices.DefaultCollectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    //START : SQLdm 9.0 (Abhishek Joshi) -get the total instances from CWF and the network
                    DataTable availableInstances = Management.CollectionServices.DefaultCollectionService.GetAvailableSqlServerInstances();
                    List<Instance> cwfInstances = new List<Instance>();//instantiatine here so that null ref does not come if cwf call fails
                    try
                    {
                        cwfInstances = GetInstancesFromCWF();
                    }
                    catch
                    {
                        LOG.Error("error while pulling instances from cwf");
                    }

                    foreach (Instance instance in cwfInstances)
                    {
                        bool isDistinct = true;

                        foreach (DataRow dataRow in availableInstances.Rows)
                        {
                            string instanceName = dataRow[0].ToString();
                            string version = dataRow[4].ToString();

                            if (string.Compare(instance.Name.ToUpper(), instanceName.ToUpper(), true) == 0)
                            {
                                isDistinct = false;
                                break; //existing the inner loop if the server is found
                            }
                        }

                        if (isDistinct)
                        {
                            DataRow row = availableInstances.NewRow();
                            row["Name"] = instance.Name;
                            if (instance.Version != null)
                                row["Version"] = instance.Version;

                            availableInstances.Rows.Add(row);
                        }
                    }

                    return availableInstances;
                    //END : SQLdm 9.0 (Abhishek Joshi) -get the total instances from CWF and the network
                }
                catch (Exception e)
                {
                    string message = "Unable to retrieve available SQL Server Instances.";
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }

        public TestSqlConnectionResult TestSqlConnection(SqlConnectionInfo connectionInfo)
        {
            using (LOG.InfoCall("TestSqlConnection"))
            {
                if (connectionInfo == null)
                {
                    throw new ArgumentNullException("connectionInfo");
                }

                try
                {
                    LOG.DebugFormat("Connection test started for {0}.", connectionInfo.InstanceName);
                    TestSqlConnectionResult result =
                        Management.CollectionServices.DefaultCollectionService.TestSqlConnection(connectionInfo);
                    LOG.DebugFormat("Connection test completed for {0}.", connectionInfo.InstanceName);
                    return result;
                }
                catch (Exception e)
                {
                    string message = "Unable to test SQL connection. The collection service may not be available.";
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }

        private static void SendServerWorkload(MonitoredSqlServer instance)
        {
            Management.QueueDelegate(delegate ()
            {
                LOG.DebugFormat("Sending server workload for: {0}", instance.ConnectionInfo.InstanceName);
                bool result = Management.CollectionServices.StartMonitoringServer(instance);
                LOG.DebugFormat("Send server workload {0} {1}", instance.ConnectionInfo.InstanceName,
                             result ? "succeeded" : "failed");
            });
        }

        public ICollection<MonitoredSqlServer> AddMonitoredSqlServers(
            ICollection<MonitoredSqlServerConfiguration> configurations)
        {
            using (LOG.InfoCall("AddMonitoredSqlServers"))
            {
                if (configurations != null && configurations.Count > 0)
                {
                    List<MonitoredSqlServer> newServers = new List<MonitoredSqlServer>();

                    foreach (MonitoredSqlServerConfiguration configuration in configurations)
                    {
                        newServers.Add(AddMonitoredSqlServerInternal(configuration));
                    }

                    ThreadPool.QueueUserWorkItem(WebClient.WebClient.SafeKickoffAssetSynchronization, null);

                    return newServers;
                }
                else
                {
                    return null;
                }
            }
        }

        public MonitoredSqlServer AddMonitoredSqlServer(MonitoredSqlServerConfiguration configuration)
        {
            MonitoredSqlServer result = AddMonitoredSqlServerInternal(configuration);

            ThreadPool.QueueUserWorkItem(WebClient.WebClient.SafeKickoffAssetSynchronization, null);

            return result;
        }

        internal MonitoredSqlServer AddMonitoredSqlServerInternal(MonitoredSqlServerConfiguration configuration)
        {
            using (LOG.InfoCall("AddMonitoredSqlServer"))
            {
                if (configuration == null)
                {
                    throw new ArgumentNullException("configuration");
                }

                try
                {
                    // First, check the license and verify it will allow for one more server.
                    LicenseSummary license = LicenseHelper.CurrentLicense;
                    LOG.Verbose("Current license: ", license);

                    if (license.Status == LicenseStatus.OK && license.IsNotFull)
                    {
                        // License is OK for one more server.

                        MonitoredSqlServer instance = RepositoryHelper.AddMonitoredSqlServer(ManagementServiceConfiguration.ConnectionString, configuration);

                        MAuditingEngine.Instance.LogAction(new AddServer(instance),
                                                           new TagAction(instance.GetConfiguration(),
                                                                         AuditableActionType.AddServerToTag),
                                                           new VirtualizationAction(instance.GetConfiguration(), null));

                        if (instance != null)
                        {
                            Management.ScheduledCollection.AddMonitoredSqlServerState(instance);
                            // bump the number of licensed servers
                            license.MonitoredServers++;
                            LOG.Debug("Incremented monitored count to ", license.MonitoredServers);
                            // send the workload to the collection service
                            SendServerWorkload(instance);
                            Management.WriteEvent(0, Status.MonitoredServerAdded, Category.Audit, configuration.InstanceName);
                            return instance;
                        }
                        else
                        {
                            LOG.DebugFormat("No instance was added for instance configuration {0}.", configuration.InstanceName);
                            return null;
                        }
                    }
                    else
                    {
                        throw new ManagementServiceException(string.Format("Can't add SQL Server instance {0} due to license constraint.\n{1}", configuration.InstanceName, license));
                    }
                }
                catch (Exception e)
                {
                    string message =
                        string.Format("An error occurred while adding SQL Server instance {0}.",
                                      configuration.InstanceName);
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }

        public IEnumerable<MonitoredSqlServer> UpdateMonitoredSqlServers(IEnumerable<Pair<int, MonitoredSqlServerConfiguration>> configurations)
        {
            using (LOG.InfoCall("UpdateMonitoredSqlServers"))
            {
                List<MonitoredSqlServer> result = new List<MonitoredSqlServer>();
                if (configurations != null)
                {
                    foreach (Pair<int, MonitoredSqlServerConfiguration> configuration in configurations)
                    {
                        result.Add(InternalUpdateMonitoredSqlServer(configuration.First, configuration.Second, true));
                    }
                }
                return result;
            }
        }

        public MonitoredSqlServer UpdateMonitoredSqlServer(int id, MonitoredSqlServerConfiguration configuration)
        {
            using (LOG.InfoCall("UpdateMonitoredSqlServer"))
            {
                MonitoredSqlServer instance = InternalUpdateMonitoredSqlServer(id, configuration, true);
                SetScheduledTasks();
                return instance;
            }
        }

        internal static MonitoredSqlServer InternalUpdateMonitoredSqlServer(int id, MonitoredSqlServerConfiguration configuration, bool queueCollectionServiceUpdate)
        {
            using (LOG.InfoCall("InternalUpdateMonitoredSqlServer"))
            {
                if (configuration == null)
                {
                    throw new ArgumentNullException("configuration");
                }
                try
                {
                    // Do not save outdated query monitor settings
                    if (configuration.QueryMonitorConfiguration != null && configuration.QueryMonitorConfiguration.StopTimeUTC != null
                        && configuration.QueryMonitorConfiguration.StopTimeUTC.HasValue && configuration.QueryMonitorConfiguration.StopTimeUTC < DateTime.Now.ToUniversalTime())
                    {
                        configuration.QueryMonitorConfiguration.StopTimeUTC = null;
                        configuration.QueryMonitorConfiguration.IsAlertResponseQueryTrace = false;
                        configuration.QueryMonitorConfiguration.Enabled = false;
                    }

                    string connectString = ManagementServiceConfiguration.ConnectionString;
                    MonitoredSqlServer before = RepositoryHelper.GetMonitoredSqlServer(connectString, id);
                    //START SQLdm 9.1 (Ankit Srivastava ) -- Avoiding updating the default value to the database
                    if (before != null && before.QueryMonitorConfiguration != null && configuration != null && configuration.QueryMonitorConfiguration != null)
                    {
                        configuration.QueryMonitorConfiguration.FileSizeRolloverXe = before.QueryMonitorConfiguration.FileSizeRolloverXe;
                        configuration.QueryMonitorConfiguration.FileSizeXeMB = before.QueryMonitorConfiguration.FileSizeXeMB;
                    }

                    // DM Kit 1 Defect id - DE41950 (Biresh Kumar Mishra) - Custom counters are not implemented in Powershell. Until custom counter is implemented in Powershell, this code will avoid updating the default value to database.
					//SQLDM-29907. Unlink all custom counters from server properties.
                    if (before != null && before.CustomCounters != null && before.CustomCounters.Count > 0 && configuration != null && configuration.CustomCounters != null && configuration.CustomCounters.Count < 0)
                    {
                        configuration.CustomCounters = before.CustomCounters;
                    }

                    if (before != null && before.ActivityMonitorConfiguration != null && configuration != null && configuration.ActivityMonitorConfiguration != null)
                    {
                        configuration.ActivityMonitorConfiguration.FileSizeRolloverXe = before.ActivityMonitorConfiguration.FileSizeRolloverXe;
                        configuration.ActivityMonitorConfiguration.FileSizeXeMB = before.ActivityMonitorConfiguration.FileSizeXeMB;
                        configuration.ActivityMonitorConfiguration.RecordsPerRefreshXe = before.ActivityMonitorConfiguration.RecordsPerRefreshXe;
                        configuration.ActivityMonitorConfiguration.EventRetentionModeXe = before.ActivityMonitorConfiguration.EventRetentionModeXe;
                        configuration.ActivityMonitorConfiguration.MaxMemoryXeMB = before.ActivityMonitorConfiguration.MaxMemoryXeMB;
                        configuration.ActivityMonitorConfiguration.MaxEventSizeXemb = before.ActivityMonitorConfiguration.MaxEventSizeXemb;
                        configuration.ActivityMonitorConfiguration.MaxDispatchLatencyXe = before.ActivityMonitorConfiguration.MaxDispatchLatencyXe;
                        configuration.ActivityMonitorConfiguration.StartupStateXe = before.ActivityMonitorConfiguration.StartupStateXe;
                        configuration.ActivityMonitorConfiguration.TrackCausalityXe = before.ActivityMonitorConfiguration.TrackCausalityXe;
                    }

                    //END SQLdm 9.1 (Ankit Srivastava ) -- Avoiding updating the default value to the database
                    MonitoredSqlServer instance = RepositoryHelper.UpdateMonitoredSqlServer(connectString, id, configuration);
                    //SQLdm10.1 (Srishti Purohit) -- Update baseline list with current active template ids
                    instance.BaselineConfigurationList = Common.Data.BaselineHelpers.GetCustomBaselines(id, connectString);

                    Management.ScheduledCollection.UpdateMonitoredSqlServerState(instance);

                    if (instance != null)
                    {
                        bool mmChanged = instance.MaintenanceModeEnabled != before.MaintenanceModeEnabled;

                        if (mmChanged)
                        {
                            Management.ScheduledCollection.UpdateStatusDocument(id);
                            CreateMaintenanceModeAlerts(instance);
                        }

                        SendUpdatedServerWorkload(instance,
                                                  instance.MaintenanceModeEnabled && !before.MaintenanceModeEnabled,
                                                  queueCollectionServiceUpdate);

                        #region Change Log actions

                        MonitoredSqlServerConfiguration newConfig = instance.GetConfiguration();
                        MonitoredSqlServerConfiguration oldConfig = before.GetConfiguration();

                        MAuditingEngine.Instance.LogAction(new EditServerAction(newConfig, oldConfig),
                            new MaintenanceModeUpdatedAction(newConfig, oldConfig),
                            new BaselineConfigurationAction(newConfig, oldConfig),
                            new TagAction(newConfig, oldConfig, AuditableActionType.AddServerToTag),
                            new VirtualizationAction(newConfig, oldConfig),
                            new AnalysisConfigurationAction(newConfig, oldConfig));

                        #endregion Change Log actions

                        Management.WriteEvent(0, Status.MonitoredServerChanged, Category.Audit, configuration.InstanceName);
                        return instance;
                    }
                    else
                    {
                        LOG.DebugFormat("Unable to update instance {0}.", id);
                        return null;
                    }
                }
                catch (Exception e)
                {
                    string message =
                        string.Format("An error occurred while updating SQL Server instance {0}.",
                                      configuration.InstanceName);
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }

        private static void CreateMaintenanceModeAlerts(MonitoredSqlServer instance)
        {
            Management.QueueDelegate(delegate ()
            {
                LOG.DebugFormat("MaintenanceMode changed on {0} to {1}", instance.ConnectionInfo.InstanceName, instance.MaintenanceModeEnabled);
                AlertTableWriter.HandleMaintenanceModeAlerts(instance);
            });
        }

        private static void SendUpdatedServerWorkload(MonitoredSqlServer instance, bool sendStopTrace)
        {
            SendUpdatedServerWorkload(instance, sendStopTrace, true);
        }

        private static void SendUpdatedServerWorkload(MonitoredSqlServer instance, bool sendStopTrace, bool queue)
        {
            WorkQueueDelegate work = delegate ()
            {
                if (sendStopTrace)
                {
                    LOG.DebugFormat("Sending stop trace requres for: {0}", instance.ConnectionInfo.InstanceName);
                    Snapshot StopQMTrace = ManagementService.SendServerAction<StopQueryMonitorTraceConfiguration, Snapshot>(new StopQueryMonitorTraceConfiguration(instance.Id));
                }
                LOG.DebugFormat("Sending changed server workload for: {0}", instance.ConnectionInfo.InstanceName);
                bool result = Management.CollectionServices.UpdateMonitoredServer(instance);
                LOG.DebugFormat("Send changed server workload {0} {1}", instance.ConnectionInfo.InstanceName,
                             result ? "succeeded" : "failed");
            };

            if (queue)
                Management.QueueDelegate(work);
            else
                work.Invoke();
        }

        public void ActivateMonitoredSqlServer(int id)
        {
            using (LOG.InfoCall("ActivateMonitoredSqlServer")) // 
            {
                try
                {
                    // First, check the license and verify it will allow for one more server.
                    LicenseSummary license = LicenseHelper.CurrentLicense;
                    LOG.Verbose("Current license: ", license);

                    if (license.Status == LicenseStatus.OK && license.IsNotFull)
                    {
                        MonitoredSqlServer instance = RepositoryHelper.ActivateMonitoredSqlServer(ManagementServiceConfiguration.ConnectionString, id);

                        if (instance != null)
                        {
                            Management.ScheduledCollection.AddMonitoredSqlServerState(instance);
                            license.MonitoredServers++;
                            LOG.Debug("Incremented monitored count to ", license.MonitoredServers);
                            SendServerWorkload(instance);
                            Management.WriteEvent(0, Status.MonitoredServerAdded, Category.Audit, instance.InstanceName);

                            // sync asset list and permissions with pulse
                            Management.QueueDelegate(delegate () { WebClient.WebClient.SafeKickoffAssetSynchronization(null); });
                        }
                        else
                        {
                            LOG.DebugFormat("No instance was activated for instance id {0}", id);
                        }
                    }
                    else
                    {
                        throw new ManagementServiceException(string.Format("Can't activate SQL Server instance {0} due to license constraint.\n{1}", id, license));
                    }
                }
                catch (Exception e)
                {
                    string message = string.Format("An error occurred while activating SQL Server instance {0}.", id);
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }

        public void DeleteMonitoredSqlServers(ICollection<int> ids)
        {
            using (LOG.InfoCall("DeleteMonitoredSqlServers"))
            {
                if (ids != null && ids.Count > 0)
                {
                    foreach (int id in ids)
                    {
                        InternalDeleteMonitoredSqlServer(id);
                    }
                    Management.QueueDelegate(delegate () { WebClient.WebClient.SafeKickoffAssetSynchronization(ids); });
                }
            }
        }

        public void DeleteMonitoredSqlServer(int id)
        {
            if (InternalDeleteMonitoredSqlServer(id))
            {
                Management.QueueDelegate(delegate () { WebClient.WebClient.SafeKickoffAssetSynchronization(id); });
            }
        }

        private static bool InternalDeleteMonitoredSqlServer(int id)
        {
            using (LOG.InfoCall("DeleteMonitoredSqlServer")) // 
            {
                try
                {
                    if (RepositoryHelper.DeleteMonitoredSqlServer(ManagementServiceConfiguration.ConnectionString, id))
                    {
                        MAuditingEngine.Instance.LogAction(new AuditServerAction(id, true));
                        SendStopMonitoringRequest(id);
                        Management.WriteEvent(0, Status.MonitoredServerDeleted, Category.Audit, "ID=" + id);
                        LicenseHelper.CurrentLicense.MonitoredServers--;
                        LOG.Debug("Decremented monitored count to ", LicenseHelper.CurrentLicense.MonitoredServers);
                    }
                    else
                    {
                        LOG.Debug("RepositoryHelper.DeleteMonitoredSqlServer returned false, did not decrement monitored count.");
                        return false;
                    }
                }
                catch (Exception e)
                {
                    string message = string.Format("An error occurred while deleting SQL Server instance {0}.", id);
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
                return true;
            }
        }

        public void DeactivateMonitoredSqlServers(ICollection<int> ids)
        {
            using (LOG.InfoCall("DeactivateMonitoredSqlServers"))
            {
                if (ids != null && ids.Count > 0)
                {
                    foreach (int id in ids)
                    {
                        DeactivateMonitoredSqlServer(id);
                    }
                    Management.QueueDelegate(delegate () { WebClient.WebClient.SafeKickoffAssetSynchronization(ids); });
                }
            }
        }

        public void DeactivateMonitoredSqlServer(int id)
        {
            if (InternalDeactivateMonitoredSqlServer(id))
            {
                MAuditingEngine.Instance.LogAction(new AuditServerAction(id, false));

                Management.QueueDelegate(delegate () { WebClient.WebClient.SafeKickoffAssetSynchronization(id); });
            }
        }

        private bool InternalDeactivateMonitoredSqlServer(int id)
        {
            using (LOG.InfoCall("InternalDeactivateMonitoredSqlServer")) // 
            {
                try
                {
                    if (RepositoryHelper.DeactivateMonitoredSqlServer(ManagementServiceConfiguration.ConnectionString, id))
                    {
                        SendStopMonitoringRequest(id);
                        Management.WriteEvent(0, Status.MonitoredServerRemoved, Category.Audit, "ID=" + id);
                        LicenseHelper.CurrentLicense.MonitoredServers--;
                        LOG.Debug("Decremented monitored count to ", LicenseHelper.CurrentLicense.MonitoredServers);
                    }
                    else
                    {
                        LOG.Debug("RepositoryHelper.DeactivateMonitoredSqlServer returned false, did not decrement monitored count.");
                        return false;
                    }
                }
                catch (Exception e)
                {
                    string message = string.Format("An error occurred while deactiviating SQL Server instance {0}.", id);
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
                return true;
            }
        }

        private static void SendStopMonitoringRequest(int id)
        {
            Management.QueueDelegate(delegate ()
            {
                LOG.DebugFormat("Attempt to stop any tracing that may be going on for {0}", id);
                Snapshot StopQMTrace = ManagementService.SendServerAction<StopQueryMonitorTraceConfiguration, Snapshot>(new StopQueryMonitorTraceConfiguration(id));
                LOG.DebugFormat("Requesting to stop monitoring server {0}", id);
                bool result = Management.CollectionServices.StopMonitoringServer(id);
                LOG.DebugFormat("Request to stop monitoring server {0} {1}", id, result ? "succeeded" : "failed");
            });
        }
		
		//SQLDM-30197.
		public String GetPreferredClusterNode(int instanceId)
		{
			using (LOG.InfoCall("GetPreferredClusterNode"))
            {
                string instanceName = instanceId.ToString();
                ICollectionService collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName);

                if (collectionService == null)
                    collectionService = Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.GetCurrentClusterNode(instanceId);
                }
                catch (Exception e)
                {
                    string message = "Unable to retrieve current cluster node for " + instanceName;
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
		}
			

        public string GetCurrentClusterNode(int instanceId)
        {
            using (LOG.InfoCall("GetCurrentClusterNode"))
            {
                string instanceName = instanceId.ToString();
                ICollectionService collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName);

                if (collectionService == null)
                    collectionService = Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.GetCurrentClusterNode(instanceId);
                }
                catch (Exception e)
                {
                    string message = "Unable to retrieve current cluster node for " + instanceName;
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }

        public List<string> GetDisks(int instanceID)
        {
            List<string> result = null;

            using (LOG.InfoCall("GetDisks"))
            {
                try
                {
                    result = RepositoryHelper.GetDisks(instanceID);
                }
                catch
                {
                }
                if (result == null || result.Count == 0)
                {
                    LOG.Debug("No disk(s) found in respository, retrieving from the server");
                    string instanceName = instanceID.ToString();
                    ICollectionService collectionService =
                        Management.CollectionServices.GetCollectionServiceForServer(instanceID, out instanceName) ??
                        Management.CollectionServices.DefaultCollectionService;

                    if (collectionService == null)
                        throw new ManagementServiceException("This collection service interface is not available.");

                    try
                    {
                        return collectionService.GetDisks(instanceID);
                    }
                    catch (Exception e)
                    {
                        string message = "Unable to retrieve disk list for " + instanceName;
                        LOG.Error(message, e);
                        //throw new ManagementServiceException(message, e);
                    }
                }
            }

            return result;
        }

        public DataTable GetAzureDatabases(int instanceId)
        {
            using (LOG.InfoCall("GetAzureDatabases"))
            {
                string instanceName;
                var collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName) ??
                    Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.GetAzureDatabase(instanceId);
                }
                catch (Exception e)
                {
                    string message = "Unable to retrieve database list for " + instanceName;
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }

        public IDictionary<string, bool> GetDatabases(int instanceId, bool includeSystemDatabases, bool includeUserDatabases)
        {
            using (LOG.InfoCall("GetDatabases"))
            {
                string instanceName = instanceId.ToString();
                ICollectionService collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName);

                if (collectionService == null)
                    collectionService = Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.GetDatabases(instanceId, includeSystemDatabases, includeUserDatabases);
                }
                catch (Exception e)
                {
                    string message = "Unable to retrieve database list for " + instanceName;
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }

        public List<Triple<string, string, bool>> GetTables(int instanceId, string database, bool includeSystemTables, bool includeUserTables)
        {
            using (LOG.InfoCall("GetTables"))
            {
                string instanceName = instanceId.ToString();
                ICollectionService collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName);

                if (collectionService == null)
                    collectionService = Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.GetTables(instanceId, database, includeSystemTables, includeUserTables);
                }
                catch (Exception e)
                {
                    string message =
                        string.Format("Unable to retrieve table list for database '{0}' on {1}.", database, instanceName);
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }

        public IEnumerable<string> GetAgentJobNames(int instanceId)
        {
            using (LOG.InfoCall("GetAgentJobNames"))
            {
                string instanceName = instanceId.ToString();
                ICollectionService collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName);

                if (collectionService == null)
                    collectionService = Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.GetAgentJobNames(instanceId);
                }
                catch (Exception e)
                {
                    string message =
                        string.Format("Unable to retrieve available Agent Job Names on {0}.", instanceName);
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }


        public IEnumerable<string> GetAgentJobCategories(int instanceId)
        {
            using (LOG.InfoCall("GetAgentJobCategories"))
            {
                string instanceName = instanceId.ToString();
                ICollectionService collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName);

                if (collectionService == null)
                    collectionService = Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.GetAgentJobCategories(instanceId);
                }
                catch (Exception e)
                {
                    string message =
                        string.Format("Unable to retrieve available Agent Job Categories on {0}.", instanceName);
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }

        public IEnumerable<CategoryJobStep> GetAgentJobStepList(int instanceId)
        {
            using (LOG.InfoCall("GetAgentJobStepList"))
            {
                string instanceName = instanceId.ToString();
                ICollectionService collectionService = Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName);

                if (collectionService == null)
                    collectionService = Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                    throw new ManagementServiceException("The collection service interface is not available.");

                try
                {
                    return collectionService.GetAgentJobStepList(instanceId);
                }
                catch (Exception e)
                {
                    string message = string.Format("Unable to retrieve list of Agent jobs and their steps from instance {0}.", instanceName);
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }


        #endregion

        #region License Key methods

        //public void GetLicenseKeys(out int registeredServers, out IEnumerable<string> keyList)
        //{
        //    LicenseHelper.GetLicenseKeys(out registeredServers, out keyList);
        //}

        public LicenseSummary SetLicenseKeys(LicenseKeyOperation operation, IEnumerable<string> keyList)
        {
            return LicenseHelper.SetLicenseKeys(operation, keyList);
        }

        #endregion

        #region Notification Provider methods

        public IList<NotificationProviderInfo> GetNotificationProviders()
        {
            using (LOG.InfoCall("GetNotificationProviders"))
            {
                IPrincipal principal = Thread.CurrentPrincipal;

                return Management.Notification.GetNotificationProviders();
            }
        }

        public NotificationProviderInfo AddNotificationProvider(NotificationProviderInfo providerInfo)
        {
            using (LOG.InfoCall("AddNotificationProvider"))
            {
                MAuditingEngine.Instance.LogAction(new NotificationRulesActions(providerInfo, AuditableActionType.AddActionProvider));
                return Management.Notification.AddNotificationProvider(providerInfo, true);
            }
        }

        public void UpdateNotificationProvider(NotificationProviderInfo providerInfo, bool updateRules)
        {
            using (LOG.InfoCall("UpdateNotificationProvider"))
            {
                MAuditingEngine.Instance.LogAction(new NotificationRulesActions(providerInfo, AuditableActionType.EditActionProvider));
                Management.Notification.UpdateNotificationProvider(providerInfo, updateRules);
            }
        }

        public void DeleteNotificationProvider(Guid providerId)
        {
            using (LOG.InfoCall("DeleteNotificationProvider"))
            {
                MAuditingEngine.Instance.LogAction(new NotificationRulesActions(AuditableActionType.RemoveActionProvider));
                Management.Notification.DeleteNotificationProvider(providerId);
            }
        }

        public void ValidateDestination(NotificationDestinationInfo destination)
        {
            if (destination is ProgramDestination)
            {
                string path;
                string arguments;

                ProgramDestination pd = (ProgramDestination)destination;

                // split the command line into a path and program arguments
                ProgramDestination.ParseCommandLine(pd.Command, out path, out arguments);
                path = path.Replace("\"", "");

                path = System.IO.Path.GetFullPath(path);
                if (!System.IO.File.Exists(path))
                    throw new System.IO.FileNotFoundException(
                        string.Format("File {0} not accessible to the SQLDM Management Service.", path));

                path = System.IO.Path.GetFullPath(pd.StartIn.Replace("\"", ""));
                if (!string.IsNullOrEmpty(path))
                {
                    if (!System.IO.Directory.Exists(path))
                        throw new System.IO.FileNotFoundException(
                            string.Format("Start in directory {0} not accessible to the SQLDM Management Service.", path));
                }
            }
            else
            if (destination is JobDestination)
            {
            }
            else
            if (destination is SqlDestination)
            {
            }
        }

        /// <summary>
        /// Test the given action.  Returns 0 for good, anything else isn't good.  May chuck a variety of excetpions.
        /// </summary>
        public int TestAction(NotificationProviderInfo providerInfo, NotificationDestinationInfo destinationInfo, object data)
        {
            ValidateDestination(destinationInfo);
            if (destinationInfo is SnmpDestination)
            {
                SnmpNotificationProvider provider = null;
                NotificationContext context = new NotificationContext(null, destinationInfo, null, null);
                if (providerInfo == null)
                {
                    NotificationProviderContext providerContext = Management.Notification[destinationInfo.ProviderID];
                    provider = providerContext.Provider as SnmpNotificationProvider;
                }
                else
                {
                    provider = new SnmpNotificationProvider(providerInfo);
                }

                if (provider != null)
                {
                    if (provider.Send(context))
                        return 0;
                    if (context.LastSendException != null)
                        throw context.LastSendException;
                    return -1;
                }
                else
                    throw new ServiceException("Unable to create/locate SNMP Action Provider.  Please check the configuration of your SNMP Action Provider and try again.");
            }
            else
            if (destinationInfo is SmtpDestination)
            {
                SmtpNotificationProvider provider = null;
                NotificationContext context = new NotificationContext(null, destinationInfo, null, null);
                if (providerInfo == null)
                {
                    NotificationProviderContext providerContext = Management.Notification[destinationInfo.ProviderID];
                    provider = providerContext.Provider as SmtpNotificationProvider;
                }
                else
                {
                    provider = new SmtpNotificationProvider(providerInfo);
                }

                if (provider != null)
                {
                    if (provider.Send(context))
                        return 0;
                    if (context.LastSendException != null)
                        throw context.LastSendException;
                    return -1;
                }
                else
                    throw new ServiceException("Unable to create/locate SMTP Action Provider.  Please check the configuration of your SMTP Action Provider and try again.");
            }
            else
            if (destinationInfo is ProgramDestination)
            {
                ProgramNotificationProvider provider = null;
                NotificationContext context = new NotificationContext(null, destinationInfo, null, null);
                if (providerInfo == null)
                {
                    NotificationProviderContext providerContext = Management.Notification[destinationInfo.ProviderID];
                    provider = providerContext.Provider as ProgramNotificationProvider;
                }
                else
                {
                    provider = new ProgramNotificationProvider(providerInfo);
                }

                if (provider != null)
                {
                    if (provider.Send(context))
                        return 0;
                    if (context.LastSendException != null)
                        throw context.LastSendException;
                    return -1;
                }
                else
                    throw new ServiceException("Unable to create/locate Program Action Provider.  Please check the configuration of your Program Action Provider and try again.");
            }
            else
            if (destinationInfo is PulseDestination)
            {
                if (providerInfo != null)
                {
                    PulseNotificationProvider.Test((PulseNotificationProviderInfo)providerInfo);
                    return 0;
                }
                else
                    throw new ServiceException("Unable to create/locate Newsfeed Action Provider.  Please check the configuration of your Newsfeed Action Provider and try again.");
            }
            return -1;
        }

        #endregion

        #region Notification Rule methods

        public IList<NotificationRule> GetNotificationRules()
        {
            using (LOG.InfoCall("GetNotificationRules"))
            {
                return Management.Notification.GetNotificationRules();
            }
        }

        public NotificationRule AddNotificationRule(NotificationRule rule)
        {
            using (LOG.InfoCall("AddNotificationRule"))
            {
                return Management.Notification.AddNotificationRule(rule);
            }
        }

        public void UpdateNotificationRule(NotificationRule rule)
        {
            using (LOG.InfoCall("UpdateNotificationRule"))
            {
                Management.Notification.UpdateNotificationRule(rule);
            }
        }

        public void DeleteNotificationRule(Guid ruleId)
        {
            using (LOG.InfoCall("DeleteNotificationRule"))
            {
                Management.Notification.DeleteNotificationRule(ruleId);
            }
        }

        #endregion

        #region Threshold methods

        public IList<MetricThresholdEntry> GetDefaultMetricThresholds(int userViewID)
        {
            using (LOG.InfoCall("GetDefaultMetricThresholds"))
            {
                return RepositoryHelper.GetDefaultMetricThresholds(
                    ManagementServiceConfiguration.ConnectionString,
                    userViewID);
            }
        }

        public IList<MetricThresholdEntry> GetMetricThresholds(int monitoredServerID)
        {
            using (LOG.InfoCall("GetMetricThresholds"))
            {
                return RepositoryHelper.GetMetricThresholds(
                    ManagementServiceConfiguration.ConnectionString,
                    monitoredServerID);
            }
        }

        //        public void AddDefaultMetricThresholdEntries(IEnumerable<MetricThresholdEntry> entries)
        //        {
        //            using (LOG.DebugCall())
        //            {
        //                RepositoryHelper.AddDefaultMetricThresholdEntries(ManagementServiceConfiguration.ConnectionString, entries);
        //            }
        //        }

        public void ChangeAlertTemplateConfiguration(AlertConfiguration configuration)
        {
            using (LOG.InfoCall("ChangeAlertTemplateConfiguration"))
            {
                try
                {
                    // update the repository
                    RepositoryHelper.ChangeAlertTemplateConfiguration(
                        ManagementServiceConfiguration.ConnectionString,
                        configuration);
                }
                catch (Exception e)
                {
                    throw new ManagementServiceException("Unable to update alert configuration.", e);
                }

            }
        }


        public void ChangeAlertConfiguration(AlertConfiguration configuration)
        {
            using (LOG.InfoCall("ChangeAlertConfiguration"))
            {
                try
                {
                    // update the repository
                    RepositoryHelper.ChangeAlertConfiguration(ManagementServiceConfiguration.ConnectionString,
                                                              configuration);

                    Management.QueueDelegate(delegate ()
                    {
                        List<MetricThresholdEntry> changedThresholds = new List<MetricThresholdEntry>();
                        foreach (object o in configuration.ChangeItems)
                        {
                            if (o is MetricThresholdEntry)
                            {
                                changedThresholds.Add((MetricThresholdEntry)o);
                            }
                        }

                        if (changedThresholds.Count > 0)
                        {
                            int instanceId = configuration.InstanceID;
                            ICollectionService collectionService =
                                Management.CollectionServices.GetCollectionServiceForServer(instanceId);
                            collectionService.UpdateThresholdEntries(instanceId, changedThresholds);
                        }
                    });
                }
                catch (Exception e)
                {
                    throw new ManagementServiceException("Unable to update alert configuration.", e);
                }
            }
        }

        public void UpdateAlertConfigurations(IEnumerable<MetricThresholdEntry> changedItems, IEnumerable<int> targetInstances)
        {
            using (LOG.InfoCall("UpdateAlertConfigurations"))
            {
                try
                {
                    RepositoryHelper.UpdateAlertConfiguration(
                        ManagementServiceConfiguration.ConnectionString,
                        changedItems,
                        targetInstances
                        );

                    Management.QueueDelegate(delegate ()
                    {
                        InternalReplaceAlertConfiguration(changedItems, targetInstances);
                    });
                }
                catch (Exception e)
                {
                    throw new ManagementServiceException("Unable to replace alert configuration settings.", e);
                }
            }
        }

        public void ReplaceDefaultAlertConfiguration(IEnumerable<MetricThresholdEntry> configurationItems, int userViewID)
        {
            using (LOG.InfoCall("ReplaceDefaultAlertConfiguration"))
            {
                try
                {
                    RepositoryHelper.ReplaceAlertConfiguration(
                        ManagementServiceConfiguration.ConnectionString,
                        configurationItems,
                        new int[] { userViewID },
                        true
                        );
                }
                catch (Exception e)
                {
                    throw new ManagementServiceException("Unable to replace default alert configuration settings.", e);
                }
            }
        }
        public void AddAlertTemplate(int TemplateID, IEnumerable<int> targetInstances)
        {
            using (LOG.InfoCall("AddAlertTemplate"))
            {
                try
                {
                    //Audit Apply alert template to server
                    var monitoredSqlServer = RepositoryHelper.GetMonitoredSqlServers(ManagementServiceConfiguration.ConnectionString, null, true);
                    // MAuditingEngine.Instance.LogAction(new AlertTemplateAction(targetInstances, monitoredSqlServer));

                    RepositoryHelper.AddAlertTemplate(
                        ManagementServiceConfiguration.ConnectionString,
                        TemplateID,
                        targetInstances
                        );
                }
                catch (Exception e)
                {
                    throw new ManagementServiceException("Unable to add alert template settings.", e);
                }
            }
        }
        public void ReplaceAlertConfiguration(IEnumerable<MetricThresholdEntry> configurationItems, IEnumerable<int> targetInstances)
        {
            using (LOG.InfoCall("ReplaceAlertConfiguration"))
            {
                try
                {
                    //Audit Apply alert template to server
                    var monitoredSqlServer = RepositoryHelper.GetMonitoredSqlServers(ManagementServiceConfiguration.ConnectionString, null, true);
                    MAuditingEngine.Instance.LogAction(new AlertTemplateAction(targetInstances, monitoredSqlServer));

                    RepositoryHelper.ReplaceAlertConfiguration(
                        ManagementServiceConfiguration.ConnectionString,
                        configurationItems,
                        targetInstances,
                        false
                        );

                    Management.QueueDelegate(delegate ()
                        {
                            InternalReplaceAlertConfiguration(configurationItems, targetInstances);
                        });
                }
                catch (Exception e)
                {
                    throw new ManagementServiceException("Unable to replace alert configuration settings.", e);
                }
            }
        }

        internal static void InternalReplaceAlertConfiguration(IEnumerable<MetricThresholdEntry> configurationItems, IEnumerable<int> targetInstances)
        {
            using (LOG.InfoCall("InternalReplaceAlertConfiguration"))
            {
                foreach (int target in targetInstances)
                {
                    List<MetricThresholdEntry> changedThresholds = new List<MetricThresholdEntry>();
                    string instanceName = "<unknown>";
                    foreach (MetricThresholdEntry item in configurationItems)
                    {
                        item.MonitoredServerID = target;
                        changedThresholds.Add(item);
                    }
                    try
                    {
                        ICollectionService collectionService =
                            Management.CollectionServices.GetCollectionServiceForServer(target,
                                                                                        out instanceName);
                        LOG.DebugFormat(
                            "Pushing alert configuration change to collection service for {0}",
                            instanceName);

                        collectionService.UpdateThresholdEntries(target, changedThresholds);
                    }
                    catch (Exception e)
                    {
                        LOG.ErrorFormat(
                            "Error pushing alert configuration change to collection service for {0}. {1}",
                            instanceName, e.ToString());
                    }
                }
            }
        }

        public void SnoozeServersAlerts(IList<int> serverListId, int? metricId, int minutesToSnooze, string requestingUser)
        {
            foreach (int instanceId in serverListId)
            {
                SnoozeAlerts(instanceId, metricId, minutesToSnooze, requestingUser);
            }

            //audit Snooze all servers alerts
            var monitoredSqlServer = RepositoryHelper.GetMonitoredSqlServers(ManagementServiceConfiguration.ConnectionString, null, true);
            MAuditingEngine.Instance.LogAction(new SnoozeAlertsAction(serverListId, monitoredSqlServer, AuditableActionType.SnoozeAllServerAlerts));
        }

        public SnoozeInfo SnoozeAlerts(int instanceId, int? metricId, int minutesToSnooze, string requestingUser)
        {
            using (LOG.InfoCall("SnoozeAlerts"))
            {
                SnoozeInfo snoozeInfo = null;
                try
                {
                    snoozeInfo = RepositoryHelper.SnoozeAlerts(
                        ManagementServiceConfiguration.ConnectionString,
                        instanceId,
                        metricId,
                        minutesToSnooze,
                        requestingUser);

                    int[] metrics = metricId.HasValue ? new int[] { metricId.Value } : null;
                    Management.ScheduledCollection.UpdateStatusDocumentSnoozeInfo(instanceId, true, metrics);

                    MonitoredSqlServerState state = Management.ScheduledCollection.GetCachedMonitoredSqlServer(instanceId);
                    if (state != null)
                    {
                        state.UpdateSnoozeInfo(metrics, snoozeInfo);
                        Management.QueueDelegate(delegate ()
                        {
                            WriteSnoozeAlert(state.WrappedServer, metricId, minutesToSnooze, requestingUser);
                        });
                    }

                    var isSimpleSnooze = MAuditingEngine.Instance.PopAuxiliarData("isSimpleSnooze") as AuditAuxiliar<bool>;
                    if (isSimpleSnooze != null && isSimpleSnooze.Data)
                    {
                        if (metricId != null)
                        {
                            MAuditingEngine.Instance.LogAction(new SnoozeAlertsAction(AuditableActionType.SingeAlertSnooze));
                        }
                        else
                        {
                            MAuditingEngine.Instance.LogAction(new SnoozeAlertsAction(AuditableActionType.MultipleAlertsSnoozed));
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new ManagementServiceException("Unable to update snooze information in the SQLDM Repository.", e);
                }
                return snoozeInfo;
            }
        }

        private void WriteSnoozeAlert(MonitoredSqlServer instance, int? metricId, int minutesToSnooze, string requestingUser)
        {
            string header;
            string body;

            if (metricId == null)
            {
                header = string.Format("All alerts are snoozing for {0} minutes", minutesToSnooze);
                body = string.Format("All alerts on '{0}' were snoozed for {1} minutes by {2}", instance.InstanceName, minutesToSnooze, requestingUser);
            }
            else
            {
                MetricDefinitions defs = Management.GetMetricDefinitions();
                MetricDescription? desc = defs.GetMetricDescription(metricId.Value);
                string metricName = metricId.Value.ToString();
                if (desc != null)
                    metricName = desc.Value.Name;

                header = string.Format("Alert {0} is snoozing for {1} minute(s)", metricName, minutesToSnooze);
                body = string.Format("Alert {0} on {1} was snoozing for {2} minutes by {3}.", metricName, instance.InstanceName, minutesToSnooze, requestingUser);
            }

            LOG.Info(body);

            AlertTableWriter.LogOperationalAlerts(Metric.Operational,
                                                  new MonitoredObjectName(instance),
                                                  MonitoredState.OK,
                                                  header,
                                                  body);

        }

        public void UnSnoozeServersAlerts(IList<int> serverListId, int[] metrics, string requestingUser)
        {
            //call UnSnooze method all servers
            foreach (int instanceId in serverListId)
            {
                UnSnoozeAlerts(instanceId, metrics, requestingUser);
            }

            //call method audit Unsnooze all servers alerts
            var monitoredSqlServer = RepositoryHelper.GetMonitoredSqlServers(ManagementServiceConfiguration.ConnectionString, null, true);
            MAuditingEngine.Instance.LogAction(new SnoozeAlertsAction(serverListId, monitoredSqlServer, AuditableActionType.ResumeAllServerAlerts));
        }

        public SnoozeInfo UnSnoozeAlerts(int instanceId, int[] metrics, string requestingUser)
        {
            using (LOG.InfoCall("UnSnoozeAlerts"))
            {
                SnoozeInfo snoozeInfo = null;
                try
                {
                    snoozeInfo = RepositoryHelper.UnsnoozeAlerts(
                        ManagementServiceConfiguration.ConnectionString,
                        instanceId,
                        metrics,
                        requestingUser);

                    #region Change Log

                    MAuditingEngine.Instance.LogAction(new SnoozeAlertsAction(AuditableActionType.AlertsUnSnoozed));

                    #endregion Change Log

                    Management.ScheduledCollection.UpdateStatusDocumentSnoozeInfo(instanceId, false, metrics);

                    MonitoredSqlServerState state = Management.ScheduledCollection.GetCachedMonitoredSqlServer(instanceId);
                    if (state != null)
                    {
                        state.UpdateSnoozeInfo(metrics, snoozeInfo);
                        Management.QueueDelegate(delegate ()
                        {
                            WriteUnSnoozeAlert(state.WrappedServer, metrics, requestingUser);
                        });
                    }
                }
                catch (Exception e)
                {
                    throw new ManagementServiceException("Unable to replace default alert configuration settings.", e);
                }
                return snoozeInfo;
            }
        }

        private void WriteUnSnoozeAlert(MonitoredSqlServer instance, int[] metrics, string requestingUser)
        {
            string header;
            string body;


            if (metrics == null || metrics.Length == 0)
            {
                header = string.Format("All alerts resumed");
                body = string.Format("All alerts on '{0}' were resumed by {1}", instance.InstanceName, requestingUser);
            }
            else
            {
                int c = 0;
                MetricDefinitions defs = Management.GetMetricDefinitions();
                StringBuilder builder = new StringBuilder();
                foreach (int metricId in metrics)
                {
                    MetricDescription? desc = defs.GetMetricDescription(metricId);
                    string metricName = metricId.ToString();
                    if (desc != null)
                        metricName = desc.Value.Name;

                    if (builder.Length > 0)
                        builder.Append(", ");

                    if (++c > 3)
                    {
                        builder.AppendLine();
                        c = 1;
                    }

                    builder.Append(metricName);
                }

                header = string.Format("{0} alert(s) resumed", metrics.Length);
                body = string.Format("The following alerts on {0} were resumed by {1}:\r\n{2}", instance.InstanceName, requestingUser, builder);
            }

            LOG.Info(body);

            AlertTableWriter.LogOperationalAlerts(Metric.Operational,
                                                  new MonitoredObjectName(instance),
                                                  MonitoredState.OK,
                                                  header,
                                                  body);
        }

        #endregion

        #region Task methods

        public void UpdateTask(int taskID, TaskStatus status, string owner, string comments)
        {
            try
            {
                RepositoryHelper.UpdateTask(ManagementServiceConfiguration.ConnectionString, taskID, status, owner, comments);
            }
            catch (Exception e)
            {
                string message = string.Format("An error occurred while updating task id {0}.", taskID);
                LOG.Error(message, e);
                throw new ManagementServiceException(message, e);
            }
        }

        public void DeleteTask(IEnumerable<int> taskIDs)
        {
            try
            {
                RepositoryHelper.DeleteTasks(ManagementServiceConfiguration.ConnectionString, taskIDs);
            }
            catch (Exception e)
            {
                string message = string.Format("An error occurred while deleting tasks.");
                LOG.Error(message, e);
                throw new ManagementServiceException(message, e);
            }
        }

        #endregion

        #region Alert methods

        public void ClearActiveAlerts(long alertID, bool allAlerts)
        {
            lock (Management.ScheduledCollection.AlertTableSyncRoot)
            {
                object[] clearInfo = null;
                try
                {
                    using (SqlConnection connection = new SqlConnection(ManagementServiceConfiguration.ConnectionString))
                    {
                        connection.Open();
                        using (SqlTransaction xa = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                        {
                            try
                            {
                                clearInfo = RepositoryHelper.ClearAlerts(xa, null, 0, allAlerts, null, alertID);
                                xa.Commit();
                            }
                            catch
                            {
                                try { xa.Rollback(); }
                                catch { /* */ }
                                throw;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    string message =
                        string.Format("An error occurred while trying to clear {0}active alerts.",
                                      allAlerts ? "all " : "");
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
                // update the state graph for the server with current timestamp
                if (clearInfo != null)
                {
                    int instanceId = (int)clearInfo[0];
                    MonitoredSqlServerState state =
                        Management.ScheduledCollection.GetCachedMonitoredSqlServer(instanceId);
                    state.StateGraph.ManualClearEvent((int)clearInfo[1], (string)clearInfo[3], (string)clearInfo[4],
                                                      allAlerts);


                    Management.QueueDelegate(delegate ()
                        {
                            try
                            {
                                ICollectionService collSvc =
                                    Management.CollectionServices.GetCollectionServiceForServer(instanceId);
                                if (collSvc == null)
                                    throw new ArgumentException(
                                        string.Format("Unable to get interface for monitored server {0}",
                                                      instanceId));

                                MonitoredObjectName objectName = null;
                                if (!allAlerts)
                                {
                                    objectName = new MonitoredObjectName((string)clearInfo[2],
                                                                         (string)clearInfo[3],
                                                                         null,
                                                                         (string)clearInfo[4]);
                                }

                                collSvc.ClearEventState(instanceId,
                                                        (int)clearInfo[1],
                                                        state.StateGraph.LastAlertRefreshTime,
                                                        objectName);
                            }
                            catch (Exception e)
                            {
                                LOG.Error("Error trying to clear events in the collection service: ", e);
                            }
                        });
                }
            }
        }

        #endregion

        #region Grooming methods

        public void UpdateGrooming(GroomingConfiguration configuration)
        {
            using (LOG.InfoCall("UpdateGrooming"))
            {
                try
                {
                    GroomingConfiguration oldValue = GetGrooming();
                    RepositoryHelper.UpdateGrooming(ManagementServiceConfiguration.ConnectionString, configuration);

                    GroomingConfiguration newValue = GetGrooming();
                    MAuditingEngine.Instance.LogAction(new GroomingAction(newValue, oldValue, AuditableActionType.GroomingConfigurationChanged));
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public void CreateGroomJob()
        {
            using (LOG.InfoCall("CreateGroomJob"))
            {
                try
                {
                    RepositoryHelper.CreateGroomJob(ManagementServiceConfiguration.ConnectionString);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public void GroomNow()
        {
            using (LOG.InfoCall("GroomNow"))
            {
                try
                {
                    RepositoryHelper.GroomNow(ManagementServiceConfiguration.ConnectionString);
                    MAuditingEngine.Instance.LogAction(new GroomingAction());
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public void AggregateNow()
        {
            using (LOG.InfoCall("AggregateNow"))
            {
                try
                {
                    RepositoryHelper.AggregateNow(ManagementServiceConfiguration.ConnectionString);
                    MAuditingEngine.Instance.LogAction(new AggregateAction());
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public GroomingConfiguration GetGrooming()
        {
            using (LOG.InfoCall("GetGrooming"))
            {
                try
                {
                    return RepositoryHelper.GetGrooming(ManagementServiceConfiguration.ConnectionString);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public void SetQueryAggregationFlag(long signatureID, bool doNotAggregate)
        {
            using (LOG.InfoCall("SetQueryAggregationFlag"))
            {
                try
                {
                    RepositoryHelper.SetQueryAggregationFlag(ManagementServiceConfiguration.ConnectionString, signatureID, doNotAggregate);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        #endregion

        #region Tag methods

        public int AddTag(string name)
        {
            using (LOG.InfoCall("AddTag"))
            {
                return TagManager.AddTag(name);
            }
        }

        public int UpdateTagConfiguration(Tag tag)
        {
            using (LOG.InfoCall("UpdateTagConfiguration"))
            {
                Tag oldTag = RepositoryHelper.GetTagById(ManagementServiceConfiguration.ConnectionString, tag.Id);

                List<int> changedPermissions;

                // Get old permisions configuration.
                bool permissionHasChanged = tag.ComparePermissions(oldTag, out changedPermissions);
                List<PermissionDefinition> oldPermissions = new List<PermissionDefinition>();
                AuditableEntity contextEntity = CallContext.GetData(AuditableEntity.Context) as AuditableEntity;

                if (permissionHasChanged && contextEntity != null)
                {
                    foreach (int permission in changedPermissions)
                    {
                        // Deploy old permissions.
                        PermissionDefinition permissionDef = RepositoryHelper.GetPermissionDefinitionById(permission,
                            ManagementServiceConfiguration.ConnectionString);
                        oldPermissions.Add(permissionDef);
                    }
                }

                #region Change Log

                MAuditingEngine.Instance.LogAction(new TagAction(tag, oldTag));

                #endregion Change Log

                int result = TagManager.UpdateTagConfiguration(tag);

                // Log changes in the AuditingEngine.
                if (permissionHasChanged && contextEntity != null)
                {
                    // Deploy new permissions.
                    foreach (PermissionDefinition oldPermissionDef in oldPermissions)
                    {
                        PermissionDefinition permissionDef = RepositoryHelper.GetPermissionDefinitionById(
                            oldPermissionDef.PermissionID, ManagementServiceConfiguration.ConnectionString);

                        if (permissionDef != null)
                        {
                            MAuditableEntity entity = new MAuditableEntity(permissionDef.GetAuditableEntity(oldPermissionDef));
                            entity.SqlUser = contextEntity.SqlUser;
                            entity.Workstation = contextEntity.Workstation;
                            entity.WorkstationUser = contextEntity.WorkstationUser;
                            string login = permissionDef.Login;
                            entity.SetHeaderParam(login);
                            entity.Name = login;
                            MAuditingEngine.Instance.LogAction(entity, AuditableActionType.ApplicationSecurityEditUserAccount);
                        }
                    }
                }

                return result;
            }
        }

        public void RemoveTags(IList<int> tagIds)
        {
            using (LOG.InfoCall("RemoveTags"))
            {
                MAuditingEngine.Instance.LogAction(new TagAction(tagIds, AuditableActionType.DeleteTag));

                TagManager.RemoveTags(tagIds);
            }
        }

        /// <summary>
        /// SQLdm 10.1 - Praveen Suhalka - CWF 3 Integration
        /// </summary>
        /// <returns></returns>
        public ICollection<Common.CWFDataContracts.GlobalTag> GetGlobalTags()
        {

            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            CommonWebFramework cwfDetails = CommonWebFramework.GetInstance();
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by CommonWebFramework.GetInstance : {0}", stopWatch.ElapsedMilliseconds);
            var cwfHelper = new CWFHelper(cwfDetails);
            stopWatch.Reset();
            stopWatch.Start();
            ICollection<Idera.SQLdm.Common.CWFDataContracts.GlobalTag> tags = cwfHelper.GetGlobalTags();
            stopWatch.Stop();
            StartUpTimeLog.DebugFormat("Time taken by cwfHelper.GetGlobalTags : {0}", stopWatch.ElapsedMilliseconds);
            //return cwfHelper.GetGlobalTags();
            return tags;
        }

        #endregion

        #region QueryMonitor methods

        public Pair<long, string> GetQuerySignatureText(long signatureID)
        {
            using (LOG.InfoCall("GetQuerySignatureText"))
            {
                try
                {
                    return RepositoryHelper.GetQueryText(ManagementServiceConfiguration.ConnectionString, null, null, null,
                                                  signatureID);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Pair<long, string> GetQuerySignatureText(string signatureHash)
        {
            using (LOG.InfoCall("GetQuerySignatureText"))
            {
                try
                {
                    return RepositoryHelper.GetQueryText(ManagementServiceConfiguration.ConnectionString, null, null, signatureHash, null);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Pair<long, string> GetQueryStatementText(long statementID)
        {
            using (LOG.InfoCall("GetQueryStatementText"))
            {
                try
                {
                    return RepositoryHelper.GetQueryText(ManagementServiceConfiguration.ConnectionString, null, statementID, null, null);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        public Pair<long, string> GetQueryStatementText(string statementHash)
        {
            using (LOG.InfoCall("GetQueryStatementText"))
            {
                try
                {
                    return RepositoryHelper.GetQueryText(ManagementServiceConfiguration.ConnectionString, statementHash, null, null, null);
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }


        #endregion

        public string GetMonitoredSQLServerStatusDocument()
        {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                string doc = string.Empty;
                doc = Management.ScheduledCollection.MonitoredSQLServerStatusDocumentForceUpdate;
                stopWatch.Stop();
                StartUpTimeLog.DebugFormat("Time taken by GetMonitoredSQLServerStatusDocument : {0}",stopWatch.ElapsedMilliseconds);
                return doc;
        }

        public string ForceScheduledRefresh(int monitoredSqlServerID)
        {
            string result = null;

            using (LOG.InfoCall("ForceScheduledRefresh"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(monitoredSqlServerID);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          monitoredSqlServerID));

                    using (OnDemandCollectionContext<object> context = new OnDemandCollectionContext<object>())
                    {
                        // start collecting the data
                        collSvc.ForceScheduledCollection(monitoredSqlServerID, context, null);
                        // wait for the request to complete
                        context.Wait();
                    }
                    // SQLDM-29437: Reverting SQLDM-28938 due the side effect of high amount of memory
                    XmlDocument status = Management.ScheduledCollection.GetCachedStatusDocument(monitoredSqlServerID);

                    if (status != null)
                        result = status.OuterXml;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
                return result;
            }
        }

        //sqldm-30244 start
        public DateTime GetDateTime(int id,DateTime dt)
        {
            ICollectionService collsvc = Management.CollectionServices.GetCollectionServiceForServer(id);
            return collsvc.GetDateTime(dt);
        }
        //sqldm-30244 end

        public Serialized<DataTable> GetSysPerfInfoObjectList(int instanceId)
        {
            using (LOG.InfoCall("GetSysPerfInfoObjectList"))
            {
                string instanceName = instanceId.ToString();
                ICollectionService collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName);

                if (collectionService == null)
                    collectionService = Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.GetSysPerfInfoObjectList(instanceId);
                }
                catch (Exception e)
                {
                    LOG.Error(e);
                    throw;
                }
            }
        }

        public Serialized<DataTable> GetSysPerfInfoCounterList(int instanceId, string objectName)
        {
            using (LOG.InfoCall("GetSysPerfInfoCounterList"))
            {
                string instanceName = instanceId.ToString();
                ICollectionService collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName);

                if (collectionService == null)
                    collectionService = Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.GetSysPerfInfoCounterList(instanceId, objectName);
                }
                catch (Exception e)
                {
                    LOG.Error(e);
                    throw;
                }
            }
        }

        public Serialized<DataTable> GetSysPerfInfoInstanceList(int instanceId, string objectName)
        {
            using (LOG.InfoCall("GetSysPerfInfoInstanceList"))
            {
                string instanceName = instanceId.ToString();
                ICollectionService collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName);

                if (collectionService == null)
                    collectionService = Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.GetSysPerfInfoInstanceList(instanceId, objectName);
                }
                catch (Exception e)
                {
                    LOG.Error(e);
                    throw;
                }
            }
        }

        public IAzureProfile GetAzureProfile(long profileId, string resourceUri)
        {
            using (LOG.InfoCall("GetAzureProfile"))
            {
                try
                {
                    return RepositoryHelper.GetAzureProfile(ManagementServiceConfiguration.ConnectionString, profileId, resourceUri);
                }
                catch (Exception e)
                {
                    LOG.Error("GetAzureProfile:", e);
                    throw;
                }
            }
        }

        public Serialized<DataTable> GetAzureMonitorDefinitions(int instanceId,
            IMonitorManagementConfiguration monitorConfiguration)
        {
            using (LOG.InfoCall("GetAzureMonitorDefinitions"))
            {
                var instanceName = instanceId.ToString();
                var collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName) ??
                    Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.GetAzureMonitorDefinitions(instanceId, monitorConfiguration);
                }
                catch (Exception e)
                {
                    LOG.Error("Management GetAzureMonitorDefinitions: " + e);
                    // no need to wrap a CollectionServiceException 
                    if (e is CollectionServiceException)
                        throw;
                    throw new Exception(
                        string.Format(
                            "Unable to retrieve list of Azure Monitor Definitions for instance {0}-{1} using {2}",
                            instanceId,
                            monitorConfiguration != null && monitorConfiguration.Profile != null &&
                            monitorConfiguration.Profile.ApplicationProfile != null
                                ? monitorConfiguration.Profile.ApplicationProfile.Name
                                : null,
                            monitorConfiguration != null && monitorConfiguration.MonitorParameters != null &&
                            monitorConfiguration.MonitorParameters.Resource != null
                                ? monitorConfiguration.MonitorParameters.Resource.Type + " " +
                                  monitorConfiguration.MonitorParameters.Resource.Uri
                                : null),
                        new Exception(e.Message));
                }
            }
        }

        public List<IAzureResource> GetAzureApplicationResources(IMonitorManagementConfiguration configuration)
        {
            using (LOG.InfoCall("GetAzureApplicationResources"))
            {
                try
                {
                    var azureClient = new AzureManagementClient
                    {
                        Configuration = configuration
                    };
                    return azureClient.GetResources().GetAwaiter().GetResult().ToList();
                }
                catch (Exception e)
                {
                    LOG.Error("Management GetAzureApplicationResources: " + e);
                    // no need to wrap a CollectionServiceException 
                    if (e is CollectionServiceException)
                        throw;
                    throw new Exception(
                        string.Format(
                            "Unable to retrieve list of Azure Monitor Resources for {0} using {1}",
                            configuration != null && configuration.Profile != null &&
                            configuration.Profile.ApplicationProfile != null
                                ? configuration.Profile.ApplicationProfile.Name
                                : null,
                            configuration != null && configuration.MonitorParameters != null &&
                            configuration.MonitorParameters.Resource != null
                                ? configuration.MonitorParameters.Resource.Type + " " +
                                  configuration.MonitorParameters.Resource.Uri
                                : null),
                        new Exception(e.Message));
                }
            }
        }

        public List<AzureSqlModel> GetFilteredAzureApplicationResources(IMonitorManagementConfiguration configuration)
        {
            using (LOG.InfoCall("GetFilteredAzureApplicationResources"))
            {
                try
                {
                    var azureClient = new AzureManagementClient
                    {
                        Configuration = configuration
                    };
                    return azureClient.GetFilteredAzureApplicationResources().GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    LOG.Error("Management GetFilteredAzureApplicationResources: " + e);
                    // no need to wrap a CollectionServiceException 
                    if (e is CollectionServiceException)
                        throw;
                    throw new Exception(
                        string.Format(
                            "Unable to retrieve fully qualified names of Azure Monitor Resources for {0} using {1}",
                            configuration != null && configuration.Profile != null &&
                            configuration.Profile.ApplicationProfile != null
                                ? configuration.Profile.ApplicationProfile.Name
                                : null,
                            configuration != null && configuration.MonitorParameters != null &&
                            configuration.MonitorParameters.Resource != null
                                ? configuration.MonitorParameters.Resource.Type + " " +
                                  configuration.MonitorParameters.Resource.Uri
                                : null),
                        new Exception(e.Message));
                }
            }
        }

        public Serialized<DataTable> GetAzureMonitorNamespaces(int instanceId,
            IMonitorManagementConfiguration monitorConfiguration)
        {
            using (LOG.InfoCall("GetAzureMonitorNamespaces"))
            {
                var instanceName = instanceId.ToString();
                var collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName) ??
                    Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.GetAzureMonitorNamespaces(instanceId, monitorConfiguration);
                }
                catch (Exception e)
                {
                    LOG.Error("Management GetAzureMonitorNamespaces: " + e);
                    // no need to wrap a CollectionServiceException 
                    if (e is CollectionServiceException)
                        throw;
                    throw new Exception(
                        string.Format(
                            "Unable to retrieve list of Azure Monitor Namespaces for instance {0}-{1} using {2}",
                            instanceId,
                            monitorConfiguration != null && monitorConfiguration.Profile != null &&
                            monitorConfiguration.Profile.ApplicationProfile != null
                                ? monitorConfiguration.Profile.ApplicationProfile.Name
                                : null,
                            monitorConfiguration != null && monitorConfiguration.MonitorParameters != null &&
                            monitorConfiguration.MonitorParameters.Resource != null
                                ? monitorConfiguration.MonitorParameters.Resource.Type + " " +
                                  monitorConfiguration.MonitorParameters.Resource.Uri
                                : null),
                        new Exception(e.Message));
                }
            }
        }

        public Serialized<Pair<string, DataTable>> GetWmiObjectList(int instanceId, WmiConfiguration wmiConfiguration)
        {
            using (LOG.InfoCall("GetWmiObjectList"))
            {
                string instanceName = instanceId.ToString();
                ICollectionService collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName);

                if (collectionService == null)
                    collectionService = Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.GetWmiObjectList(instanceId, wmiConfiguration);
                }
                catch (Exception e)
                {
                    LOG.Error(e);
                    throw;
                }
            }
        }

        public Serialized<DataTable> GetWmiCounterList(int instanceId, string serverName, string objectName, WmiConfiguration wmiConfiguration)
        {
            using (LOG.InfoCall("GetWmiCounterList"))
            {
                string instanceName = instanceId.ToString();
                ICollectionService collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName);

                if (collectionService == null)
                    collectionService = Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.GetWmiCounterList(serverName, objectName, wmiConfiguration);
                }
                catch (Exception e)
                {
                    LOG.Error(e);
                    throw;
                }
            }
        }

        public Serialized<DataTable> GetWmiInstanceList(int instanceId, string serverName, string objectName, WmiConfiguration wmiConfiguration)
        {
            using (LOG.InfoCall("GetWmiInstanceList"))
            {
                string instanceName = instanceId.ToString();
                ICollectionService collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName);

                if (collectionService == null)
                    collectionService = Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.GetWmiInstanceList(serverName, objectName, wmiConfiguration);
                }
                catch (Exception e)
                {
                    LOG.Error(e);
                    throw;
                }
            }
        }

        public Serialized<DataTable> GetVmCounterObjectList(int instanceId)
        {
            using (LOG.InfoCall("GetVmCounterObjectList"))
            {
                string instanceName = instanceId.ToString();
                ICollectionService collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName);

                if (collectionService == null)
                    collectionService = Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.GetVmCounterObjectList(instanceId);
                }
                catch (Exception e)
                {
                    LOG.Error(e);
                    throw;
                }
            }
        }

        public object TestCustomCounter(int instanceId, CustomCounterDefinition counterDefinition)
        {
            using (LOG.InfoCall("TestCustomCounter"))
            {
                string instanceName = instanceId.ToString();
                ICollectionService collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName);

                if (collectionService == null)
                    collectionService = Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.TestCustomCounter(instanceId, counterDefinition);
                }
                catch (Exception e)
                {
                    LOG.Error(e);
                    throw;
                }
            }
        }

        public int AddCustomCounter(MetricDefinition metricDefinition, MetricDescription metricDescription, CustomCounterDefinition counterDefinition, bool alertOnFailureToCollect)
        {
            using (LOG.InfoCall("AddCustomCounter"))
            {
                int metricID = -1;
                try
                {
                    metricID = RepositoryHelper.AddCustomCounter(
                        ManagementServiceConfiguration.ConnectionString,
                        metricDefinition,
                        metricDescription,
                        counterDefinition,
                        alertOnFailureToCollect);

                    if (metricID != -1)
                    {
                        counterDefinition.MetricID = metricID;
                        metricDefinition.MetricID = metricID;

                        Management.QueueDelegate(delegate ()
                            {
                                try
                                {
                                    // add the objects to the MS cache of metric data
                                    MetricDefinitions definitions = Management.GetMetricDefinitions();
                                    definitions.AddCounter(metricDefinition, metricDescription, counterDefinition);
                                    // push the custom counter definition to the collection service (all it needs is the counter definition)
                                    ICollectionService collSvc = Management.CollectionServices.DefaultCollectionService;
                                    collSvc.AddCustomCounter(counterDefinition);
                                }
                                catch (Exception e)
                                {
                                    LOG.Error(e);
                                }
                            });
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("Error adding custom counter: ", e);
                    throw new ManagementServiceException(string.Format("Error adding custom counter {0}", metricDescription.Name), e);
                }
                return metricID;
            }
        }

        public void UpdateCustomCounter(MetricDefinition metricDefinition, MetricDescription metricDescription, CustomCounterDefinition counterDefinition, bool alertOnFailureToCollect)
        {
            using (LOG.InfoCall("UpdateCustomCounter"))
            {
                try
                {
                    RepositoryHelper.UpdateCustomCounter(
                        ManagementServiceConfiguration.ConnectionString,
                        metricDefinition,
                        metricDescription,
                        counterDefinition);

                    IList<MetricThresholdEntry> thresholdEntries = RepositoryHelper.GetDefaultMetricThresholds(ManagementServiceConfiguration.ConnectionString, 0, metricDefinition.MetricID);
                    if (thresholdEntries.Count == 1)
                    {
                        // keep the default alert configuration sync'd with the counter definition
                        AlertConfiguration config = new AlertConfiguration(0);
                        config.MetricDefinitions = new MetricDefinitions(false, false, false);
                        config.MetricDefinitions.AddCounter(metricDefinition, metricDescription, counterDefinition);

                        config.AddEntry(metricDefinition.MetricID, metricDescription, thresholdEntries[0]);
                        AlertConfigurationItem item = config[metricDefinition.MetricID, string.Empty];  // Will need to update this if any of the metrics used here support multi-thresholds

                        FlattenedThreshold threshold = item.FlattenedThresholds[1];
                        object value = threshold.Value;
                        if (value == null || !metricDefinition.DefaultInfoThresholdValue.Equals(value))
                        {
                            threshold.Value =
                                Convert.ToInt64(metricDefinition.DefaultInfoThresholdValue);
                        }

                        threshold = item.FlattenedThresholds[2];
                        value = threshold.Value;
                        if (value == null || !metricDefinition.DefaultWarningThresholdValue.Equals(value))
                        {
                            threshold.Value =
                                Convert.ToInt64(metricDefinition.DefaultWarningThresholdValue);
                        }

                        threshold = item.FlattenedThresholds[3];
                        value = threshold.Value;
                        if (value == null || !metricDefinition.DefaultCriticalThresholdValue.Equals(value))
                        {
                            threshold.Value =
                                Convert.ToInt64(metricDefinition.DefaultCriticalThresholdValue);
                        }
                        if (item.Enabled != metricDefinition.AlertEnabledByDefault)
                            item.Enabled = metricDefinition.AlertEnabledByDefault;

                        // update comparison type
                        Threshold.Operator op = Threshold.Operator.GE;
                        if (metricDefinition.ComparisonType != ComparisonType.GE)
                            op = Threshold.Operator.LE;
                        bool updateComparison = item.ThresholdEntry.WarningThreshold.Op != op;

                        if (updateComparison)
                        {
                            item.ThresholdEntry.InfoThreshold.Op = op;
                            item.ThresholdEntry.WarningThreshold.Op = op;
                            item.ThresholdEntry.CriticalThreshold.Op = op;
                        }

                        bool advancedOption = false;
                        object data = item.ThresholdEntry.Data;
                        if (data is bool)
                            advancedOption = (bool)data;
                        if (advancedOption != alertOnFailureToCollect)
                            item.SetData(alertOnFailureToCollect);

                        if (config.IsChanged)
                        {
                            config.PrepareChangedItems();
                            RepositoryHelper.ChangeAlertTemplateConfiguration(
                                ManagementServiceConfiguration.ConnectionString,
                                config);
                        }

                        Management.QueueDelegate(delegate ()
                        {
                            try
                            {
                                // replace the objects in the MS cache of metric data
                                MetricDefinitions definitions = Management.GetMetricDefinitions();
                                definitions.AddCounter(metricDefinition, metricDescription, counterDefinition);
                                // push the changes to the collection service
                                ICollectionService collSvc = Management.CollectionServices.DefaultCollectionService;
                                collSvc.UpdateCustomCounter(counterDefinition);

                                if (updateComparison)
                                {
                                    // special hack for pushing replaced thresholds when the comparison type gets changed
                                    MetricThresholdEntry[] changedThresholds = new MetricThresholdEntry[1];
                                    foreach (MetricThresholdEntry entry in RepositoryHelper.GetMetricThresholds(
                                                                                ManagementServiceConfiguration.ConnectionString,
                                                                                null,
                                                                                null,
                                                                                metricDefinition.MetricID))
                                    {
                                        changedThresholds[0] = entry;
                                        string instanceName = string.Empty;
                                        try
                                        {

                                            ICollectionService collectionService =
                                                Management.CollectionServices.GetCollectionServiceForServer(entry.MonitoredServerID,
                                                                                                            out instanceName);
                                            LOG.DebugFormat(
                                                "Pushing alert configuration change to collection service for {0}",
                                                instanceName);

                                            collectionService.UpdateThresholdEntries(entry.MonitoredServerID, changedThresholds);
                                        }
                                        catch (Exception e)
                                        {
                                            LOG.ErrorFormat(
                                                "Error pushing alert configuration change to collection service for {0}. {1}",
                                                instanceName, e.ToString());
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                LOG.Error(e);
                            }
                        });
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("Error updating custom counter: ", e);
                    throw new ManagementServiceException(string.Format("Error updating custom counter {0}", metricDescription.Description), e);
                }
            }
        }

        public void UpdateCustomCounterStatus(int metricId, bool enabled)
        {
            using (LOG.InfoCall("UpdateCustomCounterStatus"))
            {
                try
                {
                    RepositoryHelper.UpdateCustomCounterStatus(ManagementServiceConfiguration.ConnectionString, metricId, enabled);
                    // update the cached counter definition with the new status setting
                    CustomCounterDefinition ccd = Management.GetMetricDefinitions().GetCounterDefinition(metricId);
                    if (ccd != null)
                        ccd.IsEnabled = enabled;

                    Management.QueueDelegate(delegate ()
                        {
                            try
                            {
                                // push the changes to the collection service
                                ICollectionService collSvc = Management.CollectionServices.DefaultCollectionService;
                                collSvc.UpdateCustomCounterStatus(metricId, enabled);
                            }
                            catch (Exception e)
                            {
                                LOG.Error(e);
                            }
                        });
                }
                catch (Exception e)
                {
                    LOG.Error("Error updating status of custom counter: ", e);
                    throw new ManagementServiceException("Error updating status of custom counter: ", e);
                }
            }
        }

        public void DeleteCustomCounter(int metricId)
        {
            using (LOG.InfoCall("DeleteCustomCounter"))
            {
                try
                {
                    RepositoryHelper.DeleteCustomCounter(ManagementServiceConfiguration.ConnectionString, metricId);
                    // set the delete flag in the cached metric definition
                    MetricDefinition md = Management.GetMetricDefinitions().GetMetricDefinition(metricId);
                    if (md != null)
                        md.IsDeleted = true;

                    Management.QueueDelegate(delegate ()
                        {
                            try
                            {
                                ICollectionService collSvc = Management.CollectionServices.DefaultCollectionService;
                                collSvc.DeleteCustomCounter(metricId);
                            }
                            catch (Exception e)
                            {
                                LOG.Error(e);
                            }
                        });
                }
                catch (Exception e)
                {
                    LOG.Error("Error deleting custom counter: ", e);
                    throw new ManagementServiceException("Error deleting custom counter: ", e);
                }
            }
        }

        public void AddCounterToServers(int metricId, IEnumerable<int> tags, IEnumerable<int> monitoredSqlServers)
        {
            using (LOG.InfoCall("AddCounterToServers"))
            {
                try
                {
                    string connectionString = ManagementServiceConfiguration.ConnectionString;
                    RepositoryHelper.AddCounterToServer(connectionString, metricId, tags, monitoredSqlServers, true);

                    #region Change Log

                    MAuditingEngine.Instance.LogAction(new AddCounterToTagAction());

                    #endregion Change Log

                    List<MetricThresholdEntry> thresholds = new List<MetricThresholdEntry>();
                    try
                    {
                        // build a list of the threshold entry for this metric for all the specified servers    
                        foreach (int id in RepositoryHelper.GetMonitoredServersUsingCounter(connectionString, metricId))
                        {
                            MetricThresholdEntry threshold =
                                RepositoryHelper.GetMetricThreshold(ManagementServiceConfiguration.ConnectionString, id, metricId);
                            if (threshold != null)
                            {
                                thresholds.Add(threshold);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Error building custom counter sync list: ", e);
                    }

                    Management.QueueDelegate(delegate ()
                        {
                            // all the info required by the collection service to start monitoring the counter on the instance
                            // is contained in the threshold entries.  We should be sending a complete list of monitored sql servers 
                            // for this metric.  The counter will be removed from any servers not contained in monitoredSqlServers.
                            try
                            {
                                ICollectionService collSvc = Management.CollectionServices.DefaultCollectionService;
                                // push the thresholds to the collection service
                                collSvc.AddCounterToServers(thresholds, true);
                            }
                            catch (Exception e)
                            {
                                LOG.Error(e);
                            }
                        });

                    try
                    {
                        // adjust internal state
                        Management.ScheduledCollection.SyncThresholds(metricId, thresholds);
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Error updating custom counter status: ", e);
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("Error updating custom counter: ", e);
                    throw;
                    // new ManagementServiceException(String.Format("Error updating custom counter {0}", metricDescription.Description), e);
                }
            }
        }

        public Serialized<DataTable> GetDriveConfiguration(SqlConnectionInfo connectionInfo, WmiConfiguration wmiConfiguration)
        {
            using (LOG.InfoCall("GetDriveConfiguration"))
            {
                ICollectionService collectionService = Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.GetDriveConfiguration(connectionInfo, wmiConfiguration);
                }
                catch (Exception e)
                {
                    LOG.Error(e);
                    throw;
                }
            }
        }
        public int GetPulseApplicationId()
        {
            if (WebClient.WebClient.Application == null)
                throw new ApplicationException("Either SQLDM is not registered with the IDERA News Service or the SQLDM Management Service is unable to connect to the Idera News Service.");

            return WebClient.WebClient.Application.Id;
        }

        public string GetPulseServerName()
        {
            PulseNotificationProviderInfo info = WebClient.WebClient.GetPulseProviderInfo();
            if (info == null)
                return null;

            return info.PulseServer;
        }

        #region IPredictiveAnalytics Members

        public Dictionary<int, List<int>> GetServerAlerts()
        {
            try
            {
                return RepositoryHelper.GetServerAlerts(ManagementServiceConfiguration.ConnectionString);
            }
            catch (Exception e)
            {
                string message = "An error occurred in GetServerAlerts";
                LOG.Error(message, e);
                throw new ManagementServiceException(message, e);
            }
        }

        public DataTable GetPredictiveModelServers()
        {
            try
            {
                return RepositoryHelper.GetPredictiveModelServers(ManagementServiceConfiguration.ConnectionString);
            }
            catch (Exception e)
            {
                string message = "An error occurred in GetPredictiveModelServers.";
                LOG.Error(message, e);
                throw new ManagementServiceException(message, e);
            }
        }

        public Dictionary<Triple<int, int, int>, byte[]> GetPredictiveModelsForServer(int serverid)
        {
            try
            {
                return RepositoryHelper.GetPredictiveModelsForServer(ManagementServiceConfiguration.ConnectionString, serverid);
            }
            catch (Exception e)
            {
                string message = string.Format("An error occurred in GetPredictiveModelsForServer [{0}].", serverid);
                LOG.Error(message, e);
                throw new ManagementServiceException(message, e);
            }
        }

        public DataTable GetPredictiveModelInput(int serverid, int intervalMinutes, DateTime cutoffDateTime)
        {
            try
            {
                return RepositoryHelper.GetPredictiveModelInput(ManagementServiceConfiguration.ConnectionString, serverid, intervalMinutes, cutoffDateTime);
            }
            catch (Exception e)
            {
                string message = string.Format("An error occurred in GetPredictiveModelInput [{0}, {1}, {2}].", serverid, intervalMinutes, cutoffDateTime);
                LOG.Error(message, e);
                throw new ManagementServiceException(message, e);
            }
        }

        public Pair<DataTable, List<DateTime>> GetPredictiveTrainingData(int serverid, int metricid, int severity, int timeframe, DateTime cutoffDateTime)
        {
            try
            {
                return RepositoryHelper.GetPredictiveTrainingData(ManagementServiceConfiguration.ConnectionString, serverid, metricid, severity, timeframe, cutoffDateTime);
            }
            catch (Exception e)
            {
                string message = string.Format("An error occurred in GetPredictiveTrainingData [{0}, {1}, {2}, {3}, {4}].", serverid, metricid, severity, timeframe, cutoffDateTime);
                LOG.Error(message, e);
                throw new ManagementServiceException(message, e);
            }
        }

        public void SavePredictiveModel(int serverid, int metricid, int severity, int timeframe, byte[] modelDataBuffer)
        {
            try
            {
                RepositoryHelper.SavePredictiveModel(ManagementServiceConfiguration.ConnectionString, serverid, metricid, severity, timeframe, modelDataBuffer);
            }
            catch (Exception e)
            {
                string message = string.Format("An error occurred in SavePredictiveModel [{0}, {1}, {2}, {3}].", serverid, metricid, severity, timeframe);
                LOG.Error(message, e);
                throw new ManagementServiceException(message, e);
            }
        }

        public void SavePredictiveForecast(int serverid, int metricid, int severity, int timeframe, int forecast, double accuracy, DateTime expiration)
        {
            try
            {
                RepositoryHelper.SavePredictiveForecast(ManagementServiceConfiguration.ConnectionString, serverid, metricid, severity, timeframe, forecast, accuracy, expiration);
            }
            catch (Exception e)
            {
                string message = string.Format("An error occurred in SavePredictiveForecast [{0}, {1}, {2}, {3}, {4}, {5}, {6}].", serverid, metricid, severity, timeframe, forecast, accuracy, expiration);
                LOG.Error(message, e);
                throw new ManagementServiceException(message, e);
            }
        }

        public void GroomExpiredForecasts()
        {
            try
            {
                RepositoryHelper.GroomExpiredForecasts(ManagementServiceConfiguration.ConnectionString);
            }
            catch (Exception e)
            {
                string message = "An error occurred in GroomExpiredForecasts.";
                LOG.Error(message, e);
                throw new ManagementServiceException(message, e);
            }
        }

        public void SetPredictiveAnalyticsEnabled(bool predictiveAnalyticsEnabled)
        {
            try
            {
                RepositoryHelper.SetPredictiveAnalyticsEnabled(ManagementServiceConfiguration.ConnectionString, predictiveAnalyticsEnabled);

                MainService.WriteEvent(Management.EventLog,
                                       EventLogEntryType.Information,
                                       predictiveAnalyticsEnabled ? Status.PredictiveAnalyticsEnabled : Status.PredictiveAnalyticsDisabled,
                                       Category.General,
                                       "SQLDM Predictive Analytics has been updated.");

                MAuditingEngine.Instance.LogPredictiveAnalyticsAction(predictiveAnalyticsEnabled);
            }
            catch (Exception e)
            {
                string message = "An error occurred in SetPredictiveAnalyticsEnabled.";
                LOG.Error(message, e);
                throw new ManagementServiceException(message, e);
            }
        }

        public bool GetPredictiveAnalyticsEnabled()
        {
            try
            {
                return RepositoryHelper.GetPredictiveAnalyticsEnabled(ManagementServiceConfiguration.ConnectionString);
            }
            catch (Exception e)
            {
                string message = "An error occurred in GetPredictiveAnalyticsEnabled.";
                LOG.Error(message, e);
                throw new ManagementServiceException(message, e);
            }
        }

        public void SetNextPredictiveAnalyticsModelRebuild(DateTime nextRebuild)
        {
            try
            {
                RepositoryHelper.SetNextPredictiveAnalyticsModelRebuild(ManagementServiceConfiguration.ConnectionString, nextRebuild);
            }
            catch (Exception e)
            {
                string message = "An error occurred in SetNextPredictiveAnalyticsModelRebuild.";
                LOG.Error(message, e);
                throw new ManagementServiceException(message, e);
            }
        }

        public DateTime GetNextPredictiveAnalyticsModelRebuild()
        {
            try
            {
                return RepositoryHelper.GetNextPredictiveAnalyticsModelRebuild(ManagementServiceConfiguration.ConnectionString);
            }
            catch (Exception e)
            {
                string message = "An error occurred in GetNextPredictiveAnalyticsModelRebuild.";
                LOG.Error(message, e);
                throw new ManagementServiceException(message, e);
            }
        }

        public void SetNextPredictiveAnalyticsForecast(DateTime nextForecast)
        {
            try
            {
                RepositoryHelper.SetNextPredictiveAnalyticsForecast(ManagementServiceConfiguration.ConnectionString, nextForecast);
            }
            catch (Exception e)
            {
                string message = "An error occurred in SetNextPredictiveAnalyticsForecast.";
                LOG.Error(message, e);
                throw new ManagementServiceException(message, e);
            }
        }

        public DateTime GetNextPredictiveAnalyticsForecast()
        {
            try
            {
                return RepositoryHelper.GetNextPredictiveAnalyticsForecast(ManagementServiceConfiguration.ConnectionString);
            }
            catch (Exception e)
            {
                string message = "An error occurred in GetNextPredictiveAnalyticsForecast.";
                LOG.Error(message, e);
                throw new ManagementServiceException(message, e);
            }
        }

        public bool GetPredictiveAnalyticsHasModels()
        {
            try
            {
                return RepositoryHelper.GetPredictiveAnalyticsHasModels(ManagementServiceConfiguration.ConnectionString);
            }
            catch (Exception e)
            {
                string message = "An error occurred in GetPredictiveAnalyticsHasModels.";
                LOG.Error(message, e);
                throw new ManagementServiceException(message, e);
            }
        }

        #endregion

        #region Wait Type Definitions

        private static Dictionary<string, WaitTypeInfo> waitTypes; // key-> wait_type, values: [category id, category name, wait defintion]
        private static readonly object waitTypeUpdateLock = new object();

        internal static Dictionary<string, WaitTypeInfo> GetWaitTypes()
        {
            lock (waitTypeUpdateLock)
            {
                // if we haven't gotten it yet, then go get them
                if (waitTypes == null)
                {
                    waitTypes = new Dictionary<string, WaitTypeInfo>();
                    DataTable data = RepositoryHelper.GetWaitTypeDefinitions();

                    string waittype = string.Empty;
                    int categoryid = 0;
                    string categoryname = string.Empty;
                    string description = string.Empty;
                    string helplink = string.Empty;

                    foreach (DataRow row in data.Rows)
                    {
                        waittype = (string)row["WaitType"];
                        categoryname = (string)row["Category"];
                        categoryid = -1;
                        description = string.Empty;
                        helplink = string.Empty;

                        if (!(row["CategoryID"] is DBNull)) categoryid = (int)row["CategoryID"];
                        if (!(row["Description"] is DBNull)) description = (string)row["Description"];
                        if (!(row["HelpLink"] is DBNull)) helplink = (string)row["HelpLink"];

                        if (!waitTypes.ContainsKey(waittype))
                            waitTypes.Add(waittype, new WaitTypeInfo(waittype, categoryid, categoryname, description, helplink));
                    }
                }
            }

            return waitTypes;
        }

        #endregion

        //Start - SQLdm 9.0 -(Ankit Srivastava) - added new method to get the timed out instances
        public KeyValuePair<bool, string> GetGroomingTimedOutState(int sqlServerId)
        {
            string connectString = ManagementServiceConfiguration.ConnectionString;
            return RepositoryHelper.GetGroomingTimedOutState(connectString, sqlServerId);
        }
        //End - SQLdm 9.0 -(Ankit Srivastava) - added new method to get the timed out instances


        //START : SQLdm 9.0 (Abhishek Joshi) -get the CWF Web URL for advanced query views
        public string GetCWFWebURL()
        {
            return CommonWebFramework.GetInstance().WebUIURL;
        }
        //END : SQLdm 9.0 (Abhishek Joshi) -get the CWF Web URL for advanced query views

        //[START] SQLdm 10.0 (Rajesh Gupta): LM 2.0 Integration - get the CWF Base URL for Desktop Client
        public string GetCWFBaseURL()
        {
            return CommonWebFramework.GetInstance().BaseURL;
        }
        //[END] SQLdm 10.0 (Rajesh Gupta): LM 2.0 Integration   - get the CWF Base URL for Desktop Client

        //[START] SQLdm 10.0 (Rajesh Gupta): LM 2.0 Integration - get the CWF ProductID for Desktop Client
        public string GetCWFProductID()
        {
            return CommonWebFramework.GetInstance().ProductID.ToString();
        }
        //[END] SQLdm 10.0 (Rajesh Gupta): LM 2.0 Integration   - get the CWF ProductID for Desktop Client


        //[START] SQLdm 10.0 (Rajesh Gupta): LM 2.0 Integration - get the CWF Base URL for Desktop Client
        public void WriteToRegistry(string path, string keyname, string value)
        {
            RegistryHelper.WriteToRegistry(path, keyname, value);
        }
        //[END] SQLdm 10.0 (Rajesh Gupta): LM 2.0 Integration   - get the CWF Base URL for Desktop Client

        //[START] SQLdm 10.0 (Rajesh Gupta): LM 2.0 Integration - Writing LM Registry Keys
        public void RegisterLicenseManager()
        {
            LMHelper.RegisterLicenseManager();
        }
        //[END] SQLdm 10.0 (Rajesh Gupta): LM 2.0 Integration - Writing LM Registry Keys

        //START : SQLdm 9.0 (Abhishek Joshi) -get the instance list from CWF
        public List<Instance> GetInstancesFromCWF()
        {
            CWFHelper cwfHelper = new CWFHelper(CommonWebFramework.GetInstance());
            List<Instance> cwfInstanceList = cwfHelper.GetAllProductInstancesFromCWF();
            return cwfInstanceList;
        }
        //END : SQLdm 9.0 (Abhishek Joshi) -get the instance list from CWF

        //START : SQLdm 9.1 (Abhishek Joshi) -Get a list of WebUIURL, WebURI, registered instance name and query statistics Id (applicable to sql statements)
        public List<string> GetWebUIQueryInfo()
        {
            List<string> result = new List<string>();
            CommonWebFramework instance = CommonWebFramework.GetInstance();
            result.Add(instance.WebUIURL);
            result.Add(instance.WebURI);
            result.Add(instance.RegisteredInstanceName);
            return result;
        }
        //END : SQLdm 9.1 (Abhishek Joshi) -Get a list of WebUIURL, WebURI, registered instance name and query statistics Id (applicable to sql statements)

        //START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --get the disk size details
        public Serialized<DiskSizeDetails> GetDiskSizeDetails(DatabaseProbeConfiguration configuration, WmiConfiguration wmiConfig)
        {
            Serialized<DiskSizeDetails> snapshot = null;

            using (LOG.InfoCall("GetDiskSizeDetails"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collSvc == null)
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          configuration.MonitoredServerId));

                    using (OnDemandCollectionContext<DiskSizeDetails> context = new OnDemandCollectionContext<DiskSizeDetails>())
                    {
                        // start collecting the data
                        collSvc.GetDiskSizeDetails(configuration, wmiConfig, context, null);
                        // wait for the request to complete
                        snapshot = context.Wait();
                    }
                    return snapshot;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }
        //END : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --get the disk size details

        //START : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --get the filegroups of a SQL server
        public IList<string> GetFilegroups(int instanceId, string databaseName, bool isDefaultThreshold)
        {
            using (LOG.InfoCall("GetFilegroups"))
            {
                string instanceName = instanceId.ToString();
                ICollectionService collectionService =
                    Management.CollectionServices.GetCollectionServiceForServer(instanceId, out instanceName);

                if (collectionService == null)
                    collectionService = Management.CollectionServices.DefaultCollectionService;

                if (collectionService == null)
                {
                    throw new ManagementServiceException("The collection service interface is not available.");
                }

                try
                {
                    return collectionService.GetFilegroups(instanceId, databaseName, isDefaultThreshold);
                }
                catch (Exception e)
                {
                    string message = "Unable to retrieve filegroups list for " + instanceName;
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }
        //END : SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --get the filegroups of a SQL server


        /// <summary>
        /// SQLdm 10.0 vineet kumar -- doctor integration-- get prescriptive optimize script
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        public string GetPrescriptiveOptimizeScript(int monitoredSqlServerId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation)
        {
            string script = string.Empty;
            using (LOG.InfoCall("GetPrescriptiveOptimizeScript"))
            {
                try
                {

                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(monitoredSqlServerId);

                    if (collSvc == null)
                    {
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          monitoredSqlServerId));
                    }
                    script = collSvc.GetPrescriptiveOptimizeScript(monitoredSqlServerId, recommendation);
                    return script;

                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// SQLdm 10.0 srishti purohit-- doctor integration-- get prescriptive optimization messages --GetMessages
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        public List<string> GetPrescriptiveOptimizeMessages(int monitoredSqlServerId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation)
        {
            List<string> messages = new List<string>();
            using (LOG.InfoCall("GetPrescriptiveOptimizeMessages"))
            {
                try
                {

                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(monitoredSqlServerId);

                    if (collSvc == null)
                    {
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          monitoredSqlServerId));
                    }
                    messages = collSvc.GetPrescriptiveOptimizeMessages(monitoredSqlServerId, recommendation);
                    return messages;

                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// SQLdm 10.0 srishti purohit-- doctor integration-- get prescriptive optimization messages --GetMessages
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        public List<string> GetPrescriptiveUndoMessages(int monitoredSqlServerId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation)
        {
            List<string> messages = new List<string>();
            using (LOG.InfoCall("GetPrescriptiveUndoMessages"))
            {
                try
                {

                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(monitoredSqlServerId);

                    if (collSvc == null)
                    {
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          monitoredSqlServerId));
                    }
                    messages = collSvc.GetPrescriptiveUndoMessages(monitoredSqlServerId, recommendation);
                    return messages;

                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }
        /// <summary>
        /// SQLdm 10.0 vineet kumar -- doctor integration-- get prescriptive analysis snapshots
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        public string GetPrescriptiveUndoScript(int monitoredSqlServerId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation)
        {
            string script = string.Empty;
            using (LOG.InfoCall("GetPrescriptiveUndoScript"))
            {
                try
                {

                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(monitoredSqlServerId);

                    if (collSvc == null)
                    {
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          monitoredSqlServerId));
                    }
                    script = collSvc.GetPrescriptiveUndoScript(monitoredSqlServerId, recommendation);
                    return script;

                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// SQLdm 10.0 Praveen Suhalka -- doctor integration-- get monitored server connection string
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        public string GetConnectionStringForServer(int monitoredSqlServerId)
        {
            string connStr = string.Empty;
            using (LOG.InfoCall("GetConnectionStringForServer"))
            {
                try
                {

                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(monitoredSqlServerId);

                    if (collSvc == null)
                    {
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          monitoredSqlServerId));
                    }
                    connStr = collSvc.GetConnectionStringForServer(monitoredSqlServerId);
                    return connStr;

                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// SQLdm 10.0 praveen suhalka -- doctor integration-- get databases name and DBIDs for a server 
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        public Dictionary<int, string> GetDatabasesForServer(int monitoredSqlServerId)
        {
            Dictionary<int, string> databseNameAndID = null;
            using (LOG.InfoCall("GetDatabasesForServer"))
            {
                try
                {

                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(monitoredSqlServerId);

                    if (collSvc == null)
                    {
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          monitoredSqlServerId));
                    }
                    using (OnDemandCollectionContext<Snapshot> context = new OnDemandCollectionContext<Snapshot>())
                    {
                        collSvc.GetDatabasesForServer(monitoredSqlServerId, context, null);
                        DatabaseNamesSnapshot snap = (DatabaseNamesSnapshot)context.Wait();
                        databseNameAndID = snap.Databases;
                    }
                    return databseNameAndID;

                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }

        }

        /// <summary>
        ///  SQLdm 10.0 praveen suhalka -- doctor integration-- get machine name from collection service
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        public string GetMachineName(int monitoredSqlServerId)
        {
            string machineName = null;
            using (LOG.InfoCall("GetMachineName"))
            {
                try
                {

                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(monitoredSqlServerId);

                    if (collSvc == null)
                    {
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          monitoredSqlServerId));
                    }
                    using (OnDemandCollectionContext<Snapshot> context = new OnDemandCollectionContext<Snapshot>())
                    {
                        collSvc.GetMachineName(monitoredSqlServerId, context, null);
                        MachineNameSnapshot snap = (MachineNameSnapshot)context.Wait();
                        machineName = snap.MachineName;
                    }
                    return machineName;

                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// 10.0 vineet -- integrate doctor -- Execute Optimize/Undo script
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        public PrescriptiveOptimizationStatusSnapshot ExecutePrescriptiveOptimization(int monitoredSqlServerId, PrescriptiveScriptConfiguration configuration)
        {
            Snapshot snapshot = null;
            using (LOG.InfoCall("GetPrescriptiveUndoScript"))
            {
                try
                {

                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(monitoredSqlServerId);

                    if (collSvc == null)
                    {
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          monitoredSqlServerId));
                    }
                    using (OnDemandCollectionContext<Snapshot> context = new OnDemandCollectionContext<Snapshot>())
                    {
                        DatabaseProbeConfiguration dbConfig = new DatabaseProbeConfiguration(monitoredSqlServerId);
                        collSvc.ExecutePrescriptiveOptimization(monitoredSqlServerId, context, null, configuration);
                        snapshot = context.Wait();
                    }
                    return (PrescriptiveOptimizationStatusSnapshot)snapshot;

                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }
        /// <summary>
        /// 10.0 Srishti Purohit -- integrate doctor -- For Learn more about
        /// </summary>
        /// <param name="configuration">The configuration object.</param>
        public DependentObjectSnapshot GetTableDependentObjects(int monitoredSqlServerId, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.DatabaseObjectName ObjectName)
        {
            Snapshot snapshot = null;
            using (LOG.InfoCall("GetTableDependentObjects"))
            {
                try
                {
                    ICollectionService collSvc =
                        Management.CollectionServices.GetCollectionServiceForServer(monitoredSqlServerId);

                    if (collSvc == null)
                    {
                        throw new ArgumentException(
                            string.Format("Unable to get interface for monitored server {0}",
                                          monitoredSqlServerId));
                    }
                    using (OnDemandCollectionContext<Snapshot> context = new OnDemandCollectionContext<Snapshot>())
                    {
                        DatabaseProbeConfiguration dbConfig = new DatabaseProbeConfiguration(monitoredSqlServerId);
                        collSvc.GetTableofDependentObject(monitoredSqlServerId, context, null, ObjectName);
                        snapshot = context.Wait();
                    }
                    return (DependentObjectSnapshot)snapshot;

                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// SQLdm 10.0 vineet kumar -- doctor integration-- get prescriptive analysis result
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        public PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result GetPrescriptiveAnalysisResult(int monitoredSqlServerId, AnalysisConfiguration config)
        {
            using (LOG.InfoCall("GetPrescriptiveAnalysisResult"))
            {
                tempAnalysisID = 0;     //Reset temp AnalysisID;
                PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result result;
                try
                {
                    IPrescriptiveAnalysisService svc = Management.GetPrescriptiveAnalysisService();
                    if (svc == null)
                    {
                        throw new ArgumentException("Unable to get interface for PA service");
                    }

                    result = svc.GetPrescriptiveAnalysisResult(monitoredSqlServerId, config);

                    if (result.Error == null)
                    {
                        LOG.Info("Total recommendations generated : " + result.TotalRecommendationCount + " .");
                        foreach (PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.AnalyzerResult analyzer in result.AnalyzerRecommendationList)
                        {
                            LOG.Info("For Analyzer : " + analyzer.AnalyzerID + " recommendations generated : " + analyzer.RecommendationCount + " .");
                        }

                        int analysisID = SaveRecommendations(result, monitoredSqlServerId);
                        tempAnalysisID = analysisID;
                        if (analysisID > 0)
                            return GetRecommendations(monitoredSqlServerId, result.AnalysisCompleteTime);
                        else
                        {
                            LOG.Error("No recommendations generated after analysis.");
                            return null;
                        }
                    }
                    else
                    {
                        throw result.Error;
                    }

                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    tempAnalysisID = 0;     //Reset temp AnalysisID
                    throw;
                }
            }
        }

        /// <summary>
        /// SQLdm 10.0 praveen suhalka -- doctor integration-- get adhoc batch analysis result
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <param name="queryText"></param>
        /// <param name="database"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result GetAdHocBatchAnalysisResult(int monitoredSqlServerId, string queryText, string database, AnalysisConfiguration config)
        {
            using (LOG.InfoCall("GetAdHocBatchAnalysisResult"))
            {
                PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result result;
                try
                {
                    IPrescriptiveAnalysisService svc = Management.GetPrescriptiveAnalysisService();
                    if (svc == null)
                    {
                        throw new ArgumentException("Unable to get interface for PA service");
                    }

                    result = svc.GetAdHocBatchAnalysisResult(monitoredSqlServerId, queryText, database, config);

                    if (result.Error == null)
                    {
                        LOG.Info("Total recommendations generated : " + result.TotalRecommendationCount + " .");
                        foreach (PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.AnalyzerResult analyzer in result.AnalyzerRecommendationList)
                        {
                            LOG.Info("For Analyzer : " + analyzer.AnalyzerID + " recommendations generated : " + analyzer.RecommendationCount + " .");
                        }

                        int analysisID = SaveRecommendations(result, monitoredSqlServerId, true, tempAnalysisID);
                        if (analysisID > 0)
                            return GetRecommendations(monitoredSqlServerId, result.AnalysisCompleteTime);
                        else
                            return null;
                    }
                    else
                    {
                        throw result.Error;
                    }
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    tempAnalysisID = 0;     //Reset temp AnalysisID
                    throw;
                }
            }
        }

        /// <summary>
        /// SQLdm 10.0 praveen suhalka -- doctor integration-- get workload analysis result
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result GetWorkLoadAnalysisResult(int monitoredSqlServerId, AnalysisConfiguration config, ActiveWaitsConfiguration waitConfig, QueryMonitorConfiguration queryConfig)
        {
            using (LOG.InfoCall("GetWorkLoadAnalysisResult"))
            {
                PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result result;
                try
                {
                    IPrescriptiveAnalysisService svc = Management.GetPrescriptiveAnalysisService();
                    if (svc == null)
                    {
                        throw new ArgumentException("Unable to get interface for PA service");
                    }

                    result = svc.GetWorkLoadAnalysisResult(monitoredSqlServerId, config, waitConfig, queryConfig);

                    if (result.Error == null)
                    {
                        LOG.Info("Total recommendations generated : " + result.TotalRecommendationCount + " .");
                        foreach (PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.AnalyzerResult analyzer in result.AnalyzerRecommendationList)
                        {
                            LOG.Info("For Analyzer : " + analyzer.AnalyzerID + " recommendations generated : " + analyzer.RecommendationCount + " .");
                        }

                        int analysisID = SaveRecommendations(result, monitoredSqlServerId, true, tempAnalysisID);
                        if (analysisID > 0)
                            return GetRecommendations(monitoredSqlServerId, result.AnalysisCompleteTime);
                        else
                            return null;
                    }
                    else
                    {
                        throw result.Error;
                    }
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    tempAnalysisID = 0;     //Reset temp AnalysisID
                    throw;
                }
            }
        }
        /// <summary>
        /// SQLdm 10.0 srishti purohit -- doctor integration-- get prescriptive analysis result for notification
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result GetPrescriptiveAnalysisResult(int monitoredSqlServerId, AnalysisConfiguration config, PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.AnalysisType analysisType)
        {
            using (LOG.InfoCall("GetPrescriptiveAnalysisResult"))
            {
                PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result result;
                try
                {
                    IPrescriptiveAnalysisService svc = Management.GetPrescriptiveAnalysisService();
                    if (svc == null)
                    {
                        throw new ArgumentException("Unable to get interface for PA service");
                    }

                    result = svc.GetPrescriptiveAnalysisResult(monitoredSqlServerId, config);
                    result.Type = analysisType;

                    if (result != null && result.Error == null)
                    {
                        LOG.Info("Total recommendations generated : " + result.TotalRecommendationCount + " .");
                        foreach (PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.AnalyzerResult analyzer in result.AnalyzerRecommendationList)
                        {
                            LOG.Info("For Analyzer : " + analyzer.AnalyzerID + " recommendations generated : " + analyzer.RecommendationCount + " .");
                        }

                        int analysisID = SaveRecommendations(result, monitoredSqlServerId);
                        if (analysisID > 0)
                            return GetRecommendations(monitoredSqlServerId, result.AnalysisCompleteTime);
                        else
                        {
                            LOG.Error("No recommendations generated after analysis.");
                            return null;
                        }
                    }
                    else
                    {
                        throw result.Error;
                    }

                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    throw;
                }
            }
        }

        /// <summary>
        /// SQLdm 10.0 (praveen suhalka) -Get the list of schedule for prescriptive analysis
        /// </summary>
        /// <returns></returns>
        public List<PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.PAScheduledTask> GetPrescriptiveAnalysisScheduleList()
        {
            return RepositoryHelper.GetPrescriptiveAnalysisScheduleList(ManagementServiceConfiguration.ConnectionString);
        }

        /// <summary>
        /// SQLdm 10.0 (praveen suhalka) -Get the prescriptive schedule analysis configuration for server
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public ScheduledPrescriptiveAnalysisConfiguration GetAnalysisConfigurationForServer(int serverID)
        {
            ScheduledPrescriptiveAnalysisConfiguration config = new ScheduledPrescriptiveAnalysisConfiguration();
            MonitoredSqlServer instance = RepositoryHelper.GetMonitoredSqlServer(ManagementServiceConfiguration.ConnectionString, serverID);
            if (instance != null)
            {
                config.analysisConfig = instance.AnalysisConfiguration;
                config.maintenanceMode = instance.MaintenanceModeEnabled;
                config.serverVersion = instance.MostRecentSQLVersion;
            }
            else
            {
                config.analysisConfig = new AnalysisConfiguration(serverID);
                config.maintenanceMode = false;
            }
            return config;
        }

        /// <summary>
        /// SQLdm 10.0 (Srishti Purohit) -To support SDR-M16 and get SnapshotValues Object
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        public SnapshotValues GetSnapshotValuesForServerForPrescriptiveAnalysis(int serverID)
        {
            SnapshotValues snapshotValues = RepositoryHelper.GetPreviousSnapshotValuesForPrescriptiveAnalysis(ManagementServiceConfiguration.ConnectionString, serverID);

            return snapshotValues;
        }

        /// <summary>
        /// SQLdm 10.0 (praveen suhalka) -push the latest schedule to PA service
        /// </summary>
        private void SetScheduledTasks()
        {
            using (LOG.InfoCall("SetScheduledTasks"))
            {
                try
                {
                    IPrescriptiveAnalysisService svc = Management.GetPrescriptiveAnalysisService();
                    if (svc == null)
                    {
                        throw new ArgumentException("Unable to get interface for PA service");
                    }

                    svc.SetScheduleTasks(GetPrescriptiveAnalysisScheduleList());

                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                }
            }
        }

        #region Recommendation Doctor's UI

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) -Get the list of Master data for Recommendations
        /// </summary>
        /// <returns></returns>
        public List<MasterRecommendation> GetMasterRecommendations()
        {
			if (MasterRecommendations.MasterRecommendationsInformation.Count == 0)
			{
				MasterRecommendations.MasterRecommendationsInformation 
					= ApplicationHelper.GetMasterRecommondation();
			}
			return MasterRecommendations.MasterRecommendationsInformation;
        }
        /// <summary>
        /// SQLdm 10.0 (srishti purohit) -Get the list of Recommendation
        /// </summary>
        /// <returns></returns>
        public PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result GetRecommendations(int serverID, DateTime filterTime)
        {
            PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result listRecommendation;
            listRecommendation = RepositoryHelper.GetRecommendations(ManagementServiceConfiguration.ConnectionString, serverID, filterTime);
            return listRecommendation;
        }

        /// <summary>
        /// SQLdm 10.5 (NITOR) -Get the Analysis Listing
        /// </summary>
        /// <returns></returns>
        public AnalysisListCollection GetAnalysisListing(int serverID)
        {
            AnalysisListCollection listRecommendation = null;
            listRecommendation = RepositoryHelper.GetAnalysisListing(ManagementServiceConfiguration.ConnectionString, serverID);
            return listRecommendation;
        }

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) -Save the list of Recommendation
        /// </summary>
        /// <returns></returns>
        public Serialized<int> SaveRecommendations(PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result analysisResult, int sqlServerID, bool isRunAnalysis = false, int tempAnalysisID = 0)
        {
            int analysisID;
            analysisID = RepositoryHelper.SaveAnalysisResults(ManagementServiceConfiguration.ConnectionString, analysisResult, sqlServerID, isRunAnalysis, tempAnalysisID);
            //Save PrescriptiveAnalysisSnapshotValuesPrevious
            RepositoryHelper.SavePrescriptiveAnalysisSnapshotValues(ManagementServiceConfiguration.ConnectionString, analysisResult.LatestSnapshot, sqlServerID);
            return analysisID;
        }

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) -Save the list of blocked Recommendation and blocked database
        /// </summary>
        /// <returns></returns>
        public Serialized<bool> BlockRecommendationDatabaseAnalysisConfiguration(int sqlServerID, List<string> blockedRecommendations, List<int> blockedDatabases)
        {
            bool isBlocked;
            isBlocked = RepositoryHelper.BlockRecommendationDatabaseAnalysisConfiguration(ManagementServiceConfiguration.ConnectionString, sqlServerID, blockedRecommendations, blockedDatabases);
            return isBlocked;
        }

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) -Update status and error message
        /// </summary>
        /// <returns></returns>
        public Serialized<bool> UpdateRecommendationOptimizationStatus(List<PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation> updatedRecommendations)
        {
            bool updatedSuccessfully;
            updatedSuccessfully = RepositoryHelper.UpdateRecommendationOptimizationStatus(ManagementServiceConfiguration.ConnectionString, updatedRecommendations);
            return updatedSuccessfully;
        }

        #endregion

        //START: SQLdm 10.0 (Tarun Sapra) - Alert recommendation alert(Operational alert)
        public void LogOperationalAlert(string monitoredObjectName, MonitoredState severity, string heading, string message)
        {
            try
            {
                AlertTableWriter.LogOperationalAlerts(
                    Metric.Operational,
                    new MonitoredObjectName(monitoredObjectName),
                    severity,
                    heading,
                    message);

                LOG.Warn("Operational Alert: ", message.ToString());
            }
            catch (Exception e)
            {
                LOG.Error("Error writing log scan failure operational alert. ", e);
            }
        }
        //END: SQLdm 10.0 (Tarun Sapra) - Alert recommendation alert(Operational alert)

        //SQLdm10.1 (Srishti Purohit)
        //Update SCOMAlertEvent table according to alert rule
        //SCOM Alert Response Action
        public void UpdateSCOMAlertEvent(int metricID, bool isSCOMAlert, Guid ruleID)
        {
            RepositoryHelper.UpdateSCOMAlertEvent(ManagementServiceConfiguration.ConnectionString, metricID, isSCOMAlert, ruleID);
        }
        //START : SQLdm 10.1 (Sristhi Purohit) -get the SWA Web URL for instance views
        public string GetSWAWebURL(string instanceName)
        {
            CommonWebFramework cwfDetails = CommonWebFramework.GetInstance();
            var cwfHelper = new CWFHelper(cwfDetails);
            string SWAurl = null;
            int SWAid = cwfHelper.GetSWAProductId(instanceName);
            if (SWAid > 0)
                SWAurl = CommonWebFramework.GetInstance().WebUIURL + "/render?id=" + SWAid + "#instanceName=" + instanceName;
            else
                LOG.Error("No registered SWA for instance : " + instanceName);
            return SWAurl;
        }
        //END : SQLdm 10.1 (Srishti Purohit) -get the CWF Web URL for instance views
        // SQLDM - 26298 - Code changes to Update the AG Role change alerts
        public void UpdateAGAlertLogRecord(DateTime dateTime, string server, int metricID, int MinutesAgeAlerts)
        {
            try
            {
                AlertTableWriter.UpdateAGAlertLogRecord(dateTime, server, metricID, MinutesAgeAlerts);
                LOG.Warn("Update the AG alert record for the server: ", server.ToString());
            }
            catch (Exception e)
            {
                LOG.Error("Error while updating the AG alert record. ", e);
            }
        }

        ///SQLdm 10.4 (Nikhil Bansal)
        /// Get the On Demand Estimated Query Plan for the given query
        public string GetEstimatedQueryPlan(EstimatedQueryPlanConfiguration configuration)
        {
            using (LOG.InfoCall("OnDemandEstimatedQueryPlan"))
            {
                QueryPlanSnapshot queryPlanSnapshot;

                try
                {
                    ICollectionService collectionService
                     = Management.CollectionServices.GetCollectionServiceForServer(configuration.MonitoredServerId);
                    if (collectionService == null)
                        throw new ArgumentException(string.Format("Unable to get interface for monitored server {0}", configuration.MonitoredServerId));
                    using (OnDemandCollectionContext<QueryPlanSnapshot> context = new OnDemandCollectionContext<QueryPlanSnapshot>())
                    {
                        // Start the estimated Query Plan Collection
                        collectionService.GetEstimatedQueryPlan(configuration, context, null);
                        // Wait for the collection to complete
                        queryPlanSnapshot = (context.Wait()).Deserialize();
                    }

                    //If IsActualPlan is true, then the plan was not fetched...
                    return !queryPlanSnapshot.IsActualPlan ? queryPlanSnapshot.QueryPlan : null;
                }
                catch (Exception e)
                {
                    LOG.Error("Error while getting the estimated query plan. ", e);
                    throw;
                }
            }
        }

        public void CheckLicense(bool enforce)
        {
            LicenseHelper.CheckLicense(enforce);
        }

        //SQLDM-29041 -- Add availability group alert options.
        public List<MonitoredSqlServer> GetMonitoredSqlServers()
        {
            return RepositoryHelper.GetMonitoredSqlServers(ManagementServiceConfiguration.ConnectionString, null, true);
        }

        //SQLDM-29041 -- Add availability group alert options.
        public MetricThresholdEntry GetMetricThresholdEntry(MonitoredSqlServer server, int metricId)
        {
            MonitoredSqlServerState monitoredSqlServerState = new MonitoredSqlServerState(server);
            return monitoredSqlServerState.GetMetricThresholdEntry(metricId);
        }

        public AWSAccountProp GetAWSResourcePrincipleDetails(string instanceName)
        {
            using (LOG.InfoCall("GetDefaultMetricThresholds"))
            {
                return RepositoryHelper.GetAWSResourcePrincipleDetails(ManagementServiceConfiguration.ConnectionString, instanceName);
            }
        }

        public List<string> GetRecommendationListForTargetPlatform(int? sqlServerId = null)
        {
            using (LOG.InfoCall("GetRecommendationListForTargetPlatform"))
            {
                return RepositoryHelper.GetRecommendationListForTargetPlatform(
                    ManagementServiceConfiguration.ConnectionString, sqlServerId);
            }
        }

        public Dictionary<int, string> GetBlockedCategoryListForTargetPlatform(int sqlServerId)
        {
            using (LOG.InfoCall("GetBlockedCategoryListForTargetPlatform"))
            {
                return RepositoryHelper.GetBlockedCategoryListForTargetPlatform(
                    ManagementServiceConfiguration.ConnectionString, sqlServerId);
            }
        }
    }
}
