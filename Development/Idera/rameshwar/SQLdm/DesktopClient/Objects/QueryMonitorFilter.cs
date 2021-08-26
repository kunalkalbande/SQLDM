using System;
using System.ComponentModel;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Configuration;
using QueryViewMode = Idera.SQLdm.DesktopClient.Views.Servers.Server.Queries.QueryMonitorView.QueryMonitorViewMode;

namespace Idera.SQLdm.DesktopClient.Objects
{
    [DefaultProperty("ApplicationNameExcludeFilter")]
    internal sealed class QueryMonitorFilter : IUserFilter
    {
        //private const QueryMonitorSummaryInterval DefaultInterval = QueryMonitorSummaryInterval.Hours;
        private const bool DefaultIncludeSqlStatements = true;
        private const bool DefaultIncludeStoredProcedures = true;
        private const bool DefaultIncludeSqlBatches = true;
        private const bool DefaultIncludeOnlyResourceRows = true; //SQLdm 8.5 (Ankit Srivastava): for Query Monitor View Filter Options
        private const long DefaultDurationFilterConst = 1000;
        private const bool DefaultCaseInsensitiveFilterConst = false;

        private int instanceId;
        private DateTime startDate;
        private DateTime endDate;
        private TimeSpan startTime = new TimeSpan(0, 0, 0);
        private TimeSpan endTime = new TimeSpan(23, 59, 59);
        //private QueryMonitorSummaryInterval interval = DefaultInterval;
        private bool includeSqlStatements = DefaultIncludeSqlStatements;
        private bool includeStoredProcedures = DefaultIncludeStoredProcedures;
        private bool includeSqlBatches = DefaultIncludeSqlBatches;
        private bool includeOnlyResourceRows = DefaultIncludeOnlyResourceRows; //SQLdm 8.5 (Ankit Srivastava): for Query Monitor View Filter Options
        private string applicationNameExcludeFilter = null;
        private string applicationNameIncludeFilter = null;
        private string hostNameExcludeFilter = null;
        private string hostNameIncludeFilter = null;
        private string databaseNameExcludeFilter = null;
        private string databaseNameIncludeFilter = null;
        private string userNameExcludeFilter = null;
        private string userNameIncludeFilter = null;
        private string sqlTextExcludeFilter = null;
        private string sqlTextIncludeFilter = null;
        private long durationFilter = DefaultDurationFilterConst;
        private long defaultDurationFilter = DefaultDurationFilterConst;
        private int utcOffset = 0;
        private bool caseInsensitive = DefaultCaseInsensitiveFilterConst;

        public QueryMonitorFilter(int instanceId)
        {
            this.instanceId = instanceId;
            startDate = DateTime.Now.Date;
            endDate = startDate;
            utcOffset = (int)TimeZone.CurrentTimeZone.GetUtcOffset(startDate).TotalMinutes;
        }

        [Browsable(false)]
        public int InstanceId
        {
            get { return instanceId; }
            set { instanceId = value; }
        }

        [DisplayName("Date Range - Begin"), Category("Period")]
        [Description("Enter the start date for the filter.")]
        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        [DisplayName("Date Range - End"), Category("Period")]
        [Description("Enter the end date for the filter.")]
        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }

        [DisplayName("Time Period - Begin"), Category("Period")]
        [Description("Enter the start time for the period within each day that should be included.")]
        public TimeSpan StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        [DisplayName("Time Period - End"), Category("Period")]
        [Description("Enter the end time for the period within each day that should be included.")]
        public TimeSpan EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        //[DisplayName("Interval"), Category("Summary Chart"), DefaultValue(DefaultInterval)]
        //[Description("Specify the grouping interval for the event summary chart.")]
        //public QueryMonitorSummaryInterval Interval
        //{
        //    get { return interval; }
        //    set { interval = value; }
        //}

        [DisplayName("Show SQL Statements"), Category("Events"), DefaultValue(DefaultIncludeSqlStatements)]
        [Description("Specify whether to include individual SQL statement executions in the filter.")]
        public bool IncludeSqlStatements
        {
            get { return includeSqlStatements; }
            set { includeSqlStatements = value; }
        }

