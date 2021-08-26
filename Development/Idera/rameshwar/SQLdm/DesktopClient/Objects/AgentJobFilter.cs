using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.DesktopClient.Helpers;
using JobSummaryFilterType = Idera.SQLdm.Common.Configuration.AgentJobSummaryConfiguration.JobSummaryFilterType;

namespace Idera.SQLdm.DesktopClient.Objects
{
    internal sealed class AgentJobFilter : IUserFilter
    {
        #region constants

        //Summary
        private const JobSummaryFilterType DEFAULT_JOBSUMMARYFILTER = JobSummaryFilterType.All;
        private const JobTimeSpan DEFAULT_TIMESPAN = JobTimeSpan.Any;
        //History
        private const bool DEFAULT_SHOW_FAILED_ONLY = false;

        public enum JobTimeSpan
        {
            [Description("Any Time")]
            Any,
            [Description("Last 30 minutes")]
            HalfHour,
            [Description("Last hour")]
            Hour,
            [Description("Last 12 hours")]
            HalfDay,
            [Description("Last day")]
            Day,
            [Description("Last week")]
            Week,
            [Description("Last month")]
            Month
        }

        #endregion

        #region fields

        //Summary
        private JobSummaryFilterType jobSummaryFilter = JobSummaryFilterType.All;
        private JobTimeSpan filterTimeSpan = DEFAULT_TIMESPAN;
        //History
        private bool showFailedOnly = DEFAULT_SHOW_FAILED_ONLY;

        #endregion

        #region constructors

        public AgentJobFilter()
        {
        }

        #endregion

        #region properties

        /// <summary>
        /// Job Status of the jobs to return
        /// </summary>
        [DisplayName("Job Status"), Category("Summary")]
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
        [DisplayName("Last Run Time"), Category("Summary")]
        [Description("Show only Jobs last run within the specified interval")]
        [DefaultValue(DEFAULT_TIMESPAN)]
        [TypeConverter(typeof(EnumDescriptionConverter))]
        public JobTimeSpan FilterTimeSpanProperty
        {
            get { return filterTimeSpan; }
            set { filterTimeSpan = value; }
        }

        [Browsable(false)]
        public TimeSpan? FilterTimeSpan
        {
            get
            {
                TimeSpan? ts = null;
                switch (filterTimeSpan)
                {
                    case JobTimeSpan.Any:
                        ts = null;
                        break;
                    case JobTimeSpan.HalfHour:
                        ts = new TimeSpan(0, 30, 0);
                        break;
                    case JobTimeSpan.Hour:
                        ts = new TimeSpan(1, 0, 0);
                        break;
                    case JobTimeSpan.HalfDay:
                        ts = new TimeSpan(12, 0, 0);
                        break;
                    case JobTimeSpan.Day:
                        ts = new TimeSpan(24, 0, 0);
                        break;
                    case JobTimeSpan.Week:
                        ts = new TimeSpan(7, 0, 0, 0);
                        break;
                    case JobTimeSpan.Month:
                        ts = new TimeSpan(DateTime.DaysInMonth(DateTime.Now.Month == 1 ? DateTime.Now.Year - 1 : DateTime.Now.Year,
                                                                DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1),
                                            0, 0, 0);
                        break;
                }

                return ts;
            }
        }

        /// <summary>
        /// Show only failed job executions
        /// </summary>
        [DisplayName("Show Failed Only"), Category("History")]
        [Description("Show only Job executions that failed")]
        [DefaultValue(DEFAULT_SHOW_FAILED_ONLY)]
        public bool ShowFailedOnly
        {
            get { return showFailedOnly; }
            set { showFailedOnly = value; }
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
            filterTimeSpan = JobTimeSpan.Any;
            showFailedOnly = false;
        }

        public bool HasDefaultValues()
        {
            if (jobSummaryFilter != DEFAULT_JOBSUMMARYFILTER
                || filterTimeSpan != DEFAULT_TIMESPAN
                || showFailedOnly != DEFAULT_SHOW_FAILED_ONLY
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
                || filterTimeSpan != JobTimeSpan.Any
                || showFailedOnly
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
            filterTimeSpan = DEFAULT_TIMESPAN;
            showFailedOnly = DEFAULT_SHOW_FAILED_ONLY;
        }

        public void UpdateValues(IUserFilter selectionFilter)
        {
            if (selectionFilter is AgentJobFilter)
            {
                AgentJobFilter filter = (AgentJobFilter)selectionFilter;
                jobSummaryFilter = filter.JobSummaryFilter;
                filterTimeSpan = filter.FilterTimeSpanProperty;
                showFailedOnly = filter.ShowFailedOnly;
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

        #endregion
    }
}
