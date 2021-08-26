using Idera.SQLdm.RegistrationService.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Idera.SQLdm.RegistrationService
{
    public class CWFRegistrationService : IProduct
    {
        private static readonly BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("SQLdmRegistrationService");

        public void SetDashboardLocation(PluginCommon.NotifyProduct product)
        {
            using (LOG.InfoCall("SetDashboardLocation"))
            {
                LOG.Info("PluginCommon.NotifyProduct JSON : " + product);
                //SQLDM 10.1 (Pulkit Puri)-- SQLDM-26208 fix
                if (string.IsNullOrEmpty(product.Password) || string.IsNullOrEmpty(product.User) || string.IsNullOrEmpty(product.DisplayName))
                {
                    LOG.Error("Error occured on setting dashboard location. UserName, Password or DisplayName for product cannot be null or empty.");
                    throw new FaultException<WebFaultException>(new WebFaultException(System.Net.HttpStatusCode.BadRequest), "UserName, Password or DisplayName for product cannot be null or empty.");
                }
                //SQLDM 10.2.2 -- SQLDM-28150 code changes to display the widgets after reregistering
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        CommonWebFramework cwfDetails = CommonWebFramework.GetInstance();
                        CWFHelper cwfHelper = new CWFHelper(cwfDetails);
                        cwfHelper.SetDashboardLocation(product);
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Error occured on setting dashboard location", e);
                        throw new WebFaultException(HttpStatusCode.InternalServerError);
                    }
                });
            }
        }

        public Stream GetProductData()
        {
            using (LOG.InfoCall("GetProductData"))
            {
                try
                {
                    CommonWebFramework cwfDetails = CommonWebFramework.GetInstance();
                    CWFHelper cwfHelper = new CWFHelper(cwfDetails);
                    byte[] formData = cwfHelper.GetProductData();
                    return new MemoryStream(formData);
                }
                catch (Exception e)
                {
                    LOG.Error("Error occured on getting product data", e);
                    throw new WebFaultException(HttpStatusCode.InternalServerError);
                }
            }
        }

    }
}
