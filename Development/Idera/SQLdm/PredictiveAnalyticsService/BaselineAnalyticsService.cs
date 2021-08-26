using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.PredictiveAnalyticsService.Helpers;
using Idera.SQLdm.Common.Data;

namespace Idera.SQLdm.PredictiveAnalyticsService
{
    static class BaselineAnalyticsService
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("BaselineAnalyticsService");
        
        public static void PerformAnalysis()
        {
            try
            {
                // load the server baseline templates
                Dictionary<int, List<BaselineConfiguration> > baselineConfigurations = DataHelper.GetBaselineConfigurations();

                //START SQLdm 10.0 (Tarun Sapra) : process every template from the list for every server id

                // loop through the configurations and calc baseline
                // the calc also saves to repo
                foreach (int serverid in baselineConfigurations.Keys)
                {
                    foreach (BaselineConfiguration config in baselineConfigurations[serverid])
                    {
                        LOG.Debug("Calculating baseline for server id " + serverid);
                        DataHelper.CalculateBaseline(serverid, config);
                    }
                }

                //END SQLdm 10.0 (Tarun Sapra) : process every template from the list for every server id
            }
            catch (Exception ex)
            {
                LOG.Error("An error was encountered during baseline analysis.", ex);
            }
        }
        
    }
}
