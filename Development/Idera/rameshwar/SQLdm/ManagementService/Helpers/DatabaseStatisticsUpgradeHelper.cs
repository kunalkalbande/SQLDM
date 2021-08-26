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
    public class DatabaseStatisticsUpgradeHelper: IDisposable

{
    private const string UpgradeDatabaseStatisticsStoredProcedure = "p_UpgradeDatabaseStatistics";


    private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("DatabaseStatisticsUpgradeHelper");
    public bool DBStatsNeedUpgrading = true;
    private const int DB_UPGRADE_LOOP_MS = 1000;

    private bool upgradeRunning = false;
    private SqlCommand command = null;
    private SqlConnection conn = null;
    private Timer timer;
    private object syncRoot = new object();
    private long rowsRemaining = -1;


    public bool UpgradeRunning { get { return upgradeRunning; } }
    public long RowsRemaining { get { return rowsRemaining; } }

    internal void Start()
    {
        lock (syncRoot)
        {
            if (DBStatsNeedUpgrading == false)
                return;

            if (upgradeRunning)
                return;

            if (timer != null)
                return;

            upgradeRunning = true;

            // Tolga K - to fix memory leak begins
            // if (timer != null)
            // {
            //    timer.Dispose();
            // }
            // Tolga K - to fix memory leak ends

            timer = new Timer(new TimerCallback(UpgradeDatabaseStatistics), null, DB_UPGRADE_LOOP_MS , Timeout.Infinite);
        }

    }

    internal void Stop()
    {
        if (timer != null)
            timer.Dispose();
        timer = null;

        if (command != null)
        {   
            command.Cancel();
            command.Dispose();
            command = null;
        }

        if (conn != null)
        {
            conn.Close();
            conn.Dispose();
            conn = null;
        }

        upgradeRunning = false;
    }

    internal void ResetForException()
    {
        Stop();
        // If we're in a tight loop of exceptions we don't want to cause CPU churn
        Thread.Sleep(10000);
    }

    internal void UpgradeDatabaseStatistics(object state)
    {
        using (LOG.DebugCall("DatabaseStatisticsUpgradeCallback"))
        {
            try
            {

                if (DBStatsNeedUpgrading == false)
                    return;

                if (conn == null)
                    conn = CachedObjectRepositoryConnectionFactory.GetRepositoryConnection();

                if (conn.State != ConnectionState.Open)
                    conn.Open();

                try
                {
                    if (command == null)
                        command = SqlHelper.CreateCommand(conn, UpgradeDatabaseStatisticsStoredProcedure);

                    SqlHelper.AssignParameterValues(command.Parameters, null);
                    command.CommandTimeout = 600; // 10 minute timeout 
                    IAsyncResult queryResult = command.BeginExecuteNonQuery(new AsyncCallback(DatabaseStatisticsUpgradeCallback), this);

                }
                catch (Exception e)
                {
                    LOG.Error("Error upgrading database statistics", e);
                    ResetForException();
                    Start();
                }
            }
            catch (Exception e)
            {
                LOG.Error("Error upgrading database statistics", e);
                ResetForException();
                Start();
            }
        }
    }

    private void DatabaseStatisticsUpgradeCallback(IAsyncResult ar)
    {
        using (LOG.DebugCall("DatabaseStatisticsUpgradeCallback"))
        {
            try
            {
                lock (syncRoot)
                {

                    command.EndExecuteNonQuery(ar);
                    rowsRemaining = (long)command.Parameters["@RowsRemaining"].Value;
                    if (rowsRemaining > 0)
                    {
                        DBStatsNeedUpgrading = true;
                    }
                    else
                    {
                        DBStatsNeedUpgrading = false;
                    }


                    upgradeRunning = false;
                    if (timer != null)
                        timer.Dispose();
                    timer = null;
                }

            }
            catch (Exception e)
            {
                LOG.Error("Error upgrading database statistics", e);
                ResetForException();
            }
            finally
            {
                Start();
            }
        }
    }

        public void Dispose()
        {
            Stop();
        }
}
}
