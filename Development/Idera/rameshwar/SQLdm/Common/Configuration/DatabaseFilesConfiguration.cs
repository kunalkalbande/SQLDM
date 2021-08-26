//------------------------------------------------------------------------------
// <copyright file="DatabaseFilesConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration object for database files on-demand probe
    /// </summary>
    [Serializable]
    public sealed class DatabaseFilesConfiguration : OnDemandConfiguration
    {
        #region fields

        private List<string> databaseNames = new List<string>();
        private Dictionary<string, DiskDrive> previousDiskDrives = null; 

        #endregion

        #region constructors

        public DatabaseFilesConfiguration(int monitoredServerId, List<string> databaseNames)
            : base(monitoredServerId)
        {
            this.databaseNames = databaseNames;
        }

        #endregion

        #region properties

        /// <summary>
        /// Databases to filter on
        /// </summary>
        public List<string> DatabaseNames
        {
            get { return databaseNames; }
            set { databaseNames = value; }
        }

        public Dictionary<string, DiskDrive> PreviousDiskDrives
        {
            get { return previousDiskDrives; }
            set { previousDiskDrives = value; }
        }

        public TempdbStatistics PreviousTempdbStatistics { get; set; }

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
