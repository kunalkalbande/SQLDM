using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using BBS.TracerX;

namespace Idera.SQLdm.DesktopClient.Helpers {
    /// <summary>
    /// This class contains a start and end date in UTC and the UTC offset
    /// relative to the local time zone for that date range.
    /// It also has methods for constructing a list of DateRangeOffsets given a start and
    /// end date in the local time zone that might include one or more Daylight Savings Time 
    /// changes (thus having different UTC offsets).
    /// </summary>
    public class DateRangeOffset
    {
        private static readonly Logger Log = Logger.GetLogger("DateRangeOffset");
        public DateTime UtcStart; // UTC
        public DateTime UtcEnd;   // UTC
        public double UtcOffsetMinutes;  // Offset to add to times in this range to convert to client's local time.

        public DateRangeOffset() { }

        /// <summary>
        /// Given a start and end date in the local time zone, add one or more DateRangeOffsets to the list.
        /// More than one will be added if the range includes a Daylight Savings Time change
        /// since each will need to have a different UtcOffsetMinutes.
        /// </summary>
        public static void AddDateRange(List<DateRangeOffset> list, DateTime localStart, DateTime localEnd) {
            using (Log.VerboseCall()) {
                Debug.Assert(localStart <= localEnd);

                while (localStart <= localEnd) {
                    DateTime nextChange = NextDSTChange(localStart);
                    Log.Verbose("end   = ", localEnd);
                    DateRangeOffset dro = new DateRangeOffset();

                    dro.UtcStart = localStart.ToUniversalTime();
                    dro.UtcOffsetMinutes = TimeZone.CurrentTimeZone.GetUtcOffset(localStart).TotalMinutes;

                    // nextChange == DateTime.MinValue if there is no DST in this time zone.

                    if (nextChange != DateTime.MinValue && localEnd > nextChange) {
                        dro.UtcEnd = nextChange.ToUniversalTime();
                        list.Add(dro);
                        Log.Verbose("Added ", dro.UtcStart, " - ", dro.UtcEnd, " with UtcOffsetMinutes = ", dro.UtcOffsetMinutes);
                        Debug.Assert(dro.UtcOffsetMinutes != TimeZone.CurrentTimeZone.GetUtcOffset(nextChange).TotalMinutes);
                        localStart = nextChange.AddTicks(1);
                    } else {
                        dro.UtcEnd = localEnd.ToUniversalTime();
                        list.Add(dro);
                        Log.Verbose("Added ", dro.UtcStart, " - ", dro.UtcEnd, " with UtcOffsetMinutes = ", dro.UtcOffsetMinutes);
                        break;
                    }
                }
            }
        }

        // Returns true if the list contains a contiguous date range (each
        // UtcStart.Date equals the previous UtcEnd.Date)
        public static bool IsContiguous(List<DateRangeOffset> list) {
            if (list.Count <= 1) return true;
            else {
                DateTime curDate = list[0].UtcStart.Date;
                foreach (DateRangeOffset dro in list) {
                    if (dro.UtcStart.Date == curDate) {
                        curDate = dro.UtcEnd.Date;
                    } else {
                        return false;
                    }
                }

                return true;
            }
        }

        // Return the first DST change after the specified start date.
        // Input and output are in the local time zone.
        private static DateTime NextDSTChange(DateTime localStart) {
            TimeZone tz = TimeZone.CurrentTimeZone;
            System.Globalization.DaylightTime daylightTime = tz.GetDaylightChanges(localStart.Year);
            DateTime nextChange;

            if (daylightTime.Start == DateTime.MinValue) {
                // No DST in this time zone, or the checkbox that says "Automatically adjust clock
                // for daylight saving changes" is unchecked in the system "Date and Time Properties" dialog.
                return DateTime.MinValue;
            }

            // The UTC offset of daylightTime.Start is the same as
            // the UTC offset for start.  I found it necessary to add the Delta to
            // get a different offset.  I also tried adding 1 second, but that did not work.
            // Note that the period between daylightTime.Start and daylightTime.Start + daylightTime.Delta
            // does not officially exist since all clocks should immediately skip that period.

            if (localStart < daylightTime.Start) 
                nextChange = daylightTime.Start + daylightTime.Delta;
            else if (localStart < daylightTime.End) 
                nextChange = daylightTime.End;
            else {
                // Have to look at the next year.
                daylightTime = tz.GetDaylightChanges(localStart.Year + 1);
                nextChange = daylightTime.Start + daylightTime.Delta;
            }

            Log.Verbose("NextDSTChange(", localStart, ") returning ", nextChange);
            return nextChange;
        }
    }
}
