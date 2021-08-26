//------------------------------------------------------------------------------
// <copyright file="GroomingConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Idera.SQLdm.Common.Attributes;

namespace Idera.SQLdm.Common.Configuration
{
    [Serializable]
    public class GroomingConfiguration
    {
        private int _alertsDays = 31;
        private int _auditDays = 90;
        private int _prescriptiveAnalysisDays = 90;
        private int _metricsDays = 365;
        private int _tasksDays = 7;
        private int _activityDays = 31;
        private SubDayType _subDayType;
        private TimeSpan _groomTime = TimeSpan.FromHours(3.0); // 3:00 AM
        private bool _updateScheduleAllowed;
        private bool _agentIsRunning;
        private bool _jobIsRunning;
        private DateTime _lastRun = DateTime.MinValue;
        private DateTime _repositoryTime = DateTime.MinValue;
        private string _lastOutcome = "Unknown";
        private int _queriesDays = 14;
        private TimeSpan _aggregationTime = TimeSpan.FromHours(2.0); // 2:00 AM;
        private SubDayType _aggregationSubDayType;
        private bool _aggregationAllowScheduleChange;
        private bool _aggregationJobIsRunning;
        private DateTime _aggregationLastRun = DateTime.MinValue;
        private string _aggregationLastOutcome = "Unknown";
        private int _aggregateForecastingDays = 1095;    // Setting Default Value to 1095 days
        private int _groomForecastingDays = 3;

        /// <summary>
        /// Determines if the schedule is once per day or on an hourly frequency.
        /// </summary>
        [AuditableAttribute("Scheduled Grooming Type")]
        public SubDayType ScheduleSubDayType
        {
            get { return _subDayType; }
            set { _subDayType = value; }
        }

        /// <summary>
        /// Time of day when gooming occurs in repository's time zone.
        /// </summary>
        [AuditableAttribute("Scheduled Grooming Time")]
        public TimeSpan GroomTime
        {
            get { return _groomTime; }
            set { _groomTime = value; }
        }

        /// <summary>
        /// Determines if the schedule is once per day or on an hourly frequency.
        /// </summary>
        [AuditableAttribute("Scheduled Aggregation Type")]
        public SubDayType AggregationSubDayType
        {
            get { return _aggregationSubDayType; }
            set { _aggregationSubDayType = value; }
        }
        
        /// <summary>
        /// Time of day when query aggregation occurs in repository's time zone.
        /// </summary>
        [AuditableAttribute("Scheduled Aggregation Time")]
        public TimeSpan AggregationTime
        {
            get { return _aggregationTime; }
            set { _aggregationTime = value; }
        }


        /// <summary>
        /// Age in days when metrics are subject to grooming.
        /// </summary>
        [AuditableAttribute("Grooming of metrics and baselines scheduled to start after [Days]")]
        public int MetricsDays
        {
            get { return _metricsDays; }
            set { _metricsDays = value; }
        }

        /// <summary>
        /// Age in days when alerts are subject to grooming.
        /// </summary>
        [AuditableAttribute("Grooming of sessions, queries, deadlocks, waits and history browser scheduled to start after [Days]")]
        public int ActivityDays
        {
            get { return _activityDays; }
            set { _activityDays = value; }
        }

        /// <summary>
        /// UpdateScheduleAllowed returns false if someone has manually
        /// changed the grooming job schedule to be incompatible with the
        /// schedule options offered in the UI.
        /// </summary>
        public bool UpdateScheduleAllowed
        {
            get { return _updateScheduleAllowed;  }
            set { _updateScheduleAllowed = value; }
        }

        /// <summary>
        /// UpdateScheduleAllowed returns false if someone has manually
        /// changed the aggregation job schedule to be incompatible with the
        /// schedule options offered in the UI.
        /// </summary>
        public bool UpdateAggregationScheduleAllowed
        {
            get { return _aggregationAllowScheduleChange; }
            set { _aggregationAllowScheduleChange = value; }
        }

