using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Values;
using Idera.SQLdm.Common.Snapshots;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class NonIncrementalColStatMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("NonIncrementalColStatMetrics");
        public string Edition { get; set; }
        public List<NonIncrementalColumnStatForDB> NonIncrementalColumnStatsForDBs { get; set; }

        public NonIncrementalColStatMetrics()
        {
            NonIncrementalColumnStatsForDBs = new List<NonIncrementalColumnStatForDB>();
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            if (snapshot.NonIncrementalColStatsSnapshotList == null || snapshot.NonIncrementalColStatsSnapshotList.Count == 0) { return; }
            foreach (NonIncrementalColumnStatSnapshot snap in snapshot.NonIncrementalColStatsSnapshotList)
            {
                //Check for error free snap shot
                if (snap != null && snap.Error == null)
                {
                    if (snap.NonIncrementalColumnStatInfo != null && snap.NonIncrementalColumnStatInfo.Rows.Count > 0)
                    {
                        NonIncrementalColumnStatForDB objDB = new NonIncrementalColumnStatForDB();
                        for (int index = 0; index < snap.NonIncrementalColumnStatInfo.Rows.Count; index++)
                        {
                            NonIncrementalColumnStats obj = new NonIncrementalColumnStats();
                            try
                            {
                                obj.Database = (string)snap.NonIncrementalColumnStatInfo.Rows[index]["Database"];
                                obj.TableName = (string)snap.NonIncrementalColumnStatInfo.Rows[index]["TableName"];
                                obj.StateName = (string)snap.NonIncrementalColumnStatInfo.Rows[index]["StateName"];
                                obj.OptiScript = (string)snap.NonIncrementalColumnStatInfo.Rows[index]["OptiScript"];
                                obj.UndoScript = (string)snap.NonIncrementalColumnStatInfo.Rows[index]["UndoScript"];
                            }
                            catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                            objDB.NonIncrementalStatsList.Add(obj);
                        }
                        NonIncrementalColumnStatsForDBs.Add(objDB);
                    }
                    else
                    {
                        _logX.Error("NonIncrementalStatMetrics not added : " + snap.Error);
                        continue;
                    }
                }
            }
        }
    }

    public class NonIncrementalColumnStatForDB
    {
        public List<NonIncrementalColumnStats> NonIncrementalStatsList { get; set; }

        public NonIncrementalColumnStatForDB()
        {
            NonIncrementalStatsList = new List<NonIncrementalColumnStats>();
        }
    }

    public class NonIncrementalColumnStats
    {
        public string Database { get; set; }
        public string TableName { get; set; }
        public string StateName { get; set; }
        public string OptiScript { get; set; }
        public string UndoScript { get; set; }
    }
}
