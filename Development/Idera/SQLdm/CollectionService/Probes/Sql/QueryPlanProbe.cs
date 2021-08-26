//------------------------------------------------------------------------------
// <copyright file="QueryPlanProbe.cs" company="Idera, Inc.">
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
    using System.Text;

    /// <summary>
    /// //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) - New Probe class
    /// </summary>
    internal class QueryPlanProbe : SqlBaseProbe
    {
        #region fields

        private QueryPlanSnapshot snapshot = null;
        private string strQuery = string.Empty;
        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public QueryPlanProbe(SqlConnectionInfo connectionInfo, string query, int? cloudProviderId)
            : base(connectionInfo)
        {
            snapshot = new QueryPlanSnapshot(connectionInfo);
            this.cloudProviderId = cloudProviderId;
            strQuery = query;
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
            StartSetShowPlanONCollector();
        }

        /// <summary>
        /// Define the SetShowPlanON collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void SetShowPlanONCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildSetShowPlanONCommand(conn, ver);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(SetShowPlanONCallback));
        }

        /// <summary>
        /// Starts the SetShowPlanON collector.
        /// </summary>
        public void StartSetShowPlanONCollector()
        {
            StartGenericCollector(new Collector(SetShowPlanONCollector), snapshot, "StartSetShowPlanONCollector", "SetShowPlanON", null, new object[] { });
        }

        /// <summary>
        /// Define the SetShowPlanON callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void SetShowPlanONCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretSetShowPlanON(rd);
            }
            //FireCompletion(snapshot, Result.Success);
            StartQueryPlanCollector();
        }

        /// <summary>
        /// Callback used to process the data returned from the SetShowPlanON collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void SetShowPlanONCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(SetShowPlanONCallback), snapshot, "SetShowPlanONCallback", "SetShowPlanON",
                            sender, e);
        }

        private void InterpretSetShowPlanON(SqlDataReader datareader)
        {
        }



        /// <summary>
        /// Define the QueryPlan collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void QueryPlanCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            using (SqlCommand cmd1 = conn.CreateCommand())
            {
                cmd1.CommandText = "set showplan_xml_with_recompile on";
                cmd1.CommandType = System.Data.CommandType.Text;
                cmd1.ExecuteNonQuery();
            }

            SqlCommand cmd = SqlCommandBuilder.BuildQueryPlanCommand(conn, ver, strQuery);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(QueryPlanCallback));
        }

        /// <summary>
        /// Starts the QueryPlan collector.
        /// </summary>
        void StartQueryPlanCollector()
        {
            StartGenericCollector(new Collector(QueryPlanCollector), snapshot, "StartQueryPlanCollector", "QueryPlan", null, new object[] { });
        }

        /// <summary>
        /// Define the QueryPlan callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void QueryPlanCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretQueryPlan(rd);
            }
            //FireCompletion(snapshot, Result.Success);
            StartSetShowPlanOFFCollector();
        }

        /// <summary>
        /// Callback used to process the data returned from the QueryPlan collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void QueryPlanCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(QueryPlanCallback), snapshot, "QueryPlanCallback", "QueryPlan",
                            sender, e);
        }

        private void InterpretQueryPlan(SqlDataReader datareader)
        {
            StringBuilder result = new StringBuilder();
            do
            {
                while (datareader.Read())
                {
                    result.Append(datareader.GetSqlString(0).ToString());
                }
            } while (datareader.NextResult());

            snapshot.QueryPlan = result.ToString();
        }



        /// <summary>
        /// Define the SetShowPlanOFF collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void SetShowPlanOFFCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildSetShowPlanOFFCommand(conn, ver);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(SetShowPlanOFFCallback));
        }

        /// <summary>
        /// Starts the SetShowPlanOFF collector.
        /// </summary>
        protected void StartSetShowPlanOFFCollector()
        {
            StartGenericCollector(new Collector(SetShowPlanOFFCollector), snapshot, "StartSetShowPlanOFFCollector", "SetShowPlanOFF", null, new object[] { });
        }

        /// <summary>
        /// Define the SetShowPlanOFF callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void SetShowPlanOFFCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretSetShowPlanOFF(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the SetShowPlanOFF collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void SetShowPlanOFFCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(SetShowPlanOFFCallback), snapshot, "SetShowPlanOFFCallback", "SetShowPlanOFF",
                            sender, e);
        }

        private void InterpretSetShowPlanOFF(SqlDataReader datareader)
        {
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
