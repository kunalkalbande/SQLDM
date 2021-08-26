//------------------------------------------------------------------------------
// <copyright file="AgentJobSummaryProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using Idera.SQLdm.CollectionService.Probes.Wmi;

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
    /// Probe for Agent Job Summary on-demand view
    /// </summary>
    internal sealed class AgentJobSummaryProbe : SqlBaseProbe
    {
        #region fields

        private AgentJobSummary agentJobSummary = null;
        private AgentJobSummaryConfiguration config = null;
        private WmiConfiguration _wmiConfig;
        private MonitoredServerWorkload workload; // SQLdm 8.6 (Ankit Srivastava) -- Added workload field - solved defect DE43661

        private ClusterCollectionSetting clusterCollectionSetting = ClusterCollectionSetting.Default;

        private bool tryDirectWmi;
        private string machineName;
        private string serverName;
        private string agentName;
        private ServiceStatusProbe _wmiprobe;
        private IAsyncResult _wmiProbeIAR;
        private bool isBypass;

        #endregion

        #region constructors

        public AgentJobSummaryProbe(SqlConnectionInfo connectionInfo, AgentJobSummaryConfiguration config, WmiConfiguration wmiConfig, ClusterCollectionSetting clusterCollectionSetting, MonitoredServerWorkload workload, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("AgentJobSummaryProbe");
            agentJobSummary = new AgentJobSummary(connectionInfo);
            this.config = config;
            _wmiConfig = wmiConfig;
            this.cloudProviderId = cloudProviderId;
            this.clusterCollectionSetting = clusterCollectionSetting;
            this.workload = workload; // SQLdm 8.6 (Ankit Srivastava) -- Added workload field - solved defect DE43661
            this.isBypass = false;
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
                StartAgentJobSummaryCollector();
            }
            else
            {
                FireCompletion(agentJobSummary, Result.Success);
            }
        }

        /// <summary>
        /// Define the AgentJobSummary collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void AgentJobSummaryCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd = new SqlCommand();
            isBypass = false;

            /*SqlCommand cmd = new SqlCommand();
            if (cloudProviderId==1)
            {
                 cmd =
               SqlCommandBuilder.BuildAgentJobSummaryCommand(conn, ver, config, clusterCollectionSetting, workload);
                sdtCollector = new SqlCollector(cmd, true);
                sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(AgentJobSummaryCallback));
            }
            else
            {
                 cmd =
               SqlCommandBuilder.BuildAgentJobSummaryCommand(conn, ver, config, _wmiConfig, clusterCollectionSetting, workload);
                sdtCollector = new SqlCollector(cmd, true);
                sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(AgentJobSummaryCallback));
            }*/
             cmd =
                SqlCommandBuilder.BuildAgentJobSummaryCommand(conn, ver, config, _wmiConfig, clusterCollectionSetting, workload);
            
            //could not find issys admin directly in probe so added following code snippet
