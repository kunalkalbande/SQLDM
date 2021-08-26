//------------------------------------------------------------------------------
// <copyright file="AlertsInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.PowerShell.Objects
{
    using System.Collections.Generic;
    using Helpers;

    public class AlertsInfo : StaticContainerInfo
    {
        internal const string ContainerName = "Alerts";
        internal const string ContainerNameLower = "alerts";
        
        public readonly string instanceName;

        internal AlertsInfo(SQLdmDriveInfo drive, string instanceName) : base(drive, ContainerName)
        {
            this.instanceName = instanceName;
        }

        public IList<AlertInfo> GetActiveAlerts()
        {
            return Helper.GetActiveAlerts(Drive, instanceName);
        }
    }
}
