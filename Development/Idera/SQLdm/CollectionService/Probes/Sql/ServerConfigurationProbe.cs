//------------------------------------------------------------------------------
// <copyright file="ServerConfigurationProbe.cs" company="Idera, Inc.">
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
    using Common.Services;
    using Common.Snapshots;

    /// <summary>
    /// //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) - New Probe class
    /// </summary>
    internal class ServerConfigurationProbe : SqlBaseProbe
    {
        #region fields

        private ServerConfigurationSnapshot snapshot = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConfigurationProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public ServerConfigurationProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            snapshot = new ServerConfigurationSnapshot(connectionInfo);
            LOG = Logger.GetLogger("SessionsProbe");
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
           
            StartServerConfigurationCollector();
           
        }

        /// <summary>
        /// Define the ServerConfiguration collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void ServerConfigurationCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildServerConfigurationCommand(conn, ver, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServerConfigurationCallback));
        }

        /// <summary>
        /// Starts the ServerConfiguration collector.
        /// </summary>
        void StartServerConfigurationCollector()
        {
            StartGenericCollector(new Collector(ServerConfigurationCollector), snapshot, "StartServerConfigurationCollector", "ServerConfiguration", ServerConfigurationCallback, new object[] { });
        }

        /// <summary>
        /// Define the ServerConfiguration callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void ServerConfigurationCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretServerConfiguration(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the ServerConfiguration collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void ServerConfigurationCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(ServerConfigurationCallback), snapshot, "ServerConfigurationCallback", "ServerConfiguration",
                            sender, e);
        }

        private void InterpretServerConfiguration(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretServerConfiguration"))
            {
                try
                {
                    snapshot.ServerConfiguration.Columns.Add("name", typeof(string));
                    snapshot.ServerConfiguration.Columns.Add("value", typeof(Int32));
                    snapshot.ServerConfiguration.Columns.Add("value_in_use", typeof(Int32));
                    snapshot.ServerConfiguration.Columns.Add("is_dynamic", typeof(Int32));

                    while (datareader.Read())
                    {
                        snapshot.ServerConfiguration.Rows.Add(
                            ProbeHelpers.ToString(datareader, "name"),
                            ProbeHelpers.ToInt32(datareader, "value"),
                            ProbeHelpers.ToInt32(datareader, "value_in_use"),
                            ProbeHelpers.ToInt32(datareader, "is_dynamic")
                        );
                    }

                    snapshot.SecuritySettings.Columns.Add("name", typeof(string));
                    snapshot.SecuritySettings.Columns.Add("value", typeof(Int32));

                    if (datareader.NextResult())
                    {
                        while (datareader.Read())
                        {
                            snapshot.SecuritySettings.Rows.Add(
                               ProbeHelpers.ToString(datareader, "name"),
                               ProbeHelpers.ToInt32(datareader, "value")
                           );
                        }
                    }

                    snapshot.VulnerableLogins.Columns.Add("username", typeof(string));
                    snapshot.VulnerableLogins.Columns.Add("policy", typeof(bool));
                    snapshot.VulnerableLogins.Columns.Add("expire", typeof(bool));

                    if (datareader.NextResult())
                    {
                        while (datareader.Read())
                        {
                            snapshot.VulnerableLogins.Rows.Add(
                               ProbeHelpers.ToString(datareader, "username"),
                               ProbeHelpers.ToBoolean(datareader, "policy"),
                               ProbeHelpers.ToBoolean(datareader, "expire")
                           );
                        }
                    }

                    snapshot.DeprecatedAgentTokenJobs.Columns.Add("name", typeof(string));
                    snapshot.DeprecatedAgentTokenJobs.Columns.Add("step_name", typeof(string));

                    if (datareader.NextResult())
                    {
                        while (datareader.Read())
                        {
                            snapshot.DeprecatedAgentTokenJobs.Rows.Add(
                               ProbeHelpers.ToString(datareader, "name"),
                               ProbeHelpers.ToString(datareader, "step_name")
                           );
                        }
                    }
                    //SQLdm10.0 -Srishti Purohit -  New Recommendations
                    if (datareader.NextResult())
                    {
                        while (datareader.Read())
                        {
                            if (DBNull.Value != datareader["Global"])
                                snapshot.TraceFlag4199 = Convert.ToBoolean(datareader["Global"]);
                        }
                    }
                    //SQLdm10.0 -Srishti Purohit -  New Recommendations (SDR-Q37, SDR-Q38)
                    if (datareader.NextResult())
                    {
                        while (datareader.Read())
                        {
                            snapshot.Compatibility.Add(ProbeHelpers.ToString(datareader, "name"), ProbeHelpers.ToInt32(datareader, "compatibility_level"));
                        }
                    }
                    //Start: SQLdm10.0 -Srishti Purohit -  new recommendations (SDR-M31, SDR-M32)
                    datareader.NextResult();
                    while (datareader.Read())
                    {
                        snapshot.MaxServerMemorySizeMB = ProbeHelpers.ToInt64(datareader, "value_in_use");
                    }

                    datareader.NextResult();
                    while (datareader.Read())
                    {
                        snapshot.BufferPoolExtFilePath = ProbeHelpers.ToString(datareader, "path");
                        snapshot.BufferPoolExtSizeKB = ProbeHelpers.ToInt64(datareader, "current_size_in_kb");
                    }
                    //End: SQLdm10.0 -Srishti Purohit -  new recommendations (SDR-M31, SDR-M32)
                    //Start: SQLdm10.0 -Srishti Purohit -  new recommendations (SDR-D23)
                    datareader.NextResult();
                    while (datareader.Read())
                    {
                        snapshot.Edition = ProbeHelpers.ToString(datareader, "Edition");
                    }

                    datareader.NextResult();
                    while (datareader.Read())
                    {
                        snapshot.IsResourceGovernerEnable = ProbeHelpers.ToBoolean(datareader, "is_enabled");
                    }

                    datareader.NextResult();
                    while (datareader.Read())
                    {
                        string name = ProbeHelpers.ToString(datareader, "name");
                        if (!string.IsNullOrEmpty(name)) { snapshot.ResourcePoolNameList.Add(name); }
                    }
                    //End: SQLdm10.0 -Srishti Purohit -  new recommendations (SDR-D23)

                    //Start: SQLdm10.0 -Srishti Purohit -  New Recommendations (SDR-Q45)
                    if (datareader.NextResult())
                    {
                        while (datareader.Read())
                        {
                            if (DBNull.Value != datareader["Status"])
                                snapshot.TraceFlag2312 = Convert.ToBoolean(datareader["Status"]);
                        }
                    }
                    if (datareader.NextResult())
                    {
                        while (datareader.Read())
                        {
                            if (DBNull.Value != datareader["Status"])
                                snapshot.TraceFlag9481 = Convert.ToBoolean(datareader["Status"]);
                        }
                    }
                    //End: SQLdm10.0 -Srishti Purohit -  new recommendations (SDR-Q45)
                    //Start: SQLdm10.0 -Srishti Purohit -  new recommendations (SDR-R8)
                    datareader.NextResult();
                    while (datareader.Read())
                    {
                        string availabilityGroupName = ProbeHelpers.ToString(datareader, "name");
                        bool dbFailover = ProbeHelpers.ToBoolean(datareader, "db_failover");
                        snapshot.AvailabilityGroups.Add(availabilityGroupName, dbFailover);
                    }
                    //End: SQLdm10.0 -Srishti Purohit -  new recommendations (SDR-R8)
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting ServerConfiguration Collector: {0}",
                                                        e,
                                                        false);
                    GenericFailureDelegate(snapshot);
                }
            }
        }


        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
