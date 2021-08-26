using System.Collections.Generic;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Microsoft.Data.Schema.ScriptDom.Sql;
using System;

namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    internal class LikeAbuseAnalyzer : AbstractQueryAnalyzer
    {
        private const Int32 id = 108;
        internal LikeAbuseAnalyzer(string database) : base(database)
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
                if (frag is LikePredicate)
                {
                    LikePredicate lp = (LikePredicate)frag;

                    Literal format = lp.SecondExpression as Literal;
                    if (format != null && format.LiteralType != LiteralType.Variable)
                    {
                        string value = format.Value;
                        int ix = value.IndexOfAny("%_[".ToCharArray());
                        if (ix == -1)
                        {
                            if (recommendation == null)
                            {
                                OffendingSql sql = new OffendingSql();
                                sql.Script = script;
                                sql.StatementSelection = fragment.GetSelectionBounds();
                                recommendation = new TSqlRecommendation(RecommendationType.LikeUseNotNeeded, Database, Application, User, Host);
                                recommendation.Sql = sql;
                                AddRecommendation(recommendation);
                            }
                            recommendation.Sql.FocusSelections.Add(frag.GetSelectionBounds());
                        }
                    }


                    
                    //if (state is WhereClause)
                    //{
                    //    if (recommendation == null)
                    //    {                            
                    //        OffendingSql sql = new OffendingSql();
                    //        sql.Script = script;
                    //        sql.StatementSelection = fragment.GetSelectionBounds();
                    //        recommendation = new TSqlRecommendation(RecommendationType.LeftExpressionInWhereClause);
                    //        recommendation.Sql = sql;
                    //        recommendations.Add(recommendation);
                    //    }
                    //    recommendation.Sql.FocusSelections.Add(frag.GetSelectionBounds());
                    //}
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