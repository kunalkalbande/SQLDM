using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Service.DataContracts.v1;
using Idera.SQLdm.Service.DataContracts.v1.Widgets;
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
using System.Collections;
using Idera.SQLdm.Service.Configuration;
using System.ServiceModel.Web;
using Idera.SQLdm.Service.Helpers.Auth;

namespace Idera.SQLdm.Service.Web
{
    partial class WebService : ITopXManager
    {
        #region ITopXManager Members

        public IList<ResponseTimeForInstance> GetTopInstanceByResponseTime(string timeZoneOffset, int count)
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetAllInstancesSummary to Implement Instance level security
            return RepositoryHelper.GetTopServerResponseTime(RestServiceConfiguration.SQLConnectInfo, timeZoneOffset, count, userToken);
        }

        /*public MonitoredSqlServerCollection GetTopServersAlerts(DateTime startDate, DateTime endDate)
        {
            var MonitoredSqlServerList = RepositoryHelper.GetMonitoredSqlServers(RestServiceConfiguration.SQLConnectInfo, true, null);

            return (new MonitoredSqlServerCollection(RepositoryHelper.GetTopServerDatabaseAlerts(RestServiceConfiguration.SQLConnectInfo, startDate, endDate, MonitoredSqlServerList.Count)));
        }*/

        public IList<LongestQueriesForInstance> GetTopQueriesByDuration(string timeZoneOffset, int count, int NumDays)
        {
            SetConnectionCredentiaslFromCWFHost();
            if (NumDays == 0)
            {
                NumDays = 1;
            }
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetTopQueriesByDuration to Implement Instance level security
            return RepositoryHelper.GetTopQueriesByDuration(RestServiceConfiguration.SQLConnectInfo, timeZoneOffset, count, NumDays,userToken);
        }

