using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Stats;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Resources;

namespace Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan.Analyzers
{
    internal class OperatorsAnalyzer : ExecutionPlanOperators, IAnalyzePlan
    {
        private const Int32 id = 204;
        List<IRecommendation> _recommendations = new List<IRecommendation>();
        private RecommendationCountHelper _counter = new RecommendationCountHelper(Properties.Settings.Default.Max_RecommendationsPerType);
        List<Exception> _exceptions = new List<Exception>();
        private string _batch;
        private TEBase _te;
        private TraceEventStats _tes;
        private TraceEventStatsCollection _tesc;

        public OperatorsAnalyzer(SqlSystemObjectManager ssom) : base(ssom) { }

        public Int32 ID { get { return id; } }

        public void Analyze(TraceEventStatsCollection tesc)
        {
            if (null == tesc) return;
            _tesc = tesc;
            foreach (TraceEventStats tes in tesc)
            {
                if (null == tes) continue;
                _batch = string.Empty;
                _te = null;
                _tes = tes;
                if (null != tes.High) { _te = tes.High; _batch = tes.High.TextData; }
                if (null != tes.Plan) Traverse(tes.Plan, tes.High.DBID);
            }
        }

        protected override void Process(IntrinsicType it)
        {
            if (null == it) return;
            if (!IsTableColumn(it.ScalarOperator)) return;
            if (!IsScan()) return;
            ProcessIntrinsicFunction(it);
        }

        protected override void Process(UDFType udf)
        {
            if (null == udf) return;
            if (!IsTableColumn(udf.ScalarOperator)) return;
            if (!IsScan()) return;
            ProcessUserDefinedFunction(udf);
        }

        protected override void Process(ConvertType ct)
        {
            if (null == ct) return;
            if (!ct.Implicit) return;
            if (!IsTableColumn(ct.ScalarOperator)) return;
            if (!IsScan()) return;
            ProcessImplicitConversion(ct);
        }

        private void ProcessUserDefinedFunction(UDFType udf)
        {
            ColumnReferenceType c = GetColumnInfo(udf.ScalarOperator);
            if (null == c) return;
            BaseStmtInfoType bsit = GetParent<BaseStmtInfoType>();
            RelOpType rot = GetParent<RelOpType>();
            AddRecommendation(new UserDefinedFunctionInPredicateRecommendation(
                                SQLHelper.RemoveBrackets(c.Database),
                                SQLHelper.RemoveBrackets(c.Schema),
                                SQLHelper.RemoveBrackets(c.Table),
                                SQLHelper.RemoveBrackets(c.Column),
                                (null == bsit) ? string.Empty : bsit.StatementText,
                                _batch,
                                (null == rot) ? string.Empty : rot.PhysicalOp.ToString(),
                                (null == _te) ? string.Empty : _te.ObjectName,
                                (null == _te) ? string.Empty : _te.ApplicationName,
                                (null == _te) ? string.Empty : _te.HostName,
                                (null == _te) ? string.Empty : _te.LoginName,
                                (null == _tes) ? 0 : _tes.TotalDuration,
                                _tesc.TraceDurationMinutes,
                                udf.FunctionName
                                ));
        }

        private void AddRecommendation(IRecommendation r)
        {
            if (null == r) return;
            //Check if Recomm exists in master recomm
            if (!MasterRecommendations.ContainsRecommendation(r.ID))
                return;
            if (!_counter.Allow(r.RecommendationType)) { return; }
            _counter.Add(r);
            _recommendations.Add(r);
        }

        private void ProcessIntrinsicFunction(IntrinsicType it)
        {
            ColumnReferenceType c = GetColumnInfo(it.ScalarOperator);
            if (null == c) return;
            BaseStmtInfoType bsit = GetParent<BaseStmtInfoType>();
            RelOpType rot = GetParent<RelOpType>();
            AddRecommendation(new IntrinsicFunctionInPredicateRecommendation(
                                SQLHelper.RemoveBrackets(c.Database),
                                SQLHelper.RemoveBrackets(c.Schema),
                                SQLHelper.RemoveBrackets(c.Table),
                                SQLHelper.RemoveBrackets(c.Column),
                                (null == bsit) ? string.Empty : bsit.StatementText,
                                _batch,
                                (null == rot) ? string.Empty : rot.PhysicalOp.ToString(),
                                (null == _te) ? string.Empty : _te.ObjectName,
                                (null == _te) ? string.Empty : _te.ApplicationName,
                                (null == _te) ? string.Empty : _te.HostName,
                                (null == _te) ? string.Empty : _te.LoginName,
                                (null == _tes) ? 0 : _tes.TotalDuration,
                                _tesc.TraceDurationMinutes,
                                it.FunctionName
                                ));
        }

        private void ProcessImplicitConversion(ConvertType ct)
        {
            ColumnReferenceType c = GetColumnInfo(ct.ScalarOperator);
            if (null == c) return;
            BaseStmtInfoType bsit = GetParent<BaseStmtInfoType>();
            RelOpType rot = GetParent<RelOpType>();
            AddRecommendation(new ImplicitConversionInPredicateRecommendation(
                                SQLHelper.RemoveBrackets(c.Database),
                                SQLHelper.RemoveBrackets(c.Schema),
                                SQLHelper.RemoveBrackets(c.Table),
                                SQLHelper.RemoveBrackets(c.Column),
                                (null == bsit) ? string.Empty : bsit.StatementText,
                                _batch,
                                (null == rot) ? string.Empty : rot.PhysicalOp.ToString(),
                                (null == _te) ? string.Empty : _te.ObjectName,
                                (null == _te) ? string.Empty : _te.ApplicationName,
                                (null == _te) ? string.Empty : _te.HostName,
                                (null == _te) ? string.Empty : _te.LoginName,
                                (null == _tes) ? 0 : _tes.TotalDuration,
                                _tesc.TraceDurationMinutes,
                                ct.DataType
                                ));
        }

        public void Clear()
        {
            _recommendations.Clear();
            _exceptions.Clear();
        }

        public IEnumerable<IRecommendation> GetRecommendations()
        {
            return (_recommendations);
        }
        public IEnumerable<Exception> GetExceptions()
        {
            return (_exceptions);
        }
    }
}
