//------------------------------------------------------------------------------
// <copyright file="ErrorLogConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.ComponentModel;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration object for error log details on-demand probe
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(PropertySorter))]
    public sealed class ErrorLogConfiguration : OnDemandConfiguration, IUserFilter
    {
        #region constants

        private const object DEFAULT_START_DATE = null;
        private const object DEFAULT_END_DATE = null;
        private const string DEFAULT_SOURCE = null;
        private const bool DEFAULT_ERROR = true;
        private const bool DEFAULT_WARNING = true;
        private const bool DEFAULT_INFORMATIONAL = true;

        //These are the names that need to be used in messages
        private const string DISPLAYNAME_START_DATE = "Date Range - Begin";
        private const string DISPLAYNAME_END_DATE = "Date Range - End";
        private const string DISPLAYNAME_SEVERITY = "Message Severity";

        private const string MESSAGE_DATES_OVERLAP = "'{0}' is greater than and '{1}' and will return no log records. Please select different dates and try again.";
        private const string MESSAGE_NO_SEVERITY = " All {0} have been excluded and no log records will be returned. Please select at least one {1} and try again.";

        #endregion

        #region fields

        private List<LogFile> logFiles = new List<LogFile>();
        private DateTime? startDate = null;
        private DateTime? endDate = null;
        private string source = DEFAULT_SOURCE;
        private bool showErrors = DEFAULT_ERROR;
        private bool showWarnings = DEFAULT_WARNING;
        private bool showInformational = DEFAULT_INFORMATIONAL;
        private DateTime? internalStartDate = null;
        private DateTime? internalEndDate = null;

        #endregion

        #region constructors

        public ErrorLogConfiguration(int monitoredServerId) : base(monitoredServerId)
        {
        }

        public ErrorLogConfiguration(int monitoredServerId, List<LogFile> logFiles)
            : base(monitoredServerId)
        {
            this.logFiles = logFiles;
        }

        public ErrorLogConfiguration(int monitoredServerId, List<LogFile> logFiles, DateTime? startDate, DateTime endDate)
            : this(monitoredServerId, logFiles)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }

        #endregion

        #region properties

        new public bool ReadyForCollection
        {
            get { return (LogFiles != null && (LogFiles.Count > 0)); }
        }


        /// <summary>
        /// Represents the list of log files to be used in pulling data
        /// </summary>
        [Browsable(false)]
        public List<LogFile> LogFiles
        {
            get { return logFiles; }
            set { logFiles = value; }
        }

        /// <summary>
        /// Display only log records from the selected date forward 
        /// </summary>
        [Browsable(false)]
        public DateTime? StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        [DisplayName(DISPLAYNAME_START_DATE), Category("Period"), DisplayOrder(1)]
        [Description("Show only log records from the selected date forward")]
        [DefaultValue(DEFAULT_START_DATE)]
        public DateTime StartDateProperty
        {
            get { return StartDate.HasValue ? StartDate.Value : DateTime.MinValue; }
            set { StartDate = value.Equals(DateTime.MinValue) ? null : (DateTime?)value; }
        }

        /// <summary>
        /// Display only log records through the selected date 
        /// </summary>
        [Browsable(false)]
        public DateTime? EndDate
        {
            get { return endDate; }
            set
            {
                endDate = value;
                if (endDate.HasValue)
                {
                    // if there is only a date, then adjust to include the entire day
                    if (endDate.Value.Ticks == endDate.Value.Date.Ticks)
                    {
                        endDate = endDate.Value.AddDays(1).AddSeconds(-1);
                    }
                }
            }
        }

        [DisplayName(DISPLAYNAME_END_DATE), Category("Period"), DisplayOrder(2)]
        [Description("Show only log records through the selected date")]
        [DefaultValue(DEFAULT_END_DATE)]
        public DateTime EndDateProperty
        {
            get { return EndDate.HasValue ? EndDate.Value : DateTime.MinValue; }
            set
            {
                EndDate = value.Equals(DateTime.MinValue) ? null : (DateTime?)value;
            }
        }

        /// <summary>
        /// Display only log records from the selected source 
        /// </summary>
        [DisplayName("Source"), Category("General")]
        [Description("Show only log records from the selected source")]
        [DefaultValue(DEFAULT_SOURCE)]
        public string Source
        {
            get { return source; }
            set { source = value != null && value.Trim().Length > 0 ? value : null; }
        }

        /// <summary>
        /// Include log records that are type Error
        /// </summary>
        [DisplayName("Show Errors"), Category(DISPLAYNAME_SEVERITY), DisplayOrder(1)]
        [Description("Include log records that are flagged as Errors")]
        [DefaultValue(DEFAULT_ERROR)]
        public bool ShowErrors
        {
            get { return showErrors; }
            set { showErrors = value; }
        }

        /// <summary>
        /// Include log records that are type Warning
        /// </summary>
        [DisplayName("Show Warnings"), Category(DISPLAYNAME_SEVERITY), DisplayOrder(2)]
        [Description("Include log records that are flagged as Warnings")]
        [DefaultValue(DEFAULT_WARNING)]
        public bool ShowWarnings
        {
            get { return showWarnings; }
            set { showWarnings = value; }
        }

        /// <summary>
        /// Include log records that are type Informational
        /// </summary>
        [DisplayName("Show Informational"), Category(DISPLAYNAME_SEVERITY), DisplayOrder(3)]
        [Description("Include log records that are flagged as Informational")]
        [DefaultValue(DEFAULT_INFORMATIONAL)]
        public bool ShowInformational
        {
            get { return showInformational; }
            set { showInformational = value; }
        }

        /// <summary>
        /// Start date for internal use only
        /// This may be greater than the user-defined start date
        /// If it is less than the user-defined start date it will be overridden
        /// </summary>
        [Browsable(false)]
        public DateTime? InternalStartDate
        {
            get { return internalStartDate; }
            set { internalStartDate = value; }
        }

        /// <summary>
        /// End date for internal use only
        /// This may be less than the user-defined start date
        /// If it is greater than the user-defined start date it will be overridden
        /// </summary>
        [Browsable(false)]
        public DateTime? InternalEndDate
        {
            get { return internalEndDate; }
            set { internalEndDate = value; }
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
            startDate = null;
            endDate = null;
            source = null;
            showErrors = true;
            showWarnings = true;
            showInformational = true;
        }

        public bool HasDefaultValues()
        {
            if (startDate != (DateTime?)DEFAULT_START_DATE
                    || endDate != (DateTime?)DEFAULT_END_DATE
                    || source != DEFAULT_SOURCE
                    || showErrors != DEFAULT_ERROR
                    || showWarnings != DEFAULT_WARNING
                    || showInformational != DEFAULT_INFORMATIONAL
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
            if (startDate.HasValue
                    || endDate.HasValue
                    || (source != null && source.Length > 0)
                    || !showErrors
                    || !showWarnings
                    || !showInformational
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
            startDate = (DateTime?)DEFAULT_START_DATE;
            endDate = (DateTime?)DEFAULT_END_DATE;
            source = DEFAULT_SOURCE;
            showErrors = DEFAULT_ERROR;
            showWarnings = DEFAULT_WARNING;
            showInformational = DEFAULT_INFORMATIONAL;
        }

        public void UpdateValues(IUserFilter selectionFilter)
        {
            if (selectionFilter is ErrorLogConfiguration)
            {
                ErrorLogConfiguration filter = (ErrorLogConfiguration)selectionFilter;
                startDate = filter.StartDate;
                endDate = filter.EndDate;
                source = filter.Source;
                showErrors = filter.ShowErrors;
                showWarnings = filter.ShowWarnings;
                showInformational = filter.ShowInformational;
            }
        }

        public bool Validate(out string Message)
        {
            if (StartDate.HasValue && EndDate.HasValue && StartDate.Value > EndDate.Value)
            {
                Message = String.Format(MESSAGE_DATES_OVERLAP, DISPLAYNAME_START_DATE, DISPLAYNAME_END_DATE);
                return false;
            }
            else if (!ShowErrors && !ShowWarnings && !ShowInformational)
            {
                Message = String.Format(MESSAGE_NO_SEVERITY, DISPLAYNAME_SEVERITY, DISPLAYNAME_SEVERITY);
                return false;
            }
            else
            {
                Message = String.Empty;
                return true;
            }
        }

        #endregion

        #endregion

        #region nested types

        #endregion

    }
}
