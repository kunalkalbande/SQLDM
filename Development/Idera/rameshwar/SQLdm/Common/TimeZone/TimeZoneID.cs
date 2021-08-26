//------------------------------------------------------------------------------
// <copyright file="TimeZoneID.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.TimeZone
{
    /// <summary>
    /// Used for checking the return value of the GetTimeZoneInformation Windows API call
    /// </summary>
    internal enum TimeZoneID
    {
        Unknown = 0,
        Standard = 1,
        Daylight = 2,
        Invalid = unchecked((int)0xFFFFFFFF)
    }
}
