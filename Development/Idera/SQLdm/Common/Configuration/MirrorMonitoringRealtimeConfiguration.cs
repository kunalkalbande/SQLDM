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
    public sealed class MirrorMonitoringRealtimeConfiguration : OnDemandConfiguration
    {
        #region fields


        #endregion

        #region constructors

        public MirrorMonitoringRealtimeConfiguration(int monitoredServerId)
            : base(monitoredServerId)
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
