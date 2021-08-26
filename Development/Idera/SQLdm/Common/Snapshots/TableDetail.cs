//------------------------------------------------------------------------------
// <copyright file="TableDetails.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents detailed diagnostic information about an individual table
    /// </summary>
    [Serializable]
    public sealed class TableDetail  : Snapshot
    {

        #region fields

        private string databaseName = null;
        private string tableName = null;
        private string schema = null;
        private int? tableId = null;
        private List<TableDependency> referencedBy = new List<TableDependency>();
        private List<TableDependency> references = new List<TableDependency>();
        private List<Index> indexes = new List<Index>();
        
        #endregion

        #region constructors

        internal TableDetail(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            
        }

        #endregion

        #region properties

        public string DatabaseName
        {
            get { return databaseName; }
            internal set { databaseName = value; }
        }

        public string TableName
        {
            get { return tableName; }
            internal set { tableName = value; }
        }

        public string Schema
        {
            get { return schema; }
            internal set { schema = value; }
        }


        public int? TableId
        {
            get { return tableId; }
            internal set { tableId = value; }
        }

        /// <summary>
        /// Gets an array of indexes that belong to the table
        /// </summary>
        public List<Index> Indexes
        {
            get { return indexes; }
            internal set { indexes = value; }
        }

    

        /// <summary>
        /// Gets objects referenced by the table
        /// </summary>
        public List<TableDependency> ReferencedBy
        {
            get { return referencedBy; }
            internal set { referencedBy = value; }
        }

        /// <summary>
        /// Gets objects that reference the table
        /// </summary>
        public List<TableDependency> References
        {
            get { return references; }
            internal set { references = value; }
        }

   

        #endregion

        #region method

       
        #endregion

    }
}
