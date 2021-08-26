//------------------------------------------------------------------------------
// <copyright file="DistributionQueueConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;
    using Idera.SQLdm.Common.Objects.Replication;
    /// <summary>
    /// Configuration object for distributor queue on demand
    /// </summary>
    [Serializable]
    public class DistributorDetailsConfiguration : OnDemandConfiguration
    {
        #region fields

        private string _distributionDatabase = null;
        private string _publisherName = null;
        private string _publication = null;
        private string _publicationDatabase = null;
        private string _subscriptionDatabase = null;
        private string _subscriptionServer = null;
        private ReplicationType _replicationType = ReplicationType.Transaction; //{tran, snap, merge}
        private TimeSpan? filterTimeSpan = null;
        private bool? _getQueue = false;
        private DateTime? _LastSampleTime;
        //This must be passed in to compare against the current running since value in the snapshot to see if the server has been rebooted
        private DateTime? _runningSince;
        private long? _LastSubscribedTranCount;
        private string _errorMessage = null;
        #endregion

        #region constructors

        public DistributorDetailsConfiguration(int monitoredServerId,
            string publisherName,
            string publicationDatabase,
            string publicationName,
            string distributionDatabase,
            string subscriptionDatabase,
            ReplicationType replicationType)
            : this(monitoredServerId, publisherName, publicationDatabase, publicationName, distributionDatabase, subscriptionDatabase , replicationType, null, null, null, null)
        {
        }
        
        public DistributorDetailsConfiguration(int monitoredServerId, 
            string publisherName, 
            string publicationDatabase, 
            string publicationName, 
            string distributionDatabase, 
            string subscriptionDatabase,
            ReplicationType replicationType,
            bool? getQueue,
            DateTime? runningSince,
            long? LastSubscribedTranCount,
            DateTime? LastSampleTime)
            : base(monitoredServerId)
        {
            this._distributionDatabase = distributionDatabase;
            this._publisherName = publisherName;
            this._publication = publicationName;
            this._replicationType = replicationType;
            this._publicationDatabase = PublicationDatabase;
            this._getQueue = getQueue;
            this._runningSince = runningSince;
            this._LastSubscribedTranCount = LastSubscribedTranCount;
            this._LastSampleTime = LastSampleTime;
        }

        #endregion

        #region properties

        public string Error
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }
        new public bool ReadyForCollection
        {
            get { return (DistributionDatabase != null && PublisherName != null); }
        }

        public DateTime? LastSampleTime
        {
            get { return _LastSampleTime; }
            set { _LastSampleTime = value; }
        }

        public bool? GetQueue
        {
            get { return _getQueue; }
            set { _getQueue = value; }
        }

        public DateTime? RunningSince
        {
            get { return _runningSince; }
            set{_runningSince = value;}
        }
        
        public long? LastSubscribedTranCount
        {
            get { return _LastSubscribedTranCount; }
            set { _LastSubscribedTranCount = value; }
        }

        public string PublicationDatabase
        {
            get { return _publicationDatabase;  }
            set{_publicationDatabase = value;}
        }

        public string DistributionDatabase
        {
            get { return _distributionDatabase; }
            set { _distributionDatabase = value; }
        }
                
        public string SubscriptionDatabase
        {
            get { return _subscriptionDatabase;  }
            set{_subscriptionDatabase = value;}
        }

        public string SubscriptionServer
        {
            get { return _subscriptionServer; }
            set { _subscriptionServer = value; }
        }

        public string PublisherName
        {
            get { return _publisherName; }
            set { _publisherName = value; }
        }

        public string Publication
        {
            get { return _publication; }
            set { _publication = value; }
        }

        public ReplicationType ReplicationType
        {
            get { return _replicationType; }
            set { _replicationType = value; }
        }

        /// <summary>
        /// Leave null to return all
        /// </summary>
        public TimeSpan? FilterTimeSpan
        {
            get { return filterTimeSpan; }
            set { filterTimeSpan = value; }
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
