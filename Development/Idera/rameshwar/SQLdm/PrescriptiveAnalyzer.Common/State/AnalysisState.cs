using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.State
{
    [Serializable]
    public enum AnalysisStateType
    {
        [Description("Unknown")] 
        Unknown = 0,
        [Description("Performance counter collection")]
        PerformanceCounterCollection,
        [Description("Performance counter analysis")]
        PerformanceCounterAnalysis,
        [Description("Database object analysis")]
        DatabaseObjectAnalysis,
        [Description("Profiler trace collection")]
        ProfilerTraceCollection,
        [Description("Execution plan collection")]
        ExecutionPlanCollection,
        [Description("Execution plan analysis")]
        ExecutionPlanAnalysis,
        [Description("Query syntax analysis")]
        QuerySyntaxAnalysis,
        [Description("Execution stats analysis")]
        ExecutionStatsAnalysis,
        [Description("Missing index analysis")]
        MissingIndexAnalysis,
        [Description("DMV index analysis")]
        DMVMissingIndexAnalysis,
        [Description("Building recommendation list")]
        BuildRecommendations,
    }
    public class AnalysisState
    {
        private object _lock = new object();
        private Dictionary<AnalysisStateType, AnalysisStateInfo> _map = new Dictionary<AnalysisStateType, AnalysisStateInfo>();

        public AnalysisState() { }

        public void Update(AnalysisStateType ast, string status, int current, int max)
        {
            lock (_lock)
            {
                AnalysisStateInfo asi = null;
                if (!_map.TryGetValue(ast, out asi)) _map.Add(ast, asi = new AnalysisStateInfo());
                asi.Update(ast, status, current, max);
            }
        }

        public void Update(AnalysisStateType ast, int current, int max)
        {
            lock (_lock)
            {
                AnalysisStateInfo asi = null;
                if (!_map.TryGetValue(ast, out asi)) _map.Add(ast, asi = new AnalysisStateInfo());
                asi.Update(ast, current, max);
            }
        }

        public void Update(AnalysisStateType ast, string status)
        {
            lock (_lock)
            {
                AnalysisStateInfo asi = null;
                if (!_map.TryGetValue(ast, out asi)) _map.Add(ast, asi = new AnalysisStateInfo());
                asi.Update(ast, status);
            }
        }

        public void Cancel(AnalysisStateType ast, string status)
        {
            lock (_lock)
            {
                AnalysisStateInfo asi = null;
                if (!_map.TryGetValue(ast, out asi)) _map.Add(ast, asi = new AnalysisStateInfo());
                asi.Cancel(ast, status);
            }
        }

        public AnalysisStateInfoCollection GetInfo()
        {
            lock (_lock)
            {
                AnalysisStateInfoCollection asic = new AnalysisStateInfoCollection();
                foreach (AnalysisStateInfo asi in _map.Values) asic.Add(asi.Clone());
                return (asic);
            }
        }
    }
}
