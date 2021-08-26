using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics.WaitingObjects;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    internal class WaitStatsAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 17;
        private static Logger _logX = Logger.GetLogger("WaitStatsAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public WaitStatsAnalyzer()
        {
            _id = id;
        }

        public override string GetDescription() { return ("Wait stats analysis"); }

        public override void Analyze(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("WaitStatsAnalyzer.Analyze"))
            {
                base.Analyze(sm, conn);
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.WaitStatsMetrics) { _logX.Debug("null WaitStatsCollector"); return; }
                if (null == sm.WaitingBatchesMetrics) { _logX.Debug("null WaitStatsCollector"); return; }
                if (null == sm.WaitStatsMetrics.Stats) { _logX.Debug("null WaitStatsCollector.Stats"); return; }
                if (null == sm.Options) { _logX.Debug("null Options"); return; }
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                if (null == sm.WMIPerfOSProcessorMetrics) { _logX.Debug("null WMIPerfOSProcessorCollector"); return; }

                if (null == sm.WaitStatsMetrics.Stats.Top(5)) { _logX.Debug("null Top Waits"); return; }

                LogTopWaits(sm);

                AnalyzeCXPACKET(sm, conn);
                AnalyzeASYNC_NETWORK_IO(sm, conn);
                AnalyzeTHREADPOOL(sm, conn);
                AnalyzePAGEIOLATCH_XX(sm, conn);
                AnalyzePAGELATCH_UP(sm, conn);
                AnalyzePAGELATCH_EXSH(sm, conn);
            }
        }

        //------------------------------------------------------------------------------------------------------
        //  PAGELATCH_ EX & SH Occurs when a task is waiting for a latch for a buffer that is not in an I/O request. 
        //  
        //  The latch request is in Exclusive mode (EX) or Shared mode (SH). 
        //  
        //  Contention can be caused by issues other than IO or memory performance, for example, heavy 
        //  concurrent inserts into the same index range can cause this kind of contention. If many inserts 
        //  must be added on the same page, they are serialized using the latch. Lots of inserts into the same 
        //  range can also cause page splits in the index which holds onto the latch while allocating a new page 
        //  (this can take time). Any read accesses to the same range as the inserts would also conflict on the 
        //  latches. The solution in these cases is to distribute the inserts using a more appropriate index.
        //  
        private void AnalyzePAGELATCH_EXSH(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzePAGELATCH_EXSH"))
            {
                string[] waits = new string[] { "PAGELATCH_EX", "PAGELATCH_SH" };
                bool startsWith = false;
#if DEBUG
                waits = new string[] { "PAGE" };
                startsWith = true;
#endif
                if (!sm.WaitStatsMetrics.IsWaitHigh(waits, 10, startsWith)) { _logX.Debug("Exit PAGELATCH_EXSH analysis due to the waits not being high."); return; }
                WaitingDatabases wdbs = sm.WaitingBatchesMetrics.GetWaitingDatabases(waits, startsWith);
                if (null == wdbs) { _logX.Debug("Exit PAGELATCH_EXSH analysis due to no waiting databases being found."); return; }
                WaitingBatches wb = sm.WaitingBatchesMetrics.GetWaitingBatches(waits, startsWith);
                AffectedBatches ab = new AffectedBatches();
                if (null != wb) ab.AddRange(wb.GetAffectedBatches());
                string db;
                List<WaitingResource> waiting;
                foreach (WaitingDatabase wd in wdbs.Values)
                {
                    CheckCancel();
                    db = sm.Options.GetDatabaseName(wd.ID);
                    if (string.IsNullOrEmpty(db)) { _logX.DebugFormat("Could not find name of db {0}.", wd.ID); continue; }
                    if (sm.Options.IsDatabaseBlocked(db)) { _logX.DebugFormat("Skipping blocked db {0}.", db); continue; }
                    waiting = wd.GetSortedWaitingResources();
                    if (null == waiting) { _logX.DebugFormat("Could not find any waiting resources for db {0}.", db); continue; }
                    if (waiting.Count > 10)
                    {
                        _logX.DebugFormat("{0} waiting resources found for db {1}.  Truncating to top 10", waiting.Count, db);
                        waiting.RemoveRange(10, waiting.Count - 10);
                    }
                    if (0 != string.Compare(conn.Database, db))
                    {
                        _logX.DebugFormat("Changing connection to db {0}.", db);
                        try { conn.ChangeDatabase(db); }
                        catch (Exception ex) { ExceptionLogger.Log(_logX, string.Format("Error changing to db {0}", db), ex); }
                    }
                    foreach (WaitingResource wr in waiting)
                    {
                        CheckCancel();
                        PageId pid = new PageId(wr.DB, wr.File, wr.Page);
                        PageInfo pinfo = PageHelper.GetPageInfo(pid, conn);
                        if (null == pinfo) { _logX.DebugFormat("Failed to load page info for db {0} and page {1}!", db, pid); continue; }
                        if (pinfo.IsTable && !pinfo.IsMSShipped)
                        {
                            if ((pinfo.IndexId != 0) && (!string.IsNullOrEmpty(pinfo.IndexName)))
                            {
                                _logX.DebugFormat("Adding PageLatch EXSH recommendation for index [{0}].[{1}].[{2}].[{3}]", db, pinfo.SchemaName, pinfo.ObjectName, pinfo.IndexName);
                                AddRecommendation(new WaitStatsPageLatchIndexRecommendation(ab, db, pinfo.SchemaName, pinfo.ObjectName, pinfo.IndexId, pinfo.IndexName));
                            }
                            else
                            {
                                _logX.DebugFormat("Adding PageLatch EXSH recommendation for table [{0}].[{1}].[{2}]", db, pinfo.SchemaName, pinfo.ObjectName);
                                AddRecommendation(new WaitStatsPageLatchTableRecommendation(ab, db, pinfo.SchemaName, pinfo.ObjectName));
                            }
                        }
                        else
                        {
                            _logX.DebugFormat("Skipping page {0} for db {1}!", pinfo, db);
                        }
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------------------
        //  PAGELATCH_UP Occurs when a task is waiting for a latch for a buffer that is not in an I/O request. 
        //  
        //  The latch request is in Update mode.  
        //  
        //  Page latch Update is used only for allocation related pages, and contention on it is frequently a sign that 
        //  more files are needed. With multiple files, allocations can be distributed across multiple files therefore 
        //  reducing demand on the per-file data structures stored on these pages. The contention is not IO performance, 
        //  but internal allocation contention to access the pages. Adding more spindles to a file or moving the file to 
        //  a faster disk does not help, nor does adding more memory.
        //
        private void AnalyzePAGELATCH_UP(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzePAGELATCH_UP"))
            {
                if (!sm.WaitStatsMetrics.IsWaitHigh("PAGELATCH_UP", 5)) { _logX.Debug("Exit PAGELATCH_UP analysis due to the waits not being high."); return; }
                WaitingDatabases wdbs = sm.WaitingBatchesMetrics.GetWaitingDatabases("PAGELATCH_UP");
                if (null == wdbs) { _logX.Debug("Exit PAGELATCH_UP analysis due to no waiting databases being found."); return; }
                WaitingBatches wb = sm.WaitingBatchesMetrics.GetWaitingBatches("PAGELATCH_UP");
                AffectedBatches ab = new AffectedBatches();
                if (null != wb) ab.AddRange(wb.GetAffectedBatches());
                string db;
                foreach (WaitingDatabase wd in wdbs.Values)
                {
                    db = sm.Options.GetDatabaseName(wd.ID);
                    if (string.IsNullOrEmpty(db)) { _logX.DebugFormat("Could not find name of db {0}.", wd.ID); continue; }
                    if (sm.Options.IsDatabaseBlocked(db)) { _logX.DebugFormat("Skipping blocked db {0}.", db); continue; }
                    _logX.DebugFormat("Adding PageLatch UP recommendation to add more files for db [{0}].", db);
                    AddRecommendation(new WaitStatsPageLatchAllocPageContentionRecommendation(ab, db));
                }
            }
        }

        private void AnalyzePAGEIOLATCH_XX(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzePAGEIOLATCH_XX"))
            {
                if (!sm.WaitStatsMetrics.IsWaitHigh("PAGEIOLATCH_", 40, true)) { _logX.Debug("Exit PAGEIOLATCH_XX analysis due to the waits not being high."); return; }
                WaitingDatabases wdbs = sm.WaitingBatchesMetrics.GetWaitingDatabases("PAGEIOLATCH_", true);
                if (null == wdbs) { _logX.Debug("Exit PAGEIOLATCH_XX analysis due to no waiting databases being found."); return; }
                WaitingBatches wb = sm.WaitingBatchesMetrics.GetWaitingBatches("PAGEIOLATCH_", true);
                AffectedBatches ab = new AffectedBatches();
                if (null != wb) ab.AddRange(wb.GetAffectedBatches());
                string db;
                foreach (WaitingDatabase wd in wdbs.Values)
                {
                    if (SQLHelper.IsSystemDB(wd.ID)) { _logX.DebugFormat("Skipping system db {0}.", wd.ID); continue; }                    
                    db = sm.Options.GetDatabaseName(wd.ID);
                    if (string.IsNullOrEmpty(db)) { _logX.DebugFormat("Could not find name of db {0}.", wd.ID); continue; }
                    if (sm.Options.IsDatabaseBlocked(db)) { _logX.DebugFormat("Skipping blocked db {0}.", db); continue; }
                    _logX.DebugFormat("Adding PageIoLatch recommendation for db [{0}].", db);
                    AddRecommendation(new WaitStatsPageIoLatchDatabaseRecommendation(ab, db));
                }
            }
        }

        private void AnalyzeTHREADPOOL(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzeTHREADPOOL"))
            {
                if (!sm.WaitStatsMetrics.IsWaitHigh("THREADPOOL", 80)) { _logX.Debug("Exit THREADPOOL analysis due to the waits not being high."); return; }
                if (0 == sm.ServerPropertiesMetrics.MaxWorkerThreads) { _logX.Debug("Exit - MaxWorkerThreads set to zero."); return; }
                //--------------------------------------------------------------------------------------------------------------
                //  The following table shows the automatically configured number of max worker threads for various combinations
                //  of CPUs and versions of SQL Server.
                //
                //  Number of CPUs              32-bit computer      64-bit computer
                //  <= 4 processors                 256                 512
                //  8 processors                    288                 576
                //  16 processors                   352                 704
                //  32 processors                   480                 960
                //
                int cpu = sm.WMIPerfOSProcessorMetrics.GetProcessorCount(sm.ServerPropertiesMetrics.AffinityMask);
                UInt64 currentThreads = sm.ServerPropertiesMetrics.MaxWorkerThreads;
                UInt64 suggestedThreads = 0;
                bool is32bit = MemoryAnalyzer.Is32BitOS(sm, conn);
                if (cpu >= 32)
                {
                    if (is32bit) suggestedThreads = 480; else suggestedThreads = 960;
                }
                else if (cpu >= 16)
                {
                    if (is32bit) suggestedThreads = 352; else suggestedThreads = 704;
                }
                else if (cpu >= 8)
                {
                    if (is32bit) suggestedThreads = 288; else suggestedThreads = 576;
                }
                else
                {
                    if (is32bit) suggestedThreads = 256; else suggestedThreads = 512;
                }
                _logX.DebugFormat("Is32Bit:{0} MaxWorkerThreads:{1} Suggested:{2}", is32bit, currentThreads, suggestedThreads);
                if (suggestedThreads > currentThreads)
                {
                    WaitingBatches wb = sm.WaitingBatchesMetrics.GetWaitingBatches("THREADPOOL");
                    AffectedBatches ab = new AffectedBatches();
                    if (null != wb) ab.AddRange(wb.GetAffectedBatches());
                    _logX.Debug("Adding thread pool wait stat recommendation");
                    AddRecommendation(new WaitStatsThreadPoolRecommendation(ab, currentThreads, suggestedThreads));
                }
            }

        }
        private void AnalyzeASYNC_NETWORK_IO(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzeASYNC_NETWORK_IO"))
            {
                if (!sm.WaitStatsMetrics.IsWaitHigh("ASYNC_NETWORK_IO", 50)) { _logX.Debug("Exit ASYNC_NETWORK_IO analysis due to the waits not being high."); return; }

                WaitingBatches wb = sm.WaitingBatchesMetrics.GetWaitingBatches("ASYNC_NETWORK_IO");
                AffectedBatches ab = new AffectedBatches();
                if (null != wb) ab.AddRange(wb.GetAffectedBatches());
                if (ab.Count > 0)
                {
                    _logX.Debug("Adding async network io wait stat recommendation");
                    AddRecommendation(new WaitStatsAsyncNetIORecommendation(ab));
                }
                else
                {
                    _logX.Debug("Skipping async network io due to no batches being found");
                }
            }
        }

        private void LogTopWaits(SnapshotMetrics sm)
        {
            using (_logX.InfoCall("WaitStatsAnalyzer.LogTopWaits"))
            {
                foreach (WaitInfo wi in sm.WaitStatsMetrics.Stats.Top(5))
                {
                    LogWaits(sm, wi);
                }
            }
        }

        private void LogWaits(SnapshotMetrics sm, WaitInfo wi)
        {
            if (null == wi) return;
            using (_logX.DebugCall(string.Format("LogWaits({0})", wi)))
            {
                WaitingBatches wb = sm.WaitingBatchesMetrics.GetWaitingBatches(wi.Type);
                if (null == wb) { _logX.Debug("No waiting batches found."); return; }
                int count = 0;
                foreach (AffectedBatch b in wb.GetAffectedBatches())
                {
                    using (_logX.DebugCall(string.Format("Batch {0} Prg:{1}", ++count, b.Name)))
                    {
                        _logX.Debug(b.Batch);
                    }
                }
            }
        }

        private void AnalyzeCXPACKET(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzeCXPACKET"))
            {
                if (!sm.WaitStatsMetrics.IsWaitHigh("CXPACKET", 5)) { _logX.Debug("Exit CXPACKET analysis due to the waits not being high."); return; }
                //------------------------------------------------------------------------
                // For OLTP servers we want to recommend that MaxDOP is set to 1 to prevent parallelized queries.
                //
                // Information about well tuned oltp queries not being parallelized:
                // http://blogs.msdn.com/b/jimmymay/archive/2008/11/28/case-study-part-1-cxpacket-wait-stats-max-degree-of-parallelism-option-introduction-to-using-wait-stats-to-identify-remediate-query-parallelism-bottlenecks.aspx
                //
                if (sm.Options.OLTPServer)
                {
                    //------------------------------------------------------------------------
                    // If this is an OLTP server with high CPU, the processor analyzer will handle
                    // generating recommendations for the high CXPACKET waits and high CPU if needed.
                    //
                    if (ProcessorAnalyzer.IsHighCpu(sm)) return;
                }
                WaitingBatches wb = sm.WaitingBatchesMetrics.GetWaitingBatches("CXPACKET");
                AffectedBatches ab = new AffectedBatches();
                if (null != wb) ab.AddRange(wb.GetAffectedBatches());
                if (sm.WaitStatsMetrics.IsWaitHigh("PAGEIOLATCH_", 20, true))
                {
                    _logX.Debug("PAGEIOLATCH_ are high along with CXPACKET waits.");
                    wb = sm.WaitingBatchesMetrics.GetWaitingBatches("PAGEIOLATCH_", true);
                    if (null != wb) ab.AddRange(wb.GetAffectedBatches());

                    if (ab.Count > 0)
                    {
                        _logX.Debug("Add WaitStatsPageIoLatchCxPacketRecommendation");
                        AddRecommendation(new WaitStatsPageIoLatchCxPacketRecommendation(ab));
                        return;
                    }
                }
                if (sm.ServerPropertiesMetrics.ParallelismThreshold < 5)
                {
                    _logX.Debug("Add WaitStatsParallelismThresholdRecommendation");
                    AddRecommendation(new WaitStatsParallelismThresholdRecommendation(ab, sm.ServerPropertiesMetrics.ParallelismThreshold, 5));
                    return;
                }
                int cpu = sm.WMIPerfOSProcessorMetrics.GetProcessorCount(sm.ServerPropertiesMetrics.AffinityMask);
                bool hyperThreaded = false;
                if (null != sm.WMIProcessorMetrics)
                {
                    if (sm.WMIProcessorMetrics.TotalNumberOfCores > 0)
                    {
                        if (sm.WMIProcessorMetrics.TotalNumberOfCores < cpu)
                        {
                            _logX.DebugFormat("Total physical cores ({0}) less than SQL Server configured cpu ({1})", sm.WMIProcessorMetrics.TotalNumberOfCores, cpu);
                            cpu = (int)sm.WMIProcessorMetrics.TotalNumberOfCores;
                            hyperThreaded = true;
                        }
                    }
                    else
                    {
                        _logX.DebugFormat("Unable to determine if hyperthreading is enabled.  This is may be due to a limitation in Windows 2003.");
                    }
                }
                if ((sm.ServerPropertiesMetrics.MaxDOP > 1) ||
                    (0 == sm.ServerPropertiesMetrics.MaxDOP))
                {
                    int MaxOnlineSchedulerCount = 0;
                    if (10 == sm.ServerPropertiesMetrics.ServerVersion.Major)
                    {
                        SQLHelper.CheckConnection(conn);
                        using (SqlCommand MaxOnlineSchedulerCountCommand = new SqlCommand(Properties.Resources.MaxOnlineSchedulerCountQuery, conn))
                        {
                            MaxOnlineSchedulerCountCommand.CommandTimeout = Constants.DefaultCommandTimeout;
                            try
                            {
                                using (SqlDataReader sdr = MaxOnlineSchedulerCountCommand.ExecuteReader())
                                {
                                    sdr.Read();
                                    MaxOnlineSchedulerCount = SQLHelper.GetInt16(sdr, 0);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logX.Error("Query Max Online Scheduler Count failed: " + ex.Message);
                                MaxOnlineSchedulerCount = 0;
                            }
                        }
                    }

                    if ((0 == sm.ServerPropertiesMetrics.MaxDOP) ||
                        (sm.ServerPropertiesMetrics.MaxDOP > 8) ||
                        (sm.ServerPropertiesMetrics.MaxDOP > (cpu / 2)) ||
                        ((MaxOnlineSchedulerCount > 0) && (sm.ServerPropertiesMetrics.MaxDOP > MaxOnlineSchedulerCount)))
                    {
                        int suggestedMaxDOP = 1;
                        if (cpu > 1) suggestedMaxDOP = cpu / 2;
                        if ((MaxOnlineSchedulerCount > 0) && (MaxOnlineSchedulerCount < cpu)) suggestedMaxDOP = MaxOnlineSchedulerCount;   //if NUMA, then MaxDOP = # of procs in a NUMA node
                        if (suggestedMaxDOP > 8) suggestedMaxDOP = 8;
                        _logX.DebugFormat("Add WaitStatsMaxDOPRecommendation recommendation to set max dop from {0} to {1}", sm.ServerPropertiesMetrics.MaxDOP, suggestedMaxDOP);
                        AddRecommendation(new WaitStatsMaxDOPRecommendation(ab, sm.ServerPropertiesMetrics.MaxDOP, (UInt32)suggestedMaxDOP));
                        return;
                    }
                }
                _logX.Debug("Add WaitStatsHighCXPacketRecommendation recommendation due to high CXPACKET waits.");
                AddRecommendation(new WaitStatsHighCXPacketRecommendation(ab, hyperThreaded, sm.Options.OLTPServer));
            }
        }
    }
}
