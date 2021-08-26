//------------------------------------------------------------------------------
// <copyright file="TableSummary.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents summary data on database tables
    /// </summary>
    [Serializable]
    public sealed class TableSummary: Snapshot
    {
        #region fields

        private DataTable tableList = new DataTable("TableSummary");
        private FileSize totalDataSize = new FileSize();
        private FileSize totalIndexSize = new FileSize();
        private FileSize totalImageSize = new FileSize();
        private float? compatibilityLevel = null;

        #endregion

        #region constructors

        internal TableSummary(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            TableList.RemotingFormat = SerializationFormat.Binary;
            TableList.Columns.Add("ObjectID", typeof (int));
            TableList.Columns.Add("DatabaseName", typeof (string));
            TableList.Columns.Add("TableName", typeof (string));
            TableList.Columns.Add("DataSize", typeof (FileSize));
            TableList.Columns.Add("IndexSize", typeof (FileSize));
            TableList.Columns.Add("TextSize", typeof (FileSize));
            TableList.Columns.Add("RowCount", typeof (long));
            TableList.Columns.Add("Schema/Owner", typeof (string));
            TableList.Columns.Add("IsSystemTable", typeof (bool));
            TableList.Columns.Add("DataSpaceName", typeof (string));
            TableList.Columns.Add("CreateDate", typeof (DateTime));
            TableList.Columns.Add("BCPLocked", typeof (bool));
            TableList.Columns.Add("FulltextCatalog", typeof (string));
            TableList.Columns.Add("Pinned", typeof (bool));
            TableList.Columns.Add("DataSpaceType", typeof(string));
        }

        #endregion

        #region properties

        public float? CompatibilityLevel
        {
            get { return compatibilityLevel; }
            set { compatibilityLevel = value; }
        }

        public DataTable TableList
        {
            get { return tableList; }
            internal set { tableList = value; }
        }

        public FileSize TotalDataSize
        {
            get { return totalDataSize; }
            internal set { totalDataSize = value; }
        }

        public FileSize TotalIndexSize
        {
            get { return totalIndexSize; }
            internal set { totalIndexSize = value; }
        }

        public FileSize TotalImageSize
        {
            get { return totalImageSize; }
            internal set { totalImageSize = value; }
        }

        public FileSize TotalDatabaseSize
        {
            get
            {
                return new FileSize(
                    TotalDataSize.Kilobytes
                    + TotalImageSize.Kilobytes
                    + TotalIndexSize.Kilobytes);
            }
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
