//------------------------------------------------------------------------------
// <copyright file="TableActionBaseConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration.ServerActions
{
    using System;

    /// <summary>
    /// Base class for reindex and update statistics
    /// </summary>
    [Serializable]
    public abstract class TableActionBaseConfiguration : OnDemandConfiguration, IServerActionConfiguration
    {
         #region fields

        private string databaseName = null;
        private int? tableId = null;
        private string indexName = null;

        #endregion

        #region constructors

        public TableActionBaseConfiguration(int monitoredServerId, string databaseName, int? tableId) : base(monitoredServerId)
        {
            this.databaseName = databaseName;
            this.tableId = tableId;
            indexName = null;
        }

        public TableActionBaseConfiguration(int monitoredServerId, string databaseName, int? tableId, string indexName)
            : base(monitoredServerId)
        {
            this.databaseName = databaseName;
            this.tableId = tableId;
            this.indexName = indexName;
        }

        #endregion

        #region properties

        public string DatabaseName
        {
            get { return databaseName; }
        }

        public int? TableId
        {
            get { return tableId; }
        }

        public string IndexName
        {
            get { return indexName; }
            set { indexName = value; }
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
