using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    public class UpdateStatisticsScriptGenerator : IScriptGenerator
    {
        private IndexRecommendation _indexRecommendation;
        public UpdateStatisticsScriptGenerator(IndexRecommendation indexRecommendation)
        {
            _indexRecommendation = indexRecommendation;
        }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            return (FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.UpdateStatistics.sql"), _indexRecommendation));
        }
    }
}
