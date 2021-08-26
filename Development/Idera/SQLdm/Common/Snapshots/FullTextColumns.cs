//------------------------------------------------------------------------------
// <copyright file="FullTextColumns.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents all full text indexed Columns for a selected catalog
    /// </summary>
    [Serializable]
    public class FullTextColumns : Snapshot
    {
        #region fields

        private DataTable ftColumns = new DataTable("Columns");
        #endregion

        #region constructors

        internal FullTextColumns(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            FtColumns.RemotingFormat = SerializationFormat.Binary;
            FtColumns.Columns.Add("Name", typeof(string));
            FtColumns.Columns.Add("Type", typeof(string));
            FtColumns.Columns.Add("Language", typeof(string));
            FtColumns.Columns.Add("Fulltext Indexed", typeof(bool));
        }

        #endregion

        #region properties

        public DataTable FtColumns
        {
            get { return ftColumns; }
            internal set { ftColumns = value; }
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
