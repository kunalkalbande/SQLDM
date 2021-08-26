using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using Idera.SQLdm.Service.ServiceContracts.v1;
using Idera.SQLdm.Service.Repository;
using Idera.SQLdm.Service.Core.Enums;
using Idera.SQLdm.Service.Helpers;
using Idera.SQLdm.Service.DataContracts.v1;
using System.ServiceModel;
using Idera.SQLdm.Service.Configuration;
using System.ServiceModel.Web;
using Idera.SQLdm.Service.Helpers.Auth;

namespace Idera.SQLdm.Service.Web
{
    public partial class WebService : IQueryManager
    {

        #region IQueryManager Members

        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - service method to return a list of supported groupings (Group By) in the WebUI console
        public IList<SupportedGrouping> GetSupportedGroupingsForQueries()
        {
            SetConnectionCredentiaslFromCWFHost();
            List<SupportedGrouping> supportedGroupList = new List<SupportedGrouping>();
            Dictionary<string,string> enumToGroupName  = new Dictionary<string,string>();

            enumToGroupName.Add(((GroupType)GroupType.Application).ToString(), RepositoryHelper.GroupByApplication);
            enumToGroupName.Add(((GroupType)GroupType.Database).ToString(), RepositoryHelper.GroupByDatabase);
            enumToGroupName.Add(((GroupType)GroupType.User).ToString(), RepositoryHelper.GroupByUser);
            enumToGroupName.Add(((GroupType)GroupType.Client).ToString(), RepositoryHelper.GroupByClient);
            enumToGroupName.Add(((GroupType)GroupType.QuerySignature).ToString(), RepositoryHelper.GroupByQuerySignature);
            enumToGroupName.Add(((GroupType)GroupType.QueryStatement).ToString(), RepositoryHelper.GroupByQueryStatement);

            supportedGroupList.Add(new SupportedGrouping((int)GroupType.Application, enumToGroupName[((GroupType)GroupType.Application).ToString()]));
            supportedGroupList.Add(new SupportedGrouping((int)GroupType.Database, enumToGroupName[((GroupType)GroupType.Database).ToString()]));
            supportedGroupList.Add(new SupportedGrouping((int)GroupType.User, enumToGroupName[((GroupType)GroupType.User).ToString()]));
            supportedGroupList.Add(new SupportedGrouping((int)GroupType.Client, enumToGroupName[((GroupType)GroupType.Client).ToString()]));
            supportedGroupList.Add(new SupportedGrouping((int)GroupType.QuerySignature, enumToGroupName[((GroupType)GroupType.QuerySignature).ToString()]));
            supportedGroupList.Add(new SupportedGrouping((int)GroupType.QueryStatement, enumToGroupName[((GroupType)GroupType.QueryStatement).ToString()])); 

            return supportedGroupList;
        }


        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - service method to return a list of supported metrics (View) in the WebUI console
        public IList<SupportedMetric> GetSupportedMetricsForQueries()
        {
            SetConnectionCredentiaslFromCWFHost();
            List<SupportedMetric> supportedMetricList = new List<SupportedMetric>();
            Dictionary<string, string> enumToMetricName = new Dictionary<string, string>();

            enumToMetricName.Add(((MetricType)MetricType.DurationInMilliSeconds).ToString(), RepositoryHelper.DurationView);
            enumToMetricName.Add(((MetricType)MetricType.CPUTimeInMilliSeconds).ToString(), RepositoryHelper.CPUTimeView);
            enumToMetricName.Add(((MetricType)MetricType.Reads).ToString(), RepositoryHelper.ReadsView);
            enumToMetricName.Add(((MetricType)MetricType.Writes).ToString(), RepositoryHelper.WritesView);
            enumToMetricName.Add(((MetricType)MetricType.InputOutput).ToString(), RepositoryHelper.InputOutputView);
            enumToMetricName.Add(((MetricType)MetricType.BlockingDurationInMilliSeconds).ToString(), RepositoryHelper.BlockingDurationView);
            enumToMetricName.Add(((MetricType)MetricType.WaitDurationInMilliSeconds).ToString(), RepositoryHelper.WaitDurationView);
            enumToMetricName.Add(((MetricType)MetricType.Deadlocks).ToString(), RepositoryHelper.DeadlocksView);

            supportedMetricList.Add(new SupportedMetric((int)MetricType.DurationInMilliSeconds, enumToMetricName[((MetricType)MetricType.DurationInMilliSeconds).ToString()]));
            supportedMetricList.Add(new SupportedMetric((int)MetricType.CPUTimeInMilliSeconds, enumToMetricName[((MetricType)MetricType.CPUTimeInMilliSeconds).ToString()]));
            supportedMetricList.Add(new SupportedMetric((int)MetricType.Reads, enumToMetricName[((MetricType)MetricType.Reads).ToString()]));
            supportedMetricList.Add(new SupportedMetric((int)MetricType.Writes, enumToMetricName[((MetricType)MetricType.Writes).ToString()]));
            supportedMetricList.Add(new SupportedMetric((int)MetricType.InputOutput, enumToMetricName[((MetricType)MetricType.InputOutput).ToString()]));
            supportedMetricList.Add(new SupportedMetric((int)MetricType.BlockingDurationInMilliSeconds, enumToMetricName[((MetricType)MetricType.BlockingDurationInMilliSeconds).ToString()]));
            supportedMetricList.Add(new SupportedMetric((int)MetricType.WaitDurationInMilliSeconds, enumToMetricName[((MetricType)MetricType.WaitDurationInMilliSeconds).ToString()]));
            supportedMetricList.Add(new SupportedMetric((int)MetricType.Deadlocks, enumToMetricName[((MetricType)MetricType.Deadlocks).ToString()]));
            
            return supportedMetricList;
        }

        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - service method to return a list of applications for a particular instance based on the parameters 'startIndex' and 'recordsCount'
        public IList<Application> GetApplicationsForServer(string instanceId, string startIndex, string recordsCount)
        {
            SetConnectionCredentiaslFromCWFHost();
            List<Application> getApplicaionsList = null;
            bool isStartIndexValid = true;

            int sqlServerID, recordStartIndex = 1, noOfRecords = -1;   // noOfRecords = -1 indicates all records

            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (instanceId != null && !ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(instanceId),userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);

            }
            //END

