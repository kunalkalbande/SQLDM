// -----------------------------------------------------------------------
// <copyright file="VirtualizationAction.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.ManagementService.Auditing.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    class VirtualizationAction : IAuditAction
    {
        private VirtualizationConfiguration newConfig = null;
        private VirtualizationConfiguration oldConfig = null;
        private string instanceName = string.Empty;

        public AuditableActionType Type { get; set; }

        public VirtualizationAction(MonitoredSqlServerConfiguration newConfig, MonitoredSqlServerConfiguration oldConfig)
        {
            this.newConfig = newConfig == null ? null : newConfig.VirtualizationConfiguration;
            this.oldConfig = oldConfig == null ? null : oldConfig.VirtualizationConfiguration;
            this.instanceName = newConfig == null ? oldConfig.InstanceName : newConfig.InstanceName;
        }

        public void FillEntity(ref MAuditableEntity entity)
        {
            if (oldConfig == null && newConfig != null)
            {
                if (newConfig.VCServerType == "HyperV")
                {
                    Type = AuditableActionType.ServerLinkedToHyperV;
                }
                else if (newConfig.VCServerType == "vCenter")
                {
                    Type = AuditableActionType.ServerLinkedTovCenter;
                }
                PrepareCommonAction(ref entity, newConfig);
            }

            if (newConfig == null && oldConfig != null)
            {
                if (oldConfig.VCServerType == "HyperV")
                {
                    Type = AuditableActionType.ServerUnlinkedFromHyperV;
                }
                else if (oldConfig.VCServerType == "vCenter")
                {
                    Type = AuditableActionType.ServerUnlinkedFromvCenter;
                }
                PrepareCommonAction(ref entity, oldConfig);
            }

            if (newConfig != null && oldConfig != null)
            {
                if (newConfig.VCServerType == "HyperV")
                {
                    Type = AuditableActionType.ServerLinkConfigurationToHyperVChanged;
                }
                else if (newConfig.VCServerType == "vCenter")
                {
                    Type = AuditableActionType.ServerLinkConfigurationTovCenterChanged;
                }
                ServerLinkConfigChangedvCenterAction(ref entity);
            }
        }

        private void ServerLinkConfigChangedvCenterAction(ref MAuditableEntity entity)
        {
            entity =
                new MAuditableEntity(
                    newConfig.GetAuditableEntity(oldConfig));
            entity.Name = this.instanceName;

            if (!entity.HasMetadataProperties())
            {
                entity = null;
            }
        }

        /// <summary>
        /// Prepares an entity to log Server Linked or Unlinked to vCenter Action
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="virtualizationConfiguration"></param>
        private void PrepareCommonAction(ref MAuditableEntity entity, VirtualizationConfiguration virtualizationConfiguration)
        {
            entity = new MAuditableEntity(virtualizationConfiguration.GetAuditableEntity());
            entity.Name = this.instanceName;
        }
    }
}
