using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using Idera.SQLdm.Common.Snapshots;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class HighIndexUpdatesMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("HighIndexUpdatesMetrics");
        public List<HighIndexUpdatesForDB> HighIndexUpdatesForDBs { get; set; }

        public HighIndexUpdatesMetrics()
        {
            HighIndexUpdatesForDBs = new List<HighIndexUpdatesForDB>();
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            if (snapshot.HighIndexUpdatesSnapshotList == null || snapshot.HighIndexUpdatesSnapshotList.Count == 0) { return; }
            foreach (HighIndexUpdatesSnapshot snap in snapshot.HighIndexUpdatesSnapshotList)
            {
                //Check for error free snap shot
                if (snap != null && snap.Error == null)
                {
                    if (snap.HighIndexUpdates != null && snap.HighIndexUpdates.Rows.Count > 0)
                    {
                        HighIndexUpdatesForDB objDB = new HighIndexUpdatesForDB();
                        for (int index = 0; index < snap.HighIndexUpdates.Rows.Count; index++)
                        {
                            HighIndexUpdates obj = new HighIndexUpdates();
                            try
                            {
                                obj.DatabaseName = (string)snap.HighIndexUpdates.Rows[index]["DatabaseName"];
                                obj.TableName = (string)snap.HighIndexUpdates.Rows[index]["TableName"];
                                obj.IndexName = (string)snap.HighIndexUpdates.Rows[index]["IndexName"];
                                obj.UserReads = (long)snap.HighIndexUpdates.Rows[index]["UserReads"];
                                obj.UserWrites = (long)snap.HighIndexUpdates.Rows[index]["UserWrites"];
                                obj.WritesPerRead = (decimal)snap.HighIndexUpdates.Rows[index]["WritesPerRead"];
                                obj.DaysSinceTableCreate = (long)snap.HighIndexUpdates.Rows[index]["DaysSinceTableCreated"];
                                obj.schema = (string)snap.HighIndexUpdates.Rows[index]["Schema"];
                            }
                            catch (Exception e)
                            {
                                _logX.Error(e);
                                IsDataValid = false;
                                return;
                            }
                            objDB.HighIndexUpdatesList.Add(obj);
                        }
                        HighIndexUpdatesForDBs.Add(objDB);
                    }
                }
                else
                {
                    _logX.Error("HighIndexUpdatesMetrics not added : " + snap.Error);
                    continue;
                }
            }
        }
    }

    public class HighIndexUpdatesForDB
    {
        public List<HighIndexUpdates> HighIndexUpdatesList { get; set; }

        public HighIndexUpdatesForDB()
        {
            HighIndexUpdatesList = new List<HighIndexUpdates>();
        }
    }

    public class HighIndexUpdates
    {
        public string DatabaseName { get; set; }
        public long UserReads { get; set; }
        public long UserWrites { get; set; }
        public long DaysSinceTableCreate { get; set; }
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public Decimal WritesPerRead { get; set; }
        public string schema { get; set; }
    }
}
