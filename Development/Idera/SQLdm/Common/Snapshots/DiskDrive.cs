//------------------------------------------------------------------------------
// <copyright file="DiskDrive.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{

    using System;

    [Serializable]
    public class DiskDriveMinimal
    {
        string driveLetter = null;

        private double? diskReadsPerSec = null;
        private double? diskTransfersPerSec = null;
        private double? diskWritesPerSec = null;

        private double? diskReadsPerSec_Raw = null;
        private double? diskTransfersPerSec_Raw = null;
        private double? diskWritesPerSec_Raw = null;
        private double? timeStamp_PerfTime = null;
        private double? frequency_Perftime = null;

        public string DriveLetter
        {
            get { return driveLetter; }
            set { driveLetter = value; } // needed by ui
        }

        public double? DiskReadsPerSec
        {
            get { return diskReadsPerSec; }
            set { diskReadsPerSec = value; }
        }

        public double? DiskTransfersPerSec
        {
            get { return diskTransfersPerSec; }
            set { diskTransfersPerSec = value; }
        }

        public double? DiskWritesPerSec
        {
            get { return diskWritesPerSec; }
            set { diskWritesPerSec = value; }
        }

        internal double? DiskReadsPerSec_Raw
        {
            get { return diskReadsPerSec_Raw; }
            set { diskReadsPerSec_Raw = value; }
        }

        internal double? DiskTransfersPerSec_Raw
        {
            get { return diskTransfersPerSec_Raw; }
            set { diskTransfersPerSec_Raw = value; }
        }

        internal double? DiskWritesPerSec_Raw
        {
            get { return diskWritesPerSec_Raw; }
            set { diskWritesPerSec_Raw = value; }
        }

        internal double? TimeStamp_PerfTime
        {
            get { return timeStamp_PerfTime; }
            set { timeStamp_PerfTime = value; }
        }

        internal double? Frequency_Perftime
        {
            get { return frequency_Perftime; }
            set { frequency_Perftime = value; }
        }
    }

    /// <summary>
    /// Represents a physical disk drive
    /// </summary>
    [Serializable]
    public sealed class DiskDrive: DiskDriveMinimal
    {
        #region fields


        private FileSize unusedSize = new FileSize();
        private FileSize totalSize = new FileSize();
        private Int64? diskIdlePercent = null;
        private Int64? averageDiskQueueLength = null;
        private double? diskIdlePercentRaw = null;
        private double? diskIdlePercentBaseRaw = null;
        private double? averageDiskQueueLengthRaw = null;
        private double? timestamp_sys100ns = null;
        private DateTime? timestamp_utc = null;

        private TimeSpan? avgDiskSecPerRead = null;
        private TimeSpan? avgDiskSecPerTransfer = null;
        private TimeSpan? avgDiskSecPerWrite = null;

        private double? avgDisksecPerReadRaw = null;
        private double? avgDisksecPerRead_Base = null;
        private double? avgDisksecPerTransferRaw = null;
        private double? avgDisksecPerTransfer_Base = null;
        private double? avgDisksecPerWriteRaw = null;
        private double? avgDisksecPerWrite_Base = null;
        

        //sql areas
        private FileSize sqlUsedSize = new FileSize();
        private FileSize sqlUnusedSize = new FileSize();
        private FileSize sqlLogSize = new FileSize();

        #endregion

        #region constructors

        #endregion

        #region properties

        

        public FileSize UnusedSize
        {
            get { return unusedSize; }
            internal set { unusedSize = value; }
        }

        public FileSize TotalSize
        {
            get { return totalSize; }
            internal set { totalSize = value; }
        }


        public long? DiskIdlePercent
        {
            get { return diskIdlePercent; }
            set { diskIdlePercent = value.HasValue ? Math.Min(value.Value, 100) : value; }
        }

        public long? DiskBusyPercent
        {
            get
            {
                if (DiskIdlePercent.HasValue)
                {
                    return (100 - diskIdlePercent);
                }
                else
                {
                    return null;
                }
            }
        }

        public long? AverageDiskQueueLength
        {
            get { return averageDiskQueueLength; }
            set { averageDiskQueueLength = value; }
        }


        internal double? DiskIdlePercentRaw
        {
            get { return diskIdlePercentRaw; }
            set { diskIdlePercentRaw = value; }
        }

        internal double? DiskIdlePercentBaseRaw
        {
            get { return diskIdlePercentBaseRaw; }
            set { diskIdlePercentBaseRaw = value; }
        }

        internal double? AverageDiskQueueLengthRaw
        {
            get { return averageDiskQueueLengthRaw; }
            set { averageDiskQueueLengthRaw = value; }
        }
        
        internal double? Timestamp_Sys100ns
        {
            get { return timestamp_sys100ns; }
            set { timestamp_sys100ns = value; }
        }

        internal DateTime? Timestamp_utc
        {
            get { return timestamp_utc; }
            set { timestamp_utc = value; }
        }

        public FileSize UsedSize
        {
            get
            {
                if (TotalSize.Kilobytes.HasValue && UnusedSize.Kilobytes.HasValue)
                {
                    return new FileSize(TotalSize.Kilobytes.Value - UnusedSize.Kilobytes.Value);
                }
                else
                {
                    return new FileSize();
                }
            }
        }

        public float? PercentUsed
        {
            get
            {
                if (TotalSize.Kilobytes.HasValue && TotalSize.Kilobytes.Value > 0 && UnusedSize.Kilobytes.HasValue)
                {
                    return (float)(UsedSize.Kilobytes.Value / TotalSize.Kilobytes.Value);
                }
                else
                {
                    return null;
                }
            }
        }

        public float? PercentUnused
        {
            get
            {
                if (PercentUsed.HasValue)
                {
                    return 1 - PercentUsed;
                }
                else
                {
                    return null;
                }
            }
        }

        public TimeSpan? AvgDiskSecPerRead
        {
            get { return avgDiskSecPerRead; }
            set { avgDiskSecPerRead = value; }
        }

        public TimeSpan? AvgDiskSecPerTransfer
        {
            get { return avgDiskSecPerTransfer; }
            set { avgDiskSecPerTransfer = value; }
        }

        public TimeSpan? AvgDiskSecPerWrite
        {
            get { return avgDiskSecPerWrite; }
            set { avgDiskSecPerWrite = value; }
        }

        internal double? Timestamp_sys100ns
        {
            get { return timestamp_sys100ns; }
            set { timestamp_sys100ns = value; }
        }

        internal double? AvgDisksecPerReadRaw
        {
            get { return avgDisksecPerReadRaw; }
            set { avgDisksecPerReadRaw = value; }
        }

        internal double? AvgDisksecPerRead_Base
        {
            get { return avgDisksecPerRead_Base; }
            set { avgDisksecPerRead_Base = value; }
        }

        internal double? AvgDisksecPerTransferRaw
        {
            get { return avgDisksecPerTransferRaw; }
            set { avgDisksecPerTransferRaw = value; }
        }

        internal double? AvgDisksecPerTransfer_Base
        {
            get { return avgDisksecPerTransfer_Base; }
            set { avgDisksecPerTransfer_Base = value; }
        }

        internal double? AvgDisksecPerWriteRaw
        {
            get { return avgDisksecPerWriteRaw; }
            set { avgDisksecPerWriteRaw = value; }
        }

        internal double? AvgDisksecPerWrite_Base
        {
            get { return avgDisksecPerWrite_Base; }
            set { avgDisksecPerWrite_Base = value; }
        }

                
        //sql areas
        public FileSize SqlUsedSize
        {
            get { return sqlUsedSize; }
            internal set { sqlUsedSize = value; }
        }

        public FileSize SqlUnusedSize
        {
            get { return sqlUnusedSize; }
            internal set { sqlUnusedSize = value; }
        }

        public FileSize SqlLogSize
        {
            get { return sqlLogSize; }
            internal set { sqlLogSize = value; }
        }

        public FileSize OtherFilesSize
        {
            get
            {
                return new FileSize(TotalSize.Kilobytes - UnusedSize.Kilobytes
                                    - (SqlUsedSize.Kilobytes.HasValue ? sqlUsedSize.Kilobytes.Value : 0)
                                    - (SqlUnusedSize.Kilobytes.HasValue ? SqlUnusedSize.Kilobytes.Value : 0)
                                    - (SqlLogSize.Kilobytes.HasValue ? SqlLogSize.Kilobytes.Value : 0)
                    );
            }
        }

        public float? PercentSqlUsed
        {
            get
            {
                if (TotalSize.Kilobytes.HasValue && TotalSize.Kilobytes.Value > 0 && SqlUsedSize.Kilobytes.HasValue)
                {
                    return (float)(SqlUsedSize.Kilobytes.Value / TotalSize.Kilobytes.Value);
                }
                else
                {
                    return null;
                }
            }
        }

        public float? PercentSqlUnused
        {
            get
            {
                if (TotalSize.Kilobytes.HasValue && TotalSize.Kilobytes.Value > 0 && SqlUnusedSize.Kilobytes.HasValue)
                {
                    return (float)(SqlUnusedSize.Kilobytes.Value / TotalSize.Kilobytes.Value);
                }
                else
                {
                    return null;
                }
            }
        }

        public float? PercentSqlLog
        {
            get
            {
                if (TotalSize.Kilobytes.HasValue && TotalSize.Kilobytes.Value > 0 && SqlLogSize.Kilobytes.HasValue)
                {
                    return (float)(SqlLogSize.Kilobytes.Value / TotalSize.Kilobytes.Value);
                }
                else
                {
                    return null;
                }
            }
        }

        public float? PercentOther
        {
            get
            {
                if (TotalSize.Kilobytes.HasValue && TotalSize.Kilobytes.Value > 0 && OtherFilesSize.Kilobytes.HasValue)
                {
                    return (float)(OtherFilesSize.Kilobytes.Value / TotalSize.Kilobytes.Value);
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion
    }
}
