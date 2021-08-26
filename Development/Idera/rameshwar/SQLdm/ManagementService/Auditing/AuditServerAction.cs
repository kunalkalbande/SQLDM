using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.ManagementService.Configuration;
using Idera.SQLdm.ManagementService.Helpers;

namespace Idera.SQLdm.ManagementService.Auditing
{
    class AuditServerAction:IAuditAction
    {
        private int _id;
        private bool _deleteAll;

        public AuditServerAction(int serverId,bool deleteAll)
        {
            _id = serverId;
            _deleteAll = deleteAll;
            Type=AuditableActionType.DeleteServer;
        }


        public AuditableActionType Type { get; set; }

        public void FillEntity(ref MAuditableEntity entity)
        {
            MonitoredSqlServer sqlServer =
                RepositoryHelper.GetMonitoredSqlServer(ManagementServiceConfiguration.ConnectionString, _id);
            entity.Name = sqlServer.InstanceName;
            entity.SetHeaderParam(sqlServer.InstanceName);
            entity.MetadataProperties.Clear();            
            if (_deleteAll)
            {
                entity.AddMetadataProperty("Description",
                                           "Monitored SQL Server deleted. Removed all collected data for this instance.");
            }
            else
            {
                entity.AddMetadataProperty("Description", " Monitored SQL Server deleted. Preserved all collected data for this instance.");
            }
        }
    }
}
