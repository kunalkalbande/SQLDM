using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Stats;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Resources;

namespace Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan.Analyzers
{
    internal abstract class AbstractPlanAnalyzer : ExecutionPlanProcessor, IAnalyzePlan
    {
        private static TracerX.Logger _logX = TracerX.Logger.GetLogger("AbstractPlanAnalyzer");

        private List<IRecommendation> _recommendations = new List<IRecommendation>();
        private RecommendationCountHelper _counter = new RecommendationCountHelper(Properties.Settings.Default.Max_RecommendationsPerType);
        protected List<Exception> _exceptions = new List<Exception>();
        protected TraceEventStatsCollection _statsCollection;
        protected TraceEventStats _stats;
        protected BaseOptions _ops;
        protected Int32 _id;

        private string _className = null;

        public AbstractPlanAnalyzer(BaseOptions ops, SqlSystemObjectManager ssom) : base(ssom)
        {
            _ops = ops.BaseClone();
        }

        public Int32 ID { get { return _id; } }

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

        public virtual void Analyze(TraceEventStatsCollection tesc)
        {
            _statsCollection = tesc;
            _stats = null;
            if (null == tesc) return;
            foreach (TraceEventStats tes in tesc)
            {
                _stats = tes;
                if (null != tes) if (null != tes.Plan) Analyze(tes.Plan, tes.High.DBID);
            }

        }
        public virtual void Analyze(ShowPlanXML plan, UInt32 dbid)
        {
            Process(plan, dbid);
        }
        public override void Process(StmtBlockType[] blocks) { if (Analyze(blocks)) base.Process(blocks); }
        public override void Process(StmtBlockType block) { if (Analyze(block)) base.Process(block); }
        public override void Process(BaseStmtInfoType stmt) { if (Analyze(stmt)) base.Process(stmt); }
        public override void Process(StmtSimpleType stmt) { if (Analyze(stmt)) base.Process(stmt); }
        public override void Process(QueryPlanType plan) { if (Analyze(plan)) base.Process(plan); }

        public virtual bool Analyze(StmtBlockType[] blocks) { return (true); }
        public virtual bool Analyze(StmtBlockType block) { return (true); }
        public virtual bool Analyze(BaseStmtInfoType stmt) { return (true); }
        public virtual bool Analyze(StmtSimpleType stmt) { return (true); }
        public virtual bool Analyze(QueryPlanType plan) { return (true); }

        public virtual void Clear()
        {
            _recommendations = new List<IRecommendation>();
            _exceptions = new List<Exception>();
        }

        public virtual IEnumerable<IRecommendation> GetRecommendations()
        {
            return _recommendations;
        }

        protected void AddRecommendations(IEnumerable<IRecommendation> recs)
        {
            if (null == recs) return;
            foreach (var r in recs) AddRecommendation(r);
        }
        protected void AddRecommendation(IRecommendation r)
        {
            if (null == r) return;
            //Check if Recomm exists in master recomm
            if (!MasterRecommendations.ContainsRecommendation(r.ID))
                return;
            if (!_counter.Allow(r.RecommendationType))
            {
                _logX.DebugFormat("Recommendation limit exceeded :{0} - {1}", r.ID, r.FindingText); 
                return;
            }
            if (null != _ops)
            {
                if (_ops.IsBlocked(r))
                {
                    _logX.InfoFormat("Blocked Recommendation:{0} - {1}", r.ID, r.FindingText);
                    return;
                }
            }
            //----------------------------------------------------------------------------
            // If we already have too many recommendations, ignore the new recommendations
            // being added.  This is added as a fail safe limiting mechanism and is not intended
            // to select the best recommendations.
            // 
            if (Settings.Default.Max_RecommendationsPerAnalyzer > 0)
            {
                if (_recommendations.Count >= Settings.Default.Max_RecommendationsPerAnalyzer)
                {
                    using (_logX.InfoCall(string.Format("{0}.AddRecommendation() limit of {1} encountered", ClassName, Settings.Default.Max_RecommendationsPerAnalyzer)))
                    {
                        _logX.InfoFormat("Recommendation being thrown away due to limit of {0} recommendations", Settings.Default.Max_RecommendationsPerAnalyzer);
                        _logX.InfoFormat("Recommendation:{0} - {1}", r.ID, r.FindingText);
                    }
                    return;
                }
            }
            _counter.Add(r);
            _recommendations.Add(r);
        }

        protected void CheckCancel()
        {
            if (System.Threading.Thread.CurrentThread.ThreadState == System.Threading.ThreadState.AbortRequested)
                throw new OperationCanceledException();
        }

        public virtual IEnumerable<Exception> GetExceptions()
        {
            return _exceptions;
        }
    }
}
