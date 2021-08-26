//------------------------------------------------------------------------------
// <copyright file="TableSummaryConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration object for Table Summary on-demand probe
    /// </summary>
    [Serializable]
    public class TableSummaryConfiguration : OnDemandConfiguration
    {
        #region fields

        private bool showUserTables = true;
        private bool showSystemTables = true;
        private string databaseName = null;

        #endregion

        #region constructors

        public TableSummaryConfiguration(int monitoredServerId, bool showUserTables, bool showSystemTables, string databaseName) : base(monitoredServerId)
        {
            this.showUserTables = showUserTables;
            this.showSystemTables = showSystemTables;
            this.databaseName = databaseName;
        }

        #endregion

        #region properties

        new public bool ReadyForCollection
        {
            get { return (DatabaseName != null && (ShowSystemTables || ShowUserTables)); }
        }

        public bool ShowUserTables
        {
            get { return showUserTables; }
            set { showUserTables = value; }
        }

        public bool ShowSystemTables
        {
            get { return showSystemTables; }
            set { showSystemTables = value; }
        }

        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
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
