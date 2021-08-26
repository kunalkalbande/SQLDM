using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Values;
using Idera.SQLdm.Common.Snapshots;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class IndexContentionMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("IndexContentionMetrics");
        public List<IndexContentionForDB> IndexContentionForDBs { get; set; }

        public IndexContentionMetrics()
        {
            IndexContentionForDBs = new List<IndexContentionForDB>();
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            if (snapshot.IndexContentionSnapshotList == null || snapshot.IndexContentionSnapshotList.Count == 0) { return; }

            foreach (IndexContentionSnapshot snap in snapshot.IndexContentionSnapshotList)
            {
                //Check for error free snap shot
                if (snap != null && snap.Error == null)
                {
                    IndexContentionForDB objDB = new IndexContentionForDB();
                    if (snap.PageLatchIndexContention != null && snap.PageLatchIndexContention.Rows.Count > 0)
                    {
                        for (int index = 0; index < snap.PageLatchIndexContention.Rows.Count; index++)
                        {
                            PageLatch obj = new PageLatch();
                            try
                            {
                                obj.DatabaseName = (string)snap.PageLatchIndexContention.Rows[index]["DatabaseName"];
                                obj.TableName = (string)snap.PageLatchIndexContention.Rows[index]["TableName"];
                                obj.IndexName = (string)snap.PageLatchIndexContention.Rows[index]["IndexName"];
                                obj.Partition = (Int32)snap.PageLatchIndexContention.Rows[index]["Partition"];
                                obj.PageLatchWaitCount = (Int64)snap.PageLatchIndexContention.Rows[index]["PageLatchWaitCount"];
                                obj.PageLatchWaitInMs = (Int64)snap.PageLatchIndexContention.Rows[index]["PageLatchWaitInMs"];
                                obj.AvgPageLatchWaitInMs = (decimal)snap.PageLatchIndexContention.Rows[index]["AvgPageLatchWaitInMs"];
                                obj.schema = (string)snap.PageLatchIndexContention.Rows[index]["Schema"];
                            }
                            catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                            objDB.PageLatchList.Add(obj);
                        }
                    }
                    if (snap.PageLockIndexContention != null && snap.PageLockIndexContention.Rows.Count > 0)
                    {
                        for (int index = 0; index < snap.PageLockIndexContention.Rows.Count; index++)
                        {
                            PageLock obj = new PageLock();
                            try
                            {
                                obj.DatabaseName = (string)snap.PageLockIndexContention.Rows[index]["DatabaseName"];
                                obj.TableName = (string)snap.PageLockIndexContention.Rows[index]["TableName"];
                                obj.IndexName = (string)snap.PageLockIndexContention.Rows[index]["IndexName"];
                                obj.Partition = (Int32)snap.PageLockIndexContention.Rows[index]["Partition"];
                                obj.PageLockCount = (Int64)snap.PageLockIndexContention.Rows[index]["PageLockCount"];
                                obj.PageLockWaitCount = (Int64)snap.PageLockIndexContention.Rows[index]["PageLockWaitCount"];
                                obj.PageLockPercent = (decimal)snap.PageLockIndexContention.Rows[index]["PageLockPercent"];
                                obj.PageLockWaitInMs = (Int64)snap.PageLockIndexContention.Rows[index]["PageLockWaitInMs"];
                                obj.AvgPageLockWaitInMs = (decimal)snap.PageLockIndexContention.Rows[index]["AvgPageLockWaitInMs"];
                                obj.schema = (string)snap.PageLockIndexContention.Rows[index]["Schema"];
                            }
                            catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                            objDB.PageLockList.Add(obj);
                        }
                    }
                    if (snap.RowLockIndexContention != null && snap.RowLockIndexContention.Rows.Count > 0)
                    {
                        for (int index = 0; index < snap.RowLockIndexContention.Rows.Count; index++)
                        {
                            RowLock obj = new RowLock();
                            try
                            {
                                obj.DatabaseName = (string)snap.RowLockIndexContention.Rows[index]["DatabaseName"];
                                obj.TableName = (string)snap.RowLockIndexContention.Rows[index]["TableName"];
                                obj.IndexName = (string)snap.RowLockIndexContention.Rows[index]["IndexName"];
                                obj.Partition = (Int32)snap.RowLockIndexContention.Rows[index]["Partition"];
                                obj.RowLockCount = (Int64)snap.RowLockIndexContention.Rows[index]["RowLockCount"];
                                obj.RowLockWaitCount = (Int64)snap.RowLockIndexContention.Rows[index]["RowLockWaitCount"];
                                obj.RowLockPercent = (decimal)snap.RowLockIndexContention.Rows[index]["RowLockPercent"];
                                obj.RowLockWaitInMs = (Int64)snap.RowLockIndexContention.Rows[index]["RowLockWaitInMs"];
                                obj.AvgRowLockWaitInMs = (decimal)snap.RowLockIndexContention.Rows[index]["AvgRowLockWaitInMs"];
                                obj.schema = (string)snap.RowLockIndexContention.Rows[index]["Schema"];
                            }
                            catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                            objDB.RowLockList.Add(obj);
                        }
                    }
                    IndexContentionForDBs.Add(objDB);
                }
                else
                {
                    _logX.Error("IndexContentionMetrics not added : " + snap.Error);
                    continue;
                }
            }
        }
    }

    public class IndexContentionForDB
    {
        public List<PageLatch> PageLatchList { get; set; }
        public List<PageLock> PageLockList { get; set; }
        public List<RowLock> RowLockList { get; set; }

        public IndexContentionForDB()
        {
            PageLatchList = new List<PageLatch>();
            PageLockList = new List<PageLock>();
            RowLockList = new List<RowLock>();
        }
    }

    public class PageLatch
    {
        public string DatabaseName { get; set; }
        public string schema { get; set; }
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public Int32 Partition { get; set; }
        public Int64 PageLatchWaitInMs { get; set; }
        public Int64 PageLatchWaitCount { get; set; }
        public Decimal AvgPageLatchWaitInMs { get; set; }
    }

    public class PageLock
    {
        public string DatabaseName { get; set; }
        public string schema { get; set; }
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public Int32 Partition { get; set; }
        public Int64 PageLockWaitInMs { get; set; }
        public Int64 PageLockCount { get; set; }
        public Int64 PageLockWaitCount { get; set; }
        public Decimal PageLockPercent { get; set; }
        public Decimal AvgPageLockWaitInMs { get; set; }
    }

    public class RowLock
    {
        public string DatabaseName { get; set; }
        public string schema { get; set; }
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public Int32 Partition { get; set; }
        public Int64 RowLockWaitInMs { get; set; }
        public Int64 RowLockCount { get; set; }
        public Int64 RowLockWaitCount { get; set; }
        public Decimal RowLockPercent { get; set; }
        public Decimal AvgRowLockWaitInMs { get; set; }
    }
}
