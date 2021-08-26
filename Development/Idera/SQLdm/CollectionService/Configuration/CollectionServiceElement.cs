//------------------------------------------------------------------------------
// <copyright file="CollectionServiceElement.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Configuration;
using Idera.SQLdm.Common;

namespace Idera.SQLdm.CollectionService.Configuration
{
    public class CollectionServiceElement : ConfigurationElement
    {
        #region fields

        // SQLdm 10.1.3 (Varun Chopra) SQLDM-19247: Timeout When Viewing Large Databases Under Tables & Indexes - Configurable Collection Time
        private const string CollectionWaitTimeInSecondsKey = "collectionWaitTimeInSeconds";
        private const string InstanceNameKey = "instanceName";
        private const string ServicePortKey = "servicePort";
        private const string DiskDriveOptionValue = "diskDriveOption";
        private const string ManagementServiceAddressKey = "managementServiceAddress";
        private const string ManagementServicePortKey = "managementServicePort";
        private const string TracerXSectionKey = "TracerX";
        private const string HeartbeatIntervalSecondsKey = "heartbeatIntervalSeconds";
        private const string WmiQueryTimeoutKey = "wmiQueryTimeOut";
        private const string DefaultSqlConnectionTimeoutKey = "defaultSqlConnectionTimeoutInSeconds";
        private const string DefaultSqlCommandTimeoutKey = "defaultSqlCommandTimeoutInSeconds";
        private const string QuietTimeSqlCommandTimeoutKey = "quietTimeSqlCommandTimeoutInSeconds";
        private const string FragmentationSqlCommandTimeoutKey = "fragmentationSqlCommandTimeoutInSeconds";
        private const string MaxRowCountLockDetailsKey = "maxRowCountLockDetails";
        private const string MaxRowCountSessionsKey = "maxRowCountSessions";
        private const string CustomOSCounterWaitTimeInSecondsKey = "customOSCounterWaitTimeInSeconds";
        private const string MaxRowCountLogScanKey = "maxRowCountLogScan";
        private const string ScheduledCollectionMaxQueueLengthKey = "scheduledCollectionMaxQueueLength";
        private const string DataDirectoryKey = "dataDirectory";
        private const string MountPointPathKey = "mountPointPath";
        private const string SnapshotDeliveryTimeoutKey = "snapshotDeliveryTimeoutInSeconds";
        private const string DropPlansFromCacheKey = "dropPlansFromCache";
        private const string MaxConcurrentServiceConnectionsKey = "maxConcurrentServiceConnections";
        private const string MaxRowCountProcedureCacheKey = "maxRowCountProcedureCache";
        private const string MaxQueryMonitorEventSizeKBKey = "maxQueryMonitorEventSizeKB";
        private const string CookedDiskDriveWaitTimeInSecondsKey = "cookedDiskDriveWaitTimeInSeconds";
        private const string MaxRowCountTableGrowthKey = "maxRowCountTableGrowth";
        private const string PublishPerformanceCountersKey = "publishPerformanceCounters";

        #endregion

        #region constructors

        #endregion

        #region properties

        [ConfigurationProperty(InstanceNameKey, IsRequired = true, IsKey = true)]
        public string InstanceName
        {
            get { return (string)this[InstanceNameKey]; }
            set { this[InstanceNameKey] = value; }
        }

        //SQLDM-30012.Change DiskCollection statistics to Instance Specific.
        [ConfigurationProperty(DiskDriveOptionValue, DefaultValue = "Server")]
        public string DiskDriveOption
        {
            get { return (string)this[DiskDriveOptionValue]; }
            set { this[DiskDriveOptionValue] = value; }
        }

        [ConfigurationProperty(ServicePortKey, DefaultValue = 5167)]
        public int ServicePort
        {
            get { return (int)this[ServicePortKey]; }
            set { this[ServicePortKey] = value; }
        }

        [ConfigurationProperty(ManagementServiceAddressKey, IsRequired = true)]
        public string ManagementServiceAddress
        {
            get { return (string)this[ManagementServiceAddressKey]; }
            set { this[ManagementServiceAddressKey] = value; }
        }

        [ConfigurationProperty(ManagementServicePortKey, DefaultValue = 5166)]
        public int ManagementServicePort
        {
            get { return (int)this[ManagementServicePortKey]; }
            set { this[ManagementServicePortKey] = value; }
        }

