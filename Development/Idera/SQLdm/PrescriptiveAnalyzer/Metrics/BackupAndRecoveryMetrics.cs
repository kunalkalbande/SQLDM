using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Values;
using Idera.SQLdm.Common.Snapshots;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class BackupAndRecoveryMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("BackupAndRecoveryMetrics");
        public List<BackupAndRecoveryForDB> BackupAndRecoveryForDBs { get; set; }

        public BackupAndRecoveryMetrics()
        {
            BackupAndRecoveryForDBs = new List<BackupAndRecoveryForDB>();
        }


        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            if (snapshot.BackupAndRecoverySnapshotList == null || snapshot.BackupAndRecoverySnapshotList.Count == 0) { return; }
            foreach (BackupAndRecoverySnapshot snap in snapshot.BackupAndRecoverySnapshotList)
            {
                //Check for error free snap shot
                if (snap != null && snap.Error == null)
                {
                    BackupAndRecoveryForDB obj = new BackupAndRecoveryForDB();

                    obj.dbname = snap.DBName;
                    if (snap.GeneralInfo != null && snap.GeneralInfo.Rows.Count > 0)
                    {
                        for (int index = 0; index < snap.GeneralInfo.Rows.Count; index++)
                        {
                            try
                            {
                                string prop = snap.GeneralInfo.Rows[index]["Name"].ToString();
                                switch (prop)
                                {
                                    case "dbi_dbccLastKnownGood":
                                        {
                                            obj.dbi_dbccLastKnownGood = snap.GeneralInfo.Rows[index]["Value"].ToString();
                                            break;
                                        }
                                    case "dbi_crdate":
                                        {
                                            obj.dbi_crdate = snap.GeneralInfo.Rows[index]["Value"].ToString();
                                            break;
                                        }
                                    case "dbid_suspectpages":
                                        {
                                            obj.dbid_suspectpages = snap.GeneralInfo.Rows[index]["Value"].ToString();
                                            break;
                                        }
                                    case "is_read_only":
                                        {
                                            obj.is_read_only = snap.GeneralInfo.Rows[index]["Value"].ToString();
                                            break;
                                        }
                                    case "Recovery_Mode":
                                        {
                                            obj.Recovery_Mode = snap.GeneralInfo.Rows[index]["Value"].ToString();
                                            break;
                                        }
                                }
                            }
                            catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                        }
                    }


                    if (snap.BackupfileInfo != null && snap.BackupfileInfo.Rows.Count > 0)
                    {
                        try
                        {
                            obj.backupFile = snap.BackupfileInfo.Rows[0]["FileName"].ToString();
                            obj.lastBackup = (DateTime)snap.BackupfileInfo.Rows[0]["StartDateTime"];
                            obj.backupFileExistsOnDisk = snap.BackupfileInfo.Rows[0]["FileExists"].ToString() == "1" ? true : false;
                        }
                        catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                    }

                    if (snap.DBFileInfo != null && snap.DBFileInfo.Rows.Count > 0)
                    {

                        for (int index = 0; index < snap.DBFileInfo.Rows.Count; index++)
                        {
                            try
                            {
                                obj.files.Add(snap.DBFileInfo.Rows[index]["PhysicalName"].ToString());
                            }
                            catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                        }
                    }

                    if (snap.BackupDateInfo != null && snap.BackupDateInfo.Rows.Count > 0)
                    {
                        try
                        {
                            obj.lastLogBackup = (DateTime)snap.BackupDateInfo.Rows[0]["BackupStartDate"];
                            obj.logBackupAge = (Int32)snap.BackupDateInfo.Rows[0]["DaysOld"];
                        }
                        catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                    }

                    BackupAndRecoveryForDBs.Add(obj);
                }
                else
                {
                    _logX.Error("BackupAndRecoverySnapshot not added : " + snap.Error);
                    continue;
                }
            }
        }
    }

    public class BackupAndRecoveryForDB
    {
        public string dbi_dbccLastKnownGood { get; set; }
        public string dbi_crdate { get; set; }
        public string dbid_suspectpages { get; set; }
        public string is_read_only { get; set; }
        public string Recovery_Mode { get; set; }

        public string dbname { get; set; }

        public string backupFile { get; set; }
        public DateTime lastBackup { get; set; }
        public Boolean backupFileExistsOnDisk { get; set; }

        public List<string> files { get; set; }

        public long logBackupAge { get; set; }
        public DateTime? lastLogBackup { get; set; }

        public BackupAndRecoveryForDB()
        {
            files = new List<string>();
            dbi_dbccLastKnownGood = string.Empty;
            dbi_crdate = string.Empty;
            is_read_only = string.Empty;
            Recovery_Mode = string.Empty;
            dbname = string.Empty;
            backupFile = string.Empty;
        }
    }
}
