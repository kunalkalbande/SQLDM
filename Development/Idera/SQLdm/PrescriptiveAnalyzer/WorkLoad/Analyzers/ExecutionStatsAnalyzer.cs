using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Stats;
using Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan.Analyzers;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Collectors;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.State;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Analyzers
{
    internal class ExecutionStatsAnalyzer 
    {
        private List<IAnalyzeDataBucket> _analyzers = new List<IAnalyzeDataBucket>();
        private BaseOptions _ops = null;

        public ExecutionStatsAnalyzer(BaseOptions ops, SqlSystemObjectManager ssom)
        {
            _ops = ops;
            _analyzers.AddRange(GetAnalyzers(ops, ssom));
        }

        internal void Analyze(List<DataBucketRanking> bucketList)
        {
            if (null == bucketList) return;
            _ops.UpdateState(AnalysisStateType.ExecutionStatsAnalysis, "Analyzing execution stats", 0, 100);
            try
            {
                int current = 0;
                foreach (DataBucketRanking bucket in bucketList)
                {
                    _ops.UpdateState(AnalysisStateType.ExecutionStatsAnalysis,  current++, bucketList.Count);
                    foreach (IAnalyzeDataBucket iadb in _analyzers)
                    {
                        iadb.Analyze(bucket.Bucket);
                    }
                }
            }
            finally
            {
                _ops.UpdateState(AnalysisStateType.ExecutionStatsAnalysis, "Completed execution stats analysis.", 100, 100);
            }
        }

        public List<IRecommendation> GetRecommendations(AnalysisConfiguration config)
        {
            List<IRecommendation> list = new List<IRecommendation>();
            foreach (IAnalyzeDataBucket analyzer in _analyzers)
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
                    }
                }
            }
            return list;
        }

        public List<AnalyzerResult> GetAnalyzerRecommendations(AnalysisConfiguration config)
        {
            List<AnalyzerResult> aRes = new List<AnalyzerResult>();
            foreach (IAnalyzeDataBucket analyzer in _analyzers)
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

        private IEnumerable<IAnalyzeDataBucket> GetAnalyzers(BaseOptions ops, SqlSystemObjectManager ssom)
        {
            yield return new ExecutionStatsVariationAnalyzer(ops, ssom);
        }
    }
}
