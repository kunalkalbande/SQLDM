//------------------------------------------------------------------------------
// <copyright file="ErrorLogProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using Idera.SQLdm.Common.Events;

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
    using Idera.SQLdm.CollectionService.Monitoring;

    /// <summary>
    /// Enter a description for this class
    /// </summary>
    internal sealed class ErrorLogProbe : SqlBaseProbe, IOnDemandProbe
    {
        #region fields
        private const int CancelCheckPollInterval = 2500;
        private ErrorLog errorLog = null;
        private ErrorLogConfiguration config = null;
        private MonitoredServerWorkload workload;

        //private static Regex errorLineRegex = new Regex(@"warning|failed|!(cycle )errorlog|access_violation", RegexOptions.IgnoreCase);
        private static Regex messageNumberPattern = new Regex(@"(?<=\[)[0-9]+(?=\])");
        private static Regex messageTypePattern = new Regex(@"(?<=-\s)[?+!](?=\s\[)");
        private Regex ErrorLogSeverity = new Regex(@"(?<=Severity:\s+)[0-9]+");

        private List<Regex> errorLogRegexCritical = new List<Regex>();
        private List<Regex> errorLogRegexWarning = new List<Regex>();
        private List<Regex> errorLogRegexInfo = new List<Regex>();
        private AdvancedAlertConfigurationSettings errorLogAdvancedSettings = null;

        List<Regex> agentLogRegexCritical = new List<Regex>();
        List<Regex> agentLogRegexWarning = new List<Regex>();
        List<Regex> agentLogRegexInfo = new List<Regex>();
        AdvancedAlertConfigurationSettings agentLogAdvancedSettings = null;

        IOnDemandContext context;
        int recordsRead = 0;

        // DateTime of last record passed to AddToMessageCollection.
        private DateTime lastDateTime;

        // Count of records passed to AddToMessageCollection that have the
        // same DateTime as lastDateTime.
        private int duplicateDateTimes = 0;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorLogProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public ErrorLogProbe(SqlConnectionInfo connectionInfo, ErrorLogConfiguration config,MonitoredServerWorkload workload, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("ErrorLogProbe");
            errorLog = new ErrorLog(connectionInfo);
            this.config = config;
            this.cloudProviderId = cloudProviderId;
            List <LogFile> logfilelist = new List<LogFile>();

            foreach (LogFile logfile in config.LogFiles)
            {
                if (!(logfile.LastModified < this.config.StartDate))
                    logfilelist.Add(logfile);
            }

            this.config.LogFiles = logfilelist;

            this.workload = workload;

            if ((config.InternalEndDate.HasValue) || (config.InternalStartDate.HasValue))
                errorLog.HasBeenInternallyFiltered = true;
        }

        #endregion

        #region properties

        private bool IsCancelled
        {
            get { return context != null ? context.IsCancelled : false; }
        }

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
                StartErrorLogCollector();
            }
            else
            {
                FireCompletion(errorLog, Result.Success);
            }
        }

        /// <summary>
        /// Define the ErrorLog collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void ErrorLogCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildErrorLogCommand(conn, ver, config);
            if(cloudProviderId==Constants.AmazonRDSId)
                cmd=
                SqlCommandBuilder.BuildRDSErrorLogCommand(conn, ver, config);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ErrorLogCallback));
        }

        /// <summary>
        /// Starts the Error Log collector.
        /// </summary>
        private void StartErrorLogCollector()
        {
            StartGenericCollector(new Collector(ErrorLogCollector), errorLog, "StartErrorLogCollector", "Error Log", ErrorLogCallback, new object[] { });
        }

        /// <summary>
        /// Define the ErrorLog callback
        /// </summary>
        private void ErrorLogCallback(CollectorCompleteEventArgs args)
        {
            try
            {
                using (SqlDataReader rd = args.Value as SqlDataReader)
                {
                    InterpretErrorLog(rd);
                }

                FireCompletion(errorLog, Result.Success);
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(errorLog, LOG, "Error interpreting Error Log Collector: {0}",
                                                        e,
                                                        false);

                // if the operation was cancelled then clear out any data that was collected.
                if (e is OperationCanceledException)
                    errorLog.Clear();

                GenericFailureDelegate(errorLog);
            }
        }

        /// <summary>
        /// Callback used to process the data returned from the ErrorLog collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ErrorLogCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(ErrorLogCallback), errorLog, "ErrorLogCallback", "Error Log",
                            sender, e);
        }

        /// <summary>
        /// Interpret ErrorLog data
        /// </summary>
        private void InterpretErrorLog(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretErrorLog"))
            {
                errorLogAdvancedSettings =
                        ScheduledCollectionEventProcessor.GetAdvancedConfiguration(
                            workload.Thresholds[(int)Metric.ErrorLog]);
                
                if (workload.Thresholds[(int)Metric.ErrorLog].IsEnabled)
                {
                    if (errorLogAdvancedSettings.LogIncludeRegexCritical != null)
                        foreach (String regexstr in errorLogAdvancedSettings.LogIncludeRegexCritical)
                        {
                            errorLogRegexCritical.Add(new Regex(regexstr));
                        }
                    if (errorLogAdvancedSettings.LogIncludeRegexWarning != null)
                        foreach (String regexstr in errorLogAdvancedSettings.LogIncludeRegexWarning)
                        {
                            errorLogRegexWarning.Add(new Regex(regexstr));
                        }
                    if (errorLogAdvancedSettings.LogIncludeRegexInfo != null)
                        foreach (String regexstr in errorLogAdvancedSettings.LogIncludeRegexInfo)
                        {
                            errorLogRegexInfo.Add(new Regex(regexstr));
                        }
                }
                
                agentLogAdvancedSettings =
                        ScheduledCollectionEventProcessor.GetAdvancedConfiguration(
                            workload.Thresholds[(int)Metric.AgentLog]);
                
                if (agentLogAdvancedSettings != null && workload.Thresholds[(int)Metric.AgentLog].IsEnabled)
                {
                    if (agentLogAdvancedSettings.LogIncludeRegexCritical != null)
                        foreach (String regexstr in agentLogAdvancedSettings.LogIncludeRegexCritical)
                        {
                            agentLogRegexCritical.Add(new Regex(regexstr));
                        }
                    if (agentLogAdvancedSettings.LogIncludeRegexWarning != null)
                        foreach (String regexstr in agentLogAdvancedSettings.LogIncludeRegexWarning)
                        {
                            agentLogRegexWarning.Add(new Regex(regexstr));
                        }
                    if (agentLogAdvancedSettings.LogIncludeRegexInfo != null)
                        foreach (String regexstr in agentLogAdvancedSettings.LogIncludeRegexInfo)
                        {
                            agentLogRegexInfo.Add(new Regex(regexstr));
                        }
                }

                if (IsCancelled)
                    throw new OperationCanceledException("Error log probe cancelled by management service.");

                // Read the log for each of the requested log files
                foreach (LogFile logFile in config.LogFiles)
                {
                    if (recordsRead % CancelCheckPollInterval == 0 && IsCancelled)
                    {
                        throw new OperationCanceledException("Error log probe cancelled by management service.");
                    }

                    if (logFile.LogType.HasValue)
                    {
                        // Read SQL Server log
                        if (logFile.LogType.Value == LogFileType.SQLServer)
                        {
                            if (errorLog.ProductVersion.Major >= 9) // Use same method for reading SQL Server 2005/2008
                            {
                                ReadMessages2005(dataReader, logFile);
                            }
                            else
                            {
                                ReadMessages2000(dataReader, logFile.Name);
                            }
                        }
                        else
                        {
                            // Read SQL Agent log
                            // If the fieldcount is 1 or less, the data is not available
                            if (dataReader.FieldCount > 1)
                            {
                                if (errorLog.ProductVersion.Major >= 9) // Use same method for reading SQL Server 2005/2008
                                {
                                    ReadSqlAgentLogLines2005(dataReader, logFile);
                                }
                                else
                                {
                                    ReadSqlAgentLogLines2000(dataReader, logFile);
                                }
                            }
                            else
                            {
                                // Warn if the agent log was unreadable
                                ProbeHelpers.LogAndAttachToSnapshot(errorLog, LOG,
                                                                    "A SQL Server Agent Log (" + logFile.Name + ") was unavailable on " +
                                                                    errorLog.ServerName, true);
                            }
                        }
                        dataReader.NextResult();
                    }
                }
            }
        }

        private void AddToMessageCollection(DataRow dr)
        {
            using (LOG.DebugCall("AddToMessageCollection"))
            {
                DateTime localTime = Convert.ToDateTime(dr["Local Time"]);

                // Apply filters to message
                if (config.InternalEndDate.HasValue)
                {
                    if (localTime >= config.InternalEndDate.Value)
                        return;
                }
                if (config.InternalStartDate.HasValue)
                {
                    if (localTime <= config.InternalStartDate.Value)
                        return;
                }
                if (config.EndDate.HasValue)
                {
                    if (localTime > config.EndDate.Value)
                        return;
                }
                if (config.StartDate.HasValue)
                {
                    if (localTime < config.StartDate.Value)
                        return;
                }
                if (config.Source != null)
                {
                    if ((string)dr["Source"] != config.Source)
                        return;
                }
                if (config.ShowErrors == false)
                {
                    if ((MonitoredState)dr["MessageType"] == MonitoredState.Critical)
                        return;
                }
                if (config.ShowWarnings == false)
                {
                    if ((MonitoredState)dr["MessageType"] == MonitoredState.Warning)
                        return;
                }
                if (config.ShowInformational == false)
                {
                    if (((MonitoredState)dr["MessageType"] == MonitoredState.OK) || ((MonitoredState)dr["MessageType"] == MonitoredState.Informational))
                        return;
                }

                // If multiple records have identical DateTimes, increment the duplicates by
                // one tick so they are no longer identical.
                // For PR 2010993 "Logs: Grid sorts data incorrectly when data is sorted by Date (descending)" 
                if (localTime == lastDateTime)
                {
                    ++duplicateDateTimes;
                    localTime = localTime.AddTicks(duplicateDateTimes);
                    dr["Local Time"] = localTime;
                }
                else
                {
                    lastDateTime = localTime;
                    duplicateDateTimes = 0;
                }

                // Set the max and minimum datetimes
                if (!errorLog.EarliestDate.HasValue || errorLog.EarliestDate.Value > localTime)
                {
                    errorLog.EarliestDate = localTime;
                }

                if (!errorLog.LatestDate.HasValue || errorLog.LatestDate.Value < localTime)
                {
                    errorLog.LatestDate = localTime;
                }

                errorLog.AddRow(dr);
            }
        }


        private void ReadMessages2005(SqlDataReader dataReader, LogFile logfile)
        {
            using (LOG.DebugCall("ReadMessages2005"))
            {
                try
                {
                    while (dataReader.Read())
                    {
                        if (++recordsRead % CancelCheckPollInterval == 0 && IsCancelled)
                        {
                            throw new OperationCanceledException("Error log probe cancelled by management service.");
                        }

                        DataRow dr = errorLog.NewRow();
                        dr["Log Source"] = LogFileType.SQLServer;
                        dr["Log Name"] = logfile.Name;
                        if (!dataReader.IsDBNull(0)) dr["Local Time"] = Convert.ToDateTime(dataReader.GetDateTime(0));
                        if (!dataReader.IsDBNull(1)) dr["Source"] = Convert.ToString(dataReader.GetString(1));
                        if (!dataReader.IsDBNull(2))
                        {
                            string buffer = Convert.ToString(dataReader.GetString(2));
                            LOG.DebugFormat("Local Time: {0} - Source: {1} - Message: {2}", dr["Local Time"], dr["Source"], buffer);
                            ProbeHelpers.ParseMessageType(dr,
                                                          buffer,
                                                          errorLogRegexCritical,
                                                          errorLogRegexWarning,
                                                          errorLogRegexInfo,
                                                          errorLogAdvancedSettings, ErrorLogSeverity, workload);

                            // if the buffer starts with a carraigeReturnLineFeed then skip that cos it 
                            // makes it look bad in the grid
                            dr["Message"] = (buffer.StartsWith("\r\n") & buffer.Length > 2) ? buffer.Substring(2) : buffer;

                        }
                        AddToMessageCollection(dr);
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(errorLog, LOG, "Error reading SQL 2005 SQL Server log", exception,
                                                        false);
                }
            }
        }

        private void ReadMessages2000(SqlDataReader dataReader, string logName)
        {
            //Set up buffer2 to handle multiple-line entries
            string outputBuffer = "";
            string inputBuffer = "";

            //Set up space location variables
            bool firstLoop = true;
            int spaceLocation = 0;

            //Set up temporary date parsing variable
            DateTime? inputTimeStamp = null;
            DateTime? outputTimeStamp = null;

            try
            {
                while (dataReader.Read())
                {
                    if (++recordsRead % CancelCheckPollInterval == 0 && IsCancelled)
                    {
                        throw new OperationCanceledException("Error log probe cancelled by management service.");
                    }

                    //Skip null lines
                    if (!dataReader.IsDBNull(0))
                    {
                        inputBuffer = dataReader.GetString(0);

                        //To keep from having to check for the date type on every line, we will check here to see where the
                        //log line object needs to start to parse out the date 
                        if (firstLoop)
                        {
                            spaceLocation = inputBuffer.IndexOf(" ") + 1;
                            spaceLocation += inputBuffer.Substring(spaceLocation).IndexOf(" ");
                            firstLoop = false;
                        }

                        //Check to see if this is a ContinuationRow and if so, append to previous input but do not save (yet)
                        if (!dataReader.IsDBNull(1) && (Convert.ToInt16(dataReader.GetValue(1)) > 0))
                        {
                            outputBuffer += inputBuffer + '\n';
                        }
                        else
                        {
                            inputTimeStamp = ProbeHelpers.ParseSqlServerLogTimeStamp2000(ref inputBuffer, spaceLocation);
                            //SQL Server 7 does not appear to utilize the ContinuationRow field, and SQL Server 2000 uses it inconsistently,
                            //so check to see if the date returned by the parse is valid.  If not, throw it away and append the message to
                            //the output buffer
                            if (inputTimeStamp == System.DateTime.MinValue)
                            {
                                outputBuffer += inputBuffer + '\n';
                            }
                            else
                            {
                                //If the current row is not a continuation row, you may safely save the output buffer
                                //Do not save empty buffers
                                if (outputBuffer != null && outputBuffer.Length > 0)
                                {
                                    DataRow dr = errorLog.NewRow();
                                    dr["Log Source"] = LogFileType.SQLServer;
                                    dr["Log Name"] = logName;
                                    dr["Local Time"] = outputTimeStamp;
                                    ProbeHelpers.ParseMessage2000(dr, outputBuffer, errorLogRegexCritical,
                                                     errorLogRegexWarning, errorLogRegexInfo,
                                                     errorLogAdvancedSettings, ErrorLogSeverity, workload);

                                    AddToMessageCollection(dr);
                                }

                                //Begin a new buffer to hold the current row and possibly append continuation rows as the loop continues
                                outputBuffer = inputBuffer;
                                outputTimeStamp = inputTimeStamp;
                            }
                        }
                    }
                }
                //This is a final check for anything left in the input buffer at the end of the loop, in case the last
                //line was a continuation row
                if (inputBuffer != null && inputBuffer.Length > 0)
                {
                    DataRow dr = errorLog.NewRow();
                    dr["Log Source"] = LogFileType.SQLServer;
                    dr["Log Name"] = logName;
                    dr["Local Time"] = outputTimeStamp;
                    ProbeHelpers.ParseMessage2000(dr, outputBuffer, errorLogRegexCritical,
                                                     errorLogRegexWarning, errorLogRegexInfo,
                                                     errorLogAdvancedSettings, ErrorLogSeverity, workload);
                    AddToMessageCollection(dr);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                ProbeHelpers.LogAndAttachToSnapshot(errorLog, LOG, "Error reading SQL 2000 SQL Server log", exception,
                                                    false);
            }
        }

        private void ReadSqlAgentLogLines2005(SqlDataReader dataReader, LogFile logFile)
        {
            using (LOG.DebugCall("ReadSqlAgentLogLines2005"))
            {
                try
                {
                    while (dataReader.Read())
                    {
                        if (++recordsRead%CancelCheckPollInterval == 0 && IsCancelled)
                        {
                            throw new OperationCanceledException("Error log probe cancelled by management service.");
                        }

                        DataRow dr = errorLog.NewRow();
                        dr["Log Source"] = LogFileType.Agent;
                        dr["Log Name"] = logFile.Name;
                        dr["Source"] = "SQL Agent";
                        if (!dataReader.IsDBNull(0)) dr["Local Time"] = Convert.ToDateTime(dataReader.GetDateTime(0));

                        ErrorLogMessageType tempSeverity = ErrorLogMessageType.Informational;
                        if (!dataReader.IsDBNull(1) && workload.Thresholds[(int) Metric.AgentLog].IsEnabled)
                        {
                            switch (dataReader.GetString(1).TrimEnd())
                            {
                                case "2":
                                    tempSeverity = ErrorLogMessageType.Warning;
                                    break;
                                case "1":
                                    tempSeverity = ErrorLogMessageType.Error;
                                    break;
                                default:
                                    tempSeverity = ErrorLogMessageType.Informational;
                                    break;
                            }
                        }

                        string message = null;
                        if (!dataReader.IsDBNull(2)) message = Convert.ToString(dataReader.GetString(2));
                        dr["Message"] = ProbeHelpers.ParseAgentMessage(message);
                        dr["Message Number"] = ProbeHelpers.ParseMessageNumber(message, messageNumberPattern);

                        LOG.DebugFormat("Local Time: {0} - Source: {1} - Message: {2}", dr["Local Time"], dr["Source"], message);
                        MonitoredState severity = MonitoredState.None;
                        if (workload.Thresholds[(int) Metric.AgentLog].CriticalThreshold.Enabled && (Convert.ToInt32(tempSeverity) <= Convert.ToInt32(workload.Thresholds[(int) Metric.AgentLog].CriticalThreshold.Value)))
                        {
                            severity = MonitoredState.Critical;
                        }
                        else if (workload.Thresholds[(int) Metric.AgentLog].WarningThreshold.Enabled && (Convert.ToInt32(tempSeverity) <= Convert.ToInt32(workload.Thresholds[(int) Metric.AgentLog].WarningThreshold.Value)))
                        {
                            severity = MonitoredState.Warning;
                        }
                        else if (workload.Thresholds[(int) Metric.AgentLog].InfoThreshold.Enabled && (Convert.ToInt32(tempSeverity) <= Convert.ToInt32(workload.Thresholds[(int) Metric.AgentLog].InfoThreshold.Value)))
                        {
                            severity = MonitoredState.Informational;
                        }
                        else
                        {
                            if (workload.Thresholds[(int) Metric.AgentLog].IsEnabled)
                                ProbeHelpers.ErrorLogScanIsMatch(message, agentLogRegexCritical,
                                                                 agentLogRegexWarning, agentLogRegexInfo,
                                                                 agentLogAdvancedSettings, ref severity);
                        }

                        dr["MessageType"] = severity;
                        LOG.DebugFormat("The severity MessageType is : {0}", severity.ToString());
                        AddToMessageCollection(dr);
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(errorLog, LOG, "Error reading SQL 2005 Agent log", exception,
                                                        false);
                }
            }
        }

        private void ReadSqlAgentLogLines2000(SqlDataReader dataReader, LogFile logFile)
        {
            string inputBuffer = "";
            string outputBuffer = "";
            string inputTimeStamp = null;
            string outputTimeStamp = null;

            try
            {
                while (dataReader.Read())
                {
                    if (++recordsRead % CancelCheckPollInterval == 0 && IsCancelled)
                    {
                        throw new OperationCanceledException("Error log probe cancelled by management service.");
                    }

                    if (!dataReader.IsDBNull(0))
                    {
                        inputBuffer = dataReader.GetString(0);

                        //Check to see if this is a ContinuationRow and if so, append to previous input but do not save (yet)
                        if (!dataReader.IsDBNull(1) && (Convert.ToInt16(dataReader.GetValue(1)) > 0))
                        {
                            outputBuffer += inputBuffer + '\n';
                        }
                        else
                        {
                            inputTimeStamp = ProbeHelpers.ParseSqlAgentLogLineTimeStamp2000(inputBuffer);
                            //SQL Server 7 does not appear to utilize the ContinuationRow field, and SQL Server 2000 uses it inconsistently,
                            //so check to see if the date returned by the parse is valid.  If not, throw it away and append the message to
                            //the output buffer
                            if (inputTimeStamp == null)
                            {
                                outputBuffer += inputBuffer + '\n';
                            }
                            else
                            {
                                //If the current row is not a continuation row, you may safely save the output buffer
                                //Do not save empty buffers
                                if (outputBuffer != null && outputBuffer.Length > 0)
                                {
                                    DataRow dr = errorLog.NewRow();
                                    dr["Log Source"] = LogFileType.Agent;
                                    dr["Log Name"] = logFile.Name;
                                    dr["Source"] = "SQL Agent";
                                    dr["Message Number"] = ProbeHelpers.ParseMessageNumber(outputBuffer, messageNumberPattern);
                                    DateTime localTime;
                                    DateTime.TryParse(outputTimeStamp, out localTime);
                                    dr["Local Time"] = localTime;
                                    ErrorLogMessageType tempSeverity = ErrorLogMessageType.Informational;
                                    if (workload.Thresholds[(int)Metric.AgentLog].IsEnabled)
                                        tempSeverity = ProbeHelpers.ParseMessageType(outputBuffer, messageTypePattern);
                                    string message;
                                    message = ProbeHelpers.ParseAgentMessage(outputBuffer);
                                    dr["Message"] = message;

                                    if ((int)tempSeverity <= (int)workload.Thresholds[(int)Metric.AgentLog].CriticalThreshold.Value)
                                    {
                                        dr["MessageType"] = MonitoredState.Critical;
                                    }
                                    else
                                    {
                                        if ((int) tempSeverity <=
                                            (int) workload.Thresholds[(int) Metric.AgentLog].WarningThreshold.Value)
                                        {
                                            dr["MessageType"] = MonitoredState.Warning;
                                        }
                                        else
                                        {
                                            MonitoredState severity = MonitoredState.None;
                                            if (workload.Thresholds[(int)Metric.AgentLog].IsEnabled)
                                                ProbeHelpers.ErrorLogScanIsMatch(message, agentLogRegexCritical, agentLogRegexWarning, agentLogRegexInfo, agentLogAdvancedSettings, ref severity);
                                            dr["MessageType"] = severity;
                                        }
                                    }
                                    
                                    
                                    AddToMessageCollection(dr);
                                }

                                //Begin a new buffer to hold the current row and possibly append continuation rows as the loop continues
                                outputBuffer = inputBuffer;
                                outputTimeStamp = inputTimeStamp;
                            }
                        }
                    }
                }
                //This is a final check for anything left in the input buffer at the end of the loop, in case the last
                //line was a continuation row
                if (inputBuffer != null && inputBuffer.Length > 0)
                {
                    DataRow dr = errorLog.NewRow();
                    dr["Log Source"] = LogFileType.Agent;
                    dr["Log Name"] = logFile.Name;
                    dr["Source"] = "SQL Agent";
                    dr["Message Number"] = ProbeHelpers.ParseMessageNumber(outputBuffer, messageNumberPattern);
                    DateTime localTime;
                    DateTime.TryParse(outputTimeStamp, out localTime);
                    dr["Local Time"] = localTime;
                    ErrorLogMessageType tempSeverity = ErrorLogMessageType.Informational;
                    if (workload.Thresholds[(int)Metric.AgentLog].IsEnabled)
                        tempSeverity = ProbeHelpers.ParseMessageType(outputBuffer, messageTypePattern);
                    string message;
                    message = ProbeHelpers.ParseAgentMessage(outputBuffer);
                    dr["Message"] = message;

                    if ((int)tempSeverity <= (int)workload.Thresholds[(int)Metric.AgentLog].CriticalThreshold.Value)
                    {
                        dr["MessageType"] = MonitoredState.Critical;
                    }
                    else
                    {
                        if ((int)tempSeverity <=
                            (int)workload.Thresholds[(int)Metric.AgentLog].WarningThreshold.Value)
                        {
                            dr["MessageType"] = MonitoredState.Warning;
                        }
                        else
                        {
                            MonitoredState severity = MonitoredState.None;
                            if (workload.Thresholds[(int)Metric.AgentLog].IsEnabled)
                                ProbeHelpers.ErrorLogScanIsMatch(message, agentLogRegexCritical, agentLogRegexWarning, agentLogRegexInfo, agentLogAdvancedSettings, ref severity);
                            dr["MessageType"] = severity;
                        }
                    }
                                    
                    AddToMessageCollection(dr);
                }
            }
            catch (OperationCanceledException )
            {
                throw;
            }
            catch (Exception exception)
            {
                ProbeHelpers.LogAndAttachToSnapshot(errorLog, LOG, "Error reading SQL 2000 Agent log", exception,
                                                    false);
            }
        }
        
     
        #endregion

        #region interface implementations

        #endregion

        #region IOnDemandProbe Members

        public IOnDemandContext Context
        {
            get
            {
                return context;
            }
            set
            {
                this.context = value;
            }
        }

        #endregion
    }
}
