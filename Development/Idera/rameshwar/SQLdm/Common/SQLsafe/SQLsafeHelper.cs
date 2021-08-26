using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Idera.SQLsafe.Shared;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.Configuration;
using System.Data.SqlClient;
using Idera.SQLsafe.Shared.Service.Backup;
using Microsoft.ApplicationBlocks.Data;
using Idera.SQLdm.Common.Objects;
using Logger = BBS.TracerX.Logger;


namespace Idera.SQLdm.Common.SQLsafe
{
    internal static class SQLsafeHelper
    {
        #region Constants

        private static readonly Logger LOG = Logger.GetLogger("RepositoryHelper");

        private const string GetBackupRestoreOperations = "dm_EnumBackupRestoreOperationHistory";
        private const string GetDefragOperations = "dm_EnumDefragOperationHistory";
        private const string GetSQLSafeVersionCommand = "select dbo.f_GetRepositoryVersion()";

        #endregion

        public static void CollectSSOperations(ScheduledRefresh refresh)
        {
            var result = refresh.SQLsafeOperations;
            var ms = refresh.MonitoredServer;
            var sqlSafeConnectionInfo = ms.SQLsafeConfig.SQLsafeConnectionInfo;

            LOG.InfoFormat("Starting SQLsafe operations collection for {0}", ms.InstanceName);

            short stage = 1;
            try
            {
                using (var conn = sqlSafeConnectionInfo.GetConnection())
                {
                    conn.Open();

                    stage = 2;
                    using(var cmd = new SqlCommand(GetBackupRestoreOperations, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@instanceId", ms.SQLsafeConfig.SQLsafeInstanceId);
                        cmd.Parameters.AddWithValue("@prvActionId", ms.SQLsafeConfig.LastBackupActionId);

                        int backups = 0, restores = 0, others = 0;

                        stage = 3;
                        using (var dataReader = cmd.ExecuteReader())
                        {
                            stage = 4;
                            while(dataReader.Read())
                            {
                                SQLsafeBackupRestoreOperation operation = new SQLsafeBackupRestoreOperation(dataReader);
                            
                                switch (operation.OperationType)
                                {
                                    case OperationType.Backup:
                                        backups++;
                                        break;
                                    case OperationType.Restore:
                                        restores++;
                                        break;
                                    default:
                                        others++;
                                        break;
                                }

                                result.Add(operation);
                                LOG.InfoFormat("{0} Backups, {1} Restores and {2} other SQLsafe operations collected for {3}", backups, restores, others, ms.InstanceName);
                            }
                        }
                    }

                    var defragList = new Dictionary<int, SQLsafeDefragOperation>();

                    stage = 5;
                    using (var cmd = new SqlCommand(GetDefragOperations, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@instanceId", ms.SQLsafeConfig.SQLsafeInstanceId);
                        cmd.Parameters.AddWithValue("@prvActionId", ms.SQLsafeConfig.LastBackupActionId);

                        int operations = 0;

                        stage = 6;
                        using (var dataReader = cmd.ExecuteReader())
                        {
                            stage = 7;
                            while(dataReader.Read())
                            {
                                var operation = new SQLsafeDefragOperation(dataReader);
                                defragList.Add(operation.OperationId, operation);
                            }

                            stage = 8;
                            dataReader.NextResult();
                            while (dataReader.Read())
                            {
                                var stats = new DefragStats(dataReader);
                                defragList[stats.OperationId].DefragStats.Add(stats);
                            }
                        }

                        stage = 9;
                        foreach(var operation in defragList.Values)
                        {
                            result.Add(operation);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                LOG.ErrorFormat("Error collecting SQLsafe Operations at stage {0}: ",stage, e);
            }
        }


        public static void TestSQLsafeConnection(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                LOG.Error("Testing SQLsafe connection with NULL connectionInfo");
                throw new ArgumentNullException("connectionInfo");
            }

            if (string.IsNullOrEmpty(connectionInfo.ConnectionString))
            {
                LOG.Error("SQLsafe Connection String is NULL or Empty");
                throw new SQLsafeApplicationException("Empty SQLsafe connection string");
            }

            SqlConnection connection = null;
            try
            {
                using(connection = connectionInfo.GetConnection())
                {
                    if (!IsSupportedSQLsafeVersion(connection))
                    {
                        throw new SQLsafeApplicationException("Unsupported version of SQLSafe");
                    }

                }
                LOG.Debug("Supported version of SQLsafe detected");
            }
            catch (Exception)
            {
                LOG.Warn("Error occurred testing the SQLsafe repository connection");
                throw;
            }
            finally
            {
                if (connection != null)
                    connection.Dispose();
            }
        }

        public static bool IsSupportedSQLsafeVersion(SqlConnection sqlSafeConnection)
        {
            try
            {
                SqlSafeVersion sqlSafeVersion = new SqlSafeVersion();

                sqlSafeVersion.Version = (string) SqlHelper.ExecuteScalar(sqlSafeConnection, CommandType.Text, GetSQLSafeVersionCommand);
                LOG.InfoFormat("SQLsafe Version [{0}] detected", sqlSafeVersion.DisplayVersion);

                return sqlSafeVersion.IsSupported;
            }
            catch (SqlException e)
            {
                LOG.Error("Error checking SQLsafe version: ", e);
                return false;
            }

        }

    }
}
