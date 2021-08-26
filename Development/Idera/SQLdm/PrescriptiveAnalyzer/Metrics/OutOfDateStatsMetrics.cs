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
    public class OutOfDateStatsMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("OutOfDateStatsMetrics");
        public List<OutOfDateStatsForDB> OutOfDateStatsForDBs { get; set; }

        public OutOfDateStatsMetrics()
        {
            OutOfDateStatsForDBs = new List<OutOfDateStatsForDB>();
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            if (snapshot.OutOfDateStatsSnapshotList == null || snapshot.OutOfDateStatsSnapshotList.Count == 0) { return; }
            foreach (OutOfDateStatsSnapshot snap in snapshot.OutOfDateStatsSnapshotList)
            {
                //Check for error free snap shot
                if (snap != null && snap.Error == null)
                {
                    if (snap.OutOfDateStatsInfo != null && snap.OutOfDateStatsInfo.Rows.Count > 0)
                    {
                        OutOfDateStatsForDB objDB = new OutOfDateStatsForDB();
                        for (int index = 0; index < snap.OutOfDateStatsInfo.Rows.Count; index++)
                        {
                            OutOfDateStats obj = new OutOfDateStats();
                            try
                            {
                                obj.Database = (string)snap.OutOfDateStatsInfo.Rows[index]["DatabaseName"];
                                obj.Table = (string)snap.OutOfDateStatsInfo.Rows[index]["Table"];
                                obj.Name = (string)snap.OutOfDateStatsInfo.Rows[index]["Name"];
                                obj.RowCount = (UInt64)snap.OutOfDateStatsInfo.Rows[index]["RowCount"];
                                obj.ModCount = (UInt64)snap.OutOfDateStatsInfo.Rows[index]["ModCount"];
                                obj.StatsDate = (DateTime)snap.OutOfDateStatsInfo.Rows[index]["StatsDate"];
                                obj.HoursSinceUpdated = (UInt64)snap.OutOfDateStatsInfo.Rows[index]["HoursSinceUpdated"];
                                obj.schema = (string)snap.OutOfDateStatsInfo.Rows[index]["Schema"];
                            }
                            catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                            objDB.OutOfDateStatsList.Add(obj);
                        }
                        OutOfDateStatsForDBs.Add(objDB);
                    }
                    else
                    {
                        _logX.Error("OutOfDateStatsMetrics not added : " + snap.Error);
                        continue;
                    }
                }
            }
        }
    }

    public class OutOfDateStatsForDB
    {
        public List<OutOfDateStats> OutOfDateStatsList { get; set; }

        public OutOfDateStatsForDB()
        {
            OutOfDateStatsList = new List<OutOfDateStats>();
        }
    }

    public class OutOfDateStats
    {
        public string Database { get; set; }
        public string schema { get; set; }
        public string Table { get; set; }
        public string Name { get; set; }
        public UInt64 RowCount { get; set; }
        public UInt64 ModCount { get; set; }
        public DateTime StatsDate { get; set; }
        public UInt64 HoursSinceUpdated { get; set; }
    }
}
