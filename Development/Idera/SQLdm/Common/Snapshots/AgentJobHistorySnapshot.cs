//------------------------------------------------------------------------------
// <copyright file="AgentJobHistorySnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents a collection of job histories
    /// </summary>
   [Serializable]
    public class AgentJobHistorySnapshot : Snapshot
    {
        #region fields

        private Dictionary<Guid, AgentJobHistory> jobHistories = new Dictionary<Guid, AgentJobHistory>();

        #endregion

        #region constructors

        internal AgentJobHistorySnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            
        }

        #endregion

        #region properties

        public Dictionary<Guid, AgentJobHistory> JobHistories
        {
            get { return jobHistories; }
            internal set { jobHistories = value; }
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
