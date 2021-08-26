//------------------------------------------------------------------------------
// <copyright file="DisabledIndexesSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the DisabledIndexes information on a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class DisabledIndexesSnapshot : Snapshot
    {
        #region fields

        private DataTable disabledIndexes = new DataTable("DisabledIndexes");

        #endregion

        #region constructors

        internal DisabledIndexesSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            disabledIndexes.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public DataTable DisabledIndexes
        {
            get { return disabledIndexes; }
            internal set { disabledIndexes = value; }
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
