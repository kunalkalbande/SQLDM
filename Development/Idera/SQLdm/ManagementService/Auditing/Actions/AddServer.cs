using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.ManagementService.Auditing.Actions
{
    class AddServer: IAuditAction
    {
        private MonitoredSqlServer _instance;

        public AddServer(MonitoredSqlServer instance)
        {
            this._instance = instance;
            this.Type = AuditableActionType.AddServer;

        }

        public AuditableActionType Type { get; set; }

        public void FillEntity(ref MAuditableEntity entity)
        {
            AuditableEntity serverEntity =  _instance.GetAuditableEntity();
            entity.Name = serverEntity.Name;
            entity.MetadataProperties.Clear();
            entity.SetHeaderParam(serverEntity.Name);

            foreach (var property in serverEntity.MetadataProperties)
            {
                entity.AddMetadataProperty(property.First, property.Second);
            }
        }
    }
}
