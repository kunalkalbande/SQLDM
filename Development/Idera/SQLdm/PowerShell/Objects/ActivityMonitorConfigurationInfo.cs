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
    using Idera.SQLdm.Common.Configuration;

    public class ActivityMonitorConfigurationInfo : ICloneable
    {
        private bool enabled;
        private FileSize traceFileSize = new FileSize(1024);
        private int traceFileRollovers;
        private int recordsPerRefresh;
        private bool deadlockEventsEnabled;
        private bool autogrowEventsEnabled;
        private bool blockingEventsEnabled;
        private int blockedProcessThreshold = 30;
        private AdvancedQueryMonitorConfiguration advancedConfiguration;
        private bool traceMonitoringEnabled;//SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events -- added new field


        public ActivityMonitorConfigurationInfo(ActivityMonitorConfiguration config)
        {
            enabled = config.Enabled;
            traceFileSize = config.TraceFileSize;
            traceFileRollovers = config.TraceFileRollovers;
            recordsPerRefresh = config.RecordsPerRefresh;
            deadlockEventsEnabled = config.DeadlockEventsEnabled;
            autogrowEventsEnabled = config.AutoGrowEventsEnabled;
            blockingEventsEnabled = config.BlockingEventsEnabled;
            blockedProcessThreshold = config.BlockedProcessThreshold;
            string xml = AdvancedQueryMonitorConfiguration.SerializeToXml(config.AdvancedConfiguration);
            advancedConfiguration = AdvancedQueryMonitorConfiguration.DeserializeFromXml(xml);
            traceMonitoringEnabled = config.TraceMonitoringEnabled;//SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events -- assign value to new field
        }

        public ActivityMonitorConfigurationInfo()
        {
            enabled = true;
            deadlockEventsEnabled = true;
            blockingEventsEnabled = true;
            autogrowEventsEnabled = true;
            blockedProcessThreshold = 30;
            traceFileRollovers = 2;
            recordsPerRefresh = 1000;
            advancedConfiguration = new AdvancedQueryMonitorConfiguration(true);
            traceMonitoringEnabled = true;
        }

        public ActivityMonitorConfigurationInfo(ActivityMonitorConfigurationInfo copy)
        {
            enabled = copy.Enabled;
            traceFileSize = copy.TraceFileSize;
            traceFileRollovers = copy.TraceFileRollovers;
            recordsPerRefresh = copy.RecordsPerRefresh;
            deadlockEventsEnabled = copy.deadlockEventsEnabled;
            blockingEventsEnabled = copy.blockingEventsEnabled;
            autogrowEventsEnabled = copy.autogrowEventsEnabled;
            blockedProcessThreshold = copy.blockedProcessThreshold;
            traceMonitoringEnabled = copy.traceMonitoringEnabled;//SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events -- assign value to new field

            AdvancedQueryMonitorConfiguration copyac = copy.AdvancedConfiguration;
            advancedConfiguration = new AdvancedQueryMonitorConfiguration(false);
            advancedConfiguration.ApplicationExcludeLike = copyac.ApplicationExcludeLike;
            advancedConfiguration.ApplicationExcludeMatch = copyac.ApplicationExcludeMatch;
            advancedConfiguration.DatabaseExcludeLike = copyac.DatabaseExcludeLike;
            advancedConfiguration.DatabaseExcludeMatch = copyac.DatabaseExcludeMatch;
            advancedConfiguration.SqlTextExcludeLike = copyac.SqlTextExcludeLike;
            advancedConfiguration.SqlTextExcludeMatch = copyac.SqlTextExcludeMatch;
        }

        internal ActivityMonitorConfiguration GetInternalConfiguration()
        {
            return
                new ActivityMonitorConfiguration(enabled, deadlockEventsEnabled, blockingEventsEnabled,
                    autogrowEventsEnabled, blockedProcessThreshold, traceFileSize, TraceFileRollovers,
                                              recordsPerRefresh, advancedConfiguration, traceMonitoringEnabled);//SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events -- added new param
        }


        public bool Enabled
        {
            get { return enabled; }
            internal set { enabled = value; }
        }

        //START SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events -- added new property
        public bool TraceMonitoringEnabled
        {
            get { return traceMonitoringEnabled; }
            set { traceMonitoringEnabled = value; }
        }
        //END SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events -- added new property

        public bool DeadlockEventsEnabled
        {
            get { return deadlockEventsEnabled; }
            internal set { deadlockEventsEnabled = value; }
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
        public bool AutogrowEventsEnabled
        {
            get
            {
                return autogrowEventsEnabled;
            }

            internal set
            {
                autogrowEventsEnabled = value;
            }
        }
        public bool BlockingEventsEnabled
        {
            get
            {
                return blockingEventsEnabled;
            }

            internal set
            {
                blockingEventsEnabled = value;
            }
        }
        public int BlockedProcessThreshold
        {
            get
            {
                return blockedProcessThreshold;
            }

            internal set
            {
                blockedProcessThreshold = value;
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
            ActivityMonitorConfigurationInfo activityProfilerConfigurationinfo = obj as ActivityMonitorConfigurationInfo;
            if (activityProfilerConfigurationinfo == null) return false;
            if (!Equals(enabled, activityProfilerConfigurationinfo.enabled)) return false;
            if (!Equals(deadlockEventsEnabled, activityProfilerConfigurationinfo.deadlockEventsEnabled)) return false;
            if (!Equals(autogrowEventsEnabled, activityProfilerConfigurationinfo.autogrowEventsEnabled)) return false;
            if (!Equals(blockingEventsEnabled, activityProfilerConfigurationinfo.blockingEventsEnabled)) return false;
            if (!Equals(traceFileSize, activityProfilerConfigurationinfo.traceFileSize)) return false;
            if (traceFileRollovers != activityProfilerConfigurationinfo.traceFileRollovers) return false;
            if (recordsPerRefresh != activityProfilerConfigurationinfo.recordsPerRefresh) return false;
            if (!Equals(advancedConfiguration, activityProfilerConfigurationinfo.advancedConfiguration)) return false;
            if (!Equals(traceMonitoringEnabled, activityProfilerConfigurationinfo.traceMonitoringEnabled)) return false; //START SQLdm 9.1 (Ankit Srivastava): Activity Monitoring with Extended Events -- compare new field as well
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
            return new ActivityMonitorConfigurationInfo(this);
        }

        #endregion
    }
}
