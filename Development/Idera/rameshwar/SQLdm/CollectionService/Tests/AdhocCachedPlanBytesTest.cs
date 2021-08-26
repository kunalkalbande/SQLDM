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

using Idera.SQLdm.CollectionService.Monitoring;
using Idera.SQLdm.CollectionService.Probes.Sql.Batches;
using Microsoft.SqlServer.Management.Smo;
using Idera.SQLdm.CollectionService.OnDemandClient;

namespace Idera.SQLdm.CollectionService.Tests
{
    
    /// <summary>
    /// SQLdm 10.0 (Praveen Suhalka) SQL Doctor integration -- Added Nunit Test Cases to test out the batches imported from SQLDoctor
    /// </summary>

    public partial class SqlDoctorBatchTest
    {

        [TestCase]
        public void AdhocCachedPlanBytesBatchfinder2000Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2000);
            batch = BatchFinder.GetAdhocCachedPlanBytes(ver, null);
            Assert.IsNullOrEmpty(batch);
        }

        [TestCase]
        public void AdhocCachedPlanBytesBatchfinder2005Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2005);
            batch = BatchFinder.GetAdhocCachedPlanBytes(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("GetAdhocCachedPlanBytes2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void AdhocCachedPlanBytesBatchfinder2008Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2008);
            batch = BatchFinder.GetAdhocCachedPlanBytes(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("GetAdhocCachedPlanBytes2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void AdhocCachedPlanBytesBatchfinder2008R2Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2008R2);
            batch = BatchFinder.GetAdhocCachedPlanBytes(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("GetAdhocCachedPlanBytes2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void AdhocCachedPlanBytesBatchfinder2012Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2012);
            batch = BatchFinder.GetAdhocCachedPlanBytes(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("GetAdhocCachedPlanBytes2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void AdhocCachedPlanBytesBatchfinder2014Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2014);
            batch = BatchFinder.GetAdhocCachedPlanBytes(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("GetAdhocCachedPlanBytes2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void AdhocCachedPlanBytesBuildCommandTest()
        {
            SqlConnection sqlConn = new SqlConnection(monitoredServer.ConnectionInfo.ConnectionString);
            ServerVersion version = monitoredServer.MostRecentSQLVersion;
            SqlCommand sqlCommActual = Idera.SQLdm.CollectionService.Probes.Sql.SqlCommandBuilder.BuildGetAdhocCachedPlanBytesCommand(sqlConn, version, null);
            Assert.IsNotNull(sqlCommActual);
            Assert.IsNotEmpty(sqlCommActual.CommandText);
        }

        [TestCase]
        public void AdhocCachedPlanBytesProbeTest()
        {
            OnDemandCollectionContext<AdhocCachedPlanBytesSnapshot> context = new OnDemandCollectionContext<AdhocCachedPlanBytesSnapshot>();
            col.CollectAdhocCachedPlanBytes(monitoredServerId, context, null);
            var snapshot = context.Wait();

            var baseSnapshot = (AdhocCachedPlanBytesSnapshot)snapshot;
            {
                string[] expectedColumns = new string[] { "Value" };
                var snapshotTable = baseSnapshot.AdhocCachedPlanBytes.DefaultView.Table;
                Assert.IsNull(baseSnapshot.Error, TestCaseHelper.SnapshotErrorMessage);
                Assert.IsFalse(baseSnapshot.CollectionFailed, TestCaseHelper.CollectionFailedMessage);
                Assert.IsNotNull(snapshotTable, TestCaseHelper.TableNullMessage);
                foreach (string column in expectedColumns)
                {
                    Assert.IsTrue(snapshotTable.Columns.Contains(column), TestCaseHelper.GetColumnNameMissingError(column));
                }
            }
        }
    }
}
