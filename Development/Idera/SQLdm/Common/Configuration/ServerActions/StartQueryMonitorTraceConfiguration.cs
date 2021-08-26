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
    public class StartQueryMonitorTraceConfiguration: OnDemandConfiguration, IServerActionConfiguration
    {
        public readonly QueryMonitorConfiguration CurrentQMConfig;
        public readonly QueryMonitorConfiguration PreviousQMConfig;

        public StartQueryMonitorTraceConfiguration(int monitoredServerId,
                                                    QueryMonitorConfiguration currentConfig,
                                                    QueryMonitorConfiguration previousConfig)  : base(monitoredServerId)
        {
            this.CurrentQMConfig = currentConfig;
            this.PreviousQMConfig = previousConfig;
        }
    }
}
