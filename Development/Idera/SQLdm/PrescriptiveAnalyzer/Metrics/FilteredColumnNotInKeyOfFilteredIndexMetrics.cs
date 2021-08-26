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
    public class FilteredColumnNotInKeyOfFilteredIndexMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("FilteredColumnNotInKeyOfFilteredIndexMetrics");
        public string Edition { get; set; }
        public List<FilteredColumnNotInKeyOfFilteredIndexForDB> FilteredColumnNotInKeyOfFilteredIndexForDBs { get; set; }

        public FilteredColumnNotInKeyOfFilteredIndexMetrics()
        {
            FilteredColumnNotInKeyOfFilteredIndexForDBs = new List<FilteredColumnNotInKeyOfFilteredIndexForDB>();
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            if (snapshot.FilteredIndexSnapshotList == null || snapshot.FilteredIndexSnapshotList.Count == 0) { return; }
            foreach (FilteredColumnNotInKeyOfFilteredIndexSnapshot snap in snapshot.FilteredIndexSnapshotList)
            {
                //Check for error free snap shot
                if (snap != null && snap.Error == null)
                {
                    if (snap.FilteredColumnNotInKeyOfFilteredIndexInfo != null && snap.FilteredColumnNotInKeyOfFilteredIndexInfo.Rows.Count > 0)
                    {
                        FilteredColumnNotInKeyOfFilteredIndexForDB objDB = new FilteredColumnNotInKeyOfFilteredIndexForDB();
                        for (int index = 0; index < snap.FilteredColumnNotInKeyOfFilteredIndexInfo.Rows.Count; index++)
                        {
                            FilteredColumnNotInKeyOfFilteredIndex obj = new FilteredColumnNotInKeyOfFilteredIndex();
                            try
                            {
                                obj.Database = (string)snap.FilteredColumnNotInKeyOfFilteredIndexInfo.Rows[index]["dbname"];
                                obj.TableName = (string)snap.FilteredColumnNotInKeyOfFilteredIndexInfo.Rows[index]["TableName"];
                                obj.SchemaName = (string)snap.FilteredColumnNotInKeyOfFilteredIndexInfo.Rows[index]["SchemaName"];
                                obj.IndexName = (string)snap.FilteredColumnNotInKeyOfFilteredIndexInfo.Rows[index]["IndexName"];
                            }
                            catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                            objDB.FilteredColumnNotInKeyOfFilteredIndexList.Add(obj);
                        }
                        FilteredColumnNotInKeyOfFilteredIndexForDBs.Add(objDB);
                    }
                    else
                    {
                        _logX.Error("FilteredColumnNotInKeyOfFilteredIndexMetrics not added : " + snap.Error);
                        continue;
                    }
                }
            }
        }
    }

    public class FilteredColumnNotInKeyOfFilteredIndexForDB
    {
        public List<FilteredColumnNotInKeyOfFilteredIndex> FilteredColumnNotInKeyOfFilteredIndexList { get; set; }

        public FilteredColumnNotInKeyOfFilteredIndexForDB()
        {
            FilteredColumnNotInKeyOfFilteredIndexList = new List<FilteredColumnNotInKeyOfFilteredIndex>();
        }
    }

    public class FilteredColumnNotInKeyOfFilteredIndex
    {
        public string Database { get; set; }
        public string TableName { get; set; }
        public string SchemaName { get; set; }
        public string IndexName { get; set; }
    }
}
