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
    public class PublisherDetails : Snapshot
    {
        #region fields

        private ReplicationState replicationStatus = ReplicationState.Unknown;
        
        private string _distributor = null;
        private string _distributionDatabase = null;
        
        private string _publisherInstance;
        private string _publisherDB;
        private string _publicationName;
        private string _publicationDescription;
        private string _sqlAgentStatus;
        // This is tabular in case we choose to list all published databases but for 
        // now it is one published database
        private DataTable publishedDatabases = new DataTable("PublicationsTable");

        #endregion

        #region constructors
        /// <summary>
        /// Call this constructor to create a subscriberdetails snapshot with just an error
        /// </summary>
        /// <param name="ErrorMessage"></param>
        public PublisherDetails(string ErrorMessage)
        {
            SetError(ErrorMessage, null);
        }

        public PublisherDetails(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            publishedDatabases.RemotingFormat = SerializationFormat.Binary;
            publishedDatabases.Columns.Add("publisherDB", typeof(string));
            publishedDatabases.Columns.Add("replicatedTransactions", typeof(int));
            publishedDatabases.Columns.Add("replicationRate", typeof(double));
            publishedDatabases.Columns.Add("replicationLatency", typeof(double));
        }

        #endregion

        #region properties

        public string SQLAgentStatus
        {
            get { return _sqlAgentStatus; }
            set { _sqlAgentStatus = value; }
        }

        public string PublisherInstance
        {
            get { return _publisherInstance; }
            set { _publisherInstance = value; }
        }
        public string PublisherDatabase
        {
            get { return _publisherDB; }
            set { _publisherDB = value; }
        }
        public string PublicationName
        {
            get { return _publicationName; }
            set { _publicationName = value; }
        }
        public string PublicationDescription
        {
            get { return _publicationDescription; }
            set { _publicationDescription = value; }
        }

        /// <summary>
        /// This is a datatable but it should only have one row in our current configuration
        /// Populated from sp_replCounters
        /// </summary>
        public DataTable PublishedDatabases
        {
            get { return publishedDatabases; }
            internal set { publishedDatabases = value; }
        }
        /// <summary>
        /// Populated with results from sp_helpdistributor
        /// </summary>
        public ReplicationState ReplicationStatus
        {
            get { return replicationStatus; }
            internal set { replicationStatus = value; }
        }
        /// <summary>
        /// Populated with results from sp_helpdistributor
        /// </summary>
        public string Distributor
        {
            get { return _distributor; }
            internal set { _distributor = value; }
        }
        /// <summary>
        /// Populated with results from sp_helpdistributor
        /// </summary>
        public string DistributionDatabase
        {
            get { return _distributionDatabase; }
            internal set { _distributionDatabase = value; }
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
