using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using props = Idera.SQLdm.PrescriptiveAnalyzer.Common.Properties;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Ranking
{
    internal class DatabaseRankingStats
    {
        private UInt64 _lowIO = UInt64.MaxValue;
        private UInt64 _highIO = UInt64.MinValue;

        private Dictionary<string, UInt64> _dbToIO = new Dictionary<string, UInt64>();

        private DatabaseRankingStats() { }
        public DatabaseRankingStats(SqlConnectionInfo info)
        {
            try
            {
                using (SqlConnection conn = SQLHelper.GetConnection(info))
                {
                    SqlCommand command = new SqlCommand(props.Resources.DatabaseRankingStats, conn);
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = Constants.DefaultCommandTimeout;
                    using (SqlDataReader r = command.ExecuteReader())
                    {
                        string db = string.Empty;
                        UInt64 io = 0;
                        while (r.Read())
                        {
                            try
                            {
                                db = DataHelper.ToString(r, "Database");
                                io = DataHelper.ToUInt64(r, "IO");
                                if (0 == io) continue;

                                _lowIO = Math.Min(_lowIO, io);
                                _highIO = Math.Max(_highIO, io);
                                _dbToIO.Add(db, io);
                            }
                            catch (Exception ex)
                            {
                                ExceptionLogger.Log(string.Format("DatabaseRankingStats Add {0}  {1} Exception: ", db, io), ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("DatabaseRankingStats() Exception: ", ex);
            }
        }

        internal double GetPercentile(IProvideDatabase r)
        {
            if (null == r) return (0);
            UInt64 io = 0;
            if (_dbToIO.TryGetValue(r.Database, out io))
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
