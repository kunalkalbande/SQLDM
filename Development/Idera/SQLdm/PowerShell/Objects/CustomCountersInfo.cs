//------------------------------------------------------------------------------
// <copyright file="CustomCountersInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell.Objects
{
    using Helpers;
    using Idera.SQLdm.Common.Events;
    using Wintellect.PowerCollections;

    internal class CustomCountersInfo : StaticContainerInfo
    {

        internal CustomCountersInfo(SQLdmDriveInfo drive) : base(drive, "CustomCounters")
        {
        }

        public IList<CustomCounterInfo> Counters
        {
            get
            {
                return Algorithms.ReadOnly(Helper.GetCustomCounters(Drive, null));
            }
        }

    }
}
