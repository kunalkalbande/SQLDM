//using System.Linq;
using NUnit.Framework;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.CollectionService.OnDemandClient;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Metrics;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Snapshots;

namespace Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Tests
{
    public partial class PrescriptiveAnalyzerTest
    {
        [TestCase]
        public void WorstFillFactorMetricTest()
        {
            OnDemandCollectionContext<PAWorstFillFactorIndexesSnapshot> context = new OnDemandCollectionContext<PAWorstFillFactorIndexesSnapshot>();
            col.CollectWorstFillFactorIndexes(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWorstFillFactorIndexesSnapshot baseSnapshot = snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WorstFillFactorIndexesSnapshotValueShutdown = baseSnapshot;
            WorstIndexFillFactorMetrics metrics = new WorstIndexFillFactorMetrics();
            metrics.AddSnapshot(snap);

            var snapshotTable = baseSnapshot.WorstFillFactorIndexes.DefaultView.Table;
            Assert.IsNull(baseSnapshot.Error, "Ërror in snapshot is not null");
            Assert.IsFalse(baseSnapshot.CollectionFailed, "CollectionFailed is true for snapshot");
            Assert.IsNotNull(snapshotTable, "snapshot table is null");
            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void SQLModuleOptionsMetricTest()
        {
            SQLModuleOptionsMetrics metrics = new SQLModuleOptionsMetrics();
            metrics.AddSnapshot(snap);
            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void OverlappingIndexesMetricTest()
        {
            OverlappingIndexesMetrics metrics = new OverlappingIndexesMetrics();
            metrics.AddSnapshot(snap);
            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void OutOfDateStatsMetricTest()
        {
            OutOfDateStatsMetrics metrics = new OutOfDateStatsMetrics();
            metrics.AddSnapshot(snap);
            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void NUMANodeCountersMetricTest()
        {
            OnDemandCollectionContext<PANUMANodeCountersSnapshot> context = new OnDemandCollectionContext<PANUMANodeCountersSnapshot>();
            col.CollectNUMANodeCounters(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PANUMANodeCountersSnapshot baseSnapshot = (PANUMANodeCountersSnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.NUMANodeCountersSnapshotValueShutdown = baseSnapshot;
            NUMANodeCountersMetrics metrics = new NUMANodeCountersMetrics();
            metrics.AddSnapshot(snap);

            var snapshotTable = baseSnapshot.NUMANodeCounters.DefaultView.Table;
            Assert.IsNull(baseSnapshot.Error, "Ërror in snapshot is not null");
            Assert.IsFalse(baseSnapshot.CollectionFailed, "CollectionFailed is true for snapshot");
            Assert.IsNotNull(snapshotTable, "snapshot table is null");
            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void IndexContentionMetricTest()
        {
            IndexContentionMetrics metrics = new IndexContentionMetrics();
            metrics.AddSnapshot(snap);
            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void HighIndexUpdatesMetricTest()
        {
            HighIndexUpdatesMetrics metrics = new HighIndexUpdatesMetrics();
            metrics.AddSnapshot(snap);
            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void FragmentedIndexesMetricTest()
        {
            FragmentedIndexesMetrics metrics = new FragmentedIndexesMetrics();
            metrics.AddSnapshot(snap);
            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void DisabledIndexesMetricTest()
        {
            DisabledIndexMetrics metrics = new DisabledIndexMetrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void DBSecurityMetricTest()
        {
            DBSecurityMetrics metrics = new DBSecurityMetrics();
            metrics.AddSnapshot(snap);
            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void BackupAndRecoveryMetricTest()
        {
            BackupAndRecoveryMetrics metrics = new BackupAndRecoveryMetrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void DatabaseInfoSnapshotMetricsTest()
        {
            OnDemandCollectionContext<PAGetMasterFilesSnapshot> context = new OnDemandCollectionContext<PAGetMasterFilesSnapshot>();
            col.CollectMasterFiles(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAGetMasterFilesSnapshot baseSnapshot = (PAGetMasterFilesSnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.GetMasterFilesSnapshotValueStartup = baseSnapshot;
            DatabaseInfoSnapshotMetrics metrics = new DatabaseInfoSnapshotMetrics();
            metrics.AddSnapshot(snap);

            var snapshotTable = baseSnapshot.DatabaseFileInfo.DefaultView.Table;
            Assert.IsNull(baseSnapshot.Error, "Ërror in snapshot is not null");
            Assert.IsFalse(baseSnapshot.CollectionFailed, "CollectionFailed is true for snapshot");
            Assert.IsNotNull(snapshotTable, "snapshot table is null");
            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }


        [TestCase]
        public void ServerConfigurationMetricsTest()
        {
            OnDemandCollectionContext<PAServerConfigurationSnapshot> context = new OnDemandCollectionContext<PAServerConfigurationSnapshot>();
            col.CollectServerConfiguration(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAServerConfigurationSnapshot baseSnapshot = (PAServerConfigurationSnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.ServerConfigurationSnapshotValueStartup = baseSnapshot;
            ServerConfigurationMetrics metrics = new ServerConfigurationMetrics();
            metrics.AddSnapshot(snap);

            var snapshotTable1 = baseSnapshot.ServerConfiguration.DefaultView.Table;
            var snapshotTable2 = baseSnapshot.DeprecatedAgentTokenJobs.DefaultView.Table;
            var snapshotTable3 = baseSnapshot.VulnerableLogins.DefaultView.Table;
            var snapshotTable4 = baseSnapshot.SecuritySettings.DefaultView.Table;

            Assert.IsNull(baseSnapshot.Error, "Ërror in snapshot is not null");
            Assert.IsFalse(baseSnapshot.CollectionFailed, "CollectionFailed is true for snapshot");
            Assert.IsNotNull(snapshotTable1, "snapshot table is null");
            Assert.IsNotNull(snapshotTable2, "snapshot table is null");
            Assert.IsNotNull(snapshotTable3, "snapshot table is null");
            Assert.IsNotNull(snapshotTable4, "snapshot table is null");
            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        //[TestCase]
        //public void ServerConfigurationMetricsTest()
        //{
        //    OnDemandCollectionContext<PAConfigurationSnapshot> context = new OnDemandCollectionContext<PAConfigurationSnapshot>();
        //    col.CollectConfiguration(monitoredServerId, context, null);
        //    var snapshot = context.Wait();
        //    PAConfigurationSnapshot baseSnapshot = (PAConfigurationSnapshot)snapshot;
        //    PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
        //    snap.ConfigurationSnapshotValue = baseSnapshot;
        //    ServerConfigurationMetrics metrics = new ServerConfigurationMetrics();
        //    metrics.AddSnapshot(snap);

        //    var snapshotTable1 = baseSnapshot.ConfigurationSettings.DefaultView.Table;
        //    var snapshotTable2 = baseSnapshot.ServerProperties.DefaultView.Table;

        //    Assert.IsNull(baseSnapshot.Error, "Ërror in snapshot is not null");
        //    Assert.IsFalse(baseSnapshot.CollectionFailed, "CollectionFailed is true for snapshot");
        //    Assert.IsNotNull(snapshotTable1, "snapshot table is null");
        //    Assert.IsNotNull(snapshotTable2, "snapshot table is null");
        //    Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        //}

        [TestCase]
        public void EstRowsMetricsTest()
        {
            OnDemandCollectionContext<PAQueryPlanEstRowsSnapshot> context = new OnDemandCollectionContext<PAQueryPlanEstRowsSnapshot>();
            col.CollectQueryPlanEstRows(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAQueryPlanEstRowsSnapshot baseSnapshot = (PAQueryPlanEstRowsSnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.QueryPlanEstRowsSnapshotValue = baseSnapshot;
            EstRowsMetrics metrics = new EstRowsMetrics();
            metrics.AddSnapshot(snap);

            var snapshotTable = baseSnapshot.QueryPlanEstRows.DefaultView.Table;
            Assert.IsNull(baseSnapshot.Error, "Ërror in snapshot is not null");
            Assert.IsFalse(baseSnapshot.CollectionFailed, "CollectionFailed is true for snapshot");
            Assert.IsNotNull(snapshotTable, "snapshot table is null");
            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void WaitingBatchesMetricsTest()
        {
            OnDemandCollectionContext<PAWaitingBatchesSnapshot> context = new OnDemandCollectionContext<PAWaitingBatchesSnapshot>();
            col.CollectWaitingBatches(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWaitingBatchesSnapshot baseSnapshot = (PAWaitingBatchesSnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WaitingBatchesSnapshotValueStartup = baseSnapshot;
            WaitingBatchesMetrics metrics = new WaitingBatchesMetrics();
            metrics.AddSnapshot(snap);

            var snapshotTable = baseSnapshot.WaitingBatches.DefaultView.Table;
            Assert.IsNull(baseSnapshot.Error, "Ërror in snapshot is not null");
            Assert.IsFalse(baseSnapshot.CollectionFailed, "CollectionFailed is true for snapshot");
            Assert.IsNotNull(snapshotTable, "snapshot table is null");
            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        //[TestCase]
        //public void DBPropertiesMetricsTest()
        //{
        //    OnDemandCollectionContext<PADatabaseConfigurationSnapshot> context = new OnDemandCollectionContext<PADatabaseConfigurationSnapshot>();
        //    col.CollectDatabaseConfiguration(monitoredServerId, context, null);
        //    var snapshot = context.Wait();
        //    PADatabaseConfigurationSnapshot baseSnapshot = (PADatabaseConfigurationSnapshot)snapshot;
        //    PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
        //    snap.DatabaseConfigurationSnapshotValue = baseSnapshot;
        //    DBPropertiesMetrics metrics = new DBPropertiesMetrics();
        //    metrics.AddSnapshot(snap);

        //    var snapshotTable = baseSnapshot.ConfigurationSettings.DefaultView.Table;
        //    Assert.IsNull(baseSnapshot.Error, "Ërror in snapshot is not null");
        //    Assert.IsFalse(baseSnapshot.CollectionFailed, "CollectionFailed is true for snapshot");
        //    Assert.IsNotNull(snapshotTable, "snapshot table is null");
        //    Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        //}

        [TestCase]
        public void WMIVolumeMetricsTest()
        {
            OnDemandCollectionContext<PAWmiVolumeSnapshot> context = new OnDemandCollectionContext<PAWmiVolumeSnapshot>();
            col.CollectWmiVolume(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWmiVolumeSnapshot baseSnapshot = (PAWmiVolumeSnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WmiVolumeSnapshotValueStartup = baseSnapshot;
            WMIVolumeMetrics metrics = new WMIVolumeMetrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void WMITCPv6MetricsTest()
        {
            OnDemandCollectionContext<PAWmiTCPv6Snapshot> context = new OnDemandCollectionContext<PAWmiTCPv6Snapshot>();
            col.CollectWmiTCPv6(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWmiTCPv6Snapshot baseSnapshot = (PAWmiTCPv6Snapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WmiTCPv6SnapshotValueStartup = baseSnapshot;
            WMITCPv6Metrics metrics = new WMITCPv6Metrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void WMITCPv4MetricsTest()
        {
            OnDemandCollectionContext<PAWmiTCPv4Snapshot> context = new OnDemandCollectionContext<PAWmiTCPv4Snapshot>();
            col.CollectWmiTCPv4(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWmiTCPv4Snapshot baseSnapshot = (PAWmiTCPv4Snapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WmiTCPv4SnapshotValueStartup = baseSnapshot;
            WMITCPv4Metrics metrics = new WMITCPv4Metrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void WMITCPMetricsTest()
        {
            OnDemandCollectionContext<PAWmiTCPSnapshot> context = new OnDemandCollectionContext<PAWmiTCPSnapshot>();
            col.CollectWmiTCP(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWmiTCPSnapshot baseSnapshot = (PAWmiTCPSnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WmiTCPSnapshotValueStartup = baseSnapshot;
            WMITCPMetrics metrics = new WMITCPMetrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void WMIServiceMetricsTest()
        {
            OnDemandCollectionContext<PAWmiServiceSnapshot> context = new OnDemandCollectionContext<PAWmiServiceSnapshot>();
            col.CollectWmiService(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWmiServiceSnapshot baseSnapshot = (PAWmiServiceSnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WmiServiceSnapshotValueShutdown = baseSnapshot;
            WMIServiceMetrics metrics = new WMIServiceMetrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void WMIProcessorMetricsTest()
        {
            OnDemandCollectionContext<PAWmiProcessorSnapshot> context = new OnDemandCollectionContext<PAWmiProcessorSnapshot>();
            col.CollectWmiProcessor(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWmiProcessorSnapshot baseSnapshot = (PAWmiProcessorSnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WmiProcessorSnapshotValueInterval = new PAWmiProcessorSnapshot("test");
            snap.WmiProcessorSnapshotValueInterval = baseSnapshot;
            WMIProcessorMetrics metrics = new WMIProcessorMetrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void WMIProcessMetricsTest()
        {
            OnDemandCollectionContext<PAWmiProcessSnapshot> context = new OnDemandCollectionContext<PAWmiProcessSnapshot>();
            col.CollectWmiProcess(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWmiProcessSnapshot baseSnapshot = (PAWmiProcessSnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WmiProcessSnapshotValueShutdown = baseSnapshot;
            WMIProcessMetrics metrics = new WMIProcessMetrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void WMIPhysicalMemoryMetricsTest()
        {
            OnDemandCollectionContext<PAWmiPhysicalMemorySnapshot> context = new OnDemandCollectionContext<PAWmiPhysicalMemorySnapshot>();
            col.CollectWmiPhysicalMemory(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWmiPhysicalMemorySnapshot baseSnapshot = (PAWmiPhysicalMemorySnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WmiPhysicalMemorySnapshotValueShutdown = baseSnapshot;
            WMIPhysicalMemoryMetrics metrics = new WMIPhysicalMemoryMetrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void WMIPerfOSSystemMetricsTest()
        {
            OnDemandCollectionContext<PAWmiPerfOSSystemSnapshot> context = new OnDemandCollectionContext<PAWmiPerfOSSystemSnapshot>();
            col.CollectWmiPerfOSSystem(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWmiPerfOSSystemSnapshot baseSnapshot = (PAWmiPerfOSSystemSnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WmiPerfOSSystemSnapshotValueStartup = baseSnapshot;
            WMIPerfOSSystemMetrics metrics = new WMIPerfOSSystemMetrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void WMIPerfOSProcessorMetricsTest()
        {
            OnDemandCollectionContext<PAWmiPerfOSProcessorSnapshot> context = new OnDemandCollectionContext<PAWmiPerfOSProcessorSnapshot>();
            col.CollectWmiPerfOSProcessor(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWmiPerfOSProcessorSnapshot baseSnapshot = (PAWmiPerfOSProcessorSnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WmiPerfOSProcessorSnapshotValueStartup = baseSnapshot;
            WMIPerfOSProcessorMetrics metrics = new WMIPerfOSProcessorMetrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void WMIPerfOSMemoryMetricsTest()
        {
            OnDemandCollectionContext<PAWmiPerfOSMemorySnapshot> context = new OnDemandCollectionContext<PAWmiPerfOSMemorySnapshot>();
            col.CollectWmiPerfOSMemory(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWmiPerfOSMemorySnapshot baseSnapshot = (PAWmiPerfOSMemorySnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WmiPerfOSMemorySnapshotValueShutdown = baseSnapshot;
            WMIPerfOSMemoryMetrics metrics = new WMIPerfOSMemoryMetrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void WMIPerfDiskPhysicalDiskMetricsTest()
        {
            OnDemandCollectionContext<PAWmiPerfDiskPhysicalDiskSnapshot> context = new OnDemandCollectionContext<PAWmiPerfDiskPhysicalDiskSnapshot>();
            col.CollectWmiPerfDiskPhysicalDisk(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWmiPerfDiskPhysicalDiskSnapshot baseSnapshot = (PAWmiPerfDiskPhysicalDiskSnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WmiPerfDiskPhysicalDiskSnapshotValueStartup = baseSnapshot;
            WMIPerfDiskPhysicalDiskMetrics metrics = new WMIPerfDiskPhysicalDiskMetrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void WMIPerfDiskLogicalDiskMetricsTest()
        {
            OnDemandCollectionContext<PAWmiPerfDiskLogicalDiskSnapshot> context = new OnDemandCollectionContext<PAWmiPerfDiskLogicalDiskSnapshot>();
            col.CollectWmiPerfDiskLogicalDisk(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWmiPerfDiskLogicalDiskSnapshot baseSnapshot = (PAWmiPerfDiskLogicalDiskSnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WmiPerfDiskLogicalDiskSnapshotValueStartup = baseSnapshot;
            WMIPerfDiskLogicalDiskMetrics metrics = new WMIPerfDiskLogicalDiskMetrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void WMIPageFileMetricsTest()
        {
            OnDemandCollectionContext<PAWmiPageFileSnapshot> context = new OnDemandCollectionContext<PAWmiPageFileSnapshot>();
            col.CollectWmiPageFile(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWmiPageFileSnapshot baseSnapshot = (PAWmiPageFileSnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WmiPageFileSnapshotValueStartup = baseSnapshot;
            WMIPageFileMetrics metrics = new WMIPageFileMetrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void WMINetworkRedirectorMetricsTest()
        {
            OnDemandCollectionContext<PAWmiNetworkRedirectorSnapshot> context = new OnDemandCollectionContext<PAWmiNetworkRedirectorSnapshot>();
            col.CollectWmiNetworkRedirector(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWmiNetworkRedirectorSnapshot baseSnapshot = (PAWmiNetworkRedirectorSnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WmiNetworkRedirectorSnapshotValueStartup = baseSnapshot;
            WMINetworkRedirectorMetrics metrics = new WMINetworkRedirectorMetrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void WMINetworkInterfaceMetricsTest()
        {
            OnDemandCollectionContext<PAWmiNetworkInterfaceSnapshot> context = new OnDemandCollectionContext<PAWmiNetworkInterfaceSnapshot>();
            col.CollectWmiNetworkInterface(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWmiNetworkInterfaceSnapshot baseSnapshot = (PAWmiNetworkInterfaceSnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WmiNetworkInterfaceSnapshotValueStartup = baseSnapshot;
            WMINetworkInterfaceMetrics metrics = new WMINetworkInterfaceMetrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void WMIEncryptableVolumeMetricsTest()
        {
            OnDemandCollectionContext<PAWmiEncryptableVolumeSnapshot> context = new OnDemandCollectionContext<PAWmiEncryptableVolumeSnapshot>();
            col.CollectWmiEncryptableVolume(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWmiEncryptableVolumeSnapshot baseSnapshot = (PAWmiEncryptableVolumeSnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WmiEncryptableVolumeSnapshotValueShutdown = baseSnapshot;
            WMIEncryptableVolumeMetrics metrics = new WMIEncryptableVolumeMetrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

        [TestCase]
        public void WMIComputerSystemMetricsTest()
        {
            OnDemandCollectionContext<PAWmiComputerSystemSnapshot> context = new OnDemandCollectionContext<PAWmiComputerSystemSnapshot>();
            col.CollectWmiComputerSystem(monitoredServerId, context, null);
            var snapshot = context.Wait();
            PAWmiComputerSystemSnapshot baseSnapshot = (PAWmiComputerSystemSnapshot)snapshot;
            PAPrescriptiveAnalyticsSnapshot snap = new PAPrescriptiveAnalyticsSnapshot();
            snap.WmiComputerSystemSnapshotValueShutdown = baseSnapshot;
            WMIComputerSystemMetrics metrics = new WMIComputerSystemMetrics();
            metrics.AddSnapshot(snap);

            Assert.IsTrue(metrics.IsDataValid, "Metrics is not populated.");
        }

    }
}
