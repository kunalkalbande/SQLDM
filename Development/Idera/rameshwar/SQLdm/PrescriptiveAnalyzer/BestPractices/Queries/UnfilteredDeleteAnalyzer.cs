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
    using System;

    internal class UnfilteredDeleteAnalyzer : AbstractQueryAnalyzer
    {
        private const Int32 id = 118;
        public UnfilteredDeleteAnalyzer(string database) : base(database)
        {
            _id = id;
        }

        public override void Analyze(string script, TSqlStatement fragment)
        {
            // allow the base class a chance to handle this statement
            base.Analyze(script, fragment);

            TSqlFragmentWalker walker = new TSqlFragmentWalker();
            IEnumerator<TSqlFragment> e = walker.GetEnumerator(fragment, true);
            while (e.MoveNext())
            {
                TSqlFragment frag = e.Current;
                if (frag is DeleteStatement)
                {
                    Analyze(script, (DeleteStatement)frag);
                }
            }
        }

        public void Analyze(string script, DeleteStatement fragment)
        {
            if (fragment.WhereClause == null && 
                fragment.Target is SchemaObjectDataModificationTarget &&
                (fragment.WithCommonTableExpressionsAndXmlNamespaces == null || fragment.WithCommonTableExpressionsAndXmlNamespaces.CommonTableExpressions.Count == 0))
            {
                Dictionary<string,TableSource> sources = TSqlParsingHelpers.GetTableSources(fragment.FromClauses);
                if (sources.Count <= 1)
                {
                    SchemaObjectDataModificationTarget sodmt = (SchemaObjectDataModificationTarget)fragment.Target;
                    string key = TSqlParsingHelpers.GetNormalizedName(sodmt.SchemaObject.Identifiers);
                    string target = TSqlParsingHelpers.GetNormalizedName(sodmt.SchemaObject.Identifiers, false);

                    string table = sodmt.SchemaObject.BaseIdentifier.Value;

                    string schema = String.Empty;
                    if (sodmt.SchemaObject.SchemaIdentifier != null)
                        schema = sodmt.SchemaObject.SchemaIdentifier.Value;
                   
                    if (sources.ContainsKey(key))
                    {
                        SchemaObjectTableSource ts = sources[key] as SchemaObjectTableSource;
                        if (ts == null)
                            return;

                        target = TSqlParsingHelpers.GetNormalizedName(ts.SchemaObject.Identifiers, false);
                        schema = String.Empty;
                        if (ts.SchemaObject.SchemaIdentifier != null)
                            schema = ts.SchemaObject.SchemaIdentifier.Value;
                        table = ts.SchemaObject.BaseIdentifier.Value;
                    }
                    
                    OffendingSql sql = new OffendingSql();
                    sql.Script = script;
                    sql.StatementSelection = TSqlParsingHelpers.GetSelectionRectangle(fragment);

                    TSqlRecommendationWithTable recommendation = new TSqlRecommendationWithTable(RecommendationType.UnfilteredDelete, Database, Application, User, Host);
                    recommendation.Sql = sql;
                    recommendation.TableName = target;

                    DatabaseObjectName sourceObject = new DatabaseObjectName();
                    sourceObject.DatabaseName = this.Database;

                    if (!String.IsNullOrEmpty(schema))
                        sourceObject.SchemaName = schema;
                    sourceObject.ObjectName = table;
                    recommendation.AddSourceObject(sourceObject);

	                AddRecommendation(recommendation);  
                }
            }
        }
    }
}
