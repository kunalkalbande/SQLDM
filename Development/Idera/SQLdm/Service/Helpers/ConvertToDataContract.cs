using DC = Idera.SQLdm.Service.DataContracts.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using O = Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Snapshots;
using System.Globalization;
using Idera.SQLdm.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;


namespace Idera.SQLdm.Service.Helpers
{
    public static class ConvertToDataContract
    {
		const String UNKNOWN_SESSION_APPLICATION_NAME = "Unknowing";
        public static IEnumerable<DC.MonitoredSqlServer> ToDC(IEnumerable<O.MonitoredSqlServer> list)
        {
            IList<DC.MonitoredSqlServer> result = new List<DC.MonitoredSqlServer>();
            foreach (var obj in list)
            {
                result.Add(ConvertToDataContract.ToDC(obj));
            }

            return result;
        }

        internal static DC.MonitoredSqlServer ToDC(O.MonitoredSqlServer obj)
        {
            return new DC.MonitoredSqlServer
            {
                FriendlyServerName=obj.FriendlyServerName,//SQLdm 10.1 - (Pulkit Puri) 
                SQLServerId = obj.Id,
                InstanceName = obj.InstanceName,
                IsActive = obj.IsActive,
                IsSQLSafeConnected = obj.IsSQLsafeConnected.ToString(),
                IsVirtualized = obj.IsVirtualized.ToString(),
                maintenanceModeEnabled = obj.MaintenanceModeEnabled.ToString(),
                InstanceEdition = obj.MostRecentSQLEdition,
                MostRecentSQLVersion = obj.MostRecentSQLVersion != null ? obj.MostRecentSQLVersion.DisplayVersion : string.Empty,
                RegisteredDate = obj.RegisteredDate.ToString()
            };
        }


        public static DC.Metric ToDC(Idera.SQLdm.Common.Events.MetricDefinition source, ref IDictionary<int, Type> nonNumericMetrics)
        {
            DC.Metric result = new DC.Metric()
            {
                MetricCategory = source.MetricCategory.ToString("F"),
                DefaultInfoValue = source.DefaultInfoThresholdValue.ToString("F"),
                MetricId = source.MetricID,
                Name = source.Name,
                Description = source.Description,
                DefaultCriticalThreshold = source.DefaultCriticalThresholdValue,
                DefaultWarningThreshold = source.DefaultWarningThresholdValue,
                MaxValue = source.MaxValue,
                MinValue = source.MinValue
            };


            return result;
        }

