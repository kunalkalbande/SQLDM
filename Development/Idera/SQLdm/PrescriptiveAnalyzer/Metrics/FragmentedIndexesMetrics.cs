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
    public class FragmentedIndexesMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("FragmentedIndexesMetrics");
        public List<FragmentedIndexesForDB> FragmentedIndexesForDBs { get; set; }

        public FragmentedIndexesMetrics()
        {
            FragmentedIndexesForDBs = new List<FragmentedIndexesForDB>();
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            if (snapshot.FragmentedIndexesSnapshotList == null || snapshot.FragmentedIndexesSnapshotList.Count == 0) { return; }
            foreach (FragmentedIndexesSnapshot snap in snapshot.FragmentedIndexesSnapshotList)
            {
                //Check for error free snap shot
                if (snap != null && snap.Error == null)
                {
                    if (snap.FragmentedIndexes != null && snap.FragmentedIndexes.Rows.Count > 0)
                    {
                        FragmentedIndexesForDB objDB = new FragmentedIndexesForDB();
                        for (int index = 0; index < snap.FragmentedIndexes.Rows.Count; index++)
                        {
                            FragmentedIndex obj = new FragmentedIndex();
                            try
                            {
                                obj.DatabaseName = (string)snap.FragmentedIndexes.Rows[index]["DatabaseName"];
                                obj.TableName = (string)snap.FragmentedIndexes.Rows[index]["TableName"];
                                obj.IndexName = (string)snap.FragmentedIndexes.Rows[index]["IndexName"];
                                obj.Partition = (Int32)snap.FragmentedIndexes.Rows[index]["Partition"];
                                obj.FragPercent = (double)snap.FragmentedIndexes.Rows[index]["FragPercent"];
                                obj.PartitionPages = (long)snap.FragmentedIndexes.Rows[index]["PartitionPages"];
                                obj.TablePages = (long)snap.FragmentedIndexes.Rows[index]["TablePages"];
                                obj.TotalServerBufferPages = (long)snap.FragmentedIndexes.Rows[index]["TotalServerBufferPages"];
                                obj.schema = (string)snap.FragmentedIndexes.Rows[index]["Schema"];
                            }
                            catch (Exception e)
                            {
                                _logX.Error(e);
                                IsDataValid = false;
                                return;
                            }
                            objDB.FragmentedIndexes.Add(obj);
                        }
                        FragmentedIndexesForDBs.Add(objDB);
                    }

                }
                else
                {
                    _logX.Error("FragmentedIndexesMetrics not added : " + snap.Error);
                    continue;
                }
            }
        }
    }


    public class FragmentedIndexesForDB
    {
        public List<FragmentedIndex> FragmentedIndexes { get; set; }

        public FragmentedIndexesForDB()
        {
            FragmentedIndexes = new List<FragmentedIndex>();
        }
    }

    public class FragmentedIndex
    {
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public Int32 Partition { get; set; }
        public double FragPercent { get; set; }

        public long PartitionPages { get; set; }

        public long TablePages { get; set; }
        public long TotalServerBufferPages { get; set; }

        public string schema { get; set; }
    }
}
