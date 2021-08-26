//------------------------------------------------------------------------------
// <copyright file="DatabaseDetail.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a database on the target SQL Server
    /// </summary>
    [Serializable]
    public sealed class DatabaseDetail : DatabaseStatistics
    {
        #region fields  

        private float? compatibilityLevel = null;
        private DateTime? lastBackup = null;
        private int? mode = null;
        private DatabaseOptions options = null;
        private long? processCount = null;
        //private List<RecoveryOperation> recoveryOperations = null;
        private DatabaseReplicationCategories replicationCategories = new DatabaseReplicationCategories(0);
        //private bool? inaccessible = null;
        private int? systemTables = null;
        private int? userTables = null;
        private int? oldestOpenTransactionSpid = null;
        private DateTime? oldestOpenTransactionStartTime = null;
        private DateTime? lastCheckpoint = null;
        private RecoveryModel recoveryType = RecoveryModel.Simple;
        private decimal? inMemoryStorageUsage = null;

        #endregion

        #region constructors

        public DatabaseDetail(string serverName, string dbName) : base(serverName, dbName)
        {
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets whether certain database behaviors are internal set to be compatible with the 
        /// specified earlier version of SQL Server.
        /// </summary>
        public float? CompatibilityLevel
        {
            get { return compatibilityLevel; }
            internal set { compatibilityLevel = value; }
        }


        /// <summary>
        /// Gets the last backup date, which is the last time the log would have been dumped.
        /// </summary>
        public DateTime? LastBackup
        {
            get { return lastBackup; }
            internal set { lastBackup = value; }
        }

        /// <summary>
        /// Gets a value used internally for locking a database while it is being created.
        /// </summary>
        public int? Mode
        {
            get { return mode; }
            internal set { mode = value; }
        }

        /// <summary>
        /// Gets the current database options.
        /// </summary>
        public DatabaseOptions Options
        {
            get { return options; }
            internal set { options = value; }
        }

        /// <summary>
        /// Gets the number of processes using the database.
        /// </summary>
        public long? ProcessCount
        {
            get { return processCount; }
            internal set { processCount = value; }
        }

       /// <summary>
        /// Gets the various replication methods being used by this database.
        /// </summary>
        public DatabaseReplicationCategories ReplicationCategory
        {
            get { return replicationCategories; }
            internal set { replicationCategories = value; }
        }

        /// <summary>
        /// Count of all tables
        /// </summary>
        public new int? TableCount
        {
            get { return SystemTables + UserTables; }  
        }

        /// <summary>
        /// Count of system tables
        /// </summary>
        public int? SystemTables
        {
            get { return systemTables; }
            internal set { systemTables = value; }
        }

        /// <summary>
        /// Count of user tables
        /// </summary>
        public int? UserTables
        {
            get { return userTables; }
            internal set { userTables = value; }
        }

        /// <summary>
        /// Session ID of oldest open transaction
        /// </summary>
        public int? OldestOpenTransactionSpid
        {
            get { return oldestOpenTransactionSpid; }
            internal set { oldestOpenTransactionSpid = value; }
        }

        /// <summary>
        /// Start time (UTC) of oldest open transaction
        /// </summary>
        public DateTime? OldestOpenTransactionStartTime
        {
            get { return oldestOpenTransactionStartTime; }
            internal set { oldestOpenTransactionStartTime = value; }
        }

        /// <summary>
        /// Last log checkpoint (UTC) - No longer populated
        /// </summary>
        public DateTime? LastCheckpoint
        {
            get { return lastCheckpoint; }
            internal set { lastCheckpoint = value; }
        }

        /// <summary>
        /// Returns bool for whether the database is a system database
        /// </summary>
        new public bool IsSystemDatabase
        {
            get
            {
                switch (Name)
                {
                    case "master":
                    case "tempdb":
                    case "model":
                    case "msdb":
                        return true;
                    default:
                        if (ReplicationCategory != null && ReplicationCategory.DistributionDatabase)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                }
            }
        }


        public RecoveryModel RecoveryType
        {
            get { return recoveryType; }
            internal set { recoveryType = value; }
        }

        public decimal? InMemoryStorageUsage
        {
            get { return inMemoryStorageUsage; }
            internal set { inMemoryStorageUsage = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region private int?erface implementations

        #endregion

        #region nested types

        #endregion

    }
}