        internal static DC.ServerOverview ToDCAbridged(SqlDataReader dataReader)
        {
            DC.ServerOverview result = new DC.ServerOverview();

            var value = dataReader["AgentServiceStatus"];
            if (value != DBNull.Value)
            {
                result.agentServiceStatus = Enum.GetName(ServiceState.NotInstalled.GetType(), (int)value);
            }

            value = dataReader["DTCServiceStatus"];
            if (value != DBNull.Value)
            {
                result.dtcServiceStatus = Enum.GetName(ServiceState.NotInstalled.GetType(), (int)value);
            }

            value = dataReader["SqlServerServiceStatus"];
            if (value != DBNull.Value)
            {
                result.sqlServiceStatus = Enum.GetName(ServiceState.NotInstalled.GetType(), (int)value);
            }

            //START : SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --read SQLBrowser status
            int SQLBrowserServiceStatusOrdinal = dataReader.GetOrdinal("SQLBrowserServiceStatus");
            if (!dataReader.IsDBNull(SQLBrowserServiceStatusOrdinal)) 
            {
                result.sqlBrowserServiceStatus = Enum.GetName(ServiceState.NotInstalled.GetType(), (int) dataReader[SQLBrowserServiceStatusOrdinal]);
            }

            int SQLActiveDirectoryHelperServiceStatusOrdinal = dataReader.GetOrdinal("SQLActiveDirectoryHelperServiceStatus");
            if (!dataReader.IsDBNull(SQLActiveDirectoryHelperServiceStatusOrdinal))
            {
                result.sqlActiveDirectoryHelperServiceStatus = Enum.GetName(ServiceState.NotInstalled.GetType(), (int)dataReader[SQLActiveDirectoryHelperServiceStatusOrdinal]);
            }
            //END : SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --read SQLBrowser status

            value = dataReader["InstanceName"];
            if (value != DBNull.Value)
            {
                result.InstanceName = (string)value;
            }
            
            value = dataReader["SqlServerEdition"];
            if (value != DBNull.Value)
            {
                result.sqlServerEdition = (string)value;
            }

            value = dataReader["ResponseTimeInMilliseconds"];
            if (value != DBNull.Value)
            {
                result.responseTime = (int)value;
            }

            value = dataReader["ServerVersion"];
            if (value != DBNull.Value)
            {
                result.ProductVersion = (string)value;
            }
            else 
            {
                result.ProductVersion = "N/A";
            }

            result.systemProcesses = new DC.ServerSystemProcesses();
            value = dataReader["BlockedProcesses"];
            if (value != DBNull.Value)
            {
                result.systemProcesses.blockedProcesses = (int)value;
            }
            result.systemProcesses.currentUserProcesses = dataReader["UserProcesses"] != DBNull.Value ? (int)dataReader["UserProcesses"] : 0;

            result.targetServerMemory = dataReader["SqlMemoryAllocatedInKilobytes"] != DBNull.Value ? (long)dataReader["SqlMemoryAllocatedInKilobytes"] : 0;
            result.totalServerMemory = dataReader["SqlMemoryUsedInKilobytes"] != DBNull.Value ? (long)dataReader["SqlMemoryUsedInKilobytes"] : 0;            

            result.statistics = new DC.ServerStatistics();
            result.statistics.cpuPercentage = dataReader["CPUActivityPercentage"] != DBNull.Value ? (double)dataReader["CPUActivityPercentage"] : 0;
            result.statistics.totalConnections = dataReader["Logins"] != DBNull.Value ? (long)dataReader["Logins"] : 0;
            //result.statistics.diskRead = dataReader["DiskRead"] != DBNull.Value ? (long)dataReader["DiskRead"] : 0;;
            //result.statistics.diskWrite = dataReader["DiskWrite"] != DBNull.Value ? (long)dataReader["DiskWrite"] : 0;;
            result.statistics.ioTimeDelta = dataReader["IOTimeDelta"] != DBNull.Value ? (long)dataReader["IOTimeDelta"] : 0;
            result.statistics.SqlMemoryUsed = dataReader["SqlMemoryUsedInKilobytes"] != DBNull.Value ? (long)dataReader["SqlMemoryUsedInKilobytes"] : 0;;

            result.statistics.pageReads = dataReader["PageReads"] != DBNull.Value ? (long)dataReader["PageReads"] : 0;
            result.statistics.pageWrites = dataReader["PageWrites"] != DBNull.Value ? (long)dataReader["PageWrites"] : 0;
            
            result.osMetricsStatistics = new DC.OSMetrics();
            result.osMetricsStatistics.AvailableBytes = dataReader["OSAvailableMemoryInKilobytes"] != DBNull.Value ? (long)dataReader["OSAvailableMemoryInKilobytes"] : 0;
            result.osMetricsStatistics.ProcessorQueueLength = dataReader["ProcessorQueueLength"] != DBNull.Value ? (double)dataReader["ProcessorQueueLength"] : 0;

            var ordinal = dataReader.GetOrdinal("MaintenanceModeEnabled");
            if (!dataReader.IsDBNull(ordinal)) 
            {
                result.maintenanceModeEnabled = dataReader.GetBoolean(ordinal);
            }

            return result;
        }


