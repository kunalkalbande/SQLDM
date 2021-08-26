using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.SQL
{
    public class LongRunningJob
    {
        internal string JobName { get; private set; }
        internal TimeSpan LastRunDuration { get; private set; }
        internal TimeSpan MaxRunDuration { get; private set; }
        internal TimeSpan AvgRunDuration { get; private set; }

        internal double LastRunDurationMinutes
        {
            get { return Math.Round(LastRunDuration.TotalMinutes, 1); }
        }
        internal double AvgRunDurationMinutes
        {
            get { return Math.Round(AvgRunDuration.TotalMinutes, 1); }
        }

        internal LongRunningJob(DataRow row)
        {
            JobName = DataHelper.ToString(row, "job_name");

            LastRunDuration = ToTimeSpan(DataHelper.ToInt32(row, "last_run_duration"));
            MaxRunDuration  = ToTimeSpan(DataHelper.ToInt32(row, "max_run_duration"));
            AvgRunDuration  = ToTimeSpan(DataHelper.ToInt32(row, "avg_run_duration"));
        }

        static TimeSpan ToTimeSpan(int jdt)
        {
            int hours = jdt / 10000;
            int minutes = (jdt / 100) % 100;
            int seconds = jdt % 100;
            return TimeSpan.FromSeconds(hours * 3600 + minutes * 60 + seconds);
        }
    }
}
