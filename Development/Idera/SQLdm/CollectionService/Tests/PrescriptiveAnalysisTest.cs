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
    partial class SqlDoctorBatchTest
    {
        [TestCase]
        public void PrescriptiveAnalyticsTest()
        {
            OnDemandCollectionContext<Snapshot> context = new OnDemandCollectionContext<Snapshot>();
            col.RunPrescriptiveAnalysis(monitoredServerId, context, null, null, AnalysisCollectorType.Startup);
            var snapshot = context.WaitMultiple();

            OnDemandCollectionContext<List<Snapshot>> context1 = new OnDemandCollectionContext<List<Snapshot>>();
            col.RunPrescriptiveAnalysis(monitoredServerId, context1, null, null, AnalysisCollectorType.Startup);
            var snapshot1 = context.Wait();
            Assert.IsNotNull(1);

            //var baseSnapshot = (List<Snapshot>)snapshot;
            {
                //string[] expectedColumns = new string[] { "Value" };
                //var snapshotTable = baseSnapshot.AdhocCachedPlanBytes.DefaultView.Table;
                //Assert.IsNull(baseSnapshot.Error, TestCaseHelper.SnapshotErrorMessage);
                //Assert.IsFalse(baseSnapshot.CollectionFailed, TestCaseHelper.CollectionFailedMessage);
                //Assert.IsNotNull(snapshotTable, TestCaseHelper.TableNullMessage);
                //foreach (string column in expectedColumns)
                //{
                //    Assert.IsTrue(snapshotTable.Columns.Contains(column), TestCaseHelper.GetColumnNameMissingError(column));
                //}
            }
        }
    }
}
