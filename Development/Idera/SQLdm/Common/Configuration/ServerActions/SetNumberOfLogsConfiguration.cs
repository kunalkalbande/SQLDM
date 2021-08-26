//------------------------------------------------------------------------------
// <copyright file="SetNumberOfLogsConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration.ServerActions
{
    using System;

    /// <summary>
    /// Set number of SQL Server logs
    /// </summary>
    [Serializable]
    public sealed class SetNumberOfLogsConfiguration : OnDemandConfiguration, IServerActionConfiguration
    {
        #region fields

        private bool setUnlimited = false;
        private int? numberOfLogs = null;

        #endregion

        #region constructors

        public SetNumberOfLogsConfiguration(int monitoredServerId, bool setUnlimited)
            : base(monitoredServerId)
        {
            this.setUnlimited = setUnlimited;
        }

        public SetNumberOfLogsConfiguration(int monitoredServerId, int? numberOfLogs)
            : base(monitoredServerId)
        {
            this.numberOfLogs = numberOfLogs;
        }

        public SetNumberOfLogsConfiguration(int monitoredServerId, bool setUnlimited, int? numberOfLogs) : base(monitoredServerId)
        {
            this.setUnlimited = setUnlimited;
            this.numberOfLogs = numberOfLogs;
        }

        #endregion

        #region properties

        /// <summary>
        /// Boolean to set unlimited.  Set to true to set unlimited. Setting to false has no effect.
        /// Overrides NumberOfLogs
        /// </summary>
        public bool SetUnlimited
        {
            get { return setUnlimited; }
            set { setUnlimited = value; }
        }

        /// <summary>
        /// Integer to set limited. 
        /// </summary>
        public int? NumberOfLogs
        {
            get { return numberOfLogs; }
            set { numberOfLogs = value; }
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
