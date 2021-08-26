//------------------------------------------------------------------------------
// <copyright file="BaseProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using BBS.TracerX;

namespace Idera.SQLdm.CollectionService.Probes
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Idera.SQLdm.CollectionService.Probes.Collectors;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Snapshots;

    /// <summary>
    /// Abstract base probe class.  Probes are responsible for building snapshots by using
    /// one or more collector.
    /// </summary>
    abstract class BaseProbe : IProbe
    {
        #region fields

        private AsyncResult asyncResult;
        private object asyncResultLock = new object();
        private DateTime startTime;
        private EventHandler<SnapshotCompleteEventArgs> snapshotReadyCallback;
        private bool hasCompleted;

        #endregion

        #region constructors

        protected BaseProbe()
        {
            StartTime = DateTime.Now;
        }

        #endregion

        #region properties

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (asyncResult != null)
                    return asyncResult.AsyncWaitHandle;

                return null;
            }
        }

        public DateTime StartTime
        {
            get { return startTime; }
            private set { startTime = value; }
        }

        protected bool IsFinished
        {
            get
            {
                if (asyncResult != null)
                    return asyncResult.IsCompleted;

                return false;
            }
        }

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Starts collection in a derived class.
        /// </summary>
        protected abstract void Start();

        public IAsyncResult BeginProbe(EventHandler<SnapshotCompleteEventArgs> snapshotReadyCallback)
        {
            this.snapshotReadyCallback = snapshotReadyCallback;
            asyncResult = new AsyncResult(null, false, false);

            Start();

            return asyncResult;
        }
        
        // Pass ProbeError to handle permission errors
        protected void FireCompletion(Snapshot snapshot, Result result, string collectorName = null, ProbePermissionHelpers.ProbeError probeError = null)
        {
            // Add check to ensure FireCompletion has not already fired
            if (!hasCompleted)
            {
                lock (asyncResultLock)
                {
                    
                    if (result == Result.Failure || result == Result.Unsupported)
                    {
                        if (snapshot.Error == null)
                        {
                            snapshot.SetError(
                                string.Format("[{0}] : Collection Failed.{1}", snapshot.ServerName,
                                    probeError != null ? Environment.NewLine + probeError.ToString() : string.Empty),
                                null);
                        }
                    }

                    if (result == Result.PermissionViolation)
                    {
                        if (snapshot.Error == null)
                        {
                            // Update Probe Error
                            probeError = probeError ?? snapshot.ProbeError ?? new ProbePermissionHelpers.ProbeError()
                            {
                                Name = collectorName
                            };
                            
                            // Update Error Message with Insufficient Access Permissions with Collector Name
                            snapshot.SetPermissionError(
                                string.Format("[{0}] : {1} Probe Collection failed due to insufficient access. {2}{3}",
                                    snapshot.ServerName, collectorName, Environment.NewLine, probeError), null, probeError);
                        }
                    }

                    if (IsFinished)
                        return;

                    hasCompleted = true;

                    if (asyncResult != null)
                        asyncResult.FireCompletion(result);

                    if (snapshotReadyCallback != null)
                        snapshotReadyCallback(this, new SnapshotCompleteEventArgs(snapshot, result));
                }
            }
            else
            {
                Logger.GetLogger("BaseProbe").Error("FireCompletion fired twice.  Suppressing second return.");
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (asyncResult != null)
            {
                asyncResult.Dispose();
                asyncResult = null;
            }
        }

        #endregion

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
