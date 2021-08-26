using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Values;
using Idera.SQLdm.Common.Snapshots;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class OverlappingIndexesMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("OverlappingIndexesMetrics");
        public List<OverlappingIndexForDB> OverlappingIndexForDBs { get; set; }

        public OverlappingIndexesMetrics()
        {
            OverlappingIndexForDBs = new List<OverlappingIndexForDB>();
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            if (snapshot.OverlappingIndexesSnapshotList == null || snapshot.OverlappingIndexesSnapshotList.Count == 0) { return; }
            foreach (OverlappingIndexesSnapshot snap in snapshot.OverlappingIndexesSnapshotList)
            {
                //Check for error free snap shot
                if (snap != null && snap.Error == null)
                {
                    OverlappingIndexForDB objDB = new OverlappingIndexForDB();
                    if (snap.DuplicateIndexInfo != null && snap.DuplicateIndexInfo.Rows.Count > 0)
                    {
                        for (int index = 0; index < snap.DuplicateIndexInfo.Rows.Count; index++)
                        {
                            OverlappingIndex obj = new OverlappingIndex();
                            try
                            {
                                obj.DupIndex = snap.DuplicateIndexInfo.Rows[index]["DupIndex"].ToString() == "true" ? true : false;
                                obj.DatabaseName = (string)snap.DuplicateIndexInfo.Rows[index]["DatabaseName"];
                                obj.TableName = (string)snap.DuplicateIndexInfo.Rows[index]["TableName"];
                                obj.IndexName = (string)snap.DuplicateIndexInfo.Rows[index]["IndexName"];
                                obj.IndexId = (Int32)snap.DuplicateIndexInfo.Rows[index]["IndexId"];//
                                obj.IndexUnique = (bool)snap.DuplicateIndexInfo.Rows[index]["IndexUnique"];
                                obj.IndexPrimaryKey = (bool)snap.DuplicateIndexInfo.Rows[index]["IndexPrimaryKey"];
                                obj.IndexUsage = (long)snap.DuplicateIndexInfo.Rows[index]["IndexUsage"];
                                obj.IndexUpdates = (long)snap.DuplicateIndexInfo.Rows[index]["IndexUpdates"];
                                obj.IndexKeySize = (Int32)snap.DuplicateIndexInfo.Rows[index]["IndexKeySize"];//
                                obj.IndexForeignKeys = (Int32)snap.DuplicateIndexInfo.Rows[index]["IndexForeignKeys"];//
                                obj.DupIndexName = (string)snap.DuplicateIndexInfo.Rows[index]["DupIndexName"];
                                obj.DupIndexId = (int)snap.DuplicateIndexInfo.Rows[index]["DupIndexId"];
                                obj.DupIndexUnique = (bool)snap.DuplicateIndexInfo.Rows[index]["DupIndexUnique"];
                                obj.DupIndexPrimaryKey = (bool)snap.DuplicateIndexInfo.Rows[index]["DupIndexPrimaryKey"];
                                obj.DupIndexUsage = (long)snap.DuplicateIndexInfo.Rows[index]["DupIndexUsage"];
                                obj.DupIndexUpdates = (long)snap.DuplicateIndexInfo.Rows[index]["DupIndexUpdates"];
                                obj.DupIndexKeySize = (Int32)snap.DuplicateIndexInfo.Rows[index]["DupIndexKeySize"];//
                                obj.DupIndexForeignKeys = (Int32)snap.DuplicateIndexInfo.Rows[index]["DupIndexForeignKeys"];//
                                obj.schema = (string)snap.DuplicateIndexInfo.Rows[index]["Schema"];
                                obj.IndexIncludeCols = string.Empty;
                                obj.IndexCols = string.Empty;
                                obj.DupIndexIncludeCols = string.Empty;
                                obj.DupIndexCols = string.Empty;
                            }
                            catch (Exception e)
                            {
                                _logX.Error(e);
                                IsDataValid = false;
                                return;
                            }
                            objDB.OverlappingIndexList.Add(obj);
                        }
                    }
                    if (snap.PartialDuplicateIndexInfo != null && snap.PartialDuplicateIndexInfo.Rows.Count > 0)
                    {
                        for (int index = 0; index < snap.PartialDuplicateIndexInfo.Rows.Count; index++)
                        {
                            OverlappingIndex obj = new OverlappingIndex();
                            try
                            {
                                obj.DupIndex = snap.PartialDuplicateIndexInfo.Rows[index]["DupIndex"].ToString() == "true" ? true : false;
                                obj.DatabaseName = (string)snap.PartialDuplicateIndexInfo.Rows[index]["DatabaseName"];
                                obj.TableName = (string)snap.PartialDuplicateIndexInfo.Rows[index]["TableName"];
                                obj.IndexName = (string)snap.PartialDuplicateIndexInfo.Rows[index]["IndexName"];
                                obj.IndexId = (Int32)snap.PartialDuplicateIndexInfo.Rows[index]["IndexId"];//
                                obj.IndexUnique = (bool)snap.PartialDuplicateIndexInfo.Rows[index]["IndexUnique"];
                                obj.IndexPrimaryKey = (bool)snap.PartialDuplicateIndexInfo.Rows[index]["IndexPrimaryKey"];
                                obj.IndexUsage = (long)snap.PartialDuplicateIndexInfo.Rows[index]["IndexUsage"];
                                obj.IndexUpdates = (long)snap.PartialDuplicateIndexInfo.Rows[index]["IndexUpdates"];
                                obj.IndexKeySize = (long)snap.PartialDuplicateIndexInfo.Rows[index]["IndexKeySize"];
                                obj.IndexForeignKeys = (Int32)snap.PartialDuplicateIndexInfo.Rows[index]["IndexForeignKeys"];//
                                obj.DupIndexName = (string)snap.PartialDuplicateIndexInfo.Rows[index]["DupIndexName"];
                                obj.DupIndexId = (int)snap.PartialDuplicateIndexInfo.Rows[index]["DupIndexId"];
                                obj.DupIndexUnique = (bool)snap.PartialDuplicateIndexInfo.Rows[index]["DupIndexUnique"];
                                obj.DupIndexPrimaryKey = (bool)snap.PartialDuplicateIndexInfo.Rows[index]["DupIndexPrimaryKey"];
                                obj.DupIndexUsage = (long)snap.PartialDuplicateIndexInfo.Rows[index]["DupIndexUsage"];
                                obj.DupIndexUpdates = (long)snap.PartialDuplicateIndexInfo.Rows[index]["DupIndexUpdates"];
                                obj.DupIndexKeySize = (long)snap.PartialDuplicateIndexInfo.Rows[index]["DupIndexKeySize"];
                                obj.DupIndexForeignKeys = (Int32)snap.PartialDuplicateIndexInfo.Rows[index]["DupIndexForeignKeys"];//
                                obj.IndexIncludeCols = (string)snap.PartialDuplicateIndexInfo.Rows[index]["IndexIncludeCols"];
                                obj.IndexCols = (string)snap.PartialDuplicateIndexInfo.Rows[index]["IndexCols"];
                                obj.DupIndexIncludeCols = (string)snap.PartialDuplicateIndexInfo.Rows[index]["DupIndexIncludeCols"];
                                obj.DupIndexCols = (string)snap.PartialDuplicateIndexInfo.Rows[index]["DupIndexCols"];
                                obj.schema = (string)snap.PartialDuplicateIndexInfo.Rows[index]["Schema"];
                            }
                            catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                            objDB.OverlappingIndexList.Add(obj);
                        }
                    }
                    OverlappingIndexForDBs.Add(objDB);

                }
                else
                {
                    _logX.Error("OverlappingIndexesMetrics not added : " + snap.Error);
                    continue;
                }
            }
        }
    }

    public class OverlappingIndexForDB
    {
        public List<OverlappingIndex> OverlappingIndexList { get; set; }

        public OverlappingIndexForDB()
        {
            OverlappingIndexList = new List<OverlappingIndex>();
        }
    }

    public class OverlappingIndex
    {
        public string DatabaseName { get; set; }
        public string schema { get; set; }
        public bool DupIndex { get; set; }
        public string TableName { get; set; }
        public bool DupIndexUnique { get; set; }
        public bool IndexUnique { get; set; }
        public bool DupIndexPrimaryKey { get; set; }
        public bool IndexPrimaryKey { get; set; }
        public long IndexId { get; set; }
        public long IndexForeignKeys { get; set; }
        public long DupIndexForeignKeys { get; set; }
        public long IndexKeySize { get; set; }
        public long DupIndexKeySize { get; set; }

        public string IndexIncludeCols { get; set; }
        public string DupIndexIncludeCols { get; set; }

        public string IndexCols { get; set; }
        public string DupIndexCols { get; set; }

        public string IndexName { get; set; }
        public long IndexUsage { get; set; }
        public long IndexUpdates { get; set; }

        public int DupIndexId { get; set; }
        public string DupIndexName { get; set; }
        public long DupIndexUsage { get; set; }
        public long DupIndexUpdates { get; set; }
    }
}
