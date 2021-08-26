using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Idera.SQLdm.Service.DataContracts.v1.CustomDashboard;
using Idera.SQLdm.Service.ServiceContracts.v1;
using Idera.SQLdm.Service.Repository;
using Idera.SQLdm.Service.Configuration;
using System.ServiceModel.Web;
using System.Net;
using Idera.SQLdm.Service.Helpers;

namespace Idera.SQLdm.Service.Web
{
    /// <summary>
    /// SQLdm 10.0 (Srishti Purohit): implementing CustomDashboard functionality
    /// </summary>
    public partial class WebService : ICustomDashboard
    {
        #region CustomDashboardModule
        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To Insert new custom dashboard record in CustomDashboard Table
        /// </summary>
        /// <param name="customDashboardName"></param>
        /// <param name="isDefault"></param>
        /// <param name="userSID"></param>
        public CustomDashboard CreateCustomDashboard(string customDashboardName, string isDefault)
        {
            SetConnectionCredentiaslFromCWFHost();
            CustomDashboard responseCustomDashboard = null;

            bool parseRes;
            string userSID = userToken.UserSID;

            if (string.IsNullOrWhiteSpace(customDashboardName) || string.IsNullOrWhiteSpace(isDefault) || string.IsNullOrWhiteSpace(userSID))
            {
                throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Parameters supplied (CustomDashboardName/ isDefault/ user SID) are invalid.");
            }
            else
            {
                //Check if the same named dashboard is already there
                if (RepositoryHelper.CheckDuplicateDashboardName(RestServiceConfiguration.SQLConnectInfo, customDashboardName, userSID))
                {
                    throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Dashboard name must be unique.");
                }
                else
                {
                    if (bool.TryParse(isDefault, out parseRes))
                    {
                        responseCustomDashboard = new CustomDashboard();
                        responseCustomDashboard = RepositoryHelper.CreateCustomDashboard(RestServiceConfiguration.SQLConnectInfo, customDashboardName, Convert.ToBoolean(isDefault), userSID);
                        if (responseCustomDashboard == null)
                            throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Some error occured while adding record in CustomDashboard DB table.");
                    }
                    else
                        throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Value for isDefaultOnUI is incorrect.");

                }
            }
            return responseCustomDashboard;
        }

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To delete the selected custom dashboard
        /// </summary>
        /// <param name="customDashBoardId"></param>
        public CustomDashboard Delete(string customDashBoardId)
        {
            SetConnectionCredentiaslFromCWFHost();
            CustomDashboard deletedRecord = new CustomDashboard();
            int custId = 0;
            //Valid custId format
            if (!int.TryParse(customDashBoardId, out custId))
            {
                throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Value for customDashBoard ID is incorrect/ not a number.");
            }
            else
                deletedRecord = RepositoryHelper.DeleteDashboardById(RestServiceConfiguration.SQLConnectInfo, custId);
            if (deletedRecord == null)
                throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Some error occured while deleting record in CustomDashboard DB table.");

            return deletedRecord;
        }

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To get create custom dashboards
        /// </summary>
        /// <param name="userSID"></param>
        public List<CustomDashboard> GetAllDasboards()
        {
            SetConnectionCredentiaslFromCWFHost();
            List<CustomDashboard> allCustomDashboards = new List<CustomDashboard>();

            string userSID = userToken.UserSID;
            
            if (string.IsNullOrWhiteSpace(userSID))
            {
                throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Parameters supplied (CustomDashboardName/ isDefault/ user SID) are invalid.");
            }
            else
            {
                allCustomDashboards = RepositoryHelper.GetAllCustomDashboards(RestServiceConfiguration.SQLConnectInfo, userSID);
            }
            return allCustomDashboards;
        }

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To update existing Custom Dashboard for a user
        /// </summary>
        /// <param name="customDashboardName"></param>
        /// <param name="isDefault"></param>
        /// <param name="tags"></param>
        public CustomDashboard UpdateCustomDashboard(string customDashboardid, string customDashboardName, string isDefault, string tags)
        {
            SetConnectionCredentiaslFromCWFHost();
            CustomDashboard responseCustomDashboard = null;
            Int64 intDashboardId = 0;

            string userSID = userToken.UserSID;
            
            bool parseRes;
            if (!Int64.TryParse(customDashboardid, out intDashboardId))
            {
                throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Dashboard ID supplied is invalid.");
            }
            else
            {

                //Check if the same named dashboard is already there
                if (!RepositoryHelper.CheckDashboardExists(RestServiceConfiguration.SQLConnectInfo, intDashboardId))
                {
                    throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Dashboard does not exists.");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(customDashboardName) || string.IsNullOrWhiteSpace(isDefault))
                    {
                        throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Parameters supplied (CustomDashboardName/ isDefault/ user SID) are invalid.");
                    }
                    else
                    {
                        if (bool.TryParse(isDefault, out parseRes))
                        {
                            responseCustomDashboard = new CustomDashboard();
                            responseCustomDashboard = RepositoryHelper.UpdateCustomDashboard(RestServiceConfiguration.SQLConnectInfo, intDashboardId, customDashboardName, Convert.ToBoolean(isDefault), userSID, tags);
                            if (responseCustomDashboard == null)
                                throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Some error occured while adding record in CustomDashboard DB table.");
                        }
                        else
                            throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Value for isDefaultOnUI is incorrect.");
                    }
                }
            }
            return responseCustomDashboard;
        }

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To make copy of existing Custom Dashboard for a user
        /// </summary>
        /// <param name="customDashboardid"></param>
        public bool CreateCopyCustomDashboard(string customDashboardid)
        {
            SetConnectionCredentiaslFromCWFHost();
            Int64 intDashboardId = 0;

            bool copyResult;
            if (!Int64.TryParse(customDashboardid, out intDashboardId))
            {
                throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Dashboard ID supplied is invalid.");
            }
            else
            {

                //Check if the same named dashboard is already there
                if (!RepositoryHelper.CheckDashboardExists(RestServiceConfiguration.SQLConnectInfo, intDashboardId))
                {
                    throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Dashboard does not exists.");
                }
                else
                {
                            copyResult = RepositoryHelper.CopyCustomDashboard(RestServiceConfiguration.SQLConnectInfo, intDashboardId);
                           if(!copyResult)
                            throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Could not create copy of Custom dashboard. some error while running DB script.");
                    }
                }
            return copyResult;
        }

