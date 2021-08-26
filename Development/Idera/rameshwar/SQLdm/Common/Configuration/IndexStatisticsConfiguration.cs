//------------------------------------------------------------------------------
// <copyright file="IndexStatisticsConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration for Index Statistics on demand probe
    /// </summary>
    [Serializable]
    public sealed class IndexStatisticsConfiguration:OnDemandConfiguration
    {
        #region fields

        private string databaseName = null;
        private string indexName = null;
        private int? tableId = null;

        #endregion

        #region constructors

        public IndexStatisticsConfiguration(int monitoredServerId) : base(monitoredServerId)
        {
        }

        public IndexStatisticsConfiguration(int monitoredServerId, string databaseName, string indexName, int? tableId) : base(monitoredServerId)
        {
            this.databaseName = databaseName;
            this.indexName = indexName;
            this.tableId = tableId;
        }

        #endregion

        #region properties

        new public bool ReadyForCollection
        {
            get { return (DatabaseName != null && DatabaseName != null && TableId.HasValue); }
        }

        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        public string IndexName
        {
            get { return indexName; }
            set { indexName = value; }
        }

        public int? TableId
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
