//------------------------------------------------------------------------------
// <copyright file="DistributionQueueConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration object for distributor queue on demand
    /// </summary>
    [Serializable]
    public class DistributorQueueConfiguration : OnDemandConfiguration
    {
        #region fields

        private string _distributionDatabase = null;
        private string _publisherName = null;
        private string _publication = null;
        private TimeSpan? filterTimeSpan = null;
        private string _subscriberName = null;
        private string _subscriberDatabase = null;
        #endregion

        #region constructors

        public DistributorQueueConfiguration(int monitoredServerId, string distributionDatabase, string publisherName, string publication, string subscriberName, string subscriberDatabase) : base(monitoredServerId)
        {
            _distributionDatabase = distributionDatabase;
            _publisherName = publisherName;
            _publication = publication;
            _subscriberName = subscriberName;
            _subscriberDatabase = subscriberDatabase;
        }

        #endregion

        #region properties

        new public bool ReadyForCollection
        {
            get { return (DistributionDatabase != null && PublisherName != null); }
        }

        public string Publication
        {
            get { return _publication; }
            set { _publication = value; }
        }
        public string DistributionDatabase
        {
            get { return _distributionDatabase; }
            set { _distributionDatabase = value; }
        }

        public string PublisherName
        {
            get { return _publisherName; }
            set { _publisherName = value; }
        }
        
        public string SubscriberName
        {
            get { return _subscriberName; }
            set { _subscriberName = value; }
        }

        public string SubscriberDatabase
        {
            get { return _subscriberDatabase; }
            set { _subscriberDatabase = value; }
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
