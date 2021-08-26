//------------------------------------------------------------------------------
// <copyright file="ScheduledRefreshAlerts.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using Wintellect.PowerCollections;

    /// <summary>
    /// Holds alert-related refresh data
    /// </summary>
    [Serializable]
    public sealed class ScheduledRefreshAlerts
    {
        #region fields

        private bool? agentXpEnabled = null;
        private List<BlockingSession> blockingSessions = new List<BlockingSession>();
        private List<ProbePermissionHelpers.ProbeError> probePermissionErrors = new List<ProbePermissionHelpers.ProbeError>();
        private bool? clrEnabled = null;
        private bool? commandShellEnabled = null;
        private BombedJobs jobFailures = new BombedJobs();
        private List<AgentJobRunning> longRunningJobs = new List<AgentJobRunning>();
        private List<OpenTransaction> oldestOpenTransactions = new List<OpenTransaction>();
        private List<Session> resourceCheckSessions = new List<Session>();
        private List<Triple<MonitoredState, String, bool>> errorLogScanLines = new List<Triple<MonitoredState, String, bool>>();
        private MonitoredState errorLogMostSevereMatch = MonitoredState.OK;
        private List<Triple<MonitoredState, String, bool>> agentLogScanLines = new List<Triple<MonitoredState, String, bool>>();
        private MonitoredState agentLogMostSevereMatch = MonitoredState.OK;
        private DateTime? logScanDate = null;
        private Exception logScanFailure = null;
        private CompletedJobs jobsCompleted = new CompletedJobs();
        private long errorLogCurrentFileSizeInBytes = -1;
        private long agentLogCurrentFileSizeInBytes = -1;

        #endregion

        #region constructors

        #endregion

        #region properties

        public bool? AgentXpEnabled
        {
            get { return agentXpEnabled; }
            internal set { agentXpEnabled = value; }
        }
        /// <summary>
        /// List of blocking sessions gathered from sys.processes.
        /// This gets processed by the scheduledrefresheventprocessor
        /// </summary>
        public List<BlockingSession> BlockingSessions
        {
            get { return blockingSessions; }
            internal set { blockingSessions = value; }
        }

        /// <summary>
        /// List of Probe Permission Errors gathered from ScheduledCollection Probes
        /// This gets processed by the scheduledrefresheventprocessor
        /// </summary>
        public List<ProbePermissionHelpers.ProbeError> ProbePermissionErrors
        {
            get { return probePermissionErrors; }
            internal set { probePermissionErrors = value; }
        }

        public bool? ClrEnabled
        {
            get { return clrEnabled; }
            internal set { clrEnabled = value; }
        }

        public bool? CommandShellEnabled
        {
            get { return commandShellEnabled; }
            internal set { commandShellEnabled = value; }
        }

        public BombedJobs JobFailures
        {
            get { return jobFailures; }
            internal set { jobFailures = value; }
        }


        public List<AgentJobRunning> LongRunningJobs
        {
            get { return longRunningJobs; }
            internal set { longRunningJobs = value; }
        }


        public List<OpenTransaction> OldestOpenTransactions
        {
            get { return oldestOpenTransactions; }
            internal set { oldestOpenTransactions = value; }
        }


        public List<Session> ResourceCheckSessions
        {
            get { return resourceCheckSessions; }
            internal set { resourceCheckSessions = value; }
        }

        /// <summary>
        /// Represents datetime of log line, string, and bool for 
        /// whether this is the matched event (if false, this is a 
        /// preceeding or succeeding line)
        /// </summary>
        public List<Triple<MonitoredState, String, bool>> ErrorLogScanLines
        {
            get { return errorLogScanLines; }
            internal set { errorLogScanLines = value; }
        }


        public MonitoredState ErrorLogMostSevereMatch
        {
            get { return errorLogMostSevereMatch; }
            internal set { 
                if (value > errorLogMostSevereMatch)
                    errorLogMostSevereMatch = value; 
            }
        }

        /// <summary>
        /// The size of the current ErrorLog File in Bytes or -1 if less that limit
        /// </summary>
        public long ErrorLogFileSizeInBytes
        {
            get { return errorLogCurrentFileSizeInBytes; }
            set { errorLogCurrentFileSizeInBytes = value; }
        }
        /// <summary>
        /// The size of the current agent log file in Bytes or -1 if less that limit
        /// </summary>
        public long AgentLogFileSizeInBytes
        {
            get { return agentLogCurrentFileSizeInBytes; }
            set { agentLogCurrentFileSizeInBytes = value; }
        }

        /// <summary>
        /// Represents datetime of log line, string, and bool for 
        /// whether this is the matched event (if false, this is a 
        /// preceeding or succeeding line)
        /// </summary>
        public List<Triple<MonitoredState, String, bool>> AgentLogScanLines
        {
            get { return agentLogScanLines; }
            internal set { agentLogScanLines = value; }
        }


        public MonitoredState AgentLogMostSevereMatch
        {
            get { return agentLogMostSevereMatch; }
            internal set
            {
                if (value > agentLogMostSevereMatch)
                    agentLogMostSevereMatch = value;
            }
        }

        //public MonitoredState ErrorlogMostSevereMatch
        //{
        //    get { return errorLogMostSevereMatch; }
        //    internal set
        //    {
        //        if (value > errorLogMostSevereMatch)
        //            errorLogMostSevereMatch = value;
        //    }
        //}

        public DateTime? LogScanDate
        {
            get { return logScanDate; }
            internal set { logScanDate = value; }
        }


        public Exception LogScanFailure
        {
            get { return logScanFailure; }
            internal set
            {
                if (logScanFailure == null)
                    logScanFailure = value;
            }
        }

        public CompletedJobs JobsCompleted
        {
            get { return jobsCompleted; }
            internal set { jobsCompleted = value; }
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
