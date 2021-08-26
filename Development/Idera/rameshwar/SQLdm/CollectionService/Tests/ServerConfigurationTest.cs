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
    
    /// <summary>
    /// SQLdm 10.0 (Praveen Suhalka) SQL Doctor integration -- Added Nunit Test Cases to test out the batches imported from SQLDoctor
    /// </summary>

    public partial class SqlDoctorBatchTest
    {

        [TestCase]
        public void ServerConfigurationBatchfinder2000Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2000);
            batch = BatchFinder.ServerConfiguration(ver, null);
            Assert.IsNullOrEmpty(batch);
        }

        [TestCase]
        public void ServerConfigurationfinder2005Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2005);
            batch = BatchFinder.ServerConfiguration(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("ServerConfiguration2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void ServerConfigurationfinder2008Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2008);
            batch = BatchFinder.ServerConfiguration(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("ServerConfiguration2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void ServerConfigurationBatchfinder2008R2Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2008R2);
            batch = BatchFinder.ServerConfiguration(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("ServerConfiguration2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void ServerConfigurationBatchfinder2012Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2012);
            batch = BatchFinder.ServerConfiguration(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("ServerConfiguration2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void ServerConfigurationBatchfinder2014Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2014);
            batch = BatchFinder.ServerConfiguration(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("ServerConfiguration2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void ServerConfigurationBuildCommandTest()
        {
            SqlConnection sqlConn = new SqlConnection(monitoredServer.ConnectionInfo.ConnectionString);
            ServerVersion version = monitoredServer.MostRecentSQLVersion;
            SqlCommand sqlCommActual = Idera.SQLdm.CollectionService.Probes.Sql.SqlCommandBuilder.BuildServerConfigurationCommand(sqlConn, version, null);
            Assert.IsNotNull(sqlCommActual);
            Assert.IsNotEmpty(sqlCommActual.CommandText);
        }

        [TestCase]
        public void ServerConfigurationProbeTest()
        {
            OnDemandCollectionContext<ServerConfigurationSnapshot> context = new OnDemandCollectionContext<ServerConfigurationSnapshot>();
            col.CollectServerConfiguration(monitoredServerId, context, null);
            var snapshot = context.Wait();

            var baseSnapshot = (ServerConfigurationSnapshot)snapshot;
            Assert.IsNull(baseSnapshot.Error, TestCaseHelper.SnapshotErrorMessage);
            Assert.IsFalse(baseSnapshot.CollectionFailed, TestCaseHelper.CollectionFailedMessage);
            {
                string[] expectedColumns = new string[] { "name", "value", "value_in_use", "is_dynamic" };
                var snapshotTable = baseSnapshot.ServerConfiguration.DefaultView.Table;
                Assert.IsNotNull(snapshotTable, TestCaseHelper.TableNullMessage);
                foreach (string column in expectedColumns)
                {
                    Assert.IsTrue(snapshotTable.Columns.Contains(column), TestCaseHelper.GetColumnNameMissingError(column));
                }
            }
            {
                string[] expectedColumns = new string[] { "username", "policy", "expire" };
                var snapshotTable = baseSnapshot.VulnerableLogins.DefaultView.Table;
                Assert.IsNotNull(snapshotTable, TestCaseHelper.TableNullMessage);
                foreach (string column in expectedColumns)
                {
                    Assert.IsTrue(snapshotTable.Columns.Contains(column), TestCaseHelper.GetColumnNameMissingError(column));
                }
            }
            {
                string[] expectedColumns = new string[] { "name", "value" };
                var snapshotTable = baseSnapshot.SecuritySettings.DefaultView.Table;
                Assert.IsNotNull(snapshotTable, TestCaseHelper.TableNullMessage);
                foreach (string column in expectedColumns)
                {
                    Assert.IsTrue(snapshotTable.Columns.Contains(column), TestCaseHelper.GetColumnNameMissingError(column));
                }
            }
            {
                string[] expectedColumns = new string[] { "name", "step_name" };
                var snapshotTable = baseSnapshot.DeprecatedAgentTokenJobs.DefaultView.Table;
                Assert.IsNotNull(snapshotTable, TestCaseHelper.TableNullMessage);
                foreach (string column in expectedColumns)
                {
                    Assert.IsTrue(snapshotTable.Columns.Contains(column), TestCaseHelper.GetColumnNameMissingError(column));
                }
            }
        }
    }
}