        internal static DC.ServerOverview ToDC(Common.Snapshots.ServerOverview source, string timeZoneOffset)
        {

            DC.ServerOverview result = new DC.ServerOverview()
            {
                agentServiceStatus = source.AgentServiceStatus.HasValue ? Enum.GetName(source.AgentServiceStatus.Value.GetType(), source.AgentServiceStatus.Value) : Enum.GetName(ServiceState.UnableToMonitor.GetType(), ServiceState.UnableToMonitor),
                dtcServiceStatus = source.DtcServiceStatus.HasValue ? Enum.GetName(source.DtcServiceStatus.Value.GetType(), source.DtcServiceStatus.Value) : Enum.GetName(ServiceState.UnableToMonitor.GetType(), ServiceState.UnableToMonitor),
                fullTextServiceStatus = source.FullTextServiceStatus.HasValue ? Enum.GetName(source.FullTextServiceStatus.Value.GetType(), source.FullTextServiceStatus.Value) : Enum.GetName(ServiceState.UnableToMonitor.GetType(), ServiceState.UnableToMonitor),
                sqlServiceStatus = source.SqlServiceStatus.HasValue ? Enum.GetName(source.SqlServiceStatus.Value.GetType(), source.SqlServiceStatus.Value) : Enum.GetName(ServiceState.UnableToMonitor.GetType(), ServiceState.UnableToMonitor),
                language = source.Language,
                loginHasAdministratorRights = source.LoginHasAdministratorRights.HasValue ? source.LoginHasAdministratorRights.Value : false,
                maxConnections = source.MaxConnections.HasValue ? source.MaxConnections.Value : 0,
                physicalMemory = source.PhysicalMemory != null && source.PhysicalMemory.Kilobytes.HasValue ? source.PhysicalMemory.Kilobytes.Value : 0,
                processorCount = source.ProcessorCount.HasValue ? source.ProcessorCount.Value : 0,
                processorsUsed = source.ProcessorsUsed.HasValue ? source.ProcessorsUsed.Value : 0,
                processorType = source.ProcessorType,
                realServerName = source.RealServerName,
                InstanceName = source.ServerName,
                serverHostName = source.ServerHostName,
                sqlServerEdition = source.SqlServerEdition,
                windowsVersion = source.WindowsVersion,
                targetServerMemory = source.TargetServerMemory != null && source.TargetServerMemory.Kilobytes.HasValue ? source.TargetServerMemory.Kilobytes.Value : 0, 
                totalServerMemory = source.TotalServerMemory != null && source.TotalServerMemory.Kilobytes.HasValue ? source.TotalServerMemory.Kilobytes.Value : 0,
                procedureCacheSize = source.ProcedureCacheSize != null && source.ProcedureCacheSize.Kilobytes.HasValue ? source.ProcedureCacheSize.Kilobytes.Value : 0,
                procedureCachePercentageUsed = source.ProcedureCachePercentageUsed.HasValue ? source.ProcedureCachePercentageUsed.Value : 0,
                responseTime = source.ResponseTime,

                //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                runningSince = source.RunningSince.HasValue ? DateTimeHelper.ConvertToLocal((DateTime.SpecifyKind((source.RunningSince.Value), DateTimeKind.Utc)), timeZoneOffset) : Convert.ToDateTime("1-1-1900"),
                
                ProductVersion = source.ProductVersion != null ? source.ProductVersion.Version : "N/A",
                ProductEdition = source.ProductEdition,
                isClustered = source.IsClustered.HasValue ? source.IsClustered.Value : false,
                clusterNodeName = source.ClusterNodeName,
                maintenanceModeEnabled = source.MaintenanceModeEnabled  //SQLdm 9.0 (Abhishek Joshi) --Web Console Improvements -set maintenance mode status when each instance is viewed separately
            };


            if (source.SystemProcesses != null)
            {
                result.systemProcesses = new DC.ServerSystemProcesses()
                {
                    activeProcesses = source.SystemProcesses.ActiveProcesses.HasValue ? source.SystemProcesses.ActiveProcesses.Value : 0,
                    blockedProcesses = source.SystemProcesses.BlockedProcesses.HasValue ? source.SystemProcesses.BlockedProcesses.Value : 0,
                    computersHoldingProcesses = source.SystemProcesses.ComputersHoldingProcesses.HasValue ? source.SystemProcesses.ComputersHoldingProcesses.Value : 0,
                    currentSystemProcesses = source.SystemProcesses.CurrentSystemProcesses.HasValue ? source.SystemProcesses.CurrentSystemProcesses.Value : 0,
                    currentUserProcesses = source.SystemProcesses.CurrentUserProcesses.HasValue ? source.SystemProcesses.CurrentUserProcesses.Value : 0,
                    leadBlockers = source.SystemProcesses.LeadBlockers.HasValue ? source.SystemProcesses.LeadBlockers.Value : 0,
                    openTransactions = source.SystemProcesses.OpenTransactions.HasValue ? source.SystemProcesses.OpenTransactions.Value : 0,
                    systemProcessesConsumingCpu = source.SystemProcesses.SystemProcessesConsumingCpu.HasValue ? source.SystemProcesses.SystemProcessesConsumingCpu.Value : 0,
                    userProcessesConsumingCpu = source.SystemProcesses.UserProcessesConsumingCpu.HasValue ? source.SystemProcesses.UserProcessesConsumingCpu.Value : 0
                };
            }

            if (source.DatabaseSummary != null)
            {
                result.databaseSummary = new DC.ServerDatabaseSummary()
                {
                    databaseCount = source.DatabaseSummary.DatabaseCount.HasValue ? source.DatabaseSummary.DatabaseCount.Value : 0,
                    dataFileCount = source.DatabaseSummary.DataFileCount.HasValue ? source.DatabaseSummary.DataFileCount.Value : 0,
                    dataFileSpaceAllocated = source.DatabaseSummary.DataFileSpaceAllocated != null && source.DatabaseSummary.DataFileSpaceAllocated.Kilobytes.HasValue ? source.DatabaseSummary.DataFileSpaceAllocated.Kilobytes.Value : 0,
                    dataFileSpaceUsed = source.DatabaseSummary.DataFileSpaceUsed != null && source.DatabaseSummary.DataFileSpaceUsed.Kilobytes.HasValue ? source.DatabaseSummary.DataFileSpaceUsed.Kilobytes.Value : 0,
                    logFileCount = source.DatabaseSummary.LogFileCount.HasValue ? source.DatabaseSummary.LogFileCount.Value : 0,
                    logFileSpaceAllocated = source.DatabaseSummary.LogFileSpaceAllocated != null && source.DatabaseSummary.LogFileSpaceAllocated.Kilobytes.HasValue ? source.DatabaseSummary.LogFileSpaceAllocated.Kilobytes.Value : 0,
                    logFileSpaceUsed = source.DatabaseSummary.LogFileSpaceUsed != null && source.DatabaseSummary.LogFileSpaceUsed.Kilobytes.HasValue ? source.DatabaseSummary.LogFileSpaceUsed.Kilobytes.Value : 0               
                };
            }

            if (source.Statistics != null)
            {
                result.statistics = new DC.ServerStatistics()
                {
                    checkpointPages = source.Statistics.CheckpointPages.HasValue ? source.Statistics.CheckpointPages.Value : 0,
                    idlePercentage = source.Statistics.IdlePercentage.HasValue ? source.Statistics.IdlePercentage.Value : 0,
                    ioPercentage = source.Statistics.IoPercentage.HasValue ? source.Statistics.IoPercentage.Value : 0,
                    lazyWrites = source.Statistics.LazyWrites.HasValue ? source.Statistics.LazyWrites.Value : 0,
                    logFlushes = source.Statistics.LogFlushes.HasValue ? source.Statistics.LogFlushes.Value : 0,
                    lockWaits = source.Statistics.LockWaits.HasValue ? source.Statistics.LockWaits.Value : 0,
                    fullScans = source.Statistics.FullScans.HasValue ? source.Statistics.FullScans.Value : 0,
                    cpuPercentage = source.Statistics.CpuPercentage.HasValue ? source.Statistics.CpuPercentage.Value : 0,
                    BufferCacheHitRatio = source.Statistics.BufferCacheHitRatio.HasValue ? source.Statistics.BufferCacheHitRatio.Value : 0,
                    totalConnections = source.Statistics.TotalConnections.HasValue ? source.Statistics.TotalConnections.Value : 0,
                    packetErrors = source.Statistics.PacketErrors.HasValue ? source.Statistics.PacketErrors.Value : 0,
                    packetsReceived = source.Statistics.PacketsReceived.HasValue ? source.Statistics.PacketsReceived.Value : 0,
                    packetsSent = source.Statistics.PacketsSent.HasValue ? source.Statistics.PacketsSent.Value : 0,
                    pageLifeExpectancy = source.Statistics.PageLifeExpectancy.HasValue ? source.Statistics.PageLifeExpectancy.Value.Ticks : 0,
                    pageLookups = source.Statistics.PageLookups.HasValue ? source.Statistics.PageLookups.Value : 0,
                    pageReads = source.Statistics.PageReads.HasValue ? source.Statistics.PageReads.Value : 0,
                    pageSplits = source.Statistics.PageSplits.HasValue ? source.Statistics.PageSplits.Value : 0,
                    pageWrites = source.Statistics.PageWrites.HasValue ? source.Statistics.PageWrites.Value : 0,
                    CacheHitRatio = source.Statistics.CacheHitRatio.HasValue ? source.Statistics.CacheHitRatio.Value : 0,
                    readaheadPages = source.Statistics.ReadaheadPages.HasValue ? source.Statistics.ReadaheadPages.Value : 0,
                    sqlCompilations = source.Statistics.SqlCompilations.HasValue ? source.Statistics.SqlCompilations.Value : 0,
                    sqlRecompilations = source.Statistics.SqlRecompilations.HasValue ? source.Statistics.SqlRecompilations.Value : 0,
                    tableLockEscalations = source.Statistics.TableLockEscalations.HasValue ? source.Statistics.TableLockEscalations.Value : 0,
                    workfilesCreated = source.Statistics.WorkfilesCreated.HasValue ? source.Statistics.WorkfilesCreated.Value : 0,
                    diskRead = source.Statistics.DiskRead.HasValue ? source.Statistics.DiskRead.Value : 0,
                    diskErrors = source.Statistics.DiskErrors.HasValue ? source.Statistics.DiskErrors.Value : 0,
                    diskWrite = source.Statistics.DiskWrite.HasValue ? source.Statistics.DiskWrite.Value : 0,
                    cpuBusyDelta = source.Statistics.CpuBusyDelta != null && source.Statistics.CpuBusyDelta.Ticks.HasValue ? source.Statistics.CpuBusyDelta.CpuTimeSpan.Ticks : 0, 
                    idleTimeDelta = source.Statistics.IdleTimeDelta != null && source.Statistics.IdleTimeDelta.Ticks.HasValue ? source.Statistics.IdleTimeDelta.CpuTimeSpan.Ticks : 0,
                    ioTimeDelta = source.Statistics.IoTimeDelta != null && source.Statistics.IoTimeDelta.Ticks.HasValue ? source.Statistics.IoTimeDelta.CpuTimeSpan.Ticks : 0,
                    timeTicks = source.Statistics.TimeTicks.HasValue ? source.Statistics.TimeTicks.Value : 0,
                    batchRequests = source.Statistics.BatchRequests.HasValue ? source.Statistics.BatchRequests.Value : 0,
                    WorktablesCreated = source.Statistics.WorktablesCreated.HasValue ? source.Statistics.WorktablesCreated.Value : 0,
                    ReplicationLatencyInSeconds = source.Statistics.ReplicationLatencyInSeconds.HasValue ? source.Statistics.ReplicationLatencyInSeconds.Value : 0,
                    ReplicationSubscribed = source.Statistics.ReplicationSubscribed.HasValue ? source.Statistics.ReplicationSubscribed.Value : 0,
                    ReplicationUndistributed = source.Statistics.ReplicationUndistributed.HasValue ? source.Statistics.ReplicationUndistributed.Value : 0,
                    ReplicationUnsubscribed = source.Statistics.ReplicationUnsubscribed.HasValue ? source.Statistics.ReplicationUnsubscribed.Value : 0,
                    TempDBSize = source.Statistics.TempDBSize != null && source.Statistics.TempDBSize.Kilobytes.HasValue ? source.Statistics.TempDBSize.Kilobytes.Value : 0,
                    TempDBSizePercent = source.Statistics.TempDBSizePercent.HasValue ? source.Statistics.TempDBSizePercent.Value : 0,
                    OldestOpenTransactionsInMinutes = source.Statistics.OldestOpenTransactionsInMinutes.HasValue ? source.Statistics.OldestOpenTransactionsInMinutes.Value : 0,
                    PageLifeExpectancySeconds = source.Statistics.PageLifeExpectancySeconds.HasValue ? source.Statistics.PageLifeExpectancySeconds.Value : 0,
                    SqlMemoryUsed = source.TotalServerMemory != null && source.TotalServerMemory.Kilobytes.HasValue ? source.TotalServerMemory.Kilobytes.Value : 0

                };
            }

            if (source.OSMetricsStatistics != null)
            {
                result.osMetricsStatistics = new DC.OSMetrics()
                {
                    PercentDiskIdleTime = source.OSMetricsStatistics.PercentDiskIdleTime.HasValue ? source.OSMetricsStatistics.PercentDiskIdleTime.Value : 0,
                    PercentDiskTime = source.OSMetricsStatistics.PercentDiskTime.HasValue ? source.OSMetricsStatistics.PercentDiskTime.Value : 0,
                    AvgDiskQueueLength = source.OSMetricsStatistics.AvgDiskQueueLength.HasValue ? source.OSMetricsStatistics.AvgDiskQueueLength.Value : 0,
                    AvailableBytes = source.OSMetricsStatistics.AvailableBytes != null && source.OSMetricsStatistics.AvailableBytes.Kilobytes.HasValue ? source.OSMetricsStatistics.AvailableBytes.Kilobytes.Value : 0,
                    PercentPrivilegedTime = source.OSMetricsStatistics.PercentPrivilegedTime != null && source.OSMetricsStatistics.PercentPrivilegedTime.HasValue ? source.OSMetricsStatistics.PercentPrivilegedTime.Value : 0,
                    ProcessorQueueLength = source.OSMetricsStatistics.ProcessorQueueLength.HasValue ? source.OSMetricsStatistics.ProcessorQueueLength.Value : 0,
                    PercentProcessorTime = source.OSMetricsStatistics.PercentProcessorTime.HasValue ? source.OSMetricsStatistics.PercentProcessorTime.Value : 0,
                    PercentUserTime = source.OSMetricsStatistics.PercentUserTime.HasValue ? source.OSMetricsStatistics.PercentUserTime.Value : 0,
                    TotalPhysicalMemory = source.OSMetricsStatistics.TotalPhysicalMemory != null && source.OSMetricsStatistics.TotalPhysicalMemory.Kilobytes.HasValue ? source.OSMetricsStatistics.TotalPhysicalMemory.Kilobytes.Value : 0,
                    PagesPersec = source.OSMetricsStatistics.PagesPersec.HasValue ? source.OSMetricsStatistics.PagesPersec.Value : 0
                };
            }


            if (source.TempdbStatistics != null)
            {
                result.TempDBSummary = new DC.TempdbSummaryStatistics()
                {
                    internalObjectsMegabytes = result.TempDBSummary.internalObjectsMegabytes,
                    mixedExtentsMegabytes = result.TempDBSummary.mixedExtentsMegabytes,
                    unallocatedSpaceMegabytes = result.TempDBSummary.unallocatedSpaceMegabytes,
                    userObjectsMegabytes = result.TempDBSummary.userObjectsMegabytes,
                    versionStoreMegabytes = result.TempDBSummary.versionStoreMegabytes
                };

                //// Total filesize is sum of all individual file sizes. Use source.DbStatistics["tempdb"].UsedSize instead
                //Int64 internalObjectsMegabytes, mixedExtentsMegabytes, unallocatedSpaceMegabytes, userObjectsMegabytes, versionStoreMegabytes;
                //if (Int64.TryParse(result.TempDBSummary.internalObjectsMegabytes, out internalObjectsMegabytes) &&
                //    Int64.TryParse(result.TempDBSummary.mixedExtentsMegabytes, out mixedExtentsMegabytes) &&
                //    Int64.TryParse(result.TempDBSummary.unallocatedSpaceMegabytes, out unallocatedSpaceMegabytes) &&
                //    Int64.TryParse(result.TempDBSummary.userObjectsMegabytes, out userObjectsMegabytes) &&
                //    Int64.TryParse(result.TempDBSummary.versionStoreMegabytes, out versionStoreMegabytes))
                //{
                //    result.TempDBSummary.filesize = (internalObjectsMegabytes + mixedExtentsMegabytes + unallocatedSpaceMegabytes + userObjectsMegabytes + versionStoreMegabytes).ToString();
                //}
            }

            if (source.DbStatistics != null && source.DbStatistics.Count > 0 && source.DbStatistics["tempdb"] != null)
            {
                result.TempDBStatistics = new DC.DatabaseStatistics()
                {
                    UsedSize = source.DbStatistics["tempdb"].UsedSize != null && source.DbStatistics["tempdb"].UsedSize.Kilobytes.HasValue ? source.DbStatistics["tempdb"].UsedSize.Kilobytes.Value : 0,
                    UnusedSize = source.DbStatistics["tempdb"].UnusedSize != null && source.DbStatistics["tempdb"].UnusedSize.Kilobytes.HasValue ? source.DbStatistics["tempdb"].UnusedSize.Kilobytes.Value : 0,
                    PercentDataSize = source.DbStatistics["tempdb"].PercentDataSize.HasValue ? source.DbStatistics["tempdb"].PercentDataSize.Value : 0
                };
            }

            return result;
        }

