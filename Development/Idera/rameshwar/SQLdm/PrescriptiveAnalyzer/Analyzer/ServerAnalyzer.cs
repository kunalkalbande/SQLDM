using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using System.Data.SqlClient;
//using Idera.SQLdoctor.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;


namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    public class ServerAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 14;
        private static Logger _logX = Logger.GetLogger("ServerAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public ServerAnalyzer()
        {
            _id = id;
        }

        public override string GetDescription() { return ("Server resource analysis"); }

        public override void Analyze(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("Analyze"))
            {
                base.Analyze(sm, conn);
                AnalyzeCompilations(sm);
            }
        }

        /// <summary>
        /// make sure that the compilations per second are not over 10% of the batches per second.
        /// </summary>
        /// <param name="sc"></param>
        private void AnalyzeCompilations(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzeCompilations"))
            {
                if (null == sm) { _logX.Debug("null SnapshotMetrics"); return; }
                if (null == sm.SampledServerResourcesMetrics) { _logX.Debug("null Sampled Server Resources Collector"); return; }
                _logX.DebugFormat("BatchReqSec={0}  SqlCompilationsSec={1}", sm.SampledServerResourcesMetrics.BatchReqSec, sm.SampledServerResourcesMetrics.SqlCompilationsSec);
                if (sm.SampledServerResourcesMetrics.BatchReqSec > 10)
                {
                    double r = (double)sm.SampledServerResourcesMetrics.SqlCompilationsSec / (double)sm.SampledServerResourcesMetrics.BatchReqSec;
                    if (r > 0.1)
                    {
                        _logX.DebugFormat("High compilations recommendation added {0}", r);
                        AddRecommendation(new HighCompilationsRecommendation(sm.SampledServerResourcesMetrics.BatchReqSec, sm.SampledServerResourcesMetrics.SqlCompilationsSec));
                    }
                }
            }
        }
    }
}