        [ConfigurationProperty(TracerXSectionKey, DefaultValue = "TracerX")]
        public string TracerXSectionName
        {
            get { return (string)this[TracerXSectionKey]; }
            set { this[TracerXSectionKey] = value; }
        }

        [ConfigurationProperty(HeartbeatIntervalSecondsKey, DefaultValue = 180)]
        public int HeartbeatIntervalSeconds
        {
            get { return (int)this[HeartbeatIntervalSecondsKey]; }
            set { this[HeartbeatIntervalSecondsKey] = value; }
        }

        /// <summary>
        /// Configurable Collection Time
        /// </summary>
        /// <remarks>
        /// SQLdm 10.1.3 (Varun Chopra) SQLDM-19247: Timeout When Viewing Large Databases Under Tables & Indexes 
        /// </remarks>
        [ConfigurationProperty(CollectionWaitTimeInSecondsKey, DefaultValue = 300)]
        public int CollectionWaitTimeInSeconds
        {
            get { return (int)this[CollectionWaitTimeInSecondsKey]; }
            set { this[CollectionWaitTimeInSecondsKey] = value; }
        }

        [ConfigurationProperty(WmiQueryTimeoutKey, DefaultValue = 90)]
        public int WmiQueryTimeout
        {
            get { return (int)this[WmiQueryTimeoutKey]; }
            set { this[WmiQueryTimeoutKey] = value; }
        }

        [ConfigurationProperty(DefaultSqlConnectionTimeoutKey, DefaultValue = Constants.DefaultConnectionTimeout)]
        public int DefaultSqlConnectionTimeout
        {
            get { return (int)this[DefaultSqlConnectionTimeoutKey]; }
            set { this[DefaultSqlConnectionTimeoutKey] = value; }
        }

        [ConfigurationProperty(DefaultSqlCommandTimeoutKey, DefaultValue = Constants.DefaultCommandTimeout)]
        public int DefaultSqlCommandTimeout
        {
            get { return (int)this[DefaultSqlCommandTimeoutKey]; }
            set { this[DefaultSqlCommandTimeoutKey] = value; }
        }

        [ConfigurationProperty(QuietTimeSqlCommandTimeoutKey, DefaultValue = Constants.QuietTimeCommandTimeout)]
        public int QuietTimeSqlCommandTimeout
        {
            get { return (int)this[QuietTimeSqlCommandTimeoutKey]; }
            set { this[QuietTimeSqlCommandTimeoutKey] = value; }
        }

        [ConfigurationProperty(FragmentationSqlCommandTimeoutKey, DefaultValue = Constants.QuietTimeCommandTimeout)]
        public int FragmentationSqlCommandTimeout
        {
            get { return (int)this[FragmentationSqlCommandTimeoutKey]; }
            set { this[FragmentationSqlCommandTimeoutKey] = value; }
        }

        [ConfigurationProperty(MaxRowCountLockDetailsKey, DefaultValue = 10000)]
        public int MaxRowCountLockDetails
        {
            get { return (int)this[MaxRowCountLockDetailsKey]; }
            set { this[MaxRowCountLockDetailsKey] = value; }
        }

        [ConfigurationProperty(MaxRowCountSessionsKey, DefaultValue = 10000)]
        public int MaxRowCountSessions
        {
            get { return (int)this[MaxRowCountSessionsKey]; }
            set { this[MaxRowCountSessionsKey] = value; }
        }

        [ConfigurationProperty(CustomOSCounterWaitTimeInSecondsKey, DefaultValue = 1)]
        public int CustomOSCounterWaitTimeInSeconds
        {
            get { return (int)this[CustomOSCounterWaitTimeInSecondsKey]; }
            set { this[CustomOSCounterWaitTimeInSecondsKey] = value; }
        }

        [ConfigurationProperty(MaxRowCountLogScanKey, DefaultValue = 10000)]
        public int MaxRowCountLogScan
        {
            get { return (int)this[MaxRowCountLogScanKey]; }
            set { this[MaxRowCountLogScanKey] = value; }
        }

        [ConfigurationProperty(ScheduledCollectionMaxQueueLengthKey, DefaultValue = 3)]
        public int ScheduledCollectionMaxQueueLength
        {
            get
            {
                int result = (int)this[ScheduledCollectionMaxQueueLengthKey];
                if (result < 1)
                    result = 3;
                return result;
            }
            set { this[ScheduledCollectionMaxQueueLengthKey] = value; }
        }

