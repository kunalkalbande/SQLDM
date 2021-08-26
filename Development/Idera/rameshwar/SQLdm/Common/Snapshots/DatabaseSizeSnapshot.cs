//------------------------------------------------------------------------------
// <copyright file="DatabaseSizeSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Events;

namespace Idera.SQLdm.Common.Snapshots
{
    /// <summary>
    /// Represents snapshot information for database size alerting
    /// </summary>
    [Serializable]
    public sealed class DatabaseSizeSnapshot: AlertableSnapshot
    {
        // SQLdm 10.2 (Tolga K) - Availability group information, SQLDM-24576
        private AlwaysOnAvailabilityGroupsSnapshot availabilityGroupsSnapshot = new AlwaysOnAvailabilityGroupsSnapshot();

        private Dictionary<String, DatabaseStatistics> dbStatistics = new Dictionary<string, DatabaseStatistics>();
        private Dictionary<string, DiskDrive> diskDrives = new Dictionary<string, DiskDrive>();
        private ServerDatabaseSummary databaseSummary = new ServerDatabaseSummary();
        private TimeSpan? timeDelta;
        private List<Metric> alertableMetrics = new List<Metric>();

        private List<DiskDriveStatistics> diskDriveStatistics = new List<DiskDriveStatistics>();   // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --disk drive statistics object
        
        public DatabaseSizeSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            alertableMetrics.AddRange(Idera.SQLdm.Common.Events.AlertingMetrics.DatabaseMetrics);
        }

        // SQLdm 10.2 (Tolga K) - Availability group information, SQLDM-24576
        public AlwaysOnAvailabilityGroupsSnapshot AvailabilityGroupsSnapshot
        {
            get { return availabilityGroupsSnapshot; }
            set { availabilityGroupsSnapshot = value; }
        }

        public override AlertableSnapshotType SnapshotType
        {
            get { return AlertableSnapshotType.DatabaseSize; }
        }

        public Dictionary<string, DatabaseStatistics> DbStatistics
        {
            get { return dbStatistics; }
            internal set { dbStatistics = value; }
        }

        // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --disk drive statistics property
        public List<DiskDriveStatistics> DiskDriveStatistics
        {
            get { return diskDriveStatistics; }
            internal set { diskDriveStatistics = value; }
        }

        public Dictionary<string, DiskDrive> DiskDrives
        {
            get { return diskDrives; }
            internal set { diskDrives = value; }
        }

        public ServerDatabaseSummary DatabaseSummary
        {
            get { return databaseSummary; }
            internal set { databaseSummary = value; }
        }

        public TimeSpan? TimeDelta
        {
            get { return timeDelta; }
            internal set { timeDelta = value; }
        }

        public override List<Metric> AlertableMetrics
        {
            get { return alertableMetrics; }
            set { alertableMetrics = value; }
        }
    }
}
