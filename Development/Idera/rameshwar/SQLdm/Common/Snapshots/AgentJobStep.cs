//------------------------------------------------------------------------------
// <copyright file="AgentJobStep.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents a single step from a SQL Server Agent Job
    /// </summary>
    [Serializable]
    public sealed class AgentJobStep
    {
        #region fields

        private int? instanceId = null;
        private string message = null;
        private string name = null;
        private int? retries = null;
        private TimeSpan runDuration = new TimeSpan();
        private JobStepRunStatus? runStatus = null;
        private DateTime? startTime = null;
        private int? stepId = null;
        private int? sqlMessageId = null;
        private int? sqlSeverity = null;

        #endregion

        #region constructors

        internal AgentJobStep()
        {
            
        }

        //int?ernal JobStep(
        //    int? runSeconds,
        //    int? runStatus,
        //    DateTime? startTime
        //    )
        //{
        //    if (runSeconds < 0) throw new ArgumentOutOfRangeException("runSeconds");
        //    _runSeconds = runSeconds;
        //    _runStatus = ConvertToRunStatus(runStatus);
        //    _startTime = startTime;
        //}

        internal AgentJobStep(
            int? instanceId,
            string message,
            string name,
            int? retries,
            TimeSpan runDuration,
            int? runStatus,
            DateTime? startTime,
            int? stepId)
        {
            this.instanceId = instanceId;
            this.message = message;
            this.name = name;
            this.retries = retries;
            this.runDuration = runDuration;
            this.runStatus = AgentJob.ConvertToJobStepRunStatus(runStatus);
            this.startTime = startTime;
            this.stepId = stepId;
        }

        internal AgentJobStep(
            int? instanceId,
            string message,
            string name,
            int? retries,
            TimeSpan runDuration,
            int? runStatus,
            DateTime? startTime,
            int? stepId,
            int? sqlMessageId,
            int? sqlSeverity)
        {
            this.instanceId = instanceId;
            this.message = message;
            this.name = name;
            this.retries = retries;
            this.runDuration = runDuration;
            this.runStatus = AgentJob.ConvertToJobStepRunStatus(runStatus);
            this.startTime = startTime;
            this.stepId = stepId;
            this.sqlMessageId = sqlMessageId;
            this.sqlSeverity = sqlSeverity;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets end time
        /// </summary>
        public DateTime? EndTime
        {
            get { return StartTime.Value.Add(RunDuration); }
        }

        /// <summary>
        /// Gets instance ID
        /// </summary>
        public int? InstanceId
        {
            get { return instanceId; }
        }

        /// <summary>
        /// Gets step message
        /// </summary>
        public string Message
        {
            get { return message; }
        }

        /// <summary>
        /// Gets step name
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets step retries attempted
        /// </summary>
        public int? Retries
        {
            get { return retries; }
        }

        /// <summary>
        /// Gets how long the step took to run
        /// </summary>
        public TimeSpan RunDuration
        {
            get { return runDuration; }
        }

        public JobStepRunStatus? RunStatus
        {
            get { return runStatus; }
        }

        /// <summary>
        /// Gets timestamp when the step started
        /// </summary>
        public DateTime? StartTime
        {
            get { return startTime; }
        }

        /// <summary>
        /// Gets SQL Server Step ID
        /// </summary>
        public int? StepId
        {
            get { return stepId; }
        }

        /// <summary>
        /// Gets Job step execution SQL Message ID
        /// </summary>
        public int? SQLMessageId
        {
            get { return sqlMessageId; }
        }

        /// <summary>
        /// Gets Job Step execution sql severity
        /// </summary>
        public int? SQLSeverity
        {
            get { return sqlSeverity; }
        }

        #endregion

        #region methods


        #endregion

        #region nested types

        #endregion

    }
}
