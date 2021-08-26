//------------------------------------------------------------------------------
// <copyright file="OverlappingIndexesProbe.cs" company="Idera, Inc.">
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
    /// //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) - New Probe class
    /// </summary>
    internal class OverlappingIndexesProbe : SqlBaseProbe
    {
        #region fields

        private OverlappingIndexesSnapshot snapshot = null;
        private string db;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OverlappingIndexesProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public OverlappingIndexesProbe(SqlConnectionInfo connectionInfo, string DB, int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            db = DB;
            snapshot = new OverlappingIndexesSnapshot(connectionInfo);
            //snapshot = new SnapshotList<OverlappingIndexesSnapshot>();
            LOG = Logger.GetLogger("SessionsProbe");
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
            StartOverlappingIndexesCollector();
        }

        /// <summary>
        /// Define the OverlappingIndexes collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void OverlappingIndexesCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildOverlappingIndexesCommand(conn, ver, db);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(OverlappingIndexesCallback));
        }

        /// <summary>
        /// Starts the OverlappingIndexes collector.
        /// </summary>
        void StartOverlappingIndexesCollector()
        {
            StartGenericCollector(new Collector(OverlappingIndexesCollector), snapshot, "StartOverlappingIndexesCollector", "OverlappingIndexes", OverlappingIndexesCallback, new object[] { });
        }

        /// <summary>
        /// Define the OverlappingIndexes callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void OverlappingIndexesCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretOverlappingIndexes(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the OverlappingIndexes collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void OverlappingIndexesCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(OverlappingIndexesCallback), snapshot, "OverlappingIndexesCallback", "OverlappingIndexes",
                            sender, e);
        }

        private void InterpretOverlappingIndexes(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretOverlappingIndexes"))
            {
                try
                {
                    while (datareader.HasRows)
                    {
                        //OverlappingIndexesSnapshot snap = new OverlappingIndexesSnapshot(connectionInfo);
                        while (datareader.Read())
                        {
                            snapshot.DuplicateIndexInfo.Rows.Add(
                                ProbeHelpers.ToString(datareader, "DupIndex"), //
                                ProbeHelpers.ToString(datareader, "DatabaseName"),
                                ProbeHelpers.ToString(datareader, "Schema"),

                                ProbeHelpers.ToString(datareader, "TableName"),
                                ProbeHelpers.ToString(datareader, "IndexName"),
                                ProbeHelpers.ToInt32(datareader, "IndexId"),//

                                ProbeHelpers.ToBoolean(datareader, "IndexUnique"),
                                ProbeHelpers.ToBoolean(datareader, "IndexPrimaryKey"),
                                ProbeHelpers.ToInt64(datareader, "IndexUsage"),

                                ProbeHelpers.ToInt64(datareader, "IndexUpdates"),
                                ProbeHelpers.ToInt32(datareader, "IndexKeySize"),//
                                ProbeHelpers.ToInt32(datareader, "IndexForeignKeys"),//

                                ProbeHelpers.ToString(datareader, "DupIndexName"),
                                ProbeHelpers.ToInt32(datareader, "DupIndexId"),//
                                ProbeHelpers.ToBoolean(datareader, "DupIndexUnique"),

                                ProbeHelpers.ToBoolean(datareader, "DupIndexPrimaryKey"),
                                ProbeHelpers.ToInt64(datareader, "DupIndexUsage"),
                                ProbeHelpers.ToInt64(datareader, "DupIndexUpdates"),

                                ProbeHelpers.ToInt32(datareader, "DupIndexKeySize"),//
                                ProbeHelpers.ToInt32(datareader, "DupIndexForeignKeys")//
                            );
                        }

                        if (datareader.NextResult())
                        {
                            while (datareader.Read())
                            {
                                snapshot.PartialDuplicateIndexInfo.Rows.Add(
                                    ProbeHelpers.ToString(datareader, "DupIndex"),//
                                    ProbeHelpers.ToString(datareader, "DatabaseName"),
                                    ProbeHelpers.ToString(datareader, "Schema"),

                                    ProbeHelpers.ToString(datareader, "TableName"),
                                    ProbeHelpers.ToString(datareader, "IndexName"),
                                    ProbeHelpers.ToInt32(datareader, "IndexId"),//

                                    ProbeHelpers.ToInt64(datareader, "IndexUsage"),
                                    ProbeHelpers.ToInt64(datareader, "IndexUpdates"),
                                    ProbeHelpers.ToInt64(datareader, "IndexKeySize"),

                                    ProbeHelpers.ToBoolean(datareader, "IndexUnique"),
                                    ProbeHelpers.ToBoolean(datareader, "IndexPrimaryKey"),
                                    ProbeHelpers.ToInt32(datareader, "IndexForeignKeys"),//

                                    ProbeHelpers.ToString(datareader, "IndexIncludeCols"),
                                    ProbeHelpers.ToString(datareader, "IndexCols"),

                                    ProbeHelpers.ToString(datareader, "DupIndexName"),
                                    ProbeHelpers.ToInt64(datareader, "DupIndexUsage"),
                                    ProbeHelpers.ToInt64(datareader, "DupIndexUpdates"),

                                    ProbeHelpers.ToInt32(datareader, "DupIndexId"),//
                                    ProbeHelpers.ToBoolean(datareader, "DupIndexUnique"),
                                    ProbeHelpers.ToBoolean(datareader, "DupIndexPrimaryKey"),

                                    ProbeHelpers.ToInt64(datareader, "DupIndexKeySize"),
                                    ProbeHelpers.ToInt32(datareader, "DupIndexForeignKeys"),//

                                    ProbeHelpers.ToString(datareader, "DupIndexIncludeCols"),
                                    ProbeHelpers.ToString(datareader, "DupIndexCols")
                                    );
                            }
                        }
                        datareader.NextResult();
                        //snapshot.ListItems.Add(snap);
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting OverlappingIndexes Collector: {0}",
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
