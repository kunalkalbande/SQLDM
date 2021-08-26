//------------------------------------------------------------------------------
// <copyright file="QueryPlanSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the QueryPlan information on a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class QueryPlanSnapshot : Snapshot
    {
        #region fields

        private string queryPlan;
        private bool isActualPlan;      //SQLdm 10.4 (Nikhil Bansal) - Added for estimated query plan

        #endregion

        #region constructors

        internal QueryPlanSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            queryPlan = String.Empty;
            isActualPlan = true;
        }

        #endregion

        #region properties

        public string QueryPlan
        {
            get { return queryPlan; }
            internal set { queryPlan = value; }
        }

        public bool IsActualPlan
        {
            get { return isActualPlan; }
            set { isActualPlan = value; }
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