        [DisplayName("Show Stored Procedures"), Category("Events"), DefaultValue(DefaultIncludeStoredProcedures)]
        [Description("Specify whether to include stored procedure executions in the filter.")]
        public bool IncludeStoredProcedures
        {
            get { return includeStoredProcedures; }
            set { includeStoredProcedures = value; }
        }

        [DisplayName("Show SQL Batches"), Category("Events"), DefaultValue(DefaultIncludeSqlBatches)]
        [Description("Specify whether to include SQL batch executions in the filter.")]
        public bool IncludeSqlBatches
        {
            get { return includeSqlBatches; }
            set { includeSqlBatches = value; }
            
        }

        //SQLdm 8.5 (Ankit Srivastava): for Query Monitor View Filter Options -- Added new property
        [DisplayName("Exclude Currently Running Queries"), Category("Events"), DefaultValue(DefaultIncludeOnlyResourceRows)]
        [Description("Specify whether to include the currently running queries in the filter.")]
        public bool IncludeOnlyResourceRows
        {
            get { return includeOnlyResourceRows; }
            set { includeOnlyResourceRows = value; }
        }

        [DisplayName("Exclude Application"), Category("Exclude Filters")]
        [Description(
            "Specify the application name to exclude in the filter. This filter option can be specified specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard."
            )]
        public string ApplicationNameExcludeFilter
        {
            get
            {
                if (applicationNameExcludeFilter == null || applicationNameExcludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return applicationNameExcludeFilter;
                }
            }
            set { applicationNameExcludeFilter = value; }
        }

        [DisplayName("Include Application"), Category("Include Filters")]
        [Description(
            "Specify the application name to include in the filter. This filter option can be specified specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard."
            )]
        public string ApplicationNameIncludeFilter
        {
            get
            {
                if (applicationNameIncludeFilter == null || applicationNameIncludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return applicationNameIncludeFilter;
                }
            }
            set { applicationNameIncludeFilter = value; }
        }

        [DisplayName("Exclude Client Computer"), Category("Exclude Filters")]
        [Description(
            "Specify the client computer name to exclude in the filter. This filter option can be specified specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard."
            )]
        public string HostNameExcludeFilter
        {
            get
            {
                if (hostNameExcludeFilter == null || hostNameExcludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return hostNameExcludeFilter;
                }
            }
            set { hostNameExcludeFilter = value; }
        }

        [DisplayName("Include Client Computer"), Category("Include Filters")]
        [Description(
            "Specify the client computer name to include in the filter. This filter option can be specified specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard."
            )]
        public string HostNameIncludeFilter
        {
            get
            {
                if (hostNameIncludeFilter == null || hostNameIncludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return hostNameIncludeFilter;
                }
            }
            set { hostNameIncludeFilter = value; }
        }

        [DisplayName("Exclude Database"), Category("Exclude Filters")]
        [Description(
            "Specify the database name to exclude in the filter. This filter option can be specified specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard."
            )]
        public string DatabaseNameExcludeFilter
        {
            get
            {
                if (databaseNameExcludeFilter == null || databaseNameExcludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return databaseNameExcludeFilter;
                }
            }
            set { databaseNameExcludeFilter = value; }
        }

        [DisplayName("Include Database"), Category("Include Filters")]
        [Description(
            "Specify the database name to include in the filter. This filter option can be specified specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard."
            )]
        public string DatabaseNameIncludeFilter
        {
            get
            {
                if (databaseNameIncludeFilter == null || databaseNameIncludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return databaseNameIncludeFilter;
                }
            }
            set { databaseNameIncludeFilter = value; }
        }

        [DisplayName("Exclude User"), Category("Exclude Filters")]
        [Description(
            "Specify the SQL user name to exclude in the filter. This filter can be specified specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard."
            )]
        public string UserNameExcludeFilter
        {
            get
            {
                if (userNameExcludeFilter == null || userNameExcludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return userNameExcludeFilter;
                }
            }
            set { userNameExcludeFilter = value; }
        }

