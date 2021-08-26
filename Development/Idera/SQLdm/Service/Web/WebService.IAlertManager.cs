using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using Idera.SQLdm.Service.ServiceContracts.v1;
using Idera.SQLdm.Service.Repository;
using Idera.SQLdm.Service.Helpers;
using System.Data.SqlTypes;

using Idera.SQLdm.Service.DataContracts.v1;
using System.ServiceModel;
using Idera.SQLdm.Service.Configuration;
using System.ServiceModel.Web;
using Idera.SQLdm.Service.Helpers.Auth;
using System.Diagnostics;
using Idera.SQLdm.Service.Helpers.CWF;
using Idera.SQLdm.Common.Helpers;

namespace Idera.SQLdm.Service.Web
{
    public partial class WebService : IAlertManager
    {
        #region IAlertManager Members

/*        public IList<Alert> GetAlerts(string instanceId, int startingAlertId, int maxRows, DateTime startDate, DateTime endDate, string severity, string metricid, string category, bool isActive)
        {
            string[] instanceList = null;            
            Idera.SQLdm.Common.Events.MetricCategory? metriccategory = null;
            //IList<Common.Objects.MonitoredSqlServer> MonitoresSqlServerList = RepositoryHelper.GetMonitoredSqlServers(RestServiceConfiguration.SQLConnectInfo, false, null);
            IDictionary<int, string> instanceIdToInstanceName = RepositoryHelper.GetInstanceIdToName(RestServiceConfiguration.SQLConnectInfo);

            IDictionary<string, int> instanceNameToInstanceId = new Dictionary<string, int>();
            foreach (int id in instanceIdToInstanceName.Keys)
            {
                string instanceName;
                instanceIdToInstanceName.TryGetValue(id, out instanceName);
                instanceNameToInstanceId.Add(instanceName, id);
            }

            int passedInstanceId = 0;
            if (string.IsNullOrEmpty(instanceId))
            {               
                instanceList = new string [instanceIdToInstanceName.Count];
                int i = 0;
                foreach (int id in instanceIdToInstanceName.Keys)
                {
                    string instanceName;
                    instanceIdToInstanceName.TryGetValue(id, out instanceName);
                    instanceList [i++] = instanceName;
                }
            }
            else if (int.TryParse(instanceId, out passedInstanceId))
            {
                string instanceName;
                instanceIdToInstanceName.TryGetValue(passedInstanceId, out instanceName);
                instanceList = new string[] { instanceName };
            }
            else
            {
                throw new FaultException("InstanceId must be int");
            }

            //Load CategoryValue
             if(!string.IsNullOrEmpty(category)) {                  
                 metriccategory = (Idera.SQLdm.Common.Events.MetricCategory)Enum.Parse(typeof(Idera.SQLdm.Common.Events.MetricCategory), category); 
             }
            
            // Call Repo
            var AlertList = RepositoryHelper.GetAlerts(RestServiceConfiguration.SQLConnectInfo, instanceList, null, startingAlertId,maxRows,startDate, endDate,severity,metricid, metriccategory, isActive);
                        
            // load Alert.sqlserverId from MonitoresSqlServerList
            if (string.IsNullOrEmpty(instanceId))
            {
                foreach (var alert in AlertList)
                {
                    int instanceIdFromMap;
                    instanceNameToInstanceId.TryGetValue(alert.ServerName, out instanceIdFromMap);
                    alert.SQLServerId = instanceIdFromMap.ToString();//MonitoresSqlServerList.Where(ss => ss.InstanceName.Equals(alert.ServerName, StringComparison.OrdinalIgnoreCase)).Select(ss => ss.Id).FirstOrDefault().ToString();
                }
            }
            else
            {
                foreach (var alert in AlertList)
                {
                    alert.SQLServerId = passedInstanceId.ToString();
                }
            }

            //return (new AlertCollection(AlertList));            
            return AlertList;
        }
*/
        private static int? parseNullableInt(string input) {
            if (!string.IsNullOrEmpty(input))
            {
                int output;
                if (int.TryParse(input, out output))
                {
                    return output;
                }
            }
            return null;
        }

