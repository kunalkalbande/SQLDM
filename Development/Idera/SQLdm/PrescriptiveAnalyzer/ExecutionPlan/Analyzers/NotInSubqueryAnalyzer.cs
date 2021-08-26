using System;
using System.Collections.Generic;
//using System.Linq;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Stats;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;
using Wintellect.PowerCollections;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan.Analyzers
{
    public class NotInSubqueryAnalyzer : TraverseObjectProps, IAnalyzePlan
    {
        private const Int32 id = 203;
        private static TracerX.Logger _logX = TracerX.Logger.GetLogger("NotInSubqueryAnalyzer");

        private List<IRecommendation> _recommendations = new List<IRecommendation>();
        protected List<Exception> _exceptions = new List<Exception>();

        private Set<ColumnReferenceType> iddict = new Set<ColumnReferenceType>();
        private IdentType identType;

        public NotInSubqueryAnalyzer(SqlSystemObjectManager ssom) : base(ssom) 
        {
        }

        public Int32 ID { get { return id; } }

        public void Analyze(TraceEventStatsCollection tesc)
        {
            if (null == tesc) return;
            foreach (TraceEventStats tes in tesc)
            {
                if (Properties.Settings.Default.Max_RecommendationsPerType > 0)
                {
                    if (_recommendations.Count >= Properties.Settings.Default.Max_RecommendationsPerType) return;
                }

                try
                {
                    if (null == tes) continue;
                    Analyze(tes.Plan, tes.High.DBID);
                    if (iddict.Count == 0) continue;

                    //ColumnReferenceType crt = iddict.First();
                    ColumnReferenceType crt = iddict.ToArray()[0];

                    TSqlRecommendationWithColumn recommendation = 
                        new TSqlRecommendationWithColumn(RecommendationType.NotInUsedOnNullableColumn, 
                                                         crt.Database, 
                                                         tes.High.ApplicationName,
                                                         tes.High.LoginName,
                                                         tes.High.HostName);
                    OffendingSql sql = new OffendingSql();
                    sql.Script = tes.High.TextData;
                    recommendation.Sql = sql;

                    recommendation.TableName = crt.Schema + "." + crt.Table;
                    recommendation.ColumnName = crt.Column;
                    foreach (var columnReference in iddict)
                    {
                        DatabaseObjectName name = new DatabaseObjectName();
                        name.DatabaseName = SQLHelper.RemoveBrackets(columnReference.Database);
                        name.SchemaName = SQLHelper.RemoveBrackets(columnReference.Schema);
                        name.ObjectName = SQLHelper.RemoveBrackets(columnReference.Table);
                        recommendation.AddSourceObject(name);
                    } 
                    _recommendations.Add(recommendation);
                }
                catch (Exception e)
                {
                    if (e is System.Threading.ThreadAbortException)
                        throw e;

                    _logX.Error(e);
                }
            } 
        }

        public void Analyze(ShowPlanXML plan, UInt32 dbid)
        {
            if (null == plan) return;
            Clear();
            Traverse(plan, dbid);
        }

        public void Clear()
        {
            identType = null;
            iddict.Clear();
        }

        public IEnumerable<IRecommendation> GetRecommendations()
        {
            return _recommendations;
        }

        public IEnumerable<Exception> GetExceptions()
        {
            return null;
        }

        private IEnumerable<object> GetFilteredJoins(ShowPlanXML showPlanXML)
        {
            return null;
        }

        protected override bool Process(object o)
        {
            if (null == o) return (false);
            
            if (o is RelOpType) Process((RelOpType)o);
            else if (o is NestedLoopsType) Process((NestedLoopsType)o);
            else if (o is TableScanType) Process((TableScanType)o);
            else if (o is IndexScanType) Process((IndexScanType)o);

            return (true);

        }

        protected void Process(RelOpType rop)
        {
            switch (rop.PhysicalOp)
            {
                case PhysicalOpType.NestedLoops:
                    identType = null;
                    break;
                case PhysicalOpType.IndexScan:
                case PhysicalOpType.ClusteredIndexScan:
                case PhysicalOpType.TableScan:
                    break;
                default:
                    identType = null;
                    break;
            }
        }

        protected void Process(NestedLoopsType o)
        {
            if (o.Predicate != null && o.Predicate.ScalarOperator != null)
            {
                CompareType comparison = o.Predicate.ScalarOperator.Item as CompareType;
                if (comparison != null)
                {
                    switch(comparison.CompareOp)
                    {
                        case CompareOpType.IS:
                            if (comparison.ScalarOperator != null && comparison.ScalarOperator.Length == 2)
                            {
                                if (comparison.ScalarOperator[0].Item is IdentType && comparison.ScalarOperator[1].Item is ConstType)
                                {
                                    Process((IdentType)comparison.ScalarOperator[0].Item, (ConstType)comparison.ScalarOperator[1].Item);
                                }
                            }
                            break;
                    }
                }
            }
        }

        protected void Process(IdentType itype, ConstType ctype)
        {
            if (itype.ColumnReference != null && ctype.ConstValue.Equals("NULL"))
                identType = itype;
            else
                identType = null;
        }

        protected void Process(IndexScanType o)
        {
            if (identType != null)
            {
                if (o.Predicate == null && o.Object != null && o.Object.Length == 1)
                {
                    ColumnReferenceType crt = identType.ColumnReference;
                    //ObjectType ot = o.Object.First();
                    ObjectType ot = o.Object[0];
                    if (crt.Database.Equals(ot.Database) &&
                        crt.Schema.Equals(ot.Schema) &&
                        crt.Table.Equals(ot.Table))
                        CaptureColumnInfo(crt);
                }
            }
        }

        protected void Process(TableScanType o)
        {
            if (identType != null)
            {
                if (o.Predicate == null && o.Object != null && o.Object.Length == 1)
                {
                    ColumnReferenceType crt = identType.ColumnReference;
                    //ObjectType ot = o.Object.First();
                    ObjectType ot = o.Object[0];
                    if (crt.Database.Equals(ot.Database) &&
                        crt.Schema.Equals(ot.Schema) &&
                        crt.Table.Equals(ot.Table))
                        CaptureColumnInfo(crt);

                }
            }
        }

        private void CaptureColumnInfo(ColumnReferenceType o)
        {
            iddict.Add(o);
        }
    }
}
