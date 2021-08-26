using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.ManagementService.Configuration;

namespace Idera.SQLdm.ManagementService.Helpers
{
    public class BaselineTemplatesUpgradeHelper
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("BaselineTemplatesUpgradeHelper");
        public  bool   BaselineTemplatesNeedUpgrading = true;
        private bool   upgradeRunning = false;
        private Thread upgradeThread;

        private const string GetServersNeedingUpgrade = "select m.SQLServerID, RefRangeUseDefaults, RefRangeStartTimeUTC, RefRangeEndTimeUTC, RefRangeDays from MonitoredSQLServers m where m.SQLServerID not in (select distinct(SQLServerID) from BaselineTemplates)";

        internal void Start()
        {
            if (upgradeRunning)
                return;

            if (upgradeThread != null)
                Stop(TimeSpan.FromSeconds(5));

            upgradeRunning = true;

            upgradeThread              = new Thread(UpgradeBaselineTemplates);
            upgradeThread.Name         = "UpgradeBaselineTemplates";
            upgradeThread.IsBackground = true;
            upgradeThread.Priority     = ThreadPriority.BelowNormal;
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

        internal void UpgradeBaselineTemplates()
        {
            int       serverid    = 0;
            bool?     useDefaults = null;
            DateTime? start       = null;
            DateTime? end         = null;
            short?    days        = null;

            try
            {
                if (BaselineTemplatesNeedUpgrading == false)
                    return;
                //SQlDM-28022 - Handling connection object to avoid leakage and Ensure connection is closed
                // get the servers which don't have baseline templates
              using (SqlConnection connection = CachedObjectRepositoryConnectionFactory.GetRepositoryConnection())
              {
                  connection.Open();

                using (SqlDataReader reader = SqlHelper.ExecuteReader(connection, CommandType.Text, GetServersNeedingUpgrade))
                {
                    if (!reader.HasRows)
                    {
                        BaselineTemplatesNeedUpgrading = false;
                        upgradeRunning = false;
                        return;
                    }

                    while (reader.Read())
                    {
                        serverid = reader.GetInt32(0);

                        useDefaults = null;
                        start = null;
                        end   = null;
                        days  = null;

                        if(reader[1] != DBNull.Value)
                            useDefaults = (bool)reader[1];

                        if (reader[2] != DBNull.Value)
                            start = (DateTime)reader[2];

                        if (reader[3] != DBNull.Value)
                            end = (DateTime)reader[3];

                        if (reader[4] != DBNull.Value)
                            days = (short)((byte)reader[4]);

                        try
                        {
                            BaselineConfiguration config = new BaselineConfiguration(useDefaults, start, end, days, null, null, null);
                            RepositoryHelper.SaveBaselineTemplate(ManagementServiceConfiguration.ConnectionString, serverid, config);
                        }
                        catch (Exception ex)
                        {
                            LOG.Error("An error occurring upgrading baseline template for server "+ serverid, ex);                            
                        }
                    }

                    BaselineTemplatesNeedUpgrading = false;
                    upgradeRunning = false;
                }
              }
            }
            catch (Exception e)
            {
                LOG.Error("Error upgrading baseline templates.", e);
                upgradeRunning = false;
            }
        }
    }
}
