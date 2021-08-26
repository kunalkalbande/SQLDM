using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    internal class IoAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 8;
        private static Logger _logX = Logger.GetLogger("IoAnalyzer");
        protected override Logger GetLogger() { return (_logX); }
        public IoAnalyzer()
        {
            _id = id;
        }
        public override string GetDescription() { return ("Disk IO analysis"); }
        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            using (_logX.DebugCall("IoAnalyzer.Analyze"))
            {
                base.Analyze(sm, conn);

                int totalRecommendations = 0;
                int disksAnalyzed = 0;

                bool hasSeManageVolumeName = HasSeManageVolumeName(sm);

                List<string> allLogicalDisks = new List<string>();
                allLogicalDisks.AddRange(sm.WMIPerfDiskLogicalDiskMetrics.LogicalDiskNames);
                if (allLogicalDisks.Contains("_Total"))
                    allLogicalDisks.Remove("_Total");
                if (allLogicalDisks.Count > 1)
                    allLogicalDisks.Sort();

                _logX.DebugFormat("System has {0} logical disks", allLogicalDisks.Count);

                double highDiskQueueLengthTarget = Properties.Settings.Default.IO_HighDiskQueueLength;
                foreach (string logicalDiskName in sm.WMIPerfDiskLogicalDiskMetrics.LogicalDiskNames)
                {
                    CheckCancel();

                    if (logicalDiskName.Equals("_total", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    double? avgDiskQueueLength = sm.WMIPerfDiskLogicalDiskMetrics.GetAverageDiskQueueLength(logicalDiskName);
                    if (avgDiskQueueLength.HasValue)
                        _logX.DebugFormat("Disk '{0}' has an avg. disk queue length of {1}", logicalDiskName, avgDiskQueueLength.Value);
                    else
                    {
                        _logX.DebugFormat("Disk '{0}' has a disk queue length of unknown - skipping", logicalDiskName);
                        continue;
                    }

                    if (avgDiskQueueLength.Value <= highDiskQueueLengthTarget)
                    {
                        _logX.DebugFormat("Disk '{0}' skipped because avg. disk queue length did not exceed threshold of {1}.", logicalDiskName, highDiskQueueLengthTarget);
                        continue;
                    }

                    disksAnalyzed++;

                    int perDiskRecommendationCount = 0;

                    double? avgDiskSecPerTransfer = sm.WMIPerfDiskLogicalDiskMetrics.GetAverageDiskSecondsPerTransfer(logicalDiskName);
                    if (avgDiskSecPerTransfer.HasValue)
                        _logX.DebugFormat("Disk '{0}' has an avg. disk sec/transfer of {1}", logicalDiskName, avgDiskSecPerTransfer.Value);
                    else
                        _logX.DebugFormat("Disk '{0}' has a avg. disk sec/transfer of unknown", logicalDiskName);

                    bool driveContainsMdfData = sm.DatabaseInfoSnapshotMetrics.IsDatabaseDataHostedOnDrive(logicalDiskName, allLogicalDisks);
                    _logX.DebugFormat("Disk '{0}' does {1}contain a database data file", logicalDiskName, driveContainsMdfData ? "" : "not ");
                    bool driveContainsLdfData = sm.DatabaseInfoSnapshotMetrics.IsDatabaseLogsHostedOnDrive(logicalDiskName, allLogicalDisks);
                    _logX.DebugFormat("Disk '{0}' does {1}contain a database log file", logicalDiskName, driveContainsLdfData ? "" : "not ");

                    if (HasPageFile(logicalDiskName, sm))
                    {
                        _logX.DebugFormat("Disk '{0}' contains a page file", logicalDiskName);
                        if (driveContainsMdfData || driveContainsLdfData)
                        {
                            double avgPagesPerSecond = Math.Round(sm.WMIPerfOSMemoryMetrics.AvgPagesPerSecond, 2);
                            _logX.DebugFormat("Disk '{0}' has memory pages per second of {1}", logicalDiskName, avgPagesPerSecond);
                            if (avgPagesPerSecond > Properties.Settings.Default.IO_HighPagesPerSecond)
                            {
                                AddDiskStrugglingFinding1(logicalDiskName, sm, avgDiskQueueLength.Value, avgDiskSecPerTransfer, avgPagesPerSecond);
                                totalRecommendations++;
                                perDiskRecommendationCount++;
                            }
                            else
                            {
                                AddDiskStrugglingFinding2(logicalDiskName, sm, avgDiskQueueLength.Value, avgDiskSecPerTransfer);
                                totalRecommendations++;
                                perDiskRecommendationCount++;
                            }
                        }
                    }
                    else
                        _logX.DebugFormat("Disk '{0}' does not contain a page file", logicalDiskName);


                    if (driveContainsMdfData && driveContainsLdfData)
                    {
                        double? diskTimePercent = sm.WMIPerfDiskLogicalDiskMetrics.GetAveragePercentDiskTime(logicalDiskName);
                        if (diskTimePercent.HasValue)
                        {
                            _logX.DebugFormat("Disk '{0}' has avg pct disk time of {1}", logicalDiskName, diskTimePercent.Value);
                            if (diskTimePercent > Properties.Settings.Default.IO_HighPercentDiskTime)
                            {

                                double? diskTransPerSec = sm.WMIPerfDiskLogicalDiskMetrics.GetAverageDiskTransfersPerSecond(logicalDiskName);
                                double? splitIoPerSec = sm.WMIPerfDiskLogicalDiskMetrics.GetAverageSplitIoPerSecond(logicalDiskName);
                                if (splitIoPerSec.HasValue && diskTransPerSec.HasValue)
                                {
                                    double splitRatio = (diskTransPerSec.Value != 0.0)
                                            ? splitIoPerSec.Value / diskTransPerSec.Value
                                            : 0.0;

                                    _logX.DebugFormat("Disk '{0}' split io/sec is {1} and disk transfers/sec % is {2} for a ratio of {3}", logicalDiskName, splitIoPerSec.Value, diskTransPerSec.Value, splitRatio);
                                    if (splitRatio > Properties.Settings.Default.IO_HighSplitIOToDiskTransfersPercentage)
                                    {
                                        AddDiskStrugglingFinding3(logicalDiskName, sm, avgDiskQueueLength.Value, avgDiskSecPerTransfer, splitRatio);
                                        totalRecommendations++;
                                        perDiskRecommendationCount++;
                                    }
                                    else
                                    {
                                        ulong blockSize = GetAllocationSize(logicalDiskName, sm);
                                        _logX.DebugFormat("Disk '{0}' has a disk allocation size of {1}", logicalDiskName, blockSize);
                                        if (blockSize != 0 && blockSize < Properties.Settings.Default.IO_MinVolumeBlockSize)
                                        {
                                            AddBadAllocationSizeFinding(logicalDiskName, sm, blockSize, avgDiskSecPerTransfer, avgDiskQueueLength.Value);
                                            totalRecommendations++;
                                            perDiskRecommendationCount++;
                                        }
                                        else
                                        {
                                            if (blockSize == 0)
                                                _logX.DebugFormat("Disk '{0}' skipping disk allocation size check", logicalDiskName);

                                            double? diskSecPerTransfer = null;
                                            if (diskTimePercent.HasValue && diskTimePercent.Value > 0)
                                            {
                                                diskSecPerTransfer = sm.WMIPerfDiskLogicalDiskMetrics.GetAverageDiskSecondsPerTransfer(logicalDiskName);
                                            }

                                            _logX.DebugFormat("Disk '{0}' disk sec/transfer is {1} and disk time % is {2}", logicalDiskName, diskSecPerTransfer, diskTimePercent);
                                            if ((diskSecPerTransfer.HasValue) &&
                                                ((diskTimePercent * diskSecPerTransfer) > Properties.Settings.Default.IO_DiskSecPerTransTimesPctDiskTime))
                                            {
                                                AddDiskStrugglingFinding4(logicalDiskName, sm, avgDiskQueueLength.Value, avgDiskSecPerTransfer);
                                                totalRecommendations++;
                                                perDiskRecommendationCount++;
                                            }
                                            else
                                            {
                                                AddStrugglingDiskFinding5(logicalDiskName, sm, avgDiskQueueLength.Value, avgDiskSecPerTransfer);
                                                totalRecommendations++;
                                                perDiskRecommendationCount++;
                                            }
                                        }
                                    }
                                }
                                else
                                    _logX.DebugFormat("Disk '{0}' skipped 50-53 because of missing split io/sec ({1}) or disk transfers/sec ({2})", logicalDiskName, splitIoPerSec, diskTransPerSec);
                            }
                            else
                                _logX.DebugFormat("Disk '{0}' skipped 50-53 due to avg pct disk time threshold of {1}", logicalDiskName, Properties.Settings.Default.IO_HighPercentDiskTime);
                        }
                        else
                            _logX.DebugFormat("Disk '{0}' skipped 50-53 due to percent disk time threshold of {1}", logicalDiskName, Properties.Settings.Default.IO_HighPercentDiskTime);
                    }
                }

                if (disksAnalyzed == 0)
                {
                    _logX.InfoFormat("Disks analyzed is 0.  Checking IO Wait Percentage against threshold: {0}", Properties.Settings.Default.IO_HighIOWaitsPercentOfTotal);
                    if (IOWaitPercentage(sm) > Properties.Settings.Default.IO_HighIOWaitsPercentOfTotal)
                        AddIoSbsStressedFinding(sm);
                    return;
                }

                if (!hasSeManageVolumeName)
                {
                    AddNeedManageVolumnNamePermissionFinding(sm);
                    totalRecommendations++;
                }

                totalRecommendations += PerformTempDbChecks(sm, conn);

                CheckCancel();
                PerformObscureChecks(sm, ref totalRecommendations);

                CheckCancel();
                //foreach (DatabaseInfoSnapshot snapshot in sm.DatabaseInfoSnapshotMetrics.Find(dbinfo => dbinfo.IsAutoShrinkEnabled))
                foreach (DatabaseInfoSnapshot snapshot in sm.DatabaseInfoSnapshotMetrics.FindAutoShrinkEnabledDb())
                {
                    AddAutoShrinkRecommendation(sm, snapshot);
                    totalRecommendations++;
                }

                CheckCancel();
                int filFactorThreshold = Properties.Settings.Default.IO_IOFillFactorThresholdMedium;
                //foreach (WorstDatabaseFillFactorSnapshot snapshot in sm.WorstIndexFillFactorCollector.Find(worst => worst.FillFactor < filFactorThreshold))
                foreach (WorstDatabaseFillFactorSnapshot snapshot in sm.WorstIndexFillFactorMetrics.Find(filFactorThreshold))
                {
                    AddFillFactorRecommendation(sm, snapshot);
                    totalRecommendations++;
                }
            }
        }

        private void AddFillFactorRecommendation(SnapshotMetrics sm, WorstDatabaseFillFactorSnapshot snapshot)
        {
            FillFactorRecommendation recommendation = new FillFactorRecommendation();
            recommendation.DatabaseName = snapshot.DatabaseName;
            recommendation.TableName = snapshot.TableName;
            recommendation.SchemaName = snapshot.SchemaName;
            recommendation.FillFactor = snapshot.FillFactor;
            recommendation.DataSizeMB = snapshot.DataSizeMB;
            recommendation.IndexSizeMB = snapshot.IndexsizeMB;
            recommendation.IndexName = snapshot.IndexName;

            AddRecommendation(recommendation);
        }

        private void AddAutoShrinkRecommendation(SnapshotMetrics sm, DatabaseInfoSnapshot snapshot)
        {
            DatabaseAutoShrinkRecommendation recommendation = new DatabaseAutoShrinkRecommendation(sm.Options.ProductionServer);
            recommendation.DatabaseName = snapshot.DatabaseName;
            AddRecommendation(recommendation);
        }

        private int PerformTempDbChecks(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            using (_logX.DebugCall("PerformTempDbChecks"))
            {
                int count = 0;

                //DatabaseInfoSnapshot snapshot = sm.DatabaseInfoSnapshotMetrics.FindFirstDatabase(db => db.DatabaseId == 2);
                DatabaseInfoSnapshot snapshot = sm.DatabaseInfoSnapshotMetrics.FindFirstDatabaseById(2);
                if (snapshot != null)
                {
                    //DatabaseFileInfo[] dataFiles = snapshot.FindFiles(db => db.TypeId == 0).ToArray();
                    IEnumerable<DatabaseFileInfo> dF = snapshot.FindFilesByTypeId(0);
                    int dataFilesCount = 0;
                    foreach (var item in dF)
                    {
                        dataFilesCount++;
                    }

                    DatabaseFileInfo[] dataFiles = new DatabaseFileInfo[dataFilesCount];
                    System.Collections.IEnumerator e = dF.GetEnumerator();
                    int index=0;
                    while (e.MoveNext())
                    {
                        dataFiles[index] = (DatabaseFileInfo)e.Current;
                        index++;
                    }

                    //long smallest = dataFiles.Min(db => db.Size);
                    long smallest = long.MaxValue;

                    foreach (DatabaseFileInfo dbf in dataFiles)
                    {
                        if (dbf.Size < smallest)
                        {
                            smallest = dbf.Size;
                        }
                    }

                    if (dataFiles.Length > 1)
                    {
                        //long largest = dataFiles.Max(db => db.Size);
                        long largest = 0;
                        foreach (DatabaseFileInfo dbf in dataFiles)
                        {
                            if (dbf.Size > largest)
                            {
                                largest = dbf.Size;
                            }
                        }

                        _logX.DebugFormat("tempdb - smallest is {0} and largest is {1}", smallest, largest);
                        if (smallest != largest)
                        {
                            AddTempDbDataFileMismatchFinding(sm, snapshot, smallest, largest);
                            count++;
                        }
                        else if (IsAutogrowthEnabled(conn, dataFiles))
                        {
                            AddTempDbAutogrowthFinding();
                            count++;
                        }
                    }
                    else
                        _logX.DebugFormat("tempdb size is {0}", smallest);

                    //long totalInitial = dataFiles.Sum(file => file.InitialSize);
                    long totalInitial = 0;
                    foreach (DatabaseFileInfo dbf in dataFiles)
                    {
                        totalInitial += dbf.InitialSize;
                    }

                    //long totalCurrent = dataFiles.Sum(file => file.Size);
                    long totalCurrent = 0;
                    foreach (DatabaseFileInfo dbf in dataFiles)
                    {
                        totalCurrent += dbf.Size;
                    }

                    _logX.DebugFormat("tempdb - total data size is {0}; total initial size is {1}", totalCurrent, totalInitial);
                    if (totalCurrent > totalInitial * 5)
                    {
                        AddTempDbInitialSizeFinding(sm, snapshot, totalInitial, totalCurrent);
                        count++;
                    }

                    int processorCount = sm.WMIPerfOSProcessorMetrics.GetProcessorCount(sm.ServerPropertiesMetrics.AffinityMask);
                    _logX.DebugFormat("Found {0} processors (AffinityMask={1})", processorCount, sm.ServerPropertiesMetrics.AffinityMask);

                    double fileProcessorRatio = (processorCount > 0) ? ((double)dataFiles.Length) / processorCount : 1.0;
                    _logX.DebugFormat("There are {0} data files.  The file/processor ratio is {1}.", dataFiles.Length, fileProcessorRatio);

                    if (1 == processorCount)
                    {
                        if (fileProcessorRatio > 1.0)
                        {
                            AddTooManyTempDbFilesFinding(sm, processorCount, dataFiles.Length);
                            count++;
                        }
                    }
                    else
                    {
                        if (dataFiles.Length > 8 || fileProcessorRatio > 0.5)
                        {
                            AddTooManyTempDbFilesFinding(sm, processorCount, dataFiles.Length);
                            count++;
                        }
                        else
                        {
                            if (dataFiles.Length < 8 && fileProcessorRatio < 0.5)
                            {
                                if (sm.SampledServerResourcesMetrics.SumTempDbMetadataWaits > Properties.Settings.Default.IO_HighTempDbMetadataWaits)
                                {
                                    int status = GetTraceFlagStatus(conn, 1118);
                                    AddTempDbWaitFinding(sm, processorCount, sm.SampledServerResourcesMetrics.TempDbMetadataWaitingSampleCounts, status);
                                    count++;
                                }
                                else if (fileProcessorRatio < 0.5)
                                {
                                    _logX.Debug("Deprecated recommendation SDR-D12 will not be added to the analysis results");
                                    //AddTempDbOptimizationFinding(sc, processorCount, dataFiles.Length);
                                    //count++;    This recommendation has been deprecated
                                }
                            }
                        }
                    }

                    string recoveryModel = sm.ServerPropertiesMetrics.TempDbRecoveryModel;
                    _logX.DebugFormat("tempdb recovery model is {0}", recoveryModel);
                    if (!String.IsNullOrEmpty(recoveryModel) && !recoveryModel.ToLower().Equals("simple"))
                    {
                        AddTempDbSimpleRecoveryFinding(sm, recoveryModel);
                        count++;
                    }
                }
                else
                    _logX.Debug("Database info for tempdb not found");

                return count;
            }
        }

        /// <summary>
        /// Report #56
        /// </summary>
        private void AddTempDbInitialSizeFinding(SnapshotMetrics sm, DatabaseInfoSnapshot snapshot, long totalInitial, long totalCurrent)
        {
            using (_logX.DebugCall("AddTempDbInitialSizeFinding"))
            {
                TempDbInitialSizeRecommendation recommendation = new TempDbInitialSizeRecommendation();
                recommendation.InitialSizeMB = totalInitial / 1048576;
                recommendation.CurrentSizeMB = totalCurrent / 1048576;
                AddRecommendation(recommendation);
            }
        }

        private void PerformObscureChecks(SnapshotMetrics sm, ref int totalRecommendations)
        {
            using (_logX.DebugCall("PerformObscureChecks"))
            {
                if (totalRecommendations == 0)
                {
                    if (sm.ServerPropertiesMetrics != null)
                    {
                        double processorCount = sm.WMIPerfOSProcessorMetrics.GetProcessorCount(0);
                        double processorQueueLength = sm.WMIPerfOSSystemMetrics.AvgProcessorQueueLength;
                        if ((processorQueueLength / processorCount) < Properties.Settings.Default.IO_HighProcessorQueueLengthPerProcessor)
                        {
                            double sqlcpu = sm.WMIPerfOSProcessorMetrics.GetAvgCpu(sm.ServerPropertiesMetrics.AffinityMask);
                            _logX.DebugFormat("Processor count is {0} and queue length is {1} and sql server cpu is {2}", processorCount, processorQueueLength, sqlcpu);
                            if (sqlcpu < Properties.Settings.Default.IO_HighSqlServerCPU)
                            {
                                _logX.Debug("Page compression recommendations should be checked.");
                                _common.PageCompression = true;
                            }
                        }
                        else
                            _logX.DebugFormat("Processor count is {0} and avg processor queue length is {1}", processorCount, processorQueueLength);
                    }
                }
                else
                    _logX.DebugFormat("Skipping compression checks because IO recommendations were already generated");
            }
        }

        private void AddTempDbSimpleRecoveryFinding(SnapshotMetrics sm, string recoveryModel)
        {
            using (_logX.DebugCall("AddTempDbSimpleRecoveryFinding"))
            {
                AddRecommendation(new TempDbRecoveryModelRecommendation());
            }
        }

        private void AddTooManyTempDbFilesFinding(SnapshotMetrics sm, int processorCount, int numberFiles)
        {
            using (_logX.DebugCall("AddTooManyTempDbFilesFinding"))
            {
                TempDbTooManyFilesRecommendation recommendation = new TempDbTooManyFilesRecommendation();
                recommendation.ProcessorCount = processorCount;
                recommendation.FileCount = numberFiles;
                AddRecommendation(recommendation);
            }
        }

        private void AddTempDbOptimizationFinding(SnapshotMetrics sm, int processorCount, int numberFiles)
        {
            //We are no longer generating this recommendation because there's no perceived problem to solve here.  
            //SDR-D11 has the same recommendation but it is generated only when we do find TempDB to be a bottleneck


            //using (_logX.DebugCall("AddTempDbOptimizationFinding"))
            //{
            //    TempDbFileCountRecommendation recommendation = new TempDbFileCountRecommendation();
            //    recommendation.ProcessorCount = processorCount;
            //    recommendation.FileCount = numberFiles;
            //    AddRecommendation(recommendation);
            //}
        }

        private int GetTraceFlagStatus(System.Data.SqlClient.SqlConnection conn, int flag)
        {
            string sqlTraceCommand = "CREATE table #trace_temp(flag varchar, stat varchar, glob varchar, sess varchar) " +
                                    "INSERT #trace_temp " +
                                    "EXEC('DBCC TRACESTATUS (" + flag + ");') " +
                                    "SELECT stat FROM #trace_temp " +
                                    "DROP table #trace_temp";

            object obj = Common.Helpers.SQLHelper.GetScalarResult(sqlTraceCommand, conn);

            if (obj != null)
            {
                string status = obj.ToString();
                return int.Parse(status);
            }

            //Treating this case as trace flag 1118 is OFF
            return 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="dataFiles">precondition: The array has at least one element</param>
        /// <returns></returns>
        private bool IsAutogrowthEnabled(System.Data.SqlClient.SqlConnection conn, DatabaseFileInfo[] dataFiles)
        {
            //bool isSameGrowth = dataFiles.All(db => (db.IsPercentGrowth == dataFiles.First().IsPercentGrowth) &&
            //                                        (db.Growth == dataFiles.First().Growth));

            bool isTraceFlagOff = (GetTraceFlagStatus(conn, 1117) == 0);

            //bool isAutoGrowEnabled = (dataFiles.Any(db => db.Growth != 0));
            bool isSameGrowth = true;
            bool isAutoGrowEnabled =false;

            
            if (dataFiles.Length > 0)
            {
                var first = dataFiles[0];
                foreach (DatabaseFileInfo db in dataFiles)
                {
                    if ((db.IsPercentGrowth != first.IsPercentGrowth) || (db.Growth != first.Growth))
                    {
                        isSameGrowth = false;
                        break;
                    }
                }
                foreach (DatabaseFileInfo db in dataFiles)
                {
                    if (db.Growth != 0)
                    {
                        isAutoGrowEnabled = true;
                        break;
                    }
                }
            }

            return (!isSameGrowth && isTraceFlagOff && isAutoGrowEnabled);
        }

        private void AddTempDbWaitFinding(SnapshotMetrics sm, int processorCount, int waitingSampleCount, int status)
        {
            using (_logX.DebugCall("AddTempDbWaitFinding"))
            {
                TempDbWaitingRecommendation recommendation = new TempDbWaitingRecommendation(status);
                recommendation.WaitingSampleCount = waitingSampleCount;
                AddRecommendation(recommendation);
            }
        }

        /// <summary>
        /// Report #55
        /// </summary>
        private void AddTempDbDataFileMismatchFinding(SnapshotMetrics sm, DatabaseInfoSnapshot snapshot, long smallest, long largest)
        {
            using (_logX.DebugCall("AddTempDbDataFileMismatchFinding"))
            {
                TempDbFileSizeMismatchRecommendation recommendation = new TempDbFileSizeMismatchRecommendation();
                recommendation.SmallestFileSize = smallest;
                recommendation.LargestFileSize = largest;
                AddRecommendation(recommendation);
            }
        }

        private void AddTempDbAutogrowthFinding()
        {
            using (_logX.DebugCall("AddTempDbAutogrowthFinding"))
            {
                TempDbAutogrowthRecommendation recommendation = new TempDbAutogrowthRecommendation();
                AddRecommendation(recommendation);
            }
        }

        /// <summary>
        ///  Report #51
        /// </summary>
        /// <param name="logicalDiskName"></param>
        /// <param name="sc"></param>
        /// <param name="blockSize"></param>
        private void AddBadAllocationSizeFinding(string logicalDiskName, SnapshotMetrics sm, ulong blockSize, double? avgDiskSecPerTransfer, double avgDiskQueueLength)
        {
            _logX.DebugFormat("Disk '{0}' adding recommendation for finding 51", logicalDiskName);
            DiskBlockSizeRecommendation recommendation = new DiskBlockSizeRecommendation(logicalDiskName);
            recommendation.AllocationSizeKB = blockSize / 1024;
            if (avgDiskSecPerTransfer.HasValue)
                recommendation.AvgDiskSecPerTransfer = avgDiskSecPerTransfer.Value;
            //recommendation.AvgDiskQueueLength = Math.Round(avgDiskQueueLength, 2);
            AddRecommendation(recommendation);
        }

        /// <summary>
        /// Report 48a
        /// </summary>
        /// <param name="sc"></param>
        private void AddIoSbsStressedFinding(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AddIoSbsStressedFinding"))
            {
                AddRecommendation(new DiskWaitingRecommendation());
            }
        }

        private double IOWaitPercentage(SnapshotMetrics sm)
        {
            SampledServerResources snapshot = sm.SampledServerResourcesMetrics.FirstSnapshot;
            if (snapshot != null)
            {
                double iowaits = snapshot.IoWaits;
                double totalwaits = snapshot.TotalWaits;
                double result = iowaits / totalwaits;
                _logX.InfoFormat("IO Wait Percentage = {0}/{1} = {2}", iowaits, totalwaits, result);
                return result;
            }
            _logX.Info("Sampled Server Resources not found.  Returning 0.");
            return 0;
        }

        private bool HasSeManageVolumeName(SnapshotMetrics sm)
        {
            string state = sm.ServerPropertiesMetrics.SeManageVolumePrivilege;
            if (String.IsNullOrEmpty(state))
            {
                // if we have no sql service account then we were not able to run the whoami command
                if (String.IsNullOrEmpty(sm.ServerPropertiesMetrics.SQLServerServiceAccount))
                {
                    _logX.Debug("Manage volume permission and sql server service account not set.  Assume permission set.");
                    return true;
                }
                // if we have an account and no state then assume the account does not have the permission
                _logX.Debug("Manage volume permission net set but service account has a value.  Assume no permission.");
                return false;
            }
            else
                _logX.Debug("Manage volume name permission: " + state);

            return state.Equals("enabled", StringComparison.InvariantCultureIgnoreCase);
        }

        private void AddNeedManageVolumnNamePermissionFinding(SnapshotMetrics sm)
        {
            SeManageVolumeNameRecommendation recommendation = new SeManageVolumeNameRecommendation();
            string serviceAccount = sm.ServerPropertiesMetrics.SQLServerServiceAccount;
            if (!String.IsNullOrEmpty(serviceAccount))
                recommendation.SQLServerServiceAccount = serviceAccount;
            AddRecommendation(recommendation);
        }

        /// <summary>
        /// Report #48
        /// </summary>
        private void AddDiskStrugglingFinding1(string logicalDiskName, SnapshotMetrics sm, double avgDiskQueueLength, double? avgDiskSecPerTransfer, double avgPagesPerSecond)
        {
            _logX.DebugFormat("Disk '{0}' adding recommendation for finding 48", logicalDiskName);
            DiskQueueLengthRecommendation1 recommendation = new DiskQueueLengthRecommendation1(logicalDiskName);
            recommendation.AvgDiskQueueLength = Math.Round(avgDiskQueueLength, 2); ;
            recommendation.AvgPagesPerSecond = avgPagesPerSecond;
            if (avgDiskSecPerTransfer.HasValue)
                recommendation.AvgDiskSecPerTransfer = avgDiskSecPerTransfer.Value;
            AddRecommendation(recommendation);
        }

        /// <summary>
        /// Report #49
        /// </summary>
        private void AddDiskStrugglingFinding2(string logicalDiskName, SnapshotMetrics sm, double avgDiskQueueLength, double? avgDiskSecPerTransfer)
        {
            _logX.DebugFormat("Disk '{0}' adding recommendation for finding 49", logicalDiskName);
            DiskQueueLengthRecommendation2 recommendation = new DiskQueueLengthRecommendation2(logicalDiskName);
            recommendation.AvgDiskQueueLength = Math.Round(avgDiskQueueLength, 2);
            if (avgDiskSecPerTransfer.HasValue)
                recommendation.AvgDiskSecPerTransfer = avgDiskSecPerTransfer.Value;
            AddRecommendation(recommendation);
        }

        /// <summary>
        /// Report #50
        /// </summary>
        private void AddDiskStrugglingFinding3(string logicalDiskName, SnapshotMetrics sm, double avgDiskQueueLength, double? avgDiskSecPerTransfer, double avgDiskIOSplitRatio)
        {
            _logX.DebugFormat("Disk '{0}' adding recommendation for finding 50", logicalDiskName);
            DiskQueueLengthRecommendation3 recommendation = new DiskQueueLengthRecommendation3(logicalDiskName);
            //recommendation.AvgDiskQueueLength = Math.Round(avgDiskQueueLength, 2);
            if (avgDiskSecPerTransfer.HasValue)
                recommendation.AvgDiskSecPerTransfer = avgDiskSecPerTransfer.Value;
            recommendation.AvgDiskIOSplitRatio = avgDiskIOSplitRatio;
            AddRecommendation(recommendation);
        }

        /// <summary>
        ///  Report #52
        /// </summary>
        /// <param name="logicalDiskName"></param>
        /// <param name="sc"></param>
        private void AddDiskStrugglingFinding4(string logicalDiskName, SnapshotMetrics sm, double avgDiskQueueLength, double? avgDiskSecPerTransfer)
        {
            _logX.DebugFormat("Disk '{0}' adding recommendation for finding 52", logicalDiskName);
            DiskQueueLengthRecommendation5 recommendation = new DiskQueueLengthRecommendation5(logicalDiskName);
            recommendation.AvgDiskQueueLength = Math.Round(avgDiskQueueLength, 2);
            if (avgDiskSecPerTransfer.HasValue)
                recommendation.AvgDiskSecPerTransfer = avgDiskSecPerTransfer.Value;
            AddRecommendation(recommendation);
        }

        /// <summary>
        ///  Report #53
        /// </summary>
        private void AddStrugglingDiskFinding5(string logicalDiskName, SnapshotMetrics sm, double avgDiskQueueLength, double? avgDiskSecPerTransfer)
        {
            _logX.DebugFormat("Disk '{0}' adding recommendation for finding 53", logicalDiskName);
            DiskQueueLengthRecommendation6 recommendation = new DiskQueueLengthRecommendation6(logicalDiskName);
            recommendation.AvgDiskQueueLength = Math.Round(avgDiskQueueLength, 2);
            if (avgDiskSecPerTransfer.HasValue)
                recommendation.AvgDiskSecPerTransfer = avgDiskSecPerTransfer.Value;
            AddRecommendation(recommendation);
        }

        private bool HasPageFile(string logicalDiskName, SnapshotMetrics sm)
        {
            //return sc.WMIPageFileCollector.GetPageFileCount(name =>
            //         name.StartsWith(logicalDiskName, StringComparison.InvariantCultureIgnoreCase)) > 0;
            return sm.WMIPageFileMetrics.GetPageFileCount(logicalDiskName) > 0;
        }

        private ulong GetAllocationSize(string logicalDisk, SnapshotMetrics sm)
        {
            using (_logX.DebugCall("GetAllocationSize"))
            {
                WMIVolume volume = sm.WMIVolumeMetrics.GetVolume(logicalDisk);
                if (volume != null)
                    return volume.BlockSize;
                else
                    _logX.Debug("WMIVolume information not found for " + logicalDisk);
                return 0;
            }
        }
    }
}
