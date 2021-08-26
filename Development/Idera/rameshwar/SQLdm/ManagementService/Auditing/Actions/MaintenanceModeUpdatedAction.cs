//------------------------------------------------------------------------------
// <copyright file="CustomAttributes.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.ManagementService.Auditing.Actions
{
    /// <summary>
    /// Prepares the environment to log changes for Maintenance Mode
    /// </summary>
    class MaintenanceModeUpdatedAction : IAuditAction
    {
        private MonitoredSqlServerConfiguration _newConfiguration;
        private MonitoredSqlServerConfiguration _oldConfiguration;
        private EditServerAction _auxiliarAction;

        public AuditableActionType Type { get; set; }

        /// <summary>
        /// Recieves the new Maintenance configuration and the Old configuration
        /// </summary>
        /// <param name="newConfiguration"></param>
        /// <param name="oldConfiguration"></param>
        public MaintenanceModeUpdatedAction(MonitoredSqlServerConfiguration newConfiguration, MonitoredSqlServerConfiguration oldConfiguration)
        {
            this._newConfiguration = newConfiguration;
            this._oldConfiguration = oldConfiguration;

            _auxiliarAction = new EditServerAction(newConfiguration, oldConfiguration);
            
        }

        /// <summary>
        /// Recieves an MAuditableEntity which already has the default values like SQLUser, and then adds more flavors to it according to the case.
        /// </summary>
        /// <param name="entity"></param>
        public void FillEntity(ref MAuditableEntity auditableEntity)
        {
            auditableEntity = new MAuditableEntity(_newConfiguration.GetAuditableEntity());
            
            AuditableEntity entity=new AuditableEntity();

            _auxiliarAction.SetMaintenanceMode(_newConfiguration, _oldConfiguration, ref entity);

            foreach (var property in entity.MetadataProperties)
            {
                auditableEntity.AddMetadataProperty(property.First,property.Second);    
            }

            if (!auditableEntity.HasMetadataProperties())
            {
                auditableEntity = null;
                return;
            }

            GetMaintenanceModeActionTypeText(_newConfiguration);
        }

        /// <summary>
        /// According to the Maintenance change, it returns a string with the description of the text.
        /// </summary>
        /// <param name="serverConfiguration"></param>
        /// <returns></returns>
        private void GetMaintenanceModeActionTypeText(MonitoredSqlServerConfiguration serverConfiguration)
        {
            switch (serverConfiguration.MaintenanceMode.MaintenanceModeType)
            {
                case MaintenanceModeType.Always:
                    Type = AuditableActionType.MaintenanceModeManuallyEnabled;
                    break;
                case MaintenanceModeType.Never:
                    Type = AuditableActionType.MaintenanceModeManuallyDisabled;
                    break;
                case MaintenanceModeType.Recurring:
                    Type = AuditableActionType.MaintenanceModeScheduleChanged;
                    break;
                case MaintenanceModeType.Once:
                    Type = AuditableActionType.MaintenanceModeScheduleChanged;
                    break;
                default:
                    Type = AuditableActionType.None;
                    break;
            }
        }
    }
}
