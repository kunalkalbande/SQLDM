using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using System.Data;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Ranking
{
    public class RecommendationRankingComparer : IComparer<IRecommendation>
    {
        //private AnalysisConfiguration _config = null;
        private string _filterDatabase = string.Empty;
        private string _filterApplication = string.Empty;
        private RankingStats _rankingStats = null;

        public RecommendationRankingComparer(string filterDatabase, string filterApplication, SqlConnectionInfo info) : base() 
        {
            //_config = config;
            //_rankingStats = new RankingStats(config.ConnectionInfo);
            _filterDatabase = filterDatabase;
            _filterApplication = filterApplication;
            _rankingStats = new RankingStats(info); ;
        }

        public int Compare(IRecommendation x, IRecommendation y)
        {
            //x.ComputeRankFactor(_config, _rankingStats);
            //y.ComputeRankFactor(_config, _rankingStats);
            int result = y.ComputedRankFactor.CompareTo(x.ComputedRankFactor);
            //----------------------------------------------------------------------------
            // If both recommendations have the same rank factor, compare the relevance
            // 
            if (0 == result) result = y.Relevance.CompareTo(x.Relevance);
            //----------------------------------------------------------------------------
            // If both recommendations have the same rank factor and relevance, compare the recommendation type
            // 
            if (0 == result) result = y.RecommendationType.CompareTo(x.RecommendationType);
            return (result);
        }

        public void ComputeRankFactor(IEnumerable<IRecommendation> recs)
        {
            foreach (IRecommendation r in recs)
            {
                r.ComputeRankFactor(_filterDatabase,_filterApplication, _rankingStats);
            }
        }
    }
}
