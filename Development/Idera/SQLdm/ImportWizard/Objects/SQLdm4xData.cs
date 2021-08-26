using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;

namespace Idera.SQLdm.ImportWizard.Objects
{
    public class SQLdm4xData
    {
        #region types

        public class ServerStatistics
        {
            #region members

            private DateTime m_UTCCollectionDateTime;
            private float m_TimeDeltaInSeconds;
            private float m_CPUActivityPercentage;
            private float m_IOActivityPercentage;
            private long m_PageReads; //*
            private long m_PageWrites; //*
            private long m_PageErrors; //*
            private long m_LazyWriterWrites; //*
            private long m_CheckpointWrites; //*
            private long m_LogFlushes; //*
            private long m_PacketsReceived; //*
            private long m_PacketsSent; //*
            private long m_PacketErrors; //*
            private long m_Logins; //*
            private int m_UserProcesses;
            private int m_ClientComputers;
            private long m_Transactions; //*
            private long m_OldestOpenTransactionsInMinutes;
            private long m_ReplicationUnsubscribed; //*
            private long m_ReplicationSubscribed; //*
            private long m_ReplicationUndistributed; //*
            private float m_ReplicationLatencyInSeconds;
            private float m_TempDBSizePercent;
            private long m_TempDBSizeInKilobytes;
            private float m_ProcedureCacheSizePercent;
            private long m_ProcedureCacheSizeInKilobytes;
            private float m_BufferCacheHitRatioPercentage;
            private float m_ProcedureCacheHitRatioPercentage;
            private int m_ResponseTimeInMilliseconds;
            private long m_SqlCompilations; //*
            private long m_SqlRecompilations; //*
            private long m_WorkFilesCreated; //*
            private long m_WorkTablesCreated; //*
            private long m_SqlMemoryAllocatedInKilobytes;
            private long m_SqlMemoryUsedInKilobytes;
            private long m_TableLockEscalations; //*
            private long m_ReadAheadPages; //*
            private long m_PageSplits; //*
            private long m_FullScans; //*
            private long m_PageLifeExpectancy;
            private long m_LockWaits; //*
            private long m_PageLookups; //*

            #endregion

            #region ctor

            public ServerStatistics(
                    DateTime utcCollectionTime,
                    float timeDeltaInSeconds,
                    float cpuActivityPercentage,
                    float ioActivityPercentage,
                    long pageReads,
                    long pageWrites,
                    long pageErrors,
                    long lazyWriterWrites,
                    long checkpointWrites,
                    long logFlushes,
                    long packetsReceived,
                    long packetsSent,
                    long packetErrors,
                    long logins,
                    int userProcesses,
                    int clientComputers,
                    long transactions,
                    long oldestOpenTransactionsInMinutes,
                    long replicationUnsubscribed,
                    long replicationSubscribed, //*
                    long replicationUndistributed, //*
                    float replicationLatencyInSeconds,
                    float tempDBSizePercent,
                    long tempDBSizeInKilobytes,
                    float procedureCacheSizePercent,
                    long procedureCacheSizeInKilobytes,
                    float bufferCacheHitRatioPercentage,
                    float procedureCacheHitRatioPercentage,
                    int responseTimeInMilliseconds,
                    long sqlCompilations, //*
                    long sqlRecompilations, //*
                    long workFilesCreated, //*
                    long workTablesCreated, //*
                    long sqlMemoryAllocatedInKilobytes,
                    long sqlMemoryUsedInKilobytes,
                    long tableLockEscalations, //*
                    long readAheadPages, //*
                    long pageSplits, //*
                    long fullScans, //*
                    long pageLifeExpectancy,
                    long lockWaits, //*
                    long pageLookups //*
                )
            {
                m_UTCCollectionDateTime = utcCollectionTime;
                m_TimeDeltaInSeconds = timeDeltaInSeconds;
                m_CPUActivityPercentage = cpuActivityPercentage;
                m_IOActivityPercentage = ioActivityPercentage;
                m_PageReads = pageReads;
                m_PageWrites = pageWrites;
                m_PageErrors = pageErrors;
                m_LazyWriterWrites = lazyWriterWrites;
                m_CheckpointWrites = checkpointWrites;
                m_LogFlushes = logFlushes;
                m_PacketsReceived = packetsReceived;
                m_PacketsSent = packetsSent;
                m_PacketErrors = packetErrors;
                m_Logins = logins;
                m_UserProcesses = userProcesses;
                m_ClientComputers = clientComputers;
                m_Transactions = transactions;
                m_OldestOpenTransactionsInMinutes = oldestOpenTransactionsInMinutes;
                m_ReplicationUnsubscribed = replicationUnsubscribed;
                m_ReplicationSubscribed = replicationSubscribed;
                m_ReplicationUndistributed = replicationUndistributed;
                m_ReplicationLatencyInSeconds = replicationLatencyInSeconds;
                m_TempDBSizePercent = tempDBSizePercent;
                m_TempDBSizeInKilobytes = tempDBSizeInKilobytes;
                m_ProcedureCacheSizePercent = procedureCacheSizePercent;
                m_ProcedureCacheSizeInKilobytes = procedureCacheSizeInKilobytes;
                m_BufferCacheHitRatioPercentage = bufferCacheHitRatioPercentage;
                m_ProcedureCacheHitRatioPercentage = procedureCacheHitRatioPercentage;
                m_ResponseTimeInMilliseconds = responseTimeInMilliseconds;
                m_SqlCompilations = sqlCompilations;
                m_SqlRecompilations = sqlRecompilations;
                m_WorkFilesCreated = workFilesCreated;
                m_WorkTablesCreated = workTablesCreated;
                m_SqlMemoryAllocatedInKilobytes = sqlMemoryAllocatedInKilobytes;
                m_SqlMemoryUsedInKilobytes = sqlMemoryUsedInKilobytes;
                m_TableLockEscalations = tableLockEscalations;
                m_ReadAheadPages = readAheadPages;
                m_PageSplits = pageSplits;
                m_FullScans = fullScans;
                m_PageLifeExpectancy = pageLifeExpectancy;
                m_LockWaits = lockWaits;
                m_PageLookups = pageLookups;
            }

            #endregion

            #region properties

            public SqlDateTime UTCCollectionDateTime
            {
                get { return new SqlDateTime(m_UTCCollectionDateTime); }
            }

            public SqlDouble TimeDeltaInSeconds
            {
                get { return new SqlDouble(m_TimeDeltaInSeconds); }
            }

            public SqlDouble CPUActivityPercentage
            {
                get { return new SqlDouble(m_CPUActivityPercentage); }
            }

