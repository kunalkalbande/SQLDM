//------------------------------------------------------------------------------
// <copyright file="MonitoredStateFlags.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common
{
    using System;

    /// <summary>
    /// Flags used to determine what states we're interested in.
    /// </summary>
    [Flags]
    public enum MonitoredStateFlags : byte
    {
        None = 0x00,
        OK = 0x01,
        Informational = 0x02,
        Warning = 0x04,
        Critical = 0x08,
        All = 0xFF
    }
}