        #endregion
        #region CustomWidgetsForDashboard

        /// <summary>
        /// SQLdm 10.0 (Srishti Purohit): implementing CustomDashboard adding widgets to dashboard functionality
        /// </summary>
        /// <param name="customDashboardId"></param>
        /// <param param name="widgetName"></param>
        /// <param name="widgetTypeId"></param>
        /// <param name="metricId"></param>
        /// <param name="sourceServerIds"></param>
        public CustomDashboardWidgets CreateCustomDashboardWidget(string customDashboardId, string widgetName, string widgetTypeId, string metricId, string match, string tagIds, string sourceServerIds)
        {
            SetConnectionCredentiaslFromCWFHost();
            CustomDashboardWidgets responseCustomDashboard = new CustomDashboardWidgets();
            int dashboardId = -1, widTypeId = -1, widMetricId = -1;
            int matchSeq = -1;
            
            if (tagIds == null) tagIds = string.Empty;

            List<string> tagId = tagIds.Split(',').ToList<string>();
            List<int> tagIdList = null;
            List<string> sourceId = sourceServerIds.Split(',').ToList<string>();
            List<int> sourceIdList = new List<int>();
            List<int> sIdList = new List<int>();
            int res;
            tagIdList = tagId.Select(f => int.TryParse(f, out res) ? res : -1).ToList();
            //sourceIdList = sourceId.Select(f => int.TryParse(f, out res) ? res : -1).ToList();

            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security            
            sIdList = sourceId.Select(f => int.TryParse(f, out res) ? res : -1).ToList();
            sourceIdList = sIdList.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x)).ToList();
            //END

