using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    // Scan for composite expression in a where clause that includes a column
    internal class CompositeExpressionSearchAnalyzer : AbstractQueryAnalyzer
    {
        private const Int32 id = 102;
        internal CompositeExpressionSearchAnalyzer(string database) : base(database)
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
            TSqlFragment outerExpression = null;
            //TSqlFragment lastErrorExpression = null;

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

                    if (frag is BinaryExpression)
                    {
                        if (outerExpression == peState)
                        {
                            outerExpression = null;
                            //lastErrorExpression = null;
                        }
                        peState = peStack.Pop();
                    }
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
                    if (state is WhereClause && peState is BinaryExpression)
                    {
                        if (recommendation == null)
                        {
                            //    OffendingSql sql = new OffendingSql();
                            //    sql.Script = script;
                            //    sql.StatementSelection = fragment.GetSelectionBounds();
                            //    recommendation = new TSqlRecommendation(RecommendationType.CompositeExpressionInPredicate);
                            //    recommendation.Sql = sql;
                            //    recommendations.Add(recommendation);
                            //}
                            //if (outerExpression != null && outerExpression != lastErrorExpression)
                            //{
                            //    recommendation.Sql.FocusSelections.Add(peState.GetSelectionBounds());
                            //    lastErrorExpression = outerExpression;
                            //}
                        }
                    }
                }
                else
                if (frag is BinaryExpression)
                {
                    if (InterestingBinaryExpression((BinaryExpression)frag))
                    {
                        peStack.Push(peState);
                        peState = frag;

                        if (outerExpression == null || !outerExpression.Contains(frag))
                        {
                            outerExpression = frag;
                        }
                    }
                }
            }
        }

        private bool InterestingBinaryExpression(BinaryExpression be)
        {
            switch (be.BinaryExpressionType)
            {
                case BinaryExpressionType.Add:
                case BinaryExpressionType.BitwiseAnd:
                case BinaryExpressionType.BitwiseOr:
                case BinaryExpressionType.BitwiseXor:
                case BinaryExpressionType.Divide:
                case BinaryExpressionType.Modulo:
                case BinaryExpressionType.Multiply:
                case BinaryExpressionType.Subtract:
                    return true;
            }
            return false;
        }

        private bool PushEndFilter(TSqlFragment fragment)
        {
            if (fragment is WhereClause)
                return true;

            if (fragment is Subquery)
                return true;

            if (fragment is ParenthesisExpression)
                return false;

            if (fragment is BinaryExpression)
                return InterestingBinaryExpression((BinaryExpression)fragment);

            return false;
        }
    }
}
