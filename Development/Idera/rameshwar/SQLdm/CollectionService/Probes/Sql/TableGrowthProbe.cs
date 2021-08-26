//------------------------------------------------------------------------------
// <copyright file="TableGrowthProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Threading;

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

    internal sealed class TableGrowthProbe : SqlBaseProbe
    {
        #region fields

        private TableGrowthSnapshot refresh = null;
        private TableGrowthConfiguration config = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TableGrowthProbe"/> class.
        /// </summary>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public TableGrowthProbe(SqlConnectionInfo connectionInfo, TableGrowthConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("TableGrowthProbe:" + connectionInfo.InstanceName);
            refresh = new TableGrowthSnapshot(connectionInfo);
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
                Thread.Sleep(5000);     // This is a basic flood control method to slow down refreshes against fast local servers
                StartTableGrowthCollector();
            }
            else
            {
                FireCompletion(refresh, Result.Success);
            }
        }

        /// <summary>
        /// Define the TableGrowth collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void TableGrowthCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
           
            try
            {
                SqlCommand cmd =
           SqlCommandBuilder.BuildTableGrowthCommand(conn,
                                                     ver,
                                                     config,
                                                     cloudProviderId);
                sdtCollector = new SqlCollector(cmd, true);
                sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(TableGrowthCallback));
            }
            catch (Exception ex)
            {
                LOG.Error(string.Format("Caught exception in TableGrowthCollector.  conn is null: {0}, stdCollector is null: {1}, ver is null: {2}", conn == null, sdtCollector == null, ver == null), ex);
            }
    
        }

        /// <summary>
        /// Starts the Table Growth collector.
        /// </summary>
        private void StartTableGrowthCollector()
        {
            StartGenericCollector(new Collector(TableGrowthCollector), refresh, "StartTableGrowthCollector", "Table Growth", TableGrowthCallback, new object[] { });
        }

        /// <summary>
        /// Define the TableGrowth callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void TableGrowthCallback(CollectorCompleteEventArgs e)
        {
            try
            {
                if(!config.InRetry)
                    config.PreviousGrowthStatisticsRunTime = config.LastGrowthStatisticsRunTime;
                config.LastGrowthStatisticsRunTime = refresh.TimeStampLocal;
                using (SqlDataReader rd = e.Value as SqlDataReader)
                {
                    InterpretTableGrowth(rd);
                }
            }
            catch (Exception ex)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh,
                                                    LOG,
                                                    "Error in Table Growth callback: {0}",
                                                    ex,
                                                    false);
                GenericFailureDelegate(refresh);
            }
            finally
            {
                FireCompletion(refresh, Result.Success);
            }
        }

        /// <summary>
        /// Callback used to process the data returned from the TableGrowth collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void TableGrowthCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(TableGrowthCallback),
                            refresh,
                            "TableGrowthCallback",
                            "Table Growth",
                            sender,
                            e);
        }

        /// <summary>
        /// Interpret TableGrowth data
        /// </summary>
        private void InterpretTableGrowth(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretTableGrowth"))
            {
                try
                {
                    string dbName;
                    string tableName;
                    string schemaName;
                    bool isSystemTable;
                    config.InRetry = false;
                    int tablesRead = 0;
                    do
                    {
                        while (dataReader.Read())
                        {
                            if (dataReader.FieldCount == 10)
                            {
                                if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(1) && !dataReader.IsDBNull(7))
                                {
                                    config.InRetry = true;
                                    dbName = dataReader.IsDBNull(0) ? null : dataReader.GetString(0).TrimEnd();
                                    tableName = dataReader.IsDBNull(1) ? null : dataReader.GetString(1);
                                    schemaName = dataReader.IsDBNull(7) ? null : dataReader.GetString(7);
                                    isSystemTable = dataReader.IsDBNull(8) ? false : dataReader.GetBoolean(8);
                                    if (!dataReader.IsDBNull(9))
                                    {
                                        int dbid = dataReader.GetInt32(9);
                                        if (!config.AlreadyCollectedDatabases.Contains(dbid))
                                            config.AlreadyCollectedDatabases.Add(dbid);
                                    }


                                    if (dbName != null && tableName != null)
                                    {
                                        tablesRead++;

                                        //Add the database to our dictionary if we haven't already);)
                                        if (!refresh.DbStatistics.ContainsKey(dbName))
                                        {
                                            refresh.DbStatistics.Add(dbName,
                                                                     new DatabaseStatistics(refresh.ServerName, dbName));
                                        }


                                        //Add the database to our dictionary if we haven't already
                                        if (!refresh.DbStatistics[dbName].TableSizes.ContainsKey(string.Format("{0}.{1}",schemaName,tableName)))
                                        {
                                            refresh.DbStatistics[dbName].TableSizes.Add(string.Format("{0}.{1}",schemaName,tableName),
                                                                                        new TableSize(refresh.ServerName,
                                                                                                      dbName,
                                                                                                      tableName, schemaName,
                                                                                                      isSystemTable));
                                        }

                                        if (!dataReader.IsDBNull(2))
                                            refresh.DbStatistics[dbName].TableSizes[string.Format("{0}.{1}", schemaName, tableName)].DataSize =
                                                new FileSize((decimal) dataReader.GetValue(2));

                                        if (!dataReader.IsDBNull(3))
                                            refresh.DbStatistics[dbName].TableSizes[string.Format("{0}.{1}", schemaName, tableName)].IndexSize =
                                                new FileSize(dataReader.GetDecimal(3));

                                        if (!dataReader.IsDBNull(4))
                                            refresh.DbStatistics[dbName].TableSizes[string.Format("{0}.{1}", schemaName, tableName)].ImageTextSize =
                                                new FileSize(dataReader.GetDecimal(4));

                                        if (!dataReader.IsDBNull(5))
                                            refresh.DbStatistics[dbName].TableSizes[string.Format("{0}.{1}", schemaName, tableName)].RowCount =
                                                dataReader.GetInt64(5);
                                    }
                                }
                                //error
                            }
                            else
                            {
                                if (!dataReader.IsDBNull(0))
                                {
                                    object o = dataReader.GetValue(0);
                                    LOG.Warn("Message returned by TableGrowth: " + o.ToString());
                                    Thread.Sleep(200000);     // Slow collector down to prevent flooding
                                }
                            }
                        }
                    } while (dataReader.NextResult());


                    LOG.InfoFormat("Tables read by interpreter: {0}", tablesRead);

                    //If we're no longer retrying then clear this out)
                    if (!config.InRetry)
                        config.AlreadyCollectedDatabases.Clear();
                }
                catch 
                {
                }
            }
        }

        #endregion

        #region interface implementations

        #endregion
    }
}
