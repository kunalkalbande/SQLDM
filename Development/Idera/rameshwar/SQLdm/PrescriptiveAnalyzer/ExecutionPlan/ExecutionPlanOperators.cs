using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan
{
    class ExecutionPlanOperators : TraverseObjectProps
    {
        public ExecutionPlanOperators(SqlSystemObjectManager ssom) : base(ssom) { }
        protected override bool Process(object o)
        {
            if (null == o) return (false);
            if (o is IntrinsicType) Process((IntrinsicType)o);
            if (o is UDFType) Process((UDFType)o);
            if (o is ConvertType) Process((ConvertType)o);
            return (true);
        }

        protected bool IsScan()
        {
            RelOpType rot = GetParent<RelOpType>();
            if (null == rot) return (false);
            if ((PhysicalOpType.ClusteredIndexScan == rot.PhysicalOp) ||
                (PhysicalOpType.IndexScan == rot.PhysicalOp) ||
                (PhysicalOpType.TableScan == rot.PhysicalOp))
            {
                return (true);
            }
            return (false);
        }

        protected virtual void Process(IntrinsicType it) { }
        protected virtual void Process(UDFType udf) { }
        protected virtual void Process(ConvertType ct) { }

        protected ColumnReferenceType GetColumnInfo(ScalarType[] ast)
        {
            if (null == ast) return (null);
            ColumnReferenceType c = null;
            foreach (ScalarType st in ast) if (null != (c = GetColumnInfo(st))) return (c);
            return (null);
        }

        protected ColumnReferenceType GetColumnInfo(ScalarType st)
        {
            if (st.Item is IdentType)
            {
                IdentType it = st.Item as IdentType;
                if (null != it)
                {
                    if (null == it.ColumnReference) return (null);
                    if (string.IsNullOrEmpty(it.ColumnReference.Database)) return (null);
                    if (string.IsNullOrEmpty(it.ColumnReference.Column)) return (null);
                    return (it.ColumnReference);
                }
            }
            return (null);
        }

        protected bool IsTableColumn(ScalarType[] ast)
        {
            if (null == ast) return (false);
            foreach (ScalarType st in ast) if (IsTableColumn(st)) return (true);
            return (false);
        }

        protected bool IsTableColumn(ScalarType st)
        {
            if (st.Item is IdentType)
            {
                return (IsTableColumn(st.Item as IdentType));
            }
            return (false);
        }

        protected bool IsTableColumn(IdentType it)
        {
            if (null == it) return (false);
            return (IsTableColumn(it.ColumnReference));
        }

        protected bool IsTableColumn(ColumnReferenceType c)
        {
            if (null == c) return (false);
            return (!string.IsNullOrEmpty(c.Database) && !string.IsNullOrEmpty(c.Column));
        }
    }
}
