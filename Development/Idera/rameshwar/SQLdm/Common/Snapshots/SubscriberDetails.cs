//------------------------------------------------------------------------------
// <copyright file="ReplicationPublisherQueue.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents subscription details
    /// </summary>
    [Serializable]
    public class SubscriberDetails : Snapshot
    {
        #region fields
        private ReplicationState replicationStatus = ReplicationState.Unknown;
        private string _publisherDataBase;
        private string _publisherInstance;
        private string _publicationName;
        private int _replicationType;
        private int _subscriptionType;
        private DateTime _LastUpdated;
        private int _lastSyncStatus;
        private string _lastSyncSummary;
        private DateTime _lastSyncTime;
        private string _subscriberDatabase;

        #endregion

        #region constructors
        /// <summary>
        /// Call this constructor to create a subscriberdetails snapshot with just an error
        /// </summary>
        /// <param name="ErrorMessage"></param>
        public SubscriberDetails(string ErrorMessage)
        {
            SetError(ErrorMessage, null);
        }

        public SubscriberDetails(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
        }

        #endregion

        #region properties

        public string PublisherInstance
        {
            get { return _publisherInstance; }
            set { _publisherInstance = value; }
        }

        public string PublisherDB
        {
            get { return _publisherDataBase; }
            set { _publisherDataBase = value; }
        }

        public string SubscriberDatabase
        {
            get { return _subscriberDatabase; }
            set { _subscriberDatabase = value; }
        }

        public string PublicationName
        {
            get { return _publicationName; }
            set { _publicationName = value; }
        }

        public int ReplicationType
        {
            get { return _replicationType; }
            set { _replicationType = value; }
        }

        public int SubscriptionType
        {
            get { return _subscriptionType; }
            set { _subscriptionType = value; }
        }

        public DateTime LastUpdated
        {
            get { return _LastUpdated; }
            set { _LastUpdated = value; }
        }

        public int LastSyncStatus
        {
            get { return _lastSyncStatus; }
            set { _lastSyncStatus = value; }
        }

        public string LastSyncSummary
        {
            get { return _lastSyncSummary; }
            set { _lastSyncSummary = value; }
        }

        public DateTime LastSyncTime
        {
            get { return _lastSyncTime; }
            set { _lastSyncTime = value; }
        }

        public ReplicationState ReplicationStatus
        {
            get { return replicationStatus; }
            internal set { replicationStatus = value; }
        }
        #endregion

        #region events

        #endregion

        #region methods
        /// <summary>
        /// Sets the error that caused collection to fail
        /// Only one exception is permitted as it is the one which initially caused the failure
        /// </summary>
        internal new void SetError(string message, Exception e)
        {
            if (Error == null)
            {
                if (e == null)
                {
                    Error = new Exception(message);
                }
                else
                {
                    Error = new Exception(String.Format(message, e.Message), e);
                }
            }
        }
        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
