using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using System.Data;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WorstIndexFillFactorMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("WorstIndexFillFactorMetrics");
        private List<WorstDatabaseFillFactorSnapshot> _indexes = new List<WorstDatabaseFillFactorSnapshot>();

        public IEnumerable<WorstDatabaseFillFactorSnapshot> Find(int filFactorThreshold)
        {
            List<WorstDatabaseFillFactorSnapshot> list = new List<WorstDatabaseFillFactorSnapshot>();
            foreach (WorstDatabaseFillFactorSnapshot snap in _indexes)
            {
                if (snap.FillFactor < filFactorThreshold)
                {
                    list.Add(snap);
                }
            }
            return list;
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("WorstIndexFillFactorMetrics not added : " + snapshot.Error); return; }
            if (snapshot.WorstFillFactorIndexesSnapshotValueShutdown == null) {  return; }
            if (snapshot.WorstFillFactorIndexesSnapshotValueShutdown.WorstFillFactorIndexes != null && snapshot.WorstFillFactorIndexesSnapshotValueShutdown.WorstFillFactorIndexes.Rows.Count > 0)
            {
                for (int index = 0; index < snapshot.WorstFillFactorIndexesSnapshotValueShutdown.WorstFillFactorIndexes.Rows.Count; index++)
                {
                    WorstDatabaseFillFactorSnapshot obj = new WorstDatabaseFillFactorSnapshot();
                    try
                    {
                        obj.DatabaseName = (string)snapshot.WorstFillFactorIndexesSnapshotValueShutdown.WorstFillFactorIndexes.Rows[index]["DatabaseName"];
                        obj.TableName = (string)snapshot.WorstFillFactorIndexesSnapshotValueShutdown.WorstFillFactorIndexes.Rows[index]["TableName"];
                        obj.IndexName = (string)snapshot.WorstFillFactorIndexesSnapshotValueShutdown.WorstFillFactorIndexes.Rows[index]["IndexName"];
                        obj.SchemaName = (string)snapshot.WorstFillFactorIndexesSnapshotValueShutdown.WorstFillFactorIndexes.Rows[index]["SchemaName"];
                        obj.FillFactor = (int)snapshot.WorstFillFactorIndexesSnapshotValueShutdown.WorstFillFactorIndexes.Rows[index]["FillFactor"];
                        obj.DataSizeMB = (int)snapshot.WorstFillFactorIndexesSnapshotValueShutdown.WorstFillFactorIndexes.Rows[index]["DataSizeInMB"];
                        obj.IndexsizeMB = (int)snapshot.WorstFillFactorIndexesSnapshotValueShutdown.WorstFillFactorIndexes.Rows[index]["IndexSizeInMB"];
                    }
                    catch
                    {
                        IsDataValid = false;
                        return;
                    }
                    _indexes.Add(obj);
                }
            }
        }

    }

    public class WorstDatabaseFillFactorSnapshot
    {
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public string SchemaName { get; set; }
        public int FillFactor { get; set; }
        public int DataSizeMB { get; set; }
        public int IndexsizeMB { get; set; }
    }
}
