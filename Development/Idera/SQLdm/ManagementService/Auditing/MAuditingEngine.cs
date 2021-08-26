// -----------------------------------------------------------------------
// <copyright file="MAuditingEngine.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Idera.Newsfeed.Shared;
using Idera.Newsfeed.Shared.Security;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.ManagementService.Auditing.Actions;
using Idera.SQLdm.ManagementService.Configuration;
using Idera.SQLdm.ManagementService.Helpers;

namespace Idera.SQLdm.ManagementService.Auditing
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    class MAuditingEngine : AuditingEngine
    {
        #region Fields
        private static MAuditingEngine instance = null;
        private static readonly object padlock = new object();

        /// TODO Need to check back this commented code
        private MAuditableEntity _InitializationEntity = null;

        #endregion Fields


        #region Properties

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static MAuditingEngine Instance
        {
            get
            {
                lock (padlock)
                {
                    return instance ?? (instance = new MAuditingEngine());
                }
            }
        }

        public MAuditableEntity DefaultAuditableEntity
        {
            get
            {
                string completeWorkStationUserName = String.Format("{0}\\{1}", Environment.UserDomainName,
                                                                    Environment.UserName);

                string repositoryUser = ManagementServiceConfiguration.RepositoryUser;

                return new MAuditableEntity
                {
                    SqlUser = string.IsNullOrEmpty(repositoryUser) ? completeWorkStationUserName : repositoryUser,
                    Workstation = Environment.MachineName,
                    WorkstationUser =
                        completeWorkStationUserName
                };
            }
        }

        #endregion Properties


        #region Constructors

        private MAuditingEngine()
        {
            base.HeaderTemplateList = SQLdm.ManagementService.ManagementService.GetAuditingHeaderTemplates();
        }

        #endregion Constructors

        /// <summary>
        /// Particular LogAction given an specific IAuditableAction
        /// </summary>
        /// <param name="action"></param>
        internal void LogAction(IAuditAction action)
        {
            var entity = new MAuditableEntity();
            action.FillEntity(ref entity);

            if (entity != null)
            {
                LogAction(entity, action.Type);
            }
        }

        /// <summary>
        /// Given a list of Audit Actions, it iterates over it and commits the actions
        /// </summary>
        /// <param name="action"></param>
        internal void LogAction(params IAuditAction[] action)
        {
            foreach (IAuditAction auditAction in action)
            {
                LogAction(auditAction);
            }
        }

        /// <summary>
        /// Particular LogAction for MAuditableEngine which does not depend on a ManagementServer Instance as AuditableEngine does
        /// </summary>
        /// <param name="auditEntity"></param>
        /// <param name="actionType"></param>
        public void LogAction(MAuditableEntity auditEntity, AuditableActionType actionType)
        {
            if (auditEntity == null)
            {
                return;
            }

            if (!auditEntity.Workstation.Equals("SQLdmMobile"))
            {
                var auditable = CallContext.GetData(AuditContextData.ContextName) as AuditContextData;

                if (auditable == null)
                {
                    return;
                }

                auditEntity.SqlUser = auditable.SqlUser;
                auditEntity.Workstation = auditable.Workstation;
                auditEntity.WorkstationUser = auditable.WorkstationUser;
            }
            
            auditEntity.TimeStamp =         DateTime.Now.ToUniversalTime();
            auditEntity.MetaData =          ProcessMetaDataProperties(auditEntity);
            auditEntity.Header =            ProcessHeaderTemplate(actionType, auditEntity.HeaderParamList);
            GetBodyFormat(auditEntity);

            try
            {
                SQLdm.ManagementService.ManagementService.LogChangeEvent(auditEntity, (short)actionType);
            }
            catch (Exception e)
            {
                //LOG.ErrorFormat("Management Service not found for AuditingEngine.LogAction. {0}", e);
            }
        }

        /// <summary>
        /// Method that helps logging any ServerAction of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configuration"></param>
        public void LogServerAction<T>(T configuration)
        {
            var auditableEntity = GetAuditableEntityFromCorrectContext();

            MAuditableEntity mAuditableEntity = auditableEntity == null ? DefaultAuditableEntity : new MAuditableEntity(auditableEntity);

            AuditableActionType actionType = AuditableActionType.None;

            if (typeof(T) == typeof(KillSessionConfiguration))
            {
                LogAction(new SessionAction(AuditableActionType.SessionKilled));
            }

            if (typeof(T) == typeof(FullTextActionConfiguration))
            {
                PrepareFullTextAction(configuration as FullTextActionConfiguration, mAuditableEntity, ref actionType);
            }

            if(typeof(T) == typeof(FreeProcedureCacheConfiguration))
            {
                PrepareFreeProcedureCacheAction(configuration as FreeProcedureCacheConfiguration, mAuditableEntity, ref actionType);
            }

            if (typeof(T) == typeof(BlockedProcessThresholdConfiguration))
            {
                PrepareBlockedProcessThresholdConfigurationAction(configuration as BlockedProcessThresholdConfiguration, mAuditableEntity, ref actionType);
            }

            if (typeof(T) == typeof(ServiceControlConfiguration))
            {
                PrepareServiceControlConfigurationAction(configuration as ServiceControlConfiguration, mAuditableEntity, ref actionType);
            }

            if (typeof(T) == typeof(JobControlConfiguration))
            {
                PrepareJobControlConfigurationAction(configuration as JobControlConfiguration, mAuditableEntity, ref actionType);
            }

            if (typeof (T) == typeof (RecycleLogConfiguration))
            {
                LogAction(new RecycleLogAction(configuration as RecycleLogConfiguration));
            }

            if (typeof(T) == typeof(RecycleAgentLogConfiguration))
            {
                LogAction(new RecycleLogAction(configuration as RecycleAgentLogConfiguration));
            }
            if (typeof(T) == typeof(AdhocQueryConfiguration))
            {
                PrepareQueryControlConfigurationAction(configuration as AdhocQueryConfiguration, mAuditableEntity, ref actionType);
            }

            if (typeof(T) == typeof(UpdateStatisticsConfiguration))
            {
                //Remove the audit event on management service
                //LogAction(new UpdateStatisticsAction(configuration as UpdateStatisticsConfiguration));
            }

            if (typeof(T) == typeof(ReconfigurationConfiguration))
            {
                LogAction(new EditConfigurationValue(configuration as ReconfigurationConfiguration));
            }

            if (typeof(T) == typeof(MirroringPartnerActionConfiguration))
            {
                LogAction(new MirrorAction(configuration as MirroringPartnerActionConfiguration));
            }

            if (actionType != AuditableActionType.None)
            {
                LogAction(mAuditableEntity, actionType);    
            }
        }

        private static AuditableEntity GetAuditableEntityFromCorrectContext()
        {
            AuditableEntity auditableEntity = null;
            LogicalCallContextData principalContext = CallContext.GetData("ThreadPrincipal") as LogicalCallContextData;

            string user = string.Empty;
            SQLdmIdentity sqldmCreds;
            if (principalContext != null)
            {
                //if this is not null the source is mobile
                sqldmCreds = principalContext.Principal.Identity as SQLdmIdentity;
                if (sqldmCreds != null)
                {
                    auditableEntity = new AuditableEntity()
                                          {
                                              WorkstationUser =
                                                  sqldmCreds.PrincipalNames.Length > 0
                                                      ? sqldmCreds.PrincipalNames[0]
                                                      : ManagementServiceConfiguration.RepositoryUser,
                                              SqlUser = sqldmCreds.Name,
                                              Workstation = "SQLdmMobile",
                                          };
                }
            }
            else
            {
                var contextData = CallContext.GetData(AuditContextData.ContextName) as AuditContextData;
                if(contextData != null)
                {
                    auditableEntity = new MAuditableEntity(contextData);
                }
               
            }
            return auditableEntity;
        }

        /// <summary>
        /// Creates an Auditable Entity ready for SQL new query ran
        /// </summary>
        /// <param name="adhocQueryConfiguration"></param>
        /// <param name="mAuditableEntity"></param>
        /// <param name="actionType"></param>
        private void PrepareQueryControlConfigurationAction(AdhocQueryConfiguration configuration, MAuditableEntity mAuditableEntity, ref AuditableActionType actionType)
        {
            string connectString = ManagementServiceConfiguration.ConnectionString;
            string instanceName = RepositoryHelper.GetMonitoredSqlServer(connectString, configuration.MonitoredServerId).InstanceName;

            mAuditableEntity.Name = instanceName;
            mAuditableEntity.SetHeaderParam(instanceName);

            string databaseName = String.Empty;
            string row = "10";

            string[] query = configuration.Sql.Split('\n');
            int breakcount = 0;
            
            for (int i = 0; i < query.Length; i++)
            {
                if (query[i].ToLower().Contains("use "))
                {
                    string[] dbuse = query[i].Split(' ');
                    databaseName = dbuse[dbuse.Length - 1];
                    breakcount++;
                }
                if (query[i].ToLower().Contains("rowcount"))
                {
                    string[] rownum = query[i].Split(' ');
                    row = rownum[rownum.Length - 1];
                    breakcount++;
                }
                if (breakcount==2)
                {
                    break;
                }
            }

            mAuditableEntity.AddMetadataProperty("Selected Server", instanceName);
            mAuditableEntity.AddMetadataProperty("Selected Database",
                                                 String.IsNullOrEmpty(databaseName) ? "None" : databaseName);
            mAuditableEntity.AddMetadataProperty("Row limit", row);
            mAuditableEntity.AddMetadataProperty("Query:", string.Empty);
            mAuditableEntity.AddMetadataProperty("    "+configuration.Sql, string.Empty);

            actionType = AuditableActionType.RunQuery;
        }

        #region ServerActions

        /// <summary>
        /// Creates an Auditable Entity ready for SQL Job started and stopped
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="mAuditableEntity"></param>
        /// <param name="actionType"></param>
        private void PrepareJobControlConfigurationAction(JobControlConfiguration configuration, MAuditableEntity mAuditableEntity, ref AuditableActionType actionType)
        {
            /*
                Object Name:Job Name
                Action Name:SQL Job Started
                Change Summary:Repository user "{0}" stopped SQL Job "{1}"
                Change Details: Affected Server
             */
            string connectString = ManagementServiceConfiguration.ConnectionString;
            string instanceName = RepositoryHelper.GetMonitoredSqlServer(connectString, configuration.MonitoredServerId).InstanceName;

            mAuditableEntity.Name = configuration.JobName;
            mAuditableEntity.AddMetadataProperty("Affected Server", instanceName);
            mAuditableEntity.SetHeaderParam(mAuditableEntity.SqlUser, configuration.JobName);

            if (configuration.Action == JobControlAction.Start)
            {
                actionType = AuditableActionType.SQLJobStarted;
            }

            if (configuration.Action == JobControlAction.Stop)
            {
                actionType = AuditableActionType.SQLJobStopped;
            }
        }

        /// <summary>
        /// Creates an Auditable Entity ready for SQL Server Agent started and stopped
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="mAuditableEntity"></param>
        /// <param name="actionType"></param>
        private void PrepareServiceControlConfigurationAction(ServiceControlConfiguration configuration, MAuditableEntity mAuditableEntity, ref AuditableActionType actionType)
        {
            /*
                Object Name:Service / Job name
                Action Name:SQL Server Agent Service Started
                Change Summary:Repository user "{0}" started SQL Server Agent Service
                Change Details:Server Affected
             */

            var type = typeof(ServiceName);
            var memInfo = type.GetMember(configuration.ServiceToAffect.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute),
                false);
            var serviceName = ((DescriptionAttribute)attributes[0]).Description;


            string connectString = ManagementServiceConfiguration.ConnectionString;
            string instanceName = RepositoryHelper.GetMonitoredSqlServer(connectString, configuration.MonitoredServerId).InstanceName;

            mAuditableEntity.Name = serviceName;
            mAuditableEntity.AddMetadataProperty("Affected Server", instanceName);
            mAuditableEntity.SetHeaderParam(mAuditableEntity.SqlUser);

            switch (configuration.ServiceToAffect)
            {
                case ServiceName.Agent:
                    if (configuration.Action == ServiceControlConfiguration.ServiceControlAction.Start)
                        actionType = AuditableActionType.SQLServerAgentServiceStarted;
                    else if (configuration.Action == ServiceControlConfiguration.ServiceControlAction.Stop)
                        actionType = AuditableActionType.SQLServerAgentServiceStopped;
                    break;
                case ServiceName.DTC:
                    if (configuration.Action == ServiceControlConfiguration.ServiceControlAction.Start)
                        actionType = AuditableActionType.DTCServiceStarted;
                    else if (configuration.Action == ServiceControlConfiguration.ServiceControlAction.Stop)
                        actionType = AuditableActionType.DTCServiceStopped;
                    break;
                case ServiceName.FullTextSearch:
                    if (configuration.Action == ServiceControlConfiguration.ServiceControlAction.Start)
                        actionType = AuditableActionType.FTSServiceStarted;
                    else if (configuration.Action == ServiceControlConfiguration.ServiceControlAction.Stop)
                        actionType = AuditableActionType.FTSServiceStopped;
                    break;
                case ServiceName.SqlServer:
                    if (configuration.Action == ServiceControlConfiguration.ServiceControlAction.Start)
                        actionType = AuditableActionType.SQLServerServiceStarted;
                    else if (configuration.Action == ServiceControlConfiguration.ServiceControlAction.Stop)
                        actionType = AuditableActionType.SQLServerServiceStopped;
                    break;
                //START : SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --process start/stop of the new sql services
                case ServiceName.Browser:
                    if (configuration.Action == ServiceControlConfiguration.ServiceControlAction.Start)
                        actionType = AuditableActionType.BrowserServiceStarted;
                    else if (configuration.Action == ServiceControlConfiguration.ServiceControlAction.Stop)
                        actionType = AuditableActionType.BrowserServiceStopped;
                    break;
                case ServiceName.ActiveDirectoryHelper:
                    if (configuration.Action == ServiceControlConfiguration.ServiceControlAction.Start)
                        actionType = AuditableActionType.ActiveDirectoryHelperServiceStarted;
                    else if (configuration.Action == ServiceControlConfiguration.ServiceControlAction.Stop)
                        actionType = AuditableActionType.ActiveDirectoryHelperServiceStopped;
                    break;
                //END : SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --process start/stop of the new sql services
            }

        }

        /// <summary>
        /// Creates an Auditable Entity ready for Blocked Process Threshold Configuration
        /// </summary>
        /// <param name="blockedProcessThresholdConfiguration"></param>
        /// <param name="mAuditableEntity"></param>
        /// <param name="actionType"></param>
        private void PrepareBlockedProcessThresholdConfigurationAction(BlockedProcessThresholdConfiguration configuration, MAuditableEntity mAuditableEntity, ref AuditableActionType actionType)
        {
            string connectString = ManagementServiceConfiguration.ConnectionString;
            string instanceName = RepositoryHelper.GetMonitoredSqlServer(connectString, configuration.MonitoredServerId).InstanceName;

            mAuditableEntity.Name = instanceName;

            if (configuration.RunValue.HasValue)
            {
                mAuditableEntity.AddMetadataProperty("Enabled capturing of Blocking events",
                                                     configuration.RunValue > 0 ? "True" : "False");

                mAuditableEntity.AddMetadataProperty("Changed Blocked Process Threshold value to",
                                                     configuration.RunValue.Value.ToString());
            }

            mAuditableEntity.SetHeaderParam(mAuditableEntity.SqlUser);

            actionType = AuditableActionType.BlockedProcessThresholdConfigurationEdited;
        }

        /// <summary>
        /// Creates an Auditable Entity ready for Log a Free Procedure Cache Action
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="mAuditableEntity"></param>
        /// <param name="actionType"></param>
        private void PrepareFreeProcedureCacheAction(FreeProcedureCacheConfiguration configuration, MAuditableEntity mAuditableEntity, ref AuditableActionType actionType)
        {
            string connectString = ManagementServiceConfiguration.ConnectionString;
            string instanceName = RepositoryHelper.GetMonitoredSqlServer(connectString, configuration.MonitoredServerId).InstanceName;

            mAuditableEntity.Name = instanceName;
            mAuditableEntity.SetHeaderParam(mAuditableEntity.SqlUser);

            actionType = AuditableActionType.ClearProcedureCacheTriggered;
        }

        /// <summary>
        /// Prepares Auditable Entity and AuditableAction Type for being Logged
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="auditableEntity"></param>
        /// <param name="auditableActionType"></param>
        public void PrepareFullTextAction(FullTextActionConfiguration configuration, MAuditableEntity auditableEntity, ref AuditableActionType auditableActionType)
        {
            string connectString = ManagementServiceConfiguration.ConnectionString;
            string instanceName = RepositoryHelper.GetMonitoredSqlServer(connectString, configuration.MonitoredServerId).InstanceName;

            auditableEntity.Name = configuration.Catalogname;
            auditableEntity.AddMetadataProperty("Affected Server", instanceName);
            auditableEntity.SetHeaderParam(auditableEntity.SqlUser);

            switch (configuration.Action)
            {
                case FullTextActionConfiguration.FullTextAction.Optimize:
                    auditableActionType = AuditableActionType.FullTextSearchOptimized;
                    break;
                case FullTextActionConfiguration.FullTextAction.Rebuild:
                    auditableActionType = AuditableActionType.FullTextSearchRebuilt;
                    break;
            }
        }

        /// <summary>
        /// Creates an AuditableEntity and log it to the AuditingEngine about Predictive Analytics
        /// </summary>
        /// <param name="state">True if the Predictive Analytics was enabled</param>
        public void LogPredictiveAnalyticsAction(bool predictiveAnalyticsEnabled)
        {

            var auditContextData = CallContext.GetData(AuditContextData.ContextName) as AuditContextData;

            if (auditContextData != null)
            {
                var entity = new MAuditableEntity(auditContextData);
                entity.Name = "Predictive Analytics";
                LogAction(entity,
                          predictiveAnalyticsEnabled
                              ? AuditableActionType.PredictiveAnalyticsEnabled
                              : AuditableActionType.PredictiveAnalyticsDisabled);
            }
        }

        #endregion Server Actions

        /// <summary>
        /// Add a log entry when a user is created from Application security.
        /// </summary>
        public static void LogAddPermissionAuditAction()
        {
            var permissions =
                RepositoryHelper.GetAvailablePermissionsIds(ManagementServiceConfiguration.ConnectionString);

            // Get the last inserted ID.
            int lastId = -1;

            foreach (int id in permissions)
            {
                if (id > lastId)
                {
                    lastId = id;
                }
            }

            if (lastId > -1)
            {
                var contextEntity = CallContext.GetData(AuditContextData.ContextName) as AuditContextData;

                var permissionDef = RepositoryHelper.GetPermissionDefinitionById(lastId,
                                                                                 ManagementServiceConfiguration.ConnectionString);

                if (permissionDef != null && contextEntity != null)
                {
                    var entity = new MAuditableEntity(permissionDef.GetAuditableEntity());

                    var tags = GetConcatTagsByPermissionId(lastId);
                    entity.AddMetadataProperty("Associated Tags", tags);

                    entity.SqlUser = contextEntity.SqlUser;
                    entity.Workstation = contextEntity.Workstation;
                    entity.WorkstationUser = contextEntity.WorkstationUser;
                    entity.Name = permissionDef.Login;
                    entity.SetHeaderParam(permissionDef.Login);

                    MAuditingEngine.Instance.LogAction(entity, AuditableActionType.ApplicationSecurityAddUserAccount);
                }
            }
        }

        /// <summary>
        /// Returns a concatenated string containing all Tags associated to the given Permission Id
        /// </summary>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        private static string GetConcatTagsByPermissionId(int permissionId)
        {
            var tagIds =
                    RepositoryHelper.GetTagIdsByPermissionId(
                        ManagementServiceConfiguration.ConnectionString, permissionId);

            var tags = RepositoryHelper.GetTags(ManagementServiceConfiguration.ConnectionString);

            var tagsList = new List<string>();
            bool found = false;
            
            foreach (Tag tag in tags)
            {
                foreach (int tagId in tagIds)
                {
                    if(tag.Id == tagId)
                    {
                        tagsList.Add(tag.Name);
                        found = true;
                        break;
                    }
                }
            }

            var concatenatedTags = new StringBuilder();

            // We have no tags associated to that permission
            if(tagsList.Count == 0)
            {
                return "None";
            }

            foreach (string tagName in tagsList)
            {
                concatenatedTags.Append(tagName + ", ");
            }

            concatenatedTags.Remove(concatenatedTags.Length - 2, 2);

            return concatenatedTags.ToString();
        }
    }
}

