//------------------------------------------------------------------------------
// <copyright file="JobSummary.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents a summary of SQL Agent jobs
    /// </summary>
    [Serializable]
    public class AgentJobSummary : Snapshot
    {
        #region fields

        private ServiceState agentServiceState = ServiceState.UnableToMonitor;
        private Dictionary<Guid, AgentJob> jobs = new Dictionary<Guid, AgentJob>();

        #endregion

        #region constructors

        internal AgentJobSummary(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            
        }

        #endregion

        #region properties

        public ServiceState AgentServiceState
        {
            get { return agentServiceState; }
            internal set { agentServiceState = value; }
        }

        public Dictionary<Guid, AgentJob> Jobs
        {
            get { return jobs; }
            internal set { jobs = value; }
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