bool isSysAdmin = false;
            try
            {
                if (cloudProviderId == Constants.AmazonRDSId)
                {
                    using (SqlConnection connection = new SqlConnection(connectionInfo.ConnectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = @"select is_srvrolemember('sysadmin') AS SysAdmin;";//SQLdm 10.1 (srishti purohit) -IsSysAdmin check
                            using (var reader = command.ExecuteReader())
                            {

                                while (reader.Read())
                                {
                                    isSysAdmin = Convert.ToBoolean(reader["SysAdmin"]);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }


            if (cloudProviderId == Constants.AmazonRDSId && !isSysAdmin)
            {
                cmd =
             SqlCommandBuilder.BuildAgentJobSummaryCommand(conn, ver, config, clusterCollectionSetting, workload);
                isBypass = true;
            }
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(AgentJobSummaryCallback));

        }

        /// <summary>
        /// Starts the Agent Job Summary collector.
        /// </summary>
        private void StartAgentJobSummaryCollector()
        {
            StartGenericCollector(new Collector(AgentJobSummaryCollector), agentJobSummary, "StartAgentJobSummaryCollector", "Agent Job Summary", AgentJobSummaryCallback, new object[] { });
        }

        /// <summary>
        /// Define the AgentJobSummary callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void AgentJobSummaryCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretAgentJobSummary(rd);
            }

                if (tryDirectWmi)
                StartMachineNameCollector();//SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameCollector before AgentStatusCollector
            //StartAgentStatusCollection();
            else
                FireCompletion(agentJobSummary, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the AgentJobSummary collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void AgentJobSummaryCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(AgentJobSummaryCallback), agentJobSummary, "AgentJobSummaryCallback", "Agent Job Summary",
                            sender, e);
        }

		//START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
        /// <summary>
        /// Starts the Machine Name collector.
        /// before the WMI probe for Service Status which needs the machine name 
        /// </summary>
        private void StartMachineNameCollector()
        {
            // Skip permissions for CloudProviders
            MachineNameProbe machineProbe = new MachineNameProbe(connectionInfo, cloudProviderId);
            machineProbe.BeginProbe(MachineNameCallback);
        }

        /// <summary>
        /// Machine Name Collector Call back
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MachineNameCallback(object sender, SnapshotCompleteEventArgs e)
        {
            // This means we cancelled out the probe
            if (e.Result == Result.Failure)
                return;
            if (e.Snapshot != null)
            {
                var _machineSnapshot = e.Snapshot as MachineNameSnapshot;
                if (_machineSnapshot != null)
                    machineName = _machineSnapshot.MachineName;
            }

            ((IDisposable)sender).Dispose();

            // start the next probe 
            StartAgentStatusCollection();
        }
		//END SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm

        void StartAgentStatusCollection()
        {
            using (LOG.DebugCall("StartAgentStatusCollection"))
            {
                _wmiprobe = new ServiceStatusProbe(machineName, _wmiConfig);

                int i = serverName.IndexOf('\\');
                agentName = (i > 0) ? "SQLAGENT$" + serverName.Substring(i + 1) : "SQLSERVERAGENT";
                _wmiprobe.ServiceNames = new string[] {agentName};

                _wmiProbeIAR = _wmiprobe.BeginProbe(OnAgentStatusComplete);
            }
        }

        private void OnAgentStatusComplete(object sender, ProbeCompleteEventArgs args)
        {
            var services = args.Data as Dictionary<string, Service>;
            if (services != null)
            {
                foreach (var entry in services)
                {
                    if (entry.Key.Equals(agentName, StringComparison.CurrentCultureIgnoreCase)) 
                    {
                        agentJobSummary.AgentServiceState = entry.Value.RunningState;
                    }
                }
            }
            FireCompletion(agentJobSummary, Result.Success);
        }

        /// <summary>
        /// Interpret AgentJobSummary data
        /// </summary>
        private void InterpretAgentJobSummary(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretAgentJobSummary"))
            {
                try
                {
                   // could not find issys admin directly in probe so added following code snippet
bool isSysAdmin = false;
                    try
                    {
                        if (cloudProviderId == Constants.AmazonRDSId)
                        {
                            using (SqlConnection connection = new SqlConnection(connectionInfo.ConnectionString))
                            {
                                connection.Open();

                                using (SqlCommand command = connection.CreateCommand())
                                {
                                    command.CommandText = @"select is_srvrolemember('sysadmin') AS SysAdmin;";//SQLdm 10.1 (srishti purohit) -IsSysAdmin check
                                    using (var reader = command.ExecuteReader())
                                    {

                                        while (reader.Read())
                                        {
                                            isSysAdmin = Convert.ToBoolean(reader["SysAdmin"]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }


                    if (cloudProviderId == Constants.AmazonRDSId && !isSysAdmin && isBypass==true)
                    {
                        ReadJobSummaryHistory(dataReader);
                    }
                    else
                    {
                        ReadAgentStatus(dataReader);
                        if (_wmiConfig.DirectWmiEnabled)
                            ReadMachineName(dataReader);

                        ReadJobSummaryHistory(dataReader, ReadJobDurations(dataReader));
                    }

                    //    ReadAgentStatus(dataReader);
                    //if (_wmiConfig.DirectWmiEnabled)
                    //    ReadMachineName(dataReader);

                    //ReadJobSummaryHistory(dataReader, ReadJobDurations(dataReader));

                    
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(agentJobSummary, LOG, "Error interpreting Agent Job Summary Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(agentJobSummary, e);
                }
            }
        }

        private void ReadMachineName(SqlDataReader dataReader)
        {
            try
            {
                if (dataReader.Read())
                {
                    //machineName = dataReader.GetString(0);//START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe to get the machine name
                    serverName = dataReader.GetString(1);
                    tryDirectWmi = true;
                }
            }
            catch(Exception e)
            {
                tryDirectWmi = false;
            }
            finally
            {
                dataReader.NextResult();
            }
        }

        /// <summary>
        /// Reads agent status
        /// </summary>
        private void ReadAgentStatus(SqlDataReader dataReader)
        {
            // SQLdm 10.3 (Varun Chopra) Linux Support for Agent Service
            agentJobSummary.AgentServiceState = ProbeHelpers.ReadServiceState(dataReader, LOG, cloudProviderId);        
        }

        /// <summary>
        /// Create a hashtable out of the job durations
        /// </summary>
        private Hashtable ReadJobDurations(SqlDataReader dataReader)
        {
            Hashtable durations = new Hashtable();
            try
            {
                if (dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        // If the first record is "got job duration" then there are no durations to report, so move on
                        if (dataReader.GetValue(0).ToString().ToLower() != "got job duration")
                        {
                            // If there are durations, attempt to read them
                            ReadJobDurationsRecords(dataReader, ref durations);
                        }
                    }
                }

                return durations;
            }
            catch (Exception exception)
            {
                ProbeHelpers.LogAndAttachToSnapshot(agentJobSummary, LOG, "Error interpreting Agent Job Durations Collector: {0}", exception,
                                                        false);
                return durations;
            }
            finally
            {
                try
                {
                    /*
                    if(cloudProviderId==1)
                    {

                    }
                    else
                    {
                        dataReader.NextResult();
                    }*/
                    dataReader.NextResult();
                }
                catch (Exception e)
                {
                    LOG.Error("Error reading agent job durations.  Suppressing error.",e);
                }
            }
        }

        /// <summary>
        /// Read the durations from the recordset
        /// </summary>
        private void ReadJobDurationsRecords(SqlDataReader dataReader, ref Hashtable durations)
        {
            try
            {
                // Add the duration from the first read, skipping if the ID is null
                if (!dataReader.IsDBNull(0))
                {
                    SafeAdd(durations, dataReader.GetGuid(0), dataReader.GetInt32(1));
                }

                // Add each additional duration, skipping if the ID is null
                while (dataReader.Read())
                {
                    if (!dataReader.IsDBNull(0))
                    {
                        SafeAdd(durations, dataReader.GetGuid(0), dataReader.GetInt32(1));
                    }
                }
            }
            catch (Exception exception)
            {
                ProbeHelpers.LogAndAttachToSnapshot(agentJobSummary, LOG, "Error interpreting Agent Job Durations Collector: {0}", exception,
                                                       false);
            }

        }

        private static void SafeAdd(Hashtable durations, Guid guid, Int32 runtime)
        {
            if (!durations.ContainsKey(guid))
            {
                durations.Add(guid,runtime);
            }
        }

        /// <summary>
        /// Read the job history
        /// </summary>
        private void ReadJobSummaryHistory(SqlDataReader dataReader, Hashtable durations)
        {
            // Set up temporary variables
            AgentJob job;
            Guid currentJob = System.Guid.Empty;
            TimeSpan utcOffset = agentJobSummary.TimeStamp.Value.Subtract(agentJobSummary.TimeStampLocal.Value);

            while (dataReader.Read())
            {
                try
                {
                    if (!dataReader.IsDBNull(0) && dataReader.GetValue(0).ToString().ToLower() == "got job duration")
                    {
                        dataReader.NextResult();
                    }
                    else
                        // Skip the record if there is no name or no job id
                        if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(2))
                        {
                            currentJob = dataReader.GetGuid(0);

                            // Set up a new job
                            job = new AgentJob(dataReader.GetString(2));

                            if (!dataReader.IsDBNull(21)) job.LastRunStatus = AgentJob.ConvertToJobStepRunStatus(dataReader.GetInt32(21));

                            // Apply filtering for failed jobs
                            if (config.JobSummaryFilter == AgentJobSummaryConfiguration.JobSummaryFilterType.Failed)
                            {
                                if (job.LastRunStatus != JobStepRunStatus.Failed)
                                {
                                    continue;
                                }
                            }

                            // Read last run date
                            if ((!dataReader.IsDBNull(19)) && (!dataReader.IsDBNull(20)))
                            {
                                DateTime? localRunDate =
                                    ProbeHelpers.CombineTwoFieldTimeStamp(dataReader.GetInt32(19),
                                                                          dataReader.GetInt32(20));

                                if (localRunDate.HasValue)
                                {
                                    job.LastRunStartTime = localRunDate + utcOffset;
                                }
                            }

                            job.Status = AgentJob.ConvertToRunStatus(dataReader.IsDBNull(25) ? 0 : dataReader.GetInt32(25));

                            // Apply filtering for last run date - never filter out currently running
                            if (config.FilterTimeSpan.HasValue && job.Status != JobRunStatus.Executing)
                            {
                                if (job.LastRunStartTime < DateTime.UtcNow - config.FilterTimeSpan.Value)
                                {
                                    continue;
                                }
                            }

                            // Set properties on the job
                            if (!dataReader.IsDBNull(6)) job.Category= dataReader.GetString(6);
                            if (!dataReader.IsDBNull(4)) job.JobDescription = dataReader.GetString(4);
                            if (!dataReader.IsDBNull(1)) job.OriginatingServer = dataReader.GetString(1);
                            job.Enabled= (dataReader.IsDBNull(3) ? false : Convert.ToInt32(dataReader.GetValue(3)) > 0 ? true : false);
                            job.JobId = (currentJob);

                            if ((!dataReader.IsDBNull(22)) && (!dataReader.IsDBNull(23)))
                            {
                                DateTime? localRunDate = 
                                    ProbeHelpers.CombineTwoFieldTimeStamp(dataReader.GetInt32(22),
                                                                          dataReader.GetInt32(23));
                                if (localRunDate.HasValue)
                                {
                                    job.NextRunDate = localRunDate + utcOffset;
                                }
                            }


                            job.Owner= (dataReader.IsDBNull(7) ? "" : dataReader.GetString(7));
                            job.RetryAttempt= (dataReader.IsDBNull(27) ? -1 : dataReader.GetInt32(27));
                            job.Scheduled= (dataReader.IsDBNull(29) ? false : Convert.ToInt32(dataReader.GetValue(29)) > 0 ? true : false);
                            job.Version= (dataReader.IsDBNull(18) ? -1 : dataReader.GetInt32(18));
                            

                            

                            // Look for the job duration in the hashtable populated earlier.  If it is not found, default to 0
                            if (durations.ContainsKey(currentJob))
                            {
                                int inputDuration = (int)durations[dataReader.GetGuid(0)];
                                job.RunDuration = ProbeHelpers.TimeSpanFromHHMMSS(inputDuration);
                            }

                            agentJobSummary.Jobs.Add(job.JobId,job);
                        }
                }
                catch
                {
                    break;
                }
            }
        }

        private void ReadJobSummaryHistory(SqlDataReader dataReader)
        {
            // Set up temporary variables
            AgentJob job;
            Guid currentJob = System.Guid.Empty;
            TimeSpan utcOffset = agentJobSummary.TimeStamp.Value.Subtract(agentJobSummary.TimeStampLocal.Value);

            while (dataReader.Read())
            {
                try
                {
                    if (!dataReader.IsDBNull(0) && dataReader.GetValue(0).ToString().ToLower() == "got job duration")
                    {
                        //dataReader.NextResult();
                    }
                    else
                        // Skip the record if there is no name or no job id
                        if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(2))
                    {
                        currentJob = dataReader.GetGuid(0);

                        // Set up a new job
                        job = new AgentJob(dataReader.GetString(2));

                        if (!dataReader.IsDBNull(21)) job.LastRunStatus = AgentJob.ConvertToJobStepRunStatus(dataReader.GetInt32(21));

                        
                        // Apply filtering for failed jobs
                        if (config.JobSummaryFilter == AgentJobSummaryConfiguration.JobSummaryFilterType.Failed)
                        {
                            if (job.LastRunStatus != JobStepRunStatus.Failed)
                            {
                                continue;
                            }
                        }

                        // Read last run date
                        if ((!dataReader.IsDBNull(22)) && (!dataReader.IsDBNull(23)))
                        {
                            DateTime? localRunDate =
                                ProbeHelpers.CombineTwoFieldTimeStamp(dataReader.GetInt32(22),
                                                                      dataReader.GetInt32(23));

                            if (localRunDate.HasValue)
                            {
                                job.LastRunStartTime = localRunDate + utcOffset;
                            }
                        }

                        //job.Status = AgentJob.ConvertToRunStatus(dataReader.IsDBNull(25) ? 0 : dataReader.GetInt32(25));

                        if (!dataReader.IsDBNull(16))
                        {
                            job.Status = AgentJob.ConvertToRunStatus(4);
                        }
                        else
                        {
                            job.Status = AgentJob.ConvertToRunStatus(1);
                        }

                        // Apply filtering for last run date - never filter out currently running
                        if (config.FilterTimeSpan.HasValue && job.Status != JobRunStatus.Executing)
                        {
                            if (job.LastRunStartTime < DateTime.UtcNow - config.FilterTimeSpan.Value)
                            {
                                continue;
                            }
                        }

                        // Set properties on the job
                        if (!dataReader.IsDBNull(6)) job.Category = dataReader.GetString(6);
                        if (!dataReader.IsDBNull(4)) job.JobDescription = dataReader.GetString(4);
                        //if (!dataReader.IsDBNull(1)) job.OriginatingServer = dataReader.GetString(1);
                        job.Enabled = (dataReader.IsDBNull(3) ? false : Convert.ToInt32(dataReader.GetValue(3)) > 0 ? true : false);
                        job.JobId = (currentJob);

                        //if ((!dataReader.IsDBNull(22)) && (!dataReader.IsDBNull(23)))
                        //{
                        //    DateTime? localRunDate =
                        //        ProbeHelpers.CombineTwoFieldTimeStamp(dataReader.GetInt32(22),
                        //                                              dataReader.GetInt32(23));
                        //    if (localRunDate.HasValue)
                        //    {
                        //        job.NextRunDate = localRunDate + utcOffset;
                        //    }
                        //}

                        if (!dataReader.IsDBNull(20))
                        {
                            job.NextRunDate = Convert.ToDateTime(dataReader.GetDateTime(20));
                        }

                        if(job.NextRunDate!=null)
                        {
                            job.Scheduled = true;
                        }
                        else
                        {
                            job.Scheduled = false;
                        }

                        job.Owner = (dataReader.IsDBNull(25) ? "" : dataReader.GetString(25));
                        //job.RetryAttempt = (dataReader.IsDBNull(27) ? -1 : dataReader.GetInt32(27));
                        //job.Scheduled = (dataReader.IsDBNull(29) ? false : Convert.ToInt32(dataReader.GetValue(29)) > 0 ? true : false);
                        //job.Version = (dataReader.IsDBNull(18) ? -1 : dataReader.GetInt32(18));


                        if (!dataReader.IsDBNull(24))
                        {
                            job.RunDuration = ProbeHelpers.TimeSpanFromHHMMSS(dataReader.GetInt32(24));
                        }


                            // Look for the job duration in the hashtable populated earlier.  If it is not found, default to 0
                            /*if (durations.ContainsKey(currentJob))
                            {
                                int inputDuration = (int)durations[dataReader.GetGuid(0)];
                                job.RunDuration = ProbeHelpers.TimeSpanFromHHMMSS(inputDuration);
                            }*/

                            agentJobSummary.Jobs.Add(job.JobId, job);
                    }
                }
                catch
                {
                    break;
                }
            }
        }

        #endregion

        #region interface implementations

        #endregion
    }
}
