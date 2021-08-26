//------------------------------------------------------------------------------
// <copyright file="ReconfigurationConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration.ServerActions
{
    using System;
    using Idera.SQLdm.Common.Configuration.ServerActions;

    /// <summary>
    /// Configuration object to reconfigure a monitored SQL server
    /// </summary>
    [Serializable]
    public sealed class BlockedProcessThresholdConfiguration : OnDemandConfiguration, IServerActionConfiguration
    {
        #region fields

        private string configurationName = null;
        private int? runValue = null;

        #endregion

        #region constructors

        public BlockedProcessThresholdConfiguration(int monitoredServerId, int? runValue)
            : base(monitoredServerId)
        {
            this.runValue = runValue;
        }

        #endregion

        #region properties

        public string ConfigurationName
        {
            get { return configurationName; }
            set { configurationName = value; }
        }

        public int? RunValue
        {
            get { return runValue; }
            set { runValue = value; }
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
