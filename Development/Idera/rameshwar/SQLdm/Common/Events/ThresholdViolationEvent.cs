//------------------------------------------------------------------------------
// <copyright file="ThresholdViolationEvent.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Events
{
    using System;
    using Idera.SQLdm.Common.Objects;
    using System.Runtime.Serialization;

    /// <summary>
    /// Sent from the collection service to the management service
    /// every time data is collected that violates a threshold.
    /// </summary>
    [Serializable]
    public class ThresholdViolationEvent : BaseEvent
    {
        #region fields
    
        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ThresholdViolationEvent"/> class.
        /// </summary>
        /// <param name="monitoredObject">The monitored object.</param>
        /// <param name="metricID">The metric.</param>
        /// <param name="monitoredState">State of the monitored.</param>
        /// <param name="value">The value.</param>
        public ThresholdViolationEvent(MonitoredObjectName monitoredObject, int metricID, MonitoredState monitoredState, object value, DateTime timestamp, object additionalData)
            : base(monitoredObject, metricID, monitoredState, value, timestamp, additionalData)
        {
        }

        #endregion

        #region methods

//        public MonitoredSqlServer GetMonitoredServer()
//        {
//            object mo = MonitoredObject;
//            if (mo is MonitoredSqlServer)
//                return mo as MonitoredSqlServer;
//            if (mo is MonitoredDatabase)
//                return ((MonitoredDatabase) mo).Server;
//            if (mo is MonitoredTable)
//                return ((MonitoredTable) mo).Database.Server;
//            return null;
//        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("Threshold Violation: {0}[{1}] = {2}({3})", MonitoredObject, MetricID, Value, MonitoredState);
        }

        #endregion

    }

    [Serializable]
    public class OperationalThresholdViolationEvent : ThresholdViolationEvent
    {
        private DateTime _lastUpdated;

        public OperationalThresholdViolationEvent(MonitoredObjectName monitoredObject, int metricID, MonitoredState monitoredState, object value, DateTime timestamp, object additionalData) 
            : base(monitoredObject, metricID, monitoredState, value, timestamp, additionalData)
        {
            _lastUpdated = timestamp;
        }

        public OperationalThresholdViolationEvent(ThresholdViolationEvent oldEvent)
            : base (oldEvent.MonitoredObject, oldEvent.MetricID, oldEvent.MonitoredState, oldEvent.Value, oldEvent.OccuranceTime, oldEvent.AdditionalData)
        {
            _lastUpdated = oldEvent.OccuranceTime;
        }

        public DateTime LastUpdate
        {
            get { return _lastUpdated;  }
            set { _lastUpdated = value; }
        }
    }

}
