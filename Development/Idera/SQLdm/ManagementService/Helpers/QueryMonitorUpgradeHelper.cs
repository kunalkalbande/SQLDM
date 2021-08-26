using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Snapshots;
using Microsoft.ApplicationBlocks.Data;

namespace Idera.SQLdm.ManagementService.Helpers
{
    public class QueryMonitorUpgradeHelper
    {
        private const string UpgradeQueryMonitorProcedure = "p_GetQueryMonitorUpgradeData";
        private const string UpgradeQueryMonitorSaveProcedure = "p_MoveQueryMonitorStatement";


        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("RepositoryHelper");
        public bool QueryMonitorNeedsUpgrading = true;
        

        private bool upgradeRunning = false;
        private Thread upgradeThread;
        private bool inUpgradeWindow = true;

        internal void Start()
        {
            if (upgradeRunning)
                return;

            if (upgradeThread != null)
                Stop(TimeSpan.FromSeconds(5));

            upgradeRunning = true;
            inUpgradeWindow = true;

            upgradeThread = new Thread(UpgradeQueryMonitorData);
            upgradeThread.Name = "UpgradeQueryMonitorData";
            upgradeThread.IsBackground = true;
            upgradeThread.Priority = ThreadPriority.BelowNormal;
            upgradeThread.Start();
        }

        internal void Stop(TimeSpan timeout)
        {

            if (upgradeThread == null)
                return;

            upgradeThread.Interrupt();

            ThreadState state = upgradeThread.ThreadState;

            try
            {
                if (upgradeThread.Join(timeout))
                {
                    LOG.VerboseFormat("Background thread {0} has ended.", upgradeThread != null ? upgradeThread.Name : "Upgrade Thread");
                    upgradeThread = null;
                }
                else
                {
                    LOG.DebugFormat("Ran out of time waiting for thread {0} to end.", upgradeThread != null ? upgradeThread.Name : "Upgrade Thread");
                }
            }
            catch (ThreadInterruptedException)
            {
                LOG.DebugFormat("TIE waiting for thread {0} in {1} state to end.", upgradeThread != null ? upgradeThread.Name : "Upgrade Thread", state);
            }
        }