        public static IEnumerable<DC.Tag> ToDC(IEnumerable<O.Tag> list)
        {
            IList<DC.Tag> result = new List<DC.Tag>();
            foreach (var obj in list)
            {
                result.Add(new DC.Tag
                {
                    Id = obj.Id,
                    Name = obj.Name,
                    Instances = obj.Instances
                });
            }

            return result;
        }

        internal static DC.ServerStatistics ToDC(ServerStatistics source)
        {
            DC.ServerStatistics result = new DC.ServerStatistics();
            if (source != null)
            {
                result = new DC.ServerStatistics()
                {
                    checkpointPages = source.CheckpointPages.HasValue ? source.CheckpointPages.Value : 0,
                    idlePercentage = source.IdlePercentage.HasValue ? source.IdlePercentage.Value : 0,
                    ioPercentage = source.IoPercentage.HasValue ? source.IoPercentage.Value : 0,
                    lazyWrites = source.LazyWrites.HasValue ? source.LazyWrites.Value : 0,
                    logFlushes = source.LogFlushes.HasValue ? source.LogFlushes.Value : 0,
                    lockWaits = source.LockWaits.HasValue ? source.LockWaits.Value : 0,
                    fullScans = source.FullScans.HasValue ? source.FullScans.Value : 0,
                    cpuPercentage = source.CpuPercentage.HasValue ? source.CpuPercentage.Value : 0,
                    BufferCacheHitRatio = source.BufferCacheHitRatio.HasValue ? source.BufferCacheHitRatio.Value : 0,
                    totalConnections = source.TotalConnections.HasValue ? source.TotalConnections.Value : 0,
                    packetErrors = source.PacketErrors.HasValue ? source.PacketErrors.Value : 0,
                    packetsReceived = source.PacketsReceived.HasValue ? source.PacketsReceived.Value : 0,
                    packetsSent = source.PacketsSent.HasValue ? source.PacketsSent.Value : 0,
                    pageLifeExpectancy = source.PageLifeExpectancy.HasValue ? source.PageLifeExpectancy.Value.Ticks : 0,
                    pageLookups = source.PageLookups.HasValue ? source.PageLookups.Value : 0,
                    pageReads = source.PageReads.HasValue ? source.PageReads.Value : 0,
                    pageSplits = source.PageSplits.HasValue ? source.PageSplits.Value : 0,
                    pageWrites = source.PageWrites.HasValue ? source.PageWrites.Value : 0,
                    CacheHitRatio = source.CacheHitRatio.HasValue ? source.CacheHitRatio.Value : 0,
                    readaheadPages = source.ReadaheadPages.HasValue ? source.ReadaheadPages.Value : 0,
                    sqlCompilations = source.SqlCompilations.HasValue ? source.SqlCompilations.Value : 0,
                    sqlRecompilations = source.SqlRecompilations.HasValue ? source.SqlRecompilations.Value : 0,
                    tableLockEscalations = source.TableLockEscalations.HasValue ? source.TableLockEscalations.Value : 0,
                    workfilesCreated = source.WorkfilesCreated.HasValue ? source.WorkfilesCreated.Value : 0,
                    diskRead = source.DiskRead.HasValue ? source.DiskRead.Value : 0,
                    diskErrors = source.DiskErrors.HasValue ? source.DiskErrors.Value : 0,
                    diskWrite = source.DiskWrite.HasValue ? source.DiskWrite.Value : 0,
                    cpuBusyDelta = source.CpuBusyDelta != null && source.CpuBusyDelta.Ticks.HasValue ? source.CpuBusyDelta.CpuTimeSpan.Ticks : 0,
                    idleTimeDelta = source.IdleTimeDelta != null && source.IdleTimeDelta.Ticks.HasValue ? source.IdleTimeDelta.CpuTimeSpan.Ticks : 0,
                    ioTimeDelta = source.IoTimeDelta != null && source.IoTimeDelta.Ticks.HasValue ? source.IoTimeDelta.CpuTimeSpan.Ticks : 0,
                    timeTicks = source.TimeTicks.HasValue ? source.TimeTicks.Value : 0,
                    batchRequests = source.BatchRequests.HasValue ? source.BatchRequests.Value : 0,
                    WorktablesCreated = source.WorktablesCreated.HasValue ? source.WorktablesCreated.Value : 0,
                    ReplicationLatencyInSeconds = source.ReplicationLatencyInSeconds.HasValue ? source.ReplicationLatencyInSeconds.Value : 0,
                    ReplicationSubscribed = source.ReplicationSubscribed.HasValue ? source.ReplicationSubscribed.Value : 0,
                    ReplicationUndistributed = source.ReplicationUndistributed.HasValue ? source.ReplicationUndistributed.Value : 0,
                    ReplicationUnsubscribed = source.ReplicationUnsubscribed.HasValue ? source.ReplicationUnsubscribed.Value : 0,
                    TempDBSize = source.TempDBSize != null && source.TempDBSize.Kilobytes.HasValue ? source.TempDBSize.Kilobytes.Value : 0,
                    TempDBSizePercent = source.TempDBSizePercent.HasValue ? source.TempDBSizePercent.Value : 0
                };
            }

            return result;
        }

