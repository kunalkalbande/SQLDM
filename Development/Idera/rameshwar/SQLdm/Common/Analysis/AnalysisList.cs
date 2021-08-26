using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Common.Analysis
{
    [Serializable]
    public class AnalysisList
    {
        private Int32 _sqlServerID;
        private Int32 _analysisID;
        private DateTime _analysisStartTime;
        private DateTime _analysisCompleteTime;
        private string _analysisDuration;
        private Int32 _totalRecommendationCount;
        private string _analysisType;
        private Exception _error;
        private float _computedRankFactor;

        public AnalysisList()
        { }

        public string Type
        {
            get { return _analysisType; }
            set { _analysisType = value; }
        }

        public Exception Error
        {
            get { return _error; }
            set { _error = value; }
        }

        public Int32 SQLServerID
        {
            get { return _sqlServerID; }
            set { _sqlServerID = value; }
        }

        public Int32 AnalysisID
        {
            get { return _analysisID; }
            set { _analysisID = value; }
        }

        public DateTime AnalysisStartTime
        {
            get { return _analysisStartTime; }
            set { _analysisStartTime = value; }
        }

        public DateTime AnalysisCompleteTime
        {
            get { return _analysisCompleteTime; }
            set { _analysisCompleteTime = value; }
        }

        public Int32 TotalRecommendationCount
        {
            get { return _totalRecommendationCount; }
            set { _totalRecommendationCount = value; }
        }

        public string AnalysisDuration
        {
            get { return _analysisDuration; }
            set { _analysisDuration = value; }
        }

        public float ComputedRankFactor
        {
            get { return _computedRankFactor; }
            set { _computedRankFactor = value; }
        }
    }

    [Serializable]
    public class AnalysisListCollection
    {
        private List<AnalysisList> _analysisListCollection;

        public List<AnalysisList> AnalysisListColl
        {
            get { return _analysisListCollection; }
            set { _analysisListCollection = value; }
        }
    }
}
