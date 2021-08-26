using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdm.Service.Web;
using Idera.SQLdm.Service.DataContracts.v1;
using Idera.SQLdm.Service;
using NUnit.Framework;


namespace Idera.SQLdm.Service.Test
{
    class WebServiceTest  :TestRoot
    {
        /// <summary>
        ///A test for GetVersion
        ///</summary>
        [TestCase]
        public void GetVersionTest()
        {
            WebService target = new WebService();             
            string actual;
            actual = target.GetVersion();
            Assert.IsNotNullOrEmpty(actual);            
        }

        /// <summary>
        ///A test for ForceCleanup
        ///</summary>
        [TestCase] 
        public void ForceCleanupTest()
        {
            WebService target = new WebService();
            target.ForceCleanup();
            Assert.IsTrue(true);
        }

        /// <summary>
        ///A test for GetServiceStatus
        ///</summary>
        [TestCase]
        public void GetServiceStatusTest()
        {
            WebService target = new WebService();
            GetServiceStatusResponse actual;
            actual = target.GetServiceStatus();
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.CanRun.HasValue ? actual.CanRun.Value : false);
            Assert.IsNotNullOrEmpty(actual.Version);            
        }        
    }
}
