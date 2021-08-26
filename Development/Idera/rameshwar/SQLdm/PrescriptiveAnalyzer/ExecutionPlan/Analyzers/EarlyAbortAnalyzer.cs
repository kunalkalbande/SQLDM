using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Stats;

namespace Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan.Analyzers
{
    internal class EarlyAbortAnalyzer : AbstractPlanAnalyzer
    {
        class StatementsAborted
        {
            public TraceEventStats Stats { get; set; }
            public List<BaseStmtInfoType> Aborted { get; set; }
        }

        private const Int32 id = 201;
        private static TracerX.Logger _logX = TracerX.Logger.GetLogger("EarlyAbortAnalyzer");

        List<StatementsAborted> _statementsAborted = null;
        List<BaseStmtInfoType> _aborted = null;
        public EarlyAbortAnalyzer(BaseOptions ops, SqlSystemObjectManager ssom) : base(ops, ssom) 
        {
            _id = id;
        }

        public override void Analyze(ShowPlanXML plan, UInt32 dbid)
        {
            if (null != _aborted) _aborted.Clear();
            base.Analyze(plan, dbid);
            if (null != _aborted)
            {
                if (_aborted.Count > 0)
                {
                    if (null == _statementsAborted) _statementsAborted = new List<StatementsAborted>();
                    _statementsAborted.Add(new StatementsAborted() { Stats = _stats, Aborted = _aborted });
                }
            }
        }

        public override void Clear()
        {
            _statementsAborted = null;
            _aborted = null;
            base.Clear();
        }

        public override IEnumerable<IRecommendation> GetRecommendations()
        {
            if (null != _statementsAborted)
            {
                List<BatchStatements> memExceeded = new List<BatchStatements>();
                List<string> stmts = new List<string>();
                UInt64 count = 0;
                foreach (StatementsAborted aborted in _statementsAborted)
                {
                    stmts.Clear();
                    foreach (BaseStmtInfoType stmt in aborted.Aborted)
                    {
                        if (BaseStmtInfoTypeStatementOptmEarlyAbortReason.MemoryLimitExceeded == stmt.StatementOptmEarlyAbortReason)
                        {
                            stmts.Add(stmt.StatementText);
                            if (null != aborted.Stats) count += aborted.Stats.TotalCount;
                        }
                    }
                    if (stmts.Count > 0) memExceeded.Add(new BatchStatements(aborted.Stats.High.TextData, stmts));
                }
                if (memExceeded.Count > 0)
                {
                    int traceMinutes = GetTraceDurationMinutes();
                    _logX.InfoFormat("Added MemLimitExceededRecommendation.  Occurred {0} times over {1} minutes.", count, traceMinutes);
                    yield return new MemLimitExceededRecommendation(memExceeded, count, traceMinutes);
                }
            }
        }

        private int GetTraceDurationMinutes()
        {
            if (null == _statsCollection) return (1);
            return (_statsCollection.TraceDurationMinutes);
        }

        public override bool Analyze(BaseStmtInfoType bsif)
        {
            if (!bsif.StatementOptmEarlyAbortReasonSpecified) return (false);
            //----------------------------------------------------------------------------
            // Based on msdn link: http://msdn.microsoft.com/en-us/library/ms189298.aspx
            // 
            //  If the query optimizer prematurely terminates query optimization, the 
            //  StatementOptmEarlyAbortReason attribute is returned for the StmtSimple element 
            //  in XML Showplan output. The possible values that can display for this attribute 
            //  are TimeOut, GoodEnoughPlanFound, and MemoryLimitExceeded. 
            //
            //  If TimeOut or GoodEnoughPlanFound are returned for this attribute, no action is necessary. 
            //  The Showplan returned contains correct results.
            //
            //  If MemoryLimitExceeded is returned for the StatementOptmEarlyAbortReason attribute, 
            //  the XML Showplan produced will still be correct, but it may not be optimal. Try one of 
            //  the following methods to increase available memory: 
            //    1) Reduce the load on the server. 
            //    2) Increase memory available to SQL Server. For more information, see Managing Memory for Large Databases. 
            //    3) Check the max server memory option that is set with sp_configure, 
            //       and increase the value if it is too low. For more information, see Server Memory Options.
            //
            switch (bsif.StatementOptmEarlyAbortReason)
            {
                case BaseStmtInfoTypeStatementOptmEarlyAbortReason.GoodEnoughPlanFound:
                case BaseStmtInfoTypeStatementOptmEarlyAbortReason.TimeOut:
                    {
                        break;
                    }
                case BaseStmtInfoTypeStatementOptmEarlyAbortReason.MemoryLimitExceeded:
                    {
                        if (null == _aborted) _aborted = new List<BaseStmtInfoType>();
                        _aborted.Add(bsif);
                        break;
                    }
            }
            return (false);
        }
    }
}
