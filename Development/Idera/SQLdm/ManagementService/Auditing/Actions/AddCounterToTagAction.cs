//------------------------------------------------------------------------------
// <copyright file="CustomAttributes.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Idera.SQLdm.Common.Auditing;

namespace Idera.SQLdm.ManagementService.Auditing.Actions
{
    using System;

    /// <summary>
    /// Prepares the environment to log changes for Link Counters to Tag actions
    /// </summary>
    class AddCounterToTagAction : IAuditAction
    {
        public AuditableActionType Type { get; set; }

        public void FillEntity(ref MAuditableEntity entity)
        {
            Type = Common.Auditing.AuditableActionType.CustomCounterAddedToTag;

            object contextEntity = MAuditingEngine.Instance.PopAuxiliarData("CustomCounterAddedToTag");

            if (contextEntity == null)
            {
                entity = null;
                return;
            }

            var mainEntity = new MAuditableEntity(contextEntity as AuditableEntity);

            string[] tags = mainEntity.GetPropertyValue("LinkedTags").Split(',');
            string[] unlinkedTags = mainEntity.GetPropertyValue("UnLinkedTags").Split(',');
            string[] addedServers = mainEntity.GetPropertyValue("LinkedServers").Split(',');
            string[] removedServers = mainEntity.GetPropertyValue("UnLinkedServers").Split(',');

            bool hasTags = tags[0] != string.Empty;
            bool hasAddedServers = addedServers[0] != string.Empty;
            bool hasRemovedServers = removedServers[0] != string.Empty;
            bool hasUnlinkedTags = unlinkedTags[0] != string.Empty;

            if (hasAddedServers)
            {
                foreach (string server in addedServers)
                {
                    var auditableEntity = new MAuditableEntity();
                    auditableEntity.Name = mainEntity.Name;
                    auditableEntity.AddMetadataProperty("Custom Counter linked to Server", server);
                    auditableEntity.AddMetadataProperty("Custom Counter Name", mainEntity.Name);

                    MAuditingEngine.Instance.LogAction(auditableEntity, AuditableActionType.ServerPropertiesChanged);
                }
            }

            if (hasRemovedServers)
            {
                foreach (string server in removedServers)
                {
                    var auditableEntity = new MAuditableEntity();
                    auditableEntity.Name = mainEntity.Name;
                    auditableEntity.AddMetadataProperty("Custom Counter unlinked from Server", server);
                    auditableEntity.AddMetadataProperty("Custom Counter Name", mainEntity.Name);

                    MAuditingEngine.Instance.LogAction(auditableEntity, AuditableActionType.ServerPropertiesChanged);
                }
            }

            if (hasTags)
            {
                foreach (string tag in tags)
                {
                    var auditableEntity = new MAuditableEntity();
                    auditableEntity.Name = mainEntity.Name;
                    auditableEntity.AddMetadataProperty("Linked Tag", tag);
                    auditableEntity.SetHeaderParam(mainEntity.Name, tag);

                    MAuditingEngine.Instance.LogAction(auditableEntity, Type);
                }
            }
            
            if(hasUnlinkedTags)
            {
                foreach (string tag in unlinkedTags)
                {
                    var auditableEntity = new MAuditableEntity();
                    auditableEntity.Name = mainEntity.Name;
                    auditableEntity.AddMetadataProperty("Unlinked Tag", tag);
                    auditableEntity.SetHeaderParam(mainEntity.Name, tag);

                    MAuditingEngine.Instance.LogAction(auditableEntity, AuditableActionType.CustomCounterRemovedToTag);
                }
            }
            
            entity = null;
        }
    }
}
