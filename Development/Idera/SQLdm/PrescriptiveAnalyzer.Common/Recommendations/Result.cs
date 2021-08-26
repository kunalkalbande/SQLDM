using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Values;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    //This ENUM is maaped with DB table PrescriptiveAnalysisType
    //Any modification on it might impact analysis
    //So please check the mapping before updating
    //10.0 SQLdm Srishti Purohit
    public enum AnalysisType
    {
        Default = 1,
        WorkLoad = 2,
        AdHocBatch = 3,
        ScheduledDefault = 4,
        ScheduledWorkLoad = 5,
        ScheduledDiagnose = 6,
        AlertDefault = 7,
        AlertWorkLoad = 8,
        AlertDiagnose = 9
    }

    [Serializable]
    public class Result
    {
        private Int32 _sqlServerID;
        private DateTime _analysisStartTime;
        private DateTime _analysisCompleteTime;
        private Int32 _totalRecommendationCount;
        private List<AnalyzerResult> _analyzerRecommendationList;
        private AnalysisType _analysisType = AnalysisType.Default;
        private Exception _error;

        //new variable to support SDR-M16
        private SnapshotValues latestSnapshot;
        
        public Result()
        {
            _analyzerRecommendationList = new List<AnalyzerResult>();
        }

        public AnalysisType Type
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

        public List<AnalyzerResult> AnalyzerRecommendationList
        {
            get { return _analyzerRecommendationList; }
            set { _analyzerRecommendationList = value; }
        }

        public SnapshotValues LatestSnapshot
        {
            get { return latestSnapshot; }
            set { latestSnapshot = value; }
        }
    }

    [Serializable]
    public class AnalyzerResult
    {
        private Int32 _analyzerID;
        private Int32 _status;
        private List<IRecommendation> _recommendationList;

        public AnalyzerResult()
        {
            _recommendationList = new List<IRecommendation>();
        }

        public Int32 AnalyzerID
        {
            get { return _analyzerID; }
            set { _analyzerID = value; }
        }
        //10.0 Srishti Purohit Temporary change to support save script
        public Int32 Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public Int32 RecommendationCount
        {
            get { return _recommendationList.Count; }
        }

        public List<IRecommendation> RecommendationList
        {
            get { return _recommendationList; }
            set { _recommendationList = value; }
        }
    }

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
        {   }

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