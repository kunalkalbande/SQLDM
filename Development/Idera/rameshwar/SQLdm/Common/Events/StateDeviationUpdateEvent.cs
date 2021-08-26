//------------------------------------------------------------------------------
// <copyright file="StateDeviationUpdateEvent.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Events
{
    using System;
    using System.Runtime.Serialization;
    using Objects;
    using Status;

    /// <summary>
    /// Sent from the collection service to the management service
    /// when a state deviation's state or value changes.
    /// </summary>
    [Serializable]
    public class StateDeviationUpdateEvent : IEvent, ISerializable
    {
        #region fields

        private StateDeviationEvent deviationEvent;
        private DateTime occuranceTime;
        protected bool stateChanged;
        //protected MonitoredState previousState;

        #endregion

        #region constructors

        public StateDeviationUpdateEvent(MetricState state) : this(state.CachedEvent, state.LastUpdate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:StateDeviationUpdateEvent"/> class.
        /// </summary>
        /// <param name="deviationEvent">The deviation event.</param>
//        public StateDeviationUpdateEvent(StateDeviationEvent deviationEvent)
//        {
//            DeviationEvent = deviationEvent;
//            OccuranceTime = DateTime.Now;
//        }

        public StateDeviationUpdateEvent(StateDeviationEvent deviationEvent, DateTime timestamp)
        {
            DeviationEvent = deviationEvent;
            OccuranceTime = timestamp;
        }

        protected StateDeviationUpdateEvent(SerializationInfo info, StreamingContext context)
        {
            deviationEvent = (StateDeviationEvent)info.GetValue("event", typeof(StateDeviationEvent));
            occuranceTime = info.GetDateTime("occuranceTime");
            stateChanged = info.GetBoolean("stateChanged");
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the deviation event.
        /// </summary>
        /// <value>The deviation event.</value>
        public StateDeviationEvent DeviationEvent
        {
            get { return deviationEvent; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("deviationEvent");
                deviationEvent = value;
            }
        }

        public MonitoredState PreviousState
        {
            get { return deviationEvent.PreviousState; }
            set 
            {
                if (value != null)
                    deviationEvent.PreviousState = value;
                else
                    deviationEvent.PreviousState = MonitoredState.OK;
            }
        }


        #endregion

        #region events

        #endregion

        #region methods

        public virtual Transition GetTransition()
        {
            Transition transition = Transition.OK_Info;

            switch (PreviousState)
            {
                case MonitoredState.Informational:
                    switch (MonitoredState)
                    {
                        case MonitoredState.OK:
                            transition = Transition.Info_OK;
                            break;
                        case MonitoredState.Informational:
                            transition = Transition.Info_Info;
                            break;
                        case MonitoredState.Warning:
                            transition = Transition.Info_Warning;
                            break;
                        case MonitoredState.Critical:
                            transition = Transition.Info_Critical;
                            break;
                    }
                    break;
                case MonitoredState.Warning:
                    switch (MonitoredState)
                    {
                        case MonitoredState.OK:
                            transition = Transition.Warning_OK;
                            break;
                        case MonitoredState.Informational:
                            transition = Transition.Warning_Info;
                            break;
                        case MonitoredState.Warning:
                            transition = Transition.Warning_Warning;
                            break;
                        case MonitoredState.Critical:
                            transition = Transition.Warning_Critical;
                            break;
                    }
                    break;
                case MonitoredState.Critical:
                    switch (MonitoredState)
                    {
                        case MonitoredState.OK:
                            transition = Transition.Critical_OK;
                            break;
                        case MonitoredState.Informational:
                            transition = Transition.Critical_Info;
                            break;
                        case MonitoredState.Warning:
                            transition = Transition.Critical_Warning;
                            break;
                        case MonitoredState.Critical:
                            transition = Transition.Critical_Critical;
                            break;
                    }
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
            return String.Format("Deviation Update: {4} - {0}[{1}] = {2}({3}){5}", DeviationEvent.MonitoredObject, DeviationEvent.MetricID, DeviationEvent.Value, DeviationEvent.MonitoredState, OccuranceTime, StateChanged ? " State Changed" : String.Empty);
        }

        #endregion

        #region interface implementations

        #region IEvent Members

        /// <summary>
        /// Gets the event code.
        /// </summary>
        /// <value>The event code.</value>
        public int MetricID
        {
            get { return DeviationEvent.MetricID; }
        }

        /// <summary>
        /// Gets the occurance time.
        /// </summary>
        /// <value>The occurance time.</value>
        public DateTime OccuranceTime
        {
            get { return occuranceTime; }
            private set { occuranceTime = value; }
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public virtual MonitoredState MonitoredState
        {
            get { return DeviationEvent.MonitoredState; }
        }


        public MonitoredObjectName MonitoredObject
        {
            get { return deviationEvent.MonitoredObject; }
        }

        public object Value 
        {
            get { return deviationEvent.Value; } 
        }
        /// <summary>
        /// SQLdm10.1[Srishti Purohit] -- To updated alert msg according to metric
        /// </summary>
        public object MetricValue
        {
            get { return deviationEvent.MetricValue; }
        }
        /// <summary>
        /// SQLdm10.1[Srishti Purohit] -- To updated alert msg according to baseline threshold check for metric
        /// </summary>
        public bool IsBaselineEnabled
        {
            get { return deviationEvent.IsBaselineEnabled; }
        }

        public object AdditionalData
        {
            get { return deviationEvent.AdditionalData; }
            set
            {
                if (deviationEvent == null)
                    throw new ArgumentException("AdditionalData");
                deviationEvent.AdditionalData = value;
            }
        }

        public bool StateChanged
        {
            get { return stateChanged;  }
            set { stateChanged = value; }
        }

        #endregion

        #endregion

        #region nested types

        #endregion


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("event", deviationEvent);
            info.AddValue("occuranceTime", occuranceTime);
            info.AddValue("stateChanged", stateChanged);
        }
    }
}
