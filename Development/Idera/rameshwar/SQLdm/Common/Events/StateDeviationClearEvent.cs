//------------------------------------------------------------------------------
// <copyright file="StateClearEvent.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Events
{
    using System;
    using System.Runtime.Serialization;
    using Status;

    /// <summary>
    /// Sent from the collection service to the management service when
    /// a state deviation is resolved.
    /// </summary>
    [Serializable]
    public class StateDeviationClearEvent : StateDeviationUpdateEvent
    {
        public StateDeviationClearEvent(MetricState deviationEvent) : base(deviationEvent)
        {
            stateChanged = true;
        }

//        public StateDeviationClearEvent(StateDeviationEvent deviationEvent) : base(deviationEvent)
//        {
//            stateChanged = true;
//        }

        public StateDeviationClearEvent(StateDeviationEvent deviationEvent, DateTime timestamp) : base(deviationEvent, timestamp)
        {
            stateChanged = true;
        }

        protected StateDeviationClearEvent(SerializationInfo info, StreamingContext context) : base (info, context)
        {
        }

        public override MonitoredState MonitoredState
        {
            get
            {
                return MonitoredState.OK;
            }
        }

        public override Transition GetTransition()
        {
            Transition transition = Transition.Critical_OK;

            switch (DeviationEvent.MonitoredState)
            {
                case MonitoredState.Informational:
                    transition = Transition.Info_OK;
                    break;
                case MonitoredState.Warning:
                    transition = Transition.Warning_OK;
                    break;
                case MonitoredState.Critical:
                    transition = Transition.Critical_OK;
                    break;
            }
            return transition;
        }

        public override string ToString()
        {
            return String.Format("Deviation Clear: {4} - {0}[{1}] = {2}({3})", DeviationEvent.MonitoredObject, DeviationEvent.MetricID, DeviationEvent.Value, DeviationEvent.MonitoredState, OccuranceTime);
        }

    }
}
