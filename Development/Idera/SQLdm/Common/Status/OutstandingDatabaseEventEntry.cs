//------------------------------------------------------------------------------
// <copyright file="OutstandingDatabaseEvent.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Status
{
    using System;

    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;

    /// <summary>
    /// An outstanding database event.
    /// </summary>
    public class OutstandingDatabaseEventEntry // : OutstandingEventEntry
    {
        #region fields

        private MonitoredDatabase database;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:OutstandingDatabaseEvent"/> class.
        /// </summary>
        private OutstandingDatabaseEventEntry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:OutstandingDatabaseEvent"/> class.
        /// </summary>
        /// <param name="deviationEvent">The deviation event.</param>
        private OutstandingDatabaseEventEntry(StateDeviationEvent deviationEvent)
        //    : base(deviationEvent)
        {
//            Database = deviationEvent.MonitoredObject as MonitoredDatabase;
//            if (Database == null)
//                throw new ArgumentException("Attempt to create OutstandingDatabaseEvent from invalid StateDeviationEvent");
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>The server.</value>
        public MonitoredSqlServer Server
        {
            get { return Database.Server; }
            private set { }
        }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>The database.</value>
        public MonitoredDatabase Database
        {
            get { return database; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("database");
                database = value;
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
