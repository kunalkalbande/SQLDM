using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.Common.Recommendations
{
    [Serializable]
    public class MultiStatementRecommendation : Recommendation
    {
        private List<OffendingSql> statements;


        public MultiStatementRecommendation(RecommendationType type) : base(type)
        {
            statements = new List<OffendingSql>();
        }

        public void AddOffendingSql(string script, SelectionRectangle stmtBounds, SelectionRectangle focusBounds)
        {
            OffendingSql problem = new OffendingSql();
            problem.Script = script;
            problem.StatementSelection = stmtBounds;
            if (focusBounds != null)
                problem.FocusSelections.Add(focusBounds);
            statements.Add(problem);
        }

        public List<OffendingSql> Statements
        {
            get { return statements; }
        }
    }
}
