//------------------------------------------------------------------------------
// <copyright file="WorstFillFactorIndexesProbe.cs" company="Idera, Inc.">
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
    internal class WorstFillFactorIndexesProbe : SqlBaseProbe
    {
        #region fields

        private WorstFillFactorIndexesSnapshot snapshot = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorstFillFactorIndexesProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public WorstFillFactorIndexesProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            snapshot = new WorstFillFactorIndexesSnapshot(connectionInfo);
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
            StartWorstFillFactorIndexesCollector();
        }

        /// <summary>
        /// Define the WorstFillFactorIndexes collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void WorstFillFactorIndexesCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildGetWorstFillFactorIndexesCommand(conn, ver, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(WorstFillFactorIndexesCallback));
        }

        /// <summary>
        /// Starts the WorstFillFactorIndexes collector.
        /// </summary>
        void StartWorstFillFactorIndexesCollector()
        {
            StartGenericCollector(new Collector(WorstFillFactorIndexesCollector), snapshot, "StartWorstFillFactorIndexesCollector", "WorstFillFactorIndexes", WorstFillFactorIndexesCallback, new object[] { });
        }

        /// <summary>
        /// Define the WorstFillFactorIndexes callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void WorstFillFactorIndexesCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretWorstFillFactorIndexes(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the WorstFillFactorIndexes collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void WorstFillFactorIndexesCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(WorstFillFactorIndexesCallback), snapshot, "WorstFillFactorIndexesCallback", "WorstFillFactorIndexes",
                            sender, e);
        }

        private void InterpretWorstFillFactorIndexes(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretWorstFillFactorIndexes"))
            {
                try
                {
                    snapshot.WorstFillFactorIndexes.Columns.Add("DatabaseName", typeof(string));
                    snapshot.WorstFillFactorIndexes.Columns.Add("TableName", typeof(string));
                    snapshot.WorstFillFactorIndexes.Columns.Add("IndexName", typeof(string));
                    snapshot.WorstFillFactorIndexes.Columns.Add("SchemaName", typeof(string));

                    snapshot.WorstFillFactorIndexes.Columns.Add("FillFactor", typeof(int));
                    snapshot.WorstFillFactorIndexes.Columns.Add("DataSizeInMB", typeof(int));
                    snapshot.WorstFillFactorIndexes.Columns.Add("IndexSizeInMB", typeof(int));

                    while (datareader.Read())
                    {
                        snapshot.WorstFillFactorIndexes.Rows.Add(
                            ProbeHelpers.ToString(datareader,"database_name"),
                            ProbeHelpers.ToString(datareader,"table_name"),
                            ProbeHelpers.ToString(datareader,"index_name"),
                            ProbeHelpers.ToString(datareader,"schema_name"),

                            ProbeHelpers.ToInt32(datareader,"fillfactor"),
                            ProbeHelpers.ToInt32(datareader,"datasizeinmb"),
                            ProbeHelpers.ToInt32(datareader,"indexsizeinmb")
                        );
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting WorstFillFactorIndexes Collector: {0}",
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