            if (String.IsNullOrWhiteSpace(startIndex))   // set the 'recordStartIndex' to 0 so that 'isStartIndexValid' variable can be reset later in the if condition
                recordStartIndex = 0;

            if (!(String.IsNullOrWhiteSpace(startIndex)))   // parse and set the parameters if 'startIndex' is not null or empty whitespaces
            {
                int.TryParse(startIndex, out recordStartIndex);
                noOfRecords = RepositoryHelper.DefaultRecordsCount;
            }
            if (!(String.IsNullOrWhiteSpace(recordsCount)))  // change 'noOfRecords' if 'recordsCount' is not null or empty whitespaces
            {
                int.TryParse(recordsCount, out noOfRecords);
            }
            if (recordStartIndex < 1)   // handle the case when 'startIndex' is parsable, but the value is off limits
            {
                recordStartIndex = 1;
                isStartIndexValid = false;
            }
            if (noOfRecords < 1)    // handle the case when 'recordsCount' is parsable, but the value is off limits
            {
                if (isStartIndexValid)          // handle the case when 'startIndex' is a valid integer but 'recordsCount' is invalid
                    noOfRecords = RepositoryHelper.DefaultRecordsCount;
                else
                    noOfRecords = -1;           // print all records, when both 'startIndex' and 'recordsCount' are invalid
            }

            if (int.TryParse(instanceId, out sqlServerID)) 
            {
                getApplicaionsList = RepositoryHelper.GetApplicationsForInstance(RestServiceConfiguration.SQLConnectInfo, sqlServerID, recordStartIndex, noOfRecords);
            }   

