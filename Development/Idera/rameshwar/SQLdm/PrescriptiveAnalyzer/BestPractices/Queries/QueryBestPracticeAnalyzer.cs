using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Microsoft.Data.Schema.ScriptDom;
using System.IO;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.MetaData;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    internal class QueryBestPracticeAnalyzer
    {
        IEnumerable<IAnalyzeQueries> analyzers;
        ServerVersion version;

        internal QueryBestPracticeAnalyzer(ServerVersion version)
        {
            this.version = version;
            analyzers = GetQueryAnalyzers(String.Empty);
        }

        public List<IRecommendation> GetRecommendations(AnalysisConfiguration config)
        {
            List<IRecommendation> list = new List<IRecommendation>();
            foreach (IAnalyzeQueries analyzer in analyzers)
            {
                IRecommendation[] recommendations = analyzer.GetRecommendations();
                if (recommendations != null && recommendations.Length > 0)
                {
                    for (int i = 0; i < recommendations.Length; i++)
                    {
                        bool addRecoInResult = true;
                        if (config != null && config.BlockedRecommendationID != null && config.BlockedRecommendationID.Contains(recommendations[i].ID))
                        {
                            addRecoInResult = false;
                        }
                        if (config != null && config.BlockedCategories != null && config.BlockedCategories.ContainsValue(recommendations[i].Category))
                        {
                            addRecoInResult = false;
                        }

                        if (addRecoInResult == true)
                        {
                            list.Add(recommendations[i]);
                        }
                        //yield return recommendations[i];
                    }
                }
            }
            return list;
        }

        public List<AnalyzerResult> GetAnalyzerRecommendations(AnalysisConfiguration config)
        {
            List<AnalyzerResult> aRes = new List<AnalyzerResult>();
            foreach (IAnalyzeQueries analyzer in analyzers)
            {
                
                IRecommendation[] recommendations = analyzer.GetRecommendations();
                if (recommendations != null && recommendations.Length > 0)
                {
                    AnalyzerResult res = new AnalyzerResult();
                    res.AnalyzerID = analyzer.ID;
                    for (int i = 0; i < recommendations.Length; i++)
                    {
                        bool addRecoInResult = true;
                        if (config != null && config.BlockedRecommendationID != null && config.BlockedRecommendationID.Contains(recommendations[i].ID))
                        {
                            addRecoInResult = false;
                        }
                        if (config != null && config.BlockedCategories != null && config.BlockedCategories.ContainsValue(recommendations[i].Category))
                        {
                            addRecoInResult = false;
                        }

                        if (addRecoInResult == true)
                        {
                            res.RecommendationList.Add(recommendations[i]);
                        }
                    }
                    aRes.Add(res);
                }
            }
            return aRes;
        }

        internal void AnalyzeScript(string database, string application, string user, string host, string script)
        {
            IList<ParseError> parseErrors;
            TSqlScript parsedScript = ParseScript(script, out parseErrors);
            if (script != null && parseErrors.Count == 0)
            {
                foreach (IAnalyzeQueries iaq in analyzers)
                {
                    iaq.Database = database;
                    iaq.Application = application;
                    iaq.Host = host;
                    iaq.User = user;
                    iaq.Analyze(script, parsedScript);
                }
            }
        }

        private TSqlScript ParseScript(string script, out IList<ParseError> errors)
        {
            using (TextReader reader = new StringReader(script))
            {
                TSqlParser parser = TSqlParsingHelpers.GetParser(version, false);
                return parser.Parse(reader, out errors) as TSqlScript;
            }
        }

        private IEnumerable<IAnalyzeQueries> GetQueryAnalyzers(string database)
        {
            List<IAnalyzeQueries> list = new List<IAnalyzeQueries>();
            list.Add(new SelectStarAbuseAnalyzer(database));
            list.Add(new UnfilteredDeleteAnalyzer(database));
            list.Add(new UnionSetAbuseAnalyzer(database));
            list.Add(new TwoExpressionCoalesceAnalyzer(database));
            list.Add(new TopVsRowCountAnalyzer(database));
            list.Add(new CloseCursorAnalyzer(database));
            list.Add(new CompositeExpressionSearchAnalyzer(database));
            list.Add(new FastForwardCursorAnalyzer(database));
            list.Add(new FunctionInSearchAnalyzer(database));
            list.Add(new HintAbuseAnalyzer(database));
            list.Add(new LeftUsedInSearchAnalyzer(database));
// DR229    list.Add(new NakedInsertAnalyzer(database));
            list.Add(new SelectDistinctAbuseAnalyzer(database));
            list.Add(new TopTheHardWayAnalyzer(database));
            //---------------------------------------------------------------
            // Removed SDR-Q17 and SDR-Q18  
            //
            // list.Add(new NestedMinMaxAnalyzer(database));
            // list.Add(new LikeAbuseAnalyzer(database)); 

            return list;
        }
    }
}
