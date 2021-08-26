using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Microsoft.Data.Schema.ScriptDom;
using System.IO;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Stats;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.MissingIndexes;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Collectors;
using Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan.Analyzers;
//using Idera.SQLdm.PrescriptiveAnalyzer.Engine;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.State;
using Idera.SQLdm.Common.Configuration;


namespace Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan
{
    internal class ExecutionPlanAnalyzer
    {
        private List<IAnalyzePlan> _analyzers;
        private BaseOptions _ops;

        private MissingIndexAnalyzer _missingIndexAnalyzer = null;

        public HypotheticalIndexCleanupThread IndexCleanupThread
        {
            get 
            {
                if (null == _missingIndexAnalyzer) return null;
                return (_missingIndexAnalyzer.IndexCleanupThread);
            }
        }

        private ExecutionPlanAnalyzer() { }
        internal ExecutionPlanAnalyzer(BaseOptions ops, SqlSystemObjectManager ssom, int traceDurationMintues)
        {
            _ops = ops;
            _analyzers = GetPlanAnalyzers(ops, ssom, traceDurationMintues);
        }

        public List<IRecommendation> GetRecommendations(AnalysisConfiguration config)
        {
            List<IRecommendation> list = new List<IRecommendation>();
            foreach (IAnalyzePlan analyzer in _analyzers)
            {
                IEnumerable<IRecommendation> recommendations = analyzer.GetRecommendations();
                if (null != recommendations)
                {
                    foreach (IRecommendation r in recommendations)
                    {
                        bool addRecoInResult = true;
                        if (config != null && config.BlockedRecommendationID != null && config.BlockedRecommendationID.Contains(r.ID))
                        {
                            addRecoInResult = false;
                        }
                        if (config != null && config.BlockedCategories != null && config.BlockedCategories.ContainsValue(r.Category))
                        {
                            addRecoInResult = false;
                        }

                        if (addRecoInResult == true)
                        {
                            list.Add(r);
                        }
                    }
                }
            }
            return list;
        }

        public List<AnalyzerResult> GetAnalyzerRecommendations(AnalysisConfiguration config)
        {
            List<AnalyzerResult> aRes = new List<AnalyzerResult>();
            foreach (IAnalyzePlan analyzer in _analyzers)
            {
                AnalyzerResult res = new AnalyzerResult();
                IEnumerable<IRecommendation> recommendations = analyzer.GetRecommendations();
                foreach (IRecommendation r in recommendations)
                {
                    res.AnalyzerID = analyzer.ID;
                    bool addRecoInResult = true;
                    if (config != null && config.BlockedRecommendationID != null && config.BlockedRecommendationID.Contains(r.ID))
                    {
                        addRecoInResult = false;
                    }
                    if (config != null && config.BlockedCategories != null && config.BlockedCategories.ContainsValue(r.Category))
                    {
                        addRecoInResult = false;
                    }

                    if (addRecoInResult == true)
                    {
                        res.RecommendationList.Add(r);
                    }
                }
                if (res.RecommendationList.Count > 0) { aRes.Add(res); }
            }
            return aRes;
        }

        public IEnumerable<Exception> GetExceptions()
        {
            foreach (IAnalyzePlan analyzer in _analyzers)
            {
                IEnumerable<Exception> ex = analyzer.GetExceptions();
                if (null != ex)
                {
                    foreach (Exception e in ex)
                    {
                        yield return e;
                    }
                }
            }
        }

        internal void Analyze(TraceEventStatsCollection tesc)//, Progress progress)
        {
            if (null == tesc) return;

            int step = 0;

            //progress.AddAnalysisWork(tesc.Count * _analyzers.Count);
            _ops.UpdateState(AnalysisStateType.ExecutionPlanAnalysis, "Analyzing execution plans...", 0, _analyzers.Count);
            foreach (IAnalyzePlan iap in _analyzers)
            {
                _ops.UpdateState(AnalysisStateType.ExecutionPlanAnalysis, step, _analyzers.Count);
                //progress.SetStepName(String.Format("Analyzing query plans - step {0} of {1}", ++step, _analyzers.Count));
                iap.Analyze(tesc);
                //progress.AnalysisWorkCompleted(tesc.Count);
            }
            _ops.UpdateState(AnalysisStateType.ExecutionPlanAnalysis, "Completed execution plan analysis.", 100, 100);
        }

        private List<IAnalyzePlan> GetPlanAnalyzers(BaseOptions ops, SqlSystemObjectManager ssom, int traceDurationMintues)
        {
            List<IAnalyzePlan> list = new List<IAnalyzePlan>();
            list.Add(_missingIndexAnalyzer = new MissingIndexAnalyzer(ops, ssom, traceDurationMintues));
            list.Add(new EarlyAbortAnalyzer(ops, ssom));
            list.Add(new WarningAnalyzer(ops, ssom));
            list.Add(new OperatorsAnalyzer(ssom));
            list.Add(new NotInSubqueryAnalyzer(ssom));
            return list;
        }
    }
}
