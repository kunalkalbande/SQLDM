//------------------------------------------------------------------------------
// <copyright file="StopSessionDetailsTraceConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration.ServerActions
{
    using System;

    /// <summary>
    /// Stop the session details trace for a given client session
    /// </summary>
    [Serializable]
    public sealed class StopSessionDetailsTraceConfiguration : OnDemandConfiguration, IServerActionConfiguration
    {
        #region fields

        #endregion

        #region constructors

        public StopSessionDetailsTraceConfiguration(int monitoredServerId, Guid sessionId) : base(monitoredServerId)
        {
            ClientSessionId = sessionId;
        }

        public StopSessionDetailsTraceConfiguration(SessionDetailsConfiguration config)
            : this(config.MonitoredServerId, config.ClientSessionId)
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
