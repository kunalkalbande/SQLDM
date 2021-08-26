using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.ApplicationBlocks.Data;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using BBS.TracerX;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;

namespace Idera.SQLdm.ImportWizard.Objects
{
    public static class SQLdm5x
    {
        #region types

        public sealed class RepositoryInfo
        {
            #region members

            private string m_VersionString;
            private bool m_IsSysAdmin;
            private bool m_IsSQLdmAdmin;

            #endregion

            #region ctors

            public RepositoryInfo(
                    string versionString,
                    bool isSysAdmin,
                    bool isSQLdmAdmin
                )
            {
                m_VersionString = versionString;
                m_IsSysAdmin = isSysAdmin;
                m_IsSQLdmAdmin = isSQLdmAdmin;
            }

            #endregion

            #region properties

            public string VersionString
            {
                get { return m_VersionString; }
            }

            public bool IsValidVersion
            {
                get { return CheckRepositoryVersion(m_VersionString); }
            }

            public bool IsSysAdmin
            {
                get { return m_IsSysAdmin; }
            }

            public bool IsSQLdmAdmin
            {
                get { return m_IsSQLdmAdmin; }
            }

            #endregion

            #region methods

            public static bool CheckRepositoryVersion(string version)
            {
                return version == Constants.ValidRepositorySchemaVersion;
            }

            #endregion
        }

        public sealed class MonitoredSqlServer : IComparable
        {
            #region members

            private string m_Instance;
            private string m_RegKeyName;
            private int m_Id;
            private string m_Version;
            private DateTime m_RegistrationTimeUTC;
            private DateTime m_EarliestImportTimeUTC;

            #endregion

            #region Ctors

            public MonitoredSqlServer(
                    SqlString instance,
                    SqlInt32 id,
                    SqlString version,
                    SqlDateTime registrationTime,
                    SqlDateTime earliestImportTime
                )
            {
                Debug.Assert(!instance.IsNull && !id.IsNull && !registrationTime.IsNull);

                m_Instance = instance.Value.ToUpper();
                m_RegKeyName = m_Instance;
                m_Id = id.Value;
                m_Version = version.IsNull ? string.Empty : version.Value;
                m_RegistrationTimeUTC = registrationTime.Value;
                m_EarliestImportTimeUTC = earliestImportTime.IsNull ? DateTime.MinValue : earliestImportTime.Value;
            }

            #endregion

            #region properties

            public string Instance {
                get { return m_Instance; }
            }

            /// <summary>
            /// The registry key in 4.x registry tree for this server.
            /// Normally equal to Instance, but may be "(local)", ".", or "127.0.0.1"
            /// for the local computer.
            /// </summary>
            public string RegKeyName {
                get {return m_RegKeyName; }
                set { m_RegKeyName = value; }
            }

            public int Id
            {
                get { return m_Id; }
            }

            public string Version
            {
                get { return m_Version; }
            }

            public DateTime RegistrationTimeUTC
            {
                get { return m_RegistrationTimeUTC; }
            }

            public DateTime RegistrationTimeLocal
            {
                get { return RegistrationTimeUTC.ToLocalTime(); }
            }

            public DateTime EarliestImportTimeUTC
            {
                get { return m_EarliestImportTimeUTC; }
            }

            public DateTime EarliestImportTimeLocal
            {
                get { return EarliestImportTimeUTC.ToLocalTime(); }
            }

            public bool IsNeverImported
            {
                get { return EarliestImportTimeUTC == DateTime.MinValue; }
            }

            public DateTime EffectiveImportTimeUTC
            {
                get
                {
                    // If never been imported, then effective import
                    // date is registration date.   Else its the min
                    // of the two dates.
                    if (IsNeverImported)
                    {
                        return RegistrationTimeUTC;
                    }
                    else
                    {
                        return (EarliestImportTimeUTC < RegistrationTimeUTC ?
                                    EarliestImportTimeUTC : RegistrationTimeUTC);
                    }
                }
            }

            public DateTime EffectiveImportTimeLocal
            {
                get { return EffectiveImportTimeUTC.ToLocalTime(); }
            }

            public DateTime EffectiveImportDateLocal
            {
                get { return EffectiveImportTimeLocal.Date; }
            }

            public string EffectiveImportDateLocalString
            {
                get { return EffectiveImportDateLocal.ToShortDateString(); }
            }

            #endregion

            #region methods

            public override bool Equals(object obj)
            {
                if (obj == null) { return false; }

                if (this.GetType() != obj.GetType()) { return false; }

                MonitoredSqlServer other = (MonitoredSqlServer)obj;

                return m_Instance == other.m_Instance;
            }

            public override int GetHashCode()
            {
                return m_Instance.GetHashCode();
            }

            public override string ToString()
            {
                return m_Instance;
            }

            public int CompareTo(object obj)
            {
                // Cast to SqlServer object.
                MonitoredSqlServer s = obj as MonitoredSqlServer;
                if (s == null) { throw new ArgumentException("object is not a MonitoredSqlServer"); }

                // Do comparison.
                return string.Compare(m_Instance, s.m_Instance, true);
            }

            public bool IsImportDateValid(DateTime date)
            {
                Debug.Assert(date != DateTime.MinValue || date != DateTime.MaxValue);

                // If the input date is less then or equal effective import time,
                // then its valid.   Note the input time should be in local time.
                return EffectiveImportDateLocal >= date;
            }

            public bool GetTimeStampRangeLocal(
                    DateTime date,
                    out DateTime start,
                    out DateTime end
                )
            {
                // Init return.
                start = end = DateTime.MinValue;

                // Check if date is valid.
                bool ret = IsImportDateValid(date);

                // If date is valid for import, then compute start
                // and end times.
                if (ret)
                {
                    // Set start time to the input date.
                    start = date;

                    // If the effective import date part is the same as input date part,
                    // then we have to set the end to the effective import date.
                    // Else we set the end to the end of the day.
                    if (EffectiveImportDateLocal == date.Date)
                    {
                        end = EffectiveImportTimeLocal;
                    }
                    else
                    {
                        end = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
                    }

                    // If the start and end dates are the same then there is no
                    // data to import.
                    ret = end > start;
                }

                return ret;
            }

            public static void GetEffectiveImportDateRange(
                    List<SQLdm5x.MonitoredSqlServer> servers,
                    out DateTime startDate,
                    out DateTime endDate
                )
            {
                Debug.Assert(servers.Count > 0);

                // Init return.
                startDate = DateTime.MaxValue;
                endDate = DateTime.MinValue;

                foreach (SQLdm5x.MonitoredSqlServer s in servers)
                {
                    DateTime effectiveImportDate = s.EffectiveImportDateLocal;
                    if (effectiveImportDate < startDate)
                    {
                        startDate = effectiveImportDate;
                    }
                    if (effectiveImportDate > endDate)
                    {
                        endDate = effectiveImportDate;
                    }
                }
            }

            public static DateTime GetHighestEffectiveImportDate(
                    List<SQLdm5x.MonitoredSqlServer> servers
                )
            {
                Debug.Assert(servers.Count > 0);

                DateTime highest = DateTime.MinValue;
                foreach (SQLdm5x.MonitoredSqlServer s in servers)
                {
                    DateTime effectiveImportDate = s.EffectiveImportDateLocal;
                    if (effectiveImportDate > highest)
                    {
                        highest = effectiveImportDate;
                    }
                }

                return highest;
            }

            #endregion
        }

        #endregion

        #region constants

        #region SPs

        private const string GetRepositoryInfoStoredProcedure = "p_RepositoryInfo";
        private const string GetMonitoredSqlServersStoredProcedure = "p_ImportGetMonitoredSqlServers";
        private const string InsertDatabaseNameStoredProcedure = "p_InsertDatabaseName";
        private const string InsertTableNameStoredProcedure = "p_InsertTableName";
        private const string ImportUpdateEarliestDateImportedStoredProcedure = "p_ImportUpdateEarliestDateImported";

        #endregion

        #region ServerStatistics Table

        private const string ServerStatisticsTableName = "dbo.ServerStatistics";

