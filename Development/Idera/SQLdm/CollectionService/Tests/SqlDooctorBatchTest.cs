using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Data.SqlClient;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.CollectionService.OnDemandClient;
using Idera.SQLdm.CollectionService.Monitoring;
using Idera.SQLdm.CollectionService.Probes.Sql.Batches;
using Microsoft.SqlServer.Management.Smo;

namespace Idera.SQLdm.CollectionService.Tests
{
    public enum MontioredServerType
    {
        Fresh,
        Repository
    }

    [TestFixture(Category = "SQLdmCollectionService", Description = "Contains test cases for collection service ")]
    public partial class SqlDoctorBatchTest
    {
        public MonitoredServerWorkload _workload;
        public SqlConnectionInfo _sqlConnection = new SqlConnectionInfo(); // This holds the connection to the repository
        public OnDemandCollectionManager col = new Monitoring.OnDemandCollectionManager();
        public MonitoredSqlServer monitoredServer;  //monitored server
        public int monitoredServerId = 2;
        public MontioredServerType serverType = MontioredServerType.Fresh;
        public string monitoredServerName = "ACCOLITE-PC";
        public string monitoredDatabaseName = "SQLdmRepository";

        [SetUp]
        public void Setup()
        {
            //connection string for repo
            _sqlConnection.InstanceName = "ACCOLITE-PC";
            _sqlConnection.DatabaseName = "SQLdmRepository";
            _sqlConnection.UseIntegratedSecurity = true;

            //set default instance name
            Idera.SQLdm.CollectionService.Configuration.CollectionServiceConfiguration.InstanceName = "Default";

            //Set the server to be monitored
            if (serverType == MontioredServerType.Fresh)
                SetFreshMonitoredServer();
            else
                SetMonitoredserversFromRepository();
        }

        public void SetFreshMonitoredServer()
        {
            MonitoredSqlServerConfiguration config = new MonitoredSqlServerConfiguration(new SqlConnectionInfo(monitoredServerName, monitoredDatabaseName));
            config.MostRecentSQLVersion = new ServerVersion("12.0.2000");
            monitoredServer = new MonitoredSqlServer(monitoredServerId, DateTime.Now, config);
            monitoredServer.MostRecentSQLVersion = config.MostRecentSQLVersion;
            col.AddMonitoredServer(monitoredServer);
        }

        public void SetMonitoredserversFromRepository()
        {
            //Get sql sever to monitor
            monitoredServer = TestCaseHelper.GetMonitoredSqlServer(_sqlConnection.ConnectionString, monitoredServerId);
            _workload = TestCaseHelper.GetMonitoredServerWorkload(monitoredServer, _sqlConnection.ConnectionString);//This is not required for on demand collection

            //Add monitored server to on demand collection context
            col.AddMonitoredServer(monitoredServer);
        }
    }
}
