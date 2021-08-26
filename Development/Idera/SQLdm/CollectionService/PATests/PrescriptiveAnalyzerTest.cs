using System;
using System.Collections.Generic;
//using System.Linq;
using NUnit.Framework;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.CollectionService.Monitoring;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.CollectionService.OnDemandClient;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.AdHoc;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Snapshots;

namespace Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Tests
{
    public enum MontioredServerType
    {
        Fresh,
        Repository
    }

    [TestFixture(Category = "SQLdmCollectionService", Description = "Contains test cases for collection service ")]
    public partial class PrescriptiveAnalyzerTest
    {
        public MonitoredServerWorkload _workload;
        public Idera.SQLdm.Common.Configuration.SqlConnectionInfo _sqlConnection = new Idera.SQLdm.Common.Configuration.SqlConnectionInfo(); // This holds the connection to the repository
        public OnDemandCollectionManager col = new Idera.SQLdm.CollectionService.Monitoring.OnDemandCollectionManager();
        public MonitoredSqlServer monitoredServer;  //monitored server
        public int monitoredServerId = 2;
        public MontioredServerType serverType = MontioredServerType.Fresh;
        public string monitoredServerName = "ACCOLITE-PC";
        public string monitoredDatabaseName = "SQLdmRepository";
        public PAPrescriptiveAnalyticsSnapshot snap;

        [SetUp]
        public void Setup()
        {
            //connection string for repo
            _sqlConnection.InstanceName = "Vineet-Accolite\\SS2014";
            _sqlConnection.DatabaseName = "SQLdmRepository1";
            _sqlConnection.UseIntegratedSecurity = true;

            //set default instance name
            Idera.SQLdm.CollectionService.Configuration.CollectionServiceConfiguration.InstanceName = "Default";

            //Set the server to be monitored
            if (serverType == MontioredServerType.Fresh)
                SetFreshMonitoredServer();
            else
                SetMonitoredserversFromRepository();

            List<string> dbNames = new List<string>();
            OnDemandCollectionContext<DatabaseNamesSnapshot> c0 = new OnDemandCollectionContext<DatabaseNamesSnapshot>();
            col.CollectDatabaseNames(monitoredServerId, c0, null);
            var dbsnapshot = ((DatabaseNamesSnapshot)c0.Wait());

            List<PASnapshot> lstSnapshot = new List<PASnapshot>();

            OnDemandCollectionContext<PADatabaseRankingSnapshot> c = new OnDemandCollectionContext<PADatabaseRankingSnapshot>();
            col.CollectDatabaseRanking(monitoredServerId, c, null);
            lstSnapshot.Add((PADatabaseRankingSnapshot)c.Wait());

            AnalysisConfiguration config = new AnalysisConfiguration(1);
            config.IncludeDatabaseName = "SQLdmRepository";
            config.BlockedDatabases = new Dictionary<int, string>();
            config.BlockedDatabases.Add(1, "Test");
            config.BlockedDatabases.Add(2, "master");
            config.BlockedDatabases.Add(3, "model");
            config.BlockedDatabases.Add(4, "msdb");
            config.BlockedDatabases.Add(5, "tempdb");

            List<string> dbs = new List<string>();
            foreach (string db in dbsnapshot.Databases.Values)
            {
                if (config.IncludeDatabaseName != null)
                {
                    if (db == config.IncludeDatabaseName)
                    { dbs.Add(db); break; }

                }
                else
                {
                    if (config.BlockedDatabases != null)
                    {
                        if (!config.BlockedDatabases.ContainsValue(db)) dbs.Add(db);
                    }
                    else
                    {
                        dbs.Add(db);
                    }
                }
            }

            foreach (string db in dbs)
            {
                OnDemandCollectionContext<PASQLModuleOptionsSnapshot> c1 = new OnDemandCollectionContext<PASQLModuleOptionsSnapshot>();
                col.CollectSqlModuleOptions(monitoredServerId, c1, null, db);
                lstSnapshot.Add((PASQLModuleOptionsSnapshot)c1.Wait());

                OnDemandCollectionContext<PAOverlappingIndexesSnapshot> c2 = new OnDemandCollectionContext<PAOverlappingIndexesSnapshot>();
                col.CollectOverlappingIndexes(monitoredServerId, c2, null, db);
                lstSnapshot.Add((PAOverlappingIndexesSnapshot)c2.Wait());

                OnDemandCollectionContext<PAOutOfDateStatsSnapshot> c3 = new OnDemandCollectionContext<PAOutOfDateStatsSnapshot>();
                col.CollectOutOfDateStats(monitoredServerId, c3, null, db);
                lstSnapshot.Add((PAOutOfDateStatsSnapshot)c3.Wait());

                OnDemandCollectionContext<PAIndexContentionSnapshot> c4 = new OnDemandCollectionContext<PAIndexContentionSnapshot>();
                col.CollectIndexContention(monitoredServerId, c4, null, db);
                lstSnapshot.Add((PAIndexContentionSnapshot)c4.Wait());

                OnDemandCollectionContext<PAFragmentedIndexesSnapshot> c5 = new OnDemandCollectionContext<PAFragmentedIndexesSnapshot>();
                col.CollectFragmentedIndexes(monitoredServerId, c5, null, db);
                lstSnapshot.Add((PAFragmentedIndexesSnapshot)c5.Wait());

                OnDemandCollectionContext<PADisabledIndexesSnapshot> c6 = new OnDemandCollectionContext<PADisabledIndexesSnapshot>();
                col.CollectDisabledIndexes(monitoredServerId, c6, null, db);
                lstSnapshot.Add((PADisabledIndexesSnapshot)c6.Wait());

                OnDemandCollectionContext<PADBSecuritySnapshot> c7 = new OnDemandCollectionContext<PADBSecuritySnapshot>();
                col.CollectDBSecurity(monitoredServerId, c7, null, db);
                lstSnapshot.Add((PADBSecuritySnapshot)c7.Wait());

                OnDemandCollectionContext<PABackupAndRecoverySnapshot> c8 = new OnDemandCollectionContext<PABackupAndRecoverySnapshot>();
                col.CollectBackupAndRecovery(monitoredServerId, c8, null, db);
                lstSnapshot.Add((PABackupAndRecoverySnapshot)c8.Wait());

                OnDemandCollectionContext<PADatabaseConfigurationSnapshot> c9 = new OnDemandCollectionContext<PADatabaseConfigurationSnapshot>();
                // col.CollectDatabaseConfiguration(monitoredServerId, c9, null);
                snap.DatabaseConfigurationSnapshotValue = (PADatabaseConfigurationSnapshot)c9.Wait();
            }

            snap = new PAPrescriptiveAnalyticsSnapshot(null, null, null, lstSnapshot);

        }

