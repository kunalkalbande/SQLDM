//------------------------------------------------------------------------------
// <copyright file="CollectionServiceWorkload.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Events;
    using Wintellect.PowerCollections;

    /// <summary>
    /// Stores information about a specific Collection Service, along with the
    /// Collection Service's scheduled workload.
    /// </summary>
    [Serializable]
    public class CollectionServiceWorkload : ISerializable
    {
        #region fields

        private CollectionServiceInfo collectionServiceInfo;
        private IList<MonitoredServerWorkload> monitoredServerWorkloads;
        private MetricDefinitions metricDefinitions;
        private MultiDictionary<int, int> customCounterTags;
        private List<Pair<String, int?>> excludedWaitTypes;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CollectionServiceWorkload"/> class.
        /// </summary>
        public CollectionServiceWorkload()
        {
        }

        public CollectionServiceWorkload(CollectionServiceInfo csi)
        {
            collectionServiceInfo = csi;
            MonitoredServerWorkloads = new List<MonitoredServerWorkload>();
        }

        public CollectionServiceWorkload(CollectionServiceInfo csi, IList<MonitoredServerWorkload> workload, MetricDefinitions metricDefinitions)
        {
            this.collectionServiceInfo = csi;
            MonitoredServerWorkloads = workload;
            this.metricDefinitions = metricDefinitions;
        }

        protected CollectionServiceWorkload(SerializationInfo info, StreamingContext context)
        {
            collectionServiceInfo = (CollectionServiceInfo)info.GetValue("collectionServiceInfo", typeof(CollectionServiceInfo));
            monitoredServerWorkloads = (IList<MonitoredServerWorkload>)info.GetValue("monitoredServerWorkloads", typeof(IList<MonitoredServerWorkload>));
            metricDefinitions = (MetricDefinitions)info.GetValue("metricDefinitions", typeof(MetricDefinitions));
            customCounterTags = (MultiDictionary<int, int>)info.GetValue("customCounterTags", typeof(MultiDictionary<int, int>));
            excludedWaitTypes = (List<Pair<string,int?>>)info.GetValue("excludedWaitTypes", typeof(List<Pair<string,int?>>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("collectionServiceInfo", collectionServiceInfo);
            info.AddValue("monitoredServerWorkloads", monitoredServerWorkloads);
            info.AddValue("metricDefinitions", metricDefinitions);

            if (customCounterTags == null || customCounterTags.Count == 0)
                info.AddValue("customCounterTags", null);
            else
                info.AddValue("customCounterTags", customCounterTags);

            info.AddValue("excludedWaitTypes", excludedWaitTypes);
        }

        public CollectionServiceInfo CollectionService
        {
            get { return collectionServiceInfo;  }
        }
        
        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public Guid Id
        {
            get { return collectionServiceInfo.Id; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:CollectionServiceInfo"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled
        {
            get { return collectionServiceInfo.Enabled; }
        }

        public DateTime NextHeartbeatExpectedUTC
        {
            get { return collectionServiceInfo.LastHeartbeatReceived; }
        }
        
        /// <summary>
        /// Gets or sets the monitored server workloads.
        /// </summary>
        /// <value>The monitored server workloads.</value>
        public IList<MonitoredServerWorkload> MonitoredServerWorkloads
        {
            get { return monitoredServerWorkloads; }
            set { monitoredServerWorkloads = value; }
        }

        public MetricDefinitions MetricDefinitions
        {
            get { return metricDefinitions;  }
            set { metricDefinitions = value; }
        }

        public MultiDictionary<int,int> CustomCounterTags
        {
            get
            {
                if (customCounterTags == null)
                    customCounterTags = new MultiDictionary<int, int>(false);
                return customCounterTags;
            }
            set { customCounterTags = value; }
        }

        public List<Pair<String, int?>> ExcludedWaitTypes
        {
            get {
                if (excludedWaitTypes == null)
                {
                    excludedWaitTypes = new List<Pair<String, int?>>();
                }
                return excludedWaitTypes;
            }
            set { excludedWaitTypes = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