        [ConfigurationProperty(DataDirectoryKey, DefaultValue = "")]
        public string DataDirectory
        {
            get { return (string)this[DataDirectoryKey]; }
            set { this[DataDirectoryKey] = value; }
        }

        [ConfigurationProperty(MountPointPathKey, DefaultValue = "")]
        public string MountPointPath
        {
            get { return (string)this[MountPointPathKey]; }
            set { this[MountPointPathKey] = value; }
        }

        [ConfigurationProperty(SnapshotDeliveryTimeoutKey, DefaultValue = 90)]
        public int SnapshotDeliveryTimeoutInSeconds
        {
            get { return (int)this[SnapshotDeliveryTimeoutKey]; }
            set { this[SnapshotDeliveryTimeoutKey] = value; }
        }

        [ConfigurationProperty(DropPlansFromCacheKey, DefaultValue = "")]
        public string DropPlansFromCache
        {
            get { return (string)this[DropPlansFromCacheKey]; }
            set { this[DropPlansFromCacheKey] = value; }
        }

        [ConfigurationProperty(MaxConcurrentServiceConnectionsKey, DefaultValue = 30)]
        public int MaxConcurrentServiceConnections
        {
            get { return (int)this[MaxConcurrentServiceConnectionsKey]; }
            set { this[MaxConcurrentServiceConnectionsKey] = value; }
        }

        [ConfigurationProperty(MaxRowCountProcedureCacheKey, DefaultValue = 10000)]
        public int MaxRowCountProcedureCache
        {
            get { return (int)this[MaxRowCountProcedureCacheKey]; }
            set { this[MaxRowCountProcedureCacheKey] = value; }
        }

        [ConfigurationProperty(MaxQueryMonitorEventSizeKBKey, DefaultValue = -1)]
        public int MaxQueryMonitorEventSizeKB
        {
            get { return (int)this[MaxQueryMonitorEventSizeKBKey]; }
            set { this[MaxQueryMonitorEventSizeKBKey] = value; }
        }

        [ConfigurationProperty(CookedDiskDriveWaitTimeInSecondsKey, DefaultValue = 0)]
        public int CookedDiskDriveWaitTimeInSeconds
        {
            get { return (int)this[CookedDiskDriveWaitTimeInSecondsKey]; }
            set { this[CookedDiskDriveWaitTimeInSecondsKey] = value; }
        }

        [ConfigurationProperty(MaxRowCountTableGrowthKey, DefaultValue = 5000)]
        public int MaxRowCountTableGrowth
        {
            get { return (int)this[MaxRowCountTableGrowthKey]; }
            set { this[MaxRowCountTableGrowthKey] = value; }
        }

