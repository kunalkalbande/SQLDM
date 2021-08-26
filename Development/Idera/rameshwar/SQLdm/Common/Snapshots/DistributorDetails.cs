//------------------------------------------------------------------------------
// <copyright file="DistributorQueue.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
using Idera.SQLdm.Common.Objects.Replication;

    /// <summary>
    /// Represents the queued items on the distributor for a given publisher
    /// </summary>
    [Serializable]
    public class DistributorDetails : Snapshot
    {
        #region fields
        private ReplicationState replicationStatus = ReplicationState.Unknown;
        private ReplicationType _replicationType = ReplicationType.Transaction;
        private string publisherServer = null;
        private DataTable nonSubscribedTransactions = new DataTable("NonSubscribedTransactions");
        private DataTable distributionDetails = new DataTable("distributionDetails");
        private string _distributorInstance;
        private string _distributorDatabase;
        private string _subscriptionDatabase;
        private string _subscriptionServer;
        #endregion

        #region constructors
        public DistributorDetails(string ErrorMessage)
        {
            SetError(ErrorMessage,null);
        }

        internal DistributorDetails(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            nonSubscribedTransactions.RemotingFormat = SerializationFormat.Binary;
            nonSubscribedTransactions.Columns.Add("EntryTime", typeof(DateTime));
            nonSubscribedTransactions.Columns.Add("PublisherDatabase", typeof(string));
            nonSubscribedTransactions.Columns.Add("Subscriber", typeof(string));
            nonSubscribedTransactions.Columns.Add("Command", typeof(string));

            distributionDetails.RemotingFormat = SerializationFormat.Binary;

        }


        #endregion

        #region properties
        
        public string DistributorInstance
        {
            get { return _distributorInstance; }
            set { _distributorInstance = value; }
        }

        public string DistributorDatabase
        {
            get { return _distributorDatabase; }
            set { _distributorDatabase = value; }
        }
        /// <summary>
        /// Gets the publisher server name
        /// </summary>
        public string PublisherServer
        {
            get { return publisherServer; }
            internal set { publisherServer = value; }
        }

        public DataTable DistributionDetails
        {
            get { return distributionDetails; }
            internal set { distributionDetails = value; }
        }

        public DataTable NonSubscribedTransactions
        {
            get { return nonSubscribedTransactions; }
            internal set { nonSubscribedTransactions = value; }
        }

        public ReplicationState ReplicationStatus
        {
            get { return replicationStatus; }
            internal set { replicationStatus = value; }
        }

        public ReplicationType SelectedReplicationType
        {
            get { return _replicationType; }
            set { _replicationType = value; }
        }

        public string DefaultSubscriptionDatabase
        {
            get { return _subscriptionDatabase; }
            set { _subscriptionDatabase = value; }
        }

        public string SubscriptionServer
        {
            get { return _subscriptionServer; }
            set { _subscriptionServer = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods
        
        public void InitializeDistributorDetails()
        {
            InitializeDistributorDetails(false);
        }
        
        public void InitializeDistributorDetails(bool initForMerge)
        {
            if(!initForMerge){
                distributionDetails.Columns.Add("Articles", typeof(int));
                distributionDetails.Columns.Add("PublisherDBID", typeof(int));
                distributionDetails.Columns.Add("PublisherID", typeof(int));
                distributionDetails.Columns.Add("Publisher", typeof(string));
                distributionDetails.Columns.Add("Distributor", typeof(string));
                distributionDetails.Columns.Add("Subscriber", typeof(string));
                distributionDetails.Columns.Add("SubscriptionType", typeof(string));
                distributionDetails.Columns.Add("Sync Type", typeof(string));
                distributionDetails.Columns.Add("PublicationID", typeof(Int32));
                distributionDetails.Columns.Add("Publication", typeof(string));
                distributionDetails.Columns.Add("SubscriberID", typeof(Int16));
                distributionDetails.Columns.Add("AgentID", typeof(int));
                distributionDetails.Columns.Add("Subscribed", typeof(Int64));
                distributionDetails.Columns.Add("Non-Subscribed", typeof(Int64));
                distributionDetails.Columns.Add("SubscriptionLatency", typeof(int));
                distributionDetails.Columns.Add("SubscriptionRate", typeof(double));
                distributionDetails.Columns.Add("CatchUpTime", typeof(DateTime));
                distributionDetails.Columns.Add("SampleTime", typeof(DateTime));
            }
            else
            {
                distributionDetails.Columns.Add("SubscriberName", typeof(string));
                distributionDetails.Columns.Add("MergeSubscriptionstatus", typeof(string));
                distributionDetails.Columns.Add("SubscriberDB", typeof(string));
                distributionDetails.Columns.Add("Type", typeof(int));
                distributionDetails.Columns.Add("Agent Name", typeof(string));
                distributionDetails.Columns.Add("Last Action", typeof(string));
                distributionDetails.Columns.Add("Delivery Rate", typeof(int));
                distributionDetails.Columns.Add("Publisher Insertcount", typeof(int));
                distributionDetails.Columns.Add("Publisher Updatecount", typeof(int));
                distributionDetails.Columns.Add("Publisher Deletecount", typeof(int));
                distributionDetails.Columns.Add("Publisher Conflicts", typeof(int));
                distributionDetails.Columns.Add("Subscriber Insertcount", typeof(int));
                distributionDetails.Columns.Add("Subscriber Updatecount", typeof(int));
                distributionDetails.Columns.Add("Subscriber Deletecount", typeof(int));
                distributionDetails.Columns.Add("Subscriber Conflicts", typeof(int));
            }

        }
        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
