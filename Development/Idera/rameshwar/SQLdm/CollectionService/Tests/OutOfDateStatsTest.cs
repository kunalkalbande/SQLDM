﻿using System;
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
    /// SQLdm 10.0 (Vineet Kumar) SQL Doctor integration -- Added Nunit Test Cases to test out the batches imported from SQLDoctor
    /// </summary>

    public partial class SqlDoctorBatchTest
    {
        [TestCase]
        public void OutOfDateStatsBatchfinder2000Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2000);
            batch = BatchFinder.OutOfDateStats(ver);
            Assert.IsNullOrEmpty(batch);
        }

        [TestCase]
        public void OutOfDateStatsBatchfinder2005Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2005);
            batch = BatchFinder.OutOfDateStats(ver);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("OutOfDateStats2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void OutOfDateStatsBatchfinder2008Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2008);
            batch = BatchFinder.OutOfDateStats(ver);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("OutOfDateStats2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void OutOfDateStatsBatchfinder2008R2Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2008R2);
            batch = BatchFinder.OutOfDateStats(ver);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("OutOfDateStats2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void OutOfDateStatsBatchfinder2012Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2012);
            batch = BatchFinder.OutOfDateStats(ver);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("OutOfDateStats2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void OutOfDateStatsBatchfinder2014Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2014);
            batch = BatchFinder.OutOfDateStats(ver);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("OutOfDateStats2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void OutOfDateStatsBuildCommandTest()
        {
            SqlConnection sqlConn = new SqlConnection(monitoredServer.ConnectionInfo.ConnectionString);
            ServerVersion version = monitoredServer.MostRecentSQLVersion;
            SqlCommand sqlCommActual = Idera.SQLdm.CollectionService.Probes.Sql.SqlCommandBuilder.BuildOutOfDateStatsCommand(sqlConn, version, null);
            Assert.IsNotNull(sqlCommActual);
            Assert.IsNotEmpty(sqlCommActual.CommandText);
        }

        [TestCase]
        public void OutOfDateStatsProbeTest()
        {
            string[] expectedColumns = new string[] { "DatabaseName", "Schema", "Table", "Name", "RowCount", "ModCount", "StatsDate", "HoursSinceUpdated" };
            OnDemandCollectionContext<OutOfDateStatsSnapshot
                > context = new OnDemandCollectionContext<OutOfDateStatsSnapshot>();
            col.CollectOutOfDateStats(monitoredServerId, context, null, null);
            var snapshot = context.Wait();
            var baseSnapshot = (OutOfDateStatsSnapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.OutOfDateStatsInfo.DefaultView.Table;
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