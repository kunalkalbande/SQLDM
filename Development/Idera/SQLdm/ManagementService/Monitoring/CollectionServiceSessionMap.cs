//------------------------------------------------------------------------------
// <copyright file="CollectionServiceSessionMap.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService.Monitoring
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Stores collection service sessions by service id and session id so the session
    /// can be retrieved quickly by either key.
    /// </summary>
    public class CollectionServiceSessionMap { /*: IEnumerable<CollectionServiceSessionInfo>
    {
        #region fields

        private object lockObject;
        private Dictionary<Guid, CollectionServiceSessionInfo> serviceIdMap;
        private Dictionary<Guid, CollectionServiceSessionInfo> sessionIdMap;
 
        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CollectionServiceSessionMap"/> class.
        /// </summary>
        public CollectionServiceSessionMap()
        {
            lockObject = new object();
            serviceIdMap = new Dictionary<Guid, CollectionServiceSessionInfo>();
            sessionIdMap = new Dictionary<Guid, CollectionServiceSessionInfo>();
        }

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Adds the specified session id.
        /// </summary>
        /// <param name="sessionInfo">The session info.</param>
        public void Add(CollectionServiceSessionInfo sessionInfo)
        {
            lock (lockObject)
            {
                sessionIdMap[sessionInfo.SessionId] = sessionInfo;
                serviceIdMap[sessionInfo.ServiceId] = sessionInfo;
            }
        }

        /// <summary>
        /// Gets the by service id.
        /// </summary>
        /// <param name="serviceId">The service id.</param>
        /// <returns></returns>
        public CollectionServiceSessionInfo GetByServiceId(Guid serviceId)
        {
            lock (lockObject)
            {
                CollectionServiceSessionInfo sessionInfo = null;
                serviceIdMap.TryGetValue(serviceId, out sessionInfo);
                return sessionInfo;
            }
        }

        /// <summary>
        /// Gets the by session id.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <returns></returns>
        public CollectionServiceContext GetBySessionId(Guid sessionId)
        {
            lock (lockObject)
            {
                CollectionServiceSessionInfo sessionInfo = null;
                sessionIdMap.TryGetValue(sessionId, out sessionInfo);
                return sessionInfo;
            }
        }

        /// <summary>
        /// Removes the by service id.
        /// </summary>
        /// <param name="serviceId">The service id.</param>
        /// <returns></returns>
        public bool RemoveByServiceId(Guid serviceId)
        {
            lock (lockObject)
            {
                CollectionServiceSessionInfo sessionInfo = GetByServiceId(serviceId);
                if (sessionInfo == null)
                    return false;
                serviceIdMap.Remove(serviceId);

                return sessionIdMap.Remove(sessionInfo.SessionId);
            }
        }

        /// <summary>
        /// Removes the by session id.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <returns></returns>
        public bool RemoveBySessionId(Guid sessionId)
        {
            lock (lockObject)
            {
                CollectionServiceSessionInfo sessionInfo = GetBySessionId(sessionId);
                if (sessionInfo == null)
                    return false;
                sessionIdMap.Remove(sessionId);

                return serviceIdMap.Remove(sessionInfo.ServiceId);
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            lock (lockObject)
            {
                sessionIdMap.Clear();
                serviceIdMap.Clear();
            }
        }

        #region IEnumerable<CollectionServiceSessionInfo> Members

        public IEnumerator<CollectionServiceSessionInfo> GetEnumerator()
        {
            lock (lockObject)
            {
                return serviceIdMap.Values.GetEnumerator();
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (lockObject)
            {
                return serviceIdMap.Values.GetEnumerator();
            }
        }

        #endregion

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
                                                */
    }
}
