//------------------------------------------------------------------------------
// <copyright file="AgentJobFailure.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents a failed SQL agent job
    /// </summary>
    [Serializable]
    public sealed class AgentJobFailure : AgentJobBase
    {
        #region fields

        private string stepName = null;
        private int? sqlMessageId = null;
        private int? sqlSeverity = null;
        private DateTime? runTime = null;
        private string command = null;
        private string errorMessage = null;
        private int? executions = null;
        private int? failedRuns = null;
        private DateTime? collectionsSince = null;
        private int stepId = 0;

        #endregion

        #region constructors

        #endregion

        #region properties

        /// <summary>
        /// Step Id of the failing step
        /// </summary>
        public int StepId
        {
            get { return stepId; }
            internal set { stepId = value; }
        }

        /// <summary>
        /// Name of failing step
        /// </summary>
        public string StepName
        {
            get { return stepName; }
            internal set { stepName = value; }
        }

        /// <summary>
        /// SQL message ID
        /// </summary>
        public int? SqlMessageId
        {
            get { return sqlMessageId; }
            internal set { sqlMessageId = value; }
        }

        /// <summary>
        /// Severity of error
        /// </summary>
        public int? SqlSeverity
        {
            get { return sqlSeverity; }
            internal set { sqlSeverity = value; }
        }

        /// <summary>
        /// Time that the failing job started
        /// </summary>
        public DateTime? RunTime
        {
            get { return runTime; }
            internal set { runTime = value; }
        }

        /// <summary>
        /// SQL Command executed
        /// </summary>
        public string Command
        {
            get { return command; }
            internal set { command = value; }
        }

        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage
        {
            get { return errorMessage; }
            internal set { errorMessage = value; }
        }

        /// <summary>
        /// Number of executions (successful or not) in the collection window
        /// </summary>
        public int? Executions
        {
            get { return executions; }
            set { executions = value; }
        }

        /// <summary>
        /// Number of failures in the collection window
        /// </summary>
        public int? FailedRuns
        {
            get { return failedRuns; }
            set { failedRuns = value; }
        }

        /// <summary>
        /// Number of successes in the collection window
        /// </summary>
        public int? SuccessfulRuns
        {
            get { return executions - failedRuns; }
        }


        /// <summary>
        /// Represents the date/time since when the Failure/Execution count has been calculated
        /// </summary>
        public DateTime? CollectionsSince
        {
            get { return collectionsSince; }
            set { collectionsSince = value; }
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
