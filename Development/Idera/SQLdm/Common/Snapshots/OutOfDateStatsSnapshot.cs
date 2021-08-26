//------------------------------------------------------------------------------
// <copyright file="OutOfDateStatsSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the OutOfDateStats info on a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class OutOfDateStatsSnapshot : Snapshot
    {
        #region fields

        private DataTable outOfDateStatsInfo = new DataTable("OutOfDateStatsInfo");

        #endregion

        #region constructors

        internal OutOfDateStatsSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            outOfDateStatsInfo.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public DataTable OutOfDateStatsInfo
        {
            get { return outOfDateStatsInfo; }
            internal set { outOfDateStatsInfo = value; }
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
