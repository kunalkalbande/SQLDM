//------------------------------------------------------------------------------
// <copyright file="DatabaseConfigurationProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Data;
using System.Globalization;
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


    /// <summary>
    /// Enter a description for this class
    /// </summary>
    internal sealed class DatabaseConfigurationProbe : SqlBaseProbe
    {
        #region fields

        private DatabaseConfigurationSnapshot databaseConfiguration = null;
        private DatabaseProbeConfiguration config = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConfigurationProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public DatabaseConfigurationProbe(SqlConnectionInfo connectionInfo, DatabaseProbeConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("DatabaseConfigurationProbe");
            databaseConfiguration = new DatabaseConfigurationSnapshot(connectionInfo);
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
                StartDatabaseConfigurationCollector();
            }
            else
            {
                FireCompletion(databaseConfiguration, Result.Success);
            }
        }

        /// <summary>
        /// Define the DatabaseConfiguration collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void DatabaseConfigurationCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildDatabaseConfigurationCommand(conn, ver, config, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DatabaseConfigurationCallback));
        }

        /// <summary>
        /// Starts the Database Configuration collector.
        /// </summary>
        private void StartDatabaseConfigurationCollector()
        {
            StartGenericCollector(new Collector(DatabaseConfigurationCollector), databaseConfiguration, "StartDatabaseConfigurationCollector", "Database Configuration", DatabaseConfigurationCallback, new object[] { });
        }

        /// <summary>
        /// Define the DatabaseConfiguration callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DatabaseConfigurationCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretDatabaseConfiguration(rd);
            }
            FireCompletion(databaseConfiguration, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the DatabaseConfiguration collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DatabaseConfigurationCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(DatabaseConfigurationCallback), databaseConfiguration, "DatabaseConfigurationCallback", "Database Configuration",
                            sender, e);
        }

        /// <summary>
        /// Interpret DatabaseConfiguration data
        /// </summary>
        private void InterpretDatabaseConfiguration(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretDatabaseConfiguration"))
            {
                try
                {
                    TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;

                    databaseConfiguration.ConfigurationSettings.BeginLoadData();
                    while (dataReader.Read())
                    {
                        DataRow dr = databaseConfiguration.ConfigurationSettings.NewRow();
                        for (int i = 0; i<dataReader.FieldCount; i++)
                        {
                            if (!dataReader.IsDBNull(i))
                            {
                                if (i == 30)
                                {
                                    string compat = dataReader.GetValue(i).ToString();
                                    dr[i] = Convert.ToDouble(compat) / 10f;
                                }
                                else
                                {
                                    if (i > 1 && databaseConfiguration.ConfigurationSettings.Columns[i].DataType == typeof(string))
                                    {
                                        {
                                            string str = textInfo.ToTitleCase(((string)dataReader.GetValue(i)).ToLowerInvariant());
                                            dr[i] = str.Length > 0 ? str.Replace('_', ' ') : null;
                                        }
                                    }
                                    else
                                    {
                                        dr[i] = dataReader.GetValue(i);
                                    }
                                }
                            }
                        }
                        databaseConfiguration.ConfigurationSettings.Rows.Add(dr);
            
                    }
                    databaseConfiguration.ConfigurationSettings.EndLoadData();
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(databaseConfiguration, LOG, "Error interpreting Database Configuration Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(databaseConfiguration);
                }
            }
        }

        #endregion

        #region interface implementations

        #endregion
    }
}
