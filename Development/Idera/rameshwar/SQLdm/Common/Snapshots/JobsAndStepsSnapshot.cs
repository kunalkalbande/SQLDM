//------------------------------------------------------------------------------
// <copyright file="JobsAndStepsSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents snapshot for Jobs And Steps views
    /// </summary>
    [Serializable]
    public sealed class JobsAndStepsSnapshot : Snapshot
    {
        #region fields
        
        private TimeSpan? duration;
        private List<string> InstanceJobsAndSteps = new List<string>();

        #endregion

        #region constructors

        /// <summary>
        /// Constructs JobsAndStepsSnapshot object
        /// </summary>
        /// <param name="info"></param>
        public JobsAndStepsSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
        }

        #endregion

        #region properties

        public List<string> JobsAndSteps
        {
            get { return InstanceJobsAndSteps; }
            internal set { InstanceJobsAndSteps = value; }
        }
        
        public TimeSpan? Duration
        {
            get { return duration; }
            internal set { duration = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion
    }
}
