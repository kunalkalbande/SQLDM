//------------------------------------------------------------------------------
// <copyright file="RarelyUsedIndexOnInMemoryTableSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the RarelyUsedIndexOnInMemoryTable info on a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class RarelyUsedIndexOnInMemoryTableSnapshot : Snapshot
    {
        #region fields

        private string edition = string.Empty;
        private DataTable rarelyUsedIndexOnInMemoryTableInfo = new DataTable("RarelyUsedIndexOnInMemoryTableInfo");

        #endregion

        #region constructors

        internal RarelyUsedIndexOnInMemoryTableSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            RarelyUsedIndexOnInMemoryTableInfo.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public string Edition
        {
            get { return edition; }
            set { edition = value; }
        }
        public DataTable RarelyUsedIndexOnInMemoryTableInfo
        {
            get { return rarelyUsedIndexOnInMemoryTableInfo; }
            internal set { rarelyUsedIndexOnInMemoryTableInfo = value; }
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
