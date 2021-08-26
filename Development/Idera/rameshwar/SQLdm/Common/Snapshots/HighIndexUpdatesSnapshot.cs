//------------------------------------------------------------------------------
// <copyright file="HighIndexUpdatesSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the HighIndexUpdates information on a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class HighIndexUpdatesSnapshot : Snapshot
    {
        #region fields

        private DataTable highIndexUpdates = new DataTable("HighIndexUpdates");

        #endregion

        #region constructors

        internal HighIndexUpdatesSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            highIndexUpdates.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public DataTable HighIndexUpdates
        {
            get { return highIndexUpdates; }
            internal set { highIndexUpdates = value; }
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
