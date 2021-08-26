using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    internal class NestedMinMaxAnalyzer : AbstractQueryAnalyzer
    {
        private const Int32 id = 111;
        public NestedMinMaxAnalyzer(string database) : base(database)
        {
            _id = id;
        }

        public override void Analyze(string script, TSqlStatement fragment)
        {
            // allow the base class a chance to handle this statement
            base.Analyze(script, fragment);

            TSqlNestedMinMaxRecommendation recommendation = null;
            SelectionRectangle lastProblemArea = null;

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
                if (frag is FunctionCall)
                {
                    string innerFunctionName;
                    if (CheckFunction((FunctionCall)frag, out innerFunctionName))
                    {
                        if (recommendation == null)
                        {
                            OffendingSql sql = new OffendingSql();
                            sql.Script = script;
                            sql.StatementSelection = fragment.GetSelectionBounds();
                            recommendation = new TSqlNestedMinMaxRecommendation(RecommendationType.NestedMinMax, Database, Application, User, Host);
                            recommendation.InnerFunctionName = innerFunctionName;
                            recommendation.Sql = sql;
                            AddRecommendation(recommendation);
                        }
                        SelectionRectangle rect = frag.GetSelectionBounds();
                        if (lastProblemArea == null || rect.Start.Offset > lastProblemArea.Start.Offset + lastProblemArea.Length)
                        {
                            lastProblemArea = rect;
                            recommendation.Sql.FocusSelections.Add(rect);
                        }
                    }
                }
            }
        }

        private bool CheckFunction(FunctionCall functionCall, out string innerFunctionName)
        {
            innerFunctionName = null;

            string functionName = functionCall.FunctionName.Value.ToLowerInvariant();
            switch (functionName)
            {
                case "min":
                case "max":
                    break;
                default:
                    return false;
            }

            if (functionCall.Parameters != null && functionCall.Parameters.Count == 1)
            {
                PrimaryExpression expression = functionCall.Parameters[0] as PrimaryExpression;
                if (expression != null)
                {
                    if (expression is Column || expression is Literal || expression is ParenthesisExpression)
                        return false;
                    else
                    {
                        if (expression is FunctionCall)
                            innerFunctionName = ((FunctionCall)expression).FunctionName.Value;
                        else
                        if (expression is CastCall)
                            innerFunctionName = "CAST";
                        else
                        if (expression is ConvertCall)
                            innerFunctionName = "CONVERT";
                        else
                        if (expression is UserDefinedTypePropertyAccess)
                            innerFunctionName = "Udt";
                        else
                        if (expression is LeftFunctionCall)
                            innerFunctionName = "LEFT";
                        else
                        if (expression is RightFunctionCall)
                            innerFunctionName = "RIGHT";
                        else
                        if (expression is PartitionFunctionCall)
                            innerFunctionName = "Partition";
                        else
                        if (expression is ParameterlessCall)
                            innerFunctionName = "Call";
                        else
                        if (expression is Subquery)
                            innerFunctionName = "Subquery";
                        else
                        if (expression is OdbcFunctionCall)
                            innerFunctionName = "ODBC";
                        else
                        if (expression is CaseExpression)
                            innerFunctionName = "CASE";
                        else
                        if (expression is NullIfExpression)
                            innerFunctionName = "NULLIF";
                        else
                        if (expression is CoalesceExpression)
                            innerFunctionName = "COALESCE";
                        else
                        {
                            innerFunctionName = expression.GetType().Name;
                        }

                        return true;
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
