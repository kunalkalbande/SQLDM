//------------------------------------------------------------------------------
// <copyright file="DatabaseFileSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Data;
using System.IO;
using Idera.SQLdm.Common.Configuration;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents physical files for a database
    /// </summary>
    [Serializable]
    public sealed class DatabaseFilesSnapshot : Snapshot
    {
        #region fields

        private Dictionary<string, DatabaseFile> databaseFiles = new Dictionary<string, DatabaseFile>();
        private Dictionary<string, DatabaseFileGroup> databaseFileGroups = new Dictionary<string, DatabaseFileGroup>();
        private Dictionary<string, DiskDrive> diskDrives = new Dictionary<string, DiskDrive>();
        private Dictionary<string, DatabaseFileGroup> databaseLogs = null;
        private Dictionary<string, DiskDrive> sqlDiskDrives = null;
        private bool isFileSystemObjectDataAvailable = true;


        #endregion

        #region constructors

        public DatabaseFilesSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {

        }

        #endregion

        #region properties

        public Dictionary<string, DatabaseFile> DatabaseFiles
        {
            get { return databaseFiles; }
            internal set { databaseFiles = value; }
        }


        public Dictionary<string, DatabaseFileGroup> DatabaseFileGroups
        {
            get { return databaseFileGroups; }
            internal set { databaseFileGroups = value; }
        }

        /// <summary>
        /// Returns a list of databases with a summary of the log files for the database
        /// </summary>
        public Dictionary<string, DatabaseFileGroup> DatabaseLogs
        {
            get
            {
                if (databaseLogs == null)
                    databaseLogs = GetDatabaseLogs(DatabaseFiles, DiskDrives);
                return databaseLogs;
            }
            internal set { databaseLogs = value; }
        }

        public Dictionary<string, DiskDrive> DiskDrives
        {
            get { return diskDrives; }
            set { diskDrives = value; }
        }

        public Dictionary<string, DiskDrive> SqlDiskDrives
        {
            get
            {
                if (sqlDiskDrives == null)
                    sqlDiskDrives = GetSqlDiskDrives(DatabaseFiles, DiskDrives);
                return sqlDiskDrives;
            }
            set { sqlDiskDrives = value; }
        }

        public bool IsFileSystemObjectDataAvailable
        {
            get { return isFileSystemObjectDataAvailable; }
            set { isFileSystemObjectDataAvailable = value; }
        }

        public TempdbStatistics TempdbStatistics { get; internal set; }
        
        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Summarize the logs from the provided list of files
        /// </summary>
        /// <param name="InputFiles">The list of database files to summarize</param>
        /// <param name="InputDisks">The list of disks where the Sql files are</param>
        /// <returns></returns>
        public static Dictionary<string, DatabaseFileGroup> GetDatabaseLogs(Dictionary<string, DatabaseFile> InputFiles, Dictionary<string, DiskDrive> InputDrives)
        {
            Dictionary<string, DatabaseFileGroup> databaseLogs = new Dictionary<string, DatabaseFileGroup>();
            // this list consists of Key = Pair<database, logicalfile>, Value = Pair<drive, logfile>
            Dictionary<Pair<string, string>, Pair<string, DatabaseFile>> logFiles = new Dictionary<Pair<string, string>, Pair<string, DatabaseFile>>();

            // process all the files and create a list of the log files with the drive it is on
            //  and create a log FileGroup for each database
            foreach (DatabaseFile file in InputFiles.Values)
            {
                if (!file.IsDataFile)
                {
                    string drive = string.Empty;
                    drive = Path.GetPathRoot(file.FilePath);
                    if (drive.IndexOf(Path.VolumeSeparatorChar) > 0)
                    {
                        drive = drive.Substring(0, drive.IndexOf(Path.VolumeSeparatorChar)).ToUpper();
                    }
                    else
                    {
                        drive = string.Empty;
                    }
                    logFiles.Add(new Pair<string, string>(file.DatabaseName, file.LogicalFilename), new Pair<string, DatabaseFile>(drive, file));

                    DatabaseFileGroup log;
                    if (databaseLogs.ContainsKey(file.DatabaseName))
                    {
                        log = databaseLogs[file.DatabaseName];

                        AutogrowStatus autogrow = log.Autogrow;
                        log.Autogrow = (file.ConfiguredGrowth == 0 ? AutogrowStatus.AutogrowOff : AutogrowStatus.AutogrowOn);
                        if (autogrow != log.Autogrow)
                        {
                            log.Autogrow = AutogrowStatus.Mixed;
                        }
                    }
                    else
                    {
                        log = new DatabaseFileGroup();
                        log.DatabaseName = file.DatabaseName;

                        log.Autogrow = (file.ConfiguredGrowth == 0 ? AutogrowStatus.AutogrowOff : AutogrowStatus.AutogrowOn);
                        log.ExpansionSpace.Kilobytes = 0;
                        databaseLogs.Add(file.DatabaseName, log);
                    }
                    log.FileCount += 1;
                    log.CurrentSize.Kilobytes = log.CurrentSize.Kilobytes == null ? file.CurrentSize.Kilobytes : log.CurrentSize.Kilobytes + file.CurrentSize.Kilobytes;
                    log.CurrentUsedSize.Kilobytes = log.CurrentUsedSize.Kilobytes == null ? file.CurrentUsedSize.Kilobytes : log.CurrentUsedSize.Kilobytes + file.CurrentUsedSize.Kilobytes;
                    log.CurrentFreeSize.Kilobytes = log.CurrentFreeSize.Kilobytes == null ? file.CurrentFreeSize.Kilobytes : log.CurrentFreeSize.Kilobytes + file.CurrentFreeSize.Kilobytes;
                }
            }

            // now calculate the max available for each database
            foreach (KeyValuePair<string, DatabaseFileGroup> databaseLog in databaseLogs)
            {
                string database = databaseLog.Key;
                // this list consists of Key = drive, Value = Pair<currentSize, expansionSpace>
                Dictionary<string, Pair<decimal, decimal>> logDriveSums = new Dictionary<string, Pair<decimal, decimal>>();
                foreach (KeyValuePair<Pair<string, string>, Pair<string, DatabaseFile>> logFile in logFiles)
                {
                    if (logFile.Key.First == database)
                    {
                        Pair<decimal, decimal> driveSums;
                        if (logDriveSums.TryGetValue(logFile.Value.First, out driveSums))
                        {
                            driveSums.First += logFile.Value.Second.CurrentSize.Kilobytes.HasValue ? logFile.Value.Second.CurrentSize.Kilobytes.Value : 0;
                            driveSums.Second += logFile.Value.Second.ExpansionSpace.Kilobytes.HasValue ? logFile.Value.Second.ExpansionSpace.Kilobytes.Value : 0;
                        }
                        else
                        {
                            driveSums = new Pair<decimal, decimal>(
                                    logFile.Value.Second.CurrentSize.Kilobytes.HasValue ? logFile.Value.Second.CurrentSize.Kilobytes.Value : 0,
                                    logFile.Value.Second.ExpansionSpace.Kilobytes.HasValue ? logFile.Value.Second.ExpansionSpace.Kilobytes.Value : 0);
                            logDriveSums.Add(logFile.Value.First, driveSums);
                        }
                    }
                }

                foreach (KeyValuePair<string, Pair<decimal, decimal>> driveSums in logDriveSums)
                {
                    if (InputDrives.ContainsKey(driveSums.Key))
                    {
                        decimal driveFreeSpace = InputDrives[driveSums.Key].UnusedSize.Kilobytes.HasValue ? InputDrives[driveSums.Key].UnusedSize.Kilobytes.Value : 0;
                        if (driveSums.Value.Second > driveFreeSpace)
                        {
                            databaseLog.Value.ExpansionSpace.Kilobytes += driveFreeSpace;
                            //databaseLog.Value.MaximumPotentialFileSize.Kilobytes += (driveSums.Value.First + driveFreeSpace);
                        }
                        else
                        {
                            databaseLog.Value.ExpansionSpace.Kilobytes += (driveSums.Value.Second);
                            //databaseLog.Value.MaximumPotentialFileSize.Kilobytes += driveSums.Value.Second;
                        }
                    }
                    else
                    {
                        databaseLog.Value.ExpansionSpace.Kilobytes += (driveSums.Value.Second - driveSums.Value.First);
                        //databaseLog.Value.MaximumPotentialFileSize.Kilobytes += driveSums.Value.Second;
                    }
                }
            }

            return databaseLogs;
        }

        /// <summary>
        /// Summarize the logs from the provided list of files
        /// </summary>
        /// <param name="InputFiles">The list of database files to summarize</param>
        /// <param name="InputDrives">The list of disks where the Sql files are</param>
        /// <returns></returns>
        public static Dictionary<string, DiskDrive> GetSqlDiskDrives(Dictionary<string, DatabaseFile> InputFiles, Dictionary<string, DiskDrive> InputDrives)
        {
            Dictionary<string, DiskDrive> drives = new Dictionary<string, DiskDrive>();
            foreach (DiskDrive diskDrive in InputDrives.Values)
            {
                DiskDrive sqlDrive = new DiskDrive();
                sqlDrive.DriveLetter = diskDrive.DriveLetter;
                sqlDrive.UnusedSize = diskDrive.UnusedSize;
                sqlDrive.TotalSize = diskDrive.TotalSize;
                sqlDrive.SqlUsedSize.Kilobytes = 0;
                sqlDrive.SqlUnusedSize.Kilobytes = 0;
                sqlDrive.SqlLogSize.Kilobytes = 0;
                foreach (DatabaseFile file in InputFiles.Values)
                {
                    if (file.DriveName != null && file.DriveName.Length > 0 && file.DriveName.ToUpper() == sqlDrive.DriveLetter.ToUpper())
                    {
                        if (file.IsDataFile)
                        {
                            sqlDrive.SqlUsedSize.Kilobytes += file.CurrentUsedSize.Kilobytes.HasValue ? file.CurrentUsedSize.Kilobytes.Value : 0;
                            sqlDrive.SqlUnusedSize.Kilobytes += file.CurrentFreeSize.Kilobytes.HasValue ? file.CurrentFreeSize.Kilobytes.Value : 0;
                        }
                        else
                        {
                            sqlDrive.SqlLogSize.Kilobytes += file.CurrentSize.Kilobytes.HasValue ? file.CurrentSize.Kilobytes.Value : 0;
                        }
                    }
                }
                drives.Add(diskDrive.DriveLetter, sqlDrive);
            }

            return drives;
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
