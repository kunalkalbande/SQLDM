//------------------------------------------------------------------------------
// <copyright file="AgentJob.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    [Serializable]
    public class AgentJobBase
    {
        protected string category = null;
        protected string jobDescription = null;
        protected string jobName = null;
        private Guid jobId = Guid.Empty;

        #region constructors

        internal AgentJobBase()
        {
        }

        internal AgentJobBase(string name)
        {
            this.jobName = name;
        }

        #endregion
        #region properties

        /// <summary>
        /// Gets category for the job
        /// </summary>
        public string Category
        {
            get { return category; }
            internal set { category = value; }
        }

        /// <summary>
        /// Gets description for the job
        /// </summary>
        public string JobDescription
        {
            get { return jobDescription; }
            internal set { jobDescription = value; }
        }

        /// <summary>
        /// Gets name for the job
        /// </summary>
        public string JobName
        {
            get { return jobName; }
            internal set { jobName = value; }
        }

        /// <summary>
        /// Gets Job ID for the job
        /// </summary>
        public Guid JobId
        {
            get { return jobId; }
            internal set { jobId = value; }
        }

        #endregion
    }

    [Serializable]
    public sealed class AgentJobClear : AgentJobBase
    {
        #region fields

        private int stepId = 0;
        private TimeSpan runDuration = new TimeSpan();
        private DateTime? lastRunStartTime = null;

        #endregion

        public TimeSpan RunDuration
        {
            get { return runDuration; }
            set { runDuration = value; }
        }

        public DateTime? LastRunStartTime
        {
            get { return lastRunStartTime; }
            set { lastRunStartTime = value; }
        }

        public int StepID
        {
            get { return stepId; }
            set { stepId = value; }
        }
    }

    /// <summary>
    /// Represents a SQL Agent Job
    /// </summary>
    [Serializable]
    public sealed class AgentJob : AgentJobBase
    {
        #region fields

        private bool? enabled = null;
        private DateTime? nextRunDate = null;
        private string owner = null;
        private int? retryAttempt = null;
        private bool? scheduled = null;
        private string originatingServer = null;
        private JobRunStatus? status = null;
        private int? version = null;
        private TimeSpan runDuration = new TimeSpan();
        private DateTime? lastRunStartTime = null;
        private JobStepRunStatus? lastRunStatus = null;


        #endregion

        #region constructors

        internal AgentJob(string name) : base(name)
        {
        }

        //internal JobDiagnostics(
        //    JobHistoryCollection history,
        //    Guid? jobId,
        //    DateTime? lastRefresh,
        //    string name,
        //    string server
        //    )
        //{
        //    if (server == null || server.Length == 0) throw new ArgumentNullException("server");

        //    this.history = history;
        //    this.name = name;
        //    this.lastRefresh = lastRefresh;
        //    this.jobId = jobId;
        //    this.server = server;
        //}

        //internal JobDiagnostics(
        //    string category,
        //    string description,
        //    bool? enabled,
        //    JobHistoryCollection history,
        //    Guid? jobId,
        //    DateTime? lastRefresh,
        //    string name,
        //    DateTime? nextRunDate,
        //    string owner,
        //    int? retryAttempt,
        //    bool? scheduled,
        //    string server,
        //    JobRunStatus? status,
        //    int? version)
        //{
        //    if (server == null || server.Length == 0) throw new ArgumentNullException("server");

        //    this.category = category;
        //    this.description = description;
        //    this.enabled = enabled;
        //    this.history = history;
        //    this.name = name;
        //    this.lastRefresh = lastRefresh;
        //    this.jobId = jobId;
        //    this.nextRunDate = nextRunDate;
        //    this.owner = owner;
        //    this.retryAttempt = retryAttempt;
        //    this.scheduled = scheduled;
        //    this.server = server;
        //    this.status = status;
        //    this.version = version;
        //}

        ///// <summary>
        ///// Initializes a new instance of the JobDiagnostics class representing a failed
        ///// collection and providing trace information regarding why the failure occurred.
        ///// </summary>
        ///// <param name="server">The sampled SQL Server name.</param>
        ///// <param name="name">The sampled job name.</param>
        ///// <param name="traceInfo">Diagnostic information about the failure.</param>
        //internal JobDiagnostics(
        //    string server,
        //    string name,
        //    string traceInfo)
        //{
        //    if (server == null || server.Length == 0) throw new ArgumentNullException("server");
        //    if (traceInfo == null || traceInfo.Length == 0) throw new ArgumentNullException("traceInfo");

        //    this.name = name;
        //    this.server = server;
        //    SetCollectionFailed(traceInfo);
        //}

        #endregion

        #region properties


        /// <summary>
        /// Gets enabled for the job
        /// </summary>
        public bool? Enabled
        {
            get { return enabled; }
            internal set { enabled = value; }
        }

        /// <summary>
        /// Gets start date of last job run (in UTC)
        /// </summary>
        public DateTime? LastRunStartTime
        {
            get { return lastRunStartTime; }
            internal set { lastRunStartTime = value; }
        }

        /// <summary>
        /// Gets end date of last job run (in UTC)
        /// </summary>
        public DateTime? LastRunEndTime
        {
            get { 
                if (lastRunStartTime.HasValue)
                {
                    return lastRunStartTime.Value.Add(runDuration);
                }
                else
                {
                    return null;
                }
            }
        }


        public JobStepRunStatus? LastRunStatus
        {
            get { return lastRunStatus; }
            internal set { lastRunStatus = value; }
        }

        /// <summary>
        /// Gets nextRunDate for the job (in UTC)
        /// </summary>
        public DateTime? NextRunDate
        {
            get { return nextRunDate; }
            internal set { nextRunDate = value; }
        }

        /// <summary>
        /// Gets owner for the job
        /// </summary>
        public string Owner
        {
            get { return owner; }
            internal set { owner = value; }
        }

        /// <summary>
        /// Gets retryAttempt for the job
        /// </summary>
        public int? RetryAttempt
        {
            get { return retryAttempt; }
            internal set { retryAttempt = value; }
        }


        public TimeSpan RunDuration
        {
            get { return runDuration; }
            internal set { runDuration = value; }
        }

        /// <summary>
        /// Gets server where the job resides
        /// </summary>
        public string OriginatingServer
        {
            get { return originatingServer; }
            internal set { originatingServer = value; }
        }

        /// <summary>
        /// Gets scheduled for the job
        /// </summary>
        public bool? Scheduled
        {
            get { return scheduled; }
            internal set { scheduled = value; }
        }

        /// <summary>
        /// Gets status for the job
        /// </summary>
        public JobRunStatus? Status
        {
            get { return status; }
            internal set { status = value; }
        }

        public bool IsRunning
        {
            get
            {
                if (!Status.HasValue)
                    return false;

                switch (Status.Value)
                {
                    case JobRunStatus.NotRunning:
                    case JobRunStatus.Suspended:
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets version for the job
        /// </summary>
        public int? Version
        {
            get { return version; }
            internal set { version = value; }
        }

        #endregion

        #region methods

        /// <summary>
        /// Convert the run status ID to JobRunStatus? 
        /// </summary>
        internal static JobRunStatus? ConvertToRunStatus(int? runStatus)
        {
            if (!runStatus.HasValue)
                return null;

            switch (runStatus.Value)
            {
                case 1:
                    return JobRunStatus.Executing;
                case 2:
                    return JobRunStatus.WaitingForThread;
                case 3:
                    return JobRunStatus.BetweenRetries;
                case 4:
                    return JobRunStatus.NotRunning;
                case 5:
                    return JobRunStatus.Suspended;
                case 6:
                    return JobRunStatus.WaitingForStepCompletion;
                case 7:
                    return JobRunStatus.CleanUp;
                default:
                    return JobRunStatus.Unknown;
            }
        }

        /// <summary>
        /// Convert the run status ID to JobStepRunStatus 
        /// </summary>
        internal static JobStepRunStatus? ConvertToJobStepRunStatus(int? runStatus)
        {
            if (!runStatus.HasValue)
                return null;

            switch (runStatus.Value)
            {
                case 0:
                    return JobStepRunStatus.Failed;
                case 1:
                    return JobStepRunStatus.Succeeded;
                case 2:
                    return JobStepRunStatus.Retry;
                case 3:
                    return JobStepRunStatus.Cancelled;
                case 4:
                    return JobStepRunStatus.InProgress;
                case 5:
                    return JobStepRunStatus.NotYetRun;
                default:
                    return JobStepRunStatus.Unknown;
            }
        }

        #endregion

        #region nested types

        #endregion
    }
}
