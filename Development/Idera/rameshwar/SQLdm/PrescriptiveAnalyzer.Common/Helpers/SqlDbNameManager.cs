using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Globalization;
using System.IO;
using Microsoft.Win32;
//using Idera.SQLdoctor.Common;
//using Idera.SQLdoctor.Common.Configuration;
//using Idera.SQLdoctor.Common.Recommendations;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers
{
    public class SqlDbNameManager
    {
        private Dictionary<UInt32, string> _dbnames = new Dictionary<UInt32, string>();
        private object _lockNames = new object();

        public string GetDatabaseName(SqlConnectionInfo info, UInt32 id)
        {
            string db = null;
            if (0 != id)
            {
                try
                {
                    lock (_lockNames)
                    {
                        if (!_dbnames.TryGetValue(id, out db))
                        {
                            db = SQLHelper.GetDatabaseName(info, id);
                            _dbnames.Add(id, db);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(string.Format("SqlDbNameManager.GetDatabaseName {0} Exception: ", id), ex);
                }
            }
            return db;
        }

        public string GetDatabaseName(SqlConnection conn, UInt32 id)
        {
            string db = null;
            if (0 != id)
            {
                try
                {
                    lock (_lockNames)
                    {
                        if (!_dbnames.TryGetValue(id, out db))
                        {
                            db = SQLHelper.GetDatabaseName(conn, id);
                            _dbnames.Add(id, db);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(string.Format("SqlDbNameManager.GetDatabaseName {0} Exception: ", id), ex);
                }
            }
            return db;
        }

        public bool UpdateDatabaseName(SqlConnection conn, UInt32 id)
        {
            string db = GetDatabaseName(conn, id);
            if (!String.IsNullOrEmpty(db) && 0 != string.Compare(conn.Database, db))
            {
                conn.ChangeDatabase(db);
                return true;
            }
            return false;
        }
    }
}

