//------------------------------------------------------------------------------
// <copyright file="FilteredColumnNotInKeyOfFilteredIndexProbe.cs" company="Idera, Inc.">
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
    /// //SQLdm 10.0 (srishti purohit) (Embedded SQLDoctor Analytics) - New Probe class
    /// </summary>
    internal class FilteredColumnNotInKeyOfFilteredIndexProbe : SqlBaseProbe
    {
        #region fields

        private FilteredColumnNotInKeyOfFilteredIndexSnapshot snapshot = null;
        private string db;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FilteredColumnNotInKeyOfFilteredIndexProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public FilteredColumnNotInKeyOfFilteredIndexProbe(SqlConnectionInfo connectionInfo, string DB, int? cloudProviderId)
            : base(connectionInfo)
        {
            db = DB;
            snapshot = new FilteredColumnNotInKeyOfFilteredIndexSnapshot(connectionInfo);
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
            StartFilteredColumnNotInKeyOfFilteredIndexCollector();
        }

        /// <summary>
        /// Define the FilteredColumnNotInKeyOfFilteredIndex collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void FilteredColumnNotInKeyOfFilteredIndexCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildFilteredColumnNotInKeyOfFilteredIndexCommand(conn, ver, db);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(FilteredColumnNotInKeyOfFilteredIndexCallback));
        }

        /// <summary>
        /// Starts the Configuration collector.
        /// </summary>
        void StartFilteredColumnNotInKeyOfFilteredIndexCollector()
        {
            StartGenericCollector(new Collector(FilteredColumnNotInKeyOfFilteredIndexCollector), snapshot, "StartFilteredColumnNotInKeyOfFilteredIndexCollector", "FilteredColumnNotInKeyOfFilteredIndex", FilteredColumnNotInKeyOfFilteredIndexCallback, new object[] { });
        }

        /// <summary>
        /// Define the FilteredColumnNotInKeyOfFilteredIndex callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void FilteredColumnNotInKeyOfFilteredIndexCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretFilteredColumnNotInKeyOfFilteredIndex(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the FilteredColumnNotInKeyOfFilteredIndex collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void FilteredColumnNotInKeyOfFilteredIndexCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(FilteredColumnNotInKeyOfFilteredIndexCallback), snapshot, "FilteredColumnNotInKeyOfFilteredIndexCallback", "FilteredColumnNotInKeyOfFilteredIndex",
                            sender, e);
        }

        private void InterpretFilteredColumnNotInKeyOfFilteredIndex(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretFilteredColumnNotInKeyOfFilteredIndex"))
            {
                try
                {
                    snapshot.FilteredColumnNotInKeyOfFilteredIndexInfo.Columns.Add("dbname", typeof(string));
                    snapshot.FilteredColumnNotInKeyOfFilteredIndexInfo.Columns.Add("SchemaName", typeof(string));
                    snapshot.FilteredColumnNotInKeyOfFilteredIndexInfo.Columns.Add("TableName", typeof(string));

                    snapshot.FilteredColumnNotInKeyOfFilteredIndexInfo.Columns.Add("IndexName", typeof(string));

                    while (datareader.Read())
                    {
                        snapshot.FilteredColumnNotInKeyOfFilteredIndexInfo.Rows.Add(
                            ProbeHelpers.ToString(datareader, "dbname"),
                            ProbeHelpers.ToString(datareader, "SchemaName"),
                            ProbeHelpers.ToString(datareader, "TableName"),

                            ProbeHelpers.ToString(datareader, "IndexName")
                            );
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting FilteredColumnNotInKeyOfFilteredIndex Collector: {0}",
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
