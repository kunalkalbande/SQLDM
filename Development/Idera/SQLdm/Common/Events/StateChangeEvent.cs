//------------------------------------------------------------------------------
// <copyright file="StateChangeEvent.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Events
{
    using System;
    using Idera.SQLdm.Common.Objects;

    /// <summary>
    /// Sent from the collection service to the management service
    /// every time a data point changes state.
    /// </summary>
    [Serializable]
    public class StateChangeEvent : BaseEvent
    {
        #region fields

        private MonitoredState previousState;
        private bool invalidObject;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:StateChangeEvent"/> class.
        /// </summary>
        private StateChangeEvent()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:StateChangeEvent"/> class.
        /// </summary>
        /// <param name="monitoredObject">The monitored object.</param>
        /// <param name="metric">The metric.</param>
        /// <param name="monitoredState">State of the monitored.</param>
        /// <param name="previousState">State of the previous.</param>
        /// <param name="value">The value.</param>
        public StateChangeEvent(MonitoredObjectName monitoredObject, int metricID, MonitoredState monitoredState, MonitoredState previousState, object value, DateTime timestamp, object additionalData)
            : base(monitoredObject, metricID, monitoredState, value, timestamp, additionalData)
        {
            PreviousState = previousState;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the state of the previous.
        /// </summary>
        /// <value>The state of the previous.</value>
        public MonitoredState PreviousState
        {
            get { return previousState; }
            set { previousState = value; }
        }

        /// <summary>
        /// Indicates if the referenced object is no longer valid.
        /// </summary>
        public bool InvalidObject
        {
            get { return invalidObject;  }
            set { invalidObject = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("StateChange: {0}[{1}] = {2}({3}->{4})", MonitoredObject, MetricID, Value, PreviousState, MonitoredState);
        }

        #endregion

    }
}
