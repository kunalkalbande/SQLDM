using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers
{
    public class SQLTablePropHelper
    {
        public class SQLTableProp
        {
            public readonly bool IsSystemTable;
            private SQLTableProp() { }
            public SQLTableProp(SqlConnectionInfo info, string db, string schema, string table) 
            {
                try
                {
                    IsSystemTable = SQLHelper.IsSystemTable(info, db, schema, table);
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(string.Format("SQLTableProp({0}, {1}, {2}) Exception:", db, schema, table), ex);
                }
            }
        }

        private SqlConnectionInfo _info = null;
        private Dictionary<string, int> _mapDbId = new Dictionary<string, int>();
        private Dictionary<int, Dictionary<string, SQLTableProp>> _mapDbToTableProps = new Dictionary<int, Dictionary<string, SQLTableProp>>();

        private SQLTablePropHelper() { }
        public SQLTablePropHelper(SqlConnectionInfo info)
        {
            _info = info;
        }

        public bool IsSystemTable(string db, string schema, string table)
        {
            int dbid = GetDatabaseId(db);
            if (dbid <= 4) return (true);
            SQLTableProp props = GetTableProps(db, schema, table);
            if (null == props) return (false);
            return (props.IsSystemTable);
        }

        private SQLTableProp GetTableProps(string db, string schema, string table)
        {
            Dictionary<string, SQLTableProp> tableProps = null;
            int dbid = GetDatabaseId(db);
            if (_mapDbToTableProps.TryGetValue(dbid, out tableProps))
            {
                if (null != tableProps)
                {
                    return (GetTableProps(db, schema, table, tableProps));
                }
            }
            _mapDbToTableProps[dbid] = tableProps = new Dictionary<string, SQLTableProp>();
            return (GetTableProps(db, schema, table, tableProps));
        }

        private SQLTableProp GetTableProps(string db, string schema, string table, Dictionary<string, SQLTableProp> tableProps)
        {
            if (null == tableProps) return (null);
            SQLTableProp props = null;
            string key = string.Format("{0}.{1}", schema, table);
            if (tableProps.TryGetValue(key, out props))
            {
                if (null != props) return (props);
            }
            tableProps[key] = props = new SQLTableProp(_info, db, schema, table);
            return (props);
        }

        private int GetDatabaseId(string db)
        {
            int id = 0;
            if (string.IsNullOrEmpty(db)) { System.Diagnostics.Debug.Assert(false, "No Database name given!"); return (id); }
            try
            {
                if (_mapDbId.TryGetValue(db, out id)) return (id);
                id = SQLHelper.GetDatabaseId(_info, db);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(string.Format("SQLTablePropHelper.GetDatabaseId({0}) Exception:", db), ex);
            }
            _mapDbId.Add(db, id);
            return (id);
        }
    }
}
