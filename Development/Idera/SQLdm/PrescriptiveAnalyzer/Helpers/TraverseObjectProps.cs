using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Reflection;

using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Helpers
{
    public abstract class TraverseObjectProps
    {
        private readonly int _maxDepth = 500;
        private SqlSystemObjectManager _ssom;
        protected Stack<object> _ancestors = new Stack<object>();
        private TraverseObjectProps() { }
        public TraverseObjectProps(SqlSystemObjectManager ssom) : this() { _ssom = ssom; }
        public TraverseObjectProps(int maxDepth, SqlSystemObjectManager ssom) : this(ssom) { _maxDepth = maxDepth; }
        protected virtual bool Process(object o) { return (true); }
        protected void Traverse(object o, UInt32 dbid)
        {
            try
            {
                _ancestors.Push(o);
                if (null == o) return;
                if (o is StmtSimpleType) { if (IsSystemStoredProc(o as StmtSimpleType, dbid)) return; }

                if (!Process(o)) return;
                if (_ancestors.Count > _maxDepth) return;
                if (o is IList)
                {
                    var list = o as IList;
                    if (null == list) return;
                    foreach (object li in list) Traverse(li, dbid);
                    return;
                }
                Type oType = o.GetType();
                if (null == oType) return;
                PropertyInfo[] props = oType.GetProperties();
                if (null == props) return;
                foreach (PropertyInfo propInfo in props)
                {
                    if (null == propInfo) continue;
                    if (propInfo.CanRead)
                    {
                        try
                        {
                            if (!propInfo.PropertyType.IsPrimitive) Traverse(propInfo.GetValue(o, null), dbid);
                        }
                        catch (Exception ex)
                        {
                            SQLdm.PrescriptiveAnalyzer.Common.ExceptionLogger.Log("TraverseObjectProps.Traverse() Exception: ", ex);
                            if (ex is System.Threading.ThreadAbortException)
                                throw;
                        }
                    }
                }
            }
            finally { _ancestors.Pop(); }
        }

        private bool IsSystemStoredProc(StmtSimpleType sst, UInt32 dbid)
        {
            if (null == sst) return (false);
            if (null == sst.StoredProc) return (false);
            if (string.IsNullOrEmpty(sst.StoredProc.ProcName)) return (false);
            if (_ssom.IsSystemObject(dbid, 0, sst.StoredProc.ProcName)) return (true);
            return (false);
        }
        public T GetParent<T>()
        {
            foreach (object o in _ancestors) { if (o is T) return((T)o); }
            return (default(T));
        }
    }
}
