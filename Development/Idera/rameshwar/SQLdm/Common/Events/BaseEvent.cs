//------------------------------------------------------------------------------
// <copyright file="Event.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Events
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Idera.SQLdm.Common.Objects;

    /// <summary>
    /// Abstract class that defines common event properties.
    /// </summary>
    [Serializable]
    public abstract class BaseEvent : IEvent
    {
        #region fields

        private DateTime occuranceTime;
        private MonitoredObjectName monitoredObject;
        private int metricID;
        private MonitoredState monitoredState;
        private object value;
        private object metricValue;
        private bool isBaselineEnabled = false;
        private object additionalData;
        protected bool stateChanged;
        protected MonitoredState previousState;

        #endregion

        #region constructors

        public BaseEvent()
        {
            stateChanged = false;
            previousState = MonitoredState.OK;
        }

        //        public BaseEvent(MonitoredObjectName monitoredObject, Metric metric, MonitoredState monitoredState, object value, DateTime timestamp)
        //            : this(monitoredObject, (int)metric, monitoredState, value, timestamp)
        //        {
        //        }
        public BaseEvent(MonitoredObjectName monitoredObject, int metricID, MonitoredState monitoredState, object value, DateTime timestamp)
        {
            OccuranceTime = timestamp;
            MonitoredObject = monitoredObject;
            MetricID = metricID;
            MonitoredState = monitoredState;
            Value = value;           
            MetricValue = value;
            StateChanged = false;
            PreviousState = MonitoredState.OK;
        }
        public BaseEvent
        (
            MonitoredObjectName monitoredObject,
            int metricID,
            MonitoredState monitoredState,
            object value,
            DateTime timestamp,
            object additionalData) :
            this(monitoredObject, metricID, monitoredState, value, timestamp)
        {
            this.additionalData = additionalData;
        }
        /// <summary>
        /// SQLdm10.1[Srishti Purohit] --Creating overloaded consturctor To updated alert msg according to metric
        /// </summary>
        /// <param name="monitoredObject"></param>
        /// <param name="metricID"></param>
        /// <param name="monitoredState"></param>
        /// <param name="value"></param>
        /// <param name="isBaselineEnabled"></param>
        /// <param name="metricValue"></param>
        /// <param name="timestamp"></param>
        public BaseEvent(MonitoredObjectName monitoredObject, int metricID, MonitoredState monitoredState, object value, object metricValue, DateTime timestamp, bool isBaselineEnabled = false)
        { 
            OccuranceTime = timestamp;
            MonitoredObject = monitoredObject;
            MetricID = metricID;
            MonitoredState = monitoredState;
            Value = value;
            //If metricValue is null give it IComparable value
            if (metricValue == null && metricValue != value)
                metricValue = value;
            MetricValue = metricValue;
            IsBaselineEnabled = isBaselineEnabled;
            StateChanged = false;
            PreviousState = MonitoredState.OK;
        }
        /// <summary>
        /// SQLdm10.1[Srishti Purohit] --Creating overloaded consturctor To updated alert msg according to metric
        /// </summary>
        /// <param name="monitoredObject"></param>
        /// <param name="metricID"></param>
        /// <param name="monitoredState"></param>
        /// <param name="value"></param>
        /// <param name="metricValue"></param>
        /// <param name="timestamp"></param>
        /// <param name="additionalData"></param>
        public BaseEvent
        (
            MonitoredObjectName monitoredObject, 
            int metricID, 
            MonitoredState monitoredState, 
            object value,
            object metricValue,
            DateTime timestamp, 
            object additionalData,
            bool isBaselineEnabled = false) :
            this(monitoredObject, metricID, monitoredState, value, metricValue, timestamp, isBaselineEnabled)
        {
            this.additionalData = additionalData;
        }

        #endregion

        #region properties

        public DateTime OccuranceTime
        {
            get { return occuranceTime; }
            set { occuranceTime = value; }
        }

        public MonitoredObjectName MonitoredObject
        {
            get { return monitoredObject; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("monitoredObject");
                monitoredObject = value;
            }
        }

        /// <summary>
        /// Gets the event code.
        /// </summary>
        /// <value>The event code.</value>
        public int MetricID
        {
            get { return metricID; }
            set { metricID = value; }
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public MonitoredState MonitoredState
        {
            get { return monitoredState; }
            set { monitoredState = value; }
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
        /// SQLdm10.1[Srishti Purohit] -- To updated alert msg according to metric
        /// </summary>
        public object MetricValue
        {
            get { return metricValue; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                this.metricValue = value;
            }
        }
        /// <summary>
        /// SQLdm10.1[Srishti Purohit] -- To updated alert msg according to baseline threshold check for metric
        /// </summary>
        public bool IsBaselineEnabled
        {
            get { return isBaselineEnabled; }
            set
            {
                this.isBaselineEnabled = value;
            }
        }
        /// <summary>
        /// Gets or sets the threshold.
        /// </summary>
        /// <value>The threshold.</value>
        private byte[] SerializedValue
        {
            get
            {
                BinaryFormatter frmt = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    frmt.Serialize(ms, Value);
                    ms.Close();
                    return ms.GetBuffer();
                }
            }
            set
            {
                BinaryFormatter frmt = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream(value))
                {
                    Value = frmt.Deserialize(ms);
                }
            }
        }

        public object AdditionalData
        {
            get { return this.additionalData; }
            set { this.additionalData = value;  }
        }

        public virtual bool StateChanged
        {
            get { return stateChanged;  }
            set { stateChanged = value; }
        }

        public virtual MonitoredState PreviousState
        {
            get { return previousState; }
            set { previousState = value; }
        }

        #endregion

        public virtual Transition GetTransition()
        {
            return Transition.Info_Info;
        }
    }
}
