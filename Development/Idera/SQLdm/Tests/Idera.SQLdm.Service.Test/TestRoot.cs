using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Diagnostics;
using Idera.SQLdm.Service.Repository;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Service.Web;
using Idera.SQLdm.Service.DataContracts.v1;

namespace Idera.SQLdm.Service.Test
{
    [TestFixture(Category="REST API", Description="Tests For REST API services for Web UI")]
    public abstract class TestRoot
    {
        public static bool isDBconfigurationSet = false;
        public TestRoot()
        {
            //Debugger.Launch();
            if (!isDBconfigurationSet)
            {
                WebService.LoadTestDBConfigurations(@"FREEDOM\FOR_IDERA", "SQLdmRepository");
                isDBconfigurationSet = true;
            }
        }
        
        public int getActiveInstanceId()
        {
            WebService target = new WebService();
            string ActiveOnly = "true";
            string FilterField = string.Empty;
            string FilterValue = string.Empty;
            MonitoredSqlServerCollection actual = target.GetShortInstances(ActiveOnly, FilterField, FilterValue);
            if (actual.Count > 0)
            {
                return actual.First().SQLServerId;
            }
            return 0;
        }

        public long getActiveAlertId()
        {
            //WebService target = new WebService(); 
            //string instancename = null; 
            //int startingAlertId = 0; 
            //int maxRows = 1; 
            //DateTime startDate = new DateTime(); 
            //DateTime endDate = new DateTime();
            //string severity = null;
            //string metricid = null;
            //string category = null; 
            //bool isActive = true;             
            //IList<Alert> actual;
            //actual = target.GetAlerts(instancename, startingAlertId, maxRows, startDate, endDate, severity, metricid, category, isActive);
            //if (actual.Count > 0)
            //{
            //    return actual.First().AlertId;
            //}
            return 0;
        }

        public int getActiveMetricId()
        {
            //WebService target = new WebService();
            //string instancename = null;
            //int startingAlertId = 0;
            //int maxRows = 1;
            //DateTime startDate = new DateTime();
            //DateTime endDate = new DateTime();
            //string severity = null;
            //string metricid = null;
            //string category = null;
            //bool isActive = true;
            //IList<Alert> actual;
            //actual = target.GetAlerts(instancename, startingAlertId, maxRows, startDate, endDate, severity, metricid, category, isActive);
            //if (actual.Count > 0)
            //{
            //    return actual.First().Metric.MetricId;
            //}
            return 0;
        }



    }
}