        public void SetFreshMonitoredServer()
        {
            MonitoredSqlServerConfiguration config = new MonitoredSqlServerConfiguration(new Idera.SQLdm.Common.Configuration.SqlConnectionInfo(monitoredServerName, monitoredDatabaseName));
            config.MostRecentSQLVersion = new Idera.SQLdm.Common.ServerVersion("12.0.2000");
            monitoredServer = new MonitoredSqlServer(monitoredServerId, DateTime.Now, config);
            monitoredServer.MostRecentSQLVersion = config.MostRecentSQLVersion;
            col.AddMonitoredServer(monitoredServer);
        }

        public void SetMonitoredserversFromRepository()
        {
            ////Get sql sever to monitor
            //monitoredServer = TestCaseHelper.GetMonitoredSqlServer(_sqlConnection.ConnectionString, monitoredServerId);
            //_workload = TestCaseHelper.GetMonitoredServerWorkload(monitoredServer, _sqlConnection.ConnectionString);//This is not required for on demand collection

            ////Add monitored server to on demand collection context
            //col.AddMonitoredServer(monitoredServer);
        }

        [TestCase]
        public void AnalysisEnginTest()
        {
            //SQLdm.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration.PASqlConnectionInfo connInfo = new Common.Configuration.SqlConnectionInfo();
            //connInfo.InstanceName = "sasa";
            int sqlServerID = 1;
            PAAnalysisConfiguration config = new PAAnalysisConfiguration(sqlServerID);
            //config.IncludeDatabaseName = "SQLdmRepository";
            config.BlockedRecommendationID = new List<string>();
            config.BlockedRecommendationID.Add("SDR-R3");
            config.BlockedCategories = new Dictionary<int, string>();
            config.BlockedCategories.Add(1, "Index Optimization");
            RecommendationEngine engine = new RecommendationEngine(config, snap);
            engine.Run();
            Assert.IsTrue(engine.Results.TotalRecommendationCount > 0);
            Assert.IsTrue(engine.Recommendations.Count > 0);
            string s = engine.Recommendations[0].FindingText;
        }

        [TestCase]
        public void AdhocBatchAnalysisTest()
        {
            //SQLdm.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration.PASqlConnectionInfo connInfo = new Common.Configuration.SqlConnectionInfo();
            //connInfo.InstanceName = "sasa";
            int sqlServerID = 1;
            PAAnalysisConfiguration config = new PAAnalysisConfiguration(sqlServerID);
            config.IncludeDatabaseName = "SQLdmRepository";
            snap.ConnectionString = "Data Source=ACCOLITE-PC;Initial Catalog=Test;Integrated Security=SSPI;";
            RecommendationEngine engine = new RecommendationEngine(config, snap);

            //AdHocBatch b = new AdHocBatch("select DatabaseName from Alerts where DatabaseName = 'tempdb';", 1, 5);
            PAAdHocBatch b = new PAAdHocBatch("select * from Alerts where DatabaseName = 'tempdb';", 1, 5);
            PAAdHocBatches bs = new PAAdHocBatches();
            bs.Add(b);

            engine.RunAdHocBatchAnalysis(bs);//, queryPlanXMl);
            Assert.IsTrue(engine.Recommendations.Count > 0);
        }

        [TestCase]
        public void WorkLoadAnalysisTest()
        {
            int sqlServerID = 1;
            PAAnalysisConfiguration config = new PAAnalysisConfiguration(sqlServerID);
            config.AnalysisDuration = 2;
            config.BlockedDatabases = new Dictionary<int, string>();
            config.BlockedDatabases.Add(1, "master");
            config.BlockedDatabases.Add(4, "msdb");
            config.BlockedDatabases.Add(3, "model");
            //config.IncludeDatabaseName = "SQLdmRepository";
            snap.ConnectionString = "Data Source=ACCOLITE-PC;Initial Catalog=Test;Integrated Security=SSPI;";
            snap.MachineName = "ACCOLITE-PC";
            RecommendationEngine engine = new RecommendationEngine(config, snap);
            //engine.RunWorkLoadAnalysis();
            Assert.IsTrue(engine.Recommendations.Count > 0);
        }

        [TestCase]
        public void ConnectionTest()
        {
            PASqlConnectionInfo connInfo = new PASqlConnectionInfo();
            connInfo.ConnectionString = "Initial Catalog=Test;Data Source=ACCOLITE-PC;Integrated Security=SSPI;";
            System.Data.SqlClient.SqlConnection conn = connInfo.GetConnection();
            conn.Open();
            string str = conn.ServerVersion;

            string c = col.GetConnectionStringForServer(2);
            c = c.Split(';')[0].Split('=')[1];

        }


    }
}
