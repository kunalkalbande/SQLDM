//------------------------------------------------------------------------------
// <copyright file="ScheduledCollectionData.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Services
{
    using System;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Snapshots;
    using System.Collections.Generic;

    /// <summary>
    /// Message sent from the collection service to the management service that
    /// contains snapshots and events for a single collection of a single monitored server.
    /// </summary>
    [Serializable]
    public class ScheduledCollectionDataMessage : IDisposable
    {
        #region fields

        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("ScheduledCollectionDataMessage");
        private MonitoredSqlServer monitoredServer;
        private Snapshot snapshot;
        private List<Guid> snapshotSinks;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ScheduledCollectionData"/> class.
        /// </summary>
        /// <param name="monitoredServer">The monitored server.</param>
        public ScheduledCollectionDataMessage(MonitoredSqlServer monitoredServer, Snapshot snapshot)
        {
            MonitoredServer = monitoredServer;
            Snapshot = snapshot;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the monitored server.
        /// </summary>
        /// <value>The monitored server.</value>
        public MonitoredSqlServer MonitoredServer
        {
            get { return monitoredServer; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("monitoredServer");
                monitoredServer = value;
            }
        }

        /// <summary>
        /// Gets or sets the snapshot.
        /// </summary>
        /// <value>The snapshot.</value>
        public Snapshot Snapshot
        {
            get { return snapshot; }
            private set
            {
                if(value == null)
                    throw new ArgumentNullException("snapshot");
                snapshot = value;
            }
        }

        #endregion

        #region events

        #endregion

        #region methods

        public void Dispose()
        {
            snapshot = null;
            monitoredServer = null;
        }

        public void AddSink(ISnapshotSink sink)
        {
            try
            {
                if (sink.Cancelled)
                return;

                if (snapshotSinks == null)
                {
                    snapshotSinks = new List<Guid>();
                }
                snapshotSinks.Add(sink.RegisterSink());
            }
            catch (Exception e)
            {
                LOG.Warn("Error adding sink to on-demand refresh request: ", e);
            }
        }

        public List<Guid> GetSinks()
        {
            return snapshotSinks;
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
