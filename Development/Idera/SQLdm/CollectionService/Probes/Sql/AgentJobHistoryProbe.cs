//------------------------------------------------------------------------------
// <copyright file="AgentJobHistoryProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;


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
    /// Probe for Agent Job History
    /// </summary>
    internal sealed class AgentJobHistoryProbe : SqlBaseProbe
    {
        #region fields

        private AgentJobHistorySnapshot agentJobHistory = null;
        private AgentJobHistoryConfiguration config = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentJobHistoryProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public AgentJobHistoryProbe(SqlConnectionInfo connectionInfo, AgentJobHistoryConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("AgentJobHistoryProbe");
            agentJobHistory = new AgentJobHistorySnapshot(connectionInfo);
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
            if (config != null && config.ReadyForCollection && cloudProviderId!=CLOUD_PROVIDER_ID_AZURE)//sqldm-30299 changes
            {
                 StartAgentJobHistoryCollector();
            }
            else
            {
                FireCompletion(agentJobHistory, Result.Success);
            }
        }

        /// <summary>
        /// Define the AgentJobHistory collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void AgentJobHistoryCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            //if (cloudProviderId == 1)
            //{

            //}
            //else
            //{
            //    SqlCommand cmd =
            //   SqlCommandBuilder.BuildAgentJobHistoryCommand(conn, ver, config);
            //    sdtCollector = new SqlCollector(cmd, true);
            //    sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(AgentJobHistoryCallback));
            //}

            SqlCommand cmd =
                SqlCommandBuilder.BuildAgentJobHistoryCommand(conn, ver, config);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(AgentJobHistoryCallback));
        }

        /// <summary>
        /// Starts the Agent Job History collector.
        /// </summary>
        private void StartAgentJobHistoryCollector()
        {
            StartGenericCollector(new Collector(AgentJobHistoryCollector), agentJobHistory, "StartAgentJobHistoryCollector", "Agent Job History", AgentJobHistoryCallback, new object[] { });
        }

        /// <summary>
        /// Define the AgentJobHistory callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void AgentJobHistoryCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretAgentJobHistory(rd);
            }
            FireCompletion(agentJobHistory, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the AgentJobHistory collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void AgentJobHistoryCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(AgentJobHistoryCallback), agentJobHistory, "AgentJobHistoryCallback", "Agent Job History",
                            sender, e);
        }

        /// <summary>
        /// Interpret AgentJobHistory data
        /// </summary>
        private void InterpretAgentJobHistory(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretAgentJobHistory"))
            {
                try
                {
                    ReadJobHistory(dataReader);
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(agentJobHistory, LOG, "Error interpreting Agent Job History Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(agentJobHistory);
                }
            }
        }

        private void ReadJobHistory(SqlDataReader dataReader)
        {
            try
            {
                Guid bufferGuid = Guid.Empty;
                AgentJobStep peekData = null;
                string peekName = null;

                while (dataReader.Read())
                {
                    List<AgentJobStep> bufferSteps = new List<AgentJobStep>();
                    
                    if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(6))
                    {
                        Guid id;
                        AgentJobStep jobStep;
                        string name = null;
                        
                        if (peekData != null)
                        {
                            id = bufferGuid;
                            name = peekName;
                            jobStep = peekData;

                        }
                        else
                        {
                            id = dataReader.GetGuid(0);

                            if (!dataReader.IsDBNull(9)) name = dataReader.GetString(9);

                            string message = dataReader.IsDBNull(8) ? "" : dataReader.GetString(8);
                            if (message != null && message.Length > 0)
                                message = (message.Trim()).TrimStart(new char[] { '\n', '\r' });

                            jobStep = new AgentJobStep(
                                // Instance ID
                                dataReader.GetInt32(1),
                                // Message
                                message,
                                // Name
                                dataReader.IsDBNull(2) ? "" : dataReader.GetString(2),
                                // Retries
                                dataReader.IsDBNull(7) ? 0 : dataReader.GetInt32(7),
                                // Run Seconds
                                dataReader.IsDBNull(5) ? new TimeSpan(0) : ProbeHelpers.TimeSpanFromHHMMSS(dataReader.GetInt32(5)),
                                // Status
                                dataReader.IsDBNull(4) ? -1 : dataReader.GetInt32(4),
                                // Start Time
                                dataReader.GetDateTime(3),
                                // Step Id
                                dataReader.IsDBNull(6) ? -1 : dataReader.GetInt32(6)
                                );
                        }
                       

                        while (jobStep.StepId != 0)
                        {
                            bufferSteps.Add(jobStep);
                            if (dataReader.Read())
                            {
                                bufferGuid = dataReader.GetGuid(0);
                                if (bufferGuid != id)
                                {
                                    if (!dataReader.IsDBNull(9)) peekName = dataReader.GetString(9);

                                    string message = dataReader.IsDBNull(8) ? "" : dataReader.GetString(8);
                                    if (message != null && message.Length > 0)
                                        message = (message.Trim()).TrimStart(new char[] { '\n', '\r' });

                                    peekData = new AgentJobStep(
                                        // Instance ID
                                    dataReader.GetInt32(1),
                                        // Message
                                    message,
                                        // Name
                                    dataReader.IsDBNull(2) ? "" : dataReader.GetString(2),
                                        // Retries
                                    dataReader.IsDBNull(7) ? 0 : dataReader.GetInt32(7),
                                        // Run Seconds
                                    ProbeHelpers.TimeSpanFromHHMMSS(dataReader.GetInt32(5)),
                                        // Status
                                    dataReader.IsDBNull(4) ? -1 : dataReader.GetInt32(4),
                                        // Start Time
                                    dataReader.GetDateTime(3),
                                        // Step Id
                                    dataReader.IsDBNull(6) ? -1 : dataReader.GetInt32(6)
                                    );
                                    break;
                                }
                                else
                                {
                                    peekData = null;

                                    string message = dataReader.IsDBNull(8) ? "" : dataReader.GetString(8);
                                    if (message != null && message.Length > 0)
                                        message = (message.Trim()).TrimStart(new char[] { '\n', '\r' });

                                    jobStep = new AgentJobStep(
                                        // Instance ID
                                        dataReader.GetInt32(1),
                                        // Message
                                        message,
                                        // Name
                                        dataReader.IsDBNull(2) ? "" : dataReader.GetString(2),
                                        // Retries
                                        dataReader.IsDBNull(7) ? 0 : dataReader.GetInt32(7),
                                        // Run Seconds
                                        ProbeHelpers.TimeSpanFromHHMMSS(dataReader.GetInt32(5)),
                                        // Status
                                        dataReader.IsDBNull(4) ? -1 : dataReader.GetInt32(4),
                                        // Start Time
                                        dataReader.GetDateTime(3),
                                        // Step Id
                                        dataReader.IsDBNull(6) ? -1 : dataReader.GetInt32(6)
                                        );
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (!config.ShowFailedOnly || jobStep.RunStatus == JobStepRunStatus.Failed || jobStep.RunStatus == JobStepRunStatus.Cancelled)
                        {
                            if (!agentJobHistory.JobHistories.ContainsKey(id))
                            {
                                agentJobHistory.JobHistories.Add(id, new AgentJobHistory(name, id));
                            }
                            agentJobHistory.JobHistories[id].Executions.Add(new AgentJobExecution(bufferSteps, jobStep));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(agentJobHistory, LOG, "Error interpreting Agent Job History Collector: {0}", e,
                                                        false);
            }
        }
        #endregion

        #region interface implementations

        #endregion
    }
}