        private const string ParamServerStatsSQLServerID = "SQLServerID";
        private const string ParamServerStatsUTCCollectionDateTime = "UTCCollectionDateTime";
        private const string ParamServerStatsTimeDeltaInSeconds = "TimeDeltaInSeconds";
        private const string ParamServerStatsAgentServiceStatus = "AgentServiceStatus";
        private const string ParamServerStatsSqlServerServiceStatus = "SqlServerServiceStatus";
        private const string ParamServerStatsDTCServiceStatus = "DTCServiceStatus";
        private const string ParamServerStatsFullTextSearchStatus = "FullTextSearchStatus";
        private const string ParamServerStatsBufferCacheHitRatioPercentage = "BufferCacheHitRatioPercentage";
        private const string ParamServerStatsCheckpointWrites = "CheckpointWrites";
        private const string ParamServerStatsClientComputers = "ClientComputers";
        private const string ParamServerStatsCPUActivityPercentage = "CPUActivityPercentage";
        private const string ParamServerStatsCPUTimeDelta = "CPUTimeDelta";
        private const string ParamServerStatsCPUTimeRaw = "CPUTimeRaw";
        private const string ParamServerStatsFullScans = "FullScans";
        private const string ParamServerStatsIdleTimeDelta = "IdleTimeDelta";
        private const string ParamServerStatsIdleTimePercentage = "IdleTimePercentage";
        private const string ParamServerStatsIdleTimeRaw = "IdleTimeRaw";
        private const string ParamServerStatsIOActivityPercentage = "IOActivityPercentage";
        private const string ParamServerStatsIOTimeDelta = "IOTimeDelta";
        private const string ParamServerStatsIOTimeRaw = "IOTimeRaw";
        private const string ParamServerStatsLazyWriterWrites = "LazyWriterWrites";
        private const string ParamServerStatsLockWaits = "LockWaits";
        private const string ParamServerStatsLogins = "Logins";
        private const string ParamServerStatsLogFlushes = "LogFlushes";
        private const string ParamServerStatsSqlMemoryAllocatedInKilobytes = "SqlMemoryAllocatedInKilobytes";
        private const string ParamServerStatsSqlMemoryUsedInKilobytes = "SqlMemoryUsedInKilobytes";
        private const string ParamServerStatsOldestOpenTransactionsInMinutes = "OldestOpenTransactionsInMinutes";
        private const string ParamServerStatsPacketErrors = "PacketErrors";
        private const string ParamServerStatsPacketsReceived = "PacketsReceived";
        private const string ParamServerStatsPacketsSent = "PacketsSent";
        private const string ParamServerStatsPageErrors = "PageErrors";
        private const string ParamServerStatsPageLifeExpectancy = "PageLifeExpectancy";
        private const string ParamServerStatsPageLookups = "PageLookups";
        private const string ParamServerStatsPageReads = "PageReads";
        private const string ParamServerStatsPageSplits = "PageSplits";
        private const string ParamServerStatsPageWrites = "PageWrites";
        private const string ParamServerStatsProcedureCacheHitRatioPercentage = "ProcedureCacheHitRatioPercentage";
        private const string ParamServerStatsProcedureCacheSizeInKilobytes = "ProcedureCacheSizeInKilobytes";
        private const string ParamServerStatsProcedureCacheSizePercent = "ProcedureCacheSizePercent";
        private const string ParamServerStatsReadAheadPages = "ReadAheadPages";
        private const string ParamServerStatsReplicationLatencyInSeconds = "ReplicationLatencyInSeconds";
        private const string ParamServerStatsReplicationSubscribed = "ReplicationSubscribed";
        private const string ParamServerStatsReplicationUndistributed = "ReplicationUndistributed";
        private const string ParamServerStatsReplicationUnsubscribed = "ReplicationUnsubscribed";
        private const string ParamServerStatsResponseTimeInMilliseconds = "ResponseTimeInMilliseconds";
        private const string ParamServerStatsServerVersion = "ServerVersion";
        private const string ParamServerStatsSqlCompilations = "SqlCompilations";
        private const string ParamServerStatsSqlRecompilations = "SqlRecompilations";
        private const string ParamServerStatsTableLockEscalations = "TableLockEscalations";
        private const string ParamServerStatsTempDBSizeInKilobytes = "TempDBSizeInKilobytes";
        private const string ParamServerStatsTempDBSizePercent = "TempDBSizePercent";
        private const string ParamServerStatsTransactions = "Transactions";
        private const string ParamServerStatsUserProcesses = "UserProcesses";
        private const string ParamServerStatsWorkFilesCreated = "WorkFilesCreated";
        private const string ParamServerStatsWorkTablesCreated = "WorkTablesCreated";
        private const string ParamServerStatsSystemProcesses = "SystemProcesses";
        private const string ParamServerStatsUserProcessesConsumingCPU = "UserProcessesConsumingCPU";
        private const string ParamServerStatsSystemProcessesConsumingCPU = "SystemProcessesConsumingCPU";
        private const string ParamServerStatsBlockedProcesses = "BlockedProcesses";
        private const string ParamServerStatsOpenTransactions = "OpenTransactions";
        private const string ParamServerStatsDatabaseCount = "DatabaseCount";
        private const string ParamServerStatsDataFileCount = "DataFileCount";
        private const string ParamServerStatsLogFileCount = "LogFileCount";
        private const string ParamServerStatsDataFileSpaceAllocatedInKilobytes = "DataFileSpaceAllocatedInKilobytes";
        private const string ParamServerStatsDataFileSpaceUsedInKilobytes = "DataFileSpaceUsedInKilobytes";
        private const string ParamServerStatsLogFileSpaceAllocatedInKilobytes = "LogFileSpaceAllocatedInKilobytes";
        private const string ParamServerStatsLogFileSpaceUsedInKilobytes = "LogFileSpaceUsedInKilobytes";
        private const string ParamServerStatsTotalLocks = "TotalLocks";
        private const string ParamServerStatsBufferCacheSizeInKilobytes = "BufferCacheSizeInKilobytes";

        #endregion

        #region OSStatistics Table

        private const string OSStatisticsTableName = "dbo.OSStatistics";

        private const string ParamOSStatsSQLServerID = "SQLServerID";
        private const string ParamOSStatsUTCCollectionDateTime = "UTCCollectionDateTime";
        private const string ParamOSStatsOSTotalPhysicalMemoryInKilobytes = "OSTotalPhysicalMemoryInKilobytes";
        private const string ParamOSStatsOSAvailableMemoryInKilobytes = "OSAvailableMemoryInKilobytes";
        private const string ParamOSStatsPagesPerSecond = "PagesPerSecond";
        private const string ParamOSStatsProcessorTimePercent = "ProcessorTimePercent";
        private const string ParamOSStatsPrivilegedTimePercent = "PrivilegedTimePercent";
        private const string ParamOSStatsUserTimePercent = "UserTimePercent";
        private const string ParamOSStatsProcessorQueueLength = "ProcessorQueueLength";
        private const string ParamOSStatsDiskTimePercent = "DiskTimePercent";
        private const string ParamOSStatsDiskQueueLength = "DiskQueueLength";

        #endregion

        #region DatabaseStatistics Table

        private const string DatabaseStatisticsTableName = "dbo.DatabaseStatistics";

        private const string ParamDbStatsDatabaseID = "DatabaseID";
        private const string ParamDbStatsUTCCollectionDateTime = "UTCCollectionDateTime";
        private const string ParamDbStatsDatabaseStatus = "DatabaseStatus";
	    private const string ParamDbStatsTransactions = "Transactions";
        private const string ParamDbStatsLogFlushWaits = "LogFlushWaits";
	    private const string ParamDbStatsLogFlushes = "LogFlushes";
        private const string ParamDbStatsLogKilobytesFlushed = "LogKilobytesFlushed";
        private const string ParamDbStatsLogCacheReads = "LogCacheReads";
        private const string ParamDbStatsLogCacheHitRatio = "LogCacheHitRatio";
        private const string ParamDbStatsDataFileSizeInKilobytes = "DataFileSizeInKilobytes";
        private const string ParamDbStatsLogFileSizeInKilobytes = "LogFileSizeInKilobytes";
        private const string ParamDbStatsDataSizeInKilobytes = "DataSizeInKilobytes";
        private const string ParamDbStatsLogSizeInKilobytes = "LogSizeInKilobytes";
        private const string ParamDbStatsTextSizeInKilobytes = "TextSizeInKilobytes";
        private const string ParamDbStatsIndexSizeInKilobytes = "IndexSizeInKilobytes";
        private const string ParamDbStatsLogExpansionInKilobytes = "LogExpansionInKilobytes";
        private const string ParamDbStatsDataExpansionInKilobytes = "DataExpansionInKilobytes";
        private const string ParamDbStatsPercentLogSpace = "PercentLogSpace";
        private const string ParamDbStatsPercentDataSize = "PercentDataSize";
        private const string ParamDbStatsTimeDeltaInSeconds = "TimeDeltaInSeconds";

        #endregion

        #region TableGrowth Table

        private const string TableGrowthTableName = "dbo.TableGrowth";

	    private const string ParamTableGrowthTableID = "TableID";
	    private const string ParamTableGrowthUTCCollectionDateTime = "UTCCollectionDateTime";
	    private const string ParamTableGrowthNumberOfRows = "NumberOfRows";
	    private const string ParamTableGrowthDataSize = "DataSize";
	    private const string ParamTableGrowthTextSize = "TextSize";
	    private const string ParamTableGrowthIndexSize = "IndexSize";
        private const string ParamTableGrowthTimeDeltaInSeconds = "TimeDeltaInSeconds";

        #endregion

        #region TableReorganization Table

        private const string TableReorganizationTableName = "dbo.TableReorganization";

	    private const string ParamTableReorgTableID = "TableID";
        private const string ParamTableReorgUTCCollectionDateTime = "UTCCollectionDateTime";
        private const string ParamTableReorgScanDensity = "ScanDensity";
        private const string ParamTableReorgLogicalFragmentation = "LogicalFragmentation";
        private const string ParamTableReorgTimeDeltaInSeconds = "TimeDeltaInSeconds";

