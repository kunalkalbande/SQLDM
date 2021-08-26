//------------------------------------------------------------------------------
// <copyright file="SubscriberDetailsProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Data;
using System.Text;
//using System.Text.RegularExpressions;

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
    internal sealed class SubscriberDetailsProbe : SqlBaseProbe
    {
        #region fields

        private SubscriberDetails _subscriberDetails = null;
        private SubscriberDetailsConfiguration _config = null;
        private DataTable _subscriptionsDataTable = new DataTable("SubscriptionDetails");

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherDetailsProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="config"></param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public SubscriberDetailsProbe(SqlConnectionInfo connectionInfo, SubscriberDetailsConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("SubscriberDetailsProbe");
            _subscriberDetails = new SubscriberDetails(connectionInfo);
            _config = config;
            this.cloudProviderId = cloudProviderId;

            //_subscriptionsDataTable.RemotingFormat = SerializationFormat.Binary;
            _subscriptionsDataTable.Columns.Add("PublisherInstance", typeof(string));
            _subscriptionsDataTable.Columns.Add("PublisherDB", typeof(string));
            _subscriptionsDataTable.Columns.Add("PublicationName", typeof(string));
            _subscriptionsDataTable.Columns.Add("ReplicationType", typeof(int));
            _subscriptionsDataTable.Columns.Add("SubscriptionType", typeof(int));
            _subscriptionsDataTable.Columns.Add("LastUpdated", typeof(DateTime));
            _subscriptionsDataTable.Columns.Add("SubscriberDatabase", typeof(string));
            _subscriptionsDataTable.Columns.Add("LastSyncStatus", typeof(int));
            _subscriptionsDataTable.Columns.Add("LastSyncSummary", typeof(string));
            _subscriptionsDataTable.Columns.Add("LastSyncTime", typeof(DateTime));
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
            if (_config != null && _config.ReadyForCollection)
            {
                StartSubscriberDetailsCollector();
            }
            else
            {
                FireCompletion(_subscriberDetails, Result.Success);
            }
        }

        /// <summary>
        /// Define the subscriber details collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void SubscriberDetailsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildSubscriberDetailsCommand(conn, ver, _config.SubscriberDatabase);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(SubscriberDetailsCallback));
        }

        /// <summary>
        /// Starts the Subscriber details collector.
        /// exec sp_helpdistributor
        /// </summary>
        private void StartSubscriberDetailsCollector()
        {
            StartGenericCollector(new Collector(SubscriberDetailsCollector), _subscriberDetails, "StartSubscriberDetailsCollector", "Replication Subscriber Details", null, new object[] { });
        }

        /// <summary>
        /// Define the subscriber details callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void SubscriberDetailsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretSubscriberDetails(rd);
            }
            FireCompletion(_subscriberDetails, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the ReplicationStatus collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void SubscriberDetailsCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(SubscriberDetailsCallback),
                _subscriberDetails,
                "SubscriberDetailsCallback",
                "Replication Subscriber Details",
                sender, e);
        }


        /// <summary>
        /// Interpret Replication Details data
        /// </summary>
        private void InterpretSubscriberDetails(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretSubscriberDetails"))
            {
                try
                {
                    while (dataReader.Read())
                    {
                        DataRow dr = _subscriptionsDataTable.NewRow();

                        if (!dataReader.IsDBNull(0))dr["PublisherInstance"] = dataReader.GetString(0);
                        if (!dataReader.IsDBNull(1))dr["PublisherDB"] = dataReader.GetString(1);
                        if (!dataReader.IsDBNull(2))dr["PublicationName"] = dataReader.GetString(2);
                        if (!dataReader.IsDBNull(3))dr["ReplicationType"] = dataReader.GetInt32(3);

                        if (!dataReader.IsDBNull(4))dr["SubscriptionType"] = dataReader.GetInt32(4);
                        if (!dataReader.IsDBNull(5))dr["LastUpdated"] = dataReader.GetDateTime(5);
                        if (!dataReader.IsDBNull(6))dr["SubscriberDatabase"] = dataReader.GetString(6);
                        if (!dataReader.IsDBNull(8))dr["LastSyncStatus"] = dataReader.GetInt32(8);
                        if (!dataReader.IsDBNull(9))dr["LastSyncSummary"] = dataReader.GetString(9);
                        if (!dataReader.IsDBNull(10)) dr["LastSyncTime"] = dataReader.GetDateTime(10);

                        _subscriptionsDataTable.Rows.Add(dr);
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(_subscriberDetails, LOG, "Error interpreting Subscriber details Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(_subscriberDetails);
                }

                DataRow[] found = _subscriptionsDataTable.Select(string.Format("SubscriberDatabase = '{0}' AND PublicationName = '{1}'", _config.SubscriberDatabase.Replace("'", "''"), _config.Publication.Replace("'", "''")));

                if (_subscriptionsDataTable.Rows.Count == 0)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(_subscriberDetails, LOG, string.Format("No subscription information for publication \"{1}\" was found in database \"{0}\"", _config.SubscriberDatabase, _config.Publication), false);
                    return;
                }
                if (found.Length == 0)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(_subscriberDetails, LOG, string.Format("Subscriber {0} is not subscribing to {1}",_config.SubscriberDatabase, _config.Publication), false);
                    return;
                }
                
                //return only the details for the selected subscription
                for (int i = 0; i < found.Length; i++)
                {
                    if (!(found[i]["PublisherDB"] is DBNull)) _subscriberDetails.PublisherDB = found[i]["PublisherDB"].ToString();
                    if (!(found[i]["PublicationName"] is DBNull)) _subscriberDetails.PublisherInstance = found[i]["PublicationName"].ToString();
                    if (!(found[i]["ReplicationType"] is DBNull)) _subscriberDetails.ReplicationType = (int)found[i]["ReplicationType"];
                    if (!(found[i]["SubscriptionType"] is DBNull)) _subscriberDetails.SubscriptionType = (int)found[i]["SubscriptionType"];
                    if (!(found[i]["LastUpdated"] is DBNull)) _subscriberDetails.LastUpdated = (DateTime)found[i]["LastUpdated"];
                    if (!(found[i]["SubscriberDatabase"] is DBNull)) _subscriberDetails.SubscriberDatabase = found[i]["SubscriberDatabase"].ToString();
                    if (!(found[i]["LastSyncStatus"] is DBNull)) _subscriberDetails.LastSyncStatus = (int)found[i]["LastSyncStatus"];
                    if (!(found[i]["LastSyncSummary"] is DBNull)) _subscriberDetails.LastSyncSummary = found[i]["LastSyncSummary"].ToString();
                    if (!(found[i]["LastSyncTime"] is DBNull)) _subscriberDetails.LastSyncTime = (DateTime)found[i]["LastSyncTime"];
                    continue;
                }
            }
        }

        #endregion

        #region interface implementations

        #endregion
    }
}
