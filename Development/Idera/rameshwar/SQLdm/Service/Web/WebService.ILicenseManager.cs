﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdm.Service.ServiceContracts.v1;
using Idera.SQLdm.Service.Repository;
using Idera.SQLdm.Service.Helpers;
using Idera.SQLdm.Service.Configuration;
using Idera.SQLdm.Common.Auditing;
using BBS.License;
using System.Data.SqlClient;
using Idera.SQLdm.Common;
using System.Net;
using System.ServiceModel.Web;
using System.Data;
using Idera.SQLdm.Common.Services;
using Microsoft.ApplicationBlocks.Data;
using Idera.SQLdm.Common.Messages;
using Idera.SQLdm.Service.DataContracts.v1.Errors;
using Microsoft.VisualBasic;


namespace Idera.SQLdm.Service.Web
{
    public partial class WebService : ILicenseManager, Idera.LicenseManager.ProductPlugin.ServiceContracts.ILicenseManager
    {

        #region ILicenseManager Members

        public DataContracts.v1.License.LicenseDetails GetLicense()
        {
            SetConnectionCredentiaslFromCWFHost();
            return ConvertToDataContract.ToDC(RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo));
        }

        #endregion
        //[START] SQLdm 10.0 (Rajesh Gupta): LM 2.0 Integration - Implementing ILicenseManager Interface
        public bool ApplyLicenseKey(Idera.LicenseManager.ProductPlugin.DataContracts.License license)
        {

            {
                try
                {
                    SetConnectionCredentiaslFromCWFHost();
                }
                catch (Exception ex)
                {
                    _logX.ErrorFormat("An error occurred authenticating : {0}", ex);
                    throw new LicenseManagerException("Authentication Failed : " + ex.Message, true);
                }
                try
                {
                    var licSummary = RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo);
                    BBSLic lic = RepositoryHelper.LoadKeyString(license.Key);
                    if (lic != null)
                    {
                        // Key was loaded successfully.  Check for invalid properties.
                        if (lic.ProductID != Common.Constants.ProductId)
                        {
                            throw new LicenseManagerException("The specified key is for a different product.");
                        }
                        //Start: SQL DM 9.0 (Vineet Kumar) (License Changes) -- Adding check for license key version. Entered key should be for >-9.0 version or it should be trial
                        else if (!lic.IsTrial && lic.ProductVersion.Major < 9)
                        {
                            throw new LicenseManagerException(@"The license key is for an older version of SQL diagnostic manager.  Please generate a new license key from ""generate New Key"" option ");
                        }
                        //End: SQL DM 9.0 (Vineet Kumar) (License Changes) -- Adding check for license key version. Entered key should be for >-9.0 version or it should be trial
                        else if (lic.IsExpired)
                        {
                            throw new LicenseManagerException("The specified key has already expired.");
                        }
                        else if (!lic.CheckScopeHash(licSummary.Repository))
                        {
                            throw new LicenseManagerException(string.Format("The specified key cannot be used with repository {0}", licSummary.Repository));
                        }
                        else if (!LicenseSummary.IsLicenseReasonable(lic))
                        {
                            throw new LicenseManagerException("The specified key is invalid");
                        }
                        else
                        {
                            RepositoryHelper.AddOrReplaceKey(lic, license.Key);
                            return true;
                        }
                    }
                    else
                        throw new LicenseManagerException("No key was specified");
                }
                catch (LicenseManagerException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    _logX.ErrorFormat("An error occurred while Applying Key : {0}", ex);
                    throw new LicenseManagerException("An error occurred while Applying Key : " + ex.Message);
                }
            }
        }

        public Idera.LicenseManager.ProductPlugin.DataContracts.EnvironmentalData GetEnvironmentalData()
        {
            try
            {
                SetConnectionCredentiaslFromCWFHost();
            }
            catch (Exception ex)
            {
                _logX.ErrorFormat("An error occurred authenticating : {0}", ex);
                throw new LicenseManagerException("Authentication Failed : " + ex.Message, true);
            }
            try
            {
                Idera.LicenseManager.ProductPlugin.DataContracts.EnvironmentalData EnvironmentalData = new Idera.LicenseManager.ProductPlugin.DataContracts.EnvironmentalData();
                EnvironmentalData.CPUCount = Convert.ToString(Environment.ProcessorCount);
                EnvironmentalData.Memory = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory.ToString();
                EnvironmentalData.OSVersion = Convert.ToString(Environment.OSVersion);
                using (SqlConnection connection = RestServiceConfiguration.SQLConnectInfo.GetConnection(Idera.SQLdm.Common.Constants.DesktopClientConnectionStringApplicationName))
                {
                    connection.Open();
                    EnvironmentalData.SQLServerVersion = connection.ServerVersion;
                }
                return EnvironmentalData;
            }
            catch (Exception ex)
            {
                _logX.ErrorFormat("An error occurred while fetching environmental info : {0}", ex);
                throw new LicenseManagerException("An error occurred while fetching Environmental Data : " + ex.Message); ;
            }
        }

        List<Idera.LicenseManager.ProductPlugin.DataContracts.LicenseKey> Idera.LicenseManager.ProductPlugin.ServiceContracts.ILicenseManager.GetLicenseDetails()
        {

            try
            {
                SetConnectionCredentiaslFromCWFHost();
            }
            catch (Exception ex)
            {
                _logX.ErrorFormat("An error occurred authenticating : {0}", ex);
                throw new LicenseManagerException("Authentication Failed : " + ex.Message, true);
            }
            try
            {
                List<Idera.LicenseManager.ProductPlugin.DataContracts.LicenseKey> LicenseKeysDetails = new List<Idera.LicenseManager.ProductPlugin.DataContracts.LicenseKey>();
                var licSummary = RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo);
                foreach (var checkedKey in licSummary.CheckedKeys)
                {
                    Idera.LicenseManager.ProductPlugin.DataContracts.LicenseKey licKey = new Idera.LicenseManager.ProductPlugin.DataContracts.LicenseKey();
                    licKey.Expiration = Convert.ToString(checkedKey.KeyObject.ExpirationDate);
                    licKey.Instances = checkedKey.KeyObject.Limit1;
                    licKey.IsUnlimited = (checkedKey.KeyObject.Limit1 == BBSLic.Unlimited);
                    licKey.Key = checkedKey.KeyString;
                    licKey.LicenseStatus = Convert.ToString(licSummary.Status);
                    licKey.Scope = licSummary.Repository;
                    licKey.IsEnterprise = checkedKey.KeyObject.IsEnterprise;//Incorporated new build of licence manager.
                    if (checkedKey.KeyObject.IsTrial)
                    {
                        licKey.Type = "Trial";
                    }
                    else
                        licKey.Type = "Production";
                    LicenseKeysDetails.Add(licKey);
                }
                return LicenseKeysDetails;
            }
            catch (Exception ex)
            {
                _logX.ErrorFormat("An error occurred while fetching license details : {0}", ex);
                throw new LicenseManagerException("An error occurred while fetching license details : " + ex.Message);
            }
        }

        public Idera.LicenseManager.ProductPlugin.DataContracts.LicenseSummary GetLicenseSummary()
        {
            try
            {
                SetConnectionCredentiaslFromCWFHost();
            }
            catch (Exception ex)
            {
                _logX.ErrorFormat("An error occurred authenticating : {0}", ex);
                throw new LicenseManagerException("Authentication Failed : " + ex.Message, true);
            }
            try
            {
                Idera.LicenseManager.ProductPlugin.DataContracts.LicenseSummary licenseSummary = new Idera.LicenseManager.ProductPlugin.DataContracts.LicenseSummary();
                //fill the values from repository
                var licSummary = RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo);
                licenseSummary.IsUnlimited = licSummary.IsUnlimited;
                licenseSummary.TotalKeys = Convert.ToString(licSummary.LicensedServers);
                licenseSummary.KeysUsed = Convert.ToString(licSummary.MonitoredServers);
                return licenseSummary;
            }
            catch (Exception ex)
            {
                _logX.ErrorFormat("An error occurred while fetching License summary : {0}", ex);
                throw new LicenseManagerException("An error occurred while fetching license summary : " + ex.Message);
            }
        }


        public Idera.LicenseManager.ProductPlugin.DataContracts.Product GetProductInfo()
        {
            try
            {
                SetConnectionCredentiaslFromCWFHost();
            }
            catch (Exception ex)
            {
                _logX.ErrorFormat("An error occurred authenticating : {0}", ex);
                throw new LicenseManagerException("Authentication Failed : " + ex.Message, true);
            }
            try
            {
                Idera.LicenseManager.ProductPlugin.DataContracts.Product product = new Idera.LicenseManager.ProductPlugin.DataContracts.Product();
                product.ProductCode = Common.Constants.ProductId;
                product.ProductName = Common.Constants.PRODUCT_SHORT_NAME;
                product.ProductVersion = (new CommonAssemblyInfo()).GetCommonAssemblyVersion();
                return product;
            }
            catch (Exception ex)
            {
                _logX.ErrorFormat("An error occurred while fetching product information : {0}", ex);
                throw new LicenseManagerException("An error occurred while fetching license summary : " + ex.Message);
            }
        }

        public string Ping()
        {
            try
            {
                SetConnectionCredentiaslFromCWFHost();
            }
            catch (Exception ex)
            {
                _logX.ErrorFormat("An error occurred authenticating : {0}", ex);
                throw new LicenseManagerException("Authentication Failed : " + ex.Message, true);
            }
            return "Success";
        }

        public bool ReleaseLicenseKey(Idera.LicenseManager.ProductPlugin.DataContracts.License license)
        {

            {
                try
                {
                    SetConnectionCredentiaslFromCWFHost();
                }
                catch (Exception ex)
                {
                    _logX.ErrorFormat("An error occurred authenticating : {0}", ex);
                    throw new LicenseManagerException("Authentication Failed : " + ex.Message, true);
                }
                try
                {
                    List<string> keys = new List<string>();
                    Dictionary<String, String> licensesServerNumber = new Dictionary<string, string>();
                    var licSummary = RepositoryHelper.GetLicenseKeys(RestServiceConfiguration.SQLConnectInfo);
                    List<CheckedKey> keyList = new List<CheckedKey>();
                    CheckedKey ck = new CheckedKey(license.Key);
                    keyList.Add(ck);
                    foreach (CheckedKey item in keyList)
                    {
                        CheckedKey listObject = item;
                        keys.Add(listObject.KeyString);
                        licensesServerNumber.Add(listObject.KeyString, listObject.KeyObject.Limit1.ToString());
                    }
                    int flag = 0;
                    foreach (CheckedKey item in licSummary.CheckedKeys)
                    {
                        if (item.KeyString.Equals(ck.KeyString))
                        {
                            flag = 1;
                        }
                    }
                    if (flag == 0)
                    {
                        throw new LicenseManagerException("Specified Key Does Not Exist");
                    }
                    try
                    {
                        //to audit licenses delete
                        foreach (var licenses in licensesServerNumber)
                        {
                            RepositoryHelper.SetAuditableEntity(LicenseKeyOperation.Remove, licenses.Key, licenses.Value);
                        }

                        LicenseSummary license1 = RepositoryHelper.SetLicenseKeys(LicenseKeyOperation.Remove, keys);
                        return true;

                    }
                    catch (Exception ex)
                    {
                        _logX.ErrorFormat("An error occurred while releasing a key : {0}", ex);
                        throw new LicenseManagerException("An error occurred while removing license keys.");
                    }
                }
                catch (LicenseManagerException ex)
                {
                    _logX.ErrorFormat("An error occurred while releasing a key : {0}", ex);
                    throw;
                }
                catch (Exception ex)
                {
                    _logX.ErrorFormat("An error occurred while releasing a key : {0}", ex);
                    throw new LicenseManagerException("An error occurred while removing license keys : " + ex.Message);
                }
            }
        }

        public bool UpdateUsedLicenses(string usedLicense)
        {
            throw new NotImplementedException();
        }
        //[END] SQLdm 10.0 (Rajesh Gupta): LM 2.0 Integration - Implementing ILicenseManager Interface
    }
}
