//------------------------------------------------------------------------------
// <copyright file="ReplicationPublisherQueue.cs" company="Idera, Inc.">
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
    /// Represents replication publisher queue
    /// </summary>
    [Serializable]
    public class PublisherQueue : Snapshot
    {
        #region fields

        private ReplicationState replicationStatus = ReplicationState.Unknown;
        private string distributor = null;
        private string distributionDatabase = null;

        private DataTable nonDistributedTransactions = new DataTable("NonDistributedTransactions");

        #endregion

        #region constructors
        
        internal PublisherQueue(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            nonDistributedTransactions.RemotingFormat = SerializationFormat.Binary;
            nonDistributedTransactions.Columns.Add("Database", typeof (string));
            nonDistributedTransactions.Columns.Add("Command", typeof(string));
        }

        #endregion

        #region properties

        public ReplicationState ReplicationStatus
        {
            get { return replicationStatus; }
            internal set { replicationStatus = value; }
        }

        public string Distributor
        {
            get { return distributor; }
            internal set { distributor = value; }
        }

        public string DistributionDatabase
        {
            get { return distributionDatabase; }
            internal set { distributionDatabase = value; }
        }

        public DataTable NonDistributedTransactions
        {
            get { return nonDistributedTransactions; }
            internal set { nonDistributedTransactions = value; }
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
