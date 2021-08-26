//------------------------------------------------------------------------------
// <copyright file="NUMANodeCountersSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the NUMANodeCounters information on a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class NUMANodeCountersSnapshot : Snapshot
    {
        #region fields

        private DataTable nUMANodeCounters = new DataTable("NUMANodeCounters");

        #endregion

        #region constructors

        internal NUMANodeCountersSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            nUMANodeCounters.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public DataTable NUMANodeCounters
        {
            get { return nUMANodeCounters; }
            internal set { nUMANodeCounters = value; }
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
