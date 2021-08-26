//------------------------------------------------------------------------------
// <copyright file="BaseBatch.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.CollectionService.Probes.Collectors
{
    using System;
    using Idera.SQLdm.Common.Services;
    using System.Threading;

    /// <summary>
    /// Enter a description for this class
    /// </summary>
    abstract class BaseCollector : ICollector
    {
        #region fields

        protected ManualResetEvent asyncWaitHandle;
        protected EventHandler<CollectorCompleteEventArgs> collectionCompleteCallback;
        protected EventHandler<CollectorCompleteEventArgs> collectionNonQueryExecutionCompleteCallback; //SQLDM 10.3 (Manali Hukkeri) : Technical debt changes

        #endregion

        #region constructors

        protected BaseCollector()
        {
        }

        #endregion

        #region properties

        public WaitHandle AsyncWaitHandle
        {
            get { return asyncWaitHandle; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        public abstract IAsyncResult BeginCollection(EventHandler<CollectorCompleteEventArgs> collectionCompleteCallback);
        public abstract IAsyncResult BeginCollectionNonQueryExecution(EventHandler<CollectorCompleteEventArgs> collectionNonQueryExecutionCompleteCallback);

        protected void FireCompletion()
        {
            asyncWaitHandle.Set();
        }

        #endregion

        #region interface implementations

        public virtual void Dispose()
        {
            if (asyncWaitHandle != null)
            {
                try { asyncWaitHandle.Close(); } catch { /* */ }
            }
        }

        #endregion

        #region nested types

        #endregion

    }
}
