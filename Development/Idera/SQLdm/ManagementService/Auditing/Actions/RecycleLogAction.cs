using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.ManagementService.Configuration;
using Idera.SQLdm.ManagementService.Helpers;

namespace Idera.SQLdm.ManagementService.Auditing.Actions
{
    class RecycleLogAction : IAuditAction
    {
        private MonitoredSqlServer _sqlServer;
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("RecycleLogAction");

        public RecycleLogAction(RecycleLogConfiguration recycleLogConfiguration)
        {
            if (recycleLogConfiguration == null)
            {
                _sqlServer = null;
                return;
            }

            _sqlServer = RepositoryHelper.GetMonitoredSqlServer(ManagementServiceConfiguration.ConnectionString,
                                                                   recycleLogConfiguration.MonitoredServerId);
            Type = AuditableActionType.RecycleLogSQLServer;
        }

        public RecycleLogAction(RecycleAgentLogConfiguration agentLogConfiguration)
        {
            if (agentLogConfiguration == null)
            {
                _sqlServer = null;
                return;
            }

            _sqlServer = RepositoryHelper.GetMonitoredSqlServer(ManagementServiceConfiguration.ConnectionString,
                                                                 agentLogConfiguration.MonitoredServerId);
            Type = AuditableActionType.RecycleLogSQLServerAgent;
        }


        public void FillEntity(ref MAuditableEntity entity)
        {
            var auditable = CallContext.GetData(AuditContextData.ContextName) as AuditContextData;

            if (_sqlServer == null)
            {
                entity = null;
                return;
            }

            string sqlUser = (auditable == null) ? string.Empty : auditable.SqlUser;
            if(string.IsNullOrEmpty(sqlUser))
            {
                LOG.Warn("SQL user not found for logging Recycle Log Action");
            }

            entity.Name = _sqlServer.InstanceName;

            entity.SetHeaderParam(sqlUser);
            entity.MetadataProperties.Clear();
            entity.AddMetadataProperty("Server Name",_sqlServer.InstanceName);
        }

        public AuditableActionType Type { get; set; }
    }
}
