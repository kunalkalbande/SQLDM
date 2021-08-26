//------------------------------------------------------------------------------
// <copyright file="StartQueryMonitorTraceConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.Common.Configuration.ServerActions
{
    [Serializable]
    public class StartActivityMonitorTraceConfiguration : OnDemandConfiguration, IServerActionConfiguration
    {
        public readonly ActivityMonitorConfiguration CurrentActivityMonitorConfig;
        public readonly ActivityMonitorConfiguration PreviousActivityMonitorConfig;

        public StartActivityMonitorTraceConfiguration(int monitoredServerId,
                                                    ActivityMonitorConfiguration currentConfig,
                                                    ActivityMonitorConfiguration previousConfig)
            : base(monitoredServerId)
        {
            this.CurrentActivityMonitorConfig = currentConfig;
            this.PreviousActivityMonitorConfig = previousConfig;
        }
    }
}