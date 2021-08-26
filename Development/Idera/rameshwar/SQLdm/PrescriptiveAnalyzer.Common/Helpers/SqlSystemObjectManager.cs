using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Globalization;
using System.IO;
using Microsoft.Win32;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers
{
    public class SqlSystemObjectManager
    {
        private class SqlObjectIdName
        {
            public string Name { get; private set; }
            public int ID { get; private set; }
            private SqlObjectIdName() { }
            public SqlObjectIdName(int id, string name)
            {
                Name = string.IsNullOrEmpty(name) ? string.Empty : name;
                ID = id;
            }
            public override bool Equals(object obj)
            {
                SqlObjectIdName o = obj as SqlObjectIdName;
                if (null == o) return (false);
                if (ID != o.ID) { return (false);}
                if (0 == ID)
                {
                    return (0 == string.Compare(o.Name, Name));
                }
                return (true);
            }
            public override int GetHashCode()
            {
                if ((0 == ID) && !string.IsNullOrEmpty(Name))
                {
                    return (Name.GetHashCode());
                }
                return (ID.GetHashCode());
            }
            public override string ToString()
            {
                if ((0 == ID) && !string.IsNullOrEmpty(Name))
                {
                    return (Name.ToString());
                }
                return ID.ToString();
            }
        }

        private SqlConnectionInfo _info = null;
        private Dictionary<UInt32, Dictionary<SqlObjectIdName, bool>> _dbs = new Dictionary<UInt32, Dictionary<SqlObjectIdName, bool>>();
        private object _lock = new object();

        private Dictionary<string, UInt32> _dbNameToID = new Dictionary<string, UInt32>();
        private object _lockNameToID = new object();

        private SqlSystemObjectManager() { }
        public SqlSystemObjectManager(SqlConnectionInfo info) 
        {
            _info = info;
        }

        public bool IsSystemObject(IRecommendation r)
        {
            try
            {

                if (null == r) return (false);
                IProvideTableName ipt = r as IProvideTableName;
                if (null == ipt)
                {
                    return (false);
                }
                //-----------------------------------------------------------------
                //  If the object is from the sys schema, ignore it.
                //
                if (0 == string.Compare(ipt.Schema, "sys")) return (true);

                return (IsSystemObject(ipt.Database, SQLHelper.Bracket(ipt.Schema, ipt.Table)));
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("IsSystemObject(IRecommendation r) Exception: ", ex);
            }
            return (false);
        }
        public bool IsSystemObject(string db, string objname)
        {
            return (IsSystemObject(GetDBID(db), 0, objname));
        }

        private UInt32 GetDBID(string db)
        {
            UInt32 result = 0;
            lock (_lockNameToID)
            {
                if (!_dbNameToID.TryGetValue(db, out result)) { _dbNameToID.Add(db, result = Convert.ToUInt32(SQLHelper.GetDatabaseId(_info, db))); }
            }
            return (result);
        }
        public bool IsSystemObject(UInt32 dbid, int id, string name)
        {
            if (SQLHelper.IsSystemDB(dbid)) return (true);
            bool result = false;
            SqlObjectIdName idName = new SqlObjectIdName(id, name);
            lock (_lock)
            {
                Dictionary<SqlObjectIdName, bool> dbobjs = GetDatabaseObjects(dbid);
                if (dbobjs.TryGetValue(idName, out result)) { return (result); }
                result = GetIsSystemObject(dbid, id, name);
                dbobjs.Add(idName, result);
            }
            return (result);
        }

        private bool GetIsSystemObject(UInt32 dbid, int id, string name)
        {
            if (SQLHelper.IsSystemDB(dbid)) return (true);
            try
            {
                return (SQLHelper.IsSystemObject(_info, dbid, id, name));
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(string.Format("GetIsSystemObject({0}, {1}) Exception: ", dbid, id), ex);
                return (false);
            }
        }

        private Dictionary<SqlObjectIdName, bool> GetDatabaseObjects(UInt32 dbid)
        {
            Dictionary<SqlObjectIdName, bool> dbobjs = null;
            if (!_dbs.TryGetValue(dbid, out dbobjs))
            {
                dbobjs = new Dictionary<SqlObjectIdName, bool>();
                _dbs.Add(dbid, dbobjs);
            }
            return (dbobjs);
        }
    }
}

