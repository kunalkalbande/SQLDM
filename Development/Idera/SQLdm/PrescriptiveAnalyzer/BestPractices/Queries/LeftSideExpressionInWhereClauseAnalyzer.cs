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
    using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
    using Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries;
    using System;

    internal class LeftSideExpressionInWhereClauseAnalyzer : AbstractQueryAnalyzer
    {

        private int totalBatches;
        private int totalStatements;
        private int totalProblemBatches;
        //private int totalProblemStatements;
        private const Int32 id = 106;

        public LeftSideExpressionInWhereClauseAnalyzer(string database) : base(database)
        {
            _id = id;
        }

        public override void Analyze(string script, TSqlStatement fragment)
        {
            bool problemStatementsInBatch = false;
            bool problemInStatement = false;

            IEnumerator<TSqlFragment> e = TSqlParsingHelpers.GetDepthFirstEnumerator(fragment, true);
            while (e.MoveNext())
            {
                TSqlFragment frag = e.Current;
                if (frag is TSqlBatch)
                {
                    totalBatches++;
                    if (problemStatementsInBatch)
                        totalProblemBatches++;

                    problemStatementsInBatch = false;
                    problemInStatement = false;
                    continue;
                }
                if (frag is TSqlStatement)
                {
                    totalStatements++;
                    problemInStatement = false;
                    continue;
                }

                if (problemInStatement)
                    continue;

                if (frag is WhereClause)
                {
                    WhereClause c = (WhereClause)frag;

                    BinaryExpression be = new BinaryExpression();


//                    if (c.ColumnType == ColumnType.Wildcard)
//                    {
//                        totalProblemStatements++;
//                        problemInStatement = true;
//                        problemStatementsInBatch = true;
//                    }
                }
            }

            if (problemStatementsInBatch)
                totalProblemBatches++;
        }
    }
}
