//------------------------------------------------------------------------------
// <copyright file="OutstandingTableEvent.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Status
{
    using System;

    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Objects;

    /// <summary>
    /// An outstanding table event.
    /// </summary>
    public class OutstandingTableEventEntry // : OutstandingEventEntry
    {
        #region fields

//        private MonitoredTable table;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:OutstandingTableEvent"/> class.
        /// </summary>
        private OutstandingTableEventEntry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:OutstandingTableEvent"/> class.
        /// </summary>
        /// <param name="deviationEvent">The deviation event.</param>
        private OutstandingTableEventEntry(StateDeviationEvent deviationEvent)
        //    : base(deviationEvent)
        {
//            Table = deviationEvent.MonitoredObject as MonitoredTable;
//            if (Table == null)
//                throw new ArgumentException("Attempt to create OutstandingTableEvent from invalid StateDeviationEvent");
        }

        #endregion

        #region properties

//        /// <summary>
//        /// Gets or sets the server.
//        /// </summary>
//        /// <value>The server.</value>
//        public MonitoredSqlServer Server
//        {
//            get { return Database.Server; }
//            private set { }
//        }
//
//        /// <summary>
//        /// Gets or sets the database.
//        /// </summary>
//        /// <value>The database.</value>
//        public MonitoredDatabase Database
//        {
//            get { return Table.Database; }
//            private set { }
//        }
//
//        /// <summary>
//        /// Gets or sets the table.
//        /// </summary>
//        /// <value>The table.</value>
//        public MonitoredTable Table
//        {
//            get { return table; }
//            set
//            {
//                if (value == null)
//                    throw new ArgumentNullException("table");
//                table = value;
//            }
//        }

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
