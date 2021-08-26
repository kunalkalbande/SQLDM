//------------------------------------------------------------------------------
// <copyright file="QueryMonitorConfigurationInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Objects
{
    using System;
    using System.Collections.Generic;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Snapshots;
    using Wintellect.PowerCollections;
    using Idera.SQLdm.Common.Attributes;

    public class QueryMonitorConfigurationInfo : ICloneable
    {
        private const int TopPlanCountDefault = 75;
        private const int TopPlanCategoryDefault = 0;
        private bool enabled;
        private bool sqlBatchEventsEnabled;
        private bool sqlStatementEventsEnabled;
        private bool storedProcedureEventsEnabled;
        private TimeSpan durationFilter;

        private int topPlanCountFilter;
        private int topPlanCategoryFilter;
        private TimeSpan cpuUsageFilter;
        private int logicalDiskReads;
        private int physicalDiskWrites;
        private FileSize traceFileSize = new FileSize(1024);
        private int traceFileRollovers;
        private int recordsPerRefresh;
        private bool deadlockEventsEnabled;
        private AdvancedQueryMonitorConfiguration advancedConfiguration;

        //Power shell snapin for remaining for controls
        //SQLdm10.1 (Srishti Purohit)
        private bool queryMonitorTraceMonitoringEnabled;
        private bool queryMonitorCollectQueryPlan;
        private bool queryMonitorCollectEstimatedQueryPlan;
        // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store
        private bool queryMonitorQueryStoreMonitoringEnabled;

        public QueryMonitorConfigurationInfo(QueryMonitorConfiguration config)
        {
            enabled = config.Enabled;
            sqlBatchEventsEnabled = config.SqlBatchEventsEnabled;
            sqlStatementEventsEnabled = config.SqlStatementEventsEnabled;
            storedProcedureEventsEnabled = config.StoredProcedureEventsEnabled;
            durationFilter = config.DurationFilter;
            topPlanCountFilter = config.TopPlanCountFilter;
            topPlanCategoryFilter = config.TopPlanCategoryFilter;
            cpuUsageFilter = config.CpuUsageFilter;
            logicalDiskReads = config.LogicalDiskReads;
            physicalDiskWrites = config.PhysicalDiskWrites;
            traceFileSize = config.TraceFileSize;
            traceFileRollovers = config.TraceFileRollovers;
            recordsPerRefresh = config.RecordsPerRefresh;
            string xml = AdvancedQueryMonitorConfiguration.SerializeToXml(config.AdvancedConfiguration);
            advancedConfiguration = AdvancedQueryMonitorConfiguration.DeserializeFromXml(xml);

            //Power shell snapin for remaining for controls
            //SQLdm10.1 (Srishti Purohit)
            queryMonitorTraceMonitoringEnabled = config.TraceMonitoringEnabled;
            // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store
            queryMonitorQueryStoreMonitoringEnabled = config.QueryStoreMonitoringEnabled;
            queryMonitorCollectQueryPlan = config.CollectQueryPlan;
            queryMonitorCollectEstimatedQueryPlan = config.CollectEstimatedQueryPlan;
        }

        public QueryMonitorConfigurationInfo()
        {
            enabled = false;
            sqlBatchEventsEnabled = true;
            sqlStatementEventsEnabled = true;
            storedProcedureEventsEnabled = true;
            deadlockEventsEnabled = true;
            topPlanCountFilter = TopPlanCountDefault;
            topPlanCategoryFilter = TopPlanCategoryDefault;
            durationFilter = TimeSpan.FromMilliseconds(5000);
            cpuUsageFilter = TimeSpan.Zero;
            logicalDiskReads = 0;
            physicalDiskWrites = 0;
            traceFileRollovers = 2;
            recordsPerRefresh = 1000;
            advancedConfiguration = new AdvancedQueryMonitorConfiguration(true);
            //Power shell snapin for remaining for controls
            //SQLdm10.1 (Srishti Purohit)
            queryMonitorTraceMonitoringEnabled = false;
            // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store
            queryMonitorQueryStoreMonitoringEnabled = false;
            queryMonitorCollectQueryPlan = true;
            queryMonitorCollectEstimatedQueryPlan = false;
        }

        public QueryMonitorConfigurationInfo(QueryMonitorConfigurationInfo copy)
        {
            enabled = copy.Enabled;
            sqlBatchEventsEnabled = copy.SqlBatchEventsEnabled;
            sqlStatementEventsEnabled = copy.SqlStatementEventsEnabled;
            storedProcedureEventsEnabled = copy.StoredProcedureEventsEnabled;
            topPlanCountFilter = copy.TopPlanCountFilter;
            topPlanCategoryFilter = copy.TopPlanCategoryFilter;
            durationFilter = copy.DurationFilter;
            cpuUsageFilter = copy.CpuUsageFilter;
            logicalDiskReads = copy.LogicalDiskReads;
            physicalDiskWrites = copy.PhysicalDiskWrites;
            traceFileSize = copy.TraceFileSize;
            traceFileRollovers = copy.TraceFileRollovers;
            recordsPerRefresh = copy.RecordsPerRefresh;

            AdvancedQueryMonitorConfiguration copyac = copy.AdvancedConfiguration;
            advancedConfiguration = new AdvancedQueryMonitorConfiguration(false);
            advancedConfiguration.ApplicationExcludeLike = copyac.ApplicationExcludeLike;
            advancedConfiguration.ApplicationExcludeMatch = copyac.ApplicationExcludeMatch;
            advancedConfiguration.DatabaseExcludeLike = copyac.DatabaseExcludeLike;
            advancedConfiguration.DatabaseExcludeMatch = copyac.DatabaseExcludeMatch;
            advancedConfiguration.SqlTextExcludeLike = copyac.SqlTextExcludeLike;
            advancedConfiguration.SqlTextExcludeMatch = copyac.SqlTextExcludeMatch;

            //Power shell snapin for remaining for controls
            //SQLdm10.1 (Srishti Purohit)
            queryMonitorTraceMonitoringEnabled = copy.queryMonitorTraceMonitoringEnabled;
            // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store
            queryMonitorQueryStoreMonitoringEnabled = copy.queryMonitorQueryStoreMonitoringEnabled;
            queryMonitorCollectQueryPlan = copy.queryMonitorCollectQueryPlan;
            queryMonitorCollectEstimatedQueryPlan = copy.queryMonitorCollectEstimatedQueryPlan;
        }

        internal QueryMonitorConfiguration GetInternalConfiguration()
        {
            return
                new QueryMonitorConfiguration(enabled, sqlBatchEventsEnabled, sqlStatementEventsEnabled,
                                              storedProcedureEventsEnabled, durationFilter, cpuUsageFilter,
                                              logicalDiskReads, physicalDiskWrites, traceFileSize, TraceFileRollovers,
                                              recordsPerRefresh, advancedConfiguration, queryMonitorTraceMonitoringEnabled //10.1 (srishti purohit) -- Power shell support
                                              , queryMonitorCollectQueryPlan, queryMonitorCollectEstimatedQueryPlan,    //SQLdm 9.1 (Ankit Srivastava) -- Fixed Open functional isssue--changed default value according to the implementaion
                                              this.topPlanCountFilter,
                                              this.topPlanCategoryFilter,
                                              this.queryMonitorQueryStoreMonitoringEnabled);  // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store
        }


        [Auditable("Enabled Query Monitor Configuration")]
        public bool Enabled
        {
            get { return enabled; }
            internal set { enabled = value; }
        }

        [Auditable("Enabled capturing of SQL batches")]
        public bool SqlBatchEventsEnabled
        {
            get { return sqlBatchEventsEnabled; }
            internal set { sqlBatchEventsEnabled = value; }
        }

        [Auditable("Enabled capturing of SQL statements")]
        public bool SqlStatementEventsEnabled
        {
            get { return sqlStatementEventsEnabled; }
            internal set { sqlStatementEventsEnabled = value; }
        }

        [Auditable("Enabled capturing of stored procedures and triggers")]
        public bool StoredProcedureEventsEnabled
        {
            get { return storedProcedureEventsEnabled; }
            internal set { storedProcedureEventsEnabled = value; }
        }

        [Auditable("Enabled capturing of deadlock events")]
        public bool DeadlockEventsEnabled
        {
            get { return deadlockEventsEnabled; }
            internal set { deadlockEventsEnabled = value; }
        }

        [Auditable("Changed Duration threshold to (milliseconds)")]
        public TimeSpan DurationFilter
        {
            get { return durationFilter; }
            internal set { durationFilter = value; }
        }

        [Auditable("Top Query Plan Count")]
        public int TopPlanCountFilter
        {
            get { return topPlanCountFilter; }
            internal set { topPlanCountFilter = value; }
        }

        [Auditable("Top Query Plan Category")]
        public int TopPlanCategoryFilter
        {
            get { return topPlanCategoryFilter; }
            internal set { topPlanCategoryFilter = value; }
        }

        [Auditable("Changed CPU Usage threshold to (milliseconds)")]
        public TimeSpan CpuUsageFilter
        {
            get { return cpuUsageFilter; }
            internal set { cpuUsageFilter = value; }
        }

        [Auditable("Changed Logical disk reads threshold to")]
        public int LogicalDiskReads
        {
            get { return logicalDiskReads; }
            internal set { logicalDiskReads = value; }
        }

        [Auditable("Changed Physical disk writes threshold to")]
        public int PhysicalDiskWrites
        {
            get { return physicalDiskWrites; }
            internal set { physicalDiskWrites = value; }
        }

        public FileSize TraceFileSize
        {
            get { return traceFileSize; }
            internal set { traceFileSize = value; }
        }

        public int TraceFileRollovers
        {
            get { return traceFileRollovers; }
            internal set { traceFileRollovers = value; }
        }

        public int RecordsPerRefresh
        {
            get { return recordsPerRefresh; }
            internal set { recordsPerRefresh = value; }
        }
        public bool QueryMonitorTraceMonitoringEnabled
        {
            get
            {
                return queryMonitorTraceMonitoringEnabled;
            }

            internal set
            {
                queryMonitorTraceMonitoringEnabled = value;
            }
        }

        /// <summary>
        /// Query monitor using Query Store
        /// </summary>
        public bool QueryMonitorQueryStoreMonitoringEnabled
        {
            get
            {
                return queryMonitorQueryStoreMonitoringEnabled;
            }

            internal set
            {
                queryMonitorQueryStoreMonitoringEnabled = value;
            }
        }

        public bool QueryMonitorCollectQueryPlan
        {
            get
            {
                return queryMonitorCollectQueryPlan;
            }

            internal set
            {
                queryMonitorCollectQueryPlan = value;
            }
        }

        public bool QueryMonitorCollectEstimatedQueryPlan
        {
            get
            {
                return queryMonitorCollectEstimatedQueryPlan;
            }

            internal set
            {
                queryMonitorCollectEstimatedQueryPlan = value;
            }
        }

        public AdvancedQueryMonitorConfiguration AdvancedConfiguration
        {
            get
            {
                if (advancedConfiguration == null)
                {
                    advancedConfiguration = new AdvancedQueryMonitorConfiguration();
                }

                return advancedConfiguration;
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            QueryMonitorConfigurationInfo queryMonitorConfiguration = obj as QueryMonitorConfigurationInfo;
            if (queryMonitorConfiguration == null) return false;
            if (!Equals(enabled, queryMonitorConfiguration.enabled)) return false;
            if (!Equals(sqlBatchEventsEnabled, queryMonitorConfiguration.sqlBatchEventsEnabled)) return false;
            if (!Equals(sqlStatementEventsEnabled, queryMonitorConfiguration.sqlStatementEventsEnabled)) return false;
            if (!Equals(storedProcedureEventsEnabled, queryMonitorConfiguration.storedProcedureEventsEnabled)) return false;
            if (!Equals(deadlockEventsEnabled, queryMonitorConfiguration.deadlockEventsEnabled)) return false;
            if (!Equals(durationFilter, queryMonitorConfiguration.durationFilter)) return false;
            if (!Equals(this.topPlanCountFilter, queryMonitorConfiguration.topPlanCountFilter)) return false;
            if (!Equals(this.topPlanCategoryFilter, queryMonitorConfiguration.topPlanCategoryFilter)) return false;
            if (!Equals(cpuUsageFilter, queryMonitorConfiguration.cpuUsageFilter)) return false;
            if (logicalDiskReads != queryMonitorConfiguration.logicalDiskReads) return false;
            if (physicalDiskWrites != queryMonitorConfiguration.physicalDiskWrites) return false;
            if (!Equals(traceFileSize, queryMonitorConfiguration.traceFileSize)) return false;
            if (traceFileRollovers != queryMonitorConfiguration.traceFileRollovers) return false;
            if (recordsPerRefresh != queryMonitorConfiguration.recordsPerRefresh) return false;
            if (!Equals(advancedConfiguration, queryMonitorConfiguration.advancedConfiguration)) return false;
            //Power shell snapin for remaining for controls
            //SQLdm10.1 (Srishti Purohit)
            if (queryMonitorTraceMonitoringEnabled != queryMonitorConfiguration.queryMonitorTraceMonitoringEnabled) return false;
            // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store
            if (queryMonitorQueryStoreMonitoringEnabled != queryMonitorConfiguration
                    .queryMonitorQueryStoreMonitoringEnabled)
            {
                return false;
            }
            if (queryMonitorCollectQueryPlan != queryMonitorConfiguration.queryMonitorCollectQueryPlan) return false;
            if (queryMonitorCollectEstimatedQueryPlan != queryMonitorConfiguration.queryMonitorCollectEstimatedQueryPlan) return false;
            return true;
        }

        public string[] ExcludedApplications
        {
            get
            {
                List<string> result = new List<string>();
                string[] values = advancedConfiguration.ApplicationExcludeMatch;
                if (values != null && values.Length > 0)
                    result.AddRange(values);
                values = advancedConfiguration.ApplicationExcludeLike;
                if (values != null && values.Length > 0)
                    result.AddRange(values);
                return result.ToArray();
            }
            set
            {
                string[] matchFilters, likeFilters;
                ParseFilterStrings(value, out matchFilters, out likeFilters);
                advancedConfiguration.ApplicationExcludeLike = likeFilters;
                advancedConfiguration.ApplicationExcludeMatch = matchFilters;
            }
        }
        public string[] ExcludedDatabases
        {
            get
            {
                List<string> result = new List<string>();
                string[] values = advancedConfiguration.DatabaseExcludeMatch;
                if (values != null && values.Length > 0)
                    result.AddRange(values);
                values = advancedConfiguration.DatabaseExcludeLike;
                if (values != null && values.Length > 0)
                    result.AddRange(values);
                return result.ToArray();
            }
            set
            {
                string[] matchFilters, likeFilters;
                ParseFilterStrings(value, out matchFilters, out likeFilters);
                advancedConfiguration.DatabaseExcludeLike = likeFilters;
                advancedConfiguration.DatabaseExcludeMatch = matchFilters;
            }
        }
        public string[] ExcludedSqlText
        {
            get
            {
                List<string> result = new List<string>();
                string[] values = advancedConfiguration.SqlTextExcludeMatch;
                if (values != null && values.Length > 0)
                    result.AddRange(values);
                values = advancedConfiguration.SqlTextExcludeLike;
                if (values != null && values.Length > 0)
                    result.AddRange(values);
                return result.ToArray();
            }
            set
            {
                string[] matchFilters, likeFilters;
                ParseFilterStrings(value, out matchFilters, out likeFilters);
                advancedConfiguration.SqlTextExcludeLike = likeFilters;
                advancedConfiguration.SqlTextExcludeMatch = matchFilters;
            }
        }


        private static void ParseFilterStrings(string[] filters, out string[] matchFilters, out string[] likeFilters)
        {
            OrderedSet<string> matchSet = new OrderedSet<string>();
            OrderedSet<string> likeSet = new OrderedSet<string>();

            if (filters != null)
            {
                foreach (string untrimmedFilter in filters)
                {
                    string filter = untrimmedFilter.Trim();

                    if (filter.Length > 1 && filter[0] == '[' && filter[filter.Length - 1] == ']')
                    {
                        filter = filter.Remove(0, 1);
                        filter = filter.Remove(filter.Length - 1, 1);
                    }

                    if (filter.Contains("%") && !likeSet.Contains(filter))
                    {
                        likeSet.Add(filter);
                    }
                    else if (!matchSet.Contains(filter))
                    {
                        matchSet.Add(filter);
                    }
                }
            }

            matchFilters = matchSet.ToArray();
            likeFilters = likeSet.ToArray();
        }


        #region ICloneable Members

        public object Clone()
        {
            return new QueryMonitorConfigurationInfo(this);
        }

        #endregion
    }
}
