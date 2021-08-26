//------------------------------------------------------------------------------
// <copyright file="DatabaseRankingSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the DatabaseRanking info of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class DatabaseRankingSnapshot : Snapshot
    {
        #region fields

        private DataTable databaseRanks = new DataTable("DatabaseRanks");
        private string connectionString = string.Empty;

        #endregion

        #region constructors

        internal DatabaseRankingSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            databaseRanks.RemotingFormat = SerializationFormat.Binary;
            connectionString = info.ConnectionString;
        }

        #endregion

        #region properties

        public DataTable DatabaseRanks
        {
            get { return databaseRanks; }
            internal set { databaseRanks = value; }
        }

        public String ConnectionString
        {
            get { return connectionString; }
            internal set { connectionString = value; }
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
