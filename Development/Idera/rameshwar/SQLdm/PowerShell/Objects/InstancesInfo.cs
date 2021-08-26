//------------------------------------------------------------------------------
// <copyright file="InstancesInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell.Objects
{
    using System.Collections.Generic;
    using Helpers;
    using Wintellect.PowerCollections;

    public class InstancesInfo : StaticContainerInfo
    {
        internal const string ContainerName = "Instances";
        internal const string ContainerNameLower = "instances";

        internal InstancesInfo(SQLdmDriveInfo drive) : base(drive, ContainerName)
        {
        }

        internal IList<MonitoredSqlServerInfo> GetInstances(ProgressProvider progress)
        {
            return Algorithms.ReadOnly(Helper.GetInstances(Drive, null, progress));
        }

        public IList<MonitoredSqlServerInfo> Instances
        {
            get
            {
                return GetInstances(null);
            } 
        }
    }
}