        /// <summary>
        /// Age in days when alerts are subject to grooming.
        /// </summary>
        [AuditableAttribute("Grooming of inactive alerts scheduled to start after [Days]")]
        public int AlertsDays
        {
            get { return _alertsDays; }
            set { _alertsDays = value; }
        }

        /// <summary>
        /// Age in days when queries are subject to aggregation.
        /// </summary>
        [AuditableAttribute("Aggregation of query data scheduled to start after [Days]")]
        public int QueriesDays
        {
            get { return _queriesDays; }
            set { _queriesDays = value; }
        }

        /// <summary>
        /// Age in days when Audit information are subject to grooming.
        /// </summary>
        [AuditableAttribute("Grooming of change log data scheduled to start after [Days]")]
        public int AuditDays
        {
            get { return _auditDays; }
            set { _auditDays = value; }
        }

        //10.0 SQLdm srishti purohit
        //Prescriptive analysis old data grooming implementation
        [AuditableAttribute("Grooming of Prescriptive analysis old data scheduled to start after [Days]")]
        public int PADataDays
        {
            get { return _prescriptiveAnalysisDays; }
            set { _prescriptiveAnalysisDays = value; }
        }

        //Forecasting analysis old data aggregation implementation
        [AuditableAttribute("Aggregate forecasting data older than [Days]")]
        public int FADataDays
        {
            get { return _aggregateForecastingDays; }
            set { _aggregateForecastingDays = value; }
        }

        //Grooming for forecast category
        [AuditableAttribute("Groom forecasting data older than [Days]")]
        public int GroomForecastingDays
        {
            get { return _groomForecastingDays; }
            set { _groomForecastingDays = value; }
        }


        /// <summary>
        /// Age in days when tasks are subject to grooming.
        /// </summary>
        public int TasksDays
        {
            get { return _tasksDays; }
            set { _tasksDays = value; }
        }
        
        /// <summary>
        /// True if the SQL Server Agent is running.  Output only.
        /// </summary>
        public bool AgentIsRunning
        {
            get { return _agentIsRunning; }
            set { _agentIsRunning = value; }
        }

        /// <summary>
        /// True if the groom job is is running.  Output only.
        /// </summary>
        public bool JobIsRunning
        {
            get { return _jobIsRunning; }
            set { _jobIsRunning = value; }
        }

        /// <summary>
        /// True if the aggregation job is is running.  Output only.
        /// </summary>
        public bool AggregationJobIsRunning
        {
            get { return _aggregationJobIsRunning; }
            set { _aggregationJobIsRunning = value;}
        }

        public DateTime LastRun
        {
            get { return _lastRun; }
            set { _lastRun = value; }
        }

        public string LastOutcome
        {
            get { return _lastOutcome; }
            set { _lastOutcome = value; }
        }

        [AuditableAttribute(false)]
        public DateTime RepositoryTime
        {
            get { return _repositoryTime; }
            set { _repositoryTime = value; }
        }

        public DateTime AggregationLastRun
        {
            get { return _aggregationLastRun; }
            set { _aggregationLastRun = value;}
        }

        public string AggregationLastOutcome
        {
            get { return _aggregationLastOutcome; }
            set { _aggregationLastOutcome = value; }
        }


        public override string ToString()
        {
            return string.Format("ActivityDays = {0}, AlertsDays = {1}, MetricsDays = {2}, TasksDays = {3}, GroomTime = {4}, AgentIsRunning = {5}, JobIsRunning = {6}, LastRun = {7}, LastOutcome = {8}, RepositoryTime = {9}.", ActivityDays, AlertsDays, MetricsDays, TasksDays, GroomTime, AgentIsRunning, JobIsRunning, LastRun, LastOutcome, RepositoryTime);
        }

        [Serializable]
        public enum SubDayType
        {
            Once = 1,
            Minutes = 4,
            Hours = 8
        }
    }
}
