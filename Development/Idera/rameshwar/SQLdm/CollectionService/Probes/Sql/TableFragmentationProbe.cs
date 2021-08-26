//------------------------------------------------------------------------------
// <copyright file="TableFragmentationProbe.cs" company="Idera, Inc.">
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

    internal sealed class TableFragmentationProbe : SqlBaseProbe
    {
        #region fields

        private TableFragmentationSnapshot refresh = null;
        private TableFragmentationConfiguration config = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TableFragmentationProbe"/> class.
        /// </summary>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public TableFragmentationProbe(SqlConnectionInfo connectionInfo, TableFragmentationConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("TableFragmentationProbe:" + connectionInfo.InstanceName);
            this.cloudProviderId = cloudProviderId;
            refresh = new TableFragmentationSnapshot(connectionInfo);
            this.config = config;
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
                StartTableFragmentationWorkloadCollector();
            }
            else
            {
                FireCompletion(refresh, Result.Success);
            }
        }

        #region fragmentation workload


        /// <summary>
        /// Define the TableFragmentationWorkload collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void TableFragmentationWorkloadCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {

            try
            {
                if (config.CollectorStatus < TableFragmentationCollectorStatus.Running)
                {
                    if (config.CollectorStatus < TableFragmentationCollectorStatus.Starting)
                    {
                        config.PresentFragmentationStatisticsRunTime = refresh.TimeStampLocal;
                        config.PreviousFragmentationStatisticsRunTime = config.LastFragmentationStatisticsRunTime;
                        config.CollectorStatus = TableFragmentationCollectorStatus.Starting;
                    }
                    else if (config.CollectorStatus == TableFragmentationCollectorStatus.Starting)
                    {
                        config.CollectorStatus = TableFragmentationCollectorStatus.Running;
                    }
                }
                SqlCommand cmd =
                   SqlCommandBuilder.BuildTableFragmentationWorkloadCommand(conn,
                                                             ver,
                                                             config, cloudProviderId);
                sdtCollector = new SqlCollector(cmd, true);
                sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(TableFragmentationWorkloadCallback));
            }
            catch (Exception ex)
            {
                LOG.Error(string.Format("Caught exception in TableFragmentationWorkloadCollector.  conn is null: {0}, stdCollector is null: {1}, ver is null: {2}", conn == null, sdtCollector == null, ver == null), ex);
            }

        }

        /// <summary>
        /// Starts the Table FragmentationWorkload collector.
        /// </summary>
        private void StartTableFragmentationWorkloadCollector()
        {
            StartGenericCollector(new Collector(TableFragmentationWorkloadCollector), refresh, "StartTableFragmentationWorkloadCollector", "Table FragmentationWorkload", TableFragmentationWorkloadCallback, new object[] { });
        }

        /// <summary>
        /// Define the TableFragmentationWorkload callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void TableFragmentationWorkloadCallback(CollectorCompleteEventArgs e)
        {
            // No work to do here
            StartTableFragmentationCollector();
        }

        /// <summary>
        /// Callback used to process the data returned from the TableFragmentationWorkload collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void TableFragmentationWorkloadCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(TableFragmentationWorkloadCallback),
                            refresh,
                            "TableFragmentationWorkloadCallback",
                            "Table FragmentationWorkload",
                            sender,
                            e);
        }

        #endregion

        /// <summary>
        /// Define the TableFragmentation collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void TableFragmentationCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {

            try
            {
                SqlCommand cmd =
           SqlCommandBuilder.BuildTableFragmentationCommand(conn,
                                                     ver,
                                                     config, cloudProviderId);
                sdtCollector = new SqlCollector(cmd, true);
                sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(TableFragmentationCallback));
            }
            catch (Exception ex)
            {
                LOG.Error(string.Format("Caught exception in TableFragmentationCollector.  conn is null: {0}, stdCollector is null: {1}, ver is null: {2}", conn == null, sdtCollector == null, ver == null), ex);
            }

        }

        /// <summary>
        /// Starts the Table Fragmentation collector.
        /// </summary>
        private void StartTableFragmentationCollector()
        {
            if (refresh.ProductVersion.Major > 8)
            {
                //Additional permissions for Fragmentation2005
                SqlConnection connection = OpenConnection();
                var sqlCommand = SqlCommandBuilder.BuildFragmentationPermissionsCommand(connection,
                    new ServerVersion(connection.ServerVersion), config, cloudProviderId);

                StartGenericCollector(new Collector(TableFragmentationCollector), refresh, "StartTableFragmentationCollector", "Table Fragmentation", TableFragmentationCallback, new object[] { sqlCommand });
            }
            else
            {
                StartGenericCollector(new Collector(TableFragmentationCollector), refresh, "StartTableFragmentationCollector", "Table Fragmentation", null, new object[] { });
            }
        }

        /// <summary>
        /// Define the TableFragmentation callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void TableFragmentationCallback(CollectorCompleteEventArgs e)
        {
            try
            {
                config.LastFragmentationStatisticsRunTime = refresh.TimeStampLocal;
                using (SqlDataReader rd = e.Value as SqlDataReader)
                {
                    InterpretTableFragmentation(rd);
                }
            }
            catch (Exception ex)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh,
                                                    LOG,
                                                    "Error in Table Fragmentation callback: {0}",
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
        /// Callback used to process the data returned from the TableFragmentation collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void TableFragmentationCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(TableFragmentationCallback),
                            refresh,
                            "TableFragmentationCallback",
                            "Table Fragmentation",
                            sender,
                            e);
        }

        /// <summary>
        /// Interpret TableFragmentation data
        /// </summary>
        private void InterpretTableFragmentation(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretTableFragmentation"))
            {
                try
                {
                    string dbName = null;
                    string tableName = null;
                    string schemaName = null;
                    bool isSystemTable;
                    
                    do
                    {
                        while (dataReader.Read())
                        {
                            if (dataReader.FieldCount == 1)
                            {
                                string message = "";
                                if (!dataReader.IsDBNull(0))
                                    message = dataReader.GetString(0);
                                if (!String.IsNullOrEmpty(message))
                                {
                                    if (message == "Complete")
                                    {
                                        config.ForceFinish = true;

                                        // Do not need to wind down through Stopping if this was started and stopped in the same refresh
                                        config.CollectorStatus =
                                            config.CollectorStatus ==
                                            TableFragmentationCollectorStatus.Running
                                                ? TableFragmentationCollectorStatus.Stopping
                                                : TableFragmentationCollectorStatus.Stopped;
                                    }
                                    else
                                    {
                                        LOG.Warn("Message returned by Fragmentation Collector: " + message);
                                    }
                                }
                                continue;
                            }
                            if (dataReader.FieldCount == 4) // SQL 2005+
                            {
                                if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(1) && !dataReader.IsDBNull(2))
                                {
                                    dbName = dataReader.IsDBNull(0) ? null : dataReader.GetString(0).TrimEnd();
                                    tableName = dataReader.IsDBNull(1) ? null : dataReader.GetString(1);
                                    schemaName = dataReader.IsDBNull(2) ? null : dataReader.GetString(2);
                                    isSystemTable = false;

                                    //Add the database to our dictionary if we haven't already);)
                                    if (!refresh.DbStatistics.ContainsKey(dbName))
                                    {
                                        refresh.DbStatistics.Add(dbName,
                                                                 new DatabaseStatistics(refresh.ServerName, dbName));
                                    }


                                    //Add the database to our dictionary if we haven't already
                                    if (!refresh.DbStatistics[dbName].TableReorganizations.ContainsKey(tableName))
                                    {
                                        refresh.DbStatistics[dbName].TableReorganizations.Add(tableName,
                                                                                    new TableReorganization(refresh.ServerName,
                                                                                                  dbName,
                                                                                                  tableName, schemaName,
                                                                                                  isSystemTable));
                                    }

                                    if (!dataReader.IsDBNull(3))
                                        refresh.DbStatistics[dbName].TableReorganizations[tableName].LogicalFragmentation =
                                            dataReader.GetDouble(3);


                                    

                                }
                                //error
                            }
                            else
                            {
                                if (refresh.ProductVersion.Major == 8)  // SQL 2000
                                {
                                    if (dataReader.FieldCount == 2 && !dataReader.IsDBNull(0) && !dataReader.IsDBNull(1))
                                    {
                                        dbName = dataReader.GetString(0);
                                        schemaName = dataReader.GetString(1);
                                    }
                                    else
                                    {
                                        if (!dataReader.IsDBNull(0) && dbName != null && schemaName != null)
                                        {
                                            tableName = dataReader.GetString(0);

                                            isSystemTable = false;

                                            //Add the database to our dictionary if we haven't already);)
                                            if (!refresh.DbStatistics.ContainsKey(dbName))
                                            {
                                                refresh.DbStatistics.Add(dbName,
                                                                         new DatabaseStatistics(refresh.ServerName,
                                                                                                dbName));
                                            }


                                            //Add the database to our dictionary if we haven't already
                                            if (
                                                !refresh.DbStatistics[dbName].TableReorganizations.ContainsKey(tableName))
                                            {
                                                refresh.DbStatistics[dbName].TableReorganizations.Add(tableName,
                                                                                                      new TableReorganization
                                                                                                          (refresh.
                                                                                                               ServerName,
                                                                                                           dbName,
                                                                                                           tableName,
                                                                                                           schemaName,
                                                                                                           isSystemTable));
                                            }


                                            refresh.DbStatistics[dbName].TableReorganizations[tableName].
                                                ScanDensity =
                                                dataReader.GetDouble(15);


                                            refresh.DbStatistics[dbName].TableReorganizations[tableName].
                                                LogicalFragmentation
                                                = dataReader.GetDouble(18);
                                        }
                                    }
                                }
                            }
                        }
                    } while (dataReader.NextResult());
                }
                catch (Exception ex)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh,
                                                    LOG,
                                                    "Error in Table Fragmentation interpreter: {0}",
                                                    ex,false);
                }
            }
        }

        #endregion

        #region interface implementations

        #endregion
    }
}
