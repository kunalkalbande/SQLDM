using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdm.Service.ServiceContracts.v1;
using Idera.SQLdm.Service.DataContracts.v1.Category;
using Idera.SQLdm.Service.DataContracts.v1.Category.Sessions;
using System.ServiceModel;
using Idera.SQLdm.Service.Repository;

using Idera.SQLdm.Service.DataContracts.v1;
using Idera.SQLdm.Service.DataContracts.v1.Databases;
using Idera.SQLdm.Service.Configuration;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Service.Helpers;
using System.ServiceModel.Web;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Service.Helpers.Auth;

namespace Idera.SQLdm.Service.Web
{
    partial class WebService : ICategoryManager
    {
        #region ICategoryManager Members

        public IList<ResourcesForInstance> GetResourcesForInstance(string InstanceId, string timeZoneOffset, int NumHistoryMinutes, DateTime endDate, int limit)
        {
            SetConnectionCredentiaslFromCWFHost();
            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (!ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(InstanceId),userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            //End

            int serverId;

            if (int.TryParse(InstanceId, out serverId))
            {
                return RepositoryHelper.GetResourcesForInstance(RestServiceConfiguration.SQLConnectInfo, serverId, timeZoneOffset, NumHistoryMinutes, endDate, limit);                
            }
            else
            {
                throw new FaultException("InstanceId must be int");
            }
        }

        public IList<FileDrivesForInstance> GetFileDrivesForInstance(string InstanceId)
        {
            SetConnectionCredentiaslFromCWFHost();
            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (!ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(InstanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            //End

            int passedinstanceId;
            if (int.TryParse(InstanceId, out passedinstanceId))
            {
                return RepositoryHelper.GetFileDrivesForInstance(RestServiceConfiguration.SQLConnectInfo, passedinstanceId);
            }
            else
            {
                throw new FaultException("InstanceId must be int");
            }
        }

        public IList<FileActivityForInstance> GetFileActivityForInstance(string InstanceId, string timeZoneOffset, int NumHistoryMinutes)
        {
            SetConnectionCredentiaslFromCWFHost();
            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (!ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(InstanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            //End

            int passedinstanceId;
            if (int.TryParse(InstanceId, out passedinstanceId))
            {
                return RepositoryHelper.GetFileActivityForInstance(RestServiceConfiguration.SQLConnectInfo, passedinstanceId, timeZoneOffset, NumHistoryMinutes);
            }
            else
            {
                throw new FaultException("InstanceId must be int");
            }
        }

        public IList<ServerWaitsForInstance> GetServerWaitsForInstance(string InstanceId, string timeZoneOffset, int NumHistoryMinutes, DateTime endDate, string category)
        {
            SetConnectionCredentiaslFromCWFHost();
            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (!ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(InstanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            //End

            int passedinstanceId;
            if (int.TryParse(InstanceId, out passedinstanceId))
            {
                return RepositoryHelper.GetServerWaitsForInstance(RestServiceConfiguration.SQLConnectInfo, passedinstanceId, timeZoneOffset, NumHistoryMinutes, endDate, category);
            }
            else
            {
                throw new FaultException("InstanceId must be int");
            }
        }

        //public IList<SessionsForInstance> GetSessionsForInstance(string InstanceId, DateTime start, DateTime end, int limit, string type)
        public IList<SessionsForInstance> GetSessionsForInstance(string InstanceId, string timeZoneOffset, int limit, string UserSessionsOnly, string excludeSQLDMSessions)
        {
            SetConnectionCredentiaslFromCWFHost();
            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (!ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(InstanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            //End

            int passedinstanceId;
            if (int.TryParse(InstanceId, out passedinstanceId))
            {
                //SQLdm 8.6 (Ankit Srivastava) -- API Defects - Converted to appropriate boolean values in case random/empty Strings
                bool UserSessionsOnlyValue = false;
                if (!String.IsNullOrWhiteSpace(UserSessionsOnly) && !Boolean.TryParse(UserSessionsOnly, out UserSessionsOnlyValue)) UserSessionsOnlyValue = false;

                bool excludeSQLDMSessionsValue = false;
                if (!String.IsNullOrWhiteSpace(excludeSQLDMSessions) && !Boolean.TryParse(excludeSQLDMSessions, out excludeSQLDMSessionsValue)) excludeSQLDMSessionsValue = false;

                // Tolga K - SQLDM-19828
                if(limit == 0)
                {
                    // The default value of limit is 7 days, 7*24*60 = 10800 mins
                    limit = 10800;
                }

                return RepositoryHelper.GetSessionsForInstance(RestServiceConfiguration.SQLConnectInfo, passedinstanceId, timeZoneOffset, limit, UserSessionsOnlyValue, excludeSQLDMSessionsValue);
            }
            else
            {
                throw new FaultException("InstanceId must be int");
            }
        }

        /*public IList<SessionActivityForInstance> GetSessionsActivityForInstance(string InstanceId, int NumHistoryMinutes)
        {
            int passedinstanceId;
            if (int.TryParse(InstanceId, out passedinstanceId))
            {
                return RepositoryHelper.GetSessionsActivityForInstance(RestServiceConfiguration.SQLConnectInfo, passedinstanceId, NumHistoryMinutes);
            }
            else
            {
                throw new FaultException("InstanceId must be int");
            }
        }*/

        public IList<SessionResponseTimeForInstance> GetSessionResponseTimeForInstance(string InstanceId, int NumHistoryMinutes)
        {
            SetConnectionCredentiaslFromCWFHost();
            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (!ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(InstanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            //End

            int passedinstanceId;
            if (int.TryParse(InstanceId, out passedinstanceId))
            {
                return RepositoryHelper.GetSessionResponseTimeForInstance(RestServiceConfiguration.SQLConnectInfo, passedinstanceId, NumHistoryMinutes);
            }
            else
            {
                throw new FaultException("InstanceId must be int");
            }
        }

        public IList<QueryStatisticsForInstance> GetQueryStatsForInstance(string InstanceId, string timeZoneOffset, int NumHistoryMinutes, int limit)
        {
            SetConnectionCredentiaslFromCWFHost();
            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (!ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(InstanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            //End

            int passedinstanceId;
            if (int.TryParse(InstanceId, out passedinstanceId))
            {
                return RepositoryHelper.GetQueryStatsForInstance(RestServiceConfiguration.SQLConnectInfo, passedinstanceId, timeZoneOffset, NumHistoryMinutes, limit);
            }
            else
            {
                throw new FaultException("InstanceId must be int");
            }          
        }

        public IList<QueryWaitStatisticsForInstance> GetQueryWaitStatisticsForInstance(string InstanceId, string timeZoneOffset, DateTime startDate, DateTime endDate, string WaitTypeID, string WaitCategoryID, string SQLStatementID, string ApplicationID, string DatabaseID, string HostID, string SessionID, string LoginID)
        {
            SetConnectionCredentiaslFromCWFHost();
            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (!ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(InstanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            //End

            int instanceId;
            int? waitTypeId, waitCategoryId, sqlStatementId, applicationId, databaseId, hostId, sessionId, loginId;
            if (!int.TryParse(InstanceId, out instanceId))
            {
                throw new FaultException("InstanceId must be int");
            }
            else
            {
                //START SQLdm 10.0 (Sanjali Makkar): To add filters respective to IDs of different parameters
                waitTypeId = parseNullableParameters(WaitTypeID);
                waitCategoryId = parseNullableParameters(WaitCategoryID);
                sqlStatementId = parseNullableParameters(SQLStatementID);
                applicationId = parseNullableParameters(ApplicationID);
                databaseId = parseNullableParameters(DatabaseID);
                hostId = parseNullableParameters(HostID);
                sessionId = parseNullableParameters(SessionID);
                loginId = parseNullableParameters(LoginID);

                //To modify startDate and endDate fields 
                DateTime? startDateTimeInUTC = DateTimeHelper.ConvertToUTC(startDate, timeZoneOffset);
                DateTime? endDateTimeInUTC = DateTimeHelper.ConvertToUTC(endDate, timeZoneOffset);

                ConfigureDateTime(ref startDateTimeInUTC, ref endDateTimeInUTC);
                //END SQLdm 10.0 (Sanjali Makkar): To add filters respective to IDs of different parameters

                return RepositoryHelper.GetQueryWaitStatisticsForInstance(RestServiceConfiguration.SQLConnectInfo, instanceId, timeZoneOffset, startDateTimeInUTC, endDateTimeInUTC, waitTypeId, waitCategoryId, sqlStatementId, applicationId, databaseId, hostId, sessionId, loginId);
            }
        }

        public IList<QueryWaitStatisticsForInstanceOverview> GetQueryWaitStatisticsForInstanceOverview(string InstanceId, string timeZoneOffset, DateTime startDate, DateTime endDate)
        {
            SetConnectionCredentiaslFromCWFHost();
            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (!ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(InstanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            //End

            int instanceId;
            if (!int.TryParse(InstanceId, out instanceId))
            {
                throw new FaultException("InstanceId must be int");
            }
            else
            {
                //To modify startDate and endDate fields 
                DateTime? startDateTimeInUTC = DateTimeHelper.ConvertToUTC(startDate, timeZoneOffset);
                DateTime? endDateTimeInUTC = DateTimeHelper.ConvertToUTC(endDate, timeZoneOffset);

                ConfigureDateTime(ref startDateTimeInUTC, ref endDateTimeInUTC);

                return RepositoryHelper.GetQueryWaitStatisticsForInstanceOverview(RestServiceConfiguration.SQLConnectInfo, instanceId, timeZoneOffset, startDateTimeInUTC, endDateTimeInUTC);
            }
        }
        public IList<DataContracts.v1.Databases.CapacityUsageForDatabase> GetCapacityUsageForDatabase(string InstanceId, string DatabaseId, int NumHistoryMinutes)
        {
            SetConnectionCredentiaslFromCWFHost();
           
            IList<CapacityUsageForDatabase> res = new List<CapacityUsageForDatabase>();
            CapacityUsageForDatabase data = new CapacityUsageForDatabase
            {

                DataFileSizeInMb = 1.2,
                UnusedDataSizeInMb = 2.3,
                LogFileSizeInMb = 3.4,
                UnusedLogSizeInMb = Double.Parse(DatabaseId),// 4.5, 
                NoOfFiles = 6,
                UTCCollectionDateTime = DateTime.Now
            };
            res.Add(data);
            return res;
        }
        //SQLdm 10.2 Added history range support
        public IList<DataContracts.v1.Databases.TempDBStats> GetTempDBStatsForInstance(string InstanceId, string timeZoneOffset, int NumHistoryMinutes, DateTime endDate)
        {
            SetConnectionCredentiaslFromCWFHost();
            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (!ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(InstanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            //End

            int passedinstanceId;
            if (int.TryParse(InstanceId, out passedinstanceId))
            {
                return RepositoryHelper.GetTempDBStatsForInstance(RestServiceConfiguration.SQLConnectInfo, passedinstanceId, timeZoneOffset, NumHistoryMinutes,endDate);
            }
            else
            {
                throw new FaultException("InstanceId must be int");
            }
        }      


        /// <summary>
        /// This API is not being used by web UI team. Should be removed in next release as seems to be a dead API now.
        /// </summary>
        /// <param name="InstanceId"></param>
        /// <param name="timeZoneOffset"></param>
        /// <returns></returns>
        public IList<AvailabilityGroupSummaryForDatabase> GetAvailabilityGroupForInstance(string InstanceId, string timeZoneOffset)
        {
            SetConnectionCredentiaslFromCWFHost();
            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (!ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(InstanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            //End

            int passedinstanceId;
            if (int.TryParse(InstanceId, out passedinstanceId))
            {
                return RepositoryHelper.GetAvailabilityGroupForInstance(RestServiceConfiguration.SQLConnectInfo, passedinstanceId, timeZoneOffset);
            }
            else
            {
                throw new FaultException("InstanceId must be int");
            }
        }
        //SQLdm 10.2 Added history range support
        public IList<AvailabilityGroupSummaryForDatabase> GetAvailabilityGroupStatsForInstance(string InstanceId, string timeZoneOffset, int NumHistoryMinutes,DateTime endDate)
        {
            SetConnectionCredentiaslFromCWFHost();
            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (!ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(InstanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            //End

            int passedinstanceId;
            if (int.TryParse(InstanceId, out passedinstanceId))
            {
                return RepositoryHelper.GetAvailabilityGroupForInstance(RestServiceConfiguration.SQLConnectInfo, passedinstanceId, timeZoneOffset, NumHistoryMinutes,endDate);
            }
            else
            {
                throw new FaultException("InstanceId must be int");
            }
        }

        //SQLdm 8.5 (Ankit Srivastava): for  Categories API-  Session Statistics
        //public IList<SessionSnapshot> GetSessionStatisticsForInstance(string InstanceId, int NumHistoryMinutes)            
        public IList<ServerSessionStatistics> GetSessionStatisticsForInstance(string InstanceId, string timeZoneOffset, int NumHistoryMinutes, DateTime endDate)
        {
            SetConnectionCredentiaslFromCWFHost();
            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (!ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(InstanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            //End

            int passedinstanceId;
            if (int.TryParse(InstanceId, out passedinstanceId))
            {
                return RepositoryHelper.GetSessionStatisticsForInstance(RestServiceConfiguration.SQLConnectInfo, passedinstanceId, timeZoneOffset, NumHistoryMinutes, endDate);
            }
            else
            {
                throw new FaultException("InstanceId must be int");
            }
        }


  /*          public IList<MetricHistory> GetMetricsHistoryByMetricID(string InstanceId, string metricId, string numOfMinInHistory, string databaseId)
        {
            SetConnectionCredentiaslFromCWFHost();
            int passedInstanceId;
            int passedMetricId;
            int numOfMinutesInHistory, databaseID;


            int.TryParse(numOfMinInHistory, out numOfMinutesInHistory);
            int.TryParse(databaseId, out databaseID);

            if (int.TryParse(InstanceId, out passedInstanceId) && (int.TryParse(metricId, out passedMetricId)))
            {
                return RepositoryHelper.GetMetricsHistoryByMetricID(RestServiceConfiguration.SQLConnectInfo, passedInstanceId, passedMetricId, numOfMinutesInHistory, databaseID);
            }
            else
            {
                return null;
            }
        }

        */

        //SQLdm 9.1 (Sanjali Makkar) (Baseline Statistics) - Adding Baseline Statistics For Metric

        public IList<BaselineForMetric> GetBaselineForMetric(string InstanceId, string MetricId, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate, int limit)
        {
            SetConnectionCredentiaslFromCWFHost();
            //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
            if (!ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(InstanceId), userToken))
            {
                throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);
            }
            //End

            int serverId, metricId, numHistoryMinutes;

            if (int.TryParse(InstanceId, out serverId) && int.TryParse(MetricId, out metricId))
            {
                if (NumHistoryMinutes != null)
                {
                    if (int.TryParse(NumHistoryMinutes, out numHistoryMinutes))
                    {
                        return RepositoryHelper.GetBaselineForMetric(RestServiceConfiguration.SQLConnectInfo, serverId, metricId, timeZoneOffset, numHistoryMinutes, endDate, limit);
                    }
                    else
                        throw new FaultException("Number of minutes should be integer");
                }
                else
                {
                    return RepositoryHelper.GetBaselineForMetric(RestServiceConfiguration.SQLConnectInfo, serverId, metricId, timeZoneOffset, - 1, endDate, limit);
                }
            }
            else
            {
                throw new FaultException("InstanceId and MetricId both must be integer");
            }
        }

        private static int? parseNullableParameters(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
                {
                    int output;
                    if (int.TryParse(input, out output))
                    {
                        return output;
                    }
                    else
                        throw new FaultException("Supplied Parameter is not of correct type");
                }
            return null;
        }

        //START SQLdm 10.0 (Sanjali Makkar): To modify startDate and endDate fields according to whether they are null or not
        private static void ConfigureDateTime(ref DateTime? startDateTimeInUTC, ref DateTime? endDateTimeInUTC)
        {
            //case 1: StartTime != null and EndTime == null
            if (!startDateTimeInUTC.Equals(DateTime.MinValue) && endDateTimeInUTC.Equals(DateTime.MinValue))
                endDateTimeInUTC = null;

            //case 2: StartTime == null and EndTime != null
            if (startDateTimeInUTC.Equals(DateTime.MinValue) && !endDateTimeInUTC.Equals(DateTime.MinValue))
                startDateTimeInUTC = null;

            //case 3: StartTime == null and EndTime == null
            if (startDateTimeInUTC.Equals(DateTime.MinValue) && endDateTimeInUTC.Equals(DateTime.MinValue))
                startDateTimeInUTC = endDateTimeInUTC = null;
            
            //case 4: StartTime != null and EndTime != null
            //Nothing to do
        }
        //END SQLdm 10.0 (Sanjali Makkar): To modify startDate and endDate fields according to whether they are null or not

        #endregion

        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///Get Paging per second info
        ///</summary>
        public IList<OSPagesPerSec> GetMemoryOSPagingPerSecond(string InstanceId, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate)
        {
            SetConnectionCredentiaslFromCWFHost();
            try {
                int instance = int.Parse(InstanceId);
                float tzo = float.Parse(timeZoneOffset);
                return RepositoryHelper.GetMemoryOSPagingPerSecond(RestServiceConfiguration.SQLConnectInfo, instance, tzo, NumHistoryMinutes, endDate);
            }
            catch(Exception e)
            {
                LOG.ErrorFormat("Please check the url format {0}"+e.Message);
                return null;
            }
            
        }

        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///Get Database Running Statistics
        ///</summary>
        public IList<DatabaseRunningStatistics> GetDBStatistics(string InstanceId, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate)
        {
            SetConnectionCredentiaslFromCWFHost();
            try
            {
                int instance = int.Parse(InstanceId);
                float tzo = float.Parse(timeZoneOffset);
                return RepositoryHelper.GetDBStatistics(RestServiceConfiguration.SQLConnectInfo, instance, tzo, NumHistoryMinutes, endDate);
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Please check the url format {0}" + e.Message);
                return null;
            }
        }

        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///Get CPU Statistics
        ///</summary>
        public IList<CPUStatistics> GetCPUStatistics(string InstanceId, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate)
        {
            SetConnectionCredentiaslFromCWFHost();
            try
            {
                int instance = int.Parse(InstanceId);
                float tzo = float.Parse(timeZoneOffset);
                return RepositoryHelper.GetCPUStatistics(RestServiceConfiguration.SQLConnectInfo, instance, tzo, NumHistoryMinutes, endDate);
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Please check the url format {0}" + e.Message);
                return null;
            }
        }

        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///Get Network Statistics
        ///</summary>
        public IList<NetworkStatistics> GetNetworkStatistics(string InstanceId, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate)
        {
            SetConnectionCredentiaslFromCWFHost();
            try
            {
                int instance = int.Parse(InstanceId);
                float tzo = float.Parse(timeZoneOffset);
                return RepositoryHelper.GetNetworkStatistics(RestServiceConfiguration.SQLConnectInfo, instance, tzo, NumHistoryMinutes, endDate);
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Please check the url format {0}" + e.Message);
                return null;
            }
        }

        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///Lock Waits Statistics
        ///</summary>
        public IList<LockWaitsStatistics> GetLockWaitsStatistics(string InstanceId, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate)
        {
            SetConnectionCredentiaslFromCWFHost();
            try
            {
                int instance = int.Parse(InstanceId);
                float tzo = float.Parse(timeZoneOffset);
                return RepositoryHelper.GetLockWaitsStatistics(RestServiceConfiguration.SQLConnectInfo, instance, tzo, NumHistoryMinutes, endDate);
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Please check the url format {0}" + e.Message);
                return null;
            }
        }

        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///File Activity Statistics
        ///</summary>
        public IList<FileActivityForInstance> GetFileStatistics(string InstanceId, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate)
        {
            SetConnectionCredentiaslFromCWFHost();
            try
            {
                int instance = int.Parse(InstanceId);
                float tzo = float.Parse(timeZoneOffset);
                return RepositoryHelper.FileActivityStatistics(RestServiceConfiguration.SQLConnectInfo, instance, tzo, NumHistoryMinutes, endDate);
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Please check the url format {0}" + e.Message);
                return null;
            }
        }

        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///Custom Counter Statistics
        ///</summary>
        public IList<CustomCounterStats> GetCustomCounterStatistics(string InstanceId, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate)
        {
            SetConnectionCredentiaslFromCWFHost();
            try
            {
                int instance = int.Parse(InstanceId);
                float tzo = float.Parse(timeZoneOffset);
                return RepositoryHelper.GetCustomCounterStatistics(RestServiceConfiguration.SQLConnectInfo, instance, tzo, NumHistoryMinutes, endDate);
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Please check the url format {0}" + e.Message);
                return null;
            }
        }

        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///Get Server Waits Dashboard
        ///</summary>
        public IList<ServerWaitsDashboard> GetServerWaitsDashboard(string InstanceId, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate)
        {
            SetConnectionCredentiaslFromCWFHost();
            try
            {
                int instance = int.Parse(InstanceId);
                float tzo = float.Parse(timeZoneOffset);
                return RepositoryHelper.GetServerWaitsDashboard(RestServiceConfiguration.SQLConnectInfo, instance, tzo, NumHistoryMinutes, endDate);
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Please check the url format {0}" + e.Message);
                return null;
            }
        }


        ///<summary>
        ///Author : Nishant Adhikari
        ///Version : SQLdm 10.2
        ///Get Virtualization Stats
        ///</summary>
        public VirtualizationList GetVirtualizationStats(string InstanceId, string timeZoneOffset, string NumHistoryMinutes, DateTime endDate)
        {
            SetConnectionCredentiaslFromCWFHost();
            try
            {
                int instance = int.Parse(InstanceId);
                float tzo = float.Parse(timeZoneOffset);
                return RepositoryHelper.GetVirtualizationStats(RestServiceConfiguration.SQLConnectInfo, instance, tzo, NumHistoryMinutes, endDate);
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Please check the url format {0}" + e.Message);
                return null;
            }
        }

    }
}
