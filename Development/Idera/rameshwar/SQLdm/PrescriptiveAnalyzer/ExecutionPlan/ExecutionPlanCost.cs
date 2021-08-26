using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;

namespace Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan
{
    class ExecutionPlanCost : ExecutionPlanProcessor
    {
        private double _totalCost;
        private double _totalRows;

        public double TotalCost { get {return(_totalCost);} }
        public double TotalRows { get { return (_totalRows); } }

        public ExecutionPlanCost(SqlSystemObjectManager ssom) : base(ssom) {  }

        public void CalculateCost(ShowPlanXML plan, UInt32 dbid)
        {
            _totalCost = 0.0;
            _totalRows = 0.0;
            Process(plan, dbid);
        }

        public override void Process(BaseStmtInfoType stmt)
        {
            if (stmt.StatementSubTreeCostSpecified) _totalCost += stmt.StatementSubTreeCost;
            if (stmt.StatementEstRowsSpecified) _totalRows += stmt.StatementEstRows;
        }
    }
}
