//------------------------------------------------------------------------------
// <copyright file="SqlCommandBuilder.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
// Change Log   ----------------------------------------------------------------
// Modified By          :   Pruthviraj Nikam
// Modification ID      :   M1
// Date                 :   31-Jan-2019
// Description          :   Done changes for New Azure SQL DB Alerts.
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.CollectionService.Configuration;
using Idera.SQLdm.CollectionService.Helpers;
using Idera.SQLdm.CollectionService.Monitoring;
using Idera.SQLdm.CollectionService.Probes.Sql.Batches;
using Idera.SQLdm.CollectionService.Probes.Wmi;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.Thresholds;
using Microsoft.ApplicationBlocks.Data;
using Wintellect.PowerCollections;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    /// <summary>
    /// This class contains the information necessary to build SQL batches for each probe
    /// </summary>
    internal static class SqlCommandBuilder
    {
        /// <summary>
        /// We will read query monitor data every 50 events
        /// </summary>
        internal const int QueryMonitorReadEventCount2012Ex = 50;
        internal const int ActivityMonitorReadEventCount2012Ex = 23;

        #region fields

        private static Logger LOG = Logger.GetLogger("SqlCommandBuilder");
        private static int AMAZON_CLOUD_ID = 1;
        private static int AZURE_CLOUD_ID = 2;
        private static int AZURE_MANAGED_CLOUD_ID = 5;


        #endregion

        #region constructors

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        #region Server Actions
        /// <summary>
        /// 10.0 -- doctor integration
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="connection"></param>
        /// <param name="prescriptiveScriptConfiguration"></param>
        /// <returns></returns>
        internal static List<SqlCommand> BuildPrescriptiveOptimizeCommand(SqlConnectionInfo sqlConnection, SqlConnection connection, IRecommendation recommendation, bool isUndo)
        {
            List<SqlCommand> lstCommands = new List<SqlCommand>();
            List<StringBuilder> batches = new List<StringBuilder>();
            string tsql = string.Empty;
            if (!isUndo)
            {
                try
                {
                    tsql = GetPrescriptiveOptimizeScript(sqlConnection, recommendation);
                    batches.AddRange(Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Helpers.PASQLHelper.SqlParser(tsql, "GO"));

                }
                catch (Exception ex)
                {
                    LOG.Error("Optimization Exception in getting scripts to run. ", ex);
                }
            }
            else
            {
                try
                {
                    tsql = GetPrescriptiveUndoScript(sqlConnection, recommendation);

                    batches.AddRange(Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Helpers.PASQLHelper.SqlParser(tsql, "GO"));
                }
                catch (Exception ex)
                {
                    LOG.Error("Undo Optimization Exception in getting scripts to run. ", ex);
                }
            }
            foreach (var batch in batches)
            {
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = batch.ToString();
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = SqlHelper.CommandTimeout;
                lstCommands.Add(cmd);
            }

            return lstCommands;
        }


        internal static string GetPrescriptiveOptimizeScript(SqlConnectionInfo sqlConnection, Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation)
        {
            string tsql = string.Empty;
            if (recommendation is IScriptGeneratorProvider)
            {
                IScriptGenerator generator = ((IScriptGeneratorProvider)recommendation).GetScriptGenerator();
                PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration.PASqlConnectionInfo con;
                if (sqlConnection.UseIntegratedSecurity)
                    con = new PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration.PASqlConnectionInfo(sqlConnection.InstanceName, sqlConnection.DatabaseName);
                else
                    con = new PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration.PASqlConnectionInfo(sqlConnection.InstanceName, sqlConnection.UserName, sqlConnection.Password);
                tsql = generator.GetTSqlFix(con);
            }
            return tsql;
        }

        internal static List<string> GetPrescriptiveOptimizeMessages(SqlConnectionInfo sqlConnection, Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation)
        {
            List<string> recommendationsWithMessages = new List<string>();
            if (recommendation is IMessageGenerator)
            {
                List<string> messages = ((IMessageGenerator)recommendation).GetMessages(recommendation.OptimizationStatus, sqlConnection.GetConnection());
                if (messages.Count > 0)
                {
                    recommendationsWithMessages.AddRange(messages);
                }
            }
            return recommendationsWithMessages;
        }

        internal static List<string> GetPrescriptiveUndoMessages(SqlConnectionInfo sqlConnection, Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation)
        {
            List<string> recommendationsWithMessages = new List<string>();
            if (recommendation is IUndoMessageGenerator)
            {
                List<string> messages = ((IUndoMessageGenerator)recommendation).GetUndoMessages(recommendation.OptimizationStatus, sqlConnection.GetConnection());
                if (messages.Count > 0)
                {
                    recommendationsWithMessages.AddRange(messages);
                }
            }
            return recommendationsWithMessages;
        }

        internal static SqlCommand BuildAWSServicesCommand(SqlConnection connection)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += "select * from sys.dm_server_services";
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;

        }

        internal static string GetPrescriptiveUndoScript(SqlConnectionInfo sqlConnection, Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation recommendation)
        {
            string tsql = string.Empty;
            if (recommendation is Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IUndoScriptGeneratorProvider)
            {
                Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IUndoScriptGenerator generator
                    = ((Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IUndoScriptGeneratorProvider)recommendation).GetUndoScriptGenerator();
                PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration.PASqlConnectionInfo con;
                if (sqlConnection.UseIntegratedSecurity)
                    con = new PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration.PASqlConnectionInfo(sqlConnection.InstanceName, sqlConnection.DatabaseName);
                else
                    con = new PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration.PASqlConnectionInfo(sqlConnection.InstanceName, sqlConnection.UserName, sqlConnection.Password);
                tsql = generator.GetTSqlUndo(con);
            }
            return tsql;
        }

        /// <summary>
        /// 10.0 vineet -- doctor integration
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="connection"></param>
        /// <param name="prescriptiveScriptConfiguration"></param>
        /// <returns></returns>
        internal static SqlCommand BuildPrescriptiveUndoCommand(SqlConnectionInfo sqlConnection, SqlConnection connection, PrescriptiveScriptConfiguration prescriptiveScriptConfiguration)
        {
            SqlCommand cmd = connection.CreateCommand();
            var recommendation = prescriptiveScriptConfiguration.Recommendation;
            if (recommendation is Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IUndoScriptGeneratorProvider)
            {
                Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IUndoScriptGenerator generator
                    = ((Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IUndoScriptGeneratorProvider)recommendation).GetUndoScriptGenerator();
                PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration.PASqlConnectionInfo con;
                if (sqlConnection.UseIntegratedSecurity)
                    con = new PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration.PASqlConnectionInfo(sqlConnection.InstanceName, sqlConnection.DatabaseName);
                else
                    con = new PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration.PASqlConnectionInfo(sqlConnection.InstanceName, sqlConnection.UserName, sqlConnection.Password);
                string tsql = generator.GetTSqlUndo(con);
                cmd.CommandText = tsql;
            }
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildFreeProcedureCacheCommand(SqlConnection connection, ServerVersion ver)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchConstants.FreeProcCache;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildFullTextActionCommand(SqlConnection connection, ServerVersion ver,
                                                              FullTextActionConfiguration config)
        {
            SqlCommand cmd = connection.CreateCommand();
            switch (config.Action)
            {
                case FullTextActionConfiguration.FullTextAction.Optimize:
                    cmd.CommandText +=
                        String.Format(BatchConstants.FullTextOptimize,
                                      config.DatabaseName != null ? config.DatabaseName.Replace("]", "]]") : "",
                                      config.Catalogname != null
                                          ? config.Catalogname.Replace("]", "]]")
                                          : "");
                    break;
                case FullTextActionConfiguration.FullTextAction.Rebuild:
                    cmd.CommandText += String.Format(BatchConstants.FullTextAction,
                                                     config.DatabaseName != null
                                                         ? config.DatabaseName.Replace("]", "]]")
                                                         : "",
                                                     config.Catalogname != null
                                                         ? config.Catalogname.Replace("'", "''")
                                                         : "",
                                                     BatchConstants.FullTextRebuild);
                    break;
                case FullTextActionConfiguration.FullTextAction.Repopulate:
                    cmd.CommandText += String.Format(BatchConstants.FullTextAction,
                                                     config.DatabaseName != null
                                                         ? config.DatabaseName.Replace("]", "]]")
                                                         : "",
                                                     config.Catalogname != null
                                                         ? config.Catalogname.Replace("'", "''")
                                                         : "",
                                                     BatchConstants.FullTextRepopulate);

                    break;
            }
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildJobAlertsCommand(SqlConnection connection, ServerVersion ver, string realServerName, ScheduledRefresh previousRefresh, MonitoredServerWorkload workload)
        {
            SqlCommand cmd = connection.CreateCommand();
            string bombedJobs = workload.GetMetricThresholdEnabled(Metric.BombedJobs)
                        ? BuildBombedJobs(ver, workload) : "";
            string completedJobs = workload.GetMetricThresholdEnabled(Metric.JobCompletion) ? BuildCompletedJobs(ver, workload, previousRefresh) : "";
            string longJobs = workload.GetMetricThresholdEnabled(Metric.LongJobs) ||
                              workload.GetMetricThresholdEnabled(Metric.LongJobsMinutes)
                                  ? BuildLongJobs(ver, workload, workload.MonitoredServer.CloudProviderId)
                                  : "";
            cmd.CommandText += BatchConstants.CopyrightNotice + BatchConstants.BatchHeader + bombedJobs + completedJobs + longJobs;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildJobControlCommand(SqlConnection connection, ServerVersion ver,
                                                          JobControlConfiguration config)
        {
            SqlCommand cmd = connection.CreateCommand();

            switch (config.Action)
            {
                case JobControlAction.Start:
                    string sql =
                        String.Format(BatchConstants.StartAgentJob,
                                      config.JobName != null ? config.JobName.Replace("'", "''") : "");
                    if (!String.IsNullOrEmpty(config.JobStep))
                        sql += String.Format(",@step_name='{0}'", config.JobStep.Replace("'", "''"));
                    cmd.CommandText = sql;
                    break;
                case JobControlAction.Stop:
                    cmd.CommandText +=
                        String.Format(BatchConstants.StopAgentJob,
                                      config.JobName != null ? config.JobName.Replace("'", "''") : "");
                    break;
            }
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildKillSessionCommand(SqlConnection connection, ServerVersion ver,
                                                           KillSessionConfiguration config)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += String.Format(BatchConstants.KillSession, config.KillSpid);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildReConfigurationCommand(SqlConnection connection, ServerVersion ver,
                                                               ReconfigurationConfiguration config, int? cloudProviderId)
        {
            SqlCommand cmd = null;

            if (config != null && config.ConfigurationName != null && config.RunValue.HasValue)
            {
                cmd = connection.CreateCommand();
                cmd.CommandText = BatchConstants.CopyrightNotice + BatchConstants.BatchHeader;
                if(cloudProviderId != Constants.MicrosoftAzureId)
                    cmd.CommandText += String.Format(BatchConstants.Reconfigure, config.ConfigurationName, config.RunValue);
                cmd.CommandText += BatchFinder.Configuration(ver, cloudProviderId);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = SqlHelper.CommandTimeout;
            }

            return cmd;
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        internal static SqlCommand BuildBlockedProcessThresholdChangeCommand(SqlConnection connection, ServerVersion ver,
                                                               BlockedProcessThresholdConfiguration config, int? cloudProviderId)
        {
            SqlCommand cmd = null;

            config.ConfigurationName = ver.Major >= 10 ? "blocked process threshold (s)" : "blocked process threshold";

            if (config != null && config.ConfigurationName != null && config.RunValue.HasValue)
            {
                cmd = connection.CreateCommand();
                cmd.CommandText = BatchConstants.CopyrightNotice + BatchConstants.BatchHeader;

                if (ver.Major > 8) //SQLdm 9.0 (Tarun Sapra) -- Fixing rally defect DE42595. Bypassing running of sp_configure(Reconfigure in this case) when server is SQLServer2000 or below
                {
                    if (cloudProviderId.HasValue && cloudProviderId.Value == 3)
                    {
                        cmd.CommandText += BatchConstants.AdvancedOptionsHeader;
                    }
                    if (cloudProviderId != Constants.MicrosoftAzureId)
                        cmd.CommandText += String.Format(BatchConstants.Reconfigure, config.ConfigurationName, config.RunValue);
                }

                cmd.CommandText += BatchFinder.Configuration(ver, cloudProviderId);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = SqlHelper.CommandTimeout;
            }

            return cmd;
        }
        internal static SqlCommand BuildServiceControlCommand(SqlConnection connection, ServerVersion ver,
                                                              ServiceControlConfiguration config, string serverName)
        {
            string serviceControlAction = "";
            switch (config.Action)
            {
                case ServiceControlConfiguration.ServiceControlAction.Continue:
                    serviceControlAction = BatchConstants.ServiceContinue;
                    break;
                case ServiceControlConfiguration.ServiceControlAction.Pause:
                    serviceControlAction = BatchConstants.ServicePause;
                    break;
                case ServiceControlConfiguration.ServiceControlAction.Start:
                    serviceControlAction = BatchConstants.ServiceStart;
                    break;
                case ServiceControlConfiguration.ServiceControlAction.Stop:
                    serviceControlAction = BatchConstants.ServiceStop;
                    break;
            }
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchConstants.CopyrightNotice + BatchConstants.BatchHeader;
            cmd.CommandText +=
                String.Format(BatchConstants.ServiceControlCommand,
                              serviceControlAction,
                              ProbeHelpers.GetServiceString(config.ServiceToAffect, ver, serverName).Replace("'", "''"));
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildRecycleLogCommand(SqlConnection connection, ServerVersion ver)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchConstants.RecycleLog;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildRecycleAgentLogCommand(SqlConnection connection, ServerVersion ver)
        {
            SqlCommand cmd = connection.CreateCommand();
            // Only available for SQL 2005/2008
            if (ver.Major >= 9)
            {
                cmd.CommandText += BatchConstants.RecycleAgentLog;
            }
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildReindexCommand(SqlConnection connection, ServerVersion ver,
                                                       ReindexConfiguration config, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText +=
                String.Format(BatchFinder.Reindex(ver, cloudProviderId),
                              config.DatabaseName != null ? config.DatabaseName.Replace("]", "]]") : "",
                              config.TableId,
                              config.IndexName != null ? config.IndexName.Replace("'", "''") : "");
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildServerShutdownCommand(SqlConnection connection, ServerVersion ver,
                                                              bool withNoWait)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += withNoWait
                                   ? BatchConstants.ShutdownSQLServerWithNoWait
                                   : BatchConstants.ShutdownSQLServer;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildSetNumberOfLogsCommand(SqlConnection connection, ServerVersion ver,
                                                               SetNumberOfLogsConfiguration config)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (config.SetUnlimited)
            {
                cmd.CommandText += BatchConstants.SetUnlimitedLogs;
            }
            else
            {
                if (config.NumberOfLogs.HasValue)
                {
                    cmd.CommandText += String.Format(BatchConstants.SetLimitedLogs, config.NumberOfLogs.Value);
                }
            }

            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        internal static SqlCommand BuildStopSessionDetailsTraceCommand(SqlConnection connection, ServerVersion ver,
                                                                       StopSessionDetailsTraceConfiguration config, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += String.Format(BatchFinder.StopSessionDetailsTrace(ver, cloudProviderId), config.ClientSessionId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildUpdateStatistcisCommand(SqlConnection connection, ServerVersion ver,
                                                                UpdateStatisticsConfiguration config, int? cloudProviderId)
        {
            // SQLdm Minimum Privileges - Varun Chopra - Same conditions are used in Probe Permission Helper Validations checks
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText +=
                String.Format(BatchFinder.UpdateStatistics(ver, cloudProviderId),
                              config.DatabaseName != null ? config.DatabaseName.Replace("]", "]]") : "",
                              config.TableId,
                              config.IndexName != null ? config.IndexName.Replace("'", "''") : "",
                              ver.Major >= 9 ? "schema_name" : "user_name");
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        /// <summary>
        /// Build Update Statistics Permissions Batch - Requires Database name and Table Id
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ver"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        internal static SqlCommand BuildUpdateStatistcisPermissionsCommand(SqlConnection connection, ServerVersion ver,
            UpdateStatisticsConfiguration config, int? cloudProviderId)
        {
            // SQLdm Minimum Privileges - Varun Chopra - Same conditions are used in Server Action Probe for Update Statistics
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText +=
                String.Format(BatchFinder.UpdateStatisticsPermissions(ver, cloudProviderId),
                    config.DatabaseName != null ? config.DatabaseName.Replace("]", "]]") : "",
                    config.TableId,
                    ver.Major >= 9 ? "schema_name" : "user_name");
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        
        internal static SqlCommand BuildReindexPermissionsCommand(SqlConnection connection, ServerVersion ver,
         ReindexConfiguration config, int? cloudProviderId)
        {
            // SQLdm Minimum Privileges - Varun Chopra - Same conditions are used in Server Action Probe for Reindex
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText +=
                String.Format(BatchFinder.ReindexPermissions(ver, cloudProviderId),
                    config.DatabaseName != null ? config.DatabaseName.Replace("]", "]]") : "",
                    config.TableId,
                    config.IndexName != null ? config.IndexName.Replace("'", "''") : "");
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        /// <summary>
        /// Build Fragmentation Permissions Batch - Requires Database name and Table Id
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ver"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        internal static SqlCommand BuildFragmentationPermissionsCommand(SqlConnection connection, ServerVersion ver, TableFragmentationConfiguration config, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandTimeout = CollectionServiceConfiguration.GetCollectionServiceElement().FragmentationSqlCommandTimeout > SqlHelper.CommandTimeout ? CollectionServiceConfiguration.GetCollectionServiceElement().FragmentationSqlCommandTimeout : SqlHelper.CommandTimeout;

            int rowcountLimit = 500;
            int timeout = (int)((cmd.CommandTimeout > 90) ? (cmd.CommandTimeout - 60) : (cmd.CommandTimeout * .90));

            cmd.CommandText =
                String.Format(BatchFinder.FragmentationPermissions(ver, cloudProviderId),
                    rowcountLimit,
                    timeout,
                    config.Order);
            cmd.CommandType = CommandType.Text;
            return cmd;
        }
        
        internal static SqlCommand BuildMirroringPartnerActionPermissionsCommand(SqlConnection connection, ServerVersion ver,
            MirroringPartnerActionConfiguration config, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText +=
                String.Format(BatchFinder.MirroringPartnerActionPermissions(ver, cloudProviderId),
                    config.Database != null ? config.Database.Replace("]", "]]") : "");
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildMirroringPartnerActionCommand(SqlConnection connection, ServerVersion ver,
                                                                      MirroringPartnerActionConfiguration config)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText +=
                String.Format(BatchFinder.MirroringPartnerAction(ver),
                              config.Database != null ? config.Database.Replace("]", "]]") : "",
                              config.Action);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        #endregion

        #region Collectors

        //internal static SqlCommand BuildActivityProfilerCommand(SqlConnection connection, ServerVersion ver, ActivityProfilerConfiguration config)
        //{
        //    StringBuilder SQLTextPredicate = new StringBuilder();
        //    StringBuilder AppNamePredicate = new StringBuilder();
        //    StringBuilder DbNamePredicate = new StringBuilder();

        //    SqlCommand cmd = connection.CreateCommand();
        //    bool alterExistingTrace = false;

        //    string loopTime = TimeSpan.FromMilliseconds(config.LoopTimeMilliseconds).ToString();
        //    loopTime = loopTime.TrimEnd(new char[] { '0', '.' });

        //    StringBuilder filterStatementsLevel1 = new StringBuilder();
        //    StringBuilder filterStatementsLevel2 = new StringBuilder();

        //    int rowcountmax = -1;
        //    string topStatement = "";
        //    if (config.AdvancedConfiguration != null)
        //    {
        //        if (!config.Equals(config.PreviousConfiguration))
        //        {
        //            if (config.PreviousConfiguration != null)
        //            {
        //                //if we have not responded to this change
        //                if (!config.ActivityProfilerConfigChangeResponse)
        //                {
        //                    alterExistingTrace = true;
        //                    LOG.Debug("Activity Profiler config has changed");
        //                    //signal that we have responded
        //                    config.ActivityProfilerConfigChangeResponse = true;
        //                }
        //            }
        //        }

        //        if (config.AdvancedConfiguration.ApplicationExcludeLike != null)
        //        {
        //            foreach (string filterString in config.AdvancedConfiguration.ApplicationExcludeLike)
        //            {
        //                if (!string.IsNullOrEmpty(filterString))
        //                {
        //                    filterStatementsLevel1.Append(
        //                        String.Format(BatchConstants.ExcludeLike, "program_name", filterString.Replace("'", "''"))
        //                        );

        //                    AppNamePredicate.Append(string.Format(BatchConstants.ExcludePredicateNotLikeAppName, filterString.ToLower().Replace("'", "''")));
        //                }
        //            }
        //        }

        //        if (config.AdvancedConfiguration.ExcludeDM)
        //        {
        //            filterStatementsLevel1.Append(
        //                String.Format(BatchConstants.ExcludeLike, "program_name", Constants.ConnectionStringApplicationNamePrefix + '%')
        //                );
        //            AppNamePredicate.Append(string.Format(BatchConstants.ExcludePredicateNotLikeAppName, Constants.ConnectionStringApplicationNamePrefix + '%'));
        //        }

        //        if (config.AdvancedConfiguration.ApplicationExcludeMatch != null)
        //        {
        //            foreach (string filterString in config.AdvancedConfiguration.ApplicationExcludeMatch)
        //            {
        //                if (!string.IsNullOrEmpty(filterString))
        //                {
        //                    filterStatementsLevel1.Append(
        //                        String.Format(BatchConstants.ExcludeMatch, "program_name", filterString.Replace("'", "''"))
        //                        );
        //                    AppNamePredicate.Append(string.Format(BatchConstants.ExcludePredicateNotEqualAppName, filterString.ToLower().Replace("'", "''")));
        //                }
        //            }
        //        }

        //        if (config.AdvancedConfiguration.DatabaseExcludeLike != null)
        //        {
        //            foreach (string filterString in config.AdvancedConfiguration.DatabaseExcludeLike)
        //            {
        //                if (!string.IsNullOrEmpty(filterString))
        //                {
        //                    filterStatementsLevel1.Append(
        //                        String.Format(BatchConstants.ExcludeLike, "db_name(r.database_id)", filterString.Replace("'", "''"))
        //                        );
        //                    DbNamePredicate.Append(string.Format(BatchConstants.ExcludePredicateNotLikeDbName, filterString.ToLower().Replace("'", "''")));
        //                }
        //            }
        //        }
        //        if (config.AdvancedConfiguration.DatabaseExcludeMatch != null)
        //        {
        //            foreach (string filterString in config.AdvancedConfiguration.DatabaseExcludeMatch)
        //            {
        //                if (!string.IsNullOrEmpty(filterString))
        //                {
        //                    filterStatementsLevel1.Append(
        //                        String.Format(BatchConstants.ExcludeMatch, "db_name(r.database_id)", filterString.Replace("'", "''"))
        //                        );
        //                    DbNamePredicate.Append(string.Format(BatchConstants.ExcludePredicateNotEqualDbName, filterString.ToLower().Replace("'", "''")));
        //                }
        //            }
        //        }

        //        //Exclude blank statements
        //        SQLTextPredicate.Append(string.Format(BatchConstants.ExcludePredicateGreaterThanSqlText, ""));

        //        if (config.AdvancedConfiguration.SqlTextExcludeLike != null)
        //        {
        //            foreach (string filterString in config.AdvancedConfiguration.SqlTextExcludeLike)
        //            {
        //                if (!string.IsNullOrEmpty(filterString))
        //                {
        //                    filterStatementsLevel2.Append(
        //                        String.Format(BatchConstants.ExcludeLike, "statement_txt", filterString.Replace("'", "''"))
        //                        );
        //                    SQLTextPredicate.Append(string.Format(BatchConstants.ExcludePredicateNotLikeSqlText, filterString.ToLower().Replace("'", "''")));
        //                }
        //            }
        //        }
        //        if (config.AdvancedConfiguration.SqlTextExcludeMatch != null)
        //        {
        //            foreach (string filterString in config.AdvancedConfiguration.SqlTextExcludeMatch)
        //            {
        //                if (!string.IsNullOrEmpty(filterString))
        //                {
        //                    filterStatementsLevel2.Append(
        //                        String.Format(BatchConstants.ExcludeMatch, "statement_txt", filterString.Replace("'", "''"))
        //                        );
        //                    SQLTextPredicate.Append(string.Format(BatchConstants.ExcludePredicateNotEqualSqlText, filterString.ToLower().Replace("'", "''")));
        //                }
        //            }
        //        }

        //        if (config.AdvancedConfiguration.Rowcount > 0)
        //        {
        //            rowcountmax = config.AdvancedConfiguration.Rowcount;
        //            topStatement = string.Format("top ({0})", rowcountmax);
        //        }
        //    }

        //    List<Pair<string, int?>> excluded = Collection.GetExcludedWaitTypes();
        //    var excludedString = new StringBuilder();
        //    var excludedWaits = new StringBuilder();
        //    var completePredicate = new StringBuilder();

        //    if (ver.Major >= 13 && config.EnabledXe)
        //    {
        //        if (excluded != null && excluded.Count > 0)
        //        {
        //            foreach (Pair<string, int?> s in excluded)
        //            {
        //                excludedString.Append(String.Format("'{0}',", s.First));

        //                //build the exclude waits part of the predicate
        //                if (s.Second.HasValue)
        //                {
        //                    excludedWaits.Append(string.Format(BatchConstants.ExcludePredicateWaitType,
        //                                                        s.Second.ToString()));
        //                }
        //            }

        //            //if we have something to exclude
        //            if (excludedString.Length > 1)
        //            {
        //                //trim the trailing comma
        //                excludedString.Remove(excludedString.Length - 1, 1);
        //            }
        //            else
        //            {
        //                //just exclude "" which wont exist
        //                excludedString.Append("");
        //            }
        //            //excludedString.Insert(0, BatchConstants.ExcludedWaitTypes);
        //        }

        //        completePredicate.Append(string.Format(BatchConstants.ExcludePredicateActiveWaits,
        //                string.Format(BatchConstants.ExcludePredicateDurationLessThan, 50),
        //                string.Format(BatchConstants.ExcludePredicateSystem, 0),
        //                excludedWaits,
        //                SQLTextPredicate,
        //                AppNamePredicate,
        //                DbNamePredicate));

        //        if (filterStatementsLevel2.Length >= 5)
        //            filterStatementsLevel2 = filterStatementsLevel2.Replace("and", "where", 0, 5);

        //        cmd.CommandText +=
        //            String.Format(BatchFinder.ActivityProfiler(ver),
        //                          config.CollectionTimeSeconds,
        //                          config.FileNameXEsession,
        //                          filterStatementsLevel1,
        //                          filterStatementsLevel2,
        //                          alterExistingTrace ? 1 : 0,
        //                          topStatement,
        //                          excludedString,
        //                          completePredicate,
        //                          config.MaxDispatchLatencyXe,
        //                          config.FileSizeRolloverXe,
        //                          config.MaxMemoryXeMB,
        //                          config.FileSizeXeMB,
        //                          config.EventRetentionModeXe,
        //                          config.MaxEventSizeXemb,
        //                          config.MemoryPartitionModeXe,
        //                          config.TrackCausalityXe ? "ON" : "OFF",
        //                          config.StartupStateXe ? "ON" : "OFF");

        //        LOG.Debug("XE - Building Active Waits collector with Extended Events.");
        //    }
        //    else
        //    {

        //        cmd.CommandText +=
        //            String.Format(BatchFinder.ActivityProfiler(new ServerVersion("10.0.5500")),
        //                          config.CollectionTimeSeconds,
        //                          loopTime,
        //                          filterStatementsLevel1,
        //                          filterStatementsLevel2,
        //                          rowcountmax,
        //                          topStatement,
        //                          excludedString,
        //                          ver.MasterDatabaseCompatibility < 90 ? "use tempdb\r\n" : "use master\r\n");

        //        LOG.Debug("No XE - Building Activity Profiler collector without Extended Events.");
        //    }
        //    cmd.CommandType = CommandType.Text;
        //    cmd.CommandTimeout = SqlHelper.CommandTimeout;

        //    return cmd;

        //}

        /// <summary>
        /// Build Active Waits Command for Query Store for collecting query waits
        /// Reads data from databases where query store is enabled
        /// </summary>
        /// <param name="serverVersion">SQL Version</param>
        /// <param name="config">Query Monitor Configuration to add filters</param>
        /// <param name="cloudProviderId">Cloud Provider</param>
        /// <returns>Start Command</returns>
        internal static string BuildActiveWaitsCommandTextQs(
            ServerVersion serverVersion,
            ActiveWaitsConfiguration config,
            int? cloudProviderId,
            string startTime = null
        )
        {
            if (serverVersion.Major >= 14 || cloudProviderId == Constants.MicrosoftAzureManagedInstanceId ||
                cloudProviderId == Constants.MicrosoftAzureId) // SQL Server 2017+
            {
                // Dynamic Query inside string 
                // Query Store filter is inside string '' in sql so we need to ensure we pass strings with double ''
                var queryStoreFilter = new StringBuilder();
                var sessionFilter = new StringBuilder();
                var databaseNamesFilter = new StringBuilder();

                // signal that we have responded since for query store changing of filters will suffice
                config.ActiveWaitsConfigChangeResponse = true;

                // Exclude System Queries to be included once we have session data working Ensure its first statement for session filter
                // sessionFilter.AppendLine(BatchConstants.QueryWaitsExcludeSystemFilterQs);

                // Exclude Dm Filters
                AddExcludeDmFilterQs(config, sessionFilter);

                // Application Include and Exclude with Like and Not Like Filters
                AddApplicationFilterQs(config, sessionFilter);
                
                // Database Include and Exclude with Like and Not Like Filters
                AddDatabaseFilterQs(config, databaseNamesFilter);

                // To resume reading where last left off - Ensure its first statement of querystore filter
                queryStoreFilter.AppendLine(BatchConstants.QueryWaitsStateCheckQs);

                // Hardcoded Duration to support waits greater than 50 ms
                queryStoreFilter.Append(queryStoreFilter.Length > 0 ? BatchConstants.AndConnector : BatchConstants.WhereConnector);
                queryStoreFilter.Append(BatchConstants.QueryWaitsDurationFilterQs);

                // Add SQL Text Filter
                AddSqlTextFilterQs(config, queryStoreFilter);

                // Add Generic Predicate Statements
                AddExclusionWaitStatementsPredicateQs(config, queryStoreFilter);

                // Add Top X ShowPlan Filter config.CollectQueryPlan
                var topCountFilter = AddTopCountFilter(config);

                // Log Filters
                LOG.Debug("BuildActiveWaitsCommandTextQs:: Filters for QueryWaits using Query Store");
                LOG.Debug("BuildActiveWaitsCommandTextQs::queryStoreFilter " + queryStoreFilter.ToString());
                LOG.Debug("BuildActiveWaitsCommandTextQs::sessionFilter " + sessionFilter.ToString());
                LOG.Debug("BuildActiveWaitsCommandTextQs::databaseNamesFilter " + databaseNamesFilter);
                LOG.Debug("BuildActiveWaitsCommandTextQs::topCountFilter " + topCountFilter);

                if (cloudProviderId == Constants.MicrosoftAzureId)
                {
                    return string.Format(
                        BatchFinder.ActiveWaitsReadQs(serverVersion, cloudProviderId),
                        queryStoreFilter.ToString(),
                        sessionFilter.ToString(),
                        databaseNamesFilter,
                        topCountFilter,
                        string.IsNullOrEmpty(startTime) ? "GETUTCDATE()" : startTime).Replace("''", "'");
                }

                return string.Format(
                    BatchFinder.ActiveWaitsReadQs(serverVersion, cloudProviderId),
                    queryStoreFilter.ToString(),
                    sessionFilter.ToString(),
                    databaseNamesFilter,
                    topCountFilter);
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Add conditions based on <see cref="Collection.GetExcludedWaitTypes"/>
        /// </summary>
        /// <param name="config">for getting required filters</param>
        /// <param name="queryStoreFilter">conditions gets appended to this filter</param>
        private static void AddExclusionWaitStatementsPredicateQs(ActiveWaitsConfiguration config, StringBuilder queryStoreFilter)
        {
            var excluded = Collection.GetExcludedWaitTypes();
            if (excluded != null && excluded.Count > 0)
            {
                var excludedString = new StringBuilder();
                var excludedWaits = new StringBuilder();
                var excludedCount = excluded.Count;
                for (var index = 0; index < excludedCount; index++)
                {
                    Pair<string, int?> s = excluded[index];

                    var firstHasValue = !string.IsNullOrEmpty(s.First);
                    var secondHasValue = s.Second.HasValue;

                    if (index == excludedCount - 1)  // Last Element - No Comma in the end
                    {
                        if (firstHasValue)
                        {
                            excludedString.Append(string.Format(BatchConstants.FirstParameterInSqlQuotes, s.First));
                        }
                        if (secondHasValue)
                        {
                            excludedWaits.Append(string.Format(BatchConstants.FirstParameter, s.Second.ToString()));
                        }
                    }
                    else
                    {
                        if (firstHasValue)
                        {
                            excludedString.Append(string.Format(BatchConstants.FirstParameterInSqlQuotesWithComma, s.First));
                        }
                        if (secondHasValue)
                        {
                            excludedWaits.Append(string.Format(BatchConstants.FirstParameterWithComma, s.Second.ToString()));
                        }
                    }
                }
                if (excludedWaits.Length > 0)
                {
                    queryStoreFilter.Append(
                        queryStoreFilter.Length > 0 ? BatchConstants.AndConnector : BatchConstants.WhereConnector);
                    queryStoreFilter.Append(
                        String.Format(BatchConstants.QueryWaitsExcludedPredicateQs, excludedWaits.ToString()));
                }
                if (excludedString.Length > 0)
                {
                    queryStoreFilter.Append(
                        queryStoreFilter.Length > 0 ? BatchConstants.AndConnector : BatchConstants.WhereConnector);
                    queryStoreFilter.Append(
                        String.Format(BatchConstants.QueryWaitsExcludedStringPredicateQs, excludedString.ToString()));
                }
            }
        }

        /// <summary>
        /// Add Top X based on Rowcount provided
        /// </summary>
        /// <param name="config">for getting required filters</param>
        /// <returns>filter string</returns>
        private static string AddTopCountFilter(ActiveWaitsConfiguration config)
        {
            var topCountFilter = string.Empty;
            if (config.AdvancedConfiguration != null && config.AdvancedConfiguration.Rowcount > 0)
            {
                var rowcountmax = config.AdvancedConfiguration.Rowcount;
                topCountFilter = string.Format(BatchConstants.TopWaitFilterQs, rowcountmax);
            }
            return topCountFilter;
        }

        /// <summary>
        /// Add conditions on SQL Text and its Length
        /// </summary>
        /// <param name="config">for getting required filters</param>
        /// <param name="queryStoreFilter">conditions gets appended to this filter</param>
        private static void AddSqlTextFilterQs(ActiveWaitsConfiguration config, StringBuilder queryStoreFilter)
        {
            AddToFinalFilter(BatchConstants.SqlTextNotNullConditionQs, queryStoreFilter, BatchConstants.AndConnector, BatchConstants.WhereConnector);

            if (config.AdvancedConfiguration != null)
            {
                AddGenericFilterQs(
                    BatchConstants.QueryMonitorTextDataFilterQs,
                    config.AdvancedConfiguration.SqlTextExcludeLike,
                    config.AdvancedConfiguration.SqlTextExcludeMatch,
                    config.AdvancedConfiguration.SqlTextIncludeLike,
                    config.AdvancedConfiguration.SqlTextIncludeMatch,
                    queryStoreFilter,
                    BatchConstants.AndConnector,
                    BatchConstants.WhereConnector);
            }

            //default is -1 meaning do nothing. +ve means limit in the interpreter. -ve means limit in the batch
            var sqlTextLengthLimit = CollectionServiceConfiguration.GetCollectionServiceElement().MaxQueryMonitorEventSizeKB;
            //if the limiter is a negative value then this is to be used as the character length limit of the sql_text used in our where clause
            var sqlTextLengthLimitCondition = sqlTextLengthLimit < -1
                                                  ? string.Format(
                                                      BatchConstants.SqlTextLengthLimiterQs,
                                                      Math.Abs(sqlTextLengthLimit))
                                                  : string.Empty;
            AddToFinalFilter(sqlTextLengthLimitCondition, queryStoreFilter, BatchConstants.AndConnector, BatchConstants.WhereConnector);
        }

        /// <summary>
        /// Add Databae filters based on configuration, will be part of if statement
        /// </summary>
        /// <param name="config">for getting required filters</param>
        /// <param name="databaseFilter">conditions gets appended to this filter</param>
        /// <remarks>
        /// Adds 1=1 condition if no condition found for databases
        /// </remarks>
        private static void AddDatabaseFilterQs(ActiveWaitsConfiguration config, StringBuilder databaseFilter)
        {
            if (config.AdvancedConfiguration != null)
            {
                AddGenericFilterQs(
                    BatchConstants.QueryMonitorDatabaseFilterQs,
                    config.AdvancedConfiguration.DatabaseExcludeLike,
                    config.AdvancedConfiguration.DatabaseExcludeMatch,
                    config.AdvancedConfiguration.DatabaseIncludeLike,
                    config.AdvancedConfiguration.DatabaseIncludeMatch,
                    databaseFilter,
                    BatchConstants.AndConnector,
                    BatchConstants.SpaceConnector);
            }

            // Cannot pass empty filter since inside if statement
            if (databaseFilter.Length == 0)
            {
                databaseFilter.Append(BatchConstants.QueryMonitorAlwaysTrueFilterQs);
            }
        }

        /// <summary>
        /// Add Application filters based on configuration, will be part of where clause
        /// </summary>
        /// <param name="config">for getting application filters</param>
        /// <param name="sessionFilter">conditions gets appended to this filter</param>
        private static void AddApplicationFilterQs(ActiveWaitsConfiguration config, StringBuilder sessionFilter)
        {
            if(config.AdvancedConfiguration!=null)
            {
                AddGenericFilterQs(
                BatchConstants.QueryMonitorApplicationFilterQs,
                config.AdvancedConfiguration.ApplicationExcludeLike,
                config.AdvancedConfiguration.ApplicationExcludeMatch,
                config.AdvancedConfiguration.ApplicationIncludeLike,
                config.AdvancedConfiguration.ApplicationIncludeMatch,
                sessionFilter,
                BatchConstants.AndConnector,
                BatchConstants.WhereConnector);}
        }

        /// <summary>
        /// Add Exclude SQLdm filter based on Application Filters
        /// </summary>
        /// <param name="config">for getting required filters</param>
        /// <param name="sessionFilter">conditions gets appended to this filter</param>
        private static void AddExcludeDmFilterQs(ActiveWaitsConfiguration config, StringBuilder sessionFilter)
        {
            if (config.AdvancedConfiguration != null && config.AdvancedConfiguration.ExcludeDM)
            {
                sessionFilter.Append(
                    String.Format(
                        BatchConstants.QueryMonitorApplicationFilterQs,
                        (sessionFilter.Length == 0 ? BatchConstants.WhereConnector : BatchConstants.AndConnector),
                        BatchConstants.NotLikeConnector,
                        Constants.ConnectionStringApplicationNamePrefix + BatchConstants.PercentageChar));

                sessionFilter.Append(
                    String.Format(
                        BatchConstants.QueryMonitorApplicationFilterQs,
                        (sessionFilter.Length == 0 ? BatchConstants.WhereConnector : BatchConstants.AndConnector),
                        BatchConstants.NotConnector,
                        BatchConstants.DiagnosticManExcludeFilter));
            }
        }
      
        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        internal static SqlCommand BuildActiveWaitsCommand(SqlConnection connection, ServerVersion ver, ActiveWaitsConfiguration config, int? cloudProviderId = null, bool? isSysadminMemebr = false)
        {
            SqlCommand cmd = connection.CreateCommand();
            // SQLdm 10.4(Varun Chopra) query waits using Query Store
            if ((ver.Major >= 14 || cloudProviderId == Constants.MicrosoftAzureManagedInstanceId ||
                 cloudProviderId == Constants.MicrosoftAzureId) && config.EnabledQs)
            {
                string startTime = null;

                if (cloudProviderId == Constants.MicrosoftAzureId)
                {
                    startTime = PersistenceManager.Instance.GetAzureQsStartTime(config.MonitoredServerId, connection.Database, AzureQsType.ActiveWaits);
                }

                LOG.Debug("QS - Building Active Waits collector with Query Store.");
                // add query store commands for collecting active waits
                cmd.CommandText += BuildActiveWaitsCommandTextQs(ver, config, cloudProviderId, startTime);
            }
            else   // Using Extended Events and Trace
            {
                
                StringBuilder SQLTextPredicate = new StringBuilder();
                StringBuilder AppNamePredicate = new StringBuilder();
                StringBuilder DbNamePredicate = new StringBuilder();
                
                bool alterExistingTrace = false;
                
                StringBuilder filterStatementsLevel1 = new StringBuilder();
                StringBuilder filterStatementsLevel2 = new StringBuilder();
                
                string loopTime = TimeSpan.FromMilliseconds(config.LoopTimeMilliseconds).ToString();
                loopTime = loopTime.TrimEnd(new char[] { '0', '.' });
                int rowcountmax = -1;
                string topStatement = "";
                if (config.AdvancedConfiguration != null)
                {
                    if (!config.Equals(config.PreviousConfiguration))
                    {
                        if (config.PreviousConfiguration != null)
                        {
                            //if we have not responded to this change
                            if (!config.ActiveWaitsConfigChangeResponse)
                            {
                                alterExistingTrace = true;
                                LOG.Debug("Advanced config has changed");
                                //signal that we have responded
                                config.ActiveWaitsConfigChangeResponse = true;
                                config.PreviousConfiguration = config;
                            }
                        }
                    }
                
                    if (config.AdvancedConfiguration.ExcludeDM)
                    {
                        filterStatementsLevel1.Append(
                            String.Format(BatchConstants.ExcludeLike, "program_name", Constants.ConnectionStringApplicationNamePrefix +     BatchConstants.PercentageChar)
                            );
                        AppNamePredicate.Append(string.Format(BatchConstants.ExcludePredicateNotLikeAppName,     Constants.ConnectionStringApplicationNamePrefix + BatchConstants.PercentageChar));
                    }
                
                    bool isAppExcluded = false;
                    if (config.AdvancedConfiguration.ApplicationExcludeLike != null)
                    {
                        foreach (string filterString in config.AdvancedConfiguration.ApplicationExcludeLike)
                        {
                            if (!string.IsNullOrEmpty(filterString))
                            {
                                isAppExcluded = true;
                                filterStatementsLevel1.Append(
                                    String.Format(BatchConstants.ExcludeLike, "program_name", filterString.Replace("'", "''"))
                                    );
                
                                AppNamePredicate.Append(string.Format(BatchConstants.ExcludePredicateNotLikeAppName, filterString.ToLower().Replace  ("'",   "''")));
                            }
                        }
                    }
                
                    if (config.AdvancedConfiguration.ApplicationExcludeMatch != null)
                    {
                        foreach (string filterString in config.AdvancedConfiguration.ApplicationExcludeMatch)
                        {
                            if (!string.IsNullOrEmpty(filterString))
                            {
                                isAppExcluded = true;
                                filterStatementsLevel1.Append(
                                    String.Format(BatchConstants.ExcludeMatch, "program_name", filterString.Replace("'", "''"))
                                    );
                                AppNamePredicate.Append(string.Format(BatchConstants.ExcludePredicateNotEqualAppName, filterString.ToLower().Replace   ("'",  "''")));
                            }
                        }
                    }
                
                    if (!isAppExcluded)
                    {
                        //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters
                        if (config.AdvancedConfiguration.ApplicationIncludeLike != null)
                        {
                            foreach (string filterString in config.AdvancedConfiguration.ApplicationIncludeLike)
                            {
                                if (!string.IsNullOrEmpty(filterString))
                                {
                                    filterStatementsLevel1.Append(
                                        String.Format(BatchConstants.IncludeLike, "program_name", filterString.Replace("'", "''"))
                                        );
                                    AppNamePredicate.Append(string.Format(BatchConstants.IncludePredicateLikeAppName, filterString.ToLower().Replace   ("'",  "''")));
                                }
                            }
                        }
                
                        if (config.AdvancedConfiguration.ApplicationIncludeMatch != null)
                        {
                            foreach (string filterString in config.AdvancedConfiguration.ApplicationIncludeMatch)
                            {
                                if (!string.IsNullOrEmpty(filterString))
                                {
                                    filterStatementsLevel1.Append(
                                        String.Format(BatchConstants.IncludeMatch, "program_name", filterString.Replace("'", "''"))
                                        );
                                    AppNamePredicate.Append(string.Format(BatchConstants.IncludePredicateEqualAppName, filterString.ToLower().Replace    ("'", "''")));
                                }
                            }
                        }
                        //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters
                
                    }
                    if (cloudProviderId != Constants.MicrosoftAzureId) // Azure is db specific. Include/Exclude db is being handled in collector
                    {
                        bool isDbExcluded = false;
                        if (config.AdvancedConfiguration.DatabaseExcludeLike != null)
                        {
                            foreach (string filterString in config.AdvancedConfiguration.DatabaseExcludeLike)
                            {
                                if (!string.IsNullOrEmpty(filterString))
                                {
                                    isDbExcluded = true;
                                    filterStatementsLevel1.Append(
                                        String.Format(BatchConstants.ExcludeLike, "db_name(r.database_id)", filterString.Replace("'", "''"))
                                        );
                                    DbNamePredicate.Append(string.Format(BatchConstants.ExcludePredicateNotLikeDbName, filterString.ToLower().Replace("'", "''")));
                                }
                            }
                        }


                        if (config.AdvancedConfiguration.DatabaseExcludeMatch != null)
                        {
                            foreach (string filterString in config.AdvancedConfiguration.DatabaseExcludeMatch)
                            {
                                if (!string.IsNullOrEmpty(filterString))
                                {

                                    isDbExcluded = true;
                                    filterStatementsLevel1.Append(
                                        String.Format(BatchConstants.ExcludeMatch, "db_name(r.database_id)", filterString.Replace("'", "''"))
                                        );
                                    DbNamePredicate.Append(string.Format(BatchConstants.ExcludePredicateNotEqualDbName, filterString.ToLower().Replace("'", "''")));
                                }
                            }
                        }


                        if (!isDbExcluded)
                        {
                            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters
                            if (config.AdvancedConfiguration.DatabaseIncludeLike != null)
                            {
                                foreach (string filterString in config.AdvancedConfiguration.DatabaseIncludeLike)
                                {
                                    if (!string.IsNullOrEmpty(filterString))
                                    {
                                        filterStatementsLevel1.Append(
                                            String.Format(BatchConstants.IncludeLike, "db_name(r.database_id)", filterString.Replace("'", "''"))
                                            );
                                        DbNamePredicate.Append(string.Format(BatchConstants.IncludePredicateLikeDbName, filterString.ToLower().Replace("'", "''")));
                                    }
                                }
                            }

                            if (config.AdvancedConfiguration.DatabaseIncludeMatch != null)
                            {
                                foreach (string filterString in config.AdvancedConfiguration.DatabaseIncludeMatch)
                                {
                                    if (!string.IsNullOrEmpty(filterString))
                                    {
                                        filterStatementsLevel1.Append(
                                            String.Format(BatchConstants.IncludeMatch, "db_name(r.database_id)", filterString.Replace("'", "''"))
                                            );
                                        DbNamePredicate.Append(string.Format(BatchConstants.IncludePredicateEqualDbName, filterString.ToLower().Replace("'", "''")));
                                    }
                                }
                            }
                            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters

                        }
                    }
                    //Exclude blank statements
                    SQLTextPredicate.Append(string.Format(BatchConstants.ExcludePredicateGreaterThanSqlText, ""));
                
                
                    if (config.AdvancedConfiguration.SqlTextExcludeLike != null)
                    {
                        foreach (string filterString in config.AdvancedConfiguration.SqlTextExcludeLike)
                        {
                            if (!string.IsNullOrEmpty(filterString))
                            {
                                filterStatementsLevel2.Append(
                                    String.Format(BatchConstants.ExcludeLike, "statement_txt", filterString.Replace("'", "''"))
                                    );
                                SQLTextPredicate.Append(string.Format(BatchConstants.ExcludePredicateNotLikeSqlText, filterString.ToLower().Replace  ("'",   "''")));
                            }
                        }
                    }
                    if (config.AdvancedConfiguration.SqlTextExcludeMatch != null)
                    {
                        foreach (string filterString in config.AdvancedConfiguration.SqlTextExcludeMatch)
                        {
                            if (!string.IsNullOrEmpty(filterString))
                            {
                                filterStatementsLevel2.Append(
                                    String.Format(BatchConstants.ExcludeMatch, "statement_txt", filterString.Replace("'", "''"))
                                    );
                                SQLTextPredicate.Append(string.Format(BatchConstants.ExcludePredicateNotEqualSqlText, filterString.ToLower().Replace   ("'",  "''")));
                            }
                        }
                    }
                
                    //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters
                    if (config.AdvancedConfiguration.SqlTextIncludeLike != null)
                    {
                        foreach (string filterString in config.AdvancedConfiguration.SqlTextIncludeLike)
                        {
                            if (!string.IsNullOrEmpty(filterString))
                            {
                                filterStatementsLevel2.Append(
                                    String.Format(BatchConstants.IncludeLike, "statement_txt", filterString.Replace("'", "''"))
                                    );
                                SQLTextPredicate.Append(string.Format(BatchConstants.IncludePredicateLikeSqlText, filterString.ToLower().Replace("'",     "''")));
                            }
                        }
                    }
                    if (config.AdvancedConfiguration.SqlTextIncludeMatch != null)
                    {
                        foreach (string filterString in config.AdvancedConfiguration.SqlTextIncludeMatch)
                        {
                            if (!string.IsNullOrEmpty(filterString))
                            {
                                filterStatementsLevel2.Append(
                                    String.Format(BatchConstants.IncludeMatch, "statement_txt", filterString.Replace("'", "''"))
                                    );
                                SQLTextPredicate.Append(string.Format(BatchConstants.IncludePredicateEqualSqlText, filterString.ToLower().Replace("'",     "''")));
                            }
                        }
                    }
                    //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters
                
                
                
                    if (config.AdvancedConfiguration.Rowcount > 0)
                    {
                        rowcountmax = config.AdvancedConfiguration.Rowcount;
                        topStatement = string.Format("top ({0})", rowcountmax);
                    }
                }
                
                List<Pair<string, int?>> excluded = Collection.GetExcludedWaitTypes();
                //List<Pair<string, int?>> excluded = null;
                var excludedString = new StringBuilder();
                var excludedWaits = new StringBuilder();
                var completePredicate = new StringBuilder();
                
                if (ver.Major >= 11 && config.EnabledXe)
                {
                    if (excluded != null && excluded.Count > 0)
                    {
                        foreach (Pair<string, int?> s in excluded)
                        {
                            excludedString.Append(String.Format("'{0}',", s.First));
                
                            //build the exclude waits part of the predicate
                            if (s.Second.HasValue)
                            {
                                excludedWaits.Append(string.Format(BatchConstants.ExcludePredicateWaitType,
                                                                    s.Second.ToString()));
                            }
                        }
                
                        //if we have something to exclude
                        if (excludedString.Length > 1)
                        {
                            //trim the trailing comma
                            excludedString.Remove(excludedString.Length - 1, 1);
                        }
                        else
                        {
                            //just exclude "" which wont exist
                            excludedString.Append("");
                        }
                        //excludedString.Insert(0, BatchConstants.ExcludedWaitTypes);
                    }
                
                    completePredicate.Append(string.Format(BatchConstants.ExcludePredicateActiveWaits,
                            string.Format(BatchConstants.ExcludePredicateDurationLessThan, 50),
                            string.Format(BatchConstants.ExcludePredicateSystem, 0),
                            excludedWaits,
                            SQLTextPredicate,
                            AppNamePredicate,
                            DbNamePredicate));
                
                    if (filterStatementsLevel2.Length >= 5)
                        filterStatementsLevel2 = filterStatementsLevel2.Replace("and", "where", 0, 5);
                
                    // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
                    //Start--SQLdm 10.3 (Tushar)--Support of extended events LINQ assembly for active waits. 
                    if ((bool)isSysadminMemebr)
                    {
                        cmd.CommandText +=
                            String.Format(BatchFinder.ActiveWaits(ver, cloudProviderId, (bool)isSysadminMemebr),
                            config.CollectionTimeSeconds,
                            config.FileNameXEsession,
                            filterStatementsLevel1,
                            filterStatementsLevel2,
                            alterExistingTrace ? 1 : 0,
                            topStatement,
                            excludedString,
                            completePredicate,
                            config.MaxDispatchLatencyXe,
                            config.FileSizeRolloverXe,
                            config.MaxMemoryXeMB,
                            config.FileSizeXeMB,
                            config.EventRetentionModeXe,
                            config.MaxEventSizeXemb,
                            config.MemoryPartitionModeXe,
                            config.TrackCausalityXe ? "ON" : "OFF",
                            config.StartupStateXe ? "ON" : "OFF",
                            BatchFinder.ActiveWaitsReadEX(cloudProviderId));
                        LOG.Debug("XE - Building Active Waits collector with Extended Events api");
                    }
                    else
                    {
                        cmd.CommandText +=
                        String.Format(BatchFinder.ActiveWaits(ver,cloudProviderId,(bool)isSysadminMemebr),
                                  config.CollectionTimeSeconds,
                                  config.FileNameXEsession,
                                  filterStatementsLevel1,
                                  filterStatementsLevel2,
                                  alterExistingTrace ? 1 : 0,
                                  topStatement,
                                  excludedString,
                                  completePredicate,
                                  config.MaxDispatchLatencyXe,
                                  config.FileSizeRolloverXe,
                                  config.MaxMemoryXeMB,
                                  config.FileSizeXeMB,
                                  config.EventRetentionModeXe,
                                  config.MaxEventSizeXemb,
                                  config.MemoryPartitionModeXe,
                                  config.TrackCausalityXe ? "ON" : "OFF",
                                  config.StartupStateXe ? "ON" : "OFF");
                        LOG.Debug("XE - Building Active Waits collector with Extended Events.");
                    }
                    //ENd--SQLdm 10.3 (Tushar)--Support of extended events LINQ assembly for active waits. 
                }
                else
                {
                    if (excluded != null && excluded.Count > 0)
                    {
                
                        foreach (Pair<string, int?> s in excluded)
                        {
                            excludedString.Append(String.Format("'{0}',", s.First));
                        }
                        //if we have something to exclude
                        if (excludedString.Length > 1)
                        {
                            //trim the trailing comma
                            excludedString.Remove(excludedString.Length - 1, 1);
                        }
                        else
                        {
                            //just exclude "" which wont exist
                            excludedString.Append("");
                        }
                    }
                
                    cmd.CommandText +=
                        String.Format(BatchFinder.ActiveWaits(new ServerVersion("10.0.5500"), cloudProviderId),
                                      config.CollectionTimeSeconds,
                                      loopTime,
                                      filterStatementsLevel1,
                                      filterStatementsLevel2,
                                      rowcountmax,
                                      topStatement,
                                      excludedString,
                                      ver.MasterDatabaseCompatibility < 90 ? "use tempdb\r\n" : "use master\r\n");
                
                    LOG.Debug("No XE - Building Active Waits collector without Extended Events.");
                }
            }
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;

            return cmd;
        }

        /// <summary>
        /// SQLdm 10.3 (Tushar) - Batch to write last filename, record count
        /// </summary>
        /// <param name="lastFileName"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        internal static string BuildActiveWaitesWriteCommand(string lastFileName, long recordCount)
        {
            return String.Format(BatchFinder.ActiveWaitsWriteEX(), lastFileName, recordCount);
        }

        internal static SqlCommand BuildAlternateDistributionStatusCommand(SqlConnection connection, ServerVersion ver)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchConstants.CopyrightNotice + BatchConstants.BatchHeader +
                               BatchConstants.AlternateDistributionStatus;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildPublicationCountersCommand(SqlConnection connection, ServerVersion ver, PublisherDetailsConfiguration config)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchConstants.CopyrightNotice + BatchConstants.BatchHeader +
                               string.Format(BatchConstants.PublicationCounters, config.PublisherDatabase.Replace("'", "''"));
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildAgentJobHistoryCommand(SqlConnection connection, ServerVersion ver,
                                                               AgentJobHistoryConfiguration config)
        {
            SqlCommand cmd = connection.CreateCommand();
            string filterString = "";
            if (config.JobIdList.Count > 0)
            {
                StringBuilder jobIdList = new StringBuilder();
                foreach (Guid jobid in config.JobIdList)
                {
                    jobIdList.Append(",'");
                    jobIdList.Append(jobid);
                    jobIdList.Append("'");
                }
                if (jobIdList.Length > 1)
                {
                    filterString = String.Format(BatchConstants.FilterForJobId, jobIdList.Remove(0, 1));
                }
            }
            cmd.CommandText += String.Format(BatchFinder.AgentJobHistory(ver), filterString);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        // Start ID: M1
        /// <summary>
        /// Build the Azure SQL Metric Command
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        internal static SqlCommand BuildAzureSQLMetricCommand(SqlConnection connection)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.AzureSQLMetric();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildCloudSQLDatabaseCommand(SqlConnection connection)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.CloudSQLDatabase();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        // End ID: M1

        internal static SqlCommand BuildAgentJobSummaryCommand(SqlConnection connection, ServerVersion ver,
                                                               AgentJobSummaryConfiguration config,
                                                               WmiConfiguration wmiConfig,
                                                               ClusterCollectionSetting clusterCollectionSetting,
                                                                MonitoredServerWorkload workload)
        {

            LogSpOAContext(connection.DataSource, "AgentJobSummary", (int)wmiConfig.OleAutomationContext);
            string productEdition = (workload.MonitoredServer.ConnectionInfo != null && workload.MonitoredServer.ConnectionInfo.ConnectionString != null) ? CollectionHelper.GetProductEdition(workload.MonitoredServer.ConnectionInfo.ConnectionString) : string.Empty; // SQLdm 8.6 (Ankit Srivastava) -- getting Product Edition - solved defect DE43661
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText =
                String.Format(BatchFinder.AgentJobSummary(ver),
                              (int)wmiConfig.OleAutomationContext,
                              config.JobSummaryFilter == AgentJobSummaryConfiguration.JobSummaryFilterType.Running
                                  ? BatchConstants.FilterForRunningJobs
                                  : "",
                              wmiConfig.OleAutomationDisabled ? 1 : 0,
                              (int)clusterCollectionSetting > 0 ? BatchConstants.ClusterSettingFalse : "",
                              wmiConfig.DirectWmiEnabled ? 1 : 0,
                              productEdition.IndexOf("Express", StringComparison.OrdinalIgnoreCase) > -1 ? 1 : 0); //setting flag for express edition. Ankit Srivastava SQLdm 8.6
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildAgentJobSummaryCommand(SqlConnection connection, ServerVersion ver,
                                                         AgentJobSummaryConfiguration config,
                                                         //WmiConfiguration wmiConfig,
                                                         ClusterCollectionSetting clusterCollectionSetting,
                                                          MonitoredServerWorkload workload)
        {
            //LogSpOAContext(connection.DataSource, "AgentJobSummary", (int)wmiConfig.OleAutomationContext);
            string productEdition = (workload.MonitoredServer.ConnectionInfo != null && workload.MonitoredServer.ConnectionInfo.ConnectionString != null) ? CollectionHelper.GetProductEdition(workload.MonitoredServer.ConnectionInfo.ConnectionString) : string.Empty; // SQLdm 8.6 (Ankit Srivastava) -- getting Product Edition - solved defect DE43661
            SqlCommand cmd = connection.CreateCommand();
           
            cmd.CommandText = "select sj.job_id,sj.originating_server_id as originating_server,sj.name,sj.enabled,sj.description,sj.start_step_id,category=ISNULL(sc.name, FORMATMESSAGE(14205)),sj.notify_level_eventlog,sj.notify_level_email,sj.notify_level_netsend,sj.notify_level_page,sj.delete_level,sj.date_created,sj.date_modified,sj.version_number,sja.start_execution_date,sja.last_executed_step_id,sja.last_executed_step_date,sja.stop_execution_date,sja.job_history_id,sja.next_scheduled_run_date,sjh.run_status,sjh.run_date,sjh.run_time,sjh.run_duration,sl.loginname  from msdb.dbo.sysjobs sj left join msdb.dbo.syscategories sc on sj.category_id=sc.category_id left join msdb.dbo.sysjobactivity sja on sj.job_id=sja.job_id left join msdb.dbo.sysjobhistory sjh on sja.job_history_id=sjh.instance_id left join master.dbo.syslogins sl on sl.sid=sj.owner_sid";

            //cmd.CommandText = "select * from msdb.dbo.sysjobs";
            /* String.Format(BatchFinder.AgentJobSummary(ver),
                           //(int)wmiConfig.OleAutomationContext,
                           config.JobSummaryFilter == AgentJobSummaryConfiguration.JobSummaryFilterType.Running
                               ? BatchConstants.FilterForRunningJobs
                               : "",
                           //wmiConfig.OleAutomationDisabled ? 1 : 0,
                           (int)clusterCollectionSetting > 0 ? BatchConstants.ClusterSettingFalse : "",
                           //wmiConfig.DirectWmiEnabled ? 1 : 0,
                           productEdition.IndexOf("Express", StringComparison.OrdinalIgnoreCase) > -1 ? 1 : 0); //setting flag for express edition. Ankit Srivastava SQLdm 8.6 */
             cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }


        internal static SqlCommand BuildBackupRestoreHistoryCommand(SqlConnection connection, ServerVersion ver,
                                                                    BackupRestoreHistoryConfiguration config)
        {
            SqlCommand cmd = connection.CreateCommand();
            StringBuilder databaseIncludeString = new StringBuilder();
            foreach (string database in config.DatabaseNames)
            {
                databaseIncludeString.Append(",'");
                databaseIncludeString.Append(database.Replace("'", "''"));
                databaseIncludeString.Append("'");
            }
            databaseIncludeString.Remove(0, 1);

            if (config.ShowBackups)
            {
                if (config.ShowLogicalFileNames)
                {
                    cmd.CommandText +=
                        String.Format(BatchFinder.BackupHistoryFull(ver), databaseIncludeString, config.DaysToShow);
                    if (ver.Major >= 9) // SQL Server 2005/2008
                    {
                        cmd.CommandText += BatchConstants.IncludePresentFileBackupsOnly2005;
                    }
                }
                else
                {
                    cmd.CommandText +=
                        String.Format(BatchFinder.BackupHistorySmall(ver), databaseIncludeString, config.DaysToShow);
                }
            }

            if (config.ShowRestores)
            {
                if (config.ShowLogicalFileNames)
                {
                    cmd.CommandText +=
                        String.Format(BatchFinder.RestoreHistoryFull(ver), databaseIncludeString, config.DaysToShow);
                    if (ver.Major >= 9) // SQL Server 2005/2008
                    {
                        cmd.CommandText += BatchConstants.IncludePresentFileBackupsOnly2005;
                    }
                }
                else
                {
                    cmd.CommandText +=
                        String.Format(BatchFinder.RestoreHistorySmall(ver), databaseIncludeString, config.DaysToShow);
                }
            }
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildConfigurationCommand(SqlConnection connection, ServerVersion ver, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.Configuration(ver, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        //START: SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- Added method for new probes.
        internal static SqlCommand BuildSetShowPlanONCommand(SqlConnection connection, ServerVersion ver)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "set showplan_xml_with_recompile on";
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildQueryPlanCommand(SqlConnection connection, ServerVersion ver, string query)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildSetShowPlanOFFCommand(SqlConnection connection, ServerVersion ver)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "set showplan_xml_with_recompile off";
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildIndexContentionCommand(SqlConnection connection, ServerVersion ver, string db)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (!string.IsNullOrEmpty(db))
            {
                cmd.CommandText += "PRINT '" + db + "'; ";
                cmd.CommandText += System.Environment.NewLine;
            }
            cmd.CommandText += BatchFinder.IndexContention(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildOverlappingIndexesCommand(SqlConnection connection, ServerVersion ver, string db)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (!string.IsNullOrEmpty(db))
            {
                cmd.CommandText += "PRINT '" + db + "'; ";
                cmd.CommandText += System.Environment.NewLine;
            }
            cmd.CommandText += BatchFinder.OverlappingIndexes(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildDisabledIndexesCommand(SqlConnection connection, ServerVersion ver, string db)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (!string.IsNullOrEmpty(db))
            {
                cmd.CommandText += "PRINT '" + db + "'; ";
                cmd.CommandText += System.Environment.NewLine;
            }
            cmd.CommandText += BatchFinder.DisabledIndexes(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildDatabaseNamesCommand(SqlConnection connection, ServerVersion ver, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.DatabaseNames(ver, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildFragmentedIndexesCommand(SqlConnection connection, ServerVersion ver, string db, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (!string.IsNullOrEmpty(db))
            {
                cmd.CommandText += "PRINT '" + db + "'; ";
                cmd.CommandText += System.Environment.NewLine;
            }
            cmd.CommandText += BatchFinder.FragmentedIndexes(ver, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildHighIndexUpdatesCommand(SqlConnection connection, ServerVersion ver, string db)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (!string.IsNullOrEmpty(db))
            {
                cmd.CommandText += "PRINT '" + db + "'; ";
                cmd.CommandText += System.Environment.NewLine;
            }
            cmd.CommandText += BatchFinder.HighIndexUpdates(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildGetWorstFillFactorIndexesCommand(SqlConnection connection, ServerVersion ver, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.GetWorstFillFactorIndexes(ver, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildBackupAndRecoveryCommand(SqlConnection connection, ServerVersion ver, string db, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (!string.IsNullOrEmpty(db))
            {
                cmd.CommandText += "PRINT '" + db + "'; ";
                cmd.CommandText += System.Environment.NewLine;
            }
            cmd.CommandText += BatchFinder.BackupAndRecovery(ver, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildOutOfDateStatsCommand(SqlConnection connection, ServerVersion ver, string db)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (!string.IsNullOrEmpty(db))
            {
                cmd.CommandText += "PRINT '" + db + "'; ";
                cmd.CommandText += System.Environment.NewLine;
            }
            cmd.CommandText += BatchFinder.OutOfDateStats(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildSQLModuleOptionsCommand(SqlConnection connection, ServerVersion ver, string db)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (!string.IsNullOrEmpty(db))
            {
                cmd.CommandText += "PRINT '" + db + "'; ";
                cmd.CommandText += System.Environment.NewLine;
            }
            cmd.CommandText += BatchFinder.SQLModuleOptions(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildDBSecurityCommand(SqlConnection connection, ServerVersion ver, string db, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (!string.IsNullOrEmpty(db))
            {
                cmd.CommandText += "PRINT '" + db + "'; ";
                cmd.CommandText += System.Environment.NewLine;
            }
            cmd.CommandText += BatchFinder.DBSecurity(ver, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildGetMasterFilesCommand(SqlConnection connection, ServerVersion ver, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.GetMasterFiles(ver, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildNUMANodeCountersCommand(SqlConnection connection, ServerVersion ver, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.NUMANodeCounters(ver, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildQueryPlanEstRowsCommand(SqlConnection connection, ServerVersion ver, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.QueryPlanEstRows(ver, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildSampleServerResourcesCommand(SqlConnection connection, ServerVersion ver, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.SampleServerResources(ver, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildGetAdhocCachedPlanBytesCommand(SqlConnection connection, ServerVersion ver, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.GetAdhocCachedPlanBytes(ver, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildGetLockedPageKBCommand(SqlConnection connection, ServerVersion ver, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.GetLockedPageKB(ver, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildServerConfigurationCommand(SqlConnection connection, ServerVersion ver, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.ServerConfiguration(ver, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildWaitingBatchesCommand(SqlConnection connection, ServerVersion ver, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.WaitingBatches(ver, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildDatabaseRankingCommand(SqlConnection connection, ServerVersion ver)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.DatabaseRanking(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        //END: SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- Added method for new probes.

        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I23 Adding new analyzer 
        internal static SqlCommand BuildNonIncrementalColumnStatsCommand(SqlConnection connection, ServerVersion ver, string db)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (!string.IsNullOrEmpty(db))
            {
                cmd.CommandText += "PRINT '" + db + "'; ";
                cmd.CommandText += System.Environment.NewLine;
            }
            cmd.CommandText += BatchFinder.NonIncrementalColumnStats(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I25, I26, I28 Adding new Batch comm 
        internal static SqlCommand BuildHashIndexCommand(SqlConnection connection, ServerVersion ver, string db)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (!string.IsNullOrEmpty(db))
            {
                cmd.CommandText += "PRINT '" + db + "'; ";
                cmd.CommandText += System.Environment.NewLine;
            }
            cmd.CommandText += BatchFinder.HashIndex(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-Q-39,Q40,Q41, Q42 Adding new Batch comm 
        internal static SqlCommand BuildQueryStoreCommand(SqlConnection connection, ServerVersion ver, string db)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (!string.IsNullOrEmpty(db))
            {
                cmd.CommandText += "PRINT '" + db + "'; ";
                cmd.CommandText += System.Environment.NewLine;
            }
            cmd.CommandText += BatchFinder.QueryStore(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I29 Adding new Batch comm 
        internal static SqlCommand BuildRarelyUsedIndexOnInMemoryTableCommand(SqlConnection connection, ServerVersion ver, string db)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (!string.IsNullOrEmpty(db))
            {
                cmd.CommandText += "PRINT '" + db + "'; ";
                cmd.CommandText += System.Environment.NewLine;
            }
            cmd.CommandText += BatchFinder.RarelyUsedIndexOnInMemoryTable(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        //SQLdm 10.0 (Srishti Purohit) New Recommendation Q46,Q47,Q48,Q49,Q50 New Batch
        internal static SqlCommand BuildQueryAnalyzerCommand(SqlConnection connection, ServerVersion ver, string db)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (!string.IsNullOrEmpty(db))
            {
                cmd.CommandText += "PRINT '" + db + "'; ";
                cmd.CommandText += System.Environment.NewLine;
            }
            cmd.CommandText += BatchFinder.QueryAnalyzer(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I30
        internal static SqlCommand BuildColumnStoreIndexCommand(SqlConnection connection, ServerVersion ver, string db, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (!string.IsNullOrEmpty(db))
            {
                cmd.CommandText += "PRINT '" + db + "'; ";
                cmd.CommandText += System.Environment.NewLine;
            }
            cmd.CommandText += BatchFinder.ColumnStoreIndex(ver, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I31
        internal static SqlCommand BuildFilteredColumnNotInKeyOfFilteredIndexCommand(SqlConnection connection, ServerVersion ver, string db)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (!string.IsNullOrEmpty(db))
            {
                cmd.CommandText += "PRINT '" + db + "'; ";
                cmd.CommandText += System.Environment.NewLine;
            }
            cmd.CommandText += BatchFinder.FilteredColumnNotInKeyOfFilteredIndex(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-Q43 Adding new batch 
        internal static SqlCommand BuildHighCPUTimeProcedureCommand(SqlConnection connection, ServerVersion ver, string db)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (!string.IsNullOrEmpty(db))
            {
                cmd.CommandText += "PRINT '" + db + "'; ";
                cmd.CommandText += System.Environment.NewLine;
            }
            cmd.CommandText += BatchFinder.HighCPUTimeProcedure(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I24
        internal static SqlCommand BuildLargeTableStatsCommand(SqlConnection connection, ServerVersion ver, string db)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (!string.IsNullOrEmpty(db))
            {
                cmd.CommandText += "PRINT '" + db + "'; ";
                cmd.CommandText += System.Environment.NewLine;
            }
            cmd.CommandText += BatchFinder.LargeTableStats(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-M33
        internal static SqlCommand BuildBufferPoolExtIOCommand(SqlConnection connection, ServerVersion ver, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.BufferPoolExtIO(ver, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildCustomCounterOSCommand(SqlConnection connection, ServerVersion ver,
                                                               CustomCounterDefinition def, int spOAContext,
                                                               bool disableOle)
        {
            LogSpOAContext(connection.DataSource, "CustomCounterOS", spOAContext);
            string instanceName = String.IsNullOrEmpty(def.InstanceName) ? "=@" : "." + def.InstanceName;

            SqlCommand cmd = connection.CreateCommand();
            TimeSpan customTimeSpan =
                TimeSpan.FromSeconds(
                    CollectionServiceConfiguration.GetCollectionServiceElement().CustomOSCounterWaitTimeInSeconds > 0
                        ? CollectionServiceConfiguration.GetCollectionServiceElement().CustomOSCounterWaitTimeInSeconds
                        : 0);
            cmd.CommandText = BatchConstants.CopyrightNotice + BatchConstants.CustomCounterNotice +
                              BatchConstants.BatchHeader;
            cmd.CommandText +=
                String.Format(BatchFinder.CustomCounterOS(ver),
                              spOAContext,
                              def.ObjectName,
                              instanceName,
                              def.CounterName,
                              customTimeSpan,
                              disableOle ? 1 : 0);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }


        internal static SqlCommand BuildCustomCounterSQLCommand(SqlConnection connection, ServerVersion ver,
                                                                CustomCounterDefinition def, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = BatchConstants.CopyrightNotice + BatchConstants.CustomCounterNotice +
                              BatchConstants.BatchHeader;
            cmd.CommandText +=
                String.Format(BatchFinder.CustomCounterSQL(ver, cloudProviderId), def.ObjectName, def.CounterName, def.InstanceName, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildCustomCounterSQLStatementCommand(SqlConnection connection, ServerVersion ver,
                                                                         CustomCounterDefinition def)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = BatchConstants.CustomCounterSQLStatementNotice;
            cmd.CommandText += def.SqlStatement;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildDatabaseConfigurationCommand(SqlConnection connection, ServerVersion ver,
                                                                     DatabaseProbeConfiguration config, int? cloudProviderId)
        {
            StringBuilder filterStatements = new StringBuilder();
            if (!config.IncludeSystemDatabases)
            {
                filterStatements.Append(ver.Major >= 9 // SQL Server 2005/2008
                                            ? BatchConstants.ExcludeSystemDatabasesSysDatabases2005
                                            : BatchConstants.ExcludeSystemDatabases);
            }

            if (config.DatabaseNameFilter != null && config.DatabaseNameFilter.Length > 0)
            {
                filterStatements.Append(" and lower(name) = lower('");
                filterStatements.Append(config.DatabaseNameFilter.Replace("'", "''"));
                filterStatements.Append("') ");
            }

            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += String.Format(BatchFinder.DatabaseConfiguration(ver, cloudProviderId), filterStatements);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildFileGroupCommand(SqlConnection connection, ServerVersion ver, DatabaseProbeConfiguration config,
                                                            WmiConfiguration wmiConfiguration, DiskCollectionSettings disksettings,
                                                             object driveProbeData,int? cloud_provider_id=null)
        {
            var spOAContext = (int)wmiConfiguration.OleAutomationContext;
            LogSpOAContext(connection.DataSource, "DiskDrives", spOAContext);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = BuildDiskDrivesString(connection, ver, wmiConfiguration, disksettings, driveProbeData,cloud_provider_id);
            cmd.CommandText += String.Format(BatchFinder.FileGroup(ver));
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildDatabaseFilesCommand(SqlConnection connection,
                                                             ServerVersion ver,
                                                             DatabaseFilesConfiguration config,
                                                             WmiConfiguration wmiConfiguration,
                                                             DiskCollectionSettings disksettings,
                                                             object driveProbeData,int? cloud_provider_id=null)
        {
            var spOAContext = (int)wmiConfiguration.OleAutomationContext;
            LogSpOAContext(connection.DataSource, "DiskDrives", spOAContext);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = BuildDiskDrivesString(connection, ver, wmiConfiguration, disksettings, driveProbeData,cloud_provider_id);
            if (config.DatabaseNames.Count > 0)
            {
                StringBuilder databaseIncludeString = new StringBuilder();
                foreach (string database in config.DatabaseNames)
                {
                    databaseIncludeString.Append(",'");
                    databaseIncludeString.Append(database.Replace("'", "''"));
                    databaseIncludeString.Append("'");
                }
                databaseIncludeString.Remove(0, 1);

                cmd.CommandText += String.Format(BatchFinder.DatabaseFiles(ver), databaseIncludeString);
                if (ver.Major >= 9 && config.DatabaseNames.Count == 1 && config.DatabaseNames[0] == "tempdb")
                {
                    cmd.CommandText += BatchFinder.TempdbSummary(ver, cloud_provider_id);
                }
            }
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildDatabaseSizeCommand(SqlConnection connection,
                                                            ServerVersion ver,
                                                            WmiConfiguration wmiConfiguration,
                                                            DiskCollectionSettings diskSettings,
                                                            object driveProbeData,int? cloud_provider_id = null)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            if (cloud_provider_id == 2)
            {
                cmd.CommandText += BatchFinder.DatabaseSize(ver, cloud_provider_id);
                return cmd;
            }

            var spOAContext = (int)wmiConfiguration.OleAutomationContext;
            LogSpOAContext(connection.DataSource, "DiskDrives", spOAContext);
            cmd.CommandText = BuildDiskDrivesString(connection, ver, wmiConfiguration, diskSettings, driveProbeData, cloud_provider_id);
            //START SQLdm 9.1 (Ankit Srivastava)-Filegroup and Mount Point Monitoring Improvements - format databasesize batch Append DiskDriveStats and FileGroup batches
            var driveProbeObject = driveProbeData as DriveStatisticsWmiProbe.DriveStatisticsWmiDetails;

            cmd.CommandText += String.Format(BatchFinder.DatabaseSize(ver, cloud_provider_id),
                (driveProbeObject != null && !String.IsNullOrEmpty(driveProbeObject.DiskSqlFilter)) ? " where " + driveProbeObject.DiskSqlFilter : " where len(drive_letter) > 1 ",
                (driveProbeObject != null && !String.IsNullOrEmpty(driveProbeObject.DiskSqlFilter)) ? " where " + driveProbeObject.DiskSqlFilter.Replace("'", "''") : String.Empty,
                (driveProbeObject != null && !String.IsNullOrEmpty(driveProbeObject.DiskSqlFilter)) ? " and " + driveProbeObject.DiskSqlFilter : String.Empty);

            cmd.CommandText += String.Format(BatchFinder.DiskDriveStatistics(ver));
            cmd.CommandText += String.Format(BatchFinder.FileGroup(ver));
            //END SQLdm 9.1 (Ankit Srivastava)-Filegroup and Mount Point Monitoring Improvements - format databasesize batch Append DiskDriveStats and FileGroup batches
            return cmd;
        }

        internal static SqlCommand BuildDatabaseSummaryCommand(SqlConnection connection, ServerVersion ver,
                                                               bool includeSummaryData, string databaseNameFilter,
                                                               bool includeSystemDatabases,
                                                               WmiConfiguration wmiConfiguration,
                                                               DiskCollectionSettings diskSettings,
                                                               object driveProbeData,int? cloud_provider_id=null)
        {
            SqlCommand cmd = connection.CreateCommand();
            StringBuilder filterStatements = new StringBuilder();
            if (cloud_provider_id == AZURE_CLOUD_ID)
            {
                cmd.CommandText += BatchFinder.DatabaseSummary(ver, cloud_provider_id);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = SqlHelper.CommandTimeout;
                return cmd;
            }

            if (!includeSystemDatabases)
            {
                filterStatements.Append(BatchConstants.ExcludeSystemDatabases);
            }

            if (databaseNameFilter != null && databaseNameFilter.Length > 0)
            {
                filterStatements.Append(" and lower(name) = lower('");
                filterStatements.Append(databaseNameFilter.Replace("'", "''"));
                filterStatements.Append("') ");
            }
            cmd.CommandText = BuildDiskDrivesString(connection, ver, wmiConfiguration, diskSettings, driveProbeData,cloud_provider_id);
            if (ver.IsGreaterThanSql2008Sp1R2())
            {
                cmd.CommandText += String.Format(BatchFinder.DatabaseSummary(ver, cloud_provider_id),
                                                filterStatements,
                                                includeSummaryData
                                                    ? BatchConstants.SessionCountEnabled
                                                    : BatchConstants.SessionCountDisabled,
                                                      cloud_provider_id != 1 
                                                    ? includeSummaryData 
                                                    ? BatchConstants.OldestOpenTransaction : "" : "");
            }
            else
            {
               cmd.CommandText += String.Format(BatchFinder.DatabaseSummary(ver,cloud_provider_id),
                                              filterStatements,
                                              includeSummaryData
                                                  ? BatchConstants.SessionCountEnabled
                                                  : BatchConstants.SessionCountDisabled,
                                                    cloud_provider_id != 1 
                                                  ? includeSummaryData 
                                                  ? BatchConstants.OldestOpenTransaction2000 : "" : "");
            }
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildDatabaseAlwaysOnStatisticsCommand(SqlConnection connection, ServerVersion ver)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.DatabaseAlwaysOnStatistics(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildDatabaseAlwaysOnTopologyCommand(SqlConnection connection, ServerVersion ver)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.DatabaseAlwaysOnTopology(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildDiskDrivesCommand(SqlConnection connection, ServerVersion ver, WmiConfiguration wmiConfiguration, DiskCollectionSettings diskSettings, object driveProbeData,int? cloud_provider_id=null)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = BuildDiskDrivesString(connection, ver, wmiConfiguration, diskSettings, driveProbeData,cloud_provider_id);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }


        // Certain versions of Windows have difficulty with certain counters
        // If direct WMI is not an option, use pre-cooked counters
        private static string CookedDiskDriveSegment(DiskCollectionSettings diskSettings)
        {
            try
            {

                TimeSpan diskDriveDelay = TimeSpan.FromSeconds(0);
                if (diskSettings.CookedDiskDriveWaitTimeInSeconds.HasValue)
                    diskDriveDelay = TimeSpan.FromSeconds(diskSettings.CookedDiskDriveWaitTimeInSeconds.Value);
                else
                    diskDriveDelay = TimeSpan.FromSeconds(
                        CollectionServiceConfiguration.GetCollectionServiceElement().CookedDiskDriveWaitTimeInSeconds > 0
                            ? CollectionServiceConfiguration.GetCollectionServiceElement().CookedDiskDriveWaitTimeInSeconds
                            : 0);
                if (diskDriveDelay.TotalSeconds > 0)
                {
                    return String.Format(BatchConstants.CookedDiskDriveSegment, diskDriveDelay);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private static string BuildDiskDrivesString(SqlConnection connection, ServerVersion ver, WmiConfiguration wmiConfiguration, DiskCollectionSettings diskSettings,int? cloud_provider_id=null)
        {
            return BuildDiskDrivesString(connection, ver, wmiConfiguration, diskSettings, null,cloud_provider_id);
        }


        private static string BuildDiskDrivesString(SqlConnection connection, ServerVersion ver, WmiConfiguration wmiConfiguration, DiskCollectionSettings diskSettings, object driveProbeData,int? cloud_provider_id=null)
        {
            // The cooked disk drive segment is optional, based on a config value
            string cookedString = CookedDiskDriveSegment(diskSettings);

            var spOAContext = (int)wmiConfiguration.OleAutomationContext;

            // Build the size string
            string diskSize = BuildDiskSizeString(connection, ver, diskSettings, driveProbeData,cloud_provider_id);
            var driveProbeObject = driveProbeData as DriveStatisticsWmiProbe.DriveStatisticsWmiDetails;

            string diskstats = String.Format(BatchFinder.DiskDrives(ver),
                                 spOAContext,
                                 wmiConfiguration.OleAutomationDisabled ? 1 : 0,
                                 cookedString,
                //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - Adding required disk filter for previously implemented collection
                                 (driveProbeObject != null && !String.IsNullOrEmpty(driveProbeObject.DiskSqlFilter)) ? BatchConstants.WhereConnector + driveProbeObject.DiskSqlFilter : String.Empty);

            return diskSize + diskstats;
        }

        private static string BuildDiskSizeString(SqlConnection connection, ServerVersion ver, DiskCollectionSettings diskSettings, object driveProbeData,int? cloud_provider_id=null)
        {
            // First, try to use WMI data if any to build the drive list
            string specifiedDisk = BuildDriveStatisticsInserts(connection, (driveProbeData as DriveStatisticsWmiProbe.DriveStatisticsWmiDetails),cloud_provider_id, ver);

            // If the WMI data is not available, use xp_fixeddrives from the batch
            // NOTE: xp_fixeddrives does not capture mount points!
            if (String.IsNullOrEmpty(specifiedDisk))
                specifiedDisk = BatchFinder.DiskSize(ver);

            if (cloud_provider_id == AMAZON_CLOUD_ID)
            {
                specifiedDisk += String.Format(BatchConstants.SpecifiedDiskSizeRDS);
            }
            //SQLDM-30012.Change DiskCollection statistics to Instance Specific.
            //   Get the diskDriveOption from the Configuration file.
            if (cloud_provider_id != AZURE_CLOUD_ID)
            {
                string diskDriveOption = CollectionServiceConfiguration.GetCollectionServiceElement().DiskDriveOption;

                if (!diskDriveOption.Equals("Default"))
                {
                    if(ver.Major >= 9)
                    {
                        specifiedDisk += BatchConstants.DeleteOtherDrives;
                    }
                    else
                    {
                        specifiedDisk += BatchConstants.DeleteOtherDrives2000;
                    }
                }
            }

            // Next consider any manually added mount points or other drives and returnAll is false hence DiskSqlFilter is ampty //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - 
            string mountPointString = MountPointSegment(ver, diskSettings);
            bool isDriveFilterSet = driveProbeData != null
                                     ? !String.IsNullOrEmpty((driveProbeData as DriveStatisticsWmiProbe.DriveStatisticsWmiDetails).DiskSqlFilter)
                                     : true;

            if (!String.IsNullOrEmpty(mountPointString) && isDriveFilterSet)
            {
                specifiedDisk += String.Format(BatchConstants.SpecifiedDiskDrivesSegment, mountPointString, ver.MasterDatabaseCompatibility > 7 ? " collate database_default " : "");
            }

            // Finally, combine with the header
            return String.Format(BatchFinder.DiskDriveHeader(ver),
                                specifiedDisk);
        }


        /// <summary>
        /// The data and log full alerts are calculated in part based on the size of disks.
        /// To avoid moving all of that alert logic from the batch to the service in the course of a hotfix, the data from direct WMI must be inserted for the batch to use.
        /// To complicate matters, though, inserting the values directly into the batch causes a pileup of batches into plan cache as the available disk values change over time.
        /// By putting this data into tempdb it is available for use in the big batch without causing a plan cache miss.
        /// </summary>
        private static string BuildDriveStatisticsInserts(SqlConnection connection, DriveStatisticsWmiProbe.DriveStatisticsWmiDetails driveProbeData,int? cloud_provider_id=null, ServerVersion ver=null)
        {
            using (LOG.DebugCall("BuildDriveStatisticsInserts"))
            {
                try
                {
                    if (driveProbeData == null || driveProbeData.DiskMap == null || driveProbeData.DiskMap.Count == 0) return String.Empty;

                    List<string> drives = null;
                    if(cloud_provider_id!=AZURE_CLOUD_ID)
                       drives= GetDrives(connection, ver);

                    var sb = new StringBuilder();

                    sb.Append(BatchConstants.InsertDiskDrive);
                    foreach (var drive in driveProbeData.DiskMap.Values)
                    {
                        if (drive.DriveType == 4 || drive.DriveType == 5) continue;
                        foreach (var mountPath in drive.Paths)
                        {
                            if (cloud_provider_id != AZURE_CLOUD_ID)
                            {
                                if (!drives.Contains(mountPath))
                                {
                                    continue;
                                }
                            }
                            sb.AppendFormat(
                                "insert into tempdb..Disks (drive_letter,unused_size,total_size, hostname) values(''{0}'',{1},{2}, host_name())",
                                mountPath.TrimEnd(new Char[] { '\\' }).TrimEnd(new Char[] { ':' }),
                                drive.FreeSpace,
                                drive.TotalSize
                                );
                            sb.AppendLine();
                        }
                    }
                    sb.Append("' with recompile");  // This is critical to prevent plan cache pileups
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = sb.ToString();
                        LOG.Verbose("Executing: ", cmd.CommandText);
                        cmd.ExecuteNonQuery();
                    }

                    sb = new StringBuilder();

                    sb.AppendLine("truncate table tempdb..#disk_drives");
                    sb.AppendLine("insert into tempdb..#disk_drives(drive_letter,unused_size,total_size) select drive_letter,unused_size,total_size from tempdb..Disks where hostname = host_name()");
                    LOG.DebugFormat("Build batch Drive Disk: {0} ", sb.ToString());

                    return sb.ToString();
                }
                catch (Exception e)
                {
                    LOG.Error("Exception in BuildDriveStatisticsInserts", e);
                    return String.Empty;
                }
            }
        }

        //SQLDM-30012.Change DiskCollection statistics to Instance Specific.
        //Get the disk drive option from the config file. Default value is Server.
        private static List<string> GetDrives(SqlConnection connection, ServerVersion ver)
        {
            
            string diskDriveoption = CollectionServiceConfiguration.GetCollectionServiceElement().DiskDriveOption;
            List<String> drives = new List<String>();

            if (!diskDriveoption.Equals("Default"))
            {
                var getDriveCmd = new StringBuilder();

                if(ver.Major >= 9)
                {
                    getDriveCmd.Append(BatchConstants.GetInstanceDrives);
                }
                else 
                {
                    getDriveCmd.Append(BatchConstants.GetInstanceDrives2005);
                }

                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = getDriveCmd.ToString();
                    using (SqlDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            string drive = dataReader.GetString(0);
                            drives.Add(drive);
                        }
                        LOG.Verbose("Executing: ", cmd.CommandText);
                    }
                }

            }
            return drives;
        }

        internal static SqlCommand BuildDistributorQueueCommand(SqlConnection connection, ServerVersion ver,
                                                                DistributorQueueConfiguration config)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += String.Format(
                BatchFinder.DistributorQueue(ver),
                (config.DistributionDatabase ?? "").Replace("]", "]]"),
                config.FilterTimeSpan.HasValue ? config.FilterTimeSpan.Value.TotalSeconds : 0,
                config.PublisherName,
                (config.Publication ?? "").Replace("'", "''"),
                config.SubscriberName ?? "",
                (config.SubscriberDatabase ?? "").Replace("'", "''")
                );
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildDropPlansFromCacheCommand(SqlConnection connection, ServerVersion ver)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.DropPlansFromCache(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildDistributorDetailsPermissionsCommand(SqlConnection connection,
            ServerVersion ver,
            DistributorDetailsConfiguration config)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += String.Format(
                BatchFinder.DistributorDetailsPermissions(ver),
                (config.DistributionDatabase ?? "").Replace("]", "]]"));
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildDistributorDetailsCommand(SqlConnection connection, ServerVersion ver,
                                                                DistributorDetailsConfiguration config)
        {
            SqlCommand cmd = connection.CreateCommand();
            switch (config.ReplicationType)
            {
                case Common.Objects.Replication.ReplicationType.Snapshot:
                case Common.Objects.Replication.ReplicationType.Transaction:
                    cmd.CommandText += String.Format(
                        BatchFinder.DistributorDetails(ver),
                        (config.DistributionDatabase ?? "").Replace("]", "]]"),
                        config.PublisherName,
                        (config.Publication ?? "").Replace("'", "''"),
                        (config.SubscriptionDatabase ?? "").Replace("'", "''"),
                        config.SubscriptionServer ?? "");
                    break;
                case Common.Objects.Replication.ReplicationType.Merge:
                    cmd.CommandText = BatchConstants.CopyrightNotice + BatchConstants.CustomCounterNotice +
                              BatchConstants.BatchHeader;
                    cmd.CommandText += "set transaction isolation level read committed "
                        + string.Format(BatchConstants.MergeSubscriptions,
                        (config.DistributionDatabase ?? "").Replace("]", "]]"),
                        config.PublisherName,
                        (config.PublicationDatabase ?? "").Replace("'", "''''"),
                        (config.Publication ?? "").Replace("'", "''''"));
                    break;
            }
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildDTCStatusCommand(SqlConnection connection, ServerVersion ver,
                                                         SqlConnectionInfo connectionInfo, WmiConfiguration wmiConfig, ClusterCollectionSetting clusterCollectionSetting, bool hasServiceControlExecuteAccess, int? cloudProviderId)
        {
            var spOAContext = (int)wmiConfig.OleAutomationContext;
            LogSpOAContext(connection.DataSource, "DTCStatus", spOAContext);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = string.Format(BatchFinder.DTCStatus(ver, hasServiceControlExecuteAccess, cloudProviderId),
                                            spOAContext,
                                            wmiConfig.OleAutomationDisabled ? 1 : 0,
                                            clusterCollectionSetting > 0 ? BatchConstants.ClusterSettingFalse : "",
                                            wmiConfig.DirectWmiEnabled ? 1 : 0);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        private static string MountPointSegment(ServerVersion ver, DiskCollectionSettings diskSettings)
        {
            string returnString = null;
            if (diskSettings != null && diskSettings.AutoDiscover == false)
            {
                StringBuilder sb = new StringBuilder();

                if (diskSettings.Drives != null && diskSettings.Drives.Length > 0)
                {
                    foreach (string disk in diskSettings.Drives)
                    {
                        sb.Append(
                            String.Format("union select '{0}' ",
                                          disk.TrimEnd(new Char[] { '\\' }).TrimEnd(new Char[] { ':' }).Replace("'", "''")));
                    }
                    if (sb.Length > 6)
                        sb.Remove(0, 6);
                }
                else
                {
                    sb.Append("select 'No Drives Configured'");
                }

                returnString = sb.ToString();

            }
            else if (!String.IsNullOrEmpty(CollectionServiceConfiguration.GetCollectionServiceElement().MountPointPath))
            {
                LOG.Verbose("Mount point path(s) detected in config file: " + CollectionServiceConfiguration.GetCollectionServiceElement().MountPointPath);
                string whereSegment = "";
                string whereNotSegment = "";
                foreach (string s in CollectionServiceConfiguration.GetCollectionServiceElement().MountPointPath.Split(';'))
                {
                    if (s.Length > 0)
                    {
                        string path = s.Trim().TrimEnd('\\');
                        whereSegment += " or " + String.Format(ver.Major >= 9 ? BatchConstants.MountPointPathWhereSegment2005 : BatchConstants.MountPointPathWhereSegment2000, path, "");
                        whereNotSegment += " or " +
                            String.Format(ver.Major >= 9 ? BatchConstants.MountPointPathWhereSegment2005 : BatchConstants.MountPointPathWhereSegment2000, path, " not ");
                    }
                }
                whereSegment = whereSegment.Length > 4 ? whereSegment.Remove(0, 4) : whereSegment;
                whereNotSegment = whereNotSegment.Length > 4 ? whereNotSegment.Remove(0, 4) : whereNotSegment;

                returnString = String.Format(ver.Major >= 9 ? BatchConstants.MountPointPathSegment2005 : BatchConstants.MountPointPathSegment2000,
                                                            whereSegment, whereNotSegment);
            }

            return returnString;
        }

        internal static string BuildFileActivity(ServerVersion ver, WmiConfiguration wmiConfiguration, int? cloudProviderId = null)
        {
            return String.Format(
                BatchFinder.FileActivity(ver, cloudProviderId),
                1, // Autodiscover Drives
                @"select 1",// Specified Drives
                cloudProviderId==AZURE_CLOUD_ID?1:wmiConfiguration.OleAutomationDisabled ? 1 : 0,//SQLdm 10.0 (Tarun Sapra)- If sql server is hosted on azure instance, then disable the ole
                (int)wmiConfiguration.OleAutomationContext,
                wmiConfiguration.DirectWmiEnabled ? 1 : 0);
        }

        internal static SqlCommand BuildFileActivityCommand(SqlConnection connection, ServerVersion ver, WmiConfiguration wmiConfiguration, DiskCollectionSettings diskSettings)
        {
            SqlCommand cmd = connection.CreateCommand();

            string mountPointString = MountPointSegment(ver, diskSettings);
            string specifiedDisk = !String.IsNullOrEmpty(mountPointString) ? String.Format(BatchConstants.FileActivitySpecifiedDrivesSegment, mountPointString) : @"select 1";

            cmd.CommandText += String.Format(
                BatchFinder.FileActivity(ver),
                String.IsNullOrEmpty(mountPointString) ? 1 : 0, // Autodiscover Drives
                specifiedDisk,// Specified Drives
                wmiConfiguration.OleAutomationDisabled ? 1 : 0,
                (int)wmiConfiguration.OleAutomationContext,
                wmiConfiguration.DirectWmiEnabled ? 1 : 0);

            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildFullTextCatalogsCommand(SqlConnection connection, ServerVersion ver)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.FullTextCatalogs(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildFullTextColumnsCommand(SqlConnection connection, ServerVersion ver,
                                                               FullTextColumnsConfiguration config)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += String.Format(BatchFinder.FullTextColumns(ver),
                                             config.DatabaseName != null ? config.DatabaseName.Replace("]", "]]") : "",
                                             config.TableId.HasValue ? config.TableId.Value : -1);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildFullTextSearchServiceCommand(SqlConnection connection, ServerVersion ver,
                                                                     WmiConfiguration wmiConfig,
                                                                     ClusterCollectionSetting clusterCollectionSetting, bool hasServiceControlExecuteAccess)
        {
            var spOAContext = (int)wmiConfig.OleAutomationContext;
            LogSpOAContext(connection.DataSource, "FullTextSearch", spOAContext);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = string.Format(BatchFinder.FullTextSearchService(ver, hasServiceControlExecuteAccess),
                                            spOAContext,
                                            wmiConfig.OleAutomationDisabled ? 1 : 0,
                                            (int)clusterCollectionSetting > 0 ? BatchConstants.ClusterSettingFalse : "",
                                            wmiConfig.DirectWmiEnabled ? 1 : 0);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildFullTextTablesCommand(SqlConnection connection, ServerVersion ver,
                                                              FullTextTablesConfiguration config)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += String.Format(BatchFinder.FullTextTables(ver),
                                             config.DatabaseName != null ? config.DatabaseName.Replace("]", "]]") : "",
                                             config.CatalogName != null ? config.CatalogName.Replace("'", "''") : "",
                                             ver.Major >= 9 ? "schema_name" : "user_name");
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildIndexStatisticsCommand(SqlConnection connection, ServerVersion ver,
                                                               IndexStatisticsConfiguration config)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText +=
                String.Format(BatchFinder.IndexStatistics(ver),
                              config.DatabaseName != null ? config.DatabaseName.Replace("]", "]]") : "",
                              config.TableId,
                              config.IndexName.Replace("'", "''"),
                              ver.Major >= 9 ? "schema_name" : "user_name");
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }


        internal static SqlCommand BuildErrorLogCommand(SqlConnection connection, ServerVersion ver,
                                                        ErrorLogConfiguration config)
        {
            SqlCommand cmd = connection.CreateCommand();

            if (ver.Major == 8)
            {
                cmd.CommandText += BatchConstants.AgentLog2000Header;
            }
            else
            {
                cmd.CommandText += BatchConstants.SqlErrorLog2005Header;
            }

            foreach (LogFile logFile in config.LogFiles)
            {
                if (logFile.LogType.HasValue && logFile.LogType.Value == LogFileType.SQLServer)
                {
                    if (ver.Major == 8)
                    {
                        cmd.CommandText += String.Format(BatchConstants.SqlErrorLog2000,
                                                         logFile.Number.HasValue && logFile.Number.Value > 0
                                                             ? logFile.Number.Value.ToString()
                                                             : "");
                    }
                    else
                    {
                        cmd.CommandText += String.Format(BatchConstants.SqlErrorLog2005,
                                                         logFile.Number.HasValue && logFile.Number.Value > 0
                                                             ? logFile.Number.Value.ToString()
                                                             : "",
                                                         config.StartDate.HasValue
                                                             ? "'" + config.StartDate.Value.ToString("s") + "'"
                                                             : "'1900-01-01 00:00:00 AM'",
                                                         config.EndDate.HasValue
                                                             ? "'" + config.EndDate.Value.ToString("s") + "'"
                                                             : "GetDate()");
                    }
                }
                if (logFile.LogType.HasValue && logFile.LogType.Value == LogFileType.Agent)
                {
                    if (ver.Major == 8)
                    {
                        cmd.CommandText += (String.Format(BatchConstants.AgentLog2000Segment, logFile.Name));
                    }
                    else
                    {
                        cmd.CommandText += (String.Format(BatchConstants.AgentLog2005,
                                                          logFile.Number,
                                                          config.StartDate.HasValue
                                                              ? "'" + config.StartDate.Value.ToString("s") + "'"
                                                              : "'1900-01-01 00:00:00 AM'",
                                                          config.EndDate.HasValue
                                                              ? "'" + config.EndDate.Value.ToString("s") + "'"
                                                              : "GetDate()"));
                    }
                }
            }

            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        internal static SqlCommand BuildRDSErrorLogCommand(SqlConnection connection, ServerVersion ver,
                                                    ErrorLogConfiguration config)
        {
            SqlCommand cmd = connection.CreateCommand();

                cmd.CommandText += BatchConstants.RDSSqlErrorLog2005Header;
            foreach (LogFile logFile in config.LogFiles)
            {
                if (logFile.LogType.HasValue && logFile.LogType.Value == LogFileType.SQLServer)
                {
                 
                 
                 
                        cmd.CommandText += String.Format(BatchConstants.RDSSqlErrorLog2005,
                                                         logFile.Number.HasValue && logFile.Number.Value > 0
                                                             ? logFile.Number.Value.ToString()
                                                             : "",
                                                         config.StartDate.HasValue
                                                             ? "'" + config.StartDate.Value.ToString("s") + "'"
                                                             : "'1900-01-01 00:00:00 AM'",
                                                         config.EndDate.HasValue
                                                             ? "'" + config.EndDate.Value.ToString("s") + "'"
                                                             : "GetDate()");
                   
                }
                if (logFile.LogType.HasValue && logFile.LogType.Value == LogFileType.Agent)
                {
                
                        cmd.CommandText += (String.Format(BatchConstants.RDSAgentLog2005,
                                                          logFile.Number,
                                                          config.StartDate.HasValue
                                                              ? "'" + config.StartDate.Value.ToString("s") + "'"
                                                              : "'1900-01-01 00:00:00 AM'",
                                                          config.EndDate.HasValue
                                                              ? "'" + config.EndDate.Value.ToString("s") + "'"
                                                              : "GetDate()"));
                   
                }
            }

            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        internal static SqlCommand BuildLogListCommand(SqlConnection connection, ServerVersion ver, MonitoredServerWorkload workload) // SQLdm 8.6 (Ankit Srivastava) -- added new parameter - solved defect DE43661
        {
            string productEdition = (workload.MonitoredServer.ConnectionInfo != null && workload.MonitoredServer.ConnectionInfo.ConnectionString != null) ? CollectionHelper.GetProductEdition(workload.MonitoredServer.ConnectionInfo.ConnectionString) : string.Empty; // SQLdm 8.6 (Ankit Srivastava) -- getting Product Edition - solved defect DE43661
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += String.Format(BatchFinder.LogList(ver),
                productEdition.IndexOf("Express", StringComparison.OrdinalIgnoreCase) > -1 ? 1 : 0); //SQLdm 8.6 (Ankit Srivastava) -- seeting skipAgentLog palceholder if the product Edition is express
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildLogScanVariables(SqlConnection connection,
                                                       ScheduledRefresh previousRefresh,
                                                       ScheduledRefresh refresh,
                                                       int? cloudProviderId)
        {
            // Determine start point for this refresh
            DateTime? startDate = null;
            if (previousRefresh != null && previousRefresh.Alerts.LogScanDate.HasValue)
            {
                startDate = previousRefresh.Alerts.LogScanDate;
            }
            else
            {
                startDate = PersistenceManager.Instance.GetLogScanDate();
            }

            // This should only apply for the very first collection
            // or after a cache clear
            if (!startDate.HasValue)
                startDate = refresh.TimeStampLocal;

            // Set start point for next refresh
            refresh.Alerts.LogScanDate = refresh.TimeStampLocal;

            SqlCommand cmd = connection.CreateCommand();

            cmd.CommandText += BatchConstants.CopyrightNotice + BatchConstants.BatchHeader +
                String.Format( cloudProviderId.HasValue && (cloudProviderId == AZURE_CLOUD_ID) ? BatchConstants.LogScanVariableSegmentUnsupportedTempdb : BatchConstants.LogScanVariableSegment,
                            startDate.Value.ToString("yyyy'-'MM'-'dd HH':'mm':'ss")
                            );
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildLogScanCommand(SqlConnection connection, ServerVersion ver,
                                                       MonitoredServerWorkload workload,
                                                       int? cloudProviderId)
        {

            SqlCommand cmd = connection.CreateCommand();
            //--	Variables:	
            //--	[0] - Rowcount max to return (flood control)
            //--	[1] - Enumeration to return for Agent Logs
            //--	[2] - Enumeration to return for SQL Logs
            //--	[3] - Skip error log (1 if true)
            //--	[4] - Skip agent log (1 if true)
            //--    [5] - Error Log Size Limit
            //--    [6] - Agent Log Size Limit

            //Get the advanced config settings
            var errorLogConfig = ScheduledCollectionEventProcessor.GetAdvancedConfiguration(workload.Thresholds[(int)Metric.ErrorLog]);
            var agentLogConfig = ScheduledCollectionEventProcessor.GetAdvancedConfiguration(workload.Thresholds[(int)Metric.AgentLog]);
            string productEdition = (workload.MonitoredServer.ConnectionInfo != null && workload.MonitoredServer.ConnectionInfo.ConnectionString != null) ? CollectionHelper.GetProductEdition(workload.MonitoredServer.ConnectionInfo.ConnectionString) : string.Empty;//getting the product edition. Gaurav Karwal SQLdm 8.6

            if (ver.Major >= 9)
            {
                cmd.CommandText +=
                    String.Format(BatchFinder.LogScan(ver, cloudProviderId),
                                  CollectionServiceConfiguration.GetCollectionServiceElement().MaxRowCountLogScan,
                                  (int)LogFileType.Agent,
                                  (int)LogFileType.SQLServer,
                                  workload.GetMetricThresholdEnabled(Metric.ErrorLog) ? 0 : 1,
                                  (productEdition.IndexOf("Express", StringComparison.OrdinalIgnoreCase) > -1) ? 1 : (workload.GetMetricThresholdEnabled(Metric.AgentLog) ? 1 : 0),//setting agent log collecton flag only when non express. Gaurav Karwal SQLdm 8.6.
                                  errorLogConfig.LogSizeLimit * 1024,
                                  agentLogConfig.LogSizeLimit * 1024);
            }
            else
            {
                cmd.CommandText +=
                    String.Format(BatchFinder.LogScan(ver, cloudProviderId),
                                  CollectionServiceConfiguration.GetCollectionServiceElement().MaxRowCountLogScan,
                                  (int)LogFileType.Agent,
                                  (int)LogFileType.SQLServer,
                                  workload.GetMetricThresholdEnabled(Metric.ErrorLog) ? 0 : 1,
                                  workload.GetMetricThresholdEnabled(Metric.AgentLog) ? 0 : 1,
                                  errorLogConfig.LogSizeLimit * 1024,
                                  agentLogConfig.LogSizeLimit * 1024);
            }
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildLockDetailsCommand(SqlConnection connection, ServerVersion ver,
                                                           LockDetailsConfiguration config, int? cloudProviderId = null)
        {
            SqlCommand cmd = connection.CreateCommand();

            // Set up tempdb filter
            StringBuilder tempDbFilter = new StringBuilder();
            if (!config.ShowTempDbLocks)
            {
                tempDbFilter.Append(ver.Major > 8
                                        ? BatchConstants.TempdbLockFilter2005
                                        : BatchConstants.TempdbLockFilter2000);
            }

            if (!config.ShowSharedLocks)
            {
                tempDbFilter.Append(ver.Major > 8
                                        ? BatchConstants.HideSharedLocks2005
                                        : BatchConstants.HideSharedLocks2000);
            }


            // Set up blocking filter
            string blockingFilter = "";
            if (config.FilterForBlocking && config.FilterForBlocked)
            {
                blockingFilter =
                    ver.Major > 8
                        ? BatchConstants.FilterForBlockedandBlocking2005
                        : BatchConstants.FilterForBlockedandBlocking2000;
            }
            else
            {
                if (config.FilterForBlocking)
                {
                    blockingFilter =
                    ver.Major > 8
                        ? BatchConstants.FilterForBlocking2005
                        : BatchConstants.FilterForBlocking2000;
                }

                if (config.FilterForBlocked)
                {
                    blockingFilter =
                    ver.Major > 8
                        ? BatchConstants.FilterForBlocked2005
                        : BatchConstants.FilterForBlocked2000;
                }
            }

            // Set up spid filter
            string spidFilter = "";
            if (config.SpidFilter.HasValue)
            {
                spidFilter = String.Format(
                    ver.Major > 8
                    ? BatchConstants.FilterForSpid2005
                    : BatchConstants.FilterForSpid2000
                    , config.SpidFilter);
            }

            // Append blocking filter and spid filter
            if (ver.Major > 8)
            {
                cmd.CommandText += String.Format(BatchFinder.LockDetails(ver, cloudProviderId),
                                                 tempDbFilter,
                                                 spidFilter,
                                                 blockingFilter,
                                                 BatchFinder.LockCounterStatistics(ver, cloudProviderId),
                                                 CollectionServiceConfiguration.GetCollectionServiceElement().
                                                     MaxRowCountLockDetails,
                                                 ver.MasterDatabaseCompatibility < 90
                                                     ? BatchConstants.LockDetailsSqlTextCompatibilityMode80
                                                     : BatchConstants.LockDetailsSqlTextCompatibilityMode90);
            }
            else
            {
                cmd.CommandText += String.Format(BatchFinder.LockDetails(ver, cloudProviderId),
                                                 tempDbFilter,
                                                 spidFilter,
                                                 blockingFilter,
                                                 BatchFinder.LockCounterStatistics(ver, cloudProviderId),
                                                 CollectionServiceConfiguration.GetCollectionServiceElement().
                                                     MaxRowCountLockDetails);
            }
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        //internal static SqlCommand BuildMemoryCommand(SqlConnection connection, ServerVersion ver, bool includePinned)
        //{
        //    SqlCommand cmd = connection.CreateCommand();
        //    cmd.CommandText += BatchFinder.Memory(ver);
        //    if (ver.Major == 8 && includePinned)
        //    {
        //        cmd.CommandText += BatchFinder.PinnedTables(ver);
        //    }
        //    cmd.CommandType = CommandType.Text;
        //    cmd.CommandTimeout = SqlHelper.CommandTimeout;
        //    return cmd;
        //}

        internal static SqlCommand BuildMirrorMonitoringRealtimeCommand(SqlConnection connection, ServerVersion ver)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.MirrorMonitoringRealtime(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildMirrorMonitoringHistoryCommand(SqlConnection connection, ServerVersion ver,
                                                                       MirrorMonitoringHistoryConfiguration config)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += String.Format(BatchFinder.MirrorMonitoringHistory(ver),
                                             config.MirroredDatabaseName,
                                             config.Mode);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildMirrorDetailsCommand(SqlConnection connection, ServerVersion ver)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.MirrorDetails(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildMirrorMetricsUpdateCommand(SqlConnection connection, ServerVersion ver)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.MirrorMetricsUpdate(ver);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildProcedureCacheCommand(SqlConnection connection, ServerVersion ver,
                                                              ProcedureCacheConfiguration config, int? cloudProviderId)
        {
            int procCacheRowCount = CollectionServiceConfiguration.GetCollectionServiceElement().MaxRowCountProcedureCache;

            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += string.Format(BatchFinder.ProcedureCache(ver, cloudProviderId), procCacheRowCount);
            if (config.ShowProcedureCacheList)
            {
                cmd.CommandText += String.Format(BatchFinder.ProcedureCacheList(ver, cloudProviderId), procCacheRowCount);
            }
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        //internal static SqlCommand BuildResourceCommand(SqlConnection connection, ServerVersion ver)
        //{
        //    SqlCommand cmd = connection.CreateCommand();
        //    cmd.CommandText += BatchFinder.Resources(ver);
        //    cmd.CommandType = CommandType.Text;
        //    cmd.CommandTimeout = SqlHelper.CommandTimeout;
        //    return cmd;
        //}

        internal static SqlCommand BuildServicesCommand(SqlConnection connection, ServerVersion ver, WmiConfiguration wmiConfiguration, ClusterCollectionSetting clusterCollectionSetting, bool hasServiceControlExecuteAccess)
        {
            var spOAContext = (int)wmiConfiguration.OleAutomationContext;
            LogSpOAContext(connection.DataSource, "Services", spOAContext);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += string.Format(BatchFinder.Services(ver, hasServiceControlExecuteAccess),
                                            spOAContext,
                                            wmiConfiguration.OleAutomationDisabled ? 1 : 0,
                                            (int)clusterCollectionSetting > 0 ? BatchConstants.ClusterSettingFalse : "",
                                            wmiConfiguration.DirectWmiEnabled ? 1 : 0);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        internal static SqlCommand BuildSessionDetailsCommand(SqlConnection connection, ServerVersion ver, int spid,
                                                              bool traceOn,
                                                              Guid clientSessionId, TimeSpan traceLength,
                                                              TimeSpan restartTime,
                                                              int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();

            // Set up trace if necessary
            string trace = null;
            if (traceOn)
            {
                trace = BatchFinder.SessionDetailsTrace(ver, cloudProviderId);
                if (ver.Major == 8)
                {
                    trace =
                        String.Format(trace,
                                      spid,
                                      clientSessionId,
                                      Math.Floor(traceLength.TotalMinutes),
                                      Math.Floor(restartTime.TotalMinutes));
                }
                else
                {
                    trace = String.Format(trace,
                                          spid,
                                          clientSessionId,
                                          Math.Floor(traceLength.TotalMinutes),
                                          Math.Floor(restartTime.TotalMinutes),
                                          5,
                                          5);
                }
            }

            cmd.CommandText +=
                ver.Major == 8
                    ? String.Format(BatchFinder.SessionDetails(ver), spid, trace)
                    : String.Format(BatchFinder.SessionDetails(ver),
                                    spid,
                                    trace,
                                    ver.MasterDatabaseCompatibility < 90
                                        ? BatchConstants.SessionDetailsCompatibilityMode80
                                        : BatchConstants.SessionDetailsCompatibilityMode90);

            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildSessionSummaryCommand(SqlConnection connection, ServerVersion ver, string filter, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();

            string p1 = String.Empty;

            if (!String.IsNullOrEmpty(filter))
            {
                if (!filter.Contains("%"))
                    filter = "%" + filter + "%";
                p1 = String.Format(" p join sys.databases d on p.dbid = d.database_id " +
                                   "where spid like '{0}' " +
                                   "or hostname like '{0}' " +
                                   "or program_name like '{0}' " +
                                   "or loginame like '{0}' " +
                                   "or d.name like '{0}'", filter.Replace("'", "''"));
            }

            cmd.CommandText +=
                String.Format(BatchFinder.SessionSummary(ver, cloudProviderId),
                              BatchFinder.SessionCount(ver, p1,cloudProviderId),
                              BatchFinder.LockCounterStatistics(ver,cloudProviderId));
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildConnectionStatusCommand(SqlConnection connection)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "select 1;";
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildResponseTimeCommand(SqlConnection connection)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = "select 1;";
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        internal static SqlCommand BuildElasticPoolCommand(SqlConnection connection)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = BatchFinder.AzureElasticPool();
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildReplicationCommand(SqlConnection connection, ServerVersion ver, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BatchFinder.ReplicationCheck(ver, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildPermissionsCommand(SqlConnection connection, SqlConnectionInfo connectionInfo, int? cloudProviderId, bool replicationMonitoringDisabled = false)
        {
		    // Build Permissions Command Batch with Minimum / Metadata / Collector / Replication, etc related permissions
            SqlCommand cmd = connection.CreateCommand();
            ServerVersion ver = new ServerVersion(connection.ServerVersion);
            cmd.CommandText = BatchFinder.ReadMinimumPermissions(ver, cloudProviderId);
            cmd.CommandText += BatchFinder.ReadMetadataPermissions(ver, cloudProviderId);
            cmd.CommandText += BatchFinder.ReadCollectionPermissions(ver, cloudProviderId);
            if (!replicationMonitoringDisabled)
            {
                cmd.CommandText += BatchFinder.ReadReplicationPermissions(ver, cloudProviderId);
            }
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildMasterPermissionsCommand(SqlConnection connection, SqlConnectionInfo connectionInfo, int? cloudProviderId)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            // Build Permissions Command Batch with Minimum / Metadata / Collector / Replication, etc related permissions
            SqlCommand cmd = connection.CreateCommand();
            ServerVersion ver = new ServerVersion(connection.ServerVersion);
            cmd.CommandText = BatchFinder.ReadMasterPermissions(ver, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildServerBasicsCommand(SqlConnection connection, SqlConnectionInfo connectionInfo,
                                                            WmiConfiguration wmiConfiguration,
                                                            ClusterCollectionSetting clusterCollectionSetting, int? cloudProviderId = null)
        {
            var spOAContext = (int)wmiConfiguration.OleAutomationContext;
            LogSpOAContext(connection.DataSource, "ServerBasics", spOAContext);
            SqlCommand cmd = connection.CreateCommand();
            ServerVersion ver = new ServerVersion(connection.ServerVersion);
            if (cloudProviderId == null)
            {
                cmd.CommandText = string.Format(BatchFinder.ServerBasics(ver),
                                                spOAContext,
                                                wmiConfiguration.OleAutomationDisabled ? 1 : 0,
                                                (int)clusterCollectionSetting > 0 ? BatchConstants.ClusterSettingFalse : "",
                                                wmiConfiguration.DirectWmiEnabled ? 1 : 0);
            }
            //START: SQLdm 10.0 (Tarun Sapra): Minimal Cloud Support- In case of cloud dbs disable the ole
            else
            {
                cmd.CommandText = string.Format(BatchFinder.ServerBasics(ver,cloudProviderId),
                                                spOAContext,
                                                1,//wmiConfiguration.OleAutomationDisabled                                                
                                                (int)clusterCollectionSetting > 0 ? BatchConstants.ClusterSettingFalse : "",
                                                wmiConfiguration.DirectWmiEnabled ? 1 : 0); 
            }
            //END: SQLdm 10.0 (Tarun Sapra): Minimal Cloud Support- In case of cloud dbs disable the ole
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildServerDetailsCommand(SqlConnection connection,
                                                             ServerVersion ver,
                                                             string realServerName,
                                                             ScheduledRefresh previousRefresh,
                                                             MonitoredServerWorkload workload, int? cloudProviderId = null)
        {
            string resourceCheck = BuildResourceCheck(ver, previousRefresh, workload, cloudProviderId);
            string blockingCheck = BuildBlockingCheck(ver, workload, cloudProviderId);
            string memory = BatchFinder.Memory(ver, cloudProviderId);
            string waitStats = ver.Major > 8 ? BuildWaitStats(ver, cloudProviderId) : null;
            string tempdbSummary = ver.Major > 8 ? BatchFinder.TempdbSummary(ver, cloudProviderId) : null;


            SqlCommand cmd = connection.CreateCommand();

            if (ver.Major > 8)
            {
                cmd.CommandText = String.Format(BatchFinder.ServerDetails(ver,cloudProviderId),//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
                                              ProbeHelpers.GetSysperfinfoObjectPrefix(realServerName),
                                              BatchFinder.SessionCount(ver, cloudProviderId),
                                              resourceCheck,
                                              blockingCheck,
                                              memory,
                                              waitStats,
                                              tempdbSummary);

            }
            else
            {
                cmd.CommandText = String.Format(BatchFinder.ServerDetails(ver, cloudProviderId),
                                            ProbeHelpers.GetSysperfinfoObjectPrefix(realServerName),
                                            BatchFinder.SessionCount(ver),
                                            resourceCheck,
                                            blockingCheck,
                                            memory);
            }
            if (cloudProviderId == AZURE_MANAGED_CLOUD_ID)
            {
                cmd.CommandText = cmd.CommandText + BatchFinder.AzureSQLMetric();
            }


            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout > 300 ? SqlHelper.CommandTimeout : 300;
            return cmd;
        }

        #region conditional alert sql

        private static string BuildResourceCheck(ServerVersion ver, ScheduledRefresh previousRefresh,
                                                 MonitoredServerWorkload workload, int? cloudProviderId)
        {
            float warningValue = 0;
            float criticalValue = 0;

            //check for exclude SQLdm application
            AdvancedAlertConfigurationSettings config =
                    ScheduledCollectionEventProcessor.GetAdvancedConfiguration(
                        workload.Thresholds[(int)Metric.ResourceAlert]);
            string filterStatementsLevel1 = "";

            if (config != null && config.ExcludeSqldmApp)
            {//and program_name not like 'SQL diagnostic manager%'
                filterStatementsLevel1 =
                    String.Format(BatchConstants.ExcludeLike, "program_name",
                                  Constants.ConnectionStringApplicationNamePrefix + BatchConstants.PercentageChar);
            }

            if (workload.Thresholds[(int)Metric.ResourceAlert].WarningThreshold.Value != null)
            {
                float.TryParse(workload.Thresholds[(int)Metric.ResourceAlert].WarningThreshold.Value.ToString(),
                               out warningValue);
            }

            if (workload.Thresholds[(int)Metric.ResourceAlert].CriticalThreshold.Value != null)
            {
                float.TryParse(workload.Thresholds[(int)Metric.ResourceAlert].CriticalThreshold.Value.ToString(),
                               out criticalValue);
            }

            string resourceCheck = previousRefresh == null ? BatchConstants.ResetResourceCheckTable : "";

            if (ver.Major > 8)
            {
                resourceCheck += String.Format
                    (BatchFinder.ResourceCheck(ver, cloudProviderId),
                     criticalValue > warningValue ? warningValue * 1000 : criticalValue * 1000,
                     BuildSessionFilters(
                         ScheduledCollectionEventProcessor.GetAdvancedConfiguration(
                             workload.Thresholds[(int)Metric.ResourceAlert])),
                     ver.MasterDatabaseCompatibility < 90
                         ? BatchConstants.ResourceCheckInputbufferCompatibilityMode80
                         : BatchConstants.ResourceCheckInputbufferCompatibilityMode90,
                         filterStatementsLevel1);
            }
            else
            {
                resourceCheck += String.Format
                    (BatchFinder.ResourceCheck(ver, cloudProviderId),
                     criticalValue > warningValue ? warningValue * 1000 : criticalValue * 1000,
                     BuildSessionFilters(
                         ScheduledCollectionEventProcessor.GetAdvancedConfiguration(
                             workload.Thresholds[(int)Metric.ResourceAlert])),
                             filterStatementsLevel1);
            }


            return resourceCheck;
        }

        private static string BuildBlockingCheck(ServerVersion ver, MonitoredServerWorkload workload, int? cloudProviderId)
        {
            object value = 0;
            //check for exclude SQLdm application
            AdvancedAlertConfigurationSettings config =
                    ScheduledCollectionEventProcessor.GetAdvancedConfiguration(
                        workload.Thresholds[(int)Metric.BlockedSessions]);
            string filterStatementsLevel1 = "";
            if (config != null && config.ExcludeSqldmApp)
            {
                filterStatementsLevel1 =
                    String.Format(BatchConstants.ExcludeLike, "p.program_name",
                                  Constants.ConnectionStringApplicationNamePrefix + BatchConstants.PercentageChar);
            }
            // if warning is enabled use it otherwise use the critical threshold value
            MetricThresholdEntry entry = workload.Thresholds[(int)Metric.BlockingAlert];
            if (entry.WarningThreshold.Enabled)
            {
                value = entry.WarningThreshold.Value;
            }
            else
                value = entry.CriticalThreshold.Value;

            if (ver.Major > 8)
            {
                return String.Format
                    (BatchFinder.BlockingCheck(ver, cloudProviderId),
                     ((int)Convert.ChangeType(value, typeof(Int32))) * 1000,
                     ver.MasterDatabaseCompatibility < 90
                         ? BatchConstants.BlockingCheckInputbufferCompatibilityMode80
                         : BatchConstants.BlockingCheckInputbufferCompatibilityMode90,
                         filterStatementsLevel1);
            }
            else
            {
                return String.Format
                    (BatchFinder.BlockingCheck(ver, cloudProviderId),
                     ((int)Convert.ChangeType(value, typeof(Int32))) * 1000,
                     filterStatementsLevel1);
            }
        }

        internal static SqlCommand BuildMachineName(SqlConnection connection)
        {
            SqlCommand cmd = connection.CreateCommand();
            ServerVersion ver = new ServerVersion(connection.ServerVersion);
            cmd.CommandText = BatchConstants.GetMachineName;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        /// <summary>
        /// SQLdm 10.0 srishti purohit-- doctor integration-- affected procedures and tables for specific table of database
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ObjectName"></param>
        /// <returns></returns>
        internal static SqlCommand BuildDependentObjectsCommand(SqlConnection connection, DatabaseObjectName ObjectName)
        {
            SqlCommand cmd = connection.CreateCommand();
            ServerVersion ver = new ServerVersion(connection.ServerVersion);
            cmd.CommandText = String.Format(BatchFinder.DependentObjectBatch(ver));
            //Add parameter to command
            if (ObjectName != null)
            {
                cmd.CommandText = cmd.CommandText.Replace("@DatabaseName", ObjectName.DatabaseName);
                cmd.CommandText = cmd.CommandText.Replace("@TableName", "'" + ObjectName.ObjectName + "'");
                cmd.CommandText = cmd.CommandText.Replace("@SchemaName", "'" + ObjectName.SchemaName + "'");
            }
            else
                cmd.CommandText = string.Empty;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        //START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- New method which will generate the command to fetch the Computer NetBIOS name 
        internal static SqlCommand BuildComputerBIOSName(SqlConnection connection)
        {
            SqlCommand cmd = connection.CreateCommand();
            ServerVersion ver = new ServerVersion(connection.ServerVersion);
            cmd.CommandText = BatchConstants.GetComputerBIOSName;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        //END SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- New method which will generate the command to fetch the Computer NetBIOS name 

        //private static string GetJobCategoryMatchString(AdvancedAlertConfigurationSettings advancedSettings,
        //                                                out bool matchFilterApplied)
        //{
        //    matchFilterApplied = false;
        //    string categoryMatch = @"''";


        //    if (advancedSettings != null
        //        && advancedSettings.JobCategoryExcludeMatch != null
        //        && advancedSettings.JobCategoryExcludeMatch.Length != 0)
        //    {
        //        foreach (string matchString in advancedSettings.JobCategoryExcludeMatch)
        //        {
        //            categoryMatch += @",'" + matchString.Replace("'", "''").ToLower() + @"'";
        //            matchFilterApplied = true;
        //        }
        //    }

        //    return categoryMatch != @"''" ? String.Format(BatchConstants.CategoryFilter, categoryMatch) : "";
        //}

        //private static string GetJobCategoryLikeString(AdvancedAlertConfigurationSettings advancedSettings,
        //                                               out bool likeFilterApplied)
        //{
        //    likeFilterApplied = false;
        //    string categoryLike = "";

        //    if (advancedSettings != null
        //        && advancedSettings.JobCategoryExcludeLike != null
        //        && advancedSettings.JobCategoryExcludeLike.Length != 0)
        //    {
        //        foreach (string likeString in advancedSettings.JobCategoryExcludeLike)
        //        {
        //            categoryLike +=
        //                String.Format(BatchConstants.CategoryLikeFilter, likeString.Replace("'", "''").ToLower());
        //            likeFilterApplied = true;
        //        }
        //    }
        //    return categoryLike;
        //}

        //private static string GetJobNameMatchString(AdvancedAlertConfigurationSettings advancedSettings,
        //                                            out bool matchFilterApplied)
        //{
        //    matchFilterApplied = false;
        //    string jobMatch = @"''";


        //    if (advancedSettings != null
        //        && advancedSettings.JobNameExcludeMatch != null
        //        && advancedSettings.JobNameExcludeMatch.Length != 0)
        //    {
        //        foreach (string matchString in advancedSettings.JobNameExcludeMatch)
        //        {
        //            jobMatch += @",'" + matchString.Replace("'", "''").ToLower() + @"'";
        //            matchFilterApplied = true;
        //        }
        //    }

        //    return jobMatch != @"''" ? String.Format(BatchConstants.JobNameFilter, jobMatch) : "";
        //}

        //private static string GetJobNameLikeString(AdvancedAlertConfigurationSettings advancedSettings,
        //                                           out bool likeFilterApplied)
        //{
        //    likeFilterApplied = false;
        //    string jobLike = @"";

        //    if (advancedSettings != null
        //        && advancedSettings.JobNameExcludeLike != null
        //        && advancedSettings.JobNameExcludeLike.Length != 0)
        //    {
        //        foreach (string likeString in advancedSettings.JobNameExcludeLike)
        //        {
        //            jobLike += String.Format(BatchConstants.JobNameLikeFilter, likeString.Replace("'", "''").ToLower());
        //            likeFilterApplied = true;
        //        }
        //    }
        //    return jobLike;
        //}

        internal static string BuildJobFilters(AdvancedAlertConfigurationSettings advancedSettings, bool escapeString)
        {
            StringBuilder filterString = new StringBuilder("");

            if (advancedSettings == null ||
                ((advancedSettings.JobIncludeFilter.Count == 0) && (advancedSettings.JobExcludeFilter.Count == 0)))
            {
                return filterString.ToString();
            }

            filterString.Append(" AND (");

            if ((advancedSettings.JobIncludeFilter != null) && (advancedSettings.JobIncludeFilter.Count > 0))
            {
                filterString.Append(" (");
                foreach (JobFilter jobFilter in advancedSettings.JobIncludeFilter)
                {
                    filterString.Append("(");
                    filterString.Append((jobFilter.Category.OpCode == OpCodes.Equals) ?
                                            string.Format(BatchConstants.CategoryEquals, jobFilter.Category.Filter.Replace("'", "''")) :
                                            string.Format(BatchConstants.CategoryLike, jobFilter.Category.Filter.Replace("'", "''")));
                    filterString.Append(") AND (");
                    filterString.Append((jobFilter.JobName.OpCode == OpCodes.Equals) ?
                                            string.Format(BatchConstants.JobNameEquals, jobFilter.JobName.Filter.Replace("'", "''")) :
                                            string.Format(BatchConstants.JobNameLike, jobFilter.JobName.Filter.Replace("'", "''")));


                    if (advancedSettings.AlertOnJobSteps)
                    {
                        if (advancedSettings.Metric == Metric.LongJobs || advancedSettings.Metric == Metric.LongJobsMinutes)
                        {
                            filterString.Append(") AND (");
                            filterString.Append(BatchConstants.LJIncludeStep0);
                            filterString.Append((jobFilter.StepName.OpCode == OpCodes.Equals) ?
                                                string.Format(BatchConstants.LJStepNameEquals, jobFilter.StepName.Filter.Replace("'", "''")) :
                                                string.Format(BatchConstants.LJStepNameLike, jobFilter.StepName.Filter.Replace("'", "''")));
                        }
                        else
                        {
                            filterString.Append(") AND (");
                            filterString.Append(BatchConstants.IncludeStep0);
                            filterString.Append((jobFilter.StepName.OpCode == OpCodes.Equals) ?
                                                string.Format(BatchConstants.StepNameEquals, jobFilter.StepName.Filter.Replace("'", "''")) :
                                                string.Format(BatchConstants.StepNameLike, jobFilter.StepName.Filter.Replace("'", "''")));
                        }
                    }

                    filterString.Append(") \r\n       OR ");
                }

                if (filterString.ToString().EndsWith(BatchConstants.OrConnector))
                    filterString.Remove((filterString.Length - 3), 3);

                filterString.Append(")");

            }

            if ((advancedSettings.JobExcludeFilter != null) && (advancedSettings.JobExcludeFilter.Count > 0))
            {
                if (advancedSettings.JobIncludeFilter.Count > 0)
                    filterString.Append(BatchConstants.AndConnector);

                filterString.Append("NOT (");

                foreach (JobFilter jobFilter in advancedSettings.JobExcludeFilter)
                {
                    filterString.Append("(");
                    filterString.Append((jobFilter.Category.OpCode == OpCodes.Equals) ?
                                            string.Format(BatchConstants.CategoryEquals, jobFilter.Category.Filter.Replace("'", "''")) :
                                            string.Format(BatchConstants.CategoryLike, jobFilter.Category.Filter.Replace("'", "''")));
                    filterString.Append(") AND (");
                    filterString.Append((jobFilter.JobName.OpCode == OpCodes.Equals) ?
                                            string.Format(BatchConstants.JobNameEquals, jobFilter.JobName.Filter.Replace("'", "''")) :
                                            string.Format(BatchConstants.JobNameLike, jobFilter.JobName.Filter.Replace("'", "''")));

                    if (advancedSettings.AlertOnJobSteps)
                    {
                        filterString.Append(") AND (");

                        if (advancedSettings.Metric == Metric.LongJobs || advancedSettings.Metric == Metric.LongJobsMinutes)
                        {
                            filterString.Append((jobFilter.StepName.OpCode == OpCodes.Equals) ?
                                                string.Format(BatchConstants.LJStepNameEquals, jobFilter.StepName.Filter.Replace("'", "''")) :
                                                string.Format(BatchConstants.LJStepNameLike, jobFilter.StepName.Filter.Replace("'", "''")));
                        }
                        else
                        {
                            filterString.Append((jobFilter.StepName.OpCode == OpCodes.Equals) ?
                                                string.Format(BatchConstants.StepNameEquals, jobFilter.StepName.Filter.Replace("'", "''")) :
                                                string.Format(BatchConstants.StepNameLike, jobFilter.StepName.Filter.Replace("'", "''")));
                        }
                    }

                    filterString.Append(") \r\n       OR ");
                }

                if (filterString.ToString().EndsWith(BatchConstants.OrConnector))
                    filterString.Remove((filterString.Length - 3), 3);

                filterString.Append(")");
            }

            filterString.Append(")\r\n");

            if (escapeString && filterString.Length > 0)
                filterString.Replace("'", "''");

            return filterString.ToString();
        }



        //internal static string BuildJobFilters(AdvancedAlertConfigurationSettings advancedSettings, bool escapeString)
        //{
        //    bool categoryMatchFilter, categoryLikeFilter, jobMatchFilter, jobLikeFilter;

        //    string categoryMatch = GetJobCategoryMatchString(advancedSettings, out categoryMatchFilter);
        //    string categoryLike = GetJobCategoryLikeString(advancedSettings, out categoryLikeFilter);
        //    string jobMatch = GetJobNameMatchString(advancedSettings, out jobMatchFilter);
        //    string jobLike = GetJobNameLikeString(advancedSettings, out jobLikeFilter);

        //    string returnstring = 
        //        (categoryMatchFilter ? categoryMatch : "") +
        //        (categoryLikeFilter ? categoryLike : "") +
        //        (jobMatchFilter ? jobMatch : "") +
        //        (jobLikeFilter ? jobLike : "");

        //    if (escapeString && returnstring != null && returnstring.Length > 0)
        //        return returnstring.Replace("'", "''");
        //    else
        //        return returnstring;
        //}


        internal static SqlCommand BuildBombedJobsVariables(SqlConnection connection, string realServerName, ServerVersion ver, ScheduledRefresh previousRefresh,
                                              MonitoredServerWorkload workload)
        {
            try
            {
                SqlCommand cmd = connection.CreateCommand();
                int lastJobId = 0;
                if (previousRefresh != null && previousRefresh.Alerts.JobFailures.LastInstanceId != 0)
                {
                    lastJobId = previousRefresh.Alerts.JobFailures.LastInstanceId;
                }
                else
                {
                    lastJobId = PersistenceManager.Instance.GetFailedJobInstanceId(workload.MonitoredServer.InstanceName);
                }

                //List<Guid> guids = new List<Guid>();
                List<Pair<Guid, int>> prvFailedJobs;

                if (previousRefresh != null && previousRefresh.Alerts.JobFailures.LastInstanceId != 0 &&
                    previousRefresh.Alerts.JobFailures.FailedJobSteps != null)
                {
                    prvFailedJobs = previousRefresh.Alerts.JobFailures.FailedJobSteps;
                    //guids = previousRefresh.Alerts.JobFailures.FailedJobGuids;
                }
                else
                {
                    prvFailedJobs = PersistenceManager.Instance.GetFailedJobSteps(realServerName);
                    //guids = PersistenceManager.Instance.GetFailedJobGuids(realServerName);
                }

                string guidList = "";
                //foreach (Guid g in guids)
                //{
                //    guidList += "	union select '" + g + "', host_name()\r\n";
                //}
                foreach (Pair<Guid, int> job in prvFailedJobs)
                {
                    guidList += " union select'" + job.First + "', host_name(), " + job.Second + "\r\n";
                }

                AdvancedAlertConfigurationSettings config =
                    ScheduledCollectionEventProcessor.GetAdvancedConfiguration(
                        workload.Thresholds[(int)Metric.BombedJobs]);

                cmd.CommandText =
                    String.Format
                        (BatchFinder.FailedJobVariables(ver),
                         lastJobId + 1,
                         config != null ? (config.TreatSingleJobFailureAsCritical ? 1 : 0) : 1,
                         guidList,
                         lastJobId);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = SqlHelper.CommandTimeout;
                return cmd;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static string BuildBombedJobs(ServerVersion ver, MonitoredServerWorkload workload)
        {
            try
            {
                AdvancedAlertConfigurationSettings config =
                    ScheduledCollectionEventProcessor.GetAdvancedConfiguration(
                        workload.Thresholds[(int)Metric.BombedJobs]);

                if (config == null)
                    config = new AdvancedAlertConfigurationSettings(Metric.BombedJobs, workload.Thresholds[(int)Metric.BombedJobs].Data);

                return
                    String.Format
                        (BatchFinder.FailedJobs(ver),
                         BuildJobFilters(config, ver.Major > 8),
                         config != null && !config.AlertOnJobSteps ? BatchConstants.OnFailAction : "",
                         config != null && !config.AlertOnJobSteps ? BatchConstants.JobNotInTempTable : ""
                         );
            }
            catch
            {
                return null;
            }
        }

        private static string BuildCompletedJobs(ServerVersion ver, MonitoredServerWorkload workload, ScheduledRefresh previousRefresh)
        {
            try
            {
                int lastJobId = 0;

                if (previousRefresh != null && previousRefresh.Alerts.JobsCompleted.LastInstanceId != 0)
                    lastJobId = previousRefresh.Alerts.JobsCompleted.LastInstanceId;
                else
                    lastJobId = PersistenceManager.Instance.GetCompletedJobInstanceId(workload.MonitoredServer.InstanceName);

                AdvancedAlertConfigurationSettings config =
                   ScheduledCollectionEventProcessor.GetAdvancedConfiguration(workload.Thresholds[(int)Metric.JobCompletion]);

                if (config == null)
                    config = new AdvancedAlertConfigurationSettings(Metric.JobCompletion, workload.Thresholds[(int)Metric.JobCompletion].Data);

                return String.Format(BatchFinder.CompletedJobs(ver), lastJobId + 1, BuildJobFilters(config, false), (config.AlertOnJobSteps ? BatchConstants.IncludeJobSteps : ""));
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static string BuildLongJobs(ServerVersion ver, MonitoredServerWorkload workload, int? cloudProviderId)
        {
            float warningThreshold = 1;
            float criticalThreshold = 1;
            float lowest = 1;
            float highest = 1;
            string longJobsPercent = "";
            string longJobsMinutes = "";

            bool alertOnSteps = false;

            if (workload.GetMetricThresholdEnabled(Metric.LongJobs))
            {
                if (workload.Thresholds[(int)Metric.LongJobs].WarningThreshold.Value != null)
                {
                    float.TryParse(workload.Thresholds[(int)Metric.LongJobs].WarningThreshold.Value.ToString(),
                                   out warningThreshold);
                }

                if (workload.Thresholds[(int)Metric.LongJobs].CriticalThreshold.Value != null)
                {
                    float.TryParse(workload.Thresholds[(int)Metric.LongJobs].CriticalThreshold.Value.ToString(),
                                   out criticalThreshold);
                }

                lowest = criticalThreshold > warningThreshold ? 1 + warningThreshold / 100 : 1 + criticalThreshold / 100;
                highest = criticalThreshold > warningThreshold ? 1 + criticalThreshold / 100 : 1 + warningThreshold / 100;

                AdvancedAlertConfigurationSettings config =
                   ScheduledCollectionEventProcessor.GetAdvancedConfiguration(workload.Thresholds[(int)Metric.LongJobs]);

                if (config == null)
                    config = new AdvancedAlertConfigurationSettings(Metric.LongJobs, workload.Thresholds[(int)Metric.LongJobs].Data);

                longJobsPercent = String.Format
                    (BatchFinder.LongJobsByPercent(ver),
                     BuildJobFilters(config, false),
                     lowest.ToString(CultureInfo.InvariantCulture),
                    //Run with the lowest threshold even if it's backwards
                     highest.ToString(CultureInfo.InvariantCulture),
                    //Run with the highest threshold even if it's backwards
                     (int)Metric.LongJobs,
                     config.MinSecondsRuntimeRunningJobByPercent
                    //,config.AlertOnJobSteps ? BatchConstants.LongJobIncludeSteps1 : "",
                    //config.AlertOnJobSteps ? BatchConstants.LongJobIncludeSteps2 : "",
                    //config.AlertOnJobSteps ? "" : BatchConstants.LongJobStep0Only
                     );
                alertOnSteps = config.AlertOnJobSteps;
            }

            if (workload.GetMetricThresholdEnabled(Metric.LongJobsMinutes))
            {
                if (workload.Thresholds[(int)Metric.LongJobsMinutes].WarningThreshold.Value != null)
                {
                    float.TryParse(workload.Thresholds[(int)Metric.LongJobsMinutes].WarningThreshold.Value.ToString(),
                                   out warningThreshold);
                }

                if (workload.Thresholds[(int)Metric.LongJobsMinutes].CriticalThreshold.Value != null)
                {
                    float.TryParse(workload.Thresholds[(int)Metric.LongJobsMinutes].CriticalThreshold.Value.ToString(),
                                   out criticalThreshold);
                }

                lowest = criticalThreshold > warningThreshold ? warningThreshold * 60 : criticalThreshold * 60;
                highest = criticalThreshold > warningThreshold ? criticalThreshold * 60 : warningThreshold * 60;

                AdvancedAlertConfigurationSettings config =
                   ScheduledCollectionEventProcessor.GetAdvancedConfiguration(workload.Thresholds[(int)Metric.LongJobsMinutes]);

                if (config == null)
                    config = new AdvancedAlertConfigurationSettings(Metric.LongJobsMinutes, workload.Thresholds[(int)Metric.LongJobsMinutes].Data);

                longJobsMinutes = String.Format
                    (BatchFinder.LongJobsByRuntime(ver),
                     BuildJobFilters(config, false),
                     lowest.ToString(CultureInfo.InvariantCulture),
                    //Run with the lowest threshold even if it's backwards
                     highest.ToString(CultureInfo.InvariantCulture),
                    //Run with the highest threshold even if it's backwards
                     (int)Metric.LongJobsMinutes
                    //,config.AlertOnJobSteps ? BatchConstants.LongJobIncludeSteps1 : "",
                    //config.AlertOnJobSteps ? BatchConstants.LongJobIncludeSteps2 : "",
                    //config.AlertOnJobSteps ? "" : BatchConstants.LongJobStep0Only
                     );
                if (!alertOnSteps)
                    alertOnSteps = config.AlertOnJobSteps;
            }


            return String.Format(alertOnSteps ? BatchFinder.LongJobsWithSteps(ver, cloudProviderId) : BatchFinder.LongJobs(ver, cloudProviderId), longJobsPercent, longJobsMinutes);
        }

        #endregion

        #endregion

        private static string BuildSessionMatchSubFilter(string[] settingString, string filterName)
        {
            string sessionMatch = @"''";

            if (settingString != null
                && settingString.Length != 0)
            {
                foreach (string matchString in settingString)
                {
                    sessionMatch += @",'" + matchString.Replace("'", "''").ToLower().TrimEnd() + @"'";
                }
            }

            return
                sessionMatch.Length > 2
                    ? String.Format(BatchConstants.SessionMatch, filterName, sessionMatch.Substring(3))
                    : "";
        }

        private static string BuildSessionLikeSubFilter(string[] settingString, string filterName)
        {
            string sessionLikeSubFilter = @"";

            if (settingString != null
                && settingString.Length != 0)
            {
                foreach (string likeString in settingString)
                {
                    sessionLikeSubFilter +=
                        String.Format(BatchConstants.SessionLike,
                                      filterName,
                                      likeString.Replace("'", "''").ToLower().TrimEnd());
                }
            }
            return sessionLikeSubFilter;
        }

        internal static string BuildSessionFilters(AdvancedAlertConfigurationSettings advancedSettings)
        {
            if (advancedSettings == null)
                return "";

            string filterOut = "";

            // Apply match filters

            filterOut =
                BuildSessionMatchSubFilter(advancedSettings.AppNameExcludeMatch, BatchConstants.SessionApp);

            filterOut +=
                BuildSessionLikeSubFilter(advancedSettings.AppNameExcludeLike, BatchConstants.SessionApp);

            filterOut +=
                BuildSessionMatchSubFilter(advancedSettings.HostExcludeMatch, BatchConstants.SessionHost);

            filterOut +=
                BuildSessionLikeSubFilter(advancedSettings.HostExcludeLike, BatchConstants.SessionHost);

            filterOut +=
                BuildSessionMatchSubFilter(advancedSettings.UserExcludeMatch, BatchConstants.SessionUser);

            filterOut +=
                BuildSessionLikeSubFilter(advancedSettings.UserExcludeLike, BatchConstants.SessionUser);

            return filterOut;
        }


        internal static SqlCommand BuildOldestOpenCommand(SqlConnection connection, ServerVersion ver,
                                                          MonitoredServerWorkload workload)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (ver.Major > 8)
            {
                cmd.CommandText = String.Format(BatchFinder.OldestOpenTransaction(ver, workload.MonitoredServer.CloudProviderId),
                                                BuildSessionFilters(
                                                    ScheduledCollectionEventProcessor.GetAdvancedConfiguration(
                                                        workload.Thresholds[(int)Metric.OldestOpenTransMinutes])),
                                                ver.MasterDatabaseCompatibility < 90
                                                    ? BatchConstants.SqlTextCompatibilityMode80
                                                    : BatchConstants.SqlTextCompatibilityMode90);
            }
            else
            {
                cmd.CommandText = String.Format(BatchFinder.OldestOpenTransaction(ver, workload.MonitoredServer.CloudProviderId),
                                                BuildSessionFilters(
                                                    ScheduledCollectionEventProcessor.GetAdvancedConfiguration(
                                                        workload.Thresholds[(int)Metric.OldestOpenTransMinutes])));
            }
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        internal static SqlCommand BuildDatabaseLastBackupDateTimeCommand(SqlConnection connection, String databaseName)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = String.Format(BatchFinder.DatabaseLastBakcupDateTime(), "'" + databaseName + "'");

            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildAvailabilitySummaryCommand(SqlConnection connection)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = BatchFinder.AlwaysOnToplogySummary();

            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildAvailabilityDetailCommand(SqlConnection connection, string groupId, string replicaId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = String.Format(BatchFinder.AlwaysOnToplogyDetail(), groupId, replicaId);

            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildOSMetricsCommand(SqlConnection connection, ServerVersion ver, WmiConfiguration wmiConfiguration, int SqlProcessID, int? cloudProviderId)
        {
            var spOAContext = (int)wmiConfiguration.OleAutomationContext;
            LogSpOAContext(connection.DataSource, "OSMetrics", spOAContext);
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = String.Format(BatchFinder.OSMetrics(ver, cloudProviderId),
                                            spOAContext,
                                            wmiConfiguration.OleAutomationDisabled ? 1 : 0,
                                            TimeSpan.FromSeconds(
                                                    CollectionServiceConfiguration.GetCollectionServiceElement().CustomOSCounterWaitTimeInSeconds > 0
                                                        ? CollectionServiceConfiguration.GetCollectionServiceElement().CustomOSCounterWaitTimeInSeconds
                                                        : 0),
                                                        wmiConfiguration.DirectWmiEnabled ? 1 : 0,
                                                        SqlProcessID);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildAlwaysOnMetricsCommand(SqlConnection connection, ServerVersion ver)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = String.Format("{0} {1}", BatchFinder.AlwaysOnTopology(ver), BatchFinder.AlwaysOnStatistics(ver));
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildTableGrowthCommand(SqlConnection connection, ServerVersion ver, TableGrowthConfiguration config, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandTimeout = CollectionServiceConfiguration.GetCollectionServiceElement().QuietTimeSqlCommandTimeout > SqlHelper.CommandTimeout ? CollectionServiceConfiguration.GetCollectionServiceElement().QuietTimeSqlCommandTimeout : SqlHelper.CommandTimeout;
            StringBuilder databaseExcludeString = new StringBuilder();
            foreach (string database in config.TableStatisticsExcludedDatabases)
            {
                databaseExcludeString.Append(",'");
                databaseExcludeString.Append(database.ToLower().Replace("'", "''"));
                databaseExcludeString.Append("'");
            }
            StringBuilder alreadyCollectedDatabases = new StringBuilder();
            alreadyCollectedDatabases.Append("0");
            foreach (int i in config.AlreadyCollectedDatabases)
            {
                alreadyCollectedDatabases.Append(",");
                alreadyCollectedDatabases.Append(i);
            }

            // Flood control rowcount limit
            int rowcountLimit = CollectionServiceConfiguration.GetCollectionServiceElement().MaxRowCountTableGrowth;


            // Don't allow 0 or negative numbers - revert to 1);
            if (rowcountLimit < 1)
                rowcountLimit = 1;

            LOG.VerboseFormat("Table Growth Rowcount Limit: {0}", rowcountLimit);

            int timeout = (int)((cmd.CommandTimeout > 90) ? (cmd.CommandTimeout - 60) : (cmd.CommandTimeout * .90));

            LOG.VerboseFormat("Table Growth Timeout: {0}", timeout);

            config.MaxOrMin = !config.MaxOrMin;

            cmd.CommandText =
                String.Format(
                    BatchFinder.TableGrowth(ver, cloudProviderId),
                    databaseExcludeString,
                    rowcountLimit,
                    timeout,
                    alreadyCollectedDatabases,
                    config.MaxOrMin ? "max" : "min");


            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        //internal static SqlCommand BuildReorganizationCommand(SqlConnection connection, ServerVersion ver,
        //                                                      List<string> databaseToExclude, bool IncludeSystemTables,
        //                                                      MonitoredServerWorkload workload, List<int> ReorgRetryDatabases,
        //                                                      Dictionary<int, Dictionary<int, Pair<int, string>>> ReorgRetryTables,
        //                                                      int reverseOrder)
        //{
        //    SqlCommand cmd = connection.CreateCommand();
        //    StringBuilder databaseExcludeString = new StringBuilder();
        //    StringBuilder retryDatabaseList = new StringBuilder();
        //    StringBuilder retryTableList = new StringBuilder();
        //    foreach (string database in databaseToExclude)
        //    {
        //        databaseExcludeString.Append(",'");
        //        databaseExcludeString.Append(database.ToLower().Replace("'", "''"));
        //        databaseExcludeString.Append("'");
        //    }
        //    FileSize minimumTableSize =
        //        (FileSize) workload.MonitoredServer.ReorganizationMinimumTableSize;

        //    LOG.InfoFormat("Minimum table size to be included in table reorganization is {0} pages.",
        //                   minimumTableSize.Pages);

        //    if (ReorgRetryDatabases != null && ReorgRetryDatabases.Count > 0)
        //    {
        //        retryDatabaseList.Append(BatchConstants.ReorgDatabaseRetryQuery);
        //        retryDatabaseList.Append("(0");
        //        foreach (int i in ReorgRetryDatabases)
        //        {
        //            retryDatabaseList.Append(",");
        //            retryDatabaseList.Append(i);
        //        }
        //        retryDatabaseList.Append(")");
        //    }

        //    if (ReorgRetryTables != null && ReorgRetryTables.Count > 0)
        //    {
        //       foreach (int dbid in ReorgRetryTables.Keys)
        //       {
        //           foreach (int tableid in ReorgRetryTables[dbid].Keys)
        //           {
        //               retryTableList.Append("union select ");
        //               retryTableList.Append(dbid);
        //               retryTableList.Append(",");
        //               retryTableList.Append(tableid);
        //               retryTableList.Append(",");
        //               retryTableList.Append(ReorgRetryTables[dbid][tableid].First);
        //               retryTableList.Append(",'");
        //               retryTableList.Append(ReorgRetryTables[dbid][tableid].Second);
        //               retryTableList.Append("' ");
        //           }
        //       }
        //    }

        //    cmd.CommandTimeout = CollectionServiceConfiguration.GetCollectionServiceElement().QuietTimeSqlCommandTimeout > SqlHelper.CommandTimeout ? CollectionServiceConfiguration.GetCollectionServiceElement().QuietTimeSqlCommandTimeout : SqlHelper.CommandTimeout;
        //    cmd.CommandText =
        //        String.Format(BatchFinder.Reorganization(ver),
        //                      databaseExcludeString,
        //                      IncludeSystemTables ? 'S' : 'B',
        //                      Math.Floor((decimal)minimumTableSize.Pages),
        //                      retryDatabaseList,
        //                      retryTableList,
        //                      reverseOrder,
        //                      ((cmd.CommandTimeout > 90)? (cmd.CommandTimeout-60): (cmd.CommandTimeout*.90)));
        //    cmd.CommandType = CommandType.Text;

        //    return cmd;
        //}

        internal static SqlCommand BuildTableFragmentationWorkloadCommand(SqlConnection connection, ServerVersion ver,
                                                              TableFragmentationConfiguration config, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();

            StringBuilder databaseExcludeString = new StringBuilder();
            foreach (string database in config.TableStatisticsExcludedDatabases)
            {
                databaseExcludeString.Append(",'");
                databaseExcludeString.Append(database.ToLower().Replace("'", "''"));
                databaseExcludeString.Append("'");
            }
            FileSize minimumTableSize = config.FragmentationMinimumTableSize;

            LOG.InfoFormat("Minimum table size to be included in table fragmentation is {0} pages.",
                           minimumTableSize.Pages);

            string commandText = String.Format(BatchFinder.FragmentationWorkload(ver, cloudProviderId),
                                               databaseExcludeString,
                                               Math.Floor((decimal)minimumTableSize.Pages),
                                               config.PresentFragmentationStatisticsRunTime.Value.ToString(
                                                   "yyyy'-'MM'-'dd HH':'mm':'ss"));

            if (CollectionServiceConfiguration.GetCollectionServiceElement().FragmentationSqlCommandTimeout >
                SqlHelper.CommandTimeout)
            {
                cmd.CommandTimeout =
                    CollectionServiceConfiguration.GetCollectionServiceElement().FragmentationSqlCommandTimeout;

                commandText = commandText.Replace(BatchConstants.BatchTimeout, "set lock_timeout " + cmd.CommandTimeout * 1000);
            }
            else
            {
                cmd.CommandTimeout = SqlHelper.CommandTimeout;
            }

            cmd.CommandText = commandText;
            cmd.CommandType = CommandType.Text;

            return cmd;
        }


        internal static SqlCommand BuildTableFragmentationCommand(SqlConnection connection, ServerVersion ver,
                                                              TableFragmentationConfiguration config, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandTimeout = CollectionServiceConfiguration.GetCollectionServiceElement().FragmentationSqlCommandTimeout > SqlHelper.CommandTimeout ? CollectionServiceConfiguration.GetCollectionServiceElement().FragmentationSqlCommandTimeout : SqlHelper.CommandTimeout;

            int rowcountLimit = 500;
            int timeout = (int)((cmd.CommandTimeout > 90) ? (cmd.CommandTimeout - 60) : (cmd.CommandTimeout * .90));

            cmd.CommandText =
                String.Format(BatchFinder.Fragmentation(ver, cloudProviderId),
                              rowcountLimit,
                              timeout,
                              config.Order);
            cmd.CommandType = CommandType.Text;

            return cmd;
        }


        internal static SqlCommand BuildReplicationStatusCommand(SqlConnection connection, ServerVersion ver,
                                                                 WmiConfiguration wmiConfig,
                                                                 ClusterCollectionSetting clusterCollectionSetting)
        {
            var spOAContext = (int)wmiConfig.OleAutomationContext;
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += String.Format(BatchFinder.ReplicationStatus(ver),
                                          spOAContext,
                                          wmiConfig.OleAutomationDisabled ? 1 : 0,
                                          (int)clusterCollectionSetting > 0 ? BatchConstants.ClusterSettingFalse : "",
                                          wmiConfig.DirectWmiEnabled ? 1 : 0);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildSubscriberDetailsCommand(SqlConnection connection, ServerVersion ver, string subscriberDatabase)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += string.Format(BatchConstants.CopyrightNotice + BatchConstants.BatchHeader +
                BatchConstants.SubscriptionsEnum, subscriberDatabase.Replace("'", "''"));
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        #region Activity Monitor
        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        private static string BuildActivityMonitorTraceCommandText(
            ServerVersion ver,
            ActivityMonitorConfiguration config,
            bool hasBeenStopped,
            bool excludeReadSegment,
            DateTime? timeStamp,
            int? cloudProviderId)
        {
            StringBuilder traceSegments = new StringBuilder();
            StringBuilder filterStatements = new StringBuilder();

            //default is -1 meaning do nothing. +ve means limit in the interpreter. -ve means limit in the batch
            long? sqlTextLengthLimit = CollectionServiceConfiguration.GetCollectionServiceElement().MaxQueryMonitorEventSizeKB;

            // Not available for SQL 2000 - this is handled by the batchfinder
            if (config.DeadlockEventsEnabled)
            {
                traceSegments.Append(BatchFinder.ActivityMonitorTraceDeadlocks(ver));
            }

            if (config.BlockingEventsEnabled)
            {
                traceSegments.Append(BatchFinder.ActivityMonitorBlocking(ver));
            }

            if (config.AutoGrowEventsEnabled)
            {
                traceSegments.Append(String.Format(BatchFinder.ActivityProfilerTraceAutogrow(ver),
                                                   ver.Major == 8 ? "@P2" : "@P1"));
            }

            const string DateFormat = "yyyy'-'MM'-'dd HH':'mm':'ss";
            const string DateText = "'{0}'";

            var stopTime = config.StopTimeUTC != null && config.StopTimeUTC.HasValue && (timeStamp == null || config.StopTimeUTC >= timeStamp)
                               ? string.Format(DateText, config.StopTimeUTC.Value.AddMinutes(10).ToString(DateFormat))
                               : string.Format(DateText,
                                               DateTime.Now.ToUniversalTime().AddHours(
                                                   Constants.QueryMonitorTraceDefaultDuration).ToString(
                                                       DateFormat));

            string stopAfterReading = "";

            // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
            // If the Stop Time is reached then we need to restart the trace. Note that we need to always update
            // the trace for Activity Monitor for an unlimited trace. Only if the Collection Service crashes
            // then the Activity Monitor trace will stop.
            if (config.StopTimeUTC <= timeStamp.Value.AddHours(1))
            {
                // We need to update the next Stop Time manually
                config.StopTimeUTC = DateTime.Now.ToUniversalTime().AddHours(Constants.QueryMonitorTraceDefaultDuration);

                // In order to restart the trace we need to create the Stop query
                stopAfterReading = String.Format(BatchFinder.ActivityMonitorStop(ver, cloudProviderId), "RestartAfterReadingTrace");

                if (ver.Major >= 9) // SQL Server 2005/2008/2012
                {
                    stopAfterReading += String.Format(
                        BatchFinder.ActivityMonitorRestart(ver, cloudProviderId),
                        (config.TraceFileSize != null && config.TraceFileSize.Megabytes >= 1)
                            ? Math.Floor(config.TraceFileSize.Megabytes.Value)
                            : BatchConstants.DefaultQueryMonitorTraceFileSizeMegabytes,
                        (config.TraceFileRollovers >= BatchConstants.DefaultQueryMonitorTraceFileRollover)
                            ? config.TraceFileRollovers
                            : BatchConstants.DefaultQueryMonitorTraceFileRollover,
                        traceSegments,
                        filterStatements,
                        "",
                        stopTime);
                }
                else
                {
                    stopAfterReading += String.Format(
                        BatchFinder.ActivityMonitorRestart(ver, cloudProviderId),
                        traceSegments,
                        filterStatements,
                        stopTime
                        );
                }
            }

            if (ver.Major >= 9) // SQL Server 2005/2008/2012
            {
                //if the limiter is a negative value then this is to be used as the character length limit of the sql_text used in our where clause
                var sqlTextLengthLimiter = sqlTextLengthLimit < -1
                                                  ? string.Format(BatchConstants.SQLTextLengthLimiter,
                                                                  Math.Abs(sqlTextLengthLimit.Value))
                                                  : "";

                //or (datalength(TextData) < 4000 and EventClass <> 92 and EventClass <> 93)
                return String.Format(
                           BatchFinder.ActivityMonitor(ver, cloudProviderId),
                           (config.TraceFileSize != null && config.TraceFileSize.Megabytes >= 1)
                               ? Math.Floor(config.TraceFileSize.Megabytes.Value)
                               : BatchConstants.DefaultQueryMonitorTraceFileSizeMegabytes,
                           (config.TraceFileRollovers >= BatchConstants.DefaultQueryMonitorTraceFileRollover)
                               ? config.TraceFileRollovers
                               : BatchConstants.DefaultQueryMonitorTraceFileRollover,
                           traceSegments,
                           filterStatements,
                           excludeReadSegment
                               ? ""
                               : String.Format(BatchFinder.ActivityMonitorRead(ver), sqlTextLengthLimiter),
                           stopTime
                           ) + stopAfterReading;
            }
            else
            {
                string sp4Restart = null;
                // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
                // Restart trace for SQL 2000 SP4 if it has not already been restarted
                if (!hasBeenStopped && (ver.Major == 8) && (ver.Build >= 2039))
                {
                    sp4Restart = String.Format(BatchFinder.ActivityMonitorStop(ver, cloudProviderId), "SP4RestartTrace");
                    sp4Restart += String.Format(
                        BatchFinder.ActivityMonitorRestart(ver, cloudProviderId),
                        traceSegments,
                        filterStatements,
                        stopTime
                        ) + stopAfterReading;
                }

                if (ver.Major == 8)
                {
                    return String.Format(
                           BatchFinder.ActivityMonitor(ver, cloudProviderId),
                           traceSegments,
                           filterStatements,
                           excludeReadSegment ? "" : BatchFinder.ActivityMonitorRead(ver),
                           stopTime
                           ) + (stopAfterReading.Length > 0 ? stopAfterReading : sp4Restart);
                }
                else
                {
                    return String.Format(
                               BatchFinder.ActivityMonitor(ver, cloudProviderId),
                               traceSegments,
                               filterStatements,
                               excludeReadSegment ? "" : BatchFinder.ActivityMonitorRead(ver),
                               stopTime
                               ) + (stopAfterReading.Length > 0 ? stopAfterReading : sp4Restart);
                }
            }
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        internal static SqlCommand BuildActivityMonitorTraceCommand(
            SqlConnection connection,
            ServerVersion ver,
            ActivityMonitorConfiguration config,
            ActivityMonitorConfiguration prevConfig,
            DateTime? timeStamp,
            int? cloudProviderId
            )
        {
            SqlCommand cmd = connection.CreateCommand();
            bool hasBeenStopped = false;
            if (!config.Equals(prevConfig) && !(config.StopTimeUTC.HasValue && config.StopTimeUTC <= timeStamp))
            {
                cmd.CommandText = String.Format(BatchFinder.ActivityMonitorStop(ver, cloudProviderId), "ChangeTrace");
                hasBeenStopped = true;
            }

            // We need to have the trace run indefinetly when Activity Monitore trace is enabled, but we dont want to use NULL,
            // Instead we need to continuosly set a Stop Time manually and whenever we reach that Stop Time we will re set that 
            // time on the BuildActivityMonitorTraceCommandText
            if (config.StopTimeUTC == null)
            {
                config.StopTimeUTC =
                    DateTime.Now.ToUniversalTime().AddHours(Constants.QueryMonitorTraceDefaultDuration);
            }

            cmd.CommandText += BuildActivityMonitorTraceCommandText(ver, config, hasBeenStopped, false, timeStamp, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        internal static SqlCommand BuildActivityMonitorStartCommand(
            SqlConnection connection,
            ServerVersion ver,
            StartActivityMonitorTraceConfiguration qmc,
            int? cloudProviderId
            )
        {

            SqlCommand cmd = connection.CreateCommand();
            bool hasBeenStopped = false;
            if (!qmc.CurrentActivityMonitorConfig.Equals(qmc.PreviousActivityMonitorConfig))
            {
                cmd.CommandText = String.Format(BatchFinder.ActivityMonitorStop(ver, cloudProviderId), "ActivityProfilerStartAction");
                hasBeenStopped = true;
            }

            // We need to have the trace run indefinetly when Activity Monitore trace is enabled, but we dont want to use NULL,
            // Instead we need to continuosly set a Stop Time manually and whenever we reach that Stop Time we will re set that 
            // time on the BuildActivityMonitorTraceCommandText
            if (qmc.CurrentActivityMonitorConfig.StopTimeUTC == null)
            {
                qmc.CurrentActivityMonitorConfig.StopTimeUTC =
                    DateTime.Now.ToUniversalTime().AddHours(Constants.QueryMonitorTraceDefaultDuration);
            }

            cmd.CommandText += BuildActivityMonitorTraceCommandText(ver, qmc.CurrentActivityMonitorConfig, hasBeenStopped, true, null, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        internal static SqlCommand BuildActivityMonitorTraceCommandWithRestart(
            SqlConnection connection,
            ServerVersion ver,
            ActivityMonitorConfiguration config,
            DateTime? timeStamp, int? cloudProviderId)
        {
            string setting = ver.Major >= 10 ? "blocked process threshold (s)" : "blocked process threshold";

            // We need to have the trace run indefinetly when Activity Monitore trace is enabled, but we dont want to use NULL,
            // Instead we need to continuosly set a Stop Time manually and whenever we reach that Stop Time we will re set that 
            // time on the BuildActivityMonitorTraceCommandText
            if (config.StopTimeUTC == null)
            {
                config.StopTimeUTC =
                    DateTime.Now.ToUniversalTime().AddHours(Constants.QueryMonitorTraceDefaultDuration);
            }

            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = String.Format(BatchFinder.ActivityMonitorStop(ver, cloudProviderId), "RestartTrace");
            cmd.CommandText += BuildActivityMonitorTraceCommandText(ver, config, true, false, timeStamp, cloudProviderId);

            if (ver.Major > 8 && config.BlockingEventsEnabled && cloudProviderId != Constants.MicrosoftAzureId)
            {
                //only try and reconfigure if > sql 2000
                cmd.CommandText += String.Format(BatchConstants.Reconfigure, setting, config.BlockedProcessThreshold);
            }

            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        internal static SqlCommand BuildActivityMonitorStopCommand(SqlConnection connection, ServerVersion ver, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = String.Format(BatchFinder.ActivityMonitorStop(ver, cloudProviderId), "StopTrace");
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        //START SQLdm 9.1 (Ankit Srivastava) - Activity Monitoring with Extended Events -- New build command methods 

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        private static string BuildActivityMonitorCommandTextEX(ServerVersion serverVersion, ActivityMonitorConfiguration activityMonitorConfig,
            bool hasItChanged, bool excludeReadSegment, int? cloudProviderId, int? numOfCloudDbs = null)
        {
            //START SQLdm 9.1 (Ankit Srivastava) -- Fixed NUnit errors
            if (serverVersion == null || activityMonitorConfig == null) return String.Empty;
            if (serverVersion.Major <= 10) return string.Empty;//extended events for Activity Monitoring is only supported for 2012 and above
            //END SQLdm 9.1 (Ankit Srivastava) -- Fixed NUnit errors

            StringBuilder extendedEventsSegments = new StringBuilder();
            StringBuilder filterStatements = new StringBuilder();

            //default is -1 meaning do nothing. +ve means limit in the interpreter. -ve means limit in the batch
            long? sqlTextLengthLimit = CollectionServiceConfiguration.GetCollectionServiceElement().MaxQueryMonitorEventSizeKB;

            bool isEventSegmentEmpty = true;
            // Not available for SQL 2005 and below - this is handled by the batchfinder
            if (activityMonitorConfig.DeadlockEventsEnabled)
            {
                extendedEventsSegments.Append(String.Format(BatchFinder.ActivityMonitorDeadlocksEx(serverVersion, cloudProviderId), string.Empty));
                isEventSegmentEmpty = false;
            }

            if (activityMonitorConfig.BlockingEventsEnabled && serverVersion.Major > 10)
            {
                if (!isEventSegmentEmpty)
                    extendedEventsSegments.Append(" + ', '+ ");
                extendedEventsSegments.Append(String.Format(BatchFinder.ActivityMonitorBlockingEx(serverVersion,cloudProviderId), string.Empty));
                isEventSegmentEmpty = false;
            }

            if (activityMonitorConfig.AutoGrowEventsEnabled)
            {
                if (!isEventSegmentEmpty && cloudProviderId!=2)
                    extendedEventsSegments.Append(" + ', '+ ");
                extendedEventsSegments.Append(String.Format(BatchFinder.ActivityMonitorAutogrowEx(serverVersion,cloudProviderId), (serverVersion.Major > 10 ? " WHERE ([size_change_kb]>(0)) " : String.Empty)));
            }





            if (serverVersion.Major > 10) // SQL Server 2012 and above
            {
                if (cloudProviderId == Constants.MicrosoftAzureId ||
                    cloudProviderId == Constants.MicrosoftAzureManagedInstanceId)
                {
                    return String.Format(
                        BatchFinder.ActivityMonitorEx(serverVersion, cloudProviderId),
                        activityMonitorConfig.FileNameXEsession, //Not required for Azure
                        hasItChanged ? 1 : 0,
                        activityMonitorConfig.MaxDispatchLatencyXe,
                        activityMonitorConfig.FileSizeRolloverXe, //Not required for Azure
                        activityMonitorConfig.MaxMemoryXeMB,
                        activityMonitorConfig.FileSizeXeMB, //Not required for Azure
                        activityMonitorConfig.EventRetentionModeXe,
                        activityMonitorConfig.MaxEventSizeXemb,
                        activityMonitorConfig.MemoryPartitionModeXe,
                        activityMonitorConfig.StartupStateXe ? "ON" : "OFF",
                        activityMonitorConfig.TrackCausalityXe ? "ON" : "OFF",
                        extendedEventsSegments,
                        excludeReadSegment
                            ? String.Empty
                            : (cloudProviderId == 2 || cloudProviderId == 5
                                ? String.Format(BatchFinder.ActivityMonitorReadEx(serverVersion, cloudProviderId),
                                    ActivityMonitorReadEventCount2012Ex,
                                    ActivityMonitorReadEventCount2012Ex)
                                : String.Format(BatchFinder.ActivityMonitorReadEx(serverVersion, cloudProviderId)))
                        );
                }
                else
                {
                    return String.Format(
                        BatchFinder.ActivityMonitorEx(serverVersion, cloudProviderId),
                        activityMonitorConfig.FileNameXEsession, //Not required for Azure
                        hasItChanged ? 1 : 0,
                        activityMonitorConfig.MaxDispatchLatencyXe,
                        activityMonitorConfig.FileSizeRolloverXe, //Not required for Azure
                        activityMonitorConfig.MaxMemoryXeMB,
                        activityMonitorConfig.FileSizeXeMB, //Not required for Azure
                        activityMonitorConfig.EventRetentionModeXe,
                        activityMonitorConfig.MaxEventSizeXemb,
                        activityMonitorConfig.MemoryPartitionModeXe,
                        activityMonitorConfig.StartupStateXe ? "ON" : "OFF",
                        activityMonitorConfig.TrackCausalityXe ? "ON" : "OFF",
                        extendedEventsSegments,
                        excludeReadSegment ? String.Empty : String.Format(BatchFinder.ActivityMonitorReadEx(serverVersion, cloudProviderId), String.Empty)
                        );
                }
            }
            //SQLdm 9.1 (Ankit Srivastava) - Rolled back Activity Monitoring with Extended events for SQL Server 2008

            return null;
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        internal static SqlCommand BuildActivityMonitorCommandEX(SqlConnection connection, ServerVersion serverVersion, ActivityMonitorConfiguration activityMonitorconfig,
            ActivityMonitorConfiguration prevConfig, int? cloudProviderId,int? numOfCloudDbs = null)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (activityMonitorconfig == null || prevConfig == null) return cmd;// SQLdm 9.1 (Ankit Srivastava) -- Fixed NUnit errors
            bool hasItChanged = false;

            if (!activityMonitorconfig.Equals(prevConfig))
            {
                cmd.CommandText = BatchFinder.ActivityMonitorStopEx(serverVersion,cloudProviderId);
                hasItChanged = true;
            }

            cmd.CommandText += BuildActivityMonitorCommandTextEX(serverVersion, activityMonitorconfig, hasItChanged, false, cloudProviderId,numOfCloudDbs);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        internal static SqlCommand BuildActivityMonitorStartCommandEX(SqlConnection connection, ServerVersion serverVersion,
            StartActivityMonitorTraceConfiguration startActivityMonitorConfig, int?cloudProviderId)
        {

            SqlCommand cmd = connection.CreateCommand();

            if (startActivityMonitorConfig == null) return cmd;// SQLdm 9.1 (Ankit Srivastava) -- Fixed NUnit errors
            bool hasItChanged = false;
            if (!startActivityMonitorConfig.CurrentActivityMonitorConfig.Equals(startActivityMonitorConfig.PreviousActivityMonitorConfig))
            {
                cmd.CommandText = BatchFinder.ActivityMonitorStopEx(serverVersion,cloudProviderId);
                hasItChanged = true;
            }

            cmd.CommandText = BatchFinder.ActivityMonitorStopEx(serverVersion,cloudProviderId);
            cmd.CommandText += BuildActivityMonitorCommandTextEX(serverVersion, startActivityMonitorConfig.CurrentActivityMonitorConfig, hasItChanged, true, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        internal static SqlCommand BuildActivityMonitorCommandWithRestartEX(SqlConnection connection, ServerVersion serverVersion,
            ActivityMonitorConfiguration activityMonitorconfig, int? cloudProviderId)
        {
            string setting = serverVersion.Major >= 10 ? "blocked process threshold (s)" : "blocked process threshold";

            SqlCommand cmd = connection.CreateCommand();
            if (activityMonitorconfig == null) return cmd;// SQLdm 9.1 (Ankit Srivastava) -- Fixed NUnit errors
            cmd.CommandText = BatchFinder.ActivityMonitorStopEx(serverVersion,cloudProviderId);
            cmd.CommandText += BuildActivityMonitorCommandTextEX(serverVersion, activityMonitorconfig, true, false, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        // to stop the extended event session
        internal static SqlCommand BuildActivityMonitorStopCommandEX(SqlConnection connection, ServerVersion serverVersion,int ?cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();

            if (connection == null || serverVersion == null)
                return cmd;
            cmd.CommandText = BatchFinder.ActivityMonitorStopEx(serverVersion,cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        // to stop both extended event session and the SQL trace
        internal static SqlCommand BuildActivityMonitorStopCommandAll(SqlConnection connection, ServerVersion serverVersion, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();

            if (connection == null || serverVersion == null)
                return cmd;
            cmd.CommandText = String.Format(BatchFinder.ActivityMonitorStop(serverVersion, cloudProviderId), "StopTrace");
            cmd.CommandText += BatchFinder.ActivityMonitorStopEx(serverVersion,cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        //END SQLdm 9.1 (Ankit Srivastava) - Activity Monitoring with Extended Events -- New build command methods 

        #endregion

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        internal static SqlCommand BuildQueryMonitorStopCommand(SqlConnection connection, ServerVersion ver, int? cloudProviderId = null)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = String.Format(BatchFinder.QueryMonitorStop(ver, cloudProviderId), "StopTrace");
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildQueryMonitorStartCommand(
            SqlConnection connection,
            ServerVersion ver,
            StartQueryMonitorTraceConfiguration qmc,
            int? cloudProviderId
            )
        {

            // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
            SqlCommand cmd = connection.CreateCommand();
            bool hasBeenStopped = false;
            if (!qmc.CurrentQMConfig.Equals(qmc.PreviousQMConfig))
            {
                cmd.CommandText = String.Format(BatchFinder.QueryMonitorStop(ver, cloudProviderId), "QueryMonitorStartAction");
                hasBeenStopped = true;
            }

            // We need to update StopTimeUTC only when it comes null or it does not come from Alert response
            // In case it comes from Alert Respons then we certainly dont want to change that time
            //if (!qmc.CurrentQMConfig.IsAlertResponseQueryTrace && qmc.CurrentQMConfig.StopTimeUTC == null)
            //{
            //    qmc.CurrentQMConfig.StopTimeUTC = DateTime.Now.ToUniversalTime().AddHours(Common.Constants.QueryMonitorTraceDefaultDuration);
            //}

            cmd.CommandText += BuildQueryMonitorTraceCommandText(ver, qmc.CurrentQMConfig, hasBeenStopped, true, null);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        internal static SqlCommand BuildQueryMonitorTraceCommand(
            SqlConnection connection,
            ServerVersion ver,
            QueryMonitorConfiguration config,
            QueryMonitorConfiguration prevConfig,
            DateTime? timeStamp,
            int? cloudProviderId = null
            )
        {
            SqlCommand cmd = connection.CreateCommand();
            bool hasBeenStopped = false;
            if (!config.Equals(prevConfig) && !(config.StopTimeUTC.HasValue && config.StopTimeUTC <= timeStamp))
            {
                cmd.CommandText = String.Format(BatchFinder.QueryMonitorStop(ver, cloudProviderId), "ChangeTrace");
                hasBeenStopped = true;
            }

            // We need to update StopTimeUTC only when it comes null or it does not come from Alert response
            // In case it comes from Alert Respons then we certainly dont want to change that time
            //if (!config.IsAlertResponseQueryTrace && config.StopTimeUTC == null)
            //{
            //    config.StopTimeUTC = DateTime.Now.ToUniversalTime().AddHours(Common.Constants.QueryMonitorTraceDefaultDuration);
            //}

            cmd.CommandText += BuildQueryMonitorTraceCommandText(ver, config, hasBeenStopped, false, timeStamp, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        /// <summary>
        /// Build Query Monitor Stop Command for Query Store = Clears the State for Query Store
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="serverVersion">SQL Version</param>
        /// <param name="cloudProviderId">Cloud Provider</param>
        /// <returns>Stop Command</returns>
        internal static SqlCommand BuildQueryMonitorStopCommandQs(SqlConnection connection, ServerVersion serverVersion, int? cloudProviderId = null)
        {
            SqlCommand cmd = connection.CreateCommand();

            if (connection == null || serverVersion == null)
                return cmd;
            cmd.CommandText = BatchFinder.QueryMonitorStopQs(serverVersion, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        /// <summary>
        /// Build Query Monitor Restart Command for Query Store 
        /// Clears the State for Query Store and Reads data from databases where query store is enabled
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="serverVersion">SQL Version</param>
        /// <param name="config">Query Monitor Configuration to add filters</param>
        /// <param name="cloudProviderId">Cloud Provider</param>
        /// <returns>Restart Command</returns>
        internal static SqlCommand BuildQueryMonitorQsCommandWithRestart(
            SqlConnection connection,
            ServerVersion serverVersion,
            QueryMonitorConfiguration config,
            int? serverId,
            int? cloudProviderId
        )
        {
            SqlCommand cmd = connection.CreateCommand();
            if (connection == null || serverVersion == null || config == null)
            {
                return cmd;
            }

            string startTime = null;

            if (cloudProviderId != Constants.MicrosoftAzureId)
            {
                cmd.CommandText = BatchFinder.QueryMonitorStopQs(serverVersion, cloudProviderId);
            }
            else if(serverId != null)
            {
                // Set the start time on restart
                PersistenceManager.Instance.SetAzureQsStartTime(serverId.Value, connection.Database, AzureQsType.QueryMonitor, startTime);
            }
            cmd.CommandText += BuildQueryMonitorCommandTextQs(serverVersion, config, cloudProviderId, startTime);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        /// <summary>
        /// Build Query Monitor Start Command for Query Store 
        /// Reads data from databases where query store is enabled
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="serverVersion">SQL Version</param>
        /// <param name="config">Query Monitor Configuration to add filters</param>
        /// <param name="cloudProviderId">Cloud Provider</param>
        /// <returns>Read Command</returns>
        internal static SqlCommand BuildQueryMonitorQsCommand(
            SqlConnection connection,
            ServerVersion serverVersion,
            QueryMonitorConfiguration config,
            int? serverId = null,
            int? cloudProviderId = null
        )
        {
            SqlCommand cmd = connection.CreateCommand();
            if (connection == null || serverVersion == null || config == null)
            {
                return cmd;
            }
            string startTime = null;

            if (cloudProviderId == Constants.MicrosoftAzureId && serverId != null)
            {
                startTime = PersistenceManager.Instance.GetAzureQsStartTime(serverId.Value, connection.Database, AzureQsType.QueryMonitor);
            }
            cmd.CommandText += BuildQueryMonitorCommandTextQs(serverVersion, config, cloudProviderId, startTime);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        /// <summary>
        /// Build Query Monitor Start Command for Query Store 
        /// Reads data from databases where query store is enabled
        /// </summary>
        /// <param name="serverVersion">SQL Version</param>
        /// <param name="config">Query Monitor Configuration to add filters</param>
        /// <param name="cloudProviderId">Cloud Provider</param>
        /// <param name="isRestart">We might use it to set the last start time for azure</param>
        /// <returns>Start Command</returns>
        internal static string BuildQueryMonitorCommandTextQs(
            ServerVersion serverVersion,
            QueryMonitorConfiguration config,
            int? cloudProviderId,
            string startTime = null
        )
        {
            // TODO: Use isRestart for the azure server
            if (serverVersion.Major >= 13 || cloudProviderId == Constants.MicrosoftAzureManagedInstanceId
                || cloudProviderId == Constants.MicrosoftAzureId) // SQL Server 2016+
            {
                // Dynamic Query inside string 
                // Query Store filter is inside string '' in sql so we need to ensure we pass strings with double ''
                var queryStoreFilter = new StringBuilder();
                var sessionFilter = new StringBuilder();
                var databaseNamesFilter = new StringBuilder();

                queryStoreFilter.AppendLine(BatchConstants.QueryMonitorStateCheckQs);

                // Add Generic Predicate Statements
                AddPredicateStatementsQs(config, queryStoreFilter);

                // Add SQL Text Filter
                AddSqlTextFilterQs(config, queryStoreFilter);

                // Add Statement Type Filter - Stored Procedures or SQL Statements
                AddStatementTypeFilterQs(config, queryStoreFilter);

                // Add Top X ShowPlan Filter config.CollectQueryPlan
                var topPlanCategoryFilter = GetTopPlanCategoryFIlterQs(config);
                var topPlanCountFilter = config.TopPlanCountFilter.ToString();

                // Exclude Dm Filters
                AddExcludeDmFilterQs(config, sessionFilter);

                // Application Include and Exclude with Like and Not Like Filters
                AddApplicationFilterQs(config, sessionFilter);

                // Database Include and Exclude with Like and Not Like Filters
                AddDatabaseFilterQs(config, databaseNamesFilter);

                // Collect Actual Plans
                var collectActualPlans = config.CollectQueryPlan;

                // Log Filters
                LOG.Debug("BuildQueryMonitorCommandTextQs Filters for QueryMonitorReadQs");
                LOG.Debug("BuildQueryMonitorCommandTextQs::queryStoreFilter " + queryStoreFilter.ToString());
                LOG.Debug("BuildQueryMonitorCommandTextQs::sessionFilter " + sessionFilter.ToString());
                LOG.Debug("BuildQueryMonitorCommandTextQs::databaseNamesFilter " + databaseNamesFilter);
                LOG.Debug("BuildQueryMonitorCommandTextQs::topPlanCategoryFilter " + topPlanCategoryFilter);
                LOG.Debug("BuildQueryMonitorCommandTextQs::topPlanCountFilter " + topPlanCountFilter);
                LOG.Debug("BuildQueryMonitorCommandTextQs::collectActualPlans " + collectActualPlans);

                if (cloudProviderId == Constants.MicrosoftAzureId)
                {
                    return string.Format(
                        BatchFinder.QueryMonitorReadQs(serverVersion, cloudProviderId),
                        topPlanCategoryFilter,
                        queryStoreFilter.ToString(),
                        topPlanCountFilter,
                        sessionFilter.ToString(),
                        databaseNamesFilter,
                        collectActualPlans
                            ? BatchConstants.CollectActualPlanTrueQs
                            : BatchConstants.CollectActualPlanFalseQs,
                        string.IsNullOrEmpty(startTime)
                            ? "GETUTCDATE()"
                            : startTime).Replace("''", "'");
                }

                return string.Format(
                    BatchFinder.QueryMonitorReadQs(serverVersion, cloudProviderId),
                    topPlanCategoryFilter,
                    queryStoreFilter.ToString(),
                    topPlanCountFilter,
                    sessionFilter.ToString(),
                    databaseNamesFilter,
                    collectActualPlans
                        ? BatchConstants.CollectActualPlanTrueQs
                        : BatchConstants.CollectActualPlanFalseQs);
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Add Generic Filter Statements covering Match / Like / Not Like cases for inclusion or exclusion
        /// </summary>
        /// <param name="genericFormatString">Generic Comparator String</param>
        /// <param name="excludeLike">Priority over include</param>
        /// <param name="excludeMatch">Priority over include</param>
        /// <param name="includeLike">Handled only when no exclusion present</param>
        /// <param name="includeMatch">Handled only when no exclusion present</param>
        /// <param name="finalFilter">Generic Filter gets appended to this filter</param>
        /// <param name="connector">Connector when adding to <paramref name="finalFilter"/></param>
        /// <param name="starter">Starting Connector when adding to <paramref name="finalFilter"/></param>
        private static void AddGenericFilterQs(
            string genericFormatString,
            string[] excludeLike,
            string[] excludeMatch,
            string[] includeLike,
            string[] includeMatch,
            StringBuilder finalFilter,
            string connector,
            string starter)
        {
            // Stores the filter
            StringBuilder genericFilter = new StringBuilder();

            // Handle exclusion first if present
            bool isExcluded = false;
            if (excludeMatch != null)
            {
                foreach (string filterString in excludeMatch)
                {
                    if (filterString != null && filterString.Length > 0)
                    {
                        genericFilter.Append(
                            String.Format(
                                genericFormatString,
                                (!isExcluded ? BatchConstants.SpaceConnector : BatchConstants.AndConnector),
                                BatchConstants.NotConnector,
                                filterString));
                        isExcluded = true;
                    }
                }
            }
            if (excludeLike != null)
            {
                foreach (string filterString in excludeLike)
                {
                    if (filterString != null && filterString.Length > 0)
                    {
                        genericFilter.Append(
                            String.Format(
                                genericFormatString,
                                (!isExcluded ? BatchConstants.SpaceConnector : BatchConstants.AndConnector),
                                BatchConstants.NotLikeConnector,
                                filterString));
                        isExcluded = true;
                    }
                }
            }
            if (!isExcluded) //giving priority to exlclusion
            {
                bool isFirst = true;
                if (includeMatch != null)
                {
                    foreach (string filterString in includeMatch)
                    {
                        if (filterString != null && filterString.Length > 0)
                        {
                            genericFilter.Append(
                                String.Format(
                                    genericFormatString,
                                    (isFirst ? BatchConstants.LeftParenthesisConnector : BatchConstants.OrConnector),
                                    BatchConstants.EqualsConnector,
                                    filterString));
                            isFirst = false;
                        }
                    }
                }
                if (includeLike != null)
                {
                    foreach (string filterString in includeLike)
                    {
                        if (filterString != null && filterString.Length > 0)
                        {
                            genericFilter.Append(
                                String.Format(
                                    genericFormatString,
                                    (isFirst ? BatchConstants.LeftParenthesisConnector : BatchConstants.OrConnector),
                                    BatchConstants.LikeConnector,
                                    filterString));
                            isFirst = false;
                        }
                    }
                }
                if (!isFirst)
                {
                    genericFilter.Append(BatchConstants.RightParenthesisConnector);
                }
            }
            // Add to final filter
            AddToFinalFilter(genericFilter, finalFilter, connector, starter);
        }

        /// <summary>
        /// Add Application filters based on configuration, will be part of where clause
        /// </summary>
        /// <param name="config">for getting application filters</param>
        /// <param name="sessionFilter">conditions gets appended to this filter</param>
        private static void AddApplicationFilterQs(QueryMonitorConfiguration config, StringBuilder sessionFilter)
        {
            if(config.AdvancedConfiguration != null)
            {
                AddGenericFilterQs(
                BatchConstants.QueryMonitorApplicationFilterQs,
                config.AdvancedConfiguration.ApplicationExcludeLike,
                config.AdvancedConfiguration.ApplicationExcludeMatch,
                config.AdvancedConfiguration.ApplicationIncludeLike,
                config.AdvancedConfiguration.ApplicationIncludeMatch,
                sessionFilter,
                BatchConstants.AndConnector,
                BatchConstants.WhereConnector);}
        }

        /// <summary>
        /// Add Databae filters based on configuration, will be part of if statement
        /// </summary>
        /// <param name="config">for getting required filters</param>
        /// <param name="databaseFilter">conditions gets appended to this filter</param>
        /// <remarks>
        /// Adds 1=1 condition if no condition found for databases
        /// </remarks>
        private static void AddDatabaseFilterQs(QueryMonitorConfiguration config, StringBuilder databaseFilter)
        {
            if(config.AdvancedConfiguration != null)
            {
                AddGenericFilterQs(
                BatchConstants.QueryMonitorDatabaseFilterQs,
                config.AdvancedConfiguration.DatabaseExcludeLike,
                config.AdvancedConfiguration.DatabaseExcludeMatch,
                config.AdvancedConfiguration.DatabaseIncludeLike,
                config.AdvancedConfiguration.DatabaseIncludeMatch,
                databaseFilter,
                BatchConstants.AndConnector,
                BatchConstants.SpaceConnector);}

            // Cannot pass empty filter since inside if statement
            if (databaseFilter.Length == 0)
            {
                databaseFilter.Append(BatchConstants.QueryMonitorAlwaysTrueFilterQs);
            }
        }

        /// <summary>
        /// Add Exclude SQLdm filter based on Application Filters
        /// </summary>
        /// <param name="config">for getting required filters</param>
        /// <param name="sessionFilter">conditions gets appended to this filter</param>
        private static void AddExcludeDmFilterQs(QueryMonitorConfiguration config, StringBuilder sessionFilter)
        {
            if (config.AdvancedConfiguration != null && config.AdvancedConfiguration.ExcludeDM)
            {
                sessionFilter.Append(
                    String.Format(
                        BatchConstants.QueryMonitorApplicationFilterQs,
                        (sessionFilter.Length == 0 ? BatchConstants.WhereConnector : BatchConstants.AndConnector),
                        BatchConstants.NotLikeConnector,
                        Constants.ConnectionStringApplicationNamePrefix + BatchConstants.PercentageChar));

                sessionFilter.Append(
                    String.Format(
                        BatchConstants.QueryMonitorApplicationFilterQs,
                        (sessionFilter.Length == 0 ? BatchConstants.WhereConnector : BatchConstants.AndConnector),
                        BatchConstants.NotConnector,
                        BatchConstants.DiagnosticManExcludeFilter));
            }
        }

        /// <summary>
        /// Get Top Plan Category to order defines Top Plan by
        /// </summary>
        /// <param name="config">Query Configuration for getting required filter</param>
        /// <returns>Category</returns>
        private static string GetTopPlanCategoryFIlterQs(QueryMonitorConfiguration config)
        {
            var topPlanCategory = string.Empty;
            switch (config.TopPlanCategoryFilter)
            {
                case BatchConstants.ConfigTopPlanDurationCategoryFilter:
                    topPlanCategory = BatchConstants.TopPlanDurationCategoryQs;
                    break;
                case BatchConstants.ConfigTopPlanReadCategoryFilter:
                    topPlanCategory = BatchConstants.TopPlanReadCategoryQs;
                    break;
                case BatchConstants.ConfigTopPlanCpuCategoryFilter:
                    topPlanCategory = BatchConstants.TopPlanCpuCategoryQs;
                    break;
                case BatchConstants.ConfigTopPlanWriteCategoryFilter:
                    topPlanCategory = BatchConstants.TopPlanWriteCategoryQs;
                    break;
            }
            return topPlanCategory;
        }

        /// <summary>
        /// Add conditions based on Statement Type defined on type of sys.objects
        /// </summary>
        /// <param name="config">for getting required filters</param>
        /// <param name="queryStoreFilter">conditions gets appended to this filter</param>
        private static void AddStatementTypeFilterQs(QueryMonitorConfiguration config, StringBuilder queryStoreFilter, string connector = BatchConstants.AndConnector, string starter = BatchConstants.WhereConnector)
        {
            // Include Stored Procedure and Statements Filter
            var statementTypeFilter = new StringBuilder();
            
            // If need to collect for both SP and Statements no need of any where conditions
            //  By default collects all query data for SP and Statements
            if (!(config.StoredProcedureEventsEnabled && config.SqlStatementEventsEnabled))
            {
                if (config.StoredProcedureEventsEnabled)
                {
                    statementTypeFilter.AppendLine(BatchConstants.SqlProcedureFilterQs);
                }
                else
                {
                    statementTypeFilter.AppendLine(BatchConstants.SqlStatementsFilterQs);
                }
            }
            AddToFinalFilter(statementTypeFilter, queryStoreFilter, connector, starter);
        }

        /// <summary>
        /// Adds generic filter to final filter if null using starter otherwise using connector
        /// </summary>
        /// <param name="genericfilter">filter to be added</param>
        /// <param name="finalFilter">main filter</param>
        /// <param name="connector">connector to connect generic filter with main filter</param>
        /// <param name="starter">connector to append before generic filter while setting main filter</param>
        /// <example>
        /// <paramref name="genericfilter"/> = " sql_text=''s'' "
        /// <paramref name="connector"/> = " AND "
        /// <paramref name="starter"/> = " WHERE "
        /// If <paramref name="finalFilter"/> is empty we use 
        ///   " WHERE sql_text=''s''  "
        /// else
        ///   ..." AND sql_text=''s''  "
        /// </example>
        private static void AddToFinalFilter(StringBuilder genericfilter, StringBuilder finalFilter, string connector, string starter)
        {
            if (genericfilter.Length > 0)
            {
                finalFilter.Append(finalFilter.Length > 0 ? connector : starter);
                finalFilter.Append(genericfilter.ToString());
            }
        }

        /// <summary>
        /// Adds generic filter to final filter if null using starter otherwise using connector
        /// </summary>
        /// <param name="genericfilter">filter to be added</param>
        /// <param name="finalFilter">main filter</param>
        /// <param name="connector">connector to connect generic filter with main filter</param>
        /// <param name="starter">connector to append before generic filter while setting main filter</param>
        /// <example>
        /// <paramref name="genericfilter"/> = " sql_text=''s'' "
        /// <paramref name="connector"/> = " AND "
        /// <paramref name="starter"/> = " WHERE "
        /// If <paramref name="finalFilter"/> is empty we use 
        ///   " WHERE sql_text=''s''  "
        /// else
        ///   ..." AND sql_text=''s''  "
        /// </example>
        private static void AddToFinalFilter(string genericString, StringBuilder finalFilter, string connector, string starter)
        {
            if (!string.IsNullOrEmpty(genericString))
            {
                finalFilter.Append(finalFilter.Length > 0 ? connector : starter);
                finalFilter.Append(genericString);
            }
        }

        /// <summary>
        /// Add conditions on SQL Text and its Length
        /// </summary>
        /// <param name="config">for getting required filters</param>
        /// <param name="queryStoreFilter">conditions gets appended to this filter</param>
        private static void AddSqlTextFilterQs(QueryMonitorConfiguration config, StringBuilder queryStoreFilter)
        {
            AddToFinalFilter(BatchConstants.SqlTextNotNullConditionQs, queryStoreFilter, BatchConstants.AndConnector, BatchConstants.WhereConnector);
            
            if(config.AdvancedConfiguration != null)
            {
                AddGenericFilterQs(
                BatchConstants.QueryMonitorTextDataFilterQs,
                config.AdvancedConfiguration.SqlTextExcludeLike,
                config.AdvancedConfiguration.SqlTextExcludeMatch,
                config.AdvancedConfiguration.SqlTextIncludeLike,
                config.AdvancedConfiguration.SqlTextIncludeMatch,
                queryStoreFilter,
                BatchConstants.AndConnector,
                BatchConstants.WhereConnector);
            }

            //default is -1 meaning do nothing. +ve means limit in the interpreter. -ve means limit in the batch
            var sqlTextLengthLimit = CollectionServiceConfiguration.GetCollectionServiceElement().MaxQueryMonitorEventSizeKB;
            //if the limiter is a negative value then this is to be used as the character length limit of the sql_text used in our where clause
            var sqlTextLengthLimitCondition = sqlTextLengthLimit < -1
                                                  ? string.Format(
                                                      BatchConstants.SqlTextLengthLimiterQs,
                                                      Math.Abs(sqlTextLengthLimit))
                                                  : string.Empty;
            AddToFinalFilter(sqlTextLengthLimitCondition, queryStoreFilter, BatchConstants.AndConnector, BatchConstants.WhereConnector);
        }

        /// <summary>
        /// Add conditions based on Duration / CPU / Reads / Writes
        /// </summary>
        /// <param name="config">for getting required filters</param>
        /// <param name="queryStoreFilter">conditions gets appended to this filter</param>
        private static void AddPredicateStatementsQs(QueryMonitorConfiguration config, StringBuilder queryStoreFilter)
        {
            // Format duration filter)
            if (config.DurationFilter != null && config.DurationFilter.TotalMilliseconds > 0)
            {
                queryStoreFilter.Append(queryStoreFilter.Length > 0 ? BatchConstants.AndConnector : BatchConstants.WhereConnector);
                queryStoreFilter.Append(
                    String.Format(
                        BatchConstants.QueryMonitorDurationPredicateQs,
                        // Query Monitoring with Query Store -- making adjustment on the duration value according to the version
                        // Microseconds for SQL 2008 +
                        config.DurationFilter.TotalMilliseconds * 1000));
            }

            // Format cpu filter
            if (config.CpuUsageFilter != null && config.CpuUsageFilter.TotalMilliseconds > 0)
            {
                queryStoreFilter.Append(queryStoreFilter.Length > 0 ? BatchConstants.AndConnector : BatchConstants.WhereConnector);
                queryStoreFilter.Append(
                    String.Format(
                        BatchConstants.QueryMonitorCpuTimeConsumedPredicateQs,
                        // Query Monitoring with Query Store -- making adjustment on the cpu usage value according to the version
                        config.CpuUsageFilter.TotalMilliseconds * 1000));
            }

            // Fomat reads filter
            if (config.LogicalDiskReads > 0)
            {
                queryStoreFilter.Append(queryStoreFilter.Length > 0 ? BatchConstants.AndConnector : BatchConstants.WhereConnector);
                queryStoreFilter.Append(string.Format(BatchConstants.QueryMonitorReadsPredicateQs, config.LogicalDiskReads));
            }

            // Fomat writes filter
            if (config.PhysicalDiskWrites > 0)
            {
                queryStoreFilter.Append(queryStoreFilter.Length > 0 ? BatchConstants.AndConnector : BatchConstants.WhereConnector);
                queryStoreFilter.Append(String.Format(BatchConstants.QueryMonitorWritesPredicateQs, config.PhysicalDiskWrites));
            }
        }

        public static string BuildFilteredMonitoredDatabasesAzureQuery(string predicates)
        {
            return string.Format(BatchFinder.GetFilteredMonitoredDatabasesAzure(), predicates);
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        internal static SqlCommand BuildQueryMonitorTraceCommandWithRestart(
            SqlConnection connection,
            ServerVersion ver,
            QueryMonitorConfiguration config,
            DateTime? timeStamp, 
            int? cloudProviderId = null
            )
        {
            // We need to update StopTimeUTC only when it comes null or it does not come from Alert response
            // In case it comes from Alert Respons then we certainly dont want to change that time
            //if (!config.IsAlertResponseQueryTrace && config.StopTimeUTC == null)
            //{
            //    config.StopTimeUTC = DateTime.Now.ToUniversalTime().AddHours(Common.Constants.QueryMonitorTraceDefaultDuration);
            //}

            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = String.Format(BatchFinder.QueryMonitorStop(ver, cloudProviderId), "RestartTrace");
            cmd.CommandText += BuildQueryMonitorTraceCommandText(ver, config, true, false, timeStamp, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        //START SQLdm 10.0 (Tarun Sapra) --For getting estimated query, when actual plan is not available
        //It will build the query for getting the estimated plan from the database
        internal static SqlCommand BuildQueryMonitorReadEstimatedPlanCommand(SqlConnection connection, ServerVersion ver, string sqlQuery)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += String.Format(BatchFinder.QueryMonitorReadEstimatedPlan(ver), sqlQuery);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        //END SQLdm 10.0 (Tarun Sapra) --For getting estimated query, when actual plan is not available

        private static string BuildQueryMonitorStringFilter(
            TraceColumn column,
            TraceLogicalOperator op,
            TraceComparisonOperator comp,
            string filter)
        {
            return String.Format(BatchConstants.QueryMonitorFilterString, (int)column, (int)op, (int)comp, filter);
        }

        private static string BuildQueryMonitorExcludeMatch(TraceColumn column, string filter)
        {
            return BuildQueryMonitorStringFilter(column,
                                                 TraceLogicalOperator.And,
                                                 TraceComparisonOperator.NotEquals,
                                                 filter);
        }

        private static string BuildQueryMonitorExcludeLike(TraceColumn column, string filter)
        {
            return BuildQueryMonitorStringFilter(column,
                                                 TraceLogicalOperator.And,
                                                 TraceComparisonOperator.NotLike,
                                                 filter);
        }

        //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters -start
        private static string BuildQueryMonitorIncludeMatch(TraceColumn column, string filter, bool isFirst)
        {
            return BuildQueryMonitorStringFilter(column,
                                                 isFirst ? TraceLogicalOperator.And : TraceLogicalOperator.Or,
                                                 TraceComparisonOperator.Equals,
                                                 filter);
        }

        private static string BuildQueryMonitorIncludeLike(TraceColumn column, string filter, bool isFirst)
        {
            return BuildQueryMonitorStringFilter(column,
                                                 isFirst ? TraceLogicalOperator.And : TraceLogicalOperator.Or,
                                                 TraceComparisonOperator.Like,
                                                 filter);
        }
        //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters - end

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        internal static string BuildQueryMonitorTraceCommandText(
            ServerVersion ver,
            QueryMonitorConfiguration config,
            bool hasBeenStopped,
            bool excludeReadSegment,
            DateTime? timeStamp,
            int? cloudProviderId = null
            )
        {
            StringBuilder traceSegments = new StringBuilder();
            StringBuilder filterStatements = new StringBuilder();

            //default is -1 meaning do nothing. +ve means limit in the interpreter. -ve means limit in the batch
            long? sqlTextLengthLimit = CollectionServiceConfiguration.GetCollectionServiceElement().MaxQueryMonitorEventSizeKB;

            // Add code to monitor stored procs
            if (config.StoredProcedureEventsEnabled)
            {
                traceSegments.Append(BatchFinder.QueryMonitorTraceSP(ver));
            }

            if (config.SqlStatementEventsEnabled)
            {
                traceSegments.Append(BatchFinder.QueryMonitorTraceSingleStmt(ver));
            }

            if (config.SqlBatchEventsEnabled)
            {
                traceSegments.Append(BatchFinder.QueryMonitorTraceBatches(ver));
            }

            //// Not available for SQL 2000 - this is handled by the batchfinder
            //if (config.DeadlockEventsEnabled)
            //{
            //    traceSegments.Append(BatchFinder.QueryMonitorTraceDeadlocks(ver));
            //}

            //traceSegments.Append(String.Format(BatchFinder.QueryMonitorTraceAutogrow(ver),ver.Major == 8 ? "@P2" : "@P1"));

            // Format duration filter)
            if (config.DurationFilter != null && config.DurationFilter.TotalMilliseconds > 0)
            {
                filterStatements.Append(
                    String.Format(BatchConstants.QueryMonitorDurationFilter,
                                  (ver.Major >= 9)
                                      ?
                    // Microseconds for SQL 2005/2008
                                  config.DurationFilter.TotalMilliseconds * 1000
                                      :
                    // Milliseconds for SQL 2000
                                  config.DurationFilter.TotalMilliseconds
                        ));
            }

            // Format cpu filter
            if (config.CpuUsageFilter != null && config.CpuUsageFilter.TotalMilliseconds > 0)
            {
                filterStatements.Append(
                    String.Format(BatchConstants.QueryMonitorCPUTimeConsumedFilter,
                                  config.CpuUsageFilter.TotalMilliseconds));
            }

            // Fomat reads filter
            if (config.LogicalDiskReads > 0)
            {
                filterStatements.Append(
                    String.Format(BatchConstants.QueryMonitorReadsFilter, config.LogicalDiskReads));
            }

            // Fomat writes filter
            if (config.PhysicalDiskWrites > 0)
            {
                filterStatements.Append(
                    String.Format(BatchConstants.QueryMonitorWritesFilter, config.PhysicalDiskWrites));
            }

            if (config.AdvancedConfiguration.ExcludeDM)
            {
                filterStatements.Append(
                    BuildQueryMonitorExcludeLike(TraceColumn.ApplicationName,
                                                 Constants.ConnectionStringApplicationNamePrefix + BatchConstants.PercentageChar)
                    );


                filterStatements.Append(
                    BuildQueryMonitorExcludeMatch(TraceColumn.ApplicationName, BatchConstants.DiagnosticManExcludeFilter)
                    );

            }

            bool isAppExcluded = false;
            if (config.AdvancedConfiguration.ApplicationExcludeLike != null)
            {
                foreach (string filterString in config.AdvancedConfiguration.ApplicationExcludeLike)
                {
                    if (filterString != null && filterString.Length > 0)
                    {
                        isAppExcluded = true;
                        filterStatements.Append(
                            BuildQueryMonitorExcludeLike(TraceColumn.ApplicationName, filterString)
                            );
                    }
                }
            }

            if (config.AdvancedConfiguration.ApplicationExcludeMatch != null)
            {
                foreach (string filterString in config.AdvancedConfiguration.ApplicationExcludeMatch)
                {
                    if (filterString != null && filterString.Length > 0)
                    {
                        isAppExcluded = true;
                        filterStatements.Append(
                            BuildQueryMonitorExcludeMatch(TraceColumn.ApplicationName, filterString)
                            );
                    }
                }
            }

            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters
            bool isFirstApp = true;
            if (!isAppExcluded)
            {
                if (config.AdvancedConfiguration.ApplicationIncludeLike != null)
                {
                    foreach (string filterString in config.AdvancedConfiguration.ApplicationIncludeLike)
                    {
                        if (filterString != null && filterString.Length > 0)
                        {
                            filterStatements.Append(
                                BuildQueryMonitorIncludeLike(TraceColumn.ApplicationName, filterString, isFirstApp)
                                );
                            isFirstApp = false;
                        }
                    }
                }

                if (config.AdvancedConfiguration.ApplicationIncludeMatch != null)
                {
                    foreach (string filterString in config.AdvancedConfiguration.ApplicationIncludeMatch)
                    {
                        if (filterString != null && filterString.Length > 0)
                        {
                            filterStatements.Append(
                                BuildQueryMonitorIncludeMatch(TraceColumn.ApplicationName, filterString, isFirstApp)
                                );
                            isFirstApp = false;
                        }
                    }
                }
            }
            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters

            bool isDbExcluded = false;
            if (config.AdvancedConfiguration.DatabaseExcludeLike != null)
            {
                foreach (string filterString in config.AdvancedConfiguration.DatabaseExcludeLike)
                {
                    if (filterString != null && filterString.Length > 0)
                    {
                        isDbExcluded = true;
                        filterStatements.Append(
                        BuildQueryMonitorExcludeLike(TraceColumn.DatabaseName, filterString)
                        );
                    }
                }
            }


            if (config.AdvancedConfiguration.DatabaseExcludeMatch != null)
            {
                foreach (string filterString in config.AdvancedConfiguration.DatabaseExcludeMatch)
                {
                    if (filterString != null && filterString.Length > 0)
                    {
                        isDbExcluded = true;
                        filterStatements.Append(
                            BuildQueryMonitorExcludeMatch(TraceColumn.DatabaseName, filterString)
                            );
                    }
                }
            }


            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters
            bool isFirstDb = true;
            if (!isDbExcluded)
            {
                if (config.AdvancedConfiguration.DatabaseIncludeLike != null)
                {
                    foreach (string filterString in config.AdvancedConfiguration.DatabaseIncludeLike)
                    {
                        if (filterString != null && filterString.Length > 0)
                        {
                            filterStatements.Append(
                            BuildQueryMonitorIncludeLike(TraceColumn.DatabaseName, filterString, isFirstDb)
                            );
                            isFirstDb = false;
                        }
                    }
                }
                if (config.AdvancedConfiguration.DatabaseIncludeMatch != null)
                {
                    foreach (string filterString in config.AdvancedConfiguration.DatabaseIncludeMatch)
                    {
                        if (filterString != null && filterString.Length > 0)
                        {
                            filterStatements.Append(
                                BuildQueryMonitorIncludeMatch(TraceColumn.DatabaseName, filterString, isFirstDb)
                                );
                            isFirstDb = false;
                        }
                    }
                }
            }
            //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters


            string stopAfterReading = "";
            string restartAfterReading = "";

            if (config.IsAlertResponseQueryTrace && config.StopTimeUTC.HasValue && config.StopTimeUTC <= timeStamp)
            {
                // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
                stopAfterReading = String.Format(BatchFinder.QueryMonitorStop(ver, cloudProviderId), "StopAfterReadingTrace");
            }

            const string DateFormat = "yyyy'-'MM'-'dd HH':'mm':'ss";
            const string DateText = "'{0}'";

            var stopTime = config.StopTimeUTC != null && config.StopTimeUTC.HasValue &&
                           (timeStamp == null || config.StopTimeUTC >= timeStamp)
                               ? string.Format(DateText, config.StopTimeUTC.Value.AddMinutes(10).ToString(DateFormat))
                               : string.Format(DateText,
                                               DateTime.Now.ToUniversalTime().AddHours(
                                                   Constants.QueryMonitorTraceDefaultDuration).ToString(
                                                       DateFormat));

            // If is not Alert Responce Query trace then we need to have unlimited tracing. In that case and if 
            // the Stop Time reaches the timestamp then we need to restart the trace.
            if (!config.IsAlertResponseQueryTrace && config.StopTimeUTC.HasValue && config.StopTimeUTC <= timeStamp.Value.AddHours(1))
            {
                // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
                restartAfterReading = String.Format(BatchFinder.QueryMonitorStop(ver, cloudProviderId), "RestartAfterReadingTrace");

                if (ver.Major >= 9)
                {
                    restartAfterReading += String.Format(
                        BatchFinder.QueryMonitorRestart(ver, cloudProviderId),
                        (config.TraceFileSize != null && config.TraceFileSize.Megabytes >= 1)
                            ? Math.Floor(config.TraceFileSize.Megabytes.Value)
                            : BatchConstants.DefaultQueryMonitorTraceFileSizeMegabytes,
                        (config.TraceFileRollovers >= BatchConstants.DefaultQueryMonitorTraceFileRollover)
                            ? config.TraceFileRollovers
                            : BatchConstants.DefaultQueryMonitorTraceFileRollover,
                        traceSegments,
                        filterStatements,
                        "",
                        stopTime);
                }
                else
                {
                    restartAfterReading += String.Format(
                        BatchFinder.QueryMonitorRestart(ver, cloudProviderId),
                        traceSegments,
                        filterStatements,
                        stopTime
                        );
                }
            }

            if (ver.Major >= 9) // SQL Server 2005/2008
            {
                //if the limiter is a negative value then this is to be used as the character length limit of the sql_text used in our where clause
                var sqlTextLengthLimiter = sqlTextLengthLimit < -1
                                                  ? string.Format(BatchConstants.SQLTextLengthLimiter,
                                                                  Math.Abs(sqlTextLengthLimit.Value))
                                                  : "";

                //or (datalength(TextData) < 4000 and EventClass <> 92 and EventClass <> 93)
                return String.Format(
                           BatchFinder.QueryMonitor(ver, cloudProviderId),
                           (config.TraceFileSize != null && config.TraceFileSize.Megabytes >= 1)
                               ? Math.Floor(config.TraceFileSize.Megabytes.Value)
                               : BatchConstants.DefaultQueryMonitorTraceFileSizeMegabytes,
                           (config.TraceFileRollovers >= BatchConstants.DefaultQueryMonitorTraceFileRollover)
                               ? config.TraceFileRollovers
                               : BatchConstants.DefaultQueryMonitorTraceFileRollover,
                           traceSegments,
                           filterStatements,
                           excludeReadSegment
                               ? ""
                               : String.Format(BatchFinder.QueryMonitorRead(ver), sqlTextLengthLimiter),
                           stopTime
                           ) + (config.IsAlertResponseQueryTrace ? stopAfterReading : restartAfterReading);
            }
            else
            {
                // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
                string sp4Restart = null;
                // Restart trace for SQL 2000 SP4 if it has not already been restarted
                if (!hasBeenStopped && (ver.Major == 8) && (ver.Build >= 2039))
                {
                    sp4Restart = String.Format(BatchFinder.QueryMonitorStop(ver, cloudProviderId), "SP4RestartTrace");
                    sp4Restart += String.Format(
                        BatchFinder.QueryMonitorRestart(ver, cloudProviderId),
                        traceSegments,
                        filterStatements,
                        stopTime
                        ) + stopAfterReading;
                }

                if (ver.Major == 8)
                {
                    return String.Format(
                           BatchFinder.QueryMonitor(ver, cloudProviderId),
                           traceSegments,
                           filterStatements,
                           excludeReadSegment ? "" : BatchFinder.QueryMonitorRead(ver),
                           stopTime
                           ) + (stopAfterReading.Length > 0 ? stopAfterReading : sp4Restart);
                }
                else
                {
                    return String.Format(
                            BatchFinder.QueryMonitor(ver, cloudProviderId),
                            traceSegments,
                            filterStatements,
                            excludeReadSegment ? "" : BatchFinder.QueryMonitorRead(ver),
                            stopTime
                            ) + (stopAfterReading.Length > 0 ? stopAfterReading : sp4Restart);
                }
            }
        }

        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- New method for creating queries-start
        private static string BuildQueryMonitorExcludeMatchEx(string action, string filter, bool isEmptyTillNow)
        {
            string logicalOperator = isEmptyTillNow ? " where " : " and ";
            return (logicalOperator + action + " <> ''" + filter + "''");
        }

        //SQLdm 9.0 (Ankit Srivastava) - Query Monitoring with Extended Event Session -- Removed extra parameter
        private static string BuildQueryMonitorExcludeLikeEx(string action, string filter, bool isEmptyTillNow)
        {
            string logicalOperator = isEmptyTillNow ? " where " : " and ";
            return (logicalOperator + " NOT (" + BatchConstants.LikeStringPredicate2012 + "(" + action + ",N''" + filter + "''))");
        }

        private static string BuildQueryMonitorIncludeMatchEx(string action, string filter, bool isFirst, bool isEmptyTillNow)
        {
            string logicalOperator = isEmptyTillNow ? " where " : (isFirst ? " and " : " or ");
            return (logicalOperator + action + " = ''" + filter + "''");
        }

        //SQLdm 9.0 (Ankit Srivastava) - Query Monitoring with Extended Event Session -- Removed extra parameter
        private static string BuildQueryMonitorIncludeLikeEx(string action, string filter, bool isFirst, bool isEmptyTillNow)
        {
            string logicalOperator = isEmptyTillNow ? " where " : (isFirst ? " and " : " or ");
            return (logicalOperator + " (" + BatchConstants.LikeStringPredicate2012 + "(" + action + ",N''" + filter + "''))");
        }
        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Events -- New method for creating queries -end

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- New method for stopping the trace and the extended event session -start
        internal static SqlCommand BuildQueryMonitorStopCommandAll(SqlConnection connection, ServerVersion serverVersion, int? cloudProviderId)
        {
            // Note: Add Stop Query Store Stop command when its configurable
            SqlCommand cmd = connection.CreateCommand();

            if (connection == null || serverVersion == null)
                return cmd;
            cmd.CommandText = String.Format(BatchFinder.QueryMonitorStop(serverVersion, cloudProviderId), "StopTrace");
            cmd.CommandText += BatchFinder.QueryMonitorStopEx(serverVersion, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- New method for stopping the trace and the extended event session - end 

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- New method for stopping it -start
        internal static SqlCommand BuildQueryMonitorStopCommandEX(SqlConnection connection, ServerVersion serverVersion, int? cloudProviderId = null)
        {
            SqlCommand cmd = connection.CreateCommand();

            if (connection == null || serverVersion == null)
                return cmd;
            cmd.CommandText = BatchFinder.QueryMonitorStopEx(serverVersion, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- New method for stopping it - end

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- New method for starting it -start
        internal static SqlCommand BuildQueryMonitorStartCommandEX(
           SqlConnection connection,
           ServerVersion serverVersion,
           StartQueryMonitorTraceConfiguration qmc,
           ActiveWaitsConfiguration waitConfig,
           int? cloudProviderId = null
           )
        {

            SqlCommand cmd = connection.CreateCommand();
            if (connection == null || serverVersion == null || qmc == null || qmc.PreviousQMConfig == null || qmc.CurrentQMConfig == null || waitConfig == null)
                return cmd;

            bool hasBeenStopped = false;
            if (!qmc.CurrentQMConfig.Equals(qmc.PreviousQMConfig))
            {
                cmd.CommandText = BatchFinder.QueryMonitorStopEx(serverVersion, cloudProviderId);
                hasBeenStopped = true;
            }

            // We need to update StopTimeUTC only when it comes null or it does not come from Alert response
            // In case it comes from Alert Respons then we certainly dont want to change that time
            //if (!qmc.CurrentQMConfig.IsAlertResponseQueryTrace && qmc.CurrentQMConfig.StopTimeUTC == null)
            //{
            //    qmc.CurrentQMConfig.StopTimeUTC = DateTime.Now.ToUniversalTime().AddHours(Common.Constants.QueryMonitorTraceDefaultDuration);
            //}

            cmd.CommandText += BuildQueryMonitorCommandTextEX(serverVersion, qmc.CurrentQMConfig, waitConfig, !hasBeenStopped, null, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- New method for starting it -end

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- New method for running it - start
        internal static SqlCommand BuildQueryMonitorEXCommand(
            SqlConnection connection,
            ServerVersion serverVersion,
            QueryMonitorConfiguration config,
            QueryMonitorConfiguration prevConfig,
            ActiveWaitsConfiguration waitConfig,
            DateTime? timeStamp,
            bool hasBeenStopped,
            int? cloudProviderId = null,
            int? numOfCloudDbs = null
            )
        {
            SqlCommand cmd = connection.CreateCommand();
            if (connection == null || serverVersion == null || config == null || prevConfig == null || waitConfig == null)
                return cmd;

            if (!config.Equals(prevConfig) && !(config.StopTimeUTC.HasValue && config.StopTimeUTC <= timeStamp))
            {
                cmd.CommandText = BatchFinder.QueryMonitorStopEx(serverVersion, cloudProviderId);
                hasBeenStopped = true;
            }

            // We need to update StopTimeUTC only when it comes null or it does not come from Alert response
            // In case it comes from Alert Respons then we certainly dont want to change that time
            //if (!config.IsAlertResponseQueryTrace && config.StopTimeUTC == null)
            //{
            //    config.StopTimeUTC = DateTime.Now.ToUniversalTime().AddHours(Common.Constants.QueryMonitorTraceDefaultDuration);
            //}

            cmd.CommandText += BuildQueryMonitorCommandTextEX(serverVersion, config, waitConfig, !hasBeenStopped, timeStamp, cloudProviderId, numOfCloudDbs);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- New method for running it - end

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- New method for restarting it -start
        internal static SqlCommand BuildQueryMonitorEXCommandWithRestart(
            SqlConnection connection,
            ServerVersion serverVersion,
            QueryMonitorConfiguration config,
            ActiveWaitsConfiguration waitConfig,
            DateTime? timeStamp,
            int? cloudProviderId = null,
            int? numOfCloudDbs = null
            )
        {
            SqlCommand cmd = connection.CreateCommand();
            if (connection == null || serverVersion == null || config == null || waitConfig == null)
                return cmd;

            // We need to update StopTimeUTC only when it comes null or it does not come from Alert response
            // In case it comes from Alert Respons then we certainly dont want to change that time
            //if (!config.IsAlertResponseQueryTrace && config.StopTimeUTC == null)
            //{
            //    config.StopTimeUTC = DateTime.Now.ToUniversalTime().AddHours(Common.Constants.QueryMonitorTraceDefaultDuration);
            //}


            cmd.CommandText = BatchFinder.QueryMonitorStopEx(serverVersion, cloudProviderId);
            cmd.CommandText += BuildQueryMonitorCommandTextEX(serverVersion, config, waitConfig, true, timeStamp, cloudProviderId, numOfCloudDbs);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- New method for restarting it -end 

        // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- New method for building the Extended Event session command -Start 
        internal static string BuildQueryMonitorCommandTextEX(
            ServerVersion serverVersion,
            QueryMonitorConfiguration config,
            ActiveWaitsConfiguration waitConfig,
            bool isExists,
            DateTime? timeStamp,
            int? cloudProviderId,
            int? numOfCloudDbs = null
            )
        {
            bool isSQL2008 = serverVersion.Major == 10;

            StringBuilder eventsSegments = new StringBuilder();
            StringBuilder moduleEndPredicatesStatements = new StringBuilder();
            StringBuilder ShowPlanPredicatesStatements = new StringBuilder();
            StringBuilder genericPredicatesStatements = new StringBuilder();

            //default is -1 meaning do nothing. +ve means limit in the interpreter. -ve means limit in the batch
            long? sqlTextLengthLimit = CollectionServiceConfiguration.GetCollectionServiceElement().MaxQueryMonitorEventSizeKB;

            //if (!config.IsAlertResponseQueryTrace && config.StopTimeUTC == null)
            //{
            //    config.StopTimeUTC = DateTime.Now.ToUniversalTime().AddHours(Common.Constants.QueryMonitorTraceDefaultDuration);
            //}

            // Format duration filter)
            if (config.DurationFilter != null && config.DurationFilter.TotalMilliseconds > 0)
            {
                genericPredicatesStatements.Append(" where ");
                genericPredicatesStatements.Append(
                    String.Format(BatchConstants.QueryMonitorDurationPredicate,
                                  //start --SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- making adjustment on the duration value according to the version
                                  // Microseconds for SQL 2008 +
                                  config.DurationFilter.TotalMilliseconds * 1000
                        //end --SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- making adjustment on the duration value according to the version
                        ));

            }


            if (config.StoredProcedureEventsEnabled)
            {
                moduleEndPredicatesStatements = new StringBuilder();
                moduleEndPredicatesStatements.Append(genericPredicatesStatements.ToString());
            }

            // Format cpu filter
            if (config.CpuUsageFilter != null && config.CpuUsageFilter.TotalMilliseconds > 0)
            {
                if (genericPredicatesStatements.Length > 0)
                    genericPredicatesStatements.Append(" and ");
                else
                    genericPredicatesStatements.Append(" where ");

                // SQLDM 10.2.2 - SQLDM-28101 Query information not collected on certain instances through Extended Events. Using SQL Tracing collects the Query statistics.
                if (isSQL2008)
                {
                    // SQL 2008
                    genericPredicatesStatements.Append(
                    String.Format(BatchConstants.QueryMonitorCPUTimeConsumedPredicate2008,
                    //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- making adjustment on the cpu usage value according to the version
                    config.CpuUsageFilter.TotalMilliseconds));
                }
                else
                {
                    // SQL 2012 and above
                    genericPredicatesStatements.Append(
                    String.Format(BatchConstants.QueryMonitorCPUTimeConsumedPredicate,
                    //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- making adjustment on the cpu usage value according to the version
                    config.CpuUsageFilter.TotalMilliseconds * 1000));
                }
            }

            ShowPlanPredicatesStatements.Append(genericPredicatesStatements.ToString()); // setting duration and cpu filters

            // Fomat reads filter
            if (config.LogicalDiskReads > 0)
            {
                if (genericPredicatesStatements.Length > 0)
                    genericPredicatesStatements.Append(" and ");
                else
                    genericPredicatesStatements.Append(" where ");

                // SQLDM-28101 (Varun Chopra) Query information not collected through Extended Events
                genericPredicatesStatements.Append(isSQL2008
                                ? string.Format("reads >= {0}", config.LogicalDiskReads)
                                : string.Format("logical_reads >= {0}", config.LogicalDiskReads));
            }

            // Fomat writes filter
            if (config.PhysicalDiskWrites > 0)
            {
                if (genericPredicatesStatements.Length > 0)
                    genericPredicatesStatements.Append(" and ");
                else
                    genericPredicatesStatements.Append(" where ");

                // SQLDM-28101 (Varun Chopra) Query information not collected through Extended Events
                genericPredicatesStatements.Append(String.Format("writes >= {0}", config.PhysicalDiskWrites));
            }

            StringBuilder applicationFilter = new StringBuilder();//SQLdm 9.0 (Ankit Srivastava) - Query Monitoring Improvement with Extended Events - declared application filter 
            var additionalPredicatesStatements = new StringBuilder();
            if (config.AdvancedConfiguration.ExcludeDM)
            {

                //START - SQLdm 9.0 (Ankit Srivastava) - Query Monitoring Improvement with Extended Events - used new filter formats for SQL 2008 for NOT LIKE
                if (isSQL2008)
                    applicationFilter.Append(
                        String.Format(BatchConstants.QueryMonitorApplicationFilter2008, (applicationFilter.Length == 0 ? BatchConstants.SpaceConnector : " and "), " not like ", Constants.ConnectionStringApplicationNamePrefix + BatchConstants.PercentageChar)
                                );

                else
                    additionalPredicatesStatements.Append(
                        BuildQueryMonitorExcludeLikeEx(Constants.ExtendedEventActionsApplicationName,
                                                     Constants.ConnectionStringApplicationNamePrefix + BatchConstants.PercentageChar, true)
                        );
                //END - SQLdm 9.0 (Ankit Srivastava) - Query Monitoring Improvement with Extended Events - used new filter formats for SQL 2008 for NOT LIKE

                additionalPredicatesStatements.Append(
                    BuildQueryMonitorExcludeMatchEx(Constants.ExtendedEventActionsApplicationName, BatchConstants.DiagnosticManExcludeFilter, isSQL2008)
                    );


            }

            bool isAppExcluded = false;
            if (config.AdvancedConfiguration.ApplicationExcludeLike != null)
            {
                foreach (string filterString in config.AdvancedConfiguration.ApplicationExcludeLike)
                {
                    if (filterString != null && filterString.Length > 0)
                    {
                        isAppExcluded = true;
                        //Start - SQLdm 9.0 (Ankit Srivastava) - Query Monitoring Improvement with Extended Events - used new filter formats for SQL 2008 for NOT LIKE
                        if (isSQL2008)
                        {
                            applicationFilter.Append(
                                String.Format(BatchConstants.QueryMonitorApplicationFilter2008, (applicationFilter.Length == 0 ? BatchConstants.SpaceConnector : " and "), " not like ", filterString)
                                );
                        }
                        else
                            additionalPredicatesStatements.Append(
                                BuildQueryMonitorExcludeLikeEx(Constants.ExtendedEventActionsApplicationName, filterString, additionalPredicatesStatements.Length == 0)
                                );
                        //End - SQLdm 9.0 (Ankit Srivastava) - Query Monitoring Improvement with Extended Events - used new filter formats for SQL 2008 for NOT LIKE
                    }
                }
            }

            if (config.AdvancedConfiguration.ApplicationExcludeMatch != null)
            {
                foreach (string filterString in config.AdvancedConfiguration.ApplicationExcludeMatch)
                {
                    if (filterString != null && filterString.Length > 0)
                    {
                        isAppExcluded = true;
                        additionalPredicatesStatements.Append(
                            BuildQueryMonitorExcludeMatchEx(Constants.ExtendedEventActionsApplicationName, filterString, additionalPredicatesStatements.Length == 0)
                            );
                    }
                }
            }


            bool isFirstApp = true;
            if (!isAppExcluded)//giving priority to exlclusion
            {
                if (config.AdvancedConfiguration.ApplicationIncludeLike != null)
                {
                    foreach (string filterString in config.AdvancedConfiguration.ApplicationIncludeLike)
                    {
                        if (filterString != null && filterString.Length > 0)
                        {
                            //Start - SQLdm 9.0 (Ankit Srivastava) - Query Monitoring Improvement with Extended Events - used new filter formats for SQL 2008 for LIKE
                            if (isSQL2008)
                            {
                                applicationFilter.Append(
                                    String.Format(BatchConstants.QueryMonitorApplicationFilter2008, (isFirstApp ? BatchConstants.LeftParenthesisConnector : " or "), " like ", filterString)
                                    );
                            }
                            else
                                additionalPredicatesStatements.Append(
                                    BuildQueryMonitorIncludeLikeEx(Constants.ExtendedEventActionsApplicationName, filterString, isFirstApp, additionalPredicatesStatements.Length == 0)
                                    );
                            isFirstApp = false;
                        }
                    }
                }
                if (!isFirstApp && isSQL2008)
                    applicationFilter.Append(BatchConstants.RightParenthesisConnector);
                //End - SQLdm 9.0 (Ankit Srivastava) - Query Monitoring Improvement with Extended Events - used new filter formats for SQL 2008 for LIKE

                if (config.AdvancedConfiguration.ApplicationIncludeMatch != null)
                {
                    foreach (string filterString in config.AdvancedConfiguration.ApplicationIncludeMatch)
                    {
                        if (filterString != null && filterString.Length > 0)
                        {
                            additionalPredicatesStatements.Append(
                                BuildQueryMonitorIncludeMatchEx(Constants.ExtendedEventActionsApplicationName, filterString, isFirstApp, additionalPredicatesStatements.Length == 0)
                                );
                            isFirstApp = false;
                        }
                    }
                }
            }

            StringBuilder databaseFilter = new StringBuilder();//SQLdm 9.0 (Ankit Srivastava) - Query Monitoring Improvement with Extended Events - declared db filter 
            bool isDbExcluded = false;
            if (cloudProviderId != Constants.MicrosoftAzureId && cloudProviderId != Constants.MicrosoftAzureManagedInstanceId) // SQLdm 11.0 Azure is db Specific. It is being handled in ScheduledRefresh Probe.
            {
                if (config.AdvancedConfiguration.DatabaseExcludeLike != null)
                {
                    foreach (string filterString in config.AdvancedConfiguration.DatabaseExcludeLike)
                    {
                        if (filterString != null && filterString.Length > 0)
                        {
                            isDbExcluded = true;
                            //Start - SQLdm 9.0 (Ankit Srivastava) - Query Monitoring Improvement with Extended Events - used new filter formats for SQL 2008 for NOT LIKE 
                            if (isSQL2008)
                            {
                                databaseFilter.Append(
                                        String.Format(BatchConstants.QueryMonitorDatabaseFilter2008, (databaseFilter.Length == 0 ? BatchConstants.SpaceConnector : " and "), " not like ", filterString)
                                        );
                            }
                            else
                                additionalPredicatesStatements.Append(
                                BuildQueryMonitorExcludeLikeEx(Constants.ExtendedEventActionsDatabaseName, filterString, additionalPredicatesStatements.Length == 0)
                                );
                            //End - SQLdm 9.0 (Ankit Srivastava) - Query Monitoring Improvement with Extended Events - used new filter formats for SQL 2008 for NOT LIKE 
                        }
                    }
                }


                if (config.AdvancedConfiguration.DatabaseExcludeMatch != null)
                {
                    foreach (string filterString in config.AdvancedConfiguration.DatabaseExcludeMatch)
                    {
                        if (filterString != null && filterString.Length > 0)
                        {
                            isDbExcluded = true;
                            //START-SQLdm 9.1 (Ankit Srivastava) --Creating db filter for 2008 instead of predicates
                            if (isSQL2008)
                            {
                                databaseFilter.Append(
                                        String.Format(BatchConstants.QueryMonitorDatabaseFilter2008, (databaseFilter.Length == 0 ? BatchConstants.SpaceConnector : " and "), BatchConstants.NotConnector, filterString)
                                        );
                            }
                            else
                                //END-SQLdm 9.1 (Ankit Srivastava) --Creating db filter for 2008 instead of predicates
                                additionalPredicatesStatements.Append(
                                    BuildQueryMonitorExcludeMatchEx(Constants.ExtendedEventActionsDatabaseName, filterString, additionalPredicatesStatements.Length == 0)
                                    );
                        }
                    }
                }

                bool isFirstDb = true;
                if (!isDbExcluded) //giving priority to exclusion
                {
                    if (config.AdvancedConfiguration.DatabaseIncludeLike != null)
                    {
                        foreach (string filterString in config.AdvancedConfiguration.DatabaseIncludeLike)
                        {
                            if (filterString != null && filterString.Length > 0)
                            {
                                //Start - SQLdm 9.0 (Ankit Srivastava) - Query Monitoring Improvement with Extended Events - used new filter formats for SQL 2008 for LIKE 
                                if (isSQL2008)
                                {
                                    databaseFilter.Append(
                                        String.Format(BatchConstants.QueryMonitorDatabaseFilter2008, (isFirstDb ? BatchConstants.LeftParenthesisConnector : " or "), " like ", filterString)
                                        );
                                }
                                else
                                    additionalPredicatesStatements.Append(
                                    BuildQueryMonitorIncludeLikeEx(Constants.ExtendedEventActionsDatabaseName, filterString, isFirstDb, additionalPredicatesStatements.Length == 0)
                                    );
                                isFirstDb = false;
                            }
                        }
                    }
                    //END - SQLdm 9.0 (Ankit Srivastava) - Query Monitoring Improvement with Extended Events - used new filter formats for SQL 2008 for LIKE

                    if (config.AdvancedConfiguration.DatabaseIncludeMatch != null)
                    {
                        foreach (string filterString in config.AdvancedConfiguration.DatabaseIncludeMatch)
                        {
                            if (filterString != null && filterString.Length > 0)
                            {
                                //START-SQLdm 9.1 (Ankit Srivastava) --Creating db filter for 2008 instead of predicates
                                if (isSQL2008)
                                {
                                    databaseFilter.Append(
                                        String.Format(BatchConstants.QueryMonitorDatabaseFilter2008, (isFirstDb ? BatchConstants.LeftParenthesisConnector : " or "), BatchConstants.EqualsConnector, filterString)
                                        );
                                }
                                else
                                    //END-SQLdm 9.1 (Ankit Srivastava) --Creating db filter for 2008 instead of predicates
                                    additionalPredicatesStatements.Append(
                                        BuildQueryMonitorIncludeMatchEx(Constants.ExtendedEventActionsDatabaseName, filterString, isFirstDb, additionalPredicatesStatements.Length == 0)
                                        );
                                isFirstDb = false;
                            }
                        }
                    }

                    if (!isFirstDb && isSQL2008)
                        databaseFilter.Append(BatchConstants.RightParenthesisConnector);
                }
            }
            // END CONDITION
            StringBuilder sqlTextFilter = new StringBuilder();
            var isSQLTextExcluded = false;
            if (config.AdvancedConfiguration.SqlTextExcludeLike != null)
            {
                foreach (string filterString in config.AdvancedConfiguration.SqlTextExcludeLike)
                {
                    if (filterString != null && filterString.Length > 0)
                    {
                        sqlTextFilter.Append(
                            //SQLdm 9.0 (Ankit Srivastava) - Query Monitoring Improvement with Extended Events - Corrected Operator
                            String.Format(serverVersion.Major >= 11 ? BatchConstants.QueryMonitorTextDataFilter2012 : BatchConstants.QueryMonitorTextDataFilter2008, (!isSQLTextExcluded ? BatchConstants.SpaceConnector : " and "), " not like ", filterString)
                            );
                        isSQLTextExcluded = true;
                    }
                }
            }

            if (config.AdvancedConfiguration.SqlTextExcludeMatch != null)
            {
                foreach (string filterString in config.AdvancedConfiguration.SqlTextExcludeMatch)
                {
                    if (filterString != null && filterString.Length > 0)
                    {
                        sqlTextFilter.Append(
                            //SQLdm 9.0 (Ankit Srivastava) - Query Monitoring Improvement with Extended Events - Corrected Operator
                            String.Format(serverVersion.Major >= 11 ? BatchConstants.QueryMonitorTextDataFilter2012 : BatchConstants.QueryMonitorTextDataFilter2008, (!isSQLTextExcluded ? BatchConstants.SpaceConnector : " and "), BatchConstants.NotConnector, filterString)
                            );
                        isSQLTextExcluded = true;
                    }
                }
            }

            //Creating the SQL Text filter (this is not applied at the events but while readin
            bool isFirstTextFilter = true;
            if (!isSQLTextExcluded) //giving priority to exlclusion
            {
                if (config.AdvancedConfiguration.SqlTextIncludeMatch != null)
                {
                    foreach (string filterString in config.AdvancedConfiguration.SqlTextIncludeMatch)
                    {
                        if (filterString != null && filterString.Length > 0)
                        {
                            sqlTextFilter.Append(
                                String.Format(serverVersion.Major >= 11 ? BatchConstants.QueryMonitorTextDataFilter2012 : BatchConstants.QueryMonitorTextDataFilter2008, (isFirstTextFilter ? " (" : " or "), BatchConstants.EqualsConnector, filterString)
                            );
                            isFirstTextFilter = false;
                        }
                    }
                }
                if (config.AdvancedConfiguration.SqlTextIncludeLike != null)
                {
                    foreach (string filterString in config.AdvancedConfiguration.SqlTextIncludeLike)
                    {
                        if (filterString != null && filterString.Length > 0)
                        {
                            sqlTextFilter.Append(
                            String.Format(serverVersion.Major >= 11 ? BatchConstants.QueryMonitorTextDataFilter2012 : BatchConstants.QueryMonitorTextDataFilter2008, (isFirstTextFilter ? " (" : " or "), " like ", filterString)
                                );
                            isFirstTextFilter = false;
                        }
                    }
                }
            }
            if (!isFirstTextFilter)
                sqlTextFilter.Append(BatchConstants.RightParenthesisConnector);//Start - SQLdm 9.0 (Ankit Srivastava) - Query Monitoring Improvement with Extended Events - Added extra space to collsion fo words


            //if the primary StringBuilder is not empty then replace the where with and
            if (moduleEndPredicatesStatements.Length == 0)
                moduleEndPredicatesStatements.Append(additionalPredicatesStatements.ToString());
            else
                moduleEndPredicatesStatements.Append(additionalPredicatesStatements.ToString().Replace("where", "and"));

            //start --SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring Improvement - removed duration filter for the showPlan events
            if (config.CollectQueryPlan)
            {
                //START SQLdm 9.1 (Ankit Srivastava) -- Fixed DE44573 - replacing where if duration filter exists already
                if (ShowPlanPredicatesStatements.Length == 0)
                    ShowPlanPredicatesStatements.Append(additionalPredicatesStatements.ToString());
                else
                    ShowPlanPredicatesStatements.Append(additionalPredicatesStatements.ToString().Replace("where", "and"));
                //END SQLdm 9.1 (Ankit Srivastava) -- Fixed DE44573 - replacing where if duration filter exists already
            }
            //end --SQLdm 9.0 (Ankit Srivastava) -- Query Monitoring Improvement - removed duration filter for the showPlan events

            if (genericPredicatesStatements.Length == 0)
                genericPredicatesStatements.Append(additionalPredicatesStatements.ToString());
            else
                genericPredicatesStatements.Append(additionalPredicatesStatements.ToString().Replace("where", "and"));

            var planHandleAction = config.CollectQueryPlan ? "sqlserver.plan_handle," : String.Empty;


            // Add code to monitor stored procs
            if (config.StoredProcedureEventsEnabled)
            {
                if (eventsSegments.Length > 0)
                    eventsSegments.Append(" + ");
                eventsSegments.Append(String.Format(BatchFinder.QueryMonitorExtendedEventsSP(serverVersion, cloudProviderId), moduleEndPredicatesStatements.ToString(), planHandleAction));
            }

            if (config.SqlStatementEventsEnabled)
            {
                if (eventsSegments.Length > 0)
                    eventsSegments.Append(" + ");
                eventsSegments.AppendLine(String.Format(BatchFinder.QueryMonitorExtendedEventsSingleStmt(serverVersion, cloudProviderId), genericPredicatesStatements.ToString(), planHandleAction));
            }

            if (config.SqlBatchEventsEnabled && serverVersion.Major > 10)
            {
                if (eventsSegments.Length > 0)
                    eventsSegments.Append(" + ");
                eventsSegments.AppendLine(String.Format(BatchFinder.QueryMonitorExtendedEventsBatches(serverVersion, cloudProviderId), genericPredicatesStatements.ToString(), planHandleAction));
            }

            string stopAfterReading = String.Empty;
            string restartAfterReading = String.Empty;

            // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
            if (config.IsAlertResponseQueryTrace && config.StopTimeUTC.HasValue && config.StopTimeUTC <= timeStamp)
            {
                stopAfterReading = BatchFinder.QueryMonitorStopEx(serverVersion, cloudProviderId);
            }

            const string DateFormat = BatchConstants.QueryMonitorDateTimeFormat;
            const string DateText = "'{0}'";

            var stopTime = config.StopTimeUTC != null && config.StopTimeUTC.HasValue &&
                           (timeStamp == null || config.StopTimeUTC >= timeStamp)
                               ? string.Format(DateText, config.StopTimeUTC.Value.AddMinutes(10).ToString(DateFormat))
                               : string.Format(DateText,
                                               DateTime.Now.ToUniversalTime().AddHours(
                                                   Constants.QueryMonitorTraceDefaultDuration).ToString(
                                                       DateFormat));

            // If is not Alert Responce Query trace then we need to have unlimited tracing. In that case and if 
            // the Stop Time reaches the timestamp then we need to restart the trace.
            if (!config.IsAlertResponseQueryTrace && config.StopTimeUTC.HasValue && config.StopTimeUTC <= timeStamp.Value.AddHours(1))
            {
                restartAfterReading = BatchFinder.QueryMonitorStopEx(serverVersion, cloudProviderId);

                if (serverVersion.Major >= 11) // SQL Server 2012
                {
                    restartAfterReading += String.Format(
                        BatchFinder.QueryMonitorRestartEX(serverVersion, cloudProviderId),
                        ShowPlanPredicatesStatements,
                        Constants.ExtendedEventActionsFileName,//fileName
                        isExists ? 1 : 0,
                        waitConfig.MaxDispatchLatencyXe,
                        //START SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback - assigning new property values  for query monitoring extended event session configuration
                        config.FileSizeRolloverXe,
                        waitConfig.MaxMemoryXeMB,
                        config.FileSizeXeMB,
                        //END SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback - assigning new property values  for query monitoring extended event session configuration
                        waitConfig.EventRetentionModeXe,
                        waitConfig.MaxEventSizeXemb,
                        waitConfig.MemoryPartitionModeXe,
                        waitConfig.StartupStateXe ? "ON" : "OFF",
                        waitConfig.TrackCausalityXe ? "ON" : "OFF",
                        eventsSegments,
                        config.CollectQueryPlan ? 1 : 0
                        );
                }
                //SQLdm 9.0 (Ankit Srivastava) - modified the if and else condition
                else if (serverVersion.Major == 10) // SQL Server 2008
                {
                    restartAfterReading += String.Format(
                        BatchFinder.QueryMonitorRestartEX(serverVersion, cloudProviderId),
                        Constants.ExtendedEventActionsFileName,//fileName
                        isExists ? 1 : 0,
                        waitConfig.MaxDispatchLatencyXe,
                        //START SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback - assigning new property values  for query monitoring extended event session configuration
                        config.FileSizeRolloverXe,
                        waitConfig.MaxMemoryXeMB,
                        config.FileSizeXeMB,
                        //END SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback - assigning new property values  for query monitoring extended event session configuration
                        waitConfig.EventRetentionModeXe,
                        waitConfig.MaxEventSizeXemb,
                        waitConfig.MemoryPartitionModeXe,
                        waitConfig.StartupStateXe ? "ON" : "OFF",
                        waitConfig.TrackCausalityXe ? "ON" : "OFF",
                        eventsSegments
                        );
                }
            }

            //if the limiter is a negative value then this is to be used as the character length limit of the sql_text used in our where clause
            var sqlTextLengthLimiter = sqlTextLengthLimit < -1
                                              ? string.Format(BatchConstants.SQLTextLengthLimiter,
                                                              Math.Abs(sqlTextLengthLimit.Value))
                                              : "";
            //or (datalength(TextData) < 4000 and EventClass <> 92 and EventClass <> 93)

            // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
            if (serverVersion.Major >= 11) // SQL Server 2012
            {
                int waitTime = waitConfig.CollectionTimeSeconds;
                if(numOfCloudDbs != null && numOfCloudDbs > 0)
                {
                    int dbCount = numOfCloudDbs.GetValueOrDefault();
                    waitTime = (int)Math.Max(1, Math.Round(Convert.ToDecimal(waitTime/dbCount), 0, MidpointRounding.AwayFromZero));
                }
                return String.Format(
                           BatchFinder.QueryMonitorEX(serverVersion, cloudProviderId),
                           ShowPlanPredicatesStatements,
                           Constants.ExtendedEventActionsFileName,//fileName
                           isExists ? 1 : 0,
                           waitConfig.MaxDispatchLatencyXe,
                           //START SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback - assigning new property values  for query monitoring extended event session configuration
                           config.FileSizeRolloverXe,
                           waitConfig.MaxMemoryXeMB,
                           config.FileSizeXeMB,
                           //END SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback - assigning new property values  for query monitoring extended event session configuration    
                           waitConfig.EventRetentionModeXe,
                           waitConfig.MaxEventSizeXemb,
                           waitConfig.MemoryPartitionModeXe,
                           waitConfig.StartupStateXe ? "ON" : "OFF",
                           waitConfig.TrackCausalityXe ? "ON" : "OFF",
                           eventsSegments,                   
                            string.Format(
                               BatchFinder.QueryMonitorReadEX(serverVersion, cloudProviderId),
                               config.CollectQueryPlan ? 1 : 0,
                               Math.Round(
                                   (decimal)Math.Min(SqlHelper.CommandTimeout, waitTime) * 3 / 4,
                                   0,
                                   MidpointRounding.AwayFromZero),
                               QueryMonitorReadEventCount2012Ex),
                           config.CollectQueryPlan ? 1 : 0)
                       + (config.IsAlertResponseQueryTrace ? stopAfterReading : restartAfterReading);
            }
            //SQLdm 9.0 (Ankit Srivastava) - modified the if and else condition
            else if (serverVersion.Major == 10) // SQL Server 2008
            {
                //START SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback - construting the filter string for read query
                StringBuilder filterString = new StringBuilder();
                if (sqlTextFilter.Length == 0)
                {
                    if (applicationFilter.Length == 0)
                    {
                        if (databaseFilter.Length > 0)
                        {
                            filterString.Append(" where ");
                            filterString.Append(databaseFilter.ToString());
                        }
                    }
                    else
                    {
                        filterString.Append(" where ");
                        filterString.Append(applicationFilter.ToString());
                        if (databaseFilter.Length > 0)
                        {
                            filterString.Append(" and ");
                            filterString.Append(databaseFilter.ToString());
                        }
                    }
                }
                else
                {
                    filterString.Append(" where ");
                    filterString.Append(sqlTextFilter.ToString());
                    if (applicationFilter.Length == 0)
                    {
                        if (databaseFilter.Length > 0)
                        {
                            filterString.Append(" and ");
                            filterString.Append(databaseFilter.ToString());
                        }
                    }
                    else
                    {
                        filterString.Append(" and ");
                        filterString.Append(applicationFilter.ToString());
                        if (databaseFilter.Length > 0)
                        {
                            filterString.Append(" and ");
                            filterString.Append(databaseFilter.ToString());
                        }
                    }
                }
                // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
                if (filterString.Length == 0)
                    filterString.Append(BatchConstants.WhereConnector);
                else
                    filterString.Append(BatchConstants.AndConnector);
                filterString.Append(" (TextData is NOT NULL AND RTrim(LTrim(TextData)) <> '') ");
                //END SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback - construting the filter string for read query
                return String.Format(
                        BatchFinder.QueryMonitorEX(serverVersion, cloudProviderId),
                        Constants.ExtendedEventActionsFileName,//fileName
                        isExists ? 1 : 0,
                        waitConfig.MaxDispatchLatencyXe,
                        //START SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback - assigning new property values  for query monitoring extended event session configuration
                        config.FileSizeRolloverXe,
                        waitConfig.MaxMemoryXeMB,
                        config.FileSizeXeMB,
                        //END SQLdm 9.1 (Ankit Srivastava) - Query Monitoring Extended events 9.0 feedback - assigning new property values  for query monitoring extended event session configuration
                        waitConfig.EventRetentionModeXe,
                        waitConfig.MaxEventSizeXemb,
                        waitConfig.MemoryPartitionModeXe,
                        waitConfig.StartupStateXe ? "ON" : "OFF",
                        waitConfig.TrackCausalityXe ? "ON" : "OFF",
                        eventsSegments,
                        //SQLdm 9.0 (Ankit Srivastava) - Query Monitoring Improvement with Extended Events - Added new filter statements for SQL 2008 for LIKE and NOT LIKE 
                        string.Format(BatchFinder.QueryMonitorReadEX(serverVersion, cloudProviderId),
                        filterString.ToString(),         //{0}

                        //START - SQLdm 10.4 (Nikhil Bansal) - Query Monitor Performance Improvement - Added a Top X Plan Filter and a Filter Category
                        config.TopPlanCountFilter,       //{1}
                        config.TopPlanCategoryFilter)    //{2}
                                                         //END - SQLdm 10.4 (Nikhil Bansal) - Query Monitor Performance Improvement - Added a Top X Plan Filter and a Filter Category

                        ) + (config.IsAlertResponseQueryTrace ? stopAfterReading : restartAfterReading);
            }
            else return String.Empty;
        }
        //SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session-- New method for building the Extended Event session command -end 

        internal static SqlCommand BuildServerOverviewCommand(SqlConnection connection, ServerVersion ver, WmiConfiguration wmiConfiguration, int? cloudProviderId = null,string dbname=null)//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added a new optional param 
        {
            //START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added a new optional param 
            var spOAContext = (int)wmiConfiguration.OleAutomationContext;
            LogSpOAContext(connection.DataSource, "ServerOverview", spOAContext);
            string fileActivity = BuildFileActivity(ver, wmiConfiguration, cloudProviderId);
            string tempdbSummary = ver.Major > 8 ? BatchFinder.TempdbSummary(ver,cloudProviderId) : null;
            string waitStats = ver.Major > 8 ? BuildWaitStats(ver,cloudProviderId) : null;
            SqlCommand cmd = connection.CreateCommand();
            
            cmd.CommandText =
                String.Format(BatchFinder.ServerOverview(ver, cloudProviderId),
                              spOAContext,
                              BatchFinder.SessionCount(ver, cloudProviderId),
                              BatchFinder.LockCounterStatistics(ver,cloudProviderId),
                              wmiConfiguration.OleAutomationDisabled ? 1 : 0,
                              wmiConfiguration.DirectWmiEnabled ? 1 : 0) + fileActivity + tempdbSummary + waitStats;
            //END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Added a new optional param 
            if(cloudProviderId==AZURE_CLOUD_ID||cloudProviderId==AZURE_MANAGED_CLOUD_ID)
            {
                cmd.CommandText = cmd.CommandText + BatchFinder.AzureSQLMetric();
            }
            if(cloudProviderId==Constants.MicrosoftAzureId)
            {
                cmd.CommandText = cmd.CommandText + string.Format(BatchFinder.AzureElasticPool(),dbname);
            }
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildSessionsCommand(SqlConnection connection, ServerVersion ver,
                                                        SessionsConfiguration configuration, int? cloudProviderId = null,String dbname=null)
        {
            using (LOG.DebugCall("BuildSessionsCommand"))
            {
                //sqldm-30013 start
                if (dbname == null)
                    dbname = "master";
                //sqldm-30013 end
                SqlCommand cmd = connection.CreateCommand();

                LOG.Debug(String.Format("Appending process filters"));
                StringBuilder processesFilter = new StringBuilder();
                if (configuration.Active)
                {
                    LOG.Verbose("Appending: IncludeActiveProcesses");
                    processesFilter.Append(ver.Major == 8 ? BatchConstants.IncludeActiveProcesses : BatchConstants.IncludeActiveProcesses2005);
                }
                if (configuration.Blocked)
                {
                    LOG.Verbose("Appending: IncludeBlockedProcesses");
                    processesFilter.Append(ver.Major == 8 ? BatchConstants.IncludeBlockedProcesses : BatchConstants.IncludeBlockedProcesses2005);
                }
                if (configuration.LeadBlockers)
                {
                    LOG.Verbose("Appending: IncludeLeadBlockers");
                    processesFilter.Append(ver.Major == 8 ? BatchConstants.IncludeLeadBlockers : BatchConstants.IncludeLeadBlockers2005);
                }
                if (configuration.BlockingBlocked)
                {
                    LOG.Verbose("Appending: IncludeBlockingBlockedProcesses");
                    processesFilter.Append(ver.Major == 8 ? BatchConstants.IncludeBlockingBlockedProcesses : BatchConstants.IncludeBlockingBlockedProcesses2005);
                }
                if (configuration.BlockingOrBlocked)
                {
                    LOG.Verbose("Appending: IncludeBlockingOrBlockedProcesses");
                    processesFilter.Append(ver.Major == 8 ? BatchConstants.IncludeBlockingOrBlockedProcesses : BatchConstants.IncludeBlockingOrBlockedProcesses2005);
                }
                if (configuration.ConsumingCpu)
                {
                    LOG.Verbose("Appending: IncludeProcessesConsumingCPU");
                    processesFilter.Append(ver.Major == 8 ? BatchConstants.IncludeProcessesConsumingCPU : BatchConstants.IncludeProcessesConsumingCPU2005);
                }
                if (configuration.Locking)
                {
                    LOG.Verbose("Appending: IncludeLockingProcesses");
                    processesFilter.Append(ver.Major == 8 ? BatchConstants.IncludeLockingProcesses : BatchConstants.IncludeLockingProcesses2005);
                }
                if (configuration.NonSharedLocking)
                {
                    LOG.Verbose("Appending: IncludeNonSharedLockingProcesses");
                    processesFilter.Append(ver.Major == 8 ? BatchConstants.IncludeNonSharedLockingProcesses : BatchConstants.IncludeNonSharedLockingProcesses2005);
                }
                if (configuration.OpenTransactions)
                {
                    LOG.Verbose("Appending: IncludeProcessesWithOpenTransactions");
                    processesFilter.Append(BatchConstants.IncludeProcessesWithOpenTransactions);
                }
                if (configuration.TempdbAffecting && ver.Major >= 9)
                {
                    LOG.Verbose("Appending: TempdbAffecting");
                    processesFilter.Append(BatchConstants.IncludeTempdbAffecting);
                }
                if (!String.IsNullOrEmpty(configuration.SearchTerm))
                {
                    LOG.Verbose("Appending: IncludeProcessesSearchTerm {" + configuration.SearchTerm + "}");
                    string term = configuration.SearchTerm.Replace("'", "''");
                    if (!term.Contains("%"))
                        term = "%" + term + "%";

                    processesFilter.AppendFormat(ver.Major == 8 ? BatchConstants.IncludeProcessesSearchTerm : BatchConstants.IncludeProcessesSearchTerm2005, term);
                }
                if (!String.IsNullOrWhiteSpace(configuration.ApplicationIncludeFilter))
                {
                    processesFilter.AppendFormat(BatchConstants.IncludeLike, "program_name", configuration.ApplicationIncludeFilter);
                }
                if (!String.IsNullOrWhiteSpace(configuration.ApplicationExcludeFilter))
                {
                    processesFilter.AppendFormat(BatchConstants.ExcludeLike, "program_name", configuration.ApplicationExcludeFilter);
                }
                if (!String.IsNullOrWhiteSpace(configuration.HostIncludeFilter))
                {
                    processesFilter.AppendFormat(BatchConstants.IncludeLike, "host_name", configuration.HostIncludeFilter);
                }
                if (!String.IsNullOrWhiteSpace(configuration.HostExcludeFilter))
                {
                    processesFilter.AppendFormat(BatchConstants.ExcludeLike, "host_name", configuration.HostExcludeFilter);
                }


                if (configuration.Spid.HasValue)
                {   // if requesting a specific spid change the filter string to only filter on spid
                    if (ver.Major == 8)
                    {
                        processesFilter.Length = 0;
                        processesFilter.AppendFormat("and a.spid = {0} \n", configuration.Spid.Value);
                    }
                    else
                    {
                        processesFilter.Length = 0;
                        processesFilter.AppendFormat("and sess.session_id = {0} \n", configuration.Spid.Value);

                    }
                }

                processesFilter.Append("\n");


                // Processes to exclude
                if (configuration.ExcludeDiagnosticManagerProcesses)
                {
                    LOG.Debug("Excluding SQL Diagnostic Manager processes");
                    processesFilter.Append(
                        String.Format(ver.Major == 8 ? BatchConstants.ExcludeDMProcesses : BatchConstants.ExcludeDMProcesses2005, Constants.ConnectionStringApplicationNamePrefix));
                    processesFilter.Append("\n");
                }

                if (configuration.ExcludeSystemProcesses)
                {
                    LOG.Debug("Excluding system processes");
                    processesFilter.Append(ver.Major == 8 ? BatchConstants.ExcludeSystemProcesses : BatchConstants.ExcludeSystemProcesses2005);
                    processesFilter.Append("\n");
                }

                // put this section after any where clause filtering
                int top = CollectionServiceConfiguration.GetCollectionServiceElement().MaxRowCountSessions;
                if (ver.Major == 8)
                {
                    if (configuration.TopCpu)
                    {
                        top = configuration.TopLimit;
                        processesFilter.Append("and a.cpu > 0 \n");
                        processesFilter.Append("order by a.cpu desc\n");
                    }
                    else if (configuration.TopIo)
                    {
                        top = configuration.TopLimit;
                        processesFilter.Append("and a.physical_io > 0 \n");
                        processesFilter.Append("order by a.physical_io desc\n");
                    }
                    else if (configuration.TopMemory)
                    {
                        top = configuration.TopLimit;
                        processesFilter.Append("and a.memusage > 0 \n");
                        processesFilter.Append("order by a.memusage desc\n");
                    }
                    else if (configuration.TopWait)
                    {
                        top = configuration.TopLimit;
                        processesFilter.Append("and a.waittime > 0 \n");
                        processesFilter.Append("order by a.waittime desc\n");
                    }

                }
                else
                {
                    if (configuration.TopCpu)
                    {
                        top = configuration.TopLimit;
                        processesFilter.Append("and sess.cpu_time > 0 \n");
                        processesFilter.Append("order by sess.cpu_time desc\n");
                    }
                    else if (configuration.TopIo)
                    {
                        top = configuration.TopLimit;
                        processesFilter.Append("and (sess.reads + sess.writes + sess.logical_reads) > 0 \n");
                        processesFilter.Append("order by  (sess.reads + sess.writes + sess.logical_reads) desc\n");
                    }
                    else if (configuration.TopMemory)
                    {
                        top = configuration.TopLimit;
                        processesFilter.Append("and sess.memory_usage > 0 \n");
                        processesFilter.Append("order by sess.memory_usage desc\n");
                    }
                    else if (configuration.TopWait)
                    {
                        top = configuration.TopLimit;
                        processesFilter.Append("and wait_time > 0 \n");
                        processesFilter.Append("order by wait_time desc\n");
                    }
                }

                if (ver.Major > 8)
                {
                   
                    cmd.CommandText =
                        String.Format(BatchFinder.Sessions(ver,cloudProviderId),
                                      processesFilter,
                                      "",
                                      top > 0
                                          ? " top " + top
                                          : "",
                                          ver.MasterDatabaseCompatibility < 90 ? "tempdb" : dbname,
                                          configuration.InputBufferLimiter > 0 ? string.Format(" top {0} ", configuration.InputBufferLimiter.ToString()) : "");
                   
                }
                else
                {
                    string orderStringSQLdmappname = "'" + Constants.ConnectionStringApplicationNamePrefix + "%'";
                    cmd.CommandText =
                        String.Format(BatchFinder.Sessions(ver,cloudProviderId),
                                      processesFilter,
                                      BatchFinder.SessionCount(ver),
                                      top > 0
                                          ? " top " + top
                                          : "",
                                      configuration.InputBufferLimiter > 0
                                          ? " top " + configuration.InputBufferLimiter
                                          : "",
                                          orderStringSQLdmappname);
                }


                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = SqlHelper.CommandTimeout;
                return cmd;
            }
        }

        internal static SqlCommand BuildTableDetailsCommand(SqlConnection connection, ServerVersion ver,
                                                            TableDetailConfiguration config, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText +=
                String.Format(BatchFinder.TableDetails(ver, cloudProviderId),
                              config.Database != null ? config.Database.Replace("]", "]]") : "",
                              config.TableID,
                              ver.Major >= 9 ? "schema_name" : "user_name");
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildTableSummaryCommand(SqlConnection connection, ServerVersion ver,
                                                            TableSummaryConfiguration config, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            string tableTypeFilter;
            // If showing all tables, do not filter at all
            if (config.ShowSystemTables && config.ShowUserTables)
            {
                tableTypeFilter = "";
            }
            else
            {
                tableTypeFilter =
                    String.Format(BatchConstants.TableTypeFilter, config.ShowSystemTables ? "1" : "0");
            }
            cmd.CommandText +=
                String.Format(BatchFinder.TableSummary(ver, cloudProviderId),
                              config.DatabaseName != null ? config.DatabaseName.Replace("]", "]]") : "",
                              tableTypeFilter);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static string BuildWaitStats(ServerVersion ver,int? cloudProviderId = null)
        {
            List<Pair<string, int?>> excluded = Collection.GetExcludedWaitTypes();
            StringBuilder excludedString = new StringBuilder();
            if (excluded != null && excluded.Count > 0)
            {
                foreach (Pair<string, int?> s in excluded)
                {
                    excludedString.Append(String.Format("'{0}',", s.First));
                }
                excludedString.Remove(excludedString.Length - 1, 1);
            }
            return string.Format(BatchFinder.WaitStatistics(ver,cloudProviderId), excludedString);
        }

        internal static SqlCommand BuildWaitStatsCommand(SqlConnection connection, ServerVersion ver, int? cloudProviderId)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += BuildWaitStats(ver, cloudProviderId);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildWmiTestCommand(SqlConnection connection, ServerVersion ver, TestWmiConfiguration config)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += String.Format(BatchFinder.WmiConfigurationTest(ver),
                                                (int)config.WmiConfig.OleAutomationContext,
                                                config.WmiConfig.OleAutomationDisabled ? 1 : 0,
                                                config.WmiConfig.DirectWmiEnabled ? 1 : 0);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static SqlCommand BuildJobsAndStepsCommand(SqlConnection connection, ServerVersion ver, JobsAndStepsConfiguration config)
        {
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText += String.Format(config.IsSelectedJobMode ? BatchConstants.GetJobsMode : BatchConstants.GetStepsMode, config.JobName);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }

        internal static void LogSpOAContext(string instance, string probeName, int spOAContext)
        {
            if (instance == null || probeName == null)
            {
                return;
            }

            string temp = instance;
            temp += " - ";
            temp += probeName;
            temp += " sp_OACreate context is ";

            switch (spOAContext)
            {
                case 1:
                    temp += " InProc";
                    LOG.Debug(temp);
                    break;

                case 4:
                    temp += " OutOfProc";
                    LOG.Info(temp);
                    break;

                case 5:
                    temp += " Both";
                    LOG.Debug(temp);
                    break;

                default:
                    temp += " Unknown (error)";
                    LOG.Error(temp);
                    break;
            }

        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) - Batch to write last filename, record count
        /// </summary>
        internal static string BildQueryMonitor2012ExWriteQuery(string lastFileName, long recordCount)
        {
            return String.Format(BatchFinder.QueryMonitorWriteEX(), lastFileName, recordCount);
        }


        /// <summary>
        /// SQLdm 10.3 (Tushar) - Batch to write last filename, record count
        /// </summary>
        /// <param name="lastFileName"></param>
        /// <param name="recordCount"></param>
        /// <param name="cloudProviderId"></param>
        /// <returns></returns>
        internal static string BuildActivityMonitor2012ExWriteQuery(string lastFileName, long recordCount)
        {
            // SQLdm 10.3 (Varun Chopra) Linux Support by passing cloudProviderId
            return String.Format(BatchFinder.ActivityMonitorWriteEX(), lastFileName, recordCount);
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

        public static SqlCommand BuildDbOwnerPermissionsCommand(SqlConnection connection, ServerVersion serverVersion, FullTextActionConfiguration serverActionConfig)
        {
            SqlCommand cmd = connection.CreateCommand();
            if (serverVersion.IsGreaterThanSql2008Sp1R2())
            {
                cmd.CommandText += BatchConstants.DbOwnerPermissionsCommand;
            }
            else
            {
                cmd.CommandText += BatchConstants.DbOwnerPermissionsCommand2000;
            }
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = SqlHelper.CommandTimeout;
            return cmd;
        }
    }
}
