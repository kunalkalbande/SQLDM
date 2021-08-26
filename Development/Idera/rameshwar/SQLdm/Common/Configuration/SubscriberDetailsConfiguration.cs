//------------------------------------------------------------------------------
// <copyright file="PublisherQueueConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;
    using Idera.SQLdm.Common.Snapshots;
    using System.Data;

    /// <summary>
    /// Configuration object for publisher details
    /// </summary>
    [Serializable]
    public class SubscriberDetailsConfiguration : OnDemandConfiguration
    {
        #region fields

        private string _subscriberDatabase;
        private string _publication;

        #endregion

        #region constructors

        /// <summary>
        /// Configuration object for subscriber details request
        /// </summary>
        /// <param name="monitoredServerId"></param>
        /// <param name="subscriberDatabase"></param>
        /// <param name="publisherInstance"></param>
        /// <param name="publisherDB"></param>
        public SubscriberDetailsConfiguration(int monitoredServerId, string subscriberDatabase, string publication)
            : base(monitoredServerId)
        {
            _subscriberDatabase = subscriberDatabase;
            _publication = publication;
        }

        #endregion

        #region properties

        public string Publication
        {
            get { return _publication; }
            set { _publication = value; }
        }

        public string SubscriberDatabase
        {
            get { return _subscriberDatabase; }
            set { _subscriberDatabase = value; }
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
