// ===============================
// AUTHOR       : CWF Team - Gowrish 
// PURPOSE      : Backend Isolation
// TICKET       : SQLDM-29086
// ===============================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using BBS.TracerX;
using PluginCommon;
using Idera.SQLdm.Service.DataModels;
using Constants = Idera.SQLdm.Common.Constants;
using System.ServiceModel.Web;
using Idera.SQLdm.Common.Helpers;


namespace Idera.SQLdm.Service.Helpers.CWF
{
    public class CWFHelper
    {
        private static readonly Logger LogX = Logger.GetLogger("CWFHelper");
		private const string CWF_BASE_URI = "/IderaCoreServices/v1";

        public static Principal GetPrincipal(String authHeader)
        {
            using (LogX.InfoCall("GetPrincipal"))
            {
                AuthenticationHeaderRequest request = new AuthenticationHeaderRequest();
                request.AuthenticationHeader = authHeader;
                request.ProductId = Convert.ToInt32(RegistryHelper.GetValueFromRegistry("ProductID"));

                String requestJson = JsonHelper.ToJSON<AuthenticationHeaderRequest>(request);

                GetPrincipalResponse principalResponse = HttpRequestHelper.Post<GetPrincipalResponse>(requestJson, GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.GetPrincipal), authHeader);
                User user = JsonHelper.FromJSON<User>(Decrypt(principalResponse.user));
                IPrincipal principal = getPrincipalObject(principalResponse.principal);
                return new Principal(user, principal);
            }
        }

