//------------------------------------------------------------------------------
// <copyright file="CustomCounterConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Configuration
{
    using System;
    using Events;

    /// <summary>
    /// Configuration object for custom counter collection
    /// </summary>
    [Serializable]
    public class CustomCounterConfiguration : OnDemandConfiguration
    {
        #region fields

        private int metricID = 0;
        private CustomCounterSnapshot previousSnapshot;
        private CustomCounterDefinition counterDefinition;

        #endregion

        #region constructors

        public CustomCounterConfiguration(int monitoredServerId, int metricID) : base(monitoredServerId)
        {
            this.metricID = metricID;
        }

        public CustomCounterConfiguration(int monitoredServerId, CustomCounterSnapshot previousSnapshot)
            : this(monitoredServerId, previousSnapshot.Definition)
        {
            this.previousSnapshot = previousSnapshot;
        }

        public CustomCounterConfiguration(int monitoredServerId, CustomCounterDefinition counterDefinition)
            : base(monitoredServerId)
        {
            // metricID may not have been set if this is a new counter that has not been added to the repository
            metricID = counterDefinition.MetricID;
            this.counterDefinition = counterDefinition;
        }

        #endregion

        #region properties

        public int MetricID
        {
            get { return metricID; }
            set { metricID = value; }
        }

        /// <summary>
        /// Previous snapshot in a series of collections.  The MetricID of a previous snapshot overrides the MetricID of 
        /// the configuration object if there is a conflict.
        /// </summary>
        public CustomCounterSnapshot PreviousSnapshot
        {
            get { return previousSnapshot; }
            set { previousSnapshot = value; }
        }

        public CustomCounterDefinition CounterDefinition
        {
            get { return counterDefinition; }
            set { counterDefinition = value; }
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
