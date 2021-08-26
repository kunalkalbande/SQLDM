//------------------------------------------------------------------------------
// <copyright file="TempdbStatistics.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.Snapshots
{
    [Serializable]
    public class TempdbStatistics
    {
        private long? versionStoreGenerationKilobytes_Raw = null;
        private long? versionStoreCleanupKilobytes_Raw = null;

        private decimal? versionStoreGenerationKilobytes = null;
        private decimal? versionStoreCleanupKilobytes = null;

        private TimeSpan tempdbPFSWaitTime = new TimeSpan(0);
        private TimeSpan tempdbGAMWaitTime = new TimeSpan(0);
        private TimeSpan tempdbSGAMWaitTime = new TimeSpan(0);

        private DateTime? timeStamp = null;
        private TimeSpan? timeDelta = null;

        public TempdbStatistics()
        {
        }

        public decimal? VersionStoreGenerationKilobytes
        {
            get { return versionStoreGenerationKilobytes; }
            set { versionStoreGenerationKilobytes = value; }
        }

        public decimal? VersionStoreCleanupKilobytes
        {
            get { return versionStoreCleanupKilobytes; }
            set { versionStoreCleanupKilobytes = value; }
        }

        public TimeSpan TempdbPFSWaitTime
        {
            get { return tempdbPFSWaitTime; }
            set { tempdbPFSWaitTime = value; }
        }

        public TimeSpan TempdbGAMWaitTime
        {
            get { return tempdbGAMWaitTime; }
            set { tempdbGAMWaitTime = value; }
        }

        public TimeSpan TempdbSGAMWaitTime
        {
            get { return tempdbSGAMWaitTime; }
            set { tempdbSGAMWaitTime = value; }
        }

        internal long? VersionStoreGenerationKilobytesRaw
        {
            get { return versionStoreGenerationKilobytes_Raw; }
            set { versionStoreGenerationKilobytes_Raw = value; }
        }

        internal long? VersionStoreCleanupKilobytesRaw
        {
            get { return versionStoreCleanupKilobytes_Raw; }
            set { versionStoreCleanupKilobytes_Raw = value; }
        }

        public TimeSpan? TimeDelta
        {
            get { return timeDelta; }
            set { timeDelta = value; }
        }

        public DateTime? TimeStamp
        {
            get { return timeStamp; }
            set { timeStamp = value; }
        }
    }

    [Serializable]
    public class TempdbSummaryStatistics : TempdbStatistics
    {
        private decimal? userObjectsMegabytes = null;
        private decimal? internalObjectsMegabytes = null;
        private decimal? versionStoreMegabytes = null;
        private decimal? mixedExtentsMegabytes = null;
        private decimal? unallocatedSpaceMegabytes = null;

        public TempdbSummaryStatistics()
        {
        }

        public TempdbSummaryStatistics(TempdbStatistics tempdbStatistics)
        {
            TempdbGAMWaitTime = tempdbStatistics.TempdbGAMWaitTime;
            TempdbPFSWaitTime = tempdbStatistics.TempdbPFSWaitTime;
            TempdbSGAMWaitTime = tempdbStatistics.TempdbSGAMWaitTime;
            TimeDelta = tempdbStatistics.TimeDelta;
            TimeStamp = tempdbStatistics.TimeStamp;
            VersionStoreCleanupKilobytes = tempdbStatistics.VersionStoreCleanupKilobytes;
            VersionStoreCleanupKilobytesRaw = tempdbStatistics.VersionStoreCleanupKilobytesRaw;
            VersionStoreGenerationKilobytes = tempdbStatistics.VersionStoreGenerationKilobytes;
            VersionStoreGenerationKilobytesRaw = tempdbStatistics.VersionStoreGenerationKilobytesRaw;
        }

        public decimal? UserObjectsMegabytes
        {
            get { return userObjectsMegabytes; }
            set { userObjectsMegabytes = value; }
        }

        public decimal? InternalObjectsMegabytes
        {
            get { return internalObjectsMegabytes; }
            set { internalObjectsMegabytes = value; }
        }

        public decimal? VersionStoreMegabytes
        {
            get { return versionStoreMegabytes; }
            set { versionStoreMegabytes = value; }
        }

        public decimal? MixedExtentsMegabytes
        {
            get { return mixedExtentsMegabytes; }
            set { mixedExtentsMegabytes = value; }
        }

        public decimal? UnallocatedSpaceMegabytes
        {
            get { return unallocatedSpaceMegabytes; }
            set { unallocatedSpaceMegabytes = value; }
        }
    }
}