        private IList<Alert> GetAlertsForWebConsole(string timeZoneOffset, DateTime startDate, DateTime endDate, string instanceId, string severity,
            string metric, string category, string orderBy, string orderType, int limit, bool activeOnly, bool abridged = false, int instanceLogicalOperator = 1, int metricLogicalOperator = 1, int severityLogicalOperator = 1)
        {
            //SetConnectionCredentiaslFromCWFHost();
            
            // Call Repo
            //Sanjali Makkar(10.0): Commenting out the following logic as startDate and endDate are taken care of in the ConvertToUTC method, defined in DateTimeHelper Class
            /*DateTime? start = null;
            if (!startDate.Equals(default(DateTime)))
            {
                start = startDate;
            }

            DateTime? end = null;
            if (!endDate.Equals(default(DateTime)))
            {
                end = endDate;
            }*/

            //int? instanceIdInt = parseNullableInt(instanceId);
            //int? severityInt = parseNullableInt(severity);
            //int? metricInt = parseNullableInt(metric);
            
            //START SQLdm 10.0 (Sanjali Makkar): To modify startDate and endDate fields 
            DateTime startDateTimeInUTC = DateTimeHelper.ConvertToUTC(startDate, timeZoneOffset);
            DateTime endDateTimeInUTC = DateTimeHelper.ConvertToUTC(endDate, timeZoneOffset);

            ConfigureDateTime(ref startDateTimeInUTC, ref endDateTimeInUTC);
            //END SQLdm 10.0 (Sanjali Makkar): To modify startDate and endDate fields 

            // var AlertList = RepositoryHelper.GetAlertsForWebConsole(RestServiceConfiguration.SQLConnectInfo, timeZoneOffset,
            //    startDateTimeInUTC,  endDateTimeInUTC, instanceIdInt, severityInt, metricInt, category, orderBy, orderType, limit, activeOnly, abridged,userToken);
            //return (new AlertCollection(AlertList));            

            var AlertList = RepositoryHelper.GetAlertsForWebConsole(RestServiceConfiguration.SQLConnectInfo, timeZoneOffset,
                startDateTimeInUTC, endDateTimeInUTC, instanceId, severity, metric, category, orderBy, orderType, limit, activeOnly, abridged, userToken, instanceLogicalOperator, metricLogicalOperator, severityLogicalOperator);


            return AlertList;
        }