            public SqlDouble IOActivityPercentage
            {
                get { return new SqlDouble(m_IOActivityPercentage); }
            }

            public SqlInt64 PageReads
            {
                get { return new SqlInt64(m_PageReads); }
            }

            public SqlInt64 PageWrites
            {
                get { return new SqlInt64(m_PageWrites); }
            }

            public SqlInt64 PageErrors
            {
                get { return new SqlInt64(m_PageErrors); }
            }

            public SqlInt64 LazyWriterWrites
            {
                get { return new SqlInt64(m_LazyWriterWrites); }
            }

            public SqlInt64 CheckpointWrites
            {
                get { return new SqlInt64(m_CheckpointWrites); }
            }

            public SqlInt64 LogFlushes
            {
                get { return new SqlInt64(m_LogFlushes); }
            }

            public SqlInt64 PacketsReceived
            {
                get { return new SqlInt64(m_PacketsReceived); }
            }

            public SqlInt64 PacketsSent
            {
                get { return new SqlInt64(m_PacketsSent); }
            }

            public SqlInt64 PacketErrors
            {
                get { return new SqlInt64(m_PacketErrors); }
            }

            public SqlInt64 Logins
            {
                get { return new SqlInt64(m_Logins); }
            }

            public SqlInt32 UserProcesses
            {
                get { return new SqlInt32(m_UserProcesses); }
            }

            public SqlInt32 ClientComputers
            {
                get { return new SqlInt32(m_ClientComputers); }
            }

            public SqlInt64 Transactions
            {
                get { return new SqlInt64(m_Transactions); }
            }

            public SqlInt64 OldestOpenTransactionsInMinutes
            {
                get { return new SqlInt64(m_OldestOpenTransactionsInMinutes); }
            }

            public SqlInt64 ReplicationUnsubscribed
            {
                get { return new SqlInt64(m_ReplicationUnsubscribed); }
            }

            public SqlInt64 ReplicationSubscribed
            {
                get { return new SqlInt64(m_ReplicationSubscribed); }
            }

            public SqlInt64 ReplicationUndistributed
            {
                get { return new SqlInt64(m_ReplicationUndistributed); }
            }

            public SqlDouble ReplicationLatencyInSeconds
            {
                get { return new SqlDouble(m_ReplicationLatencyInSeconds); }
            }

            public SqlDouble TempDBSizePercent
            {
                get { return new SqlDouble(m_TempDBSizePercent); }
            }

            public SqlInt64 TempDBSizeInKilobytes
            {
                get { return new SqlInt64(m_TempDBSizeInKilobytes); }
            }

            public SqlDouble ProcedureCacheSizePercent
            {
                get { return new SqlDouble(m_ProcedureCacheSizePercent); }
            }

            public SqlInt64 ProcedureCacheSizeInKilobytes
            {
                get { return new SqlInt64(m_ProcedureCacheSizeInKilobytes); }
            }

            public SqlDouble BufferCacheHitRatioPercentage
            {
                get { return new SqlDouble(m_BufferCacheHitRatioPercentage); }
            }

            public SqlDouble ProcedureCacheHitRatioPercentage
            {
                get { return new SqlDouble(m_ProcedureCacheHitRatioPercentage); }
            }

            public SqlInt32 ResponseTimeInMilliseconds
            {
                get { return new SqlInt32(m_ResponseTimeInMilliseconds); }
            }

            public SqlInt64 SqlCompilations
            {
                get { return new SqlInt64(m_SqlCompilations); }
            }

            public SqlInt64 SqlRecompilations
            {
                get { return new SqlInt64(m_SqlRecompilations); }
            }

            public SqlInt64 WorkFilesCreated
            {
                get { return new SqlInt64(m_WorkFilesCreated); }
            }

            public SqlInt64 WorkTablesCreated
            {
                get { return new SqlInt64(m_WorkTablesCreated); }
            }

            public SqlInt64 SqlMemoryAllocatedInKilobytes
            {
                get { return new SqlInt64(m_SqlMemoryAllocatedInKilobytes); }
            }

            public SqlInt64 SqlMemoryUsedInKilobytes
            {
                get { return new SqlInt64(m_SqlMemoryUsedInKilobytes); }
            }

            public SqlInt64 TableLockEscalations
            {
                get { return new SqlInt64(m_TableLockEscalations); }
            }

            public SqlInt64 ReadAheadPages
            {
                get { return new SqlInt64(m_ReadAheadPages); }
            }

            public SqlInt64 PageSplits
            {
                get { return new SqlInt64(m_PageSplits); }
            }

            public SqlInt64 FullScans
            {
                get { return new SqlInt64(m_FullScans); }
            }

            public SqlInt64 PageLifeExpectancy
            {
                get { return new SqlInt64(m_PageLifeExpectancy); }
            }

            public SqlInt64 LockWaits
            {
                get { return new SqlInt64(m_LockWaits); }
            }

            public SqlInt64 PageLookups
            {
                get { return new SqlInt64(m_PageLookups); }
            }

            #endregion
        }

        public class OSMetrics
        {
            #region members

            private DateTime m_UTCCollectionDateTime;
            private long m_OSTotalPhysicalMemoryInKilobytes;
            private long m_OSAvailableMemoryInKilobytes;
            private float m_PagesPerSecond;
            private float m_ProcessorTimePercent;
            private float m_PrivilegedTimePercent;
            private float m_UserTimePercent;
            private float m_ProcessorQueueLength;
            private float m_DiskTimePercent;
            private float m_DiskQueueLength;

            #endregion

            #region ctors

            public OSMetrics(
                    DateTime utcCollectionDateTime,
                    long osTotalPhysicalMemoryInKilobytes,
                    long osAvailableMemoryInKilobytes,
                    float pagesPerSecond,
                    float processorTimePercent,
                    float privilegedTimePercent,
                    float userTimePercent,
                    float processorQueueLength,
                    float diskTimePercent,
                    float diskQueueLength
                )
            {
                m_UTCCollectionDateTime = utcCollectionDateTime;
                m_OSTotalPhysicalMemoryInKilobytes = osTotalPhysicalMemoryInKilobytes;
                m_OSAvailableMemoryInKilobytes = osAvailableMemoryInKilobytes;
                m_PagesPerSecond = pagesPerSecond;
                m_ProcessorTimePercent = processorTimePercent;
                m_PrivilegedTimePercent = privilegedTimePercent;
                m_UserTimePercent = userTimePercent;
                m_ProcessorQueueLength = processorQueueLength;
                m_DiskTimePercent = diskTimePercent;
                m_DiskQueueLength = diskQueueLength;
            }

            #endregion

