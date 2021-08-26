//------------------------------------------------------------------------------
// <copyright file="DatabaseMirroringSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents snapshot for resource views
    /// </summary>
    [Serializable]
    public sealed class MirrorMonitoringRealtimeSnapshot : Snapshot
    {
        #region fields

        private Dictionary<string, MirrorMonitoringDatabaseDetail> MirroredDatabases = new Dictionary<string, MirrorMonitoringDatabaseDetail>(StringComparer.InvariantCultureIgnoreCase);
        private TimeSpan? timeDelta = new TimeSpan(0);
        #endregion

        #region constructors

        /// <summary>
        /// Constructs with mirrored databases of the server in info.instancename
        /// </summary>
        /// <param name="info"></param>
        public MirrorMonitoringRealtimeSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
        }
        #endregion

        #region properties


        public Dictionary<string, MirrorMonitoringDatabaseDetail> Databases
        {
            get { return MirroredDatabases; }
            internal set { MirroredDatabases = value; }
        }

        public TimeSpan? TimeDelta
        {
            get { return timeDelta; }
            internal set { timeDelta = value; }
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
