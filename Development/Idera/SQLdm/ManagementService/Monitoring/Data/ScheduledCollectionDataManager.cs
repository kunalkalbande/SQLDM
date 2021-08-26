//------------------------------------------------------------------------------
// <copyright file="ScheduledCollectionDataManager.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.IO;
using Idera.SQLdm.Common.Serialization;
using Idera.SQLdm.Common.Snapshots.AlwaysOn;
using Idera.SQLdm.ManagementService.Configuration;
using Idera.SQLdm.ManagementService.Helpers;
using Idera.SQLdm.ManagementService.Health;

namespace Idera.SQLdm.ManagementService.Monitoring.Data
{
    using System;
    using System.Data.SqlClient;
    using System.Data.SqlTypes;
    using System.Diagnostics;
    using System.Text;
    using Common;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.Status;
    using Microsoft.ApplicationBlocks.Data;
    using Idera.SQLdm.Common.Objects.Replication;
    using Vim25Api;
    using Idera.SQLdm.Common.Snapshots.Cloud;

    public class ScheduledCollectionDataManager : IDisposable
    {
        private SqlConnection connection;
        private SqlTransaction transaction;
        private static Object alwaysOnLock = new Object();

        private SqlCommand saveEventCommand;
        private SqlCommand deleteEventCommand;

        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("ScheduledCollectionDataManager");

        public ScheduledCollectionDataManager()
        {
            connection = CachedObjectRepositoryConnectionFactory.GetRepositoryConnection();
            connection.Open();
            transaction = connection.BeginTransaction();
        }

        public ScheduledCollectionDataManager(SqlConnection connection)
        {
            this.connection = connection;

            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            transaction = connection.BeginTransaction();
        }

        public MonitoredObjectStateGraph GetStateGraph(MonitoredObject monitoredObject)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SaveScheduledRefresh(ScheduledRefresh refresh)
        {
            using (LOG.DebugCall("SaveScheduledRefresh"))
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                bool rolledback = false;
                Exception crapOutException = null;
                Dictionary<string, int> DatabaseNames = new Dictionary<string, int>();
                Dictionary<long, long> cachedSqlCopy = Management.ScheduledCollection.CachedSQLStatements;
                Dictionary<long, long> cachedSigCopy = Management.ScheduledCollection.CachedSQLSignatures;
                Dictionary<long, long> cachedHostNameCopy = Management.ScheduledCollection.CachedHostNames;
                Dictionary<long, long> cachedApplicationNameCopy = Management.ScheduledCollection.CachedApplicationNames;
                Dictionary<long, long> cachedLoginNameCopy = Management.ScheduledCollection.CachedLoginNames;

                try
                {
                    if (refresh.ProductVersion != null)
                    {
                        //SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: If sql server is hosted on cloud, then dont check for Collection Failed check
                        if (refresh.ProductVersion.Major > 7 &&
                            refresh.TimeStamp != null &&
                            (refresh.MonitoredServer.CloudProviderId != null || !refresh.CollectionFailed))
                        {
                            try
                            {
                                SaveServerStatistics(refresh);
                            }
                            catch (Exception e)
                            {
                                LOG.Error("Save server statistic failed", e);
                            }

                            bool saveDatabaseStatisticsSucceded = false;
                            try
                            {
                                DatabaseNames = SaveDatabaseStatistics(refresh);
                                saveDatabaseStatisticsSucceded = true;
                            }
                            catch (Exception e)
                            {
                                if (!IsSQLKeyException(e))
                                {
                                    LOG.Error("Save database statistics failed", e);
                                }
                            }

                            try
                            {
                                SaveReplicationTopology(refresh);
                            }
                            catch (Exception e)
                            {
                                if (!IsSQLKeyException(e))
                                {
                                    LOG.Error("Save repeplication topology failed", e);
                                }
                            }

                            try
                            {
                                SaveMirrorStatistics(refresh);
                            }
                            catch (Exception e)
                            {
                                LOG.Error("Save mirror statistics failed", e);
                            }

                            try
                            {
                                SaveDiskDrives(refresh);
                            }
                            catch (Exception e)
                            {
                                LOG.Error("Save disk drives failed", e);
                            }

                            try
                            {
                                SaveAlwaysOnTopology(refresh);
                            }
                            catch (Exception ex)
                            {
                                LOG.Error("Save AlwaysOn topology failed", ex);
                            }

                            try
                            {
                                SaveAlwaysOnStatistics(refresh);
                            }
                            catch (Exception ex)
                            {
                                LOG.Error("Save AlwaysOn statistics failed.", ex);
                            }

                            try
                            {
                                SaveSqlServerConfiguration(refresh);
                            }
                            catch (Exception ex)
                            {
                                LOG.Error("Save SQL Server Configuration failed.", ex);
                            }

                            TimeSpan utcOffset = TimeSpan.FromHours(0);
                            if (refresh.TimeStamp.HasValue && refresh.TimeStampLocal.HasValue)
                                utcOffset = refresh.TimeStampLocal.Value - refresh.TimeStamp.Value;
                            if (saveDatabaseStatisticsSucceded)
                            {
                                try
                                {
                                    SaveDeadlocks(refresh, DatabaseNames, ref cachedSqlCopy, ref cachedSigCopy, ref cachedHostNameCopy, ref cachedApplicationNameCopy, ref cachedLoginNameCopy, utcOffset);
                                }
                                catch (Exception e)
                                {
                                    LOG.Error("Save dead locks failed", e);
                                }

                                try
                                {
                                    SaveQueryMonitorStatements(refresh, DatabaseNames, ref cachedSqlCopy, ref cachedSigCopy, ref cachedHostNameCopy, ref cachedApplicationNameCopy, ref cachedLoginNameCopy, utcOffset);
                                }
                                catch (Exception e)
                                {
                                    LOG.Error("Save query monitor statements failed", e);
                                }
                            }
                            else
                            {
                                LOG.Info("Cannot save dead locks and query monitor statements, because save database statistics failed");
                            }

                            try
                            {
                                SaveOSStatistics(refresh);
                            }
                            catch (Exception e)
                            {
                                LOG.Error("Save OS statistics failed", e);
                            }

                            if (refresh.MonitoredServer.IsVirtualized)
                            {
                                if (refresh.Server.VMConfig != null)
                                {
                                    try
                                    {
                                        SaveVMStatistics(refresh);
                                    }
                                    catch (Exception e)
                                    {
                                        if (!IsSQLKeyException(e))
                                        {
                                            LOG.ErrorFormat("Save VM Statistics failed for [{0}]", refresh.ServerName, e);
                                        }
                                    }
                                }
                                else
                                {
                                    LOG.InfoFormat("[{0}] Refresh did not contain VM data.", refresh.ServerName);
                                }
                            }

                            try
                            {
                                SaveSessionsAndLocks(refresh);
                            }
                            catch (Exception e)
                            {
                                if (!IsSQLKeyException(e))
                                {
                                    LOG.Error("Save session and locks failed", e);
                                }
                            }

                            try
                            {
                                SaveWaitStatistics(refresh);
                            }
                            catch (Exception e)
                            {
                                LOG.Error("Save wait statistics failed", e);
                            }

                            if (saveDatabaseStatisticsSucceded)
                            {
                                try
                                {
                                    SaveActiveWaits(refresh, DatabaseNames, ref cachedSqlCopy, ref cachedSigCopy, ref cachedHostNameCopy, ref cachedApplicationNameCopy, ref cachedLoginNameCopy, utcOffset);
                                }
                                catch (Exception e)
                                {
                                    LOG.Error("Save active waits failed", e);
                                }

                                try
                                {
                                    SaveBlocking(refresh, DatabaseNames, ref cachedSqlCopy, ref cachedSigCopy, ref cachedHostNameCopy, ref cachedApplicationNameCopy, ref cachedLoginNameCopy, utcOffset);
                                }
                                catch (Exception e)
                                {
                                    LOG.Error("Save blocking failed", e);
                                }
                            }
                            else
                            {
                                LOG.Info("Cannot save active waits and blocking, because save database statistics was failed");
                            }

                            try
                            {
                                SaveCustomCounters(refresh);
                            }
                            catch (Exception e)
                            {
                                if (!IsSQLKeyException(e))
                                {
                                    LOG.Error("Save custom counters failed", e);
                                }
                            }
                        }
                        else
                        {
                            UpdateServerVersion(refresh);
                            UpdateLastRefreshTime(refresh);
                        }
                    }
                    else
                        UpdateLastRefreshTime(refresh);

                    Management.ScheduledCollection.AddCachedSqlStatement(cachedSqlCopy);
                    Management.ScheduledCollection.AddCachedApplicationName(cachedApplicationNameCopy);
                    Management.ScheduledCollection.AddCachedHostName(cachedHostNameCopy);
                    Management.ScheduledCollection.AddCachedLoginName(cachedLoginNameCopy);

                    // always process the events last 
                    try
                    {
                        ProcessEvents(refresh);
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Process events failed", e);
                    }
                    Commit();

                    try
                    {
                        //Mirroring configuration sessions for deleted mirrors must be
                        //after the commit which is accessing the tables to add data
                        GroomMirroringPreferredConfig(refresh);
                        GroomMirroringParticipants(refresh);
                        GroomReplicationTopology(refresh);
                        GroomAlwaysOnTopology(refresh);
                    }
                    catch (Exception ex)
                    {
                        LOG.Error("An error occured in the grooming of mirroring and\\or replication tables or Always On Topology. The error is " + ex.Message);
                    }

                    stopwatch.Stop();
                    LOG.DebugFormat("Save statistics took {0} milliseconds.", stopwatch.ElapsedMilliseconds);
                }
                catch (Exception e)
                {
                    bool rethrow = !IsSQLKeyException(e);

                    if (rethrow)
                        crapOutException = e;

                    // backout changes 
                    Rollback();

                    rolledback = rethrow;
                }

                if (rolledback)
                {
                    LOG.Info("SaveStatistics failed.  Updating last refresh time.");
                    // at the very least try to update the last refresh time in the monitored server table
                    try
                    {
                        UpdateLastRefreshTime(refresh);
                        Commit();
                    }
                    catch (Exception)
                    {
                        Rollback();
                    }
                }
                if (crapOutException != null)
                    throw crapOutException;
            }
        }

        /// <summary>
        /// To control duplicated SQL server keys at the moment to insert
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static bool IsSQLKeyException(Exception e)
        {
            if (e is SqlException)
            {
                // eat these exceptions
                switch (((SqlException)e).Number)
                {
                    case 547:
                        LOG.Info("Foreign Key violation in scheduled collection being dropped.");
                        return true;
                    case 2627:
                    case 2601:
                        LOG.Info("Unique Index/Constriant violation in scheduled collection being dropped.");
                        return true;
                }
            }
            return false;
        }

        private static void ExpireQueryMonitorCaches()
        {
            Management.ScheduledCollection.AddCachedSqlStatement(null);
            Management.ScheduledCollection.AddCachedApplicationName(null);
            Management.ScheduledCollection.AddCachedHostName(null);
            Management.ScheduledCollection.AddCachedLoginName(null);

        }

        protected SqlTransaction GetTransaction()
        {
            if (transaction == null)
                transaction = connection.BeginTransaction();

            return transaction;
        }

        protected void UpdateServerVersion(ScheduledRefresh refresh)
        {
            using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_UpdateServerVersion"))
            {
                try
                {
                    command.Transaction = GetTransaction();
                    SqlHelper.AssignParameterValues(command.Parameters,
                                                    refresh.MonitoredServer.Id,
                                                    refresh.ProductVersion.Version,
                                                    refresh.ProductEdition);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    if (!IsSQLKeyException(e))
                    {
                        LOG.Error("Update server version failed", e);
                        throw;
                    }
                }
            }
        }

        protected void UpdateStatisticsRunTime(ScheduledRefresh refresh)
        {
            using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_UpdateStatisticsRunTime"))
            {
                DateTime? lastGrowthRunTimeUTC = refresh.MonitoredServer.TableGrowthConfiguration.LastGrowthStatisticsRunTime == null
                                                     ? null
                                                     : refresh.TimeStamp;
                DateTime? lastReorgRunTimeUTC = refresh.MonitoredServer.TableFragmentationConfiguration.LastFragmentationStatisticsRunTime == null
                                                    ? null
                                                    : refresh.TimeStamp;
                try
                {
                    command.Transaction = GetTransaction();
                    SqlHelper.AssignParameterValues(command.Parameters,
                                                    refresh.MonitoredServer.Id,
                                                    refresh.MonitoredServer.TableGrowthConfiguration.LastGrowthStatisticsRunTime,
                                                    lastGrowthRunTimeUTC,
                                                    refresh.MonitoredServer.TableFragmentationConfiguration.LastFragmentationStatisticsRunTime,
                                                    lastReorgRunTimeUTC);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    if (!IsSQLKeyException(e))
                    {
                        LOG.Error("Update statistics run time failed", e);
                    }
                    throw;
                }
            }
        }

        public void UpdateLastRefreshTime(ScheduledRefresh refresh)
        {
            // This is now done in the transaction after alerts have all been added.

            //            try
            //            {
            //                SqlHelper.ExecuteNonQuery(GetTransaction(), "p_UpdateLastRefreshTime", refresh.MonitoredServer.Id, refresh.TimeStamp);
            //            }
            //            catch (Exception e)
            //            {
            //                LOG.Error("Update last refresh time failed", e);
            //                throw;
            //            }
        }

        protected void SaveServerStatistics(ScheduledRefresh refresh)
        {
            using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertServerStatistics"))
            {
                try
                {
                    long pcstart = 0, pcend = 0;

                    Statistics.QueryPerformanceCounter(ref pcend);
                    Statistics.ServerStatisticWritten(pcstart, pcend);

                    command.Transaction = GetTransaction();

                    decimal? tempDBSize = null;
                    double? tempDBSizePercent = null;
                    if (refresh.DbStatistics.ContainsKey("tempdb"))
                    {
                        tempDBSize = refresh.DbStatistics["tempdb"].UsedSize.Kilobytes;
                        tempDBSizePercent = refresh.DbStatistics["tempdb"].PercentDataSize * 100;
                    }

                    OpenTransaction singleOldestOpenTransaction = new OpenTransaction();
                    foreach (OpenTransaction tran in refresh.Alerts.OldestOpenTransactions)
                    {
                        if (!singleOldestOpenTransaction.RunTime.HasValue)
                        {
                            singleOldestOpenTransaction = tran;
                        }
                        else if (tran.RunTime.HasValue &&
                                 tran.RunTime.Value.TotalMilliseconds >
                                 singleOldestOpenTransaction.RunTime.Value.TotalMilliseconds)
                        {
                            singleOldestOpenTransaction = tran;
                        }
                    }
                    //5.4.1 (release 11)
                    double ReadThroughput = 0;
                    double WriteThroughput = 0;
                    double SwapUsage = 0;
                    double ReadLatency = 0;
                    double WriteLatency = 0;
                    double CPUCreditBalance = 0;
                    double CPUCreditUsage = 0;
                    double DiskQueueDepth = 0;
                    if (refresh.AWSCloudMetrics != null && refresh.AWSCloudMetrics.Count > 0)
                    {
                        refresh.AWSCloudMetrics.TryGetValue("ReadThroughput", out ReadThroughput);
                        refresh.AWSCloudMetrics.TryGetValue("WriteThroughput", out WriteThroughput);
                        refresh.AWSCloudMetrics.TryGetValue("SwapUsage", out SwapUsage);
                        refresh.AWSCloudMetrics.TryGetValue("ReadLatency", out ReadLatency);
                        refresh.AWSCloudMetrics.TryGetValue("WriteLatency", out WriteLatency);
                        refresh.AWSCloudMetrics.TryGetValue("CPUCreditBalance", out CPUCreditBalance);
                        refresh.AWSCloudMetrics.TryGetValue("CPUCreditUsage", out CPUCreditUsage);
                        refresh.AWSCloudMetrics.TryGetValue("DiskQueueDepth", out DiskQueueDepth);
                    }
                    //6.2.4
                    double dataIOUsage = 0;
                    double logIOUsage = 0;
                    double dataIORate = 0;
                    double logIORate = 0;
                    if (refresh.Server.AzureIOMetrics != null && refresh.Server.AzureIOMetrics.Count > 0)
                    {
                        String dataIOUsageStr;
                        String logIOUsageStr;
                        String dataIORateStr;
                        String logIORateStr;

                        foreach (var azureIoMetric in refresh.Server.AzureIOMetrics)
                        {
                            var initial = azureIoMetric;
                            initial.TryGetValue("dataIOUsage", out dataIOUsageStr);
                            initial.TryGetValue("logIOUsage", out logIOUsageStr);
                            initial.TryGetValue("dataIORate", out dataIORateStr);
                            initial.TryGetValue("logIORate", out logIORateStr);

                            dataIOUsage += Double.Parse(dataIOUsageStr);
                            logIOUsage += Double.Parse(logIOUsageStr);
                            dataIORate += Double.Parse(dataIORateStr);
                            logIORate += Double.Parse(logIORateStr);

                        }
                    }

                    SqlHelper.AssignParameterValues(command.Parameters, // procedure p_InsertServerStatistics
                                                    refresh.MonitoredServer.Id,
                                                    //	@SQLServerID int,                       
                                                    refresh.TimeStamp,
                                                    // 	@UTCCollectionDateTime datetime,        
                                                    refresh.Server.TimeDelta.Value.TotalSeconds,
                                                    (int)refresh.Server.AgentServiceStatus,
                                                    dataIOUsage,//6.2.4
                                                    logIOUsage,
                                                    dataIORate,
                                                    logIORate,
                                                    (int)refresh.Server.SqlServiceStatus,
                                                    (int)refresh.Server.DtcServiceStatus,
                                                    (int)refresh.Server.FullTextServiceStatus,
                                                    refresh.Server.Statistics.BufferCacheHitRatio,
                                                    // 	@BufferCacheHitRatioPercentage float,   
                                                    refresh.Server.Statistics.CheckpointPages,
                                                    // 	@CheckpointWrites bigint,               
                                                    refresh.Server.SystemProcesses.ComputersHoldingProcesses,
                                                    // 	@ClientComputers bigint,                
                                                    refresh.Server.Statistics.CpuPercentage.HasValue
                                                        ? Math.Round(refresh.Server.Statistics.CpuPercentage.Value, 3)
                                                        : refresh.Server.Statistics.CpuPercentage,
                                                    // 	@CPUActivityPercentage float,           
                                                    (refresh.Server.Statistics.CpuBusyDelta != null)
                                                        ? refresh.Server.Statistics.CpuBusyDelta.Ticks
                                                        : null, // 	@CPUTimeDelta bigint,                   
                                                    refresh.Server.Statistics.CpuBusyRaw,
                                                    // 	@CPUTimeRaw bigint,      
                                                    refresh.Replication.Publisher.ReplicationLatency,
                                                    //@DistributionLatencyInSeconds
                                                    refresh.Server.Statistics.FullScans,
                                                    // 	@FullScans bigint,                      
                                                    (refresh.Server.Statistics.IdleTimeDelta != null)
                                                        ? refresh.Server.Statistics.IdleTimeDelta.Ticks
                                                        : null, // 	@IdleTimeDelta bigint,                  
                                                    refresh.Server.Statistics.IdlePercentage.HasValue
                                                        ? Math.Round(refresh.Server.Statistics.IdlePercentage.Value, 3)
                                                        : refresh.Server.Statistics.IdlePercentage,
                                                    // 	@IdleTimePercentage float,              
                                                    refresh.Server.Statistics.IdleTimeRaw,
                                                    // 	@IdleTimeRaw bigint,                    
                                                    refresh.Server.Statistics.IoPercentage.HasValue
                                                        ? Math.Round(refresh.Server.Statistics.IoPercentage.Value, 3)
                                                        : refresh.Server.Statistics.IoPercentage,
                                                    // 	@IOActivityPercentage float,            
                                                    (refresh.Server.Statistics.IoTimeDelta != null)
                                                        ? refresh.Server.Statistics.IoTimeDelta.Ticks
                                                        : null, // 	@IOTimeDelta bigint,                    
                                                    refresh.Server.Statistics.IoTimeRaw,
                                                    // 	@IOTimeRaw bigint,                      
                                                    refresh.Server.Statistics.LazyWrites,
                                                    // 	@LazyWriterWrites bigint,               
                                                    refresh.Server.Statistics.LockWaits,
                                                    // 	@LockWaits bigint,                      
                                                    refresh.Server.Statistics.TotalConnections,
                                                    // 	@Logins bigint,                         
                                                    refresh.Server.Statistics.LogFlushes,
                                                    // 	@LogFlushes bigint,                      
                                                    refresh.Server.TargetServerMemory.Kilobytes,
                                                    // 	@MemoryAllocated bigint,                
                                                    refresh.Server.TotalServerMemory.Kilobytes,
                                                    // 	@MemoryUsed bigint,                     
                                                    singleOldestOpenTransaction.RunTime.HasValue
                                                        ? Math.Floor(
                                                              singleOldestOpenTransaction.RunTime.Value.TotalMinutes)
                                                        : 0, // 	@OldestOpenTransactionsInMinutes bigint,
                                                    refresh.Server.Statistics.PacketErrors,
                                                    // 	@PacketErrors bigint,                   
                                                    refresh.Server.Statistics.PacketsReceived,
                                                    // 	@PacketsReceived bigint,                
                                                    refresh.Server.Statistics.PacketsSent,
                                                    // 	@PacketsSent bigint,                    
                                                    refresh.Server.Statistics.DiskErrors,
                                                    // 	@PageErrors bigint,                     
                                                    (refresh.Server.Statistics.PageLifeExpectancy.HasValue)
                                                        ? refresh.Server.Statistics.PageLifeExpectancy.Value.
                                                              TotalSeconds
                                                        : 0, // 	@PageLifeExpectancy bigint,             
                                                    refresh.Server.Statistics.PageLookups,
                                                    // 	@PageLookups bigint,                    
                                                    refresh.Server.Statistics.PageReads,
                                                    // 	@PageReads bigint,                      
                                                    refresh.Server.Statistics.PageSplits,
                                                    // 	@PageSplits bigint,                     
                                                    refresh.Server.Statistics.PageWrites,
                                                    // 	@PageWrites bigint,                     
                                                    refresh.Server.Statistics.CacheHitRatio,
                                                    // 	@ProcedureCacheHitRatioPercentage float,
                                                    refresh.Server.ProcedureCacheSize.Kilobytes,
                                                    // 	@ProcedureCacheSize bigint,             
                                                    refresh.Server.ProcedureCachePercentageUsed,
                                                    // 	@ProcedureCacheSizePercent float,       
                                                    refresh.Server.Statistics.ReadaheadPages,
                                                     // 	@ReadAheadPages bigint,                 
                                                     refresh.Replication.SubscriptionLatency.TotalSeconds,
                                                    // 	@ReplicationLatencyInSeconds float,     
                                                    refresh.Replication.SubscribedDeliveredTransactions,
                                                    // 	@ReplicationSubscribed bigint,          
                                                    refresh.Replication.Publisher.ReplicatedTrans,
                                                    // 	@ReplicationUndistributed bigint,       
                                                    refresh.Replication.NonSubscribedTransactions,
                                                    // 	@ReplicationUnsubscribed bigint,        
                                                    refresh.Server.ResponseTime,
                        // 	@ResponseTimeInMilliseconds int,        
                        refresh.Server.ProductVersion != null ? refresh.Server.ProductVersion.Version : "11.0.0.0",
                                                    //  @ServerVersion nvarchar(30),
                                                    String.IsNullOrEmpty(refresh.Server.ProductEdition)
                                                        ? refresh.ProductEdition
                                                        : refresh.Server.ProductEdition,
                                                    refresh.Server.Statistics.SqlCompilations,
                                                    // 	@SqlCompilations bigint,                
                                                    refresh.Server.Statistics.SqlRecompilations,
                                                    // 	@SqlRecompilations bigint,              
                                                    refresh.Server.Statistics.TableLockEscalations,
                                                    // 	@TableLockEscalations bigint,           
                                                    tempDBSize, // 	@TempDBSize bigint,                     
                                                    tempDBSizePercent, // 	@TempDBSizePercent float,    
                                                                       //TODO: Fix this
                                                    refresh.Server.Statistics.BatchRequests,
                                                    // 	@Transactions bigint,                   
                                                    refresh.Server.SystemProcesses.CurrentUserProcesses,
                                                    // 	@UserProcesses bigint,                  
                                                    refresh.Server.Statistics.WorkfilesCreated,
                                                    // 	@WorkFilesCreated bigint,               
                                                    refresh.Server.Statistics.WorktablesCreated,
                                                    // 	@WorkTablesCreated bigint,   
                                                    refresh.Server.SystemProcesses.CurrentSystemProcesses,
                                                    //@SystemProcesses bigint,
                                                    refresh.Server.SystemProcesses.UserProcessesConsumingCpu,
                                                    //@UserProcessesConsumingCPU bigint, 
                                                    refresh.Server.SystemProcesses.SystemProcessesConsumingCpu,
                                                    //@SystemProcessesConsumingCPU bigint, 
                                                    refresh.Server.SystemProcesses.BlockedProcesses,
                                                    //@BlockedProcesses bigint, 
                                                    refresh.Server.SystemProcesses.OpenTransactions,
                                                    //@OpenTransactions bigint,
                                                    refresh.Server.DatabaseSummary.DatabaseCount, //@DatabaseCount int,
                                                    refresh.Server.DatabaseSummary.DataFileCount, //@DataFileCount int,
                                                    refresh.Server.DatabaseSummary.LogFileCount, //@LogFileCount int,
                                                    refresh.Server.DatabaseSummary.DataFileSpaceAllocated.Kilobytes ==
                                                    null
                                                        ? (object)null
                                                        :
                                                            Math.Round(
                                                                refresh.Server.DatabaseSummary.DataFileSpaceAllocated.
                                                                    Kilobytes.Value, 0),
                                                    //@DataFileSpaceAllocatedInKilobytes dec(12,0),
                                                    refresh.Server.DatabaseSummary.DataFileSpaceUsed.Kilobytes == null
                                                        ? (object)null
                                                        :
                                                            Math.Round(
                                                                refresh.Server.DatabaseSummary.DataFileSpaceUsed.
                                                                    Kilobytes.Value, 0),
                                                    //@DataFileSpaceUsedInKilobytes dec(12,0),
                                                    refresh.Server.DatabaseSummary.LogFileSpaceAllocated.Kilobytes ==
                                                    null
                                                        ? (object)null
                                                        :
                                                            Math.Round(
                                                                refresh.Server.DatabaseSummary.LogFileSpaceAllocated.
                                                                    Kilobytes.Value, 0),
                                                    //@LogFileSpaceAllocatedInKilobytes dec(12,0),
                                                    refresh.Server.DatabaseSummary.LogFileSpaceUsed.Kilobytes == null
                                                        ? (object)null
                                                        :
                                                            Math.Round(
                                                                refresh.Server.DatabaseSummary.LogFileSpaceUsed.
                                                                    Kilobytes.Value, 0),
                                                    //@LogFileSpaceUsedInKilobytes dec(12,0),
                                                    (refresh.Server.TotalLocks.HasValue)
                                                        ? Math.Round(refresh.Server.TotalLocks.Value, 0)
                                                        : refresh.Server.TotalLocks,
                                                    refresh.Server.Statistics.BufferCacheSize.Kilobytes,
                                                    refresh.Server.SystemProcesses.ActiveProcesses, //@ActiveProcesses
                                                    refresh.Server.SystemProcesses.LeadBlockers, //@LeadBlockers
                                                    refresh.MemoryStatistics.CommittedPages.Kilobytes == null
                                                        ? (object)null
                                                        : Math.Round(
                                                              refresh.MemoryStatistics.CommittedPages.Kilobytes.Value, 0),
                                                    //@CommittedInKilobytes
                                                    refresh.MemoryStatistics.ConnectionMemory.Kilobytes == null
                                                        ? (object)null
                                                        : Math.Round(
                                                              refresh.MemoryStatistics.ConnectionMemory.Kilobytes.Value,
                                                              0), //@ConnectionMemoryInKilobytes
                                                    refresh.MemoryStatistics.FreePages.Kilobytes == null
                                                        ? (object)null
                                                        : Math.Round(
                                                              refresh.MemoryStatistics.FreePages.Kilobytes.Value, 0),
                                                    //@FreePagesInKilobytes
                                                    refresh.MemoryStatistics.GrantedWorkspaceMemory.Kilobytes == null
                                                        ? (object)null
                                                        : Math.Round(
                                                              refresh.MemoryStatistics.GrantedWorkspaceMemory.Kilobytes.
                                                                  Value, 0), //@GrantedWorkspaceMemoryInKilobytes
                                                    refresh.MemoryStatistics.LockMemory.Kilobytes == null
                                                        ? (object)null
                                                        : Math.Round(
                                                              refresh.MemoryStatistics.LockMemory.Kilobytes.Value, 0),
                                                    //@LockMemoryInKilobytes
                                                    refresh.MemoryStatistics.OptimizerMemory.Kilobytes == null
                                                        ? (object)null
                                                        : Math.Round(
                                                              refresh.MemoryStatistics.OptimizerMemory.Kilobytes.Value,
                                                              0), //@OptimizerMemoryInKilobytes
                                                    refresh.MemoryStatistics.TotalServerMemory.Kilobytes == null
                                                        ? (object)null
                                                        : Math.Round(
                                                              refresh.MemoryStatistics.TotalServerMemory.Kilobytes.Value,
                                                              0), //@TotalServerMemoryInKilobytes
                                                    refresh.MemoryStatistics.FreeCachePages.Kilobytes == null
                                                        ? (object)null
                                                        : Math.Round(
                                                              refresh.MemoryStatistics.FreeCachePages.Kilobytes.Value, 0),
                                                    //@FreeCachePagesInKilobytes
                                                    refresh.MemoryStatistics.CachePages.Kilobytes == null
                                                        ? (object)null
                                                        : Math.Round(
                                                              refresh.MemoryStatistics.CachePages.Kilobytes.Value, 0),
                                                    //@CachePagesInKilobytes
                                                    refresh.Server.MaxConnections, //@MaxConnections
                                                    refresh.Server.PhysicalMemory.Kilobytes == null
                                                        ? (object)null
                                                        : Math.Round(refresh.Server.PhysicalMemory.Kilobytes.Value, 0),
                                                    //@PhysicalMemoryInKilobytes
                                                    refresh.Server.ProcessorCount, //@ProcessorCount
                                                    refresh.Server.ProcessorsUsed, //@ProcessorsUsed
                                                    refresh.Server.ProcessorType, //@ProcessorType
                                                    refresh.Server.ServerHostName, //@ServerHostName
                                                    refresh.Server.RealServerName, //@RealServerName
                                                    refresh.Server.WindowsVersion, //@WindowsVersion
                                                    refresh.Server.SqlServerEdition, //@SqlServerEdition
                                                    refresh.Server.RunningSince, //@RunningSince
                                                    refresh.Server.IsClustered, //@IsClustered
                                                    (refresh.Server.OSMetricsStatistics != null)
                                                        ? refresh.Server.OSMetricsStatistics.OsStatisticAvailability
                                                        : "procedure unavailable", //@OsStatisticAvailability
                                                    refresh.Server.ClusterNodeName,
                                                    refresh.Server.TempdbStatistics.VersionStoreGenerationKilobytes,
                                                    refresh.Server.TempdbStatistics.VersionStoreCleanupKilobytes,
                                                    refresh.Server.TempdbStatistics.TempdbPFSWaitTime.TotalMilliseconds,
                                                    refresh.Server.TempdbStatistics.TempdbGAMWaitTime.TotalMilliseconds,
                                                    refresh.Server.TempdbStatistics.TempdbSGAMWaitTime.TotalMilliseconds,
                                                    refresh.Server.Statistics.Transactions,
                                                    (int)refresh.Server.SQLBrowserServiceStatus,   //SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --update the new service columns
                                                    (int)refresh.Server.SQLActiveDirectoryHelperServiceStatus,
                                                    refresh.Server.ManagedInstanceStorageLimit,

                                                     ReadThroughput,//5.4.1
                                                     WriteThroughput,
                                                     SwapUsage,
                                                     ReadLatency,
                                                     WriteLatency,
                                                     CPUCreditBalance,
                                                     CPUCreditUsage,
                                                     DiskQueueDepth,
                                                     null
                        );

                    command.ExecuteNonQuery();
                    Commit();
                }
                catch (Exception e)
                {
                    // only log exception details if exception is not from a duplicate record
                    if (!IsSQLKeyException(e))
                    {
                        LOG.Error("Save server statistics failed", e);
                        throw;
                    }
                }
            }
        }


