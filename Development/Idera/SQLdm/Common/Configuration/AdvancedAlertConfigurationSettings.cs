//------------------------------------------------------------------------------
// <copyright file="AdvancedAlertConfigurationSettings.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Text;
using Idera.SQLdm.Common.Events;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.Common.Configuration
{
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Idera.SQLdm.Common.Status;
    using Objects;
    using Snapshots;
    using Thresholds;

    [Serializable]
    public class AdvancedAlertConfigurationSettings : ISerializable
    {
        [Serializable]
        private enum FilterType
        {
            DatabaseMatch,
            DatabaseLike,
            TableMatch,
            TableLike,
            JobCategoryMatch,
            JobCategoryLike,
            JobNameMatch,
            JobNameLike,
            AppNameMatch,
            AppNameLike,
            HostMatch,
            HostLike,
            UserMatch,
            UserLike,
            LogMatchCritical,
            LogLikeCritical,
            LogRegexCritical,
            LogMatchWarning,
            LogLikeWarning,
            LogRegexWarning,
            LogSizeLimit,
            DriveExcludeMatch,
            LogMatchInfo,
            LogLikeInfo,
            LogRegexInfo,

            //SQLdm 8.5 (Ankit Srivastava): for Inclusion filters
            DatabaseMatchInclude,
            DatabaseLikeInclude,
            AppNameMatchInclude,
            AppNameLikeInclude,

            //SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --apply the filegroup filter changes
            FilegroupMatch,
            FilegroupLike
        }

        private readonly static string[] EmptyStringArray = new string[0];

        private Metric metric;
        private int suppressionPeriodMinutes;
       
        private int thresholdTime;
        private int thresholdRefreshTime;
        private SnoozeInfo snoozeInfo;
        private bool treatCustomCounterFailureAsCritical;
        private bool treatSingleJobFailureAsCritical = true;
        private bool includeAutogrowInDatabaseSizeCalc = true;
        private int numberMinutesJobAlertActive = 720;
        private int numberMinutesAgingAlertActive = 720;
        //SQLDM-29210
        private bool includeReadOnly = true;

        //SQLDM-29041 -- Add availability group alert options.
        private String availabilityGroupOption = String.Empty;
        private String replicaCollectionOption = String.Empty;

        //private int numberMinutesFailoverAlertActive = 720;
        private readonly Dictionary<FilterType, string[]> stringFilters;
        private bool alertOnJobSteps = false;
        private List<JobFilter> jobIncludeFilter;
        private List<JobFilter> jobExcludeFilter;
        private int minGenerationRatePerSecond = 10;
        private FileSize minVersionStoreSize = new FileSize(1024);
        private int minSecondsRuntimeRunningJobByPercent = 5;
        private bool excludeSqldmApp = false;
        private long logSizeLimit = 10240;
        private string replicaInstanceName = String.Empty; //SQLdm 8.6 (Ankit Srivastava) -- Preferred Node Feature -Saving replica instance name

        public AdvancedAlertConfigurationSettings()
        {
            stringFilters = new Dictionary<FilterType, string[]>();
            jobExcludeFilter = new List<JobFilter>();
            jobIncludeFilter = new List<JobFilter>();
        }

        public AdvancedAlertConfigurationSettings(Metric metric, object setting)
        {
            this.metric = metric;
            if (setting is AdvancedAlertConfigurationSettings)
            {   // shallow copy
                snoozeInfo = ((AdvancedAlertConfigurationSettings)setting).snoozeInfo;
                stringFilters = ((AdvancedAlertConfigurationSettings)setting).stringFilters;
                JobIncludeFilter = ((AdvancedAlertConfigurationSettings)setting).JobIncludeFilter;
                JobExcludeFilter = ((AdvancedAlertConfigurationSettings)setting).JobExcludeFilter;
                ReplicaInstanceName = ((AdvancedAlertConfigurationSettings)setting).ReplicaInstanceName; //SQLdm 8.6 (Ankit Srivastava) -- Preferred Node Feature -Saving replica instance name
                
            }
            else
            {
                stringFilters = new Dictionary<FilterType, string[]>();

                jobExcludeFilter = new List<JobFilter>();
                jobIncludeFilter = new List<JobFilter>();

                switch (metric)
                {
                    case Metric.Custom:
                        if (setting is bool)
                            treatCustomCounterFailureAsCritical = (bool)setting;
                        break;
                    case Metric.BombedJobs:
                        if (setting is bool)
                            treatSingleJobFailureAsCritical = (bool)setting;
                        break;
                }
            }
        }

        public AdvancedAlertConfigurationSettings(SerializationInfo info, StreamingContext context)
        {
            stringFilters = new Dictionary<FilterType, string[]>();
            metric = (Metric)info.GetValue("metric", typeof(Metric));
            suppressionPeriodMinutes = info.GetInt32("suppressionPeriodMinutes");
            //SQLDM-30444
           // alertFilterIndex = info.GetString("alertFilterIndex");

            thresholdTime= info.GetInt32("thresholdTime");
            thresholdRefreshTime = info.GetInt32("thresholdRefreshTime");
            replicaInstanceName = info.GetString("replicaInstanceName"); //SQLdm 8.6 (Ankit Srivastava) -- Preferred Node Feature -Saving replica instance name
            snoozeInfo = (SnoozeInfo)info.GetValue("snoozeInfo", typeof(SnoozeInfo));
            treatCustomCounterFailureAsCritical = info.GetBoolean("treatCustomCounterFailureAsCritical");
            treatSingleJobFailureAsCritical = info.GetBoolean("treatSingleJobFailureAsCritical");
            includeAutogrowInDatabaseSizeCalc = info.GetBoolean("includeAutogrowInDatabaseSizeCalc");

            //SQLDM-29210 -- Add option to include alerts for Read-only databases.
            includeReadOnly = info.GetBoolean("includeReadOnly");

            //SQLDM-29041 -- Add availability group alert options.
            availabilityGroupOption = info.GetString("availabilityGroupOption");
            replicaCollectionOption = info.GetString("replicaCollectionOption");

            numberMinutesJobAlertActive = info.GetInt32("numberMinutesJobAlertActive");
            numberMinutesAgingAlertActive = info.GetInt32("numberMinutesAgingAlertActive");
            DatabaseExcludeMatch = (string[])info.GetValue("DatabaseExcludeMatch", typeof(string[]));
            DatabaseExcludeLike = (string[])info.GetValue("DatabaseExcludeLike", typeof(string[]));

            //SQLdm 8.5 (Ankit Srivastava): for Inclusion filters
            DatabaseIncludeMatch = (string[])info.GetValue("DatabaseIncludeMatch", typeof(string[]));
            DatabaseIncludeLike = (string[])info.GetValue("DatabaseIncludeLike", typeof(string[]));

            //SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --apply the filegroup filter changes
            FilegroupExcludeMatch = (string[])info.GetValue("FilegroupExcludeMatch", typeof(string[]));
            FilegroupExcludeLike = (string[])info.GetValue("FilegroupExcludeLike", typeof(string[]));

            TableExcludeMatch = (string[])info.GetValue("TableExcludeMatch", typeof(string[]));
            TableExcludeLike = (string[])info.GetValue("TableExcludeLike", typeof(string[]));
            JobNameExcludeMatch = (string[])info.GetValue("JobNameExcludeMatch", typeof(string[]));
            JobNameExcludeLike = (string[])info.GetValue("JobNameExcludeLike", typeof(string[]));
            JobCategoryExcludeMatch = (string[])info.GetValue("JobCategoryExcludeMatch", typeof(string[]));
            JobCategoryExcludeLike = (string[])info.GetValue("JobCategoryExcludeLike", typeof(string[]));
            AppNameExcludeMatch = (string[])info.GetValue("AppNameExcludeMatch", typeof(string[]));
            AppNameExcludeLike = (string[])info.GetValue("AppNameExcludeLike", typeof(string[]));

            //SQLdm 8.5 (Ankit Srivastava): for Inclusion filters
            AppNameIncludeMatch = (string[])info.GetValue("AppNameIncludeMatch", typeof(string[]));
            AppNameIncludeLike = (string[])info.GetValue("AppNameIncludeLike", typeof(string[]));

            HostExcludeMatch = (string[])info.GetValue("HostExcludeMatch", typeof(string[]));
            HostExcludeLike = (string[])info.GetValue("HostExcludeLike", typeof(string[]));
            UserExcludeMatch = (string[])info.GetValue("UserExcludeMatch", typeof(string[]));
            UserExcludeLike = (string[])info.GetValue("UserExcludeLike", typeof(string[]));
            LogIncludeMatchCritical = (string[])info.GetValue("LogIncludeMatchCritical", typeof(string[]));
            LogIncludeLikeCritical = (string[])info.GetValue("LogIncludeLikeCritical", typeof(string[]));
            LogIncludeRegexCritical = (string[])info.GetValue("LogIncludeRegexCritical", typeof(string[]));
            LogIncludeMatchWarning = (string[])info.GetValue("LogIncludeMatchWarning", typeof(string[]));
            LogIncludeLikeWarning = (string[])info.GetValue("LogIncludeLikeWarning", typeof(string[]));
            LogIncludeRegexWarning = (string[])info.GetValue("LogIncludeRegexWarning", typeof(string[]));
            LogIncludeMatchInfo = (string[])info.GetValue("LogIncludeMatchInfo", typeof(string[]));
            LogIncludeLikeInfo = (string[])info.GetValue("LogIncludeLikeInfo", typeof(string[]));
            LogIncludeRegexInfo = (string[])info.GetValue("LogIncludeRegexInfo", typeof(string[]));
            DrivesExcludedMatch = (string[])info.GetValue("DrivesExcludedMatch", typeof(string[]));
            jobIncludeFilter = (List<JobFilter>)info.GetValue("jobIncludeFilter", typeof(List<JobFilter>));
            jobExcludeFilter = (List<JobFilter>)info.GetValue("jobExcludeFilter", typeof(List<JobFilter>));
            alertOnJobSteps = info.GetBoolean("alertOnJobSteps");
            minGenerationRatePerSecond = info.GetInt32("minGenerationRatePerSecond");
            minVersionStoreSize = (FileSize)info.GetValue("minVersionStoreSize", typeof(FileSize));
            minSecondsRuntimeRunningJobByPercent = info.GetInt32("minSecondsRuntimeRunningJobByPercent");
            excludeSqldmApp = info.GetBoolean("excludeSqldmApp");
            logSizeLimit = info.GetInt32("logSizeLimit");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            

            info.AddValue("metric", metric);
            info.AddValue("suppressionPeriodMinutes", suppressionPeriodMinutes);

            info.AddValue("thresholdTime", thresholdTime);
            info.AddValue("thresholdRefreshTime", thresholdRefreshTime);
            info.AddValue("replicaInstanceName", replicaInstanceName); //SQLdm 8.6 (Ankit Srivastava) -- Preferred Node Feature -Saving replica instance name
            info.AddValue("snoozeInfo", snoozeInfo);
            info.AddValue("treatCustomCounterFailureAsCritical", treatCustomCounterFailureAsCritical);
            info.AddValue("treatSingleJobFailureAsCritical", treatSingleJobFailureAsCritical);
            info.AddValue("includeAutogrowInDatabaseSizeCalc", includeAutogrowInDatabaseSizeCalc);
            //SQLDM-29210 -- Add option to include alerts for Read-only databases.
            info.AddValue("includeReadOnly", includeReadOnly);
            //SQLDM-29041 -- Add availability group alert options.
            info.AddValue("availabilityGroupOption", availabilityGroupOption);
            info.AddValue("replicaCollectionOption", replicaCollectionOption);

            info.AddValue("numberMinutesJobAlertActive", numberMinutesJobAlertActive);
            info.AddValue("numberMinutesAgingAlertActive", numberMinutesAgingAlertActive);
            info.AddValue("DatabaseExcludeMatch", DatabaseExcludeMatch);
            info.AddValue("DatabaseExcludeLike", DatabaseExcludeLike);

            //SQLdm 8.5 (Ankit Srivastava): for Inclusion filters
            info.AddValue("DatabaseIncludeMatch", DatabaseIncludeMatch);
            info.AddValue("DatabaseIncludeLike", DatabaseIncludeLike);

            info.AddValue("TableExcludeMatch", TableExcludeMatch);
            info.AddValue("TableExcludeLike", TableExcludeLike);
            info.AddValue("JobNameExcludeMatch", JobNameExcludeMatch);
            info.AddValue("JobNameExcludeLike", JobNameExcludeLike);
            info.AddValue("JobCategoryExcludeMatch", JobCategoryExcludeMatch);
            info.AddValue("JobCategoryExcludeLike", JobCategoryExcludeLike);
            info.AddValue("AppNameExcludeMatch", AppNameExcludeMatch);
            info.AddValue("AppNameExcludeLike", AppNameExcludeLike);

            //SQLdm 8.5 (Ankit Srivastava): for Inclusion filters
            info.AddValue("AppNameIncludeMatch", AppNameIncludeMatch);
            info.AddValue("AppNameIncludeLike", AppNameIncludeLike);

            info.AddValue("HostExcludeMatch", HostExcludeMatch);
            info.AddValue("HostExcludeLike", HostExcludeLike);
            info.AddValue("UserExcludeMatch", UserExcludeMatch);
            info.AddValue("UserExcludeLike", UserExcludeLike);
            info.AddValue("LogIncludeMatchCritical", LogIncludeMatchCritical);
            info.AddValue("LogIncludeLikeCritical", LogIncludeLikeCritical);
            info.AddValue("LogIncludeRegexCritical", LogIncludeRegexCritical);
            info.AddValue("LogIncludeMatchWarning", LogIncludeMatchWarning);
            info.AddValue("LogIncludeLikeWarning", LogIncludeLikeWarning);
            info.AddValue("LogIncludeRegexWarning", LogIncludeRegexWarning);
            info.AddValue("LogIncludeMatchInfo", LogIncludeMatchInfo);
            info.AddValue("LogIncludeLikeInfo", LogIncludeLikeInfo);
            info.AddValue("LogIncludeRegexInfo", LogIncludeRegexInfo);
            info.AddValue("DrivesExcludedMatch", DrivesExcludedMatch);
            info.AddValue("jobIncludeFilter", jobIncludeFilter);
            info.AddValue("jobExcludeFilter", jobExcludeFilter);
            info.AddValue("alertOnJobSteps", alertOnJobSteps);
            info.AddValue("minGenerationRatePerSecond", minGenerationRatePerSecond);
            info.AddValue("minVersionStoreSize", minVersionStoreSize);
            info.AddValue("minSecondsRuntimeRunningJobByPercent", minSecondsRuntimeRunningJobByPercent);
            info.AddValue("excludeSqldmApp", excludeSqldmApp);
            info.AddValue("logSizeLimit", logSizeLimit);

            //SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --apply the filegroup filter changes
            info.AddValue("FilegroupExcludeMatch", FilegroupExcludeMatch);
            info.AddValue("FilegroupExcludeLike", FilegroupExcludeLike);
        }

        /// <summary>
        /// Log  size limit in KB
        /// </summary>
        public long LogSizeLimit
        {
            get { return logSizeLimit; }
            set { logSizeLimit = value; }
        }

        public bool ExcludeSqldmApp
        {
            get { return excludeSqldmApp; }
            set { excludeSqldmApp = value; }
        }

        public int MinSecondsRuntimeRunningJobByPercent
        {
            get { return minSecondsRuntimeRunningJobByPercent; }
            set { minSecondsRuntimeRunningJobByPercent = value; }
        }

        public int MinGenerationRatePerSecond
        {
            get { return minGenerationRatePerSecond; }
            set { minGenerationRatePerSecond = value; }
        }

        public FileSize MinVersionStoreSize
        {
            get { return minVersionStoreSize; }
            set { minVersionStoreSize = value; }
        }

        public bool IncludeAutogrowInDatabaseSizeCalc
        {
            get { return includeAutogrowInDatabaseSizeCalc; }
            set { includeAutogrowInDatabaseSizeCalc = value; }
        }

        //SQLDM-29210 -- Add option to include alerts for Read-only databases.
        public bool IncludeReadOnly
        {
            get { return includeReadOnly; }
            set { includeReadOnly = value; }
        }

        //SQLDM-29041 -- Add availability group alert options.
        public String AvailabilityGroupOption
        {
            get { return availabilityGroupOption; }
            set { availabilityGroupOption = value; }
        }

        public String ReplicaCollectionOption
        {
            get { return replicaCollectionOption; }
            set { replicaCollectionOption = value; }
        }

        public Metric Metric
        {
            get { return metric; }
            set { metric = value; }
        }

        public int NumberMinutesJobAlertsAreActive
        {
            get { return numberMinutesJobAlertActive; }
            set { numberMinutesJobAlertActive = value; }
        }

        //Used for cluster failover and mirroring role change
        public int NumberMinutesAgingAlertsAreActive
        {
            get { return numberMinutesAgingAlertActive; }
            set { numberMinutesAgingAlertActive = value; }
        }

        [XmlIgnore]
        public SnoozeInfo SnoozeInfo
        {
            get { return snoozeInfo; }
            set { snoozeInfo = value; }
        }

        public int SuppressionPeriodMinutes
        {
            get { return suppressionPeriodMinutes; }
            set { suppressionPeriodMinutes = value; }
        }

        public int ThresholdTime
        {
            get { return thresholdTime; }
            set { thresholdTime = value; }
        }
        public int ThresholdRefreshTime
        {
            get { return thresholdRefreshTime; }
            set { thresholdRefreshTime = value; }
        }

        //public bool IsBaseLineEnabled
        //{
        //    get { return isBaseLineEnabled; }
        //    set { isBaseLineEnabled = value; }
        //}
        //SQLdm 8.6 (Ankit Srivastava) -- Preferred Node Feature -Saving replica instance name
        public string ReplicaInstanceName 
        {
            get { return replicaInstanceName; }
            set { replicaInstanceName = value; }
        }


        public bool AlertOnJobSteps
        {
            get { return alertOnJobSteps; }
            set { alertOnJobSteps = value; }
        }

        private void SetStringArrayValue(FilterType filterType, IEnumerable value)
        {
            if (value is string[])
            {
                SetStringArrayValue(filterType, (string[])value);
            }
            else
            {
                List<string> temp = new List<string>();
                foreach (object o in value)
                {
                    temp.Add(o.ToString());
                    // Pretty sure this doesn't look right so I changed to the line above
                    //temp.Add(value.ToString());
                }
                SetStringArrayValue(filterType, temp.ToArray());
            }
        }

        private void SetStringArrayValue(FilterType filterType, string[] value)
        {
            if (value == null || value.Length == 0)
            {
                if (stringFilters.ContainsKey(filterType))
                    stringFilters.Remove(filterType);
            }
            else
            {
                if (stringFilters.ContainsKey(filterType))
                    stringFilters.Remove(filterType);
                stringFilters.Add(filterType, value);
            }
        }

        [XmlArrayItem("Match")]
        public string[] DatabaseExcludeMatch
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.DatabaseMatch, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.DatabaseMatch, value);
            }
        }

        //SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --apply the filegroup filter changes
        [XmlArrayItem("Match")]
        public string[] FilegroupExcludeMatch
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.FilegroupMatch, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.FilegroupMatch, value);
            }
        }

        //SQLdm 8.5 (Ankit Srivastava): for Inclusion filters
        [XmlArrayItem("Match")]
        public string[] DatabaseIncludeMatch
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.DatabaseMatchInclude, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.DatabaseMatchInclude, value);
            }
        }
        //SQLdm 8.5 (Ankit Srivastava): for Inclusion filters

        [XmlArrayItem("Like")]
        public string[] DatabaseExcludeLike
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.DatabaseLike, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.DatabaseLike, value);
            }
        }

        //SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --apply the filegroup filter changes
        [XmlArrayItem("Like")]
        public string[] FilegroupExcludeLike
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.FilegroupLike, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.FilegroupLike, value);
            }
        }

        //SQLdm 8.5 (Ankit Srivastava): for Inclusion filters
        [XmlArrayItem("LikeInclude")]
        public string[] DatabaseIncludeLike
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.DatabaseLikeInclude, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.DatabaseLikeInclude, value);
            }
        }
        //SQLdm 8.5 (Ankit Srivastava): for Inclusion filters

        [XmlArrayItem("Match")]
        public string[] TableExcludeMatch
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.TableMatch, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.TableMatch, value);
            }
        }

        [XmlArrayItem("Like")]
        public string[] TableExcludeLike
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.TableLike, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.TableLike, value);
            }
        }

        [XmlArrayItem("Match")]
        public string[] JobNameExcludeMatch
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.JobNameMatch, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.JobNameMatch, value);
            }
        }

        [XmlArrayItem("Like")]
        public string[] JobNameExcludeLike
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.JobNameLike, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.JobNameLike, value);
            }
        }

        [XmlArrayItem("Match")]
        public string[] JobCategoryExcludeMatch
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.JobCategoryMatch, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.JobCategoryMatch, value);
            }
        }

        [XmlArrayItem("Like")]
        public string[] JobCategoryExcludeLike
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.JobCategoryLike, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.JobCategoryLike, value);
            }
        }

        [XmlArrayItem("Match")]
        public string[] AppNameExcludeMatch
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.AppNameMatch, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.AppNameMatch, value);
            }
        }

        //SQLdm 8.5 (Ankit Srivastava): for Inclusion filters
        [XmlArrayItem("Match")]
        public string[] AppNameIncludeMatch
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.AppNameMatchInclude, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.AppNameMatchInclude, value);
            }
        }
        //SQLdm 8.5 (Ankit Srivastava): for Inclusion filters

        [XmlArrayItem("Like")]
        public string[] AppNameExcludeLike
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.AppNameLike, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.AppNameLike, value);
            }
        }

        //SQLdm 8.5 (Ankit Srivastava): for Inclusion filters -starts
        [XmlArrayItem("Like")]
        public string[] AppNameIncludeLike
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.AppNameLikeInclude, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.AppNameLikeInclude, value);
            }
        }
        //SQLdm 8.5 (Ankit Srivastava): for Inclusion filters -ends

        [XmlArrayItem("Match")]
        public string[] HostExcludeMatch
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.HostMatch, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.HostMatch, value);
            }
        }

        [XmlArrayItem("Like")]
        public string[] HostExcludeLike
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.HostLike, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.HostLike, value);
            }
        }

        [XmlArrayItem("Match")]
        public string[] UserExcludeMatch
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.UserMatch, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.UserMatch, value);
            }
        }

        [XmlArrayItem("Like")]
        public string[] UserExcludeLike
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.UserLike, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.UserLike, value);
            }
        }

        [XmlArrayItem("Match")]
        public string[] LogIncludeMatchCritical
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.LogMatchCritical, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.LogMatchCritical, value);
            }
        }

        [XmlArrayItem("Like")]
        public string[] LogIncludeLikeCritical
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.LogLikeCritical, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.LogLikeCritical, value);
            }
        }

        public string[] LogIncludeRegexCritical
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.LogRegexCritical, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.LogRegexCritical, value);
            }
        }

        [XmlArrayItem("Match")]
        public string[] LogIncludeMatchWarning
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.LogMatchWarning, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.LogMatchWarning, value);
            }
        }

        [XmlArrayItem("Like")]
        public string[] LogIncludeLikeWarning
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.LogLikeWarning, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.LogLikeWarning, value);
            }
        }

        public string[] LogIncludeRegexWarning
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.LogRegexWarning, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.LogRegexWarning, value);
            }
        }

        [XmlArrayItem("Match")]
        public string[] LogIncludeMatchInfo
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.LogMatchInfo, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.LogMatchInfo, value);
            }
        }

        [XmlArrayItem("Like")]
        public string[] LogIncludeLikeInfo
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.LogLikeInfo, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.LogLikeInfo, value);
            }
        }

        public string[] LogIncludeRegexInfo
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.LogRegexInfo, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.LogRegexInfo, value);
            }
        }

        [XmlArrayItem("Match")]
        public string[] DrivesExcludedMatch
        {
            get
            {
                string[] result = EmptyStringArray;
                stringFilters.TryGetValue(FilterType.DriveExcludeMatch, out result);
                return result;
            }
            set
            {
                SetStringArrayValue(FilterType.DriveExcludeMatch, value);
            }
        }

        [XmlArrayItem("IncludeJob")]
        public List<JobFilter> JobIncludeFilter
        {
            get { return jobIncludeFilter; }
            set { jobIncludeFilter = value; }
        }

        [XmlArrayItem("ExcludeJob")]
        public List<JobFilter> JobExcludeFilter
        {
            get { return jobExcludeFilter; }
            set { jobExcludeFilter = value; }
        }


        public bool TreatCustomCounterFailureAsCritical
        {
            get { return treatCustomCounterFailureAsCritical; }
            set { treatCustomCounterFailureAsCritical = value; }
        }

        public bool TreatSingleJobFailureAsCritical
        {
            get { return treatSingleJobFailureAsCritical; }
            set { treatSingleJobFailureAsCritical = value; }
        }

        public string DatabaseExcludeToString()
        {
            return CombineStringArrays(DatabaseExcludeMatch, DatabaseExcludeLike);
        }

        // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --filegroup filters exclude text
        public string FilegroupExcludeToString()
        {
            return CombineStringArrays(FilegroupExcludeMatch, FilegroupExcludeLike);
        }

        //SQLdm 8.5 (Ankit Srivastava): for Inclusion filters
        public string DatabaseIncludeToString()
        {
            return CombineStringArrays(DatabaseIncludeMatch, DatabaseIncludeLike);
        }

        public string DriveExcludeToString()
        {
            return CombineStringArrays(DrivesExcludedMatch, null);
        }

        public string TableExcludeToString()
        {
            return CombineStringArrays(TableExcludeMatch, TableExcludeLike);
        }

        public string JobsExcludeToString()
        {
            return CombineStringArrays(JobNameExcludeMatch, JobNameExcludeLike);
        }

        public string JobCategoriesExcludeToString()
        {
            return CombineStringArrays(JobCategoryExcludeMatch, JobCategoryExcludeLike);
        }

        public string ApplicationsExcludeToString()
        {
            return CombineStringArrays(AppNameExcludeMatch, AppNameExcludeLike);
        }

        //SQLdm 8.5 (Ankit Srivastava): for Inclusion filters
        public string ApplicationsIncludeToString()
        {
            return CombineStringArrays(AppNameIncludeMatch, AppNameIncludeLike);
        }

        public string HostServersExcludeToString()
        {
            return CombineStringArrays(HostExcludeMatch, HostExcludeLike);
        }

        public string UsersExcludeToString()
        {
            return CombineStringArrays(UserExcludeMatch, UserExcludeLike);
        }

        public string LogIncludeCriticalToString()
        {
            return CombineStringArrays(LogIncludeMatchCritical, LogIncludeLikeCritical);
        }

        public string LogRegexIncludeCriticalToString()
        {
            return RegexToString(LogIncludeRegexCritical);
        }

        public string LogIncludeWarningToString()
        {
            return CombineStringArrays(LogIncludeMatchWarning, LogIncludeLikeWarning);
        }

        public string LogRegexIncludeWarningToString()
        {
            return RegexToString(LogIncludeRegexWarning);
        }

        public string LogIncludeInfoToString()
        {
            return CombineStringArrays(LogIncludeMatchInfo, LogIncludeLikeInfo);
        }

        public string LogRegexIncludeInfoToString()
        {
            return RegexToString(LogIncludeRegexInfo);
        }

        private static string RegexToString(IEnumerable<string> set1)
        {
            OrderedSet<string> orderedSet = new OrderedSet<string>();

            if (set1 != null)
            {
                foreach (string exclusion in set1)
                {
                    if (!orderedSet.Contains(exclusion))
                    {
                        orderedSet.Add(exclusion);
                    }
                }
            }

            StringBuilder regexString = new StringBuilder();

            foreach (string exclusion in orderedSet)
            {
                if (regexString.Length != 0)
                {
                    regexString.Append("\n");
                }
                regexString.Append(exclusion);
            }

            return regexString.ToString();
        }


        private static string CombineStringArrays(IEnumerable<string> set1, IEnumerable<string> set2)
        {
            OrderedSet<string> orderedSet = new OrderedSet<string>();

            if (set1 != null)
            {
                foreach (string exclusion in set1)
                {
                    if (!(string.IsNullOrEmpty(exclusion) || orderedSet.Contains(exclusion)))
                    {
                        orderedSet.Add(exclusion);
                    }
                }
            }

            if (set2 != null)
            {
                foreach (string exclusion in set2)
                {
                    if (!(string.IsNullOrEmpty(exclusion) || orderedSet.Contains(exclusion)))
                    {
                        orderedSet.Add(exclusion);
                    }
                }
            }

            StringBuilder combinedString = new StringBuilder();

            foreach (string exclusion in orderedSet)
            {
                if (combinedString.Length != 0)
                {
                    combinedString.Append("; ");
                }

                combinedString.Append("[");
                combinedString.Append(exclusion);
                combinedString.Append("]");
            }

            return combinedString.ToString();
        }


    }

    public enum AlertFilterReason
    {
        NotFiltered,
        Filtered,
        Smoothed,
        Snoozed
    }

    /// <summary>
    /// Class providing wildcard string matching.
    /// </summary>
    public static class AlertFilterHelper
    {
        private const char LIKEALL = '%';
        private const char LIKEONE = '_';

        private static char[] TOKENS = new char[] { LIKEALL, LIKEONE };

        public static AlertFilterReason IsFiltered(MetricThresholdEntry thresholdEntry, IEvent ievent)
        {
            object data = thresholdEntry.Data;
            if (data != null)
            {
                AdvancedAlertConfigurationSettings advancedSettings =
                    data is AdvancedAlertConfigurationSettings
                        ? (AdvancedAlertConfigurationSettings)data
                        : new AdvancedAlertConfigurationSettings(MetricDefinition.GetMetric(thresholdEntry.MetricID),
                                                                 data);
                return IsFiltered(advancedSettings, ievent.MonitoredObject, ievent.AdditionalData);
            }
            return AlertFilterReason.NotFiltered;
        }

        public static AlertFilterReason IsFiltered(AdvancedAlertConfigurationSettings advancedSettings, MonitoredObjectName objectName, object additionalData)
        {


            if (advancedSettings != null)
            {
                switch (advancedSettings.Metric)
                {
                    //START: SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --added new metrices for Filegroup space
                    case Metric.FilegroupSpaceFullPct:
                    case Metric.FilegroupSpaceFullSize:
                    //END: SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --added new metrices for Filegroup space
                    case Metric.DatabaseStatus:
                    case Metric.DatabaseSizePct:
                    case Metric.TransLogSize:
                    case Metric.ReorganisationPct:
                    case Metric.DataFileAutogrow:
                    case Metric.LogFileAutogrow:
                    case Metric.DatabaseSizeMb:
                    case Metric.TransLogSizeMb:
                    //SQLDM-19212 fix (Barkha Khatri )
                    case Metric.DatabaseLastBackupDate:
                        //if (IsDatabaseExcluded(advancedSettings, objectName))
                        if (IsDatabaseExcluded(advancedSettings, objectName) || IsDatabaseIncluded(advancedSettings, objectName))
                            return AlertFilterReason.Filtered;
                        break;
                    case Metric.BombedJobs:
                    case Metric.LongJobs:
                    case Metric.LongJobsMinutes:
                    case Metric.JobCompletion:
                        if (IsJobExcluded(advancedSettings, additionalData))
                            return AlertFilterReason.Filtered;
                        break;
                    case Metric.OldestOpenTransMinutes:
                    case Metric.ResourceAlert:
                    case Metric.SessionTempdbSpaceUsage:
                        if (IsSessionExcluded(advancedSettings, additionalData))
                            return AlertFilterReason.Filtered;
                        break;
                    case Metric.BlockingAlert:
                        if (IsBlockedSessionExcluded(advancedSettings, additionalData))
                            return AlertFilterReason.Filtered;
                        break;
                    case Metric.OSDiskFull:
                    case Metric.OSDiskAverageDiskQueueLengthPerDisk:
                    case Metric.OSDiskPhysicalDiskTimePctPerDisk:
                    case Metric.AverageDiskMillisecondsPerRead:
                    case Metric.AverageDiskMillisecondsPerTransfer:
                    case Metric.AverageDiskMillisecondsPerWrite:
                    case Metric.DiskReadsPerSecond:
                    case Metric.DiskTransfersPerSecond:
                    case Metric.DiskWritesPerSecond:
                    case Metric.OsDiskFreeMb:
                        if (IsDiskExcluded(advancedSettings, additionalData))
                            return AlertFilterReason.Filtered;
                        break;
                    case Metric.VersionStoreGenerationRatio:
                        if (IsVersionStoreGenerationRatioExcluded(advancedSettings, additionalData))
                            return AlertFilterReason.Filtered;
                        if (IsVersionStoreSizeExcluded(advancedSettings, additionalData, Metric.VersionStoreGenerationRatio))
                            return AlertFilterReason.Filtered;
                        break;
                    //SQLDM-28237. Update LongRunnignVersionStoreTransaction
                    case Metric.LongRunningVersionStoreTransaction:
                        if (IsVersionStoreSizeExcluded(advancedSettings, additionalData, Metric.LongRunningVersionStoreTransaction))
                            return AlertFilterReason.Filtered;
                        break;
                }
            }
            return AlertFilterReason.NotFiltered; ;
        }

        private static bool IsVersionStoreGenerationRatioExcluded(AdvancedAlertConfigurationSettings advancedSettings, object additionalData)
        {
            object[] tempdbData = additionalData as object[];
            if (tempdbData != null)
            {
                return ((decimal)tempdbData[0] < advancedSettings.MinGenerationRatePerSecond);
            }
            return false;
        }


        private static bool IsVersionStoreSizeExcluded(AdvancedAlertConfigurationSettings advancedSettings, object additionalData, Metric metric)
        {
            if (metric == Metric.VersionStoreGenerationRatio)
            {
                object[] tempdbData = additionalData as object[];
                if (tempdbData != null && advancedSettings.MinVersionStoreSize != null && advancedSettings.MinVersionStoreSize.Megabytes.HasValue)
                {
                    return ((decimal)tempdbData[2] < advancedSettings.MinVersionStoreSize.Megabytes);
                }
            }
            else
            {
                Pair<Session, object[]> tempdbData = (Pair<Session, object[]>)additionalData;
                if (tempdbData != null && tempdbData.Second != null && tempdbData.Second.Length >= 3 && advancedSettings.MinVersionStoreSize != null && advancedSettings.MinVersionStoreSize.Megabytes.HasValue)
                {
                    return ((decimal)tempdbData.Second[2] < advancedSettings.MinVersionStoreSize.Megabytes);
                }
            }

            return false;
        }

        private static bool IsDiskExcluded(AdvancedAlertConfigurationSettings advancedSettings, object additionalData)
        {
            DiskDrive drive = additionalData as DiskDrive;

            if (drive != null)
            {
                string driveName = drive.DriveLetter;
                driveName = driveName.TrimEnd('\\', '/');

                switch (driveName.Length)
                {
                    case 1:
                        if (driveName[0] == ':')
                            break;
                        driveName = driveName.ToUpper() + ":";
                        break;
                    case 2:
                        driveName = driveName.ToUpper();
                        break;
                    default:
                        break;
                }

                if (IsMatchValue(advancedSettings.DrivesExcludedMatch, driveName))
                {
                    return true;
                }
            }

            return false;

        }


        private static bool IsBlockedSessionExcluded(AdvancedAlertConfigurationSettings advancedSettings, object additionalData)
        {
            BlockingSession blocking = additionalData as BlockingSession;

            if (blocking != null)
            {
                if (IsSessionExcluded(advancedSettings, additionalData))
                    return true;
            }

            return false;
        }

        private static bool IsSessionExcluded(AdvancedAlertConfigurationSettings advancedSettings, object additionalData)
        {
            IFilterableSession session = additionalData as IFilterableSession;

            if (session != null)
            {
                if (IsMatchValue(advancedSettings.AppNameExcludeMatch, session.Application) || IsLikeValue(advancedSettings.AppNameExcludeLike, session.Application))
                    return true;
                if (IsMatchValue(advancedSettings.UserExcludeMatch, session.Login) || IsLikeValue(advancedSettings.UserExcludeLike, session.Login))
                    return true;
                if (IsMatchValue(advancedSettings.HostExcludeMatch, session.Host) || IsLikeValue(advancedSettings.HostExcludeLike, session.Host))
                    return true;
            }

            return false;
        }

        private static bool IsJobExcluded(AdvancedAlertConfigurationSettings advancedSettings, object additionalData)
        {
            Triple<string, string, string> jobInfo = new Triple<string, string, string>();
            bool isJobStep = false;

            jobInfo.First = ((AgentJobBase)additionalData).Category;

            jobInfo.Second = ((AgentJobBase)additionalData).JobName;

            if (additionalData is AgentJobFailure)
            {
                jobInfo.Third = ((AgentJobFailure)additionalData).StepName;

                // If Alert On Steps is false, then the records we get for job failures are job level, 
                //  but they reflect which step triggered the failure
                isJobStep = advancedSettings.AlertOnJobSteps == false;
            }
            else if (additionalData is AgentJobCompletion)
            {
                jobInfo.Third = ((AgentJobCompletion)additionalData).StepName;
                isJobStep = ((AgentJobCompletion)additionalData).StepId == 0;
            }
            else if (additionalData is AgentJobRunning)
            {
                jobInfo.Third = ((AgentJobRunning)additionalData).StepName;

                // If Alert On Steps is false, then the records we get for job failures are job level, 
                //  but they reflect which step triggered the failure
                isJobStep = advancedSettings.AlertOnJobSteps == false;
            }

            bool isExcluded = true;

            if (advancedSettings.JobIncludeFilter.Count == 0)
                isExcluded = false;
            else
            {
                foreach (JobFilter filter in advancedSettings.JobIncludeFilter)
                {
                    if (advancedSettings.AlertOnJobSteps)
                    {
                        if (MatchFilter(filter.Category, jobInfo.First) &&
                            MatchFilter(filter.JobName, jobInfo.Second) &&
                            MatchFilter(filter.StepName, jobInfo.Third))
                        {
                            isExcluded = false;
                        }
                        else if (MatchFilter(filter.Category, jobInfo.First) &&
                            MatchFilter(filter.JobName, jobInfo.Second) &&
                            isJobStep)
                        {
                            isExcluded = false;
                        }
                    }
                    else
                    {
                        if (MatchFilter(filter.Category, jobInfo.First) &&
                            MatchFilter(filter.JobName, jobInfo.Second))
                        {
                            isExcluded = false;
                        }
                    }
                }
            }


            // Exclude the alert if it is for a specific step and AlertOnJobSteps is turned off.
            if (advancedSettings.AlertOnJobSteps == false && isJobStep == false)
                isExcluded = true;

            // If you made it through the include filter and isExclude is true, then there is no reason to check the exclusion filters
            if (isExcluded == true) return isExcluded;

            if (advancedSettings.JobExcludeFilter.Count == 0)
                isExcluded = false;
            else
            {
                foreach (JobFilter filter in advancedSettings.JobExcludeFilter)
                {
                    if (advancedSettings.AlertOnJobSteps)
                    {
                        if (MatchFilter(filter.Category, jobInfo.First) &&
                            MatchFilter(filter.JobName, jobInfo.Second) &&
                            MatchFilter(filter.StepName, jobInfo.Third))
                        {
                            isExcluded = true;
                        }
                    }
                    else
                    {
                        if (MatchFilter(filter.Category, jobInfo.First) &&
                            MatchFilter(filter.JobName, jobInfo.Second))
                        {
                            isExcluded = true;
                        }
                    }
                }
            }

            return isExcluded;
        }

        public static bool IsSnoozed(AdvancedAlertConfigurationSettings advancedSettings, DateTime alertTimeUTC)
        {
            if (advancedSettings != null)
            {
                SnoozeInfo snoozeInfo = advancedSettings.SnoozeInfo;
                if (snoozeInfo != null)
                {
                    return (alertTimeUTC < snoozeInfo.StopSnoozing);
                }
            }
            return false;
        }

        public static bool IsSmoothed(AdvancedAlertConfigurationSettings settings, StateDeviationUpdateEvent updateEvent)
        {
            return updateEvent != null &&
                 IsSmoothed(settings, updateEvent.OccuranceTime, updateEvent.DeviationEvent.OccuranceTime);
        }

        public static bool IsSmoothed(AdvancedAlertConfigurationSettings settings, DateTime alertTimeUTC, DateTime alertStartTimeUTC)
        {
            bool result = false;
            if (settings != null && settings.SuppressionPeriodMinutes > 0)
            {
                TimeSpan duration = alertTimeUTC - alertStartTimeUTC;
                result = (duration.TotalMinutes <= settings.SuppressionPeriodMinutes);
            }
            return result;
        }

        //SQLDM-30444
        //SnapshotFilterThreshold should always be less than the totalsnapshot count
        //snapshotFilterThreshold should be greater than 1 as alert generated even without filter for 1. 
        public static bool IsAlertable(MetricState deviation, int? snapshotFilterThreshold, int? totalSnapshotCount)
        {
            bool result = true;
            if(snapshotFilterThreshold != null && totalSnapshotCount != null && snapshotFilterThreshold < totalSnapshotCount && snapshotFilterThreshold > 1)
            {
                if (deviation.TimesThresholdCrossed < snapshotFilterThreshold && deviation.TotalThresholdLimit > totalSnapshotCount)
                {
                    return false;
                }
            }
            return result;
        }

        public static bool IsSmoothingEnabled(AdvancedAlertConfigurationSettings settings)
        {
            return settings != null && settings.SuppressionPeriodMinutes > 0;
        }

        public static bool IsDatabaseExcluded(AdvancedAlertConfigurationSettings advancedSettings, MonitoredObjectName name)
        {
            return (IsMatchValue(advancedSettings.DatabaseExcludeMatch, name.DatabaseName) || IsLikeValue(advancedSettings.DatabaseExcludeLike, name.DatabaseName));
        }

        //SQLdm 8.5 (Ankit Srivastava): for Inclusion filters
        public static bool IsDatabaseIncluded(AdvancedAlertConfigurationSettings advancedSettings, MonitoredObjectName name)
        {
            return (IsMatchValue(advancedSettings.DatabaseIncludeMatch, name.DatabaseName) || IsLikeValue(advancedSettings.DatabaseIncludeLike, name.DatabaseName));
        }

        public static bool IsTableExcluded(string[] tableExcludeMatch, string[] tableExcludeLike, string databaseName, string schemaName, string tableName)
        {
            string qualName = databaseName + "." + schemaName + "." + tableName;
            return (IsMatchValue(tableExcludeMatch, qualName) || IsLikeValue(tableExcludeLike, qualName));
        }

        internal static bool IsMatchValue(string[] patterns, string value)
        {
            if (patterns == null || patterns.Length == 0)
                return false;

            if (value != null)
                value = value.TrimEnd().ToLower();

            foreach (string pattern in patterns)
            {
                if (value == null || value.Length == 0)
                {
                    if (pattern == null || pattern.Length == 0)
                        return true;
                    else
                        continue;
                }
                else
                {
                    if (value.Equals(pattern.ToLower()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool IsLikeValue(string[] patterns, string value)
        {
            if (patterns == null || patterns.Length == 0)
                return false;

            if (value != null)
                value = value.TrimEnd().ToLower();

            foreach (string pattern in patterns)
            {
                if (value == null || value.Length == 0)
                {
                    if (pattern == null || pattern.Length == 0 || pattern == LIKEALL.ToString())
                    {
                        return true;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    if (Match(pattern.ToLower(), value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool MatchFilter(FilterSet FilterValue, string value)
        {
            if (FilterValue == null || value == null)
                return false;

            if (FilterValue.OpCode == OpCodes.Equals)
                return FilterValue.Filter.Trim().ToLower().Equals(value.Trim().ToLower());
            else
                return Match(FilterValue.Filter.Trim().ToLower(), value.Trim().ToLower());
        }


        public static bool Match(string pattern, string s)
        {
            int i, j, cp, mp;

            if (pattern.IndexOfAny(TOKENS) == -1)
                return (s.Equals(pattern));

            int s_Length = s.Length;
            int p_Length = pattern.Length;

            for (i = 0, j = 0; i < s_Length && j < p_Length && pattern[j] != LIKEALL; i++, j++)
            {
                if ((pattern[j] != s[i]) && (pattern[j] != LIKEONE))
                    return false;
            }

            if (j == p_Length)
                return s_Length == p_Length;

            for (cp = 0, mp = 0; i < s.Length; )
            {
                if (j < pattern.Length && pattern[j] == LIKEALL)
                {
                    if ((j++) >= pattern.Length)
                        return true;

                    mp = j;
                    cp = i + 1;
                }
                else if (j < p_Length && (pattern[j] == s[i] || pattern[j] == LIKEONE))
                {
                    j++;
                    i++;
                }
                else
                {
                    j = mp;
                    i = cp++;
                }
            }

            while (j < p_Length && pattern[j] == LIKEALL)
            {
                j++;
            }

            return j >= p_Length;
        }

        
    }


    [Serializable]
    public class SnoozeInfo : ICloneable
    {
        private DateTime startSnoozing;
        private DateTime stopSnoozing;
        private string snoozedBy;
        private string unsnoozedBy;

        public SnoozeInfo()
        {
        }

        public SnoozeInfo(DateTime startTimeUTC, DateTime endTimeUTC, string startUser, string endUser)
        {
            this.startSnoozing = startTimeUTC;
            this.stopSnoozing = endTimeUTC;
            this.snoozedBy = startUser;
            this.unsnoozedBy = endUser;
        }

        public SnoozeInfo(DateTime startTimeUTC, int durationInMinutes, string user)
        {
            startSnoozing = startTimeUTC;
            stopSnoozing = startTimeUTC + TimeSpan.FromMinutes(durationInMinutes);
            snoozedBy = user;
            unsnoozedBy = String.Empty;
        }

        public void Change(DateTime startTimeUTC, DateTime endTimeUTC, string user)
        {
            if (startTimeUTC > stopSnoozing)
            {
                startSnoozing = startTimeUTC;
                snoozedBy = user;
            }

            stopSnoozing = endTimeUTC;
            unsnoozedBy = user;
        }

        public void Change(DateTime startTimeUTC, int durationInMinutes, string user)
        {
            if (startTimeUTC > stopSnoozing)
            {
                startSnoozing = startTimeUTC;
                snoozedBy = user;
            }

            stopSnoozing = startTimeUTC + TimeSpan.FromMinutes(durationInMinutes);
            if (stopSnoozing <= DateTime.UtcNow)
                unsnoozedBy = user;
        }

        public void Stop(DateTime endTimeUTC, string user)
        {
            if (endTimeUTC < stopSnoozing)
            {
                stopSnoozing = endTimeUTC;
                unsnoozedBy = user;
            }
        }

        public bool IsSnoozed(DateTime timestampUtc)
        {
            return timestampUtc <= stopSnoozing;
        }

        public DateTime StartSnoozing
        {
            get { return startSnoozing; }
        }

        public DateTime StopSnoozing
        {
            get { return stopSnoozing; }
        }
        public string SnoozedBy
        {
            get { return snoozedBy; }
        }

        public string UnsnoozedBy
        {
            get { return unsnoozedBy; }
        }

        #region ICloneable Members

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }

    public enum OpCodes
    {
        Like,
        Equals
    }

    [Serializable]
    public class FilterSet
    {
        #region fields

        public const string WILDCARD = "%";
        private OpCodes opCode;
        private string filter;

        #endregion

        #region constructors
        public FilterSet()
        {
        }

        public FilterSet(OpCodes opCode, string filter)
        {
            this.OpCode = opCode;
            this.Filter = filter;
        }
        #endregion

        #region properties
        [XmlAttribute]
        public OpCodes OpCode
        {
            get { return opCode; }
            set { opCode = value; }
        }

        public string Filter
        {
            get { return filter; }
            set
            {
                if (value == null || value.Length == 0)
                    filter = WILDCARD;
                else
                    filter = value;
            }
        }

        public bool IsWildCard
        {
            get { return filter == WILDCARD; }
        }

        #endregion
    }


    [Serializable]
    public class JobFilter
    {
        #region fields
        FilterSet jobName;
        FilterSet category;
        FilterSet stepName;
        #endregion

        #region constructors
        public JobFilter()
        {
        }

        public JobFilter(FilterSet name)
        {
            this.JobName = name;
        }

        public JobFilter(FilterSet name, FilterSet category)
        {
            this.JobName = name;
            this.Category = category;
        }

        public JobFilter(FilterSet name, FilterSet category, FilterSet step)
        {
            this.JobName = name;
            this.Category = category;
            this.StepName = step;
        }
        #endregion

        #region properties
        public FilterSet JobName
        {
            get { return jobName; }
            set { jobName = value; }
        }

        public FilterSet Category
        {
            get { return category; }
            set { category = value; }
        }

        public FilterSet StepName
        {
            get { return stepName; }
            set { stepName = value; }
        }
        #endregion

    }

}
