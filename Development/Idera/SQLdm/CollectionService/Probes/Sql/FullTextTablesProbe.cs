//------------------------------------------------------------------------------
// <copyright file="FullTextTablesProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Data;

namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    using System;
    using System.Data.SqlClient;
    using BBS.TracerX;
    using Collectors;
    using Common;
    using Common.Configuration;
    using Common.Snapshots;
    using Common.Services;


    /// <summary>
    /// Full Text Tables on-demand probe
    /// </summary>
    internal sealed class FullTextTablesProbe : SqlBaseProbe
    {
        #region fields

        private FullTextTables fullTextTables = null;
        private FullTextTablesConfiguration config = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FullTextTablesProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public FullTextTablesProbe(SqlConnectionInfo connectionInfo, FullTextTablesConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("FullTextTablesProbe");
            fullTextTables = new FullTextTables(connectionInfo);
            this.config = config;
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
            if (config != null && config.ReadyForCollection)
            {
                StartFullTextTablesCollector();
            }
            else
            {
                FireCompletion(fullTextTables, Result.Success);
            }
        }

        /// <summary>
        /// Define the FullTextTables collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void FullTextTablesCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildFullTextTablesCommand(conn, ver, config);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(FullTextTablesCallback));
        }

        /// <summary>
        /// Starts the Full Text Tables collector.
        /// </summary>
        private void StartFullTextTablesCollector()
        {
            StartGenericCollector(new Collector(FullTextTablesCollector), fullTextTables, "StartFullTextTablesCollector", "Full Text Tables", FullTextTablesCallback, new object[] { });
        }

        /// <summary>
        /// Define the FullTextTables callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void FullTextTablesCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretFullTextTables(rd);
            }
            FireCompletion(fullTextTables, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the FullTextTables collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void FullTextTablesCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(FullTextTablesCallback), fullTextTables, "FullTextTablesCallback", "Full Text Tables",
                            sender, e);
        }

        /// <summary>
        /// Interpret FullTextTables data
        /// </summary>
        private void InterpretFullTextTables(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretFullTextTables"))
            {
                try
                {
                    fullTextTables.Tables.BeginLoadData();
                    do {
                    while (dataReader.Read())
                        {
                            DataRow dr = fullTextTables.Tables.NewRow();
                            for (int i = 0; i < dataReader.FieldCount; i++)
                            {
                                if (!dataReader.IsDBNull(i))
                                {
                                    dr[i] = dataReader.GetValue(i);
                                }
                            }
                            fullTextTables.Tables.Rows.Add(dr);
                        }
                    } while (dataReader.NextResult());
                    fullTextTables.Tables.EndLoadData();
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(fullTextTables, LOG, "Error interpreting Full Text Tables Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(fullTextTables);
                }
            }
        }
        #endregion

        #region interface implementations

        #endregion
    }
}
