//------------------------------------------------------------------------------
// <copyright file="AgentJobHistory.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents a single execution of a SQL Agent Job
    /// </summary>
    [Serializable]
    public class AgentJobHistory
    {
        #region fields

        private string name = null;
        private Guid? jobId = null;
        private List<AgentJobExecution> executions = new List<AgentJobExecution>();


        #endregion

        #region constructors


        public AgentJobHistory(string name, Guid? jobId)
        {
            this.name = name;
            this.jobId = jobId;
        }

        #endregion

        #region properties

        public List<AgentJobExecution> Executions
        {
            get { return executions; }
            internal set { executions = value; }
        }

        public string Name
        {
            get { return name; }
            internal set { name = value; }
        }

        public Guid? JobId
        {
            get { return jobId; }
            internal set { jobId = value; }
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
