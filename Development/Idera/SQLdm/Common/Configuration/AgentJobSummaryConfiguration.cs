//------------------------------------------------------------------------------
// <copyright file="JobSummaryConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.ComponentModel;

namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration object for Job Summary on-demand probe
    /// </summary>
    [Serializable]
    public class AgentJobSummaryConfiguration : OnDemandConfiguration, IUserFilter
    {
        #region constants

        private const JobSummaryFilterType DEFAULT_JOBSUMMARYFILTER = JobSummaryFilterType.All;
        private const object DEFAULT_TIMESPAN = null;              // change validation if default is not null

        #endregion

        #region fields

        private JobSummaryFilterType jobSummaryFilter = JobSummaryFilterType.All;
        private TimeSpan? filterTimeSpan = (TimeSpan?)DEFAULT_TIMESPAN;

        #endregion

        #region constructors

        public AgentJobSummaryConfiguration(int monitoredServerId) : base(monitoredServerId)
        {
        }


        public AgentJobSummaryConfiguration(int monitoredServerId, JobSummaryFilterType jobSummaryFilter, TimeSpan? filterTimeSpan) : base(monitoredServerId)
        {
            this.jobSummaryFilter = jobSummaryFilter;
            this.filterTimeSpan = filterTimeSpan;
        }

        #endregion

        #region properties

        /// <summary>
        /// Job Status of the jobs to return
        /// </summary>
        [DisplayName("Job Status"), Category("General")]
        [Description("Show only Jobs with the selected status")]
        [DefaultValue(DEFAULT_JOBSUMMARYFILTER)]
        public JobSummaryFilterType JobSummaryFilter
        {
            get { return jobSummaryFilter; }
            set { jobSummaryFilter = value; }
        }

        /// <summary>
        /// Time span of the jobs to return. Leave null to return all
        /// </summary>
        [DisplayName("Time Span"), Category("General")]
        [Description("Show only Jobs within the specified interval")]
        [DefaultValue(DEFAULT_TIMESPAN)]
        public TimeSpan? FilterTimeSpan
        {
            get { return filterTimeSpan; }
            set { filterTimeSpan = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #region IUserFilter Members

        public void ClearValues()
        {
            jobSummaryFilter = JobSummaryFilterType.All;
            filterTimeSpan = null;
        }

        public bool HasDefaultValues()
        {
            if (jobSummaryFilter != DEFAULT_JOBSUMMARYFILTER
                    || filterTimeSpan.HasValue   // handle this way because the default is null
                )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsFiltered()
        {
            if (jobSummaryFilter != JobSummaryFilterType.All
                    || filterTimeSpan.HasValue
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ResetValues()
        {
            jobSummaryFilter = DEFAULT_JOBSUMMARYFILTER;
            filterTimeSpan = (TimeSpan?)DEFAULT_TIMESPAN;
        }

        public void UpdateValues(IUserFilter selectionFilter)
        {
            if (selectionFilter is AgentJobSummaryConfiguration)
            {
                AgentJobSummaryConfiguration filter = (AgentJobSummaryConfiguration)selectionFilter;
                jobSummaryFilter = filter.JobSummaryFilter;
                filterTimeSpan = filter.FilterTimeSpan;
            }
        }

        public bool Validate(out string Message)
        {
            Message = String.Empty;
            return true;
        }

        #endregion

        #endregion

        #region nested types

        public enum JobSummaryFilterType
        {
            All,
            Failed,
            Running
        }

        #endregion
    }
}
