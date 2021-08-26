using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Idera.SQLdm.Service;
using Idera.SQLdm.Service.Repository;
using Idera.SQLdm.Service.Web;
using Idera.SQLdm.Service.DataContracts.v1;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
namespace Idera.SQLdm.Service.Test
{
    class TopXManagerTest:TestRoot
    {

        #region GetTopQueriesByDuration Tests
        [TestCase]
        public void GetTopQueriesByDurationTestWithDaysNull()
        {
            //WebService target = new WebService();
            //string count = "10";
            //var actual = target.GetTopQueriesByDuration(count, null);
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count > 0 && actual.Count <= 10);
            //Assert.IsNotNull(actual.FirstOrDefault());
            //Assert.IsNotNull(actual.First().Database);
            //Assert.IsNotNull(actual.First().InstanceId);
            //Assert.IsNotNull(actual.First().InstanceName);
        }

        [TestCase]
        public void GetTopQueriesByDurationTestWithNull()
        {
            //WebService target = new WebService();
            //var actual = target.GetTopQueriesByDuration(null, null);
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count > 0);
            //Assert.IsNotNull(actual.FirstOrDefault());
            //Assert.IsNotNull(actual.First().Database);
            //Assert.IsNotNull(actual.First().InstanceId);
            //Assert.IsNotNull(actual.First().InstanceName);
        }

        [TestCase]
        public void GetTopQueriesByDurationTestWithCountNull()
        {
            //WebService target = new WebService();
            //var   actual=target.GetTopQueriesByDuration(null, "100");
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count > 0);
            //Assert.IsNotNull(actual.FirstOrDefault());
            //Assert.IsNotNull(actual.First().Database);
            //Assert.IsNotNull(actual.First().InstanceId);
            //Assert.IsNotNull(actual.First().InstanceName);
        }

        [TestCase]
        public void GetTopQueriesByDurationTestWithCountStr()
        {
            //WebService target = new WebService();
            //IList<DataContracts.v1.Widgets.LongestQueriesForInstance> actual=new List<DataContracts.v1.Widgets.LongestQueriesForInstance>();
            //try
            //{
            //    actual = target.GetTopQueriesByDuration("Ten", "100");
            //}
            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);
        }

        [TestCase]
        public void GetTopQueriesByDurationTestWithDaysStr()
        {
            //WebService target = new WebService();
            //IList<DataContracts.v1.Widgets.LongestQueriesForInstance> actual=new List<DataContracts.v1.Widgets.LongestQueriesForInstance>();
            //try
            //{
            //    actual = target.GetTopQueriesByDuration("10", "hundred");
            //}
            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);
        } 
        #endregion

        #region GetTopServersAlerts Tests

        [TestCase]
        public void GetTopServersAlertsTestWithNew()
        {
            WebService target = new WebService();
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            GetTopServersAlertsTest(target, startDate, endDate);
        }

        [TestCase]
        public void GetTopServersAlertsTestWithLesserEnd()
        {

            WebService target = new WebService();
            DateTime startDate = DateTime.Now;
            DateTime endDate = startDate.AddDays(-7);
            GetTopServersAlertsTest(target, startDate, endDate);
        }

        private static void GetTopServersAlertsTest(WebService target, DateTime startDate, DateTime endDate)
        {
            //MonitoredSqlServerCollection actual;
            //actual = target.GetTopServersAlerts(startDate, endDate);
            //Assert.IsNotNull(actual);
            //Assert.Greater(actual.Count, 0);
            //Assert.IsNotNull(actual.First());
            //Assert.IsNotNull(actual.First().SQLServerId);
            //Assert.IsNotNull(actual.First().Databases);
            //Assert.IsTrue(actual.First().Databases.Count > 0);
            //Assert.IsNotNull(actual.First().Databases.First());
            //Assert.IsNotNull(actual.First().Databases.First().DatabaseName);
            //Assert.IsNotNull(actual.First().Databases.First().TotalAlertCount);
        }

        
        #endregion

        #region GetBlockedSessionCount tests

        [TestCase]
        public void GetBlockedSessionCountTestWithInt()
        {
            //WebService target = new WebService();
            //string count = "8";
            //var actual = target.GetBlockedSessionCount(count);
            //Assert.IsNotNull(actual);
            //Assert.IsNotNull(actual.Count.ToString().Equals(count));
            //foreach (var mss in actual)
            //{
            //    Assert.IsNotNull(mss.BlockedSessionCount);
            //}
        }

        [TestCase]
        public void GetBlockedSessionCountTestWithStr()
        {
            //WebService target = new WebService();
            //string count = "eight";

            //IList<DataContracts.v1.Widgets.BlockedSessionForInstance> actual = new List<DataContracts.v1.Widgets.BlockedSessionForInstance>();
            //try
            //{
            //    actual = target.GetBlockedSessionCount(count);     
            //}
            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);
        } 

        #endregion

        #region GetSessionByCPUUsage Tests

        [TestCase]
        public void GetTopSessionByCPUUsageTestWithInt()
        {
            //WebService target = new WebService();
            //string count = "10";
            //var actual = target.GetTopSessionByCPUUsage(count);
            //Assert.IsNotNull(actual);
            //Assert.IsNotNull(actual.First());
            //Assert.IsNotNull(actual.First().InstanceId);
            //Assert.IsNotNull(actual.First().DatabaseName);
            //foreach (var res in actual)
            //{
            //    Assert.IsNotNull(res.DatabaseName);
            //    Assert.IsNotNull(res.SessionID);
            //}
        }

        [TestCase]
        public void GetTopSessionByCPUUsageTestWithStr()
        {
            //WebService target = new WebService();
            //string count = "ten";

            //IList<DataContracts.v1.Widgets.SessionsByCPUUsage> actual = new List<DataContracts.v1.Widgets.SessionsByCPUUsage>();
            //try
            //{
            //    actual = target.GetTopSessionByCPUUsage(count);
            //}
            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);
            }
        
        #endregion

        #region GetTopDatabaseByActivity Tests
        [TestCase]
        public void GetTopDatabaseByActivityTestWithInt()
        {
            //WebService target = new WebService();
            //string count = "5";
            //var actual = target.GetTopDatabaseByActivity(count);
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count > 0 && actual.Count <=5, "Sequence returned None or more than 5 elements. Check Data/procedure in DB");
            //Assert.IsNotNull(actual.First());
            //Assert.IsNotNull(actual.First().InstanceId);
            //Assert.IsNotNull(actual.First().DatabaseName);
            //foreach (var res in actual)
            //{
            //    Assert.IsNotNull(res.DatabaseName);
            //    Assert.IsNotNull(res.TransactionPerSec);
            //}
        }

        [TestCase]
        public void GetTopDatabaseByActivityTestWithStr()
        {
            //WebService target = new WebService();
            //string count = "five";
            //IList<DataContracts.v1.Widgets.DatabaseByActivity> actual = new List<DataContracts.v1.Widgets.DatabaseByActivity>();
            //try
            //{
            //    actual = target.GetTopDatabaseByActivity(count);
            //}
            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);
        }
        
        
        #endregion

        #region GetTopInstancesByQueryCount Tests
        [TestCase]
        public void GetTopInstancesByQueryCountTestWithInt()
        {
            //WebService target = new WebService();
            //string count = "7";
            //var actual = target.GetTopInstancesByQueryCount(count);
            //Assert.IsNotNull(actual);
            //Assert.IsNotNull(actual.First());
            //Assert.IsTrue(actual.Count > 0 && actual.Count <= 7, "Sequence returned None or more than 7 elements. Check Data/procedure in DB");
            //foreach (var res in actual)
            //{
            //    Assert.IsNotNull(res.NoOfQueries);
            //    Assert.IsNotNull(res.InstanceID);
            //    Assert.IsNotNull(res.UTCCollectionDateTime);
            //}
        }

        [TestCase]
        public void GetTopInstancesByQueryCountTestWithStr()
        {
            //WebService target = new WebService();
            //string count = "seven";
            //IList<DataContracts.v1.Widgets.InstancesByQueryCount> actual = new List<DataContracts.v1.Widgets.InstancesByQueryCount>();
            //try
            //{
            //    actual = target.GetTopInstancesByQueryCount(count);
            //}
            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);

        }  
        #endregion

        #region GetInstancesByTempDbUtilization Tests
        [TestCase]
        public void GetInstancesByTempDbUtilizationTestWithInt()
        {
            //WebService target = new WebService();
            //string count = "1";
            //var actual = target.GetInstancesByTempDbUtilization(count);
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count > 0);
            //Assert.IsNotNull(actual.FirstOrDefault());
            //Assert.IsNotNull(actual.First().InstanceName);
            //Assert.IsNotNull(actual.First().InstanceId);
            //Assert.IsNotNull(actual.First().TempDBUsageInKB);
        }

        [TestCase]
        public void GetInstancesByTempDbUtilizationTestWithStr()
        {
            //WebService target = new WebService();
            //string count = "One";
            //IList<DataContracts.v1.Widgets.TempDBUtilizationForInstance> actual = new List<DataContracts.v1.Widgets.TempDBUtilizationForInstance>();
            //try
            //{
            //    actual = target.GetInstancesByTempDbUtilization(count);
            //}
            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);
       
        } 
        #endregion
  
        #region GetInstancesByQueries Tests
        [TestCase]
        public void GetInstancesByQueriesTestWithInt()
        {
            //WebService target = new WebService();
            //DateTime startDate = new DateTime();
            //DateTime endDate = new DateTime();
            //string count = "1";
            //MonitoredSqlServerCollection actual;
            //actual = target.GetInstancesByQueries(startDate, endDate, count);
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count > 0);
            //Assert.IsNotNull(actual.FirstOrDefault());
            //Assert.IsNotNull(actual.First().InstanceName);
            //Assert.IsNotNull(actual.First().SQLServerId);
            //Assert.IsNotNull(actual.First().NumOfQueries);
        }

        [TestCase]
        public void GetInstancesByQueriesTestWithStr()
        {
            //WebService target = new WebService();
            //DateTime startDate = new DateTime();
            //DateTime endDate = new DateTime();
            //string count = "One";
            //MonitoredSqlServerCollection actual = new MonitoredSqlServerCollection();
            //try
            //{
            //    actual = target.GetInstancesByQueries(startDate, endDate, count);
            //}

            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);
            
        }

        [TestCase]
        public void GetInstancesByQueriesTestWithLesserEnd()
        {
            //WebService target = new WebService();
            //DateTime startDate = new DateTime();
            //DateTime endDate = startDate.AddDays(-7);
            //string count = "1";
            //MonitoredSqlServerCollection actual = new MonitoredSqlServerCollection();
            //try
            //{
            //    actual = target.GetInstancesByQueries(startDate, endDate, count);
            //}

            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);
        } 
        #endregion
      
        #region GetInstancesByDiskSpace Tests
		[TestCase]
        public void GetInstancesByDiskSpaceTestWithInt()
        {
            //WebService target = new WebService();
            //string count = "1";
            //var actual = target.GetInstancesByDiskSpace( count);
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count > 0);
            //Assert.IsNotNull(actual.FirstOrDefault());
            //Assert.IsNotNull(actual.First().InstanceName);
            //Assert.IsNotNull(actual.First().InstanceId);
            //Assert.IsNotNull(actual.First().DiskSpaceUtilizationPercentage);
        }

        [TestCase]
        public void GetInstancesByDiskSpaceTestWithStr()
        {
            //WebService target = new WebService();
            //string count = "One";

            //IList<DataContracts.v1.Widgets.DiskSpaceByInstance> actual = new List<DataContracts.v1.Widgets.DiskSpaceByInstance>();
            //try
            //{
            //    actual = target.GetInstancesByDiskSpace(count);
            //}
            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);
        }
	#endregion
        
        
        #region GetTopInstanceBySessionCount Tests
        [TestCase]
        public void GetTopInstanceBySessionCountWithStr()
        {
            //WebService target = new WebService();
            //string count = "One";

            //IEnumerable<DataContracts.v1.Widgets.SessionCountForInstance> actual = new List<DataContracts.v1.Widgets.SessionCountForInstance>();
            //try
            //{
            //    actual = target.GetTopInstanceBySessionCount(count);
            //}
            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);
        }

        [TestCase]
        public void GetTopInstanceBySessionCount()
        {
            //WebService target = new WebService();
            //string count = "10";

            //IEnumerable<DataContracts.v1.Widgets.SessionCountForInstance> actual = new List<DataContracts.v1.Widgets.SessionCountForInstance>();
            //actual = target.GetTopInstanceBySessionCount(count);
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count<DataContracts.v1.Widgets.SessionCountForInstance>() > 0);
            //Assert.IsNotNull(actual.First().InstanceId);
            //Assert.IsNotNull(actual.First().SessionIDCount);
            //Assert.IsNotNull(actual.First().InstanceName);
            //Assert.IsNotNull(actual.First().UTCCollectionDateTime);

        }
        #endregion

        #region GetInstancesByConnectionCount Tests
        [TestCase]
        public void InstancesByConnectionCountWithStr()
        {
            //WebService target = new WebService();
            //string count = "One";

            //IEnumerable<DataContracts.v1.Widgets.InstancesByConnectionCount> actual = new List<DataContracts.v1.Widgets.InstancesByConnectionCount>();
            //try
            //{
            //    actual = target.GetInstancesByConnectionCount(count);
            //}
            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);
        }
        [TestCase]
        public void InstancesByConnectionCountWithInt()
        {
        //    WebService target = new WebService();
        //    string count = "10";

        //    IEnumerable<DataContracts.v1.Widgets.InstancesByConnectionCount> actual = new List<DataContracts.v1.Widgets.InstancesByConnectionCount>();
        //    actual = target.GetInstancesByConnectionCount(count);
        //    Assert.IsNotNull(actual);
        //    Assert.IsTrue(actual.Count<DataContracts.v1.Widgets.InstancesByConnectionCount>() > 0);
        //    Assert.IsNotNull(actual.First().InstanceId);
        //    Assert.IsNotNull(actual.First().Severity);
        //    Assert.IsNotNull(actual.First().InstanceName);
        //    Assert.IsNotNull(actual.First().ActiveConnectionCount);
        //    Assert.IsNotNull(actual.First().UTCCollectionDateTime);

        }
        #endregion

        #region GetDatabasesByFileSize Tests
        [TestCase]
        public void GetDatabasesByFileSizeWithStr()
        {
            //WebService target = new WebService();
            //string count = "One";

            //IEnumerable<DataContracts.v1.Widgets.DatabasesByDatabaseFileSize> actual = new List<DataContracts.v1.Widgets.DatabasesByDatabaseFileSize>();
            //try
            //{
            //    actual = target.GetDatabasesByFileSize(count);
            //}
            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);
        }
        [TestCase]
        public void GetDatabasesByFileSizeWithInt()
        {
            //WebService target = new WebService();
            //string count = "10";

            //IEnumerable<DataContracts.v1.Widgets.DatabasesByDatabaseFileSize> actual = new List<DataContracts.v1.Widgets.DatabasesByDatabaseFileSize>();
            //actual = target.GetDatabasesByFileSize(count);
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count<DataContracts.v1.Widgets.DatabasesByDatabaseFileSize>() > 0);
            //Assert.IsNotNull(actual.First().DatabaseName);
            //Assert.IsNotNull(actual.First().Severity);
            //Assert.IsNotNull(actual.First().InstanceName);
            //Assert.IsNotNull(actual.First().FileSizeInMB);
            //Assert.IsNotNull(actual.First().Severity);
            //Assert.IsNotNull(actual.First().UTCCollectionDateTime);

        }
        #endregion

        #region GetInstancesByAlerts Tests
        [TestCase]
        public void GetInstancesByAlertsWithStr()
        {
            //WebService target = new WebService();
            //string count = "One";

            //IEnumerable<DataContracts.v1.Widgets.AlertsCountForInstance> actual = new List<DataContracts.v1.Widgets.AlertsCountForInstance>();
            //try
            //{
            //    actual = target.GetInstancesByAlerts(count);
            //}
            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);
        }
        [TestCase]
        public void GetInstancesByAlertsWithInt()
        {
            //WebService target = new WebService();
            //string count = "10";

            //IEnumerable<DataContracts.v1.Widgets.AlertsCountForInstance> actual = new List<DataContracts.v1.Widgets.AlertsCountForInstance>();
            //actual = target.GetInstancesByAlerts(count);
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count<DataContracts.v1.Widgets.AlertsCountForInstance>() > 0);
            //Assert.IsNotNull(actual.First().InstanceId);
            //Assert.IsNotNull(actual.First().MaxSeverity);
            //Assert.IsNotNull(actual.First().InstanceName);
            //Assert.IsNotNull(actual.First().AlertCount);

        }
        #endregion

        #region GetDatabasesByAlerts Tests
        [TestCase]
        public void GetDatabasesByAlertsWithStr()
        {
            //WebService target = new WebService();
            //string count = "One";

            //IEnumerable<DataContracts.v1.Widgets.AlertsCountForDatabase> actual = new List<DataContracts.v1.Widgets.AlertsCountForDatabase>();
            //try
            //{
            //    actual = target.GetDatabasesByAlerts(count);
            //}
            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);
        }
        [TestCase]
        public void GetDatabasesByAlertsWithInt()
        {
            //WebService target = new WebService();
            //string count = "10";

            //IEnumerable<DataContracts.v1.Widgets.AlertsCountForDatabase> actual = new List<DataContracts.v1.Widgets.AlertsCountForDatabase>();
            //actual = target.GetDatabasesByAlerts(count);
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count<DataContracts.v1.Widgets.AlertsCountForDatabase>() > 0);
            //Assert.IsNotNull(actual.First().InstanceId);
            //Assert.IsNotNull(actual.First().MaxSeverity);
            //Assert.IsNotNull(actual.First().InstanceName);
            //Assert.IsNotNull(actual.First().AlertCount);
            //Assert.IsNotNull(actual.First().DatabaseName);

        }
        #endregion


        #region GetInstancesBySqlCpuLoad Tests
        [TestCase]
        public void GetInstancesBySqlCpuLoadWithStr()
        {
            //WebService target = new WebService();
            //string count = "One";

            //IEnumerable<DataContracts.v1.Widgets.SqlCpuLoadForInstance> actual = new List<DataContracts.v1.Widgets.SqlCpuLoadForInstance>();
            //try
            //{
            //    actual = target.GetInstancesBySqlCpuLoad(count);
            //}
            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);
        }

        [TestCase]
        public void GetInstancesBySqlCpuLoadWithInt()
        {
            //WebService target = new WebService();
            //string count = "10";

            //IEnumerable<DataContracts.v1.Widgets.SqlCpuLoadForInstance> actual = new List<DataContracts.v1.Widgets.SqlCpuLoadForInstance>();
            //actual = target.GetInstancesBySqlCpuLoad(count);
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count<DataContracts.v1.Widgets.SqlCpuLoadForInstance>() > 0);
            //Assert.IsNotNull(actual.First().InstanceId);
            //Assert.IsNotNull(actual.First().Severity);
            //Assert.IsNotNull(actual.First().InstanceName);
            //Assert.IsNotNull(actual.First().CPUUsageInPercentage);

        }

        #endregion

        #region GetInstancesByIOPhysicalCount Tests
        [TestCase]
        public void GetInstancesByIOPhysicalCountWithStr()
        {
            //WebService target = new WebService();
            //string count = "One";

            //IEnumerable<DataContracts.v1.Widgets.IOPhysicalUsageForInstance> actual = new List<DataContracts.v1.Widgets.IOPhysicalUsageForInstance>();
            //try
            //{
            //    actual = target.GetInstancesByIOPhysicalCount(count);
            //}
            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);
        }

        [TestCase]
        public void GetInstancesByIOPhysicalCountWithInt()
        {
            //WebService target = new WebService();
            //string count = "10";

            //IEnumerable<DataContracts.v1.Widgets.IOPhysicalUsageForInstance> actual = new List<DataContracts.v1.Widgets.IOPhysicalUsageForInstance>();
            //actual = target.GetInstancesByIOPhysicalCount(count);
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count<DataContracts.v1.Widgets.IOPhysicalUsageForInstance>() > 0);
            //Assert.IsNotNull(actual.First().InstanceId);
            //Assert.IsNotNull(actual.First().Severity);
            //Assert.IsNotNull(actual.First().InstanceName);
            //Assert.IsNotNull(actual.First().SQLPhysicalIO);
            
        }

        #endregion

        #region GetInstancesBySqlMemoryUsage Tests

        [TestCase]
        public void GetInstancesBySqlMemoryUsageWithStr()
        {
            //WebService target = new WebService();
            //string count = "One";

            //IEnumerable<DataContracts.v1.Widgets.SqlMemoryUsageForInstance> actual = new List<DataContracts.v1.Widgets.SqlMemoryUsageForInstance>();
            //try
            //{
            //    actual = target.GetInstancesBySqlMemoryUsage(count);
            //}
            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);
        }

        [TestCase]
        public void GetInstancesBySqlMemoryUsageWithInt()
        {
            //WebService target = new WebService();
            //string count = "10";

            //IEnumerable<DataContracts.v1.Widgets.SqlMemoryUsageForInstance> actual = new List<DataContracts.v1.Widgets.SqlMemoryUsageForInstance>();
            //actual = target.GetInstancesBySqlMemoryUsage(count);
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count<DataContracts.v1.Widgets.SqlMemoryUsageForInstance>() > 0);
            //Assert.IsNotNull(actual.First().InstanceId);
            //Assert.IsNotNull(actual.First().Severity);
            //Assert.IsNotNull(actual.First().InstanceName);
            //Assert.IsNotNull(actual.First().SqlMemoryAllocatedInMB);
            //Assert.IsNotNull(actual.First().SqlMemoryUsedInMB);
            


        }

        #endregion

        #region GetInstancesByWaitStatistics Tests

        [TestCase]
        public void GetInstancesByWaitStatisticsWithStr()
        {
            //WebService target = new WebService();
            //string count = "One";

            //IList<DataContracts.v1.Widgets.WaitStatisticsByInstance> actual = new List<DataContracts.v1.Widgets.WaitStatisticsByInstance>();
            //try
            //{
            //    actual = target.GetInstancesByWaitStatistics(count);
            //}
            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);
        }

        [TestCase]
        public void GetInstancesByWaitStatisticsWithInt()
        {
            //WebService target = new WebService();
            //string count = "10";

            //IList<DataContracts.v1.Widgets.WaitStatisticsByInstance> actual = new List<DataContracts.v1.Widgets.WaitStatisticsByInstance>();
            //actual = target.GetInstancesByWaitStatistics(count);
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count > 0);
            //Assert.IsNotNull(actual.First().Application);
            //Assert.IsNotNull(actual.First().InstanceID);
            //Assert.IsNotNull(actual.First().InstanceName);
            //Assert.IsNotNull(actual.First().WaitTimeInMs);
            //Assert.IsNotNull(actual.First().UTCCollectionDateTime);
            //Assert.IsNotNull(actual.First().WaitType);
            

        }

        #endregion

        #region GetTopDatabasesByGrowth Tests

        [TestCase]
        public void GetTopDatabasesByGrowthWithStr()
        {
            //WebService target = new WebService();
            //string count = "One";
            //int numHistoryDays = 100;

            //IList<DataContracts.v1.Widgets.ProjectedGrowthOfDatabaseSize> actual = new List<DataContracts.v1.Widgets.ProjectedGrowthOfDatabaseSize>();
            //try
            //{
            //    actual = target.GetTopDatabasesByGrowth(count,numHistoryDays);
            //}
            //catch
            //{
            //    Assert.Fail();
            //    return;
            //}
            //Assert.IsNull(actual);
        }


        [TestCase]
        public void GetTopDatabasesByGrowthWithInt()
        {
            //WebService target = new WebService();
            //string count = "10";
            //int numHistoryDays = 100;

            //IList<DataContracts.v1.Widgets.ProjectedGrowthOfDatabaseSize> actual = new List<DataContracts.v1.Widgets.ProjectedGrowthOfDatabaseSize>();
            //actual = target.GetTopDatabasesByGrowth(count, numHistoryDays);
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count > 0);
            //Assert.IsNotNull(actual.First().DatabaseName);
            //Assert.IsNotNull(actual.First().InstanceID);
            //Assert.IsNotNull(actual.First().InstanceName);
            //Assert.IsNotNull(actual.First().TotalSizeDiffernceKb);
            //Assert.IsNotNull(actual.First().UTCCollectionDateTime);
            
        }

        #endregion
    }
}
