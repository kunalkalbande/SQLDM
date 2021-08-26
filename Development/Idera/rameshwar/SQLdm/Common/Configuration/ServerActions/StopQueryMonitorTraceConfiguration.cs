//------------------------------------------------------------------------------
// <copyright file="StopQueryMonitorTraceConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration.ServerActions
{
    using System;

    /// <summary>
    /// Stop the query monitor trace for a given
    /// </summary>
    [Serializable]
    public class StopQueryMonitorTraceConfiguration : OnDemandConfiguration, IServerActionConfiguration
    {
        #region fields

        #endregion

        #region constructors

        public StopQueryMonitorTraceConfiguration(int monitoredServerId) : base(monitoredServerId)
        {
        }

        #endregion

        #region properties

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
