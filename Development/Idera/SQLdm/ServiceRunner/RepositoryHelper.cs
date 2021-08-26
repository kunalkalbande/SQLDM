using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdm.Common.Configuration;
using PluginCommon;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Idera.SQLdm.Service.Runner
{
    public class RepositoryHelper
    {
        private static RepositoryHelper _instance;
        private ConnectionCredentials _dashboardConnInfo = null;
        private string _ideraDashboardConnectionString = string.Empty;

        private RepositoryHelper(ConnectionCredentials connInfo) 
        {
            string[] locationParams = connInfo.Location.Split(';');
            string serverName = locationParams.Count() > 0 ? locationParams[0] : string.Empty;
            string databaseName = locationParams.Count() > 0  ? locationParams[1] : string.Empty;

            _dashboardConnInfo = connInfo;
            _ideraDashboardConnectionString = "Data Source=" + serverName + ";Initial Catalog=" + databaseName + ";Integrated Security=SSPI;User ID="+ connInfo.ConnectionUser +";Password=" + connInfo.ConnectionPassword + ";";
        }

        public static RepositoryHelper GetHelper(ConnectionCredentials connInfo) 
        {
            RepositoryHelper _localInstance = null;
            if (_instance != null)
            {
                lock (_instance)
                {
                    _localInstance = _instance;
                }
            }else
            {
                _localInstance = _instance = new RepositoryHelper(connInfo);
            }
            return _localInstance;
        }

        public ConnectionCredentials GetConnectionCredentialsOfProductInstance(int instanceId) 
        {

            SqlConnection conn = null;
            ConnectionCredentials sqldmConnCredentials = new ConnectionCredentials();
            try
            {
                conn = new SqlConnection(_ideraDashboardConnectionString);
                SqlCommand command = new SqlCommand();
                command.CommandText = @"Common.p_GetConnectionCredentialsOfInstance";
                command.Connection = conn;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@InstanceID", instanceId);
                conn.Open();
                SqlDataReader dr = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                
              
                while (dr.Read()) 
                {
                    int ordinal = dr.GetOrdinal("Location");
                    if (!dr.IsDBNull(ordinal)) 
                    {
                        sqldmConnCredentials.Location = (string) dr[ordinal];
                    }

                    ordinal = dr.GetOrdinal("ConnectionUser");
                    if (!dr.IsDBNull(ordinal))
                    {
                        sqldmConnCredentials.ConnectionUser = (string)dr[ordinal];
                    }

                    ordinal = dr.GetOrdinal("ConnectionPassword");
                    if (!dr.IsDBNull(ordinal))
                    {
                        sqldmConnCredentials.ConnectionPassword = (string)dr[ordinal];
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally 
            {
                if (conn.State != System.Data.ConnectionState.Closed)
                {
                    conn.Close();
                }

                conn.Dispose();
            }

            return sqldmConnCredentials;
        }

    }
}
