using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics.WaitingObjects;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    public class ProcessorAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 13;
        private static Logger _logX = Logger.GetLogger("ProcessorAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public ProcessorAnalyzer()
        {
            _id = id;
        }

        public override string GetDescription() { return ("Processor analysis"); }

        public override void Analyze(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("ProcessorAnalyzer.Analyze"))
            {
                base.Analyze(sm, conn);
                if (IsHighCpu(sm))
                {
                    AnalyzeCoreUsage(sm);
                    AnalyzePreviousProcessorFrequency(sm);
                    AnalyzeFrequency(sm);
                    AnalyzeMaxDOP(sm);
                    //AnalyzeDatabaseEncryption(sc, conn);
                    AnalyzeProcesses(sm);
                    AnalyzeContextSwitches(sm);
                    AnalyzeEncryptedConnections(sm);
                    AnalyzeInterrupts(sm);
                    AnalyzeEncryptableVolumes(sm);
                    AnalyzeAuditing(sm);
                    AnalyzeVirtualMachine(sm);
                    AnalyzeThemes(sm);
                }
            }
        }

        private void AnalyzeThemes(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzeThemes"))
            {
                CheckCancel();
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.Options) { _logX.Debug("null Options"); return; }
                if (!sm.Options.ProductionServer) { _logX.Debug("not a ProductionServer"); return; }
                _common.ThemesService = true;
            }
        }

        private void AnalyzeVirtualMachine(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzeVirtualMachine"))
            {
                CheckCancel();
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.Options) { _logX.Debug("null Options"); return; }
                if (null == sm.WMIBiosMetrics) { _logX.Debug("null WMIBiosCollector"); return; }
                if (!sm.Options.ProductionServer) { _logX.Debug("not a ProductionServer"); return; }
                if (sm.WMIBiosMetrics.IsVirtualMachine)
                {
                    _logX.Debug("Skip adding recommendation for virtual machine");
                    //_logX.Debug("Adding recommendation for virtual machine");
                    //AddRecommendation(new CpuVirtualMachineRecommendation());
                }
            }
        }

        private void AnalyzeAuditing(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzeAuditing"))
            {
                CheckCancel();
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.Options) { _logX.Debug("null Options"); return; }
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                if (!sm.Options.ProductionServer) { _logX.Debug("not a Production server"); return; }
                if (sm.ServerPropertiesMetrics.C2AuditMode)
                {
                    _logX.Debug("Skip adding recommendation for C2 audit mode being enabled");
                    //_logX.Debug("Adding recommendation for C2 audit mode being enabled");
                    //AddRecommendation(new CpuC2AuditRecommendation());
                }
            }
        }

        private void AnalyzeEncryptableVolumes(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzeEncryptableVolumes"))
            {
                CheckCancel();
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.WMIEncryptableVolumeMetrics) { _logX.Debug("null WMIEncryptableVolumeCollector"); return; }
                if (null == sm.WMIPerfDiskLogicalDiskMetrics) { _logX.Debug("null WMIPerfDiskLogicalDiskCollector"); return; }
                if (null == sm.DatabaseInfoSnapshotMetrics) { _logX.Debug("null DatabaseInfoSnapshotCollector"); return; }
                string[] encryptedDrives = sm.WMIEncryptableVolumeMetrics.DriveLetters;
                if (null == encryptedDrives) { _logX.Debug("null WMIEncryptableVolumeCollector.DriveLetters"); return; }
                if (encryptedDrives.Length <= 0) { _logX.Debug("No encryptable volumes found."); return; }
                _logX.DebugFormat("Checking {0} encryptable volume(s)", encryptedDrives.Length);

                //----------------------------------------------------------------------------
                // If we have encryptable volumes (bitlocker encrypted), check each volume to
                // see if it is being encrypted and if the volume is used for either the page file
                // or sql server data files.
                // 
                List<string> allLogicalDisks = new List<string>();
                WMIEncryptableVolume v;
                bool driveContainsSqlData;
                bool hasPageFile;
                allLogicalDisks.AddRange(sm.WMIPerfDiskLogicalDiskMetrics.LogicalDiskNames);
                if (allLogicalDisks.Contains("_Total")) allLogicalDisks.Remove("_Total");
                if (allLogicalDisks.Count > 1) allLogicalDisks.Sort();
                _logX.DebugFormat("Found {0} logical disks", allLogicalDisks.Count);
                foreach (string d in encryptedDrives)
                {
                    if (string.IsNullOrEmpty(d)) continue;
                    v = sm.WMIEncryptableVolumeMetrics.GetVolume(d);
                    if (null == v) { _logX.DebugFormat("Could not find encryptable volume for {0}", d); continue; }
                    _logX.DebugFormat("EncryptableVolume  DriveLetter:{0}  ProtectionStatus:{1}", v.DriveLetter, v.ProtectionStatus);
                    if (0 == v.ProtectionStatus) { _logX.DebugFormat("Drive {0} is not bitlocker encrypted ({1})", v.DriveLetter, v.ProtectionStatus); continue; }
                    driveContainsSqlData = sm.DatabaseInfoSnapshotMetrics.IsDatabaseHostedOnDrive(v.DriveLetter, allLogicalDisks);
                    hasPageFile = HasPageFile(d, sm);
                    _logX.DebugFormat("DriveLetter:{0}  HasPageFile:{1}  DriveContainsSqlData:{2}", v.DriveLetter, hasPageFile, driveContainsSqlData);
                    //----------------------------------------------------------------------------
                    // If the volume is being used for sql server data files or the page file
                    // add a recommendation.
                    // 
                    if (driveContainsSqlData || hasPageFile)
                    {
                        _logX.Debug("Adding recommendation for encrypted volume");
                        AddRecommendation(new CpuEncryptedVolumeRecommendation(d, v.ProtectionStatus, hasPageFile, driveContainsSqlData));
                    }
                }
            }
        }

        private bool HasPageFile(string d, SnapshotMetrics sm)
        {
            using (_logX.DebugCall(string.Format("HasPageFile({0})", d)))
            {
                return (sm.WMIPageFileMetrics.GetPageFileCount(d) > 0);
            }
        }

        private void AnalyzeInterrupts(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzeInterrupts"))
            {
                CheckCancel();
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.WMIPerfOSProcessorMetrics) { _logX.Debug("null WMIPerfOSProcessorCollector"); return; }
                if (null == sm.WMIPerfDiskPhysicalDiskMetrics) { _logX.Debug("null WMIPerfDiskPhysicalDiskCollector"); return; }
                if (null == sm.WMINetworkInterfaceMetrics) { _logX.Debug("null WMINetworkInterfaceCollector"); return; }
                UInt32 interruptsPerSec = sm.WMIPerfOSProcessorMetrics.GetAvgInterruptsPerSec();
                UInt32 transfersPerSec = sm.WMIPerfDiskPhysicalDiskMetrics.GetAvgDiskTransfersPerSec("_Total");
                UInt64 networkPackets = sm.WMINetworkInterfaceMetrics.AvgPacketsPerSec_TotalForAllCards;
                double adjustedActivity = ((double)(transfersPerSec + networkPackets) / Settings.Default.CPU_ActivityPerSecDenominator) + Settings.Default.CPU_ActivityPerSecAdjustment;
                _logX.DebugFormat("AvgInterruptsPerSec={0}  DiskTransfersPerSec={1}  PacketsPerSec={2}  CPU_ActivityPerSecDenominator={3}  CPU_ActivityPerSecAdjustment={4}  adjustedActivity={5}", interruptsPerSec, transfersPerSec, networkPackets, Settings.Default.CPU_ActivityPerSecDenominator, Settings.Default.CPU_ActivityPerSecAdjustment, adjustedActivity);
                //----------------------------------------------------------------------------
                // If the interrupts per second is less than our adjusted activity amount we 
                // are not experiencing slowdowns due to interrupts so we can exit out.
                // 
                if (interruptsPerSec <= adjustedActivity) return;
                double avgInterruptTime = sm.WMIPerfOSProcessorMetrics.GetAvgPercentInterruptTime();
                _logX.DebugFormat("AvgPercentInterruptTime={0}  CPU_HighPercentInterruptTime={1}", avgInterruptTime, Settings.Default.CPU_HighPercentInterruptTime);
                //----------------------------------------------------------------------------
                // If the processor is not spending a large amount of time processing interrupts, exit.
                // 
                if (avgInterruptTime <= Settings.Default.CPU_HighPercentInterruptTime) return;
                //----------------------------------------------------------------------------
                // At this point we know that a high number of interrupts are being processed
                // so we will try to determine if it is due to network or disk activity.
                // 
                List<string> highQueueDrives;
                if (NetworkAnalyzer.IsNetworkSlow(sm))
                {
                    List<string> cards = sm.WMINetworkInterfaceMetrics.GetCardsWithHighQueueLength();
                    _logX.InfoFormat("Added interrupts recommendation due to slow network ({0})", cards);
                    AddRecommendation(new CpuInterruptsNetworkRecommendation(transfersPerSec + networkPackets, interruptsPerSec, avgInterruptTime, cards));
                    return;
                }
                else if (null != (highQueueDrives = GetDrivesWithHighQueueLengths(sm)))
                {
                    _logX.InfoFormat("Added interrupts recommendation due to drives with high queue lengths ({0})", highQueueDrives);
                    AddRecommendation(new CpuInterruptsDiskRecommendation(transfersPerSec + networkPackets, interruptsPerSec, avgInterruptTime, highQueueDrives));
                    return;
                }
                if ((transfersPerSec + networkPackets) <= 0)
                {
                    _logX.Info("Skipped adding CPU interrupt recommendation due to lack of disk/network activity.");
                    return;
                }
                if (0 == interruptsPerSec)
                {
                    _logX.Info("Skipped adding CPU interrupt recommendation due to lack of interrupts per second.");
                    return;
                }
                if (0.0 == Math.Round((transfersPerSec + networkPackets) / (double)interruptsPerSec, 1))
                {
                    _logX.Info("Skipped adding CPU interrupt recommendation due to low activity to interrupt ratio.");
                    return;
                }
                _logX.Info("Added interrupts recommendation");
                AddRecommendation(new CpuInterruptsRecommendation(transfersPerSec + networkPackets, interruptsPerSec, avgInterruptTime));
            }
        }

        private List<string> GetDrivesWithHighQueueLengths(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("GetDrivesWithHighQueueLengths"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return (null); }
                if (null == sm.WMIPerfDiskLogicalDiskMetrics) { _logX.Debug("null WMIPerfDiskLogicalDiskCollector"); return (null); }
                List<string> d = null;
                double hi = Properties.Settings.Default.IO_HighDiskQueueLength;
                foreach (string n in sm.WMIPerfDiskLogicalDiskMetrics.LogicalDiskNames)
                {
                    if (n.Equals("_total", StringComparison.CurrentCultureIgnoreCase)) continue;
                    if (sm.WMIPerfDiskLogicalDiskMetrics.GetAverageDiskQueueLength(n) > hi)
                    {
                        if (null == d) d = new List<string>();
                        d.Add(n);
                    }
                }
                return (d);
            }
        }

        private void AnalyzeEncryptedConnections(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzeEncryptedConnections"))
            {
                CheckCancel();
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                if (null == sm.SampledServerResourcesMetrics) { _logX.Debug("null SampledServerResourcesCollector"); return; }
                _logX.DebugFormat("EncryptedConnections={0}", sm.ServerPropertiesMetrics.EncryptedConnections);
                if (sm.ServerPropertiesMetrics.EncryptedConnections > 0)
                {
                    _logX.DebugFormat("Connections:{0}  BatchesPerSec:{1}", sm.SampledServerResourcesMetrics.Connections, sm.SampledServerResourcesMetrics.BatchReqSec);
                    _logX.Debug("Added encrypted connections recommendation");
                    _common.EncryptedConnections = true;
                }
            }
        }

        private void AnalyzeContextSwitches(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzeContextSwitches"))
            {
                CheckCancel();
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.WMIPerfOSSystemMetrics) { _logX.Debug("null WMIPerfOSSystemCollector"); return; }
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                if (sm.ServerPropertiesMetrics.LightweightPooling) { _logX.Debug("Lightweight pooling already enabled.  No need to continue."); return; }
                UInt32 contextSwitches = sm.WMIPerfOSSystemMetrics.ContextSwitchesPerSec;
                _logX.InfoFormat("ContextSwitchesPerSec:{0}  Threshold:{1}", contextSwitches, Settings.Default.CPU_ContextSwitchesPerSecond);
                //----------------------------------------------------------------------------
                // If our context switches per second are not over our threshold, exit out since 
                // we do not care to check if too many threads are causing the cpu's to thrash.
                // 
                if (contextSwitches <= Settings.Default.CPU_ContextSwitchesPerSecond) return;

                if (null == sm.WMIPerfOSProcessorMetrics) { _logX.Debug("null WMIPerfOSProcessorCollector"); return; }
                int privilegedTime = sm.WMIPerfOSProcessorMetrics.GetAvgPrivilegedTime();
                _logX.InfoFormat("AvgPrivilegedTime:{0}  Threshold:{1}", privilegedTime, Settings.Default.CPU_AvgPrivilegedTimePercent);
                //----------------------------------------------------------------------------
                // If the cpu is not spending alot of time in privileged time, no need to 
                // continue since we are not overloading the cpu with work switching between threads.
                // 
                if (privilegedTime <= Settings.Default.CPU_AvgPrivilegedTimePercent) return;

                if (null == sm.WMIProcessorMetrics) { _logX.Debug("null WMIProcessCollector"); return; }
                WMIProcess proc = sm.WMIProcessMetrics.GetProcessById(sm.ServerPropertiesMetrics.ServerProcessID);
                _logX.DebugFormat("pulling WMIProcess for pid:{0}", sm.ServerPropertiesMetrics.ServerProcessID);
                if (null == proc) { _logX.Debug("null WMIProcess - could not find server process!"); return; }
                _logX.InfoFormat("Thread count:{0}  pid:{1}  CPU_MaxSqlThreadCount:{2}", proc.ThreadCount, proc.ProcessId, Settings.Default.CPU_MaxSqlThreadCount);
                if (proc.ThreadCount <= Settings.Default.CPU_MaxSqlThreadCount) return;
                int processorTime = sm.WMIPerfOSProcessorMetrics.GetAvgCpu(sm.ServerPropertiesMetrics.AffinityMask);
                _logX.InfoFormat("ProcessorTimePercent:{0}  AffinityMask:{1}  CPU_LightweightPoolingMaxProcessorTimePercent:{2}", processorTime, sm.ServerPropertiesMetrics.AffinityMask, Settings.Default.CPU_LightweightPoolingMinProcessorTimePercent);
                if (processorTime < Settings.Default.CPU_LightweightPoolingMinProcessorTimePercent) return;
                int processorCount = sm.WMIPerfOSProcessorMetrics.GetProcessorCount(sm.ServerPropertiesMetrics.AffinityMask);
                _logX.InfoFormat("ProcessorCount:{0}  AffinityMask:{1}  CPU_LightweightPoolingMinProcessorCount:{2}", processorCount, sm.ServerPropertiesMetrics.AffinityMask, Settings.Default.CPU_LightweightPoolingMinProcessorCount);
                if (processorCount < Settings.Default.CPU_LightweightPoolingMinProcessorCount) return;
                if (!sm.ServerPropertiesMetrics.LightweightPooling)
                {
                    _logX.Info("Lightweight pooling recommendation added.");
                    _logX.Info("The add of the lightweight pooling recommendation was skipped!");
                    //AddRecommendation(new CpuLightweightPoolingRecommendation(contextSwitches, privilegedTime, proc.ThreadCount));
                }
            }
        }

        private void AnalyzeProcesses(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzeProcesses"))
            {
                CheckCancel();
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.Options) { _logX.Debug("null Options"); return; }
                if (!sm.Options.ProductionServer) { _logX.Debug("not a ProductionServer server"); return; }
                _common.MultipleInstances = true;
            }
        }

        //private void AnalyzeDatabaseEncryption(SnapshotCollector sc, SqlConnection conn)
        //{
        //    using (_logX.DebugCall("AnalyzeDatabaseEncryption"))
        //    {
        //        if (null == sc) { _logX.Debug("null SnapshotCollector"); return; }
        //        if (null == conn) { _logX.Debug("null SqlConnection"); return; }

        //        CheckCancel();
        //        ServerVersion ver = new ServerVersion(conn.ServerVersion);
        //        if (ver.Major < (int)SSVer_e.SS_2008) { _logX.DebugFormat("Database encryption not supported for {0}", ver.ToString()); return; }
        //        DataTable dt = GetDataTable("EncryptedDatabases", ver, conn);
        //        if (null == dt) { _logX.Debug("null DataTable of encrypted databases"); return; }
        //        if (null == dt.Rows) { _logX.Debug("null DataTable.Rows of encrypted databases"); return; }
        //        if (dt.Rows.Count <= 0) { _logX.Debug("No encrypted databases found"); return; }
        //        AnalyzeEncryptIO(ver, conn);
        //        AnalyzeEncryptAlgorithm(dt, conn);
        //    }

        //}

        //private void AnalyzeEncryptAlgorithm(DataTable dt, SqlConnection conn)
        //{
        //    using (_logX.DebugCall("AnalyzeEncryptAlgorithm"))
        //    {
        //        if (null == dt) { _logX.Debug("null DataTable of encrypted database algorithm"); return; }
        //        if (null == dt.Rows) { _logX.Debug("null DataTable.Rows of encrypted database algorithm"); return; }
        //        if (dt.Rows.Count <= 0) { _logX.Debug("No encrypted databases found"); return; }
        //        string name;
        //        string algorithm;
        //        int keyLength;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            name = DataHelper.ToString(dr, "DatabaseName");
        //            algorithm = DataHelper.ToString(dr, "Algorithm");
        //            keyLength = DataHelper.ToInt32(dr, "KeyLength");
        //            if (string.IsNullOrEmpty(algorithm) || string.IsNullOrEmpty(name)) continue;
        //            _logX.InfoFormat("Database:{0}  Algorithm:{1}  KeyLength:{2}", name, algorithm, keyLength);
        //            if ((0 != string.Compare(algorithm, "AES", true)) || (128 != keyLength))
        //            {
        //                _logX.Info("Add encryption algorith recommendation");
        //                AddRecommendation(new CpuDatabaseEncryptionAlgorithmRecommendation(name, algorithm, keyLength));
        //            }
        //        }
        //    }
        //}

        //private void AnalyzeEncryptIO(ServerVersion ver, SqlConnection conn)
        //{
        //    using (_logX.DebugCall("AnalyzeEncryptIO"))
        //    {
        //        CheckCancel();
        //        DataTable dt = GetDataTable("DatabaseIOBytes", ver, conn);
        //        if (null == dt) { _logX.Debug("null DataTable of encrypted database io bytes"); return; }
        //        if (null == dt.Rows) { _logX.Debug("null DataTable.Rows of encrypted database io bytes"); return; }
        //        if (dt.Rows.Count <= 0) { _logX.Debug("No databases found"); return; }

        //        CheckCancel();
        //        UInt64 bytes = 0;
        //        UInt64 total = 0;
        //        UInt64 encTotal = 0;
        //        string name;
        //        bool encrypted = false;
        //        List<string> encryptedDatabases = new List<string>();
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            bytes = DataHelper.ToUInt64(dr, "IO");
        //            encrypted = DataHelper.ToBoolean(dr, "Encrypted");
        //            name = DataHelper.ToString(dr, "DatabaseName");
        //            _logX.DebugFormat("Database:{0}  IO:{1}  Encrypted:{2}", name, bytes, encrypted);
        //            if (encrypted)
        //            {
        //                encTotal += bytes;
        //                encryptedDatabases.Add(name);
        //            }
        //            total += bytes;
        //        }
        //        _logX.InfoFormat("TotalIO:{0}  MaxEncryptedPercent:{1}  EncryptedBytes:{2}", total, Settings.Default.CPU_MaxEncryptedIOPercent, encTotal);
        //        if (encTotal <= 0) return;
        //        double encryptedPercent = (encTotal * 100.0) / total;
        //        if (Math.Round(encryptedPercent) > Settings.Default.CPU_MaxEncryptedIOPercent)
        //        {
        //            _logX.Info("Encrypted database io recommendation added.");
        //            AddRecommendation(new CpuEncryptedDatabaseIORecommendation(encryptedDatabases, encryptedPercent, encTotal, total));
        //        }
        //    }
        //}

        //private DataTable GetDataTable(string batchName, ServerVersion ver, SqlConnection conn)
        //{
        //    using (_logX.DebugCall(string.Format("GetDataTable({0})", batchName)))
        //    {
        //        try
        //        {
        //            string sql = Batches.BatchFinder.GetBatch(batchName, ver);
        //            if (string.IsNullOrEmpty(sql)) { _logX.Debug("no sql text"); return (null); }

        //            using (SqlDataAdapter a = new SqlDataAdapter(sql, conn))
        //            {
        //                DataTable dt = new DataTable();
        //                a.SelectCommand.CommandTimeout = BatchConstants.DefaultCommandTimeout;
        //                a.Fill(dt);
        //                return (dt);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ExceptionLogger.Log(_logX, string.Format("GetDataTable({0}) Exception:", batchName), ex);
        //        }
        //    }
        //    return (null);
        //}

        private void AnalyzeMaxDOP(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzeMaxDOP"))
            {
                CheckCancel();
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.Options) { _logX.Debug("null Options"); return; }
                if (!sm.Options.OLTPServer) { _logX.Debug("Exit MaxDOP analysis due to the server not being an OLTP server."); return; }

                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                if (null == sm.WMIPerfOSProcessorMetrics) { _logX.Debug("null WMIPerfOSProcessorCollector"); return; }
                if (null == sm.WaitStatsMetrics) { _logX.Debug("null WaitStatsCollector"); return; }
                if (null == sm.WaitStatsMetrics.Stats) { _logX.Debug("null WaitStatsCollector.Stats"); return; }

                int cpu = sm.WMIPerfOSProcessorMetrics.GetProcessorCount(sm.ServerPropertiesMetrics.AffinityMask);
                _logX.InfoFormat("MaxDOP:{0} AffinityMask:{1}  SQL Server CPU:{2}", sm.ServerPropertiesMetrics.MaxDOP, sm.ServerPropertiesMetrics.AffinityMask, cpu);

                if (sm.WaitStatsMetrics.IsWaitHigh("CXPACKET", 5))
                {
                    if (sm.ServerPropertiesMetrics.MaxDOP != 1)
                    {
                        _logX.Debug("Add recommendation to set MaxDOP to one due to a high number of CXPACKET waits on an OLTP server.");
                        AffectedBatches ab = null;
                        if (null != sm.WaitingBatchesMetrics)
                        {
                            WaitingBatches wb = sm.WaitingBatchesMetrics.GetWaitingBatches("CXPACKET");
                            if (null != wb)
                            {
                                ab = wb.GetAffectedBatches();
                            }
                        }
                        AddRecommendation(new CpuDisableParallelismRecommendation(sm.ServerPropertiesMetrics.MaxDOP, cpu, ab));
                    }
                }
                else if (sm.ServerPropertiesMetrics.MaxDOP > 1)
                {
                    _logX.InfoFormat("Suggested MaxDOP of {0} based on cpu count of {1} with a max dop per cpu of {2} ", (cpu * Settings.Default.CPU_MaxDopPerCPU), cpu, Settings.Default.CPU_MaxDopPerCPU);
                    if ((sm.ServerPropertiesMetrics.MaxDOP > (cpu * Settings.Default.CPU_MaxDopPerCPU)) || (sm.ServerPropertiesMetrics.MaxDOP > 8))
                    {
                        AddMaxDopRecommendation((int)Math.Round(cpu * Settings.Default.CPU_MaxDopPerCPU), sm.ServerPropertiesMetrics.MaxDOP, cpu);
                    }
                }
                else if (0 == sm.ServerPropertiesMetrics.MaxDOP)
                {
                    _logX.InfoFormat("CPU_MaxCPUWhenMaxDOPZero:{0}", Settings.Default.CPU_MaxCPUWhenMaxDopZero);
                    if (cpu > Settings.Default.CPU_MaxCPUWhenMaxDopZero)
                    {
                        AddMaxDopRecommendation((int)Math.Round(cpu * Settings.Default.CPU_MaxDopPerCPU), sm.ServerPropertiesMetrics.MaxDOP, cpu);
                    }
                }
            }
        }

        private void AddMaxDopRecommendation(int suggestedMaxDOP, UInt32 maxDop, int cpu)
        {
            _logX.InfoFormat("MaxDOP recommendation added with suggested max dop of {0}.  MaxDOP={1}  CPU={2}", suggestedMaxDOP, maxDop, cpu);
            AddRecommendation(new CpuMaxDOPRecommendation(suggestedMaxDOP, maxDop, cpu));
        }

        private void AnalyzeFrequency(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzeFrequency"))
            {
                CheckCancel();
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.WMIProcessorMetrics) { _logX.Debug("null WMIProcessorCollector"); return; }
                if (!sm.WMIProcessorMetrics.HitMaxClockSpeedOnAllProcessors)
                {
                    _logX.Info("Max clock speed was never reached.");
                    AddRecommendation(new CpuFullFrequencyRecommendation(sm.WMIProcessorMetrics.AvgFreqMaxPercentage));
                }
                else if (!sm.WMIProcessorMetrics.SustainedMaxClockSpeed)
                {
                    _logX.Info("Max clock speed was not sustained throughout the analysis.");
                    AddRecommendation(new CpuConstantFrequencyRecommendation(sm.WMIProcessorMetrics.AvgFreqMaxPercentage));
                }
            }
        }

        private void AnalyzePreviousProcessorFrequency(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzePreviousProcessorFrequency"))
            {
                CheckCancel();
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.Previous) { _logX.Debug("null Previous"); return; }
                if (null == sm.Current) { _logX.Debug("null Current"); return; }
                _logX.InfoFormat("MaxClockSpeed - Current:{0}  Previous:{1}", sm.Current.TotalMaxClockSpeed, sm.Previous.TotalMaxClockSpeed);
                _logX.InfoFormat("NumberOfLogicalProcessors - Current:{0}  Previous:{1}", sm.Current.TotalNumberOfLogicalProcessors, sm.Previous.TotalNumberOfLogicalProcessors);
                if (sm.Current.TotalMaxClockSpeed < sm.Previous.TotalMaxClockSpeed)
                {
                    _logX.Info("Clock speed reduced recommendation added.");
                    AddRecommendation(new CpuClockSpeedLostRecommendation(sm.Current.TotalNumberOfLogicalProcessors,
                                                                                sm.Previous.TotalNumberOfLogicalProcessors,
                                                                                sm.Current.TotalMaxClockSpeed,
                                                                                sm.Previous.TotalMaxClockSpeed
                                                                                ));
                }
            }
        }

        public static bool IsHighCpu(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("IsHighCpu"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return (false); }
                if (null == sm.WMIPerfOSProcessorMetrics) { _logX.Debug("null WMIPerfOSProcessorCollector"); return (false); }
                UInt64 affinity = 0;
                int cpu = 0;
                if (null != sm.ServerPropertiesMetrics) affinity = sm.ServerPropertiesMetrics.AffinityMask;
                cpu = sm.WMIPerfOSProcessorMetrics.GetAvgCpu(affinity);
                _logX.InfoFormat("Average CPU {0}% based on affinity 0x{1} - threshold {2}%", cpu, affinity.ToString("X"), Settings.Default.CPU_HighAvgLoadPercentage);
                if (Settings.Default.CPU_HighAvgLoadPercentage < cpu)
                {
                    _logX.InfoFormat("High avg load of {0}% has been execeeded by current load of {1}%", Settings.Default.CPU_HighAvgLoadPercentage, cpu);
                    return (true);
                }
                if (null == sm.WMIPerfOSSystemMetrics) { _logX.Debug("null WMIPerfOSSystemCollector"); return (false); }
                double avgQueue = sm.WMIPerfOSSystemMetrics.AvgProcessorQueueLength;
                cpu = sm.WMIPerfOSProcessorMetrics.GetProcessorCount(affinity);
                _logX.InfoFormat("SQL Server can use {0} processors.  The average queue length is {1}.  High avg per cpu {2}", cpu, avgQueue, Settings.Default.CPU_HighAvgQueueLength);
                if (avgQueue > Settings.Default.CPU_HighAvgQueueLength)
                {
                    if ((avgQueue / cpu) > Settings.Default.CPU_HighAvgQueueLength)
                    {
                        _logX.InfoFormat("Average queue per processor of {0} exceeds high average of {1}.", (avgQueue / cpu), Settings.Default.CPU_HighAvgQueueLength);
                        return (true);
                    }
                }
            }
            _logX.Info("The cpu is not being over utilized.");
            return (false);
        }

        /// <summary>
        /// make sure that the compilations per second are not over 10% of the batches per second.
        /// </summary>
        /// <param name="sc"></param>
        private void AnalyzeCoreUsage(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzeCoreUsage"))
            {
                CheckCancel();
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.WMIPerfOSProcessorMetrics) { _logX.Debug("null WMIPerfOSProcessorCollector"); return; }
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                //----------------------------------------------------------------------------
                // If the affinity mask is not set we do not need to continue to test for the 
                // core usage recommendations since all of the cores are available for sql server
                // to use.
                // 
                if (0 == sm.ServerPropertiesMetrics.AffinityMask) return;
                _logX.InfoFormat("Affinity is set to 0x{0}.", sm.ServerPropertiesMetrics.AffinityMask.ToString("X"));
                //----------------------------------------------------------------------------
                // If the affinity mask that is set allows for all the cores to be used, don't
                // continue with the core usage recommendations.
                // 
                int cpuCountTotal = sm.WMIPerfOSProcessorMetrics.GetProcessorCount(0);
                int cpuCountAllowed = sm.WMIPerfOSProcessorMetrics.GetProcessorCount(sm.ServerPropertiesMetrics.AffinityMask);
                _logX.InfoFormat("CpuCount:{0}  Affinity adjusted count:{1}", cpuCountTotal, cpuCountAllowed);
                if (cpuCountAllowed >= cpuCountTotal) return;
                if ((null != sm.Current) && (null != sm.Previous))
                {
                    _logX.InfoFormat("Allowed processor count:{0}  Previous allowed processor count:{1}", sm.Current.AllowedProcessorCount, sm.Previous.AllowedProcessorCount);
                    if (sm.Current.AllowedProcessorCount < sm.Previous.AllowedProcessorCount)
                    {
                        _logX.Info("Cpu Affinity Reduced recommendation added");
                        AddRecommendation(new CpuAffinityReducedRecommendation(cpuCountTotal,
                                                                                    sm.Current.AllowedProcessorCount,
                                                                                    sm.Previous.AllowedProcessorCount,
                                                                                    sm.WMIPerfOSProcessorMetrics.GetAvgCpu(sm.ServerPropertiesMetrics.AffinityMask),
                                                                                    sm.ServerPropertiesMetrics.AffinityMask
                                                                                ));
                        return;
                    }
                }
                _logX.Info("Cpu Affinity recommendation added");
                AddRecommendation(new CpuAffinityRecommendation(cpuCountTotal, cpuCountAllowed, sm.WMIPerfOSProcessorMetrics.GetAvgCpu(sm.ServerPropertiesMetrics.AffinityMask), sm.ServerPropertiesMetrics.AffinityMask));
            }
        }
    }
}
