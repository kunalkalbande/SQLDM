//------------------------------------------------------------------------------
// <copyright file="FileActivitySnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    [Serializable]
    public class FileActivityDisk: DiskDriveMinimal
    {
        #region fields

        private Dictionary<string, FileActivityFile> files = new Dictionary<string, FileActivityFile>();

        #endregion

        #region constructors

        #endregion

        #region properties

        public Dictionary<string, FileActivityFile> Files
        {
            get { return files; }
            internal set { files = value; }
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

    [Serializable]
    public class FileActivityFile : ICloneable
    {
        #region fields

        private string databaseName;
        private decimal? readsRaw;
        private decimal? writesRaw;
        private decimal? reads;
        private decimal? writes;
        private decimal? transfers;
        private double? readsPerSec;
        private double? writesPerSec;
        private double? transfersPerSec;
        private string filename;
        private string filepath;
        private FileActivityFileType fileType = FileActivityFileType.Unknown;
        private string driveName;
        private bool isOtherFiles; // ui specific


       
        #endregion

        #region constructors

        #endregion

        #region properties

        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        internal decimal? ReadsRaw
        {
            get { return readsRaw; }
            set { readsRaw = value; }
        }

        internal decimal? WritesRaw
        {
            get { return writesRaw; }
            set { writesRaw = value; }
        }

        public decimal? Reads
        {
            get { return reads; }
            internal set { reads = value; }
        }

        public decimal? Writes
        {
            get { return writes; }
            internal set { writes = value; }
        }

        public decimal? Transfers
        {
            get { return transfers; }
            internal set { transfers = value; }
        }

        public double? ReadsPerSec
        {
            get { return readsPerSec; }
            set { readsPerSec = value; }
        }

        public double? WritesPerSec
        {
            get { return writesPerSec; }
            set { writesPerSec = value; }
        }

        public double? TransfersPerSec
        {
            get { return transfersPerSec; }
            set { transfersPerSec = value; }
        }

        public string Filename
        {
            get { return filename; }
            set { 
                if (!String.IsNullOrEmpty(value))
                    filename = value.Trim(); 
            }
        }

        public FileActivityFileType FileType
        {
            get { return fileType; }
            set { fileType = value; }
        }

        public string Filepath
        {
            get { return filepath; }
            set { 
                if (!String.IsNullOrEmpty(value))
                    filepath = value.Trim(); 
            }
        }

        public string DriveName
        {
            get { return driveName; }
            set { driveName = value; }
        }


        public bool IsOtherFiles
        {
            get { return isOtherFiles; }
            set { isOtherFiles = value; } // please leave public - for UI
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

        #region nested types

        #endregion
    }

    [Serializable]
    public class TempdbFileActivity : FileActivityFile
    {
         // The following are only set for tempdb

        private FileSize fileSize;
        private FileSize userObjects;
        private FileSize internalObjects;
        private FileSize versionStore;
        private FileSize mixedExtents;
        private FileSize unallocatedSpace;

        public TempdbFileActivity()
        {
        }

        public TempdbFileActivity(FileActivityFile file)
        {
            SetValuesFromFileActivity(file);
        }

        public FileSize FileSize
        {
            get { return fileSize; }
            internal set { fileSize = value; }
        }

        public FileSize UserObjects
        {
            get { return userObjects; }
            internal set { userObjects = value; }
        }

        public FileSize InternalObjects
        {
            get { return internalObjects; }
            internal set { internalObjects = value; }
        }

        public FileSize VersionStore
        {
            get { return versionStore; }
            internal set { versionStore = value; }
        }

        public FileSize MixedExtents
        {
            get { return mixedExtents; }
            internal set { mixedExtents = value; }
        }

        public FileSize UnallocatedSpace
        {
            get { return unallocatedSpace; }
            internal set { unallocatedSpace = value; }
        }

        internal void SetValuesFromFileActivity(FileActivityFile file)
        {
            if (file != null)
            {
                this.DatabaseName = file.DatabaseName;
                this.ReadsRaw = file.ReadsRaw;
                this.WritesRaw = file.WritesRaw;
                this.ReadsPerSec = file.ReadsPerSec;
                this.WritesPerSec = file.WritesPerSec;
                this.Filename = file.Filename;
                this.Filepath = file.Filepath;
                this.FileType = file.FileType;
                this.DriveName = file.DriveName;
                this.IsOtherFiles = file.IsOtherFiles;
            }
        }
    }

    /// <summary>
    /// Represents file activity data
    /// </summary>
    [Serializable]
    public class FileActivitySnapshot : Snapshot
    {
        #region fields

        private Dictionary<string, FileActivityDisk> drives = new Dictionary<string, FileActivityDisk>();
        private string osStatisticAvailability;

        private const string ServiceAvailableText = "available";
        private const string ServiceTimedOutText = "service timedout";
        private const string ServiceUnavailableText = "service unavailable";
        private const string ProcedureUnavailableText = "procedure unavailable";
        private const string LightweightPoolingText = "lightweight pooling";
        private const string UnavailableDueToLWPText = "unavailable due to lightweight pooling"; // LWP = Lightweight pooling
                    

        #endregion

        #region constructors

        public FileActivitySnapshot(SqlConnectionInfo info
            ) : base(info.InstanceName)
        {
        }

        #endregion

        #region properties

        public Dictionary<string, FileActivityDisk> Drives
        {
            get { return drives; }
            internal set { drives = value; }
        }

        public string OsStatisticAvailability
        {
            get { return osStatisticAvailability; }
            internal set { osStatisticAvailability = value; }
        }

        public OSMetricsStatus OsMetricsStatus
        {
            get
            {
                switch (OsStatisticAvailability)
                {
                    case ServiceAvailableText:
                        return OSMetricsStatus.Available;
                    case ServiceUnavailableText:
                        return OSMetricsStatus.WMIServiceUnreachable;
                    case ProcedureUnavailableText:
                        return OSMetricsStatus.OLEAutomationUnavailable;
                    case LightweightPoolingText:
                    case UnavailableDueToLWPText:
                        return OSMetricsStatus.UnavailableDueToLightweightPooling;
                    case ServiceTimedOutText:
                        return OSMetricsStatus.WMIServiceTimedOut;
                    default:
                        return OSMetricsStatus.Disabled;
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
