//------------------------------------------------------------------------------
// <copyright file="ReplicationSummary.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Collections.Generic;
    using Idera.SQLdm.Common.Objects.Replication;

    /// <summary>
    /// Represents replication summary data
    /// </summary>
    [Serializable]
    public class ReplicationSummary
    {

        #region variables
        private ReplicationState replicationStatus = ReplicationState.Unknown;
        private TimeSpan subscriptionLatency = new TimeSpan();
        //10.0 SQLdm srishti purohit -- for baseline comparision
        private double? subscriptionLatencyBaselineMean;
        private double? subscriptionLatencyBaselinePerc;
        private Int64? nonSubscribedTransactions = null;
        private Int64? subscribedDeliveredTransactions = null;
        private Int64? nonDistributedTransactions = null;
        //10.0 SQLdm srishti purohit -- for baseline comparision
        private double? nonDistributedTransactionsBaselineMean = null;
        private double? nonDistributedTransactionsBaselinePerc = null;
        private cPublisher publisher = new cPublisher();
        //private cDistributor distributor = new cDistributor();
        //private Dictionary<string, publishedDB> publishedDatabases = new Dictionary<string, publishedDB>();
        private Dictionary<string, cDistributor> distributorDatabases = new Dictionary<string, cDistributor>();
        private Dictionary<int, subscribedDB> subscribedDatabases = new Dictionary<int, subscribedDB>();
        //START: SQLdm 10.0 (Tarun Sapra)- Fields for mean and perc
        private double? nonSubscribedTransactionsBaselineMean = null;
        private double? nonSubscribedTransactionsAsBaselinePerc = null;
        //END: SQLdm 10.0 (Tarun Sapra)- Fields for mean and perc
        #endregion

        #region constructors

        #endregion

        #region properties

        //START: SQLdm 10.0 (Tarun Sapra)- Fields for mean and perc
        public double? NonSubscribedTransactionsBaselineMean
        {
            get { return nonSubscribedTransactionsBaselineMean; }
            internal set { nonSubscribedTransactionsBaselineMean = value; }
        }
        public double? NonSubscribedTransactionsAsBaselinePerc
        {
            get { return nonSubscribedTransactionsAsBaselinePerc; }
            internal set { nonSubscribedTransactionsAsBaselinePerc = value; }
        }
        //END: SQLdm 10.0 (Tarun Sapra)- Fields for mean and perc

        /// <summary>
        /// Subscribed databases - the key is the publisher
        /// </summary>
        public Dictionary<int, subscribedDB> SubscribedDatabases
        {
            get { return subscribedDatabases; }
            set { subscribedDatabases = value; }
        }

        /// <summary>
        /// Distributor databases
        /// There is one distributor for each publisher
        /// </summary>
        public Dictionary<string, cDistributor> Distributors
        {
            get { return distributorDatabases; }
            set { distributorDatabases = value; }
        }

        public cPublisher Publisher
        {
            get { return publisher; }
            set { publisher = value; }
        }

        public ReplicationState ReplicationStatus
        {
            get { return replicationStatus; }
            internal set { replicationStatus = value; }
        }

        public TimeSpan SubscriptionLatency
        {
            get { return subscriptionLatency; }
            internal set { subscriptionLatency = value; }
        }

        //10.0 SQLdm srishti purohit -- for baseline comparision
        public double? SubscriptionLatencyBaselineMean
        {
            get { return subscriptionLatencyBaselineMean; }
            set { subscriptionLatencyBaselineMean = value; }
        }
        //10.0 SQLdm srishti purohit -- for baseline comparision
        public double? SubscriptionLatencyBaselinePerc
        {
            get { return subscriptionLatencyBaselinePerc; }
            set { subscriptionLatencyBaselinePerc = value; }
        }

        public long? NonSubscribedTransactions
        {
            get { return nonSubscribedTransactions; }
            internal set { nonSubscribedTransactions = value; }
        }

        public long? SubscribedDeliveredTransactions
        {
            get { return subscribedDeliveredTransactions; }
            internal set { subscribedDeliveredTransactions = value; }
        }

        public long? NonDistributedTransactions
        {
            get { return nonDistributedTransactions; }
            internal set { nonDistributedTransactions = value; }
        }
        //10.0 SQLdm srishti purohit -- for baseline comparision
        public double? NonDistributedTransactionsBaselineMean
        {
            get { return nonDistributedTransactionsBaselineMean; }
            internal set { nonDistributedTransactionsBaselineMean = value; }
        }
        //10.0 SQLdm srishti purohit -- for baseline comparision
        public double? NonDistributedTransactionsBaselinePerc
        {
            get { return nonDistributedTransactionsBaselinePerc; }
            internal set { nonDistributedTransactionsBaselinePerc = value; }
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
