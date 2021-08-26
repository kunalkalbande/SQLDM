using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class DeadlockMetrics : BaseMetrics
    {

        internal string TraceFile { get; set; }

        internal int DeadlockCount { get; set; }
        internal IEnumerable<Deadlock> Deadlocks { get; set; } 
    }
}
