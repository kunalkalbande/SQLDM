//------------------------------------------------------------------------------
// <copyright file="SessionDetailsProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Text.RegularExpressions;

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
    internal sealed class SessionDetailsProbe : SqlBaseProbe
    {
        #region fields

        private SessionDetailSnapshot sessionDetails = null;
        private SessionDetailsConfiguration config = null;

        #region Regexes
        // Regular expressions for DBCC PSS

        // Most of these regexes are in the form (?<=prowcount\s*=\s*)-*\d+
        // This is a prefix lookaround in the form "prowcount = " with 
        // any number of spaces allowed, followed by a greedy match on
        // a numeric of any length (positive or negative)
        Regex rowCountRegex = new Regex(@"(?<=prowcount\s*=\s*)-*\d+");
        Regex readsRegex = new Regex(@"(?<=pbufread\s*=\s*)-*\d+");
        Regex writesRegex = new Regex(@"(?<=pbufwrite\s*=\s*)-*\d+");
        Regex cursorFetchStatusRegex = new Regex(@"(?<=pfetchstat\s*=\s*)-*\d+");
        Regex cursorSetRowsRegex = new Regex(@"(?<=pcrsrows\s*=\s*)-*\d+");
        // Note that we check for both last error and previous error
        Regex lastErrorRegex = new Regex(@"(?<=ec_lasterror\s*=\s*)-*\d+");
        Regex prevErrorRegex = new Regex(@"(?<=ec_preverror\s*=\s*)-*\d+");
        Regex lineNumberRegex = new Regex(@"(?<=pline\s*=\s*)-*\d+");
        Regex lockWaitTimeoutRegex = new Regex(@"(?<=pLockTimeout\s*=\s*)-*\d+");
        Regex textSizeRegex = new Regex(@"(?<=ptextsize\s*=\s*)-*\d+");
        Regex nestingLevelRegex = new Regex(@"(?<=CNestLevel\s*=\s*)-*\d+");
        Regex deadlockPriorityRegex = new Regex(@"(?<=pdeadlockpri\s*=\s*)-*\d+");
        Regex transactionIsolationLevelRegex = new Regex(@"(?<=isolation_level\s*=\s*)-*\d+");
        // This regex matches on a full word and breaks at the word boundary
        Regex languageRegex = new Regex(@"(?<=Language\s*=\s*)\w+\b");
        // This regex matches a numeric in the form of 0x000000
        Regex optionsRegex = new Regex(@"(?<=poptions\s+=\s+\dx)\d+");

        private int sequenceNumber = 1;

        #endregion



        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionDetailsProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public SessionDetailsProbe(SqlConnectionInfo connectionInfo, SessionDetailsConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("SessionDetailsProbe");
            sessionDetails = new SessionDetailSnapshot(connectionInfo);
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
                StartSessionDetailsCollector();
            }
            else
            {
                FireCompletion(sessionDetails, Result.Success);
            }
            
        }

        /// <summary>
        /// Define the SessionDetails collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void SessionDetailsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            conn.InfoMessage += new SqlInfoMessageEventHandler(ReadDbccPss);
            // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
            SqlCommand cmd =
                SqlCommandBuilder.BuildSessionDetailsCommand(conn, ver, config.SpidFilter, config.TraceOn,config.ClientSessionId,config.TraceLength,config.TraceRestartTime, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(SessionDetailsCallback));
        }

        /// <summary>
        /// Starts the Session Details collector.
        /// </summary>
        private void StartSessionDetailsCollector()
        {
            StartGenericCollector(new Collector(SessionDetailsCollector), sessionDetails, "StartSessionDetailsCollector", "Session Details", SessionDetailsCallback, new object[] { config.TraceOn });
        }

        /// <summary>
        /// Define the SessionDetails callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void SessionDetailsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretSessionDetails(rd);
            }
            FireCompletion(sessionDetails, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the SessionDetails collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void SessionDetailsCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(SessionDetailsCallback), sessionDetails, "SessionDetailsCallback", "Session Details",
                            sender, e);
        }

        /// <summary>
        /// Interpret SessionDetails data
        /// </summary>
        private void InterpretSessionDetails(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretSessionDetails"))
            {
                try
                {
                    ReadSessionBasics(dataReader);

                    if (sessionDetails.Details.Status != SessionStatus.Ended)
                    {
                        // DBCC Inputbuffer is a separate resultset for SQL 2000 only
                        if (sessionDetails.ProductVersion.Major == 8)
                        {
                            ReadDbccInputbuffer(dataReader);
                        }
                        else
                        {
                            //Read session details for SQL 2005
                            ReadSessionDetails(dataReader);
                        }
                    }

                    ReadTraceData(dataReader);
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(sessionDetails, LOG, "Error interpreting Session Details Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(sessionDetails);
                }
            }
        }

        private void ReadSessionBasics(SqlDataReader dataReader)
        {
            try
            {
                if (!dataReader.HasRows)
                {
                    sessionDetails.Details.Status = SessionStatus.Ended;
                    return;
                }

                while (dataReader.Read())
                {
                    if (!dataReader.IsDBNull(0))
                    {
                        sessionDetails.Details = new SessionDetail();

                        sessionDetails.Details.Spid = dataReader.GetInt32(0);
                        if (!dataReader.IsDBNull(1)) sessionDetails.Details.UserName = dataReader.GetString(1);
                        if (!dataReader.IsDBNull(2)) sessionDetails.Details.Workstation = dataReader.GetString(2);
                        if (!dataReader.IsDBNull(3))
                            sessionDetails.Details.Status = Session.ConvertToSessionStatus(dataReader.GetString(3));
                        if (!dataReader.IsDBNull(4)) sessionDetails.Details.Application = dataReader.GetString(4);
                        if (!dataReader.IsDBNull(5)) sessionDetails.Details.Command = dataReader.GetString(5);
                        if (!dataReader.IsDBNull(6)) sessionDetails.Details.Database = dataReader.GetString(6);
                        if (!dataReader.IsDBNull(7)) sessionDetails.Details.Cpu = TimeSpan.FromMilliseconds(dataReader.GetInt32(7));
                        if (!dataReader.IsDBNull(8))
                            sessionDetails.Details.Memory.Kilobytes = dataReader.GetInt32(8) * 8;
                        if (!dataReader.IsDBNull(9)) sessionDetails.Details.PhysicalIo = dataReader.GetInt64(9);
                        if (!dataReader.IsDBNull(10)) sessionDetails.Details.BlockedBy = dataReader.GetInt32(10);
                        if (!dataReader.IsDBNull(11)) sessionDetails.Details.BlockingCount = dataReader.GetInt32(11);
                        if (!dataReader.IsDBNull(12)) sessionDetails.Details.LoggedInSince = dataReader.GetDateTime(12);
                        if (!dataReader.IsDBNull(13)) sessionDetails.Details.LastActivity = dataReader.GetDateTime(13);
                        if (!dataReader.IsDBNull(14))
                            sessionDetails.Details.OpenTransactions = Convert.ToInt32(dataReader.GetInt16(14));
                        if (!dataReader.IsDBNull(15))
                            sessionDetails.Details.WorkstationNetAddress = dataReader.GetString(15);
                        if (!dataReader.IsDBNull(16)) sessionDetails.Details.NetLibrary = dataReader.GetString(16);
                        if (!dataReader.IsDBNull(17))
                            sessionDetails.Details.WaitTime = TimeSpan.FromMilliseconds(dataReader.GetInt64(17));
                        if (!dataReader.IsDBNull(18))
                            sessionDetails.Details.ExecutionContext = Convert.ToInt32(dataReader.GetInt16(18));
                        if (!dataReader.IsDBNull(19))
                            sessionDetails.Details.WaitType = dataReader.GetString(19).TrimEnd(new Char[] {' ', '\0'});
                        if (!dataReader.IsDBNull(20))
                            sessionDetails.Details.WaitResource =
                                dataReader.GetString(20).TrimEnd(new Char[] {' ', '\0'});
                        //The last command is only part of this recordset for SQL 2005.  For SQL 2000 we need to read DBCC Inputbuffer.
                        if (sessionDetails.ProductVersion.Major >= 9) // SQL Server 2005/2008
                        {
                            if (!dataReader.IsDBNull(21)) sessionDetails.Details.LastCommand = dataReader.GetString(21);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(sessionDetails, LOG, "Unable to read Session Basics: ", e, false);
                GenericFailureDelegate(sessionDetails);
            }
        }
        
        private void ReadSessionDetails(SqlDataReader dataReader)
        {
            try
            {
                dataReader.NextResult();
                if (dataReader.Read())
                {
                    if (!dataReader.IsDBNull(0)) sessionDetails.Details.RowCount = dataReader.GetInt64(0);
                    if (!dataReader.IsDBNull(1)) sessionDetails.Details.Reads = dataReader.GetInt64(1);
                    if (!dataReader.IsDBNull(2)) sessionDetails.Details.Writes = dataReader.GetInt64(2);
                    if (!dataReader.IsDBNull(3))
                        sessionDetails.Details.CursorFetchStatus = sessionDetails.Details.StringToCursorFetchStatus(dataReader.GetInt32(3).ToString(), sessionDetails.ProductVersion);
                    if (!dataReader.IsDBNull(4)) sessionDetails.Details.OpenTransactions = dataReader.GetInt32(4);
                    if (!dataReader.IsDBNull(5)) sessionDetails.Details.LastError = dataReader.GetInt32(5);
                    if (!dataReader.IsDBNull(6))
                        sessionDetails.Details.LockWaitTimeout = TimeSpan.FromMilliseconds(dataReader.GetInt32(6));
                    if (!dataReader.IsDBNull(7)) sessionDetails.Details.TextSize = dataReader.GetInt32(7);
                    if (!dataReader.IsDBNull(8))
                        sessionDetails.Details.DeadlockPriority = Convert.ToInt16(dataReader.GetInt32(8));
                    if (!dataReader.IsDBNull(9))
                        sessionDetails.Details.TransactionIsolationLevel = (TransactionIsolation) dataReader.GetInt16(9);
                    if (!dataReader.IsDBNull(10)) sessionDetails.Details.Language = dataReader.GetString(10);
                    if (!dataReader.IsDBNull(11))
                        sessionDetails.Details.Options.QuotedIdentifier = dataReader.GetBoolean(12);
                    if (!dataReader.IsDBNull(12)) sessionDetails.Details.Options.Arithabort = dataReader.GetBoolean(12);
                    if (!dataReader.IsDBNull(13))
                        sessionDetails.Details.Options.AnsiNullDefaultOn = dataReader.GetBoolean(13);
                    if (!dataReader.IsDBNull(14))
                        sessionDetails.Details.Options.AnsiDefaults = dataReader.GetBoolean(14);
                    if (!dataReader.IsDBNull(15))
                        sessionDetails.Details.Options.AnsiWarnings = dataReader.GetBoolean(15);
                    if (!dataReader.IsDBNull(16))
                        sessionDetails.Details.Options.AnsiPadding = dataReader.GetBoolean(16);
                    if (!dataReader.IsDBNull(17)) sessionDetails.Details.Options.AnsiNulls = dataReader.GetBoolean(17);
                    if (!dataReader.IsDBNull(18))
                        sessionDetails.Details.Options.ConcatNullYieldsNull = dataReader.GetBoolean(18);
                    if (!dataReader.IsDBNull(19)) sessionDetails.Details.NestingLevel = dataReader.GetInt32(19);
                }
            }
            catch(Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(sessionDetails, LOG, "Unable to read Session Details: ", e, false);
                GenericFailureDelegate(sessionDetails);
            }
            
        }

        private void ReadDbccPss(object sender, SqlInfoMessageEventArgs e)
        {
            try
            {
                foreach (SqlError err in e.Errors)
                {
                    // Set RowCount from PSS data 
                    if (!sessionDetails.Details.RowCount.HasValue && rowCountRegex.IsMatch(err.Message))
                        sessionDetails.Details.RowCount = Convert.ToInt64(rowCountRegex.Match(err.Message).Value);
                    
                    // Set Reads from PSS data 
                    if (!sessionDetails.Details.Reads.HasValue && readsRegex.IsMatch(err.Message))
                        sessionDetails.Details.Reads = Convert.ToInt64(readsRegex.Match(err.Message).Value);
                    
                    // Set Writes from PSS data 
                    if (!sessionDetails.Details.Writes.HasValue && writesRegex.IsMatch(err.Message))
                        sessionDetails.Details.Writes = Convert.ToInt64(writesRegex.Match(err.Message).Value);
                    
                   
                    // Set CursorFetchStatus from PSS data 
                    if (sessionDetails.Details.CursorFetchStatus == null && cursorFetchStatusRegex.IsMatch(err.Message))
                        sessionDetails.Details.CursorFetchStatus = sessionDetails.Details.StringToCursorFetchStatus(cursorFetchStatusRegex.Match(err.Message).Value, sessionDetails.ProductVersion);
                    
                    // Set CursorSetRows from PSS data 
                    if (!sessionDetails.Details.CursorSetRows.HasValue && cursorSetRowsRegex.IsMatch(err.Message))
                        sessionDetails.Details.CursorSetRows = Convert.ToInt64(cursorSetRowsRegex.Match(err.Message).Value);

                    //select description from master..sysmessages where msglangid = 1033 and error = " & lngGetSQLError & " if  @@ROWCOUNT = 0 select ''"
                    //"select description from master..sysmessages where error = " & lngGetSQLError & " if  @@ROWCOUNT = 0 select ''"
                    
                    // Set LastError from PSS data - try the PrevError
                    if ((!sessionDetails.Details.LastError.HasValue ||
                        (sessionDetails.Details.LastError.HasValue &&  sessionDetails.Details.LastError == 0))
                        && prevErrorRegex.IsMatch(err.Message))
                        sessionDetails.Details.LastError = Convert.ToInt32(lastErrorRegex.Match(err.Message).Value);

                    // Set LastError from PSS data - try the LastError
                    if ((!sessionDetails.Details.LastError.HasValue ||
                        (sessionDetails.Details.LastError.HasValue && sessionDetails.Details.LastError == 0))
                        && lastErrorRegex.IsMatch(err.Message))
                        sessionDetails.Details.LastError = Convert.ToInt32(lastErrorRegex.Match(err.Message).Value);
                    
                    // Set LineNumber from PSS data 
                    if (!sessionDetails.Details.LineNumber.HasValue && lineNumberRegex.IsMatch(err.Message))
                        sessionDetails.Details.LineNumber = Convert.ToInt32(lineNumberRegex.Match(err.Message).Value);
                    
                    // Set LockWaitTimeout from PSS data 
                    if (!sessionDetails.Details.LockWaitTimeout.HasValue && lockWaitTimeoutRegex.IsMatch(err.Message))
                        sessionDetails.Details.LockWaitTimeout = TimeSpan.FromMilliseconds(Convert.ToInt64(lockWaitTimeoutRegex.Match(err.Message).Value));
                    
                    // Set TextSize from PSS data 
                    if (!sessionDetails.Details.TextSize.HasValue && textSizeRegex.IsMatch(err.Message))
                        sessionDetails.Details.TextSize = Convert.ToInt64(textSizeRegex.Match(err.Message).Value);
                    
                    // Set NestingLevel from PSS data 
                    if (!sessionDetails.Details.NestingLevel.HasValue && nestingLevelRegex.IsMatch(err.Message))
                        sessionDetails.Details.NestingLevel = Convert.ToInt64(nestingLevelRegex.Match(err.Message).Value);
                    
                    // Set DeadlockPriority from PSS data 
                    if (!sessionDetails.Details.DeadlockPriority.HasValue && deadlockPriorityRegex.IsMatch(err.Message))
                        sessionDetails.Details.DeadlockPriority = Convert.ToInt16(deadlockPriorityRegex.Match(err.Message).Value);
                    
                    // Set TransactionIsolationLevel from PSS data 
                    if (!sessionDetails.Details.TransactionIsolationLevel.HasValue && transactionIsolationLevelRegex.IsMatch(err.Message))
                        sessionDetails.Details.TransactionIsolationLevel = (TransactionIsolation)Convert.ToInt32(transactionIsolationLevelRegex.Match(err.Message).Value);
                    
                    // Set Language from PSS data 
                    if (sessionDetails.Details.Language == null && languageRegex.IsMatch(err.Message))
                        sessionDetails.Details.Language = languageRegex.Match(err.Message).Value;
                    
                    // Set Options from PSS data 
                    if (sessionDetails.Details.Options.StatusMask == 0 && optionsRegex.IsMatch(err.Message))
                        sessionDetails.Details.Options.StatusMask =
                            int.Parse(optionsRegex.Match(err.Message).Value, System.Globalization.NumberStyles.HexNumber);

                }
            }
            catch (Exception exception)
            {
                ProbeHelpers.LogAndAttachToSnapshot(sessionDetails, LOG, "Error processing DBCC PSS data: {0}", exception,
                                                   true);
            }
        }

        /// <summary>
        /// DBCC Inputbuffer is a separate resultset for SQL 2000 only
        /// </summary>
        private void ReadDbccInputbuffer(SqlDataReader dataReader)
        {
            try
            {
                //Move to the next record - we do this at the beginning rather than the end of the previous method
                //throughout this probe because the ReadTraceData needs to do a while loop on NextResult
                dataReader.NextResult();

                while (dataReader.Read())
                {
                    if (dataReader.FieldCount == 3)
                    {
                        if (!dataReader.IsDBNull(2))
                        {
                            sessionDetails.Details.LastCommand = dataReader.GetString(2);
                        }
                        else
                        {
                            //Treat any problem as just a warning - do not fail
                            ProbeHelpers.LogAndAttachToSnapshot(sessionDetails, LOG, "Unable to read DBCC Inputbuffer: the expected field was null",true);
                        }
                    }
                    else
                    {
                        //Treat any problem as just a warning - do not fail
                        ProbeHelpers.LogAndAttachToSnapshot(sessionDetails, LOG, "Unable to read DBCC Inputbuffer: the field count was incorrect",true);
                    }
                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(sessionDetails, LOG, "Unable to read DBCC Inputbuffer: ", e, false);
                GenericFailureDelegate(sessionDetails);
            }
        }


        private void ReadTraceData(SqlDataReader dataReader)
        {
            try
            {
                sequenceNumber = config.NextSequenceNumber;

                while(dataReader.NextResult())
                {
                    while (dataReader.Read()) 
                    {
                        if (dataReader.FieldCount == 7)
                        {
                            TraceStatement statement = new TraceStatement();
                            statement.SequenceNumber = sequenceNumber++;
                            if (!dataReader.IsDBNull(0)) statement.EventType = (TraceEventType) dataReader.GetInt32(0);
                            if (!dataReader.IsDBNull(1))
                                statement.Duration = TimeSpan.FromMilliseconds(dataReader.GetInt64(1));
                            if (!dataReader.IsDBNull(2)) statement.CompletionTime = dataReader.GetDateTime(2);
                            if (!dataReader.IsDBNull(3)) statement.SqlText = dataReader.GetString(3);
                            if (!dataReader.IsDBNull(4)) statement.Reads = dataReader.GetInt64(4);
                            if (!dataReader.IsDBNull(5)) statement.Writes = dataReader.GetInt64(5);
                            if (!dataReader.IsDBNull(6)) statement.CpuTime = TimeSpan.FromMilliseconds(dataReader.GetInt64(6));

                            if (sessionDetails.TraceItems == null)
                                sessionDetails.TraceItems = new List<TraceStatement>();

                            sessionDetails.TraceItems.Add(statement);
                        }
                        else
                        {
                            //Treat this problem as just a warning in case we do have other valid records - do not fail
                            ProbeHelpers.LogAndAttachToSnapshot(sessionDetails, LOG, "Unable to read from session trace: the field count was incorrect", true);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(sessionDetails, LOG, "Unable to read from session trace: ",e, false);
                GenericFailureDelegate(sessionDetails);
            }
        }
        #endregion

        #region interface implementations

        #endregion
    }
}
