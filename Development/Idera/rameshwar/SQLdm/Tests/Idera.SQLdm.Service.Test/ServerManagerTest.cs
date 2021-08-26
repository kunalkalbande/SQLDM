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
    public class ServerManagerTest: TestRoot
    {

        #region GetTags Test
        [TestCase]
        public void GetTagsTest()
        {
            WebService target = new WebService();
            TagsCollection actual;
            actual = target.GetTags();
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 0);
            Assert.IsNotNull(actual.FirstOrDefault());
            Assert.IsNotNull(actual.First().Id);
            Assert.IsNotNull(actual.First().Name);
        } 
        #endregion

        #region GetShortInstances Tests
        [TestCase]
        public void GetShortInstancesTestAllBlank(string activeOnly)
        {
            WebService target = new WebService();
            string FilterField = string.Empty;
            string FilterValue = string.Empty;
            MonitoredSqlServerCollection actual = target.GetShortInstances(activeOnly, FilterField, FilterValue);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 0);
        }

        [TestCase]
        public void GetShortInstancesTestAllNull(string activeOnly)
        {
            WebService target = new WebService();
            string FilterField =null;
            string FilterValue =null;
            MonitoredSqlServerCollection actual = target.GetShortInstances(activeOnly, FilterField, FilterValue);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 0);
        }

        [TestCase]
        public void GetShortInstancesTestWithBlankField(string activeOnly)
        {
            WebService target = new WebService();
            string FilterField = "";
            string FilterValue = "Test";
            MonitoredSqlServerCollection actual = target.GetShortInstances(activeOnly, FilterField, FilterValue);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 0);
        }

        [TestCase]
        public void GetShortInstancesTestWithOnlyFieldTag(string activeOnly)
        {
            WebService target = new WebService();
            string FilterField = "TagId";
            string FilterValue = "";
            MonitoredSqlServerCollection actual = target.GetShortInstances(activeOnly, FilterField, FilterValue);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 0);
        }

        [TestCase]
        public void GetShortInstancesTestWithOnlyFieldInstance(string activeOnly)
        {
            WebService target = new WebService();
            string FilterField = "InstanceName";
            string FilterValue = "";
            MonitoredSqlServerCollection actual = target.GetShortInstances( activeOnly, FilterField, FilterValue);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 0);
        }

        [TestCase]
        public void GetShortInstancesTestWithInstnace(string activeOnly)
        {
            WebService target = new WebService();
            string FilterField = "InstanceName";
            string FilterValue = "MSSQLSERVER12";
            MonitoredSqlServerCollection actual = target.GetShortInstances(activeOnly, FilterField, FilterValue);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 0);
        } 

        #endregion

        #region GetShortInstanceDetails Test

        [TestCase]
        public void GetShortInstanceDetailsTestWithNull()
        {
            WebService target = new WebService();
            string InstanceId = null;
            MonitoredSqlServer actual = target.GetShortInstanceDetails(InstanceId);
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.SQLServerId);
            Assert.IsNotNull(actual.InstanceName);
        }

        [TestCase]
        public void GetShortInstanceDetailsTestWithBlank()
        {
            WebService target = new WebService();
            string InstanceId = String.Empty;
            MonitoredSqlServer actual = target.GetShortInstanceDetails(InstanceId);
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.SQLServerId);
            Assert.IsNotNull(actual.InstanceName);
        }

        [TestCase]
        public void GetShortInstanceDetailsTestWithActiveId()
        {
            WebService target = new WebService();
            string InstanceId = getActiveInstanceId().ToString();
            MonitoredSqlServer actual = target.GetShortInstanceDetails(InstanceId);
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.SQLServerId);
            Assert.IsNotNull(actual.InstanceName);
        }

        [TestCase]
        public void GetShortInstanceDetailsTestWithStr()
        {
            WebService target = new WebService();
            string InstanceId = "test";
            MonitoredSqlServer actual = target.GetShortInstanceDetails(InstanceId);
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.SQLServerId);
            Assert.IsNotNull(actual.InstanceName);
        }

        [TestCase]
        public void GetShortInstanceDetailsTestWithWrongId()
        {
            WebService target = new WebService();
            string InstanceId = "4";
            MonitoredSqlServer actual = target.GetShortInstanceDetails(InstanceId);
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.SQLServerId);
            Assert.IsNotNull(actual.InstanceName);
        } 
        #endregion

        #region GetTopServerResponseTime Tests
        [TestCase]
        public void GetTopServerResponseTimeTestNull()
        {
            //WebService target = new WebService();
            //string count = null;
            //var actual = target.GetTopInstanceByResponseTime(count);
            //Assert.IsNotNull(actual);
            //Assert.IsNotNull(actual.FirstOrDefault());
            //Assert.IsNotNull(actual.First().ResponseTimeMillis);
            //Assert.IsNotNull(actual.First().InstanceId);
            //Assert.IsNotNull(actual.First().InstanceName);
            //Assert.IsNotNull(actual.First().UTCCollectionDateTime);
        }

        [TestCase]
        public void GetTopServerResponseTimeTestEmpty()
        {
            //WebService target = new WebService();
            //string count = String.Empty;
            //var actual = target.GetTopInstanceByResponseTime(count);
            //Assert.IsNotNull(actual);
            //Assert.IsNotNull(actual.FirstOrDefault());
            //Assert.IsNotNull(actual.First().ResponseTimeMillis);
            //Assert.IsNotNull(actual.First().InstanceId);
            //Assert.IsNotNull(actual.First().InstanceName);
            //Assert.IsNotNull(actual.First().UTCCollectionDateTime);
        }

        [TestCase]
        public void GetTopServerResponseTimeTestInt()
        {
            //WebService target = new WebService();
            //string count = "10";
            //var actual = target.GetTopInstanceByResponseTime(count);
            //Assert.IsNotNull(actual);
            //Assert.IsNotNull(actual.FirstOrDefault());
            //Assert.IsNotNull(actual.First().ResponseTimeMillis);
            //Assert.IsNotNull(actual.First().InstanceId);
            //Assert.IsNotNull(actual.First().InstanceName);
            //Assert.IsNotNull(actual.First().UTCCollectionDateTime);
        }

        [TestCase]
        public void GetTopServerResponseTimeTestStr()
        {
            //WebService target = new WebService();
            //string count = "Ten";
            //var actual = target.GetTopInstanceByResponseTime(count);
            //Assert.IsNotNull(actual);
            //Assert.IsNotNull(actual.FirstOrDefault());
            //Assert.IsNotNull(actual.First().ResponseTimeMillis);
            //Assert.IsNotNull(actual.First().InstanceId);
            //Assert.IsNotNull(actual.First().InstanceName);
            //Assert.IsNotNull(actual.First().UTCCollectionDateTime);
        } 
        #endregion

        #region GetServerStatisticsHistory Tests
        [TestCase]
        public void GetServerStatisticsHistoryTestWithActive(int numOfMinutes)
        {
            WebService target = new WebService();
            string InstanceId = getActiveInstanceId().ToString();
            MonitoredSqlServer actual;
            actual = GetServerStatisticsHistoryTest(target, InstanceId, numOfMinutes);
        }

        [TestCase]
        public void GetServerStatisticsHistoryTestWithNull(int numOfMinutes)
        {
            WebService target = new WebService();
            string InstanceId = null;
            MonitoredSqlServer actual;
            actual = GetServerStatisticsHistoryTest(target, InstanceId, numOfMinutes);
        }

        [TestCase]
        public void GetServerStatisticsHistoryTestWithEmpty(int numOfMinutes)
        {
            WebService target = new WebService();
            string InstanceId = String.Empty;
            MonitoredSqlServer actual;
            actual = GetServerStatisticsHistoryTest(target, InstanceId, numOfMinutes);
        }

        private static MonitoredSqlServer GetServerStatisticsHistoryTest(WebService target, string InstanceId, int numOfMinutes)
        {
            MonitoredSqlServer actual;

            //START SQLdm 10.0 (Sanjali Makkar) : Adding the argument of TimeZoneOffset
            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
            string timeZoneOffset = '-' + offset.TotalHours.ToString();

            //SQLdm 10.2 (Anshika Sharma) : Added DateTime.Now to the function below for testing purpose
            actual = target.GetServerStatisticsHistory(InstanceId, timeZoneOffset, numOfMinutes, DateTime.Now);
            //END SQLdm 10.0 (Sanjali Makkar) : Adding the argument of TimeZoneOffset

            
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.SQLServerId);
            Assert.IsNotNull(actual.StatisticsHistory);
            Assert.IsTrue(actual.StatisticsHistory.Count > 0);
            Assert.IsNotNull(actual.StatisticsHistory.First());
            Assert.IsNotNull(actual.StatisticsHistory.First().CPUActivityPercentage);
            Assert.IsNotNull(actual.StatisticsHistory.First().DiskTimePercent);
            Assert.IsNotNull(actual.StatisticsHistory.First().IdlePercentage);
            Assert.IsNotNull(actual.StatisticsHistory.First().OSAvailableMemoryInKilobytes);
            Assert.IsNotNull(actual.StatisticsHistory.First().OSTotalPhysicalMemoryInKilobytes);
            Assert.IsNotNull(actual.StatisticsHistory.First().ReponseTimeinMilliSeconds);
            Assert.IsNotNull(actual.StatisticsHistory.First().SqlMemoryAllocatedInKilobytes);
            Assert.IsNotNull(actual.StatisticsHistory.First().SqlMemoryUsedInKilobytes);
            Assert.IsNotNull(actual.StatisticsHistory.First().UTCCollectionDateTime);
            return actual;
        } 
        #endregion

        #region GetInstancesWithSeverity Test
        [TestCase]
        //public void GetInstancesWithSeverityTest()
        //{
        //    WebService target = new WebService();
        //    MonitoredSqlServerStatusCollection actual = null;
        //    actual = target.GetInstancesWithSeverity();
        //    Assert.IsNotNull(actual);
        //    Assert.IsTrue(actual.Count > 0);
        //    Assert.IsNotNull(actual.First());
        //    Assert.IsNotNull(actual.First().Severity);
        //    Assert.IsNotNull(actual.First().SqlServerCollection);
        //    Assert.IsTrue(actual.First().SqlServerCollection.Count > 0);
        //    Assert.IsNotNull(actual.First().SqlServerCollection.First());
        //    Assert.IsNotNull(actual.First().SqlServerCollection.First().SQLServerId);
        //    Assert.IsNotNull(actual.First().SqlServerCollection.First().InstanceEdition);
        //    Assert.IsNotNull(actual.First().SqlServerCollection.First().InstanceName);
        //} 
        #endregion

        #region GetInstances Tests
        [TestCase]
        public void GetInstancesTestCriticalByInstance(bool activeOnly)
        {
            WebService target = new WebService();
            ServerSummaryContainerCollection actual;
            string severity = "Critical";
            string strOrderBy = "InstanceName";
            actual = GetInstancesTest(activeOnly, target, severity, strOrderBy);
        }

        [TestCase]
        public void GetInstancesTestNullByInstance(bool activeOnly)
        {
            WebService target = new WebService();
            ServerSummaryContainerCollection actual;
            string severity = null;
            string strOrderBy = "InstanceName";
            actual = GetInstancesTest(activeOnly, target, severity, strOrderBy);
        }

        [TestCase]
        public void GetInstancesTestEmptyByInstance(bool activeOnly)
        {
            WebService target = new WebService();
            ServerSummaryContainerCollection actual;
            string severity = string.Empty;
            string strOrderBy = "InstanceName";
            actual = GetInstancesTest(activeOnly, target, severity, strOrderBy);
        }

        [TestCase]
        public void GetInstancesTestCriticalByNull(bool activeOnly)
        {
            WebService target = new WebService();
            ServerSummaryContainerCollection actual;
            string severity = "Critical";
            string strOrderBy = null;
            actual = GetInstancesTest(activeOnly, target, severity, strOrderBy);
        }

        [TestCase]
        public void GetInstancesTestCriticalByEmpty(bool activeOnly)
        {
            WebService target = new WebService();
            ServerSummaryContainerCollection actual;
            string severity = "Critical";
            string strOrderBy = string.Empty;
            actual = GetInstancesTest(activeOnly, target, severity, strOrderBy);
        }

        private static ServerSummaryContainerCollection GetInstancesTest(bool activeOnly, WebService target, string severity, string strOrderBy)
        {
            ServerSummaryContainerCollection actual;

            actual = target.GetInstances();
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Count > 0);
            Assert.IsNotNull(actual.First());
            Assert.IsNotNull(actual.First().Overview);
            Assert.IsNotNull(actual.First().ServerStatus);
            return actual;
        }       
        #endregion


         [TestCase]
        //SQLdm 8.5 (Gaurav): Testing GetDatabaseOverviewMethod
        public void GetDatabasesByInstanceTest()
        {
             WebService target = new WebService();
             string instanceId = getActiveInstanceId().ToString();
             int systemDbCount = 4;
             IList<DataContracts.v1.Database.MonitoredSqlServerDatabase> actual = null;

             //START SQLdm 10.0 (Sanjali Makkar) : Adding the argument of TimeZoneOffset
             TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
             string timeZoneOffset = '-' + offset.TotalHours.ToString();

             actual = target.GetDatabasesByInstance(instanceId, timeZoneOffset);
             //END SQLdm 10.0 (Sanjali Makkar) : Adding the argument of TimeZoneOffset
             
             Assert.IsNotNull(actual);
             Assert.Greater(actual.Count, 0);
             Assert.IsTrue(actual.Count(x => x.IsSystemDatabase == true) >= systemDbCount);
             Assert.IsTrue(actual.Count > systemDbCount);            
        }

         //[TestCase]
         //public void GetLatestResponseTimesByInstanceTest()
         //{
         //    WebService target = new WebService();
         //    string instanceId = getActiveInstanceId().ToString();
         //    IList<DataContracts.v1.Widgets.ResponseTimeForInstance> actual = null;
         //    actual = target.GetLatestResponseTimesByInstance();
         //    Assert.IsNotNull(actual);
         //    Assert.Greater(actual.Count, 0);
         //    Assert.IsNotNullOrEmpty(actual.First().InstanceName);
         //    Assert.Greater(actual.First().InstanceId ,0);
         //    Assert.IsNotNull(actual.First().Severity);
         //    Assert.IsNotNull(actual.First().ResponseTimeMillis);
         //    Assert.IsNotNull(actual.First().UTCCollectionDateTime);
         //    Assert.IsNotNull(actual.First());
         //}
    }
}
