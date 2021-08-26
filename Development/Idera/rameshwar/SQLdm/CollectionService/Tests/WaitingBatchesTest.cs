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
        public void WaitingBatchesBatchfinder2000Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2000);
            batch = BatchFinder.WaitingBatches(ver, null);
            Assert.IsNullOrEmpty(batch);
        }

        [TestCase]
        public void WaitingBatchesfinder2005Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2005);
            batch = BatchFinder.WaitingBatches(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("WaitingBatches2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void WaitingBatchesfinder2008Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2008);
            batch = BatchFinder.WaitingBatches(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("WaitingBatches2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void WaitingBatchesBatchfinder2008R2Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2008R2);
            batch = BatchFinder.WaitingBatches(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("WaitingBatches2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void WaitingBatchesBatchfinder2012Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2012);
            batch = BatchFinder.WaitingBatches(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("WaitingBatches2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void WaitingBatchesBatchfinder2014Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2014);
            batch = BatchFinder.WaitingBatches(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("WaitingBatches2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void WaitingBatchesBuildCommandTest()
        {
            SqlConnection sqlConn = new SqlConnection(monitoredServer.ConnectionInfo.ConnectionString);
            ServerVersion version = monitoredServer.MostRecentSQLVersion;
            SqlCommand sqlCommActual = Idera.SQLdm.CollectionService.Probes.Sql.SqlCommandBuilder.BuildWaitingBatchesCommand(sqlConn, version, null);
            Assert.IsNotNull(sqlCommActual);
            Assert.IsNotEmpty(sqlCommActual.CommandText);
        }

        [TestCase]
        public void WaitingBatchesProbeTest()
        {
            string[] expectedColumns = new string[] { "wait_type", "wait_duration_ms", "session_id", "resource_description", "program_name", "text" };
            OnDemandCollectionContext<WaitingBatchesSnapshot> context = new OnDemandCollectionContext<WaitingBatchesSnapshot>();
            col.CollectWaitingBatches(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (WaitingBatchesSnapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.WaitingBatches.DefaultView.Table;
                Assert.IsNull(baseSnapshot.Error, TestCaseHelper.SnapshotErrorMessage);
                Assert.IsFalse(baseSnapshot.CollectionFailed, TestCaseHelper.CollectionFailedMessage);
                Assert.IsNotNull(snapshotTable, TestCaseHelper.TableNullMessage);
                foreach (string column in expectedColumns)
                {
                    Assert.IsTrue(snapshotTable.Columns.Contains(column), TestCaseHelper.GetColumnNameMissingError(column));
                }
            }
        }

        [TestCase]
        public void WaitingBatchesProbeTest11()
        {
            string[] expectedColumns = new string[] { "wait_type", "wait_duration_ms", "session_id", "resource_description", "program_name", "text" };
            OnDemandCollectionContext<DatabaseNamesSnapshot> context = new OnDemandCollectionContext<DatabaseNamesSnapshot>();
            col.CollectDatabaseNames(monitoredServerId, context, null);
            var snapshot = context.Wait();
            //var baseSnapshot = (DatabaseNamesSnapshot)snapshot;
            //{
            //    var snapshotTable = baseSnapshot.WaitingBatches.DefaultView.Table;
            //    Assert.IsNull(baseSnapshot.Error, TestCaseHelper.SnapshotErrorMessage);
            //    Assert.IsFalse(baseSnapshot.CollectionFailed, TestCaseHelper.CollectionFailedMessage);
            //    Assert.IsNotNull(snapshotTable, TestCaseHelper.TableNullMessage);
            //    foreach (string column in expectedColumns)
            //    {
            //        Assert.IsTrue(snapshotTable.Columns.Contains(column), TestCaseHelper.GetColumnNameMissingError(column));
            //    }
            //}
        }
    }
}
