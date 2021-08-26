using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    internal class HintAbuseAnalyzer : AbstractQueryAnalyzer
    {
        private const Int32 id = 105;

        public HintAbuseAnalyzer(string database) : base(database)
        {
            _id = id;
        }

        public override void Analyze(string script, TSqlStatement fragment)
        {
            // allow the base class a chance to handle this statement
            base.Analyze(script, fragment);

            TSqlStatement currentStatement = null;
            TSqlFragmentWalker walker = new TSqlFragmentWalker();
            walker.DescendFilter = ReturnChildren;
            IEnumerator<TSqlFragment> e = walker.GetEnumerator(fragment, true);

            List<TSqlFragment> hints = new List<TSqlFragment>();
            while (e.MoveNext())
            {
                TSqlFragment frag = e.Current;
                if (frag is TSqlStatement)
                {
                    if (hints.Count > 0)
                    {
                        CreateRecommendation(script, currentStatement, hints);
                        hints.Clear();
                    }
                    currentStatement = (TSqlStatement)frag;
                } else
                if (frag is SimpleOptimizerHint)
                {
                    switch (((SimpleOptimizerHint)frag).HintKind)
                    {   // skip FAST and MaxDop hints
                        case SimpleOptimizerHintKind.Fast:
                        case SimpleOptimizerHintKind.MaxDop:
                            break;
                        default:
                            hints.Add(frag);
                            break;
                    }
                }
                else
                if (frag is TableHintsOptimizerHint)
                {
                    if (((TableHintsOptimizerHint)frag).TableHints.Count == 1)
                    {   // don't ding em for nolock hints
                        //SimpleTableHint sth = ((TableHintsOptimizerHint)frag).TableHints.First() as SimpleTableHint;
                        SimpleTableHint sth = ((TableHintsOptimizerHint)frag).TableHints[0] as SimpleTableHint;
                        if (sth != null && sth.HintKind == SimpleTableHintKind.NoLock)
                            continue;
                    }
                    hints.Add(frag);
                }
                else
                if (frag is TableHint)
                {   // don't ding em for nolock hints
                    if (frag is SimpleTableHint)
                    {   switch (((SimpleTableHint)frag).HintKind)
                        {
                            case SimpleTableHintKind.NoLock:
                            case SimpleTableHintKind.FastFirstRow:
                                continue;
                        }
                    }
                    hints.Add(frag);
                }
                else
                if (frag is QualifiedJoin && ((QualifiedJoin)frag).JoinHint != JoinHint.None)
                {
                    hints.Add(frag);
                }
            }
            if (hints.Count > 0)
                CreateRecommendation(script, currentStatement, hints);
        }

        private void CreateRecommendation(string script, TSqlStatement currentStatement, List<TSqlFragment> hints)
        {
            if (currentStatement == null)
                return;

            OffendingSql sql = new OffendingSql();
            sql.Script = script;
            sql.StatementSelection = TSqlParsingHelpers.GetSelectionRectangle(currentStatement);

            bool didOptimizerHints = false;

            foreach (TSqlFragment hint in hints)
            {
                if (hint is OptimizerHint)
                {   // only do this once no matter how many optimizer hints we found
                    if (!didOptimizerHints)
                    {
                        didOptimizerHints = true;
                        SelectionRectangle rect = TSqlParsingHelpers.GetOptionClauseBounds(currentStatement);
                        if (rect != null)
                            sql.FocusSelections.Add(rect);
                    }
                }
                else
                if (hint is QualifiedJoin)
                {
                    SelectionRectangle rect = ((QualifiedJoin)hint).GetJoinHintBounds();
                    if (rect != null)
                        sql.FocusSelections.Add(rect);
                }
                else
                    sql.FocusSelections.Add(TSqlParsingHelpers.GetSelectionRectangle(hint));
            }

            TSqlHintRecommendation recommendation = new TSqlHintRecommendation(RecommendationType.HintAbuse, Database, Application, User, Host);
            recommendation.Sql = sql;

            Stack<TSqlFragment> hintStack = new Stack<TSqlFragment>(hints);
            while (hintStack.Count > 0)
            {
                TSqlFragment frag = hintStack.Pop();
                if (frag is QualifiedJoin && ((QualifiedJoin)frag).JoinHint != JoinHint.None)
                {
                    recommendation.AddHint(((QualifiedJoin)frag).JoinHint.ToString());
                }
                else
                if (frag is SimpleTableHint)
                {
                    recommendation.AddHint(((SimpleTableHint)frag).HintKind.ToString());
                }
                else
                if (frag is SimpleOptimizerHint)
                {
                    recommendation.AddHint(((SimpleOptimizerHint)frag).HintKind.ToString());
                }
                else
                if (frag is TableHintsOptimizerHint)
                {
                    foreach (TableHint hint in ((TableHintsOptimizerHint)frag).TableHints)
                    {
                        hintStack.Push(hint);
                    }
                } 
                else
                if (frag is IndexTableHint)
                {
                    recommendation.AddHint("Index");
                }
            }

            AddRecommendation(recommendation);
        }

        private bool ReturnChildren(TSqlFragment fragment)
        {
            if (fragment is TableHintsOptimizerHint)
                return false;
            else if (fragment is TableHint)
                return false;
            else if (fragment is OptimizerHint)
                return false;

            return true;
        }
    }
}
