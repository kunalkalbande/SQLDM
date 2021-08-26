using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class BlockingProcessMetrics : BaseMetrics
    {
        public int BlockingProessCount { get; set; }
        public IEnumerable<BlockingProcess> BlockingProcesses { get; set; }
    }
}
