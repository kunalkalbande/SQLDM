//------------------------------------------------------------------------------
// <copyright file="ProbeHelpers.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Events.AzureMonitor;
using Idera.SQLdm.Services.Common.Probes.Azure;
using Idera.SQLdm.Services.Common.Probes.Azure.Interfaces;
using Wintellect.PowerCollections;
using Azure = Microsoft.Azure.Management.Monitor.Models;
namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    using System;
    using System.Data.SqlTypes;
    using BBS.TracerX;
    using Idera.SQLdm.Common.Snapshots;
    using System.Data.SqlClient;
    using System.Linq;
    using Idera.SQLdm.CollectionService.Probes.Collectors;

    /// <summary>
    /// Helper methods for parsing SQL batches
    /// </summary>
    internal sealed class ProbeHelpers
    {
        #region constants

        //private const string CounterNameBatchRequests = "batch requests/sec";
        //private const string CounterNameCheckpointPages = "checkpoint pages/sec";
        //private const string CounterNameLazyWrites = "lazy writes/sec";
        //private const string CounterNameLockWaits = "lock waits/sec";
        //private const string CounterNamePageLifeExpectancy = "page life expectancy";
        //private const string CounterNamePageLookups = "page lookups/sec";
        //private const string CounterNamePageReads = "page reads/sec";
        //private const string CounterNamePageWrites = "page writes/sec";
        //private const string CounterNameSqlCompilations = "sql compilations/sec";
        //private const string CounterNameSqlReCompilations = "sql re-compilations/sec";
        //private const string CounterNameTargetServerMemory = "target server memory(kb)";
        //private const string CounterNameTargetServerMemory2005 = "target server memory (kb)";
        //private const string CounterNameTotalServerMemory = "total server memory (kb)"; 
        //private const string CounterNameWorkfileCreated = "workfiles created/sec";
        //private const string CounterNameWorktablesCreated = "worktables created/sec";

        //private const string ReplicationStateDistributorSubscriber = "DistributorSubscriber";
        //private const string ReplicationStateNotInstalled = "NotInstalled";
        //private const string ReplicationStatePublishesSubscribes = "PublishesSubscribes";
        //private const string ReplicationStateRunningLocally = "RunningLocally";

        private const string ServiceStateNotInstalled = "not installed";
        private const string ServiceStatePaused = "paused";
        private const string ServiceStateRunning = "running";
        private const string ServiceStateStopped = "stopped";
        private const string ServiceStateStartPending = "start pending";
        private const string ServiceStateStarting = "starting";
        private const string ServiceStateStopPending = "stop pending";
        private const string ServiceStateStopping = "stopping";
        private const string ServiceStatePausePending = "pause pending";
        private const string ServiceStatePausing = "pausing";
        private const string ServiceStateContinuePending = "continue pending";
        private const string ServiceStateContinuing = "continuing";
        private const string ServiceStateUnableToMonitor = "unable to monitor";
        private const string ServiceStateUnableToConnect = "unable to connect";

        internal const string DatabaseStateSuspect = "suspect";
        internal const string DatabaseStateLoading = "loading";
        internal const string DatabaseStateNotAccessible = "not accessible";
        internal const string DatabaseStateRecovering = "recovering";
        internal const string DatabaseStateOffline = "offline";
        internal const string DatabaseStatusStandby = "standby";
        internal const string DatabaseStatusRestoringMirror = "restoringmirror";



        #endregion

        #region fields
        private static Dictionary<string, Azure.MetricDefinition> metricDefinitionsMap = new Dictionary<string, Azure.MetricDefinition>();

        private static Logger LOG = Logger.GetLogger("ProbeHelpers");
        private static Dictionary<string, int> extendedEventNameToTraceEventIdMap =  //SqlDM 10.2 (Anshul Aggarwal) - Maps xe_event_name to trace_event_id.
            new Dictionary<string, int>()
        {
             {"rpc_completed", 10},
            {"rpc_starting", 11},
            {"sql_batch_completed", 12},
            {"sql_batch_starting", 13},
            {"login", 14},
            {"logout", 15},
            {"attention", 16},
            {"existing_connection", 17},
            {"server_start_stop", 18},
            {"dtc_transaction", 19},
            {"error_reported", 21},
            {"errorlog_written", 22},
            {"lock_released", 23},
            {"lock_acquired", 24},
            {"lock_deadlock", 25},
            {"lock_cancel", 26},
            {"lock_timeout", 27},
            {"degree_of_parallelism", 28},
            {"exception_ring_buffer_recorded", 33},
            {"sp_cache_miss", 34},
            {"sp_cache_insert", 35},
            {"sp_cache_remove", 36},
            {"sql_statement_recompile", 37},
            {"sp_cache_hit", 38},
            {"sql_statement_starting", 40},
            {"sql_statement_completed", 41},
            {"module_start", 42},
            {"module_end", 43},
            {"sp_statement_starting", 44},
            {"sp_statement_completed", 45},
            {"object_created", 46},
            {"object_deleted", 47},
            {"sql_transaction", 50},
            {"scan_started", 51},
            {"scan_stopped", 52},
            {"cursor_open", 53},
            {"transaction_log", 54},
            {"hash_warning", 55},
            {"auto_stats", 58},
            {"lock_deadlock_chain", 59},
            {"lock_escalation", 60},
            {"oledb_error", 61},
            {"execution_warning", 67},
            {"query_pre_execution_showplan", 68},
            {"sort_warning", 69},
            {"cursor_prepare", 70},
            {"prepare_sql", 71},
            {"exec_prepared_sql", 72},
            {"unprepare_sql", 73},
            {"cursor_execute", 74},
            {"cursor_recompile", 75},
            {"cursor_implicit_conversion", 76},
            {"cursor_unprepare", 77},
            {"cursor_close", 78},
            {"missing_column_statistics", 79},
            {"missing_join_predicate", 80},
            {"server_memory_change", 81},
            {"user_event", 82},
            {"database_file_size_change", 92},
            {"query_post_execution_showplan", 98},
            {"oledb_call", 119},
            {"oledb_query_interface", 120},
            {"oledb_data_read", 121},
            {"broker_conversation", 124},
            {"deprecation_announcement", 125},
            {"deprecation_final_support", 126},
            {"exchange_spill", 127},
            {"broker_conversation_group", 136},
            {"blocked_process_report", 137},
            {"ucs_connection_setup", 138},
            {"broker_forwarded_message_sent", 139},
            {"broker_forwarded_message_dropped", 140},
            {"broker_message_classify", 141},
            {"broker_transmission_exception", 142},
            {"broker_queue_disabled", 143},
            {"broker_mirrored_route_state_changed", 144},
            {"xml_deadlock_report", 148},
            {"database_xml_deadlock_report",148},
            {"broker_remote_message_acknowledgement", 149},
            {"full_text_crawl_started", 155},
            {"full_text_crawl_stopped", 156},
            {"fulltextlog_written", 158},
            {"broker_message_undeliverable", 160},
            {"broker_corrupted_message", 161},
            {"broker_activation", 163},
            {"object_altered", 164},
            {"uncached_sql_batch_statistics", 165},
            {"query_cache_removal_statistics", 165},
            {"database_mirroring_state_change", 167},
            {"query_post_compilation_showplan", 168},
            {"begin_tran_starting", 181},
            {"begin_tran_completed", 182},
            {"promote_tran_starting", 183},
            {"promote_tran_completed", 184},
            {"commit_tran_starting", 185},
            {"commit_tran_completed", 186},
            {"rollback_tran_starting", 187},
            {"rollback_tran_completed", 188},
            {"lock_timeout_greater_than_0", 189},
            {"progress_report_online_index_operation", 190},
            {"save_tran_starting", 191},
            {"save_tran_completed", 192},
            {"background_job_error", 193},
            {"oledb_provider_information", 194},
            {"assembly_load", 196},
            {"xquery_static_type", 198},
            {"qn_subscription", 199},
            {"qn_parameter_table", 200},
            {"qn_template", 201},
            {"qn_dynamics", 202},
            {"bitmap_disabled_warning", 212},
            {"database_suspect_data_page", 213},
            {"cpu_threshold_exceeded", 214},
            {"preconnect_starting", 215},
            {"preconnect_completed", 216},
            {"plan_guide_successful", 217},
            {"plan_guide_unsuccessful", 218},
        };

        #endregion

        #region constructors

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        internal static void LogAndAttachToSnapshot(Snapshot snapshot, BBS.TracerX.Logger log, string message, bool warningOnly)
        {
            if (snapshot != null)
            {
                string formattedMessage = "[" + snapshot.ServerName + "] " + message;

                if (warningOnly)
                {
                    log.WarnFormat(formattedMessage.Replace("{", "{{").Replace("}", "}}"));
                }
                else
                {
                    //Flag the collection as failed
                    snapshot.SetError(formattedMessage, null);
                    log.ErrorFormat(formattedMessage.Replace("{", "{{").Replace("}", "}}"));
                }
            }
            else
            {
                log.ErrorFormat("[Unknown Server] " + message);
            }
        }

        internal static void LogAndAttachToSnapshot(Snapshot snapshot, BBS.TracerX.Logger log, string message, Exception e, bool warningOnly)
        {
            if (snapshot != null)
            {
                string formattedMessage = "[" + snapshot.ServerName + "] " + message;

                if (warningOnly)
                {
                    log.WarnFormat(formattedMessage + ": {0}", e);
                }
                else
                {
                    //Fail the collection
                    snapshot.SetError(formattedMessage, e);
                    log.ErrorFormat(formattedMessage + ": {0}", e);
                }
            }
            else
            {
                log.ErrorFormat("[Unknown Server] " + message + ": {0}", e);
            }
        }

        public static TimeSpan TimeSpanFromHHMMSS(int hhmmss)
        {
            try
            {
                string hhmmssString = hhmmss.ToString();
                hhmmssString = hhmmssString.PadLeft(6, '0');

                string ss = hhmmssString.Substring(hhmmssString.Length - 2, 2);
                string mm = hhmmssString.Substring(hhmmssString.Length - 4, 2);
                string hh = hhmmssString.Substring(0, hhmmssString.Length - 4);

                return new TimeSpan(Convert.ToInt32(hh), Convert.ToInt32(mm), Convert.ToInt32(ss));
            }
            catch
            {
                return new TimeSpan(0);
            }
        }

        /// <summary>
        /// Combines two SQL fields to a single timestamp
        /// </summary>
        public static DateTime? CombineTwoFieldTimeStamp(int yearMonthDay, int hourMinuteSecond)
        {
            try
            {
                if (yearMonthDay > 0)
                {
                    return Convert.ToDateTime(String.Format("{0:####/##/##} {1:00:##:##}", yearMonthDay, hourMinuteSecond), new CultureInfo("en-US"));
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }

        }

        internal static string GetSysperfinfoObjectPrefix(string serverName)
        {
            Regex instanceLocation = new Regex(@"(?<=\\)\S*");
            string instanceName = serverName != null ? instanceLocation.Match(serverName).ToString() : null; // Handling cases of Server Name 

            if (instanceName == null || instanceName.Length == 0)
            {
                return "SQLServer";
            }
            else
            {
                return "MSSQL$" + instanceName;
            }
        }

        // Deprecated in Project Needmore (5.6) - VH
        ///// <summary>
        ///// Parses a database status from a string
        ///// </summary>
        //internal static DatabaseStatus GetDatabaseStatus(string status)
        //{
        //    status = status.Trim('.');
        //    status = status.ToLower();

        //    switch (status)
        //    {
        //        case DatabaseStateLoading:
        //            return DatabaseStatus.Loading;
        //        case DatabaseStateNotAccessible:
        //            return DatabaseStatus.Inaccessible;
        //        case DatabaseStateOffline:
        //            return DatabaseStatus.Offline;
        //        case DatabaseStateRecovering:
        //            return DatabaseStatus.Recovering;
        //        case DatabaseStateSuspect:
        //            return DatabaseStatus.Suspect;
        //        case DatabaseStatusStandby:
        //            return DatabaseStatus.Standby;
        //        case DatabaseStatusRestoringMirror:
        //            return DatabaseStatus.RestoringMirror;
        //        default:
        //            return DatabaseStatus.Undetermined;
        //    }
        //}

        /// <summary>
        /// Parses a service status from a string
        /// </summary>
        internal static string GetServiceState(ServiceState status)
        {
            switch (status)
            {
                case ServiceState.NotInstalled:
                    return ServiceStateNotInstalled;

                case ServiceState.Paused:
                    return ServiceStatePaused;

                case ServiceState.Running:
                    return ServiceStateRunning;

                case ServiceState.Stopped:
                    return ServiceStateStopped;

                case ServiceState.StartPending:
                    return ServiceStateStartPending;

                case ServiceState.StopPending:
                    return ServiceStateStopPending;

                case ServiceState.PausePending:
                    return ServiceStatePausePending;

                case ServiceState.ContinuePending:
                    return ServiceStateContinuePending;

                case ServiceState.UnableToMonitor:
                    return ServiceStateUnableToMonitor;

                default:
                    return ServiceStateUnableToConnect;
            }
        }

        /// <summary>
        /// Parses a service status from a string
        /// </summary>
        internal static ServiceState GetServiceState(string status)
        {
            status = status.Trim('.');
            status = status.ToLower();

            switch (status)
            {
                case ServiceStateNotInstalled:
                    return ServiceState.NotInstalled;
                case ServiceStatePaused:
                    return ServiceState.Paused;
                case ServiceStateRunning:
                    return ServiceState.Running;
                case ServiceStateStopped:
                    return ServiceState.Stopped;
                case ServiceStateStartPending:
                case ServiceStateStarting:
                    return ServiceState.StartPending;
                case ServiceStateStopPending:
                case ServiceStateStopping:
                    return ServiceState.StopPending;
                case ServiceStatePausePending:
                case ServiceStatePausing:
                    return ServiceState.PausePending;
                case ServiceStateContinuePending:
                case ServiceStateContinuing:
                    return ServiceState.ContinuePending;
                case ServiceStateUnableToMonitor:
                    return ServiceState.UnableToMonitor;
                default:
                    return ServiceState.UnableToConnect;
            }
        }

        // SQLdm 10.3 (Varun Chopra) Linux Support for Service Status
        internal static ServiceState ReadServiceState(SqlDataReader dataReader, BBS.TracerX.Logger LOG, int? cloudProviderId, bool inputInteger = false)
        {
            try
            {
                if (dataReader.Read())
                {
                    return inputInteger
                        ? (ServiceState)dataReader.GetInt32(0)
                        : GetServiceState(dataReader.GetString(0));
                }
                else
                    return ServiceState.UnableToMonitor;
            }
            catch (InvalidCastException invalidCastException)
            {
                try
                {
                    // Failed Case
                    return inputInteger
                        ? GetServiceState(dataReader.GetString(0))
                        : (ServiceState)dataReader.GetInt32(0);
                }
                catch (Exception exception)
                {
                    LOG.Error("Error reading service state", exception);
                    return ServiceState.UnableToMonitor;
                }
            }
            catch (Exception exception)
            {
                //TODO: Add handling for service not found errors
                LOG.Error("Error reading service state", exception);
                return ServiceState.UnableToMonitor;
            }
            finally
            {
                try
                {
                    dataReader.NextResult();
                }
                catch (Exception exception)
                {
                    // Handling for Exceptions on next results
                    LOG.Error("Error while getting next result reading service state", exception);
                }
            }
        }


        internal static string WindowsVersion(string versionString)
        {
            const string WindowsNTString = "Windows NT";
            const string Windows2000String = "Windows 2000";
            const string WindowsXPString = "Windows XP";
            const string Windows2003String = "Windows 2003";
            const string Windows2008String = "Windows 2008";

            if (versionString.Substring(0, 1) == "4")
                return WindowsNTString;
            switch (versionString.Substring(0, 3))
            {
                case "5.0":
                    return Windows2000String;
                case "5.1":
                    return WindowsXPString;
                case "5.2":
                    return Windows2003String;
                case "6.0":
                    return Windows2008String;
                default:
                    return "Windows " + versionString;
            }
        }


        internal static OSMetrics ReadOSMetrics(SqlDataReader dataReader, OSMetrics previousRefresh, Snapshot logTarget, BBS.TracerX.Logger LOG, int? cloudProviderId)
        {
            OSMetrics refresh = new OSMetrics();
            try
            {
                if (!dataReader.HasRows || !dataReader.Read())
                {
                    LOG.Info("No data was returned by the OS Metrics collector");
                    return refresh;
                }

                else
                {
                    if (!dataReader.IsDBNull(0))
                    {
                        refresh.OsStatisticAvailability = dataReader.GetString(0).ToLower();

                        if (refresh.OsStatisticAvailability == "available")
                        {
                            dataReader.NextResult();
                            if (dataReader.Read())
                            {
                                bool logTargetIsNull = (logTarget == null || logTarget.TimeStamp == null);

                                DateTime? inputUTCTimestamp = logTargetIsNull ? new DateTime() : logTarget.TimeStamp;

                                try
                                {
                                    if (!dataReader.IsDBNull(13))
                                    {
                                        inputUTCTimestamp = dataReader.GetDateTime(13);
                                    }
                                }
                                catch (Exception exception)
                                {
                                    LOG.ErrorFormat("Error trying to retrieve DateTime from the DataReader. Detailed exception message: {0}", exception);
                                }

                                refresh = OSMetrics.CookCounters(
                                    dataReader.IsDBNull(0) ? new double?() : double.Parse(dataReader.GetString(0)),
                                    dataReader.IsDBNull(1) ? new double?() : double.Parse(dataReader.GetString(1)),
                                    dataReader.IsDBNull(2) ? new double?() : Double.Parse(dataReader.GetString(2)),
                                    dataReader.IsDBNull(3) ? new double?() : double.Parse(dataReader.GetString(3)),
                                    dataReader.IsDBNull(4) ? new double?() : double.Parse(dataReader.GetString(4)),
                                    dataReader.IsDBNull(5) ? new double?() : double.Parse(dataReader.GetString(5)),
                                    dataReader.IsDBNull(6) ? new double?() : double.Parse(dataReader.GetString(6)),
                                    dataReader.IsDBNull(7) ? new double?() : double.Parse(dataReader.GetString(7)),
                                    dataReader.IsDBNull(8) ? new double?() : double.Parse(dataReader.GetString(8)),
                                    dataReader.IsDBNull(9) ? new double?() : double.Parse(dataReader.GetString(9)),
                                    dataReader.IsDBNull(10) ? new double?() : double.Parse(dataReader.GetString(10)),
                                    dataReader.IsDBNull(11) ? new double?() : double.Parse(dataReader.GetString(11)),
                                    dataReader.IsDBNull(12) ? new double?() : double.Parse(dataReader.GetString(12)),
                                    dataReader.IsDBNull(14) ? new double?() : double.Parse(dataReader.GetString(14)),
                                    inputUTCTimestamp,
                                    (previousRefresh != null) ? previousRefresh : null);

                                if (refresh.DiffFromExpectedTimeStampError > 0)
                                {
                                    LOG.Warn(
                                        "OS Metrics detected an invalid value for " + refresh.DiffFromExpectedTimeStampName + " and disposed of related metrics.  TimeStamp_Sys100NS_Delta: " +
                                        refresh.TimeStamp_Sys100NS_Delta +
                                        " Difference from Expected: " + refresh.DiffFromExpectedTimeStampError);
                                }

                                if (refresh.TotalPhysicalMemory == null || !refresh.TotalPhysicalMemory.Bytes.HasValue || refresh.TotalPhysicalMemory.Bytes <= 0)
                                {
                                    dataReader.NextResult();
                                    if (dataReader.Read())
                                    {
                                        if (!dataReader.IsDBNull(2))
                                        {
                                            //refresh.TotalPhysicalMemory = new FileSize();
                                            refresh.TotalPhysicalMemory.Megabytes = dataReader.GetInt32(2);
                                        }
                                    }
                                }
                            }

                            return refresh;
                        }
                        else
                        {
                            LogAndAttachToSnapshot(logTarget, LOG,
                                                   String.Format("OS Metrics are unavailable.  Reason: {0}",
                                                                 refresh.OsStatisticAvailability), true);
                            return refresh;
                        }
                    }
                    else
                    {
                        LogAndAttachToSnapshot(logTarget, LOG, "Read OS Metrics failed: Dataset was null",
                            cloudProviderId == Constants.MicrosoftAzureManagedInstanceId);
                        return refresh;
                    }
                }
            }
            catch (Exception exception)
            {
                LogAndAttachToSnapshot(logTarget, LOG, "Read OS Metrics failed: {0}", exception, false);
                return refresh;
            }

        }

        internal static OSMetrics ReadAWSOSMetrics(SqlDataReader dataReader, OSMetrics previousRefresh, Snapshot logTarget, BBS.TracerX.Logger LOG, int? cloudProviderId, string serverName)
        {
            OSMetrics refresh = new OSMetrics();
            try
            {
                if (!dataReader.HasRows || !dataReader.Read())
                {
                    LOG.Info("No data was returned by the OS Metrics collector");
                    return refresh;
                }

                else
                {
                    bool checkAwsCred = AWSMetricCollector.IsValidAWSCredentials(serverName);
                    if (!dataReader.IsDBNull(0))
                    {
                        refresh.OsStatisticAvailability = dataReader.GetString(0).ToLower();

                        bool logTargetIsNull = (logTarget == null || logTarget.TimeStamp == null);

                        DateTime? inputUTCTimestamp = logTargetIsNull ? new DateTime() : logTarget.TimeStamp;
                        refresh = OSMetrics.CookCounters(
                            double.Parse(Convert.ToString(dataReader.GetInt64(1))),
                           checkAwsCred == true ? AWSMetricCollector.GetAWSCloudWatchMetrics(serverName, Enum.GetName(typeof(Common.Snapshots.Cloud.CloudMetricList.AWSMetric), Common.Snapshots.Cloud.CloudMetricList.AWSMetric.FreeableMemory)) : 0,
                            null,
                            checkAwsCred == true ? AWSMetricCollector.GetAWSCloudWatchMetrics(serverName, Enum.GetName(typeof(Common.Snapshots.Cloud.CloudMetricList.AWSMetric), Common.Snapshots.Cloud.CloudMetricList.AWSMetric.CPUUtilization)) : 0,
                            null,
                            null,
                            null,
                            null,
                             checkAwsCred == true ? AWSMetricCollector.GetAWSCloudWatchMetrics(serverName, Enum.GetName(typeof(Common.Snapshots.Cloud.CloudMetricList.AWSMetric), Common.Snapshots.Cloud.CloudMetricList.AWSMetric.DiskQueueDepth)) : 0,
                            null,
                            null,
                            null,
                            null,
                            null,
                            inputUTCTimestamp,
                            (previousRefresh != null) ? previousRefresh : null);

                        refresh.TotalPhysicalMemory.Bytes = (refresh.TotalPhysicalMemory.Bytes * 1024);
                        return refresh;
                    }
                    else
                    {
                        LogAndAttachToSnapshot(logTarget, LOG, "Read OS Metrics failed: Dataset was null",
                            cloudProviderId == Constants.MicrosoftAzureManagedInstanceId);
                        return refresh;
                    }
                }
            }
            catch (Exception exception)
            {
                LogAndAttachToSnapshot(logTarget, LOG, "Read OS Metrics failed: {0}", exception, false);
                return refresh;
            }

        }

        internal static DateTime? ReadServerDate(SqlDataReader dataReader, Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            try
            {
                if (dataReader.Read())
                {
                    if (dataReader.IsDBNull(0))
                        return null;
                    else
                        return dataReader.GetDateTime(0);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception exception)
            {
                ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read server datetime failed: {0}", exception, false);
                return null;
            }
            finally
            {
                dataReader.NextResult();
            }
        }



        internal static void ReadLockStatistics(SqlDataReader dataReader, Snapshot logTarget, BBS.TracerX.Logger LOG, LockStatistics lockCounters, SqlBaseProbe.FailureDelegate failureDelegate)
        {
            try
            {
                LockCounter tempTotalCounter = new LockCounter();
                while (dataReader.Read())
                {
                    // Skip if there is no counter name
                    if (!dataReader.IsDBNull(0))
                    {
                        // Set counter name
                        string counterName = dataReader.GetString(0).ToLower().TrimEnd(new char[] { ' ' });

                        // Set instance name - default to "latch" if none
                        string instanceName = dataReader.IsDBNull(1)
                                           ? "latch"
                                           : dataReader.GetString(1).ToLower().TrimEnd(new char[] { ' ' });

                        //Read counter value
                        Int64 counterValue = dataReader.IsDBNull(2) ? 0 : Convert.ToInt64(dataReader.GetValue(2));

                        // Add the counter value to the proper counter object
                        switch (instanceName)
                        {
                            case "allocunit":
                                SetCounter(lockCounters.AllocUnitCounters, tempTotalCounter, counterName, counterValue);
                                break;
                            case "application":
                                SetCounter(lockCounters.ApplicationCounters, tempTotalCounter, counterName, counterValue);
                                break;
                            case "database":
                                SetCounter(lockCounters.DatabaseCounters, tempTotalCounter, counterName, counterValue);
                                break;
                            case "extent":
                                SetCounter(lockCounters.ExtentCounters, tempTotalCounter, counterName, counterValue);
                                break;
                            case "file":
                                SetCounter(lockCounters.FileCounters, tempTotalCounter, counterName, counterValue);
                                break;
                            case "hobt":
                                SetCounter(lockCounters.HeapCounters, tempTotalCounter, counterName, counterValue);
                                break;
                            case "key":
                                SetCounter(lockCounters.KeyCounters, tempTotalCounter, counterName, counterValue);
                                break;
                            case "metadata":
                                SetCounter(lockCounters.MetadataCounters, tempTotalCounter, counterName, counterValue);
                                break;
                            case "object":
                                SetCounter(lockCounters.ObjectCounters, tempTotalCounter, counterName, counterValue);
                                break;
                            case "page":
                                SetCounter(lockCounters.PageCounters, tempTotalCounter, counterName, counterValue);
                                break;
                            case "rid":
                                SetCounter(lockCounters.RidCounters, tempTotalCounter, counterName, counterValue);
                                break;
                            case "table":
                                SetCounter(lockCounters.TableCounters, tempTotalCounter, counterName, counterValue);
                                break;

                            // Catch both "_total" and "latch" instances
                            default:
                                // Filter for actual latch counters
                                if (counterName == "latch waits/sec" || counterName == "total latch wait time (ms)")
                                {
                                    SetCounter(lockCounters.LatchCounters, tempTotalCounter, counterName, counterValue);
                                }
                                break;
                        }
                    }
                }
                //Set the actual total counter here to preserve proper calculations
                lockCounters.TotalCounters.DeadlocksTotal = tempTotalCounter.DeadlocksTotal;
                lockCounters.TotalCounters.RequestsTotal = tempTotalCounter.RequestsTotal;
                lockCounters.TotalCounters.TimeoutsTotal = tempTotalCounter.TimeoutsTotal;
                lockCounters.TotalCounters.WaitTimeTotal = tempTotalCounter.WaitTimeTotal;
                lockCounters.TotalCounters.WaitsTotal = tempTotalCounter.WaitsTotal;
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Error interpreting Lock Counter Statistics: {0}", e,
                                                   false);
                if (failureDelegate != null)
                    failureDelegate(logTarget, e);
            }
            finally
            {
                dataReader.NextResult();
            }
        }


        private static void SetCounter(LockCounter counter, LockCounter total, string counterType, Int64 value)
        {
            if (counter == null)
            {
                counter = new LockCounter();
            }

            // Assign value to proper counter
            switch (counterType)
            {
                case "latch waits/sec":
                    if (counter.WaitsTotal == null)
                        counter.WaitsTotal = value;
                    else
                        counter.WaitsTotal += value;
                    break;
                case "lock requests/sec":
                    if (counter.RequestsTotal == null)
                        counter.RequestsTotal = value;
                    else
                        counter.RequestsTotal += value;
                    if (!total.RequestsTotal.HasValue)
                    {
                        total.RequestsTotal = 0;
                    }
                    total.RequestsTotal += value;
                    break;
                case "lock timeouts/sec":
                    if (counter.TimeoutsTotal == null)
                        counter.TimeoutsTotal = value;
                    else
                        counter.TimeoutsTotal += value;
                    if (!total.TimeoutsTotal.HasValue)
                    {
                        total.TimeoutsTotal = 0;
                    }
                    total.TimeoutsTotal += value;
                    break;
                case "lock wait time (ms)":
                    if (counter.WaitTimeTotal == null)
                        counter.WaitTimeTotal = TimeSpan.FromMilliseconds(value);
                    else
                        counter.WaitTimeTotal += TimeSpan.FromMilliseconds(value);
                    if (!total.WaitTimeTotal.HasValue)
                    {
                        total.WaitTimeTotal = TimeSpan.FromMilliseconds(0);
                    }
                    total.WaitTimeTotal += total.WaitTimeTotal.Value.Add(TimeSpan.FromMilliseconds(value));
                    break;
                case "lock waits/sec":
                    if (counter.WaitsTotal == null)
                        counter.WaitsTotal = value;
                    else
                        counter.WaitsTotal += value;
                    if (!total.WaitsTotal.HasValue)
                    {
                        total.WaitsTotal = 0;
                    }
                    total.WaitsTotal += value;
                    break;
                case "number of deadlocks/sec":
                    if (counter.DeadlocksTotal == null)
                        counter.DeadlocksTotal = value;
                    else
                        counter.DeadlocksTotal += value;
                    if (!total.DeadlocksTotal.HasValue)
                    {
                        total.DeadlocksTotal = 0;
                    }
                    total.DeadlocksTotal += value;
                    break;
                case "total latch wait time (ms)":
                    if (counter.WaitTimeTotal == null)
                        counter.WaitTimeTotal = TimeSpan.FromMilliseconds(value);
                    else
                        counter.WaitTimeTotal += TimeSpan.FromMilliseconds(value);
                    break;
            }
        }


        internal static Int64? CalculateCounterDelta(Int64? previousCounter, Int64? currentCounter)
        {
            Int64? counterDelta = currentCounter - previousCounter;
            if (counterDelta < 0)
                return null;
            else
                return counterDelta;
        }

        internal static decimal? CalculateCounterDelta(decimal? previousCounter, decimal? currentCounter)
        {
            decimal? counterDelta = currentCounter - previousCounter;
            if (counterDelta < 0)
                return null;
            else
                return counterDelta;
        }

        internal static FileSize CalculateCounterDelta(FileSize previousCounter, FileSize currentCounter)
        {
            if (currentCounter == null || previousCounter == null)
                return null;
            FileSize counterDelta = new FileSize();
            counterDelta.Bytes = currentCounter.Bytes - previousCounter.Bytes;
            if (counterDelta.Bytes < 0)
                counterDelta.Bytes = 0;
            //else
            return counterDelta;
        }

        internal static string GetServiceString(ServiceName service, ServerVersion ver, string serverName)
        {
            if (service == ServiceName.DTC)
                return "MSDTC";

            //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --return service name for new SQL Server services
            if (service == ServiceName.Browser && ver.Major > 8)
                return "SQLBrowser";

            if (service == ServiceName.ActiveDirectoryHelper)
            {
                if (ver.Major == 8 || ver.Major == 9)
                    return "MSSQLServerADHelper";
                else if (ver.Major == 10)
                    return "MSSQLServerADHelper100";
            }
            //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --return service name for new SQL Server services

            if (service == ServiceName.FullTextSearch && ver.Major == 8)
                return "MSSearch";

            Regex instanceLocation = new Regex(@"(?<=\\)\S*");
            string instanceName = instanceLocation.Match(serverName).ToString();

            if (instanceName == null || instanceName.Length == 0)
            {
                switch (service)
                {
                    case ServiceName.Agent:
                        return "SQLSERVERAGENT";
                    case ServiceName.FullTextSearch:
                        return "MSFTESQL";
                    case ServiceName.SqlServer:
                        return "MSSQLSERVER";
                    default:
                        return null;
                }
            }
            else
            {
                switch (service)
                {
                    case ServiceName.Agent:
                        return "SQLAGENT$" + instanceName;
                    case ServiceName.FullTextSearch:
                        return "MSFTESQL$" + instanceName;
                    case ServiceName.SqlServer:
                        return "MSSQL$" + instanceName;
                    default:
                        return null;
                }
            }
        }

        internal static void ReadAvgCpuPercent(SqlDataReader dataReader, Snapshot logTarget, BBS.TracerX.Logger LOG,
            ServerOverview refresh, SqlBaseProbe.FailureDelegate failureDelegate)
        {
            try
            {
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(0))
                            refresh.OSMetricsStatistics.PercentProcessorTime = Decimal.ToDouble(dataReader.GetDecimal(0));
                    }
                }
            }
            catch (Exception e)
            {
                LogAndAttachToSnapshot(logTarget, LOG, "Error interpreting Memory Counters: {0}", e,
                                                    false);
                failureDelegate(logTarget, e);
            }
        }
        internal static void ReadMemoryCounters(SqlDataReader dataReader, Snapshot logTarget, BBS.TracerX.Logger LOG, Memory MemoryStatistics, ServerVersion ProductVersion,
      SqlBaseProbe.FailureDelegate failureDelegate)
        {
            try
            {
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(0))
                        {
                            switch (dataReader.GetString(0).Trim().ToLower())
                            {
                                case "cache pages"://'Procedure cache pages' counter in 2000 and from sys.dm_exec_cached_plans >= 2008
                                    if (dataReader.FieldCount >= 2 && !dataReader.IsDBNull(1))
                                    {
                                        if (ProductVersion.Major >= 9)
                                        {
                                            if (MemoryStatistics.CachePages.Kilobytes == null)
                                                MemoryStatistics.CachePages.Kilobytes = dataReader.GetDecimal(1);
                                            else
                                                MemoryStatistics.CachePages.Kilobytes += dataReader.GetDecimal(1);
                                        }
                                        else
                                        {
                                            if (MemoryStatistics.CachePages.Pages == null)
                                                MemoryStatistics.CachePages.Pages = dataReader.GetInt32(1);
                                            else
                                                MemoryStatistics.CachePages.Pages += dataReader.GetInt32(1);

                                        }
                                    }
                                    break;
                                case "committed pages": // 'Database pages' counter <+= 2008. 'Database Cache Memory (KB)' in 2012
                                    if (dataReader.FieldCount >= 2 && !dataReader.IsDBNull(1))
                                    {
                                        if (MemoryStatistics.CommittedPages.Pages == null)
                                            MemoryStatistics.CommittedPages.Pages =
                                            (ProductVersion.Major >= 9 // SQL Server 2005 or greater
                                                 ? dataReader.GetDecimal(1)
                                                 : dataReader.GetInt32(1));
                                        else
                                            MemoryStatistics.CommittedPages.Pages +=
                                            (ProductVersion.Major >= 9 // SQL Server 2005 or greater
                                                 ? dataReader.GetDecimal(1)
                                                 : dataReader.GetInt32(1));
                                    }
                                    break;
                                case "connection memory (kb)":
                                    if (dataReader.FieldCount >= 2 && !dataReader.IsDBNull(1))
                                    {
                                        if (MemoryStatistics.ConnectionMemory.Kilobytes == null)
                                            MemoryStatistics.ConnectionMemory.Kilobytes =
                                            (ProductVersion.Major >= 9 // SQL Server 2005 or greater
                                                 ? dataReader.GetDecimal(1)
                                                 : dataReader.GetInt32(1));
                                        else
                                            MemoryStatistics.ConnectionMemory.Kilobytes +=
                                            (ProductVersion.Major >= 9 // SQL Server 2005 or greater
                                                 ? dataReader.GetDecimal(1)
                                                 : dataReader.GetInt32(1));
                                    }
                                    break;
                                case "free pages": //buffer manager:free pages <+= 2008
                                    if (dataReader.FieldCount >= 2 && !dataReader.IsDBNull(1))
                                    {
                                        if (MemoryStatistics.FreePages.Pages == null)
                                            MemoryStatistics.FreePages.Pages =
                                            (ProductVersion.Major >= 9 // SQL Server 2005 or greater
                                                 ? dataReader.GetDecimal(1)
                                                 : dataReader.GetInt32(1));
                                        else
                                            MemoryStatistics.FreePages.Pages +=
                                            (ProductVersion.Major >= 9 // SQL Server 2005 or greater
                                                 ? dataReader.GetDecimal(1)
                                                 : dataReader.GetInt32(1));
                                    }
                                    break;
                                case "free memory (kb)"://memory manager in 2012
                                    if (dataReader.FieldCount >= 2 && !dataReader.IsDBNull(1))
                                    {
                                        if (MemoryStatistics.FreePages.Kilobytes == null)
                                            MemoryStatistics.FreePages.Kilobytes =
                                            (ProductVersion.Major >= 9 // SQL Server 2005 or greater
                                                 ? dataReader.GetDecimal(1)
                                                 : dataReader.GetInt32(1));
                                        else
                                            MemoryStatistics.FreePages.Kilobytes +=
                                            (ProductVersion.Major >= 9 // SQL Server 2005 or greater
                                                 ? dataReader.GetDecimal(1)
                                                 : dataReader.GetInt32(1));
                                    }
                                    break;
                                case "granted workspace memory (kb)":
                                    if (dataReader.FieldCount >= 2 && !dataReader.IsDBNull(1))
                                    {
                                        if (MemoryStatistics.GrantedWorkspaceMemory.Kilobytes == null)
                                            MemoryStatistics.GrantedWorkspaceMemory.Kilobytes =
                                            (ProductVersion.Major >= 9 // SQL Server 2005 or greater
                                                 ? dataReader.GetDecimal(1)
                                                 : dataReader.GetInt32(1));
                                        else
                                            MemoryStatistics.GrantedWorkspaceMemory.Kilobytes +=
                                            (ProductVersion.Major >= 9 // SQL Server 2005 or greater
                                                 ? dataReader.GetDecimal(1)
                                                 : dataReader.GetInt32(1));
                                    }
                                    break;
                                case "lock memory (kb)":
                                    if (dataReader.FieldCount >= 2 && !dataReader.IsDBNull(1))
                                    {
                                        if (MemoryStatistics.LockMemory.Kilobytes == null)
                                            MemoryStatistics.LockMemory.Kilobytes =
                                            (ProductVersion.Major >= 9 // SQL Server 2005 or greater
                                                 ? dataReader.GetDecimal(1)
                                                 : dataReader.GetInt32(1));
                                        else
                                            MemoryStatistics.LockMemory.Kilobytes +=
                                            (ProductVersion.Major >= 9 // SQL Server 2005 or greater
                                                 ? dataReader.GetDecimal(1)
                                                 : dataReader.GetInt32(1));
                                    }
                                    break;
                                case "optimizer memory (kb)":
                                    if (dataReader.FieldCount >= 2 && !dataReader.IsDBNull(1))
                                    {
                                        if (MemoryStatistics.OptimizerMemory.Kilobytes == null)
                                            MemoryStatistics.OptimizerMemory.Kilobytes =
                                            (ProductVersion.Major >= 9 // SQL Server 2005 or greater
                                                 ? dataReader.GetDecimal(1)
                                                 : dataReader.GetInt32(1));
                                        else
                                            MemoryStatistics.OptimizerMemory.Kilobytes +=
                                            (ProductVersion.Major >= 9 // SQL Server 2005 or greater
                                                 ? dataReader.GetDecimal(1)
                                                 : dataReader.GetInt32(1));
                                    }
                                    break;
                                case "total pages": // this counter brought back total server memoory in pages up to incl 2008
                                    if (dataReader.FieldCount >= 2 && !dataReader.IsDBNull(1))
                                    {
                                        if (MemoryStatistics.BufferCachePages.Pages == null)
                                            MemoryStatistics.BufferCachePages.Pages =
                                            (ProductVersion.Major >= 9 // SQL Server 2005 or greater
                                                 ? dataReader.GetDecimal(1)
                                                 : dataReader.GetInt32(1));
                                        else
                                            MemoryStatistics.BufferCachePages.Pages +=
                                            (ProductVersion.Major >= 9 // SQL Server 2005 or greater
                                                 ? dataReader.GetDecimal(1)
                                                 : dataReader.GetInt32(1));
                                    }
                                    break;
                                // SQL Server 2005's Total Memory counter does not include Proc Cache
                                case "total server memory (kb)": //This has always been in memory manager. This is total server mem in kb
                                    if (dataReader.FieldCount >= 2 && !dataReader.IsDBNull(1))
                                    {
                                        if (ProductVersion.Major < 9)
                                        {
                                            if (MemoryStatistics.TotalServerMemory.Kilobytes == null)
                                                MemoryStatistics.TotalServerMemory.Kilobytes = dataReader.GetInt32(1);
                                            else
                                                MemoryStatistics.TotalServerMemory.Kilobytes += dataReader.GetInt32(1);
                                        }
                                        else if (ProductVersion.Major < 11)
                                        {
                                            if (MemoryStatistics.TotalServerMemory.Kilobytes == null)
                                                MemoryStatistics.TotalServerMemory.Kilobytes = dataReader.GetDecimal(1) + MemoryStatistics.CachePages.Kilobytes;
                                            else
                                                MemoryStatistics.TotalServerMemory.Kilobytes += dataReader.GetDecimal(1) + MemoryStatistics.CachePages.Kilobytes;
                                        }
                                        else //in 2012 the total pages no longer come from buffer manager. We derive them here.
                                        {
                                            if (MemoryStatistics.TotalServerMemory.Kilobytes == null)
                                                MemoryStatistics.TotalServerMemory.Kilobytes = dataReader.GetDecimal(1) + MemoryStatistics.CachePages.Kilobytes;
                                            else
                                                MemoryStatistics.TotalServerMemory.Kilobytes += dataReader.GetDecimal(1) + MemoryStatistics.CachePages.Kilobytes;
                                        }

                                    }
                                    break;
                                case "target server memory (kb)":
                                    if (dataReader.FieldCount >= 2 && !dataReader.IsDBNull(1))
                                    {
                                        if (MemoryStatistics.TargetServerMemory.Kilobytes == null)
                                            MemoryStatistics.TargetServerMemory.Kilobytes = dataReader.GetInt32(1);
                                        else
                                            MemoryStatistics.TargetServerMemory.Kilobytes += dataReader.GetInt32(1);
                                    }
                                    break;
                            } // end switch GetString()
                        }
                    } // end while read()

                } // end if hasRows
            } // end try
            catch (Exception e)
            {
                LogAndAttachToSnapshot(logTarget, LOG, "Error interpreting Memory Counters: {0}", e,
                                                    false);
                failureDelegate(logTarget, e);
            }
        }

        internal static void ReadAzureManagedInstancePages(SqlDataReader dataReader, Snapshot logTarget, BBS.TracerX.Logger LOG, ServerOverview refresh, SqlBaseProbe.FailureDelegate failureDelegate)
        {
            try
            {
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        if (dataReader.IsDBNull(0))
                        {
                            continue;
                        }
                        switch (dataReader.GetString(0).Trim().ToLower())
                        {
                            case "page writes/sec":
                                if (dataReader.FieldCount >= 2 && !dataReader.IsDBNull(1))
                                {
                                    refresh.PagesWritePersec = dataReader.GetInt64(1);
                                }
                                break;

                            case "page reads/sec":
                                if (dataReader.FieldCount >= 2 && !dataReader.IsDBNull(1))
                                {
                                    refresh.PagesReadPersec = dataReader.GetInt64(1);
                                }
                                break;
                        }
                    }
                }
            } // end try
            catch (Exception e)
            {
                LogAndAttachToSnapshot(logTarget, LOG, "Error interpreting Storage Limit: {0}", e,
                false);
                failureDelegate(logTarget, e);
            }
        }

        //6.2.3
        internal static void ReadAzureStorageSizeLimit(SqlDataReader dataReader, Snapshot logTarget, BBS.TracerX.Logger LOG, ServerOverview refresh, SqlBaseProbe.FailureDelegate failureDelegate)
        {
            try
            {
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        String dbName = dataReader["database_name"].ToString().Trim();
                        long storageLimit = Int64.Parse(dataReader["MaxSizeInBytes"].ToString().Trim());
                        decimal? storageSizeLimit = storageLimit / (1024 * 1024 * 1024);
                        if (refresh.DbStatistics.ContainsKey(dbName) && refresh.DbStatistics[dbName].AzureCloudStorageLimit != storageSizeLimit && storageSizeLimit > 0)
                        {
                            refresh.DbStatistics[dbName].AzureCloudStorageLimit = storageSizeLimit;
                        }

                    }
                }
            } // end try
            catch (Exception e)
            {
                LogAndAttachToSnapshot(logTarget, LOG, "Error interpreting Storage Limit: {0}", e,
                false);
                failureDelegate(logTarget, e);
            }
        }

        internal static void ReadAzureDbDetail(SqlDataReader dataReader, Snapshot logTarget, BBS.TracerX.Logger LOG, ServerOverview refresh,
            SqlBaseProbe.FailureDelegate failureDelegate)
        {
            try
            {
                if (dataReader.HasRows)
                {
                    List<Dictionary<String, String>> obj = new List<Dictionary<String, String>>();
                    while (dataReader.Read())
                    {
                        String databaseName = String.Empty; // Initializing
                        decimal? avgCpuPercent = null, avgDataIoPercent = null, avgLogWritePercent = null;
                        if (!dataReader.IsDBNull(0)) databaseName = dataReader.GetString(0);
                        if (!dataReader.IsDBNull(1)) avgCpuPercent = dataReader.GetDecimal(1);
                        if (!dataReader.IsDBNull(2)) avgDataIoPercent = dataReader.GetDecimal(2);
                        if (!dataReader.IsDBNull(3)) avgLogWritePercent = dataReader.GetDecimal(3);
                        int? dtuLimit = null;
                        if (!dataReader.IsDBNull(4))
                            dtuLimit = dataReader.GetInt32(4);
                        decimal? cpuLimit = null;
                        if (!dataReader.IsDBNull(5))
                            cpuLimit = dataReader.GetDecimal(5);
                        if (!String.Equals(databaseName, "tempdb", StringComparison.OrdinalIgnoreCase))
                            refresh.DbStatistics[databaseName].AzureDbDetail = new AzureDbDetail(avgCpuPercent, avgDataIoPercent, avgLogWritePercent, dtuLimit, cpuLimit);

                    }
                }
            } // end try
            catch (Exception e)
            {
                LogAndAttachToSnapshot(logTarget, LOG, "Error interpreting Azure detais: {0}", e,
                                                    false);
                failureDelegate(logTarget, e);
            }
        }


        //6.2.4
        internal static void ReadAzureIO(SqlDataReader dataReader, Snapshot logTarget, BBS.TracerX.Logger LOG, ServerOverview refresh, SqlBaseProbe.FailureDelegate failureDelegate)
        {
            try
            {
                if (dataReader.HasRows)
                {
                    List<Dictionary<String, String>> obj = new List<Dictionary<String, String>>();

                    while (dataReader.Read())
                    {
                        var properties = new Dictionary<String, String>();
                        var dbName = dataReader["database_name"].ToString();
                        properties.Add("databaseName", dbName);
                        properties.Add("dataIOUsage", dataReader["avg_data_io_percent"].ToString());
                        properties.Add("logIOUsage", dataReader["avg_log_write_percent"].ToString());
                        var timeDelta = CalculateTimeDelta(refresh.TimeDelta);
                        if (timeDelta != 0)
                        {
                            double dataIORate = double.Parse(dataReader["avg_data_io_percent"].ToString()) / timeDelta;
                            properties.Add("dataIORate", dataIORate.ToString());
                            double logIORate = double.Parse(dataReader["avg_log_write_percent"].ToString()) / timeDelta;
                            properties.Add("logIORate", logIORate.ToString());
                        }
                        else
                        {
                            properties.Add("dataIORate", "0");
                            properties.Add("logIORate", "0");
                        }
                        obj.Add(properties);

                        // 6.2.3 -- (Dhruv Rana)
                        decimal? Allocated, Used;
                        Int32 dbid;
                        Used = Decimal.Parse(dataReader["storage_in_megabytes"].ToString().Trim());
                        Allocated = Decimal.Parse(dataReader["allocated_storage_in_megabytes"].ToString().Trim());
                        dbid = Int32.Parse(dataReader["database_id"].ToString().Trim());

                        //Add the database to our dictionary if we haven't already
                        //We do not take stats on mssqlsystemresource)
                        if (dbName != "mssqlsystemresource" &&
                            !refresh.DbStatistics.ContainsKey(dbName))
                        {
                            refresh.DbStatistics.Add(dbName, new DatabaseStatistics(refresh.ServerName, dbName, dbid));
                        }

                        if (refresh.DbStatistics.ContainsKey(dbName) && refresh.DbStatistics[dbName].AzureCloudAllocatedMemory != Allocated && Allocated > 0)
                        {
                            refresh.DbStatistics[dbName].AzureCloudAllocatedMemory = Allocated;
                        }

                        if (refresh.DbStatistics.ContainsKey(dbName) && refresh.DbStatistics[dbName].AzureCloudUsedMemory != Used && Used > 0)
                        {
                            refresh.DbStatistics[dbName].AzureCloudUsedMemory = Used;
                        }
                    }
                    refresh.AzureIOMetrics = obj;

                } // end try
            }
            catch (Exception e)
            {
                LogAndAttachToSnapshot(logTarget, LOG, "Error interpreting Memory Counters: {0}", e,
                                                    false);
                failureDelegate(logTarget, e);
            }
        }

        //6.2.4

        // 5.1
        internal static void ReadAzureManagedInstance(SqlDataReader dataReader, Snapshot logTarget, BBS.TracerX.Logger LOG, ServerOverview refresh, SqlBaseProbe.FailureDelegate failureDelegate)
        {
            try
            {
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        decimal limit;
                        limit = Decimal.Parse(dataReader["reserved_storage_mb"].ToString().Trim());
                        refresh.ManagedInstanceStorageLimit = (limit / 1024); // Storage Limit in GB
                    }

                }
            } // end try
            catch (Exception e)
            {
                LogAndAttachToSnapshot(logTarget, LOG, "Error interpreting Memory Counters: {0}", e,
                                                    false);
                failureDelegate(logTarget, e);
            }
        }


        internal static double CalculateTimeDelta(TimeSpan? timeSpan)
        {
            TimeSpan value = timeSpan.Value;
            return value.Seconds;
        }

        internal static void ReadDatabaseStatus(SqlDataReader dataReader, Dictionary<string, DatabaseStatistics> previousDbStatistics, ServerOverview refresh, Logger LOG)
        {
            string dbName = "";

            while (dataReader.Read())
            {
                if (!dataReader.IsDBNull(0))
                {
                    long dbid = dataReader.GetInt64(0);
                    dbName = dataReader.GetString(1).TrimEnd();

                    //Add the database to our dictionary if we haven't already
                    //We do not take stats on mssqlsystemresource)
                    if (dbName != "mssqlsystemresource" &&
                        !refresh.DbStatistics.ContainsKey(dbName))
                    {
                        refresh.DbStatistics.Add(dbName, new DatabaseStatistics(refresh.ServerName, dbName, dbid));
                    }

                    //if the databaseid's are all out of date go and update them.
                    if (refresh.DbStatistics.ContainsKey(dbName) && refresh.DbStatistics[dbName].DatabaseId != dbid && dbid > 0)
                    {
                        refresh.DbStatistics[dbName].DatabaseId = dbid;
                    }

                    // Read database status if applicable
                    if (dataReader.FieldCount == 3)
                    {
                        int status = dataReader.GetInt32(2);
                        refresh.DbStatistics[dbName].Status = (DatabaseStatus)status;
                    }

                }
            }
        }

        internal static void ReadDatabaseCounters(SqlDataReader dataReader, Dictionary<string, DatabaseStatistics> previousDbStatistics, ServerOverview refresh, Logger LOG)
        {
            dataReader.NextResult();
            string prevDbName = null;

            while (dataReader.Read())
            {
                //Check for valid data
                if (dataReader.FieldCount != 3)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG,
                                                        "Read database statistics failed - fieldcount was incorrect",
                                                        true);
                    return;
                }

                if (!dataReader.IsDBNull(0) && !dataReader.IsDBNull(1) && !dataReader.IsDBNull(2))
                {
                    var dbName = dataReader.GetString(2).TrimEnd();

                    //Add the database to our dictionary if we haven't already
                    //We do not take stats on mssqlsystemresource
                    if (dbName != prevDbName && dbName != "mssqlsystemresource" &&
                        !refresh.DbStatistics.ContainsKey(dbName))
                    {
                        refresh.DbStatistics.Add(dbName, new DatabaseStatistics(refresh.ServerName, dbName));
                    }
                    prevDbName = dbName;

                    //Assign the correct property according to the counter name
                    //We do not take stats on mssqlsystemresource
                    if (dbName != "mssqlsystemresource")
                    {
                        switch (dataReader.GetString(0).Trim().ToLower())
                        {
                            case "transactions/sec":
                                refresh.DbStatistics[dbName].Transactions_Raw = dataReader.GetInt64(1);
                                break;
                            case "log flushes/sec":
                                refresh.DbStatistics[dbName].LogFlushes_Raw = dataReader.GetInt64(1);
                                break;
                            case "log bytes flushed/sec":
                                //refresh.DbStatistics[dbName].LogSizeFlushed_Raw = new FileSize();
                                refresh.DbStatistics[dbName].LogSizeFlushed_Raw.Bytes = dataReader.GetInt64(1);
                                break;
                            case "log flush waits/sec":
                                refresh.DbStatistics[dbName].LogFlushWaits_Raw = dataReader.GetInt64(1);
                                break;
                            case "log cache reads/sec":
                                refresh.DbStatistics[dbName].LogCacheReads_Raw = dataReader.GetInt64(1);
                                break;
                            case "log cache hit ratio":
                                refresh.DbStatistics[dbName].LogCacheHitRatio_Raw = dataReader.GetInt64(1);
                                break;
                            case "log cache hit ratio base":
                                refresh.DbStatistics[dbName].LogCacheHitRatio_Base = dataReader.GetInt64(1);
                                break;
                        }
                    }
                }
            }
        }

        internal static void ReadDatabaseWaitTime(SqlDataReader dataReader, Dictionary<string, DatabaseStatistics> previousDbStatistics, ServerOverview refresh, Logger LOG)
        {
            dataReader.NextResult();

            while (dataReader.Read())
            {
                if (!dataReader.IsDBNull(0))
                {
                    var dbName = dataReader.GetString(0).TrimEnd();

                    //Add the database to our dictionary if we haven't already
                    //We do not take stats on mssqlsystemresource
                    if (dbName != "mssqlsystemresource" &&
                        !refresh.DbStatistics.ContainsKey(dbName))
                    {
                        refresh.DbStatistics.Add(dbName, new DatabaseStatistics(refresh.ServerName, dbName));
                    }

                    //Assign the correct property according to the counter name
                    //We do not take stats on mssqlsystemresource
                    if (dbName != "mssqlsystemresource")
                    {
                        if (!dataReader.IsDBNull(1)) refresh.DbStatistics[dbName].Reads_Raw = dataReader.GetDecimal(1);
                        if (!dataReader.IsDBNull(2)) refresh.DbStatistics[dbName].Writes_Raw = dataReader.GetDecimal(2);
                        if (!dataReader.IsDBNull(3)) refresh.DbStatistics[dbName].BytesRead_Raw = dataReader.GetDecimal(3);
                        if (!dataReader.IsDBNull(4)) refresh.DbStatistics[dbName].BytesWritten_Raw = dataReader.GetDecimal(4);
                        if (!dataReader.IsDBNull(5)) refresh.DbStatistics[dbName].IoStallMs_Raw = dataReader.GetDecimal(5);
                    }
                }
            }
        }

        internal static void ReadDatabaseStatistics(SqlDataReader dataReader, Dictionary<string, DatabaseStatistics> previousDbStatistics, ServerOverview refresh, Logger LOG)
        {
            using (LOG.DebugCall("ReadDatabaseStatistics"))
            {
                try
                {
                    dataReader.NextResult();
                    ReadDatabaseStatus(dataReader, previousDbStatistics, refresh, LOG);
                    ReadDatabaseCounters(dataReader, previousDbStatistics, refresh, LOG);
                    ReadDatabaseWaitTime(dataReader, previousDbStatistics, refresh, LOG);
                    CalculateDatabaseStatistics(previousDbStatistics, refresh);
                }
                catch (Exception exception)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh, LOG, "Read database statistics failed: {0}", exception,
                                                        true);
                    return;
                }
            }
        }


        internal static void CalculateDatabaseStatistics(Dictionary<string, DatabaseStatistics> previousDbStatistics, ServerOverview refresh)
        {
            //Loop through all of the databases we've gathered stats for so far
            foreach (DatabaseStatistics dbstats in refresh.DbStatistics.Values)
            {
                //Calculate the hit ratio even on the first refresh
                if (dbstats.LogCacheHitRatio_Base.HasValue)
                {
                    if (dbstats.LogCacheHitRatio_Base > 0)
                    {
                        dbstats.LogCacheHitRatio = 100 * (double)dbstats.LogCacheHitRatio_Raw /
                                                   dbstats.LogCacheHitRatio_Base;
                    }
                    else
                    {
                        dbstats.LogCacheHitRatio = 0;
                    }
                }

                //Once we have at least 2 refreshes, go ahead and calculate remaining DB stats
                if (previousDbStatistics != null)
                {
                    //Look up the previous refresh of our current database
                    if (previousDbStatistics.ContainsKey(dbstats.Name))
                    {
                        DatabaseStatistics prevStats = previousDbStatistics[dbstats.Name];
                        dbstats.Transactions =
                            ProbeHelpers.CalculateCounterDelta(prevStats.Transactions_Raw, dbstats.Transactions_Raw);
                        dbstats.LogFlushes =
                            ProbeHelpers.CalculateCounterDelta(prevStats.LogFlushes_Raw, dbstats.LogFlushes_Raw);
                        dbstats.LogSizeFlushed.Bytes =
                            ProbeHelpers.CalculateCounterDelta(prevStats.LogSizeFlushed_Raw, dbstats.LogSizeFlushed_Raw).Bytes;
                        dbstats.LogFlushWaits =
                            ProbeHelpers.CalculateCounterDelta(prevStats.LogFlushWaits_Raw, dbstats.LogFlushWaits_Raw);
                        dbstats.LogCacheReads =
                            ProbeHelpers.CalculateCounterDelta(prevStats.LogCacheReads_Raw, dbstats.LogCacheReads_Raw);
                        dbstats.Reads = ProbeHelpers.CalculateCounterDelta(prevStats.Reads_Raw, dbstats.Reads_Raw);
                        dbstats.Writes = ProbeHelpers.CalculateCounterDelta(prevStats.Writes_Raw, dbstats.Writes_Raw);
                        dbstats.BytesRead = ProbeHelpers.CalculateCounterDelta(prevStats.BytesRead_Raw, dbstats.BytesRead_Raw);
                        dbstats.BytesWritten = ProbeHelpers.CalculateCounterDelta(prevStats.BytesWritten_Raw, dbstats.BytesWritten_Raw);
                        dbstats.IoStallMs = ProbeHelpers.CalculateCounterDelta(prevStats.IoStallMs_Raw, dbstats.IoStallMs_Raw);


                    }
                }
            }
        }


        internal static Dictionary<string, DiskDrive> ReadDiskDrives(SqlDataReader dataReader, Dictionary<string, DiskDrive> previousValues, out bool isFileSystemObjectDataAvailable, Logger LOG)
        {
            using (LOG.DebugCall("ReadDiskDrives"))
            {
                Dictionary<string, DiskDrive> DiskDrives = new Dictionary<string, DiskDrive>();
                isFileSystemObjectDataAvailable = true;
                while (dataReader.Read())
                {
                    DiskDrive dd = new DiskDrive();
                    if (!dataReader.IsDBNull(0)) dd.DriveLetter = dataReader.GetString(0).Trim();
                    if (!dataReader.IsDBNull(1)) dd.UnusedSize.Bytes = dataReader.GetDecimal(1);
                    if (!dataReader.IsDBNull(2)) dd.TotalSize.Bytes = dataReader.GetDecimal(2);
                    else { isFileSystemObjectDataAvailable = false; }
                    if (!dataReader.IsDBNull(3)) dd.DiskIdlePercentRaw = double.Parse(dataReader.GetString(3));
                    if (!dataReader.IsDBNull(4)) dd.DiskIdlePercentBaseRaw = double.Parse(dataReader.GetString(4));
                    if (!dataReader.IsDBNull(5)) dd.AverageDiskQueueLengthRaw = double.Parse(dataReader.GetString(5));
                    if (!dataReader.IsDBNull(6)) dd.Timestamp_Sys100ns = double.Parse(dataReader.GetString(6));
                    if (!dataReader.IsDBNull(7)) dd.AvgDisksecPerReadRaw = double.Parse(dataReader.GetString(7));
                    if (!dataReader.IsDBNull(8)) dd.AvgDisksecPerRead_Base = double.Parse(dataReader.GetString(8));
                    if (!dataReader.IsDBNull(9)) dd.AvgDisksecPerTransferRaw = double.Parse(dataReader.GetString(9));
                    if (!dataReader.IsDBNull(10)) dd.AvgDisksecPerTransfer_Base = double.Parse(dataReader.GetString(10));
                    if (!dataReader.IsDBNull(11)) dd.AvgDisksecPerWriteRaw = double.Parse(dataReader.GetString(11));
                    if (!dataReader.IsDBNull(12)) dd.AvgDisksecPerWrite_Base = double.Parse(dataReader.GetString(12));
                    if (!dataReader.IsDBNull(13)) dd.Frequency_Perftime = double.Parse(dataReader.GetString(13));
                    if (!dataReader.IsDBNull(14)) dd.DiskReadsPerSec_Raw = double.Parse(dataReader.GetString(14));
                    if (!dataReader.IsDBNull(15)) dd.DiskTransfersPerSec_Raw = double.Parse(dataReader.GetString(15));
                    if (!dataReader.IsDBNull(16)) dd.DiskWritesPerSec_Raw = double.Parse(dataReader.GetString(16));
                    if (!dataReader.IsDBNull(17)) dd.TimeStamp_PerfTime = double.Parse(dataReader.GetString(17));
                    if (!dataReader.IsDBNull(18)) dd.Timestamp_utc = dataReader.GetDateTime(18);



                    CalcDiskDrive(dd, previousValues);

                    if (LOG.IsVerboseEnabled)
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("Verbose Disk Data for " + dd.DriveLetter);
                        sb.AppendLine("Raw:");
                        sb.AppendLine("dd.UnusedSize.Bytes = " + dd.UnusedSize.Bytes);
                        sb.AppendLine("dd.TotalSize.Bytes = " + dd.TotalSize.Bytes);
                        sb.AppendLine("dd.DiskIdlePercentRaw = " + dd.DiskIdlePercentRaw);
                        sb.AppendLine("dd.DiskIdlePercentBaseRaw = " + dd.DiskIdlePercentBaseRaw);
                        sb.AppendLine("dd.AverageDiskQueueLengthRaw = " + dd.AverageDiskQueueLengthRaw);
                        sb.AppendLine("dd.Timestamp_Sys100ns = " + dd.Timestamp_Sys100ns);
                        sb.AppendLine("dd.AvgDisksecPerReadRaw = " + dd.AvgDisksecPerReadRaw);
                        sb.AppendLine("dd.AvgDisksecPerRead_Base = " + dd.AvgDisksecPerRead_Base);
                        sb.AppendLine("dd.AvgDisksecPerTransferRaw = " + dd.AvgDisksecPerTransferRaw);
                        sb.AppendLine("dd.AvgDisksecPerTransfer_Base = " + dd.AvgDisksecPerTransfer_Base);
                        sb.AppendLine("dd.AvgDisksecPerWriteRaw = " + dd.AvgDisksecPerWriteRaw);
                        sb.AppendLine("dd.AvgDisksecPerWrite_Base = " + dd.AvgDisksecPerWrite_Base);
                        sb.AppendLine("dd.Frequency_Perftime = " + dd.Frequency_Perftime);
                        sb.AppendLine("dd.DiskReadsPerSec_Raw = " + dd.DiskReadsPerSec_Raw);
                        sb.AppendLine("dd.DiskTransfersPerSec_Raw = " + dd.DiskTransfersPerSec_Raw);
                        sb.AppendLine("dd.DiskWritesPerSec_Raw = " + dd.DiskWritesPerSec_Raw);
                        sb.AppendLine("dd.TimeStamp_PerfTime = " + dd.TimeStamp_PerfTime);
                        sb.AppendLine("dd.Timestamp_utc = " + dd.Timestamp_utc);
                        sb.AppendLine("Calculated:");
                        sb.AppendLine("dd.DiskIdlePercent = " + dd.DiskIdlePercent);
                        sb.AppendLine("dd.AverageDiskQueueLength = " + dd.AverageDiskQueueLength);
                        sb.AppendLine("dd.AvgDiskSecPerRead (ms) = " + (dd.AvgDiskSecPerRead.HasValue ? dd.AvgDiskSecPerRead.Value.TotalMilliseconds.ToString() : ""));
                        sb.AppendLine("dd.AvgDiskSecPerTransfer (ms) = " + (dd.AvgDiskSecPerTransfer.HasValue ? dd.AvgDiskSecPerTransfer.Value.TotalMilliseconds.ToString() : ""));
                        sb.AppendLine("dd.AvgDiskSecPerWrite (ms) = " + (dd.AvgDiskSecPerWrite.HasValue ? dd.AvgDiskSecPerWrite.Value.TotalMilliseconds.ToString() : ""));
                        sb.AppendLine("dd.DiskReadsPerSec = " + dd.DiskReadsPerSec);
                        sb.AppendLine("dd.DiskTransfersPerSec = " + dd.DiskTransfersPerSec);
                        sb.AppendLine("dd.DiskWritesPerSec = " + dd.DiskWritesPerSec);
                        LOG.Verbose(sb.ToString());
                    }

                    DiskDrives.Add(dd.DriveLetter, dd);
                }
                return DiskDrives;
            }

        }

        internal static void CalcDiskDrive(DiskDrive dd, Dictionary<string, DiskDrive> previousValues)
        {
            if (previousValues != null && previousValues.ContainsKey(dd.DriveLetter))
            {
                double? delta; // throw away
                double? timestampDelta;
                double? perftimeDelta;
                TimeSpan? UTCdelta;
                bool breakOut;

                timestampDelta = OSMetrics.Calculate_Timer_Delta(
                    previousValues[dd.DriveLetter].Timestamp_Sys100ns,
                    dd.Timestamp_Sys100ns);

                perftimeDelta = OSMetrics.Calculate_Timer_Delta(
                    previousValues[dd.DriveLetter].TimeStamp_PerfTime,
                    dd.TimeStamp_PerfTime);

                UTCdelta = dd.Timestamp_utc - previousValues[dd.DriveLetter].Timestamp_utc;

                breakOut = !(timestampDelta.HasValue && timestampDelta.Value > 0);

                if (!breakOut)
                {
                    if (!UTCdelta.HasValue || (timestampDelta / 10000000) >= UTCdelta.Value.TotalSeconds * 1.25 || (perftimeDelta / dd.Frequency_Perftime) >= UTCdelta.Value.TotalSeconds * 1.25)
                    {
                        breakOut = true;

                        double expectedDelta = UTCdelta.HasValue ? UTCdelta.Value.TotalSeconds : 0.0;
                        LOG.Warn("Disk Metrics detected an invalid value for Drive " +
                            dd.DriveLetter +
                         " and disposed of related metrics.  TimeStamp_Sys100NS_Delta: " +
                         (timestampDelta / 10000000) +
                         " or TimeStamp_PerfTime_Delta: " +
                         (perftimeDelta / dd.Frequency_Perftime) +
                         " greater than UTCdeltaValue:" +
                         (expectedDelta * 1.25)
                         );
                    }
                }

                if (!breakOut)
                {
                    if (dd.DiskIdlePercentBaseRaw < 0)
                    {
                        dd.DiskIdlePercent = (long?)dd.DiskIdlePercentRaw;
                    }
                    else
                    {
                        double? diskIdle = OSMetrics.Calculate_PERF_100NSEC_TIMER(
                            previousValues[dd.DriveLetter].DiskIdlePercentRaw,
                            dd.DiskIdlePercentRaw,
                            out delta,
                            timestampDelta);//SQLDM-29069

                        if (previousValues[dd.DriveLetter].DiskIdlePercentRaw > dd.DiskIdlePercentRaw)
                        {
                            dd.DiskIdlePercent = null;
                        }
                        else
                        {
                            dd.DiskIdlePercent = diskIdle.HasValue ? Convert.ToInt64(diskIdle) : new long?();
                        }

                    }

                    double? queueLen = OSMetrics.Calculate_PERF_COUNTER_100NS_QUEUELEN_TYPE(
                                                                    previousValues[dd.DriveLetter].
                                                                    AverageDiskQueueLengthRaw,
                                                                    dd.AverageDiskQueueLengthRaw,
                                                                    out delta,
                                                                    timestampDelta);

                    dd.AverageDiskQueueLength = queueLen.HasValue ? Convert.ToInt64(queueLen) : new long?();

                    double? throughput =
                    OSMetrics.Calculate_PERF_AVERAGE_TIMER(previousValues[dd.DriveLetter].AvgDisksecPerReadRaw,
                                                           dd.AvgDisksecPerReadRaw,
                                                           previousValues[dd.DriveLetter].AvgDisksecPerRead_Base,
                                                           dd.AvgDisksecPerRead_Base,
                                                           dd.Frequency_Perftime);

                    dd.AvgDiskSecPerRead = throughput.HasValue ? TimeSpan.FromSeconds(throughput.Value) : new TimeSpan?();

                    throughput =
                    OSMetrics.Calculate_PERF_AVERAGE_TIMER(previousValues[dd.DriveLetter].AvgDisksecPerTransferRaw,
                                                           dd.AvgDisksecPerTransferRaw,
                                                           previousValues[dd.DriveLetter].AvgDisksecPerTransfer_Base,
                                                           dd.AvgDisksecPerTransfer_Base,
                                                           dd.Frequency_Perftime);

                    dd.AvgDiskSecPerTransfer = throughput.HasValue ? TimeSpan.FromSeconds(throughput.Value) : new TimeSpan?();

                    throughput =
                    OSMetrics.Calculate_PERF_AVERAGE_TIMER(previousValues[dd.DriveLetter].AvgDisksecPerWriteRaw,
                                                          dd.AvgDisksecPerWriteRaw,
                                                          previousValues[dd.DriveLetter].AvgDisksecPerWrite_Base,
                                                          dd.AvgDisksecPerWrite_Base,
                                                          dd.Frequency_Perftime);

                    dd.AvgDiskSecPerWrite = throughput.HasValue ? TimeSpan.FromSeconds(throughput.Value) : new TimeSpan?();

                    dd.DiskReadsPerSec =
                    OSMetrics.Calculate_PERF_COUNTER_COUNTER(previousValues[dd.DriveLetter].DiskReadsPerSec_Raw,
                                                          dd.DiskReadsPerSec_Raw,
                                                          out delta,
                                                          perftimeDelta,
                                                          dd.Frequency_Perftime);

                    dd.DiskTransfersPerSec =
                    OSMetrics.Calculate_PERF_COUNTER_COUNTER(previousValues[dd.DriveLetter].DiskTransfersPerSec_Raw,
                                                          dd.DiskTransfersPerSec_Raw,
                                                          out delta,
                                                          perftimeDelta,
                                                          dd.Frequency_Perftime);

                    dd.DiskWritesPerSec =
                    OSMetrics.Calculate_PERF_COUNTER_COUNTER(previousValues[dd.DriveLetter].DiskWritesPerSec_Raw,
                                                          dd.DiskWritesPerSec_Raw,
                                                          out delta,
                                                          perftimeDelta,
                                                          dd.Frequency_Perftime);

                }
            }
        }

        internal static void ReadTempdbStatistics(SqlDataReader dataReader, Snapshot logTarget, Logger LOG,
            TempdbStatistics previousTempdbStatistics, ref TempdbStatistics tempdbStatistics, ref List<TempdbFileActivity> fileActivityList,
            DateTime? previousDateTime, DateTime? currentDateTime)
        {
            using (LOG.DebugCall("InterpretTempdbSummary"))
            {
                try
                {
                    fileActivityList = new List<TempdbFileActivity>();

                    dataReader.NextResult();

                    while (dataReader.Read())
                    {
                        TempdbFileActivity tempdbFile = new TempdbFileActivity();
                        tempdbFile.DatabaseName = "tempdb";
                        if (!dataReader.IsDBNull(0)) tempdbFile.Filename = dataReader.GetString(0);
                        if (!dataReader.IsDBNull(1)) tempdbFile.Filepath = dataReader.GetString(1);
                        if (!dataReader.IsDBNull(2))
                        {
                            tempdbFile.FileSize = new FileSize();
                            tempdbFile.FileSize.Pages = dataReader.GetInt32(2);
                        }
                        if (!dataReader.IsDBNull(3))
                        {
                            tempdbFile.UserObjects = new FileSize();
                            tempdbFile.UserObjects.Pages = dataReader.GetInt64(3);
                        }
                        if (!dataReader.IsDBNull(4))
                        {
                            tempdbFile.InternalObjects = new FileSize();
                            tempdbFile.InternalObjects.Pages = dataReader.GetInt64(4);
                        }
                        if (!dataReader.IsDBNull(5))
                        {
                            tempdbFile.VersionStore = new FileSize();
                            tempdbFile.VersionStore.Pages = dataReader.GetInt64(5);
                        }
                        if (!dataReader.IsDBNull(6))
                        {
                            tempdbFile.MixedExtents = new FileSize();
                            tempdbFile.MixedExtents.Pages = dataReader.GetInt64(6);
                        }
                        if (!dataReader.IsDBNull(7))
                        {
                            tempdbFile.UnallocatedSpace = new FileSize();
                            tempdbFile.UnallocatedSpace.Pages = dataReader.GetInt64(7);
                        }

                        fileActivityList.Add(tempdbFile);
                    }

                    dataReader.NextResult();

                    tempdbStatistics = new TempdbStatistics();

                    while (dataReader.Read())
                    {
                        string label = "";
                        long? value = null;
                        if (!dataReader.IsDBNull(0)) label = dataReader.GetString(0);
                        if (!dataReader.IsDBNull(1)) value = dataReader.GetInt64(1);

                        if (label == "Version Generation rate (KB/s)" && value.HasValue)
                        {
                            tempdbStatistics.VersionStoreGenerationKilobytesRaw = value.Value;
                        }
                        else
                            if (label == "Version Cleanup rate (KB/s)" && value.HasValue)
                        {
                            tempdbStatistics.VersionStoreCleanupKilobytesRaw = value.Value;
                        }
                    }

                    if (previousTempdbStatistics != null)
                    {
                        tempdbStatistics.VersionStoreCleanupKilobytes =
                            ProbeHelpers.CalculateCounterDelta(
                                previousTempdbStatistics.VersionStoreCleanupKilobytesRaw,
                                tempdbStatistics.VersionStoreCleanupKilobytesRaw);
                        tempdbStatistics.VersionStoreGenerationKilobytes =
                            ProbeHelpers.CalculateCounterDelta(
                                previousTempdbStatistics.VersionStoreGenerationKilobytesRaw,
                                tempdbStatistics.VersionStoreGenerationKilobytesRaw);
                    }

                    tempdbStatistics.TimeStamp = currentDateTime;

                    if (previousDateTime != null)
                    {
                        tempdbStatistics.TimeDelta = currentDateTime.Value.Subtract(previousDateTime.Value);
                    }

                    dataReader.NextResult();

                    while (dataReader.Read())
                    {
                        string label = "";
                        decimal? value = null;
                        if (!dataReader.IsDBNull(1)) label = dataReader.GetString(1);
                        if (!dataReader.IsDBNull(0)) value = dataReader.GetDecimal(0);

                        if (value.HasValue)
                        {
                            TimeSpan waitTime = TimeSpan.FromMilliseconds((double)value.Value);
                            switch (label)
                            {
                                case "PFS":
                                    tempdbStatistics.TempdbPFSWaitTime = waitTime;

                                    break;
                                case "GAM":
                                    tempdbStatistics.TempdbGAMWaitTime = waitTime;
                                    break;
                                case "SGAM":
                                    tempdbStatistics.TempdbGAMWaitTime = waitTime;
                                    break;
                                default:
                                    LOG.Warn("Unexpected results from tempdb contention collector: " + label);
                                    break;

                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(logTarget,
                                                        LOG,
                                                        "Error interpreting Tempdb Summary Collector: {0}",
                                                        e, true);
                }
            }
        }

        #region Log Methods




        /// <summary>
        /// Parses the message portion of a SQL Agent Log line by searching for the end of the message number [###]
        /// </summary>
        internal static string ParseAgentMessage(string buffer)
        {
            try
            {
                int index = buffer.IndexOf("]");
                if ((index > 0) && (index + 1 < buffer.Length))
                {
                    return buffer.Substring(buffer.IndexOf("]") + 1).Trim(new char[] { ' ' });
                }
                else
                {
                    //if there is no message number brace, just return the whole line in case there is something useful in it.
                    return buffer;
                }
            }
            catch
            {
                return null;
            }
        }

        internal static void ParseMessage2000(DataRow dr, string buffer, List<Regex> errorLogRegexCritical, List<Regex> errorLogRegexWarning, List<Regex> errorLogRegexInfo, AdvancedAlertConfigurationSettings advancedSettings, Regex ErrorLogSeverity, MonitoredServerWorkload workload)
        {
            ParseSource2000(dr, ref buffer);
            if (buffer.Length > 5)
            {
                ParseMessageType(dr, buffer, errorLogRegexCritical, errorLogRegexWarning, errorLogRegexInfo, advancedSettings, ErrorLogSeverity, workload);
            }
            dr["Message"] = buffer;
        }

        private static void ParseSource2000(DataRow dr, ref string buffer)
        {
            int spaceLocation = buffer.IndexOf(" ");
            if (spaceLocation < 0)
            {
                dr["Source"] = buffer;
                buffer = "";
            }
            else
            {
                dr["Source"] = buffer.Substring(0, spaceLocation).Trim(new char[] { ' ' });
                buffer = buffer.Substring(spaceLocation, buffer.Length - spaceLocation).Trim(new char[] { ' ' });
            }
        }

        internal static void ParseMessageType(DataRow dr, string eventString, List<Regex> errorLogRegexCritical, List<Regex> errorLogRegexWarning, List<Regex> errorLogRegexInfo, AdvancedAlertConfigurationSettings advancedSettings, Regex ErrorLogSeverity, MonitoredServerWorkload workload)
        {
            using (LOG.DebugCall("ParseMessageType"))
            {
                MonitoredState severity = MonitoredState.None;

                if (workload.Thresholds[(int)Metric.ErrorLog].IsEnabled && ErrorLogSeverity.IsMatch(eventString))
                {
                    LOG.DebugFormat("Error Severity Match the Message: {0}", eventString);

                    int severityMatch = Convert.ToInt32(ErrorLogSeverity.Match(eventString).Value);
                    LOG.DebugFormat("The severity parse match is : {0}", severityMatch);
                    LOG.DebugFormat("Metric value : {0}", (int)Metric.ErrorLog);
                    LOG.DebugFormat("InfoThreshold value : {0},InfoThreshold type: {1}", workload.Thresholds[(int)Metric.ErrorLog].InfoThreshold.Value,
                                                            workload.Thresholds[(int)Metric.ErrorLog].InfoThreshold.Value.GetType());

                    if (workload.Thresholds[(int)Metric.ErrorLog].InfoThreshold.Enabled && (severityMatch >= Convert.ToInt32(workload.Thresholds[(int)Metric.ErrorLog].InfoThreshold.Value)))
                    {
                        severity = MonitoredState.Informational;
                    }

                    LOG.DebugFormat("WarningThreshold value : {0},WarningThreshold type: {1}", workload.Thresholds[(int)Metric.ErrorLog].WarningThreshold.Value,
                                                            workload.Thresholds[(int)Metric.ErrorLog].WarningThreshold.Value.GetType());

                    if (workload.Thresholds[(int)Metric.ErrorLog].WarningThreshold.Enabled && (severityMatch >= Convert.ToInt32(workload.Thresholds[(int)Metric.ErrorLog].WarningThreshold.Value)))
                    {
                        severity = MonitoredState.Warning;
                    }

                    LOG.DebugFormat("CriticalThreshold value : {0},CriticalThreshold type: {1}", workload.Thresholds[(int)Metric.ErrorLog].CriticalThreshold.Value,
                                                            workload.Thresholds[(int)Metric.ErrorLog].CriticalThreshold.Value.GetType());

                    if (workload.Thresholds[(int)Metric.ErrorLog].CriticalThreshold.Enabled && (severityMatch >= Convert.ToInt32(workload.Thresholds[(int)Metric.ErrorLog].CriticalThreshold.Value)))
                    {
                        severity = MonitoredState.Critical;
                    }

                    LOG.DebugFormat("The severity error is : {0}", severity.ToString());
                }
                if (workload.Thresholds[(int)Metric.ErrorLog].IsEnabled && severity < MonitoredState.Informational)
                    ErrorLogScanIsMatch(eventString, errorLogRegexCritical, errorLogRegexWarning, errorLogRegexInfo, advancedSettings, ref severity);

                dr["MessageType"] = severity;
                LOG.DebugFormat("The severity MessageType is : {0}", severity.ToString());
            }

        }


        internal static DateTime ParseSqlServerLogTimeStamp2000(ref string message, int spaceLocation)
        {
            try
            {
                DateTime timeStamp;
                Regex validFirstCharacters = new Regex(@"[0-9]", RegexOptions.IgnoreCase);
                //Send back as invalid if the string is not long enough for a date
                if (message.Length >= spaceLocation)
                {
                    //Send back as invalid if the date does not start at the very first character
                    if (!validFirstCharacters.IsMatch(message.Substring(0, 1)))
                    {
                        return System.DateTime.MinValue;
                    }
                    else
                    {
                        //If the time stamp is valid, return it and truncate the message to exclude it
                        timeStamp = Convert.ToDateTime(message.Substring(0, spaceLocation), new CultureInfo("en-US"));
                        message = message.Substring(spaceLocation).Trim(new char[] { ' ' });
                        return timeStamp;
                    }
                }
                else
                {
                    return System.DateTime.MinValue;
                }
            }
            catch
            {
                return System.DateTime.MinValue;
            }
        }

        /// <summary>
        /// Parses the timestamp from a SQL Agent Log line
        /// </summary>
        public static string ParseSqlAgentLogLineTimeStamp2000(string line)
        {
            try
            {
                //Matches specifically ####-##-## followed by any number of spaces, followed by ##:##:##
                Regex dateReg1 = new Regex(@"[0-9]{4,4}-[0-9]{2,2}-[0-9]{2,2}\s+[0-9]{1,2}:[0-9]{2,2}:[0-9]{2,2}");
                Match dateMatch = dateReg1.Match(line);
                if (dateMatch.Success)
                {
                    return dateMatch.Value;
                }
                else
                {
                    //Matches specifically ##/##/#### followed by any number of spaces, followed by ##:##:##
                    Regex dateReg2 = new Regex(@"([0-9]{2,2}/){2,2}[0-9]{4,4}\s+[0-9]{1,2}:[0-9]{2,2}:[0-9]{2,2}");

                    dateMatch = dateReg2.Match(line);
                    if (dateMatch.Success)
                    {
                        return dateMatch.Value;
                    }
                    else
                    {
                        try
                        {
                            return Convert.ToDateTime(line.Substring(0, line.IndexOf(" - ")).Trim(new char[] { ' ' }), new CultureInfo("en-US")).ToString();
                        }
                        catch
                        {
                            return null;
                        }
                    }

                }

            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Parses the error type for the SQL Agent Log from the indicator +, -, or ?
        /// </summary>
        internal static ErrorLogMessageType ParseMessageType(string buffer, Regex messageTypePattern)
        {

            switch (messageTypePattern.Match(buffer).Value)
            {
                case "!":
                    return ErrorLogMessageType.Error;

                case "+":
                    return ErrorLogMessageType.Warning;

                case "?":
                    return ErrorLogMessageType.Informational;

                default:
                    return ErrorLogMessageType.Informational;

            }
        }

        /// <summary>
        /// Parses the message number from a SQL Agent Log line by searching for the first instance of [###]
        /// </summary>
        internal static int ParseMessageNumber(string buffer, Regex messageNumberPattern)
        {
            return messageNumberPattern.IsMatch(buffer) ? Convert.ToInt32(messageNumberPattern.Match(buffer).Value) : 0;
        }

        internal static bool ErrorLogScanIsMatch(string eventString, List<Regex> errorLogRegexCritical, List<Regex> errorLogRegexWarning, List<Regex> errorLogRegexInfo, AdvancedAlertConfigurationSettings advancedSettings, ref MonitoredState severity)
        {
            using (LOG.DebugCall("ErrorLogScanIsMatch"))
            {
                if (advancedSettings != null)
                {
                    if (AlertFilterHelper.IsMatchValue(advancedSettings.LogIncludeMatchCritical, eventString))
                    {
                        severity = MonitoredState.Critical;
                        return true;
                    }
                    if (AlertFilterHelper.IsLikeValue(advancedSettings.LogIncludeLikeCritical, eventString))
                    {
                        severity = MonitoredState.Critical;
                        return true;
                    }
                }
                foreach (Regex regex in errorLogRegexCritical)
                {
                    if (regex.IsMatch(eventString))
                    {
                        severity = MonitoredState.Critical;
                        return true;
                    }
                }
                if (advancedSettings != null)
                {
                    if (AlertFilterHelper.IsMatchValue(advancedSettings.LogIncludeMatchWarning, eventString))
                    {
                        severity = MonitoredState.Warning;
                        return true;
                    }
                    if (AlertFilterHelper.IsLikeValue(advancedSettings.LogIncludeLikeWarning, eventString))
                    {
                        severity = MonitoredState.Warning;
                        return true;
                    }
                }
                foreach (Regex regex in errorLogRegexWarning)
                {
                    if (regex.IsMatch(eventString))
                    {
                        severity = MonitoredState.Warning;
                        return true;
                    }
                }

                if (advancedSettings != null)
                {
                    if (AlertFilterHelper.IsMatchValue(advancedSettings.LogIncludeMatchInfo, eventString))
                    {
                        severity = MonitoredState.Informational;
                        return true;
                    }
                    if (AlertFilterHelper.IsLikeValue(advancedSettings.LogIncludeLikeInfo, eventString))
                    {
                        severity = MonitoredState.Informational;
                        return true;
                    }
                }
                foreach (Regex regex in errorLogRegexInfo)
                {
                    if (regex.IsMatch(eventString))
                    {
                        severity = MonitoredState.Informational;
                        return true;
                    }
                }
                severity = MonitoredState.OK;
                return false;
            }
        }

        internal static Dictionary<string, Wait> ReadWaitStatistics(SqlDataReader dataReader, TimeSpan? timeDelta)
        {
            Dictionary<string, Wait> Waits = new Dictionary<string, Wait>();
            while (dataReader.Read())
            {
                string waitType;
                Wait wait = new Wait();
                if (!dataReader.IsDBNull(0))
                {
                    waitType = dataReader.GetString(0);
                    wait.WaitType = waitType;

                    if (!dataReader.IsDBNull(1)) wait.WaitingTasksCountTotal = dataReader.GetInt64(1);
                    if (!dataReader.IsDBNull(2)) wait.WaitTimeTotal = TimeSpan.FromMilliseconds(dataReader.GetInt64(2));
                    if (!dataReader.IsDBNull(3)) wait.MaxWaitTime = TimeSpan.FromMilliseconds(dataReader.GetInt64(3));
                    if (!dataReader.IsDBNull(4)) wait.ResourceWaitTimeTotal = TimeSpan.FromMilliseconds(dataReader.GetInt64(4));
                    wait.TimeDelta = timeDelta;


                    if (Waits.Count == 0 || !Waits.ContainsKey(waitType))
                    {
                        Waits.Add(waitType, wait);
                    }
                    else
                    {
                        Waits[waitType].WaitingTasksCountTotal += wait.WaitingTasksCountTotal;
                        Waits[waitType].WaitTimeTotal += wait.WaitTimeTotal;
                        Waits[waitType].MaxWaitTime += wait.MaxWaitTime;
                        Waits[waitType].ResourceWaitTimeTotal += wait.ResourceWaitTimeTotal;
                    }
                }
            }
            return Waits;
        }

        #endregion

        #region DataTable Helpers

        internal static DataTable GetTable(SqlDataReader dataReader)
        {
            return GetTable(dataReader, true);
        }

        internal static DataTable GetTable(SqlDataReader dataReader, bool convertDatesToLocalTime)
        {
            return GetTable(dataReader, convertDatesToLocalTime, null);
        }

        internal static DataTable GetTable(SqlDataReader dataReader, bool convertDatesToLocalTime, bool? isColumnReadOnly)
        {
            if (dataReader == null)
            {
                return null;
            }

            List<int> dateColumns = new List<int>();
            DataTable schemaTable = dataReader.GetSchemaTable();
            DataTable dataTable = new DataTable();

            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                string columnName = schemaTable.Rows[i]["ColumnName"].ToString();
                if (dataTable.Columns.Contains(columnName))
                {   // ensure we formulate a unique column name for each column in the data table
                    string btn = schemaTable.Rows[i]["BaseTableName"] as string;
                    if (btn != null && !dataTable.Columns.Contains(btn + "." + columnName))
                    {   // start by trying to prepend the base table name
                        columnName = btn + "." + columnName;
                    }
                    else
                    {   // resort to appending a number to the end
                        int n = 1;
                        while (dataTable.Columns.Contains(columnName + "_" + n))
                            n++;
                        columnName = columnName + "_" + n;
                    }
                }

                if (!dataTable.Columns.Contains(columnName))
                {
                    DataColumn dataColumn = new DataColumn();
                    dataColumn.ColumnName = columnName;
                    dataColumn.Unique = Convert.ToBoolean(schemaTable.Rows[i]["IsUnique"]);
                    dataColumn.AllowDBNull = Convert.ToBoolean(schemaTable.Rows[i]["AllowDBNull"]);
                    dataColumn.ReadOnly = isColumnReadOnly.HasValue
                                              ? isColumnReadOnly.Value
                                              : Convert.ToBoolean(schemaTable.Rows[i]["IsReadOnly"]);
                    dataColumn.DataType = schemaTable.Rows[i]["DataType"] as Type;
                    dataTable.Columns.Add(dataColumn);

                    if (convertDatesToLocalTime && dataColumn.DataType == typeof(DateTime))
                        dateColumns.Add(i);
                }
            }

            object[] itemArray = new object[dataReader.FieldCount];

            dataTable.BeginLoadData();
            while (dataReader.Read())
            {
                try
                {
                    dataReader.GetValues(itemArray);
                }
                catch (OverflowException oe)
                {
                    SafeLoadRow(dataReader, itemArray, dataTable);
                }
                if (dateColumns.Count > 0)
                {
                    foreach (int columnIndex in dateColumns)
                    {
                        if (itemArray[columnIndex] != DBNull.Value)
                        {
                            itemArray[columnIndex] = ((DateTime)itemArray[columnIndex]).ToLocalTime();
                        }
                    }
                }

                dataTable.LoadDataRow(itemArray, true);
            }
            dataTable.EndLoadData();

            return dataTable;
        }

        public static void SafeLoadRow(SqlDataReader reader, object[] itemArray, DataTable dataTable)
        {

            int n = itemArray.Length;
            for (int i = 0; i < n; i++)
            {
                try
                {
                    if (dataTable.Columns[i].DataType == typeof(decimal))
                    {
                        SqlDecimal sqlDecimal = reader.GetSqlDecimal(i);
                        itemArray[i] = sqlDecimal.IsNull ? (object)DBNull.Value : Convert.ToDecimal(sqlDecimal.ToDouble());
                    }
                    else
                        itemArray[i] = reader.GetValue(i);
                }
                catch (OverflowException e)
                {
                    itemArray[i] = DBNull.Value;
                }
            }
        }

        #endregion

        //START: SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- DataReader Helper Methods
        #region DataReader Helper

        internal static Int32 ToInt32(SqlDataReader dr, string prop)
        {
            int ordinal = dr.GetOrdinal(prop);
            if (!dr.IsDBNull(ordinal))
            {
                return Convert.ToInt32(dr.GetValue(ordinal));
            }
            return (0);
        }

        internal static Int64 ToInt64(SqlDataReader dr, string prop)
        {
            int ordinal = dr.GetOrdinal(prop);
            if (!dr.IsDBNull(ordinal))
            {
                return Convert.ToInt64(dr.GetValue(ordinal));
            }
            return (0);
        }

        internal static byte ToByte(SqlDataReader dr, string prop)
        {
            int ordinal = dr.GetOrdinal(prop);
            if (!dr.IsDBNull(ordinal))
            {
                return dr.GetByte(ordinal);
            }
            return (0);
        }


        internal static Decimal ToDecimal(SqlDataReader dr, string prop)
        {
            int ordinal = dr.GetOrdinal(prop);
            if (!dr.IsDBNull(ordinal))
            {
                return dr.GetDecimal(ordinal);
            }
            return (0);
        }

        internal static double ToDouble(SqlDataReader dr, string prop)
        {
            int ordinal = dr.GetOrdinal(prop);
            if (!dr.IsDBNull(ordinal))
            {
                double val;
                if (double.TryParse(dr[prop].ToString(), out val))
                {
                    return val;
                }
            }
            return (0);
        }

        internal static string ToString(SqlDataReader dr, string prop)
        {
            int ordinal = dr.GetOrdinal(prop);
            if (!dr.IsDBNull(ordinal))
            {
                return dr.GetString(ordinal);
            }
            return (string.Empty);
        }

        internal static DateTime ToDateTime(SqlDataReader dr, string prop)
        {
            int ordinal = dr.GetOrdinal(prop);
            if (!dr.IsDBNull(ordinal))
            {
                return dr.GetDateTime(ordinal);
            }
            return (DateTime.MinValue);
        }


        internal static bool ToBoolean(SqlDataReader dr, string prop)
        {
            int ordinal = dr.GetOrdinal(prop);
            if (!dr.IsDBNull(ordinal))
            {
                return dr.GetBoolean(ordinal);
            }
            return (false);
        }

        #endregion
        //END: SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- DataReader Helper Methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

        #region Extended Events Helper

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) - Converts LIKE expression to Regular expression.
        /// </summary>
        public static string GetRegexFromLikeExpression(string likeExpression)
        {
            return Regex.Replace(
                likeExpression,
                @"[%]|\[[^]]*\]|[^%_[]+",
                match =>
                {
                    if (match.Value == "%")
                    {
                        return ".*";
                    }
                    return Regex.Escape(match.Value);
                });
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Returns trace event id based on extended event name.
        /// </summary>
        public static int GetTraceEventId(string extendedEventName)
        {
            int eventId = -1;
            if (extendedEventNameToTraceEventIdMap.TryGetValue(extendedEventName, out eventId))
                return eventId;
            return -1;
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Truncates a DateTime to a specified resolution.
        /// </summary>
        /// <param name="date">The DateTime object to truncate</param>
        /// <param name="resolution">e.g. to round to nearest second, TimeSpan.TicksPerSecond</param>
        /// <returns>Truncated DateTime</returns>
        public static DateTime Truncate(DateTime date, long resolution)
        {
            return new DateTime(date.Ticks - (date.Ticks % resolution), date.Kind);
        }

        /// <summary>
        ///  SqlDM 10.2 (Anshul Aggarwal) - Used to compare plan_handle which are used as dictionary key.
        /// </summary>
        public class ByteArrayComparer : IEqualityComparer<byte[]>
        {
            public bool Equals(byte[] a, byte[] b)
            {
                return a.SequenceEqual(b);
            }

            public int GetHashCode(byte[] a)
            {
                return a.Aggregate(0, (acc, i) => unchecked(acc * 457 + i * 389));
            }
        }

        public static TimeSpan GetClosestTime(IAzureMonitorManagementClient azureMonitorClient,
            TimeSpan scheduledInterval, string metricDisplayName)
        {
            var closestTime = TimeSpan.FromMinutes(5);

            var metricDefinition = metricDefinitionsMap.ContainsKey(metricDisplayName)
                ? metricDefinitionsMap[metricDisplayName] ?? GetMetricDefinition(azureMonitorClient, metricDisplayName)
                : GetMetricDefinition(azureMonitorClient, metricDisplayName);

            if (!metricDefinitionsMap.ContainsKey(metricDisplayName) || metricDefinitionsMap[metricDisplayName] == null)
            {
                metricDefinitionsMap[metricDisplayName] = metricDefinition;
            }

            if (metricDefinition == null)
            {
                return closestTime;
            }

            var min = double.MaxValue;
            foreach (var metricAvailability in metricDefinition.MetricAvailabilities)
            {
                var timeGrain = metricAvailability.TimeGrain;
                if (timeGrain == null)
                {
                    continue;
                }

                // Ensure that the difference is minimum
                var currentMin = Math.Abs((scheduledInterval - timeGrain).Value.TotalSeconds);
                if (currentMin > min)
                {
                    break;
                }

                min = currentMin;
                closestTime = timeGrain.Value;
            }

            return closestTime;
        }

        private static Azure.MetricDefinition GetMetricDefinition(IAzureMonitorManagementClient azureMonitorClient, string metricDisplayName)
        {
            var metricDefinitions = azureMonitorClient.GetMetricDefinitions(azureMonitorClient.Configuration).GetAwaiter().GetResult().ToList();
            return metricDefinitions.FirstOrDefault(m => m.Name.LocalizedValue == metricDisplayName);
        }

        public static string GetMetricName(AzureManagementClient azureMonitorClient, string metricDisplayName)
        {
            var metricDefinition = metricDefinitionsMap.ContainsKey(metricDisplayName)
                ? metricDefinitionsMap[metricDisplayName] ?? GetMetricDefinition(azureMonitorClient, metricDisplayName)
                : GetMetricDefinition(azureMonitorClient, metricDisplayName);

            if (!metricDefinitionsMap.ContainsKey(metricDisplayName) || metricDefinitionsMap[metricDisplayName] == null)
            {
                metricDefinitionsMap[metricDisplayName] = metricDefinition;
            }

            if (metricDefinition == null)
            {
                return metricDisplayName;
            }

            return metricDefinition.Name.Value;
        }
    }

    #endregion
}
