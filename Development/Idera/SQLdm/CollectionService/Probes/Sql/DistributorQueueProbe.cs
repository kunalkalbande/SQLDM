//------------------------------------------------------------------------------
// <copyright file="DistributorQueueProbe.cs" company="Idera, Inc.">
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
    /// Distributor queue probe
    /// </summary>
    internal sealed class DistributorQueueProbe : SqlBaseProbe
    {
        #region fields

        private DistributorQueue distributorQueue = null;
        private DistributorQueueConfiguration config = null;

        private Regex commandTypeRegex = new Regex(@"(?<=.*{CALL.*sp_MS)...", RegexOptions.IgnoreCase);
        private Regex objectNameRegex = new Regex(@"(?<=.*{CALL.*sp_MS.*_).*(?=\])", RegexOptions.IgnoreCase);
        //private Regex argumentListRegex = new Regex(@"(?<=.*{.*}).*", RegexOptions.IgnoreCase);


        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributorQueueProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public DistributorQueueProbe(SqlConnectionInfo connectionInfo, DistributorQueueConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("DistributorQueueProbe");
            distributorQueue = new DistributorQueue(connectionInfo);
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
                StartDistributorQueueCollector();
            }
            else
            {
                FireCompletion(distributorQueue, Result.Success);
            }
        }

        /// <summary>
        /// Define the DistributorQueue collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void DistributorQueueCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildDistributorQueueCommand(conn, ver, config);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DistributorQueueCallback));
        }

        /// <summary>
        /// Starts the Distributor Queue collector.
        /// </summary>
        private void StartDistributorQueueCollector()
        {
            StartGenericCollector(new Collector(DistributorQueueCollector), distributorQueue, "StartDistributorQueueCollector", "Distributor Queue", null, new object[] { });
        }

        /// <summary>
        /// Define the DistributorQueue callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DistributorQueueCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretDistributorQueue(rd);
            }
            FireCompletion(distributorQueue, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the DistributorQueue collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DistributorQueueCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(DistributorQueueCallback), distributorQueue, "DistributorQueueCallback", "Distributor Queue",
                            sender, e);
        }

        /// <summary>
        /// Interpret DistributorQueue data
        /// </summary>
        private void InterpretDistributorQueue(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretDistributorQueue"))
            {
                try
                {
                    // If the fieldcount is 1, we were unable to find the distributor
                    if (dataReader.FieldCount == 1)
                    {
                        distributorQueue.SetError("The distributor database [" + config.DistributionDatabase +
                                                 "] does not exist on the distribution server " +
                                                 distributorQueue.ServerName, null);
                    }
                    else
                    {
                        distributorQueue.NonSubscribedTransactions.BeginLoadData();
                        while (dataReader.Read())
                        {
                            if (distributorQueue.PublisherServer == null)
                            {
                                if (!dataReader.IsDBNull(1)) distributorQueue.PublisherServer = dataReader.GetString(1);
                            }

                            DataRow dr = distributorQueue.NonSubscribedTransactions.NewRow();
                            if (!dataReader.IsDBNull(0)) dr["Subscriber"] = dataReader.GetString(0);
                            if (!dataReader.IsDBNull(2)) dr["PublisherDatabase"] = dataReader.GetString(2);
                            if (!dataReader.IsDBNull(3)) dr["EntryTime"] = dataReader.GetDateTime(3);

                            byte[] data = { };
                            if (!dataReader.IsDBNull(4)) data = (byte[])dataReader.GetSqlBinary(4);

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


                            distributorQueue.NonSubscribedTransactions.Rows.Add(dr);
                        }

                        distributorQueue.NonSubscribedTransactions.EndLoadData();
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(distributorQueue, LOG, "Error interpreting Distributor Queue Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(distributorQueue);
                }
            }
        }

        #endregion

        #region interface implementations

        #endregion
    }
}
