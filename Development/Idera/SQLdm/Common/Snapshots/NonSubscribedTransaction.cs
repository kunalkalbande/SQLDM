//------------------------------------------------------------------------------
// <copyright file="NonSubscribedTransaction.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Enter a description for this class
    /// </summary>
    public sealed class NonSubscribedTransaction
    {
        #region fields

        private string _distributor = null;
        private DateTime? _entryTime = null;
        private string _publisherDatabase = null;
        private string _publisherServer = null;
        private string _subscribingServer = null;
        
        #endregion

        #region constructors

        #endregion

        #region properties

        /// <summary>
        /// Gets the distributor database for the transaction
        /// </summary>
        public string Distributor
        {
            get { return _distributor; }
        }

        /// <summary>
        /// Gets the entry time of the transaction
        /// </summary>
        public DateTime? EntryTime
        {
            get { return _entryTime; }
        }

        /// <summary>
        /// Gets the publisher database name for the transaction
        /// </summary>
        public string PublisherDatabase
        {
            get { return _publisherDatabase; }
        }

        /// <summary>
        /// Gets the publisher server name for the transaction
        /// </summary>
        public string PublisherServer
        {
            get { return _publisherServer; }
        }

        /// <summary>
        /// Gets the subscribing server name for the transaction
        /// </summary>
        public string SubscribingServer
        {
            get { return _subscribingServer; }
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
