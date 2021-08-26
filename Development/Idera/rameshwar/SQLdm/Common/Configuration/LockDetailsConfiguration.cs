//------------------------------------------------------------------------------
// <copyright file="LockDetailsConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.ComponentModel;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration object for lock details details on-demand probe
    /// </summary>
    [Serializable]
    public sealed class LockDetailsConfiguration : SessionSummaryConfiguration, IUserFilter
    {
        #region constants

        private const object DEFAULT_SPIDFILTER = null;     // change validation if default is not null
        private const bool DEFAULT_BLOCKING = false;
        private const bool DEFAULT_BLOCKED = false;
        private const bool DEFAULT_SHOW_TEMPDB = true;
        private const bool DEFAULT_SHOW_SHARED = true;

        #endregion

        #region fields

        private int? spidFilter = null;
        private bool filterForBlocking = DEFAULT_BLOCKING;
        private bool filterForBlocked = DEFAULT_BLOCKED;
        private bool showTempDbLocks = DEFAULT_SHOW_TEMPDB;
        private bool showSharedLocks = DEFAULT_SHOW_SHARED;

        #endregion

        #region constructors

        /// <summary>
        /// Initialize a new instance of the LockDetailsConfiguration class.
        /// </summary>
        /// <param name="monitoredServerId">SQL Server connection information.</param>
        /// <param name="filterForBlocking">If true, filter for blocking locks only</param>
        /// <param name="filterForBlocking">If true, filter for blocking locks</param>
        /// <param name="filterForBlocking">If true, filter for blocked locks</param>
        public LockDetailsConfiguration(int monitoredServerId, LockDetails lockDetails, bool filterForBlocking, bool filterForBlocked)
            : base(monitoredServerId)
        {
            this.filterForBlocking = filterForBlocking;
            this.filterForBlocked = filterForBlocked;
            if (lockDetails != null)
            {
                ServerStartTime = lockDetails.ServerStartupTime;
                PreviousLockStatistics = lockDetails.LockCounters;
            }
        }

        /// <summary>
        /// Initialize a new instance of the LockDetailsConfiguration class.
        /// </summary>
        /// <param name="monitoredServerId">SQL Server connection information.</param>
        public LockDetailsConfiguration(int monitoredServerId, LockDetails lockDetails)
            : this(monitoredServerId, lockDetails, DEFAULT_BLOCKING, DEFAULT_BLOCKED)
        {
        }

        #endregion

        #region properties

        /// <summary>
        /// SPID of the session to return locks for
        /// </summary>
        [DisplayName("Session ID"), Category("Locks")]
        [Description("Show only locks for the chosen Session ID")]
        [DefaultValue(DEFAULT_SPIDFILTER)]
        public int? SpidFilter
        {
            get { return spidFilter; }
            set { spidFilter = value; }
        }

        /// <summary>
        /// If true, filter for blocking locks
        /// </summary>
        [DisplayName("Show Blocking"), Category("Locks")]
        [Description("Show only locks which are blocking another Session")]
        [DefaultValue(DEFAULT_BLOCKING)]
        public bool FilterForBlocking
        {
            get { return filterForBlocking; }
            set { filterForBlocking = value; }
        }

        /// <summary>
        /// If true, filter for blocked locks
        /// </summary>
        [DisplayName("Show Blocked"), Category("Locks")]
        [Description("Show only locks which are blocked by another Session")]
        [DefaultValue(DEFAULT_BLOCKED)]
        public bool FilterForBlocked
        {
            get { return filterForBlocked; }
            set { filterForBlocked = value; }
        }

        /// <summary>
        /// If true, show tempDb locks
        /// </summary>
        [DisplayName("Show TempDB Locks"), Category("Locks")]
        [Description("Include locks on TempDB")]
        [DefaultValue(DEFAULT_SHOW_TEMPDB)]
        public bool ShowTempDbLocks
        {
            get { return showTempDbLocks; }
            set { showTempDbLocks = value; }
        }

        /// <summary>
        /// If true, show shared locks
        /// </summary>
        [DisplayName("Show Shared Locks"), Category("Locks")]
        [Description("Include shared locks")]
        [DefaultValue(DEFAULT_SHOW_TEMPDB)]
        public bool ShowSharedLocks
        {
            get { return showSharedLocks; }
            set { showSharedLocks = value; }
        }


        #endregion

        #region events

        #endregion

        #region methods

        public void SetPreviousSnapshot(LockDetails lockDetails)
        {
            if (lockDetails != null)
            {
                ServerStartTime = lockDetails.ServerStartupTime;
                PreviousLockStatistics = lockDetails.LockCounters;
            }
        }

        #endregion

        #region interface implementations

        #region IUserFilter Members

        public void ClearValues()
        {
            spidFilter = null;
            filterForBlocking = false;
            filterForBlocked = false;
            showTempDbLocks = true;
            showSharedLocks = false;
        }

        public bool HasDefaultValues()
        {
            if (spidFilter.HasValue     // handle this way because the default is null
                    || filterForBlocking != DEFAULT_BLOCKING
                    || filterForBlocked != DEFAULT_BLOCKED
                    || showTempDbLocks != DEFAULT_SHOW_TEMPDB
                    || showSharedLocks != DEFAULT_SHOW_SHARED
                )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsFiltered()
        {
            if (spidFilter.HasValue
                    || filterForBlocking
                    || filterForBlocked
                    || !showTempDbLocks
                    || showSharedLocks
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ResetValues()
        {
            spidFilter = (int?)DEFAULT_SPIDFILTER;
            filterForBlocking = DEFAULT_BLOCKING;
            filterForBlocked = DEFAULT_BLOCKED;
            showTempDbLocks = DEFAULT_SHOW_TEMPDB;
            showSharedLocks = DEFAULT_SHOW_SHARED;
        }

        public void UpdateValues(IUserFilter selectionFilter)
        {
            if (selectionFilter is LockDetailsConfiguration)
            {
                LockDetailsConfiguration filter = (LockDetailsConfiguration)selectionFilter;
                spidFilter = filter.SpidFilter;
                filterForBlocking = filter.FilterForBlocking;
                filterForBlocked = filter.FilterForBlocked;
                showTempDbLocks = filter.ShowTempDbLocks;
                showSharedLocks = filter.ShowSharedLocks;
            }
        }

        public bool Validate(out string Message)
        {
            Message = String.Empty;
            return true;
        }

        #endregion

        #endregion

        #region nested types

        #endregion

    }
}
