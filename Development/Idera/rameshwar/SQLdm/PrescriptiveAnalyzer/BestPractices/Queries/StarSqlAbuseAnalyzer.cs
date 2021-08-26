//------------------------------------------------------------------------------
// <copyright file="StarSqlAbuseAnalyzer.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;

namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    using System.Collections;
    using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
    using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
    using Microsoft.Data.Schema.ScriptDom.Sql;
    using Idera.SQLdm.PrescriptiveAnalyzer.Properties;
    using System;

    internal class SelectStarAbuseAnalyzer : AbstractQueryAnalyzer
    {
        private const Int32 id = 114;
        public SelectStarAbuseAnalyzer(string database) : base(database)
        {
            _id = id;
        }

        public override void Analyze(string script, TSqlStatement fragment)
        {
            // allow the base class a chance to handle this statement
            base.Analyze(script, fragment);
         
            TSqlFragmentWalker walker = new TSqlFragmentWalker();
            walker.DescendFilter = ReturnChildren;
            IEnumerator<TSqlFragment> e = walker.GetEnumerator(fragment, true);
            while (e.MoveNext())
            {
                TSqlFragment frag = e.Current;
                if (frag is SelectStatement)
                {
                    Analyze(script, (SelectStatement)frag);
                }
            }
        }

        private void Analyze(string script, SelectStatement select)
        {
            QuerySpecification spec = select.QueryExpression as QuerySpecification;
            if (spec == null)
                return;

            foreach (TSqlFragment element in spec.SelectElements)
            {
                if (element is SelectColumn)
                {
                    Column c = ((SelectColumn)element).Expression as Column;
                    if (c != null && c.ColumnType == ColumnType.Wildcard)
                    {
                        OffendingSql sql = new OffendingSql();
                        sql.Script = script;
                        sql.StatementSelection = TSqlParsingHelpers.GetSelectionRectangle(select);
                        sql.FocusSelections.Add(TSqlParsingHelpers.GetSelectionRectangle(element));

                        TSqlRecommendation recommendation = new TSqlRecommendation(RecommendationType.SelectStarAbuse, Database, Application, User, Host);
                        recommendation.Sql = sql;

                        if (spec.FromClauses.Count > 0)
                        {
                            Dictionary<string, TableSource> tableSources = TSqlParsingHelpers.GetTableSources(select);
                            if (c.Identifiers.Count == 0)
                            {
                                int ttcount = TSqlParsingHelpers.GetTableSourceCount(tableSources.Values, true, true);
                                recommendation.UsesTempTable = ttcount > 0;
                            }
                            else
                            {
                                string tableNameOrAlias = c.Identifiers[0].Value;

                                string tableName = TSqlParsingHelpers.TryGetTableName(tableNameOrAlias, tableSources);
                                if (!string.IsNullOrEmpty(tableName) && (tableName[0] == '#' || tableName[0] == '@'))
                                    recommendation.UsesTempTable = true;
                            }
                        }

                        if (recommendation.UsesTempTable && !Settings.Default.BP_SelectStarAbuseIncludeTempTables)
                            continue;

                        AddRecommendation(recommendation);
                        break;
                    }
                }
            }
        }

        private bool ReturnChildren(TSqlFragment fragment)
        {
            if (fragment is FunctionCall)
            {
                switch (((FunctionCall)fragment).FunctionName.Value.ToLower())
                {
                    case "count":
                        return false;
                }
            }
            else if (fragment is ExistsPredicate)
                return false;
            else if (fragment is Subquery)
                return false;
            else if (fragment is SelectStatement)
                return false;

                return true;
        }
    }
}
