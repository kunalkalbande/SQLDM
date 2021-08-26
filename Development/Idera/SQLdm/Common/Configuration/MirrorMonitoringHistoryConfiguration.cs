//------------------------------------------------------------------------------
// <copyright file="DatabaseMirroringConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration object for DatabaseMirroring views
    /// </summary>
    [Serializable]
    public sealed class MirrorMonitoringHistoryConfiguration : OnDemandConfiguration
    {
        #region fields

        private string _Database;
        private int _Mode;

        #endregion

        #region constructors

        public MirrorMonitoringHistoryConfiguration(int monitoredServerId, string DatabaseName, int Mode)
            : base(monitoredServerId)
        {
            _Database = DatabaseName;
            _Mode = Mode;
        }

        #endregion

        #region properties

        public string MirroredDatabaseName
        {
            get { return _Database; }
            set { _Database = value; }
        }
        
        public int Mode
        {
            get { return _Mode; }
            set { _Mode = value; }
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
