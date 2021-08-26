//------------------------------------------------------------------------------
// <copyright file="ConfigurationProbe.cs" company="Idera, Inc.">
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
    /// On-demand probe for server sessions
    /// </summary>
    internal class ConfigurationProbe : SqlBaseProbe
    {
        #region fields

        private ConfigurationSnapshot snapshot = null;
        private ServerVersion version = null;       //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- added property to use in InterpretConfiguration method
        //private ReconfigurationConfiguration reconfig = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public ConfigurationProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
            : base(connectionInfo)
        {
            snapshot = new ConfigurationSnapshot(connectionInfo);
            LOG = Logger.GetLogger("SessionsProbe");
            //reconfig = configuration;
            // Skip permissions for CloudProviders
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
            //if (cloudProviderId != CLOUD_PROVIDER_ID_AZURE)
                StartConfigurationCollector();
            //else
            //    FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Define the Configuration collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void ConfigurationCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            version = ver;//SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics)
            SqlCommand cmd =
                           SqlCommandBuilder.BuildConfigurationCommand(conn, ver, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ConfigurationCallback));
        }

        /// <summary>
        /// Starts the Configuration collector.
        /// </summary>
        void StartConfigurationCollector()
        {
            StartGenericCollector(new Collector(ConfigurationCollector), snapshot, "StartConfigurationCollector", "Configuration", ConfigurationCallback, new object[] { });
        }

        /// <summary>
        /// Define the Configuration callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void ConfigurationCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretConfiguration(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the configuration collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void ConfigurationCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(ConfigurationCallback), snapshot, "ConfigurationCallback", "Configuration",
                            sender, e);
        }

        private void InterpretConfiguration(SqlDataReader datareader)
        {
            snapshot.ConfigurationSettings.Columns.Add("Name", typeof(string));
            snapshot.ConfigurationSettings.Columns.Add("Comment", typeof(string));
            snapshot.ConfigurationSettings.Columns.Add("Minimum", typeof(int));
            snapshot.ConfigurationSettings.Columns.Add("Maximum", typeof(int));
            snapshot.ConfigurationSettings.Columns.Add("Config Value", typeof(int));
            snapshot.ConfigurationSettings.Columns.Add("Run Value", typeof(int));
            snapshot.ConfigurationSettings.Columns.Add("Restart Required", typeof(bool));
            snapshot.ConfigurationSettings.Columns.Add("ID", typeof(int)); //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new column for configuration id

            //START: SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- add columns to newly added ServerProperties in the snapshot.
            if (version.Major >= 9)
            {
                snapshot.ServerProperties.Columns.Add("Name", typeof(string));
                snapshot.ServerProperties.Columns.Add("Value", typeof(string));
            }
            //END: SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- add columns to newly added ServerProperties in the snapshot.

            while (datareader.Read())
            {
                snapshot.ConfigurationSettings.Rows.Add(
                    datareader.GetString(0),
                    datareader.GetString(1),
                    datareader.GetInt32(2),
                    datareader.GetInt32(3),
                    datareader.GetInt32(4),
                    datareader.GetInt32(5),
                    datareader.GetInt32(6) == 1 ? true : false,
                    version.Major >= 9 ? datareader.GetInt32(7) : 0 //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- populate new column for configuration id
                );
            }

            //START: SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- add rows to newly added ServerProperties in the snapshot.
            if (version.Major >= 9)
            {
                if (datareader.NextResult())
                {
                    while (datareader.Read())
                    {
                        snapshot.ServerProperties.Rows.Add(
                            ProbeHelpers.ToString(datareader, "Name"),
                            ProbeHelpers.ToString(datareader, "Value")
                            );
                    }
                }
            }
            //END: SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- add rows to newly added ServerProperties in the snapshot.
        }


        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