        internal void UpgradeQueryMonitorData()
        {
            try
            {
                if (QueryMonitorNeedsUpgrading == false)
                    return;

                using (SqlConnection connection = CachedObjectRepositoryConnectionFactory.GetRepositoryConnection())
                {
                    connection.Open();

                    while (inUpgradeWindow)
                    {
                        try
                        {
                            DataTable dt = new DataTable();

                            Dictionary<long, long> cachedSqlCopy = new Dictionary<long, long>();
                            Dictionary<long, long> cachedSigCopy = new Dictionary<long, long>();
                            Dictionary<long, long> cachedHostNameCopy = new Dictionary<long, long>();
                            Dictionary<long, long> cachedApplicationNameCopy = new Dictionary<long, long>();
                            Dictionary<long, long> cachedLoginNameCopy = new Dictionary<long, long>();

                            using (SqlCommand command = SqlHelper.CreateCommand(connection, UpgradeQueryMonitorProcedure))
                            {
                                SqlDataReader reader = command.ExecuteReader();

                                dt.Load(reader);
                            }

                            if (dt.Rows.Count == 0)
                            {
                                LOG.Info("No data for Query Monitor upgrade.");
                                QueryMonitorNeedsUpgrading = false;
                                upgradeRunning = false;
                                return;
                            }

                            if (dt.Columns.Count == 1)
                            {
                                LOG.Info("Query Monitor upgrade paused.");
                                inUpgradeWindow = false;
                                QueryMonitorNeedsUpgrading = true;
                                upgradeRunning = false;
                                return;
                            }

                            LOG.Info("Upgrading Query Monitor.");

                            using (
                                SqlCommand command = SqlHelper.CreateCommand(connection, UpgradeQueryMonitorSaveProcedure))
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    try
                                    {
                                        string LoginName = row["SqlUserName"].ToString();
                                        string StatementText = row["StatementText"].ToString();


                                        string SQLHash = SqlParsingHelper.GetStatementHash(StatementText);
                                        long SQLHashNumeric = SQLHash.GetHashCode();
                                        long SQLId = -1;
                                        cachedSqlCopy.TryGetValue(SQLHashNumeric, out SQLId);

                                        string signature = SqlParsingHelper.GetReadableSignature(StatementText);
                                        string signatureHash = SqlParsingHelper.GetSignatureHash(StatementText);
                                        long SQLSigId = -1;

                                        long signatureHashNumeric = signatureHash.GetHashCode();
                                        cachedSigCopy.TryGetValue(signatureHashNumeric, out SQLSigId);

                                        long applicationNameId = -1;
                                        long applicationNumeric = row["ApplicationName"].ToString().GetHashCode();
                                        cachedApplicationNameCopy.TryGetValue(applicationNumeric, out applicationNameId);

                                        long hostNameId = -1;
                                        long hostNameNumeric = row["ClientComputerName"].ToString().GetHashCode();
                                        cachedHostNameCopy.TryGetValue(hostNameNumeric, out hostNameId);

                                        long loginNameId = -1;
                                        long loginNameNumeric = LoginName.GetHashCode();
                                        cachedLoginNameCopy.TryGetValue(loginNameNumeric, out loginNameId);

                                        DateTime CompletionTime = DateTime.Parse(row["CompletionTime"].ToString());
                                        TimeSpan Duration =
                                            TimeSpan.FromMilliseconds(
                                                Convert.ToInt64(row["DurationMilliseconds"].ToString()));

                                        SqlHelper.AssignParameterValues(command.Parameters,
                                                                        row["SQLServerID"], //@SQLServerID int,
                                                                        row["UTCCollectionDateTime"],
                                                                        //@UTCCollectionDateTime datetime,
                                                                        applicationNameId > 0
                                                                            ? null
                                                                            : row["ApplicationName"],
                                                                        //@ApplicationName nvarchar(256),
                                                                        applicationNameId, //@ApplicationNameID int output,
                                                                        null, //@DatabaseName nvarchar(255),
                                                                        row["DatabaseID"], //@DatabaseID int output,
                                                                        hostNameId > 0 ? null : row["ClientComputerName"],
                                                                        //@HostName nvarchar(256),
                                                                        hostNameId, //@HostNameID int output,
                                                                        loginNameId > 0 ? null : row["SqlUserName"],
                                                                        //@LoginName nvarchar(256),                                           
                                                                        loginNameId, //@LoginNameID int output,
                                                                        row["Spid"], //@SessionID smallint,
                                                                        row["StatementType"], //@StatementType int,
                                                                        SQLId > 0 ? null : StatementText,
                                                                        //@SQLStatement varchar(4000),
                                                                        SQLHash, //@SQLStatementHash nvarchar(30),
                                                                        SQLId, //@SQLStatementID int output,
                                                                        SQLSigId > 0 ? null : signature,
                                                                        //@SQLSignature nvarchar(4000),
                                                                        signatureHash, //@SQLSignatureHash nvarchar(30),
                                                                        SQLSigId, //@SQLSignatureID int output,
                                                                        CompletionTime.Subtract(Duration), //@StatementUTCStartTime datetime,
                                                                        null,
                                                                        row["DurationMilliseconds"], //@DurationMilliseconds bigint,
                                                                        row["CPUMilliseconds"], //@CPUMilliseconds bigint,
                                                                        row["Reads"], //@Reads bigint,
                                                                        row["Writes"], //@Writes bigint,
                                                                        row["CompletionTime"] //@CompletionTime datetime
                                            );
                                        command.ExecuteNonQuery();
                                    }
                                    catch (Exception e)
                                    {
                                        LOG.Error("Error upgrading Query Monitor statement.", e);
                                        cachedSqlCopy.Clear();
                                        cachedSigCopy.Clear();
                                        cachedHostNameCopy.Clear();
                                        cachedApplicationNameCopy.Clear();
                                        cachedLoginNameCopy.Clear();
                                    }
                                    //SQlDM-28022 - Handling connection object to avoid leakage and Ensure No sleep for each row
                                    // Thread.Sleep(100);
                                }
                            }
                        }

                        catch (Exception e)
                        {
                            LOG.Error("Error upgrading Query Monitor data.  Stopping upgrade.", e);
                            upgradeRunning = false;
                            inUpgradeWindow = false;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                LOG.Error("Error upgrading Query Monitor data.", e);
                upgradeRunning = false;
            }
        }
    }
}
