//------------------------------------------------------------------------------
// <copyright file="RequestResult.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Services
{
    using System;
    using System.Threading;

    /// <summary>
    /// Result object for asynchronous remoted operations
    /// </summary>
    public class AsyncResult : MarshalByRefObject, IAsyncResult, IDisposable
    {
        #region fields
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("AsyncResult");

        private object asyncState;
        private ManualResetEvent asyncWaitHandle;
        private bool completedSynchronously;
        private bool isCompleted;
        private Result result;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AsyncRequestResult"/> class.
        /// </summary>
        /// <param name="asyncState">State of the async.</param>
        /// <param name="completedSynchronously">if set to <c>true</c> [completed synchronously].</param>
        /// <param name="isCompleted">if set to <c>true</c> [is completed].</param>
        public AsyncResult(object asyncState, bool completedSynchronously, bool isCompleted)
            : this(asyncState, null, completedSynchronously, isCompleted)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AsyncRequestResult"/> class.
        /// </summary>
        /// <param name="asyncState">State of the async.</param>
        /// <param name="asyncWaitHandle">The async wait handle.</param>
        /// <param name="completedSynchronously">if set to <c>true</c> [completed synchronously].</param>
        /// <param name="isCompleted">if set to <c>true</c> [is completed].</param>
        public AsyncResult(object asyncState, WaitHandle asyncWaitHandle, bool completedSynchronously, bool isCompleted)
        {
            AsyncState = asyncState;
            if (asyncWaitHandle != null)
                AsyncWaitHandle = asyncWaitHandle;
            CompletedSynchronously = completedSynchronously;
            IsCompleted = isCompleted;
        }

        #endregion

        #region properties

        #region IAsyncResult Members

        /// <summary>
        /// Gets a user-defined object that qualifies or contains information about an asynchronous operation.
        /// </summary>
        /// <value></value>
        /// <returns>A user-defined object that qualifies or contains information about an asynchronous operation.</returns>
        public object AsyncState
        {
            get { return asyncState; }
            protected set { asyncState = value; }
        }

        /// <summary>
        /// Gets a <see cref="T:System.Threading.WaitHandle"></see> that is used to wait for an asynchronous operation to complete.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Threading.WaitHandle"></see> that is used to wait for an asynchronous operation to complete.</returns>
        public WaitHandle AsyncWaitHandle
        {
            get 
            { 
                if (asyncWaitHandle == null)
                    asyncWaitHandle = new ManualResetEvent(IsCompleted);

                return asyncWaitHandle;
            }
            protected set
            {
                using (LOG.DebugCall("AsyncWaitHandle.Set")) {
                    if (asyncWaitHandle != value)
                    {
                        if (asyncWaitHandle != null)
                        {
                            LOG.Debug("Closing asyncWaitHandle");
                            asyncWaitHandle.Close();
                        }
                        asyncWaitHandle = value as ManualResetEvent;
                    }
                }
            }
        }

        /// <summary>
        /// Gets an indication of whether the asynchronous operation completed synchronously.
        /// </summary>
        /// <value></value>
        /// <returns>true if the asynchronous operation completed synchronously; otherwise, false.</returns>
        public bool CompletedSynchronously
        {
            get { return completedSynchronously; }
            protected set { completedSynchronously = value; }
        }

        /// <summary>
        /// Gets an indication whether the asynchronous operation has completed.
        /// </summary>
        /// <value></value>
        /// <returns>true if the operation is complete; otherwise, false.</returns>
        public bool IsCompleted
        {
            get { return isCompleted; }
            protected set
            {
                if (isCompleted != value)
                {
                    isCompleted = value;
                    // signal waiters that we are complete
                    if (asyncWaitHandle != null)
                        asyncWaitHandle.Set();
                }
            }
        }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public Result Result
        {
            get { return result; }
            protected set { result = value; }
        }

        #endregion

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Fires the completion.
        /// </summary>
        /// <param name="result">The result.</param>
        public void FireCompletion(Result result)
        {
            lock (this)
            {
                Result = result;
                CompletedSynchronously = false;
                IsCompleted = true;
            }
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

        public void Dispose()
        {
            if (asyncWaitHandle != null)
            {
                try
                {
                    asyncWaitHandle.Close();
                    LOG.Debug("Closed asyncWaitHandle");
                }
                catch (Exception e)
                {
                    LOG.Error("Exception closing manual reset event object: " + e.Message);
                }
            }
        }
    }
}
