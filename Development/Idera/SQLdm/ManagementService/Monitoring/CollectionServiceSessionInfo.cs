//------------------------------------------------------------------------------
// <copyright file="CollectionServiceSessionInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Idera.SQLdm.Common.Services;

namespace Idera.SQLdm.ManagementService.Monitoring
{
    using System;
using Idera.SQLdm.Common.Configuration;

    public delegate void CollectionServiceSessionInfoDelegate(CollectionServiceSessionInfo session);

    /// <summary>
    /// Contains information about a collection service session.
    /// </summary>
    public class CollectionServiceSessionInfo
    {
        #region fields

        private CollectionServiceInfo   collectionService;
        private Guid                    configurationVersionId;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CollectionServiceSessionInfo"/> class.
        /// </summary>
        /// <param name="serviceId">The service id.</param>
        /// <param name="sessionId">The session id.</param>
        /// <param name="hostname">The hostname.</param>
        /// <param name="displayName">Name of the display.</param>
        public CollectionServiceSessionInfo(CollectionServiceInfo collectionService)
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

        public DateTime NextHeartbeatExpected
        {
            get { return collectionService.NextHeartbeatExpected; }
            set { collectionService.NextHeartbeatExpected = value; }
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
            object[] args = new object[] { collectionService.Address, collectionService.Port };
            Uri uri = new Uri(String.Format("tcp://{0}:{1}/Collection", args));
            return RemotingHelper.GetObject<ICollectionService>(uri.ToString());
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
