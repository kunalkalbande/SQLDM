using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using props = Idera.SQLdm.PrescriptiveAnalyzer.Common.Properties;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Ranking
{
    internal class TableRankingStats
    {
        private UInt64 _lowIO = UInt64.MaxValue;
        private UInt64 _highIO = UInt64.MinValue;

        private Dictionary<string, UInt64> _tableToIO = new Dictionary<string, UInt64>();

        private TableRankingStats() { }
        public TableRankingStats(SqlConnectionInfo info)
        {
            try
            {
                SQLSchemaNameHelper ssnh = new SQLSchemaNameHelper(info);

                using (SqlConnection conn = SQLHelper.GetConnection(info))
                {
                    SqlCommand command = new SqlCommand(props.Resources.TableRankingStats, conn);
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = Constants.DefaultCommandTimeout;
                    using (SqlDataReader r = command.ExecuteReader())
                    {
                        string name = string.Empty;
                        string db = string.Empty;
                        UInt32 id = 0;
                        UInt64 io = 0;
                        while (r.Read())
                        {
                            try
                            {
                                io = DataHelper.ToUInt64(r, "IO");
                                if (0 == io) continue;

                                id = DataHelper.ToUInt32(r, "ObjectID");
                                db = DataHelper.ToString(r, "Database");
                                name = string.Format("{0}.{1}.{2}", db, ssnh.GetSchemaName(id, db), ssnh.GetObjectName(id, db));

                                _lowIO = Math.Min(_lowIO, io);
                                _highIO = Math.Max(_highIO, io);
                                if (!_tableToIO.ContainsKey(name))
                                {
                                    _tableToIO.Add(name, io);
                                }
                            }
                            catch (Exception ex)
                            {
                                ExceptionLogger.Log(string.Format("TableRankingStats Add {0} {1} Exception: ", name, io), ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("TableRankingStats() Exception: ", ex);
            }
        }

        internal double GetPercentile(IProvideTableName r)
        {
            if (null == r) return (0);
            UInt64 io = 0;
            string name = string.Format("{0}.{1}.{2}", r.Database, r.Schema, r.Table);
            if (_tableToIO.TryGetValue(name, out io))
            {
                UInt64 high = _highIO - _lowIO;
                io -= _lowIO;
                if (high > 0)
                {
                    return ((io * 1.0) / high);
                }
            }
            return (0);
        }
    }
}