            #region properties

            public SqlDateTime UTCCollectionDateTime
            {
                get { return new SqlDateTime(m_UTCCollectionDateTime); }
            }

            public SqlInt64 OSTotalPhysicalMemoryInKilobytes
            {
                get { return new SqlInt64(m_OSTotalPhysicalMemoryInKilobytes); }
            }

            public SqlInt64 OSAvailableMemoryInKilobytes
            {
                get { return new SqlInt64(m_OSAvailableMemoryInKilobytes); }
            }

            public SqlDouble PagesPerSecond
            {
                get { return new SqlDouble(m_PagesPerSecond); }
            }

            public SqlDouble ProcessorTimePercent
            {
                get { return new SqlDouble(m_ProcessorTimePercent); }
            }

            public SqlDouble PrivilegedTimePercent
            {
                get { return new SqlDouble(m_PrivilegedTimePercent); }
            }

            public SqlDouble UserTimePercent
            {
                get { return new SqlDouble(m_UserTimePercent); }
            }

            public SqlDouble ProcessorQueueLength
            {
                get { return new SqlDouble(m_ProcessorQueueLength); }
            }

            public SqlDouble DiskTimePercent
            {
                get { return new SqlDouble(m_DiskTimePercent); }
            }

            public SqlDouble DiskQueueLength
            {
                get { return new SqlDouble(m_DiskQueueLength); }
            }

            #endregion
        }

        public class DbStatistics
        {
            #region members

            private DateTime m_UTCCollectionDateTime;
            private float m_TimeDeltaInSeconds;
            private long m_Transactions;
            private long m_LogFlushWaits;
            private long m_LogFlushes;
            private long m_LogKilobytesFlushed;
            private float m_LogCacheHitRatio;

            #endregion

            #region ctors

            public DbStatistics(
                    DateTime utcCollectionDateTime,
                    float timeDeltaInSeconds,
                    long transactions,
                    long logFlushWaits,
                    long logFlushes,
                    long logKilobytesFlushed,
                    float logCacheHitRatio
                )
            {
                m_UTCCollectionDateTime = utcCollectionDateTime;
                m_TimeDeltaInSeconds = timeDeltaInSeconds;
                m_Transactions = transactions;
                m_LogFlushWaits = logFlushWaits;
                m_LogFlushes = logFlushes;
                m_LogKilobytesFlushed = logKilobytesFlushed;
                m_LogCacheHitRatio = logCacheHitRatio;
            }

            #endregion

            #region properties

            public SqlDateTime UTCCollectionDateTime
            {
                get { return m_UTCCollectionDateTime; }
            }

            public SqlDouble TimeDeltaInSeconds
            {
                get { return m_TimeDeltaInSeconds; }
            }

            public SqlInt64 Transactions
            {
                get { return m_Transactions; }
            }

            public SqlInt64 LogFlushWaits
            {
                get { return m_LogFlushWaits; }
            }

            public SqlInt64 LogFlushes
            {
                get { return m_LogFlushes; }
            }

            public SqlInt64 LogKilobytesFlushed
            {
                get { return m_LogKilobytesFlushed; }
            }

            public SqlDouble LogCacheHitRatio
            {
                get { return m_LogCacheHitRatio; }
            }

            #endregion
        }

        public class DbSpaceStatistics
        {
            #region members

            private DateTime m_UTCCollectionDateTime;
            private decimal m_DataFileSizeInKilobytes;
            private decimal m_DataSizeInKilobytes;
            private decimal m_LogFileSizeInKilobytes;
            private decimal m_LogSizeInKilobytes;
            private decimal m_IndexSizeInKilobytes;
            private decimal m_TextSizeInKilobytes;

            #endregion

            #region ctors

            public DbSpaceStatistics(
                    DateTime utcCollectionDateTime,
                    decimal dataFileSizeInKilobytes,
                    decimal dataSizeInKilobytes,
                    decimal logFileSizeInKilobytes,
                    decimal logSizeInKilobytes,
                    decimal indexSizeInKilobytes,
                    decimal textSizeInKilobytes
                )
            {
                m_UTCCollectionDateTime = utcCollectionDateTime;
                m_DataFileSizeInKilobytes = dataFileSizeInKilobytes;
                m_DataSizeInKilobytes = dataSizeInKilobytes;
                m_LogFileSizeInKilobytes = logFileSizeInKilobytes;
                m_LogSizeInKilobytes = logSizeInKilobytes;
                m_IndexSizeInKilobytes = indexSizeInKilobytes;
                m_TextSizeInKilobytes = textSizeInKilobytes;
            }

            #endregion

            #region properties

            public SqlDecimal DataFileSizeInKilobytes
            {
                get { return new SqlDecimal(m_DataFileSizeInKilobytes); }
            }

            public SqlDecimal DataSizeInKilobytes
            {
                get { return new SqlDecimal(m_DataSizeInKilobytes); }
            }

            public SqlDecimal LogFileSizeInKilobytes
            {
                get { return new SqlDecimal(m_LogFileSizeInKilobytes); }
            }

            public SqlDecimal LogSizeInKilobytes
            {
                get { return new SqlDecimal(m_LogSizeInKilobytes); }
            }

            public SqlDecimal IndexSizeInKilobytes
            {
                get { return new SqlDecimal(m_IndexSizeInKilobytes); }
            }

            public SqlDecimal TextSizeInKilobytes
            {
                get { return new SqlDecimal(m_TextSizeInKilobytes); }
            }

            #endregion
        }

        public class Table
        {
            #region members

            private string m_TableName;
            private string m_SchemaName;
            private DateTime m_UTCCollectionDateTime;
            private float m_TimeDeltaInSeconds;
            private int m_NumberOfRows;
            private float m_DataSize;
            private float m_TextSize;
            private float m_IndexSize;
            private float m_ScanDensity;

            #endregion

            #region ctors

            public Table(
                    string table,
                    string schema,
                    DateTime utcCollectionDateTime,
                    float timeDeltaInSeconds,
                    int numberOfRows,
                    float dataSize,
                    float textSize,
                    float indexSize,
                    float scanDensity
                )
            {
                m_TableName = table;
                m_SchemaName = schema;
                m_UTCCollectionDateTime = utcCollectionDateTime;
                m_TimeDeltaInSeconds = timeDeltaInSeconds;
                m_NumberOfRows = numberOfRows;
                m_DataSize = dataSize;
                m_TextSize = textSize;
                m_IndexSize = indexSize;
                m_ScanDensity = scanDensity;
            }

            #endregion

            #region properties

            public string SchemaName
            {
                get { return m_SchemaName; }
            }

