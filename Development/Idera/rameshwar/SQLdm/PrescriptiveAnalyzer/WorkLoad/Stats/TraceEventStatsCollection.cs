using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Stats
{
    public class TraceEventStatsCollection : List<TraceEventStats>
    {
        public int TraceDurationMinutes { get; set; }
        public UInt64 ActivityDuration { get; set; }
        public UInt64 ActivityReads { get; set; }
        public UInt64 ActivityWrites { get; set; }
        public UInt64 ActivityCPU { get; set; }
        public SqlSystemObjectManager SSOM { get; private set; }

        private Dictionary<string, List<TraceEventStats>> _mapTablesToEvents = new Dictionary<string, List<TraceEventStats>>();
        private ExecutionPlanTables _executionPlanTables = null;
        public TraceEventStatsCollection(SqlSystemObjectManager ssom)
        {
            SSOM = ssom;
            _executionPlanTables = new ExecutionPlanTables(ssom);
        }
        public IEnumerable<TraceEventStats> GetAllEventStatsForTable(string table, bool excludeWorstTSQL)
        {
            List<TraceEventStats> result = null;
            _mapTablesToEvents.TryGetValue(table, out result);
            if (excludeWorstTSQL && (null != result)) result.RemoveAll(delegate(TraceEventStats s) { if (null == s) return (true); return (s.IsWorstTSQL()); });
            return (result);
        }
        public new void Add(TraceEventStats item)
        {
            List<TraceEventStats> stats;
            foreach (string table in _executionPlanTables.GetTables(item.Plan, item.High.DBID))
            {
                if (!_mapTablesToEvents.TryGetValue(table, out stats))
                {
                    stats = new List<TraceEventStats>();
                    _mapTablesToEvents.Add(table, stats);
                }
                stats.Add(item);
            }
            base.Add(item);
        }
        public new void AddRange(IEnumerable<TraceEventStats> collection)
        {

            foreach (TraceEventStats item in collection) Add(item);
        }

        public void SortByCost()
        {
            this.Sort(new TraceEventStatsCostComparer());
        }
    }
}
