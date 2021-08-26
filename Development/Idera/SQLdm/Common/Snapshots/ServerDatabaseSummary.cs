//------------------------------------------------------------------------------
// <copyright file="ServerDatabaseSummary.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the top-level summary information for the databases on a server
    /// </summary>
    [Serializable]
    public sealed class ServerDatabaseSummary
    {
        #region fields

        private int? databaseCount = null;
        private int? dataFileCount = null;
        private int? logFileCount = null;
        private FileSize dataFileSpaceAllocated = new FileSize();
        private FileSize dataFileSpaceUsed = new FileSize();
        private FileSize logFileSpaceAllocated = new FileSize();
        private FileSize logFileSpaceUsed = new FileSize();

        //START: SQLdm 10.0 (Tarun Sapra)- baseline mean and per of baseline
        private double? dataUsedPctBaselineMean = null;
        private double? dataUsedPctAsBaselinePerc = null;

        private double? logUsedPctBaselineMean = null;
        private double? logUsedPctAsBaselinePerc = null;
        //END: SQLdm 10.0 (Tarun Sapra)- baseline mean and per of baseline
        #endregion

        #region constructors

        #endregion

        #region properties

        //START: SQLdm 10.0 (Tarun Sapra)- baseline mean and per of baseline
        public double? DataUsedPctBaselineMean
        {
            get { return dataUsedPctBaselineMean; }
            set { dataUsedPctBaselineMean = value; }
        }
        public double? DataUsedPctAsBaselinePerc
        {
            get { return dataUsedPctAsBaselinePerc; }
            set { dataUsedPctAsBaselinePerc = value; }
        }

        public double? LogUsedPctBaselineMean
        {
            get { return logUsedPctBaselineMean; }
            set { logUsedPctBaselineMean = value; }
        }
        public double? LogUsedPctAsBaselinePerc
        {
            get { return logUsedPctAsBaselinePerc; }
            set { logUsedPctAsBaselinePerc = value; }
        }
        //END: SQLdm 10.0 (Tarun Sapra)- baseline mean and per of baseline

        /// <summary>
        /// Total number of databases on server (max: 32,767)
        /// </summary>
        public int? DatabaseCount
        {
            get { return databaseCount; }
            set { databaseCount = value; }
        }

        /// <summary>
        /// Total number of data files on server (max:  1,073,676,289 )
        /// </summary>
        public int? DataFileCount
        {
            get { return dataFileCount; }
            set { dataFileCount = value; }
        }

        /// <summary>
        /// Total number of data files on server (max:  1,073,676,289 )
        /// </summary>
        public int? LogFileCount
        {
            get { return logFileCount; }
            set { logFileCount = value; }
        }

        /// <summary>
        /// Total data file space allocated across all databases (max: 32 TB per file, capping at 1,048,516 TB per database)
        /// </summary>
        public FileSize DataFileSpaceAllocated
        {
            get { return dataFileSpaceAllocated; }
            set { dataFileSpaceAllocated = value; }
        }

        /// <summary>
        /// Total data file space used across all databases (max: 32 TB per file, capping at 1,048,516 TB per database)
        /// </summary>
        public FileSize DataFileSpaceUsed
        {
            get { return dataFileSpaceUsed; }
            set { dataFileSpaceUsed = value; }
        }

        /// <summary>
        /// Total log file space allocated across all databases (max: 32 TB per file)
        /// </summary>
        public FileSize LogFileSpaceAllocated
        {
            get { return logFileSpaceAllocated; }
            set { logFileSpaceAllocated = value; }
        }

        /// <summary>
        /// Total log file space used across all databases (max: 32 TB per file)
        /// </summary>
        public FileSize LogFileSpaceUsed
        {
            get { return logFileSpaceUsed; }
            set { logFileSpaceUsed = value; }
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
