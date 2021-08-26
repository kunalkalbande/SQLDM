using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    public static class DateTimeHelper
    {
        public static string GetRealTimeDateQuickHistoricalSnapshots()
        {
            var formatedString = "Real Time: " + DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern);
            return formatedString;
        }

        public static string GetStartEndDateQuickHistoricalSnapshots(DateTime startTime, DateTime endTime)
        {
            var formatedString = startTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern) + " (" + startTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern) + ") to " + endTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern) + " (" + endTime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern) + ")";
            return formatedString;
        }
    }
}
