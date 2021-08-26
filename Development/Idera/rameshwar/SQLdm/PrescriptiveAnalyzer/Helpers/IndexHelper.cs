using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Batches;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Helpers
{
    internal static class IndexHelper
    {
        private static Logger _logX = Logger.GetLogger("IndexHelper");

        public static long GetNonclusteredIndexSize(ServerVersion ver, SqlConnection conn, string database, string schema, string table, IEnumerable<string> eqColumns, IEnumerable<string> neqColumns, IEnumerable<string> includeColumns)
        {
            List<string> keyColumns = new List<string>();
            if (null != eqColumns)
            {
                foreach (string col in eqColumns) keyColumns.Add(col);
            }
            if (null != neqColumns)
            {
                foreach (string col in neqColumns) keyColumns.Add(col);
            }
            return (GetNonclusteredIndexSize(ver, conn, database, schema, table, keyColumns, includeColumns));
        }
        public static long GetNonclusteredIndexSize(ServerVersion ver, SqlConnection conn, string database, string schema, string table, IEnumerable<string> keyColumns, IEnumerable<string> includeColumns)
        {
            using (_logX.InfoCall(string.Format("GetNonclusteredIndexSize", SQLHelper.Bracket(database, schema, table))))
            {
                try
                {
                    string sql = string.Format("select name, column_id from sys.columns where object_id = object_id({0});", SQLHelper.CreateSafeString(SQLHelper.Bracket(schema, table)));
                    Dictionary<string, int> colMap = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                        cmd.CommandType = System.Data.CommandType.Text;
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                colMap.Add(DataHelper.ToString(r, 0), DataHelper.ToInt32(r, 1));
                            }
                        }
                    }
                    if (colMap.Count <= 0)
                    {
                        _logX.Info("No columns found!");
                        return (0);
                    }

                    sql = BatchFinder.GetEstimateNonclusteredIndexSize(ver, database, schema, table, false, 100, GetColumnIds(colMap, keyColumns), GetColumnIds(colMap, includeColumns));
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                        cmd.CommandType = System.Data.CommandType.Text;
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                int Index_Rows_Per_Page = SQLHelper.GetInt32(r, 0);
                                long Num_Leaf_Pages = SQLHelper.GetInt64(r, 1);
                                long Leaf_Space_Used = SQLHelper.GetInt64(r, 2);
                                long Nonleaf_Levels = 1 + Convert.ToInt64(Math.Ceiling(Math.Log(((double)Num_Leaf_Pages / Index_Rows_Per_Page), Index_Rows_Per_Page)));
                                long Num_Index_Pages = 0;
                                long Index_Space_Used = 0;
                                while (Nonleaf_Levels >= 1)
                                {
                                    Num_Index_Pages += Convert.ToInt64(Math.Ceiling((double)Num_Leaf_Pages / Math.Pow(Index_Rows_Per_Page, Nonleaf_Levels--)));
                                }
                                Index_Space_Used = 8192 * Num_Index_Pages;
                                return (Leaf_Space_Used + Index_Space_Used);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "GetNonclusteredIndexSize", ex);
                }
                return (0);
            }
        }

        private static int[] GetColumnIds(Dictionary<string, int> map, IEnumerable<string> names)
        {
            if (null == names) return (null);
            //if (names.Count() <= 0) return (null);
            //int[] ids = new int[names.Count()];
            int namesCount = 0;
            foreach (var item in names)
            { namesCount++; }
            if (namesCount <= 0) return (null);
            int[] ids = new int[namesCount];
            int i = 0;
            foreach (string name in names) ids[i++] = GetColumnId(map, name);
            return (ids);
        }

        private static int GetColumnId(Dictionary<string, int> map, string name)
        {
            int id = 0;
            if (!map.TryGetValue(name, out id))
            {
                map.TryGetValue(SQLHelper.RemoveBrackets(name), out id);
            }
            return (id);
        }
    }
}
