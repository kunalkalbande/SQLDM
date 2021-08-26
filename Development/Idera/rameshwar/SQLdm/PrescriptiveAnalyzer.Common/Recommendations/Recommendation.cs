using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Resources;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
//using Idera.SQLdm.PrescriptiveAnalyzer.Common.Ranking;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Objects;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Ranking;
using System.Data;
using System.Data.SqlClient;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    
    [Serializable]
    public class Recommendation : IRecommendation , IMessageGenerator, IUndoMessageGenerator
    {
        protected const int LOW_CONFIDENCE = 1;
        protected const int HIGH_CONFIDENCE = 10;
        protected const int LOW_IMPACT = 1;
        protected const int HIGH_IMPACT = 3;

        private RecommendationType _type;
        protected string _id;
        private int _rankID;
        private float _computedRankFactor;
        private double? _relevance;
        private bool _isFlagged;
        private int _uniqueAnalysisRecommendationIDFromDB;
        private RecommendationOptimizationStatus _optimizationStatus = RecommendationOptimizationStatus.NotOptimized;
        private string _optimizationErrorMessage;

        [NonSerialized]
        private string _loggerName;
        [NonSerialized]
        private Logger _logX;
        private string _categoryTextResolved;
        [NonSerialized]
        private IList<string> _tagsTextResolved;

        private string _descriptionTextResolved;
        private string _findingTextResolved;
        private string _impactTextResolved;
        private string _problemTextResolved;
        private string _recommendationTextResolved;
        [NonSerialized]
        private string _additionalConsiderationsResolved;
        private int? _confidenceFactorResolved;
        [NonSerialized]
        private int? _impactFactorResolved;
        [NonSerialized]
        private RecommendationLinks _recommendationLinksResolved;

        public Recommendation()
        {

        }

        public Recommendation(RecommendationType type)
        {
            this._type = type;
            _id = FindingIdAttribute.GetFindingId(type);
        }

        public Recommendation(RecommendationType type, RecommendationProperties recProp)
        {
            this._type = type;
            _id = FindingIdAttribute.GetFindingId(type);
        }

        public Recommendation(RecommendationType type, String id)
        {
            this._type = type;
            this.ID = id;
        }

        public Recommendation(RecommendationType type, String id, RecommendationProperties recProp)
        {
            this._type = type;
            this.ID = id;
        }

        protected string LoggerName 
        { 
            get 
            { 
                if (string.IsNullOrEmpty(_loggerName)) { _loggerName = GetType().Name; }
                return (_loggerName);
            } 
        }
        protected Logger LOG 
        { 
            get 
            {
                if (null == _logX) _logX = BBS.TracerX.Logger.GetLogger(LoggerName);
                return (_logX);
            } 
        }

        #region IRecommendation Members

        public virtual RecommendationType RecommendationType
        {
            get { return _type; }
            set 
            { 
                _type = value;
                if (string.IsNullOrEmpty(_id))
                    _id = FindingIdAttribute.GetFindingId(_type);
            }
        }

        public String ID
        {
            get 
            {
                if (String.IsNullOrEmpty(_id))
                    _id = FindingIdAttribute.GetFindingId(_type);
                return _id; 
            }

            set { _id = value; }
        }

        public int RankID { get { return (_rankID); } private set { _rankID = value; } }
        public float ComputedRankFactor { get { return (_computedRankFactor); } set { _computedRankFactor = value; } }
        public double Relevance 
        { 
            get 
            {
                if (null != _relevance)
                {
                    if (_relevance.HasValue) return (_relevance.Value);
                }
                return (0.0); 
            } 
        }

        //10.0 SQLdm srishti purohit -- property to be populated from DB to get flag status
        public bool IsFlagged { get { return (_isFlagged); } set { _isFlagged = value;  } }
        
        //10.0 SQLdm srishti purohit -- property to be populated from DB to saved unique recommendation ID
        public int AnalysisRecommendationID { get { return (_uniqueAnalysisRecommendationIDFromDB); } set { _uniqueAnalysisRecommendationIDFromDB = value; } }

        //10.0 SQLdm srishti purohit -- property to be populated from DB to save status of optimization performed
        public virtual RecommendationOptimizationStatus OptimizationStatus
        {
            get { return _optimizationStatus; }
            set
            {
                _optimizationStatus = value;
            }
        }
        public virtual string OptimizationErrorMessage
        {
            get { return _optimizationErrorMessage; }
            set
            {
                _optimizationErrorMessage = value;
            }
        }
        public void ComputeRankFactor(string filterDatabase, string filterApplication, RankingStats rankingStats)
        {
            if (0 != _computedRankFactor) return;
            //if (null == config) { _computedRankFactor = (ConfidenceFactor * ImpactFactor); return; }
            try
            {
                _computedRankFactor = Convert.ToSingle(ConfidenceFactor * (ImpactFactor + GetRelevance(filterDatabase, filterApplication, rankingStats)));
                //_computedRankFactor = Convert.ToSingle(ConfidenceFactor * (ImpactFactor + Relevance));
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(string.Format("ComputeRankFactor  ConfidenceFactor:{0}  ImpactFactor:{1}  GetRelevance:{2}", ConfidenceFactor, ImpactFactor, GetRelevance(filterDatabase, filterApplication, rankingStats)), ex);
                //ExceptionLogger.Log(string.Format("ComputeRankFactor  ConfidenceFactor:{0}  ImpactFactor:{1}  GetRelevance:{2}", ConfidenceFactor, ImpactFactor, Relevance), ex);
            }
        }

        private double GetRelevance(string filterDatabase, string filterApplication, RankingStats rankingStats)
        {
            try
            {
                if (null != _relevance)
                {
                    if (_relevance.HasValue) return (_relevance.Value);
                }
                _relevance = MasterRecommendations.GetRelevance(ID);
                if (!string.IsNullOrEmpty(filterDatabase))
                {
                    if (0 == string.Compare(filterDatabase, GetDatabaseName(), true)) { ++_relevance; }
                }
                if (!string.IsNullOrEmpty(filterApplication))
                {
                    if (!string.IsNullOrEmpty(GetApplicationName())) { ++_relevance; }
                }
                _relevance = GetRelevance(_relevance.Value, rankingStats);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("GetRelevance Exception: ", ex);
                if (null == _relevance) { _relevance = 0; }
                if (!_relevance.HasValue) { _relevance = 0; }
            }
            return (_relevance.Value);
        }

        protected virtual double GetRelevance(double baseRelevance, RankingStats rankingStats)
        {
            var r = this as IProvideDatabase;
            if (null == r) return (baseRelevance);
            double p = rankingStats.GetPercentile(r);
            if (p > 0.0)
            {
                if (p > 1.0) LOG.DebugFormat("RankStats Percentile: {0} for {1}", p, this.ToString());
                return (p + baseRelevance);
            }
            return (baseRelevance);
        }

        private string GetApplicationName()
        {
            var i = this as IProvideApplicationName;
            if (null == i) return (string.Empty);
            return (string.IsNullOrEmpty(i.ApplicationName) ? string.Empty : i.ApplicationName);
        }

        private string GetDatabaseName()
        {
            var i = this as IProvideDatabase;
            if (null == i) return (string.Empty);
            return (string.IsNullOrEmpty(i.Database) ? string.Empty : i.Database);
        }
        
        public int ConfidenceFactor
        {
            get 
            {
                if (!_confidenceFactorResolved.HasValue)
                    _confidenceFactorResolved = MasterRecommendations.GetConfidenceFactor(ID);
                return WithinBounds(AdjustConfidenceFactor(_confidenceFactorResolved.Value), LOW_CONFIDENCE, HIGH_CONFIDENCE);
            }
        }
        /// <summary>
        /// Allow derived objects to adjust the confidence factor
        /// </summary>
        /// <param name="i">base confidence</param>
        /// <returns>adjusted confidence</returns>
        public virtual int AdjustConfidenceFactor(int i) { return (i); }

        public int ImpactFactor
        {
            get 
            {
                if (!_impactFactorResolved.HasValue)
                    _impactFactorResolved = MasterRecommendations.GetImpactFactor(ID);
                return WithinBounds(AdjustImpactFactor(_impactFactorResolved.Value), LOW_IMPACT, HIGH_IMPACT);
            }
        }

        /// <summary>
        /// Allow derived objects to adjust the impact factor.
        /// </summary>
        /// <param name="i">base impact factor</param>
        /// <returns>adjusted impact factor</returns>
        public virtual int AdjustImpactFactor(int i) { return (i); }

        private int WithinBounds(int i, int low, int high)
        {
            if (i < low) return (low);
            if (i > high) return (high);
            return (i);
        }

        public virtual IEnumerable<String> AffectedBatches
        {
            get { return null; }
        }

        public virtual IEnumerable<DatabaseObjectName> SourceObjects 
        {
            get 
            {
                var ipt = this as IProvideTableName;
                if (null != ipt)
                {
                    var don = new DatabaseObjectName();
                    var l = new List<DatabaseObjectName>();
                    l.Add(don);
                    don.DatabaseName = ipt.Database;
                    don.SchemaName = ipt.Schema;
                    don.ObjectName = ipt.Table;
                    return (l);
                }
                return null; 
            } 
        }

        public string Category
        {
            get 
            {
                if (_categoryTextResolved == null)
                    _categoryTextResolved = MasterRecommendations.GetCategory(ID);
                return _categoryTextResolved;
            }
        }

        public IList<string> Tags
        {
            get 
            { 
                if (_tagsTextResolved == null)
                    _tagsTextResolved = MasterRecommendations.GetTags(ID);
                return _tagsTextResolved;
            }
        }

        public RecommendationLinks Links
        {
            get 
            { 
                if (_recommendationLinksResolved == null)
                    _recommendationLinksResolved = MasterRecommendations.GetInfoLinks(ID);
                return _recommendationLinksResolved;
            }
        }

        public String FindingText
        {
            get 
            {
                if (_findingTextResolved == null)
                {
                    string template = MasterRecommendations.GetFinding(ID);
                    if (String.IsNullOrEmpty(template))
                    {
                        LOG.ErrorFormat("Unable to find finding text for {0}", LoggerName);
                        _findingTextResolved = String.Empty;
                    }
                    else
                    {
                        try
                        {
                            _findingTextResolved = FormatHelper.Format(template, this);
                        }
                        catch (Exception e)
                        {
                            _findingTextResolved = template;
                            LOG.ErrorFormat("Error formatting finding text for {0}: {1}", _type, e);
                        }
                    }
                }

                return _findingTextResolved;
            }
        }

        public String DescriptionText
        {
            get
            {
                if (_descriptionTextResolved == null)
                {
                    string template = MasterRecommendations.GetDescription(ID);
                    if (String.IsNullOrEmpty(template))
                    {
                        LOG.ErrorFormat("Unable to find finding text for {0}", LoggerName);
                        _descriptionTextResolved = String.Empty;
                    }
                    else
                    {
                        try
                        {
                            _descriptionTextResolved = FormatHelper.Format(template, this);
                        }
                        catch (Exception e)
                        {
                            _descriptionTextResolved = template;
                            LOG.ErrorFormat("Error formatting finding text for {0}: {1}", _type, e);
                        }
                    }
                }

                return _descriptionTextResolved;
            }
        }


        public String ImpactExplanationText
        {
            get 
            {
                if (_impactTextResolved == null)
                {

                    string template = MasterRecommendations.GetImpactExplanation(ID);
                    if (String.IsNullOrEmpty(template))
                    {
                        LOG.ErrorFormat("Unable to find impact explanation text for {0}", LoggerName);
                        _impactTextResolved = String.Empty;
                    }
                    else
                    {
                        try
                        {
                            _impactTextResolved = FormatHelper.Format(template, this);
                        }
                        catch (Exception e)
                        {
                            _impactTextResolved = template;
                            LOG.ErrorFormat("Error formatting impact explanation text for {0}: {1}", _type, e);
                        }
                    }
                }
                return _impactTextResolved;
            }
        }

        public String ProblemExplanationText
        {
            get
            {
                if (_problemTextResolved == null)
                {

                    string template = MasterRecommendations.GetProblemExplanation(ID);
                    if (String.IsNullOrEmpty(template))
                    {
                        LOG.ErrorFormat("Unable to find problem explanation text for {0}", LoggerName);
                        _problemTextResolved = String.Empty;
                    }
                    else
                    {
                        try
                        {
                            _problemTextResolved = FormatHelper.Format(template, this, true);
                        }
                        catch (Exception e)
                        {
                            _problemTextResolved = template;
                            LOG.ErrorFormat("Error formatting problem explanation text for {0}: {1}", _type, e);
                        }
                    }
                }
                return _problemTextResolved;
            }
        }

        public string AdditionalConsiderations
        {
            get
            {
                if (_additionalConsiderationsResolved == null)
                {
                    string template = MasterRecommendations.GetAdditionalConsiderations(ID);
                    if (String.IsNullOrEmpty(template))
                    {
                        LOG.ErrorFormat("Unable to find additional considerations for {0}", LoggerName);
                        _additionalConsiderationsResolved = String.Empty;
                    }
                    else
                    {
                        try
                        {
                            _additionalConsiderationsResolved = FormatHelper.Format(template, this, true);
                        }
                        catch (Exception e)
                        {
                            _additionalConsiderationsResolved = template;
                            LOG.ErrorFormat("Error formatting additional considerations for {0}: {1}", _type, e);
                        }
                    }
                }
                return (_additionalConsiderationsResolved);
            }
        }
        public string RecommendationText
        {
            get
            {
                if (_recommendationTextResolved == null)
                {
                    string template = MasterRecommendations.GetRecommendation(ID);
                    if (String.IsNullOrEmpty(template))
                    {
                        LOG.ErrorFormat("Unable to find recommendation text for {0}", LoggerName);
                        _recommendationTextResolved = String.Empty;
                    }
                    else
                    {
                        try
                        {
                            _recommendationTextResolved = FormatHelper.Format(template, this);
                        }
                        catch (Exception e)
                        {
                            _recommendationTextResolved = template;
                            LOG.ErrorFormat("Error formatting recommendation text for {0}: {1}", _type, e);
                        }
                    }
                }
                return _recommendationTextResolved;
            }

        }

        public virtual bool IsScriptGeneratorProvider
        {
            get { return this is IScriptGeneratorProvider; }
        }

        public virtual bool IsUndoScriptGeneratorProvider
        {
            get { return this is IUndoScriptGeneratorProvider; }
        }

        #endregion

        public static void RankRecommendations(List<IRecommendation> recs, string filterDatabase, string filterApplication, SqlConnectionInfo info)
        {
            if (null != recs)
            {
                //#if DEBUG
                //                Dictionary<string, bool> ids = new Dictionary<string, bool>() 
                //                { 
                //                    {"SDR-I5", false},
                //                    {"SDR-I9", false},
                //                    {"SDR-I10", false},
                //                    {"SDR-I12", false},
                //                    {"SDR-I14", false},
                //                    {"SDR-I17", false},
                //                    {"SDR-I18", false},
                //                    {"SDR-I19", false},
                //                    {"SDR-I16", false},
                //                    {"SDR-Q7", false}
                //                };
                //                recs.RemoveAll(delegate(IRecommendation ir) 
                //                { 
                //                    bool f;
                //                    if (ids.TryGetValue(ir.ID, out f))
                //                    {
                //                        ids[ir.ID] = true;
                //                        return (f);
                //                    }
                //                    return (true);                    
                //                });
                //#endif
                int n = 0;
                var c = new RecommendationRankingComparer(filterDatabase, filterApplication, info);
                c.ComputeRankFactor(recs);
                recs.Sort(c);
                recs.ForEach(delegate(IRecommendation ir)
                {
                    var r = ir as Recommendation;
                    if (null != r) r._rankID = ++n;
                });
            }
        }

        #region IMessageGenerator Members

        public List<string> GetUndoMessages(RecommendationOptimizationStatus res, SqlConnection connection)
        {
            List<string> messages = new List<string>();

            if (null != res)
            {
                if ((res != RecommendationOptimizationStatus.OptimizationCompleted) && (res != RecommendationOptimizationStatus.OptimizationUndone))
                {
                    messages.Add("Verify that the original optimization has been performed before executing this Undo Optimization script.");
                }
                else
                {
                    if (res == RecommendationOptimizationStatus.OptimizationUndone)
                    {
                        messages.Add("This undo script has been executed previously. Running this undo script again could harm your SQL Server environment.");
                    }
                }
            }
            return messages;
        }

        public List<string> GetMessages(RecommendationOptimizationStatus res, SqlConnection connection)
        {
            List<string> messages = new List<string>();

            if (null != res)
            {
                if (res == RecommendationOptimizationStatus.OptimizationCompleted)
                {
                    messages.Add("This optimization script has been executed previously. Running this script again could harm your SQL Server environment.");
                }
            }
            return messages;
        }
        #endregion


        virtual public void SetProperties(Dictionary<string, string> lstProperties)
        {
            
        }

        virtual public Dictionary<string, string> GetProperties()
        {
            return new Dictionary<string, string>();
        }
    }
}
