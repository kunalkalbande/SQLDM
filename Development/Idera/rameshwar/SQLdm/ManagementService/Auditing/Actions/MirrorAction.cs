using System;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.ManagementService.Configuration;
using Idera.SQLdm.ManagementService.Helpers;

namespace Idera.SQLdm.ManagementService.Auditing.Actions
{
    class MirrorAction : IAuditAction
    {
        private MirroringPartnerActionConfiguration _mirrorConfiguration;
        public MirrorAction(MirroringPartnerActionConfiguration mirrorConfiguration)
        {
            _mirrorConfiguration = mirrorConfiguration;
            switch (_mirrorConfiguration.Action)
            {
                case MirroringPartnerActionConfiguration.MirroringPartnerActions.Failover:
                    Type = AuditableActionType.MirrorFailOver;
                    break;
                case MirroringPartnerActionConfiguration.MirroringPartnerActions.Suspend:
                    Type = AuditableActionType.MirrorSuspend;
                    break;
                case MirroringPartnerActionConfiguration.MirroringPartnerActions.Resume:
                    Type = AuditableActionType.MirrorResume;
                    break;                
            }            
            
        }

        public void FillEntity(ref MAuditableEntity entity)
        {
            if (_mirrorConfiguration == null)
            {
                entity = null;
                return;
            }

            entity.Name = _mirrorConfiguration.Database;
            entity.SetHeaderParam(_mirrorConfiguration.Database);

            var sqlServer = RepositoryHelper.GetMonitoredSqlServer(ManagementServiceConfiguration.ConnectionString,
                                                                    _mirrorConfiguration.MonitoredServerId);            
            entity.MetadataProperties.Clear();
            entity.AddMetadataProperty("Server Name",sqlServer.InstanceName);
            entity.AddMetadataProperty("Database Name",_mirrorConfiguration.Database);
            
        }

        public AuditableActionType Type { get; set; }
    }
}
