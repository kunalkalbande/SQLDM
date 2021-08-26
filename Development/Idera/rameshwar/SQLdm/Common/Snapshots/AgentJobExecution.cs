//------------------------------------------------------------------------------
// <copyright file="AgentJobExecution.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents a single execution of a job
    /// </summary>
    [Serializable]
    public class AgentJobExecution
    {
        #region fields

        private List<AgentJobStep> steps = new List<AgentJobStep>();
        private AgentJobStep outcome = new AgentJobStep();

        #endregion

        #region constructors

        public AgentJobExecution(List<AgentJobStep> steps, AgentJobStep outcome)
        {
            this.steps = steps;
            this.outcome = outcome;
        }

        #endregion

        #region properties

        public List<AgentJobStep> Steps
        {
            get { return steps; }
            internal set { steps = value; }
        }

        public AgentJobStep Outcome
        {
            get { return outcome; }
            internal set { outcome = value; }
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
