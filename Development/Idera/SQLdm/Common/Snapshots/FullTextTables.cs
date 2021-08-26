//------------------------------------------------------------------------------
// <copyright file="FullTextTables.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents all full text indexed tables for a selected catalog
    /// </summary>
    [Serializable]
    public class FullTextTables : Snapshot
    {
        #region fields

        private DataTable tables = new DataTable("Tables");
        #endregion

        #region constructors

        internal FullTextTables(SqlConnectionInfo info)
            : base(info.InstanceName)
         {
             tables.RemotingFormat = SerializationFormat.Binary;
             DataColumn[] keys = new DataColumn[1];

             tables.Columns.Add("Name", typeof(string));
             tables.Columns.Add("Schema name", typeof(string));
             tables.Columns.Add("Rows", typeof(long));

            DataColumn column;
             column = new DataColumn();
             column.DataType = typeof(long);
             column.ColumnName = "TableID";

             keys[0] = column;
             tables.Columns.Add(column);

             tables.PrimaryKey = keys;
         }

        #endregion

        #region properties

        public DataTable Tables
        {
            get { return tables; }
            internal set { tables = value; }
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
