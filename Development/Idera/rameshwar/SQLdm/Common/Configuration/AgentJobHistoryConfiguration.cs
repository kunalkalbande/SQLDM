//------------------------------------------------------------------------------
// <copyright file="AgentJobHistoryConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;

namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration object for Agent Job History on-demand view
    /// </summary>
    [Serializable]
    public class AgentJobHistoryConfiguration: OnDemandConfiguration
    {
        #region fields

        private bool showFailedOnly = false;
        private List<Guid> jobIdList = new List<Guid>();

        #endregion

        #region constructors

        public AgentJobHistoryConfiguration(int monitoredServerId) : base(monitoredServerId)
        {
        }

        public AgentJobHistoryConfiguration(int monitoredServerId, bool showFailedOnly) : base(monitoredServerId)
        {
            this.showFailedOnly = showFailedOnly;
        }

        #endregion

        #region properties

        public bool ShowFailedOnly
        {
            get { return showFailedOnly; }
            set { showFailedOnly = value; }
        }

        public List<Guid> JobIdList
        {
            get { return jobIdList; }
            set { jobIdList = value; }
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
