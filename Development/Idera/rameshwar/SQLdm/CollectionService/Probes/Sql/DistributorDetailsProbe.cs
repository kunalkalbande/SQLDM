//------------------------------------------------------------------------------
// <copyright file="DistributorDetailsProbe.cs" company="Idera, Inc.">
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
    /// Distributor Details probe
    /// </summary>
    internal sealed class DistributorDetailsProbe : SqlBaseProbe
    {
        #region fields

        private DistributorDetails distributorDetails = null;
        private DistributorDetailsConfiguration config = null;
        private DistributorQueueConfiguration queueConfig = null;

        private Regex commandTypeRegex = new Regex(@"(?<=.*{CALL.*sp_MS)...", RegexOptions.IgnoreCase);
        private Regex objectNameRegex = new Regex(@"(?<=.*{CALL.*sp_MS.*_).*(?=\])", RegexOptions.IgnoreCase);
        private Regex argumentListRegex = new Regex(@"(?<=.*{.*}).*", RegexOptions.IgnoreCase);

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributorDetailsProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="config">config object</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public DistributorDetailsProbe(SqlConnectionInfo connectionInfo, DistributorDetailsConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("DistributorDetailsProbe");
            distributorDetails = new DistributorDetails(connectionInfo);
            distributorDetails.DefaultSubscriptionDatabase = config.SubscriptionDatabase;
            distributorDetails.SelectedReplicationType = config.ReplicationType;
            distributorDetails.SubscriptionServer = config.SubscriptionServer;
            this.config = config;
            this.cloudProviderId = cloudProviderId;
            this.queueConfig = new DistributorQueueConfiguration(config.MonitoredServerId, 
                config.DistributionDatabase, 
                config.PublisherName, 
                config.Publication,
                config.SubscriptionServer,
                config.SubscriptionDatabase);
            this.queueConfig.ClientSessionId = config.ClientSessionId;
            this.queueConfig.FilterTimeSpan = config.FilterTimeSpan;
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
                StartDistributorDetailsCollector();
            }
            else
            {
                FireCompletion(distributorDetails, Result.Success);
            }
        }

         ///<summary>
         ///Define the DistributorQueue collector
         ///</summary>
         ///<param name="conn">Open SQL connection</param>
         ///<param name="sdtCollector">Standard SQL collector</param>
         ///<param name="ver">Server version</param>
        private void DistributorQueueCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildDistributorQueueCommand(conn, ver, queueConfig);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DistributorQueueCallback));
        }

        /// <summary>
        /// Define the Distributor Details collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void DistributorDetailsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildDistributorDetailsCommand(conn, ver, config);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DistributorDetailsCallback));
        }

        /// <summary>
        /// Starts the Distributor Details collector.
        /// </summary>
        private void StartDistributorDetailsCollector()
        {
            SqlCommand permissionsCommand = null;
            if (config.ReplicationType == Common.Objects.Replication.ReplicationType.Snapshot ||
                config.ReplicationType == Common.Objects.Replication.ReplicationType.Transaction)
            {
                var connection = OpenConnection();
                permissionsCommand =
                    SqlCommandBuilder.BuildDistributorDetailsPermissionsCommand(connection,
                        new ServerVersion(connection.ServerVersion), config);
            }

            StartGenericCollector(new Collector(DistributorDetailsCollector), distributorDetails, "StartDistributorDetailsCollector", "Distributor Details", DistributorDetailsCallback, new object[] { permissionsCommand });
        }
        /// <summary>
        /// Starts the Distributor Queue collector.
        /// </summary>
        private void StartDistributorQueueCollector()
        {
            StartGenericCollector(new Collector(DistributorQueueCollector), distributorDetails, "StartDistributorQueueCollector", "Distributor Queue", DistributorQueueCallback, new object[] { });
        }
        /// <summary>
        /// Define the DistributorQueue callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DistributorDetailsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretDistributorDetails(rd);
            }
            bool blnGetQueue = false;
            if(config.GetQueue.HasValue) blnGetQueue = config.GetQueue.Value;

            if (blnGetQueue)
            {
                StartDistributorQueueCollector();
            }
            else
            {
                FireCompletion(distributorDetails, Result.Success);
            }
            
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
            FireCompletion(distributorDetails, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the DistributorQueue collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DistributorQueueCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(DistributorQueueCallback), distributorDetails, "DistributorQueueCallback", "Distributor Queue",
                            sender, e);
        }

        /// <summary>
        /// Callback used to process the data returned from the DistributorDetails collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DistributorDetailsCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(DistributorDetailsCallback), distributorDetails, "DistributorDetailsCallback", "Distributor Details",
                            sender, e);
        }

        /// <summary>
        /// Interpret Distributor Details data
        /// </summary>
        private void InterpretDistributorDetails(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretDistributorDetails"))
            {
                try
                {
                    
                    distributorDetails.InitializeDistributorDetails(config.ReplicationType == Idera.SQLdm.Common.Objects.Replication.ReplicationType.Merge);

                    distributorDetails.DistributionDetails.BeginLoadData();
                    
                    while (dataReader.Read())
                    {
                        DataRow dr = distributorDetails.DistributionDetails.NewRow();
                        #region datatable
                        //distributionDetails.Columns.Add("Articles", typeof(int));
                        //distributionDetails.Columns.Add("PublisherDBID", typeof(int));
                        //distributionDetails.Columns.Add("PublisherID", typeof(int));
                        //distributionDetails.Columns.Add("Publisher", typeof(string));
                        //distributionDetails.Columns.Add("Distributor", typeof(string));
                        //distributionDetails.Columns.Add("Subscriber", typeof(string));
                        //distributionDetails.Columns.Add("SubscriptionType", typeof(string));
                        //distributionDetails.Columns.Add("Sync Type", typeof(string));
                        //distributionDetails.Columns.Add("Publication", typeof(string));
                        //distributionDetails.Columns.Add("SubscriberID", typeof(int));
                        //distributionDetails.Columns.Add("AgentID", typeof(int));
                        //distributionDetails.Columns.Add("Subscribed", typeof(int));
                        //distributionDetails.Columns.Add("Non-Subscribed", typeof(int));
                        //distributionDetails.Columns.Add("SubscriptionLatency", typeof(int));
                        //distributionDetails.Columns.Add("SubscriptionRate", typeof(string));
                        //distributionDetails.Columns.Add("CatchUpTime", typeof(DateTime));
                        //distributionDetails.Columns.Add("SampleTime", typeof(DateTime));


                        //distributionDetails.Columns.Add("SubscriberName", typeof(string));
                        //distributionDetails.Columns.Add("MergeSubscriptionstatus", typeof(string));
                        //distributionDetails.Columns.Add("SubscriberDB", typeof(string));
                        //distributionDetails.Columns.Add("Type", typeof(int));
                        //distributionDetails.Columns.Add("Agent Name", typeof(string));
                        //distributionDetails.Columns.Add("Last Action", typeof(string));
                        //distributionDetails.Columns.Add("Delivery Rate", typeof(int));
                        //distributionDetails.Columns.Add("Publisher Insertcount", typeof(int));
                        //distributionDetails.Columns.Add("Publisher Updatecount", typeof(int));
                        //distributionDetails.Columns.Add("Publisher Deletecount", typeof(int));
                        //distributionDetails.Columns.Add("Publisher Conflicts", typeof(int));
                        //distributionDetails.Columns.Add("Subscriber Insertcount", typeof(int));
                        //distributionDetails.Columns.Add("Subscriber Updatecount", typeof(int));
                        //distributionDetails.Columns.Add("Subscriber Deletecount", typeof(int));
                        //distributionDetails.Columns.Add("Subscriber Conflicts", typeof(int));

                        #endregion
                        if (config.ReplicationType != Common.Objects.Replication.ReplicationType.Merge)
                        {
                            long? nonSubscribed = null;

                            if (!dataReader.IsDBNull(0)) dr["Articles"] = dataReader.GetInt32(0);
                            if (!dataReader.IsDBNull(1)) dr["PublisherDBID"] = dataReader.GetInt32(1);
                            if (!dataReader.IsDBNull(2)) dr["PublisherID"] = dataReader.GetInt16(2);
                            if (!dataReader.IsDBNull(3)) dr["Publisher"] = dataReader.GetString(3);
                            if (!dataReader.IsDBNull(4)) dr["Distributor"] = dataReader.GetString(4);
                            if (!dataReader.IsDBNull(5)) dr["Subscriber"] = dataReader.GetString(5);
                            if (!dataReader.IsDBNull(6)) dr["SubscriptionType"] = dataReader.GetString(6);
                            if (!dataReader.IsDBNull(7)) dr["Sync Type"] = dataReader.GetString(7);
                            if (!dataReader.IsDBNull(8)) dr["PublicationID"] = dataReader.GetInt32(8);
                            if (!dataReader.IsDBNull(9)) dr["Publication"] = dataReader.GetString(9);
                            if (!dataReader.IsDBNull(10)) dr["SubscriberID"] = dataReader.GetInt16(10);
                            if (!dataReader.IsDBNull(11)) dr["AgentID"] = dataReader.GetInt32(11);
                            if (!dataReader.IsDBNull(12)) dr["Subscribed"] = dataReader.GetInt64(12);
                            if (!dataReader.IsDBNull(13)) nonSubscribed = dataReader.GetInt64(13);
                            if (!dataReader.IsDBNull(14)) dr["SubscriptionLatency"] = dataReader.GetInt32(14);
                            
                            if(nonSubscribed.HasValue) dr["Non-Subscribed"] = nonSubscribed.Value;

                            //if we have a sample time then use it to calculate the rate
                            if (!dataReader.IsDBNull(15))
                            {
                                long? subscribed = null;
                                DateTime? sampleTime = dataReader.GetDateTime(15);
                                if (!dataReader.IsDBNull(12)) subscribed = dataReader.GetInt64(12);

                                double? subscriptionRate = GetSubscriptionRate(sampleTime, 
                                                                                    config.LastSampleTime,
                                                                                    subscribed,
                                                                                    config.LastSubscribedTranCount);

                                DateTime catchUpTime =DateTime.MinValue;
                                if(subscriptionRate.HasValue && nonSubscribed.HasValue)
                                    catchUpTime = GetCatchUpTime(subscriptionRate.Value, nonSubscribed.Value);

                                dr["SubscriptionRate"] = subscriptionRate.HasValue ? subscriptionRate.Value : 0;
                                
                                //add the sample time to the return table
                                dr["CatchUpTime"] = catchUpTime;
                                dr["SampleTime"] = sampleTime;
                            }
                        }
                        else
                        {
                            if (!dataReader.IsDBNull(0)) dr["SubscriberName"] = dataReader.GetString(0);
                            if (!dataReader.IsDBNull(1)) dr["MergeSubscriptionstatus"] = dataReader.GetInt32(1);
                            if (!dataReader.IsDBNull(2)) dr["SubscriberDB"] = dataReader.GetString(2);
                            if (!dataReader.IsDBNull(3)) dr["Type"] = dataReader.GetInt32(3);
                            if (!dataReader.IsDBNull(4)) dr["Agent Name"] = dataReader.GetString(4);
                            if (!dataReader.IsDBNull(5)) dr["Last Action"] = dataReader.GetString(5);
                            if (!dataReader.IsDBNull(9)) dr["Delivery Rate"] = dataReader.GetDouble(9);
                            if (!dataReader.IsDBNull(10)) dr["Publisher Insertcount"] = dataReader.GetInt32(10);
                            if (!dataReader.IsDBNull(11)) dr["Publisher Updatecount"] = dataReader.GetInt32(11);
                            if (!dataReader.IsDBNull(12)) dr["Publisher Deletecount"] = dataReader.GetInt32(12);
                            if (!dataReader.IsDBNull(13)) dr["Publisher Conflicts"] = dataReader.GetInt32(13);
                            if (!dataReader.IsDBNull(14)) dr["Subscriber Insertcount"] = dataReader.GetInt32(14);
                            if (!dataReader.IsDBNull(15)) dr["Subscriber Updatecount"] = dataReader.GetInt32(15);
                            if (!dataReader.IsDBNull(16)) dr["Subscriber Deletecount"] = dataReader.GetInt32(16);
                            if (!dataReader.IsDBNull(17)) dr["Subscriber Conflicts"] = dataReader.GetInt32(17);
                        }

                        distributorDetails.DistributionDetails.Rows.Add(dr);
                    }

                    distributorDetails.DistributionDetails.EndLoadData();
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(distributorDetails, LOG, "Error interpreting Distributor Details Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(distributorDetails);
                }
            }
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
                        distributorDetails.SetError("The distributor database [" + config.DistributionDatabase +
                                                 "] does not exist on the distribution server " +
                                                 distributorDetails.ServerName, null);
                    }
                    else
                    {
                        distributorDetails.NonSubscribedTransactions.BeginLoadData();
                        while (dataReader.Read())
                        {
                            if (distributorDetails.PublisherServer == null)
                            {
                                if (!dataReader.IsDBNull(1)) distributorDetails.PublisherServer = dataReader.GetString(1);
                            }

                            DataRow dr = distributorDetails.NonSubscribedTransactions.NewRow();
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


                            distributorDetails.NonSubscribedTransactions.Rows.Add(dr);
                        }

                        distributorDetails.NonSubscribedTransactions.EndLoadData();
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(distributorDetails, LOG, "Error interpreting Distributor Queue Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(distributorDetails);
                }
            }
        }
        /// <summary>
        /// Get the rate of subscription
        /// </summary>
        /// <param name="SampleTime"></param>
        /// <param name="LastSampleTime"></param>
        /// <param name="Sample"></param>
        /// <param name="LastSample"></param>
        /// <returns></returns>
        private double? GetSubscriptionRate(DateTime? SampleTime, DateTime? LastSampleTime, long? Sample, long? LastSample)
        {
            if (!SampleTime.HasValue || !LastSampleTime.HasValue) return null;
            if (!Sample.HasValue || !LastSample.HasValue) return null;

            long Delta = Sample.Value - LastSample.Value;
            TimeSpan timeDelta = SampleTime.Value.Subtract(LastSampleTime.Value);

            return Delta/timeDelta.TotalSeconds;

        }
        private DateTime GetCatchUpTime(double? currentRate, long? queueLength)
        {
            if (!currentRate.HasValue || !queueLength.HasValue) return DateTime.MinValue;

            DateTime catchUpTime = DateTime.MinValue;
            if (currentRate > 0)
            {
                double? secondsTillCatchUp = queueLength / currentRate;

                catchUpTime = DateTime.Now.Add(new TimeSpan(0, 0, (int)secondsTillCatchUp.Value));
            }

            return catchUpTime;
        }
        #endregion

        #region interface implementations

        #endregion
    }
}
