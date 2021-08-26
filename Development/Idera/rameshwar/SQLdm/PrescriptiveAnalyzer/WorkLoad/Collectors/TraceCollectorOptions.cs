using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Cache;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.State;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Collectors
{
    public class TraceCollectorOptions : BaseOptions, ICloneable
    {
        public string Name;
        public UInt64 MinQueryTime; // microseconds 1 millionth of a second
        public UInt64 MaxQueryTime; // microseconds 1 millionth of a second
        public int SampleDurationSeconds;
        public int TotalDurationMinutes;
        public bool ContinuousCollection;
        public bool CollectWorstPerformingTSQL;

        public StringCache TSQLCache = null;
        public PlanCache PlanCache = null;

        private TraceCollectorOptions() :base(null){ }
        internal TraceCollectorOptions(AnalysisState state, StringCache tSQLCache, PlanCache planCache, int id, SqlConnectionInfo info, bool continuousCollection, bool collectWorstPerformingTSQL, UInt64 minQueryTime, UInt64 maxQueryTime, int sampleDurationSeconds, int totalDurationMinutes)
            : base(state)
        {
            TSQLCache = tSQLCache;
            PlanCache = planCache;
            ConnectionInfo = info;
            MinQueryTime = minQueryTime;
            MaxQueryTime = maxQueryTime;
            SampleDurationSeconds = sampleDurationSeconds;
            TotalDurationMinutes = totalDurationMinutes;
            ContinuousCollection = continuousCollection;
            CollectWorstPerformingTSQL = collectWorstPerformingTSQL;
            string machineNameSafe = Environment.MachineName;
            Array.ForEach<char>(Path.GetInvalidFileNameChars(), delegate(char c) { machineNameSafe = machineNameSafe.Replace(c.ToString(), "_"); });
            Name = "SQLdmdocTrace_" + machineNameSafe + "_" + id.ToString();
        }

        public object Clone()
        {
            TraceCollectorOptions o = this.MemberwiseClone() as TraceCollectorOptions;
            if (null != o) o.ConnectionInfo = this.ConnectionInfo.Clone();
            o.TSQLCache = this.TSQLCache;
            return (o);
        }

    }
}
