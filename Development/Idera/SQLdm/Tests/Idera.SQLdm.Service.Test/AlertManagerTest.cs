using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdm.Service.Web;
using Idera.SQLdm.Service.DataContracts.v1;
using Idera.SQLdm.Service;
using NUnit.Framework;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Service.Test
{
    class AlertManagerTest : TestRoot
    {
        /// <summary>
        ///A test for GetAlertDetails
        ///</summary>
        [TestCase]
        public void GetAlertDetailsTest()
        {
            WebService target = new WebService(); 
            string alertId = getActiveAlertId().ToString();
            Alert actual;

            //START SQLdm 10.0 (Sanjali Makkar) : Adding the argument of TimeZoneOffset
            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);
            string timeZoneOffset = '-' + offset.TotalHours.ToString();

            actual = target.GetAlertDetails(alertId, timeZoneOffset);
            //END SQLdm 10.0 (Sanjali Makkar) : Adding the argument of TimeZoneOffset
            
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.AlertId);
            Assert.IsNotNull(actual.ServerName);
            Assert.IsNotNull(actual.Metric);
            Assert.IsNotNull(actual.Metric.MetricId);
            Assert.IsNotNull(actual.Metric.MetricCategory);
            Assert.IsNotNull(actual.Heading);
            Assert.IsNotNull(actual.Message);            
        }      

        /// <summary>
        ///A test for GetAlerts
        ///</summary>
        [TestCase]
        public void GetAlertsMaxRowTest()
        {
            //WebService target = new WebService();
            //string instancename = null;
            //int startingAlertId = 0;
            //int maxRows = 3;
            //DateTime startDate = new DateTime();
            //DateTime endDate = new DateTime();
            //string severity = null;
            //string metricid = null;
            //string category = null;
            //bool isActive = true;            
            //var actual = target.GetAlerts(instancename, startingAlertId, maxRows, startDate, endDate, severity, metricid, category, isActive);            
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count == maxRows);           
        }

        /// <summary>
        ///A test for GetAlerts
        ///</summary>
        [TestCase]
        public void GetAlertsMetricTest()
        {
            //WebService target = new WebService();
            //string instancename = null;
            //int startingAlertId = 0;
            //int maxRows = 3;
            //DateTime startDate = new DateTime();
            //DateTime endDate = new DateTime();
            //string severity = null;
            //string metricid = getActiveMetricId().ToString();
            //string category = null;
            //bool isActive = true;
            //var actual = target.GetAlerts(instancename, startingAlertId, maxRows, startDate, endDate, severity, metricid, category, isActive);
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count == maxRows);
            //Assert.IsNotNull(actual.First().Metric);
            //Assert.IsTrue(actual.First().Metric.MetricId.ToString().Equals(metricid));
        }

        /// <summary>
        ///A test for GetAlerts
        ///</summary>
        [TestCase]
        public void GetAlertsCategoryTest()
        {
            //WebService target = new WebService();
            //string instancename = null;
            //int startingAlertId = 0;
            //int maxRows = 3;
            //DateTime startDate = new DateTime();
            //DateTime endDate = new DateTime();
            //string severity = null;
            //string metricid = null;
            //string category = Idera.SQLdm.Common.Events.MetricCategory.Services.ToString();
            //bool isActive = true;
            //var actual = target.GetAlerts(instancename, startingAlertId, maxRows, startDate, endDate, severity, metricid, category, isActive);
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count == maxRows);
            //Assert.IsNotNull(actual.First().Metric);
            //Assert.IsTrue(actual.First().Metric.MetricCategory.ToString().Equals(category));
        }

        /// <summary>
        ///A test for GetTopServersAlerts
        ///</summary>
        [TestCase]
        public void GetTopServersAlertsTest()
        {
            //WebService target = new WebService(); 
            //DateTime startDate = new DateTime(); 
            //DateTime endDate = new DateTime();             
            //MonitoredSqlServerCollection actual;
            //actual = target.GetTopServersAlerts(startDate, endDate);
            //Assert.IsNotNull(actual);
            //Assert.IsTrue(actual.Count > 0);
            //Assert.IsNotNull(actual.First().SQLServerId);
            //Assert.IsNotNull(actual.First().InstanceName);
            //Assert.IsNotNull(actual.First().Databases);
            //Assert.IsTrue(actual.First().Databases.Count > 0);
            //Assert.IsNotNull(actual.First().Databases.First().DatabaseName);
            //Assert.IsNotNull(actual.First().Databases.First().TotalAlertCount);
        }
    }
}
