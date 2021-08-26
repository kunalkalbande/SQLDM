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
    public sealed class MirrorMonitoringHistorySnapshot : Snapshot
    {
        #region fields

        private string _DatabaseName;
        private List<MirroringMetrics> _MirroringMetrics = new List<MirroringMetrics>();
        private TimeSpan? timeDelta = new TimeSpan(0);
        #endregion

        #region constructors

        /// <summary>
        /// Constructs with mirrored databases of the server in info.instancename
        /// </summary>
        /// <param name="info"></param>
        public MirrorMonitoringHistorySnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
        }
        #endregion

        #region properties


        public List<MirroringMetrics> Metrics
        {
            get { return _MirroringMetrics; }
            internal set { _MirroringMetrics = value; }
        }

        public TimeSpan? TimeDelta
        {
            get { return timeDelta; }
            internal set { timeDelta = value; }
        }

        public string MirroredDatabase
        {
            get { return _DatabaseName; }
            internal set { _DatabaseName = value; }
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
