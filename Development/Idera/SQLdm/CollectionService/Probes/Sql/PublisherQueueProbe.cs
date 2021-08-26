//------------------------------------------------------------------------------
// <copyright file="PublisherQueueProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Data;
using System.Text;
using System.Text.RegularExpressions;

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
    /// On-Demand probe for Publisher Queue
    /// </summary>
    internal sealed class PublisherQueueProbe : SqlBaseProbe
    {
        #region fields

        private PublisherQueue publisherQueue = null;
        private int numberOfRecords = 200;
        private PublisherQueueConfiguration config = null;

        private Regex commandTypeRegex = new Regex(@"(?<=.*{CALL.*sp_MS)...", RegexOptions.IgnoreCase);
        private Regex objectNameRegex = new Regex(@"(?<=.*{CALL.*sp_MS.*_).*(?=\])", RegexOptions.IgnoreCase);
       // private Regex argumentListRegex = new Regex(@"(?<=.*{.*}).*", RegexOptions.IgnoreCase);

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherQueueProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        public PublisherQueueProbe(SqlConnectionInfo connectionInfo, PublisherQueueConfiguration config)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("PublisherQueueProbe");
            publisherQueue = new PublisherQueue(connectionInfo);
            if (config != null)
            {
                numberOfRecords = config.NumberOfRecords;
            }
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
                StartReplicationStatusCollector();
            }
            else
            {
                FireCompletion(publisherQueue, Result.Success);
            }
        }

        /// <summary>
        /// Define the ReplicationStatus collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void ReplicationStatusCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildReplicationStatusCommand(conn, ver);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ReplicationStatusCallback));
        }

        /// <summary>
        /// Starts the Replication Status collector.
        /// </summary>
        private void StartReplicationStatusCollector()
        {
            StartGenericCollector(new Collector(ReplicationStatusCollector), publisherQueue, "StartReplicationStatusCollector", "Replication Status");
        }

        /// <summary>
        /// Define the ReplicationStatus callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ReplicationStatusCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretReplicationStatus(rd);
            }
            StartPublisherQueueCollector();
        }

        /// <summary>
        /// Callback used to process the data returned from the ReplicationStatus collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ReplicationStatusCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(ReplicationStatusCallback),
                            publisherQueue,
                            "ReplicationStatusCallback",
                            "Replication Status",
                            new FailureDelegate(ReplicationStatusFailureDelegate),
                            new FailureDelegate(GenericFailureDelegate),
                            sender,
                            e, true);
        }

        /// <summary>
        /// ReplicationStatus failure delegate
        /// </summary>
        /// <param name="snapshot">Snapshot to return with failure</param>
        private void ReplicationStatusFailureDelegate(Snapshot snapshot, Exception e)
        {
            StartAlternateDistributionStatusCollector();
        }

        /// <summary>
        /// Define the PublisherQueue collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void PublisherQueueCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildPublisherQueueCommand(conn, ver,numberOfRecords);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(PublisherQueueCallback));
        }

        /// <summary>
        /// Starts the Publisher Queue collector.
        /// </summary>
        private void StartPublisherQueueCollector()
        {
            if (publisherQueue.ReplicationStatus != ReplicationState.NotInstalled || publisherQueue.ReplicationStatus == ReplicationState.Unknown)
            {
                StartGenericCollector(new Collector(PublisherQueueCollector), publisherQueue,
                                      "StartPublisherQueueCollector", "Publisher Queue");
            }
            else
            {
                LOG.Info("Skipping publisher queue - replication status is " + publisherQueue.ReplicationStatus);
                FireCompletion(publisherQueue, Result.Success);
            }
        }

        /// <summary>
        /// Define the PublisherQueue callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void PublisherQueueCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretPublisherQueue(rd);
            }
            FireCompletion(publisherQueue, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the PublisherQueue collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void PublisherQueueCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(PublisherQueueCallback), publisherQueue, "PublisherQueueCallback", "Publisher Queue",
                            sender, e);
        }

        /// <summary>
        /// Define the AlternateDistributionStatus collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void AlternateDistributionStatusCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildAlternateDistributionStatusCommand(conn, ver);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(AlternateDistributionStatusCallback));
        }

        /// <summary>
        /// Starts the Alternate Replication Status collector.
        /// </summary>
        private void StartAlternateDistributionStatusCollector()
        {
            StartGenericCollector(new Collector(AlternateDistributionStatusCollector), publisherQueue, "StartAlternateDistributionStatusCollector", "Alternate Distribution Status");
        }

        /// <summary>
        /// ReplicationStatus failure delegate
        /// </summary>
        /// <param name="snapshot">Snapshot to return with failure</param>
        private void AlternateDistributionStatusFailureDelegate(Snapshot snapshot, Exception e)
        {
            StartPublisherQueueCollector();
        }

        /// <summary>
        /// Define the AlternateDistributionStatus callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void AlternateDistributionStatusCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretAlternateDistributionStatus(rd);
            }
            StartPublisherQueueCollector();
        }

        /// <summary>
        /// Callback used to process the data returned from the AlternateDistributionStatus collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void AlternateDistributionStatusCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(AlternateDistributionStatusCallback),
                           publisherQueue,
                           "AlternateDistributionStatusCallback",
                           "Alternate Distribution Status",
                           new FailureDelegate(AlternateDistributionStatusFailureDelegate),
                           new FailureDelegate(GenericFailureDelegate),
                           sender,
                           e, true);
        }


        /// <summary>
        /// Interpret ReplicationStatus data
        /// </summary>
        private void InterpretReplicationStatus(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretReplicationStatus"))
            {
                try
                {
                    if (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(1))
                            publisherQueue.ReplicationStatus =
                                (ReplicationState)dataReader.GetInt32(1);
                        dataReader.NextResult();
                    }
                    if (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(1))
                            publisherQueue.Distributor = dataReader.GetString(1);
                        if (!dataReader.IsDBNull(2))
                            publisherQueue.DistributionDatabase = dataReader.GetString(2);

                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(publisherQueue, LOG, "Error interpreting Replication Status Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(publisherQueue);
                }
            }
        }

        /// <summary>
        /// Interpret AlternateDistributionStatus data
        /// </summary>
        private void InterpretAlternateDistributionStatus(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretAlternateDistributionStatus"))
            {
                try
                {
                    if (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(0))
                            publisherQueue.ReplicationStatus = (dataReader.GetBoolean(0))
                                                                   ? ReplicationState.Installed
                                                                   : ReplicationState.NotInstalled;
                        if (!dataReader.IsDBNull(1))
                            publisherQueue.Distributor = dataReader.GetString(1);
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(publisherQueue, LOG, "Error interpreting Alternate Distribution Status Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(publisherQueue);
                }
            }
        }

        /// <summary>
        /// Interpret PublisherQueue data
        /// </summary>
        private void InterpretPublisherQueue(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretPublisherQueue"))
            {
                try
                {
                    string bufferDatabaseName = null;
                    publisherQueue.NonDistributedTransactions.BeginLoadData();
                    do
                    {
                        while (dataReader.Read())
                        {
                            if (dataReader.FieldCount == 2)
                            {
                                if (!dataReader.IsDBNull(1)) bufferDatabaseName = dataReader.GetString(1);
                            }
                            else
                            {
                                DataRow dr = publisherQueue.NonDistributedTransactions.NewRow();
                                dr["Database"] = bufferDatabaseName;

                                byte[] data = { };
                                

                                if (!dataReader.IsDBNull(2)) data = (byte[])dataReader.GetSqlBinary(2);

                                if (data.Length > 0)
                                {
                                    string rawCommand = null;
                                    StringBuilder outputCommand = new StringBuilder();
                                    rawCommand = new string(Encoding.Unicode.GetChars(data));

                                    // Isolate command 
                                    if (commandTypeRegex.IsMatch(rawCommand))
                                    {
                                        switch (commandTypeRegex.Match(rawCommand).Value.ToLower())
                                        {
                                            case "upd":
                                                outputCommand.Append("update ");
                                                break;
                                            case "del":
                                                outputCommand.Append("delete ");
                                                break;
                                            case "ins":
                                                outputCommand.Append("insert ");
                                                break;
                                        }
                                    }

                                    if (outputCommand.Length > 0)
                                    {
                                        // Isolate object name
                                        if (objectNameRegex.IsMatch(rawCommand))
                                        {
                                            outputCommand.Append(objectNameRegex.Match(rawCommand).Value + ' ');
                                        }
                                    }
                                    else
                                    {
                                        outputCommand.Append(rawCommand);
                                    }


                                    dr["Command"] = outputCommand;

                                }

                                publisherQueue.NonDistributedTransactions.Rows.Add(dr);
                            }
                        }
                    } while (dataReader.NextResult());
                    publisherQueue.NonDistributedTransactions.EndLoadData();
                }
                catch (SqlException sqle)
                {
                    switch (sqle.Number)
                    {
                        case 18752:
                            ProbeHelpers.LogAndAttachToSnapshot(publisherQueue, LOG, "Error executing the Publisher Queue Collector:  Another connection is locking the required procedures.  SQL Server does not allow two connections to execute log-related procedures simultaneously.  This may occur if another copy of SQLdm is already monitoring this view.  Please close the other view or try again later.", sqle, false);
                            break;
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(publisherQueue, LOG, "Error interpreting Publisher Queue Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(publisherQueue);
                }
            }
        }
      

        #endregion

        #region interface implementations

        #endregion
    }
}