        //Added on 26th June (Ankit Srivastava)
        internal static DC.Widgets.SessionCountForInstance ToDC(SessionSnapshot sessionsnapshot,DC.Widgets.SessionCountForInstance result)
        {

            if (sessionsnapshot == null || sessionsnapshot.SessionList == null || sessionsnapshot.SessionList.Count == 0)
                return result;

            result.SessionIDCount = sessionsnapshot.SessionList.Count;
            
            return result;
        }

        internal static IList<DC.Category.Sessions.SessionsForInstance> ToDC(SessionSnapshot sessionsnapshot, string timeZoneOffset)
        {
            IList<DC.Category.Sessions.SessionsForInstance> result = new List<DC.Category.Sessions.SessionsForInstance>();

            if (sessionsnapshot == null || sessionsnapshot.SessionList == null || sessionsnapshot.SessionList.Count == 0)
                return result;

            foreach (var obj in sessionsnapshot.SessionList)
            {
                result.Add(ConvertToDataContract.ToDC(obj, timeZoneOffset));
            }

            return result;
        }

        private static DC.Category.Sessions.SessionsForInstance ToDC(KeyValuePair<Wintellect.PowerCollections.Pair<int?, DateTime?>, Session> sessionobj, string timeZoneOffset)
        {
            Session obj = sessionobj.Value;
            DC.Category.Sessions.SessionsForInstance result = new DC.Category.Sessions.SessionsForInstance()
            {
                //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                UTCCollectionDateTime = sessionobj.Key.Second.HasValue ? (DateTimeHelper.ConvertToLocal(DateTime.SpecifyKind(sessionobj.Key.Second.Value, DateTimeKind.Utc), timeZoneOffset)) : Convert.ToDateTime("1-1-1900"), //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time
                
                connection = new DC.Category.Sessions.Connection
                {
					Application = String.IsNullOrEmpty(obj.Application) ? UNKNOWN_SESSION_APPLICATION_NAME : obj.Application,
                    IsUserSession = obj.IsUserProcess,
                    IsSystemSession = obj.IsSystemProcess,
                    Command = obj.Command,
                    Database = obj.Database,
                    ExecutionContext = obj.ExecutionContext.HasValue ? obj.ExecutionContext.Value : -1,
                    Host = obj.Workstation,
                    Id = sessionobj.Key.First.HasValue ? sessionobj.Key.First.Value : -1,
                    NetLibrary = obj.NetLibrary,
                    NetworkAddress = obj.WorkstationNetAddress,
                    Status = obj.Status.ToString(),
                    TransactionIsolationLevel = obj.TransactionIsolationLevel.ToString(),
                    Type = obj.WaitType,
                    User = obj.UserName
                },
                lockInformation = new DC.Category.Sessions.LockInformation
                {
                    BlockedBy = obj.BlockedBy,
                    BlockedCount = obj.BlockingCount,
                    Blocking = obj.Blocking,
                    Resource = obj.WaitResource,
                    WaitTime = Convert.ToInt64(obj.WaitTime.TotalMilliseconds),
                    WaitType = obj.WaitType
                },
                tempDBUsage = new DC.Category.Sessions.TempDBUsage
                {
                    VersionStoreElapsedSeconds = obj.VersionStoreElapsedTime.HasValue ? obj.VersionStoreElapsedTime.Value.Ticks : 0,
                    spaceUsed = new DC.Category.Sessions.TempDBUsage.SpaceUsed
                    {

                        SessionInterval = obj.SessionInternalSpaceUsed.AsString(CultureInfo.CurrentCulture),
                        SessionUser = obj.SessionUserSpaceUsed.AsString(CultureInfo.CurrentCulture),
                        TaskInterval = obj.TaskInternalSpaceUsed.AsString(CultureInfo.CurrentCulture),
                        TaskUser = obj.TaskUserSpaceUsed.AsString(CultureInfo.CurrentCulture)
                    }
                },
                usage = new DC.Category.Sessions.Usage
                {
                    Cpu = obj.Cpu != null ? Convert.ToInt64(obj.Cpu.TotalMilliseconds) : 0,
                    CpuDelta = obj.CpuDelta != null ?  Convert.ToInt64(obj.CpuDelta.TotalMilliseconds) : 0,

                    //SQLdm 10.0 (Sanjali Makkar) : Applying Offset to DateTime Fields of API Response so as to return them in local time
                    LastActivity = obj.LastActivity.HasValue ? (DateTimeHelper.ConvertToLocal(DateTime.SpecifyKind(obj.LastActivity.Value, DateTimeKind.Utc), timeZoneOffset)) : Convert.ToDateTime("1-1-1900"), //To Specify that the DateTime object returned from the Database corresponds to UTC, and then converting it to local time
                    LoginTime = obj.LoggedInSince.HasValue ? (DateTimeHelper.ConvertToLocal(DateTime.SpecifyKind(obj.LoggedInSince.Value, DateTimeKind.Utc), timeZoneOffset)) : Convert.ToDateTime("1-1-1900"),
                    
                    Memory = Convert.ToString(obj.Memory.Kilobytes),
                    OpenTransactions = obj.OpenTransactions,
                    PhysicalIO = obj.PhysicalIo
                }
            };

            return result;
        }

