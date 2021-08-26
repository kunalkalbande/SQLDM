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
        public void WmiProcessProbeTest()
        {
            string[] expectedColumns = new string[] { "Name", "CommandLine", "ProcessId", "ThreadCount", "Priority", "WorkingSetSize" };
            OnDemandCollectionContext<WmiProcessSnapshot> context = new OnDemandCollectionContext<WmiProcessSnapshot>();
            col.CollectWmiProcess(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (WmiProcessSnapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.WmiProcess.DefaultView.Table;
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
        public void WmiVolumeProbeTest()
        {
            string[] expectedColumns = new string[] { "Name", "FileSystem", "BlockSize" };
            OnDemandCollectionContext<WmiVolumeSnapshot> context = new OnDemandCollectionContext<WmiVolumeSnapshot>();
            col.CollectWmiVolume(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (WmiVolumeSnapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.WmiVolume.DefaultView.Table;
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
        public void WmiTCPProbeTest()
        {
            string[] expectedColumns = new string[] { "SegmentsPersec", "SegmentsRetransmittedPerSec"};
            OnDemandCollectionContext<WmiTCPSnapshot> context = new OnDemandCollectionContext<WmiTCPSnapshot>();
            col.CollectWmiTCP(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (WmiTCPSnapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.WmiTCP.DefaultView.Table;
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
        public void WmiTCPv4ProbeTest()
        {
            string[] expectedColumns = new string[] { "SegmentsPersec", "SegmentsRetransmittedPerSec" };
            OnDemandCollectionContext<WmiTCPv4Snapshot> context = new OnDemandCollectionContext<WmiTCPv4Snapshot>();
            col.CollectWmiTCPv4(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (WmiTCPv4Snapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.WmiTCP.DefaultView.Table;
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
        public void WmiTCPv6ProbeTest()
        {
            string[] expectedColumns = new string[] { "SegmentsPersec", "SegmentsRetransmittedPerSec" };
            OnDemandCollectionContext<WmiTCPv6Snapshot> context = new OnDemandCollectionContext<WmiTCPv6Snapshot>();
            col.CollectWmiTCPv6(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (WmiTCPv6Snapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.WmiTCP.DefaultView.Table;
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
        public void WmiPerfOSSystemProbeTest()
        {
            string[] expectedColumns = new string[] { "ProcessorQueueLength", "ContextSwitchesPerSec", "Frequency_Sys100NS", "Timestamp_Sys100NS" };
            OnDemandCollectionContext<WmiPerfOSSystemSnapshot> context = new OnDemandCollectionContext<WmiPerfOSSystemSnapshot>();
            col.CollectWmiPerfOSSystem(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (WmiPerfOSSystemSnapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.WmiPerfOSSystem.DefaultView.Table;
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
        public void WmiPerfOSMemoryProbeTest()
        {
            string[] expectedColumns = new string[] { "PagesPersec", "Timestamp_Sys100NS" };
            OnDemandCollectionContext<WmiPerfOSMemorySnapshot> context = new OnDemandCollectionContext<WmiPerfOSMemorySnapshot>();
            col.CollectWmiPerfOSMemory(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (WmiPerfOSMemorySnapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.WmiPerfOSMemory.DefaultView.Table;
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
        public void WmiNetworkRedirectorProbeTest()
        {
            string[] expectedColumns = new string[] { "NetworkErrorsPerSec" };
            OnDemandCollectionContext<WmiNetworkRedirectorSnapshot> context = new OnDemandCollectionContext<WmiNetworkRedirectorSnapshot>();
            col.CollectWmiNetworkRedirector(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (WmiNetworkRedirectorSnapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.WmiNetworkRedirector.DefaultView.Table;
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
        public void WmiEncryptableVolumeProbeTest()
        {
            string[] expectedColumns = new string[] { "DriveLetter", "ProtectionStatus" };
            OnDemandCollectionContext<WmiEncryptableVolumeSnapshot> context = new OnDemandCollectionContext<WmiEncryptableVolumeSnapshot>();
            col.CollectWmiEncryptableVolume(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (WmiEncryptableVolumeSnapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.WmiEncryptableVolume.DefaultView.Table;
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
        public void WmiComputerSystemProbeTest()
        {
            string[] expectedColumns = new string[] { "DomainRole" };
            OnDemandCollectionContext<WmiComputerSystemSnapshot> context = new OnDemandCollectionContext<WmiComputerSystemSnapshot>();
            col.CollectWmiComputerSystem(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (WmiComputerSystemSnapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.WmiComputerSystem.DefaultView.Table;
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
        public void WmiProcessorProbeTest()
        {
            string[] expectedColumns = new string[] { "NumberOfLogicalProcessors", "MaxClockSpeed", "NumberOfCores", "CurrentClockSpeed", "AddressWidth" };
            OnDemandCollectionContext<WmiProcessorSnapshot> context = new OnDemandCollectionContext<WmiProcessorSnapshot>();
            col.CollectWmiProcessor(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (WmiProcessorSnapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.WmiProcessor.DefaultView.Table;
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
        public void WmiPerfOSProcessorProbeTest()
        {
            string[] expectedColumns = new string[] { "Name", "InterruptsPerSec", "PercentProcessorTime", "PercentPrivilegedTime", "PercentInterruptTime", "Frequency_Sys100NS", "Timestamp_Sys100NS" };
            OnDemandCollectionContext<WmiPerfOSProcessorSnapshot> context = new OnDemandCollectionContext<WmiPerfOSProcessorSnapshot>();
            col.CollectWmiPerfOSProcessor(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (WmiPerfOSProcessorSnapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.WmiPerfOSProcessor.DefaultView.Table;
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
        public void WmiNetworkInterfaceProbeTest()
        {
            string[] expectedColumns = new string[] { "Name", "PacketsPerSec", "OutputQueueLength", "PacketsReceivedErrors", "PacketsOutboundErrors", "CurrentBandwidth", "BytesTotalPerSec", "Frequency_Sys100NS", "Timestamp_Sys100NS" };
            OnDemandCollectionContext<WmiNetworkInterfaceSnapshot> context = new OnDemandCollectionContext<WmiNetworkInterfaceSnapshot>();
            col.CollectWmiNetworkInterface(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (WmiNetworkInterfaceSnapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.WmiNetworkInterface.DefaultView.Table;
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
        public void WmiPageFileProbeTest()
        {
            string[] expectedColumns = new string[] { "Name" };
            OnDemandCollectionContext<WmiPageFileSnapshot> context = new OnDemandCollectionContext<WmiPageFileSnapshot>();
            col.CollectWmiPageFile(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (WmiPageFileSnapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.WmiPageFile.DefaultView.Table;
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
        public void WmiPhysicalMemoryProbeTest()
        {
            string[] expectedColumns = new string[] { "Caption", "Capacity" };
            OnDemandCollectionContext<WmiPhysicalMemorySnapshot> context = new OnDemandCollectionContext<WmiPhysicalMemorySnapshot>();
            col.CollectWmiPhysicalMemory(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (WmiPhysicalMemorySnapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.WmiPhysicalMemory.DefaultView.Table;
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
        public void WmiPerfDiskLogicalDiskProbeTest()
        {
            string[] expectedColumns = new string[] { "Name", "AvgDiskQueueLength", "CurrentDiskQueueLength", "SplitIOPerSec", "PercentDiskTime", "PercentDiskTime_Base", "AvgDisksecPerTransfer", "AvgDisksecPerTransfer_Base", "DiskTransfersPerSec", "Timestamp_Sys100NS" };
            OnDemandCollectionContext<WmiPerfDiskLogicalDiskSnapshot> context = new OnDemandCollectionContext<WmiPerfDiskLogicalDiskSnapshot>();
            col.CollectWmiPerfDiskLogicalDisk(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (WmiPerfDiskLogicalDiskSnapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.WmiPerfDiskLogicalDisk.DefaultView.Table;
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
        public void WmiPerfDiskPhysicalDiskProbeTest()
        {
            string[] expectedColumns = new string[] { "Name", "DiskTransfersPerSec", "Frequency_Sys100NS", "Timestamp_Sys100NS" };
            OnDemandCollectionContext<WmiPerfDiskPhysicalDiskSnapshot> context = new OnDemandCollectionContext<WmiPerfDiskPhysicalDiskSnapshot>();
            col.CollectWmiPerfDiskPhysicalDisk(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (WmiPerfDiskPhysicalDiskSnapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.WmiPerfDiskPhysicalDisk.DefaultView.Table;
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
        public void WmiServiceProbeTest()
        {
            string[] expectedColumns = new string[] { "Name", "ProcessId", "State" };
            OnDemandCollectionContext<WmiServiceSnapshot> context = new OnDemandCollectionContext<WmiServiceSnapshot>();
            col.CollectWmiService(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (WmiServiceSnapshot)snapshot;
            {
                var snapshotTable = baseSnapshot.WmiService.DefaultView.Table;
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
        public void QueryPlanProbeTest()
        {
            //string[] expectedColumns = new string[] { "Name", "ProcessId", "State" };
            OnDemandCollectionContext<QueryPlanSnapshot> context = new OnDemandCollectionContext<QueryPlanSnapshot>();
            col.CollectQueryPlan(monitoredServerId, context, null, "select * from PrescriptiveAnalysis");
            var snapshot = context.Wait();
            var baseSnapshot = (QueryPlanSnapshot)snapshot;
            {
                //var snapshotTable = baseSnapshot.WmiService.DefaultView.Table;
                Assert.IsNull(baseSnapshot.Error, TestCaseHelper.SnapshotErrorMessage);
                Assert.IsFalse(baseSnapshot.CollectionFailed, TestCaseHelper.CollectionFailedMessage);
                //Assert.IsNotNull(snapshotTable, TestCaseHelper.TableNullMessage);
                //foreach (string column in expectedColumns)
                //{
                //    Assert.IsTrue(snapshotTable.Columns.Contains(column), TestCaseHelper.GetColumnNameMissingError(column));
                //}
            }
        }

        [TestCase]
        public void DatabasesNameProbeTest()
        {
            OnDemandCollectionContext<DatabaseNamesSnapshot> context = new OnDemandCollectionContext<DatabaseNamesSnapshot>();
            col.CollectDatabaseNames(monitoredServerId, context, null);
            var snapshot = context.Wait();
            var baseSnapshot = (DatabaseNamesSnapshot)snapshot;
            Assert.IsNull(baseSnapshot.Error, TestCaseHelper.SnapshotErrorMessage);
            Assert.IsFalse(baseSnapshot.CollectionFailed, TestCaseHelper.CollectionFailedMessage);
        }

    }
}