        public IList<Alert> GetAlertsForWebConsole(string timeZoneOffset, DateTime startDate, DateTime endDate, string instanceId, string severity,
            string metric, string category, string orderBy, string orderType, int limit, bool activeOnly)
        {
            SetConnectionCredentiaslFromCWFHost();

            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (instanceId!= null && !ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(instanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            
            //END

            return GetAlertsForWebConsole(timeZoneOffset, startDate, endDate, instanceId, severity,
                metric, category, orderBy, orderType, limit, activeOnly, false);
        }


        public AlertsV2 GetAlertsForWebConsoleGrid(string timeZoneOffset, DateTime startDate, DateTime endDate, string instanceId, string severity,
            string metric, string category, string orderBy, string orderType, int limit, bool activeOnly, int gPage, int gLimit, string advancedFilterParam, string sort)
        {
            LOG.DebugFormat("Parameters for Alerts Grid are {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}", timeZoneOffset, startDate, endDate, instanceId, severity, metric, category, orderBy, orderType, limit, activeOnly, advancedFilterParam, sort);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            SetConnectionCredentiaslFromCWFHost();
            activeOnly = true;
            //Logical Operator 1= Contains 2= Doesnot contains
            int metricLogicalOperator = 1; 
            int instancecLogicalOperator = 1;
            int severityLogicalOperator = 1;
            int sqldmProductLogicalOperator = 1;
            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (instanceId != null && !ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(instanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }

            //SQLdm CWF Integration - Start Sort Parameter
            if (null != sort)
            {
                ExtSortParams sortOptions = JsonHelper.FromJSON<ExtSortParams>(sort);
                ExtSortParam sortProp = sortOptions.First();
                LOG.DebugFormat("SortOptions {0} {1}", sortProp.property, sortProp.direction);
                switch (sortProp.property)
                {
                    case "Instance":
                        orderBy = "ServerName";
                        break;
                    case "SQLdm Repository":
                        orderBy = "DatabaseName";
                        break;
                    case "Category":
                        orderBy = "Category";
                        break;
                    case "Severity":
                        orderBy = "Severity";
                        break;
                    case "Metric":
                        orderBy = "Metric";
                        break;
                    default:
                        orderBy = "UTCOccurrenceDateTime";
                        break;
                }
                orderType = sortProp.direction;

            }
            if (null == orderBy)
            {
                orderBy = "UTCOccurrenceDateTime";
            }
            //SQLdm CWF Integration - End Sort Parameter            

            List<string> multipleServers = null;
            List<string> multipleInstances = null;
            List<string> multipleSeverity = null;
            List<string> multipleMetrics = null;

            AlertsGridAdvancedFilterParams advancedFilterParams = JsonHelper.FromJSON<AlertsGridAdvancedFilterParams>(advancedFilterParam);
            if (advancedFilterParams != null)
            {
                foreach (AlertsGridAdvancedFilterParam param in advancedFilterParams)
                {
                    if (param.property == "advanceFilter" && param.value.Count > 0)
                    {
                        foreach (AlertsGridAdvancedFilterParamFilter filter in param.value)
                        {
                            if (filter.property == "instance" && filter.value != null)
                            {
                                instanceId = System.String.Join(",", filter.value);
                                multipleServers = filter.value;
                                if ("notContains".Equals(filter.op))
                                {
                                    instancecLogicalOperator = 2;
                                }
                            }
                            else if (filter.property == "sqldm" && filter.value != null)
                            {
                                multipleInstances = filter.value;
                                if ("notContains".Equals(filter.op))
                                {
                                    sqldmProductLogicalOperator = 2;
                                }

                            }
                            else if (filter.property == "severity" && filter.value != null)
                            {
                                severity = System.String.Join(",", filter.value);
                                multipleSeverity = filter.value;
                                if ("notContains".Equals(filter.op))
                                {
                                    severityLogicalOperator = 2;
                                }
                            }
                            else if (filter.property == "metric" && filter.value != null)
                            {
                                metric = System.String.Join(",", filter.value);
                                multipleMetrics = filter.value;
                                if ("notContains".Equals(filter.op))
                                {
                                    metricLogicalOperator = 2;
                                }
                            }
                            else if (filter.property == "span" && filter.value != null && filter.op == "between")
                            {
                                activeOnly = false;
                                String sDate = filter.value[0];
                                String sTime = filter.value[1];
                                String eDate = filter.value[2];
                                String eTime = filter.value[3];
                                DateTime startD = DateTime.Parse(sDate);
                                DateTime endD = DateTime.Parse(eDate);
                                DateTime startT = DateTime.Parse(sTime);
                                DateTime endT = DateTime.Parse(eTime);
                                LogX.ErrorFormat("Date and Time are {0}, {1}, {2}, {3}", startD, endD, startT, endT);
                                startD.AddHours(startT.Hour);
                                startD.AddMinutes(startT.Minute);
                                startD.AddSeconds(startT.Second);
                                endD.AddHours(endT.Hour);
                                endD.AddMinutes(endT.Minute);
                                endD.AddSeconds(endT.Second);
                                startDate = startD;
                                endDate = endD;
                            }
                        }
                    }
                }
            }
            if(sqldmProductLogicalOperator==2 && multipleInstances!=null && multipleInstances.Capacity != 0)
            {
                PluginCommon.Products allProducts = CWFHelper.GetProductInstances(null);
                List<String> multipleInstances1 = new List<string>();
                foreach(PluginCommon.Product prd in allProducts) {
                    if (!multipleInstances.Contains(prd.InstanceName))
                    {
                        multipleInstances1.Add(prd.InstanceName);
                        multipleInstances = multipleInstances1;
                    }
                }
            }
            IList<Alert> alerts = new List<Alert>();
            string currentInstanceName = RegistryHelper.GetValueFromRegistry("DisplayName").ToString();
            if (null == multipleInstances || (multipleInstances.Capacity == 1 && multipleInstances.First().ToString().Equals(currentInstanceName)))
            {
                alerts = GetAlertsForWebConsole(timeZoneOffset, startDate, endDate, instanceId, severity,
                    metric, category, orderBy, orderType, limit, activeOnly, false, instancecLogicalOperator,metricLogicalOperator,severityLogicalOperator);
            }
            else if (multipleInstances.Capacity != 0)
            {
                IList<Alert> alerts1 = new List<Alert>();
                foreach (string instanceName in multipleInstances)
                {
                    IList <Alert> alertsOfCurrentInstance = new List<Alert>();
                    if (instanceName.Equals(currentInstanceName))
                    {
                        alertsOfCurrentInstance = GetAlertsForWebConsole(timeZoneOffset, startDate, endDate, instanceId, severity,
                    metric, category, orderBy, orderType, limit, activeOnly, false,instancecLogicalOperator,metricLogicalOperator,severityLogicalOperator);
                    }
                    else
                    {
                        PluginCommon.Products products = CWFHelper.GetProductInstances(instanceName);
                        var userAuthHeader = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
                        string alertsParam = "/" + timeZoneOffset + "?startTime=" + startDate + "&endTime=" + endDate + "&activeOnly=" + activeOnly;
                        if (null != products && products.Capacity != 0)
                        {
                            PluginCommon.Product product = products.First();
                            string restURL = product.RestURL;
                            if (product.IsSelfHosted)
                            {
                                LogX.DebugFormat("Product is SelfHosted and calling {0} to get Alerts", restURL + "/AlertsForWebConsole/tzo"+ alertsParam);
                                alertsOfCurrentInstance = HttpRequestHelper.Get<IList<Alert>>(restURL + "/AlertsForWebConsole/tzo"+ alertsParam, userAuthHeader);
                            }
                            else
                            {
                                restURL = CWFHelper.GetProductNonSelfhostedAPIEndPoint("/" + restURL);
                                LogX.DebugFormat("Product is Not SelfHosted and calling {0} to get Alerts", restURL + "/AlertsForWebConsole/tzo"+ alertsParam);
                                alertsOfCurrentInstance = HttpRequestHelper.Get<IList<Alert>>(restURL + "/AlertsForWebConsole/tzo"+ alertsParam, userAuthHeader);
                            }
                        }
                    }
                    ((List<Alert>)alerts1).AddRange(alertsOfCurrentInstance);
                }
                foreach (Alert a in alerts1)
                {
                    bool instanceCondition = true;
                    bool metricsCondition = true;
                    bool severityCondition = true;
                    if (null != multipleServers)
                    {
                        bool isServerContained = multipleInstances.Contains(a.SQLServerId + "");
                        instanceCondition = instancecLogicalOperator == 2 ? !isServerContained: isServerContained;
                    }
                    if (null != multipleMetrics)
                    {
                        bool isMetricContained = multipleMetrics.Contains(a.Metric + "");
                        metricsCondition = metricLogicalOperator == 2 ? !isMetricContained : isMetricContained;

                    }
                    if (null != multipleSeverity)
                    {
                        bool isSeverityContained = multipleSeverity.Contains(a.Severity + "");
                        severityCondition = severityLogicalOperator == 2 ? !isSeverityContained : isSeverityContained;
                    }
                    if (instanceCondition && metricsCondition && severityCondition)
                    {
                        alerts.Add(a);
                    }

                    switch (orderBy)
                    {
                        case "ServerName":
                            if (null != orderType && "asc".Equals(orderType.ToLower()))
                            {
                                alerts = alerts.OrderBy(o => o.ServerName).ToList();
                            }
                            else
                            {
                                alerts = alerts.OrderByDescending(o => o.ServerName).ToList();

                            }
                            break;
                        case "DatabaseName":
                            if (null != orderType && "asc".Equals(orderType.ToLower()))
                            {
                                alerts = alerts.OrderBy(o => o.ServerName).ToList();
                            }
                            else
                            {
                                alerts = alerts.OrderByDescending(o => o.ServerName).ToList();

                            }
                            break;
                        case "Category":
                            if (null != orderType && "asc".Equals(orderType.ToLower()))
                            {
                                alerts = alerts.OrderBy(o => o.ServerName).ToList();
                            }
                            else
                            {
                                alerts = alerts.OrderByDescending(o => o.ServerName).ToList();

                            }
                            break;
                        case "Severity":
                            if (null != orderType && "asc".Equals(orderType.ToLower()))
                            {
                                alerts = alerts.OrderBy(o => o.ServerName).ToList();
                            }
                            else
                            {
                                alerts = alerts.OrderByDescending(o => o.ServerName).ToList();

                            }
                            break;
                        case "Metric":
                            if (null != orderType && "asc".Equals(orderType.ToLower()))
                            {
                                alerts = alerts.OrderBy(o => o.ServerName).ToList();
                            }
                            else
                            {
                                alerts = alerts.OrderByDescending(o => o.ServerName).ToList();

                            }
                            break;
                        default:
                            if (null != orderType && "asc".Equals(orderType.ToLower()))
                            {
                                alerts = alerts.OrderBy(o => o.ServerName).ToList();
                            }
                            else
                            {
                                alerts = alerts.OrderByDescending(o => o.ServerName).ToList();

                            }
                            break;
                    }

                }
            }
            AlertsV2 alertsV2 = new AlertsV2();
            if (null != alerts)
            {
                alertsV2.totalAlerts = alerts.Count;
                if (gLimit == 0)
                {
                    alertsV2.Alerts = GetGridAlerts(alerts);
                }
                else
                {
                    alertsV2.Alerts = GetGridAlerts(alerts).Skip((gPage - 1) * gLimit).Take(gLimit).ToList();
                }
            }
            LOG.DebugFormat("Time taken to get the Alerts For WebConsole is {0}.", timer.Elapsed);
            timer.Stop();
           
            return alertsV2;
        }

        public IList<GridAlert> GetGridAlerts(IList<Alert> alerts)
        {
            IList<GridAlert> gridAlerts = new List<GridAlert>();
            foreach(Alert alert in alerts)
            {
                GridAlert gridAlert = new GridAlert();
                gridAlert.AlertId = alert.AlertId;
                gridAlert.DatabaseName = alert.DatabaseName;
                gridAlert.Heading = alert.Heading;
                gridAlert.IsActive = alert.IsActive;
                gridAlert.Message = alert.Message;
                gridAlert.Metric = alert.Metric;
				gridAlert.PreviousAlertSeverity = alert.PreviousAlertSeverity;
                gridAlert.ServerName = alert.ServerName;
                switch (alert.Severity)
                {
                    case 1:
                        gridAlert.Severity = "Ok";
                        break;
                    case 2:
                        gridAlert.Severity = "Informational";
                        break;
                    case 4:
                        gridAlert.Severity = "Warning";
                        break;
                    case 8:
                        gridAlert.Severity = "Critical";
                        break;
                    default:
                        gridAlert.Severity = "Ok";
                        break;
                }
                gridAlert.SQLServerId = alert.SQLServerId;
                gridAlert.StateEvent = alert.StateEvent;
                gridAlert.StringValue = alert.StringValue;
                gridAlert.UTCOccurrenceDateTime = alert.UTCOccurrenceDateTime;
                gridAlert.Value = alert.Value;

                gridAlerts.Add(gridAlert);

            }
            return gridAlerts;
        }

        public IList<Alert> GetAlertsForWebConsoleAbridged(string timeZoneOffset, DateTime startDate, DateTime endDate, string instanceId, string severity,
            string metric, string category, string orderBy, string orderType, int limit, bool activeOnly)
        {
            SetConnectionCredentiaslFromCWFHost();

            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (instanceId != null && !ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(instanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            //END

            return GetAlertsForWebConsole(timeZoneOffset, startDate, endDate, instanceId, severity,
                metric, category, orderBy, orderType, limit, activeOnly, true);
        }

        public Alert GetAlertDetails(string alertId, string timeZoneOffset)
        {
            SetConnectionCredentiaslFromCWFHost();
            int passedAlertId;
            if (int.TryParse(alertId, out passedAlertId))
            {
                //SQLdm 10.0 (Swati Gogia):Passed userToken to GetAlertDetails to Implement Instance level security
                return RepositoryHelper.GetAlertDetails(RestServiceConfiguration.SQLConnectInfo, passedAlertId, timeZoneOffset, userToken);
            }
            else
            {
                return null;
            }
        }

        public IList<TimedValue> GetMetricsHistoryForAlert(string alertId, string timeZoneOffset, int numHistoryHours)
        {
            SetConnectionCredentiaslFromCWFHost();
            int passedAlertId;

            if (int.TryParse(alertId, out passedAlertId))
            {
                //SQLdm 10.0 (Swati Gogia):Passed userToken to GetMetricsHistoryForAlert to Implement Instance level security
                return RepositoryHelper.GetMetricsHistoryForAlert(RestServiceConfiguration.SQLConnectInfo, passedAlertId, timeZoneOffset, numHistoryHours,userToken);
            }
            else
            {
                return null;
            }
        }

        public MetricCollection GetMetricsDetails(string timeZoneOffset, string metricId, int maxRows, DateTime startDate, DateTime endDate)
        {
            SetConnectionCredentiaslFromCWFHost();
            //START - SQLdm 10.0 Gaurav Karwal: commenting this code as dates are not being used
            ////START SQLdm 10.0 (Sanjali Makkar): To modify startDate and endDate fields 
            //DateTime startDateTimeInUTC = DateTimeHelper.ConvertToUTC(startDate, timeZoneOffset);
            //DateTime endDateTimeInUTC = DateTimeHelper.ConvertToUTC(endDate, timeZoneOffset);

            //ConfigureDateTime(ref startDateTimeInUTC, ref endDateTimeInUTC);
            ////END SQLdm 10.0 (Sanjali Makkar): To modify startDate and endDate fields 

            //END - SQLdm 10.0 Gaurav Karwal: commenting this code as dates are not being used
            var MetricsList = RepositoryHelper.GetMetrics(RestServiceConfiguration.SQLConnectInfo, metricId, maxRows);
            return (new MetricCollection(MetricsList));            
        }

        public IList<AlertsByCategory> GetAlertsCountByCategory(bool PerInstance)
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetAlertsCountByCategory to Implement Instance level security
            return RepositoryHelper.GetAlertsCountByCategory(RestServiceConfiguration.SQLConnectInfo, userToken, PerInstance);
        }

        public IList<AlertsByDatabase> GetAlertsCountByDatabase ()
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetAlertsCountByDatabase to Implement Instance level security
            return RepositoryHelper.GetAlertsCountByDatabase(RestServiceConfiguration.SQLConnectInfo, userToken);
        }

        //START SQLdm 10.0 (Sanjali Makkar): To modify startDate and endDate fields according to whether they are null or not
        private static void ConfigureDateTime(ref DateTime startDateTimeInUTC, ref DateTime endDateTimeInUTC)
        {
            //case 1: StartTime != null and EndTime == null
            if (!startDateTimeInUTC.Equals(DateTime.MinValue) && endDateTimeInUTC.Equals(DateTime.MinValue))
                endDateTimeInUTC = DateTime.UtcNow;

            //case 2: StartTime == null and EndTime != null
            if (startDateTimeInUTC.Equals(DateTime.MinValue) && !endDateTimeInUTC.Equals(DateTime.MinValue))
                startDateTimeInUTC = (DateTime)SqlDateTime.MinValue; //done so as to avoid 'SQLDateTime Overflow' exception as C# DateTime.MinValue is 1/1/0001 whereas SqlDateTime.MinValue is 1/1/1753

            //case 3: StartTime == null and EndTime == null
            if (startDateTimeInUTC.Equals(DateTime.MinValue) && endDateTimeInUTC.Equals(DateTime.MinValue))
            {
                startDateTimeInUTC = (DateTime)SqlDateTime.MinValue; //done so as to avoid 'SQLDateTime Overflow' exception as C# DateTime.MinValue is 1/1/0001 whereas SqlDateTime.MinValue is 1/1/1753
                endDateTimeInUTC = DateTime.UtcNow;
            }
            
            //case 4: StartTime != null and EndTime != null
            //Nothing to do
         }

        public List<AdvanceFilter> GetAlertsAdvanceFilters()
        {
            SetConnectionCredentiaslFromCWFHost();
            return RepositoryHelper.GetAlertsAdvanceFilters(RestServiceConfiguration.SQLConnectInfo);
        }

        public void AddAlertsAdvanceFilters(string filterName, string filterConfig)
        {
            SetConnectionCredentiaslFromCWFHost();
            RepositoryHelper.AddAlertAdvanceFilter(RestServiceConfiguration.SQLConnectInfo, filterName, filterConfig);
        }

        public void DeleteAlertsAdvanceFilters(string filterName)
        {
            SetConnectionCredentiaslFromCWFHost();
            RepositoryHelper.DeleteAlertAdvanceFilter(RestServiceConfiguration.SQLConnectInfo, filterName);
        }
        //END SQLdm 10.0 (Sanjali Makkar): To modify startDate and endDate fields according to whether they are null or not

        #endregion
    }
}
