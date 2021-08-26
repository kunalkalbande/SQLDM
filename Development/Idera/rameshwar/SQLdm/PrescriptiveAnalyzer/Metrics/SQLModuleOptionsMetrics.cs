using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using Idera.SQLdm.Common.Snapshots;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class SQLModuleOptionsMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("SQLModuleOptionsMetrics");
        public List<SQLModuleOptionsForDB> SQLModuleOptionsForDBs { get; set; }

        public SQLModuleOptionsMetrics()
        {
            SQLModuleOptionsForDBs = new List<SQLModuleOptionsForDB>();
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            if (snapshot.SQLModuleOptionsSnapshotList == null || snapshot.SQLModuleOptionsSnapshotList.Count == 0) { return; }
            foreach (SQLModuleOptionsSnapshot snap in snapshot.SQLModuleOptionsSnapshotList)
            {
                //Check for error free snap shot
                if (snap != null && snap.Error == null)
                {
                    if (snap.SQLModuleOptions != null && snap.SQLModuleOptions.Rows.Count > 0)
                    {
                        SQLModuleOptionsForDB objDB = new SQLModuleOptionsForDB();
                        for (int index = 0; index < snap.SQLModuleOptions.Rows.Count; index++)
                        {
                            SQLModuleOptions obj = new SQLModuleOptions();
                            try
                            {
                                obj.DatabaseName = (string)snap.SQLModuleOptions.Rows[index]["DatabaseName"];
                                obj.ObjectName = (string)snap.SQLModuleOptions.Rows[index]["ObjectName"];
                                obj.ObjectType = (string)snap.SQLModuleOptions.Rows[index]["ObjectType"];
                                obj.ObjectDefinition = (string)snap.SQLModuleOptions.Rows[index]["SQL"];
                                obj.UsesAnsiNulls = (bool)snap.SQLModuleOptions.Rows[index]["uses_ansi_nulls"];
                                obj.UsesQuotedIdentifier = (bool)snap.SQLModuleOptions.Rows[index]["uses_quoted_identifier"];
                                obj.SchemaName = (string)snap.SQLModuleOptions.Rows[index]["Schema"];
                            }
                            catch (Exception e)
                            {
                                _logX.Error(e);
                                IsDataValid = false;
                                return;
                            }
                            objDB.SQLModuleOptionsList.Add(obj);
                        }
                        SQLModuleOptionsForDBs.Add(objDB);
                    }
                }
                else
                {
                    _logX.Error("SQLModuleOptionsMetrics not added : " + snap.Error);
                    continue;
                }
            }
        }
    }

    public class SQLModuleOptionsForDB
    {
        public List<SQLModuleOptions> SQLModuleOptionsList { get; set; }

        public SQLModuleOptionsForDB()
        {
            SQLModuleOptionsList = new List<SQLModuleOptions>();
        }
    }

    public class SQLModuleOptions
    {
        public bool UsesQuotedIdentifier { get; set; }
        public bool UsesAnsiNulls { get; set; }
        public string DatabaseName { get; set; }
        public string SchemaName { get; set; }
        public string ObjectName { get; set; }
        public string ObjectType { get; set; }
        public string ObjectDefinition { get; set; }
    }
}
