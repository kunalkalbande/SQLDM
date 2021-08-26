//------------------------------------------------------------------------------
// <copyright file="OutstandingEvent.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Status
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Thresholds;

    /// <summary>
    /// Contains an outstanding event
    /// </summary>
    public class OutstandingEventEntry
    {
//        private Guid id;
        private DateTime occuranceTime;
        private int metricID;
        private MonitoredState state;
        private object value;

        private MonitoredObjectName monitoredObject;

        public OutstandingEventEntry()
        {
        }

        public OutstandingEventEntry(StateDeviationEvent deviationEvent)
        {
            if (deviationEvent == null)
                throw new ArgumentNullException("deviationEvent");

            OccuranceTime = deviationEvent.OccuranceTime;
            MetricID = deviationEvent.MetricID;
            State = deviationEvent.MonitoredState;
            Value = deviationEvent.Value;

            MonitoredObjectName = deviationEvent.MonitoredObject;

        }

        /// <summary>
        /// Gets or sets the occurance time.
        /// </summary>
        /// <value>The occurance time.</value>
        public DateTime OccuranceTime
        {
            get { return occuranceTime; }
            set { occuranceTime = value; }
        }

        /// <summary>
        /// Gets or sets the metric.
        /// </summary>
        /// <value>The metric.</value>
        public int MetricID
        {
            get { return metricID; }
            set { metricID = value; }
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public MonitoredState State
        {
            get { return state; }
            set { state = value; }
        }

        /// <summary>
        /// Gets or sets the monitored object name.
        /// </summary>
        /// <value>The state.</value>
        public MonitoredObjectName MonitoredObjectName
        {
            get { return monitoredObject; }
            set { monitoredObject = value; }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get { return value; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                this.value = value;
            }
        }

        /// <summary>
        /// Gets or sets the threshold.
        /// </summary>
        /// <value>The threshold.</value>
        public string SerializedValue
        {
            get
            {
                XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(object), Threshold.SUPPORTED_TYPES);
                StringBuilder buffer = new StringBuilder();
                using (XmlWriter writer = XmlWriter.Create(buffer))
                {
                    serializer.Serialize(writer, value);
                    writer.Flush();
                }
                return buffer.ToString();
            }
            set
            {
                XmlSerializer serializer = Idera.SQLdm.Common.Data.XmlSerializerFactory.GetSerializer(typeof(object), Threshold.SUPPORTED_TYPES);
                Value = serializer.Deserialize(XmlReader.Create(new StringReader(value)));
            }
        }
    }
}
