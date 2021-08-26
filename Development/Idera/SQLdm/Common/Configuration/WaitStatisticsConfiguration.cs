//------------------------------------------------------------------------------
// <copyright file="WaitStatsConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration object for wait statistics on-demand probe
    /// </summary>
    [Serializable]
    public class WaitStatisticsConfiguration : OnDemandConfiguration
    {
        #region fields

        private Dictionary<string, Wait> previousWaits = new Dictionary<string, Wait>();
        private DateTime? previousTimeStamp;
        private DateTime? previousServerStartup;

        #endregion

        #region constructors

        public WaitStatisticsConfiguration(int monitoredServerId, WaitStatisticsSnapshot previousRefresh)
            : base(monitoredServerId)
        {
            previousWaits = previousRefresh == null ? null : previousRefresh.Waits;
            previousTimeStamp = previousRefresh == null ? (DateTime?)null : previousRefresh.TimeStamp;
            previousServerStartup = previousRefresh == null ? (DateTime?)null : previousRefresh.ServerStartupTime;
        }

        
        #endregion

        #region properties

        public Dictionary<string, Wait> PreviousWaits
        {
            get { return previousWaits; }
            internal set { previousWaits = value; }
        }

        public DateTime? PreviousTimeStamp
        {
            get { return previousTimeStamp; }
        }

        public DateTime? PreviousServerStartup
        {
            get { return previousServerStartup; }
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
