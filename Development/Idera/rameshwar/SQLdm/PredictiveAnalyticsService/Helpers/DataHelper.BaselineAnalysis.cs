using System;
using System.Collections.Generic;
using System.Text;

using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.PredictiveAnalyticsService.Configuration;
using Idera.SQLdm.Common.Services;

namespace Idera.SQLdm.PredictiveAnalyticsService.Helpers
{
    internal static partial class DataHelper
    {
        private static IBaselineAnalysis GetBaselineManagementService()
        {
            string address = PredictiveAnalyticsConfiguration.ManagementServiceAddress;
            int    port    = PredictiveAnalyticsConfiguration.ManagementServicePort;

            try
            {
                Uri               uri   = new Uri(String.Format("tcp://{0}:{1}/Management", address, port));
                ServiceCallProxy  proxy = new ServiceCallProxy(typeof(IBaselineAnalysis), uri.ToString());
                IBaselineAnalysis iba   = proxy.GetTransparentProxy() as IBaselineAnalysis;

                return iba;
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception contacting managment service.", ex);
                return null;
            }
        }

        public static Dictionary<int, List<BaselineConfiguration>> GetBaselineConfigurations() //SQLdm 10.0 (Tarun Sapra) : returning list of configs
        {
            IBaselineAnalysis service = GetBaselineManagementService();

            if (service == null)
                return new Dictionary<int, List<BaselineConfiguration> >(); 

            try
            {
                return service.GetBaselineConfigurations();
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in GetBaselineConfigurations.", ex);
                return new Dictionary<int, List<BaselineConfiguration> >();
            }
        }

        public static void CalculateBaseline(int serverid, BaselineConfiguration config)
        {
            IBaselineAnalysis service = GetBaselineManagementService();

            if (service == null)
                return;

            try
            {
                service.CalculateBaseline(serverid, config);
                LOG.Info("Reloading ServerWorkload object after Calculating Baselines in PA service.");
                service.RefreshServerWorkload(serverid);
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in CalculateBaseline.", ex);
            }            
        }
    }
}