            public string TableName
            {
                get { return m_TableName; }
            }

            public SqlDateTime UTCCollectionDateTime
            {
                get { return new SqlDateTime(m_UTCCollectionDateTime); }
            }

            public SqlDouble TimeDeltaInSeconds
            {
                get { return new SqlDouble(m_TimeDeltaInSeconds); }
            }

            public SqlInt64 NumberOfRows
            {
                get { return new SqlInt64(m_NumberOfRows); }
            }

            public SqlDouble DataSize
            {
                get { return new SqlDouble(m_DataSize); }
            }

            public SqlDouble TextSize
            {
                get { return new SqlDouble(m_TextSize); }
            }

            public SqlDouble IndexSize
            {
                get { return new SqlDouble(m_IndexSize); }
            }

            public SqlDouble ScanDensity
            {
                get { return new SqlDouble(m_ScanDensity); }
            }

            #endregion
        }

        public class Database
        {
            #region types

            private enum DbStatisticsFields
            {
                TransactionsPerMinute = 7,
                LogFlushWaitsPerMinute = 5,
                LogFlushesPerMinute = 6, 
                LogKilobytesFlushedPerMinute = 2, 
                LogCacheHitRatio = 3
            }

            private enum DbSpaceStatisticsFields
            {
                DataSizeAllocatedInKilobytes = 2,
                DataSizeInKilobytes = 4, 
                LogSizeAllocatedInKilobytes = 6, 
                LogSizeInKilobytes = 7, 
                IndexSizeInKilobytes = 5, 
                TextSizeInKilobytes = 3
            }

            private enum TableStatisticsFields
            {
                NumberOfRows = 2, 
                DataSizeInKilobytes = 3, 
                TextSizeInKilobytes = 4, 
                IndexSizeInKilobytes = 5, 
                ReorganizationPercentage = 6
            }

            #endregion

            #region members

            private string m_Server;
            private string m_Db;
            private DateTime m_StartUTC;
            private DateTime m_EndUTC;
            private List<DbStatistics> m_DbStatistics;
            private List<DbSpaceStatistics> m_DbSpaceStatistics;
            private List<Table> m_Tables;

            #endregion

            #region ctors

            public Database(
                    string server,
                    string db,
                    DateTime startUTC,
                    DateTime endUTC
                )
            {
                m_Server = server;
                m_Db = db;
                m_StartUTC = startUTC;
                m_EndUTC = endUTC;
                m_DbStatistics = new List<DbStatistics>();
                m_DbSpaceStatistics = new List<DbSpaceStatistics>();
                m_Tables = new List<Table>();
            }

            #endregion

            #region properties

            public string Server
            {
                get { return m_Server; }
            }

            public string Db
            {
                get { return m_Db; }
            }

            public bool IsSystemDb
            {
                get { return IsSystemDbTest(Db); }
            }

            public DateTime StartUTC
            {
                get { return m_StartUTC; }
            }

            public DateTime StartLocal
            {
                get { return StartUTC.ToLocalTime(); }
            }

            public DateTime EndUTC
            {
                get { return m_EndUTC; }
            }

            public DateTime EndLocal
            {
                get { return EndUTC.ToLocalTime(); }
            }

            public List<DbStatistics> DbStatisticsData
            {
                get { return m_DbStatistics; }
            }

