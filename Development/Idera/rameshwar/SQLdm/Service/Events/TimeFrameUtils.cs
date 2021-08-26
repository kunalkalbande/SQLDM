using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Idera.SQLdm.Service.Core.Enums;
namespace Idera.SQLdm.Service.Events
{
    /// <summary>
    /// SQLdm10.2(srishti purohit) - Gives SummaryLevel calculation logic and convert time into seconds for GetQueryWaitStatisticsForInstanceOverview API on instance overview page in web UI
    /// SQLdm-28027 defect fix
    /// </summary>
    public static class TimeFrameUtils
    {
        public const int MAX_BARS_IN_OVERTIME_GRAPH = 52;
        /// <summary>
        /// Interval to be used to add default rows in list to fix SQLdm28027
        /// </summary>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <returns></returns>
        public static SummaryLevel GetSummaryLevel(long fromTime, long toTime)
        {
            long timeDiff = toTime - fromTime;

            //DateTime cal = DateTime.Now;
            //cal.AddDays(-1);

            //SummaryLevel minSummaryLevel = SummaryLevel.S;
            //if (fromTime < cal.Ticks)
            //{
            //    minSummaryLevel = SummaryLevel.H;
            //}

            if (timeDiff <= 0)
            {
                throw new Exception("From time " + fromTime + " must be earlier than To time " + toTime);
            }

            foreach (SummaryLevel sl in Enum.GetValues(typeof(SummaryLevel)))
            {
                if (timeDiff < MAX_BARS_IN_OVERTIME_GRAPH * (int)sl)
                    //&& Array.IndexOf(Enum.GetValues(sl.GetType()), sl) >= Array.IndexOf(Enum.GetValues(minSummaryLevel.GetType()), minSummaryLevel))
                    return sl;
            }

            throw new Exception("Timeframe is too wide. It must be narrower than " + MAX_BARS_IN_OVERTIME_GRAPH
                + " weeks");
        }
        /// <summary>
        /// Converting time in long value
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetTimeSeconds(DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }
    }
}

