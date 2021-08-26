using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Microsoft.Win32;
// SQLdm 10.0 (Rajesh Gupta) : LM 2.0 Integration - Register License Manger,Write Registry Keys
namespace Idera.SQLdm.ManagementService.Helpers
{
    internal static class LMHelper
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("LMHelper");
        #region Methods

        public static  void RegisterLicenseManager()
        {
            try
            {
                LOG.Info("Writing LM Registry Keys.");
                CommonWebFramework cwfDetails = CommonWebFramework.GetInstance();
                string baseUrl = cwfDetails.BaseURL + @"/" + Common.Constants.PRODUCT_SHORT_NAME + @"/" + cwfDetails.ProductID;
                string pVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                int index = pVersion.IndexOf('.', pVersion.IndexOf('.') + 1);
                pVersion = pVersion.Remove(index);
                string pCode = Idera.SQLdm.Common.Constants.ProductId.ToString();
                if (!string.IsNullOrEmpty(baseUrl))
                {
                    RegistryHelper.WriteToRegistry(Common.Constants.REGISTRY_PATH_LM, "BaseUrl", baseUrl);
                    RegistryHelper.WriteToRegistry(Common.Constants.REGISTRY_PATH_LM, "PVersion", pVersion);
                    RegistryHelper.WriteToRegistry(Common.Constants.REGISTRY_PATH_LM, "PCode", pCode);                    
                }
                LOG.Info("LM Registry Keys written.");
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occured while writing LM Registry Keys", ex);
            }
        }
        #endregion

    }
}
