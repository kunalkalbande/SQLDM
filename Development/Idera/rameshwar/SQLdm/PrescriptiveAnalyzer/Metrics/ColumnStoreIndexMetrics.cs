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
    public class ColumnStoreIndexMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("ColumnStoreIndexMetrics");
        public List<ColumnStoreIndexForDB> ColumnStoreIndexForDBs { get; set; }

        public ColumnStoreIndexMetrics()
        {
            ColumnStoreIndexForDBs = new List<ColumnStoreIndexForDB>();
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            if (snapshot.ColumnStoreIndexSnapshotList == null || snapshot.ColumnStoreIndexSnapshotList.Count == 0) { return; }
            foreach (ColumnStoreIndexSnapshot snap in snapshot.ColumnStoreIndexSnapshotList)
            {
                //Check for error free snap shot
                if (snap != null && snap.Error == null)
                {
                    if (snap.ColumnStoreIndexInfo != null && snap.ColumnStoreIndexInfo.Rows.Count > 0)
                    {
                        ColumnStoreIndexForDB objDB = new ColumnStoreIndexForDB();
                        for (int index = 0; index < snap.ColumnStoreIndexInfo.Rows.Count; index++)
                        {
                            ColumnStoreIndex obj = new ColumnStoreIndex();
                            try
                            {
                                obj.Database = (string)snap.ColumnStoreIndexInfo.Rows[index]["DatabaseName"];
                                obj.Table = (string)snap.ColumnStoreIndexInfo.Rows[index]["Table"];
                                obj.schema = (string)snap.ColumnStoreIndexInfo.Rows[index]["Schema"];
                            }
                            catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                            objDB.ColumnStoreIndexList.Add(obj);
                        }
                        ColumnStoreIndexForDBs.Add(objDB);
                    }
                    else
                    {
                        _logX.Error("ColumnStoreIndexMetrics not added : " + snap.Error);
                        continue;
                    }
                }
            }
        }
    }

    public class ColumnStoreIndexForDB
    {
        public List<ColumnStoreIndex> ColumnStoreIndexList { get; set; }

        public ColumnStoreIndexForDB()
        {
            ColumnStoreIndexList = new List<ColumnStoreIndex>();
        }
    }

    public class ColumnStoreIndex
    {
        public string Database { get; set; }
        public string schema { get; set; }
        public string Table { get; set; }
    }
}