        private void ProcessEvents(ScheduledRefresh refresh)
        {
            //            IEnumerable<IEvent> events = refresh.Events;
            //            if (events != null)
            //            {
            //
            //                foreach (IEvent evnt in events)
            //                {
            //                    try
            //                    {
            //                        if (evnt is StateDeviationClearEvent)
            //                        {
            //                            DeleteOutstandingEvent(
            //                                new OutstandingEventEntry((evnt as StateDeviationClearEvent).DeviationEvent));
            //                            continue;
            //                        }
            //
            //                        if (evnt is StateDeviationEvent)
            //                        {
            //                            string subject = GetFormattedMessage(refresh, evnt as StateDeviationEvent);
            //                            SaveOutstandingEvent(new OutstandingEventEntry(evnt as StateDeviationEvent), subject);
            //                        }
            //                        else if (evnt is StateDeviationUpdateEvent)
            //                        {
            //                            string subject = GetFormattedMessage(refresh, ((StateDeviationUpdateEvent) evnt).DeviationEvent);
            //                            SaveOutstandingEvent(new OutstandingEventEntry((evnt as StateDeviationUpdateEvent).DeviationEvent), subject);
            //                        }
            //                    } catch (Exception e)
            //                    {
            //                        LOG.ErrorFormat("Error writing outstanding event for {0}", evnt);
            //                    }
            //                }
            //            }
            //if (refresh.DatabaseSizeError != null)
            //{
            //    StringBuilder message = new StringBuilder("Database size information could not be collected. ");
            //    for (Exception e = refresh.DatabaseSizeError; e != null; e = e.InnerException)
            //    {
            //        message.Append(" ").Append(e.Message);
            //    }
            //    try
            //    {
            //        AlertTableWriter.LogOperationalAlerts(
            //            Metric.Operational,
            //            new MonitoredObjectName(refresh.ServerName),
            //            MonitoredState.Warning,
            //            "Database size information could not be collected.",
            //            message.ToString());

            //        LOG.Warn("Operational Alert: ", message.ToString());
            //    }
            //    catch (Exception e)
            //    {
            //        LOG.Error("Error writing database size failure operational alert. ", e);
            //    }
            //}
            if (refresh.Alerts.LogScanFailure != null)
            {
                StringBuilder message = new StringBuilder("There was a problem processing the SQL Server or Agent Event Logs");
                for (Exception e = refresh.Alerts.LogScanFailure; e != null; e = e.InnerException)
                {
                    message.Append(" ").Append(e.Message);
                }
                try
                {
                    AlertTableWriter.LogOperationalAlerts(
                        Metric.Operational,
                        new MonitoredObjectName(refresh.ServerName),
                        MonitoredState.Warning,
                        "There was a problem processing the SQL Server or Agent Event Logs",
                        message.ToString());

                    LOG.Warn("Operational Alert: ", message.ToString());
                }
                catch (Exception e)
                {
                    LOG.Error("Error writing log scan failure operational alert. ", e);
                }
            }
            if (refresh.Alerts.ProbePermissionErrors != null && refresh.Alerts.ProbePermissionErrors.Count > 0)
            {
                // To Raise Operational Alerts for Probe Permission Errors
                foreach (var alertsProbePermissionError in refresh.Alerts.ProbePermissionErrors)
                {
                    try
                    {
                        AlertTableWriter.LogOperationalAlerts(
                            Metric.Operational,
                            new MonitoredObjectName(refresh.ServerName),
                            MonitoredState.Warning,
                            "Probe Permission Violation - " + alertsProbePermissionError.Name,
                            alertsProbePermissionError.ToString());

                        LOG.Warn("Operational Alert: ", alertsProbePermissionError.ToString());
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Error writing Probe Permission Violation failure operational alert. ", e);
                    }
                }
                refresh.Alerts.ProbePermissionErrors.Clear();
            }
            // Create Operational Alerts for Active Waits Probe Permission Failures
            if (refresh.ActiveWaits != null && refresh.ActiveWaits.ProbeError != null)
            {
                try
                {
                    AlertTableWriter.LogOperationalAlerts(
                        Metric.Operational,
                        new MonitoredObjectName(refresh.ServerName),
                        MonitoredState.Warning,
                        "Probe Permission Violation - " + refresh.ActiveWaits.ProbeError.Name,
                        refresh.ActiveWaits.ProbeError.ToString());

                    LOG.Warn("Operational Alert: ", refresh.ActiveWaits.ProbeError.ToString());
                }
                catch (Exception e)
                {
                    LOG.Error("Error writing Active Waits Probe Permission Violation failure operational alert. ", e);
                }
            }

            if (refresh.QueryMonitorError != null)
            {
                StringBuilder message = new StringBuilder("Query monitor data could not be collected. ");
                for (Exception e = refresh.QueryMonitorError; e != null; e = e.InnerException)
                {
                    message.Append(" ").Append(e.Message);
                }
                try
                {
                    AlertTableWriter.LogOperationalAlerts(
                        Metric.Operational,
                        new MonitoredObjectName(refresh.ServerName),
                        MonitoredState.Warning,
                        "Query monitor data could not be collected.",
                        message.ToString());

                    LOG.Warn("Operational Alert: ", message.ToString());
                }
                catch (Exception e)
                {
                    LOG.Error("Error writing query monitor failure operational alert. ", e);
                }
            }
        }

        //private string GetFormattedMessage(ScheduledRefresh refresh, BaseEvent baseEvent)
        //{
        //    string subject = null;
        //    try
        //    {
        //        // new state or updates need a formatted message
        //        MetricDefinitions definitions = Management.GetMetricDefinitions();
        //        MessageMap messageMap = definitions.GetMessages(baseEvent.MetricID);
        //        if (messageMap != null)
        //        {
        //            if (baseEvent.AdditionalData is CustomCounterSnapshot)
        //            {
        //                // update the additional data to include the metric description
        //                Pair<CustomCounterSnapshot, MetricDescription?> newData =
        //                    new Pair<CustomCounterSnapshot, MetricDescription?>();
        //                newData.First = (CustomCounterSnapshot) baseEvent.AdditionalData;
        //                newData.Second = definitions.GetMetricDescription(baseEvent.MetricID);
        //                baseEvent.AdditionalData = newData;
        //            }

        //            if (baseEvent.MetricID == (int) Metric.SqlServiceStatus &&
        //                (ServiceState) baseEvent.Value == ServiceState.UnableToMonitor)
        //            {
        //                subject = messageMap.FormatMessage(refresh, baseEvent, MessageType.Header);
        //                subject += ".  ";
        //                subject += refresh.Error != null ? refresh.Error.Message : "";
        //            }

        //            else
        //            {
        //                subject = messageMap.FormatMessage(refresh, baseEvent, MessageType.Header);
        //            }
        //        }
        //        else
        //        {
        //            subject = null;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        LOG.Error("Error creating formatted message for state event", e);
        //    }
        //    return subject;
        //}

        protected Dictionary<string, int> SaveDatabaseStatisticsBulk(ScheduledRefresh refresh)
        {
            var databaseNames = new Dictionary<string, int>();

            using (LOG.VerboseCall("SaveDatabaseStatisticsBulk"))
            {
                DataTable statsTable = new DataTable("DatabaseStats");
                Guid sourceGuid = Guid.NewGuid();

                DataColumn sourceIdColumn = new DataColumn("SourceID", typeof(SqlGuid));
                statsTable.Columns.Add(sourceIdColumn);
                statsTable.Columns.Add("ServerID", typeof(Int32));
                statsTable.Columns.Add("DatabaseName", typeof(string));
                statsTable.Columns.Add("SystemDatabase", typeof(bool));
                statsTable.Columns.Add("UTCCollectionDateTime", typeof(DateTime));
                statsTable.Columns.Add("DatabaseStatus", typeof(Int32));
                statsTable.Columns.Add("Transactions", typeof(Int64));
                statsTable.Columns.Add("LogFlushWaits", typeof(Int64));
                statsTable.Columns.Add("LogFlushes", typeof(Int64));
                statsTable.Columns.Add("LogKilobytesFlushed", typeof(Int64));
                statsTable.Columns.Add("LogCacheReads", typeof(Int64));
                statsTable.Columns.Add("LogCacheHitRatio", typeof(double));
                statsTable.Columns.Add("TimeDeltaInSeconds", typeof(double));
                statsTable.Columns.Add("NumberReads", typeof(decimal));
                statsTable.Columns.Add("NumberWrites", typeof(decimal));
                statsTable.Columns.Add("BytesRead", typeof(decimal));
                statsTable.Columns.Add("BytesWritten", typeof(decimal));
                statsTable.Columns.Add("IoStallMS", typeof(decimal));
                statsTable.Columns.Add("DatabaseCreateDate", typeof(DateTime));
                statsTable.Columns.Add("LastBackupDateTime", typeof(DateTime));
                //SQLdm 11 Azure Metrics
                statsTable.Columns.Add("AverageDataIO", typeof(decimal));
                statsTable.Columns.Add("AverageLogIO", typeof(decimal));
                statsTable.Columns.Add("MaxWorker", typeof(decimal));
                statsTable.Columns.Add("MaxSession", typeof(decimal));
                statsTable.Columns.Add("DatabaseAverageMemoryUsage", typeof(decimal));
                statsTable.Columns.Add("InMemoryStorageUsage", typeof(decimal));
                statsTable.Columns.Add("AvgCpuPercent", typeof(decimal));
                statsTable.Columns.Add("AvgDataIoPercent", typeof(decimal));
                statsTable.Columns.Add("AvgLogWritePercent", typeof(decimal));
                statsTable.Columns.Add("DtuLimit", typeof(Int64));

                // 6.2.3
                statsTable.Columns.Add("AzureCloudAllocatedMemory", typeof(decimal));
                statsTable.Columns.Add("AzureCloudUsedMemory", typeof(decimal));
                statsTable.Columns.Add("AzureCloudStorageLimit", typeof(decimal));

                //Elastic Pool Support
                statsTable.Columns.Add("ElasticPool", typeof(string));

                try
                {
                    foreach (DatabaseStatistics dbstats in refresh.DbStatistics.Values)
                    {

                        if (refresh.Server != null && refresh.MonitoredServer != null &&
                                dbstats.Status != DatabaseStatus.Undetermined && !String.IsNullOrEmpty(dbstats.Name) && refresh.Server.TimeDelta.HasValue && refresh.Server.TimeDelta.Value.TotalSeconds > 0)

                        {
                            DataRow row = statsTable.NewRow();

                            //SQLdm 11 Azure Metrics
                            decimal? averageDataIO = null, averageLogIO = null, maxWorker = null, maxSession = null, dbAvgMemoryUsage = null, inMemoryStorageUsage = null;
                            string elasticPool = null;
                            if (refresh.AzureCloudMetrics.ContainsKey(dbstats.Name))
                            {
                                if (refresh.AzureCloudMetrics[dbstats.Name].ContainsKey("AverageDataIOPercent"))
                                    averageDataIO = Convert.ToDecimal(refresh.AzureCloudMetrics[dbstats.Name]["AverageDataIOPercent"]);
                                if (refresh.AzureCloudMetrics[dbstats.Name].ContainsKey("AverageLogWritePercent"))
                                    averageLogIO = Convert.ToDecimal(refresh.AzureCloudMetrics[dbstats.Name]["AverageLogWritePercent"]);
                                if (refresh.AzureCloudMetrics[dbstats.Name].ContainsKey("MaxWorkerPercent"))
                                    maxWorker = Convert.ToDecimal(refresh.AzureCloudMetrics[dbstats.Name]["MaxWorkerPercent"]);
                                if (refresh.AzureCloudMetrics[dbstats.Name].ContainsKey("MaxSessionPercent"))
                                    maxSession = Convert.ToDecimal(refresh.AzureCloudMetrics[dbstats.Name]["MaxSessionPercent"]);
                                if (refresh.AzureCloudMetrics[dbstats.Name].ContainsKey("DatabaseAverageMemoryUsagePercent"))
                                    dbAvgMemoryUsage = Convert.ToDecimal(refresh.AzureCloudMetrics[dbstats.Name]["DatabaseAverageMemoryUsagePercent"]);
                                if (refresh.AzureCloudMetrics[dbstats.Name].ContainsKey("InMemoryStorageUsagePercent"))
                                    inMemoryStorageUsage = Convert.ToDecimal(refresh.AzureCloudMetrics[dbstats.Name]["InMemoryStorageUsagePercent"]);
                            }
                            foreach (var pool in refresh.AzureElasticPools)
                            {
                                if (pool.Value.Contains(dbstats.Name))
                                {
                                    elasticPool = pool.Key;
                                }
                            }
                            row["SourceID"] = sourceGuid;
                            row["ServerID"] = refresh.MonitoredServer.Id;       //used to join to sqlserverdatabasenames to get the dbid
                            row["DatabaseName"] = dbstats.Name;                 //used to join to sqlserverdatabasenames to get the dbid
                            row["SystemDatabase"] = dbstats.IsSystemDatabase;   //inserted into SQLServerDatabaseNames if it is not already in there
                            row["UTCCollectionDateTime"] = refresh.TimeStamp;
                            row["DatabaseStatus"] = (int)dbstats.Status;
                            row["Transactions"] = dbstats.Transactions ?? (object)DBNull.Value;
                            row["LogFlushWaits"] = dbstats.LogFlushWaits ?? (object)DBNull.Value;
                            row["LogFlushes"] = dbstats.LogFlushes ?? (object)DBNull.Value;
                            row["LogKilobytesFlushed"] = SafeKilobytes<long>(dbstats.LogSizeFlushed) ?? (object)DBNull.Value;
                            row["LogCacheReads"] = dbstats.LogCacheReads ?? (object)DBNull.Value;
                            row["LogCacheHitRatio"] = dbstats.LogCacheHitRatio ?? (object)DBNull.Value;
                            row["TimeDeltaInSeconds"] = refresh.Server.TimeDelta.Value.TotalSeconds;
                            row["NumberReads"] = dbstats.Reads ?? (object)DBNull.Value;
                            row["NumberWrites"] = dbstats.Writes ?? (object)DBNull.Value;
                            row["BytesRead"] = dbstats.BytesRead ?? (object)DBNull.Value;
                            row["BytesWritten"] = dbstats.BytesWritten ?? (object)DBNull.Value;
                            row["IoStallMS"] = dbstats.IoStallMs ?? (object)DBNull.Value;
                            row["DatabaseCreateDate"] = dbstats.DateCreated ?? (object)DBNull.Value;    //inserted into SQLServerDatabaseNames if it is not already in there
                            row["LastBackupDateTime"] = dbstats.LastBackup ?? (object)DBNull.Value; //SQLdm 10.0 (Gaurav Karwal): adding last backup date
                            //SQLdm 11 Azure Metrics
                            row["AverageDataIO"] = averageDataIO ?? (object)DBNull.Value;
                            row["AverageLogIO"] = averageLogIO ?? (object)DBNull.Value;
                            row["MaxWorker"] = maxWorker ?? (object)DBNull.Value;
                            row["MaxSession"] = maxSession ?? (object)DBNull.Value;
                            row["DatabaseAverageMemoryUsage"] = dbAvgMemoryUsage ?? (object)DBNull.Value;
                            row["InMemoryStorageUsage"] = inMemoryStorageUsage ?? (object)DBNull.Value;

                            if (dbstats.AzureDbDetail != null)
                            {
                                row["AvgCpuPercent"] = dbstats.AzureDbDetail.AvgCpuPercent;
                                row["AvgDataIoPercent"] = dbstats.AzureDbDetail.AvgDataIoPercent;
                                row["AvgLogWritePercent"] = dbstats.AzureDbDetail.AvgLogWritePercent;
                                row["DtuLimit"] = dbstats.AzureDbDetail.DtuLimit ?? (object)DBNull.Value;
                            }

                            // 6.2.3
                            row["AzureCloudAllocatedMemory"] = dbstats.AzureCloudAllocatedMemory ?? (object)DBNull.Value;
                            row["AzureCloudUsedMemory"] = dbstats.AzureCloudUsedMemory ?? (object)DBNull.Value;
                            row["AzureCloudStorageLimit"] = dbstats.AzureCloudStorageLimit ?? (object)DBNull.Value;

                            // Elastic Pool Support
                            row["ElasticPool"] = elasticPool;
                            statsTable.Rows.Add(row);
                        }
                    }

                    statsTable.AcceptChanges();

                    long pcstart = 0, pcend = 0;
                    Statistics.QueryPerformanceCounter(ref pcstart);

                    using (var xa = GetTransaction())
                    {
                        SqlDataReader reader = null;
                        try
                        {
                            using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_DatabaseStatisticsFromStaging"))
                            {
                                command.Transaction = xa;

                                ServerVersion version = new ServerVersion(connection.ServerVersion);
                                SqlBulkCopyOptions options = version.Major >= 9
                                                                             ? SqlBulkCopyOptions.Default
                                                                             : SqlBulkCopyOptions.Default |
                                                                               SqlBulkCopyOptions.CheckConstraints;

                                using (var bulkCopy = new SqlBulkCopy(connection, options, xa))
                                {
                                    bulkCopy.DestinationTableName = "stageDatabaseStatistics";
                                    //This cant work because a view cannot support the update of 2 base tables
                                    //bulkCopy.DestinationTableName = "viewStageDatabaseStatistics";
                                    bulkCopy.WriteToServer(statsTable);

                                    try
                                    {
                                        SqlHelper.AssignParameterValues(command.Parameters, sourceGuid);
                                        reader = command.ExecuteReader();

                                        while (reader.Read())
                                        {
                                            databaseNames.Add((string)reader["DatabaseName"],
                                                              (int)reader["DatabaseID"]);
                                        }
                                    }
                                    finally
                                    {
                                        if (reader != null) reader.Close();

                                    }
                                }
                                Commit();
                            }

                        }
                        catch (Exception e)
                        {
                            try
                            {
                                Rollback();
                            }
                            catch
                            {
                                /* */
                            }

                            if (!IsSQLKeyException(e))
                            {
                                LOG.ErrorFormat("Exception doing bulk copy to the Database stats details: {0}", e);
                            }
                            throw;
                        }
                    }

                    Statistics.QueryPerformanceCounter(ref pcend);
                    Statistics.DatabaseStatisticWrittenBulk(pcstart, pcend, statsTable.Rows.Count);
                }
                catch (Exception e)
                {
                    if (!IsSQLKeyException(e))
                    {
                        LOG.ErrorFormat("Exception in SaveDatabaseStatisticsBulk: {0}", e);
                    }
                    throw;
                }
                return databaseNames;
            }
        }

