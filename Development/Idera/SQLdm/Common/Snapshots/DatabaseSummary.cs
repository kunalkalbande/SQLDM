//------------------------------------------------------------------------------
// <copyright file="DatabaseSummary.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents snapshot information for the database summary view
    /// </summary>
    [Serializable]
    public sealed class DatabaseSummary : Snapshot
    {
        #region fields

        private ServerDatabaseSummary summaryData = new ServerDatabaseSummary();
        private Dictionary<string, DatabaseDetail> databases = new Dictionary<string, DatabaseDetail>();
        private TimeSpan? timeDelta = new TimeSpan(0);

        #endregion

        #region constructors

        public DatabaseSummary(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
        }

        #endregion

        #region properties

        public ServerDatabaseSummary SummaryData
        {
            get { return summaryData; }
            internal set { summaryData = value; }
        }

        public Dictionary<string, DatabaseDetail> Databases
        {
            get { return databases; }
            internal set { databases = value; }
        }

        public TimeSpan? TimeDelta
        {
            get { return timeDelta; }
            internal set { timeDelta = value; }
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
