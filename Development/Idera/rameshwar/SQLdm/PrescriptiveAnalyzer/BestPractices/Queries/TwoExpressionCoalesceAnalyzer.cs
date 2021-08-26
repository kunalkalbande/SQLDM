
namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    using System.Collections;
    using System.Collections.Generic;
    using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
    using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
    using Microsoft.Data.Schema.ScriptDom.Sql;
    using System;

    internal class TwoExpressionCoalesceAnalyzer : AbstractQueryAnalyzer
    {
        private const Int32 id = 117;
        public TwoExpressionCoalesceAnalyzer(string database) : base(database)
        {
            _id = id;
        }

        public override void Analyze(string script, TSqlStatement fragment)
        {
            // allow the base class a chance to handle this statement
            base.Analyze(script, fragment);

            TSqlFragmentWalker walker = new TSqlFragmentWalker();
            IEnumerator<TSqlFragment> e = walker.GetEnumerator(fragment, true);

            TSqlStatement currentStatement = null;
            List<CoalesceExpression> coalesceExpressions = new List<CoalesceExpression>();

            while (e.MoveNext())
            {
                TSqlFragment frag = e.Current;
                if (frag is TSqlStatement)
                {
                    if (coalesceExpressions.Count > 0)
                        Analyze(script, currentStatement, coalesceExpressions);
                    currentStatement = (TSqlStatement)frag;
                    coalesceExpressions.Clear();
                    continue;
                }

                if (frag is CoalesceExpression)
                {
                    if (((CoalesceExpression)frag).Expressions.Count > 2)
                        continue;

                    coalesceExpressions.Add((CoalesceExpression)frag);
                }
            }
            if (coalesceExpressions.Count > 0)
                Analyze(script, currentStatement, coalesceExpressions);
        }

        private void Analyze(string script, TSqlStatement currentStatement, List<CoalesceExpression> coalesceExpressions)
        {
            OffendingSql sql = new OffendingSql();
            sql.Script = script;
            sql.StatementSelection = TSqlParsingHelpers.GetSelectionRectangle(currentStatement);

            foreach (CoalesceExpression ce in coalesceExpressions)
            {
                sql.FocusSelections.Add(TSqlParsingHelpers.GetSelectionRectangle(ce));
            }

            TSqlRecommendation recommendation = new TSqlRecommendation(RecommendationType.TwoExpressionCoalesce, Database, Application, User, Host);
            recommendation.Sql = sql;
            AddRecommendation(recommendation);
        }
    }
}
