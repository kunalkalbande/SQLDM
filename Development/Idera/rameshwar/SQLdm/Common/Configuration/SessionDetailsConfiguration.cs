//------------------------------------------------------------------------------
// <copyright file="SessionDetailsConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration object for the session details view.
    /// </summary>
    [Serializable]
    public sealed class SessionDetailsConfiguration : OnDemandConfiguration
    {
        #region fields

        private const int DEFAULT_TRACE_LENGTH_IN_MINUTES = 30;
        private const int DEFAULT_TRACE_RESTART_IN_MINUTES = 5;

        private const string MESSAGE_TRACE_VARIABLES_INCOMPATIBLE =
            "The trace length must be longer than the trace restart length";

        private int spidFilter;
        private bool traceOn;
        private int nextSequenceNumber = 1;
        private TimeSpan traceLength = TimeSpan.FromMinutes(DEFAULT_TRACE_LENGTH_IN_MINUTES);
        private TimeSpan traceRestartTime = TimeSpan.FromMinutes(DEFAULT_TRACE_RESTART_IN_MINUTES);

        #endregion

        #region constructors

        public SessionDetailsConfiguration(int monitoredServerId, int spidFilter, bool traceOn) : base(monitoredServerId)
        {
            this.spidFilter = spidFilter;
            this.traceOn = traceOn;
            nextSequenceNumber = 1;
        }

        #endregion

        #region properties

        new public bool ReadyForCollection
        {
            get 
            {
                string temp;
                return (SpidFilter > 0 && Validate(out temp)); 
            }
        }

        /// <summary>
        /// Session ID of session to gather details on
        /// </summary>
        public int SpidFilter
        {
            get { return spidFilter; }
            set { spidFilter = value; }
        }

        /// <summary>
        /// True to return trace data, false to stop trace
        /// </summary>
        public bool TraceOn
        {
            get { return traceOn; }
            set { traceOn = value; }
        }

        /// <summary>
        /// Default length of trace.  The trace will shut down after remaining idle for this length of time.
        /// Value will be converted to full minutes.
        /// </summary>
        public TimeSpan TraceLength
        {
            get { return traceLength; }
            set { traceLength = value; }
        }

        /// <summary>
        /// Threshold time remaining at which point the trace will be restarted.  This should always be higher than the refresh rate
        /// and lower than the trace length.  Setting this value too high can result in lost data.
        /// </summary>
        public TimeSpan TraceRestartTime
        {
            get { return traceRestartTime; }
            set { traceRestartTime = value; }
        }

        public int NextSequenceNumber
        {
            get { return nextSequenceNumber; }
            set { nextSequenceNumber = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods


        public bool Validate(out string Message)
        {
            if (TraceOn & TraceLength < TraceRestartTime)
            {
                Message = MESSAGE_TRACE_VARIABLES_INCOMPATIBLE;
                return false;
            }
            else
            {
                Message = String.Empty;
                return true;
            }
        }
        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
