using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Data.SqlClient;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    public class  BackupAndRecoveryAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 1;
        private static Logger _logX = Logger.GetLogger("BackupAndRecoveryAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public override string GetDescription() { return ("BackupAndRecovery analysis"); }

        public BackupAndRecoveryAnalyzer()
        {
            _id = id;
        }

        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            if (sm == null) return;
            if (sm.BackupAndRecoveryMetrics == null) return;
            foreach (BackupAndRecoveryForDB metrics in sm.BackupAndRecoveryMetrics.BackupAndRecoveryForDBs)
            {
                bool tempdb = (0 == string.Compare(metrics.dbname, "tempdb", true));
                if (metrics.dbi_dbccLastKnownGood == null)
                    _logX.WarnFormat("metrics.dbi_dbccLastKnownGood is null for database {0}. Cannot generate rcommendation for this record as data is insufficient", metrics.dbname);
                else if (metrics.dbi_crdate == null)
                    _logX.WarnFormat("metrics.dbi_crdate is null for database {0}. Cannot generate rcommendation for this record as data is insufficient", metrics.dbname);
                else
                {
//to handle case if metrics datetime string is ""
                    //DateTime dbccLastKnownGood = Convert.ToDateTime(metrics.dbi_dbccLastKnownGood, CultureInfo.CreateSpecificCulture("en-US"));
                    DateTime dbccLastKnownGood;
                    if (!DateTime.TryParse(metrics.dbi_dbccLastKnownGood, out dbccLastKnownGood))
                        dbccLastKnownGood = DateTime.MinValue;
                    DateTime crdate;
                    if (!DateTime.TryParse(metrics.dbi_crdate, out crdate))
                        crdate = DateTime.MinValue;
                    int dbid_suspectpages = Convert.ToInt32(metrics.dbid_suspectpages);
                    bool isReadOnly = metrics.is_read_only == "0" ? false : true;
                    string recoveryMode = metrics.Recovery_Mode;

                    if (0 != dbid_suspectpages)
                    {
                        _logX.Debug(string.Format("Database [{0}] has suspect pages (dbid:{1})", metrics.dbname, dbid_suspectpages));
                        AddRecommendation(new DatabaseSuspectPagesRecommendation(metrics.dbname));
                    }
                    if (crdate > dbccLastKnownGood)
                    {
                        _logX.Debug(string.Format("Default dbccLastKnownGood to create date for database [{0}] (crdate:{1} dbccLastKnownGood:{2})", metrics.dbname, crdate, dbccLastKnownGood));
                        dbccLastKnownGood = crdate;
                    }
                    if (!tempdb && !isReadOnly && (dbccLastKnownGood > DateTime.MinValue))
                    {
                        TimeSpan ts = DateTime.Now - dbccLastKnownGood;
                        if (ts.TotalDays > Settings.Default.DaysSinceLastCheckDB)
                        {
                            _logX.Info(string.Format("Database: [{0}] dbi_dbccLastKnownGood: {1}  TotalDays: {2}", metrics.dbname, dbccLastKnownGood, ts.TotalDays));
                            AddRecommendation(new DatabaseCheckIntegrityRecommendation(metrics.dbname, ts.TotalDays));
                        }
                    }



                    string backupFile = metrics.backupFile;
                    DateTime lastBackup = metrics.lastBackup;
                    bool backupFileExistsOnDisk = metrics.backupFileExistsOnDisk;
                    if (backupFile == null && lastBackup == null)
                    {
                        _logX.Debug(string.Format("Failed to read the next result that should have backup information for database {0}", metrics.dbname)); return;
                    }

                    List<string> files = new List<string>();

                    if (crdate > lastBackup)
                    {
                        _logX.Debug(string.Format("Default lastBackup to create date for database [{0}] (crdate:{1} lastBackup:{2})", metrics.dbname, crdate, lastBackup));
                        lastBackup = crdate;
                    }

                    //--------------------------------------------------------------------------------------------------
                    //  Backups out of date (SQL Server 2008 R2 BPA Information)
                    //
                    //  If you run the BPA tool and encounter an Error with the title of 
                    //  Database Engine - backups outdated for databases, then you need to verify the 
                    //  schedule of the backups for your databases and ensure the backups are happening at 
                    //  the schedule you intended it to happen. You will encounter this error if you have 
                    //  not performed a backup of your database in the last one day.
                    //  
                    //  For more infromation about the system tables used by the BPA tool to find the 
                    //  schedule of the backups, refer to the Books Online topic: Viewing Information 
                    //  About Backups (http://msdn.microsoft.com/en-us/library/ms188653.aspx) 
                    //
                    if (!isReadOnly && !tempdb)
                    {
                        TimeSpan ts = DateTime.Now - lastBackup;
                        if (Math.Round(ts.TotalDays, 0) > Settings.Default.DaysSinceLastBackup)
                        {
                            _logX.Info(string.Format("Database: [{0}] lastBackup: {1}  TotalDays: {2}", metrics.dbname, lastBackup, ts.TotalDays));
                            AddRecommendation(new DatabaseOutdatedBackupsRecommendation(metrics.dbname, ts.TotalDays));
                        }
                    }


                    files = metrics.files;
                    if (files == null) { _logX.Debug(string.Format("Failed to read the list of files for database {0}", metrics.dbname)); return; }
                    //--------------------------------------------------------------------------------------------------
                    //  Backups and file on same volume (SQL Server 2008 R2 BPA Information)
                    //  
                    //  The SQL Server 2008 R2 Best Practice Analyzer (SQL Server 2008 R2 BPA) provides rules 
                    //  to detect situations where some of these backup recommendations are not followed. 
                    //  The SQL Server 2008 R2 BPA supports both SQL Server 2008 and SQL Server 2008 R2. 
                    //  
                    //  If you run the BPA tool and encounter an Error with the title of DatabaseEngine - dDatabase 
                    //  files and backups exist on the same volume, then you need to verify the location where you 
                    //  store the backups for your databases. You will encounter this error if the backups are 
                    //  stored in the same location as the database files. If you get this error, there are a 
                    //  couple of important points to consider:
                    //    1. This rule checks only logical volumes of the location for the backup 
                    //       file and the database file. You need to manually ensure that these 
                    //       logical volumes are actually on seperate physical disks or drives.
                    //    2. You could encounter this error from the BPA tool when there are old 
                    //       entries present in the backup history tables that shows backup was taken 
                    //       to the same volume as the database files. If you are aware of such backups, 
                    //       you could either ignore the error or clean up the old information from the 
                    //       msdb backup history tables using the stored procedure sp_delete_database_backuphistory.
                    //    3. You could encounter this error if you have your database files and backup 
                    //       files located on network locations or shares. Currently the BPA tool evaluates 
                    //       the first 3 characters of the physical path to find out the drive name. 
                    //       
                    //  For more infromation about the system tables used by the BPA tool to find the location of the backups, 
                    //  refer to the Books Online topic: 
                    //    Viewing Information About Backups (http://msdn.microsoft.com/en-us/library/ms188653.aspx) 
                    //
                    if (!string.IsNullOrEmpty(backupFile) && backupFile.Length > 2)
                    {
                        int slash = backupFile.Substring(2).IndexOfAny(new char[] { '\\', '/' }) + 2;
                        if (slash < 2) slash = 2;
                        string prefix = backupFile.Substring(0, slash).ToUpper();
                        foreach (string file in files)
                        {
                            if (file.ToUpper().StartsWith(prefix))
                            {
                                _logX.Info(string.Format("Database: [{0}] backup ({1}) on same volume ({2})", metrics.dbname, backupFile, file));
                                AddRecommendation(new DatabaseBackupsOnSameVolumeRecommendation(metrics.dbname, backupFile, file));
                                break;
                            }
                        }
                    }

                    if (!backupFileExistsOnDisk)
                    {
                        _logX.Info(string.Format("Database: [{0}] backup ({1}) no longer exists on disk", metrics.dbname, backupFile));
                        AddRecommendation(new DatabaseBackupsDeletedRecommendation(metrics.dbname, backupFile));
                    }

                    if ("SIMPLE" != recoveryMode.ToUpper())
                    {

                        DateTime? lastLogBackup = metrics.lastLogBackup;
                        Int64 logBackupAge = metrics.logBackupAge;
                        //Changes done to support SDR-R7
                        if (lastLogBackup == null && logBackupAge <= 1)
                        {
                            logBackupAge = (Int64)(DateTime.Now - crdate).TotalDays;
                            lastLogBackup = null;
                            _logX.Debug(string.Format("Failed to read recovery model & last log backup for database {0}", metrics.dbname));
                            return;
                        }
                        if (logBackupAge > 1)
                        {
                            AddRecommendation(new DatabaseNoRecentLogBackupRecommendation(metrics.dbname, logBackupAge, recoveryMode));
                            if (null == lastLogBackup)
                                _logX.Info(string.Format("Database: [{0}] LastLogBackup: never  TotalDays: {1}", metrics.dbname, logBackupAge));
                            else
                                _logX.Info(string.Format("Database: [{0}] LastLogBackup: {1}  TotalDays: {2}", metrics.dbname, lastLogBackup, logBackupAge));
                        }
                    }
                    else
                        _logX.Debug(string.Format("Skip reading log backup history for database [{0}] because recovery mode is simple", metrics.dbname));
                }
            }
        }
    }
}
