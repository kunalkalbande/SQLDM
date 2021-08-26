//------------------------------------------------------------------------------
// <copyright file="TableSize.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents sizing information for a table
    /// </summary>
   [Serializable]
    public  class TableSize : Table, ICloneable
    {
        #region fields

        private FileSize dataSize = null;
        private FileSize imageTextSize = null;
        private FileSize indexSize = null;
        private long? rowCount = null;
        
        #endregion

        #region constructors

        public TableSize(string serverName, string databaseName, string tableName, string schema, bool? isSystemTable)
            : base(serverName, databaseName, tableName, schema, isSystemTable)
        {
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets data space used by table
        /// </summary>
        public FileSize DataSize
        {
            get { return dataSize; }
            internal set { dataSize = value; }
        }

        /// <summary>
        /// Gets image and text space used by table
        /// </summary>
        public FileSize ImageTextSize
        {
            get { return imageTextSize; }
            internal set { imageTextSize = value; }
        }

        /// <summary>
        /// Gets index space used by table
        /// </summary>
        public FileSize IndexSize
        {
            get { return indexSize; }
            internal set { indexSize = value; }
        }

        /// <summary>
        /// Gets row count of table
        /// </summary>
        public long? RowCount
        {
            get { return rowCount; }
            internal set { rowCount = value; }
        }

       
        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        public object Clone()
        {
            TableSize t = new TableSize(this.ServerName, this.Database, this.Name, this.Schema, this.IsSystemTable);
            t.DataSize = dataSize.Copy();
            t.ImageTextSize = imageTextSize.Copy();
            t.IndexSize = indexSize.Copy();
            t.RowCount = rowCount;
            return t;
        }

        #endregion

        #region nested types

        #endregion

    }
}
