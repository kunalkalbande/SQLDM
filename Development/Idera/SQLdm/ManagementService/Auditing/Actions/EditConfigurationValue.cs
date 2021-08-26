using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.ManagementService.Configuration;
using Idera.SQLdm.ManagementService.Helpers;

namespace Idera.SQLdm.ManagementService.Auditing.Actions
{
    class EditConfigurationValue:IAuditAction
    {
        private ReconfigurationConfiguration _reconfigurationConfiguration;
        private MonitoredSqlServer _sqlServer;

        public EditConfigurationValue(ReconfigurationConfiguration reconfigurationConfiguration)
        {
            _reconfigurationConfiguration = reconfigurationConfiguration;

            if (_reconfigurationConfiguration == null)
            {
                _sqlServer = null;
                return;
            }

            _sqlServer = RepositoryHelper.GetMonitoredSqlServer(ManagementServiceConfiguration.ConnectionString,
                                                                   reconfigurationConfiguration.MonitoredServerId);

            Type= AuditableActionType.ConfigurationValueChanged;
        }

        public void FillEntity(ref MAuditableEntity entity)
        {               
            entity.Name = _reconfigurationConfiguration.ConfigurationName;
            entity.MetadataProperties.Clear();
            entity.SetHeaderParam(_reconfigurationConfiguration.ConfigurationName);
            entity.AddMetadataProperty("New Value",_reconfigurationConfiguration.RunValue.ToString());
            entity.AddMetadataProperty("Server Name",_sqlServer.InstanceName);
        }

        public AuditableActionType Type { get; set; }
    }
}
