//------------------------------------------------------------------------------
// <copyright file="FullTextColumnsConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration object for full text columns on-demand probe
    /// </summary>
    [Serializable]
    public class FullTextColumnsConfiguration: OnDemandConfiguration
    {
        #region fields

        string databaseName = null;
        private long? tableId = null;

        #endregion

        #region constructors

        public FullTextColumnsConfiguration(int monitoredServerId) : base(monitoredServerId)
        {
        }


        public FullTextColumnsConfiguration(int monitoredServerId, string databaseName, long? tableId) : base(monitoredServerId)
        {
            this.databaseName = databaseName;
            this.tableId = tableId;
        }

        #endregion

        #region properties

        new public bool ReadyForCollection
        {
            get { return (DatabaseName != null && TableId.HasValue); }
        }

        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        public long? TableId
        {
            get { return tableId; }
            set { tableId = value; }
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
