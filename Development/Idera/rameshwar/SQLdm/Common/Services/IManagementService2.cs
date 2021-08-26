//------------------------------------------------------------------------------
// <copyright file="IManagementService2.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Services 
{
    using System;
    using Data;
    using Idera.SQLdm.Common.Configuration;
    using Snapshots;

    /// <summary>
    /// The interface implemented by the management service's hosted remotable object.  These
    /// methods are for use by the Collection Services.
    /// </summary>
    public interface IManagementService2 
    {
        /// <summary>
        /// Adds/Returns the ID of a collection service instance.  Called when installing
        /// a new collection service instance.
        /// </summary>
        /// <param name="machineName"></param>
        /// <param name="instanceName"></param>
        /// <param name="servicePort"></param>
        /// <returns></returns>
        Guid RegisterCollectionService(string machineName, string instanceName, string address, int servicePort, bool force);
        
        /// <summary>
        /// Removes a collection service from the management service & repository.  Called 
        /// when removing a collection service instance.
        /// </summary>
        /// <param name="machineName"></param>
        /// <param name="instanceName"></param>
        void UnregisterCollectionService(string machineName, string instanceName);
        
        /// <summary>
        /// Requests the configuration information for the collection service.
        /// </summary>
        /// <param name="collectionServiceInstance">The name of the collection service instance.</param>
        /// <param name="collectionServiceMachine">The name of the collection service machine.</param>
        /// <returns>Collection service workload</returns>
        CollectionServiceWorkload GetCollectionServiceWorkload(string collectionServiceInstance, string collectionServiceMachine);
        CollectionServiceWorkload GetCollectionServiceWorkload(Guid collectionServiceId);
        
        
        
/*        
        /// <summary>
        /// Opens the collection service session.
        /// </summary>
        /// <param name="serviceId">The service id.</param>
        /// <param name="hostname">The hostname.</param>
        /// <param name="displayName">Name of the display.</param>
        /// <returns>A new collection service session info.</returns>
        CollectionServiceWorkloadMessage OpenCollectionServiceSession(string collectionServiceInstance, string collectionServiceMachine, out Guid collectionServiceId);

        /// <summary>
        /// Reopens the collection service session.
        /// </summary>
        /// <param name="sessionId">The session id.</param>
        /// <returns></returns>
        CollectionServiceWorkloadMessage ReopenCollectionServiceSession(Guid sessionId);
*/

        Result CloseCollectionService(Guid collectionServiceId);

        /// <summary>
        /// Processes the collection service heartbeat.
        /// </summary>
        /// <param name="collectionServiceId">The collection service id</param>
        /// <param name="nextExpected">next time to expect a heartbeat</param>
        /// <returns></returns>
        Result ProcessCollectionServiceHeartbeat(Guid collectionServiceId, TimeSpan nextExpected, DateTime? lastSnapshotDeliveryAttempt, TimeSpan? lastSnapshotDeliveryAttemptTime, Exception lastSnapshotDeliveryException, int scheduledRefreshDeliveryTimeoutCount);


        /// <summary>
        /// Called by the client to send the scheduled collection data to the Management Service.
        /// </summary>
        void ProcessScheduledCollectionData(Guid collectionServiceId, int monitoredServerId, Serialized<ScheduledCollectionDataMessage> scheduledRefresh);

        /// <summary>
        /// Test method.
        /// </summary>
        /// <returns>the object passed to it</returns>
        object Echo(object incoming);

        //<summary>
        // Called by Collection Service to check and enforce license.
        //</summary>
        void CheckLicense(bool enforce);
    }
}
