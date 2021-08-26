//------------------------------------------------------------------------------
// <copyright file="TableGrowth.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents reorganization data for a table
    /// </summary>
    [Serializable]
    public sealed class TableReorganization : Table,ICloneable
    {
        #region fields

        private double? scanDensity = null;
        private double? logicalFragmentation = null;
        
        #endregion

        #region constructors

        public TableReorganization(string serverName, string databaseName, string tableName, string schemaName, bool? isSystemTable)
            : base(serverName, databaseName, tableName, schemaName, isSystemTable)
        {
        }

        #endregion

        #region properties


        /// <summary>
        /// Scan density from DBCC SHOWCONTIG
        /// Shows table reorganization in percent
        /// </summary>
        public double? ScanDensity
        {
            get { return scanDensity; }
            internal set { scanDensity = value; }
        }
        
        /// <summary>
        /// Logical fragmentation of clustered index
        /// </summary>
        public double? LogicalFragmentation
        {
            get { return logicalFragmentation; }
            internal set { logicalFragmentation = value; }
        }
        
        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations


        public object Clone()
        {
            TableReorganization t = new TableReorganization(this.ServerName, this.Database, this.Name, this.Schema, this.IsSystemTable);
            t.ScanDensity = scanDensity;
            t.LogicalFragmentation = logicalFragmentation;
            return t;
        }

        #endregion

        #region nested types

        #endregion

    }
}
