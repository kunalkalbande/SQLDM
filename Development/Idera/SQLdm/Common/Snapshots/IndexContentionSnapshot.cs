//------------------------------------------------------------------------------
// <copyright file="IndexContentionSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the IndexContention information on a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class IndexContentionSnapshot : Snapshot
    {
        #region fields

        private DataTable pageLatchIndexContention = new DataTable("PageLatchIndexContention");
        private DataTable pageLockIndexContention = new DataTable("PageLockIndexContention");
        private DataTable rowLockIndexContention = new DataTable("RowLockIndexContention");

        #endregion

        #region constructors

        internal IndexContentionSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            pageLatchIndexContention.RemotingFormat = SerializationFormat.Binary;
            pageLockIndexContention.RemotingFormat = SerializationFormat.Binary;
            rowLockIndexContention.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public DataTable PageLatchIndexContention
        {
            get { return pageLatchIndexContention; }
            internal set { pageLatchIndexContention = value; }
        }

        public DataTable PageLockIndexContention
        {
            get { return pageLockIndexContention; }
            internal set { pageLockIndexContention = value; }
        }

        public DataTable RowLockIndexContention
        {
            get { return rowLockIndexContention; }
            internal set { rowLockIndexContention = value; }
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
