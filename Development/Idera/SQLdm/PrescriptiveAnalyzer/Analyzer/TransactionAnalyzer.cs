using System;
using System.Collections.Generic;
using System.Text;
using TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    internal class TransactionAnalyzer : AbstractAnalyzer
    {
        private static Logger _logX = Logger.GetLogger("TransactionAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public override string GetDescription() { return ("Transaction analysis"); }

        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            using (_logX.DebugCall("TransactionAnalyzer.Analyze"))
            {
                base.Analyze(sm, conn);

                if (sm.TransactionMetrics.OpenTransactionCount > 0)
                {
                    foreach (OpenTransaction ot in sm.TransactionMetrics.OpenTransactions)
                    {
                        OpenTransactionRecommendation recommendation = new OpenTransactionRecommendation(ot.Database, ot.Program, ot.Login, ot.Host);
                        recommendation.TID = ot.TID;
                        recommendation.SPID = ot.SPID;
                        recommendation.Start = ot.Start;
                        recommendation.Duration = ot.Duration;

                        if (!String.IsNullOrEmpty(ot.Sql))
                        {
                            OffendingSql sql = new OffendingSql();
                            sql.Script = ot.Sql;
                            sql.StatementSelection = new SelectionRectangle(new BufferLocation(-1, -1, -1), ot.Sql.Length);
                            recommendation.Sql = sql;
                        }
                        AddRecommendation(recommendation);
                    }
                }
            }
        }
    }
}
