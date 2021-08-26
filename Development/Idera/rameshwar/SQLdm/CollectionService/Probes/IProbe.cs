//------------------------------------------------------------------------------
// <copyright file="IProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.CollectionService.Probes
{
    using System;
    using System.Threading;

    using Idera.SQLdm.Common.Snapshots;
    using Monitoring;

    /// <summary>
    /// Interface implemented by all probes.
    /// </summary>
    public interface IProbe : IDisposable
    {
        #region properties

        /// <summary>
        /// Gets the async wait handle.
        /// </summary>
        /// <value>The async wait handle.</value>
        WaitHandle AsyncWaitHandle { get; }
        DateTime StartTime { get; }
        
        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Begins the probe.
        /// </summary>
        /// <param name="dataReadyCallback">The data ready callback.</param>
        /// <returns></returns>
        IAsyncResult BeginProbe(EventHandler<SnapshotCompleteEventArgs> dataReadyCallback);

        #endregion
    }

    public interface IOnDemandProbe
    {
        IOnDemandContext Context { get; set; }              
    }
 
}
