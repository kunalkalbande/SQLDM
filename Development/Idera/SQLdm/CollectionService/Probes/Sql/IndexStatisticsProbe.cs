//------------------------------------------------------------------------------
// <copyright file="IndexStatisticsProbe.cs" company="Idera, Inc.">
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
    using Common.Snapshots;
    using Common.Services;


    /// <summary>
    /// Enter a description for this class
    /// </summary>
    internal sealed class IndexStatisticsProbe : SqlBaseProbe
    {
        #region fields

        private IndexStatistics indexStatistics = null;
        private IndexStatisticsConfiguration config = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexStatisticsProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public IndexStatisticsProbe(SqlConnectionInfo connectionInfo, IndexStatisticsConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("IndexStatisticsProbe");
            indexStatistics = new IndexStatistics(connectionInfo);
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
                StartIndexStatisticsCollector();
            }
            else
            {
                FireCompletion(indexStatistics, Result.Success);
            }
        }

        /// <summary>
        /// Define the IndexStatistics collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void IndexStatisticsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildIndexStatisticsCommand(conn, ver, config);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(IndexStatisticsCallback));
        }

        /// <summary>
        /// Starts the Index Statistics collector.
        /// </summary>
        private void StartIndexStatisticsCollector()
        {
            StartGenericCollector(new Collector(IndexStatisticsCollector), indexStatistics, "StartIndexStatisticsCollector", "Index Statistics", IndexStatisticsCallback, new object[] { });
        }

        /// <summary>
        /// Define the IndexStatistics callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void IndexStatisticsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretIndexStatistics(rd);
            }
            FireCompletion(indexStatistics, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the IndexStatistics collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void IndexStatisticsCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(IndexStatisticsCallback), indexStatistics, "IndexStatisticsCallback", "Index Statistics",
                            sender, e);
        }

        /// <summary>
        /// Interpret IndexStatistics data
        /// </summary>
        private void InterpretIndexStatistics(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretIndexStatistics"))
            {
                try
                {
                    if (indexStatistics.ProductVersion.Major >= 9) // Use same method for reading SQL Server 2005/2008
                    {
                        ReadIndexStatistics2005(dataReader);
                    }
                    else
                    {
                        ReadIndexStatistics2000(dataReader);
                    }
                    ReadIndexColumns(dataReader);
                    ReadIndexDataDistribution(dataReader);
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(indexStatistics, LOG, "Error interpreting Index Statistics Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(indexStatistics);
                }
            }
        }

        private void ReadIndexStatistics2000(SqlDataReader dataReader)
        {
            try
            {
                if (dataReader.Read())
                {
                    if (!dataReader.IsDBNull(2)) indexStatistics.RowsSampled = (dataReader.GetInt64(2));
                    if (!dataReader.IsDBNull(3)) indexStatistics.DistributionSteps = (dataReader.GetInt16(3));
                    if (!dataReader.IsDBNull(5)) indexStatistics.AverageKeyLength = (dataReader.GetFloat(5));
                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(indexStatistics, LOG, "Error interpreting Index Statistics: {0}", e,
                                                       false);
            }
            finally
            {
                dataReader.NextResult();
            }
        }

        private void ReadIndexStatistics2005(SqlDataReader dataReader)
        {
            try
            {
                if (dataReader.Read())
                {
                    if (!dataReader.IsDBNull(3)) indexStatistics.RowsSampled = (dataReader.GetInt64(3));
                    if (!dataReader.IsDBNull(4)) indexStatistics.DistributionSteps = (dataReader.GetInt16(4));
                    if (!dataReader.IsDBNull(6)) indexStatistics.AverageKeyLength = (dataReader.GetFloat(6));
                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(indexStatistics, LOG, "Error interpreting Index Statistics: {0}", e,
                                                       false);
            }
            finally
            {
                dataReader.NextResult();
            }
        }

        private void ReadIndexColumns(SqlDataReader dataReader)
        {
            try
            {
                while  (dataReader.Read())
                {
                    if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(1) && !dataReader.IsDBNull(2))
                        indexStatistics.IndexColumns.Add(new IndexColumn(dataReader.GetFloat(1), dataReader.GetFloat(0), dataReader.GetFloat(0) * indexStatistics.RowsSampled, dataReader.GetString(2)));
                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(indexStatistics, LOG, "Error interpreting Index Columns: {0}", e,
                                                       false);
            }
            finally
            {
                dataReader.NextResult();
            }

        }

        private void ReadIndexDataDistribution(SqlDataReader dataReader)
        {
            try
            {
                while (dataReader.Read())
                {
                    if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(2))
                    {
                        indexStatistics.DataDistribution.Add(new IndexDataDistribution(dataReader.GetValue(0).ToString(), dataReader.GetFloat(2)));
                    }
                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(indexStatistics, LOG, "Error interpreting Index Data Distribution: {0}", e,
                                                       false);
            }
            finally
            {
                dataReader.NextResult();
            }

        }

        #endregion

        #region interface implementations

        #endregion
    }
}