        public static void CreateTags(CreateTags tags)
        {
            using (LogX.InfoCall("CreateTags"))
            {
                String requestJson = JsonHelper.ToJSON<CreateTags>(tags);
                var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                HttpRequestHelper.Post<Tags>(requestJson, GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.SyncTags), header);
            }
        }

        public static void AddTagResources(TagResourcesList tagResourceList)
        {
            using (LogX.InfoCall("AddTagResources"))
            {
                foreach (TagResource tagResource in tagResourceList)
                {
                    String requestJson = JsonHelper.ToJSON<TagResource>(tagResource);
                    var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                    HttpRequestHelper.Post<TagResource>(requestJson, GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.SyncResourcesToTag), header);
                }
            }
        }

        public static void RefreshToken()
        {
            using (LogX.InfoCall("RefreshToken"))
            {
                RefreshToken request = new RefreshToken();
                request.EncryptedToken = RegistryHelper.GetValueFromRegistry(Constants.RefreshTokenRegistryKey).ToString();
                String requestJson = JsonHelper.ToJSON<RefreshToken>(request);
                var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                String newAuthToken = HttpRequestHelper.Post<String>(requestJson, GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.RefreshToken), header);
                //Constants.AuthToken = newAuthToken;
                GenericHelpers.UpdateRegistryForAuthTokenAndRefreshToken(newAuthToken, null);
            }
        }

        public static Tag GetTag(string tagId)
        {
            using (LogX.InfoCall("GetTag"))
            {
                var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                return HttpRequestHelper.Get<Tag>(String.Format(GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.GetTag), tagId), header);
            }

        }

        public static void DeleteTagResources(TagResourcesList resourceList)
        {
            using (LogX.InfoCall("DeleteTagResources"))
            {
                var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                foreach (TagResource tagResource in resourceList)
                {
                    String requestJson = JsonHelper.ToJSON<TagResource>(tagResource);
                    HttpRequestHelper.Delete(requestJson, GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.SyncResourcesToTag), header);
                }
            }
        }

        public static void DeleteTags(Tags tags)
        {
            using (LogX.InfoCall("DeleteTags"))
            {
                foreach (Tag tag in tags)
                {
                    String requestJson = JsonHelper.ToJSON<Tag>(tag);
                    var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                    HttpRequestHelper.Delete(requestJson, GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.SyncTags), header);
                }
            }
                
            
        }

        private static IPrincipal getPrincipalObject(EncryptedData encryptedData)
        {
            using (LogX.InfoCall("Deserialize principal response"))
            {
                String decryptedData = Decrypt(encryptedData);
                byte[] principalDataBytes = Convert.FromBase64String(decryptedData);
                IPrincipal principal = null;
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream(principalDataBytes))
                {
                    principal = (IPrincipal)bf.Deserialize(ms);
                }
                return principal;
            }
        }

        private static String Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            using (LogX.InfoCall("Decrypttor"))
            {
                string plaintext = null;


                // SQLDM-30032--Create an Aes algoritham object to make it work with FIPS complaint 
                using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
                {
                    //Set the encryption key and the IV to decrypt
                    aesAlg.KeySize = 256;
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;

                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption. 
                    using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt, Encoding.UTF8))
                            {
                                // Read the decrypted bytes from the decrypting stream 
                                // and place them into the string object
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                return plaintext;
            }
        }

        public static String Decrypt(EncryptedData encryptedData)
        {
            using (LogX.InfoCall("Decrypt"))
            {
                byte[] cipherText = Convert.FromBase64String(encryptedData.data);
                if (cipherText == null || cipherText.Length <= 0)
                {
                    throw new ArgumentNullException("cipherText");
                }
                byte[] IV = Convert.FromBase64String(encryptedData.iv);
                if (IV == null || IV.Length <= 0)
                {
                    throw new ArgumentNullException("IV");
                }
                byte[] keyBytes = generateHashKey();
                return Decrypt(cipherText, keyBytes, IV);
            }
        }

        private static byte[] generateHashKey()
        {
            using (LogX.InfoCall("Generate hash key"))
            {
                String version = RegistryHelper.GetValueFromRegistry("Version",Constants.REGISTRY_PATH).ToString();
                //Currently CWF stores the version as 10.3.0.0 irrespective of what suffix is. Hence CWF encrypts using 10.3.0.0 and so DM need to decrypt using same 10.3.0.0
                //version = "10.3.0.0";
                String instance = RegistryHelper.GetValueFromRegistry("DisplayName").ToString();
                String encryptionKey = GenericHelpers.ConcatStrings(RegistryHelper.GetValueFromRegistry("ProductID").ToString(), Constants.Underscore, Constants.ProductName, Constants.Underscore, instance, Constants.Underscore, version);
                //SQLDM-30032--Modify MD5 algoritham to to SHA 256 algorithm for hashing to make it work with FIPS complaint 
                HashAlgorithm algorithm = SHA256.Create();
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey.ToUpper()));
            }
        }

        public static Tags GetTags(String userAuthHeader)
        {
            using (LogX.InfoCall("GetTags"))
            {
                Tags tags = HttpRequestHelper.Get<Tags>(String.Format(GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.GetTags), RegistryHelper.GetValueFromRegistry("ProductID").ToString()), userAuthHeader);
                return tags;
            }
        }

        public static ProductInstances GetTagInstances(String tagId, String userAuthHeader)
        {
            using (LogX.InfoCall("GetTagInstances"))
            {
                ProductInstances instances = HttpRequestHelper.Get<ProductInstances>(String.Format(GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.GetTagInstances), tagId), userAuthHeader);
                return instances;
            }
        }

        /// <summary>
        /// Get CWF Alerts for the product
        /// </summary>
        /// <param name="productIdAsString">Product Id</param>
        /// <param name="userAuthHeader">Authentication Header</param>
        /// <returns></returns>
        public static Alerts GetCwfAlerts(string productIdAsString, string userAuthHeader)
        {
            using (LogX.InfoCall("GetCwfAlerts"))
            {
                Alerts alerts = HttpRequestHelper.Get<Alerts>(GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.SyncAlerts) + "?productId=" + productIdAsString, userAuthHeader);
                return alerts;
            }
        }

        /// <summary>
        /// Deletes the Alerts in the CWF
        /// </summary>
        /// <param name="alerts">Alerts to be removed</param>
        /// <param name="userAuthHeader">Authentication Header</param>
        public static void DeleteCwfAlerts(Alerts alerts, string userAuthHeader)
        {
            using (LogX.InfoCall("DeleteCwfAlerts"))
            {
                String requestJson = JsonHelper.ToJSON<Alerts>(alerts);
                HttpRequestHelper.Delete(requestJson, GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.SyncAlerts), userAuthHeader);
            }
        }

        public static void SynchronizeAlerts(Alerts alerts, String userAuthHeader)
        {
            using (LogX.InfoCall("SynchronizeAlerts"))
            {
                String requestJson = JsonHelper.ToJSON<Alerts>(alerts);
                HttpRequestHelper.Post<String>(requestJson, GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.SyncAlerts), userAuthHeader);
            }
        }

        public static CreateInstanceResults SynchronizeInstances(CreateInstances instances, String userAuthHeader)
        {
            using (LogX.InfoCall("SynchronizeInstances"))
            {
                String requestJson = JsonHelper.ToJSON<CreateInstances>(instances);
                CreateInstanceResults instanceResults = HttpRequestHelper.Post<CreateInstanceResults>(requestJson, String.Format(GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.SyncInstances), RegistryHelper.GetValueFromRegistry("ProductID").ToString()), userAuthHeader);
                return instanceResults;
            }
        }

        public static ProductInstances RegisterInstances(RegisterProductInstances instances, String userAuthHeader)
        {
            using (LogX.InfoCall("RegisterInstances"))
            {
                String requestJson = JsonHelper.ToJSON<RegisterProductInstances>(instances);
                ProductInstances instanceResults = HttpRequestHelper.Post<ProductInstances>(requestJson, String.Format(GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.RegisterInstances), RegistryHelper.GetValueFromRegistry("ProductID").ToString()), userAuthHeader);
                return instanceResults;
            }
        }

        public static void AssignInstancePermissions(string productId, string permissionId, Instance instance)
        {
            using (LogX.InfoCall("AssignInstancePermissions"))
            {
                String requestJson = JsonHelper.ToJSON<Instance>(instance);
                var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                HttpRequestHelper.Post<ProductInstances>(requestJson, String.Format(GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.RegisterInstances) + "?productId=" + productId + "&permissionId=" + permissionId, RegistryHelper.GetValueFromRegistry("ProductID").ToString()), header);
            }
        }

        public static void AddWidgets(String productId, PluginCommon.Widgets widgets)
        {
            using (LogX.InfoCall("AddWidgets"))
            {
                String requestJson = JsonHelper.ToJSON<PluginCommon.Widgets>(widgets);
                var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                HttpRequestHelper.Post<ProductInstances>(requestJson, String.Format(GetCWFDashboardServicesAPIEndPoint(CWFApiEndoints.RegisterWidgets), RegistryHelper.GetValueFromRegistry("ProductID").ToString()), header);
            }
        }

        public static ProductInstances GetProductInstances(string name, string owner, string location, string op, string managed, Boolean userSpecific)
        {
            using (LogX.InfoCall("GetProductInstances"))
            {
                var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                ProductInstances productInstances = HttpRequestHelper.Get<ProductInstances>(String.Format(GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.GetProductInstances) + "?op=" + op + "&name=" + name + "&owner=" + owner + "&location=" + location + "&managed=" + managed + "&userSpecific" + userSpecific), header);
                return productInstances;
            }
        }

        public static User GetUser(string userId)
        {
            using (LogX.InfoCall("GetUser"))
            {
                var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                User user = HttpRequestHelper.Get<User>(String.Format(GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.GetUser), userId), header);
                return user;
            }
        }

        public static User CreateUser(User user)
        {
            using (LogX.InfoCall("GetUser"))
            {
                String requestJson = JsonHelper.ToJSON<User>(user);
                var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                user = HttpRequestHelper.Post<User>(requestJson, String.Format(GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.CreateUser)), header);
                return user;
            }
        }

        public static User GetUserForName(string username)
        {
            using (LogX.InfoCall("GetUser"))
            {
                var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                User user = HttpRequestHelper.Get<User>(String.Format(GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.CreateUser)) + "?account=" + username, header);
                return user;
            }
        }

        public static Users GetUsers()
        {
            using (LogX.InfoCall("GetUsers"))
            {
                var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                Users users = HttpRequestHelper.Get<Users>(String.Format(GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.CreateUser)), header);
                return users;
            }
        }

        public static Roles GetRoles()
        {
            using (LogX.InfoCall("GetRoles"))
            {
                var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                Roles roles = HttpRequestHelper.Get<Roles>(String.Format(GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.Roles)), header);
                return roles;
            }
        }

        public static void UnregisterProductInstances(string productid, Instances instances)
        {
            using (LogX.InfoCall("UnregisterProductInstances"))
            {
                String requestJson = JsonHelper.ToJSON<Instances>(instances);
                var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                HttpRequestHelper.Post<Instances>(requestJson, String.Format(GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.UnregisterInstances), productid), header);
            }
        }

        public static void RevokePermission()
        {
            
        }

        public static ConnectionCredentials GetConnectionCredentialsOfProductInstance(string productid)
        {
            using (LogX.InfoCall("UnregisterProductInstances"))
            {
                var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                ConnectionCredentials credentials = HttpRequestHelper.Get<ConnectionCredentials>(String.Format(GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.GetConnectionCredentialsOfProductInstance), productid), header);
                return credentials;
            }
        }

        public static void CreateUpdateAlertsMetadata(AlertsMetadata alertsMetadata)
        {
            using (LogX.InfoCall("CreateUpdateAlertsMetadata"))
            {
                String requestJson = JsonHelper.ToJSON<AlertsMetadata>(alertsMetadata);
                var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                HttpRequestHelper.Post<AlertsMetadata>(requestJson, String.Format(GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.CreateUpdateAlertsMetadata)), header);
            }
        }

        public static void EditUser(EditUser editUser, string userId)
        {
            using (LogX.InfoCall("EditUser"))
            {
                String requestJson = JsonHelper.ToJSON<EditUser>(editUser);
                var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                HttpRequestHelper.Post<String>(requestJson, String.Format(GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.EditUser), userId), header);
            }
        }
        public static String GetProductNonSelfhostedAPIEndPoint(string api)
        {
            String dashboardHost = RegistryHelper.GetValueFromRegistry("DashboardHost").ToString();
            String dashboardPort = RegistryHelper.GetValueFromRegistry("DashboardPort").ToString();
            String coreServicesURL = GenericHelpers.ConcatStrings("http://", dashboardHost, ":", dashboardPort, "/");
            return GenericHelpers.ConcatStrings(coreServicesURL, api);
        }
        public static String GetCWFCoreServicesAPIEndPoint(string api)
        {
            String dashboardHost = RegistryHelper.GetValueFromRegistry("DashboardHost").ToString();
            String dashboardPort = RegistryHelper.GetValueFromRegistry("DashboardPort").ToString();
            String coreServicesURL = GenericHelpers.ConcatStrings("http://", dashboardHost, ":", dashboardPort, CWFConstants.BaseCoreServieEndpointUrl);
            return GenericHelpers.ConcatStrings(coreServicesURL, api);
        }

        public static String GetCWFDashboardServicesAPIEndPoint(string api)
        {
            String dashboardHost = RegistryHelper.GetValueFromRegistry("DashboardHost").ToString();
            String dashboardPort = RegistryHelper.GetValueFromRegistry("DashboardPort").ToString();
            String coreServicesURL = GenericHelpers.ConcatStrings("http://", dashboardHost, ":", dashboardPort, CWFConstants.BaseDashboardServicesEndpointUrl);
            return GenericHelpers.ConcatStrings(coreServicesURL, api);
        }

        public static Products GetProductInstances(string instanceName)
        {
            var userAuthHeader = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
            String restURL = GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.GetSQLDMProductsByInstanceName);
            if (!String.IsNullOrEmpty(instanceName))
            {
                restURL = restURL + "?instanceName=" + instanceName;
            }
            return HttpRequestHelper.Get<Products>(restURL, userAuthHeader);
        }
        //public static void CreateUpdateAlertsMetadata(AlertsMetadata alertsMetadata)
        //{
        //    using (LogX.InfoCall("CreateUpdateAlertsMetadata"))
        //    {
        //        String requestJson = JsonHelper.ToJSON<AlertsMetadata>(alertsMetadata);
        //        HttpRequestHelper.Post<AlertsMetadata>(requestJson, String.Format(CWFApiEndoints.CreateUpdateAlertsMetadata), Constants.AuthToken);
        //    }
        //}
		

        //SQLDM-29855
        public static Products GetProducts(string instanceName)
        {
            var userAuthHeader = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
            String restURL = GetCWFCoreServicesAPIEndPoint(CWFApiEndoints.GetProducts);
            if (!String.IsNullOrEmpty(instanceName))
            {
                restURL = restURL + "?instanceName=" + instanceName;
            }
            return HttpRequestHelper.Get<Products>(restURL, userAuthHeader);
        }

        //SQLDM-29855
        public static int GetSWAProductId(string instanceName)
        {
            int SWA_prodId = 0;
            Products products = GetProducts(instanceName);
            products.ForEach(prod =>
            {
                if (prod.Name == "SQLWorkloadAnalysis")
                {
                    SWA_prodId = prod.Id;
                }
            });
            return SWA_prodId;
        }

        

    
    }
}
