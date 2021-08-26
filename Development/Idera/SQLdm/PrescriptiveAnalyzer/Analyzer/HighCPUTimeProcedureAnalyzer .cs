using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    class HighCPUTimeProcedureAnalyzer: AbstractAnalyzer
    {
        private const Int32 id = 25;
        private static Logger _logX = Logger.GetLogger("HighCPUTimeProcedureAnalyzer");
        protected override Logger GetLogger() { return (_logX); }
        
        public HighCPUTimeProcedureAnalyzer()
        {
            _id = id;
        }
        public override string GetDescription() { return ("HighCPUTimeProcedure analysis"); }

        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            if (sm == null) return;
            if (sm.HighCPUTimeProcedureMetrics == null || sm.HighCPUTimeProcedureMetrics.HighCPUTimeProceduresForDBs == null) return;
            foreach (HighCPUTimeProceduresForDB metrics in sm.HighCPUTimeProcedureMetrics.HighCPUTimeProceduresForDBs)
            {
                foreach (HighCPUTimeProcedures snap in metrics.HighCPUTimeProceduresList)
                {
                    AddRecommendation(new FrequentlyExecutedProcedureWithHighCPUTimeRecommendation(snap.Database, snap.ProcedureName));
                }
            }
        }
    }
}
