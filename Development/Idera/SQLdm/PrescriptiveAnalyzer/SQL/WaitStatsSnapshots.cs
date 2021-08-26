using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.SQL
{
    internal class WaitStatsSnapshots : List<WaitStats>
    {
        public WaitStats GetDifference()
        {
            if (Count <= 0) return (null);
            return (this.Last() - this.First());
        }
    }
}