        /// <summary>
        /// Configurable publish Performance Counters property
        /// </summary>
        /// <remarks>
        ///  SQLDM-28034: Configuration property to publish or don't publish Performance Counters to the OS
        /// </remarks>
        [ConfigurationProperty(PublishPerformanceCountersKey, DefaultValue = false)]
        public bool PublishPerformanceCounters
        {
            get { return (bool)this[PublishPerformanceCountersKey]; }
            set { this[PublishPerformanceCountersKey] = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Gets the config record for the named instance from the config file.  If
        /// the record does not exist a default one is created and added to the file.
        /// </summary>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public static CollectionServiceElement GetElement(string instanceName)
        {
            CollectionServiceElement element = null;
            CollectionServicesSection section = CollectionServicesSection.GetSection();
            if (section != null)
                element = section.CollectionServices[instanceName];

            if (element == null)
            {
                // update the local configuration file
                System.Configuration.Configuration configuration =
                    ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // Get/Create the Idera.SQLdm configuration section
                section = configuration.GetSection(CollectionServicesSection.SectionName) as CollectionServicesSection;
                if (section == null)
                {
                    section = new CollectionServicesSection();
                    configuration.Sections.Add(CollectionServicesSection.SectionName, section);
                }
                element = new CollectionServiceElement();
                element.InstanceName = instanceName;

                section.CollectionServices[instanceName] = element;

                if (element.DefaultSqlCommandTimeout == 0)
                    element.DefaultSqlCommandTimeout = Constants.DefaultCommandTimeout;

                if (element.QuietTimeSqlCommandTimeout == 0)
                    element.QuietTimeSqlCommandTimeout = Constants.QuietTimeCommandTimeout;

                if (element.DefaultSqlConnectionTimeout == 0)
                    element.DefaultSqlConnectionTimeout = Constants.DefaultConnectionTimeout;

                if (element.MaxRowCountLockDetails == 0)
                    element.MaxRowCountLockDetails = 10000;

                if (element.CustomOSCounterWaitTimeInSeconds == 0)
                    element.CustomOSCounterWaitTimeInSeconds = 1;

                if (element.MaxRowCountLogScan == 0)
                    element.MaxRowCountLogScan = 10000;

                if (element.ScheduledCollectionMaxQueueLength == 0)
                    element.ScheduledCollectionMaxQueueLength = 3;

                if (element.SnapshotDeliveryTimeoutInSeconds == 0)
                    element.SnapshotDeliveryTimeoutInSeconds = 90;

                if (element.MaxConcurrentServiceConnections == 0)
                    element.MaxConcurrentServiceConnections = 30;

                if (element.MaxRowCountProcedureCache == 0)
                    element.MaxRowCountProcedureCache = 10000;

                if (element.MaxRowCountTableGrowth == 0)
                    element.MaxRowCountTableGrowth = 5000;


                configuration.Save();

                CollectionServicesSection.Refresh();

                section = CollectionServicesSection.GetSection();
                element = section.CollectionServices[instanceName];
            }

            return element;
        }

        public void Save()
        {
            string instanceName = InstanceName;

            CollectionServiceElement element = null;
            CollectionServicesSection section = CollectionServicesSection.GetSection();

            // update the local configuration file
            System.Configuration.Configuration configuration =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // Get/Create the Idera.SQLdm configuration section
            section = configuration.GetSection(CollectionServicesSection.SectionName) as CollectionServicesSection;
            if (section == null)
            {
                section = new CollectionServicesSection();
                configuration.Sections.Add(CollectionServicesSection.SectionName, section);
            }
            element = section.CollectionServices[instanceName];
            if (element == null)
            {
                element = new CollectionServiceElement();
                element.InstanceName = instanceName;
                section.CollectionServices[instanceName] = element;
            }

            element.HeartbeatIntervalSeconds = HeartbeatIntervalSeconds;
            element.ManagementServiceAddress = ManagementServiceAddress;
            element.ManagementServicePort = ManagementServicePort;
            element.TracerXSectionName = TracerXSectionName;
            element.ServicePort = ServicePort;
            element.DataDirectory = DataDirectory;
            element.MountPointPath = MountPointPath;
            element.DropPlansFromCache = DropPlansFromCache;
            element.MaxConcurrentServiceConnections = MaxConcurrentServiceConnections;
            element.MaxRowCountProcedureCache = MaxRowCountProcedureCache;
            element.MaxQueryMonitorEventSizeKB = MaxQueryMonitorEventSizeKB;
            //SQLDM-30012.Change DiskCollection statistics to Instance Specific.
            element.DiskDriveOption = DiskDriveOption;

            if (element.DefaultSqlCommandTimeout == 0)
                element.DefaultSqlCommandTimeout = Constants.DefaultCommandTimeout;

            if (element.QuietTimeSqlCommandTimeout == 0)
                element.QuietTimeSqlCommandTimeout = Constants.QuietTimeCommandTimeout;

            if (element.DefaultSqlConnectionTimeout == 0)
                element.DefaultSqlConnectionTimeout = Constants.DefaultConnectionTimeout;

            if (element.MaxRowCountLockDetails == 0)
                element.MaxRowCountLockDetails = 10000;

            if (element.CustomOSCounterWaitTimeInSeconds == 0)
                element.CustomOSCounterWaitTimeInSeconds = 1;

            if (element.MaxRowCountLogScan == 0)
                element.MaxRowCountLogScan = 10000;

            if (element.ScheduledCollectionMaxQueueLength == 0)
                element.ScheduledCollectionMaxQueueLength = 3;

            if (element.SnapshotDeliveryTimeoutInSeconds == 0)
                element.SnapshotDeliveryTimeoutInSeconds = 90;

            if (element.MaxConcurrentServiceConnections == 0)
                element.MaxConcurrentServiceConnections = 30;

            if (element.MaxRowCountProcedureCache == 0)
                element.MaxRowCountProcedureCache = 10000;

            if (element.MaxRowCountTableGrowth == 0)
                element.MaxRowCountTableGrowth = 5000;

            configuration.Save();

            CollectionServicesSection.Refresh();
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