            return getApplicaionsList;
        }


        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - service method to return a list of clients for a particular instance based on the parameters 'startIndex' and 'recordsCount'
        public IList<Client> GetClientsForServer(string instanceId, string startIndex, string recordsCount)
        {
            SetConnectionCredentiaslFromCWFHost();
            List<Client> getClientsList = null;
            bool isStartIndexValid = true;

            int sqlServerID, recordStartIndex = 1, noOfRecords = -1;   // noOfRecords = -1 indicates all records

            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (instanceId != null && !ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(instanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);

            }
            //END

            if (String.IsNullOrWhiteSpace(startIndex))   // set the 'recordStartIndex' to 0 so that 'isStartIndexValid' variable can be reset later in the if condition
                recordStartIndex = 0;

            if (!(String.IsNullOrWhiteSpace(startIndex)))   // parse and set the parameters if 'startIndex' is not null or empty whitespaces
            {
                int.TryParse(startIndex, out recordStartIndex);
                noOfRecords = RepositoryHelper.DefaultRecordsCount;
            }
            if (!(String.IsNullOrWhiteSpace(recordsCount)))  // change 'noOfRecords' if 'recordsCount' is not null or empty whitespaces
            {
                int.TryParse(recordsCount, out noOfRecords);
            }
            if (recordStartIndex < 1)   // handle the case when 'startIndex' is parsable, but the value is off limits
            {
                recordStartIndex = 1;
                isStartIndexValid = false;
            }
            if (noOfRecords < 1)    // handle the case when 'recordsCount' is parsable, but the value is off limits
            {
                if (isStartIndexValid)          // handle the case when 'startIndex' is a valid integer but 'recordsCount' is invalid
                    noOfRecords = RepositoryHelper.DefaultRecordsCount;
                else
                    noOfRecords = -1;           // print all records, when both 'startIndex' and 'recordsCount' are invalid
            }

            if (int.TryParse(instanceId, out sqlServerID))
            {
                getClientsList = RepositoryHelper.GetClientsForInstance(RestServiceConfiguration.SQLConnectInfo, sqlServerID, recordStartIndex, noOfRecords);
            }

            return getClientsList;
        }


        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - service method to return a list of users for a particular instance based on the parameters 'startIndex' and 'recordsCount'
        public IList<User> GetUsersForServer(string instanceId, string startIndex, string recordsCount)
        {
            SetConnectionCredentiaslFromCWFHost();
            List<User> getUsersList = null;
            bool isStartIndexValid = true;

            int sqlServerID, recordStartIndex = 1, noOfRecords = -1;   // noOfRecords = -1 indicates all records

            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (instanceId != null && !ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(instanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);

            }
            //END

            if (String.IsNullOrWhiteSpace(startIndex))   // set the 'recordStartIndex' to 0 so that 'isStartIndexValid' variable can be reset later in the if condition
                recordStartIndex = 0;

            if (!(String.IsNullOrWhiteSpace(startIndex)))   // parse and set the parameters if 'startIndex' is not null or empty whitespaces
            {
                int.TryParse(startIndex, out recordStartIndex);
                noOfRecords = RepositoryHelper.DefaultRecordsCount;
            }
            if (!(String.IsNullOrWhiteSpace(recordsCount)))  // change 'noOfRecords' if 'recordsCount' is not null or empty whitespaces
            {
                int.TryParse(recordsCount, out noOfRecords);
            }
            if (recordStartIndex < 1)   // handle the case when 'startIndex' is parsable, but the value is off limits
            {
                recordStartIndex = 1;
                isStartIndexValid = false;
            }
            if (noOfRecords < 1)    // handle the case when 'recordsCount' is parsable, but the value is off limits
            {
                if (isStartIndexValid)          // handle the case when 'startIndex' is a valid integer but 'recordsCount' is invalid
                    noOfRecords = RepositoryHelper.DefaultRecordsCount;
                else
                    noOfRecords = -1;           // print all records, when both 'startIndex' and 'recordsCount' are invalid
            }

            if (int.TryParse(instanceId, out sqlServerID))
            {
                getUsersList = RepositoryHelper.GetUsersForInstance(RestServiceConfiguration.SQLConnectInfo, sqlServerID, recordStartIndex, noOfRecords);
            }

            return getUsersList;
        }


        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - service method to return a list of databases for a particular instance based on the parameters 'startIndex' and 'recordsCount'
        public IList<DatabaseInformation> GetDatabasesForServer(string instanceId, string startIndex, string recordsCount)
        {
            SetConnectionCredentiaslFromCWFHost();
            List<DatabaseInformation> getDatabasesList = null;
            bool isStartIndexValid = true;

            int sqlServerID, recordStartIndex = 1, noOfRecords = -1;   // noOfRecords = -1 indicates all records

            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (instanceId != null && !ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(instanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);

            }
            //END

            if (String.IsNullOrWhiteSpace(startIndex))   // set the 'recordStartIndex' to 0 so that 'isStartIndexValid' variable can be reset later in the if condition
                recordStartIndex = 0;

            if (!(String.IsNullOrWhiteSpace(startIndex)))   // parse and set the parameters if 'startIndex' is not null or empty whitespaces
            {
                int.TryParse(startIndex, out recordStartIndex);
                noOfRecords = RepositoryHelper.DefaultRecordsCount;
            }
            if (!(String.IsNullOrWhiteSpace(recordsCount)))  // change 'noOfRecords' if 'recordsCount' is not null or empty whitespaces
            {
                int.TryParse(recordsCount, out noOfRecords);
            }
            if (recordStartIndex < 1)   // handle the case when 'startIndex' is parsable, but the value is off limits
            {
                recordStartIndex = 1;
                isStartIndexValid = false;
            }
            if (noOfRecords < 1)    // handle the case when 'recordsCount' is parsable, but the value is off limits
            {
                if (isStartIndexValid)          // handle the case when 'startIndex' is a valid integer but 'recordsCount' is invalid
                    noOfRecords = RepositoryHelper.DefaultRecordsCount;
                else
                    noOfRecords = -1;           // print all records, when both 'startIndex' and 'recordsCount' are invalid
            }

            if (int.TryParse(instanceId, out sqlServerID))
            {
                getDatabasesList = RepositoryHelper.GetDatabasesForInstance(RestServiceConfiguration.SQLConnectInfo, sqlServerID, recordStartIndex, noOfRecords);
            }
            return getDatabasesList;
        }


        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - service method to return the query plan for a particular QueryStatisticsID
        public IList<QueryPlan> GetQueryPlan(string instanceId, string queryId)
        {
            SetConnectionCredentiaslFromCWFHost();
            List<QueryPlan> getQueryPlanList = null;
            int sqlServerID, queryStatisticsID; //SQLdm 9.0 (Ankit Srivastava) - Query Plan View - changing the variable name to statisticsid

            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (instanceId != null && !ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(instanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);

            }
            //END

            if (int.TryParse(instanceId, out sqlServerID) && int.TryParse(queryId, out queryStatisticsID))//SQLdm 9.0 (Ankit Srivastava) - Query Plan View - changing the variable name to statisticsid
            {
                getQueryPlanList = RepositoryHelper.GetQueryPlans(RestServiceConfiguration.SQLConnectInfo, sqlServerID, queryStatisticsID);//SQLdm 9.0 (Ankit Srivastava) - Query Plan View - changing the variable name to statisticsid
            }
            return getQueryPlanList;
        }

        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - service method to return the data of a given group according to the set filters
        public IList<QueryMonitorStatisticsData> GetQueryMonitorData(string instanceId, string viewId, string groupId, string timeZoneOffset, QueryMonitorFilters queryFilters)
        {
            SetConnectionCredentiaslFromCWFHost();
            List<QueryMonitorStatisticsData> getQueryMonitorDataList = null;
            int sqlServerID, enumGroupID, recordStartIndex, recordsCount;
            float timeOffset;
            DateTime startTimestamp;
            DateTime endTimestamp;
            DateTime? startDate = null;
            DateTime? endDate = null;
            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (instanceId != null && !ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(instanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);

            }
            //END

            if (int.TryParse(instanceId, out sqlServerID) && int.TryParse(groupId, out enumGroupID))
            {
                int sqlSignatureID = 0, statementTypeID = -1, includeSQLStatement = 1, includeSQLProcedure = 1, includeSQLBatch = 1, 
                    includeIncompletedQueries = 0, includeTimeOverlappedQueries = 0;

                int.TryParse(queryFilters.QuerySignatureId, out sqlSignatureID);
                if (int.TryParse(queryFilters.EventTypeId, out statementTypeID))
                {
                    if (statementTypeID < 0 && statementTypeID > 2)
                        statementTypeID = -1;
                }
                else
                    statementTypeID = -1;


                if (!String.IsNullOrWhiteSpace(queryFilters.AdvancedFilters))
                {
                    if (!(int.TryParse(queryFilters.AdvancedFilters[0].ToString(), out includeSQLStatement) && (includeSQLStatement == 0 || includeSQLStatement == 1)))
                        includeSQLStatement = 1;
                    if (!(int.TryParse(queryFilters.AdvancedFilters[1].ToString(), out includeSQLProcedure) && (includeSQLProcedure == 0 || includeSQLProcedure == 1)))
                        includeSQLProcedure = 1;
                    if (!(int.TryParse(queryFilters.AdvancedFilters[2].ToString(), out includeSQLBatch) && (includeSQLBatch == 0 || includeSQLBatch == 1)))
                        includeSQLBatch = 1;
                    if (!(int.TryParse(queryFilters.AdvancedFilters[3].ToString(), out includeTimeOverlappedQueries) && (includeTimeOverlappedQueries == 0 || includeTimeOverlappedQueries == 1)))
                        includeTimeOverlappedQueries = 0;
                    if (!(int.TryParse(queryFilters.AdvancedFilters[4].ToString(), out includeIncompletedQueries) && (includeIncompletedQueries == 0 || includeIncompletedQueries == 1)))
                        includeIncompletedQueries = 0;
                }
                
                string groupBy = ((GroupType)enumGroupID).ToString();

                if (String.IsNullOrWhiteSpace(queryFilters.SQLExclude))                // check for valid values of wildcards of SQL Text
                    queryFilters.SQLExclude = null;
                else if (queryFilters.SQLExclude.Length > 0)
                {
                    if (queryFilters.SQLExclude[0] != '%' && queryFilters.SQLExclude[queryFilters.SQLExclude.Length - 1] != '%')
                        queryFilters.SQLExclude = "%" + queryFilters.SQLExclude + "%";
                }

                if (String.IsNullOrWhiteSpace(queryFilters.SQLInclude))
                    queryFilters.SQLInclude = null;
                else if (queryFilters.SQLInclude.Length > 0)
                {
                    if (queryFilters.SQLInclude[0] != '%' && queryFilters.SQLInclude[queryFilters.SQLInclude.Length - 1] != '%')
                        queryFilters.SQLInclude = "%" + queryFilters.SQLInclude + "%";
                }

                if (String.IsNullOrWhiteSpace(queryFilters.SortBy))                // check for valid values of Sorting parameters
                    queryFilters.SortBy = null;
                if (String.IsNullOrWhiteSpace(queryFilters.SortOrder))
                    queryFilters.SortOrder = null;

                float.TryParse(timeZoneOffset, out timeOffset);
                Math.Round(timeOffset, 2);

                if ((Convert.ToInt32(Math.Abs(timeOffset)) < 0) || (Convert.ToInt32(Math.Abs(timeOffset)) > 23))  // return null, if the timeZoneOffset is not valid
                    return getQueryMonitorDataList;


                if (!DateTime.TryParseExact(queryFilters.StartTimestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTimestamp)) 
                {
                    _logX.Warn("The parsing of start date was unsuccessful" + queryFilters.StartTimestamp);
                };

                if (!DateTime.TryParseExact(queryFilters.EndTimestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out endTimestamp)) 
                {
                    _logX.Warn("The parsing of end date was unsuccessful" + queryFilters.EndTimestamp);
                }

                // handle if timestamp is not valid
                if (!startTimestamp.Equals(default(DateTime)))
                {
                    startDate = startTimestamp.AddHours(timeOffset);
                }

                if (!endTimestamp.Equals(default(DateTime)))
                {
                    endDate = endTimestamp.AddHours(timeOffset);
                }

                int.TryParse(queryFilters.StartIndex, out recordStartIndex);             // send all records if any of these parameter is invalid
                if (recordStartIndex < 1)
                    recordStartIndex = 0;
                int.TryParse(queryFilters.RowCount, out recordsCount);
                if (recordsCount < 1)
                    recordsCount = 0;

                // START: SQLdm 9.0 (Abhishek Joshi) --WebUI Query Monitoring -moving the logic of adding paranthesis around comma separated values in the stored procedure corresponding to a group
                if (!(String.IsNullOrWhiteSpace(queryFilters.ApplicationIds)))                   // change the pipe separated Ids to comma separated for easy checking in stored procedure
                    queryFilters.ApplicationIds = queryFilters.ApplicationIds.Replace('|', ',');
                if (!(String.IsNullOrWhiteSpace(queryFilters.DatabaseIds)))
                    queryFilters.DatabaseIds = queryFilters.DatabaseIds.Replace('|', ',');
                if (!(String.IsNullOrWhiteSpace(queryFilters.ClientIds)))
                    queryFilters.ClientIds = queryFilters.ClientIds.Replace('|', ',');
                if (!(String.IsNullOrWhiteSpace(queryFilters.UserIds)))
                    queryFilters.UserIds = queryFilters.UserIds.Replace('|', ',');
                // END: SQLdm 9.0 (Abhishek Joshi) --WebUI Query Monitoring -moving the logic of adding paranthesis around comma separated values in the stored procedure corresponding to a group

                if (String.IsNullOrWhiteSpace(queryFilters.SortOrder))             // default value for sort order
                    queryFilters.SortOrder = "ASC";

                if (groupBy.Equals("Application"))
                {
                    if (String.IsNullOrWhiteSpace(queryFilters.SortBy))
                        queryFilters.SortBy = "Application";

                    getQueryMonitorDataList = RepositoryHelper.GetQueryMonitorStatisticsByApplication(RestServiceConfiguration.SQLConnectInfo, sqlServerID, timeOffset, queryFilters.ApplicationIds, queryFilters.DatabaseIds, queryFilters.UserIds, queryFilters.ClientIds, queryFilters.SQLInclude, queryFilters.SQLExclude,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startDate, endDate, queryFilters.SortBy, queryFilters.SortOrder, recordStartIndex, recordsCount, sqlSignatureID, statementTypeID);
                }
                else if (groupBy.Equals("Database"))
                {
                    if (String.IsNullOrWhiteSpace(queryFilters.SortBy))
                        queryFilters.SortBy = "DatabaseName";

                    getQueryMonitorDataList = RepositoryHelper.GetQueryMonitorStatisticsByDatabase(RestServiceConfiguration.SQLConnectInfo, sqlServerID, timeOffset, queryFilters.ApplicationIds, queryFilters.DatabaseIds, queryFilters.UserIds, queryFilters.ClientIds, queryFilters.SQLInclude, queryFilters.SQLExclude,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startDate, endDate, queryFilters.SortBy, queryFilters.SortOrder, recordStartIndex, recordsCount, sqlSignatureID, statementTypeID);
                }
                else if (groupBy.Equals("User"))
                {
                    if (String.IsNullOrWhiteSpace(queryFilters.SortBy))
                        queryFilters.SortBy = "UserName";

                    getQueryMonitorDataList = RepositoryHelper.GetQueryMonitorStatisticsByUser(RestServiceConfiguration.SQLConnectInfo, sqlServerID, timeOffset, queryFilters.ApplicationIds, queryFilters.DatabaseIds, queryFilters.UserIds, queryFilters.ClientIds, queryFilters.SQLInclude, queryFilters.SQLExclude,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startDate, endDate, queryFilters.SortBy, queryFilters.SortOrder, recordStartIndex, recordsCount, sqlSignatureID, statementTypeID);
                }
                else if (groupBy.Equals("Client"))
                {
                    if (String.IsNullOrWhiteSpace(queryFilters.SortBy))
                        queryFilters.SortBy = "Client";

                    getQueryMonitorDataList = RepositoryHelper.GetQueryMonitorStatisticsByClient(RestServiceConfiguration.SQLConnectInfo, sqlServerID, timeOffset, queryFilters.ApplicationIds, queryFilters.DatabaseIds, queryFilters.UserIds, queryFilters.ClientIds, queryFilters.SQLInclude, queryFilters.SQLExclude,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startDate, endDate, queryFilters.SortBy, queryFilters.SortOrder, recordStartIndex, recordsCount, sqlSignatureID, statementTypeID);
                }
                else if (groupBy.Equals("QuerySignature"))
                {
                    if (String.IsNullOrWhiteSpace(queryFilters.SortBy))
                        queryFilters.SortBy = "SignatureSQLText";

                    getQueryMonitorDataList = RepositoryHelper.GetQueryMonitorStatisticsByQuerySignature(RestServiceConfiguration.SQLConnectInfo, sqlServerID, timeOffset, queryFilters.ApplicationIds, queryFilters.DatabaseIds, queryFilters.UserIds, queryFilters.ClientIds, queryFilters.SQLInclude, queryFilters.SQLExclude,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startDate, endDate, queryFilters.SortBy, queryFilters.SortOrder, recordStartIndex, recordsCount, sqlSignatureID, statementTypeID);
                }
                else if (groupBy.Equals("QueryStatement")) // SQLdm 9.0 (Abhishek Joshi) --WebUI Query Monitoring -correction to handle any invalid value of groupId
                {
                    if (String.IsNullOrWhiteSpace(queryFilters.SortBy))
                        queryFilters.SortBy = "StatementSQLText";

                    getQueryMonitorDataList = RepositoryHelper.GetQueryMonitorStatisticsByQueryStatement(RestServiceConfiguration.SQLConnectInfo, sqlServerID, timeOffset, queryFilters.ApplicationIds, queryFilters.DatabaseIds, queryFilters.UserIds, queryFilters.ClientIds, queryFilters.SQLInclude, queryFilters.SQLExclude,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startDate, endDate, queryFilters.SortBy, queryFilters.SortOrder, recordStartIndex, recordsCount, sqlSignatureID, statementTypeID);
                }

            }
            return getQueryMonitorDataList;
        }


        // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - service method to return the query monitor data of a given group for graph representation
        public IList<QueryMonitorDataForGraphs> GetQueryMonitorDataForGraphs(string instanceId, string viewId, string groupId, string timeZoneOffset, QueryMonitorFilters queryFilters)
        {
            SetConnectionCredentiaslFromCWFHost();
            List<QueryMonitorDataForGraphs> getQueryMonitorDataList = null;
            int sqlServerID, enumGroupID, viewID;               
            float timeOffset;
            DateTime startTimestamp;
            DateTime endTimestamp;
            DateTime? startDate = null;
            DateTime? endDate = null;

            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (instanceId != null && !ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(instanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);

            }
            //END

            if (int.TryParse(instanceId, out sqlServerID) && int.TryParse(groupId, out enumGroupID) && int.TryParse(viewId, out viewID))
            {
                int sqlSignatureID = 0, statementTypeID = -1, includeSQLStatement = 1, includeSQLProcedure = 1, includeSQLBatch = 1,
                    includeIncompletedQueries = 0, includeTimeOverlappedQueries = 0;

                int.TryParse(queryFilters.QuerySignatureId, out sqlSignatureID);
                if (int.TryParse(queryFilters.EventTypeId, out statementTypeID))
                {
                    if (statementTypeID < 0 && statementTypeID > 2)
                        statementTypeID = -1;
                }
                else
                    statementTypeID = -1;

                if (!String.IsNullOrWhiteSpace(queryFilters.AdvancedFilters))
                {
                    if (!(int.TryParse(queryFilters.AdvancedFilters[0].ToString(), out includeSQLStatement) && (includeSQLStatement == 0 || includeSQLStatement == 1)))
                        includeSQLStatement = 1;
                    if (!(int.TryParse(queryFilters.AdvancedFilters[1].ToString(), out includeSQLProcedure) && (includeSQLProcedure == 0 || includeSQLProcedure == 1)))
                        includeSQLProcedure = 1;
                    if (!(int.TryParse(queryFilters.AdvancedFilters[2].ToString(), out includeSQLBatch) && (includeSQLBatch == 0 || includeSQLBatch == 1)))
                        includeSQLBatch = 1;
                    if (!(int.TryParse(queryFilters.AdvancedFilters[3].ToString(), out includeTimeOverlappedQueries) && (includeTimeOverlappedQueries == 0 || includeTimeOverlappedQueries == 1)))
                        includeTimeOverlappedQueries = 0;
                    if (!(int.TryParse(queryFilters.AdvancedFilters[4].ToString(), out includeIncompletedQueries) && (includeIncompletedQueries == 0 || includeIncompletedQueries == 1)))
                        includeIncompletedQueries = 0;
                }

                string groupBy = ((GroupType)enumGroupID).ToString();

                if (String.IsNullOrWhiteSpace(queryFilters.SQLExclude))                // check for valid values of wildcards of SQL Text
                    queryFilters.SQLExclude = null;
                else if (queryFilters.SQLExclude.Length > 0)
                {
                    if (queryFilters.SQLExclude[0] != '%' && queryFilters.SQLExclude[queryFilters.SQLExclude.Length - 1] != '%')
                        queryFilters.SQLExclude = "%" + queryFilters.SQLExclude + "%";
                }

                if (String.IsNullOrWhiteSpace(queryFilters.SQLInclude))
                    queryFilters.SQLInclude = null;
                else if (queryFilters.SQLInclude.Length > 0)
                {
                    if (queryFilters.SQLInclude[0] != '%' && queryFilters.SQLInclude[queryFilters.SQLInclude.Length - 1] != '%')
                        queryFilters.SQLInclude = "%" + queryFilters.SQLInclude + "%";
                }

                if (!DateTime.TryParseExact(queryFilters.StartTimestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out startTimestamp))
                {
                    _logX.Warn("The parsing of start date was unsuccessful" + queryFilters.StartTimestamp);
                };

                if (!DateTime.TryParseExact(queryFilters.EndTimestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out endTimestamp))
                {
                    _logX.Warn("The parsing of end date was unsuccessful" + queryFilters.EndTimestamp);
                }

                // handle if timestamp is not valid
                if (!startTimestamp.Equals(default(DateTime)))
                {
                    startDate = startTimestamp;
                }

                if (!endTimestamp.Equals(default(DateTime)))
                {
                    endDate = endTimestamp;
                }

                float.TryParse(timeZoneOffset, out timeOffset);
                Math.Round(timeOffset, 2);

                if ((viewID < 1) || (viewID > 8) || (startDate == null) || (endDate == null) || (Convert.ToInt32(Math.Abs(timeOffset)) < 0) || (Convert.ToInt32(Math.Abs(timeOffset)) > 23))  // return null, if the viewID is an invalid integer, or timestamp is null 
                    return getQueryMonitorDataList;

                startTimestamp = startTimestamp.AddHours(timeOffset);
                endTimestamp = endTimestamp.AddHours(timeOffset);

                if (!(String.IsNullOrWhiteSpace(queryFilters.ApplicationIds)))                   // change the pipe separated Ids to comma separated for easy checking in stored procedure
                    queryFilters.ApplicationIds = queryFilters.ApplicationIds.Replace('|', ',');
                if (!(String.IsNullOrWhiteSpace(queryFilters.DatabaseIds)))
                    queryFilters.DatabaseIds = queryFilters.DatabaseIds.Replace('|', ',');
                if (!(String.IsNullOrWhiteSpace(queryFilters.ClientIds)))
                    queryFilters.ClientIds = queryFilters.ClientIds.Replace('|', ',');
                if (!(String.IsNullOrWhiteSpace(queryFilters.UserIds)))
                    queryFilters.UserIds = queryFilters.UserIds.Replace('|', ',');

                if (groupBy.Equals("Application"))
                {
                    getQueryMonitorDataList = RepositoryHelper.GetQueryMonitorDataByApplicationForGraphs(RestServiceConfiguration.SQLConnectInfo, sqlServerID, viewID, timeOffset, queryFilters.ApplicationIds, queryFilters.DatabaseIds, queryFilters.UserIds, queryFilters.ClientIds, queryFilters.SQLInclude, queryFilters.SQLExclude,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startTimestamp, endTimestamp, sqlSignatureID, statementTypeID);
                }
                else if (groupBy.Equals("Database"))
                {
                    getQueryMonitorDataList = RepositoryHelper.GetQueryMonitorDataByDatabaseForGraphs(RestServiceConfiguration.SQLConnectInfo, sqlServerID, viewID, timeOffset, queryFilters.ApplicationIds, queryFilters.DatabaseIds, queryFilters.UserIds, queryFilters.ClientIds, queryFilters.SQLInclude, queryFilters.SQLExclude,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startTimestamp, endTimestamp, sqlSignatureID, statementTypeID);
                }
                else if (groupBy.Equals("User"))
                {
                    getQueryMonitorDataList = RepositoryHelper.GetQueryMonitorDataByUserForGraphs(RestServiceConfiguration.SQLConnectInfo, sqlServerID, viewID, timeOffset, queryFilters.ApplicationIds, queryFilters.DatabaseIds, queryFilters.UserIds, queryFilters.ClientIds, queryFilters.SQLInclude, queryFilters.SQLExclude,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startTimestamp, endTimestamp, sqlSignatureID, statementTypeID);
                }
                else if (groupBy.Equals("Client"))
                {
                    getQueryMonitorDataList = RepositoryHelper.GetQueryMonitorDataByClientForGraphs(RestServiceConfiguration.SQLConnectInfo, sqlServerID, viewID, timeOffset, queryFilters.ApplicationIds, queryFilters.DatabaseIds, queryFilters.UserIds, queryFilters.ClientIds, queryFilters.SQLInclude, queryFilters.SQLExclude,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startTimestamp, endTimestamp, sqlSignatureID, statementTypeID);
                }
                else if (groupBy.Equals("QuerySignature"))
                {
                    getQueryMonitorDataList = RepositoryHelper.GetQueryMonitorDataByQuerySignatureForGraphs(RestServiceConfiguration.SQLConnectInfo, sqlServerID, viewID, timeOffset, queryFilters.ApplicationIds, queryFilters.DatabaseIds, queryFilters.UserIds, queryFilters.ClientIds, queryFilters.SQLInclude, queryFilters.SQLExclude,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startTimestamp, endTimestamp, sqlSignatureID, statementTypeID);
                }
                else if (groupBy.Equals("QueryStatement")) 
                {
                    getQueryMonitorDataList = RepositoryHelper.GetQueryMonitorDataByQueryStatementForGraphs(RestServiceConfiguration.SQLConnectInfo, sqlServerID, viewID, timeOffset, queryFilters.ApplicationIds, queryFilters.DatabaseIds, queryFilters.UserIds, queryFilters.ClientIds, queryFilters.SQLInclude, queryFilters.SQLExclude,
                      includeSQLStatement, includeSQLProcedure, includeSQLBatch, includeIncompletedQueries, includeTimeOverlappedQueries, startTimestamp, endTimestamp, sqlSignatureID, statementTypeID);
                }
            }
            return getQueryMonitorDataList;
        }

        #endregion
    }
}
