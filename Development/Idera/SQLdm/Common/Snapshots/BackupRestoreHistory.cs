//------------------------------------------------------------------------------
// <copyright file="BackupRestoreHistory.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Configuration;
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents backup and restore history
    /// </summary>
   [Serializable]
    public sealed class BackupRestoreHistory: Snapshot
    {
        #region fields

        private DataTable backupHistory = new DataTable("BackupHistory");
        private DataTable restoreHistory = new DataTable("RestoreHistory");

        #endregion

        #region constructors



        internal BackupRestoreHistory(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            BackupHistory.RemotingFormat = SerializationFormat.Binary;
            BackupHistory.Columns.Add("DatabaseName", typeof (string));
            BackupHistory.Columns.Add("Type", typeof (BackupType));
            BackupHistory.Columns.Add("DateTime", typeof(DateTime));
            BackupHistory.Columns.Add("User", typeof (string));
            BackupHistory.Columns.Add("Size", typeof (FileSize));
            BackupHistory.Columns.Add("Path", typeof(string));
            BackupHistory.Columns.Add("Filename", typeof(string));
            //BackupHistory.Columns.Add("Point In Time", typeof(string));
            //BackupHistory.Columns.Add("Replace", typeof(bool));

            RestoreHistory.RemotingFormat = SerializationFormat.Binary;
            RestoreHistory.Columns.Add("DatabaseName", typeof(string));
            RestoreHistory.Columns.Add("Type", typeof(BackupType));
            RestoreHistory.Columns.Add("DateTime", typeof(DateTime));
            RestoreHistory.Columns.Add("User", typeof(string));
            RestoreHistory.Columns.Add("Path", typeof(string));
            RestoreHistory.Columns.Add("StopAtTime", typeof (DateTime));
            RestoreHistory.Columns.Add("StopAtMark", typeof (string));
            RestoreHistory.Columns.Add("Replace", typeof(bool));
            RestoreHistory.Columns.Add("Filename", typeof(string));


        }

        #endregion

        #region properties

        
        public DataTable BackupHistory
        {
            get { return backupHistory; }
            internal set { backupHistory = value; }
        }


        public DataTable RestoreHistory
        {
            get { return restoreHistory; }
            internal set { restoreHistory = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
