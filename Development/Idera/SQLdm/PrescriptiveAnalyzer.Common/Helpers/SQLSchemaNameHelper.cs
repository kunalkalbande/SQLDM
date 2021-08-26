using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers
{
    public class SQLSchemaNameHelper
    {
        private SqlConnectionInfo _info = null;
        private SqlConnection _conn = null;
        private Dictionary<string, SQLSchemaNameToDB> _dbs = new Dictionary<string, SQLSchemaNameToDB>();
        private object _lock = new object();

        private SQLSchemaNameHelper() { }
        public SQLSchemaNameHelper(SqlConnectionInfo info) { _info = info; }
        public SQLSchemaNameHelper(SqlConnection conn) { _conn = conn; }

        private SQLSchemaNameToDB GetSchemaNameToDB(UInt32 id, string db)
        {
            SQLSchemaNameToDB ssntb = null;
            if (!_dbs.TryGetValue(db, out ssntb))
            {
                ssntb = new SQLSchemaNameToDB(db);
                _dbs.Add(db, ssntb);
            }
            return (ssntb);
        }

        private SQLSchemaNameToDB.SQLSchemaName GetSQLSchemaName(UInt32 id, string db)
        {
            if (string.IsNullOrEmpty(db)) return (null);
            lock (_lock)
            {
                SQLSchemaNameToDB ssntb = GetSchemaNameToDB(id, db);
                if (null == ssntb) return (null);
                if (null != _info) return (ssntb.GetSchemaName(_info, id));
                if (null != _conn) return (ssntb.GetSchemaName(_conn, id));
                Debug.Assert(false, "No connection!");
                return (null);
            }
        }

        public string GetObjectName(UInt32 id, string db)
        {
            var n = GetSQLSchemaName(id, db);
            if (null == n) return (string.Empty);
            return (n.ObjectName);
        }

        public string GetSchemaName(UInt32 id, string db)
        {
            var n = GetSQLSchemaName(id, db);
            if (null == n) return (string.Empty);
            return (n.SchemaName);
        }

        private class SQLSchemaNameToDB
        {
            private Dictionary<UInt32, SQLSchemaName> _idToNames = new Dictionary<UInt32, SQLSchemaName>();
            private string _db;

            private SQLSchemaNameToDB() { }
            public SQLSchemaNameToDB(string db) { _db = db; }

            internal SQLSchemaName GetSchemaName(SqlConnectionInfo info, UInt32 id)
            {
                SQLSchemaName n;
                if (!_idToNames.TryGetValue(id, out n))
                {
                    n = new SQLSchemaName(info, id, _db);
                    _idToNames.Add(id, n);
                }
                return (n);
            }
            internal SQLSchemaName GetSchemaName(SqlConnection conn, UInt32 id)
            {
                SQLSchemaName n;
                if (!_idToNames.TryGetValue(id, out n))
                {
                    n = new SQLSchemaName(conn, id, _db);
                    _idToNames.Add(id, n);
                }
                return (n);
            }
            internal class SQLSchemaName
            {
                public string ObjectName { get; private set; }
                public string SchemaName { get; private set; }

                private SQLSchemaName() { }
                public SQLSchemaName(SqlConnection cnn, UInt32 id, string db)
                {
                    Load(cnn, id, db);
                }

                private void Load(SqlConnection conn, UInt32 id, string db)
                {
                    try
                    {
                        string sql = string.Format(Properties.Resources.GetObjectSchemaName, SQLHelper.Bracket(db), id);
                        using (SqlCommand command = new SqlCommand(sql, conn))
                        {
                            command.CommandType = CommandType.Text;
                            command.CommandTimeout = Constants.DefaultCommandTimeout;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    ObjectName = DataHelper.ToString(reader, "ObjectName");
                                    SchemaName = DataHelper.ToString(reader, "SchemaName");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogger.Log(string.Format("SQLSchemaName.Load(id={0}, db={1}) Exception: ", id, db), ex);
                    }
                }
                public SQLSchemaName(SqlConnectionInfo info, UInt32 id, string db) 
                {
                    try
                    {
                        using (SqlConnection conn = SQLHelper.GetConnection(info))
                        {
                            Load(conn, id, db);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogger.Log(string.Format("SQLSchemaName(id={0}, db={1}) Exception: ", id, db), ex);
                    }
                }
            }
        }
    }
}
