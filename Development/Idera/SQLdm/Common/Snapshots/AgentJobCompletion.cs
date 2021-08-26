using Idera.SQLdm.Common.Events;
using System.Collections.Generic;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Details of SQL Server Agent Jobs that have completed
    /// </summary>
    [Serializable]
    public sealed class AgentJobCompletion : AgentJobBase
    {
        #region fields

        private int stepId = 0;
        private string stepName = null;
        private TimeSpan runDuration = new TimeSpan();
        private JobStepCompletionStatus? runStatus = null;
        private DateTime? startTime = null;
        private int? sqlMessageId = null;
        private int? sqlSeverity = null;
        private string command = null;
        private string message = null;
        private int runs = 0;
        private int successful = 0;
        private int failures = 0;
        private int retries = 0;
        private int canceled = 0;
        private DateTime? collectionSince = null;

        #endregion

        #region constructors

        public AgentJobCompletion()
        {
        }

        #endregion

        #region Properties

        public int StepId
        {
            get { return stepId; }
            internal set { stepId = value; }
        }

        public string StepName
        {
            get { return stepName; }
            internal set { stepName = value; }
        }

        public TimeSpan RunDuration
        {
            get { return runDuration; }
            internal set { runDuration = value; }
        }

        public JobStepCompletionStatus? RunStatus
        {
            get { return runStatus; }
            internal set { runStatus = value; }
        }

        public DateTime? StartTime
        {
            get { return startTime; }
            internal set { startTime = value; }
        }

        public DateTime? EndTime
        {
            get { return StartTime.Value.Add(RunDuration); }
        }

        public int? SQLMessageId
        {
            get { return sqlMessageId; }
            internal set { sqlMessageId = value; }
        }

        public int? SQLServerity
        {
            get { return sqlSeverity; }
            internal set { sqlSeverity = value; }
        }

        public string Command
        {
            get { return command; }
            internal set { command = value; }
        }

        public string Message
        {
            get { return message; }
            internal set { message = value; }
        }

        public int Runs
        {
            get { return runs; }
            internal set { runs = value; }
        }

        public int Successful
        {
            get { return successful; }
            internal set { successful = value; }
        }

        public int Failures
        {
            get { return failures; }
            internal set { failures = value; }
        }

        public int Retries
        {
            get { return retries; }
            internal set { retries = value; }
        }

        public int Canceled
        {
            get { return canceled; }
            internal set { canceled = value; }
        }

        public DateTime? CollectionSince
        {
            get { return collectionSince; }
            internal set { collectionSince = value; }
        }

        #endregion
        /// <summary>
        /// Convert the run status ID to JobStepRunStatus 
        /// </summary>
        internal static JobStepCompletionStatus? ConvertToJobStepCompletionStatus(int? runStatus)
        {
            if (!runStatus.HasValue)
                return null;

            switch (runStatus.Value)
            {
                case 0:
                    return JobStepCompletionStatus.Failed;
                case 1:
                    return JobStepCompletionStatus.Succeeded;
                case 2:
                    return JobStepCompletionStatus.Retry;
                case 3:
                    return JobStepCompletionStatus.Cancelled;
                default:
                    return JobStepCompletionStatus.Unknown;
            }
        }
    }
}
