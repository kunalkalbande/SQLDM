//------------------------------------------------------------------------------
// <copyright file="SnapshotReadyEventArgs.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.CollectionService.Probes
{
    using System;

    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Snapshots;

    /// <summary>
    /// Event arguments that contain the collected snapshot and result.  An instance of
    /// this class is passed to the callback provided when a probe is launched.
    /// </summary>
    public class SnapshotCompleteEventArgs : EventArgs
    {
        #region fields

        private Snapshot snapshot;
        private Result result;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SnapshotReadyEventArgs"/> class.
        /// </summary>
        /// <param name="snapshot">The snapshot.</param>
        public SnapshotCompleteEventArgs(Snapshot snapshot, Result result)
        {
            Snapshot = snapshot;
            Result = result;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the snapshot.
        /// </summary>
        /// <value>The snapshot.</value>
        public Snapshot Snapshot
        {
            get { return snapshot; }
            private set { snapshot = value; }
        }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        public Result Result
        {
            get { return result; }
            private set { result = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
