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
    public class LargeTableStatsMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("LargeTableStatsMetrics");
        public List<LargeTableStatsForDB> LargeTableStatsForDBs { get; set; }

        public LargeTableStatsMetrics()
        {
            LargeTableStatsForDBs = new List<LargeTableStatsForDB>();
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            if (snapshot.LargeTableStatsSnapshotList == null || snapshot.LargeTableStatsSnapshotList.Count == 0) { return; }
            foreach (LargeTableStatsSnapshot snap in snapshot.LargeTableStatsSnapshotList)
            {
                //Check for error free snap shot
                if (snap != null && snap.Error == null)
                {
                    if (snap.LargeTableStatsInfo != null && snap.LargeTableStatsInfo.Rows.Count > 0)
                    {
                        LargeTableStatsForDB objDB = new LargeTableStatsForDB();
                        for (int index = 0; index < snap.LargeTableStatsInfo.Rows.Count; index++)
                        {
                            LargeTableStats obj = new LargeTableStats();
                            try
                            {
                                obj.Database = (string)snap.LargeTableStatsInfo.Rows[index]["DatabaseName"];
                                obj.Table = (string)snap.LargeTableStatsInfo.Rows[index]["Table"];
                                obj.schema = (string)snap.LargeTableStatsInfo.Rows[index]["Schema"];
                            }
                            catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                            objDB.LargeTableStatsList.Add(obj);
                        }
                        LargeTableStatsForDBs.Add(objDB);
                    }
                    else
                    {
                        _logX.Error("LargeTableStatsMetrics not added : " + snap.Error);
                        continue;
                    }
                }
            }
        }
    }

    public class LargeTableStatsForDB
    {
        public List<LargeTableStats> LargeTableStatsList { get; set; }

        public LargeTableStatsForDB()
        {
            LargeTableStatsList = new List<LargeTableStats>();
        }
    }

    public class LargeTableStats
    {
        public string Database { get; set; }
        public string schema { get; set; }
        public string Table { get; set; }
    }
}
