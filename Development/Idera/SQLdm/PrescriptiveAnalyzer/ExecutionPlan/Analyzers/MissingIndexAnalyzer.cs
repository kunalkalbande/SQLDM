using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.State;
using Idera.SQLdm.PrescriptiveAnalyzer.MissingIndexes;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Stats;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Collectors;

namespace Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan.Analyzers
{
    internal class MissingIndexAnalyzer : AbstractPlanAnalyzer
    {
        private const Int32 id = 202;
        private static TracerX.Logger _logX = TracerX.Logger.GetLogger("MissingIndexAnalyzer");

        private List<MissingIndexGroupType> _missingIndexes = null;
        private List<PlanWithMissingIndexes> _plansWithMissingIndexes = null;
        private DMVMissingIndexes _dmvIndexes = null;
        private int _traceDurationMintues;

        public HypotheticalIndexCleanupThread IndexCleanupThread { get; private set; }

        public MissingIndexAnalyzer(BaseOptions ops, SqlSystemObjectManager ssom, int traceDurationMintues) : base(ops, ssom) 
        {
            _id = id;
            _traceDurationMintues = traceDurationMintues; 
        }

        public override void Analyze(TraceEventStatsCollection tesc)
        {
            if (_ops.IsProductionServer) return;
            _missingIndexes = null;
            _plansWithMissingIndexes = null;
            base.Analyze(tesc);
            //------------------------------------------------------------------------
            // for the adhoc batch analysis, do not analyze the dmv missing indexes.
            //
            if (!_ops.IsAdHocBatchAnalysis) { _dmvIndexes = new DMVMissingIndexes(_ops); }
            if (!_ops.IsRecommendationTypeBlocked(RecommendationType.MissingIndex))
            {
                if (null != _plansWithMissingIndexes) Analyze(_plansWithMissingIndexes);
            }
            else
            {
                _logX.Info("Skipped missing index analysis due to recommendation type being blocked.");
            }
            if (!_ops.IsRecommendationTypeBlocked(RecommendationType.DMVMissingIndex) && (null != _dmvIndexes))
            {
                _dmvIndexes.Analyze(_ops.ConnectionInfo);
                IEnumerable<IRecommendation> recs = _dmvIndexes.GetRecommendations();
                if (null != recs) AddRecommendations(recs);
            }
            else
            {
                _logX.Info("Skipped DMV missing index recommendations due to recommendation type being blocked.");
            }
        }

        public override void Analyze(ShowPlanXML plan, UInt32 dbid)
        {
            _missingIndexes = null;
            base.Analyze(plan, dbid);
            if (null != _missingIndexes)
            {
                if (null == _plansWithMissingIndexes) _plansWithMissingIndexes = new List<PlanWithMissingIndexes>();
                _plansWithMissingIndexes.Add(new PlanWithMissingIndexes(_stats, _missingIndexes));
                _missingIndexes = null;
            }
        }

        public override bool Analyze(QueryPlanType plan)
        {
            if (null == plan) { return (base.Analyze(plan));}
            if (null == plan.MissingIndexes) { return (base.Analyze(plan)); }
            if (plan.MissingIndexes.Length > 0)
            {
                if (!_stats.IsWorstTSQL())
                {
                    if (null == _missingIndexes) _missingIndexes = new List<MissingIndexGroupType>(plan.MissingIndexes.Length);
                    _missingIndexes.AddRange(plan.MissingIndexes);
                }
            }
            return (base.Analyze(plan));
        }

        public override void Clear()
        {
            base.Clear();
            _missingIndexes = null;
            _plansWithMissingIndexes = null;
        }

        private void Analyze(List<PlanWithMissingIndexes> missingIndexes)
        {
            if (null == missingIndexes) return;
            if (missingIndexes.Count <= 0) return;
            _ops.UpdateState(AnalysisStateType.MissingIndexAnalysis, "Starting missing index analysis...", 0, 100);
            Dictionary<string, MissingIndexesOnTable> tablesMissingIndexes = new Dictionary<string, MissingIndexesOnTable>(missingIndexes.Count);
            foreach (PlanWithMissingIndexes pwmi in missingIndexes)
            {
                if (null == pwmi.MissingIndexes) continue;
                foreach (MissingIndexGroupType migt in pwmi.MissingIndexes)
                {
                    if (null == migt.MissingIndex) continue;
                    foreach (MissingIndexType mit in migt.MissingIndex)
                    {
                        if (_ops.IsDatabaseBlocked(SQLHelper.RemoveBrackets(mit.Database))) { _logX.DebugFormat("Skipping missing index check on tables of database {0}", SQLHelper.RemoveBrackets(mit.Database)); continue; }
                        MissingIndexesOnTable mii;
                        string table = string.Format("{0}.{1}.{2}", mit.Database, mit.Schema, mit.Table);
                        if (!tablesMissingIndexes.TryGetValue(table, out mii)) tablesMissingIndexes.Add(table, mii = new MissingIndexesOnTable(_statsCollection.SSOM, _statsCollection.GetAllEventStatsForTable(table, true), SQLHelper.RemoveBrackets(mit.Database), SQLHelper.RemoveBrackets(mit.Schema), SQLHelper.RemoveBrackets(mit.Table), _traceDurationMintues));
                        mii.AddMissingIndex(pwmi.Stats, mit.ColumnGroup);
                    }
                }
            }
            CheckCancel();
            using (SqlConnection conn = SQLHelper.GetConnection(_ops.ConnectionInfo))
            {
                var tp = new SQLTablePropHelper(_ops.ConnectionInfo);
                var cleanup = new HypotheticalIndexCleanupThread(_ops.ConnectionInfo);
                List<IRecommendation> recs = new List<IRecommendation>();
                _ops.UpdateState(AnalysisStateType.MissingIndexAnalysis, string.Format("Analyzing activity for {0} tables.", tablesMissingIndexes.Count), 0, 100);
                int count = 0;
                foreach (MissingIndexesOnTable miot in tablesMissingIndexes.Values)
                {
                    if (null != _dmvIndexes) { _dmvIndexes.RemoveMatches(miot); }
                    _ops.UpdateState(AnalysisStateType.MissingIndexAnalysis, count++, tablesMissingIndexes.Count);
                    if (!tp.IsSystemTable(miot.Database, miot.Schema, miot.Table)) 
                    {
                        _ops.UpdateState(AnalysisStateType.MissingIndexAnalysis, string.Format("Analyzing missing indexes on {0}.{1}.{2}.", miot.Database, miot.Schema, miot.Table));
                        miot.Analyze(conn, cleanup, recs); 
                    }
                }
                if (recs.Count > 0) AddRecommendations(recs);
                if (cleanup.IsAlive)
                {
                    _logX.Debug("Cleanup thread is still alive.  Saving thread information.");
                    IndexCleanupThread = cleanup;
                }
            }
            _ops.UpdateState(AnalysisStateType.MissingIndexAnalysis, "Completed missing index analysis.", 100, 100);
        }

    }
}
