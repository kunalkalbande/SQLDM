using System.Collections.Generic;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Microsoft.Data.Schema.ScriptDom.Sql;
using System;

namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    internal class FunctionInSearchAnalyzer : AbstractQueryAnalyzer
    {
        private const Int32 id = 104;

        internal FunctionInSearchAnalyzer(string database) : base(database)
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

            Stack<TSqlFragment> peStack = new Stack<TSqlFragment>();
            TSqlFragment peState = fragment;
            TSqlFragment lastPeError = null;

            TSqlFragmentWalker walker = new TSqlFragmentWalker();
            walker.PushEndFilter = PushEndFilter;
            IEnumerator<TSqlFragment> e = walker.GetEnumerator(fragment, true);
            while (e.MoveNext())
            {
                TSqlFragment frag = e.Current;
                if (frag is TSqlEndFragment)
                {
                    frag = ((TSqlEndFragment)frag).StartFragment;
                    if (frag == state)
                        state = stateStack.Pop();
                    
                    if (frag is PrimaryExpression)
                        peState = peStack.Pop();
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

                    peStack.Push(peState);
                    peState = frag;
                }
                else
                if (frag is Column)
                {
                    if (state is WhereClause && peState is PrimaryExpression && !(peState is Subquery))
                    {
                        FunctionCall fc = peState as FunctionCall;
                        if (fc != null && fc.Parameters != null && fc.Parameters.Count > 1)
                        {
                            switch (fc.FunctionName.Value.ToLower())
                            {
                                case "dateadd":
                                case "datediff":
                                case "datepart":
                                    if (fc.Parameters[0] == frag)
                                        continue;
                                    break;
                            }
                        }

                        if (recommendation == null)
                        {
                            OffendingSql sql = new OffendingSql();
                            sql.Script = script;
                            sql.StatementSelection = fragment.GetSelectionBounds();
                            recommendation = new TSqlRecommendation(RecommendationType.FunctionInWhereClause, Database, Application, User, Host);
                            recommendation.Sql = sql;
                            AddRecommendation(recommendation);
                            lastPeError = null;
                        }
                        if (lastPeError != peState)
                        {
                            recommendation.Sql.FocusSelections.Add(peState.GetSelectionBounds());
                            lastPeError = peState;
                        }
                    }
                }
                else
                if (frag is Literal || frag is ParenthesisExpression)
                {   // want to ignore these specialized PrimaryExpressions
                }
                else
                if (frag is PrimaryExpression)
                {
                    peStack.Push(peState);
                    peState = frag;
                }
            }
        }

        private bool PushEndFilter(TSqlFragment fragment)
        {
            if (fragment is WhereClause)
                return true;
            if (fragment is Subquery)
                return true;

            if (fragment is ParenthesisExpression)
                return false;
            if (fragment is Literal)
                return false;
            if (fragment is Column)
                return false;

            if (fragment is PrimaryExpression)
                return true;

            return false;
        }
    }
}
