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
        public void OverlappingIndexesBatchfinder2000Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2000);
            batch = BatchFinder.OverlappingIndexes(ver);
            Assert.IsNullOrEmpty(batch);
        }

        [TestCase]
        public void OverlappingIndexesfinder2005Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2005);
            batch = BatchFinder.OverlappingIndexes(ver);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("OverlappingIndexes2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void OverlappingIndexesfinder2008Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2008);
            batch = BatchFinder.OverlappingIndexes(ver);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("OverlappingIndexes2008.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void OverlappingIndexesBatchfinder2008R2Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2008R2);
            batch = BatchFinder.OverlappingIndexes(ver);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("OverlappingIndexes2008.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void OverlappingIndexesBatchfinder2012Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2012);
            batch = BatchFinder.OverlappingIndexes(ver);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("OverlappingIndexes2008.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void OverlappingIndexesBatchfinder2014Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2014);
            batch = BatchFinder.OverlappingIndexes(ver);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("OverlappingIndexes2008.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void OverlappingIndexesBuildCommandTest()
        {
            SqlConnection sqlConn = new SqlConnection(monitoredServer.ConnectionInfo.ConnectionString);
            ServerVersion version = monitoredServer.MostRecentSQLVersion;
            SqlCommand sqlCommActual = Idera.SQLdm.CollectionService.Probes.Sql.SqlCommandBuilder.BuildOverlappingIndexesCommand(sqlConn, version, null);
            Assert.IsNotNull(sqlCommActual);
            Assert.IsNotEmpty(sqlCommActual.CommandText);
        }

        [TestCase]
        public void OverlappingIndexesProbeTest()
        {
            OnDemandCollectionContext<OverlappingIndexesSnapshot> context = new OnDemandCollectionContext<OverlappingIndexesSnapshot>();
            col.CollectOverlappingIndexes(monitoredServerId, context, null, null);
            var snapshot = context.Wait();


            var baseSnapshot = (OverlappingIndexesSnapshot)snapshot;
            Assert.IsNull(baseSnapshot.Error, TestCaseHelper.SnapshotErrorMessage);
            Assert.IsFalse(baseSnapshot.CollectionFailed, TestCaseHelper.CollectionFailedMessage);
            {
                string[] expectedColumns = new string[] { "DupIndex", "DatabaseName","Schema", "TableName","IndexName", "IndexId","IndexUnique", "IndexPrimaryKey",
                     "IndexUsage", "IndexUpdates", "IndexKeySize","IndexForeignKeys", "DupIndexName","DupIndexId","DupIndexUnique", "DupIndexPrimaryKey", "DupIndexUsage",
                     "DupIndexUpdates", "DupIndexKeySize","DupIndexForeignKeys" };
                var snapshotTable = baseSnapshot.DuplicateIndexInfo.DefaultView.Table;
                Assert.IsNotNull(snapshotTable, TestCaseHelper.TableNullMessage);
                foreach (string column in expectedColumns)
                {
                    Assert.IsTrue(snapshotTable.Columns.Contains(column), TestCaseHelper.GetColumnNameMissingError(column));
                }
            }
            {
                string[] expectedColumns = new string[] { "DupIndex", "DatabaseName", "Schema", "TableName", "IndexName", "IndexId", "IndexUnique", "IndexPrimaryKey", 
                    "IndexUsage", "IndexUpdates", "IndexKeySize", "IndexForeignKeys", "DupIndexName", "DupIndexId", "DupIndexUnique", "DupIndexPrimaryKey", "DupIndexUsage",
                    "DupIndexUpdates", "DupIndexKeySize", "DupIndexForeignKeys" };
                var snapshotTable = baseSnapshot.PartialDuplicateIndexInfo.DefaultView.Table;
                Assert.IsNotNull(snapshotTable, TestCaseHelper.TableNullMessage);
                foreach (string column in expectedColumns)
                {
                    Assert.IsTrue(snapshotTable.Columns.Contains(column), TestCaseHelper.GetColumnNameMissingError(column));
                }
            }
        }
    }
}
