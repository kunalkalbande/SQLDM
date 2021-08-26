using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data.SqlClient;
using BBS.TracerX;
//using Idera.SQLdoctor.AnalysisEngine.Batches;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    class IndexContentionAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 7;
        private static Logger _logX = Logger.GetLogger("IndexContentionAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public override string GetDescription() { return ("IndexContention analysis"); }

        public IndexContentionAnalyzer()
        {
            _id = id;
        }

        private Int64 _minThreshold;
        private UInt64 _serverUpSeconds;

        private delegate void ProcessRow(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn);

        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            if (sm == null) return;
            if (sm.IndexContentionMetrics == null) return;
            _minThreshold = 0;
            _serverUpSeconds = 0;
            double days = 0;
            if (sm.ServerPropertiesMetrics != null) 
            {
                days = sm.ServerPropertiesMetrics.MinutesRunning / (60 * 24);
                _serverUpSeconds = (UInt64)Math.Round((days * 24 * 60 * 60), 0);
                _minThreshold = (Int64)Math.Round((days * 24 * 60 * 60) * .1, 0);
            }

            _logX.InfoFormat("Index contention min wait time ms {0}.  Server has been running {1} days or a total of {2} seconds.", _minThreshold, days, _serverUpSeconds);

            foreach (IndexContentionForDB metrics in sm.IndexContentionMetrics.IndexContentionForDBs)
            {
                foreach (PageLatch snap in metrics.PageLatchList)
                {
                    if (string.IsNullOrEmpty(snap.DatabaseName)) continue;
                    ProcessPageLatch(snap, conn);
                }
                foreach (PageLock snap in metrics.PageLockList)
                {
                    if (string.IsNullOrEmpty(snap.DatabaseName)) continue;
                    ProcessPageLock(snap, conn);
                }
                foreach (RowLock snap in metrics.RowLockList)
                {
                    if (string.IsNullOrEmpty(snap.DatabaseName)) continue;
                    ProcessRowLock(snap, conn);
                }
            }
        }
        private void ProcessPageLatch(PageLatch snap, System.Data.SqlClient.SqlConnection conn)
        {
            Int64 waitsMs = snap.PageLatchWaitInMs;
            string db = snap.DatabaseName;
            var ir = new IndexPageLatchContentionRecommendation(
                                    db,
                                    snap.schema,
                                    snap.TableName,
                                    snap.IndexName,
                                    snap.Partition,
                                    snap.PageLatchWaitCount,
                                    waitsMs,
                                    snap.AvgPageLatchWaitInMs,
                                    _serverUpSeconds
                                );
            if (_minThreshold > waitsMs)
            {
                _logX.Info(string.Format("Skipped Page Latch due to threshold.  Min:{0}  Wait:{1}  Index: {2}", _minThreshold, waitsMs, ir));
                return;
            }
            AddRecommendation(ir);
        }
        private void ProcessPageLock(PageLock snap, System.Data.SqlClient.SqlConnection conn)
        {
            Int64 waitsMs = snap.PageLockWaitInMs;
            string db = snap.DatabaseName;
            var ir = new IndexPageLockContentionRecommendation(
                                    db,
                                    snap.schema,
                                    snap.TableName,
                                    snap.IndexName,
                                    snap.Partition,
                                    snap.PageLockCount,
                                    snap.PageLockWaitCount,
                                    snap.PageLockPercent,
                                    waitsMs,
                                    snap.AvgPageLockWaitInMs,
                                    _serverUpSeconds
                                );
            if (_minThreshold > waitsMs)
            {
                _logX.Info(string.Format("Skipped Page Lock due to threshold.  Min:{0}  Wait:{1}  Index: {2}", _minThreshold, waitsMs, ir));
                return;
            }
            AddRecommendation(ir);
        }
        private void ProcessRowLock(RowLock snap, System.Data.SqlClient.SqlConnection conn)
        {
            Int64 waitsMs = snap.RowLockWaitInMs;
            string db = snap.DatabaseName;
            var ir = new IndexRowLockContentionRecommendation(
                                    db,
                                    snap.schema,
                                    snap.TableName,
                                    snap.IndexName,
                                    snap.Partition,
                                    snap.RowLockCount,
                                    snap.RowLockWaitCount,
                                    snap.RowLockPercent,
                                    waitsMs,
                                    snap.AvgRowLockWaitInMs,
                                    _serverUpSeconds
                                );
            if (_minThreshold > waitsMs)
            {
                _logX.Info(string.Format("Skipped Row Lock due to threshold.  Min:{0}  Wait:{1}  Index: {2}", _minThreshold, waitsMs, ir));
                return;
            }
            AddRecommendation(ir);
        }
    }
}
