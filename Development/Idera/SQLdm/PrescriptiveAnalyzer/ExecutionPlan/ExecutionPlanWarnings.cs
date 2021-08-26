using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan
{
    class StatementWarning
    {
        private readonly BaseStmtInfoType _bsit;
        private readonly RelOpType _rot;
        private readonly WarningsType _wt;
        private StatementWarning() { }
        public StatementWarning(BaseStmtInfoType bsit, RelOpType rot, WarningsType wt) { _bsit = bsit; _rot = rot;  _wt = wt; }
        public WarningsType Warning { get { return (_wt); } }
        public BaseStmtInfoType Statement { get { return (_bsit); } }
        public RelOpType RelOp { get { return (_rot); } }
    }

    class ExecutionPlanWarnings : TraverseObjectProps
    {
        private List<StatementWarning> _warnings = new List<StatementWarning>();
        public ExecutionPlanWarnings(SqlSystemObjectManager ssom) : base(ssom) { }
        public IEnumerable<StatementWarning> GetWarnings(ShowPlanXML plan, UInt32 dbid)
        {
            _warnings.Clear();
            Traverse(plan, dbid);
            return (_warnings);
        }
        protected override bool Process(object o)
        {
            if (null == o) return (false);
            if (o is WarningsType) Process((WarningsType)o);
            return (true);
        }
        private void Process(WarningsType wt)
        {
            if (null == wt) return;
            if (wt.NoJoinPredicate) { AddWarning(wt); return; }
            if (null == wt.ColumnsWithNoStatistics) return;
            if (wt.ColumnsWithNoStatistics.Length > 0) { AddWarning(wt); return; }
        }

        private void AddWarning(WarningsType wt)
        {
            _warnings.Add(new StatementWarning(GetParent<BaseStmtInfoType>(), GetParent<RelOpType>(), wt));
        }
    }
}
