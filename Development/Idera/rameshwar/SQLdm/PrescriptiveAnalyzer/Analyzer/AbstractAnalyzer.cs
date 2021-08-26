using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using BBS.TracerX;
//using Idera.SQLdoctor.Common.Recommendations;
//using Idera.SQLdoctor.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Resources;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    public abstract class AbstractAnalyzer : IAnalyze
    {
        private string _className = null;
        private List<IRecommendation> _recommendations = new List<IRecommendation>();
        private RecommendationCountHelper _counter = new RecommendationCountHelper(Properties.Settings.Default.Max_RecommendationsPerType);
        protected List<Exception> _exceptions = new List<Exception>();
        protected SnapshotMetrics _sm;
        protected SnapshotCommonRecommendations _common;
        protected Int32 _id;

        public AbstractAnalyzer() { }

        protected void CheckCancel()
        {
            CheckCancel(_sm);
        }

        protected static void CheckCancel(SnapshotMetrics sm)
        {
            //if (sm.IsCancelled)
            //    throw new OperationCanceledException();
        }

        //public void Analyze(SnapshotCommonRecommendations common, SnapshotMetrics sm, SqlConnection conn)
        //{
        //    if (null == sm) return;

        //    _sm = sm;
        //    Analyze(sm, conn);
        //}

        public string ClassName
        {
            get
            {
                if (null != _className) return (_className);
                string name = ToString();
                try { name = this.GetType().Name; }
                catch { }
                return (_className = name);
            }
        }

        public Int32 ID { get { return _id; } }

        public virtual string GetDescription() { return (ClassName); }
        public virtual void Analyze(SnapshotMetrics sm, SqlConnection conn)
        {
            //CheckCancel();
        }
        public virtual void Clear()
        {
            _recommendations = new List<IRecommendation>();
            _exceptions = new List<Exception>();
        }
        public virtual IEnumerable<IRecommendation> GetRecommendations()
        {
            return _recommendations;
        }
        public virtual IEnumerable<Exception> GetExceptions()
        {
            return _exceptions;
        }

        public int GetRecommendationCount() { return ((null == _recommendations) ? 0 : _recommendations.Count); }
        
        protected void AddRecommendation(IRecommendation r)
        {
            if (null == r) return;
            //Check if Recomm exists in master recomm
            if (!MasterRecommendations.ContainsRecommendation(r.ID))
                return;
            if (!_counter.Allow(r.RecommendationType))
            {
                Logger l = GetLogger();
                if (null != l) { l.DebugFormat("Recommendation limit exceeded :{0} - {1}", r.ID, r.FindingText); }
                return;
            }
            //----------------------------------------------------------------------------
            // If we already have too many recommendations, ignore the new recommendations
            // being added.  This is added as a fail safe limiting mechanism and is not intended
            // to select the best recommendations.
             
            if (Settings.Default.Max_RecommendationsPerAnalyzer > 0)
            {
                if (_recommendations.Count >= Settings.Default.Max_RecommendationsPerAnalyzer)
                {
                    Logger l = GetLogger();
                    if (null != l)
                    {
                        using (l.InfoCall(string.Format("{0}.AddRecommendation() limit of {1} encountered", ClassName, Settings.Default.Max_RecommendationsPerAnalyzer)))
                        {
                            l.InfoFormat("Recommendation being thrown away due to limit of {0} recommendations", Settings.Default.Max_RecommendationsPerAnalyzer);
                            l.InfoFormat("Recommendation:{0} - {1}", r.ID, r.FindingText);
                        }
                    }
                    return;
                }
            }
            _counter.Add(r);
            _recommendations.Add(r);
        }

        protected void AddException(string msg) { AddException(msg, null); }
        protected void AddException(string msg, Exception e)
        {
            try
            {
                throw new Exception(msg, e);
            }
            catch (Exception ex)
            {
                AddException(ex);
            }
        }

        protected void AddException(Exception ex)
        {
            _exceptions.Add(ex);
        }

        protected abstract Logger GetLogger();
        protected bool IsValid(BaseMetrics i)
        {
            if (null == i) return (false);
            if (i.IsDataValid) return (true);
            //Logger l = GetLogger();
            //if (null != l) { l.DebugFormat("{0} is not valid.  HasException:{1}  ExecutionCount:{2}", i.ClassName, i.HasException, i.ExecutionCount); }
            return (false);
        }
    }
}
