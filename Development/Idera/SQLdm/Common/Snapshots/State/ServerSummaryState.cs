//------------------------------------------------------------------------------
// <copyright file="ServerSummaryStatus.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots.State
{
    using System;
    using System.Collections.Generic;
    
    using Idera.SQLdm.Common.Thresholds;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common;

    /// <summary>
    /// Enter a description for this class
    /// </summary>
    [Serializable]
    public class ServerSummaryState : SnapshotState, IEquatable<ServerSummaryState>
    {
        #region fields

        private MonitoredState agentStatus;
        private MonitoredState responseTime;
        private MonitoredState openTransactions;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ServerSummaryState"/> class.
        /// </summary>
        public ServerSummaryState()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ServerSummaryStatus"/> class.
        /// </summary>
        /// <param name="snapshot">The snapshot.</param>
        /// <param name="warning">The warning.</param>
        /// <param name="critical">The critical.</param>
        public ServerSummaryState(ServerSummary snapshot, IDictionary<Metric, MetricThresholdEntry> thresholds)
        {
            if (snapshot.AgentStatus != null && thresholds.ContainsKey(Metric.AgentServiceStatus))
                AgentStatus = GetState(snapshot.AgentStatus.Value, thresholds[Metric.AgentServiceStatus]);

            if (snapshot.ResponseTime != null && thresholds.ContainsKey(Metric.ServerResponseTime))
                ResponseTime = GetState(snapshot.ResponseTime.Value, thresholds[Metric.ServerResponseTime]);

            if (snapshot.OpenTransactions != null && thresholds.ContainsKey(Metric.ServerOpenTransactions))
                OpenTransactions = GetState(snapshot.OpenTransactions.Value, thresholds[Metric.ServerOpenTransactions]);
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the agent status.
        /// </summary>
        /// <value>The agent status.</value>
        public MonitoredState AgentStatus
        {
            get { return agentStatus; }
            private set { agentStatus = value; }
        }

        /// <summary>
        /// Gets or sets the response time.
        /// </summary>
        /// <value>The response time.</value>
        public MonitoredState ResponseTime
        {
            get { return responseTime; }
            private set { responseTime = value; }
        }

        /// <summary>
        /// Gets or sets the open transactions.
        /// </summary>
        /// <value>The open transactions.</value>
        public MonitoredState OpenTransactions
        {
            get { return openTransactions; }
            private set { openTransactions = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #region IEquatable<ServerSummaryState> Members

        public bool Equals(ServerSummaryState other)
        {
            return
                (other != null &&
                ResponseTime == other.ResponseTime &&
                OpenTransactions == other.OpenTransactions);
        }

        #endregion

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