        /// <summary>
        /// Get List of Databases using stored procedure p_GetDatabases
        /// </summary>
        /// <param name="connectionInfo">Connection Information</param>
        /// <param name="serverId">Server Id</param>
        /// <returns>List of databases excluding system databases</returns>
        public static List<String> GetDatabaseList(SqlConnection connectionInfo, int serverId)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connectionInfo, "p_GetDatabases", serverId))
            {
                List<String> databases = new List<String>();

                while (dataReader.Read())
                {
                    string database = dataReader.GetString(1).Trim();
                    bool isSystemDatabase = dataReader.GetBoolean(2);

                    // Excluding System Databases
                    if (database.Length > 0 && !isSystemDatabase)
                        databases.Add(database);
                }

                return databases;
            }

        }

        protected Dictionary<string, int> SaveDatabaseStatistics(ScheduledRefresh refresh)
        {
            Dictionary<string, int> DatabaseNames = new Dictionary<string, int>();

            using (LOG.VerboseCall("SaveDatabaseStatistics"))
            {
                bool tableStatisticsSaved = false;
                bool blnBulkInsertSucceeded = false;
                try
                {
                    DatabaseNames = SaveDatabaseStatisticsBulk(refresh);
                    blnBulkInsertSucceeded = true;
                }
                catch (Exception e1)
                {
                    if (!IsSQLKeyException(e1))
                    {
                        LOG.ErrorFormat("Exception occurred in SaveDatabaseStatisticsBulk: {0}", e1);
                    }
                }
                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertDatabaseStatistics"))
                {

                    try
                    {
                        int databaseID = -1;

                        // SQLdm 10.1.3(Varun Chopra) - Get List of all the databases excluding system databases
                        List<string> sqlServerDatabaseNames = GetDatabaseList(connection, refresh.MonitoredServer.Id);

                        foreach (DatabaseStatistics dbstats in refresh.DbStatistics.Values)
                        {
                            long pcstart = 0, pcend = 0;

                            decimal? averageDataIO = null, averageLogIO = null, maxWorker = null, maxSession = null, dbAvgMemoryUsage = null, inMemoryStorageUsage = null;
                            // SQLdm 10.1.3(Varun Chopra) - Remove Active Databases from database list
                            if (!string.IsNullOrEmpty(dbstats.Name) && sqlServerDatabaseNames.Contains(dbstats.Name))
                            {
                                // Remove Active Databases
                                sqlServerDatabaseNames.Remove(dbstats.Name);
                            }
                            // SQLdm 11 Azure Metrics
                            if (refresh.AzureCloudMetrics.ContainsKey(dbstats.Name))
                            {
                                if (refresh.AzureCloudMetrics[dbstats.Name].ContainsKey("AverageDataIOPercent"))
                                    averageDataIO = Convert.ToDecimal(refresh.AzureCloudMetrics[dbstats.Name]["AverageDataIOPercent"]);
                                if (refresh.AzureCloudMetrics[dbstats.Name].ContainsKey("AverageLogWritePercent"))
                                    averageLogIO = Convert.ToDecimal(refresh.AzureCloudMetrics[dbstats.Name]["AverageLogWritePercent"]);
                                if (refresh.AzureCloudMetrics[dbstats.Name].ContainsKey("MaxWorkerPercent"))
                                    maxWorker = Convert.ToDecimal(refresh.AzureCloudMetrics[dbstats.Name]["MaxWorkerPercent"]);
                                if (refresh.AzureCloudMetrics[dbstats.Name].ContainsKey("MaxSessionPercent"))
                                    maxSession = Convert.ToDecimal(refresh.AzureCloudMetrics[dbstats.Name]["MaxSessionPercent"]);
                                if (refresh.AzureCloudMetrics[dbstats.Name].ContainsKey("DatabaseAverageMemoryUsagePercent"))
                                    dbAvgMemoryUsage = Convert.ToDecimal(refresh.AzureCloudMetrics[dbstats.Name]["DatabaseAverageMemoryUsagePercent"]);
                                if (refresh.AzureCloudMetrics[dbstats.Name].ContainsKey("InMemoryStorageUsagePercent"))
                                    inMemoryStorageUsage = Convert.ToDecimal(refresh.AzureCloudMetrics[dbstats.Name]["InMemoryStorageUsagePercent"]);
                            }

                            // Only save if we actually read the database and have calculated data
                            if (refresh.Server != null && refresh.MonitoredServer != null &&
                                dbstats.Status != DatabaseStatus.Undetermined && !String.IsNullOrEmpty(dbstats.Name) && refresh.Server.TimeDelta.HasValue && refresh.Server.TimeDelta.Value.TotalSeconds > 0)
                            {
                                if (!blnBulkInsertSucceeded)
                                {
                                    Statistics.QueryPerformanceCounter(ref pcstart);

                                    LOG.VerboseFormat("Saving database statistics for '{0}'", dbstats.Name);
                                    command.Transaction = GetTransaction();
                                    SqlHelper.AssignParameterValues(command.Parameters,
                                                                    refresh.MonitoredServer.Id,
                                                                    dbstats.Name,
                                                                    dbstats.IsSystemDatabase,
                                                                    refresh.TimeStamp,
                                                                    (int)dbstats.Status,
                                                                    dbstats.Transactions,
                                                                    dbstats.LogFlushWaits,
                                                                    dbstats.LogFlushes,
                                                                    SafeKilobytes<long>(dbstats.LogSizeFlushed),
                                                                    dbstats.LogCacheReads,
                                                                    dbstats.LogCacheHitRatio,
                                                                    refresh.Server.TimeDelta.Value.TotalSeconds,
                                                                    null,
                                                                    dbstats.Reads,
                                                                    dbstats.Writes,
                                                                    dbstats.BytesRead,
                                                                    dbstats.BytesWritten,
                                                                    dbstats.IoStallMs,
                                                                    dbstats.DateCreated,
                                                                    dbstats.LastBackup,
                                                                    averageDataIO, //SQLdm 11 Azure Metric
                                                                    averageLogIO, //SQLdm 11 Azure Metric
                                                                    maxWorker, //SQLdm 11 Azure Metric
                                                                    maxSession, //SQLdm 11 Azure Metric
                                                                    dbAvgMemoryUsage, //SQLdm 11 Azure Metric
                                                                    inMemoryStorageUsage, //SQLdm 11 Azure Metric
                                                                    null, //  @ReturnMessage
                                                                    dbstats.AzureDbDetail.AvgCpuPercent,
                                                                    dbstats.AzureDbDetail.AvgDataIoPercent,
                                                                    dbstats.AzureDbDetail.AvgLogWritePercent,
                                                                    dbstats.AzureDbDetail.DtuLimit,
                                                                    dbstats.AzureCloudAllocatedMemory,
                                                                    dbstats.AzureCloudUsedMemory,
                                                                    dbstats.AzureCloudStorageLimit,
                                                                    dbstats.ElasticPool
                                        );

                                    command.ExecuteNonQuery();
                                    Commit();
                                    databaseID = (int)command.Parameters["@ReturnDatabaseID"].Value;
                                    if (!DatabaseNames.ContainsKey(dbstats.Name))
                                        DatabaseNames.Add(dbstats.Name, databaseID);
                                    LOG.VerboseFormat("Database statistics saved.  Database id for '{0}' is {1}",
                                                      dbstats.Name, databaseID);

                                    Statistics.QueryPerformanceCounter(ref pcend);
                                    Statistics.DatabaseStatisticWritten(pcstart, pcend);
                                }
                                else
                                {
                                    if (DatabaseNames.ContainsKey(dbstats.Name))
                                    {
                                        DatabaseNames.TryGetValue(dbstats.Name, out databaseID);
                                    }
                                    else
                                    {
                                        databaseID = -1;
                                    }
                                }

                                if (databaseID > 0 && dbstats.Files != null && dbstats.Files.Count > 0)
                                {

                                    if (dbstats.Name == "tempdb")
                                    {
                                        using (
                                            SqlCommand tempdFileCommand = SqlHelper.CreateCommand(connection,
                                                                                                  "p_InsertTempdbFileData")
                                            )
                                        {
                                            long pcstartTempDB = 0, pcendTempDB = 0;
                                            Statistics.QueryPerformanceCounter(ref pcstartTempDB);

                                            tempdFileCommand.Transaction = GetTransaction();
                                            foreach (FileActivityFile file in dbstats.Files.Values)
                                            {
                                                if (file is TempdbFileActivity)
                                                {
                                                    TempdbFileActivity tempdbFile = (TempdbFileActivity)file;

                                                    LOG.VerboseFormat("Saving tempdb file data for '{0}'",
                                                                      file.Filename);

                                                    SqlHelper.AssignParameterValues(tempdFileCommand.Parameters,
                                                                                    databaseID,
                                                                                    tempdbFile.Filename,
                                                                                    tempdbFile.FileType,
                                                                                    tempdbFile.Filepath,
                                                                                    null,
                                                                                    refresh.TimeStamp,
                                                                                    refresh.Server.TimeDelta.
                                                                                        HasValue
                                                                                        ? refresh.Server.TimeDelta.
                                                                                              Value
                                                                                              .
                                                                                              TotalSeconds
                                                                                        : 0,
                                                                                    tempdbFile.DriveName,
                                                                                    SafeKilobytes<long>(tempdbFile.FileSize),
                                                                                    SafeKilobytes<long>(tempdbFile.UserObjects),
                                                                                    SafeKilobytes<long>(tempdbFile.InternalObjects),
                                                                                    SafeKilobytes<long>(tempdbFile.VersionStore),
                                                                                    SafeKilobytes<long>(tempdbFile.MixedExtents),
                                                                                    SafeKilobytes<long>(tempdbFile.UnallocatedSpace)
                                                        );
                                                    tempdFileCommand.ExecuteNonQuery();


                                                }
                                            }
                                            Commit();
                                            Statistics.QueryPerformanceCounter(ref pcendTempDB);
                                            Statistics.TempDBFileStatisticWritten(pcstartTempDB, pcendTempDB, dbstats.Files.Values.Count);
                                        }
                                    }
                                }

                            }
                            else
                            {
                                if (refresh.Server != null && refresh.MonitoredServer != null && !String.IsNullOrEmpty(dbstats.Name))
                                {
                                    if (!DatabaseNames.ContainsKey(dbstats.Name))
                                    {

                                        using (
                                            SqlCommand insertDbNameCommand = SqlHelper.CreateCommand(connection,
                                                                                                  "p_InsertDatabaseName")
                                            )
                                        {
                                            insertDbNameCommand.Transaction = GetTransaction();

                                            SqlHelper.AssignParameterValues(insertDbNameCommand.Parameters,
                                                                                       refresh.MonitoredServer.Id,
                                                                dbstats.Name,
                                                                dbstats.IsSystemDatabase,
                                                                dbstats.DateCreated,
                                                                0,
                                                                null,
                                                                null
                                                            );

                                            insertDbNameCommand.ExecuteNonQuery();
                                            databaseID = (int)insertDbNameCommand.Parameters["@DatabaseID"].Value;
                                            if (!DatabaseNames.ContainsKey(dbstats.Name))
                                                DatabaseNames.Add(dbstats.Name, databaseID);

                                            Commit();
                                        }


                                    }
                                }
                            }


                            if (dbstats.TableSizes.Count > 0)
                            {
                                command.Transaction = GetTransaction();
                                LOG.VerboseFormat("Saving {1} table growth statistics for '{0}'", dbstats.Name,
                                                  dbstats.TableSizes.Count);
                                TimeSpan? timeDelta = refresh.MonitoredServer.TableGrowthConfiguration.LastGrowthStatisticsRunTime -
                                                      refresh.MonitoredServer.TableGrowthConfiguration.PreviousGrowthStatisticsRunTime;

                                SaveTableGrowthStatistics(dbstats, databaseID, refresh.MonitoredServer.Id,
                                                          refresh.TimeStamp, timeDelta);
                                tableStatisticsSaved = true;
                                LOG.VerboseFormat("Table growth statistics for '{0}' saved", dbstats.Name);
                                Commit();
                            }
                            if (dbstats.TableReorganizations.Count > 0)
                            {
                                command.Transaction = GetTransaction();
                                LOG.VerboseFormat("Saving {1} table reorg statistics for '{0}'", dbstats.Name,
                                                  dbstats.TableReorganizations.Count);
                                TimeSpan? timeDelta = refresh.MonitoredServer.TableFragmentationConfiguration.LastFragmentationStatisticsRunTime -
                                                      refresh.MonitoredServer.TableFragmentationConfiguration.PreviousFragmentationStatisticsRunTime;

                                SaveReorganization(dbstats, databaseID, refresh.MonitoredServer.Id,
                                                   refresh.TimeStamp, timeDelta);
                                tableStatisticsSaved = true;
                                LOG.VerboseFormat("Table reorg statistics for '{0}' saved", dbstats.Name);
                                Commit();
                            }
                        }

                        if (sqlServerDatabaseNames.Count > 0)
                        {
                            // SQLdm 10.1.3(Varun Chopra) - Update IsDeleted column for inactive databases
                            // SQLdm 10.1.3(Vamshi Krishna) - Changing the params order as the sp moved the isdeleted as last param
                            foreach (var sqlServerDatabaseName in sqlServerDatabaseNames)
                            {
                                using (SqlCommand insertDbNameCommand = SqlHelper.CreateCommand(connection, "p_InsertDatabaseName"))
                                {
                                    insertDbNameCommand.Transaction = GetTransaction();

                                    SqlHelper.AssignParameterValues(insertDbNameCommand.Parameters,
                                                        refresh.MonitoredServer.Id,
                                                        sqlServerDatabaseName,
                                                        0,
                                                        null,
                                                        null,
                                                        null,
                                                        1
                                                    );

                                    insertDbNameCommand.ExecuteNonQuery();

                                    Commit();
                                }
                            }
                        }

                        if (tableStatisticsSaved)
                            UpdateStatisticsRunTime(refresh);
                        return DatabaseNames;
                    }
                    catch (Exception e)
                    {
                        if (!IsSQLKeyException(e))
                        {
                            LOG.Error("Save database statistics failed", e);
                        }
                        throw;
                    }
                }
            }
        }



        internal Dictionary<string, int> SaveDatabaseSize(DatabaseSizeSnapshot refresh)
        {
            using (LOG.VerboseCall("SaveDatabaseSize"))
            {
                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertDatabaseSize"))
                {
                    Dictionary<string, int> DatabaseNames = new Dictionary<string, int>();
                    try
                    {
                        int databaseID = -1;

                        foreach (DatabaseStatistics dbstats in refresh.DbStatistics.Values)
                        {
                            command.Transaction = GetTransaction();
                            // Only save if we actually read the database
                            if (dbstats.Status != DatabaseStatus.Undetermined && !String.IsNullOrEmpty(dbstats.Name))
                            {
                                LOG.VerboseFormat("Saving database statistics for '{0}'", dbstats.Name);
                                SqlHelper.AssignParameterValues(command.Parameters,
                                                                refresh.Id,
                                                                dbstats.Name,
                                                                dbstats.IsSystemDatabase,
                                                                refresh.TimeStamp,
                                                                (int)dbstats.Status,
                                                                SafeKilobytes<decimal>(dbstats.DataFileSize),
                                                                SafeKilobytes<decimal>(dbstats.LogFileSize),
                                                                SafeKilobytes<decimal>(dbstats.LogSizeUsed),
                                                                SafeKilobytes<decimal>(dbstats.DataSize),
                                                                SafeKilobytes<decimal>(dbstats.TextSize),
                                                                SafeKilobytes<decimal>(dbstats.IndexSize),
                                                                SafeKilobytes<decimal>(dbstats.LogExpansion),
                                                                SafeKilobytes<decimal>(dbstats.DatabaseExpansion),
                                                                dbstats.PercentLogSpace != null
                                                                    ? dbstats.PercentLogSpace * 100
                                                                    : null,
                                                                dbstats.PercentDataSize != null
                                                                    ? dbstats.PercentDataSize * 100
                                                                    : null,
                                                                refresh.TimeDelta.HasValue
                                                                    ? refresh.TimeDelta.Value.TotalSeconds
                                                                    : 0,
                                                                null,
                                                                dbstats.DateCreated,
                                                                null //  @ReturnMessage
                                    );

                                command.ExecuteNonQuery();
                                databaseID = (int)command.Parameters["@ReturnDatabaseID"].Value;
                                // Defect fix for DE 20479 Aditya Shukla SQLdm 8.6
                                if (databaseID > 0)
                                {
                                    if (!DatabaseNames.ContainsKey(dbstats.Name))
                                        DatabaseNames.Add(dbstats.Name, databaseID);
                                }
                                Commit();
                                LOG.VerboseFormat("Database size statistics saved.  Database id for '{0}' is {1}",
                                                  dbstats.Name, databaseID);
                                //START SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - Saving FileGroup Statistics
                                using (SqlCommand fileGroupCommand = SqlHelper.CreateCommand(connection, "p_AddDatabaseFileInformation"))
                                {

                                    foreach (var fileStats in dbstats.FileStatistics)
                                    {
                                        try
                                        {
                                            fileGroupCommand.Transaction = GetTransaction();
                                            LOG.VerboseFormat("Saving database file group for '{0}'", fileStats.FileGroupName + "-" + fileStats.DatabaseName);
                                            SqlParameter[] parameters = SqlHelperParameterCache.GetSpParameterSet(connection, "p_AddDatabaseFileInformation");

                                            parameters[0].Value = fileStats.MaximumSize;
                                            parameters[1].Value = fileStats.InitialSize;
                                            parameters[2].Value = fileStats.UsedSpace;
                                            parameters[3].Value = fileStats.AvailableSpace;
                                            parameters[4].Value = fileStats.FreeDiskSpace;
                                            parameters[5].Value = fileStats.DriveName;
                                            parameters[6].Value = fileStats.FileName;
                                            parameters[7].Value = fileStats.FilePath;
                                            parameters[8].Value = fileStats.FileGroupName;
                                            parameters[9].Value = fileStats.IsDataFile;
                                            parameters[10].Value = dbstats.Name;
                                            parameters[11].Value = refresh.Id;
                                            parameters[12].Value = fileStats.UTCCollectionDateTime;
                                            SqlHelper.AssignParameterValues(fileGroupCommand.Parameters, parameters);
                                            fileGroupCommand.ExecuteNonQuery();
                                            Commit();
                                        }
                                        //START SQLdm 9.1 (Ankit Srivastava) Fixed an issue of Deadlock which was failing the Save Database Statistics Collection
                                        catch (Exception ex)
                                        {
                                            LOG.ErrorFormat("Saving database file group for '{0}' failed: {1}", fileStats.FileGroupName + "-" + fileStats.DatabaseName, ex, ex.InnerException);
                                            Rollback();
                                        }
                                        //END SQLdm 9.1 (Ankit Srivastava) Fixed an issue of Deadlock which was failing the Save Database Statistics Collection

                                    }
                                }
                                //END SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - Saving FileGroup Statistics
                                if (databaseID > 0 && dbstats.Files != null && dbstats.Files.Count > 0)
                                {
                                    using (
                                        SqlCommand fileCommand = SqlHelper.CreateCommand(connection, "p_InsertFileActivity")
                                        )
                                    {
                                        fileCommand.Transaction = GetTransaction();
                                        foreach (FileActivityFile file in dbstats.Files.Values)
                                        {
                                            if (file.Reads != null || file.Writes != null)
                                            {
                                                LOG.VerboseFormat("Saving database file activity for '{0}'",
                                                                  file.Filename);

                                                SqlHelper.AssignParameterValues(fileCommand.Parameters,
                                                                                databaseID,
                                                                                file.Filename,
                                                                                file.FileType,
                                                                                file.Filepath,
                                                                                null,
                                                                                refresh.TimeStamp,
                                                                                refresh.TimeDelta.HasValue
                                                                                    ? refresh.TimeDelta.Value.
                                                                                          TotalSeconds
                                                                                    : 0,
                                                                                file.Reads,
                                                                                file.Writes,
                                                                                file.DriveName);
                                                fileCommand.ExecuteNonQuery();

                                            }
                                        }
                                        Commit();

                                    }

                                    if (dbstats.Name == "tempdb")
                                    {
                                        using (
                                            SqlCommand tempdFileCommand = SqlHelper.CreateCommand(connection,
                                                                                                  "p_InsertTempdbFileData")
                                            )
                                        {
                                            tempdFileCommand.Transaction = GetTransaction();
                                            foreach (FileActivityFile file in dbstats.Files.Values)
                                            {
                                                if (file is TempdbFileActivity)
                                                {
                                                    TempdbFileActivity tempdbFile = (TempdbFileActivity)file;

                                                    LOG.VerboseFormat("Saving tempdb file data for '{0}'",
                                                                      file.Filename);

                                                    SqlHelper.AssignParameterValues(tempdFileCommand.Parameters,
                                                                                    databaseID,
                                                                                    tempdbFile.Filename,
                                                                                    tempdbFile.FileType,
                                                                                    tempdbFile.Filepath,
                                                                                    null,
                                                                                    refresh.TimeStamp,
                                                                                    refresh.TimeDelta.
                                                                                        HasValue
                                                                                        ? refresh.TimeDelta.
                                                                                              Value
                                                                                              .
                                                                                              TotalSeconds
                                                                                        : 0,
                                                                                    tempdbFile.DriveName,
                                                                                    SafeKilobytes<long>(tempdbFile.FileSize),
                                                                                    SafeKilobytes<long>(tempdbFile.UserObjects),
                                                                                    SafeKilobytes<long>(tempdbFile.InternalObjects),
                                                                                    SafeKilobytes<long>(tempdbFile.VersionStore),
                                                                                    SafeKilobytes<long>(tempdbFile.MixedExtents),
                                                                                    SafeKilobytes<long>(tempdbFile.UnallocatedSpace)
                                                        );
                                                    tempdFileCommand.ExecuteNonQuery();


                                                }
                                            }
                                            Commit();
                                        }
                                    }
                                }

                            }
                            else
                                continue;
                        }

                    }
                    catch (Exception e)
                    {
                        try
                        {
                            // The System.Data.SqlClient.SqlTransaction's Rollback implementation could raise an exeption.
                            Rollback();
                        }
                        catch (Exception re)
                        {
                            LOG.Error("Rollback failed", re);
                        }
                        if (!IsSQLKeyException(e))
                        {
                            LOG.Error("Save database statistics failed", e);
                            throw;
                        }
                    }
                    // VH - Removed 7/25/2012
                    //SaveDiskDrives(refresh);
                    return DatabaseNames;
                }
            }
        }

        /// <summary>
        /// save disk drive statistics -//SQLdm 9.1 (Ankit Srivastava) -Filegroup and Mount Point Monitoring Improvements - new method
        /// </summary>
        /// <param name="refresh"></param>
        internal void SaveDiskDriveStatistics(DatabaseSizeSnapshot refresh)
        {
            using (LOG.VerboseCall("SaveDiskDriveStatistics"))
            {
                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_AddDiskDriveInformation"))
                {
                    Dictionary<string, int> DatabaseNames = new Dictionary<string, int>();
                    try
                    {
                        foreach (DiskDriveStatistics diskDriveInfo in refresh.DiskDriveStatistics)
                        {
                            try
                            {
                                command.Transaction = GetTransaction();
                                // Only save if we actually read the disk 
                                if (!String.IsNullOrEmpty(diskDriveInfo.DriveName))
                                {
                                    LOG.VerboseFormat("Saving database statistics for '{0}'", diskDriveInfo.DriveName);
                                    SqlParameter[] parameters = SqlHelperParameterCache.GetSpParameterSet(connection, "p_AddDiskDriveInformation");
                                    parameters[0].Value = refresh.Id;
                                    parameters[1].Value = diskDriveInfo.UTCCollectionDateTime;
                                    parameters[2].Value = diskDriveInfo.DriveName;
                                    parameters[3].Value = diskDriveInfo.UnusedSizeKB;
                                    parameters[4].Value = diskDriveInfo.TotalSizeKB;
                                    parameters[5].Value = diskDriveInfo.DiskReadsPerSecond;
                                    parameters[6].Value = diskDriveInfo.DiskWritesPerSecond;
                                    SqlHelper.AssignParameterValues(command.Parameters, parameters);

                                    command.ExecuteNonQuery();
                                    Commit();
                                }
                            }
                            //START SQLdm 9.1 (Ankit Srivastava) Added this implementation to avoid any kind of deadlock
                            catch (Exception ex)
                            {
                                LOG.ErrorFormat("Saving database statistics for '{0}' failed: {1}", diskDriveInfo.DriveName, ex);
                                Rollback();
                            }
                            //END SQLdm 9.1 (Ankit Srivastava) Added this implementation to avoid any kind of deadlock
                        }
                    }
                    catch (Exception e)
                    {
                        if (!IsSQLKeyException(e))
                        {
                            LOG.Error("Save disk drive statistics failed", e);
                            throw;
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Save the replication related mertics for this server
        /// </summary>
        /// <param name="refresh"></param>
        protected void SaveReplicationTopology(ScheduledRefresh refresh)
        {
            using (LOG.VerboseCall("UpdateReplicationTopology"))
            {
                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_UpdateReplicationTopology"))
                {
                    command.Transaction = GetTransaction();
                    try
                    {
                        //int databaseID = -1;
                        long pcstartRepl = 0, pcendRepl = 0;
                        Statistics.QueryPerformanceCounter(ref pcstartRepl);

                        foreach (publishedDB published in refresh.Replication.Publisher.PublishedDatabases.Values)
                        {
                            // Only save if we actually read the database
                            if (refresh.Server != null && refresh.MonitoredServer != null)
                            {
                                foreach (subscribedDB subscribed in published.Subscriptions)
                                {
                                    LOG.VerboseFormat("Saving replication topology for publisher '{0}'",
                                                      refresh.ServerName + ".." + published.Instance);

                                    string distributorInstance = refresh.Replication.Publisher.Distributor == null ? "" : refresh.Replication.Publisher.Distributor.Instance;
                                    string distributorName = refresh.Replication.Publisher.Distributor == null ? "" : refresh.Replication.Publisher.Distributor.Dbname;
                                    long distributorMaxLatency = refresh.Replication.Publisher.Distributor == null ? 0 : refresh.Replication.Publisher.Distributor.MaxSubscriptionLatency;

                                    SqlHelper.AssignParameterValues(command.Parameters,
                                            0,
                                            published.Instance.ToUpper(),
                                            published.DBName,
                                            distributorInstance.ToUpper(),
                                            distributorName,
                                            subscribed.Instance.ToUpper(),
                                            subscribed.Dbname,
                                            refresh.TimeStamp,
                                            refresh.Replication.SubscribedDeliveredTransactions,
                                            refresh.Replication.NonSubscribedTransactions,
                                            refresh.Replication.Publisher.ReplicatedTrans,
                                            refresh.Replication.Publisher.ReplicationLatency,
                                            distributorMaxLatency,
                                            null,
                                            null,
                                            null,
                                            null,
                                            null,
                                            null,
                                            subscribed.SubscriptionStatus,
                                            published.PublicationName,
                                            null,
                                            subscribed.Articles
                                        );
                                    command.ExecuteNonQuery();
                                    //databaseID = (int)command.Parameters["@ReturnDatabaseID"].Value;
                                    LOG.VerboseFormat("Replication topology saved.  Publisher is '{0}'",
                                                      published.FullPublisherName);

                                }
                            }
                            else
                                continue;

                        }
                        Statistics.QueryPerformanceCounter(ref pcendRepl);
                        Statistics.ReplicationStatisticWritten(pcstartRepl, pcendRepl, refresh.Replication.Publisher.PublishedDatabases.Values.Count);
                    }
                    catch (Exception e)
                    {
                        if (!IsSQLKeyException(e))
                        {
                            LOG.Error("Save replication topology failed", e);
                        }
                        throw;
                    }
                }
            }
            using (LOG.VerboseCall("SaveReplicationTopologySubscriber"))
            {
                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_UpdateReplicationTopologySubscriber"))
                {
                    command.Transaction = GetTransaction();
                    try
                    {
                        long pcstartReplSubscriber = 0, pcendReplSubscriber = 0;
                        Statistics.QueryPerformanceCounter(ref pcstartReplSubscriber);
                        foreach (subscribedDB subscribed in refresh.Replication.SubscribedDatabases.Values)
                        {
                            // Only save if we actually read the database
                            if (refresh.Server != null && refresh.MonitoredServer != null)
                            {
                                try
                                {
                                    LOG.VerboseFormat("Saving replication topology for subscriber '{0}'",
                                                                              refresh.ServerName + ".." + subscribed.Dbname);
                                    SqlHelper.AssignParameterValues(command.Parameters,
                                            subscribed.PublisherInstance.ToUpper(),
                                            subscribed.Publisherdb,
                                            subscribed.Instance.ToUpper(),
                                            subscribed.Dbname,
                                            refresh.TimeStamp,
                                            subscribed.ReplicationType,
                                            subscribed.SubscriptionType,
                                            subscribed.LastUpdated,
                                            subscribed.LastSyncStatus,
                                            subscribed.LastSyncSummary,
                                            subscribed.LastSyncTime == DateTime.MinValue ? SqlDateTime.MinValue : subscribed.LastSyncTime,
                                            subscribed.PublicationName
                                        );
                                    command.ExecuteNonQuery();
                                    //databaseID = (int)command.Parameters["@ReturnDatabaseID"].Value;
                                    LOG.VerboseFormat("Replication topology saved.  subscribed is '{0}'",
                                                      subscribed.Instance + '.' + subscribed.Dbname);
                                }
                                catch (Exception e)
                                {
                                    if (!IsSQLKeyException(e))
                                    {
                                        LOG.Error("Save replication topology for subscriber failed", e);
                                    }
                                }
                            }
                            else
                                continue;
                        }
                        Statistics.QueryPerformanceCounter(ref pcendReplSubscriber);
                        Statistics.ReplicationSubscriberStatisticWritten(pcstartReplSubscriber, pcendReplSubscriber, refresh.Replication.SubscribedDatabases.Values.Count);

                    }
                    catch (Exception e)
                    {
                        if (!IsSQLKeyException(e))
                        {
                            LOG.Error("Save replication topology failed", e);
                        }
                        throw;
                    }
                }
            }
            using (LOG.VerboseCall("SaveReplicationTopologyDistributor"))
            {
                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_UpdateReplicationTopologyDistributor"))
                {
                    command.Transaction = GetTransaction();
                    try
                    {
                        //int databaseID = -1;

                        if (refresh.Replication.Distributors.Count > 0)
                        {
                            Dictionary<string, cDistributor> distributorDatabases = refresh.Replication.Distributors;

                            long pcstartReplDistributor = 0, pcendReplDistributor = 0;
                            Statistics.QueryPerformanceCounter(ref pcstartReplDistributor);

                            foreach (cDistributor distributed in distributorDatabases.Values)
                            {

                                //cDistributor distributed = refresh.Replication.Distributor;

                                // Only save if we actually read the database
                                if (refresh.Server != null && refresh.MonitoredServer != null)
                                {
                                    LOG.VerboseFormat("Saving replication topology for distributor '{0}'",
                                                      refresh.ServerName + ".." + distributed.Dbname);

                                    foreach (publishedDB published in distributed.DistributedPublications.Values)
                                    {

                                        try
                                        {
                                            if (published.Subscriptions.Count == 0)
                                            {
                                                SqlHelper.AssignParameterValues(command.Parameters,
                                                          published.Instance,
                                                          published.DBName,
                                                          distributed.Instance,
                                                          distributed.Dbname,
                                                          "",
                                                          "",
                                                          refresh.TimeStamp,
                                                          published.SubscribedTrans,
                                                          published.NonSubscribedTrans,
                                                          distributed.MaxSubscriptionLatency,
                                                          published.PublicationName,
                                                          published.PublicationDescription,
                                                          published.PublishedArticles,
                                                          published.ReplicationType
                                                      );

                                                command.ExecuteNonQuery();

                                            }
                                            else
                                            {
                                                foreach (subscribedDB subscribed in published.Subscriptions)
                                                {
                                                    SqlHelper.AssignParameterValues(command.Parameters,
                                                            published.Instance.ToUpper(),
                                                            published.DBName,
                                                            distributed.Instance.ToUpper(),
                                                            distributed.Dbname,
                                                            (subscribed.Instance ?? "").ToUpper(),
                                                            subscribed.Dbname,
                                                            refresh.TimeStamp,
                                                            published.SubscribedTrans,
                                                            published.NonSubscribedTrans,
                                                            distributed.MaxSubscriptionLatency,
                                                            published.PublicationName,
                                                            published.PublicationDescription,
                                                            published.PublishedArticles,
                                                            published.ReplicationType
                                                        );

                                                    command.ExecuteNonQuery();

                                                }

                                                //databaseID = (int)command.Parameters["@ReturnDatabaseID"].Value;
                                                LOG.VerboseFormat("Replication topology saved.  distributor is '{0}'",
                                                                  distributed.Instance + '.' + distributed.Dbname);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            if (!IsSQLKeyException(e))
                                            {
                                                LOG.Error("Save replication topology for distributor failed", e);
                                            }
                                        }
                                    }
                                }
                            }
                            Statistics.QueryPerformanceCounter(ref pcendReplDistributor);
                            Statistics.ReplicationDistributorStatisticWritten(pcstartReplDistributor, pcendReplDistributor, distributorDatabases.Values.Count);
                        }
                    }
                    catch (Exception e)
                    {
                        if (!IsSQLKeyException(e))
                        {
                            LOG.Error("Save replication topology failed", e);
                        }
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Remove mirroring participant entries for servers that are no longer monitored
        /// </summary>
        /// <param name="refresh"></param>
        protected static void GroomReplicationTopology(ScheduledRefresh refresh)
        {
            MonitoredSqlServer thisServer = refresh.MonitoredServer;

            //if the collection failed don't groom
            if (refresh.CollectionFailed) return;

            //if this server is in maintence mode then do not groom
            if (thisServer.MaintenanceModeEnabled) return;

            //Delete from Replication Topology all sessions on this server that are no longer in replication
            try
            {
                if (thisServer != null)
                {
                    long pcstart = 0, pcend = 0;
                    Statistics.QueryPerformanceCounter(ref pcstart);

                    Dictionary<int, ReplicationSession> sessions =
                        Helpers.RepositoryHelper.GetReplicationTopology(ManagementServiceConfiguration.ConnectionString, thisServer.Id);

                    //What roles is this server fulfilling according to the saved topology?
                    bool serverIsPublisher = false;
                    bool serverIsDistributor = false;
                    bool serverIsSubscriber = false;
                    int intDeleteParticipantsMask = 0;

                    foreach (KeyValuePair<int, ReplicationSession> sessionKeyValue in sessions)
                    {
                        serverIsPublisher = false;
                        serverIsDistributor = false;
                        serverIsSubscriber = false;
                        if (sessionKeyValue.Value.PublisherSQLServerID == thisServer.Id) serverIsPublisher = true;
                        if (sessionKeyValue.Value.DistributorSQLServerID == thisServer.Id) serverIsDistributor = true;
                        if (sessionKeyValue.Value.SubscriberSQLServerID == thisServer.Id) serverIsSubscriber = true;

                        ReplicationSession thisSession = sessionKeyValue.Value;
                        //if this was a publisher check that it still is
                        if (serverIsPublisher)
                        {
                            bool blnFoundTheSubscription = false;
                            bool publicationHasGone = false;
                            MonitoredSqlServer publisher =
                                RepositoryHelper.GetMonitoredSqlServer(ManagementServiceConfiguration.ConnectionString,
                                                                       thisSession.PublisherSQLServerID.Value);

                            if ((refresh.Replication.Publisher.InstanceName ?? "").ToUpper() != publisher.InstanceName.ToUpper())
                            {
                                //groom from the repository
                                intDeleteParticipantsMask = intDeleteParticipantsMask | 1;
                                publicationHasGone = true;
                            }
                            else
                            {
                                int intKey = (thisSession.PublisherInstance + thisSession.PublisherDB + thisSession.Publication).ToLower().GetHashCode();

                                //if the publication exists
                                if (refresh.Replication.Publisher.PublishedDatabases.ContainsKey(intKey))
                                {
                                    //dictionaryKey = (published.Instance + published.DBName + published.PublicationName).ToString().ToLower().GetHashCode();
                                    publishedDB published = refresh.Replication.Publisher.PublishedDatabases[intKey];
                                    //if the publisher is still published check that it still has the correct subscriptions

                                    //loop through all subscriptions this publisher believes are subscribing to this publication
                                    for (int i = 0; i < published.Subscriptions.Count; i++)
                                    {
                                        if (published.Subscriptions[i].Instance.ToUpper() == thisSession.SubscriberInstance.ToUpper()
                                            && published.Subscriptions[i].Dbname == thisSession.SubscriberDB)
                                        {
                                            blnFoundTheSubscription = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    //The publisher details are only expected to be on the publisher for transactional replication
                                    if (sessionKeyValue.Value.ReplicationType == (int)ReplicationType.Transaction)
                                    {
                                        //if this session previously was subscribed
                                        if (!sessionKeyValue.Value.SubscriberInstanceAndDB.Equals("N\\A"))
                                        {
                                            //if the publisher has gone then delete it and all subscriptions
                                            publicationHasGone = true;
                                        }
                                    }
                                }
                            }

                            if (publicationHasGone)
                            {
                                if (sessionKeyValue.Value.ReplicationType == (int)ReplicationType.Transaction)
                                {
                                    Helpers.RepositoryHelper.DeleteReplicationSessionFromServer(ManagementServiceConfiguration.ConnectionString,
                                                                                              thisSession.PublisherInstance,
                                                                                              thisSession.PublisherDB,
                                                                                              thisSession.Publication,
                                                                                              thisSession.SubscriberInstance,
                                                                                              thisSession.SubscriberDB);
                                }
                            }
                            else
                            {
                                //we found the publication but no subscription
                                if (!blnFoundTheSubscription)
                                {
                                    //Removing because this is from the publishers perspective. The publisher does not even know about merge replication so
                                    //merge subscriptions should not be deleted because the publisher is unaware of them
                                    if (sessionKeyValue.Value.ReplicationType == (int)ReplicationType.Transaction)
                                    {
                                        //Helpers.RepositoryHelper.DeleteReplicationSessionFromServer(ManagementServiceConfiguration.ConnectionString,
                                        //                                                          sessionKeyValue.Value.PublisherInstance,
                                        //                                                          sessionKeyValue.Value.PublisherDB,
                                        //                                                          sessionKeyValue.Value.Publication,
                                        //                                                          sessionKeyValue.Value.SubscriberInstance,
                                        //                                                          sessionKeyValue.Value.SubscriberDB);
                                    }
                                }
                            }

                        }
                        if (serverIsDistributor)
                        {
                            //if the refresh does not contain this session distributor
                            string distribKey = string.Empty;

                            if (thisSession.DistributorInstance != null && thisSession.DistributorDB != null)
                            {
                                distribKey = thisSession.DistributorInstance.ToUpper() + "." +
                                             thisSession.DistributorDB;

                                if (!(refresh.Replication.Distributors.ContainsKey(distribKey)))
                                {
                                    //groom this distributor
                                    intDeleteParticipantsMask = intDeleteParticipantsMask | 2;

                                    RepositoryHelper.DeleteReplicationSessionFromServer(
                                        ManagementServiceConfiguration.ConnectionString,
                                        thisSession.PublisherInstance,
                                        thisSession.PublisherDB,
                                        thisSession.Publication,
                                        thisSession.SubscriberInstance == "" ? null : thisSession.SubscriberInstance,
                                        thisSession.SubscriberDB == "" ? null : thisSession.SubscriberDB);
                                }
                                else //if the distributor has been found then look through all the publications
                                {
                                    cDistributor distrib = refresh.Replication.Distributors[distribKey];
                                    bool blnFoundPublicationMatch = false;

                                    foreach (KeyValuePair<int, publishedDB> keyValuePair in distrib.DistributedPublications)
                                    {
                                        if (thisSession.Publication == keyValuePair.Value.PublicationName &&
                                            thisSession.PublisherInstance.ToLower() == keyValuePair.Value.Instance.ToLower() &&
                                            thisSession.PublisherDB == keyValuePair.Value.DBName)
                                        {
                                            blnFoundPublicationMatch = true;
                                            bool blnFoundSubscriber = false;
                                            //Matching publication has been found. Now search for subscriptions
                                            publishedDB thisPublication = keyValuePair.Value;

                                            foreach (subscribedDB subscription in thisPublication.Subscriptions)
                                            {
                                                if (subscription.Instance.ToLower() == thisSession.SubscriberInstance.ToLower() &&
                                                    subscription.Dbname == thisSession.SubscriberDB)
                                                {
                                                    //the subscriber is still in the refresh
                                                    blnFoundSubscriber = true;
                                                }
                                            }

                                            //If the subscriber and publisher have gone then no brainer, delete.
                                            //if the subscriber is not found it may be that it is because there is no subscriber.
                                            //Test this by checking if it had a subscriber before. If it has a subscriber and now 
                                            //it doesnt then delete it
                                            if ((!blnFoundSubscriber && !blnFoundPublicationMatch) ||
                                                (!blnFoundSubscriber && !string.IsNullOrEmpty(sessionKeyValue.Value.SubscriberInstance) && !string.IsNullOrEmpty(sessionKeyValue.Value.SubscriberDB)))
                                            {
                                                RepositoryHelper.DeleteReplicationSessionFromServer(ManagementServiceConfiguration.ConnectionString,
                                                  sessionKeyValue.Value.PublisherInstance,
                                                  sessionKeyValue.Value.PublisherDB,
                                                  sessionKeyValue.Value.Publication,
                                                  sessionKeyValue.Value.SubscriberInstance,
                                                  sessionKeyValue.Value.SubscriberDB);
                                            }
                                        }
                                    }
                                    //if the publication has been deleted according to the distributor
                                    if (!blnFoundPublicationMatch)
                                    {
                                        intDeleteParticipantsMask = intDeleteParticipantsMask | 2;
                                        //only groom the partner out if the server is not in maintenance mode
                                        Helpers.RepositoryHelper.DeleteReplicationSessionFromServer(
                                            ManagementServiceConfiguration.ConnectionString,
                                            sessionKeyValue.Value.PublisherInstance,
                                            sessionKeyValue.Value.PublisherDB,
                                            sessionKeyValue.Value.Publication,
                                            null,
                                            null);
                                    }
                                }
                            }
                        }//end of distributor


                        //This bit will be set even if, according to sysdatabases on the server, this is not a subscriber
                        //The result is that no subscriber side grooming can be trusted. We must rely on the distributor
                        if (!serverIsSubscriber) continue;

                        //dictionaryKey = (replSubscriber.PublisherInstance + publishedDatabase + replSubscriber.Instance + subscribedDatabase + replSubscriber.PublicationName).ToLower().GetHashCode();
                        if (!refresh.Replication.SubscribedDatabases.ContainsKey((sessionKeyValue.Value.PublisherInstance + sessionKeyValue.Value.PublisherDB + sessionKeyValue.Value.SubscriberInstance + sessionKeyValue.Value.SubscriberDB + sessionKeyValue.Value.Publication).ToLower().GetHashCode()))
                        {
                            //groom this subscriber
                            intDeleteParticipantsMask = intDeleteParticipantsMask | 4;
                            //if there is a distributor collection
                            if (sessionKeyValue.Value.DistributorInstance != null && sessionKeyValue.Value.DistributorDB != null)
                            {
                                //check to see if the current session is not referenced by a distributor. if not referenced delete it
                                //I am commenting this out 3/10/2009 because sometimes the subscriber bit on the database status is not set
                                //This results in it not showing up in subscribed databaes and then being groomed and added continuously
                                if (!refresh.Replication.Distributors.ContainsKey(sessionKeyValue.Value.DistributorInstance + "." + sessionKeyValue.Value.DistributorDB))
                                {
                                    //Helpers.RepositoryHelper.DeleteReplicationSessionFromServer(ManagementServiceConfiguration.ConnectionString,
                                    //                                                          sessionKeyValue.Value.PublisherInstance,
                                    //                                                          sessionKeyValue.Value.PublisherDB,
                                    //                                                          sessionKeyValue.Value.Publication,
                                    //                                                          null, 
                                    //                                                          null);
                                }
                            }
                            else //if there are no distributors then just delete the session
                            {
                                //Helpers.RepositoryHelper.DeleteReplicationSessionFromServer(ManagementServiceConfiguration.ConnectionString,
                                //                                                          sessionKeyValue.Value.PublisherInstance,
                                //                                                          sessionKeyValue.Value.PublisherDB,
                                //                                                          sessionKeyValue.Value.Publication,
                                //                                                          null,
                                //                                                          null);
                            }
                        }
                    }

                    Statistics.QueryPerformanceCounter(ref pcend);
                    Statistics.GroomMirrorReplTopologyStatisticWritten(pcstart, pcend, sessions.Count);
                    //foreach (KeyValuePair<int, ReplicationSession> sessionKeyValue in sessions)
                    //{
                    //    ////if the refresh contains this server
                    //    //if (refresh.Replication.ContainsKey(participantKeyValue.Key)) continue;

                    //    ////only groom the partner out if the server is not in maintenance mode
                    //    //if (!thisServer.MaintenanceModeEnabled)
                    //    //{
                    //    //    Helpers.RepositoryHelper.DeleteMirroringSessionFromServer(ManagementServiceConfiguration.ConnectionString,
                    //    //                                                              participantKeyValue.Value, participantKeyValue.Key);
                    //    //}
                    //}
                }
            }
            catch (Exception e)
            {
                if (thisServer != null)
                {
                    LOG.Error("Grooming of replication participants failed for server " + thisServer.InstanceName + ".", e);
                }
                else
                {
                    LOG.Error("Grooming of replication participants failed.", e);
                }
                throw;
            }
        }

        /// <summary>
        /// Remove from the topology tables the fields that does not match with the current topology configuration.
        /// </summary>
        /// <param name="refresh"></param>
        protected void GroomAlwaysOnTopology(ScheduledRefresh refresh)
        {
            using (LOG.VerboseCall("GroomAlwaysOnTopology"))
            {
                if (refresh.AvailabilityGroupsSnapshot == null)
                    return;

                try
                {
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_GroomAlwaysOnAvailabilityGroupTopologyXml"))
                    {
                        List<AvailabilityGroup> availabilityGroupItems = refresh.AvailabilityGroupsSnapshot.AvailabilityGroupItems;

                        try
                        {
                            if (refresh.AvailabilityGroupsSnapshot != null && availabilityGroupItems.Count > 0)
                            {
                                command.Transaction = GetTransaction();

                                // Execute procedure.
                                command.ExecuteNonQuery();
                                Commit();
                                LOG.Debug("The availability Groups has been groommed.");
                            }
                        }
                        catch (Exception ex)
                        {
                            if (!IsSQLKeyException(ex))
                            {
                                String errorMessage =
                                    String.Format(
                                        "The grooming for 'availability groups' cannot be processed. AlwaysOn Topology: {0}",
                                        XmlObjectSerializer.SerializeToString(
                                            refresh.AvailabilityGroupsSnapshot.AvailabilityGroupItems));
                                LOG.Error(errorMessage, ex);
                            }
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!IsSQLKeyException(ex))
                    {
                        LOG.Error("Groom AlwaysOn topology has been failed.", ex);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Update AlwaysOn Topology.
        /// </summary>
        /// <param name="refresh"></param>
        protected internal void SaveAlwaysOnTopology(ScheduledRefresh refresh)
        {
            using (LOG.VerboseCall("SaveAlwaysOnTopology"))
            {
                if (refresh.AvailabilityGroupsSnapshot == null)
                    return;

                try
                {
                    lock (alwaysOnLock)
                    {
                        using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_UpdateAlwaysOnAvailabilityGroupTopologyXml"))
                        {
                            List<AvailabilityGroup> availabilityGroupItems = refresh.AvailabilityGroupsSnapshot.AvailabilityGroupItems;

                            if (refresh.AvailabilityGroupsSnapshot != null && availabilityGroupItems.Count > 0)
                            {
                                try
                                {
                                    command.Transaction = GetTransaction();

                                    Stream stream = refresh.AvailabilityGroupsSnapshot.AvailabilityGroupItemsToXml();
                                    stream.Seek(0, SeekOrigin.Begin);
                                    SqlXml xmlObject = new SqlXml(stream);
                                    SqlHelper.AssignParameterValues(command.Parameters, xmlObject);

                                    // Execute procedure.
                                    command.ExecuteNonQuery();
                                    Commit();
                                    LOG.Debug("The availability Groups has been updated.");

                                    if (LOG.IsVerboseEnabled)
                                    {
                                        String debugMessage = String.Format("AlwaysOn Topology: {0}",
                                            XmlObjectSerializer.SerializeToString(refresh.AvailabilityGroupsSnapshot.AvailabilityGroupItems));
                                        LOG.Verbose(debugMessage);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (!IsSQLKeyException(ex))
                                    {
                                        String errorMessage =
                                            String.Format(
                                                "The 'availability groups' cannot be processed. AlwaysOn Topology: {0}",
                                                XmlObjectSerializer.SerializeToString(
                                                    refresh.AvailabilityGroupsSnapshot.AvailabilityGroupItems));
                                        LOG.Error(errorMessage, ex);
                                    }
                                    throw;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!IsSQLKeyException(ex))
                    {
                        LOG.Error("Save AlwaysOn topology has been failed.", ex);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Insert in the statistic information for the AlwaysOn in the database.
        /// </summary>
        /// <param name="refresh">Statistic information.</param>
        protected internal void SaveAlwaysOnStatistics(ScheduledRefresh refresh)
        {
            using (LOG.VerboseCall("SaveAllwaysOnStatistics"))
            {
                if (refresh.AvailabilityGroupsSnapshot == null)
                    return;

                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertAlwaysOnStatistics"))
                {
                    foreach (AlwaysOnStatistics replicaStatistic in refresh.AvailabilityGroupsSnapshot.ReplicaStatistics)
                    {
                        command.Transaction = GetTransaction();

                        try
                        {
                            bool replicaStatisticHasConsistentState =
                                replicaStatistic.ReplicaId != Guid.Empty &&
                                replicaStatistic.GroupId != Guid.Empty &&
                                replicaStatistic.DatabaseId != -1 &&
                                replicaStatistic.GroupDatabaseId != Guid.Empty;

                            if (replicaStatisticHasConsistentState)
                            {
                                LOG.Verbose("Save AlwaysOn statistic for StatisticID: {0}.", replicaStatistic.AlwaysOnStatisticsId);

                                SqlHelper.AssignParameterValues(command.Parameters,
                                    refresh.TimeStamp,
                                    replicaStatistic.ReplicaId,
                                    replicaStatistic.GroupId,
                                    replicaStatistic.GroupDatabaseId,
                                    replicaStatistic.DatabaseId,
                                    replicaStatistic.IsFailoverReady,
                                    replicaStatistic.SynchronizationDatabaseState,
                                    replicaStatistic.SynchronizationDatabaseHealth,
                                    replicaStatistic.DatabaseState,
                                    replicaStatistic.IsSuspended,
                                    replicaStatistic.LastHardenedTime,
                                    replicaStatistic.LogSendQueueSize,
                                    replicaStatistic.LogSendRate,
                                    replicaStatistic.RedoQueueSize,
                                    replicaStatistic.RedoRate,
                                    replicaStatistic.ReplicaRole,
                                    replicaStatistic.OperationalState,
                                    replicaStatistic.ConnectedState,
                                    replicaStatistic.SynchronizationReplicaHealth,
                                    replicaStatistic.LastConnectionErrorNumber,
                                    replicaStatistic.LastConnectedErrorDescription,
                                    replicaStatistic.LastConnectErrorTimestamp,
                                    replicaStatistic.EstimatedDataLossTime,
                                    replicaStatistic.SynchronizationPerformace,
                                    replicaStatistic.FileStreamSendRate,
                                    refresh.Server.TimeDelta.HasValue ? refresh.Server.TimeDelta.Value.TotalSeconds : 0,
                                    replicaStatistic.EstimatedRecoveryTime
                                );

                                // Executing the transact query.
                                command.ExecuteNonQuery();
                                Commit();
                                LOG.Warn("The AlwaysOn statistics has been added.");

                                if (LOG.IsVerboseEnabled)
                                {
                                    String debugMessage = String.Format("AlwaysOn Statistics: {0}",
                                        XmlObjectSerializer.SerializeToString(replicaStatistic));
                                    LOG.Verbose(debugMessage);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (!IsSQLKeyException(ex))
                            {
                                String messageError =
                                    String.Format("AlwaysOn statistics table failed to save. AlwaysOnStatistics: {0}",
                                                  XmlObjectSerializer.SerializeToString(replicaStatistic));
                                LOG.Error(messageError, ex);
                                throw;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save the SQL Server blocked Threshold Configuration details.
        /// </summary>
        /// <param name="refresh">SQL Server Configuration Details.</param>
        protected internal void SaveSqlServerConfiguration(ScheduledRefresh refresh)
        {
            using (LOG.VerboseCall("SaveSqlServerConfiguration"))
            {
                int blockedProcessThresholdOnServer = 0;
                if (refresh.ConfigurationDetails != null && refresh.ConfigurationDetails.Error == null)
                {
                    foreach (DataRow row in refresh.ConfigurationDetails.ConfigurationSettings.Rows)
                    {
                        string rowName = (string)row["Name"];
                        rowName = rowName.ToLower();
                        if (rowName.StartsWith("blocked process threshold"))
                        {
                            blockedProcessThresholdOnServer = (int)row["Config Value"];
                            break;
                        }
                    }

                    if (blockedProcessThresholdOnServer != refresh.MonitoredServer.ActivityMonitorConfiguration.BlockedProcessThreshold)
                    {
                        refresh.MonitoredServer.ActivityMonitorConfiguration.BlockedProcessThreshold = blockedProcessThresholdOnServer;
                        ManagementService.InternalUpdateMonitoredSqlServer(refresh.MonitoredServer.Id,
                                                                            refresh.MonitoredServer.GetConfiguration(),
                                                                            true);
                    }
                }
            }
        }

        /// <summary>
        /// Save the mirroring statistics to the repository.
        /// Saves to mirroring statistics
        /// </summary>
        /// <param name="refresh"></param>
        protected void SaveMirrorStatistics(ScheduledRefresh refresh)
        {
            using (LOG.VerboseCall("SaveMirrorStatistics"))
            {
                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertMirroringStatistics"))
                {
                    command.Transaction = GetTransaction();
                    try
                    {
                        long pcstart = 0, pcend = 0;
                        Statistics.QueryPerformanceCounter(ref pcstart);

                        int databaseID = -1;

                        foreach (MirrorMonitoringDatabaseDetail mirrorstats in refresh.MirroredDatabases.Values)
                        {
                            // Only save if we actually read the database
                            if (refresh.Server != null && refresh.MonitoredServer != null &&
                               mirrorstats.Status != null && !String.IsNullOrEmpty(mirrorstats.Name))
                            {
                                LOG.VerboseFormat("Saving mirroring statistics for '{0}'",
                                                  mirrorstats.ServerName + ".." + mirrorstats.DatabaseName);
                                SqlHelper.AssignParameterValues(command.Parameters,
                                        refresh.MonitoredServer.Id,
                                        mirrorstats.DatabaseName,
                                        mirrorstats.ServerInstance,
                                        mirrorstats.Partner,
                                        mirrorstats.MirroringGuid,
                                        mirrorstats.CurrentMirroringMetrics.Role,
                                        mirrorstats.CurrentMirroringMetrics.MirroringState,
                                        mirrorstats.CurrentMirroringMetrics.WitnessStatus,
                                        mirrorstats.CurrentMirroringMetrics.LogGenerationRate,
                                        mirrorstats.CurrentMirroringMetrics.UnsentLog,
                                        mirrorstats.CurrentMirroringMetrics.SendRate,
                                        mirrorstats.CurrentMirroringMetrics.UnrestoredLog,
                                        mirrorstats.CurrentMirroringMetrics.RecoveryRate,
                                        mirrorstats.CurrentMirroringMetrics.TransactionDelay,
                                        mirrorstats.CurrentMirroringMetrics.TransactionsPerSec,
                                        mirrorstats.CurrentMirroringMetrics.AverageDelay,
                                        mirrorstats.CurrentMirroringMetrics.TimeRecorded,
                                        mirrorstats.CurrentMirroringMetrics.TimeBehind,
                                        mirrorstats.CurrentMirroringMetrics.LocalTime,
                                        mirrorstats.PartnerAddress,
                                        mirrorstats.WitnessAddress,
                                        mirrorstats.SafetyLevel,
                                        refresh.TimeStamp,
                                        null,
                                        null

                                    );
                                command.ExecuteNonQuery();
                                databaseID = (int)command.Parameters["@ReturnDatabaseID"].Value;
                                LOG.VerboseFormat("Database mirroring statistics saved.  Database id for '{0}' is {1}",
                                                  mirrorstats.DatabaseName, databaseID);
                            }
                            else
                                continue;

                        }
                        Statistics.QueryPerformanceCounter(ref pcend);
                        Statistics.MirrorStatisticWritten(pcstart, pcend, refresh.MirroredDatabases.Count);
                    }
                    catch (Exception e)
                    {
                        if (!IsSQLKeyException(e))
                        {
                            LOG.Error("Save database mirroring statistics failed", e);
                            throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Remove mirroring participant entries for servers that are no longer monitored
        /// </summary>
        /// <param name="refresh"></param>
        protected static void GroomMirroringParticipants(ScheduledRefresh refresh)
        {
            MonitoredSqlServer thisServer = refresh.MonitoredServer;

            //Delete from MirroringParticipants all sessions on this server that are no longer in mirroreddatabases
            try
            {
                if (thisServer != null)
                {
                    long pcstart = 0, pcend = 0;
                    Statistics.QueryPerformanceCounter(ref pcstart);

                    Dictionary<Guid, int> participants =
                        Helpers.RepositoryHelper.GetMirroringParticipantsForServer(ManagementServiceConfiguration.ConnectionString, thisServer.Id);

                    foreach (KeyValuePair<Guid, int> participantKeyValue in participants)
                    {
                        //if the refresh no longer contains this guid
                        if (refresh.MirroredDatabases.ContainsKey(participantKeyValue.Key)) continue;

                        //only groom the partner out if the server is not in maintenance mode
                        if (!thisServer.MaintenanceModeEnabled)
                        {
                            Helpers.RepositoryHelper.DeleteMirroringSessionFromServer(ManagementServiceConfiguration.ConnectionString,
                                                                                      participantKeyValue.Value, participantKeyValue.Key);
                        }
                    }
                    Statistics.QueryPerformanceCounter(ref pcend);
                    Statistics.GroomMirrorPaticipantsStatisticWritten(pcstart, pcend, participants.Count);
                }
            }
            catch (Exception ex)
            {
                if (thisServer != null)
                {
                    LOG.Error("Deleting of mirroring participants failed for server " + thisServer.InstanceName + ". " + ex.Message);
                }
                else
                {
                    LOG.Error("Deleting of mirroring participants failed. " + ex.Message);
                }
                throw;
            }
        }

        /// <summary>
        /// Groom preferred configuration entries that don't have corresponding entries in the refresh
        /// </summary>
        /// <param name="refresh"></param>
        protected static void GroomMirroringPreferredConfig(ScheduledRefresh refresh)
        {

            //Delete sessions from MirroringPreferredConfig for which the mirroring sessions have been stopped
            try
            {
                long pcstart = 0, pcend = 0;
                Statistics.QueryPerformanceCounter(ref pcstart);

                foreach (KeyValuePair<Guid, MirrorMonitoringDatabaseDetail> detailkeyvalue in refresh.noLongerMirroredDatabases)
                {
                    Helpers.RepositoryHelper.DeleteMirroringPreferredConfig(ManagementServiceConfiguration.ConnectionString,
                        detailkeyvalue.Key);
                }
                Statistics.QueryPerformanceCounter(ref pcend);
                Statistics.GroomMirrorPrefCfgStatisticWritten(pcstart, pcend, refresh.noLongerMirroredDatabases.Count);

            }
            catch (Exception)
            {
                LOG.Error("Deleting of orphaned mirroring configurations failed.");
                throw;
            }
        }

        protected void SaveDiskDrives(DatabaseSizeSnapshot refresh)
        {
            SaveDiskDrives(refresh.Id, refresh.TimeStamp, refresh.DiskDrives);
        }

        protected void SaveDiskDrives(ScheduledRefresh refresh)
        {
            SaveDiskDrives(refresh.Id, refresh.TimeStamp, refresh.DiskDrives);
        }

        protected void SaveDiskDrives(int Id, DateTime? TimeStamp, Dictionary<string, DiskDrive> DiskDrives)
        {
            using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertDiskDrive"))
            {
                try
                {
                    if (DiskDrives == null || DiskDrives.Count == 0)
                        return;

                    long pcstart = 0, pcend = 0;
                    Statistics.QueryPerformanceCounter(ref pcstart);

                    command.Transaction = GetTransaction();
                    foreach (DiskDrive d in DiskDrives.Values)
                    {
                        SqlHelper.AssignParameterValues(command.Parameters,
                                                        Id,
                                                        d.DriveLetter,
                                                        TimeStamp,
                                                        SafeKilobytes<decimal>(d.UnusedSize),
                                                        SafeKilobytes<decimal>(d.TotalSize),
                                                        d.DiskIdlePercent,
                                                        d.AverageDiskQueueLength,
                                                        d.AvgDiskSecPerRead.HasValue
                                                            ? new double?(d.AvgDiskSecPerRead.Value.TotalMilliseconds)
                                                            : null,
                                                        d.AvgDiskSecPerTransfer.HasValue
                                                            ? new double?(d.AvgDiskSecPerTransfer.Value.TotalMilliseconds)
                                                            : null,
                                                        d.AvgDiskSecPerWrite.HasValue
                                                            ? new double?(d.AvgDiskSecPerWrite.Value.TotalMilliseconds)
                                                            : null,
                                                        d.DiskReadsPerSec,
                                                        d.DiskTransfersPerSec,
                                                        d.DiskWritesPerSec,
                                                        null //  @ReturnMessage
                            );

                        command.ExecuteNonQuery();
                    }
                    Commit();

                    Statistics.QueryPerformanceCounter(ref pcend);
                    Statistics.DiskStatisticWritten(pcstart, pcend, DiskDrives.Count);
                }
                catch (Exception e)
                {
                    if (!IsSQLKeyException(e))
                    {
                        LOG.Error("Save disk drives failed", e);
                        throw;
                    }
                }
            }
        }


        protected void SaveDeadlocks(ScheduledRefresh refresh, Dictionary<string, int> DatabaseNames,
                                                  ref Dictionary<long, long> cachedSqlCopy,
                                                  ref Dictionary<long, long> cachedSigCopy,
                                                  ref Dictionary<long, long> cachedHostNameCopy,
                                                  ref Dictionary<long, long> cachedApplicationNameCopy,
                                                  ref Dictionary<long, long> cachedLoginNameCopy,
                                                  TimeSpan utcOffset)
        {
            try
            {
                if (refresh.Deadlocks == null || refresh.Deadlocks.Count == 0)
                    return;

                if (DatabaseNames == null || DatabaseNames.Count == 0)
                    DatabaseNames = new Dictionary<string, int>();

                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertDeadlock"))
                {
                    long pcstart = 0, pcend = 0;
                    Statistics.QueryPerformanceCounter(ref pcstart);

                    foreach (DeadlockInfo d in refresh.Deadlocks)
                    {
                        if (d == null)
                            continue;

                        command.Transaction = GetTransaction();


                        SqlHelper.AssignParameterValues(command.Parameters,
                                                        refresh.MonitoredServer.Id,
                                                        refresh.TimeStamp,
                                                        d.GetId(),
                                                        d.XdlString,
                                                        null //  @ReturnMessage
                            );

                        command.ExecuteNonQuery();

                        Commit();

                        try
                        {
                            using (SqlCommand processCommand = SqlHelper.CreateCommand(connection, "p_InsertDeadlockProcess"))
                            {
                                List<object[]> processes = d.GetInsertData();
                                foreach (object[] o in processes)
                                {
                                    if (o == null)
                                        continue;

                                    string databaseName = o[0].ToString();
                                    int spid = Convert.ToInt32(o[1]);
                                    string appName = o[2].ToString();
                                    string loginName = o[3].ToString();
                                    string host = o[4].ToString();
                                    string lastCommand = o[5].ToString();

                                    processCommand.Transaction = GetTransaction();


                                    string StatementText = lastCommand != null && lastCommand.Length > 0
                                                               ? lastCommand.Trim()
                                                               : null;

                                    if (StatementText == null)
                                        continue;

                                    string SQLHash = SqlParsingHelper.GetStatementHash(StatementText);
                                    long SQLHashNumeric = SQLHash.GetHashCode();
                                    long SQLId = -1;
                                    cachedSqlCopy.TryGetValue(SQLHashNumeric, out SQLId);

                                    string signature = SqlParsingHelper.GetReadableSignature(StatementText);
                                    string signatureHash = SqlParsingHelper.GetSignatureHash(StatementText);
                                    long SQLSigId = -1;
                                    long signatureHashNumeric = signatureHash.GetHashCode();
                                    cachedSigCopy.TryGetValue(signatureHashNumeric, out SQLSigId);

                                    long applicationNameId = -1;
                                    long applicationNumeric = appName.GetHashCode();
                                    cachedApplicationNameCopy.TryGetValue(applicationNumeric, out applicationNameId);

                                    long hostNameId = -1;
                                    long hostNameNumeric = host.GetHashCode();
                                    cachedHostNameCopy.TryGetValue(hostNameNumeric, out hostNameId);

                                    long loginNameId = -1;
                                    long loginNameNumeric = loginName.GetHashCode();
                                    cachedLoginNameCopy.TryGetValue(loginNameNumeric, out loginNameId);

                                    int databaseNameId =
                                        DatabaseNames.ContainsKey(databaseName)
                                            ? (int)DatabaseNames[databaseName]
                                            : 0;

                                    SqlHelper.AssignParameterValues(processCommand.Parameters,
                                                                    refresh.MonitoredServer.Id, //@SQLServerID int,
                                                                    refresh.TimeStamp,//@UTCCollectionDateTime datetime,
                                                                    d.StartTime, //@UTCOccurrenceDateTime datetime,
                                                                    (d.StartTime.HasValue) ? d.StartTime.Value.Add(utcOffset) : d.StartTime, //@StatementLocalStartTime
                                                                    d.GetId(), //@DeadlockID uniqueidentifier,   
                                                                    applicationNameId > 0 ? null : appName,//@ApplicationName nvarchar(256),
                                                                    applicationNameId, //@ApplicationNameID int output,
                                                                    databaseNameId > 0 ? null : databaseName,//@DatabaseName nvarchar(255),
                                                                    databaseNameId, //@DatabaseID int output,
                                                                    hostNameId > 0 ? null : host,//@HostName nvarchar(256),
                                                                    hostNameId, //@HostNameID int output,
                                                                    loginNameId > 0 ? null : loginName,//@LoginName nvarchar(256),                                           
                                                                    loginNameId, //@LoginNameID int output,
                                                                    spid, //@SessionID smallint,
                                                                    SQLId > 0 ? null : StatementText,//@SQLStatement varchar(4000),
                                                                    SQLHash, //@SQLStatementHash nvarchar(30),
                                                                    SQLId, //@SQLStatementID int output,
                                                                    SQLSigId > 0 ? null : signature,//@SQLSignature nvarchar(4000),
                                                                    signatureHash, //@SQLSignatureHash nvarchar(30),
                                                                    SQLSigId //@SQLSignatureID int output,
                                        );

                                    try
                                    {
                                        processCommand.ExecuteNonQuery();
                                    }
                                    catch (SqlException)
                                    {
                                        // Try again in case it's just bad caching
                                        Rollback();
                                        processCommand.Transaction = GetTransaction();
                                        SqlHelper.AssignParameterValues(processCommand.Parameters,
                                                                        refresh.MonitoredServer.Id, //@SQLServerID int,
                                                                        refresh.TimeStamp,
                                                                        //@UTCCollectionDateTime datetime,
                                                                        d.StartTime, //@UTCOccurrenceDateTime datetime,
                                                                        (d.StartTime.HasValue) ? d.StartTime.Value.Add(utcOffset) : d.StartTime,
                                                                        d.GetId(), //@DeadlockID uniqueidentifier,   
                                                                        appName, //@ApplicationName nvarchar(256),
                                                                        0, //@ApplicationNameID int output,
                                                                        databaseName, //@DatabaseName nvarchar(255),
                                                                        0, //@DatabaseID int output,
                                                                        host, //@HostName nvarchar(256),
                                                                        0, //@HostNameID int output,
                                                                        loginName,
                                                                        //@LoginName nvarchar(256),                                           
                                                                        0, //@LoginNameID int output,
                                                                        spid, //@SessionID smallint,
                                                                        StatementText, //@SQLStatement varchar(4000),
                                                                        SQLHash, //@SQLStatementHash nvarchar(30),
                                                                        0, //@SQLStatementID int output,
                                                                        signature, //@SQLSignature nvarchar(4000),
                                                                        signatureHash, //@SQLSignatureHash nvarchar(30),
                                                                        0 //@SQLSignatureID int output,
                                            );

                                        processCommand.ExecuteNonQuery();
                                        cachedSqlCopy.Clear();
                                        cachedApplicationNameCopy.Clear();
                                        cachedHostNameCopy.Clear();
                                        cachedLoginNameCopy.Clear();
                                        ExpireQueryMonitorCaches();
                                    }


                                    if ((Int64.TryParse(processCommand.Parameters["@SQLStatementID"].Value.ToString(), out SQLId)) &&
                                        (!cachedSqlCopy.ContainsKey(SQLHashNumeric)))
                                    {
                                        cachedSqlCopy.Add(SQLHashNumeric, SQLId);
                                    }

                                    if ((Int64.TryParse(processCommand.Parameters["@ApplicationNameID"].Value.ToString(), out applicationNameId)) &&
                                       (!cachedApplicationNameCopy.ContainsKey(applicationNumeric)))
                                    {
                                        cachedApplicationNameCopy.Add(applicationNumeric, applicationNameId);
                                    }

                                    if ((Int64.TryParse(processCommand.Parameters["@HostNameID"].Value.ToString(), out hostNameId)) &&
                                      (!cachedHostNameCopy.ContainsKey(hostNameNumeric)))
                                    {
                                        cachedHostNameCopy.Add(hostNameNumeric, hostNameId);
                                    }

                                    if ((Int64.TryParse(processCommand.Parameters["@LoginNameID"].Value.ToString(), out loginNameId)) &&
                                     (!cachedLoginNameCopy.ContainsKey(loginNameNumeric)))
                                    {
                                        cachedLoginNameCopy.Add(loginNameNumeric, loginNameId);
                                    }

                                    Commit();

                                }
                            }
                        }
                        catch (Exception e)
                        {
                            if (!IsSQLKeyException(e))
                            {
                                LOG.Error("Save deadlock processes failed.", e);
                            }
                            Rollback();
                        }
                    }

                    Statistics.QueryPerformanceCounter(ref pcend);
                    Statistics.DeadlocksStatisticWritten(pcstart, pcend, refresh.Deadlocks.Count);

                }
            }
            catch (Exception e)
            {
                if (!IsSQLKeyException(e))
                {
                    LOG.Error("Save deadlocks failed", e);
                    throw;
                }
            }
        }




        private Nullable<RETURNTYPE> SafeKilobytes<RETURNTYPE>(FileSize fileSize) where RETURNTYPE : struct
        {
            Nullable<RETURNTYPE> result = null;
            if (fileSize != null && fileSize.Kilobytes.HasValue)
            {
                decimal kb = Math.Floor(fileSize.Kilobytes.Value);
                result = (RETURNTYPE)Convert.ChangeType(kb, typeof(RETURNTYPE));
            }
            return result;
        }

        protected void SaveTableGrowthStatistics(DatabaseStatistics dbstats, int dbID, int serverID,
                                                 DateTime? serverTime, TimeSpan? timeDelta)
        {
            using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertTableGrowth"))
            {
                try
                {
                    double? timeDeltaInSeconds;
                    if (timeDelta.HasValue)
                        timeDeltaInSeconds = timeDelta.Value.TotalSeconds;
                    else
                        timeDeltaInSeconds = null;

                    foreach (TableSize tableSize in dbstats.TableSizes.Values)
                    {
                        command.Transaction = GetTransaction();
                        SqlHelper.AssignParameterValues(command.Parameters,
                                                        serverID,
                                                        serverTime,
                                                        dbstats.Name,
                                                        DBNull.Value, // "
                                                        dbID,
                                                        tableSize.Name,
                                                        tableSize.Schema,
                                                        tableSize.IsSystemTable,
                                                        tableSize.RowCount,
                                                        SafeKilobytes<double>(tableSize.DataSize),
                                                        SafeKilobytes<double>(tableSize.ImageTextSize),
                                                        SafeKilobytes<double>(tableSize.IndexSize),
                                                        timeDeltaInSeconds,
                                                        null //  @ReturnMessage
                            );

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("Save table growth failed", e);
                    throw;
                }
            }
        }

        protected void SaveReorganization(DatabaseStatistics dbstats, int dbID, int serverID, DateTime? serverTime,
                                          TimeSpan? timeDelta)
        {
            using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertTableReorganization"))
            {
                try
                {
                    double? timeDeltaInSeconds;
                    if (timeDelta.HasValue)
                        timeDeltaInSeconds = timeDelta.Value.TotalSeconds;
                    else
                        timeDeltaInSeconds = null;

                    foreach (TableReorganization reorg in dbstats.TableReorganizations.Values)
                    {
                        command.Transaction = GetTransaction();
                        SqlHelper.AssignParameterValues(command.Parameters,
                                                        serverID,
                                                        serverTime,
                                                        dbstats.Name,
                                                        DBNull.Value, // "
                                                        dbID,
                                                        reorg.Name,
                                                        reorg.Schema,
                                                        reorg.IsSystemTable,
                                                        reorg.ScanDensity,
                                                        reorg.LogicalFragmentation,
                                                        timeDeltaInSeconds,
                                                        null //  @ReturnMessage
                            );

                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("Save table reorganization failed", e);
                    throw;
                }
            }
        }

        protected void SaveQueryMonitorStatements(ScheduledRefresh refresh, Dictionary<string, int> DatabaseNames,
                                                  ref Dictionary<long, long> cachedSqlCopy,
                                                  ref Dictionary<long, long> cachedSigCopy,
                                                  ref Dictionary<long, long> cachedHostNameCopy,
                                                  ref Dictionary<long, long> cachedApplicationNameCopy,
                                                  ref Dictionary<long, long> cachedLoginNameCopy,
                                                  TimeSpan utcOffset)
        {
            using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertQueryMonitorStatement"))
            {
                try
                {


                    if (DatabaseNames == null || DatabaseNames.Count == 0)
                        DatabaseNames = new Dictionary<string, int>();

                    long pcstart = 0, pcend = 0;
                    Statistics.QueryPerformanceCounter(ref pcstart);

                    for (var index = 0; index < refresh.QueryMonitorStatements.Count; index++)
                    {
                        QueryMonitorStatement stmt = refresh.QueryMonitorStatements[index];
                        command.Transaction = this.GetTransaction();
                        try
                        {
                            string LoginName = stmt.SqlUser;
                            string DecompressedStatementText = stmt.SqlText; //Decompress the string
                            if (!String.IsNullOrEmpty(DecompressedStatementText))
                                DecompressedStatementText = DecompressedStatementText.Trim();

                            string SQLHash = SqlParsingHelper.GetStatementHash(DecompressedStatementText);
                            long SQLHashNumeric = SQLHash.GetHashCode();
                            long SQLId = -1;
                            cachedSqlCopy.TryGetValue(SQLHashNumeric, out SQLId);

                            string signatureHash = stmt.SignatureHash;
                            string signature = null;
                            byte[] signatureBytes = null;
                            long SQLSigId = -1;
                            if (!refresh.QueryMonitorSignatures.TryGetValue(stmt.SignatureHash, out signatureBytes))
                            {
                                signature = SqlParsingHelper.GetReadableSignature(DecompressedStatementText);
                                signatureHash = SqlParsingHelper.GetSignatureHash(DecompressedStatementText);
                            }
                            else
                            {
                                signature = Serialized<string>.DeserializeCompressed<string>(signatureBytes);
                            }

                            long signatureHashNumeric = signatureHash.GetHashCode();
                            cachedSigCopy.TryGetValue(signatureHashNumeric, out SQLSigId);

                            long applicationNameId = -1;
                            long applicationNumeric = !String.IsNullOrEmpty(stmt.AppName)
                                                          ? stmt.AppName.GetHashCode()
                                                          : -1;
                            cachedApplicationNameCopy.TryGetValue(applicationNumeric, out applicationNameId);

                            long hostNameId = -1;
                            long hostNameNumeric = !String.IsNullOrEmpty(stmt.Client) ? stmt.Client.GetHashCode() : -1;
                            cachedHostNameCopy.TryGetValue(hostNameNumeric, out hostNameId);

                            long loginNameId = -1;
                            long loginNameNumeric = !String.IsNullOrEmpty(stmt.SqlUser)
                                                        ? stmt.SqlUser.GetHashCode()
                                                        : -1;
                            cachedLoginNameCopy.TryGetValue(loginNameNumeric, out loginNameId);

                            int databaseNameId =
                                DatabaseNames.ContainsKey(stmt.Database)
                                    ? (int)DatabaseNames[stmt.Database]
                                    : 0;




                            SqlHelper.AssignParameterValues(command.Parameters,
                                                            refresh.MonitoredServer.Id, //@SQLServerID int,
                                                            refresh.TimeStamp,//@UTCCollectionDateTime datetime,
                                                            applicationNameId > 0 ? null : stmt.AppName,//@ApplicationName nvarchar(256),
                                                            applicationNameId, //@ApplicationNameID int output,
                                                            databaseNameId > 0 ? null : stmt.Database, //@DatabaseName nvarchar(255),
                                                            databaseNameId, //@DatabaseID int output,
                                                            hostNameId > 0 ? null : stmt.Client, //@HostName nvarchar(256),
                                                            hostNameId, //@HostNameID int output,
                                                            loginNameId > 0 ? null : LoginName,//@LoginName nvarchar(256),                                           
                                                            loginNameId, //@LoginNameID int output,
                                                            stmt.Spid, //@SessionID smallint,
                                                            (int)stmt.StatementType, //@StatementType int,
                                                            SQLId > 0 ? null : DecompressedStatementText,//@SQLStatement varchar(4000),
                                                            SQLHash, //@SQLStatementHash nvarchar(30),
                                                            SQLId, //@SQLStatementID int output,
                                                            SQLSigId > 0 ? null : signature, //@SQLSignature nvarchar(4000),
                                                            signatureHash, //@SQLSignatureHash nvarchar(30),
                                                            SQLSigId, //@SQLSignatureID int output,
                                                            stmt.CompletionTime.Value.Subtract(stmt.Duration.Value),//@StatementUTCStartTime datetime,
                                                            stmt.CompletionTime.Value.Subtract(stmt.Duration.Value).Add(utcOffset), //@StatementLocalStartTime
                                                            stmt.Duration.Value.TotalMilliseconds,//@DurationMilliseconds bigint,
                                                            stmt.CpuTime != null ? stmt.CpuTime.Value.TotalMilliseconds : 0,//@CPUMilliseconds bigint,
                                                            stmt.Reads, //@Reads bigint,
                                                            stmt.Writes,//@Writes bigint
                                                                        // START SQLdm 10.0 (Tarun Sapra) --If query plan is null then insert estimated query plan
                                                            !string.IsNullOrEmpty(stmt.QueryPlan) ? ObjectHelper.CompressString(stmt.QueryPlan) : String.Empty,//@QueryPlan nvarchar(max) optional                                                            
                                                            stmt.IsActualPlan
                                                            // END SQLdm 10.0 (Tarun Sapra) --If query plan is null then insert estimated query plan
                                                            );
                            //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session added the Query plan parameter

                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (SqlException)
                            {
                                // Try again in case it's just bad caching

                                Rollback();
                                command.Transaction = GetTransaction();
                                SqlHelper.AssignParameterValues(command.Parameters,
                                                                refresh.MonitoredServer.Id, //@SQLServerID int,
                                                                refresh.TimeStamp,
                                                                //@UTCCollectionDateTime datetime,
                                                                stmt.AppName, //@ApplicationName nvarchar(256),
                                                                0, //@ApplicationNameID int output,
                                                                stmt.Database, //@DatabaseName nvarchar(255),
                                                                0, //@DatabaseID int output,
                                                                stmt.Client, //@HostName nvarchar(256),
                                                                0, //@HostNameID int output,
                                                                LoginName,
                                                                //@LoginName nvarchar(256),                                           
                                                                0, //@LoginNameID int output,
                                                                stmt.Spid, //@SessionID smallint,
                                                                (int)stmt.StatementType, //@StatementType int,
                                                                DecompressedStatementText, //@SQLStatement varchar(4000),
                                                                SQLHash, //@SQLStatementHash nvarchar(30),
                                                                0, //@SQLStatementID int output,
                                                                signature, //@SQLSignature nvarchar(4000),
                                                                signatureHash, //@SQLSignatureHash nvarchar(30),
                                                                0, //@SQLSignatureID int output,
                                                                stmt.CompletionTime.Value.Subtract(stmt.Duration.Value),//@StatementUTCStartTime datetime,
                                                                stmt.CompletionTime.Value.Subtract(stmt.Duration.Value).Add(utcOffset), //@StatementLocalStartTime
                                                                stmt.Duration.Value.TotalMilliseconds,
                                                                //@DurationMilliseconds bigint,
                                                                stmt.CpuTime.Value.TotalMilliseconds,
                                                                //@CPUMilliseconds bigint,
                                                                stmt.Reads, //@Reads bigint,
                                                                stmt.Writes, //@Writes bigint
                                                                             // START SQLdm 10.0 (Tarun Sapra) --If query plan is null then insert estimated query plan
                                                                !string.IsNullOrEmpty(stmt.QueryPlan) ? ObjectHelper.CompressString(stmt.QueryPlan) : String.Empty,//@QueryPlan nvarchar(max) optional                                                            
                                                                stmt.IsActualPlan
                                    // END SQLdm 10.0 (Tarun Sapra) --If query plan is null then insert estimated query plan
                                    );
                                //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session added the Query plan parameter
                                command.ExecuteNonQuery();
                                cachedSqlCopy.Clear();
                                cachedApplicationNameCopy.Clear();
                                cachedHostNameCopy.Clear();
                                cachedLoginNameCopy.Clear();
                                ExpireQueryMonitorCaches();
                            }

                            if ((Int64.TryParse(command.Parameters["@SQLStatementID"].Value.ToString(), out SQLId))
                                && (!cachedSqlCopy.ContainsKey(SQLHashNumeric)))
                            {
                                cachedSqlCopy.Add(SQLHashNumeric, SQLId);
                            }

                            if ((Int64.TryParse(
                                        command.Parameters["@ApplicationNameID"].Value.ToString(),
                                        out applicationNameId))
                                && (!cachedApplicationNameCopy.ContainsKey(applicationNumeric)))
                            {
                                cachedApplicationNameCopy.Add(applicationNumeric, applicationNameId);
                            }

                            if ((Int64.TryParse(command.Parameters["@HostNameID"].Value.ToString(), out hostNameId))
                                && (!cachedHostNameCopy.ContainsKey(hostNameNumeric)))
                            {
                                cachedHostNameCopy.Add(hostNameNumeric, hostNameId);
                            }

                            if ((Int64.TryParse(command.Parameters["@LoginNameID"].Value.ToString(), out loginNameId))
                                && (!cachedLoginNameCopy.ContainsKey(loginNameNumeric)))
                            {
                                cachedLoginNameCopy.Add(loginNameNumeric, loginNameId);
                            }

                            this.Commit();
                        }
                        catch (Exception e)
                        {
                            if (!IsSQLKeyException(e))
                            {
                                LOG.Error("Save query monitor statements failed", e);
                            }
                            this.Rollback();
                        }
                    }

                    Statistics.QueryPerformanceCounter(ref pcend);
                    Statistics.QMStatementsStatisticWritten(pcstart, pcend, refresh.QueryMonitorStatements.Count);

                }
                catch (Exception e)
                {
                    if (!IsSQLKeyException(e))
                    {
                        LOG.Error("Save query monitor statements failed", e);
                        throw;
                    }
                }
            }
        }

        protected void SaveOSStatistics(ScheduledRefresh refresh)
        {
            if (refresh.TimeStamp == null)
                return;

            using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertOSStatistics"))
            {
                try
                {
                    long pcstart = 0, pcend = 0;
                    Statistics.QueryPerformanceCounter(ref pcstart);

                    command.Transaction = GetTransaction();
                    SqlHelper.AssignParameterValues(command.Parameters, // procedure p_InsertServerStatistics
                                                    refresh.MonitoredServer.Id,
                                                    //	@SQLServerID int,                       
                                                    refresh.TimeStamp,
                                                   // 	@UTCCollectionDateTime datetime,        
                                                    refresh.Server.OSMetricsStatistics.TotalPhysicalMemory.Kilobytes,
                                                    refresh.Server.OSMetricsStatistics.AvailableBytes.Kilobytes,
                                                    refresh.Server.OSMetricsStatistics.PagesPersec ?? refresh.Server.PagesPersec,
                                                    refresh.Server.OSMetricsStatistics.PercentProcessorTime,
                                                    refresh.Server.OSMetricsStatistics.PercentPrivilegedTime,
                                                    refresh.Server.OSMetricsStatistics.PercentUserTime,
                                                    refresh.Server.OSMetricsStatistics.ProcessorQueueLength,
                                                    refresh.Server.OSMetricsStatistics.PercentDiskTime,
                                                    refresh.Server.OSMetricsStatistics.AvgDiskQueueLength,
                                                    null //  @ReturnMessage
                        );

                    command.ExecuteNonQuery();

                    Statistics.QueryPerformanceCounter(ref pcend);
                    Statistics.OSStatisticWritten(pcstart, pcend);
                }
                catch (Exception e)
                {
                    if (!IsSQLKeyException(e))
                    {
                        LOG.Error("Save OS statistics failed", e);
                        throw;
                    }
                }
            }
        }


        protected void SaveVMStatistics(ScheduledRefresh refresh)
        {
            if (refresh.TimeStamp == null)
                return;

            // Save VM COnfig Data
            using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertVMConfigData"))
            {
                try
                {
                    if (refresh.Server.VMConfig.HostName != null && refresh.Server.VMConfig.Name != null && refresh.Server.VMConfig.HostName.Length > 0 && refresh.Server.VMConfig.Name.Length > 0)
                    {
                        long pcstartVMCfg = 0, pcendVMCfg = 0;
                        Statistics.QueryPerformanceCounter(ref pcstartVMCfg);

                        command.Transaction = GetTransaction();
                        SqlHelper.AssignParameterValues(command.Parameters,
                                                        refresh.MonitoredServer.Id,
                                                        refresh.TimeStamp,
                                                        refresh.Server.VMConfig.InstanceUUID,
                                                        refresh.Server.VMConfig.Name,
                                                        (int)refresh.Server.VMConfig.Status,
                                                        refresh.Server.VMConfig.HostName,
                                                        refresh.Server.VMConfig.BootTime <= (DateTime)SqlDateTime.MinValue
                                                            ? Convert.ToDateTime("01/01/1900")
                                                            : refresh.Server.VMConfig.BootTime,
                                                        refresh.Server.VMConfig.NumCPUs,
                                                        refresh.Server.VMConfig.CPULimit,
                                                        refresh.Server.VMConfig.CPUReserve,
                                                        (refresh.Server.VMConfig.MemSize != null && refresh.Server.VMConfig.MemSize.Kilobytes.HasValue)
                                                            ? Math.Round(refresh.Server.VMConfig.MemSize.Kilobytes.Value, 0)
                                                            : (object)null,
                                                        (refresh.Server.VMConfig.MemLimit != null && refresh.Server.VMConfig.MemLimit.Kilobytes.HasValue)
                                                            ? Math.Round(refresh.Server.VMConfig.MemLimit.Kilobytes.Value, 0)
                                                            : (object)null,
                                                        (refresh.Server.VMConfig.MemReserve != null && refresh.Server.VMConfig.MemReserve.Kilobytes.HasValue)
                                                            ? Math.Round(refresh.Server.VMConfig.MemReserve.Kilobytes.Value, 0)
                                                            : (object)null,
                                                        null
                                                        );

                        command.ExecuteNonQuery();
                        Statistics.QueryPerformanceCounter(ref pcendVMCfg);
                        Statistics.VMConfigStatisticWritten(pcstartVMCfg, pcendVMCfg);
                    }
                }
                catch (Exception e)
                {
                    if (!IsSQLKeyException(e))
                    {
                        LOG.ErrorFormat("Save VM Config Data Failed [{0}]", refresh.ServerName, e);
                    }
                    throw;
                }
            }

            // Save ESX Config Data
            using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertESXConfigData"))
            {
                try
                {
                    if (refresh.Server.VMConfig.ESXHost.Name != null && refresh.Server.VMConfig.ESXHost.DomainName != null && refresh.Server.VMConfig.ESXHost.Name.Length > 0 && refresh.Server.VMConfig.ESXHost.DomainName.Length > 0)
                    {
                        long pcstartESXCfg = 0, pcendESXCfg = 0;
                        Statistics.QueryPerformanceCounter(ref pcstartESXCfg);

                        command.Transaction = GetTransaction();
                        SqlHelper.AssignParameterValues(command.Parameters,
                                                        refresh.MonitoredServer.Id,
                                                        refresh.TimeStamp,
                                                        refresh.Server.VMConfig.InstanceUUID,
                                                        refresh.Server.VMConfig.ESXHost.Name,
                                                        refresh.Server.VMConfig.ESXHost.DomainName,
                                                        (int)refresh.Server.VMConfig.ESXHost.Status,
                                                        refresh.Server.VMConfig.ESXHost.BootTime <= (DateTime)SqlDateTime.MinValue
                                                            ? Convert.ToDateTime("01/01/1900")
                                                            : refresh.Server.VMConfig.ESXHost.BootTime,
                                                        refresh.Server.VMConfig.ESXHost.CPUMHz,
                                                        refresh.Server.VMConfig.ESXHost.NumCPUCores,
                                                        refresh.Server.VMConfig.ESXHost.NumCPUPkgs,
                                                        refresh.Server.VMConfig.ESXHost.NumCPUThreads,
                                                        refresh.Server.VMConfig.ESXHost.NumNICs,
                                                        (refresh.Server.VMConfig.ESXHost.MemSize != null && refresh.Server.VMConfig.ESXHost.MemSize.Kilobytes.HasValue)
                                                            ? Math.Round(refresh.Server.VMConfig.ESXHost.MemSize.Kilobytes.Value, 0)
                                                            : (object)null,
                                                        null
                                                        );

                        command.ExecuteNonQuery();

                        Statistics.QueryPerformanceCounter(ref pcendESXCfg);
                        Statistics.ESXConfigStatisticWritten(pcstartESXCfg, pcendESXCfg);
                    }
                }
                catch (Exception e)
                {
                    if (!IsSQLKeyException(e))
                    {
                        LOG.Error("Save ESX Config Data Failed", e);
                    }
                    throw;
                }
            }

            // Save VM Perf Stats Data
            using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertVMStatistics"))
            {
                try
                {
                    if (refresh.Server.VMConfig.HostName != null && refresh.Server.VMConfig.Name != null && refresh.Server.VMConfig.HostName.Length > 0 && refresh.Server.VMConfig.Name.Length > 0)
                    {
                        long pcstartVM = 0, pcendVM = 0;
                        Statistics.QueryPerformanceCounter(ref pcstartVM);

                        command.Transaction = GetTransaction();
                        SqlHelper.AssignParameterValues(command.Parameters,
                                                        refresh.MonitoredServer.Id,
                                                        refresh.TimeStamp,
                                                        refresh.Server.VMConfig.PerfStats.CpuUsage,
                                                        refresh.Server.VMConfig.PerfStats.CpuUsageMHz,
                                                        refresh.Server.VMConfig.PerfStats.CpuReady,
                                                        refresh.Server.VMConfig.PerfStats.CpuSwapWait,
                                                        refresh.Server.VMConfig.PerfStats.MemSwapInRate,
                                                        refresh.Server.VMConfig.PerfStats.MemSwapOutRate,
                                                        (refresh.Server.VMConfig.PerfStats.MemSwapped != null && refresh.Server.VMConfig.PerfStats.MemSwapped.Kilobytes.HasValue)
                                                            ? Math.Round(refresh.Server.VMConfig.PerfStats.MemSwapped.Kilobytes.Value, 0)
                                                            : (object)null,
                                                        (refresh.Server.VMConfig.PerfStats.MemActive != null && refresh.Server.VMConfig.PerfStats.MemActive.Kilobytes.HasValue)
                                                            ? Math.Round(refresh.Server.VMConfig.PerfStats.MemActive.Kilobytes.Value, 0)
                                                            : (object)null,
                                                        (refresh.Server.VMConfig.PerfStats.MemConsumed != null && refresh.Server.VMConfig.PerfStats.MemConsumed.Kilobytes.HasValue)
                                                            ? Math.Round(refresh.Server.VMConfig.PerfStats.MemConsumed.Kilobytes.Value, 0)
                                                            : (object)null,
                                                        (refresh.Server.VMConfig.PerfStats.MemGranted != null && refresh.Server.VMConfig.PerfStats.MemGranted.Kilobytes.HasValue)
                                                            ? Math.Round(refresh.Server.VMConfig.PerfStats.MemGranted.Kilobytes.Value, 0)
                                                            : (object)null,
                                                        (refresh.Server.VMConfig.PerfStats.MemBallooned != null && refresh.Server.VMConfig.PerfStats.MemBallooned.Kilobytes.HasValue)
                                                            ? Math.Round(refresh.Server.VMConfig.PerfStats.MemBallooned.Kilobytes.Value, 0)
                                                            : (object)null,
                                                        refresh.Server.VMConfig.PerfStats.MemUsage,
                                                        refresh.Server.VMConfig.PerfStats.DiskRead,
                                                        refresh.Server.VMConfig.PerfStats.DiskWrite,
                                                        refresh.Server.VMConfig.PerfStats.DiskUsage,
                                                        refresh.Server.VMConfig.PerfStats.NetUsage,
                                                        refresh.Server.VMConfig.PerfStats.NetReceived,
                                                        refresh.Server.VMConfig.PerfStats.NetTransmitted,
                                                        refresh.Server.VMConfig.PerfStats.PagePerSec,
                                                        refresh.Server.VMConfig.PerfStats.AvaialableByteHyperV.Megabytes,
                                                        null
                                                        );

                        command.ExecuteNonQuery();

                        Statistics.QueryPerformanceCounter(ref pcendVM);
                        Statistics.VMStatisticWritten(pcstartVM, pcendVM);
                    }
                }
                catch (Exception e)
                {
                    if (!IsSQLKeyException(e))
                    {
                        LOG.Error("Save VM Perf Stats Failed [{0}]", refresh.ServerName, e);
                    }
                    throw;
                }
            }

            // Save ESX Perf Stats Data
            using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertESXStatistics"))
            {
                try
                {
                    if (refresh.Server.VMConfig.ESXHost.Name != null && refresh.Server.VMConfig.ESXHost.DomainName != null && refresh.Server.VMConfig.ESXHost.Name.Length > 0 && refresh.Server.VMConfig.ESXHost.DomainName.Length > 0)
                    {
                        long pcstartESX = 0, pcendESX = 0;
                        Statistics.QueryPerformanceCounter(ref pcstartESX);

                        command.Transaction = GetTransaction();
                        SqlHelper.AssignParameterValues(command.Parameters,
                                                        refresh.MonitoredServer.Id,
                                                        refresh.TimeStamp,
                                                        refresh.Server.VMConfig.ESXHost.PerfStats.CpuUsage,
                                                        refresh.Server.VMConfig.ESXHost.PerfStats.CpuUsageMHz,
                                                        refresh.Server.VMConfig.ESXHost.PerfStats.MemSwapInRate,
                                                        refresh.Server.VMConfig.ESXHost.PerfStats.MemSwapOutRate,
                                                        (refresh.Server.VMConfig.ESXHost.PerfStats.MemActive != null && refresh.Server.VMConfig.ESXHost.PerfStats.MemActive.Kilobytes.HasValue)
                                                            ? Math.Round(refresh.Server.VMConfig.ESXHost.PerfStats.MemActive.Kilobytes.Value, 0)
                                                            : (object)null,
                                                        (refresh.Server.VMConfig.ESXHost.PerfStats.MemConsumed != null && refresh.Server.VMConfig.ESXHost.PerfStats.MemConsumed.Kilobytes.HasValue)
                                                            ? Math.Round(refresh.Server.VMConfig.ESXHost.PerfStats.MemConsumed.Kilobytes.Value, 0)
                                                            : (object)null,
                                                        (refresh.Server.VMConfig.ESXHost.PerfStats.MemGranted != null && refresh.Server.VMConfig.ESXHost.PerfStats.MemGranted.Kilobytes.HasValue)
                                                            ? Math.Round(refresh.Server.VMConfig.ESXHost.PerfStats.MemGranted.Kilobytes.Value, 0)
                                                            : (object)null,
                                                        (refresh.Server.VMConfig.ESXHost.PerfStats.MemBallooned != null && refresh.Server.VMConfig.ESXHost.PerfStats.MemBallooned.Kilobytes.HasValue)
                                                            ? Math.Round(refresh.Server.VMConfig.ESXHost.PerfStats.MemBallooned.Kilobytes.Value, 0)
                                                            : (object)null,
                                                        refresh.Server.VMConfig.ESXHost.PerfStats.MemUsage,
                                                        refresh.Server.VMConfig.ESXHost.PerfStats.DiskRead,
                                                        refresh.Server.VMConfig.ESXHost.PerfStats.DiskWrite,
                                                        refresh.Server.VMConfig.ESXHost.PerfStats.DiskDeviceLatency,
                                                        refresh.Server.VMConfig.ESXHost.PerfStats.DiskKernelLatency,
                                                        refresh.Server.VMConfig.ESXHost.PerfStats.DiskQueueLatency,
                                                        refresh.Server.VMConfig.ESXHost.PerfStats.DiskTotalLatency,
                                                        refresh.Server.VMConfig.ESXHost.PerfStats.DiskUsage,
                                                        refresh.Server.VMConfig.ESXHost.PerfStats.NetUsage,
                                                        refresh.Server.VMConfig.ESXHost.PerfStats.NetReceived,
                                                        refresh.Server.VMConfig.ESXHost.PerfStats.NetTransmitted,
                                                        refresh.Server.VMConfig.ESXHost.PerfStats.PagePerSec,
                                                        refresh.Server.VMConfig.ESXHost.PerfStats.AvaialableByteHyperV.Megabytes,
                                                        null
                                                        );

                        command.ExecuteNonQuery();

                        Statistics.QueryPerformanceCounter(ref pcendESX);
                        Statistics.ESXStatisticWritten(pcstartESX, pcendESX);
                    }
                }
                catch (Exception e)
                {
                    if (!IsSQLKeyException(e))
                    {
                        LOG.Error("Save ESX Perf Stats Failed", e);
                    }
                    throw;
                }
            }
        }

        protected void SaveCustomCounters(ScheduledRefresh refresh)
        {
            if (refresh.TimeStamp == null)
                return;

            MetricDefinitions metricDefinitions = Management.GetMetricDefinitions();
            StringBuilder failedCounterDetails = new StringBuilder();

            using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertCustomCounterStatistics"))
            {
                try
                {
                    command.Transaction = GetTransaction();
                    double? timeDeltaInSeconds;
                    double? runTimeInMilliseconds;

                    long pcstart = 0, pcend = 0;
                    Statistics.QueryPerformanceCounter(ref pcstart);

                    foreach (CustomCounterSnapshot customCounter in refresh.CustomCounters.Values)
                    {
                        if (customCounter == null)
                        {
                            continue;
                        }
                        else
                        {
                            if (customCounter.TimeDelta != null && customCounter.TimeDelta.HasValue)
                            {
                                timeDeltaInSeconds = customCounter.TimeDelta.Value.TotalSeconds;
                            }
                            else
                            {
                                timeDeltaInSeconds = null;
                            }

                            if (customCounter.RunTime != null && customCounter.RunTime.HasValue)
                                runTimeInMilliseconds = customCounter.RunTime.Value.TotalMilliseconds;
                            else
                                runTimeInMilliseconds = null;

                            SqlDecimal raw = customCounter.RawValue.HasValue
                                                 ?
                                                     Convert.ToDecimal(customCounter.RawValue.Value)
                                                 :
                                                     SqlDecimal.Null;

                            SqlDecimal delta = customCounter.DeltaValue.HasValue
                                                   ?
                                                       Convert.ToDecimal(customCounter.DeltaValue.Value)
                                                   :
                                                       SqlDecimal.Null;

                            SqlHelper.AssignParameterValues(command.Parameters,
                                                            refresh.MonitoredServer.Id, //@SQLServerID int,
                                                            refresh.TimeStamp,
                                                            //@UTCCollectionDateTime datetime,
                                                            customCounter.Definition.MetricID, //@MetricID int,
                                                            timeDeltaInSeconds, //@TimeDeltaInSeconds float,
                                                            raw, //@RawValue decimal(38,15),
                                                            delta, //@DeltaValue decimal(38,15),
                                                            customCounter.Error != null
                                                                ? customCounter.Error.Message
                                                                : null, //@ErrorMessage nvarchar(255),
                                                            runTimeInMilliseconds,
                                                            null //@ReturnMessage nvarchar(128) output
                                );

                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (OverflowException)
                            {
                                SqlHelper.AssignParameterValues(command.Parameters,
                                                                refresh.MonitoredServer.Id, //@SQLServerID int,
                                                                refresh.TimeStamp,
                                                                //@UTCCollectionDateTime datetime,
                                                                customCounter.Definition.MetricID, //@MetricID int,
                                                                timeDeltaInSeconds, //@TimeDeltaInSeconds float,
                                                                null, //@RawValue decimal(38,15),
                                                                null, //@DeltaValue decimal(38,15),
                                                                "The collected value for this counter caused an overflow exception while being written to the repository.  Please change your counter definition so that the unscaled value is less than 15 digits.",
                                                                //@ErrorMessage nvarchar(255),
                                                                runTimeInMilliseconds,
                                                                null //@ReturnMessage nvarchar(128) output
                                    );

                                command.ExecuteNonQuery();
                            }
                            if (customCounter.Error != null)
                            {
                                if (failedCounterDetails.Length > 0)
                                    failedCounterDetails.Append(", ");

                                MetricDescription? md =
                                    metricDefinitions.GetMetricDescription(customCounter.Definition.MetricID);
                                if (md.HasValue)
                                {
                                    failedCounterDetails.Append(md.Value.Name);
                                }
                            }
                        }
                    }
                    Statistics.QueryPerformanceCounter(ref pcend);
                    Statistics.CustomStatisticWritten(pcstart, pcend, refresh.CustomCounters.Count);
                }
                catch (Exception e)
                {
                    if (!IsSQLKeyException(e))
                    {
                        LOG.Error("Save custom counters failed", e);
                    }
                    throw;
                }
                finally
                {
                    if (failedCounterDetails.Length > 0)
                        WriteCustomCounterFailedAlert(refresh.ServerName, failedCounterDetails.ToString());
                }
            }
        }

        protected void WriteCustomCounterFailedAlert(string instanceName, string failedCounterList)
        {
            MonitoredObjectName name = new MonitoredObjectName(instanceName);
            string heading = "One or more custom counters were not collected";
            string message =
                String.Format(
                    "SQL Server instance {0} - The following custom counters were not collected: \r\n   {1} ",
                    instanceName, failedCounterList.ToString());

            AlertTableWriter.LogOperationalAlerts(Metric.Operational, name, MonitoredState.Critical, heading, message);
        }

        protected void SaveSessionsAndLocks(ScheduledRefresh refresh)
        {
            if (refresh.TimeStamp == null)
                return;

            byte[] serializedLockList = null;
            byte[] serializedLockCounters = null;
            byte[] serializedSessionList = null;
            byte[] serializedSystemProcesses = null;

            long pcstart = 0, pcend = 0;
            Statistics.QueryPerformanceCounter(ref pcstart);

            if (refresh.LockList != null)
            {
                try
                {
                    serializedLockList = Serialized<object>.SerializeCompressed(refresh.LockList.LockList);
                }
                catch (Exception e)
                {
                    LOG.Warn("Unable to serialize LockList.LockList: ", e);
                }
                try
                {
                    serializedLockCounters = Serialized<object>.SerializeCompressed(refresh.LockList.LockCounters);
                }
                catch (Exception e)
                {
                    LOG.Warn("Unable to serialize LockList.LockCounters: ", e);
                }
            }
            else
                LOG.Warn("Scheduled refresh did not contain a LockList.");


            if (refresh.SessionList != null)
            {
                try
                {
                    serializedSessionList = Serialized<object>.SerializeCompressed(refresh.SessionList.SessionList);
                }
                catch (Exception e)
                {
                    LOG.Warn("Unable to serialize SessionList.SessionList: ", e);
                }
                try
                {
                    serializedSystemProcesses =
                        Serialized<object>.SerializeCompressed(refresh.SessionList.SystemProcesses);
                }
                catch (Exception e)
                {
                    LOG.Warn("Unable to serialize SessionList.SystemProcesses: ", e);
                }
            }
            else
                LOG.Warn("Scheduled refresh did not contain a SessionList.");

            // no need to write to the repository unless at least one of the values is set
            if (serializedLockList != null || serializedSessionList != null)
            {
                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertServerActivity"))
                {
                    try
                    {
                        command.Transaction = GetTransaction();
                        SqlHelper.AssignParameterValues(command.Parameters,
                                                        refresh.MonitoredServer.Id,
                                                        refresh.TimeStamp,
                                                        null, // @StateOverview (not setting at this time)
                                                        serializedSystemProcesses, // @Sessions
                                                        serializedSessionList, // @Locks
                                                        serializedLockCounters, // @LockStatistics
                                                        serializedLockList, // @LockList
                                                        refresh.SnapshotType
                            );

                        command.ExecuteNonQuery();
                        Commit();
                    }
                    catch (Exception e)
                    {
                        if (!IsSQLKeyException(e))
                        {
                            LOG.Error("Save Sessions and Locks statistics failed", e);
                        }
                        throw;
                    }
                }
            }
            Statistics.QueryPerformanceCounter(ref pcend);
            Statistics.SessionAndLocksStatisticWritten(pcstart, pcend);
        }

        protected void SaveWaitStatistics(ScheduledRefresh refresh)
        {
            try
            {
                if (refresh.ProductVersion.Major > 8 && refresh.WaitStats != null && refresh.WaitStats.HasBeenCalculated &&
                    refresh.WaitStats.Waits.Count > 0)
                {
                    Dictionary<string, long> waitTypesCopy = Management.ScheduledCollection.CachedWaitTypes;
                    Dictionary<string, long> missingWaitTypes = new Dictionary<string, long>();
                    long waitStatisticsId;

                    long pcstartWtType = 0, pcendWtType = 0;
                    Statistics.QueryPerformanceCounter(ref pcstartWtType);

                    foreach (string waitType in refresh.WaitStats.Waits.Keys)
                    {
                        if (!waitTypesCopy.ContainsKey(waitType))
                        {
                            missingWaitTypes.Add(waitType, -1);
                        }
                    }

                    if (missingWaitTypes.Keys.Count > 0)
                    {
                        Dictionary<string, long> saveWaitTypes = new Dictionary<string, long>();

                        using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertWaitTypes"))
                        {
                            try
                            {
                                foreach (string waitType in missingWaitTypes.Keys)
                                {
                                    command.Transaction = GetTransaction();
                                    SqlHelper.AssignParameterValues(command.Parameters,
                                                                    waitType,
                                                                    null,
                                                                    //@ReturnID
                                                                    null //  @ReturnMessage
                                        );

                                    command.ExecuteNonQuery();
                                    saveWaitTypes.Add(waitType, (int)command.Parameters["@ReturnID"].Value);
                                }
                            }
                            catch (Exception e)
                            {
                                if (!IsSQLKeyException(e))
                                {
                                    LOG.Error("Save wait types failed", e);
                                }
                                throw;
                            }
                        }

                        waitTypesCopy = Management.ScheduledCollection.AddCachedWaitStats(saveWaitTypes);
                        Commit();
                    }
                    Statistics.QueryPerformanceCounter(ref pcendWtType);
                    Statistics.WaitTypeStatisticWritten(pcstartWtType, pcendWtType, missingWaitTypes.Keys.Count);

                    using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertWaitStatistics"))
                    {
                        try
                        {
                            long pcstartWtSt = 0, pcendWtSt = 0;
                            Statistics.QueryPerformanceCounter(ref pcstartWtSt);

                            command.Transaction = GetTransaction();
                            SqlHelper.AssignParameterValues(command.Parameters,
                                                            refresh.MonitoredServer.Id,
                                                            refresh.TimeStamp,
                                                            refresh.Server.TimeDelta.Value.TotalSeconds,
                                                            null,
                                                            null //  @ReturnMessage
                                );

                            command.ExecuteNonQuery();
                            waitStatisticsId = (long)command.Parameters["@ReturnID"].Value;

                            Statistics.QueryPerformanceCounter(ref pcendWtSt);
                            Statistics.WaitStStatisticWritten(pcstartWtSt, pcendWtSt);
                        }
                        catch (Exception e)
                        {
                            if (!IsSQLKeyException(e))
                            {
                                LOG.Error("Save wait statistics failed", e);
                            }
                            throw;
                        }
                    }
                    Commit();


                    try
                    {
                        long pcstartWtStDtl = 0, pcendWtStDtl = 0;
                        Statistics.QueryPerformanceCounter(ref pcstartWtStDtl);

                        try
                        {
                            SaveWaitStatsBulk(refresh, waitTypesCopy, waitStatisticsId);
                        }
                        catch (Exception e)
                        {
                            if (!IsSQLKeyException(e))
                            {
                                LOG.ErrorFormat(
                                    "Error occurred while doing a bulk copy for Server Waits Details: {0}", e);
                            }

                            using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertWaitStatisticsDetails"))
                            {
                                foreach (Wait wait in refresh.WaitStats.Waits.Values)
                                {
                                    if (waitTypesCopy.ContainsKey(wait.WaitType))
                                    {
                                        // We only want to save if there is useful information of some sort
                                        // Max wait time is not included because it should not have changed if nothing else changed
                                        if ((wait.WaitingTasksCountDelta > 0) ||
                                            ((wait.WaitTimeDelta.HasValue
                                                  ? wait.WaitTimeDelta.Value.TotalMilliseconds
                                                  : 0) > 0) ||
                                            ((wait.ResourceWaitTimeDelta.HasValue
                                                  ? wait.ResourceWaitTimeDelta.Value.TotalMilliseconds
                                                  : 0) > 0))
                                        {
                                            command.Transaction = GetTransaction();
                                            SqlHelper.AssignParameterValues(command.Parameters,
                                                                            waitStatisticsId,
                                                                            waitTypesCopy[wait.WaitType],
                                                                            wait.WaitingTasksCountDelta,
                                                                            wait.WaitTimeDelta.HasValue
                                                                                ? wait.WaitTimeDelta.Value.
                                                                                      TotalMilliseconds
                                                                                : (double?)null,
                                                                            wait.MaxWaitTime.HasValue
                                                                                ? wait.MaxWaitTime.Value.
                                                                                      TotalMilliseconds
                                                                                : (double?)null,
                                                                            wait.ResourceWaitTimeDelta.HasValue
                                                                                ? wait.ResourceWaitTimeDelta.Value.
                                                                                      TotalMilliseconds
                                                                                : (double?)null,
                                                                            null //  @ReturnMessage
                                                );

                                            command.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }

                        Statistics.QueryPerformanceCounter(ref pcendWtStDtl);
                        Statistics.WaitStDtlStatisticWritten(pcstartWtStDtl, pcendWtStDtl, refresh.WaitStats.Waits.Values.Count);
                    }
                    catch (Exception e)
                    {
                        if (!IsSQLKeyException(e))
                        {
                            LOG.Error("Save wait statistics details failed", e);
                        }
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                if (!IsSQLKeyException(e))
                {
                    LOG.Error("Save waits failed", e);
                    throw;
                }
            }
        }

        /// <summary>
        /// Use a bulk copy to write the waits stats
        /// </summary>
        /// <param name="refresh"></param>
        /// <param name="waitTypesCopy"></param>
        /// <param name="waitStatisticsId"></param>
        protected void SaveWaitStatsBulk(ScheduledRefresh refresh, Dictionary<string, long> waitTypesCopy, long waitStatisticsId)
        {

            DataTable waitTable = new DataTable("ServerWaits");

            DataColumn idColumn = new DataColumn("WaitStatisticsID", typeof(Int64));
            waitTable.Columns.Add(idColumn);
            waitTable.Columns.Add("WaitTypeID", typeof(Int32));
            waitTable.Columns.Add("WaitingTasks", typeof(Int64));
            waitTable.Columns.Add("WaitTimeInMilliseconds", typeof(Int64));
            waitTable.Columns.Add("MaxWaitTimeInMilliseconds", typeof(Int64));
            waitTable.Columns.Add("ResourceWaitTimeInMilliseconds", typeof(Int64));

            using (SqlTransaction xa = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    //long pcstartWtStDtl = 0, pcendWtStDtl = 0;
                    //Statistics.QueryPerformanceCounter(ref pcstartWtStDtl);
                    ServerVersion version = new ServerVersion(connection.ServerVersion);
                    SqlBulkCopyOptions options = version.Major >= 9
                                                                 ? SqlBulkCopyOptions.Default
                                                                 : SqlBulkCopyOptions.Default |
                                                                   SqlBulkCopyOptions.CheckConstraints;

                    foreach (var wait in refresh.WaitStats.Waits.Values)
                    {
                        if (waitTypesCopy.ContainsKey(wait.WaitType))
                        {
                            // We only want to save if there is useful information of some sort
                            // Max wait time is not included because it should not have changed if nothing else changed
                            if ((wait.WaitingTasksCountDelta > 0) ||
                            ((wait.WaitTimeDelta.HasValue ? wait.WaitTimeDelta.Value.TotalMilliseconds : 0) > 0) ||
                            ((wait.ResourceWaitTimeDelta.HasValue ? wait.ResourceWaitTimeDelta.Value.TotalMilliseconds : 0) > 0))
                            {
                                //command.Transaction = GetTransaction();
                                DataRow row = waitTable.NewRow();
                                row["WaitStatisticsID"] = waitStatisticsId;
                                row["WaitTypeID"] = waitTypesCopy[wait.WaitType];
                                row["WaitingTasks"] = wait.WaitingTasksCountDelta.HasValue
                                                                ? wait.WaitingTasksCountDelta.Value
                                                               : (object)DBNull.Value;
                                row["WaitTimeInMilliseconds"] = wait.WaitTimeDelta.HasValue
                                                                    ? wait.WaitTimeDelta.Value.TotalMilliseconds
                                                                    : (object)DBNull.Value;
                                row["MaxWaitTimeInMilliseconds"] = wait.MaxWaitTime.HasValue
                                                                       ? wait.MaxWaitTime.Value.TotalMilliseconds
                                                                       : (object)DBNull.Value;
                                row["ResourceWaitTimeInMilliseconds"] = wait.ResourceWaitTimeDelta.HasValue
                                                                            ? wait.ResourceWaitTimeDelta.Value.
                                                                                  TotalMilliseconds
                                                                            : (object)DBNull.Value;


                                waitTable.Rows.Add(row);
                            }
                        }
                    }

                    LOG.VerboseFormat("About to bulk copy {0} waits", waitTable.Rows.Count);

                    waitTable.AcceptChanges();
                    using (var bulkCopy = new SqlBulkCopy(connection, options, xa))
                    {
                        bulkCopy.DestinationTableName = "WaitStatisticsDetails";
                        bulkCopy.WriteToServer(waitTable);
                    }
                    xa.Commit();
                    //Statistics.QueryPerformanceCounter(ref pcendWtStDtl);
                    //Statistics.WaitStDtlStatisticWritten(pcstartWtStDtl, pcendWtStDtl, refresh.WaitStats.Waits.Values.Count);
                }
                catch (Exception e)
                {
                    try
                    {
                        xa.Rollback();
                    }
                    catch
                    {
                        /* */
                    }
                    if (!IsSQLKeyException(e))
                    {
                        LOG.ErrorFormat("Exception doing bulk copy to the wait stats details: {0}", e);
                    }
                    throw;
                }
            }
        }

        protected void SaveActiveWaits(ScheduledRefresh refresh, Dictionary<string, int> DatabaseNames,
                                                  ref Dictionary<long, long> cachedSqlCopy,
                                                  ref Dictionary<long, long> cachedSigCopy,
                                                  ref Dictionary<long, long> cachedHostNameCopy,
                                                  ref Dictionary<long, long> cachedApplicationNameCopy,
                                                  ref Dictionary<long, long> cachedLoginNameCopy,
                                                  TimeSpan utcOffset)
        {
            using (LOG.VerboseCall("SaveActiveWaits"))
            {
                if (refresh.ActiveWaits != null && refresh.ActiveWaits.ActiveWaits != null &&
                    refresh.ActiveWaits.ActiveWaits.Rows.Count > 0)
                {
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertActiveWait"))
                    {
                        try
                        {
                            if (DatabaseNames == null || DatabaseNames.Count == 0)
                                DatabaseNames = new Dictionary<string, int>();

                            long pcstart = 0, pcend = 0;
                            Statistics.QueryPerformanceCounter(ref pcstart);

                            foreach (DataRow dr in refresh.ActiveWaits.ActiveWaits.Rows)
                            {
                                command.Transaction = GetTransaction();
                                try
                                {
                                    // ReSharper disable InconsistentNaming
                                    string programName =
                                        (dr["ProgramName"] == DBNull.Value)
                                            ? string.Empty
                                            : dr["ProgramName"].ToString();
                                    string databaseName =
                                        dr["DatabaseName"].GetType() != typeof(System.DBNull)
                                            ? (string)dr["DatabaseName"]
                                            : "";
                                    string hostName =
                                        dr["HostName"] == DBNull.Value ? string.Empty : dr["HostName"].ToString();
                                    string loginName = dr["LoginName"] == DBNull.Value ? string.Empty : dr["LoginName"].ToString();
                                    int sessionId = -1;
                                    if (dr["SessionID"] != DBNull.Value)
                                    {
                                        int.TryParse(dr["SessionID"].ToString(), out sessionId);
                                    }
                                    string StatementText = ((string)dr["StatementText"]).Trim();


                                    string SQLHash = SqlParsingHelper.GetStatementHash(StatementText);

                                    long SQLHashNumeric = SQLHash.GetHashCode();
                                    long SQLId = -1;
                                    cachedSqlCopy.TryGetValue(SQLHashNumeric, out SQLId);

                                    string signature = SqlParsingHelper.GetReadableSignature(StatementText);
                                    string signatureHash = SqlParsingHelper.GetSignatureHash(StatementText);
                                    long SQLSigId = -1;
                                    long signatureHashNumeric = signatureHash.GetHashCode();
                                    cachedSigCopy.TryGetValue(signatureHashNumeric, out SQLSigId);

                                    long applicationNameId = -1;
                                    long applicationNumeric = programName.GetHashCode();
                                    cachedApplicationNameCopy.TryGetValue(applicationNumeric, out applicationNameId);

                                    long hostNameId = -1;
                                    long hostNameNumeric = hostName.GetHashCode();
                                    cachedHostNameCopy.TryGetValue(hostNameNumeric, out hostNameId);

                                    long loginNameId = -1;
                                    long loginNameNumeric = loginName.GetHashCode();
                                    cachedLoginNameCopy.TryGetValue(loginNameNumeric, out loginNameId);

                                    int databaseNameId =
                                        DatabaseNames.ContainsKey(databaseName)
                                            ? (int)DatabaseNames[databaseName]
                                            : 0;


                                    DateTime? StatementUTCStartTime = null;
                                    try
                                    {
                                        StatementUTCStartTime = (DateTime)dr["StatementUTCStartTime"];
                                    }
                                    catch (Exception e)
                                    {
                                        LOG.Error("Null start time for query waits", e);
                                        continue;
                                    }

                                    long WaitDuration = 0;
                                    long.TryParse(dr["WaitDuration"].ToString(), out WaitDuration);
                                    string WaitType = (string)dr["WaitType"];
                                    long MSTicks = 0;
                                    long.TryParse(dr["MSTicks"].ToString(), out MSTicks);

                                    SqlHelper.AssignParameterValues(command.Parameters,
                                                                    refresh.MonitoredServer.Id, //@SQLServerID int,
                                                                    refresh.TimeStamp,//@UTCCollectionDateTime datetime,
                                                                    applicationNameId > 0 ? null : programName,//@ApplicationName nvarchar(256),
                                                                    applicationNameId, //@ApplicationNameID int output,
                                                                    databaseNameId > 0 ? null : databaseName,//@DatabaseName nvarchar(255),
                                                                    databaseNameId, //@DatabaseID int output,
                                                                    hostNameId > 0 ? null : hostName,//@HostName nvarchar(256),
                                                                    hostNameId, //@HostNameID int output,
                                                                    loginNameId > 0 ? null : loginName,//@LoginName nvarchar(256),                                           
                                                                    loginNameId, //@LoginNameID int output,
                                                                    sessionId, //@SessionID smallint,
                                                                    SQLId > 0 ? null : StatementText,//@SQLStatement varchar(4000),
                                                                    SQLHash, //@SQLStatementHash nvarchar(30),
                                                                    SQLId, //@SQLStatementID int output,
                                                                    SQLSigId > 0 ? null : signature,//@SQLSignature nvarchar(4000),
                                                                    signatureHash, //@SQLSignatureHash nvarchar(30),
                                                                    SQLSigId, //@SQLSignatureID int output,
                                                                    StatementUTCStartTime,//@StatementUTCStartTime datetime,
                                                                    StatementUTCStartTime.Value.Add(utcOffset),
                                                                    WaitDuration, //@WaitDuration bigint,
                                                                    WaitType, //@WaitType varchar(120),
                                                                    Management.ScheduledCollection.CachedWaitTypes.
                                                                        ContainsKey(WaitType)
                                                                        ? (long?)
                                                                          Management.ScheduledCollection.CachedWaitTypes
                                                                              [WaitType]
                                                                        : null, //@WaitTypeID int output
                                                                    MSTicks
                                        );
                                    // ReSharper restore InconsistentNaming

                                    try
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                    catch (SqlException)
                                    {
                                        // Try again in case it's just bad caching
                                        Rollback();
                                        command.Transaction = GetTransaction();
                                        SqlHelper.AssignParameterValues(command.Parameters,
                                                                        refresh.MonitoredServer.Id, //@SQLServerID int,
                                                                        refresh.TimeStamp,
                                                                        //@UTCCollectionDateTime datetime,
                                                                        programName, //@ApplicationName nvarchar(256),
                                                                        0, //@ApplicationNameID int output,
                                                                        databaseName, //@DatabaseName nvarchar(255),
                                                                        0, //@DatabaseID int output,
                                                                        hostName, //@HostName nvarchar(256),
                                                                        0, //@HostNameID int output,
                                                                        loginName,
                                                                        //@LoginName nvarchar(256),                                           
                                                                        0, //@LoginNameID int output,
                                                                        sessionId, //@SessionID smallint,
                                                                        StatementText, //@SQLStatement varchar(4000),
                                                                        SQLHash, //@SQLStatementHash nvarchar(30),
                                                                        0, //@SQLStatementID int output,
                                                                        signature, //@SQLSignature nvarchar(4000),
                                                                        signatureHash, //@SQLSignatureHash nvarchar(30),
                                                                        0, //@SQLSignatureID int output,
                                                                        StatementUTCStartTime,//@StatementUTCStartTime datetime,
                                                                        StatementUTCStartTime.Value.Add(utcOffset),
                                                                        WaitDuration, //@WaitDuration bigint,
                                                                        WaitType, //@WaitType varchar(120),
                                                                        Management.ScheduledCollection.CachedWaitTypes.
                                                                            ContainsKey(WaitType)
                                                                            ? (long?)
                                                                              Management.ScheduledCollection.
                                                                                  CachedWaitTypes[WaitType]
                                                                            : null, //@WaitTypeID int output
                                                                        MSTicks
                                            );
                                        command.ExecuteNonQuery();
                                        cachedSqlCopy.Clear();
                                        cachedApplicationNameCopy.Clear();
                                        cachedHostNameCopy.Clear();
                                        cachedLoginNameCopy.Clear();
                                        ExpireQueryMonitorCaches();
                                    }


                                    if (
                                        (Int64.TryParse(command.Parameters["@SQLStatementID"].Value.ToString(),
                                                        out SQLId)) &&
                                        (!cachedSqlCopy.ContainsKey(SQLHashNumeric)))
                                    {
                                        cachedSqlCopy.Add(SQLHashNumeric, SQLId);
                                    }

                                    if (
                                        (Int64.TryParse(command.Parameters["@ApplicationNameID"].Value.ToString(),
                                                        out applicationNameId)) &&
                                        (!cachedApplicationNameCopy.ContainsKey(applicationNumeric)))
                                    {
                                        cachedApplicationNameCopy.Add(applicationNumeric, applicationNameId);
                                    }

                                    if (
                                        (Int64.TryParse(command.Parameters["@HostNameID"].Value.ToString(),
                                                        out hostNameId)) &&
                                        (!cachedHostNameCopy.ContainsKey(hostNameNumeric)))
                                    {
                                        cachedHostNameCopy.Add(hostNameNumeric, hostNameId);
                                    }

                                    if (
                                        (Int64.TryParse(command.Parameters["@LoginNameID"].Value.ToString(),
                                                        out loginNameId)) &&
                                        (!cachedLoginNameCopy.ContainsKey(loginNameNumeric)))
                                    {
                                        cachedLoginNameCopy.Add(loginNameNumeric, loginNameId);
                                    }
                                    Commit();
                                }
                                catch (Exception e)
                                {
                                    if (!IsSQLKeyException(e))
                                    {
                                        LOG.Error("Save active wait failed", e);
                                    }
                                    Rollback();
                                }
                            }

                            Statistics.QueryPerformanceCounter(ref pcend);
                            Statistics.QueryWaitStatisticWritten(pcstart, pcend, refresh.ActiveWaits.ActiveWaits.Rows.Count);
                        }
                        catch
                            (Exception e)
                        {
                            if (!IsSQLKeyException(e))
                            {
                                LOG.Error("Save active waits failed", e);
                                throw;
                            }
                        }
                    }
                }
            }
        }
        private void MatchForAzure(ScheduledRefresh refresh)
        {
            foreach (var info in refresh.BlockingSessions.Values)
            {
                bool found = false;
                //if there is a match in Blocking Sessions
                foreach (var session in refresh.Alerts.BlockingSessions)
                {
                    if (session.Spid.HasValue)
                    {
                        if (info.LastBatchCompleted.HasValue && session.BlockingLastBatch.HasValue)
                        {
                            if (info.SessionId == session.Spid)
                            {
                                found = true;
                                //give the sessions from sysprocesses an XActID
                                session.xActID = info.XActId;
                                session.BlockID = info.GetId();
                                //trace will have a more accurate blocking time
                                //session.BlockingTime = info.BlockingTime;
                                session.WaitResource = info.WaitResource;
                                session.IsInterRefresh = true;
                            }
                        }
                    }
                }
            }
        }

        //SQLDM-30037
        protected BlockingSessionInfo GetBlockingSessionInfo(Dictionary<long, BlockingSessionInfo> blockingSessions, int? spid)
        {
            foreach (BlockingSessionInfo blockingSessionInfo in blockingSessions.Values)
            {
                if (blockingSessionInfo.SessionId == spid)
                {
                    return blockingSessionInfo;
                }
            }
            return null;
        }

        protected void SaveBlocking(ScheduledRefresh refresh, Dictionary<string, int> DatabaseNames,
                                                   ref Dictionary<long, long> cachedSqlCopy,
                                                   ref Dictionary<long, long> cachedSigCopy,
                                                   ref Dictionary<long, long> cachedHostNameCopy,
                                                   ref Dictionary<long, long> cachedApplicationNameCopy,
                                                   ref Dictionary<long, long> cachedLoginNameCopy,
                                                   TimeSpan utcOffset)
        {
            using (LOG.VerboseCall("SaveBlocking"))
            {
                if (refresh.Alerts != null && refresh.Alerts.BlockingSessions != null &&
                    refresh.Alerts.BlockingSessions.Count > 0)
                {
                    using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_InsertBlockingSession"))
                    {
                        try
                        {
                            if (DatabaseNames == null || DatabaseNames.Count == 0)
                                DatabaseNames = new Dictionary<string, int>();

                            long pcstart = 0, pcend = 0;
                            Statistics.QueryPerformanceCounter(ref pcstart);

                            foreach (BlockingSession block in refresh.Alerts.BlockingSessions)
                            {
                                using (SqlCommand reportCommand = SqlHelper.CreateCommand(connection, "p_InsertBlock"))
                                {
                                    try
                                    {
                                        if (refresh.MonitoredServer.CloudProviderId == 2) //SQLDM 11.0 Azure Blocking session Support:Sync up between the data from extended events and refresh Probes.
                                        {
                                            MatchForAzure(refresh);
                                        }
                                        string xdl = "";
                                        int? spid = block.Spid;
                                        //SQLDM-30037
                                        BlockingSessionInfo blockingSessionInfo = GetBlockingSessionInfo(refresh.BlockingSessions, spid);

                                        xdl = blockingSessionInfo.XdlString;
                                        Guid blockId = blockingSessionInfo.GetId();


                                        if (spid != null)
                                        {
                                            reportCommand.Transaction = GetTransaction();

                                            SqlHelper.AssignParameterValues(reportCommand.Parameters,
                                                                            blockId,
                                                                            refresh.MonitoredServer.Id,
                                                                            refresh.TimeStamp,
                                                                            block.xActID,
                                                                            xdl,
                                                                            null //  @ReturnMessage
                                                );

                                            reportCommand.ExecuteNonQuery();

                                            Commit();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (!IsSQLKeyException(ex))
                                        {
                                            LOG.Error("Save Blocking report failed.", ex);
                                        }
                                        Rollback();
                                    }

                                    command.Transaction = GetTransaction();
                                    try
                                    {

                                        string SQLHash = SqlParsingHelper.GetStatementHash(block.InputBuffer);
                                        long SQLHashNumeric = SQLHash.GetHashCode();
                                        long SQLId = -1;
                                        cachedSqlCopy.TryGetValue(SQLHashNumeric, out SQLId);

                                        string signature = SqlParsingHelper.GetReadableSignature(block.InputBuffer);
                                        string signatureHash = SqlParsingHelper.GetSignatureHash(block.InputBuffer);
                                        long SQLSigId = -1;
                                        long signatureHashNumeric = signatureHash.GetHashCode();
                                        cachedSigCopy.TryGetValue(signatureHashNumeric, out SQLSigId);

                                        long applicationNameId = -1;
                                        long applicationNumeric = block.Application.GetHashCode();
                                        cachedApplicationNameCopy.TryGetValue(applicationNumeric, out applicationNameId);

                                        long hostNameId = -1;
                                        long hostNameNumeric = block.Host.GetHashCode();
                                        cachedHostNameCopy.TryGetValue(hostNameNumeric, out hostNameId);

                                        long loginNameId = -1;
                                        long loginNameNumeric = block.Login.GetHashCode();
                                        cachedLoginNameCopy.TryGetValue(loginNameNumeric, out loginNameId);

                                        int databaseNameId =
                                            DatabaseNames.ContainsKey(block.Databasename)
                                                ? (int)DatabaseNames[block.Databasename]
                                                : 0;


                                        SqlHelper.AssignParameterValues(command.Parameters,
                                                                        refresh.MonitoredServer.Id, //@SQLServerID int,
                                                                        refresh.TimeStamp,
                                                                        block.xActID.HasValue ? refresh.BlockingSessions[block.xActID.Value].GetId() : Guid.Empty,
                                                                        //@UTCCollectionDateTime datetime,
                                                                        applicationNameId > 0 ? null : block.Application,
                                                                        //@ApplicationName nvarchar(256),
                                                                        applicationNameId,
                                                                        //@ApplicationNameID int output,
                                                                        databaseNameId > 0 ? null : block.Databasename,
                                                                        //@DatabaseName nvarchar(255),
                                                                        databaseNameId, //@DatabaseID int output,
                                                                        hostNameId > 0 ? null : block.Host,
                                                                        //@HostName nvarchar(256),
                                                                        hostNameId, //@HostNameID int output,
                                                                        loginNameId > 0 ? null : block.Login,
                                                                        //@LoginName nvarchar(256),                                           
                                                                        loginNameId, //@LoginNameID int output,
                                                                        block.Spid, //@SessionID smallint,
                                                                        SQLId > 0 ? null : block.InputBuffer,
                                                                        //@SQLStatement varchar(4000),
                                                                        SQLHash, //@SQLStatementHash nvarchar(30),
                                                                        SQLId, //@SQLStatementID int output,
                                                                        SQLSigId > 0 ? null : signature,
                                                                        //@SQLSignature nvarchar(4000),
                                                                        signatureHash, //@SQLSignatureHash nvarchar(30),
                                                                        SQLSigId, //@SQLSignatureID int output,
                                                                        block.BlockingStartTimeUTC,
                                                                        //@BlockingStartTimeUTC datetime,
                                                                        (block.BlockingStartTimeUTC.HasValue)
                                                                            ? block.BlockingStartTimeUTC.Value.Add(
                                                                                utcOffset)
                                                                            : block.BlockingStartTimeUTC,
                                                                        block.BlockingTime.TotalMilliseconds
                                            //@@BlockingDurationMilliseconds bigint,

                                            );

                                        try
                                        {
                                            command.ExecuteNonQuery();
                                        }
                                        catch (SqlException)
                                        {
                                            // Try again in case it's just bad caching
                                            Rollback();
                                            command.Transaction = GetTransaction();
                                            SqlHelper.AssignParameterValues(command.Parameters,
                                                                            refresh.MonitoredServer.Id,
                                                                            //@SQLServerID int,
                                                                            refresh.TimeStamp,
                                                                            block.xActID.HasValue ? refresh.BlockingSessions[block.xActID.Value].GetId() : Guid.Empty,
                                                                            //@UTCCollectionDateTime datetime,
                                                                            block.Application,
                                                                            //@ApplicationName nvarchar(256),
                                                                            0, //@ApplicationNameID int output,
                                                                            block.Databasename,
                                                                            //@DatabaseName nvarchar(255),
                                                                            0, //@DatabaseID int output,
                                                                            block.Host, //@HostName nvarchar(256),
                                                                            0, //@HostNameID int output,
                                                                            block.Login,
                                                                            //@LoginName nvarchar(256),                                           
                                                                            0, //@LoginNameID int output,
                                                                            block.Spid, //@SessionID smallint,
                                                                            block.InputBuffer,
                                                                            //@SQLStatement varchar(4000),
                                                                            SQLHash, //@SQLStatementHash nvarchar(30),
                                                                            SQLId, //@SQLStatementID int output,
                                                                            signature, //@SQLSignature nvarchar(4000),
                                                                            signatureHash,
                                                                            //@SQLSignatureHash nvarchar(30),
                                                                            0, //@SQLSignatureID int output,
                                                                            block.BlockingStartTimeUTC,
                                                                            //@BlockingStartTimeUTC datetime,
                                                                            (block.BlockingStartTimeUTC.HasValue)
                                                                                ? block.BlockingStartTimeUTC.Value.
                                                                                      Add(utcOffset)
                                                                                : block.BlockingStartTimeUTC,
                                                                            block.BlockingTime.TotalMilliseconds);
                                            //@@BlockingDurationMilliseconds bigint,
                                            command.ExecuteNonQuery();
                                            cachedSqlCopy.Clear();
                                            cachedApplicationNameCopy.Clear();
                                            cachedHostNameCopy.Clear();
                                            cachedLoginNameCopy.Clear();
                                            ExpireQueryMonitorCaches();
                                        }


                                        if (
                                            (Int64.TryParse(command.Parameters["@SQLStatementID"].Value.ToString(),
                                                            out SQLId)) &&
                                            (!cachedSqlCopy.ContainsKey(SQLHashNumeric)))
                                        {
                                            cachedSqlCopy.Add(SQLHashNumeric, SQLId);
                                        }

                                        if (
                                            (Int64.TryParse(command.Parameters["@ApplicationNameID"].Value.ToString(),
                                                            out applicationNameId)) &&
                                            (!cachedApplicationNameCopy.ContainsKey(applicationNumeric)))
                                        {
                                            cachedApplicationNameCopy.Add(applicationNumeric, applicationNameId);
                                        }

                                        if (
                                            (Int64.TryParse(command.Parameters["@HostNameID"].Value.ToString(),
                                                            out hostNameId)) &&
                                            (!cachedHostNameCopy.ContainsKey(hostNameNumeric)))
                                        {
                                            cachedHostNameCopy.Add(hostNameNumeric, hostNameId);
                                        }

                                        if (
                                            (Int64.TryParse(command.Parameters["@LoginNameID"].Value.ToString(),
                                                            out loginNameId)) &&
                                            (!cachedLoginNameCopy.ContainsKey(loginNameNumeric)))
                                        {
                                            cachedLoginNameCopy.Add(loginNameNumeric, loginNameId);
                                        }
                                        Commit();
                                    }
                                    catch (Exception e)
                                    {
                                        if (!IsSQLKeyException(e))
                                        {
                                            LOG.Error("Save blocking session failed", e);
                                        }
                                        Rollback();
                                    }
                                }
                            }
                            Statistics.QueryPerformanceCounter(ref pcend);
                            Statistics.BlockingStatisticWritten(pcstart, pcend, refresh.Alerts.BlockingSessions.Count);
                        }
                        catch
                            (Exception e)
                        {
                            if (!IsSQLKeyException(e))
                            {
                                LOG.Error("Save Blocking failed", e);
                                throw;
                            }
                        }
                    }
                }
            }
        }

        protected void LogAndThrow(string message)
        {
            LOG.Error(message);
            throw new ApplicationException(message);
        }

        protected internal void Commit()
        {
            if (transaction != null)
            {
                transaction.Commit();
                transaction.Dispose();
                transaction = null;
            }
        }

        protected void Rollback()
        {
            if (transaction != null)
            {
                transaction.Rollback();
                transaction.Dispose();
                transaction = null;
            }
        }

        public void Dispose()
        {
            if (saveEventCommand != null)
            {
                saveEventCommand.Dispose();
                saveEventCommand = null;
            }
            if (deleteEventCommand != null)
            {
                deleteEventCommand.Dispose();
                deleteEventCommand = null;
            }
            if (transaction != null)
            {
                transaction.Dispose();
                transaction = null;
            }
            if (connection != null)
            {
                connection.Dispose();
                connection = null;
            }
        }
    }
}
