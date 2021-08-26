//------------------------------------------------------------------------------
// <copyright file="HighCPUTimeProcedureSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-Q43 Adding new snapshot 
    /// </summary>
    [Serializable]
    public sealed class HighCPUTimeProcedureSnapshot : Snapshot
    {
        #region fields

        private string edition = string.Empty;

        private DataTable highCPUTimeProcedureInfo = new DataTable("HighCPUTimeProcedureInfo");

        #endregion

        #region constructors

        internal HighCPUTimeProcedureSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            highCPUTimeProcedureInfo.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public string Edition
        {
            get { return edition; }
            set { edition = value; }
        }

        public DataTable HighCPUTimeProcedureInfo
        {
            get { return highCPUTimeProcedureInfo; }
            internal set { highCPUTimeProcedureInfo = value; }
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
