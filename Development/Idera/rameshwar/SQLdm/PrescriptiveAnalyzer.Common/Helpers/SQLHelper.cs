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
    public enum SSVer_e
    {
        SS_Unknown = 0,
        SS_7 = 7,
        SS_2000,
        SS_2005,
        SS_2008
    };

    /// <summary>
    /// Summary description for SQLHelper.
    /// </summary>
    public class SQLHelper
    {
        public static readonly int RESOURCEDB_ID = 32767;

        private static Logger _logX = Logger.GetLogger("SQLHelper");

        public static void SetOleAutomationConfig(SqlConnection conn, bool enabled)
        {
            string sql = string.Format(Properties.Resources.SetOleAutomationConfig, (enabled ? "1" : "0"));
            using (SqlCommand command = new SqlCommand(sql, conn))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = Constants.DefaultCommandTimeout;
                command.ExecuteNonQuery();
            }
        }
        public static bool IsSystemDB(UInt32 dbid)
        {
            if (RESOURCEDB_ID == dbid) return (true);
            if (dbid <= 4) return (true);
            return (false);
        }

        public static bool IsSysAdmin(SqlConnection conn)
        {
            string sql = "select IS_SRVROLEMEMBER ('sysadmin')";
            using (SqlCommand command = new SqlCommand(sql, conn))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = Constants.DefaultCommandTimeout;
                return (Convert.ToBoolean(command.ExecuteScalar()));
            }
        }

        public static string GetComputerNamePhysicalNetBIOS(SqlConnection conn)
        {
            string sql = "select SERVERPROPERTY('ComputerNamePhysicalNetBIOS')";
            using (SqlCommand command = new SqlCommand(sql, conn))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = Constants.DefaultCommandTimeout;
                return (Convert.ToString(command.ExecuteScalar()));
            }
        }

        public static string GetMachineName(SqlConnection conn)
        {
            string sql = "select SERVERPROPERTY('MachineName')";
            using (SqlCommand command = new SqlCommand(sql, conn))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = Constants.DefaultCommandTimeout;
                return (Convert.ToString(command.ExecuteScalar()));
            }
        }

        public static bool IsOleEnabled(SqlConnection conn)
        {
            using (_logX.DebugCall(string.Format("IsOleEnabled({0})", conn)))
            {
                try
                {
                    string sql = string.Format(Properties.Resources.IsOleEnabled);
                    _logX.DebugFormat("IsOleEnabled - TSQL:{0}", sql);
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = Constants.DefaultCommandTimeout;
                        return (Convert.ToBoolean(command.ExecuteScalar()));
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "IsOleEnabled() Exception:", ex);
                    throw;
                }
            }
        }

        public static bool IsCmdShellEnabled(SqlConnection conn)
        {
            return false;//fixing DE46288. Preventing use of cmd shell
            string sql = string.Format(Properties.Resources.IsCmdShellEnabled);
            using (SqlCommand command = new SqlCommand(sql, conn))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = Constants.DefaultCommandTimeout;
                return (Convert.ToBoolean(command.ExecuteScalar()));
            }
        }

        /// <summary>
        /// Log instance information for the given sql server.
        /// </summary>
        /// <param name="server">sql server to log information for</param>
        public static void LogInstanceVersion(SqlConnectionInfo info)
        {
            if (null == info) return;
            using (_logX.InfoCall(string.Format("SQLHelper.LogInstanceVersion({0})", info)))
            {
                try
                {
                    using (SqlConnection conn = GetConnection(info))
                    {
                        LogInstanceVersion(conn);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "Connection Exception: ", ex);
                }
            }
        }

        /// <summary>
        /// Log instance information for the given sql server.
        /// </summary>
        /// <param name="conn">connection to sql server</param>
        public static void LogInstanceVersion(SqlConnection conn)
        {
            using (_logX.InfoCall(string.Format("SQLHelper.LogInstanceVersion({0}, {1}, {2})", conn.DataSource, conn.Database, conn.ServerVersion)))
            {
                string sql = string.Format("EXEC master..xp_msver");
                try
                {
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = Constants.DefaultCommandTimeout;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                _logX.Info(string.Format("{0}: {1}", reader[1].ToString(), reader[3].ToString()));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, string.Format("{0} Exception: ", sql), ex);
                }
                CheckConnection(conn);
                _logX.Info("Environment.MachineName:" + Environment.MachineName);
                _logX.Info("GetComputerNamePhysicalNetBIOS:" + GetComputerNamePhysicalNetBIOS(conn));
                _logX.Info("GetMachineName:" + GetMachineName(conn));
                string ver = GetSqlVersionString(conn);
                if (!string.IsNullOrEmpty(ver))
                    _logX.Info("SQLServer Version:\n", ver.Replace("\r\n", "\n").Replace("\t", "  "));
            }
        }

        public static object GetScalarResult(string sql, SqlConnection conn)
        {
            object o = null;
            using (_logX.DebugCall(string.Format("SQLHelper.GetScalarResult({0})", sql)))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = Constants.DefaultCommandTimeout;
                        o = command.ExecuteScalar();
                        _logX.DebugFormat("GetScalarResult:{0}", (null == o ? "null" : o.ToString()));
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "GetScalarResult Exception: ", ex);
                }
            }
            return (o);
        }

        public static object RegRead(SqlConnection conn, string root, string key, string valueName)
        {
            string sql = string.Format("exec master..xp_regread {0},{1},{2}", CreateSafeString(root), CreateSafeString(key), CreateSafeString(valueName));
            object o = null;
            using (_logX.DebugCall(string.Format("SQLHelper.RegRead({0})", sql)))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = Constants.DefaultCommandTimeout;
                        using (SqlDataReader r = command.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                if (r.FieldCount >= 2)
                                {
                                    o = r[1];
                                    break;
                                }
                            }
                        }
                        _logX.DebugFormat("RegReadResult:{0}", (null == o ? "null" : o.ToString()));
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "RegRead Exception: ", ex);
                }
            }
            return (o);
        }

        public static string AdjustInstanceName(string instance)
        {
            if (string.IsNullOrEmpty(instance))
            {
                return (instance);
            }
            if (0 == string.Compare(instance, Environment.MachineName, true))
            {
                return (Environment.MachineName.ToUpper());
            }
            if (instance.Equals(".") ||
                instance.ToLower().Equals("localhost") ||
                instance.Equals("127.0.0.1") ||
                instance.ToLower().Equals("(local)"))
            {
                return (Environment.MachineName.ToUpper());
            }
            if (instance.StartsWith(".\\") ||
                instance.ToLower().StartsWith("localhost\\") ||
                instance.StartsWith("127.0.0.1\\") ||
                instance.ToLower().StartsWith("(local)\\"))
            {
                return (Environment.MachineName.ToUpper() + instance.Substring(instance.IndexOf('\\')).ToUpper());
            }
            if (!instance.Contains(@"\"))
            {
                if (!instance.ToUpper().StartsWith(Environment.MachineName.ToUpper() + @"\"))
                {
                    //if (!IsClustered())
                    {
                        return (Environment.MachineName.ToUpper() + @"\" + instance.ToUpper());
                    }
                }
            }
            return (instance.ToUpper());
        }

        /// <summary>
        /// Determine if the database exists on the given sql server instance.
        /// </summary>
        /// <param name="server">sql server instance</param>
        /// <param name="database">database name</param>
        /// <returns>true if the database is found</returns>
        public static bool DatabaseExists(SqlConnectionInfo info, string database)
        {
            string sql = string.Format("IF (DB_ID({0}) IS NULL) SELECT 0 ELSE SELECT 1", CreateSafeString(database));
            using (SqlConnection conn = GetConnection(info))
            {
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = Constants.DefaultCommandTimeout;
                    return (Convert.ToBoolean(command.ExecuteScalar()));
                }
            }
        }

        /// <summary>
        /// Get the major version of the sql server instance
        /// </summary>
        /// <param name="server">the sql server instance</param>
        /// <returns>the major version of the instance</returns>
        public static int GetMajorVersion(SqlConnectionInfo info)
        {
            string sql = "SELECT CAST(SUBSTRING(CAST(SERVERPROPERTY('ProductVersion') AS NVARCHAR), 1, CHARINDEX('.', CAST(SERVERPROPERTY('ProductVersion') AS NVARCHAR)) -1) AS INT)";
            using (SqlConnection conn = GetConnection(info))
            {
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = Constants.DefaultCommandTimeout;
                    return (Convert.ToInt32(command.ExecuteScalar()));
                }
            }
        }

        public static List<string> GetDatabases(SqlConnectionInfo info)
        {
            string sql = "select name from master..sysdatabases WHERE has_dbaccess(name) = 1 ORDER BY name ASC";
            List<string> databases = null;
            using (SqlConnection conn = GetConnection(info))
            {
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = Constants.DefaultCommandTimeout;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        databases = new List<string>();
                        while (reader.Read())
                        {
                            databases.Add(GetString(reader, 0));
                        }
                    }
                }
            }
            return (databases);
        }

        public static int GetDatabaseId(SqlConnectionInfo info, string database)
        {
            using (SqlConnection conn = GetConnection(info))
            {
                return (GetDatabaseId(conn, database));
            }
        }

        public static int GetDatabaseId(SqlConnection conn, string database)
        {
            string sql = string.Format("SELECT DB_ID({0})", CreateSafeString(database));
            using (SqlCommand command = new SqlCommand(sql, conn))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = Constants.DefaultCommandTimeout;
                return (Convert.ToInt32(command.ExecuteScalar()));
            }
        }
        public static int GetObjectId(SqlConnection conn, string objName)
        {
            string sql = string.Format("SELECT OBJECT_ID({0})", CreateSafeString(objName));
            using (SqlCommand command = new SqlCommand(sql, conn))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = Constants.DefaultCommandTimeout;
                return (Convert.ToInt32(command.ExecuteScalar()));
            }
        }

        public static bool IsSystemObject(SqlConnectionInfo info, UInt32 dbid, int id, string name)
        {
            if (SQLHelper.IsSystemDB(dbid)) return (true);
            using (SqlConnection conn = GetConnection(info))
            {
                return (IsSystemObject(conn, dbid, id, name));
            }
        }

        public static bool IsSystemObject(SqlConnection conn, UInt32 dbid, int id, string name)
        {
            if (SQLHelper.IsSystemDB(dbid)) return (true);
            string obj = string.IsNullOrEmpty(name) ? id.ToString() : string.Format("object_id({0})", CreateSafeString(name));
            string sql = string.Format(Properties.Resources.IsSystemObject, Bracket(GetDatabaseName(conn, dbid)), obj);
            using (SqlCommand command = new SqlCommand(sql, conn))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = Constants.DefaultCommandTimeout;
                return (Convert.ToBoolean(command.ExecuteScalar()));
            }
        }

        public static bool IsSystemTable(SqlConnectionInfo info, string database, string schema, string table)
        {
            using (SqlConnection conn = GetConnection(info))
            {
                return (IsSystemTable(conn, database, schema, table));
            }
        }

        public static bool IsSystemTable(SqlConnection conn, string database, string schema, string table)
        {
            string sql = string.Format(Properties.Resources.IsSystemTable, Bracket(database), CreateSafeString(Bracket(schema, table)));
            StringBuilder sb = new StringBuilder(128);
            using (SqlCommand command = new SqlCommand(sql, conn))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = Constants.DefaultCommandTimeout;
                return (Convert.ToBoolean(command.ExecuteScalar()));
            }
        }

        public static string GetDatabaseName(SqlConnectionInfo info, UInt32 id)
        {
            using (SqlConnection conn = GetConnection(info))
            {
                return (GetDatabaseName(conn, id));
            }
        }
        public static string GetDatabaseName(SqlConnection conn, UInt32 id)
        {
            string sql = string.Format("SELECT DB_NAME({0})", id);
            using (SqlCommand command = new SqlCommand(sql, conn))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = Constants.DefaultCommandTimeout;
                return (Convert.ToString(command.ExecuteScalar()));
            }
        }

        /// <summary>
        /// Get the installed sql server instances on the current machine.
        /// </summary>
        /// <param name="keyPath">the registry key path</param>
        /// <param name="instanceList">the list of instances</param>
        private static void GetInstalledInstances(string keyPath, List<string> instanceList)
        {
            using (_logX.InfoCall(string.Format("SQLHelper.GetInstalledInstances({0})", keyPath)))
            {
                List<string> tempList = new List<string>();
                List<string> clusteredList = new List<string>();
                string machine = Environment.MachineName.ToUpper();

                try
                {
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath))
                    {
                        if (key != null)
                        {
                            string[] instances = (string[])key.GetValue("InstalledInstances");

                            if (instances != null)
                            {
                                foreach (string instance in instances)
                                {
                                    if (0 == string.Compare(instance, "MSSQLSERVER", true))
                                    {
                                        if (!instanceList.Contains(machine))
                                        {
                                            instanceList.Add(machine);
                                            //_logX.Info("Add instance " + machine);
                                        }
                                    }
                                    else if (0 == string.Compare(instance, "MSSMLBIZ", true))
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        //----------------------------------------------------------------------------
                                        // Build a temp list of instance names to be used in looking up the clustered
                                        // address of the sql server instance
                                        // 
                                        if (!tempList.Contains(instance.ToUpper()))
                                        {
                                            tempList.Add(instance.ToUpper());
                                            _logX.Info("Add temp instance " + instance);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)  // Error reading registry, continue
                {
                    ExceptionLogger.Log(string.Format("GetInstances({0})", keyPath), ex);
                }

                //----------------------------------------------------------------------------
                // Additional lookup to determine if there are virtual addresses due to clustering
                // 
                try
                {
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath + @"\Instance Names\SQL"))
                    {
                        if (key != null)
                        {
                            string[] valueNames = key.GetValueNames();
                            bool defaultInstance;
                            _logX.Info(@"Instance Names\SQL: ", valueNames);
                            foreach (string valueName in valueNames)
                            {
                                defaultInstance = (0 == string.Compare(valueName, "MSSQLSERVER", true));
                                if (tempList.Contains(valueName) || defaultInstance)
                                {
                                    object obj = key.GetValue(valueName);
                                    if (null != obj)
                                    {
                                        string clusterName = GetClusterNameValue(keyPath + @"\" + obj.ToString());
                                        if (!string.IsNullOrEmpty(clusterName))
                                        {
                                            clusteredList.Add(clusterName + (defaultInstance ? "" : @"\" + valueName));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)  // Error reading registry, continue
                {
                    ExceptionLogger.Log(string.Format("GetInstances({0})", keyPath + @"\Instance Names\SQL"), ex);
                }

                if (clusteredList.Count > 0)
                {
                    AppendToList(string.Empty, clusteredList, instanceList);
                }
                else
                {
                    AppendToList(machine + @"\", tempList, instanceList);
                }
            }
        }

        private static void AppendToList(string prefix, List<string> tempList, List<string> instanceList)
        {
            foreach (string instance in tempList)
            {
                if (!instanceList.Contains(prefix + instance))
                {
                    _logX.Info("Add Instance " + prefix + instance);
                    instanceList.Add(prefix + instance);
                }
            }
        }

        /// <summary>
        /// Return the clustered virtual server address for the sql server instance.
        /// </summary>
        /// <param name="keyPath"></param>
        /// <returns></returns>
        private static string GetClusterNameValue(string keyPath)
        {
            using (_logX.InfoCall(string.Format("SQLHelper.GetClusterNameValue({0})", keyPath)))
            {
                try
                {
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath + @"\Cluster"))
                    {
                        if (key != null)
                        {
                            object obj = key.GetValue("ClusterName");
                            if (null != obj)
                            {
                                _logX.Info("ClusterName ", obj);
                                return (obj.ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(string.Format("GetClusterNameValue({0})", keyPath), ex);
                }
                return (string.Empty);
            }
        }

        public static IEnumerable<string> GetAllServerInstances()
        {
            DataTable table = System.Data.Sql.SqlDataSourceEnumerator.Instance.GetDataSources();
            foreach (DataRow row in table.Rows)
            {
                if (null != row[0]) yield return (row[0].ToString() + (string.IsNullOrEmpty(row[1].ToString()) ? "" : "\\" + row[1].ToString()));
            }
        }

        /// <summary>
        /// Return a list of local sql server instances.  Support a clustered environment.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetLocalServerInstances()
        {
            using (_logX.InfoCall("SQLHelper.GetLocalServerInstances()"))
            {
                List<string> instanceList = new List<string>();
                string machine = Environment.MachineName.ToUpper();
                string prefix = machine + "\\";

                GetInstalledInstances(@"SOFTWARE\Microsoft\Microsoft SQL Server", instanceList);
                GetInstalledInstances(@"SOFTWARE\Wow6432Node\Microsoft\Microsoft SQL Server", instanceList);
                if (instanceList.Count == 0)
                {
                    if (IsSQLPath(@"SOFTWARE\Microsoft\MSSQLServer\Setup") || IsSQLPath(@"SOFTWARE\Wow6432Node\Microsoft\MSSQLServer\Setup"))
                    {
                        instanceList.Add(machine);
                    }
                }

                int serverMajorVersion;
                foreach (string instance in instanceList)
                {
                    try
                    {
                        SqlConnectionInfo info = new SqlConnectionInfo(instance);
                        info.ConnectionTimeout = 2;
                        serverMajorVersion = SQLHelper.GetMajorVersion(new SqlConnectionInfo(instance));
                    }
                    catch (Exception ex)  // Error pulling the server version.
                    {
                        ExceptionLogger.Log(string.Format("GetLocalServerInstances: GetMajorVersion({0}) Exception: ", instance), ex);
                        continue;
                    }
                    //----------------------------------------------------------------------------
                    // Only return the supported SQL server versions (currently 2005 and 2008)
                    // 
                    if ((serverMajorVersion == (int)SSVer_e.SS_2008) ||
                        (serverMajorVersion == (int)SSVer_e.SS_2005))
                    {
                        yield return (instance);
                    }
                }
            }
        }

        private static bool IsSQLPath(string keyPath)
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath))
                {
                    string test = key.GetValue("SQLPath") as string;
                    return (!string.IsNullOrEmpty(test));
                }
            }
            catch (Exception ex)  // Error reading registry, continue
            {
                ExceptionLogger.Log("IsSQLPath(string keyPath)", ex);
            }
            return (false);
        }


        public static SqlConnection GetConnection(SqlConnectionInfo info)
        {
            SqlConnection conn = info.GetConnection();
            conn.Open();
            return conn;
        }

        public static void CheckConnection(SqlConnection conn)
        {
            if (conn.State == ConnectionState.Open) return;

            if (conn.State == ConnectionState.Broken)
            {
                conn.Close();
            }

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
        }

        #region Safe SqlReader readers

        static public int ByteToInt(SqlDataReader rdr, int index)
        {
            int retval = -1;

            if (!rdr.IsDBNull(index))
            {
                retval = (int)rdr.GetByte(index);
            }

            return retval;
        }

        static public bool ByteToBool(SqlDataReader rdr, int index)
        {
            bool retval = false;

            if (!rdr.IsDBNull(index))
            {
                if (rdr.GetByte(index) != 0)
                    retval = true;
                else
                    retval = false;
            }

            return retval;
        }

        static public string GetString(SqlDataReader rdr, int index)
        {
            string retval;

            if (!rdr.IsDBNull(index))
            {
                retval = rdr.GetString(index);
            }
            else
            {
                retval = "";
            }

            return retval;
        }

        static public string GetSafeString(SqlDataReader rdr, int index)
        {
            string retval;

            retval = GetString(rdr, index);
            return CreateSafeString(rdr.GetString(index));
        }

        static public DateTime GetDateTime(SqlDataReader rdr, int index)
        {
            DateTime retval;

            if (!rdr.IsDBNull(index))
            {
                retval = rdr.GetDateTime(index);
            }
            else
            {
                retval = DateTime.MinValue;
            }

            return retval;
        }

        static public double GetDouble(SqlDataReader rdr, int index)
        {
            double retval;

            if (!rdr.IsDBNull(index))
            {
                retval = rdr.GetDouble(index);
            }
            else
            {
                retval = 0.0;
            }

            return retval;
        }

        static public long GetInt64(SqlDataReader rdr, int index)
        {
            long retval;

            if (!rdr.IsDBNull(index))
            {
                retval = rdr.GetInt64(index);
            }
            else
            {
                retval = 0;
            }

            return retval;
        }

        static public byte GetByte(SqlDataReader rdr, int index)
        {

            if (!rdr.IsDBNull(index))
            {
                return (rdr.GetByte(index));
            }
            return 0;
        }

        static public bool GetBoolean(SqlDataReader rdr, int index)
        {

            if (!rdr.IsDBNull(index))
            {
                return (rdr.GetBoolean(index));
            }
            return false;
        }

        static public int GetInt32(SqlDataReader rdr, int index)
        {
            int retval;

            if (!rdr.IsDBNull(index))
            {
                retval = rdr.GetInt32(index);
            }
            else
            {
                retval = 0;
            }

            return retval;
        }

        static public int GetInt16(SqlDataReader rdr, int index)
        {
            int retval;

            if (!rdr.IsDBNull(index))
            {
                retval = rdr.GetInt16(index);
            }
            else
            {
                retval = 0;
            }

            return retval;
        }

        #endregion

        #region Safe DataRow readers

        static public string
           GetRowString(
              DataRow row,
              string colName
           )
        {
            if (row.IsNull(colName))
                return "";
            else
                return (string)row[colName];
        }

        static public int
           GetRowInt32(
              DataRow row,
              int colNdx
           )
        {
            if (row.IsNull(colNdx))
                return 0;
            else
                return (int)row[colNdx];
        }

        static public DateTime
           GetRowDateTime(
              DataRow row,
              int colNdx
           )
        {
            if (row.IsNull(colNdx))
                return DateTime.MinValue;
            else
                return (DateTime)row[colNdx];
        }

        static public string
           GetRowString(
              DataRow row,
              int colNdx
           )
        {
            if (row.IsNull(colNdx))
                return "";
            else
                return (string)row[colNdx];
        }

        static public int
           GetRowInt32(
              DataRow row,
              string colName
           )
        {
            if (row.IsNull(colName))
                return 0;
            else
                return (int)row[colName];
        }

        static public DateTime
           GetRowDateTime(
              DataRow row,
              string colName
           )
        {
            if (row.IsNull(colName))
                return DateTime.MinValue;
            else
                return (DateTime)row[colName];
        }

        #endregion

        #region Get SQL Server Version Routines

        //------------------------------------------------------------------
        // GetSqlVersionString - Retrieve instance's version string
        //------------------------------------------------------------------
        static public string
           GetSqlVersionString(
              SqlConnection conn
           )
        {
            string versionString = null;

            try
            {
                using (SqlCommand myCommand = new SqlCommand("SELECT @@VERSION", conn))
                {
                    versionString = (string)myCommand.ExecuteScalar();
                }
            }
            catch { }

            return versionString;
        }

        //------------------------------------------------------------------
        // GetSqlVersion - return integer representation of major version
        //                 8 = 2000, 9 = 2005
        //
        //  Two versions
        //  (1) Read from SQL Server and parse
        //  (2) Just parse a string for version
        //
        //   Note: Any badness in string results in us returning 0 for version
        //------------------------------------------------------------------
        // GetSqlVersion - read from SQL Server
        //------------------------------------------------------------------
        static public int
           GetSqlVersion(
              SqlConnection conn
           )
        {
            string sqlVersionString = GetSqlVersionString(conn);

            if (sqlVersionString == null) return 0;

            return GetSqlVersion(sqlVersionString);
        }

        //------------------------------------------------------------------
        // GetSqlVersion - parse version string
        //------------------------------------------------------------------
        static public int
           GetSqlVersion(
              string versionString
           )
        {
            int sqlVersion = 0;
            string sqlVersionMajor;
            int iStart, iEnd;
            try
            {
                sqlVersion = 0;
                iStart = versionString.IndexOf("- ");
                if (iStart == -1) return 0;
                iStart += 2;
                iEnd = versionString.IndexOf(".", iStart);
                if (iEnd == -1) return 0;
                sqlVersionMajor = versionString.Substring(iStart, iEnd - iStart);
                sqlVersion = int.Parse(sqlVersionMajor);
            }
            catch
            {
                sqlVersion = 0;
            }
            return sqlVersion;
        }
        #endregion

        #region Create Escaped Strings

        //-----------------------------------------------------------------------
        // CreateSafeString - creates safe string parameter includes
        //                    single quotes; used to create sql parameters
        //-----------------------------------------------------------------------
        static public string CreateSafeString(string propName)
        {
            return CreateSafeString(propName, int.MaxValue);
        }

        //-----------------------------------------------------------------------
        // CreateSafeString - creates safe string parameter includes
        //                    single quotes; used to create sql parameters with
        //                    length limit
        //-----------------------------------------------------------------------
        static public string CreateSafeString(string propName, int limit)
        {
            StringBuilder newName;
            string tmpValue;

            if (propName == null)
            {
                newName = new StringBuilder("null");
            }
            else
            {
                newName = new StringBuilder("'");
                tmpValue = propName.Replace("'", "''");
                if (tmpValue.Length > limit)
                {
                    if (tmpValue[limit - 1] == '\'')
                    {
                        limit--;
                        if (tmpValue[limit - 1] == '\'')
                            limit--;
                    }
                    tmpValue = tmpValue.Remove(limit, tmpValue.Length - limit);
                }
                newName.Append(tmpValue);
                newName.Append("'");
            }

            return newName.ToString();
        }

        //-----------------------------------------------------------------------
        // CreateSafeDateTimeString - creates safe string parameter
        //                            includes single quotes
        //                            used to create sql parameters
        //-----------------------------------------------------------------------
        static public string
           CreateSafeDateTimeString(
              DateTime timestamp
           )
        {
            return CreateSafeDateTimeString(timestamp, true);
        }

        //-----------------------------------------------------------------------
        // CreateSafeDateTimeString - creates safe string parameter
        //                            includes single quotes
        //                            used to create sql parameters
        //-----------------------------------------------------------------------
        static public string
           CreateSafeDateTimeString(
              DateTime timestamp,
              bool includeQuotes
           )
        {
            /* The problem with CultureInfo.CurrentCulture.DateTimeFormat is it doesn't work
             * on UK platforms.  SQL Server complains that the datetime values cannot be converted.
             * So use the SQL Server CONVERT function to explicitly convert the value from ODBC format */
            /*
            StringBuilder newName = new StringBuilder("");
         
            if ( timestamp == DateTime.MinValue )
            {
               newName.Append("null");
            }
            else
            {
                  if ( includeQuotes) newName = new StringBuilder("'");
               //newName.Append( timestamp.ToString( "yyyy-MM-dd HH:mm:ss.fff",
               //                                    DateTimeFormatInfo.InvariantInfo ) );
               newName.AppendFormat( timestamp.ToString( CultureInfo.CurrentCulture.DateTimeFormat ) );
                 if ( includeQuotes) newName.Append("'");
              }
		   
               //builder.Append(String.Format("{0}-{1}-{2} {3}:{4}:{5}", filterDateTime.Year, filterDateTime.Month,
               //filterDateTime.Day, filterDateTime.Hour, filterDateTime.Minute, filterDateTime.Second)) ;
            //ErrorLog.Instance.Write( ErrorLog.Level.UltraDebug, "CreateSafeDateTimeString() returning " + newName.ToString());
		   
         
            //return newName.ToString();
            */
            return CreateSafeDateTime(timestamp);
        }

        //-----------------------------------------------------------------------
        // CreateSafeDateTime - creates a SQL Server CONVERT function call for 
        //                      DateTime Value
        //-----------------------------------------------------------------------
        static public string
           CreateSafeDateTime(
              DateTime timestamp
           )
        {
            string newString;

            if (timestamp == DateTime.MinValue)
            {
                newString = "null";
            }
            else
            {
                newString = String.Format("CONVERT(DATETIME, '{0}-{1}-{2} {3}:{4}:{5}.{6:000}',121)", timestamp.Year,
                    timestamp.Month, timestamp.Day, timestamp.Hour, timestamp.Minute, timestamp.Second, timestamp.Millisecond);
                /*
                newString = String.Format( "CONVERT( DATETIME, '{0}', 20 ) ", 
                                           timestamp.ToString( "yyyy-MM-dd HH:mm:ss.fff",
                                                                            DateTimeFormatInfo.InvariantInfo ));
                                                                            */
            }
            //ErrorLog.Instance.Write( ErrorLog.Level.UltraDebug, "CreateSafeDateTime() returning " + newString);

            return newString;
        }

        static public string CreateDateTimeForStoredProc(DateTime timestamp)
        {
            string newString;

            if (timestamp == DateTime.MinValue)
            {
                newString = "null";
            }
            else
            {
                newString = String.Format("'{0}-{1}-{2} {3}:{4}:{5}.{6:000}'", timestamp.Year,
                   timestamp.Month, timestamp.Day, timestamp.Hour, timestamp.Minute, timestamp.Second, timestamp.Millisecond);
            }
            return newString;
        }

        /// <summary>
        /// Bracket the given string if needed
        /// </summary>
        /// <param name="s">string to bracket</param>
        /// <returns>bracketed string</returns>
        static public string Bracket(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            //if (s.StartsWith("[") && s.EndsWith("]")) return (s);
            return (CreateBracketedString(s));
        }

        static public string Bracket(string database, string schema, string table)
        {
            return (string.Format("{0}.{1}.{2}", Bracket(database), Bracket(schema), Bracket(table)));
        }

        static public string Bracket(string schema, string table)
        {
            return (string.Format("{0}.{1}", Bracket(schema), Bracket(table)));
        }

        //-----------------------------------------------------------------------
        // CreateSafeDatabaseName - creates safe db name for SQL
        //-----------------------------------------------------------------------
        static public string CreateBracketedString(string dbName)
        {
            StringBuilder newName;

            newName = new StringBuilder("[");
            newName.Append(dbName.Replace("]", "]]"));
            newName.Append("]");

            return newName.ToString();
        }

        //-----------------------------------------------------------------------
        // RemoveBrackets - convert the db name away from a bracketed string
        //-----------------------------------------------------------------------
        static public string RemoveBrackets(string dbName)
        {
            if (dbName != null && dbName.StartsWith("[")) { return (dbName.Replace("]]", "]").TrimStart('[').TrimEnd(']'));}
            return dbName;
        }

        //-----------------------------------------------------------------------
        // CreateSafeDatabaseNameForConnectionString
        //
        // (1) If database begins or ends with blanks, wrap in quotes
        // (2) If contains one of ;'" then you need to espace with ' or ". Use "
        //     unless first character is single quote then use double quote
        // (3) If string contains any escaped chars enclose in quotes
        //-----------------------------------------------------------------------
        static public string CreateSafeDatabaseNameForConnectionString(string dbName)
        {
            if (string.IsNullOrEmpty(dbName))
            {
                return (dbName);
            }

            // Use double quote as escape character unless first character is double
            // quote; then use single quote
            string doubleQuote = "\"";

            bool encloseInQuotes = false;

            // Do we need to enclose in quotes? (contains semicolon or leading or trailing spaces)
            if ((-1 != dbName.IndexOf(";")) ||
                 (dbName[0] == ' ' || dbName[dbName.Length - 1] == ' '))
            {
                encloseInQuotes = true;
            }

            if (encloseInQuotes)
            {
                // escape any double quotes
                dbName = dbName.Replace(doubleQuote, "\"\"");
                dbName = doubleQuote + dbName + doubleQuote;
            }

            return (dbName);
        }

        #endregion

        static public List<StringBuilder> SqlParser(string sql, string batchSeparator)
        {
            bool newBatchNextLine = true;
            char[] crlf = new char[] { '\r', '\n' };
            string currentLine = "";

            List<StringBuilder> batches = new List<StringBuilder>();

            while (sql != "")
            {
                // loop through lines of SQL one line at a time
                int pos = sql.IndexOfAny(crlf);
                if (pos != -1)
                {
                    // append all CR or LF to current line
                    while (pos + 1 <= sql.Length)
                    {
                        if (sql[pos] == '\r' || sql[pos] == '\n') pos++;
                        else break;
                    }
                    currentLine = sql.Substring(0, pos);
                    sql = sql.Substring(pos);
                }
                else
                {
                    currentLine = sql;
                    sql = "";
                }

                bool foundGO = false;

                // eat leading white space and comments
                string consumedLine = ConsumeLeadingWhiteSpaceAndComments(currentLine);

                if (consumedLine.ToUpper().StartsWith(batchSeparator))
                {
                    foundGO = true;

                    // is there anything after the GO but comments
                    consumedLine = consumedLine.Substring(2);
                    consumedLine = ConsumeLeadingWhiteSpaceAndComments(consumedLine);

                    if (consumedLine.Length > 2 && !consumedLine.StartsWith("--") && !consumedLine.StartsWith("\r\n"))
                    {
                        foundGO = false;
                    }
                }

                if (foundGO)
                {
                    newBatchNextLine = true;
                }
                else
                {
                    if (newBatchNextLine)
                    {
                        batches.Add(new StringBuilder(1024));
                        newBatchNextLine = false;
                    }
                    batches[batches.Count - 1].Append(currentLine);
                }
            }

            return (batches);
        }

        static private string ConsumeLeadingWhiteSpaceAndComments(string inString)
        {
            string consumedLine = inString;

            // we dont care about the final string so we can just do a blind
            // replace of tabs with spaces - makes it easier to deal with white space
            consumedLine = consumedLine.Replace('\t', ' ');

            bool stillEating = true;

            while (stillEating)
            {
                stillEating = false;

                consumedLine = consumedLine.Trim();
                if (consumedLine.Length >= 2 && consumedLine.StartsWith("/*"))
                {
                    // in comment - eat it
                    int pos = consumedLine.IndexOf("*/");
                    if (pos == -1)
                    {
                        consumedLine = "";
                    }
                    else
                    {
                        consumedLine = consumedLine.Substring(pos + 2);
                        stillEating = true;  //; keep eating as long as we find comments
                    }
                }
            }

            return (consumedLine);
        }

        private const string GetProcedureSourceSql = "select c.text from sys.procedures p left outer join sys.syscomments c on p.object_id = c.id where p.name = @PROCNAME and is_ms_shipped = 0 and encrypted = 0";
        public static string GetProcedureSource(SqlConnection connection, string database, string procedureName)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();
  
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = GetProcedureSourceSql;
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@PROCNAME", SqlDbType.NVarChar, 128).Value = procedureName;
                object value = command.ExecuteScalar();
                if (value != DBNull.Value)
                    return value.ToString();
            }
            return null;
        }
    }
}