        #endregion

        #region QueryMonitor Table

        private const string QueryMonitorTableName = "dbo.QueryMonitor";

	    private const string ParamQueryMonitorSQLServerID = "SQLServerID";
	    private const string ParamQueryMonitorUTCCollectionDateTime = "UTCCollectionDateTime";
	    private const string ParamQueryMonitorDatabaseID = "DatabaseID";
	    private const string ParamQueryMonitorCompletionTime = "CompletionTime";
	    private const string ParamQueryMonitorDurationMilliseconds = "DurationMilliseconds";
	    private const string ParamQueryMonitorCPUMilliseconds = "CPUMilliseconds";
	    private const string ParamQueryMonitorReads = "Reads";
	    private const string ParamQueryMonitorWrites = "Writes";
	    private const string ParamQueryMonitorNtUserName = "NtUserName";
	    private const string ParamQueryMonitorSqlUserName = "SqlUserName";
	    private const string ParamQueryMonitorClientComputerName = "ClientComputerName";
	    private const string ParamQueryMonitorApplicationName = "ApplicationName";
	    private const string ParamQueryMonitorStatementType = "StatementType";
        private const string ParamQueryMonitorStatementText = "StatementText";

        #endregion

        #endregion

        #region members

        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger(typeof(Program));

        #endregion

        #region methods

