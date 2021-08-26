//------------------------------------------------------------------------------
// <copyright file="OverlappingIndexesSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
//using System.Collections.Generic;

    /// <summary>
    /// Represents the OverlappingIndexes information on a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class OverlappingIndexesSnapshot : Snapshot
    {
        #region fields

        private DataTable duplicateIndexInfo = new DataTable("DuplicateIndexInfo");
        private DataTable partialDuplicateIndexInfo = new DataTable("PartialDuplicateIndexInfo");

        #endregion

        #region constructors

        internal OverlappingIndexesSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            duplicateIndexInfo.RemotingFormat = SerializationFormat.Binary;
            partialDuplicateIndexInfo.RemotingFormat = SerializationFormat.Binary;
            InitializeDuplicateIndexInfo();
            InitializePartialDuplicateIndexInfo();
        }

        #endregion

        #region properties

        public DataTable DuplicateIndexInfo
        {
            get { return duplicateIndexInfo; }
            internal set { duplicateIndexInfo = value; }
        }

        public DataTable PartialDuplicateIndexInfo
        {
            get { return partialDuplicateIndexInfo; }
            internal set { partialDuplicateIndexInfo = value; }
        }


        #endregion

        #region events

        #endregion

        #region methods

        private void InitializeDuplicateIndexInfo()
        {
            DuplicateIndexInfo.Columns.Add("DupIndex", typeof(string));//
            DuplicateIndexInfo.Columns.Add("DatabaseName", typeof(string));
            DuplicateIndexInfo.Columns.Add("Schema", typeof(string));

            DuplicateIndexInfo.Columns.Add("TableName", typeof(string));
            DuplicateIndexInfo.Columns.Add("IndexName", typeof(string));
            DuplicateIndexInfo.Columns.Add("IndexId", typeof(int));//

            DuplicateIndexInfo.Columns.Add("IndexUnique", typeof(bool));
            DuplicateIndexInfo.Columns.Add("IndexPrimaryKey", typeof(bool));
            DuplicateIndexInfo.Columns.Add("IndexUsage", typeof(long));

            DuplicateIndexInfo.Columns.Add("IndexUpdates", typeof(long));
            DuplicateIndexInfo.Columns.Add("IndexKeySize", typeof(int));//
            DuplicateIndexInfo.Columns.Add("IndexForeignKeys", typeof(int));//

            DuplicateIndexInfo.Columns.Add("DupIndexName", typeof(string));
            DuplicateIndexInfo.Columns.Add("DupIndexId", typeof(int));//
            DuplicateIndexInfo.Columns.Add("DupIndexUnique", typeof(bool));

            DuplicateIndexInfo.Columns.Add("DupIndexPrimaryKey", typeof(bool));
            DuplicateIndexInfo.Columns.Add("DupIndexUsage", typeof(long));
            DuplicateIndexInfo.Columns.Add("DupIndexUpdates", typeof(long));

            DuplicateIndexInfo.Columns.Add("DupIndexKeySize", typeof(int));//
            DuplicateIndexInfo.Columns.Add("DupIndexForeignKeys", typeof(int));//
        }

        private void InitializePartialDuplicateIndexInfo()
        {

            PartialDuplicateIndexInfo.Columns.Add("DupIndex", typeof(string));//
            PartialDuplicateIndexInfo.Columns.Add("DatabaseName", typeof(string));
            PartialDuplicateIndexInfo.Columns.Add("Schema", typeof(string));

            PartialDuplicateIndexInfo.Columns.Add("TableName", typeof(string));
            PartialDuplicateIndexInfo.Columns.Add("IndexName", typeof(string));
            PartialDuplicateIndexInfo.Columns.Add("IndexId", typeof(int));//

            PartialDuplicateIndexInfo.Columns.Add("IndexUsage", typeof(long));
            PartialDuplicateIndexInfo.Columns.Add("IndexUpdates", typeof(long));
            PartialDuplicateIndexInfo.Columns.Add("IndexKeySize", typeof(long));

            PartialDuplicateIndexInfo.Columns.Add("IndexUnique", typeof(bool));
            PartialDuplicateIndexInfo.Columns.Add("IndexPrimaryKey", typeof(bool));
            PartialDuplicateIndexInfo.Columns.Add("IndexForeignKeys", typeof(int));//

            PartialDuplicateIndexInfo.Columns.Add("IndexIncludeCols", typeof(string));
            PartialDuplicateIndexInfo.Columns.Add("IndexCols", typeof(string));

            PartialDuplicateIndexInfo.Columns.Add("DupIndexName", typeof(string));
            PartialDuplicateIndexInfo.Columns.Add("DupIndexUsage", typeof(long));
            PartialDuplicateIndexInfo.Columns.Add("DupIndexUpdates", typeof(long));

            PartialDuplicateIndexInfo.Columns.Add("DupIndexId", typeof(int));//
            PartialDuplicateIndexInfo.Columns.Add("DupIndexUnique", typeof(bool));
            PartialDuplicateIndexInfo.Columns.Add("DupIndexPrimaryKey", typeof(bool));

            PartialDuplicateIndexInfo.Columns.Add("DupIndexKeySize", typeof(long));
            PartialDuplicateIndexInfo.Columns.Add("DupIndexForeignKeys", typeof(int));//

            PartialDuplicateIndexInfo.Columns.Add("DupIndexIncludeCols", typeof(string));
            PartialDuplicateIndexInfo.Columns.Add("DupIndexCols", typeof(string));
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }


}
