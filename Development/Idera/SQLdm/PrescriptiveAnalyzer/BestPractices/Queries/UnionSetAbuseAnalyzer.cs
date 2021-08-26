//------------------------------------------------------------------------------
// <copyright file="StarSqlAbuseAnalyzer.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;

namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    using System.Collections;
    using System.IO;
    using Microsoft.Data.Schema.ScriptDom;
    using Microsoft.Data.Schema.ScriptDom.Sql;
    using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
    using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
    using Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries;
    using System;

    internal class UnionSetAbuseAnalyzer : AbstractQueryAnalyzer
    {
        private const Int32 id = 119;

        public UnionSetAbuseAnalyzer(string database) : base(database)
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
            List<BinaryQueryExpression> unionExpressions = new List<BinaryQueryExpression>();

            while (e.MoveNext())
            {
                TSqlFragment frag = e.Current;
                if (frag is TSqlStatement)
                {
                    if (unionExpressions.Count > 0)
                        Analyze(script, currentStatement, unionExpressions);
                    currentStatement = (TSqlStatement)frag;
                    unionExpressions.Clear();
                    continue;
                }

                if (frag is BinaryQueryExpression && ((BinaryQueryExpression)frag).BinaryQueryExpressionType == BinaryQueryExpressionType.Union)
                {
                    if (((BinaryQueryExpression)frag).All)
                        continue;

                    unionExpressions.Add((BinaryQueryExpression)frag);
                }
            }
            if (unionExpressions.Count > 0)
                Analyze(script, currentStatement, unionExpressions);
        }

        private void Analyze(string script, TSqlStatement currentStatement, List<BinaryQueryExpression> unionExpressions)
        {
            OffendingSql sql = new OffendingSql();
            sql.Script = script;
            sql.StatementSelection = TSqlParsingHelpers.GetSelectionRectangle(currentStatement);

            for (int j = unionExpressions.Count - 1; j >= 0; j--)
            {
                BinaryQueryExpression bqe = unionExpressions[j];
                TSqlParserToken unionToken = null;
                // the union token lies between the last token of the first query and the first token of the last query
                for (int i = bqe.FirstQueryExpression.LastTokenIndex; i < bqe.SecondQueryExpression.FirstTokenIndex; i++)
                {
                    if (bqe.ScriptTokenStream[i].TokenType == TSqlTokenType.Union)
                    {
                        unionToken = bqe.ScriptTokenStream[i];
                        break;
                    }
                }
                if (unionToken != null)
                    sql.FocusSelections.Add(TSqlParsingHelpers.GetSelectionRectangle(unionToken));
            }

            TSqlRecommendation recommendation = new TSqlRecommendation(RecommendationType.UnionSetAbuse, Database, Application, User, Host);
            recommendation.Sql = sql;
            AddRecommendation(recommendation);
        }
    }
}
