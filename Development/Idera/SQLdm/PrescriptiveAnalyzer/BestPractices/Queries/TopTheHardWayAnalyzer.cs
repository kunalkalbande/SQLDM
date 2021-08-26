using System.Collections.Generic;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Microsoft.Data.Schema.ScriptDom.Sql;
using System.Diagnostics;
using System;
//using System.Linq;

namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    internal class TopTheHardWayAnalyzer : AbstractQueryAnalyzer
    {
        private const Int32 id = 115;
        public TopTheHardWayAnalyzer(string database) : base(database)
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
                    if (state is WhereClause && CheckSubquery((Subquery)frag))
                    {
                        if (recommendation == null)
                        {                            
                            OffendingSql sql = new OffendingSql();
                            sql.Script = script;
                            sql.StatementSelection = fragment.GetSelectionBounds();
                            recommendation = new TSqlRecommendation(RecommendationType.AnsiTopEquivalent, Database, Application, User, Host);
                            recommendation.Sql = sql;
                            AddRecommendation(recommendation);
                        }
                        recommendation.Sql.FocusSelections.Add(frag.GetSelectionBounds());
                    }
                    stateStack.Push(state);
                    state = frag;
                }
            }
        }

        private bool CheckSubquery(Subquery subquery)
        {
            SubquerySpecification spec = subquery.QueryExpression as SubquerySpecification;
            if (spec != null)
            {
                
                List<SelectColumn> list = new List<SelectColumn>();
                foreach (TSqlFragment f in spec.SelectElements)
                {
                    if (f.GetType() == typeof(SelectColumn))
                    {
                        list.Add((SelectColumn)f);
                    }
                }
                //foreach (SelectColumn selectColumn in spec.SelectElements.OfType<SelectColumn>())
                //{
                //    if (selectColumn.Expression != null && selectColumn.Expression is FunctionCall)
                //    {
                //        FunctionCall functionCall = (FunctionCall)selectColumn.Expression;
                //        string functionName = functionCall.FunctionName.Value;
                //        if (!string.IsNullOrEmpty(functionName) && functionName.Equals("count", System.StringComparison.InvariantCultureIgnoreCase))
                //        {
                //            return true;
                //        }
                //    }
                //}
                foreach (SelectColumn selectColumn in list)
                {
                    if (selectColumn.Expression != null && selectColumn.Expression is FunctionCall)
                    {
                        FunctionCall functionCall = (FunctionCall)selectColumn.Expression;
                        string functionName = functionCall.FunctionName.Value;
                        if (!string.IsNullOrEmpty(functionName) && functionName.Equals("count", System.StringComparison.InvariantCultureIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
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
