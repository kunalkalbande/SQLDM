//------------------------------------------------------------------------------
// <copyright file="TimeZoneInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.TimeZone
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using Microsoft.Win32;

    /// <summary>
    /// Class which holds information about a TimeZone. Both a name and 
    /// a TimeZoneStruct are included.
    /// </summary>
    public class TimeZoneInfo : IComparable
    {
        private string tziName;
        private TimeZoneStruct tziStruct;

        public TimeZoneInfo()
        {
            tziName = "";
            tziStruct = new TimeZoneStruct();
        }

        public string Name
        {
            get { return tziName; }
            set { tziName = value; }
        }

        public TimeZoneStruct TimeZoneStruct
        {
            get { return tziStruct; }
            set { 
                tziStruct = value;
            }
        }

        #region overrides
        public override string ToString()
        {
            return tziName;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (this.GetType() != obj.GetType())
                return false;

            TimeZoneInfo theTZI = (TimeZoneInfo)obj;
            if (
                (theTZI.tziName == this.tziName) &&
                (theTZI.tziStruct.Bias == this.tziStruct.Bias) &&
                (theTZI.tziStruct.DaylightBias == this.tziStruct.DaylightBias) &&
                (theTZI.tziStruct.DaylightDate == this.tziStruct.DaylightDate) &&
                (theTZI.tziStruct.StandardBias == this.tziStruct.StandardBias) &&
                (theTZI.tziStruct.StandardDate == this.tziStruct.StandardDate)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.tziStruct.Bias;
        }

        public static bool operator ==(TimeZoneInfo tzi1, TimeZoneInfo tzi2)
        {
            return (tzi1.Equals(tzi2));
        }

        public static bool operator !=(TimeZoneInfo tzi1, TimeZoneInfo tzi2)
        {
            return (!tzi1.Equals(tzi2));
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            if (!(obj is TimeZoneInfo))
            {
                throw new ArgumentException();
            }

            return ((TimeZoneInfo)obj).tziStruct.Bias - this.tziStruct.Bias;
        }

        #endregion

        #region external functions
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern TimeZoneID GetTimeZoneInformation(out TimeZoneStruct tzi);
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern bool SystemTimeToTzSpecificLocalTime([In] ref TimeZoneStruct tzOfInterest,
            [In] ref SystemTime Utc, out SystemTime timeOfInterest);
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern bool TzSpecificLocalTimeToSystemTime([In] ref TimeZoneStruct sourceTimeZone,
            [In] ref SystemTime sourceTimeLocal, out SystemTime Utc);
        #endregion

        #region static prop & method to get some useful info
        /// <summary>
        /// Retrieves the TimeZoneInfo structure based on the current system TimeZone.
        /// </summary>
        public static TimeZoneInfo CurrentTimeZone
        {
            get
            {
                //first we must convert the LocalTime SystemTime structure to a Utc time so that
                //we can adjust to another TimeZone
                TimeZoneInfo localTz = new TimeZoneInfo();
                if (TimeZoneInfo.GetTimeZoneInformation(out localTz.tziStruct) == TimeZoneID.Invalid)
                {
                    throw new SystemException("Error occurred.");
                }
                else
                {
                    return localTz;
                }
            }
        }

        public static TimeZoneInfo GetTimeZone(string name)
        {
            foreach (TimeZoneInfo tzi in GetSystemTimeZones())
            {
                if (tzi.Name == name)
                {
                    return tzi;
                }
            }
            return null;
        }



        /// <summary>
        /// Retrieves an array of TimeZoneInfo structures by pulling values from the 
        /// Registry.
        /// </summary>
        /// <returns>Array of TimeZoneInfo structures</returns>
        public static TimeZoneInfo[] GetSystemTimeZones()
        {
            //get a reference to the key that contains all of the timezone information
            //on the current machine
            RegistryKey rootKey = null;

            try
            {

                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    rootKey = Registry.LocalMachine;
                    rootKey = rootKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones\");
                }
                else
                {
                    if (Environment.OSVersion.Platform == PlatformID.Win32Windows)
                    {
                        rootKey = Registry.LocalMachine;
                        rootKey = rootKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Time Zones\");
                    }
                    else
                    {
                        throw new PlatformNotSupportedException();
                    }
                }

                //store the subkey names as an array of strings
                string[] zones = rootKey.GetSubKeyNames();

                //create the array TimeZoneInfo structs to the same count as the timezone keys
                TimeZoneInfo[] tzInfos = new TimeZoneInfo[zones.Length];

                Type tziType = typeof(TZI);
                int tziSize = Marshal.SizeOf(tziType);
                for (int i = 0; i < zones.Length; i++)
                {
                    RegistryKey zoneKey = null;
                    string display;
                    string sName;
                    string dName;
                    byte[] bytes;

                    try
                    {
                        zoneKey = rootKey.OpenSubKey(zones[i]);
                        display = zoneKey.GetValue("Display").ToString();
                        sName = zoneKey.GetValue("Std").ToString();
                        dName = zoneKey.GetValue("Dlt").ToString();

                        bytes = zoneKey.GetValue("TZI") as byte[];
                    }
                    finally
                    {
                        if (zoneKey != null)
                            zoneKey.Close();
                    }
                    if (bytes == null)
                        continue;
                    if (bytes.Length < tziSize)
                        continue;

                    GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                    IntPtr buffer = handle.AddrOfPinnedObject();
                    TZI tzi = (TZI)Marshal.PtrToStructure(buffer, tziType);
                    handle.Free();

                    //construct a new TimeZoneInfo class and populate the members with the
                    //values we've pulled from the registry and add it to the array of
                    //TimeZoneInfo classes that we are going to be returning
                    TimeZoneInfo tzInfo = new TimeZoneInfo();
                    tzInfo.tziName = display;
                    tzInfo.tziStruct.Bias = tzi.Bias;
                    tzInfo.tziStruct.DaylightBias = tzi.DaylightBias;
                    tzInfo.tziStruct.DaylightDate = tzi.DaylightDate;
                    tzInfo.tziStruct.StandardBias = tzi.StandardBias;
                    tzInfo.tziStruct.StandardDate = tzi.StandardDate;
                    tzInfo.tziStruct.DaylightName = dName;
                    tzInfo.tziStruct.StandardName = sName;

                    //now add to array
                    tzInfos.SetValue(tzInfo, i);
                }

                //return the TimeZoneInfo array that we just constructed
                return tzInfos;
            }
            finally
            {
                if (rootKey != null)
                    rootKey.Close();
            }
        }

        #endregion

        #region static conversion functions

        /// <summary>
        /// Converts the local time of the source time-zone to the local time of the
        /// destination time-zone.
        /// </summary>
        /// <param name="source">The source time-zone</param>
        /// <param name="destination">The destination time-zone</param>
        /// <param name="sourceLocalTime">The local time of the source time-zone that is
        /// to be converted to the local time in the destination time-zone</param>
        /// <returns>The local time in the destination time-zone.</returns>
        public static DateTime Convert(TimeZoneInfo source, TimeZoneInfo destination,
            DateTime sourceLocalTime)
        {
            //since we now have the UtcTime, we can forward the call to the ConvertUtcTimeZone
            //method and return that functions return value
            return TimeZoneInfo.ToLocalTime(destination,
                TimeZoneInfo.ToUniversalTime(source, sourceLocalTime));
        }
        /// <summary>
        /// Converts the UtcTime to the local time of the destination time-zone.
        /// </summary>
        /// <param name="destination">The destination time-zone</param>
        /// <param name="utcTime">Utc time that is to be converted to the local time of
        /// the destination time-zone.</param>
        /// <returns>DateTime that represents the local time in the destination time-zone</returns>
        public static DateTime ToLocalTime(TimeZoneInfo destination, DateTime utcTime)
        {
            if (utcTime == DateTime.MinValue)
                return utcTime;

            //since the SystemTimeToTzSpecificLocalTime is only available in NT+
            //we are going to have to have to calculate the conversion ourselves
            //for the folks with the Windows9x/Me OS
            //NOTE: See note regarding performing the calculation ourselves in the 
            //ToUniversalTime static function
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                //call the function to convert the Utc System time to the local time of the 
                //TimeZone represented by the TimeZoneInfo structure passed in
                SystemTime specificTime;
                //get a SystemTime representation of the utcTime DateTime to
                //pass to the WinAPI function
                SystemTime stUtc = SystemTime.FromDateTime(utcTime);

                SystemTimeToTimeZoneLocalTime(ref destination.tziStruct, ref stUtc, out specificTime);
                //return the TimeZone specific local time that was calculated
                return specificTime.ToDateTime();
            }
            else
            {
                //first we must convert the utcTime to the local time without any regard to the
                //daylight saving issues.  We'll deal with that next
                utcTime = utcTime.AddMinutes(-destination.tziStruct.Bias);
                //now we must determine if the specified local time is during the daylight saving
                //time period.  If it is, then we add the Bias and Daylight bias to the value, otherwise
                //we add the Bias and Standard bias (I believe that StandardBias is always 0)				
                if (destination.IsDaylightSavingTime(utcTime))
                {
                    return utcTime.AddMinutes(-destination.tziStruct.DaylightBias);
                }
                else
                {
                    return utcTime.AddMinutes(-destination.tziStruct.StandardBias);
                }
            }
        }

        internal static void
           SystemTimeToTimeZoneLocalTime(
              ref TimeZoneStruct tzs,
              ref SystemTime utc,
              out SystemTime specificTime)
        {
            if (!TimeZoneInfo.SystemTimeToTzSpecificLocalTime(ref tzs, ref utc, out specificTime))
            {
                string msg = String.Format("Error occurred converting SystemTime to TimeZone specific local time.  Error code: {0}.",
                                            Marshal.GetLastWin32Error());
                //TODO: Figure out a better exception to throw here, just a placeholder now
                throw new SystemException(msg);
            }
        }

        internal static void
           TimeZoneLocalTimeToUtcTime(
              ref TimeZoneStruct tzs,
              ref SystemTime localTime,
              out SystemTime utcTime)
        {
            if (!TimeZoneInfo.TzSpecificLocalTimeToSystemTime(ref tzs, ref localTime, out utcTime))
            {
                string msg = String.Format("Error occurred converting TimeZone specific local time to UTC time.  Error code: {0}.",
                                            Marshal.GetLastWin32Error());
                //TODO: Figure out a better exception to throw here, just a placeholder now
                throw new SystemException(msg);
            }
        }
        /// <summary>
        /// Converts a local time of the source time-zone to a Utc time
        /// </summary>
        /// <param name="source">The source time-zone</param>
        /// <param name="sourceLocalTime">The local time in the source time-zone</param>
        /// <returns>The Utc time that is equivalent to the local time in the source time-zone.</returns>
        public static DateTime ToUniversalTime(TimeZoneInfo source, DateTime sourceLocalTime)
        {
            if (sourceLocalTime == DateTime.MinValue)
                return sourceLocalTime;

            //since the TzSpecificLocalTimeToSystemTime is only available in XP+ and 
            //.NET Server+, we are going to have to manually calculate the conversion
            //NOTE: The conversion that we perform is only going to be as accurace as the 
            //information contained in the source TimeZoneInfo structure.  I'm sure the 
            //WinAPI function suffers from the same limitation, but be sure that the structure
            //is accurate.  For example, a collaborator (Sean) mentioned that Jerusalem
            //picks some arbitrary dates every year to have daylight saving time.  If you
            //plan to use this function to service that timezone (as an example), make 
            //sure the DaylightDate and StandardDate values in the TimeZoneInfo 
            //structure are updated correctly.
            if ((Environment.OSVersion.Platform == PlatformID.Win32NT) &&
                (Environment.OSVersion.Version.Major >= 5) &&
                (Environment.OSVersion.Version.Minor >= 1))
            {
                SystemTime utcTime;
                //get a SystemTime representation of the sourceLocalTime DateTime to
                //pass to the WinAPI function
                SystemTime stLocal = SystemTime.FromDateTime(sourceLocalTime);
                TimeZoneLocalTimeToUtcTime(ref source.tziStruct, ref stLocal, out utcTime);
                return utcTime.ToDateTime();
            }
            else
            {
                //first we must determine if the specified local time is during the daylight saving
                //time period.  If it is, then we add the Bias and Daylight bias to the value, otherwise
                //we add the Bias and Standard bias (I believe that StandardBias is always 0)
                if (source.IsDaylightSavingTime(sourceLocalTime))
                {
                    return sourceLocalTime.AddMinutes(source.tziStruct.Bias + source.tziStruct.DaylightBias);
                }
                else
                {
                    return sourceLocalTime.AddMinutes(source.tziStruct.Bias + source.tziStruct.StandardBias);
                }
            }
        }
        /// <summary>
        /// Calculates the date that a time change is going to occur given the year and
        /// the SystemTime structure that represents either the StandardDate or DaylightDate
        /// values in the TimeZoneInfo structure
        /// </summary>
        /// <param name="year">The year to calculate the change for</param>
        /// <param name="changeTime">The SystemTime structure that contains information
        /// for calculating the date a time change is to occur.</param>
        /// <returns>A DateTime object the represents when a time change will occur</returns>
        /// <remarks>Returns DateTime.MinValue when no time change is to occur</remarks>
        private static DateTime getChangeDate(int year, SystemTime changeTime)
        {
            //if there is no change month specified, then there is no change to calculate
            //so we will return the minimun DateTime
            if (changeTime.Month == 0)
                return DateTime.MinValue;

            DateTime changeDate;
            //if the the day value is anything less than 5, then we are going to calculate
            //from the start of the month, otherwise we are going to calculate from the beginning
            //of the next month
            if (changeTime.Day < 5)
            {
                //create a date that is the first date of the month when time change occurs
                changeDate = new DateTime(year, changeTime.Month, 1, changeTime.Hour, 0, 0);
                //if the day of week of the current changeDate is less than the DayOfWeek of
                //the changeTime, then we can just subtract the two values.
                int diff = 0;
                if ((short)changeDate.DayOfWeek <= changeTime.DayOfWeek)
                {
                    diff = changeTime.DayOfWeek - (int)changeDate.DayOfWeek;
                }
                else
                {
                    diff = 7 - ((int)changeDate.DayOfWeek - changeTime.DayOfWeek);
                }
                //add the number of days in difference plus 7 * (Day - 1)
                changeDate = changeDate.AddDays(diff + (7 * (changeTime.Day - 1)));
            }
            else
            {
                //create a date that is the first date of the month when the time change occurs
                changeDate = new DateTime(year, changeTime.Month + 1, 1, changeTime.Hour, 0, 0);
                //if the day of week of the current changeDate is less than the DayOfWeek of
                //the changeTime, then we can just subtract the two values.
                if ((short)changeDate.DayOfWeek > changeTime.DayOfWeek)
                {
                    //subtract whatever the last DayOfWeek value is from the current DayOfWeek value
                    changeDate = changeDate.AddDays(-((int)changeDate.DayOfWeek -
                        changeTime.DayOfWeek));
                }
                else
                {
                    //get the difference in days for the DayOfWeek values and then subtract
                    //that difference from 7 to get the number of days we have to adjust
                    changeDate = changeDate.AddDays(-(7 - (changeTime.DayOfWeek -
                        (int)changeDate.DayOfWeek)));
                }
            }
            return changeDate;
        }

        #endregion

        #region instance methods

        /// <summary>
        /// Converts the local time of the current time-zone to the local time of the 
        /// destination time-zone.
        /// </summary>
        /// <param name="destination">The destination time-zone</param>
        /// <param name="sourceLocalTime">The local time in the current time zone that is 
        /// to be converted to the local time in the destination time-zone</param>
        /// <returns>The local time in the destination time-zone.</returns>
        public DateTime Convert(TimeZoneInfo destination, DateTime localTime)
        {
            return TimeZoneInfo.Convert(this, destination, localTime);
        }
        /// <summary>
        /// Converts a local time of the current time-zone to a Utc time
        /// </summary>
        /// <param name="sourceLocalTime">The local time in the current time-zone</param>
        /// <returns>The Utc time that is equivalent to the local time in the current time-zone.</returns>
        public DateTime ToUniversalTime(DateTime sourceLocalTime)
        {
            //call the static ToUniversalTime method passing the current TimeZoneInfo class
            //as the source time zone
            return TimeZoneInfo.ToUniversalTime(this, sourceLocalTime);
        }
        /// <summary>
        /// Converts the UtcTime to the local time of the current time-zone.
        /// </summary>
        /// <param name="utcTime">Utc time that is to be converted to the local time of
        /// the current time-zone.</param>
        /// <returns>DateTime that represents the local time in the current time-zone</returns>
        public DateTime ToLocalTime(DateTime utcTime)
        {
            //call the static ToLocalTime method passing the current TimeZoneInfo class
            //as the destination time zone
            return TimeZoneInfo.ToLocalTime(this, utcTime);
        }
        /// <summary>
        /// Wether daylight saving time is observed in the time-zone
        /// </summary>
        public bool ObservesDaylightTime
        {
            get
            {
                //if the month value of the TimeZoneInfo structure is 0, then it doesn't
                //observer daylight savings time
                return this.tziStruct.DaylightDate.Month != 0;
            }
        }
        /// <summary>
        /// The name of the current TimeZoneInfo structure (Daylight or Standard) based on the date/time.
        /// </summary>
        /// <param name="time">The time to evaluate determine the correct time zone name from</param>
        /// <returns>Either the standard or daylight name of the time zone</returns>
        public string GetTimeZoneName(DateTime time)
        {
            //if the provided time is during the daylight savings time, then return
            //the DaylightName, otherwise return the StandardName
            if (this.IsDaylightSavingTime(time))
            {
                return this.tziStruct.DaylightName;
            }
            else
            {
                return this.tziStruct.StandardName;
            }
        }
        /// <summary>
        /// Returns a value indicating whether the specified date and time is within a 
        /// daylight saving time period.
        /// </summary>
        /// <param name="time">DateTime to evaluate</param>
        /// <returns>True if the time value occurs during the daylight saving time
        /// period for the given year, otherwise false.</returns>
        /// <remarks>The summary description is lifted right from the MSDN docs for the
        /// same method on the TimeZone class.</remarks>
        public bool IsDaylightSavingTime(DateTime time)
        {
            DaylightTime daylightTime = this.GetDaylightChanges(time.Year);
            if (daylightTime == null)
            {
                //if there is on daylight saving time, return false
                return false;
            }
            else
            {
                //forward the call to the overloaded methed with the daylightTime
                //class we constructed to perform the lifting for us
                return TimeZoneInfo.IsDaylightSavingTime(time, daylightTime);
            }
        }
        /// <summary>
        /// Returns a value indicating whether the specified date and time is within a 
        /// daylight saving time period.
        /// </summary>
        /// <param name="time">DateTime to evaluate</param>
        /// <param name="daylightTime">The DaylightTime object that represents a daylight time
        /// period.</param>
        /// <returns>True if the time value occurs during the daylight saving time
        /// period for the given year, otherwise false.</returns>
        /// <remarks>The summary description is lifted right from the MSDN docs for the
        /// same method on the TimeZone class.</remarks>
        public static bool IsDaylightSavingTime(DateTime time, DaylightTime daylightTime)
        {
            //if a null DaylightTime object was passed in, then we need to throw an
            //exception
            if (daylightTime == null)
                throw new ArgumentNullException("daylightTime cannot be null");
            //determine if the date passed in is between the start and end date
            //I'm pretty sure they are just doing >= and <= comparisons, but I'll 
            //let the framework class do the work
            return (System.TimeZone.IsDaylightSavingTime(time, daylightTime));
        }
        /// <summary>
        /// The date of the standard time change
        /// </summary>
        /// <param name="year">The year to calculate the standard change for</param>
        /// <returns>DateTime that represents when the standard time change occurs</returns>
        /// <remarks>Returns DateTime.MinValue if there is no time change</remarks>
        public DateTime GetStandardDateTime(int year)
        {
            //call the getChangeDate function to calculate the date of change
            return TimeZoneInfo.getChangeDate(year, this.tziStruct.StandardDate);
        }
        /// <summary>
        /// The date of the daylight time change.
        /// </summary>
        /// <param name="year">The year to calculate the daylight change for</param>
        /// <returns>DateTime that represents when the daylight time change occurs</returns>
        /// <remarks>Returns DateTime.MinValue if there is no time change</remarks>
        public DateTime GetDaylightDateTime(int year)
        {
            //call the getChangeDate function to calculate the date of change
            return TimeZoneInfo.getChangeDate(year, this.tziStruct.DaylightDate);
        }
        /// <summary>
        /// The daylight time changes for the current time-zone
        /// </summary>
        /// <param name="year">Year to retrieve the daylight changes for</param>
        /// <returns>A DaylightTime object that represents the daylight time for a given year</returns>
        /// <remarks>Returns null if there is no time change</remarks>
        public DaylightTime GetDaylightChanges(int year)
        {
            //if the current timezone information doesn't have adjustments
            //for daylight time, then return null
            if (!this.ObservesDaylightTime)
                return null;

            //construct a DateTime object for the DaylightDate for the current timezone
            return new DaylightTime(this.GetDaylightDateTime(year), this.GetStandardDateTime(year),
                TimeSpan.FromMinutes(-this.tziStruct.DaylightBias));
        }

        #endregion
    }
}
