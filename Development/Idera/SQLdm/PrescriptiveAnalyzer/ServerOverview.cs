using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Batches;
using System.Globalization;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer
{
    [Serializable]
    public class ServerOverview //: IServerOverview
    {
        private static Logger _logX = Logger.GetLogger("ServerOverview");

        public ServerVersion ServerVersion { get; private set; }
        public bool OleEnabled { get; private set; }
        public bool IsSysAdmin { get; private set; }
        public DateTime StartupDate { get; private set; }
        public IList<string> Databases { get; private set; }

        public ServerOverview(SqlConnection conn, ServerVersion ver)
        {
            Load(conn, ver);
        }

        public ServerOverview(SqlConnectionInfo connInfo) 
        {
            using (var conn = SQLHelper.GetConnection(connInfo))
            {
                Load(conn, connInfo.GetProductVersion(conn));
            }
        }

        public TimeSpan UpTime 
        {
            get
            {
                if ((DateTime.MinValue == StartupDate) || (DateTime.MaxValue == StartupDate))
                {
                    return (TimeSpan.MinValue);
                }
                return (DateTime.Now - StartupDate);
            }
        }

        private void Load(SqlConnection conn, ServerVersion ver)
        {
            using (_logX.InfoCall(string.Format("Load({0}, {1}, {2}, {3})", conn.DataSource, conn.Database, conn.ServerVersion, ver)))
            {
                using (var command = new SqlCommand(BatchFinder.GetBatch("ServerOverview", ver), conn))
                {
                    command.CommandType = CommandType.Text;
                    command.CommandTimeout = BatchConstants.DefaultCommandTimeout;

                    using (var reader = command.ExecuteReader())
                    {
                        Load(reader);
                    }
                }
            }
        }

        private void Load(SqlDataReader reader)
        {
            using (_logX.InfoCall("Load(SqlDataReader reader)"))
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) || DataHelper.IsNull(reader[0])) continue;

                    var name = reader.GetString(0).ToLower();
                    _logX.InfoFormat("{0}: {1}", DataHelper.ToString(reader, 0), DataHelper.ToString(reader, 1));

                    switch (name)
                    {
                        case "productversion":
                            ServerVersion = new ServerVersion(reader[1] as string);

                            if (ServerVersion.Major < 9)
                            {
                                // We don't support versions older than SQL 2005, so exit
                                return;
                            }
                            break;
                        case "oleenabled":
                            OleEnabled = DataHelper.ToBoolean(reader, 1);
                            break;
                        case "issysadmin":
                            IsSysAdmin = DataHelper.ToBoolean(reader, 1);
                            break;
                        case "startupdate":
                            //StartupDate = DataHelper.ToDateTime(reader, 1, CultureInfo.CreateSpecificCulture("en-US"));
                            break;
                    }
                }

                // Move on the database list
                reader.NextResult();
                var databases = new List<string>();

                while (reader.Read())
                {
                    databases.Add(reader.GetString(0));
                }

                Databases = databases;
            }
        }
    }
}
