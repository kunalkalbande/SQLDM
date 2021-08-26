using System;
using System.Collections.Generic;
using System.Text;
using TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    internal class JobAnalyzer : AbstractAnalyzer
    {
        private static Logger _logX = Logger.GetLogger("JobAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public override string GetDescription() { return ("SQL Agent job analysis"); }
        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            using (_logX.DebugCall("JobAnalyzer.Analyze"))
            {
                base.Analyze(sm, conn);

                if (sm.LongRunningJobsMetrics != null)
                {
                    if (sm.LongRunningJobsMetrics.LongRunningJobCount > 0)
                    {
                        foreach (LongRunningJob job in sm.LongRunningJobsMetrics.LongRunningJobs)
                        {
                            LongRunningJobRecommendation recommendation = new LongRunningJobRecommendation();
                            recommendation.JobName = job.JobName;
                            recommendation.LastRunDuration = job.LastRunDuration;
                            recommendation.MaxRunDuration = job.MaxRunDuration;
                            recommendation.AvgRunDuration = job.AvgRunDuration;
                            AddRecommendation(recommendation);
                        }
                    }
                }
            }
        }
    }
}
