//------------------------------------------------------------------------------
// <copyright file="DistributorQueue.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the queued items on the distributor for a given publisher
    /// </summary>
    [Serializable]
    public class DistributorQueue : Snapshot
    {
        #region fields

        private string publisherServer = null;
        private DataTable nonSubscribedTransactions = new DataTable("NonSubscribedTransactions");
        
        #endregion

        #region constructors

        internal DistributorQueue(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            nonSubscribedTransactions.RemotingFormat = SerializationFormat.Binary;
            nonSubscribedTransactions.Columns.Add("EntryTime", typeof(DateTime));
            nonSubscribedTransactions.Columns.Add("PublisherDatabase", typeof(string));
            nonSubscribedTransactions.Columns.Add("Subscriber", typeof(string));
            nonSubscribedTransactions.Columns.Add("Command", typeof(string));
        }


        #endregion

        #region properties

        /// <summary>
        /// Gets the publisher server name
        /// </summary>
        public string PublisherServer
        {
            get { return publisherServer; }
            internal set { publisherServer = value; }
        }


        public DataTable NonSubscribedTransactions
        {
            get { return nonSubscribedTransactions; }
            internal set { nonSubscribedTransactions = value; }
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
