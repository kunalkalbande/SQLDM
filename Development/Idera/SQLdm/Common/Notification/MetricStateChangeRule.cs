//------------------------------------------------------------------------------
// <copyright file="MetricStateChangeRule.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Notification
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [Serializable]
    public class MetricStateChangeRule
    {
        private List<MonitoredState> previousState;
        private List<MonitoredState> newState;

        private bool enabled;

        public MetricStateChangeRule()
        {
            Enabled = false;
        }

        [XmlAttribute]
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                if (!enabled)
                {
                    Initialize();
                }
            }
        }

        private void Initialize()
        {
            previousState = new List<MonitoredState>(3);
            newState = new List<MonitoredState>(3);
        }

        [XmlElement(typeof(MonitoredState))]
        public List<MonitoredState> NewState
        {
            get
            {
                return newState;
            }
            set
            {
                newState = value;
            }
        }

        [XmlElement(typeof(MonitoredState))]
        public List<MonitoredState> PreviousState
        {
            get
            {
                return previousState;
            }
            set
            {
                previousState = value;
            }
        }

        public bool IsValid()
        {
            // both lists must have a checked item
            if (previousState.Count == 0)
                return false;

            if (newState.Count == 0)
                return false;

            // if either list has only 1 checked item, that item must
            // not be checked in the other list
            if (newState.Count == 1 || previousState.Count == 1)
            {
                if (newState.Count == 1)
                    return !previousState.Contains(newState[0]);

                return !newState.Contains(previousState[0]);
            }
            return true;
        }
    }
}