        [DisplayName("Include User"), Category("Include Filters")]
        [Description(
            "Specify the SQL user name to include in the filter. This filter can be specified specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard."
            )]
        public string UserNameIncludeFilter
        {
            get
            {
                if (userNameIncludeFilter == null || userNameIncludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return userNameIncludeFilter;
                }
            }
            set { userNameIncludeFilter = value; }
        }

        [DisplayName("Exclude SQL Text"), Category("Exclude Filters")]
        [Description(
            "Specify the SQL text to exclude in the filter. This filter can be specified specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard."
            )]
        public string SqlTextExcludeFilter
        {
            get
            {
                if (sqlTextExcludeFilter == null || sqlTextExcludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return sqlTextExcludeFilter;
                }
            }
            set { sqlTextExcludeFilter = value; }
        }

        [DisplayName("Include SQL Text"), Category("Include Filters")]
        [Description(
            "Specify the SQL text to include in the filter. This filter can be specified specifically or in the form of a LIKE clause in T-SQL, where '%' is used as a wildcard."
            )]
        public string SqlTextIncludeFilter
        {
            get
            {
                if (sqlTextIncludeFilter == null || sqlTextIncludeFilter.Trim().Length == 0)
                {
                    return null;
                }
                else
                {
                    return sqlTextIncludeFilter;
                }
            }
            set { sqlTextIncludeFilter = value; }
        }

        [DisplayName("Minimum Duration (ms)"), Category("Execution"), DefaultValue(DefaultDurationFilterConst)]
        [Description("Specify the minimum duration threshold (in milliseconds) to include in the filter.")]
        [Browsable(false)]
        [Auditable("Changed Duration threshold to (milliseconds)")]
        public long DurationFilter
        {
            get { return durationFilter; }
            set { durationFilter = value; }
        }

        [Browsable(false)]
        public long DefaultDurationFilter
        {
            get { return defaultDurationFilter; }
        }

        [Browsable(false)]
        public int UtcOffset
        {
            get { return utcOffset; }
            set { utcOffset = value; }
        }

        public void SetDefaultDurationFilter(long duration)
        {
            defaultDurationFilter = duration;
        }

        [DisplayName("Case Insensitive Match"), Category("Misc"), DefaultValue(DefaultCaseInsensitiveFilterConst)]
        [Description("Specify whether to perform a case-insensitive match when grouping like statements.")]
        [Browsable(false)]  // formerly shown and now hidden
        public bool CaseInsensitive
        {
            get { return caseInsensitive; }
            set { caseInsensitive = value; }
        }

        #region IUserFilter Members

        public void ClearValues()
        {
            //interval = DefaultInterval;
            includeSqlStatements = true;
            includeStoredProcedures = true;
            includeSqlBatches = true;
            includeOnlyResourceRows = true; //SQLdm 8.5 (Ankit Srivastava): for Query Monitor View Filter Options
            applicationNameExcludeFilter = null;
            applicationNameIncludeFilter = null;
            hostNameExcludeFilter = null;
            hostNameIncludeFilter = null;
            databaseNameExcludeFilter = null;
            databaseNameIncludeFilter = null;
            userNameExcludeFilter = null;
            userNameIncludeFilter = null;
            sqlTextExcludeFilter = null;
            sqlTextIncludeFilter = null;
            durationFilter = long.MaxValue;
            //utcOffset = 0;
        }

        public bool HasDefaultValues()
        {
            return //interval == DefaultInterval &&
                   includeSqlStatements == DefaultIncludeSqlStatements &&
                   includeStoredProcedures == DefaultIncludeStoredProcedures &&
                   includeSqlBatches == DefaultIncludeSqlBatches &&
                   includeOnlyResourceRows == DefaultIncludeOnlyResourceRows && //SQLdm 8.5 (Ankit Srivastava): for Query Monitor View Filter Options
                   applicationNameExcludeFilter == null &&
                   applicationNameIncludeFilter == null &&
                   hostNameExcludeFilter == null &&
                   hostNameIncludeFilter == null &&
                   databaseNameExcludeFilter == null &&
                   databaseNameIncludeFilter == null &&
                   userNameExcludeFilter == null &&
                   userNameIncludeFilter == null &&
                   sqlTextExcludeFilter == null &&
                   sqlTextIncludeFilter == null &&
                   durationFilter == defaultDurationFilter &&
                   caseInsensitive == DefaultCaseInsensitiveFilterConst;

        }

