//------------------------------------------------------------------------------
// <copyright file="LockedPageKBSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the LockedPageKB info of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class LockedPageKBSnapshot : Snapshot
    {
        #region fields

        private DataTable lockedPageKB = new DataTable("LockedPageKB");
        #endregion

        #region constructors

        internal LockedPageKBSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            lockedPageKB.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public DataTable LockedPageKB
        {
            get { return lockedPageKB; }
            internal set { lockedPageKB = value; }
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
