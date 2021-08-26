using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Service.DataContracts.v1;
using Idera.SQLdm.Service.Helpers;

using Idera.SQLdm.Service.Repository;
using Idera.SQLdm.Service.ServiceContracts.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common;
using Idera.SQLdm.Service.DataContracts.v1.Widgets;
using Idera.SQLdm.Service.DataContracts.v1.Database;
using Idera.SQLdm.Service.Configuration;
using System.Security.Principal;
//using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using System.ServiceModel.Web;
using System.Collections;
using Idera.SQLdm.Service.Helpers.Auth;
using Idera.SQLdm.Common.Helpers;
using Idera.SQLdm.Service.Helpers.CWF;
using PluginCommon;
using BBS.TracerX;
//using CO=Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.Service.Web
{    
    public partial class WebService : IServerManager
    {
        private static readonly Logger LogX = Logger.GetLogger("IServerManager");
        public Idera.SQLdm.Common.Objects.ApplicationSecurity.UserToken GetUserToken() 
        {
            SetConnectionCredentiaslFromCWFHost();
            return userToken;
        }

        public ServerSummaryContainerCollection GetInstances()
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetAllInstancesSummary to Implement Instance level security
            IList<ServerSummaryContainer> serverSummaryList = RepositoryHelper.GetAllInstancesSummary(RestServiceConfiguration.SQLConnectInfo, userToken);
            //adding global tags
            ICollection<Idera.SQLdm.Common.CWFDataContracts.GlobalTag> globaltags = GetGlobalTagsFromCWF(UserInfoFromCWFHost);
            RepositoryHelper.LoadServerTags(RestServiceConfiguration.SQLConnectInfo, serverSummaryList, globaltags);
            return (new ServerSummaryContainerCollection(serverSummaryList));
        }
        public ServerSummaryContainerV2 GetInstancesByInstanceName(string instanceName, string pageVal, string limitVal)
        {
            int page = pageVal == null ? 0 : ValidateAndConvertToInt(pageVal);
            int limit = limitVal == null ? 0 : ValidateAndConvertToInt(limitVal);
            List<ServerSummaryContainer> dashboardInstances = new List<ServerSummaryContainer>();
            var userAuthHeader = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
            if (String.IsNullOrWhiteSpace(instanceName) || instanceName.Equals(RegistryHelper.GetValueFromRegistry("DisplayName").ToString()))
            {
                LogX.DebugFormat("Returning Dashboard instances for current product instance for instanceName:{0}", instanceName);
                dashboardInstances.AddRange(GetInstances());
                Products products = CWFHelper.GetProductInstances(RegistryHelper.GetValueFromRegistry("DisplayName").ToString());
                if (products.Count > 0)
                {
                    Product sqldmProduct = products.First();

                    foreach (ServerSummaryContainer ssc in dashboardInstances)
                    {
                        ssc.Product = sqldmProduct;
                    }
                }
                    
            }
            else if ("All".Equals(instanceName))
            {
                LogX.DebugFormat("Received All, hence Fetching Dashboard instances of All SQLdm instances registered with CWF");
                Products products = CWFHelper.GetProductInstances(null);
                LogX.DebugFormat("Number for product instacnes found are {0}", products.Count);
                foreach (Product product in products)
                {
                    string restURL = product.RestURL;
                    if (product.InstanceName.Equals(RegistryHelper.GetValueFromRegistry("DisplayName").ToString())){
                        dashboardInstances.AddRange(GetInstances());
                    }
                    else if (product.IsSelfHosted)
                    {
                        LogX.DebugFormat("Product is SelfHosted and calling {0} to get dashboard instances", restURL + "/instances");
                        dashboardInstances.AddRange(HttpRequestHelper.Get<ServerSummaryContainerCollection>(restURL + "/instances", userAuthHeader));
                    }
                    else
                    {
                        restURL = CWFHelper.GetCWFCoreServicesAPIEndPoint("/" + restURL);
                        LogX.DebugFormat("Product is Not SelfHosted and calling {0} to get dashboard instances", restURL + "/instances");
                        dashboardInstances.AddRange(HttpRequestHelper.Get<ServerSummaryContainerCollection>(restURL + "/instances", userAuthHeader));
                    }
                    foreach (ServerSummaryContainer ssc in dashboardInstances)
                    {
                        ssc.Product = product;
                    }
                }
            }
            else {
                LogX.DebugFormat("Returning Dashboard instances for different product instance for instanceName:{0}", instanceName);
                Products products = CWFHelper.GetProductInstances(instanceName);
                LogX.DebugFormat("Products count is {0}", products.Count);
                if (products.Count > 0)
                {
                    Product p = products.First();
                    string restURL = p.RestURL;
                    if (p.IsSelfHosted)
                    {
                        LogX.DebugFormat("Product is SelfHosted and calling {0} to get dashboard instances", restURL + "/instances");
                        dashboardInstances.AddRange(HttpRequestHelper.Get<ServerSummaryContainerCollection>(restURL + "/instances", userAuthHeader));
                    }
                    else
                    {
                        restURL = CWFHelper.GetProductNonSelfhostedAPIEndPoint("/" + restURL);
                        LogX.DebugFormat("Product is Not SelfHosted and calling {0} to get dashboard instances", restURL + "/instances");
                        dashboardInstances.AddRange(HttpRequestHelper.Get<ServerSummaryContainerCollection>(restURL + "/instances", userAuthHeader));
                    }
                    foreach (ServerSummaryContainer ssc in dashboardInstances)
                    {
                        ssc.Product = p;
                    }
                }
            }
            //START
            //SQLDM-29855. Update SWA Icon in Dashboard
            //foreach (ServerSummaryContainer ssc in dashboardInstances)
            //{
            //    string SWAurl = null;
            //    int SWAid = CWFHelper.GetSWAProductId(ssc.Overview.InstanceName);
            //    if (SWAid > 0)
            //    {
            //        ssc.Overview.isSWAInstance = true;
            //        ssc.Overview.SWAUrl = SWAurl;
            //    }
            //    else
            //    {
            //        ssc.Overview.isSWAInstance = false;
            //        ssc.Overview.SWAUrl = SWAurl;
            //    }
            //}
            //END
            ServerSummaryContainerV2 serverSummaryContainerV2 = new ServerSummaryContainerV2();
            if (null != dashboardInstances)
            {
                serverSummaryContainerV2.totalResults = dashboardInstances.Count;
                if (limit == 0)
                {
                    serverSummaryContainerV2.ServerSummaryContainerList = dashboardInstances.ToList();
                }
                else
                {
                    serverSummaryContainerV2.ServerSummaryContainerList = dashboardInstances.Skip((page - 1) * limit).Take(limit).ToList();
                }
            }
            return serverSummaryContainerV2;
        }

        /**
         *  Converts the passed String value to it's equivalent Integer value.
         *  If the convertion fails, a bad request execption will be thrown.
         */
        private int ValidateAndConvertToInt(String value)
        {
            int intValue;
            if (!int.TryParse(value, out intValue))
            {
                String errorMessage = String.Format("Invalid integer parameter received: {0}", value);
                LogX.Error(errorMessage);
            }
            return intValue;
        }


        /*public MonitoredSqlServerStatusCollection GetInstancesWithSeverity()
        {
            IList<MonitoredSqlServerStatus> MonitoredSqlServerStatusList = RepositoryHelper.GetStatus(null);
            return (new MonitoredSqlServerStatusCollection(MonitoredSqlServerStatusList));
        }*/


        public MonitoredSqlServerCollection GetShortInstances(string ActiveOnlyFlag, string FilterField, string FilterValue)
        {
            SetConnectionCredentiaslFromCWFHost();
            //changed by Gaurav Karwal to take the ActiveFlag as string. with bollean, WCF deserializer was throwing an error before reaching this code if ActiveOnly flag was blank
            bool ActiveInstancesOnly = false;
            if (!String.IsNullOrWhiteSpace(ActiveOnlyFlag) && !Boolean.TryParse(ActiveOnlyFlag, out ActiveInstancesOnly)) ActiveInstancesOnly = false;

            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetMonitoredSqlServers to Implement Instance level security
            var MonitoredSqlServerList = RepositoryHelper.GetMonitoredSqlServers(RestServiceConfiguration.SQLConnectInfo, ActiveInstancesOnly, userToken, FilterField, FilterValue);
            return (new MonitoredSqlServerCollection(ConvertToDataContract.ToDC(MonitoredSqlServerList)));
           
        }

        public MonitoredSqlServer GetShortInstanceDetails(string InstanceId)
        {
            SetConnectionCredentiaslFromCWFHost();

            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (InstanceId != null && !ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(InstanceId),userToken)) 
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            //End
            

            int passedinstanceId;
            if (int.TryParse(InstanceId, out passedinstanceId))
            {
                var monitorSqlServer = RepositoryHelper.GetMonitoredSqlServer(RestServiceConfiguration.SQLConnectInfo, passedinstanceId);
                return ConvertToDataContract.ToDC(monitorSqlServer);
            }
            else
            {
                throw new FaultException("InstanceId must be int");
            }
        }

        public ServerSummaryContainer GetInstanceDetails(string instanceId, string timeZoneOffset)
        {
            SetConnectionCredentiaslFromCWFHost();

            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (instanceId != null && !ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(instanceId), userToken)) 
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
                
            }
            //END
            int passedinstanceId;
            ServerSummaryContainer serverSummaryContainer = null;
            if (int.TryParse(instanceId, out passedinstanceId))
            {
                IList<ServerSummaryContainer> serverSummaryList =  RepositoryHelper.GetAllInstancesSummary(RestServiceConfiguration.SQLConnectInfo, userToken);

                foreach (ServerSummaryContainer serverSummary in serverSummaryList)
                {
                    //SQLdm10.2 (Srishti Purohit) defect SQLDM-28000 fix
                    //For nonreachable instance serverStatus will be null
                    //TODO : this logic has to be improved as to get health of one instance we need to get data of all instances
                    if (serverSummary != null && serverSummary.Overview != null && serverSummary.Overview.SQLServerId == passedinstanceId)
                    {
                        serverSummaryContainer = serverSummary;
                        break;
                    }
                }
                RepositoryHelper.LoadServerStatusFor(RestServiceConfiguration.SQLConnectInfo, serverSummaryContainer, null, null);
                return serverSummaryContainer;
            }
            else
            {
                throw new FaultException("InstanceId must be int");
            }
        }

        public MonitoredSqlServer GetServerStatisticsHistory(string InstanceId, string timeZoneOffset, int NumHistoryMinutes, DateTime endDate)
        {
            SetConnectionCredentiaslFromCWFHost();

            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (InstanceId != null && !ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(InstanceId), userToken)) 
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            //END
            int passedinstanceId;
            if (int.TryParse(InstanceId, out passedinstanceId))
            {
                return RepositoryHelper.GetServerStatisticsHistory(RestServiceConfiguration.SQLConnectInfo, passedinstanceId, timeZoneOffset, NumHistoryMinutes, endDate);
            }
            else
            {
                throw new FaultException("InstanceId must be int");
            }
        }
      
		public TagsCollection GetTags ()
        {
            SetConnectionCredentiaslFromCWFHost();
            ICollection<Idera.SQLdm.Common.Objects.Tag> tagsFromDB = RepositoryHelper.GetTags(RestServiceConfiguration.SQLConnectInfo);
            return (new TagsCollection(ConvertToDataContract.ToDC(tagsFromDB)));
        }
      
        public IList<MonitoredSqlServerDatabase> GetDatabasesByInstance(string InstanceId, string timeZoneOffset) 
        {
            SetConnectionCredentiaslFromCWFHost();

            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (InstanceId != null && !ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(InstanceId), userToken)) 
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            //END
            int instID;
            IList < MonitoredSqlServerDatabase> results = null;
            if (int.TryParse(InstanceId, out instID))
                results = RepositoryHelper.GetServerDatabasesOverview(RestServiceConfiguration.SQLConnectInfo, instID, timeZoneOffset);
            else
                results = new List<MonitoredSqlServerDatabase>();

            return results ;
        }

        public IList<string> AreYouUp(string reflectingData)
        {
            SetConnectionCredentiaslFromCWFHost();
            string responseOne = reflectingData;
            string responseTwo = string.Concat(reflectingData.ToCharArray().Reverse().ToArray());
            List<string> responseList = new List<string>();
            responseList.Add(responseOne);
            responseList.Add(responseTwo);
            return responseList;
        }
        /*public IList<ResponseTimeForInstance> GetLatestResponseTimesByInstance()
        {
            return RepositoryHelper.GetLatestResponseTimesByInstance(RestServiceConfiguration.SQLConnectInfo);
        }*/

        //SQL10.1 Srishti Purohit
        //Health index Scale factors
        
        public HealthIndexScaleFactors GetScaleFactors()
        {
            SetConnectionCredentiaslFromCWFHost();
            HealthIndexScaleFactors healthCoefficientInfo = RepositoryHelper.GetScaleFactors(RestServiceConfiguration.SQLConnectInfo);
            return healthCoefficientInfo;
        }
        //SQL10.1 Srishti Purohit
        //Health index Scale factors

        public void UpdateScaleFactors(HealthIndexCoefficient healthCoefficient, List<InstanceScaleFoctor> ins, List<TagScaleFactor> tagScaleFactors)
        {

            SetConnectionCredentiaslFromCWFHost();
            // (start)SQLdm 10.1 (Pulkit Puri)-- adding validation to update scale factors 
            // SQLDM-25542: proper error should be thrown for the null values of scale factors
            if (healthCoefficient == null)//SQLdm 10.1 (Pulkit Puri) fix for SQLDM- 25912
            {
                throw new FaultException<WebFaultException>(new WebFaultException(System.Net.HttpStatusCode.BadRequest), "{Wrong value for scale factor" + ":" + "cannot be null value" + "}");
            }
            // (end)SQLdm 10.1 (Pulkit Puri)-- adding validation to update scale factors 

            // (start)SQLdm 10.1 (Pulkit Puri)-- adding validation to update scale factors 
            // Value of health scale factor should be between 0 and 10 (Instance scale factor)
            if (ins != null)
            {
                foreach (InstanceScaleFoctor instance in ins)
                {
                    if (!(BuisnessLogicHelper.IsHealthscaleFactorValid(instance.InstanceHealthScaleFactor)))// InstanceHealthScaleFactor can be zero also
                    {
                        throw new FaultException<WebFaultException>(new WebFaultException(System.Net.HttpStatusCode.BadRequest), "{Wrong value for Instance scale factor" + ":" + "should be between 0 to 10" + "}");
                    }
                }
            }

            // Value of health scale factor should be between 0 and 10 (tag scale factor)
            if (tagScaleFactors != null)
            {
                foreach (TagScaleFactor tag in tagScaleFactors)
                {
                    if (!(BuisnessLogicHelper.IsHealthscaleFactorValid(tag.TagHealthScaleFactor)))
                    {
                        throw new FaultException<WebFaultException>(new WebFaultException(System.Net.HttpStatusCode.BadRequest), "{Wrong value for Tag scale factor" + ":" + "should be between 0 to 10" + "}");
                    }
                }
            }

            // (end) SQLdm 10.1 (Pulkit Puri)


            // (start) SQLdm 10.1 (Pulkit Puri)--add validation to health coefficient
            if(!(BuisnessLogicHelper.IsHealthscaleFactorValid(healthCoefficient.HealthIndexCoefficientForCriticalAlert)) ||
               !(BuisnessLogicHelper.IsHealthscaleFactorValid(healthCoefficient.HealthIndexCoefficientForInformationalAlert)) ||
               !(BuisnessLogicHelper.IsHealthscaleFactorValid(healthCoefficient.HealthIndexCoefficientForWarningAlert)))
                {
                throw new FaultException<WebFaultException>(new WebFaultException(System.Net.HttpStatusCode.BadRequest), "{Wrong value for health coefficient" + ":" + "should be between 0 to 10" + "}");
                }
            // (end) SQLdm 10.1 (Pulkit Puri)

            RepositoryHelper.UpdateScaleFactors(RestServiceConfiguration.SQLConnectInfo, healthCoefficient, ins, tagScaleFactors);            
        }


        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///Get Consolidated Instance Overview
        ///</summary>
        public ConsolidatedInstanceOverview GetConsolidatedInstanceOverview(String InstanceID)
        {
       
            SetConnectionCredentiaslFromCWFHost();
            ConsolidatedInstanceOverview consolidatedInstanceOverView = RepositoryHelper.GetConsolidatedInstanceOverview(RestServiceConfiguration.SQLConnectInfo,InstanceID);
            return consolidatedInstanceOverView;
        }
    }
}
