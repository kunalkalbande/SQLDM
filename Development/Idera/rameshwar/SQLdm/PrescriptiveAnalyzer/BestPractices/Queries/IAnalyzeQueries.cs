using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    internal interface IAnalyzeQueries
    {
        Int32 ID { get; }
        string Application { set; }
        string Database { set; }
        string Host { set; }
        string User { set; }
        // analyze a TSQL script
        void Analyze(string script, TSqlScript scriptParseTree);
        // analyze a TSQL batch
        void Analyze(string script, TSqlBatch batchParseTree);
        // analyze a TSQL statement
        void Analyze(string script, TSqlStatement stmtParseTree);

        // clear recommendations
        void Clear();
        // get recommendations
        IRecommendation[] GetRecommendations();
    }
}
