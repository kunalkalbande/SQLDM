//------------------------------------------------------------------------------
// <copyright file="Table.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents basic information on a table
    /// </summary>
    [Serializable]
    public class Table 
    {
        #region fields

        private string serverName = null;
        private string database = null;
        private string name = null;
        private string schema = null;
        private bool? isSystemTable = null;
        
        #endregion

        #region constructors

        public Table(string serverName, string databaseName, string tableName, string schema, bool? isSystemTable)
        {
            this.serverName = serverName;
            database = databaseName;
            name = tableName;
            this.schema = schema;
            this.isSystemTable = isSystemTable;
        }

        #endregion

        #region properties

        public string ServerName
        {
            get { return serverName; }
            internal set { serverName = value; }
        }

        /// <summary>
        /// Gets database to which object belongs
        /// </summary>
        public string Database
        {
            get { return database; }
        }

        /// <summary>
        /// Gets object name
        /// </summary>		
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets object's full name in [schemaname].[objectname] format
        /// </summary>
        public string FullName
        {
            get { return "[" + schema + "].[" + name + "]"; }
        }

        /// <summary>
        /// Gets table schema name
        /// </summary>
        public string Schema
        {
            get { return schema; }
        }


        public bool? IsSystemTable
        {
            get { return isSystemTable; }
            internal set { isSystemTable = value; }
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