        internal static IList<DC.Category.Sessions.SessionResponseTimeForInstance> ToDC(IList<SessionSummary> sessionlist)
        {
            IList<DC.Category.Sessions.SessionResponseTimeForInstance> result = new List<DC.Category.Sessions.SessionResponseTimeForInstance>();

            if (sessionlist == null || sessionlist.Count == 0)
                return result;

            foreach (var obj in sessionlist)
            {
                result.Add(ConvertToDataContract.ToDC(obj));
            }

            return result;
        }

        private static DC.Category.Sessions.SessionResponseTimeForInstance ToDC(SessionSummary obj)
        {
            DC.Category.Sessions.SessionResponseTimeForInstance result = new DC.Category.Sessions.SessionResponseTimeForInstance()
            {
                timeinmils = obj.ResponseTime.HasValue ? obj.ResponseTime.Value.Ticks : 0,
            };

            return result;
        }

        internal static DC.License.LicenseDetails ToDC(Common.LicenseSummary obj)
        {
            DC.License.LicenseDetails result = new DC.License.LicenseDetails()
            {
                CheckedKeyCount = obj.CheckedKeys != null ? obj.CheckedKeys.Count : 0,
                Expiration = obj.Expiration,
                IsPermanent = obj.IsPermanent,
                IsTrial = obj.IsTrial,
                IsUnlimited = obj.IsUnlimited,
                LicensedServers = obj.LicensedServers,
                MonitoredServers = obj.MonitoredServers,
                Repository = obj.Repository,
                Status = Enum.GetName(obj.Status.GetType(), obj.Status)
            };

            return result;
        }
    }
}
