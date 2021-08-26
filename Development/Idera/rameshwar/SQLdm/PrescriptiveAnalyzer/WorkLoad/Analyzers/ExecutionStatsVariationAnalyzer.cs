using System.Collections.Generic;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Collectors;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using System;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Analyzers
{
    internal class ExecutionStatsVariationAnalyzer : IAnalyzeDataBucket
    {
        private const Int32 id = 301;
        private static Logger _logX = Logger.GetLogger("ExecutionStatsVariationAnalyzer");
        private List<IRecommendation> _recommendations = new List<IRecommendation>();
        private BaseOptions _baseops;
        private SqlSystemObjectManager _ssom = null;
        private bool _cancelled;

        internal ExecutionStatsVariationAnalyzer(BaseOptions ops, SqlSystemObjectManager ssom)
        {
            _ssom = ssom;
            _baseops = ops;
        }

        public Int32 ID { get { return id; } }

        public void Analyze(DataBucket bucket)
        {
            if (_cancelled)
                throw new OperationCanceledException();
            if (Properties.Settings.Default.Max_RecommendationsPerType > 0)
            {
                if (_recommendations.Count >= Properties.Settings.Default.Max_RecommendationsPerType) return;
            }

            if (bucket.TotalCount < 5)
                return;

            if (bucket.HighDuration.Duration < 1000)
                return;

            TEBase hi = null;
            TEBase lo = null;

            // As per US5247, based on Brent Ozar's recommendation we flag this only if there is a >= 5x difference in reads
            if (bucket.LowReads.Reads > 0 && (bucket.HighReads.Reads / bucket.LowReads.Reads) >= 5)
            {
                lo = bucket.LowReads;
                hi = bucket.HighReads;
            }
            
            if (lo != null && hi != null)
                CreateRecommendation(bucket, lo, hi);
        }

        private void CreateRecommendation(DataBucket bucket, TEBase low, TEBase hi)
        {
            string db = (null != _baseops) ? _baseops.GetDatabaseName(hi.DBID) : string.Empty;
            if (null != _ssom)
            {
                if (_ssom.IsSystemObject(hi.DBID, 0, hi.ObjectName))
                {
                    return;
                }
            }
            CachedPlanMisuseRecommendation cpmr = new CachedPlanMisuseRecommendation(db, hi.ApplicationName, hi.LoginName, hi.HostName);
            cpmr.ExecutionCount = bucket.TotalCount;
            cpmr.MinimumDuration = low.Duration;
            cpmr.MaximumDuration = hi.Duration;
            cpmr.MinimumCPU = low.CPU;
            cpmr.MaximumCPU = hi.CPU;
            cpmr.MinimumReads = low.Reads;
            cpmr.MaximumReads = hi.Reads;
            cpmr.ObjectName = hi.ObjectName;

            OffendingSql sql = new OffendingSql();
            sql.Script = hi.TextData;
            cpmr.Sql = sql;

            _logX.DebugFormat("Adding for database '{0}', application '{1}', object '{2}'", cpmr.Database, cpmr.ApplicationName, cpmr.ObjectName);    
            _recommendations.Add(cpmr);
        }

        public IRecommendation[] GetRecommendations()
        {
            return _recommendations.ToArray();
        }
    }
}
