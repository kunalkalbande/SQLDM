//------------------------------------------------------------------------------
// <copyright file="StarSqlAbuseAnalyzer.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;

namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    using System;
    using System.Collections;
    using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
    using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
    using Microsoft.Data.Schema.ScriptDom.Sql;

    internal class TopVsRowCountAnalyzer : AbstractQueryAnalyzer
    {
        bool rowcount_on;
        bool select_only;
        private const Int32 id = 116;
        
        public TopVsRowCountAnalyzer(string database) : base(database)
        {
            _id = id;
        }

        public override void Analyze(string script, TSqlBatch batchParseTree)
        {
            rowcount_on = false;
            select_only = true;

            if (ServerVersion != null && ServerVersion.Major > 9)
                select_only = false;

            base.Analyze(script, batchParseTree);
        }


        public override void Analyze(string script, TSqlStatement fragment)
        {
            TSqlFragmentWalker walker = new TSqlFragmentWalker();
            walker.DescendFilter = ReturnChildren;
            IEnumerator<TSqlFragment> e = walker.GetEnumerator(fragment, true);
            while (e.MoveNext())
            {
                TSqlFragment frag = e.Current;

                if (frag is SetRowCountStatement)
                {
                    rowcount_on = true;
                    Literal count = ((SetRowCountStatement)frag).NumberRows;
                    if (count.LiteralType != LiteralType.Variable)
                    {
                        int v;
                        if (Int32.TryParse(count.Value, out v))
                        {
                            rowcount_on = v > 0;
                        }
                    }
                    continue;
                }

                if (rowcount_on)
                {
                    if (frag is SelectStatement)
                    {
                        CheckSelect(script, (SelectStatement)frag);
                        continue;
                    }
                    if (!select_only)
                    {
                        if (frag is UpdateStatement)
                        {
                            CheckUpdate(script, (UpdateStatement)frag);
                            continue;
                        }
                        if (frag is DeleteStatement)
                        {
                            CheckDelete(script, (DeleteStatement)frag);
                            continue;
                        }
                    }
                }
            }
        }

        private void CheckDelete(string script, DeleteStatement deleteStatement)
        {
            if (deleteStatement.TopRowFilter != null)
                return;

            OffendingSql sql = new OffendingSql();
            sql.Script = script;
            sql.StatementSelection = TSqlParsingHelpers.GetSelectionRectangle(deleteStatement);

            TSqlRecommendation recommendation = new TSqlRecommendation(RecommendationType.TopVsRowCount, Database, Application, User, Host);
            recommendation.Sql = sql;

            AddRecommendation(recommendation);
        }

        private void CheckUpdate(string script, UpdateStatement updateStatement)
        {
            if (updateStatement.TopRowFilter != null)
                return;

            OffendingSql sql = new OffendingSql();
            sql.Script = script;
            sql.StatementSelection = TSqlParsingHelpers.GetSelectionRectangle(updateStatement);

            TSqlRecommendation recommendation = new TSqlRecommendation(RecommendationType.TopVsRowCount, Database, Application, User, Host);
            recommendation.Sql = sql;

            AddRecommendation(recommendation);
        }

        private void CheckSelect(string script, SelectStatement select)
        {
            QuerySpecification spec = select.QueryExpression as QuerySpecification;
            if (spec == null || spec.TopRowFilter != null)
                return;

            OffendingSql sql = new OffendingSql();
            sql.Script = script;
            sql.StatementSelection = TSqlParsingHelpers.GetSelectionRectangle(select);

            TSqlRecommendation recommendation = new TSqlRecommendation(RecommendationType.TopVsRowCount, Database, Application, User, Host);
            recommendation.Sql = sql;

            AddRecommendation(recommendation);
        }

        private bool ReturnChildren(TSqlFragment fragment)
        {
            if (fragment is ExistsPredicate)
                return false;
            else if (fragment is Subquery)
                return false;
            else if (fragment is SelectStatement)
                return false;
            else if (fragment is InsertStatement)
                return false;
            else if (fragment is DeleteStatement)
                return false;
            else if (fragment is UpdateStatement)
                return false;

            return true;
        }
    }
}
