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
    public class HashIndexMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("HashIndexMetrics");
        public string Edition { get; set; }
        public List<HashIndexForDB> HashIndexForDBs { get; set; }

        private HashIndexForDB objDB = new HashIndexForDB();

        public HashIndexMetrics()
        {
            HashIndexForDBs = new List<HashIndexForDB>();
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            if (snapshot.HashIndexSnapshotList == null || snapshot.HashIndexSnapshotList.Count == 0) { return; }
            foreach (HashIndexSnapshot snap in snapshot.HashIndexSnapshotList)
            {
                //Check for error free snap shot
                if (snap != null && snap.Error == null)
                {
                    objDB = new HashIndexForDB();
                    AddHashIndexSnapshot(snap);
                    AddScannedHashIndexSnapshot(snap);

                    HashIndexForDBs.Add(objDB);
                }
            }
        }
        private void AddHashIndexSnapshot(HashIndexSnapshot snap)
        {
            if (snap.HashIndexInfo != null && snap.HashIndexInfo.Rows.Count > 0)
            {
                for (int index = 0; index < snap.HashIndexInfo.Rows.Count; index++)
                {
                    HashIndex obj = new HashIndex();
                    try
                    {
                        obj.DatabaseName = (string)snap.HashIndexInfo.Rows[index]["DatabaseName"];
                        obj.TableName = (string)snap.HashIndexInfo.Rows[index]["TableName"];
                        obj.SchemaName = (string)snap.HashIndexInfo.Rows[index]["SchemaName"];
                        obj.IndexName = (string)snap.HashIndexInfo.Rows[index]["IndexName"];
                        obj.total_bucket_count = Convert.ToInt64(snap.HashIndexInfo.Rows[index]["total_bucket_count"]);
                        obj.empty_bucket_count = Convert.ToInt64(snap.HashIndexInfo.Rows[index]["empty_bucket_count"]);
                        obj.EmptyBucketPercent = Convert.ToDouble(snap.HashIndexInfo.Rows[index]["EmptyBucketPercent"]);
                        obj.avg_chain_length = Convert.ToInt64(snap.HashIndexInfo.Rows[index]["avg_chain_length"]);
                        obj.max_chain_length = Convert.ToInt64(snap.HashIndexInfo.Rows[index]["max_chain_length"]);
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                    objDB.HashIndexList.Add(obj);
                }
            }
            else
            {
                _logX.Error("HashIndexMetrics  not added : " + snap.Error);
                //continue;
            }
        }
        private void AddScannedHashIndexSnapshot(HashIndexSnapshot snap)
        {
            if (snap.ScannedHashIndexInfo != null && snap.ScannedHashIndexInfo.Rows.Count > 0)
            {
                for (int index = 0; index < snap.ScannedHashIndexInfo.Rows.Count; index++)
                {
                    ScannedHashIndex obj = new ScannedHashIndex();
                    try
                    {
                        obj.ScannedDatabaseName = (string)snap.HashIndexInfo.Rows[index]["DatabaseName"];
                        obj.ScannedTableName = (string)snap.HashIndexInfo.Rows[index]["TableName"];
                        obj.ScannedSchemaName = (string)snap.HashIndexInfo.Rows[index]["SchemaName"];
                        obj.ScannedIndexName = (string)snap.HashIndexInfo.Rows[index]["IndexName"];
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                    objDB.ScannedHashIndexList.Add(obj);
                }
            }
            else
            {
                _logX.Error("HashIndexMetrics for ScannedHashIndex  not added : " + snap.Error);
                //continue;
            }
        }
    }

    public class HashIndexForDB
    {
        public List<HashIndex> HashIndexList { get; set; }
        public List<ScannedHashIndex> ScannedHashIndexList { get; set; }

        public HashIndexForDB()
        {
            HashIndexList = new List<HashIndex>();
            ScannedHashIndexList = new List<ScannedHashIndex>();
        }

    }

    public class HashIndex
    {
        public string DatabaseName { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public long total_bucket_count { get; set; }
        public long empty_bucket_count { get; set; }
        public double EmptyBucketPercent { get; set; }
        public long avg_chain_length { get; set; }
        public long max_chain_length { get; set; }
    }
    public class ScannedHashIndex
    {
        public string ScannedDatabaseName { get; set; }
        public string ScannedSchemaName { get; set; }
        public string ScannedTableName { get; set; }
        public string ScannedIndexName { get; set; }
    }
}
