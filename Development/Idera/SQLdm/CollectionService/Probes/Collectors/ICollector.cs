//------------------------------------------------------------------------------
// <copyright file="ISqlBatch.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.CollectionService.Probes.Collectors
{
    using System;
    using System.Threading;

    /// <summary>
    /// Enter a description for this interface
    /// </summary>
    interface ICollector : IDisposable
    {
        #region properties

        WaitHandle AsyncWaitHandle { get; }

        #endregion

        #region events

        #endregion

        #region methods

        IAsyncResult BeginCollection(EventHandler<CollectorCompleteEventArgs> collectionCompleteCallback);

        #endregion
    }
}
