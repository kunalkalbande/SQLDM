//------------------------------------------------------------------------------
// <copyright file="AdhocCachedPlanBytesSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the AdhocCachedPlanBytes info of a monitored server //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class AdhocCachedPlanBytesSnapshot : Snapshot
    {
        #region fields

        private DataTable adhocCachedPlanBytes = new DataTable("AdhocCachedPlanBytes");
        #endregion

        #region constructors
        public AdhocCachedPlanBytesSnapshot()
        { }

        internal AdhocCachedPlanBytesSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            adhocCachedPlanBytes.RemotingFormat = SerializationFormat.Binary;
        }

        #endregion

        #region properties

        public DataTable AdhocCachedPlanBytes
        {
            get { return adhocCachedPlanBytes; }
            internal set { adhocCachedPlanBytes = value; }
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
