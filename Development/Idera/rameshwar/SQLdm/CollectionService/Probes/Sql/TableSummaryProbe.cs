//------------------------------------------------------------------------------
// <copyright file="TableSummaryProbe.cs" company="Idera, Inc.">
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
    /// Enter a description for this class
    /// </summary>
    internal sealed class TableSummaryProbe : SqlBaseProbe
    {
        #region fields

        private TableSummary tableSummary = null;
        private TableSummaryConfiguration config = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TableSummaryProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public TableSummaryProbe(SqlConnectionInfo connectionInfo, TableSummaryConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("TableSummaryProbe");
            tableSummary = new TableSummary(connectionInfo);
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
                    StartTableSummaryCollector();
                }
                else
                {
                    FireCompletion(tableSummary, Result.Success);
                }
            
        }

        /// <summary>
        /// Define the TableSummary collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void TableSummaryCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildTableSummaryCommand(conn, ver, config, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(TableSummaryCallback));
        }

        /// <summary>
        /// Starts the Table Summary collector.
        /// </summary>
        private void StartTableSummaryCollector()
        {
            StartGenericCollector(new Collector(TableSummaryCollector), tableSummary, "StartTableSummaryCollector", "Table Summary", TableSummaryCallback, new object[] { });
        }

        /// <summary>
        /// Define the TableSummary callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void TableSummaryCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretTableSummary(rd);
            }
            FireCompletion(tableSummary, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the TableSummary collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void TableSummaryCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(TableSummaryCallback), tableSummary, "TableSummaryCallback", "Table Summary",
                            sender, e);
        }

        /// <summary>
        /// Interpret TableSummary data
        /// </summary>
        private void InterpretTableSummary(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretTableSummary"))
            {
                try
                {
                    if (dataReader.Read())
                    {
                        object tempCompat;
                        float tempfloat;
                        if (!dataReader.IsDBNull(0))
                        {
                            tempCompat = dataReader.GetValue(0);
                            tempfloat = float.Parse(tempCompat.ToString());
                            tableSummary.CompatibilityLevel = tempfloat/10f;
                        }
                    }

                    if (tableSummary.CompatibilityLevel > 6)
                    {
                        dataReader.NextResult();
                        tableSummary.TableList.BeginLoadData();
                        while (dataReader.Read())
                        {
                            DataRow dr = tableSummary.TableList.NewRow();
                            for (int i = 0; i < dataReader.FieldCount; i++)
                            {
                                if (!dataReader.IsDBNull(i))
                                {
                                    switch (i)
                                    {
                                        case 3:
                                        case 4:
                                        case 5:
                                            dr[i] = new FileSize(dataReader.GetDecimal(i));
                                            break;
                                        default:
                                            dr[i] = dataReader.GetValue(i);
                                            break;
                                        case 14:
                                            dr[i] = dataReader.GetString(i) == "PS" ? "Partition Scheme" : "Filegroup";
                                            break;
                                    }
                                }
                            }
                            if (dataReader.FieldCount < 15)
                                dr["DataSpaceType"] = "Filegroup";
                            tableSummary.TableList.Rows.Add(dr);
                        }
                        tableSummary.TableList.EndLoadData();
                        dataReader.NextResult();
                        if (dataReader.Read())
                        {
                            if (!dataReader.IsDBNull(0))
                                tableSummary.TotalDataSize.Kilobytes = dataReader.GetDecimal(0);
                            if (!dataReader.IsDBNull(1))
                                tableSummary.TotalIndexSize.Kilobytes = dataReader.GetDecimal(1);
                            if (!dataReader.IsDBNull(2))
                                tableSummary.TotalImageSize.Kilobytes = dataReader.GetDecimal(2);
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(tableSummary, LOG, "Error interpreting Table Summary Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(tableSummary);
                }
            }
        }

        #endregion

        #region interface implementations

        #endregion
    }
}
