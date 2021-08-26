//------------------------------------------------------------------------------
// <copyright file="FragmentedIndexesSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the configuration settings on a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class FragmentedIndexesSnapshot : Snapshot
    {
        #region fields

        private DataTable fragmentedIndexes = new DataTable("FragmentedIndexes");

        #endregion

        #region constructors

        internal FragmentedIndexesSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            fragmentedIndexes.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public DataTable FragmentedIndexes
        {
            get { return fragmentedIndexes; }
            internal set { fragmentedIndexes = value; }
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
