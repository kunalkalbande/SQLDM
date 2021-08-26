using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Services
{
    public interface IBaselineAnalysis
    {
        /// <summary>
        /// Returns the list of CURRENT baseline configurations for each server (key = serverId).
        /// </summary>
        /// <returns></returns>
        Dictionary<int, List<BaselineConfiguration> > GetBaselineConfigurations();//SQLdm 10.0 (Tarun Sapra) : get the list of all the configs

        /// <summary>
        /// Calculates and saves the baseline for the given server using the given template
        /// </summary>
        /// <param name="serverid"></param>
        /// <param name="?"></param>
        void CalculateBaseline(int serverid, BaselineConfiguration config);

        /// <summary>
        /// SQLdm 10.1 (Srishti Purohit) - Refresh workload method for async workload synchronization
        /// </summary>
        /// <param name="serverid"></param>
        /// <param name="?"></param>
        void RefreshServerWorkload(int serverid, bool sendStopTrace = false);
    }
}
