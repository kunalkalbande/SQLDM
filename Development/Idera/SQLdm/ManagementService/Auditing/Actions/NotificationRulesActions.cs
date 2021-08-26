//------------------------------------------------------------------------------
// <copyright file="CustomAttributes.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Notification;

namespace Idera.SQLdm.ManagementService.Auditing.Actions
{
    /// <summary>
    /// Strategy class to Log Actions
    /// </summary>
    class NotificationRulesActions : IAuditAction
    {
        private const string AddKey = "AddActionResponse";
        private const string DeleteKey = "DeletedActionResponse";
        private const string EditKey = "EditActionResponse";
        private const string CopyKey = "CopyActionResponse";
        private const string CopiedFromKey = "EditActionCopiedFrom";
        private const string EditAlertResponseKey = "EditAlertResponse";
        private const string EditType = "EditType";
        
        private const string NotificationProviderType = "Notification provider type";

        private NotificationProviderInfo notificationProviderInfo = null;


        public NotificationRulesActions(AuditableActionType type)
        {
            Type = type;
        }

        public NotificationRulesActions(NotificationProviderInfo notificationProviderInfo, AuditableActionType type)
        {
            Type = type;
            this.notificationProviderInfo = notificationProviderInfo;
        }

        /// <summary>
        /// Fills the given entity with more information related to the action
        /// </summary>
        /// <param name="persistingEntity"></param>
        public void FillEntity(ref MAuditableEntity persistingEntity)
        {
            var auxiliarAuditableEntity = new MAuditableEntity();
            object contextEntity;

            switch (Type)
            {
                case AuditableActionType.AddAlertResponse:
                    contextEntity = MAuditingEngine.Instance.PopAuxiliarData(AddKey);
                    persistingEntity = new MAuditableEntity(contextEntity as AuditableEntity);
                    persistingEntity.SetHeaderParam(persistingEntity.Name);
                    break;

                case AuditableActionType.RemoveAlertResponse:
                    contextEntity = MAuditingEngine.Instance.PopAuxiliarData(DeleteKey);

                    if (contextEntity == null) return;

                    auxiliarAuditableEntity =
                        new MAuditableEntity(contextEntity as AuditableEntity);

                    persistingEntity.Name = auxiliarAuditableEntity.Name;
                    persistingEntity.SetHeaderParam(persistingEntity.Name);
                    break;

                case AuditableActionType.EditAlertResponse:
                    contextEntity = MAuditingEngine.Instance.PopAuxiliarData(EditKey);
                    
                    if (contextEntity == null) return;

                    auxiliarAuditableEntity =
                        new MAuditableEntity(contextEntity as AuditableEntity);

                    string auxiliarDataValue = auxiliarAuditableEntity.GetPropertyValue(EditType);

                    persistingEntity.Name = auxiliarAuditableEntity.Name;
                    persistingEntity.AddMetadataProperty(auxiliarAuditableEntity.MetadataProperties[1].First, auxiliarAuditableEntity.MetadataProperties[1].Second);
                    persistingEntity.AddMetadataProperty(auxiliarAuditableEntity.MetadataProperties[0].First, auxiliarAuditableEntity.MetadataProperties[0].Second);
                    persistingEntity.SetHeaderParam(persistingEntity.Name);

                    if (auxiliarDataValue == CopyKey)
                    {
                        Type = AuditableActionType.CopyAlertResponse;
                        persistingEntity.SetHeaderParam(persistingEntity.Name, auxiliarAuditableEntity.GetPropertyValue(CopiedFromKey));
                    }

                    break;
                case AuditableActionType.AddActionProvider:
                    AddNotificationProvider(ref persistingEntity);
                    break;
                case AuditableActionType.EditActionProvider:
                    //EditNotificationProvider(ref persistingEntity);
                    persistingEntity = null;
                    break;
                case AuditableActionType.RemoveActionProvider:
                    RemoveActionProviderAction(ref persistingEntity);
                    break;
            }

            if(persistingEntity != null)
            {
                persistingEntity.RemovePropertyValueByKey(EditType);
                persistingEntity.RemovePropertyValueByKey(CopiedFromKey);
            }
        }

        /// <summary>
        /// Prepares the entity for logging a remove action
        /// </summary>
        /// <param name="entity"></param>
        private void RemoveActionProviderAction(ref MAuditableEntity entity)
        {
            var contextEntity = MAuditingEngine.Instance.PopAuxiliarData("DeleteNotificationActionEntity");
            if(contextEntity == null)
            {
                entity = null;
                return;
            }

            entity = new MAuditableEntity(contextEntity as AuditableEntity);
            entity.SetHeaderParam(entity.GetPropertyValue("Action"));
            entity.RemovePropertyValueByKey("Action");
        }

        private void EditNotificationProvider(ref MAuditableEntity persistingEntity)
        {
            var contextData = MAuditingEngine.Instance.PopAuxiliarData("OldNotificationProviderInfo");
            
            if(contextData == null)
            {
                persistingEntity = null;
                return;
            }

            persistingEntity = GetAuditableEntity(contextData as NotificationProviderInfo, notificationProviderInfo);
            persistingEntity.SetHeaderParam(persistingEntity.Name);
        }

        private void AddNotificationProvider(ref MAuditableEntity persistingEntity)
        {
            persistingEntity = new MAuditableEntity(notificationProviderInfo.GetAuditableEntity());
            persistingEntity.SetHeaderParam(persistingEntity.Name);
        }

        /// <summary>
        /// Returns an Auditable Entity based on an oldValue
        /// </summary>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        public MAuditableEntity GetAuditableEntity(NotificationProviderInfo oldValue, NotificationProviderInfo newValue)
        {
            var propertiesComparer = new PropertiesComparer();
            List<PropertiesComparer.PropertiesData> changedProperties = propertiesComparer.GetNewProperties(oldValue, newValue);
            var entity = new MAuditableEntity();
            entity.Name = newValue.Name;

            foreach (var property in changedProperties)
            {
                entity.AddMetadataProperty(property.Name, property.Value);
            }

            if (entity.HasMetadataProperties())
            {
                entity.AddMetadataProperty(NotificationProviderType, newValue.Caption);
            }

            return entity;
        }

        /// <summary>
        /// Gets Sets the Action Type
        /// </summary>
        public AuditableActionType Type { get; set; }
    }
}
