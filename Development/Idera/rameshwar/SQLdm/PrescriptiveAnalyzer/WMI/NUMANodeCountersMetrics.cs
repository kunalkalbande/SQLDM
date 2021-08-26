using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class NUMANodeCountersMetrics
    {
        public IEnumerable<NUMANodeCounters> NumaNodeCountersList { get; set; }
    }
}
