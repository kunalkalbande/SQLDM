using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Cache;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.State;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Collectors
{
    public class ExtendedEventCollectorOptions : BaseOptions, ICloneable
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

        public QueryMonitorConfiguration queryConfig;
        public ActiveWaitsConfiguration waitConfig;

        private ExtendedEventCollectorOptions() :base(null){ }
        internal ExtendedEventCollectorOptions(AnalysisState state, StringCache tSQLCache, PlanCache planCache, int id, Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration.SqlConnectionInfo info, bool continuousCollection, bool collectWorstPerformingTSQL, UInt64 minQueryTime, UInt64 maxQueryTime, int sampleDurationSeconds, int totalDurationMinutes)
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
            Name = "SQLdmdocExEvent_" + id.ToString(); 
        }

        public object Clone()
        {
            ExtendedEventCollectorOptions o = this.MemberwiseClone() as ExtendedEventCollectorOptions;
            if (null != o) o.ConnectionInfo = this.ConnectionInfo.Clone();
            o.TSQLCache = this.TSQLCache;
            return (o);
        }

    }
}
