//------------------------------------------------------------------------------
// <copyright file="CompletedJobs.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Idera.SQLdm.Common.Events;
using System.Collections.Generic;


namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Details of SQL Server Agent Jobs that have completed.
    /// </summary>
    [Serializable]
    public sealed class CompletedJobs 
    {
        #region fields

        private int lastInstanceId = 0;
        private List<AgentJobCompletion> jobList = new List<AgentJobCompletion>();

        #endregion

        #region constructors

        #endregion

        #region properties

        public int LastInstanceId
        {
            get { return lastInstanceId; }
            internal set { lastInstanceId = value; }
        }

        public List<AgentJobCompletion> JobList
        {
            get { return jobList; }
            internal set { jobList = value; }
        }

        #endregion

    }
}
