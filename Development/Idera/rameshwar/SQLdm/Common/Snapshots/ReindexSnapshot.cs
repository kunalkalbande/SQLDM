//------------------------------------------------------------------------------
// <copyright file="ReindexSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Returns data from DBCC SHOWCONTIG after a reindex
    /// </summary>
    [Serializable]
    public class ReindexSnapshot : ServerActionSnapshot
    {
        #region fields

        private double? fragmentation = null;
        private string databaseName = null;
        private string tableName = null;
        private string indexName = null;
        private int? tableId = null;

        #endregion

        #region constructors

        internal ReindexSnapshot(string serverName)
            : base(serverName)
        {
            
        }

        #endregion

        #region properties

        /// <summary>
        /// Scan Density of reindexed object
        /// </summary>
        internal double? ScanDensity
        {
            set { fragmentation = 100d - value; }
        }

        /// <summary>
        /// Fragmentation of reindexed object
        /// </summary>
        public double? Fragmentation
        {
            get { return fragmentation; }
            set { fragmentation = value; }
        }

        /// <summary>
        /// Table name of reindexed object
        /// </summary>
        public string TableName
        {
            get { return tableName; }
            internal set { tableName = value; }
        }

        /// <summary>
        /// Database name of reindexed object.
        /// </summary>
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        /// <summary>
        /// Index name of reindexed object
        /// </summary>
        public string IndexName
        {
            get { return indexName; }
            internal set { indexName = value; }
        }

        /// <summary>
        /// Table ID of reindexed object
        /// </summary>
        public int? TableId
        {
            get { return tableId; }
            internal set { tableId = value; }
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
