//------------------------------------------------------------------------------
// <copyright file="JobControlConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Configuration.ServerActions
{
    using System;

    /// <summary>
    /// Start a SQL Agent job
    /// </summary>
    [Serializable]
    public class JobControlConfiguration : OnDemandConfiguration, IServerActionConfiguration
    {
        #region fields

        private string jobName = null;
        private string jobStep = null;
        private JobControlAction action = JobControlAction.Start;

        #endregion

        #region constructors


        public JobControlConfiguration(int monitoredServerId, string jobName, JobControlAction action)
            : this(monitoredServerId, jobName, null, action)
        {
            this.jobName = jobName;
            this.action = action;
        }

        public JobControlConfiguration(int monitoredServerId, string jobName, string jobStep, JobControlAction action)
            : base(monitoredServerId)
        {
            this.jobName = jobName;
            this.jobStep = jobStep;
            this.action = action;
        }


        #endregion

        #region properties

        public string JobName
        {
            get { return jobName; }
            set { jobName = value; }
        }

        public string JobStep
        {
            get { return jobStep; }
            set { jobStep = value; }
        }


        public JobControlAction Action
        {
            get { return action; }
            set { action = value; }
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
