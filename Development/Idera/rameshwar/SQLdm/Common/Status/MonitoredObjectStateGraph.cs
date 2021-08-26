//------------------------------------------------------------------------------
// <copyright file="MonitoredObjectStateGraph.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Status
{
    using System;
    using System.Collections.Generic;

    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Thresholds;
    using Idera.SQLdm.Common.Objects;
    using System.Runtime.Serialization;
    using Idera.SQLdm.Common;
    using Wintellect.PowerCollections;

    /// <summary>
    /// Helper class used to keep track of a monitored object's state based on
    /// outstanding StateDeviationEvents.
    /// </summary>
    [Serializable]
    public class MonitoredObjectStateGraph : ISerializable
    {
        #region fields

        private MonitoredSqlServer monitoredServer;
        private IDictionary<int, IDictionary<int, MetricState>> deviationMap;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MonitoredObjectStateGraph"/> class.
        /// </summary>
        /// <param name="monitoredServer">The monitored object.</param>
        public MonitoredObjectStateGraph(MonitoredSqlServer monitoredServer)
        {
            MonitoredServer = monitoredServer;
            DeviationMap = new Dictionary<int, IDictionary<int, MetricState>>();
        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="T:MonitoredObjectStateGraph"/> class.
//        /// </summary>
//        /// <param name="deviations">The deviations.</param>
//        public MonitoredObjectStateGraph(IEnumerable<StateDeviationEvent> deviations)
//        {
//            DeviationMap = new Dictionary<int, IDictionary<int, StateDeviationEvent>>();
//            foreach (StateDeviationEvent deviation in deviations)
//            {
//                AddEvent(deviation);                
//            }
//        }

        protected MonitoredObjectStateGraph(SerializationInfo info, StreamingContext context)
        {
            monitoredServer = (MonitoredSqlServer)info.GetValue("monitoredServer", typeof(MonitoredSqlServer));
            deviationMap = (IDictionary<int, IDictionary<int, MetricState>>)
                info.GetValue("deviationMap", typeof(IDictionary<int, IDictionary<int, MetricState>>));
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the monitored object.
        /// </summary>
        /// <value>The monitored object.</value>
        public MonitoredSqlServer MonitoredServer
        {
            get { return monitoredServer; }
            private set
            {
                if(value == null)
                    throw new ArgumentNullException("monitoredServer");
                monitoredServer = value;
            }
        }

        /// <summary>
        /// Gets or sets the deviation map.
        /// </summary>
        /// <value>The deviation map.</value>
        protected IDictionary<int, IDictionary<int, MetricState>> DeviationMap
        {
            get { return deviationMap; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("deviationMap");
                deviationMap = value;
            }
        }

        #endregion

        #region methods

//        /// <summary>
//        /// Gets the state.
//        /// </summary>
//        /// <returns></returns>
//        public MonitoredState GetState()
//        {
//            MonitoredState state = MonitoredState.None;
//
//            foreach (IDictionary<Metric, StateDeviationEvent> deviations in DeviationMap.Values)
//            {
//                foreach (StateDeviationEvent deviation in deviations.Values)
//                {
//                    state = (MonitoredState)Math.Max((byte)state, (byte)deviation.MonitoredState);
//                }
//            }
//
//            return state;
//        }
//
//        /// <summary>
//        /// Gets the state.
//        /// </summary>
//        /// <param name="monitoredObjectId">The target.</param>
//        /// <returns></returns>
//        public MonitoredState GetState(int monitoredObjectId)
//        {
//            MonitoredState state = MonitoredState.None;
//
//            foreach (StateDeviationEvent deviation in GetEvents(monitoredObjectId))
//            {
//                state = (MonitoredState)Math.Max((byte)state, (byte)deviation.MonitoredState);
//            }
//            return state;
//        }
//
//        /// <summary>
//        /// Gets the state.
//        /// </summary>
//        /// <param name="monitoredObjectId">The target.</param>
//        /// <param name="metric">The metric.</param>
//        /// <returns></returns>
//        public MonitoredState GetState(int monitoredObjectId, Metric metric)
//        {
//            StateDeviationEvent deviation = GetEvent(monitoredObjectId, metric);
//
//            if (deviation == null)
//                return MonitoredState.OK;
//            else
//                return deviation.MonitoredState;
//        }

        public MetricState GetEvent(MonitoredObjectName objectName, int metricID)
        {
            return GetEvent(objectName.GetHashCode(), metricID);
        }

        /// <summary>
        /// Gets the event.
        /// </summary>
        /// <param name="monitoredObjectId">The target.</param>
        /// <param name="metricID">The metric.</param>
        /// <returns></returns>
        public MetricState GetEvent(int monitoredObjectId, int metricID)
        {
            IDictionary<int, MetricState> deviations = null;
            if (!DeviationMap.TryGetValue(monitoredObjectId, out deviations))
                return null;

            MetricState deviation = null;
            deviations.TryGetValue(metricID, out deviation);
            return deviation;
        }

        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MetricState> GetEvents()
        {
            List<MetricState> ret = new List<MetricState>();

            foreach (IDictionary<int,MetricState> deviations in DeviationMap.Values)
            {
                ret.AddRange(deviations.Values);
            }

            return ret;
        }

        /// <summary>
        /// Gets a map of all the outstanding state deviation events for a given metric.
        /// </summary>
        /// <param name="metric"></param>
        /// <returns></returns>
        public IDictionary<MonitoredObjectName,MetricState> GetEvents(Metric metric)
        {
            return GetEvents((int) metric);
        }

        public IDictionary<MonitoredObjectName,MetricState> GetEvents(int metricID)
        {
            Dictionary<MonitoredObjectName, MetricState> map =
                new Dictionary<MonitoredObjectName, MetricState>();

            MetricState deviation = null;

            foreach (IDictionary<int,MetricState> deviations in DeviationMap.Values)
            {
                if (deviations.TryGetValue(metricID, out deviation))
                    map.Add(deviation.MonitoredObject, deviation);
            }

            return map;
        }

        /// <summary>
        /// Return a map of all the databases and tables that have events.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string,Set<string>> GetDatabaseAndTableMap()
        {
            Dictionary<string, Set<string>> map = new Dictionary<string, Set<string>>();

            foreach (IDictionary<int,MetricState> deviations in DeviationMap.Values)
            {
                foreach (MetricState deviation in deviations.Values)
                {
                    MonitoredObjectName mon = deviation.MonitoredObject;
                    if (mon.IsDatabase)
                    {
                        if (!map.ContainsKey(mon.DatabaseName))
                            map.Add(mon.DatabaseName, new Set<string>());
                    } else
                    if (mon.IsTable)
                    {
                        if (!map.ContainsKey(mon.DatabaseName))
                            map.Add(mon.DatabaseName, new Set<string>());

                        map[mon.DatabaseName].Add(mon.TableName);
                    }
                    break;
                }
            }
       
            return map;
        }

        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <param name="monitoredObject">The target.</param>
        /// <returns></returns>
        public IEnumerable<MetricState> GetEvents(MonitoredObjectName monitoredObject)
        {
            IDictionary<int, MetricState> deviations = null;
            if (DeviationMap.TryGetValue(monitoredObject.GetHashCode(), out deviations))
                return Algorithms.ToArray(deviations.Values);

            return new List<MetricState>();
        }

        /// <summary>
        /// Adds the event.
        /// </summary>
        /// <param name="deviation">The deviation.</param>
        public void AddEvent(MetricState deviation)
        {
            IDictionary<int, MetricState> deviations = null;
            int id = GetId(deviation.MonitoredObject);
            if (!DeviationMap.TryGetValue(id, out deviations))
            {
                deviations = new Dictionary<int, MetricState>();
                DeviationMap[id] = deviations;
            }
            deviations[deviation.MetricID] = deviation;
        }

        /// <summary>
        /// Clears the event.
        /// </summary>
        /// <param name="deviation">The deviation.</param>
        /// <returns></returns>
        public bool ClearEvent(MetricState deviation)
        {
            IDictionary<int, MetricState> deviations = null;
            int id = GetId(deviation.MonitoredObject);
            if (DeviationMap.TryGetValue(id, out deviations))
            {
                bool ret = deviations.Remove(deviation.MetricID);
                if (deviations.Count == 0)
                    DeviationMap.Remove(id);

                return ret;
            }
            return false;
        }

        public void ClearAllEvents(int[] metrics)
        {
            ClearAllEvents(metrics, null);
        }

        public void ClearAllEvents(int[] metrics, DateTime? cutoffTime)
        {
            List<int> empties = null;

            foreach (int objectNameKey in DeviationMap.Keys)
            {
                MetricState metricState;
                IDictionary<int, MetricState> deviations;
                if (deviationMap.TryGetValue(objectNameKey, out deviations))
                {
                    foreach (int metric in metrics)
                    {
                        if (cutoffTime.HasValue)
                        {
                            if (!deviations.TryGetValue(metric, out metricState))
                                continue;

                            if (cutoffTime.Value < metricState.StartTime)
                                continue;
                        }
                        deviations.Remove(metric);
                        if (deviations.Count == 0)
                        {
                            if (empties == null)
                                empties = new List<int>();
                            empties.Add(objectNameKey);
                        }
                    }
                }
            }
            if (empties != null)
            {
                foreach (int objectNameKey in empties)
                {
                    deviationMap.Remove(objectNameKey);
                }
            }
        }

        private static int GetId(object obj)
        {
            if (obj is MonitoredObjectName)
                return obj.GetHashCode();
            if (obj is MonitoredSqlServer)
                return ((MonitoredSqlServer) obj).Id;
            if (obj is MonitoredObject)
                return ((MonitoredObject) obj).Id;
            if (obj is int)
                return (int)obj;
            return -1;
        }

        #endregion

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("monitoredServer", monitoredServer);
            info.AddValue("deviationMap", deviationMap);
        }
    }

    [Serializable]
    public class MetricState
    {
        public readonly MonitoredObjectName MonitoredObject;
        public readonly int MetricID;
        
        private DateTime startTime;
        private DateTime lastUpdate;

        private MonitoredState severity;
        private object additionalData;
        private object value;
        private object metricValue;

        private bool smoothed;
        private bool needsClearing;
        private bool acknowledged;
        private bool isBaselineEnabled;


        //SQLDM-30444
        private int timesThresholdCrossed;
        private int totalThresholdLimit;
        private bool isAlertable;

        public int TimesThresholdCrossed
        {
            get { return timesThresholdCrossed; }
            set { timesThresholdCrossed = value; }
        }

        public int TotalThresholdLimit
        {
            get { return totalThresholdLimit; }
            set { totalThresholdLimit = value; }
        }

        public bool IsAlertable
        {
            get { return isAlertable; }
            set { isAlertable = value; }
        }

        private StateDeviationEvent cachedEvent;

        public MetricState(MonitoredObjectName monitoredObject, int metricId, DateTime startTime)
        {
            //Cloning object
            MonitoredObject = (MonitoredObjectName)monitoredObject.Clone();
            MetricID = metricId;
            this.startTime = lastUpdate = startTime;
        }

        public StateDeviationEvent CachedEvent
        {
            get
            {
                if (cachedEvent == null)
                    cachedEvent = new StateDeviationEvent(this);
                return cachedEvent;
            }
        }

        public bool Acknowledged
        {
            get { return acknowledged; }
            set { acknowledged = value; }
        }

        public bool NeedsClearing
        {
            get { return needsClearing; }
            set { needsClearing = value; }
        }

        public bool Smoothed
        {
            get { return smoothed; }
            set { smoothed = value; }
        }

        public DateTime LastUpdate
        {
            get { return lastUpdate; }
            set { lastUpdate = value; }
        }

        public DateTime StartTime
        {
            get { return startTime; }
            set 
            { 
                startTime = value;
                if (cachedEvent != null)
                    cachedEvent.OccuranceTime = value;
            }
        }

        public MonitoredState MonitoredState
        {
            get { return severity; }
            set 
            { 
                severity = value;
                if (severity > MonitoredState.OK && cachedEvent != null)
                    cachedEvent.MonitoredState = value;
            }
        }

        public object Value
        {
            get { return value; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                this.value = value;
                if (cachedEvent != null)
                    cachedEvent.Value = value;                
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
                if (cachedEvent != null)
                    cachedEvent.MetricValue = value;
            }
        }
        public bool IsBaselineEnabled
        {
            get { return isBaselineEnabled; }
            set { isBaselineEnabled = value; }
        }

        public object AdditionalData
        {
            get { return this.additionalData; }
            set 
            { 
                this.additionalData = value;
                if (cachedEvent != null)
                    cachedEvent.AdditionalData = value;
            }
        }

        public override string ToString()
        {
            return String.Format("Issue: {5} - {0}[{1}] = {2}({3}) Start: {4}", MonitoredObject, MetricID, Value, MonitoredState, StartTime, LastUpdate);
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
            MetricState other = obj as MetricState;
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


    }

}
