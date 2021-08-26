//------------------------------------------------------------------------------
// <copyright file="SessionSummaryProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------


namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading;
    using BBS.TracerX;
    using Collectors;
    using Common;
    using Common.Configuration;
    using Common.Services;
    using Common.Snapshots;
    using Idera.SQLdm.CollectionService.Helpers;

    /// <summary>
    /// On-demand probe for SessionSummaryProbe
    /// </summary>
    internal class SessionSummaryProbe : SqlBaseProbe
    {
        #region fields

        private SessionSummary snapshot = null;
        private DateTime? serverStartTime = null;
        private SessionSummaryConfiguration config = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionSummaryProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="configuration">The configuration info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        private List<string> cloudDBNames = new List<string>();
        private int numberOfDatabases = 0;
        public SessionSummaryProbe(SqlConnectionInfo connectionInfo, SessionSummaryConfiguration configuration, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("SessionSummaryProbe");
            this.cloudProviderId = cloudProviderId;
            snapshot = new SessionSummary(connectionInfo);
            if (configuration != null)
            {
                serverStartTime = configuration.ServerStartTime;
                if(configuration.PreviousLockStatistics != null)
                {
                    snapshot.LockCounters = configuration.PreviousLockStatistics;
                }
            }
            config = configuration;
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
            if (config != null && config.ReadyForCollection && cloudProviderId!=CLOUD_PROVIDER_ID_AZURE)//SQLDM-30299 changes
            {
                StartSessionSummaryCollector();
            }
            else if (config != null && config.ReadyForCollection && cloudProviderId==CLOUD_PROVIDER_ID_AZURE)//SQLDM-11.0 Azure Support for Blocking and Locking Sessions by collecting and aggregating data for each Database within azure instance
            {
                cloudDBNames = CollectionHelper.GetDatabases(connectionInfo, LOG);
                numberOfDatabases = 0;
                if (cloudDBNames.Count > 0)
                {
                    StartSessionSummaryCollectorAzure();
                }
               
            }
            else
            {
                FireCompletion(snapshot, Result.Success);
            }
        }

        // Start :Azure Support for Blocking and Locking Sessions
        /// <summary>
        /// Define the SessionSummary collector Azure
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void SessionSummaryCollectorAzure(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver, string dbname)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildSessionSummaryCommand(conn, ver, config.SearchTerm, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(SessionSummaryCallbackAzure));
        }
        /// <summary>
        /// Starts the Session Summary collector Azure.
        /// </summary>
        private void StartSessionSummaryCollectorAzure()
        {
            //StartGenericCollector(new Collector(SessionSummaryCollectorAzure), snapshot, "StartSessionSummaryCollector", "Session Summary", SessionSummaryCallbackAzure, new object[] { });
            StartGenericCollectorDatabase(new CollectorDatabase(SessionSummaryCollectorAzure),
                snapshot,
                "StartSessionSummaryCollector",
                "Session Summary",
              SessionSummaryCallbackAzure, cloudDBNames[numberOfDatabases], new object[] { });
        }

        /// <summary>
        /// Define the SessionSummary Azure callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void SessionSummaryCallbackAzure(CollectorCompleteEventArgs e)
        {
            if (serverStartTime != snapshot.ServerStartupTime)
            {
                LOG.Info("Server restart detected.  Disposing of previous metrics.");
                snapshot.LockCounters = new LockStatistics();
            }
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretSessionSummary(rd);
            }

        }

        /// <summary>
        /// Callback used to process the data returned from the SessionSummary collector Azure.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
         private void SessionSummaryCallbackAzure(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing SessionSummaryCallbackAzure to work queue.");
                QueueCallback(snapshot, sender as SqlCollector, SessionSummaryCallbackAzure, e);
                return;
            }

            using (LOG.DebugCall("SessionSummaryCallbackAzure"))
            {
                Interlocked.Increment(ref numberOfDatabases);
                NextCollector nextCollector = numberOfDatabases < cloudDBNames.Count? StartSessionSummaryCollectorAzure :new NextCollector(StartResponseTimeCollector);
                GenericCallback(new CollectorCallback(SessionSummaryCallbackAzure), snapshot, "SessionSummaryCallbackAzure", "Session Summary Azure", nextCollector,
                            sender, e);
               
            }

        }
       // End :Azure Support for Blocking and Locking Sessions

        /// <summary>
        /// Define the SessionSummary collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void SessionSummaryCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildSessionSummaryCommand(conn, ver, config.SearchTerm, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(SessionSummaryCallback));
        }

        /// <summary>
        /// Starts the Session Summary collector.
        /// </summary>
        private void StartSessionSummaryCollector()
        {
            StartGenericCollector(new Collector(SessionSummaryCollector), snapshot, "StartSessionSummaryCollector", "Session Summary", SessionSummaryCallback, new object[] { });
        }

        
        /// <summary>
        /// Define the SessionSummary callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void SessionSummaryCallback(CollectorCompleteEventArgs e)
        {
            if (serverStartTime != snapshot.ServerStartupTime)
            {
                LOG.Info("Server restart detected.  Disposing of previous metrics.");
                snapshot.LockCounters = new LockStatistics();
            }
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretSessionSummary(rd);
            }
            
        }

        /// <summary>
        /// Callback used to process the data returned from the SessionSummary collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void SessionSummaryCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(SessionSummaryCallback), snapshot, "SessionSummaryCallback", "Session Summary",new NextCollector(StartResponseTimeCollector),
                            sender, e);
        }

        /// <summary>
        /// Create Response Time Collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void ResponseTimeCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd = SqlCommandBuilder.BuildResponseTimeCommand(conn);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ResponseTimeCallback));
        }

        /// <summary>
        /// Starts the response time collector.
        /// </summary>
        void StartResponseTimeCollector()
        {
            StartGenericCollector(new Collector(ResponseTimeCollector), snapshot, "StartResponseTimeCollector",
                                 "Response Time", null, new object[] { });
        }

        /// <summary>
        /// Define the Response Time callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void ResponseTimeCallback(CollectorCompleteEventArgs e)
        {
            if (e.ElapsedMilliseconds.HasValue)
            {
                snapshot.ResponseTime = TimeSpan.FromMilliseconds(e.ElapsedMilliseconds.Value);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the response time collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The Idera.SQLdm.Probes.Collectors.CollectorCompleteEventArgs instance containing the event data.</param>
        void ResponseTimeCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(ResponseTimeCallback), snapshot, "ResponseTimeCallback", "Response Time", sender, e);
        }

        /// <summary>
        /// Interpret sessions data
        /// </summary>
        private void InterpretSessionSummary(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretSessionSummary"))
            {
                try
                {
                    ServerOverviewInterpreter.ReadProcessDetails(dataReader, snapshot.SystemProcesses, snapshot, LOG);
                    ProbeHelpers.ReadLockStatistics(dataReader, snapshot,LOG,snapshot.LockCounters,GenericFailureDelegate);
                    ReadOutstandingLocks(dataReader);

                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot, LOG, "Error interpreting Session Summary Collector: {0}", e,
                                                       false);
                    GenericFailureDelegate(snapshot);
                }
            }
        }


        private void ReadOutstandingLocks(SqlDataReader dataReader)
        {
            try
            {
                while (dataReader.Read())
                {
                    if (dataReader.FieldCount == 3 && !dataReader.IsDBNull(0) && !dataReader.IsDBNull(1) && !dataReader.IsDBNull(2))
                    {
                        string s = dataReader.GetDataTypeName(2);
                        switch (dataReader.GetString(0))
                        {
                            case "allocunit":
                            case "allocation_unit":
                                SetOutstanding(snapshot.LockCounters.AllocUnitCounters, snapshot.LockCounters.TotalCounters, dataReader.GetString(1), dataReader.GetInt64(2));
                                break;
                            case "application":
                                SetOutstanding(snapshot.LockCounters.ApplicationCounters, snapshot.LockCounters.TotalCounters, dataReader.GetString(1), dataReader.GetInt64(2));
                                break;
                            case "database":
                                SetOutstanding(snapshot.LockCounters.DatabaseCounters, snapshot.LockCounters.TotalCounters, dataReader.GetString(1), dataReader.GetInt64(2));
                                break;
                            case "extent":
                                SetOutstanding(snapshot.LockCounters.ExtentCounters, snapshot.LockCounters.TotalCounters, dataReader.GetString(1), dataReader.GetInt64(2));
                                break;
                            case "file":
                                SetOutstanding(snapshot.LockCounters.FileCounters, snapshot.LockCounters.TotalCounters, dataReader.GetString(1), dataReader.GetInt64(2));
                                break;
                            case "hobt":
                                SetOutstanding(snapshot.LockCounters.HeapCounters, snapshot.LockCounters.TotalCounters, dataReader.GetString(1), dataReader.GetInt64(2));
                                break;
                            case "key":
                                SetOutstanding(snapshot.LockCounters.KeyCounters, snapshot.LockCounters.TotalCounters, dataReader.GetString(1), dataReader.GetInt64(2));
                                break;
                            case "metadata":
                                SetOutstanding(snapshot.LockCounters.MetadataCounters, snapshot.LockCounters.TotalCounters, dataReader.GetString(1), dataReader.GetInt64(2));
                                break;
                            case "object":
                                SetOutstanding(snapshot.LockCounters.ObjectCounters, snapshot.LockCounters.TotalCounters, dataReader.GetString(1), dataReader.GetInt64(2));
                                break;
                            case "page":
                                SetOutstanding(snapshot.LockCounters.PageCounters, snapshot.LockCounters.TotalCounters, dataReader.GetString(1), dataReader.GetInt64(2));
                                break;
                            case "rid":
                                SetOutstanding(snapshot.LockCounters.RidCounters, snapshot.LockCounters.TotalCounters, dataReader.GetString(1), dataReader.GetInt64(2));
                                break;
                            case "table":
                                SetOutstanding(snapshot.LockCounters.TableCounters, snapshot.LockCounters.TotalCounters, dataReader.GetString(1), dataReader.GetInt64(2));
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(snapshot, LOG, "Error interpreting Session Summary Collector (Outstanding Locks): {0}", e,
                                                   false);
                GenericFailureDelegate(snapshot);
            }

        }

        private static void SetOutstanding(LockCounter counter, LockCounter total,string status,Int64? count)
        {
            switch (status)
            {
                case "grant":
                case "granted":
                    if (counter.OutstandingGranted == null)
                        counter.OutstandingGranted = count;
                    else
                        counter.OutstandingGranted += count;
                    if(total.OutstandingGranted==null)
                        total.OutstandingGranted = count;
                    else
                        total.OutstandingGranted += count;
                    break;
                case "convert":
                case "converting":
                    if(counter.OutstandingConverted==null)
                    counter.OutstandingConverted = count;
                    else
                        counter.OutstandingConverted += count;
                    if(total.OutstandingConverted==null)
                    total.OutstandingConverted = count;
                    else
                        counter.OutstandingConverted += count;
                    break;
                case "wait":
                case "waiting":
                    if (counter.OutstandingWaiting == null)
                        counter.OutstandingWaiting = count;
                    else
                        counter.OutstandingWaiting += count;
                    if(total.OutstandingWaiting==null)
                    total.OutstandingWaiting = count;
                    else
                        total.OutstandingWaiting += count;
                    break;
            }
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
