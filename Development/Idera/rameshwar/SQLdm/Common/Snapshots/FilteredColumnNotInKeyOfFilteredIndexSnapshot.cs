//------------------------------------------------------------------------------
// <copyright file="FilteredColumnNotInKeyOfFilteredIndexSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the FilteredColumnNotInKeyOfFilteredIndex info on a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class FilteredColumnNotInKeyOfFilteredIndexSnapshot : Snapshot
    {
        #region fields

        private DataTable filteredColumnNotInKeyOfFilteredIndexInfo = new DataTable("FilteredColumnNotInKeyOfFilteredIndexInfo");

        #endregion

        #region constructors

        internal FilteredColumnNotInKeyOfFilteredIndexSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            filteredColumnNotInKeyOfFilteredIndexInfo.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public DataTable FilteredColumnNotInKeyOfFilteredIndexInfo
        {
            get { return filteredColumnNotInKeyOfFilteredIndexInfo; }
            internal set { filteredColumnNotInKeyOfFilteredIndexInfo = value; }
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