        public static SQLdm5x.RepositoryInfo GetRepositoryInfo(
                SqlConnectionInfo connectionInfo
            )
        {
            Debug.Assert(connectionInfo != null);

            // Get version string from the repository.
            string versionString = string.Empty;
            bool isSysAdmin = false;
            bool isSQLdmAdmin = false;
            if (connectionInfo != null)
            {
                using (SqlConnection connection = connectionInfo.GetConnection())
                {
                    try
                    {
                        using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure,
                                                                                    GetRepositoryInfoStoredProcedure))
                        {
                            while (dataReader.Read())
                            {
                                switch (dataReader.GetString(0))
                                {
                                    case "Repository Version":
                                        versionString = dataReader.GetString(2);
                                        break;
                                }
                            }
                        }

                        // Get permission.
                        if (SQLdm5x.RepositoryInfo.CheckRepositoryVersion(versionString))
                        {
                            UserToken ut = new UserToken();
                            ut.Refresh(connectionInfo.ConnectionString);
                            isSysAdmin = ut.IsSysadmin;
                            isSQLdmAdmin = ut.IsSQLdmAdministrator;
                        }
                    }
                    catch (SqlException e)
                    {
                        // Assuming that a valid connection can be established to the SQL Server, 
                        // an invalid call to the version procedure would indicate an invalid database;
                        // all other exceptions will be passed on.
                        //
                        // Error 2812 = cannot find stored procedure in SQL Server 2000 & 2005
                        //
                        Log.Error(e);
                        if (e.Number == 2812)
                        {
                            return null;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }

            return new SQLdm5x.RepositoryInfo(versionString, isSysAdmin, isSQLdmAdmin);
        }

        public static List<SQLdm5x.MonitoredSqlServer> RegisteredSQLServers(
                SqlConnectionInfo connectionInfo
            )
        {
            Debug.Assert(connectionInfo != null);

            // Create return list.
            List<SQLdm5x.MonitoredSqlServer> instances = new List<SQLdm5x.MonitoredSqlServer>();

            // Get monitored sql server instances.
            if (connectionInfo != null)
            {
                using (SqlConnection connection = connectionInfo.GetConnection())
                {
                    try
                    {
                        // If not sysadmin throw exception.
                        RepositoryInfo ri = GetRepositoryInfo(connectionInfo);
                        if (!ri.IsSysAdmin)
                        {
                            throw (new Exception("To import historical data you must connect to the SQLdm Repository as a member of the sysadmin role."));
                        }

                        // Get registered servers.
                        using (SqlDataReader dataReader =
                                SqlHelper.ExecuteReader(connection, GetMonitoredSqlServersStoredProcedure, true))
                        {
                            while (dataReader.Read())
                            {
                                SqlInt32 sqlServerID = dataReader.GetSqlInt32(0);
                                SqlString instanceName = dataReader.GetSqlString(1);
                                SqlString serverVersion = dataReader.GetSqlString(2);
                                SqlBoolean active = dataReader.GetSqlBoolean(3);
                                SqlDateTime registeredDate = dataReader.GetSqlDateTime(4);
                                SqlDateTime earliestDateImportedFromLegacySQLdm = dataReader.GetSqlDateTime(5);
                                instances.Add(new SQLdm5x.MonitoredSqlServer(instanceName, sqlServerID, serverVersion, registeredDate,
                                                    earliestDateImportedFromLegacySQLdm));
                            }
                        }
                    }
                    catch (SqlException e)
                    {
                        // Assuming that a valid connection can be established to the SQL Server, 
                        // an invalid call to the version procedure would indicate an invalid database;
                        // all other exceptions will be passed on.
                        //
                        // Error 2812 = cannot find stored procedure in SQL Server 2000 & 2005
                        //
                        Log.Error(e);
                        if (e.Number == 2812)
                        {
                            return null;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }

            return instances;
        }

        public static bool WriteData(
                SqlConnectionInfo connectionInfo,
                int serverId,
                SQLdm4xData data,
                out string errStr
            )
        {
            Debug.Assert(connectionInfo != null && data != null);

            // Init return.
            bool isError = false;
            errStr = string.Empty;

            // Check inputs.
            if (connectionInfo == null || data == null)
            {
                Log.Error("Connection information or historical data is not specified");
                errStr = "Connection information or historical data is not specified";
                isError = true;
            }

            // Create SQL connection object.
            if (!isError)
            {
                using (SqlConnection connection = connectionInfo.GetConnection())
                {
                    try
                    {
                        // Open the connection.
                        connection.Open();

                        // Create a transaction object.
                        using (SqlTransaction transaction = connection.BeginTransaction("Import"))
                        {
                            // Write server statistics
                            writeServerStatistics(connection, transaction, serverId, data.ServerStatisticsData);

                            // Write OS statistcis
                            writeOSStatistics(connection, transaction, serverId, data.OSMetricsData);

                            // Write worst performing data
                            writeWorstPeformingData(connection, transaction, serverId, data.WorstPerformingData);

                            // For each database
                            foreach (SQLdm4xData.Database db in data.DatabaseData)
                            {
                                // Write database statistics & database space statistics
                                writeDatabaseStatistics(connection, transaction, serverId, db);

                                // Write table statistics.
                                writeTableStatistics(connection, transaction, serverId, db);
                            }

                            // Update server's latest import date.
                            updateImportDate(transaction, serverId, data.StartUTC);

                            // Commit transaction.
                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Exception encountered when writing data to the Repository - ", ex);
                        errStr = ex.Message;
                        isError = true;
                    }
                }
            }

            return isError;
        }

        #region ServerStatistics helpers

        private static void writeServerStatistics(
                SqlConnection connection,
                SqlTransaction transaction,
                int serverId,
                List<SQLdm4xData.ServerStatistics> serverStatistics
            )
        {
            // Create bulk copy object.
            using (SqlBulkCopy bcp = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
            {
                // Check bcp.
                if (bcp == null)
                {
                    Log.Error("bcp object is null, no further processing will be done.");
                    return;
                }

                // Set the destination table and time out.
                bcp.DestinationTableName = ServerStatisticsTableName;
                bcp.BulkCopyTimeout = global::Idera.SQLdm.ImportWizard.Properties.Settings.Default.TimeOut;

                // Create the datatable object.
                using (DataTable dt = createServerStatisticsDataTable())
                {
                    // Check the datatable object.
                    if (dt == null)
                    {
                        Log.Error("datatable object is null, no further processing will be done.");
                        return;
                    }

                    // Fill the server statistics.
                    foreach (SQLdm4xData.ServerStatistics s in serverStatistics)
                    {
                        // Create new data row.
                        DataRow dr = dt.NewRow();

                        // Fill the data row with data.
                        dr[ParamServerStatsSQLServerID] = new SqlInt32(serverId);
                        dr[ParamServerStatsUTCCollectionDateTime] = s.UTCCollectionDateTime;
                        dr[ParamServerStatsTimeDeltaInSeconds] = s.TimeDeltaInSeconds;
                        dr[ParamServerStatsCPUActivityPercentage] = s.CPUActivityPercentage;
                        dr[ParamServerStatsIOActivityPercentage] = s.IOActivityPercentage;
                        dr[ParamServerStatsPageReads] = s.PageReads;
                        dr[ParamServerStatsPageWrites] = s.PageWrites;
                        dr[ParamServerStatsPageErrors] = s.PageErrors;
                        dr[ParamServerStatsLazyWriterWrites] = s.LazyWriterWrites;
                        dr[ParamServerStatsCheckpointWrites] = s.CheckpointWrites;
                        dr[ParamServerStatsLogFlushes] = s.LogFlushes;
                        dr[ParamServerStatsPacketsReceived] = s.PacketsReceived;
                        dr[ParamServerStatsPacketsSent] = s.PacketsSent;
                        dr[ParamServerStatsPacketErrors] = s.PacketErrors;
                        dr[ParamServerStatsLogins] = s.Logins;
                        dr[ParamServerStatsUserProcesses] = s.UserProcesses;
                        dr[ParamServerStatsClientComputers] = s.ClientComputers;
                        dr[ParamServerStatsTransactions] = s.Transactions;
                        dr[ParamServerStatsOldestOpenTransactionsInMinutes] = s.OldestOpenTransactionsInMinutes;
                        dr[ParamServerStatsReplicationUnsubscribed] = s.ReplicationUnsubscribed;
                        dr[ParamServerStatsReplicationSubscribed] = s.ReplicationSubscribed;
                        dr[ParamServerStatsReplicationUndistributed] = s.ReplicationUndistributed;
                        dr[ParamServerStatsReplicationLatencyInSeconds] = s.ReplicationLatencyInSeconds;
                        dr[ParamServerStatsTempDBSizePercent] = s.TempDBSizePercent;
                        dr[ParamServerStatsTempDBSizeInKilobytes] = s.TempDBSizeInKilobytes;
                        dr[ParamServerStatsProcedureCacheSizePercent] = s.ProcedureCacheSizePercent;
                        dr[ParamServerStatsProcedureCacheSizeInKilobytes] = s.ProcedureCacheSizeInKilobytes;
                        dr[ParamServerStatsBufferCacheHitRatioPercentage] = s.BufferCacheHitRatioPercentage;
                        dr[ParamServerStatsProcedureCacheHitRatioPercentage] = s.ProcedureCacheHitRatioPercentage;
                        dr[ParamServerStatsResponseTimeInMilliseconds] = s.ResponseTimeInMilliseconds;
                        dr[ParamServerStatsSqlCompilations] = s.SqlCompilations;
                        dr[ParamServerStatsSqlRecompilations] = s.SqlRecompilations;
                        dr[ParamServerStatsWorkFilesCreated] = s.WorkFilesCreated;
                        dr[ParamServerStatsWorkTablesCreated] = s.WorkTablesCreated;
                        dr[ParamServerStatsSqlMemoryAllocatedInKilobytes] = s.SqlMemoryAllocatedInKilobytes;
                        dr[ParamServerStatsSqlMemoryUsedInKilobytes] = s.SqlMemoryUsedInKilobytes;
                        dr[ParamServerStatsTableLockEscalations] = s.TableLockEscalations;
                        dr[ParamServerStatsReadAheadPages] = s.ReadAheadPages;
                        dr[ParamServerStatsPageSplits] = s.PageSplits;
                        dr[ParamServerStatsFullScans] = s.FullScans;
                        dr[ParamServerStatsPageLifeExpectancy] = s.PageLifeExpectancy;
                        dr[ParamServerStatsLockWaits] = s.LockWaits;
                        dr[ParamServerStatsPageLookups] = s.PageLookups;

                        // Add row to the data table.
                        dt.Rows.Add(dr);

                        // Write to repository.
                        if (dt.Rows.Count > global::Idera.SQLdm.ImportWizard.Properties.Settings.Default.NumRowsInBatch)
                        {
                            try
                            {
                                bcp.WriteToServer(dt);
                                dt.Clear();
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Exception encountered when writing Server Statistics to the Repository - ", ex);
                                throw ex;
                            }
                        }
                    }

                    // Write to repository, any remaining rows.
                    if (dt.Rows.Count > 0)
                    {
                        try
                        {
                            bcp.WriteToServer(dt);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Exception encountered when writing Server Statistics to the Repository - ", ex);
                            throw ex;
                        }
                    }
                }
            }
        }

        private static DataTable createServerStatisticsDataTable()
        {
            DataTable dt = null;

            using (DataColumn
                        colSQLServerID = new DataColumn(ParamServerStatsSQLServerID, typeof(SqlInt32)),
                        colUTCCollectionDateTime = new DataColumn(ParamServerStatsUTCCollectionDateTime, typeof(SqlDateTime)),
                        colTimeDeltaInSeconds = new DataColumn(ParamServerStatsTimeDeltaInSeconds, typeof(SqlDouble)),
                        colAgentServiceStatus = new DataColumn(ParamServerStatsAgentServiceStatus, typeof(SqlInt32)),
                        colSqlServerServiceStatus = new DataColumn(ParamServerStatsSqlServerServiceStatus, typeof(SqlInt32)),
                        colDTCServiceStatus = new DataColumn(ParamServerStatsDTCServiceStatus, typeof(SqlInt32)),
                        colFullTextSearchStatus = new DataColumn(ParamServerStatsFullTextSearchStatus, typeof(SqlInt32)),
                        colBufferCacheHitRatioPercentage = new DataColumn(ParamServerStatsBufferCacheHitRatioPercentage, typeof(SqlDouble)),
                        colCheckpointWrites = new DataColumn(ParamServerStatsCheckpointWrites, typeof(SqlInt64)),
                        colClientComputers = new DataColumn(ParamServerStatsClientComputers, typeof(SqlInt32)),
                        colCPUActivityPercentage = new DataColumn(ParamServerStatsCPUActivityPercentage, typeof(SqlDouble)),
                        colCPUTimeDelta = new DataColumn(ParamServerStatsCPUTimeDelta, typeof(SqlInt64)),
                        colCPUTimeRaw = new DataColumn(ParamServerStatsCPUTimeRaw, typeof(SqlInt64)),
                        colFullScans = new DataColumn(ParamServerStatsFullScans, typeof(SqlInt64)),
                        colIdleTimeDelta = new DataColumn(ParamServerStatsIdleTimeDelta, typeof(SqlInt64)),
                        colIdleTimePercentage = new DataColumn(ParamServerStatsIdleTimePercentage, typeof(SqlDouble)),
                        colIdleTimeRaw = new DataColumn(ParamServerStatsIdleTimeRaw, typeof(SqlInt64)),
                        colIOActivityPercentage = new DataColumn(ParamServerStatsIOActivityPercentage, typeof(SqlDouble)),
                        colIOTimeDelta = new DataColumn(ParamServerStatsIOTimeDelta, typeof(SqlInt64)),
                        colIOTimeRaw = new DataColumn(ParamServerStatsIOTimeRaw, typeof(SqlInt64)),
                        colLazyWriterWrites = new DataColumn(ParamServerStatsLazyWriterWrites, typeof(SqlInt64)),
                        colLockWaits = new DataColumn(ParamServerStatsLockWaits, typeof(SqlInt64)),
                        colLogins = new DataColumn(ParamServerStatsLogins, typeof(SqlInt64)),
                        colLogFlushes = new DataColumn(ParamServerStatsLogFlushes, typeof(SqlInt64)),
                        colSqlMemoryAllocatedInKilobytes = new DataColumn(ParamServerStatsSqlMemoryAllocatedInKilobytes, typeof(SqlInt64)),
                        colSqlMemoryUsedInKilobytes = new DataColumn(ParamServerStatsSqlMemoryUsedInKilobytes, typeof(SqlInt64)),
                        colOldestOpenTransactionsInMinutes = new DataColumn(ParamServerStatsOldestOpenTransactionsInMinutes, typeof(SqlInt64)),
                        colPacketErrors = new DataColumn(ParamServerStatsPacketErrors, typeof(SqlInt64)),
                        colPacketsReceived = new DataColumn(ParamServerStatsPacketsReceived, typeof(SqlInt64)),
                        colPacketsSent = new DataColumn(ParamServerStatsPacketsSent, typeof(SqlInt64)),
                        colPageErrors = new DataColumn(ParamServerStatsPageErrors, typeof(SqlInt64)),
                        colPageLifeExpectancy = new DataColumn(ParamServerStatsPageLifeExpectancy, typeof(SqlInt64)),
                        colPageLookups = new DataColumn(ParamServerStatsPageLookups, typeof(SqlInt64)),
                        colPageReads = new DataColumn(ParamServerStatsPageReads, typeof(SqlInt64)),
                        colPageSplits = new DataColumn(ParamServerStatsPageSplits, typeof(SqlInt64)),
                        colPageWrites = new DataColumn(ParamServerStatsPageWrites, typeof(SqlInt64)),
                        colProcedureCacheHitRatioPercentage = new DataColumn(ParamServerStatsProcedureCacheHitRatioPercentage, typeof(SqlDouble)),
                        colProcedureCacheSizeInKilobytes = new DataColumn(ParamServerStatsProcedureCacheSizeInKilobytes, typeof(SqlInt64)),
                        colProcedureCacheSizePercent = new DataColumn(ParamServerStatsProcedureCacheSizePercent, typeof(SqlDouble)),
                        colReadAheadPages = new DataColumn(ParamServerStatsReadAheadPages, typeof(SqlInt64)),
                        colReplicationLatencyInSeconds = new DataColumn(ParamServerStatsReplicationLatencyInSeconds, typeof(SqlDouble)),
                        colReplicationSubscribed = new DataColumn(ParamServerStatsReplicationSubscribed, typeof(SqlInt64)),
                        colReplicationUndistributed = new DataColumn(ParamServerStatsReplicationUndistributed, typeof(SqlInt64)),
                        colReplicationUnsubscribed = new DataColumn(ParamServerStatsReplicationUnsubscribed, typeof(SqlInt64)),
                        colResponseTimeInMilliseconds = new DataColumn(ParamServerStatsResponseTimeInMilliseconds, typeof(SqlInt32)),
                        colServerVersion = new DataColumn(ParamServerStatsServerVersion, typeof(SqlString)),
                        colSqlCompilations = new DataColumn(ParamServerStatsSqlCompilations, typeof(SqlInt64)),
                        colSqlRecompilations = new DataColumn(ParamServerStatsSqlRecompilations, typeof(SqlInt64)),
                        colTableLockEscalations = new DataColumn(ParamServerStatsTableLockEscalations, typeof(SqlInt64)),
                        colTempDBSizeInKilobytes = new DataColumn(ParamServerStatsTempDBSizeInKilobytes, typeof(SqlInt64)),
                        colTempDBSizePercent = new DataColumn(ParamServerStatsTempDBSizePercent, typeof(SqlDouble)),
                        colTransactions = new DataColumn(ParamServerStatsTransactions, typeof(SqlInt64)),
                        colUserProcesses = new DataColumn(ParamServerStatsUserProcesses, typeof(SqlInt32)),
                        colWorkFilesCreated = new DataColumn(ParamServerStatsWorkFilesCreated, typeof(SqlInt64)),
                        colWorkTablesCreated = new DataColumn(ParamServerStatsWorkTablesCreated, typeof(SqlInt64)),
                        colSystemProcesses = new DataColumn(ParamServerStatsSystemProcesses, typeof(SqlInt32)),
                        colUserProcessesConsumingCPU = new DataColumn(ParamServerStatsUserProcessesConsumingCPU, typeof(SqlInt32)),
                        colSystemProcessesConsumingCPU = new DataColumn(ParamServerStatsSystemProcessesConsumingCPU, typeof(SqlInt32)),
                        colBlockedProcesses = new DataColumn(ParamServerStatsBlockedProcesses, typeof(SqlInt32)),
                        colOpenTransactions = new DataColumn(ParamServerStatsOpenTransactions, typeof(SqlInt32)),
                        colDatabaseCount = new DataColumn(ParamServerStatsDatabaseCount, typeof(SqlInt32)),
                        colDataFileCount = new DataColumn(ParamServerStatsDataFileCount, typeof(SqlInt32)),
                        colLogFileCount = new DataColumn(ParamServerStatsLogFileCount, typeof(SqlInt32)),
                        colDataFileSpaceAllocatedInKilobytes = new DataColumn(ParamServerStatsDataFileSpaceAllocatedInKilobytes, typeof(SqlDecimal)),
                        colDataFileSpaceUsedInKilobytes = new DataColumn(ParamServerStatsDataFileSpaceUsedInKilobytes, typeof(SqlDecimal)),
                        colLogFileSpaceAllocatedInKilobytes = new DataColumn(ParamServerStatsLogFileSpaceAllocatedInKilobytes, typeof(SqlDecimal)),
                        colLogFileSpaceUsedInKilobytes = new DataColumn(ParamServerStatsLogFileSpaceUsedInKilobytes, typeof(SqlDecimal)),
                        colTotalLocks = new DataColumn(ParamServerStatsTotalLocks, typeof(SqlDecimal)),
                        colBufferCacheSizeInKilobytes = new DataColumn(ParamServerStatsBufferCacheSizeInKilobytes, typeof(SqlInt64))
                  )
            {
                dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[] {
                                            colSQLServerID,
                                            colUTCCollectionDateTime,
                                            colTimeDeltaInSeconds,
                                            colAgentServiceStatus,
                                            colSqlServerServiceStatus,
                                            colDTCServiceStatus,
                                            colFullTextSearchStatus,
                                            colBufferCacheHitRatioPercentage,
                                            colCheckpointWrites,
                                            colClientComputers,
                                            colCPUActivityPercentage,
                                            colCPUTimeDelta,
                                            colCPUTimeRaw,
                                            colFullScans,
                                            colIdleTimeDelta,
                                            colIdleTimePercentage,
                                            colIdleTimeRaw,
                                            colIOActivityPercentage,
                                            colIOTimeDelta,
                                            colIOTimeRaw,
                                            colLazyWriterWrites,
                                            colLockWaits,
                                            colLogins,
                                            colLogFlushes,
                                            colSqlMemoryAllocatedInKilobytes,
                                            colSqlMemoryUsedInKilobytes,
                                            colOldestOpenTransactionsInMinutes,
                                            colPacketErrors,
                                            colPacketsReceived,
                                            colPacketsSent,
                                            colPageErrors,
                                            colPageLifeExpectancy,
                                            colPageLookups,
                                            colPageReads,
                                            colPageSplits,
                                            colPageWrites,
                                            colProcedureCacheHitRatioPercentage,
                                            colProcedureCacheSizeInKilobytes,
                                            colProcedureCacheSizePercent,
                                            colReadAheadPages,
                                            colReplicationLatencyInSeconds,
                                            colReplicationSubscribed,
                                            colReplicationUndistributed,
                                            colReplicationUnsubscribed,
                                            colResponseTimeInMilliseconds,
                                            colServerVersion,
                                            colSqlCompilations,
                                            colSqlRecompilations,
                                            colTableLockEscalations,
                                            colTempDBSizeInKilobytes,
                                            colTempDBSizePercent,
                                            colTransactions,
                                            colUserProcesses,
                                            colWorkFilesCreated,
                                            colWorkTablesCreated,
                                            colSystemProcesses,
                                            colUserProcessesConsumingCPU,
                                            colSystemProcessesConsumingCPU,
                                            colBlockedProcesses,
                                            colOpenTransactions,
                                            colDatabaseCount,
                                            colDataFileCount,
                                            colLogFileCount,
                                            colDataFileSpaceAllocatedInKilobytes,
                                            colDataFileSpaceUsedInKilobytes,
                                            colLogFileSpaceAllocatedInKilobytes,
                                            colLogFileSpaceUsedInKilobytes,
                                            colTotalLocks,
                                            colBufferCacheSizeInKilobytes
                                     }
                                    );
            }

            return dt;
        }

        #endregion

        #region OSStats helpers

        private static void writeOSStatistics(
                SqlConnection connection,
                SqlTransaction transaction,
                int serverId,
                List<SQLdm4xData.OSMetrics> osMetrics
            )
        {
            // Create bulk copy object.
            using (SqlBulkCopy bcp = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
            {
                // Check bcp.
                if (bcp == null)
                {
                    Log.Error("bcp object is null, no further processing will be done.");
                    return;
                }

                // Set the destination table and time out.
                bcp.DestinationTableName = OSStatisticsTableName;
                bcp.BulkCopyTimeout = global::Idera.SQLdm.ImportWizard.Properties.Settings.Default.TimeOut;

                // Create the datatable object.
                using (DataTable dt = createOSMetricsDataTable())
                {
                    // Check the datatable object.
                    if (dt == null)
                    {
                        Log.Error("datatable object is null, no further processing will be done.");
                        return;
                    }

                    // Fill the os metrics.
                    foreach (SQLdm4xData.OSMetrics osm in osMetrics)
                    {
                        // Create new data row.
                        DataRow dr = dt.NewRow();

                        // Fill the data row with data.
                        dr[ParamOSStatsSQLServerID] = new SqlInt32(serverId);
                        dr[ParamOSStatsUTCCollectionDateTime] = osm.UTCCollectionDateTime;
                        dr[ParamOSStatsOSTotalPhysicalMemoryInKilobytes] = osm.OSTotalPhysicalMemoryInKilobytes;
                        dr[ParamOSStatsOSAvailableMemoryInKilobytes] = osm.OSAvailableMemoryInKilobytes;
                        dr[ParamOSStatsPagesPerSecond] = osm.PagesPerSecond;
                        dr[ParamOSStatsProcessorTimePercent] = osm.ProcessorTimePercent;
                        dr[ParamOSStatsPrivilegedTimePercent] = osm.PrivilegedTimePercent;
                        dr[ParamOSStatsUserTimePercent] = osm.UserTimePercent;
                        dr[ParamOSStatsProcessorQueueLength] = osm.ProcessorQueueLength;
                        dr[ParamOSStatsDiskTimePercent] = osm.DiskTimePercent;
                        dr[ParamOSStatsDiskQueueLength] = osm.DiskQueueLength;

                        // Add row to the data table.
                        dt.Rows.Add(dr);

                        // Write to repository in chunks.
                        if (dt.Rows.Count > global::Idera.SQLdm.ImportWizard.Properties.Settings.Default.NumRowsInBatch)
                        {
                            try
                            {
                                bcp.WriteToServer(dt);
                                dt.Clear();
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Exception encountered when writing OS Statistics to the Repository - ", ex);
                                throw ex;
                            }
                        }
                    }

                    // Write to repository, any remaining rows.
                    if (dt.Rows.Count > 0)
                    {
                        try
                        {
                            bcp.WriteToServer(dt);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Exception encountered when writing OS Statistics to the Repository - ", ex);
                            throw ex;
                        }
                     }
                }
            }
        }

        private static DataTable createOSMetricsDataTable()
        {
            DataTable dt = null;

            using (DataColumn
                        colSQLServerID = new DataColumn(ParamOSStatsSQLServerID, typeof(SqlInt32)),
                        colUTCCollectionDateTime = new DataColumn(ParamOSStatsUTCCollectionDateTime, typeof(SqlDateTime)),
                        colOSTotalPhysicalMemoryInKilobytes = new DataColumn(ParamOSStatsOSTotalPhysicalMemoryInKilobytes, typeof(SqlInt64)),
                        colOSAvailableMemoryInKilobytes = new DataColumn(ParamOSStatsOSAvailableMemoryInKilobytes, typeof(SqlInt64)),
                        colPagesPerSecond = new DataColumn(ParamOSStatsPagesPerSecond, typeof(SqlDouble)),
                        colProcessorTimePercent = new DataColumn(ParamOSStatsProcessorTimePercent, typeof(SqlDouble)),
                        colPrivilegedTimePercent = new DataColumn(ParamOSStatsPrivilegedTimePercent, typeof(SqlDouble)),
                        colUserTimePercent = new DataColumn(ParamOSStatsUserTimePercent, typeof(SqlDouble)),
                        colProcessorQueueLength = new DataColumn(ParamOSStatsProcessorQueueLength, typeof(SqlDouble)),
                        colDiskTimePercent = new DataColumn(ParamOSStatsDiskTimePercent, typeof(SqlDouble)),
                        colDiskQueueLength = new DataColumn(ParamOSStatsDiskQueueLength, typeof(SqlDouble))
                  )
            {
                dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[] {
                                            colSQLServerID,
                                            colUTCCollectionDateTime,
	                                        colOSTotalPhysicalMemoryInKilobytes,
	                                        colOSAvailableMemoryInKilobytes,
	                                        colPagesPerSecond,
	                                        colProcessorTimePercent,
	                                        colPrivilegedTimePercent,
	                                        colUserTimePercent,
	                                        colProcessorQueueLength,
	                                        colDiskTimePercent,
                                            colDiskQueueLength
                                     }
                                    );
            }

            return dt;
        }

        #endregion

        #region WorstPerforming helpers

        private static void writeWorstPeformingData(
                SqlConnection connection,
                SqlTransaction transaction,
                int serverId,
                List<SQLdm4xData.WorstPerforming> worstPerformingData
            )
        {
            // Create bulk copy object.
            using (SqlBulkCopy bcp = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
            {
                // Check bcp.
                if (bcp == null)
                {
                    Log.Error("bcp object is null, no further processing will be done.");
                    return;
                }

                // Set the destination table & time out.
                bcp.DestinationTableName = QueryMonitorTableName;
                bcp.BulkCopyTimeout = global::Idera.SQLdm.ImportWizard.Properties.Settings.Default.TimeOut;
                
                // Create the datatable object.
                using (DataTable dt = createQueryMonitorDataTable())
                {
                    // Check the datatable object.
                    if (dt == null)
                    {
                        Log.Error("datatable object is null, no further processing will be done.");
                        return;
                    }

                    // Fill the worst performing data.
                    foreach (SQLdm4xData.WorstPerforming wpd in worstPerformingData)
                    {
                        // Create new data row.
                        DataRow dr = dt.NewRow();

                        // Fill the data row with data.
                        dr[ParamQueryMonitorSQLServerID] = new SqlInt32(serverId);
                        dr[ParamQueryMonitorUTCCollectionDateTime] = wpd.UTCCollectionDateTime;
                        dr[ParamQueryMonitorDatabaseID] = new SqlInt32(getDatabaseId(transaction, serverId, wpd.DatabaseUsed, SQLdm4xData.Database.IsSystemDbTest(wpd.DatabaseUsed)));
                        dr[ParamQueryMonitorCompletionTime] = wpd.UTCCompletionTime;
                        dr[ParamQueryMonitorDurationMilliseconds] = wpd.DurationMilliseconds;
                        dr[ParamQueryMonitorCPUMilliseconds] = wpd.CPUMilliseconds;
                        dr[ParamQueryMonitorReads] = wpd.Reads;
                        dr[ParamQueryMonitorWrites] = wpd.Writes;
                        dr[ParamQueryMonitorNtUserName] = wpd.NtUserName;
                        dr[ParamQueryMonitorSqlUserName] = wpd.SqlUserName;
                        dr[ParamQueryMonitorApplicationName] = wpd.ApplicationName;
                        dr[ParamQueryMonitorClientComputerName] = wpd.ClientComputerName;
                        dr[ParamQueryMonitorStatementType] = wpd.StatementType;
                        dr[ParamQueryMonitorStatementText] = wpd.StatementText;

                        // Add row to the data table.
                        dt.Rows.Add(dr);

                        // Write to repository in chunks.
                        if (dt.Rows.Count > global::Idera.SQLdm.ImportWizard.Properties.Settings.Default.NumRowsInBatch)
                        {
                            try
                            {
                                bcp.WriteToServer(dt);
                                dt.Clear();
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Exception encountered when writing Worst Performing to the Repository - ", ex);
                                throw ex;
                            }
                        }
                    }

                    // Write to repository, any remaining rows.
                    if (dt.Rows.Count > 0)
                    {
                        try
                        {
                            bcp.WriteToServer(dt);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Exception encountered when writing Worst Performing to the Repository - ", ex);
                            throw ex;
                        }
                    }
                }
            }
        }

        private static DataTable createQueryMonitorDataTable()
        {
            DataTable dt = null;

            using (DataColumn
                        colSQLServerID = new DataColumn(ParamQueryMonitorSQLServerID, typeof(SqlInt32)),
                        colUTCCollectionDateTime = new DataColumn(ParamQueryMonitorUTCCollectionDateTime, typeof(SqlDateTime)),
                        colDatabaseID = new DataColumn(ParamQueryMonitorDatabaseID, typeof(SqlInt32)),
                        colCompletionTime = new DataColumn(ParamQueryMonitorCompletionTime, typeof(SqlDateTime)),
                        colDurationMilliseconds = new DataColumn(ParamQueryMonitorDurationMilliseconds, typeof(SqlInt64)),
                        colCPUMilliseconds = new DataColumn(ParamQueryMonitorCPUMilliseconds, typeof(SqlInt64)),
                        colReads = new DataColumn(ParamQueryMonitorReads, typeof(SqlInt64)),
                        colWrites = new DataColumn(ParamQueryMonitorWrites, typeof(SqlInt64)),
                        colNtUserName = new DataColumn(ParamQueryMonitorNtUserName, typeof(SqlString)),
                        colSqlUserName = new DataColumn(ParamQueryMonitorSqlUserName, typeof(SqlString)),
                        colClientComputerName = new DataColumn(ParamQueryMonitorClientComputerName, typeof(SqlString)),
                        colApplicationName = new DataColumn(ParamQueryMonitorApplicationName, typeof(SqlString)),
                        colStatementType = new DataColumn(ParamQueryMonitorStatementType, typeof(SqlInt32)),
                        colStatementText = new DataColumn(ParamQueryMonitorStatementText, typeof(SqlString))
                  )
            {
                dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[] {
	                                        colSQLServerID,
	                                        colUTCCollectionDateTime,
	                                        colDatabaseID,
	                                        colCompletionTime,
	                                        colDurationMilliseconds,
	                                        colCPUMilliseconds,
	                                        colReads,
	                                        colWrites,
	                                        colNtUserName,
	                                        colSqlUserName,
	                                        colClientComputerName,
	                                        colApplicationName,
	                                        colStatementType,
                                            colStatementText
                                     }
                                    );
            }

            return dt;
        }

        #endregion

        #region DatabaseStats helpers

        private static void writeDatabaseStatistics(
                SqlConnection connection,
                SqlTransaction transaction,
                int serverId,
                SQLdm4xData.Database db
            )
        {
            // Get the database id.
            int dbId = getDatabaseId(transaction, serverId, db.Db, db.IsSystemDb);

            // Get the statistics values.
            List<SQLdm4xData.DbStatistics> dbStats = db.DbStatisticsData;
            SQLdm4xData.DbSpaceStatistics dbSpaceStats = db.DbSpaceStatisticsData;

            // Create bulk copy object.
            using (SqlBulkCopy bcp = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
            {
                // Check bcp.
                if (bcp == null)
                {
                    Log.Error("bcp object is null, no further processing will be done for the db - ", db.Db);
                    return;
                }

                // Set the destination table and time out.
                bcp.DestinationTableName = DatabaseStatisticsTableName;
                bcp.BulkCopyTimeout = global::Idera.SQLdm.ImportWizard.Properties.Settings.Default.TimeOut;

                // Create the datatable object.
                using (DataTable dt = createDatabaseStatsDataTable())
                {
                    // Check the datatable object.
                    if (dt == null)
                    {
                        Log.Error("datatable object is null, no further processing will be done for the db - " + db.Db);
                        return;
                    }

                    // Fill the db stats.
                    foreach (SQLdm4xData.DbStatistics dbs in dbStats)
                    {
                        // Create new data row.
                        DataRow dr = dt.NewRow();

                        // Fill the data row with data.
                        dr[ParamDbStatsDatabaseID] = new SqlInt32(dbId);
                        dr[ParamDbStatsUTCCollectionDateTime] = dbs.UTCCollectionDateTime;
                        dr[ParamDbStatsTransactions] = dbs.Transactions;
                        dr[ParamDbStatsLogFlushWaits] = dbs.LogFlushWaits;
                        dr[ParamDbStatsLogFlushes] = dbs.LogFlushes;
                        dr[ParamDbStatsLogKilobytesFlushed] = dbs.LogKilobytesFlushed;
                        dr[ParamDbStatsLogCacheHitRatio] = dbs.LogCacheHitRatio;
                        dr[ParamDbStatsTimeDeltaInSeconds] = dbs.TimeDeltaInSeconds;
                        if (dbSpaceStats != null)
                        {
                            dr[ParamDbStatsDataFileSizeInKilobytes] = dbSpaceStats.DataFileSizeInKilobytes;
                            dr[ParamDbStatsLogFileSizeInKilobytes] = dbSpaceStats.LogFileSizeInKilobytes;
                            dr[ParamDbStatsDataSizeInKilobytes] = dbSpaceStats.DataSizeInKilobytes;
                            dr[ParamDbStatsLogSizeInKilobytes] = dbSpaceStats.LogSizeInKilobytes;
                            dr[ParamDbStatsTextSizeInKilobytes] = dbSpaceStats.TextSizeInKilobytes;
                            dr[ParamDbStatsIndexSizeInKilobytes] = dbSpaceStats.IndexSizeInKilobytes;
                        }

                        // Add row to the data table.
                        dt.Rows.Add(dr);

                        // Write to repository in chunks.
                        if (dt.Rows.Count > global::Idera.SQLdm.ImportWizard.Properties.Settings.Default.NumRowsInBatch)
                        {
                            try
                            {
                                bcp.WriteToServer(dt);
                                dt.Clear();
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Exception encountered when writing DB Statistics to the Repository for the db -  " + db.Db + " - ", ex);
                                throw ex;
                            }
                        }
                    }

                    // Write to repository, any remaining rows.
                    if (dt.Rows.Count > 0)
                    {
                        try
                        {
                            bcp.WriteToServer(dt);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Exception encountered when writing DB Statistics to the Repository for the db -  " + db.Db + " - ", ex);
                            throw ex;
                        }
                    }
                }
            }
        }

        private static DataTable createDatabaseStatsDataTable()
        {
            DataTable dt = null;

            using (DataColumn
                        colDatabaseID = new DataColumn(ParamDbStatsDatabaseID, typeof(SqlInt32)),
                        colUTCCollectionDateTime = new DataColumn(ParamDbStatsUTCCollectionDateTime, typeof(SqlDateTime)),
                        colDatabaseStatus = new DataColumn(ParamDbStatsDatabaseStatus, typeof(SqlInt32)),
                        colTransactions = new DataColumn(ParamDbStatsTransactions, typeof(SqlInt64)),
                        colLogFlushWaits = new DataColumn(ParamDbStatsLogFlushWaits, typeof(SqlInt64)),
                        colLogFlushes = new DataColumn(ParamDbStatsLogFlushes, typeof(SqlInt64)),
                        colLogKilobytesFlushed = new DataColumn(ParamDbStatsLogKilobytesFlushed, typeof(SqlInt64)),
                        colLogCacheReads = new DataColumn(ParamDbStatsLogCacheReads, typeof(SqlInt64)),
                        colLogCacheHitRatio = new DataColumn(ParamDbStatsLogCacheHitRatio, typeof(SqlDouble)),
                        colDataFileSizeInKilobytes = new DataColumn(ParamDbStatsDataFileSizeInKilobytes, typeof(SqlDecimal)),
                        colLogFileSizeInKilobytes = new DataColumn(ParamDbStatsLogFileSizeInKilobytes, typeof(SqlDecimal)),
                        colDataSizeInKilobytes = new DataColumn(ParamDbStatsDataSizeInKilobytes, typeof(SqlDecimal)),
                        colLogSizeInKilobytes = new DataColumn(ParamDbStatsLogSizeInKilobytes, typeof(SqlDecimal)),
                        colTextSizeInKilobytes = new DataColumn(ParamDbStatsTextSizeInKilobytes, typeof(SqlDecimal)),
                        colIndexSizeInKilobytes = new DataColumn(ParamDbStatsIndexSizeInKilobytes, typeof(SqlDecimal)),
                        colLogExpansionInKilobytes = new DataColumn(ParamDbStatsLogExpansionInKilobytes, typeof(SqlDecimal)),
                        colDataExpansionInKilobytes = new DataColumn(ParamDbStatsDataExpansionInKilobytes, typeof(SqlDecimal)),
                        colPercentLogSpace = new DataColumn(ParamDbStatsPercentLogSpace, typeof(SqlDouble)),
                        colPercentDataSize = new DataColumn(ParamDbStatsPercentDataSize, typeof(SqlDouble)),
                        colTimeDeltaInSeconds = new DataColumn(ParamDbStatsTimeDeltaInSeconds, typeof(SqlDouble))
                  )
            {
                dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[] {
                                            colDatabaseID,
	                                        colUTCCollectionDateTime,
	                                        colDatabaseStatus,
	                                        colTransactions,
	                                        colLogFlushWaits,
	                                        colLogFlushes,
	                                        colLogKilobytesFlushed,
	                                        colLogCacheReads,
	                                        colLogCacheHitRatio,
	                                        colDataFileSizeInKilobytes,
	                                        colLogFileSizeInKilobytes,
	                                        colDataSizeInKilobytes,
	                                        colLogSizeInKilobytes,
	                                        colTextSizeInKilobytes,
	                                        colIndexSizeInKilobytes,
	                                        colLogExpansionInKilobytes,
	                                        colDataExpansionInKilobytes,
	                                        colPercentLogSpace,
	                                        colPercentDataSize,
                                            colTimeDeltaInSeconds
                                     }
                                    );
            }

            return dt;
        }

        private static void writeTableStatistics(
                SqlConnection connection,
                SqlTransaction transaction,
                int serverId,
                SQLdm4xData.Database db            
            )
        {
            // Create bulk copy object.
            using (SqlBulkCopy bcpg = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction),
                               bcpr = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
            {
                // Check bcp.
                if (bcpg == null || bcpr == null)
                {
                    Log.Error("bcp object is null, no further processing will be done for the tables of db - ", db.Db);
                    return;
                }

                // Set the destination table and time out.
                bcpg.DestinationTableName = TableGrowthTableName;
                bcpg.BulkCopyTimeout = global::Idera.SQLdm.ImportWizard.Properties.Settings.Default.TimeOut;
                bcpr.DestinationTableName = TableReorganizationTableName;
                bcpr.BulkCopyTimeout = global::Idera.SQLdm.ImportWizard.Properties.Settings.Default.TimeOut;

                // Create the datatable object.
                using (DataTable dtg = createTableGrowthDataTable(),
                                 dtr = createTableReorgDataTable())
                {
                    // Check the datatable object.
                    if (dtg == null || dtr == null)
                    {
                        Log.Error("datatable object is null, no further processing will be done for the tables of db - " + db.Db);
                        return;
                    }

                    foreach (SQLdm4xData.Table tbl in db.TableData)
                    {
                        // Create new data row.
                        DataRow drg = dtg.NewRow();
                        DataRow drr = dtr.NewRow();

                        // Get table id.
                        int tblId = getTableId(transaction, serverId, db.Db, db.IsSystemDb, tbl.TableName, tbl.SchemaName);

                        // Fill the data row with data.
                        drg[ParamTableGrowthTableID] = new SqlInt32(tblId);
                        drg[ParamTableGrowthUTCCollectionDateTime] = tbl.UTCCollectionDateTime;
                        drg[ParamTableGrowthNumberOfRows] = tbl.NumberOfRows;
                        drg[ParamTableGrowthDataSize] = tbl.DataSize;
                        drg[ParamTableGrowthTextSize] = tbl.TextSize;
                        drg[ParamTableGrowthIndexSize] = tbl.IndexSize;

                        drr[ParamTableReorgTableID] = new SqlInt32(tblId);
                        drr[ParamTableReorgUTCCollectionDateTime] = tbl.UTCCollectionDateTime;
                        drr[ParamTableReorgScanDensity] = tbl.ScanDensity;

                        // Add row to the data table.
                        dtg.Rows.Add(drg);
                        dtr.Rows.Add(drr);

                        // Write to repository in chunks.
                        if (dtg.Rows.Count > global::Idera.SQLdm.ImportWizard.Properties.Settings.Default.NumRowsInBatch)
                        {
                            try
                            {
                                bcpg.WriteToServer(dtg);
                                dtg.Clear();
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Exception encountered when writing Table Growth to the Repository for the db -  " + db.Db + " - ", ex);
                                throw ex;
                            }
                        }
                        if (dtr.Rows.Count > global::Idera.SQLdm.ImportWizard.Properties.Settings.Default.NumRowsInBatch)
                        {
                            try
                            {
                                bcpr.WriteToServer(dtr);
                                dtr.Clear();
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Exception encountered when writing Table Reorganization to the Repository for the db -  " + db.Db + " - ", ex);
                                throw ex;
                            }
                        }
                    }

                    // Write to repository, any remaining rows.
                    if (dtg.Rows.Count > 0)
                    {
                        try
                        {
                            bcpg.WriteToServer(dtg);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Exception encountered when writing Table Growth to the Repository for the db -  " + db.Db + " - ", ex);
                            throw ex;
                        }
                    }
                    if (dtr.Rows.Count > 0)
                    {
                        try
                        {
                            bcpr.WriteToServer(dtr);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Exception encountered when writing Table Reorganization to the Repository for the db -  " + db.Db + " - ", ex);
                            throw ex;
                        }
                    }
                }
            }
        }

        private static DataTable createTableGrowthDataTable()
        {
            DataTable dt = null;

            using (DataColumn
 	                    colTableGrowthTableID = new DataColumn(ParamTableGrowthTableID, typeof(SqlInt32)),
	                    colTableGrowthUTCCollectionDateTime = new DataColumn(ParamTableGrowthUTCCollectionDateTime, typeof(SqlDateTime)),
	                    colTableGrowthNumberOfRows = new DataColumn(ParamTableGrowthNumberOfRows, typeof(SqlInt64)),
	                    colTableGrowthDataSize = new DataColumn(ParamTableGrowthDataSize, typeof(SqlDouble)),
	                    colTableGrowthTextSize = new DataColumn(ParamTableGrowthTextSize, typeof(SqlDouble)),
	                    colTableGrowthIndexSize = new DataColumn(ParamTableGrowthIndexSize, typeof(SqlDouble)),
                        colTableGrowthTimeDeltaInSeconds = new DataColumn(ParamTableGrowthTimeDeltaInSeconds, typeof(SqlDouble))
                 )
            {
                dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[] {
 	                                    colTableGrowthTableID,
	                                    colTableGrowthUTCCollectionDateTime,
	                                    colTableGrowthNumberOfRows,
	                                    colTableGrowthDataSize,
	                                    colTableGrowthTextSize,
	                                    colTableGrowthIndexSize,
                                        colTableGrowthTimeDeltaInSeconds
                                     }
                                    );
            }

            return dt;
        }

        private static DataTable createTableReorgDataTable()
        {
            DataTable dt = null;

            using (DataColumn
	                    colTableReorgTableID = new DataColumn(ParamTableReorgTableID, typeof(SqlInt32)),
                        colTableReorgUTCCollectionDateTime = new DataColumn(ParamTableReorgUTCCollectionDateTime, typeof(SqlDateTime)),
                        colTableReorgScanDensity = new DataColumn(ParamTableReorgScanDensity, typeof(SqlDouble)),
                        colTableReorgLogicalFragmentation = new DataColumn(ParamTableReorgLogicalFragmentation, typeof(SqlDouble)),
                        colTableReorgTimeDeltaInSeconds = new DataColumn(ParamTableReorgTimeDeltaInSeconds, typeof(SqlDouble))
                 )
            {
                dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[] {
	                                        colTableReorgTableID,
                                            colTableReorgUTCCollectionDateTime,
                                            colTableReorgScanDensity,
                                            colTableReorgLogicalFragmentation,
                                            colTableReorgTimeDeltaInSeconds
                                     }
                                    );
            }

            return dt;
        }

        #endregion

        #region Misc helpers

        private static int getDatabaseId(
                SqlTransaction transaction,
                int serverId,
                string dbName,
                bool isSystemDb
            )
        {
            // Create and fill the query params.
            SqlParameter[] arParms = new SqlParameter[5];
            arParms[0] = new SqlParameter("@SQLServerID", SqlDbType.Int);
            arParms[0].Direction = ParameterDirection.Input;
            arParms[0].Value = serverId;

            arParms[1] = new SqlParameter("@DatabaseName", SqlDbType.NVarChar);
            arParms[1].Direction = ParameterDirection.Input;
            arParms[1].Value = dbName;

            arParms[2] = new SqlParameter("@SystemDatabase", SqlDbType.Bit);
            arParms[2].Direction = ParameterDirection.Input;
            arParms[2].Value = isSystemDb;

            arParms[3] = new SqlParameter("@DatabaseID", SqlDbType.Int);
            arParms[3].Direction = ParameterDirection.Output;

            arParms[4] = new SqlParameter("@ReturnMessage", SqlDbType.NVarChar);
            arParms[4].Direction = ParameterDirection.Output;
            arParms[4].Value = "";

            // Query for database id.
            try
            {
                SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure,
                                InsertDatabaseNameStoredProcedure, arParms);
            }
            catch (Exception ex)
            {
                Log.Error("Exception encountered when querying for database id for db - " + dbName + " - ", ex);
                throw ex;
            }

            // Get the db id.
            int dbId = 0;
            if (arParms[3].Value is int)
            {
                dbId = (int)arParms[3].Value;
            }
            else
            {
                Log.Error("@DatabaseID is not an int for database id for db - " + dbName);
                throw new Exception("DatabaseID is not an int");
            }

            return dbId;
        }

        private static int getTableId(
                SqlTransaction transaction,
                int serverId,
                string dbName,
                bool isSystemDb,
                string tableName,
                string schemaName
            )
        {
            // Create and fill the query params.
            SqlParameter[] arParms = new SqlParameter[9];
            arParms[0] = new SqlParameter("@SQLServerID", SqlDbType.Int);
            arParms[0].Direction = ParameterDirection.Input;
            arParms[0].Value = serverId;

            arParms[1] = new SqlParameter("@DatabaseName", SqlDbType.NVarChar);
            arParms[1].Direction = ParameterDirection.Input;
            arParms[1].Value = dbName;

            arParms[2] = new SqlParameter("@SystemDatabase", SqlDbType.Bit);
            arParms[2].Direction = ParameterDirection.Input;
            arParms[2].Value = isSystemDb;

            arParms[3] = new SqlParameter("@TableName", SqlDbType.NVarChar);
            arParms[3].Direction = ParameterDirection.Input;
            arParms[3].Value = tableName;

            arParms[4] = new SqlParameter("@SchemaName", SqlDbType.NVarChar);
            arParms[4].Direction = ParameterDirection.Input;
            arParms[4].Value = schemaName;

            arParms[5] = new SqlParameter("@SystemTable", SqlDbType.Bit);
            arParms[5].Direction = ParameterDirection.Input;
            arParms[5].Value = false;

            arParms[6] = new SqlParameter("@DatabaseID", SqlDbType.Int);
            arParms[6].Direction = ParameterDirection.Output;

            arParms[7] = new SqlParameter("@TableID", SqlDbType.Int);
            arParms[7].Direction = ParameterDirection.Output;

            arParms[8] = new SqlParameter("@ReturnMessage", SqlDbType.NVarChar);
            arParms[8].Direction = ParameterDirection.Output;
            arParms[8].Value = "";

            // Query for the table id.
            try
            {
                SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure,
                                InsertTableNameStoredProcedure, arParms);
            }
            catch (Exception ex)
            {
                Log.Error("Exception encountered when querying for table id for table - " + dbName + " - [" + schemaName + "].[" + tableName + "] - ", ex);
                throw ex;
            }
            
            // Get the table id.
            int tblId = 0;
            if (arParms[7].Value is int)
            {
                tblId = (int)arParms[7].Value;
            }
            else
            {
                Log.Error("@TableID is not an int for table - " + dbName + " - [" + schemaName + "].[" + tableName + "]");
                throw new Exception("Table ID is not an int");
            }

            return tblId;
        }

        private static void updateImportDate(
                SqlTransaction transaction,
                int serverId,
                DateTime importDate
            )
        {
            // Create and fill the query params.
            SqlParameter[] arParms = new SqlParameter[2];
            arParms[0] = new SqlParameter("@SQLServerID", SqlDbType.Int);
            arParms[0].Direction = ParameterDirection.Input;
            arParms[0].Value = serverId;

            arParms[1] = new SqlParameter("@EarliestDateImportedFromLegacySQLdm", SqlDbType.DateTime);
            arParms[1].Direction = ParameterDirection.Input;
            arParms[1].Value = importDate;

            // Update import date.
            try
            {
                SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure,
                                ImportUpdateEarliestDateImportedStoredProcedure, arParms);
            }
            catch (Exception ex)
            {
                Log.Error("Exception raised when updating the server import date in the Repository - ", ex);
                throw ex;
            }
        }

        #endregion

        #endregion
    }
}
