//------------------------------------------------------------------------------
// <copyright file="BackupAndRecoverySnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the BackupAndRecovery information of a monitored server
    /// </summary>
    [Serializable]
    public sealed class BackupAndRecoverySnapshot : Snapshot
    {
        #region fields

        private DataTable generalInfo = new DataTable("GeneralInfo");
        private DataTable backupfileInfo = new DataTable("BackupfileInfo");
        private DataTable dBFileInfo = new DataTable("DBFileInfo");
        private DataTable backupDateInfo = new DataTable("BackupDateInfo");
        private string dbName;

        #endregion

        #region constructors
        public BackupAndRecoverySnapshot()
        {
        }
        internal BackupAndRecoverySnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            generalInfo.RemotingFormat = SerializationFormat.Binary;
            backupfileInfo.RemotingFormat = SerializationFormat.Binary;
            dBFileInfo.RemotingFormat = SerializationFormat.Binary;
            backupDateInfo.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public string DBName
        {
            get { return dbName; }
            set { dbName = value; }
        }

        public DataTable GeneralInfo
        {
            get { return generalInfo; }
            set { generalInfo = value; }
        }

        public DataTable BackupfileInfo
        {
            get { return backupfileInfo; }
            set { backupfileInfo = value; }
        }

        public DataTable DBFileInfo
        {
            get { return dBFileInfo; }
            set { dBFileInfo = value; }
        }

        public DataTable BackupDateInfo
        {
            get { return backupDateInfo; }
            set { backupDateInfo = value; }
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
