using System;
using System.Text;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Configuration
{
    [Serializable]
    public sealed class QueryMonitorConfiguration
    {
        //START SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new fields for uery monitoring extended event session configuration
        public const int FileSizeXeMBDefault = 20;
        public const int FileSizeRolloverXeDefault = 5;
        private const int TopPlanCountDefault = 75;
        private const int TopPlanCategoryDefault = 0;

        // SQLdm 10.4 (Varun Chopra) Enable Query Monitor using Query Store
        private const bool QueryStoreMonitoringEnabledDefault = false;

        private int fileSizeXEMB = FileSizeXeMBDefault;
        private int fileSizeRolloverXE = FileSizeRolloverXeDefault;
        //END SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new fields for uery monitoring extended event session configuration

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
        private AdvancedQueryMonitorConfiguration advancedConfiguration;
        private DateTime? stopTimeUTC = null;
        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  -- Added two new fields
        private bool traceMonitoringEnabled = false;
        private bool collectQueryPlan = false;//SQLdm 9.1 (Ankit Srivastava) -- Fixed Open functional isssue--changed default value according to the implementaion
        private bool collectEstimatedQueryPlan = true;

        // SQLdm 10.4 (Varun Chopra) Enable Query Monitor using Query Store
        private bool queryStoreMonitoringEnabled = false;

        public QueryMonitorConfiguration(bool enabled, bool sqlBatchEventsEnabled, bool sqlStatementEventsEnabled,
                                         bool storedProcedureEventsEnabled, TimeSpan durationFilter,
                                         TimeSpan cpuUsageFilter, int logicalDiskReads, int physicalDiskWrites,
                                         FileSize traceFileSize, int traceFileRollovers, int recordsPerRefresh,
                                         AdvancedQueryMonitorConfiguration advancedConfiguration, bool traceMonitoringEnabled, bool collectQueryPlan, bool collectEstimatedQueryPlan = true,  //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- modifed the Constructor with two new parameters
                                         int topPlanCount = TopPlanCountDefault,
                                         int topPlanCountCategory = TopPlanCategoryDefault,
                                         bool queryStoreMonitoringEnabled = QueryStoreMonitoringEnabledDefault)
            //SQLdm 10.0 - Display Estimated Query Plan - Added a flag for collecting estimated query plan only
            :
            this(enabled, sqlBatchEventsEnabled, sqlStatementEventsEnabled, storedProcedureEventsEnabled, durationFilter, cpuUsageFilter,
            logicalDiskReads, physicalDiskWrites, traceFileSize, traceFileRollovers, recordsPerRefresh, advancedConfiguration, null, traceMonitoringEnabled, collectQueryPlan, collectEstimatedQueryPlan, 
            topPlanCount,
            topPlanCountCategory,
            queryStoreMonitoringEnabled)
        {

        }

        public QueryMonitorConfiguration(bool enabled, bool sqlBatchEventsEnabled, bool sqlStatementEventsEnabled,
                                         bool storedProcedureEventsEnabled, TimeSpan durationFilter,
                                         TimeSpan cpuUsageFilter, int logicalDiskReads, int physicalDiskWrites,
                                         FileSize traceFileSize, int traceFileRollovers, int recordsPerRefresh,
                                         AdvancedQueryMonitorConfiguration advancedConfiguration, DateTime? stopTimeUTC, bool traceMonitoringEnabled, bool collectQueryPlan, bool collectEstimatedQueryPlan = true,  //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- modifed the Constructor with two new parameters
                                         int topPlanCount = TopPlanCountDefault,
                                         int topPlanCategory = TopPlanCategoryDefault,
                                         bool queryStoreMonitoringEnabled = QueryStoreMonitoringEnabledDefault)
        //SQLdm 10.0 - Display Estimated Query Plan - Added a flag for collecting estimated query plan only
        {
            this.enabled = enabled;
            // SQLDM-29633: Use Extended Events for disabled Query Monitor
            if (!enabled && queryStoreMonitoringEnabled)
            {
                this.queryStoreMonitoringEnabled = this.traceMonitoringEnabled = false;
            }
            else
            {
                //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- assigned the field values 
                this.queryStoreMonitoringEnabled = queryStoreMonitoringEnabled;
                // SQLdm 10.4 (Varun Chopra) Enable Query Monitor using Query Store
                this.traceMonitoringEnabled = traceMonitoringEnabled;
            }

            this.sqlBatchEventsEnabled = sqlBatchEventsEnabled;
            this.sqlStatementEventsEnabled = sqlStatementEventsEnabled;
            this.storedProcedureEventsEnabled = storedProcedureEventsEnabled;
            this.durationFilter = durationFilter;
            this.topPlanCountFilter = topPlanCount;
            this.topPlanCategoryFilter = topPlanCategory;
            this.cpuUsageFilter = cpuUsageFilter;
            this.logicalDiskReads = logicalDiskReads;
            this.physicalDiskWrites = physicalDiskWrites;
            this.traceFileSize = traceFileSize;
            this.traceFileRollovers = traceFileRollovers;
            this.recordsPerRefresh = recordsPerRefresh;
            this.advancedConfiguration = advancedConfiguration;
            this.stopTimeUTC = stopTimeUTC;
            this.collectQueryPlan = collectQueryPlan;
            this.collectEstimatedQueryPlan = collectEstimatedQueryPlan;
        }

        //START SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new properties for uery monitoring extended event session configuration
        /// <summary>
        /// Size of the XE output file
        /// </summary>
        [Auditable(false)]
        public int FileSizeXeMB
        {
            get { return fileSizeXEMB; }
            set { fileSizeXEMB = value; }
        }
        /// <summary>
        /// Number of XE files to allow before rollover
        /// </summary>
        [Auditable(false)]
        public int FileSizeRolloverXe
        {
            get { return fileSizeRolloverXE; }
            set { fileSizeRolloverXE = value; }
        }
        //END SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - adding new properties for uery monitoring extended event session configuration

        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session - Added new property whether the query Monitoring is enabled via trace
        [Auditable("Query Monitoring by trace enabled")]
        public bool TraceMonitoringEnabled
        {
            get { return traceMonitoringEnabled; }
            set { traceMonitoringEnabled = value; }
        }

        /// <summary>
        /// Enable Query Monitor using Query Store
        /// </summary>
        [Auditable("Query Monitoring by Query Store enabled")]
        public bool QueryStoreMonitoringEnabled
        {
            get { return this.queryStoreMonitoringEnabled; }
            set { this.queryStoreMonitoringEnabled = value; }
        }

        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session - Added new property whether to collect the query plans or not
        [Auditable("to collect actual query plan or not ")]
        public bool CollectQueryPlan
        {
            get { return collectQueryPlan; }
            set { collectQueryPlan = value; }
        }
        
        [Auditable("to collect estimated query plan or not ")]
        public bool CollectEstimatedQueryPlan
        {
            get { return collectEstimatedQueryPlan; }
            set { collectEstimatedQueryPlan = value; }
        }

        [Auditable("Enabled Query Monitor Configuration")]
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        [Auditable("Enabled capturing of SQL batches")]
        public bool SqlBatchEventsEnabled
        {
            get { return sqlBatchEventsEnabled; }
        }

        [Auditable("Enabled capturing of SQL statements")]
        public bool SqlStatementEventsEnabled
        {
            get { return sqlStatementEventsEnabled; }
        }

        [Auditable("Enabled capturing of stored procedures and triggers")]
        public bool StoredProcedureEventsEnabled
        {
            get { return storedProcedureEventsEnabled; }
        }

        [Auditable("Changed Duration threshold to (milliseconds)", false)]
        public TimeSpan DurationFilter
        {
            get { return durationFilter; }
        }

        [Auditable("Top Query Plan Count", false)]
        public int TopPlanCountFilter
        {
            get { return topPlanCountFilter; }
        }

        [Auditable("Top Query Plan Category", false)]
        public int TopPlanCategoryFilter
        {
            get { return topPlanCategoryFilter; }
        }

        [Auditable("Changed CPU Usage threshold to (milliseconds)", false)]
        public TimeSpan CpuUsageFilter
        {
            get { return cpuUsageFilter; }
        }

        [Auditable("Changed Logical disk reads threshold to")]
        public int LogicalDiskReads
        {
            get { return logicalDiskReads; }
        }

        [Auditable("Changed Physical disk writes threshold to")]
        public int PhysicalDiskWrites
        {
            get { return physicalDiskWrites; }
        }

        public FileSize TraceFileSize
        {
            get { return traceFileSize; }
        }

        public int TraceFileRollovers
        {
            get { return traceFileRollovers; }
        }

        public int RecordsPerRefresh
        {
            get { return recordsPerRefresh; }
        }

        [Auditable(false)]
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

        public DateTime? StopTimeUTC
        {
            get { return stopTimeUTC; }
            set { stopTimeUTC = value; }
        }

        [Auditable(false)]
        public bool IsAlertResponseQueryTrace { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            QueryMonitorConfiguration queryMonitorConfiguration = obj as QueryMonitorConfiguration;
            if (queryMonitorConfiguration == null) return false;
            if (!Equals(enabled, queryMonitorConfiguration.enabled)) return false;
            if (!Equals(sqlBatchEventsEnabled, queryMonitorConfiguration.sqlBatchEventsEnabled)) return false;
            if (!Equals(sqlStatementEventsEnabled, queryMonitorConfiguration.sqlStatementEventsEnabled)) return false;
            if (!Equals(storedProcedureEventsEnabled, queryMonitorConfiguration.storedProcedureEventsEnabled)) return false;
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
            if (!Equals(stopTimeUTC, stopTimeUTC)) return false;
            //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  -- Added equals check for newly added fields
            if (traceMonitoringEnabled != queryMonitorConfiguration.traceMonitoringEnabled) return false;
            // SQLdm 10.4 (Varun Chopra) Enable Query Monitor using Query Store
            if (queryStoreMonitoringEnabled != queryMonitorConfiguration.queryStoreMonitoringEnabled)
            {
                return false;
            }
            if (collectQueryPlan != queryMonitorConfiguration.collectQueryPlan) return false;
            if (collectEstimatedQueryPlan != queryMonitorConfiguration.collectEstimatedQueryPlan) return false;
            //START SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - added equals check for newly added fields
            if (fileSizeXEMB != queryMonitorConfiguration.fileSizeXEMB) return false;
            if (fileSizeRolloverXE != queryMonitorConfiguration.fileSizeRolloverXE) return false;
            //END SQLdm 9.1 (Ankit Srivastava): Query Monitor 9.0 Improvement feedback - added equals check for newly added fields
            return true;
        }

        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  -- adding new method for checking if if Query Monitoring technique changed.
        public bool IsTraceMonitoringTechniqueChanged(bool prevTraceMonitoringEnabled)
        {
            return (traceMonitoringEnabled != prevTraceMonitoringEnabled);
        }

        #region Formatting Methods

        // These properties and methods are used to return string data for display
        [Auditable(false)]
        public string EnabledString
        {
            get
            {
                return enabled ? "Enabled" : "Disabled";
            }
        }

        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  --  Added two new properties for ToString Conversion
        [Auditable(false)]
        public string TraceMonitoringEnabledString
        {
            get
            {
                // SQLdm 10.4 (Varun Chopra) Enable Query Monitor using Query Store
                return this.traceMonitoringEnabled
                           ? "Monitoring by Trace"
                           : this.queryStoreMonitoringEnabled
                               ? "Monitoring by Query Store"
                               : "Monitoring by Extended events";
            }
        }

        [Auditable(false)]
        public string CollectActualQueryPlanString
        {
            get
            {
                return collectQueryPlan ? "Enabled" : "Disabled";
            }
        }

        [Auditable(false)]
        public string CollectEstimatedQueryPlanString
        {
            get
            {
                return collectEstimatedQueryPlan ? "Enabled" : "Disabled";
            }
        }

        [Auditable(false)]
        public string CollectionString
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(sqlBatchEventsEnabled ? "Batches, " : "");
                sb.Append(sqlStatementEventsEnabled ? "Statements, " : "");
                sb.Append(storedProcedureEventsEnabled ? "Stored Procedures, " : "");

                if (sb.Length > 2)
                    sb.Remove(sb.Length - 2, 2);

                return sb.ToString();
            }
        }

        [Auditable(false)]
        public string FilterTypeString
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(durationFilter.TotalMilliseconds > 0 ? durationFilter.TotalMilliseconds + "ms duration, " : "");
                sb.Append(cpuUsageFilter.TotalMilliseconds > 0 ? cpuUsageFilter.TotalMilliseconds + "ms cpu, " : "");
                sb.Append(logicalDiskReads > 0 ? logicalDiskReads + "reads, " : "");
                sb.Append(physicalDiskWrites > 0 ? physicalDiskWrites + "writes, " : "");

                sb.Append(this.topPlanCountFilter > 0 ? this.topPlanCountFilter + " top plans count, " : "");
                sb.Append(this.topPlanCategoryFilter >= 0 ? this.topPlanCategoryFilter + " top plans category, " : "");

                if (sb.Length > 2)
                    sb.Remove(sb.Length - 2, 2);

                return sb.ToString();
            }
        }

        [Auditable(false)]
        public string AdvancedFilterString
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (advancedConfiguration != null)
                {
                    if (advancedConfiguration.ApplicationExcludeToString().Length > 0)
                    {
                        sb.Append("Application: ");
                        sb.Append(advancedConfiguration.ApplicationExcludeToString());
                    }
                    if (advancedConfiguration.DatabaseExcludeToString().Length > 0)
                    {
                        if (sb.Length > 0)
                            sb.Append(" ");
                        sb.Append("Database: ");
                        sb.Append(advancedConfiguration.DatabaseExcludeToString());
                    }
                    if (advancedConfiguration.SqlTextExcludeToString().Length > 0)
                    {
                        if (sb.Length > 0)
                            sb.Append(" ");
                        sb.Append("SQL: ");
                        sb.Append(advancedConfiguration.SqlTextExcludeToString());
                    }
                }
                if (advancedConfiguration != null && advancedConfiguration.ExcludeDM)
                {
                    if (sb.Length > 0)
                        sb.Append(" ");
                    sb.Append("Exclude SQLdm");
                }
                if (sb.Length > 0)
                    return sb.ToString();

                return "None";
            }
        }

        [Auditable("Application excluded from query monitoring")]
        public string ExcludeApplications
        {
            get { return advancedConfiguration.ApplicationExcludeToString(); }
        }

        [Auditable("Database excluded from query monitoring")]
        public string ExcludeDatabase
        {
            get { return advancedConfiguration.DatabaseExcludeToString(); }
        }

        [Auditable("SQL text excluded from query monitoring")]
        public string ExcludeSqlText
        {
            get { return advancedConfiguration.SqlTextExcludeToString(); }
        }

        [Auditable("Excluded SQLdm queries from the results")]
        public bool ExcludeSQLdmQueries
        {
            get
            {
                if (advancedConfiguration != null)
                {
                    return advancedConfiguration.ExcludeDM;
                }
                return false;
            }
        }

        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(EnabledString);
            if (enabled)
            {
                sb.Append(" - ");

                //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session   -- Appended ToString() conversions of newly added fields
                sb.Append(TraceMonitoringEnabledString);
                sb.Append(" - ");
                sb.Append("Query Plan Collection: ");
                sb.Append(CollectActualQueryPlanString);
                sb.Append(" - ");

                sb.Append("Collection: ");
                sb.Append(CollectionString);

                sb.Append(" Filters: ");

                string filterString = FilterTypeString;

                if (filterString.Length > 0)
                {
                    sb.Append(filterString);
                }
                else
                {
                    sb.Append("(none)");
                }

            }

            return sb.ToString();
        }

        #endregion


    }
}