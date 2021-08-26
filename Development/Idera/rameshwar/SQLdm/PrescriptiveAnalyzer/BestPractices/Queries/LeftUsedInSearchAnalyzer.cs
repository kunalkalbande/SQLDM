using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    internal class LeftUsedInSearchAnalyzer : AbstractQueryAnalyzer
    {
        private const Int32 id = 107;
        internal LeftUsedInSearchAnalyzer(string database) : base(database)
        {
            _id = id;
        }

        public override void Analyze(string script, TSqlStatement fragment)
        {
            // allow the base class a chance to handle this statement
            base.Analyze(script, fragment);

            TSqlRecommendation recommendation = null;

            Stack<TSqlFragment> stateStack = new Stack<TSqlFragment>();
            TSqlFragment state = fragment;

            TSqlFragmentWalker walker = new TSqlFragmentWalker();
            walker.PushEndFilter = PushEndFilter;
            IEnumerator<TSqlFragment> e = walker.GetEnumerator(fragment, true);
            while (e.MoveNext())
            {
                TSqlFragment frag = e.Current;
                if (frag is TSqlEndFragment)
                {
                    state = stateStack.Pop();
                }
                else                
                if (frag is WhereClause)
                {
                    stateStack.Push(state);
                    state = frag;
                }
                else
                if (frag is Subquery)
                {
                    stateStack.Push(state);
                    state = frag;
                }
                else
                if (frag is LeftFunctionCall)
                {
                    if (state is WhereClause)
                    {
                        if (recommendation == null)
                        {                            
                            OffendingSql sql = new OffendingSql();
                            sql.Script = script;
                            sql.StatementSelection = fragment.GetSelectionBounds();
                            recommendation = new TSqlRecommendation(RecommendationType.LeftExpressionInWhereClause, Database, Application, User, Host);
                            recommendation.Sql = sql;
                            AddRecommendation(recommendation);
                        }
                        recommendation.Sql.FocusSelections.Add(frag.GetSelectionBounds());
                    }
                }
            }
        }

        private bool PushEndFilter(TSqlFragment fragment)
        {
            if (fragment is WhereClause)
                return true;
            if (fragment is Subquery)
                return true;

            return false;
        }
    }
}
