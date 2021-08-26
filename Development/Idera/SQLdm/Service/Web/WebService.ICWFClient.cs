using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using Idera.SQLdm.Service.ServiceContracts.v1;
using Idera.SQLdm.Service.Repository;
using System.Security.Principal;
using Idera.SQLdm.Service.DataContracts.v1;
using System.ServiceModel;
using Idera.SQLdm.Service.Configuration;
using System.ServiceModel.Web;
using CWFContracts = Idera.SQLdm.Common.CWFDataContracts;
using Idera.SQLdm.Service.Helpers.CWF;
using PluginCommon;
using Idera.SQLdm.Service.DataModels;
using Idera.SQLdm.Common.Helpers;

namespace Idera.SQLdm.Service.Web
{
    /// <summary>
    /// SQLdm 9.0 (Gaurav Karwal): implementing the caller to the cwf functions for synchronizing instances
    /// </summary>
    public partial class WebService : ICWFManager
    {
        /// <summary>
        /// synchronizes alerts between product and CWF
        /// </summary>
        /// <param name="freshAlerts"></param>
        public void SyncAlertsWithCWF(List<CWFContracts.Alert> freshAlerts)
        {
            /* CWF Team - Backend Isolation - SQLDM-29086
             * Changing type from IPrincipal to Principal
             * Removing If condicion */
            Principal userInfo = GetPrincipalFromCWFHost(); //getting the principal from cwf
            List<PluginCommon.AlertMetadata> listAlertMetadata = new List<AlertMetadata>();
            string productIdAsString = GetProductIdFromRequest();
            //if (_myHost != null)
            //{
            Alerts listCWFAlerts = ObjectTranslator.TranslateToCWFAlertList(freshAlerts);
            /* CWF Team - Backend Isolation - SQLDM-29086
             * Calling CWF Rest API instead of CWF Host Object */
            //_myHost.SynchronizeAlerts(productIdAsString, listCWFAlerts, userInfo);
            var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");

            // SQLDM - 29552: Sync Alerts with CWF
            // Get Alerts from the CWF Alerts for the product
            var cwfAlerts = CWFHelper.GetCwfAlerts(productIdAsString, header);

            // Delete CWF Alerts present for the product
            CWFHelper.DeleteCwfAlerts(cwfAlerts, header);

            // Add Alerts
            CWFHelper.SynchronizeAlerts(listCWFAlerts, header);
            
            //START SQLDM 10.1 (Barkha Khatri ) SQLDM-25910 fix --populating alertsMetaData table 
            listCWFAlerts.ForEach(alert =>
            {
                if (listAlertMetadata.FirstOrDefault(a => a.Metric == alert.Metric) == null)
                {
                    AlertMetadata metadata = new AlertMetadata();
                    int prodIdAsInt = 0;
                    metadata.Metric = alert.Metric;
                    if (int.TryParse(productIdAsString, out prodIdAsInt))
                    {
                        metadata.ProductId = prodIdAsInt;
                        metadata.LinkToDashboard = string.Empty;
                        metadata.KB = string.Empty;
                        metadata.LinkToDetails = string.Empty;
                        metadata.LinkToWiki = string.Empty;
                        metadata.Message = string.Empty;
                        metadata.PluralMessage = string.Empty;
                        metadata.PluralPreview = string.Empty;
                        metadata.Preview = string.Empty;
                        listAlertMetadata.Add(metadata);

                    }

                }
            });
            PluginCommon.AlertsMetadata alertsMetadata = new AlertsMetadata(listAlertMetadata);
            /* CWF Team - Backend Isolation - SQLDM-29086
             * Calling CWF Api instead of CWF Host Object */
            //_myHost.CreateUpdateAlertsMetadata(alertsMetadata, userInfo);
            CWFHelper.CreateUpdateAlertsMetadata(alertsMetadata);
            //END SQLDM 10.1 (Barkha Khatri ) SQLDM-25910 fix --populating alertsMetaData table 
            //}
            //else throw new WebFaultException(System.Net.HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// synchronizes instances between product and CWF
        /// </summary>
        /// <param name="freshInstances"></param>
        public void SyncInstanceWithCWF(List<CWFContracts.Instance> freshInstances)
        {
            /* CWF Team - Backend Isolation - SQLDM-29086
             * Changinf type from IPrincipal to Principal */
            //IPrincipal userInfo = GetPrincipalFromCWFHost(); //getting the principal from cwf
            Principal userInfo = GetPrincipalFromCWFHost();
            string productId = GetProductIdFromRequest();

            //SQLdm 10.1 (Pulkit Puri)--For adding proper validation of InstanceStatus 
            //SQLDM-25789 --Proper error mesaage for invalid instance status 
            int invalidInstances = 0;
            string errorMessage = "Following Instances have Invalid Instance status value: " + "--";
            foreach (CWFContracts.Instance ins in freshInstances)
            {
                if (!(Enum.IsDefined(typeof(Idera.SQLdm.Common.CWFDataContracts.InstanceStatus), ins.InstanceStatus)))
                {
                    invalidInstances++;
                    errorMessage += invalidInstances + @" - " + ins.Name + "--";
                }
            }

            if (invalidInstances > 0)
                throw new FaultException<WebFaultException>
                    (new WebFaultException(System.Net.HttpStatusCode.BadRequest), errorMessage);

            //if (_myHost != null)
            //{
            CreateInstances sQLdmInstances = ObjectTranslator.TranslateToCWFInstanceList(freshInstances);

            /* CWF Team - Backend Isolation - SQLDM-29086
             * Calling CWF Api instead of CWF Host Object */
            //_myHost.SynchronizeInstances(productId, sQLdmInstances, userInfo);
            var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
            CWFHelper.SynchronizeInstances(sQLdmInstances,header);

            //[START] SQLdm 10.0 (Gaurav Karwal) : getting all instances to re-insert them. not sure why CWF did it this way!!
            /* CWF Team - Backend Isolation - SQLDM-29086
             * Calling CWF Api instead of CWF Host Object */
            //ProductInstances pinstances = _myHost.GetProductInstances(null, null, null, "or", "false", userInfo);
            ProductInstances pinstances = CWFHelper.GetProductInstances(String.Empty, String.Empty, String.Empty, "or", String.Empty,false);
            RegisterProductInstances rpi = new RegisterProductInstances();
            //SQLDM 10.1 (Barkha Khatri)
            //adding logic for unregistering instances
            Instances deletedInstances = new Instances();
            foreach (var item in pinstances)
            {
                if (item.Products.FindIndex(product => product.Id == Int32.Parse(productId)) >= 0)
                {
                    var instance = freshInstances.FirstOrDefault(match => (string.Compare(match.Name, item.Name, StringComparison.OrdinalIgnoreCase) == 0));

                    if (instance != null)
                    {
                        RegisterProductInstance rpiItem = new RegisterProductInstance(item.Id);
                        rpiItem.Status = (PluginCommon.InstanceStatus)instance.InstanceStatus;
                        rpi.Add(rpiItem);
                    }
                    else
                    {
                        Instance inst = new Instance();
                        inst.Comments = item.Comments;
                        inst.Edition = item.Edition;
                        inst.Location = item.Location;
                        inst.Name = item.Name;
                        inst.Owner = item.Owner;
                        inst.UtcFirstSeen = item.UtcFirstSeen;
                        inst.UtcLastSeen = item.UtcLastSeen;
                        inst.Version = item.Version;
                        inst.Id = item.Id;
                        deletedInstances.Add(inst);

                    }
                }
            }

            if (rpi.Count > 0)
            {
                /* CWF Team - Backend Isolation - SQLDM-29086
                 * Calling CWF API instead of CWF Host Object */
                //_myHost.RegisterProductInstances(productId, rpi, userInfo);
                CWFHelper.RegisterInstances(rpi,header);
            }
            if (deletedInstances.Count > 0)
            {
                /* CWF Team - Backend Isolation - SQLDM-29086
                 * Calling CWF API instead of CWF Host Object */
                //_myHost.UnregisterProductInstances(productId, deletedInstances, userInfo);
                CWFHelper.UnregisterProductInstances(productId,deletedInstances);
            }
            //[END] SQLdm 10.0 (Gaurav Karwal) : getting all instances to re-insert them. not sure why CWF did it this way!!

            //}
            //else throw new WebFaultException(System.Net.HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// synchronizes users between product and CWF
        /// </summary>
        /// <param name="freshInstances"></param>
        public void SyncUsersWithCWF(List<CWFContracts.User> freshUsers)
        {
            /* CWF Team - Backend Isolation - SQLDM-29086
             * Changing type from IPrincipal to Principal */
            //IPrincipal userInfo = GetPrincipalFromCWFHost(); //getting the principal from cwf
            Principal userInfo = GetPrincipalFromCWFHost();
            string productId = GetProductIdFromRequest();
            int intProdId;
            if (!Int32.TryParse(productId, out intProdId))
            {
                _eventLog.WriteEntry("Could not parse Product ID : " + productId);
            }
            //if (_myHost != null)
            //{
            // START: SQLdm 9.0 (Abhishek Joshi) -fixed defect DE44344
            /* CWF Team - Backend Isolation - SQLDM-29086
             * Calling CWF API instead of CWF Host Object */
            //Roles availableRoles = _myHost.GetRoles(userInfo);
            Roles availableRoles = CWFHelper.GetRoles();
            int productUserRoleId = 0, productAdminRoleId = 0;
            foreach (Role role in availableRoles)
            {
                if (role.RoleName.Equals("ProductUser", StringComparison.OrdinalIgnoreCase))
                    productUserRoleId = role.RoleID;
                if (role.RoleName.Equals("ProductAdministrator", StringComparison.OrdinalIgnoreCase))
                    productAdminRoleId = role.RoleID;
            }


            // END: SQLdm 9.0 (Abhishek Joshi) -fixed defect DE44344
            //SQLDM 10.1 (Barkha Khatri)
            //SQLDM-26607 fix 
            //deleting users which are deleted from DM or SQL server 
            //User is not deleted from User Table , only Permission table values are deleted corresponding to deleted users for this particular product id
            /* CWF Team - Backned Isolation - SQLDM-29086
             * Calling CWF API instead of CWF Host Object */
            //Users existingUsers = _myHost.GetUsers(userInfo);
            Users existingUsers = CWFHelper.GetUsers();
            foreach (var existingUserCheck in existingUsers)
            {
                if (existingUserCheck.Permissions.FindIndex(permission => permission.ProductID == intProdId) >= 0)
                {
                    var userObj = freshUsers.FirstOrDefault(match => (string.Compare(match.Account, existingUserCheck.Account, StringComparison.OrdinalIgnoreCase) == 0));
                    if (userObj == null)
                    {
                        foreach (var permission in existingUserCheck.Permissions)
                        {
                            if (permission.ProductID == intProdId)
                            {
                                /* CWF Team - Backned Isolation - SQLDM-29086
                                 * Calling CWF API instead of CWF Host Object */
                                //_myHost.RevokePermission(productId, permission.Id.ToString(), userInfo);
                                CWFHelper.RevokePermission();
                                _eventLog.WriteEntry("Product ID : " + intProdId.ToString() + " removed while syncing.");
                            }
                        }

                    }
                }
            }
            ObjectTranslator.TranslateToCWFUserList1(freshUsers).ForEach(u =>
            {
                try
                {
                    /* CWF Team - Backend Isolation - SQLDM-29086
                     * Calling CWF API instead of CWF Host Object */
                    //var existingUser = _myHost.GetUserForName(u.Account, userInfo);
                    //var user = existingUser != null ? existingUser : _myHost.CreateUser(u, userInfo);
                    var existingUser = CWFHelper.GetUserForName(u.Account);
                    var user = existingUser != null ? existingUser : CWFHelper.CreateUser(u);
                    // START: SQLdm 9.0 (Abhishek Joshi) -fixed defect DE44344

                    if (productUserRoleId > 0 && productAdminRoleId > 0)
                    {
                        if (intProdId > 0)
                        {
                            EditPermissions permissionsList = new EditPermissions();
                            EditPermission permission = new EditPermission()
                            {
                                RoleID = freshUsers.FirstOrDefault(match => match.SID == u.SID).IsAdmin == true ? productAdminRoleId : productUserRoleId, //SQLdm 10.1 (GK): sending admin role for admin user else product user role
                                ProductID = intProdId
                            };

                            permissionsList.Add(permission);

                            foreach (var existingPermission in user.Permissions)
                            {
                                if (existingPermission.ProductID != intProdId)
                                {
                                    permission = new EditPermission() { RoleID = existingPermission.RoleID, ProductID = existingPermission.ProductID };
                                    permissionsList.Add(permission);
                                }
                            }


                            EditUser updatedInfo = new EditUser();
                            updatedInfo.Account = user.Account;
                            updatedInfo.IsEnabled = user.IsEnabled;
                            updatedInfo.Roles = permissionsList;

                            /* CWF Team - Backend Isolation - SQLDM-29086
                             * Calling CWF API instead of CWF Host Object */
                            //_myHost.EditUser(updatedInfo, user.Id.ToString(), userInfo);
                            //PluginCommon.User savedUser = _myHost.GetUser(user.Id.ToString(), userInfo);
                            //ProductInstances pinstances = _myHost.GetProductInstances(null, null, null, "or", "false", userInfo);
                            CWFHelper.EditUser(updatedInfo,user.Id.ToString());
                            PluginCommon.User savedUser = CWFHelper.GetUser(user.Id.ToString());
                            ProductInstances pinstances = CWFHelper.GetProductInstances(String.Empty, String.Empty, String.Empty, "or", String.Empty, false);
                            //Commenting unwanted loop creation. searching for respective instance in below loop itself
                            //List<Instance> mappedCWFInstances = new List<Instance>();
                            //foreach (var item in pinstances)
                            //{
                            //    if (freshUsers.FirstOrDefault(match => match.SID == u.SID).LinkedInstances.Contains(item.Name, StringComparer.CurrentCultureIgnoreCase))
                            //    {
                            //        Instance inst = new Instance();
                            //        inst.Comments = item.Comments;
                            //        inst.Edition = item.Edition;
                            //        inst.Location = item.Location;
                            //        inst.Name = item.Name;
                            //        inst.Owner = item.Owner;
                            //        inst.UtcFirstSeen = item.UtcFirstSeen;
                            //        inst.UtcLastSeen = item.UtcLastSeen;
                            //        inst.Version = item.Version;
                            //        inst.Id = item.Id;
                            //        mappedCWFInstances.Add(inst);
                            //    }
                            //}
                            if (savedUser != null && pinstances != null && pinstances.Count > 0)
                            {
                                foreach (string instanceName in freshUsers.FirstOrDefault(match => match.SID == u.SID).LinkedInstances)
                                {
                                    var item = pinstances.FirstOrDefault(match => (string.Compare(match.Name, instanceName, StringComparison.OrdinalIgnoreCase) == 0));
                                    if (item != null)
                                    {
                                        Instance inst = new Instance();
                                        inst.Comments = item.Comments;
                                        inst.Edition = item.Edition;
                                        inst.Location = item.Location;
                                        inst.Name = item.Name;
                                        inst.Owner = item.Owner;
                                        inst.UtcFirstSeen = item.UtcFirstSeen;
                                        inst.UtcLastSeen = item.UtcLastSeen;
                                        inst.Version = item.Version;
                                        inst.Id = item.Id;
                                        foreach (UserPermission userPermission in savedUser.Permissions)
                                        {
                                            _eventLog.WriteEntry("Permission Id :" + userPermission.Id.ToString() + " and instance : " + instanceName);
                                            /* CWF Team - Backend Isolation - SQLDM-29086
                                             * Calling CWF Api instead of Host Object */
                                            //_myHost.AssignInstancePermissions(intProdId.ToString(), userPermission.Id.ToString(), inst, userInfo);
                                            CWFHelper.AssignInstancePermissions(intProdId.ToString(), userPermission.Id.ToString(), inst);
                                        }
                                    }
                                }
                            }
                            _eventLog.WriteEntry("Product ID : " + intProdId.ToString() + "and user id : " + user.Id.ToString() + " added while syncing.");
                        }
                    }

                    // END: SQLdm 9.0 (Abhishek Joshi) -fixed defect DE44344
                }
                catch (WebFaultException ex_webfault)
                {
                    _logX.Warn("Creation of the user " + u.Account + " failed. error details: " + ex_webfault.Message + " http status code: " + ex_webfault.StatusCode + "::" + (ex_webfault.InnerException != null ? ex_webfault.InnerException.Message : string.Empty));
                }
                catch (Exception ex)
                {
                    _logX.Error("Creation of the user " + u.Account + " failed. error details: " + ex.Message + "::" + (ex.InnerException != null ? ex.InnerException.Message : string.Empty));
                }
            });

            //}
            //else throw new WebFaultException(System.Net.HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// SQLdm 9.0 (Abhishek Joshi): get a list of all instances from CWF
        /// </summary>
        /// <returns></returns>
        public List<CWFContracts.Instance> GetInstancesFromCWF()
        {
            /* CWF Team - Backend Isolation - SQLDM-29086
             * Chaning Type from IPrincipal to Principal
             * Calling CWF Api instead of Host Object */
            //IPrincipal userInfo = GetPrincipalFromCWFHost();
            Principal userInfo = GetPrincipalFromCWFHost();
            return ObjectTranslator.TranslateToCWFContract(CWFHelper.GetProductInstances(String.Empty, String.Empty, String.Empty, String.Empty, String.Empty,false));
            /*if (_myHost != null)
                return CWFHelper.ObjectTranslator.TranslateToCWFContract(_myHost.GetProductInstances(String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, userInfo));
            else
                throw new WebFaultException(System.Net.HttpStatusCode.InternalServerError); */
        }

        /// <summary>
        /// SQLdm 9.1 (Gaurav Karwal): adds widgets to dashboard
        /// </summary>
        /// <param name="dashboardWidgets"></param>
        public void AddDashboardWidgets()
        {
            /* CWF Team - Backend Isolation - SQLDM-29086
             * Chaning Type from IPrincipal to Principal
             * Calling CWF Api instead of Host Object */
            //IPrincipal userInfo = GetPrincipalFromCWFHost(); //getting the principal from cwf
            Principal userInfo = GetPrincipalFromCWFHost();
            string productId = GetProductIdFromRequest();
            CWFHelper.AddWidgets(productId, ObjectTranslator.TranslateToCWFDashboardWidgetList(GetDashboardWidgetsList()));
            /* if (_myHost != null) _myHost.AddWidgets(productId, CWFHelper.ObjectTranslator.TranslateToCWFDashboardWidgetList(GetDashboardWidgetsList()), userInfo);
            else throw new WebFaultException(System.Net.HttpStatusCode.InternalServerError); */
        }

        /// <summary>
        /// Gets the list of all the widgets to be registered on Idera Dashboard
        /// </summary>
        /// <returns></returns>
        private List<CWFContracts.DashboardWidget> GetDashboardWidgetsList()
        {
            List<CWFContracts.DashboardWidget> allDashboardWidgets = new List<CWFContracts.DashboardWidget>();

            string SQLDM_VERSION_FOR_WIDGETS = RegistryHelper.GetValueFromRegistry("Version").ToString();

            //SQLdm 9.1 (Gaurav Karwal): Overall status widget - shows up on the Overview tab on Idera dashboard
            CWFContracts.DashboardWidget overallStatusWidget = new CWFContracts.DashboardWidget();
            overallStatusWidget.Name = "Overall Status | SQLDM";
            overallStatusWidget.Type = "Product Status";
            overallStatusWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            overallStatusWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/overallStatusWidget.zul";
            overallStatusWidget.DataURI = "/ProductStatus";
            overallStatusWidget.Description = "Provides an Overview of the total alerts";
            overallStatusWidget.Version = SQLDM_VERSION_FOR_WIDGETS;

            Dictionary<string, string> overallStatusWidgetSettings = new Dictionary<string, string>();
            overallStatusWidgetSettings.Add("Limit", "10");//for limiting the records to 10 by default
            overallStatusWidget.Settings = overallStatusWidgetSettings;

            overallStatusWidget.DefaultViews = "Overview";
            overallStatusWidget.Collapsed = true;

            //SQLdm 9.1 (Gaurav Karwal): Instance status widget - shows up on the Overview tab on Idera dashboard
            CWFContracts.DashboardWidget instanceStatusWidget = new CWFContracts.DashboardWidget();
            instanceStatusWidget.Name = "Instance Status | SQLDM";
            instanceStatusWidget.Type = "Instance Status";
            instanceStatusWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            instanceStatusWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instanceStatusWidget.zul";
            instanceStatusWidget.DataURI = "/InstanceStatus";
            instanceStatusWidget.Description = "Provides an overview of the instance at each level";
            instanceStatusWidget.Version = SQLDM_VERSION_FOR_WIDGETS;
            Dictionary<string, string> instanceStatusWidgetSettings = new Dictionary<string, string>();
            instanceStatusWidgetSettings.Add("Limit", "10");//for limiting the records to 10 by default
            instanceStatusWidget.Settings = instanceStatusWidgetSettings;
            instanceStatusWidget.DefaultViews = "Overview";
            instanceStatusWidget.Collapsed = true;

            //SQLdm 9.1 (Gaurav Karwal): Top Instances by Alert Count widget - shows up on the Details tab on Idera dashboard
            CWFContracts.DashboardWidget topInstancesbyAlertCountWidget = new CWFContracts.DashboardWidget();
            topInstancesbyAlertCountWidget.Name = "SQLDM – Top Instances by Alert Count";
            topInstancesbyAlertCountWidget.Type = "Top X";
            topInstancesbyAlertCountWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            topInstancesbyAlertCountWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instancesByAlertsCountWidget.zul";
            topInstancesbyAlertCountWidget.DataURI = "/Instances/ByAlerts";
            topInstancesbyAlertCountWidget.Description = "List top instances by alert count.";
            topInstancesbyAlertCountWidget.Version = SQLDM_VERSION_FOR_WIDGETS;
            Dictionary<string, string> settingTopInstanceWidgets = new Dictionary<string, string>();
            settingTopInstanceWidgets.Add("Limit", "10");//for limiting the records to 10 by default
            topInstancesbyAlertCountWidget.Settings = settingTopInstanceWidgets;
            topInstancesbyAlertCountWidget.DefaultViews = "Details";
            topInstancesbyAlertCountWidget.Collapsed = true;

            //SQLdm 9.1 (Gaurav Karwal): Top Databases by Alert Count widget - shows up on the Details tab on Idera dashboard
            CWFContracts.DashboardWidget topDatabasesByAlertCountWidget = new CWFContracts.DashboardWidget();
            topDatabasesByAlertCountWidget.Name = "SQLDM – Top Databases by Alert Counts";
            topDatabasesByAlertCountWidget.Type = "Top X";
            topDatabasesByAlertCountWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            topDatabasesByAlertCountWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/databasesByAlertsCountWidget.zul";
            topDatabasesByAlertCountWidget.DataURI = "/Instances/Databases/ByAlerts";
            topDatabasesByAlertCountWidget.Description = "List top databases by alert counts.";
            topDatabasesByAlertCountWidget.Version = SQLDM_VERSION_FOR_WIDGETS;
            Dictionary<string, string> settingTopDatabasesByAlertWidgets = new Dictionary<string, string>();
            settingTopDatabasesByAlertWidgets.Add("Limit", "10");//for limiting the records to 10 by default
            topDatabasesByAlertCountWidget.Settings = settingTopDatabasesByAlertWidgets;

            topDatabasesByAlertCountWidget.DefaultViews = "Details";
            topDatabasesByAlertCountWidget.Collapsed = true;

            //SQLdm 9.1 (Gaurav Karwal): Top Instances by CPU Usage widget - shows up on the Details tab on Idera dashboard
            CWFContracts.DashboardWidget topInstanceByCPUUsageWidget = new CWFContracts.DashboardWidget();
            topInstanceByCPUUsageWidget.Name = "SQLDM – Top Instances by CPU Usage";
            topInstanceByCPUUsageWidget.Type = "Top X";
            topInstanceByCPUUsageWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            topInstanceByCPUUsageWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instancesByCpuUsageWidget.zul";
            topInstanceByCPUUsageWidget.DataURI = "/Instances/BySqlCpuLoad";
            topInstanceByCPUUsageWidget.Description = "List top instances by CPU usage.";
            topInstanceByCPUUsageWidget.Version = SQLDM_VERSION_FOR_WIDGETS;

            Dictionary<string, string> settingTopInstanceByCPUUsageWidget = new Dictionary<string, string>();
            settingTopInstanceByCPUUsageWidget.Add("Limit", "10");//for limiting the records to 10 by default
            topInstanceByCPUUsageWidget.Settings = settingTopInstanceByCPUUsageWidget;

            topInstanceByCPUUsageWidget.DefaultViews = "Details";
            topInstanceByCPUUsageWidget.Collapsed = true;

            //SQLdm 9.1 (Gaurav Karwal): Alert Count By Category widget - shows up on the Details tab on Idera dashboard
            CWFContracts.DashboardWidget alertCountsByCategoryWidget = new CWFContracts.DashboardWidget();
            alertCountsByCategoryWidget.Name = "SQLDM – Alert Counts by Category";
            alertCountsByCategoryWidget.Type = "Top X";
            alertCountsByCategoryWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            alertCountsByCategoryWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/alertCountsByCategory.zul";
            alertCountsByCategoryWidget.DataURI = "/numAlertsByCategory";
            alertCountsByCategoryWidget.Description = "Shows alert counts by alert category.";
            alertCountsByCategoryWidget.Version = SQLDM_VERSION_FOR_WIDGETS;

            Dictionary<string, string> settingAlertCountsByCategoryWidget = new Dictionary<string, string>();
            settingAlertCountsByCategoryWidget.Add("Limit", "10");//for limiting the records to 10 by default
            alertCountsByCategoryWidget.Settings = settingAlertCountsByCategoryWidget;

            alertCountsByCategoryWidget.DefaultViews = "Details";
            alertCountsByCategoryWidget.Collapsed = true;

            //SQLdm 9.1 (Gaurav Karwal): List of Active Alerts widget - shows up on the Overview tab on Idera dashboard
            CWFContracts.DashboardWidget activeAlertListWidget = new CWFContracts.DashboardWidget();
            activeAlertListWidget.Name = "SQLDM – Active Alerts List";
            activeAlertListWidget.Type = "Top X";
            activeAlertListWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            activeAlertListWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/alertsListWidget.zul";
            activeAlertListWidget.DataURI = "/AlertsForWebConsole/tzo/{timeZoneOffset}";
            activeAlertListWidget.Description = "List of Active Alerts";
            activeAlertListWidget.Version = SQLDM_VERSION_FOR_WIDGETS;

            Dictionary<string, string> settingActiveAlertListWidget = new Dictionary<string, string>();
            settingActiveAlertListWidget.Add("Limit", "10");//for limiting the records to 10 by default
            activeAlertListWidget.Settings = settingActiveAlertListWidget;

            activeAlertListWidget.DefaultViews = "Overview";
            activeAlertListWidget.Collapsed = true;

            //SQLdm 9.1 (Gaurav Karwal): List of Instances widget - shows up on the Overview tab on Idera dashboard
            CWFContracts.DashboardWidget instanceListWidget = new CWFContracts.DashboardWidget();
            instanceListWidget.Name = "SQLDM – Instances List";
            instanceListWidget.Type = "Top X";
            instanceListWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            instanceListWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instancesListWidget.zul";
            instanceListWidget.DataURI = "/Instances";
            instanceListWidget.Description = "List of Instances";
            instanceListWidget.Version = SQLDM_VERSION_FOR_WIDGETS;

            Dictionary<string, string> settingInstanceListWidget = new Dictionary<string, string>();
            settingInstanceListWidget.Add("Limit", "10");//for limiting the records to 10 by default
            instanceListWidget.Settings = settingInstanceListWidget;

            instanceListWidget.DefaultViews = "Overview";
            instanceListWidget.Collapsed = true;

            //START: SQLdm 10.0 (Gaurav Karwal): including the top x widgets

            //top instance by memory usage

            CWFContracts.DashboardWidget memoryUsageWidget = new CWFContracts.DashboardWidget();
            memoryUsageWidget.Name = "SQLDM – Top Instances by Memory Usage";
            memoryUsageWidget.Type = "Top X";
            memoryUsageWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            memoryUsageWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instancesByMemoryUsageWidget.zul";
            memoryUsageWidget.DataURI = "/Instances/BySqlMemoryUsage";
            memoryUsageWidget.Description = "List top instances by memory usage";
            memoryUsageWidget.Version = SQLDM_VERSION_FOR_WIDGETS;
            Dictionary<string, string> settingMemoryUsageWidget = new Dictionary<string, string>();
            settingMemoryUsageWidget.Add("Limit", "10");//for limiting the records to 10 by default
            memoryUsageWidget.Settings = settingMemoryUsageWidget;
            memoryUsageWidget.DefaultViews = "";
            memoryUsageWidget.Collapsed = true;

            //top by response time
            CWFContracts.DashboardWidget responseTimeWidget = new CWFContracts.DashboardWidget();
            responseTimeWidget.Name = "SQLDM – Top Instances by Response Time";
            responseTimeWidget.Type = "Top X";
            responseTimeWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            responseTimeWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instancesByResponseTimeWidget.zul";
            responseTimeWidget.DataURI = "/Instances/TopInstanceByResponseTime/tzo/{timeZoneOffset}";
            responseTimeWidget.Description = "List top instances by response time";
            responseTimeWidget.Version = SQLDM_VERSION_FOR_WIDGETS;
            Dictionary<string, string> settingResponseTimeWidget = new Dictionary<string, string>();
            settingResponseTimeWidget.Add("Limit", "10");//for limiting the records to 10 by default
            responseTimeWidget.Settings = settingResponseTimeWidget;
            responseTimeWidget.DefaultViews = "";
            responseTimeWidget.Collapsed = true;

            //by waits
            CWFContracts.DashboardWidget instanceByWaitsWidget = new CWFContracts.DashboardWidget();
            instanceByWaitsWidget.Name = "SQLDM – Top Instances by Waits";
            instanceByWaitsWidget.Type = "Top X";
            instanceByWaitsWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            instanceByWaitsWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instancesByWaitsWidget.zul";
            instanceByWaitsWidget.DataURI = "/Instances/TopInstancesByWaits";
            instanceByWaitsWidget.Description = "List top instances by waits";
            instanceByWaitsWidget.Version = SQLDM_VERSION_FOR_WIDGETS;
            Dictionary<string, string> settingInstanceByWaitsWidget = new Dictionary<string, string>();
            settingInstanceByWaitsWidget.Add("Limit", "10");//for limiting the records to 10 by default
            instanceByWaitsWidget.Settings = settingInstanceByWaitsWidget;
            instanceByWaitsWidget.DefaultViews = "";
            instanceByWaitsWidget.Collapsed = true;

            //by queries
            CWFContracts.DashboardWidget instanceByQueriesWidget = new CWFContracts.DashboardWidget();
            instanceByQueriesWidget.Name = "SQLDM – Top Instances by Queries";
            instanceByQueriesWidget.Type = "Top X";
            instanceByQueriesWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            instanceByQueriesWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instancesByQueriesWidget.zul";
            instanceByQueriesWidget.DataURI = "/Instances/TopInstanceByQueryCount/tzo/{timeZoneOffset}";
            instanceByQueriesWidget.Description = "List top instances by queries";
            instanceByQueriesWidget.Version = SQLDM_VERSION_FOR_WIDGETS;
            Dictionary<string, string> settingInstanceByQueriesWidget = new Dictionary<string, string>();
            settingInstanceByQueriesWidget.Add("Limit", "10");//for limiting the records to 10 by default
            instanceByQueriesWidget.Settings = settingInstanceByQueriesWidget;
            instanceByQueriesWidget.DefaultViews = "";
            instanceByQueriesWidget.Collapsed = true;

            //by IO
            CWFContracts.DashboardWidget instanceByIOWidget = new CWFContracts.DashboardWidget();
            instanceByIOWidget.Name = "SQLDM – Top Instances by I/O";
            instanceByIOWidget.Type = "Top X";
            instanceByIOWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            instanceByIOWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instancesByIOWidget.zul";
            instanceByIOWidget.DataURI = "/Instances/ByIOPhysicalCount";
            instanceByIOWidget.Description = "List top instances by I/O";
            instanceByIOWidget.Version = SQLDM_VERSION_FOR_WIDGETS;
            Dictionary<string, string> settingInstanceByIOWidget = new Dictionary<string, string>();
            settingInstanceByIOWidget.Add("Limit", "10");//for limiting the records to 10 by default
            instanceByIOWidget.Settings = settingInstanceByIOWidget;
            instanceByIOWidget.DefaultViews = "";
            instanceByIOWidget.Collapsed = true;

            //by sessions
            CWFContracts.DashboardWidget instanceBySessionsWidget = new CWFContracts.DashboardWidget();
            instanceBySessionsWidget.Name = "SQLDM – Top Instances by Sessions";
            instanceBySessionsWidget.Type = "Top X";
            instanceBySessionsWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            instanceBySessionsWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instancesBySessionsWidget.zul";
            instanceBySessionsWidget.DataURI = "/Instances/TopInstanceBySessionCount/tzo/{timeZoneOffset}";
            instanceBySessionsWidget.Description = "List top instances by Sessions";
            instanceBySessionsWidget.Version = SQLDM_VERSION_FOR_WIDGETS;
            Dictionary<string, string> settingInstanceBySessionsWidget = new Dictionary<string, string>();
            settingInstanceBySessionsWidget.Add("Limit", "10");//for limiting the records to 10 by default
            instanceBySessionsWidget.Settings = settingInstanceBySessionsWidget;
            instanceBySessionsWidget.DefaultViews = "";
            instanceBySessionsWidget.Collapsed = true;

            //by blocked sessions
            CWFContracts.DashboardWidget instanceByBlockedSessionsWidget = new CWFContracts.DashboardWidget();
            instanceByBlockedSessionsWidget.Name = "SQLDM – Top Instances by Blocked Sessions";
            instanceByBlockedSessionsWidget.Type = "Top X";
            instanceByBlockedSessionsWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            instanceByBlockedSessionsWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instancesByBlockedSessionsWidget.zul";
            instanceByBlockedSessionsWidget.DataURI = "/Instances/TopInstanceByBlockedSessions/tzo/{timeZoneOffset}";
            instanceByBlockedSessionsWidget.Description = "List top instances by Blocked Sessions";
            instanceByBlockedSessionsWidget.Version = SQLDM_VERSION_FOR_WIDGETS;
            Dictionary<string, string> settingInstanceByBlockedSessionsWidget = new Dictionary<string, string>();
            settingInstanceByBlockedSessionsWidget.Add("Limit", "10");//for limiting the records to 10 by default
            instanceByBlockedSessionsWidget.Settings = settingInstanceByBlockedSessionsWidget;
            instanceByBlockedSessionsWidget.DefaultViews = "";
            instanceByBlockedSessionsWidget.Collapsed = true;

            //by active connections
            CWFContracts.DashboardWidget instanceByActiveConnectionsWidget = new CWFContracts.DashboardWidget();
            instanceByActiveConnectionsWidget.Name = "SQLDM – Top Instances by Active Connections";
            instanceByActiveConnectionsWidget.Type = "Top X";
            instanceByActiveConnectionsWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            instanceByActiveConnectionsWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instancesByActiveConnectionsWidget.zul";
            instanceByActiveConnectionsWidget.DataURI = "/Instances/TopInstancesByConnCount/tzo/{timeZoneOffset}";
            instanceByActiveConnectionsWidget.Description = "List top instances by Active Connections";
            instanceByActiveConnectionsWidget.Version = SQLDM_VERSION_FOR_WIDGETS;
            Dictionary<string, string> settingInstanceByActiveConnectionsWidget = new Dictionary<string, string>();
            settingInstanceByActiveConnectionsWidget.Add("Limit", "10");//for limiting the records to 10 by default
            instanceByActiveConnectionsWidget.Settings = settingInstanceByActiveConnectionsWidget;
            instanceByActiveConnectionsWidget.DefaultViews = "";
            instanceByActiveConnectionsWidget.Collapsed = true;

            //by disk space util
            CWFContracts.DashboardWidget instanceByDiskSpaceUtilizationWidget = new CWFContracts.DashboardWidget();
            instanceByDiskSpaceUtilizationWidget.Name = "SQLDM – Top Instances by Disk Space Utilization";
            instanceByDiskSpaceUtilizationWidget.Type = "Top X";
            instanceByDiskSpaceUtilizationWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            instanceByDiskSpaceUtilizationWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instancesByDiskSpaceWidget.zul";
            instanceByDiskSpaceUtilizationWidget.DataURI = "/Instances/TopInstanceByDiskSpace/tzo/{timeZoneOffset}";
            instanceByDiskSpaceUtilizationWidget.Description = "List top instances by Disk Space Utilization";
            instanceByDiskSpaceUtilizationWidget.Version = SQLDM_VERSION_FOR_WIDGETS;
            Dictionary<string, string> settingInstanceByDiskSpaceUtilizationWidget = new Dictionary<string, string>();
            settingInstanceByDiskSpaceUtilizationWidget.Add("Limit", "10");//for limiting the records to 10 by default
            instanceByDiskSpaceUtilizationWidget.Settings = settingInstanceByDiskSpaceUtilizationWidget;
            instanceByDiskSpaceUtilizationWidget.DefaultViews = "";
            instanceByDiskSpaceUtilizationWidget.Collapsed = true;

            //by temp db utilization
            CWFContracts.DashboardWidget instanceByTempDBSpaceUtilizationWidget = new CWFContracts.DashboardWidget();
            instanceByTempDBSpaceUtilizationWidget.Name = "SQLDM – Top Instances by TempDB Space Utilization";
            instanceByTempDBSpaceUtilizationWidget.Type = "Top X";
            instanceByTempDBSpaceUtilizationWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            instanceByTempDBSpaceUtilizationWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instancesByTempDBUtilizationWidget.zul";
            instanceByTempDBSpaceUtilizationWidget.DataURI = "/Instances/InstancesByTempDbUtilization/tzo/{timeZoneOffset}";
            instanceByTempDBSpaceUtilizationWidget.Description = "List top instances by TempDB Space Utilization";
            instanceByTempDBSpaceUtilizationWidget.Version = SQLDM_VERSION_FOR_WIDGETS;
            Dictionary<string, string> settingInstanceByTempDBSpaceUtilizationWidget = new Dictionary<string, string>();
            settingInstanceByTempDBSpaceUtilizationWidget.Add("Limit", "10");//for limiting the records to 10 by default
            instanceByTempDBSpaceUtilizationWidget.Settings = settingInstanceByTempDBSpaceUtilizationWidget;
            instanceByTempDBSpaceUtilizationWidget.DefaultViews = "";
            instanceByTempDBSpaceUtilizationWidget.Collapsed = true;

            //by top session CPU
            CWFContracts.DashboardWidget sessionsByCPUUsageWidget = new CWFContracts.DashboardWidget();
            sessionsByCPUUsageWidget.Name = "SQLDM – Top Sessions by CPU Usage";
            sessionsByCPUUsageWidget.Type = "Top X";
            sessionsByCPUUsageWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            sessionsByCPUUsageWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/sessionsByCpuUsageWidget.zul";
            sessionsByCPUUsageWidget.DataURI = "/Instances/TopSessionsByCPUUsage/tzo/{timeZoneOffset}";
            sessionsByCPUUsageWidget.Description = "List top sessions by CPU usage";
            sessionsByCPUUsageWidget.Version = SQLDM_VERSION_FOR_WIDGETS;
            Dictionary<string, string> settingSessionsByCPUUsageWidget = new Dictionary<string, string>();
            settingSessionsByCPUUsageWidget.Add("Limit", "10");//for limiting the records to 10 by default
            sessionsByCPUUsageWidget.Settings = settingSessionsByCPUUsageWidget;
            sessionsByCPUUsageWidget.DefaultViews = "";
            sessionsByCPUUsageWidget.Collapsed = true;



            //by top queries by ex time
            CWFContracts.DashboardWidget queriesByExecutionTimeWidget = new CWFContracts.DashboardWidget();
            queriesByExecutionTimeWidget.Name = "SQLDM – Top Queries by Execution Time";
            queriesByExecutionTimeWidget.Type = "Top X";
            queriesByExecutionTimeWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            queriesByExecutionTimeWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/queriesByExecutionTimeWidget.zul";
            queriesByExecutionTimeWidget.DataURI = "/Instances/TopInstanceByQueryDuration/tzo/{timeZoneOffset}";
            queriesByExecutionTimeWidget.Description = "List top queries by Execution Time";
            queriesByExecutionTimeWidget.Version = SQLDM_VERSION_FOR_WIDGETS;
            Dictionary<string, string> settingQueriesByExecutionTimeWidget = new Dictionary<string, string>();
            settingQueriesByExecutionTimeWidget.Add("Limit", "10");//for limiting the records to 10 by default
            queriesByExecutionTimeWidget.Settings = settingQueriesByExecutionTimeWidget;
            queriesByExecutionTimeWidget.DefaultViews = "";
            queriesByExecutionTimeWidget.Collapsed = true;

            //by top databases by size
            CWFContracts.DashboardWidget databaseBySizeWidget = new CWFContracts.DashboardWidget();
            databaseBySizeWidget.Name = "SQLDM – Top Databases by Size";
            databaseBySizeWidget.Type = "Top X";
            databaseBySizeWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            databaseBySizeWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/databasesBySizeWidget.zul";
            databaseBySizeWidget.DataURI = "/Instances/TopDatabasesBySize/tzo/{timeZoneOffset}";
            databaseBySizeWidget.Description = "List top databases by Size";
            databaseBySizeWidget.Version = SQLDM_VERSION_FOR_WIDGETS;
            Dictionary<string, string> settingDatabaseBySizeWidget = new Dictionary<string, string>();
            settingDatabaseBySizeWidget.Add("Limit", "10");//for limiting the records to 10 by default
            databaseBySizeWidget.Settings = settingDatabaseBySizeWidget;
            databaseBySizeWidget.DefaultViews = "";
            databaseBySizeWidget.Collapsed = true;

            //by top databases by growth
            CWFContracts.DashboardWidget databaseByGrowthWidget = new CWFContracts.DashboardWidget();
            databaseByGrowthWidget.Name = "SQLDM – Top Databases by Growth";
            databaseByGrowthWidget.Type = "Top X";
            databaseByGrowthWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            databaseByGrowthWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/databasesByGrowthWidget.zul";
            databaseByGrowthWidget.DataURI = "/Instances/GetTopDatabasesByGrowth/tzo/{timeZoneOffset}";
            databaseByGrowthWidget.Description = "List top databases by Growth";
            databaseByGrowthWidget.Version = SQLDM_VERSION_FOR_WIDGETS;
            Dictionary<string, string> settingDatabaseByGrowthWidget = new Dictionary<string, string>();
            settingDatabaseByGrowthWidget.Add("Limit", "10");//for limiting the records to 10 by default
            settingDatabaseByGrowthWidget.Add("numDays", "7");
            databaseByGrowthWidget.Settings = settingDatabaseByGrowthWidget;
            databaseByGrowthWidget.DefaultViews = "";
            databaseByGrowthWidget.Collapsed = true;

            //by top databases by activity
            CWFContracts.DashboardWidget databaseByActivityWidget = new CWFContracts.DashboardWidget();
            databaseByActivityWidget.Name = "SQLDM – Top Databases by Activity";
            databaseByActivityWidget.Type = "Top X";
            databaseByActivityWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            databaseByActivityWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/databasesByActivityWidget.zul";
            databaseByActivityWidget.DataURI = "/Instances/TopDatabaseByActivity/tzo/{timeZoneOffset}";
            databaseByActivityWidget.Description = "List top databases by Activity";
            databaseByActivityWidget.Version = SQLDM_VERSION_FOR_WIDGETS;
            Dictionary<string, string> settingDatabaseByActivityWidget = new Dictionary<string, string>();
            settingDatabaseByActivityWidget.Add("Limit", "10");//for limiting the records to 10 by default
            databaseByActivityWidget.Settings = settingDatabaseByActivityWidget;
            databaseByActivityWidget.DefaultViews = "";
            databaseByActivityWidget.Collapsed = true;

            //END: SQLdm 10.0 (Gaurav Karwal): including the top x widgets

            //adding all widges to the list
            allDashboardWidgets.Add(overallStatusWidget);
            allDashboardWidgets.Add(instanceStatusWidget);
            allDashboardWidgets.Add(topInstancesbyAlertCountWidget);
            allDashboardWidgets.Add(topDatabasesByAlertCountWidget);
            allDashboardWidgets.Add(topInstanceByCPUUsageWidget);
            allDashboardWidgets.Add(alertCountsByCategoryWidget);
            allDashboardWidgets.Add(activeAlertListWidget);
            allDashboardWidgets.Add(instanceListWidget);
            //START: SQLdm 10.0 (Gaurav Karwal): including the top x widgets
            allDashboardWidgets.Add(memoryUsageWidget);
            allDashboardWidgets.Add(responseTimeWidget);
            allDashboardWidgets.Add(instanceByWaitsWidget);
            allDashboardWidgets.Add(instanceByQueriesWidget);
            allDashboardWidgets.Add(instanceByIOWidget);
            allDashboardWidgets.Add(instanceBySessionsWidget);
            allDashboardWidgets.Add(instanceByBlockedSessionsWidget);
            allDashboardWidgets.Add(instanceByActiveConnectionsWidget);
            allDashboardWidgets.Add(instanceByDiskSpaceUtilizationWidget);
            allDashboardWidgets.Add(instanceByTempDBSpaceUtilizationWidget);
            allDashboardWidgets.Add(sessionsByCPUUsageWidget);
            allDashboardWidgets.Add(queriesByExecutionTimeWidget);
            allDashboardWidgets.Add(databaseBySizeWidget);
            allDashboardWidgets.Add(databaseByGrowthWidget);
            allDashboardWidgets.Add(databaseByActivityWidget);
            //END: SQLdm 10.0 (Gaurav Karwal): including the top x widgets
            return allDashboardWidgets;
        }

    }
}