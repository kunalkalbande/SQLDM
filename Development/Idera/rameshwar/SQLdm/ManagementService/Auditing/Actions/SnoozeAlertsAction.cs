//------------------------------------------------------------------------------
// <copyright file="CustomAttributes.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.ManagementService.Configuration;

namespace Idera.SQLdm.ManagementService.Auditing.Actions
{
    class SnoozeAlertsAction : IAuditAction
    {
        private List<MonitoredSqlServer> monitoredSqlServerList = null;
        private string tagName = String.Empty;
        private string userName = String.Empty;
        private string serverName = String.Empty;
        private IList<int> serverListInstaces = null;
        private DataTable dataAlertsTable = null;
        private const String snoozeServer = "Affected Server";

        public SnoozeAlertsAction(AuditableActionType auditableAction)
        {
            Type = auditableAction;
        }

        public SnoozeAlertsAction(IList<int> serverListId, List<MonitoredSqlServer> monitoredSqlServer, AuditableActionType auditableAction)
        {
            serverListInstaces = serverListId;
            monitoredSqlServerList = monitoredSqlServer;
            Type = auditableAction;
        }

        public void FillEntity(ref MAuditableEntity maAuditableEntity)
        {
            switch (Type)
            {
                //in case Snooze or resume all servers
                case AuditableActionType.SnoozeAllServerAlerts:
                    var contextSnoozeAllServer = MAuditingEngine.Instance.PopAuxiliarData("AuditSnnozeAllSeversAlerts") as AuditAuxiliar<AuditableEntity>;
                    tagName = contextSnoozeAllServer != null ? contextSnoozeAllServer.Data.Name : String.Empty;
                    userName = contextSnoozeAllServer != null ? contextSnoozeAllServer.Data.SqlUser : String.Empty;
                    AuditAllServerSnoozeOrResumeAction(ref maAuditableEntity);
                    break;

                case AuditableActionType.ResumeAllServerAlerts:
                    var contextResumeAllServers = MAuditingEngine.Instance.PopAuxiliarData("AuditResumeAllSeversAlerts") as AuditAuxiliar<AuditableEntity>;
                    tagName = contextResumeAllServers != null ? contextResumeAllServers.Data.Name : String.Empty;
                    userName = contextResumeAllServers != null ? contextResumeAllServers.Data.SqlUser : String.Empty;
                    AuditAllServerSnoozeOrResumeAction(ref maAuditableEntity);
                    break;

                case AuditableActionType.SingeAlertSnooze:
                    var singleAlertAuditable = MAuditingEngine.Instance.PopAuxiliarData("AuditSingleSnoozeAlert") as AuditAuxiliar<AuditableEntity>;
                    if (singleAlertAuditable != null)
                    {
                        maAuditableEntity.Name = singleAlertAuditable.Data.Name;
                        maAuditableEntity.MetadataProperties.AddRange(singleAlertAuditable.Data.MetadataProperties);
                    }
                    maAuditableEntity.SetHeaderParam(singleAlertAuditable.Data.SqlUser);
                    break;

                case AuditableActionType.MultipleAlertsSnoozed:
                    var snoozeMultipleAlert = MAuditingEngine.Instance.PopAuxiliarData("AuditSingleSnoozeAlert") as AuditAuxiliar<AuditableEntity>;
                    
                    if (snoozeMultipleAlert != null)
                    {
                        maAuditableEntity.Name = snoozeMultipleAlert.Data.Name;
                        maAuditableEntity.MetadataProperties.AddRange(snoozeMultipleAlert.Data.MetadataProperties);
                    }

                    maAuditableEntity.SetHeaderParam(snoozeMultipleAlert.Data.SqlUser);
                    break;

                case AuditableActionType.AlertsUnSnoozed:
                    var contextObject = MAuditingEngine.Instance.PopAuxiliarData("SnoozeEntity");
                    var contextEntity = CallContext.GetData(AuditContextData.ContextName) as AuditContextData;

                    if (contextObject == null)
                    {
                        maAuditableEntity = null;
                        return;
                    }

                    maAuditableEntity = new MAuditableEntity(contextObject as AuditableEntity);
                    maAuditableEntity.SetHeaderParam(contextEntity == null ? ManagementServiceConfiguration.RepositoryUser : contextEntity.SqlUser);

                    break;
            }
        }

        public AuditableActionType Type { get; set; }

        /// <summary>
        /// Audit Mass Snooze or resume all server alerts action
        /// </summary>
        /// <param name="AuditableActionType"></param>
        ///  /// <param name="MAuditableEntity"></param>
        private void AuditAllServerSnoozeOrResumeAction(ref MAuditableEntity maAuditableEntity)
        {
            maAuditableEntity.Name = tagName;
            maAuditableEntity.MetadataProperties.Clear();
            maAuditableEntity.SetHeaderParam(userName);

            //to fill out autitable maAuditableEntity
            maAuditableEntity.AddMetadataProperty("Total severs", serverListInstaces.Count.ToString());
            foreach (int instanceId in serverListInstaces)
            {
                if (monitoredSqlServerList != null)
                {
                    var sqlServer = monitoredSqlServerList.Find(item => item.Id == instanceId);

                    if (sqlServer != null)
                    {
                        maAuditableEntity.AddMetadataProperty(snoozeServer, sqlServer.InstanceName);
                    }
                }
            }
        }
    }
}
