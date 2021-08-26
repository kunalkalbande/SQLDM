//------------------------------------------------------------------------------
// <copyright file="LargeTableStatsSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the LargeTableStats info on a monitored server //SQLdm 10.0 (srishti Purohit) (New Recommendation) -- I30 -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class LargeTableStatsSnapshot : Snapshot
    {
        #region fields

        private DataTable largeTableStatsInfo = new DataTable("LargeTableStatsInfo");

        #endregion

        #region constructors

        internal LargeTableStatsSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            largeTableStatsInfo.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public DataTable LargeTableStatsInfo
        {
            get { return largeTableStatsInfo; }
            internal set { largeTableStatsInfo = value; }
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
