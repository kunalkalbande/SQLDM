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
    public class DisabledIndexMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("DisabledIndexMetrics");
        public List<DisabledIndexesForDB> DisabledIndexesForDBs { get; set; }

        public DisabledIndexMetrics()
        {
            DisabledIndexesForDBs = new List<DisabledIndexesForDB>();
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            if (snapshot.DisabledIndexesSnapshotList == null || snapshot.DisabledIndexesSnapshotList.Count == 0) { return; }
            foreach (DisabledIndexesSnapshot snap in snapshot.DisabledIndexesSnapshotList)
            {
                if (snap.DisabledIndexes != null && snap.DisabledIndexes.Rows.Count > 0)
                {
                    DisabledIndexesForDB objDB = new DisabledIndexesForDB();
                    for (int index = 0; index < snap.DisabledIndexes.Rows.Count; index++)
                    {
                        DisabledIndex obj = new DisabledIndex();
                        try
                        {
                            obj.DatabaseName = (string)snap.DisabledIndexes.Rows[index]["DatabaseName"];
                            obj.TableName = (string)snap.DisabledIndexes.Rows[index]["TableName"];
                            obj.IndexName = (string)snap.DisabledIndexes.Rows[index]["IndexName"];
                            obj.IndexId = Convert.ToInt32(snap.DisabledIndexes.Rows[index]["index_id"].ToString());
                            obj.IsDisabled = (bool)snap.DisabledIndexes.Rows[index]["is_disabled"];
                            obj.IsHypothetical = (bool)snap.DisabledIndexes.Rows[index]["is_hypothetical"];
                            obj.schema = (string)snap.DisabledIndexes.Rows[index]["Schema"];
                        }
                        catch(Exception e)
                        {
                            _logX.Error(e);
                            IsDataValid = false;
                            return;
                        }
                        objDB.DisabledIndexes.Add(obj);
                    }
                    DisabledIndexesForDBs.Add(objDB);
                }
            }
        }
    }

    public class DisabledIndexesForDB
    {
        public List<DisabledIndex> DisabledIndexes { get; set; }

        public DisabledIndexesForDB()
        {
            DisabledIndexes = new List<DisabledIndex>();
        }
    }

    public class DisabledIndex
    {
        public string schema { get; set; }
        public string DatabaseName { get; set; }
        public int ObjectID { get; set; }
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public int IndexId { get; set; }
        public Boolean IsDisabled { get; set; }
        public Boolean IsHypothetical { get; set; }
    }
}
