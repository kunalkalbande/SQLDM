//------------------------------------------------------------------------------
// <copyright file="ISnapshotSink.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Services
{
    using System;

    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.Data;

    /// <summary>
    /// The sink interface implemented by any object that wishes to request on-demand snapshots.
    /// We are using Serialized&lt;Snapshot&gt; so that we don't have to deserialize the snapshot 
    /// just to turn around and serialize it again to return to the client.
    /// </summary>
    public interface ISnapshotSink
    {
        #region properties

        /// <summary>
        /// Get or set the amount of time the management service will wait for the Process method
        /// to be called before timing out.  
        /// </summary>
        TimeSpan ManagementServiceWaitTime { get; set; }

        /// <summary>
        /// Sets a flag to signal users that the request using this sink should be cancelled.
        /// </summary>
        bool Cancelled { get; set; }

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Processes the specified snapshot.
        /// </summary>
        /// <param name="snapshot">The collected snapshot</param>
        /// <param name="state">State object passed from the caller</param>
        void Process(ISerialized snapshot, object state);

        /// <summary>
        /// Registers the sink in the management service so that it can be found
        /// using the returned id.
        /// </summary>
        /// <returns></returns>
        Guid RegisterSink();

        /// <summary>
        /// SQLdm 10.0 Vineet -- Added to support multiple probes
        /// </summary>
        /// <param name="listSnapshot"></param>
        /// <param name="state"></param>
        void ProcessMultiple(System.Collections.Generic.List<ISerialized> listSnapshot, object state);
        #endregion
    }
}
