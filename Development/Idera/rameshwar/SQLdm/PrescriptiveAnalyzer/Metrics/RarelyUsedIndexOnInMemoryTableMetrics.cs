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
    public class RarelyUsedIndexOnInMemoryTableMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("RarelyUsedIndexOnInMemoryTableMetrics");
        public string Edition { get; set; }
        public List<RarelyUsedIndexOnInMemoryTableForDB> RarelyUsedIndexOnInMemoryTableForDBs { get; set; }

        public RarelyUsedIndexOnInMemoryTableMetrics()
        {
            RarelyUsedIndexOnInMemoryTableForDBs = new List<RarelyUsedIndexOnInMemoryTableForDB>();
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            if (snapshot.RarelyUsedIndexOnInMemoryTableSnapshotList == null || snapshot.RarelyUsedIndexOnInMemoryTableSnapshotList.Count == 0) { return; }
            foreach (RarelyUsedIndexOnInMemoryTableSnapshot snap in snapshot.RarelyUsedIndexOnInMemoryTableSnapshotList)
            {
                //Check for error free snap shot
                if (snap != null && snap.Error == null)
                {
                    if (snap.RarelyUsedIndexOnInMemoryTableInfo != null && snap.RarelyUsedIndexOnInMemoryTableInfo.Rows.Count > 0)
                    {
                        RarelyUsedIndexOnInMemoryTableForDB objDB = new RarelyUsedIndexOnInMemoryTableForDB();
                        for (int index = 0; index < snap.RarelyUsedIndexOnInMemoryTableInfo.Rows.Count; index++)
                        {
                            RarelyUsedIndexOnInMemoryTable obj = new RarelyUsedIndexOnInMemoryTable();
                            try
                            {
                                obj.Database = (string)snap.RarelyUsedIndexOnInMemoryTableInfo.Rows[index]["dbname"];
                                obj.TableName = (string)snap.RarelyUsedIndexOnInMemoryTableInfo.Rows[index]["TableName"];
                                obj.SchemaName = (string)snap.RarelyUsedIndexOnInMemoryTableInfo.Rows[index]["SchemaName"];
                                obj.IndexName = (string)snap.RarelyUsedIndexOnInMemoryTableInfo.Rows[index]["IndexName"];
                                obj.rows_returned = (long)snap.RarelyUsedIndexOnInMemoryTableInfo.Rows[index]["rows_returned"];
                                obj.scans_started = (long)snap.RarelyUsedIndexOnInMemoryTableInfo.Rows[index]["scans_started"];
                            }
                            catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                            objDB.RarelyUsedIndexOnInMemoryTableList.Add(obj);
                        }
                        RarelyUsedIndexOnInMemoryTableForDBs.Add(objDB);
                    }
                    else
                    {
                        _logX.Error("RarelyUsedIndexOnInMemoryTableMetrics not added : " + snap.Error);
                        continue;
                    }
                }
            }
        }
    }

    public class RarelyUsedIndexOnInMemoryTableForDB
    {
        public List<RarelyUsedIndexOnInMemoryTable> RarelyUsedIndexOnInMemoryTableList { get; set; }

        public RarelyUsedIndexOnInMemoryTableForDB()
        {
            RarelyUsedIndexOnInMemoryTableList = new List<RarelyUsedIndexOnInMemoryTable>();
        }
    }

    public class RarelyUsedIndexOnInMemoryTable
    {
        public string Database { get; set; }
        public string TableName { get; set; }
        public string SchemaName { get; set; }
        public string IndexName { get; set; }
        public long rows_returned { get; set; }
        public long scans_started { get; set; }
    }
}
