//------------------------------------------------------------------------------
// <copyright file="CustomAttributes.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.ManagementService.Auditing.Actions
{
    class AlertTemplateAction : IAuditAction
    {
        private IEnumerable<int> serverNamesList = null;
        private List<MonitoredSqlServer> monitoredSqlServerList = null;
        private string tagName = String.Empty;
        private const String MasterTemplateName = "Master Template";
        private const String AffectedServerTemplate = "Affected Server";

        public AlertTemplateAction(IEnumerable<int> serverNames, List<MonitoredSqlServer> monitoredSqlServer)
        {
            this.serverNamesList = serverNames;
            this.Type = AuditableActionType.ApplyAlertTemplateToServer;
            this.monitoredSqlServerList = monitoredSqlServer;
        }

        public void FillEntity(ref MAuditableEntity maAuditableEntity)
        {
            var contextTagName = MAuditingEngine.Instance.PopAuxiliarData("ApplyAlertTemplateToServer") as AuditAuxiliar<string>;
            this.tagName = contextTagName != null ? contextTagName.Data : string.Empty;

            if (monitoredSqlServerList != null)
            {
                maAuditableEntity.Name = tagName;
                maAuditableEntity.MetadataProperties.Clear();
                maAuditableEntity.SetHeaderParam(tagName);
                maAuditableEntity.AddMetadataProperty(MasterTemplateName, tagName);
                //to fill out autitable maAuditableEntity
                foreach (int instanceId in serverNamesList)
                {
                    if (monitoredSqlServerList != null)
                    {
                        var sqlServer = monitoredSqlServerList.Find(item => item.Id == instanceId);

                        if (sqlServer != null)
                        {
                            maAuditableEntity.AddMetadataProperty(AffectedServerTemplate, sqlServer.InstanceName);
                        }
                    }
                }
            }
        }

        public AuditableActionType Type { get; set; }
    }
}
