using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using System.Data.SqlClient;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Batches;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Helpers
{
    internal static class AutoPilotHelpers
    {
        private static Logger _logX = Logger.GetLogger("AutoPilotHelpers");
        private static readonly int MAX_PLAN_CHAR_COUNT = 5 * 1024 * 1024; // 5 meg max characters

        internal static bool Init(SqlConnection cnn, int dbid)
        {
            StringBuilder sb = new StringBuilder(1024);
            sb.AppendLine("set quoted_identifier on");
            sb.AppendLine("set arithabort on");
            sb.AppendLine("set concat_null_yields_null on");
            sb.AppendLine("set ansi_nulls on");
            sb.AppendLine("set ansi_padding on");
            sb.AppendLine("set ansi_warnings on");
            sb.AppendLine("set numeric_roundabort off");
            ExecNonQuery(cnn, sb.ToString());

            sb.Remove(0, sb.Length);
            sb.AppendLine(String.Format("dbcc autopilot (5,{0})", dbid));
            sb.AppendLine(String.Format("dbcc autopilot (11,{0},0,0,0,0)", dbid));
            return ExecNonQuery(cnn, sb.ToString());
        }

        /// <summary>
        /// Set the statements within a batch that should be analyzed.  When a batch has multiple statements and
        /// only a certain number of those statements need to be analyzed, this method can be used to specify which
        /// statements within the batch based on the statement index.  
        /// 
        /// For example, 
        /// 
        ///   if only the third statement within a batch should be analyzed:
        ///     dbcc autopilot (8,{db id}, 0, 0, 0, 3)  
        ///     
        ///   if the third and forth statements within a batch should be analyzed:
        ///     dbcc autopilot (8,{db id}, 0, 0, 0, 3)  
        ///     dbcc autopilot (8,{db id}, 0, 0, 0, 4)  
        ///     
        /// Note:  If all statements within a batch should be analyzed, this method does not need to be used.
        /// 
        /// </summary>
        /// <param name="cnn">SQL Server connection object</param>
        /// <param name="dbid">The id for the database</param>
        /// <param name="stmtIdx">Indexes of the statements that should be analyzed</param>
        /// <returns>true for success</returns>
        internal static bool SetStmtsToAnalyze(SqlConnection cnn, int dbid, IEnumerable<int> stmtIdx)
        {
            if (null == stmtIdx) return (false);
            //if (stmtIdx.Count() <= 0) return (false);
            int stmtIdxCount = 0;
            foreach (var item in stmtIdx)
            { stmtIdxCount++; }
            if (stmtIdxCount <= 0) return (false);
            StringBuilder sb = new StringBuilder(1024);
            foreach (int idx in stmtIdx) sb.AppendLine(String.Format("dbcc autopilot (8,{0}, 0, 0, 0, {1})", dbid, idx));
            return ExecNonQuery(cnn, sb.ToString());
        }

        /// <summary>
        /// If 'SetStmtsToAnalyze' was called, this should be called after the batch has been analyzed to return back
        /// to the default state.
        /// </summary>
        /// <param name="cnn">SQL Server connection object</param>
        /// <param name="dbid">The id for the database</param>
        /// <returns>true for success</returns>
        internal static bool ResetStmtsToAnalyze(SqlConnection cnn, int dbid)
        {
            return ExecNonQuery(cnn, String.Format("dbcc autopilot (8,{0}, 0, 0, 0, 0)", dbid));
        }

        /// <summary>
        /// Reset the hypothetical indexes that are available for use on the database.
        /// </summary>
        /// <param name="cnn">SQL Server connection object</param>
        /// <param name="dbid">The database id</param>
        /// <returns>true for success</returns>
        internal static bool ResetHypotheticalIndexes(SqlConnection cnn, int dbid)
        {
            return ExecNonQuery(cnn, String.Format("dbcc autopilot (5,{0}, 0, 0, 0)", dbid));
        }

        /// <summary>
        /// Set the hypothetical indexes that are available for use on a database table.
        /// </summary>
        /// <param name="cnn">SQL Server connection object</param>
        /// <param name="dbid">The database id</param>
        /// <param name="tblid">The table id</param>
        /// <param name="heapOrClustered">Stats for a heap or a hypthetical clustered index</param>
        /// <param name="indexStats">hypothetical index stats for non-clustered indexes</param>
        /// <returns>true for success</returns>
        internal static bool SetHypotheticalIndexes(SqlConnection cnn, int dbid, int tblid, IndexStats heapOrClustered, IEnumerable<IndexStats> indexStats)
        {
            StringBuilder sb = new StringBuilder(1024);
            sb.AppendLine(String.Format("dbcc autopilot (5,{0}, 0, 0, 0)", dbid));
            if (null != heapOrClustered)
            {
                if (heapOrClustered.Heap)
                {   // append heap stats
                    sb.AppendLine(String.Format("dbcc autopilot (10,{0}, {1}, 0, {2}, 0, {3})", dbid, tblid, heapOrClustered.Pages, heapOrClustered.Rows));
                }
                else
                {   // append clustered index stats
                    sb.AppendLine(String.Format("dbcc autopilot (6,{0}, {1}, 0, {2}, 0, {3})", dbid, tblid, heapOrClustered.Pages, heapOrClustered.Rows));
                }
            }
            //----------------------------------------------------------------------------
            // Build all of the stats for the non-clustered indexes
            // 
            if (null != indexStats)
            {
                foreach (IndexStats stats in indexStats)
                {
                    sb.AppendLine(String.Format("dbcc autopilot (0,{0}, {1}, {2}, {3}, 0, {4})", dbid, tblid, stats.ID, stats.Pages, stats.Rows));
                }
            }
            return ExecNonQuery(cnn, sb.ToString());
        }

        /// <summary>
        /// Set the hypothetical indexes that are available for use on a database table.
        /// </summary>
        /// <param name="cnn">SQL Server connection object</param>
        /// <param name="dbid">The database id</param>
        /// <param name="tblid">The table id</param>
        /// <param name="indexStats">hypothetical index stats for non-clustered indexes</param>
        /// <returns>true for success</returns>
        internal static bool SetHypotheticalIndexes(SqlConnection cnn, int dbid, int tblid, IEnumerable<IndexStats> indexStats)
        {
            return (SetHypotheticalIndexes(cnn, dbid, tblid, null, indexStats));
        }

        internal static bool ClearOptimizerHint(SqlConnection cnn, int dbid)
        {
            string query = String.Format("dbcc autopilot (5,{0},0,0,0)", dbid);
            return ExecNonQuery(cnn, query);
        }

        internal static bool SetOptimizerHints(SqlConnection cnn, IEnumerable<OptimizerHint> hints)
        {
            StringBuilder builder = new StringBuilder();

            foreach (OptimizerHint hint in hints)
            {
                string test = String.Format("dbcc autopilot (6,{0},{1},{2},0,0,0)", hint.DatabaseId, hint.TableId, hint.IndexId);
                if (ExecNonQuery(cnn, test))
                {
                    builder.AppendLine(test);
                }
            }

            return ExecNonQuery(cnn, builder.ToString());
        }

        internal static bool SetAutoPilot(SqlConnection cnn, bool on)
        {
            return ExecNonQuery(cnn, String.Format("set autopilot {0}", on ? "on" : "off"));
        }

        internal static bool SetShowPlan(SqlConnection cnn, bool on)
        {
            return ExecNonQuery(cnn, String.Format("set showplan_xml_with_recompile {0}", on ? "on" : "off"));
        }

        internal static string RunWithAutoPilot(SqlConnection cnn, string batch)
        {
            if (SetAutoPilot(cnn, true))
            {
                try
                {
                    return ExecForPlan(cnn, batch);
                }
                finally
                {
                    SetAutoPilot(cnn, false);
                }
            }

            return null;
        }

        internal static string RunWithShowPlan(SqlConnection cnn, string batch, bool throwException)
        {
            if (SetShowPlan(cnn, true))
            {
                try
                {
                    return ExecForPlan(cnn, batch, throwException);
                }
                finally
                {
                    SetShowPlan(cnn, false);
                }
            }

            return null;
        }


        internal static string ExecForPlan(SqlConnection cnn, string query)
        {
            return (ExecForPlan(cnn, query, false));
        }
        internal static string ExecForPlan(SqlConnection cnn, string query, bool throwException)
        {
            StringBuilder result = new StringBuilder();
            try
            {
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                    cmd.CommandType = System.Data.CommandType.Text;
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        do
                        {
                            while (r.Read())
                            {
                                if (result.Length >= MAX_PLAN_CHAR_COUNT)
                                {
                                    using (_logX.ErrorCall("ExecForPlan MAX LIMIT!"))
                                    {
                                        _logX.Error(query.Replace("\r\n", "\n").Replace("\t", "  "));
                                    }
                                    return (string.Empty);
                                }
                                result.Append(r.GetSqlString(0).ToString());
                            }
                        } while (r.NextResult());
                    }
                }
            }
            catch (Exception e)
            {
                using (_logX.DebugCall("ExecForPlan Error:" + e.Message)) 
                { 
                    _logX.Debug(query.Replace("\r\n", "\n").Replace("\t", "  "));
                    if (e is System.OutOfMemoryException)
                    {
                        ProcessInfoHelper.Log(_logX);
                    }
                }
                if (throwException) throw;
                return (string.Empty);
            }
            return result.ToString();
        }

        internal static bool ExecNonQuery(SqlConnection cnn, string query)
        {
            try
            {
                SQLHelper.CheckConnection(cnn);
                using (SqlCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                    cmd.CommandType = System.Data.CommandType.Text;
                    cmd.ExecuteNonQuery();
                    return (true);
                }
            }
            catch (Exception e)
            {
                using (_logX.DebugCall("ExecNonQuery Error:" + e.Message)) { _logX.Debug(query.Replace("\r\n", "\n").Replace("\t", "  ")); }
            }
            return (false);
        }

        internal class IndexStats
        {
            private IndexStats() { }
            public IndexStats(int id, UInt64 pages, UInt64 rows) { ID = id; Pages = pages; Rows = rows; }
            public int ID { get; set; }
            public bool Heap { get { return (0 == ID); } }
            public UInt64 Pages { get; set; }
            public UInt64 Rows { get; set; }
        }
    }
}
