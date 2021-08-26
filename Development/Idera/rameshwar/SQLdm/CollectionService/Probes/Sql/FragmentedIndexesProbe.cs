//------------------------------------------------------------------------------
// <copyright file="FragmentedIndexesProbe.cs" company="Idera, Inc.">
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
    internal class FragmentedIndexesProbe : SqlBaseProbe
    {
        #region fields

        private FragmentedIndexesSnapshot snapshot = null;
        private string db;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FragmentedIndexesProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public FragmentedIndexesProbe(SqlConnectionInfo connectionInfo, string DB, int? cloudProviderId)
            : base(connectionInfo)
        {
            db = DB;
            snapshot = new FragmentedIndexesSnapshot(connectionInfo);
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
            StartFragmentedIndexesCollector();
        }

        /// <summary>
        /// Define the FragmentedIndexes collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void FragmentedIndexesCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildFragmentedIndexesCommand(conn, ver, db, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(FragmentedIndexesCallback));
        }

        /// <summary>
        /// Starts the FragmentedIndexes collector.
        /// </summary>
        void StartFragmentedIndexesCollector()
        {
            StartGenericCollector(new Collector(FragmentedIndexesCollector), snapshot, "StartFragmentedIndexesCollector", "FragmentedIndexes", FragmentedIndexesCallback, new object[] { });
        }

        /// <summary>
        /// Define the FragmentedIndexes callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void FragmentedIndexesCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretFragmentedIndexes(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the FragmentedIndexes collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void FragmentedIndexesCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(FragmentedIndexesCallback), snapshot, "FragmentedIndexesCallback", "FragmentedIndexes",
                            sender, e);
        }

        private void InterpretFragmentedIndexes(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretFragmentedIndexes"))
            {
                try
                {
                    snapshot.FragmentedIndexes.Columns.Add("DatabaseName", typeof(string));
                    snapshot.FragmentedIndexes.Columns.Add("Schema", typeof(string));
                    snapshot.FragmentedIndexes.Columns.Add("TableName", typeof(string));

                    snapshot.FragmentedIndexes.Columns.Add("IndexName", typeof(string));
                    snapshot.FragmentedIndexes.Columns.Add("IndexId", typeof(int));//
                    snapshot.FragmentedIndexes.Columns.Add("Partition", typeof(int));

                    snapshot.FragmentedIndexes.Columns.Add("FragPercent", typeof(double));//
                    snapshot.FragmentedIndexes.Columns.Add("PartitionPages", typeof(long));

                    snapshot.FragmentedIndexes.Columns.Add("TablePages", typeof(long));
                    snapshot.FragmentedIndexes.Columns.Add("TotalServerBufferPages", typeof(long));

                    while (datareader.Read())
                    {

                        snapshot.FragmentedIndexes.Rows.Add(
                            ProbeHelpers.ToString(datareader, "DatabaseName"),
                            ProbeHelpers.ToString(datareader, "Schema"),
                            ProbeHelpers.ToString(datareader, "TableName"),

                            ProbeHelpers.ToString(datareader, "IndexName"),
                            ProbeHelpers.ToInt32(datareader, "IndexId"),//
                            ProbeHelpers.ToInt32(datareader, "Partition"),

                            ProbeHelpers.ToDouble(datareader, "FragPercent"),//
                            ProbeHelpers.ToInt64(datareader, "PartitionPages"),

                            ProbeHelpers.ToInt64(datareader, "TablePages"),
                            ProbeHelpers.ToInt64(datareader, "TotalServerBufferPages")
                        );
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting FragmentedIndexes Collector: {0}",
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
