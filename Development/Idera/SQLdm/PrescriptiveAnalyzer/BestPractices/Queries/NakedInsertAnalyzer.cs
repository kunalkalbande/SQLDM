using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    internal class NakedInsertAnalyzer : AbstractQueryAnalyzer
    {
        private const Int32 id = 110;
        public NakedInsertAnalyzer(string database) : base(database)
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
                if (frag is InsertStatement)
                {
                    Analyze(script, (InsertStatement)frag);
                }
            }
        }

        public void Analyze(string script, InsertStatement insert)
        {
            if (insert.Columns == null || insert.Columns.Count == 0)
            {
                OffendingSql sql = new OffendingSql();
                sql.Script = script;
                sql.StatementSelection = TSqlParsingHelpers.GetSelectionRectangle(insert);
                sql.FocusSelections.Add(TSqlParsingHelpers.GetSelectionRectangle(insert.Target));

                TSqlRecommendation recommendation = new TSqlRecommendation(RecommendationType.NakedInsert, Database, Application, User, Host);
                recommendation.Sql = sql;

                AddRecommendation(recommendation);  
            }
        }
    }
}
