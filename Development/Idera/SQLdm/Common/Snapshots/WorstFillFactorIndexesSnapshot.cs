//------------------------------------------------------------------------------
// <copyright file="WorstFillFactorIndexesSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the worst Fill Factor Indexes information on a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class WorstFillFactorIndexesSnapshot : Snapshot
    {
        #region fields

        private DataTable worstFillFactorIndexes = new DataTable("WorstFillFactorIndexes");

        #endregion

        #region constructors

        internal WorstFillFactorIndexesSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            worstFillFactorIndexes.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public DataTable WorstFillFactorIndexes
        {
            get { return worstFillFactorIndexes; }
            internal set { worstFillFactorIndexes = value; }
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
