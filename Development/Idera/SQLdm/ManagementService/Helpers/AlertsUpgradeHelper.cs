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
    public class AlertsUpgradeHelper
    {
        //private const string UpgradeQueryMonitorProcedure = "p_GetQueryMonitorUpgradeData";
        //private const string UpgradeQueryMonitorSaveProcedure = "p_MoveQueryMonitorStatement";
        private const string CheckLeftToDo = "select Name, Internal_Value from RepositoryInfo where Name in ('AlertsLeftToDo','SALeftToDo','TasksLeftToDo')";
        private const string UpgradeAlertsProcedure = "p_UpgradeAlertData";


        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("RepositoryHelper");
        public bool AlertsNeedUpgrading = true;

        private bool upgradeRunning = false;
        private Thread upgradeThread;

        internal void Start()
        {
            if (upgradeRunning)
                return;

            if (upgradeThread != null)
                Stop(TimeSpan.FromSeconds(5));

            upgradeRunning = true;

            upgradeThread = new Thread(UpgradeInformationalAlerts);
            upgradeThread.Name = "UpgradeInformationalAlerts";
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

        internal void UpgradeInformationalAlerts()
        {
            try
            {
                if (AlertsNeedUpgrading == false)
                    return;

                int errorCount = 0;
                //SQlDM-28022 - Handling connection object to avoid leakage and Ensure connection is closed
                using (SqlConnection connection = CachedObjectRepositoryConnectionFactory.GetRepositoryConnection())
                {
                    connection.Open();

                    while (AlertsNeedUpgrading)
                    {
                        try
                        {
                            int AlertsToDo = 0,
                                SAToDo = 0,
                                TasksToDo = 0;

                            // upgrade logic here
                            using (
                                SqlDataReader reader = SqlHelper.ExecuteReader(connection, CommandType.Text,
                                    CheckLeftToDo))
                            {
                                if (!reader.HasRows)
                                {
                                    AlertsNeedUpgrading = false;
                                    upgradeRunning = false;
                                    return;
                                }

                                while (reader.Read())
                                {
                                    switch (reader.GetString(0).Trim().ToLower())
                                    {
                                        case "alertslefttodo":
                                            AlertsToDo = reader.GetInt32(1);
                                            break;
                                        case "salefttodo":
                                            SAToDo = reader.GetInt32(1);
                                            break;
                                        case "taskslefttodo":
                                            TasksToDo = reader.GetInt32(1);
                                            break;
                                    }
                                }
                            }

                            if (AlertsToDo == 0 && SAToDo == 0 && TasksToDo == 0)
                            {
                                AlertsNeedUpgrading = false;
                                upgradeRunning = false;
                                return;
                            }

                            using (SqlCommand command = SqlHelper.CreateCommand(connection, UpgradeAlertsProcedure))
                            {
                                try
                                {
                                    command.ExecuteNonQuery();
                                }
                                catch (Exception e)
                                {
                                    errorCount++;
                                    if (errorCount <= 5)
                                    {
                                        LOG.WarnFormat(
                                            "An error occurred while upgrading informational alert related data.  Will try {0} more times.",
                                            (5 - errorCount), e);
                                    }
                                    else
                                    {
                                        LOG.Error(
                                            "Error calling proc to upgrade informational alert related data.  Ran out of retry attempts.  Contact Tech Support for further assistance.",
                                            e);
                                        AlertsNeedUpgrading = false;
                                    }
                                }
                                // Commenting Thread Sleep
                                // Thread.Sleep(180);
                            }
                        }
                        catch (Exception e)
                        {
                            LOG.Error("Error upgrading informational alerts.  Stopping upgrade.", e);
                            upgradeRunning = false;
                            AlertsNeedUpgrading = false;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                LOG.Error("Error upgrading informational alerts.", e);
                upgradeRunning = false;
            }
        }

    }
}
