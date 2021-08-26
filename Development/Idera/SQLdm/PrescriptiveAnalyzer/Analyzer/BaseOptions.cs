using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.State;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    public class BaseOptions
    {
        public SqlConnectionInfo ConnectionInfo { get; set; }
        private List<string> _blockedDatabases;
        private List<RecommendationType> _blockedRecommendationTypes;
        private SqlDbNameManager _nameManager = new SqlDbNameManager();
        private AnalysisState _state = null;

        private BaseOptions() { }
        internal BaseOptions(AnalysisState state) { _state = state; }
        public string FilterApplicationName { get; set; }
        public string FilterDatabaseName { get; set; }
        public string FilterCategory { get; set; }
        private List<string> _filterCategories;
        public List<string> FilterCategories
        {
            get
            {
                if (null == _filterCategories) return (null);
                return (new List<string>(_filterCategories));
            }
            set
            {
                if (null == value) { _filterCategories = null; return; }
                _filterCategories = new List<string>(value);
            }
        }
        public bool GenerateTestRecommendations { get; set; }
        public bool IsAdHocBatchAnalysis { get; set; }

        public IEnumerable<string> BlockedDatabases
        {
            get
            {
                if (null == _blockedDatabases) return (null);
                return (new List<string>(_blockedDatabases));
            }
            set
            {
                if (null == value) { _blockedDatabases = null; return; }
                _blockedDatabases = new List<string>(value);
                _blockedDatabases.Sort();
            }
        }

        public IEnumerable<RecommendationType> BlockedRecommendationTypes
        {
            get
            {
                if (null == _blockedRecommendationTypes) return (null);
                return (new List<RecommendationType>(_blockedRecommendationTypes));
            }
            set
            {
                if (null == value) { _blockedRecommendationTypes = null; return; }
                _blockedRecommendationTypes = new List<RecommendationType>(value);
                _blockedRecommendationTypes.Sort();
            }
        }

        public string GetDatabaseName(uint dbid)
        {
            return _nameManager.GetDatabaseName(ConnectionInfo, dbid);
        }
        public bool UpdateDatabaseName(SqlConnection conn, UInt32 dbid)
        {
            return _nameManager.UpdateDatabaseName(conn, dbid);
        }

        public bool IsDatabaseBlocked(UInt32 dbid)
        {
            return (IsDatabaseBlocked(_nameManager.GetDatabaseName(ConnectionInfo, dbid)));
        }

        public bool IsDatabaseBlocked(string db)
        {
            if (string.IsNullOrEmpty(db)) return (false);
            if (string.IsNullOrEmpty(FilterDatabaseName))
            {
                if (!AreDatabasesBlocked()) return (false);
                if (null == _blockedDatabases) return (false);
                if (_blockedDatabases.Count <= 0) return (false);
                return (_blockedDatabases.BinarySearch(db) >= 0);
            }
            return (0 != FilterDatabaseName.CompareTo(db));
        }

        public bool AreDatabasesBlocked()
        {
            if (!string.IsNullOrEmpty(FilterDatabaseName)) return (true);
            if (null == _blockedDatabases) return (false);
            if (_blockedDatabases.Count <= 0) return (false);
            return (true);
        }

        public bool IsRecommendationTypeBlocked(RecommendationType rt)
        {
            if (null == _blockedRecommendationTypes) return (false);
            if (_blockedRecommendationTypes.Count <= 0) return (false);
            return (_blockedRecommendationTypes.BinarySearch(rt) >= 0);
        }
        public BaseOptions BaseClone()
        {
            BaseOptions ops = new BaseOptions();
            ops.ConnectionInfo = this.ConnectionInfo.Clone();
            ops.BlockedDatabases = this.BlockedDatabases;
            ops.BlockedRecommendationTypes = this.BlockedRecommendationTypes;
            ops.FilterApplicationName = this.FilterApplicationName;
            ops.FilterDatabaseName = this.FilterDatabaseName;
            ops.FilterCategory = this.FilterCategory;
            ops.FilterCategories = this.FilterCategories;
            ops.GenerateTestRecommendations = this.GenerateTestRecommendations;
            ops.IsAdHocBatchAnalysis = this.IsAdHocBatchAnalysis;
            ops._state = this._state;
            return (ops);
        }
        internal void CancelState(AnalysisStateType ast, string status)
        {
            if (null != _state) _state.Cancel(ast, status);
        }

        internal void UpdateState(AnalysisStateType ast, string status, int current, int max)
        {
            if (null != _state) _state.Update(ast, status, current, max);
        }

        internal void UpdateState(AnalysisStateType ast, int current, int max)
        {
            if (null != _state) _state.Update(ast, current, max);
        }

        internal void UpdateState(AnalysisStateType ast, string status)
        {
            if (null != _state) _state.Update(ast, status);
        }

        internal bool IsBlocked(IRecommendation r)
        {
            if (null == r) return (false);
            if (IsRecommendationTypeBlocked(r.RecommendationType)) return (true);
            if (null == FilterCategories)
            {
                if (!string.IsNullOrEmpty(FilterCategory))
                {
                    if (0 != string.Compare(r.Category, FilterCategory, true)) return (true);
                }
            }
            else
            {
                bool notInFilter = true;
                foreach (string category in FilterCategories)
                    notInFilter = notInFilter && (0 != string.Compare(r.Category, category, true));
                if (notInFilter) return true;
            }
            if (!AreDatabasesBlocked()) return (false);
            var i = r as IProvideDatabase;
            if (null == i) return (false);
            if (!string.IsNullOrEmpty(i.Database))
            {
                return (IsDatabaseBlocked(i.Database));
            }
            return (false);
        }
    }
}
