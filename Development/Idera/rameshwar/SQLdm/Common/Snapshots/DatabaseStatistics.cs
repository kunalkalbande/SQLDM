//------------------------------------------------------------------------------
// <copyright file="DatabaseStatistics.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents database level performance statistics
    /// </summary>
    [Serializable]
    public class DatabaseStatistics : DatabaseSize
    {
        #region fields

        private long? transactions_Raw = null;
        private long? logFlushes_Raw = null;
        private FileSize logSizeFlushed_Raw = new FileSize();
        private long? logCacheReads_Raw = null;
        private long? logFlushWaits_Raw = null;
        private long? logCacheHitRatio_Base = null;
        private long? logCacheHitRatio_Raw = null;
       
        private long? transactions = null;
        private long? logFlushes = null;
        private FileSize logSizeFlushed = new FileSize();
        private long? logCacheReads = null;
        private double? logCacheHitRatio = null;
        private long? logFlushWaits = null;

        private Dictionary<String, TableSize> tableSizes = new Dictionary<string, TableSize>();
        private Dictionary<String, TableReorganization> tableReorganizations = new Dictionary<string, TableReorganization>();
        //TODO Add a list of filesstats here
        private bool isSystemDatabase = false;

        private decimal? reads_Raw = null;
        private decimal? writes_Raw = null;
        private decimal? bytesRead_Raw = null;
        private decimal? bytesWritten_Raw = null;
        private decimal? ioStallMs_Raw = null;

        private decimal? reads = null;
        private decimal? writes = null;
        private decimal? bytesRead = null;
        private decimal? bytesWritten = null;
        private decimal? ioStallMs = null;

        private bool logAutogrowDetected = false;
        private bool dataAutogrowDetected = false;

        private DateTime? dateCreated = null;
        private DateTime? lastBackup = null;

        private bool? isPrimary = null;

        private long databaseId = 0;

        private AzureDbDetail azureDbDetail;

        private Dictionary<string, FileActivityFile> files = new Dictionary<string, FileActivityFile>();

        private List<DatabaseFileStatistics> fileStatistics = new List<DatabaseFileStatistics>();  // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --files statistics object

        private decimal? azureCloudAllocatedMemory = null;
        private decimal? azureCloudUsedMemory = null;
        private decimal? azureCloudStorageLimit = null;

        private string elasticPool = null;

        #endregion

        #region constructors

        public DatabaseStatistics(string serverName, string dbName) : base(serverName, dbName)
        {
        }

        public DatabaseStatistics(string serverName, string dbName, long databaseId)
            : base(serverName, dbName)
        {
            this.databaseId = databaseId;
        }

        public override object Clone()
        {
            // base class does a memberwiseclone.  have to handle complex objects ourselves.
            var result = (DatabaseStatistics)base.Clone();

            result.logSizeFlushed_Raw = logSizeFlushed_Raw.Copy();
            result.logSizeFlushed = logSizeFlushed.Copy();

            if (files.Count > 0)
            {
                result.files.Clear();
                foreach (var entry in files)
                {
                    result.files.Add(entry.Key, (FileActivityFile) entry.Value.Clone());
                }
            }
            if (tableSizes.Count > 0)
            {
                result.tableSizes.Clear();
                foreach (var entry in tableSizes)
                {
                    result.tableSizes.Add(entry.Key, (TableSize)entry.Value.Clone());
                }
            }
            if (tableReorganizations.Count > 0)
            {
                result.tableReorganizations.Clear();
                foreach (var entry in tableReorganizations)
                {
                    result.tableReorganizations.Add(entry.Key, (TableReorganization)entry.Value.Clone());
                }
            }

            return result;
        }

        #endregion

        #region properties

        #region raw values

        public long? Transactions_Raw
        {
            get { return transactions_Raw; }
            internal set { transactions_Raw = value; }
        }

        public long? LogFlushes_Raw
        {
            get { return logFlushes_Raw; }
            internal set { logFlushes_Raw = value; }
        }

        public FileSize LogSizeFlushed_Raw
        {
            get { return logSizeFlushed_Raw; }
            internal set { logSizeFlushed_Raw = value; }
        }

        public long? LogCacheReads_Raw
        {
            get { return logCacheReads_Raw; }
            internal set { logCacheReads_Raw = value; }
        }

        public long? LogFlushWaits_Raw
        {
            get { return logFlushWaits_Raw; }
            internal set { logFlushWaits_Raw = value; }
        }

        public long? LogCacheHitRatio_Base
        {
            get { return logCacheHitRatio_Base; }
            internal set { logCacheHitRatio_Base = value; }
        }

        public long? LogCacheHitRatio_Raw
        {
            get { return logCacheHitRatio_Raw; }
            internal set { logCacheHitRatio_Raw = value; }
        }

        #endregion

        public Dictionary<string, FileActivityFile> Files
        {
            get { return files; }
            internal set { files = value; }
        }

        // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --files statistics property
        public List<DatabaseFileStatistics> FileStatistics
        {
            get { return fileStatistics; }
            internal set { fileStatistics = value; }
        }

        public long DatabaseId
        {
            get { return databaseId; }
            internal set { databaseId = value; }
        }

        public long? Transactions
        {
            get { return transactions; }
            internal set { transactions = value; }
        }

        public long? LogFlushes
        {
            get { return logFlushes; }
            internal set { logFlushes = value; }
        }

        public FileSize LogSizeFlushed
        {
            get { return logSizeFlushed; }
            internal set { logSizeFlushed = value; }
        }

        public AzureDbDetail AzureDbDetail
        {
            get { return azureDbDetail; }
            internal set { azureDbDetail = value; }
        }

        public long? LogCacheReads
        {
            get { return logCacheReads; }
            internal set { logCacheReads = value; }
        }

        public double? LogCacheHitRatio
        {
            get { return logCacheHitRatio; }
            internal set { logCacheHitRatio = value; }
        }

        public long? LogFlushWaits
        {
            get { return logFlushWaits; }
            internal set { logFlushWaits = value; }
        }

        public Dictionary<string, TableSize> TableSizes
        {
            get { return tableSizes; }
            internal set { tableSizes = value; }
        }

        public Dictionary<string, TableReorganization> TableReorganizations
        {
            get { return tableReorganizations; }
            internal set { tableReorganizations = value; }
        }


        public bool IsSystemDatabase
        {
            get { return isSystemDatabase; }
            internal set { isSystemDatabase = value; }
        }


        public decimal? Reads_Raw
        {
            get { return reads_Raw; }
            internal set { reads_Raw = value; }
        }

        public decimal? Writes_Raw
        {
            get { return writes_Raw; }
            internal set { writes_Raw = value; }
        }

        public decimal? BytesRead_Raw
        {
            get { return bytesRead_Raw; }
            internal set { bytesRead_Raw = value; }
        }

        public decimal? BytesWritten_Raw
        {
            get { return bytesWritten_Raw; }
            internal set { bytesWritten_Raw = value; }
        }

        public decimal? IoStallMs_Raw
        {
            get { return ioStallMs_Raw; }
            internal set { ioStallMs_Raw = value; }
        }


        public decimal? Reads
        {
            get { return reads; }
            internal set { reads = value; }
        }

        public decimal? Writes
        {
            get { return writes; }
            internal set { writes = value; }
        }

        public decimal? BytesRead
        {
            get { return bytesRead; }
            internal set { bytesRead = value; }
        }

        public decimal? BytesWritten
        {
            get { return bytesWritten; }
            internal set { bytesWritten = value; }
        }

        public decimal? IoStallMs
        {
            get { return ioStallMs; }
            internal set { ioStallMs = value; }
        }

       

        /// <summary>
        /// Gets the date the database was created.
        /// </summary>
        public DateTime? DateCreated
        {
            get { return dateCreated; }
            internal set { dateCreated = value; }
        }

        public DateTime? LastBackup
        {
            get { return lastBackup; }
            internal set { lastBackup = value; }
        }

        public bool LogAutogrowDetected
        {
            get { return logAutogrowDetected; }
            set { logAutogrowDetected = value; }
        }

        public bool DataAutogrowDetected
        {
            get { return dataAutogrowDetected; }
            set { dataAutogrowDetected = value; }
        }

        // true mean it is primary role in Availability Group
        // false mean it is sencondary role in Availability Group
        // null mean it is not part of Availability Group
        public bool? IsPrimary
        {
            get { return isPrimary; }
            set { isPrimary = value; }
        }


        // Start - 6.2.3
        public decimal? AzureCloudAllocatedMemory
        {
            get { return azureCloudAllocatedMemory; }
            set { azureCloudAllocatedMemory = value; }
        }

        public decimal? AzureCloudUsedMemory
        {
            get { return azureCloudUsedMemory; }
            set { azureCloudUsedMemory = value; }
        }

        public decimal? AzureCloudStorageLimit
        {
            get { return azureCloudStorageLimit; }
            set { azureCloudStorageLimit = value; }
        }
        public string ElasticPool
        {
            get { return elasticPool; }
            set { elasticPool = value; }
        }
        // end 6.2.3
        #endregion

        #region events

        #endregion

        #region methods


        private static Int64? CalculateInt64CounterDelta(Int64? previousCounter, Int64? currentCounter)
        {
            Int64? counterDelta = currentCounter - previousCounter;
            if (counterDelta < 0)
                return null;
            else
                return counterDelta;
        }

        public static void CalculateTransactions(ScheduledRefresh previousRefresh, ScheduledRefresh currentRefresh)
        {
            foreach (DatabaseStatistics dbstats in currentRefresh.DbStatistics.Values)
            {
                if (previousRefresh.DbStatistics.ContainsKey(dbstats.Name))
                {
                    dbstats.Transactions = CalculateInt64CounterDelta(previousRefresh.DbStatistics[dbstats.Name].Transactions_Raw, dbstats.Transactions_Raw);
                }
            }
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
