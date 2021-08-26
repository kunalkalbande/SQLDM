//------------------------------------------------------------------------------
// <copyright file="SystemTime.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.TimeZone
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A structure (similiar to the DateTime structure) that is used for 
    /// performing calculations of the differences it time between various
    /// time zones.  This is the equivalent of the SYSTEMTIME structure 
    /// in the Windows API.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SystemTime
    {
        [MarshalAs(UnmanagedType.U2)]
        public short Year;
        [MarshalAs(UnmanagedType.U2)]
        public short Month;
        [MarshalAs(UnmanagedType.U2)]
        public short DayOfWeek;
        [MarshalAs(UnmanagedType.U2)]
        public short Day;
        [MarshalAs(UnmanagedType.U2)]
        public short Hour;
        [MarshalAs(UnmanagedType.U2)]
        public short Minute;
        [MarshalAs(UnmanagedType.U2)]
        public short Second;
        [MarshalAs(UnmanagedType.U2)]
        public short Milliseconds;

        #region overrides
        public override bool Equals(object obj)
        {
            //try
            //{
            if (obj == null)
                return false;
            if (this.GetType() != obj.GetType())
                return false;

            SystemTime theST = (SystemTime)obj;
            if (
                (this.Year == theST.Year) &&
                (this.Month == theST.Month) &&
                (this.DayOfWeek == theST.DayOfWeek) &&
                (this.Day == theST.Day) &&
                (this.Hour == theST.Hour) &&
                (this.Minute == theST.Minute) &&
                (this.Second == theST.Second) &&
                (this.Milliseconds == theST.Milliseconds)
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
            //	return false;
            //}
        }

        public override int GetHashCode()
        {
            DateTime theDateTime = this.ToDateTime();
            return theDateTime.GetHashCode();
        }

        public static bool operator ==(SystemTime st1, SystemTime st2)
        {
            return (st1.Equals(st2));
        }

        public static bool operator !=(SystemTime st1, SystemTime st2)
        {
            return (!st1.Equals(st2));
        }

        #endregion

        #region external functions
        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern void GetLocalTime(out SystemTime localTime);
        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern void GetSystemTime(out SystemTime localTime);
        #endregion

        #region public instance methods

        /// <summary>
        /// Converts the SystemTime structure to a System.DateTime structure
        /// </summary>
        /// <returns></returns>
        public DateTime ToDateTime()
        {
            //construct a DateTime object from the SystemTime object that was passed in
            return SystemTime.ToDateTime(this);
        }

        #endregion

        #region static time retrieval functions

        /// <summary>
        /// Get the current time in Utc Format
        /// </summary>
        public static SystemTime UtcNow
        {
            get
            {
                //use the GetSystemTime function to return the current UTC time
                SystemTime sysTime;
                SystemTime.GetSystemTime(out sysTime);
                return sysTime;
            }
        }
        /// <summary>
        /// Get the current time in local format
        /// </summary>
        public static SystemTime Now
        {
            get
            {
                //use the GetLocalTime function to return the current local time
                SystemTime sysTime;
                SystemTime.GetLocalTime(out sysTime);
                return sysTime;
            }
        }

        #endregion

        #region static conversion function

        /// <summary>
        /// Converts a SystemTime structure to a System.DateTime structure
        /// </summary>
        /// <param name="time">The SystemTime structure to be converted</param>
        /// <returns>A System.DateTime structure that was created based on
        /// the value of the SystemTime structure.</returns>
        public static DateTime ToDateTime(SystemTime time)
        {
            //construct a DateTime object from the SystemTime object that was passed in
            return new DateTime(time.Year, time.Month, time.Day, time.Hour,
                time.Minute, time.Second, time.Milliseconds);
        }
        /// <summary>
        /// Converts a System.DateTime structure to a SystemTime structure
        /// </summary>
        /// <param name="time">System.DateTime structure to be converted</param>
        /// <returns>A SystemTime structure that was created based on
        /// the value of the System.DateTime structure.</returns>
        public static SystemTime FromDateTime(DateTime time)
        {
            //create the SystemTime object and populate it's values from the 
            //DateTime object that was passed in
            SystemTime sysTime;
            sysTime.Day = (short)time.Day;
            sysTime.DayOfWeek = (short)time.DayOfWeek;
            sysTime.Hour = (short)time.Hour;
            sysTime.Milliseconds = (short)time.Millisecond;
            sysTime.Minute = (short)time.Minute;
            sysTime.Month = (short)time.Month;
            sysTime.Second = (short)time.Second;
            sysTime.Year = (short)time.Year;
            return sysTime;
        }

        /// <summary>
        /// Converts a SystemTime structure to a System.DateTime structure
        /// </summary>
        /// <param name="time">The SystemTime structure to be converted</param>
        /// <returns>A System.DateTime structure that was created based on
        /// the value of the SystemTime structure.</returns>
        public static DateTime ToTimeZoneDateTime(SystemTime time)
        {
            if (time.Year == 0 && time.Month == 0)
            {
                return DateTime.MinValue;
            }
            else
            {
                return new DateTime(time.Year + 2000,
                                     time.Month,
                                     time.Day,
                                     time.Hour,
                                      time.Minute,
                                      time.Second,
                                      time.DayOfWeek); // store day of the week
            }
        }
        /// <summary>
        /// Converts a System.DateTime structure to a SystemTime structure
        /// </summary>
        /// <param name="time">System.DateTime structure to be converted</param>
        /// <returns>A SystemTime structure that was created based on
        /// the value of the System.DateTime structure.</returns>
        public static SystemTime FromTimeZoneDateTime(DateTime time)
        {
            //create the SystemTime object and populate it's values from the 
            //DateTime object that was passed in
            SystemTime sysTime;

            if (time == DateTime.MinValue)
            {
                sysTime.Year = 0;
                sysTime.Month = 0;
                sysTime.Day = 0;
                sysTime.DayOfWeek = 0;
            }
            else
            {
                sysTime.Year = (short)(time.Year - 2000);
                sysTime.Month = (short)time.Month;
                sysTime.Day = (short)time.Day;
                sysTime.DayOfWeek = (short)time.Millisecond;
            }

            sysTime.Hour = 0;
            sysTime.Minute = 0;
            sysTime.Second = 0;
            sysTime.Milliseconds = 0;

            return sysTime;
        }


        #endregion
    }
}
