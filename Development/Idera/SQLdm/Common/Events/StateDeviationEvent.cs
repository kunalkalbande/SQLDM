//------------------------------------------------------------------------------
// <copyright file="StateDeviationEvent.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Events
{
    using System;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Status;

    /// <summary>
    /// Sent from the collection service to the management service when
    /// a value exceeds a threshold.
    /// </summary>
    [Serializable]
    public class StateDeviationEvent : BaseEvent
    {
        #region fields

        private bool acknowledged;

        #endregion

        #region constructors

        /// <summary>
        /// Private default constructor for hibernate
        /// </summary>
        protected StateDeviationEvent()
        {
            stateChanged = true;
        }
        public StateDeviationEvent(MonitoredObjectName monitoredObject, int metricID, MonitoredState monitoredState, object value, DateTime timestamp, object additionalData, object metricValue = null)
            : base(monitoredObject, metricID, monitoredState, value, metricValue, timestamp, additionalData)
        {
            stateChanged = true;
        }
        public StateDeviationEvent(MonitoredObjectName monitoredObject, int metricID, MonitoredState monitoredState, object value, DateTime timestamp, object additionalData, bool isBaselineEnabled, object metricValue = null)
            : base(monitoredObject, metricID, monitoredState, value, metricValue, timestamp, additionalData, isBaselineEnabled)
        {
            stateChanged = true;
        }

        public StateDeviationEvent(MetricState state)
            : this(state.MonitoredObject, state.MetricID, state.MonitoredState, state.Value, state.StartTime, state.AdditionalData, state.IsBaselineEnabled, state.MetricValue)
        {
        }

        public StateDeviationEvent(OutstandingEventEntry outstandingEvent)
        {
            OccuranceTime = outstandingEvent.OccuranceTime;
            MetricID = outstandingEvent.MetricID;
            MonitoredState = outstandingEvent.State;
            Value = outstandingEvent.Value;
            MonitoredObject = outstandingEvent.MonitoredObjectName;
        }

        public StateDeviationEvent(StateDeviationEvent copy) : base(copy.MonitoredObject, copy.MetricID, copy.MonitoredState, copy.Value, copy.OccuranceTime, copy.AdditionalData)
        {
            this.stateChanged = copy.StateChanged;
        }

        #endregion

        #region properties

        public bool Acknowledged
        {
            get { return acknowledged;  }
            set { acknowledged = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        public override Transition GetTransition()
        {
            Transition transition = Transition.OK_Critical;

            switch (MonitoredState)
            {
                case MonitoredState.Informational:
                    transition = Transition.OK_Info;
                    break;
                case MonitoredState.Warning:
                    transition = Transition.OK_Warning;
                    break;
                case MonitoredState.Critical:
                    transition = Transition.OK_Critical;
                    break;
            }

            return transition;
        }




        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return String.Format("Deviation: {4} - {0}[{1}] = {2}({3})", MonitoredObject, MetricID, Value, MonitoredState, OccuranceTime);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            StateDeviationEvent other = obj as StateDeviationEvent;
            if (other == null)
                return false;

            return (MetricID == other.MetricID && other.MonitoredObject.Equals(MonitoredObject));
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return (MetricID.ToString() + "~" + MonitoredObject.ToString()).GetHashCode();
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
