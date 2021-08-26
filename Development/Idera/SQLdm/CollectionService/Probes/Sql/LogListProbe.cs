//------------------------------------------------------------------------------
// <copyright file="LogListProbe.cs" company="Idera, Inc.">
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
    /// List SQL and Agent Logs
    /// </summary>
    internal sealed class LogListProbe : SqlBaseProbe
    {
        #region fields

        private LogFileList logList = null;
        private MonitoredServerWorkload workload; // SQLdm 8.6 (Ankit Srivastava) -- Added workload field - solved defect DE43661

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LogListProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public LogListProbe(SqlConnectionInfo connectionInfo,MonitoredServerWorkload workload, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("LogListProbeProbe");
            logList = new LogFileList(connectionInfo);
            this.workload = workload; // SQLdm 8.6 (Ankit Srivastava) -- Added workload field - solved defect DE43661
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
            StartLogListCollector();
        }

        /// <summary>
        /// Define the LogList collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void LogListCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildLogListCommand(conn, ver,workload);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(LogListCallback));
        }

        /// <summary>
        /// Starts the Log List collector.
        /// </summary>
        private void StartLogListCollector()
        {
            StartGenericCollector(new Collector(LogListCollector), logList, "StartLogListCollector", "Log List", LogListCallback, new object[] { });
        }

        /// <summary>
        /// Define the LogList callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void LogListCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretLogList(rd);
            }
            FireCompletion(logList, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the LogList collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void LogListCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(LogListCallback), logList, "LogListCallback", "Log List",
                            sender, e);
        }

        /// <summary>
        /// Interpret LogList data
        /// </summary>
        private void InterpretLogList(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretLogList"))
            {
                try
                {
                    ReadSqlAgentLogList(dataReader);
                    ReadSqlServerLogList(dataReader);
                    ReadNumberOfErrorLogs(dataReader);
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logList, LOG, "Error interpreting Log List Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(logList);
                }
            }
        }

        /// <summary>
        /// Interpret the SQL Server log list for any version of SQL Server
        /// </summary>
        private void ReadSqlServerLogList(SqlDataReader dataReader)
        {
            try
            {
                while (dataReader.Read())
                {
                    LogFile logFileItem = new LogFile();
                    logFileItem.LogType = LogFileType.SQLServer;

                    if (!dataReader.IsDBNull(0)) logFileItem.Number = dataReader.GetInt32(0);
                    if (!dataReader.IsDBNull(1))
                    {
                        if (logList.ProductVersion.Major == 8)
                        {
                            DateTime itemTime = System.DateTime.MinValue;
                            DateTime.TryParse(dataReader.GetValue(1).ToString(), out itemTime);
                            if (itemTime > DateTime.MinValue)
                            {
                                logFileItem.LastModified = itemTime;
                            }
                        }
                        else
                        {
                            logFileItem.LastModified = dataReader.GetDateTime(1); 
                        }
                    }
                    if (!dataReader.IsDBNull(2)) logFileItem.LogFileSize.Bytes = dataReader.GetInt32(2);

                    logFileItem.Name = logFileItem.Number > 0 ? "ERRORLOG." + logFileItem.Number : "ERRORLOG";
                    logList.LogList.Add(logFileItem);
                }
            }
            catch (Exception exception)
            {
                ProbeHelpers.LogAndAttachToSnapshot(logList, LOG, "Interpret SQL log list failed", exception,
                                                    true);
            }
            finally
            {
                dataReader.NextResult();
            }
        }

        private void ReadSqlAgentLogList(SqlDataReader dataReader)
        {
            try
            {
                // Read name of Agent log
                string logName = "SQLAGENT.OUT";
                if (dataReader.Read())
                {
                    if (!dataReader.IsDBNull(0)) logName = dataReader.GetString(0).ToLower();
                    dataReader.NextResult();
                    // Read list of agent logs
                    while (dataReader.Read())
                    {
                        LogFile logFileItem = new LogFile();
                        logFileItem.LogType = LogFileType.Agent;
                        if (!dataReader.IsDBNull(0)) logFileItem.Number = dataReader.GetInt32(0);
                        if (!dataReader.IsDBNull(1)) logFileItem.LastModified = dataReader.GetDateTime(1);
                        if (!dataReader.IsDBNull(2)) logFileItem.LogFileSize.Bytes = dataReader.GetInt32(2);

                        if (logList.ProductVersion.Major == 8)
                        {
                            string agentLogFileName = "";
                            if (!dataReader.IsDBNull(3)) agentLogFileName = dataReader.GetString(3);
                            if(!String.IsNullOrEmpty(agentLogFileName))
                            {
                                //trim out the path
                                logFileItem.Name = agentLogFileName.Substring(agentLogFileName.LastIndexOf("\\") + 1);
                            }
                        }
                        if (logFileItem.Name == null)
                        {
                            logFileItem.Name = logFileItem.Number > 0
                                                   ? logName.Substring(0, logName.LastIndexOf(".")) + "." +
                                                     logFileItem.Number
                                                   : logName;
                        }
                        logList.LogList.Add(logFileItem);
                    }
                }
                else
                {
                    LOG.Warn("No data returned for ReadSqlAgentLogList");
                }
            }
            catch (Exception exception)
            {
                ProbeHelpers.LogAndAttachToSnapshot(logList, LOG, "Interpret SQL Agent log list failed", exception,
                                                    true);
            }
            finally
            {
                dataReader.NextResult();
            }

        }


        

        private void ReadNumberOfErrorLogs(SqlDataReader dataReader)
        {
            if (dataReader.Read())
            {
                if (!dataReader.IsDBNull(0)) logList.MaximumSqlLogs = dataReader.GetInt32(0);
            }
        }
        #endregion

        #region interface implementations

        #endregion
    }
}
