using System;
using System.Collections.Generic;
using System.Text;
using TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    internal class BlockingAnalyzer : AbstractAnalyzer
    {
        private static Logger _logX = Logger.GetLogger("BlockingAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public override string GetDescription() { return ("Blocking process analysis"); }
        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            using (_logX.DebugCall("BlockingAnalyzer.Analyze"))
            {
                base.Analyze(sm, conn);

                if (sm.BlockingProcessMetrics.BlockingProessCount > 0)
                {
                    foreach (BlockingProcess bp in sm.BlockingProcessMetrics.BlockingProcesses)
                    {
                        BlockingProcessRecommendation recommendation = new BlockingProcessRecommendation(bp.Database, bp.ApplicationName, bp.UserName, bp.HostName);
                        recommendation.SPID = bp.SPID;
                        recommendation.BlockedWait = bp.BlockedWait;
                        recommendation.BlockedNumberOfProcesses = bp.BlockedNumberOfProcesses;
                        recommendation.BlockedObject = bp.BlockedResource;

                        if (!String.IsNullOrEmpty(bp.BlockedTSQL))
                        {
                            OffendingSql sql = new OffendingSql();
                            sql.Script = bp.BlockedTSQL;
                            recommendation.Sql = sql;
                        }

                        AddRecommendation(recommendation);
                    }
                }
            }
        }
    }
}
