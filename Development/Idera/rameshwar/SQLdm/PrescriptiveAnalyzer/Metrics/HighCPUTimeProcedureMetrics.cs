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
    public class HighCPUTimeProcedureMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("HighCPUTimeProcedureMetrics");
        public string Edition { get; set; }
        public List<HighCPUTimeProceduresForDB> HighCPUTimeProceduresForDBs { get; set; }

        public HighCPUTimeProcedureMetrics()
        {
            HighCPUTimeProceduresForDBs = new List<HighCPUTimeProceduresForDB>();
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            if (snapshot.HighCPUTimeProcedureSnapshotList == null || snapshot.HighCPUTimeProcedureSnapshotList.Count == 0) { return; }
            foreach (HighCPUTimeProcedureSnapshot snap in snapshot.HighCPUTimeProcedureSnapshotList)
            {
                //Check for error free snap shot
                if (snap != null && snap.Error == null)
                {
                    if (snap.HighCPUTimeProcedureInfo != null && snap.HighCPUTimeProcedureInfo.Rows.Count > 0)
                    {
                        HighCPUTimeProceduresForDB objDB = new HighCPUTimeProceduresForDB();
                        for (int index = 0; index < snap.HighCPUTimeProcedureInfo.Rows.Count; index++)
                        {
                            HighCPUTimeProcedures obj = new HighCPUTimeProcedures();
                            try
                            {
                                obj.Database = (string)snap.HighCPUTimeProcedureInfo.Rows[index]["DbName"];
                                obj.ProcedureName = (string)snap.HighCPUTimeProcedureInfo.Rows[index]["ProcedureName"];
                            }
                            catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                            objDB.HighCPUTimeProceduresList.Add(obj);
                        }
                        HighCPUTimeProceduresForDBs.Add(objDB);
                    }
                    else
                    {
                        _logX.Error("HighCPUTimeProceduresMetrics not added : " + snap.Error);
                        continue;
                    }
                }
            }
        }
    }

    public class HighCPUTimeProceduresForDB
    {
        public List<HighCPUTimeProcedures> HighCPUTimeProceduresList { get; set; }

        public HighCPUTimeProceduresForDB()
        {
            HighCPUTimeProceduresList = new List<HighCPUTimeProcedures>();
        }
    }

    public class HighCPUTimeProcedures
    {
        public string Database { get; set; }
        public string ProcedureName { get; set; }
    }
}
