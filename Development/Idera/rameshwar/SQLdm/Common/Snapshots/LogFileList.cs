//------------------------------------------------------------------------------
// <copyright file="LogFileList.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common.Configuration;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Returns a list of all available logs
    /// </summary>
    [Serializable]
    public sealed class LogFileList : Snapshot
    {
        #region fields

        private List<LogFile> logList = new List<LogFile>();
        private int? maximumSqlLogs = null;
        private bool possibleToDelete = true;

        #endregion

        #region constructors


        internal LogFileList(SqlConnectionInfo info)
            : base(info.InstanceName)
        {

        }

        #endregion

        #region properties

        /// <summary>
        /// Combined list of all available logs
        /// </summary>
        public List<LogFile> LogList
        {
            get { return logList; }
            internal set { logList = value; }
        }

        /// <summary>
        /// Maximum number of SQL Server logs
        /// </summary>
        public int? MaximumSqlLogs
        {
            get { return maximumSqlLogs; }
            internal set { maximumSqlLogs = value; }
        }

        /// <summary>
        /// Boolean value returning whether the number of SQL Server logs is unlimited
        /// </summary>
        public bool UnlimitedSqlLogs
        {
            get { return maximumSqlLogs == 0; }
        }

        /// <summary>
        /// Boolean value returning whether it is possible to delete an error log
        /// Not currently used for anything
        /// </summary>
        internal bool PossibleToDelete
        {
            get { return possibleToDelete; }
            set { possibleToDelete = value; }
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
