//------------------------------------------------------------------------------
// <copyright file="NonIncrementalColumnStatSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I23 Adding new snapshot 
    /// </summary>
    [Serializable]
    public sealed class NonIncrementalColumnStatSnapshot : Snapshot
    {
        #region fields

        private string edition = string.Empty;

        private DataTable nonIncrementalColumnStatInfo = new DataTable("NonIncrementalColumnStatInfo");

        #endregion

        #region constructors

        internal NonIncrementalColumnStatSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            nonIncrementalColumnStatInfo.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public string Edition
        {
            get { return edition; }
            set { edition = value; }
        }

        public DataTable NonIncrementalColumnStatInfo
        {
            get { return nonIncrementalColumnStatInfo; }
            internal set { nonIncrementalColumnStatInfo = value; }
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
