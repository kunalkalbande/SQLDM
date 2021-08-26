using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Data.SqlClient;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    //SQLdm 9.0 (Ankit Srivastava) - Query Monitoring with Extended Events - new class for unit test cases for three methods when passed NULL arguements
    [TestFixture(Category = "SQLdmCollectionService", Description = "Contains test cases for collection service ")]
    class SqlCommandBuilderEX_Test
    {
        [TestCase]
        public void BuildQueryMonitorStartCommandEXTest()
        {
            SqlConnection sqlConn = new SqlConnection();
            ServerVersion version = new ServerVersion("11.00.2100");
            StartQueryMonitorTraceConfiguration qmConfig = null;
            ActiveWaitsConfiguration waitConfig = null;
            SqlCommand sqlCommActual = SqlCommandBuilder.BuildQueryMonitorStartCommandEX(sqlConn, version, qmConfig, waitConfig);
            Assert.IsNotNull(sqlCommActual);
            
            Assert.IsEmpty(sqlCommActual.CommandText);

        }

        [TestCase]
        public void BuildQueryMonitorEXCommandTest()
        {
            SqlConnection sqlConn = new SqlConnection();
            ServerVersion version = new ServerVersion("11.00.2100");
            QueryMonitorConfiguration config = null;
            QueryMonitorConfiguration prevConfig = null;
            ActiveWaitsConfiguration waitConfig = null;
            SqlCommand sqlCommActual = SqlCommandBuilder.BuildQueryMonitorEXCommand(sqlConn, version,config,prevConfig, waitConfig,DateTime.Now,true);
            Assert.IsNotNull(sqlCommActual);

            Assert.IsEmpty(sqlCommActual.CommandText);

        }

        [TestCase]
        public void BuildQueryMonitorEXCommandWithRestartTest()
        {
            SqlConnection sqlConn = new SqlConnection();
            ServerVersion version = new ServerVersion("11.00.2100");
            QueryMonitorConfiguration config = null;
            ActiveWaitsConfiguration waitConfig = null;
            SqlCommand sqlCommActual = SqlCommandBuilder.BuildQueryMonitorEXCommandWithRestart(sqlConn, version, config, waitConfig,DateTime.Now);
            Assert.IsNotNull(sqlCommActual);

            Assert.IsEmpty(sqlCommActual.CommandText);

        }


        //START SQLdm 9.1(Ankit Srivastava) NUNit Test Cases
        [TestCase]
        public void BuildActivityMonitorCommandEXTest()
        {
            SqlConnection sqlConn = new SqlConnection();
            ServerVersion version = new ServerVersion("11.00.2100");
            ActivityMonitorConfiguration amConfig = null;
            ActivityMonitorConfiguration amConfigPrev = null;
            SqlCommand sqlCommActual = SqlCommandBuilder.BuildActivityMonitorCommandEX(sqlConn, version, amConfig, amConfigPrev, null);
            Assert.IsNotNull(sqlCommActual);

            Assert.IsEmpty(sqlCommActual.CommandText);
        }

        [TestCase]
        public void BuildActivityMonitorStartCommandEXTest()
        {
            SqlConnection sqlConn = new SqlConnection();
            ServerVersion version = new ServerVersion("11.00.2100");
            StartActivityMonitorTraceConfiguration amConfig = null;
            SqlCommand sqlCommActual = SqlCommandBuilder.BuildActivityMonitorStartCommandEX(sqlConn, version, amConfig, null);
            Assert.IsNotNull(sqlCommActual);

            Assert.IsEmpty(sqlCommActual.CommandText);
        }

        [TestCase]
        public void BuildActivityMonitorCommandWithRestartEXTest()
        {
            SqlConnection sqlConn = new SqlConnection();
            ServerVersion version = new ServerVersion("11.00.2100");
            ActivityMonitorConfiguration config = null;
            SqlCommand sqlCommActual = SqlCommandBuilder.BuildActivityMonitorCommandWithRestartEX(sqlConn, version,config, null );
            Assert.IsNotNull(sqlCommActual);

            Assert.IsEmpty(sqlCommActual.CommandText);
        }
		//END SQLdm 9.1 (Ankit Srivastava) -- NUnit Test cases

    }
}
