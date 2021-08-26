//------------------------------------------------------------------------------
// <copyright file="QueryPlanEstRowsSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the QueryPlanEstRows information on a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class QueryPlanEstRowsSnapshot : Snapshot
    {
        #region fields

        private DataTable queryPlanEstRows = new DataTable("QueryPlanEstRows");

        #endregion

        #region constructors

        internal QueryPlanEstRowsSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            queryPlanEstRows.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public DataTable QueryPlanEstRows
        {
            get { return queryPlanEstRows; }
            internal set { queryPlanEstRows = value; }
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
