using System;
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    /// <summary>
    /// SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --adding new class for the disk drive stats
    /// </summary>
    [Serializable]
    public class DiskDriveStatistics
    {
        #region fields

        private int? sqlServerID = null;
        private DateTime? collectionDateTime = null;
        private string driveName;
        private double unusedSizeKB;
        private double totalSizeKB;
        private long diskIdlePercent;
        private long averageDiskQueueLength;
        private long averageDiskMillisecondsPerRead;
        private long averageDiskMillisecondsPerTransfer;
        private long averageDiskMillisecondsPerWrite;
        private double diskReadsPerSecond;
        private long diskTransfersPerSecond;
        private double diskWritesPerSecond;
        private DateTime? databaseSizeTime = null;

        private double? sqlDataUsedMB;
        private double? sqlDataFreeMB;
        private double? sqlLogFileMB;
        private double? nonSQLDiskUsageMB;

        #endregion

        #region properties

        public int? SQLServerID
        {
            get { return sqlServerID; }
            set { sqlServerID = value; }
        }

        public DateTime? UTCCollectionDateTime
        {
            get { return collectionDateTime; }
            set { collectionDateTime = value; }
        }

        public string DriveName
        {
            get { return driveName; }
            set { driveName = value; }
        }

        //free space
        public double UnusedSizeKB
        {
            get { return unusedSizeKB; }
            set { unusedSizeKB = value; }
        }

        //total size
        public double TotalSizeKB
        {
            get { return totalSizeKB; }
            set { totalSizeKB = value; }
        }


        public double? SQLDataUsedMB
        {
            get { return sqlDataUsedMB; }
            set { sqlDataUsedMB = value; }
        }

        public double? SQLDataFreeMB
        {
            get { return sqlDataFreeMB; }
            set { sqlDataFreeMB = value; }
        }

        public double? SQLLogFileMB
        {
            get { return sqlLogFileMB; }
            set { sqlLogFileMB = value; }
        }

        public double? NonSQLDiskUsageMB
        {
            get
            {
				//SQLdm 9.1 (Ankit Srivastava) - Fixed DE44566 .. modified tha nonSQLDiskUsage calculation
                if (nonSQLDiskUsageMB == null)
                    return ((totalSizeKB - unusedSizeKB) / 1024 - (sqlDataUsedMB ?? 0) - (sqlDataFreeMB ?? 0) - (SQLLogFileMB ?? 0));
                else
                    return nonSQLDiskUsageMB;
            }
        
            set { nonSQLDiskUsageMB = value; }
        }


        public long DiskIdlePercent
        {
            get { return diskIdlePercent; }
            set { diskIdlePercent = value; }
        }

        public long AverageDiskQueueLength
        {
            get { return averageDiskQueueLength; }
            set { averageDiskQueueLength = value; }
        }

        public long AverageDiskMillisecondsPerRead
        {
            get { return averageDiskMillisecondsPerRead; }
            set { averageDiskMillisecondsPerRead = value; }
        }

        public long AverageDiskMillisecondsPerTransfer
        {
            get { return averageDiskMillisecondsPerTransfer; }
            set { averageDiskMillisecondsPerTransfer = value; }
        }

        public long AverageDiskMillisecondsPerWrite
        {
            get { return averageDiskMillisecondsPerWrite; }
            set { averageDiskMillisecondsPerWrite = value; }
        }

        public double DiskReadsPerSecond
        {
            get { return diskReadsPerSecond; }
            set { diskReadsPerSecond = value; }
        }

        public long DiskTransfersPerSecond
        {
            get { return diskTransfersPerSecond; }
            set { diskTransfersPerSecond = value; }
        }

        public double DiskWritesPerSecond
        {
            get { return diskWritesPerSecond; }
            set { diskWritesPerSecond = value; }
        }

        public DateTime? DatabaseSizeTime
        {
            get { return databaseSizeTime; }
            set { databaseSizeTime = value; }
        }

        #endregion

    }

}
