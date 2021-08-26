//------------------------------------------------------------------------------
// <copyright file="ColumnStoreIndexSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the ColumnStoreIndex info on a monitored server //SQLdm 10.0 (srishti Purohit) (New Recommendation) -- I30 -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class ColumnStoreIndexSnapshot : Snapshot
    {
        #region fields

        private string edition = string.Empty;
        private DataTable columnStoreIndexInfo = new DataTable("ColumnStoreIndexInfo");

        #endregion

        #region constructors

        internal ColumnStoreIndexSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            ColumnStoreIndexInfo.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public string Edition
        {
            get { return edition; }
            set { edition = value; }
        }
        public DataTable ColumnStoreIndexInfo
        {
            get { return columnStoreIndexInfo; }
            internal set { columnStoreIndexInfo = value; }
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
