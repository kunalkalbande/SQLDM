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
    /// SQLdm 10.0 (Vineet Kumar) SQL Doctor integration -- Added Nunit Test Cases to test out the batches imported from SQLDoctor
    /// </summary>

    public partial class SqlDoctorBatchTest
    {
        [TestCase]
        public void BackupAndRecoveryBatchfinder2000Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2000);
            batch = BatchFinder.BackupAndRecovery(ver, null);
            Assert.IsNullOrEmpty(batch);
        }

        [TestCase]
        public void BackupAndRecoveryBatchfinder2005Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2005);
            batch = BatchFinder.BackupAndRecovery(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("BackupAndRecovery2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void BackupAndRecoveryBatchfinder2008Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2008);
            batch = BatchFinder.BackupAndRecovery(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("BackupAndRecovery2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void BackupAndRecoveryBatchfinder2008R2Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2008R2);
            batch = BatchFinder.BackupAndRecovery(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("BackupAndRecovery2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void BackupAndRecoveryBatchfinder2012Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2012);
            batch = BatchFinder.BackupAndRecovery(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("BackupAndRecovery2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void BackupAndRecoveryBatchfinder2014Test()
        {
            ServerVersion ver;
            string batch;
            ver = new ServerVersion(TestCaseHelper.Version2014);
            batch = BatchFinder.BackupAndRecovery(ver, null);
            Assert.IsNotNullOrEmpty(batch);
            string expectedBatch = BatchFinder.GetBatchForTest("BackupAndRecovery2005.sql");
            Assert.AreEqual(expectedBatch, batch);
        }

        [TestCase]
        public void BackupAndRecoveryBuildCommandTest()
        {
            SqlConnection sqlConn = new SqlConnection(monitoredServer.ConnectionInfo.ConnectionString);
            ServerVersion version = monitoredServer.MostRecentSQLVersion;
            SqlCommand sqlCommActual = Idera.SQLdm.CollectionService.Probes.Sql.SqlCommandBuilder.BuildBackupAndRecoveryCommand(sqlConn, version, null, null);
            Assert.IsNotNull(sqlCommActual);
            Assert.IsNotEmpty(sqlCommActual.CommandText);
        }

        [TestCase]
        public void BackupAndRecoveryProbeTest()
        {
            string[] expectedColumns = new string[] { "Name", "Value", "FileName", "StartDateTime", "FileExists", "PhysicalName", "DaysOld", "BackupStartDate" };
            OnDemandCollectionContext<BackupAndRecoverySnapshot
                > context = new OnDemandCollectionContext<BackupAndRecoverySnapshot>();
            col.CollectBackupAndRecovery(monitoredServerId, context, null, null);
            var snapshot = context.Wait();
            var baseSnapshot = (BackupAndRecoverySnapshot)snapshot;
            {
                Assert.IsNull(baseSnapshot.Error, TestCaseHelper.SnapshotErrorMessage);
                Assert.IsFalse(baseSnapshot.CollectionFailed, TestCaseHelper.CollectionFailedMessage);

                var backupFileTable = baseSnapshot.BackupfileInfo.DefaultView.Table;
                var dbfileTable = baseSnapshot.DBFileInfo.DefaultView.Table;
                var genTable = baseSnapshot.GeneralInfo.DefaultView.Table;
                var backupDateTable = baseSnapshot.BackupDateInfo.DefaultView.Table;

                Assert.IsNotNull(dbfileTable, TestCaseHelper.TableNullMessage);
                Assert.IsNotNull(dbfileTable, TestCaseHelper.TableNullMessage);
                Assert.IsNotNull(genTable, TestCaseHelper.TableNullMessage);
                Assert.IsNotNull(backupDateTable, TestCaseHelper.TableNullMessage);

                Assert.IsTrue(backupFileTable.Columns.Contains("FileName"), TestCaseHelper.GetColumnNameMissingError("FileName"));
                Assert.IsTrue(backupFileTable.Columns.Contains("StartDateTime"), TestCaseHelper.GetColumnNameMissingError("StartDateTime"));
                Assert.IsTrue(backupFileTable.Columns.Contains("FileExists"), TestCaseHelper.GetColumnNameMissingError("FileExists"));

                Assert.IsTrue(dbfileTable.Columns.Contains("PhysicalName"), TestCaseHelper.GetColumnNameMissingError("PhysicalName"));

                Assert.IsTrue(genTable.Columns.Contains("Name"), TestCaseHelper.GetColumnNameMissingError("Name"));
                Assert.IsTrue(genTable.Columns.Contains("Value"), TestCaseHelper.GetColumnNameMissingError("Value"));

                Assert.IsTrue(backupDateTable.Columns.Contains("DaysOld"), TestCaseHelper.GetColumnNameMissingError("DaysOld"));
                Assert.IsTrue(backupDateTable.Columns.Contains("BackupStartDate"), TestCaseHelper.GetColumnNameMissingError("BackupStartDate"));

            }
        }
    }
}