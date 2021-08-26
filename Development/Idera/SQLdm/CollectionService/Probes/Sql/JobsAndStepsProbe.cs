//------------------------------------------------------------------------------
// <copyright file="JobsAndStepsProbe.cs" company="Idera, Inc.">
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
    using Common.Snapshots;
    using Common.Services;


    /// <summary>
    /// Enter a description for this class
    /// </summary>
    internal sealed class JobsAndStepsProbe : SqlBaseProbe
    {
        #region fields

        private JobsAndStepsSnapshot jobsAndStepsSnapshot = null;
        private JobsAndStepsConfiguration config = null;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of JobsAndStepsProbe
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <param name="config"></param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public JobsAndStepsProbe(SqlConnectionInfo connectionInfo, JobsAndStepsConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            LOG = Logger.GetLogger("JobsAndStepsProbe");
            jobsAndStepsSnapshot = new JobsAndStepsSnapshot(connectionInfo);
            this.config = config;
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
                StartJobsAndStepsCollector();
            }
            else
            {
                FireCompletion(jobsAndStepsSnapshot, Result.Success);
            }
        }

        /// <summary>
        /// Define the Jobs And Steps collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void JobsAndStepsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildJobsAndStepsCommand(conn, ver, config);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(JobsAndStepsCallback));
        }

        /// <summary>
        /// Starts the Jobs And Steps collector.
        /// </summary>
        private void StartJobsAndStepsCollector()
        {
            StartGenericCollector(new Collector(JobsAndStepsCollector), jobsAndStepsSnapshot, "StartJobsAndStepsCollector", "Jobs and Steps", null, new object[] { });
        }

        /// <summary>
        /// Define the StartJobsAndSteps callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void JobsAndStepsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                if (e.ElapsedMilliseconds == null)
                {
                    jobsAndStepsSnapshot.Duration = null;
                }
                else
                {
                    jobsAndStepsSnapshot.Duration = TimeSpan.FromMilliseconds(e.ElapsedMilliseconds.Value);
                }
                
                InterpretJobsAndSteps(rd);
            }
            FireCompletion(jobsAndStepsSnapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the StartJobsAndSteps collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void JobsAndStepsCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(JobsAndStepsCallback), jobsAndStepsSnapshot, "JobsAndStepsCallback", "Jobs and Steps",
                            sender, e);
        }

        /// <summary>
        /// Interpret JobsAndSteps data
        /// </summary>
        private void InterpretJobsAndSteps(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretJobsAndSteps"))
            {
                try
                {
                    string currentJobOrStep = String.Empty;

                    jobsAndStepsSnapshot.JobsAndSteps.Clear();
                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            if (config.IsSelectedJobMode)
                            {
                                if (!dataReader.IsDBNull(2)) currentJobOrStep = dataReader.GetString(2).TrimEnd();
                            }
                            else
                            {
                                if (!dataReader.IsDBNull(1)) currentJobOrStep = dataReader.GetString(1).TrimEnd();
                            }
                            jobsAndStepsSnapshot.JobsAndSteps.Add(currentJobOrStep);
                            
                        } 
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(jobsAndStepsSnapshot, LOG, "Error interpreting Jobs And Steps Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(jobsAndStepsSnapshot);
                }
            }
        }
        #endregion

        #region interface implementations

        #endregion
    }
}