        public bool IsFiltered()
        {
            return !(includeSqlStatements &&
                   includeStoredProcedures &&
                   includeSqlBatches &&
                   includeOnlyResourceRows && //SQLdm 8.5 (Ankit Srivastava): for Query Monitor View Filter Options
                   applicationNameExcludeFilter == null &&
                   applicationNameIncludeFilter == null &&
                   hostNameExcludeFilter == null &&
                   hostNameIncludeFilter == null &&
                   databaseNameExcludeFilter == null &&
                   databaseNameIncludeFilter == null &&
                   userNameExcludeFilter == null &&
                   userNameIncludeFilter == null &&
                   sqlTextExcludeFilter == null &&
                   sqlTextIncludeFilter == null &&
                   durationFilter == 0);
        }

        public void ResetValues()
        {
            ResetTimeRangeValues();
            //interval = DefaultInterval;
            includeSqlStatements = DefaultIncludeSqlStatements;
            includeStoredProcedures = DefaultIncludeStoredProcedures;
            includeSqlBatches = DefaultIncludeSqlBatches;
            includeOnlyResourceRows = DefaultIncludeOnlyResourceRows; //SQLdm 8.5 (Ankit Srivastava): for Query Monitor View Filter Options
            applicationNameExcludeFilter = null;
            applicationNameIncludeFilter = null;
            hostNameExcludeFilter = null;
            hostNameIncludeFilter = null;
            databaseNameExcludeFilter = null;
            databaseNameIncludeFilter = null;
            userNameExcludeFilter = null;
            userNameIncludeFilter = null;
            sqlTextExcludeFilter = null;
            sqlTextIncludeFilter = null;
            durationFilter = defaultDurationFilter;
            //utcOffset = 0;
            caseInsensitive = DefaultCaseInsensitiveFilterConst;
        }

        public void ResetTimeRangeValues()
        {
            startDate = DateTime.Now.Date;
            endDate = startDate;
            startTime = new TimeSpan(0, 0, 0);
            endTime = new TimeSpan(23, 59, 59);
        }

        public void UpdateValues(IUserFilter selectionFilter)
        {
            if (selectionFilter is QueryMonitorFilter)
            {
                QueryMonitorFilter filter = (QueryMonitorFilter)selectionFilter;
                startDate = filter.StartDate;
                endDate = filter.EndDate;
                startTime = filter.StartTime;
                endTime = filter.EndTime;
                //interval = filter.Interval;
                includeSqlStatements = filter.IncludeSqlStatements;
                includeStoredProcedures = filter.IncludeStoredProcedures;
                includeSqlBatches = filter.IncludeSqlBatches;
                includeOnlyResourceRows = filter.IncludeOnlyResourceRows; //SQLdm 8.5 (Ankit Srivastava): for Query Monitor View Filter Options
                applicationNameExcludeFilter = filter.ApplicationNameExcludeFilter;
                applicationNameIncludeFilter = filter.ApplicationNameIncludeFilter;
                hostNameExcludeFilter = filter.HostNameExcludeFilter;
                hostNameIncludeFilter = filter.HostNameIncludeFilter;
                databaseNameExcludeFilter = filter.DatabaseNameExcludeFilter;
                databaseNameIncludeFilter = filter.DatabaseNameIncludeFilter;
                userNameExcludeFilter = filter.UserNameExcludeFilter;
                userNameIncludeFilter = filter.UserNameIncludeFilter;
                sqlTextExcludeFilter = filter.SqlTextExcludeFilter;
                sqlTextIncludeFilter = filter.SqlTextIncludeFilter;
                durationFilter = filter.DurationFilter;
                defaultDurationFilter = filter.DefaultDurationFilter;
                caseInsensitive = filter.CaseInsensitive;
            }
        }

        public bool Validate(out string Message)
        {
            Message = null;
            return true;
        }

        #endregion
    }

    //internal enum QueryMonitorSummaryInterval
    //{
    //    Hours = 1,
    //    Days,
    //    Months,
    //    Years
    //}
}