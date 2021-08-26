//------------------------------------------------------------------------------
// <copyright file="OutstandingServerEvent.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Status
{
    using System;

    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;

    /// <summary>
    /// An outstanding server event.
    /// </summary>
    public class OutstandingServerEventEntry // : OutstandingEventEntry
    {
        #region fields

        private MonitoredServer server;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:OutstandingServerEvent"/> class.
        /// </summary>
        private OutstandingServerEventEntry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:OutstandingServerEvent"/> class.
        /// </summary>
        /// <param name="deviationEvent">The deviation event.</param>
        private OutstandingServerEventEntry(StateDeviationEvent deviationEvent)
         //   : base(deviationEvent)
        {
//            Server = deviationEvent.MonitoredObject as MonitoredServer;
//            if (Server == null)
//                throw new ArgumentException("Attempt to create OutstandingServerEvent from invalid StateDeviationEvent");
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>The server.</value>
        public MonitoredServer Server
        {
            get { return server; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("server");
                server = value;
            }
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
