//------------------------------------------------------------------------------
// <copyright file="AgentJobRunning.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Idera.SQLdm.Common.Events;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Enter a description for this class
    /// </summary>
    [Serializable]
    public sealed class AgentJobRunning : AgentJobBase
    {
        #region fields

        private string stepName = null;
        private int? retryAttempt = null;
        private TimeSpan runningTime = new TimeSpan(0);
        private DateTime? startedAt = null;
        private double? averageDurationInSeconds = null;
        private Metric associatedThreshold = Metric.LongJobs;
        private int stepId = 0;
        private TimeSpan stepRunningTime = new TimeSpan(0);
        private double? averageStepDurationInSeconds = null;
        //SQLdm 10.1.3 (Vamshi Krishna) SQLDM-19816 - creating a variable for reading the job_last_run_duration value 
        private double? lastRunningJobDurationInSeconds = null;

        #endregion

        #region constructors

        #endregion

        #region properties

        public string StepName
        {
            get { return stepName; }
            internal set { stepName = value; }
        }

        public int? RetryAttempt
        {
            get { return retryAttempt; }
            internal set { retryAttempt = value; }
        }

        public TimeSpan RunningTime
        {
            get { return runningTime; }
            internal set { runningTime = value; }
        }

       

        public double? RunTimePercentOver
        {
            get
            {
                double? ads = AverageDurationInSeconds;
                if (ads > 0)
                {
                    //SQLdm 10.1.3 (Vamshi Krishna) SQLDM-19816 - changed the logic to use the job_last_run_duration value for calculation
                    return (lastRunningJobDurationInSeconds / ads * 100) - 100;
                }
                else
                {
                    return 0;
                }
            }
        }

        public double? StepRunTimePercentOver
        {
            get
            {
                double? ads = AverageStepDurationInSeconds;
                if (ads > 0)
                {
                    return (stepRunningTime.TotalSeconds / ads * 100) - 100;
                }
                else
                {
                    return 0;
                }
            }
        }

        public DateTime? StartedAt
        {
            get { return startedAt; }
            internal set { startedAt = value; }
        }

        public double? AverageDurationInSeconds
        {
            get { return averageDurationInSeconds ?? runningTime.TotalSeconds; }
            internal set { averageDurationInSeconds = value; }
        }


        public Metric AssociatedThreshold
        {
            get { return associatedThreshold; }
            internal set { associatedThreshold = value; }
        }

        public int StepID
        {
            get { return stepId; }
            internal set { stepId = value; }
        }

        public TimeSpan StepRunningTime
        {
            get { return stepRunningTime; }
            internal set { stepRunningTime = value; }
        }

        public double? AverageStepDurationInSeconds
        {
            get { return averageStepDurationInSeconds; }
            set { averageStepDurationInSeconds = value; }
        }

        //SQLdm 10.1.3 (Vamshi Krishna) SQLDM-19816 - reading the job_last_run_duration value 
        public double? LastRunningJobDurationInSeconds
        {
            get { return lastRunningJobDurationInSeconds; }
            set { lastRunningJobDurationInSeconds = value; }
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
