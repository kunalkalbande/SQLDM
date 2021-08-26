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
    using System.Collections.Generic;

    /// <summary>
    /// //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-Q39,Q40,Q41, Q42 Adding new snapshot 
    /// </summary>
    [Serializable]
    public sealed class QueryStoreSnapshot : Snapshot
    {
        #region fields

        private string dbname = string.Empty;
        private List<string> planName = new List<string>();

        private DataTable queryStoreInfo = new DataTable("QueryStoreInfo");

        #endregion

        #region constructors

        internal QueryStoreSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            queryStoreInfo.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public string DbName
        {
            get { return dbname; }
            set { dbname = value; }
        }

        public DataTable QueryStoreInfo
        {
            get { return queryStoreInfo; }
            internal set { queryStoreInfo = value; }
        }
        public List<string> PlanName
        {
            get { return planName; }
            set { planName = value; }
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
