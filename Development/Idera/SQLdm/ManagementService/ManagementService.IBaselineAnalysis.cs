using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.ManagementService.Helpers;
using Idera.SQLdm.ManagementService.Configuration;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.ManagementService
{
    public partial class ManagementService : IBaselineAnalysis
    {        
        public Dictionary<int, List<Common.Configuration.BaselineConfiguration> > GetBaselineConfigurations() //SQLdm 10.0 (Tarun Sapra) : returning list of configs 
        {
            using (LOG.InfoCall("GetBaselineConfigurations"))
            {
                try
                {
                    return RepositoryHelper.GetBaselineConfigurations(ManagementServiceConfiguration.ConnectionString);
                }
                catch (Exception e)
                {
                    string message = "An error occurred in GetBaselineConfigurations.";
                    LOG.Error(message, e);
                    throw new ManagementServiceException(message, e);
                }
            }
        }

        public void CalculateBaseline(int serverid, BaselineConfiguration config)
        {
            using (LOG.InfoCall("CalculateBaseline"))
            {
                try
                {
                    RepositoryHelper.CalculateBaseline(ManagementServiceConfiguration.ConnectionString, serverid, config.TemplateID, config.Template);
                }
                catch (Exception ex)
                {
                    string message = "An error occurred in GetBaselineConfigurations.";
                    LOG.Error(message, ex);
                    throw new ManagementServiceException(message, ex);
                }
            }
        }
        public void RefreshServerWorkload(int serverid, bool sendStopTrace = false)
        {
            string connectString = ManagementServiceConfiguration.ConnectionString;
            MonitoredSqlServer instance = RepositoryHelper.GetMonitoredSqlServer(connectString, serverid);

            //SQLdm 10.1 (Srishti Purohit) - Refresh workload method for async workload synchronization
            SendUpdatedServerWorkload(instance, false);
        }
    }
}
