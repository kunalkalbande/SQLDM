using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using System.Data;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Ranking
{
    public class RankingStats
    {
        private TableRankingStats _tableStats = null;
        private DatabaseRankingStats _databaseStats = null;

        private RankingStats() { }
        public RankingStats(SqlConnectionInfo info) 
        {
            try
            {
                _tableStats = new TableRankingStats(info);
                _databaseStats = new DatabaseRankingStats(info);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("RankingStats() Exception: ", ex);
            }
        }

        public double GetPercentile(IProvideDatabase r)
        {
            if (null != _databaseStats)
            {
                double d = _databaseStats.GetPercentile(r) / 2;
                if (d > 1.0) return (1.0);
                return (d);
            }
            return (0.0);
        }

        public double GetPercentile(IndexRecommendation r)
        {
            double tp = 0;
            double dp = 0;

            if (null != _tableStats)
            {
                tp = _tableStats.GetPercentile(r);
            }
            if (null != _databaseStats)
            {
                dp = _databaseStats.GetPercentile(r) / 2;
            }
            dp += tp;
            if (dp > 1.0) return (1.0);
            return (dp);
        }

    }
}
