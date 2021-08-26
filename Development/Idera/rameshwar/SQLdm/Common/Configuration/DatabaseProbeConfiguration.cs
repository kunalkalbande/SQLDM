//------------------------------------------------------------------------------
// <copyright file="DatabaseProbeConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Base class for database on-demand probe configurations
    /// </summary>
    [Serializable]
    public class DatabaseProbeConfiguration : OnDemandConfiguration
    {
        #region fields

        private bool includeSystemDatabases = false;
        private string databaseNameFilter = null;

        #endregion

        #region constructors

        public DatabaseProbeConfiguration(int monitoredServerId) : base(monitoredServerId)
        {
        }

        public DatabaseProbeConfiguration(int monitoredServerId, bool includeSystemDatabases) : base(monitoredServerId)
        {
            this.includeSystemDatabases = includeSystemDatabases;
        }

        #endregion

        #region properties


        public string DatabaseNameFilter
        {
            get { return databaseNameFilter; }
            set { databaseNameFilter = value; }
        }

        public bool IncludeSystemDatabases
        {
            get { return includeSystemDatabases; }
            set { includeSystemDatabases = value; }
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
