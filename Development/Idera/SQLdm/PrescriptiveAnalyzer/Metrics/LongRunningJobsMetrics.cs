using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class LongRunningJobsMetrics : BaseMetrics
    {
        public int LongRunningJobCount { get; set; }
        public IEnumerable<LongRunningJob> LongRunningJobs { get; set; }
    }
}
