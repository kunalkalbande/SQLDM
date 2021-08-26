using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;

namespace Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan
{
    internal abstract class ExecutionPlanProcessor
    {
        private UInt32 _dbid;
        private SqlSystemObjectManager _ssom;
        private ExecutionPlanProcessor() { }
        public ExecutionPlanProcessor(SqlSystemObjectManager ssom) : this() { _ssom = ssom; }
        public virtual void Process(ShowPlanXML plan, UInt32 dbid)
        {
            _dbid = dbid;
            if (null == plan) return;
            if (null == plan.BatchSequence) return;
            foreach (StmtBlockType[] blocks in plan.BatchSequence)
            {
                Process(blocks);
            }
        }
        public virtual void Process(StmtBlockType[] blocks)
        {
            if (null == blocks) return;
            foreach (StmtBlockType block in blocks)
            {
                Process(block);
            }
        }
        public virtual void Process(StmtBlockType block)
        {
            if (null == block) return;
            if (null == block.Items) return;
            foreach (BaseStmtInfoType stmt in block.Items)
            {
                Process(stmt);
            }
        }
        public virtual void Process(BaseStmtInfoType stmt)
        {
            if (stmt is StmtSimpleType)
            {
                if (IsSystemStoredProc((StmtSimpleType)stmt)) { return; }
                Process((StmtSimpleType)stmt);
            }
            else if (stmt is StmtCondType) Process((StmtCondType)stmt);
            else if (stmt is StmtCursorType) Process((StmtCursorType)stmt);
            else if (stmt is StmtReceiveType) Process((StmtReceiveType)stmt);
            else if (stmt is StmtUseDbType) Process((StmtUseDbType)stmt);
        }

        private bool IsSystemStoredProc(StmtSimpleType sst)
        {
            if (null == sst) return (false);
            if (null == sst.StoredProc) return (false);
            if (string.IsNullOrEmpty(sst.StoredProc.ProcName)) return (false);
            if (_ssom.IsSystemObject(_dbid, 0, sst.StoredProc.ProcName)) return (true);
            return (false);
        }

        public virtual void Process(StmtCursorType stmt) { if (null != stmt) Process(stmt.CursorPlan); }
        public virtual void Process(CursorPlanType cpt) { if (null != cpt) Process(cpt.Operation); }
        public virtual void Process(CursorPlanTypeOperation[] ops) { if (null != ops) foreach (CursorPlanTypeOperation op in ops) Process(op); }
        public virtual void Process(CursorPlanTypeOperation op) { if (null != op) Process(op.QueryPlan); }

        public virtual void Process(StmtReceiveType stmt) { if (null != stmt) Process(stmt.ReceivePlan); }
        public virtual void Process(ReceivePlanTypeOperation[] ops) { if (null != ops) foreach (ReceivePlanTypeOperation op in ops) Process(op); }
        public virtual void Process(ReceivePlanTypeOperation op) { if (null != op) Process(op.QueryPlan); }

        public virtual void Process(StmtUseDbType stmt) { }

        public virtual void Process(StmtSimpleType stmt) { if (null != stmt) Process(stmt.QueryPlan); }

        public virtual void Process(StmtCondType stmt) { if (null != stmt) Process(stmt.Condition); }
        public virtual void Process(StmtCondTypeCondition cond) { if (null != cond) Process(cond.QueryPlan); }

        public virtual void Process(QueryPlanType plan) { }

    }
}
