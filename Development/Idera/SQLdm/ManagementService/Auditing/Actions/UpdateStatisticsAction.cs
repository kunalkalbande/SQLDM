using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.ManagementService.Helpers;

namespace Idera.SQLdm.ManagementService.Auditing.Actions
{
    class UpdateStatisticsAction:IAuditAction
    {
        private UpdateStatisticsConfiguration _updateStatistics;

        public UpdateStatisticsAction(UpdateStatisticsConfiguration updateStatistics)
        {
            _updateStatistics = updateStatistics;
            Type = AuditableActionType.UpdateStatistics;
        }

        public void FillEntity(ref MAuditableEntity entity)
        {            
            entity.SetHeaderParam(entity.SqlUser);
            entity.MetadataProperties.Clear();            
            entity.AddMetadataProperty("Data Base Name", _updateStatistics.DatabaseName);            

            ManagementService managementService =new ManagementService();                
            TableDetailConfiguration configuration = new TableDetailConfiguration(_updateStatistics.MonitoredServerId, _updateStatistics.TableId, _updateStatistics.DatabaseName);            
            TableDetail tableDetail =managementService.GetTableDetails(configuration);
            entity.Name = tableDetail.TableName;                        
            entity.AddMetadataProperty("Table Name", tableDetail.TableName);            
        }

        public AuditableActionType Type { get; set; }
    }
}
