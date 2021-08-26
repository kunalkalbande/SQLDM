//------------------------------------------------------------------------------
// <copyright file="SessionsProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

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
    using System.Collections.Generic;
    using Wintellect.PowerCollections;
    using Idera.SQLdm.CollectionService.Helpers;


    /// <summary>
    /// On-demand probe for server sessions
    /// </summary>
    internal class SessionsProbe : SqlBaseProbe
    {
        #region fields

        private SessionSnapshot sessionSnapshot = null;
        private Dictionary<Pair<int?, DateTime?>, Pair<TimeSpan, Int64?>> previousValues = new Dictionary<Pair<int?, DateTime?>, Pair<TimeSpan, Int64?>>();
        private SessionsConfiguration configuration = null;
        //private int? cloudProviderId = null;
        //sqldm -30013 start
        int databaseCount = 0;//gives the count of databases of azure instance for which collection has been done
        int numberOfDatabases=0;//gives the number of databases in azure instance
        object sessionLockobj = null;
        //sqldm-30013 end
        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionsProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="configuration">The configuration info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public SessionsProbe(SqlConnectionInfo connectionInfo, SessionsConfiguration configuration, int? cloudProviderId = null) //SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId; //SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
            sessionLockobj = new object();//sqldm-30013 used for synchronization between threads while collecting data from all databases
            LOG = Logger.GetLogger("SessionsProbe");
            sessionSnapshot = new SessionSnapshot(connectionInfo);
            this.configuration = configuration;
            if (configuration != null)
                previousValues = configuration.PreviousValues;
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
            if (configuration != null && configuration.ReadyForCollection)
            {
                StartSessionsCollector();
            }
            else
            {
                FireCompletion(sessionSnapshot, Result.Success);
            }
        }

        /// <summary>
        /// Define the Session collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void SessionsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
           
            SqlCommand cmd =
                           SqlCommandBuilder.BuildSessionsCommand(conn, ver, configuration, cloudProviderId);
           
            sdtCollector = new SqlCollector(cmd, true);
            
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(SessionsCallback));
           
           
        }
        //sqldm-30013 start
        void SessionsCollectorDatabase(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver, String dbname)
        {
            
      
            
                SqlCommand cmd =
                               SqlCommandBuilder.BuildSessionsCommand(conn, ver, configuration, cloudProviderId,dbname);

                sdtCollector = new SqlCollector(cmd, true);

                sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(SessionsCallbackDatabase));
            
            
        }
        //sqldm-30013 end
        /// <summary>
        /// Starts the session list collector.
        /// </summary>
        void StartSessionsCollector()
        {
            //sqldm-30013 start //handling case for azure 
            if (cloudProviderId == 2)
            {
               List<String> dbInfo = CollectionHelper.GetDatabases(connectionInfo,LOG);
                databaseCount = 0;
                numberOfDatabases = dbInfo.Count;
               
                foreach (String dbin in dbInfo)
                {
                    StartGenericCollectorDatabase(new CollectorDatabase(SessionsCollectorDatabase), sessionSnapshot, "StartSessionsCollector", "Session List", SessionsCallbackDatabase,dbin, new object[] { });
                }
            }
            //sqldm-30013 end
            else
                StartGenericCollector(new Collector(SessionsCollector), sessionSnapshot, "StartSessionsCollector", "Session List", SessionsCallback, new object[] { });

        }

        /// <summary>
        /// Define the Sessions callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void SessionsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                if (sessionSnapshot.ProductVersion.Major == 8)
                {
                    InterpretSessions2000(rd);
                }
                else
                {
                    InterpretSessions(rd);
                }
            }
            FireCompletion(sessionSnapshot, Result.Success);
        }

        //sqldm-30013 start
        /// <summary>
        /// Define the Sessions callback for azure
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void SessionsCallbackDatabase(CollectorCompleteEventArgs e)
        {
            if (e.Result == Result.Success)
            {
                if (e.Value != null)
                {
                    using (SqlDataReader rd = e.Value as SqlDataReader)
                    {
                        lock (sessionLockobj)
                        {
                            if (sessionSnapshot.ProductVersion.Major == 8)
                            {
                                InterpretSessions2000(rd);
                            }
                            else
                            {
                                InterpretSessions(rd);
                            }
                        }
                    }
                }
            }
            lock(sessionLockobj)
            {
                if (databaseCount == numberOfDatabases - 1)
                    FireCompletion(sessionSnapshot, Result.Success);
                else
                    databaseCount++;
            }
        }
        //sqldm-30013 end
        /// <summary>
        /// Callback used to process the data returned from the sessions collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void SessionsCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(SessionsCallback), sessionSnapshot, "SessionsCallback", "Session List",
                            sender, e);
        }
        //start sqldm 30013
        void SessionsCallbackDatabase(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(SessionsCallbackDatabase), sessionSnapshot,
                "SessionsCallback", "Session List", new FailureDelegate(GenericFailureDelegate), new FailureDelegate(GenericFailureDelegate),
                            null, sender, e, true, true);
            /*GenericCallback(new CollectorCallback(SessionsCallbackDatabase), sessionSnapshot, "SessionsCallback", "Session List",
                            sender, e);*/
        }
        //end sqldm 30013
        /// <summary>
        /// Interpret sessions data
        /// </summary>
        private void InterpretSessions2000(SqlDataReader dataReader)
        {
            //the 2000 batch gets sessioncount as a separate batch
            ServerOverviewInterpreter.ReadProcessDetails(dataReader, sessionSnapshot.SystemProcesses, sessionSnapshot, LOG);

            ReadSessions(dataReader);

            CalculateSessionDeltas();
        }

        /// <summary>
        /// Interpret sessions data
        /// </summary>
        private void InterpretSessions(SqlDataReader dataReader)
        {
            ReadSessions(dataReader);

            //The interpretation formerly done by ServerOverviewInterpreter.ReadProcessDetails is now in here
            CalculateSessionCounts(sessionSnapshot.SystemProcesses);
        }

        /// <summary>
        /// Read sessions data
        /// </summary>
        private void ReadSessions(SqlDataReader dataReader)
        {
            

            using (LOG.DebugCall("InterpretSessions"))
            {
                

                try
                {
                 if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            if (sessionSnapshot.ProductVersion.Major == 8)
                            {
                                ReadSessions2000(dataReader);
                            }
                            else
                            {

                                ReadSessions2005(dataReader);
                            }
                           
                        }

                        try
                        {
                            int spid = 0;
                            DateTime? loginTime = null;
                            while (dataReader.NextResult())
                            {
                                dataReader.Read();
                                switch (dataReader.FieldCount)
                                {
                                    case 2:
                                        if (!dataReader.IsDBNull(0)) spid = dataReader.GetInt32(0);
                                        if (!dataReader.IsDBNull(1)) loginTime = dataReader.GetDateTime(1);
                                        break;
                                    case 3:
                                        if (!dataReader.IsDBNull(2))
                                        {
                                            Pair<int?, DateTime?> identifier =
                                                new Pair<int?, DateTime?>(spid, loginTime);
                                            if (sessionSnapshot.SessionList.ContainsKey(identifier))
                                                sessionSnapshot.SessionList[identifier].LastCommand =
                                                    dataReader.GetString(2);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                     catch (Exception e)
                     {
                         LOG.Error("There was an error when reading session command text",e);
                     }
                    }
                    else
                    {
                        LOG.Verbose(
                                "No fields returned");
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(sessionSnapshot, LOG, "Error interpreting Session List Collector: {0}", e,
                                                       false);
                    GenericFailureDelegate(sessionSnapshot);
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }


        private void ReadSessions2000(SqlDataReader dataReader)
        {
            if (dataReader.FieldCount >= 21)
            {
                if (!dataReader.IsDBNull(0))
                {
                    Session session = new Session();

                    session.Spid = dataReader.GetInt32(0);
                    if (!dataReader.IsDBNull(1)) session.UserName = dataReader.GetString(1);
                    if (!dataReader.IsDBNull(2)) session.Workstation = dataReader.GetString(2);
                    if (!dataReader.IsDBNull(3))
                        session.Status = Session.ConvertToSessionStatus(dataReader.GetString(3));
                    if (!dataReader.IsDBNull(4)) session.Application = dataReader.GetString(4);
                    if (!dataReader.IsDBNull(5)) session.Command = dataReader.GetString(5);
                    if (!dataReader.IsDBNull(6)) session.Database = dataReader.GetString(6);
                    if (!dataReader.IsDBNull(7))
                        session.Cpu = TimeSpan.FromMilliseconds(dataReader.GetInt32(7));
                    if (!dataReader.IsDBNull(8))
                        session.Memory.Kilobytes = dataReader.GetInt32(8) * 8;
                    if (!dataReader.IsDBNull(9)) session.PhysicalIo = dataReader.GetInt64(9);
                    if (!dataReader.IsDBNull(10)) session.BlockedBy = dataReader.GetInt32(10);
                    if (!dataReader.IsDBNull(11)) session.BlockingCount = dataReader.GetInt32(11);
                    if (!dataReader.IsDBNull(12)) session.LoggedInSince = dataReader.GetDateTime(12);
                    if (!dataReader.IsDBNull(13)) session.LastActivity = dataReader.GetDateTime(13);
                    if (!dataReader.IsDBNull(14))
                        session.OpenTransactions = Convert.ToInt32(dataReader.GetInt16(14));
                    if (!dataReader.IsDBNull(15))
                        session.WorkstationNetAddress = dataReader.GetString(15);
                    if (!dataReader.IsDBNull(16)) session.NetLibrary = dataReader.GetString(16);
                    if (!dataReader.IsDBNull(17))
                        session.WaitTime = TimeSpan.FromMilliseconds(dataReader.GetInt64(17));
                    if (!dataReader.IsDBNull(18))
                        session.ExecutionContext = Convert.ToInt32(dataReader.GetInt16(18));
                    if (!dataReader.IsDBNull(19))
                        session.WaitType = dataReader.GetString(19).TrimEnd(new Char[] {' ', '\0'});
                    if (!dataReader.IsDBNull(20))
                        session.WaitResource = dataReader.GetString(20).TrimEnd(new Char[] {' ', '\0'});
                    if (dataReader.FieldCount >= 22)
                        if (!dataReader.IsDBNull(21)) session.LastCommand = dataReader.GetString(21);
                    


                    if (sessionSnapshot.SessionList.ContainsKey(session.InternalSessionIdentifier))
                    {
                        LOG.Verbose("A duplicate SPID was detected during the session probe on " +
                                  sessionSnapshot.ServerName + ".  The SPID is " + session.Spid +
                                  ".  Discarding duplicate data.");
                    }
                    else
                    {
                        sessionSnapshot.SessionList.Add(session.InternalSessionIdentifier, session);
                    }
                }

                else
                {
                    LOG.Error(
                        "There was an error while reading the session list: The SPID field was null.");
                }

            }
            else
            {
                LOG.Error(
                    "There was an error while reading the session list: The field count was incorrect.");
            }
        }

        private void ReadSessions2005(SqlDataReader dataReader)
        {

                if (!dataReader.IsDBNull(0))
                {
                    Session session = new Session();

                    session.Spid = dataReader.GetInt16(0);
                    if (!dataReader.IsDBNull(1)) session.UserName = dataReader.GetString(1);
                    if (!dataReader.IsDBNull(2)) session.Workstation = dataReader.GetString(2);
                    if (!dataReader.IsDBNull(3))
                        session.Status = Session.ConvertToSessionStatus(dataReader.GetString(3));
                    if (!dataReader.IsDBNull(4)) session.Application = dataReader.GetString(4);
                    if (!dataReader.IsDBNull(5)) session.Command = dataReader.GetString(5);
                    if (!dataReader.IsDBNull(6)) session.Database = dataReader.GetString(6);
                    if (!dataReader.IsDBNull(7))//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: In case of azure, null is returned here
                    {
                        session.Cpu = TimeSpan.FromMilliseconds(Convert.ToDouble(dataReader[7]));
                    }
                    if (!dataReader.IsDBNull(8))
                        session.Memory.Kilobytes = Convert.ToInt64(dataReader[8]) * 8;
                    if (!dataReader.IsDBNull(9) && !dataReader.IsDBNull(10) )
                        session.PhysicalIo = dataReader.GetInt64(9) + dataReader.GetInt64(10); // sum of reads and writes
                    
                    if (!dataReader.IsDBNull(12)) session.BlockedBy = dataReader.GetInt16(12);
                    if (!dataReader.IsDBNull(13)) session.BlockingCount = dataReader.GetInt32(13);
                    if (!dataReader.IsDBNull(14)) session.LoggedInSince = dataReader.GetDateTime(14);

                    if (!dataReader.IsDBNull(15)) session.LastActivity = dataReader.GetDateTime(15);

                    if (!dataReader.IsDBNull(17))
                        session.OpenTransactions = Convert.ToInt32(dataReader.GetInt16(17));
                    if (!dataReader.IsDBNull(18))
                         session.NetLibrary = dataReader.GetString(18);
                    if (!dataReader.IsDBNull(19)) session.WorkstationNetAddress = dataReader.GetString(19);
                    if (!dataReader.IsDBNull(20))
                        session.WaitTime = TimeSpan.FromMilliseconds(dataReader.GetInt32(20));
                    if (!dataReader.IsDBNull(21))
                        session.ExecutionContext = dataReader.GetInt32(21);
                    if (!dataReader.IsDBNull(23))
                        session.WaitType = dataReader.GetString(23).TrimEnd(new Char[] { ' ', '\0' });
                    if (!dataReader.IsDBNull(24))
                        session.WaitResource = dataReader.GetString(24).TrimEnd(new Char[] { ' ', '\0' });
                    if (dataReader.FieldCount >= 25)
                    {
                        if (!dataReader.IsDBNull(25)) session.LastCommand = dataReader.GetString(25);
                        if (!dataReader.IsDBNull(26))
                            session.VersionStoreElapsedTime = TimeSpan.FromSeconds(dataReader.GetInt64(26));
                        if (!dataReader.IsDBNull(27) )
                        {
                            int isolation = dataReader.GetInt16(27);
                            if (isolation <= 5)
                            {
                                session.TransactionIsolationLevel = (TransactionIsolation) (isolation);
                            }
                        }
                        
                        if (!dataReader.IsDBNull(29))
                            session.SessionUserAllocatedTotal = new FileSize {Pages = dataReader.GetInt32(29)};
                        else
                        {
                            session.SessionUserAllocatedTotal = new FileSize(0);
                        }
                        if (!dataReader.IsDBNull(30))
                            session.SessionUserDeallocatedTotal = new FileSize { Pages = dataReader.GetInt32(30) };
                        if (!dataReader.IsDBNull(31))
                            session.TaskUserAllocatedTotal = new FileSize { Pages = dataReader.GetInt32(31) };
                        if (!dataReader.IsDBNull(32))
                            session.TaskUserDeallocatedTotal = new FileSize { Pages = dataReader.GetInt32(32) };
                        if (!dataReader.IsDBNull(33))
                            session.SessionInternalAllocatedTotal = new FileSize { Pages = dataReader.GetInt32(33) };
                        if (!dataReader.IsDBNull(34))
                            session.SessionInternalDeallocatedTotal = new FileSize { Pages = dataReader.GetInt32(34) };
                        if (!dataReader.IsDBNull(35))
                            session.TaskInternalAllocatedTotal = new FileSize { Pages = dataReader.GetInt32(35) };
                        if (!dataReader.IsDBNull(36))
                            session.TaskInternalDeallocatedTotal = new FileSize { Pages = dataReader.GetInt32(36) };
                    }

                    if (sessionSnapshot.SessionList.ContainsKey(session.InternalSessionIdentifier))
                    {
                        LOG.Verbose("A duplicate SPID was detected during the session probe on " +
                                  sessionSnapshot.ServerName + ".  The SPID is " + session.Spid +
                                  ".  Discarding duplicate data.");
                    }
                    else
                    {
                        sessionSnapshot.SessionList.Add(session.InternalSessionIdentifier, session);
                    }
                }

                else
                {
                    LOG.Error(
                        "There was an error while reading the session list: The SPID field was null.");
                }

            
        }

        /// <summary>
        /// Calculate the difference in CPU and IO between refreshes
        /// Also count the session types and categories for the active session types graph
        /// </summary>
        /// <param name="refresh"></param>
        private void CalculateSessionCounts(ServerSystemProcesses refresh)
        {
            int systemProcessesConsumingCPU = 0;
            int userProcessesConsumingCPU = 0;
            int blockedProcesses = 0;
            int processWithOpenTrans = 0;
            int userProcesses = 0;
            int systemProcesses = 0;
            int distinctComputers = 0;
            int leadBlockers = 0;
            int activeProcesses = 0;
            string prevHostName = "";

            if (refresh == null)
            {
                LOG.Error("The ServerSystemProcess object used in CalculateSessionCounts is NULL!");
                return;
            }

            List<String> distinctComputersList = new List<string>();

            foreach (var session in sessionSnapshot.SessionList.Values)
            {
                if (previousValues.ContainsKey(session.InternalSessionIdentifier))
                {
                    if (previousValues[session.InternalSessionIdentifier].First != null)
                        session.CpuDelta = TimeSpan.FromMilliseconds(session.Cpu.TotalMilliseconds - previousValues[session.InternalSessionIdentifier].First.TotalMilliseconds);
                    if (previousValues[session.InternalSessionIdentifier].Second.HasValue)
                        session.PhysicalIoDelta = session.PhysicalIo - previousValues[session.InternalSessionIdentifier].Second;
                }
                
                if (session.CpuDelta.Milliseconds > 0 && string.IsNullOrEmpty(session.WorkstationNetAddress))
                    systemProcessesConsumingCPU++;
                if (session.CpuDelta.Milliseconds > 0 && !string.IsNullOrEmpty(session.WorkstationNetAddress))
                    userProcessesConsumingCPU++;
                if (session.BlockedBy.HasValue && session.BlockedBy > 0) blockedProcesses++;
                if (session.OpenTransactions.HasValue && session.OpenTransactions > 0) processWithOpenTrans++;
                if (!string.IsNullOrEmpty(session.WorkstationNetAddress)) userProcesses++;
                if (string.IsNullOrEmpty(session.WorkstationNetAddress)) systemProcesses++;

                if (session.Workstation != null)
                {
                    if (!session.Workstation.Equals(prevHostName) && (!session.IsSystemProcess))
                    {
                        if (!distinctComputersList.Contains(session.Workstation))
                            distinctComputersList.Add(session.Workstation);
                    }
                }
                else
                {
                    LOG.DebugFormat("Session {0} has a null workstation.  Net Address is not null", session.Spid);
                }

                if (session.Blocking && session.BlockedBy == null) leadBlockers++;
                if(!session.Status.ToString().ToLower().Equals("sleeping") && !string.IsNullOrEmpty(session.WorkstationNetAddress))
                activeProcesses++;

                if (!session.IsSystemProcess && session.Workstation != null) prevHostName = session.Workstation;
            }

            distinctComputers = distinctComputersList.Count;

            refresh.SystemProcessesConsumingCpu = systemProcessesConsumingCPU;
            refresh.UserProcessesConsumingCpu = userProcessesConsumingCPU;
            refresh.BlockedProcesses = blockedProcesses;
            refresh.OpenTransactions = processWithOpenTrans;
            refresh.CurrentUserProcesses = userProcesses;
            refresh.CurrentSystemProcesses = systemProcesses;
            refresh.ComputersHoldingProcesses = distinctComputers;
            refresh.LeadBlockers = leadBlockers;
            refresh.ActiveProcesses = activeProcesses;

        }
        /// <summary>
        /// Calculate the difference in CPU and IO between refreshes
        /// </summary>
        private void CalculateSessionDeltas()
        {
            foreach (Session session in sessionSnapshot.SessionList.Values)
            {
                if (previousValues.ContainsKey(session.InternalSessionIdentifier))
                {
                    if (previousValues[session.InternalSessionIdentifier].First != null)
                        session.CpuDelta = TimeSpan.FromMilliseconds(session.Cpu.TotalMilliseconds - previousValues[session.InternalSessionIdentifier].First.TotalMilliseconds);
                    if (previousValues[session.InternalSessionIdentifier].Second.HasValue)
                        session.PhysicalIoDelta = session.PhysicalIo - previousValues[session.InternalSessionIdentifier].Second;
                }
            }
        }

        #endregion

        #region interface implementations

        #endregion
    }
}
