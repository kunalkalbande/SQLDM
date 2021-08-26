//------------------------------------------------------------------------------
// <copyright file="QueryPlanProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    using Idera.SQLdm.CollectionService.Probes.Collectors;
    using Idera.SQLdm.Common;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Snapshots;
    using System;
    using System.Data.SqlClient;

    /// <summary>
    /// SQLdm 10.4 Nikhil Bansal
    /// New Probe Class to collect the Estimated Query Plan On Demand 
    /// </summary>
    internal class EstimatedQueryPlanProbe : SqlBaseProbe
    {

        #region fields

        private QueryPlanSnapshot snapshot;
        private EstimatedQueryPlanConfiguration config;
        private SqlConnection conn;

        #endregion

        #region constructors

        public EstimatedQueryPlanProbe(SqlConnectionInfo connectionInfo, EstimatedQueryPlanConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            snapshot = new QueryPlanSnapshot(connectionInfo);
            this.config = config;
            this.cloudProviderId = cloudProviderId;
        }

        #endregion

        #region methods

        /// <summary>
        /// Starts the probe, virtual method called by BaseProbe.BeginProbe().
        /// If this returns cleanly, probe must call FireCompletion()
        /// </summary>
        protected override void Start()
        {
            StartUseDBCollector();
        }

        #region UseDB Command

        /// <summary>
        /// Starts the UseDB collector.
        /// </summary>
        void StartUseDBCollector()
        {
            StartGenericCollector(new Collector(UseDBCollector), snapshot, "StartUseDBCollector", "UseDBCollector", null, new object[] { });
        }

        /// <summary>
        /// Define the UseDB collector
        /// </summary>
        /// <param name="connection">Open SQL connection</param>
        /// <param name="collector">Standard SQL collector</param>
        /// <param name="version">Server version</param>
        void UseDBCollector(SqlConnection connection, SqlCollector collector, ServerVersion version)
        {
            conn = new SqlConnection(connection.ConnectionString);
            conn.Open();
            SqlCommand cmdUseDB = conn.CreateCommand();
            cmdUseDB.CommandText = String.Format("Use [{0}]", config.DatabaseName);
            collector = new SqlCollector(cmdUseDB, true);
            collector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(UseDBCallback));
        }

        /// <summary>
        /// Callback used to process the data returned from the UseDB collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void UseDBCallback(object sender, CollectorCompleteEventArgs e)
        {
            SqlDataReader reader = e.Value as SqlDataReader;
            reader.Close();
            StartSetShowPlanONCollector();
        }

        #endregion

        #region SetShowPlanON Command
        /// <summary>
        /// Starts the SetShowPlanOn collector.
        /// </summary>
        void StartSetShowPlanONCollector()
        {
            StartGenericCollector(new Collector(SetShowPlanONCollector), snapshot, "StartSetShowPlanONCollector", "SetShowPlanONCollector", null, new object[] { });
        }

        /// <summary>
        /// Define the SetShowPlanON collector
        /// </summary>
        /// <param name="connection">Open SQL connection</param>
        /// <param name="collector">Standard SQL collector</param>
        /// <param name="version">Server version</param>
        void SetShowPlanONCollector(SqlConnection connection, SqlCollector collector, ServerVersion version)
        {
            SqlCommand cmdSetShowPlanON = conn.CreateCommand();
            cmdSetShowPlanON.CommandText = "SET SHOWPLAN_XML_WITH_RECOMPILE ON";
            collector = new SqlCollector(cmdSetShowPlanON, true);
            collector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(SetShowPlanONCallback));
        }

        /// <summary>
        /// Callback used to process the data returned from the SetShowPlanON collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void SetShowPlanONCallback(object sender, CollectorCompleteEventArgs e)
        {
            SqlDataReader reader = e.Value as SqlDataReader;
            reader.Close();

            StartEstimatedQueryPlanCollector();
        }

        #endregion

        #region EstimatedQueryPlan Command
        /// <summary>
        /// Starts the EstimatedQueryPlan collector.
        /// </summary>
        void StartEstimatedQueryPlanCollector()
        {
            StartGenericCollector(new Collector(EstimatedQueryPlanCollector), snapshot, "StartEstimatedQueryPlanCollector", "EstimatedQueryPlanCollector", null, new object[] { });
        }

        /// <summary>
        /// Define the EstimatedQueryPlan collector
        /// </summary>
        /// <param name="connection">Open SQL connection</param>
        /// <param name="collector">Standard SQL collector</param>
        /// <param name="version">Server version</param>
        void EstimatedQueryPlanCollector(SqlConnection connection, SqlCollector collector, ServerVersion version)
        {
            SqlCommand cmdReadEstimatedPlan = SqlCommandBuilder.BuildQueryMonitorReadEstimatedPlanCommand(conn, version, config.QueryText);
            collector = new SqlCollector(cmdReadEstimatedPlan, true);
            collector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(EstimatedQueryPlanCallback));
        }

        /// <summary>
        /// Callback used to process the data returned from the EstimatedQueryPlan collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void EstimatedQueryPlanCallback(object sender, CollectorCompleteEventArgs e)
        {
            SqlDataReader reader = e.Value as SqlDataReader;
            if (reader != null)
            {
                InterpretEstimatedQueryPlan(reader);
                reader.Close();
            }
            StartSetShowPlanOFFCollector();
        }

        /// <summary>
        /// Interpret the data retrieved by the collector
        /// </summary>
        /// <param name="reader">SQL Data Reader containing the data retrieved by the collector</param>
        private void InterpretEstimatedQueryPlan(SqlDataReader reader)
        {
            using(reader)
            {
                if(reader != null)
                { 
                    if(reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            snapshot.QueryPlan = Convert.ToString(reader.GetSqlString(0));
                            snapshot.IsActualPlan = false;
                        }
                    }
                }
            }
        }

        #endregion

        #region SetShowPlanOFF Command

        /// <summary>
        /// Starts the SetShowPlanOFF collector.
        /// </summary>
        void StartSetShowPlanOFFCollector()
        {
            StartGenericCollector(new Collector(SetShowPlanOFFCollector), snapshot, "StartSetShowPlanOFFCollector", "SetShowPlanOFFCollector", null, new object[] { });
        }

        /// <summary>
        /// Define the SetShowPlanOFF collector
        /// </summary>
        /// <param name="connection">Open SQL connection</param>
        /// <param name="collector">Standard SQL collector</param>
        /// <param name="version">Server version</param>
        void SetShowPlanOFFCollector(SqlConnection connection, SqlCollector collector, ServerVersion version)
        {
            SqlCommand cmdSetShowPlanOFF = conn.CreateCommand();
            cmdSetShowPlanOFF.CommandText = "SET SHOWPLAN_XML_WITH_RECOMPILE OFF";
            collector = new SqlCollector(cmdSetShowPlanOFF, true);
            collector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(SetShowPlanOFFCallback));
        }

        /// <summary>
        /// Callback used to process the data returned from the SetShowPlanOFF collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void SetShowPlanOFFCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(SetShowPlanOFFCallback), snapshot, "SetShowPlanOFFCallback", "SetShowPlanOFF", sender, e);
        }

        /// <summary>
        /// Define the SetShowPlanOFF callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void SetShowPlanOFFCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader reader = e.Value as SqlDataReader)
            {
                InterpretSetShowPlanOFF(reader);
            }

            conn.Close();

            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Interpret the data retrieved by the collector
        /// </summary>
        /// <param name="reader">SQL Data Reader containing the data retrieved by the collector</param>
        private void InterpretSetShowPlanOFF(SqlDataReader reader)
        {
        }

        #endregion

        #endregion

    }
}
