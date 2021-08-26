using System;
using System.Data;
using System.Data.SqlClient;
//using System.Linq;
using System.Text;
using System.Collections.Generic;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
//using Idera.SQLdm.PrescriptiveAnalyzer.Batches;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Batches;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Collectors
{
    public class WorstPerformingTSQL
    {
        private static Logger _logX = Logger.GetLogger("WorstPerformingTSQL");

        List<TEWorstTSQL> _worstTSQL = null;
        BaseOptions _options = null;

        public IEnumerable<TEWorstTSQL> WorstTSQL { get { return (_worstTSQL); } }

        private WorstPerformingTSQL() { }
        public WorstPerformingTSQL(SqlConnection conn, BaseOptions options)
        {
            _options = options;
            using (_logX.InfoCall("WorstPerformingTSQL Construction"))
            {
                try
                {
                    Load(conn, new ServerVersion(conn.ServerVersion));
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "WorstPerformingTSQL.Load() Exception: ", ex);
                }
            }
        }
        private void Load(SqlConnection conn, ServerVersion ver)
        {
            using (DataSet ds = new DataSet())
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(BatchFinder.GetBatch("WorstPerformingTSQL", ver), conn))
                {
                    adapter.SelectCommand.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                    try
                    {
                        adapter.Fill(ds);
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogger.Log(_logX, "WorstPerformingTSQL.SqlDataAdapter.Fill() Exception: ", ex);
                    }
                }
                if (null == ds) return;
                if (null == ds.Tables) return;
                SQLSchemaNameHelper ssnh = new SQLSchemaNameHelper(conn);
                foreach (DataTable dt in ds.Tables)
                {
                    if (null == dt) continue;
                    if (null == dt.Rows) continue;
                    if (0 >= dt.Rows.Count) continue;
                    if (null == _worstTSQL) _worstTSQL = new List<TEWorstTSQL>(dt.Rows.Count);
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (null == dr) continue;
                        _worstTSQL.Add(new TEWorstTSQL(dr, ssnh, _options));
                    }
                }
            }
        }
    }
}
