using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan
{
    class ExecutionPlanTables : TraverseObjectProps
    {
        private List<string> _tables = new List<string>();
        private List<string> _indexes = new List<string>();

        public IEnumerable<string> Tables { get {return(_tables);} }

        public ExecutionPlanTables(SqlSystemObjectManager ssom) : base(ssom) { }

        public IEnumerable<string> GetTables(ShowPlanXML plan, UInt32 dbid)
        {
            _tables.Clear();
            Traverse(plan, dbid);
            return (_tables);
        }
        public IEnumerable<string> GetIndexes(ShowPlanXML plan, UInt32 dbid)
        {
            _indexes.Clear();
            Traverse(plan, dbid);
            return (_indexes);
        }
        protected override bool Process(object o) 
        {
            if (null == o) return (false);
            if (o is ColumnReferenceType) Process((ColumnReferenceType)o);
            if (o is ObjectType) Process((ObjectType)o);
            return (true); 
        }
        private void AddTable(string database, string schema, string table)
        {
            string item = string.Format("{0}.{1}.{2}", database, schema, table);
            if (!_tables.Contains(item)) _tables.Add(item);
        }
        private void AddIndex(string database, string schema, string table, string index)
        {
            string item = string.Format("{0}.{1}.{2}.{3}", database, schema, table, index);
            if (!_indexes.Contains(item)) _indexes.Add(item);
        }
        private void Process(ObjectType objectType)
        {
            if (null == objectType) return;
            if (string.IsNullOrEmpty(objectType.Database)) return;
            if (string.IsNullOrEmpty(objectType.Schema)) return;
            if (string.IsNullOrEmpty(objectType.Table)) return;
            AddTable(objectType.Database, objectType.Schema, objectType.Table);
            if (string.IsNullOrEmpty(objectType.Index)) return;
            AddIndex(objectType.Database, objectType.Schema, objectType.Table, objectType.Index);
        }
        private void Process(ColumnReferenceType columnReferenceType)
        {
            if (null == columnReferenceType) return;
            if (string.IsNullOrEmpty(columnReferenceType.Database)) return;
            if (string.IsNullOrEmpty(columnReferenceType.Schema)) return;
            if (string.IsNullOrEmpty(columnReferenceType.Table)) return;
            AddTable(columnReferenceType.Database, columnReferenceType.Schema, columnReferenceType.Table);
        }
    }
}
