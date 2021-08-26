//------------------------------------------------------------------------------
// <copyright file="SessionSummaryConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.ComponentModel;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration object for sessions summary on-demand probe
    /// </summary>
    [Serializable]
    public class SessionSummaryConfiguration : OnDemandConfiguration
    {
        #region fields

        private LockStatistics previousLockStatistics = null;
        private DateTime? serverStartTime = null;
        private String searchTerm = null;

        #endregion

        #region constructors

        public SessionSummaryConfiguration(int monitoredServerId) : base(monitoredServerId)
        {
        }


        public SessionSummaryConfiguration(int monitoredServerId, SessionSummary sessionSummary)
            : base(monitoredServerId)
        {
            if (sessionSummary != null)
            {
                PreviousLockStatistics = sessionSummary.LockCounters;
                serverStartTime = sessionSummary.ServerStartupTime;
            }
        }

        #endregion

        #region properties

        [Browsable(false)]
        public LockStatistics PreviousLockStatistics
        {
            get { return previousLockStatistics; }
            protected set { previousLockStatistics = value; }
        }

        [Browsable(false)]
        public DateTime? ServerStartTime
        {
            get { return serverStartTime; }
            protected set { serverStartTime = value; }
        }

        [Browsable(false)]
        public String SearchTerm
        {
            get { return searchTerm; }
            set { searchTerm = value; }
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
