//------------------------------------------------------------------------------
// <copyright file="BombedJobs.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using Wintellect.PowerCollections;

    /// <summary>
    /// Represents a collection of failed jobs
    /// </summary>
    [Serializable]
    public sealed class BombedJobs
    {
        #region fields

        private List<AgentJobFailure> jobList = new List<AgentJobFailure>();
        private List<AgentJobClear> clearJobList = new List<AgentJobClear>();
        private int lastInstanceId = 0;
        //private List<Guid> failedJobGuids = new List<Guid>();
        private List<Pair<Guid, int>> failedJobSteps = new List<Pair<Guid, int>>();
        #endregion

        #region constructors

        #endregion

        #region properties

        /// <summary>
        /// List of failed jobs
        /// </summary>
        public List<AgentJobFailure> JobList
        {
            get { return jobList; }
            internal set { jobList = value; }
        }

        /// <summary>
        /// List of jobs to clear
        /// </summary>
        public List<AgentJobClear> ClearJobList
        {
            get { return clearJobList; }
            internal set { clearJobList = value; }
        }

        /// <summary>
        /// Last instanceID checked for failed jobs
        /// Used to prevent re-alerting
        /// </summary>
        public int LastInstanceId
        {
            get { return lastInstanceId; }
            internal set { lastInstanceId = value; }
        }

        ///// <summary>
        ///// This is to shortcut a round-trip to the Persistence Manager only
        ///// </summary>
        //public List<Guid> FailedJobGuids
        //{
        //    get { return failedJobGuids; }
        //    internal set { failedJobGuids = value; }
        //}

        /// <summary>
        /// This is to shortcut a round-trip to the Persistence Manager only
        /// </summary>
        public List<Pair<Guid, int>> FailedJobSteps
        {
            get { return failedJobSteps; }
            internal set { failedJobSteps = value; }
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
