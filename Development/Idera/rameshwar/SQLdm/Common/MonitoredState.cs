//------------------------------------------------------------------------------
// <copyright file="MonitoredState.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common
{
    using System;

    /// <summary>
    /// This enumeration represents the possible state of all monitored objects and values.
    /// </summary>
    public enum MonitoredState : byte
    {
        None = MonitoredStateFlags.None,
        OK = MonitoredStateFlags.OK,
        Informational = MonitoredStateFlags.Informational,
        Warning = MonitoredStateFlags.Warning,
        Critical = MonitoredStateFlags.Critical,
    }
}
