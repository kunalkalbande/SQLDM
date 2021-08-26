using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using Idera.SQLdm.Common.Snapshots;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class DBSecurityMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("DBSecurityMetrics");
        public List<DBSecurityForDB> DBSecurityForDBs { get; set; }

        public DBSecurityMetrics()
        {
            DBSecurityForDBs = new List<DBSecurityForDB>();
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("DBSecurityMetrics not added : " + snapshot.Error); return; }
            if (snapshot.DBSecuritySnapshotList == null || snapshot.DBSecuritySnapshotList.Count == 0) { return; }
            foreach (DBSecuritySnapshot snap in snapshot.DBSecuritySnapshotList)
            {
                //Check for error free snap shot
                if (snap != null && snap.Error == null)
                {
                    DBSecurityForDB obj = new DBSecurityForDB(snap.DbName);
                    if (snap.DBSecurity != null && snap.DBSecurity.Rows.Count > 0)
                    {
                        for (int index = 0; index < snap.DBSecurity.Rows.Count; index++)
                        {
                            try
                            {
                                string prop = snap.DBSecurity.Rows[index][0].ToString();
                                switch (prop)
                                {
                                    case "GuestHasDatabaseAccess":
                                        {
                                            obj.GuestHasDatabaseAccess = (bool)snap.DBSecurity.Rows[index]["value"];
                                            break;
                                        }
                                    case "IsTrustworthyVulnerable":
                                        {
                                            obj.IsTrustworthyVulnerable = (bool)snap.DBSecurity.Rows[index]["value"];
                                            break;
                                        }
                                    case "NonSystemDatabaseWeakKey":
                                        {
                                            obj.NonSystemDatabaseWeakKey = (bool)snap.DBSecurity.Rows[index]["value"];
                                            break;
                                        }
                                    case "SystemDatabaseSymmetricKey":
                                        {
                                            obj.SystemDatabaseSymmetricKey = (bool)snap.DBSecurity.Rows[index]["value"];
                                            break;
                                        }
                                }
                            }
                            catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                        }
                    }
                    DBSecurityForDBs.Add(obj);

                }
                else
                {
                    _logX.Error("DBSecurityMetrics not added : " + snapshot.Error);
                }
            }
        }
    }

    public class DBSecurityForDB
    {
        public bool GuestHasDatabaseAccess { get; set; }
        public bool IsTrustworthyVulnerable { get; set; }
        public bool SystemDatabaseSymmetricKey { get; set; }
        public bool NonSystemDatabaseWeakKey { get; set; }
        public string dbname { get; set; }
        //to set DB property
        //To fix recomm generation problem with SDR-S7
        public DBSecurityForDB(string dbName)
        {
            this.dbname = dbName;
        }
    }
}