            if (int.TryParse(customDashboardId, out dashboardId) && int.TryParse(widgetTypeId, out widTypeId) && int.TryParse(metricId, out widMetricId) && int.TryParse(match, out matchSeq))
            {
                switch (matchSeq)
                {
                    //Single source
                    case 1:
                        {
                            if (!sourceIdList.Contains(-1) && sourceIdList.Count == 1)
                            {
                                responseCustomDashboard = getResponseCreateWIdget(responseCustomDashboard, dashboardId, widgetName, widTypeId, widMetricId, matchSeq, null, sourceIdList);
                            }
                            else
                                throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Sourcce or tag Parameters supplied are invalid.");
                            break;
                        }
                    //Multiple Sourece
                    case 2:
                        {
                            if (!sourceIdList.Contains(-1) && sourceIdList.Count >= 1)
                            {
                                responseCustomDashboard = getResponseCreateWIdget(responseCustomDashboard, dashboardId, widgetName, widTypeId, widMetricId, matchSeq, null, sourceIdList);
                            }
                            else
                                throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Sourcce or tag Parameters supplied are invalid.");
                            break;
                        }
                    //Select by Tag Names
                    case 3:
                        {
                            if (tagIdList!=null && !tagIdList.Contains(-1) && tagIdList.Count >= 1)
                            {
                                responseCustomDashboard = getResponseCreateWIdget(responseCustomDashboard, dashboardId, widgetName, widTypeId, widMetricId, matchSeq, tagIdList, null);
                            }
                            else
                                throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Sourcce or tag Parameters supplied are invalid.");
                            break;
                        }
                    //All Source
                    case 4:
                        {

                            responseCustomDashboard = getResponseCreateWIdget(responseCustomDashboard, dashboardId, widgetName, widTypeId, widMetricId, matchSeq, null, null);

                            break;
                        }
                    default:
                        {
                            throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Match supplied is invalid.");
                        }
                }
            }
            else
            {
                throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Parameters supplied are invalid.");

            }
            return responseCustomDashboard;
        }
        private static CustomDashboardWidgets getResponseCreateWIdget(CustomDashboardWidgets responseCustomDashboard, int dashboardId, string widgetName, int widTypeId, int widMetricId, int matchSeq, List<int> tagNumber, List<int> sourceIdList)
        {
            responseCustomDashboard.relatedCustomDashboardID = dashboardId;
            responseCustomDashboard.WidgetName = widgetName;
            responseCustomDashboard.WidgetTypeID = widTypeId;
            responseCustomDashboard.MetricID = widMetricId;
            responseCustomDashboard.Match = matchSeq;
            responseCustomDashboard.TagId = tagNumber;
            responseCustomDashboard.sqlServerId = sourceIdList;

            if (responseCustomDashboard.WidgetID != 0)
            {
                responseCustomDashboard = RepositoryHelper.UpdateCustomDashboardWidget(RestServiceConfiguration.SQLConnectInfo, responseCustomDashboard);
                _logX.Info("Widget ID updated for widgetID " + responseCustomDashboard.WidgetID);
            }
            else
            {
                responseCustomDashboard = RepositoryHelper.AddWidgetToCustomDashboard(RestServiceConfiguration.SQLConnectInfo, responseCustomDashboard);
                _logX.Info("Widget ID inserted in DB for widgetID " + responseCustomDashboard.WidgetID);
            }
            
            return responseCustomDashboard;
        }

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To get all Widgets for specific Custom Dashboard for a user
        /// </summary>

        public List<CustomDashboardWidgets> GetAllWidgets(string id)
        {
            SetConnectionCredentiaslFromCWFHost();
            List<CustomDashboardWidgets> responseWidgetList = new List<CustomDashboardWidgets>();
            int dashboardId;
            
            if (int.TryParse(id, out dashboardId))
            {
                //SQLdm 10.0 (Swati Gogia):Passed userToken to GetAllWidgets to Implement Instance level security
                responseWidgetList = RepositoryHelper.GetAllWidgets(RestServiceConfiguration.SQLConnectInfo, dashboardId, userToken);

            }
            else
            {
                throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Dashboard ID supplied is invalid.");

            }
            return responseWidgetList;

        }

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To update existing Widget for specific Custom Dashboard for a user
        /// </summary>
        /// <param name="customDashboardid"></param>
        /// <param name="widgetId"></param>
        /// <param name="widgetName"></param>
        /// <param name="widgetTypeId"></param>
        /// <param name="metricId"></param>
        /// <param name="match"></param>
        /// <param name="tagIds"></param>
        /// <param name="sourceServerIds"></param>
        public CustomDashboardWidgets UpdateCustomDashboardWidget(string customDashboardid, string widgetId, string widgetName, string widgetTypeId, string metricId, string match, string tagIds, string sourceServerIds)
        {
            SetConnectionCredentiaslFromCWFHost();
            CustomDashboardWidgets responseCustomDashboard = new CustomDashboardWidgets();
            int dashboardId = -1, intWidgetId =-1, widTypeId = -1, widMetricId = -1;
            int matchSeq = -1;
            List<string> tagId = tagIds.Split(',').ToList<string>();
            List<int> tagIdList = new List<int>();
            List<string> sourceId = sourceServerIds.Split(',').ToList<string>();
            List<int> sourceIdList = new List<int>();
            List<int> sIdList = new List<int>();//added
            int res;
            tagIdList = tagId.Select(f => int.TryParse(f, out res) ? res : -1).ToList();
           // sourceIdList = sourceId.Select(f => int.TryParse(f, out res) ? res : -1).ToList();

            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            sIdList = sourceId.Select(f => int.TryParse(f, out res) ? res : -1).ToList();
            sourceIdList = sIdList.Where(x => userToken.AssignedServers.Any(y => y.Server.SQLServerID == x)).ToList();
            //END

            if (int.TryParse(customDashboardid, out dashboardId) && int.TryParse(widgetId, out intWidgetId) && int.TryParse(widgetTypeId, out widTypeId) && int.TryParse(metricId, out widMetricId) && int.TryParse(match, out matchSeq))
            {
                responseCustomDashboard.WidgetID = intWidgetId;
                switch (matchSeq)
                {
                    //Single source
                    case 1:
                        {
                            if (!sourceIdList.Contains(-1) && sourceIdList.Count == 1)
                            {
                                responseCustomDashboard = getResponseCreateWIdget(responseCustomDashboard, dashboardId, widgetName, widTypeId, widMetricId, matchSeq, null, sourceIdList);
                            }
                            else
                                throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Sourcce or tag Parameters supplied are invalid.");
                            break;
                        }
                    //Multiple Sourece
                    case 2:
                        {
                            if (!sourceIdList.Contains(-1) && sourceIdList.Count >= 1)
                            {
                                responseCustomDashboard = getResponseCreateWIdget(responseCustomDashboard, dashboardId, widgetName, widTypeId, widMetricId, matchSeq, null, sourceIdList);
                            }
                            else
                                throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Sourcce or tag Parameters supplied are invalid.");
                            break;
                        }
                    //Select by Tag Names
                    case 3:
                        {
                            if (!tagIdList.Contains(-1) && tagIdList.Count >= 1)
                            {
                                responseCustomDashboard = getResponseCreateWIdget(responseCustomDashboard, dashboardId, widgetName, widTypeId, widMetricId, matchSeq, tagIdList, null);
                            }
                            else
                                throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Sourcce or tag Parameters supplied are invalid.");
                            break;
                        }
                    //All Source
                    case 4:
                        {

                            responseCustomDashboard = getResponseCreateWIdget(responseCustomDashboard, dashboardId, widgetName, widTypeId, widMetricId, matchSeq, null, null);

                            break;
                        }
                    default:
                        {
                            throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Match supplied is invalid.");
                        }
                }
            }
            else
            {
                throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Parameters supplied are invalid.");

            }
            return responseCustomDashboard;
        }

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To delete the selected custom dashboard Widget
        /// </summary>
        /// <param name="customDashboardid"></param>
        /// <param name="widgetId"></param>
        public bool DeleteWidget(string customDashboardid, string widgetId)
        {

            SetConnectionCredentiaslFromCWFHost();
            int parsedWidgetId = 0;
            bool isRecordDeleted = false;
            //Valid custId format
            if (!int.TryParse(widgetId, out parsedWidgetId))
            {
                throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Value for widget ID is incorrect/ not a number.");
            }
            else
                isRecordDeleted = RepositoryHelper.DeleteWidgetById(RestServiceConfiguration.SQLConnectInfo, parsedWidgetId);
            if (isRecordDeleted == false)
                throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Some error occured while remoing record from widget DB table. Might be widget ID does not exists.");

            return isRecordDeleted;
        }

        #endregion
        #region MasterDataForCustomDashboard

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To get all Widget Types
        /// </summary>        
        public Dictionary<int, string> GetAllWidgetTypes()
        {
            SetConnectionCredentiaslFromCWFHost();
            Dictionary<int, string> widgetTypes = new Dictionary<int, string>();
            widgetTypes = RepositoryHelper.GetAllWidgetTypes(RestServiceConfiguration.SQLConnectInfo);

            return widgetTypes;
        }

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) : To get all Match Types
        /// </summary>        
        public Dictionary<int, string> GetAllMatchTypes()
        {
            SetConnectionCredentiaslFromCWFHost();
            Dictionary<int, string> matchTypes = new Dictionary<int, string>();
            matchTypes = RepositoryHelper.GetAllMatchTypes(RestServiceConfiguration.SQLConnectInfo);

            return matchTypes;
        }
        #endregion


        #region MetricValuesForWidgets

        /// <summary>
        /// Get Metric values for widgets
        /// Author: srishti purohit
        /// Product Version: SQLdm 10.0
        /// </summary>
        /// <param name="dashboardId"></param>
        /// <param name="widgetId"></param>
        /// <param name="timeZoneOffset"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public MetricValueCollectionForCustomDashboard GetMetricValueForWidget(string dashboardId, string widgetId, string timeZoneOffset, DateTime startTime, DateTime endTime)
        {
            int intDashboardId;
            int intWidgetId;
            SetConnectionCredentiaslFromCWFHost();
            DateTime startDateTimeInUTC = DateTimeHelper.ConvertToUTC(startTime, timeZoneOffset);
            DateTime endDateTimeInUTC = DateTimeHelper.ConvertToUTC(endTime, timeZoneOffset);
            //List<MetricValueforCustomDashboard> allMetricValuesForCustomWidgets = null;
            MetricValueCollectionForCustomDashboard allMetricValuesForCustomWidget = null;
            ConfigureDateTime(ref startDateTimeInUTC, ref endDateTimeInUTC);
            if (!int.TryParse(dashboardId, out intDashboardId) || !int.TryParse(widgetId, out intWidgetId))
            {
                throw new FaultException<WebFaultException>(new WebFaultException(HttpStatusCode.BadRequest), "Dashboard ID/ Widget ID supplied is invalid.");
            }
            else
            {
                //allMetricValuesForCustomWidgets = new List<MetricValueforCustomDashboard>();

                //SQLdm 10.0 (Swati Gogia):Passed userToken to GetAllMetricValuesForCustomWidget to Implement Instance level security
                var MetricsValueList = RepositoryHelper.GetAllMetricValuesForCustomWidget(RestServiceConfiguration.SQLConnectInfo, intDashboardId, intWidgetId, timeZoneOffset, startDateTimeInUTC, endDateTimeInUTC,userToken);
                allMetricValuesForCustomWidget = new MetricValueCollectionForCustomDashboard(MetricsValueList);
            }

            return (allMetricValuesForCustomWidget);
            //return allMetricValuesForCustomWidgets;
        }
        #endregion

    }
}
