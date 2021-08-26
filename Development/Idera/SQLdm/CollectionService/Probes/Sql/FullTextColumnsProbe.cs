//------------------------------------------------------------------------------
// <copyright file="FullTextColumnsProbe.cs" company="Idera, Inc.">
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
    /// Full Text Columns on-demand probe
    /// </summary>
    internal sealed class FullTextColumnsProbe : SqlBaseProbe
    {
        #region fields

        private FullTextColumns fullTextColumns = null;
        FullTextColumnsConfiguration config = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FullTextColumnsProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public FullTextColumnsProbe(SqlConnectionInfo connectionInfo, FullTextColumnsConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("FullTextColumnsProbe");
            fullTextColumns = new FullTextColumns(connectionInfo);
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
                StartFullTextColumnsCollector();
            }
            else
            {
                FireCompletion(fullTextColumns, Result.Success);
            }
        }

        /// <summary>
        /// Define the FullTextColumns collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void FullTextColumnsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildFullTextColumnsCommand(conn, ver, config);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(FullTextColumnsCallback));
        }

        /// <summary>
        /// Starts the Full Text Columns collector.
        /// </summary>
        private void StartFullTextColumnsCollector()
        {
            StartGenericCollector(new Collector(FullTextColumnsCollector), fullTextColumns, "StartFullTextColumnsCollector", "Full Text Columns", FullTextColumnsCallback, new object[] { });
        }

        /// <summary>
        /// Define the FullTextColumns callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void FullTextColumnsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretFullTextColumns(rd);
            }
            FireCompletion(fullTextColumns, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the FullTextColumns collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void FullTextColumnsCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(FullTextColumnsCallback), fullTextColumns, "FullTextColumnsCallback", "Full Text Columns",
                            sender, e);
        }

        /// <summary>
        /// Interpret FullTextColumns data
        /// </summary>
        private void InterpretFullTextColumns(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretFullTextColumns"))
            {
                try
                {
                    fullTextColumns.FtColumns.BeginLoadData();
                    do
                    {
                        while (dataReader.Read())
                        {
                            DataRow dr = fullTextColumns.FtColumns.NewRow();
                            for (int i = 0; i < dataReader.FieldCount; i++)
                            {
                                if (!dataReader.IsDBNull(i))
                                {
                                    dr[i] = dataReader.GetValue(i);
                                }
                            }
                            fullTextColumns.FtColumns.Rows.Add(dr);
                        }
                    } while (dataReader.NextResult());
                    fullTextColumns.FtColumns.EndLoadData();
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(fullTextColumns, LOG, "Error interpreting Full Text Columns Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(fullTextColumns);
                }
            }
        }

        #endregion

        #region interface implementations

        #endregion
    }
}
