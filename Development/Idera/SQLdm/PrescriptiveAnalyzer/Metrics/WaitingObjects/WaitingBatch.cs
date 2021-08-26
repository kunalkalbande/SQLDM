using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics.WaitingObjects
{
    internal class WaitingBatch
    {
        public string Batch { get; set; }
        public int Count { get; set; }
        public string Program { get; set; }
    }
}