            public DbSpaceStatistics DbSpaceStatisticsData
            {
                get 
                {
                    if (m_DbSpaceStatistics.Count == 1)
                    {
                        return m_DbSpaceStatistics[0];
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            public List<Table> TableData
            {
                get { return m_Tables; }
            }

            #endregion

            #region methods

            public bool UpdateData(
                    SQLdm4x.StatisticsReader statsReader
                )
            {
                // Error handling - if any exception is encountered when reading data
                // then we set the flag and continue processing.  i.e. we are going to
                // collect as much data as possible.
                bool isAnyError = false;

                // Read database statistics.
                string statsStr = "DB- " + Db;
                try
                {
                    Stopwatch timer = Stopwatch.StartNew();
                    string[,] stats = statsReader.ReadDatabaseStatistics(Server, Db, StartLocal, EndLocal);
                    updateDbStatistics(stats);
                    timer.Stop();
                    statsStr += " - Statistics : " + timer.ElapsedMilliseconds.ToString() + " ms";
                }
                catch (Exception ex)
                {
                    Log.Error("Exception encountered while reading Db Statistics", ex);
                    isAnyError = true;
                }

                // Read database space statistics.
                try
                {
                    Stopwatch timer = Stopwatch.StartNew();
                    string[,] stats = statsReader.ReadDatabaseSpaceStatistics(Server, Db, StartLocal, EndLocal);
                    updateDbSpaceStatistics(stats);
                    timer.Stop();
                    statsStr += ", Space Statistics : " + timer.ElapsedMilliseconds.ToString() + " ms";
                }
                catch (Exception ex)
                {
                    Log.Error("Exception encountered while reading Db Space Statistics", ex);
                    isAnyError = true;
                }

                // Get a list of tables in the database
                string[] tables = null;
                try
                {
                    tables = statsReader.GetTables(Server, Db);
                }
                catch (Exception ex)
                {
                    Log.Error("Exception encountered while getting list of tables ", ex);
                    tables = null;
                    isAnyError = true;
                }

                // For each table
                if (tables != null)
                {
                    try
                    {
                        Stopwatch timer = Stopwatch.StartNew();
                        int cntr = 0;
                        foreach (string tbl in tables)
                        {
                            // Read table statistics
                            ++cntr;
                            if (!string.IsNullOrEmpty(tbl))
                            {
                                string[,] stats = statsReader.ReadTableStatistics(Server, Db, tbl, StartLocal, EndLocal);
                                updateTable(tbl, stats);
                            }
                        }
                        timer.Stop();
                        statsStr += ", Table Statistics (" + cntr.ToString() + " tables): " + timer.ElapsedMilliseconds.ToString() + " ms";
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Exception encountered while reading Table statistics ", ex);
                        isAnyError = true;
                    }
                }
                else
                {
                    Log.Info("No tables to process.");
                }

                // Log database stats.
                Log.Info(statsStr);

                return isAnyError;
            }

            public static bool IsSystemDbTest(
                    string dbName
                )
            {
                return (string.Compare(dbName, "master", true) == 0
                      || string.Compare(dbName, "msdb", true) == 0
                      || string.Compare(dbName, "tempdb", true) == 0
                      || string.Compare(dbName, "model", true) == 0);
            }

            private void updateDbStatistics(
                    string[,] stats
                )
            {
                // No db stats return.
                if (stats == null) { return; }

                // Get lower/upper bound of stats array.
                int rowsLowerBound = stats.GetLowerBound(1);
                int rowsUpperBound = stats.GetUpperBound(1);

                // Process each row.
                Wintellect.PowerCollections.Set<DateTime> processedTimeStamps = null;
                for (int row = rowsLowerBound + 1; row <= rowsUpperBound; row++)
                {
                    // Get the collection time, if not in the range then skip the row.
                    DateTime collectionTime = DateTime.Parse(stats[0, row]).ToUniversalTime();
                    if (collectionTime < StartUTC || collectionTime > EndUTC || isDuplicateTimeStamp(collectionTime, ref processedTimeStamps))
                    {
                        continue;
                    }

                    // Create database statistics object and add to list.
                    DbStatistics dbo = new DbStatistics(
                                            collectionTime,
                                            TimeDeltaInSeconds,
                                            (long)float.Parse(stats[(int)DbStatisticsFields.TransactionsPerMinute, row]), // transactionsPerMinute
                                            (long)float.Parse(stats[(int)DbStatisticsFields.LogFlushWaitsPerMinute, row]), // logFlushWaitsPerMinute
                                            (long)float.Parse(stats[(int)DbStatisticsFields.LogFlushesPerMinute, row]), // logFlushesPerMinute
                                            (long)float.Parse(stats[(int)DbStatisticsFields.LogKilobytesFlushedPerMinute, row]), // logKilobytesFlushedPerMinute
                                            float.Parse(stats[(int)DbStatisticsFields.LogCacheHitRatio, row]) // logCacheHitRatio
                                        );
                    m_DbStatistics.Add(dbo);
                }
            }

            private void updateDbSpaceStatistics(
                   string[,] stats
                )
            {
                // If no data, return.
                if (stats == null) { return; }

                // Get lower/upper bound of stats array.
                int rowsLowerBound = stats.GetLowerBound(1);
                int rowsUpperBound = stats.GetUpperBound(1);

                // Process each row.
                Wintellect.PowerCollections.Set<DateTime> processedTimeStamps = null;
                for (int row = rowsLowerBound + 1; row <= rowsUpperBound; row++)
                {
                    // Get the collection time, if not in the range then skip the row.
                    DateTime collectionTime = DateTime.Parse(stats[0, row]).ToUniversalTime();
                    if (collectionTime < StartUTC || collectionTime > EndUTC || isDuplicateTimeStamp(collectionTime, ref processedTimeStamps))
                    {
                        continue;
                    }

                    // Create the db space object and add to list.
                    DbSpaceStatistics obj = new DbSpaceStatistics(
                                                collectionTime,
                                                (decimal)float.Parse(stats[(int)DbSpaceStatisticsFields.DataSizeAllocatedInKilobytes, row]), // dataSizeAllocatedInKilobytes
                                                (decimal)float.Parse(stats[(int)DbSpaceStatisticsFields.DataSizeInKilobytes, row]), // dataSizeInKilobytes
                                                (decimal)float.Parse(stats[(int)DbSpaceStatisticsFields.LogSizeAllocatedInKilobytes, row]), // logSizeAllocatedInKilobytes
                                                (decimal)float.Parse(stats[(int)DbSpaceStatisticsFields.LogSizeInKilobytes, row]), // logSizeInKilobytes
                                                (decimal)float.Parse(stats[(int)DbSpaceStatisticsFields.IndexSizeInKilobytes, row]), // indexSizeInKilobytes
                                                (decimal)float.Parse(stats[(int)DbSpaceStatisticsFields.TextSizeInKilobytes, row]) //textSizeInKilobytes
                                            );
                    m_DbSpaceStatistics.Add(obj);
                }
            }

            private void updateTable(
                    string tbl,
                    string[,] tblStats
                )
            {
                // No table stats return.
                if (tblStats == null) { return; }

                // Get the table name.
                string table = tbl.EndsWith(".spn") ? tbl.Remove(tbl.Length - 4, 4) : tbl;
                string tableName, schemaName;
                splitTableName(table, out schemaName, out tableName);

                // Get upper and lower bounds.
                int rowsLowerBound = tblStats.GetLowerBound(1);
                int rowsUpperBound = tblStats.GetUpperBound(1);

                // Process each row.
                Wintellect.PowerCollections.Set<DateTime> processedTimeStamps = null;
                for (int row = rowsLowerBound + 1; row <= rowsUpperBound; row++)
                {
                    // Get the collection time, if not in the range then skip the row.
                    DateTime collectionTime = DateTime.Parse(tblStats[0, row]).ToUniversalTime();
                    if (collectionTime < StartUTC || collectionTime > EndUTC || isDuplicateTimeStamp(collectionTime, ref processedTimeStamps))
                    {
                        continue;
                    }

                    // Create table object and add to list.
                    Table to = new Table(
                                    tableName,
                                    schemaName,
                                    collectionTime,
                                    TimeDeltaInSeconds,
                                    (int)float.Parse(tblStats[(int)TableStatisticsFields.NumberOfRows, row]), // numberOfRows
                                    float.Parse(tblStats[(int)TableStatisticsFields.DataSizeInKilobytes, row]), // dataSizeInKilobytes
                                    float.Parse(tblStats[(int)TableStatisticsFields.TextSizeInKilobytes, row]), // textSizeInKilobytes
                                    float.Parse(tblStats[(int)TableStatisticsFields.IndexSizeInKilobytes, row]), // indexSizeInKilobytes
                                    float.Parse(tblStats[(int)TableStatisticsFields.ReorganizationPercentage, row]) // reorganizationPercentage
                            );
                    m_Tables.Add(to);
                }
            }

            private static void splitTableName(
                    string fqTable,
                    out string schema,
                    out string table
                )
            {
                // Init returns.
                schema = table = string.Empty;

                // Test if fully qualified name is in the following format
                // [schema].[table].
                int mi = fqTable.IndexOf("].[");
                if (fqTable.StartsWith("[")
                        && fqTable.EndsWith("]")
                            && mi != -1)
                {
                    schema = fqTable.Substring(1, mi - 1);
                    table = fqTable.Substring(mi + 3, fqTable.Length - mi - 4);
                }
                else
                {
                    schema = "";
                    table = fqTable;
                }
            }

            #endregion
        }

        public class WorstPerforming
        {
            #region members

            private DateTime m_UTCCollectionDateTime;
            private string m_DatabaseUsed;
            private DateTime m_UTCCompletionTime;
            private long m_DurationMilliseconds;
            private long m_CPUMilliseconds;
            private long m_Reads;
            private long m_Writes;
            private string m_NtUserName;
            private string m_SqlUserName;
            private string m_ApplicationName;
            private string m_ClientComputerName;
            private int m_StatementType;
            private string m_StatementText;

            #endregion

            #region ctors

            public WorstPerforming(
                    DateTime utcCollectionDateTime,
                    string databaseUsed,
                    DateTime utcCompletionTime,
                    long durationMilliseconds,
                    long cpuMilliseconds,
                    long reads,
                    long writes,
                    string ntUserName,
                    string sqlUserName,
                    string applicationName,
                    string clientComputerName,
                    int statementType,
                    string statementText
                )
            {
                m_UTCCollectionDateTime = utcCollectionDateTime;
                m_DatabaseUsed = databaseUsed;
                m_UTCCompletionTime = utcCompletionTime;
                m_DurationMilliseconds = durationMilliseconds;
                m_CPUMilliseconds = cpuMilliseconds;
                m_Reads = reads;
                m_Writes = writes;
                m_NtUserName = ntUserName;
                m_SqlUserName = sqlUserName;
                m_ApplicationName = applicationName;
                m_ClientComputerName = clientComputerName;
                m_StatementType = statementType;
                m_StatementText = statementText;
            }

            #endregion

            #region properties

            public SqlDateTime UTCCollectionDateTime
            {
                get { return new SqlDateTime(m_UTCCollectionDateTime); }
            }

            public string DatabaseUsed
            {
                get { return m_DatabaseUsed; }
            }

            public SqlDateTime UTCCompletionTime
            {
                get { return new SqlDateTime(m_UTCCompletionTime); }
            }

            public SqlInt64 DurationMilliseconds
            {
                get { return new SqlInt64(m_DurationMilliseconds); }
            }

            public SqlInt64 CPUMilliseconds
            {
                get { return new SqlInt64(m_CPUMilliseconds); }
            }

            public SqlInt64 Reads
            {
                get { return new SqlInt64(m_Reads); }
            }

            public SqlInt64 Writes
            {
                get { return new SqlInt64(m_Writes); }
            }

            public SqlString NtUserName
            {
                get { return new SqlString(m_NtUserName); }
            }

            public SqlString SqlUserName
            {
                get { return new SqlString(m_SqlUserName); }
            }

            public SqlString ApplicationName
            {
                get { return new SqlString(m_ApplicationName); }
            }

            public SqlString ClientComputerName
            {
                get { return new SqlString(m_ClientComputerName); }
            }

            public SqlInt32 StatementType
            {
                get { return new SqlInt32(m_StatementType); }
            }

            public SqlString StatementText
            {
                get { return new SqlString(m_StatementText); }
            }

            #endregion
        }

        private enum ServerStatFields
        {
            CpuActivityPercentage = 3,
            IoActivityPercentage = 4,
            PageReadsPerMinute = 13,
            PageWritesPerMinute = 14,
            PageErrorsPerMinute = 15,
            LazyWriterWritePerMinute = 34,
            CheckpointWritesPerMinute = 35,
            LogWritePerMinute = 36,
            PacketsReceivedPerMinute = 10,
            PacketsSentPerMinute = 11,
            PacketErrorsPerMinute = 12,
            LoginsPerMinute = 5,
            AverageUserProcesses = 17,
            ClientComputers = 29,
            TransactionPerMinute = 23,
            OldestOpenTransactionInMinutes = 24,
            ReplicationMaxUnsubscribedPerMinute = 19,
            ReplicationSubscribedPerMinute = 20,
            ReplicationMaxUndistributedPerMinute = 21,
            ReplicationLatencyInSeconds = 22,
            MaximumTempDBSizePercent = 8,
            MaximumTempDBSizeKilobytes = 9,
            MaximumProcedureCacheSizePercent = 6,
            MaximumProcedureCacheSizeKilobytes = 7,
            BufferCacheHitRatioPercentage = 2,
            ProcedureCacheHitRatioPercentage = 16,
            AverageResponseTimeInMilliseconds = 25,
            SqlCompilationsPerMinute = 27,
            SqlRecompilationsPerMinute = 28,
            WorkFilesCreatedPerMinute = 30,
            WorkTablesCreatedPerMinute = 31,
            MaximumMemoryAllocatedInKilobytes = 32,
            MaximumMemoryUsedInKilobytes = 33,
            TableLockEscalationsPerMinute = 39,
            ReadAheadPagesPerMinute = 40,
            PagesSplitPerMinute = 37,
            FullScansPerMinute = 38,
            MaximumPageLifeExpectancy = 41,
            LockWaitsPerMinute = 43,
            PageLookupsPerMinute = 42
        }

        private enum OSMetricsFields
        {
            TotalPhysicalMemory = 1,
            AvailableBytes = 2,
            PagesPerSecond = 4,
            PercentProcessorTime = 6,
            PercentPrivilegedTime = 8,
            PercentUserTime = 10,
            ProcessorQueueLength = 11,
            PercentDiskTime = 13,
            AvgDiskQueueLength = 15
        }

        #endregion

        #region constants

        private const float TimeDeltaInMinutes = 10;
        private const float TimeDeltaInSeconds = TimeDeltaInMinutes * 60;
                
        #endregion

        #region members

        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger("Import");

        private string m_Server;
        private DateTime m_StartUTC;
        private DateTime m_EndUTC;
        private List<ServerStatistics> m_ServerStatistics;
        private List<OSMetrics> m_OsMetrics;
        private List<Database> m_Databases;
        private List<WorstPerforming> m_WorstPerforming;

        #endregion

        #region ctors

        public SQLdm4xData(
                string server,
                DateTime startUTC,
                DateTime endUTC
            )
        {
            m_Server = server;
            m_StartUTC = startUTC;
            m_EndUTC = endUTC;
            m_ServerStatistics = new List<ServerStatistics>();
            m_OsMetrics = new List<OSMetrics>();
            m_Databases = new List<Database>();
            m_WorstPerforming = new List<WorstPerforming>();
        }

        #endregion

        #region properties

        public string Server
        {
            get { return m_Server; }
        }

        public DateTime StartUTC
        {
            get { return m_StartUTC; }
        }

        public DateTime StartLocal
        {
            get { return StartUTC.ToLocalTime(); }
        }

        public DateTime EndUTC
        {
            get { return m_EndUTC; }
        }

        public DateTime EndLocal
        {
            get { return EndUTC.ToLocalTime(); }
        }

        public List<ServerStatistics> ServerStatisticsData
        {
            get { return m_ServerStatistics; }
        }

        public List<OSMetrics> OSMetricsData
        {
            get { return m_OsMetrics; }
        }

        public List<Database> DatabaseData
        {
            get { return m_Databases; }
        }

        public List<WorstPerforming> WorstPerformingData
        {
            get { return m_WorstPerforming; }
        }

        #endregion

        #region methods

        public bool UpdateData(
                SQLdm4x.StatisticsReader statsReader,
                SQLdm4x.WorstPerformingReader worstPerfReader
            )
        {
            Debug.Assert(statsReader != null && worstPerfReader != null);

            // Error handling - if any exception is encountered when reading data
            // then we set the flag and continue processing.  i.e. we are going to
            // collect as much data as possible.
            bool isAnyError = false;

            // Read server statistics
            string statsStr = string.Empty;
            try
            {
                Stopwatch timer = Stopwatch.StartNew();
                string[,] stats = statsReader.ReadServerStatistics(Server, StartLocal, EndLocal);
                updateServerStatistics(stats);
                timer.Stop();
                statsStr = "Server- Statistics : " + timer.ElapsedMilliseconds.ToString() + " ms";
            }
            catch (Exception ex)
            {
                Log.Error("Exception encountered while reading Server Statistics", ex);
                isAnyError = true;
            }

            // Read OS metrics
            try
            {
                Stopwatch timer = Stopwatch.StartNew();
                string[,] stats = statsReader.ReadOSMetrics(Server, StartLocal, EndLocal);
                updateOSMetrics(stats);
                timer.Stop();
                statsStr += ", OS Metrics : " + timer.ElapsedMilliseconds.ToString() + " ms";
            }
            catch (Exception ex)
            {
                Log.Error("Exception encountered while reading OS Metrics ", ex);
                isAnyError = true;
            }

            // Read worst performing data.
            try
            {
                Stopwatch timer = Stopwatch.StartNew();
                ArrayList worstPerfStats = worstPerfReader.ReadWorstPerforming(Server, StartLocal, EndLocal);
                updateWorstPerforming(worstPerfStats);
                timer.Stop();
                statsStr += ", Worst Performing : " + timer.ElapsedMilliseconds.ToString() + " ms";
            }
            catch (Exception ex)
            {
                Log.Error("Exception encountered while reading Worst Performing data ", ex);
                isAnyError = true;
            }

            // Log server related statistics.
            Log.Info(statsStr);

            // Get a list of databases.
            string[] databases = null;
            try
            {
                databases = statsReader.GetDatabases(m_Server);
            }
            catch (Exception ex)
            {
                Log.Error("Exception encountered while get list of databases ", ex);
                databases = null;
                isAnyError = true;
            }

            // For each database 
            if (databases != null)
            {
                foreach (string db in databases)
                {
                    // Create new database object.
                    SQLdm4xData.Database database = new SQLdm4xData.Database(Server, db, StartUTC, EndUTC);

                    // Update database object.
                    isAnyError = database.UpdateData(statsReader);

                    // Add database object to data.
                    DatabaseData.Add(database);
                }
            }
            else
            {
                Log.Info("No databases to process.");
            }

            return isAnyError;
        }

        private void updateServerStatistics(
                string[,] stats
            )
        {
            // No server stats return.
            if (stats == null) { return; }

            // Get upper and lower bound of the stats array.
            int rowsLowerBound = stats.GetLowerBound(1);
            int rowsUpperBound = stats.GetUpperBound(1);

            // Process each stat row.
            Wintellect.PowerCollections.Set<DateTime> processedTimeStamps = null;
            for (int row = rowsLowerBound + 1; row <= rowsUpperBound; row++)
            {
                // Get the collection time, if not in the range then skip the row.
                DateTime collectionTime = DateTime.Parse(stats[0, row]).ToUniversalTime();
                if (collectionTime < StartUTC || collectionTime > EndUTC || isDuplicateTimeStamp(collectionTime, ref processedTimeStamps))
                {
                    continue;
                }

                // Create server statistics object, and add to the list.
                ServerStatistics s =
                        new ServerStatistics(
                                collectionTime,
                                TimeDeltaInSeconds,
                                float.Parse(stats[(int)ServerStatFields.CpuActivityPercentage, row]), // cpuActivityPercentage
                                float.Parse(stats[(int)ServerStatFields.IoActivityPercentage, row]), // ioActivityPercentage
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.PageReadsPerMinute, row])), // pageReadsPerMinute
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.PageWritesPerMinute, row])), // pageWritePerMinute
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.PageErrorsPerMinute, row])), // pageErrorsPerMinute
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.LazyWriterWritePerMinute, row])), // lazyWriterWritePerMinute
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.CheckpointWritesPerMinute, row])), // checkpointWritesPerMinute
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.LogWritePerMinute, row])), // logWritePerMinute
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.PacketsReceivedPerMinute, row])), // packetsReceivedPerMinute
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.PacketsSentPerMinute, row])), // packetsSentPerMinute
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.PacketErrorsPerMinute, row])), // packertErrorsPerMinute
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.LoginsPerMinute, row])), // loginsPerMinute
                                (int)float.Parse(stats[(int)ServerStatFields.AverageUserProcesses, row]), // averageUserProcesses
                                int.Parse(stats[(int)ServerStatFields.ClientComputers, row]), // clientComputers
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.TransactionPerMinute, row])), // transactionPerMinute
                                long.Parse(stats[(int)ServerStatFields.OldestOpenTransactionInMinutes, row]), // oldestOpenTransactionInMinutes
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.ReplicationMaxUnsubscribedPerMinute, row])), // replicationMaxUnsubscribedPerMinute
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.ReplicationSubscribedPerMinute, row])), // replicationSubscribedPerMinute
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.ReplicationMaxUndistributedPerMinute, row])), // replicationMaxUndistributedPerMinute
                                float.Parse(stats[(int)ServerStatFields.ReplicationLatencyInSeconds, row]), // replicationLatencyInSeconds
                                float.Parse(stats[(int)ServerStatFields.MaximumTempDBSizePercent, row]), // maximumTempDBSizePercent
                                long.Parse(stats[(int)ServerStatFields.MaximumTempDBSizeKilobytes, row]), // maximumTempDBSizeKilobytes
                                float.Parse(stats[(int)ServerStatFields.MaximumProcedureCacheSizePercent, row]), // maximumProcedureCacheSizePercent
                                long.Parse(stats[(int)ServerStatFields.MaximumProcedureCacheSizeKilobytes, row]), // maximumProcedureCacheSizeKilobytes
                                float.Parse(stats[(int)ServerStatFields.BufferCacheHitRatioPercentage, row]), // bufferCacheHitRatioPercentage
                                float.Parse(stats[(int)ServerStatFields.ProcedureCacheHitRatioPercentage, row]), // procedureCacheHitRatioPercentage
                                (int)float.Parse(stats[(int)ServerStatFields.AverageResponseTimeInMilliseconds, row]), // averageResponseTimeInMilliseconds
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.SqlCompilationsPerMinute, row])), // sqlCompilationsPerMinute
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.SqlRecompilationsPerMinute, row])), //sqlRecompilationsPerMinute
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.WorkFilesCreatedPerMinute, row])), // workFilesCreatedPerMinute
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.WorkTablesCreatedPerMinute, row])), // workTablesCreatedPerMinute
                                long.Parse(stats[(int)ServerStatFields.MaximumMemoryAllocatedInKilobytes, row]), // maximumMemoryAllocatedInKilobytes
                                long.Parse(stats[(int)ServerStatFields.MaximumMemoryUsedInKilobytes, row]), // maximumMemoryUserInKilobytes
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.TableLockEscalationsPerMinute, row])), // tableLockEscalationsPerMinute
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.ReadAheadPagesPerMinute, row])), // readAheadPagesPerMinute
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.PagesSplitPerMinute, row])), // pagesSplitPerMinute
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.FullScansPerMinute, row])), // fullScansPerMinute
                                long.Parse(stats[(int)ServerStatFields.MaximumPageLifeExpectancy, row]), // maximumPageLifeExpectancy
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.LockWaitsPerMinute, row])), // lockWaitsPerMinute
                                convertFromRate(float.Parse(stats[(int)ServerStatFields.PageLookupsPerMinute, row])) // pageLookupsPerMinute
                        );

                m_ServerStatistics.Add(s);
            }
        }

        private void updateOSMetrics(
                string[,] stats
            )
        {
            // No server stats return.
            if (stats == null) { return; }

            // Get upper and lower bounds.
            int rowsLowerBound = stats.GetLowerBound(1);
            int rowsUpperBound = stats.GetUpperBound(1);

            Wintellect.PowerCollections.Set<DateTime> processedTimeStamps = null;
            for (int row = rowsLowerBound + 1; row <= rowsUpperBound; row++)
            {
                // Get the collection time, if not in the range then skip the row.
                DateTime collectionTime = DateTime.Parse(stats[0, row]).ToUniversalTime();
                if (collectionTime < StartUTC || collectionTime > EndUTC || isDuplicateTimeStamp(collectionTime,ref processedTimeStamps))
                {
                    continue;
                }

                // Create OS metrics object and add to the list.
                OSMetrics osm = new OSMetrics(
                                    collectionTime,
                                    (long)(float.Parse(stats[(int)OSMetricsFields.TotalPhysicalMemory, row]) / 1000.0), // totalPhysicalMemory
                                    (long)(float.Parse(stats[(int)OSMetricsFields.AvailableBytes, row]) / 1000.0), // availableBytes
                                    float.Parse(stats[(int)OSMetricsFields.PagesPerSecond, row]), // pagesPerSecond
                                    float.Parse(stats[(int)OSMetricsFields.PercentProcessorTime, row]), // percentProcessorTime
                                    float.Parse(stats[(int)OSMetricsFields.PercentPrivilegedTime, row]), // percentPrivilegedTime
                                    float.Parse(stats[(int)OSMetricsFields.PercentUserTime, row]), // percentUserTime
                                    float.Parse(stats[(int)OSMetricsFields.ProcessorQueueLength, row]), // processorQueueLength
                                    float.Parse(stats[(int)OSMetricsFields.PercentDiskTime, row]), // percentDiskTime
                                    float.Parse(stats[(int)OSMetricsFields.AvgDiskQueueLength, row]) // avgDiskQueueLength
                            );
                m_OsMetrics.Add(osm);
            }
        }

        private void updateWorstPerforming(
                ArrayList statements
            )
        {
            // If no data, return.
            if (statements == null) { return; }

            // Process each statement.
            for (int i = 0; i < statements.Count; i++)
            {
                string[] statement = statements[i] as string[];

                if (statement != null)
                {
                    try
                    {
                        // Create object and add to list.
                        WorstPerforming wp = new WorstPerforming(
                                                DateTime.Parse(statement[(int)SQLdm4x.WorstPerformingReader.Fields.LocalCollectionDateTime] as string).ToUniversalTime(),
                                                statement[(int)SQLdm4x.WorstPerformingReader.Fields.DatabaseUsed], // databaseUsed
                                                DateTime.Parse(statement[(int)SQLdm4x.WorstPerformingReader.Fields.CompletionTime] as string).ToUniversalTime(), // completionTime
                                                long.Parse(statement[(int)SQLdm4x.WorstPerformingReader.Fields.AverageDurationMS]), // averageDurationMilliseconds
                                                long.Parse(statement[(int)SQLdm4x.WorstPerformingReader.Fields.CPUTimeMS]), // cpuTimeMilliseconds
                                                long.Parse(statement[(int)SQLdm4x.WorstPerformingReader.Fields.Reads]), // diskReads
                                                long.Parse(statement[(int)SQLdm4x.WorstPerformingReader.Fields.Writes]), // diskWrites
                                                statement[(int)SQLdm4x.WorstPerformingReader.Fields.NTUserName], // ntUserName
                                                statement[(int)SQLdm4x.WorstPerformingReader.Fields.SQLUserName], // sqlUserName
                                                statement[(int)SQLdm4x.WorstPerformingReader.Fields.ApplicationName], // applicationName
                                                statement[(int)SQLdm4x.WorstPerformingReader.Fields.ClientComputerName], // clientComputerName
                                                int.Parse(statement[(int)SQLdm4x.WorstPerformingReader.Fields.StatementType]), // statementType
                                                statement[(int)SQLdm4x.WorstPerformingReader.Fields.StatementText] // statementText
                                            );
                        m_WorstPerforming.Add(wp);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Unable to parse worst performing statement, ", ex);
                    }
                }
            }

        }

        private static long convertFromRate(float val)
        {
            double r = val * TimeDeltaInMinutes;
            return (long)r;
        }

        private static bool isDuplicateTimeStamp(
                DateTime timeStamp,
                ref Wintellect.PowerCollections.Set<DateTime> processedTimeStamps
            )
        {
            bool flag = false;
            if (processedTimeStamps == null)
            {
                flag = false;
                processedTimeStamps = new Wintellect.PowerCollections.Set<DateTime>();
                processedTimeStamps.Add(timeStamp);
            }
            else
            {
                if(processedTimeStamps.Contains(timeStamp))
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                    processedTimeStamps.Add(timeStamp);
                }
            }
            return flag;
        }

        #endregion
    }
}
