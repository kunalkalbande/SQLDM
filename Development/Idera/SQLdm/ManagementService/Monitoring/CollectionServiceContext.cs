//------------------------------------------------------------------------------
// <copyright file="CollectionServiceContext.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Idera.SQLdm.Common.Services;

namespace Idera.SQLdm.ManagementService.Monitoring
{
    using System;
using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.ManagementService.Configuration;

    public delegate void CollectionServiceSessionInfoDelegate(CollectionServiceContext context);

    /// <summary>
    /// Contains information about a collection service session.
    /// </summary>
    public class CollectionServiceContext
    {
        #region fields

        private CollectionServiceInfo   collectionService;
        private Guid                    configurationVersionId;

        private DateTime? lastScheduledRefreshDeliveryAttempt;
        private TimeSpan? lastScheduledRefreshDeliveryDuration;
        private Exception lastScheduledRefreshException;
        private int scheduledRefreshDeliveryTimeoutCount;

        #endregion

        #region constructors

        public CollectionServiceContext(CollectionServiceInfo collectionService)
        {
            CollectionService = collectionService;
            configurationVersionId = Guid.NewGuid();
        }

        #endregion

        #region properties

        public CollectionServiceInfo CollectionService
        {
            get { return collectionService; }
            set { collectionService = value; }
        }

        /// <summary>
        /// Gets or sets the service id.
        /// </summary>
        /// <value>The service id.</value>
        public Guid ServiceId
        {
            get { return collectionService.Id; }
        }

        public Guid ConfigurationVersionId
        {
            get { return configurationVersionId; }
            set { configurationVersionId = value; }
        }

        public DateTime LastHeartbeatReceived
        {
            get { return collectionService.LastHeartbeatReceived; }
            set { collectionService.LastHeartbeatReceived = value; }
        }

        /// <summary>
        /// DateTime of when the MS will alert that the CS has had a heart attack.
        /// </summary>
        public DateTime NextHeartbeatExpected
        {
            get {
                return LastHeartbeatReceived + 
                    ManagementServiceConfiguration.HeartbeatInterval +
                    ManagementServiceConfiguration.HeartbeatInterval; 
            }
        }

        internal void SetLastScheduledDeliveryInfo(DateTime? lastSnapshotDeliveryAttempt, TimeSpan? lastSnapshotDeliveryAttemptTime, Exception lastSnapshotDeliveryException, int scheduledRefreshDeliveryTimeoutCount)
        {
            lock (collectionService)
            {
                this.lastScheduledRefreshDeliveryAttempt = lastSnapshotDeliveryAttempt;
                this.lastScheduledRefreshDeliveryDuration = lastSnapshotDeliveryAttemptTime;
                this.lastScheduledRefreshException = lastSnapshotDeliveryException;
                this.scheduledRefreshDeliveryTimeoutCount = scheduledRefreshDeliveryTimeoutCount;
            }
        }

        internal DateTime? LastScheduledRefreshDeliveryTime
        {
            get
            {
                lock (collectionService)
                {
                    return this.lastScheduledRefreshDeliveryAttempt;
                }
            }
        }

        internal TimeSpan? LastScheduledRefreshDeliveryDuration
        {
            get
            {
                lock (collectionService)
                {
                    return lastScheduledRefreshDeliveryDuration;
                }
            }       
        }

        internal Exception LastScheduledRefreshDeliveryException
        {
            get
            {
                lock (collectionService)
                {
                    return lastScheduledRefreshException;
                }
            }
        }
        
        internal int ScheduledRefreshDeliveryTimeoutCount
        {
            get
            {
                lock (collectionService)
                {
                    return scheduledRefreshDeliveryTimeoutCount;
                }
            }
        }

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <returns></returns>
        public ICollectionService GetService()
        {
            //return GetService<ICollectionService>("Collection");
//          Uri uri = new Uri(String.Format("tcp://{0}:{1}/Collection", collectionService.Address, collectionService.Port));

            // hard coded to use the loopback address for talking to the collection service
            Uri uri = new Uri(String.Format("tcp://127.0.0.1:{0}/Collection", collectionService.Port));
            ServiceCallProxy proxy = new ServiceCallProxy(typeof(ICollectionService), uri.ToString());
            return proxy.GetTransparentProxy() as ICollectionService;
        }

        public TYPE GetService<TYPE>(string name) where TYPE : class
        {
//            Uri uri = new Uri(String.Format("tcp://{0}:{1}/{2}", collectionService.Address, collectionService.Port, name));

            // hard coded to use the loopback address for talking to the collection service
            Uri uri = new Uri(String.Format("tcp://127.0.0.1:{0}/{1}", collectionService.Port, name));
//          return RemotingHelper.GetObject<TYPE>(uri.ToString());
            ServiceCallProxy proxy = new ServiceCallProxy(typeof(TYPE), uri.ToString());
            return proxy.GetTransparentProxy() as TYPE;            
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
