//------------------------------------------------------------------------------
// <copyright file="OutOfDateStatsProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    using System;
    using System.Data.SqlClient;
    using BBS.TracerX;
    using Collectors;
    using Common;
    using Common.Configuration;
    using Common.Services;
    using Common.Snapshots;

    /// <summary>
    /// //SQLdm 10.0 (Srishti Purohit) (Embedded SQLDoctor Analytics) - New Probe class
    /// </summary>
    internal class HashIndexProbe : SqlBaseProbe
    {
        #region fields

        private HashIndexSnapshot snapshot = null;
        private string db;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HashIndexProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public HashIndexProbe(SqlConnectionInfo connectionInfo, string DB, int? cloudProviderId)
            : base(connectionInfo)
        {
            db = DB;
            snapshot = new HashIndexSnapshot(connectionInfo);
            LOG = Logger.GetLogger("SessionsProbe");
            this.cloudProviderId = cloudProviderId;
        }

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Starts the probe, virtual method called by BaseProbe.BeginProbe().
        /// If this returns cleanly, probe must call FireCompletion()
        /// </summary>
        protected override void Start()
        {
            StartHashIndexCollector();
        }

        /// <summary>
        /// Define the OutOfDateStats collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void HashIndexCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildHashIndexCommand(conn, ver, db);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(HashIndexCallback));
        }

        /// <summary>
        /// Starts the Configuration collector.
        /// </summary>
        void StartHashIndexCollector()
        {
            StartGenericCollector(new Collector(HashIndexCollector), snapshot, "StartHashIndexCollector", "HashIndex", HashIndexCallback, new object[] { });
        }

        /// <summary>
        /// Define the OutOfDateStats callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void HashIndexCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretHashIndex(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the HashIndex collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void HashIndexCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(HashIndexCallback), snapshot, "HashIndexCallback", "HashIndex",
                            sender, e);
        }

        private void InterpretHashIndex(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretHashIndex"))
            {
                try
                {
                    snapshot.HashIndexInfo.Columns.Add("DatabaseName", typeof(string));
                    snapshot.HashIndexInfo.Columns.Add("SchemaName", typeof(string));
                    snapshot.HashIndexInfo.Columns.Add("TableName", typeof(string));

                    snapshot.HashIndexInfo.Columns.Add("IndexName", typeof(string));
                    snapshot.HashIndexInfo.Columns.Add("total_bucket_count", typeof(long));
                    snapshot.HashIndexInfo.Columns.Add("empty_bucket_count", typeof(long));

                    snapshot.HashIndexInfo.Columns.Add("EmptyBucketPercent", typeof(double));
                    snapshot.HashIndexInfo.Columns.Add("avg_chain_length", typeof(long));
                    snapshot.HashIndexInfo.Columns.Add("max_chain_length", typeof(long));

                    //For Scanned Hash Table I27

                    snapshot.ScannedHashIndexInfo.Columns.Add("DatabaseName", typeof(string));
                    snapshot.ScannedHashIndexInfo.Columns.Add("SchemaName", typeof(string));
                    snapshot.ScannedHashIndexInfo.Columns.Add("TableName", typeof(string));

                    snapshot.ScannedHashIndexInfo.Columns.Add("IndexName", typeof(string));
                    if(datareader.Read())
                    {
                        if (!datareader.IsDBNull(0))
                        {
                            snapshot.Edition = datareader.GetString(0);
                        }
                    }
                    if (snapshot.Edition.ToLower().Contains("enterprise"))
                    {
                        datareader.NextResult();
                        while (datareader.Read())
                        {
                            snapshot.HashIndexInfo.Rows.Add(
                                ProbeHelpers.ToString(datareader, "DatabaseName"),
                                ProbeHelpers.ToString(datareader, "SchemaName"),
                                ProbeHelpers.ToString(datareader, "TableName"),

                                ProbeHelpers.ToString(datareader, "IndexName"),
                                ProbeHelpers.ToInt64(datareader, "total_bucket_count"),
                                ProbeHelpers.ToInt64(datareader, "empty_bucket_count"),

                                ProbeHelpers.ToDouble(datareader, "EmptyBucketPercent"),
                                ProbeHelpers.ToInt64(datareader, "avg_chain_length"),
                                ProbeHelpers.ToInt64(datareader, "max_chain_length")
                                );
                        }
                        datareader.NextResult();
                        while (datareader.Read())
                        {
                            snapshot.ScannedHashIndexInfo.Rows.Add(ProbeHelpers.ToString(datareader, "dbname"),
                                ProbeHelpers.ToString(datareader, "SchemaName"),
                                ProbeHelpers.ToString(datareader, "TableName"),
                                ProbeHelpers.ToString(datareader, "IndexName")
                                );
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting HashIndex Collector: {0}",
                                                        e,
                                                        false);
                    GenericFailureDelegate(snapshot);
                }
            }
        }


        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