        public IList<BlockedSessionForInstance> GetBlockedSessionCount(string timeZoneOffset, int count)
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetTopBlockedSessionCount to Implement Instance level security
            return RepositoryHelper.GetTopBlockedSessionCount(RestServiceConfiguration.SQLConnectInfo, timeZoneOffset, count,userToken);
        }

        //SQLdm 9.1 (Sanjali Makkar): adding the filter of Instance ID
        public IList<SessionsByCPUUsage> GetTopSessionsByCPUUsage(string timeZoneOffset, string InstanceId, int count)
        {
            SetConnectionCredentiaslFromCWFHost();
            int serverId;

            if (InstanceId != null)
            {
                //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                if (InstanceId !=null && !ServerAuthorizationHelper.IsServerAuthorized(Convert.ToInt32(InstanceId),userToken))
                {
                    throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);

                }
                //END
                if (int.TryParse(InstanceId, out serverId))
                {
                    return RepositoryHelper.GetTopSessionsByCPUUsage(RestServiceConfiguration.SQLConnectInfo, timeZoneOffset, count, serverId,userToken);
                }
                else
                {
                    throw new FaultException("InstanceID must be int");
                }
            }

            else
            {
                return RepositoryHelper.GetTopSessionsByCPUUsage(RestServiceConfiguration.SQLConnectInfo, timeZoneOffset, count, null, userToken);
            }
        }

        public IList<InstancesByQueryCount> GetTopInstancesByQueryCount(string timeZoneOffset, int count)
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetTopInstancesByQueryCount to Implement Instance level security
            return RepositoryHelper.GetTopInstancesByQueryCount(RestServiceConfiguration.SQLConnectInfo, timeZoneOffset, count,userToken);
        }

        public IList<DatabaseByActivity> GetTopDatabaseByActivity(string timeZoneOffset, int count)
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetTopDatabaseByActivity to Implement Instance level security
            return RepositoryHelper.GetTopDatabaseByActivity(RestServiceConfiguration.SQLConnectInfo, timeZoneOffset, count, userToken);
        }


        //SQLdm 8.5 (Ankit Srivastava): for Top X API- Tempdb Utilization
        public IList<TempDBUtilizationForInstance> GetInstancesByTempDbUtilization(string timeZoneOffset, int count)
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetInstancesByTempDbUtilization to Implement Instance level security
            return RepositoryHelper.GetInstancesByTempDbUtilization(RestServiceConfiguration.SQLConnectInfo, timeZoneOffset, count, userToken);
        }

        //SQLdm 8.5 (Ankit Srivastava): for Top X API- Query Monitor Event
        /*public MonitoredSqlServerCollection GetInstancesByQueries(DateTime startDate, DateTime endDate, string countOfInstances)
        {
            int passedCountOfInstances;
            if (!int.TryParse(countOfInstances, out passedCountOfInstances))
            {
                passedCountOfInstances = 10;
            }

            return (new MonitoredSqlServerCollection(RepositoryHelper.GetInstancesByQueries(RestServiceConfiguration.SQLConnectInfo, startDate, endDate, passedCountOfInstances)));
        }*/

        //SQLdm 8.5 (Ankit Srivastava): for Top X API- Disk Space
        public IList<DiskSpaceByInstance> GetInstancesByDiskSpace(string timeZoneOffset, int count)
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetInstancesByDiskSpace to Implement Instance level security
            return RepositoryHelper.GetInstancesByDiskSpace(RestServiceConfiguration.SQLConnectInfo, timeZoneOffset, count, userToken);
        }

        public IList<SessionCountForInstance> GetTopInstanceBySessionCount(string timeZoneOffset, int count)
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetTopInstancesBySessionCount to Implement Instance level security
            return RepositoryHelper.GetTopInstancesBySessionCount(RestServiceConfiguration.SQLConnectInfo, timeZoneOffset, count, userToken);
        }

        //SQLdm 8.5 (Gaurav Karwal): Returns the list of all instances by connection count
        public List<InstancesByConnectionCount> GetInstancesByConnectionCount(string timeZoneOffset, int count) 
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetInstancesByConnectionCount to Implement Instance level security
            return RepositoryHelper.GetInstancesByConnectionCount(RestServiceConfiguration.SQLConnectInfo, timeZoneOffset, count, userToken);
        }

        public List<DatabasesByDatabaseFileSize> GetDatabasesByFileSize(string timeZoneOffset, int count) 
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetDatabasesByFileSize to Implement Instance level security
            return RepositoryHelper.GetDatabasesByFileSize(RestServiceConfiguration.SQLConnectInfo, timeZoneOffset, count, userToken);
        }
        //SQLdm 8.5 (Ankit Srivastava): for Top X API- Instance Alerts
        public IEnumerable<AlertsCountForInstance> GetInstancesByAlerts(int count)
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetInstancesByAlerts to Implement Instance level security
            return (new List<AlertsCountForInstance>(RepositoryHelper.GetInstancesByAlerts(RestServiceConfiguration.SQLConnectInfo, count, userToken)));
        }

        //SQLdm 8.5 (Ankit Srivastava): for Top X API- Most Alerts for Databases
        public IEnumerable<AlertsCountForDatabase> GetDatabasesByAlerts(int count)
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetDatabasesByAlerts to Implement Instance level security
            return (new List<AlertsCountForDatabase>(RepositoryHelper.GetDatabasesByAlerts(RestServiceConfiguration.SQLConnectInfo, count, userToken)));
        }

        //SQLdm 8.5 (Ankit Srivastava): for Top X API-  Sql Cpu Load
        public IEnumerable<SqlCpuLoadForInstance> GetInstancesBySqlCpuLoad(int count)
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetInstancesBySqlCpuLoad to Implement Instance level security
            return (new List<SqlCpuLoadForInstance>(RepositoryHelper.GetInstancesBySqlCpuLoad(RestServiceConfiguration.SQLConnectInfo, count, userToken)));
        }

        //SQLdm 8.5 (Ankit Srivastava): for Top X API- IO Physical Count
        public IEnumerable<IOPhysicalUsageForInstance> GetInstancesByIOPhysicalCount(int count)
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetInstancesByIOPhysicalCount to Implement Instance level security
            return (new List<IOPhysicalUsageForInstance>(RepositoryHelper.GetInstancesByIOPhysicalCount(RestServiceConfiguration.SQLConnectInfo, count, userToken)));
        }


        //SQLdm 8.5 (Ankit Srivastava): for Top X API-  Sql Memory Usage
        public IEnumerable<SqlMemoryUsageForInstance> GetInstancesBySqlMemoryUsage(int count)
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetInstancesBySqlMemoryUsage to Implement Instance level security
            return (new List<SqlMemoryUsageForInstance>(RepositoryHelper.GetInstancesBySqlMemoryUsage(RestServiceConfiguration.SQLConnectInfo, count, userToken)));
        }

        public List<WaitStatisticsByInstance> GetInstancesByWaitStatistics(int count) 
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetInstancesByWaitStatistics to Implement Instance level security
            return RepositoryHelper.GetInstancesByWaitStatistics(RestServiceConfiguration.SQLConnectInfo, count, userToken);
        }

        public IList<ProjectedGrowthOfDatabaseSize> GetTopDatabasesByGrowth(string timeZoneOffset, int count, int numHistoryDays)
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetInstancesByWaitStatistics to Implement Instance level security
            return RepositoryHelper.GetTopDatabasesByGrowth(RestServiceConfiguration.SQLConnectInfo, timeZoneOffset, count, numHistoryDays, userToken);            
        }

        //SQLdm 9.1 (Sanjali Makkar): for Top X API- Sessions by I/O Activity
        public IList<SessionsByIOActivity> GetTopSessionsByIOActivity(string timeZoneOffset, string InstanceId, int count)
        {
            SetConnectionCredentiaslFromCWFHost();
            int serverId;
            if (InstanceId != null)
            {
                //START SQLdm 10.0 (Swati Gogia): Implemented for Instance level security
                if (userToken.AssignedServers.Select(x => x.Server.SQLServerID == Convert.ToInt32(InstanceId)).Count() == 0)
                {
                    throw new WebFaultException(System.Net.HttpStatusCode.Forbidden);

                }
                //END
                if (int.TryParse(InstanceId, out serverId))
                {
                    return RepositoryHelper.GetTopSessionsByIOActivity(RestServiceConfiguration.SQLConnectInfo, timeZoneOffset, count, serverId);
                }
                else
                {
                    throw new FaultException("InstanceID must be int");
                }
            }
            else
            {
                return RepositoryHelper.GetTopSessionsByIOActivity(RestServiceConfiguration.SQLConnectInfo, timeZoneOffset, count, null);
            }
        }


        public IDictionary<int, decimal[]> getThresholdsForMetric(int metricId)
        {
            SetConnectionCredentiaslFromCWFHost();
            return RepositoryHelper.getThresholdsForMetric(RestServiceConfiguration.SQLConnectInfo, metricId);
        }

        #endregion
    }
}
