//------------------------------------------------------------------------------
// <copyright file="DatabaseSummaryConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration object for database summary on-demand probe
    /// </summary>
    [Serializable]
    public class DatabaseSummaryConfiguration : DatabaseProbeConfiguration
    {
        #region fields

        private Dictionary<string, DatabaseStatistics> previousDatabaseStatistics =
            new Dictionary<string, DatabaseStatistics>();
        private DateTime? lastRefresh = null;
        private DateTime? serverStartupTime = null;
        private bool includeSummaryData = false;

        #endregion

        #region constructors

        public DatabaseSummaryConfiguration(int monitoredServerId) : base(monitoredServerId)
        {
        }

        public DatabaseSummaryConfiguration(int monitoredServerId, bool includeSystemDatabases) : base(monitoredServerId, includeSystemDatabases)
        {
        }

        public DatabaseSummaryConfiguration(int monitoredServerId, bool includeSystemDatabases, DatabaseSummary previousDatabaseSummary)
            : base(monitoredServerId, includeSystemDatabases)
        {
            SetPreviousRefresh(previousDatabaseSummary);
        }


        #endregion

        #region properties

        public Dictionary<string, DatabaseStatistics> PreviousDatabaseStatistics
        {
            get { return previousDatabaseStatistics; }
            set { previousDatabaseStatistics = value; }
        }

        public DateTime? LastRefresh
        {
            get { return lastRefresh; }
            set { lastRefresh = value; }
        }

        public DateTime? ServerStartupTime
        {
            get { return serverStartupTime; }
            set { serverStartupTime = value; }
        }


        public bool IncludeSummaryData
        {
            get { return includeSummaryData; }
            set { includeSummaryData = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        public void SetPreviousRefresh(DatabaseSummary previousDatabaseSummary)
        {
            //Remove unnecessary data
            if (previousDatabaseSummary != null)
            {
                previousDatabaseStatistics.Clear();
                foreach (DatabaseDetail detail in previousDatabaseSummary.Databases.Values)
                {
                    DatabaseStatistics dbstats = new DatabaseStatistics(detail.ServerName, detail.Name);
                    dbstats.Transactions_Raw = detail.Transactions_Raw;
                    dbstats.LogFlushes_Raw = detail.LogFlushes_Raw;
                    dbstats.LogSizeFlushed_Raw = detail.LogSizeFlushed_Raw;
                    dbstats.LogSizeFlushed_Raw = detail.LogSizeFlushed_Raw;
                    dbstats.LogFlushWaits_Raw = detail.LogFlushWaits_Raw;
                    dbstats.LogCacheReads_Raw = detail.LogCacheReads_Raw;

                    previousDatabaseStatistics.Add(dbstats.Name, dbstats);
                }

            lastRefresh = previousDatabaseSummary.TimeStamp;
            serverStartupTime = previousDatabaseSummary.ServerStartupTime;
            }
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
