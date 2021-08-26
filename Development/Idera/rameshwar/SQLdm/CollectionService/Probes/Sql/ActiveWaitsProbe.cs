//------------------------------------------------------------------------------
// <copyright file="ActiveWaitsProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

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
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using Microsoft.SqlServer.XEvent.Linq;
    using Microsoft.SqlServer.XEvent;
    using System.Threading;
    using Idera.SQLdm.CollectionService.Helpers;

    /// <summary>
    /// Active waits probe - collects query-level wait data
    /// </summary>
    internal sealed class ActiveWaitsProbe : SqlBaseProbe
    {
        private const int StatementUtcStartTimeColumnIndex = 0;

        private const int WaitDurationColumnIndex = 1;

        private const int SessionIdColumnIndex = 2;

        private const string WaitTypeColumn = "Wait Type";

        private const string HostNameColumn = "HostName";

        private const string ProgramNameColumn = "program_name";

        private const string LoginNameColumn = "LoginName";

        private const string DatabaseNameColumn = "DatabaseName";

        private const string StatementTxtColumn = "statement_txt";

        private const string SessionIdColumn = "session_id";

        private const string WaitDurationColumn = "WaitDuration";

        private const string StartTimeColumn = "StartTime";

        private const int WaitTypeColumnIndex = 3;

        private const int HostNameColumnIndex = 4;

        private const int ProgramNameColumnIndex = 5;

        private const int LoginNameColumnIndex = 6;

        private const int DatabaseNameColumnIndex = 7;

        private const int StatementTxtColumnIndex = 8;

        private const int UtcCollectionDateTimeColumnIndex = 10;

        private const int MsTicksColumnIndex = 9;

        #region fields

        private ActiveWaitsSnapshot refresh = null;
        private ActiveWaitsConfiguration config = null;
        private readonly string activeWaitsLogStartTemplate = "Active Waits - "; //SQLdm 10.3 (Tushar)--Log template for Active Waits collector.

        //SQLDM 10.3 : Technical debt changes
        private string currentFileName;
        private string dbLastFileName;
        private long lastFileRecordCount;
        private long dbLastRecordCount;
        private List<string> cloudDBNamesAWAzure = new List<string>();
        private int numberOfDatabasesAW;
        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveWaitsProbe"/> class.
        /// </summary>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public ActiveWaitsProbe(SqlConnectionInfo connectionInfo, ActiveWaitsConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("ActiveWaitsProbe:" + connectionInfo.InstanceName);
            refresh = new ActiveWaitsSnapshot(connectionInfo);
            this.config = config;
            // SQLdm 10.3 (Varun Chopra) Linux Support for Active Waits
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
                if(cloudProviderId == Constants.MicrosoftAzureId)
                {
                    numberOfDatabasesAW = 0;
                    initializeDBNamesForAWAzure();
                }
                StartActiveWaitsCollector();
            }
            else
            {
                FireCompletion(refresh, Result.Success);
            }
        }

        //START-  SQLDM 10.3 (Manali Hukkeri) : Technical debt changes
        /// <summary>
        /// Starts the ActiveWaits State store collector. This collector will save the last file the last line read.
        /// </summary>
        private void StartActiveWaitsStateStoreCollector()
        {
            StartGenericCollector(new Collector(ActiveWaitsStateStoreCollector), refresh, "StartActiveWaitsStateStoreCollector", "Active Waits", null, new object[] { this.config.EnabledXe, this.config.EnabledQs });
        }

        /// <summary>
        /// Create Acitive Waits State Store Collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void ActiveWaitsStateStoreCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand saveCommand = null;
            using (LOG.DebugCall("SaveActiveWaitsState"))
            {
                if (!this.config.EnabledQs && this.refresh.CollectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember) && currentFileName != null && (currentFileName != dbLastFileName || lastFileRecordCount > dbLastRecordCount))
                {
                    // SQLdm 10.3 (Varun Chopra) Linux Support by passing cloudProviderId
                    saveCommand = new SqlCommand(SqlCommandBuilder.BuildActiveWaitesWriteCommand(
                        currentFileName, lastFileRecordCount), conn);
                    sdtCollector = new SqlCollector(saveCommand, true);
                }
                else
                {
                    sdtCollector = new SqlCollector(new SqlCommand(string.Empty, conn), true);
                }

                LOG.Info(string.Format(activeWaitsLogStartTemplate + "Trying to save active waits state : FileName = {0}, RecordCount = {1}", currentFileName, lastFileRecordCount));
                sdtCollector.BeginCollectionNonQueryExecution(new EventHandler<CollectorCompleteEventArgs>(ActiveWaitsStateStoreCallback));
                LOG.Info(string.Format(activeWaitsLogStartTemplate + "Saved successfully. FileName = {0}, RecordCount = {1}", currentFileName, lastFileRecordCount));
            }
        }

        /// <summary>
        /// Callback used to process the data returned from the Active Waits State Store collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The Idera.SQLdm.Probes.Collectors.CollectorCompleteEventArgs instance containing the event data.</param>
        private void ActiveWaitsStateStoreCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(ActiveWaitsStateStoreCallback),
                          refresh,
                          "ActiveWaitsStateStoreCallback",
                          "Active Waits state store",
                          sender,
                          e);

        }

        /// <summary>
        /// Define the ActiveWaitsStateStoreCallback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ActiveWaitsStateStoreCallback(CollectorCompleteEventArgs e)
        {
            // Write the Last Read File Name and Offset to the tempdb
            // Callback required for logging purposes only
            if (e.Result == Result.Success)
            {
                LOG.Info("ActiveWaitsStateStoreCallback() successfully completed");
            }
            else
            {
                var exceptionMessage = "ActiveWaitsStateStoreCallback() Active Wait - Write the Last Read File Name and Offset to the tempdb failed ";
                if (e.Exception != null)
                {
                    exceptionMessage += e.Exception.ToString();
                }
                LOG.Error(exceptionMessage);
            }
        }

        ///END-  SQLDM 10.3 (Manali Hukkeri) : Technical debt changes
        ///
        private StringBuilder activeWaitsDbSelection(bool notCondition, string dbName)
        {
            StringBuilder res = new StringBuilder();
            res.Append(" AND name ");
            if (notCondition)
                res.Append("NOT ");
            res.Append("LIKE '" + dbName + "'");
            return res;
        }
        private string getDbPredicates(string[] dbs, bool notCondition)
        {
            StringBuilder res = new StringBuilder();
            if (dbs != null)
            {
                foreach (string filterString in dbs)
                {
                    if (filterString != null && filterString.Length > 0)
                    {
                        res.Append(activeWaitsDbSelection(notCondition, filterString));
                    }
                }
            }
            return res.ToString();
        }
        private void initializeDBNamesForAWAzure()
        {
            //GET CLOUD DB NAMES
            StringBuilder predicateString = new StringBuilder();
            // Prioritize Exclude, as per on-prem
            predicateString.Append(getDbPredicates(config.AdvancedConfiguration.DatabaseExcludeMatch, true));
            predicateString.Append(getDbPredicates(config.AdvancedConfiguration.DatabaseExcludeLike, true));
            if (predicateString.Length == 0)
            {
                predicateString.Append(getDbPredicates(config.AdvancedConfiguration.DatabaseIncludeMatch, false));
                predicateString.Append(getDbPredicates(config.AdvancedConfiguration.DatabaseIncludeLike, false));
            }
            string sqlQuery = SqlCommandBuilder.BuildFilteredMonitoredDatabasesAzureQuery(predicateString.ToString());
            cloudDBNamesAWAzure = CollectionHelper.GetFilteredDatabasesForMonitoringAzure(connectionInfo, sqlQuery, LOG);
        }

        private void ActiveWaitsCollectorDatabase(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver, string dbName)
        {
            if (ver.Major >= 9)
            {
                try
                {
                    SqlCommand cmd = null;
                    //if user is sysadmin member, then passing false flag to command constructor method 
                    //to use extended events old way without api.
                    cmd =
                    SqlCommandBuilder.BuildActiveWaitsCommand(conn, ver, config, cloudProviderId, false);
                    
                    sdtCollector = new SqlCollector(cmd, true);
                    sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ActiveWaitsCallback));
                }
                catch (Exception ex)
                {
                    LOG.Error(string.Format("Caught exception in ActiveWaitsCollector.  conn is null: {0}, stdCollector is null: {1}, ver is null: {2}", conn == null, sdtCollector == null, ver == null), ex);
                }
            }
            else
            {
                refresh.SetError(
                    "Version not supported.",
                    new Exception("Wait Statistics are not available for SQL Server versions before 2005.")); //JSFIX
                FireCompletion(refresh, Result.Success); //JSFIX            }
            }
        }

        /// <summary>
        /// Define the ActiveWaits collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void ActiveWaitsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            if (ver.Major >= 9)
            {
                try
                {
                    SqlCommand cmd = null;
                    if (refresh.CollectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember) && 
                        cloudProviderId != Constants.MicrosoftAzureManagedInstanceId && cloudProviderId != Constants.MicrosoftAzureId)
                    {
                        //if user is sysadmin member, then passing true flag to command constructor method 
                        //to use extended events api.
                        cmd =
                        SqlCommandBuilder.BuildActiveWaitsCommand(conn, ver, config, cloudProviderId,true);
                    }
                    else
                    {
                        //if user is sysadmin member, then passing false flag to command constructor method 
                        //to use extended events old way without api.
                        cmd =
                        SqlCommandBuilder.BuildActiveWaitsCommand(conn, ver, config, cloudProviderId,false);
                    }
                    sdtCollector = new SqlCollector(cmd, true);
                    sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ActiveWaitsCallback));
                }
                catch (Exception ex)
                {
                    LOG.Error(string.Format("Caught exception in ActiveWaitsCollector.  conn is null: {0}, stdCollector is null: {1}, ver is null: {2}", conn == null, sdtCollector == null, ver == null), ex);
                }
            }
            else
            {
                refresh.SetError(
                    "Version not supported.",
                    new Exception("Wait Statistics are not available for SQL Server versions before 2005.")); //JSFIX
                FireCompletion(refresh, Result.Success); //JSFIX            }
            }
        }

        /// <summary>
        /// Starts the Active Waits collector.
        /// </summary>
        private void StartActiveWaitsCollector()
        {
            if (cloudProviderId == Constants.MicrosoftAzureId)
            {
                StartGenericCollectorDatabase(new CollectorDatabase(ActiveWaitsCollectorDatabase), refresh,
                    "StartActiveWaitsCollectorDatabase", "Active Waits", ActiveWaitsCallback,
                    cloudDBNamesAWAzure[numberOfDatabasesAW], new object[] { config.EnabledXe, this.config.EnabledQs });
            }
            else
            {
                StartGenericCollector(new Collector(ActiveWaitsCollector), refresh, "StartActiveWaitsCollector",
                    "Active Waits", ActiveWaitsCallback, new object[] { config.EnabledXe, this.config.EnabledQs });
            }
        }
        /// <summary>
        /// Define the ActiveWaits callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ActiveWaitsCallback(CollectorCompleteEventArgs e)
        {
            try
            {
                if (config.EnabledQs && (refresh.ProductVersion.Major >= 14 ||
                                         cloudProviderId == Constants.MicrosoftAzureManagedInstanceId ||
                                         cloudProviderId == Constants.MicrosoftAzureId))
                {
                    // Write code for interpreting ActiveWaits for Query store
                    using (SqlDataReader rd = e.Value as SqlDataReader)
                    {
                        InterpretActiveWaitsQs(rd, e.Database);
                    }
                }
                else if (refresh.ProductVersion.Major > 10 && config.EnabledXe)
                {
                    //If user is sysadmin member, then active waits are collected using Extended events api.
                    if (cloudProviderId == Constants.MicrosoftAzureManagedInstanceId || cloudProviderId == Constants.MicrosoftAzureId)
                    {
                        //In user is non-sysadmin, then active waits are collected the old way.
                        using (SqlDataReader rd = e.Value as SqlDataReader)
                        {
                            InterpretActiveWaits(FilterActiveWaitsManagedInstance(rd));
                        }
                    }
                    else if (refresh.CollectionPermissions.HasFlag(CollectionPermissions.SYSADMINMember))
                    {
                        InterpretActiveWaits(GetActiveWaitsXEReaderData(e));
                    }
                    else
                    {
                        //In user is non-sysadmin, then active waits are collected the old way.
                        using (SqlDataReader rd = e.Value as SqlDataReader)
                        {
                            InterpretActiveWaits(FilterActiveWaits(rd));
                        }
                    }
                }
                else
                {
                    using (SqlDataReader rd = e.Value as SqlDataReader)
                    {
                            InterpretActiveWaits(rd);
                    }
                }
                FireCompletion(refresh, Result.Success);
            }
            catch (Exception ex)
            {
                ProbeHelpers.LogAndAttachToSnapshot(refresh,
                                                    LOG,
                                                    "Error in Active Waits callback: {0}",
                                                    ex,
                                                    false);
                GenericFailureDelegate(refresh);
            }
        }

        /// <summary>
        /// Callback used to process the data returned from the ActiveWaits collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ActiveWaitsCallback(object sender, CollectorCompleteEventArgs e)
        {

            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing Active Waits Callback to work queue.");
                QueueCallback(refresh, sender as SqlCollector, ActiveWaitsCallback, e);
                return;
            }
            NextCollector nextCollector = new NextCollector(StartActiveWaitsStateStoreCollector);
            if(cloudProviderId == Constants.MicrosoftAzureId)
            {
                Interlocked.Increment(ref numberOfDatabasesAW);
                nextCollector = numberOfDatabasesAW < cloudDBNamesAWAzure.Count
                        ? StartActiveWaitsCollector
                        : nextCollector;
            }
            GenericCallback(new CollectorCallback(ActiveWaitsCallback),
                refresh,
                "ActiveWaitsCallback",
                "Activie Waits State store",
                e.Result == Result.PermissionViolation ? null : nextCollector,
                sender,
                e);
        }

        /// <summary>
        /// Interpret ActiveWaits data from Query Store
        /// </summary>
        /// <param name="dataReader"></param>
        private void InterpretActiveWaitsQs(SqlDataReader dataReader, string database = null)
        {
            using (this.LOG.DebugCall("InterpretActiveWaitsQs"))
            {
                try
                {
                    var numRows = 0;
                    this.refresh.ActiveWaits.BeginLoadData();

                    if (dataReader != null)
                    {
                        DateTimeOffset maxDateTime = DateTimeOffset.MinValue;
                        string dbName = database;

                        while (dataReader.Read())
                        {
                            var dr = this.refresh.ActiveWaits.NewRow();

                            // Read Wait Type
                            var ordinalIndex = dataReader.GetOrdinal(WaitTypeColumn);
                            dr[WaitTypeColumnIndex] = dataReader.IsDBNull(ordinalIndex)
                                                          ? null
                                                          : dataReader.GetString(ordinalIndex);

                            // Read HostName
                            ordinalIndex = dataReader.GetOrdinal(HostNameColumn);
                            dr[HostNameColumnIndex] = dataReader.IsDBNull(ordinalIndex)
                                                          ? null
                                                          : dataReader.GetString(ordinalIndex);

                            // Read program_name
                            ordinalIndex = dataReader.GetOrdinal(ProgramNameColumn);
                            dr[ProgramNameColumnIndex] = dataReader.IsDBNull(ordinalIndex)
                                                             ? null
                                                             : dataReader.GetString(ordinalIndex);

                            // Read LoginName
                            ordinalIndex = dataReader.GetOrdinal(LoginNameColumn);
                            dr[LoginNameColumnIndex] = dataReader.IsDBNull(ordinalIndex)
                                                           ? null
                                                           : dataReader.GetString(ordinalIndex);

                            // Read DatabaseName
                            ordinalIndex = dataReader.GetOrdinal(DatabaseNameColumn);
                            var databaseName = dataReader.IsDBNull(ordinalIndex)
                                ? null
                                : dataReader.GetString(ordinalIndex);
                            dr[DatabaseNameColumnIndex] = databaseName;
                            
                            if (dbName == null)
                            {
                                dbName = databaseName;
                            }

                            // Read statement_txt
                            ordinalIndex = dataReader.GetOrdinal(StatementTxtColumn);
                            dr[StatementTxtColumnIndex] = dataReader.IsDBNull(ordinalIndex)
                                                              ? null
                                                              : dataReader.GetString(ordinalIndex);

                            // Read Session
                            ordinalIndex = dataReader.GetOrdinal(SessionIdColumn);
                            if (!dataReader.IsDBNull(ordinalIndex))
                            {
                                dr[SessionIdColumnIndex] = dataReader.GetInt16(ordinalIndex);
                            }

                            // Read WaitDuration
                            long duration = 0;
                            ordinalIndex = dataReader.GetOrdinal(WaitDurationColumn);
                            if (!dataReader.IsDBNull(ordinalIndex))
                            {
                                // Duration in ms
                                duration = dataReader.GetInt64(ordinalIndex);
                                dr[WaitDurationColumnIndex] = duration;
                            }

                            // Read Start Time
                            ordinalIndex = dataReader.GetOrdinal(StartTimeColumn);
                            if (!dataReader.IsDBNull(ordinalIndex))
                            {
                                // Start Time
                                var startTime = dataReader.GetDateTimeOffset(ordinalIndex);
                                dr[StatementUtcStartTimeColumnIndex] = startTime.DateTime;

                                // UTC Collection Date time
                                dr[UtcCollectionDateTimeColumnIndex] = startTime.AddMilliseconds(duration).DateTime;

                                // MSTicks End Time in Ticks
                                dr[MsTicksColumnIndex] = startTime.Add(TimeSpan.FromMilliseconds(duration)).Ticks;
                                if (cloudProviderId == Constants.MicrosoftAzureId && startTime > maxDateTime)
                                {
                                    maxDateTime = startTime;
                                }
                            }

                            this.refresh.ActiveWaits.Rows.Add(dr);
                            numRows++;
                        }

                        // Set Start time for Azure
                        if (cloudProviderId == Constants.MicrosoftAzureId && dbName != null)
                        {
                            PersistenceManager.Instance.SetAzureQsStartTime(config.MonitoredServerId, dbName,
                                AzureQsType.ActiveWaits,
                                string.Format(Constants.LastStartTimeAzureQsFormat,
                                    (maxDateTime != DateTimeOffset.MinValue ? maxDateTime : DateTime.UtcNow).ToString(
                                        "yyyy, MM, dd, HH, mm, ss")));
                        }
                    }
                    else
                    {
                        //if we got no results just return
                        this.LOG.Debug("Active Waits QS - No data returned from Query Store.");
                    }
                    this.refresh.ActiveWaits.EndLoadData();
                    this.LOG.Debug(string.Format("Active waits QS collector read {0} rows.", numRows));
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(
                        this.refresh,
                        this.LOG,
                        "Error interpreting Active Waits QS Collector: {0}",
                        e,
                        false);
                    this.GenericFailureDelegate(this.refresh);
                }
            }
        }

        /// <summary>
        /// Interpret ActiveWaits data
        /// </summary>
        /// <param name="dataReader"></param>
        private void InterpretActiveWaits(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretActiveWaits"))
            {
                try
                {
                    int i = 0;
                    DateTime time;
                    long ticks;

                    refresh.ActiveWaits.BeginLoadData();

                    while (dataReader.Read())
                    {
                        DataRow dr = refresh.ActiveWaits.NewRow();
                        if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(10))
                        {
                            time = dataReader.GetDateTime(10);//UTCStartTime of the bucket
                            ticks = dataReader.GetInt64(0);// Duration Ticks ms - from start of bucket or start of wait, whichever is most recent
                            
                            dr[0] = time.Add(TimeSpan.FromMilliseconds(ticks));

                            if (!dataReader.IsDBNull(1))
                                dr[1] = dataReader.GetInt64(1);//sum of all like waits / elapsed time
                            if (!dataReader.IsDBNull(2))
                                dr[2] = dataReader.GetInt16(2);//session
                            if (!dataReader.IsDBNull(3))
                                dr[3] = dataReader.GetString(3);//wait_type
                            if (!dataReader.IsDBNull(4))
                                dr[4] = dataReader.GetString(4);//host
                            if (!dataReader.IsDBNull(5))
                                dr[5] = dataReader.GetString(5);//program
                            if (!dataReader.IsDBNull(6))
                                dr[6] = dataReader.GetString(6);//login
                            if (!dataReader.IsDBNull(7))
                                dr[7] = dataReader.GetString(7);//dbname
                            if (!dataReader.IsDBNull(8))
                                dr[8] = dataReader.GetString(8);//statement_txt
                            if (!dataReader.IsDBNull(9))
                                dr[9] = dataReader.GetInt64(9);//MSTicks at wait start time or bucket start whichever is later
                            if (!dataReader.IsDBNull(10))
                                dr[10] = dataReader.GetDateTime(10);//Time of collection

                            refresh.ActiveWaits.Rows.Add(dr);
                            i++;
                        }
                        else
                        {
                            if (dataReader.IsDBNull(0))
                                LOG.Warn("Ticks column was null for query waits");
                            if (dataReader.IsDBNull(10))
                                LOG.Warn("Time column was null for query waits");
                        }
                    }
                    
                    refresh.ActiveWaits.EndLoadData();
                    LOG.Debug(String.Format("Active waits collector read {0} rows.",i));
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh,
                                                        LOG,
                                                        "Error interpreting Active Waits Collector: {0}",
                                                        e,
                                                        false);
                    GenericFailureDelegate(refresh);
                }
            }
        }
        
        /// <summary>
        /// Interpret Active Waits from Extended Events
        /// </summary>
        /// <param name="dataReader"></param>
        private void InterpretActiveWaits(DbDataReader dataReader)
        {
            
            using (LOG.DebugCall("InterpretActiveWaits-XE"))
            {
                try
                {
                    if (dataReader == null) 
                        return;

                    int i = 0;
                    DateTime time;
                    long ticks;

                    refresh.ActiveWaits.BeginLoadData();

                    while (dataReader.Read())
                    {
                        DataRow dr = refresh.ActiveWaits.NewRow();
                        if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(10))
                        {
                            time = dataReader.GetDateTime(10);

                            try
                            {
                                // Bug with existing code
                                ticks = dataReader.GetInt64(1);
                            }
                            catch (InvalidCastException)
                            {
                                ticks = dataReader.GetInt32(1);
                            }
                            dr[0] = time.Add(TimeSpan.FromMilliseconds(ticks));

                            if (!dataReader.IsDBNull(0))
                            {
                                try
                                {
                                    dr[1] = dataReader.GetInt64(0);
                                }
                                catch (InvalidCastException)
                                {
                                    dr[1] = dataReader.GetInt32(0);
                                }
                            }
                            if (!dataReader.IsDBNull(2))
                                dr[2] = dataReader.GetInt32(2);
                            if (!dataReader.IsDBNull(3))
                                dr[3] = dataReader.GetString(3);
                            if (!dataReader.IsDBNull(4))
                                dr[4] = dataReader.GetString(4);
                            if (!dataReader.IsDBNull(5))
                                dr[5] = dataReader.GetString(5);
                            if (!dataReader.IsDBNull(6))
                                dr[6] = dataReader.GetString(6);
                            if (!dataReader.IsDBNull(7))
                                dr[7] = dataReader.GetString(7);
                            if (!dataReader.IsDBNull(8))
                                dr[8] = dataReader.GetString(8);
                            if (!dataReader.IsDBNull(9))
                                dr[9] = dataReader.GetInt64(9);
                            if (!dataReader.IsDBNull(10))
                                dr[10] = dataReader.GetDateTime(10);//Time of collection
                            refresh.ActiveWaits.Rows.Add(dr);
                            i++;
                        }
                        else
                        {
                            if (dataReader.IsDBNull(0))
                                LOG.Warn("Ticks column was null for query waits");
                            if (dataReader.IsDBNull(10))
                                LOG.Warn("Time column was null for query waits");
                        }
                    }

                    refresh.ActiveWaits.EndLoadData();
                    LOG.Debug(String.Format("Active waits collector read {0} rows.", i));
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh,
                                                        LOG,
                                                        "Error interpreting Active Waits Collector: {0}",
                                                        e,
                                                        false);
                    GenericFailureDelegate(refresh);
                }
            }
        }

        /// <summary>
        /// SQLdm 10.3 (Tushar)--Method to read the active waits extended events data using api.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private DbDataReader GetActiveWaitsXEReaderData(CollectorCompleteEventArgs e)
        {
            using (LOG.DebugCall("GetActiveWaitsXEReaderData"))
            {
                Stopwatch filterSW = new Stopwatch();
                filterSW.Start();

                List<Tuple<string, string>> files = new List<Tuple<string, string>>();
                string flag = "", lastReadFlag = "";
                //long dbLastRecordCount = 0;

                using (SqlDataReader rd = e.Value as SqlDataReader)
                {
                    while (rd.Read())
                    {
                        if (!rd.IsDBNull(0)) flag = rd.GetString(0);
                        //Flag Format: 'SQLdm1 - ' + host_name() + ' - ' + convert(nvarchar(50),@currenttime,121) 
                        if (flag.Length > 23) lastReadFlag = flag.Substring(0, flag.Length - 23);
                    }
                    LOG.Debug(activeWaitsLogStartTemplate + string.Format("Last user defined flag was entered at {0}", String.IsNullOrEmpty(flag) ? "NA" : flag));

                    rd.NextResult();
                    while (rd.Read())
                    {
                        if (!rd.IsDBNull(0)) dbLastFileName = rd.GetString(0);
                        if (!rd.IsDBNull(1)) dbLastRecordCount = rd.GetInt64(1);
                        break;
                    }
                    LOG.Debug(activeWaitsLogStartTemplate + string.Format("DB FileName = {0} DB RecordCount = {1}", dbLastFileName, dbLastRecordCount));

                    rd.NextResult();   // Read filenames
                    while (rd.Read())
                    {
                        string fileName = null;
                        string fileNameInMilliseconds = null;
                        if (!rd.IsDBNull(0))
                            fileName = rd.GetString(0);
                        if (!rd.IsDBNull(1))
                            fileNameInMilliseconds = rd.GetString(1);
                        if (fileName != null && fileNameInMilliseconds != null)
                            files.Add(Tuple.Create(fileName, fileNameInMilliseconds));
                    }

                    LOG.Debug(activeWaitsLogStartTemplate + string.Format("Files to process - {0}", string.Join(",", files.Select(f => f.Item1 + ":" + f.Item2))));
                }
                var table = ReadActiveWaitsData(files, flag, lastReadFlag, dbLastFileName, dbLastRecordCount);
                filterSW.Stop();
                LOG.Info(string.Format(activeWaitsLogStartTemplate + "Reading and Filtering took {0} milliseconds.", filterSW.ElapsedMilliseconds));
                return table;
            }
        }

        //Start-SQLdm 10.3 (Tushar)--Utility methods for active waits extended events api.
         private string GetTextData(PublishedEvent eventData)
        {
            PublishedEventField eventField;
            if (eventData.Fields.TryGetValue("statement", out eventField) && eventField.Value != null)
                return GetString(eventField).Replace("\r\n", "\n");
            if (eventData.Fields.TryGetValue("batch_text", out eventField) && eventField.Value != null)
                return GetString(eventField).Replace("\r\n", "\n");
            if (eventData.Fields.TryGetValue("user_info", out eventField) && eventField.Value != null)
                return GetString(eventField);
            return null;

        }

        private string GetTextData(PublishedEvent eventData,string fieldName)
        {
            PublishedEventField eventField;
            if (eventData.Fields.TryGetValue(fieldName, out eventField) && eventField.Value != null)
                return GetString(eventField).Replace("\r\n", "\n");
            return null;

        }

        private string GetString(PublishedEventField eventField)
        {
            if (eventField.Value is string)
                return eventField.Value.ToString();
            else if (eventField.Value is XMLData)
                return ((XMLData)eventField.Value).RawString;
            return string.Empty;
        }
        //End-SQLdm 10.3 (Tushar)--Utility methods for active waits extended events api.

        /// <summary>
        /// SQLdm 10.3 (Tushar)--Method to create the data table to store active waits data.
        /// </summary>
        /// <returns></returns>
        private DataTable ConstructUnfilteredDataTable()
        {
            //Initialize the data table
            var unfilteredTable = new DataTable("unfilteredDataTable");
            unfilteredTable.Columns.Add(WaitDurationColumn,Type.GetType("System.Int32"));
            unfilteredTable.Columns.Add("Ticks", Type.GetType("System.Int64"));
            unfilteredTable.Columns.Add(SessionIdColumn,Type.GetType("System.Int32"));
            unfilteredTable.Columns.Add(WaitTypeColumn, Type.GetType("System.String"));
            unfilteredTable.Columns.Add(HostNameColumn, Type.GetType("System.String"));
            unfilteredTable.Columns.Add(ProgramNameColumn, Type.GetType("System.String"));
            unfilteredTable.Columns.Add(LoginNameColumn,Type.GetType("System.String"));
            unfilteredTable.Columns.Add(DatabaseNameColumn,Type.GetType("System.String"));
            unfilteredTable.Columns.Add(StatementTxtColumn, Type.GetType("System.String"));
            unfilteredTable.Columns.Add("MSTicks", Type.GetType("System.Int64"));
            unfilteredTable.Columns.Add(StartTimeColumn, Type.GetType("System.DateTime"));
            unfilteredTable.Columns.Add("RowNumber", Type.GetType("System.Int32"));

            DataColumn[] keys = new DataColumn[1];
            keys[0] = unfilteredTable.Columns["RowNumber"];
            
            unfilteredTable.PrimaryKey = keys;
            return unfilteredTable;
        }


        /// <summary>
        /// SQLdm 10.3 (Tushar)--Read active waits data using extended events.
        /// </summary>
        /// <param name="files"></param>
        /// <param name="flag"></param>
        /// <param name="lastReadFlag"></param>
        /// <param name="dbLastFileName"></param>
        /// <param name="dbLastRecordCount"></param>
        /// <returns></returns>
        private DbDataReader ReadActiveWaitsData(List<Tuple<string, string>> files, string flag, string lastReadFlag, string dbLastFileName, long dbLastRecordCount)
        {
            using (LOG.DebugCall("ReadActivityMonitorData"))
            {
                //string currentFileName = null;
                DataTable unfilteredTable = null;

                PublishedAction action = null;
                long lastRowRead = 0, currentRowRead = 0, stopReadingAtRow = 0, rowCounter = 0, longestRowText = 0,
                    countNonNullTextData = 0, currentRecordCount = 0;// lastFileRecordCount = 0;
                bool initializedDatatable = false, goNoFurther = false, shouldSkipEvents = dbLastFileName != null;

                for (int fileIndex = 0; fileIndex < files.Count; fileIndex++)
                {
                    if (goNoFurther)    // Stop processing if flag set to true in inner loop.
                        break;

                    currentFileName = files[fileIndex].Item2;
                    List<PublishedEvent> filteredEvents = new List<PublishedEvent>();
                    Tuple<string, string> file = files[fileIndex];
                    using (QueryableXEventData events = new QueryableXEventData(connectionInfo.ConnectionString, file.Item1,
                        EventStreamSourceOptions.EventFile, EventStreamCacheOptions.DoNotCache))
                    {
                        foreach (PublishedEvent eventData in events)
                        {
                            if (fileIndex == files.Count - 1) // For last file, track events.
                                lastFileRecordCount++;

                            if (shouldSkipEvents)   // Need to skip such events
                            {
                                int comparison = file.Item2.CompareTo(dbLastFileName);
                                if (comparison < 0)
                                    continue;

                                if (comparison == 0 && currentRecordCount <= dbLastRecordCount)
                                {
                                    currentRecordCount++;
                                    continue;
                                }

                                shouldSkipEvents = false;
                            }

                            rowCounter++;
                            string objectName = eventData.Name;
                            string textData = GetTextData(eventData);
                            if (textData != null)
                            {
                                if (textData.Length > longestRowText) longestRowText = textData.Length;

                                //row["TextData"] = textData; Dont put it in textdata column. Waste of memory. 
                                //We already have it an a variable. Cant propogate this to multiple rows.
                                countNonNullTextData++;
                            }
                            string userInfo = GetTextData(eventData, "user_info");
                            switch (objectName)
                            {
                                case "user_event":
                                    if (eventData != null && textData.Contains(lastReadFlag))
                                    {
                                        lastRowRead = currentRowRead;
                                        currentRowRead = rowCounter;

                                        if (userInfo != null && userInfo == flag)
                                        {
                                            stopReadingAtRow = currentRowRead;
                                            LOG.Debug(string.Format("Flag found at row {0}, Previous flag was at row {1}",
                                                            stopReadingAtRow.ToString(CultureInfo.InvariantCulture),
                                                            lastRowRead.ToString(CultureInfo.InvariantCulture)));

                                            goNoFurther = true;
                                        }
                                    }
                                    break;
                            }
                            if (goNoFurther)
                                break;
                            
                            filteredEvents.Add(eventData);
                        }

                        rowCounter = initializedDatatable ? unfilteredTable.Rows.Count: 0;
                        foreach (PublishedEvent eventData in filteredEvents)  // Process on filtered events
                        {
                            if (string.IsNullOrWhiteSpace(eventData.Name))
                                continue;

                            if (initializedDatatable == false)
                            {
                                LOG.Debug(activeWaitsLogStartTemplate + "Initializing unfiltered table");
                                unfilteredTable = ConstructUnfilteredDataTable();
                                initializedDatatable = true;
                            }

                            DataRow row = unfilteredTable.NewRow();
                            string textData = GetTextData(eventData);
                            var isDataNotNull = false;
                            rowCounter++;
                            PublishedEventField eventValue = null;
                            PublishedAction actionValue = null;

                            row["RowNumber"] = rowCounter;

                            if (eventData.Fields.TryGetValue("duration", out eventValue) && eventValue != null &&
                                eventValue.Value != null)
                            {
                                isDataNotNull = true;
                                row[WaitDurationColumn] = eventValue.Value;
                                row["Ticks"] = -1 * (long.Parse(row[WaitDurationColumn].ToString()));
                            }

                            if (eventData.Actions.TryGetValue(SessionIdColumn, out actionValue) && actionValue != null &&
                                actionValue.Value != null)
                            {
                                isDataNotNull = true;
                                row[SessionIdColumn] = actionValue.Value;
                            }

                            if (eventData.Fields.TryGetValue("wait_type", out eventValue) && eventValue != null &&
                                eventValue.Value != null)
                            {
                                isDataNotNull = true;
                                row[WaitTypeColumn] = eventValue.Value.ToString();
                            }

                            if (eventData.Actions.TryGetValue("client_hostname", out actionValue) &&
                                actionValue != null && actionValue.Value != null)
                            {
                                isDataNotNull = true;
                                row[HostNameColumn] = (string)actionValue.Value;
                            }

                            if (eventData.Actions.TryGetValue("client_app_name", out actionValue) && actionValue != null && actionValue.Value != null)
                            {
                                isDataNotNull = true;
                                row[ProgramNameColumn] = (string)actionValue.Value;
                            }

                            if (eventData.Actions.TryGetValue("nt_username", out actionValue) && actionValue != null && actionValue.Value != null)
                            {
                                isDataNotNull = true;
                                row[LoginNameColumn] = actionValue.Value.ToString().ToUpper();
                                
                            }

                            if (eventData.Actions.TryGetValue("database_name", out actionValue) && actionValue != null && actionValue.Value != null)
                            {
                                isDataNotNull = true;
                                row[DatabaseNameColumn] = (string)actionValue.Value;
                            }

                            if (eventData.Actions.TryGetValue("sql_text", out actionValue) && actionValue != null &&
                                actionValue.Value != null)
                            {
                                isDataNotNull = true;
                                row[StatementTxtColumn] = (string)actionValue.Value;
                            }

                            if (eventData.Actions.TryGetValue("collect_system_time", out actionValue) && actionValue != null && actionValue.Value != null)
                            {
                                DateTime systemTime;
                                if (DateTime.TryParse(actionValue.Value.ToString(), out systemTime) && systemTime != null)
                                {
                                    //End Time in Ticks
                                    row["MSTicks"] = systemTime.ToUniversalTime().Ticks;
                                    //StartTime = EndTime - Duration
                                    row[StartTimeColumn] = systemTime.ToUniversalTime() 
                                        + new TimeSpan(-1 * (long.Parse(row[WaitDurationColumn].ToString())));
                                    isDataNotNull = true;
                                }
                            }
                            if (isDataNotNull)
                            {
                                unfilteredTable.Rows.Add(row);
                            }
                            else
                            {
                                rowCounter--;
                            }
                        }


                    }

                } //end of loop

                //// If processed new data, save last read file name to DB.
                //if (currentFileName != null && (currentFileName != dbLastFileName || lastFileRecordCount > dbLastRecordCount))
                //{
                //    SaveActiveWaitsState(currentFileName, lastFileRecordCount);
                //}
                //else
                //{
                //    LOG.Info(activeWaitsLogStartTemplate + "Skipping active waits State Save Operation - No changes in state.");
                //    LOG.Info(string.Format(activeWaitsLogStartTemplate + "DB FileName = {0} CurrentFileName = {1} DB RecordCount = {2} Current RecordCount = {3}", dbLastFileName,
                //        currentFileName, dbLastRecordCount, lastFileRecordCount));
                //}

                LOG.Debug(string.Format(activeWaitsLogStartTemplate + "There are {0} unfiltered rows", rowCounter.ToString(CultureInfo.InvariantCulture)));
                LOG.Debug(string.Format(activeWaitsLogStartTemplate + "There is {0} difference between the row numbers", (stopReadingAtRow - lastRowRead).ToString(CultureInfo.InvariantCulture)));
                LOG.Debug(string.Format(activeWaitsLogStartTemplate + "There are {0} rows of non-null textdata", countNonNullTextData.ToString(CultureInfo.InvariantCulture)));
                LOG.Debug(string.Format(activeWaitsLogStartTemplate + "The longest TextData in any one row was {0} characters", longestRowText.ToString(CultureInfo.InvariantCulture)));

                if (!initializedDatatable)
                {
                    LOG.Debug(activeWaitsLogStartTemplate + "DataTable was not initialized.");
                    return null;
                }

                DataRow[] dr = unfilteredTable.Select(string.Format("RowNumber > {0} and RowNumber < {1}",
                                                         lastRowRead.ToString(CultureInfo.InvariantCulture),
                                                         (stopReadingAtRow > lastRowRead
                                                              ? stopReadingAtRow
                                                              : currentRowRead).ToString(CultureInfo.InvariantCulture)), "RowNumber desc");

                // if data was filtered, pick it, otherwise continue with original table.
                DataTable filteredData = (dr.Length != unfilteredTable.Rows.Count) ? (dr.Length > 0 ? dr.CopyToDataTable() : unfilteredTable.Clone())
                    : unfilteredTable;
                LOG.Info(string.Format(activeWaitsLogStartTemplate + "There are {0} filtered rows", filteredData.Rows.Count.ToString(CultureInfo.InvariantCulture)));

                //Return the dbdatareader of the filtered data
                return filteredData.CreateDataReader();
            }
        }

        ///// <summary>
        ///// SQLdm 10.3 (Tushar)--Save active waits extended events state.
        ///// </summary>
        ///// <param name="fileName"></param>
        ///// <param name="recordCount"></param>
        //public void SaveActiveWaitsState(string fileName,long recordCount)
        //{
        //    using (LOG.DebugCall("SaveActiveWaitsState"))
        //    {
        //        SqlConnection connection = null;
        //        try
        //        {
        //            connection = OpenConnection();
        //            SqlCommand saveCommand = new SqlCommand(SqlCommandBuilder.BuildActiveWaitesWriteCommand(
        //                fileName, recordCount), connection);

        //            LOG.Info(string.Format(activeWaitsLogStartTemplate + "Trying to save active waits state : FileName = {0}, RecordCount = {1}", fileName, recordCount));
        //            saveCommand.ExecuteNonQuery();
        //            LOG.Info(string.Format(activeWaitsLogStartTemplate + "Saved successfully. FileName = {0}, RecordCount = {1}", fileName, recordCount));
        //            connection.Close();
        //        }
        //        catch (Exception e)
        //        {
        //            LOG.Warn(activeWaitsLogStartTemplate + "Exception occurred in SaveActiveWaitsState() while saving active waits state : ", e);
        //            if (connection != null && connection.State != System.Data.ConnectionState.Closed)
        //            {
        //                SqlConnection.ClearPool(connection);
        //                connection.Dispose();
        //            }
        //        }
        //    }
        //}

        private DbDataReader FilterActiveWaitsManagedInstance(SqlDataReader unfilteredDataReader)
        {
            var activeWaitsDataSet = new DataSet("activeWaits");

            activeWaitsDataSet.Tables.Add(new DataTable("unfilteredDataTable"));
            activeWaitsDataSet.Tables.Add(new DataTable("xmlDataTable"));

            var unfilteredTable = activeWaitsDataSet.Tables["unfilteredDataTable"];
            var xmlTable = activeWaitsDataSet.Tables["xmlDataTable"];

            long lastRowRead = 0;
            long currentRowRead = 0;
            long stopReadingAtRow = 0;
            long rowCounter = 0;
            bool initializedDatatable = false;
            bool initializedxmlDatatable = false;

            bool goNoFurther = false;
            DataRow row = null;
            int xeRows = 0;
            int dmvRows = 0;

            long nowTicks = 0;

            string flag = "";
            string lastReadFlag = "";

            using (LOG.DebugCall("FilterActiveWaits"))
            {
                try
                {
                    string ringBufferPrefix = null;

                    Stopwatch filterSW = new Stopwatch();
                    filterSW.Start();

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    if (unfilteredDataReader != null)
                    {
                        while (unfilteredDataReader.Read())
                        {
                            if (!unfilteredDataReader.IsDBNull(0)) flag = unfilteredDataReader.GetString(0);
                            if (!unfilteredDataReader.IsDBNull(1)) nowTicks = unfilteredDataReader.GetInt64(1);
                            if (flag.Length > 23) lastReadFlag = flag.Substring(0, flag.Length - 23);
                        }

                        LOG.Debug(string.Format("Last user defined flag was entered at {0}",
                                                String.IsNullOrEmpty(flag) ? "NA" : flag));

                        unfilteredDataReader.NextResult();

                        while (unfilteredDataReader.Read())
                        {
                            //if the event column is not null
                            if (!unfilteredDataReader.IsDBNull(0))
                            {
                                if (initializedDatatable == false)
                                {
                                    LOG.Debug("Initializing unfiltered table");

                                    //Initialize the data table
                                    unfilteredTable.Columns.Add("RowNumber", Type.GetType("System.Int32"));
                                    unfilteredTable.Columns.Add("object_name", Type.GetType("System.String"));
                                    unfilteredTable.Columns.Add("event_data", Type.GetType("System.String"));
                                    unfilteredTable.Columns.Add("file_offset", Type.GetType("System.Int64"));

                                    initializedDatatable = true;
                                }
                                rowCounter++;

                                row = unfilteredTable.NewRow();

                                row["RowNumber"] = rowCounter;

                                string objectName = null;
                                string eventData = null;

                                if (!unfilteredDataReader.IsDBNull(0))
                                {
                                    eventData = unfilteredDataReader.GetString(0);
                                    row["event_data"] = eventData;
                                    unfilteredTable.Rows.Add(row);
                                    stopReadingAtRow = 2;
                                    ringBufferPrefix = @"/RingBufferTarget";
                                }
                            }

                            if (goNoFurther)
                                break;
                        }
                    }
                    sw.Stop();

                    LOG.Debug(string.Format("It took {0} ms to process the filter", sw.ElapsedMilliseconds));

                    if (!initializedDatatable) return null;

                    //rows are populated oldest to newest
                    //we trun this around to limit just the most recent rows
                    DataRow[] dr =
                        unfilteredTable.Select(string.Format("RowNumber > {0} and RowNumber < {1}",
                                                                lastRowRead.ToString(CultureInfo.InvariantCulture),
                                                                (stopReadingAtRow > lastRowRead
                                                                    ? stopReadingAtRow
                                                                    : currentRowRead).ToString(
                                                                        CultureInfo.InvariantCulture)), "RowNumber desc");

                    LOG.Debug(string.Format("There are {0} filtered rows",
                                            dr.Length.ToString(CultureInfo.InvariantCulture)));

                    //flood control
                    // Modified the code to fix rally issue DE41438 Aditya Shukla SQLdm 8.6
                    var maxRow = config.AdvancedConfiguration.Rowcount > 0 && dr.Length < config.AdvancedConfiguration.Rowcount
                                     ? dr.Length
                                     : (config.AdvancedConfiguration.Rowcount > 0 ? config.AdvancedConfiguration.Rowcount : dr.Length);


                    //Now we have a filtereddata datatable with only the requisite number of rows
                    //////////////////////////////////////////////////////////////////////////////////////////////////////
                    for (int i = 0; i < maxRow; i++)
                    {
                        xeRows++;

                        //We must now create a datatable that is the same as the data we must return and start transforming the XML into here
                        if (!initializedxmlDatatable) initializedxmlDatatable = InitActiveWaitsTable(ref xmlTable);

                        row = xmlTable.NewRow();

                        //get the xml data
                        string xmlString = dr[i][2].ToString();

                        var doc = new XmlDocument();
                        doc.LoadXml(xmlString);

                        if (doc.DocumentElement != null)
                        {
                            var eventPrefix = ringBufferPrefix + @"/event/";
                            var eventNodes = doc.DocumentElement.SelectNodes(eventPrefix.TrimEnd('/'));
                            // var userInfoNodes = doc.DocumentElement.SelectNodes(eventPrefix + @"data[@name='user_info']");
                            string startFlag = null;
                            /*foreach (XmlNode node in userInfoNodes)
                            {
                                if (node == null)
                                {
                                    continue;
                                }
                                if (node.InnerText == flag)
                                {
                                    LOG.Debug(string.Format("Flag found : {0}", flag));
                                    break;
                                }

                                startFlag = node.InnerText;
                            }
                            */
                            bool hasStartFlagReached = startFlag == null;
                            foreach (XmlNode eventNode in eventNodes)
                            {
                                /*XmlNode node = eventNode.SelectSingleNode(@"data[@name='user_info']");

                                if (!hasStartFlagReached && startFlag != null)
                                {
                                    // Skip till we reach user event
                                    if (node == null || node.InnerText != startFlag)
                                    {
                                        continue;
                                    }

                                    hasStartFlagReached = true;
                                    continue;
                                }

                                if (node != null)
                                {
                                    if (node.InnerText == flag)
                                    {
                                        // go no further - end flag reached
                                        break;
                                    }
                                }
                                */
                                if (eventNode == null || !PopulateRow(eventNode, null, row))
                                {
                                    continue;
                                }
                                //once all the fields have data                        
                                xmlTable.Rows.Add(row);
                                row = xmlTable.NewRow();
                            }
                        }
                        else
                        {
                            //once all the fields have data                           
                                xmlTable.Rows.Add(row);            
                        }
                    }
                    LOG.Debug(string.Format("Added {0} XE rows for active waits", xeRows.ToString(CultureInfo.InvariantCulture)));

                    //We now have a fully populated datatable with all of the events from the XE file
                    //Next we need to add the contents of the dmv
                    unfilteredDataReader.NextResult();

                    while (unfilteredDataReader.Read())
                    {
                        if (!initializedxmlDatatable) initializedxmlDatatable = InitActiveWaitsTable(ref xmlTable);

                        dmvRows++;

                        row = xmlTable.NewRow();
                        //changing ticks to duration in ms. This was derived before from ticks but we now have it
                        if (!unfilteredDataReader.IsDBNull(0)) row["Ticks"] = -1 * unfilteredDataReader.GetInt64(1);

                        if (!unfilteredDataReader.IsDBNull(1)) row[WaitDurationColumn] = unfilteredDataReader.GetInt64(1);
                        if (!unfilteredDataReader.IsDBNull(2)) row[SessionIdColumn] = unfilteredDataReader.GetInt16(2);
                        if (!unfilteredDataReader.IsDBNull(3)) row[WaitTypeColumn] = unfilteredDataReader.GetString(3);
                        if (!unfilteredDataReader.IsDBNull(4)) row[HostNameColumn] = unfilteredDataReader.GetString(4);
                        if (!unfilteredDataReader.IsDBNull(5)) row[ProgramNameColumn] = unfilteredDataReader.GetString(5);
                        if (!unfilteredDataReader.IsDBNull(6)) row[LoginNameColumn] = unfilteredDataReader.GetString(6);
                        if (!unfilteredDataReader.IsDBNull(7)) row[DatabaseNameColumn] = unfilteredDataReader.GetString(7);
                        if (!unfilteredDataReader.IsDBNull(8)) row[StatementTxtColumn] = unfilteredDataReader.GetString(8);
                        if (!unfilteredDataReader.IsDBNull(9)) row["MSTicks"] = unfilteredDataReader.GetInt64(9);                    
                        if (!unfilteredDataReader.IsDBNull(10)) row[StartTimeColumn] = unfilteredDataReader.GetDateTime(10);

                        xmlTable.Rows.Add(row);                             
                    }
                    LOG.Debug(string.Format("Added {0} DMV rows for active waits", dmvRows.ToString(CultureInfo.InvariantCulture)));
                }
                catch (Exception ex)
                {
                    LOG.Error("Error occurred filtering Active Waits", ex.Message);
                }
            }

            return GroupActiveWaits(xmlTable).CreateDataReader();
        }

        /// <summary>
        /// filter the Extended Events file data
        /// </summary>
        /// <param name="unfilteredDataReader"></param>
        /// <returns></returns>
        private DbDataReader FilterActiveWaits(SqlDataReader unfilteredDataReader)
        {
            var activeWaitsDataSet = new DataSet("activeWaits");
            
            activeWaitsDataSet.Tables.Add(new DataTable("unfilteredDataTable"));
            activeWaitsDataSet.Tables.Add(new DataTable("xmlDataTable"));
            
            var unfilteredTable = activeWaitsDataSet.Tables["unfilteredDataTable"];
            var xmlTable = activeWaitsDataSet.Tables["xmlDataTable"];

            long lastRowRead = 0;
            long currentRowRead = 0;
            long stopReadingAtRow = 0;
            long rowCounter = 0;
            bool initializedDatatable = false;
            bool initializedxmlDatatable = false;
            
            bool goNoFurther = false;
            DataRow row = null;
            int xeRows = 0;
            int dmvRows = 0;

            long nowTicks = 0;

            string flag = "";
            string lastReadFlag = "";

            using (LOG.DebugCall("FilterActiveWaits"))
            {
                try
                {
                    Stopwatch filterSW = new Stopwatch();
                    filterSW.Start();

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    if (unfilteredDataReader != null)
                    {
                        while (unfilteredDataReader.Read())
                        {
                            if (!unfilteredDataReader.IsDBNull(0)) flag = unfilteredDataReader.GetString(0);
                            if (!unfilteredDataReader.IsDBNull(1)) nowTicks = unfilteredDataReader.GetInt64(1);
                            if (flag.Length > 23) lastReadFlag = flag.Substring(0, flag.Length - 23);
                        }

                        LOG.Debug(string.Format("Last user defined flag was entered at {0}",
                                                String.IsNullOrEmpty(flag) ? "NA" : flag));

                        unfilteredDataReader.NextResult();

                        while (unfilteredDataReader.Read())
                        {
                            //if the event column is not null
                            if (!unfilteredDataReader.IsDBNull(0))
                            {
                                if (initializedDatatable == false)
                                {
                                    LOG.Debug("Initializing unfiltered table");

                                    //Initialize the data table
                                    unfilteredTable.Columns.Add("RowNumber", Type.GetType("System.Int32"));
                                    unfilteredTable.Columns.Add("object_name", Type.GetType("System.String"));
                                    unfilteredTable.Columns.Add("event_data", Type.GetType("System.String"));
                                    unfilteredTable.Columns.Add("file_offset", Type.GetType("System.Int64"));

                                    initializedDatatable = true;
                                }
                                rowCounter++;

                                row = unfilteredTable.NewRow();

                                row["RowNumber"] = rowCounter;

                                string objectName = null;
                                string eventData = null;

                                if (!unfilteredDataReader.IsDBNull(0))
                                    objectName = unfilteredDataReader.GetString(0);

                                if (!unfilteredDataReader.IsDBNull(1))
                                    eventData = unfilteredDataReader.GetString(1);

                                //if(config.AdvancedConfiguration.Rowcount > 0)
                                //{
                                //    if (rowCounter >= config.AdvancedConfiguration.Rowcount)
                                //    {
                                //        lastRowRead = currentRowRead;
                                //        currentRowRead = rowCounter;

                                //        stopReadingAtRow = currentRowRead;
                                //        LOG.Debug(string.Format("XE resultset limited to {0} rows", config.AdvancedConfiguration.Rowcount));
                                //        break;
                                //    }
                                //}

                                switch (objectName)
                                {
                                    case "user_event":
                                        //if this is a flag raise by the activewaits process on this host
                                        if (eventData != null && eventData.Contains(lastReadFlag))
                                        {
                                            //walk up the SQLdm2 rows
                                            lastRowRead = currentRowRead;
                                            currentRowRead = rowCounter;

                                            var doc = new XmlDocument();
                                            doc.LoadXml(eventData);

                                            if (doc.DocumentElement != null)
                                            {
                                                XmlNode node =
                                                    doc.DocumentElement.SelectSingleNode(@"/event/data[@name='user_info']");

                                                if (node != null)
                                                {
                                                    if (node.InnerText == flag)
                                                    {
                                                        stopReadingAtRow = currentRowRead;
                                                        LOG.Debug(
                                                            string.Format(
                                                                "Flag found at row {0}, Previous flag was at row {1}",
                                                                stopReadingAtRow.ToString(CultureInfo.InvariantCulture),
                                                                lastRowRead.ToString(CultureInfo.InvariantCulture)));

                                                        goNoFurther = true;
                                                    }
                                                }
                                            }
                                        }


                                        break;
                                    default:
                                        if (!unfilteredDataReader.IsDBNull(0))
                                            row["object_name"] = objectName;

                                        if (!unfilteredDataReader.IsDBNull(1))
                                            row["event_data"] = eventData;

                                        if (!unfilteredDataReader.IsDBNull(2))
                                            row["file_offset"] = unfilteredDataReader.GetInt64(2);

                                        unfilteredTable.Rows.Add(row);
                                        break;
                                }

                            }

                            if (goNoFurther)
                                break;
                        }
                    }
                    sw.Stop();

                  LOG.Debug(string.Format("It took {0} ms to process the filter", sw.ElapsedMilliseconds));

                    if (!initializedDatatable) return null;
                    //rows are populated oldest to newest
                    //we trun this around to limit just the most recent rows
                    DataRow[] dr =
                        unfilteredTable.Select(string.Format("RowNumber > {0} and RowNumber < {1}",
                                                                lastRowRead.ToString(CultureInfo.InvariantCulture),
                                                                (stopReadingAtRow > lastRowRead
                                                                    ? stopReadingAtRow
                                                                    : currentRowRead).ToString(
                                                                        CultureInfo.InvariantCulture)),"RowNumber desc");

                    LOG.Debug(string.Format("There are {0} filtered rows",
                                            dr.Length.ToString(CultureInfo.InvariantCulture)));

                    //flood control
                    // Modified the code to fix rally issue DE41438 Aditya Shukla SQLdm 8.6
                    var maxRow = config.AdvancedConfiguration.Rowcount > 0 && dr.Length < config.AdvancedConfiguration.Rowcount
                                     ? dr.Length
                                     : (config.AdvancedConfiguration.Rowcount > 0 ? config.AdvancedConfiguration.Rowcount : dr.Length);
                    
                    //Now we have a filtereddata datatable with only the requisite number of rows
                    //////////////////////////////////////////////////////////////////////////////////////////////////////
                    for (int i = 0; i < maxRow; i++)
                    {
                        xeRows++;

                        //We must now create a datatable that is the same as the data we must return and start transforming the XML into here
                        if (!initializedxmlDatatable) initializedxmlDatatable = InitActiveWaitsTable(ref xmlTable);

                        row = xmlTable.NewRow();

                        //get the xml data
                        string xmlString = dr[i][2].ToString();

                        var doc = new XmlDocument();
                        doc.LoadXml(xmlString);

                        if (doc.DocumentElement != null)
                        {
                            var node =
                                doc.DocumentElement.SelectSingleNode(@"/event/data[@name='duration']");

                            if(node !=null) row[WaitDurationColumn] = int.Parse(node.InnerText);

                            row["Ticks"] = -1 * (long.Parse(row[WaitDurationColumn].ToString()));

                            node = doc.DocumentElement.SelectSingleNode(@"/event/action[@name='session_id']");
                            if (node != null) row[SessionIdColumn] = int.Parse(node.InnerText);
                            node = doc.DocumentElement.SelectSingleNode(@"/event/data[@name='wait_type']/text");
                            if (node != null) row[WaitTypeColumn] = node.InnerText;
                            node = doc.DocumentElement.SelectSingleNode(@"/event/action[@name='client_hostname']");
                            if (node != null) row[HostNameColumn] = node.InnerText;
                            node = doc.DocumentElement.SelectSingleNode(@"/event/action[@name='client_app_name']");
                            if (node != null) row[ProgramNameColumn] = node.InnerText;
                            node = doc.DocumentElement.SelectSingleNode(@"/event/action[@name='nt_username']");
                            if (node != null) row[LoginNameColumn] = node.InnerText.ToUpper();
                            node = doc.DocumentElement.SelectSingleNode(@"/event/action[@name='database_name']");
                            if (node != null) row[DatabaseNameColumn] = node.InnerText;
                            node = doc.DocumentElement.SelectSingleNode(@"/event/action[@name='sql_text']");
                            if (node != null) row[StatementTxtColumn] = node.InnerText;
                            //node = doc.DocumentElement.SelectSingleNode(@"/event/action[@name='database_id']");
                            node = doc.DocumentElement.SelectSingleNode(@"/event/action[@name='collect_system_time']");
                            if (node != null)
                            {
                                //End Time in Ticks
                                row["MSTicks"] = DateTime.Parse(node.InnerText).ToUniversalTime().Ticks;
                                //StartTime = EndTime - Duration
                                row[StartTimeColumn] = DateTime.Parse(node.InnerText).ToUniversalTime() 
                                    + new TimeSpan(-1 * (long.Parse(row[WaitDurationColumn].ToString())));
                            }
                                
                        }

                        //once all the fields have data
                        xmlTable.Rows.Add(row);
                    }
                    LOG.Debug(string.Format("Added {0} XE rows for active waits", xeRows.ToString(CultureInfo.InvariantCulture)));
                    
                    //We now have a fully populated datatable with all of the events from the XE file
                    //Next we need to add the contents of the dmv
                    unfilteredDataReader.NextResult();

                    while (unfilteredDataReader.Read())
                    {
                        if (!initializedxmlDatatable) initializedxmlDatatable = InitActiveWaitsTable(ref xmlTable);

                        dmvRows++;

                        row = xmlTable.NewRow();
                        //changing ticks to duration in ms. This was derived before from ticks but we now have it
                        if(!unfilteredDataReader.IsDBNull(0)) row["Ticks"] = -1 * unfilteredDataReader.GetInt64(1);

                        if(!unfilteredDataReader.IsDBNull(1)) row[WaitDurationColumn] = unfilteredDataReader.GetInt64(1);
                        if(!unfilteredDataReader.IsDBNull(2)) row[SessionIdColumn] = unfilteredDataReader.GetInt16(2);
                        if(!unfilteredDataReader.IsDBNull(3)) row[WaitTypeColumn] = unfilteredDataReader.GetString(3);
                        if(!unfilteredDataReader.IsDBNull(4)) row[HostNameColumn] = unfilteredDataReader.GetString(4);
                        if(!unfilteredDataReader.IsDBNull(5)) row[ProgramNameColumn] = unfilteredDataReader.GetString(5);
                        if(!unfilteredDataReader.IsDBNull(6)) row[LoginNameColumn] = unfilteredDataReader.GetString(6);
                        if(!unfilteredDataReader.IsDBNull(7)) row[DatabaseNameColumn] = unfilteredDataReader.GetString(7);
                        if(!unfilteredDataReader.IsDBNull(8)) row[StatementTxtColumn] = unfilteredDataReader.GetString(8);
                        if(!unfilteredDataReader.IsDBNull(9)) row["MSTicks"] = unfilteredDataReader.GetInt64(9);
                        if(!unfilteredDataReader.IsDBNull(10)) row[StartTimeColumn] = unfilteredDataReader.GetDateTime(10);

                        xmlTable.Rows.Add(row);
                    }
                    LOG.Debug(string.Format("Added {0} DMV rows for active waits", dmvRows.ToString(CultureInfo.InvariantCulture)));
                }
                catch(Exception ex)
                {
                    LOG.Error("Error occurred filtering Active Waits", ex.Message);
                }
            }

            return GroupActiveWaits(xmlTable).CreateDataReader();
        }

        private bool PopulateRow(XmlNode doc, string eventPrefix, DataRow row)
        {
            var node =
                doc.SelectSingleNode(eventPrefix + @"data[@name='duration']");

            if (node == null)
            {
                return false;
            }

            row[WaitDurationColumn] = -1 * int.Parse(node.InnerText);
            row["Ticks"] = -1 * (long.Parse(row[WaitDurationColumn].ToString()));

            node = doc.SelectSingleNode(eventPrefix + @"action[@name='session_id']");
            if (node != null) row[SessionIdColumn] = int.Parse(node.InnerText);
            node = doc.SelectSingleNode(eventPrefix + @"data[@name='wait_type']/text");
            if (node != null) row[WaitTypeColumn] = node.InnerText;
            node = doc.SelectSingleNode(eventPrefix + @"action[@name='client_hostname']");
            if (node != null) row[HostNameColumn] = node.InnerText;
            node = doc.SelectSingleNode(eventPrefix + @"action[@name='client_app_name']");
            if (node != null) row[ProgramNameColumn] = node.InnerText;
            node = doc.SelectSingleNode(eventPrefix + @"action[@name='username']");
            if (node != null) row[LoginNameColumn] = node.InnerText.ToUpper();
            node = doc.SelectSingleNode(eventPrefix + @"action[@name='database_name']");
            if (node != null) row[DatabaseNameColumn] = node.InnerText;
            node = doc.SelectSingleNode(eventPrefix + @"action[@name='sql_text']");
            if (node != null) row[StatementTxtColumn] = node.InnerText;
            //node = doc.DocumentElement.SelectSingleNode(@"/event/action[@name='database_id']");
            node = doc.SelectSingleNode(eventPrefix + @"action[@name='collect_system_time']");
            if (node != null)
            {
                //End Time in Ticks
                row["MSTicks"] = DateTime.Parse(node.InnerText).ToUniversalTime().Ticks;
                //StartTime = EndTime - Duration
                row[StartTimeColumn] = DateTime.Parse(node.InnerText).ToUniversalTime()
                                       + new TimeSpan( (long.Parse(row[WaitDurationColumn].ToString())));
            }

            return true;
        }

        /// <summary>
        /// Group the xmltable - max(ticks), sum(duration), time is now.UTCtime. Group the rest.
        /// </summary>
        /// <param name="xmlTable">Datatable of filtered data</param>
        /// <returns>Datatable containing the grouped data</returns>
        DataTable GroupActiveWaits(DataTable xmlTable)
        {
            
            bool initializedGroupedDatatable = false;

            var groupedTable = new DataTable("groupedTable");
            int groupedRows = 0;

            using (LOG.DebugCall("GroupActiveWaits"))
            {
                try
                {
                    //We need to group on everything other than ticks, waits and starttime
                    //min(ticks), sum(waitduration), starttime = getutctime
                    DateTime startTime = DateTime.UtcNow;
                    var grouped = new Dictionary<int, ActiveWaitsRow>();

                    foreach (DataRow unGroupedRow in xmlTable.Rows)
                    {
                        //create a new activerow object.
                        var thisNonGroupedRow = new ActiveWaitsRow(unGroupedRow) {StartTime = startTime};

                        //if the group does not already exist
                        if (!grouped.ContainsKey(thisNonGroupedRow.GetHashCode()))
                        {
                            //add the group
                            grouped.Add(thisNonGroupedRow.GetHashCode(), thisNonGroupedRow);
                        }
                        else  //if the group exists, update the existing group record
                        {
                            //max ticks
                            //if the ticks of this row exceed the value in the group, save this to the group
                            if (thisNonGroupedRow.Ticks > 0 && thisNonGroupedRow.Ticks > grouped[thisNonGroupedRow.GetHashCode()].Ticks)
                                grouped[thisNonGroupedRow.GetHashCode()].Ticks = thisNonGroupedRow.Ticks;

                            //sum duration
                            var groupDuration = grouped[thisNonGroupedRow.GetHashCode()].WaitDuration;
                            if (groupDuration != null) grouped[thisNonGroupedRow.GetHashCode()].WaitDuration = groupDuration
                                                                              + thisNonGroupedRow.WaitDuration;
                            //start time is utc time
                            grouped[thisNonGroupedRow.GetHashCode()].StartTime = startTime;

                            //Min MSTicks
                            if (thisNonGroupedRow.MSTicks > 0 && thisNonGroupedRow.MSTicks < grouped[thisNonGroupedRow.GetHashCode()].MSTicks)
                                grouped[thisNonGroupedRow.GetHashCode()].MSTicks = thisNonGroupedRow.MSTicks;
                        }
                    }

                    //transfer the grouped data to a data table
                    foreach (KeyValuePair<int, ActiveWaitsRow> kvp in grouped)
                    {
                        if (!initializedGroupedDatatable)
                            initializedGroupedDatatable = InitActiveWaitsTable(ref groupedTable);

                        groupedRows++;

                        DataRow row = groupedTable.NewRow();

                        //changing ticks to duration in ms. This was derived before from ticks but we now have it
                        row["Ticks"] = kvp.Value.Ticks;
                        if(kvp.Value.WaitDuration.HasValue) row[WaitDurationColumn] = kvp.Value.WaitDuration;
                        if (kvp.Value.MSTicks.HasValue) row["MSTicks"] = kvp.Value.MSTicks;
                        row[StartTimeColumn] = kvp.Value.StartTime;


                        if (kvp.Value.SessionId.HasValue) row[SessionIdColumn] = kvp.Value.SessionId;
                        row[WaitTypeColumn] = kvp.Value.WaitType;
                        row[HostNameColumn] = kvp.Value.HostName;
                        row[ProgramNameColumn] = kvp.Value.ProgramName;
                        row[LoginNameColumn] = kvp.Value.LoginName;
                        row[DatabaseNameColumn] = kvp.Value.DatabaseName;
                        row[StatementTxtColumn] = kvp.Value.StatementTxt;

                        groupedTable.Rows.Add(row);
                    }
                    LOG.Debug(string.Format("Added {0} grouped rows for active waits",
                                            groupedRows.ToString(CultureInfo.InvariantCulture)));
                }
                catch (Exception e)
                {
                    LOG.Error("Error occurred grouping Active Waits", e.Message);
                }
            }

            return groupedTable;
        }

        /// <summary>
        /// Initialize data table with columns for WaitTypes
        /// </summary>
        /// <param name="table"></param>
        /// <returns>true when successful</returns>
        bool InitActiveWaitsTable(ref DataTable table)
        {
            try
            {
                LOG.Debug(string.Format("Initializing {0}", table.TableName));

                //Initialize the data table
                // ReSharper disable AssignNullToNotNullAttribute
                table.Columns.Add("Ticks", Type.GetType("System.Int64"));

                table.Columns.Add(WaitDurationColumn, Type.GetType("System.Int64"));
                table.Columns.Add(SessionIdColumn, Type.GetType("System.Int32"));
                table.Columns.Add(WaitTypeColumn, Type.GetType("System.String"));
                table.Columns.Add(HostNameColumn, Type.GetType("System.String"));
                table.Columns.Add(ProgramNameColumn, Type.GetType("System.String"));
                table.Columns.Add(LoginNameColumn, Type.GetType("System.String"));
                table.Columns.Add(DatabaseNameColumn, Type.GetType("System.String"));
                table.Columns.Add(StatementTxtColumn, Type.GetType("System.String"));
                table.Columns.Add("MSTicks", Type.GetType("System.Int64"));
                table.Columns.Add(StartTimeColumn, Type.GetType("System.DateTime"));
                // ReSharper restore AssignNullToNotNullAttribute
                return true;
            }
            catch(Exception e)
            {
                LOG.Error(e.Message);
                return false;
            }
        }

        #endregion

        #region interface implementations

        #endregion
    }
    
    /// <summary>
    /// Class for grouping of ActiveWaits rows.
    /// </summary>
    public class ActiveWaitsRow
    {
        private readonly int? session_id;
        private readonly string waitType;
        private readonly string hostName;
        private readonly string program_name;
        private readonly string loginName;
        private readonly string database_name;
        private readonly string statement_txt;
        private long? msTicks;

        private long? ticks;
        private long? waitDuration;
        private DateTime startTime;

        /// <summary>
        /// Construct with populated data row
        /// </summary>
        /// <param name="row">Fully populated active waits data row</param>
        public ActiveWaitsRow(DataRow row)
        {
            if (row["Ticks"] != null && row["Ticks"] != DBNull.Value) ticks = (long)row["Ticks"];
            if (row["WaitDuration"] != null && row["WaitDuration"] != DBNull.Value) waitDuration = (long)row["WaitDuration"];
            if (row["session_id"] != null && row["session_id"] != DBNull.Value) session_id = (int)row["session_id"];
            if (row["Wait Type"] != null && row["Wait Type"] != DBNull.Value) waitType = (string)row["Wait Type"];
            if (row["HostName"] != null && row["HostName"] != DBNull.Value) hostName = (string)row["HostName"];
            if (row["program_name"] != null && row["program_name"] != DBNull.Value) program_name = (string)row["program_name"];
            if (row["LoginName"] != null && row["LoginName"] != DBNull.Value) loginName = (string)row["LoginName"];
            if (row["DatabaseName"] != null && row["DatabaseName"] != DBNull.Value) database_name = (string)row["DatabaseName"];
            if (row["statement_txt"] != null && row["statement_txt"] != DBNull.Value) statement_txt = (string)row["statement_txt"];
            if (row["MSTicks"] != null && row["MSTicks"] != DBNull.Value) msTicks = (long)row["MSTicks"];
        }

        #region Properties
        
        public int? SessionId
        {
            get { return session_id; }
        }

        public string WaitType
        {
            get { return waitType; }
        }

        public string HostName
        {
            get { return hostName; }
        }

        public string ProgramName
        {
            get { return program_name; }
        }

        public string LoginName
        {
            get { return loginName; }
        }

        public string DatabaseName
        {
            get { return database_name; }
        }

        public string StatementTxt
        {
            get { return statement_txt; }
        }
        
        /// <summary>
        /// MS Ticks at start time
        /// </summary>
        public long? MSTicks
        {
            get { return msTicks; }
            set { msTicks = value; }
        }

        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }
        
        /// <summary>
        /// Sum of all waits durations
        /// </summary>
        public long? WaitDuration
        {
            get { return waitDuration; }
            set { waitDuration = value; }
        }

        //Duration Ticks ms
        public long? Ticks
        {
            get { return ticks; }
            set { ticks = value; }
        }
        #endregion

        /// <summary>
        /// Check equality of group by elements
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ActiveWaitsRow other)
        {
            return GetHashCode() == other.GetHashCode();
            //return this.session_id == other.session_id &&
            //       this.waitType == other.waitType &&
            //       this.hostName == other.hostName &&
            //       this.program_name == other.program_name &&
            //       this.loginName == other.loginName &&
            //       this.database_name == other.database_name &&
            //       this.statement_txt == other.statement_txt;
        }

        /// <summary>
        /// Get a hash code that is unique for all non-group by columns in the row
        /// </summary>
        /// <returns>hash of group by elements</returns>
        public override int GetHashCode()
        {
            int resultHashCode = 0;
            if (this.session_id != null)
                resultHashCode = resultHashCode ^ this.session_id.GetHashCode();
            if (this.waitType != null)
                resultHashCode = resultHashCode ^ this.waitType.GetHashCode();
            if (this.hostName != null)
                resultHashCode = resultHashCode ^ this.hostName.GetHashCode();
            if (this.program_name != null)
                resultHashCode = resultHashCode ^ this.program_name.GetHashCode();
            if (this.loginName != null)
                resultHashCode = resultHashCode ^ this.loginName.GetHashCode();
            if (this.database_name != null)
                resultHashCode = resultHashCode ^ this.database_name.GetHashCode();
            if (this.statement_txt != null)
                resultHashCode = resultHashCode ^ this.statement_txt.GetHashCode();
            return resultHashCode;
        }
    }
}
