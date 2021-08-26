//------------------------------------------------------------------------------
// <copyright file="JobsAndStepsConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration object for JobsAndSteps views
    /// </summary>
    [Serializable]
    public sealed class JobsAndStepsConfiguration : OnDemandConfiguration
    {
        #region fields

        private bool isSelectJobMode;
        private string jobName;

        #endregion

        #region constructor

        public JobsAndStepsConfiguration(int monitoredServerId)
            : base(monitoredServerId)
        {

        }

        #endregion

        #region properties

        public bool IsSelectedJobMode
        {
            set { this.isSelectJobMode = value; }
            get { return this.isSelectJobMode; }
        }

        public string JobName
        {
            set { this.jobName = value; }
            get { return this.jobName; }
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
