//------------------------------------------------------------------------------
// <copyright file="IEventContainer.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Idera.SQLdm.Common.Events;

    [Serializable]
    public abstract class AlertableSnapshot : Snapshot, IEventContainer
    {
        private List<IEvent> events = null;
        private int id;
        private List<Metric> alertableMetrics = new List<Metric>();

        protected AlertableSnapshot(string serverName) : base(serverName)
        {
        }

        public int Id
        {
            get { return id; }
            internal set { id = value; }
        }

        /// <summary>
        /// Server Hostname property for storing hostname for the snapshots
        /// </summary>
        /// <remarks>
        /// SQLdm 10.2 (Varun Chopra) - Defect SQLDM-26709 - Add Hostname type for alerting (SMTP, SNMP)
        /// </remarks>
        public string ServerHostName { get; set; }

        public abstract AlertableSnapshotType SnapshotType { get; }

        public IEnumerable<IEvent> Events
        {
            get
            {
                if (events == null)
                    return null;

                return events.ToArray();
            }
        }

        public int NumberOfEvents
        {
            get { return events != null ? events.Count : 0; }
        }

        public IEvent GetEvent(int i)
        {
            if (i >= NumberOfEvents)
                return null;

            return events[i];
        }

        public void AddEvent(IEvent evnt)
        {
            if (events == null)
                events = new List<IEvent>();

            events.Add(evnt);
        }

        public void AddEvent(IEnumerable<IEvent> evnts)
        {
            if (events == null)
                events = new List<IEvent>();

            events.AddRange(evnts);
        }

        private MonitoredState GetState(int metricID)
        {
            foreach (IEvent evnt in Events)
            {
                if (evnt.MetricID == metricID && evnt is ThresholdViolationEvent)
                {
                    ThresholdViolationEvent violation = evnt as ThresholdViolationEvent;
                    return violation.MonitoredState;
                }
            }
            return MonitoredState.OK;
        }

        private MonitoredState GetPreviousState(int metricID)
        {
            foreach (IEvent evnt in Events)
            {
                if (evnt.MetricID == metricID && evnt is StateChangeEvent)
                {
                    StateChangeEvent statechange = evnt as StateChangeEvent;
                    return statechange.PreviousState;
                }
            }
            return MonitoredState.None;
        }

        public abstract List<Metric> AlertableMetrics { get; set; }
    }
}
