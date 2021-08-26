//------------------------------------------------------------------------------
// <copyright file="TimeZoneStruct.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.TimeZone
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Structure which contains the equivalent of the 
    /// TIME_ZONE_INFORMATION structure in the Windows API.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct TimeZoneStruct
    {
        [MarshalAs(UnmanagedType.I4)]
        public Int32 Bias;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string StandardName;
        public SystemTime StandardDate;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 StandardBias;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DaylightName;
        public SystemTime DaylightDate;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 DaylightBias;


        public override bool Equals(object obj)
        {
            //try
            //{
            if (obj == null)
                return false;
            if (this.GetType() != obj.GetType())
                return false;

            TimeZoneStruct theTZS = (TimeZoneStruct)obj;
            if (
                (theTZS.Bias == this.Bias) &&
                (theTZS.DaylightBias == this.DaylightBias) &&
                (theTZS.DaylightDate == this.DaylightDate) &&
                (theTZS.StandardBias == this.StandardBias) &&
                (theTZS.StandardDate == this.StandardDate)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
            //}
            //catch
            //{
            //return false;
            //}
        }

        public override int GetHashCode()
        {
            return this.Bias; // Not sure this is a good idea but...
        }
    }
}
